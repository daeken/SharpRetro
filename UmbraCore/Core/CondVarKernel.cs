namespace UmbraCore.Core;

// condvar-fix : Horizon condvar+mutex protocol, mirroring
// Atmosphere's kern_k_condition_variable.cpp + kern_k_address_arbiter mutex
// path. Replaces SyncManager's WaitProcessWideKeyAtomic / SignalProcessWideKey
// / ArbitrateLock / ArbitrateUnlock callbacks.
// cs) treats the cv_key as a per-address
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
        public ulong Fp;             // X29 at SVC entry (for watchdog game-stack)
    }

    // Set per-SVC-entry from NativeReentry (X29). Watchdog reads
    // blocked Waiters' .Fp → Kernel.StackTrace → which game fns
    // are on stack at deadlock-time. = instrument.
    public static readonly ThreadLocal<ulong> LastFp = new();
    public static readonly ThreadLocal<int> _tlsLogged = new();

    // ── Trace ring (UMBRA_WATCHDOG): last N lock/unlock/wait/
    // signal calls. Dumped on stall. = the SEQUENCE that the
    // state-snapshot doesn't give.
    static readonly string[] _trace = new string[256];
    static int _traceIdx;
    static readonly bool _tracing = Environment.GetEnvironmentVariable("UMBRA_WATCHDOG") != null;
    static void Trc(string s) {
        if(!_tracing) return;
        _trace[Interlocked.Increment(ref _traceIdx) & 255] =
            $"[t{Environment.CurrentManagedThreadId}] {s}";
    }

    // Watchdog dump: who's waiting on what. Called from outside
    // (Present-watchdog when main thread stalls) so takes
    // _schedLock. Output = which mutex addrs have waiters with
    // no path to wake (= the deadlock signature).
    public static void DumpState(string tag) {
        lock(_schedLock) {
            $"[cvk] {tag}: cv={_cv.Count} mtx={_mtx.Count}".Log();
            if(_tracing) {
                $"[cvk]   ── trace (last 256, oldest→newest) ──".Log();
                for(var i = 1; i <= 256; i++) {
                    var s = _trace[(_traceIdx + i) & 255];
                    if(s != null) $"[cvk]   {s}".Log();
                }
            }
            foreach(var (k, l) in _cv)
                if(l.Count > 0)
                    $"[cvk]   cv@{k:x}: {l.Count} waiters [{string.Join(",", l.Select(w => $"h{w.Handle:x}→m{w.MutexAddr:x}"))}] *cv={*(uint*)k:x}".Log();
            foreach(var (a, l) in _mtx)
                if(l.Count > 0) {
                    $"[cvk]   mtx@{a:x}: {l.Count} waiters [{string.Join(",", l.Select(w => $"h{w.Handle:x}"))}] *addr={*(uint*)a:x}".Log();
                    foreach(var w in l) {
                        $"[cvk]     ↳ h{w.Handle:x} game-stack (fp=0x{w.Fp:x}):".Log();
                        try { Kernel.StackTrace((ulong*)w.Fp); }
                        catch(Exception e) { $"[cvk]       (threw: {e.Message})".Log(); }
                        // raw stack scan: every 8B word in
                        // [fp..fp+0x800] that lands in a code region
                        // = candidate LR. Catches frames the FP-walk
                        // misses (= leaf fns that don't push fp, OR
                        // the M1-holding frame the CONTRADICTION says
                        // must exist).
                        try {
                            // w.Fp = (ulong)state (host NativeState*);
                            // *state = X29 = game-fp. Walk from there.
                            var gfp = *(ulong*)w.Fp;
                            $"[cvk]     ↳ h{w.Handle:x} RAW [0x{gfp:x}..+0x800]:".Log();
                            for(var o = 0; o < 0x800; o += 8) {
                                var v = *(ulong*)(gfp + (ulong)o);
                                if(v < 0xfff5_b000_0000 || v >= 0xfff5_c000_0000) continue;
                                var s = Kernel.Symbols.FirstOrDefault(x => x.Key.Start <= v && v < x.Key.End);
                                if(s.Value == null) continue;
                                $"[cvk]       [+0x{o:x}] 0x{v:x}  {s.Value}+0x{v - s.Key.Start:x}".Log();
                            }
                        } catch(Exception e) { $"[cvk]       (raw threw: {e.Message})".Log(); }
                    }
                    // + raw bytes around the mutex (what object
                    // is M2 inside? — vtable/type ptr/canary).
                    var b = (ulong*)(a & ~0xFul);
                    $"[cvk]     mem[{(ulong)b:x}..]: {b[-2]:x16} {b[-1]:x16} | {b[0]:x16} {b[1]:x16} {b[2]:x16} {b[3]:x16}".Log();
                }
        }
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
        var w = new Waiter {
            Handle = (uint) reqHandle, MutexAddr = addr,
            Fp = LastFp.Value,  // = NativeState->X29 at SVC entry
        };
        Trc($"L  m={addr:x} own={ownerHandle:x} req={reqHandle:x} *={*(uint*)addr:x}");
        lock(_schedLock) {
            var tag = *(uint*) addr;
            // Re-check: userland saw an owner, but it may have unlocked since.
            if((tag & ~WaitMask) == 0) {
                *(uint*) addr = (uint) reqHandle;
                return 0;
            }
            // Atmosphere KCV WaitForAddress (): kernel does NOT
            // write *addr — userland already OR'd WaitMask via its
            // own ldaxr/stlxr CAS at ICS::Enter+0x60. Kernel just
            // verifies tag == owner|WaitMask (else return — userland
            // re-loops). My prior `*addr = tag|WaitMask` was a non-
            // atomic RMW that races the owner's native `stlxr 0` in
            // ICS::Leave: if owner clears between my read-tag and
            // write-tag|WM, I write a STALE owner-tag back; owner's
            // already gone (nn::os owner=null nest=0); next time
            // ex-owner Enters, the +0x3c early-return fires (tag&~WM
            // == myHandle) and it "acquires" without owning.             // raw-stack confirmed h1f's M1.nest=1+owner=h1f with NO
            // M1-frame on stack = exactly this signature.
            if(tag != ((uint)ownerHandle | WaitMask)) {
                Trc($"L! m={addr:x} tag={tag:x}≠own|W → retry");
                return 0;  // userland re-loops fast-path
            }
            // DON'T write *addr. Userland set WaitMask. Just enqueue.
            Mtx(addr).AddLast(w);
        }
        w.Ev.Wait();
        return w.Result;
    }

    public static ulong ArbitrateUnlock(ulong addr) {
        Trc($"U  m={addr:x} *={*(uint*)addr:x}");
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
        Trc($"W  cv={cvKey:x} m={addr:x} h={handle:x} t={(long)timeoutNs}");
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
        Trc($"S  cv={cvKey:x} n={c}");
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
                    // Mutex held → w joins that mutex's waiter list.
                    // Atmosphere does CAS-OR for the WaitMask write
                    // (UpdateLockAtomic); a plain RMW races the owner's
                    // native stlxr-0 in ICS::Leave (= same race as
                    // ArbitrateLock above). Use Interlocked.Or = atomic.
                    Interlocked.Or(ref *(uint*) w.MutexAddr, WaitMask);
                    Mtx(w.MutexAddr).AddLast(w);
                }
            }
            if(list.Count == 0) *(uint*) cvKey = 0;
        }
        return 0;
    }
}
