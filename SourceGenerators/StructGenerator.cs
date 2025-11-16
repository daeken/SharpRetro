using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenerators;

[Generator]
public class StructGenerator : ISourceGenerator {
	public void Initialize(GeneratorInitializationContext context) =>
		context.RegisterForSyntaxNotifications(() => new MyReceiver());
	
	public void Execute(GeneratorExecutionContext context) {
		var sr = (MyReceiver) context.SyntaxContextReceiver ?? throw new();

		var errorI = 0;
		void Error(string message) => context.AddSource($"error{errorI++}.cs", $"#error {message}");

		var fields = new List<(string StructName, string FieldName, string FieldType, string Offset)>();
		var arrFields = new List<(string StructName, string FieldName, string FieldType, string Offset)>();
		
		foreach(var sds in sr.Structs) {
			if(sds.BaseList == null || !sds.BaseList.Types.Any()) continue;

			if(context.Compilation.GetSemanticModel(sds.SyntaxTree).GetDeclaredSymbol(sds) is not ITypeSymbol typeSymbol) continue;
			if(!typeSymbol.Interfaces.Any(x => x.ToDisplayString() == "JitBase.IJitStruct")) continue;
			
			var sn = typeSymbol.ToDisplayString();
			
			foreach(var member in sds.Members) {
				if(member is FieldDeclarationSyntax fds) {
					var fld = fds.Declaration.Variables.First();
					var isArr = fld.ArgumentList != null;
					var ft = fds.Declaration.Type;
					if(!isArr && ft is ArrayTypeSyntax ats) {
						isArr = true;
						ft = ats.ElementType;
					}
					var type = ft.ToFullString();
					var name = fld.Identifier.ValueText;
					var attr = fds.AttributeLists.SelectMany(x => x.Attributes).First(y => y.Name.ToString() == "FieldOffset");
					var arg = attr.ArgumentList?.Arguments[0];
					var offset = attr.ArgumentList?.Arguments[0].ToFullString();
					(isArr ? arrFields : fields).Add((sn, name, type, offset));
				}
			}
		}

		var members = "";
		foreach(var (sn, fn, ft, offset) in fields) {
			members += $"\t\textension(JitBase.IStructRef<{sn}> sr) {{\n";
			members += $"\t\t\tpublic IRuntimeValue<{ft}> {fn} {{\n";
			members += $"\t\t\t\tget => sr.GetField<{ft}>(\"{fn}\", (ulong) ({offset}));\n";
			members += $"\t\t\t\tset => sr.SetField(\"{fn}\", (ulong) ({offset}), value);\n";
			members += "\t\t\t}\n";
			members += "\t\t}\n";
		}
		foreach(var (sn, fn, ft, offset) in arrFields) {
			members += $"\t\textension(JitBase.IStructRef<{sn}> sr) {{\n";
			members += $"\t\t\tpublic RuntimeIndexer<{sn}, {ft}> {fn} => new(\"{fn}\", sr, (ulong) ({offset}));\n";
			members += "\t\t}\n";
			//members += $"\t\tpublic static IRuntimeValue<{ft}> {fn}(this JitBase.IStructRef<{sn}> sr, IRuntimeValue<int> index) => sr.GetFieldElement<{ft}>((ulong) ({offset}), index);\n";
			//members += $"\t\tpublic static void {fn}(this JitBase.IStructRef<{sn}> sr, IRuntimeValue<int> index, IRuntimeValue<{ft}> value) => sr.SetFieldElement((ulong) ({offset}), index, value);\n";
		}
		
		context.AddSource("JitStructExtensions.g.cs", $@"
using System.Runtime.Intrinsics;
namespace JitBase {{
	public static class JitStructExtensions {{
{members}	}}
}}
");
	}

	class MyReceiver : ISyntaxContextReceiver {
		internal readonly List<TypeDeclarationSyntax> Structs = [];

		public void OnVisitSyntaxNode(GeneratorSyntaxContext context) {
			if(context.Node is TypeDeclarationSyntax tds) {
				if(tds.Keyword.ValueText == "struct")
					Structs.Add(tds);
			}
		}
	}
}
