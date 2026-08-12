# 07_THREAD_PROC — threads, processes, TLS/FLS, topology

Wave contract: WINE-SPECS/WAVE-CONTRACT.md. Wine @ /local/home/seratb/wine (master, depth-1).
Shim @ /tmp/alky-shims-lib.rs (3211-line dispatch) + the CreateThread ORGAN (native-registered, not
in the dispatch match; repo copy at alky/crates/alky-loader/src/shims_thread.rs — ‡ the repo organ
may be a different generation than the /tmp lib: repo mints handles `0x9000+seq`
(shims_thread.rs:96,912) while the /tmp lib's ExitThread/GetThreadId assume `0x7000_0000|tid`
(alky-shims-lib.rs:1432,1565). Behavior cited from the repo organ is marked ‡ accordingly.)

Touched by CP2077 (touched_apis_cp2077.txt): CreateProcessW, FlsAlloc/FlsGetValue/FlsSetValue,
GetCurrentProcess, GetCurrentProcessId, GetCurrentThread(Id), GetLogicalProcessorInformationEx,
SetThreadPriority, SwitchToThread, TlsAlloc/TlsGetValue/TlsSetValue.
Assigned but NOT touched: CreateThread¹, SuspendThread, ResumeThread, TlsFree, FlsFree,
SetThreadAffinityMask, QueueUserAPC, GetSystemCpuSetInformation².
¹ CP2077 reaches threads via `_beginthreadex`/threadpool — CreateThread the organ still serves both.
² GetSystemCpuSetInformation IS in the touched list (line 57 region of the file) — treated as touched.

---

## 1. SPEC

### CreateThread
`HANDLE CreateThread(SECURITY_ATTRIBUTES*, SIZE_T stack, LPTHREAD_START_ROUTINE start, void* param, DWORD flags, DWORD* id)`
— kernelbase/thread.c:169-175, forwards to CreateRemoteThreadEx(GetCurrentProcess(),…) at :174.
- CreateRemoteThreadEx (thread.c:110-165): builds PS_ATTRIBUTE_LIST with CLIENT_ID + TEB address,
  calls `NtCreateThreadEx(…, THREAD_ALL_ACCESS, …, CREATE_SUSPENDED, 0, stack_commit, stack_reserve …)`
  — the thread is ALWAYS created suspended at the NT layer (:144-146), then
  `if (!(flags & CREATE_SUSPENDED)) NtResumeThread(handle, &ret)` (:161). So CREATE_SUSPENDED
  semantics = the new thread has suspend count 1 and has NOT executed one instruction of `start`.
- Success: nonzero HANDLE (real, must be CloseHandle'd); `*id` filled with the new TID if non-NULL.
- Failure: NULL + last-error via set_ntstatus (:144).
- Stack: `flags & STACK_SIZE_PARAM_IS_A_RESERVATION ? reserve : commit` split (:139-141);
  size 0 = executable defaults.
- Conformance (tests/thread.c:426-516 test_CreateThread_basic): exit code retrievable after death
  (GetExitCodeThread == thread return value, :471-473); `*id` ≠ caller's tid (:487-489); handle
  survives thread death until closed. TIDs on real Windows are multiples of 4 (allocated from the
  handle-table ID space) — no conformance test asserts it, but anti-debug code in the wild does.
- CREATE_SUSPENDED conformance (tests/thread.c:520-560): thread created suspended →
  `SuspendThread(t)==1` (count was 1, now 2), `ResumeThread(t)==2`, thread STILL not running after
  one resume (`WaitForSingleObject(t,1000)==WAIT_TIMEOUT`, :529-531), `ResumeThread(t)==1` → now runs.

### CreateProcessW
`BOOL CreateProcessW(app, cmdline, pattr, tattr, inherit, flags, env, cur_dir, STARTUPINFOW*, PROCESS_INFORMATION*)`
— kernelbase/process.c:711-718 → CreateProcessInternalW (:508-696).
- Success: TRUE; PROCESS_INFORMATION filled `{hProcess, hThread, dwProcessId, dwThreadId}`
  (:679-683); child's initial thread resumed unless CREATE_SUSPENDED (:683).
- Failure: FALSE + last-error (image not found → ERROR_FILE_NOT_FOUND; bad cur_dir →
  ERROR_DIRECTORY :555-559; non-PE → ERROR_BAD_EXE_FORMAT path). `info` fields zeroed early (:563-564)
  but MSDN-contract = undefined on failure — callers may only trust them on TRUE.
- Priority-class flags warn-only in Wine (:549-552).

### GetCurrentProcess / GetCurrentProcessId
- GetCurrentProcess: constant pseudo-handle `(HANDLE)~0` — kernelbase/process.c:768-771 and the
  inline in include/processthreadsapi.h:231-234. Never closed, valid everywhere in-process.
- GetCurrentProcessId: `HandleToULong(NtCurrentTeb()->ClientId.UniqueProcess)` — process.c:777-780.
  Stable for process lifetime; PIDs are multiples of 4 on real Windows and never collide with any
  live TID (same ID space).
- (Adjacent: GetCurrentThread = `(HANDLE)~1`, processthreadsapi.h:241-244.)

### SuspendThread  (census suspect)
`DWORD SuspendThread(HANDLE)` — kernelbase/thread.c:689-700.
- **Return value = the PREVIOUS suspend count** (0 = was running, now suspended; 1 = was suspended
  once, now twice). Failure = `(DWORD)-1` + last-error (:699). Win9x-compat quirk: NT-in-Win9x-mode
  returns 0 for the current thread (:694-696) — not relevant to NT contract.
- Mechanism: NtSuspendThread → wineserver `suspend_thread` (server/thread.c:914-925): increments
  `thread->suspend` up to MAXIMUM_SUSPEND_COUNT (127, include/winnt.h:2376), sends SIGUSR1 to stop
  the target (server/thread.c:905-911), returns the OLD count. Count > 127 →
  STATUS_SUSPEND_COUNT_EXCEEDED. Suspending a TERMINATED thread → STATUS_ACCESS_DENIED → -1
  (server/thread.c:1877-1887; conformance tests/thread.c:549-551 expects exactly -1).
- **Side-effect the caller trusts: after return, the target executes NOTHING** until resumed. The
  canonical consumer chain is `SuspendThread → GetThreadContext → (stack walk / patch) → ResumeThread`
  (crash handlers, profilers, GC-style rendezvous). A second pattern: self-suspend
  (`SuspendThread(GetCurrentThread())`) only RETURNS after another thread resumes it — it's a
  synchronization primitive.
- Handle needs THREAD_SUSPEND_RESUME access; without it → -1 (tests/thread.c:592-596).

### ResumeThread
`DWORD ResumeThread(HANDLE)` — kernelbase/thread.c:455-460.
- Return = PREVIOUS suspend count: 0 = was not suspended (no-op, still returns 0 — conformance
  tests/thread.c:553-556 accepts 0 after terminate), 1 = was suspended once and is NOW running,
  N>1 = still suspended (N-1 remaining). Failure = -1.
- wineserver resume_thread (server/thread.c:927-936): decrements, wakes at 0.

### TlsAlloc / TlsGetValue / TlsSetValue / TlsFree
kernelbase/thread.c:723-828. Two banks: TEB->TlsSlots[64] (TLS_MINIMUM_AVAILABLE) via PEB
TlsBitmap, then TEB->TlsExpansionSlots[1024] via TlsExpansionBitmap (lazily heap-alloc'd, :736-747).
- TlsAlloc (:723): bitmap find-first-clear under PEB lock; **index reuse is the norm**; the slot is
  zeroed in the CURRENT thread at alloc (:733, :746) and was zeroed process-wide by the prior
  TlsFree. Exhaustion (64+1024 all set) → `TLS_OUT_OF_INDEXES` (~0) + ERROR_NO_MORE_ITEMS (:750).
- TlsGetValue (:785-800): returns slot value; **sets last-error to ERROR_SUCCESS on success**
  (:787) — this is load-bearing: return 0 + GetLastError()==ERROR_SUCCESS distinguishes a stored
  zero from a bad index (bad index → NULL + ERROR_INVALID_PARAMETER, :792-795). No PEB lock — hot path.
- TlsSetValue (:804-828): stores; invalid expansion index → FALSE + ERROR_INVALID_PARAMETER
  (:810-816 lower bank is unchecked ≤63… index<64 always valid). Lazily allocates expansion array.
- TlsFree (:760-781): validates the bit WAS set (double-free / never-alloc'd → FALSE +
  ERROR_INVALID_PARAMETER, :777; conformance tests/thread.c:499-506), clears bitmap, then
  `NtSetInformationThread(ThreadZeroTlsCell)` zeroes that cell in **every thread of the process**
  (:776) so the next TlsAlloc hands out a clean slot. Success does NOT touch last-error
  (conformance :494-497 sees its 0xCAFEF00D sentinel preserved).
- Conformance test_TLS (tests/thread.c:1776-1791 driving 1596-1770): values are strictly per-thread;
  a new thread reads 0 from any slot it hasn't written.

### FlsAlloc / FlsGetValue / FlsSetValue / FlsFree  (fiber-local storage)
kernelbase wrappers thread.c:1268-1305 over ntdll RtlFls* (ntdll/thread.c:516-676).
- Process-wide index space, chunked; **index 0 is prohibited** (reserved at first alloc,
  ntdll/thread.c:560-562; conformance fiber.c expects first index == 1). Max index space
  MAX_FLS_DATA_COUNT = 0xff0 (ntdll/thread.c:465); exhaustion → FLS_OUT_OF_INDEXES.
- FlsAlloc(callback) **registers the callback process-wide** (ntdll/thread.c:567: stored in the
  callback chunk; NULL callback stored as ~0 sentinel). Per-thread storage (TEB->FlsSlots) is
  lazily created and linked into a process-global list of all threads' FLS data
  (fls_alloc_data, ntdll/thread.c:498-509) — that list is what lets FlsFree reach every thread.
- FlsSetValue (:632-655): index validated (0 or ≥0xff0 → STATUS_INVALID_PARAMETER → FALSE);
  lazily allocs the thread's chunk; stores.
- FlsGetValue (:660-672): kernelbase sets ERROR_SUCCESS on success (thread.c:1294); bad/never-set
  index → STATUS_INVALID_PARAMETER → NULL. Never-set-but-valid index → NULL with SUCCESS status.
- FlsFree (:581-628): validates index → for **every thread's** live non-NULL value at that index,
  **calls the registered callback synchronously with that value** (:615, under the FLS lock), NULLs
  the slot, then unregisters the callback. Conformance fiber.c:395-460: callback called exactly once
  per live value with the stored pointer.
- **Thread-exit semantics — the load-bearing part:** LdrShutdownThread calls
  `RtlProcessFlsData(TEB->FlsSlots, 1)` (fires all callbacks for that thread's live values) then
  flags=2 (frees + unlinks) — ntdll/loader.c:3963, 3994. So a callback registered via FlsAlloc
  **fires at every thread exit** (and at process exit for the last thread, loader.c:3928). CRT
  (ucrtbase) hangs its per-thread data destructor on exactly this; games use it for per-thread
  scratch-arena teardown. RtlProcessFlsData itself: ntdll/thread.c:678-731 (flags=1 = fire+unlink,
  flags=2 = free storage, no callbacks — conformance fiber.c:480-525).

### GetLogicalProcessorInformationEx
`BOOL GetLogicalProcessorInformationEx(LOGICAL_PROCESSOR_RELATIONSHIP rel, SYSTEM_LOGICAL_PROCESSOR_INFORMATION_EX* buf, DWORD* len)`
— kernelbase/memory.c:1520-1537.
- NULL len → FALSE + ERROR_INVALID_PARAMETER (:1527-1531). Two-call pattern: too-small buffer →
  FALSE + ERROR_INSUFFICIENT_BUFFER (STATUS_INFO_LENGTH_MISMATCH mapped :1534) with `*len` =
  required bytes. Success: TRUE, `*len` = bytes written.
- Buffer = packed variable-size records, each `{Relationship, Size, union…}`; consumer must walk by
  `Size`. **The relationship arg FILTERS**: RelationProcessorCore → one record per physical core
  (GroupMask carries the SMT siblings; Flags=LTP_PC_SMT when >1 LP); RelationCache → one per cache;
  RelationNumaNode, RelationGroup, RelationProcessorPackage similarly; RelationAll (0xffff) → all of
  them concatenated (ntdll/unix/system.c:4152+ serves it from the cached host topology snapshot
  built at boot). REDengine sizes worker pools from the CORE-record count vs the LP-mask popcount —
  both must be coherent.

### GetSystemCpuSetInformation
`BOOL GetSystemCpuSetInformation(SYSTEM_CPU_SET_INFORMATION* info, ULONG len, ULONG* retlen, HANDLE process, ULONG flags)`
— kernelbase/memory.c:1540-1552.
- `*retlen` zeroed up-front (:1548); NtQuerySystemInformationEx(SystemCpuSetInformation) fills one
  32-byte SYSTEM_CPU_SET_INFORMATION per logical processor (ntdll/unix/system.c:1785-1840
  create_cpuset_info — Id, Group, LogicalProcessorIndex, CoreIndex, LastLevelCacheIndex derived from
  the same topology snapshot as GLPI-Ex). Probe call (small/NULL buffer) → FALSE +
  ERROR_INSUFFICIENT_BUFFER with `*retlen` = required. **On real Windows ≥ Win10 this never
  succeeds with zero entries** — count == LP count.

### SetThreadPriority / SetThreadAffinityMask
- SetThreadPriority — kernelbase/thread.c:617-621: `NtSetInformationThread(ThreadBasePriority)`.
  Valid range −2..2 plus THREAD_PRIORITY_TIME_CRITICAL(15)/IDLE(−15); out-of-range → FALSE +
  ERROR_INVALID_PARAMETER **and the priority is unchanged** (conformance tests/thread.c:688-745:
  set→GetThreadPriority readback must equal the set value for every valid level; invalid input
  leaves the old value readable).
- SetThreadAffinityMask — kernel32/thread.c:96-105: queries ThreadBasicInformation first, then
  NtSetInformationThread(ThreadAffinityMask); **returns the PREVIOUS mask**, 0 on failure
  (mask ⊄ process affinity → ERROR_INVALID_PARAMETER). Side-effect: the thread really is confined
  to those CPUs.

### SwitchToThread
kernelbase/thread.c:705-708: `return NtYieldExecution() != STATUS_NO_YIELD_PERFORMED`.
- TRUE = the OS actually ran another thread; FALSE = nothing to yield to. Wine itself approximates
  the bit via sched_yield + RUSAGE_THREAD deltas (ntdll/unix/sync.c:2395-2420) — the honest signal
  is best-effort even in Wine.

### QueueUserAPC
`DWORD QueueUserAPC(PAPCFUNC func, HANDLE thread, ULONG_PTR data)` — kernelbase/thread.c:425-429:
NtQueueApcThread with a marshalling thunk (call_user_apc :416-421).
- Nonzero = queued; FALSE + last-error on failure. Queuing to a TERMINATED thread → FALSE +
  ERROR_GEN_FAILURE (conformance tests/sync.c:3024-3050, the STATUS_UNSUCCESSFUL mapping).
- **Delivery contract:** the APC runs in the TARGET thread's context, only when that thread enters
  an ALERTABLE wait (SleepEx(…,TRUE), WaitForSingleObjectEx/WaitForMultipleObjectsEx(…,TRUE),
  MsgWaitForMultipleObjectsEx w/ MWMO_ALERTABLE). The wait then returns WAIT_IO_COMPLETION after
  ALL queued APCs ran, in FIFO order (conformance sync.c:3055-3060: queue → SleepEx(100,TRUE) →
  WAIT_IO_COMPLETION, apc_count==1). Handle needs THREAD_SET_CONTEXT. APCs also drain at
  NtTestAlert and during thread termination cleanup on real NT.

---

## 2. DIVERGENCE table

| # | fn | real Windows (Wine cite) | ours (shim cite) | severity |
|---|----|--------------------------|------------------|----------|
| D1 | SuspendThread | Target provably stopped on return; ret = prior count; −1 on fail (kernelbase/thread.c:689-700, server/thread.c:914-925) | `ret = 0` — **target keeps running**; claims "was running, now suspended" (alky-shims-lib.rs:1564) | **TRUST-CHAIN** (the fake-success census class; details §3) |
| D2 | ResumeThread | ret = prior count: 0 if wasn't suspended, N>1 = still suspended (thread.c:455-460; tests:529-556) | `ret = 1` constant — "was suspended once, now running" even for never-suspended/terminated threads (lib.rs:1563) | WRONG-DATA (consistent with D3's lie; wrong for the terminated-thread → 0 case tests assert) |
| D3 | CreateThread | CREATE_SUSPENDED = thread exists but has executed nothing until ResumeThread (kernelbase/thread.c:144-161; tests:520-560) | organ ignores flags&4 — spawns immediately, only logs " SUSPENDED" (shims_thread.rs:917 ‡) | **BLOCKER-class race**: spawn-then-configure patterns (set affinity/priority/bookkeeping, then resume) run the thread before setup; self-suspend rendezvous ordering breaks |
| D4 | CreateThread | TIDs multiple-of-4 from handle-ID space; NULL on fail + last-error | organ mints sequential tids from 1000 (shims_thread.rs:97,868 ‡); no last-error on pthread_create fail (:909-911 ‡) | BENIGN |
| D5 | FlsAlloc/FlsFree/thread-exit | callback registered; fires per live value on FlsFree (ntdll/thread.c:615) and at EVERY thread exit via RtlProcessFlsData(…,1) (ntdll/loader.c:3963,3994) | **callback argument discarded** — FlsAlloc never stores it (lib.rs:2429); no exit hook fires anything; FlsFree just drops the index (lib.rs:2432) | **BLOCKER/leak-UAF class**: CRT per-thread-data dtor + any game cleanup registered there never runs → per-thread-exit leak; if the callback unlinks a per-thread node from a global structure, its absence leaves dangling pointers = later UAF |
| D6 | FlsAlloc | index 0 prohibited, first index = 1; cap 0xff0; FLS_OUT_OF_INDEXES on exhaustion (ntdll/thread.c:560-562,465) | shared counter (see D7) starting 0x1000 if FLS first-toucher; unbounded; can never return 0 in practice | BENIGN |
| D7 | TlsAlloc vs FlsAlloc | disjoint namespaces (TEB slots+bitmaps vs process FLS chunks) | ONE shared `TLS` counter + ONE `TLSV` value map serve both (lib.rs:83,87,2195-2205,2429-2432) — indices come from the same monotonic counter, values keyed (tid,idx) in one map | BENIGN today (single counter ⇒ no collisions), fragile: any future per-namespace reset/reuse collides Tls and Fls values |
| D8 | TlsGetValue/FlsGetValue | set last-error ERROR_SUCCESS on success (kernelbase/thread.c:787,1294) — the stored-zero vs bad-index discriminator | last-error untouched (lib.rs:2202,2431) | WRONG-DATA (low; a caller checking GetLastError after a 0 read sees a stale code) |
| D9 | TlsFree/TlsSetValue | double-free / bad index → FALSE + ERROR_INVALID_PARAMETER (thread.c:777,810-816; tests:499-506); TlsFree zeroes the cell in all threads (:776) | always ret 1; no validation; freed values persist in TLSV (lib.rs:2199,2205) | BENIGN (no index reuse ⇒ stale values unreachable; validation only surfaces game bugs) |
| D10 | GetLogicalProcessorInformationEx | `relationship` arg filters record types; RelationAll = full concatenated topology (memory.c:1520-1537, unix/system.c:4152+) | **arg ignored** — every query gets the same single RelationProcessorCore record (lib.rs:1573-1597) | WRONG-DATA: a RelationCache/NumaNode/All caller iterating by `.Relationship` finds zero matching records (or misparses) |
| D11 | GetLogicalProcessorInformationEx | one record per physical core; SMT expressed via Flags+mask; core-record count == physical cores | ONE core record, GroupMask 0xff, Flags=0 (lib.rs:1583-1595) = "1 physical core carrying 8 non-SMT LPs" — self-contradictory topology | **TRUST-CHAIN** (pool sizing): count-the-core-records consumers (REDengine's pattern) size worker pools to **1**; popcount-the-mask consumers get 8. Two coherent-looking APIs disagree |
| D12 | GetSystemCpuSetInformation | probe → FALSE + ERROR_INSUFFICIENT_BUFFER + required len; success always ≥1 entry/LP (memory.c:1540-1552, unix/system.c:1785+) | TRUE + retlen 0 always (lib.rs:1566-1572) — "success, zero CPU sets", a shape real Win10+ never produces | WRONG-DATA (deliberate honest-empty → game takes its no-cpusets fallback; ‡ CP2077 tolerates it — shipped Win7 path) |
| D13 | SetThreadPriority + GetThreadPriority | value stored & readable back; invalid value rejected, old value preserved (thread.c:617-621; tests:688-745) | Set: ret 1 no-op (lib.rs:1562); Get: ret 0 constant (lib.rs:1598) — a set of HIGHEST reads back NORMAL | WRONG-DATA (readback broken; scheduling effect lost — advisory) |
| D14 | SetThreadAffinityMask | returns previous mask, 0+ERROR_INVALID_PARAMETER for bad mask, actually confines the thread (kernel32/thread.c:96-105) | ret 0xff constant, no-op, no validation (lib.rs:1609) | BENIGN (perf-hint loss; 0xff is at least consistent with D11's 8-LP story) |
| D15 | SwitchToThread | TRUE only if a yield actually happened (thread.c:705-708) | `yield_now(); ret=1` always (lib.rs:2767) | BENIGN (backoff loops stay at yield tier; correctness unaffected) |
| D16 | GetCurrentProcessId | real PID, mult-of-4, disjoint from all TIDs | constant 1000 (lib.rs:2273) — **collides with the organ's first minted TID (NEXT_TID starts at 1000, shims_thread.rs:97 ‡)** | BENIGN-latent: `tid == pid` is an impossible state on Windows; anything keying a shared map by "id" can alias thread 1000 with the process |
| D17 | QueueUserAPC + alertable delivery | queue → target's next alertable wait runs APCs FIFO, returns WAIT_IO_COMPLETION; terminated target → ERROR_GEN_FAILURE (thread.c:425-429; tests/sync.c:3024-3060) | **no arm at all** → die-loud unhandled (lib.rs:3207 `_ => handled_real=false`); additionally Sleep/SleepEx share one non-alertable arm (lib.rs:2761) and WaitForSingleObjectEx ignores the alertable flag (lib.rs:2524) — no delivery point exists | MISSING — die-loud is the honest state per contract; untouched by CP2077 |
| D18 | CreateProcessW | launches; TRUE + PROCESS_INFORMATION; FALSE + accurate last-error | honest FALSE + log, but last-error = 2 ERROR_FILE_NOT_FOUND even when the image exists (lib.rs:1008-1016) | BENIGN (deliberate honest-fail; the code fib routes launchers into their file-missing branch rather than access-denied — chosen, works) |
| D19 | GetCurrentProcess | pseudo-handle ~0 | `u64::MAX` (lib.rs:2378) | **PASS** — exact real value (GetCurrentThread ~1 = `MAX-1` at :2379 likewise) |
| D20 | TlsAlloc/Get/Set core | per-thread values, 0-default | per-(tid,idx) map keyed by TEB ClientId.UniqueThread via gs:[0x48] (lib.rs:88-93,2195-2204), 0-default | **PASS** (per-thread isolation + zero-init honored; caveats D8/D9) |

PASS summary (constant answers that ARE the honest answer): GetCurrentProcess (D19),
GetCurrentThread, TLS core behavior (D20), CreateProcessW-as-honest-fail (D18 modulo error code),
Sleep being a real sleep (lib.rs:2761-2765).

---

## 3. RET-0 GRADING (constant-return arms)

**SuspendThread → 0 — FAKE-SUCCESS, the sync-class census suspect. Priority depth:**
- *What the return value means:* the suspend count BEFORE this call (kernelbase/thread.c:689-700;
  server/thread.c:914-925 returns `old_count`). 0 specifically asserts "it was running and I have
  now stopped it". −1 means failure. The value is a counter contract: N suspends need N resumes
  (tests/thread.c:520-560 count ladder).
- *Who consumes it:* (a) suspend→GetThreadContext→stack-walk→resume chains — crash handlers
  (CP2077 ships one), sampling profilers, in-game watchdog dumps; (b) suspend-count ladders that
  match Suspend/Resume pairs (engine job-system pause-the-world); (c) self-suspend rendezvous
  (`SuspendThread(GetCurrentThread())` returns only after a peer resumes — the RETURN ITSELF is the
  sync signal); (d) anti-tamper checking the counter monotonicity (rare in CP2077's case).
- *What breaks when the thread isn't actually suspended:* the (a) chain reads a LIVE thread's
  context/stack — torn RSP/RIP pairs, stack pages mutating mid-walk → garbage dumps at best, sampler
  crash at worst; the fake 0 tells it the snapshot is safe when it's a race. (b) pause-the-world
  proceeds to mutate shared state believing workers are parked → data races that Windows ordering
  made impossible. (c) is inverted: the self-suspender continues INSTANTLY as if resumed — the
  happens-before edge the game built (peer publishes data, THEN resumes me) is gone; reads-before-
  publish = corruption. Our paired ResumeThread=1 keeps the lie self-consistent for the trivial
  suspend-once/resume-once pattern, which is why it hasn't detonated yet — count ladders and
  context-capture chains are where it goes off.
- *Honest fix:* real implementation is cheap on Linux — the Wine mechanism verbatim: directed
  `pthread_kill(SIGUSR1)`, handler parks on a per-thread futex/condvar, per-thread `suspend: u8`
  counter under the THREADS map lock; SuspendThread returns old count, ResumeThread decrements and
  wakes at 0; self-suspend = park in the caller after signaling. Until then the honest constant is
  **~0u32 (failure) + a loud log**, not 0: every consumer above treats −1 as "skip this thread /
  degrade" (the crash handler skips a thread it can't stop) instead of trusting a snapshot that
  isn't one. Die-loud is acceptable per contract; fail-honest is strictly better here because the
  (a) consumer has a real degraded path.

Other constant arms:
- **ResumeThread → 1** (lib.rs:1563): fake (asserts a suspension existed). Fix rides the
  SuspendThread fix: return the real decremented-from count; with CREATE_SUSPENDED honored (below)
  the constant dies naturally.
- **CreateThread ignoring CREATE_SUSPENDED** (organ, shims_thread.rs:917 ‡): not a ret-constant but
  the same fake-success family (claims a suspended thread). Fix sketch: trampoline checks flags&4 →
  parks on the thread's suspend-gate condvar BEFORE first guest instruction, count=1;
  ResumeThread(handle) decrements/wakes. ~20 lines given the fix above.
- **SetThreadPriority → 1 / GetThreadPriority → 0** (lib.rs:1562,1598): fake-success pair —
  readback contract broken (tests:688-745). Fix: per-tid `priority: i32` in the THREADS map; Set
  validates range (−2..2, ±15) → FALSE+ERROR_INVALID_PARAMETER else stores; Get returns stored.
  Optional: map to `setpriority`/nice best-effort. Cheap, kills the readback lie.
- **SetThreadAffinityMask → 0xff** (lib.rs:1609): fake-success with a plausible constant. Fix:
  store per-tid mask (default 0xff), return previous stored; optionally `sched_setaffinity`.
  Low urgency — advisory consumer class.
- **SwitchToThread → 1** (lib.rs:2767): near-honest — the yield genuinely happens; only the
  did-a-switch-occur bit is invented, and Wine itself approximates it (unix/sync.c:2395+).
  **Fully-correct-constant** for practical purposes; leave.
- **GetCurrentProcess → u64::MAX** (lib.rs:2378): **fully-correct-constant** (the real pseudo-handle).
- **GetCurrentProcessId → 1000** (lib.rs:2273): correct-shaped constant (nonzero, mult-of-4); flaw
  is the TID-space collision (D16). Fix: one line — start NEXT_TID at 1004, or pid=4.
- **GetSystemCpuSetInformation → TRUE+len 0** (lib.rs:1566-1572): honest-empty but wrong SHAPE
  (real Win10+ never zero-entry-succeeds; probe should fail-with-required). Fix: emit 8×32-byte
  entries (Id 0x100+i, Group 0, LPIndex/CoreIndex i, one entry per LP of the SAME 8-CPU model as
  GLPI) with the proper two-call dance — keeps the topology story single-sourced.
- **GetLogicalProcessorInformationEx** (lib.rs:1573-1597): not constant-return but WRONG-DATA at
  the census tier (D10/D11). Fix: honor `relationship`; emit 8 RelationProcessorCore records
  (mask 1<<i, Flags 0) + one RelationGroup + one RelationNumaNode + optional caches; RelationAll =
  concatenation. This is THE CPU-topology answer REDengine sizes pools from — 8 cores must read as
  8 core-records, not 1.
- **FlsAlloc discarding its callback** (lib.rs:2429) + **FlsFree → 1** (lib.rs:2432): fake-success
  on the side-effect channel (D5) — the callback IS the contract. Fix sketch: `FLS_CB: Mutex<Map<u32, u64>>`;
  FlsAlloc stores; FlsFree drains TLSV for that idx across all tids, calling the guest callback with
  each value (win64 call via the existing guest-call plumbing), then unregisters; ExitThread
  (lib.rs:1421-1432, where thread_mark_done already lives) walks TLSV for the dying tid over FLS
  indices and fires callbacks before marking done. Without the exit hook, every CRT-carrying thread
  exit leaks its ptd — and any game cleanup registered there silently never runs.
- **TlsFree/TlsSetValue/FlsSetValue → 1 unvalidated** (lib.rs:2199,2205,2430): behavior behind the
  return is real (values stored per-thread); missing validation only converts game bugs into silent
  ones. Low priority; acceptable constants.
- **CreateProcessW → FALSE** (lib.rs:1008-1016): **honest-fail, correct per contract** — keep; swap
  last-error to something not claiming file-absence (e.g. ERROR_ACCESS_DENIED) if a launcher ever
  mis-branches on 2.
- **_beginthreadex → 0 in dispatch** (lib.rs:1219-1221): honest-fail NULL for the
  shouldn't-happen path (normally organ-served native-side). Fine.
- **QueueUserAPC — absent** (die-loud via lib.rs:3207): honest per contract. If ever needed:
  per-thread APC queue drained at an alertable-aware SleepEx/WFSOEx (both currently ignore
  alertable — lib.rs:2761,2524 — so the wait arms must learn the flag first, returning
  WAIT_IO_COMPLETION after draining).
