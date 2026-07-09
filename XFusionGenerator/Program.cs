using CoreArchCompiler;

namespace XFusionGenerator;

public class Program : Core {
	public static void Main(string[] args) {
		// Feature set = the CPU we're compiling. CLI args override the default (ia32 dev set).
		var features = args.Length != 0 ? args : ["ia32"];
		Console.WriteLine($"XFusion: compiling with features [{string.Join(", ", features)}]");

		var isaPath = File.Exists("xfusion.isa") ? "xfusion.isa"
			: Path.Combine(AppContext.BaseDirectory, "xfusion.isa");
		var defs = ParseSpecFile(isaPath, features, new(), XFusionDef.Parse);
		Console.WriteLine($"{defs.Count} defs parsed.");
		// Generation (disassembler/interpreter/recompiler) lands with XF-1.
	}
}
