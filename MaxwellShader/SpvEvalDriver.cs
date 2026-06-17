namespace MaxwellShader;

using System.Text;

// (T6)×77 ×3: --spv-eval driver. Compiles a Maxwell shader
// .bin fresh → SPIR-V → SpvEval at one fragment, with cbufs
// wired from a captured frame's ubos.bin (for c[3+]) and
// parameterized c[1]/c[2] (= the unknowns we're solving
// for). Dumps the FULL %id trace + ‡-notes.
//
// Usage (via UmbraCli/Program.cs --spv-eval dispatch):
//   --spv-eval <sh.bin> <frame-dir> <draw#> <px> <py>
//     [--c1 x,y,z,w,…]  [--tex b=r,g,b,a …]  [--trace]
//
// e.g. fs244 @ trunk with filmic-guess c1:
//   --spv-eval /tmp/umbra-shaders/sh0244-t2.bin \
//     /tmp/nvncap6l/frame-36030 663 195 300 \
//     --c1 0.06,0.004,-0.0667,0.715,0.072 \
//     --tex 16=0,0,0,1 --tex 8=0,0,0,1 --tex 14=0,0,0,1
//
// fs111 @ trunk (= the r152a-squish target — trace where
// FragCoord→G-buf-UV goes; let tex callback PRINT the UV):
//   --spv-eval /tmp/umbra-shaders/sh0111-t2.bin \
//     /tmp/nvncap6l/frame-36030 631 195 300 --trace

public static class SpvEvalDriver {
    public static int Run(string[] args) {
        // ── parse ──
        var binPath = args[0];
        var frameDir = args[1];
        var drawN = int.Parse(args[2]);
        var px = float.Parse(args[3]);
        var py = float.Parse(args[4]);
        float[]? c1 = null, c2 = null;
        var texOverride = new Dictionary<uint, float[]>();
        var fullTrace = false;
        for(var i=5; i<args.Length; i++) {
            switch(args[i]) {
            case "--c1": c1 = ParseFloats(args[++i]); break;
            case "--c2": c2 = ParseFloats(args[++i]); break;
            case "--tex": {
                var (b,v) = ParseTex(args[++i]);
                texOverride[b] = v; break; }
            case "--trace": fullTrace = true; break;
            default:
                Console.Error.WriteLine($"⚠ unknown opt: {args[i]}");
                break;
            }
        }

        // ── compile FRESH (kt[39] currency: cached .spv may
        // be stale; .bin is rewritten every replay run) ──
        var bin = File.ReadAllBytes(binPath);
        var spv = Compiler.Compile(bin, out var notes,
            out var texKinds, $"spveval");
        Console.WriteLine($"[spveval] {binPath}: bin={bin.Length}B → spv={spv.Length}B");
        foreach(var n in notes)
            Console.WriteLine($"[spveval]   compiler-note: {n}");
        // Persist for cross-check vs spirv-dis if wanted.
        File.WriteAllBytes(binPath + ".fresh.spv", spv);

        // ── load captured cbufs for this draw from ubos.bin
        // (= the REAL c[3..10] data; per ×76×3(j) layout:
        // draws.bin 256B/rec, ubo-offsets@68+k×4 (u32),
        // ubo-lengths@132+k×2 (u16)) ──
        var draws = File.ReadAllBytes($"{frameDir}/draws.bin");
        var ubos  = File.ReadAllBytes($"{frameDir}/ubos.bin");
        var rec = draws.AsSpan(drawN*256, 256);
        var ubo = new byte[16][];
        for(var k=0; k<16; k++) {
            var off = BitConverter.ToUInt32(rec.Slice(68+k*4,4));
            var len = BitConverter.ToUInt16(rec.Slice(132+k*2,2));
            ubo[k] = len>0 ? ubos.AsSpan((int)off,len).ToArray()
                           : Array.Empty<byte>();
        }
        Console.WriteLine($"[spveval] draw#{drawN} captured Ubos: " +
            string.Join(" ", Enumerable.Range(0,16)
                .Where(k=>ubo[k].Length>0)
                .Select(k=>$"[{k}]{(k<8?"VS":"FS")}sl{k%8}={ubo[k].Length}B")));

        // ── build env ──
        var W=1920f; var H=1080f;
        var env = new SpvEvalEnv {
            BuiltIn = {
                // FragCoord = (x+0.5, y+0.5, depth, 1/w).
                // ‡ depth/w: fs111 uses gl_FragCoord.w for
                // perspective; for full-screen-quad VS at
                // z=0,w=1 ⟹ depth=0, 1/w=1. fs244 only reads
                // .w via 1.0/in (= temp_3 in glsl).
                [15] = new[]{px+0.5f, py+0.5f, 0f, 1f},
            },
            In = {
                // ‡ VS-passed UV. For our fullscreen-tri VS
                // (sh0205 etc) this is screen-space [0,1].
                // ⚠ Y-direction depends on VS; OriginUpper
                // Left ⟹ uv.y = py/H is the safe v0 guess.
                // The fs111 FragCoord→UV trace will tell us
                // if this even matters for that path.
                [(0,0)] = px/W,
                [(0,1)] = py/H,
            },
        };
        var cbufLog = new HashSet<(uint,uint,uint,uint)>();
        env.Cbuf = (set, bind, v4i, cmp) => {
            // SpirvEmit: VS cbuf[N]→set0/bN; FS cbuf[N]→set2/bN.
            // bind = Maxwell c[bind]. NVN slot K → c[K+3] ⟹
            // captured Ubos[stage*8 + (bind-3)] for bind∈[3,10].
            // bind∈{1,2}: not captured ⟹ use --c1/--c2 OR 1.0
            // (= the C1_ONES default we're trying to replace).
            var stage = set==2 ? 1 : 0;
            float v;
            if(bind==1 && c1!=null) {
                var word = (int)(v4i*4+cmp);
                v = word<c1.Length ? c1[word] : 0f;
            } else if(bind==2 && c2!=null) {
                var word = (int)(v4i*4+cmp);
                v = word<c2.Length ? c2[word] : 0f;
            } else if(bind>=3 && bind<=10) {
                var k = stage*8 + (int)(bind-3);
                var off = (int)(v4i*16 + cmp*4);
                v = (k<16 && off+4<=ubo[k].Length)
                    ? BitConverter.ToSingle(ubo[k], off) : 0f;
            } else {
                // c[1]/c[2] without --c1/--c2 ⟹ default 1.0
                // (= what C1_ONES does today; lets us repro
                // the GPU-side behavior as the baseline).
                v = 1f;
            }
            if(cbufLog.Add((set,bind,v4i,cmp)))
                Console.WriteLine($"[spveval]   cbuf set{set} c[{bind}][{v4i}].{("xyzw"[(int)cmp])} = {v}");
            return v;
        };
        env.Tex = (set, bind, coord, sk, lod, dref) => {
            var cs = string.Join(",", coord.Select(c=>$"{c:0.####}"));
            var v = texOverride.TryGetValue(bind, out var ov)
                ? ov : new[]{0f,0f,0f,1f};
            Console.WriteLine($"[spveval]   tex set{set} b{bind} sk={sk} coord=({cs}) lod={lod}{(dref!=0?$" dref={dref}":"")} → ({v[0]},{v[1]},{v[2]},{v[3]})");
            return v;
        };
        StringBuilder? tr = null;
        if(fullTrace) {
            tr = new();
            env.OnTrace = (id, op, v) => {
                var s = v.Length==1
                    ? $"{BitConverter.UInt32BitsToSingle(v[0]),12:0.######} (0x{v[0]:x8})"
                    : "(" + string.Join(",", v.Select(u=>
                        $"{BitConverter.UInt32BitsToSingle(u):0.####}")) + ")";
                tr.AppendLine($"  %{id,-4} {op,-9} = {s}");
            };
        }

        // ── eval ──
        Console.WriteLine($"[spveval] eval @ px=({px},{py}) FragCoord=({px+0.5f},{py+0.5f}) uv=({px/W:0.####},{py/H:0.####}):");
        SpvEvalResult r;
        try {
            r = SpvEval.Eval(spv, env);
        } catch(Exception ex) {
            Console.WriteLine($"[spveval] ⚠ eval threw: {ex.Message}");
            if(tr!=null) Console.WriteLine($"[spveval] trace up to throw:\n{tr}");
            return 1;
        }

        // ── output ──
        Console.WriteLine($"[spveval] oColor = ({r.OColor[0]:0.######}, {r.OColor[1]:0.######}, {r.OColor[2]:0.######}, {r.OColor[3]:0.######})");
        Console.WriteLine($"[spveval]        ≈ 8-bit ({Clamp8(r.OColor[0])}, {Clamp8(r.OColor[1])}, {Clamp8(r.OColor[2])})");
        if(r.Notes.Count>0) {
            Console.WriteLine($"[spveval] ‡-notes ({r.Notes.Count}):");
            foreach(var n in r.Notes) Console.WriteLine($"  {n}");
        }
        Console.WriteLine($"[spveval] Names: " + string.Join(" ",
            r.Names.Select(kv=>$"%{kv.Key}={kv.Value}")));
        if(tr!=null) {
            Console.WriteLine($"[spveval] full trace ({r.Trace.Count} ids):");
            Console.Write(tr);
        }
        return 0;
    }

    static float[] ParseFloats(string s) =>
        s.Split(',').Select(float.Parse).ToArray();
    static (uint,float[]) ParseTex(string s) {
        var p = s.Split('=');
        return (uint.Parse(p[0]), ParseFloats(p[1]));
    }
    static int Clamp8(float f) =>
        (int)MathF.Round(MathF.Max(0,MathF.Min(1,f))*255);
}
