using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UmbraGenerators;

[Generator]
public class HookGenerator : ISourceGenerator {
    static readonly DiagnosticDescriptor MustBePublic =
        new(id: "HOOK001",
            title: "Hook method must be public",
            messageFormat: "Hook method '{0}' must be public",
            category: "HookGen",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    static readonly DiagnosticDescriptor MustBeStatic =
        new(id: "HOOK002",
            title: "Hook method must be static",
            messageFormat: "Hook method '{0}' must be static",
            category: "HookGen",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    
    public void Initialize(GeneratorInitializationContext context) =>
        context.RegisterForSyntaxNotifications(() => new MyReceiver());

    public void Execute(GeneratorExecutionContext context) {
        var mr = (MyReceiver) context.SyntaxContextReceiver ?? throw new();
        var hooks = new List<string>();
        foreach(var mds in mr.Methods) {
            if(context.Compilation.GetSemanticModel(mds.SyntaxTree).GetDeclaredSymbol(mds) is not IMethodSymbol ims)
                continue;
            var attr = ims.GetAttributes()
                .FirstOrDefault(x => $"{x.AttributeClass!.ToDisplayString()}" == "UmbraCore.Kernel.Hook");
            if(attr == null) continue;
            if(!ims.DeclaredAccessibility.HasFlag(Accessibility.Public)) {
                context.ReportDiagnostic(Diagnostic.Create(MustBePublic, mds.GetLocation(), ims.Name));
                continue;
            }
            if(!ims.IsStatic) {
                context.ReportDiagnostic(Diagnostic.Create(MustBeStatic, mds.GetLocation(), ims.Name));
                continue;
            }

            var symbol = attr.ConstructorArguments[0].Value as string;
            hooks.Add(GenerateHook(
                $"{ims.ContainingType.ToDisplayString()}.{ims.Name}",
                symbol, ims.ReturnType, ims.Parameters.Select(x => x.Type).ToList()));
        }
        
        context.AddSource("HookWrappers.g.cs", $@"
namespace UmbraCore.Kernel;
public partial class HookManager {{
	public unsafe void InitializeWrappers() {{
{string.Join("\n", hooks)}
	}}
}}
");
    }

    string GenerateHook(string name, string symbol, ITypeSymbol returnType, List<ITypeSymbol> args) {
        string Canon(ITypeSymbol type) => type.ToDisplayString();
        
        var code = $"\t\tRegister(\"{symbol}\", cpuState => {{\n";
        var argCode = new List<string>();
        var call = $"{name}({string.Join(", ", argCode)})";
        switch(Canon(returnType)) {
            case "void":
                code += $"\t\t\t{call};\n";
                break;
            case "byte": case "ushort": case "uint": case "ulong":
                code += $"\t\t\tcpuState->X0 = (ulong) {call};\n";
                break;
            case "sbyte":
                code += $"\t\t\tcpuState->X0 = (ulong) unchecked((byte) {call});\n";
                break;
            case "short":
                code += $"\t\t\tcpuState->X0 = (ulong) unchecked((ushort) {call});\n";
                break;
            case "int":
                code += $"\t\t\tcpuState->X0 = (ulong) unchecked((uint) {call});\n";
                break;
            case "long":
                code += $"\t\t\tcpuState->X0 = unchecked((ulong) {call});\n";
                break;
            case "System.UInt128":
                code += $"\t\t\tvar temp = {call};\n";
                code += "\t\t\tcpuState->X0 = unchecked((ulong) temp);\n";
                code += "\t\t\tcpuState->X1 = (ulong) (temp >> 64);\n";
                break;
            case "System.Int128":
                code += $"\t\t\tvar temp = unchecked((System.UInt128) {call});\n";
                code += "\t\t\tcpuState->X0 = unchecked((ulong) temp);\n";
                code += "\t\t\tcpuState->X1 = (ulong) (temp >> 64);\n";
                break;
            default:
                code += $"#error Unsupported hook return type {Canon(returnType)}\n";
                break;
        }
        code += "\t\t\treturn cpuState->X30;\n";
        return code + "\t\t});";
    }
    
    class MyReceiver : ISyntaxContextReceiver {
        internal readonly List<MethodDeclarationSyntax> Methods = [];

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context) {
            if(context.Node is MethodDeclarationSyntax { AttributeLists.Count: > 0 } mds)
                Methods.Add(mds);
        }
    }
}
