using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Aarch64Cpu;

namespace UmbraCore.Core;

public unsafe class KThread : KObject {
    public CpuState* CpuState;
    // svcCreateThread X5 / SetThreadCoreMask X1. nnMain (= the
    // base KThread, not a SpawnedThread) defaults to 0 (= NPDM
    // default for apps). SpawnedThreads get it set in CreateThread.
    public int IdealCore;
    public readonly IntPtr State, Stack;
    public IntPtr TlsBase;
    public ulong Tpidr;

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
        game.Callbacks.SetThreadCoreMask = (handle, coreId, mask) => {
            $"[thr] SetThreadCoreMask h=0x{handle:X} core={(int)coreId} mask=0x{mask:X}".Log();
            var t = Kernel.Get<KThread>((uint)handle) ?? CurrentThread;
            if(t != null && (int)coreId >= 0) t.IdealCore = (int)coreId;
            return 0;
        };
        // tested 7/8-stall same-sig REFUTED; reverted to →0.
        // The deadlock isn't per-core-resource collision.
        game.Callbacks.GetCurrentProcessorNumber = () => 0;
        game.Callbacks.CreateThread = (entrypoint, threadContext, stackTop, priority, coreId, ref handle) => {
            // coreId: per Switch SVC, s32; -2 = "use default core
            // from NPDM" (= core 0 for apps), 0-3 = pin to that
            // core. deadlock discriminator: real hw pins
            // threads → same-core threads can't be concurrently
            // in critical sections → no ABBA window. Umbra runs
            // every spawned thread on its own host Thread = true
            // concurrency = the window opens.
            var spawned = new SpawnedThread(entrypoint, threadContext, stackTop);
            spawned.IdealCore = (int) coreId;
            Threads.Add(spawned);
            handle = spawned.Handle;
            $"[thr] CreateThread h=0x{handle:X} ep=0x{entrypoint:X} prio={priority} core={(int)coreId} sp=0x{stackTop:X}".Log();
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