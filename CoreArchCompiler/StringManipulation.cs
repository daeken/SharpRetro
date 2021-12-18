using System;
using System.Linq;

namespace CoreArchCompiler; 

public class StringManipulation : Builtin {
	public override void Define() {
		Expression("string-concat", list => EType.String.AsRuntime(list.AnyRuntime),
				list => string.Join(" + ", list.Skip(1).Select(x => x.Type is EString ? GenerateExpression(x) : $"({GenerateExpression(x)}).ToString()")),
				_ => "/*UNIMPLEMENTED*/")
			.Interpret((list, state) =>
				list.Skip(1).Aggregate("", (current, elem) => (string) (current + state.Evaluate(elem).ToString())));
		Expression("hex", list => EType.String.AsRuntime(list.AnyRuntime),
			list => list[1].Type is EInt(_, var bits) ? $"$\"0x{{({GenerateExpression(list[1])}):x0{bits / 4}}}\"" : throw new NotSupportedException())
			.NoInterpret(); // TODO: Implement
	}
}