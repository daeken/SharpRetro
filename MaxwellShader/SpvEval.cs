namespace MaxwellShader;

// (T6)×77 ×3: SPIR-V interpreter for full-screen-quad FS
// debugging. Per sera kt[12]×30 (Jun-17 ~05:00Z): "why don't
// we just write a fucking spirv interpreter? … if we're just
// doing fragment shaders for a full-screen quad, just write
// a spirv interpreter where you can query the value of any
// variable. it's a tiny language; not like we're writing a
// full rasterization pipeline."
//
// = the kt[22] §7-3rd-domain instrument, generalized one
// layer below MaxwellEval (which evals our IL): this evals
// the SPIR-V we ACTUALLY emit, so it catches SpirvEmit bugs
// the IL-eval can't. + it's the value-level shader debugger
// that GPU-side replay can't be ("debugging at the value
// level is a bitch in shaders" — sera).
//
// SCOPE (verified ×77×2 against our actual sh0244/sh0111):
//   - Single-invocation, single-fragment. No rasterization.
//   - PURE SSA DATAFLOW: 0 OpBranchConditional, 0 OpPhi, 0
//     OpLoopMerge, 1 OpLabel in BOTH fs244 + fs111 (the two
//     immediate targets: tonemap + deferred-light). v0
//     THROWS on OpBranch* (kt[2] fail-fast; some shaders in
//     the corpus DO loop — IlLoop emits OpLoopMerge — and
//     those need a CFG-walk = v1).
//   - 0 Function-storage OpVariable in fs244 ⟹ no local
//     "memory"; all values are SSA result-ids. (IlLoop's
//     promote-to-Function-var path is the only producer; v0
//     handles Function-vars via _mem dict for the few that
//     have them, but no aliasing/pointers-into-arrays.)
//   - Values: everything is a uint[] (length 1=scalar, 2/3/4
//     =vec). Float ops bitcast in/out. Bool = 0u/1u.
//   - Types: minimally tracked (just enough for OpConstant
//     width, vector comp-count, image dim). SPIR-V is fully
//     type-annotated so we mostly don't need our own infer.
//
// THE KILLER FEATURE: Trace[id] = the value at every result-
// producing instruction. After Eval, query any %id (or via
// Names[id] → human label from OpName) to see what the
// shader computed there. = sera's "query the value of any
// variable" exactly.
//
// First targets (T6)×77×4:
//   (A) fs244 @ trunk-px (195,300): feed cbuf1 = {0.06,0.004,
//       −0.067,…} vs {1,1,1} vs back-solved; cbuf3 = captured
//       Ubos[8]; tex(sl0/1/2) = sampled rt2/etc dumps at the
//       VS-passed UV. ⟹ trace where the curve goes wrong.
//   (B) fs111 @ trunk-px: feed gl_FragCoord=(195.5,300.5) +
//       captured Ubos[8..13] for c[3..8]; tex = G-buf c[0/1/
//       2] sampled at WHATEVER UV fs111 computes. ⟹ trace
//       the FragCoord→G-buf-UV chain (= the r152a "scene
//       squished into upper half" root: if computed UV.y ≈
//       0.55 instead of 0.28 at trunk ⟹ found it).

public class SpvEvalEnv {
    // gl_FragCoord (and any other BuiltIn we hit). Key = the
    // SPIR-V BuiltIn enum value (FragCoord=15).
    public Dictionary<uint, float[]> BuiltIn = new();
    // Vertex→fragment interpolated inputs, by (Location,
    // Component). For full-screen-quad these are usually
    // just the UV (in_0_0, in_0_1).
    public Dictionary<(int Loc, int Comp), float> In = new();
    // cbuf read: (set, binding, vec4Index, component) → float.
    // Caller wires this to captured ubos.bin for c[3+] and to
    // experimental values for c[1]/c[2] (= the unknowns).
    public Func<uint, uint, uint, uint, float> Cbuf
        = (s,b,i,c) => 0f;
    // Texture sample: (set, binding, coord[], sampKind, lod,
    // dref) → vec4. sampKind = the Maxwell SampKind type-bits
    // (D2=2, Cube=4, Shadow=+0x20 etc, per SpirvEmit). Caller
    // wires this to dumped intermediary RTs (bilinear-sample
    // a PPM/PFM at coord×dims).
    public Func<uint, uint, float[], int, float, float, float[]>
        Tex = (s,b,c,k,l,d) => new[]{0f,0f,0f,0f};
    // Optional: log every result-producing instruction as it
    // evaluates (= live trace). (id, opName, value).
    public Action<uint, string, uint[]>? OnTrace;
}

public class SpvEvalResult {
    public float[] OColor = new float[4];
    // Every result-id's computed value. Query any %id.
    public Dictionary<uint, uint[]> Trace = new();
    // OpName'd ids → human label (cbuf1, tex_10_k2, …).
    public Dictionary<uint, string> Names = new();
    // Decorations we care about: id → {Binding, Set, Location,
    // Component, BuiltIn}. For mapping vars→env callbacks.
    public Dictionary<uint, Dictionary<string,uint>> Decor = new();
    // ‡-markers from this eval (unhandled-but-skipped ops,
    // assumptions made). Per spec §4 lifter-self-‡.
    public List<string> Notes = new();
    // (T6)×79 VS-mode: per-(Location,Component) Output values
    // = what the rasterizer interpolates and feeds the next
    // stage's env.In. gl_Position lands in BuiltInOut[0].
    // For FS, Out[(0,k)] mirrors OColor[k] (oColor is the
    // Location-0 Output; OColor kept for backward-compat
    // with ×77×4's verified-≡-GPU results).
    public Dictionary<(int Loc, int Comp), float> Out = new();
    public Dictionary<uint, float[]> BuiltInOut = new();
    public float[] Position =>
        BuiltInOut.TryGetValue(0, out var p) ? p : new float[4];
    public float F(uint id, int c=0) =>
        BitConverter.UInt32BitsToSingle(Trace[id][c]);
}

public static class SpvEval {
    // Opcode constants (subset; the ones we handle). Numeric
    // values per SPIR-V spec / SpirvEmit.cs:40-100.
    const uint OpName=5, OpExtInstImport=11, OpExtInst=12,
        OpMemoryModel=14, OpEntryPoint=15, OpExecutionMode=16,
        OpCapability=17, OpTypeVoid=19, OpTypeBool=20,
        OpTypeInt=21, OpTypeFloat=22, OpTypeVector=23,
        OpTypeImage=25, OpTypeSampledImage=27, OpTypeArray=28,
        OpTypeStruct=30, OpTypePointer=32, OpTypeFunction=33,
        OpConstantTrue=41, OpConstantFalse=42, OpConstant=43,
        OpConstantComposite=44, OpFunction=54, OpFunctionEnd=56,
        OpVariable=59, OpLoad=61, OpStore=62, OpAccessChain=65,
        OpDecorate=71, OpMemberDecorate=72,
        OpCompositeConstruct=80, OpCompositeExtract=81,
        OpImageSampleImplicitLod=87, OpImageSampleExplicitLod=88,
        OpImageSampleDrefImplicitLod=89,
        OpImageSampleDrefExplicitLod=90,
        OpConvertFToU=109, OpConvertFToS=110, OpConvertSToF=111,
        OpConvertUToF=112, OpBitcast=124,
        OpSNegate=126, OpFNegate=127,
        OpIAdd=128, OpFAdd=129, OpISub=130, OpFSub=131,
        OpIMul=132, OpFMul=133, OpFDiv=136,
        OpLogicalOr=166, OpLogicalAnd=167, OpLogicalNot=168,
        OpSelect=169, OpIEqual=170, OpINotEqual=171,
        OpUGreaterThan=172, OpSGreaterThan=173,
        OpUGreaterThanEqual=174, OpSGreaterThanEqual=175,
        OpULessThan=176, OpSLessThan=177,
        OpULessThanEqual=178, OpSLessThanEqual=179,
        OpFOrdEqual=180, OpFOrdNotEqual=182,
        OpFOrdLessThan=184, OpFOrdGreaterThan=186,
        OpFOrdLessThanEqual=188, OpFOrdGreaterThanEqual=190,
        OpShiftRightLogical=194, OpShiftRightArithmetic=195,
        OpShiftLeftLogical=196, OpBitwiseOr=197,
        OpBitwiseXor=198, OpBitwiseAnd=199, OpNot=200,
        OpLoopMerge=246, OpSelectionMerge=247, OpLabel=248,
        OpBranch=249, OpBranchConditional=250, OpKill=252,
        OpReturn=253, OpLogicalNotEqual=164;
    // GLSL.std.450 ext-inst fns we hit (per ×77×2 census:
    // fs244 uses only FMin/FMax; fs111 + others use more).
    const uint Glsl_Round=1, Glsl_Trunc=3, Glsl_FAbs=4,
        Glsl_Floor=8, Glsl_Ceil=9, Glsl_Fract=10,
        Glsl_Sin=13, Glsl_Cos=14, Glsl_Pow=26,
        Glsl_Exp2=29, Glsl_Log2=30, Glsl_Sqrt=31,
        Glsl_InvSqrt=32, Glsl_FMin=37, Glsl_FMax=40,
        Glsl_FClamp=43, Glsl_Fma=50,
        Glsl_PackHalf2x16=58, Glsl_UnpackHalf2x16=62;

    // Type info we keep: kind + (for vec) comp-count + (for
    // ptr) pointee. Minimal — SPIR-V is fully typed inline
    // so we only need this for OpConstant width + vec extract.
    enum TK { Void, Bool, I32, U32, F32, Vec, Ptr, Img,
              SampImg, Arr, Struct, Fn }
    record Ty(TK K, int N=1, uint Inner=0);

    // Marker for "this id is an OpVariable" — Load/Store/
    // AccessChain treat it specially (env-backed for Input/
    // Uniform; _mem for Function). The actual value lives in
    // env (for inputs/cbufs/tex) or _mem (for Function vars).
    record Var(uint TyId, uint StorageClass);
    // AccessChain result: a deferred address. Resolved at
    // OpLoad time (cbuf: env.Cbuf(set,bind,idx,comp); tex:
    // not addressable; struct/array index chain).
    record Chain(uint BaseId, uint[] Indices);

    public static SpvEvalResult Eval(byte[] spv, SpvEvalEnv env) {
        var r = new SpvEvalResult();
        var ty = new Dictionary<uint, Ty>();
        var val = r.Trace;     // id → uint[] computed value
        var obj = new Dictionary<uint, object>();  // Var/Chain
        var mem = new Dictionary<uint, uint[]>();   // Function-var store
        var dec = r.Decor;
        uint glslExt = 0;
        uint outColorId = 0;

        // ── parse + execute in one pass (instructions are
        // already in def-before-use SSA order for the pure-
        // dataflow case; types/constants/decorations come
        // before OpFunction). ──
        if(spv.Length < 20 || BitConverter.ToUInt32(spv,0)
                != 0x07230203)
            throw new InvalidDataException("not SPIR-V");
        var w = new uint[spv.Length/4];
        Buffer.BlockCopy(spv, 0, w, 0, spv.Length);
        var i = 5;   // skip header (magic,ver,gen,bound,schema)

        static float F(uint u)=>BitConverter.UInt32BitsToSingle(u);
        static uint  U(float f)=>BitConverter.SingleToUInt32Bits(f);
        uint[] V(uint id) => val.TryGetValue(id, out var v) ? v
            : throw new InvalidOperationException(
                $"%{id} not yet defined (forward-ref or unhandled producer)");
        float Ff(uint id) => F(V(id)[0]);
        Dictionary<string,uint> D(uint id) =>
            dec.TryGetValue(id, out var d) ? d
            : (dec[id] = new());

        while(i < w.Length) {
            var op = w[i] & 0xFFFF;
            var wc = (int)(w[i] >> 16);
            if(wc == 0) throw new InvalidDataException(
                $"zero-wordcount @word {i}");
            var a = w.AsSpan(i+1, wc-1);  // operands
            // For result-producing ops: a[0]=result-type,
            // a[1]=result-id, a[2..]=operands. For others
            // (Decorate, Store, …) the layout differs.
            uint rid = 0; uint[] rv = null!;
            void Set(params uint[] v) { rv = v; }
            void SetF(params float[] f) {
                rv = new uint[f.Length];
                for(var k=0;k<f.Length;k++) rv[k]=U(f[k]);
            }

            switch(op) {
            // ── header / metadata (no-op for eval) ──
            case OpCapability or OpMemoryModel
              or OpExecutionMode or OpEntryPoint
              or OpMemberDecorate or OpFunction
              or OpFunctionEnd or OpLabel or OpReturn:
                break;
            case OpExtInstImport:
                glslExt = a[0];
                break;
            case OpName: {
                // a[0]=target-id; a[1..]=UTF-8 name
                var nb = new byte[(wc-2)*4];
                Buffer.BlockCopy(spv, (i+2)*4, nb, 0, nb.Length);
                var s = System.Text.Encoding.UTF8
                    .GetString(nb).TrimEnd('\0');
                r.Names[a[0]] = s;
                if(s == "oColor" || s.StartsWith("oColor"))
                    outColorId = a[0];
                break; }
            case OpDecorate: {
                // a[0]=target a[1]=decoration a[2..]=literals
                var key = a[1] switch {
                    33 => "Binding", 34 => "DescriptorSet",
                    30 => "Location", 31 => "Component",
                    11 => "BuiltIn", 6 => "ArrayStride",
                    2  => "Block",
                    _ => $"d{a[1]}" };
                D(a[0])[key] = wc>3 ? a[2] : 1;
                break; }

            // ── types ──
            case OpTypeVoid:   ty[a[0]]=new(TK.Void); break;
            case OpTypeBool:   ty[a[0]]=new(TK.Bool); break;
            case OpTypeFloat:  ty[a[0]]=new(TK.F32); break;
            case OpTypeInt:
                ty[a[0]]=new(a[2]!=0?TK.I32:TK.U32); break;
            case OpTypeVector:
                ty[a[0]]=new(TK.Vec,(int)a[2],a[1]); break;
            case OpTypePointer:
                ty[a[0]]=new(TK.Ptr,1,a[2]); break;
            case OpTypeImage or OpTypeSampledImage:
                ty[a[0]]=new(TK.SampImg); break;
            case OpTypeArray or OpTypeStruct:
                ty[a[0]]=new(TK.Struct); break;
            case OpTypeFunction:
                ty[a[0]]=new(TK.Fn); break;

            // ── constants ──
            case OpConstant:
                rid=a[1]; Set(a[2]); break;
            case OpConstantTrue:
                rid=a[1]; Set(1); break;
            case OpConstantFalse:
                rid=a[1]; Set(0); break;
            case OpConstantComposite: {
                rid=a[1];
                rv = new uint[wc-3];
                for(var k=0;k<rv.Length;k++) rv[k]=V(a[2+k])[0];
                break; }

            // ── variables / memory ──
            case OpVariable: {
                // a[0]=ptr-type a[1]=result a[2]=StorageClass
                rid=a[1];
                obj[rid] = new Var(a[0], a[2]);
                // StorageClass: 0=UniformConstant(tex)
                // 1=Input 2=Uniform(cbuf) 3=Output 7=Function
                if(a[2]==7) mem[rid] = new uint[4];
                break; }
            case OpAccessChain: {
                // a[0]=rty a[1]=rid a[2]=base a[3..]=indices
                rid=a[1];
                var idxs = new uint[wc-4];
                for(var k=0;k<idxs.Length;k++)
                    idxs[k]=V(a[3+k])[0];
                obj[rid] = new Chain(a[2], idxs);
                break; }
            case OpLoad: {
                rid=a[1];
                rv = LoadVar(a[2], ty, obj, mem, val, dec,
                             env, r.Notes, r.Names);
                break; }
            case OpStore: {
                // a[0]=ptr a[1]=value. (T6)×79 VS-mode: route
                // ALL Output stores by decoration (BuiltIn →
                // BuiltInOut[bi]; Location/Component → Out
                // [(loc,comp)]). vs63 has 12 scalar out_N_M +
                // 1 vec4 gl_Position (BuiltIn=0); fs244 has 1
                // vec4 oColor (Location 0). OColor[] kept as
                // the FS-convenience alias (= ×77×4 verified).
                var v = V(a[1]);
                uint tgt = a[0]; int? chC = null;
                if(obj.TryGetValue(tgt, out var oc0)
                        && oc0 is Chain c0) {
                    tgt = c0.BaseId;
                    chC = (int)c0.Indices[^1];
                }
                if(!(obj.TryGetValue(tgt, out var ov)
                        && ov is Var(_, var vsc))) {
                    r.Notes.Add($"‡ OpStore to %{a[0]} (not Var) ignored");
                    break;
                }
                if(vsc == 7) {
                    // Function-var. ‡ chC-indexed component-
                    // write not modeled (0 in fs244+fs111+vs63
                    // per ×77×2 census; v1 if a shader hits it).
                    mem[tgt] = v;
                    break;
                }
                if(vsc == 3) {  // Output
                    var bd = dec.GetValueOrDefault(tgt) ?? new();
                    var cb = chC ?? 0;
                    if(bd.TryGetValue("BuiltIn", out var bi)) {
                        if(!r.BuiltInOut.TryGetValue(bi, out var bo))
                            r.BuiltInOut[bi] = bo = new float[4];
                        for(var k=0;k<v.Length && cb+k<4;k++)
                            bo[cb+k] = F(v[k]);
                    } else {
                        var loc = (int)bd.GetValueOrDefault("Location");
                        var cmp = (int)bd.GetValueOrDefault("Component");
                        for(var k=0;k<v.Length;k++)
                            r.Out[(loc, cmp+cb+k)] = F(v[k]);
                    }
                    if(tgt == outColorId) {
                        if(chC.HasValue) {
                            if(chC.Value<4) r.OColor[chC.Value]=F(v[0]);
                        } else
                            for(var k=0;k<Math.Min(4,v.Length);k++)
                                r.OColor[k] = F(v[k]);
                    }
                    break;
                }
                r.Notes.Add($"‡ OpStore to %{a[0]} sc={vsc} ignored");
                break; }

            // ── composite ──
            case OpCompositeConstruct: {
                rid=a[1];
                rv = new uint[wc-3];
                for(var k=0;k<rv.Length;k++) rv[k]=V(a[2+k])[0];
                break; }
            case OpCompositeExtract: {
                rid=a[1];
                Set(V(a[2])[(int)a[3]]);
                break; }

            // ── ALU: float ──
            case OpFNegate: rid=a[1]; SetF(-Ff(a[2])); break;
            case OpFAdd: rid=a[1]; SetF(Ff(a[2])+Ff(a[3])); break;
            case OpFSub: rid=a[1]; SetF(Ff(a[2])-Ff(a[3])); break;
            case OpFMul: rid=a[1]; SetF(Ff(a[2])*Ff(a[3])); break;
            case OpFDiv: rid=a[1]; SetF(Ff(a[2])/Ff(a[3])); break;
            // ── ALU: int ──
            case OpSNegate: rid=a[1]; Set((uint)(-(int)V(a[2])[0])); break;
            case OpIAdd: rid=a[1]; Set(V(a[2])[0]+V(a[3])[0]); break;
            case OpISub: rid=a[1]; Set(V(a[2])[0]-V(a[3])[0]); break;
            case OpIMul: rid=a[1]; Set(V(a[2])[0]*V(a[3])[0]); break;
            case OpNot:  rid=a[1]; Set(~V(a[2])[0]); break;
            case OpBitwiseAnd: rid=a[1]; Set(V(a[2])[0]&V(a[3])[0]); break;
            case OpBitwiseOr:  rid=a[1]; Set(V(a[2])[0]|V(a[3])[0]); break;
            case OpBitwiseXor: rid=a[1]; Set(V(a[2])[0]^V(a[3])[0]); break;
            case OpShiftLeftLogical:
                rid=a[1]; Set(V(a[2])[0]<<(int)V(a[3])[0]); break;
            case OpShiftRightLogical:
                rid=a[1]; Set(V(a[2])[0]>>(int)V(a[3])[0]); break;
            case OpShiftRightArithmetic:
                rid=a[1]; Set((uint)((int)V(a[2])[0]>>(int)V(a[3])[0])); break;
            // ── convert ──
            case OpBitcast: rid=a[1]; Set(V(a[2])[0]); break;
            case OpConvertSToF: rid=a[1]; SetF((int)V(a[2])[0]); break;
            case OpConvertUToF: rid=a[1]; SetF(V(a[2])[0]); break;
            case OpConvertFToU: rid=a[1]; Set((uint)Ff(a[2])); break;
            case OpConvertFToS: rid=a[1]; Set((uint)(int)Ff(a[2])); break;
            // ── compare → bool (0/1) ──
            case OpFOrdEqual: rid=a[1]; Set(Ff(a[2])==Ff(a[3])?1u:0); break;
            case OpFOrdNotEqual: rid=a[1]; Set(Ff(a[2])!=Ff(a[3])?1u:0); break;
            case OpFOrdLessThan: rid=a[1]; Set(Ff(a[2])<Ff(a[3])?1u:0); break;
            case OpFOrdGreaterThan: rid=a[1]; Set(Ff(a[2])>Ff(a[3])?1u:0); break;
            case OpFOrdLessThanEqual: rid=a[1]; Set(Ff(a[2])<=Ff(a[3])?1u:0); break;
            case OpFOrdGreaterThanEqual: rid=a[1]; Set(Ff(a[2])>=Ff(a[3])?1u:0); break;
            case OpIEqual: rid=a[1]; Set(V(a[2])[0]==V(a[3])[0]?1u:0); break;
            case OpINotEqual: rid=a[1]; Set(V(a[2])[0]!=V(a[3])[0]?1u:0); break;
            case OpUGreaterThan: rid=a[1]; Set(V(a[2])[0]>V(a[3])[0]?1u:0); break;
            case OpUGreaterThanEqual: rid=a[1]; Set(V(a[2])[0]>=V(a[3])[0]?1u:0); break;
            case OpULessThan: rid=a[1]; Set(V(a[2])[0]<V(a[3])[0]?1u:0); break;
            case OpULessThanEqual: rid=a[1]; Set(V(a[2])[0]<=V(a[3])[0]?1u:0); break;
            case OpSGreaterThan: rid=a[1]; Set((int)V(a[2])[0]>(int)V(a[3])[0]?1u:0); break;
            case OpSGreaterThanEqual: rid=a[1]; Set((int)V(a[2])[0]>=(int)V(a[3])[0]?1u:0); break;
            case OpSLessThan: rid=a[1]; Set((int)V(a[2])[0]<(int)V(a[3])[0]?1u:0); break;
            case OpSLessThanEqual: rid=a[1]; Set((int)V(a[2])[0]<=(int)V(a[3])[0]?1u:0); break;
            // ── logical ──
            case OpLogicalNot: rid=a[1]; Set(V(a[2])[0]==0?1u:0); break;
            case OpLogicalAnd: rid=a[1]; Set(V(a[2])[0]!=0&&V(a[3])[0]!=0?1u:0); break;
            case OpLogicalOr:  rid=a[1]; Set(V(a[2])[0]!=0||V(a[3])[0]!=0?1u:0); break;
            case OpLogicalNotEqual: rid=a[1]; Set(V(a[2])[0]!=V(a[3])[0]?1u:0); break;
            // ── select (= the entire control-flow story for
            // fs244+fs111: 15 / 93 of these, ZERO branches) ──
            case OpSelect: {
                rid=a[1];
                rv = V(a[2])[0]!=0 ? V(a[3]) : V(a[4]);
                break; }

            // ── GLSL.std.450 ext-inst ──
            case OpExtInst: {
                rid=a[1];
                if(a[2]!=glslExt) {
                    r.Notes.Add($"‡ OpExtInst set %{a[2]} ≠ GLSL.std.450");
                    Set(0); break;
                }
                var fn=a[3];
                float x=Ff(a[4]);
                float y=wc>6?Ff(a[5]):0, z=wc>7?Ff(a[6]):0;
                SetF(fn switch {
                    Glsl_FAbs => MathF.Abs(x),
                    Glsl_Floor=> MathF.Floor(x),
                    Glsl_Ceil => MathF.Ceiling(x),
                    Glsl_Trunc=> MathF.Truncate(x),
                    Glsl_Round=> MathF.Round(x),
                    Glsl_Fract=> x-MathF.Floor(x),
                    Glsl_Sin  => MathF.Sin(x),
                    Glsl_Cos  => MathF.Cos(x),
                    Glsl_Sqrt => MathF.Sqrt(x),
                    Glsl_InvSqrt=> 1f/MathF.Sqrt(x),
                    Glsl_Exp2 => MathF.Pow(2,x),
                    Glsl_Log2 => MathF.Log2(x),
                    Glsl_Pow  => MathF.Pow(x,y),
                    Glsl_FMin => MathF.Min(x,y),
                    Glsl_FMax => MathF.Max(x,y),
                    Glsl_FClamp=> MathF.Max(y,MathF.Min(x,z)),
                    Glsl_Fma  => x*y+z,
                    // ‡ Pack/UnpackHalf2x16: scalar-only here;
                    // SpirvEmit uses 58 for I2F.HI (×?). v0 =
                    // bitcast-no-op + ‡-note.
                    _ => Note(r,$"‡ ExtInst {fn} unhandled → 0",0f),
                });
                break; }

            // ── image sample ──
            case OpImageSampleImplicitLod
              or OpImageSampleExplicitLod
              or OpImageSampleDrefImplicitLod
              or OpImageSampleDrefExplicitLod: {
                rid=a[1];
                // a[2]=sampledImage(loaded var) a[3]=coord
                // [a[4]=dref] [imageOperands lod …]
                var simg = a[2];
                // Trace back to the OpVariable to read its
                // (set,binding) from decorations.
                var baseId = simg;
                while(obj.TryGetValue(baseId, out var bo)
                        && bo is Chain bch)
                    baseId = bch.BaseId;
                // simg is the result of OpLoad on a tex var;
                // we stashed the var-id in val[simg][0] as a
                // back-pointer (see LoadVar SampImg arm).
                if(val.TryGetValue(simg, out var sv))
                    baseId = sv[0];
                var bd = dec.GetValueOrDefault(baseId)
                       ?? new();
                var set = bd.GetValueOrDefault("DescriptorSet");
                var bind= bd.GetValueOrDefault("Binding");
                var coord = V(a[3]);
                var fc = new float[coord.Length];
                for(var k=0;k<fc.Length;k++) fc[k]=F(coord[k]);
                var isDref = op is OpImageSampleDrefImplicitLod
                                or OpImageSampleDrefExplicitLod;
                var dref = isDref ? Ff(a[4]) : 0f;
                // ‡ lod: ExplicitLod has imageOperands+Lod
                // after coord/[dref]; v0 ignores it (passes
                // 0). The intermediary dumps are mip-0 anyway.
                var rgba = env.Tex(set, bind, fc,
                    /*sampKind*/ fc.Length>=3?4:2, 0f, dref);
                if(isDref) SetF(rgba[0]);  // Dref → scalar
                else SetF(rgba[0],rgba[1],rgba[2],rgba[3]);
                break; }

            // ── control-flow: kt[2] FAIL-FAST. fs244+fs111
            // have ZERO of these (verified ×77×2). When a
            // shader hits this, it needs the v1 CFG-walk
            // (block-by-block, OpPhi resolves from incoming-
            // edge). The throw-text IS the discriminator. ──
            case OpBranch or OpBranchConditional
              or OpLoopMerge or OpSelectionMerge:
                throw new NotSupportedException(
                    $"SpvEval v0: control-flow op {op} @word {i}"
                  + $" — this shader has real CFG (IlIf/IlLoop"
                  + $" path); needs v1 block-walk. fs244+fs111"
                  + $" don't hit this (verified ×77×2).");
            case OpKill:
                r.Notes.Add("OpKill — fragment discarded");
                r.OColor = new[]{float.NaN,float.NaN,
                                 float.NaN,float.NaN};
                return r;

            default:
                // kt[2]: don't silently skip. ‡-note + 0.
                // (kt[22]'s lesson: my §7-oracle's `else:
                // continue` silently skipped FADD-R → false
                // gl_Position.w → built walls#4+#5 from
                // garbage. THROW would've been better; but
                // some ops genuinely don't matter (OpLine,
                // OpNop). Compromise: ‡-note every one, set
                // result=0 if it has a result-id, so the
                // first wrong-value is traceable to a ‡.)
                r.Notes.Add($"‡ op {op} (wc={wc}) @word {i} unhandled");
                // Heuristic: ops with a result-id have it at
                // word[1] when word[0] is a type-id (∈ ty).
                if(wc>=3 && ty.ContainsKey(a[0])) {
                    rid=a[1]; Set(0);
                }
                break;
            }

            if(rid != 0 && rv != null) {
                val[rid] = rv;
                env.OnTrace?.Invoke(rid, OpN(op), rv);
            }
            i += wc;
        }
        return r;
    }

    static float Note(SpvEvalResult r, string s, float v) {
        r.Notes.Add(s); return v;
    }

    // OpLoad: resolve a Var or Chain to its value. The hairy
    // part — this is where env callbacks fire.
    static uint[] LoadVar(uint ptr,
            Dictionary<uint,Ty> ty,
            Dictionary<uint,object> obj,
            Dictionary<uint,uint[]> mem,
            Dictionary<uint,uint[]> val,
            Dictionary<uint,Dictionary<string,uint>> dec,
            SpvEvalEnv env, List<string> notes,
            Dictionary<uint,string> names) {
        static uint U(float f)=>BitConverter.SingleToUInt32Bits(f);
        if(!obj.TryGetValue(ptr, out var o)) {
            notes.Add($"‡ OpLoad %{ptr}: not a Var/Chain");
            return new uint[]{0};
        }
        switch(o) {
        case Var(var tyId, var sc): {
            var d = dec.GetValueOrDefault(ptr) ?? new();
            switch(sc) {
            case 1: {  // Input (gl_FragCoord, in_N_M)
                if(d.TryGetValue("BuiltIn", out var bi)) {
                    if(env.BuiltIn.TryGetValue(bi, out var fv)) {
                        var u=new uint[fv.Length];
                        for(var k=0;k<fv.Length;k++) u[k]=U(fv[k]);
                        return u;
                    }
                    notes.Add($"‡ BuiltIn {bi} not in env → 0");
                    return new uint[]{0,0,0,0};
                }
                var loc = (int)d.GetValueOrDefault("Location");
                var cmp = (int)d.GetValueOrDefault("Component");
                // (T6)×79 VS-mode: SpirvEmit declares FS
                // inputs as SCALAR (in_0_0, in_0_1 separately
                // per ×77×2 sh0244.dis) but VS attribute
                // inputs as VEC4 (in_0 = vec4 per sh0063.dis).
                // Read pointee comp-count from the var's
                // pointer-type's Inner. env.In still keys on
                // (loc,comp) — the rasterizer fills (0,0..3)
                // for a vec4 attr at Location 0.
                var n = 1;
                if(ty.TryGetValue(tyId, out var pt)
                   && ty.TryGetValue(pt.Inner, out var it)
                   && it.K == TK.Vec)
                    n = it.N;
                var uv = new uint[n];
                var miss = 0;
                for(var k=0;k<n;k++) {
                    if(env.In.TryGetValue((loc,cmp+k), out var iv))
                        uv[k] = U(iv);
                    else { uv[k]=0; miss++; }
                }
                if(miss == n)
                    notes.Add($"‡ in_{loc}_{cmp}{(n>1?$"..{cmp+n-1}":"")} not in env → 0");
                return uv;
            }
            case 0: {  // UniformConstant = sampledImage var.
                // OpLoad on a tex-var produces a "sampled-
                // image object" — opaque. We stash the var-id
                // so OpImageSample* can find its decorations.
                return new[]{ptr};
            }
            case 7:  // Function — local var
                return mem.GetValueOrDefault(ptr)
                    ?? new uint[]{0};
            case 2: case 3:  // Uniform/Output bare (no chain)
                notes.Add($"‡ OpLoad bare Uniform/Output %{ptr} — expected via AccessChain");
                return new uint[]{0};
            default:
                notes.Add($"‡ OpLoad sc={sc} %{ptr}");
                return new uint[]{0};
            }
        }
        case Chain(var baseId, var idxs): {
            // Resolve the base var.
            if(obj[baseId] is not Var(var btyId, var bsc)) {
                notes.Add($"‡ Chain base %{baseId} not a Var");
                return new uint[]{0};
            }
            var bd = dec.GetValueOrDefault(baseId) ?? new();
            switch(bsc) {
            case 2: {  // Uniform = cbuf. Our cbuf layout =
                // struct{vec4[256]} ⟹ idxs = [0, vec4Idx, comp].
                // (per SpirvEmit CbufVar @243-272.)
                var set = bd.GetValueOrDefault("DescriptorSet");
                var bind= bd.GetValueOrDefault("Binding");
                var v4i = idxs.Length>1 ? idxs[1] : 0;
                var cmp = idxs.Length>2 ? idxs[2] : 0;
                var f = env.Cbuf(set, bind, v4i, cmp);
                return new[]{U(f)};
            }
            case 1: {  // Input chain (e.g. gl_FragCoord[3])
                if(bd.TryGetValue("BuiltIn", out var bi)
                   && env.BuiltIn.TryGetValue(bi, out var fv)) {
                    var c = (int)idxs[0];
                    return new[]{U(c<fv.Length?fv[c]:0)};
                }
                notes.Add($"‡ Input-chain %{baseId}[{idxs[0]}]");
                return new uint[]{0};
            }
            case 7: {  // Function-var component
                var m = mem.GetValueOrDefault(baseId)
                    ?? new uint[]{0};
                var c = (int)idxs[0];
                return new[]{c<m.Length?m[c]:0};
            }
            default:
                notes.Add($"‡ Chain sc={bsc}");
                return new uint[]{0};
            }
        }
        default:
            return new uint[]{0};
        }
    }

    static string OpN(uint op) => op switch {
        OpFAdd=>"FAdd",OpFMul=>"FMul",OpFSub=>"FSub",
        OpFDiv=>"FDiv",OpFNegate=>"FNeg",OpSelect=>"Select",
        OpExtInst=>"Ext",OpLoad=>"Load",OpBitcast=>"Bitcast",
        OpCompositeExtract=>"Extract",
        OpCompositeConstruct=>"Construct",
        OpAccessChain=>"Chain",OpConstant=>"Const",
        OpImageSampleImplicitLod=>"TexI",
        OpImageSampleExplicitLod=>"TexE",
        OpImageSampleDrefImplicitLod=>"TexDI",
        OpFOrdGreaterThan=>"F>",OpFOrdLessThan=>"F<",
        OpLogicalNot=>"!",OpLogicalAnd=>"&&",
        _=>$"op{op}",
    };
}
