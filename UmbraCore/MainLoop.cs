using System.Runtime.InteropServices;
using System.Text;
using NxCommon;
using UmbraCore.Kernel;

namespace UmbraCore;

public class MainLoop {
    public static MainLoop Instance;
    
    public readonly GameWrapper Game;
    public readonly List<ExeModule> Modules = [];
    
    public unsafe MainLoop(string libPath, string romFsPath) {
        Instance = this;
        Game = new GameWrapper(libPath);

        Game.Callbacks.Debug = pc => {
            Console.WriteLine($"Running from 0x{pc:X}");
            var thread = Globals.ThreadManager.CurrentThread;
            for(var i = 0; i < 31; ++i) {
                Console.Write($"X{i}: 0x{thread.CpuState->X[i]:X} ");
                if(i != 0 && i % 4 == 0)
                    Console.WriteLine();
            }
            Console.WriteLine();
            if(*(ulong*) 0x78006c45f0 != 0) // nn::os::detail::g_OsBootParamter (sic))
                Console.WriteLine($"g_OsBootParamter set! {*(ulong*) 0x78006c45f0:X}");
        };
        Game.Callbacks.LoadModule =
            (loadBase, data, size, textStart, textEnd, roStart, roEnd, dataStart, dataEnd) => {
                Console.WriteLine($"Loading module at 0x{loadBase:X}");
                Globals.MemoryManager.Mmap(loadBase, size);
                Buffer.MemoryCopy(data, (void*) loadBase, size, size);
                Modules.Add(new(loadBase, size, textStart, textEnd, roStart, roEnd, dataStart, dataEnd));
            };
        Game.Callbacks.WriteSr = (_, _, _, _, _, _) => {
            Console.WriteLine($"WriteSR attempted");
        };
        Game.Callbacks.OutputDebugString = (addr, size) => {
            Console.WriteLine($"Foo? {addr:X} {size}");
            Console.WriteLine($"Debug string: {Encoding.ASCII.GetString(new Span<byte>((void*) addr, (int) size))}");
            return 0;
        };
        Globals.Setup(Game);
        Game.Setup();
        var thread = Globals.ThreadManager.CurrentThread;
        thread.CpuState->X0 = 0;
        thread.CpuState->X1 = 0xFFFF8001; // thread handle
        thread.CpuState->X30 = 0xCAFEBABEDEADBEEFUL;
        thread.RunFrom(0x71_0000_0000UL, 0xCAFEBABEDEADBEEFUL);
        //_ = new Rtld(Modules);
    }
}