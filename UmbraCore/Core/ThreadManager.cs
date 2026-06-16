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

    // per-core serialization. On real hw, threads on the
    // same core run mutually-exclusive (per-core scheduler;
    // strict-prio + 10ms-RR same-prio). Umbra's 1 host-Thread
    // per game-thread = ALL run concurrently. The 10 core-1
    // threads' cv-Wait/Signal worker-loop runs at host-speed
    // (= 10× cv-ops/sec vs hw) = more (q)-deadlock windows.
    // = each game-thread holds CoreLock[idealCore]
    // while in native; releases at SVC-entry, reacquires at
    // SVC-exit. ⟹ ≤1 thread per game-core in native at a time.
    // Approximates Atmos "reschedule on SVC-exit" (= the SVC
    // is where preemption fires). NOT priority-aware (v0); the
    // 10ms-RR via DPC = ‡not modeled (would need a side-thread
    // that flags preemption).
    // UMBRA_CORE_LOCK enables. core=-2 (= "any core"; SDK uses
    // for system threads) → core 3 (= reserved for system on hw).
    public static readonly bool CoreLockEnabled =
        Environment.GetEnvironmentVariable("UMBRA_CORE_LOCK") != null;
    public static readonly SemaphoreSlim[] CoreLock = CoreLockEnabled
        ? [new(1,1), new(1,1), new(1,1), new(1,1)] : null!;
    public static int CoreFor(int idealCore)
        => idealCore < 0 ? 3 : (idealCore & 3);
    public static void CoreEnter(KThread t) {
        if(CoreLockEnabled) CoreLock[CoreFor(t.IdealCore)].Wait();
    }
    public static void CoreExit(KThread t) {
        if(CoreLockEnabled) CoreLock[CoreFor(t.IdealCore)].Release();
    }

    public ThreadManager() {
        var main = new KThread();
        main.IdealCore = 0;  // nnMain = NPDM default core 0
        Threads.Add(_CurrentThread.Value = main);
        // nnMain enters native via Rtld.RunFrom (NOT
        // StartThread); acquire its core-lock here so the
        // SVC dispatch's CoreExit/CoreEnter are balanced.
        CoreEnter(main);
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
                CoreEnter(spawned);  // hold core-lock in native
                try {
                    spawned.RunFrom(spawned.Entrypoint, 0xCAFEBABEDEADBEEF);
                } catch(Exception e) {
                    e.Log();
                    Environment.Exit(-1);
                } finally {
                    CoreExit(spawned);
                }
            }).Start();
            return 0;
        };
        game.Callbacks.SleepThread = ns => Thread.Sleep((int) (ns / 1_000_000));
        // (T6)×69: Svc 0x1E GetSystemTick. Was throw-
        // NotImplemented (default); game uses MRS
        // CNTPCT_EL0 directly (op=3 fast-path) so this
        // never fired in legoworlds, but other games
        // may. Same source as the MRS path = consistent.
        game.Callbacks.GetSystemTick = () => UmbraTime.Ticks();
    }
}