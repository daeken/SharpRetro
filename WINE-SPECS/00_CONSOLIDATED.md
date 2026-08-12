# Wine-spec wave — consolidated verdict table (7 of 8 families landed)

**The question this answers ("is every exposed API wine-identical or fail-loud?"):
NO — the wave found ~213 divergence rows across all 8 families, of which
~44 are TRUST-CHAIN class (a consumer makes a trust/branch decision on the
wrong answer — the version-info-root's class) and ~19 BLOCKER-class. ~70 rows
graded honest-PASS (correct constants / matching implementations), credit
where due.**
Marker-counts are upper bounds (some rows carry two labels); per-spec deduped
counts live in each file's own summary line.

| family | fns | rows | trust-chain | blocker | headline |
|---|---|---|---|---|---|
| 01 window/messages (pilot) | ~12 | 18 | (6 graded blocker-class) | 6 | no QS_* state; sent-class msgs wrongly posted; WINDOWPOS lParam=0 |
| 02 sync | 24 | 20 | 3 | 1 | named-object ALREADY_EXISTS never set; WFSO untracked-handle=signaled; CV timeout wrong last-error |
| 03 file-io | 26 | 24 | 8 | 3 | CREATE_NEW falls through; FNF/PNF collapsed; GetFileType=CHAR; FindFirst can't see dirs; OVERLAPPED writes ignored |
| 04 memory+version | 13 | 27 | 5 | 0 | VirtualQuery reserved-vs-committed + AllocationBase + Type=IMAGE all wrong; VirtualProtect constant-TRUE w/ fake old-prot; GFVIS size formula ≠ (len×2)+4 |
| 05 locale/text | 12 | 13 | 5 | 0 | GetLocaleInfoEx/LCMapString/GetStringType constant-0 (the census suspects, confirmed vs wine column); MB/WC flags+lengths ignored; EnumSystemLocales success-with-zero-callbacks |
| 07 thread/proc | 15 | 12 | 2 | 1 | SuspendThread fake-success (census-confirmed); topology self-contradictory (1 core-record carrying 8 LPs — pool-sizing reads 1) |
| 08 time/sysinfo | 16 | 14 | 5 | 1 | GetSystemTime FROZEN at 2025-01-01 while FILETIME APIs run real = two disagreeing nows; SystemTimeToFileTime ignores the date; VerifyVersionInfo + IsProcessorFeaturePresent constant-TRUE |
| 09 misc/loader/COM | ~40 | 50 | 7 | 1 | GetLastError process-GLOBAL (cross-thread bleed); LoadLibrary fake-handles + GetModuleHandle always-exe = impossible-state pair; all KNOWNFOLDERIDs collapse to one dir; CloseHandle never validates; deterministic 'random' |

## The fix-queue, trust-chain-first (the ranked union)
1. **Clock coherence (08 r1/r2/r3/r4)** — GetSystemTime frozen-2025 vs real-epoch
   FILETIME: two APIs disagree about NOW by 1.5 years; the conversion pair
   ignores dates entirely. One shared clock-source fix.
2. **VirtualQuery fidelity (04 D1/D2/D5)** — reserved-vs-committed, AllocationBase,
   MEM_IMAGE Type: the allocator/anti-tamper walk discriminators (top census
   suspect, now spec'd against wine's exact fill).
3. **Locale/text family (05 D1-D5)** — the constant-0 quintet: language-selection
   + CRT ctype degrade; the regional-content chain reads GetLocaleInfoEx.
4. **File-identity + error-shape (03)** — CREATE_NEW semantics, ALREADY_EXISTS
   last-error, FNF/PNF split, GetFileType, FindFirst attrs/wildcards/case.
5. **Named-object semantics (02 r1)** — ALREADY_EXISTS + same-object-by-name.
6. **CS owner-tid id-space mismatch (02 r14)** + CV timeout last-error (r4, one-line).
7. **VirtualProtect real mprotect + old-prot round-trip (04 D12)**.
8. **Topology coherence (07 D10/D11)** — one record per core, SMT via flags.
9. **VerifyVersionInfo/IsProcessorFeaturePresent real tables (08 r5/r6)**.
10. Window-family QS_* + message-classification (01 — interacts with the
    CW_* behavior switches; fix alongside the window organ's next pass).
11. **Per-thread GetLastError (09)** — currently a process-global Mutex:
    cross-thread error bleed corrupts every call-then-check under threads.
    One TEB field. Arguably belongs at #2 — it's every-API-wide.
12. Module-registry coherence (09) — LoadLibrary fake-handles +
    GetModuleHandle-always-exe contradict each other (impossible state on
    real Windows); one registry makes both honest.
13. Distinct KNOWNFOLDERID roots + CloseHandle validation + real entropy
    (BCryptGenRandom rides splitmix-over-det-ticks — 3-line getrandom fix).

Blocker-class-but-unreached rows (OVERLAPPED writes, WFMO abandonment, IOCP
combined-form) = die-loud candidates per the all-stubs ruling rather than
implement-real, until a title exercises them.

Sources: per-family specs in this directory; each row cites wine file:line +
shim file:line. Wine conformance tests outrank wine impl where they disagree.
