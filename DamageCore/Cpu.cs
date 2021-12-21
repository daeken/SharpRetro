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
			State.InterruptsEnabledPending = State.InterruptsEnableScheduled;
			/*Console.WriteLine($"PC 0x{State.PC:X04}  SP 0x{State.SP:X04}");
			Console.WriteLine($"A 0x{State.Registers[0b111]:X02}  B 0x{State.Registers[0b000]:X02}");
			Console.WriteLine($"C 0x{State.Registers[0b001]:X02}  D 0x{State.Registers[0b010]:X02}");
			Console.WriteLine($"E 0x{State.Registers[0b011]:X02}  F 0x{State.Flags:X02}");
			Console.WriteLine($"H 0x{State.Registers[0b100]:X02}  L 0x{State.Registers[0b101]:X02}");*/
			Interpreter.RunOne();
			if(State.InterruptsEnabledPending && State.InterruptsEnableScheduled) {
				State.InterruptsEnabled = true;
				State.InterruptsEnableScheduled = State.InterruptsEnabledPending = false;
			}

			if(State.InterruptsEnabled && (Core.InterruptEnable & Core.InterruptFlag & 0b11111) != 0) {
				for(var i = 0; i < 5; ++i) {
					if((Core.InterruptEnable & Core.InterruptFlag & (1 << i)) == 0) continue;
					Console.WriteLine($"Interrupt {i}!");
					Core.InterruptFlag &= (byte) ~(1 << i);
					State.InterruptsEnabled = false;
					State.SP -= 2;
					Memory.Write16(State.SP, State.PC);
					State.PC = (ushort) (0x40 + 8 * i);
					Interpreter.AddCycles(5);
					break;
				}
			}
		}
	}
}