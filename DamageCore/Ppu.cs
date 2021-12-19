namespace DamageCore; 

public class Ppu {
	public Ppu(Core core) => core.Timing.TimeSync(Run());

	IEnumerable<ulong> Run() {
		while(true) {
			Console.WriteLine("PPU to wait 100 cycles!");
			yield return 100;
		}
	}
}