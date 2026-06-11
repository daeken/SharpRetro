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
        public IntPtr RegisterHook;
        public IntPtr GetSdlWindow;
        public IntPtr GetSdlRenderer;
        public IntPtr RecompileShader;
        public IntPtr FreeShader;
    }

    readonly NativeLibCallbacks* Callbacks;

    public ulong LoadX18, StoreX18;

    public HookManager() {
        InitializeWrappers();
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            // LoadX18/StoreX18 stay 0 (NxTranslate Linux arm doesn't emit x18
            // trampolines). Managed hooks (Route A) registered below mirror
            // NativeLib/library.mm:103-117 — nv::InitializeGraphics no-op +
            // nvnBootstrapLoader → managed GetProcAddress + nn::vi::* stubs.
            // Route A v0
            RegisterLinuxHooks();
            return;
        }
        var lib = NativeLibrary.Load("../UmbraCore/NativeLib/cmake-build-debug/libNativeLib.dylib");
        Callbacks = (NativeLibCallbacks*) Marshal.AllocHGlobal(sizeof(NativeLibCallbacks));
        Callbacks->RegisterHook = Marshal.GetFunctionPointerForDelegate<RegisterHookDelegate>(NativeRegister);
        Callbacks->GetSdlWindow = Marshal.GetFunctionPointerForDelegate<GetSdlWindowDelegate>(GetSdlWindow);
        Callbacks->GetSdlRenderer = Marshal.GetFunctionPointerForDelegate<GetSdlRendererDelegate>(GetSdlRenderer);
        Callbacks->RecompileShader = Marshal.GetFunctionPointerForDelegate<RecompileShaderDelegate>(RecompileShader);
        Callbacks->FreeShader = Marshal.GetFunctionPointerForDelegate<FreeShaderDelegate>(FreeShader);
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

    // Raw C-ABI hook: the game calls `funcPtr` directly with the C signature
    // (PLT-patched via Rtld:54 SymbolAddrs overlay). For [UnmanagedCallersOnly]
    // statics — pass `(nint)(delegate* unmanaged<...>)&Method`. Route A
    public void RegisterRaw(string symbol, nint funcPtr) {
        $"Registering managed hook for {symbol} -- 0x{funcPtr:X}".Log();
        Hooks[symbol] = (null, (ulong) funcPtr);
    }

    partial void RegisterLinuxHooks();
}