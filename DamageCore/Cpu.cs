namespace DamageCore; 

public class Cpu {
	public readonly Core Core;
	public readonly Memory Memory;
	public readonly State State;
	public readonly Interpreter Interpreter;

	public Cpu(Core core, ICartridge cartridge) {
		Core = core;
		Memory = new(core, cartridge);
		State = new(Memory);
		Interpreter = new(core, State);
	}

	public void Run() {
		while(true) {
			Console.WriteLine($"PC 0x{State.PC:X04}  SP 0x{State.SP:X04}");
			Console.WriteLine($"A 0x{State.Registers[0b111]:X02}  B 0x{State.Registers[0b000]:X02}");
			Console.WriteLine($"C 0x{State.Registers[0b001]:X02}  D 0x{State.Registers[0b010]:X02}");
			Console.WriteLine($"E 0x{State.Registers[0b011]:X02}  F 0x{State.Flags:X02}");
			Console.WriteLine($"H 0x{State.Registers[0b100]:X02}  L 0x{State.Registers[0b101]:X02}");
			Interpreter.RunOne();
		}
	}
}