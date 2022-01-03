using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenerators;

[Generator]
public class StructGenerator : ISourceGenerator {
	public void Initialize(GeneratorInitializationContext context) =>
		context.RegisterForSyntaxNotifications(() => new MyReceiver());
	
	public void Execute(GeneratorExecutionContext context) {
		var sr = (MyReceiver) context.SyntaxContextReceiver ?? throw new Exception();

		var errorI = 0;
		void Error(string message) => context.AddSource($"error{errorI++}.cs", $"#error {message}");

		var fields = new List<(string StructName, string FieldName, string FieldType, int Offset)>();
		
		foreach(var sds in sr.Structs) {
			if(sds.BaseList == null || !sds.BaseList.Types.Any()) continue;

			if(context.Compilation.GetSemanticModel(sds.SyntaxTree).GetDeclaredSymbol(sds) is not ITypeSymbol typeSymbol) continue;
			if(!typeSymbol.Interfaces.Any(x => x.ToDisplayString() == "JitBase.IJitStruct")) continue;
			
			var sn = typeSymbol.ToDisplayString();
			
			foreach(var member in sds.Members) {
				if(member is FieldDeclarationSyntax fds) {
					var type = fds.Declaration.Type.ToFullString();
					var name = fds.Declaration.Variables.First().Identifier.ValueText;
					var attr = fds.AttributeLists.SelectMany(x => x.Attributes).First(y => y.Name.ToString() == "FieldOffset");
					var offset = int.Parse(attr.ArgumentList?.Arguments[0].ToFullString() ?? throw new Exception());
					fields.Add((sn, name, type, offset));
				}
			}
		}

		var members = "";
		foreach(var (sn, fn, ft, offset) in fields) {
			members += $"\t\tpublic static IRuntimeValue<{ft}> {fn}(this JitBase.IStructRef<{sn}> sr) => sr.GetField<{ft}>({offset}UL);\n";
			members += $"\t\tpublic static void {fn}(this JitBase.IStructRef<{sn}> sr, IRuntimeValue<{ft}> value) => sr.SetField({offset}UL, value);\n";
		}
		
		context.AddSource("Test.g.cs", $@"
namespace JitBase {{
	public static class StructExtensions {{
{members}	}}
}}
");
	}

	class MyReceiver : ISyntaxContextReceiver {
		internal readonly List<TypeDeclarationSyntax> Structs = new();

		public void OnVisitSyntaxNode(GeneratorSyntaxContext context) {
			if(context.Node is TypeDeclarationSyntax tds) {
				if(tds.Keyword.ValueText == "struct")
					Structs.Add(tds);
			}
		}
	}
}
