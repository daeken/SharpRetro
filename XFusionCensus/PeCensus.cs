using System.Text.Json;
using Iced.Intel;
using LiftIl;
using XFusionCpu;

namespace XFusionCensus;

/// PE64 census: the walls-ladder over a Windows x64 image (Cyberpunk 2077 is the
/// driving corpus). The ELF census linear-sweeps .text with XED lengths; a PE's
/// .text interleaves jump tables and padding with code, so a linear sweep over it
/// decodes data (the first PE pass "found" out/outsb/iretd in a game binary). This
/// arm instead does recursive descent from known-code roots:
///   - every .pdata RUNTIME_FUNCTION start (incl. chained fragments -- still code)
///   - optional symbol-address file (CDPR's cyberpunk2077_addresses.json shape:
///     {"Addresses":[{"offset":"0001:XXXXXXXX"},...]} -- section 1 = .text)
///   - direct call/jmp/jcc targets discovered on the way
/// with iced as the in-process length/flow oracle (XED is not needed here).
/// Indirect branches end a path; jump-table targets are recovered only to the extent
/// another root reaches them -- the .pdata-coverage line says how much that leaves.
///
/// Per instruction: does OUR decoder accept it, agree on length, agree on mnemonic
/// (modulo condition-code spellings), lift without throwing, and lift to real IL
/// rather than an intrin/unimpl stub. Each failing class is ranked by occurrence.
/// Usage: XFusionCensus pe <exe> [--symbols file.json] [--out dir] [--top N]
public static class PeCensus {
	public static int Run(string[] args) {
		var path = args[1];
		string symbols = null, outDir = null;
		var top = 25;
		for(var a = 2; a < args.Length - 1; a++)
			switch(args[a]) {
				case "--symbols": symbols = args[++a]; break;
				case "--out": outDir = args[++a]; break;
				case "--top": top = int.Parse(args[++a]); break;
			}

		var img = new PeImage(File.ReadAllBytes(path));
		var (textVa, textSize) = img.Text;
		Console.WriteLine($"{path}: base 0x{img.Base:x}, .text rva 0x{textVa:x} size 0x{textSize:x}, .pdata {img.Pdata.Count} entries");

		var roots = new List<uint>(img.Pdata.Select(p => p.Begin));
		if(symbols != null) {
			var syms = ReadSymbolRoots(symbols, textVa);
			Console.WriteLine($"{symbols}: {syms.Count} .text addresses");
			roots.AddRange(syms);
		}

		var st = new Stats();
		var visited = new bool[textSize];  // per-byte: covered by a committed instruction
		var started = new HashSet<uint>();
		var rejected = new HashSet<uint>();
		var work = new Stack<uint>(roots.Distinct());
		var code = img.Data;
		var reader = new ByteArrayCodeReader(code);
		var decoder = Iced.Intel.Decoder.Create(64, reader);
		var run = new List<(uint Rva, Instruction Ins)>();
		var pushes = new List<(int At, uint Target)>();
		long runsRejected = 0;
		while(work.Count > 0) {
			// Decode one straight-line run speculatively and commit it only if it looks
			// like code: no invalid encoding, nothing a user-mode MSVC binary never emits.
			// The symbol file carries some non-code .text addresses and fallthrough after a
			// noreturn call lands in padding/data; both die here instead of polluting the
			// census with out/outsb/enter "instructions".
			run.Clear(); pushes.Clear();
			var rva = work.Pop();
			var ok = true;
			while(true) {
				if(rva < textVa || rva >= textVa + textSize || started.Contains(rva) || rejected.Contains(rva)) break;
				if(run.Count > 0 && run.Any(r => r.Rva == rva)) break;
				var off = img.RvaToOffset(rva);
				reader.Position = off;
				decoder.IP = img.Base + rva;
				var ins = decoder.Decode();
				if(ins.IsInvalid || Implausible(ins)) { ok = false; st.IcedInvalid++; break; }
				run.Add((rva, ins));
				var next = rva + (uint) ins.Length;
				var stop = false;
				switch(ins.FlowControl) {
					case FlowControl.Next:
						break;
					case FlowControl.Interrupt:
						stop = ins.Mnemonic == Mnemonic.Int3;
						break;
					case FlowControl.Call or FlowControl.IndirectCall or FlowControl.XbeginXabortXend:
						if(ins.FlowControl == FlowControl.Call) pushes.Add((run.Count - 1, (uint) (ins.NearBranchTarget - img.Base)));
						break;
					case FlowControl.ConditionalBranch:
						pushes.Add((run.Count - 1, (uint) (ins.NearBranchTarget - img.Base)));
						break;
					case FlowControl.UnconditionalBranch:
						if(ins.Op0Kind is OpKind.NearBranch64) pushes.Add((run.Count - 1, (uint) (ins.NearBranchTarget - img.Base)));
						stop = true;
						break;
					default:
						stop = true;
						break;
				}
				if(stop) break;
				rva = next;
			}
			if(!ok) {
				// Fallthrough past a call is the speculative edge (the callee may be
				// noreturn): keep the run up to its last call, reject what follows.
				runsRejected++;
				var keep = run.FindLastIndex(r => r.Ins.FlowControl is FlowControl.Call or FlowControl.IndirectCall) + 1;
				foreach(var (r, _) in run.Skip(keep)) rejected.Add(r);
				run.RemoveRange(keep, run.Count - keep);
				pushes.RemoveAll(t => t.At >= keep);
			}
			foreach(var (r, ins) in run) {
				started.Add(r);
				for(var k = 0; k < ins.Length; k++) visited[r - textVa + k] = true;
				var off = img.RvaToOffset(r);
				st.Check(code.AsSpan(off, Math.Min(15, code.Length - off)), in ins, img.Base + r);
			}
			foreach(var (_, t) in pushes) work.Push(t);
		}
		Console.WriteLine($"runs rejected as non-code: {runsRejected}");

		// How much .pdata-declared code did descent reach? The gap is jump-table-only
		// blocks (indirect jmp targets) plus whatever .pdata over-claims.
		long pdBytes = 0, pdSeen = 0;
		foreach(var (b, e) in img.Pdata)
			for(var r = b; r < e; r++) {
				if(r < textVa || r >= textVa + textSize) continue;
				pdBytes++;
				if(visited[r - textVa]) pdSeen++;
			}
		st.Report(top, outDir);
		Console.WriteLine($".pdata coverage: {pdSeen}/{pdBytes} bytes reached ({100.0 * pdSeen / pdBytes:f2}%)");
		return st.Undecoded + st.LenMismatch + st.MnemonicMismatch + st.LiftFail > 0 ? 1 : 0;
	}

	static bool Implausible(in Instruction ins) => ins.Mnemonic is
		Mnemonic.In or Mnemonic.Insb or Mnemonic.Insw or Mnemonic.Insd or
		Mnemonic.Out or Mnemonic.Outsb or Mnemonic.Outsw or Mnemonic.Outsd or
		Mnemonic.Cli or Mnemonic.Sti or Mnemonic.Hlt or Mnemonic.Iret or Mnemonic.Iretd or Mnemonic.Iretq or
		Mnemonic.Retf or Mnemonic.Enter or Mnemonic.Xlatb or Mnemonic.Lgdt or Mnemonic.Lidt or
		Mnemonic.Lldt or Mnemonic.Ltr or Mnemonic.Invd or Mnemonic.Wbinvd or Mnemonic.Rdmsr or Mnemonic.Wrmsr;

	static List<uint> ReadSymbolRoots(string path, uint textVa) {
		using var fs = File.OpenRead(path);
		using var doc = JsonDocument.Parse(fs);
		var ret = new List<uint>();
		foreach(var e in doc.RootElement.GetProperty("Addresses").EnumerateArray()) {
			var o = e.GetProperty("offset").GetString();
			if(o == null || !o.StartsWith("0001:")) continue;
			ret.Add(textVa + Convert.ToUInt32(o[5..], 16));
		}
		return ret;
	}

	class Stats {
		public long Insns, Decoded, Lifted, Stubbed, IcedInvalid;
		public long Undecoded, LenMismatch, MnemonicMismatch, LiftFail;
		readonly Dictionary<string, (long N, string Ex)> undecoded = new(), lenMis = new(), mnemMis = new(),
			liftFail = new(), stubs = new();
		readonly Dictionary<int, long> defHits = new();

		static void Bump(Dictionary<string, (long N, string Ex)> d, string k, Func<string> ex) =>
			d[k] = d.TryGetValue(k, out var v) ? (v.N + 1, v.Ex) : (1, ex());

		public void Check(ReadOnlySpan<byte> bytes, in Instruction ins, ulong pc) {
			Insns++;
			var hex = Convert.ToHexString(bytes[..ins.Length]);
			var icedText = ins.ToString();
			if(!Disassembler.DecodeInsn(bytes, XMode.Bits64, out var d)) {
				Undecoded++;
				Bump(undecoded, $"{ins.Mnemonic.ToString().ToLowerInvariant()} ({ins.Encoding})", () => $"{hex} = {icedText}");
				return;
			}
			Decoded++;
			defHits[d.DefId] = defHits.GetValueOrDefault(d.DefId) + 1;
			var def = Disassembler.DefNames[d.DefId];
			if(d.Len != ins.Length) {
				LenMismatch++;
				Bump(lenMis, $"{def} vs iced {ins.Mnemonic}", () => $"{hex} = {icedText} (ours len {d.Len})");
				return;
			}
			var ours = Disassembler.RenderInsn(in d, pc, XMode.Bits64);
			var om = OurMnemonic(ours);
			var im = ins.Mnemonic.ToString().ToLowerInvariant();
			if(om != im) {
				MnemonicMismatch++;
				Bump(mnemMis, $"{def} -> iced {im}", () => $"{hex} = iced '{icedText}' ours '{ours}'");
			}
			IlBlock block;
			try {
				block = X86Lifter.Lift(in d, pc, XMode.Bits64);
				if(block == null) throw new Exception("null block");
			} catch(Exception e) {
				LiftFail++;
				var msg = e.Message.Length > 70 ? e.Message[..70] : e.Message;
				Bump(liftFail, $"{def}: {e.GetType().Name}: {msg}", () => $"{hex} = {icedText}");
				return;
			}
			Lifted++;
			var stub = FindStub(block);
			if(stub != null) {
				Stubbed++;
				Bump(stubs, $"{def} | {stub}", () => $"{hex} = {icedText}");
			}
		}

		/// Our text -> bare mnemonic in iced's spelling. Condition-code aliases are the
		/// only normalization: anything else differing is a real decode disagreement.
		static string OurMnemonic(string text) {
			var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			var i = 0;
			while(i < parts.Length - 1 && parts[i] is "lock" or "rep" or "repe" or "repz" or "repne" or "repnz"
				      or "data16" or "addr32" or "bnd" or "notrack" or "xacquire" or "xrelease") i++;
			var m = parts[i].ToLowerInvariant();
			if(m == "fwait") return "wait";
			foreach(var stem in (string[]) ["cmov", "set", "j"]) {
				if(!m.StartsWith(stem) || m == "jmp" || m.StartsWith("jrcxz") || m.StartsWith("jecxz")) continue;
				var cc = m[stem.Length..];
				var canon = cc switch {
					"z" => "e", "nz" => "ne", "nb" or "nc" => "ae", "c" or "nae" => "b", "nbe" => "a", "na" => "be",
					"nle" => "g", "nl" => "ge", "nge" => "l", "ng" => "le", "pe" => "p", "po" => "np", _ => cc
				};
				return stem + canon;
			}
			return m;
		}

		/// First opaque node in the tree: intrinsic or unimplemented. Faults/traps are
		/// real architectural semantics, not stubs.
		static string FindStub(Il il) => il switch {
			IlIntrin(_, var n, var a) => "intrin." + n,
			IlUnimpl(var s, var w) => $"unimpl {s}{(w != null ? " — " + w : "")}",
			IlBlock(var b) => First(b),
			IlLet(_, var v) => FindStub(v),
			IlWriteReg(_, _, var v) => FindStub(v),
			IlStore(var a, var v) => FindStub(a) ?? FindStub(v),
			IlBranch(_, var t, var c) => FindStub(t) ?? (c != null ? FindStub(c) : null),
			IlIf(var c, var t, var e) => FindStub(c) ?? First(t) ?? First(e),
			IlLoop(var c, var b, var g) => FindStub(c) ?? First(b) ?? (g != null ? FindStub(g) : null),
			IlBin(_, _, var l, var r) => FindStub(l) ?? FindStub(r),
			IlUn(_, _, var x) => FindStub(x),
			IlCast(_, _, var x) => FindStub(x),
			IlLoad(_, var a) => FindStub(a),
			IlIfV(_, var c, var t, var e) => FindStub(c) ?? FindStub(t) ?? FindStub(e),
			IlVecBuild(_, _, var es) => First(es),
			IlVecAll(_, _, var s) => FindStub(s),
			IlVecElem(_, var v, var i) => FindStub(v) ?? FindStub(i),
			IlVecZeroTop(_, var v) => FindStub(v),
			IlVecBin(_, _, _, var l, var r) => FindStub(l) ?? FindStub(r),
			IlVecUn(_, _, _, var x) => FindStub(x),
			IlVecInsert(_, _, var i, var v) => FindStub(i) ?? FindStub(v),
			IlSyscall(var n) => FindStub(n),
			_ => null,
		};
		static string First(IEnumerable<Il> xs) {
			foreach(var x in xs) if(FindStub(x) is { } s) return s;
			return null;
		}

		public void Report(int top, string outDir) {
			Console.WriteLine();
			Console.WriteLine($"insns {Insns} | decoded {Decoded} ({Pct(Decoded, Insns)}) | undecoded {Undecoded} | LEN-MISMATCH {LenMismatch} | MNEMONIC-MISMATCH {MnemonicMismatch}");
			Console.WriteLine($"lifted {Lifted} ({Pct(Lifted, Decoded)} of decoded) | lift-fail {LiftFail} | stubbed {Stubbed} ({Pct(Stubbed, Lifted)} of lifted) | distinct defs {defHits.Count} | rejected-run ends {IcedInvalid}");
			Section("UNDECODED", undecoded, top);
			Section("LENGTH MISMATCHES", lenMis, top);
			Section("MNEMONIC MISMATCHES", mnemMis, top);
			Section("LIFT FAILURES", liftFail, top);
			Section("STUBBED LIFTS (intrin/unimpl)", stubs, top);
			if(outDir == null) return;
			Directory.CreateDirectory(outDir);
			foreach(var (name, d) in new[] { ("undecoded", undecoded), ("lenmismatch", lenMis), ("mnemonic", mnemMis), ("liftfail", liftFail), ("stubs", stubs) })
				File.WriteAllLines(Path.Combine(outDir, name + ".txt"),
					d.OrderByDescending(x => x.Value.N).Select(x => $"{x.Value.N,9}  {x.Key}   e.g. {x.Value.Ex}"));
		}

		static string Pct(long a, long b) => b == 0 ? "-" : $"{100.0 * a / b:f3}%";

		static void Section(string title, Dictionary<string, (long N, string Ex)> d, int top) {
			if(d.Count == 0) return;
			Console.WriteLine();
			Console.WriteLine($"{title} ({d.Count} classes, {d.Values.Sum(v => v.N)} insns):");
			foreach(var (k, (n, ex)) in d.OrderByDescending(x => x.Value.N).Take(top))
				Console.WriteLine($"  {n,8}  {k}   e.g. {(ex.Length > 90 ? ex[..90] + "…" : ex)}");
		}
	}
}

/// Minimal PE32+ reader: sections, image base, .pdata.
class PeImage {
	public readonly byte[] Data;
	public readonly ulong Base;
	public readonly (uint Va, uint Size) Text;
	public readonly List<(uint Begin, uint End)> Pdata = new();
	readonly List<(uint Va, uint VSize, uint RawOff, uint RawSize)> sections = new();

	public PeImage(byte[] data) {
		Data = data;
		if(data[0] != 'M' || data[1] != 'Z') throw new InvalidDataException("not PE");
		var pe = BitConverter.ToInt32(data, 0x3C);
		var nsec = BitConverter.ToUInt16(data, pe + 6);
		var optSize = BitConverter.ToUInt16(data, pe + 20);
		var opt = pe + 24;
		if(BitConverter.ToUInt16(data, opt) != 0x20B) throw new InvalidDataException("not PE32+");
		Base = BitConverter.ToUInt64(data, opt + 24);
		for(var i = 0; i < nsec; i++) {
			var s = opt + optSize + 40 * i;
			var name = System.Text.Encoding.ASCII.GetString(data, s, 8).TrimEnd('\0');
			var sec = (BitConverter.ToUInt32(data, s + 12), BitConverter.ToUInt32(data, s + 8),
				BitConverter.ToUInt32(data, s + 20), BitConverter.ToUInt32(data, s + 16));
			sections.Add(sec);
			if(name == ".text") Text = (sec.Item1, Math.Min(sec.Item2, sec.Item4));
		}
		var excRva = BitConverter.ToUInt32(data, opt + 112 + 3 * 8);
		var excSize = BitConverter.ToUInt32(data, opt + 112 + 3 * 8 + 4);
		if(excRva != 0) {
			var o = RvaToOffset(excRva);
			for(var i = 0; i < excSize / 12; i++)
				Pdata.Add((BitConverter.ToUInt32(data, o + 12 * i), BitConverter.ToUInt32(data, o + 12 * i + 4)));
		}
	}

	public int RvaToOffset(uint rva) {
		foreach(var (va, vs, ro, rs) in sections)
			if(rva >= va && rva < va + Math.Max(vs, rs)) return (int) (rva - va + ro);
		throw new ArgumentOutOfRangeException(nameof(rva), $"rva 0x{rva:x} not in any section");
	}
}
