using System.Diagnostics;
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
                else if(vtype is IpcType.BufferType) {
                } else
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
            if(retType == "void")
                args.AddRange(func.Outputs.Select((x, i) =>
                    $"{(x.Type is IpcType.BufferType ? "" : "out ")}{GenType(x.Type)} {(x.Name == null && x.Type is IpcType.PidType ? "pid" : x.Name ?? $"_{i + func.Inputs.Count}")}"));
            cb += $"protected virtual {retType} {Rename(func.Name)}({string.Join(", ", args)}) =>";
            cb++;
            if(func.Outputs.Count == 0)
                cb += $"Console.WriteLine(\"Stub hit for {csns}.{Rename(name)}.{Rename(func.Name)}\");";
            else
                cb += $"throw new NotImplementedException(\"{csns}.{Rename(name)}.{Rename(func.Name)} not implemented\");";
            cb--;
        }
        cb += "protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {";
        cb++;
        cb += "switch(im.CommandId) {";
        cb++;
        foreach(var func in iface.Functions) {
            var hasRet = func.Outputs.Count == 1 && func.Outputs[0].Type is not IpcType.BytesType and not IpcType.BufferType;
            var outI = 0;
            var inputOffset = 8;
            var moveInOffset = 0;
            var copyInOffset = 0;
            var outputOffset = 8;
            var moveOutOffset = 0;
            var copyOutOffset = 0;
            var bufferNums = new Dictionary<int, int>();
            var after = new CodeBuilder();

            string GenInputArg(IpcType type) {
                void Align(int align) {
                    while(inputOffset % align != 0) inputOffset++;
                }
                switch(type) {
                    case IpcType.BoolType: {
                        var ret = $"im.GetData<bool>({inputOffset})";
                        inputOffset += 4;
                        return ret;
                    }
                    case IpcType.BufferType(var btype, var ttype): {
                        bufferNums.TryAdd(ttype, 0);
                        var cbo = bufferNums[ttype];
                        bufferNums[ttype]++;
                        return $"im.Get{GenType(type)}(0x{ttype:X}, {cbo})";
                    }
                    case IpcType.BytesType(var size, var alignment): {
                        if(alignment != -1) Align(alignment);
                        var ret = $"im.GetBytes({inputOffset}, 0x{size:X})";
                        inputOffset += size;
                        return ret;
                    }
                    case IpcType.EnumType(IpcType.IntType(var size, _), _) { Alignment: var alignment }: {
                        Align(alignment == -1 ? size / 8 : alignment);
                        var ret = $"im.GetData<{GenType(type)}>({inputOffset})";
                        inputOffset += size / 8;
                        return ret;
                    }
                    case IpcType.IntType(var size, var signed) { Alignment: var alignment }: {
                        Align(alignment == -1 ? size / 8 : alignment);
                        var ret = $"im.GetData<{GenType(type)}>({inputOffset})";
                        inputOffset += size / 8;
                        return ret;
                    }
                    case IpcType.FloatType(var size) { Alignment: var alignment }: {
                        Align(alignment == -1 ? size / 8 : alignment);
                        var ret = $"im.GetData<{GenType(type)}>({inputOffset})";
                        inputOffset += size / 8;
                        return ret;
                    }
                    case IpcType.HandleType(var style):
                        return style == HandleStyle.Copy
                            ? $"Kernel.Get<KObject>(im.GetCopy({copyInOffset++}))"
                            : $"Kernel.Get<KObject>(im.GetMove({moveInOffset++}))";
                    case IpcType.ObjectType(var iname):
                        return $"Kernel.Get<{RenameNamespace(iname)}>(im.GetMove({moveInOffset++}))";
                    case IpcType.StructType { Alignment: var alignment }:
                        Align(alignment == -1 ? 8 : alignment);
                        return $"*({GenType(type)}*) im.GetDataPointer({inputOffset})"; // TODO: Inc offset
                    case IpcType.PidType: return "im.Pid";
                    default: throw new NotImplementedException($"Unknown type for GenInputArg: {type}");
                }
            }

            string GenOutputArg(IpcType type, bool isRet = false) {
                void Align(int align) {
                    while(outputOffset % align != 0) outputOffset++;
                }
                var vname = isRet ? "_return" : $"_{outI++}";
                switch(type) {
                    case IpcType.BoolType:
                        after += $"om.SetData({outputOffset}, {vname});";
                        outputOffset += 4;
                        break;
                    case IpcType.BufferType(_, var ttype): {
                        bufferNums.TryAdd(ttype, 0);
                        var cbo = bufferNums[ttype];
                        bufferNums[ttype]++;
                        return $"im.Get{GenType(type)}(0x{ttype:X}, {cbo})";
                    }
                    case IpcType.BytesType(var size, var alignment): {
                        Debug.Assert(size != -1);
                        if(alignment != -1) Align(alignment);
                        after += $"om.SetBytes({outputOffset}, {vname});";
                        outputOffset += size;
                        break;
                    }
                    case IpcType.EnumType(IpcType.IntType(var size, _), _) { Alignment: var alignment }:
                        Align(alignment == -1 ? size / 8 : alignment);
                        after += $"om.SetData({outputOffset}, {vname});";
                        outputOffset += size / 8;
                        break;
                    case IpcType.FloatType(var size) { Alignment: var alignment }:
                        Align(alignment == -1 ? size / 8 : alignment);
                        after += $"om.SetData({outputOffset}, {vname});";
                        outputOffset += size / 8;
                        break;
                    case IpcType.IntType(var size, _) { Alignment: var alignment }:
                        Align(alignment == -1 ? size / 8 : alignment);
                        after += $"om.SetData({outputOffset}, {vname});";
                        outputOffset += size / 8;
                        break;
                    case IpcType.HandleType(var style):
                        if(style == HandleStyle.Copy)
                            after += $"om.Copy({copyOutOffset++}, CreateHandle({vname}, copy: true));";
                        else
                            after += $"om.Move({moveOutOffset++}, CreateHandle({vname}));";
                        break;
                    case IpcType.ObjectType:
                        after += $"om.Move({moveOutOffset++}, CreateHandle({vname}));";
                        break;
                    case IpcType.StructType { Alignment: var alignment }:
                        Align(alignment == -1 ? 8 : alignment);
                        after += $"*({GenType(type)}*) om.GetDataPointer({outputOffset}) = {vname};"; // TODO: Inc offset
                        break;
                    case IpcType.ArrayType(var etype, var length): {
                        Debug.Assert(length > 0);
                        var ptr = $"ptr_{outI++}";
                        after += $"var {ptr} = ({GenType(etype)}*) om.GetDataPointer({outputOffset});";
                        for(var i = 0; i < length; ++i)
                            after += $"{ptr}[{i}] = {vname}[{i}];";
                        // TODO: Inc offset
                        break;
                    }
                    default: throw new NotImplementedException($"Unknown type for GenOutputArg: {type}");
                }
                return $"out var {vname}";
            }
            
            cb += $"case 0x{func.CmdId:X}: {{ // {Rename(func.Name)}";
            cb++;
            var args = func.Inputs.Select(x => GenInputArg(x.Type)).ToList();
            if(hasRet)
                GenOutputArg(func.Outputs[0].Type, isRet: true);
            else
                args.AddRange(func.Outputs.Select(x => GenOutputArg(x.Type)).Where(x => x != null));
            cb += $"om.Initialize({moveOutOffset}, {copyOutOffset}, {outputOffset - 8});";
            cb += $"{(hasRet ? "var _return = " : "")}{Rename(func.Name)}({string.Join(", ", args)});";
            if(after.Code.Length != 0)
                cb += after.Code.Trim();
            cb += "break;";
            cb--;
            cb += "}";
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
    IpcType.BytesType => "byte[]",
    IpcType.PidType => "ulong",
    IpcType.BufferType(IpcType.BytesType, _) => "Span<byte>",
    IpcType.BufferType(IpcType.UnknownType, _) or
        IpcType.BufferType(IpcType.ArrayType(IpcType.UnknownType, _), _) => "Span<byte>",
    IpcType.BufferType(IpcType.ArrayType(var dataType, _), _) => $"Span<{GenType(dataType)}>",
    IpcType.BufferType(var dataType, _) => $"Span<{GenType(dataType)}>",
    IpcType.ArrayType(var dataType, _) => $"{GenType(dataType)}[]",
    _ => throw new NotImplementedException($"Can't generate C# type for {type}"),
};

static string Rename(string name) {
    var ret = new string([name[0]]).ToUpper() + name[1..];
    return ret is "Close" or "Finalize" ? "_" + ret : ret;
}

static string RenameNamespace(string name) => string.Join(".", name.Split("::").Select(Rename));
static string GetName(string name) => name.Split("::").Last();
static string GetNamespaceName(string name) => string.Join("::", name.Split("::").SkipLast(1));
