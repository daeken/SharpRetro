using LibSharpRetro;

namespace DamageCore;

public class Core : ICore {
	public string Name { get; } = "Damage";
	public string ShortDescription { get; } = "Game Boy/Game Boy Color core";
	public string LongDescription { get; } = null;
	public bool Pausable { get; } = true;
	public bool SupportSaveStates { get; } = true;
	public GraphicsBackend GraphicsBackends { get; } = GraphicsBackend.Framebuffer;

	[UserOption("Scaling mode")] public ScalingMode ScalingMode = ScalingMode.None;
	[UserOption("Preserve aspect ratio")] public bool PreserveAspectRatio = true;

	IFramebufferBackend FramebufferBackend;

	Cpu Cpu;

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
		Cpu = new(cartridge) {
			State = {
				PC = 0x0100
			}
		};
		return true;
	}

	public void Unload() {
		throw new NotImplementedException();
	}

	public void Run() {
		while(true)
			Cpu.Run();
	}

	public void Stop() {
		throw new NotImplementedException();
	}

	public void Pause() {
		throw new NotImplementedException();
	}
}