namespace MaxwellShader;

// §7-verify-lite: IL evaluator for Maxwell-lifted shaders. Walks the
// structurized IL (MaxwellLift.LiftShader → Structurize) over a
// Maxwell-shaped state (gpr/pred/cbuf/attr-in/attr-out). Verify
// target = the §7 python-oracle's known-correct sh0349 values
// (gl_Position=(-0.145,0.118,-7.20,-7.0), out_0=(0.3696,0.1172,…),
// out_8=(0.831×3,0.996)).
//
// ⚠️ Epistemic caveat: this evaluator + MaxwellLift share the SAME
// understanding-of-SASS-semantics (both written by barrow from the
// same InstDecoders.cs reads). Disagreement = MaxwellLift didn't
// emit what I MEANT (= field-position-class bugs, the wall#3
// modifier-bit class). Agreement ≠ correctness (= both may share
// the same misunderstanding; the ‡-notes mark where). True
// eval_native = stage-3 pixel-oracle OR sera-hw OR 3rd-party.

using System;
using System.Collections.Generic;
using System.Linq;

public class MaxwellState {
    // GPRs stored as uint (typeless 32-bit; bitcast at use-site).
    // R255 = RZ = 0. P7 = PT = true.
    public uint[] R = new uint[256];
    public bool[] P = new bool[8] { false,false,false,false,false,false,false,true };
    public bool Discarded;
    public Dictionary<int, float> AttrOut = new();   // byte-off → value
    // Callbacks for env-supplied data:
    public Func<int, int, float> Cbuf = (slot, woff) => 0;     // c[slot][word]
    public Func<int, float> AttrIn = off => 0;                  // a[off]
    public Func<int, int, float[], float[]> Sample =
        (handle, sk, coords) => new float[]{0,0,0,1};            // tex sample (rgba)

    public float Rf(int n) => n == 255 ? 0f : BitConverter.UInt32BitsToSingle(R[n]);
    public void Wf(int n, float v) { if(n != 255) R[n] = BitConverter.SingleToUInt32Bits(v); }
}

public class MaxwellEval {
    readonly MaxwellState S;
    readonly Dictionary<int, object> _tmp = new();   // IlLet/IlTmp (float or float[])
    public static bool Trace =
        Environment.GetEnvironmentVariable("MAXEVAL_TRACE") == "1";

    public MaxwellEval(MaxwellState s) => S = s;

    public static MaxwellState Run(IEnumerable<Il> body, MaxwellState s) {
        var ev = new MaxwellEval(s);
        try { foreach(var il in body) ev.Stmt(il); }
        catch(MaxwellExit) { }
        return s;
    }

    class MaxwellExit : Exception { }

    void Stmt(Il il) {
        switch(il) {
            case IlBlock b: foreach(var c in b.Body) Stmt(c); break;
            case IlNote: break;
            case IlUnimpl u: throw new($"‡unimpl in eval: {u.SourceOp}");
            case IlExit: throw new MaxwellExit();
            case IlDiscard: S.Discarded = true; throw new MaxwellExit();
            case IlLet l: _tmp[l.Id] = Expr(l.Val); break;
            case IlWriteReg(RegKind.Gpr, var n, var v): {
                if(n == 255) break;
                var ev = Expr(v);
                S.R[n] = U(ev);
                if(Trace) Console.Error.WriteLine(
                    $"    R{n} := {F(ev),9:F4}");
                break;
            }
            case IlWriteReg(RegKind.Pred, var n, var v):
                if(n != 7) S.P[n] = B(Expr(v)); break;
            case IlAttrStore(IlConst(_, var off), var v):
                S.AttrOut[(int)(uint)off] = F(Expr(v)); break;
            case IlIf(var c, var th, var el):
                foreach(var s in (B(Expr(c)) ? th : el)) Stmt(s);
                break;
            case IlLoop(var c, var lb, var g): {
                if(g != null && !B(Expr(g))) break;
                var iters = 0;
                do {
                    foreach(var s in lb) Stmt(s);
                    if(++iters > 65536) throw new("‡ loop >64K iters");
                } while(B(Expr(c)));
                break;
            }
            case IlBranch:
                throw new($"‡ MaxwellEval: raw IlBranch (Structurize miss)");
            default:
                throw new($"‡ MaxwellEval Stmt: {il.GetType().Name}");
        }
    }

    // Values are boxed: float | uint | bool | float[] (vec). Type
    // tracked by the IL node's Ty; conversions explicit.
    static float F(object o) => o switch {
        float f => f, uint u => BitConverter.UInt32BitsToSingle(u),
        bool b => b ? 1f : 0f,
        _ => throw new($"F({o?.GetType().Name})") };
    static uint U(object o) => o switch {
        uint u => u, float f => BitConverter.SingleToUInt32Bits(f),
        bool b => b ? 1u : 0u,
        _ => throw new($"U({o?.GetType().Name})") };
    static bool B(object o) => o switch {
        bool b => b, uint u => u != 0, float f => f != 0,
        _ => throw new($"B({o?.GetType().Name})") };

    object Expr(Il e) {
        switch(e) {
            case IlConst(var ty, var v):
                // ⚠️ MUST box explicitly per-branch — C# ternary
                // unifies float|uint → float (implicit uint→float
                // conversion!), so `cond ? float : uint` returns
                // float ALWAYS. Caught by §7-verify trace day-28
                // ~12:00Z (cbuf[5][278134784] = SingleToUInt32Bits
                // (52.0f)/4).
                if(ty == IlType.U1) return (uint)v != 0;
                if(ty is IlType.F) return BitConverter.UInt32BitsToSingle((uint)v);
                return (uint)v;
            case IlReadReg(_, RegKind.Gpr, var n):
                return n == 255 ? 0f : S.Rf(n);
            case IlReadReg(_, RegKind.Pred, var n):
                return n == 7 ? true : S.P[n];
            case IlTmp(_, var id): return _tmp[id];
            case IlAttrLoad(_, IlConst(_, var off)):
                return S.AttrIn((int)(uint)off);
            case IlCbufLoad(_, var slot, var offE): {
                // off is BYTE offset (MaxwellLift ×4'd it for arith
                // C-forms; LDC passes it straight). Word-index = /4.
                if(Trace && offE is IlConst(var t2, var v2))
                    Console.Error.WriteLine(
                        $"      [IlCbufLoad slot={slot} offE.ty={t2} bits={v2} (uint)bits={(uint)v2}]");
                var ev = Expr(offE);
                var bo = (int) U(ev);
                if(Trace) Console.Error.WriteLine(
                    $"      [→ ev={ev}({ev?.GetType().Name}) bo={bo} woff={bo/4}]");
                return S.Cbuf(slot, bo / 4);
            }
            case IlInterp(_, IlConst(_, var off), var mode, var w):
                // FS interpolate: v0 = treat as AttrIn (no perspective
                // correction since we're scalar-eval, not raster).
                // (M2-ζ′) IlInterp's semantics ARE interp(off)×mul
                // when mode==1 (the IL carries the mul; SpirvEmit
                // applies it). The prior comment "done by caller"
                // was WRONG (no separate FMUL in lift output —
                // it's IlInterp.Mul). sh0022 ratio 7.69=rcp(0.13)
                // = lvp applied mul, this didn't.
                return mode == 1
                    ? S.AttrIn((int)(uint)off) * F(Expr(w))
                    : S.AttrIn((int)(uint)off);
            case IlMufu(_, var op, var x): {
                var v = F(Expr(x));
                return op switch {
                    0 => MathF.Cos(v), 1 => MathF.Sin(v),
                    2 => MathF.Pow(2, v), 3 => MathF.Log2(v),
                    4 => 1f / v,         // rcp
                    5 => 1f / MathF.Sqrt(v),  // rsq
                    8 => MathF.Sqrt(v),
                    _ => throw new($"‡ Mufu op={op}") };
            }
            case IlSample(_, IlConst(_, var h), var co, var sk, _): {
                var coords = co.Select(c => F(Expr(c))).ToArray();
                return S.Sample((int)(uint)h, sk, coords);
            }
            case IlVecElem(_, var v, IlConst(_, var idx)): {
                var vec = Expr(v);
                if(vec is float[] fa) return fa[(int)(uint)idx];
                throw new($"‡ VecElem on {vec?.GetType().Name}");
            }
            case IlIntrin(_, var nm, var args): {
                var av = args.Select(Expr).ToArray();
                return nm switch {
                    "UnpackHalf2x16" => UnpackH2(U(av[0])),
                    "PackHalf2x16" => PackH2(F(av[0]), F(av[1])),
                    _ => throw new($"‡ Intrin {nm}") };
            }
            case IlIfV(_, var c, var t, var el):
                return B(Expr(c)) ? Expr(t) : Expr(el);
            case IlUn(var ty, var op, var x): {
                var v = Expr(x);
                if(ty == IlType.U32) return op switch {
                    UnOp.Neg => (uint)(-(int)U(v)), UnOp.Not => ~U(v),
                    _ => throw new($"‡ Un U32 {op}") };
                if(ty == IlType.U1) return op switch {
                    UnOp.Not => !B(v), _ => throw new($"‡ Un U1 {op}") };
                return op switch {
                    UnOp.Neg => -F(v), UnOp.Abs => MathF.Abs(F(v)),
                    UnOp.Floor => MathF.Floor(F(v)),
                    UnOp.Ceil => MathF.Ceiling(F(v)),
                    UnOp.Round => MathF.Round(F(v)),
                    _ => throw new($"‡ Un F {op}") };
            }
            case IlCast(var ty, var ck, var x): {
                var v = Expr(x);
                return ck switch {
                    CastKind.Bitcast => ty == IlType.U32 ? (object)U(v) : F(v),
                    CastKind.SToF => (float)(int)U(v),
                    CastKind.UToF => (float)U(v),
                    CastKind.FToSI => (uint)(int)F(v),
                    CastKind.FToUI => (uint)F(v),
                    _ => throw new($"‡ Cast {ck}") };
            }
            case IlBin(var ty, var op, var a, var b): {
                var va = Expr(a); var vb = Expr(b);
                if(ty == IlType.U32) {
                    var ua = U(va); var ub = U(vb);
                    return op switch {
                        BinOp.Add => ua + ub, BinOp.Sub => ua - ub,
                        BinOp.Mul => ua * ub,
                        BinOp.And => ua & ub, BinOp.Or => ua | ub,
                        BinOp.Xor => ua ^ ub,
                        BinOp.Shl => ua << (int)ub,
                        BinOp.Shr => ua >> (int)ub,
                        // (M2-δ) Sar = arithmetic-right (sext).
                        // C# >> on int is arithmetic ⟹ cast.
                        BinOp.Sar => (uint)((int)ua >> (int)(ub & 31)),
                        _ => throw new($"‡ Bin U32 {op}") };
                }
                if(ty == IlType.U1) {
                    // Discriminate by operand-A type (same as SpirvEmit).
                    if(a.Ty == IlType.U32) {
                        var ua = U(va); var ub = U(vb);
                        int sa = (int)ua, sb = (int)ub;
                        return op switch {
                            BinOp.Eq => ua == ub, BinOp.Ne => ua != ub,
                            BinOp.Slt => sa < sb, BinOp.Sle => sa <= sb,
                            BinOp.Sgt => sa > sb, BinOp.Sge => sa >= sb,
                            BinOp.Ult => ua < ub, BinOp.Ule => ua <= ub,
                            BinOp.Ugt => ua > ub, BinOp.Uge => ua >= ub,
                            BinOp.And => B(va) && B(vb),
                            BinOp.Or  => B(va) || B(vb),
                            BinOp.Xor => B(va) != B(vb),
                            _ => throw new($"‡ Bin U1(int) {op}") };
                    }
                    if(a.Ty == IlType.U1) {
                        return op switch {
                            BinOp.And => B(va) && B(vb),
                            BinOp.Or  => B(va) || B(vb),
                            BinOp.Xor => B(va) != B(vb),
                            _ => throw new($"‡ Bin U1(bool) {op}") };
                    }
                    var fa = F(va); var fb = F(vb);
                    return op switch {
                        BinOp.Eq => fa == fb, BinOp.Ne => fa != fb,
                        BinOp.Slt => fa < fb, BinOp.Sle => fa <= fb,
                        BinOp.Sgt => fa > fb, BinOp.Sge => fa >= fb,
                        _ => throw new($"‡ Bin U1(f) {op}") };
                }
                // F32
                var xa = F(va); var xb = F(vb);
                return op switch {
                    BinOp.Add => xa + xb, BinOp.Sub => xa - xb,
                    BinOp.Mul => xa * xb,
                    BinOp.FMin => MathF.Min(xa, xb),
                    BinOp.FMax => MathF.Max(xa, xb),
                    _ => throw new($"‡ Bin F32 {op}") };
            }
            default:
                throw new($"‡ MaxwellEval Expr: {e.GetType().Name}");
        }
    }

    static float[] UnpackH2(uint u) => new[] {
        (float)BitConverter.UInt16BitsToHalf((ushort)(u & 0xFFFF)),
        (float)BitConverter.UInt16BitsToHalf((ushort)(u >> 16)) };
    static uint PackH2(float lo, float hi) =>
        (uint)BitConverter.HalfToUInt16Bits((Half)lo)
        | ((uint)BitConverter.HalfToUInt16Bits((Half)hi) << 16);
}
