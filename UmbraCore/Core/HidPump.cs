// HID shared-memory pump. Populates the IHidServer.Memory shmem with a
// connected Player1 FullKey controller + per-frame ring entries with
// scripted button state. Game's nn::hid SDK reads this shmem directly
// (no per-frame IPC), so writing the right offsets is the whole input
// path.
//
// Layout reference = switchbrew (https://switchbrew.org/wiki/HID_Shared_Memory)
// + nn::hid SDK behavior; verified-for-understanding via ryujinx
// SharedMemory/Npad/* struct sizes (reference-only). Offsets are
// protocol-defined, not implementation-specific.
//
// shmem layout (40000B):
//   Npad section @ 0x9A00 = Array10<NpadState>, each 0x5000B.
//     NpadId→slot: Player1..8=0..7, Handheld=8, Other=9.
//   Per NpadState (NpadInternalState):
//     +0x00  StyleSet u32  (bit0=FullKey bit1=Handheld bit2=JoyDual …)
//     +0x04  JoyAssignmentMode u32
//     +0x08  FullKeyColor {Attr,Body,Btn} 3×u32
//     +0x14  JoyColor {Attr,LB,LBtn,RB,RBtn} 5×u32
//     +0x28  RingLifo<NpadCommonState> ×7  each 0x350B
//            (FullKey, Handheld, JoyDual, JoyL, JoyR, Palma, SystemExt)
//     +0x1758 RingLifo<SixAxisSensorState> ×6
//     +0x29B0 DeviceType u32, _ u32, SystemProperties u64,
//             SystemButtonProperties u32, BatteryLevel u32×3, …
//   RingLifo<T> = {unused u64, bufferCount u64, index u64, count u64}
//                 + 17× {SamplingNumber u64, T}
//   NpadCommonState = {SamplingNumber u64, Buttons u64, StickL i32×2,
//                      StickR i32×2, Attributes u32, _resv u32} = 40B
//   NpadButton.A = bit0; .B=bit1 …; .Plus=bit10
//   NpadAttribute.IsConnected = bit0; .IsWired = bit1
//
// v0 = scripted via UMBRA_HID env: comma-sep "A@60-65,B@80,Plus@90"
// → frame 60-65 hold A, frame 80 tap B, frame 90 tap Plus. Default
// (no env) = controller connected, no buttons. Pump.Tick(frame) called
// per-Present from NvnVulkan.

using System.Globalization;
using UmbraCore.Services.Nn.Hid;

namespace UmbraCore.Core;

public static unsafe class HidPump {
    const int NpadBase = 0x9A00, NpadStride = 0x5000;
    const int RingsBase = 0x28, RingStride = 0x350;
    const int RingHdrSz = 32, EntrySz = 48, RingCount = 17;
    // ‡ Footer (DeviceType/SystemProperties) offset: 0x28 + 7×0x350
    // + 6×SixAxisLifoSize. SixAxisSensorState = 8+8+12+12+12+12+36+4
    // = 104B → AtomicStorage = 112B → RingLifo = 32+17×112 = 1936 =
    // 0x790. So footer @ 0x28 + 7×0x350 + 6×0x790 = 0x28+0x1730+0x2D60
    // = 0x44B8. ‡ verify via SDK read (the game reads StyleSet at +0
    // and the ring at +0x28; footer matters for IsConnected detection
    // in some titles → write it in case).
    const int FooterOff = 0x44B8;

    static readonly (string name, ulong bit)[] BtnMap = [
        ("A", 1ul<<0), ("B", 1ul<<1), ("X", 1ul<<2), ("Y", 1ul<<3),
        ("L", 1ul<<6), ("R", 1ul<<7), ("ZL", 1ul<<8), ("ZR", 1ul<<9),
        ("Plus", 1ul<<10), ("Minus", 1ul<<11),
        ("DLeft", 1ul<<12), ("DUp", 1ul<<13), ("DRight", 1ul<<14), ("DDown", 1ul<<15),
    ];

    record struct Press(ulong Mask, int Lo, int Hi, bool DrawRel);
    static List<Press> _script = [];
    static long _samp = 1;
    static ulong _lastBtn;
    static bool _init, _dumped, _shimmed;
    static byte* _base;

    // Look up a runtime address by mangled name (Kernel.Symbols
    // is range→name; linear scan, one-shot).
    static ulong SymAddr(string mangled) =>
        Kernel.Symbols.FirstOrDefault(kv => kv.Value == mangled).Key.Start;

    // "0xADDR" or "*(0xADDR)" or "*(*(0xADDR)+OFF)" — chained
    // deref for following GOT/ptr-table indirections from the
    // outside.
    static ulong Deref(string s) {
        s = s.Trim();
        if(s.StartsWith("*(") && s.EndsWith(")")) {
            var inner = s[2..^1];
            // Allow "+0xOFF" suffix on the inner expr.
            var pi = inner.LastIndexOf('+');
            ulong off = 0;
            if(pi > 0 && inner.IndexOf('(', pi) < 0) {
                off = Convert.ToUInt64(inner[(pi+1)..].Trim(), 16);
                inner = inner[..pi];
            }
            var ia = Deref(inner) + off;
            return *(ulong*)ia;
        }
        return Convert.ToUInt64(s, 16);
    }

    // Full runtime image dump: contiguous code regions (per
    // Kernel.Symbols spans, gap >1MB = module boundary) → one
    // .bin per region + .regions index + .syms. = the input for
    // offline disasm/xrefs against the EXACT bytes the game has
    // loaded (post-reloc, post-PLT/GOT-resolve), without running
    // umbra per-query.
    static void DumpRuntimeImage(string baseP) {
        // Use MemoryManager.Regions (= actual mapped ranges, incl
        // PLT/GOT/data past where symbols end). Filter to regions
        // that contain at least one game symbol (= the loaded
        // modules; skips host-side heap allocations).
        var symLo = Kernel.Symbols.Keys.Min(k => k.Start);
        var symHi = Kernel.Symbols.Keys.Max(k => k.End);
        var runs = Kernel.MemoryManager.Regions
            .Where(r => r.Key + r.Value.Size > symLo && r.Key < symHi)
            .OrderBy(r => r.Key)
            .Select(r => (lo: r.Key, hi: r.Key + r.Value.Size))
            .ToList();
        // Merge adjacent (≤64KB gap — covers the patch-trampoline
        // region UmbraCore inserts between modules, which has no
        // game symbols and would otherwise be skipped; ICS::Leave
        // +0x0 etc. branch into it, so disasm/xref needs it).
        for(var i = runs.Count - 1; i > 0; i--)
            if(runs[i].lo - runs[i-1].hi <= 0x10000) {
                runs[i-1] = (runs[i-1].lo, runs[i].hi);
                runs.RemoveAt(i);
            }
        using var rw = new StreamWriter($"{baseP}.regions");
        long total = 0;
        foreach(var (lo, hi0) in runs) {
            var hi = (hi0 + 0xfff) & ~0xffful;
            var sz = (int)(hi - lo);
            try {
                var b = new byte[sz];
                new ReadOnlySpan<byte>((void*) lo, sz).CopyTo(b);
                File.WriteAllBytes($"{baseP}.{lo:x}.bin", b);
                rw.WriteLine($"{lo:x} {hi:x} {sz}");
                total += sz;
            } catch(Exception e) {
                $"[hid] image-dump {lo:x}..{hi:x} FAILED: {e.Message}".Log();
            }
        }
        using(var sw = new StreamWriter($"{baseP}.syms"))
            foreach(var ((lo, hi), nm) in Kernel.Symbols.OrderBy(kv => kv.Key.Start))
                sw.WriteLine($"{lo:x16} {hi-lo,6:x} {nm}");
        // (η) M4-v2: hook addresses (= our own UCO stubs that
        // game pfnc_* slots point at). NOT a contiguous region
        // (= CoreCLR JIT/exec-heap, scattered) — instead emit
        // addr→name so RuntimeImage overlay can resolve
        // pfnc-deref past `?unmapped`. Same .syms format
        // (size=0 = synthetic).
        using(var hw = new StreamWriter($"{baseP}.hooks")) {
            foreach(var (nm, (_, fp)) in Kernel.HookManager.Hooks)
                hw.WriteLine($"{fp:x16} {0,6:x} [hook] {nm}");
            foreach(var (nm, fp) in NvnLinux.Table)
                hw.WriteLine($"{(ulong)fp:x16} {0,6:x} [nvn-stub] {nm}");
        }
        $"[hid] runtime-image: {runs.Count} regions, {total/1e6:F1}MB → {baseP}.*".Log();
    }

    // drawFrameN = frames since first draw (= "menu is up" clock).
    // UMBRA_HID frames may be absolute (e.g. "A@60") OR draw-
    // relative ("A@d50" = 50 frames after first draw); the latter
    // is host-speed-stable.
    public static void Tick(int frameN, int drawFrameN) {
        if(!_init) {
            _init = true;
            var env = Environment.GetEnvironmentVariable("UMBRA_HID") ?? "";
            foreach(var part in env.Split(',', StringSplitOptions.RemoveEmptyEntries)) {
                // "A@60-65" or "A@60" (= single-frame tap)
                var at = part.IndexOf('@');
                if(at < 0) continue;
                var bn = part[..at].Trim();
                var rng = part[(at+1)..].Trim();
                var bit = BtnMap.FirstOrDefault(b =>
                    b.name.Equals(bn, StringComparison.OrdinalIgnoreCase)).bit;
                if(bit == 0) { $"[hid] unknown button '{bn}'".Log(); continue; }
                // "60-65" = absolute frameN. "d50-d60" = relative
                // to first-draw frame (host-speed-stable; menu
                // appearance is wall-clock-gated by async load,
                // so absolute frameN varies wildly with QUIET).
                static (int n, bool d) Pf(string s) =>
                    s.StartsWith('d') ? (int.Parse(s[1..]), true)
                                      : (int.Parse(s), false);
                (int n, bool d) lo, hi;
                var dash = rng.IndexOf('-');
                if(dash < 0) lo = hi = Pf(rng);
                else { lo = Pf(rng[..dash]); hi = Pf(rng[(dash+1)..]); }
                _script.Add(new(bit, lo.n, hi.n, lo.d));
            }
            $"[hid] script: {_script.Count} presses; mem @0x{IHidServer.Memory.Address:X}".Log();
            // One-shot symbol-table dump (from Rtld-loaded modules,
            // = the NSO's .dynsym; NxTranslate's .so is stripped so
            // static analysis can't get these). Used to find the
            // game's nn::hid SDK read fns for offset verification.
            if(Environment.GetEnvironmentVariable("UMBRA_DUMP_SYMS") is {} sp) {
                using var sw = new StreamWriter(sp);
                foreach(var ((lo, hi), nm) in Kernel.Symbols.OrderBy(kv => kv.Key.Start))
                    sw.WriteLine($"{lo:x16} {hi-lo,6:x} {nm}");
                $"[hid] dumped {Kernel.Symbols.Count} symbols → {sp}".Log();
            }
            // UMBRA_DUMP_FN="0xADDR,0xSIZE,/path" → raw bytes from
            // a runtime address (= the mapped game code; for
            // disasm-via-objdump when the .so's offset mapping
            // is awkward).
            // Handled per-frame below (so @N syntax can defer
            // the dump to a later frame).
        }
        // UMBRA_DUMP_FN="ADDR,SIZE,/path[@N];ADDR2,SIZE2,/path2[@N2];…"
        // — addr may be hex (0x…) OR a deref "*(0x…)+OFF" (chained;
        // for following GOT/ptr-table indirections). @N = dump at
        // frame N (default 1). One-shot per spec.
        // UMBRA_DUMP_FN="image,/path" — full runtime-image dump:
        // walks Kernel.Symbols, finds contiguous runs (gap >1MB =
        // new module), dumps each to /path.{lo:x}.bin + writes
        // /path.regions (lo,hi per line) + /path.syms.
        if(!_dumped && Environment.GetEnvironmentVariable("UMBRA_DUMP_FN") is {} dfp0) {
            if(dfp0.StartsWith("image,")) {
                _dumped = true;
                DumpRuntimeImage(dfp0[6..]);
            } else
            foreach(var spec0 in dfp0.Split(';')) {
                var atI = spec0.LastIndexOf('@');
                var dfN = atI > 0 ? int.Parse(spec0[(atI+1)..]) : 1;
                if(frameN < dfN) continue;
                var spec = atI > 0 ? spec0[..atI] : spec0;
                var parts = spec.Split(',');
                var fa = Deref(parts[0]);
                var fs = (int) Convert.ToUInt64(parts[1], 16);
                var fb = new byte[fs];
                new ReadOnlySpan<byte>((void*) fa, fs).CopyTo(fb);
                File.WriteAllBytes(parts[2], fb);
                $"[hid] dumped {fs}B @ 0x{fa:X} → {parts[2]} (frame {frameN})".Log();
            }
            if(!dfp0.Split(';').Any(s => {
                var i = s.LastIndexOf('@');
                return i > 0 && int.Parse(s[(i+1)..]) > frameN;
            })) _dumped = true;
        }
        // ── SHIM: force player↔pad assignment ────────────────
        // The game's GUI2Manager::ProcessInput2_ForStage gates on
        // IsValidControllerForPlayer / GetRawJPad, which read
        // NuPads's player→port map. Normally the controller-applet
        // populates that; our applet stubs no-op. Calling
        // NuPadMapPlayerToPort(player=0, port=0) directly forces
        // it. One-shot at frame 30 (= NuPads is init'd by then;
        // verified via NuPad output dump @f80 having btn data).
        // ‡ Hypothesis test — if A works after this, the wall =
        // controller-applet stubs; reverse out the natural path
        // once unblocked.
        // Fire shim at first-draw + 5 (= NuPads definitely init'd;
        // host-speed-stable). Was frameN>=30 which fires too early
        // under QUIET (~283fps → menu appears @f300 not @f50).
        if(!_shimmed && drawFrameN >= 5
           && Environment.GetEnvironmentVariable("UMBRA_SHIM_PAD") != null) {
            _shimmed = true;
            var fa = SymAddr("_Z20NuPadMapPlayerToPortii");
            if(fa != 0) {
                $"[hid] SHIM: NuPadMapPlayerToPort(0,0) @ 0x{fa:X}".Log();
                ((delegate* unmanaged<int, int, void>) fa)(0, 0);
                // Also player 1 → port 1 (some menus check player>=1).
                ((delegate* unmanaged<int, int, void>) fa)(1, 1);
                $"[hid] SHIM done".Log();
            } else {
                $"[hid] SHIM: _Z20NuPadMapPlayerToPortii not found".Log();
            }
        }

        var addr = IHidServer.Memory.Address;
        if(addr == 0) return;   // not yet mapped by game
        _base = (byte*) addr;

        // Compute current button mask from script.
        ulong btn = 0;
        foreach(var p in _script) {
            var fn = p.DrawRel ? drawFrameN : frameN;
            if(fn >= p.Lo && fn <= p.Hi) btn |= p.Mask;
        }

        var samp = ++_samp;
        // Per disasm of the game's nn::hid::detail::GetNpadStates
        // (FullKey) (via UMBRA_DUMP_FN): NpadId→slot is
        //   id 0..7 → slot 0..7; id 0x20 (Other) → slot 8;
        //   id 0x10 (Handheld) → slot 9; else id 0 → slot 0.
        // SDK only reads slots whose per-slot ring-ptr is set
        // (= those passed to SetSupportedNpadIdType). Write the
        // ones the game requested; default to {0, 0x10} if not
        // captured yet.
        // v0 perf: only write Player1 + the FIRST non-player ID
        // the game requested (0x20 here). Writing all 9 × 7×17
        // entries costs ~1ms/frame on host-coherent mem.
        var reqIds = IHidServer.SupportedNpadIds.Length > 0
            ? IHidServer.SupportedNpadIds : [0u, 0x10u];
        var ids = new[] { 0u }
            .Concat(reqIds.Where(i => i >= 0x10).Take(1))
            .ToArray();
        foreach(var id in ids) {
            var slot = id switch {
                <= 7 => (int) id,
                0x20 => 8,
                0x10 => 9,
                _ => 0,
            };
            // styleBit = which ring is "active" for this id.
            // FullKey for player slots, Handheld for handheld slot.
            var sb = id == 0x10 ? 1 : 0;
            WriteSlot(slot, sb, samp, btn);
        }

        // Log on btn EDGE only (press/release) — under QUIET
        // there are ~17K frames; per-5-frame log was the spam.
        if(btn != _lastBtn) {
            $"[hid] f{frameN} d{drawFrameN} btn=0x{btn:X} samp={samp}".Log();
            _lastBtn = btn;
        }
    }

    static void WriteSlot(int slot, int styleBit, long samp, ulong btn) {
        var p = _base + NpadBase + slot * NpadStride;
        // Header: StyleSet bit + JoyAssign=0 + colors=zero (game
        // doesn't gate on color attribute for connectedness).
        // StyleSet = ALL bits the game requested (it reads this
        // via GetNpadStyleSet to decide which GetNpadStates<T>
        // to call; report whatever it asked for).
        *(uint*)(p + 0x00) = IHidServer.SupportedStyleSet != 0
            ? IHidServer.SupportedStyleSet : 0x1F;
        *(uint*)(p + 0x04) = 0;                  // JoyAssignmentMode = Dual
        // Footer (‡ offset): DeviceType bit + SystemProperties.
        *(uint*)(p + FooterOff + 0)  = 1u << styleBit;          // DeviceType
        *(ulong*)(p + FooterOff + 8) = 0b1111ul << 11;          // ‡ IsAbxyAvailable etc (bits 11-14)
        *(uint*)(p + FooterOff + 0x14) = 2;                     // BatteryLevel[0]=Full-ish

        // FullKeyColor.Attribute=Ok (some games gate connectedness
        // on it). Body/Button colors don't matter for input.
        *(uint*)(p + 0x08) = 0;          // NpadColorAttribute.Ok = 0
        *(uint*)(p + 0x14) = 0;          // JoyColor.Attribute.Ok = 0
        // Write ALL 7 NpadCommonState rings (game's SDK may read
        // any of them per the StyleSet it requested; ryujinx writes
        // empty entries to all unused rings every frame for the
        // same reason — UpdateUnusedInputIfNotEqual). Active ring
        // gets the real button state; others get IsConnected only.
        // Per disasm: SDK reads bufferCount@+8 (used as modulus →
        // MUST be 17 not 0), index@+0x10, count@+0x18; reads N
        // entries from index downward, validates each entry's
        // AtomicStorage.SamplingNumber is stable across the
        // memcpy AND that consecutive entries' inner SamplingNumber
        // differ by exactly 1 (705c-7068). ⟹ Write the FULL ring
        // every tick (17 entries, sampN samp-16..samp), with
        // CURRENT-frame's btn in the latest entry only (= a
        // press lasts ≥1 sample; SDK button-edge logic handles
        // the rest). Earlier entries get prev-frame's btn (= 0
        // for v0; the script's frame-window is the hold).
        // All 7 rings. Game's NuPad layer calls GetNpadStyleSet
        // first (reads StyleSet@+0) then the matching
        // GetNpadStates<T> overload — but which one isn't yet
        // known (7 overloads in the SDK, each reads its own ring).
        // 2 slots × 7 rings × 17 entries = 238 writes/frame; fine.
        for(var ringIdx = 0; ringIdx < 7; ringIdx++) {
            var ringBtn = btn;
            var r = p + RingsBase + ringIdx * RingStride;
            *(ulong*)(r + 8)  = RingCount;                // bufferCount = 17
            var idx = (int)(samp % RingCount);
            *(ulong*)(r + 16) = (ulong) samp;             // index (= total writes; SDK does %bufferCount)
            *(ulong*)(r + 24) = (ulong) Math.Min(samp, RingCount); // count
            // Fill ALL 17 entries (samp-16 .. samp), latest at
            // (samp%17). Entry k holds sampN = samp-(latest-k mod 17).
            for(var k = 0; k < RingCount; k++) {
                var sN = samp - ((idx - k + RingCount) % RingCount);
                var e = r + RingHdrSz + k * EntrySz;
                *(ulong*)(e + 0)  = (ulong) Math.Max(sN, 0);   // AtomicStorage.SamplingNumber
                *(ulong*)(e + 8)  = (ulong) Math.Max(sN, 0);   // NpadCommonState.SamplingNumber
                // Latest entry gets ringBtn; older entries get 0
                // (= the press shows as a single transition at
                // the latest sample, which is what edge-detect
                // wants). ‡ For HOLD scripts (lo<hi), older entries
                // within [lo,hi] frame range should also have btn
                // set — but samp ticks 1:1 with frames, so the
                // ring-fill-with-history naturally carries the
                // hold across frames; it's the WITHIN-frame older
                // samples that get 0, which is fine (game polls
                // once per frame).
                *(ulong*)(e + 16) = (k == idx) ? ringBtn : 0;
                *(uint*)(e + 24) = 0; *(uint*)(e + 28) = 0;
                *(uint*)(e + 32) = 0; *(uint*)(e + 36) = 0;
                *(uint*)(e + 40) = 0b11;
                *(uint*)(e + 44) = 0;
            }
        }
    }
}
