namespace SwipcParser;

public record Version(int Major, int Minor, int Point);

public abstract record Versions {
    public record All : Versions;
    public record Range(Version Start, Version End) : Versions;
    public record Ongoing(Version Start) : Versions;
}

public abstract record SwipcNode {
    public abstract record Type :  SwipcNode;
    public record Concrete(string Name, IReadOnlyList<SwipcNode> Template, (bool Present, ulong? Length) Array) : Type {
        public override string ToString() =>
            $"Concrete {{ Name = {Name}, Template = {(Template == null ? "" : string.Join(", ", Template))}, Array = {Array} }}";
    }
    public record Enum(IReadOnlyList<SwipcNode> Template, IReadOnlyList<(string Name, ulong Value)> Members) : Type {
        public override string ToString() =>
            $"Enum {{ Template = {(Template == null ? "" : string.Join(", ", Template))}, Members = [\n{Indent(string.Join("\n", Members.Select(x => x.ToString())))}\n] }}";
    }
    public record Struct(IReadOnlyList<SwipcNode> Template, IReadOnlyList<(Type Type, string Name)> Members) : Type {
        public override string ToString() =>
            $"Struct {{ Template = {(Template == null ? "" : string.Join(", ", Template))}, Members = [\n{Indent(string.Join("\n", Members.Select(x => x.ToString())))}\n] }}";
    }
    
    public record Typedef(Versions Versions, string Name, Type Of) : SwipcNode;
    public record Interface(Versions Versions, string Name, IReadOnlyList<string> ServiceNames, IReadOnlyList<Function> Functions) : SwipcNode {
        public override string ToString() =>
            $"Interface {{ Versions = {Versions}, Name = {Name}, ServiceNames = [{string.Join(", ", ServiceNames)}], Functions = [\n{Indent(string.Join("\n", Functions.Select(x => x.ToString())))}\n] }}";
    }
    public record Function(Versions Versions, string Name, ulong CmdId, IReadOnlyList<(Type Type, string Name)> Inputs, IReadOnlyList<(Type Type, string Name)> Outputs) : SwipcNode {
        public override string ToString() =>
            $"Function {{ Versions = {Versions}, Name = {Name}, CmdId = {CmdId}, Inputs=[{string.Join(", ", Inputs)}], Outputs=[{string.Join(", ", Outputs)}] }}";
    }
    
    public record Number(ulong Value) : SwipcNode;
    
    static string Indent(string text) => string.Join('\n', text.Split('\n').Select(x => $"\t{x}"));
}

public partial class Parser {
    internal Parser() {} // For the grammar classes to be happy

    public readonly IReadOnlyList<SwipcNode.Typedef> Typedefs;
    public readonly IReadOnlyList<SwipcNode.Interface> Interfaces;
    
    public Parser(string defs) {
        var ast = Parse(defs);
        var nodes = ast.Defs.Select(x => Transform(x.Value)).ToList();
        Typedefs = nodes.OfType<SwipcNode.Typedef>().ToList();
        Interfaces = nodes.OfType<SwipcNode.Interface>().ToList();
    }

    static SwipcNode Transform(Parser node) => node switch {
        TypeDef td => new SwipcNode.Typedef(
            ParseVersions(td.decorators),
            td.name.Value, 
            (SwipcNode.Type) Transform(td.type)
        ),
        Type type => Transform(type.Value),
        ConcreteType ct => new SwipcNode.Concrete(
            ct.name.Value,
            Parse(ct.template),
            ct.length == null ? (false, null) : (true, ct.length.length == null ? null : Parse(ct.length.length))
        ),
        StructType st => new SwipcNode.Struct(
            Parse(st.template),
            st.structFields.Select(x => ((SwipcNode.Type) Transform(x.type), x.name.Value)).ToList()
        ),
        EnumType et => new SwipcNode.Enum(
            Parse(et.template),
            et.enumFields.Select(x => (x.name.Value, Parse(x.value))).ToList()
        ),
        Interface i => new SwipcNode.Interface(
            ParseVersions(i.decorators),
            i.name.Value,
            i.serviceNames == null ? [] : i.serviceNames.Head.Concat([i.serviceNames.Tail]).Select(x => x.Value).ToList(),
            i.functions.Select(x => (SwipcNode.Function) Transform(x)).ToList()
        ),
        FuncDef f => new SwipcNode.Function(
            ParseVersions(f.decorators),
            f.name.Value,
            Parse(f.cmdId),
            f.inputs.Tail == null
                ? []
                : f.inputs.Head.Concat([f.inputs.Tail])
                    .Select(x => ((SwipcNode.Type) Transform(x.Value.Item1), x.Value.Item2?.Value)).ToList(),
            f.outputs switch {
                null => [],
                NamedType named => [((SwipcNode.Type) Transform(named.Value.Item1), named.Value.Item2?.Value)],
                NamedTuple nt => nt.Head.Concat([nt.Tail])
                    .Select(x => ((SwipcNode.Type) Transform(x.Value.Item1), x.Value.Item2?.Value)).ToList(),
                _ => throw new NotSupportedException()
            }
        ),
        Expression e => Transform(e.Value),
        Number n => new SwipcNode.Number(Parse(n)),
        _ => throw new NotImplementedException($"{node} is not implemented"),
    };

    static List<SwipcNode> Parse(Template template) =>
        template?.Head.Concat([template.Tail]).Select(Transform).ToList();
    static ulong Parse(Number number) => Convert.ToUInt64(number.Value, number.Value.StartsWith("0x") ? 16 : 10);
    static Versions ParseVersions(List<Parser> decorators) {
        // TODO: Implement
        return new Versions.All();
    }
}