# 08 — TIME / SYSINFO (Wine-spec wave)

Sources: wine @ /local/home/seratb/wine (master, depth-1) · shim @ /tmp/alky-shims-lib.rs (3211 ln)
· touched-list @ WINE-SPECS/touched_apis_cp2077.txt. Format per WAVE-CONTRACT.md.
† = source-only claim, ‡ = inference.

Touched-list cross-check: CP2077's IAT touches **GetTickCount, QueryPerformanceCounter/Frequency,
GetSystemTime, GetSystemTimeAsFileTime, GetSystemTimePreciseAsFileTime, FileTimeToSystemTime,
FileTimeToLocalFileTime, SystemTimeToFileTime, GetSystemInfo, VerifyVersionInfoA/W, GetStartupInfoA/W,
GetCommandLineA/W, GetEnvironmentStringsW, FreeEnvironmentStringsW, GetEnvironmentVariableW,
CallNtPowerInformation, AppPolicyGetThreadInitializationType**. GetTickCount64 +
GetSystemTimePreciseAsFileTime are ALSO resolved dynamically (GPA_SHIMMED batch, shim :136 — "found
via GPA-miss logging at the 2M-shim native frontier"). NOT in the touched list (spec'd here as
LATENT, for completeness of the family): GetLocalTime, GetTickCount64(IAT), GetTimeZoneInformation,
SystemTimeToTzSpecificLocalTime, GetComputerNameW/ExW, GetVersion/GetVersionExW/RtlGetVersion,
IsProcessorFeaturePresent, GetNativeSystemInfo, ExpandEnvironmentStringsW, GetSystemPowerStatus.
(GetVersion, IsProcessorFeaturePresent, GetNativeSystemInfo, GetSystemPowerStatus nonetheless have
shim arms — they were hit at runtime via thunks even though absent from the static IAT list ‡.)

---

## 1. SPEC

### Tick counters

**GetTickCount() → DWORD** — ms since boot, monotonic non-decreasing, wraps at 49.7 days.
Wine: `user_shared_data->TickCount.LowPart` (TickCountMultiplier deliberately ignored) —
kernelbase/sync.c:181-186. No failure mode, no last-error. Resolution on real Windows = timer tick
(~10-16ms); callers may NOT assume 1ms granularity but MAY assume ordering.
Conformance: tests/time.c:1080-1100 `test_GetTickCount` asserts interleaved ordering
`NtGetTickCount() <= GetTickCount() <= NtGetTickCount()` — the ONLY hard contract is monotone
consistency with the kernel tick.

**GetTickCount64() → ULONGLONG** — same clock, no wrap. Wine reads High1Time/LowPart with a
High2Time seqlock retry (kernelbase/sync.c:191-203). Same no-fail contract.

### QPC

**QueryPerformanceCounter(LARGE_INTEGER*) → BOOL** — kernel32/kernelbase forward to
`RtlQueryPerformanceCounter` (ntdll/time.c:382-386) which always returns TRUE and fills from
`NtQueryPerformanceCounter` (ntdll/unix/sync.c:2471-2476) = `monotonic_counter()`
(unix/sync.c:89-108: CLOCK_BOOTTIME, falling back CLOCK_MONOTONIC, expressed in 100ns ticks).
**QueryPerformanceFrequency(LARGE_INTEGER*) → BOOL** — always TRUE; Wine reports a constant
TICKSPERSEC = 10,000,000 (10 MHz) — ntdll/time.c:391-395†.
The REAL contract (what games rely on): (a) counter is **monotonic non-decreasing across all
threads/cores**; (b) frequency is **fixed at boot** — any value is legal, callers must divide;
(c) counter/frequency = seconds, consistent with wall-clock rate to within timer drift. Real
Windows 10 on invariant-TSC hardware also reports 10 MHz. Never fails on ≥XP; the BOOL is
vestigial.

### System time

**GetSystemTime(SYSTEMTIME*) → void** — UTC now, split into 8×u16 fields
{wYear,wMonth,wDayOfWeek,wDay,wHour,wMinute,wSecond,wMilliseconds}. Wine: NtQuerySystemTime →
FileTimeToSystemTime (kernelbase/file.c:4197-4204). **GetLocalTime** = same + RtlSystemTimeToLocalTime
tz-bias first (file.c:4184-4193).
**GetSystemTimeAsFileTime(FILETIME*) → void** — UTC now as 100ns-since-1601-01-01. Wine =
NtQuerySystemTime directly (file.c:4226-4229; unix side = CLOCK_REALTIME).
**GetSystemTimePreciseAsFileTime(FILETIME*) → void** — same epoch, finest resolution
(RtlGetSystemTimePrecise, ntdll/time.c:371-377 → unix `system_time_precise`, unix/sync.c:2597).
Conformance (tests/time.c:801-814): `GetSystemTimeAsFileTime` is bracketed by two
`NtQuerySystemTime` reads — must be **ordered against the kernel clock**. tests/time.c:815-844:
precise ≥ coarse and |precise−coarse| < 1s. **The three "now" sources (SystemTime, AsFileTime,
PreciseAsFileTime) must agree on the same wall clock** — the tests convert between them and compare.

### FILETIME ↔ SYSTEMTIME conversions

**FileTimeToSystemTime(const FILETIME*, SYSTEMTIME*) → BOOL** — full civil-calendar conversion.
Failure: `ft.QuadPart < 0` → SetLastError(ERROR_INVALID_PARAMETER), FALSE
(kernelbase/file.c:4163-4167). Fills wDayOfWeek correctly (RtlTimeToTimeFields).
Conformance test_FileTimeToSystemTime (tests/time.c:370-406) checks exact field decode incl. the
1601 epoch and rounds.
**SystemTimeToFileTime(const SYSTEMTIME*, FILETIME*) → BOOL** — inverse; **validates fields**
(month 1-12, day in month, h<24, m/s<60, ms<1000) via RtlTimeFieldsToTime → ERROR_INVALID_PARAMETER
+ FALSE on bad input; **wDayOfWeek is IGNORED on input** (tests/time.c:163-235 `test_invalid_arg`
proves both: wrong day-of-week accepted, invalid month rejected). Uses full y/m/d — any date works,
not just "today".
**FileTimeToLocalFileTime(const FILETIME* utc, FILETIME* local) → BOOL** — utc + tz bias
(RtlSystemTimeToLocalTime, kernelbase/file.c:4149-4154). On a UTC-configured box this is identity.
Conformance tests/time.c:408-457 computes it as `utc + tzinfo.Bias*600000000`.
**SystemTimeToTzSpecificLocalTime(const TIME_ZONE_INFORMATION*, const SYSTEMTIME*, SYSTEMTIME*) → BOOL**
— NULL tz-info = current zone (GetDynamicTimeZoneInformation); applies Bias + Standard/DaylightBias
by DST-rule lookup for the GIVEN date (kernelbase/locale.c:7317-7355). Failure on bad SYSTEMTIME →
FALSE. Conformance tests/time.c:459-640 (fixed-bias zones with known DST transition dates).

### Time zone

**GetTimeZoneInformation(TIME_ZONE_INFORMATION*) → DWORD** — fills {Bias, StandardName/Date/Bias,
DaylightName/Date/Bias}; **returns TIME_ZONE_ID_UNKNOWN(0) / STANDARD(1) / DAYLIGHT(2)** — the
return value is DATA, not success (TIME_ZONE_ID_INVALID=0xFFFFFFFF on failure). Wine: forwards to
GetDynamicTimeZoneInformation and clamps the id (kernelbase/locale.c:6332-6352). Conformance
tests/time.c:236-368: `Bias + (id==DAYLIGHT ? DaylightBias : StandardBias)` must equal the
UTC−local delta computed via conversions — i.e. the tz answers must be **self-consistent with the
FileTimeToLocalFileTime family**.

### SYSTEM_INFO

**GetSystemInfo / GetNativeSystemInfo(SYSTEM_INFO*) → void** — kernelbase/memory.c:236/:207, both
filled by `fill_system_info` (memory.c:157-195) from NtQuerySystemInformation(SystemBasic/SystemCpu):
wProcessorArchitecture (9=AMD64), dwPageSize (4096), lpMinimum/MaximumApplicationAddress (64K /
0x7FFFFFFEFFFF on x64), dwActiveProcessorMask, dwNumberOfProcessors, **dwAllocationGranularity =
64K** (the field that already bit once — VirtualAlloc reservation sizing), dwProcessorType
(PROCESSOR_AMD_X8664=8664 for AMD64 arch), wProcessorLevel, wProcessorRevision. Never fails. The two
differ only under WOW64 (Native reports the real machine); identical for a native x64 process.

### Computer name

**GetComputerNameW(WCHAR*, DWORD* size) → BOOL** — kernel32/computername.c:41-49: forwards to
**GetComputerNameExW(ComputerNameNetBIOS,…)** (kernelbase/registry.c:3312-3350: reads
ActiveComputerName\ComputerName from the registry, falls back to gethostname-derived value†).
In/out size semantics: on success *size = chars written EXCLUDING the null; on too-small →
ERROR_BUFFER_OVERFLOW (Ex flavor: *size = required INCLUDING null†). NetBIOS names are ≤15 chars,
uppercase.

### Version — the LIE semantics

Two layers, deliberately different:
- **RtlGetVersion (ntdll/version.c:578-592) NEVER lies** — returns wine's configured version
  (default table entry WIN10 = 10.0.19045, ntdll/version.c:170-172; WIN11 = 10.0.22000 :174-176;
  overridable per-app/registry via version_init :470†).
- **GetVersionExW (kernelbase/version.c:1519-1546) lies to un-manifested apps**: first call runs
  `init_current_version` (version.c:174-259) which starts from RtlGetVersion then **downgrades to
  6.2 (Win8)** unless (a) the app manifest carries the supportedOS GUID for the running version
  (activation-context walk, :200-238), or (b) no OS-compat elements exist at all AND the PE optional
  header's MajorOperatingSystemVersion ≥ 6.3 — in which case: PE-OS-ver ≥ 10 → real version;
  6.3 ≤ ver < 10 → Win8.1 (:241-251). Size gate: dwOSVersionInfoSize must be exactly
  sizeof(OSVERSIONINFOW) or sizeof(OSVERSIONINFOEXW), else FALSE (:1523-1529).
- **GetVersion() (kernelbase/version.c:1462-1477)** — packed DWORD from the SAME lied version:
  `MAKELONG(MAKEWORD(major, minor), (platform^2)<<14)`, high word |= build for NT. 10.0.19045 →
  0x4A65000A.
- **What CP2077 sees**: CP2077 ships a manifest with Win10 supportedOS GUIDs ‡ (standard for
  DX12-era titles; unverified — extract from the exe's RT_MANIFEST to confirm). If so: real 10.0.x.
  If not: PE header OS-version ≥ 10 would still rescue it (VS2019 default is 10.0 †) — either path
  lands on 10.0.
- **VerifyVersionInfoW(OSVERSIONINFOEXW*, DWORD typeMask, DWORDLONG condMask) → BOOL** —
  kernel32/version.c:119-215: compares against **GetVersionExW's (lied) values**, field-by-field
  per 3-bit condition codes packed by VerSetConditionMask (`cond << (bit_index*3)`).
  major/minor/servicepack compare **hierarchically** (equal-so-far gates the next field †:163-199).
  Failure modes: `!typeMask || !condMask` → ERROR_BAD_ARGUMENTS + FALSE; mismatch →
  **ERROR_OLD_WIN_VERSION** + FALSE (tests/version.c:213 et passim — the last-error IS part of the
  contract). ntdll's RtlVerifyVersionInfo (ntdll/version.c:686-763) is the same algorithm against
  the UNLIED version (returns STATUS_REVISION_MISMATCH).

### IsProcessorFeaturePresent

**IsProcessorFeaturePresent(DWORD feature) → BOOL** — kernelbase/process.c:1046-1050 →
RtlIsProcessorFeaturePresent (ntdll/signal_x86_64.c:864-868):
`feature < PROCESSOR_FEATURE_MAX(64) && user_shared_data->ProcessorFeatures[feature]`.
**Out-of-range → FALSE.** x64 truth table as real Win10/Wine fills it (ntdll/unix/system.c, CPUID
sweep ~:545-600†):
| PF | # | x64 answer |
|---|---|---|
| FLOATING_POINT_PRECISION_ERRATA | 0 | FALSE (no Pentium-FDIV bug) |
| **FLOATING_POINT_EMULATED** | 1 | **FALSE** (an FPU exists — TRUE means soft-float!) |
| COMPARE_EXCHANGE_DOUBLE | 2 | TRUE |
| MMX | 3 | TRUE |
| XMMI (SSE) | 6 | TRUE |
| 3DNOW | 7 | FALSE (Intel; AMD legacy only) |
| RDTSC | 8 | TRUE |
| PAE | 9 | TRUE |
| XMMI64 (SSE2) | 10 | TRUE |
| NX | 12 | TRUE |
| SSE3 | 13 | TRUE |
| COMPARE_EXCHANGE128 | 14 | TRUE (cmpxchg16b) |
| COMPARE64_EXCHANGE128 | 15 | FALSE† |
| XSAVE_ENABLED | 17 | TRUE (modern CPUs) |
| SECOND_LEVEL_ADDRESS_TRANSLATION | 20 | host-dependent |
| VIRT_FIRMWARE_ENABLED | 21 | host-dependent |
| RDWRFSGSBASE | 22 | TRUE (modern) |
| FASTFAIL_AVAILABLE | 23 | TRUE |
| SSSE3 / SSE4_1 / SSE4_2 / AVX / AVX2 (36-40†) | | per-CPUID |
| all ARM_* features | | FALSE on x64 |

### Startup / command line / environment

**GetStartupInfoW(STARTUPINFOW*) → void** — copies from RTL_USER_PROCESS_PARAMETERS
(kernelbase/process.c:1382-1410); never fails; cb=sizeof(STARTUPINFOW)=104(x64); hStdInput/Output/
Error only meaningful when dwFlags & STARTF_USESTDHANDLES. **GetStartupInfoA** — kernel32/
kernel_main.c:82-107, returns a process-lifetime cached A-conversion (same struct every call).
**GetCommandLineA/W() → LPSTR/LPWSTR** — kernelbase/process.c:1364-1377: returns THE pointer into
process parameters (stable for process lifetime, caller must not free). Contract: contains
program name as argv[0] (conventionally quoted-if-spaces full path) + args.
**GetEnvironmentStringsW() → LPWSTR** — kernelbase/process.c:1573-1585: **a fresh heap COPY on
every call** (conformance environ.c:877-890: two calls return DIFFERENT pointers; broken() only on
NT≤5.1) of the double-null-terminated `VAR=value\0…\0\0` block. **FreeEnvironmentStringsW(LPWSTR)
→ BOOL** = HeapFree of that copy (process.c:1720-1723).
**GetEnvironmentVariableW(name, buf, size) → DWORD** — process.c:1693-1713: found+fits → len
(excl null), buf null-terminated; too-small → **len+1** (incl null), buf untouched; not found →
0 + ERROR_ENVVAR_NOT_FOUND; size==0 → len+1.
**ExpandEnvironmentStringsW(src, dst, len) → DWORD** — process.c:1520-1560: replaces %VAR%;
unknown %VAR% stays LITERAL; returns required length INCLUDING null (†:1559 returns
total_size/sizeof(WCHAR)); dst may be partially written on too-small.

### Power / app policy

**CallNtPowerInformation(level, in, inlen, out, outlen) → NTSTATUS** — powrprof.c:51-58 → straight
NtPowerInformation. **ProcessorInformation (11)**: out = PROCESSOR_POWER_INFORMATION[nproc]
{Number, MaxMhz, CurrentMhz, MhzLimit, MaxIdleState, CurrentIdleState}; wine unix side
(ntdll/unix/system.c:4680-4740) fills real MHz from cpufreq sysfs, canned 1000 MHz fallback;
NULL/0 out → STATUS_INVALID_PARAMETER, short buffer → STATUS_BUFFER_TOO_SMALL. Real Windows: same
shape, real MHz — a game will never see MaxMhz==0. Unsupported levels → STATUS_NOT_IMPLEMENTED†.
**GetSystemPowerStatus(SYSTEM_POWER_STATUS*) → BOOL** — kernel32/powermgnt.c:43-66: defaults are
the UNKNOWN sentinels (ACLineStatus=255, BatteryFlag=255, BatteryLifePercent=255,
**BatteryLifeTime=BatteryFullLifeTime=0xFFFFFFFF**), then NtPowerInformation(SystemBatteryState);
STATUS_NOT_IMPLEMENTED → TRUE with the unknowns; desktop-no-battery → BatteryFlag=128.
**AppPolicyGetThreadInitializationType(HANDLE token, enum*) → LONG** — kernelbase/main.c:113-121:
`*policy = AppPolicyThreadInitializationType_None(0)`, return ERROR_SUCCESS(0). That IS the real
desktop-process answer.

---

## 2. DIVERGENCE table

Shim column cites /tmp/alky-shims-lib.rs. "PASS" rows listed after the table.

| # | fn | real Windows (per wine) | ours | file:line (wine / shim) | severity |
|---|----|--------------------------|------|--------------------------|----------|
| 1 | GetSystemTime / GetLocalTime | UTC **now**; seconds advance; date real | **frozen 2025-01-01 12:00:00**, wMilliseconds = call-count%1000 (det_ticks(1)) — seconds NEVER advance, ms cycles per-call not per-time | file.c:4197 / shim:1504-1518 | **WRONG-DATA** — any elapsed-time or timestamp use gets a frozen clock |
| 2 | cross-API clock agreement | GetSystemTime ≡ GetSystemTimeAsFileTime ≡ Precise (tests/time.c:801-844 convert & compare) | GetSystemTimeAsFileTime/Precise = REAL epoch now (mono_epoch_100ns, shim:2207-2210, 1631-1634) but GetSystemTime = fixed 2025-01-01 → the two "now"s disagree by >1.5 years | file.c:4226 / shim:1504 vs 2207 | **TRUST-CHAIN** — code that reads both (CRT time_t vs FILETIME paths) sees contradictory reality; watchdogs comparing them misfire |
| 3 | SystemTimeToFileTime | full y/m/d civil conversion + field validation (ERROR_INVALID_PARAMETER/FALSE on bad); wDayOfWeek ignored | **ignores y/m/d entirely** — hardcoded 2025-01-01 base + time-of-day only; never validates, always TRUE | file.c:4296-4311, tests/time.c:163-235 / shim:1519-1531 | **WRONG-DATA** — converting any real date (save-file stamps, cert dates) yields 2025-01-01 garbage |
| 4 | FileTimeToSystemTime | full inverse; ft<0 → ERROR_INVALID_PARAMETER + FALSE | decodes time-of-day from fixed 2025 base; date fields always 2025-01-01(Wed); hour wraps %24 so real-epoch inputs (from our own GetSystemTimeAsFileTime!) decode to nonsense; no neg check | file.c:4158-4180 / shim:1532-1544 | **WRONG-DATA / TRUST-CHAIN** — round-trip GetSystemTimeAsFileTime→FileTimeToSystemTime is incoherent (row-2 skew squared) |
| 5 | VerifyVersionInfoW/A | condition-mask evaluation vs (lied) version; FALSE + ERROR_OLD_WIN_VERSION on mismatch; FALSE + ERROR_BAD_ARGUMENTS on empty masks | **constant TRUE** for every query | kernel32/version.c:119-215 / shim:1355 | **TRUST-CHAIN** — any *negative* check ("is OS **older** than X" for a legacy fallback, or an equality pin) silently takes the wrong branch. CP2077's own ≥Win10 gates happen to pass correctly ‡ |
| 6 | IsProcessorFeaturePresent | per-feature truth table; **feature≥64 → FALSE**; PF 0/1/7 FALSE on x64 (§SPEC table) | **constant TRUE** for every feature | process.c:1046, signal_x86_64.c:864-868 / shim:2206 | **TRUST-CHAIN** — TRUE for PF_FLOATING_POINT_EMULATED(1) declares "no FPU" (CRT soft-float paths ‡); TRUE for 3DNOW/ARM/out-of-range features can enable dead code paths. SSE/AVX answers coincide with truth |
| 7 | CallNtPowerInformation | level-specific fills; ProcessorInformation → real per-CPU MHz (canned 1000 floor); bad args → STATUS_INVALID_PARAMETER / _BUFFER_TOO_SMALL / _NOT_IMPLEMENTED | zero the out-buffer, STATUS_SUCCESS for **every** level | powrprof.c:51, unix/system.c:4680-4740 / shim:1334-1340 | **WRONG-DATA** — MaxMhz=CurrentMhz=0; a "CPU speed" HUD/telemetry divides by 0 MHz ‡; unsupported levels fake-succeed |
| 8 | GetSystemPowerStatus | unknown sentinels: BatteryLifeTime/FullLifeTime = **0xFFFFFFFF**; ACLineStatus 0/1/255 | ACLineStatus=1, BatteryFlag=128, Percent=255 ✓ but LifeTime fields zeroed → "0 seconds of battery left" | powrprof: kernel32/powermgnt.c:43-66 / shim:1341-1348 | WRONG-DATA (minor) — two u32 writes fix it |
| 9 | GetEnvironmentStringsW | fresh heap copy per call (environ.c:877-890: pointers differ); block populated (SystemRoot, PATH, USERPROFILE…) | one static **empty** `\0\0` block, same pointer every call; FreeEnvironmentStringsW no-op TRUE | process.c:1573 / shim:2496-2497, env_strings_w :842-846 | WRONG-DATA (deliberate empty-env design) — same-pointer + no-op-free are internally consistent; emptiness is the real divergence: CRT getenv, TMP/TEMP, SteamAppId ‡ all absent |
| 10 | GetEnvironmentVariableW | per-var lookup w/ len+1 too-small protocol | constant 0 + ERROR_ENVVAR_NOT_FOUND(203) | process.c:1693-1713 / shim:2498 | consistent-with-#9; truthful-negative for the empty env. Becomes WRONG the day env is populated — the buffer protocol (len+1) is unimplemented |
| 11 | GetCommandLineW/A | stable pointer; argv[0] = (conventionally quoted) full exe path | `"Cyberpunk2077.exe --launcher-skip"` — bare name, unquoted, no path (ALKY_CMDLINE overridable) | process.c:1364-1377 / shim:2305-2306, cmdline_w :818-830 | BENIGN-leaning WRONG-DATA — exe-dir-from-argv[0] consumers get "" (GetFullPathNameW shim's C:\game\ cwd compensates ‡) |
| 12 | GetSystemInfo / GetNativeSystemInfo | real core count / affinity mask / CPU level+revision | hardcoded: 8 cores, mask 0xFF, level 6, rev 0; arch/pagesize/granularity/addr-range correct (incl. **dwAllocationGranularity=64K — the prior bug's fix**, shim comment :2396-2399) | memory.c:157-195 / shim:2393-2412 | BENIGN→WRONG-DATA — job-system sizes to 8 workers regardless of host; correctness preserved, perf/affinity fidelity lost |
| 13 | GetTickCount / GetTickCount64 (DET mode) | ms since boot, rate = wall clock | ALKY_DET_CLOCK=1: **all three clock families share ONE counter** (DET_CLOCK, shim:60): GetTickCount advances it 1000/call (mono_ns det arm :73-78), QPC 1000/call, system-time 10000/call (mono_epoch_100ns :67-72). Effective rates: QPC claims 1µs/call @1GHz; FILETIME claims 1ms/call; ticks claim 1µs→ms÷1e6 ≈0/call. Cross-talk: every QPC call advances GetTickCount's clock too | sync.c:181-203 / shim:60-78, 1630 | **TRUST-CHAIN (det-mode only)** — QPC vs FILETIME rate disagreement = 1000×; a watchdog measuring "budget in QPC ticks" vs "deadline in FILETIME" misfires exactly like the EngineWatchdog incident the mono_ns comment records (shim:61-64). Real-time mode (default) is coherent: all from CLOCK_MONOTONIC/REALTIME |
| 14 | GetVersion | packed lied version | shim:2149 `0x47BB000A` (10.0.18363) — fine; **but a dead duplicate arm at :2392 (`0x0000000A` = 10.0 build 0) is unreachable** (first match wins) | version.c:1462-1477 / shim:2149, 2392 | BENIGN (code-health: delete :2392; if arm order ever changes, build 0 would leak out) |
| 15 | GetVersionExW / RtlGetVersion | the version-LIE machinery (§SPEC); games sniff build numbers here | **no arm** → GPA returns 0 truthful-negative (shim:2189-2194); IAT call would fall through to die-loud (`_ =>` shim:3205) | version.c:1519 / — | LATENT-BLOCKER — not in touched list; CP2077 uses VerifyVersionInfo instead ‡. If ever hit: implement the honest 10.0.18363, do NOT model the lie (we control the "manifest") |
| 16 | GetTimeZoneInformation, SystemTimeToTzSpecificLocalTime, GetComputerNameW/ExW, ExpandEnvironmentStringsW | §SPEC | **no arms** — same die-loud fate | locale.c:6332, :7317, computername.c:41, process.c:1520 / — | LATENT — untouched by CP2077. If added: tz = UTC-zero-bias constant (fully-correct for a UTC world, matches row-PASS FileTimeToLocalFileTime); computer name = any ≤15-char constant; ExpandEnvironmentStrings = copy-through (empty env → every %VAR% stays literal, which IS the real contract for unknown vars) |

**PASS rows** (constant/simple answers that are the honest real-Windows answer):
- **GetTickCount/GetTickCount64 (real mode)** = mono_ns()/1e6 (shim:1630): CLOCK_MONOTONIC ms — same
  clock class Wine itself uses (CLOCK_BOOTTIME/MONOTONIC, unix/sync.c:89-108). Monotone, shared,
  boot-relative. PASS. (64-bit-in-32-bit-return truncation for GetTickCount = the real wrap
  behavior. ✓)
- **QueryPerformanceCounter** (shim:2211-2215) = mono_ns, TRUE; **QueryPerformanceFrequency**
  (shim:2216) = constant 1e9, TRUE. The QPC contract is (freq=anything, stable; counter/freq =
  seconds; monotonic) — ns@1GHz satisfies it exactly. Differs from Wine's 10MHz *value* but not
  from the *contract*; only code hardcoding 10MHz (never legal) would care. PASS.
- **GetSystemTimeAsFileTime / GetSystemTimePreciseAsFileTime** (shim:2207-2210, 1631-1634) =
  116444736000000000 + CLOCK_REALTIME/100 — the textbook epoch conversion; precise==coarse source →
  the ≥ + <1s test contract holds trivially. PASS (in real mode; see row 13 for det).
- **FileTimeToLocalFileTime** copy-through (shim:1623-1627) = correct for a UTC/zero-bias world;
  self-consistent with the (absent→future) tz story. PASS-with-note: also aliases
  LocalFileTimeToFileTime — both identity, direction distinction lost = fine at bias 0.
- **GetStartupInfoW/A** zero-fill + cb=104 (shim:2307-2310): dwFlags=0 → all optional fields
  legitimately ignorable. Real Wine copies real process params, but a zeroed STARTUPINFO is a
  valid one. PASS.
- **AppPolicyGetThreadInitializationType** (shim:2131-2136): *policy=0 + ERROR_SUCCESS — byte-for-byte
  the Wine/real desktop answer (main.c:113-121). PASS.
- **VerSetConditionMask** (shim:1349-1354): correct shift-packing algorithm. PASS.

**Divergence count: 13** (rows 1-13 excluding PASS; rows 14 = code-health, 15-16 = latent-absent,
counted: 1,2,3,4,5,6,7,8,9,10,11,12,13 → 13 substantive rows; 14-16 noted but 14 is benign-dead-code
and 15/16 are unreached — headline N = 13).

---

## 3. RET-0 GRADING (constant-return arms, sera's ·1599 ruling)

| arm (shim:line) | constant | grade | honest fix |
|---|---|---|---|
| VerifyVersionInfoW/A :1355 | ret=1 | **fake-success** | implement-real (~30 ln): evaluate the 3-bit condition codes against the canonical 10.0.18363 exactly like kernel32/version.c:119-215; set ERROR_OLD_WIN_VERSION / ERROR_BAD_ARGUMENTS on the FALSE paths. All inputs already in registers/stack; VerSetConditionMask arm (:1349) already computes the mask format |
| IsProcessorFeaturePresent :2206 | ret=1 | **fake-success** | implement-real (~10 ln): `const PF_X64: u64 = bitmask` per §SPEC table; `ret = (rcx < 64 && PF_X64>>rcx & 1)`. Zero risk — every bit is knowable for our x64 target |
| GetSystemTime/GetLocalTime :1504 | frozen 2025-01-01 | **fake-data** | implement-real (~20 ln): decompose `mono_epoch_100ns()` via days-from-civil inverse (or reuse a Rust `time` crate-free algorithm); kills divergence rows 1+2. If determinism of the *date* is wanted, freeze the epoch base in det-mode only |
| SystemTimeToFileTime :1519 / FileTimeToSystemTime :1532 | 2025-base approximation | **fake-data** | implement-real: full civil↔FILETIME both ways + neg/QuadPart validation + field validation (ERROR_INVALID_PARAMETER). ~40 ln shared helper; makes conversions inverse-consistent with the (fixed) GetSystemTime |
| CallNtPowerInformation :1334 | zero-fill + 0 | **fake-success** | level-gate it: ProcessorInformation → fill {Number:i, MaxMhz:3000, CurrentMhz:3000, MhzLimit:3000, MaxIdleState:0, CurrentIdleState:0}×nproc(8, match GetSystemInfo); other levels → STATUS_NOT_IMPLEMENTED (0xC0000002) die-loud-ish (truthful) |
| GetSystemPowerStatus :1341 | partial struct | near-correct | write BatteryLifeTime=BatteryFullLifeTime=0xFFFFFFFF (offsets +4,+8); rest already honest |
| GetVersion :2149 | 0x47BB000A | **fully-correct-constant** (real Windows is also a constant per process) — delete dead twin :2392 |
| QueryPerformanceFrequency :2216 | 1e9 | **fully-correct-constant** (stable freq is the whole contract) |
| GetEnvironmentVariableW :2498 | 0 + err 203 | **fully-correct-constant** *for the empty-env design*; if env gets populated, must grow the len+1 protocol |
| FreeEnvironmentStringsW :2497 | ret=1 | **fully-correct-constant** (paired with static block; becomes a leak-avoidance no-op even with real copies) |
| GetStartupInfoW/A :2307 | zeroed+cb | **fully-correct-constant** |
| AppPolicyGetThreadInitializationType :2131 | 0 + ERROR_SUCCESS | **fully-correct-constant** (matches Wine exactly) |
| FileTimeToLocalFileTime :1623 | copy, ret=1 | **fully-correct-constant** at tz-bias 0 |
| GetSystemInfo :2393 | hardcoded 8-core fill | fully-correct-constant *in shape* (granularity/pagesize/arch/addr are the load-bearing truths and are right); core-count fidelity = optional improvement (read host nproc, cap mask accordingly) |

Priority census suspects (VirtualQuery, GetFileInformationByHandleEx, LCMapString/GetStringType,
GetLocaleInfoEx/W, SuspendThread, GetFileVersionInfo*/VerQueryValue): none in this family; noted
that LCMapStringW/GetStringTypeW ret=0 sits adjacent at shim:2495 — other wave's row.

### DET_CLOCK interaction risk (assignment call-out, consolidated)
The shim's three time sources are coherent in real mode (all CLOCK_MONOTONIC/REALTIME) but in
`ALKY_DET_CLOCK=1` they share one per-call counter at THREE different implied rates (shim:60-78):
QPC 1000 units/call (=1µs @ its claimed 1GHz), FILETIME 10000 units/call (=1ms), tick-ms ~1µs/call.
Consequences: (a) any watchdog spanning two families sees up to 1000× rate skew — the exact class
the mono_ns comment documents (EngineWatchdog computed ~300s at t=0.24s and asserted, shim:61-64);
(b) calls to ONE clock advance the OTHERS (shared counter) — call-frequency becomes time, so a
busy-poll loop fast-forwards the world; (c) GetSystemTime (row 1) doesn't participate at all
(frozen), adding a third reality. Fix sketch: per-family det counters with ONE shared rate
(units: 100ns), stepped only by a designated tick source (e.g. frame boundary), derived reads for
all families — det timelines stay bit-identical AND mutually consistent.
