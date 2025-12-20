using SwipcParser;
using static SwipcGenerator.IpcType;
namespace SwipcGenerator;

public record Interface(Versions Versions, string Name, IReadOnlyList<string> ServiceNames, IReadOnlyList<Function> Functions) : SwipcNode {
    public override string ToString() =>
        $"Interface {{ Versions = {Versions}, Name = {Name}, ServiceNames = [{string.Join(", ", ServiceNames)}], Functions = [\n{SwipcNode.Indent(string.Join("\n", Functions.Select(x => x.ToString())))}\n] }}";
}
public record Function(Versions Versions, string Name, ulong CmdId, IReadOnlyList<(IpcType Type, string Name)> Inputs, IReadOnlyList<(IpcType Type, string Name)> Outputs) : SwipcNode {
    public override string ToString() =>
        $"Function {{ Versions = {Versions}, Name = {Name}, CmdId = {CmdId}, Inputs=[{string.Join(", ", Inputs)}], Outputs=[{string.Join(", ", Outputs)}] }}";
}

public class Concretize {
    public readonly IReadOnlyDictionary<string, IpcType> Typedefs;
    public readonly IReadOnlyList<Interface> Interfaces;

    readonly Dictionary<string, IpcType> Concretized = [];
    readonly Dictionary<string, SwipcNode.Type> Forward = [];
    
    public Concretize(List<SwipcNode.Typedef> typedefs, List<SwipcNode.Interface> interfaces) {
        foreach(var x in typedefs)
            if(x.Template is null or [])
                Forward[x.Name] = x.Of;
            else {
                if(x.Name != "nn::util::BitFlagSet" || x.Template is not [_, SwipcNode.Concrete(var name, _, _)])
                    throw new NotSupportedException($"Typedef {x} not supported");
                Forward[name] = x.Of;
            }
        Interfaces = interfaces.Select(x => new Interface(
            x.Versions,
            x.Name,
            x.ServiceNames,
            x.Functions.Select(y => new Function(
                y.Versions,
                y.Name,
                y.CmdId,
                y.Inputs.Select(z => (Parse(z.Type), z.Name)).ToList(),
                y.Outputs.Select(z => (Parse(z.Type), z.Name)).ToList()
            )).ToList()
        )).ToList();
        Interfaces = Interfaces.GroupBy(x => x.Name).Select(x => x.Last()).ToList();

        Typedefs = Concretized;
        Forward.Clear();
    }

    IpcType Resolve(string name, SwipcNode.Type type) {
        Forward.Remove(name);
        return Concretized[name] = Parse(type);
    }
    
    IpcType Parse(SwipcNode.Type type) => type switch {
        SwipcNode.Concrete("buffer", [var btype, SwipcNode.Number(var tt)], (false, _)) =>
            new BufferType(Parse((SwipcNode.Type) btype), (int) tt),
        SwipcNode.Concrete("buffer", [var btype, SwipcNode.Number(var tt), _], (false, _)) =>
            new BufferType(Parse((SwipcNode.Type) btype), (int) tt),
        SwipcNode.Concrete("array", [var atype, SwipcNode.Number(var tt)], (false, _)) =>
            new BufferType(Parse((SwipcNode.Type) atype), (int) tt),
        SwipcNode.Concrete(var name, null or [], (false, _)) =>
            name switch {
                "bool" => Bool,
                "u8" or "b8" => U8, "u16"          => U16, "u32"          => U32, "u64"          => U64, "u128" => U128,
                "i8" or "s8" => I8, "i16" or "s16" => I16, "i32" or "s32" => I32, "i64" or "s64" => I64, "i128" => I128,
                "int" => I32,
                "f32" => F32, "f64" => F64,
                "bytes" => new BytesType(),
                "pid" => new PidType(),
                "KObject" => new HandleType(HandleStyle.Copy),
                "unknown" => Unknown,
                _ when Concretized.TryGetValue(name, out var ctype) => ctype,
                _ when Forward.TryGetValue(name, out var ftype) => Resolve(name, ftype),
                _ => throw new NotImplementedException($"Concrete type {name} not implemented"),
            },
        SwipcNode.Concrete("nn::util::BitFlagSet", [_, SwipcNode.Concrete(var name, _, _)], (false, _)) =>
            Concretized.TryGetValue(name, out var ty)
                ? ty : Resolve(name, Forward[name]),
        SwipcNode.Concrete("align", [SwipcNode.Number(var align), var st], (false, _)) =>
            Parse((SwipcNode.Type) st) with { Alignment = (int) align },
        SwipcNode.Concrete("handle", [SwipcNode.Concrete("copy", _, _), ..], (false, _)) =>
            new HandleType(HandleStyle.Copy),
        SwipcNode.Concrete("handle", [SwipcNode.Concrete("move", _, _), ..], (false, _)) =>
            new HandleType(HandleStyle.Move),
        SwipcNode.Concrete("bytes" or "unknown", [SwipcNode.Number(var size)], (false, _)) =>
            new BytesType((int) size),
        SwipcNode.Concrete("bytes", [SwipcNode.Number(var size), SwipcNode.Number(var align)], (false, _)) =>
            new BytesType((int) size, (int) align),
        SwipcNode.Concrete("bytes", [SwipcNode.Number(var size), SwipcNode.Concrete("unknown", _, _)], (false, _)) =>
            new BytesType((int) size),
        SwipcNode.Concrete("object", [SwipcNode.Concrete(var name, _, _)], (false, _)) =>
            new ObjectType(name),
        SwipcNode.Enum([var underlying], var members) =>
            new EnumType(Parse((SwipcNode.Type) underlying), members),
        SwipcNode.Struct(_, var fields) => new StructType(
            fields.Select(x => (x.Name, Parse(x.Type))).ToList()
        ),
        SwipcNode.Concrete(_, _, (true, var length)) c =>
            new ArrayType(Parse(c with { Array = (false, null) }), length == null ? -1 : (int) length),
        _ => throw new NotImplementedException($"Type {type} not implemented"),
    };
}