// NvnCapture.cs — NVN-boundary frame capture per
// ~/projects/Pagentry/NVNCAP.md. (T1') per sera's kt[12]×3
// steer (day-32 ~02:00Z): capture at the NVN boundary so
// the same frame drives BOTH NvnVulkan (Linux/lavapipe
// debugging, ~1s loop vs 280s emulator-boot) AND NvnMetal
// (macOS/SwiftUI, the actual §9 target). Content-addressed
// textures (sha1(w||h||rgba)[:12]) shared across frames +
// RLE for the giant black RTs (1920×1080 → 24B).
//
// Hook: NvnVulkan.RecordDrawPass calls NvnCapture.Maybe(
// frameN, draws) right after snapshotting NvnLinux.Draws.
// Env: UMBRA_NVNCAP=<dir> + UMBRA_NVNCAP_FRAMES=N,N,… (or
// piggyback _dump3dWant for auto-relative).

using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace UmbraCore.Core;

public static unsafe class NvnCapture {
    static readonly string? _dir =
        Environment.GetEnvironmentVariable("UMBRA_NVNCAP");
    static readonly HashSet<int>? _frames =
        Environment.GetEnvironmentVariable("UMBRA_NVNCAP_FRAMES")
            is {} fv
            ? fv.Split(',').Select(int.Parse).ToHashSet()
            : null;
    // Content-addressed textures already written this run
    // (= skip re-hash+re-encode for duplicates).
    static readonly HashSet<string> _texWritten = new();
    static readonly HashSet<int> _shWritten = new();

    // (T6)×21 UMBRA_NVNCAP_ON3D=count[,interval[,thresh]]:
    // capture `count` frames starting from the first frame
    // with ≥`thresh` indexed draws (= 3D-onset), every
    // `interval` frames. 3D-onset frame# varies wildly
    // with fps (u761~f14140@52fps, u766~f15300@42fps,
    // u769~f37600@106fps — cutscene is wall-clock-paced)
    // ⟹ absolute UMBRA_NVNCAP_FRAMES is fragile. Defaults:
    // interval=200, thresh=50.
    static readonly int[]? _on3d =
        Environment.GetEnvironmentVariable("UMBRA_NVNCAP_ON3D")
            is {} ov
            ? ov.Split(',').Select(int.Parse).ToArray()
            : null;
    static int _on3dFirst = -1, _on3dDone;

    public static bool Want(int frameN) =>
        _dir != null && (_frames?.Contains(frameN) ?? false);

    public static void Maybe(int frameN,
            NvnLinux.DrawRecord[] draws) {
        if(_dir == null) return;
        // ON3D: detect onset by indexed-draw count, then
        // capture relative to that frame.
        if(_on3d != null) {
            var cnt = _on3d[0];
            var ival = _on3d.Length > 1 ? _on3d[1] : 200;
            var thr  = _on3d.Length > 2 ? _on3d[2] : 50;
            if(_on3dFirst < 0) {
                var nIdx = draws.Count(d => d.IdxCount > 0);
                if(nIdx < thr) return;
                _on3dFirst = frameN;
                $"[nvncap] ON3D onset @ f{frameN} (nIdx={nIdx} ≥ {thr}); capturing {cnt}× every {ival}f".Log();
            }
            var rel = frameN - _on3dFirst;
            if(_on3dDone >= cnt || rel % ival != 0) return;
            _on3dDone++;
        } else if(!Want(frameN)) return;
        try { Capture(frameN, draws); }
        catch(Exception ex) {
            $"[nvncap] f{frameN} ✗ {ex.GetType().Name}: {ex.Message}".Log();
        }
    }

    static void Capture(int frameN, NvnLinux.DrawRecord[] draws) {
        var t0 = DateTime.UtcNow;
        var fdir = Path.Combine(_dir!, $"frame-{frameN:d5}");
        var texDir = Path.Combine(_dir!, "textures");
        var shDir = Path.Combine(_dir!, "shaders");
        Directory.CreateDirectory(fdir);
        Directory.CreateDirectory(texDir);
        Directory.CreateDirectory(shDir);

        // ── 1. distinct shaders → copy raw Maxwell .bin ──
        var shSet = new HashSet<(int,int)>();  // (idx, type)
        foreach(var d in draws) {
            if(d.VsShIdx > 0) shSet.Add((d.VsShIdx, 1));
            if(d.FsShIdx > 0) shSet.Add((d.FsShIdx, 2));
        }
        var shSrc = Environment.GetEnvironmentVariable(
                "UMBRA_T3_SHDIR") ?? "/tmp/umbra-shaders";
        foreach(var (idx, ty) in shSet) {
            // ‡ type-suffix: VS=t1, FS=t2 per existing
            // _t3ShDir convention. Some VS are -t0 (= ‡
            // tess/geom variant); try both.
            var key = idx*4 + ty;
            if(!_shWritten.Add(key)) continue;
            foreach(var t in new[]{ ty==1 ? "t1" : "t2",
                                    ty==1 ? "t0" : "t2" }) {
                var src = $"{shSrc}/sh{idx:d4}-{t}.bin";
                if(!File.Exists(src)) continue;
                var dst = Path.Combine(shDir,
                    $"sh{idx:d4}-{t}.bin");
                if(!File.Exists(dst)) File.Copy(src, dst);
                // (T6)×42 per sera ·11090 #3: NO .spv copy.
                // NvnVulkan lifts on-the-fly from .bin via
                // MaxwellShader.Compiler.Compile(). The old
                // .spv-copy here propagated stale builds
                // forward through every recapture (verified:
                // Jun-14 .spv survived u773→u778→u779 while
                // the lifter source had two fixes Jun-15).
                break;
            }
        }

        // ── 2. distinct texIds across all TexHandles[] →
        //       resolve + hash + write content-addressed ──
        var texIds = new HashSet<int>();
        foreach(var d in draws) {
            if(d.TexHandles == null) continue;
            foreach(var h in d.TexHandles) {
                var ti = (int)(h >> 32);
                if(ti != 0) texIds.Add(ti);
            }
            // also d.TexHandle (last-bound, for non-indexed)
            var ti2 = (int)(d.TexHandle >> 32);
            if(ti2 != 0) texIds.Add(ti2);
        }
        var texPool = new List<object>();
        foreach(var ti in texIds.OrderBy(x => x)) {
            var t = NvnLinux.ResolveTex((ulong)ti << 32);
            if(t == null) continue;
            var rgba = NvnLinux.DecodeForUpload(t);
            if(rgba == null) {
                // No decode (RT never written, or unhandled
                // fmt). Synthesize solid-black at declared
                // dims so the slot isn't a hole. Hashes to
                // the same file as every other same-size
                // black (= the dedup working as designed).
                // (T6)×110 ×4 ‡v0×29th-PROPER diagnostic:
                // log WHY null (CpuPtr=0? Rgba was null?
                // fmt-unhandled?). Per ×110×2(m-3) at f32822
                // 0 BC-DecodeForUpload ran ⟹ all early-
                // returned; ×52nd showed `if(first)` log-
                // gate hides savanna re-registrations ⟹
                // can't see capture-time H-state from
                // existing log. This line answers per-tex.
                $"[nvncap] tex{ti}: rgba=null → black-synth. cpu=0x{t.CpuPtr:x} fmt=0x{t.Format:x} {t.Width}×{t.Height} pool=0x{t.PoolPtr:x}+0x{t.Offset:x}".Log();
                rgba = new byte[t.Width * t.Height * 4];
            }
            var hash = TexHash(t.Width, t.Height, rgba);
            // (T6)×67: target+depth so replay can create
            // the right imageView type (cube/3D/array vs
            // 2D). target=1 (2D) is the overwhelming
            // default; emit only when ≠1 to keep manifest
            // diff-friendly with capVer-3 captures (where
            // these fields don't exist ⟹ readers default
            // to 2D, which is what they did anyway).
            object entry = t.Target == 1 && t.Depth <= 1
                ? new { texId = ti, w = t.Width, h = t.Height,
                        fmt = t.Format, hash }
                : new { texId = ti, w = t.Width, h = t.Height,
                        fmt = t.Format, hash,
                        target = t.Target, depth = t.Depth };
            texPool.Add(entry);
            if(_texWritten.Add(hash))
                WriteTex(Path.Combine(texDir, hash+".tex"),
                    t.Width, t.Height, t.Format, rgba);
        }

        // ── 3. distinct vbuf regions (by VbCpu addr) ──
        var vbMap = new Dictionary<ulong,
            (int idx, int off, int len)>();
        using var vbuf = File.Create(
            Path.Combine(fdir, "vbuf.bin"));
        var vbufRegions = new List<object>();
        // (T6)×17: include stream-1 buffers in the region
        // pool. Same dedup-by-VbCpu key; stream-0/1 may
        // alias the same storage (= one entry) or be
        // separate (= two). The per-draw record stores
        // both region-indices @36/@40.
        void AddVb(ulong cpu, ulong size) {
            if(cpu == 0 || vbMap.ContainsKey(cpu)) return;
            var len = (int)Math.Min(size, 4<<20);
            var off = (int)vbuf.Position;
            vbuf.Write(new ReadOnlySpan<byte>(
                (void*)cpu, len));
            vbMap[cpu] = (vbufRegions.Count, off, len);
            vbufRegions.Add(new {
                key = $"0x{cpu:x}", off, len,
            });
        }
        foreach(var d in draws) {
            AddVb(d.VbCpu,  d.VbSize);
            AddVb(d.VbCpu1, d.VbSize1);
        }

        // ── 4. per-draw: ibuf snapshot + ubos concat +
        //       pack 256B DrawRec ──
        using var ibuf = File.Create(
            Path.Combine(fdir, "ibuf.bin"));
        using var ubos = File.Create(
            Path.Combine(fdir, "ubos.bin"));
        using var dbin = File.Create(
            Path.Combine(fdir, "draws.bin"));
        // (T6)×33: per-draw RT-set id (1B/draw, parallel
        // to draws.bin). Index into manifest.renderTargets.
        // 256B record is fully packed; rts.bin sidecar
        // keeps capVer=3 backward-readable (v2 readers
        // ignore the file).
        using var rts = File.Create(
            Path.Combine(fdir, "rts.bin"));
        // (T6)×62: per-draw blend-state (8B/draw, parallel
        // to draws.bin). DrawRecord.BlendKey: byte 0 =
        // ColorState enable mask (target-i = bit i);
        // bytes 1-6 = BlendState target-0's {srcRGB,
        // dstRGB, srcA, dstA, eqRGB, eqA} (NVN values).
        // capVer 4. capVer<4 readers ignore the file
        // (NvnReplay falls back to per-rtId heuristic).
        using var bld = File.Create(
            Path.Combine(fdir, "blend.bin"));
        // ext-tex (slots 8..39) only if any draw uses ≥8
        var needExt = draws.Any(d => d.TexHandles != null
            && d.TexHandles.Skip(8).Any(h => h != 0));
        using var dext = needExt ? File.Create(
            Path.Combine(fdir, "draws-ext.bin")) : null;
        // (T6)×106 ×1 ‡v0×27th-PROPER: attrs[6..15] →
        // attrs-ext.bin (40B/draw). u797 census: ×11+×1
        // 8-attr sigs (skinned-char [6]=0x2e@28 RGBA32F per
        // 61st + [7]=0x27@44 bone-idx). Min(count,6) loses
        // them ⟹ Vk feeds in_attr6/7=0 ⟹ bone-idx=0 ⟹ all
        // verts skin to bone-0 ⟹ stretched character (=
        // sera kt[12]×33 hairlines-cand). Gated needAext so
        // captures with no >6-attr draws don't write the
        // file (= replay-side reads rec[228..251] only,
        // back-compat with capVer≤6). capVer=7.
        var needAext = draws.Any(d => (d.Attribs?.Length ?? 0) > 6);
        using var aext = needAext ? File.Create(
            Path.Combine(fdir, "attrs-ext.bin")) : null;

        Span<byte> rec = stackalloc byte[256];
        foreach(var d in draws) {
            rec.Clear();
            // 0..43: scalars
            W32(rec, 0, d.N);
            W32(rec, 4, d.Prim);
            W32(rec, 8, d.First);
            W32(rec, 12, d.Count);
            W32(rec, 16, d.IdxType);
            W32(rec, 20, d.IdxCount);
            W32(rec, 24, d.BaseVtx);
            // ibuf snapshot
            int ibOff = 0, ibLen = 0;
            if(d.IdxCount > 0 && d.IdxCpu != 0) {
                var isz = d.IdxType==0?1:d.IdxType==1?2:4;
                ibLen = d.IdxCount * isz;
                ibOff = (int)ibuf.Position;
                ibuf.Write(new ReadOnlySpan<byte>(
                    (void*)d.IdxCpu, ibLen));
            }
            W32(rec, 28, ibOff);
            W32(rec, 32, ibLen);
            // vbuf region ref
            var vbIdx = vbMap.TryGetValue(d.VbCpu, out var vb)
                ? vb.idx : -1;
            W32(rec, 36, vbIdx);
            // (T6)×17: @40 was reserved=0; now vbR1 (= stream
            // -1's vbufRegion index, -1 if single-stream).
            // Old captures read @40=0 ⟹ NvnReplay treats 0
            // as "region 0" — gated by manifest.version≥2.
            var vbIdx1 = d.VbCpu1 != 0
                && vbMap.TryGetValue(d.VbCpu1, out var vb1)
                ? vb1.idx : -1;
            W32(rec, 40, vbIdx1);
            W32(rec, 44, d.VsShIdx);
            W32(rec, 48, d.FsShIdx);
            // viewport+scissor (8× int16)
            W16(rec, 52, d.VpX); W16(rec, 54, d.VpY);
            W16(rec, 56, d.VpW); W16(rec, 58, d.VpH);
            W16(rec, 60, d.ScX); W16(rec, 62, d.ScY);
            W16(rec, 64, d.ScW); W16(rec, 66, d.ScH);
            // ubos[16]: {off:u32, len:u16}
            for(var i = 0; i < 16; i++) {
                var u = d.Ubos?[i];
                if(u == null) {
                    W32(rec, 68+i*4, 0);
                    W16(rec, 132+i*2, 0);
                    continue;
                }
                W32(rec, 68+i*4, (int)ubos.Position);
                W16(rec, 132+i*2, u.Length);
                ubos.Write(u);
            }
            // texHandles[8] inline (slots 0..7)
            if(d.TexHandles != null)
                for(var i = 0; i < 8; i++)
                    W64(rec, 164+i*8, d.TexHandles[i]);
            // attribs[6] + stride[2] @228-255 (= the original
            // (T6)×17 layout). (T6)×102 ×3 ‡v0×27th-PROPER:
            // u797 census shows 8-attr signatures (×11+×1) =
            // skinned-char draws with [6]=0x2e@28 (RGBA32F per
            // ×40th) + [7]=0x27@44 (bone-idx). Min(count,6)
            // LOSES attrs[6..7] ⟹ Vk feeds in_attr6/7=0 ⟹
            // bone-idx=0 ⟹ all verts skin to bone-0's matrix
            // ⟹ stretched character (= sera kt[12]×33 "fucked
            // up character geometry" hairlines-cand). capVer=7
            // adds attribs[6..15] in draws-ext.bin @256+ (10×4B
            // = 40B; ext was 256B with th[8..39]@0-255 fully
            // used, so attrs land in a 2nd ext file). v0 =
            // simplest: bytes 256.. of the rec[256] are unused?
            // No — rec is Span<byte>[256]. ⟹ pack into ext's
            // unused tail: th[8..39]=32×8B=256B fills ext
            // entirely. ⟹ need draws-ext2.bin OR widen ext to
            // 320B. v0-conservative: keep 6-cap in rec[228..251]
            // for back-compat; write attrs[6..15] to a NEW
            // attrs-ext.bin (40B/draw). NvnReplay reads it if
            // present (capVer≥7).
            for(var i = 0; i < Math.Min(
                    d.Attribs?.Length ?? 0, 6); i++) {
                var a = d.Attribs![i];
                rec[228+i*4]   = (byte)a.StreamIdx;
                rec[228+i*4+1] = (byte)a.Format;
                W16(rec, 228+i*4+2, a.Offset);
            }
            for(var i = 0; i < Math.Min(
                    d.Streams?.Length ?? 0, 2); i++)
                W16(rec, 252+i*2, d.Streams![i].Stride);
            // (T6)×106 ×1 ‡v0×27th-PROPER: attrs[6..15] → aext
            // (40B/draw; same 4B-per-attr packing as rec[228]).
            // aext is 40B-per-draw fixed-stride regardless of
            // this draw's attr-count (= replay reads aebin
            // [di*40..di*40+40]; zero-fmt slots skipped same
            // as rec[228..251] reader). Most draws have ≤6
            // attrs ⟹ 40B of zeros ⟹ aext compresses well on
            // disk; the per-draw cost is the deterministic-
            // stride simplicity at replay-time.
            if(aext != null) {
                Span<byte> ae = stackalloc byte[40];
                ae.Clear();
                var nA = d.Attribs?.Length ?? 0;
                for(var i = 6; i < Math.Min(nA, 16); i++) {
                    var a = d.Attribs![i];
                    ae[(i-6)*4]   = (byte)a.StreamIdx;
                    ae[(i-6)*4+1] = (byte)a.Format;
                    W16(ae, (i-6)*4+2, a.Offset);
                }
                aext.Write(ae);
            }
            dbin.Write(rec);
            rts.WriteByte(d.RtId);
            // (T6)×62: 8B BlendKey per-draw → blend.bin
            bld.Write(BitConverter.GetBytes(d.BlendKey));
            // ext-tex slots 8..39
            if(dext != null) {
                Span<byte> ext = stackalloc byte[256];
                if(d.TexHandles != null)
                    for(var i = 8; i < 40; i++)
                        W64(ext, (i-8)*8, d.TexHandles[i]);
                dext.Write(ext);
            }
        }

        // ── 5. manifest.json ──
        var c = NvnLinux.LastClearColor;
        var manifest = new {
            // version 3 = (T6)×33 RTT: rts.bin sidecar
            // (1B RtId/draw) + renderTargets[] table.
            // version 2 = (T6)×17 multi-stream vbuf:
            // DrawRec @40 = vbR1 (stream-1 region idx;
            // was reserved=0 in v1).
            // version 4 = (T6)×62: blend.bin sidecar
            // (8B/draw NVN blend-state). v3 readers
            // ignore it; NvnVulkan falls back to per-
            // rtId heuristic when BlendKey=0.
            // (T6)×96 ×2 ‡v0×21st: copyOps[] = the frame's
            // CopyTextureToTexture calls, drawIdxBefore-tagged
            // so NvnReplay can insert vkCmdCopyImage at the
            // right position in the draw stream. capVer=5.
            // Filter to this-frame's calls (CopyOps accumulates
            // across frames like Draws does; this-frame's
            // start = where drawIdxBefore ≥ the frame's first
            // draw. Simpler: NvnVulkan.RecordDrawPass clears
            // both per-frame ⟹ CopyOps already frame-scoped
            // by the time Maybe() fires. ‡ Verify at first
            // u795 read).
            copyOps = NvnLinux.CopyOps
                .Select(c => new {
                    di = c.DrawIdxBefore,
                    src = c.SrcTid, dst = c.DstTid,
                    srcPtr = $"0x{c.SrcPtr:x}",
                    dstPtr = $"0x{c.DstPtr:x}",
                    w = c.W, h = c.H,
                    sx = c.Sx, sy = c.Sy,
                    dx = c.Dx, dy = c.Dy,
                }).ToArray(),
            // (T6)×100 ×1 ⚠-stub-(i): ClearDepthStencil per-
            // frame at draw-position. Replay = vkCmdClear
            // Attachments(depth=…) at di. capVer=6.
            clearOps = NvnLinux.ClearOps
                .Select(c => new {
                    di = c.DrawIdxBefore, depth = c.Depth,
                    dMask = c.DepthMask, stencil = c.Stencil,
                    sMask = c.StencilMask,
                }).ToArray(),
            version = 7, frameN,
            game = Path.GetFileNameWithoutExtension(
                Environment.GetEnvironmentVariable(
                    "UMBRA_GAME_SO") ?? "unknown"),
            clearColor = new[]{ c[0], c[1], c[2], c[3] },
            drawCount = draws.Length,
            W = 1280, H = 720,  // ‡ hardcoded; = NvnVulkan.W/H
            hasExtTex = needExt,
            shaders = shSet.OrderBy(s => s.Item1)
                .Select(s => new {
                    idx = s.Item1, type = s.Item2,
                    file = $"sh{s.Item1:d4}-t{(s.Item2==1?1:2)}.bin",
                }).ToList(),
            texPool,
            vbufRegions,
            // (T6)×33: RT-set table. Each draw's rts.bin
            // byte indexes this. texId=0 ⟹ game never
            // registered the RT in the texture pool (=
            // not sampled later, e.g. final-swap).
            renderTargets = NvnLinux.RtSigs
                .Select(s => new {
                    id = (int)s.Id, nC = s.NC,
                    colors = s.Colors.Select(a => new {
                        w = a.W, h = a.H, fmt = a.Fmt,
                        // texId resolved AT CAPTURE TIME
                        // (not sig-creation; game pool-
                        // registers RTs after first
                        // SetRenderTargets per u775).
                        texId = NvnLinux.HandleToTexId(a.Handle),
                    }).ToList(),
                    depth = s.Depth == null ? null : new {
                        w = s.Depth.W, h = s.Depth.H,
                        fmt = s.Depth.Fmt,
                        texId = NvnLinux.HandleToTexId(
                            s.Depth.Handle),
                    },
                }).ToList(),
        };
        File.WriteAllText(Path.Combine(fdir, "manifest.json"),
            JsonSerializer.Serialize(manifest,
                new JsonSerializerOptions { WriteIndented=true }));

        var dt = (DateTime.UtcNow - t0).TotalMilliseconds;
        ($"[nvncap] f{frameN}: {draws.Length} draws, "
            + $"{shSet.Count} sh, {texPool.Count} tex, "
            + $"{vbufRegions.Count} vbuf-regions, "
            + $"vbuf={vbuf.Length>>10}KB ibuf={ibuf.Length>>10}KB "
            + $"ubos={ubos.Length>>10}KB → {fdir} ({dt:f0}ms)")
            .Log();
    }

    // ── content-addressed texture write ──
    static string TexHash(int w, int h, byte[] rgba) {
        // sha1(le32(w) || le32(h) || rgba)[:12hex] per spec.
        // Dimension-prefix avoids 4×4-black colliding with
        // 1920×1080-black (same content, different shape).
        Span<byte> pre = stackalloc byte[8];
        BinaryPrimitives.WriteInt32LittleEndian(pre, w);
        BinaryPrimitives.WriteInt32LittleEndian(pre[4..], h);
        using var sha = IncrementalHash.CreateHash(
            HashAlgorithmName.SHA1);
        sha.AppendData(pre);
        sha.AppendData(rgba);
        var hash = sha.GetHashAndReset();
        return Convert.ToHexString(hash, 0, 6).ToLowerInvariant();
    }

    static void WriteTex(string path, int w, int h,
            int nvnFmt, byte[] rgba) {
        if(File.Exists(path)) return;  // cross-run dedup
        // Try RLE; pick whichever's smaller.
        var rle = RleEncode(rgba);
        var enc = rle.Length < rgba.Length ? (byte)1 : (byte)0;
        var body = enc == 1 ? rle : rgba;
        using var f = File.Create(path);
        Span<byte> hdr = stackalloc byte[16];
        W32(hdr, 0, 0x5845544e);  // 'NTEX'
        W16(hdr, 4, w); W16(hdr, 6, h);
        W16(hdr, 8, nvnFmt);
        hdr[10] = enc;
        W32(hdr, 12, rgba.Length);
        f.Write(hdr);
        f.Write(body);
    }

    static byte[] RleEncode(byte[] rgba) {
        // Pixel-run: {u32 count, u8 r,g,b,a} per spec. Bail
        // early if encoded > raw (= worst case 2×; non-
        // compressible content).
        var raw = rgba.Length;
        var ms = new MemoryStream(Math.Min(raw, 4096));
        Span<byte> run = stackalloc byte[8];
        var i = 0;
        while(i < raw) {
            var r=rgba[i]; var g=rgba[i+1];
            var b=rgba[i+2]; var a=rgba[i+3];
            var n = 1; i += 4;
            while(i < raw && rgba[i]==r && rgba[i+1]==g
                    && rgba[i+2]==b && rgba[i+3]==a) {
                n++; i += 4;
            }
            BinaryPrimitives.WriteUInt32LittleEndian(
                run, (uint)n);
            run[4]=r; run[5]=g; run[6]=b; run[7]=a;
            ms.Write(run);
            if(ms.Length >= raw) return rgba;  // give up
        }
        return ms.ToArray();
    }

    // ── LE pack helpers ──
    static void W32(Span<byte> b, int o, int v) =>
        BinaryPrimitives.WriteInt32LittleEndian(b[o..], v);
    static void W16(Span<byte> b, int o, int v) =>
        BinaryPrimitives.WriteInt16LittleEndian(
            b[o..], (short)v);
    static void W64(Span<byte> b, int o, ulong v) =>
        BinaryPrimitives.WriteUInt64LittleEndian(b[o..], v);
}
