// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Runtime.InteropServices;
using Aarch64Cpu;
using NxTestEmu;

unsafe {
    var _state = new CpuState();
    var state = &_state;
    state->SP = (ulong) Marshal.AllocHGlobal(0x100000) + 0x100000 - 8;
    var callbacks = new Callbacks();
    callbacks.debug = Marshal.GetFunctionPointerForDelegate<DebugDelegate>((pc, dasm) => {
        Console.WriteLine($"{pc:X}: {dasm}");
        /*Console.WriteLine($"X0: {state->X0:X}");
        Console.WriteLine($"X1: {state->X1:X}");
        Console.WriteLine($"N: {state->NZCV_N}");
        Console.WriteLine($"Z: {state->NZCV_Z}");
        Console.WriteLine($"C: {state->NZCV_C}");
        Console.WriteLine($"V: {state->NZCV_V}");*/
    });
    unsafe {
        callbacks.loadModule = Marshal.GetFunctionPointerForDelegate<LoadModuleDelegate>((loadAddr, data, size) => {
            Console.WriteLine($"Attempting to load module at {loadAddr:X} -- {size:X} bytes");
            // PROT_READ | PROT_WRITE
            // MAP_ANON | MAP_FIXED | MAP_PRIVATE
            var ptr = LibWrapper.mmap(loadAddr, (ulong) size, 3, 0x1000 | 0x0010 | 0x0002, -1, 0);
            Debug.Assert(ptr == loadAddr);
            Buffer.MemoryCopy((void*) data, (void*) ptr, size, size);
        });
    }

    state->X0 = (ulong) Marshal.AllocHGlobal(3 * 8); // Empty config env
    state->X1 = ulong.MaxValue;
    LibWrapper.setup(state, ref callbacks);
    LibWrapper.runFrom(0x71_00000000, 0);
}

[StructLayout(LayoutKind.Sequential)]
struct ConfigEntry {
    public uint Key, Flags;
    public ulong Value0, Value1;
}

static unsafe class LibWrapper {
    [DllImport("../NxRecompile/libtest.dylib")]
    public static extern void setup(CpuState* state, ref Callbacks callbacks);
    [DllImport("../NxRecompile/libtest.dylib")]
    public static extern void runFrom(ulong addr, ulong until);

    [DllImport("libSystem.dylib")]
    public static extern ulong mmap(ulong addr, ulong len, int prot, int flags, int fd, ulong offset);
}