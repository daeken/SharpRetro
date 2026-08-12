# 04 — MEMORY + VERSION-TRUST family (VirtualAlloc/Free/Protect/Query, Heap*, GlobalMemoryStatus(Ex), GetPhysicallyInstalledSystemMemory; GetFileVersionInfoSize(Ex)W, GetFileVersionInfo(Ex)W, VerQueryValueW)

Wave-contract deliverable. Sources: wine master @ /local/home/seratb/wine (kernelbase/memory.c,
ntdll/unix/virtual.c, ntdll/heap.c, kernel32/heap.c, kernelbase/version.c, version/tests/info.c,
kernel32/tests/{virtual,heap}.c); our shim @ /tmp/alky-shims-lib.rs.
† = source-only claim, ‡ = inference.

---

## 1. SPEC

### 1.1 VirtualAlloc(lpAddress=rcx, dwSize=rdx, flAllocationType=r8, flProtect=r9) → LPVOID

Chain: kernelbase `VirtualAlloc` → `VirtualAllocEx(GetCurrentProcess(),…)` (memory.c:420-427)
→ `NtAllocateVirtualMemory` (memory.c:429-437: on NTSTATUS failure sets last-error via
`set_ntstatus` and returns **NULL**; on success returns the *rounded base*, not the caller's
pointer — `ret = addr` in/out).

NtAllocateVirtualMemory front-door checks (ntdll/unix/virtual.c:5246-5330):
- `!size` → STATUS_INVALID_PARAMETER (:5256) → last-error ERROR_INVALID_PARAMETER, return NULL.
- `type & ~(MEM_COMMIT|MEM_RESERVE|MEM_TOP_DOWN|MEM_WRITE_WATCH|MEM_RESET)` → STATUS_INVALID_PARAMETER (:5251,5262).
- zero_bits is always 0 through the kernelbase path — irrelevant for Win32 callers.

allocate_virtual_memory (virtual.c:5120-5238), the semantics a game can observe:
- **Rounding**: if lpAddress != NULL and MEM_RESERVE (without MEM_REPLACE_PLACEHOLDER): base
  rounds **down to 64 KiB** (granularity_mask); otherwise rounds down to page. Size rounds up to
  page from the *unrounded* address (ROUND_SIZE with addr). Returned base = rounded base;
  returned/observable size = rounded size (both written back through in/out params, :5231-5234).
- lpAddress < 0x10000 (and not the DOS-memory special case) → STATUS_INVALID_PARAMETER (:5145-5151).
- `type` must contain at least one of MEM_COMMIT|MEM_RESERVE|MEM_RESET → else
  STATUS_INVALID_PARAMETER (:5162-5166).
- **MEM_COMMIT on an un-reserved address** (lpAddress given, no MEM_RESERVE, no existing view):
  STATUS_NOT_MAPPED_VIEW → Win32 last-error ERROR_NOT_SUPPORTED‡ (rarely relevant; real Windows
  gives ERROR_INVALID_ADDRESS — see kernel32/tests/virtual.c which only tests the NULL/base cases).
  MEM_COMMIT on an existing reserve = commit-in-place, protection of committed pages set to
  flProtect (:5202-5210).
- Reserve+commit in one call is the normal path (`vprot |= VPROT_COMMITTED`, :5180).
- flProtect = PAGE_WRITECOPY-family for private memory → STATUS_INVALID_PAGE_PROTECTION (:5185).
- Success writes: *ret = view base, *size = rounded size, returns STATUS_SUCCESS →
  VirtualAlloc returns the base pointer. Committed memory is **zero-filled** (anonymous mmap)†.
- No-memory path: STATUS_NO_MEMORY → ERROR_NOT_ENOUGH_MEMORY, NULL.

**Fresh-alloc protection invariant**: a new MEM_COMMIT|MEM_RESERVE region answers VirtualQuery
with State=MEM_COMMIT, Protect=flProtect, AllocationProtect=flProtect, Type=MEM_PRIVATE.

### 1.2 VirtualFree(lpAddress=rcx, dwSize=rdx, dwFreeType=r8) → BOOL

kernelbase VirtualFree → VirtualFreeEx (memory.c:520-527). **Win32-level check first**
(memory.c:529-538): `type==MEM_RELEASE && size!=0` → SetLastError(ERROR_INVALID_PARAMETER),
return FALSE **without calling ntdll**. (This is the documented Windows contract: MEM_RELEASE
requires dwSize==0.)

NtFreeVirtualMemory (virtual.c:5444-5533):
- size rounds up to page, base rounds down to page (:5479-5480).
- base==NULL → STATUS_INVALID_PARAMETER (ERROR_INVALID_PARAMETER).
- no view at base → STATUS_MEMORY_NOT_ALLOCATED (:5495) → ERROR_MEM_NOT_ALLOCATED‡ (winerror
  mapping of 0xC00000A0).
- view not a VirtualAlloc view (i.e. a mapped file/image) → STATUS_INVALID_PARAMETER (:5496).
- `size==0 && base != view->base` → STATUS_FREE_VM_NOT_AT_BASE (:5497) — you must pass the
  allocation base to MEM_RELEASE, not an interior pointer.
- range overruns the view → STATUS_UNABLE_TO_FREE_VM (:5498-5499).
- MEM_DECOMMIT: decommit_pages over [base,size); size==0 means whole view from base‡; pages
  return to MEM_RESERVE state; succeeds on already-decommitted pages†.
- MEM_RELEASE: size:=view->size, whole view deleted → subsequent VirtualQuery answers MEM_FREE.
- any other dwFreeType combination → STATUS_INVALID_PARAMETER.
- Success: TRUE; the *rounded* addr/size are written back (observable only via NtFree directly).

### 1.3 VirtualProtect(lpAddress=rcx, dwSize=rdx, flNewProtect=r8, lpflOldProtect=r9) → BOOL

kernelbase VirtualProtect → VirtualProtectEx (memory.c:553-560). On NT, **old_prot==NULL is NOT
tolerated**: NtProtectVirtualMemory returns STATUS_ACCESS_VIOLATION for old_prot==NULL
(virtual.c:5550-5551; kernelbase only substitutes a dummy on Win9x version-lie, memory.c:566-569)
→ last-error ERROR_NOACCESS, FALSE.

NtProtectVirtualMemory (virtual.c:5536-5608):
- base rounds down / size rounds up to page (:5577-5578).
- Whole range must be inside ONE view: `find_view(base,size)` else STATUS_INVALID_PARAMETER
  → ERROR_INVALID_PARAMETER‡ (an interior span crossing two allocations fails).
- **Every page in the range must be committed**: `get_committed_size(...) >= size` else
  STATUS_NOT_COMMITTED (:5587-5592) → ERROR_NOT_COMMITTED‡. Protecting reserved-only pages fails.
- Success: *lpflOldProtect = previous Win32 protection of the **first page**
  (`old = get_win32_prot(vprot, view->protect)` :5589), protection applied to whole range,
  TRUE returned. On ANY failure path *old_prot = PAGE_NOACCESS is still written (:5602-5606) —
  the out-param is always touched once past the NULL check.
- New protection value invalid (get_vprot_flags fails) → STATUS_INVALID_PAGE_PROTECTION →
  ERROR_INVALID_PARAMETER‡.

### 1.4 VirtualQuery(lpAddress=rcx, lpBuffer=rdx, dwLength=r8) → SIZE_T  «TOP CENSUS SUSPECT»

kernelbase VirtualQuery → VirtualQueryEx → NtQueryVirtualMemory(MemoryBasicInformation)
(memory.c:585-604). Returns **res_len (= sizeof(MEMORY_BASIC_INFORMATION) = 48 on x64) on
success, 0 on failure** with last-error set.

get_basic_memory_info (virtual.c:5745-5785):
- `len < sizeof(MEMORY_BASIC_INFORMATION)` → STATUS_INFO_LENGTH_MISMATCH (:5751-5752) →
  ERROR_BAD_LENGTH‡, return 0.
- addr beyond working_set_limit → STATUS_INVALID_PARAMETER (fill_basic_memory_info :5695) →
  ERROR_INVALID_PARAMETER, return 0. (Real Windows: ERROR_INVALID_PARAMETER for kernel-space
  addresses — same shape.)

**MEMORY_BASIC_INFORMATION fill semantics** (fill_basic_memory_info, virtual.c:5686-5742):
Wine keeps an rb-tree of views (`views_tree`); `get_memory_region_size` (virtual.c:5611-5660)
binary-searches it and computes [region_start, region_end) = the gap or the containing view.

For **base = ROUND_ADDR(addr, page_mask)** (query is page-granular; BaseAddress is the page
base of the query address, NOT the region start):

| region kind | BaseAddress | AllocationBase | AllocationProtect | RegionSize | State | Protect | Type |
|---|---|---|---|---|---|---|---|
| free (no view) | page base of addr | **0/NULL** | **0** | gap_end − base (up to next view or working_set_limit) | MEM_FREE | PAGE_NOACCESS | **0** |
| i386 fake-reserved (outside wine reserved areas) | page base | area base | PAGE_NOACCESS | area_end − base | MEM_RESERVE | PAGE_NOACCESS | MEM_PRIVATE |
| inside a view | page base | **view base** | get_win32_prot(view->protect) — the protection *the allocation was created with* | run of pages with same vprot starting at base (get_committed_size with mask ~VPROT_WRITEWATCH, :5732) | MEM_COMMIT if page committed else MEM_RESERVE | committed ? current Win32 prot : **0** | SEC_IMAGE → MEM_IMAGE; SEC_FILE/RESERVE/COMMIT → MEM_MAPPED; else MEM_PRIVATE |

Key coalescing/observability rules a walker sees:
1. **Free regions coalesce across everything unmapped**: RegionSize spans from the query page to
   the base of the next view (rb-tree successor), AllocationBase = NULL, Protect = PAGE_NOACCESS
   (NOT 0 — but AllocationProtect IS 0). kernel32/tests/virtual.c:409-421 asserts exactly this
   shape for a released region.
2. **Within one allocation, regions split at protection boundaries**: RegionSize is the length of
   the run of pages sharing identical vprot bits (write-watch ignored) starting at the query page
   — VirtualProtect'ing a middle page splits one region into three, each answering its own
   Protect but ALL answering the same AllocationBase/AllocationProtect.
3. **Adjacent separate VirtualAlloc reservations do NOT coalesce**: distinct views ⟹ distinct
   AllocationBase ⟹ a walker advancing by `BaseAddress+RegionSize` sees each 64k-aligned
   allocation separately, never merged.†
4. **Reserved-but-uncommitted pages answer Protect=0** (not PAGE_NOACCESS) with State=MEM_RESERVE
   — tests/virtual.c:437-441 asserts Protect==0 for decommitted pages of a private alloc.
5. **Image mappings answer Type=MEM_IMAGE with per-section protection**: .text = PAGE_EXECUTE_READ
   (typically, AllocationProtect = PAGE_EXECUTE_WRITECOPY for the module as mapped†), .data =
   PAGE_READWRITE/WRITECOPY, all sharing AllocationBase = module HMODULE base. This is the shape
   anti-tamper walks to find module bounds and to distinguish its own image pages from private RWX.
6. Querying an address **above the user-space limit** fails with ERROR_INVALID_PARAMETER rather
   than answering a kernel free region — a walker must stop on first 0 return.

### 1.5 HeapAlloc / HeapFree / HeapReAlloc / HeapSize / GetProcessHeap / HeapCreate / HeapSetInformation

Win32→ntdll aliasing (the whole family is **pure forwarding**, no kernel32/kernelbase logic):
- kernelbase.spec:815 `HeapAlloc → ntdll.RtlAllocateHeap`, :819 `HeapFree → RtlFreeHeap`,
  :822 `HeapReAlloc → RtlReAllocateHeap`, :824 `HeapSize → RtlSizeHeap`.
- GetProcessHeap = `NtCurrentTeb()->Peb->ProcessHeap` (kernelbase/process.c:910-913)† — a stable
  per-process handle, never NULL after process init.

RtlAllocateHeap (ntdll/heap.c:2038-2074):
- size > ~0u/2 (on 32-bit)/heap max → NULL + per-flags status; **last-error only set when the
  caller's flag demands**: heap_set_status/RtlSetLastWin32ErrorAndNtStatusFromNtStatus is invoked
  on failure (heap.c:597-612†): STATUS_NO_MEMORY → ERROR_NOT_ENOUGH_MEMORY,
  STATUS_INVALID_PARAMETER → ERROR_INVALID_PARAMETER. Success does NOT touch last-error.
- HEAP_ZERO_MEMORY honored; HEAP_GENERATE_EXCEPTIONS raises STATUS_NO_MEMORY exception instead of
  returning NULL†.
- Returns 16-byte-aligned (x64) block†; contents undefined unless HEAP_ZERO_MEMORY.

RtlFreeHeap (heap.c:2078-2110): `!ptr` → returns **TRUE immediately** (:2084 `if (!ptr) return TRUE`
— freeing NULL succeeds, matches Windows). Invalid pointer → FALSE + ERROR_INVALID_PARAMETER‡
(via heap_set_status). Valid free → TRUE.

RtlReAllocateHeap (heap.c:2229-2260): `!ptr` → returns **NULL** (:2238 — NOT alloc-on-NULL;
kernel32/tests/heap.c relies on this). Grows/shrinks in place when possible; on move, old
contents copied, old block freed; HEAP_REALLOC_IN_PLACE_ONLY honored†. size == ~(SIZE_T)0 →
NULL + ERROR_NOT_ENOUGH_MEMORY (tests/heap.c:291-296). Failure leaves the original block valid.

RtlSizeHeap (heap.c:2349-2370): invalid heap/ptr → **~(SIZE_T)0** (i.e. (SIZE_T)-1) +
ERROR_INVALID_HANDLE‡/last-error via heap_set_status; success → exact requested size of the
block. Conformance: tests/heap.c:289-290 — a 0-byte alloc answers HeapSize()==0;
:323-324 — alloc_size answers alloc_size (the size REQUESTED, not the rounded capacity).

HeapCreate (kernel32/heap.c:54-84): wraps RtlCreateHeap(flags translated; initialSize→commit,
maxSize→reserve). maxSize==0 ⟹ growable heap. Failure → NULL + SetLastError(ERROR_NOT_ENOUGH_MEMORY)†.
RtlCreateHeap never validates against a max count — heaps are views, effectively unlimited‡.

HeapSetInformation (kernelbase/memory.c:780-786): forwards to RtlSetHeapInformation →
**HeapCompatibilityInformation is accepted and ignored** (heap.c:2594-2610: for
HeapEnableTerminationOnCorruption returns STATUS_SUCCESS; for compat-info class stores/ignores
the LFH value, returns success†). Win32 result: TRUE. Real Windows also returns TRUE for
LFH-enable on the process heap (and silently no-ops under a debugger). So: constant TRUE is
actually the honest observable here for the classes CP2077 uses.

### 1.6 GlobalMemoryStatus(lpBuffer=rcx) → void   /  GlobalMemoryStatusEx(lpBuffer=rcx) → BOOL

GlobalMemoryStatusEx (kernelbase/memory.c:1399-1461):
- `status->dwLength != sizeof(MEMORYSTATUSEX)` (=64) → SetLastError(ERROR_INVALID_PARAMETER),
  FALSE (:1403-1407). **The dwLength check is the ONLY failure mode.**
- Fills from NtQuerySystemInformation(SystemBasicInformation + SystemPerformanceInformation)†:
  ullTotalPhys, ullAvailPhys (from perf info), ullTotalPageFile/ullAvailPageFile
  (phys+swap), ullTotalVirtual = user-space VA size (0x7ffe0000 for 32-bit, ~128 TiB x64†),
  ullAvailVirtual = TotalVirtual − reserved, ullAvailExtendedVirtual = 0.
- **dwMemoryLoad = 100 − 100*AvailPhys/TotalPhys**† — games (and their allocator heuristics) read
  this as a pressure signal.

GlobalMemoryStatus (kernel32/heap.c:421-475) — the legacy DWORD variant:
- **no return value, no failure** — always fills.
- Calls GlobalMemoryStatusEx and narrows. On 32-bit: values clamp to MAXDWORD
  (TotalPageFile to 0xfff7ffff — the "Sacrifice demo" clamp :459), and **without
  IMAGE_FILE_LARGE_ADDRESS_AWARE everything further clamps to MAXLONG (2 GiB)** (:468-474);
  plus the Photoshop-4 workaround adjusting AvailPageFile when Avail sums ≥ 2 GiB. On x64
  (`_WIN64`) none of the clamps compile in — raw 64-bit values truncated into SIZE_T fields.
  CP2077 is x64 ⟹ dwLength=32 struct with full values, no clamps.

### 1.7 GetPhysicallyInstalledSystemMemory(out kilobytes=rcx) → BOOL

kernelbase/memory.c:1380-1394: `!memory` → ERROR_INVALID_PARAMETER + FALSE; else
GlobalMemoryStatusEx → `*memory = memstatus.ullTotalPhys / 1024 + 512*1024` (Wine adds 512 MiB
to approximate "installed > usable", :1391-1392) → TRUE. Contract: value in **KiB**, ≥ the
TotalPhys the same process sees from GlobalMemoryStatusEx. Real Windows reads SMBIOS; failure
mode ERROR_INVALID_DATA when SMBIOS is broken — never in practice for games.

### 1.8 GetFileVersionInfoSizeW / GetFileVersionInfoSizeExW  «13-ROUND ROOT'S CLASS»

`GetFileVersionInfoSizeW(filename, handle) = GetFileVersionInfoSizeExW(FILE_VER_GET_LOCALISED, …)`
(kernelbase/version.c:737-741). All A-variants convert and forward (:745-747, :840-857).

GetFileVersionInfoSizeExW (version.c:753-835):
- `if (ret_handle) *ret_handle = 0` **always, first** (:760) — the "handle" out-param is dead on
  NT; a caller checking it must see 0.
- `filename == NULL` → ERROR_INVALID_PARAMETER, return 0 (:762-766).
- `*filename == 0` → ERROR_BAD_PATHNAME, return 0 (:767-771).
- Resource lookup: LoadLibraryExW(LOAD_LIBRARY_AS_IMAGE_RESOURCE) → FindResource(RT_VERSION=16,
  id VS_VERSION_INFO=1; non-localised flag tries English first) (:775-790). Fallback: raw file
  read via find_version_resource for non-loadable files (:793-799).
- File doesn't exist / not a PE: CreateFileW fails → return 0 with **CreateFileW's last-error
  (ERROR_FILE_NOT_FOUND / ERROR_PATH_NOT_FOUND)**. Conformance (tests/info.c:43-59) accepts
  {PATH_NOT_FOUND, RESOURCE_DATA_NOT_FOUND, FILE_NOT_FOUND, BAD_PATHNAME, SUCCESS} — version-
  dependent; any of these shapes passes.
- PE exists but **no RT_VERSION resource**: SetLastError(**ERROR_RESOURCE_DATA_NOT_FOUND**=1812),
  return 0 (:828-833; tests/info.c:190-194 asserts exactly this for a resource-less PE).
- **Success: SetLastError(0)** (:825 — last-error is CLEARED, tests:33 accept NO_ERROR) and
  return **(len * 2) + 4** for a 32-bit resource (:826) — NOT the raw resource length. The
  doubled-plus-"FE2X" buffer is the documented XP/W2K/W2K3 behavior; the extra area is scratch
  for Unicode→ANSI conversion in later VerQueryValueA calls. 16-bit NE resources answer
  `(len − sizeof(VS_FIXEDFILEINFO)) * 4` (:817).

**Size-0-means-what**: 0 = "no version info obtainable" — the caller's trust-branch. The
last-error disambiguates: 1812 = file exists, no version resource; 2/3 = no file;
87/161 = bad argument. **A size-0 answer for a file that HAS a version resource is a lie that
flips the game's "is this DLL legit" branch** — the 13-round root class.

### 1.9 GetFileVersionInfoW / GetFileVersionInfoExW

`GetFileVersionInfoW(name, handle_ignored, datasize, data) =
GetFileVersionInfoExW(FILE_VER_GET_LOCALISED, …)` (version.c:966-969).

GetFileVersionInfoExW (version.c:862-955):
- `data == NULL` → ERROR_INVALID_DATA, FALSE (:872-876; conformance tests/info.c:248-251).
- Same resource lookup as Size. **Copy is `min(SizeofResource, datasize)` — a short buffer
  TRUNCATES SILENTLY and still returns TRUE** (:895-897). There is NO
  ERROR_INSUFFICIENT_BUFFER failure mode on NT.
- After copy, if `datasize >= wLength + 4`, the literal bytes **"FE2X" are appended at
  data+wLength** (:928-930) — the A-conversion scratch signature. Tests (info.c:340-360†)
  probe this layout.
- **Success: SetLastError(0), return TRUE** (:931-935).
- No resource → ERROR_RESOURCE_DATA_NOT_FOUND, FALSE (:938-940).
- The `handle`/`ignored` argument is completely unread.

### 1.10 The VS_VERSIONINFO blob (what data contains)

Pseudo-struct VS_VERSION_INFO_STRUCT32 (version.c:87-99), the on-disk/RT_VERSION format:
`{WORD wLength; WORD wValueLength; WORD wType; WCHAR szKey[]; pad-to-DWORD; BYTE Value[wValueLength*(wType?2:1)]; pad; children[]}` —
recursively. Root key = L"VS_VERSION_INFO", root Value = VS_FIXEDFILEINFO (52 bytes, magic
dwSignature = **0xFEEF04BD** at Value[0], tests/info.c:644). Children: L"StringFileInfo"
(→ one child per lang-block keyed "%04x%04x" langid+codepage → per-string children) and
L"VarFileInfo" (→ child L"Translation", Value = array of {WORD lang; WORD codepage} pairs).
16-vs-32 discrimination: `szKey[0] >= ' '` at offset 4 means 16-bit (VersionInfoIs16,
version.c:101-102 — a WCHAR 'V'=0x56 has high byte 0, failing the ≥' ' test on the 32-bit form).

### 1.11 VerQueryValueW(pBlock, lpSubBlock, lplpBuffer, puLen) → BOOL

version.c:1175-1226; the 32-bit walk is VersionInfo32_QueryValue (:1064-1114) over
VersionInfo32_FindChild (:1002-1017, **case-insensitive wcsnicmp** key match).

- `pBlock == NULL` → FALSE, **no last-error set** (:1182-1183).
- `lpSubBlock NULL or ""` → treated as **L"\\"** (:1185-1186) — NULL is a valid root query.
- Path walk: components split on '\', empty components skipped (so "\\\\StringFileInfo" etc.
  tolerate doubled slashes); each component looked up case-insensitively among children.
- **Component not found → `*puLen = 0` + SetLastError(ERROR_RESOURCE_TYPE_NOT_FOUND=1813) +
  FALSE** (:1090-1096). The out-len IS written on this failure. (tests/info.c:506,573 assert
  1813, accepting ERROR_SUCCESS broken() only for w2k.)
- Found: `*lplpBuffer = DWORD_ALIGN(szKey end)`; **empty value (wValueLength==0 and value
  would point past the node) → pointer to the terminator of szKey** — still TRUE with len 0
  (:1100-1102; conformance test_null_value_32 tests/info.c:700-712: ret TRUE, len==0, p = exact
  documented offset inside the key area).
- `*puLen = wValueLength` — for **text values (wType=1) this is in WCHARs** (test:719-721:
  ProductVersion "1.0.0.0" answers len==8 = 7 chars + nul), for binary (wType=0) in bytes.
- Root "\\" answer: pointer to VS_FIXEDFILEINFO, len = 0x34 (52) when present; caller checks
  dwSignature==0xFEEF04BD (test_32bit_win pattern, tests/info.c:640-660†).
- "\\VarFileInfo\\Translation": TRUE + pointer to the {lang,codepage} DWORD array, len = bytes
  (4 per pair). The canonical consumer then formats "\\StringFileInfo\\%04x%04x\\<name>".
- VerQueryValueA (:1119-1170): same walk; for **text** results converts the W value into the
  scratch area at `pBlock + wLength + 4` (the "FE2X" area, :1158-1163) and answers the
  converted ANSI pointer + ANSI length. Binary results (root, Translation) pass through
  unconverted.

---

## 2. DIVERGENCE table (Wine/Windows spec vs /tmp/alky-shims-lib.rs)

| # | fn | real Windows answers | ours answers | wine cite / shim cite | severity |
|---|----|----|----|----|----|
| D1 | VirtualQuery | reserved-uncommitted pages: State=MEM_RESERVE, Protect=0 | PROT_NONE maps answer State=MEM_COMMIT, Protect=PAGE_NOACCESS (prot==0 only on the free-gap arm) | virtual.c:5732-5738 / shims:2472-2483 | **TRUST-CHAIN** — the game reserves 16-64 GiB then walks for committed sub-ranges; committed-vs-reserved is THE discriminator its allocator + anti-tamper read |
| D2 | VirtualQuery | AllocationBase = VirtualAlloc reservation base for every page of the allocation | AllocationBase = base of the *Linux mapping* (mprotect-split fragments each answer their own base) | virtual.c:5714 / shims:2477 | **TRUST-CHAIN** — module-bounds and alloc-ownership walks key on AllocationBase equality |
| D3 | VirtualQuery | BaseAddress = page base of the QUERY address | BaseAddress = start of the whole containing mapping | virtual.c:5698 (base=ROUND_ADDR(addr)) / shims:2476 | WRONG-DATA — a `addr+RegionSize` walker still terminates, but per-page probes get region-start instead of own-page |
| D4 | VirtualQuery | free region: AllocationProtect=0, Protect=PAGE_NOACCESS, Type=0 | AllocationProtect=PAGE_NOACCESS(1) on free arm; Protect=PAGE_NOACCESS ✓, Type=0 ✓ | virtual.c:5701-5707 / shims:2478 | BENIGN-ish — tests/virtual.c:409-421 asserts AllocationProtect==0; scanner comparing !=0 misreads |
| D5 | VirtualQuery | image pages: Type=MEM_IMAGE, mapped files MEM_MAPPED | Type=MEM_PRIVATE for everything mapped | virtual.c:5723-5727 / shims:2483 | **TRUST-CHAIN** — self-integrity walks distinguish own image pages from private RWX by Type |
| D6 | VirtualQuery | RegionSize = run of pages with identical protection from query page | Linux maps are split per-protection ✓ equivalent granularity | virtual.c:5730-5734 / shims:2481 | PASS (structurally equivalent) |
| D7 | VirtualQuery | addr ≥ user-space limit → 0 + ERROR_INVALID_PARAMETER | beyond-last-map → ret 0 + err 87 ✓; BUT high Linux maps (stack, vdso, vsyscall) answer as normal committed regions | virtual.c:5693-5695 / shims:2465-2470 | BENIGN‡ — a walker sees Linux-only mappings a Windows process wouldn't have; fingerprintable |
| D8 | VirtualAlloc | size==0 → NULL + ERROR_INVALID_PARAMETER; type must contain COMMIT/RESERVE/RESET; flProtect honored & echoed by VirtualQuery | size==0 → 0 with NO last-error; alloc_type unvalidated; flProtect (r9) **entirely ignored — all commits RWX** | virtual.c:5256,5162-5166,5185 / shims:2434, virtual_alloc fn | WRONG-DATA — RWX-everything also trips anti-cheat RWX scans‡; last-error gap benign for boot |
| D9 | VirtualAlloc | commit-at-addr returns page-rounded base; reserve-at-addr rounds to 64 KiB | commit-at-addr returns caller's `addr` unrounded; bump reservations are 64 KiB-stepped ✓ | virtual.c:5138-5141 / shims virtual_alloc:12-16 | BENIGN (games pass aligned addrs) |
| D10 | VirtualAlloc | MEM_COMMIT on unreserved address → fail (STATUS_NOT_MAPPED_VIEW‡) | falls back to fresh MAP_FIXED mmap — silently succeeds | virtual.c:5199-5210 / shims virtual_alloc:17-21 | BENIGN-permissive (can't be observed as wrong by a correct caller) |
| D11 | VirtualFree | MEM_RELEASE+size≠0 → FALSE+ERROR_INVALID_PARAMETER **before ntdll**; frees unmap (later query = MEM_FREE); interior ptr → FREE_VM_NOT_AT_BASE | constant TRUE, nothing unmapped, no validation | memory.c:529-538, virtual.c:5495-5499 / shims:2491 | WRONG-DATA — freed regions still answer committed; leak is boot-safe but a free/re-query protocol misreads |
| D12 | VirtualProtect | *old = real previous prot of first page; fails STATUS_NOT_COMMITTED on reserved pages, ACCESS_VIOLATION on old==NULL; protection actually changes | constant TRUE, *old := PAGE_EXECUTE_READWRITE always, no mprotect | virtual.c:5550,5587-5592 / shims:2435-2438 | **TRUST-CHAIN** — W^X/integrity re-protect sequences verify the old value round-trips; a constant 0x40 answer is detectable in one call-pair |
| D13 | HeapReAlloc | lpMem==NULL → NULL (no alloc-on-null) | treats as fresh alloc, returns new block | heap.c:2238 / shims:2219-2246 | WRONG-DATA (CRT realloc semantics differ; msvcrt shields most callers‡) |
| D14 | HeapSize | invalid/untracked ptr → (SIZE_T)-1 + last-error | untracked ptr → **0** | heap.c:2349-2370, tests/heap.c:1556 / shims:2249-2253 | WRONG-DATA — 0 is a VALID answer for a 0-byte block (tests/heap.c:289-290); -1 is the error shape |
| D15 | HeapAlloc | requested-size echoed by HeapSize ✓; HEAP_ZERO_MEMORY zeroing ✓ (fresh bump pages); failure → NULL (+GENERATE_EXCEPTIONS raise) | matches for fresh allocs; exhaustion prints + NULL ✓ | tests/heap.c:323-324 / shims:2219-2246 | PASS for boot scope |
| D16 | HeapFree | invalid ptr → FALSE; valid → TRUE; NULL → TRUE | constant TRUE (leak-on-free) | heap.c:2084 / shims:2248 | BENIGN for boot (NULL-free TRUE matches; no double-free detection observable) |
| D17 | HeapCreate | distinct heap handle per call, blocks segregated | returns the same HEAP_HANDLE as process heap | kernel32/heap.c:54-84 / shims:2218 | BENIGN until a HeapDestroy of a private heap is expected to invalidate its blocks |
| D18 | GetProcessHeap | PEB→ProcessHeap, stable nonzero | constant HEAP_HANDLE, stable nonzero | process.c:910-913 / shims:2217 | PASS |
| D19 | HeapSetInformation | TRUE for LFH-enable/termination-on-corruption classes | constant TRUE | memory.c:780-786, heap.c:2594-2610 / shims:2137 | PASS (honest constant) |
| D20 | GlobalMemoryStatus | x64 MEMORYSTATUS = {DWORD,DWORD, 7×SIZE_T} = 64 bytes, dwLength:=64, raw 64-bit values, no clamps | writes **32-bit fields** (dwLength:=32, seven u32 = the 32-bit layout) | kernel32/heap.c:435-474 (+`#ifndef _WIN64` clamps) / shims:2956-2969 | **WRONG-DATA (layout)** — an x64 reader loads dwTotalPhys as u64 spanning two 0xFFFFFFFF fields = garbage; cache-sizing heuristics read this |
| D21 | GlobalMemoryStatusEx | dwLength!=64 → FALSE+ERROR_INVALID_PARAMETER; values from live system | no dwLength check; constant 32 GiB/24 GiB, load 25, 128 TiB virtual (correct x64 layout ✓) | memory.c:1403-1407 / shims:2970-2985 | BENIGN — plausible constants, layout right; missing validation unobservable to correct callers |
| D22 | GetPhysicallyInstalledSystemMemory | NULL out → FALSE+ERROR_INVALID_PARAMETER; KiB ≥ TotalPhys/1024 | NULL → TRUE without write; 32 GiB constant, == TotalPhys exactly (wine adds +512 MiB) | memory.c:1380-1394 / shims:1374-1377 | BENIGN — consistent with D21's constants |
| D23 | GetFileVersionInfoSizeW/A | success = **(resource len × 2) + 4**; SetLastError(0) on success; *handle:=0 | success = **raw blob.len()**; last-error untouched on success; *handle:=0 ✓ | version.c:819-826 / shims:1701-1714 | **TRUST-CHAIN** — THE 13-round-root class: any consumer comparing the size against the raw resource (or allocating and then A-converting) sees a non-Windows number; the "FE2X" scratch area doesn't exist in our sizing |
| D24 | GetFileVersionInfoSizeW | NULL name → ERROR_INVALID_PARAMETER; "" → ERROR_BAD_PATHNAME; no-resource → 1812; missing file → FILE/PATH_NOT_FOUND | every failure = 1812 ERROR_RESOURCE_DATA_NOT_FOUND | version.c:762-771,828-833 / shims:1708-1712 | BENIGN-ish — tests accept 1812 among the OR'd shapes for the file cases; NULL/"" shapes differ |
| D25 | GetFileVersionInfoW/A | short buffer → **TRUE + silent truncation** (copy min(len,datasize)); success sets last-error 0 + appends "FE2X" at data+wLength | short buffer → FALSE + ERROR_INSUFFICIENT_BUFFER(122); no FE2X; last-error untouched on success | version.c:895-897,928-935 / shims:1692-1699 | WRONG-DATA — self-consistent with our Size (blob.len() buffers always "fit"), wrong against a caller passing its own smaller budget |
| D26 | GetFileVersionInfoExW (Ex forms only) | 5th arg (data) at [rsp+0x28], datasize in r9 | reads len from [rsp+0x28] and buf from [rsp+0x30] — comment says buf@0x28; code and comment disagree ‡ | win64 ABI / shims:1688-1690 | **BLOCKER‡ for Ex-variants** (unverified-by-run; non-Ex forms the game imports use registers and are correct) |
| D27 | VerQueryValueW/A root "\\" | pointer to VS_FIXEDFILEINFO within the walked structure, *puLen = wValueLength (0x34) | scans first 0x100 bytes for 0xFEEF04BD magic, *puLen := 52 constant | version.c:1100-1108 / shims:1715-1733 | PASS-equivalent on real blobs (magic-scan lands on the same bytes); diverges only if wValueLength≠52 (pre-VS_FF layouts, hostile blobs) ‡ |
| D28 | VerQueryValueW sub-blocks | \VarFileInfo\Translation and \StringFileInfo\… on a real game EXE → **TRUE** + pointer + len; not-found → *puLen:=0 + ERROR_RESOURCE_TYPE_NOT_FOUND + FALSE | **any sub-block query → FALSE**, *puLen not written, no last-error | version.c:1090-1096,1104-1113 / shims:1734-1737 | **TRUST-CHAIN** — the canonical version-string read (root → Translation → StringFileInfo\%04x%04x\ProductName) dies at step 2 with a shape (len untouched, error untouched) real Windows never produces |
| D29 | VerQueryValueW | pBlock==NULL → FALSE quietly; NULL/"" subblock = root query | NULL subblock: read_path(0) behavior ‡untested; likely fault or empty→root-arm ✓ | version.c:1182-1186 / shims:1719-1721 | BENIGN‡ |

**Divergence count: 29 rows, of which 6 TRUST-CHAIN (D1, D2, D5, D12, D23, D28), 1 BLOCKER‡ (D26), 7 WRONG-DATA, rest BENIGN/PASS.**

---

## 3. RET-0 GRADING (constant-return arms, sera's ·1599 ruling)

| arm (shim line) | constant | grade | honest fix |
|---|---|---|---|
| VirtualProtect → 1, *old:=0x40 (2435) | fake-success | **fake** — old-prot is fabricated | IMPLEMENT-REAL: keep a page-prot map beside VALLOC bookkeeping (we already own every mmap/mprotect); return tracked prior prot, actually mprotect the range. ~30 lines. Failing that: die-loud when caller's flNewProtect ≠ RWX so the fabrication is visible at the call site |
| VirtualFree → 1 (2491) | fake-success | fake-but-boot-safe (leak-by-design) | IMPLEMENT-REAL: MEM_RELEASE → munmap + drop from VALLOC map (makes D1/D11 VirtualQuery answers truthful); MEM_DECOMMIT → mprotect(PROT_NONE)+madvise(DONTNEED). Validate the MEM_RELEASE+size≠0 → FALSE gate (memory.c:529 does it in kernelbase — 3 lines) |
| VirtualQuery (2439-2490) | — | **already IMPLEMENT-REAL** (census suspect #1 got its fix); residuals graded D1-D7 | close D1 (reserved≠committed): our own reserve path uses PROT_NONE, so map "---" perms + inside-VALLOC-range → MEM_RESERVE/Protect=0; free-gap AllocationProtect:=0 (D4, one line); Type:=MEM_IMAGE for the main-exe range (D5) |
| HeapFree → 1 (2248) | fake-success | fake-by-design (bump allocator) | acceptable for boot; real fix = free-list or mimalloc-backed heap. NULL→TRUE already matches Windows (heap.c:2084) |
| HeapSize untracked → 0 (2252) | fake-0 | **fake** — 0 collides with the legal 0-byte-block answer (tests/heap.c:289) | return (SIZE_T)-1 + SetLastError(ERROR_INVALID_PARAMETER) for untracked ptrs — 2 lines, removes the ambiguity |
| HeapValidate → 1 (2254) | constant TRUE | fully-correct-constant for a heap we define as always-consistent | — |
| HeapDestroy → 1 (2255) | fake-success | benign-fake (nothing to destroy; blocks stay valid — MORE permissive than Windows) | acceptable; real fix rides the HeapCreate-distinct-handles fix |
| HeapCreate → HEAP_HANDLE (2218) | aliased handle | fake (all heaps are the process heap) | distinct opaque handles mapping to the same bump region; blocks tagged by owner if a game ever HeapDestroy's mid-boot |
| HeapSetInformation → 1 (2137) | constant TRUE | **fully-correct-constant** — Windows answers TRUE for LFH/termination classes (heap.c:2594-2610 returns success; LFH is default-on and unobservable) | — |
| GetProcessHeap → HEAP_HANDLE (2217) | constant handle | **fully-correct-constant** — Windows answer is likewise a process-lifetime-stable opaque value | — |
| GetProcessHeaps → 1 heap (2256) | constant 1 | fully-correct-constant for a 1-heap process‡ | — |
| GlobalMemoryStatus fill (2956) | constant fill, 32-bit layout | **fake + WRONG LAYOUT on x64** (D20) | write the x64 MEMORYSTATUS (7×u64 after the two DWORDs, dwLength:=64); reuse the Ex values — 8 lines |
| GlobalMemoryStatusEx fill (2970) | constant 32/24 GiB | fake-but-plausible; layout correct | honest fix: /proc/meminfo MemTotal/MemAvailable → real values (also makes GetPhysicallyInstalled consistent for free); keep 25% load or derive |
| GetPhysicallyInstalledSystemMemory → 32 GiB (1374) | constant | fake-but-plausible, self-consistent with D21 | derive from same /proc/meminfo read, + keep ≥ TotalPhys invariant (wine adds 512 MiB: memory.c:1391) |
| GetFileVersionInfoSize* (1701) | — | IMPLEMENT-REAL (real PE/RT_VERSION walk — the 13-round fix) but **returns the wrong number** (D23) | `ret = blob.len()*2 + 4` + `set_last_error(0)` on success — the two-line spec-exact close |
| GetFileVersionInfo* (1684) | — | IMPLEMENT-REAL; failure-shape divergences D25/D26 | truncate-and-succeed (copy min, ret 1, last-error 0); append b"FE2X" at blob.len() when room; fix the Ex stack-arg indices (r9=datasize, [rsp+0x28]=data) |
| VerQueryValue root (1715) | — | IMPLEMENT-REAL (magic-scan, position-independent) — PASS on real blobs | — |
| VerQueryValue sub-block → 0 (1734) | fake-failure | **fake** — honest-0 comment is wrong for a real game EXE where Windows answers TRUE (D28) | IMPLEMENT-REAL: walk VS_VERSION_INFO_STRUCT32 (wLength/wValueLength/wType + DWORD-align, case-insensitive key compare — the exact loop at version.c:1002-1017/1064-1114, ~40 lines of safe Rust over the blob we already own). Interim die-loud: log the queried path so the branch is visible |

### The trust-chain, one paragraph
CP2077's boot walks BOTH families as *introspection*, not allocation: VirtualQuery tells its
allocator (and any anti-tamper) what the address space **is** — commit-vs-reserve (D1),
who-owns-what (D2), image-vs-private (D5) are branch inputs, and a fabricated
VirtualProtect old-prot (D12) breaks any probe that round-trips a protection change. The
version family is pure trust: size-0/FALSE from GetFileVersionInfoSize*/VerQueryValue* reads as
"this module has no identity" (D23/D28) — the exact class that cost 13 rounds before the
version_blob fix landed. The spec-exact closes are cheap: ×2+4 sizing (2 lines), sub-block walk
(~40 lines), reserved-state VirtualQuery (map "---"+VALLOC-range → MEM_RESERVE), tracked
VirtualProtect. Everything graded PASS above is genuinely the honest Windows answer and should
not be "fixed".
