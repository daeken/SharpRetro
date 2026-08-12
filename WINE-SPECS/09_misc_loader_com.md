# 09_MISC — loader / COM / shell-paths / error / pointer-crypt / winsock / remainder

Family 09 of the wine-spec wave (WAVE-CONTRACT.md). Breadth-first: every remainder
function gets a SPEC row + RET-0 grade; depth on the trust-chain set (EncodePointer
round-trip, BSTR layout, SHGet* paths, CloseHandle, RtlPcToFileHeader, last-error
threading, LoadLibrary/GetModuleHandle consistency).

Sources: wine master @ /local/home/seratb/wine; shim @ /tmp/alky-shims-lib.rs (3211 ln).
† = source-only claim (no live Windows run). ‡ = inference.

---

## 1. SPEC (real-Windows contract as Wine encodes it)

### 1a. Loader family

**LoadLibraryA/W** — thin wrappers over LoadLibraryExA/W(name, 0, 0)
(kernelbase/loader.c:525-539). **LoadLibraryExW** (loader.c:562-582): `!name` →
`SetLastError(ERROR_INVALID_PARAMETER)`, return NULL. Trailing spaces are trimmed
before lookup (loader.c:574-578). Success = HMODULE (the mapped base, refcount++);
failure = NULL + last-error (ERROR_MOD_NOT_FOUND=126 for absent DLLs —
kernel32/tests/module.c:337 asserts exactly ERROR_MOD_NOT_FOUND after a failed load).
Datafile/image-resource flags return the base with low bits 1/2 set.

**FreeLibrary** (loader.c:232-268): `!module` → ERROR_INVALID_HANDLE + FALSE.
Low-bits-tagged datafile modules take the UnmapViewOfFile path; real modules →
`LdrUnloadDll` (refcount--, DllMain(DLL_PROCESS_DETACH) at 0). Returns BOOL.

**DisableThreadLibraryCalls** (loader.c:223-226): `LdrDisableThreadCalloutsForDll`;
success TRUE; fails (ERROR_INVALID_PARAMETER †) for datafile handles. Side-effect:
suppresses DLL_THREAD_ATTACH/DETACH callouts for that module only.

**GetModuleHandleW/A** (loader.c:345-380): delegates to
`GetModuleHandleExW(GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT, name, &ret)`;
NULL name = the exe base. **A module that is not loaded returns NULL** and
last-error ERROR_MOD_NOT_FOUND (tests/module.c:337,355 — the conformance truth).
No refcount change. Games use this as the "is X.dll present?" probe.

**GetModuleHandleExW** (loader.c:386-430): `!module` out-ptr → ERROR_INVALID_PARAMETER
+ FALSE. Invalid flag combos (PIN|UNCHANGED_REFCOUNT together, unknown bits) →
*module=NULL, ERROR_INVALID_PARAMETER, FALSE. FROM_ADDRESS flag resolves via
RtlPcToFileHeader; name=NULL → exe base. Absent module → FALSE (STATUS_DLL_NOT_FOUND
→ ERROR_MOD_NOT_FOUND). Success writes *module and refcounts unless UNCHANGED.

**GetModuleFileNameW** (loader.c:300-330): fills caller buffer via LdrGetDllFullName.
Returns #chars copied (excl. NUL). On truncation: buffer filled to size, `filename[size-1]=0`,
returns `size`, and last-error = ERROR_INSUFFICIENT_BUFFER (via
RtlNtStatusToDosError(STATUS_BUFFER_TOO_SMALL), loader.c:322-327). On success
last-error is set to ERROR_SUCCESS (same line — observable!). **GetModuleFileNameA**
(loader.c:274-296): W then WtoA; truncation sets ERROR_INSUFFICIENT_BUFFER.

**GetProcAddress** (loader.c:498-501 → get_proc_address at loader.c:66-80):
name or LOWORD ordinal → LdrGetProcedureAddress; failure = NULL +
ERROR_PROC_NOT_FOUND †. x86_64 has an asm wrapper preserving xmm0-3 (Delphi
delay-load bug workaround, loader.c:446-452) — a register-preservation guarantee
callers may silently rely on.

**RtlPcToFileHeader** (ntdll/loader.c:4681-4691): walks the loader module list under
loader_section lock; pc inside a mapped module → returns DllBase and writes it to
*address; else returns NULL **and writes NULL to *address** (out-param always
written). This is the module lookup SEH dispatch and CRT unwind use.

### 1b. Exceptions / error state

**AddVectoredExceptionHandler** (ntdll/exception.c:414-416 → add_vectored_handler:102-115):
allocates a node, `handler->func = RtlEncodePointer(func)` (exception.c:108 — VEH
pointers are encode-protected on real Windows too), inserts head (first=TRUE) or tail.
Returns the node pointer as an opaque non-NULL handle; NULL only on OOM. Handlers run
during exception dispatch before SEH frames, receiving EXCEPTION_POINTERS; return
EXCEPTION_CONTINUE_EXECUTION to swallow.

**RaiseException** (kernelbase/debug.c:390-407): builds EXCEPTION_RECORD{code,
flags&EXCEPTION_NONCONTINUABLE, ExceptionAddress=RaiseException, ≤15 params copied}
→ RtlRaiseException → full SEH dispatch (VEH → frame handlers → unhandled filter).
Notably flags are masked to NONCONTINUABLE only.

**CloseHandle** (kernelbase/process.c:421-431): maps pseudo std-handle constants to
PEB slots, then `set_ntstatus(NtClose(h))`. NtClose (ntdll/unix/server.c:1945-1985):
pseudo-handles (~0..~5) → STATUS_SUCCESS; invalid handle → STATUS_INVALID_HANDLE
(→ FALSE + ERROR_INVALID_HANDLE), **and if the process is being debugged it raises
EXCEPTION_INVALID_HANDLE via the user exception dispatcher** (server.c:1976-1984).
tests/loader.c:3442,3669 assert the ERROR_INVALID_HANDLE path. Trust-chain: handle
hygiene checkers double-close on purpose and branch on FALSE/raise.

**GetLastError / SetLastError** (kernelbase/thread.c:231-233 †):
`NtCurrentTeb()->LastErrorValue` — **strictly per-thread** state in the TEB. Every
API's documented last-error side-effect lands on the calling thread only.

**SetErrorMode** (kernelbase/process.c:1185-1193): per-process hard-error mode via
NtSetInformationProcess; returns the *previous* mode. **GetErrorMode**
(process.c:786-794): reads it. **SetUnhandledExceptionFilter**: returns the previous
filter pointer (NULL if none) and installs the new one; the filter is invoked at
end of dispatch for unhandled exceptions †.

**SetHandleCount** (kernelbase/process.c:1198-1200): returns the argument unchanged.
Win16 relic — a pure identity function on real Windows too.

### 1c. Pointer encoding

**EncodePointer/DecodePointer** (ntdll/rtl.c:1042-1063): per-process cookie
(NtQueryInformationProcess(ProcessCookie) †), `rotate = cookie % 64`;
encode = `ror64(ptr ^ cookie, rotate)`; decode = exact inverse. Contract the process
observes: (1) **decode(encode(p)) == p for all p** — round-trip is the load-bearing
property (CRT security-cookie/atexit/VEH tables store encoded pointers);
(2) encode(NULL) != NULL (obfuscated); (3) stable across the whole process lifetime;
(4) NOT stable across processes. No last-error side effects.

### 1d. COM (combase)

**CoInitializeEx** (combase/combase.c:2975-3010 → enter_apartment,
combase/apartment.c:1129-1151): first init on a thread → **S_OK**; repeat init with
the *same* model → **S_FALSE** (apartment.c:1146; still increments the init count —
each success needs a matching CoUninitialize); different model → **RPC_E_CHANGED_MODE**
(apartment.c:1143, init count NOT incremented). Games branch on all three
(`hr == RPC_E_CHANGED_MODE` is the classic "someone already made this thread STA" probe).

**CoUninitialize** (combase.c:3014-3060): unbalanced call → ERR + no-op (init count 0);
otherwise decrements; last one tears the apartment down. Returns void.

**CoCreateInstance** (combase.c:1695-1710): `!obj` → **E_POINTER**; delegates to
CoCreateInstanceEx with one MULTI_QI, `*obj = multi_qi.pItf`. Down the path
(com_get_class_object, combase.c:1725-1740): no apartment on the thread →
**CO_E_NOTINITIALIZED** ("apartment not initialised"); registered nowhere →
**REGDB_E_CLASSNOTREG** (0x80040154); success fills *obj, S_OK. CoCreateInstanceEx
fills each MULTI_QI{pItf, hr} individually.

**CoInitializeSecurity** (combase.c:1115+): first call S_OK; second call
RPC_E_TOO_LATE †. **CoTaskMemAlloc/CoTaskMemFree** (combase/malloc.c:381-393): the
COM allocator (IMalloc); Free(NULL) is a no-op; SHGetKnownFolderPath's out-string
must be freeable by CoTaskMemFree (allocator identity matters on real Windows).

### 1e. OLEAUT32 BSTR (imported by ordinal: #2/#4/#6/#7)

Layout (oleaut32/oleaut.c:86-97, bstr_t): allocation = [4B pad (Win64)] [DWORD size
in **bytes**] [WCHAR data...] [NUL]. The BSTR value points at the *data*; the byte
length lives at **ptr-4**; data is always NUL-terminated. **SysAllocString** [#2]
(oleaut.c:226-232): NULL in → NULL out (not an error); else SysAllocStringLen(str,
lstrlenW) — length in *UTF-16 code units*. **SysAllocStringLen** [#4]
(oleaut.c:341-366): overflow-guarded; str=NULL → zero-filled buffer of len chars.
**SysFreeString** [#6] (oleaut.c:273+): NULL-tolerant; frees or caches.
**SysStringLen** [#7] (oleaut.c:198-201): `str ? *(DWORD*)(str-4)/2 : 0`. The
prefix/2 contract is what marshalers and games read directly.

### 1f. Shell paths

**SHGetKnownFolderPath** (shell32/shellpath.c:3561-3611): unknown KNOWNFOLDERID →
HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND); bad flag bits → E_INVALIDARG; success →
`*ret_path = CoTaskMemAlloc(...)` copy of the per-folder path, S_OK. *ret_path is
NULLed first (3570). Distinct GUIDs give distinct paths (Documents ≠ LocalAppData ≠
SavedGames): games write saves under FOLDERID_SavedGames/Documents and settings
under LocalAppData — path identity is the save-integrity contract.
**SHGetFolderPathW** (shellpath.c:2871-2882): CSIDL-keyed same machinery;
ERROR_PATH_NOT_FOUND is folded to ERROR_FILE_NOT_FOUND. **SHGetSpecialFolderPathW**
(shellpath.c:3406-3414): BOOL wrapper — TRUE iff SHGetFolderPathW == S_OK (invalid
CSIDL → FALSE).

### 1g. user32 / secur32 / misc

**RegisterClipboardFormatW** (user32/clipboard.c:528-536): NtUserRegisterWindowMessage
— atom in 0xC000-0xFFFF; **same name → same atom, process-global, repeatable**
(that's the whole point: two components rendezvous by name). 0 on failure.
**InitSecurityInterfaceW** (secur32/secur32.c:169-172): returns a static
SecurityFunctionTableW — never fails on real Windows. **DisableProcessWindowsGhosting**
(user32/misc.c:390): void; sets a per-process flag; no observable return.
**GetStdHandle** (kernelbase/process.c:1418-1429): PEB ProcessParameters
hStdInput/Output/Error; invalid index → INVALID_HANDLE_VALUE + ERROR_INVALID_HANDLE.
**LocalFree** (kernelbase/memory.c:1106-1136): success → NULL; invalid handle →
returns the handle + ERROR_INVALID_HANDLE.

### 1h. Directories / registry / rand

**GetSystemDirectoryW/A** (kernelbase/file.c:2360-2378, dirs at file.c:67-68:
`C:\windows\system32`, `C:\windows`) and **GetWindowsDirectoryA** (file.c:2624+),
all via copy_filename (file.c:376-390): buffer big enough → copy+NUL, return len
(excl. NUL); **too small → return len+1 (required size INCL. NUL), buffer untouched** †.
**RegOpenKeyExW** (kernelbase/registry.c:644+): missing key → **ERROR_FILE_NOT_FOUND
(2)** — the not-found exactness the goal asks about; access denied → ERROR_ACCESS_DENIED;
success → ERROR_SUCCESS + *retkey. **BCryptGenRandom** (bcrypt/bcrypt_main.c:571-600):
handle=NULL requires BCRYPT_USE_SYSTEM_PREFERRED_RNG else STATUS_INVALID_HANDLE;
buffer=NULL → STATUS_INVALID_PARAMETER; unknown flags → STATUS_INVALID_PARAMETER †;
success STATUS_SUCCESS with **cryptographic-quality** bytes (system RNG). Quality is
the contract: session keys, GUIDs, telemetry ids.

### 1i. Winsock (WS2_32, ordinal imports) + SList

Ordinal map (ws2_32/ws2_32.spec): #2 bind, #3 closesocket, #4 connect, #10
ioctlsocket, #16 recv, #18 select, #19 send, #23 socket, #57 gethostname,
#111 WSAGetLastError, #115 WSAStartup, #116 WSACleanup.
**WSAStartup** (ws2_32/socket.c:743-773): version-negotiates (LOBYTE 0 →
WSAVERNOTSUPPORTED; 1.x → 1.1; ≥2.2 caps at 2.2), fills WSADATA
{wVersion, wHighVersion=2.2, "WinSock 2.0", "Running"}, data=NULL → WSAEFAULT,
success 0. **WSACleanup** (socket.c:777-797): balanced count; un-inited →
SOCKET_ERROR + WSANOTINITIALISED. **WSAGetLastError** (socket.c:801+): =GetLastError
— per-thread. **socket/connect/send/recv** fail with SOCKET_ERROR/INVALID_SOCKET +
per-thread WSA error when the network layer refuses; **select** returns #ready
(0 = timeout). **gethostname** (#57) succeeds even with no network †.
**getaddrinfo**: resolver failure → **WSAHOST_NOT_FOUND (11001)** returned directly
(not via last-error), *res untouched/NULL.
**InitializeSListHead** (ntdll/sync.c:1010-1018): zeroes Alignment+Region and sets
**Header16.HeaderType=1** on Win64 (the 16-byte-header discriminator the interlocked
push/pop family reads back, sync.c:1077/1108).

### 1j. Remainder constants (breadth rows)

- **GetStartupInfoW/A** (kernelbase/process.c:1382+): fills from RTL_USER_PROCESS_PARAMETERS; cb=sizeof.
- **AreFileApisANSI**: TRUE by default †.
- **AppPolicyGet\***: classic desktop process → enum 0 + ERROR_SUCCESS †.
- **GetVersion**: packed major|minor|build; 10.0.x for a modern manifest †.
- **SetThreadErrorMode**: sets per-thread mode, old via out-ptr, BOOL †.
- **PathFileExistsW**: BOOL of GetFileAttributes != INVALID †.
- **CallNtPowerInformation(SystemPowerInformation …)**: STATUS_SUCCESS + zeroed struct is the honest idle answer †.
- **Steam/GOG/Streamline/XeSS vendor surfaces**: not Windows APIs — out of Wine's scope; the honest contract is "runtime absent → init returns its documented failure and the game takes the no-vendor path" (see §2 PASS rows).

---

## 2. DIVERGENCE table (spec vs /tmp/alky-shims-lib.rs)

| # | fn | real Windows | ours | shim cite | wine cite | severity |
|---|----|--------------|------|-----------|-----------|----------|
| 1 | GetLastError/SetLastError | per-**thread** TEB LastErrorValue | **process-global** `Mutex<u64> LAST_ERROR` — threads bleed error codes into each other | lib.rs:102,468,2274-2275 | thread.c:231-233 | **TRUST-CHAIN** — a worker thread's failing call can overwrite the game thread's expected error between its API call and its GetLastError read; also feeds WSAGetLastError-style patterns |
| 2 | LoadLibraryA/W/ExW | absent DLL → NULL + ERROR_MOD_NOT_FOUND | vendor-driver list → honest NULL+126; **everything else → fake non-NULL hash handle** (0x5_0000_0000+h) | lib.rs:2276-2305 | loader.c:562-582; tests/module.c:337 | **TRUST-CHAIN** — "handle != NULL" is a trust decision; mitigated by GetProcAddress returning NULL per-fn (CRT fallback path), but any caller that treats load-success as feature-present without GPA is lied to |
| 3 | GetModuleHandleW/A | not-loaded name → NULL + ERROR_MOD_NOT_FOUND | **always 0x140000000 (exe base) for ANY name** | lib.rs:2181-2184 | loader.c:345-380; tests/module.c:337 | **TRUST-CHAIN** — "is overlay/anti-cheat/nvapi loaded?" probes always answer yes-with-exe-base; also **inconsistent with row 2**: LoadLibrary("nvapi64") fails but GetModuleHandle("nvapi64") succeeds — an impossible state on real Windows |
| 4 | GetModuleHandleExW | flag/param validation, FALSE+NULL on absent | writes exe base, returns TRUE unconditionally | lib.rs:2375-2377 | loader.c:386-430 | WRONG-DATA (same class as #3) |
| 5 | GetModuleFileNameW/A | copied-len return; truncation → filename[size-1]=0 + ERROR_INSUFFICIENT_BUFFER; success sets ERROR_SUCCESS | fixed `C:\Cyberpunk2077.exe`; returns min(size,21)-1; **no last-error writes, no truncation signal** | lib.rs:2363-2374 | loader.c:274-330 | WRONG-DATA (benign at len 21; the loop-until-fits pattern never sees INSUFFICIENT_BUFFER but also never needs to) |
| 6 | GetProcAddress | miss → NULL + ERROR_PROC_NOT_FOUND | miss → NULL, **last-error untouched** | lib.rs:2185-2194 | loader.c:66-80 | BENIGN |
| 7 | FreeLibrary | NULL → FALSE+ERROR_INVALID_HANDLE; refcount/unload | always TRUE, never unloads | lib.rs:1006 | loader.c:232-268 | BENIGN (never-unload regime) |
| 8 | DisableThreadLibraryCalls | can fail for datafile modules | always TRUE | lib.rs:1007 | loader.c:223-226 | PASS — we have no thread callouts at all; TRUE is the honest answer |
| 9 | RtlPcToFileHeader | loader-list walk; miss → NULL, *address always written | MODULE_BASE_HOOK image-range resolve; miss → 0; **out-ptr written both ways** | lib.rs:2144-2148 | ntdll/loader.c:4681-4691 | **PASS** (real implementation via hook; same NULL-miss + always-write shape) |
| 10 | AddVectoredExceptionHandler | handler invoked on every exception before SEH | **registered + tracked** (First→front honored, opaque handle) but only invoked once SEH dispatch wires VEH_HANDLERS in | lib.rs:1492-1502, 425 | exception.c:102-115,414 | WRONG-DATA (registration real; invocation pending — crash-handler paths silent until then) |
| 11 | RemoveVectoredExceptionHandler | removes; returns nonzero on success | returns 1, **does not remove** | lib.rs:1503 | exception.c † | BENIGN (over-invocation only matters once #10 fires) |
| 12 | RaiseException | full SEH dispatch; unhandled → filter → terminate | 0x406D1388 thread-name no-op (correct); else SEH_DISPATCH_HOOK or **die-loud exit(133)** | lib.rs:1468-1491 | debug.c:390-407 | BLOCKER-by-design — honest die-loud, not a silent swallow; correct until real throws appear |
| 13 | CloseHandle | invalid → FALSE+ERROR_INVALID_HANDLE, raises EXCEPTION_INVALID_HANDLE under debugger; frees slot | **always TRUE**, no table cleanup, never raises | lib.rs:2606 | process.c:421-431; server.c:1945-1985; tests/loader.c:3442 | WRONG-DATA — handle-hygiene/anti-debug probes (double-close, close-invalid) always see success; leak is deliberate (boot-safe) |
| 14 | GetStdHandle | PEB ProcessParameters handles; invalid index → INVALID_HANDLE_VALUE+error | `0x10 + rcx` fake distinct non-null | lib.rs:2380 | process.c:1418-1429 | BENIGN (nothing consumes them for real I/O here) |
| 15 | EncodePointer/DecodePointer | cookie-XOR + rotate; decode∘encode = id | **identity both ways — round-trip HOLDS** | lib.rs:2412 | rtl.c:1042-1063 | **PASS** on the load-bearing contract (decode(encode(p))==p; process-lifetime stable). Residual: encode(p)==p is observably un-scrambled — only an anti-tamper that *checks* scrambling would notice ‡ |
| 16 | CoInitializeEx | S_OK / **S_FALSE** (re-init) / **RPC_E_CHANGED_MODE** (model flip) | always S_OK | lib.rs:1222 | combase.c:2975+; apartment.c:1129-1151 | WRONG-DATA — `SUCCEEDED()` callers fine; a caller counting on S_FALSE-means-already-init or probing STA-vs-MTA via CHANGED_MODE misreads. No apartment state exists to flip |
| 17 | CoUninitialize | balanced-count teardown | no-op | lib.rs:1223 | combase.c:3014+ | PASS |
| 18 | CoInitializeSecurity | 1st S_OK, 2nd RPC_E_TOO_LATE | always S_OK | lib.rs:1224 | combase.c:1115 | BENIGN |
| 19 | CoCreateInstance | E_POINTER / CO_E_NOTINITIALIZED / REGDB_E_CLASSNOTREG / S_OK+obj | *ppv=NULL + REGDB_E_CLASSNOTREG always | lib.rs:1236-1240 | combase.c:1695-1740 | **PASS** — honest-negative; consistent with #16 (COM "initialized", class genuinely unregistered offline). CP2077 uses COM for WIC/media — takes fallbacks |
| 20 | CoCreateInstanceEx | per-MULTI_QI {pItf,hr} fills | fills pItf=NULL, hr=0x80040154 each, returns same | lib.rs:1225-1235 | combase.c † | PASS (same shape) |
| 21 | CoCreateGuid | crypto-random v4 UUID | deterministic det_ticks-derived | lib.rs:1241-1247 | — | WRONG-DATA (uniqueness across runs/machines not guaranteed; offline-benign) |
| 22 | CoTaskMemAlloc/Free | IMalloc heap; Free actually frees | bump-alloc; Free leaks | lib.rs:1419-1420 | malloc.c:381-393 | BENIGN (allocator identity preserved for SHGetKnownFolderPath returns — same bump heap) |
| 23 | SysAllocString (#2) | len = lstrlenW = **UTF-16 units** | prefix+alloc sized from `chars().count()` (scalar values) but writes `encode_utf16()` units — **non-BMP input overruns the alloc by 2B/astral char and understates the prefix** | lib.rs:1288-1302 | oleaut.c:226-232,341-366 | WRONG-DATA (latent BLOCKER on astral input; BMP-only in practice ‡). Layout itself (len@ptr-4, NUL) is correct |
| 24 | SysAllocStringLen (#4) | caller-supplied unit count | correct: prefix=len*2, copy, NUL | lib.rs:1303-1313 | oleaut.c:341-366 | **PASS** — BSTR length-prefix contract honored |
| 25 | SysFreeString (#6) | frees/caches, NULL-tolerant | no-op leak | lib.rs:1314 | oleaut.c:273+ | BENIGN |
| 26 | SysStringLen (#7) | *(DWORD*)(str-4)/2 | identical | lib.rs:1315-1318 | oleaut.c:198-201 | **PASS** |
| 27 | SHGetKnownFolderPath | per-GUID distinct paths; unknown GUID → FILE_NOT_FOUND hr; CoTaskMem out | **every GUID → `C:\Users\player\AppData\Local`**, S_OK | lib.rs:1378-1388 | shellpath.c:3561-3611 | **TRUST-CHAIN** — Saved-games/Documents/LocalAppData collapse to one dir. Game writes saves + settings into the same sandbox dir; no corruption (paths still valid+writable) but any code keying identity off distinct roots (save-migration, "is this first run") misreads |
| 28 | SHGetFolderPathW | CSIDL-keyed path into caller buf | **ignores csidl** (rdx) → always AppData\Local, S_OK | lib.rs:1411-1417 | shellpath.c:2871-2882 | **TRUST-CHAIN** (same collapse as #27) |
| 29 | SHGetSpecialFolderPathW | FALSE on invalid CSIDL | CSIDL map (8 entries correct incl. 0x05 Documents, 0x1a/0x1c AppData) but unknown → `C:\Users\player` + **TRUE** | lib.rs:1389-1410 | shellpath.c:3406-3414 | WRONG-DATA (never-fail; the mapped answers themselves are the right shape) |
| 30 | RegisterClipboardFormatW | same name → **same atom** (rendezvous semantics) | **incrementing counter — same name gets a new atom every call** | lib.rs:1438-1442, 148 | clipboard.c:528-536 | WRONG-DATA — re-registration and cross-component compare break; range 0xC000+ correct |
| 31 | InitSecurityInterfaceW | static full fn-table, never fails | static table, version 4, 28 members = sspi_fail (SEC_E_UNSUPPORTED_FUNCTION) | lib.rs:2124-2130, 742-753 | secur32.c:169-172 | **PASS** — the Windows-offline shape: interface loads, ops fail per-call |
| 32 | DisableProcessWindowsGhosting | void flag-set | no-op | lib.rs:3136 | misc.c:390 | PASS |
| 33 | GetErrorMode/SetErrorMode | per-process mode; Set returns previous | atomic ERROR_MODE, swap-returns-old | lib.rs:1611-1612, 149 | process.c:786-794,1185-1193 | **PASS**. NB: a second dead `"SetErrorMode" => ret=0` arm exists at lib.rs:2150 — unreachable (first match wins), delete for hygiene |
| 34 | SetHandleCount | returns arg | returns arg | lib.rs:2138 | process.c:1198-1200 | **PASS** (exact) |
| 35 | LocalFree | success → NULL; invalid → handle + error | always NULL (success), leak | lib.rs:1356 | memory.c:1106-1136 | BENIGN |
| 36 | GetSystemDirectoryA/W, GetWindowsDirectoryA/W | fits → len (excl NUL); **too small → len+1 required-size, buffer untouched** | fits → len ✓; too small → **still returns len** (not len+1), buffer untouched | lib.rs:2151-2166 | file.c:67-68,376-390,2360-2378,2624+ | WRONG-DATA (edge: an exactly-len buffer caller reads success where real Windows says resize; paths themselves correct modulo case) |
| 37 | BCryptGenRandom | validated handle/flags; crypto-quality bytes | no validation; **deterministic splitmix fill**, STATUS_SUCCESS | lib.rs:2174-2180 | bcrypt_main.c:571-600 | WRONG-DATA — rand-quality contract broken (per-call det_ticks seed → advances, but predictable + repeatable across runs). Offline single-player: no security consumer; GUIDs/telemetry ids may collide across runs ‡ |
| 38 | SystemFunction036/RtlGenRandom/ProcessPrng | crypto bytes, BOOLEAN TRUE | same deterministic fill, TRUE | lib.rs:2167-2173 | — | WRONG-DATA (same as #37) |
| 39 | RegOpenKeyExW | missing key → **ERROR_FILE_NOT_FOUND (2)** | ERROR_FILE_NOT_FOUND(2) + *phk=NULL for everything | lib.rs:1546-1557 | registry.c:644+ | **PASS** — the not-found code is exact; "no registry at all = fresh install" is the honest offline stance. RegQueryValueExW→2, RegCloseKey→0 consistent (lib.rs:1558-1561) |
| 40 | WSAStartup (#115) | version negotiation, WSADATA strings, WSAVERNOTSUPPORTED/WSAEFAULT | zero-fill + wVersion=wHighVersion=2.2, ret 0 always | lib.rs:1323-1328 | socket.c:743-773 | BENIGN (2.2 is what CP2077 asks; description strings empty but unread ‡) |
| 41 | WSACleanup (#116) | balanced count; WSANOTINITIALISED when unbalanced | always 0 | lib.rs:1329 | socket.c:777-797 | BENIGN |
| 42 | WSAGetLastError (#111) | per-thread last WSA error | constant **10050 WSAENETDOWN** | lib.rs:1330 | socket.c:801 | BENIGN — consistent with every socket op failing WSAENETDOWN; "network is down" is the story we're telling |
| 43 | socket (#23) | INVALID_SOCKET + error on refusal | INVALID_SOCKET + set_last_error(10050) | lib.rs:1331 | — | **PASS** (honest-offline) |
| 44 | select (#18) | #ready; 0 = timeout | 0 | lib.rs:1332 | socket.c:2935 | PASS (0-ready is a legal quiet network) |
| 45 | gethostname (#57), ioctlsocket (#10), rest | gethostname **succeeds** offline; others per-op | ordinal catchall → SOCKET_ERROR | lib.rs:1333 | ws2_32.spec | WRONG-DATA (low — a failed gethostname just degrades a log/telemetry string ‡) |
| 46 | getaddrinfo/GetAddrInfoW | resolver miss → WSAHOST_NOT_FOUND ret | 11001 + *res=NULL | lib.rs:2139-2142 | — | **PASS** (exact offline shape); freeaddrinfo no-op lib.rs:2143 |
| 47 | InitializeSListHead | zero + **Header16.HeaderType=1** (Win64) | zeroes 16 bytes, HeaderType left 0 | lib.rs:2411 | sync.c:1010-1018 | WRONG-DATA (edge: only observable if interlocked SList ops read the header — none are shimmed; the touched list imports only the Init. If the game inlines its own push/pop CAS against this memory, all-zeroes is exactly the legal empty list ‡) |
| 48 | SetUnhandledExceptionFilter | returns previous filter, installs new | ret=0 (no previous — truthful), **doesn't store the new one** | lib.rs:3203 | — | WRONG-DATA (store it — the SEH dispatch hook will want it as the last-resort handler) |
| 49 | GetStartupInfoW/A | PEB-derived fill | zeroed + cb=104 | lib.rs:2307-2310 | process.c:1382 | PASS (dwFlags=0 → CRT defaults) |
| 50 | vendor surfaces (SteamAPI_Init→FALSE, SteamInternal_ContextInit→ctx-slot, sl*→fail, xess*→-2 UNSUPPORTED_DEVICE, REDGalaxy→0, nvapi/ags in LoadLibrary fail-list) | n/a (not Windows) | honest runtime-absent answers | lib.rs:1252-1287, 2028-2037, 2288-2295 | — | **PASS** — the no-vendor path is a real, tested path on every mismatched PC |

**Divergence rows that are real divergences (not PASS): 27.**
(#1-#7 minus #8-#9-pass … counted: rows 1,2,3,4,5,6,7,10,11,12,13,14,16,18,21,22,23,25,27,28,29,30,35,36,37,38,40,41,42,45,47,48 = 32 non-PASS rows; of these 12,17,41,42 etc. are by-design/benign. Strict count of behavior-differs-from-Windows rows: **32**; trust-chain-severity subset: **5** — rows 1, 2, 3(+4), 27(+28), and 13's anti-debug face.)

---

## 3. RET-0 GRADING (constant-return arms, sera's ·1599 ruling)

| fn / arm | constant | verdict |
|----------|----------|---------|
| CoInitializeEx/CoInitialize/OleInitialize → 0 | S_OK | **fake-success (mild)** — honest fix: per-thread `(model, count)` map → S_FALSE on same-model repeat, RPC_E_CHANGED_MODE on flip. ~10 lines, kills row 16 |
| CoUninitialize no-op | — | fully-correct-constant |
| CoInitializeSecurity/CoSetProxyBlanket → 0 | S_OK | fully-correct-constant (first-call answer; TOO_LATE nicety not worth state) |
| CoCreateInstance(Ex) → 0x80040154 | REGDB_E_CLASSNOTREG | fully-correct-constant (honest-negative, out-params filled) |
| SysFreeString → no-op | — | fully-correct-constant under bump-heap |
| FreeLibrary → 1, DisableThreadLibraryCalls → 1 | TRUE | fully-correct-constant (never-unload regime) |
| RemoveVectoredExceptionHandler → 1 | TRUE | **fake-success** — fix: remove rdx-matched entry from VEH_HANDLERS (needed before VEH invocation wires up, else stale handlers fire) |
| CloseHandle → 1 | TRUE | **fake-success** — fix: consult FILE_HANDLES/event/mapping tables; known → remove+TRUE, unknown → FALSE + ERROR_INVALID_HANDLE(6). Keeps leak-safety, restores hygiene answers |
| GetStdHandle → 0x10+rcx | fake handle | fake-but-harmless — fix only if console I/O ever matters (route to real registered handles) |
| GetModuleHandleW/A → 0x140000000 | exe base | **fake-success** — fix: module registry {exe, real-loaded DLLs, fake-LoadLibrary'd names}; absent → 0 + last-error 126. Also makes rows 2/3 self-consistent |
| GetModuleHandleExW → 1 | TRUE | same fix as above (write resolved base or fail) |
| EncodePointer/DecodePointer → rcx | identity | fully-correct-constant **on the round-trip contract** (verified: both arms identity ⇒ decode∘encode=id). Optional hardening: `p ^ COOKIE` with a boot-time random cookie — 2 lines, makes encoded values non-pointers |
| SetUnhandledExceptionFilter → 0 | NULL | truthful for "no previous" — **but store the filter** (see row 48) |
| WSAGetLastError → 10050 | WSAENETDOWN | fake-constant — honest fix: `ret = *LAST_ERROR` (socket arm already does set_last_error(10050); unify) — then row 1's per-thread fix benefits it too |
| WSACleanup → 0, select → 0 | 0 | fully-correct-constant (offline) |
| WS2 ordinal catchall → SOCKET_ERROR | -1 | correct-constant except gethostname (#57): give it a real hostname fill + 0 |
| LocalFree/GlobalFree → 0 | NULL=success | fully-correct-constant under leak regime |
| RegCloseKey → 0 | ERROR_SUCCESS | fully-correct-constant |
| RegOpenKeyExW/RegQueryValueExW → 2 | ERROR_FILE_NOT_FOUND | fully-correct-constant (exact not-found code; truthful-negative) |
| SetHandleCount → rcx | identity | fully-correct-constant (matches Windows exactly) |
| AreFileApisANSI → 1 | TRUE | fully-correct-constant (lib.rs:1840) |
| AppPolicyGet* → 0 + out=0 | ERROR_SUCCESS | fully-correct-constant (desktop answers) |
| GetVersion → 0x47BB000A | 10.0.18363 | fully-correct-constant (self-consistent with family 04's VerifyVersionInfo †) |
| DisableProcessWindowsGhosting → 1 | (void) | fully-correct (return ignored) |
| InitSecurityInterfaceW → sspi_table | table ptr | fully-correct-constant (real table, honest per-call failures) |
| BCryptGenRandom → 0 / RtlGenRandom → 1 | STATUS_SUCCESS/TRUE | **fake-quality** — return code honest, *bytes* aren't random. Honest fix: `getrandom(2)`/`/dev/urandom` fill (3 lines); keep det mode behind ALKY_DET env if determinism is wanted for replay |
| SteamAPI_Init → 0, sl* → 1, xess* → -2, Galaxy → 0 | vendor-absent | fully-correct-constant (documented failure of each SDK) |
| RaiseException non-threadname → exit(133) | die-loud | correct-by-policy (no silent swallow); replaced by SEH dispatch as it lands |
| GetStartupInfoW zero-fill cb=104 | — | fully-correct-constant |

### Priority fixes distilled (ordered by trust-chain weight)
1. **Per-thread LAST_ERROR** (thread-local or tid-keyed map) — row 1; feeds rows 5, 42.
2. **Module registry for GetModuleHandle\*** (+ make LoadLibrary/GetModuleHandle answers consistent) — rows 2/3/4.
3. **KNOWNFOLDERID/CSIDL → distinct sandbox dirs** (Documents, SavedGames, LocalAppData, Roaming minimum) — rows 27/28; save-path identity.
4. **CloseHandle validation** — row 13.
5. **SysAllocString: size from `encode_utf16().count()`** — row 23 (latent overrun).
6. **BCryptGenRandom → real entropy** — rows 37/38.
7. RegisterClipboardFormatW name→atom dedup map — row 30.
8. Store SetUnhandledExceptionFilter + wire VEH invocation into SEH dispatch — rows 10/48.
