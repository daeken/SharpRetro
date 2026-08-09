using System.Diagnostics;
using ArchCompilerCore;
using Extensions = ArchCompilerCore.Extensions;
using DoubleSharp.Linq;
using LibSharpRetro;
using PrettyPrinter;
using static Backends.CSharp.CSharpEmit;
using static ArchCompilerCore.BuiltinTypes;

namespace Backends.CSharp;

// Emit-lambdas extracted from legacy CoreArchCompiler/ControlFlow.cs
public static class ControlFlowEmit {
    public static void Register() {

		PTree EnsureBool(PTree tree) => tree.Cast<bool>();
		
        Statement("requires",
            (c, list) => {
				c += $"if({string.Join(" || ", list.Skip(1).Select(x => $"!({GenerateExpression(EnsureBool(x))})"))})";
				c++;
				c += $"goto {NextLabel};";
				c--;
			});
        Statement("block",
            (c, list) => LinqExtensions.ForEach(list.Skip(1), x => GenerateStatement(c, (PList) x)));
        Expression("block",
            list => $@"LibSharpRetro.FunctionalHelpers.Funcify(() => {{
{string.Join('\n', list.Skip(1).Select((x, i) => {
	string code;
	if(x is PList xl) {
		var c = new CodeBuilder();
		GenerateStatement(c, xl);
		code = c.Code;
	} else
		code = $"({GenerateExpression(x)});";
	if(i == list.Count - 2)
		return $"\t\treturn ({GenerateType(list.Type)}) {code.Trim()}";
	return $"\t\t{code.Trim()}";
}))}
	}})()",
            list => $@"LibSharpRetro.FunctionalHelpers.Funcify(() => {{
{string.Join('\n', list.Skip(1).Select((x, i) => {
	string code;
	if(x is PList xl) {
		var c = new CodeBuilder();
		GenerateStatement(c, xl);
		code = c.Code;
	} else
		code = GenerateExpression(x) + ";";
	if(i == list.Count - 2)
		return $"\t\treturn ({code.Trim().TrimEnd(';')}).Store();";
	return $"\t\t{code.Trim()}";
}))}
	}})()");
        Statement("if",
            (c, list) => {
				c += $"if({GenerateExpression(EnsureBool(list[1]))}) {{";
				c++;
				GenerateStatement(c, (PList) list[2]);
				c--;
				c += "} else {";
				c++;
				GenerateStatement(c, (PList) list[3]);
				c--;
				c += "}";
			},
            (c, list) => {
				if(list[1].Type.Runtime) {
					c += "builder.If(";
					c++;
					c += $"{GenerateExpression(EnsureBool(list[1]))}, ";
					c += "() => {";
					c++;
					GenerateStatement(c, (PList) list[2]);
					c--;
					c += "}, ";
					c += "() => {";
					c++;
					GenerateStatement(c, (PList) list[3]);
					c--;
					c += "});";
					c--;
				} else {
					c += $"if({GenerateExpression(EnsureBool(list[1]))}) {{";
					c++;
					GenerateStatement(c, (PList) list[2]);
					c--;
					c += "} else {";
					c++;
					GenerateStatement(c, (PList) list[3]);
					c--;
					c += "}";
				}
			});
        Expression("if",
            list => {
			var a = GenerateExpression(list[2]);
			var b = GenerateExpression(list[3]);
			if(!a.StartsWith("throw")) a = $"({a})";
			if(!b.StartsWith("throw")) b = $"({b})";
			var at = list[2].Type;
			var bt = list[3].Type;
			// Special cases for unimplemented -- void
			if(at is EUnit || bt is EUnit)
				return $"({GenerateExpression(EnsureBool(list[1]))}) ? {a} : {b}";
			string type;
			if(at == bt || at is not EInt(var asigned, var asized) || bt is not EInt(var bsigned, var bsized))
				type = GenerateType(at);
			else
				type = GenerateType(new EInt(asigned && bsigned, Math.Max(asized, bsized)));
			return $"({GenerateExpression(EnsureBool(list[1]))}) ? ({type}) {a} : ({type}) {b}";
		},
            list => {
			var a = GenerateExpression(list[2]);
			var b = GenerateExpression(list[3]);
			var at = list[2].Type;
			var bt = list[3].Type;
			// Special cases for unimplemented -- void
			if(at is EUnit || bt is EUnit) {
				if(list[1].Type.Runtime) throw new NotImplementedException();
				return $"({GenerateExpression(EnsureBool(list[1]))}) ? {a} : {b}";
			}

			string type;
			if(at == bt || at is not EInt(var asigned, var asized) || bt is not EInt(var bsigned, var bsized))
				type = GenerateType(at.AsRuntime(at.Runtime || bt.Runtime));
			else
				type = GenerateType(new EInt(asigned && bsigned, Math.Max(asized, bsized)).AsRuntime(at.Runtime || bt.Runtime));
			
			if(list[1].Type.Runtime) {
				if(a.StartsWith("throw")) a = "null";
				if(b.StartsWith("throw")) b = "null";

				if(!type.StartsWith("IRuntimeValue"))
					type = $"IRuntimeValue<{type}>";
				
				return $"builder.Ternary({GenerateExpression(EnsureBool(list[1]))}, ({type}) builder.EnsureRuntime({a}), ({type}) builder.EnsureRuntime({b}))";
			}
				
			if(!a.StartsWith("throw")) a = $"({a})";
			if(!b.StartsWith("throw")) b = $"({b})";
			return $"({GenerateExpression(EnsureBool(list[1]))}) ? ({type}) builder.EnsureRuntime({a}) : ({type}) builder.EnsureRuntime{b}";
		});


		Interpret("if", (list, state) => Extensions.AsBool(state.Evaluate(list[1])) ? state.Evaluate(list[2]) : state.Evaluate(list[3]));
			
        Statement("for",
            (c, list) => {
				if(list[1] is not PList dlist || dlist[0] is not PName vname) throw new NotSupportedException();
				int start = 0, end = 0, step = 1;
				var name = vname.Name;
				if(dlist.Count == 2) {
					if (dlist[1] is not PInt ei) throw new NotSupportedException();
					end = (int) ei.Value;
				} else if(dlist.Count == 3) {
					if (dlist[1] is not PInt si || dlist[2] is not PInt ei) throw new NotSupportedException();
					start = (int) si.Value;
					end = (int) ei.Value;
				} else if(dlist.Count == 4) {
					if(dlist[1] is not PInt si || dlist[2] is not PInt ei || dlist[3] is not PInt ti)
						throw new NotSupportedException();
					start = (int) si.Value;
					end = (int) ei.Value;
					step = (int) ti.Value;
				}
				else
					throw new NotSupportedException();

				for(var i = start; i < end; i += step) {
					var pi = new PInt(i);
					pi.Type = new EInt(true, 32);
					LinqExtensions.ForEach(list.Skip(2), x => GenerateStatement(c, ((PList) x).MapLeaves(y => y is PName pn && pn.Name == name ? pi : y)));
				}
			});
        Statement("when",
            (c, list) => {
				c += $"if({GenerateExpression(EnsureBool(list[1]))}) {{";
				c++;
				LinqExtensions.ForEach(list.Skip(2), x => GenerateStatement(c, (PList) x));
				c--;
				c += "}";
			},
            (c, list) => {
				if(list[1].Type.Runtime) {
					c += "builder.When(";
					c++;
					c += $"{GenerateExpression(EnsureBool(list[1]))}, ";
					c += "() => {";
					c++;
					LinqExtensions.ForEach(list.Skip(2), x => GenerateStatement(c, (PList) x));
					c--;
					c += "});";
					c--;
				} else {
					c += $"if({GenerateExpression(EnsureBool(list[1]))}) {{";
					c++;
					LinqExtensions.ForEach(list.Skip(2), x => GenerateStatement(c, (PList) x));
					c--;
					c += "}";
				}
			});


		void SwitchGen(CodeBuilder c, PList list) {
			c += $"switch({GenerateExpression(list[1])}) {{";
			c++;
			for(var i = 2; i < list.Count; i += 2)
				if(i + 1 == list.Count) {
					c += "default: {";
					c++;
					GenerateStatement(c, (PList) list[i]);
					c += "break;";
					c--;
					c += "}";
				} else {
					c += $"case ({GenerateType(list[1].Type)}) ({GenerateExpression(list[i])}): {{";
					c++;
					GenerateStatement(c, (PList) list[i + 1]);
					c += "break;";
					c--;
					c += "}";
				}
			c--;
			c += "}";
		}
        Statement("match",
            SwitchGen,
            (c, list) => {
				var isRuntime = list[1].Type.Runtime;
				for(var i = 2; !isRuntime && i < list.Count; i += 2) {
					if(list.Count - 1 == i) break;
					isRuntime = list[i].Type.Runtime;
				}
				if(Context != ContextTypes.Recompiler || !isRuntime) {
					SwitchGen(c, list);
					return;
				}

				var mtype = $"IRuntimeValue<{GenerateType(list[1].Type.AsCompiletime())}>";
				c += $"builder.Switch(builder.EnsureRuntime({GenerateExpression(list[1])}), ";
				c += 2;
				for(var i = 2; i < list.Count; i += 2) {
					var isDefault = i + 1 == list.Count;
					c += $"({(isDefault ? "null" : $"({mtype}) builder.EnsureRuntime({GenerateExpression(list[i])})")}, () => {{";
					c++;
					GenerateStatement(c, (PList) list[i + (isDefault ? 0 : 1)]);
					c--;
					c += $"}}){(i + 2 >= list.Count ? "" : ",")}";
				}
				c--;
				c += ");";
				c--;
			});


		string MatchGen(PList list) {
			var rtype = list.Count == 3 ? list[2].Type : list[3].Type;
			var rs = GenerateType(rtype);

			string Expr(PTree slist) {
				var repr = GenerateExpression(slist);
				if(rtype.Runtime && !slist.Type.Runtime && !repr.StartsWith("throw "))
					repr = $"builder.LiteralValue({repr})";
				if(slist.Type == rtype || repr.StartsWith("throw "))
					return repr;
				return $"({rs}) ({repr})";
			}
			
			var opts = new List<string>();
			for(var i = 2; i < list.Count; i += 2)
				opts.Add(i + 1 == list.Count
					? $"_ => {Expr(list[i])}"
					: $"({GenerateType(list[1].Type)}) ({GenerateExpression(list[i])}) => {Expr(list[i + 1])}");
			var tn = TempName();
			return $"{GenerateExpression(list[1])} switch {{ {string.Join(", ", opts)} }}";
		}

        Expression("match",
            MatchGen,
            list => {
				var isRuntime = list[1].Type.Runtime;
				for(var i = 2; !isRuntime && i < list.Count; i += 2) {
					if(list.Count - 1 == i) break;
					isRuntime |= list[i].Type.Runtime;
				}
				if(Context != ContextTypes.Recompiler || !isRuntime)
					return MatchGen(list);

				var mtype = $"IRuntimeValue<{GenerateType(list[1].Type.AsCompiletime())}>";
				var c = $"builder.Switch(builder.EnsureRuntime({GenerateExpression(list[1])}), ";
				for(var i = 2; i < list.Count; i += 2) {
					var isDefault = i + 1 == list.Count;
					c += $"({(isDefault ? "null" : $"({mtype}) builder.EnsureRuntime({GenerateExpression(list[i])})")}, () => ";
					c += GenerateExpression(list[i + (isDefault ? 0 : 1)]);
					c += $"){(i + 2 >= list.Count ? "" : ", ")}";
				}
				c += ")";
				return c;
			});


		Interpret("match", (list, state) => {
			var mv = state.Evaluate(list[1]);
			for(var i = 2; i < list.Count; i += 2) {
				if(i + 1 < list.Count) {
					var cv = state.Evaluate(list[i]);
					var mcond = false;
					try {
						mcond = cv == mv;
					} catch(Exception) {
						mcond = (ulong) cv == (ulong) mv;
					}
					if(mcond)
						return state.Evaluate(list[i + 1]);
				} else
					return state.Evaluate(list[i]);
			}
			throw new BailoutException(); // This can only be hit if nothing matches and there's no default case
		});

        BranchExpression("branch",
            list => $"Branch({GenerateExpression(list[1])})");
        Statement("assert",
            (_, _) => { });
        Expression("unimplemented",
            _ => "throw new NotImplementedException()");
    }
}
