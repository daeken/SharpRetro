using System.Runtime.InteropServices;
using Aarch64Cpu;

namespace UmbraCore.Kernel;

public unsafe class KThread {
    public CpuState* CpuState;
    public readonly IntPtr State, Stack, TlsBase;

    public unsafe KThread(ulong stackSize = 8 * 1024 * 1024) {
        State = Marshal.AllocHGlobal(Marshal.SizeOf<CpuState>());
        CpuState = (CpuState*) State;
        Stack = Marshal.AllocHGlobal((int) stackSize);
        CpuState->SP = (ulong) Stack + stackSize;
        TlsBase = Marshal.AllocHGlobal(0x11C0);
        CpuState->TlsBase = (ulong) TlsBase;
        var tls = (ulong*) CpuState->TlsBase;
        tls[63] = CpuState->TlsBase + 0x1000;
        tls = (ulong*) tls[63];
        tls[9] = tls[10] = CpuState->SP;
        tls[11] = stackSize;
        tls[54] = 0xcafe000f; // thread handle
    }

    public unsafe void RunFrom(ulong addr, ulong until) {
        MainLoop.Instance.Game.RunFrom(CpuState, addr, until);
    }
}

public class ThreadManager {
    public readonly List<KThread> Threads = [];
    readonly ThreadLocal<KThread> _CurrentThread = new();
    public KThread CurrentThread => _CurrentThread.Value;

    public ThreadManager() {
        Threads.Add(_CurrentThread.Value = new());
    }

    public void Setup(GameWrapper game) {
        game.Callbacks.GetThreadPriority = (handle, ref priority) => {
            priority = 5;
            return 0;
        };
    }
}