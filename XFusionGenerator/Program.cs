using CoreArchCompiler;

namespace XFusionGenerator;

public class Program : Core {
	public static void Main(string[] args) {
		// Feature set = the CPU we're compiling. CLI args override the default (ia32 dev set).
		var features = args.Length != 0 ? args : ["ia32"];
		Console.WriteLine($"XFusion: compiling with features [{string.Join(", ", features)}]");

		var isaDir = File.Exists("xfusion.isa") ? "." : AppContext.BaseDirectory;
		var pp = new Preprocessor(features);
		var tree = pp.Include(Path.Combine(isaDir, "xfusion.isa"));
		pp.ValidateEnabled();
		tree = MacroProcessor.Rewrite(tree);

		var (templates, defs) = XFusionDef.CollectAll(tree);
		Console.WriteLine($"{templates.Count} instruction templates, {defs.Count} encodings.");

		var outDir = FindOutDir();
		Directory.CreateDirectory(outDir);
		File.WriteAllText(Path.Combine(outDir, "Disassembler.cs"), DisassemblerGenerator.Generate(defs));
		Console.WriteLine($"Wrote {Path.Combine(outDir, "Disassembler.cs")}");
	}

	static string FindOutDir() {
		// Run from repo root or XFusionGenerator/ or bin/: find XFusionCpu/ upward.
		var dir = Directory.GetCurrentDirectory();
		for(var d = dir; d != null; d = Path.GetDirectoryName(d)) {
			var cand = Path.Combine(d, "XFusionCpu");
			if(Directory.Exists(cand)) return Path.Combine(cand, "Generated");
		}
		throw new DirectoryNotFoundException("XFusionCpu/ not found upward from cwd");
	}
}
