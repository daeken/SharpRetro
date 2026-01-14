#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
using System.Runtime.InteropServices;
using Aarch64Cpu;

namespace UmbraCore.Core;

public unsafe delegate ulong HookDelegate(CpuState* cpuState);
[AttributeUsage(AttributeTargets.Method)]
public class Hook(string Symbol) : Attribute;

public unsafe partial class HookManager {
    public readonly Dictionary<string, (HookDelegate Delegate, ulong FuncPtr)> Hooks = [];
    delegate void SetupDelegate(NativeLibCallbacks* callbacks);
    delegate void RegisterHookDelegate(IntPtr namePtr, ulong funcPtr);
    delegate IntPtr GetSdlWindowDelegate();
    delegate IntPtr GetSdlRendererDelegate();
    delegate IntPtr RecompileShaderDelegate(IntPtr shader, ulong size);
    delegate void FreeShaderDelegate(IntPtr shader);

    [StructLayout(LayoutKind.Sequential)]
    struct NativeLibCallbacks {
        public RegisterHookDelegate RegisterHook;
        public GetSdlWindowDelegate GetSdlWindow;
        public GetSdlRendererDelegate GetSdlRenderer;
        public RecompileShaderDelegate RecompileShader;
        public FreeShaderDelegate FreeShader;
    }

    readonly NativeLibCallbacks* Callbacks;

    public ulong LoadX18, StoreX18;

    public HookManager() {
        InitializeWrappers();
        var lib = NativeLibrary.Load("../UmbraCore/NativeLib/cmake-build-debug/libNativeLib.dylib");
        Callbacks = (NativeLibCallbacks*) Marshal.AllocHGlobal(sizeof(NativeLibCallbacks));
        Callbacks->RegisterHook = NativeRegister;
        Callbacks->GetSdlWindow = GetSdlWindow;
        Callbacks->GetSdlRenderer = GetSdlRenderer;
        Callbacks->RecompileShader = RecompileShader;
        Callbacks->FreeShader = FreeShader;
        var setup = Marshal.GetDelegateForFunctionPointer<SetupDelegate>(NativeLibrary.GetExport(lib, "setup"));
        setup(Callbacks);
    }

    IntPtr GetSdlWindow() => Kernel.Renderer.SdlWindow;
    IntPtr GetSdlRenderer() => Kernel.Renderer.SdlRenderer;

    IntPtr RecompileShader(IntPtr shader, ulong size) {
        throw new NotImplementedException();
    }
    void FreeShader(IntPtr shader) {
        throw new NotImplementedException();
    }

    void NativeRegister(IntPtr namePtr, ulong funcPtr) {
        var name = Marshal.PtrToStringAnsi(namePtr)!;
        if(name.StartsWith('$')) {
            switch(name) {
                case "$getX18":
                    LoadX18 = funcPtr;
                    break;
                case "$setX18":
                    StoreX18 = funcPtr;
                    break;
                default:
                    throw new NotSupportedException($"Unhandled magic hook name '{name}'");
            }
            return;
        }
        if(name.StartsWith("_Z"))
            $"Registering native hook for {name} [{CxxDemangler.CxxDemangler.Demangle(name)}] -- 0x{funcPtr:X}".Log();
        else
            $"Registering native hook for {name} -- 0x{funcPtr:X}".Log();
        Hooks[name] = (null, funcPtr);
    }

    public void Register(string symbol, HookDelegate hook) =>
        Hooks[symbol] = (hook, (ulong) Marshal.GetFunctionPointerForDelegate(hook));
}