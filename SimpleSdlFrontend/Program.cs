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

var fb = new SdlFramebuffer();
var ab = new SdlAudio();
core.Setup(fb, ab);
if(!core.Load(args[1]))
	throw new Exception("Failed to load file");

SDL_Init(SDL_INIT_EVERYTHING);

while(true) {
	while(SDL_PollEvent(out var evt) != 0) {
		switch(evt.type) {
			case SDL_EventType.SDL_KEYDOWN:
				core.KeyDown((byte) evt.key.keysym.scancode);
				break;
			case SDL_EventType.SDL_KEYUP:
				core.KeyUp((byte) evt.key.keysym.scancode);
				break;
			case SDL_EventType.SDL_QUIT:
				goto end;
		}
	}
	
	core.Run();
	fb.Flip();
}

end:

core.Unload();
core.Teardown();

class SdlFramebuffer : IFramebufferBackend {
	(int, int) _Resolution;
	public (int Width, int Height) Resolution {
		get => _Resolution;
		set {
			if(_Resolution == value) return;
			_Resolution = value;
			SDL_GetDesktopDisplayMode(SDL_GetWindowDisplayIndex(Window), out var dm);
			var (w, h) = value;
			var scale = 1;
			while(true) {
				scale++;
				var (sw, sh) = (w * scale, h * scale);
				if(sw > dm.w || sh > dm.h) break;
			}
			scale--;
			if(scale != 1) scale--;
			SDL_SetWindowSize(Window, w * scale, h * scale);
			SDL_SetWindowPosition(Window, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED);
			Framebuffer = new byte[w * h * 3];
			Texture = SDL_CreateTexture(Renderer, SDL_PIXELFORMAT_RGB24,
				(int) SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING, w, h);
		}
	}
	readonly IntPtr Window, Renderer;
	IntPtr Texture;
	public byte[] Framebuffer { get; private set; }


	public SdlFramebuffer() {
		Window = SDL_CreateWindow("SharpRetro", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, 640, 480, 0);
		Renderer = SDL_CreateRenderer(Window, -1, SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
		//Resolution = (640, 480);
	}

	internal unsafe void Flip() {
		fixed(byte* fbptr = Framebuffer)
			SDL_UpdateTexture(Texture, IntPtr.Zero, (IntPtr) fbptr, Resolution.Width * 3);
		SDL_RenderClear(Renderer);
		SDL_RenderCopy(Renderer, Texture, IntPtr.Zero, IntPtr.Zero);
		SDL_RenderPresent(Renderer);
	}
}

class SdlAudio : IAudioBackend {
	public int SampleRate { get; set; } = 44100;
	public int Channels { get; set; } = 2;
	readonly SDL_AudioSpec Requested, Got;

	readonly Queue<short> RawSamples = new();
	bool Playing;

	public SdlAudio() {
		Requested = new SDL_AudioSpec {
			freq = 44100, 
			format = AUDIO_S16SYS, 
			channels = 2, 
			silence = 0, 
			samples = 256, 
			callback = Feed
		};
		SDL_OpenAudio(ref Requested, out Got);
	}

	unsafe void Feed(IntPtr _, IntPtr stream, int len) {
		lock(RawSamples) {
			var span = new Span<short>((void*) stream, len / 2);
			for(var i = 0; i < span.Length && RawSamples.TryDequeue(out var sample); ++i)
				span[i] = sample;
		}
	}
	
	public void AddSamples(IEnumerable<short> data) {
		lock(RawSamples)
			data.ForEach(RawSamples.Enqueue);
		if(!Playing) {
			Playing = true;
			SDL_PauseAudio(0);
		}
	}
}
