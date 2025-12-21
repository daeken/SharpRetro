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
    .Where(x => x.Value is IpcType.StructType or IpcType.EnumType)
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
    ))).Where(x => x.Item2.Interfaces.Count != 0 || x.Item2.Typedefs.Count != 0).ToDictionary();

foreach(var (name, (tds, ids)) in namespaces)
    BuildNamespace(name, tds, ids);
return;

static void BuildNamespace(string nsName, Dictionary<string, IpcType> typedefs, Dictionary<string, Interface> interfaces) {
    var csns = RenameNamespace(nsName);
    var cb = new CodeBuilder();
    cb += "using System.Runtime.InteropServices;";
    cb += "using UmbraCore.Core;";
    cb += "// ReSharper disable once CheckNamespace";
    cb += $"namespace UmbraCore.Services.{csns};";

    foreach(var (name, typedef) in typedefs) {
        if(typedef is IpcType.EnumType edef) {
            cb += $"public enum {Rename(name)} : {GenType(edef.UnderlyingType)} {{";
            cb++;
            foreach(var (vname, value) in edef.Options)
                cb += $"{Rename(vname)} = 0x{value:X},";
            cb--;
            cb += "}";
        } else if(typedef is IpcType.StructType sdef) {
            cb += "[StructLayout(LayoutKind.Sequential, Pack = 1)]";
            cb += $"public unsafe struct {Rename(name)} {{";
            cb++;
            foreach(var (vname, vtype) in sdef.Fields) {
                if(vtype is IpcType.BytesType(-1, _))
                    cb += $"public byte _{Rename(vname)};";
                else if(vtype is IpcType.BytesType(var size, _))
                    cb += $"public fixed byte {Rename(vname)}[{size}];";
                else
                    cb += $"public {GenType(vtype)} {Rename(vname)};";
            }
            cb--;
            cb += "}";
        }
    }

    foreach(var (name, iface) in interfaces) {
        cb += $"public partial class {Rename(name)} : _{Rename(name)}_Base;";
        cb += $"public abstract class _{Rename(name)}_Base : IpcInterface {{";
        cb++;
        foreach(var func in iface.Functions) {
            Console.WriteLine($"{csns}.{Rename(name)}.{Rename(func.Name)}");
            var retType = func.Outputs.Count == 1 && func.Outputs[0].Type is not IpcType.BytesType and not IpcType.BufferType
                ? GenType(func.Outputs[0].Type)
                : "void";
            var args = func.Inputs.Select((x, i) => $"{GenType(x.Type)} {x.Name ?? $"_{i}"}").ToList();
            cb += $"protected virtual {retType} {Rename(func.Name)}({string.Join(", ", args)}) =>";
            cb++;
            if(func.Outputs.Count == 0)
                cb += $"Console.WriteLine(\"Stub hit for {csns}.{Rename(name)}.{Rename(func.Name)}\");";
            else
                cb += $"throw new NotImplementedException(\"{csns}.{Rename(name)}.{Rename(func.Name)} not implemented\");";
            cb--;
        }
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

static string GenType(IpcType type) => type switch {
    IpcType.BoolType => "bool",
    IpcType.IntType(8, false) => "byte",
    IpcType.IntType(8, true) => "sbyte",
    IpcType.IntType(16, false) => "ushort",
    IpcType.IntType(16, true) => "short",
    IpcType.IntType(32, false) => "uint",
    IpcType.IntType(32, true) => "int",
    IpcType.IntType(64, false) => "ulong",
    IpcType.IntType(64, true) => "long",
    IpcType.IntType(128, false) => "UInt128",
    IpcType.IntType(128, true) => "Int128",
    IpcType.FloatType(32) => "float",
    IpcType.FloatType(64) => "double",
    IpcType.HandleType(HandleStyle.Copy) => "KObject",
    IpcType.HandleType(HandleStyle.Move) => "IpcInterface",
    IpcType.ObjectType("unknown") => "IpcInterface",
    IpcType.ObjectType(var name) => RenameNamespace(name),
    IpcType.EnumType { Name: {} name } => RenameNamespace(name),
    IpcType.StructType { Name: {} name } => RenameNamespace(name),
    IpcType.BytesType => "Span<byte>",
    IpcType.PidType => "ulong",
    IpcType.BufferType(IpcType.BytesType, _) => "Span<byte>",
    IpcType.BufferType(IpcType.UnknownType, _) => "Span<byte>",
    IpcType.BufferType(IpcType.ArrayType(var dataType, _), _) => $"Span<{GenType(dataType)}>",
    IpcType.BufferType(var dataType, _) => $"Span<{GenType(dataType)}>",
    _ => throw new NotImplementedException($"Can't generate C# type for {type}"),
};

static string Rename(string name) => new string([name[0]]).ToUpper() + name[1..];
static string RenameNamespace(string name) => string.Join(".", name.Split("::").Select(Rename));
static string GetName(string name) => name.Split("::").Last();
static string GetNamespaceName(string name) => string.Join("::", name.Split("::").SkipLast(1));
