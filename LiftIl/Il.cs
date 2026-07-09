namespace LiftIl;

// Low-level typed IL. One instruction → one IlBlock.
// No register allocation, no calling convention — architectural regs by index,
// memory as flat address space, flags as a single nzcv word. SSA-ish via IlLet/IlTmp.
//
// `IlUnimpl` is the lifter self-‡: the lifter says "I don't model this"
// instead of guessing. Carries the source op + a reason string for the agent reading it.

public enum RegKind { X, W, Sp, Wsp, V, Nzcv, Pc, Sys,
    // Maxwell shader regs. Gpr = R0-R254 (32-bit
    // each, R255=RZ); Pred = P0-P6 (1-bit, P7=PT). Distinct from X/W
    // so IlEval/SPIR-V backend can dispatch without arch-tagging.
    Gpr, Pred,
    // x86 (XFusion). X86 = the 64-bit GPR file (0=RAX 1=RCX 2=RDX 3=RBX
    // 4=RSP 5=RBP 6=RSI 7=RDI 8-15=R8-R15; sub-widths are extract/insert,
    // except 32-bit writes which zero-extend). Eflags: idx=-1 whole word,
    // else the architectural bit (CF=0 PF=2 AF=4 ZF=6 SF=7 DF=10 OF=11).
    // X86Seg = segment base as opaque Sys-like context (0=ES..5=GS).
    // St = x87 stack (0-7, TOP-relative). Xmm = vector file 0-31 (value
    // type carries the width: v128/v256/v512).
    X86, Eflags, X86Seg, St, Xmm }

public abstract record IlType {
    public sealed record Void() : IlType { public override string ToString() => "void"; }
    public sealed record I(bool Signed, int Bits) : IlType {
        public override string ToString() => $"{(Signed ? "i" : "u")}{Bits}";
    }
    public sealed record F(int Bits) : IlType { public override string ToString() => $"f{Bits}"; }
    public sealed record Vec(int Bits) : IlType { public override string ToString() => $"v{Bits}"; }

    public static readonly IlType V0 = new Void();
    public static readonly IlType U1 = new I(false, 1);
    public static readonly IlType U8 = new I(false, 8);
    public static readonly IlType U32 = new I(false, 32);
    public static readonly IlType U64 = new I(false, 64);
}

public enum BinOp { Add, Sub, Mul, UDiv, SDiv, URem, SRem, And, Or, Xor, Shl, Shr, Sar, Ror,
                    Eq, Ne, Ult, Ule, Ugt, Uge, Slt, Sle, Sgt, Sge,
                    FMin, FMax }   // float min/max (Maxwell FMNMX)
public enum UnOp { Neg, Not, Clz, Rbit, Popcnt, Sqrt, Abs, Floor, Ceil, Round }
public enum CastKind { Zext, Sext, Trunc, Bitcast, IToF, FToI, FExt, FTrunc,
                       SToF, UToF, FToSI, FToUI }   // Maxwell I2F/F2I (sign-aware)
public enum BranchKind { Jmp, Call, Ret, CondJmp, Fallthrough }

public abstract record Il(IlType Ty) {
    public sealed override string ToString() => Pp(this, 0);
    static string Ind(int n) => new(' ', n * 2);

    static string Pp(Il il, int d) => il switch {
        IlConst(var t, var b) => $"({t} #{(b > ulong.MaxValue ? b.ToString() : ((ulong)b).ToString("x"))})",
        IlReadReg(var t, var k, var i) => $"({t} {RegName(k, i)})",
        IlReadPc(var t) => $"({t} pc)",
        IlTmp(var t, var id) => $"({t} %{id})",
        IlBin(var t, var op, var l, var r) => $"({t} {op.ToString().ToLower()} {Pp(l, d)} {Pp(r, d)})",
        IlUn(var t, var op, var x) => $"({t} {op.ToString().ToLower()} {Pp(x, d)})",
        IlCast(var t, var k, var x) => $"({t} {k.ToString().ToLower()} {Pp(x, d)})",
        IlLoad(var t, var a) => $"({t} load {Pp(a, d)})",
        IlIfV(var t, var c, var th, var el) => $"({t} if {Pp(c, d)} {Pp(th, d)} {Pp(el, d)})",
        IlBlock(var body) => body.Count == 1 ? Pp(body[0], d)
            : $"(block\n{string.Join("\n", body.Select(s => Ind(d+1) + Pp(s, d+1)))})",
        IlLet(var id, var v) => $"(let %{id} = {Pp(v, d)})",
        IlWriteReg(var k, var i, var v) => $"({RegName(k, i)} := {Pp(v, d)})",
        IlStore(var a, var v) => $"(store {Pp(a, d)} {Pp(v, d)})",
        IlBranch(var k, var tgt, var c) => c == null
            ? $"({k.ToString().ToLower()} {Pp(tgt, d)})"
            : $"({k.ToString().ToLower()} {Pp(tgt, d)} if {Pp(c, d)})",
        IlIf(var c, var th, var el) => $"(if {Pp(c, d)}\n{Ind(d+1)}{Pp(new IlBlock(th), d+1)}\n{Ind(d+1)}{Pp(new IlBlock(el), d+1)})",
        IlVecBuild(var b, var et, var es) => $"(v{b} build.{et} {string.Join(" ", es.Select(e => Pp(e, d)))})",
        IlVecAll(var b, var et, var s) => $"(v{b} all.{et} {Pp(s, d)})",
        IlVecElem(var t, var v, var i) => $"({t} elem {Pp(v, d)} {Pp(i, d)})",
        IlVecZeroTop(var b, var v) => $"(v{b} zerotop {Pp(v, d)})",
        IlVecBin(var b, var et, var op, var l, var r) => $"(v{b} v{op.ToString().ToLower()}.{et} {Pp(l, d)} {Pp(r, d)})",
        IlVecUn(var b, var et, var op, var x) => $"(v{b} v{op.ToString().ToLower()}.{et} {Pp(x, d)})",
        IlVecInsert(var ri, var et, var i, var val) => $"(V{ri}[{Pp(i, d)}].{et} := {Pp(val, d)})",
        IlSyscall(var n) => $"(svc {Pp(n, d)})",
        IlUnimpl(var src, var why) => $"(‡unimpl {src}{(why != null ? $" — {why}" : "")})",
        // shader-IL
        IlAttrLoad(var t, var o) => $"({t} attr-in {Pp(o, d)})",
        IlAttrStore(var o, var v) => $"(attr-out {Pp(o, d)} := {Pp(v, d)})",
        IlCbufLoad(var t, var s, var o) => $"({t} c[{s}][{Pp(o, d)}])",
        IlInterp(var t, var o, var m, var mul) => $"({t} interp.{m} {Pp(o, d)} × {Pp(mul, d)})",
        IlSample(var t, var h, var cs, var dim, var mask) => $"({t} sample{dim}d.{mask:x} {Pp(h, d)} [{string.Join(" ", cs.Select(c => Pp(c, d)))}])",
        IlMufu(var t, var op, var x) => $"({t} mufu.{MufuName(op)} {Pp(x, d)})",
        IlExit(var c) => c == null ? "(exit)" : $"(exit if {Pp(c, d)})",
        IlDiscard(var c) => c == null ? "(discard)" : $"(discard if {Pp(c, d)})",
        // (?IlIntrin) previously read as
        // "unresolved node" when it was just printer-miss.
        // Print the actual intrinsic so --lift output is
        // traceable (PackHalf2x16/UnpackHalf2x16/etc).
        IlIntrin(var t, var nm, var args)
            => $"({t} intrin.{nm} {string.Join(" ", args.Select(a => Pp(a, d)))})",
        _ => $"(?{il.GetType().Name})",
    };

    static string MufuName(int op) => op switch {
        0=>"cos",1=>"sin",2=>"ex2",3=>"lg2",4=>"rcp",5=>"rsq",
        6=>"rcp64h",7=>"rsq64h",8=>"sqrt",_=>$"?{op}" };

    static string RegName(RegKind k, int i) => k switch {
        RegKind.X => $"X{i}", RegKind.W => $"W{i}",
        RegKind.Sp => "SP", RegKind.Wsp => "WSP",
        RegKind.V => $"V{i}",
        RegKind.Nzcv => i < 0 ? "NZCV" : $"NZCV.{"nzcv"[i]}",
        RegKind.Pc => "PC",
        RegKind.Sys => $"S{(i>>14)&3}_{(i>>11)&7}_C{(i>>7)&15}_C{(i>>3)&15}_{i&7}",
        RegKind.Gpr => i == 255 ? "RZ" : $"R{i}",
        RegKind.Pred => i == 7 ? "PT" : $"P{i}",
        RegKind.X86 => i < 8 ? new[]{"RAX","RCX","RDX","RBX","RSP","RBP","RSI","RDI"}[i] : $"R{i}",
        RegKind.Eflags => i < 0 ? "EFLAGS" : $"EFLAGS.{i switch { 0=>"C", 2=>"P", 4=>"A", 6=>"Z", 7=>"S", 10=>"D", 11=>"O", _=>i.ToString() }}",
        RegKind.X86Seg => new[]{"ES","CS","SS","DS","FS","GS"}[i & 7],
        RegKind.St => $"ST{i}",
        RegKind.Xmm => $"XMM{i}",
        _ => $"{k}{i}",
    };
}

// — values —
public record IlConst(IlType Ty, UInt128 Bits) : Il(Ty);
public record IlReadReg(IlType Ty, RegKind Kind, int Idx) : Il(Ty);
public record IlReadPc(IlType Ty) : Il(Ty);
public record IlTmp(IlType Ty, int Id) : Il(Ty);
public record IlBin(IlType Ty, BinOp Op, Il L, Il R) : Il(Ty);
public record IlUn(IlType Ty, UnOp Op, Il X) : Il(Ty);
public record IlCast(IlType Ty, CastKind Kind, Il X) : Il(Ty);
public record IlLoad(IlType Ty, Il Addr) : Il(Ty);
public record IlIfV(IlType Ty, Il Cond, Il Then, Il Else) : Il(Ty); // value-producing cond (csel)
// ── Annotations (‡-marks that flow through to SPIR-V emit as no-ops
// + a stderr note; for v0 partial-impl flagging). ──
public record IlNote(string Text) : Il(IlType.U1);
// Generic intrinsic — SpirvEmit dispatches by Name to GLSL.std.450
// (UnpackHalf2x16/PackHalf2x16/etc) or core ops. Args eval'd, result
// type per Ty. The "I don't want a new IL node for every builtin"
// escape hatch.
public record IlIntrin(IlType Ty, string Name, params Il[] Args) : Il(Ty);

// — vectors (128-bit lane-typed) —
// ElemTy carries lane type/width for ops where it isn't recoverable from
// children. IlVecBuild = (vector e0..eN); IlVecAll = splat scalar to all lanes;
// IlVecElem = extract scalar lane; IlVecZeroTop = zero high half.
// Bits parametrized: 128 for aarch64 NEON, 256/512 for AVX ymm/zmm.
public record IlVecBuild(int Bits, IlType ElemTy, IReadOnlyList<Il> Elems) : Il(new IlType.Vec(Bits));
public record IlVecAll(int Bits, IlType ElemTy, Il Scalar) : Il(new IlType.Vec(Bits));
public record IlVecElem(IlType Ty, Il Vec, Il Idx) : Il(Ty);
public record IlVecZeroTop(int Bits, Il Vec) : Il(new IlType.Vec(Bits));
public record IlVecBin(int Bits, IlType ElemTy, BinOp Op, Il L, Il R) : Il(new IlType.Vec(Bits));
public record IlVecUn(int Bits, IlType ElemTy, UnOp Op, Il X) : Il(new IlType.Vec(Bits));

// — effect: insert one lane into Vd in place —
public record IlVecInsert(int RegIdx, IlType ElemTy, Il Idx, Il Val) : Il(IlType.V0);
public record IlSyscall(Il Num) : Il(IlType.V0);

// — effects (Ty = Void) —
public record IlBlock(IReadOnlyList<Il> Body) : Il(IlType.V0);
public record IlLet(int Id, Il Val) : Il(IlType.V0);
public record IlWriteReg(RegKind Kind, int Idx, Il Val) : Il(IlType.V0);
public record IlStore(Il Addr, Il Val) : Il(IlType.V0);
public record IlBranch(BranchKind Kind, Il Target, Il Cond = null) : Il(IlType.V0);
public record IlIf(Il Cond, IReadOnlyList<Il> Then, IReadOnlyList<Il> Else) : Il(IlType.V0);
// if(EntryGuard) do { Body } while(Cond). EntryGuard=null → uncond
// do-while. Maxwell backward-BRA inside an IlIf → EntryGuard=if-cond.
// SpirvEmit: store-vars → SelectionMerge → BranchCond(guard, pre,
// merge) → pre: Branch hdr → hdr: LoopMerge(merge2,cont) → body →
// cont: BranchCond(cond, hdr, merge2) → merge2: Branch merge →
// merge: load-vars. Reg-state round-trips through alloca vars on
// BOTH paths (skip = store→immediate-load = identity).
public record IlLoop(Il Cond, IReadOnlyList<Il> Body, Il EntryGuard = null) : Il(IlType.V0);
public record IlUnimpl(string SourceOp, string Why = null) : Il(IlType.V0);

// ─── Shader-IL (Maxwell tier-3 M1) ───
// These model GPU-shader concepts that don't reduce to load/store on a
// flat address space: attribute I/O (the rasterizer/vertex-fetch
// boundary), constant buffers (driver-bound, indexed by slot+offset),
// interpolation (FS-only, the rasterizer's per-pixel value of a VS
// output), and texture sampling. M2 (IL→SPIR-V) maps each to its
// SPIR-V op directly (OpLoad on Input/Output storage class,
// OpAccessChain into a Uniform block, OpImageSampleImplicitLod, etc.).
//
// Off is a BYTE offset into the attribute/cbuf space (Maxwell native;
// SPIR-V will /4 to a component index). Gpr reads/writes use
// IlReadReg/IlWriteReg with RegKind.Gpr; these are for the I/O that
// ISN'T a register file.

public record IlAttrLoad(IlType Ty, Il Off) : Il(Ty);          // Ald: vertex input / inter-stage in
public record IlAttrStore(Il Off, Il Val) : Il(IlType.V0);     // Ast: vertex output / inter-stage out
public record IlCbufLoad(IlType Ty, int Slot, Il Off) : Il(Ty);// c[slot][off] — slot is hw-cbuf index
public record IlInterp(IlType Ty, Il Off, int Mode, Il Mul) : Il(Ty);  // Ipa: interp(attr) × mul
// Texture sample. Coords = spatial coords (1-3 per Dim) ++
// [array-layer if Array] ++ [depth-ref if Depth] ++ [lod if HasLod].
// SampKind packs: bits[0:2]=dim(1/2/3/cube=4) | bit4=array | bit5=
// depth-compare | bit6=explicit-lod. CompMask = which RGBA components
// the caller wants (MaxwellLift routes to GPRs via IlVecElem).
public record IlSample(IlType Ty, Il Handle, IReadOnlyList<Il> Coords, int SampKind, int CompMask) : Il(Ty);
public static class SampKind {
    public const int D1=1, D2=2, D3=3, Cube=4,
        Array=0x10, Depth=0x20, HasLod=0x40, LodZero=0x80;
}
public record IlMufu(IlType Ty, int Op, Il X) : Il(Ty);        // rcp/rsq/sin/cos/lg2/ex2/sqrt
public record IlExit(Il Cond = null) : Il(IlType.V0);          // shader thread exit (predicated)
public record IlDiscard(Il Cond = null) : Il(IlType.V0);       // Kil: FS discard
