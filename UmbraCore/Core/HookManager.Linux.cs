using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace UmbraCore.Core;

// Route A v0 : managed equivalents of NativeLib/library.mm:103-138
// hooks. Mirrors sera's macOS design — intercept at the SDK symbol level via
// Rtld:54's SymbolAddrs overlay, so the SDK's nvn/nvdrv path never runs.
// nv::InitializeGraphics → no-op (this alone makes my walls 1, 4-7 unreachable;
// on macOS the .mm hook is exactly this). nvnBootstrapLoader → managed
// GetProcAddress that hands back behavior-only stubs per nvn fn name. nn::vi::*
// → return-0 stubs.
// The nvn stub pool (NvnStubs.cs once it exists) is what the NVN call census maps:
// 144 used fns, signatures from nvn.h, callers from xrefs-indirect. For v0
// every name resolves to one universal log-return-1 stub; the FIRST game-check
// that fails on that = , and static analysis on the callsite tells us
// what return shape it expects.

public unsafe partial class HookManager {

    // : SIGSEGV in native game code. ~4GB process
    // → coredump takes minutes (looked like a hang under `timeout -s KILL`).
    // gdb live-attach hung ×3. Managed [UnmanagedCallersOnly] handler ×4
    // attempts all installed (rc=0) but never visibly fired — CoreCLR's
    // reverse-pinvoke stub (native→managed GC-mode transition) runs BEFORE
    // any UCO method body and is NOT async-signal-safe → secondary SEGV in
    // libcoreclr masks the original (verified via dmesg print-fatal-signals:
    // pc/lr both in libcoreclr.so when the "bare write(2)" managed handler
    // was armed). A managed signal handler is structurally wrong.
    // Native C handler (UmbraCore/Native/segvtrap.c → /tmp/libsegvtrap.so) = the only
    // correct shape: write(2) + _exit only, prints fault/pc/lr/x0..x29 + a
    // 12-frame fp-walk. Installed from nv::InitializeGraphics hook = late,
    // post-CoreCLR-PAL + post-game-signal-setup, in game-thread context.
    // Build: gcc -shared -fPIC -O2 -o /tmp/libsegvtrap.so UmbraCore/Native/segvtrap.c
    [DllImport("/tmp/libsegvtrap.so")] static extern void segvtrap_install();
    [DllImport("/tmp/libsegvtrap.so")] static extern void segvtrap_set_game_range(ulong lo, ulong hi);

    public static void InstallSegvHandler() {
        try {
            segvtrap_install();
            // (ε') tell segvtrap which pc-range is game code, so it
            // can CHAIN to CoreCLR's handler (→ NRE) for managed
            // faults instead of dump+exit. Use the symbol span (=
            // game .text+.data; same bound HidPump.DumpRuntimeImage
            // uses). Done here because RegisterLinuxHooks() fires
            // from nv::InitializeGraphics → post-Image.Load, so
            // Kernel.Symbols is populated.
            if(Kernel.Symbols.Count > 0) {
                var lo = Kernel.Symbols.Keys.Min(k => k.Start);
                var hi = Kernel.Symbols.Keys.Max(k => k.End);
                segvtrap_set_game_range(lo, hi);
                // (ε') §7-test: deliberate managed-pc null-deref
                // RIGHT AFTER range-set. If chain works, CoreCLR
                // converts to NRE → caught here → run continues.
                // If not, segvtrap dumps+exits (rc=139) and the
                // log shows pc=0xfffff0… (managed JIT) = ✗.
                if(Environment.GetEnvironmentVariable(
                        "UMBRA_TEST_NRE") == "1") {
                    $"[segv] (ε') §7: deliberate *(ulong*)0x10…".Log();
                    try {
                        unsafe {
                            var x = *(ulong*)0x10;
                            $"  (read 0x{x:x} — shouldn't reach)".Log();
                        }
                    } catch(Exception e) {
                        $"[segv] (ε') §7 ✓ caught: {e.GetType().Name}: {e.Message}".Log();
                    }
                }
            }
        }
        catch(Exception e) { $"[segv] native trap not available: {e.Message}".Log(); }
    }

    partial void RegisterLinuxHooks() {
        InstallSegvHandler();
        // — nv:: layer (makes nvdrv unreachable)
        RegisterRaw("_ZN2nv18InitializeGraphicsEPvm",
            (nint)(delegate* unmanaged<void*, ulong, void>)&Nv_InitializeGraphics);
        RegisterRaw("_ZN2nv20SetGraphicsAllocatorEPFPvmmS0_EPFvS0_S0_EPFS0_S0_mS0_ES0_",
            (nint)(delegate* unmanaged<void*, void*, void*, void*, void>)&Nv_SetGraphicsAllocator);

        // — nvn bootstrap → managed GetProcAddress
        RegisterRaw("nvnBootstrapLoader",
            (nint)(delegate* unmanaged<byte*, nint>)&NvnBootstrapLoader);

        // — nn::vi::* (display layer)
        RegisterRaw("_ZN2nn2vi10InitializeEv",
            (nint)(delegate* unmanaged<void>)&Vi_Initialize);
        RegisterRaw("_ZN2nn2vi11OpenDisplayEPPNS0_7DisplayEPKc",
            (nint)(delegate* unmanaged<void**, byte*, long>)&Vi_OpenDisplay);
        RegisterRaw("_ZN2nn2vi18OpenDefaultDisplayEPPNS0_7DisplayE",
            (nint)(delegate* unmanaged<void**, long>)&Vi_OpenDefaultDisplay);
        RegisterRaw("_ZN2nn2vi20GetDisplayVsyncEventEPNS_2os15SystemEventTypeEPNS0_7DisplayE",
            (nint)(delegate* unmanaged<uint*, void*, long>)&Vi_GetDisplayVsyncEvent);
        RegisterRaw("_ZN2nn2vi11CreateLayerEPPNS0_5LayerEPNS0_7DisplayE",
            (nint)(delegate* unmanaged<void**, void*, ulong>)&Vi_CreateLayer);
        RegisterRaw("_ZN2nn2vi11CreateLayerEPPNS0_5LayerEPNS0_7DisplayEii",
            (nint)(delegate* unmanaged<void**, void*, int, int, ulong>)&Vi_CreateLayerExtra);
        RegisterRaw("_ZN2nn2vi12SetLayerCropEPNS0_5LayerEiiii",
            (nint)(delegate* unmanaged<void*, int, int, int, int, ulong>)&Vi_Stub5);
        RegisterRaw("_ZN2nn2vi19SetLayerScalingModeEPNS0_5LayerENS0_11ScalingModeE",
            (nint)(delegate* unmanaged<void*, int, ulong>)&Vi_Stub2);
        RegisterRaw("_ZN2nn2vi12DestroyLayerEPNS0_5LayerE",
            (nint)(delegate* unmanaged<void*, ulong>)&Vi_Stub1);
        RegisterRaw("_ZN2nn2vi15GetNativeWindowEPPvPNS0_5LayerE",
            (nint)(delegate* unmanaged<void**, void*, long>)&Vi_GetNativeWindow);

        // — movie::SetAllocator + glslc_* — wall-N if reached
        RegisterRaw("_ZN5movie12SetAllocatorEPFPvmmS0_EPFvS0_S0_EPFS0_S0_mS0_ES0_",
            (nint)(delegate* unmanaged<void*, void*, void*, void*, void>)&Nv_SetGraphicsAllocator);
    }

    // ───────── nv:: ─────────
    [UnmanagedCallersOnly]
    static void Nv_InitializeGraphics(void* mem, ulong size) {
        $"[hook] nv::InitializeGraphics(mem={(ulong)mem:x}, size=0x{size:x}) → no-op".Log();
        // Re-arm the SIGSEGV handler from game-thread context — CoreCLR
        // installs its own (for managed null→NRE) and the game's nn::os may
        // too; this is late enough to be on top of both. .
        InstallSegvHandler();
    }

    [UnmanagedCallersOnly]
    static void Nv_SetGraphicsAllocator(void* a, void* b, void* c, void* d) =>
        "[hook] nv::SetGraphicsAllocator → no-op".Log();

    // ───────── nvn bootstrap / GetProcAddress ─────────
    // v0: every name → one universal stub. The stub returns 1 (nonzero so
    // null-checks pass; NVNboolean-returning Initialize fns read it as TRUE).
    // GetProcAddress logs each request → we see exactly the 453 nvnLoadCProcs
    // calls in order. First game-side hang/crash after that = ; the analysis tooling
    // `xrefs-indirect main pfnc_<that-fn>` + `flow` on the BLR site shows the
    // expected return shape, then a real stub gets written.
    // ‡ The universal stub takes 8 ulong args (covers all integer/pointer sigs
    // via AAPCS64 — extra args are ignored, fewer are unread). Floating args
    // (V0..V7) untouched. void-returning callers ignore X0. Out-pointer args
    // (e.g. nvnDeviceGetInteger's int* v) are NOT written → first game-check
    // on an out-param = the wall.

    static readonly Dictionary<string, nint> NvnResolved = new();
    static int _nvnReqCount;

    [UnmanagedCallersOnly]
    static nint NvnBootstrapLoader(byte* namePtr) {
        var name = Marshal.PtrToStringAnsi((nint) namePtr) ?? "?";
        $"[hook] nvnBootstrapLoader('{name}')".Log();
        if(name == "nvnDeviceGetProcAddress")
            return (nint)(delegate* unmanaged<void*, byte*, nint>)&NvnDeviceGetProcAddress;
        return ResolveNvn(name);
    }

    [UnmanagedCallersOnly]
    static nint NvnDeviceGetProcAddress(void* device, byte* namePtr) {
        var name = Marshal.PtrToStringAnsi((nint) namePtr) ?? "?";
        var n = ++_nvnReqCount;
        if(n <= 5 || n % 100 == 0)
            $"[hook] nvnDeviceGetProcAddress('{name}') [#{n}]".Log();
        return ResolveNvn(name);
    }

    // Per-name fallback thunk (instrument): each unimpl'd
    // name gets its own delegate-backed fnptr that knows its name. Replaces
    // the single universal UCO stub (which couldn't tell you WHICH of 439
    // names was being called). GetFunctionPointerForDelegate generates a
    // native→managed transition stub per delegate; the closure captures
    // `name`. Slower than UCO (full marshalling) but this is the diagnostic
    // path; hot fns get real impls in NvnLinux.Table.
    // Per-name call counts → first call + every 1000th logged with thread
    // + args. That's the discriminator for "which fn is the 10000-call poll
    // loop" and "what's main hitting now."
    delegate ulong NvnStub8(ulong x0, ulong x1, ulong x2, ulong x3,
                            ulong x4, ulong x5, ulong x6, ulong x7);
    static readonly List<Delegate> _keepAlive = new();  // GC roots for the thunks
    static readonly System.Collections.Concurrent.ConcurrentDictionary<string, int> _perNameCount = new();

    static nint MakeNamedStub(string name) {
        NvnStub8 d = (x0, x1, x2, x3, x4, x5, x6, x7) => {
            var n = _perNameCount.AddOrUpdate(name, 1, (_, c) => c + 1);
            if(n == 1 || n % 1000 == 0)
                $"[nvn-stub] {name} #{n} (t={Thread.CurrentThread.ManagedThreadId} x0={x0:x} x1={x1:x} x2={x2:x} x3={x3:x} x4={x4:x} x5={x5:x})".Log();
            return 1;
        };
        lock(_keepAlive) _keepAlive.Add(d);
        return Marshal.GetFunctionPointerForDelegate(d);
    }

    static nint ResolveNvn(string name) {
        if(NvnResolved.TryGetValue(name, out var p)) return p;
        if(name == "nvnDeviceGetProcAddress")
            p = (nint)(delegate* unmanaged<void*, byte*, nint>)&NvnDeviceGetProcAddress;
        else if(NvnLinux.Table.TryGetValue(name, out var impl) && impl != 0)
            p = impl;  // 0 = "use the named-stub" (gated hooks)
        else
            p = MakeNamedStub(name);
        NvnResolved[name] = p;
        return p;
    }

    // Universal stub (logged by total count, no name) replaced by
    // MakeNamedStub above — each unknown name gets its own delegate-thunk
    // that logs its name. Keeping the count for runtime-summary:
    public static IEnumerable<(string name, int n)> NvnStubCounts =>
        _perNameCount.OrderByDescending(kv => kv.Value).Select(kv => (kv.Key, kv.Value));

    // ───────── nn::vi::* ─────────
    [UnmanagedCallersOnly] static void Vi_Initialize() =>
        "[hook] nn::vi::Initialize → no-op".Log();
    [UnmanagedCallersOnly] static long Vi_OpenDisplay(void** disp, byte* name) {
        $"[hook] nn::vi::OpenDisplay('{Marshal.PtrToStringAnsi((nint)name)}')".Log();
        if(disp != null) *disp = (void*) 0xD15_0001;  // fake handle, nonzero
        return 0;
    }
    [UnmanagedCallersOnly] static long Vi_OpenDefaultDisplay(void** disp) {
        "[hook] nn::vi::OpenDefaultDisplay".Log();
        if(disp != null) *disp = (void*) 0xD15_0001;
        return 0;
    }
    [UnmanagedCallersOnly] static long Vi_GetDisplayVsyncEvent(uint* ev, void* disp) {
        "[hook] nn::vi::GetDisplayVsyncEvent ‡ fake event handle".Log();
        if(ev != null) *ev = 0;  // ‡ should be a real KEvent handle for WaitSync
        return 0;
    }
    [UnmanagedCallersOnly] static ulong Vi_CreateLayer(void** layer, void* disp) {
        "[hook] nn::vi::CreateLayer".Log();
        if(layer != null) *layer = (void*) 0x1A7E_0001;
        return 0;
    }
    [UnmanagedCallersOnly] static ulong Vi_CreateLayerExtra(void** layer, void* disp, int w, int h) {
        $"[hook] nn::vi::CreateLayer({w}×{h})".Log();
        if(layer != null) *layer = (void*) 0x1A7E_0001;
        return 0;
    }
    [UnmanagedCallersOnly] static long Vi_GetNativeWindow(void** win, void* layer) {
        "[hook] nn::vi::GetNativeWindow".Log();
        if(win != null) *win = (void*) 0x1A7E_0002;
        return 0;
    }
    [UnmanagedCallersOnly] static ulong Vi_Stub1(void* a) => 0;
    [UnmanagedCallersOnly] static ulong Vi_Stub2(void* a, int b) => 0;
    [UnmanagedCallersOnly] static ulong Vi_Stub5(void* a, int b, int c, int d, int e) => 0;
}
