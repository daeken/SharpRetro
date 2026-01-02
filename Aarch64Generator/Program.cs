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
		BuildRegisterMasker(defs);

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

		using var fp = File.Open("../Aarch64Cpu/Generated/Disassembler.cs", FileMode.Create);
		using var sw = new StreamWriter(fp);
		sw.Write(File.ReadAllText("DisassemblerStub.cs.skip")
			.Replace("/*%D_CODE%*/", c.Code)
			.Replace("/*%IC_CODE%*/", ic.Code)
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

	static void BuildRegisterMasker(List<Aarch64Def> defs) {
		var c = new CodeBuilder();
		c += 2;
		var labelNum = 0;
		
		foreach(var def in defs) {
			NextLabel = $"insn_{++labelNum}";
			c += $"/* {def.Name} */";
			c += $"if((insn & 0x{def.Mask:X08}) == 0x{def.Match:X08}) {{";
			c++;
			var regSet = new HashSet<(bool PossiblySp, string Name)>();
			void FindRegisters(PTree elem) {
				if(elem is not PList { Count: 2 } list) return;
				if(list[0] is not PName(var name) ||
				   name is not "gpr32" and not "gpr64" and not "gpr-or-sp32" and not "gpr-or-sp64") return;
				if(list[1] is PName(var regname))
					regSet.Add((name is "gpr-or-sp32" or "gpr-or-sp64", regname));
			}
			def.Eval.WalkLeaves(FindRegisters);
			foreach(var (psp, reg) in regSet) {
				Debug.Assert(def.Fields.ContainsKey(reg));
				var (bits, shift) =  def.Fields[reg];
				c += $"yield return ({(psp ? "true" : "false")}, {shift});";
			}
			c--;
			c += "}";
			c += $"{NextLabel}:";
		}

		c++;
		c += "yield break;";
		c--;

		using var fp = File.Open("../Aarch64Cpu/Generated/RegisterMasker.cs", FileMode.Create);
		using var sw = new StreamWriter(fp);
		sw.Write(File.ReadAllText("RegisterMaskerStub.cs.skip").Replace("/*%CODE%*/", c.Code));
	}

	static void GenerateFields(CodeBuilder c, Aarch64Def def) {
		foreach(var (key, (bits, shift)) in def.Fields)
			c += $"var {key} = (insn >> {shift}) & 0x{(1 << bits) - 1:X}U;";
	}
}