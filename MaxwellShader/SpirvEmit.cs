namespace Pagentry.Lifter;

// M2: Pagentry IL → SPIR-V binary (tier-3, day-28 ~07:00Z).
//
// v0 scope = shader-01 (VS, no sampling): IlConst, IlCast(bitcast),
// IlReadReg/IlWriteReg(Gpr), IlAttrLoad/Store, IlCbufLoad, IlBin(Add/Mul),
// IlBlock, IlExit. ~30 SPIR-V opcodes. Oracle stage-1 = spirv-val passes
// + spirv-dis readable; stage-2 = vkCreateShaderModule succeeds; stage-3
// = swap into NvnVulkan + pixels match fixed.vert.
//
// SPIR-V structure: u32 word stream. Header (5w: magic 0x07230203,
// version 0x00010000, generator, bound=maxId+1, schema=0), then
// instructions each = (wordCount<<16 | opcode) + operands. Logical
// section order is enforced (caps → memmodel → entrypoint → execmode
// → debug → decorations → types/consts/global-vars → functions).
//
// The IL is per-instruction (sequence of IlWriteReg/IlAttrStore blocks
// from MaxwellLift); SPIR-V is SSA. Bridge = `_gpr[n]` tracks the
// current SSA id for each GPR; IlReadReg looks it up, IlWriteReg
// updates it. RZ (255) → constant 0.0.
//
// Maxwell attr-space → SPIR-V mapping (per AttributeMap.cs read-for-
// understanding @ ~08:25Z; CORRECTED from my earlier 0x80=Position
// inference — that was brass/C6-adj caught at sh0349/0346 cross-check):
//   0x060 = TessLodLeft   0x064 = gl_Layer       0x068 = ViewportIdx
//   0x06c = gl_PointSize
//   0x070-0x07c = gl_Position (VS-out) / gl_FragCoord (FS-in IPA)
//   0x080+ = user attributes, loc=(off-0x80)/16, comp=(off%16)/4
//            — VS attr-IN reads vertex layout here; VS attr-OUT writes
//            varyings here; FS IPA reads varyings here. Same formula.
//   0x2c0+ = front/back diffuse/specular (fixed-func); 0x2e0 = PointCoord;
//            0x2f8 = TessCoord; 0x300+ = TexCoord0-9 (legacy fixed-func).
//   FS output: R0..R3 at EXIT → RT0.rgba (per SPH output mask).
//   cbuf:      hw slot N → Uniform block at (set=0, binding=N).
//              NVN BindUniformBuffer slot K → hw slot K+3 (CONFIRMED
//              at sh0349: NVN slot 0 (proj+view) → c[3], slot 2 → c[5]).

public enum SpvStage { Vertex, Fragment }

public class SpirvEmit {
    // ── Opcode constants (just the ~35 we need for v0). ──
    const uint OpName = 5, OpMemberName = 6;
    const uint OpExtInstImport = 11, OpExtInst = 12;
    const uint OpMemoryModel = 14, OpEntryPoint = 15, OpExecutionMode = 16;
    const uint OpCapability = 17;
    const uint OpTypeVoid = 19, OpTypeBool = 20, OpTypeInt = 21,
        OpTypeFloat = 22, OpTypeVector = 23,
        OpTypeImage = 25, OpTypeSampledImage = 27, OpTypeArray = 28,
        OpTypeStruct = 30, OpTypePointer = 32, OpTypeFunction = 33;
    const uint OpConstant = 43, OpConstantComposite = 44;
    const uint OpFunction = 54, OpFunctionEnd = 56;
    const uint OpVariable = 59, OpLoad = 61, OpStore = 62,
        OpAccessChain = 65;
    const uint OpDecorate = 71, OpMemberDecorate = 72;
    const uint OpCompositeConstruct = 80, OpCompositeExtract = 81;
    const uint OpImageSampleImplicitLod = 87, OpImageSampleExplicitLod = 88,
        OpImageSampleDrefImplicitLod = 89, OpImageSampleDrefExplicitLod = 90;
    // Conversions (I2F/F2I).
    const uint OpConvertFToU = 109, OpConvertFToS = 110,
        OpConvertSToF = 111, OpConvertUToF = 112;
    const uint OpBitcast = 124, OpSNegate = 126, OpFNegate = 127;
    const uint OpIAdd = 128, OpISub = 130, OpIMul = 132;
    const uint OpFAdd = 129, OpFSub = 131, OpFMul = 133, OpFDiv = 136;
    // Bitwise + shifts (LOP/SHL/SHR).
    const uint OpShiftRightLogical = 194, OpShiftLeftLogical = 196,
        OpBitwiseOr = 197, OpBitwiseXor = 198, OpBitwiseAnd = 199,
        OpNot = 200;
    // Integer compares (ISETP).
    const uint OpIEqual = 170, OpINotEqual = 171,
        OpUGreaterThan = 172, OpSGreaterThan = 173,
        OpUGreaterThanEqual = 174, OpSGreaterThanEqual = 175,
        OpULessThan = 176, OpSLessThan = 177,
        OpULessThanEqual = 178, OpSLessThanEqual = 179;
    // Logical (bool-result) ops + compares + select.
    const uint OpLogicalOr = 166, OpLogicalAnd = 167,
        OpLogicalNot = 168, OpLogicalNotEqual = 165;
    const uint OpSelect = 169;
    const uint OpFOrdEqual = 180, OpFOrdNotEqual = 182,
        OpFOrdLessThan = 184, OpFOrdGreaterThan = 186,
        OpFOrdLessThanEqual = 188, OpFOrdGreaterThanEqual = 190;
    // Control flow.
    const uint OpSelectionMerge = 247, OpLabel = 248,
        OpLoopMerge = 246, OpBranch = 249, OpBranchConditional = 250,
        OpKill = 252, OpReturn = 253;
    // Decorations / builtins / storage classes.
    const uint DecLocation = 30, DecBinding = 33, DecDescriptorSet = 34,
        DecBuiltIn = 11, DecBlock = 2, DecArrayStride = 6, DecOffset = 35;
    const uint BuiltInPosition = 0, BuiltInLayer = 9, BuiltInFragCoord = 15;
    const uint ScUniformConstant = 0, ScInput = 1, ScUniform = 2,
        ScOutput = 3, ScFunction = 7;
    // GLSL.std.450 instruction ids (the subset MUFU + UnOp.Abs need).
    // No native rcp — Maxwell's MUFU.rcp → OpFDiv 1.0/x.
    const uint Glsl_FAbs = 4, Glsl_Floor = 8, Glsl_Ceil = 9,
        Glsl_Sin = 13, Glsl_Cos = 14, Glsl_Exp2 = 29, Glsl_Log2 = 30,
        Glsl_Sqrt = 31, Glsl_InvSqrt = 32, Glsl_FMin = 37, Glsl_FMax = 40;

    // ── Module builder state. ──
    // Sections accumulated separately (SPIR-V requires strict order),
    // concatenated at Finish(). IDs are allocated monotone from 1.
    // _ext = OpExtInstImport (must come right after caps, before
    // OpMemoryModel). Stage tracked for FS-vs-VS dispatch.
    readonly SpvStage _stage;
    readonly List<uint> _caps = new(), _ext = new(), _entry = new(),
        _exec = new(), _debug = new(), _decor = new(),
        _types = new(), _func = new();
    uint _nextId = 1;
    uint _glsl450;       // GLSL.std.450 import id (lazy)

    public SpirvEmit(SpvStage stage = SpvStage.Vertex) { _stage = stage; }
    uint Id() => _nextId++;

    // Interning: types and constants are unique'd by structural key so
    // repeated TyFloat()/ConstF(0.5) return the same id (SPIR-V allows
    // dups but tools warn + it bloats).
    readonly Dictionary<string, uint> _intern = new();
    uint Intern(string key, Func<uint> make) {
        if(_intern.TryGetValue(key, out var id)) return id;
        id = make(); _intern[key] = id; return id;
    }

    // Emit one instruction into a section. Operands are raw u32s;
    // strings are encoded as UTF-8 + nul, padded to word boundary.
    static void Emit(List<uint> sec, uint op, params uint[] operands) {
        sec.Add(((uint)(operands.Length + 1) << 16) | op);
        sec.AddRange(operands);
    }
    static void EmitS(List<uint> sec, uint op, uint[] pre, string s) {
        var bytes = System.Text.Encoding.UTF8.GetBytes(s + "\0");
        var nw = (bytes.Length + 3) / 4;
        var words = new uint[pre.Length + nw];
        pre.CopyTo(words, 0);
        Buffer.BlockCopy(bytes, 0, words, pre.Length * 4, bytes.Length);
        Emit(sec, op, words);
    }

    // ── Type builders (interned). ──
    uint TyVoid()  => Intern("void", () => { var i=Id(); Emit(_types, OpTypeVoid, i); return i; });
    uint TyBool()  => Intern("bool", () => { var i=Id(); Emit(_types, OpTypeBool, i); return i; });
    uint TyF32()   => Intern("f32",  () => { var i=Id(); Emit(_types, OpTypeFloat, i, 32); return i; });
    uint TyU32()   => Intern("u32",  () => { var i=Id(); Emit(_types, OpTypeInt, i, 32, 0); return i; });
    uint TyI32()   => Intern("i32",  () => { var i=Id(); Emit(_types, OpTypeInt, i, 32, 1); return i; });
    uint TyVec(uint elem, uint n) => Intern($"v{n}({elem})", () => {
        var i=Id(); Emit(_types, OpTypeVector, i, elem, n); return i; });
    uint TyPtr(uint sc, uint pointee) => Intern($"p{sc}({pointee})", () => {
        var i=Id(); Emit(_types, OpTypePointer, i, sc, pointee); return i; });
    uint TyArr(uint elem, uint count, uint stride) => Intern($"a{count}/{stride}({elem})", () => {
        var len = ConstU(count);
        var i=Id(); Emit(_types, OpTypeArray, i, elem, len);
        Emit(_decor, OpDecorate, i, DecArrayStride, stride);
        return i; });
    uint TyFn(uint ret) => Intern($"fn({ret})", () => {
        var i=Id(); Emit(_types, OpTypeFunction, i, ret); return i; });

    uint ConstF(float v) => Intern($"cf{BitConverter.SingleToUInt32Bits(v):x8}", () => {
        var i=Id(); Emit(_types, OpConstant, TyF32(), i, BitConverter.SingleToUInt32Bits(v)); return i; });
    uint ConstU(uint v) => Intern($"cu{v}", () => {
        var i=Id(); Emit(_types, OpConstant, TyU32(), i, v); return i; });
    uint ConstI(int v) => Intern($"ci{v}", () => {
        var i=Id(); Emit(_types, OpConstant, TyI32(), i, (uint)v); return i; });

    // ── I/O variable builders. ──
    // Maxwell attr-space is byte-offset addressed; SPIR-V wants typed
    // variables at Locations. v0: declare each accessed (loc,comp) as
    // a scalar float Input/Output (one var per component). Coarser
    // (vec4-per-loc) is cleaner but needs CompositeExtract on every
    // load; scalar-per-component is direct. ‡ Builtin Position is
    // vec4 (the one exception — must be a single var).

    readonly Dictionary<(uint sc, int loc, int comp), uint> _ioVar = new();
    readonly Dictionary<int, uint> _vsInVec4 = new();
    uint _posOut;            // gl_Position (vec4, BuiltIn) — VS only
    readonly List<uint> _interface = new();   // for OpEntryPoint

    // Two encodings for I/O:
    // - VS-INPUT (vertex-fetch interface): vec4-per-Location.
    //   IlAttrLoad → OpLoad vec4 + OpCompositeExtract comp. ⚠️ The
    //   scalar-per-Component encoding (one float var @ Loc L Comp C)
    //   FAILS on lavapipe with vertex-attribute formats (= wall#6
    //   from stage-3 bisect, day-28 ~12:00Z — sh0349 VS + hand-FS
    //   = 0px; §7-oracle proved math correct → wall = vertex-input
    //   plumbing). Per Vulkan spec 15.1.4 it SHOULD work but doesn't
    //   on lavapipe; vec4-per-loc is the portable encoding.
    // - VS-OUTPUT / FS-INPUT (varyings) + FS-OUTPUT: scalar-per-
    //   Component (the v0 path). Component decoration on inter-
    //   stage varyings IS reliably supported (= location-overlap
    //   for packing).
    uint VsInVec4Var(int loc) {
        if(_vsInVec4.TryGetValue(loc, out var id)) return id;
        id = Id();
        var v4 = TyVec(TyF32(), 4);
        Emit(_types, OpVariable, TyPtr(ScInput, v4), id, ScInput);
        Emit(_decor, OpDecorate, id, DecLocation, (uint) loc);
        EmitS(_debug, OpName, new[]{id}, $"in_{loc}");
        _vsInVec4[loc] = id; _interface.Add(id);
        return id;
    }
    uint IoVar(uint sc, int loc, int comp) {
        // VS-input route: vec4-per-loc (above). This is for OUTPUTS
        // and FS-inputs (varyings).
        var key = (sc, loc, comp);
        if(_ioVar.TryGetValue(key, out var id)) return id;
        id = Id();
        var pty = TyPtr(sc, TyF32());
        Emit(_types, OpVariable, pty, id, sc);
        Emit(_decor, OpDecorate, id, DecLocation, (uint) loc);
        if(comp != 0) Emit(_decor, OpDecorate, id, 31 /*Component*/, (uint) comp);
        EmitS(_debug, OpName, new[]{id}, $"{(sc==ScInput?"in":"out")}_{loc}_{comp}");
        _ioVar[key] = id; _interface.Add(id);
        return id;
    }
    uint PosOut() {
        if(_posOut != 0) return _posOut;
        var v4 = TyVec(TyF32(), 4);
        _posOut = Id();
        Emit(_types, OpVariable, TyPtr(ScOutput, v4), _posOut, ScOutput);
        Emit(_decor, OpDecorate, _posOut, DecBuiltIn, BuiltInPosition);
        EmitS(_debug, OpName, new[]{_posOut}, "gl_Position");
        _interface.Add(_posOut);
        return _posOut;
    }

    // Cbuf: one Uniform block per slot, struct{float data[1024]}.
    // ⚠️ NVN cbufs are per-(stage, slot) — VS c[5] and FS c[5] are
    // DIFFERENT data (CONFIRMED at umbra72: stage=0 slot=2 = 64B model
    // mat4, stage=1 slot=2 = 16B α-threshold). Vulkan can't share a
    // binding across stages with different content, so:
    //   VS cbuf[N] → set=0 binding=N
    //   FS cbuf[N] → set=2 binding=N
    // (set=1 stays textures, shared.) NvnVulkan binds NVN(stage=0,slot=K)
    // snapshot → set0/binding=K+3, NVN(stage=1,slot=K) → set2/binding=K+3.
    readonly Dictionary<int, uint> _cbufVar = new();
    uint _cbufStructTy;
    uint CbufVar(int slot) {
        if(_cbufVar.TryGetValue(slot, out var id)) return id;
        // (M2-ζ) Harness ⟹ all cbufs at set=0 (the
        // PipelineLayout is {set0=18×UBO, set1=2×SSBO}
        // only). Real-render: VS=set0 / FS=set2.
        var set = _harness ? 0u
                : _stage == SpvStage.Fragment ? 2u : 0u;
        if(_cbufStructTy == 0) {
            // ⚠️ wall#6 (named by validation layer, day-28 ~13:30Z):
            // float[1024] stride=4 violates Vulkan UBO layout rules
            // (even relaxed: scalar-array stride must be ×16). The
            // shader CREATED but every cbuf read past [0] read the
            // wrong byte → garbage gl_Position → 0px. spirv-val (no
            // --target-env vulkan1.x) doesn't check this → my
            // "364/364 ✓" missed it. Fix = vec4[256] stride=16,
            // AccessChain (0, word/4, word%4) → ptr-to-float.
            var arr = TyArr(TyVec(TyF32(), 4), 256, 16);
            _cbufStructTy = Id();
            Emit(_types, OpTypeStruct, _cbufStructTy, arr);
            Emit(_decor, OpDecorate, _cbufStructTy, DecBlock);
            Emit(_decor, OpMemberDecorate, _cbufStructTy, 0, DecOffset, 0);
        }
        id = Id();
        Emit(_types, OpVariable, TyPtr(ScUniform, _cbufStructTy), id, ScUniform);
        Emit(_decor, OpDecorate, id, DecDescriptorSet, set);
        Emit(_decor, OpDecorate, id, DecBinding, (uint) slot);
        EmitS(_debug, OpName, new[]{id}, $"cbuf{slot}");
        _cbufVar[slot] = id;
        // ‡ Uniform vars NOT in OpEntryPoint interface pre-1.4; lavapipe
        // is 1.3 so this matters. Only Input/Output go in interface.
        return id;
    }

    // ─── (M2-α) HARNESS mode ──────────────────────────
    // Re-emit a VS/FS as a compute shader: AttrIn/AttrOut
    // → SSBO[in]/SSBO[out] (float[256], idx = maxwell-attr-
    // off/4); cbufs stay UBO@set=0; samples → ‡const(1.0)
    // v0. ExecutionModel=GLCompute, LocalSize 1,1,1.
    // ⟹ The §7-3-way: feed MaxwellEval + this-on-lavapipe
    // the SAME synthetic inputs (cbuf bytes + ssbo_in
    // floats), diff ssbo_out vs MaxwellEval's AttrOut.
    // Disagreement = bug in MaxwellLift OR SpirvEmit.
    // SPIR-V 1.0 ⟹ SSBO via ScUniform + BufferBlock
    // (deprecated-but-valid through 1.3; lavapipe ✓).
    bool _harness;
    uint _ssboTy, _ssboIn, _ssboOut;
    const uint DecBufferBlock = 3;
    void HarnessSsboDecl() {
        if(_ssboTy != 0) return;
        // struct { float data[256]; }  stride=4 (SSBO: no
        // ×16 layout rule, unlike UBO wall#6).
        var arr = TyArr(TyF32(), 256, 4);
        _ssboTy = Id();
        Emit(_types, OpTypeStruct, _ssboTy, arr);
        Emit(_decor, OpDecorate, _ssboTy, DecBufferBlock);
        Emit(_decor, OpMemberDecorate, _ssboTy, 0, DecOffset, 0);
        var pty = TyPtr(ScUniform, _ssboTy);
        _ssboIn = Id();
        Emit(_types, OpVariable, pty, _ssboIn, ScUniform);
        Emit(_decor, OpDecorate, _ssboIn, DecDescriptorSet, 1);
        Emit(_decor, OpDecorate, _ssboIn, DecBinding, 0);
        EmitS(_debug, OpName, new[]{_ssboIn}, "h_in");
        _ssboOut = Id();
        Emit(_types, OpVariable, pty, _ssboOut, ScUniform);
        Emit(_decor, OpDecorate, _ssboOut, DecDescriptorSet, 1);
        Emit(_decor, OpDecorate, _ssboOut, DecBinding, 1);
        EmitS(_debug, OpName, new[]{_ssboOut}, "h_out");
    }
    uint HarnessLoad(int off) {
        HarnessSsboDecl();
        var idx = ConstU((uint)(off / 4));
        var ac = Id();
        Emit(_func, OpAccessChain, TyPtr(ScUniform, TyF32()),
            ac, _ssboIn, ConstU(0), idx);
        var ld = Id();
        Emit(_func, OpLoad, TyF32(), ld, ac);
        return ld;
    }
    void HarnessStore(int off, uint v) {
        HarnessSsboDecl();
        var idx = ConstU((uint)(off / 4));
        var ac = Id();
        Emit(_func, OpAccessChain, TyPtr(ScUniform, TyF32()),
            ac, _ssboOut, ConstU(0), idx);
        Emit(_func, OpStore, ac, v);
    }

    // GLSL.std.450 import (lazy, into _ext section). Used by MUFU +
    // any UnOp the IL emits that doesn't have a core SPIR-V op.
    uint Glsl450() {
        if(_glsl450 != 0) return _glsl450;
        _glsl450 = Id();
        EmitS(_ext, OpExtInstImport, new[]{_glsl450}, "GLSL.std.450");
        return _glsl450;
    }

    // Sampled-image type + variable, per (handle, sampKind). Combined
    // image-sampler at set=1 binding=handle. SampKind discriminates
    // OpTypeImage params: dim (1D/2D/3D/Cube → SPIR-V Dim 0/1/2/3),
    // depth, arrayed. Same handle can be sampled with different kinds
    // in different shaders (game's texture pool entry knows its real
    // type; the SHADER's TEXS target says how to sample it). ‡ v0:
    // type per first-use; if a shader samples the same handle with
    // 2 kinds → throw (= surfaces the case).
    readonly Dictionary<(int, int), uint> _texVar = new();
    readonly Dictionary<int, uint> _tySampImg = new();   // sampKind → ty

    uint TySampImg(int sk) {
        // Key by TYPE-affecting bits only (dim+depth+arrayed). HasLod/
        // LodZero affect the sample OP, not the image TYPE — same
        // TypeImage for D2 and D2|LodZero. (spirv-val rejects dups.)
        var tk = sk & (0xf | SampKind.Array | SampKind.Depth);
        if(_tySampImg.TryGetValue(tk, out var ty)) return ty;
        var dim = (sk & 0xf) switch {
            SampKind.D1 => 0u, SampKind.D2 => 1u,
            SampKind.D3 => 2u, SampKind.Cube => 3u, _ => 1u };
        var depth = (sk & SampKind.Depth) != 0 ? 1u : 0u;
        var arrayed = (sk & SampKind.Array) != 0 ? 1u : 0u;
        var img = Id();
        Emit(_types, OpTypeImage, img, TyF32(), dim, depth, arrayed, 0, 1, 0);
        var samp = Id();
        Emit(_types, OpTypeSampledImage, samp, img);
        _tySampImg[tk] = samp;
        return samp;
    }
    uint TexVar(int handle, int sk) {
        // Same: var keyed by (handle, type-bits) not full sk.
        var tk = sk & (0xf | SampKind.Array | SampKind.Depth);
        if(_texVar.TryGetValue((handle, tk), out var id)) return id;
        var simg = TySampImg(sk);
        sk = tk;  // for the dict insert below
        id = Id();
        Emit(_types, OpVariable,
            TyPtr(ScUniformConstant, simg), id, ScUniformConstant);
        Emit(_decor, OpDecorate, id, DecDescriptorSet, 1);
        Emit(_decor, OpDecorate, id, DecBinding, (uint) handle);
        EmitS(_debug, OpName, new[]{id}, $"tex_{handle:x}_k{sk:x}");
        _texVar[(handle, sk)] = id;
        return id;
    }

    // FS gl_FragCoord (vec4 BuiltIn). For IPA at off 0x70-0x7c (= the
    // fragment's interpolated position; pos.w = 1/w on Maxwell — used
    // for the perspective-divide rcp(w) pattern).
    uint _fragCoord;
    uint FragCoord() {
        if(_fragCoord != 0) return _fragCoord;
        _fragCoord = Id();
        var v4 = TyVec(TyF32(), 4);
        Emit(_types, OpVariable, TyPtr(ScInput, v4), _fragCoord, ScInput);
        Emit(_decor, OpDecorate, _fragCoord, DecBuiltIn, BuiltInFragCoord);
        EmitS(_debug, OpName, new[]{_fragCoord}, "gl_FragCoord");
        _interface.Add(_fragCoord);
        return _fragCoord;
    }

    // FS output: vec4 oColor at Location 0. ‡ v0 = single RT only;
    // multi-RT (per SPH output mask) = M2.5.
    uint _fragOut;
    uint FragOut() {
        if(_fragOut != 0) return _fragOut;
        _fragOut = Id();
        var v4 = TyVec(TyF32(), 4);
        Emit(_types, OpVariable, TyPtr(ScOutput, v4), _fragOut, ScOutput);
        Emit(_decor, OpDecorate, _fragOut, DecLocation, 0);
        EmitS(_debug, OpName, new[]{_fragOut}, "oColor");
        _interface.Add(_fragOut);
        return _fragOut;
    }

    // IlLet/IlTmp tracking: tmpId → SSA result id of the bound expr.
    readonly Dictionary<int, uint> _tmp = new();

    // ── GPR + Predicate → SSA tracking. ──
    // Predicates: P0-P6 are bool SSA ids; P7=PT (always-true).
    readonly uint[] _gpr = new uint[256];   // current SSA id per Rn
    readonly uint[] _pred = new uint[8];    // current SSA id per Pn

    uint TyForReg(RegKind k) => k == RegKind.Pred ? TyBool() : TyF32();
    uint ConstTrue() => Intern("ctrue", () => {
        var i = Id(); Emit(_types, 41 /*OpConstantTrue*/, TyBool(), i); return i; });
    uint ConstFalse() => Intern("cfalse", () => {
        var i = Id(); Emit(_types, 42 /*OpConstantFalse*/, TyBool(), i); return i; });

    // Loop-mode: when set, Gpr/PredId/WriteReg route through
    // OpVariable Function (alloca) instead of SSA, so reg-state
    // persists across loop iterations without OpPhi. Set by IlLoop.
    Dictionary<int, uint> _loopGVar, _loopPVar;
    readonly List<uint> _funcVars = new();   // OpVariable Function (must
                                              // be at entry-block top)

    // Class-level OpLoad helper (R() is local to EmitExpr).
    uint EmLoad(uint ty, uint var) {
        var id = Id(); Emit(_func, OpLoad, ty, id, var); return id;
    }
    uint PredId(int n) {
        if(n == 7) return ConstTrue();
        if(_loopPVar?.TryGetValue(n, out var lv) == true)
            return EmLoad(TyBool(), lv);
        if(_pred[n] != 0) return _pred[n];
        Console.Error.WriteLine($"  ‡ read P{n} before write — emit false");
        // Cache → dedup the ‡-print across repeated reads of the
        // same unwritten Pn within one shader (= shared-subtree
        // visited N× in HSET*2/I2I tree-walk). Semantically
        // correct: all reads-before-first-write get the same
        // false; later WritePred overwrites.
        return _pred[n] = ConstFalse();
    }
    uint Gpr(int n) {
        if((uint)n > 255) throw new($"‡ Gpr({n}) OOB — upstream lift bug");
        if(n == 255) return ConstF(0);
        if(_loopGVar?.TryGetValue(n, out var lv) == true)
            return EmLoad(TyF32(), lv);
        if(_gpr[n] != 0) return _gpr[n];
        Console.Error.WriteLine($"  ‡ read R{n} before write");
        // Cache the 0-const → dedup the ‡-print (the over-count
        // = same Rn read 3× in one IL tree's shared subtrees).
        return _gpr[n] = ConstF(0);
    }
    void WriteGpr(int n, uint v) {
        if(n == 255) return;
        if(_loopGVar?.TryGetValue(n, out var lv) == true)
            Emit(_func, OpStore, lv, v);
        else _gpr[n] = v;
    }
    void WritePred(int n, uint v) {
        if(n == 7) return;
        if(_loopPVar?.TryGetValue(n, out var lv) == true)
            Emit(_func, OpStore, lv, v);
        else _pred[n] = v;
    }

    // Maxwell attr byte-offset → (loc, comp). VS-in: 0x80 base.
    // VS-out: 0x80-0x8c = gl_Position (special), else loc=(off-0x80)/16.
    static (int loc, int comp) AttrLoc(int off, int baseOff = 0x80) {
        var rel = off - baseOff;
        return (rel / 16, (rel % 16) / 4);
    }

    // IlConst.Value is `dynamic` (typically UInt128 from MaxwellLift's
    // C() helper). UInt128 isn't IConvertible → Convert.ToX throws.
    // Unwrap explicitly. (kt[2]: the throw IS the right behavior; this
    // is the explicit handler for the one type we actually emit.)
    static uint AsU32(dynamic v) => v is UInt128 u ? (uint)u : (uint)Convert.ToUInt64(v);
    static int AsI32(dynamic v) => v is UInt128 u ? (int)(uint)u : Convert.ToInt32(v);
    static float AsF32(dynamic v) => v switch {
        UInt128 u => BitConverter.UInt32BitsToSingle((uint)u),
        float f => f, double d => (float)d,
        _ => Convert.ToSingle(v) };

    // ── IL → SPIR-V expression walker. Returns SSA result id. ──
    // _func is the current function body (between OpLabel and OpReturn).
    uint EmitExpr(Il il) {
        uint R(uint ty, uint op, params uint[] ops) {
            var id = Id();
            var all = new uint[ops.Length + 2];
            all[0] = ty; all[1] = id; ops.CopyTo(all, 2);
            Emit(_func, op, all);
            return id;
        }
        switch(il) {
            case IlConst(var ty, var v):
                // f32 / u32 / bool constants. (bool = U1, from
                // MaxwellLift's Pred(7)=IlConst(U1,1)=PT=always-true.)
                if(ty == IlType.U1)
                    return AsU32(v) != 0 ? ConstTrue() : ConstFalse();
                return ty is IlType.F ? ConstF(AsF32(v)) : ConstU(AsU32(v));
            case IlReadReg(_, RegKind.Gpr, var n):
                return Gpr(n);
            case IlReadReg(_, RegKind.Pred, var n):
                return PredId(n);
            case IlCast(var ty, CastKind.Bitcast, var x): {
                // Bitcast preserves bits, changes type. F32↔U32 only
                // for now (Maxwell GPRs are typeless 32-bit; the IL
                // tags type per-use).
                var rty = ty == IlType.U32 ? TyU32() : TyF32();
                return R(rty, OpBitcast, EmitExpr(x));
            }
            case IlCast(var ty, var ck, var x)
                    when ck is CastKind.SToF or CastKind.UToF
                            or CastKind.FToSI or CastKind.FToUI: {
                // I2F/F2I. Source/dest types fixed at 32-bit v0
                // (the dominant case). The IL Ty says the dest type;
                // the SOURCE expr's type discriminates u32 vs f32.
                var xv = EmitExpr(x);
                var (rty, op) = ck switch {
                    CastKind.SToF => (TyF32(), OpConvertSToF),
                    CastKind.UToF => (TyF32(), OpConvertUToF),
                    CastKind.FToSI => (TyU32(), OpConvertFToS),
                    CastKind.FToUI => (TyU32(), OpConvertFToU),
                    _ => throw new() };
                // ⚠️ OpConvertSToF wants signed-int operand type;
                // we use u32 for both. SPIR-V allows it (the op
                // determines signedness interpretation, not the
                // operand type's signedness bit). ‡ spirv-val may
                // warn; if it errors, bitcast u32→i32 first.
                return R(rty, op, xv);
            }
            // Type-discriminated cases FIRST (before the unguarded
            // float fallbacks). Pattern-match `when` clauses don't
            // re-order, so order matters.
            case IlUn(var ty, UnOp.Not, var x) when ty == IlType.U1:
                return R(TyBool(), OpLogicalNot, EmitExpr(x));
            case IlUn(var ty, UnOp.Not, var x) when ty == IlType.U32:
                return R(TyU32(), OpNot, EmitExpr(x));
            case IlUn(var ty, UnOp.Neg, var x) when ty == IlType.U32:
                return R(TyU32(), OpSNegate, EmitExpr(x));
            // Float unary (default — F32 result type).
            case IlUn(_, UnOp.Neg, var x):
                return R(TyF32(), OpFNegate, EmitExpr(x));
            case IlUn(_, UnOp.Abs, var x):
                return R(TyF32(), OpExtInst, Glsl450(), Glsl_FAbs, EmitExpr(x));
            case IlUn(_, UnOp.Floor, var x):
                return R(TyF32(), OpExtInst, Glsl450(), Glsl_Floor, EmitExpr(x));
            case IlUn(_, UnOp.Ceil, var x):
                return R(TyF32(), OpExtInst, Glsl450(), Glsl_Ceil, EmitExpr(x));
            case IlUn(_, UnOp.Round, var x):
                // GLSL.std.450 RoundEven=2. ‡ Maxwell may be nearest-
                // or-zero; v0=RoundEven.
                return R(TyF32(), OpExtInst, Glsl450(), 2, EmitExpr(x));
            case IlUn(_, UnOp.Trunc, var x):
                // GLSL.std.450 Trunc=3. (T6)×43 mcb Δ-3: F2F
                // IntegerRound rmode=7 (was unmapped → pass-through).
                return R(TyF32(), OpExtInst, Glsl450(), 3, EmitExpr(x));
            case IlUn(_, UnOp.Sat, var x):
                // GLSL.std.450 FClamp=43. (T6)×43: the .SAT modifier
                // (bit 50 on float-arith ops → clamp result to [0,1]).
                // 526 instances missing across the LEGO corpus per
                // sweep-diff vs ryujinx. Emitting as FClamp (not the
                // FMin(FMax(x,0),1) pair) so the §7-oracle histogram
                // closes directly.
                return R(TyF32(), OpExtInst, Glsl450(), 43,
                    EmitExpr(x), ConstF(0f), ConstF(1f));
            case IlBin(var ty, var op, var a, var b): {
                var oa = EmitExpr(a); var ob = EmitExpr(b);
                // Result type discriminates: U1 = bool (compare or
                // logical); F32 = float arithmetic. ‡ Integer ops not
                // yet (no game shader uses them in the M1 set so far).
                // U32 integer ops (IADD/SHL/SHR/LOP + LDC index expr).
                if(ty == IlType.U32) {
                    var spo = op switch {
                        BinOp.Add => OpIAdd, BinOp.Sub => OpISub,
                        BinOp.Mul => OpIMul,
                        BinOp.And => OpBitwiseAnd, BinOp.Or => OpBitwiseOr,
                        BinOp.Xor => OpBitwiseXor,
                        BinOp.Shl => OpShiftLeftLogical,
                        BinOp.Shr => OpShiftRightLogical,
                        BinOp.Sar => (uint)195,  // OpShiftRightArithmetic
                        _ => throw new($"‡ BinOp.{op} (U32) → SPIR-V"),
                    };
                    return R(TyU32(), spo, oa, ob);
                }
                if(ty == IlType.U1) {
                    // Discriminate float-compare vs int-compare by
                    // OPERAND type (BinOp.Slt is reused for both;
                    // FSETP emits f32 operands, ISETP emits u32).
                    // ‡ v0: peek at `a` IL-type. Cleaner = separate
                    // BinOp.FSlt vs ISlt; this works for now.
                    var aIsInt = a.Ty == IlType.U32;
                    if(aIsInt) {
                        var spoi = op switch {
                            BinOp.Eq => OpIEqual, BinOp.Ne => OpINotEqual,
                            BinOp.Slt => OpSLessThan, BinOp.Sle => OpSLessThanEqual,
                            BinOp.Sgt => OpSGreaterThan, BinOp.Sge => OpSGreaterThanEqual,
                            BinOp.Ult => OpULessThan, BinOp.Ule => OpULessThanEqual,
                            BinOp.Ugt => OpUGreaterThan, BinOp.Uge => OpUGreaterThanEqual,
                            BinOp.And => OpLogicalAnd, BinOp.Or => OpLogicalOr,
                            BinOp.Xor => OpLogicalNotEqual,
                            _ => throw new($"‡ BinOp.{op} (U1, int operands)"),
                        };
                        return R(TyBool(), spoi, oa, ob);
                    }
                    var spou = op switch {
                        // Compares: operands are f32, result is bool.
                        // Maxwell FSETP fcomp → BinOp.S* by convention
                        // (MaxwellLift mapping); emit OpFOrd* (ordered
                        // = NaN→false; ‡ unordered variants when an
                        // fcomp≥8 surfaces).
                        BinOp.Slt => OpFOrdLessThan,
                        BinOp.Sle => OpFOrdLessThanEqual,
                        BinOp.Sgt => OpFOrdGreaterThan,
                        BinOp.Sge => OpFOrdGreaterThanEqual,
                        BinOp.Eq  => OpFOrdEqual,
                        BinOp.Ne  => OpFOrdNotEqual,
                        // Logical bool ops (the bop combine in FSETP).
                        BinOp.And => OpLogicalAnd,
                        BinOp.Or  => OpLogicalOr,
                        BinOp.Xor => OpLogicalNotEqual,
                        _ => throw new($"‡ BinOp.{op} (U1) → SPIR-V"),
                    };
                    return R(TyBool(), spou, oa, ob);
                }
                // FMin/FMax via GLSL.std.450 (no core SPIR-V op).
                if(op is BinOp.FMin or BinOp.FMax)
                    return R(TyF32(), OpExtInst, Glsl450(),
                        op == BinOp.FMin ? Glsl_FMin : Glsl_FMax, oa, ob);
                var spof = op switch {
                    BinOp.Add => OpFAdd, BinOp.Sub => OpFSub,
                    BinOp.Mul => OpFMul,
                    _ => throw new($"‡ BinOp.{op} → SPIR-V"),
                };
                return R(TyF32(), spof, oa, ob);
            }
            case IlAttrLoad(_, IlConst(_, var offV)): {
                var off = AsI32(offV);
                if(_harness) return HarnessLoad(off);
                var (loc, comp) = AttrLoc(off);
                // VS-input: vec4-per-loc + extract (wall#6 fix).
                if(_stage == SpvStage.Vertex) {
                    var v4l = R(TyVec(TyF32(),4), OpLoad, VsInVec4Var(loc));
                    return R(TyF32(), OpCompositeExtract, v4l, (uint)comp);
                }
                // FS-input (varyings): scalar-per-Component (fine).
                return R(TyF32(), OpLoad, IoVar(ScInput, loc, comp));
            }
            case IlInterp(_, IlConst(_, var offV), var mode, var mul): {
                // (M2-ζ) Harness: IPA → h_in[off/4] (no
                // FragCoord/Input vars in GLCompute). The
                // mode-1 mul still applies (the input data
                // is what's synthesized; perspective-divide
                // is part of the computation under test).
                if(_harness) {
                    var hoff = AsI32(offV);
                    var hv = HarnessLoad(hoff);
                    return mode == 1
                        ? R(TyF32(), OpFMul, hv, EmitExpr(mul))
                        : hv;
                }
                // FS interpolated input. Maxwell attr off:
                //   0x70-0x7c → gl_FragCoord.xyzw (BuiltIn)
                //   0x80+     → user varyings (loc=(off-0x80)/16)
                // Mode: 0=Pass(no mul), 1=Multiply(× mul-arg, perspective-
                // correct via rcp(w)), 2=Constant, 3=Sc. ‡ v0: 0/1 only.
                var off = AsI32(offV);
                uint val;
                if(off >= 0x70 && off < 0x80) {
                    // gl_FragCoord component. OpAccessChain into vec4
                    // → OpLoad scalar.
                    var comp = (uint)((off - 0x70) / 4);
                    var ac = R(TyPtr(ScInput, TyF32()), OpAccessChain,
                        FragCoord(), ConstU(comp));
                    val = R(TyF32(), OpLoad, ac);
                    // ⚠️ Maxwell IPA(0x7c) returns 1/gl_FragCoord.w
                    // (the perspective-divide reciprocal directly), NOT
                    // gl_FragCoord.w. SPIR-V's FragCoord.w IS already
                    // 1/clip.w. So loading .w gives the right value
                    // for the rcp(w) MUFU pattern that follows... ‡
                    // EXCEPT shader-02 then does MUFU.rcp on it = 1/(1/w)
                    // = w. The fmul-by-w then perspective-DIVIDES the
                    // attrs (since they're already /w'd by raster). =
                    // need to NOT divide. ‡ Stage-3 oracle settles this
                    // (= compare pixels to fixed.frag); for now load
                    // FragCoord.w as-is and let the math fall out.
                } else {
                    // User varying input — same scalar-per-comp scheme
                    // as VS attr-in.
                    var (loc, comp) = AttrLoc(off, 0x80);
                    val = R(TyF32(), OpLoad, IoVar(ScInput, loc, comp));
                }
                // Mode 1 = multiply by the mul-operand (typically rcp(w)
                // from MUFU). Mode 0 = pass-through.
                return mode == 1
                    ? R(TyF32(), OpFMul, val, EmitExpr(mul))
                    : val;
            }
            case IlMufu(_, var op, var x): {
                var xv = EmitExpr(x);
                return op switch {
                    // No GLSL.std.450 rcp — emit 1.0/x.
                    4 => R(TyF32(), OpFDiv, ConstF(1), xv),
                    // rcp64h/rsq64h = the high-half of a double-prec
                    // result; ‡ v0 unsupported (no double in shader-02).
                    6 or 7 => throw new($"‡ MUFU op={op} (64h)"),
                    // Rest map to GLSL.std.450 ext-inst.
                    _ => R(TyF32(), OpExtInst, Glsl450(), op switch {
                        0 => Glsl_Cos, 1 => Glsl_Sin, 2 => Glsl_Exp2,
                        3 => Glsl_Log2, 5 => Glsl_InvSqrt, 8 => Glsl_Sqrt,
                        _ => throw new($"‡ MUFU op={op}"),
                    }, xv),
                };
            }
            case IlSample(_, IlConst(_, var hV), var coords, var sk, _): {
                // (M2-ζ) Harness: no sampler bound. Return
                // const vec4{0.5,0.5,0.5,1.0} (= matches
                // MaxwellEval's Sample callback). Coords
                // still EmitExpr'd (side-effect-free, but
                // for SSA-bookkeeping consistency).
                if(_harness) {
                    foreach(var c in coords) EmitExpr(c);
                    var hv = TyVec(TyF32(), 4);
                    var hr = Id();
                    Emit(_func, OpCompositeConstruct, hv, hr,
                        ConstF(0.5f), ConstF(0.5f),
                        ConstF(0.5f), ConstF(1.0f));
                    return hr;
                }
                // Load combined sampled-image, build coord vector,
                // sample. SampKind discriminates dim/depth/lod.
                // Coords layout from MaxwellLift TEXS:
                //   spatial (N=dim, or 3 for cube) ++
                //   [dref if Depth] ++ [lod if HasLod].
                var handle = AsI32(hV);
                var simgTy = TySampImg(sk);
                var img = R(simgTy, OpLoad, TexVar(handle, sk));
                var cs = coords.Select(EmitExpr).ToArray();

                // Spatial coord-vec dimension. Cube=3 (xyz dir).
                var nSpat = (sk & 0xf) switch {
                    SampKind.D1 => 1, SampKind.D3 => 3,
                    SampKind.Cube => 3, _ => 2 };
                if((sk & SampKind.Array) != 0) nSpat++;
                // Build coord vec (N=1 → scalar; else vecN).
                uint coord;
                if(nSpat == 1) coord = cs[0];
                else {
                    var vN = TyVec(TyF32(), (uint) nSpat);
                    var ci = new uint[nSpat + 2];
                    ci[0] = vN; ci[1] = coord = Id();
                    for(var k = 0; k < nSpat; k++) ci[k+2] = cs[k];
                    Emit(_func, OpCompositeConstruct, ci);
                }
                int p = nSpat;
                var depth = (sk & SampKind.Depth) != 0;
                var hasLod = (sk & SampKind.HasLod) != 0;
                var lodZero = (sk & SampKind.LodZero) != 0;

                if(depth) {
                    // OpImageSampleDref{Implicit,Explicit}Lod.
                    // Result type = SCALAR float (the comparison
                    // result, not vec4). MaxwellLift's IlVecElem
                    // on the result reads .r only — we splat to
                    // vec4 so the IlVecElem still works. ‡ v0.
                    var dref = cs[p++];
                    uint res;
                    if(hasLod || lodZero) {
                        var lod = lodZero ? ConstF(0) : cs[p++];
                        res = R(TyF32(), OpImageSampleDrefExplicitLod,
                            img, coord, dref, 2/*Lod*/, lod);
                    } else {
                        res = R(TyF32(), OpImageSampleDrefImplicitLod,
                            img, coord, dref);
                    }
                    // Splat scalar → vec4 (.r=cmp, .gba=cmp). The
                    // game's depth-compare TEXS reads only .r so
                    // this is correct + lets IlVecElem work.
                    return R(TyVec(TyF32(),4), OpCompositeConstruct,
                        res, res, res, res);
                }
                // Non-depth: result = vec4.
                var v4 = TyVec(TyF32(), 4);
                if(hasLod || lodZero) {
                    var lod = lodZero ? ConstF(0) : cs[p++];
                    return R(v4, OpImageSampleExplicitLod,
                        img, coord, 2/*Lod*/, lod);
                }
                return R(v4, OpImageSampleImplicitLod, img, coord);
            }
            case IlTmp(_, var tid):
                if(_tmp.TryGetValue(tid, out var t)) return t;
                throw new($"‡ IlTmp %{tid} read before IlLet bind");
            case IlIfV(var ty, var cond, var th, var el): {
                // Value-producing select. ⚠️ Both arms eval
                // unconditionally (OpSelect, not branch) — fine for
                // pure ALU (FMNMX min/max, FSET 1.0/0.0, SEL); would
                // be wrong if either arm has side-effects. ‡ v0.
                var c = EmitExpr(cond);
                var tv = EmitExpr(th); var ev = EmitExpr(el);
                var rty = ty == IlType.U1 ? TyBool()
                        : ty == IlType.U32 ? TyU32() : TyF32();
                return R(rty, OpSelect, c, tv, ev);
            }
            case IlVecElem(_, var vec, IlConst(_, var idxV)): {
                // OpCompositeExtract: result, composite, literal-indices.
                // Works for vec2 (UnpackHalf2x16) and vec4 (IlSample).
                var v = EmitExpr(vec);
                var idx = AsU32(idxV);
                var id = Id();
                Emit(_func, OpCompositeExtract, TyF32(), id, v, idx);
                return id;
            }
            case IlIntrin(var ty, var nm, var args): {
                // Generic GLSL.std.450 intrinsic dispatch. v0:
                // UnpackHalf2x16 (uint→vec2) + PackHalf2x16
                // (vec2→uint via 2 scalar args composed here).
                var av = args.Select(EmitExpr).ToArray();
                switch(nm) {
                    case "UnpackHalf2x16":
                        return R(TyVec(TyF32(), 2), OpExtInst,
                            Glsl450(), 62, av[0]);
                    case "PackHalf2x16": {
                        // Args = (lo, hi). PackHalf2x16 takes vec2.
                        var v2 = Id();
                        Emit(_func, OpCompositeConstruct,
                            TyVec(TyF32(), 2), v2, av[0], av[1]);
                        return R(TyU32(), OpExtInst, Glsl450(), 58, v2);
                    }
                    default:
                        throw new($"‡ IlIntrin '{nm}'");
                }
            }
            case IlCbufLoad(_, var slot, IlConst(_, var offV)): {
                var byteOff = AsU32(offV);
                // struct{vec4[256]} stride=16 (wall#6 fix). AccessChain
                // (0, word/4, word%4) → ptr Uniform float.
                var w = byteOff / 4;
                var ac = R(TyPtr(ScUniform, TyF32()), OpAccessChain,
                    CbufVar(slot), ConstI(0),
                    ConstI((int)(w / 4)), ConstU(w % 4));
                return R(TyF32(), OpLoad, ac);
            }
            case IlCbufLoad(_, var slot, var offExpr): {
                // Dynamic offset (LDC with Ra≠RZ). offExpr is BYTE
                // offset → word = off>>2 → vec4Idx = word>>2 =
                // off>>4, comp = word&3 = (off>>2)&3. SPIR-V allows
                // non-const index into vector via OpAccessChain.
                var offU = EmitExpr(offExpr);
                var v4i = R(TyU32(), 195/*OpShiftRightLogical*/, offU, ConstU(4));
                var wrd = R(TyU32(), 195, offU, ConstU(2));
                var cmp = R(TyU32(), 199/*OpBitwiseAnd*/, wrd, ConstU(3));
                var ac = R(TyPtr(ScUniform, TyF32()), OpAccessChain,
                    CbufVar(slot), ConstI(0), v4i, cmp);
                return R(TyF32(), OpLoad, ac);
            }
            default:
                Console.Error.WriteLine($"  ‡ EmitExpr: {il.GetType().Name} — {il}");
                return ConstF(0);
        }
    }

    // ── IL → SPIR-V statement walker. ──
    // For VS attr-out at 0x80-0x8c we accumulate components and emit
    // one OpStore to gl_Position at the end (must be vec4).
    readonly uint[] _posComp = new uint[4];

    void EmitStmt(Il il) {
        switch(il) {
            case IlBlock b:
                foreach(var s in b.Body) EmitStmt(s);
                break;
            case IlLet(var tid, var rhs):
                _tmp[tid] = EmitExpr(rhs);
                break;
            case IlNote(var txt):
                // ‡-annotation from MaxwellLift (partial-impl flag).
                // No SPIR-V emit; surface to stderr so the sweep log
                // shows which shaders carry which ‡-notes.
                Console.Error.WriteLine($"  ‡note: {txt}");
                break;

            case IlLoop(var cond, var lbody, var entryGuard): {
                // do{body}while(cond). SPIR-V structured loop:
                //   OpBranch hdr
                //   hdr: OpLoopMerge merge cont None; OpBranch body
                //   body: …; OpBranch cont
                //   cont: c=cond; OpBranchConditional c hdr merge
                //   merge:
                // Reg-state across iterations: SSA (_gpr[N]) breaks
                // (body writes R18; next iter's R18-read needs THIS
                // iter's value, not pre-loop). v0 = OpVariable
                // Function (alloca) for every Gpr+Pred the body
                // touches: pre-loop OpStore current SSA → var;
                // inside body, OpLoad/OpStore the vars; post-loop
                // OpLoad vars → back to SSA. No OpPhi needed.
                // ⚠️ EmitIfFlatten inside the body still uses
                // _gpr[N] SSA — that's WRONG inside a loop. v0:
                // wrap the body in a "var-mode" — temporarily
                // route _gpr/_pred through OpLoad/OpStore.
                //
                // Find regs the body touches (read OR write).
                var touchG = new HashSet<int>();
                var touchP = new HashSet<int>();
                void Scan(Il s) {
                    switch(s) {
                        case IlReadReg(_, RegKind.Gpr, var n) when n != 255: touchG.Add(n); break;
                        case IlReadReg(_, RegKind.Pred, var n) when n != 7: touchP.Add(n); break;
                        case IlWriteReg(RegKind.Gpr, var n, var v) when n != 255: touchG.Add(n); Scan(v); break;
                        case IlWriteReg(RegKind.Pred, var n, var v) when n != 7: touchP.Add(n); Scan(v); break;
                        case IlIf(var c, var t, var e): Scan(c); foreach(var x in t) Scan(x); foreach(var x in e) Scan(x); break;
                        case IlLoop(var c, var b, _): Scan(c); foreach(var x in b) Scan(x); break;
                        case IlBlock(var b): foreach(var x in b) Scan(x); break;
                        case IlBin(_, _, var a, var b): Scan(a); Scan(b); break;
                        case IlUn(_, _, var a): Scan(a); break;
                        case IlCast(_, _, var a): Scan(a); break;
                        case IlIfV(_, var c, var t, var e): Scan(c); Scan(t); Scan(e); break;
                        case IlLet(_, var v): Scan(v); break;
                        case IlCbufLoad(_, _, var o): Scan(o); break;
                        case IlSample(_, var h, var co, _, _): Scan(h); foreach(var x in co) Scan(x); break;
                        case IlVecElem(_, var v, var i): Scan(v); Scan(i); break;
                        case IlIntrin(_, _, var args): foreach(var x in args) Scan(x); break;
                        case IlMufu(_, _, var a): Scan(a); break;
                        case IlAttrLoad or IlAttrStore or IlConst or IlTmp
                            or IlInterp or IlNote or IlExit or IlDiscard
                            or IlBranch or IlUnimpl: break;
                        default: throw new($"‡ IlLoop Scan: {s.GetType().Name}");
                    }
                }
                Scan(cond); foreach(var x in lbody) Scan(x);

                // Allocate Function-storage vars for touched regs.
                // ⚠️ OpVariable Function MUST be in the entry block
                // (first label), NOT here mid-function. v0: emit
                // them into _funcVars (a separate section that
                // assembles right after the entry OpLabel).
                var gVar = new Dictionary<int, uint>();
                var pVar = new Dictionary<int, uint>();
                foreach(var n in touchG) {
                    var id = Id();
                    Emit(_funcVars, OpVariable, TyPtr(ScFunction, TyF32()), id, ScFunction);
                    gVar[n] = id;
                }
                foreach(var n in touchP) {
                    var id = Id();
                    Emit(_funcVars, OpVariable, TyPtr(ScFunction, TyBool()), id, ScFunction);
                    pVar[n] = id;
                }
                // (M2-δ) HARNESS-ONLY loop-iter cap. Synthetic
                // inputs may never satisfy the loop-cond (e.g.
                // sh0272: loops while c[3][w]!=0; synthetic Cbuf
                // returns 0.0625*…≠0 for all w>0 ⟹ infinite).
                // Cap at 4096 iters: counter Function-var,
                // ++ at body-head; cont's branch-cond becomes
                // (cval && counter<4096). Real (non-harness)
                // emit unaffected. lavapipe doesn't have a
                // kill-pipeline-after-Submit op; once Dispatch
                // is in-flight on a diverging shader, the only
                // out is process-kill ⟹ cap in the SPIR-V.
                uint hCtr = 0;
                if(_harness) {
                    hCtr = Id();
                    Emit(_funcVars, OpVariable,
                        TyPtr(ScFunction, TyU32()), hCtr,
                        ScFunction);
                }
                // Pre-loop: store current SSA → vars (BEFORE the
                // entry-guard branch, so skip-arm round-trips
                // identity through the vars too).
                foreach(var (n, id) in gVar) Emit(_func, OpStore, id, Gpr(n));
                foreach(var (n, id) in pVar) Emit(_func, OpStore, id, PredId(n));
                if(_harness)
                    Emit(_func, OpStore, hCtr, ConstU(0));

                // Entry-guard (if(guard){do-while}). null → uncond.
                // SPIR-V: SelectionMerge selM; BranchCond(guard, pre,
                // selM); pre: Branch hdr; …loop…; loopM: Branch selM;
                // selM: …. The LoopMerge's merge-block (loopM) MUST
                // differ from the SelectionMerge's (selM) — nesting.
                var guard = entryGuard;
                uint pre=Id(), hdr=Id(), bodyL=Id(), cont=Id(),
                     loopM=Id(), selM = guard != null ? Id() : loopM;
                if(guard != null) {
                    var gv = EmitExpr(guard);
                    Emit(_func, OpSelectionMerge, selM, 0);
                    Emit(_func, OpBranchConditional, gv, pre, selM);
                } else {
                    Emit(_func, OpBranch, pre);
                }
                Emit(_func, OpLabel, pre);
                Emit(_func, OpBranch, hdr);
                Emit(_func, OpLabel, hdr);
                Emit(_func, OpLoopMerge, loopM, cont, 0);
                Emit(_func, OpBranch, bodyL);
                Emit(_func, OpLabel, bodyL);

                // Body: temporarily route _gpr/_pred through the
                // vars. Save current SSA, install OpLoad-on-demand
                // … but EmitExpr(IlReadReg) calls Gpr(n) which
                // reads _gpr[n] directly. = need a "loop mode"
                // flag that makes Gpr(n) emit OpLoad gVar[n] and
                // SetGpr emit OpStore. Adding _loopGVar/_loopPVar:
                _loopGVar = gVar; _loopPVar = pVar;
                foreach(var x in lbody) EmitStmt(x);
                Emit(_func, OpBranch, cont);
                Emit(_func, OpLabel, cont);
                var cval = EmitExpr(cond);
                if(_harness) {
                    // ctr++; cval &&= (ctr<4096).
                    var c0 = Id();
                    Emit(_func, OpLoad, TyU32(), c0, hCtr);
                    var c1 = Id();
                    Emit(_func, OpIAdd, TyU32(), c1, c0,
                        ConstU(1));
                    Emit(_func, OpStore, hCtr, c1);
                    var lt = Id();
                    Emit(_func, OpULessThan, TyBool(),
                        lt, c1, ConstU(4096));
                    var nc = Id();
                    Emit(_func, OpLogicalAnd, TyBool(),
                        nc, cval, lt);
                    cval = nc;
                }
                _loopGVar = null; _loopPVar = null;

                Emit(_func, OpBranchConditional, cval, hdr, loopM);
                Emit(_func, OpLabel, loopM);
                if(guard != null) {
                    Emit(_func, OpBranch, selM);
                    Emit(_func, OpLabel, selM);
                }

                // Post-loop: load vars → back to SSA (works for
                // BOTH paths: skip-arm = store→load identity).
                foreach(var (n, id) in gVar)
                    _gpr[n] = EmLoad(TyF32(), id);
                foreach(var (n, id) in pVar)
                    _pred[n] = EmLoad(TyBool(), id);
                break;
            }
            case IlWriteReg(RegKind.Gpr, var n, var rhs)
                    when _loopGVar?.ContainsKey(n) == true:
                Emit(_func, OpStore, _loopGVar[n], EmitExpr(rhs));
                break;
            case IlWriteReg(RegKind.Pred, var n, var rhs)
                    when _loopPVar?.ContainsKey(n) == true:
                Emit(_func, OpStore, _loopPVar[n], EmitExpr(rhs));
                break;
            case IlWriteReg(RegKind.Gpr, var n, var rhs):
                if(n != 255) _gpr[n] = EmitExpr(rhs);
                else EmitExpr(rhs);   // RZ sink — eval for side effects
                break;
            case IlWriteReg(RegKind.Pred, var n, var rhs):
                if(n != 7) _pred[n] = EmitExpr(rhs);
                else EmitExpr(rhs);
                break;
            case IlAttrStore(IlConst(_, var offV), var rhs): {
                var off = AsI32(offV);
                var v = EmitExpr(rhs);
                if(_harness) { HarnessStore(off, v); break; }
                if(off >= 0x70 && off < 0x80) {
                    // gl_Position component (VS-out). Accumulate;
                    // composed + stored at IlExit.
                    _posComp[(off - 0x70) / 4] = v;
                } else if(off < 0x70) {
                    // Builtin attr-out: 0x064=Layer, 0x068=ViewportIdx,
                    // 0x06c=PointSize. ‡ v0: drop (no shader in the
                    // 364 writes these as load-bearing yet). v1 =
                    // BuiltIn-decorated int Output vars.
                } else if(off >= 0x80 && off < 0x280) {
                    // User varying out, loc=(off-0x80)/16. Matches
                    // FS-in IPA's same formula → varyings line up.
                    var (loc, comp) = AttrLoc(off, 0x80);
                    Emit(_func, OpStore, IoVar(ScOutput, loc, comp), v);
                } else {
                    // 0x280+ fixed-func space (front/back color,
                    // texcoord, clip-distance). ‡ v0: drop until
                    // a shader uses it (none in the 364 yet).
                    Console.Error.WriteLine($"  ‡ AttrStore off=0x{off:x} (fixed-func space)");
                }
                break;
            }
            case IlExit:
                if(_harness) {
                    // Flush gl_Position-equiv (VS-origin shaders
                    // accumulate to _posComp via off 0x70-0x7c,
                    // but harness intercepts AttrStore — so
                    // _posComp stays empty. ✓ Already in h_out.)
                    // FS-origin: R0-R3 → h_out[0..3] (= oColor).
                    if(_stage == SpvStage.Fragment) {
                        for(var k = 0; k < 4; k++)
                            HarnessStore(k * 4, _gpr[k] != 0
                                ? _gpr[k]
                                : (k == 3 ? ConstF(1) : ConstF(0)));
                    }
                    Emit(_func, OpReturn);
                    break;
                }
                if(_stage == SpvStage.Fragment) {
                    // FS: R0-R3 at exit → oColor.rgba. ‡ v0 = RT0 only;
                    // SPH output mask + multi-RT = M2.5. Components
                    // not written by the shader → 0 (rgb) / 1 (a).
                    var v4 = TyVec(TyF32(), 4);
                    var c = new uint[4];
                    for(var k = 0; k < 4; k++)
                        c[k] = _gpr[k] != 0 ? _gpr[k]
                             : (k == 3 ? ConstF(1) : ConstF(0));
                    var col = Id();
                    Emit(_func, OpCompositeConstruct, v4, col, c[0],c[1],c[2],c[3]);
                    Emit(_func, OpStore, FragOut(), col);
                    Emit(_func, OpReturn);
                    break;
                }
                // VS: flush gl_Position from accumulated _posComp[].
                // PASS-THROUGH (no clip-space remap). The v0.7 negate
                // (designed from §7-oracle's FALSE w=-7.0; oracle
                // skipped FADD-R per kt[22]) is REVERTED. Corrected
                // §7-verify gives w=+1.0 = on-screen as-is. Y-flip /
                // depth-range = viewport state (NvnVulkan), not here.
                if(_posComp.Any(c => c != 0)) {
                    var v4 = TyVec(TyF32(), 4);
                    var c = new uint[4];
                    for(var k = 0; k < 4; k++)
                        c[k] = _posComp[k] != 0 ? _posComp[k]
                             : (k == 3 ? ConstF(1) : ConstF(0));
                    // gl_Position pass-through. ⚠️ The v0.7 negate
                    // (-x,y,0.5·(-w),-w) was designed from the §7
                    // python-oracle's w=-7.0 — which was WRONG (the
                    // oracle silently SKIPPED 3 FADD-R insns; with
                    // them, w=+1.0). Caught by §7-verify-lite at
                    // day-28 ~12:30Z (MaxwellEval vs corrected-oracle
                    // agree on w=+1.0; the v0.7 negate turned that
                    // into w=-1.0 → clipped → wall#6). The negate
                    // was MAKING correct geometry wrong.
                    // ‡ Remaining: NVN y-up vs Vulkan y-down (y-flip)
                    // and z [-1,1] vs [0,1] depth-range — handled by
                    // viewport state (negative height + minDepth/
                    // maxDepth) NOT shader, when stage-3 needs them.
                    var pos = Id();
                    Emit(_func, OpCompositeConstruct, v4, pos,
                        c[0], c[1], c[2], c[3]);
                    Emit(_func, OpStore, PosOut(), pos);
                }
                Emit(_func, OpReturn);
                break;
            case IlIf(var cond, var then, var els): {
                // Maxwell predication → SPIR-V. Two routes per body
                // shape (kt[16] simplest-that-works):
                //
                // (a) FLATTEN via OpSelect: if then-body is purely
                //     IlWriteReg(Gpr/Pred, n, expr) (no side-effects,
                //     no discard, no attr-store), evaluate expr
                //     unconditionally and OpSelect(cond, new, old) →
                //     no control flow, no OpPhi. Covers all the
                //     `@P0 fmul Rd, …` cases in sh0346.
                //
                // (b) BRANCH for IlDiscard / IlAttrStore: real
                //     OpSelectionMerge + OpBranchConditional. The
                //     then-block contains only the discard/store (no
                //     reg-writes that escape → no phi needed).
                //
                // ‡ v0: els must be empty (Maxwell predication has no
                // else-arm at insn level). Mixed bodies (reg-write +
                // discard) ‡-board'd until a shader surfaces it.
                if(els.Count != 0)
                    throw new($"‡ IlIf with non-empty else (v0)");
                // ── FLATTEN via OpSelect. Body can be arbitrary
                //    after Structurize() (nested IlIf, IlLet, IlBlock,
                //    AttrStore, …). Strategy:
                //    - WriteReg(R/Pred): compute rhs, OpSelect(c,
                //      new, old), update _gpr/_pred. = the value-side
                //      effect, conditionally applied without a branch.
                //    - Nested IlIf: AND the conds (= the nested body's
                //      reg-writes are guarded by c && innerC). Recurse
                //      via EmitIfFlatten with the conjunction as the
                //      effective cond. ‡ This evaluates inner conds +
                //      bodies unconditionally — fine for ALU-pure
                //      Maxwell (no side-effects except attr/discard,
                //      handled below). texture-sample-inside-if =
                //      executes uncond, GPR-write select-guarded =
                //      semantically OK (sample is pure; ‡ derivatives
                //      may be wrong but that's a stage-3 concern).
                //    - IlBlock: recurse on its body with same cond.
                //    - AttrStore: ‡ uncond + note (v0).
                //    - Discard: collect; one branch at the end for
                //      ANY-discard-cond.
                //    - IlLet/IlNote/etc: emit uncond (binds/no-op).
                //    - IlBranch/IlExit: throw (= Structurize couldn't
                //      handle this shape; the shader SKIPs). ──
                Il discardCond = null;
                void EmitIfFlatten(Il effC, IReadOnlyList<Il> body) {
                    var ec = EmitExpr(effC);
                    foreach(var s in body) {
                        switch(s) {
                            case IlWriteReg(var rk, var rn, var rhs): {
                                var nv = EmitExpr(rhs);
                                var old = rk == RegKind.Pred ? PredId(rn) : Gpr(rn);
                                var ty = TyForReg(rk);
                                var sel = Id();
                                Emit(_func, OpSelect, ty, sel, ec, nv, old);
                                if(rk == RegKind.Pred) _pred[rn] = sel;
                                else if(rn != 255) _gpr[rn] = sel;
                                break;
                            }
                            case IlIf(var ic, var ith, var iel)
                                    when iel.Count == 0:
                                // Conjunction: c && ic. Both eval'd.
                                EmitIfFlatten(
                                    new IlBin(IlType.U1, BinOp.And, effC, ic),
                                    ith);
                                break;
                            case IlBlock(var bb):
                                EmitIfFlatten(effC, bb);
                                break;
                            case IlNote:
                                EmitStmt(s); break;
                            case IlLet:
                                // Bind unconditionally (the IlTmp use
                                // downstream is via reg-write which
                                // gets select-guarded).
                                EmitStmt(s); break;
                            case IlAttrStore:
                                EmitStmt(s);
                                Console.Error.WriteLine($"  ‡ predicated AttrStore → uncond v0");
                                break;
                            case IlDiscard:
                                discardCond = discardCond == null ? effC
                                    : new IlBin(IlType.U1, BinOp.Or,
                                        discardCond, effC);
                                break;
                            case IlExit:
                                // Predicated EXIT inside a flattened
                                // body. ‡ v0: in FS this is = discard
                                // (early-out before writing oColor =
                                // same as discard for opaque). In VS
                                // it'd be cull-vertex (= no SPIR-V
                                // equiv; ‡-note + ignore). 
                                // sh0204 = the only case = FS.
                                if(_stage == SpvStage.Fragment) {
                                    discardCond = discardCond == null ? effC
                                        : new IlBin(IlType.U1, BinOp.Or,
                                            discardCond, effC);
                                } else {
                                    Console.Error.WriteLine(
                                        $"  ‡ predicated EXIT in VS → ignored v0");
                                }
                                break;
                            case IlLoop:
                                // Loop inside flatten = can't OpSelect
                                // a loop. Emit a REAL branch around it
                                // (effC ? enter-loop : skip). The loop
                                // body's reg-state goes through alloca
                                // vars, so pre-branch OpStore current
                                // SSA + post-merge OpLoad → SSA happens
                                // inside EmitStmt(IlLoop) regardless;
                                // the skip-arm just doesn't enter.
                                // ⚠️ This breaks pure-flatten (we now
                                // have a real branch); subsequent reg-
                                // writes in THIS EmitIfFlatten body
                                // need _gpr[N] to be valid post-merge,
                                // which it IS (IlLoop's post-merge
                                // OpLoads restore SSA from vars; the
                                // skip-arm's _gpr[N] = pre-loop SSA;
                                // ⟹ MISMATCH: the merge-block sees
                                // EITHER post-loop-load OR pre-loop
                                // SSA depending on path = needs OpPhi
                                // OR the vars need to be stored on
                                // BOTH paths). v0.5 = simplest-correct:
                                // do the pre-loop OpStore + post-loop
                                // OpLoad UNCOND (= outside the branch),
                                // and only the OpBranch-into-hdr is
                                // conditional. = if !effC, store→
                                // immediate-load = identity. ✓
                                // ⟹ Implement by adding effC into
                                // the loop's continue-cond as an AND:
                                // first iter checks effC at cont; if
                                // false, exits after 1 body-exec. ‡
                                // STILL WRONG (body executes once
                                // even if !effC). v0.5b proper =
                                // restructure IlLoop to take an
                                // entry-guard. For NOW: emit a real
                                // OpSelectionMerge around EmitStmt
                                // (IlLoop), and accept the OpPhi-
                                // missing on _gpr (= post-merge regs
                                // are the BRANCH-TAKEN values; if
                                // skip-arm taken, _gpr stale). ‡‡
                                //
                                // ACTUALLY: for sh0158 specifically,
                                // the body after the IlIf doesn't
                                // read regs the loop wrote (verified:
                                // loop writes R0-R7,R17-R24; post-
                                // loop @+5a0 reads R10/R6/R7/R19/R12
                                // — R6/R7/R19 ARE loop-written). So
                                // the OpPhi IS needed.
                                //
                                // v0.5 SIMPLEST-CORRECT: emit IlLoop
                                // unconditionally BUT pass effC down
                                // so the loop's pre-store/post-load
                                // happen, AND the loop's hdr→body
                                // branch is `OpBranchConditional
                                // effC body merge` instead of uncond
                                // OpBranch body. = loop body never
                                // runs if !effC, vars round-trip
                                // identity. Need IlLoop to know effC.
                                // ⟹ Wrap: EmitStmt(new IlLoop(
                                //   IlBin(And, effC, origCond), body))
                                // + change IlLoop to ALSO check cond
                                // BEFORE first body-exec (= while not
                                // do-while). For Maxwell's do-while
                                // semantics that's wrong UNLESS effC
                                // is the only thing that's false on
                                // entry. ‡‡‡
                                //
                                // ⟹ IlLoop carries EntryGuard. Re-emit
                                // with effC as the guard. EmitStmt
                                // handles the rest (store-vars BEFORE
                                // guard, load-vars AFTER selM merge =
                                // round-trips identity on skip-arm).
                                {
                                    var lp = (IlLoop) s;
                                    EmitStmt(new IlLoop(lp.Cond, lp.Body,
                                        EntryGuard: effC));
                                }
                                break;
                            case IlBranch:
                                throw new($"‡ IlIf body: {s.GetType().Name}");
                            default:
                                // Unknown — emit uncond (best-effort
                                // for unimpl/etc).
                                EmitStmt(s); break;
                        }
                    }
                }
                EmitIfFlatten(cond, then);

                // BRANCH only for discard.
                if(discardCond != null) {
                    var dc = EmitExpr(discardCond);
                    var lblThen = Id(); var lblMerge = Id();
                    Emit(_func, OpSelectionMerge, lblMerge, 0);
                    Emit(_func, OpBranchConditional, dc, lblThen, lblMerge);
                    Emit(_func, OpLabel, lblThen);
                    Emit(_func, OpKill);
                    Emit(_func, OpLabel, lblMerge);
                }
                break;
            }
            case IlDiscard:
                // (M2-ζ) Harness: OpKill is Fragment-only
                // (invalid in GLCompute). Emit OpReturn —
                // h_out stays NaN-sentinel ⟹ diff-side
                // sees lvN==true; eval-side sets Discarded.
                // ⟹ Both-discarded handled in HarnessDiff.
                if(_harness) {
                    Emit(_func, OpReturn);
                    break;
                }
                // OpKill terminates the invocation (= the then-block).
                // ‡ Vulkan deprecates OpKill in favor of
                // OpTerminateInvocation (SPV_KHR_terminate_invocation),
                // but lavapipe + 1.0 SPIR-V accept OpKill. v0.
                Emit(_func, OpKill);
                break;
            default:
                Console.Error.WriteLine($"  ‡ EmitStmt: {il.GetType().Name} — {il}");
                break;
        }
    }

    // ── Top-level: lift result → SPIR-V module bytes. ──
    public byte[] Compile(SpvStage stage,
            List<(ulong pc, MaxwellGenerator.MaxwellDef def, Il il)> lifted) {
        // Structurize first: predicated-BRA → IlIf(!cond, body).
        // (The flat per-insn list → nested IlIf trees for forward
        // predicated branches; v0 leaves loops/overlap as IlBranch
        // → throws below = the shader SKIPs.)
        var structured = Pagentry.Lifter.MaxwellLift.Structurize(lifted);
        return Compile(stage, structured);
    }

    public byte[] Compile(SpvStage stage, List<Il> body,
            bool harness = false) {
        _harness = harness;
        // Capabilities. (OpMemoryModel emitted AFTER _ext at assembly
        // time — it must come after OpExtInstImport in the binary.)
        Emit(_caps, OpCapability, 1);  // Shader

        // Function: void main(). Body emitted into _func; function
        // header/footer wrap it after the IL walk (so types/vars
        // referenced from the body are declared before the function).
        var fnMain = Id();
        var lblEntry = Id();

        // Walk the structurized IL.
        var sawTopExit = false;
        foreach(var il in body) {
            EmitStmt(il);
            if(il is IlExit) sawTopExit = true;
        }
        // If no top-level IlExit (= it was predicated-only, like
        // sh0204's `@!P0 EXIT` as the LAST insn, which Structurize
        // turned into IlIf(!P0,[discard])) → emit the FS-oColor-
        // flush / VS-glPosition-flush + OpReturn explicitly. The
        // implicit-fall-through case.
        if(!sawTopExit)
            EmitStmt(new IlExit());

        // OpEntryPoint: model, fn, "main", interface vars.
        // Harness ⟹ GLCompute(5) + LocalSize 1,1,1;
        // interface = [] (no Input/Output vars at SPIR-V
        // 1.0; SSBO/UBO are Uniform-class).
        var model = harness ? 5u
                  : stage == SpvStage.Vertex ? 0u : 4u;
        var ep = new List<uint> { model, fnMain };
        // (string + interface appended via EmitS variant below)
        var nameWords = EncodeStr("main");
        ep.AddRange(nameWords);
        ep.AddRange(_interface);
        Emit(_entry, OpEntryPoint, ep.ToArray());
        if(harness)
            // ExecutionMode LocalSize(17) 1 1 1
            Emit(_entry, OpExecutionMode, fnMain, 17, 1, 1, 1);
        else if(stage == SpvStage.Fragment)
            Emit(_exec, OpExecutionMode, fnMain, 7);  // OriginUpperLeft

        // Assemble: header + sections in order + function wrapper.
        // _funcVars (OpVariable Function from IlLoop) MUST follow
        // the entry OpLabel immediately (SPIR-V: all OpVariable in
        // a function must be at the start of its first block).
        var fnHdr = new List<uint>();
        Emit(fnHdr, OpFunction, TyVoid(), fnMain, 0, TyFn(TyVoid()));
        Emit(fnHdr, OpLabel, lblEntry);
        fnHdr.AddRange(_funcVars);
        var fnEnd = new List<uint>();
        Emit(fnEnd, OpFunctionEnd);

        var all = new List<uint> {
            0x07230203,         // magic
            0x00010000,         // version 1.0
            0x00504147,         // generator = 'PAG\0' (Pagentry)
            _nextId,            // bound
            0,                  // schema
        };
        all.AddRange(_caps);
        all.AddRange(_ext);
        // OpMemoryModel must follow ext-imports.
        var mm = new List<uint>();
        Emit(mm, OpMemoryModel, 0, 1);  // Logical, GLSL450
        all.AddRange(mm);
        all.AddRange(_entry);
        all.AddRange(_exec);
        all.AddRange(_debug);
        all.AddRange(_decor);
        all.AddRange(_types);
        all.AddRange(fnHdr);
        all.AddRange(_func);
        all.AddRange(fnEnd);

        var bytes = new byte[all.Count * 4];
        Buffer.BlockCopy(all.ToArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }

    static uint[] EncodeStr(string s) {
        var b = System.Text.Encoding.UTF8.GetBytes(s + "\0");
        var w = new uint[(b.Length + 3) / 4];
        Buffer.BlockCopy(b, 0, w, 0, b.Length);
        return w;
    }
}
