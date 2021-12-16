namespace DamageGenerator;
using CoreArchCompiler;

public static class Program {
	public static void Main(string[] args) {
		var defs = Core.ParseSpec(File.ReadAllText("sm83.isa"), new ExecutionState(), DmgDef.Parse);
	}
}