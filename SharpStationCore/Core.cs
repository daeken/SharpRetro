using System.Web;
using LibSharpRetro;
using static SharpStationCore.Globals;

namespace SharpStationCore;

public class Core : ICore {
	public string Name => "SharpStation";
	public string ShortDescription => "Playstation core";
	public string LongDescription => null;
	public bool Pausable => true;
	public bool SupportSaveStates => false;
	public GraphicsBackend GraphicsBackends => GraphicsBackend.Framebuffer;

	public IFramebufferBackend FramebufferBackend;
	public IAudioBackend AudioBackend;

	public bool Running { get; private set; }

	public bool CanLoad(string path) => path.ToLower().EndsWith(".cue");
	public void Setup(IGraphicsBackend graphicsBackend, IAudioBackend audioBackend) {
		if(graphicsBackend is IFramebufferBackend fbb)
			FramebufferBackend = fbb;
		else
			throw new NotSupportedException("Given non-framebuffer graphics backend");
		AudioBackend = audioBackend;
	}

	public void Teardown() {
		throw new NotImplementedException();
	}

	public bool Load(string path) {
		Reset();
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

	//public void KeyDown(byte scancode) => Joypad.KeyDown(scancode);
	//public void KeyUp(byte scancode) => Joypad.KeyUp(scancode);
}