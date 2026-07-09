using System.Diagnostics;
using XFusionCpu;

namespace XFusionCensus;

/// Differential fuzz (the reject-path tier — corpus census only exercises
/// XED-valid instructions; this feeds both decoders hostile bytes).
/// Generator: random bytes, biased toward prefix-dense/boundary shapes.
/// Verdict classes:
///   BOTH-DECODE + same text/len  = ok
///   BOTH-DECODE + len differs    = LEN-DIVERGE (worst: desync class)
///   BOTH-DECODE + text differs   = TEXT-DIVERGE
///   XED-only                     = we-undecode (fine — coverage gap, not a bug)
///   OURS-only                    = MISDECODE (we accept what Intel rejects)
///   crash/exception              = CRASH (decoder must be total)
public static class Fuzz {
	public static int Run(string[] args) {
		var n = args.Length > 1 ? int.Parse(args[1]) : 100_000;
		var seed = args.Length > 2 ? int.Parse(args[2]) : 42;
		var mode = args.Length > 3 && args[3] == "32" ? XMode.Bits32 : XMode.Bits64;
		var rng = new Random(seed);
		Console.WriteLine($"fuzz: {n} cases, seed {seed}, mode {mode}");

		var cases = new List<byte[]>(n);
		for(var c = 0; c < n; c++) cases.Add(Gen(rng));

		// our side first — also catches crashes standalone
		var ours = new (string Text, int Len)[n];
		long crashes = 0;
		for(var c = 0; c < n; c++) {
			try { ours[c] = Disassembler.Disassemble(cases[c], 0, mode); }
			catch(Exception e) {
				if(crashes++ < 5)
					Console.WriteLine($"CRASH on {Convert.ToHexString(cases[c])}: {e.GetType().Name} {e.Message}");
				ours[c] = ("«CRASH»", 0);
			}
		}

		// XED side: batch via a temp file of one-line hex (xed -d per line is too slow;
		// use raw-file decode per case chunked into one process call each 8K cases
		// is still slow — simplest robust: feed each case through `xed -d` in batches
		// via stdin-less loop is too slow at 100K. Compromise: XED only on the cases
		// where WE decoded (misdecode check) + a 2K random sample of our-rejects
		// (mutual-reject sanity). That covers the dangerous classes.
		long okSame = 0, lenDiv = 0, textDiv = 0, misdecode = 0, weReject = 0;
		var xed = Environment.GetEnvironmentVariable("XED_PATH")
			?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "oracles/xed/obj/wkit/examples/obj/xed");
		var modeArg = mode == XMode.Bits32 ? "-32" : "-64";

		var toCheck = new List<int>();
		for(var c = 0; c < n; c++)
			if(ours[c].Text != null && ours[c].Text != "«CRASH»") toCheck.Add(c);
		var rejects = new List<int>();
		for(var c = 0; c < n; c++)
			if(ours[c].Text == null) rejects.Add(c);
		var rejectSample = rejects.OrderBy(_ => rng.Next()).Take(2000).ToList();
		Console.WriteLine($"we decode {toCheck.Count}, we reject {rejects.Count} (sampling 2000), crashes {crashes}");

		var diverges = new List<string>();
		foreach(var c in toCheck.Concat(rejectSample)) {
			var hex = Convert.ToHexString(cases[c]);
			var (xText, xLen) = XedOne(xed, modeArg, hex);
			var (oText, oLen) = ours[c];
			if(oText == null) {
				if(xText != null) weReject++;  // coverage gap, fine
				continue;
			}
			if(xText == null) {
				misdecode++;
				if(diverges.Count < 25) diverges.Add($"MISDECODE {hex}: OURS '{oText}' len {oLen}; XED rejects");
				continue;
			}
			if(oLen != xLen) {
				lenDiv++;
				diverges.Insert(0, $"LEN {hex}: OURS '{oText}' len {oLen} vs XED '{xText}' len {xLen}");  // LEN always shows, front
			} else if(Norm(oText) != Norm(xText)) {
				textDiv++;
				if(diverges.Count < 25) diverges.Add($"TEXT {hex}: OURS '{oText}' vs XED '{xText}'");
			} else okSame++;
		}

		Console.WriteLine();
		Console.WriteLine($"ok {okSame} | LEN-DIVERGE {lenDiv} | TEXT-DIVERGE {textDiv} | MISDECODE {misdecode} | we-reject-XED-accepts {weReject} (sampled) | CRASH {crashes}");
		diverges.ForEach(x => Console.WriteLine("  " + x));
		return crashes + lenDiv + misdecode > 0 ? 1 : 0;
	}

	static string Norm(string s) => s.Replace(", ", ",").Replace("  ", " ").Trim().ToLowerInvariant();

	/// One hostile case: random bytes, biased toward the shapes that stress decode —
	/// prefix runs, escape bytes, VEX/EVEX leads, boundary truncations.
	static byte[] Gen(Random rng) {
		var len = rng.Next(1, 18);  // 1..17 — includes >15-byte (must reject) and truncations
		var b = new byte[len];
		rng.NextBytes(b);
		switch(rng.Next(5)) {
			case 0:  // prefix-dense: overwrite lead with prefix soup
				for(var i = 0; i < Math.Min(len, rng.Next(1, 6)); i++)
					b[i] = new byte[] { 0x66, 0x67, 0xF0, 0xF2, 0xF3, 0x2E, 0x3E, 0x64, 0x65, 0x40, 0x48, 0x4F }[rng.Next(12)];
				break;
			case 1:  // escape-led
				b[0] = 0x0F;
				if(len > 1 && rng.Next(2) == 0) b[1] = rng.Next(2) == 0 ? (byte) 0x38 : (byte) 0x3A;
				break;
			case 2:  // VEX/EVEX-led
				b[0] = new byte[] { 0xC4, 0xC5, 0x62 }[rng.Next(3)];
				break;
			// 3,4: raw random
		}
		return b;
	}

	static string _fuzzTmp;

	/// XED oracle for ONE hostile byte string: raw-file -ir decode, take the FIRST
	/// XDIS line's hex-length + text. (`xed -d` demands the input be exactly one
	/// instruction — trailing garbage falsifies both verdict and length.)
	static (string, int) XedOne(string xed, string modeArg, string hex) {
		_fuzzTmp ??= Path.GetTempFileName();
		File.WriteAllBytes(_fuzzTmp, Convert.FromHexString(hex));
		var psi = new ProcessStartInfo(xed, $"{modeArg} -ir {_fuzzTmp}") {
			RedirectStandardOutput = true, RedirectStandardError = true
		};
		using var proc = Process.Start(psi);
		var stdout = proc.StandardOutput.ReadToEnd();
		proc.StandardError.ReadToEnd();
		proc.WaitForExit();
		foreach(var line in stdout.Split('\n')) {
			if(!line.StartsWith("XDIS ")) continue;
			var colon = line.IndexOf(':');
			var addr = Convert.ToInt64(line[5..colon].Trim(), 16);
			if(addr != 0) return (null, 0);  // first insn at 0 didn't decode
			var parts = line[(colon + 1)..].Split(' ', 4, StringSplitOptions.RemoveEmptyEntries);
			if(parts.Length < 4) return (null, 0);
			return (parts[3].Trim(), parts[2].Length / 2);
		}
		return (null, 0);
	}
}
