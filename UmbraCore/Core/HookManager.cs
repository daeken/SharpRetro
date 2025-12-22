#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
using System.Runtime.InteropServices;
using Aarch64Cpu;

namespace UmbraCore.Core;

public unsafe delegate ulong HookDelegate(CpuState* cpuState);
[AttributeUsage(AttributeTargets.Method)]
public class Hook(string Symbol) : Attribute;

public partial class HookManager {
    public readonly Dictionary<string, (HookDelegate Delegate, ulong FuncPtr)> Hooks = [];
    delegate void SetupHooksDelegate(IntPtr register);
    delegate void RegisterHookDelegate(IntPtr namePtr, ulong funcPtr);

    public HookManager() {
        InitializeWrappers();
        var lib = NativeLibrary.Load("../UmbraCore/NativeLib/cmake-build-debug/libNativeLib.dylib");
        var setupHooks = Marshal.GetDelegateForFunctionPointer<SetupHooksDelegate>(NativeLibrary.GetExport(lib, "setupHooks"));
        setupHooks(Marshal.GetFunctionPointerForDelegate<RegisterHookDelegate>(NativeRegister));
    }

    void NativeRegister(IntPtr namePtr, ulong funcPtr) {
        var name = Marshal.PtrToStringAnsi(namePtr)!;
        if(name.StartsWith("_Z"))
            Console.WriteLine($"Registering native hook for {name} [{CxxDemangler.CxxDemangler.Demangle(name)}] -- 0x{funcPtr:X}");
        else
            Console.WriteLine($"Registering native hook for {name} -- 0x{funcPtr:X}");
        Hooks[name] = (null, funcPtr);
    }

    public void Register(string symbol, HookDelegate hook) =>
        Hooks[symbol] = (hook, (ulong) Marshal.GetFunctionPointerForDelegate(hook));
}