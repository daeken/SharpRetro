using LibSharpRetro;

namespace DamageCore;

public class Core : ICore {
	public string Name => "Damage";
	public string ShortDescription => "Game Boy/Game Boy Color core";
	public string LongDescription => null;
	public bool Pausable => true;
	public bool SupportSaveStates => true;
	public GraphicsBackend GraphicsBackends => GraphicsBackend.Framebuffer;

	[UserOption("Scaling mode")] public ScalingMode ScalingMode = ScalingMode.None;
	[UserOption("Preserve aspect ratio")] public bool PreserveAspectRatio = true;

	IFramebufferBackend FramebufferBackend;

	public Timing Timing;
	public Cpu Cpu;
	public Ppu Ppu;
	bool Running;

	public bool CanLoad(string path) => path.ToLower().EndsWith(".gb");
	public void Setup(IGraphicsBackend graphicsBackend) {
		if(graphicsBackend is IFramebufferBackend fbb)
			FramebufferBackend = fbb;
		else
			throw new NotSupportedException("Given non-framebuffer graphics backend");

		FramebufferBackend.Resolution = (160, 144);
	}

	public void Teardown() {
		throw new NotImplementedException();
	}

	public bool Load(string path) {
		var cartridge = ICartridge.Load(File.ReadAllBytes(path));
		if(cartridge == null) return false;
		Timing = new Timing();
		Cpu = new(this, cartridge) {
			State = {
				PC = 0x0100
			}
		};
		Ppu = new(this);
		return true;
	}

	public void Unload() {
		throw new NotImplementedException();
	}

	public void Run() {
		Running = true;
		while(Running)
			Cpu.Run();
	}

	public void Stop() => Running = false;

	public void Pause() {
		throw new NotImplementedException();
	}
	
	public void IoWrite(ushort addr, byte value) {
		switch(addr) {
			case >= 0xFF40 and <= 0xFFfB:
				Ppu.IoWrite(addr, value);
				break;
			default:
				Console.WriteLine($"Unhandled IO Write to 0x{addr:X04}: 0x{value:X02}");
				break;
		}
	}

	public byte IoRead(ushort addr) {
		switch(addr) {
			case >= 0xFF40 and <= 0xFFfB:
				return Ppu.IoRead(addr);
			default:
				Console.WriteLine($"Unhandled IO read from 0x{addr:X04}");
				return 0;
		}
	}
}