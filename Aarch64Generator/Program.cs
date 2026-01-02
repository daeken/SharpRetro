using System.Diagnostics;
using LibSharpRetro;

namespace Aarch64Generator;

using CoreArchCompiler;

public class Program : Core {
	public static void Main(string[] args) {
		var defs = Core.ParseSpec(File.ReadAllText("aarch64.isa"), new(), Aarch64Def.Parse).Select(x => (Aarch64Def) x).ToList();
		BuildDisassembler(defs);
		//BuildInterpreter(defs);
		BuildRecompiler(defs);

		//CleanupCode("../Aarch64Cpu/Generated");
	}
	
	static void BuildDisassembler(List<Aarch64Def> defs) {
		Context = ContextTypes.Disassembler;

		var c = new CodeBuilder();
		c += 2;
		var ic = new CodeBuilder();
		ic += 2;

		var labelNum = 0;

		foreach(var def in defs) {
			NextLabel = $"insn_{++labelNum}";
			c += $"/* {def.Name} */";
			c += $"if((insn & 0x{def.Mask:X08}) == 0x{def.Match:X08}) {{";
			ic += $"if((insn & 0x{def.Mask:X08}) == 0x{def.Match:X08}) {{";
			c++;
			ic++;
			GenerateFields(c, def);
			GenerateStatement(c, def.Decode);
			GenerateFields(ic, def);
			GenerateStatement(ic, def.Decode);
			ic += $"return \"{def.Name}\";";
			c += $"return {GenerateExpression(def.Disassembly)};";
			c--;
			ic--;
			c += "}";
			ic += "}";
			c += $"{NextLabel}:";
			ic += $"{NextLabel}:";
		}
		
		var gprc = new CodeBuilder();
		gprc += 2;
		var isFirst = true;
		
		foreach(var def in defs) {
			var regSet = new HashSet<(bool PossiblySp, bool IsRead, bool IsWritten, string Name)>();
			void FindRegisters(PTree elem) {
				if(elem is not PList list) return;
				if(list.Count is 2 or 3 && list[0] is PName(var name)) {
					if(name == "=" && list.Count == 3 &&
					   list[1] is PList { Count: 2 } slist &&
					   slist[0] is PName(var sname and ("gpr32" or "gpr64" or "gpr-or-sp32" or "gpr-or-sp64")) &&
					   slist[1] is PName(var sregname)
					) {
						regSet.Add((sname is "gpr-or-sp32" or "gpr-or-sp64", false, true, sregname));
						FindRegisters(list[2]);
						return;
					}
					if(name is "gpr32" or "gpr64" or "gpr-or-sp32" or "gpr-or-sp64")
						if(list[1] is PName(var regname)) {
							regSet.Add((name is "gpr-or-sp32" or "gpr-or-sp64", true, false, regname));
							return;
						}
				}
				foreach(var selem in list)
					FindRegisters(selem);
			}
			FindRegisters(def.Eval);
			if(regSet.Count == 0) continue;
			regSet = new(regSet.GroupBy(x => x.Name).Select(x => (
				x.Any(y => y.PossiblySp),
				x.Any(y => y.IsRead),
				x.Any(y => y.IsWritten),
				x.Key
			)));
			gprc += $"/* {def.Name} */";
			gprc += $"{(!isFirst ? "else " : "")}if((insn & 0x{def.Mask:X08}) == 0x{def.Match:X08}) {{";
			isFirst = false;
			gprc++;
			foreach(var (psp, r, w, reg) in regSet) {
				Debug.Assert(def.Fields.ContainsKey(reg));
				var shift = def.Fields[reg].Shift;
				gprc += $"yield return ({(psp ? "true" : "false")}, {(r ? "true" : "false")}, {(w ? "true" : "false")}, {shift});";
			}
			gprc--;
			gprc += "}";
		}

		var pcDependent = new List<string>();
		foreach(var def in defs) {
			var isDep = false;

			void FindDependent(PTree elem) {
				if(elem is not PList list || list.Count < 1) return;
				if(list.Count == 1 && list[0] is PName("pc"))
					isDep = true;
			}
			def.Decode.WalkLeaves(FindDependent);
			def.Eval.WalkLeaves(FindDependent);
			
			if(isDep)
				pcDependent.Add($"/* {def.Name} */ (insn & 0x{def.Mask:X08}) == 0x{def.Match:X08}");
		}

		using var fp = File.Open("../Aarch64Cpu/Generated/Disassembler.cs", FileMode.Create);
		using var sw = new StreamWriter(fp);
		sw.Write(File.ReadAllText("DisassemblerStub.cs.skip")
			.Replace("/*%D_CODE%*/", c.Code)
			.Replace("/*%IC_CODE%*/", ic.Code)
			.Replace("/*%MASK_CODE%*/", gprc.Code)
			.Replace("/*%PCD_CODE%*/", string.Join(" || \n\t\t\t", pcDependent))
			.Replace("/*%IC_COUNT%*/", defs.Count.ToString()));
	}
	
	static void BuildInterpreter(List<Aarch64Def> defs) {
		Context = ContextTypes.Interpreter;

		var c = new CodeBuilder();
		c += 3;
		var labelNum = 0;

		foreach(var def in defs) {
			NextLabel = $"insn_{++labelNum}";
			c += $"/* {def.Name} */";
			c += $"if((insn & 0x{def.Mask:X08}) == 0x{def.Match:X08}) {{";
			c++;
			GenerateFields(c, def);
			GenerateStatement(c, def.Decode);
			GenerateStatement(c, def.Eval);
			c += "return true;";
			c--;
			c += "}";
			c += $"{NextLabel}:";
		}

		using var fp = File.Open("../Aarch64Cpu/Generated/Interpreter.cs", FileMode.Create);
		using var sw = new StreamWriter(fp);
		sw.Write(File.ReadAllText("InterpreterStub.cs.skip").Replace("/*%CODE%*/", c.Code));
	}

	static void BuildRecompiler(List<Aarch64Def> defs) {
		Context = ContextTypes.Recompiler;

		var c = new CodeBuilder();
		c += 2;
		var labelNum = 0;

		foreach(var def in defs) {
			NextLabel = $"insn_{++labelNum}";
			c += $"/* {def.Name} */";
			c += $"if((insn & 0x{def.Mask:X08}) == 0x{def.Match:X08}) {{";
			c++;
			GenerateFields(c, def);
			GenerateStatement(c, def.Decode);
			GenerateStatement(c, def.Eval);
			c += "return true;";
			c--;
			c += "}";
			c += $"{NextLabel}:";
		}

		using var fp = File.Open("../Aarch64Cpu/Generated/Recompiler.cs", FileMode.Create);
		using var sw = new StreamWriter(fp);
		sw.Write(File.ReadAllText("RecompilerStub.cs.skip").Replace("/*%CODE%*/", c.Code));
	}

	static void GenerateFields(CodeBuilder c, Aarch64Def def) {
		foreach(var (key, (bits, shift)) in def.Fields)
			c += $"var {key} = (insn >> {shift}) & 0x{(1 << bits) - 1:X}U;";
	}
}