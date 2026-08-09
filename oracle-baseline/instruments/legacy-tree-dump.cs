using CoreArchCompiler;
var tree = ListParser.Parse(File.ReadAllText(args[0]));
var expanded = MacroProcessor.Rewrite(tree);
foreach(var form in expanded) Console.WriteLine(Fmt(form));
static string Fmt(PTree t) => t switch {
  PList l => "(" + string.Join(" ", l.Select(Fmt)) + ")",
  PName n => n.Name,
  PInt i => i.Value.ToString(),
  PString s => $"\"{s.String}\"",
  _ => t.ToString()
};
