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
				list => $"({GenerateExpression(list[1])}) switch {{ 0b110 => throw new NotSupportedException(), {{}} i => state.Registers[i] }}",
				list => $"({GenerateExpression(list[1])}) == 31 ? (LlvmRuntimeValue<uint>) 0U : (LlvmRuntimeValue<uint>) (XR[(int) {GenerateExpression(list[1])}]())")
			.Interpret((list, state) => {
				var reg = state.Evaluate(list[1]);
				return (byte) state.GetRegister(RegName(reg));
			});
		
		Expression("reg-a", _ => new EInt(false, 8).AsRuntime(), 
			_ => "state.A", 
			_ => "/*UNIMPLEMENTED*/").NoInterpret();
		
		Expression("reg-b", _ => new EInt(false, 8).AsRuntime(), 
			_ => "state.B", 
			_ => "/*UNIMPLEMENTED*/").NoInterpret();
		
		Expression("reg-c", _ => new EInt(false, 8).AsRuntime(), 
			_ => "state.C", 
			_ => "/*UNIMPLEMENTED*/").NoInterpret();
		
		Expression("reg-d", _ => new EInt(false, 8).AsRuntime(), 
			_ => "state.D", 
			_ => "/*UNIMPLEMENTED*/").NoInterpret();
		
		Expression("reg-e", _ => new EInt(false, 8).AsRuntime(), 
			_ => "state.E", 
			_ => "/*UNIMPLEMENTED*/").NoInterpret();
		
		Expression("reg-H", _ => new EInt(false, 8).AsRuntime(), 
			_ => "state.H", 
			_ => "/*UNIMPLEMENTED*/").NoInterpret();
		
		Expression("reg-L", _ => new EInt(false, 8).AsRuntime(), 
			_ => "state.L", 
			_ => "/*UNIMPLEMENTED*/").NoInterpret();
		
		Expression("reg-bc", _ => new EInt(false, 16).AsRuntime(), 
			_ => "((((ushort) state.B) << 8) | (ushort) state.C)", 
			_ => "/*UNIMPLEMENTED*/").NoInterpret();
		
		Expression("reg-de", _ => new EInt(false, 16).AsRuntime(), 
			_ => "((((ushort) state.D) << 8) | (ushort) state.E)", 
			_ => "/*UNIMPLEMENTED*/").NoInterpret();
		
		Expression("reg-hl", _ => new EInt(false, 16).AsRuntime(), 
			_ => "((((ushort) state.H) << 8) | (ushort) state.L)", 
			_ => "/*UNIMPLEMENTED*/").NoInterpret();
	}
}