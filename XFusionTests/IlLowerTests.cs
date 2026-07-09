using CoreArchCompiler;
using XFusionGenerator;  // IlLower/OperandBind shared-source (compiled into XFusionCpu)
using LiftIl;

namespace XFusionTests;

/// XF-4 acceptance rows (M1-GOLDEN.md): the walker lowers the REAL ADD template
/// from ia32-base.isa — parsed, not restated — and must reproduce the golden
/// modulo whitespace. Golden-first per the house form.
[TestFixture]
public class IlLowerTests {
	static (List<string> Params, List<PTree> Eval) LoadAdd() {
		var src = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory,
			"../../../../XFusionGenerator/ia32-base.isa"));
		var top = ListParser.Parse(src);
		foreach(var elem in top)
			if(elem is PList { Count: >= 4 } pl && pl[0] is PName("instruction")
				&& pl[1] is PName("ADD")) {
				var ps = ((PList) pl[2]).Select(x => ((PName) x).Name).ToList();
				return (ps, pl.Skip(4).ToList());  // skip head/name/params/dasm
			}
		throw new InvalidOperationException("ADD template not found");
	}

	[Test]
	public void AddRegReg() {  // 01 D8 = add eax, ebx (mode=64)
		var (ps, eval) = LoadAdd();
		var stmts = IlLower.Lower(ps, eval, new Dictionary<string, OperandBind> {
			["lval"] = new OperandBind.Reg(0, 32),
			["rval"] = new OperandBind.Reg(3, 32),
		}, 32);
		var got = Render(stmts);
		var expect = """
(block
  (let %0 = (u32 trunc (u64 RAX)))
  (let %1 = (u32 trunc (u64 RBX)))
  (let %2 = (u32 add (u32 %0) (u32 %1)))
  (RAX := (u64 zext (u32 %2)))
  (EFLAGS.C := (u1 or (u1 ult (u32 %2) (u32 %0)) (u1 ult (u32 %2) (u32 %1))))
  (EFLAGS.S := (u1 trunc (u32 shr (u32 %2) (u32 #1f))))
  (EFLAGS.O := (u1 ne (u32 and (u32 and (u32 xor (u32 %2) (u32 %0)) (u32 xor (u32 %2) (u32 %1))) (u32 #80000000)) (u32 #0)))
  (EFLAGS.A := (u1 trunc (u32 and (u32 shr (u32 xor (u32 xor (u32 %0) (u32 %1)) (u32 %2)) (u32 #4)) (u32 #1))))
  (let %3 = (u8 trunc (u32 %2)))
  (EFLAGS.P := (u1 trunc (u32 and (u32 shr (u32 #6996) (u8 and (u8 xor (u8 %3) (u8 shr (u8 %3) (u8 #4))) (u8 #f))) (u32 #1))))
  (EFLAGS.Z := (u1 eq (u32 %2) (u32 #0)))
)
""";
		Assert.That(Norm(got), Is.EqualTo(Norm(expect)));
	}

	[Test]
	public void AddMemReg() {  // 01 5D F0 = add dword ptr [rbp-0x10], ebx
		var (ps, eval) = LoadAdd();
		var addr = new IlBin(IlType.U64, BinOp.Add, new IlReadReg(IlType.U64, RegKind.X86, 5), new IlConst(IlType.U64, unchecked((ulong)-16L)));
		var stmts = IlLower.Lower(ps, eval, new Dictionary<string, OperandBind> {
			["lval"] = new OperandBind.Mem(addr, 32),
			["rval"] = new OperandBind.Reg(3, 32),
		}, 32);
		var got = Render(stmts);
		// structural assertions per golden row 2:
		Assert.That(got, Does.Contain("(let %0 = (u64 add (u64 RBP) (u64 #fffffffffffffff0)))"));
		Assert.That(got, Does.Contain("(u32 load (u64 %0))"));           // lval reads via the SHARED addr
		Assert.That(got, Does.Contain("(store (u64 %0)"));               // write-back to SAME addr
		Assert.That(got.IndexOf("(store"), Is.GreaterThan(got.IndexOf("load")));
		// exactly one addr computation
		Assert.That(got.Split("%0 =").Length, Is.EqualTo(2));
		// flag block identical shape to row 1 (spot: CF + Z)
		Assert.That(got, Does.Contain("(EFLAGS.C := (u1 or (u1 ult"));
		Assert.That(got, Does.Contain("(EFLAGS.Z := (u1 eq"));
	}

	static string Norm(string s) => string.Join('\n',
		s.Replace("\r", "").Replace("\n)", ")").Split('\n').Select(x => x.Trim()).Where(x => x.Length > 0));

	static string Render(IlBlock b) => b.ToString();

	// ---- family sweep: every 2-op ALU template lowers at every width ----
	static (List<string> Params, List<PTree> Eval) Load(string mnem) {
		var src = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory,
			"../../../../XFusionGenerator/ia32-base.isa"));
		foreach(var elem in ListParser.Parse(src))
			if(elem is PList { Count: >= 4 } pl && pl[0] is PName("instruction")
				&& pl[1] is PName(var n) && n == mnem) {
				var ps = ((PList) pl[2]).Select(x => ((PName) x).Name).ToList();
				return (ps, pl.Skip(4).ToList());
			}
		throw new InvalidOperationException($"{mnem} not found");
	}

	static string LowerRegReg(string mnem, int w) {
		var (ps, eval) = Load(mnem);
		var binds = new Dictionary<string, OperandBind>();
		binds[ps[0]] = new OperandBind.Reg(0, w);
		if(ps.Count > 1) binds[ps[1]] = new OperandBind.Reg(3, w);
		return Render(IlLower.Lower(ps, eval, binds, w));
	}

	[Test]
	public void AluFamilySweep() {
		// every flag-writing 2-op template × every width lowers without throwing
		foreach(var mnem in new[] { "ADD", "OR", "ADC", "SBB", "AND", "SUB", "XOR", "CMP", "TEST" })
			foreach(var w in new[] { 8, 16, 32, 64 }) {
				var il = LowerRegReg(mnem, w);
				Assert.That(il, Does.Contain("EFLAGS.Z"), $"{mnem}/{w}");
				Assert.That(il, Does.Contain($"(u{w} "), $"{mnem}/{w}");
			}
	}

	[Test]
	public void XorConstFlags() {  // (= CF 0) → const u1 write
		var il = LowerRegReg("XOR", 32);
		Assert.That(il, Does.Contain("(EFLAGS.C := (u1 #0))"));
		Assert.That(il, Does.Contain("(EFLAGS.O := (u1 #0))"));
	}

	[Test]
	public void CmpStoresNothing() {  // CMP computes flags only — no reg/mem write
		var il = LowerRegReg("CMP", 32);
		Assert.That(il, Does.Not.Contain(":= (u64 zext"));  // no operand write-back
		Assert.That(il, Does.Not.Contain("(store"));
		Assert.That(il, Does.Contain("EFLAGS.C"));
	}

	[Test]
	public void MovBare() {  // MOV: single write, zero flags
		var il = LowerRegReg("MOV", 32);
		Assert.That(il, Does.Not.Contain("EFLAGS"));
		Assert.That(il, Does.Contain("(RAX := (u64 zext (u32 trunc (u64 RBX))))"));
	}

	[Test]
	public void IncPreservesCf() {  // INC's defining quirk: CF untouched
		var (ps, eval) = Load("INC");
		var il = Render(IlLower.Lower(ps, eval,
			new Dictionary<string, OperandBind> { ["lval"] = new OperandBind.Reg(0, 32) }, 32));
		Assert.That(il, Does.Not.Contain("EFLAGS.C :="));
		Assert.That(il, Does.Contain("EFLAGS.Z"));
	}

	[Test]
	public void SubBorrowCf() {  // SUB: CF = lval < rval (borrow), not the ADD carry form
		var il = LowerRegReg("SUB", 32);
		Assert.That(il, Does.Contain("(EFLAGS.C := (u1 ult (u32 %0) (u32 %1)))"));
	}

	[Test]
	public void PushExplicitRsp() {  // ·62: value FIRST (push rsp = old rsp), then SP-=, then store
		var (ps, eval) = Load("PUSH");
		var il = Render(IlLower.Lower(ps, eval,
			new Dictionary<string, OperandBind> { ["src"] = new OperandBind.Reg(1, 64) }, 64));
		var iVal = il.IndexOf("(let %0 = (u64 RCX))");
		var iAdj = il.IndexOf("(RSP := (u64 sub (u64 RSP) (u64 #8)))");
		var iSto = il.IndexOf("(store (u64 RSP) (u64 %0))");
		Assert.That(iVal, Is.GreaterThanOrEqualTo(0));
		Assert.That(iAdj, Is.GreaterThan(iVal));
		Assert.That(iSto, Is.GreaterThan(iAdj));
	}

	[Test]
	public void PopExplicitRsp() {  // ·62: load [RSP] then RSP+=, dest written from the tmp
		var (ps, eval) = Load("POP");
		var il = Render(IlLower.Lower(ps, eval,
			new Dictionary<string, OperandBind> { ["dst"] = new OperandBind.Reg(1, 64) }, 64));
		var iLoad = il.IndexOf("(let %0 = (u64 load (u64 RSP)))");
		var iAdj = il.IndexOf("(RSP := (u64 add (u64 RSP) (u64 #8)))");
		var iWr = il.IndexOf("(RCX := (u64 %0))");
		Assert.That(iLoad, Is.GreaterThanOrEqualTo(0));
		Assert.That(iAdj, Is.GreaterThan(iLoad));
		Assert.That(iWr, Is.GreaterThan(iAdj));
	}

	[Test]
	public void CmovIsIteNotBranch() {  // ·62: IlIfV (csel), no intra-insn control flow
		var (ps, eval) = Load("CMOVB");
		var il = Render(IlLower.Lower(ps, eval, new Dictionary<string, OperandBind> {
			["dst"] = new OperandBind.Reg(0, 32),
			["src"] = new OperandBind.Reg(3, 32),
		}, 32));
		Assert.That(il, Does.Contain("if (u1 EFLAGS.C)"));
		Assert.That(il, Does.Not.Contain("(if (u1 EFLAGS.C)\n"));
	}

	[Test]
	public void IntrinsicPassthrough() {  // ·62: IlIntrin(V0, name, positional args)
		var (ps, eval) = Load("BSF");
		var il = Render(IlLower.Lower(ps, eval, new Dictionary<string, OperandBind> {
			["dst"] = new OperandBind.Reg(0, 32),
			["src"] = new OperandBind.Reg(3, 32),
		}, 32));
		Assert.That(il, Does.Contain("(void intrin.bsf"));
		Assert.That(il, Does.Contain("(u32 trunc (u64 RAX))"));  // operands ride as dataflow args
	}

	[Test]
	public void Sub8BitInsertWrite() {  // 8-bit write = masked insert, not zext
		var il = LowerRegReg("SUB", 8);
		Assert.That(il, Does.Contain("(u64 and (u64 RAX) (u64 #ffffffffffffff00))"));
	}
}
