// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Aarch64Cpu;
using NxTestEmu;

unsafe {
    var _state = new CpuState();
    var state = &_state;
    state->SP = (ulong) Marshal.AllocHGlobal(0x100000) + 0x100000 - 8;
    state->TlsBase = (ulong) Marshal.AllocHGlobal(0x200);
    *(ulong*) (state->TlsBase + 0x1D0) = 0xDEA0_00000000;
    *(ulong*) (state->TlsBase + 0x1E0) = 0xDEA1_00000000;
    *(ulong*) (state->TlsBase + 0x1E8) = 0xDEA2_00000000;
    *(ulong*) (state->TlsBase + 0x1F0) = 0xDEA3_00000000;
    *(ulong*) (state->TlsBase + 0x1F8) = 0xDEA4_00000000;
    var callbacks = new Callbacks();
    callbacks.debug = Marshal.GetFunctionPointerForDelegate<DebugDelegate>((pc, dasm) => {
        Console.WriteLine($"\t X0: {state->X0:X016}  X1: {state->X1:X016}");
        Console.WriteLine($"\t X2: {state->X2:X016}  X3: {state->X3:X016}");
        Console.WriteLine($"\t X4: {state->X4:X016}  X5: {state->X5:X016}");
        Console.WriteLine($"\t X6: {state->X6:X016}  X7: {state->X7:X016}");
        Console.WriteLine($"\t X8: {state->X8:X016}  X9: {state->X9:X016}");
        Console.WriteLine($"\tX10: {state->X10:X016} X11: {state->X11:X016}");
        Console.WriteLine($"\tX12: {state->X12:X016} X13: {state->X13:X016}");
        Console.WriteLine($"\tX14: {state->X14:X016} X15: {state->X15:X016}");
        Console.WriteLine($"\tX16: {state->X16:X016} X17: {state->X17:X016}");
        Console.WriteLine($"\tX18: {state->X18:X016} X19: {state->X19:X016}");
        Console.WriteLine($"\tX20: {state->X20:X016} X21: {state->X21:X016}");
        Console.WriteLine($"\tX20: {state->X20:X016} X23: {state->X23:X016}");
        Console.WriteLine($"\tX20: {state->X20:X016} X25: {state->X25:X016}");
        Console.WriteLine($"\tX20: {state->X20:X016} X27: {state->X27:X016}");
        Console.WriteLine($"\tX20: {state->X20:X016} X29: {state->X29:X016}");
        Console.WriteLine($"\tX30: {state->X30:X016}  SP: {state->SP:X016}");
        Console.WriteLine($"\tN: {state->NZCV_N}");
        Console.WriteLine($"\tZ: {state->NZCV_Z}");
        Console.WriteLine($"\tC: {state->NZCV_C}");
        Console.WriteLine($"\tV: {state->NZCV_V}");
        Console.WriteLine($"{pc:X}: {dasm}");
    });
    unsafe {
        callbacks.loadModule = Marshal.GetFunctionPointerForDelegate<LoadModuleDelegate>((loadAddr, data, size) => {
            Console.WriteLine($"Attempting to load module at {loadAddr:X} -- {size:X} bytes");
            // PROT_READ | PROT_WRITE
            // MAP_ANON | MAP_FIXED | MAP_PRIVATE
            var ptr = LibWrapper.mmap(loadAddr, (ulong) size, 3, 0x1000 | 0x0010 | 0x0002, -1, 0);
            Debug.Assert(ptr == loadAddr);
            Buffer.MemoryCopy(data, (void*) ptr, size, size);
        });
    }

    callbacks.readSr = Marshal.GetFunctionPointerForDelegate<ReadSrDelegate>((op0, op1, crn, crm, op2) => {
        Console.WriteLine("Getting SR!");
        var reg = ((0b10 | op0) << 14) | (op1 << 11) | (crn << 7) | (crm << 3) | op2;
        return reg switch {
            0b11_011_0000_0000_001 => // CtrEl0
                0x8444c004,
            0b11_011_0100_0100_000 => // FPCR
                0,
            0b11_011_0100_0100_001 => // FPSR
                0,
            0b11_011_1101_0000_011 => // TPIDR
                state->TlsBase,
            0b11_011_1110_0000_001 => // CntpctEl0
                0,
            0b11_011_0000_0000_111 => // DCZID_EL0
                0,
            _ => throw new NotSupportedException($"Unknown SR: S{op0 | 2}_{op1}_{crn}_{crm}_{op2}")
        };
    });

    callbacks.svcGetInfo = Marshal.GetFunctionPointerForDelegate<GetInfoDelegate>((infoType, handle, infoSubtype, ref info) => {
        Console.WriteLine($"Attempting to getinfo {infoType:X} {infoSubtype:X} handle {handle:X}");
        return 0;
    });

    callbacks.svcSetHeapSize = Marshal.GetFunctionPointerForDelegate<SetHeapSizeDelegate>((size, ref addr) => {
        Console.WriteLine($"Setting heap size: 0x{size:X}");
        addr = (ulong) Marshal.AllocHGlobal((int) size);
        return 0;
    });

    callbacks.svcSetMemoryPermission = Marshal.GetFunctionPointerForDelegate<SetMemoryPermissionDelegate>((addr, size, perms) => {
        Console.WriteLine($"Setting memory permissions for 0x{addr:X} -- size 0x{size:X}, perms 0x{perms:X}");
        return 0;
    });

    callbacks.svcSetMemoryAttribute = Marshal.GetFunctionPointerForDelegate<SetMemoryAttributeDelegate>((addr, size, mask, value) => {
        Console.WriteLine($"Setting memory permissions for 0x{addr:X} -- size 0x{size:X}, mask 0x{mask:X} value 0x{value:X}");
        return 0;
    });

    callbacks.svcQueryMemory = Marshal.GetFunctionPointerForDelegate<QueryMemoryDelegate>((ptr, addr, ref pinfo) => {
        Console.WriteLine($"Attempting to query memory for 0x{addr:X}");
        return 0;
    });

    callbacks.svcArbitrateUnlock = Marshal.GetFunctionPointerForDelegate<ArbitrateUnlockDelegate>(addr => {
        Console.WriteLine($"Attempting to arbitrate unlock for 0x{addr:X}");
        return 0;
    });

    callbacks.svcConnectToNamedPort = Marshal.GetFunctionPointerForDelegate<ConnectToNamedPortDelegate>((namePtr, ref handle) => {
        Console.WriteLine($"Attempting to connect to named port! '{Encoding.ASCII.GetString(new Span<byte>((void*) namePtr, 64)).Split('\0')[0]}'");
        throw new NotImplementedException();
    });

    // Env config
    state->X0 = (ulong) Marshal.AllocHGlobal(6 * 8);
    ((ulong*) state->X0)![0] = 0x00000001; // main thread handle key
    ((ulong*) state->X0)![1] = 0xdeadbeef;
    ((ulong*) state->X0)![2] = 0;
    ((ulong*) state->X0)![3] = 0; // end of list
    state->X1 = ulong.MaxValue;
    LibWrapper.setup(ref callbacks);
    LibWrapper.runFrom(state, 0x71_00000000, 0);
}

[StructLayout(LayoutKind.Sequential)]
struct ConfigEntry {
    public uint Key, Flags;
    public ulong Value0, Value1;
}

static unsafe partial class LibWrapper {
    [LibraryImport("../NxRecompile/libtest.dylib")]
    public static partial void setup(ref Callbacks callbacks);
    [LibraryImport("../NxRecompile/libtest.dylib")]
    public static partial void runFrom(CpuState* state, ulong addr, ulong until);

    [DllImport("libSystem.dylib")]
    public static extern ulong mmap(ulong addr, ulong len, int prot, int flags, int fd, ulong offset);
}