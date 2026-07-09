using CoreArchCompiler;

namespace XFusionGenerator;

/// XF-4: lowers .isa eval bodies to the consumer IL tree shape (M1-GOLDEN.md is
/// the acceptance form). Internal node set mirrors Pagentry.Lifter/Il.cs shapes;
/// the renderer emits the golden's text form. When the shared-IL repo location
/// lands (sera's call, ·51), the renderer swaps for real Il ctor emission — the
/// tree is the contract, the text is the current test surface.
///
/// Width model: widths are bits (1/8/16/32/64). Every expr carries a width.
/// Flag-write canonicalization (M1-GOLDEN note, one rule picked):
///   comparison → u1 directly; top-level (& _ 1) or (>> _ width-1) → trunc-to-u1
///   (bit 0 IS the flag); anything else → ne-#0 (bit 0 is NOT the flag — OF).
public abstract record Ilx(int W) {
	public sealed record Const(int Wc, long V) : Ilx(Wc);
	public sealed record ReadReg(string Name) : Ilx(64);                    // arch reg, 64-bit file
	public sealed record Tmp(int Wt, string Name) : Ilx(Wt);
	public sealed record Bin(int Wb, string Op, Ilx A, Ilx B) : Ilx(Wb);
	public sealed record Cast(int Wt, string Kind, Ilx A) : Ilx(Wt);        // trunc/zext/sext
	public sealed record Load(int Wl, Ilx Addr) : Ilx(Wl);
	public sealed record Pop(int Wp) : Ilx(Wp);                             // stack pop (consumer lowers vs RSP)
	public sealed record AddrOf(string Operand) : Ilx(64);                  // LEA: the mem operand's ADDRESS expr
	public sealed record Ite(int Wi, Ilx Cond, Ilx T, Ilx F) : Ilx(Wi);     // conditional value (CMOVcc data form)
}

public abstract record IlxStmt {
	public sealed record Let(Ilx.Tmp T, Ilx E) : IlxStmt;
	public sealed record If(Ilx Cond, List<IlxStmt> Then) : IlxStmt;     // consumer: IlBranch or predication — table item
	public sealed record Branch(string Kind, Ilx Target) : IlxStmt;     // jmp/call/ret
	public sealed record Push(Ilx E) : IlxStmt;                         // abstract stack op (consumer lowers vs RSP)
	public sealed record WriteReg(string Name, Ilx E) : IlxStmt;
	public sealed record WriteFlag(string Flag, Ilx E) : IlxStmt;           // E is u1
	public sealed record Store(Ilx Addr, Ilx E) : IlxStmt;
}

/// Operand binding: how a template param reads/writes for a CONCRETE instance.
public abstract record OperandBind {
	public sealed record Reg(string Name, int Width) : OperandBind;         // e.g. X86_RAX, 32
	public sealed record Mem(Ilx AddrExpr, int Width) : OperandBind;        // addr already built
	public sealed record Imm(long Value, int Width) : OperandBind;
}

public class IlLower {
	readonly List<IlxStmt> Stmts = [];
	readonly Dictionary<string, Ilx> Env = [];   // template params + mlet/let names
	readonly Dictionary<string, Ilx.Tmp> MemAddr = [];  // operand name -> bound addr tmp
	int TmpN;
	readonly int OpWidth;                        // instruction operand width (v-width)

	IlLower(int opWidth) => OpWidth = opWidth;

	/// Implicit arch-reg names usable directly in eval bodies (LEAVE: (= SP BP)).
	/// v-sized reads of the 64-bit file; the M4 generator will width-select — for
	/// the tree, the 64-bit reg is the honest object.
	static string ArchReg(string n) => n switch {
		"SP" => "X86_RSP", "BP" => "X86_RBP", "AX" => "X86_RAX", "CX" => "X86_RCX",
		"DX" => "X86_RDX", "BX" => "X86_RBX", "SI" => "X86_RSI", "DI" => "X86_RDI",
		_ => null
	};

	/// Lower one instruction instance: template params + eval body forms + concrete
	/// operand bindings. Returns the statement block.
	public static List<IlxStmt> Lower(IReadOnlyList<string> params_, IEnumerable<PTree> evalForms,
		IReadOnlyDictionary<string, OperandBind> binds, int opWidth) {
		var l = new IlLower(opWidth);
		// Pre-bind memory operand addresses (x86: address evaluated ONCE per insn).
		foreach(var (name, b) in binds)
			if(b is OperandBind.Mem mem) {
				var t = new Ilx.Tmp(64, "%addr" + (l.MemAddr.Count == 0 ? "" : l.MemAddr.Count.ToString()));
				l.Stmts.Add(new IlxStmt.Let(t, mem.AddrExpr));
				l.MemAddr[name] = t;
			}
		foreach(var p in params_)
			l.Env[p] = null;  // params resolve through binds at use
		l.Binds = binds;
		foreach(var form in evalForms)
			l.Stmt(form);
		return l.Stmts;
	}

	IReadOnlyDictionary<string, OperandBind> Binds;

	Ilx.Tmp NewTmp(int w) => new(w, $"%{TmpN++}");

	// ---- statements ----
	void Stmt(PTree t) {
		if(t is not PList l || l.Count == 0) throw new NotSupportedException($"stmt {t}");
		switch(l[0]) {
			case PName("mlet"): {
				// (mlet (n1 e1 n2 e2 ...) body...) — pairs bind in order, each as a let
				var pairs = (PList) l[1];
				for(var i = 0; i + 1 < pairs.Count; i += 2) {
					var name = ((PName) pairs[i]).Name;
					var e = Expr(pairs[i + 1]);
					var tmp = NewTmp(e.W);
					Stmts.Add(new IlxStmt.Let(tmp, e));
					Env[name] = tmp;
				}
				foreach(var body in l.Skip(2)) Stmt(body);
				break;
			}
			case PName("let"): {
				// (let name expr body...) — single binding, inner scope
				var name = ((PName) l[1]).Name;
				var e = Expr(l[2]);
				var tmp = NewTmp(e.W);
				Stmts.Add(new IlxStmt.Let(tmp, e));
				Env[name] = tmp;
				foreach(var body in l.Skip(3)) Stmt(body);
				break;
			}
			case PName("="): {
				var target = ((PName) l[1]).Name;
				var e = Expr(l[2]);
				if(IsFlag(target)) { Stmts.Add(new IlxStmt.WriteFlag(FlagName(target), CanonFlag(e))); break; }
				if(Binds.TryGetValue(target, out var b)) { WriteOperand(target, b, e); break; }
				if(ArchReg(target) is { } ar) { Stmts.Add(new IlxStmt.WriteReg(ar, e.W == 64 ? e : new Ilx.Cast(64, "zext", e))); break; }
				throw new NotSupportedException($"write target {target}");
			}
			case PName("block"):
				foreach(var f in l.Skip(1)) Stmt(f);
				break;
			case PName("if"): {
				var cond = CanonFlag(Expr(l[1]));
				// CMOVcc data form: (if C (= dst src)) with dst a reg bind → Ite (no control flow)
				if(l.Count == 3 && l[2] is PList { Count: 3 } asn && asn[0] is PName("=")
					&& asn[1] is PName(var dn) && Binds.TryGetValue(dn, out var db) && db is OperandBind.Reg) {
					var val = Expr(asn[2]);
					WriteOperand(dn, db, new Ilx.Ite(val.W, cond, val, ReadOperand(dn, db)));
					break;
				}
				// general form: guarded stmt block (SHL flag-writes)
				var inner = new IlLower(OpWidth) { Binds = Binds, TmpN = TmpN };
				foreach(var (k, v) in Env) inner.Env[k] = v;
				foreach(var (k, v) in MemAddr) inner.MemAddr[k] = v;
				foreach(var f in l.Skip(2)) inner.Stmt(f);
				TmpN = inner.TmpN;
				Stmts.Add(new IlxStmt.If(cond, inner.Stmts));
				break;
			}
			case PName("push"):
				Stmts.Add(new IlxStmt.Push(Expr(l[1])));
				break;
			case PName("branch"):
				Stmts.Add(new IlxStmt.Branch("jmp", Expr(l[1], 64)));
				break;
			case PName("intrinsic"):
				// intrinsic node — passthrough (consumer IlIntrin); not in golden scope
				throw new NotSupportedException("intrinsic lowering is XF-5 scope");
			default:
				throw new NotSupportedException($"stmt head {l[0]}");
		}
	}

	void WriteOperand(string name, OperandBind b, Ilx e) {
		switch(b) {
			case OperandBind.Reg(var reg, 64):
				Stmts.Add(new IlxStmt.WriteReg(reg, e));
				break;
			case OperandBind.Reg(var reg, 32):
				// x86-64 rule: 32-bit write ZERO-EXTENDS to 64 (not insert)
				Stmts.Add(new IlxStmt.WriteReg(reg, new Ilx.Cast(64, "zext", e)));
				break;
			case OperandBind.Reg(var reg, var w):  // 8/16: masked insert
				Stmts.Add(new IlxStmt.WriteReg(reg, new Ilx.Bin(64, "or",
					new Ilx.Bin(64, "and", new Ilx.ReadReg(reg), new Ilx.Const(64, ~((1L << w) - 1))),
					new Ilx.Cast(64, "zext", e))));
				break;
			case OperandBind.Mem:
				Stmts.Add(new IlxStmt.Store(MemAddr[name], e));
				break;
			default:
				throw new NotSupportedException($"write to {b}");
		}
	}

	// ---- expressions ----
	Ilx Expr(PTree t) => Expr(t, OpWidth);

	Ilx Expr(PTree t, int ctxW) {
		switch(t) {
			case PInt(var v): return new Ilx.Const(ctxW, v);
			case PName(var n): {
				if(Env.TryGetValue(n, out var bound) && bound != null) return bound;
				if(Binds != null && Binds.TryGetValue(n, out var b)) return ReadOperand(n, b);
				if(IsFlag(n)) return new Ilx.Tmp(1, "EFLAGS." + FlagName(n));  // flag read
				if(ArchReg(n) is { } ar) return new Ilx.ReadReg(ar);           // SP/BP/AX... implicit regs
				throw new NotSupportedException($"name {n}");
			}
			case PList l when l.Count >= 1: return ListExpr(l, ctxW);
			default: throw new NotSupportedException($"expr {t}");
		}
	}

	Ilx ReadOperand(string name, OperandBind b) => b switch {
		OperandBind.Reg(var reg, 64) => new Ilx.ReadReg(reg),
		OperandBind.Reg(var reg, var w) => new Ilx.Cast(w, "trunc", new Ilx.ReadReg(reg)),
		OperandBind.Mem(_, var w) => LoadOnce(name, w),
		OperandBind.Imm(var v, var w) => new Ilx.Const(w, v),
		_ => throw new NotSupportedException(b.ToString())
	};

	Ilx LoadOnce(string name, int w) => new Ilx.Load(w, MemAddr[name]);

	Ilx ListExpr(PList l, int ctxW) {
		var head = l[0] is PName(var h) ? h : throw new NotSupportedException(l[0].ToString());
		switch(h) {
			// width casts: (u8 x) etc
			case "u8": return new Ilx.Cast(8, "trunc", Expr(l[1]));
			case "u16": return new Ilx.Cast(16, "trunc", Expr(l[1]));
			case "u32": return new Ilx.Cast(32, "trunc", Expr(l[1]));
			case "u64": return new Ilx.Cast(64, "zext", Expr(l[1]));
			case "bitwidth": {
				var e = Expr(l[1]);
				return new Ilx.Const(ctxW, e.W);
			}
			case "pop": return new Ilx.Pop(OpWidth);
			case "next-pc": return new Ilx.Tmp(64, "%nextpc");  // consumer: IlReadPc + len (lift-time const)
			case "addr-of": {
				// LEA: the operand must be a mem bind — its ADDRESS, not its value
				var opName = ((PName) l[1]).Name;
				return Binds[opName] is OperandBind.Mem ? MemAddr[opName]
					: throw new NotSupportedException("addr-of non-mem operand");
			}
			case "sext": {
				// (sext x toWidth?) — default to op width
				var a = Expr(l[1]);
				var w = l.Count > 2 && l[2] is PInt(var wv) ? (int) wv : OpWidth;
				return new Ilx.Cast(w, "sext", a);
			}
			case "zext": {
				var a = Expr(l[1]);
				var w = l.Count > 2 && l[2] is PInt(var wv) ? (int) wv : OpWidth;
				return new Ilx.Cast(w, "zext", a);
			}
			case "~": {
				var a = Expr(l[1], ctxW);
				return new Ilx.Bin(a.W, "xor", a, new Ilx.Const(a.W, -1));
			}
		}
		// comparisons → u1
		if(h is "<" or "==" or "!=" or ">") {
			var a = Expr(l[1], ctxW);
			var b = Expr(l[2], a is Ilx.Const ? ctxW : a.W);
			if(a is Ilx.Const ca && b.W != a.W) a = ca with { Wc = b.W };
			var op = h switch { "<" => "ult", "==" => "eq", "!=" => "ne", ">" => "ugt", _ => null };
			return new Ilx.Bin(1, op, a, b);
		}
		// n-ary fold: (op a b c) → (op (op a b) c)
		var op2 = h switch {
			"+" => "add", "-" => "sub", "*" => "mul",
			"&" => "and", "|" => "or", "^" => "xor",
			">>" => "shr", "<<" => "shl", "!" => "not",
			">>a" => "sar", "rotl" => "rotl", "rotr" => "rotr",
			_ => throw new NotSupportedException($"op {h}")
		};
		if(op2 == "not") return new Ilx.Bin(1, "eq", Expr(l[1]), new Ilx.Const(Expr(l[1]).W, 0));
		var isShift = op2 is "shr" or "shl" or "sar" or "rotl" or "rotr";
		var acc = Expr(l[1], ctxW);
		for(var i = 2; i < l.Count; i++) {
			var rhs = Expr(l[i], acc.W);
			int w;
			if(isShift) {
				// shifts: result width = LEFT width; a const left keeps ctxW (the PF
				// 0x6996 table must NOT shrink to the shift-amount's width); rhs width
				// is irrelevant to the result.
				w = acc.W;
			} else {
				w = Math.Max(acc.W, rhs.W);
				// constants adopt sibling width
				if(acc is Ilx.Const c1 && rhs is not Ilx.Const) { acc = c1 with { Wc = rhs.W }; w = rhs.W; }
				if(rhs is Ilx.Const c2 && acc is not Ilx.Const) { rhs = c2 with { Wc = acc.W }; w = acc.W; }
			}
			acc = Fold(new Ilx.Bin(w, op2, acc, rhs));
		}
		return acc;
	}

	/// Constant-fold pure-constant binops ((<< 1 31) → #80000000; (- 32 1) → #1f).
	static Ilx Fold(Ilx.Bin b) {
		if(b.A is Ilx.Const(_, var x) && b.B is Ilx.Const(_, var y)) {
			long? v = b.Op switch {
				"add" => x + y, "sub" => x - y, "mul" => x * y,
				"and" => x & y, "or" => x | y, "xor" => x ^ y,
				"shl" => x << (int) y, "shr" => (long) ((ulong) x >> (int) y),
				_ => null
			};
			if(v is { } vv) return new Ilx.Const(b.W, vv);
		}
		return b;
	}

	/// Flag-write canonicalization (see class doc).
	static Ilx CanonFlag(Ilx e) {
		if(e.W == 1) return e;                                            // comparison chain
		if(e is Ilx.Const(_, var cv) && cv is 0 or 1) return new Ilx.Const(1, cv);  // (= CF 0)
		switch(e) {
			case Ilx.Bin(_, "and", _, Ilx.Const(_, 1)):                   // (& _ 1) → bit0 valid
			case Ilx.Bin(_, "shr", var a, Ilx.Const(_, var sh)) when sh == a.W - 1:  // sign shift
				return new Ilx.Cast(1, "trunc", e);
			default:
				return new Ilx.Bin(1, "ne", e, new Ilx.Const(e.W, 0));    // OF class
		}
	}

	static bool IsFlag(string n) => n is "CF" or "PF" or "AF" or "ZF" or "SF" or "OF" or "DF";
	static string FlagName(string n) => n[..1];

	// ---- renderer: the golden text form ----
	public static string Render(List<IlxStmt> stmts) {
		var sb = new System.Text.StringBuilder("(block\n");
		foreach(var s in stmts) sb.Append("  ").Append(RenderStmt(s)).Append('\n');
		return sb.Append(')').ToString();
	}

	static string RenderStmt(IlxStmt s) => s switch {
		IlxStmt.Let(var t, var e) => $"(let {t.Name} = {R(e)})",
		IlxStmt.WriteReg(var n, var e) => $"({n} := {R(e)})",
		IlxStmt.WriteFlag(var f, var e) => $"(EFLAGS.{f} := {R(e)})",
		IlxStmt.Store(var a, var e) => $"(store {R(a)} {R(e)})",
		IlxStmt.If(var c, var body) => $"(if {R(c)} {string.Join(' ', body.Select(RenderStmt))})",
		IlxStmt.Branch(var k, var t2) => $"({k} {R(t2)})",
		IlxStmt.Push(var e) => $"(push {R(e)})",
		_ => throw new NotSupportedException(s.ToString())
	};

	static string R(Ilx e) => e switch {
		Ilx.Const(var w, var v) => $"(u{w} #{v & MaskW(w):x})",
		Ilx.ReadReg(var n) => $"(u64 {n})",
		Ilx.Tmp(var w, var n) => n.StartsWith("EFLAGS") ? $"(u1 {n})" : $"(u{w} {n})",
		Ilx.Bin(var w, var op, var a, var b) => $"(u{w} {op} {R(a)} {R(b)})",
		Ilx.Cast(var w, var k, var a) => $"(u{w} {k} {R(a)})",
		Ilx.Load(var w, var a) => $"(u{w} load {R(a)})",
		Ilx.Pop(var w) => $"(u{w} pop)",
		Ilx.AddrOf(var o) => $"(u64 addr {o})",
		Ilx.Ite(var w, var c, var t2, var f) => $"(u{w} ite {R(c)} {R(t2)} {R(f)})",
		_ => throw new NotSupportedException(e.ToString())
	};

	static long MaskW(int w) => w >= 64 ? -1L : (1L << w) - 1;
}
