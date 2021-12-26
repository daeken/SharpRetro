using CoreArchCompiler;

namespace SharpStationGenerator; 

public class Expressions : Builtin {
	public override void Define() {
		Expression("pc", _ => new EInt(false, 32), _ => "pc").Interpret((_, state) => state.GetRegister("PC"));
		Expression("pcd", _ => new EInt(false, 32), _ => "(pc + 4)").Interpret((_, state) => state.GetRegister("PC") + 4);

		Expression("reg", _ => new EInt(false, 32).AsRuntime(),
				list => {
					try {
						var rnum = new ExecutionState().Evaluate(list[1]);
						if(rnum == 0) return "0U";
						return $"State->Registers[{rnum}]";
					} catch(Exception) {}
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

		Expression("copfun", _ => EUnit.RuntimeType,
				list => $"Copfun({GenerateExpression(list[1])}, {GenerateExpression(list[2])})")
			.NoInterpret();
		
		Expression("exception", _ => EUnit.RuntimeType, 
				list => $"throw new CpuException(ExceptionType.{list[1]}, pc, insn)")
			.NoInterpret();
		
		Expression("copcreg", _ => new EInt(false, 32).AsRuntime(), 
				list => $"Copcreg({GenerateExpression(list[1])}, {GenerateExpression(list[2])})")
			.NoInterpret();

		Statement("=", list => list[2].Type?.AsRuntime(list.AnyRuntime) ?? throw new NotImplementedException(),
			(c, list) => {
				if(list[1] is PList sub)
					switch(sub[0]) {
						case PName("reg"):
							try {
								var rnum = new ExecutionState().Evaluate(sub[1]);
								if(rnum != 0)
									c += $"State->Registers[{rnum}] = {GenerateExpression(list[2])};";
								return;
							} catch(Exception) {}
							var rtemp = TempName();
							c += $"var {rtemp} = {GenerateExpression(sub[1])};";
							c += $"if({rtemp} != 0)";
							c++;
							c += $"State->Registers[{rtemp}] = (uint) ({GenerateExpression(list[2])});";
							c--;
							return;
						case PName("reg-hi") or PName("reg-lo"):
							c += $"State->{(sub[0] is PName("reg-hi") ? "Hi" : "Lo")} = (uint) ({GenerateExpression(list[2])});";
							return;
						case PName("copcreg"):
							c += $"Copcreg({GenerateExpression(sub[1])}, {GenerateExpression(sub[2])}, {GenerateExpression(list[2])});";
							return;
					}

				c += $"{GenerateExpression(list[1], lhs: true)} = {GenerateExpression(list[2])};";
			},
			(c, list) => {
				c += $"/*UNIMPLEMENTED*/";
			}).NoInterpret();
	}
}