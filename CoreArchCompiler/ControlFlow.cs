using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DoubleSharp.Linq;

namespace CoreArchCompiler; 

class ControlFlow : Builtin {
	public override void Define() {
		PTree EnsureBool(PTree tree) => tree.Cast<bool>();
		
		Statement("requires", list => EType.Unit,
			(c, list) => {
				c += $"if({string.Join(" || ", list.Skip(1).Select(x => $"!({GenerateExpression(EnsureBool(x))})"))})";
				c++;
				c += $"goto {Core.NextLabel};";
				c--;
			}).Interpret(
			(list, state) =>
				list.Skip(1).Select(x => Extensions.AsBool(state.Evaluate(x))).Aggregate((a, b) => a && b)
					? true
					: throw new BailoutException());
			
		Statement("block", list => list.Last().Type,
				(c, list) => list.Skip(1).ForEach(x => GenerateStatement(c, (PList) x)))
			.Interpret((list, state) => state.Evaluate(list.Skip(1)));
			
		Expression("block", list => list.Last().Type, 
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
	}})()").Interpret((list, state) => state.Evaluate(list.Skip(1)));

		Statement("if",
			list => list[2].Type.AsRuntime(list[1].Type.Runtime ||
			                               list[2].Type is not EUnit && list[2].Type.Runtime ||
			                               list[3].Type is not EUnit && list[3].Type.Runtime),
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
			}, (c, list) => {
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
			
		Expression("if", list => list[2].Type is not EUnit ? list[2].Type : list[3].Type, list => {
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
		}, list => {
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
			return $"({GenerateExpression(EnsureBool(list[1]))}) ? ({type}) {a} : ({type}) {b}";
		});

		Interpret("if", (list, state) => Extensions.AsBool(state.Evaluate(list[1])) ? state.Evaluate(list[2]) : state.Evaluate(list[3]));
			
		Statement("for", _ => EType.Unit,
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
					list.Skip(2).ForEach(x => GenerateStatement(c, ((PList) x).MapLeaves(y => y is PName pn && pn.Name == name ? pi : y)));
				}
			}).Interpret((list, state) => {
			var rlist = (PList) list[1];
			var varName = ((PName) rlist[0]).Name;
			var range = rlist.Skip(1).Select(state.Evaluate).ToList();
			int start = 0, end = 0, step = 1;
			if(range.Count == 1)
				end = (int) range[0];
			else if(range.Count == 2)
				(start, end) = ((int) range[0], (int) range[1]);
			else if(range.Count == 3)
				(start, end, step) = ((int) range[0], (int) range[1], (int) range[2]);
			else
				throw new NotSupportedException();
			var hasPrevious = state.Locals.ContainsKey(varName);
			var preValue = hasPrevious ? state.Locals[varName] : null;
			for(var i = start; i < end; i += step) {
				state.Locals[varName] = i;
				state.Evaluate(list.Skip(2));
			}
			if(hasPrevious)
				state.Locals[varName] = preValue;
			else
				state.Locals.Remove(varName);
			return null;
		});
			
		Statement("when", list => EType.Unit.AsRuntime(list[1].Type.Runtime),
			(c, list) => {
				c += $"if({GenerateExpression(EnsureBool(list[1]))}) {{";
				c++;
				list.Skip(2).ForEach(x => GenerateStatement(c, (PList) x));
				c--;
				c += "}";
			}, (c, list) => {
				if(list[1].Type.Runtime) {
					c += "builder.When(";
					c++;
					c += $"{GenerateExpression(EnsureBool(list[1]))}, ";
					c += "() => {";
					c++;
					list.Skip(2).ForEach(x => GenerateStatement(c, (PList) x));
					c--;
					c += "});";
					c--;
				} else {
					c += $"if({GenerateExpression(EnsureBool(list[1]))}) {{";
					c++;
					list.Skip(2).ForEach(x => GenerateStatement(c, (PList) x));
					c--;
					c += "}";
				}
			}).Interpret((list, state) => Extensions.AsBool(state.Evaluate(list[1])) ? list.Skip(1).Select(x => state.Evaluate(x)).ToList() : null);

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
		Statement("match", list => list.Count == 3 ? list[2].Type : list[3].Type,
			SwitchGen, 
			(c, list) => {
				var isRuntime = list[1].Type.Runtime;
				for(var i = 2; !isRuntime && i < list.Count; i += 2) {
					if(list.Count - 1 == i) break;
					isRuntime = list[i].Type.Runtime;
				}
				if(Core.Context != ContextTypes.Recompiler || !isRuntime) {
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
			var opts = new List<string>();
			for(var i = 2; i < list.Count; i += 2)
				opts.Add(i + 1 == list.Count
					? $"_ => {GenerateExpression(list[i])}"
					: $"({GenerateType(list[1].Type)}) ({GenerateExpression(list[i])}) => {GenerateExpression(list[i + 1])}");
			var tn = TempName();
			return $"{GenerateExpression(list[1])} switch {{ {string.Join(", ", opts)} }}";
		}

		Expression("match", list => list.Count == 3 ? list[2].Type : list[3].Type,
			MatchGen, 
			list => {
				var isRuntime = list[1].Type.Runtime;
				for(var i = 2; !isRuntime && i < list.Count; i += 2) {
					if(list.Count - 1 == i) break;
					isRuntime |= list[i].Type.Runtime;
				}
				if(Core.Context != ContextTypes.Recompiler || !isRuntime)
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

		BranchExpression("branch", _ => EType.Unit.AsRuntime(), list => $"Branch({GenerateExpression(list[1])})")
			.Interpret((list, state) => state.Registers["PC"] = state.Evaluate(list[1]));

		Statement("assert", _ => EType.Unit, (_, _) => { })
			.Interpret((list, state) => {
				if(!state.Evaluate(list[1])) {
					Console.WriteLine($"Assertion failed {list[1]}: {state.Evaluate(list[2])}");
					Environment.Exit(1);
				}
				return null;
			});
			
		Expression("unimplemented", _ => EType.Unit, _ => "throw new NotImplementedException()").NoInterpret();
	}
}