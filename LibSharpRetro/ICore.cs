using System.Reflection;

namespace LibSharpRetro;
using System.IO;

public interface ICore {
	string Name { get; }
	string ShortDescription { get; }
	string LongDescription { get; }
	bool Pausable { get; }
	bool SupportSaveStates { get; }
	GraphicsBackend GraphicsBackends { get; }
	
	bool CanLoad(string path) => true;

	void Setup(IGraphicsBackend graphicsBackend, IAudioBackend audioBackend);
	void Teardown();

	bool Load(string path);
	void Unload();
	void Run();
	void Stop();
	void Pause() => throw new NotSupportedException();

	void SaveState(Stream stream) => throw new NotSupportedException();
	void LoadState(Stream stream) => throw new NotSupportedException();

	public static ICore LoadCore(string path) {
		var asm = Assembly.LoadFile(Path.GetFullPath(path));
		return asm.GetExportedTypes()
			.Where(type => typeof(ICore).IsAssignableFrom(type))
			.Select(type => (ICore) Activator.CreateInstance(type)).FirstOrDefault();
	}
	
	public void KeyDown(byte scancode) { }
	public void KeyUp(byte scancode) { }
}