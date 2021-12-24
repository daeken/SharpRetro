using System.Web;
using LibSharpRetro;

namespace DamageCore;

public class Core : ICore {
	public string Name => "Damage";
	public string ShortDescription => "Game Boy core";
	public string LongDescription => null;
	public bool Pausable => true;
	public bool SupportSaveStates => true;
	public GraphicsBackend GraphicsBackends => GraphicsBackend.Framebuffer;

	[UserOption("Scaling mode")] public ScalingMode ScalingMode = ScalingMode.None;
	[UserOption("Preserve aspect ratio")] public bool PreserveAspectRatio = true;

	public IFramebufferBackend FramebufferBackend;

	public Timing Timing;
	public Cpu Cpu;
	public Memory Memory;
	public Ppu Ppu;
	public Timer Timer;
	public Joypad Joypad;
	
	public bool Running { get; private set; }

	public byte InterruptEnable, InterruptFlag;

	public bool CanLoad(string path) => path.ToLower().EndsWith(".gb");
	public void Setup(IGraphicsBackend graphicsBackend) {
		if(graphicsBackend is IFramebufferBackend fbb)
			FramebufferBackend = fbb;
		else
			throw new NotSupportedException("Given non-framebuffer graphics backend");
	}

	public void Teardown() {
		throw new NotImplementedException();
	}

	public bool Load(string path) {
		var cartridge = ICartridge.Load(File.ReadAllBytes(path));
		if(cartridge == null) return false;
		Timing = new Timing();
		Cpu = new(this, cartridge);
		Memory = Cpu.Memory;
		Ppu = new(this);
		Timer = new(this);
		Joypad = new();
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

	string SerialOutput = "";
	byte TempSerial;
	
	public void IoWrite(ushort addr, byte value) {
		switch(addr) {
			case 0xFF0F: InterruptFlag = value; Console.WriteLine($"Setting interruptflag? 0x{value:X}"); break;
			case 0xFFFF: InterruptEnable = value; break;
			case 0xFF00: Joypad.IoWrite(addr, value); break;
			case 0xFF01: TempSerial = value; break;
			case 0xFF02:
				if(value.HasBit(7)) {
					if(TempSerial == 0x0A) {
						Console.WriteLine($"Serial debug: {SerialOutput}");
						SerialOutput = "";
					} else
						SerialOutput += (char) TempSerial;
					TempSerial = 0;
				}
				break;
			case >= 0xFF04 and <= 0xFF07: Timer.IoWrite(addr, value); break;
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
			case 0xFF4D: return 0xFF;
			case >= 0xFF40 and <= 0xFFFB: return Ppu.IoRead(addr);
			case >= 0xFF04 and <= 0xFF07: return Timer.IoRead(addr);
			case 0xFF00: return Joypad.IoRead(addr);
			case 0xFF0F: return InterruptFlag;
			case 0xFFFF: return InterruptEnable;
			default:
				Console.WriteLine($"Unhandled IO read from 0x{addr:X04}");
				return 0xFF;
		}
	}

	public void KeyDown(byte scancode) => Joypad.KeyDown(scancode);
	public void KeyUp(byte scancode) => Joypad.KeyUp(scancode);
}