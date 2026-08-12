# Wine-spec wave — per-agent contract (v1, after the 01_window_messages pilot)

Deliverable per family: WINE-SPECS/NN_family.md with three sections:

## 1. SPEC (per function)
For each assigned function: the REAL-Windows contract as Wine encodes it —
{args, return-value shape (success AND failure values), out-param fills,
side-effects the game can observe (last-error, queue-state, handle-state),
ordering guarantees}. Cite wine file:line. Wine's conformance tests
(dlls/*/tests/*.c) OUTRANK wine's implementation where they disagree;
todo_wine marks = wine itself diverges = the test's expectation is the truth.

## 2. DIVERGENCE table (spec vs our shim)
Row per difference: {fn, what real Windows answers, what ours answers,
file:line both sides, severity: TRUST-CHAIN (a consumer makes a trust/branch
decision on it — the GetFileVersionInfoSize class) / BLOCKER (caller hangs
or crashes) / WRONG-DATA / BENIGN}. Constant answers that ARE the honest
real-Windows answer (CoInitializeEx→S_OK, IsDebuggerPresent→0) = PASS, say so.

## 3. RET-0 GRADING (sera's ·1599 ruling)
Every constant-return arm in our shim for your functions, graded:
{fully-correct-constant | fake-success → name the honest fix (implement-real
sketch or die-loud)}. The 10 census suspects (VirtualQuery,
GetFileInformationByHandleEx, LCMapString/GetStringType, GetLocaleInfoEx/W,
SuspendThread, GetFileVersionInfo*/VerQueryValue) get priority depth.

Sources: wine @/local/home/seratb/wine (master, depth-1);
our shim @/tmp/alky-shims-lib.rs (3211 lines, the dispatch match);
touched-fn list @WINE-SPECS/touched_apis_cp2077.txt.
Format reference: WINE-SPECS/01_window_messages.md (the pilot).
Cite everything file:line. † for source-only claims, ‡ for inferences.
