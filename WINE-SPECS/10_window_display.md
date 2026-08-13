# WINE-SPECS 10: window/display/GDI family — the census-delta's [A] walk
(horizon, 2026-08-13; sera's ·1824 both-sides commission. Form per WAVE-CONTRACT: per-API
{SPEC = the wine/Windows contract w/ source-cite, OURS = alky-shims behavior w/ line-cite,
VERDICT}. Our arms live in alky-shims/src/lib.rs [the shared crate] + shims_user32.rs [loader].
Context: headless boot — the game must reach the renderer, not render a desktop.)

## GetSystemMetrics (lib.rs:2843)
SPEC (wine win32u/sysparams.c:7158+): per-index real answers — the boot-relevant set:
SM_CXSCREEN(0)/CYSCREEN(1)=primary-rect; SM_CMONITORS(80)=count(≥1!); SM_CXVIRTUALSCREEN(78)/
CY(79)=virtual-rect; SM_MOUSEWHEELPRESENT(75)=1; SM_CXICON(11)/CYICON(12)=32; SM_CXCURSOR(13)/
CYCURSOR(14)=32; SM_CYCAPTION(4)=~19; SM_REMOTESESSION(0x1000)=0; SM_DIGITIZER(94)=0.
OURS: {0→1920, 1→1080, EVERYTHING ELSE→0}.
VERDICT: **DIVERGENT-RISKY** — SM_CMONITORS→0 = "zero monitors" (a renderer-init reading it
gets nonsense); SM_CXVIRTUALSCREEN→0 likewise. Windows NEVER returns 0 for those on a real
session. FIX: the ~12-index table above (one match-arm each; 0 stays for the exotic tail w/
a log-once). TEST-ROW: dump indices {0,1,4,11,75,78,79,80,94,0x1000} vs golden.

## EnumDisplayDevicesW/A (lib.rs:2803)
SPEC (wine win32u/sysparams.c NtUserEnumDisplayDevices): iDevNum 0 → the adapter
(DeviceName="\\.\DISPLAY1", DeviceString, StateFlags ATTACHED|PRIMARY(0x5)); iDevNum≥count →
FALSE. W-form: WCHAR names, cb=struct-size honored.
OURS: [needs the block read — 2803-2822 fills DISPLAY_DEVICE w/ \\.\DISPLAY1 + flags] — read
at walk-time: fills name + StateFlags=0x5, iDevNum>0 → 0. VERDICT: **MATCH-shape** (verify
the DeviceString non-empty + cb-respect; the suite row = dump iDevNum 0,1).

## EnumDisplaySettings (lib.rs:2754)
SPEC (wine): iModeNum=ENUM_CURRENT_SETTINGS(-1) → current DEVMODE; 0..N = the mode list;
>N → FALSE. dmFields must carry PELSWIDTH|PELSHEIGHT|BITSPERPEL|DISPLAYFREQUENCY.
OURS: -1 → 1920×1080@60×32bpp; 0..11 = a 12-mode ladder; >11 → 0. dmFields = 0x5c0020|0x80000|
0x100000|0x400000 [POSITION|BITSPERPEL|PELSWIDTH|PELSHEIGHT|FLAGS|FREQUENCY-ish].
VERDICT: **MATCH-shape** (the dmFields mask composition looks over-broad but harmless; the
A-form fill = verify ANSI struct offsets differ [dmSize@36 vs @68]! — the wide/narrow split
in the arm handles it? READ SHOWS: `let wide = sym.contains("W")` + only the W-offsets written
= **the A-form writes W-offsets = DIVERGENT if any A-caller exists** [census shows EnumDisplaySettingsW only → LATENT, log-once + fix when an A-caller appears]).

## EnumDisplayMonitors (lib.rs:2743)
SPEC (wine): calls the callback per-monitor w/ (HMONITOR, HDC, LPRECT, LPARAM); returns TRUE
if all callbacks returned TRUE.
OURS: ONE monitor {0,0,1920,1080}, callback invoked, ret=1 unconditionally.
VERDICT: **MATCH** for the single-monitor headless model (the callback-transmute = the
guest-call path — correct; ignoring the callback's continue-value = harmless at n=1).

## SystemParametersInfoW (lib.rs:2724)
SPEC (wine): per-uiAction out-fills; SPI_GETWORKAREA → the primary work-rect.
OURS: WHEELSCROLLLINES=3, WORKAREA={0,0,1920,1080}, font-smoothing=on, UNKNOWN → write-u64(0)+ret 1.
VERDICT: **MATCH-shape w/ a fail-open tail** — unknown-SPI writes 8 zero bytes + claims success:
a caller w/ a SMALLER out-buffer gets stack-scribble (the r8≥0x1000 guard helps but size is
unknowable). Windows returns FALSE for unsupported actions. FIX: unknown → ret=0 + log-once
(fail-CLOSED per the ·1599 law). TEST-ROW: an unknown SPI → FALSE.

## RegisterClassExW / CreateWindowExW/A (lib.rs:4806/4814 + shims_user32)
SPEC: atom≠0; HWND≠0; WM_NCCREATE/WM_CREATE delivered to the wndproc SYNCHRONOUSLY during
CreateWindowEx (wine win.c — games do run code in those handlers!).
OURS: atom=sequence; HWND minted + dims stored; **wndproc-call during create = READ THE ARM**
(4814-4871 — if we don't deliver WM_CREATE, engines that build state there silently lack it).
VERDICT: **VERIFY-ROW** — the create-message delivery = the load-bearing question; the suite
row = a wndproc that logs its messages + asserts WM_NCCREATE/WM_CREATE arrived pre-return.

## DefWindowProcW (shims_user32:119 + lib.rs:4872)
SPEC: NOT constant-0 — e.g. WM_NCCREATE → TRUE(1) [returning 0 CANCELS window creation!],
WM_ERASEBKGND → nonzero-if-erased, WM_GETMINMAXINFO → 0-after-filling.
OURS: constant 0.
VERDICT: **DIVERGENT-RISKY** — if the game's wndproc forwards WM_NCCREATE to DefWindowProc
(the standard pattern) and consumes our 0 as create-failure → CreateWindowEx returns NULL on
real-Windows-semantics. Composes w/ the VERIFY-ROW above. FIX: the ~6-message table
(NCCREATE→1, NCCALCSIZE→0, ERASEBKGND→1, SETCURSOR→...); tail → 0.

## GetWindowRect/GetClientRect (lib.rs:4873) — OURS: the stored create-dims. MATCH-shape.
## AdjustWindowRect(Ex) (lib.rs:4878) — OURS: rect-unchanged+TRUE. SPEC: expands by the
style's decorations. VERDICT: ACCEPTABLE-headless (borderless-fullscreen game; log-once).
## ShowWindow/UpdateWindow/SetForegroundWindow (4871) — ret=1. MATCH-headless.
## GetDesktopWindow (4880) → fake 0xDE5C; GetForegroundWindow (4881) → HWND_MAIN. MATCH-shape.
## LoadCursorW/LoadIconW/SetCursor (4957) → 0x10C0 const. MATCH-headless (nonzero = the contract).
## GetStockObject (2742) → 0x7300_0000|idx. MATCH-shape (opaque handles; nonzero for valid idx —
   wine returns per-object handles; only == comparisons across calls matter = stable ✓).
## BeginPaint/EndPaint/FillRect (4996/5006/5007) — PAINTSTRUCT filled w/ our HDC + client rect;
   ret=1. MATCH-headless.
## DestroyWindow (4979) — unregisters + ret=1. MATCH.
## SendMessageW/PostMessageW (2820) — [read: routes to the pump/wndproc?]. VERIFY-ROW: SendMessage
   = SYNCHRONOUS wndproc call (wine: direct dispatch same-thread); if ours queues it, ordering breaks.
## GetWindowLongPtr/SetWindowLongPtr (4965/4973) — per-window slot store incl GWLP_WNDPROC/USERDATA.
   MATCH-shape (verify GWLP_WNDPROC returns the REGISTERED wndproc pre-set).
## RegisterDeviceNotificationW (2723) → opaque HDEVNOTIFY. MATCH-headless.
## RegisterRawInputDevices (2741) → TRUE. MATCH-headless (input via pump).
## timeBeginPeriod/timeEndPeriod (2857) → TIMERR_NOERROR(0). MATCH (we schedule at OS granularity).
## timeGetDevCaps (2853) → {1ms, 1000000ms}. MATCH (wine: {1,65535} — ours over-wide upper; harmless).

## THE PRIORITY FIXES FROM THIS WALK (both-sides confidence per ·1825):
1. **DefWindowProcW constant-0** (WM_NCCREATE-cancel class = a REAL boot-risk) — the message-table.
2. **GetSystemMetrics 0-tail** (SM_CMONITORS=0 = "no monitors") — the 12-index table.
3. **SystemParametersInfo unknown→fail-open** — flip to FALSE+log (the ·1599 law).
4. The CreateWindowEx WM_CREATE-delivery VERIFY row (suite) + SendMessage-synchronicity row.
5. EnumDisplaySettingsA writes W-offsets (LATENT — log-once until an A-caller exists).
