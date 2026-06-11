namespace UmbraCore.Core;

// condvar-fix : Horizon condvar+mutex protocol, mirroring
// Atmosphere's kern_k_condition_variable.cpp + kern_k_address_arbiter mutex
// path. Replaces SyncManager's WaitProcessWideKeyAtomic / SignalProcessWideKey
// / ArbitrateLock / ArbitrateUnlock callbacks.
// Sera's original HLE (SyncManager.cs) treats the cv_key as a per-address
// counted Semaphore (Increment writing *cv_key) + a separate System.
// Threading.Mutex per mutex-addr. That's wrong-shape vs the real protocol:
// - *cv_key is a 0/1 FLAG (HasWaiterFlag), written by KERNEL only. Userland
// nn::os may read it to skip calling SVC Signal when 0 (= no waiters).
// Her Increment writing a count there conflicts with userland's reads.
// - One process-wide tree keyed by cv_key, not per-addr objects.
// - "Atomic" = mutex-release + *cv_key=1 + cv-tree-insert all under ONE
// scheduler lock. Her HLE does these as three separate non-atomic steps
// → signaler can run between unlock and insert → finds no waiter → the
// wait that follows blocks forever. = her "3-gen occasional hang."
// - Signal writes the woken thread's handle into the MUTEX word (atomic
// CAS 0→handle); if mutex held, the woken thread joins that mutex's
// waiter list instead of waking immediately. = mutex-handoff is part
// of the cv protocol, not a separate post-wake ArbitrateLock.
// This impl: one global _schedLock; per-cv_key + per-mutex-addr waiter
// lists; per-waiter ManualResetEventSlim. ArbitrateLock/Unlock also routed
// here so the mutex-handoff in Signal interoperates (can't mix with
// System.Threading.Mutex — it's thread-affine and doesn't know about *addr).
// ‡ v0: FIFO not priority-ordered (Atmosphere uses a priority tree). Timeout
// honored. Doesn't handle thread termination/cancel (CancelWait path).
// Spurious wakeups possible on timeout race (game must re-check predicate
// anyway). HandleWaitMask = bit 30 per svc::HandleWaitMask.

public static unsafe class CondVarKernel {
    const uint WaitMask = 0x40000000;

    static readonly object _schedLock = new();
    static readonly Dictionary<ulong, LinkedList<Waiter>> _cv = new();
    static readonly Dictionary<ulong, LinkedList<Waiter>> _mtx = new();

    class Waiter {
        public uint Handle;          // the waiting thread's handle (= tag value)
        public ulong MutexAddr;      // saved addr; Signal writes Handle here
        public readonly ManualResetEventSlim Ev = new(false);
        public uint Result;          // 0 = success; nonzero = svc result code
    }

    static LinkedList<Waiter> Cv(ulong k) =>
        _cv.TryGetValue(k, out var l) ? l : _cv[k] = new();
    static LinkedList<Waiter> Mtx(ulong a) =>
        _mtx.TryGetValue(a, out var l) ? l : _mtx[a] = new();

    // ─── mutex (svc::ArbitrateLock / ArbitrateUnlock) ───
    // Userland fast path: CAS *addr 0→my_handle (lock) / my_handle→0 (unlock,
    // if no WaitMask). SVC is the contended/slow path. So we only see calls
    // where *addr was nonzero (lock) or had WaitMask set (unlock).

    public static ulong ArbitrateLock(ulong ownerHandle, ulong addr, ulong reqHandle) {
        var w = new Waiter { Handle = (uint) reqHandle, MutexAddr = addr };
        lock(_schedLock) {
            var tag = *(uint*) addr;
            // Re-check: userland saw an owner, but it may have unlocked since.
            if((tag & ~WaitMask) == 0) {
                *(uint*) addr = (uint) reqHandle;
                return 0;
            }
            // ‡ Atmosphere also re-checks tag == ownerHandle|WaitMask exactly;
            // if not, returns immediately (userland retries). v0 = enqueue.
            *(uint*) addr = tag | WaitMask;
            Mtx(addr).AddLast(w);
        }
        w.Ev.Wait();
        return w.Result;
    }

    public static ulong ArbitrateUnlock(ulong addr) {
        lock(_schedLock) ReleaseMutexLocked(addr);
        return 0;
    }

    // Caller holds _schedLock. Hands the mutex at addr to the next waiter
    // (writes their handle to *addr, sets WaitMask if more remain, wakes
    // them) or writes 0 if no waiters.
    static void ReleaseMutexLocked(ulong addr) {
        if(_mtx.TryGetValue(addr, out var list) && list.First is { } node) {
            list.RemoveFirst();
            var next = node.Value;
            *(uint*) addr = next.Handle | (list.Count > 0 ? WaitMask : 0);
            next.Ev.Set();
        } else {
            *(uint*) addr = 0;
        }
    }

    // ─── condvar (svc::WaitProcessWideKeyAtomic / SignalProcessWideKey) ───

    public static ulong Wait(ulong addr, ulong cvKey, ulong handle, ulong timeoutNs) {
        var w = new Waiter { Handle = (uint) handle, MutexAddr = addr };
        lock(_schedLock) {
            // Atomically: release mutex + mark cv has-waiters + insert into cv tree.
            // This IS the "Atomic" in the SVC name — the signaler can't run
            // between these because it takes _schedLock too.
            ReleaseMutexLocked(addr);
            *(uint*) cvKey = 1;             // HasWaiterFlag
            Cv(cvKey).AddLast(w);
        }

        // Block. timeout: -1 (= u64 max) → infinite; 0 → immediate timeout
        // per Atmosphere's `R_UNLESS(timeout != 0, ResultTimedOut)`.
        bool woke;
        if(timeoutNs == ulong.MaxValue || (long) timeoutNs < 0)
            { w.Ev.Wait(); woke = true; }
        else if(timeoutNs == 0)
            woke = w.Ev.IsSet;
        else
            // ‡ ns→ms loses precision; fine for HLE.
            woke = w.Ev.Wait(TimeSpan.FromMilliseconds(Math.Max(1, timeoutNs / 1_000_000.0)));

        if(!woke) {
            // Timeout: remove from whichever list we're on, then re-acquire
            // the mutex synchronously (game expects to hold it on return,
            // even on timeout — Atmosphere's TimedWait does cs->Enter).
            lock(_schedLock) {
                // May have been signalled between Wait-timeout and lock; if
                // Ev is now set, we own the mutex (or are on its list).
                if(w.Ev.IsSet) goto owned;
                Cv(cvKey).Remove(w);
                if(Cv(cvKey).Count == 0) *(uint*) cvKey = 0;
            }
            // Re-acquire mutex (blocking). Same as ArbitrateLock semantics.
            ArbitrateLock(0, addr, handle);
            return 0xEA01;  // svc::ResultTimedOut (module=1, desc=117)
        }
        owned:
        // Woke: Signal already either (a) gave us the mutex (CAS'd our
        // handle into *addr, set Ev) or (b) put us on the mutex waiter list
        // and we'll wake when ReleaseMutexLocked hands it to us. Either way,
        // by the time Ev fires, *addr has our handle (± WaitMask).
        return w.Result;
    }

    public static ulong Signal(ulong cvKey, ulong count) {
        var c = (int) count;  // s32; ≤0 = broadcast
        lock(_schedLock) {
            if(!_cv.TryGetValue(cvKey, out var list)) {
                *(uint*) cvKey = 0;
                return 0;
            }
            int n = 0;
            while(list.First is { } node && (c <= 0 || n < c)) {
                list.RemoveFirst();
                var w = node.Value;
                n++;
                // Try to hand the mutex at w.MutexAddr to w. Atmosphere does
                // UpdateLockAtomic (CAS 0→handle, else OR-in WaitMask).
                var prev = *(uint*) w.MutexAddr;
                if((prev & ~WaitMask) == 0) {
                    // Mutex free → w owns it now.
                    *(uint*) w.MutexAddr = w.Handle | (prev & WaitMask)
                        | (Mtx(w.MutexAddr).Count > 0 ? WaitMask : 0);
                    w.Ev.Set();
                } else {
                    // Mutex held → w joins that mutex's waiter list. It'll
                    // wake when the holder ArbitrateUnlock's (or cv-Wait's,
                    // which calls ReleaseMutexLocked).
                    *(uint*) w.MutexAddr = prev | WaitMask;
                    Mtx(w.MutexAddr).AddLast(w);
                }
            }
            if(list.Count == 0) *(uint*) cvKey = 0;
        }
        return 0;
    }
}
