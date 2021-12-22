using LibSharpRetro;

namespace DamageCore; 

public class Timer {
	readonly Core Core;
	byte Div, Tima, Tma, Tac;
	ulong TimerCycles = 16;

	public Timer(Core core) {
		Core = core;
		core.Timing.TimeSync(RunDiv());
		core.Timing.TimeSync(RunTimer());
	}

	IEnumerable<ulong> RunDiv() {
		while(true) {
			yield return 16;
			Div++;
		}
	}

	IEnumerable<ulong> RunTimer() {
		while(true) {
			yield return TimerCycles;
			if(!Tac.HasBit(2)) continue;

			if(Tma == 0xFF) {
				Tma = Tima;
				Core.InterruptFlag |= 4;
			} else
				Tma++;
		}
	}

	public void IoWrite(ushort addr, byte value) {
		switch(addr) {
			case 0xFF04: Div = 0; break;
			case 0xFF05: Tima = value; break;
			case 0xFF06: Tma = value; break;
			case 0xFF07:
				Tac = value;
				if(Tac.HasBit(2))
					TimerCycles = (Tac & 0b11) switch {
						0b00 => 1024, 0b01 => 16, 0b10 => 64, _ => 256
					};
				else
					TimerCycles = 16;
				break;
			default:
				Console.WriteLine($"Unhandled Timer IO Write to 0x{addr:X04}: 0x{value:X02}");
				break;
		}
	}

	public byte IoRead(ushort addr) {
		switch(addr) {
			case 0xFF04: return Div;
			case 0xFF05: return Tima;
			case 0xFF06: return Tma;
			case 0xFF07: return Tac;
			default:
				Console.WriteLine($"Unhandled Timer IO Read from 0x{addr:X04}");
				return 0;
		}
	}
}