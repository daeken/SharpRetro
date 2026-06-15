// NvnReplay.cs — load an NVNCAP frame-dir and drive it
// through NvnVulkan.{Init, Present} directly. (T1')×3 per
// sera's kt[12]×3 steer (NVNCAP.md). Kills the 280s
// emulator-boot loop ⟹ ~1s/iteration on the (c⁴⁵) bisect.
//
// Approach: zero refactor of RecordDrawPass. Populate the
// NvnLinux statics it reads (Draws, St, TexByPoolIdx,
// LastClearColor) from the capture, set UMBRA_SHADER_DIR
// to nvncap/shaders, call Init() + Present(0,0). The
// EXACT same RecordDrawPass/T3Pipe/EnsureTexBound code
// runs as under the emulator — only the source of the
// DrawRecord[]/tex-pool changes.
//
// Usage: dotnet UmbraCli.dll --replay /tmp/nvncap/frame-14600 [N]
//   (renders the frame N times — default 1; >1 for warm
//    pipeline-cache + STALE-thrash repro)
//   Output: /tmp/umbra-frame-{frameN}.ppm via DumpPpm.
//   All UMBRA_T3_* env knobs apply (DEPTHVIS_FS, SKIP_VS,
//   FS_OVERRIDE, DEPTH_OP, etc.) — same code path.

using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace UmbraCore.Core;

public static unsafe class NvnReplay {
    public static int Run(string frameDir, int repeat = 1) {
        var t0 = DateTime.UtcNow;
        var capDir = Path.GetFullPath(
            Path.Combine(frameDir, ".."));
        var shDir = Path.Combine(capDir, "shaders");
        var texDir = Path.Combine(capDir, "textures");

        // T3 reads UMBRA_SHADER_DIR for .spv lookup. Must be
        // set BEFORE Init() (InitPipeline reads it once).
        Environment.SetEnvironmentVariable(
            "UMBRA_SHADER_DIR", shDir);
        Environment.SetEnvironmentVariable("UMBRA_T3", "1");
        // Force frame-dump (Present checks _dumpFrames).
        // We'll set it to manifest.frameN below.

        $"[replay] {frameDir} (cap={capDir})".Log();

        // ── 1. manifest ──
        var m = JsonSerializer.Deserialize<JsonElement>(
            File.ReadAllText(
                Path.Combine(frameDir, "manifest.json")));
        var capVer = m.TryGetProperty("version", out var cv)
                   ? cv.GetInt32() : 1;
        var frameN = m.GetProperty("frameN").GetInt32();
        var drawCount = m.GetProperty("drawCount").GetInt32();
        var hasExt = m.TryGetProperty("hasExtTex", out var he)
                  && he.GetBoolean();
        var cc = m.GetProperty("clearColor");
        for(var i = 0; i < 4; i++)
            NvnLinux.LastClearColor[i] =
                cc[i].GetSingle();
        $"[replay] f{frameN}: {drawCount} draws, hasExt={hasExt}".Log();

        // ── 2. texPool → NvnLinux.InjectTex (St + TexByPoolIdx) ──
        var nTex = 0;
        foreach(var t in m.GetProperty("texPool")
                .EnumerateArray()) {
            var texId = t.GetProperty("texId").GetInt32();
            var w = t.GetProperty("w").GetInt32();
            var hh = t.GetProperty("h").GetInt32();
            var fmt = t.GetProperty("fmt").GetInt32();
            var hash = t.GetProperty("hash").GetString()!;
            var rgba = LoadTex(
                Path.Combine(texDir, hash + ".tex"));
            NvnLinux.InjectTex(texId, new NvnLinux.H {
                Width = w, Height = hh, Format = fmt,
                Rgba = rgba, Gen = 1,
            });
            nTex++;
        }
        $"[replay] {nTex} textures injected".Log();

        // ── 3. vbuf/ibuf/ubos → pinned native blocks ──
        // RecordDrawPass derefs d.VbCpu / d.IdxCpu as raw
        // ptrs (Buffer.MemoryCopy). Pin the whole files;
        // DrawRec offsets index into them.
        var vbufB = File.ReadAllBytes(
            Path.Combine(frameDir, "vbuf.bin"));
        var ibufB = File.ReadAllBytes(
            Path.Combine(frameDir, "ibuf.bin"));
        var ubosB = File.ReadAllBytes(
            Path.Combine(frameDir, "ubos.bin"));
        var vbufH = GCHandle.Alloc(vbufB, GCHandleType.Pinned);
        var ibufH = GCHandle.Alloc(ibufB, GCHandleType.Pinned);
        var vbufP = (ulong)vbufH.AddrOfPinnedObject();
        var ibufP = (ulong)ibufH.AddrOfPinnedObject();
        $"[replay] vbuf={vbufB.Length>>10}KB ibuf={ibufB.Length>>10}KB ubos={ubosB.Length>>10}KB pinned".Log();

        // vbufRegions[idx] → off (= base addr for d.VbCpu)
        var vbRegions = m.GetProperty("vbufRegions")
            .EnumerateArray()
            .Select(r => (
                off: r.GetProperty("off").GetInt32(),
                len: r.GetProperty("len").GetInt32()))
            .ToArray();

        // ── 4. draws.bin → DrawRecord[] ──
        var dbin = File.ReadAllBytes(
            Path.Combine(frameDir, "draws.bin"));
        var ext = hasExt ? File.ReadAllBytes(
            Path.Combine(frameDir, "draws-ext.bin")) : null;
        var draws = new NvnLinux.DrawRecord[drawCount];
        for(var i = 0; i < drawCount; i++) {
            var r = dbin.AsSpan(i*256, 256);
            var d = new NvnLinux.DrawRecord {
                N       = R32(r, 0),
                Prim    = R32(r, 4),
                First   = R32(r, 8),
                Count   = R32(r, 12),
                IdxType = R32(r, 16),
                IdxCount= R32(r, 20),
                BaseVtx = R32(r, 24),
                VsShIdx = R32(r, 44),
                FsShIdx = R32(r, 48),
                VpX = R16(r,52), VpY = R16(r,54),
                VpW = R16(r,56), VpH = R16(r,58),
                ScX = R16(r,60), ScY = R16(r,62),
                ScW = R16(r,64), ScH = R16(r,66),
                Ubos = new byte[16][],
            };
            // ibuf
            var ibOff = R32(r, 28);
            var ibLen = R32(r, 32);
            d.IdxCpu = ibLen > 0 ? ibufP + (ulong)ibOff : 0;
            // vbuf region (stream-0)
            var vbR = R32(r, 36);
            if(vbR >= 0 && vbR < vbRegions.Length) {
                var (off, len) = vbRegions[vbR];
                d.VbCpu = vbufP + (ulong)off;
                d.VbSize = (ulong)len;
                // VbGpu used as dedup-key in RecordDrawPass
                // (= same VbGpu ⟹ skip re-memcpy). Reuse the
                // region's base addr so dedup still works.
                d.VbGpu = d.VbCpu;
            }
            // (T6)×17 stream-1 vbuf region. manifest.version
            // ≥2 only (v1 captures have @40=0=region-0 which
            // would be wrong). When absent, NvnVulkan aliases
            // b1=b0 (closes vbuf-b1-NULL VUID regardless;
            // attr1-4 read constant-but-nonzero ⟹ r63b 91%
            // vs r44 49.6% even without real stream-1 data).
            if(capVer >= 2) {
                var vbR1 = R32(r, 40);
                if(vbR1 >= 0 && vbR1 < vbRegions.Length) {
                    var (off, len) = vbRegions[vbR1];
                    d.VbCpu1 = vbufP + (ulong)off;
                    d.VbSize1 = (ulong)len;
                    d.VbGpu1 = d.VbCpu1;
                }
            }
            // ubos[16]
            for(var k = 0; k < 16; k++) {
                var uo = R32(r, 68 + k*4);
                int ul = (ushort)R16(r, 132 + k*2);
                if(ul > 0)
                    d.Ubos[k] = ubosB[uo..(uo+ul)];
            }
            // texHandles[40]: 0..7 inline + 8..39 from ext
            var th = new ulong[40];
            for(var k = 0; k < 8; k++)
                th[k] = R64(r, 164 + k*8);
            if(ext != null) {
                var e = ext.AsSpan(i*256, 256);
                for(var k = 0; k < 32; k++)
                    th[8+k] = R64(e, k*8);
            }
            d.TexHandles = th;
            // d.TexHandle (= last-bound, scalar) — pick the
            // highest-numbered nonzero slot (= "last bound"
            // approximation; only used by non-indexed path).
            for(var k = 39; k >= 0; k--)
                if(th[k] != 0) { d.TexHandle = th[k]; break; }
            d.Tex = NvnLinux.ResolveTex(d.TexHandle);
            // attribs[6] + streams[2]
            var attrs = new List<NvnLinux.NvnAttrib>();
            for(var k = 0; k < 6; k++) {
                var fmt = r[228 + k*4 + 1];
                if(fmt == 0) continue;
                attrs.Add(new(
                    StreamIdx: r[228 + k*4],
                    Format:    fmt,
                    Offset:    (ushort)R16(r, 228 + k*4 + 2)));
            }
            d.Attribs = attrs.ToArray();
            d.Streams = new NvnLinux.NvnStream[] {
                new((ushort)R16(r, 252), 0),
                new((ushort)R16(r, 254), 0),
            };
            draws[i] = d;
        }
        var tLoad = (DateTime.UtcNow - t0).TotalMilliseconds;
        $"[replay] {drawCount} DrawRecords reconstructed ({tLoad:f0}ms load)".Log();

        // ── 5. Init Vulkan + drive Present() ──
        // Force frame-dump for our frameN (and frameN+k for
        // repeat>1) so DumpPpm fires.
        Environment.SetEnvironmentVariable("UMBRA_DUMP_FRAMES",
            string.Join(",", Enumerable.Range(0, repeat)
                .Select(k => (frameN+k).ToString())));
        // Present increments _frameN at top; we want the
        // FIRST Present to be frameN. ‡ _frameN is private;
        // simplest = call Present (frameN-1) times with
        // empty Draws first. Too slow for frameN~14600.
        // ⟹ Cheaper: just live with output filename =
        // /tmp/umbra-frame-001.ppm (= _frameN's first value)
        // and rename. v1-fix = expose _frameN setter.
        // Actually: UMBRA_DUMP_FRAMES wants the Present-N,
        // not the captured frameN. Set to "1,..,repeat".
        Environment.SetEnvironmentVariable("UMBRA_DUMP_FRAMES",
            string.Join(",", Enumerable.Range(1, repeat)));

        // (T1')×4 atlas-fallback: T3-block entry (@1694:
        // _t3DsVs!=0) depends on EnsureT3Sets which depends
        // on _atlasReady which depends on EnsureAtlasBound
        // which gates on NvnLinux.AtlasRgba!=null (= game's
        // first CopyBufferToTexture). No game ⟹ T3 never
        // opens (r4: rc=0 but nz=0%, T2-fallback). Inject a
        // 1×1 white atlas; EnsureAtlasBound runs normally,
        // _atlasReady=true, EnsureT3Sets fires, T3 opens.
        // ZERO NvnVulkan changes (= the SAME path as live).
        // ‡ v2 = capture the real atlas in manifest (= the
        // (c) candidate; needed for non-IDX_ONLY menu draws).
        NvnLinux.AtlasRgba = new byte[]{ 255,255,255,255 };
        NvnLinux.AtlasW = NvnLinux.AtlasH = 1;

        if(!NvnVulkan.Init()) {
            "[replay] ✗ NvnVulkan.Init failed".Log();
            return 1;
        }

        // (T1')×3×4 disc: r1-r3 segv'd identically at fault
        // =0x41800000_41800000 (= float 16.0 deref'd as ptr)
        // regardless of SKIP_VS/DEPTH ⟹ crash in non-indexed
        // legacy path (menu-atlas blit; atlas never uploaded
        // — no game). UMBRA_REPLAY_IDX_ONLY=1 filters to
        // IdxCount>0 (= 3D-only). If THAT works ⟹ legacy
        // path = the crash; non-indexed = (T1')×4.
        var idxOnly = Environment.GetEnvironmentVariable(
                "UMBRA_REPLAY_IDX_ONLY") != null;
        var use = idxOnly
            ? draws.Where(d => d.IdxCount > 0).ToArray()
            : draws;
        if(idxOnly)
            $"[replay] IDX_ONLY: {use.Length}/{draws.Length} draws (indexed-only)".Log();
        // (T6)×8 (T5) UMBRA_REPLAY_SWEEP=1: per-draw isolate.
        // Each iter K renders ONLY use[K] → replay-fNNNNN-iK.
        // ppm. One process, ~50ms/draw warm. + writes sweep-
        // index.csv {K, vsIdx, fsIdx, idxCount} so the post-
        // read can join nz% to shader. The (c⁴⁶) discriminator:
        // depthvis-sweep (which draws fill foreground?) ×
        // native-FS-sweep (which compute 0?) → the gap set.
        // (T6)×8×4 UMBRA_REPLAY_RANGE=A,B → render only
        // use[A..B] (inclusive). The prefix-bisect tool:
        // sweep showed k=110(fs801) outputs 278-color in
        // isolation; composite r25/r29 = 0% even with
        // DEPTH=0+NOBLEND ⟹ something STATEFUL in draws
        // 0-109 breaks fs801. RANGE bisects which prefix.
        if(Environment.GetEnvironmentVariable(
                "UMBRA_REPLAY_RANGE") is {} rg) {
            var ab = rg.Split(',').Select(int.Parse).ToArray();
            var a = ab[0]; var b = ab.Length>1 ? ab[1] : a;
            use = use.Skip(a).Take(b - a + 1).ToArray();
            $"[replay] RANGE=[{a},{b}]: {use.Length} draws".Log();
        }
        var sweep = Environment.GetEnvironmentVariable(
                "UMBRA_REPLAY_SWEEP") != null;
        if(sweep) {
            repeat = use.Length;
            using var sw = File.CreateText(
                $"/tmp/replay-f{frameN}-sweep.csv");
            sw.WriteLine("k,vs,fs,idxN,prim");
            for(var k = 0; k < use.Length; k++)
                sw.WriteLine($"{k},{use[k].VsShIdx},"
                    + $"{use[k].FsShIdx},{use[k].IdxCount},"
                    + $"{use[k].Prim}");
            $"[replay] SWEEP: {repeat} iters (1 draw each)".Log();
            // Re-set DUMP_FRAMES to cover all iters.
            Environment.SetEnvironmentVariable("UMBRA_DUMP_FRAMES",
                string.Join(",", Enumerable.Range(1, repeat)));
        }

        for(var iter = 0; iter < repeat; iter++) {
            // Re-fill Draws (Present clears it).
            lock(NvnLinux.Draws) {
                NvnLinux.Draws.Clear();
                if(sweep)
                    NvnLinux.Draws.Add(use[iter]);
                else
                    NvnLinux.Draws.AddRange(use);
            }
            var ti = DateTime.UtcNow;
            NvnVulkan.Present(0, iter % 3);
            var dt = (DateTime.UtcNow - ti).TotalMilliseconds;
            $"[replay] iter {iter+1}/{repeat}: Present → {dt:f0}ms".Log();
        }

        // Rename output to match captured frameN (so the
        // (c⁴⁵) bisect can compare against u743-f14600 etc).
        for(var k = 1; k <= repeat; k++) {
            var src = $"/tmp/umbra-frame-{k:d3}.ppm";
            var dst = $"/tmp/replay-f{frameN}-i{k}.ppm";
            if(File.Exists(src)) {
                if(File.Exists(dst)) File.Delete(dst);
                File.Move(src, dst);
                $"[replay] → {dst}".Log();
            }
        }

        vbufH.Free(); ibufH.Free();
        var tTotal = (DateTime.UtcNow - t0).TotalMilliseconds;
        $"[replay] done ({tTotal:f0}ms total)".Log();
        return 0;
    }

    // ── .tex loader (header + RLE/raw decode) ──
    static byte[]? LoadTex(string path) {
        if(!File.Exists(path)) return null;
        var d = File.ReadAllBytes(path);
        if(d.Length < 16
                || BinaryPrimitives.ReadUInt32LittleEndian(d)
                   != 0x5845544e) {
            $"[replay] ‡ bad .tex header: {path}".Log();
            return null;
        }
        var enc = d[10];
        var rawLen = BinaryPrimitives
            .ReadInt32LittleEndian(d.AsSpan(12));
        if(enc == 0) {
            // raw RGBA8 follows header
            return d[16..(16+rawLen)];
        }
        // RLE: {u32 count, u8 r,g,b,a} runs until rawLen filled
        var rgba = new byte[rawLen];
        int o = 16, p = 0;
        while(p < rawLen && o + 8 <= d.Length) {
            var cnt = BinaryPrimitives
                .ReadUInt32LittleEndian(d.AsSpan(o));
            byte r=d[o+4], g=d[o+5], b=d[o+6], a=d[o+7];
            o += 8;
            for(var k = 0u; k < cnt && p < rawLen; k++) {
                rgba[p++]=r; rgba[p++]=g;
                rgba[p++]=b; rgba[p++]=a;
            }
        }
        return rgba;
    }

    // ── LE read helpers ──
    static int R32(ReadOnlySpan<byte> b, int o) =>
        BinaryPrimitives.ReadInt32LittleEndian(b[o..]);
    static int R16(ReadOnlySpan<byte> b, int o) =>
        BinaryPrimitives.ReadInt16LittleEndian(b[o..]);
    static ulong R64(ReadOnlySpan<byte> b, int o) =>
        BinaryPrimitives.ReadUInt64LittleEndian(b[o..]);
}
