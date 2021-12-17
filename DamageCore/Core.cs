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

	public bool CanLoad(string path) => path.ToLower().EndsWith(".gb");
	public void Setup(IGraphicsBackend graphicsBackend) {
		if(graphicsBackend is IFramebufferBackend fbb)
			FramebufferBackend = fbb;
		else
			throw new NotSupportedException("Given non-framebuffer graphics backend");
		
		Console.WriteLine($"Disassembler: {Disassembler.Disassemble(new byte[] { 0x41, 0, 0, 0, 0, 0, 0 }, 0)}");
	}

	public void Teardown() {
		throw new NotImplementedException();
	}

	public bool Load(string path) {
		throw new NotImplementedException();
	}

	public void Unload() {
		throw new NotImplementedException();
	}

	public void Run() {
		throw new NotImplementedException();
	}

	public void Stop() {
		throw new NotImplementedException();
	}

	public void Pause() {
		throw new NotImplementedException();
	}
}