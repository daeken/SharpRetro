namespace DamageGenerator;
using CoreArchCompiler;

public class Program : Core {
	public static void Main(string[] args) {
		var defs = Core.ParseSpec(File.ReadAllText("sm83.isa"), new ExecutionState(), DmgDef.Parse);
		BuildDisassembler(defs);
	}
	
	static void BuildDisassembler(List<Def> defs) {
		Context = ContextTypes.Disassembler;

		var c = new CodeBuilder();
		c += 2;
		var ic = new CodeBuilder();
		ic += 2;

		var labelNum = 0;

		foreach(var _def in defs) {
			if(_def is not DmgDef def)
				throw new NotSupportedException();
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

	static void GenerateFields(CodeBuilder c, DmgDef def) {
		foreach(var (fname, (bi, size, shift)) in def.Fields)
			c += $"var {fname} = (insnBytes[{bi}] >> {shift}) & 0x{(1 << size) - 1:X};";
	}
}