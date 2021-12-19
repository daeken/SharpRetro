namespace DamageCore; 

public class Ppu {
	public Ppu(Core core) => core.Timing.TimeSync(Run());

	IEnumerable<ulong> Run() {
		while(true) {
			Console.WriteLine("PPU to wait 100 cycles!");
			yield return 100;
		}
	}
	
	public void IoWrite(ushort addr, byte value) {
		switch(addr) {
			default:
				Console.WriteLine($"Unhandled PPU IO Write to 0x{addr:X04}: 0x{value:X02}");
				break;
		}
	}

	public byte IoRead(ushort addr) {
		switch(addr) {
			default:
				Console.WriteLine($"Unhandled PPU IO Read from 0x{addr:X04}");
				return 0;
		}
	}
}