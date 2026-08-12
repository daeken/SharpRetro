# Sync audit — atomics / memory-model / consume-semantics lane (coram)

Scope split per ·1425: horizon = blocking primitives (his SYNC-AUDIT.md, H1-H5);
me = the layers under them — guest-visible atomics, JIT memory model, the
WFMO/event consume mechanics horizon's H1 didn't finish, shim-side atomic edges.
Sources: alky-shims/src/lib.rs @2026-08-12 00:37 snapshot (line numbers from it);
xfusion-recomp/sharpretro-jit @c89fb81.

## 🔴 C1 — THE JIT DROPS EVERY `lock` PREFIX (all Interlocked* are non-atomic)

**Where:** decode.rs:24 parses `p.lock` (0xF0); `grep -c "p.lock" lift.rs` = 0 —
no lift consumer. tier-0/tier-1 lower every mem-RMW as plain ldr → op → str;
`grep ldxr|stxr|ldax|stlx|dmb|casal` over the whole JIT = **zero atomic or fence
encodings exist**.

**What breaks:** guest `lock xadd/lock cmpxchg/lock inc/xchg-mem` (= every
Interlocked*, every `std::atomic`, CRT refcounts, MSVC magic-static guards, and
the task-system counters horizon decoded — push = lock-xadd @0x140124c74, the
join count [ctx+0x18]) executes as a non-atomic read-modify-write **racing the
other 8+ guest threads**. Windows-x86 `xchg mem,reg` is implicitly locked even
without 0xF0 — ours isn't. Symptom class: lost increments/decrements → refcount
over/under-release, task counts that never reach 0 (⚠ shaped exactly like the
window-handshake join-wall, IF the join count is ever RMW'd concurrently),
double-init through guard words. These bugs are load-dependent and unreproducible
by lockstep (single-threaded oracle can't see them).

**Fix shape (aarch64):** for `d.p.lock || XCHG-mem`: LDAXR/op/STLXR retry loop
(or LSE `LDADDAL/CASAL/SWPAL` — we're on Graviton-class hosts, LSE present).
Both tiers can route through one `Builder::mem_rmw_atomic(op, addr, val)` node;
interp arm = a host `AtomicU64` op on the identity-mapped address. CMPXCHG lift
(lift.rs:288 family) additionally needs its compare+write fused into the CAS
itself, not just made atomic per-access.

**Interim tell:** ALKY_ATOMLOG counting lock-prefixed decodes per block would
show how hot the surface is before we pay the emit work.

## 🔴 C2 — memory-ORDER: even made atomic, plain ldr/str gives no acquire/release

x86-TSO gives every load acquire + every store release semantics; `lock` ops are
full fences. ARM64 is weakly ordered. The JIT emits plain ldr/str for ALL guest
memory traffic → publish patterns (`data.write(); flag.store(1)` /
`while flag==0; read data`) can reorder on the HOST even after C1 is fixed for
the RMWs themselves. Rosetta 2 solves this with hardware TSO mode; we have no
TSO bit on Graviton.

**Pragmatic tiering (don't fence every access — that's the 2-5× cliff):**
(a) `lock`-RMWs → LDAXR/STLXR/LSE-`AL` variants = full-barrier semantics ✓ (C1's
fix already buys it); (b) MFENCE/SFENCE/LFENCE lifts (currently semantic no-ops
per sweep — fine single-threaded) → `dmb ish`; (c) plain-access TSO = accept the
gap for now (games mostly synchronize through locks + Interlocked, which (a)+(b)
cover) but **write it down as a standing ‡** — if a heisen-bug smells like
publish-order, this is the suspect list's head. XCHG-mem (implicitly locked,
lift.rs:2223 family) belongs in bucket (a).

## 🔴 C3 — WFMO wait-ANY with timeout>0 never blocks: it polls try-consume then
returns 0x102 without waiting (lib.rs:2354-2370)

The any-arm loops `try_one(h, 0)` over the handles once (consuming semantics,
correct so far), but if none is signaled it falls to `ret = 0x102` — **there is
no blocking path for wait-any with a finite timeout**; only timeout==0 (poll) is
correctly non-blocking. A `WFMO(any, 16ms)` frame-pacing wait becomes a hot
spin at the caller (they loop on 0x102) or a missed signal (they treat timeout
as give-up). fuchi's runtime cols showed WFMO callers in the boot path.
**Fix:** any-arm with tmo>0 needs a real block — condvar over "any of these
handles signaled" (register this waiter on each handle's CV, first signal wins),
or a coarse slice-poll loop (`try_all → sleep 1ms → deadline check`) as the
cheap v1 (correct, just latency-lumpy).

## 🟠 C4 — WFMO wait-ANY consume order is fixed (index 0 first) — starvation +
priority inversion vs Windows' balanced pick (same lines as C3)

Windows WFMO-any picks the LOWEST-index signaled handle too — BUT it checks all
handles atomically at wake, not by sequential consume-poll. Our sequential
try_one consumes handle[0] even when the caller's logic is starving on
handle[3]. Minor vs C3; fold into its fix (evaluate signal set, then consume
exactly the returned index).

## 🟠 C5 — ReleaseSemaphore ignores the count CAP + lies in lpPreviousCount
(lib.rs:2287-2291, sem_release :383)

`sem_release` does `*c += n` with no maximum (real semaphores fail with
ERROR_TOO_MANY_POSTS beyond lMaximumCount — CreateSemaphore's rdx arg, which
make_semaphore drops entirely) and lpPreviousCount is hardwired 0 rather than
the pre-release count. A producer/consumer ring sized by the semaphore max can
overfill; code reading previous-count for "was it empty?" decisions
mis-branches. Cheap fix: store max in SemObj; write the real prev.

## 🟠 C6 — InitOnce guest-word protocol is non-atomic AND wrong-shaped
(lib.rs:2577-2599 + the JIT-side intercept)

horizon's H5 covers the double-run race. The additional atomics-lane finding:
the guest INIT_ONCE word is read/written with plain `read()`/`write(0x2)` — on
real Windows that word is driven by interlocked CAS and *concurrently probed by
the guest's own inlined fast-path* (`if (once & 3) == 2 return;` is compiled
into the caller sometimes). Our non-atomic write can race a guest-side CAS
(the guest CAS itself is broken by C1 today — two layers of the same hole
stacked). When C1's CAS lands, this shim write must become a host atomic on the
same address, and the value protocol should be: CAS 0→(1|self-marker) to claim
pending, write 2 on complete — matching the guest fast-path's expectations.

## 🟡 C7 — event_wait/mutex_wait wake-loss window on notify_one +
consume-by-another (lib.rs:153-240 region)

`notify_one` + a third thread's try-consume between wake and re-check makes the
woken waiter re-sleep for the FULL remaining slice — with H3's 30s cap this
converts to a spurious 0x102; without the cap it's just latency. Standard
condvar-consumer pattern is fine IF all consumption paths go through the same
mutex+CV (they do here) — flagging as yellow because the auto-reset event
consume in try_one(timeout=0) (WFMO poll path) does NOT wake other CVs on
failure-rollback... which is C3's fix's job. Re-audit after C3.

## 🟡 C8 — cur_tid()-keyed recursion vs thread-handle DUPLICATION
GetCurrentThread() returns the pseudo-handle (-2) which several shims special-
case; DuplicateHandle of the pseudo-handle should produce a REAL handle bound
to the calling thread — check the Duplicate arm preserves that (I could only
see the write-target zeroing at :2287-region; flag for fuchi to confirm).

## Cross-refs with horizon's report
- His H4 (addr_lock proceed-as-owner) is the king finding for the live hunt; I
  independently converged on it (EnterCriticalSection :826-838 + both SleepCV
  re-acquire arms :2542/:2565 ignore the false return).
- His H1/H2/H3 (WFMO all-consume, INFINITE→0x102, the 30s cap) — confirmed,
  same lines; my C3/C4 extend the WFMO-ANY side he didn't reach.

## Priority for the live window-handshake hunt
1. **C1** if the join count [ctx+0x18] or the ring cursors are updated by
   lock-RMW from MULTIPLE threads (horizon's push-site walk answers this) —
   a lost xadd IS a count that never reaches 0.
2. horizon-H4 (proceed-as-owner) — scheduler-state corruption.
3. C3 if the factory's wait path crosses WFMO-any (his Peek-loop decode says
   the join loop uses its own count-poll, so lower).
