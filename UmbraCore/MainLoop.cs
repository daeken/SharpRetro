using System.Runtime.InteropServices;
using System.Text;
using LibSharpRetro;
using NxCommon;
using UmbraCore.Core;
using static UmbraCore.L;

namespace UmbraCore;

public class MainLoop {
    public static MainLoop Instance;
    
    public readonly GameWrapper Game;
    public readonly List<ExeModule> Modules = [];
    
    public unsafe MainLoop(string libPath, string romFsPath) {
        Instance = this;
        Game = new GameWrapper(libPath);

        Game.Callbacks.Debug = pc => {
            Log(() => {
                $"Running from 0x{pc:X}".Log();
                var thread = Kernel.ThreadManager.CurrentThread;
                for(var i = 0; i < 31; ++i) {
                    Console.Write($"X{i}: 0x{thread.CpuState->X[i]:X} ");
                    if(i != 0 && i % 4 == 0)
                        Console.WriteLine();
                }
                Console.WriteLine();
            });
        };
        Game.Callbacks.LoadModule =
            (loadBase, data, size, textStart, textEnd, roStart, roEnd, dataStart, dataEnd) => {
                $"Loading module at 0x{loadBase:X}".Log();
                Kernel.MemoryManager.Regions[loadBase] = (size, 0);
                MemoryHelpers.Mmap(loadBase, size, requirePosition: true);
                Buffer.MemoryCopy(data, (void*) loadBase, size, size);
                Modules.Add(new(loadBase, size, textStart, textEnd, roStart, roEnd, dataStart, dataEnd));
            };
        Game.Callbacks.InitModule = (loadBase, size) => {
            Kernel.IsNative = true;
            $"Module loaded at 0x{loadBase:X}-0x{loadBase+size:X}".Log();
            Kernel.MemoryManager.Regions[loadBase] = (size, 0);
            Modules.Add(new(loadBase, size, doRelocate: false));
        };
        Game.Callbacks.WriteSr = (op0, op1, crn, crm, op2, value) => {
            var reg = ((0b10 | (op0 & 0b1)) << 14) | ((op1 & 0b111) << 11) | ((crn & 0b1111) << 7) | ((crm & 0b1111) << 3) | (op2 & 0b111);
            $"WriteSR attempted {reg:X} {value:X}".Log();
            switch(reg) {
                case 0b11_011_1101_0000_010: // TPIDR
                    $"Writing TPIDR: {value:X}".Log();
                    if(value != 0)
                        Kernel.ThreadManager.CurrentThread.TlsBase = (IntPtr) value;
                    break;
                default:
                    $"Unhandled SR write: {reg:X} {value:X}".Log();
                    break;
            }
        };
        Game.Callbacks.ReadSr = (op0, op1, crn, crm, op2) => {
            var reg = ((0b10 | (op0 & 0b1)) << 14) | ((op1 & 0b111) << 11) | ((crn & 0b1111) << 7) | ((crm & 0b1111) << 3) | (op2 & 0b111);
            //$"ReadSR {reg:X}".Log();
            var (found, value) = reg switch {
                0b11_011_0000_0000_001 => // CtrEl0
                    (true, 0x8444c004UL),
                0b11_011_0100_0100_000 => // FPCR
                    (true, 0UL),
                0b11_011_0100_0100_001 => // FPSR
                    (true, 0UL),
                0b11_011_1101_0000_011 => // TPIDRRO
                    (true, (ulong) Kernel.ThreadManager.CurrentThread.TlsBase),
                0b11_011_1110_0000_001 => // CntpctEl0
                    (true, 0xDEADUL),
                0b11_011_0000_0000_111 => // DCZID_EL0
                    (true, 0UL),
                _ => (false, 0UL),
            };
            if(!found) $"Unknown SR: S{op0 | 2}_{op1}_{crn}_{crm}_{op2}".Log();
            return value;
        };
        Game.Callbacks.OutputDebugString = (addr, size) => {
            $"Debug string: {Encoding.ASCII.GetString(new Span<byte>((void*) addr, (int) size))}".Log();
            return 0;
        };
        Kernel.Setup(Game, romFsPath);
        Game.Setup();
        "Done with setup!".Log();
        "Press enter to continue...".Log();
        Console.ReadLine();
        if(false) {
            var thread = Kernel.ThreadManager.CurrentThread;
            thread.CpuState->X0 = 0;
            thread.CpuState->X1 = 0xFFFF8001; // thread handle
            thread.CpuState->X30 = 0xCAFEBABEDEADBEEFUL;
            thread.RunFrom(Modules[0].LoadBase, 0xCAFEBABEDEADBEEFUL);
        } else
            _ = new Rtld(Modules);
    }
}