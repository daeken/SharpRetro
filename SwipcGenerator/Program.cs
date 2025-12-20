using LibSharpRetro;
using SwipcGenerator;
using SwipcParser;

var typedefs = new List<SwipcNode.Typedef>();
var interfaces = new List<SwipcNode.Interface>();
var fns = Directory.EnumerateFiles("ipcdefs", "*.id")
    .OrderByDescending(x => Path.GetFileName(x) is "auto.id" or "switchbrew.id").ToList();
foreach(var fn in fns) {
    Console.WriteLine($"Parsing {fn}");
    var code = File.ReadAllText(fn);
    var lines = code.Split('\n');
    lines = lines.Select(x => x.Split("//")[0]).Where(x => x.Trim() != "#").ToArray();
    code = string.Join('\n', lines);
    var parser = new Parser(code);
    typedefs.AddRange(parser.Typedefs);
    interfaces.AddRange(parser.Interfaces);
}
var concrete = new Concretize(typedefs, interfaces);
var namespaceNames = concrete.Typedefs.Keys.Concat(concrete.Interfaces.Select(x => x.Name))
    .Select(GetNamespaceName).ToHashSet();
var nstd = concrete.Typedefs
    .Select(x => (GetNamespaceName(x.Key), GetName(x.Key), x.Value))
    .GroupBy(x => x.Item1)
    .Select(x => (x.Key, x.Select(y => (y.Item2, y.Value)).ToDictionary()))
    .ToDictionary();
var nsid = concrete.Interfaces
    .Select(x => (GetNamespaceName(x.Name), GetName(x.Name), x))
    .GroupBy(x => x.Item1)
    .Select(x => (x.Key, x.Select(y => (y.Item2, y.x)).ToDictionary()))
    .ToDictionary();
var namespaces = namespaceNames.Select(name => (
    name, 
    (
        Typedefs: nstd.TryGetValue(name, out var tds) ? tds : [],
        Interfaces: nsid.TryGetValue(name, out var ids) ? ids : []
    ))).ToDictionary();

foreach(var (name, (tds, ids)) in namespaces)
    BuildNamespace(name, tds, ids);
return;

static void BuildNamespace(string nsName, Dictionary<string, IpcType> typedefs, Dictionary<string, Interface> interfaces) {
    var csns = RenameNamespace(nsName);
    var cb = new CodeBuilder();
    cb += "using UmbraCore.Core;";
    cb += "// ReSharper disable once CheckNamespace";
    cb += $"namespace UmbraCore.Services.{csns};";

    foreach(var (name, iface) in interfaces) {
        cb += $"public partial class {Rename(name)} : _{Rename(name)}_Base;";
        cb += $"public abstract class _{Rename(name)}_Base : IpcInterface {{";
        cb++;
        cb += "protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {";
        cb++;
        cb += "switch(im.CommandId) {";
        cb++;
        foreach(var function in iface.Functions) {
            cb += $"case 0x{function.CmdId:X}: // {function.Name}";
            cb++;
            cb += "break;";
            cb--;
        }
        cb += "default:";
        cb++;
        cb += $"throw new NotImplementedException($\"Got unhandled command 0x{{im.CommandId:X}} in {csns}.{Rename(name)}\");";
        cb--;
        cb--;
        cb += "}";
        cb--;
        cb += "}";
        cb--;
        cb += "}";
        cb += "";
    }
    
    File.WriteAllText($"../UmbraCore/Services/Generated/{csns}.cs", cb.Code);
}

static string Rename(string name) => new string([name[0]]).ToUpper() + name[1..];
static string RenameNamespace(string name) => string.Join(".", name.Split("::").Select(Rename));
static string GetName(string name) => name.Split("::").Last();
static string GetNamespaceName(string name) => string.Join("::", name.Split("::").SkipLast(1));
