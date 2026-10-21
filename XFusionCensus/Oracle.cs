using System.Diagnostics;
using System.Runtime.InteropServices;
using Iced.Intel;
using LiftIl;
using XFusionCpu;

namespace XFusionCensus;

/// Semantics oracle: run each sampled instruction natively (oracle/xfrun, an x86-64
/// helper; on Apple silicon it executes under Rosetta 2) and through the lifter +
/// X86Machine from the same random architectural state, then diff GPRs, the
/// SDM-defined flags, xmm registers and a memory window.
///
/// Inputs are the census's per-def encoding samples (samples.txt from `pe --out`).
/// Memory operands are steered into the window by choosing base/index values (or, for
/// RIP-relative forms, by placing the instruction at window - disp - len), so every
/// access lands in bytes both sides model. Flow control, privileged/host-query
/// instructions and stubbed (intrinsic) lifts are skipped and counted, not diffed.
///
/// Usage: XFusionCensus oracle <samples.txt> [--cases N] [--filter substr] [--out dir] [--top N]
public static class Oracle {
	const ulong MemBase = 0x200000000UL;
	const int MemSize = 0x2000;
	const ulong StackTop = MemBase + 0x1800;
	const ulong CodeBase = 0x300000100UL;

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	unsafe struct NativeState {
		public fixed ulong Gpr[16];
		public ulong Rflags;
		public fixed byte Ymm[16 * 32];
		public uint Mxcsr;
		public uint Pad;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	unsafe struct NativeCase {
		public ulong CodeAddr;
		public byte CodeLen;
		public fixed byte Code[15];
		public NativeState St;
		public fixed byte Mem[MemSize];
	}

	sealed class Runner : IDisposable {
		readonly Process proc;
		readonly Stream stdin, stdout;
		public Runner(string exe) {
			proc = Process.Start(new ProcessStartInfo(exe) {
				RedirectStandardInput = true, RedirectStandardOutput = true, UseShellExecute = false
			});
			stdin = proc.StandardInput.BaseStream;
			stdout = proc.StandardOutput.BaseStream;
		}
		public unsafe (uint Signal, NativeState St, byte[] Mem) Run(in NativeCase c) {
			var buf = new byte[sizeof(NativeCase)];
			fixed(byte* p = buf) *(NativeCase*) p = c;
			stdin.Write(buf);
			stdin.Flush();
			var outBuf = new byte[4 + sizeof(NativeState) + MemSize];
			stdout.ReadExactly(outBuf);
			fixed(byte* p = outBuf) {
				var sig = *(uint*) p;
				var st = *(NativeState*) (p + 4);
				return (sig, st, outBuf.AsSpan(4 + sizeof(NativeState)).ToArray());
			}
		}
		public void Dispose() {
			try { stdin.Close(); proc.WaitForExit(2000); } catch { }
			if(!proc.HasExited) proc.Kill();
		}
	}

	/// Build oracle/xfrun for x86_64 next to the binary (once; rebuilt if sources are newer).
	static string EnsureHelper() {
		var src = FindSourceDir();
		var exe = Path.Combine(AppContext.BaseDirectory, "xfrun");
		var c = Path.Combine(src, "xfrun.c");
		var s = Path.Combine(src, "xfrun_tramp.S");
		if(File.Exists(exe) && File.GetLastWriteTimeUtc(exe) > File.GetLastWriteTimeUtc(c)
		   && File.GetLastWriteTimeUtc(exe) > File.GetLastWriteTimeUtc(s)) return exe;
		var p = Process.Start(new ProcessStartInfo("clang", $"-arch x86_64 -O2 -o \"{exe}\" \"{c}\" \"{s}\"") {
			RedirectStandardError = true
		});
		var err = p.StandardError.ReadToEnd();
		p.WaitForExit();
		if(p.ExitCode != 0) throw new Exception("building xfrun failed:\n" + err);
		return exe;
	}

	static string FindSourceDir() {
		for(var d = AppContext.BaseDirectory; d != null; d = Path.GetDirectoryName(d)) {
			var cand = Path.Combine(d, "XFusionCensus", "oracle");
			if(Directory.Exists(cand)) return cand;
			cand = Path.Combine(d, "oracle");
			if(File.Exists(Path.Combine(cand, "xfrun.c"))) return cand;
		}
		throw new DirectoryNotFoundException("XFusionCensus/oracle not found upward from the binary");
	}

	class DefStats {
		public long Cases, Pass, Fail;
		public readonly Dictionary<string, long> Skips = new();
		public readonly List<string> Examples = new();
		public long Hits;
	}

	public static int Run(string[] args) {
		var samplesPath = args[1];
		var cases = 24;
		string filter = null, outDir = null;
		var top = 40;
		for(var a = 2; a < args.Length - 1; a++)
			switch(args[a]) {
				case "--cases": cases = int.Parse(args[++a]); break;
				case "--filter": filter = args[++a]; break;
				case "--out": outDir = args[++a]; break;
				case "--top": top = int.Parse(args[++a]); break;
			}
		var samples = File.ReadAllLines(samplesPath)
			.Select(l => l.Split('\t'))
			.Where(p => p.Length == 3 && (filter == null || p[0].Contains(filter, StringComparison.OrdinalIgnoreCase)))
			.Select(p => (Def: p[0], Hits: long.Parse(p[1]), Hex: p[2]))
			.ToList();
		Console.WriteLine($"oracle: {samples.Count} encodings x {cases} states");

		using var runner = new Runner(EnsureHelper());
		var stats = new Dictionary<string, DefStats>();
		foreach(var (def, hits, hex) in samples) {
			if(!stats.TryGetValue(def, out var ds)) stats[def] = ds = new DefStats { Hits = hits };
			var code = Convert.FromHexString(hex);
			var rng = new Random(hex.GetHashCode() ^ 0x5eed);
			for(var k = 0; k < cases; k++) {
				var (verdict, detail) = OneCase(runner, code, rng);
				if(verdict == null) { ds.Cases++; ds.Pass++; continue; }
				if(verdict == "FAIL") {
					ds.Cases++; ds.Fail++;
					if(ds.Examples.Count < 3) ds.Examples.Add($"{hex}: {detail}");
					continue;
				}
				ds.Skips[verdict] = ds.Skips.GetValueOrDefault(verdict) + 1;
				break;  // a skip reason is a property of the encoding, not the state
			}
		}

		long tc = stats.Values.Sum(s => s.Cases), tp = stats.Values.Sum(s => s.Pass), tf = stats.Values.Sum(s => s.Fail);
		var failing = stats.Where(s => s.Value.Fail > 0).OrderByDescending(s => s.Value.Hits).ToList();
		Console.WriteLine($"cases {tc} | pass {tp} | FAIL {tf} | defs tested {stats.Count(s => s.Value.Cases > 0)} | defs failing {failing.Count}");
		var skipTotals = stats.Values.SelectMany(s => s.Skips).GroupBy(x => x.Key)
			.Select(g => (g.Key, g.Sum(x => x.Value))).OrderByDescending(x => x.Item2);
		Console.WriteLine("skipped encodings: " + string.Join(", ", skipTotals.Select(x => $"{x.Key} {x.Item2}")));
		Console.WriteLine();
		Console.WriteLine("FAILING DEFS (by corpus frequency):");
		foreach(var (def, s) in failing.Take(top)) {
			Console.WriteLine($"  {def,-28} corpus {s.Hits,8} | fail {s.Fail}/{s.Cases}");
			Console.WriteLine($"      {Trunc(s.Examples[0], 260)}");
		}
		if(outDir != null) {
			Directory.CreateDirectory(outDir);
			File.WriteAllLines(Path.Combine(outDir, "oracle.txt"), stats.OrderByDescending(s => s.Value.Hits).Select(s =>
				$"{s.Key}\tcorpus {s.Value.Hits}\tcases {s.Value.Cases}\tpass {s.Value.Pass}\tfail {s.Value.Fail}\t"
				+ string.Join(",", s.Value.Skips.Select(x => $"skip:{x.Key}"))
				+ string.Concat(s.Value.Examples.Select(e => "\n\t" + e))));
		}
		return tf > 0 ? 1 : 0;
	}

	static string Trunc(string s, int n) => s.Length <= n ? s : s[..n] + "…";

	static readonly HashSet<Mnemonic> HostQuery = [
		Mnemonic.Cpuid, Mnemonic.Rdtsc, Mnemonic.Rdtscp, Mnemonic.Xgetbv, Mnemonic.Rdsspq, Mnemonic.Rdsspd,
		Mnemonic.Rdrand, Mnemonic.Rdseed, Mnemonic.Rdpid, Mnemonic.Pause, Mnemonic.Hlt, Mnemonic.Umonitor,
		Mnemonic.Umwait, Mnemonic.Xlatb, Mnemonic.Ldmxcsr, Mnemonic.Vldmxcsr, Mnemonic.Stmxcsr, Mnemonic.Vstmxcsr,
		Mnemonic.Fxsave, Mnemonic.Fxsave64, Mnemonic.Fxrstor, Mnemonic.Fxrstor64, Mnemonic.Xsave, Mnemonic.Xsave64,
		Mnemonic.Xrstor, Mnemonic.Xrstor64, Mnemonic.Xsavec, Mnemonic.Xsavec64, Mnemonic.Xsaveopt, Mnemonic.Xsaveopt64,
		Mnemonic.Int, Mnemonic.Int1, Mnemonic.Int3, Mnemonic.Into, Mnemonic.Ud2, Mnemonic.Syscall,
	];

	/// One random state through both sides. Returns (null, null) on agreement,
	/// ("FAIL", detail) on disagreement, or (skip-reason, null).
	static unsafe (string, string) OneCase(Runner runner, byte[] code, Random rng) {
		if(!Disassembler.DecodeInsn(code, XMode.Bits64, out var d)) return ("undecoded", null);
		if(d.Len != code.Length) return ("len", null);
		var codeAddr = CodeBase;
		var memOff = 0x200 + rng.Next(0x18) * 0x40;  // 64-aligned target inside the window

		// RIP-relative: move the instruction so rip+disp lands at the target.
		if(d.M.RipRelative && !d.M.IsReg && HasModRmMem(in d)) {
			codeAddr = unchecked(MemBase + (ulong) memOff - (ulong) d.M.Disp - (ulong) d.Len);
			var page = codeAddr & ~0xFFFUL;
			if(page < 0x10000 || (page < MemBase + MemSize && page + 0x2000 > MemBase)) return ("rip-overlap", null);
		}

		var icedDec = Iced.Intel.Decoder.Create(64, new ByteArrayCodeReader(code));
		icedDec.IP = codeAddr;
		var ins = icedDec.Decode();
		if(ins.IsInvalid) return ("iced-invalid", null);
		if(ins.FlowControl is not (FlowControl.Next)) return ("flow", null);
		if(HostQuery.Contains(ins.Mnemonic)) return ("host-query", null);
		if(ins.SegmentPrefix is Register.FS or Register.GS) return ("fs/gs", null);
		if(d.P.AdSize) return ("addr32", null);
		if(LiftTables.Defs[d.DefId].Item2.Split(' ').Any(x => x.StartsWith('O'))) return ("abs-addr", null);
		for(var i = 0; i < ins.OpCount; i++)
			if(ins.GetOpKind(i) == OpKind.Register && ins.GetOpRegister(i).IsST()) return ("x87", null);
		if(ins.Encoding == EncodingKind.Legacy && IsX87(ins)) return ("x87", null);
		if(ins.Mnemonic == Mnemonic.Enter) return ("enter", null);
		if(ins.Mnemonic is Mnemonic.Popf or Mnemonic.Popfq) return ("popf", null);  // random TF/AC would trap

		IlBlock block;
		try { block = X86Lifter.Lift(in d, codeAddr, XMode.Bits64); }
		catch(Exception e) { return ("lift-throws", e.Message); }
		if(HasIntrin(block, out var intrName) && !MachineIntrinsics.Contains(intrName)) return ("stub", null);
		var ilUndefFlags = UndefinedFlagBits(block);

		// ---- state ----
		var st = new NativeState();
		for(var r = 0; r < 16; r++) st.Gpr[r] = RandGpr(rng);
		st.Gpr[4] = StackTop - (ulong) rng.Next(0x20) * 8;
		if(ins.Mnemonic == Mnemonic.Leave) st.Gpr[5] = StackTop - 0x40 - (ulong) rng.Next(0x10) * 8;
		if(ins.IsStringInstruction) {
			st.Gpr[6] = MemBase + 0x400 + (ulong) rng.Next(0x40);
			st.Gpr[7] = MemBase + 0xC00 + (ulong) rng.Next(0x40);
			st.Gpr[1] = (ulong) rng.Next(0, 17);
		}
		var flagBits = new[] { 0, 2, 4, 6, 7, 11 };
		st.Rflags = 0x202;
		foreach(var fb in flagBits) if(rng.Next(2) == 1) st.Rflags |= 1UL << fb;
		if(ins.IsStringInstruction && rng.Next(4) == 0) st.Rflags |= 1UL << 10;  // DF
		for(var r = 0; r < 16; r++) {
			var lanes = new byte[32];
			FillVector(rng, lanes);
			for(var b = 0; b < 32; b++) st.Ymm[r * 32 + b] = lanes[b];
		}
		st.Mxcsr = 0x1F80;
		var mem = new byte[MemSize];
		FillVector(rng, mem);

		// Steer a ModRM memory operand into the window.
		if(HasModRmMem(in d) && !d.M.RipRelative) {
			var target = MemBase + (ulong) memOff;
			var b = d.M.BaseReg; var x = d.M.IndexReg;
			if(b < 0 && x < 0) return ("abs-addr", null);
			if(b == x) return ("base==index", null);
			if(b == 4 && ins.IsStackInstruction) return ("rsp-base-stackop", null);
			ulong idxVal = 0;
			if(x >= 0) {
				idxVal = (ulong) rng.Next(0, 8);
				st.Gpr[x] = idxVal;
			}
			if(b >= 0) st.Gpr[b] = unchecked(target - (ulong) d.M.Disp - idxVal * d.M.Scale);
			else {
				var need = unchecked(target - (ulong) d.M.Disp);
				if(need % d.M.Scale != 0) return ("index-align", null);
				st.Gpr[x] = need / d.M.Scale;
			}
		}

		var nc = new NativeCase { CodeAddr = codeAddr, CodeLen = (byte) code.Length, St = st };
		for(var i = 0; i < code.Length; i++) nc.Code[i] = code[i];
		for(var i = 0; i < MemSize; i++) nc.Mem[i] = mem[i];
		var (sig, nst, nmem) = runner.Run(in nc);

		// ---- lifted side ----
		var m = new X86Machine { Mode = XMode.Bits64, Ip = codeAddr, Flags = st.Rflags };
		for(var r = 0; r < 16; r++) m.Gpr[r] = st.Gpr[r];
		for(var r = 0; r < 16; r++) {
			m.Xmm[r] = Half(st.Ymm, r, 0);
			m.YmmHi[r] = Half(st.Ymm, r, 1);
		}
		var lmem = (byte[]) mem.Clone();
		string oob = null;
		m.FetchHook = (addr, into) => {
			for(var i = 0; i < into.Length; i++) {
				var o = (long) (addr - codeAddr) + i;
				into[i] = o >= 0 && o < code.Length ? code[o] : (byte) 0x90;
			}
			return true;
		};
		m.LoadHook = (a, w) => {
			if(a < MemBase || a + (ulong) (w / 8) > MemBase + MemSize) { oob ??= $"load @{a:x}/{w}"; return 0; }
			ulong v = 0;
			for(var i = 0; i < w / 8; i++) v |= (ulong) lmem[(int) (a - MemBase) + i] << (i * 8);
			return v;
		};
		m.StoreHook = (a, v, w) => {
			if(a < MemBase || a + (ulong) (w / 8) > MemBase + MemSize) { oob ??= $"store @{a:x}/{w}"; return true; }
			for(var i = 0; i < w / 8; i++) lmem[(int) (a - MemBase) + i] = (byte) (v >> (i * 8));
			return true;
		};
		string ilErr = null;
		var ilFault = false;
		try { m.Step(); }
		catch(DivideByZeroException) { ilFault = true; }
		catch(Exception e) { ilErr = e.GetType().Name + ": " + e.Message; }

		if(sig != 0) {
			if(sig == 8 && ilFault) return (null, null);  // #DE on both sides
			// Rosetta 2 raises SIGILL for some valid encodings it does not translate
			// (SAHF with stray prefixes). Nothing to grade against.
			if(sig == 4 && !ilFault && ilErr == null) return ("native-sigill", null);
			return ("FAIL", $"native signal {sig}, IL {(ilFault ? "#DE" : ilErr ?? "completed")}");
		}
		if(ilFault) return ("FAIL", "IL raised #DE, native completed");
		if(ilErr != null) return ilErr.Contains("intrinsic", StringComparison.OrdinalIgnoreCase) ? ("stub", null) : ("FAIL", "IL exec: " + ilErr);
		if(oob != null) return ("FAIL", "IL memory access outside window: " + oob);

		var diffs = new List<string>();
		string[] gn = ["rax", "rcx", "rdx", "rbx", "rsp", "rbp", "rsi", "rdi", "r8", "r9", "r10", "r11", "r12", "r13", "r14", "r15"];
		// Rosetta 2 zero-extends RAX on a SUCCESSFUL 32-bit CMPXCHG; the SDM's operation
		// only writes the accumulator on a mismatch, and the lift follows the SDM. Unverified
		// against silicon here, so it is excluded rather than graded either way.
		var skipRax = ins.Mnemonic is Mnemonic.Cmpxchg && ins.MemorySize is MemorySize.UInt32
			&& (nst.Rflags & (1UL << 6)) != 0;
		// Rosetta 2 sign-extends MOVSXD r32, r/m32 (63 /r without REX.W) into all 64 bits;
		// the SDM's 32-bit destination zero-extends, as the lift does.
		var skipMovsxd = ins.Mnemonic is Mnemonic.Movsxd && ins.Op0Register.GetSize() == 4;
		for(var r = 0; r < 16; r++)
			if(m.Gpr[r] != nst.Gpr[r] && !(r == 0 && skipRax)
			   && !(skipMovsxd && (uint) m.Gpr[r] == (uint) nst.Gpr[r])) diffs.Add($"{gn[r]} il={m.Gpr[r]:x} hw={nst.Gpr[r]:x} (in {st.Gpr[r]:x})");
		var mask = (1UL << 0) | (1UL << 2) | (1UL << 4) | (1UL << 6) | (1UL << 7) | (1UL << 10) | (1UL << 11);
		mask &= ~IcedUndefined(ins.RflagsUndefined) & ~ilUndefFlags;
		// Rosetta 2 computes CMPXCHG's flags as (dest - accumulator); the SDM (and
		// silicon, per the atomics fleet's battery) is (accumulator - dest). Only ZF
		// agrees between the two, so it is the only flag this oracle can grade.
		if(ins.Mnemonic is Mnemonic.Cmpxchg) mask &= 1UL << 6;
		// Rosetta 2 leaves OF/AF/PF/SF unchanged on PTEST; the SDM clears them.
		if(ins.Mnemonic is Mnemonic.Ptest or Mnemonic.Vptest) mask &= (1UL << 0) | (1UL << 6);
		if(((m.Flags ^ nst.Rflags) & mask) != 0)
			diffs.Add($"flags il={FlagStr(m.Flags & mask)} hw={FlagStr(nst.Rflags & mask)} (in {FlagStr(st.Rflags)})");
		for(var r = 0; r < 16; r++) {
			var (hwLo, hwHi) = (Half(nst.Ymm, r, 0), Half(nst.Ymm, r, 1));
			if(hwLo != m.Xmm[r] || hwHi != m.YmmHi[r])
				diffs.Add(hwHi == m.YmmHi[r]
					? $"xmm{r} il={m.Xmm[r]:x32} hw={hwLo:x32} (in {Half(st.Ymm, r, 0):x32})"
					: $"ymm{r} il={m.YmmHi[r]:x32}_{m.Xmm[r]:x32} hw={hwHi:x32}_{hwLo:x32} (in {Half(st.Ymm, r, 1):x32}_{Half(st.Ymm, r, 0):x32})");
		}
		for(var i = 0; i < MemSize; i++)
			if(lmem[i] != nmem[i]) {
				var end = i;
				while(end < MemSize && end < i + 16 && lmem[end] != nmem[end]) end++;
				diffs.Add($"mem+{i:x} il={Convert.ToHexString(lmem, i, end - i)} hw={Convert.ToHexString(nmem, i, end - i)}");
				break;
			}
		if(diffs.Count == 0) return (null, null);
		return ("FAIL", $"{ins} :: {string.Join("; ", diffs)}");
	}

	static unsafe UInt128 Half(byte* ymm, int reg, int half) {
		UInt128 v = 0;
		for(var bb = 15; bb >= 0; bb--) v = (v << 8) | ymm[reg * 32 + half * 16 + bb];
		return v;
	}

	static bool HasModRmMem(in DecodedInsn d) {
		var specs = LiftTables.Defs[d.DefId].Item2;
		if(specs.Length == 0 || d.M.IsReg) return false;
		foreach(var s in specs.Split(' ')) {
			var o = XFusionGenerator.OperandSpec.Parse(s);
			if(o.Class is XFusionGenerator.OpClass.ModRmRm or XFusionGenerator.OpClass.XmmRm or XFusionGenerator.OpClass.MmxRm) return true;
		}
		return false;
	}

	static bool IsX87(in Instruction ins) {
		var op = ins.OpCode;
		return op.OpCode is >= 0xD8 and <= 0xDF && op.Table == OpCodeTableKind.Normal || ins.Mnemonic is Mnemonic.Wait;
	}

	// Intrinsics X86Machine executes natively (so the IL is diffable even though opaque).
	static readonly HashSet<string> MachineIntrinsics = [
		"idiv-wide", "movs", "stos", "lods", "scas", "cmps",
		"rep_movs", "rep_stos", "rep_lods", "repe_scas", "repne_scas", "repe_cmps", "repne_cmps", "fence",
	];

	static bool HasIntrin(IlBlock b, out string name) {
		foreach(var s in b.Body)
			if(s is IlIntrin(_, var n, _)) { name = n; return true; }
		name = null;
		return false;
	}

	static ulong UndefinedFlagBits(IlBlock b) {
		ulong m = 0;
		void Walk(IEnumerable<Il> body) {
			foreach(var s in body)
				switch(s) {
					case IlWriteReg(RegKind.Eflags, var bit, IlUndefined) when bit >= 0: m |= 1UL << bit; break;
					case IlIf(_, var t, var e): Walk(t); Walk(e); break;
				}
		}
		Walk(b.Body);
		return m;
	}

	static ulong IcedUndefined(RflagsBits f) {
		ulong m = 0;
		if((f & RflagsBits.OF) != 0) m |= 1UL << 11;
		if((f & RflagsBits.SF) != 0) m |= 1UL << 7;
		if((f & RflagsBits.ZF) != 0) m |= 1UL << 6;
		if((f & RflagsBits.AF) != 0) m |= 1UL << 4;
		if((f & RflagsBits.CF) != 0) m |= 1UL << 0;
		if((f & RflagsBits.PF) != 0) m |= 1UL << 2;
		return m;
	}

	static string FlagStr(ulong f) {
		var s = "";
		foreach(var (bit, c) in new[] { (11, 'O'), (10, 'D'), (7, 'S'), (6, 'Z'), (4, 'A'), (2, 'P'), (0, 'C') })
			s += (f >> bit & 1) != 0 ? c : '.';
		return s;
	}

	static ulong RandGpr(Random rng) => rng.Next(8) switch {
		0 => 0,
		1 => ulong.MaxValue,
		2 => (ulong) rng.Next(0, 256),
		3 => 0x8000000000000000UL >> rng.Next(0, 64),
		4 => (ulong) (uint) rng.Next() | (rng.Next(2) == 0 ? 0 : 0x80000000UL),
		_ => (ulong) rng.NextInt64() ^ ((ulong) rng.NextInt64() << 1),
	};

	/// Vector/memory fill: a mix of raw bits and plausible float lanes, so float ops
	/// see ordinary values (raw bits alone are NaN/denormal-heavy and mostly test those).
	static void FillVector(Random rng, byte[] buf) {
		var mode = rng.Next(4);
		for(var o = 0; o + 8 <= buf.Length; o += 8) {
			ulong v = mode switch {
				0 => (ulong) rng.NextInt64() ^ ((ulong) rng.NextInt64() << 1),
				1 => (ulong) BitConverter.SingleToUInt32Bits(RandFloat(rng))
				     | ((ulong) BitConverter.SingleToUInt32Bits(RandFloat(rng)) << 32),
				2 => BitConverter.DoubleToUInt64Bits(RandFloat(rng) * (double) RandFloat(rng)),
				_ => (ulong) rng.Next(-300, 300) & 0xFFFF | ((ulong) rng.Next(-300, 300) & 0xFFFF) << 16
				     | ((ulong) rng.Next(0, 70000)) << 32,
			};
			BitConverter.TryWriteBytes(buf.AsSpan(o), v);
		}
	}

	static float RandFloat(Random rng) => rng.Next(10) switch {
		0 => 0f,
		1 => rng.Next(2) == 0 ? 1f : -1f,
		2 => rng.Next(-1000, 1000),
		_ => (float) ((rng.NextDouble() * 2 - 1) * Math.Pow(10, rng.Next(-4, 5))),
	};
}
