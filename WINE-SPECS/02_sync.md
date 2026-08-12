# 02_SYNC — synchronization primitives (Wine-derived spec vs alky shim)

Family: SRW locks, critical sections, events/mutexes/semaphores, waits, condition
variables, Sleep. 24 touched functions (from `touched_apis_cp2077.txt`; confirmed by
grep — `TryAcquireSRWLockExclusive`, `TryEnterCriticalSection`, `WaitForSingleObjectEx`,
`WaitForMultipleObjects`, `SleepEx`, `WakeConditionVariable` are NOT in the touched list
but are specced briefly where they share a shim arm with a touched sibling).

Sources: wine @ /local/home/seratb/wine (master): `dlls/kernelbase/sync.c` (=KB),
`dlls/ntdll/sync.c` (=NT), `dlls/kernel32/tests/sync.c` (=TEST, outranks impl).
Shim: /tmp/alky-shims-lib.rs (=SHIM).

Wait-return constants used throughout: `WAIT_OBJECT_0`=0, `WAIT_ABANDONED`=0x80,
`WAIT_IO_COMPLETION`=0xC0, `WAIT_TIMEOUT`=0x102, `WAIT_FAILED`=0xFFFFFFFF.

---

## 1. SPEC

### 1.1 SRW locks

**InitializeSRWLock(lock)** — NT:501. Writes `lock->Ptr = 0`. Void, no last-error, no
failure mode. A zero-filled SRWLOCK is equally valid (SRWLOCK_INIT = {0}).

**AcquireSRWLockExclusive(lock)** — NT:514. Blocks until exclusively owned. Void.
Wine encoding: 32-bit word = `{ short exclusive_waiters; ushort owners }`; bit 0 of
exclusive_waiters doubles as "owned exclusive" (NT:591 release asserts it). **Recursive
exclusive acquire on the same thread is UB on real Windows = deadlock/hang** (wine
comment at NT:643: even Try "recursive calls are not allowed"). The game must never
rely on recursion succeeding; it MAY rely on it hanging (watchdog paths) — ‡ unlikely.

**AcquireSRWLockShared(lock)** — NT:556. Blocks while exclusively owned or exclusive
waiters pending; multiple concurrent shared owners allowed (`owners` count). Void.
Shared recursion: works in practice when no exclusive waiter is queued, UB otherwise
(classic SRW writer-starvation corner; TEST:2703 `test_srwlock_quirk` pokes raw lock
values to pin down wine's bit-layout compatibility).

**ReleaseSRWLockExclusive(lock)** — NT:591. Clears owners + the exclusive bit; wakes
one exclusive waiter (`RtlWakeAddressSingle`) else all shared waiters. Releasing a
lock not owned exclusive: wine `ERR`s and corrupts state (NT:602); real Windows raises
`STATUS_RESOURCE_NOT_OWNED` (0xC0000264) ‡ (wine does not model the raise).

**ReleaseSRWLockShared(lock)** — NT:616. Decrements owners; wakes an exclusive waiter
when the last shared owner leaves. Same not-owned caveat (NT:627-628).

**TryAcquireSRWLockExclusive(lock)** *(pair, not touched)* — NT:643. Returns BOOLEAN;
FALSE when owned (including own-thread recursion — documented in wine's header comment
NT:645-647). TEST:2559 `test_srwlock_base` asserts try-exclusive fails while shared is
held and try-shared fails while exclusive is held.

### 1.2 Critical sections

Layout (all offsets observable by the game): `+0 DebugInfo`, `+8 LockCount (LONG)`,
`+12 RecursionCount (LONG)`, `+16 OwningThread (HANDLE, holds the TID)`,
`+24 LockSemaphore`, `+32 SpinCount`.

**InitializeCriticalSection(crit)** — KB has no wrapper needed; kernel32 forwards to
`RtlInitializeCriticalSection` NT:200 → AndSpinCount(crit,0) → Ex(crit,spin,0).
Void; can only fail by raising on bad memory.

**InitializeCriticalSectionAndSpinCount(crit, spin)** — KB:1008
`return !RtlInitializeCriticalSectionAndSpinCount(...)` → BOOL, always TRUE in
practice (Rtl returns STATUS_SUCCESS). Spin count stored mod 0x80000000 ‡ (NT:213
masks the high bit on non-SMP wine builds it just stores).

**InitializeCriticalSectionEx(crit, spin, flags)** — KB:1016; raises on failure,
returns TRUE. Field fills (NT:218-256): **Win8+ semantics: `DebugInfo =
(void*)(ULONG_PTR)-1`** (the `no_debug_info_marker`, NT:150/230) unless
`RTL_CRITICAL_SECTION_FLAG_FORCE_DEBUG_INFO`; `LockCount = -1` (unlocked),
`RecursionCount = 0`, `OwningThread = 0`, `LockSemaphore = 0`. TEST:2926 asserts
`cs.DebugInfo == (void*)-1 || broken(!!cs.DebugInfo) /* before Win8 */` — i.e. the
modern contract is the -1 marker, NULL is the *pre-Win8-broken* shape.

**EnterCriticalSection(crit)** — NT:351 (`RtlEnterCriticalSection`).
`InterlockedIncrement(&LockCount)`: if transitions -1→0, uncontended acquire. If
already owner: `RecursionCount++` (recursion fully legal). Else block on the
semaphore/futex. On acquire: `OwningThread = current TID` (the same value
GetCurrentThreadId returns), `RecursionCount = 1`. Observable invariant after first
Enter: LockCount ≥ 0 (TEST:2993 checks `cs.LockCount` truthy while contended),
OwningThread == GetCurrentThreadId().

**TryEnterCriticalSection(crit)** *(pair, not touched)* — NT:392. BOOL. CAS -1→0 or
owner-recursion; FALSE without blocking otherwise.

**LeaveCriticalSection(crit)** — NT:433. If `--RecursionCount > 0`: just
`InterlockedDecrement(&LockCount)`. Else `OwningThread = 0` **before** the final
decrement; wakes a waiter if LockCount ≥ 0 after decrement. Void. Leaving an unowned
CS = state corruption (wine WARNs, NT:438-443 ‡ exact line of the WARN); real Windows
similar UB.

**DeleteCriticalSection(crit)** — NT:271. Frees debug info (if real), closes the
semaphore/futex handle. Void. Deleting while owned by another thread = UB.

### 1.3 Events / mutexes / semaphores (kernel objects + names)

**CreateEventA(sa, manual, initial, nameA)** — KB:552: converts the name and calls
CreateEventW path (via CreateEventExW KB:599 with flags
`CREATE_EVENT_MANUAL_RESET(1) | CREATE_EVENT_INITIAL_SET(2)`).

**CreateEventW(sa, manual, initial, nameW)** — KB:566 → CreateEventExW.
Returns HANDLE or NULL. **Last-error contract (the TRUST-CHAIN one), KB:623-627 +
TEST:620/625/631:**
- fresh create (named or anonymous): success handle + `SetLastError(0)` — TEST:620
  literally asserts `GetLastError() == 0` after a fresh named create;
- name already exists (as an event): returns a handle *to the existing object* +
  `SetLastError(ERROR_ALREADY_EXISTS)` (183) — TEST:625; the manual/initial params
  are IGNORED for the existing object;
- name exists as a *different object type*: NULL + `ERROR_INVALID_HANDLE`.
`manual_reset` selects NotificationEvent vs SynchronizationEvent at the NT layer.

**SetEvent(h)** — KB:679: `set_ntstatus(NtSetEvent(h, NULL))`. TRUE on success; FALSE
+ `ERROR_INVALID_HANDLE` on a bad handle. Manual-reset: stays signaled, releases ALL
current+future waiters until ResetEvent. Auto-reset: releases exactly ONE waiter and
is atomically consumed by that release; if no waiter, stays signaled until one wait
consumes it (TEST:632-690 walks exactly this: auto-reset event set once → first
WFSO(0) = WAIT_OBJECT_0, second = WAIT_TIMEOUT).

**ResetEvent(h)** — KB:688: `NtResetEvent`. TRUE/FALSE+last-error as above. Idempotent.

**CreateMutexW(sa, owner, name)** — KB:711 → CreateMutexExW (KB:742) with
`CREATE_MUTEX_INITIAL_OWNER`. Same name-collision contract: fresh → last-error 0
(TEST:364), existing → handle + `ERROR_ALREADY_EXISTS` (KB:754, TEST:358) and **the
bInitialOwner request is ignored** (you do NOT own the existing mutex). Recursive
acquisition legal (per-thread recursion count); released by equal ReleaseMutex count.
ReleaseMutex by a non-owner: FALSE + `ERROR_NOT_OWNER` (288) — TEST:330.
**Abandonment**: owner thread exits without releasing → next waiter gets
`WAIT_ABANDONED` (0x80) from its wait AND owns the mutex (TEST:252).

**CreateSemaphoreW(sa, initial, max, name)** — KB:796 → Ex (KB:806). Count semantics:
wait decrements, ReleaseSemaphore(h, n, &prev) adds n and writes the pre-release count
to prev (KB:846, `NtReleaseSemaphore`). Release that would exceed max: FALSE +
`ERROR_TOO_MANY_POSTS` (298), count unchanged (TEST:690-735 exercises prev-count and
the over-max failure). Name collision: same ERROR_ALREADY_EXISTS contract
(KB:818, TEST:704, fresh→0 at TEST:699/710).

### 1.4 Waits

**WaitForSingleObject(h, ms)** — KB:395 = `WaitForSingleObjectEx(h, ms, FALSE)`.

**WaitForSingleObjectEx(h, ms, alertable)** *(Ex not touched; same contract)* —
KB:404: `NtWaitForSingleObject(normalize_std_handle(h), alertable, timeout)`.
Return values the game branches on:
- `WAIT_OBJECT_0` (0) — signaled (auto-reset events/semaphores/mutexes atomically
  consumed/acquired as part of the SAME operation);
- `WAIT_ABANDONED` (0x80) — mutex acquired via owner-death; caller owns it;
- `WAIT_TIMEOUT` (0x102) — ms elapsed, `ms=0` = non-blocking poll;
- `WAIT_IO_COMPLETION` (0xC0) — alertable=TRUE and an APC ran (SleepEx family);
- `WAIT_FAILED` (0xFFFFFFFF) + `SetLastError(RtlNtStatusToDosError)` on NT_ERROR
  (KB:412-415): NULL or garbage handle → `ERROR_INVALID_HANDLE` (TEST:1288-1340,
  `test_WaitForSingleObject`: WFSO(0,0) and WFSO(0xdeadbeef,0) both WAIT_FAILED +
  ERROR_INVALID_HANDLE).
`ms=INFINITE(0xFFFFFFFF)` never returns WAIT_TIMEOUT.

**WaitForMultipleObjects(count, handles, waitAll, ms)** — KB:424 → Ex (KB:434).
`count > MAXIMUM_WAIT_OBJECTS(64)` → WAIT_FAILED + `ERROR_INVALID_PARAMETER`
(KB:440-444). waitAll=FALSE: returns `WAIT_OBJECT_0 + i` for the LOWEST-indexed
signaled handle (index carries data!); `WAIT_ABANDONED_0 + i` (0x80+i) for an
abandoned mutex at index i. waitAll=TRUE: returns WAIT_OBJECT_0 when ALL are
simultaneously acquirable — acquisition is atomic (all-or-none; partial states are
not observable). Same handle twice + waitAll → `ERROR_INVALID_PARAMETER` ‡.

**Sleep(ms)** — KB:361: `NtDelayExecution(FALSE, timeout)`. Void. Sleep(0) yields to
any ready thread of equal priority. Full duration honored (no cap).

**SleepEx(ms, alertable)** *(pair, not touched)* — KB:372: returns 0 on elapse,
`WAIT_IO_COMPLETION` (192/0xC0) if an APC ran (STATUS_USER_APC).

### 1.5 Condition variables

**InitializeConditionVariable(cv)** — NT:711: `cv->Ptr = 0`. Void. Zero-init valid.

**SleepConditionVariableCS(cv, cs, ms)** — KB:1184 → RtlSleepConditionVariableCS
(NT:763): captures the CV generation value, `RtlLeaveCriticalSection(cs)`,
`RtlWaitOnAddress(&cv->Ptr, captured, timeout)`, `RtlEnterCriticalSection(cs)` —
i.e. the release+block is gap-free vs a concurrent Wake (a wake between release and
block still wakes this waiter), and the CS is ALWAYS re-acquired before return,
success or timeout. Returns BOOL; on timeout: FALSE + **`SetLastError(ERROR_TIMEOUT)`
= 1460** (KB set_ntstatus path; TEST:2129 + TEST:2138 assert
`GetLastError() == ERROR_TIMEOUT` — NOT WAIT_TIMEOUT 258). Spurious wakeups are
permitted by contract; callers must loop on the predicate.

**SleepConditionVariableSRW(cv, srw, ms, flags)** — KB:1196 → NT:794. Same shape;
releases/re-acquires the SRW lock in the mode given by
`CONDITION_VARIABLE_LOCKMODE_SHARED` (flags bit 0). Same ERROR_TIMEOUT contract.

**WakeConditionVariable(cv)** *(pair, not touched)* — NT:731: bumps generation, wakes
ONE waiter (`RtlWakeAddressSingle`).

**WakeAllConditionVariable(cv)** — NT:742: bumps generation, wakes ALL
(`RtlWakeAddressAll`). Waking with no waiters is a legal no-op. There is no
"wake sticks until next sleep" memory — a wake before the sleep is lost (that's what
the lock+predicate is for).

---

## 2. DIVERGENCE table (Wine/Windows contract vs SHIM)

SHIM architecture (context for the rows): real cross-thread objects since audit A1/A6 —
events = condvar-backed `EventObj` (SHIM:151-182), mutexes = owner+recursion `MutexObj`
(SHIM:355-396), semaphores = counted `SemObj` (SHIM:477-500), CS/SRW = per-guest-address
recursive `AddrLock` (SHIM:399-432), CVs = generation-counter `CvObj` with gap-free
unlock (SHIM:187-215). The wait chain event→sem→thread→mutex is real blocking
(SHIM:2524-2541). That is: the *happy paths are genuinely implemented*; divergences
below are edges and out-params.

| # | fn | real Windows (cite) | ours (cite) | severity |
|---|----|--------------------|-------------|----------|
| 1 | CreateEventA/W (+Mutex/Semaphore W) | lpName honored; existing name → handle to SAME object + last-error `ERROR_ALREADY_EXISTS`; fresh → last-error 0 (KB:623-627,754,818; TEST:620/625, 358/364, 699/704) | lpName ignored entirely — every call mints a fresh anonymous object; last-error untouched either way (SHIM:2502-2514) | **TRUST-CHAIN** — single-instance guards and cross-module handle-sharing branch on ALREADY_EXISTS; also two subsystems opening "the same" named event get two unrelated events → lost signals ‡ (CP2077 is single-process; intra-process named-object sharing is the plausible hazard) |
| 2 | WFSO invalid/NULL handle | WAIT_FAILED + ERROR_INVALID_HANDLE (KB:412-415; TEST:1288-1340) | untracked handle → returns 0 = WAIT_OBJECT_0 "signaled" (SHIM:2538-2541, deliberate: files-don't-block) | **TRUST-CHAIN** — `if (WaitForSingleObject(h,0) == WAIT_OBJECT_0)` on a stale/closed handle takes the acquired branch with nothing acquired. (For genuine file handles the constant is honest.) |
| 3 | WFSO on abandoned mutex | WAIT_ABANDONED (0x80), caller owns mutex (TEST:252) | abandonment unmodeled: thread exit never releases owned mutexes → waiter blocks forever; 0x80 unreachable (SHIM:338-396 has no thread-exit hook ‡ grep: no abandon path) | **BLOCKER-if-hit** — abandoned-mutex recovery paths can never run; the failure mode is a silent infinite wait instead of a recoverable 0x80. Rare in practice (needs a crashing/exiting owner thread). |
| 4 | SleepConditionVariableCS/SRW timeout | FALSE + last-error **ERROR_TIMEOUT (1460)** (KB:1184/1196; TEST:2129/2138) | FALSE + last-error **0x102 (258, WAIT_TIMEOUT)** (SHIM:2812, 2836 `set_last_error(0x102)`) | **TRUST-CHAIN** — code distinguishing timeout-vs-real-failure via `GetLastError()==ERROR_TIMEOUT` misclassifies every timeout. One-line fix: 0x102→1460. |
| 5 | Sleep/SleepEx duration | full duration honored (KB:361/372) | capped: `rcx.min(1000)` — Sleep(5000) sleeps 1s (SHIM:2761-2763) | WRONG-DATA — timing-visible; back-off/retry loops spin 5× hotter than intended. Benign for frame-pacing sleeps (all <1s). |
| 6 | ReleaseSemaphore lpPreviousCount | receives the pre-release count (KB:846) | `*prev = 0` always (SHIM:2519-2521) | WRONG-DATA — any consumer reading prev (queue-depth accounting) sees 0. Real count exists in SemObj; fix is returning it from sem_release. |
| 7 | ReleaseSemaphore over lMaximumCount | FALSE + ERROR_TOO_MANY_POSTS, count unchanged (KB:846→NtReleaseSemaphore; TEST:690-735) | max ignored at create (SHIM:2514 takes only rdx=initial) and release always TRUE, count unbounded (SHIM:483-485) | WRONG-DATA — semaphores used as bounded gates (max=1 binary semaphore pattern) can over-count → later waiters sail through. |
| 8 | ReleaseMutex by non-owner | FALSE + ERROR_NOT_OWNER (KB:782; TEST:330) | returns TRUE regardless; comment "foreign/unknown → benign TRUE" (SHIM:2518) | WRONG-DATA (pair fn, not in touched list) — error-checking callers see success. |
| 9 | SetEvent/ResetEvent invalid handle | FALSE + ERROR_INVALID_HANDLE (KB:679/688) | TRUE always; unknown handle silently no-ops (SHIM:2515-2516) | BENIGN — CP2077 doesn't error-check these ‡. |
| 10 | WFMO count > 64 | WAIT_FAILED + ERROR_INVALID_PARAMETER (KB:440-444) | silently truncates to first 64 (`rcx.min(64)`, SHIM:2545) | BENIGN — 64+ waits are absent from the touched call-sites ‡. |
| 11 | WFMO untracked handle in set | WAIT_FAILED + ERROR_INVALID_HANDLE | treated as signaled + one-line eprintln (SHIM:2549-2553) | WRONG-DATA — same class as row 2 but per-element; wait-any returns that index as the winner. |
| 12 | WFMO waitAll atomicity | all-or-none acquisition, partial never observable | try-consume then UNDO on partial failure: consumed objects re-signaled 2ms later — a third thread can steal one mid-undo, and a manual-reset event "re-set" is lossless but an auto-reset consumed+re-set can wake a different waiter spuriously (SHIM:2563-2580) | BENIGN ‡ — window is µs-scale; spurious wake on events is contract-legal; mutex/semaphore steal-during-undo is the real (unlikely) corner. |
| 13 | WFMO abandoned mutex at index i | WAIT_ABANDONED_0+i | unreachable (row 3) | folded into row 3 |
| 14 | EnterCriticalSection → OwningThread | == GetCurrentThreadId() of owner (NT:351 sets ClientId tid) | writes `cur_tid()` = raw Linux `gettid()` (SHIM:948, 366) while the shim's GetCurrentThreadId returns the MINTED tid from TEB+0x48 (SHIM:2259-2263) — two different id spaces | **TRUST-CHAIN** — a manual ownership assert `cs.OwningThread == GetCurrentThreadId()` is always false; deadlock-detector/telemetry code that walks CS owners misattributes. Fix: write the minted tid (read TEB+0x48 like GetCurrentThreadId does). |
| 15 | LeaveCriticalSection final release | OwningThread cleared to 0 before release (NT:433) | OwningThread left stale; only LockCount/RecursionCount decremented (SHIM:968-977) | BENIGN — "is it free" checks use LockCount; stale owner-tid only misleads debug reads. |
| 16 | InitializeCriticalSection* DebugInfo | Win8+: `(void*)-1` marker (NT:230; TEST:2926) | writes 0/NULL (SHIM:932) | BENIGN — NULL is the accepted pre-Win8 "broken" shape in the test; nothing branches on -1 ‡. |
| 17 | InitializeCriticalSectionEx flags | validated; bad flags → STATUS_INVALID_PARAMETER_3 raise (NT:220-224) | flags ignored, always succeeds (SHIM:929-940) | BENIGN |
| 18 | AcquireSRWLockShared concurrency | N readers concurrent (NT:556) | shared == exclusive: one address-lock, readers serialized (SHIM:2771-2775, comment "conservative-correct") | BENIGN (perf) — never admits a race; can only over-serialize. No lost-wakeup: release wakes next. |
| 19 | AcquireSRWLockExclusive recursion | UB = deadlock (NT:643 comment; lock never recursively acquirable) | `addr_lock(rcx,false)` returns false for own-thread recursion and the return is IGNORED → recursive acquire silently "succeeds" without depth++, and the FIRST Release fully frees the lock while the caller logically still holds it (SHIM:2771-2778, 403-418) | BENIGN-leaning ‡ — Windows behavior is a hang, so any game code doing this is already broken on Windows; shim turns the hang into potential early-release. Flag: if 02-family hangs chase ever land here, this is the spot. |
| 20 | TryAcquireSRWLockShared vs shared holder | succeeds (readers share; TEST:2559) | fails — shared modeled as exclusive (SHIM:2779-2787) | WRONG-DATA (pair fn, not touched) — try-then-fallback paths take the slow branch; correctness preserved. |
| 21 | WakeConditionVariable | wakes ONE (NT:731) | `cv_wake` = notify_all — both Wake and WakeAll wake everyone (SHIM:193, 2836) | BENIGN — spurious wakeups are contract-legal; thundering-herd perf only. |
| 22 | WFSOEx/WFMOEx alertable, SleepEx APC | WAIT_IO_COMPLETION (0xC0) possible (KB:372/404) | no APC machinery; 0xC0 unreachable (SHIM:2524, 2761) | BENIGN — nothing queues user APCs in the shim'd world. |
| 23 | Named create fresh-success last-error | SetLastError(0) (TEST:620/364/699) | last-error untouched on create (SHIM:2502-2514) | WRONG-DATA — folded with row 1 in fix; callers reading GetLastError() after a create see a stale prior error. |

**PASS rows** (constant or simple behavior that IS the honest contract):
- `InitializeSRWLock` / `InitializeConditionVariable`: write 0 (SHIM:2770/2788) — exactly Wine (NT:501/711). PASS.
- `DeleteCriticalSection` no-op (SHIM:978): shim allocates nothing per-CS that needs freeing; observable contract (void, fields untouched) holds. PASS.
- Event auto-vs-manual reset exactness: `event_wait` consumes the signal iff `!manual` (SHIM:180), `event_set` notify_all + stays set until consumed/reset (SHIM:162) — matches TEST:632-690 sequence (auto: one WFSO succeeds, next times out; manual: both succeed). PASS.
- `CreateEventExW` flag decode bit0=MANUAL_RESET bit1=INITIAL_SET (SHIM:2506-2508): matches winbase.h values. PASS.
- WFSO return-value core for tracked handles: 0 vs 0x102, INFINITE never times out (SHIM:2524-2541 + H3 full-timeout fix at SHIM:171-176): matches. PASS.
- WFMO wait-any lowest-index-ish return `WAIT_OBJECT_0+i` and wait-all → 0 (SHIM:2588-2599/2573): matches shape (scan order = index order, so lowest signaled index wins each round). PASS.
- `SleepConditionVariable{CS,SRW}` release-block-reacquire atomicity: gap-free via holding the CV's own mutex across the guest-lock release (SHIM:197-215, comment block) — the lost-wakeup class Wine solves with WaitOnAddress is solved equivalently. CS re-acquire restores LockCount/RecursionCount/OwningThread; SRW re-acquire honors LOCKMODE_SHARED. PASS (modulo row 4's error code).
- `Sleep(0)` → yield-ish (no sleep, ret 0): acceptable; `SwitchToThread` separately real. PASS.

Divergence count (non-PASS, deduped rows): **20** (rows 1-12, 14-23 minus folded 13; 3 TRUST-CHAIN + 1 BLOCKER-if-hit + 8 WRONG-DATA + 8 BENIGN).

---

## 3. RET-0 / constant-arm GRADING

None of the 10 census suspects live in this family. Constant arms found in the sync
dispatch:

| arm (SHIM line) | constant | grade |
|---|---|---|
| `SetEvent`/`ResetEvent` → ret=1 (2515-2516) | TRUE always | fake-success on invalid handle only; real work on valid handles. Fix: `if event_for(h).is_none() { last_error(ERROR_INVALID_HANDLE=6); ret=0 }`. Low priority. |
| `PulseEvent` → set+reset, ret=1 (2517) | TRUE | honest-enough for a deprecated API ‡; wake-one nuance unmodeled (cv notify_all). Leave. |
| `ReleaseMutex` → ret=1 unconditional (2518) | TRUE even when mutex_release()==false | **fake-success** — honest fix: propagate the bool, FALSE+`ERROR_NOT_OWNER` (288) on non-owner. |
| `ReleaseSemaphore` → ret=1, *prev=0 (2519-2523) | TRUE + zero out-param | **fake-success on the out-param** — SemObj holds the real count; fix: `sem_release` returns `Option<prev>`, write it, and enforce max → FALSE+ERROR_TOO_MANY_POSTS(298). This is the one constant in the family that feeds live data a game reads. |
| `DeleteCriticalSection` no-op (978) | — | fully-correct-constant (void; nothing observable). |
| `InitializeCriticalSection*` → ret=1 (940) | TRUE | fully-correct-constant (Ex only fails on invalid args, which would've faulted anyway). |
| WFSO untracked-handle → ret=0 (2538-2541) | WAIT_OBJECT_0 | **fake-success** for non-file handles. Honest fix: consult the handle table kind — file/console kinds → 0 stays honest; unknown → die-loud (matches the comment's own "say so once per handle" intent, currently silent) or WAIT_FAILED+ERROR_INVALID_HANDLE. |
| `Sleep` cap `min(1000)` (2763) | — | not a constant-return but a constant-cap; honest fix: honor full duration now that H3 proved long finite waits are legit (SHIM:171 already honors 60s+ waits — the Sleep cap is a leftover of the same fear). |
| `SleepConditionVariable*` timeout `set_last_error(0x102)` (2812/2836) | 258 | **wrong-constant** — one-line fix to 1460 (ERROR_TIMEOUT). The cheapest TRUST-CHAIN close in the whole family. |

Priority order for fixes (impact × cheapness): (1) row-4 error code 0x102→1460;
(2) row-14 OwningThread minted-tid; (3) ReleaseSemaphore real prev+max; (4) row-2/11
untracked-handle honesty (die-loud); (5) named-object table for row 1 (biggest lift,
needed only if a name-collision branch is actually observed firing — instrument first:
log every non-NULL lpName at create; if CP2077 never passes names, row 1 is moot).

---
*Written by task 722f36a1 (02_SYNC wave). Wine master @ /local/home/seratb/wine;
shim @ /tmp/alky-shims-lib.rs as of 2026-08-12. † = source-only (no live Windows
cross-check); ‡ marked inline where inferred.*
