namespace SharpStationGenerator;
using CoreArchCompiler;

public class Program : Core {
	public static void Main(string[] args) {
		var defs = Core.ParseSpec(File.ReadAllText("mips-r3051.isa"), new ExecutionState(), MipsDef.Parse).Select(x => (MipsDef) x).ToList();
		BuildDisassembler(defs);
		BuildInterpreter(defs);
	}
	
	static void BuildDisassembler(List<MipsDef> defs) {
		Context = ContextTypes.Disassembler;

		var c = new CodeBuilder();
		c += 2;
		var ic = new CodeBuilder();
		ic += 2;

		var labelNum = 0;

		foreach(var def in defs) {
			NextLabel = $"insn_{++labelNum}";
			var matcher = string.Join(" && ", def.MatchBytes.Select(x => $"(insnBytes[{x.Key}] & 0x{x.Value.Mask:X}) == 0x{x.Value.Match:X}"));
			c += $"/* {def.Name} */";
			c += $"if({matcher}) {{";
			ic += $"if({matcher}) {{";
			c++;
			ic++;
			GenerateFields(c, def);
			GenerateStatement(c, def.Decode);
			GenerateFields(ic, def);
			GenerateStatement(ic, def.Decode);
			ic += $"return \"{def.Name}\";";
			c += $"pc += {def.Size};";
			c += $"return {GenerateExpression(def.Disassembly)};";
			c--;
			ic--;
			c += "}";
			ic += "}";
			c += $"{NextLabel}:";
			ic += $"{NextLabel}:";
		}

		using var fp = File.Open("../DamageCore/Generated/Disassembler.cs", FileMode.Create);
		using var sw = new StreamWriter(fp);
		sw.Write(File.ReadAllText("DisassemblerStub.cs.skip")
			.Replace("/*%D_CODE%*/", c.Code)
			.Replace("/*%IC_CODE%*/", ic.Code)
			.Replace("/*%IC_COUNT%*/", defs.Count.ToString()));
	}
	
	static void BuildInterpreter(List<MipsDef> defs) {
		Context = ContextTypes.Interpreter;

		var c = new CodeBuilder();
		c += 1;
		var labelNum = 0;

		foreach(var def in defs) {
			NextLabel = $"insn_{++labelNum}";
			c += $"/* {def.Name} */";
			var matcher = string.Join(" && ", def.MatchBytes.Select(x => $"(insnBytes[{x.Key}] & 0x{x.Value.Mask:X}) == 0x{x.Value.Match:X}"));
			c += $"if({matcher}) {{";
			c++;
			GenerateFields(c, def);
			GenerateStatement(c, def.Decode);
			c += $"pc += {def.Size};";
			GenerateStatement(c, def.Eval);
			c += "return true;";
			c--;
			c += "}";
			c += $"{NextLabel}:";
		}

		using var fp = File.Open("../DamageCore/Generated/Interpreter.cs", FileMode.Create);
		using var sw = new StreamWriter(fp);
		sw.Write(File.ReadAllText("InterpreterStub.cs.skip").Replace("/*%CODE%*/", c.Code));
	}

	static void GenerateFields(CodeBuilder c, MipsDef def) {
		foreach(var (fname, (bi, size, shift)) in def.Fields)
			c += $"var {fname} = (byte) ((byte) (insnBytes[{bi}] >> {shift}) & 0x{(1 << size) - 1:X});";
	}
}