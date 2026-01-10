using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LibSharpRetro;
using NxCommon;
using UmbraCore.Core;

namespace UmbraCore;

public class Rtld {
    public readonly Dictionary<string, ulong> SymbolAddrs = [];
    public readonly Dictionary<ulong, string> ReverseSymbols = [];

    static unsafe ulong AbortImpl(byte* first, byte* second, byte* third, ulong errCode, byte* fourth) {
        throw new Exception(
            $"Aborted: '{Marshal.PtrToStringAnsi((IntPtr) first)!.Trim()}' " + 
            $"'{Marshal.PtrToStringAnsi((IntPtr) second)!.Trim()}' " + 
            $"'{Marshal.PtrToStringAnsi((IntPtr) third)!.Trim()}' 0x{errCode:X} '{Marshal.PtrToStringAnsi((IntPtr) fourth)!.Trim()}'");
    }

    static unsafe ulong AllocateSpace(void* ignore, ulong asize) {
        $"Replacement for nn::os::detail::AddressSpaceAllocator::AllocateSpace fired for size 0x{asize:X}".Log();
        while(asize % 16384 != 0) asize++;
        asize += 0x8000; // guard pages
        foreach(var (start, size, exists, _, _) in MemoryHelpers.GetAllRegions())
            if(start >= 0x1_0000_0000 && size >= asize && !exists)
                return start + 0x4000; // guard pages
        throw new NotSupportedException();
    }

    static unsafe ulong OutputAccessLogBare(
        uint result, ulong tick, ulong tick2, byte* func, ulong unk, byte* message, byte* arg
    ) {
        //$"OutputAccessLogBare(0x{result:X}, '{Marshal.PtrToStringAnsi((IntPtr) func)}', 0x{unk:X}, '{Marshal.PtrToStringAnsi((IntPtr) message)}', '{Marshal.PtrToStringAnsi((IntPtr) arg)}')".Log();
        return 0;
    }

    static unsafe ulong* IsEnabledAccessLog() {
        return (ulong*) 1;
    }

    static unsafe void KageLog(void* logger, char* message) {
        $"Kage log: '{Marshal.PtrToStringAnsi((IntPtr) message)!.Trim()}'".Log();
    }

    public unsafe Rtld(List<ExeModule> modules) {
        foreach(var module in modules)
            foreach(var symbol in module.Symbols) {
                if(symbol.Value == 0 || symbol.Name == "") continue;
                SymbolAddrs[symbol.Name] = module.LoadBase + symbol.Value;
                ReverseSymbols[module.LoadBase + symbol.Value] = symbol.Name;
                Kernel.Symbols[(module.LoadBase + symbol.Value, module.LoadBase + symbol.Value + symbol.Size)] = symbol.Name;
            }
        foreach(var (name, (_, addr)) in Kernel.HookManager.Hooks)
            SymbolAddrs[name] = (Kernel.IsNative ? 0 : 0x8000_0000_0000_0000UL) | addr;
        /*SymbolAddrs["_ZN2nn4diag6detail9AbortImplEPKcS3_S3_iS3_z"] = 
        SymbolAddrs["_ZN2nn4diag6detail9AbortImplEPKcS3_S3_i"] = 
        SymbolAddrs["_ZN2nn4diag6detail10VAbortImplEPKcS3_S3_iPKNS_6ResultEPKNS_2os17UserExceptionInfoES3_St9__va_list"] =
        SymbolAddrs["_ZN2nn4diag6detail9AbortImplEPKcS3_S3_iPKNS_6ResultES3_z"] = 
        SymbolAddrs["_ZN2nn4diag6detail9AbortImplEPKcS3_S3_iPKNS_6ResultEPKNS_2os17UserExceptionInfoES3_z"] = 
            (ulong) Marshal.GetFunctionPointerForDelegate(AbortImpl);*/
        SymbolAddrs["_ZN2nn2os6detail21AddressSpaceAllocator13AllocateSpaceEmm"] =
            (ulong) Marshal.GetFunctionPointerForDelegate(AllocateSpace);
        SymbolAddrs["_ZN2nn2fs6detail15OutputAccessLogENS_6ResultENS_2os4TickES4_PKcPKvS6_z"] =
            (ulong) Marshal.GetFunctionPointerForDelegate(OutputAccessLogBare);
        SymbolAddrs["_ZN2nn2fs6detail18IsEnabledAccessLogEv"] =
        SymbolAddrs["_ZN4KAGE7Filesys6Logger9IsLoggingENS1_8eLogTypeE"] = 
            (ulong) Marshal.GetFunctionPointerForDelegate(IsEnabledAccessLog);
        //SymbolAddrs["_ZN4KAGE7Filesys6Logger3LogEPKcNS1_8eLogTypeE"] = 
        //    (ulong) Marshal.GetFunctionPointerForDelegate(KageLog);
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

        var thread = Kernel.ThreadManager.CurrentThread;

        void Run(string name, ulong x0 = 0, ulong x1 = 0, ulong x2 = 0, ulong x3 = 0) {
            thread.CpuState->X0 = x0;
            thread.CpuState->X1 = x1;
            thread.CpuState->X2 = x2;
            thread.CpuState->X3 = x3;
            thread.CpuState->X30 = 0;
            $"Running {name}".Log();
            if(SymbolAddrs.TryGetValue(name, out var addr))
                thread.RunFrom(addr, 0);
            else
                $"Warning: Couldn't find init symbol '{name}'".Log();
            "Done".Log();
        }

        void NotifyExceptionHandlerReady() =>
            "Exception handler ready!".Log();
        void RunInitializers() {
            "Running initializers!".Log();
            foreach(var (i, module) in modules.OrderByDescending(x => x.LoadBase).Index()) {
                if(module.Dynamic.TryGetValue(DynamicKey.INIT, out var init)) {
                    $"Running init function: {module.LoadBase + init:X} (0x{0x71_00000000UL + 0x1_00000000UL * (ulong) i + init:X})".Log();
                    thread.CpuState->X30 = 0xCAFEBABEDEADBEEFUL;
                    thread.RunFrom(module.LoadBase + init, 0xCAFEBABEDEADBEEFUL);
                    "Done".Log();
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
        ulong Patch(ulong addr) {
            if(!ReverseSymbols.TryGetValue(addr, out var name)) return addr;
            if(!SymbolAddrs.TryGetValue(name, out var naddr) ||
               addr == naddr)
                return addr;
            $"Patching {name}".Log();
            return naddr;
        }

        var addr = 0UL;
        if(sym != 0) {
            var symbol = module.Symbols[(int) sym];
            if(symbol.Value != 0)
                addr = module.LoadBase + symbol.Value;
            else {
                var name = symbol.Name;
                if(!SymbolAddrs.TryGetValue(name, out addr)) {
                    $"Warning: Couldn't resolve symbol '{name}'".Log();
                    return;
                }
            }
        }

        switch(type) {
            case RelocationType.R_AARCH64_JUMP_SLOT:
            case RelocationType.R_AARCH64_ABS64:
            case RelocationType.R_AARCH64_GLOB_DAT:
                Debug.Assert(sym != 0);
                *(ulong*) (module.LoadBase + offset) = Patch(unchecked(addr + (ulong) addend));
                break;
            case RelocationType.R_AARCH64_RELATIVE:
                if(sym != 0) throw new NotSupportedException();
                *(ulong*) (module.LoadBase + offset) =
                    Patch(unchecked(module.LoadBase + 
                                    (isRel ? *(ulong*) (module.LoadBase + offset) : 0) + (ulong) addend));
                break;
            default:
                throw new NotSupportedException($"Rtld relocation type {type} not supported");
        }
    }
}