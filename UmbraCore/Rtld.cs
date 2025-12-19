using System.Diagnostics;
using System.Runtime.InteropServices;
using LibSharpRetro;
using NxCommon;
using UmbraCore.Kernel;

namespace UmbraCore;

delegate ulong MallocHook(ulong size);
public class Rtld {
    public readonly Dictionary<string, ulong> SymbolAddrs = [];

    public unsafe Rtld(List<ExeModule> modules) {
        foreach(var module in modules)
            foreach(var symbol in module.Symbols) {
                if(symbol.Value == 0 || symbol.Name == "") continue;
                SymbolAddrs[symbol.Name] = module.LoadBase + symbol.Value;
                Kernel.Kernel.Symbols[(module.LoadBase + symbol.Value, module.LoadBase + symbol.Value + symbol.Size)] = symbol.Name;
            }
        foreach(var (name, (_, addr)) in Kernel.Kernel.HookManager.Hooks)
            SymbolAddrs[name] = (Kernel.Kernel.IsNative ? 0 : 0x8000_0000_0000_0000UL) | addr;
        foreach(var module in modules) {
            if(module.Dynamic.TryGetValue(DynamicKey.REL, out var start)) {
                var rels = module.Binary.Read<ulong, uint, uint>(start, (int) module.Dynamic[DynamicKey.RELCOUNT]);
                foreach(var (offset, type, sym) in rels)
                    Relocate(module, offset, type, sym, 0, true);
            }
            if(module.Dynamic.TryGetValue(DynamicKey.RELA, out start)) {
                var rels = module.Binary.Read<ulong, uint, uint, long>(start, (int) module.Dynamic[DynamicKey.RELASZ] / 0x18);
                foreach(var (offset, type, sym, addend) in rels)
                    Relocate(module, offset, type, sym, addend, false);
            }
            if(
                module.Dynamic.TryGetValue(DynamicKey.PLTREL, out var pltType) &&
                module.Dynamic.TryGetValue(DynamicKey.JMPREL, out start)
            ) {
                var relocs = pltType == 7 // DT_RELA
                    ? module.Binary.Read<ulong, uint, uint, long>(start, (int) module.Dynamic[DynamicKey.PLTRELSZ] / 24)
                    : module.Binary.Read<ulong, uint, uint>(start, (int) module.Dynamic[DynamicKey.PLTRELSZ] / 16)
                        .Select(x => (x.Item1, x.Item2, x.Item3, 0L)).ToArray();
                foreach(var (offset, type, sym, addend) in relocs)
                    Relocate(module, offset, type, sym, addend, pltType != 7);
            }
            if(module.Dynamic.TryGetValue(DynamicKey.PLTGOT, out var got)) {
                var plt = (ulong*) (module.LoadBase + got);
                plt[1] = 0xCAFEBABE;
                plt[2] = 0xCAFEBABF;
            }
        }

        var thread = Kernel.Kernel.ThreadManager.CurrentThread;

        void Run(string name, ulong x0 = 0, ulong x1 = 0, ulong x2 = 0, ulong x3 = 0) {
            thread.CpuState->X0 = x0;
            thread.CpuState->X1 = x1;
            thread.CpuState->X2 = x2;
            thread.CpuState->X3 = x3;
            thread.CpuState->X30 = 0;
            Console.WriteLine($"Running {name}");
            if(SymbolAddrs.TryGetValue(name, out var addr))
                thread.RunFrom(addr, 0);
            else
                Console.WriteLine($"Warning: Couldn't find init symbol '{name}'");
            Console.WriteLine("Done");
        }

        void NotifyExceptionHandlerReady() =>
            Console.WriteLine("Exception handler ready!");
        void RunInitializers() {
            Console.WriteLine("Running initializers!");
            foreach(var (i, module) in modules.Index()) {
                if(module.Dynamic.TryGetValue(DynamicKey.INIT, out var init)) {
                    Console.WriteLine($"Running init function: {module.LoadBase + init:X} (0x{0x71_00000000UL + 0x1_00000000UL * (ulong) i + init:X})");
                    thread.CpuState->X30 = 0xCAFEBABEDEADBEEFUL;
                    thread.RunFrom(module.LoadBase + init, 0xCAFEBABEDEADBEEFUL);
                    Console.WriteLine("Done");
                }
            }
        }

        if(SymbolAddrs.ContainsKey("_ZN2nn4init5StartEmmPFvvES2_")) {
            Run(
                "_ZN2nn4init5StartEmmPFvvES2_",
                0xf00b,
                0x13_0000_0F00,
                (ulong) Marshal.GetFunctionPointerForDelegate(NotifyExceptionHandlerReady),
                (ulong) Marshal.GetFunctionPointerForDelegate(RunInitializers)
            );
        } else {
            Run("__nnDetailInitLibc0");
            Run("nnosInitialize", 0xf00b, 0xdeadbee0);
            Run("__nnDetailInitLibc1");
            Run("nndiagStartup");
            Run("nninitInitializeSdkModule");
            Run("nninitInitializeAbortObserver");
            Run("nninitStartup");
            Run("__nnDetailInitLibc2");
            RunInitializers();
            Run("nnMain");
        }
    }
    
    void Relocate(ExeModule module, ulong offset, uint type, uint sym, long addend, bool isRel) =>
        Relocate(module, offset, (RelocationType) type, sym, addend, isRel);
    unsafe void Relocate(ExeModule module, ulong offset, RelocationType type, uint sym, long addend, bool isRel) {
        if(sym == 0) return;
        ulong addr;
        var symbol = module.Symbols[(int) sym];
        if(symbol.Value != 0)
            addr = module.LoadBase + symbol.Value;
        else {
            var name = symbol.Name;
            if(!SymbolAddrs.TryGetValue(name, out addr)) {
                Console.WriteLine($"Warning: Couldn't resolve symbol '{name}'");
                return;
            }
        }

        switch(type) {
            case RelocationType.R_AARCH64_JUMP_SLOT:
            case RelocationType.R_AARCH64_ABS64:
            case RelocationType.R_AARCH64_GLOB_DAT:
                *(ulong*) (module.LoadBase + offset) = unchecked(addr + (ulong) addend);
                break;
            default:
                throw new NotSupportedException($"Rtld relocation type {type} not supported");
        }
    }
}