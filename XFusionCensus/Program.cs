using System.Diagnostics;
using System.Text;
using XFusionCpu;

namespace XFusionCensus;

/// Walls-ladder instrument (XF-2). Drives Intel XED over an ELF's .text as the
/// length/disasm oracle (linear sweep with XED lengths — we never desync on XED's
/// view), asks the generated XFusion disassembler the same bytes, reports:
///   - coverage: % of instructions / bytes XFusion decodes at all
///   - top blockers: opcode bytes ranked by how many corpus bytes they block
///   - MISMATCHES: both decoded but text/length differ (the dangerous class)
/// Usage: XFusionCensus <elf> <16|32|64> [maxInsns] [--mismatches N]
public class Program {
	public static int Main(string[] args) {
		if(args.Length < 2) { Console.Error.WriteLine("usage: XFusionCensus <elf> <16|32|64> [maxInsns] [--mismatches N]"); return 1; }
		var path = args[0];
		var mode = args[1] switch { "16" => XMode.Bits16, "32" => XMode.Bits32, _ => XMode.Bits64 };
		var maxInsns = args.Length > 2 && !args[2].StartsWith("--") ? long.Parse(args[2]) : long.MaxValue;
		var showMismatches = 10;
		for(var a = 2; a < args.Length; a++)
			if(args[a] == "--mismatches" && a + 1 < args.Length) showMismatches = int.Parse(args[a + 1]);

		var (text, vaddr) = ReadTextSection(path);
		Console.WriteLine($"{path}: .text {text.Length} bytes @ 0x{vaddr:x}, mode {mode}");

		// xed -ir decodes the ENTIRE file (headers included) — hand it just the .text bytes.
		var rawPath = Path.GetTempFileName();
		File.WriteAllBytes(rawPath, text);
		List<(long, int, string)> xedLines;
		try { xedLines = RunXed(rawPath, mode); } finally { File.Delete(rawPath); }
		Console.WriteLine($"XED decoded {xedLines.Count} instructions.");

		long insnsTotal = 0, insnsOk = 0, insnsMiss = 0, insnsMismatch = 0, insnsLenMismatch = 0;
		long bytesTotal = 0, bytesOk = 0;
		var missByOpcode = new Dictionary<string, (long Insns, long Bytes, string Example)>();
		var mismatchExamples = new List<string>();
		var lenMismatchExamples = new List<string>();

		foreach(var (offset, len, xedText) in xedLines) {
			if(insnsTotal >= maxInsns) break;
			if(offset < 0 || offset + len > text.Length) continue;  // outside .text
			insnsTotal++;
			bytesTotal += len;
			var span = text.AsSpan((int) offset, Math.Min(15 + 4, text.Length - (int) offset));
			var (ours, ourLen) = Disassembler.Disassemble(span, (ulong) offset, mode);  // base 0: XED sees raw bytes
			if(ours == null) {
				insnsMiss++;
				// blocker key: prefix-skipped first opcode byte(s) from XED's bytes
				var key = OpcodeKey(text.AsSpan((int) offset, len), mode);
				var hex = Convert.ToHexString(text.AsSpan((int) offset, len));
				if(!missByOpcode.TryGetValue(key, out var e)) missByOpcode[key] = (1, len, $"{hex} = {xedText}");
				else missByOpcode[key] = (e.Insns + 1, e.Bytes + len, e.Example);
				continue;
			}
			if(ourLen != len) {
				insnsLenMismatch++;
				if(lenMismatchExamples.Count < showMismatches)
					lenMismatchExamples.Add($"@+0x{offset:x}: XED len {len} '{xedText}' vs OURS len {ourLen} '{ours}'");
				continue;
			}
			if(!TextMatches(ours, xedText)) {
				insnsMismatch++;
				if(mismatchExamples.Count < showMismatches)
					mismatchExamples.Add($"@+0x{offset:x} {Convert.ToHexString(text.AsSpan((int) offset, len))}: XED '{xedText}' vs OURS '{ours}'");
				continue;
			}
			insnsOk++;
			bytesOk += len;
		}

		Console.WriteLine();
		Console.WriteLine($"insns: {insnsTotal} total | {insnsOk} ok ({100.0 * insnsOk / insnsTotal:F2}%) | {insnsMiss} undecoded | {insnsLenMismatch} LEN-MISMATCH | {insnsMismatch} TEXT-MISMATCH");
		Console.WriteLine($"bytes: {bytesTotal} total | {bytesOk} ok ({100.0 * bytesOk / bytesTotal:F2}%)");
		Console.WriteLine();
		Console.WriteLine("top blockers (by blocked insns):");
		foreach(var (key, (insns, bytes, example)) in missByOpcode.OrderByDescending(kv => kv.Value.Insns).Take(15))
			Console.WriteLine($"  {key,-12} {insns,8} insns {bytes,9} bytes   e.g. {Truncate(example, 70)}");
		if(lenMismatchExamples.Count > 0) {
			Console.WriteLine();
			Console.WriteLine($"LENGTH MISMATCHES ({insnsLenMismatch} total — decode bugs, fix first):");
			lenMismatchExamples.ForEach(x => Console.WriteLine("  " + x));
		}
		if(mismatchExamples.Count > 0) {
			Console.WriteLine();
			Console.WriteLine($"TEXT MISMATCHES ({insnsMismatch} total):");
			mismatchExamples.ForEach(x => Console.WriteLine("  " + x));
		}
		return 0;
	}

	static string Truncate(string s, int n) => s.Length <= n ? s : s[..n] + "…";

	/// Key = escape map + first opcode byte after prefixes (the walls-ladder unit).
	static string OpcodeKey(ReadOnlySpan<byte> insn, XMode mode) {
		var i = Decode.ScanPrefixes(insn, mode, out _);
		if(i >= insn.Length) return "prefix-only";
		var b = insn[i];
		if(b != 0x0F) return $"{b:X2}";
		if(i + 1 >= insn.Length) return "0F??";
		var b2 = insn[i + 1];
		if(b2 == 0x38 && i + 2 < insn.Length) return $"0F38{insn[i + 2]:X2}";
		if(b2 == 0x3A && i + 2 < insn.Length) return $"0F3A{insn[i + 2]:X2}";
		return $"0F{b2:X2}";
	}

	/// Normalize trivial spacing differences; otherwise exact.
	static bool TextMatches(string ours, string xed) {
		static string Norm(string s) => s.Replace(", ", ",").Replace("  ", " ").Trim().ToLowerInvariant();
		return Norm(ours) == Norm(xed);
	}

	// --- XED driver: xed -ir <file> dumps 'XDIS <addr>: ... <hex> <disasm>' lines ---
	static List<(long Offset, int Len, string Text)> RunXed(string path, XMode mode) {
		var xed = Environment.GetEnvironmentVariable("XED_PATH")
			?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "oracles/xed/obj/wkit/examples/obj/xed");
		var modeArg = mode switch { XMode.Bits16 => "-16", XMode.Bits32 => "-32", _ => "-64" };
		var psi = new ProcessStartInfo(xed, $"{modeArg} -ir {path}") {
			RedirectStandardOutput = true, RedirectStandardError = true
		};
		var proc = Process.Start(psi);
		var ret = new List<(long, int, string)>();
		string line;
		while((line = proc.StandardOutput.ReadLine()) != null) {
			// XDIS 41ad0: BINARY BASE 4883EC08 sub rsp, 0x8
			if(!line.StartsWith("XDIS ")) continue;
			var colon = line.IndexOf(':');
			if(colon < 0) continue;
			var addr = Convert.ToInt64(line[5..colon].Trim(), 16);
			var rest = line[(colon + 1)..].TrimStart();
			// fields: CATEGORY EXTENSION HEXBYTES TEXT...
			var parts = rest.Split(' ', 4, StringSplitOptions.RemoveEmptyEntries);
			if(parts.Length < 4) continue;
			var hex = parts[2];
			var textPart = parts[3].Trim();
			ret.Add((addr, hex.Length / 2, textPart));
		}
		proc.WaitForExit();
		return ret;
	}

	// --- minimal ELF .text extractor (64- and 32-bit little-endian) ---
	static (byte[] Text, ulong Vaddr) ReadTextSection(string path) {
		var data = File.ReadAllBytes(path);
		if(data[0] != 0x7F || data[1] != (byte) 'E') throw new InvalidDataException("not ELF");
		var is64 = data[4] == 2;
		ulong shoff; int shentsize, shnum, shstrndx;
		if(is64) {
			shoff = BitConverter.ToUInt64(data, 0x28);
			shentsize = BitConverter.ToUInt16(data, 0x3A);
			shnum = BitConverter.ToUInt16(data, 0x3C);
			shstrndx = BitConverter.ToUInt16(data, 0x3E);
		} else {
			shoff = BitConverter.ToUInt32(data, 0x20);
			shentsize = BitConverter.ToUInt16(data, 0x2E);
			shnum = BitConverter.ToUInt16(data, 0x30);
			shstrndx = BitConverter.ToUInt16(data, 0x32);
		}
		// section-name string table
		var strSh = (int) shoff + shstrndx * shentsize;
		var strOff = is64 ? BitConverter.ToUInt64(data, strSh + 0x18) : BitConverter.ToUInt32(data, strSh + 0x10);
		for(var s = 0; s < shnum; s++) {
			var sh = (int) shoff + s * shentsize;
			var nameOff = BitConverter.ToUInt32(data, sh);
			var name = ReadCStr(data, (int) strOff + (int) nameOff);
			if(name != ".text") continue;
			ulong addr, off, size;
			if(is64) {
				addr = BitConverter.ToUInt64(data, sh + 0x10);
				off = BitConverter.ToUInt64(data, sh + 0x18);
				size = BitConverter.ToUInt64(data, sh + 0x20);
			} else {
				addr = BitConverter.ToUInt32(data, sh + 0x0C);
				off = BitConverter.ToUInt32(data, sh + 0x10);
				size = BitConverter.ToUInt32(data, sh + 0x14);
			}
			return (data.AsSpan((int) off, (int) size).ToArray(), addr);
		}
		throw new InvalidDataException(".text not found");
	}

	static string ReadCStr(byte[] data, int off) {
		var end = off;
		while(data[end] != 0) end++;
		return Encoding.ASCII.GetString(data, off, end - off);
	}
}
