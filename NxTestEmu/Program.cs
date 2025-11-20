// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;
using Aarch64Cpu;
using NxTestEmu;

var state = new CpuState();
var callbacks = new Callbacks();
callbacks.debug = Marshal.GetFunctionPointerForDelegate<DebugDelegate>((pc, dasm) => {
    Console.WriteLine($"{pc:X}: {dasm}");
});
LibWrapper.setup(ref state, ref callbacks);
LibWrapper.runFrom(0x71_00000000, 0);

static class LibWrapper {
    [DllImport("../NxRecompile/libtest.dylib")]
    public static extern void setup(ref CpuState state, ref Callbacks callbacks);
    [DllImport("../NxRecompile/libtest.dylib")]
    public static extern void runFrom(ulong addr, ulong until);
}