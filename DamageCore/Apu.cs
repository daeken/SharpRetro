using LibSharpRetro;

namespace DamageCore; 

public class Apu {
	readonly Core Core;
	readonly IAudioBackend AudioBackend;
	readonly byte[] WavePattern = new byte[16];
	
	public Apu(Core core) {
		Core = core;
		AudioBackend = Core.AudioBackend;
		core.Timing.TimeSync(Run());
	}

	IEnumerable<ulong> Run() {
		var sequence = 0;
		while(true) {
			sequence = (sequence + 1) % 8;
			
			yield return 8192; // 512hz
		}
	}
	
	public void IoWrite(ushort addr, byte value) {
		switch(addr) {
			case >= 0xFF30 and <= 0xFF3F: WavePattern[addr - 0xFF30] = value; break;
			default:
				Console.WriteLine($"Unhandled APU IO Write to 0x{addr:X04}: 0x{value:X02}");
				break;
		}
	}

	public byte IoRead(ushort addr) {
		switch(addr) {
			case >= 0xFF30 and <= 0xFF3F: return WavePattern[addr - 0xFF30];
			default:
				Console.WriteLine($"Unhandled APU IO Read from 0x{addr:X04}");
				return 0;
		}
	}
}