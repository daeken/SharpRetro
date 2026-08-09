using System.Diagnostics;
using ArchCompilerCore;
using LibSharpRetro;
using PrettyPrinter;
using static Backends.CSharp.CSharpEmit;
using static ArchCompilerCore.BuiltinTypes;

namespace Backends.CSharp;

// Emit-lambdas extracted from legacy CoreArchCompiler/ScalarMath.cs
public static class ScalarMathEmit {

	static EType FirstType(PList list) => list[1].Type.AsRuntime(list.AnyRuntime);
	static EType LogicalBool(PList list) {
		var allBool = list.All(x => x.Type is EBool) || (list.Any(x => x.Type is EBool) &&
		                                                 list.All(x =>
			                                                 x.Type is EBool or EInt(signed: false, width: 1)));
		return (allBool
				? new EBool()
				: list[1].Type)
			.AsRuntime(list.AnyRuntime);
	}

	static EType LogicalType(EType a, EType b) {
		if(a is EInt || b is EInt) {
			if(a is not EInt ai) throw new NotSupportedException("Logical expression contains lhs that is non-int");
			if(b is not EInt bi) throw new NotSupportedException("Logical expression contains rhs that is non-int");
			return new EInt(
				ai.Signed == bi.Signed && ai.Signed,
				Math.Max(ai.Width, bi.Width)
			) { Runtime = ai.Runtime || bi.Runtime };
		}
		if(a is EFloat || b is EFloat) {
			if(a is not EFloat af) throw new NotSupportedException("Logical expression contains lhs that is non-float");
			if(b is not EFloat bf) throw new NotSupportedException("Logical expression contains rhs that is non-float");
			return new EFloat(Math.Max(af.Width, bf.Width)) { Runtime = af.Runtime || bf.Runtime };
		}
		throw new NotImplementedException("Logical expression has non-int/non-float type");
	}

	static EType LogicalType(PList list) => list.Skip(1).Select(x => x.Type).Aggregate(LogicalType);

    public static void Register() {
        Expression(new[] {"+", "-", "*", "/", "%"},
            list => {
				Debug.Assert(list.Count == 3);
				list = list.HomogeneousRuntime();
				if(list[1].Type is EInt(var sa, var ba) && list[2].Type is EInt(var sb, var bb)) {
					var stype = new EInt(sa && sb, Math.Max(ba, bb))
						{Runtime = list[1].Type.Runtime || list[2].Type.Runtime};
					return
						$"(({GenerateType(stype)}) ({GenerateType(list[1].Type.AsRuntime(list.Type.Runtime))}) ({GenerateExpression(list[1])})) {list[0]} (({GenerateType(stype)}) ({GenerateType(list[2].Type.AsRuntime(list.Type.Runtime))}) ({GenerateExpression(list[2])}))";
				}

				if(list[1].Type is EFloat(var wa) && list[2].Type is EFloat(var wb)) {
					var stype = new EFloat(Math.Max(wa, wb))
						{Runtime = list[1].Type.Runtime || list[2].Type.Runtime};
					return
						$"(({GenerateType(stype)}) ({GenerateType(list[1].Type.AsRuntime(list.Type.Runtime))}) ({GenerateExpression(list[1])})) {list[0]} (({GenerateType(stype)}) ({GenerateType(list[2].Type.AsRuntime(list.Type.Runtime))}) ({GenerateExpression(list[2])}))";
				}

				throw new NotImplementedException();
			});
        Expression(new[] { "|", "&", "^" },
            list => {
				list = list.HomogeneousRuntime();
				var signed = true;
				var size = 0;
				var rtype = LogicalBool(list);
				foreach(var _elem in list.Skip(1)) {
					var elem = _elem;
					if(rtype is not EBool && elem.Type is EBool)
						elem = elem.Cast<byte>();
					if(elem.Type is EInt(var s, var ba)) {
						signed = signed && s;
						size = Math.Max(size, ba);
					} else if(rtype is not EBool)
						throw new NotImplementedException($"Expression {list} should not see type {elem.Type} in {elem}");
				}

				return list.Skip(1).Select(x => $"({GenerateExpression(x.Cast(rtype))})")
					.Aggregate((x1, x2) => $"({x1} {list[0]} {x2})");
			});
        Expression("~",
            list => $"~({GenerateExpression(list[1])})");
        Expression("-!",
            list => $"-({GenerateExpression(list[1])})");
        Expression("!",
            list => $"!({GenerateExpression(list[1].Cast<bool>())})",
            list => $"!({GenerateExpression(list[1].Cast<bool>())})");
        Expression("<<",
            list => $"({GenerateExpression(list[1])}) << (int) ({GenerateExpression(list[2])})",
            list => $"({GenerateExpression(list[1])}).LeftShift(({GenerateType(list[1].Type)}) builder.EnsureRuntime({GenerateExpression(list[2])}))");
        Expression(">>",
            list => $"({GenerateExpression(list[1])}) >> (int) ({GenerateExpression(list[2])})",
            list => $"({GenerateExpression(list[1])}).RightShift(({GenerateType(list[1].Type)}) builder.EnsureRuntime({GenerateExpression(list[2])}))");
        Expression(">>>",
            list => {
				if(list[1].Type is not EInt(false, var bs)) throw new NotSupportedException();
				return
					$"(({GenerateExpression(list[1])}) << ({bs} - (int) ({GenerateExpression(list[2])}))) | (({GenerateExpression(list[1])}) >> (int) ({GenerateExpression(list[2])}))";
			},
            list => {
				if(list[1].Type is not EInt(false, var bs)) throw new NotSupportedException();
				if(list[2].Type.Runtime)
					return
						$"(({GenerateExpression(list[1])}).LeftShift(({GenerateType(list[1].Type)}) (({GenerateType(list[2].Type)}) builder.EnsureRuntime({bs}) - builder.EnsureRuntime({GenerateExpression(list[2])}))))) | (({GenerateExpression(list[1])}).RightShift(({GenerateType(list[1].Type)}) builder.EnsureRuntime({GenerateExpression(list[2])}))";
				return
					$"(({GenerateExpression(list[1])}).LeftShift(({GenerateType(list[1].Type)}) builder.EnsureRuntime({bs} - ({GenerateExpression(list[2])}))))) | (({GenerateExpression(list[1])}).RightShift(({GenerateType(list[1].Type)}) builder.EnsureRuntime({GenerateExpression(list[2])}))";
			});
        Expression("reverse-bits",
            list => $"Math.ReverseBits({GenerateExpression(list[1])})",
            list => $"({GenerateExpression(list[1])}).ReverseBits()");
 // TODO: Implement

        Expression("count-leading-zeros",
            list => $"Math.CountLeadingZeros({GenerateExpression(list[1])})",
            list => $"({GenerateExpression(list[1])}).CountLeadingZeros()");
 // TODO: Implement

        Expression(":",
            list => {
				var offset = 0;
				return list.Skip(1).Reverse().Select(x => {
					if(x.Type is EBool) {
						var bret = $"((({GenerateType(list.Type)}) (({GenerateExpression(x)}) ? 1U : 0U)) << {offset})";
						offset++;
						return bret;
					}
					if(x.Type is not EInt(_, var width)) throw new NotSupportedException();
					var ret = $"((({GenerateType(list.Type)}) ({GenerateExpression(x)})) << {offset})";
					offset += width;
					return ret;
				}).Aggregate((a, x) =>
					$"({GenerateType(list.Type)}) ((({GenerateType(list.Type)}) {a}) | (({GenerateType(list.Type)}) {x}))");
			});
        Expression("replicate",
            list => {
				if(list[1].Type is not EInt(_, var width)) throw new NotSupportedException();
				if(list[2] is not PInt(var count)) throw new NotSupportedException();
				return Enumerable.Range(0, (int) count)
					.Select(i => $"((({GenerateType(list.Type)}) ({GenerateExpression(list[1])})) << {i * width})")
					.Aggregate((a, x) =>
						$"({GenerateType(list.Type)}) ((({GenerateType(list.Type)}) {a}) | (({GenerateType(list.Type)}) {x}))");
			});
        Expression("abs",
            list => list[1].Type switch {
					EFloat(_) => $"fabs({GenerateExpression(list[1])})", 
					_ => throw new NotSupportedException()
				},
            list => $"({GenerateExpression(list[1])}).Abs()");
        Expression("sqrt",
            list => $"({GenerateType(list.Type)}) sqrt((double) ({GenerateExpression(list[1])}))",
            list => $"({GenerateType(list.Type)}) (({GenerateType(new EFloat(64).AsRuntime(list[1].Type.Runtime))}) ({GenerateExpression(list[1])})).Sqrt()");
        Expression("round",
            list => $"round{(list[1].Type is EFloat(32) ? "f" : "")}({GenerateExpression(list[1])})",
            list => $"({GenerateExpression(list[1])}).Round()");
        Expression("round-toward-zero",
            list => $"roundTowardZero{(list[1].Type is EFloat(32) ? "f" : "")}({GenerateExpression(list[1])})",
            list => $"({GenerateExpression(list[1])}).RoundTowardZero()");
        Expression("round-half-down",
            list => $"ceil{(list[1].Type is EFloat(32) ? "f" : "")}(({GenerateExpression(list[1])}) - 0.5{(list[1].Type is EFloat(32) ? "f" : "")})",
            list => $"({GenerateExpression(list[1])}).RoundHalfDown()");
        Expression("round-half-up",
            list => $"floor{(list[1].Type is EFloat(32) ? "f" : "")}(({GenerateExpression(list[1])}) + 0.5{(list[1].Type is EFloat(32) ? "f" : "")})",
            list => $"({GenerateExpression(list[1])}).RoundHalfUp()");
        Expression("ceil",
            list => $"ceil{(list[1].Type is EFloat(32) ? "f" : "")}({GenerateExpression(list[1])})",
            list => $"({GenerateExpression(list[1])}).Ceil()");
        Expression("floor",
            list => $"floor{(list[1].Type is EFloat(32) ? "f" : "")}({GenerateExpression(list[1])})",
            list => $"({GenerateExpression(list[1])}).Floor()");
        Expression("bitwidth",
            list => {
				switch(TypeFromName(list[1])) {
					case EInt(_, var iwidth): return iwidth.ToString();
					case EFloat(var fwidth): return fwidth.ToString();
					case EVector: return "128";
					default: throw new NotSupportedException(list[1].Type.ToString());
				}
		});
        Expression("NaN?",
            list => $"isnan({GenerateExpression(list[1])}) ? 1U : 0U",
            list => $"({GenerateExpression(list[1])}).IsNaN()");
        Expression("literal",
            list => GenerateExpression(new PInt((long) new ExecutionState().Evaluate(list[1])) { Type = list[1].Type }));
    }
}
