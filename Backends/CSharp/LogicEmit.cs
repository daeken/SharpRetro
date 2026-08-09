using System.Diagnostics;
using ArchCompilerCore;
using LibSharpRetro;
using PrettyPrinter;
using static Backends.CSharp.CSharpEmit;
using static ArchCompilerCore.BuiltinTypes;

namespace Backends.CSharp;

// Emit-lambdas extracted from legacy CoreArchCompiler/Logic.cs
public static class LogicEmit {
    public static void Register() {
        Statement("let",
            (c, list) => {
				c += $"var {list[1]} = {GenerateExpression(list[2])};";
				list.Skip(3).ForEach(x => GenerateStatement(c, (PList) x));
			},
            (c, list) => {
				if(list[2].Type.Runtime)
					c += $"var {list[1]} = ({GenerateExpression(list[2])}).Store();";
				else
					c += $"var {list[1]} = {GenerateExpression(list[2])};";
				list.Skip(3).ForEach(x => GenerateStatement(c, (PList) x));
			});
        Statement("mlet",
            (c, list) => {
				if(list[1] is not PList dlist) throw new NotSupportedException();
				Debug.Assert(dlist.Count % 2 == 0);
				for(var i = 0; i < dlist.Count; i += 2)
					c += $"var {dlist[i]} = {GenerateExpression(dlist[i + 1])};";
				list.Skip(2).ForEach(x => GenerateStatement(c, (PList) x));
			},
            (c, list) => {
				if(list[1] is not PList dlist) throw new NotSupportedException();
				Debug.Assert(dlist.Count % 2 == 0);
				for(var i = 0; i < dlist.Count; i += 2)
					if(dlist[i + 1].Type.Runtime)
						c += $"var {dlist[i]} = ({GenerateExpression(dlist[i + 1])}).Store();";
					else
						c += $"var {dlist[i]} = {GenerateExpression(dlist[i + 1])};";
				list.Skip(2).ForEach(x => GenerateStatement(c, (PList) x));
			});
        Expression("ensure-runtime",
            list => GenerateExpression(list[1]),
            list => $"builder.EnsureRuntime({GenerateExpression(list[1])})");
        Expression("cast",
            list => {
					if(list[1].Type.ToString() == list.Type.ToString()) return GenerateExpression(list[1]);
					if(Context == ContextTypes.Recompiler && list[1].Type.Runtime) {
						if(list.Type is EBool)
							return $"({GenerateExpression(list[1])}) != builder.Zero<{GenerateType(list[1].Type.AsCompiletime())}>()";
						if(list[1].Type is EBool)
							return $"({GenerateType(list.Type)}) builder.Ternary({GenerateExpression(list[1])}, builder.LiteralValue(1U), builder.Zero<uint>())";
					} else {
						if(list.Type is EBool)
							return $"({GenerateExpression(list[1])}) != 0";
						if(list[1].Type is EBool)
							return $"({GenerateType(list.Type)}) (({GenerateExpression(list[1])}) ? 1U : 0U)";
					}
					return $"({GenerateType(list.Type)}) ({GenerateExpression(list[1])})";
				});
        Expression("bitcast",
            list => $"Math.Bitcast<{GenerateType(list[1].Type)}, {GenerateType(list.Type)}>({GenerateExpression(list[1])})",
            list => $"({GenerateExpression(list[1])}).Bitcast<{GenerateType(list.Type.AsCompiletime())}>()");

			
		T SignExt<T>(ulong value, int size) {
			if(typeof(T) == typeof(long))
				return (T) (object) ((value & (1UL << (size - 1))) != 0 ? (long) value - (1L << size) : (long) value);
			if(typeof(T) == typeof(int))
				return (T) (object) ((value & (1UL << (size - 1))) != 0 ? (int) value - (1 << size) : (int) value);
			throw new NotImplementedException($"Unknown return for SignExt: {typeof(T)}");
		}

        Expression("signext",
            list => $"Math.SignExt<{GenerateType(list.Type)}>({GenerateExpression(list[1])}, {((EInt) list[1].Type).Width})",
            list => $"({GenerateExpression(list[1])}).SignExt<{GenerateType(list.Type.AsCompiletime())}>({((EInt) list[1].Type).Width})");
        Expression(new[] { "==", "!=", ">", ">=", "<=", "<" },
            list => {
					list = list.HomogeneousRuntime();
					var runtime = list.AnyRuntime;
					var lhs = list[1];
					var rhs = list[2];
					var lhe = GenerateExpression(lhs);
					var rhe = GenerateExpression(rhs);
					if(lhs.Type is EInt(var lsigned, var lsize) && rhs.Type is EInt(var rsigned, var rsize)) {
						if(!lsigned && rsigned) lhe = $"({GenerateType(new EInt(true, lsize).AsRuntime(runtime))}) ({lhe})";
						if(lsigned && !rsigned) rhe = $"({GenerateType(new EInt(true, rsize).AsRuntime(runtime))}) ({rhe})";
						var signed = lsigned || rsigned;
						if(lsize < rsize) lhe = $"({GenerateType(new EInt(signed, rsize).AsRuntime(runtime))}) ({lhe})";
						if(rsize < lsize) rhe = $"({GenerateType(new EInt(signed, lsize).AsRuntime(runtime))}) ({rhe})";
					}
					return $"({lhe}) {list[0]} ({rhe})";
				});
    }
}
