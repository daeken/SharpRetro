using LibSharpRetro;

namespace UmbraCore.Core;

public abstract class Waitable : KObject {
    public readonly Queue<Func<bool, int>> Waiters = [];
    public bool Presignaled, Canceled;
		
    protected virtual bool Presignalable => true;

    public bool Wait() {
        var wait = new AutoResetEvent(false);
        var canceled = false;
        Wait(_canceled => {
            canceled = _canceled;
            wait.Set();
            return 1;
        });
        wait.WaitOne();
        return canceled;
    }
    public void Wait(Func<int> cb) => Wait(_ => cb());
    public virtual void Wait(Func<bool, int> cb) {
        lock(this) {
            if(!Presignaled || (Presignaled && cb(Canceled) == 0))
                Waiters.Enqueue(cb);
            Presignaled = false;
            Canceled = false;
        }
    }

    public void Signal(bool one = false) {
        lock(this) {
            if(Waiters.Count == 0 && Presignalable)
                Presignaled = true;
            else {
                var realHit = false;
                while(Waiters.TryDequeue(out var waiter)) {
                    var res = waiter(Canceled);
                    if(res == 0)
                        Waiters.Enqueue(waiter);
                    if(res != -1) {
                        realHit = true;
                        if(one)
                            break;
                    }
                }
                if(!realHit && Presignalable)
                    Presignaled = true;
            }
        }
    }

    public void Cancel() {
        lock(this) {
            Canceled = true;
            Signal();
        }
    }
}
	
public unsafe class Semaphore : Waitable {
    public readonly uint* Addr;
    public uint Value {
        get {
            lock(this)
                return *Addr;
        }
    }

    public Semaphore(ulong addr) => Addr = (uint*) addr;

    // : was `protected override bool Presignalable => false;`
    // — removed. With Presignalable=false, Signal with Waiters.Count==0 is a
    // PURE NO-OP. WaitProcessWideKeyAtomic does `if(sema.Value>0) early-bailout`
    // → unlock mutex → `sema.Wait`; if Increment+Signal land in that window,
    // signal is LOST and Wait blocks forever. Timing-dependent (= sera's
    // "occasional hang that's existed for 3 switch emulator generations").
    // Spurious wakeups are legal for condvars (game re-checks predicate); lost
    // wakeups are the bug. Letting it presignal closes the race.
    // Note: this race was found via Lego Worlds chase but turned out
    // NOT to be the deterministic block there (that was a SIGSEGV downstream,
    // masked by ~4GB coredump writes looking like a hang). The race is real
    // at-source regardless; the at-data verify it fixes the macOS occasional
    // = sera's call.

    public void Increment() {
        lock(this)
            (*Addr)++;
    }
		
    public void Decrement() {
        lock(this)
            (*Addr)--;
    }
}

public class Event : Waitable {
    public readonly bool AlwaysTriggered;

    public bool Triggered {
        get;
        set {
            var doSignal = !field && (value || AlwaysTriggered);
            Presignaled = field = value || AlwaysTriggered;
            if(doSignal) Signal();
        }
    }

    public Event(bool? triggered = null, bool alwaysTriggered = false) {
        AlwaysTriggered = alwaysTriggered;
        if(AlwaysTriggered)
            triggered = true;
        if(triggered != null)
            Triggered = triggered.Value;
    }
    
    public override void Wait(Func<bool, int> cb) {
        lock(this) {
            if(!Triggered || (Triggered && cb(Canceled) == 0))
                Waiters.Enqueue(cb);
            Triggered = AlwaysTriggered;
            Canceled = false;
        }
    }
}

public class SyncManager {
    readonly Dictionary<ulong, Semaphore> Semaphores = [];
    readonly Dictionary<ulong, Mutex> Mutexes = [];
    
    static TimeSpan ConvertTimeout(ulong timeout) =>
        timeout != ulong.MaxValue
            ? new TimeSpan((long) (timeout / 100))
            : new TimeSpan(0, 0, 0, 0, -1);

    // : Semaphores/Mutexes dicts had no lock; concurrent
    // EnsureX from multiple threads corrupted them ("Operations that change
    // non-concurrent collections must have exclusive access" — observed at
    // Lego Worlds once timing shifted via instrumentation). Same
    // 3-generation-occasional family as the cv lost-wakeup.
    Semaphore EnsureSemaphore(ulong addr) {
        lock(Semaphores)
            return Semaphores.TryGetValue(addr, out var s) ? s
                : Semaphores[addr] = new Semaphore(addr);
    }
    Mutex EnsureMutex(ulong addr) {
        lock(Mutexes)
            return Mutexes.TryGetValue(addr, out var m) ? m
                : Mutexes[addr] = new Mutex();
    }
    
    public unsafe void Setup(GameWrapper game) {
        game.Callbacks.ClearEvent = handle => {
            Kernel.Get<Event>(handle).Triggered = false;
            return 0;
        };
        game.Callbacks.ResetSignal = handle => {
            Kernel.Get<Event>(handle).Triggered = false;
            return 0;
        };
        game.Callbacks.SignalProcessWideKey = (semaAddr, target) => {
            $"SignalProcessWideKey {semaAddr:X}".Log();
            var semaphore = EnsureSemaphore(semaAddr);
            semaphore.Increment();
            if(target == 1)
                semaphore.Signal(true);
            else if(target == 0xFFFFFFFF)
                semaphore.Signal();
            return 0;
        };
        game.Callbacks.WaitSynchronization = (handlesAddr, numHandles, timeout, ref _activated) => {
            var handles = new Buffer<uint>(handlesAddr, numHandles * 4);
            $"WaitSynchronization([{string.Join(", ", handles.ToArray().Select(x => $"0x{x:X}"))}])".Log();
            foreach(var handle in handles) {
                var waitable = Kernel.Get<Waitable>(handle);
                if(waitable is Event evt)
                    $"0x{handle:X} is event {evt} -- Triggered {evt.Triggered} always? {evt.AlwaysTriggered}".Log();
            }
            var waitHandle = new AutoResetEvent(false);
            var activated = uint.MaxValue;
            var completed = false;
            Enumerable.Range(0, (int) numHandles).ForEach(i =>
                Kernel.Get<Waitable>(handles[i]).Wait(() => {
                    $"ASDPFOJDSFP??? {handles[i]:X}".Log();
                    if(completed) return -1;
                    lock(waitHandle) {
                        Console.WriteLine($"Activated! {i}");
                        activated = (uint) i;
                        waitHandle.Set();
                        return 1;
                    }
                }));
            if(waitHandle.WaitOne(ConvertTimeout(timeout))) {
                completed = true;
                _activated = activated;
                return 0;
            }
            completed = true;
            return 0xea01;
        };
        game.Callbacks.CancelSynchronization = handle => {
            $"CancelSynchronization(0x{handle:X})".Log();
            return 0xe401; // Thread isn't waiting. Surely this won't blow up later
        };
        game.Callbacks.WaitProcessWideKeyAtomic = (mutexAddr, semaAddr, threadHandle, timeout) => {
            $"WaitProcessWideKeyAtomic(0x{mutexAddr:X}, 0x{semaAddr:X}, 0x{threadHandle:X}, {timeout})".Log();
            var mutex = EnsureMutex(mutexAddr);
            var sema = EnsureSemaphore(semaAddr);
            if(sema.Value > 0) {
                $"Early bailout 0x{sema.Value:X}".Log();
                sema.Decrement();
                return 0;
            }
            game.Callbacks.ArbitrateUnlock(mutexAddr);
            sema.Wait();
            game.Callbacks.ArbitrateLock(0, mutexAddr, threadHandle);
            "Waited".Log();
            sema.Decrement();
            return 0;
        };
        game.Callbacks.ArbitrateLock = (curThread, mutexAddr, reqThread) => {
            //$"LockMutex(0x{curThread:X}, 0x{mutexAddr:X}, 0x{reqThread:X})".Log;
            EnsureMutex(mutexAddr).WaitOne();
            //"Locked mutex".Log;
            *(uint*) mutexAddr = (*(uint*) mutexAddr & 0x40000000) | (uint) reqThread;
            return 0;
        };
        game.Callbacks.ArbitrateUnlock = mutexAddr => {
            (*(uint*) mutexAddr) &= 0x40000000;
            try {
                EnsureMutex(mutexAddr).ReleaseMutex();
            } catch(ApplicationException) {
            }
            return 0;
        };

        // condvar-fix : overwrite the four mutex+condvar
        // SVCs with CondVarKernel.cs (Atmosphere-shape protocol; see that
        // file's header for the full read). UMBRA_LEGACY_SYNC=1 keeps the
        // originals above for A/B. The originals are wrong-shape vs the
        // real Horizon protocol (per Atmosphere kern_k_condition_variable.cpp
        // Atmosphere-protocol read) but mostly-work on macOS
        // timing; this is sera's "3-gen occasional hang" likely root.
        // ClearEvent/ResetSignal/WaitSynchronization/CancelSynchronization
        // = KEvent/handle SVCs, separate, kept above unconditionally.
        if(Environment.GetEnvironmentVariable("UMBRA_LEGACY_SYNC") != "1") {
            game.Callbacks.ArbitrateLock = (cur, addr, req) =>
                CondVarKernel.ArbitrateLock(cur, addr, req);
            game.Callbacks.ArbitrateUnlock = addr =>
                CondVarKernel.ArbitrateUnlock(addr);
            game.Callbacks.WaitProcessWideKeyAtomic = (addr, cv, h, t) =>
                CondVarKernel.Wait(addr, cv, h, t);
            game.Callbacks.SignalProcessWideKey = (cv, count) =>
                CondVarKernel.Signal(cv, count);
            "[CondVarKernel] CondVarKernel installed (Atmosphere-shape protocol)".Log();
        }
    }
}