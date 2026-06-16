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

    // per-thread last-reentry snapshot, readable from
    // watchpoint thread. mtid → (game-fp X29, game-LR X30,
    // op<<16|a, monotonic reentry-count N, ts). N-delta
    // between two wp events = how many reentries in that
    // window (= the "1.2s pure-native" verifier).
    public static readonly System.Collections.Concurrent
        .ConcurrentDictionary<int, (ulong Fp, ulong Lr, ulong OpA, ulong N, long Ts)>
        ThreadSnap = new();

    // watchpoint: poll *M1/*M2 at ~50μs; log every value-
    // CHANGE with timestamp + ALL threads' last-SVC snapshot.
    // Answers: "WHO wrote h1f's handle into M1.tag, WHEN, what
    // was each thread doing." Started by NvnVulkan watchdog
    // when UMBRA_WATCHPOINT_M1 set. M1/M2 addrs are run-stable
    // (.bss/heap deterministic with setarch -R).
    static volatile bool _wpRunning;
    public static void StartWatchpoint() {
        if(_wpRunning || Environment.GetEnvironmentVariable("UMBRA_WATCHPOINT_M1") == null)
            return;
        _wpRunning = true;
        new Thread(() => {
            // M1 = NuMemoryPool::m_globalCriticalSectionBuff +0x?
            // M2 = heap LCM cs. Both deterministic addrs (-R).
            var m1 = (uint*)0xfff5b44815b8UL;
            var m2 = (uint*)0xfff563b23058UL;
            uint p1 = 0, p2 = 0;
            int n = 0;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            // Wait until the game's done init (= M1/M2 addrs are
            // mapped). M1 is in .bss (mapped at module-load), M2 is
            // heap-alloc'd later. Trigger = first ArbitrateLock SVC
            // (= 0x1A) on M1 → both addrs are valid.
            while(ThreadSnap.IsEmpty) Thread.Sleep(50);
            // …then probe-deref via mincore-style: try-catch the
            // first read; if SEGV, we're too early. Actually: M2
            // is heap; spin until first cv-Wait fires (= _cv non-
            // empty) which guarantees both addrs alive.
            while(_mtx.Count == 0) Thread.Sleep(10);
            Thread.Sleep(100);  // let heap-allocs settle
            $"[wp] watchpoint armed; m1={(ulong)m1:x} m2={(ulong)m2:x}".Log();
            // (c⁸) MutexType base for M1 = futex−0x18.
            // nest@+8 (u32), owner@+0x10 (ThreadType*).
            var m1n = (uint*)((ulong)m1 - 0x10);   // nest
            var m1o = (ulong*)((ulong)m1 - 0x8);   // owner
            uint pn = 0; ulong po = 0;
            while(_wpRunning) {
                var v1 = *m1; var v2 = *m2;
                var vn = *m1n; var vo = *m1o;
                if(v1 != p1 || v2 != p2
                        || vn != pn || vo != po) {
                    //  log only t1+t6 (= nnMain+h1f) snaps
                    // — game-fp + LR (symbolicatable) + reentry-N
                    // (delta = "how many reentries since last wp").
                    // sw.ElapsedTicks → microseconds for readability.
                    var us = sw.ElapsedTicks * 1_000_000 / System.Diagnostics.Stopwatch.Frequency;
                    var s1 = ThreadSnap.GetValueOrDefault(1);
                    var s6 = ThreadSnap.GetValueOrDefault(6);
                    $"[wp] #{n++} {us}μs M1={v1:x}{(v1!=p1?"*":"")} n={vn}{(vn!=pn?"*":"")} o={vo:x}{(vo!=po?"*":"")} M2={v2:x}{(v2!=p2?"*":"")} | t1:N={s1.N},op={s1.OpA:x},lr={s1.Lr:x},fp={s1.Fp:x} t6:N={s6.N},op={s6.OpA:x},lr={s6.Lr:x},fp={s6.Fp:x}".Log();
                    p1 = v1; p2 = v2;
                    pn = vn; po = vo;
                }
                // ~50μs poll. SpinWait gives ~ns-resolution but
                // burns a core; Sleep(0) yields. Mix:
                Thread.SpinWait(100);
            }
        }) { IsBackground = true, Name = "wp" }.Start();
    }
    public static readonly ThreadLocal<int> _tlsLogged = new();

    // ── Trace ring (UMBRA_WATCHDOG): last N lock/unlock/wait/
    // signal calls. Dumped on stall. = the SEQUENCE that the
    // state-snapshot doesn't give.
    static readonly string[] _trace = new string[256];
    static int _traceIdx;
    static readonly bool _tracing = Environment.GetEnvironmentVariable("UMBRA_WATCHDOG") != null;
    static readonly long _trcT0 = System.Diagnostics.Stopwatch.GetTimestamp();
    static void Trc(string s) {
        if(!_tracing) return;
        //  timestamp each event so we can place trace-
        // ring entries relative to wp events (= "was this M3
        // unlock during the 1.2s window?").
        var us = (System.Diagnostics.Stopwatch.GetTimestamp() - _trcT0)
            * 1_000_000 / System.Diagnostics.Stopwatch.Frequency;
        _trace[Interlocked.Increment(ref _traceIdx) & 255] =
            $"[t{Environment.CurrentManagedThreadId}] {us}μs {s}";
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
                        // ‡(σ2) u500: w.Fp=0 (= waiter enqueued
                        // before LastFp.Value set, OR thread
                        // never hit NativeReentry — the TLS=15
                        // mode where one thread doesn't spawn).
                        // segvtrap.c intercepts SIGSEGV before
                        // CoreCLR converts to NRE → process
                        // dies despite the try-catch. Guard.
                        if(w.Fp == 0) {
                            $"[cvk]       (fp=0, skip walk)".Log();
                            continue;
                        }
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

    // (c⁹) Diagnostic: dump FP-chain when ArbitrateLock fires
    // for a specific futex addr (= the contended path; the
    // requester's SVC-entry FP walks back through ICS::Enter
    // → LockMutex → [Begin →] CALLER). Dedupe by (reqHandle,
    // frame-3-LR) so each distinct caller logs once. Set
    // UMBRA_M1_TRACE=<hex addr> to enable (e.g. fff5b44815b8
    // for LEGO Worlds' NuMemoryManager allocator futex; addr
    // is run-stable under setarch -R / ASLR-off).
    static readonly ulong _m1TraceAddr = ulong.TryParse(
        Environment.GetEnvironmentVariable("UMBRA_M1_TRACE"),
        System.Globalization.NumberStyles.HexNumber, null,
        out var a) ? a : 0;
    static readonly System.Collections.Generic.HashSet<ulong>
        _m1Seen = new();
    static void DumpM1Caller(ulong addr, ulong reqHandle, ulong fp) {
        if(_m1TraceAddr == 0 || addr != _m1TraceAddr
                || fp == 0) return;
        // Walk FP-chain: each frame = [fp→prev_fp, fp+8→saved_lr]
        var lrs = new ulong[8]; int n = 0;
        var p = fp;
        try {
            while(n < 8 && p > 0x1000 && p < 0x0001_0000_0000_0000UL) {
                var lr = *(ulong*)(p + 8);
                lrs[n++] = lr;
                var nx = *(ulong*)p;
                if(nx <= p || nx > p + 0x100000) break;
                p = nx;
            }
        } catch { }
        // Dedupe key: req | frame[3] (= past Enter/LockMutex/Begin)
        var key = (reqHandle << 48) | (n > 3 ? lrs[3] : lrs[n-1]);
        lock(_m1Seen) {
            if(!_m1Seen.Add(key)) return;
        }
        // nest@(addr−0x10) — assumes nn::os::MutexType layout
        // (futex word at +0x18; nest at +0x8). Reads as 0 if
        // the addr isn't actually a MutexType+0x18 futex.
        var nest = *(uint*)(addr - 0x10);
        var s = $"[c9] L(M1) req={reqHandle:x} nest={nest} fp0={fp:x} chain:";
        for(var i = 0; i < n; i++)
            s += $" {lrs[i]:x}";
        s.Log();
        // Symbolize via Kernel.StackTrace too (= readable)
        $"[c9]   ↳ symbolized:".Log();
        try { Kernel.StackTrace((ulong*)fp); } catch { }
    }

    public static ulong ArbitrateLock(ulong ownerHandle, ulong addr, ulong reqHandle) {
        var w = new Waiter {
            Handle = (uint) reqHandle, MutexAddr = addr,
            Fp = LastFp.Value,  // = NativeState->X29 at SVC entry
        };
        DumpM1Caller(addr, reqHandle, w.Fp);
        Trc($"L  m={addr:x} own={ownerHandle:x} req={reqHandle:x} *={*(uint*)addr:x}");
        lock(_schedLock) {
            var tag = *(uint*) addr;
            // (c²¹) Atmosphere KConditionVariable::WaitForAddress has
            // NO "tag==0 → grab it" arm. Our prior `(tag&~WM)==0 →
            // *addr=reqHandle` was a plain-store racing userland's
            // Enter LDAXR/STLXR CAS (which doesn't take _schedLock):
            // if thread C does CAS(0→C) between our read-tag=0 and
            // write-reqHandle, BOTH C and reqHandle-thread believe
            // they own the mutex. The non-atomic nest++/-- then races
            // → nest=1-stuck (Δ+=1) OR nest=-1 underflow OR freelist
            // corruption (u525 BinUnlink SIGSEGV). The (c⁹) fix below
            // removed the `*addr=tag|WM` non-atomic-RMW; same bug-
            // class lived 4 lines higher. Fix: remove the arm; the
            // `tag != owner|WM` check below catches tag==0 too →
            // return 0 → Enter re-loops @+0x30 → CAS's race-free.
            //
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
    // ⚠️ The *addr write here is OK to be non-atomic — the caller
    // (ArbitrateUnlock or Wait's release) is the OWNER releasing; no other
    // userland thread can fast-path-acquire while *addr still has owner's
    // handle (their CAS(0→…) fails). This write transitions owner→next
    // atomically wrt userland's CAS-loop (single 32-bit store IS atomic
    // on aarch64; the race was the load+test+write SEQUENCE in Signal).
    static void ReleaseMutexLocked(ulong addr) {
        if(_mtx.TryGetValue(addr, out var list) && list.First is { } node) {
            list.RemoveFirst();
            var next = node.Value;
            Volatile.Write(ref *(uint*) addr,
                next.Handle | (list.Count > 0 ? WaitMask : 0));
            next.Ev.Set();
        } else {
            Volatile.Write(ref *(uint*) addr, 0);
        }
    }

    // (q-bypass) Watchdog-only deadlock force-break. The (q)
    // CONTRADICTION (h1f M1.nest=1+owner=h1f BUT no M1-frame on
    // stack) says h1f doesn't actually hold M1 → safe to break.
    // If wrong (= h1f genuinely holds M1), this corrupts shared
    // state and the game crashes shortly after = a STRONGER
    // discriminator than the stack-walk. UMBRA_BREAK_DEADLOCK env.
    public static bool ForceBreak() {
        if(Environment.GetEnvironmentVariable("UMBRA_BREAK_DEADLOCK") == null)
            return false;
        lock(_schedLock) {
            var any = false;
            foreach(var (a, l) in _mtx.ToList())
                while(l.First is { } node) {
                    l.RemoveFirst();
                    var w = node.Value;
                    $"[cvk] FORCE-BREAK mtx@{a:x}: handing to h{w.Handle:x} (was *={*(uint*)a:x})".Log();
                    Volatile.Write(ref *(uint*) a,
                        w.Handle | (l.Count > 0 ? WaitMask : 0));
                    w.Ev.Set();
                    any = true;
                    break;  // hand to ONE waiter per mutex (= ReleaseMutexLocked semantics)
                }
            return any;
        }
    }

    // ─── condvar (svc::WaitProcessWideKeyAtomic / SignalProcessWideKey) ───

    public static ulong Wait(ulong addr, ulong cvKey, ulong handle, ulong timeoutNs) {
        var w = new Waiter { Handle = (uint) handle, MutexAddr = addr };
        Trc($"W  cv={cvKey:x} m={addr:x} h={handle:x} t={(long)timeoutNs}");
        lock(_schedLock) {
            // Atomically: mark cv has-waiters + release mutex + insert into cv tree.
            // This IS the "Atomic" in the SVC name. ⚠ The KERNEL signaler
            // (CondVarKernel.Signal) can't interleave because it takes
            // _schedLock too — but USERLAND nn::os Signal's fast-path
            // doesn't take _schedLock: it CAS-acquires the mutex, then
            // loads *cvKey, and skips the SVC if cvKey==0.
            //
            // (T6)×68: Atmosphere KCV::Wait (kern_k_condition_variable
            // .cpp:255-263) writes cvKey=HasWaiterFlag → DMB ish →
            // *addr=next_value (= mutex release), in THAT order. Our
            // prior order (ReleaseMutexLocked FIRST, cvKey=1 SECOND)
            // lets userland's fast-path interleave:
            //   us:   Volatile.Write(*addr, 0)        [release-store]
            //   them: CAS(addr, 0→theirHandle)        [acquire — sync's
            //         with our release; sees everything BEFORE it]
            //   them: load *cvKey → 0                 [our cvKey=1 is
            //         AFTER the release ⟹ NOT in the acquire's
            //         visibility guarantee]
            //   them: skip SVC SignalProcessWideKey
            //   us:   *cvKey = 1; AddLast(w)
            //   us:   w.Ev.Wait()                     [forever]
            // = lost-wakeup. The c²¹ fix (e536a21) was the ATOMICITY-
            // class race (non-atomic-RMW in ArbitrateLock); this is the
            // ORDERING-class sibling per kt[34](b), same file ~10L away.
            // Atmosphere's real-kernel KScopedSchedulerLock STOPS
            // userland (preempts all cores); our HLE _schedLock is a
            // managed Monitor that doesn't, so we need the store ORDER
            // correct where Atmosphere relies on preemption.
            //
            // Cv().AddLast can stay anywhere inside this lock block
            // (the SVC Signal takes _schedLock → blocks until we exit
            // → sees w in the list). The cvKey-vs-addr ORDER is what
            // matters for the userland fast-path's lockless cvKey-load.
            *(uint*) cvKey = 1;             // HasWaiterFlag — FIRST
            Thread.MemoryBarrier();         // = Atmosphere's DMB ish
            ReleaseMutexLocked(addr);       // *addr release — SECOND
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
                // Try to hand the mutex at w.MutexAddr to w. Atmosphere
                // KCV::Signal does UpdateLockAtomic = CAS(0→handle); on
                // CAS-fail (= someone else fast-path-acquired between
                // signaller's load and this CAS), retry-loop OR-in
                // WaitMask (also CAS) and add w to owner's waiter list.
                // v0 was non-atomic load+test+write; the then-
                // branch race = userland CAS(0→theirHandle) between
                // my load and write → my write OVERWRITES their handle
                // → both threads' nn::os layer thinks they own it →
                // EXACTLY the (q) CONTRADICTION signature (h1f's
                // M1.nest=1+owner=h1f with no M1-frame on stack).
                ref var tag = ref *(uint*) w.MutexAddr;
                var more = Mtx(w.MutexAddr).Count > 0 ? WaitMask : 0;
                // CAS-loop: try 0→w.Handle|more; on fail, OR-in WaitMask
                // (also CAS-loop) and enqueue. = Atmosphere's exact loop.
                var prev = Interlocked.CompareExchange(
                    ref tag, w.Handle | more, 0);
                if(prev == 0) {
                    w.Ev.Set();  // mutex was free; w owns it.
                } else if((prev & ~WaitMask) == 0) {
                    // prev=WaitMask only (= just-released-with-waiters,
                    // shouldn't happen since release writes next.Handle
                    // OR 0). Retry once with WaitMask|w.Handle.
                    if(Interlocked.CompareExchange(
                        ref tag, w.Handle | WaitMask, prev) == prev) {
                        w.Ev.Set();
                    } else {
                        Interlocked.Or(ref tag, WaitMask);
                        Mtx(w.MutexAddr).AddLast(w);
                    }
                } else {
                    // Held by someone. OR-WaitMask (atomic) + enqueue.
                    Interlocked.Or(ref tag, WaitMask);
                    Mtx(w.MutexAddr).AddLast(w);
                }
            }
            if(list.Count == 0) *(uint*) cvKey = 0;
        }
        return 0;
    }
}
