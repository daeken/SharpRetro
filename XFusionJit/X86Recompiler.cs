using JitBase;
using LiftIl;

namespace XFusionJit;
using XFusionCpu;

/// XF-6: the recompiler backend as a GENERIC IlBlock→IBuilder emitter — not
/// per-insn generated code (that's the aarch64 approach: 12KL of casting).
/// Walks the tree X86Lifter already produces; one semantics source stays
/// true (.isa → IlLower → {X86Machine exec | this emit}); X86Machine oracles
/// it node-for-node (same tree, interpret-vs-emit state diff = the test).
///
/// Value domain: uniform IRuntimeValue<ulong>, MASKED at width boundaries
/// (exactly X86Machine's model — every op computes at 64, result masked to
/// the node's IlType width). Signed ops = Cast<long>() round-trip. Bool
/// (u1) stays ulong 0/1 (comparisons emit Ternary(cmp, 1, 0)).
///
/// Host wires: subclass, override Branch/Intrinsic/Load/Store, drive
/// RecompileOne per insn until a branch ends the block.
public class X86Recompiler {
	protected IBuilder<ulong> B;
	protected IStructRef<X86State> S;
	protected RuntimeIndexer<X86State, ulong> Gpr, SegBase;
	readonly Dictionary<int, IRuntimeValue<ulong>> _tmps = [];

	public bool Branched;  // set when the emitted block branched (host ends the compile-block)

	/// Emit one instruction's IlBlock. Returns true on decode/emit success.
	public bool RecompileOne(IBuilder<ulong> builder, IStructRef<X86State> state,
		ReadOnlySpan<byte> code, ulong pc, XMode mode) {
		if(!Disassembler.DecodeInsn(code, mode, out var d)) return false;
		var block = X86Lifter.Lift(in d, pc, mode);
		if(block == null) return false;
		B = builder; S = state;
		Gpr = new("Gpr", S, 0x00);
		SegBase = new("SegBase", S, 0x90);
		_tmps.Clear();
		Branched = false;
		EmitBlock(block.Body);
		if(!Branched)  // fallthrough: pc += len
			S.SetField("Rip", 0x80, B.LiteralValue(pc + (ulong) d.Len));
		return true;
	}

	protected void EmitBlock(IReadOnlyList<Il> body) {
		foreach(var st in body) Emit(st);
	}

	IRuntimeValue<ulong> Flags() => S.GetField<ulong>("Flags", 0x88);
	void SetFlags(IRuntimeValue<ulong> v) => S.SetField("Flags", 0x88, v);

	void Emit(Il st) {
		switch(st) {
			case IlLet(var id, var e):
				_tmps[id] = Eval(e).Store();
				break;
			case IlWriteReg(RegKind.X86, var i, var v):
				Gpr[i] = Eval(v);
				break;
			case IlWriteReg(RegKind.Eflags, var bit, var v):
				if(bit < 0) SetFlags(Eval(v).Or(B.LiteralValue(2UL)));
				else SetFlags(Flags().And(B.LiteralValue(~(1UL << bit)))
					.Or(Eval(v).And(B.LiteralValue(1UL)).LeftShift(B.LiteralValue((ulong) bit))));
				break;
			case IlWriteReg(RegKind.X86Seg, var i, var v):
				// runtime seg-load (real-mode base=<<4) is host territory — v1 sets base.
				SegBase[i] = Eval(v);
				break;
			case IlStore(var addr, var v):
				Store(Eval(addr), Eval(v), (v.Ty as IlType.I)?.Bits ?? 64);
				break;
			case IlBranch(var kind, var target, var cond): {
				Branched = true;
				var t = Eval(target);
				if(cond == null) Branch(kind, t);
				else B.If(EvalBool(cond), () => Branch(kind, t), () => { /* fallthrough — host emits pc+=len */ });
				break;
			}
			case IlIf(var c, var then, var els):
				B.If(EvalBool(c), () => EmitBlock(then), () => EmitBlock(els));
				break;
			case IlIntrin(_, var name, var args):
				Intrinsic(name, args.Select(Eval).ToArray());
				break;
			case IlNote: break;
			default: throw new NotSupportedException($"emit {st.GetType().Name}");
		}
	}

	IRuntimeValue<ulong> Eval(Il e) {
		switch(e) {
			case IlConst(var ty, var bits):
				return B.LiteralValue(MaskC((ulong) bits, W(ty)));
			case IlReadReg(_, RegKind.X86, var i):
				return Gpr[i];
			case IlReadReg(_, RegKind.Eflags, var bit):
				return bit < 0 ? Flags()
					: Flags().RightShift(B.LiteralValue((ulong) bit)).And(B.LiteralValue(1UL));
			case IlReadReg(_, RegKind.X86Seg, var i):
				return SegBase[i];
			case IlReadPc:
				return S.GetField<ulong>("Rip", 0x80);
			case IlTmp(_, var id):
				return _tmps[id];
			case IlBin(var ty, var op, var l, var r): {
				var (a, b) = (Eval(l), Eval(r));
				var w = W(ty);
				var lw = W(l.Ty);
				var v = op switch {
					BinOp.Add => a.Add(b), BinOp.Sub => a.Sub(b), BinOp.Mul => a.Mul(b),
					BinOp.UDiv => a.Div(b), BinOp.URem => a.Mod(b),
					BinOp.SDiv => Sx(a, lw).Div(Sx(b, W(r.Ty))).Cast<ulong>(),
					BinOp.SRem => Sx(a, lw).Mod(Sx(b, W(r.Ty))).Cast<ulong>(),
					BinOp.And => a.And(b), BinOp.Or => a.Or(b), BinOp.Xor => a.Xor(b),
					BinOp.Shl => a.LeftShift(b),
					BinOp.Shr => Mask(a, lw).RightShift(b),
					BinOp.Sar => Sx(a, lw).RightShift(b.Cast<long>()).Cast<ulong>(),
					BinOp.Ror => Ror(Mask(a, lw), b, lw),
					BinOp.Eq or BinOp.Ne or BinOp.Ult or BinOp.Ule or BinOp.Ugt or BinOp.Uge
						or BinOp.Slt or BinOp.Sle or BinOp.Sgt or BinOp.Sge
						=> Cmp(op, a, b, lw),
					_ => throw new NotSupportedException($"binop {op}")
				};
				return Mask(v, w);
			}
			case IlUn(var ty, var op, var x): {
				var a = Eval(x);
				var v = op switch {
					UnOp.Neg => B.LiteralValue(0UL).Sub(a),
					UnOp.Not => a.Not(),
					_ => throw new NotSupportedException($"unop {op}")
				};
				return Mask(v, W(ty));
			}
			case IlCast(var ty, var kind, var x): {
				var a = Eval(x);
				var w = W(ty);
				return kind switch {
					CastKind.Zext or CastKind.Trunc or CastKind.Bitcast => Mask(a, w),
					CastKind.Sext => Mask(Sx(a, W(x.Ty)).Cast<ulong>(), w),
					_ => throw new NotSupportedException($"cast {kind}")
				};
			}
			case IlLoad(var ty, var addr):
				return Load(Eval(addr), W(ty));
			case IlIfV(_, var c, var t, var f):
				return B.Ternary(EvalBool(c), Eval(t), Eval(f));
			default:
				throw new NotSupportedException($"eval {e.GetType().Name}");
		}
	}

	IRuntimeValue<bool> EvalBool(Il e) => Eval(e).And(B.LiteralValue(1UL)).NE(B.LiteralValue(0UL));

	IRuntimeValue<ulong> Cmp(BinOp op, IRuntimeValue<ulong> a, IRuntimeValue<ulong> b, int lw) {
		var (am, bm) = (Mask(a, lw), Mask(b, lw));
		var (as_, bs) = (Sx(a, lw), Sx(b, lw));
		var c = op switch {
			BinOp.Eq => am.EQ(bm), BinOp.Ne => am.NE(bm),
			BinOp.Ult => am.LT(bm), BinOp.Ule => am.LTE(bm),
			BinOp.Ugt => bm.LT(am), BinOp.Uge => bm.LTE(am),
			BinOp.Slt => as_.LT(bs), BinOp.Sle => as_.LTE(bs),
			BinOp.Sgt => bs.LT(as_), BinOp.Sge => bs.LTE(as_),
			_ => throw new NotSupportedException()
		};
		return B.Ternary(c, B.LiteralValue(1UL), B.LiteralValue(0UL));
	}

	static int W(IlType t) => (t as IlType.I)?.Bits ?? 64;
	static ulong MaskC(ulong v, int w) => w >= 64 ? v : v & ((1UL << w) - 1);
	IRuntimeValue<ulong> Mask(IRuntimeValue<ulong> v, int w) =>
		w >= 64 ? v : v.And(B.LiteralValue((1UL << w) - 1));
	IRuntimeValue<long> Sx(IRuntimeValue<ulong> v, int w) =>
		w >= 64 ? v.Cast<long>()
			: v.LeftShift(B.LiteralValue((ulong) (64 - w))).Cast<long>()
				.RightShift(B.LiteralValue((long) (64 - w)));
	IRuntimeValue<ulong> Ror(IRuntimeValue<ulong> v, IRuntimeValue<ulong> n, int w) {
		var wc = B.LiteralValue((ulong) w);
		var nn = n.Mod(wc);
		return Mask(v.RightShift(nn).Or(v.LeftShift(wc.Sub(nn))), w);
	}

	// --- host hooks (override in a subclass to wire the JIT/env) ---
	protected virtual void Branch(BranchKind kind, IRuntimeValue<ulong> target) =>
		throw new NotImplementedException();
	protected virtual void Intrinsic(string name, IRuntimeValue<ulong>[] args) =>
		throw new NotImplementedException();
	/// Default load/store: builder.Pointer<T> — host overrides for MMU/MMIO.
	protected virtual IRuntimeValue<ulong> Load(IRuntimeValue<ulong> addr, int w) => w switch {
		8 => B.Pointer<byte>(addr).Value.Cast<ulong>(),
		16 => B.Pointer<ushort>(addr).Value.Cast<ulong>(),
		32 => B.Pointer<uint>(addr).Value.Cast<ulong>(),
		_ => B.Pointer<ulong>(addr).Value,
	};
	protected virtual void Store(IRuntimeValue<ulong> addr, IRuntimeValue<ulong> v, int w) {
		switch(w) {
			case 8: B.Pointer<byte>(addr).Value = v.Cast<byte>(); break;
			case 16: B.Pointer<ushort>(addr).Value = v.Cast<ushort>(); break;
			case 32: B.Pointer<uint>(addr).Value = v.Cast<uint>(); break;
			default: B.Pointer<ulong>(addr).Value = v; break;
		}
	}
}
