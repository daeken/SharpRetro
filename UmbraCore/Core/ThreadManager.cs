using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Aarch64Cpu;

namespace UmbraCore.Core;

public unsafe class KThread : KObject {
    public CpuState* CpuState;
    public readonly IntPtr State, Stack;
    public IntPtr TlsBase;

    public unsafe KThread(ulong stackSize = 8 * 1024 * 1024) {
        State = Marshal.AllocHGlobal(Marshal.SizeOf<CpuState>());
        CpuState = (CpuState*) State;
        Stack = Marshal.AllocHGlobal((int) stackSize);
        CpuState->SP = (ulong) Stack + stackSize;
        TlsBase = Marshal.AllocHGlobal(0x2000);
        CpuState->TlsBase = (ulong) TlsBase;
        Unsafe.InitBlockUnaligned((void*) TlsBase, 0, 0x2000);
        $"TLS base: {TlsBase:X}".Log();
        /*var tls = (ulong*) CpuState->TlsBase;
        tls[63] = CpuState->TlsBase + 0x1000;
        tls = (ulong*) tls[63];
        tls[9] = tls[10] = CpuState->SP;
        tls[11] = stackSize;
        tls[54] = Handle; // thread handle*/
    }

    public unsafe void RunFrom(ulong addr, ulong until) {
        MainLoop.Instance.Game.RunFrom(CpuState, addr, until);
    }
}

class SpawnedThread : KThread {
    public readonly ulong Entrypoint;
    public unsafe SpawnedThread(ulong entrypoint, ulong context, ulong sp) {
        Entrypoint = entrypoint;
        CpuState->X0 = context;
        CpuState->SP = sp;
    }
}

public class ThreadManager {
    public readonly List<KThread> Threads = [];
    readonly ThreadLocal<KThread> _CurrentThread = new();
    public KThread CurrentThread => _CurrentThread.Value;

    public ThreadManager() {
        Threads.Add(_CurrentThread.Value = new());
    }

    public unsafe void Setup(GameWrapper game) {
        game.Callbacks.GetThreadPriority = (handle, ref priority) => {
            priority = 5;
            return 0;
        };
        game.Callbacks.SetThreadPriority = (handle, priority) => 0;
        game.Callbacks.GetThreadId = (handle, ref id) => {
            id = 0xDEADBEE1;
            return 0;
        };
        game.Callbacks.SetThreadCoreMask = (handle, in1, in2) => 0;
        game.Callbacks.GetCurrentProcessorNumber = () => 0;
        game.Callbacks.CreateThread = (entrypoint, threadContext, stackTop, priority, coreId, ref handle) => {
            $"Creating thread with entrypoint 0x{entrypoint:X}".Log();
            var spawned = new SpawnedThread(entrypoint, threadContext, stackTop);
            Threads.Add(spawned);
            handle = spawned.Handle;
            return 0;
        };
        game.Callbacks.StartThread = handle => {
            $"Starting thread 0x{handle:X}".Log();
            var spawned = Kernel.Get<SpawnedThread>(handle);
            new Thread(() => {
                "New thread started!".Log();
                _CurrentThread.Value = spawned;
                spawned.CpuState->X30 = 0xCAFEBABEDEADBEEF;
                try {
                    spawned.RunFrom(spawned.Entrypoint, 0xCAFEBABEDEADBEEF);
                } catch(Exception e) {
                    e.Log();
                    Environment.Exit(-1);
                }
            }).Start();
            return 0;
        };
        game.Callbacks.SleepThread = ns => Thread.Sleep((int) (ns / 1_000_000));
    }
}