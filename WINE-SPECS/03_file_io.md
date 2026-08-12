# 03_FILE_IO — Wine-spec family (CreateFile/Read/Write/Find/mapping/IOCP/paths/filetimes)

Sources: wine @ /local/home/seratb/wine (master, depth-1) — `dlls/kernelbase/file.c` (4737L),
`dlls/kernelbase/sync.c`, `dlls/kernelbase/memory.c`, `dlls/kernelbase/path.c`,
`dlls/ntdll/unix/file.c`; conformance truth: `dlls/kernel32/tests/file.c` (7085L).
Our shim: `/tmp/alky-shims-lib.rs` (3211L).
Touched-list cross-check (`touched_apis_cp2077.txt`): all assigned fns are touched **except**
GetFileInformationByHandleEx, MapViewOfFile, UnmapViewOfFile, PostQueuedCompletionStatus,
GetOverlappedResult, FindFirstFileW (only the ExW form is touched, line 31). GFIBHE is specced
anyway (census suspect); the mapping/IOCP siblings are specced because CreateFileMappingW (16),
CreateIoCompletionPort (18) and GetQueuedCompletionStatus (65) ARE touched — a caller that
creates a mapping/port will call the sibling next frame.

`†` = source-only claim (not runtime-verified). `‡` = inference.

---

## 1. SPEC

### CreateFileW
`HANDLE CreateFileW(name, access, share, sa, creation, flags, template)` — kernelbase/file.c:795.
- Disposition → NT map (file.c:807-813): CREATE_NEW→FILE_CREATE, CREATE_ALWAYS→FILE_OVERWRITE_IF,
  OPEN_EXISTING→FILE_OPEN, OPEN_ALWAYS→FILE_OPEN_IF, TRUNCATE_EXISTING→FILE_OVERWRITE.
- Failure: returns **INVALID_HANDLE_VALUE** (never NULL). NULL/empty name → ERROR_PATH_NOT_FOUND
  (file.c:820-822); un-parseable DOS path → ERROR_PATH_NOT_FOUND (file.c:850-853).
- Error exactness on the miss classes: last component missing → ERROR_FILE_NOT_FOUND;
  a missing **directory** component → ERROR_PATH_NOT_FOUND (comes from
  STATUS_OBJECT_NAME_NOT_FOUND vs STATUS_OBJECT_PATH_NOT_FOUND through RtlNtStatusToDosError,
  file.c:916). Conformance: tests/file.c:748-750 (source miss = ERROR_FILE_NOT_FOUND),
  tests/file.c:733-738 (NULL = ERROR_PATH_NOT_FOUND).
- CREATE_NEW collision: special-cased so last-error is **ERROR_FILE_EXISTS**, *not*
  ERROR_ALREADY_EXISTS (file.c:905-912 — comment at 907 says exactly this).
- **Last-error is set on SUCCESS too**: CREATE_ALWAYS that overwrote (io.Information ==
  FILE_OVERWRITTEN) or OPEN_ALWAYS that opened an existing file (FILE_OPENED) ⇒
  SetLastError(ERROR_ALREADY_EXISTS) (file.c:920-923). Callers use this as the
  "did I create it?" discriminator.
- Sharing: share mode is passed to NtCreateFile; conflicting opens fail
  ERROR_SHARING_VIOLATION (conformance: tests/file.c:741-745, read-locked source).
- Special names: `CONIN$`/`CONOUT$`/`CON` → console paths (file.c:825-838); `\\.\` device
  namespace; VXD fallback (file.c:894-899, win9x only).

### ReadFile
`BOOL ReadFile(h, buf, count, *read, OVERLAPPED*)` — file.c:3623.
- `*read` pre-zeroed (file.c:3648 `if (result) *result = 0;`).
- OVERLAPPED path: read offset comes from `overlapped->Offset/OffsetHigh` (positioned,
  cursor-independent); `overlapped` doubles as the IO_STATUS_BLOCK (Internal=status,
  InternalHigh=bytes); hEvent is passed to NtReadFile; completion value suppressed when the
  event pointer has its LSB set (file.c:3640-3645 — the "don't post to IOCP" convention).
- No OVERLAPPED + STATUS_PENDING ⇒ wait on the **file handle** itself (file.c:3656-3660).
- `*read` filled from iosb.Information when sync, or when overlapped-and-success (:3662).
- **EOF split** (file.c:3664-3671): sync read at EOF ⇒ **TRUE with *read=0**; overlapped read
  at/past EOF ⇒ **FALSE + ERROR_HANDLE_EOF**. Games' streaming loops branch on this.
- Other failures: FALSE + RtlNtStatusToDosError. A successful fast-path overlapped read may
  return TRUE immediately (callers must tolerate both TRUE and FALSE+ERROR_IO_PENDING).
- Handle associated with an IOCP: completion packet posts even on synchronous success unless
  SetFileCompletionNotificationModes(SKIP_ON_SUCCESS) — ntdll-side behavior.†

### WriteFile
`BOOL WriteFile(h, buf, count, *written, OVERLAPPED*)` — file.c:4026.
- Mirror of ReadFile: positioned write from OVERLAPPED offset, iosb-in-overlapped, event,
  cvalue-LSB rule (:4039-4047), *written pre-zeroed (:4052), sync-wait on the file handle when
  no OVERLAPPED (:4055-4059), *written from Information (:4062), FALSE +
  RtlNtStatusToDosError on failure (:4064-4068). No EOF special case.

### SetFilePointerEx
`BOOL SetFilePointerEx(h, dist, *newpos, method)` — file.c:3899.
- FILE_BEGIN: pos = dist; FILE_CURRENT: queries FilePositionInformation then adds;
  FILE_END: queries FileStandardInformation (EndOfFile) then adds; other method ⇒
  ERROR_INVALID_PARAMETER (:3910-3935).
- Resulting position < 0 ⇒ FALSE + **ERROR_NEGATIVE_SEEK** (:3937-3941).
- Seeking beyond EOF is legal (no error, file not extended until write/SetEndOfFile).
- On success sets FilePositionInformation and fills *newpos if non-NULL (:3943-3950).

### GetFileSizeEx
`BOOL GetFileSizeEx(h, *size)` — file.c:3271. NtQueryInformationFile(FileStandardInformation),
*size = EndOfFile (:3278-3287). FALSE + mapped error on bad handle (ERROR_INVALID_HANDLE).

### GetFileAttributesW / GetFileAttributesExW
- `GetFileAttributesW` — file.c:1783: NtQueryAttributesFile on the DOS path; returns the
  attribute DWORD or **INVALID_FILE_ATTRIBUTES** (0xffffffff) + last-error with the same
  FILE_NOT_FOUND / PATH_NOT_FOUND split as CreateFileW (:1790-1800). Conformance:
  tests/file.c:4693-4695.
- `GetFileAttributesExW` — file.c:1827: level != GetFileExInfoStandard ⇒
  ERROR_INVALID_PARAMETER (:1834-1838); NULL out-ptr ⇒ ERROR_INVALID_PARAMETER; fills
  WIN32_FILE_ATTRIBUTE_DATA {attrs, ctime, atime, mtime, sizeHi, sizeLo} from
  NtQueryFullAttributesFile (:1846-1866).

### GetFileInformationByHandleEx  (CENSUS SUSPECT — file-identity class)
`BOOL GetFileInformationByHandleEx(h, class, out, size)` — file.c:3168-3268.
- Class dispatch to NtQueryInformationFile / NtQueryDirectoryFile; unknown class ⇒
  ERROR_INVALID_PARAMETER (:3180-3186 region). Success ⇒ TRUE; failure ⇒ FALSE +
  RtlNtStatusToDosError (:3260-3266).
- **FileIdInfo** (class 18) → FileIdInformation (:3216-3218). Fill (ntdll/unix/file.c:5140-5153):
  `VolumeSerialNumber` from the mount manager (**0 when mountmgr unavailable** — wine itself
  degrades here), `FileId` = 128-bit, zeroed then low 64 bits = **st_ino**. So on real
  Windows/NTFS: (volume serial, 64-bit FRN); identity pair games use for same-file /
  anti-tamper checks. FileIdBothDirectoryInfo (+Restart) → directory scan variant (:3221-3230).
- FileBasicInfo/FileStandardInfo/FileNameInfo etc. are straight NtQueryInformationFile passes
  with a buffer-size gate (ERROR_BAD_LENGTH for undersized fixed classes †).

### FindFirstFileW / FindFirstFileExW
`HANDLE FindFirstFileExW(name, level, *data, search_op, filter, flags)` — file.c:1287.
FindFirstFileW = ExW(FindExInfoStandard, FindExSearchNameMatch, 0) (file.c:1276-1284 †).
- Parameter validation: level > FindExInfoBasic ⇒ ERROR_INVALID_PARAMETER; bad search_op /
  filter / flags likewise (:1300-1330). FindExSearchLimitToDirectories is accepted but
  **ignored** by wine (FIXME) — non-directories are still returned.†
- Path split: directory part + mask. Mask preparation (file.c:1258-1286): trailing dots and
  spaces stripped; `.*` tail converted to DOS_DOT `"`; `*`/`?` get DOS_STAR/DOS_QM semantics —
  i.e. **`*.*` matches every name including extensionless ones**. Conformance:
  tests/file.c:3066-3160 (wildcard table: "*.*" finds `a`, `a.`, `.a`…; `<.>` DOS forms).
- Missing directory component ⇒ INVALID_HANDLE_VALUE + **ERROR_PATH_NOT_FOUND** (:1321);
  directory exists but nothing matches ⇒ **ERROR_FILE_NOT_FOUND**; empty mask ⇒
  ERROR_FILE_NOT_FOUND (:1355 region). Conformance: tests/file.c:2790-2860 (exact per-shape
  error table, incl. ERROR_DIRECTORY for `file\*`).
- Matching is **case-insensitive** (NtQueryDirectoryFile mask semantics over the whole
  Unicode range, not just ASCII).
- **Dot entries**: `.` and `..` ARE returned for a wildcard scan of a subdirectory; they are
  filtered **only at the root of a drive** (file.c:1583-1589: "don't return '.' and '..' in
  the root"). Conformance: tests/file.c:2744-2789 (root `*` must NOT contain them; subdir
  scans do).
- WIN32_FIND_DATAW fill: real attributes (directories carry FILE_ATTRIBUTE_DIRECTORY),
  creation/access/write times, sizes, cAlternateFileName (8.3), dwReserved0 = reparse tag for
  reparse points (:1560-1640).
- Returns a real handle; INVALID_HANDLE_VALUE on any failure.

### FindNextFileW
`BOOL FindNextFileW(h, *data)` — file.c:1535. Iterates the cached NtQueryDirectoryFile
buffer, refilling as needed; exhaustion ⇒ FALSE + **ERROR_NO_MORE_FILES** (:1620 region);
invalid handle ⇒ FALSE + ERROR_INVALID_HANDLE.†

### FindClose
`BOOL FindClose(h)` — file.c:1643. Invalid/foreign handle ⇒ FALSE + ERROR_INVALID_HANDLE;
otherwise closes the directory handle, frees the buffer, TRUE (:1648-1660).

### GetFullPathNameW
`DWORD GetFullPathNameW(name, len, buf, *lastpart)` — file.c:2091. Thin wrapper over
RtlGetFullPathName_U (:2099-2108): resolves against the **process cwd**, collapses `.`/`..`,
handles drive-relative (`C:foo`) and rooted (`\foo`) forms. Return: chars written (no NUL) on
success; required size **including** NUL when the buffer is too small; 0 + ERROR_INVALID_PARAMETER
for NULL name. *lastpart → final component (NULL if the path ends in a separator).

### GetTempPathW
`DWORD GetTempPathW(count, buf)` — file.c:2549. Env fallback chain **TMP → TEMP →
USERPROFILE → GetWindowsDirectoryW** (:2554-2558), full-pathed, trailing `\` appended
(:2573-2577). Fit ⇒ copy, **zero-fill the remainder of the buffer** (:2583-2586, the XP+
behavior), return chars **without** NUL. Too small ⇒ buffer cleared, return chars
**including** NUL (:2588-2592).

### GetCurrentDirectoryW
`UINT GetCurrentDirectoryW(buflen, buf)` — file.c:1762. RtlGetCurrentDirectory_U semantics:
fit ⇒ chars without NUL; too small ⇒ required chars **including** NUL; live process state
(tracks SetCurrentDirectoryW).

### FileTimeToLocalFileTime / FileTimeToSystemTime
- `FileTimeToLocalFileTime` — file.c:4149: local = UTC + RtlQueryTimeZoneInformation bias †
  (via RtlSystemTimeToLocalTime); FALSE on overflow.
- `FileTimeToSystemTime` — file.c:4158: **negative FILETIME ⇒ FALSE +
  ERROR_INVALID_PARAMETER** (:4165-4169); otherwise exact Gregorian decode via
  RtlTimeToTimeFields into {wYear..wMilliseconds, wDayOfWeek} (:4171-4180).

### AreFileApisANSI
`BOOL AreFileApisANSI(void)` — file.c:498: `return !oem_file_apis;` — **TRUE** unless the
process called SetFileApisToOEM. Default TRUE.

### CreateFileMappingW
`HANDLE CreateFileMappingW(file, sa, protect, sizeHi, sizeLo, name)` — sync.c:1032.
- protect → section access map; unknown protect ⇒ 0 + ERROR_INVALID_PARAMETER (:1051-1070).
  SEC_* flags split out; default SEC_COMMIT (:1044-1046).
- file == INVALID_HANDLE_VALUE ⇒ pagefile-backed; **size 0 then ⇒ 0 +
  ERROR_INVALID_PARAMETER** (:1075-1083).
- NtCreateSection; **last-error is ALWAYS set**: ERROR_ALREADY_EXISTS when the named section
  existed (handle still returned), else RtlNtStatusToDosError(status) — 0/SUCCESS on fresh
  create (:1088-1092). Returns NULL (0) on failure, not IHV.

### MapViewOfFile / UnmapViewOfFile
- `MapViewOfFile` — memory.c:278 → MapViewOfFileEx(…, NULL) → NtMapViewOfSection ViewShare.
  FILE_MAP_EXECUTE folded into protection; FILE_MAP_COPY ⇒ PAGE_WRITECOPY (copy-on-write);
  read-only views of a SEC_COMMIT file section are **coherent** with writes made through
  other views/WriteFile (shared section, not a private snapshot).† Offset must be
  allocation-granularity (64K) aligned ⇒ else ERROR_MAPPED_ALIGNMENT.† NULL on failure.
- `UnmapViewOfFile` — memory.c:384: NtUnmapViewOfSection; unknown address ⇒ FALSE
  (+ERROR_INVALID_ADDRESS on the 9x path :386-393; NT path returns the mapped status †).

### CreateIoCompletionPort / GetQueuedCompletionStatus / PostQueuedCompletionStatus
- `CreateIoCompletionPort(handle, port, key, threads)` — sync.c:1214. **port==NULL ⇒ create a
  new port** (NtCreateIoCompletion :1225-1227); THEN, if handle != INVALID_HANDLE_VALUE, the
  file handle is associated via FileCompletionInformation (:1234-1242) — i.e. the
  **combined create+associate call (handle=file, port=NULL) is the canonical form**.
  port!=NULL && handle==IHV ⇒ 0 + ERROR_INVALID_PARAMETER (:1228-1232). Association failure
  closes the fresh port and returns 0.
- `GetQueuedCompletionStatus(port, *count, *key, *ovl, timeout)` — sync.c:1249.
  ***overlapped = NULL first thing** (:1260). NtRemoveIoCompletion. Dequeued packet whose
  iosb.Status is a failure ⇒ returns **FALSE with *overlapped non-NULL** and last-error =
  mapped status (:1262-1268) — the "failed async op" shape. Timeout ⇒ FALSE +
  **WAIT_TIMEOUT** (*ovl stays NULL). INFINITE (0xffffffff) waits forever.
- `PostQueuedCompletionStatus(port, count, key, ovl)` — sync.c:1303: NtSetIoCompletion(port,
  key, ovl, STATUS_SUCCESS, count); the posted packet is indistinguishable from a completed
  I/O with those values. NULL overlapped is legal (sentinel pattern).

### GetOverlappedResult
`BOOL GetOverlappedResult(file, ovl, *result, wait)` — file.c:3352-3387.
- Reads `ovl->Internal` (the NTSTATUS) with acquire semantics (:3362).
- STATUS_PENDING && !wait ⇒ FALSE + **ERROR_IO_INCOMPLETE** (:3365-3369).
- STATUS_PENDING && wait ⇒ WaitForSingleObject(**hEvent if set, else the file handle**,
  INFINITE) (:3370-3377).
- `*result = ovl->InternalHigh`; last-error set from the status; returns TRUE iff status is
  success-class (:3383-3386).

### GetFileType
`DWORD GetFileType(h)` — file.c:3318. Magic STD_*_HANDLE constants resolved via GetStdHandle
(:3323-3326). NtQueryVolumeInformationFile(FileFsDeviceInformation): NULL/console/serial/
parallel/tape/unknown ⇒ FILE_TYPE_CHAR; named pipe ⇒ FILE_TYPE_PIPE; **default ⇒
FILE_TYPE_DISK** (:3333-3348). Failure ⇒ FILE_TYPE_UNKNOWN (0).

### PathFileExistsW
`BOOL PathFileExistsW(path)` — path.c:2554: NULL ⇒ FALSE; else
`GetFileAttributesW(path) != INVALID_FILE_ATTRIBUTES` — so last-error carries the
FILE/PATH_NOT_FOUND split from GetFileAttributesW on the FALSE path (:2556-2562).

---

## 2. DIVERGENCE (spec vs /tmp/alky-shims-lib.rs)

Shim path model: `map_guest_path` (shim:520-527) strips the drive letter and joins the
remainder under `$ALKY_SANDBOX` — a **case-sensitive** linux tree. Handles: files in
`FILE_HANDLES` via `register_file` (shim:469), find-iterators in `FIND_ITERS` (shim:990).

| # | fn | real Windows (wine cite) | ours (shim cite) | severity |
|---|----|--------------------------|------------------|----------|
| 1 | CreateFileW | CREATE_NEW ⇒ fail ERROR_FILE_EXISTS if the file exists (file.c:807,905-912) | disposition match only handles 2 (CREATE_ALWAYS) and 4 (OPEN_ALWAYS); CREATE_NEW(1) and TRUNCATE_EXISTING(5) fall through to a plain open — CREATE_NEW on an existing file **succeeds**, TRUNCATE_EXISTING doesn't truncate (shim:2641) | **TRUST-CHAIN** — fail-if-exists is the lockfile/first-run discriminator; a game's "create only if absent" silently opens the old file |
| 2 | CreateFileW | success with CREATE_ALWAYS-overwrote / OPEN_ALWAYS-opened sets last-error ERROR_ALREADY_EXISTS (file.c:920-923) | last-error untouched on success (shim:2644-2645) | **TRUST-CHAIN** — "did I create it?" branch after OPEN_ALWAYS reads a stale last-error |
| 3 | CreateFileW / GetFileAttributesW/ExW / FindFirstFile* | missing final component ⇒ ERROR_FILE_NOT_FOUND; missing directory component ⇒ ERROR_PATH_NOT_FOUND (file.c:916,1321; tests/file.c:2790-2860) | every miss ⇒ `set_last_error(2)` ERROR_FILE_NOT_FOUND (shim:2646, 1752, 1776, 987) | **TRUST-CHAIN** (named in goal: games branch on which) — "dir missing ⇒ create dir tree" vs "file missing ⇒ create file" logic collapses |
| 4 | CreateFileW | share modes enforced; conflicting open ⇒ ERROR_SHARING_VIOLATION (tests/file.c:741-745) | share mode (r8) ignored entirely (shim:2633-2641) | WRONG-DATA — single-process CP2077 rarely self-conflicts ‡; sharing-violation-as-mutex patterns break |
| 5 | ReadFile | overlapped read at EOF ⇒ FALSE + ERROR_HANDLE_EOF; sync ⇒ TRUE/0 (file.c:3664-3671) | both paths ⇒ TRUE with got=0 (shim:2673-2686 success arm; read_at past EOF returns Ok(0)) | WRONG-DATA — async streamers that terminate on ERROR_HANDLE_EOF instead see infinite TRUE/0 completions ‡ |
| 6 | ReadFile | failure ⇒ FALSE + mapped last-error (file.c:3665-3669) | failure ⇒ ret=0, **last-error not set** (shim:2687-2689) | WRONG-DATA — GetLastError() after a failed read is stale |
| 7 | WriteFile | OVERLAPPED honored: positioned write, Internal/InternalHigh fill, hEvent signal, IOCP post (file.c:4039-4062) | OVERLAPPED (5th arg) **completely ignored**: cursor-positioned write, no iosb fill, no event, no IOCP packet (shim:2692-2704) | **BLOCKER** for async writers — a GetOverlappedResult/GQCS wait on a write never completes; ReadFile got the full treatment (shim:2648-2686), WriteFile didn't |
| 8 | SetFilePointerEx | negative target ⇒ FALSE + ERROR_NEGATIVE_SEEK (file.c:3937-3941); bad method ⇒ ERROR_INVALID_PARAMETER | seek error ⇒ ret=0 with no last-error; method defaults to FILE_BEGIN for any value ≠1,2 (shim:2706-2712) | BENIGN ‡ — CP2077 seeks are computed, not user-input |
| 9 | GetFileType | disk files ⇒ FILE_TYPE_DISK (file.c:3346-3348) | constant 0x0002 FILE_TYPE_CHAR for **every** handle (shim:2381) | **TRUST-CHAIN** — CRT _isatty/buffering decisions + any loader "is this a real disk file" check get CHAR for game files |
| 10 | GetFileInformationByHandleEx | class-dispatched real data; FileIdInfo = {volume serial, 64-bit inode-as-FRN} (file.c:3168-3268; ntdll/unix/file.c:5140-5153) | constant `ret = 0` FALSE, **no last-error** (shim:1835) | **TRUST-CHAIN** (census suspect) — file-identity checks (same-file dedupe, anti-tamper volume-serial+index pairing) fail with an unreadable error; see RET-0 §3 |
| 11 | FindFirstFile* | `.`/`..` returned in subdirectory scans, filtered only at drive root (file.c:1583-1589; tests/file.c:2744-2789) | glob_enumerate = readdir, **never yields dot entries** (shim:435-447) | WRONG-DATA — skip-first-two and entry-count assumptions shift ‡; most scanners filter them anyway |
| 12 | FindFirstFile* | directories carry FILE_ATTRIBUTE_DIRECTORY in find data (file.c:1560-1640) | `fill_find_data` hardcodes attrs=0x80 FILE_ATTRIBUTE_NORMAL for everything (shim:460-463) | **TRUST-CHAIN** — recursive content discovery can't tell dirs from files ⇒ can't recurse; the goal's content-discovery class |
| 13 | FindFirstFile* | find data carries real sizes + 3 filetimes + 8.3 name + reparse tag | zeros for all of it (shim:460-467) | WRONG-DATA — size-based preallocation reads 0 ‡ |
| 14 | FindFirstFile* | `*.*` matches ALL names incl. extensionless; DOS_STAR/DOS_QM/DOS_DOT semantics (file.c:1258-1286; tests/file.c:3066-3160) | literal glob: `*.*` requires a real `.` in the name; `<`,`>`,`"` DOS forms unsupported (shim:449-457) | **TRUST-CHAIN** — a `*.*` asset scan misses extensionless files that Windows returns |
| 15 | FindFirstFile* / all path fns | NTFS lookup is case-insensitive over full Unicode | pattern match is ASCII-only case-insensitive (`eq_ignore_ascii_case`, shim:454); the **directory components are case-sensitive** linux lookups (map_guest_path, shim:520-527) — `Data\*.ini` misses `data/` | **TRUST-CHAIN** (named in goal) — content discovery silently empty on case mismatch; ERROR_FILE_NOT_FOUND masquerades as "no assets" |
| 16 | FindFirstFileExW | missing dir ⇒ ERROR_PATH_NOT_FOUND; `file\*` ⇒ ERROR_DIRECTORY (tests/file.c:2790-2860) | any empty enumeration ⇒ ERROR_FILE_NOT_FOUND(2) (shim:987) | folded into #3 (same class) — counted once |
| 17 | FindNextFileW | invalid handle ⇒ ERROR_INVALID_HANDLE | unknown handle ⇒ ERROR_NO_MORE_FILES(18) (shim:1001-1003) | BENIGN |
| 18 | FindClose | invalid handle ⇒ FALSE + ERROR_INVALID_HANDLE (file.c:1648-1655) | always TRUE (shim:1005) | BENIGN |
| 19 | GetFullPathNameW | resolves `.`/`..`, drive-relative `C:foo`, tracks live cwd (file.c:2099-2108) | prefix-concat against fixed `C:\game\`; **no dot-segment collapsing**; `C:foo` treated as absolute (shim:2311-2335); GetFullPathName**A** absent from dispatch ⇒ miss-path | WRONG-DATA — `..\data\x` stays literal; downstream string-compares on canonical paths mismatch |
| 20 | GetTempPathW | too-small ⇒ required chars **incl** NUL + buffer cleared; fit ⇒ zero-fill tail (file.c:2583-2592) | always returns 8 (`C:\Temp\`), too-small case returns 8 not 9, no zero-fill (shim:2337-2344) | BENIGN — CP2077 passes MAX_PATH buffers ‡ |
| 21 | GetCurrentDirectoryW | live cwd | constant `C:\game`, coherent with GetFullPathNameW's baked cwd; return conventions correct (shim:2354-2363) | BENIGN (coherent-constant) — breaks only if the game SetCurrentDirectory's and reads back ‡ |
| 22 | FileTimeToSystemTime | exact Gregorian decode; negative ft ⇒ ERROR_INVALID_PARAMETER (file.c:4158-4180) | hardcoded epoch base 0x01DB4A5C00000000, writes **year=2025, month=1, day=3** always, only time-of-day decoded; no validation (shim:1532-1546) | WRONG-DATA — save-file timestamps display wrong; branch risk low ‡ |
| 23 | FileTimeToLocalFileTime | UTC + tz bias (file.c:4149) | identity copy (shim:1623-1627) | BENIGN — equals Windows with TZ=UTC; ordering comparisons unaffected |
| 24 | AreFileApisANSI | TRUE unless SetFileApisToOEM (file.c:498-502) | constant 1 (shim:1840) | **PASS** — fully-correct constant (shim never exposes SetFileApisToOEM) |
| 25 | CreateFileMappingW | IHV+size0 ⇒ ERROR_INVALID_PARAMETER; named sections ⇒ ERROR_ALREADY_EXISTS semantics; last-error always set (sync.c:1075-1092) | no size validation (anon size 0 recorded, later mmap'd at ≥0x1000 — succeeds where Windows fails); names ignored; untracked file handle still returns a handle, failure deferred to MapViewOfFile (shim:2611-2630) | BENIGN ‡ — single-process, no named-section IPC in scope |
| 26 | MapViewOfFile | read-only view of a file section is **shared/coherent**; FILE_MAP_COPY = explicit copy-on-write (memory.c:278-330) | read-only ⇒ `MAP_PRIVATE` (shim:2739) — a private snapshot; writes through WriteFile/other views after the map don't appear in it | WRONG-DATA ‡ — subtle; matters only for read-map-while-writing patterns (shader-cache rewrite-in-place) |
| 27 | UnmapViewOfFile | unknown address ⇒ FALSE (+ERROR_INVALID_ADDRESS) (memory.c:384-395) | always TRUE, unknown addr silently ignored (shim:2754-2757) | BENIGN |
| 28 | CreateIoCompletionPort | **port=NULL, handle=file ⇒ create AND associate in one call** — the canonical form (sync.c:1225-1242) | that form hits the else-arm: ret=0 + ERROR_INVALID_HANDLE(6); only (IHV,NULL)=create and (file,port)=associate work (shim:2072-2089) | **BLOCKER** for the combined calling pattern — caller gets NULL port and abandons async I/O ‡ (CP2077 census shows the two-step form worked; other titles/paths may use the combined form) |
| 29 | GetQueuedCompletionStatus | INFINITE waits forever (sync.c:1296-1298 get_nt_timeout) | INFINITE capped at 3,600,000 ms — after 1h idle returns spurious WAIT_TIMEOUT (shim:2103) | WRONG-DATA — robust pools re-loop; brittle ones treat timeout as shutdown ‡ |
| 30 | GetQueuedCompletionStatus | *overlapped NULLed before anything (sync.c:1260); dequeued failed-I/O ⇒ FALSE + ovl≠NULL | *ovl written only on success/timeout paths; invalid-port path leaves it untouched; no failed-packet shape (all completions succeed) (shim:2098-2124) | BENIGN ‡ — shim never produces failed async ops |
| 31 | GetOverlappedResult | full pending/wait/result contract (file.c:3352-3387) | **absent from dispatch** — falls to `handled_real=false` (shim:3207) ⇒ unresolved-import miss path | **BLOCKER** if called (not in CP2077 touched list — untriggered so far; first async-wait user dies) |
| 32 | GetFileAttributesW | full attribute set (readonly/hidden/archive/reparse) | only 0x10 dir / 0x80 normal (shim:1748-1752) | BENIGN ‡ — read-only-check on saves would miss, none observed |
| 33 | PathFileExistsW | FALSE path leaves GetFileAttributesW's error (path.c:2556-2562) | no last-error on FALSE (shim:1739-1743) | BENIGN |

**Divergence count (non-PASS, #16 folded into #3): 24** — of which TRUST-CHAIN: 8
(#1 #2 #3 #9 #10 #12 #14 #15), BLOCKER: 3 (#7 #28 #31), WRONG-DATA: 8, BENIGN: 8 (excluding
the PASS row #24).

What our shim gets RIGHT that's worth keeping (verified against spec):
- ReadFile OVERLAPPED positioned-read via `read_at` + Internal/InternalHigh fill + hEvent
  signal + IOCP post-on-sync-success (shim:2648-2686) — matches file.c:3640-3662 incl. the
  packet-on-success behavior.
- Find handle lifecycle (FIND_ITERS map, ERROR_NO_MORE_FILES=18 on exhaustion, shim:995-1005).
- GetCurrentDirectoryW return conventions (len vs len+1, shim:2354-2363).
- GetFullPathNameW lpFilePart fill + too-small=need+1 (shim:2326-2333).
- CreateFileMappingW/MapViewOfFile real-mmap chain incl. anonymous (IHV) sections and
  size-0 = map-to-EOF (shim:2611-2753).
- GQCS condvar blocking with correct WAIT_TIMEOUT=258 + *ovl=NULL on timeout (shim:2115-2121).

---

## 3. RET-0 GRADING (constant-return arms in our dispatch, this family)

| fn (shim cite) | constant | grade |
|---|---|---|
| GetFileInformationByHandleEx (1835) | `ret=0` FALSE, no last-error | **fake-FAILURE** (the inverse census shape: honest APIs fail with a *reason*). Honest fix, implement-real sketch: `h → FILE_HANDLES fstat`; class 18 FileIdInfo (24-byte out): `VolumeSerialNumber = st_dev as u64` (wine itself uses mountmgr-or-0, ntdll/unix/file.c:5148 — st_dev is strictly better than wine's 0-fallback), `FileId[0..8] = st_ino`, rest zero. Class 0 FileBasicInfo: 4 filetimes from mtime + attrs. Class 1 FileStandardInfo: {AllocationSize, EndOfFile = len, NumberOfLinks=1, DeletePending=0, Directory}. Unknown class ⇒ FALSE + `set_last_error(87)` ERROR_INVALID_PARAMETER — die-loud-shaped, not silent. ~30 lines, kills the whole file-identity divergence (#10). |
| AreFileApisANSI (1840) | `ret=1` | **fully-correct-constant** — real Windows default; SetFileApisToOEM unexposed. PASS. |
| GetFileType (2381) | `ret=2` CHAR | **fake-success** (wrong constant for disk handles). Honest fix: `if FILE_HANDLES contains h ⇒ 1 (DISK) else 2 (CHAR)` — one lookup, matches file.c:3346-3348 for every handle class we create. |
| FindClose (1005) | `ret=1` always | correct for tracked handles; constant-TRUE for foreign handles = benign fake-success (Windows: FALSE+ERROR_INVALID_HANDLE). Acceptable; die-loud not warranted. |
| FileTimeToLocalFileTime (1623) | copy + `ret=1` | **fully-correct-constant under TZ=UTC** (bias=0 is a real Windows configuration). PASS with note. |
| FileTimeToSystemTime (1532) | `ret=1` + fake calendar fill | **fake-success** (WRONG-DATA fill, #22). Honest fix: real days-since-1601 Gregorian decode (~15 lines, civil-from-days) — no reason to keep the 2025-01-03 hardcode. |
| UnmapViewOfFile (2754) | `ret=1` always | real munmap for tracked views; constant-TRUE for unknown = benign (matches the "no 9x check" NT path closely enough). PASS-ish. |
| GetTempPathW (2337) / GetCurrentDirectoryW (2354) / GetFullPathNameW cwd (2311) | `C:\Temp\` / `C:\game` | **fully-correct-constants within the sandbox world-model** — they round-trip through map_guest_path and agree with each other; GetTempPathW even `create_dir_all`'s the backing dir. PASS. Residual: GetFullPathNameW's missing dot-collapse is #19, a logic gap not a constant-return issue. |
| GetOverlappedResult (— absent) | n/a — unresolved-import miss (3207 fallthrough) | correctly die-loud today. When WriteFile grows its OVERLAPPED arm (#7), implement for real: read `ovl.Internal`, PENDING+!wait ⇒ set_last_error(996) ERROR_IO_INCOMPLETE + FALSE; PENDING+wait ⇒ wait on hEvent-else-file; `*result = ovl.InternalHigh` — file.c:3352-3387 is ~20 lines to port. |

Census-suspect verdict (GetFileInformationByHandleEx): the constant FALSE is the *safest
wrong answer available* (games do generally carry a fallback for it — it fails on FAT32 too‡)
but it is silently stale-error'd and it kills the file-identity class. Priority: implement
FileIdInfo + FileStandardInfo per the sketch; the two cover the observed anti-tamper and
streaming-metadata uses.†
