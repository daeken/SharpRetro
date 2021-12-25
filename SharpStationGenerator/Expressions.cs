using CoreArchCompiler;

namespace SharpStationGenerator; 

public class Expressions : Builtin {
	public override void Define() {
		Expression("pc", _ => new EInt(false, 32), _ => "pc").Interpret((_, state) => state.GetRegister("PC"));

		Expression("reg", _ => new EInt(false, 32).AsRuntime(),
				list => {
					var tn = TempName();
					return $"({GenerateExpression(list[1])}) switch {{ 0 => 0U, var {tn} => State->Registers[{tn}] }}";
				},
				_ => "/*UNIMPLEMENTED*/")
			.Interpret((list, state) => {
				var reg = state.Evaluate(list[1]);
				return (byte) state.GetRegister($"%{reg}");
			});
		
		Expression("reg-hi", _ => new EInt(false, 32).AsRuntime(), 
			_ => "State->Hi").NoInterpret();
		Expression("reg-lo", _ => new EInt(false, 32).AsRuntime(), 
			_ => "State->Lo").NoInterpret();

		Statement("=", list => list[2].Type?.AsRuntime(list.AnyRuntime) ?? throw new NotImplementedException(),
			(c, list) => {
				if(list[1] is PList sub)
					switch(sub[0]) {
						case PName("reg"):
							c += $"switch({GenerateExpression(sub[1])}) {{";
							c++;
							c += $"case 0: break;";
							var tn = TempName();
							c += $"case var {tn}: State->Registers[{tn}] = (uint) ({GenerateExpression(list[2])}); break;";
							c--;
							c += "}";
							return;
						case PName("reg-hi") or PName("reg-lo"):
							c += $"State->{(sub[0] is PName("reg-hi") ? "Hi" : "Lo")} = (uint) ({GenerateExpression(list[2])};";
							return;
					}

				c += $"{GenerateExpression(list[1], lhs: true)} = {GenerateExpression(list[2])};";
			},
			(c, list) => {
				c += $"/*UNIMPLEMENTED*/";
			}).NoInterpret();
	}
}