#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
using System.Runtime.InteropServices;
using Aarch64Cpu;

namespace UmbraCore.Kernel;

public unsafe delegate ulong HookDelegate(CpuState* cpuState);
[AttributeUsage(AttributeTargets.Method)]
public class Hook(string Symbol) : Attribute;

public partial class HookManager {
    public readonly Dictionary<string, (HookDelegate Delegate, ulong FuncPtr)> Hooks = [];

    public HookManager() => InitializeWrappers();
    
    public void Register(string symbol, HookDelegate hook) =>
        Hooks[symbol] = (hook, (ulong) Marshal.GetFunctionPointerForDelegate(hook));
}