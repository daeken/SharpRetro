namespace ArchCompilerCore;

// Extracted {sig, exec} from legacy: CoreArchCompiler/ScalarMath.cs
// Emit-lambdas → Backends/CSharp (rung-2). Local helpers lifted VERBATIM from legacy.
public class ScalarMath : Builtin {

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

    public override void Define() {
        Expr(new[] {"+", "-", "*", "/", "%"}, LogicalType,
            (list, state) =>
				(state.Evaluate(list[1]), state.Evaluate(list[2])).WithCommonType((a, b) =>
					list[0].AsName() switch {
						"+" => unchecked(a + b), "-" => unchecked(a - b), 
						"*" => unchecked(a * b), "/" => unchecked(a / b), 
						"%" => unchecked(a % b), 
						_ => throw new BailoutException()
					}));
        Expr(new[] { "|", "&", "^" }, LogicalBool,
            (list, state) =>
				list.Skip(2).Aggregate((object) state.Evaluate(list[1]), (al, bl) =>
					(al, state.Evaluate(bl)).WithCommonType((a, b) =>
						list[0].AsName() switch {
							"|" => a | b,
							"&" => a & b,
							"^" => a ^ b,
							_ => throw new BailoutException()
						})));
        Expr("~", FirstType,
            (list, state) => ~state.Evaluate(list[1]));
        Expr("-!", FirstType,
            (list, state) => -state.Evaluate(list[1]));
        Expr("!", list => new EBool().AsRuntime(list[1].Type.Runtime),
            (list, state) => !Extensions.AsBool(state.Evaluate(list[1])));
        Expr("<<", FirstType,
            (list, state) => {
				var shift = (int) state.Evaluate(list[2]);
				if(list[1].Type is EInt(_, var size) && shift >= size) return 0;
				return state.Evaluate(list[1]) << shift;
			});
        Expr(">>", FirstType,
            (list, state) => {
				var shift = (int) state.Evaluate(list[2]);
				if(list[1].Type is EInt(var signed, var size) && shift >= size)
					return signed ? 0xFFFFFFFF_FFFFFFFFUL : 0;
				return state.Evaluate(list[1]) >> shift;
			});
        Expr(">>>", FirstType);
        Expr("reverse-bits", list => list[1].Type);
        Expr("count-leading-zeros", list => list[1].Type);
        Expr(":", list => new EInt(false,
				list.Skip(1).Select(y => y.Type switch {
						EInt(_, var width) => width,
						EBool => 1,
						_ => throw new NotSupportedException()
					}).Sum()).AsRuntime(list.AnyRuntime),
            (list, state) => {
			var ret = 0UL;
			foreach(var elem in list.Skip(1)) {
				var value = state.Evaluate(elem);
				if(elem.Type is not EInt(_, var width)) throw new NotSupportedException("Non-int element in :");
				ret <<= width;
				ret |= (ulong) Extensions.AsNonBool(value);
			}
			return ret;
		});
        Expr("replicate", list => new EInt(false,
				list[1].Type is EInt(_, var elemWidth) && list[2] is PInt(var count)
					? elemWidth * (int) count
					: throw new NotSupportedException()).AsRuntime(list[1].Type.Runtime),
            (list, state) => {
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
        Expr("abs", list => list[1].Type,
            (list, state) => ((object) state.Evaluate(list[1])).If<float>(x => MathF.Abs(x)).IfNot<float>(x => Math.Abs(x)));
        Expr("sqrt", list => list[1].Type,
            (list, state) => ((object) state.Evaluate(list[1])).If<float>(x => MathF.Sqrt(x)).IfNot<float>(x => Math.Sqrt((double) x)));
        Expr("round", list => list[1].Type,
            (list, state) => ((object) state.Evaluate(list[1])).If<float>(x => MathF.Round(x)).IfNot<float>(x => Math.Round((double) x)));
        Expr("round-toward-zero", list => list[1].Type,
            (list, state) => ((object) state.Evaluate(list[1])).If<float>(x => MathF.Round(x, MidpointRounding.ToZero)).IfNot<float>(x => Math.Round((double) x, MidpointRounding.ToZero)));
        Expr("round-half-down", list => list[1].Type,
            (list, state) => ((object) state.Evaluate(list[1])).If<float>(x => MathF.Ceiling(x - 0.5f)).IfNot<float>(x => Math.Ceiling((double) x - 0.5)));
        Expr("round-half-up", list => list[1].Type,
            (list, state) => ((object) state.Evaluate(list[1])).If<float>(x => MathF.Floor(x + 0.5f)).IfNot<float>(x => Math.Floor((double) x + 0.5)));
        Expr("ceil", list => list[1].Type,
            (list, state) => ((object) state.Evaluate(list[1])).If<float>(x => MathF.Ceiling(x)).IfNot<float>(x => Math.Ceiling((double) x)));
        Expr("floor", list => list[1].Type,
            (list, state) => ((object) state.Evaluate(list[1])).If<float>(x => MathF.Floor(x)).IfNot<float>(x => Math.Floor((double) x)));
        Expr("bitwidth", _ => new EInt(true, 32),
            (list, state) => TypeFromName(list[1]) switch {
			EInt(_, var width) => width, 
			EFloat(var width) => width, 
			EVector => 128, 
			var type => throw new NotSupportedException($"Bitwidth on type {type}")
		});
        Expr("NaN?", list => new EInt(false, 1).AsRuntime(list[1].Type.Runtime),
            (list, state) => 
				state.Evaluate(list[1]) switch { float v => float.IsNaN(v), double v => double.IsNaN(v), _ => false });
        Expr("literal", list => list[1].Type,
            (list, state) => state.Evaluate(list[1]));
    }
}
