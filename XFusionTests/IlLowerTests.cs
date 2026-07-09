using CoreArchCompiler;
using XFusionGenerator;

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
			["lval"] = new OperandBind.Reg("X86_RAX", 32),
			["rval"] = new OperandBind.Reg("X86_RBX", 32),
		}, 32);
		var got = IlLower.Render(stmts);
		var expect = """
(block
  (let %0 = (u32 trunc (u64 X86_RAX)))
  (let %1 = (u32 trunc (u64 X86_RBX)))
  (let %2 = (u32 add (u32 %0) (u32 %1)))
  (X86_RAX := (u64 zext (u32 %2)))
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
		var addr = new Ilx.Bin(64, "add", new Ilx.ReadReg("X86_RBP"), new Ilx.Const(64, -16));
		var stmts = IlLower.Lower(ps, eval, new Dictionary<string, OperandBind> {
			["lval"] = new OperandBind.Mem(addr, 32),
			["rval"] = new OperandBind.Reg("X86_RBX", 32),
		}, 32);
		var got = IlLower.Render(stmts);
		// structural assertions per golden row 2:
		Assert.That(got, Does.Contain("(let %addr = (u64 add (u64 X86_RBP) (u64 #fffffffffffffff0)))"));
		Assert.That(got, Does.Contain("(u32 load (u64 %addr))"));           // lval reads via the SHARED addr
		Assert.That(got, Does.Contain("(store (u64 %addr)"));               // write-back to SAME addr
		Assert.That(got.IndexOf("(store"), Is.GreaterThan(got.IndexOf("load")));
		// exactly one addr computation
		Assert.That(got.Split("%addr =").Length, Is.EqualTo(2));
		// flag block identical shape to row 1 (spot: CF + Z)
		Assert.That(got, Does.Contain("(EFLAGS.C := (u1 or (u1 ult"));
		Assert.That(got, Does.Contain("(EFLAGS.Z := (u1 eq"));
	}

	static string Norm(string s) => string.Join('\n',
		s.Replace("\r", "").Split('\n').Select(x => x.TrimEnd()).Where(x => x.Length > 0));
}
