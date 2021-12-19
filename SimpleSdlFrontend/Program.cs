using System.Runtime.InteropServices;
using LibSharpRetro;
using static SDL2.SDL;

var core = ICore.LoadCore(args[0]);
if(core == null)
	throw new Exception("Failed to load core");
Console.WriteLine($"Core name: {core.Name}");
Console.WriteLine($"Core short description: {core.ShortDescription}");
Console.WriteLine($"Core long description: {core.LongDescription}");
if(!core.CanLoad(args[1]))
	throw new Exception("Core CanLoad file returned false!");

if(!core.GraphicsBackends.HasFlag(GraphicsBackend.Framebuffer))
	throw new Exception("Core doesn't support framebuffer backend");

core.Setup(new SdlFramebuffer());
if(!core.Load(args[1]))
	throw new Exception("Failed to load file");

core.Run();

core.Unload();
core.Teardown();

unsafe class SdlFramebuffer : IFramebufferBackend {
	(int, int) _Resolution;
	public (int Width, int Height) Resolution {
		get => _Resolution;
		set {
			if(_Resolution == value) return;
			_Resolution = value;
			SDL_SetWindowSize(Window, value.Width, value.Height);
			Pixels = null;
			RefreshPixels();
		}
	}
	readonly IntPtr Window;
	byte* Pixels;

	public SdlFramebuffer() {
		SDL_Init(SDL_INIT_VIDEO);
		Window = SDL_CreateWindow("SharpRetro", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, 640, 480, 0);
		Resolution = (640, 480);
	}

	void RefreshPixels() {
		if(Pixels != null) return;
		var surfacePtr = SDL_GetWindowSurface(Window);
		var surface = Marshal.PtrToStructure<SDL_Surface>(surfacePtr);
		Pixels = (byte*) surface.pixels;
	}

	public Span<byte> Framebuffer => new(Pixels, Resolution.Width * Resolution.Height * 4);

	public void Flip() => SDL_UpdateWindowSurface(Window);
}
