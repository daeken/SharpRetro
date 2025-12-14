using NxCommon;

namespace UmbraCore;

public class MainLoop {
    public readonly GameWrapper Game;
    public readonly List<ExeModule> Modules = [];
    
    public unsafe MainLoop(string libPath, string romFsPath) {
        Game = new GameWrapper(libPath);

        Game.Callbacks.LoadModule =
            (loadBase, data, size, textStart, textEnd, roStart, roEnd, dataStart, dataEnd) => {
                Console.WriteLine($"Loading module at 0x{loadBase:X}");
                Kernel.MemoryManager.Mmap(loadBase, size);
                Buffer.MemoryCopy(data, (void*) loadBase, size, size);
                Modules.Add(new(loadBase, size, textStart, textEnd, roStart, roEnd, dataStart, dataEnd));
            };
        Game.Setup();
        _ = new Rtld(Modules);
    }
}