using System.Diagnostics;
using System.Runtime.InteropServices;
using LibSharpRetro;
using NxCommon;
using UmbraCore.Kernel;

namespace UmbraCore;

public class Rtld {
    public readonly Dictionary<string, ulong> SymbolAddrs = [];
    public unsafe Rtld(List<ExeModule> modules) {
        foreach(var module in modules)
            foreach(var symbol in module.Symbols) {
                if(symbol.Value == 0 || symbol.Name == "") continue;
                SymbolAddrs[symbol.Name] = module.LoadBase + symbol.Value;
            }
        foreach(var (name, (_, addr)) in Globals.HookManager.Hooks)
            SymbolAddrs[name] = 0x8000_0000_0000_0000UL | addr;
        foreach(var module in modules) {
            foreach(var (key, value) in module.Dynamic) Console.WriteLine($"{module.LoadBase:X} {key} -- 0x{value:X}");
            if(
                module.Dynamic.TryGetValue(DynamicKey.PLTREL, out var pltType) &&
                module.Dynamic.TryGetValue(DynamicKey.JMPREL, out var start)
            ) {
                var relocs = pltType == 7 // DT_RELA
                    ? module.Binary.Read<ulong, uint, uint, long>(start, (int) module.Dynamic[DynamicKey.PLTRELSZ] / 24)
                    : module.Binary.Read<ulong, uint, uint>(start, (int) module.Dynamic[DynamicKey.PLTRELSZ] / 16)
                        .Select(x => (x.Item1, x.Item2, x.Item3, 0L)).ToArray();
                foreach(var (offset, type, sym, addend) in relocs) {
                    Debug.Assert(sym != 0);
                    var name = module.Symbols[(int) sym].Name;
                    if(SymbolAddrs.TryGetValue(name, out var addr))
                        *(ulong*) (module.LoadBase + offset) = unchecked(addr + (ulong) addend);
                    else
                        Console.WriteLine($"Warning: Couldn't resolve symbol '{name}'");
                }
            }
            if(module.Dynamic.TryGetValue(DynamicKey.PLTGOT, out var got)) {
                var plt = (ulong*) (module.LoadBase + got);
                plt[1] = 0xCAFEBABE;
                plt[2] = 0xCAFEBABF;
            }
        }

        var thread = Globals.ThreadManager.CurrentThread;

        void Run(string name, ulong x0 = 0, ulong x1 = 0) {
            thread.CpuState->X0 = x0;
            thread.CpuState->X1 = x1;
            thread.CpuState->X30 = 0;
            Console.WriteLine($"Running {name}");
            if(SymbolAddrs.TryGetValue(name, out var addr))
                thread.RunFrom(addr, 0);
            else
                Console.WriteLine($"Warning: Couldn't find init symbol '{name}'");
            Console.WriteLine("Done");
        }
        
        Run("__nnDetailInitLibc0");
        //*(ulong*) 0x77006c45f0 = (ulong) Marshal.AllocHGlobal(0x1000); // nn::os::detail::g_OsBootParamter (sic)
        Run("nnosInitialize", 0xf00b, 0xdeadbee0);
        foreach(var module in modules.ToArray().Reverse()) {
            if(module.Dynamic.TryGetValue(DynamicKey.INIT, out var init)) {
                Console.WriteLine($"Running init function: {module.LoadBase + init:X}");
                thread.CpuState->X30 = 0xCAFEBABEDEADBEEFUL;
                thread.RunFrom(module.LoadBase + init, 0xCAFEBABEDEADBEEFUL);
                Console.WriteLine("Done");
            }
        }
    }
}