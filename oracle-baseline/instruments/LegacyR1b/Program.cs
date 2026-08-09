using CoreArchCompiler;
using Aarch64Generator;

// Rung-1b legacy-side instrument: dump typed trees post-Def.ParseAll+InferRuntime.
// Mirrors Aarch64Generator/Program.cs:10's own path (Core.ParseSpec) exactly —
// the static Core() ctor reflects+registers every Builtin subclass in loaded assemblies
// (incl aarch64's, because we <Compile Include> Builtins.cs into this project).
var defs = Core.ParseSpec(File.ReadAllText(args[0]), new(), Aarch64Def.Parse);
foreach(var d in defs) {
  Console.WriteLine($"# {d.Name}");
  Console.WriteLine($"  dasm: {FmtT(d.Disassembly)}");
  Console.WriteLine($"  decode: {FmtT(d.Decode)}");
  Console.WriteLine($"  eval: {FmtT(d.Eval)}");
  Console.WriteLine($"  locals: {string.Join(", ", d.Locals.OrderBy(kv=>kv.Key).Select(kv=>$"{kv.Key}:{kv.Value}"))}");
}
Console.Error.WriteLine($"[{defs.Count} defs typed]");

static string FmtT(PTree t) => t switch {
  PList l => $"({string.Join(" ", l.Select(FmtT))}):{l.Type}",
  PName n => $"{n.Name}:{n.Type}",
  PInt i => $"{i.Value}:{i.Type}",
  PString s => $"\"{s.String}\":{s.Type}",
  _ => t.ToString()
};
