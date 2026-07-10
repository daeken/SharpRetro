using XFusionCpu;
using XFusionJit;

namespace XFusionTests;

/// XF-6 acceptance: the backends oracle each other. Same bytes + start state,
/// X86Machine (interpret) vs CilJit-emitted (recompile) → GPR/Flags/Rip/mem
/// must agree. Divergence = an emitter bug (X86Machine is exec-verified vs
/// SDM goldens; the emitter walks the SAME tree — disagreement isolates
/// exactly one node's Eval-vs-Emit).
[TestFixture]
public unsafe class RecompileTests {
	const int MEM = 0x20000;

	static void Diff(string hex, Action<ulong[]> setup = null, ulong pc = 0x1000) {
		var code = Convert.FromHexString(hex);
		// interpreter
		var m = new X86Machine { Mode = XMode.Bits64, Mem = new byte[MEM], Ip = pc };
		code.CopyTo(m.Mem, (int) pc);
		setup?.Invoke(m.Gpr);
		while(m.Ip >= pc && m.Ip < pc + (ulong) code.Length) {
			if(!m.Step()) break;
			if(m.Halted) break;
		}
		// recompiler
		var st = default(X86State);
		var mem = new byte[MEM];
		code.CopyTo(mem, (int) pc);
		var gs = new ulong[16];
		setup?.Invoke(gs);
		for(var i = 0; i < 16; i++) st.Gpr[i] = gs[i];
		st.Rip = pc;
		st.Flags = 2;
		var fn = TestRecompiler.Compile(code, pc, XMode.Bits64, mem);
		fn(&st);
		// re-enter until Rip leaves the code range OR steady-state (v0: single-block
		// compile per re-entry — block-cache is a later perf step)
		var reEnters = 0;
		while(st.Rip >= pc && st.Rip < pc + (ulong) code.Length && reEnters++ < 200) {
			var off = (int) (st.Rip - pc);
			var f2 = TestRecompiler.Compile(code[off..], st.Rip, XMode.Bits64, mem);
			f2(&st);
		}
		// diff
		for(var i = 0; i < 16; i++)
			Assert.That(st.Gpr[i], Is.EqualTo(m.Gpr[i]), $"Gpr[{i}]");
		Assert.That(st.Flags, Is.EqualTo(m.Flags), "Flags");
		Assert.That(st.Rip, Is.EqualTo(m.Ip), "Rip");
		Assert.That(mem[..MEM].SequenceEqual(m.Mem), Is.True, "mem");
	}

	// --- the ExecTests ALU/flags corpus, through the emitter ---
	[Test] public void AddCf() => Diff("01d8", g => { g[0] = 0xFFFFFFFF; g[3] = 1; });
	[Test] public void AddOf() => Diff("01d8", g => { g[0] = 0x7FFFFFFF; g[3] = 1; });
	[Test] public void SubBorrow() => Diff("29d8", g => { g[0] = 1; g[3] = 2; });
	[Test] public void PushPop() => Diff("55" + "59", g => { g[5] = 0xDEADBEEFCAFE; g[4] = 0x8000; });
	[Test] public void CallRet() => Diff("e803000000" + "909090" + "c3", g => { g[4] = 0x8000; });
	[Test] public void MemRmw() => Diff("015df0", g => { g[5] = 0x9000; g[3] = 7; });
	[Test] public void ShlBy1() => Diff("c1e001", g => { g[0] = 0x80000000; });
	[Test] public void ShlBy0() => Diff("c1e000", g => { g[0] = 5; });
	[Test] public void JzTaken() => Diff("31c0" + "7402" + "9090");
	[Test] public void JzNotTaken() => Diff("83c801" + "7402" + "90");
	[Test] public void Cmov() => Diff("39d8" + "0f44ca", g => { g[0] = 5; g[3] = 5; g[1] = 111; g[2] = 222; });
	[Test] public void CmovNot() => Diff("39d8" + "0f44ca", g => { g[0] = 5; g[3] = 6; g[1] = 111; g[2] = 222; });

	// --- fuzz-tier: random start regs × the corpus (fixed-value goldens don't
	// exercise width/mask/sign-extend edges — n=2 on this lesson at day-3) ---
	static readonly string[] FuzzCorpus = [
		"01d8", "29d8", "31c0", "09d8", "21d8", "39d8",       // ALU Ev,Gv
		"83c005", "83e805", "83f005", "83f805",               // ALU Ev,Ib-sx
		"c1e003", "c1e803", "c1f803", "d1e0", "d1e8",         // shifts (by imm, by 1)
		"0f44ca", "0f45ca", "0f4cca",                         // cmov z/nz/l
		"ffc0", "ffc8",                                       // inc/dec
		"f7d0", "f7d8",                                       // not/neg
		"8bc3", "89d8", "b8efbeadde",                         // mov reg/reg, mov reg imm
		"55", "5d",                                           // push/pop (needs valid RSP)
	];

	[Test]
	public void FuzzInterpVsRecompile([Range(0, 4)] int seed) {
		var rnd = new Random(seed);
		var fails = new List<string>();
		foreach(var hex in FuzzCorpus) {
			for(var trial = 0; trial < 8; trial++) {
				var g0 = new ulong[16];
				for(var i = 0; i < 8; i++) g0[i] = (ulong) rnd.NextInt64();
				g0[4] = 0x8000 + (ulong) (rnd.Next(0x1000) & ~7);  // RSP: valid + aligned
				try {
					Diff(hex, g => Array.Copy(g0, g, 16));
				} catch(AssertionException e) {
					fails.Add($"{hex} seed={seed} trial={trial} regs=[{string.Join(",", g0.Take(8).Select(x => x.ToString("x")))}]: {e.Message.Split('\n')[0]}");
				} catch(Exception e) {
					fails.Add($"{hex} seed={seed} trial={trial}: THROW {e.GetType().Name}: {e.Message}");
				}
			}
		}
		Assert.That(fails, Is.Empty, string.Join("\n", fails.Take(10)));
	}
}
