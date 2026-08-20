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
	/// File: WHICH register file Idx indexes. Defaulted to X86 because that was the
	/// only file the bind vocabulary had, and the omission was a live bug: every
	/// xmm/mmx/mask/x87 operand bound to RegKind.X86, so `movdqa xmm0, xmm1` wrote
	/// RAX. Harmless while those templates were intrinsic-bodied (X86Lifter's own
	/// comment called the binds "dataflow placeholders"); the moment a template
	/// lowered to real IL the alias became a silent GPR clobber. Verified at
	/// execution, not inferred: RAX=0xAAAA…, RCX=0xBBBB…, movdqa xmm0,xmm1 → RAX
	/// became 0xBBBB….
	public sealed record Reg(int Idx, int Width, bool High8 = false,
		RegKind File = RegKind.X86) : OperandBind;
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

	// Resolve an 8-predicate compare's `pred` operand to its compile-time value. Like
	// vshuf's selector this is an Ib imm bind, so it is known here and the compare
	// lowers to a fixed shape rather than a runtime dispatch. imm8[2:0] per sse.isa:140.
	int PredOf(string name) {
		if(!Binds.TryGetValue(name, out var b) || b is not OperandBind.Imm i)
			throw new NotSupportedException($"cmp pred {name} not an imm bind");
		return (int) ((ulong) i.Value & 7);
	}

	// The 8-predicate float compare table, shared by the scalar (fcmpp) and packed
	// (vfcmpp) forms because the declaration says they ARE the same table
	// (sse.isa:135 "Same table as CMPSS"). Returns a U1 when packed==false, or a
	// per-lane all-1s/0 V128 mask when packed==true.
	//
	// Preds 4-6 are Not(0-2) rather than their own comparisons, and that is the
	// SEMANTICS not a shortcut: an ordered compare against a NaN operand is false, so
	// its negation is true, which is what NEQ/NLT/NLE mean on x86 (interp.rs:594-596
	// spells this out, and the SDM defines them that way). Composing them as
	// Not(ordered) is therefore exact; writing Ne/Sge/Sgt instead would be WRONG on
	// NaN, because those would need to be unordered-true and a bare Sge isn't.
	Il FloatPred(Il a, Il b, int pred, bool packed, int ew) {
		var vt = new IlType.F(ew);
		Il Cmp(BinOp op) => packed
			? new IlVecBin(128, vt, op, a, b)
			: new IlBin(IlType.U1, op, a, b);
		// isnan(x) = Ne(x, x) -- the identity the "fisnan" arm uses, for the same
		// reason: NaN != NaN IS the definition, so no UnOp is needed and none exists.
		Il IsNan(Il x) => packed
			? new IlVecBin(128, vt, BinOp.Ne, x, x)
			: new IlBin(IlType.U1, BinOp.Ne, x, x);
		Il Invert(Il m) => packed
			? new IlVecUn(128, vt, UnOp.Not, m)
			: new IlUn(IlType.U1, UnOp.Not, m);
		Il Unord() => packed
			? new IlVecBin(128, vt, BinOp.Or, IsNan(a), IsNan(b))
			: new IlBin(IlType.U1, BinOp.Or, IsNan(a), IsNan(b));
		return pred switch {
			0 => Cmp(BinOp.Eq),            // EQ    (ordered)
			1 => Cmp(BinOp.Slt),           // LT    (ordered; float operands => ordered-lt,
			2 => Cmp(BinOp.Sle),           // LE     per the "flt" arm's own convention)
			3 => Unord(),                  // UNORD
			4 => Invert(Cmp(BinOp.Eq)),    // NEQ   = !EQ  => true on NaN
			5 => Invert(Cmp(BinOp.Slt)),   // NLT   = !LT  => true on NaN
			6 => Invert(Cmp(BinOp.Sle)),   // NLE   = !LE  => true on NaN
			7 => Invert(Unord()),          // ORD
			_ => throw new NotSupportedException($"cmp pred {pred}")
		};
	}

	// Extract lane `idx` of `v` at element type `et`, for the permutation family. The
	// index is always a COMPILE-TIME constant here (vzip's hi is a literal, vshuf/vshufw's
	// selector is an Imm bind), so this is an IlVecElem over an IlConst rather than
	// anything needing runtime lane addressing -- which is why the permutation cluster
	// needs no node kind that LiftIl doesn't already have. IlVecElem is Il.cs:145,
	// documented there as "extract scalar lane"; MaxwellLift:1155 is the working exemplar
	// (it passes a constant index the same way).
	static Il Lane(Il v, IlType et, int idx) =>
		new IlVecElem(et, v, new IlConst(new IlType.I(false, 32), (UInt128) idx));
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
			case PName("fence"):
				// Memory-ordering barrier (MFENCE/SFENCE/LFENCE). C#-side
				// consumers are single-threaded (X86Machine oracle, isa_diff's
				// C# arm) — a well-known intrinsic marker suffices; the Rust
				// side (RustLiftGen) lowers to bd.fence() → dmb ish/SeqCst.
				Stmts.Add(new IlIntrin(IlType.V0, "fence", Array.Empty<Il>()));
				break;
			// ---- the six heads below were NEVER handled by this lowerer (verified
			// via `git log -S` over its whole history, 2026-08-20). The .isa
			// carried them from 07-09/08-09; RustLiftGen lowered them 08-09/08-10;
			// this arm never caught up, which is 21 red XFusionTests rather than a
			// regression. THREE of them (mul/div-wide, str-op) are executed BY NAME
			// in X86Machine — for those the correct lowering is the `fence` shape:
			// emit the intrinsic marker with the args the machine already reads.
			// The other three need real IL and are transcribed from RustLiftGen's
			// own arms (NOT composed — a rewrite's composed-from-memory
			// semantics is exactly what the freeze-oracle exists to catch).
			case PName("mul-wide"):
			case PName("div-wide"): {
				// (mul-wide src #t|#f) / (div-wide src #t|#f) — F6/F7 /4-/7.
				// X86Machine:169 dispatches on {mul,imul,div,idiv}-wide with
				// args = [width, src] (its own comment at :166). #t = signed,
				// which selects the imul-/idiv- name the machine expects.
				var signed = l[2] is PName("#t");
				var stem = ((PName) l[0]).Name;                       // mul-wide | div-wide
				var nm = signed ? "i" + stem : stem;                  // imul-wide | idiv-wide
				Stmts.Add(new IlIntrin(IlType.V0, nm, [C(64, OpWidth), Expr(l[1])]));
				break;
			}
			case PName("str-op"): {
				// (str-op movs|stos|lods|scas|cmps) — ONE iteration. X86Machine:275
				// takes args[0] = width (".isa convention", its own comment) and
				// X86Lifter:44-47 rewrites a bare marker to rep_/repe_/repne_ from
				// the DecodedInsn prefix — which this lowerer cannot see (it gets
				// only params/eval/binds/opWidth), so emitting the BARE name is
				// both correct and the only reachable form. The rep-wrap is the
				// lifter's job and was already written for a marker nobody emitted.
				Stmts.Add(new IlIntrin(IlType.V0, ((PName) l[1]).Name, [C(64, OpWidth)]));
				break;
			}
			case PName("cdq-cwde"): {
				// (cdq-cwde 0) = CBW/CWDE/CDQE: RAX@op_w = sext(RAX@op_w/2).
				// (cdq-cwde 1) = CWD/CDQ/CQO:  RDX@op_w = RAX >>arith (op_w-1).
				// Transcribed from RustLiftGen:192-215.
				var which = ((PInt) l[1]).Value;
				if(which == 0) {
					var half = OpWidth / 2;
					var al = new IlReadReg(U(half), RegKind.X86, 0);
					var sx = new IlCast(U(OpWidth), CastKind.Sext, al);
					Stmts.Add(new IlWriteReg(RegKind.X86, 0,
						OpWidth == 64 ? sx : new IlCast(IlType.U64, CastKind.Zext, sx)));
				} else {
					var ra = new IlReadReg(U(OpWidth), RegKind.X86, 0);
					var sn = new IlCast(new IlType.I(true, OpWidth), CastKind.Bitcast, ra);
					var fd = new IlBin(new IlType.I(true, OpWidth), BinOp.Sar, sn, C(OpWidth, OpWidth - 1));
					var fu = new IlCast(U(OpWidth), CastKind.Bitcast, fd);
					Stmts.Add(new IlWriteReg(RegKind.X86, 2,
						OpWidth == 64 ? fu : new IlCast(IlType.U64, CastKind.Zext, fu)));
				}
				break;
			}
			case PName("imul-of"): {
				// (imul-of dst a b) — 2/3-op IMUL. dst = trunc(a*b, op_w);
				// CF=OF = signed overflow = hi_half != asr(lo_half, op_w-1).
				// Transcribed from RustLiftGen:305-330 (the .isa-tier fix that
				// silicon verified at 0 IMUL diffs on 200K).
				var w2 = new IlType.I(true, OpWidth * 2);
				var wi = new IlType.I(true, OpWidth);
				var pa = new IlCast(w2, CastKind.Sext, Expr(l[2]));
				var pb = new IlCast(w2, CastKind.Sext, Expr(l[3]));
				var prod = Let(new IlBin(w2, BinOp.Mul, pa, pb));
				var lo = Let(new IlCast(U(OpWidth), CastKind.Trunc, prod));
				var hi = Let(new IlCast(U(OpWidth), CastKind.Trunc,
					new IlBin(w2, BinOp.Shr, prod, C(OpWidth * 2, OpWidth))));
				var loS = new IlCast(wi, CastKind.Bitcast, lo);
				var fill = new IlCast(U(OpWidth), CastKind.Bitcast,
					new IlBin(wi, BinOp.Sar, loS, C(OpWidth, OpWidth - 1)));
				var ovf = Let(new IlBin(IlType.U1, BinOp.Ne, hi, fill));
				var dst = ((PName) l[1]).Name;
				if(Binds.TryGetValue(dst, out var db)) WriteOperand(dst, db, lo);
				else if(ArchReg(dst) is { } dr)
					Stmts.Add(new IlWriteReg(RegKind.X86, dr, new IlCast(IlType.U64, CastKind.Zext, lo)));
				else throw new NotSupportedException($"imul-of dst {dst}");
				Stmts.Add(new IlWriteReg(RegKind.Eflags, FlagBit("CF"), ovf));
				Stmts.Add(new IlWriteReg(RegKind.Eflags, FlagBit("OF"), ovf));
				break;
			}
			case PName("vshift-bytes"): {
				// (vshift-bytes dst count l|r) — PSRLDQ/PSLLDQ whole-128 byte-shift
				// by a COMPILE-TIME imm8; count>=16 → 0 per SDM. count is an Imm
				// bind, so this resolves at lower-time exactly as RustLiftGen:171-190
				// resolves it at emit-time (a Rust-side match, not a runtime ternary).
				var tgt = ((PName) l[1]).Name;
				var cntName = ((PName) l[2]).Name;
				var right = ((PName) l[3]).Name == "r";
				if(!Binds.TryGetValue(cntName, out var cb) || cb is not OperandBind.Imm ci)
					throw new NotSupportedException($"vshift-bytes count {cntName} not an imm bind");
				var cnt = (int) (ulong) ci.Value & 0xFF;
				var v128 = new IlType.I(false, 128);
				Il rv;
				if(cnt >= 16) rv = new IlConst(new IlType.Vec(128), 0);
				else if(cnt == 0) rv = Expr(l[1]);
				else {
					var vd = new IlCast(v128, CastKind.Bitcast, Expr(l[1]));
					var sh = new IlBin(v128, right ? BinOp.Shr : BinOp.Shl, vd, new IlConst(IlType.U64, (UInt128) (cnt * 8)));
					rv = new IlCast(new IlType.Vec(128), CastKind.Bitcast, sh);
				}
				if(Binds.TryGetValue(tgt, out var tb)) WriteOperand(tgt, tb, rv);
				else throw new NotSupportedException($"vshift-bytes dst {tgt}");
				break;
			}
			default:
				throw new NotSupportedException($"stmt head {l[0]}");
		}
	}

	static Il Rsp() => new IlReadReg(IlType.U64, RegKind.X86, 4);

	void WriteOperand(string name, OperandBind b, Il e) {
		switch(b) {
			case OperandBind.Reg(var reg, _, true, _):  // AH/CH/DH/BH: insert at bits 8-15
				Stmts.Add(new IlWriteReg(RegKind.X86, reg, new IlBin(IlType.U64, BinOp.Or,
					new IlBin(IlType.U64, BinOp.And,
						new IlReadReg(IlType.U64, RegKind.X86, reg), C(64, ~0xFF00L)),
					new IlBin(IlType.U64, BinOp.Shl,
						new IlCast(IlType.U64, CastKind.Zext, e), C(64, 8)))));
				break;
			// NON-GPR files (Xmm/St/mask/seg): write the value WHOLE, no partial-write
			// wart. x86's zext-32 / masked-insert rules are GPR semantics; an xmm lane
			// write does not zero-extend into a GPR and must not touch one.
			case OperandBind.Reg(var reg, _, _, var f) when f != RegKind.X86:
				Stmts.Add(new IlWriteReg(f, reg, e));
				break;
			case OperandBind.Reg(var reg, 64, _, _):
				Stmts.Add(new IlWriteReg(RegKind.X86, reg, e));
				break;
			case OperandBind.Reg(var reg, 32, _, _):
				// x86-64 rule: 32-bit write ZERO-EXTENDS to 64 (not insert)
				Stmts.Add(new IlWriteReg(RegKind.X86, reg, new IlCast(IlType.U64, CastKind.Zext, e)));
				break;
			case OperandBind.Reg(var reg, var w, _, _):  // 8/16 low: masked insert
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
			// A literal WIDER than the context truncates, and the .isa has constants that
			// are wider than the op on purpose -- the PF parity table 0x9669 is looked up
			// with a 4-bit index inside a BYTE-width ADD, so C(8, 0x9669) masked it to
			// 0x69 and PF came out wrong on EVERY byte-form insn that uses the table.
			// Found by XFReader: silicon says PF=1 for `add cl,al` @ al=0xff cl=0x88
			// (res=0x87, idx=0xf, (0x9669>>15)&1 == 1) and the lowered IL read
			// `(u8 shr (u8 #69) ...)` -- 0x9669 & 0xFF.
			//
			// Rule transcribed from RustLiftGen.cs:465-467, which fixed the same defect on
			// the Rust arm on 2026-08-12: <256 stays at ctxW so small immediates still
			// participate at operand width (cmp/test/etc), wider goes to a type that holds
			// it. ‡ That site's comment asserted "invisible to interp-vs-C# only if C#
			// shares the truncation (it doesn't -- IlLower uses IlInt at natural width)".
			// It does share it; C() masks. So the sibling was never checked, and a comment
			// claiming a sibling is unaffected is what kept this alive for 8 days.
			case PInt(var v): return (ulong) v < 256 ? C(ctxW, v)
				: (ulong) v <= uint.MaxValue ? C(32, v) : C(64, v);
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
		OperandBind.Reg(var reg, _, true, _) => new IlCast(IlType.U8, CastKind.Trunc,
			new IlBin(IlType.U64, BinOp.Shr, new IlReadReg(IlType.U64, RegKind.X86, reg), C(64, 8))),
		// non-GPR file: read whole, no truncate-to-GPR-width
		OperandBind.Reg(var reg, var xw, _, var xf) when xf != RegKind.X86
			=> new IlReadReg(U(xw), xf, reg),
		OperandBind.Reg(var reg, 64, _, _) => new IlReadReg(IlType.U64, RegKind.X86, reg),
		OperandBind.Reg(var reg, var w, _, _) => new IlCast(U(w), CastKind.Trunc, new IlReadReg(IlType.U64, RegKind.X86, reg)),
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
			// UnOp already carries Clz/Rbit/Popcnt (LiftIl/Il.cs:42) — these three
			// were unreachable only because no case dispatched to them. BSF/BSR/
			// LZCNT/TZCNT/POPCNT are the consumers; RustLiftGen:489/etc lowers the
			// same heads to bd.clz/bd.rbit/bd.popcnt.
			case "clz": {
				var a = Expr(l[1], ctxW);
				return new IlUn(a.Ty, UnOp.Clz, a);
			}
			case "rbit": {
				var a = Expr(l[1], ctxW);
				return new IlUn(a.Ty, UnOp.Rbit, a);
			}
			case "popcnt": {
				var a = Expr(l[1], ctxW);
				return new IlUn(a.Ty, UnOp.Popcnt, a);
			}
			// ---- FLOAT-CONVERSION CLUSTER ----
			// A corpus census over 11.3M decoded insns of real .text put these at
			// ~127K of the 167,924 remaining op-level throws (~76%): as-f32 96,477 ·
			// f32 3,285 · as-f64 3,087 · int-of 2,825 · fmax 2,284 · fmin 2,066 ·
			// f64 738 · '/' 3,779. All SCALAR — no lane semantics — so they
			// transcribe from RustLiftGen's own arms the way the stmt heads did.
			// NB these throw from a DIFFERENT switch than the stmt heads (:421's
			// `op {h}` vs :159's `stmt head {l[0]}`), which is why a census keyed on
			// `case PName(...)` was structurally blind to them.
			case "f32":
			case "f64": {
				// CONVERT (not reinterpret): int→float or float→float. RustLiftGen:491-492.
				var a = Expr(l[1]);
				var w = h == "f32" ? 32 : 64;
				return new IlCast(new IlType.F(w), CastKind.SToF, a);
			}
			case "as-f32":
			case "as-f64": {
				// REINTERPRET the bits as float — no conversion. Used to read Wsd/Wss
				// operands (xmm-lane bits) as float before fcvtzs/fcvt.
				// RustLiftGen:511-512 (bd.bitcast, deliberately NOT bd.cast).
				var a = Expr(l[1]);
				var w = h == "as-f32" ? 32 : 64;
				return new IlCast(new IlType.F(w), CastKind.Bitcast, a);
			}
			case "signed": {
				// (signed W v) — reinterpret as signed int of W bits, no bit change.
				// Used before div/rem for IDIV and before f64 for CVTSI2SD.
				// RustLiftGen:535-540.
				var w = l[1] is PInt(var sw) ? (int) sw : OpWidth;
				return new IlCast(new IlType.I(true, w), CastKind.Bitcast, Expr(l[2]));
			}
			case "fmax":
			case "fmin": {
				// x86-EXACT min/max, which is NOT ARM's FMAX/FMIN: on NaN or ±0 x86
				// returns the SECOND source, where ARM propagates the NaN. BinOp.FMin/
				// FMax carry the x86 semantics (the Rust side lowers via FCMP+FCSEL for
				// the same reason). RustLiftGen:507-508.
				var a = Expr(l[1]);
				var b = Expr(l[2], W(a));
				return new IlBin(a.Ty, h == "fmax" ? BinOp.FMax : BinOp.FMin, a, b);
			}
			// ---- PACKED (V128) CLUSTER, the half that needs NO new node kinds ----
			// This comment said SEVEN heads "need additions to the shared LiftIl":
			// vzip/vshuf/vshufw, vmovmsk, vhadd, fcmpp/vfcmpp, vdpp, vcvt. SIX of them
			// are lowered in this file now, and every one needed NOTHING new. The
			// classification was wrong six times by one organ: I asked whether ONE node
			// could express the operation instead of whether the node SET could, which
			// means I was classifying by the HEAD'S NAME rather than by the constructor
			// that would receive it. "cross-lane add" and "dot-product" sound exotic;
			// both are IlVecElem extracts + scalar IlBin + one IlVecBuild.
			//
			// vcvt is the ONE that genuinely can't: IlCast(Ty, Kind, X) has no
			// element-width field, so CVTDQ2PS (4xi32->4xf32) and CVTPD2PS (2xf64->
			// 4xf32, lane-count CHANGING) are indistinguishable in it. That is a real
			// shared-LiftIl question and stays with the consumer side.
			//
			// The rule this cost six instances to state: CLASSIFY BY THE RECEIVING
			// CONSTRUCTOR, NEVER BY THE HEAD'S NAME. What decides local-vs-shared is
			// whether the ctor can carry the operation's information -- not whether the
			// operation sounds like it wants a node of its own.
			//
			// The float-vs-int discriminator is the TYPE, not the op -- exactly as the
			// scalar side already does it: DIVSS lowers as (/ (as-f32 dst) (as-f32 src))
			// -> BinOp.UDiv with an IlType.F operand (sse.isa:76, and the "/" arm at
			// :532), and MaxwellLift:276 does new IlBin(F32, BinOp.Add, ...). So a
			// packed float op is IlVecBin with ElemTy = IlType.F(ew) and the same
			// arithmetic BinOp as the integer form.
			//
			// Op codes are TRANSCRIBED from the Builder trait's own doc-comments
			// (sharpretro-jit/src/lib.rs) rather than composed -- that file is the
			// authority the Rust backend's arms read, and composing an op table from
			// memory is the exact class the freeze-oracle exists to catch.
			case "vfbin": {
				// (vfbin a b ew op) -- packed-float per-lane arith on V128 -> V128.
				// ew in {32,64}; op in {0=add,1=sub,2=mul,3=div}. RustLiftGen:665-672,
				// Builder::vfbin. div is BinOp.UDiv-with-a-float-type per DIVSS above.
				var a = Expr(l[1]); var b = Expr(l[2]);
				var ew = (int) ((PInt) l[3]).Value;
				var op = (int) ((PInt) l[4]).Value;
				var bop = op switch {
					0 => BinOp.Add, 1 => BinOp.Sub, 2 => BinOp.Mul, 3 => BinOp.UDiv,
					_ => throw new NotSupportedException($"vfbin op {op}")
				};
				return new IlVecBin(128, new IlType.F(ew), bop, a, b);
			}
			case "vmovmsk": {
				// (vmovmsk a ew) -- gather each lane's SIGN BIT into a scalar integer.
				// MOVMSKPS ew=32 (sse.isa:43) · MOVMSKPD ew=64 (sse2.isa:24) ·
				// PMOVMSKB ew=8 (sse2.isa:176). Result is a SCALAR, not a vector.
				//
				// I had this in the "needs a new shared-LiftIl node kind" set and that
				// was WRONG, for the third time by the same organ: I asked whether ONE
				// node could express the operation instead of whether the node SET
				// could. The consumer side read interp.rs:451 and pointed out the
				// decomposition; the ctor is what settles it, as it did for the
				// permutation cluster:
				//   IlVecElem(IlType Ty, Il Vec, Il Idx)   -- Il.cs:145, PER-LANE type
				// so each lane is an ordinary scalar once extracted, and everything
				// after that is scalar arithmetic that already lowers.
				//
				// The .isa's own body is the transcription source (interp.rs:451):
				//   for i in 0..n { r |= ((a.bits >> (i*ew + (ew-1))) & 1) << i; }
				// i.e. per lane: take the top bit, place it at bit i, OR them together.
				// Two ways to spell "the top bit": Shr by (ew-1) then And 1, or the
				// signed-Slt-against-zero form. The FIRST is what the interpreter does,
				// so it is what this emits -- composing the second would be a
				// different-but-equivalent shape that a byte-diff against the Rust
				// arm would flag -- the compose-from-memory class this bench keeps
				// paying for.
				//
				// Widths: the lane is extracted at its own width (ew), shifted within
				// that width, then WIDENED to 32 before placement -- because n can be
				// 16 (PMOVMSKB) and bit 15 does not exist in a u8 lane. The result type
				// is U32 to match interp.rs:451's `IlType::I{signed:false, width:32}`.
				var mv = Expr(l[1]);
				var mew = (int) ((PInt) l[2]).Value;
				var nl = 128 / mew;
				var lt = new IlType.I(false, mew);
				Il acc2 = null;
				for(var i = 0; i < nl; i++) {
					// (lane >> (ew-1)) & 1, at the lane's own width
					var bit = new IlBin(lt, BinOp.And,
						new IlBin(lt, BinOp.Shr, Lane(mv, lt, i), C(mew, mew - 1)),
						C(mew, 1));
					// widen to 32 (Zext is a no-op when mew==32; kept uniform so the
					// shift below is always in a width that can hold bit index nl-1)
					Il w32 = mew == 32 ? bit : new IlCast(U(32), CastKind.Zext, bit);
					var placed = i == 0 ? w32
						: new IlBin(U(32), BinOp.Shl, w32, C(32, i));
					acc2 = acc2 == null ? placed : new IlBin(U(32), BinOp.Or, acc2, placed);
				}
				return acc2;
			}
			case "vibin": {
				// (vibin a b ew op) -- packed-int per-lane wrapping arith on V128.
				// ew in {8,16,32,64}; op in {0=add,1=sub,2=mul,3=cmpeq,4=cmpgt}
				// (Builder::vibin). ops 3/4 produce an all-1s/all-0 per-lane MASK, and
				// BinOp.Eq/Sgt carry no mask-vs-boolean convention on a vector -- an
				// IlVecBin(Eq) is ambiguous between per-lane 1 and per-lane all-1s.
				// That convention is a shared-IL decision, so those DIE LOUD rather
				// than lower to a plausible wrong shape.
				var a = Expr(l[1]); var b = Expr(l[2]);
				var ew = (int) ((PInt) l[3]).Value;
				var op = (int) ((PInt) l[4]).Value;
				// ops 3/4 are the MASK forms (PCMPEQ*/PCMPGT*), and the per-lane result
				// convention is NOT a choice left to a backend -- it is DECLARED on the
				// instruction and implemented to match:
				//   sse2.isa:158  "; PCMPEQ*/PCMPGT*: per-lane integer compare -> all-1s/0
				//                  mask. cmpgt is SIGNED. vibin op=3/4."
				//   interp.rs:484  let m = if ew == 128 { u128::MAX } else { (1u128<<ew)-1 };
				//                  3 => if la == lb { m } else { 0 },
				//                  4 => sign-extend both to i128, then m
				// So a lane is ALL-1s at the element width, not 1 -- which is what
				// PAND-after-PCMPEQ depends on, and picking a boolean 1 here would put
				// this in disagreement with the interpreter that already executes it.
				// cmpgt is SIGNED (BinOp.Sgt); the ElemTy is signed for both, since
				// PCMPEQ's equality is width-exact either way.
				var bop = op switch {
					0 => BinOp.Add, 1 => BinOp.Sub, 2 => BinOp.Mul,
					3 => BinOp.Eq, 4 => BinOp.Sgt,
					_ => throw new NotSupportedException($"op vibin-{op}")
				};
				return new IlVecBin(128, new IlType.I(true, ew), bop, a, b);
			}
			case "vfmax":
			case "vfmin": {
				// (vfmax|vfmin a b ew) -- packed MAXPS/MINPS/MAXPD/MINPD. x86-EXACT
				// semantics, NOT ARM's FMAX: on NaN or +-0 x86 returns the SECOND
				// source. BinOp.FMin/FMax already carry that (the scalar arm at :433
				// relies on the same thing, and Builder::vfminmax lowers via FCMGT+BIT
				// for exactly this reason).
				var a = Expr(l[1]); var b = Expr(l[2]);
				var ew = (int) ((PInt) l[3]).Value;
				return new IlVecBin(128, new IlType.F(ew),
					h == "vfmax" ? BinOp.FMax : BinOp.FMin, a, b);
			}
			case "vfun": {
				// (vfun a ew op) -- packed-float unary. op in {0=sqrt} today
				// (Builder::vfun reserves 1/2 for fabs/fneg). SQRTPS/SQRTPD.
				var a = Expr(l[1]);
				var ew = (int) ((PInt) l[2]).Value;
				var op = (int) ((PInt) l[3]).Value;
				var uop = op switch {
					0 => UnOp.Sqrt,
					_ => throw new NotSupportedException($"op vfun-{op}")
				};
				return new IlVecUn(128, new IlType.F(ew), uop, a);
			}
			case "vishi": {
				// (vishi dst count-op ew dir) -- packed shift by a COMPILE-TIME imm.
				// dir in {0=shl,1=lshr,2=ashr}; count is an Imm bind, resolved here
				// exactly as vshift-bytes does at :253-258 (and as RustLiftGen:651-658
				// resolves it at emit-time rather than as a runtime ternary).
				// x86: count >= ew gives 0 for shl/lshr and all-sign for ashr, and
				// all-sign IS ashr by ew-1 -- so the clamp is the semantics, not a
				// shortcut (Builder::vishi documents the same rule).
				var a = Expr(l[1]);
				var cntName = ((PName) l[2]).Name;
				var ew = (int) ((PInt) l[3]).Value;
				var dir = (int) ((PInt) l[4]).Value;
				if(!Binds.TryGetValue(cntName, out var vb) || vb is not OperandBind.Imm vi)
					throw new NotSupportedException($"vishi count {cntName} not an imm bind");
				var cnt = (int) ((ulong) vi.Value & 0xFF);
				var et = new IlType.I(dir == 2, ew);
				if(cnt >= ew) {
					if(dir != 2) return new IlConst(new IlType.Vec(128), 0);
					cnt = ew - 1;   // ashr: saturating to all-sign
				}
				var sop = dir switch {
					0 => BinOp.Shl, 1 => BinOp.Shr, 2 => BinOp.Sar,
					_ => throw new NotSupportedException($"op vishi-dir-{dir}")
				};
				return new IlVecBin(128, et, sop, a, new IlConst(new IlType.I(false, 32), (UInt128) cnt));
			}
			// ---- 8-PREDICATE COMPARE cluster: no new node kinds either ----
			// Third head-family I had filed as "needs a shared-IL addition", and the
			// declaration settles it the same way vibin's did -- sse.isa:135, on the
			// instruction:
			//     "; CMPPS: per-lane 8-predicate compare -> per-lane all-1s/0 mask.
			//        Same table as CMPSS."
			//     "; pred = imm8[2:0] (0=EQ 1=LT 2=LE 3=UNORD 4=NEQ 5=NLT 6=NLE 7=ORD)."
			// and interp.rs (fn fcmpp :580, fn vfcmpp :391) implements exactly that,
			// including the reason preds 4-6 are NOT of 0-2 rather than their own
			// comparisons: on a NaN operand the ordered forms are false, so their
			// negations are TRUE, which is what the SDM specifies. Transcribed, not
			// composed -- the comment at :581 spells the NaN reasoning out.
			//
			// So each predicate is a composition of things that already exist:
			//   0 EQ    -> Eq            4 NEQ  -> Not(Eq)
			//   1 LT    -> Slt           5 NLT  -> Not(Slt)
			//   2 LE    -> Sle           6 NLE  -> Not(Sle)
			//   3 UNORD -> isnan(a) | isnan(b)     7 ORD -> Not(UNORD)
			// with isnan(x) = Ne(x, x), which is the identity the "fisnan" arm below
			// already uses ("NaN != NaN is the definition, so the IL already expresses
			// it") -- and Slt/Sle on FLOAT operands is ordered-compare, which the "flt"
			// arm below establishes.
			//
			// The MASK is where this could have gone wrong, and it's the vibin question
			// again: an IlVecBin(Eq) is ambiguous between per-lane 1 and per-lane all-1s,
			// and the declaration says all-1s at the element width. For the SCALAR form
			// the mask is (1<<w)-1, which is a 1-bit sext -- the standard all-1s idiom
			// (sign-extending a 1-bit value: bit 0 IS the sign bit, so 1 -> all-ones).
			// Noting that explicitly because nothing else in this file sexts from width
			// 1; the other Sext sites (:219, :384) are 8/16/32-bit.
			case "fcmpp": {
				// (fcmpp a b pred w) -- CMPSS/CMPSD. Scalar: dst[w-1:0] = mask, upper
				// preserved by write_operand's scalar rule. Operands arrive already
				// float-cast at the call site ((as-f32 dst) etc, sse.isa:141).
				var a = Expr(l[1]); var b = Expr(l[2]);
				var w = (int) ((PInt) l[4]).Value;
				var cmp = FloatPred(a, b, PredOf(((PName) l[3]).Name), false, 0);
				return new IlCast(U(w), CastKind.Sext, cmp);   // U1 -> all-1s at w
			}
			case "vfcmpp": {
				// (vfcmpp a b pred ew) -- CMPPS/CMPPD, the same table per lane. The
				// per-lane all-1s convention is the DECLARED one (sse.isa:135), so the
				// compare BinOps are unambiguous here for the same reason they are in
				// vibin's mask ops.
				var a = Expr(l[1]); var b = Expr(l[2]);
				var ew = (int) ((PInt) l[4]).Value;
				return FloatPred(a, b, PredOf(((PName) l[3]).Name), true, ew);
			}
			case "vcvt": {
				// (vcvt a kind) -- the packed CVT family. TRANSCRIBED from interp.rs:291.
				//
				// I deferred this THREE TIMES on "IlCast(Ty, Kind, X) has no
				// element-width field, so CVTDQ2PS (4xi32->4xf32) and CVTPS2PD
				// (2xf64->4xf32, lane-count CHANGING) are indistinguishable in it."
				// That is true of a VECTOR cast and the deferral presumed one. Per-lane
				// the cast is SCALAR: IlCast(F(64), FExt, <an F(32) lane>) carries the
				// widths in its own type and its operand's, and the lane-count change
				// is HOW MANY EXTRACTS THERE ARE -- a property of the IlVecBuild, not a
				// field on the cast. Same organ as the other six: I asked whether ONE
				// node expressed it instead of whether the node SET did.
				//
				// kind, from the interpreter's own match:
				//   0 4xi32->4xf32   1 4xf32->4xi32 trunc   2 2xf32->2xf64
				//   3 2xf64->2xf32   4 2xi32->2xf64         5 2xf64->2xi32 trunc
				//   6 2xf64->2xi32 round-ties-even          7 4xf32->4xi32 round
				// The i32->float directions are SIGNED (`lane as i32 as f32`), so SToF.
				// The float->int directions carry x86 INDEFINITE-INTEGER semantics, so
				// each lane reuses the (int-of) shape built at the scalar site below --
				// the sweep's p2-DENSE measured a THREE-WAY divergence when that was a
				// bare F->I cast, so it is the semantics rather than a nicety.
				var a = Expr(l[1]);
				var kind = (int) ((PInt) l[2]).Value;
				// (srcW, dstW, n, toFloat, round) per kind -- read off the interpreter,
				// not derived: kinds 6 and 7 ROUND ties-even (MXCSR default), 1 and 5
				// TRUNCATE, and that difference is the whole distinction between
				// CVTPS2DQ and CVTTPS2DQ.
				var (sw, dw, n, toF, rnd) = kind switch {
					0 => (32, 32, 4, true,  false),
					1 => (32, 32, 4, false, false),
					2 => (32, 64, 2, true,  false),
					3 => (64, 32, 2, true,  false),
					4 => (32, 64, 2, true,  false),
					5 => (64, 32, 2, false, false),
					6 => (64, 32, 2, false, true),
					7 => (32, 32, 4, false, true),
					_ => throw new NotSupportedException($"op vcvt-kind-{kind}")
				};
				// The SOURCE lane type: kinds 0 and 4 read INTEGER lanes, everything
				// else reads float lanes. Getting this wrong would silently reinterpret
				// bits -- which is exactly the class the exec-oracle caught at (int-of).
				var srcTy = (kind == 0 || kind == 4)
					? (IlType) new IlType.I(true, sw)
					: new IlType.F(sw);
				var dstTy = toF ? (IlType) new IlType.F(dw) : new IlType.I(true, dw);
				var el = new List<Il>();
				for(var i = 0; i < n; i++) {
					var lane = Lane(a, srcTy, i);
					Il conv;
					if(toF)
						// int->float is SToF; float->float is FExt (widen) or FTrunc.
						conv = (kind == 0 || kind == 4)
							? new IlCast(dstTy, CastKind.SToF, lane)
							: new IlCast(dstTy, dw > sw ? CastKind.FExt : CastKind.FTrunc, lane);
					else {
						// float->int, x86-indefinite. `rnd` applies UnOp.Round FIRST
						// (ties-even, which is what Round means here and what MXCSR's
						// default RC selects); the trunc kinds let FToSI truncate.
						var fv = rnd ? new IlUn(srcTy, UnOp.Round, lane) : lane;
						var fty = new IlType.F(sw);
						var limit = new IlCast(fty, CastKind.SToF,
							new IlConst(IlType.U64, (UInt128) 1 << (dw - 1)));
						var mag = new IlUn(fty, UnOp.Abs, fv);
						var inRange = new IlBin(IlType.U1, BinOp.Slt, mag, limit);
						var ok = new IlCast(dstTy, CastKind.FToSI, fv);
						var indef = new IlConst(dstTy, (UInt128) 1 << (dw - 1));
						conv = new IlIfV(dstTy, inRange, ok, indef);
					}
					el.Add(conv);
				}
				// The result's OWN lane count is el.Count and its lane type is dstTy --
				// so a 2xf64 result (128 bits, 2 lanes) and a 4xf32 one (128 bits, 4
				// lanes) are different IlVecBuilds rather than one ambiguous cast.
				return new IlVecBuild(128, dstTy, el);
			}
			case "vhadd": {
				// (vhadd a b ew) -- HADDPS/HADDPD: PAIRWISE add within each source, a's
				// pairs filling the low half of the result and b's the high half.
				//   n = 128/ew;  for i in 0..n/2:
				//     r[i]       = a[2i] + a[2i+1]
				//     r[n/2 + i] = b[2i] + b[2i+1]
				// TRANSCRIBED from interp.rs:374 (fn vhadd), which spells ew=32 out as
				// four explicit pairs -- p(l(a,0)+l(a,1),0) | p(l(a,2)+l(a,3),1) |
				// p(l(b,0)+l(b,1),2) | p(l(b,2)+l(b,3),3) -- and ew=64 as two. The loop
				// above is that, generalized; I checked it reproduces both spellings
				// lane-for-lane rather than trusting the generalization.
				//
				// Lanes are FLOAT here (the interpreter uses f32/f64 from_bits and a
				// float add), so ElemTy is IlType.F(ew) and the adds are IlBin over
				// F(ew) -- the same float-by-TYPE discriminator vfbin uses above.
				var a = Expr(l[1]); var b = Expr(l[2]);
				var ew = (int) ((PInt) l[3]).Value;
				if(ew != 32 && ew != 64) throw new NotSupportedException($"op vhadd-ew-{ew}");
				var ft = new IlType.F(ew);
				var n = 128 / ew;
				var el = new List<Il>();
				for(var i = 0; i < n / 2; i++)
					el.Add(new IlBin(ft, BinOp.Add, Lane(a, ft, 2 * i), Lane(a, ft, 2 * i + 1)));
				for(var i = 0; i < n / 2; i++)
					el.Add(new IlBin(ft, BinOp.Add, Lane(b, ft, 2 * i), Lane(b, ft, 2 * i + 1)));
				return new IlVecBuild(128, ft, el);
			}
			case "vdpp": {
				// (vdpp a b imm ew) -- DPPS/DPPD dot-product. imm's HIGH nibble selects
				// which lanes multiply into the sum; the LOW nibble selects which output
				// lanes receive it, the rest zero:
				//   sum = 0.0;  for i in 0..n: if imm & (1<<(4+i)): sum += a[i]*b[i]
				//   for i in 0..n: r[i] = (imm & (1<<i)) ? sum : 0.0
				// TRANSCRIBED from interp.rs:338 (fn vdpp).
				//
				// The accumulator STARTS at a float zero and each product is Added to
				// it -- not folded pairwise, and not seeded with the first product. That
				// is what the interpreter does and it is observable: 0.0 + (-0.0) is
				// +0.0, so seeding with the first term would differ on a negative-zero
				// product. Faithfulness here is the difference between two shapes a
				// byte-diff against the Rust arm would flag.
				var a = Expr(l[1]); var b = Expr(l[2]);
				var ew = (int) ((PInt) l[4]).Value;
				if(ew != 32 && ew != 64) throw new NotSupportedException($"op vdpp-ew-{ew}");
				var immName = ((PName) l[3]).Name;
				if(!Binds.TryGetValue(immName, out var db) || db is not OperandBind.Imm di)
					throw new NotSupportedException($"vdpp imm {immName} not an imm bind");
				var imm = (uint) ((ulong) di.Value & 0xFF);
				var ft = new IlType.F(ew);
				var n = 128 / ew;
				var zero = new IlConst(ft, (UInt128) 0);     // +0.0 at either width
				Il sum = zero;
				for(var i = 0; i < n; i++)
					if((imm & (1u << (4 + i))) != 0)
						sum = new IlBin(ft, BinOp.Add, sum,
							new IlBin(ft, BinOp.Mul, Lane(a, ft, i), Lane(b, ft, i)));
				var el = new List<Il>();
				for(var i = 0; i < n; i++)
					el.Add((imm & (1u << i)) != 0 ? sum : zero);
				return new IlVecBuild(128, ft, el);
			}
			// ---- LANE-PERMUTATION cluster: also NO new node kinds ----
			// I had these in the "needs a new node kind" half, from the head names. Wrong,
			// and the ctor is what settles it (same correction as vcvt, opposite direction):
			// IlVecBuild(Bits, ElemTy, Elems) and IlVecElem(Ty, Vec, Idx) BOTH already exist
			// in the shared LiftIl -- Il.cs:143/145, documented right there as
			// "IlVecBuild = (vector e0..eN); IlVecElem = extract scalar lane". And every
			// selector in this family is COMPILE-TIME (vzip's hi is a #t/#f literal;
			// vshuf/vshufw's sel is an Imm bind, resolved here exactly as vishi and
			// vshift-bytes resolve theirs). So a permutation is n IlVecElem extracts at
			// CONSTANT indices, collected by one IlVecBuild -- no runtime lane indexing,
			// hence no node kind that doesn't exist.
			//
			// ElemTy is I(false, ew): a permutation is WIDTH-typed, not sign- or
			// float-typed. SHUFPS moves 32-bit lanes and never inspects them, so calling
			// them f32 would assert something the operation doesn't depend on. The
			// declaration comment on IlVecBuild says ElemTy carries lane type "for ops
			// where it isn't recoverable from children" -- for a pure bit-permutation the
			// width is the whole of it.
			//
			// Semantics TRANSCRIBED from interp.rs (fn vzip :536, vshuf :523, vshufw :509),
			// which is the Rust backend's authority, not composed from the mnemonics.
			case "vzip": {
				// (vzip a b ew hi) -- INTERLEAVE the low (or high) halves:
				//   n = 128/ew; base = hi ? n/2 : 0
				//   for k in 0..n/2:  r[2k] = a[base+k];  r[2k+1] = b[base+k]
				// x86's PUNPCKL*/H* and UNPCKLPS/HPS. Note the .isa SWAPS args where the
				// x86 form wants it (MOVHLPS is (vzip src dst 64 #t), sse.isa:20) -- so
				// this arm takes l[1]/l[2] in order and the swap stays declarative.
				var a = Expr(l[1]); var b = Expr(l[2]);
				var ew = (int) ((PInt) l[3]).Value;
				var hi = l[4] is PName("#t");
				var n = 128 / ew;
				var et = new IlType.I(false, ew);
				var bse = hi ? n / 2 : 0;
				var el = new List<Il>();
				for(var k = 0; k < n / 2; k++) {
					el.Add(Lane(a, et, bse + k));
					el.Add(Lane(b, et, bse + k));
				}
				return new IlVecBuild(128, et, el);
			}
			case "vshuf": {
				// (vshuf a b sel ew) -- SHUFPS/SHUFPD/PSHUFD. Low half of the result is
				// selected from a, high half from b (PSHUFD passes src for both, so it
				// degenerates to a single-source shuffle -- sse2.isa:290):
				//   bits_per = ew==32 ? 2 : 1;  for i in 0..n:
				//     src = i < n/2 ? a : b;  j = (sel >> i*bits_per) & mask;  r[i] = src[j]
				var a = Expr(l[1]); var b = Expr(l[2]);
				var selName = ((PName) l[3]).Name;
				var ew = (int) ((PInt) l[4]).Value;
				if(!Binds.TryGetValue(selName, out var sb) || sb is not OperandBind.Imm si)
					throw new NotSupportedException($"vshuf sel {selName} not an imm bind");
				var sel = (uint) ((ulong) si.Value & 0xFF);
				var bitsPer = ew switch {
					32 => 2, 64 => 1,
					_ => throw new NotSupportedException($"op vshuf-ew-{ew}")
				};
				var n = 128 / ew;
				var et = new IlType.I(false, ew);
				var smask = (1u << bitsPer) - 1;
				var el = new List<Il>();
				for(var i = 0; i < n; i++) {
					var src = i < n / 2 ? a : b;
					var j = (int) ((sel >> (i * bitsPer)) & smask);
					el.Add(Lane(src, et, j));
				}
				return new IlVecBuild(128, et, el);
			}
			case "vshufw": {
				// (vshufw src sel hi) -- PSHUFLW/PSHUFHW. Shuffles the FOUR words of one
				// half by sel; the OTHER half is copied through unchanged. So all 8 lanes
				// are named, 4 permuted and 4 identity:
				//   base = hi ? 4 : 0;  for i in 0..4: r[base+i] = src[base + ((sel>>2i)&3)]
				//   and r[other+i] = src[other+i]
				var a = Expr(l[1]);
				var selName = ((PName) l[2]).Name;
				var hi = l[3] is PName("#t");
				if(!Binds.TryGetValue(selName, out var wb) || wb is not OperandBind.Imm wi)
					throw new NotSupportedException($"vshufw sel {selName} not an imm bind");
				var sel = (uint) ((ulong) wi.Value & 0xFF);
				var et = new IlType.I(false, 16);
				var bse = hi ? 4 : 0;
				var el = new List<Il>();
				for(var i = 0; i < 8; i++) {
					if(i >= bse && i < bse + 4) {
						var j = (int) ((sel >> ((i - bse) * 2)) & 3);
						el.Add(Lane(a, et, bse + j));
					} else
						el.Add(Lane(a, et, i));      // the untouched half, copied through
				}
				return new IlVecBuild(128, et, el);
			}
			case "flt": {
				var a = Expr(l[1]); var b = Expr(l[2], W(a));
				return new IlBin(IlType.U1, BinOp.Slt, a, b);   // float ordered-lt
			}
			case "feq": {
				var a = Expr(l[1]); var b = Expr(l[2], W(a));
				return new IlBin(IlType.U1, BinOp.Eq, a, b);
			}
			case "fsqrt": {
				var a = Expr(l[1], ctxW);
				return new IlUn(a.Ty, UnOp.Sqrt, a);
			}
			case "fisnan": {
				// (fisnan x) — IEEE unordered test. No UnOp for it, and there must not
				// be one: NaN != NaN is the definition, so the IL already expresses it
				// with a float Ne against itself. RustLiftGen:497 has a bd.fisnan
				// primitive because the JIT wants one instruction; the C# arm gets the
				// identity instead of a new node kind.
				var a = Expr(l[1], ctxW);
				return new IlBin(IlType.U1, BinOp.Ne, a, a);
			}
			case "int-of": {
				// (int-of W v) — x86 float→signed-int with INDEFINITE-INTEGER semantics
				// on NaN/inf/out-of-range: the SDM says 80000000H is returned, and the
				// silicon sweep's p2-DENSE Ⓗ measured a THREE-WAY divergence when this
				// was a bare F→I cast (cvttsd2si on f64 NaN: silicon 0x80000000, interp
				// 0, tier-0 0; on +inf: silicon 0x80000000, interp 0xFFFFFFFF, tier-0
				// 0x7FFFFFFF saturating). So the guard is the semantics, not a nicety.
				// Transcribed from RustLiftGen:513-534's f_to_si_x86 shape:
				//   in_range = |fv| < 2^(iw-1)  →  ternary(in_range, cast, 1<<(iw-1))
				// NaN makes the lt FALSE, so NaN falls to the indefinite value for free.
				var fv = Expr(l[2]);
				var iw = l[1] is PList bw && bw.Count == 2 && bw[0] is PName("bitwidth")
					? W(Expr(bw[1]))
					: (l[1] is PInt(var iwv) ? (int) iwv : OpWidth);
				// ⚠ W() returns 64 for ANY non-I type (IlLower.cs:58), so W(fv) on an
				// F-typed value silently reads 64 — which built an f64 limit and an
				// f64 abs against an f32 operand. Found by the exec-oracle: NaN gave
				// 0 instead of the indefinite integer, because the f32 NaN's bits
				// reinterpreted as f64 are a small positive normal, so the in-range
				// compare said TRUE. The width must come from the F type itself.
				var fw = fv.Ty is IlType.F ff2 ? ff2.Bits : 64;
				var fty = new IlType.F(fw);
				// 2^(iw-1) as a float constant of the source width
				var limit = new IlCast(fty, CastKind.SToF,
					new IlConst(IlType.U64, (UInt128) 1 << (iw - 1)));
				var mag = new IlUn(fty, UnOp.Abs, fv);
				var inRange = new IlBin(IlType.U1, BinOp.Slt, mag, limit);
				var conv = new IlCast(new IlType.I(true, iw), CastKind.FToSI, fv);
				var indef = new IlConst(new IlType.I(true, iw), (UInt128) 1 << (iw - 1));
				return new IlIfV(new IlType.I(true, iw), inRange, conv, indef);
			}
			case "bswap": {
				// (bswap x) — byte-reverse at op-width. No UnOp.Bswap exists, so
				// this composes from the primitives the IL DOES have, exactly as a
				// generic backend would: rbit reverses BITS, so bswap = rbit of
				// each byte re-reversed. Cheaper and obviously-correct form: OR of
				// per-byte shifts, which is what a C compiler emits for the 16-bit
				// case and what LLVM pattern-matches back to a bswap.
				var a = Expr(l[1], ctxW);
				var w = W(a);
				if(w % 8 != 0 || w < 16) throw new NotSupportedException($"bswap width {w}");
				var ty = U(w);
				Il bsAcc = null;
				for(var i = 0; i < w / 8; i++) {
					// byte i of the source lands at byte (nbytes-1-i) of the result
					var srcSh = i * 8;
					var dstSh = (w / 8 - 1 - i) * 8;
					Il b = srcSh == 0 ? a : new IlBin(ty, BinOp.Shr, a, C(w, srcSh));
					b = new IlBin(ty, BinOp.And, b, C(w, 0xFF));
					if(dstSh != 0) b = new IlBin(ty, BinOp.Shl, b, C(w, dstSh));
					bsAcc = bsAcc == null ? b : new IlBin(ty, BinOp.Or, bsAcc, b);
				}
				return bsAcc;
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
			// / and % were absent: the corpus census put "/" at 3,779 insns. Unsigned
			// forms; IDIV wraps its operands in (signed W ...) first, which retypes
			// them and makes BinOp.UDiv the signed op via the operand type.
			"/" => BinOp.UDiv, "%" => BinOp.URem,
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

	static bool IsFlag(string n) => n is "CF" or "PF" or "AF" or "ZF" or "SF" or "OF" or "DF" or "IDF";
	static int FlagBit(string n) => n switch {
		"CF" => 0, "PF" => 2, "AF" => 4, "ZF" => 6, "SF" => 7, "DF" => 10, "OF" => 11, "IDF" => 21, _ => -1
	};

	/// Implicit arch-reg names in eval bodies (LEAVE: (= SP BP)) → X86 file index.
	static int? ArchReg(string n) => n switch {
		"AX" => 0, "CX" => 1, "DX" => 2, "BX" => 3, "SP" => 4, "BP" => 5, "SI" => 6, "DI" => 7,
		_ => null
	};
}
