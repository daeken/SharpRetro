using CoreArchCompiler;
using LiftIl;

namespace XFusionGenerator;

/// XF-4: lowers .isa eval bodies to the shared LiftIl tree (M1-GOLDEN.md is the
/// acceptance form, rendered via LiftIl's own printer). History: v1 built an
/// internal mirror tree + text renderer while the IL lived in the consumer's
/// repo; repo-shape (b) landed LiftIl/ in SharpRetro, so this emits real nodes.
///
/// Width model: constants adopt sibling width EXCEPT shifts (result = left
/// width — the PF 0x6996 catch). Semantically-widthed constants use explicit
/// .isa width forms if the heuristic ever diverges (M1-GOLDEN doctrine).
/// Flag-write canonicalization: comparison → u1 direct; bare 0/1 const → u1;
/// top-level (& _ 1) / (>> _ w-1) → trunc-to-u1; else ne-#0.
///
/// x86 conventions (settled ·44/·53/·62 + LiftIl RegKind doc):
///   GPR file = RegKind.X86 (64-bit); 32-bit writes zext, 8/16 masked-insert.
///   Flags = RegKind.Eflags bit-indexed. CMOVcc → IlIfV. push/pop → explicit
///   RSP arithmetic. Intrinsics → IlIntrin(V0, name, args).
public abstract record OperandBind {
	/// High8: legacy AH/CH/DH/BH (8-bit reg 4-7 without REX) = bits 8-15 of
	/// GPR Idx (already remapped to 0-3 by the binder). The x86 wart, encoded once.
	public sealed record Reg(int Idx, int Width, bool High8 = false) : OperandBind;   // RegKind.X86 index
	public sealed record Mem(Il AddrExpr, int Width) : OperandBind;
	public sealed record Imm(long Value, int Width) : OperandBind;
}

public class IlLower {
	readonly List<Il> Stmts = [];
	readonly Dictionary<string, Il> Env = [];       // mlet/let names -> IlTmp
	readonly Dictionary<string, IlTmp> MemAddr = [];// operand name -> bound addr tmp
	int TmpN;
	readonly int OpWidth;
	IReadOnlyDictionary<string, OperandBind> Binds;

	IlLower(int opWidth) => OpWidth = opWidth;

	public static IlBlock Lower(IReadOnlyList<string> params_, IEnumerable<PTree> evalForms,
		IReadOnlyDictionary<string, OperandBind> binds, int opWidth) {
		var l = new IlLower(opWidth) { Binds = binds };
		// Pre-bind memory operand addresses (x86: address evaluated ONCE per insn).
		foreach(var (name, b) in binds)
			if(b is OperandBind.Mem mem) {
				var t = new IlTmp(IlType.U64, l.TmpN);
				l.Stmts.Add(new IlLet(l.TmpN++, mem.AddrExpr));
				l.MemAddr[name] = t;
			}
		foreach(var form in evalForms)
			l.Stmt(form);
		return new IlBlock(l.Stmts);
	}

	static IlType U(int w) => w switch {
		1 => IlType.U1, 8 => IlType.U8, 32 => IlType.U32, 64 => IlType.U64,
		_ => new IlType.I(false, w)
	};
	static int W(Il e) => e.Ty is IlType.I(_, var b) ? b : 64;
	static IlConst C(int w, long v) => new(U(w), (UInt128) (ulong) (v & MaskW(w)));
	static long MaskW(int w) => w >= 64 ? -1L : (1L << w) - 1;

	IlTmp Let(Il e) {
		var t = new IlTmp(e.Ty, TmpN);
		Stmts.Add(new IlLet(TmpN++, e));
		return t;
	}

	// ---- statements ----
	void Stmt(PTree t) {
		if(t is not PList l || l.Count == 0) throw new NotSupportedException($"stmt {t}");
		switch(l[0]) {
			case PName("mlet"): {
				var pairs = (PList) l[1];
				for(var i = 0; i + 1 < pairs.Count; i += 2)
					Env[((PName) pairs[i]).Name] = Let(Expr(pairs[i + 1]));
				foreach(var body in l.Skip(2)) Stmt(body);
				break;
			}
			case PName("let"): {
				Env[((PName) l[1]).Name] = Let(Expr(l[2]));
				foreach(var body in l.Skip(3)) Stmt(body);
				break;
			}
			case PName("="): {
				var target = ((PName) l[1]).Name;
				var e = Expr(l[2]);
				if(IsFlag(target)) { Stmts.Add(new IlWriteReg(RegKind.Eflags, FlagBit(target), CanonFlag(e))); break; }
				if(Binds.TryGetValue(target, out var b)) { WriteOperand(target, b, e); break; }
				if(ArchReg(target) is { } ar) {
					Stmts.Add(new IlWriteReg(RegKind.X86, ar, W(e) == 64 ? e : new IlCast(IlType.U64, CastKind.Zext, e)));
					break;
				}
				throw new NotSupportedException($"write target {target}");
			}
			case PName("block"):
				foreach(var f in l.Skip(1)) Stmt(f);
				break;
			case PName("if"): {
				var cond = CanonFlag(Expr(l[1]));
				// CMOVcc data form: (if C (= dst src)) with dst a reg bind → IlIfV (csel)
				if(l.Count == 3 && l[2] is PList { Count: 3 } asn && asn[0] is PName("=")
					&& asn[1] is PName(var dn) && Binds.TryGetValue(dn, out var db) && db is OperandBind.Reg) {
					var val = Expr(asn[2]);
					WriteOperand(dn, db, new IlIfV(val.Ty, cond, val, ReadOperand(dn, db)));
					break;
				}
				// general form: guarded stmt block (SHL flag-writes) → IlIf(then, else:[])
				var inner = new IlLower(OpWidth) { Binds = Binds, TmpN = TmpN };
				foreach(var (k, v) in Env) inner.Env[k] = v;
				foreach(var (k, v) in MemAddr) inner.MemAddr[k] = v;
				foreach(var f in l.Skip(2)) inner.Stmt(f);
				TmpN = inner.TmpN;
				Stmts.Add(new IlIf(cond, inner.Stmts, []));
				break;
			}
			case PName("push"): {
				// ·62: explicit RSP arithmetic. Value BEFORE the adjust (push rsp = OLD rsp).
				var v = Expr(l[1]);
				var vt = Let(v);
				Stmts.Add(new IlWriteReg(RegKind.X86, 4,
					new IlBin(IlType.U64, BinOp.Sub, Rsp(), C(64, W(v) / 8))));
				Stmts.Add(new IlStore(Rsp(), vt));
				break;
			}
			case PName("branch"):
				Stmts.Add(new IlBranch(BranchKind.Jmp, Expr(l[1], 64)));
				break;
			case PName("call"):
				// call-site marker for the arch-neutral scanner (IlBranch(Call, abs)) —
				// the return-address push is a separate stmt in the .isa body.
				Stmts.Add(new IlBranch(BranchKind.Call, Expr(l[1], 64)));
				break;
			case PName("ret"):
				Stmts.Add(new IlBranch(BranchKind.Ret, Expr(l[1], 64)));
				break;
			case PName("branch-if"): {
				// Jcc: IlBranch(CondJmp, target, cond) — Cond is a field on the node
				// (LiftIl:159; consumer Cfg.cs:71 reads it directly). No IlIf wrapper.
				var cond = CanonFlag(Expr(l[1]));
				Stmts.Add(new IlBranch(BranchKind.CondJmp, Expr(l[2], 64), cond));
				break;
			}
			case PName("intrinsic"): {
				// ·62: IlIntrin(V0, well-known-name, positional dataflow args).
				var name = ((PName) l[1]).Name;
				var args = new List<Il>();
				foreach(var a in l.Skip(2))
					args.Add(a is PInt(var iv) ? C(OpWidth, iv) : Expr(a));
				Stmts.Add(new IlIntrin(IlType.V0, name, args.ToArray()));
				break;
			}
			default:
				throw new NotSupportedException($"stmt head {l[0]}");
		}
	}

	static Il Rsp() => new IlReadReg(IlType.U64, RegKind.X86, 4);

	void WriteOperand(string name, OperandBind b, Il e) {
		switch(b) {
			case OperandBind.Reg(var reg, _, true):  // AH/CH/DH/BH: insert at bits 8-15
				Stmts.Add(new IlWriteReg(RegKind.X86, reg, new IlBin(IlType.U64, BinOp.Or,
					new IlBin(IlType.U64, BinOp.And,
						new IlReadReg(IlType.U64, RegKind.X86, reg), C(64, ~0xFF00L)),
					new IlBin(IlType.U64, BinOp.Shl,
						new IlCast(IlType.U64, CastKind.Zext, e), C(64, 8)))));
				break;
			case OperandBind.Reg(var reg, 64, _):
				Stmts.Add(new IlWriteReg(RegKind.X86, reg, e));
				break;
			case OperandBind.Reg(var reg, 32, _):
				// x86-64 rule: 32-bit write ZERO-EXTENDS to 64 (not insert)
				Stmts.Add(new IlWriteReg(RegKind.X86, reg, new IlCast(IlType.U64, CastKind.Zext, e)));
				break;
			case OperandBind.Reg(var reg, var w, _):  // 8/16 low: masked insert
				Stmts.Add(new IlWriteReg(RegKind.X86, reg, new IlBin(IlType.U64, BinOp.Or,
					new IlBin(IlType.U64, BinOp.And,
						new IlReadReg(IlType.U64, RegKind.X86, reg), C(64, ~((1L << w) - 1))),
					new IlCast(IlType.U64, CastKind.Zext, e))));
				break;
			case OperandBind.Mem:
				Stmts.Add(new IlStore(MemAddr[name], e));
				break;
			default:
				throw new NotSupportedException($"write to {b}");
		}
	}

	// ---- expressions ----
	Il Expr(PTree t) => Expr(t, OpWidth);

	Il Expr(PTree t, int ctxW) {
		switch(t) {
			case PInt(var v): return C(ctxW, v);
			case PName(var n): {
				if(Env.TryGetValue(n, out var bound) && bound != null) return bound;
				if(Binds != null && Binds.TryGetValue(n, out var b)) return ReadOperand(n, b);
				if(IsFlag(n)) return new IlReadReg(IlType.U1, RegKind.Eflags, FlagBit(n));
				if(ArchReg(n) is { } ar) return new IlReadReg(IlType.U64, RegKind.X86, ar);
				throw new NotSupportedException($"name {n}");
			}
			case PList l when l.Count >= 1: return ListExpr(l, ctxW);
			default: throw new NotSupportedException($"expr {t}");
		}
	}

	Il ReadOperand(string name, OperandBind b) => b switch {
		OperandBind.Reg(var reg, _, true) => new IlCast(IlType.U8, CastKind.Trunc,
			new IlBin(IlType.U64, BinOp.Shr, new IlReadReg(IlType.U64, RegKind.X86, reg), C(64, 8))),
		OperandBind.Reg(var reg, 64, _) => new IlReadReg(IlType.U64, RegKind.X86, reg),
		OperandBind.Reg(var reg, var w, _) => new IlCast(U(w), CastKind.Trunc, new IlReadReg(IlType.U64, RegKind.X86, reg)),
		OperandBind.Mem(_, var w) => new IlLoad(U(w), MemAddr[name]),
		OperandBind.Imm(var v, var w) => C(w, v),
		_ => throw new NotSupportedException(b.ToString())
	};

	Il ListExpr(PList l, int ctxW) {
		var head = l[0] is PName(var h) ? h : throw new NotSupportedException(l[0].ToString());
		switch(h) {
			case "u8": return new IlCast(IlType.U8, CastKind.Trunc, Expr(l[1]));
			case "u16": return new IlCast(U(16), CastKind.Trunc, Expr(l[1]));
			case "u32": return new IlCast(IlType.U32, CastKind.Trunc, Expr(l[1]));
			case "u64": return new IlCast(IlType.U64, CastKind.Zext, Expr(l[1]));
			case "bitwidth": return C(ctxW, W(Expr(l[1])));
			case "pop": {
				// ·62: load [RSP], bump RSP, yield the tmp.
				var vt = Let(new IlLoad(U(OpWidth), Rsp()));
				Stmts.Add(new IlWriteReg(RegKind.X86, 4,
					new IlBin(IlType.U64, BinOp.Add, Rsp(), C(64, OpWidth / 8))));
				return vt;
			}
			case "next-pc":
				// resolved at lift time when the lifter provides it (pc+len as a bind);
				// falls back to the pc node for hand-bind tests.
				return Binds != null && Binds.TryGetValue("%nextpc", out var np) && np is OperandBind.Imm(var nv, _)
					? new IlConst(IlType.U64, (ulong) nv) : new IlReadPc(IlType.U64);
			case "addr-of": {
				var opName = ((PName) l[1]).Name;
				return Binds[opName] is OperandBind.Mem ? MemAddr[opName]
					: throw new NotSupportedException("addr-of non-mem operand");
			}
			case "sext": {
				var a = Expr(l[1]);
				var w = l.Count > 2 && l[2] is PInt(var wv) ? (int) wv : OpWidth;
				return new IlCast(U(w), CastKind.Sext, a);
			}
			case "zext": {
				var a = Expr(l[1]);
				var w = l.Count > 2 && l[2] is PInt(var wv2) ? (int) wv2 : OpWidth;
				return new IlCast(U(w), CastKind.Zext, a);
			}
			case "~": {
				var a = Expr(l[1], ctxW);
				return new IlUn(a.Ty, UnOp.Not, a);
			}
		}
		// comparisons → u1
		if(h is "<" or "==" or "!=" or ">") {
			var a = Expr(l[1], ctxW);
			var b = Expr(l[2], a is IlConst ? ctxW : W(a));
			if(a is IlConst ca && W(b) != W(a)) a = new IlConst(U(W(b)), ca.Bits);
			var op = h switch { "<" => BinOp.Ult, "==" => BinOp.Eq, "!=" => BinOp.Ne, ">" => BinOp.Ugt, _ => default };
			return new IlBin(IlType.U1, op, a, b);
		}
		var op2 = h switch {
			"+" => BinOp.Add, "-" => BinOp.Sub, "*" => BinOp.Mul,
			"&" => BinOp.And, "|" => BinOp.Or, "^" => BinOp.Xor,
			">>" => BinOp.Shr, "<<" => BinOp.Shl, ">>a" => BinOp.Sar, "rotr" => BinOp.Ror,
			"!" => BinOp.Eq,  // (! x) → (== x 0)
			"rotl" => (BinOp) (-1),
			_ => throw new NotSupportedException($"op {h}")
		};
		if(h == "!") { var x = Expr(l[1]); return new IlBin(IlType.U1, BinOp.Eq, x, C(W(x), 0)); }
		if(h == "rotl") {  // no Rol in BinOp: rotl w x n = ror x (w-n)
			var x = Expr(l[1], ctxW);
			var n = Expr(l[2], W(x));
			return new IlBin(x.Ty, BinOp.Ror, x, new IlBin(U(W(x)), BinOp.Sub, C(W(x), W(x)), n));
		}
		var isShift = op2 is BinOp.Shr or BinOp.Shl or BinOp.Sar or BinOp.Ror;
		var acc = Expr(l[1], ctxW);
		for(var i = 2; i < l.Count; i++) {
			var rhs = Expr(l[i], W(acc));
			int w;
			if(isShift) w = W(acc);  // shifts: result = LEFT width (the PF 0x6996 rule)
			else {
				w = Math.Max(W(acc), W(rhs));
				if(acc is IlConst c1 && rhs is not IlConst) { acc = new IlConst(U(W(rhs)), c1.Bits); w = W(rhs); }
				if(rhs is IlConst c2 && acc is not IlConst) { rhs = new IlConst(U(W(acc)), c2.Bits); w = W(acc); }
			}
			acc = Fold(new IlBin(U(w), op2, acc, rhs));
		}
		return acc;
	}

	/// Constant-fold pure-constant binops ((<< 1 31) → #80000000; (- 32 1) → #1f).
	static Il Fold(IlBin b) {
		if(b.L is IlConst(_, var x) && b.R is IlConst(_, var y)) {
			var (xv, yv) = ((long) (ulong) x, (long) (ulong) y);
			long? v = b.Op switch {
				BinOp.Add => xv + yv, BinOp.Sub => xv - yv, BinOp.Mul => xv * yv,
				BinOp.And => xv & yv, BinOp.Or => xv | yv, BinOp.Xor => xv ^ yv,
				BinOp.Shl => xv << (int) yv, BinOp.Shr => (long) ((ulong) xv >> (int) yv),
				_ => null
			};
			if(v is { } vv) return C(W(b), vv);
		}
		return b;
	}

	/// Flag-write canonicalization (class doc).
	static Il CanonFlag(Il e) {
		if(e.Ty is IlType.I(_, 1)) return e;
		if(e is IlConst(_, var cv) && (ulong) cv is 0 or 1) return new IlConst(IlType.U1, cv);
		switch(e) {
			case IlBin(_, BinOp.And, _, IlConst(_, var m)) when (ulong) m == 1:
			case IlBin(_, BinOp.Shr, var a, IlConst(_, var sh)) when (long) (ulong) sh == W(a) - 1:
				return new IlCast(IlType.U1, CastKind.Trunc, e);
			default:
				return new IlBin(IlType.U1, BinOp.Ne, e, C(W(e), 0));
		}
	}

	static bool IsFlag(string n) => n is "CF" or "PF" or "AF" or "ZF" or "SF" or "OF" or "DF";
	static int FlagBit(string n) => n switch {
		"CF" => 0, "PF" => 2, "AF" => 4, "ZF" => 6, "SF" => 7, "DF" => 10, "OF" => 11, _ => -1
	};

	/// Implicit arch-reg names in eval bodies (LEAVE: (= SP BP)) → X86 file index.
	static int? ArchReg(string n) => n switch {
		"AX" => 0, "CX" => 1, "DX" => 2, "BX" => 3, "SP" => 4, "BP" => 5, "SI" => 6, "DI" => 7,
		_ => null
	};
}
