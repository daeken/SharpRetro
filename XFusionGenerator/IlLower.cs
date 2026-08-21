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
	// MEASURE, don't guess. This answered 64 for any non-I type, and that default was the
	// root of three separate bugs in one night: (signed (bitwidth src) src) read a 64-bit
	// source as 32, (u64 x) read a Vec(128) as 64, and int-of's own site carries an
	// eleven-line comment warning that W(fv) on an F-typed value silently reads 64.
	// F and Vec both KNOW their width; there was never a reason to guess for them.
	//
	// MEASURED with a temporary CallerLineNumber instrument over 3,500,000 golden rows:
	// 232,128 calls arrived with an F type, at four ctxW-HINT sites (fmax/fmin, flt, and
	// the two n-ary fold sites). Those were harmless TODAY -- both goldens read 0 diff
	// before this change -- because the hint only types a literal operand and the float
	// paths in question don't take one. Harmless-today is the state that becomes a bug
	// when someone adds an arm, which is exactly how the three above happened.
	//
	// The 64 fallback stays for Void/unknown, where there is genuinely nothing to read.
	static int W(Il e) => e.Ty switch {
		IlType.I(_, var b) => b,
		IlType.F(var fb) => fb,
		IlType.Vec(var vb) => vb,
		_ => 64,
	};

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
					// x86 PARTIAL-WRITE: a write narrower than 64 does NOT zero the rest of
					// the register unless it is exactly 32 wide (32-bit ops zero-extend;
					// 8/16-bit ops insert and leave the upper bits alone). Zext-ing an
					// 8-bit value to 64 here wiped the upper 56 bits.
					// Found by XFReader at p1 row 201,044: CMPXCHG's mismatch arm is
					// `(= AX _rval)` where _rval is the 8-bit destination byte, and silicon
					// gave RAX=0x11223344556677ff where C# gave 0xff.
					// The insert form is transcribed from WriteOperand's own GPR case
					// (:385-395), which has had it right all along -- this path is the one
					// that takes IMPLICIT arch-reg names from an eval body (LEAVE's
					// `(= SP BP)`, CMPXCHG's `(= AX ...)`) and it never got the rule.
					var ew = W(e);
					Il val;
					if(ew >= 64) val = e;
					else if(ew == 32) val = new IlCast(IlType.U64, CastKind.Zext, e);   // 32-bit ops zero-extend
					else {
						// insert low `ew` bits, preserve the rest
						var keep = ~((1UL << ew) - 1);
						val = new IlBin(IlType.U64, BinOp.Or,
							new IlBin(IlType.U64, BinOp.And,
								new IlReadReg(IlType.U64, RegKind.X86, ar), new IlConst(IlType.U64, keep)),
							new IlCast(IlType.U64, CastKind.Zext, e));
					}
					Stmts.Add(new IlWriteReg(RegKind.X86, ar, val));
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
				// (if cond then...) has a variadic THEN; (if cond then else) is the
				// THREE-arg form and the .isa uses it -- CMPXCHG's
				//     (if (== tval 0x0) (= dst src) (= AX _rval))
				// is match-then-store / mismatch-then-load-AL. Taking Skip(2) as one
				// variadic then-list put BOTH arms in THEN and passed [] for ELSE, so the
				// mismatch arm ran on the match path and AL was never updated on a
				// mismatch. Found by XFReader at p1 row 201,044: silicon updates AL to the
				// destination byte, C# left RAX at its pre-value, and the dumped IL showed
				// both writes inside one block.
				//
				// Exactly the defect the Rust arm had (RustLiftGen's if/else arm put
				// both stmts in then, so CMPXCHG's NEQ arm was silently a no-op). Second
				// arm, same rule, same misreading -- and a variadic-then form is
				// indistinguishable from a then/else form without knowing the arity the
				// .isa means, which is why the wrong reading survived on both.
				//
				// ‡ Disambiguation: 3 args where the LAST is a statement = then/else. The
				// .isa has no 3-arg variadic-then site (verified: the only 3-arg `if`s in
				// LiftTables are CMPXCHG's and its siblings, all then/else). A future
				// variadic 3-arg then would need an explicit (block ...) wrapper, which is
				// what the .isa already writes when it means a multi-stmt arm.
				var isThenElse = l.Count == 4;
				var thenArgs = isThenElse ? l.Skip(2).Take(1) : l.Skip(2);
				foreach(var f in thenArgs) inner.Stmt(f);
				TmpN = inner.TmpN;
				var elseStmts = new List<Il>();
				if(isThenElse) {
					var eInner = new IlLower(OpWidth) { Binds = Binds, TmpN = TmpN };
					foreach(var (k, v) in Env) eInner.Env[k] = v;
					foreach(var (k, v) in MemAddr) eInner.MemAddr[k] = v;
					eInner.Stmt(l[3]);
					TmpN = eInner.TmpN;
					elseStmts = eInner.Stmts;
				}
				Stmts.Add(new IlIf(cond, inner.Stmts, elseStmts));
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
			// XMM SCALAR WRITE: low `w` bits only, upper 128-w PRESERVED. SDM Vol 2A
			// ADDSS: "The three high-order doublewords of the destination operand remain
			// unchanged." The comment that used to sit here said to write the value WHOLE
			// and gave a correct reason for the wrong conclusion -- x86's zext-32 and
			// masked-insert rules ARE GPR semantics, and an xmm write must not touch a
			// GPR, both true; neither implies that an xmm write replaces all 128 bits.
			//
			// The Rust arm fixed this on the silicon sweep's phase-2 FIRST FIRE, where it
			// was ~1,350 of 1,600 diffs in one operand rule (operand.rs:191-215, its
			// comment names the whole family: ADDSS/SUBSS/MULSS/DIVSS/SQRTSS/CMPSS + all
			// the SD forms + MOVSS/MOVSD reg,reg + CVTSS2SD/SD2SS). This side never got
			// it. XFReader found it at p2 row 2,480,156 -- and the tell was that `got` was
			// CONSTANT across three consecutive rows while `want` varied, which is not
			// what a wrong computation looks like; it is what a write that ignores the
			// destination looks like.
			//
			// Transcribed from that rule rather than composed: read full at V128, keep
			// the high bits, bitcast the value to its own integer width (bit-preserve, not
			// value-convert -- it may be F32/F64), widen into V128, mask defensively, or.
			// ‡ The mem->reg form ZEROES the upper bits instead of merging (Wss-mem is a
			// different bind), so this is the reg,reg and computed-result path.
			case OperandBind.Reg(var reg, var w, _, RegKind.Xmm) when w < 128: {
				var v128 = new IlType.Vec(128);
				var full = new IlReadReg(v128, RegKind.Xmm, reg);
				var mask = (UInt128.One << w) - 1;
				var kept = new IlBin(v128, BinOp.And, full, new IlConst(v128, ~mask));
				var vi = new IlCast(new IlType.I(false, w), CastKind.Bitcast, e);
				var vv = new IlCast(v128, CastKind.Zext, vi);
				var vlo = new IlBin(v128, BinOp.And, vv, new IlConst(v128, mask));
				Stmts.Add(new IlWriteReg(RegKind.Xmm, reg,
					new IlBin(v128, BinOp.Or, kept, vlo)));
				break;
			}
			// NON-GPR files (full-width Xmm, St, mask, seg): write the value WHOLE.
			case OperandBind.Reg(var reg, _, _, var f) when f != RegKind.X86:
				Stmts.Add(new IlWriteReg(f, reg, e));
				break;
			case OperandBind.Reg(var reg, 64, _, _):
				Stmts.Add(new IlWriteReg(RegKind.X86, reg, e));
				break;
			case OperandBind.Reg(var reg, 32, _, _):
				// x86-64 rule: 32-bit write ZERO-EXTENDS to 64 (not insert). TRUNC FIRST:
				// a bare Zext from a value that is ALREADY U64 is a NO-OP, so the upper 32
				// bits of a wider computed value survived into the register. Hit by MOVD
				// xmm->r32, whose source is a 128-bit XMM read narrowed to 64 (XFReader p2
				// row 3,430,889: got 0x8090a0b0c0d0e0f, want 0xc0d0e0f -- the low 32 are
				// right and the next 32 should not be there).
				//
				// This is the Rust arm's own LEA-32 bug at this site -- there `write_operand` did
				// cast(v, U64) for a 32-bit reg, which was likewise a no-op for an
				// already-U64 value, and LEA at op_w=32 wrote an untruncated address (129
				// sites in the CP2077 CRT). Fixed there as cast-to-U32-then-U64; same shape
				// here, and the reason it took a second finding to reach this arm is that
				// most 32-bit writes receive an expression already computed at 32.
				// ...and skip the trunc when e is ALREADY exactly 32 bits, which is the
				// common case and the one three golden IL tests pin: emitting
				// `(u64 zext (u32 trunc (u32 x)))` is semantically identical but changes the
				// tree, and a golden that pins the tree is right to reject it.
				Stmts.Add(new IlWriteReg(RegKind.X86, reg, new IlCast(IlType.U64, CastKind.Zext,
					e.Ty is IlType.I(_, 32) ? e : new IlCast(IlType.U32, CastKind.Trunc, e))));
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
				// An implicit arch-reg name in an eval body means the register AT THE
				// OPERATION'S WIDTH, not always 64. CMPXCHG-Eb's `(- AX _rval)` compares
				// AL against a byte; reading RAX whole made the compare 64-bit, so ZF was
				// wrong whenever the upper bytes were nonzero.
				// Found by XFReader at p1 row 201,053: silicon ZF=1 (AL == the byte), C#
				// ZF=0 (RAX 0x11223344556677xx != the byte), and the dumped IL read
				// `(let %0 = (u64 RAX))` in a byte-width template.
				// Mirrors ReadOperand's own GPR case (:466-470), which truncates to the
				// bind width -- same rule, and this implicit path never got it. Widths
				// above 64 are not a GPR thing; 64 needs no truncate.
				if(ArchReg(n) is { } ar) {
					var full = new IlReadReg(IlType.U64, RegKind.X86, ar);
					return ctxW is >= 64 or <= 0 ? full : new IlCast(U(ctxW), CastKind.Trunc, full);
				}
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
			// (u64 x) is the only width-cast head that was unconditionally ZEXT while its
			// three siblings above all TRUNC. That is right for a NARROWER source (the
			// common case -- widening a u32 to a u64) and wrong for a wider one, and the
			// wider one exists: MOVD/MOVQ xmm->GPR reads an XMM at Vec(128), so
			// `(= dst (u64 src))` emitted `(u64 zext (u128 XMM0))` and wrote BOTH 32-bit
			// lanes into the GPR. XFReader found it at p2 row 3,430,881 -- got
			// 0x3f8000003f800000 where silicon wants 0x3f800000, i.e. the value duplicated
			// rather than computed wrong, which is the signature of a cast that didn't cut.
			//
			// Direction now comes from the source's own width. ‡ W() returns 64 for any
			// non-I type (:67), so a Vec/F source reads as 64 and would pick Zext again --
			// hence the explicit Vec arm rather than relying on W(). That same W() gap is
			// documented at the int-of site (:1127) for the F axis and cost a bug there
			// too; this is its third site, and the honest fix for all three is for W() to
			// stop answering for types it cannot measure.
			case "u64": {
				var src = Expr(l[1]);
				var sw = src.Ty switch {
					IlType.I(_, var sb) => sb,
					IlType.Vec(var vb) => vb,
					IlType.F(var fb) => fb,
					_ => 64,
				};
				return sw > 64 ? new IlCast(IlType.U64, CastKind.Trunc, src)
				     : sw < 64 ? new IlCast(IlType.U64, CastKind.Zext, src)
				     : src.Ty is IlType.I ? src
				     : new IlCast(IlType.U64, CastKind.Bitcast, src);
			}
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
				//
				// THE KIND DEPENDS ON THE SOURCE and the previous form always said SToF,
				// which reads the source's BITS as a signed integer. For an int source that
				// is right; for a FLOAT source it converts the wrong value entirely, since
				// an f32's bit-pattern read as an integer is a large positive number.
				// CVTSS2SD of 1.0f (0x3F800000) gave 0x41CFC00000000000 = (double)
				// 0x3F800000 = 1065353216.0, where silicon gives 0x3FF0000000000000 = 1.0.
				// XFReader found it at p2 row 3,880,993 -- and the tell is that `got` is a
				// PLAUSIBLE double, so nothing about the value looks like a type error.
				//
				// Float->float wants FExt (widen) or FTrunc (narrow) per LiftIl's own
				// CastKind list (Il.cs:43-44), which carries all four kinds precisely
				// because they are not interchangeable.
				var a = Expr(l[1]);
				var w = h == "f32" ? 32 : 64;
				var kind = a.Ty switch {
					IlType.F(var sfw) => sfw == w ? CastKind.Bitcast     // same width: no-op
					                  : sfw < w  ? CastKind.FExt
					                             : CastKind.FTrunc,
					_ => CastKind.SToF,                                  // int source
				};
				return new IlCast(new IlType.F(w), kind, a);
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
				//
				// W CAN BE A `(bitwidth <operand>)` LIST, not only an integer literal, and
				// the previous form fell through to OpWidth for anything that wasn't a
				// PInt. CVTSI2SS/SD are `(signed (bitwidth src) src)` with src = Ey, so
				// under REX.W the source is 64 bits while OpWidth is the def's 32 — the
				// value got reinterpreted as i32 and any source with bit 31 set flipped
				// sign. XFReader found it at p2 row 2,939,745: cvtsi2ss of 0x80000000 gave
				// 0xCF000000 (-2^31 as f32) where silicon gives 0x4F000000 (+2^31).
				//
				// The Rust arm never had this because its generator emits the width from
				// the OPERAND (`ops[i].width()`, RustLiftGen:688-693) rather than resolving
				// it from the IL expression's type — so `bitwidth` there is a property of
				// the bind and here it is a property of the tree. Same head, two different
				// sources of truth, and only one of them knows about REX.W.
				//
				// ⚠ AND THE SIBLING SITE ALREADY CARRIES THIS EXACT TRAP, documented: the
				// f_to_si_x86 arm at :1099 handles `(bitwidth …)` explicitly and its own
				// comment warns that W() returns 64 for any non-I type. That warning was
				// eleven lines long and about the FLOAT axis; the int axis, in the same
				// file, silently discarded the whole list. A caveat at one site is not a
				// rule at the other.
				var w = l[1] switch {
					PInt(var sw) => (int) sw,
					PList bwl when bwl.Count == 2 && bwl[0] is PName("bitwidth") => W(Expr(bwl[1])),
					_ => OpWidth,
				};
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
			case "crc32": {
				// (crc32 acc src nbits) -- the SSE4.2 CRC32 accumulator step, CRC32C
				// (Castagnoli) reflected, POLY = 0x82F63B78.
				//
				// I CLAIMED THIS WAS PRIMITIVE-BLOCKED ("needs a polynomial table") in a DM
				// and in my own notes. It is not, and it is the FOURTH time on this file
				// that a deferral fell to writing out the decomposition:
				//     for each bit i:  bit = (acc & 1) ^ ((src >> i) & 1)
				//                      acc = (acc >> 1) ^ (POLY & (0 - bit))
				// which is {shr, xor, and, sub} unrolled nbits times -- all existing ops. A
				// TABLE is a speed choice, not a correctness requirement.
				//
				// KNOWN-ANSWER VERIFIED BEFORE WRITING THIS, and the first check FAILED:
				// chaining 'a','b','c' from acc=0 gives 0x562F9CCD, not the published
				// CRC32C("abc") = 0x364B3FB7. The published value includes init=0xFFFFFFFF
				// and a final invert; the INSTRUCTION does neither -- it is the raw step. Re-run
				// with init+invert: 0x364B3FB7 exact. So the step function was right and my
				// comparand was wrong, which is a different failure from a wrong formula and
				// would have read as one.
				var cacc = Expr(l[1]);
				var csrc = Expr(l[2]);
				var cnb = (int) ((PInt) l[3]).Value;
				var cu = new IlType.I(false, 32);
				var one = new IlConst(cu, 1);
				Il crc = new IlCast(cu, CastKind.Trunc, cacc);
				for(var i = 0; i < cnb; i++) {
					// Take bit i of the source AT ITS OWN WIDTH, then narrow -- mixing a
					// 64-bit source into a 32-bit xor directly would truncate the wrong end.
					var sh = new IlBin(csrc.Ty, BinOp.Shr, csrc,
						new IlConst(new IlType.I(false, 32), (UInt128) i));
					var sbit = new IlCast(cu, CastKind.Trunc,
						new IlBin(csrc.Ty, BinOp.And, sh, new IlConst(csrc.Ty, 1)));
					var abit = new IlBin(cu, BinOp.And, crc, one);
					var bit = new IlBin(cu, BinOp.Xor, abit, sbit);
					var mask = new IlBin(cu, BinOp.Sub, new IlConst(cu, 0), bit);
					crc = new IlBin(cu, BinOp.Xor,
						new IlBin(cu, BinOp.Shr, crc, one),
						new IlBin(cu, BinOp.And, new IlConst(cu, 0x82F63B78), mask));
				}
				return crc;
			}
			case "vmulw": {
				// (vmulw a b sew) -- PMULUDQ: r[i] = zext(a[2i]) * zext(b[2i]) at 2*sew.
				// A WIDENING multiply, which vibin cannot express (it is same-width
				// lane-wise) -- but the node set can: the widening is IlCast's own type and
				// the halved lane count is how many extracts there are. The vzext lesson.
				var wa = Expr(l[1]); var wb = Expr(l[2]);
				var wsew = (int) ((PInt) l[3]).Value;
				var wdew = wsew * 2;
				var wst = new IlType.I(false, wsew);
				var wdt = new IlType.I(false, wdew);
				var wel = new List<Il>();
				for(var k = 0; k < 128 / wdew; k++)
					wel.Add(new IlBin(wdt, BinOp.Mul,
						new IlCast(wdt, CastKind.Zext, Lane(wa, wst, k * 2)),
						new IlCast(wdt, CastKind.Zext, Lane(wb, wst, k * 2))));
				return new IlVecBuild(128, wdt, wel);
			}
			case "vmadd": {
				// (vmadd a b sew) -- PMADDWD: r[i] = a[2i]*b[2i] + a[2i+1]*b[2i+1], SIGNED,
				// widening sew -> 2*sew. Multiply-then-pairwise-add, which I had listed as
				// needing a new primitive -- it does not. Two sext-mul pairs and an add per
				// output lane, all existing nodes.
				var ma = Expr(l[1]); var mb = Expr(l[2]);
				var msew = (int) ((PInt) l[3]).Value;
				var mdew = msew * 2;
				var mst = new IlType.I(true, msew);
				var mdt = new IlType.I(true, mdew);
				var mel = new List<Il>();
				for(var k = 0; k < 128 / mdew; k++) {
					Il Prod(int li) => new IlBin(mdt, BinOp.Mul,
						new IlCast(mdt, CastKind.Sext, Lane(ma, mst, li)),
						new IlCast(mdt, CastKind.Sext, Lane(mb, mst, li)));
					mel.Add(new IlBin(mdt, BinOp.Add, Prod(k * 2), Prod(k * 2 + 1)));
				}
				return new IlVecBuild(128, mdt, mel);
			}
			case "vpacks": {
				// (vpacks a b sew) -- PACKSSDW/PACKSSWB: signed SATURATING narrow from sew
				// to sew/2, low half from a and high half from b.
				//
				// The saturation is the only interesting part and it needs no min/max node:
				//     hi = (1 << (dew-1)) - 1        lo = -(1 << (dew-1))
				//     clamped = v > hi ? hi : (v < lo ? lo : v)
				// which is two Slt/Sgt tests and two ternaries on a SCALAR lane -- and
				// IlTernary is already emitted by this file (one existing site).
				var pa = Expr(l[1]); var pb = Expr(l[2]);
				var psew = (int) ((PInt) l[3]).Value;
				var pdew = psew / 2;
				var pst = new IlType.I(true, psew);
				var pdt = new IlType.I(true, pdew);
				var pn = 128 / pdew;
				var phi = (UInt128) ((1UL << (pdew - 1)) - 1);
				var plo = (UInt128) (ulong) -(1L << (pdew - 1));
				var pel = new List<Il>();
				for(var k = 0; k < pn; k++) {
					var srcv = k < pn / 2 ? pa : pb;
					var v = Lane(srcv, pst, k % (pn / 2));
					var hiC = new IlConst(pst, phi);
					var loC = new IlConst(pst, plo & (UInt128) (ulong) MaskW(psew));
					Il Blend(Il cmp, Il pick, Il keep) {
						var m = new IlBin(pst, BinOp.Sub, new IlConst(pst, 0), cmp);
						return new IlBin(pst, BinOp.Or,
							new IlBin(pst, BinOp.And, pick, m),
							new IlBin(pst, BinOp.And, keep, new IlUn(pst, UnOp.Not, m)));
					}
					var tooHi = new IlBin(pst, BinOp.Sgt, v, hiC);
					var tooLo = new IlBin(pst, BinOp.Slt, v, loC);
					var clampLo = Blend(tooLo, loC, v);
					var clamped = Blend(tooHi, hiC, clampLo);
					pel.Add(new IlCast(pdt, CastKind.Trunc, clamped));
				}
				return new IlVecBuild(128, pdt, pel);
			}
			case "vdup": {
				// (vdup src ew odd) -- MOVSHDUP/MOVSLDUP: duplicate the odd (or even)
				// ew-wide lanes into both halves of each pair.
				//   odd:  r[0]=s[1] r[1]=s[1] r[2]=s[3] r[3]=s[3]
				//   even: r[0]=s[0] r[1]=s[0] r[2]=s[2] r[3]=s[2]
				// NO NEW NODE -- four CONSTANT-index picks plus a build, the PALIGNR shape.
				var us = Expr(l[1]);
				var uew = (int) ((PInt) l[2]).Value;
				var uodd = l[3] is PName("#t");
				var uet = new IlType.I(false, uew);
				var un = 128 / uew;
				var uel = new List<Il>();
				for(var k = 0; k < un; k++)
					uel.Add(Lane(us, uet, (k & ~1) + (uodd ? 1 : 0)));
				return new IlVecBuild(128, uet, uel);
			}
			case "vishr": {
				// (vishr dst count ew dir) -- the REGISTER-count shifts PSLLW/D/Q,
				// PSRLW/D/Q, PSRAW/D. dir: 0=shl 1=shr 2=sar. The count is the SOURCE
				// XMM's low 64 bits (encoding is (Vdq Wdq)), not an immediate -- which is
				// what separates these from the -I forms that already lower via vishi.
				//
				// NO NEW NODE. Two facts: the IlVecBin shift arms already treat their RHS
				// as a SCALAR broadcast rather than a vector (X86Machine.cs:714), and the
				// count>=ew saturation rule composes from scalar ops:
				//
				//   SHL/SHR:  mask = 0 - Ult(cnt, ew)      all-1s when cnt < ew, else 0
				//             res  = (vec <<|>> cnt) & broadcast(mask)
				//   SAR:      CLAMP the count instead: c' = min(cnt, ew-1), because an
				//             arithmetic shift by ew-1 IS the sign-fill -- one clamp covers
				//             both the in-range and the saturated case.
				//
				// Verified at 12 boundaries per width (0, 1, ew-1, ew, ew+5, 255) before
				// this was written. The mask broadcast is an IlVecBuild of n copies of one
				// scalar, which is needed because And is NOT a scalar-RHS op in the lane
				// arm -- only the shifts are, so an unbroadcast mask would be sliced
				// per-lane and land in lane 0 only. That is the same defect the shift
				// count itself had (8,096 p2 rows), one operand over.
				var hd = Expr(l[1]);
				var hsrc = Expr(l[2]);
				var hew = (int) ((PInt) l[3]).Value;
				var hdir = (int) ((PInt) l[4]).Value;
				var het = new IlType.I(hdir == 2, hew);
				var hcw = new IlType.I(false, hew);
				var hn = 128 / hew;
				// count = the source's low `ew` bits. The SDM reads the low 64, but any
				// count >= ew saturates identically, so narrowing to ew is safe ONLY if the
				// out-of-range test also uses the full value -- so test at 64 and shift at ew.
				var hcnt64 = Lane(hsrc, new IlType.I(false, 64), 0);
				var hlt = new IlBin(new IlType.I(false, 64), BinOp.Ult, hcnt64,
					new IlConst(new IlType.I(false, 64), (UInt128) hew));
				if(hdir == 2) {
					// SAR: c' = (cnt & m) | ((ew-1) & ~m), the mask-then-blend on a SCALAR.
					var m64 = new IlBin(new IlType.I(false, 64), BinOp.Sub,
						new IlConst(new IlType.I(false, 64), 0), hlt);
					var keep = new IlBin(new IlType.I(false, 64), BinOp.And, hcnt64, m64);
					var other = new IlBin(new IlType.I(false, 64), BinOp.And,
						new IlConst(new IlType.I(false, 64), (UInt128) (hew - 1)),
						new IlUn(new IlType.I(false, 64), UnOp.Not, m64));
					var cp = new IlBin(new IlType.I(false, 64), BinOp.Or, keep, other);
					return new IlVecBin(128, het, BinOp.Sar, hd, cp);
				}
				var shifted = new IlVecBin(128, het, hdir == 0 ? BinOp.Shl : BinOp.Shr, hd, hcnt64);
				var mk = new IlBin(hcw, BinOp.Sub, new IlConst(hcw, 0),
					new IlCast(hcw, CastKind.Trunc, hlt));
				var mel = new List<Il>();
				for(var k = 0; k < hn; k++) mel.Add(mk);
				return new IlVecBin(128, hcw, BinOp.And, shifted, new IlVecBuild(128, hcw, mel));
			}
			case "valign": {
				// (valign dst src sel) -- PALIGNR. Concatenate src:dst as 32 bytes (src is
				// the LOW half per the SDM) and take 16 starting at the immediate.
				//
				// NO NEW NODE. Every output byte is a CONSTANT index into ONE of two
				// vectors and the immediate is compile-time, so this is the vlane-get shape
				// sixteen times with the src-vs-dst choice made AT CODEGEN plus a build.
				// The out-of-range rules fall out of the same arithmetic rather than needing
				// a branch: idx >= 32 yields a zero byte, and 16 <= idx < 32 reads dst.
				var ad = Expr(l[1]);
				var asrc = Expr(l[2]);
				var aselN = ((PName) l[3]).Name;
				if(!Binds.TryGetValue(aselN, out var asb) || asb is not OperandBind.Imm asi)
					throw new NotSupportedException($"valign sel {aselN} not an imm bind");
				var ash = (int) ((ulong) asi.Value & 0xFF);
				var ab8 = new IlType.I(false, 8);
				var ael = new List<Il>();
				for(var k = 0; k < 16; k++) {
					var idx = ash + k;
					ael.Add(idx >= 32 ? new IlConst(ab8, 0)
						: idx >= 16 ? Lane(ad, ab8, idx - 16)
						: Lane(asrc, ab8, idx));
				}
				return new IlVecBuild(128, ab8, ael);
			}
			case "vlane-get": {
				// (vlane-get src sel ew) -- PEXTRB/PEXTRW/PEXTRD/PEXTRQ. NO NEW NODE: an
				// extract with a COMPILE-TIME index, which is what IlVecElem already is
				// everywhere else in this file. The selector arrives as an Ib OPERAND
				// BINDING, not a literal, so it is resolved the way vshuf resolves its own
				// (IlLower.cs:1230-1234): Binds -> OperandBind.Imm -> the value.
				var gsrc = Expr(l[1]);
				var gselN = ((PName) l[2]).Name;
				var gew = (int) ((PInt) l[3]).Value;
				if(!Binds.TryGetValue(gselN, out var gsb) || gsb is not OperandBind.Imm gsi)
					throw new NotSupportedException($"vlane-get sel {gselN} not an imm bind");
				var gn = 128 / gew;
				var gidx = (int) ((ulong) gsi.Value & (ulong) (gn - 1));
				return Lane(gsrc, new IlType.I(false, gew), gidx);
			}
			case "vlane-set": {
				// (vlane-set dst val sel ew) -- PINSRB/PINSRW/PINSRD/PINSRQ. NO NEW NODE
				// either: rebuild all n lanes, substituting the one the selector names.
				// That is the same shape vzext uses (a build over per-lane extracts) and
				// the same compile-time-selector resolution as vlane-get.
				var sdst = Expr(l[1]);
				var sval = Expr(l[2]);
				var sselN = ((PName) l[3]).Name;
				var sew = (int) ((PInt) l[4]).Value;
				if(!Binds.TryGetValue(sselN, out var ssb) || ssb is not OperandBind.Imm ssi)
					throw new NotSupportedException($"vlane-set sel {sselN} not an imm bind");
				var sn = 128 / sew;
				var sidx = (int) ((ulong) ssi.Value & (ulong) (sn - 1));
				var slet = new IlType.I(false, sew);
				var sel2 = new List<Il>();
				for(var k = 0; k < sn; k++)
					// The inserted value is a GPR read at its own width; narrow it to the
					// lane width rather than assuming the widths match.
					sel2.Add(k == sidx ? new IlCast(slet, CastKind.Trunc, sval) : Lane(sdst, slet, k));
				return new IlVecBuild(128, slet, sel2);
			}
			case "vshufv": {
				// (vshufv dst src) -- PSHUFB. A DATA-DEPENDENT shuffle: the per-lane index
				// comes from a REGISTER, not an immediate, and bit 7 of each index zeroes
				// its output lane.
				//
				// NO NEW NODE. Two facts made it expressible and both were checked rather
				// than assumed: IlVecElem.Idx is an `Il`, not a constant, so a RUNTIME
				// index is already representable and my eval arm already evaluates one
				// (X86Machine.cs:699 does `(int) N64(Eval(vi))`); and there is no
				// IlTernary anywhere in LiftIl, so the zeroing rule uses the sign-mask
				// idiom on a scalar lane instead of a select:
				//     ix   = elem(src, i, U8)
				//     sm   = Sar(ix, 7)              all-1s if bit 7 set, else 0
				//     lane = elem(dst, ix & 0xF, U8) & ~sm
				// Verified byte-exact against the SDM rule on a 16-lane case including
				// bit-7-set, index-wrap (0x1F -> lane 0xF) and 0xFF before writing it.
				var sd = Expr(l[1]);
				var ss = Expr(l[2]);
				var b8 = new IlType.I(false, 8);
				var sel = new List<Il>();
				for(var k = 0; k < 16; k++) {
					var ix = Lane(ss, b8, k);
					var sm = new IlBin(b8, BinOp.Sar, ix, new IlConst(new IlType.I(false, 32), 7));
					var lo = new IlBin(b8, BinOp.And, ix, new IlConst(b8, 0xF));
					var val = new IlVecElem(b8, sd, lo);
					sel.Add(new IlBin(b8, BinOp.And, val, new IlUn(b8, UnOp.Not, sm)));
				}
				return new IlVecBuild(128, b8, sel);
			}
			case "vzext": {
				// (vzext a sew dew) -- PMOVZXBW/PMOVZXWD: take the LOW 128/dew lanes of a
				// at width sew and zero-extend each into a dew-wide lane.
				//
				// NO NEW NODE, and NOT via vzip-with-a-zero-operand even though that also
				// computes it (verified byte-exact first: interleaving with zeros IS a
				// zero-extension). The DSL has no vector-literal form, so a bare 0 would
				// lower as a SCALAR IlConst and Lane() would wrap it in an IlVecElem over
				// a scalar -- an operand shape nothing else in the tree produces. The
				// direct build needs no literal at all:
				//     build(dew, [ Zext(elem(a, i, sew), dew) for i in 0..128/dew ])
				// which is the vcvt lesson (day-57) reused: a LANE-COUNT change is just
				// how many extracts there are, and the width change rides IlCast's own
				// type. Both nodes already emitted and evaluated.
				var za = Expr(l[1]);
				var sew = (int) ((PInt) l[2]).Value;
				var dew = (int) ((PInt) l[3]).Value;
				var set = new IlType.I(false, sew);
				var det = new IlType.I(false, dew);
				var zn = 128 / dew;
				var zel = new List<Il>();
				for(var k = 0; k < zn; k++)
					zel.Add(new IlCast(det, CastKind.Zext, Lane(za, set, k)));
				return new IlVecBuild(128, det, zel);
			}
			case "viabs": {
				// (viabs a ew) -- PABSB/PABSW/PABSD. NO NEW NODE, and no UnOp.Abs on an
				// integer lane either: abs is the sign-mask identity, three ops I already
				// emit and evaluate.
				//     sign = Sar(x, ew-1)        all-1s if negative, 0 if not
				//     abs  = Sub(Xor(x, sign), sign)
				// Verified at the boundaries before writing it, including the one that
				// looks wrong: 0x80 (-128) -> 0x80, which is what x86 does (INT_MIN has
				// no positive representation at the width; SDM says the result is
				// INT_MIN). A composed "clamp to 0x7F" would be the wrong fix for a
				// correct answer.
				var xa = Expr(l[1]);
				var aew = (int) ((PInt) l[2]).Value;
				var aet = new IlType.I(true, aew);
				var shamt = new IlConst(new IlType.I(false, 32), (UInt128) (aew - 1));
				var sign = new IlVecBin(128, aet, BinOp.Sar, xa, shamt);
				return new IlVecBin(128, aet, BinOp.Sub,
					new IlVecBin(128, aet, BinOp.Xor, xa, sign), sign);
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
				// ops 5-8 = PMAX/PMIN, signed and unsigned. THE NODE SET EXPRESSES THESE
				// AND NO NEW BinOp IS NEEDED, which is the same question I got wrong four
				// times on this file's other heads: I asked whether ONE node carries the
				// operation instead of whether the SET does.
				//
				// BinOp has FMin/FMax and no integer siblings, so adding SMax/UMin would
				// be a shared-LiftIl change with a live consumer. It isn't necessary --
				// per-lane max is the mask-then-blend idiom the all-1s convention above
				// exists for:
				//     mask = (a >  b)        all-1s per lane where a wins   (op 4's shape)
				//     res  = (a & mask) | (b & ~mask)
				// Three nodes I already emit and already evaluate (IlVecBin And/Or,
				// IlVecUn Not), and the SIGNEDNESS rides the compare's ElemTy alone --
				// Sgt with I(true,ew) for PMAXS*, Ugt with I(false,ew) for PMAXU*.
				// A blend needs no select node because a lane mask IS the select.
				if(op >= 5 && op <= 8) {
					var signed = op is 5 or 6;              // 5=maxs 6=mins 7=maxu 8=minu
					var wantA = op is 5 or 7;               // max keeps a where a > b
					var cet = new IlType.I(signed, ew);
					var cmp = new IlVecBin(128, cet, signed ? BinOp.Sgt : BinOp.Ugt, a, b);
					var keep = wantA ? a : b;               // the operand the mask selects
					var other = wantA ? b : a;
					var inv = new IlVecUn(128, cet, UnOp.Not, cmp);
					return new IlVecBin(128, cet, BinOp.Or,
						new IlVecBin(128, cet, BinOp.And, keep, cmp),
						new IlVecBin(128, cet, BinOp.And, other, inv));
				}
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
				// 2^(iw-1) as a float constant, BY BIT-PATTERN. The previous form built it
				// with SToF(1 << (iw-1)) -- and at iw=64 that shifts into the SIGN BIT, so
				// the "limit" was SToF(0x8000000000000000) read as a signed i64 = -2^63.
				// |f| < (a negative number) is FALSE for every input, so every 64-bit
				// conversion returned the indefinite integer: CVTSS2SI/CVTSD2SI with REX.W
				// produced 0x8000000000000000 regardless of the source value. XFReader found
				// it at p2 row 2,995,775 (got constant while want varied 0,1,2,... which is
				// the signature of a guard that can only go one way).
				//
				// It fired ONLY at iw=64 -- at iw=32 the same expression is SToF(0x80000000)
				// = +2147483648.0, correct, which is why the 32-bit forms were clean and the
				// bug sat behind a passing majority.
				//
				// Transcribed from the Rust arm (operand.rs:70-77), which uses the literal
				// bit-patterns for exactly this reason and says so: "bit-patterns
				// objdump/python-verified (not composed)". Re-verified here before use:
				//   2^31 as f32 = 0x4F000000            2^63 as f32 = 0x5F000000
				//   2^31 as f64 = 0x41E0000000000000    2^63 as f64 = 0x43E0000000000000
				var limit = new IlConst(fty, (fw, iw) switch {
					(32, 32) => (UInt128) 0x4F000000,
					(32, 64) => (UInt128) 0x5F000000,
					(64, 32) => (UInt128) 0x41E0000000000000,
					(64, 64) => (UInt128) 0x43E0000000000000,
					_ => throw new NotSupportedException($"int-of fw={fw} iw={iw}"),
				});
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
