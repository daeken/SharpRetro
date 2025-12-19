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
        };
        Game.Callbacks.LoadModule =
            (loadBase, data, size, textStart, textEnd, roStart, roEnd, dataStart, dataEnd) => {
                Console.WriteLine($"Loading module at 0x{loadBase:X}");
                Globals.MemoryManager.Mmap(loadBase, size);
                Buffer.MemoryCopy(data, (void*) loadBase, size, size);
                Modules.Add(new(loadBase, size, textStart, textEnd, roStart, roEnd, dataStart, dataEnd));
            };
        Game.Callbacks.InitModule = (loadBase, size) => {
            Globals.IsNative = true;
            Console.WriteLine($"Module loaded at 0x{loadBase:X}-0x{loadBase+size:X}");
            Globals.MemoryManager.Regions[loadBase] = (size, 0);
            Modules.Add(new(loadBase, size, doRelocate: true));
        };
        Game.Callbacks.WriteSr = (_, _, _, _, _, _) => {
            Console.WriteLine($"WriteSR attempted");
        };
        Game.Callbacks.ReadSr = (op0, op1, crn, crm, op2) => {
            var reg = ((0b10 | (op0 & 0b1)) << 14) | ((op1 & 0b111) << 11) | ((crn & 0b1111) << 7) | ((crm & 0b1111) << 3) | (op2 & 0b111);
            var (found, value) = reg switch {
                0b11_011_0000_0000_001 => // CtrEl0
                    (true, 0x8444c004UL),
                0b11_011_0100_0100_000 => // FPCR
                    (true, 0UL),
                0b11_011_0100_0100_001 => // FPSR
                    (true, 0UL),
                0b11_011_1101_0000_011 => // TPIDR
                    (true, (ulong) Globals.ThreadManager.CurrentThread.TlsBase),
                0b11_011_1110_0000_001 => // CntpctEl0
                    (true, 0UL),
                0b11_011_0000_0000_111 => // DCZID_EL0
                    (true, 0UL),
                _ => (false, 0UL),
            };
            if(!found) Console.WriteLine($"Unknown SR: S{op0 | 2}_{op1}_{crn}_{crm}_{op2}");
            return value;
        };
        Game.Callbacks.OutputDebugString = (addr, size) => {
            Console.WriteLine($"Debug string: {Encoding.ASCII.GetString(new Span<byte>((void*) addr, (int) size))}");
            return 0;
        };
        Globals.Setup(Game);
        Game.Setup();
        Console.WriteLine("Done with setup!");
        if(false) {
            var thread = Globals.ThreadManager.CurrentThread;
            thread.CpuState->X0 = 0;
            thread.CpuState->X1 = 0xFFFF8001; // thread handle
            thread.CpuState->X30 = 0xCAFEBABEDEADBEEFUL;
            thread.RunFrom(Modules[0].LoadBase, 0xCAFEBABEDEADBEEFUL);
        } else
            _ = new Rtld(Modules);
    }
}