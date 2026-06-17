namespace MaxwellShader;

using System.Diagnostics;
using System.Text.Json;

// (T6)×79 ×4: SPIR-V software rasterizer for value-level
// debugging of the full render chain. Per sera kt[12]×31
// (·11404, the overnight goal): "i'd love to see a working
// rasterizer that matches what we're currently seeing, so
// we can properly debug this shit. i know it'll be slow,
// but we both know it'll pay off instantly. especially if
// we log things along the way so we can run it once and
// then do analysis for a long while after." + her tiled-
// parallel tip + her "we're going to end up writing our
// own rasterizer in the next 24 hours" 😹.
//
// = the kt[22] §7-3rd-domain instrument completed: SpvEval
// (46th) is single-px; SpvRaster runs SpvEval over a tri/
// frame, doing the VS→interp→FS that the GPU does. The
// 3-way verify is then {SpvRaster, lavapipe-replay, sera's
// ref.png}.
//
// v0 SCOPE (this build):
//   - Non-indexed triangle-list (prim=4) only. #631-668's
//     post-process draws are all this shape (cnt∈{3,6,456,
//     1536}; #631 is 1 tri). Indexed (G-buf #55-218) = v1.
//   - Single vertex-attr stream, single vec3/vec4 attr at
//     offset 0 (= vs63's in_0; #631 fmt=0x22 R32G32B32F).
//     Multi-attr / NVN-fmt-decode = v1.
//   - Tex callback samples from in-memory RtBuf[texId]
//     (float[W*H*4]) bilinear, OR returns a per-binding
//     override, OR (0,0,0,1). Pre-loading RtBuf from PPM/
//     PFM dumps (= GPU-rendered intermediaries) is the
//     "match what we're currently seeing" path; running
//     #631-668 in sequence with each draw's output feeding
//     the next's RtBuf is the "correct sw-deferred" path.
//   - No depth-test/write yet (post-process draws have
//     depth-test off; G-buf needs it = v1). No blend (=
//     overwrite; the captured BlendKey is read but v0
//     ignores it; ‡-noted per kt[2]).
//   - Tile-parallel: 32×32 tiles, Parallel.For over tile-
//     index. Each tile evals FS for its covered-px. Shared
//     output buffer (distinct-px ⟹ no sync needed within
//     a draw). ‡ SpvEval.Eval re-parses the spv per-px;
//     for 2M px that's the bottleneck. v0 mitigates via
//     `step` (sample every Nth px, default 4 ⟹ 480×270);
//     v0.5 = split SpvEval into Parse()+Eval(prog,env).
//
// LOGGING (= sera's "run once, analyze long after"):
//   - Per-draw header: vs/fs shIdx, vert count, gl_Position
//     bbox, all VS-outputs at each vert.
//   - Per-draw footer: covered-px, FS oColor min/max/mean
//     per channel, ‡-note histogram.
//   - Per-RT after write: nz%, distinct, min/max.
//   - DumpRt(): write any RtBuf to .pfm (float, lossless)
//     + .ppm (8-bit, for show_image).
//   - TraceAt(draw#, px, py): full SpvEval Trace[id] at one
//     px (= the 46th's --trace, callable from here).

public class RtBuf {
    public int W, H;
    public float[] D;     // [W*H*4] RGBA float
    public RtBuf(int w, int h) { W=w; H=h; D=new float[w*h*4]; }
    public Span<float> Px(int x, int y) =>
        D.AsSpan((y*W+x)*4, 4);
    // Bilinear sample at normalized UV (clamp-to-edge).
    public void Sample(float u, float v, Span<float> outRgba) {
        var fx = Math.Clamp(u*W - 0.5f, 0, W-1);
        var fy = Math.Clamp(v*H - 0.5f, 0, H-1);
        var x0=(int)fx; var y0=(int)fy;
        var x1=Math.Min(x0+1,W-1); var y1=Math.Min(y0+1,H-1);
        var tx=fx-x0; var ty=fy-y0;
        for(var c=0;c<4;c++) {
            var p00=D[(y0*W+x0)*4+c]; var p10=D[(y0*W+x1)*4+c];
            var p01=D[(y1*W+x0)*4+c]; var p11=D[(y1*W+x1)*4+c];
            outRgba[c] = (p00*(1-tx)+p10*tx)*(1-ty)
                       + (p01*(1-tx)+p11*tx)*ty;
        }
    }
}

// One captured frame's data, parsed from the nvncap dir.
// Standalone re-parse (NOT via NvnReplay) so SpvRaster has
// no UmbraCore dependency and can run from MaxwellShader
// alone. Layout per (T6)×79×3(ii) at-source verification
// of NvnCapture writer @238-300 + NvnReplay reader @195-285.
public class FrameData {
    public string Dir;
    public byte[] Draws, Ubos, Vbuf, Ibuf, Rts, Blend, DrawsExt;
    public int N;
    public JsonElement Manifest;
    public (int off, int len)[] VbRegions;
    public Dictionary<int, (int w, int h, int fmt, int target,
                            string hash)> TexPool = new();
    // RT-attachment texId → (rtId, attIdx, w, h, fmt, isDepth)
    public Dictionary<int, (int rt, int att, int w, int h,
                            int fmt, bool depth)> RtTex = new();
    public FrameData(string dir) {
        Dir = dir;
        byte[] R(string n) => File.Exists($"{dir}/{n}")
            ? File.ReadAllBytes($"{dir}/{n}") : Array.Empty<byte>();
        Draws = R("draws.bin"); Ubos = R("ubos.bin");
        Vbuf = R("vbuf.bin"); Ibuf = R("ibuf.bin");
        Rts = R("rts.bin"); Blend = R("blend.bin");
        DrawsExt = R("draws-ext.bin");
        N = Draws.Length / 256;
        Manifest = JsonDocument.Parse(
            File.ReadAllText($"{dir}/manifest.json")).RootElement;
        var vr = Manifest.GetProperty("vbufRegions");
        VbRegions = new (int,int)[vr.GetArrayLength()];
        for(var i=0;i<VbRegions.Length;i++) {
            var e = vr[i];
            VbRegions[i] = (e.GetProperty("off").GetInt32(),
                            e.GetProperty("len").GetInt32());
        }
        if(Manifest.TryGetProperty("texPool", out var tp))
            foreach(var t in tp.EnumerateArray())
                TexPool[t.GetProperty("texId").GetInt32()] =
                    (t.GetProperty("w").GetInt32(),
                     t.GetProperty("h").GetInt32(),
                     t.GetProperty("fmt").GetInt32(),
                     t.TryGetProperty("target",out var jt)
                        ? jt.GetInt32() : 1,
                     t.GetProperty("hash").GetString()!);
        if(Manifest.TryGetProperty("renderTargets", out var rt))
            for(var i=0;i<rt.GetArrayLength();i++) {
                var r = rt[i];
                if(r.TryGetProperty("colors", out var cs))
                    for(var j=0;j<cs.GetArrayLength();j++) {
                        var c=cs[j];
                        RtTex[c.GetProperty("texId").GetInt32()]
                          = (i,j,c.GetProperty("w").GetInt32(),
                             c.GetProperty("h").GetInt32(),
                             c.GetProperty("fmt").GetInt32(),false);
                    }
                if(r.TryGetProperty("depth", out var dp)
                   && dp.ValueKind!=JsonValueKind.Null)
                    // ‡v0×13th: rt0/1/2 ALL have depth.texId=309.
                    // TryAdd ⟹ FIRST rt in manifest order owns
                    // the mapping (= rt0). For SpvRaster this
                    // doesn't matter (we share by texId — one
                    // RtBuf[309] regardless of which rt wrote
                    // it, which is the CORRECT semantics the
                    // GPU-side EnsureRtFb gets wrong).
                    RtTex.TryAdd(dp.GetProperty("texId").GetInt32(),
                        (i,-1,dp.GetProperty("w").GetInt32(),
                         dp.GetProperty("h").GetInt32(),
                         dp.GetProperty("fmt").GetInt32(),true));
            }
    }
    public ReadOnlySpan<byte> Rec(int i) =>
        Draws.AsSpan(i*256, 256);
    public byte RtId(int i) => i<Rts.Length ? Rts[i] : (byte)0;
    public ulong BlendKey(int i) =>
        (i+1)*8<=Blend.Length
            ? BitConverter.ToUInt64(Blend, i*8) : 0;
    // Per-draw cbuf: stage∈{0=VS,1=FS}, slot∈[0,7] →
    // captured Ubos[stage*8+slot] (= NVN slot K → Maxwell
    // c[K+3] per the +3 mapping verified at-data ×76×3(j)).
    public byte[] Ubo(int draw, int stage, int slot) {
        var r = Rec(draw);
        var k = stage*8 + slot;
        var off = BitConverter.ToUInt32(r.Slice(68+k*4,4));
        var len = BitConverter.ToUInt16(r.Slice(132+k*2,2));
        return len>0 ? Ubos.AsSpan((int)off,len).ToArray()
                     : Array.Empty<byte>();
    }
}

public class RasterOptions {
    // Sample every Nth pixel in each axis (default 4 ⟹
    // 480×270 effective for 1920×1080). step=1 = full-res.
    // Per sera's tip, tiles parallelize regardless; step
    // is the v0 mitigation for SpvEval re-parsing per-px.
    public int Step = 4;
    public int TileSize = 32;
    // c[1]/c[2] (= the not-captured driver/compiler cbufs).
    // null ⟹ 1.0 (= C1_ONES baseline, matches GPU). Per-
    // shader override (key=fsShIdx) for the per-shader-
    // compiler-const hypothesis (×76's ‡v0×11th-PROPER).
    public float[]? C1, C2;
    public Dictionary<int, float[]> C1PerSh = new();
    // Per-binding tex override (= --tex equiv). Wins over
    // RtBuf lookup. For stubbing (e.g. shadow b16=1.0).
    public Dictionary<uint, float[]> TexOverride = new();
    // (T6)×85: per-texId constant override (= when texPool
    // entry isn't loaded but its value is known-constant,
    // e.g. tex256=(0,0,0,0) tex314=(1,1,1,1) per ×84×1-cont
    // -2 at-bytes). Lower priority than RtBufs (loaded)
    // and TexOverride (per-binding).
    public Dictionary<int, float[]> TexDefault = new();
    // Pre-loaded RtBuf by texId (= GPU-rendered intermediary
    // dumps fed in). The "match what we're seeing" path =
    // pre-load rt1.c[0/1/2]+depth from r153-gbuf-* dumps.
    public Dictionary<int, RtBuf> RtBufs = new();
    // Where compiled .spv live (or .bin to compile fresh).
    // (T6)×85: also where sh{N}-t{T}.cb1.bin live (= the
    // per-shader Maxwell c[1] section captured by NvnLinux
    // at ProgramSetShaders, dataGpu @ align_up(end,256);
    // 49th). auto-cb1 loads from here ⟹ SpvRaster's c[1]
    // matches GPU's automatically per-shader (= replaces
    // the --c1sh manual override for the common case).
    public string ShaderDir = "/tmp/umbra-shaders";
    // ‡ Dref compare-op. v0 = LESS (= lit if shadow-map
    // depth at coord < ref-depth). For reverse-Z engines
    // = GREATER. Flip via env if r156d shadow-stripes
    // invert vs SpvRaster.
    public bool DrefGreater = false;
    // Per-draw stats line.
    public Action<string> Log = Console.WriteLine;
    public bool LogVsVerts = true;
}

public static class SpvRaster {
    // ── Run a range of draws. Returns the RtBufs dict
    // (input ones + any RTs written by the draws). ──
    public static Dictionary<int, RtBuf> Run(
            FrameData fd, int lo, int hi, RasterOptions opt) {
        var bufs = opt.RtBufs;
        for(var di=lo; di<=hi && di<fd.N; di++) {
            try { RunDraw(fd, di, bufs, opt); }
            catch(Exception ex) {
                opt.Log($"[rast] #{di} ⚠ EXCEPTION: {ex.Message}");
                opt.Log($"  {ex.StackTrace?.Split('\n')[0]}");
            }
        }
        return bufs;
    }

    public static void RunDraw(FrameData fd, int di,
            Dictionary<int, RtBuf> bufs, RasterOptions opt) {
        var r = fd.Rec(di);
        var prim  = BitConverter.ToInt32(r.Slice(4,4));
        var first = BitConverter.ToInt32(r.Slice(8,4));
        var cnt   = BitConverter.ToInt32(r.Slice(12,4));
        var idxCnt= BitConverter.ToInt32(r.Slice(20,4));
        var vbIdx = BitConverter.ToInt32(r.Slice(36,4));
        var vsIdx = BitConverter.ToInt32(r.Slice(44,4));
        var fsIdx = BitConverter.ToInt32(r.Slice(48,4));
        var vpW   = BitConverter.ToUInt16(r.Slice(56,2));
        var vpH   = BitConverter.ToUInt16(r.Slice(58,2));
        var stride= BitConverter.ToUInt16(r.Slice(252,2));
        var rtId  = fd.RtId(di);
        var bk    = fd.BlendKey(di);
        // ‡ v0: indexed-draw and non-tri-list not handled.
        // kt[2] fail-fast: log + skip (NOT throw — let the
        // chain continue past unsupported draws).
        if(idxCnt > 0) {
            opt.Log($"[rast] #{di} rt{rtId} vs{vsIdx}/fs{fsIdx} ‡SKIP indexed (idxCnt={idxCnt}; v1)");
            return;
        }
        if(prim != 4) {
            opt.Log($"[rast] #{di} rt{rtId} vs{vsIdx}/fs{fsIdx} ‡SKIP prim={prim} (≠4 tri-list; v1)");
            return;
        }
        // ── Compile VS+FS fresh (kt[39] currency) ──
        var spvVs = CompileSh(opt.ShaderDir, vsIdx, 1);
        var spvFs = CompileSh(opt.ShaderDir, fsIdx, 2);
        if(spvVs==null || spvFs==null) {
            opt.Log($"[rast] #{di} ‡SKIP missing sh{vsIdx:d4}-t1/sh{fsIdx:d4}-t2");
            return;
        }
        // ── Load verts from vbuf (v0: single vec4 attr@0,
        // R32G32B32F ⟹ .w=1) ──
        var verts = new float[cnt][];
        if(vbIdx >= 0 && vbIdx < fd.VbRegions.Length
                && stride > 0) {
            var (vo, vl) = fd.VbRegions[vbIdx];
            var attFmt = r[228+1];   // attr[0].Format
            // ‡ v0 assumes attr[0]@offset0; comp-count from
            // fmt: 0x22≈vec3, 0x24≈vec4 (NVN R32G32B32{A32}_
            // SFLOAT, ‡-inferred). w defaults 1.
            var nC = attFmt switch { 0x24=>4, 0x22=>3,
                0x20=>2, 0x1e=>1, _=>4 };
            for(var v=0;v<cnt;v++) {
                verts[v] = new float[]{0,0,0,1};
                for(var c=0;c<nC && c<4;c++)
                    verts[v][c] = BitConverter.ToSingle(
                        fd.Vbuf, vo + (first+v)*stride + c*4);
            }
        } else {
            // ‡ no vbuf (= VS uses gl_VertexIndex). v0:
            // synthesize a fullscreen-tri (NOT what vs63
            // uses, but some VS might).
            for(var v=0;v<cnt;v++) verts[v]=new float[]{0,0,0,1};
            opt.Log($"[rast] #{di} ‡ no vbuf (vbIdx={vbIdx} stride={stride}); verts=zero");
        }
        // ── VS eval per-vert ──
        var ubo = new byte[2][][];
        for(var st=0;st<2;st++) {
            ubo[st] = new byte[8][];
            for(var sl=0;sl<8;sl++)
                ubo[st][sl] = fd.Ubo(di, st, sl);
        }
        // (T6)×85 ×2: auto-load per-shader cb1 from
        // sh{N}-t{T}.cb1.bin (= same data NvnVulkan binds
        // on GPU since 49th). C1PerSh / C1 still wins as
        // explicit-override (for A/B testing). VS+FS each
        // get their own (vs63 cb1=zero; sh0417={2.0079};
        // sh0111={1,0.005,0.99,1e-7}; sh0244=filmic).
        var cb1Vs = LoadCb1(opt.ShaderDir, vsIdx, 1);
        var cb1Fs = opt.C1PerSh.TryGetValue(fsIdx, out var c1o)
            ? FloatsToBytes(c1o)
            : LoadCb1(opt.ShaderDir, fsIdx, 2);
        var c1Fallback = opt.C1;
        Func<int,uint,uint,uint,uint,float> cbuf =
            (stage, set, bind, v4i, cmp) => {
            // set: VS=0, FS=2 per SpirvEmit. bind = c[bind].
            // bind∈[3,10] → captured Ubos[stage][bind-3].
            // bind∈{1,2} → cb1.bin / C1 / C2 / 1.0 fallback.
            if(bind==1) {
                var cb1d = stage==0 ? cb1Vs : cb1Fs;
                var off = (int)(v4i*16+cmp*4);
                if(cb1d != null && off+4 <= cb1d.Length)
                    return BitConverter.ToSingle(cb1d, off);
                var w = (int)(v4i*4+cmp);
                return c1Fallback!=null
                    ? (w<c1Fallback.Length?c1Fallback[w]:0f)
                    : 1f;
            }
            if(bind==2) {
                var w = (int)(v4i*4+cmp);
                return opt.C2!=null
                    ? (w<opt.C2.Length?opt.C2[w]:0f) : 1f;
            }
            if(bind>=3 && bind<=10) {
                var u = ubo[stage][bind-3];
                var off = (int)(v4i*16 + cmp*4);
                return off+4<=u.Length
                    ? BitConverter.ToSingle(u, off) : 0f;
            }
            return 0f;
        };
        var vsOut = new SpvEvalResult[cnt];
        for(var v=0;v<cnt;v++) {
            var env = new SpvEvalEnv {
                BuiltIn = {
                    [42] = new[]{(float)v},  // VertexIndex
                    [43] = new[]{0f},        // InstanceIndex
                },
                In = {
                    [(0,0)]=verts[v][0], [(0,1)]=verts[v][1],
                    [(0,2)]=verts[v][2], [(0,3)]=verts[v][3],
                },
                Cbuf = (s,b,i,c) => cbuf(0, s,b,i,c),
                Tex = (s,b,c,k,l,d) => new[]{0f,0f,0f,1f},
            };
            vsOut[v] = SpvEval.Eval(spvVs, env);
        }
        // gl_Position → screen (Y-flip for OriginUpperLeft).
        // ‡ v0 viewport: use VpW/VpH at (0,0); ignore VpX/Y.
        var W = (int)vpW; var H = (int)vpH;
        if(W==0 || H==0) { W=1920; H=1080; }
        var sv = new (float x, float y, float z, float iw)[cnt];
        for(var v=0;v<cnt;v++) {
            var p = vsOut[v].Position;
            var iw = p[3]!=0 ? 1f/p[3] : 1f;
            sv[v] = ((p[0]*iw+1)*0.5f*W,
                     (1-p[1]*iw)*0.5f*H,
                     p[2]*iw, iw);
        }
        // ── Output target: rt{rtId}.c[0] only for v0 (= the
        // primary attachment; multi-MRT = v1). Find its texId
        // from manifest.renderTargets[rtId].colors[0]. ──
        var rtSig = fd.Manifest.GetProperty("renderTargets")[rtId];
        var outTexId = rtSig.GetProperty("colors")[0]
                            .GetProperty("texId").GetInt32();
        if(!bufs.TryGetValue(outTexId, out var outBuf))
            bufs[outTexId] = outBuf = new RtBuf(W, H);
        // ── Tex callback: resolve binding → slot → texId
        // (via TexHandles[8] @164 + ext @DrawsExt for 8..39)
        // → RtBuf bilinear OR override OR (0,0,0,1). ──
        var th = new int[40];
        for(var k=0;k<8;k++)
            th[k] = (int)(BitConverter.ToUInt64(
                r.Slice(164+k*8,8)) >> 32);
        if(fd.DrawsExt.Length >= (di+1)*256) {
            var e = fd.DrawsExt.AsSpan(di*256, 256);
            for(var k=0;k<32;k++)
                th[8+k] = (int)(BitConverter.ToUInt64(
                    e.Slice(k*8,8)) >> 32);
        }
        // ── Header log ──
        opt.Log($"[rast] #{di} rt{rtId}→tex{outTexId} vs{vsIdx}/fs{fsIdx} prim={prim} cnt={cnt} vp={W}×{H} bk=0x{bk:x} step={opt.Step}");
        if(opt.LogVsVerts)
            for(var v=0;v<Math.Min(cnt,4);v++) {
                var p=vsOut[v].Position;
                opt.Log($"  v{v}: in=({verts[v][0]:0.###},{verts[v][1]:0.###},{verts[v][2]:0.###},{verts[v][3]:0.###}) gl_Pos=({p[0]:0.###},{p[1]:0.###},{p[2]:0.###},{p[3]:0.###}) screen=({sv[v].x:0.#},{sv[v].y:0.#}) Out={{{string.Join(" ",vsOut[v].Out.OrderBy(kv=>kv.Key.Loc*16+kv.Key.Comp).Select(kv=>$"{kv.Key.Loc}_{kv.Key.Comp}={kv.Value:0.###}"))}}}");
            }
        if(bk!=0) opt.Log($"  ‡ blend bk=0x{bk:x} ignored (v0=overwrite)");
        // ── Per-tri rasterize, tile-parallel ──
        var step = opt.Step; var ts = opt.TileSize;
        var nTx = (W+ts-1)/ts; var nTy = (H+ts-1)/ts;
        long nCov = 0; var noteHist = new Dictionary<string,int>();
        var min = new[]{1e9f,1e9f,1e9f,1e9f};
        var max = new[]{-1e9f,-1e9f,-1e9f,-1e9f};
        var sum = new double[4];
        var sync = new object();
        for(var t0=0; t0+2<cnt; t0+=3) {
            var v0=sv[t0]; var v1=sv[t0+1]; var v2=sv[t0+2];
            var o0=vsOut[t0]; var o1=vsOut[t0+1]; var o2=vsOut[t0+2];
            // Edge fns (CCW-positive area). area2 = e01(v2).
            float E(float ax,float ay,float bx,float by,
                    float px,float py)
                => (px-ax)*(by-ay) - (py-ay)*(bx-ax);
            var area2 = E(v0.x,v0.y,v1.x,v1.y,v2.x,v2.y);
            if(MathF.Abs(area2) < 1e-3f) continue;  // degenerate
            var inv = 1f/area2;
            // Tri bbox (clamped to viewport).
            var bx0=(int)MathF.Max(0,MathF.Min(v0.x,MathF.Min(v1.x,v2.x)));
            var by0=(int)MathF.Max(0,MathF.Min(v0.y,MathF.Min(v1.y,v2.y)));
            var bx1=(int)MathF.Min(W-1,MathF.Max(v0.x,MathF.Max(v1.x,v2.x)));
            var by1=(int)MathF.Min(H-1,MathF.Max(v0.y,MathF.Max(v1.y,v2.y)));
            // Collect varying keys once (= union of the 3
            // verts' Out keys; for vs63 they're identical).
            var keys = o0.Out.Keys.Union(o1.Out.Keys)
                       .Union(o2.Out.Keys).ToArray();
            Parallel.For(0, nTx*nTy, ti => {
                var tx0 = (ti%nTx)*ts; var ty0 = (ti/nTx)*ts;
                if(tx0>bx1||ty0>by1||tx0+ts<=bx0||ty0+ts<=by0)
                    return;   // tile outside tri-bbox
                long lCov=0;
                var lmin=new[]{1e9f,1e9f,1e9f,1e9f};
                var lmax=new[]{-1e9f,-1e9f,-1e9f,-1e9f};
                var lsum=new double[4];
                var lnotes=new Dictionary<string,int>();
                var smp=new float[4];
                for(var py=Math.Max(ty0,by0);
                        py<Math.Min(ty0+ts,by1+1); py+=step)
                for(var px=Math.Max(tx0,bx0);
                        px<Math.Min(tx0+ts,bx1+1); px+=step) {
                    var cx=px+0.5f; var cy=py+0.5f;
                    var w0=E(v1.x,v1.y,v2.x,v2.y,cx,cy)*inv;
                    var w1=E(v2.x,v2.y,v0.x,v0.y,cx,cy)*inv;
                    var w2=1f-w0-w1;
                    // Inside test (either winding).
                    if(!((w0>=0&&w1>=0&&w2>=0)
                       ||(w0<=0&&w1<=0&&w2<=0))) continue;
                    // Perspective-correct interp weights.
                    var pw = w0*v0.iw + w1*v1.iw + w2*v2.iw;
                    var ip = pw!=0 ? 1f/pw : 1f;
                    var b0=w0*v0.iw*ip; var b1=w1*v1.iw*ip;
                    var b2=w2*v2.iw*ip;
                    // FS env.
                    var env = new SpvEvalEnv {
                        BuiltIn = {
                            // FragCoord.w = 1/clip.w (= pw
                            // for w=1 fullscreen case = 1).
                            [15] = new[]{cx, cy,
                                b0*v0.z+b1*v1.z+b2*v2.z, pw},
                        },
                        Cbuf = (s,b,i,c) => cbuf(1, s,b,i,c),
                    };
                    foreach(var k in keys)
                        env.In[k] = b0*o0.Out.GetValueOrDefault(k)
                                  + b1*o1.Out.GetValueOrDefault(k)
                                  + b2*o2.Out.GetValueOrDefault(k);
                    env.Tex = (s,b,coord,sk,lod,dref) => {
                        if(opt.TexOverride.TryGetValue(b,out var ov))
                            return ov;
                        // set1 b=8+2*sl ⟹ sl=(b-8)/2.
                        var sl = ((int)b-8)/2;
                        if(sl<0||sl>=40||th[sl]==0)
                            return new[]{0f,0f,0f,1f};
                        if(bufs.TryGetValue(th[sl],out var rb)) {
                            rb.Sample(coord[0],
                                coord.Length>1?coord[1]:0, smp);
                            // (T6)×85 ×2: Dref shadow-compare.
                            // SpvEval passes dref≠0 only for
                            // OpImageSampleDref* (line 484);
                            // returns rgba[0] as scalar (line
                            // 490). fs111 b16 = rt9.depth
                            // shadow-map (1024×4096 cascade-
                            // stacked); dref = c[1][0].z+bias
                            // ≈ 0.99. ‡ compare-op = LESS by
                            // default (= lit if shadow-depth
                            // < ref); opt.DrefGreater flips
                            // for reverse-Z. ‡ dref==0 is
                            // ambiguous (could be real Dref
                            // of 0); v0 treats it as not-Dref
                            // since fs111's dref=0.99 always.
                            if(dref != 0f) {
                                var pass = (opt.DrefGreater
                                    ? smp[0] > dref
                                    : smp[0] < dref) ? 1f : 0f;
                                return new[]{pass,pass,pass,pass};
                            }
                            return (float[])smp.Clone();
                        }
                        // ‡ texPool tex not pre-loaded ⟹ 0.
                        // (T6)×85: TexDefault[texId] override
                        // for known-constant texPool entries
                        // (e.g. tex314=(1,1,1,1) white-default
                        // tex256=(0,0,0,0) black-default) so
                        // they don't all alias to (0,0,0,1).
                        if(opt.TexDefault.TryGetValue(
                                th[sl], out var td))
                            return td;
                        return new[]{0f,0f,0f,1f};
                    };
                    var fr = SpvEval.Eval(spvFs, env);
                    foreach(var n in fr.Notes)
                        lnotes[n] = lnotes.GetValueOrDefault(n)+1;
                    var oc = fr.OColor;
                    // Write (overwrite; ‡ no blend v0).
                    // Fill the step×step block so the
                    // output image isn't pointillist.
                    for(var dy=0;dy<step&&py+dy<H;dy++)
                    for(var dx=0;dx<step&&px+dx<W;dx++) {
                        var o=((py+dy)*W+(px+dx))*4;
                        outBuf.D[o]=oc[0]; outBuf.D[o+1]=oc[1];
                        outBuf.D[o+2]=oc[2]; outBuf.D[o+3]=oc[3];
                    }
                    lCov++;
                    for(var c=0;c<4;c++) {
                        if(oc[c]<lmin[c]) lmin[c]=oc[c];
                        if(oc[c]>lmax[c]) lmax[c]=oc[c];
                        lsum[c]+=oc[c];
                    }
                }
                if(lCov==0) return;
                lock(sync) {
                    nCov += lCov;
                    for(var c=0;c<4;c++) {
                        if(lmin[c]<min[c]) min[c]=lmin[c];
                        if(lmax[c]>max[c]) max[c]=lmax[c];
                        sum[c]+=lsum[c];
                    }
                    foreach(var (k,n) in lnotes)
                        noteHist[k]=noteHist.GetValueOrDefault(k)+n;
                }
            });
        }
        // ── Footer ──
        var nz = outBuf.D.Where((v,i)=>i%4<3 && v!=0).Count()/3;
        // (T6)×85: include α (= the SRC_α-blend gate per
        // ×85×1(c): fs111 oc.α=0 ⟹ key=0→SRC_α no-write).
        opt.Log($"  → cov={nCov}px oc.min=({min[0]:0.###},{min[1]:0.###},{min[2]:0.###},{min[3]:0.###}) max=({max[0]:0.###},{max[1]:0.###},{max[2]:0.###},{max[3]:0.###}) mean=({sum[0]/Math.Max(nCov,1):0.###},{sum[1]/Math.Max(nCov,1):0.###},{sum[2]/Math.Max(nCov,1):0.###},{sum[3]/Math.Max(nCov,1):0.###}) | tex{outTexId} nz≈{nz}");
        foreach(var (k,n) in noteHist.OrderByDescending(kv=>kv.Value).Take(5))
            opt.Log($"  ‡×{n}: {k}");
    }

    // (T6)×85 ×2: per-shader cb1 cache (= same as
    // NvnVulkan._shCb1; loaded from sh{N}-t{T}.cb1.bin).
    static readonly Dictionary<(int,int), byte[]?>
        _cb1Cache = new();
    static byte[]? LoadCb1(string dir, int shIdx, int t) {
        if(_cb1Cache.TryGetValue((shIdx,t), out var c))
            return c;
        var p = $"{dir}/sh{shIdx:d4}-t{t}.cb1.bin";
        var d = File.Exists(p) ? File.ReadAllBytes(p) : null;
        return _cb1Cache[(shIdx,t)] = d;
    }
    static byte[] FloatsToBytes(float[] f) {
        var b = new byte[f.Length*4];
        Buffer.BlockCopy(f, 0, b, 0, b.Length);
        return b;
    }

    // (T6)×85 ×2: NTEX .tex loader → RtBuf. Same format as
    // NvnReplay.LoadTex (NvnReplay.cs:410): magic 'NTEX',
    // u16 w, u16 h, u8 fmt, u8 _, u8 enc, u8 _, i32 rawLen,
    // then enc=0 raw RGBA8 / enc=1 RLE {u32 cnt, u8×4}.
    // For loading texPool entries (tex311 b14-LUT 256×1,
    // tex256/314 4×4 defaults) without going through
    // NvnReplay. ‡ Always decodes to RGBA8 (the .tex
    // capture format); float = byte/255.
    public static RtBuf? LoadTexFile(string path) {
        if(!File.Exists(path)) return null;
        var d = File.ReadAllBytes(path);
        if(d.Length<16 || BitConverter.ToUInt32(d,0)
                != 0x5845544e) return null;
        var w = BitConverter.ToUInt16(d, 4);
        var h = BitConverter.ToUInt16(d, 6);
        var enc = d[10];
        var rawLen = BitConverter.ToInt32(d, 12);
        var rgba = new byte[rawLen];
        if(enc == 0) {
            d.AsSpan(16, Math.Min(rawLen, d.Length-16))
             .CopyTo(rgba);
        } else {
            int o=16, p=0;
            while(p<rawLen && o+8<=d.Length) {
                var cnt = BitConverter.ToUInt32(d, o);
                for(var k=0u; k<cnt && p+4<=rawLen; k++) {
                    rgba[p++]=d[o+4]; rgba[p++]=d[o+5];
                    rgba[p++]=d[o+6]; rgba[p++]=d[o+7];
                }
                o += 8;
            }
        }
        var rb = new RtBuf(w, h);
        for(var p=0; p<w*h && p*4+3<rawLen; p++)
            for(var c=0;c<4;c++)
                rb.D[p*4+c] = rgba[p*4+c]/255f;
        return rb;
    }

    static readonly Dictionary<(int,int), byte[]?> _shCache = new();
    static byte[]? CompileSh(string dir, int idx, int t) {
        if(_shCache.TryGetValue((idx,t), out var c)) return c;
        var bp = $"{dir}/sh{idx:d4}-t{t}.bin";
        if(!File.Exists(bp)) return _shCache[(idx,t)]=null;
        var bin = File.ReadAllBytes(bp);
        var spv = Compiler.Compile(bin, out _, out _,
            $"rast-sh{idx}");
        return _shCache[(idx,t)] = spv;
    }

    // ── Pre-load an RtBuf from a PPM dump (8-bit ⟹ /255).
    // For "match GPU" mode: load r153-gbuf-c{0,1,2}.ppm into
    // bufs[308/310/313]. ‡ Lossy (8-bit); for HDR RTs use
    // .pfm (v0.5). ──
    // (T6)×85 ×3: load a depth-PPM (= DumpRtPpm's vf=126/
    // 129 output, which writes (1−d)^0.4×255 grayscale).
    // Inverts the stretch: d = 1 − (px/255)^2.5. ‡ 8-bit
    // lossy (~256 distinct depths); for value-match this
    // is the precision ceiling. v0.5 = write a .pfm raw-
    // float sidecar from DumpRtPpm and load THAT.
    public static RtBuf LoadDepthPpm(string path) {
        var rb = LoadPpm(path);
        for(var p=0; p<rb.W*rb.H; p++) {
            var g = rb.D[p*4];   // grayscale .r
            var d = 1f - MathF.Pow(g, 2.5f);
            rb.D[p*4]=d; rb.D[p*4+1]=d;
            rb.D[p*4+2]=d; rb.D[p*4+3]=1f;
        }
        return rb;
    }
    public static RtBuf LoadPpm(string path) {
        var d = File.ReadAllBytes(path);
        var i=0; while(d[i]!='\n') i++; i++;       // P6
        var w0=i; while(d[i]!=' ') i++;
        var W=int.Parse(System.Text.Encoding.ASCII
            .GetString(d,w0,i-w0)); i++;
        var h0=i; while(d[i]!='\n') i++;
        var H=int.Parse(System.Text.Encoding.ASCII
            .GetString(d,h0,i-h0)); i++;
        while(d[i]!='\n') i++; i++;                // 255
        var rb = new RtBuf(W,H);
        for(var p=0;p<W*H;p++) {
            rb.D[p*4+0]=d[i+p*3+0]/255f;
            rb.D[p*4+1]=d[i+p*3+1]/255f;
            rb.D[p*4+2]=d[i+p*3+2]/255f;
            rb.D[p*4+3]=1f;
        }
        return rb;
    }
    public static void DumpPpm(RtBuf rb, string path,
            bool reinhard=false) {
        using var f = File.Create(path);
        var hdr = System.Text.Encoding.ASCII.GetBytes(
            $"P6\n{rb.W} {rb.H}\n255\n");
        f.Write(hdr);
        var b = new byte[rb.W*rb.H*3];
        for(var p=0;p<rb.W*rb.H;p++)
            for(var c=0;c<3;c++) {
                var v = rb.D[p*4+c];
                if(reinhard) v = v/(v+1);
                b[p*3+c] = (byte)Math.Clamp(v*255,0,255);
            }
        f.Write(b);
    }
    public static void DumpPfm(RtBuf rb, string path) {
        // PF = RGB float32, little-endian (scale=-1.0).
        // ‡ Bottom-up per PFM convention; readers vary.
        using var f = File.Create(path);
        f.Write(System.Text.Encoding.ASCII.GetBytes(
            $"PF\n{rb.W} {rb.H}\n-1.0\n"));
        var b = new byte[rb.W*3*4];
        for(var y=rb.H-1;y>=0;y--) {
            for(var x=0;x<rb.W;x++)
                for(var c=0;c<3;c++)
                    BitConverter.GetBytes(rb.D[(y*rb.W+x)*4+c])
                        .CopyTo(b, (x*3+c)*4);
            f.Write(b);
        }
    }
}
