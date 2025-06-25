using System;
using System.Diagnostics;
using System.Linq;

namespace CoreArchCompiler; 

class ScalarMath : Builtin {
	static EType FirstType(PList list) => list[1].Type.AsRuntime(list.AnyRuntime);
	static EType FirstNonBoolType(PList list) => 
		(list[1].Type is EBool
			? new EInt(false, 1)
			: list[1].Type)
		.AsRuntime(list.AnyRuntime);
		
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
		
	public override void Define() {
		Expression(
			new[] {"+", "-", "*", "/", "%"}, LogicalType,
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
			}).Interpret(
			(list, state) =>
				(state.Evaluate(list[1]), state.Evaluate(list[2])).WithCommonType((a, b) =>
					list[0].AsName() switch {
						"+" => unchecked(a + b), "-" => unchecked(a - b), 
						"*" => unchecked(a * b), "/" => unchecked(a / b), 
						"%" => unchecked(a % b), 
						_ => throw new BailoutException()
					}));

		Expression(
			new[] { "|", "&", "^" }, FirstNonBoolType,
			list => {
				list = list.HomogeneousRuntime();
				var signed = true;
				var size = 0;
				foreach(var _elem in list.Skip(1)) {
					var elem = _elem;
					if(elem.Type is EBool)
						elem = elem.Cast<sbyte>();
					if(elem.Type is not EInt(var s, var ba))
						throw new NotImplementedException($"Expression {list} should not see type {elem.Type} in {elem}");
					signed = signed && s;
					size = Math.Max(size, ba);
				}

				var etype = new EInt(signed, size).AsRuntime(list.AnyRuntime);
				return list.Skip(1).Select(x => $"({GenerateExpression(x.Cast(etype))})")
					.Aggregate((x1, x2) => $"({x1} {list[0]} {x2})");
			}).Interpret(
			(list, state) =>
				list.Skip(2).Aggregate((object) state.Evaluate(list[1]), (al, bl) =>
					(al, state.Evaluate(bl)).WithCommonType((a, b) =>
						list[0].AsName() switch {
							"|" => a | b,
							"&" => a & b,
							"^" => a ^ b,
							_ => throw new BailoutException()
						})));
			
		Expression("~", FirstType, list => $"~({GenerateExpression(list[1])})").Interpret((list, state) => ~state.Evaluate(list[1]));
		Expression("-!", FirstType, list => $"-({GenerateExpression(list[1])})").Interpret((list, state) => -state.Evaluate(list[1]));
		Expression("!", list => new EBool().AsRuntime(list[1].Type.Runtime), 
				list => $"!({GenerateExpression(list[1].Cast<bool>())})", list => $"!({GenerateExpression(list[1].Cast<bool>())})")
			.Interpret((list, state) => !Extensions.AsBool(state.Evaluate(list[1])));
			
		Expression("<<", FirstType, 
				list => $"({GenerateExpression(list[1])}) << (int) ({GenerateExpression(list[2])})", 
				list => $"({GenerateExpression(list[1])}).LeftShift(({GenerateType(list[1].Type)}) builder.EnsureRuntime({GenerateExpression(list[2])}))")
			.Interpret((list, state) => {
				var shift = (int) state.Evaluate(list[2]);
				if(list[1].Type is EInt(_, var size) && shift >= size) return 0;
				return state.Evaluate(list[1]) << shift;
			});
			
		Expression(">>", FirstType, 
				list => $"({GenerateExpression(list[1])}) >> (int) ({GenerateExpression(list[2])})", 
				list => $"({GenerateExpression(list[1])}).RightShift(({GenerateType(list[1].Type)}) builder.EnsureRuntime({GenerateExpression(list[2])}))")
			.Interpret((list, state) => {
				var shift = (int) state.Evaluate(list[2]);
				if(list[1].Type is EInt(var signed, var size) && shift >= size)
					return signed ? 0xFFFFFFFF_FFFFFFFFUL : 0;
				return state.Evaluate(list[1]) >> shift;
			});
			
		Expression(">>>", FirstType,
			list => {
				if(list[1].Type is not EInt(false, var bs)) throw new NotSupportedException();
				return
					$"(({GenerateExpression(list[1])}) << ({bs} - (int) ({GenerateExpression(list[2])}))) | (({GenerateExpression(list[1])}) >> (int) ({GenerateExpression(list[2])}))";
			}, list => {
				if(list[1].Type is not EInt(false, var bs)) throw new NotSupportedException();
				return
					$"(({GenerateExpression(list[1])}) << ((IRuntimeValue<uint>) ({bs} - ({GenerateExpression(list[2])})))) | (({GenerateExpression(list[1])}) >> ((IRuntimeValue<uint>) ({GenerateExpression(list[2])})))";
			}).Interpret((list, state) => {
			var left = state.Evaluate(list[1]);
			var right = (int) state.Evaluate(list[2]);
			if(list[1].Type is not EInt(false, var bs)) throw new NotSupportedException();
			return (left << (bs - right)) | (left >> right);
		});

		Expression("reverse-bits", list => list[1].Type,
				list => $"ReverseBits({GenerateExpression(list[1])})",
				list => $"Call<{GenerateType(list[1].Type.AsCompiletime())}, {GenerateType(list[1].Type.AsCompiletime())}>(ReverseBits, {GenerateExpression(list[1])})")
			.NoInterpret(); // TODO: Implement

		Expression("count-leading-zeros", list => list[1].Type,
				list => $"CountLeadingZeros({GenerateExpression(list[1])})", 
				list => $"Call<{GenerateType(list[1].Type.AsCompiletime())}, {GenerateType(list[1].Type.AsCompiletime())}>(CountLeadingZeros, {GenerateExpression(list[1])})")
			.NoInterpret(); // TODO: Implement

		Expression(":", list => new EInt(false,
				list.Skip(1).Select(y => y.Type switch {
						EInt(_, var width) => width,
						EBool => 1,
						_ => throw new NotSupportedException()
					}).Sum()).AsRuntime(list.AnyRuntime),
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
			}).Interpret((list, state) => {
			var ret = 0UL;
			foreach(var elem in list.Skip(1)) {
				var value = state.Evaluate(elem);
				if(elem.Type is not EInt(_, var width)) throw new NotSupportedException("Non-int element in :");
				ret <<= width;
				ret |= (ulong) Extensions.AsNonBool(value);
			}
			return ret;
		});

		Expression("replicate", list => new EInt(false,
				list[1].Type is EInt(_, var elemWidth) && list[2] is PInt(var count)
					? elemWidth * (int) count
					: throw new NotSupportedException()).AsRuntime(list[1].Type.Runtime),
			list => {
				if(list[1].Type is not EInt(_, var width)) throw new NotSupportedException();
				if(list[2] is not PInt(var count)) throw new NotSupportedException();
				return Enumerable.Range(0, (int) count)
					.Select(i => $"((({GenerateType(list.Type)}) ({GenerateExpression(list[1])})) << {i * width})")
					.Aggregate((a, x) =>
						$"({GenerateType(list.Type)}) ((({GenerateType(list.Type)}) {a}) | (({GenerateType(list.Type)}) {x}))");
			}).Interpret((list, state) => {
			var ret = 0UL;
			var value = (ulong) state.Evaluate(list[1]);
			var count = (int) state.Evaluate(list[2]);
			if(list[1].Type is not EInt(_, var width)) throw new NotSupportedException("Non-int value for replicate");
			for(var i = 0; i < count; ++i) {
				ret <<= width;
				ret |= value;
			}
			return ret;
		});
			
		Expression("abs", list => list[1].Type, 
				list => list[1].Type switch {
					EFloat(_) => $"fabs({GenerateExpression(list[1])})", 
					_ => throw new NotSupportedException()
				}, 
				list => $"({GenerateExpression(list[1])}).Abs()")
			.Interpret((list, state) => ((object) state.Evaluate(list[1])).If<float>(x => MathF.Abs(x)).IfNot<float>(x => Math.Abs(x)));

		Expression("sqrt", list => list[1].Type,
				list => $"({GenerateType(list.Type)}) sqrt((double) ({GenerateExpression(list[1])}))",
				list => $"({GenerateType(list.Type)}) (({GenerateType(new EFloat(64).AsRuntime(list[1].Type.Runtime))}) ({GenerateExpression(list[1])})).Sqrt()")
			.Interpret((list, state) => ((object) state.Evaluate(list[1])).If<float>(x => MathF.Sqrt(x)).IfNot<float>(x => Math.Sqrt((double) x)));
			
		Expression("round", list => list[1].Type, 
				list => $"round{(list[1].Type is EFloat(32) ? "f" : "")}({GenerateExpression(list[1])})", 
				list => $"({GenerateExpression(list[1])}).Round()")
			.Interpret((list, state) => ((object) state.Evaluate(list[1])).If<float>(x => MathF.Round(x)).IfNot<float>(x => Math.Round((double) x)));
			
		Expression("round-half-down", list => list[1].Type, 
				list => $"ceil{(list[1].Type is EFloat(32) ? "f" : "")}(({GenerateExpression(list[1])}) - 0.5{(list[1].Type is EFloat(32) ? "f" : "")})", 
				list => $"({GenerateExpression(list[1])}).RoundHalfDown()")
			.Interpret((list, state) => ((object) state.Evaluate(list[1])).If<float>(x => MathF.Ceiling(x - 0.5f)).IfNot<float>(x => Math.Ceiling((double) x - 0.5)));
			
		Expression("round-half-up", list => list[1].Type, 
				list => $"floor{(list[1].Type is EFloat(32) ? "f" : "")}(({GenerateExpression(list[1])}) + 0.5{(list[1].Type is EFloat(32) ? "f" : "")})", 
				list => $"({GenerateExpression(list[1])}).RoundHalfUp()")
			.Interpret((list, state) => ((object) state.Evaluate(list[1])).If<float>(x => MathF.Floor(x + 0.5f)).IfNot<float>(x => Math.Floor((double) x + 0.5)));
			
		Expression("ceil", list => list[1].Type, 
				list => $"ceil{(list[1].Type is EFloat(32) ? "f" : "")}({GenerateExpression(list[1])})", 
				list => $"({GenerateExpression(list[1])}).Ceil()")
			.Interpret((list, state) => ((object) state.Evaluate(list[1])).If<float>(x => MathF.Ceiling(x)).IfNot<float>(x => Math.Ceiling((double) x)));
			
		Expression("floor", list => list[1].Type, 
				list => $"floor{(list[1].Type is EFloat(32) ? "f" : "")}({GenerateExpression(list[1])})", 
				list => $"({GenerateExpression(list[1])}).Floor()")
			.Interpret((list, state) => ((object) state.Evaluate(list[1])).If<float>(x => MathF.Floor(x)).IfNot<float>(x => Math.Floor((double) x)));

		Expression("bitwidth", _ => new EInt(true, 32),
			list => {
				switch(TypeFromName(list[1])) {
					case EInt(_, var iwidth): return iwidth.ToString();
					case EFloat(var fwidth): return fwidth.ToString();
					case EVector: return "128";
					default: throw new NotSupportedException(list[1].Type.ToString());
				}
		}).Interpret((list, state) => TypeFromName(list[1]) switch {
			EInt(_, var width) => width, 
			EFloat(var width) => width, 
			EVector => 128, 
			var type => throw new NotSupportedException($"Bitwidth on type {type}")
		});

		Expression("NaN?", list => new EInt(false, 1).AsRuntime(list[1].Type.Runtime),
				list => $"isnan({GenerateExpression(list[1])}) ? 1U : 0U",
				list => $"({GenerateExpression(list[1])}).IsNaN()")
			.Interpret((list, state) => 
				state.Evaluate(list[1]) switch { float v => float.IsNaN(v), double v => double.IsNaN(v), _ => false });

		Expression("literal", list => list[1].Type,
				list => GenerateExpression(new PInt((long) new ExecutionState().Evaluate(list[1])) { Type = list[1].Type }))
			.Interpret((list, state) => state.Evaluate(list[1]));
	}
}