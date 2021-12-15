using LibSharpRetro;

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

class SdlFramebuffer : IFramebufferBackend {
	
}
