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

		Expression("absorb-muldiv-delay", _ => EUnit.RuntimeType,
				_ => "AbsorbMuldivDelay()")
			.NoInterpret();
		
		Expression("copfun", _ => EUnit.RuntimeType,
				list => $"Copfun({GenerateExpression(list[1])}, {GenerateExpression(list[2])})")
			.NoInterpret();
		
		Expression("exception", _ => EUnit.RuntimeType, 
				list => $"throw new CpuException(ExceptionType.{list[1]}, pc, insn)")
			.NoInterpret();
		
		Expression("copreg", _ => new EInt(false, 32).AsRuntime(), 
				list => $"Copreg({GenerateExpression(list[1])}, {GenerateExpression(list[2])})")
			.NoInterpret();

		Expression("copcreg", _ => new EInt(false, 32).AsRuntime(), 
				list => $"Copcreg({GenerateExpression(list[1])}, {GenerateExpression(list[2])})")
			.NoInterpret();

		Expression("mul-delay", _ => EUnit.RuntimeType,
				list => $"MulDelay({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])} != 0)")
			.NoInterpret();

		Expression("div-delay", _ => EUnit.RuntimeType,
				_ => "DivDelay()")
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
						case PName("copreg"):
							c += $"Copreg({GenerateExpression(sub[1])}, {GenerateExpression(sub[2])}, {GenerateExpression(list[2])});";
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

		Statement("defer=", list => list[2].Type?.AsRuntime(list.AnyRuntime) ?? throw new NotImplementedException(),
			(c, list) => {
				if(list[1] is not PList sub) return;
				switch(sub[0]) {
					case PName("reg"):
						c += $"State->LdWhich = (uint) ({GenerateExpression(sub[1])});";
						c += $"State->LdValue = (uint) ({GenerateExpression(list[2])});";
						break;
					default:
						throw new NotSupportedException($"Defer= used on non-reg argument {sub}");
				}
			},
			(c, list) => {
				c += $"/*UNIMPLEMENTED*/";
			}).NoInterpret();
		
		Expression("load", list => TypeFromName(list[2]).AsRuntime(),
				list => {
					var type = GenerateType(list.Type);
					return $"ReadMemory<{type}>({GenerateExpression(list[1])})";
				},
				list =>
					$"((RuntimePointer<{GenerateType(list.Type.AsCompiletime())}>) ({GenerateExpression(list[1])})).value()")
			.Interpret((list, state) => state.GetMemory(state.Evaluate(list[1]), list.Type));

		Expression("store", _ => EType.Unit.AsRuntime(),
				list => $"WriteMemory({GenerateExpression(list[1])}, {GenerateExpression(list[2])})",
				list =>
					$"((RuntimePointer<{GenerateType(list[2].Type.AsCompiletime())}>) ({GenerateExpression(list[1])})).value({GenerateExpression(list[2])})")
			.Interpret((list, state) => {
				state.SetMemory(state.Evaluate(list[1]), state.Evaluate(list[2]));
				return null;
			});
		
		Statement("do-load", _ => EType.Unit.AsRuntime(), (_, _) => {}).NoInterpret();
	}
}