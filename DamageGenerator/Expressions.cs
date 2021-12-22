using CoreArchCompiler;

namespace DamageGenerator; 

public class Expressions : Builtin {
	public override void Define() {
		string RegName(int id) => id switch {
			0b000 => "B", 
			0b001 => "C", 
			0b010 => "D", 
			0b011 => "E", 
			0b100 => "H", 
			0b101 => "L", 
			0b110 => throw new BailoutException(), 
			0b111 => "A", 
			_ => throw new NotSupportedException()
		};
		
		Expression("pc", _ => new EInt(false, 16), _ => "pc").Interpret((_, state) => state.GetRegister("PC"));

		Expression("reg", _ => new EInt(false, 8).AsRuntime(),
				list => $"({GenerateExpression(list[1])}) switch {{ 0b110 => throw new NotSupportedException(), {{}} i => State.Registers[i] }}",
				_ => "/*UNIMPLEMENTED*/")
			.Interpret((list, state) => {
				var reg = state.Evaluate(list[1]);
				return (byte) state.GetRegister(RegName(reg));
			});
		
		Expression("reg-ime", _ => new EInt(false, 1).AsRuntime(), 
			_ => "State.InterruptsEnabled", 
			_ => "/*UNIMPLEMENTED*/").NoInterpret();
		
		Expression("reg-ime-schedule", _ => new EInt(false, 1).AsRuntime(), 
			_ => "State.InterruptsEnableScheduled", 
			_ => "/*UNIMPLEMENTED*/").NoInterpret();
		
		Expression("reg-flags", _ => new EInt(false, 8).AsRuntime(), 
			_ => "State.Flags", 
			_ => "/*UNIMPLEMENTED*/").NoInterpret();
		
		Expression("reg-bc", _ => new EInt(false, 16).AsRuntime(), 
			_ => "((((ushort) State.Registers[0b000]) << 8) | (ushort) State.Registers[0b001])", 
			_ => "/*UNIMPLEMENTED*/").NoInterpret();
		
		Expression("reg-de", _ => new EInt(false, 16).AsRuntime(), 
			_ => "((((ushort) State.Registers[0b010]) << 8) | (ushort) State.Registers[0b011])", 
			_ => "/*UNIMPLEMENTED*/").NoInterpret();
		
		Expression("reg-hl", _ => new EInt(false, 16).AsRuntime(), 
			_ => "((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])", 
			_ => "/*UNIMPLEMENTED*/").NoInterpret();
		
		Expression("reg-af", _ => new EInt(false, 16).AsRuntime(), 
			_ => "((((ushort) State.Registers[0b111]) << 8) | (ushort) State.Flags)", 
			_ => "/*UNIMPLEMENTED*/").NoInterpret();
		
		Expression("reg-sp", _ => new EInt(false, 16).AsRuntime(), 
			_ => "State.SP", 
			_ => "/*UNIMPLEMENTED*/").NoInterpret();
		
		Statement("=", list => list[2].Type?.AsRuntime(list.AnyRuntime) ?? throw new NotImplementedException(),
			(c, list) => {
				if(list[1] is PList sub)
					switch(sub[0]) {
						case PName("reg"):
							c += $"State.Registers[(int) {GenerateExpression(sub[1])}] = (byte) ({GenerateExpression(list[2])});";
							return;
						case PName("reg-bc"):
						case PName("reg-de"):
						case PName("reg-hl"):
							var temp = Core.TempName();
							c += $"var {temp} = (ushort) {GenerateExpression(list[2])};";
							var (a, b) = ((PName) sub[0]).Name switch {
								"reg-bc" => ("0b000", "0b001"), 
								"reg-de" => ("0b010", "0b011"), 
								"reg-hl" => ("0b100", "0b101"), 
								_ => throw new NotSupportedException()
							};
							c += $"State.Registers[{a}] = (byte) ({temp} >> 8);";
							c += $"State.Registers[{b}] = (byte) ({temp} & 0xFF);";
							return;
						case PName("reg-af"):
							var aftemp = Core.TempName();
							c += $"var {aftemp} = (ushort) {GenerateExpression(list[2])};";
							c += $"State.Registers[0b111] = (byte) ({aftemp} >> 8);";
							c += $"State.Flags = (byte) ({aftemp} & 0xF0);";
							return;
						case PName("reg-flags"):
							c += $"State.Flags = (byte) ({GenerateExpression(list[2])} & 0xF0);";
							return;
						case PName("reg-sp"):
							c += $"State.SP = (ushort) {GenerateExpression(list[2])};";
							return;
						case PName("reg-ime"):
							c += $"State.InterruptsEnabled = {GenerateExpression(list[2])};";
							return;
						case PName("reg-ime-schedule"):
							c += $"State.InterruptsEnableScheduled = {GenerateExpression(list[2])};";
							return;
					}

				c += $"{GenerateExpression(list[1], lhs: true)} = {GenerateExpression(list[2])};";
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
		
		BranchExpression("branch-default", _ => EType.Unit.AsRuntime(), list => "Branch(pc)")
			.Interpret((_, _) => null);

		BranchExpression("cycles", _ => EType.Unit.AsRuntime(), list => $"AddCycles({GenerateExpression(list[1])})")
			.NoInterpret();

		Expression("halt", _ => EType.Unit.AsRuntime(), _ => "Halt()").NoInterpret();
	}
}