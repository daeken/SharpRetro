using ArchCompilerCore;

// rung-1 shell: parse -> preprocess -> macro-expand a .isa; dump the resulting PTree.
// The legacy compiler's equivalent is: ListParser.Parse -> Preprocessor -> MacroProcessor.Rewrite
// (before Def/InferList — that's rung-1b). This lets us diff trees at each stage.
if(args.Length >= 1 && args[0] == "--heads") {
  Builtin.DefineAll();
  foreach(var (name, h) in Heads.All.OrderBy(kv => kv.Key))
    Console.WriteLine($"  {name,-20} stmt={(h.IsStatement?"y":"n")}");
  Console.WriteLine($"Total: {Heads.All.Count} heads registered");
  return 0;
}
if(args.Length < 1) {
  Console.Error.WriteLine("usage: ArchCompiler <file.isa> [--features f1,f2,...] [--stage parse|pp|macro] | --heads");
  return 1;
}
var path = args[0];
var features = args.SkipWhile(a => a != "--features").Skip(1).FirstOrDefault()?.Split(',') ?? [];
var stage = args.SkipWhile(a => a != "--stage").Skip(1).FirstOrDefault() ?? "macro";

var pp = new Preprocessor(features);
var tree = pp.Include(path);
pp.ValidateEnabled();
if(stage == "parse" || stage == "pp") { Dump(tree); return 0; }

var expanded = MacroProcessor.Rewrite(tree);
if(stage == "macro") { Dump(expanded); return 0; }

if(stage == "emit") {
  var arch = args.SkipWhile(a => a != "--arch").Skip(1).FirstOrDefault() ?? "aarch64";
  var outDir = args.SkipWhile(a => a != "--out").Skip(1).FirstOrDefault() ?? "/tmp/ac-out";
  Directory.CreateDirectory(outDir);
  Builtin.DefineAll();
  switch(arch) {
    case "aarch64": {
      new Frontends.Aarch64.Aarch64Heads().Define();
      var adefs = Def.ParseAll(expanded, Frontends.Aarch64.Aarch64Def.Parse)
        .Select(RuntimeInference.InferRuntime)
        .Cast<Frontends.Aarch64.Aarch64Def>().ToList();
      Backends.CSharp.Aarch64Scaffold.RegisterAll();
      Backends.CSharp.Aarch64Scaffold.BuildDisassembler(adefs, "Aarch64Generator", Path.Combine(outDir, "Disassembler.cs"));
      Backends.CSharp.Aarch64Scaffold.BuildRecompiler(adefs, "Aarch64Generator", Path.Combine(outDir, "Recompiler.cs"));
      break;
    }
    case "mips": {
      new Frontends.Mips.MipsHeads().Define();
      var mdefs = Def.ParseAll(expanded, Frontends.Mips.MipsDef.Parse)
        .Select(RuntimeInference.InferRuntime)
        .Cast<Frontends.Mips.MipsDef>().ToList();
      Backends.CSharp.MipsScaffold.RegisterAll();
      Backends.CSharp.MipsScaffold.BuildDisassembler(mdefs, "SharpStationGenerator", Path.Combine(outDir, "Disassembler.cs"));
      Backends.CSharp.MipsScaffold.BuildInterpreter(mdefs, "SharpStationGenerator", Path.Combine(outDir, "Interpreter.cs"));
      Backends.CSharp.MipsScaffold.BuildRecompiler(mdefs, "SharpStationGenerator", Path.Combine(outDir, "Recompiler.cs"));
      break;
    }
    case "dmg": {
      new Frontends.Dmg.DmgHeads().Define();
      var ddefs = Def.ParseAll(expanded, Frontends.Dmg.DmgDef.Parse)
        .Select(RuntimeInference.InferRuntime)
        .Cast<Frontends.Dmg.DmgDef>().ToList();
      Backends.CSharp.DmgScaffold.RegisterAll();
      Backends.CSharp.DmgScaffold.BuildDisassembler(ddefs, "DamageGenerator", Path.Combine(outDir, "Disassembler.cs"));
      Backends.CSharp.DmgScaffold.BuildInterpreter(ddefs, "DamageGenerator", Path.Combine(outDir, "Interpreter.cs"));
      break;
    }
    default: throw new NotSupportedException($"--arch {arch}");
  }
  Console.Error.WriteLine($"[emit {arch} → {outDir}]");
  return 0;
}

// stage == "typed": rung-1b — run InferType via Def.ParseAll, dump defs w/ types annotated.
Builtin.DefineAll();  // core heads
new Frontends.Aarch64.Aarch64Heads().Define();  // aarch64 per-ISA heads (rung-1b: as-is; rung-2+: → primitives/intrinsics)
var defs = Def.ParseAll(expanded, Frontends.Aarch64.Aarch64Def.Parse);
foreach(var d in defs) {
  Console.WriteLine($"# {d.Name}");
  Console.WriteLine($"  dasm: {FmtT(d.Disassembly)}");
  Console.WriteLine($"  decode: {FmtT(d.Decode)}");
  Console.WriteLine($"  eval: {FmtT(d.Eval)}");
  Console.WriteLine($"  locals: {string.Join(", ", d.Locals.OrderBy(kv=>kv.Key).Select(kv=>$"{kv.Key}:{kv.Value}"))}");
}
Console.Error.WriteLine($"[{defs.Count} defs typed]");
return 0;

static string FmtT(PTree t) => t switch {
  PList l => $"({string.Join(" ", l.Select(FmtT))}):{l.Type}",
  PName n => $"{n.Name}:{n.Type}",
  PInt i => $"{i.Value}:{i.Type}",
  PString s => $"\"{s.String}\":{s.Type}",
  _ => t.ToString()
};

static void Dump(PList top) {
  // Deterministic S-expr dump — this is rung-1's diff target.
  foreach(var form in top)
    Console.WriteLine(Fmt(form));
}
static string Fmt(PTree t) => t switch {
  PList l => "(" + string.Join(" ", l.Select(Fmt)) + ")",
  PName n => n.Name,
  PInt i => i.Value.ToString(),
  PString s => $"\"{s.String}\"",
  _ => t.ToString()
};

