using System.IO.Compression;
using XFusionCensus;
using XFusionCpu;

namespace XFusionTests;

/// Replays native-oracle results (XFusionCensus `oracle --golden`, recorded on x86-64
/// silicon semantics via Rosetta 2) through decode -> lift -> X86Machine, so the lifted
/// semantics stay pinned on hosts that cannot run x86 code. Each line is one state for
/// one encoding (2 encodings x 3 states per def, CP2077 corpus plus synthetic cases).
/// Inputs are rebuilt from the recorded seeds with the oracle's own generator.
public class GoldenReplayTests {
	const ulong MemBase = 0x200000000UL;
	const int MemSize = 0x2000;

	static IEnumerable<string> Lines() {
		var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Golden", "oracle-golden.tsv.gz");
		using var gz = new GZipStream(File.OpenRead(path), CompressionMode.Decompress);
		using var rd = new StreamReader(gz);
		string line;
		while((line = rd.ReadLine()) != null) yield return line;
	}

	[Test]
	public void LiftedSemanticsMatchNativeGoldens() {
		var failures = new List<string>();
		var n = 0;
		foreach(var line in Lines()) {
			n++;
			var err = Replay(line);
			if(err != null && failures.Count < 20) failures.Add(err);
			else if(err != null) failures.Add(null);
		}
		Assert.That(n, Is.GreaterThan(3000), "golden file truncated?");
		Assert.That(failures, Is.Empty,
			$"{failures.Count} of {n} golden cases disagree:\n{string.Join("\n", failures.Where(f => f != null))}");
	}

	static ulong Hex(string s) => Convert.ToUInt64(s, 16);

	static string Replay(string line) {
		var f = line.Split('\t');
		var code = Convert.FromHexString(f[0]);
		var codeAddr = Hex(f[1]);
		var target = Hex(f[2]);
		var flagsIn = Hex(f[3]);
		var gprIn = f[4].Split(',').Select(Hex).ToArray();
		var ymmIn = new byte[16 * 32];
		OracleFill.FillRegs(new Random(int.Parse(f[5])), ymmIn);
		var mem = new byte[MemSize];
		OracleFill.Fill(new Random(int.Parse(f[6])), mem);
		if(f[7] != "-") {
			var p = f[7].Split(':');
			BitConverter.TryWriteBytes(mem.AsSpan((int) Hex(p[0])), Hex(p[1]));
		}
		var taken = f[8] == "1";
		var flagMask = Hex(f[9]);

		var m = new X86Machine { Mode = XMode.Bits64, Ip = codeAddr, Flags = flagsIn };
		Array.Copy(gprIn, m.Gpr, 16);
		for(var r = 0; r < 16; r++) {
			m.Xmm[r] = Half(ymmIn, r, 0);
			m.YmmHi[r] = Half(ymmIn, r, 1);
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
			if(a < MemBase || a + (ulong) (w / 8) > MemBase + MemSize) { oob ??= $"load @{a:x}"; return 0; }
			ulong v = 0;
			for(var i = 0; i < w / 8; i++) v |= (ulong) lmem[(int) (a - MemBase) + i] << (i * 8);
			return v;
		};
		m.StoreHook = (a, v, w) => {
			if(a < MemBase || a + (ulong) (w / 8) > MemBase + MemSize) { oob ??= $"store @{a:x}"; return true; }
			for(var i = 0; i < w / 8; i++) lmem[(int) (a - MemBase) + i] = (byte) (v >> (i * 8));
			return true;
		};
		try { m.Step(); }
		catch(Exception e) { return $"{f[0]}: IL threw {e.GetType().Name}: {e.Message}"; }
		if(oob != null) return $"{f[0]}: IL access outside the window ({oob})";

		var diffs = new List<string>();
		var wantIp = taken ? target : codeAddr + (ulong) code.Length;
		if(m.Ip != wantIp) diffs.Add($"rip {m.Ip:x} want {wantIp:x}");
		var gprOut = (ulong[]) gprIn.Clone();
		if(f[10] != "-")
			foreach(var kv in f[10].Split(',')) { var p = kv.Split('='); gprOut[int.Parse(p[0])] = Hex(p[1]); }
		for(var r = 0; r < 16; r++)
			if(m.Gpr[r] != gprOut[r]) diffs.Add($"gpr{r} {m.Gpr[r]:x} want {gprOut[r]:x}");
		if((m.Flags & flagMask) != Hex(f[11])) diffs.Add($"flags {m.Flags & flagMask:x} want {f[11]}");
		var ymmOut = new (UInt128 Hi, UInt128 Lo)[16];
		for(var r = 0; r < 16; r++) ymmOut[r] = (Half(ymmIn, r, 1), Half(ymmIn, r, 0));
		if(f[12] != "-")
			foreach(var kv in f[12].Split(',')) {
				var p = kv.Split('=');
				var hl = p[1].Split('_');
				ymmOut[int.Parse(p[0])] = (UInt128.Parse(hl[0], System.Globalization.NumberStyles.HexNumber),
					UInt128.Parse(hl[1], System.Globalization.NumberStyles.HexNumber));
			}
		for(var r = 0; r < 16; r++)
			if(m.Xmm[r] != ymmOut[r].Lo || m.YmmHi[r] != ymmOut[r].Hi)
				diffs.Add($"ymm{r} {m.YmmHi[r]:x}_{m.Xmm[r]:x} want {ymmOut[r].Hi:x}_{ymmOut[r].Lo:x}");
		if(f[13] != "-")
			foreach(var run in f[13].Split(';')) {
				var p = run.Split(':');
				var bytes = Convert.FromHexString(p[1]);
				bytes.CopyTo(mem, (int) Hex(p[0]));
			}
		for(var i = 0; i < MemSize; i++)
			if(lmem[i] != mem[i]) { diffs.Add($"mem+{i:x} {lmem[i]:x2} want {mem[i]:x2}"); break; }
		return diffs.Count == 0 ? null : $"{f[0]} ({Disassembler.Disassemble(code, codeAddr, XMode.Bits64).Text}): {string.Join("; ", diffs)}";
	}

	static UInt128 Half(byte[] ymm, int reg, int half) {
		UInt128 v = 0;
		for(var b = 15; b >= 0; b--) v = (v << 8) | ymm[reg * 32 + half * 16 + b];
		return v;
	}
}
