# 01 — CreateWindowEx / Window-Lifecycle Message-Sequence Contract

**Oracle:** Wine master (`/local/home/seratb/wine`). **Subject shim:** Alky
`~/alky-shims/src/lib.rs` (snapshot `/tmp/alky-shims-lib.rs`, 3006 lines).
**Rule of precedence:** Wine's *tests* (`dlls/user32/tests/msg.c`) encode
real-Windows behavior and win. `todo_wine`/`msg_todo` markers = Wine's
implementation is wrong there and the test's expectation is the truth.
Marks: `†` = source-only, runtime-unverified. `‡` = inferred, not directly observed.

---

## 0. Legend and the SENT-vs-POSTED distinction (the whole game)

The Wine test tables (`msg.c:144-158`) tag every message with flags:

| flag | value | meaning |
|------|-------|---------|
| `sent` | 0x1 | **SENT** — delivered by a synchronous wndproc call that bypasses the queue. **Invisible to `PeekMessage`.** |
| `posted` | 0x2 | **POSTED** — enqueued; `PeekMessage`/`GetMessage` retrieve it; sets a `QS_*` bit. |
| `parent` | 0x4 | sent to the parent, not the window |
| `wparam`/`lparam` | 0x8/0x10 | the table asserts this param value |
| `defwinproc` | 0x20 | originates from `DefWindowProc`, not the app |
| `beginpaint` | 0x40 | delivered inside `BeginPaint` |
| `optional` | 0x80 | may or may not appear (Windows-version variance) |
| `hook`/`winevent_hook` | 0x100/0x200 | CBT / WinEvent hook callouts, not window messages |
| `msg_todo` | 0x800 | Wine gets this wrong; the table row is the real-Windows truth |

**The load-bearing fact for this family:** *every* message in the
CreateWindowEx→ShowWindow→DestroyWindow lifecycle is flagged `sent`, **not
`posted`** (verify: `grep -c posted` across `WmCreateOverlappedSeq`,
`WmShowOverlappedSeq`, `WmDestroyOverlappedSeq` = **0** posted rows;
`msg.c:186-196`, `474-517`, `729-746`). The lifecycle is a chain of
synchronous wndproc calls made on the creating thread *before the API
returns*. Nothing about window creation lands in the pump's queue.

The only things a `PeekMessage(NOREMOVE)` issued *after* a fully created+shown
window can retrieve are the **synthesized/posted** residue:
- **`WM_PAINT`** — never stored; synthesized on demand when the window has a
  non-empty update region (`server/queue.c:3366-3378`; peek check is
  `queue->paint_count && find_window_to_repaint(...)`).
- **`WM_TIMER`** — synthesized from expired timers (`server/queue.c:3380-3392`).
- genuinely posted messages / hardware input (there are none from creation).

---

## 1. SPEC — real-Windows message sequences (cited to Wine)

### (a) CreateWindowEx of an **overlapped, WS_VISIBLE** window

This is CreateWindowEx (create phase) **immediately followed by** the internal
`ShowWindow` the WS_VISIBLE bit triggers. Wine splits it into two tables:
`WmCreateOverlappedSeq` (`msg.c:186-196`) then the SetWindowPos(SWP_SHOWWINDOW)
show path `WmShowOverlappedSeq`-shaped (`msg.c:474-517`). Combined ordered
contract:

| # | message | 0x | S/P | sync/queued | wparam/lparam notes | cite |
|---|---------|----|-----|-------------|---------------------|------|
| 1 | `HCBT_CREATEWND` | — | hook | sync callout | CBT hook, not a window msg | `msg.c:187` |
| 2 | **`WM_GETMINMAXINFO`** | 0x24 | **sent** | sync | **the FIRST window message.** lParam→MINMAXINFO | `msg.c:188`; impl `win32u/window.c:6010-6016` |
| 3 | `WM_NCCREATE` | 0x81 | sent | sync | lParam = &CREATESTRUCT | `msg.c:189`; impl `win32u/window.c:6037-6040` |
| 4 | `WM_NCCALCSIZE` | 0x83 | sent\|wparam | sync | **wParam=FALSE(0)** at create; rect in screen coords | `msg.c:190`; impl `win32u/window.c:6055-6065` |
| 5 | `0x0093`,`0x0094` | — | sent\|defwinproc\|optional | sync | internal (WM_GETTEXT/uahmenu family), version-optional | `msg.c:191-192` |
| 6 | `WM_CREATE` | 0x01 | sent | sync | lParam = &CREATESTRUCT | `msg.c:194`; impl `win32u/window.c:6074-6075` |
| 7 | `WM_SIZE` | 0x05 | sent\|wparam | sync | **wParam=SIZE_RESTORED(0)**, lParam=MAKELONG(cx,cy) client | `msg.c:511`(guarded)/`754`; impl `win32u/window.c:6090` |
| 8 | `WM_MOVE` | 0x03 | sent | sync | lParam=MAKELONG(x,y) client-origin | `msg.c:512`/`755`; impl `win32u/window.c:6091` |
| — | *(WS_VISIBLE now drives the internal ShowWindow: rows below)* | | | | | `win32u/window.c:6095-6110` |
| 9 | `WM_SHOWWINDOW` | 0x18 | sent\|wparam | sync | **wParam=1 (TRUE)** | `msg.c:475` |
| 10 | `WM_WINDOWPOSCHANGING` | 0x46 | sent\|wparam | sync | wParam flags=`SWP_SHOWWINDOW\|SWP_NOSIZE\|SWP_NOMOVE`; lParam→WINDOWPOS | `msg.c:477` |
| 11 | `WM_NCPAINT` | 0x85 | sent\|wparam\|optional | sync | wParam=1 (update-region hrgn) | `msg.c:476,479` |
| 12 | `WM_ERASEBKGND` | 0x14 | sent\|optional | sync | wParam = HDC | `msg.c:481` |
| 13 | `WM_ACTIVATEAPP` | 0x1C | sent\|wparam\|optional | sync | **wParam=1 (activating)**, lParam=other-thread-id | `msg.c:487` |
| 14 | `WM_NCACTIVATE` | 0x86 | sent\|wparam\|optional | sync | wParam=1 (active) | `msg.c:488` |
| 15 | `WM_ACTIVATE` | 0x06 | sent\|wparam\|optional | sync | **wParam=WA_ACTIVE(1)**, lParam=prev-hwnd(0) | `msg.c:490` |
| 16 | `HCBT_SETFOCUS` | — | hook\|optional | sync | CBT hook | `msg.c:491` |
| 17 | `WM_IME_SETCONTEXT` | 0x281 | sent\|wparam\|defwinproc\|optional | sync | wParam=1 | `msg.c:492` |
| 18 | `WM_SETFOCUS` | 0x07 | sent\|wparam\|defwinproc\|optional | sync | wParam=prev-focus-hwnd(0); **comes AFTER activate** | `msg.c:495` |
| 19 | `WM_WINDOWPOSCHANGED` | 0x47 | sent\|wparam | sync | flags=`SWP_SHOWWINDOW\|SWP_NOSIZE\|SWP_NOMOVE\|SWP_NOCLIENTSIZE\|SWP_NOCLIENTMOVE` | `msg.c:501` |
| 20 | `WM_PAINT` | 0x0F | sent\|optional | **synth/queued** | *first-paint*; delivered via pump if app doesn't force it | `msg.c:514` |

**Activation/focus ORDER (critical):** `WM_ACTIVATEAPP` → `WM_NCACTIVATE` →
`WM_ACTIVATE` → (`HCBT_SETFOCUS`) → `WM_SETFOCUS`. Focus lands **after**
activation, never before (`msg.c:487-495`).

### (b) CreateWindowEx **WITHOUT WS_VISIBLE**

Only the create phase fires — rows 1-8 above (`WmCreateOverlappedSeq`,
`msg.c:186-196`, terminating at `WM_CREATE`; `WM_SIZE`/`WM_MOVE` per
`msg.c:754-755`). **No `WM_SHOWWINDOW`, no activation, no focus, no paint.**
The window exists, is not shown, is not active, has no invalid region. A
`PeekMessage(NOREMOVE)` immediately after sees **nothing** (empty queue) — and
that is *correct* on real Windows here. The invisible-popup analogue is
`WmCreateInvisiblePopupSeq` (`msg.c:1102`), which likewise stops at `WM_CREATE`
with no show/activate tail.

### (c) ShowWindow(SW_SHOW) on a previously-invisible overlapped window

Exactly `WmShowOverlappedSeq` (`msg.c:474-517`) — rows 9-20 of table (a),
all `sent`. Sequence gist: `WM_SHOWWINDOW(1)` → `WM_WINDOWPOSCHANGING(SWP_SHOWWINDOW)`
→ (NCPAINT/ERASE optional) → `WM_ACTIVATEAPP(1)` → `WM_NCACTIVATE(1)` →
`WM_ACTIVATE(1)` → `WM_SETFOCUS` → `WM_WINDOWPOSCHANGED(SWP_SHOWWINDOW|…NOCLIENT…)`
→ `WM_PAINT`(optional, via pump).

### (d) DestroyWindow — `WmDestroyOverlappedSeq` (`msg.c:729-746`)

| # | message | 0x | S/P | sync/queued | wparam/lparam notes | cite |
|---|---------|----|-----|-------------|---------------------|------|
| 1 | `HCBT_DESTROYWND` | — | hook | sync | CBT hook | `msg.c:730` |
| 2 | `0x0090` | — | sent\|optional | sync | internal (uahdestroywindow), optional | `msg.c:731,734` |
| 3 | `WM_WINDOWPOSCHANGING` | 0x46 | sent\|wparam | sync | flags=`SWP_HIDEWINDOW\|SWP_NOACTIVATE\|SWP_NOSIZE\|SWP_NOMOVE` | `msg.c:732` |
| 4 | `WM_WINDOWPOSCHANGED` | 0x47 | sent\|wparam | sync | +`SWP_NOCLIENTSIZE\|SWP_NOCLIENTMOVE` | `msg.c:735` |
| 5 | `WM_NCACTIVATE` | 0x86 | sent\|optional\|wparam | sync | **wParam=0 (deactivate)** | `msg.c:736` |
| 6 | `WM_ACTIVATE` | 0x06 | sent\|optional | sync | deactivation | `msg.c:737` |
| 7 | `WM_ACTIVATEAPP` | 0x1C | sent\|optional\|wparam | sync | **wParam=0** | `msg.c:738` |
| 8 | `WM_KILLFOCUS` | 0x08 | sent\|optional\|wparam | sync | wParam=0 | `msg.c:739` |
| 9 | `WM_DESTROY` | 0x02 | sent | sync | — | `msg.c:743` |
| 10 | `WM_NCDESTROY` | 0x82 | sent | sync | **LAST message; the window dies here** | `msg.c:744` |

**Destroy hides-then-deactivates-then-destroys.** The `WM_WINDOWPOSCHANGING/
CHANGED(SWP_HIDEWINDOW)` pair precedes deactivation, which precedes
`WM_DESTROY`→`WM_NCDESTROY`.

### W-vs-A class-string encoding in CREATESTRUCT

- `CreateWindowExA`/`W` differ **only** in how the class/title strings reach the
  wndproc and what encoding `CREATESTRUCT.lpszClass`/`lpszName` carry when the
  wndproc that receives `WM_NCCREATE`/`WM_CREATE` is A- vs W-registered.
- `lpszClass` may be a **string pointer** *or* an **atom** — an atom is
  `IS_INTRESOURCE` (high 16 bits zero, value ≤ 0xFFFF), passed as
  `MAKEINTATOM`. The wndproc must test `IS_INTRESOURCE(cs->lpszClass)` before
  dereferencing (`user32/win.c` CreateWindowEx path; classes resolved by atom
  in `win32u`).
- The `CREATESTRUCT` delivered to the wndproc is transcoded to match the
  *target wndproc's* charset: a W-window created by `CreateWindowExA` receives
  a `CREATESTRUCTW` with widened strings, and vice-versa. The `ansi` bool
  threaded through `send_message_timeout(..., ansi)` at NCCREATE/CREATE
  (`win32u/window.c:6040,6075`) selects the mapping.
- **Practical shim contract:** `lpParam` (CreateWindowEx arg-12) must land at
  `CREATESTRUCT.lpCreateParams` (offset 0). The shim already does this
  (`lib.rs:2808`) — this is the field the game's window factory reads at
  `WM_NCCREATE` to recover its C++ `this` (see divergence table).

---

## 2. DIVERGENCE TABLE (Wine spec ⟷ Alky shim)

Shim create arm: `lib.rs:2789-2846`. PeekMessage: `lib.rs:2861-2884`.
GetMessage: `lib.rs:2886-2934`. DispatchMessage: `lib.rs:2937-2951`.
DestroyWindow: `lib.rs:2957-2970`.

| # | area | Wine spec (real Windows) | Alky shim | severity | cite (spec / shim) |
|---|------|--------------------------|-----------|----------|--------------------|
| D1 | **WM_GETMINMAXINFO missing** | `WM_GETMINMAXINFO(0x24)` is the **FIRST** window message sent, before NCCREATE | shim never sends 0x24; NCCREATE is first | **BLOCKER-CLASS**‡ (a factory sizing its window from MINMAXINFO gets zeros) | `msg.c:188` / `lib.rs:2813` (jumps straight to 0x81) |
| D2 | **WM_NCCALCSIZE missing** | `WM_NCCALCSIZE(0x83), wParam=FALSE` sent between NCCREATE and CREATE | shim never sends 0x83 | **WRONG-DATA** (client rect never negotiated; DefWindowProc would set it) | `msg.c:190` / `lib.rs:2813-2814` (no 0x83 between) |
| D3 | **SENT vs POSTED misclassification** | SHOWWINDOW/WINDOWPOS/SIZE/ACTIVATE/SETFOCUS are all **`sent`** (synchronous, invisible to Peek) | shim **both** sends them once (`lib.rs:2819-2822`) **and POSTS copies to MSG_QUEUE** (`lib.rs:2827-2833`) → they appear in Peek | **WRONG-DATA / BLOCKER-CLASS**‡ | `msg.c:475-501` (all `sent`) / `lib.rs:2819-2838` |
| D4 | **Activation/focus order** | `WM_ACTIVATEAPP(0x1C)` → `WM_NCACTIVATE(0x86)` → `WM_ACTIVATE(0x06)` → `WM_SETFOCUS(0x07)` | shim sends only `WM_ACTIVATE(0x06)` then `WM_SETFOCUS(0x07)`; **no ACTIVATEAPP, no NCACTIVATE**; queue also lacks them | **WRONG-DATA** (app tracking focus-gain sees partial set) | `msg.c:487-495` / `lib.rs:2821-2822,2832-2833` |
| D5 | **WM_ACTIVATE wparam** | wParam = `WA_ACTIVE(1)` in **low word**, high word = minimized-flag(0) → full DWORD `1` | shim sends wParam=`1` (correct low word) | **BENIGN** (matches) | `msg.c:490` / `lib.rs:2821` |
| D6 | **WM_WINDOWPOSCHANGING/CHANGED wparam=0** | wParam carries **SWP_ flags** (SHOWWINDOW/NOSIZE/NOMOVE…); lParam→WINDOWPOS struct | shim posts 0x46/0x47 with **wParam=0, lParam=0** (`lib.rs:2829-2830`) — no flags, no WINDOWPOS ptr | **WRONG-DATA / BLOCKER-CLASS**‡ (app reading WINDOWPOS* derefs NULL) | `msg.c:477,501` / `lib.rs:2829-2830` |
| D7 | **NCPAINT/ERASEBKGND/PAINT are SENT during BeginPaint, not queued msgs** | `WM_NCPAINT(0x85)`+`WM_ERASEBKGND(0x14)` are **sent inside BeginPaint**; `WM_PAINT(0x0F)` is **synthesized** from `paint_count`, never a stored queue entry | shim **posts** 0x85, 0x14, 0x0F as literal MSG_QUEUE rows (`lib.rs:2836-2838`) | **WRONG-DATA** (Peek sees NCPAINT/ERASE as pumpable msgs; real Windows delivers those only via BeginPaint) | `server/queue.c:3366-3378`, `win32u/message.c:4074-4076` / `lib.rs:2836-2838` |
| D8 | **WM_ERASEBKGND wparam is HDC** | wParam = a valid HDC | shim posts wParam=`0x7400_0001` (its fake HDC) | **BENIGN** (self-consistent with BeginPaint's HDC) | `msg.c:481` / `lib.rs:2837` |
| D9 | **Destroy: hide-pos pair missing** | `WM_WINDOWPOSCHANGING/CHANGED(SWP_HIDEWINDOW)` precede deactivation | shim sends only `WM_DESTROY(0x02)`+`WM_NCDESTROY(0x82)` | **WRONG-DATA** (app expecting hide-before-destroy misses it) | `msg.c:732-735` / `lib.rs:2963-2964` |
| D10 | **Destroy: deactivation set missing** | `WM_NCACTIVATE(0)`,`WM_ACTIVATE`,`WM_ACTIVATEAPP(0)`,`WM_KILLFOCUS(0)` (optional) before WM_DESTROY | shim sends none | **BENIGN→WRONG-DATA**‡ (all `optional` in Wine's table) | `msg.c:736-739` / `lib.rs:2963-2964` |
| D11 | **Destroy is SENT, correct** | `WM_DESTROY`,`WM_NCDESTROY` both `sent`, in that order, NCDESTROY last | shim sends 0x02 then 0x82 synchronously — **matches** | **BENIGN** (correct) | `msg.c:743-744` / `lib.rs:2963-2964` |
| D12 | **PeekMessage retrieval order** | server order: SENT-drain → POSTED → QUIT → INPUT(hw) → **PAINT(synth)** → TIMER | shim: pure FIFO `VecDeque` front/pop_front, no priority, no QS filtering, no filterMin/Max honored | **WRONG-DATA / BLOCKER-CLASS**‡ (a Peek with a msg-range filter still gets front-of-queue) | `server/queue.c:3320-3392` / `lib.rs:2865-2882` |
| D13 | **QS_* bits / MsgWaitForMultipleObjects** | each posted/synth msg sets a QS bit (POSTMESSAGE/INPUT/PAINT/TIMER); MWMO wakes on wake_mask (`win32u/message.c:3624-3640`) | shim tracks **no QS state**; MsgWaitForMultipleObjects not modeled | **BLOCKER-CLASS**‡ (a game waiting via MWMO for QS_PAINT never wakes) | `win32u/message.c:3615-3636`, `server/queue.c:3334-3340` / (absent in `lib.rs`) |
| D14 | **PeekMessage(NOREMOVE) leaves msg; GetMessage removes** | NOREMOVE leaves the msg in queue and leaves QS bit; PM_REMOVE clears it | shim honors NOREMOVE via `q.front()` vs `q.pop_front()` (`lib.rs:2869`) — **correct mechanically** | **BENIGN** (matches for the FIFO model) | `win32u/message.c:3628-3634` / `lib.rs:2867-2869` |
| D15 | **WM_SIZE in queue duplicates the sent one** | `WM_SIZE(SIZE_RESTORED)` sent once during create | shim sends it (`lib.rs:2820`) **and** posts it (`lib.rs:2831`) → app sees it twice | **WRONG-DATA** (double resize) | `msg.c:754` / `lib.rs:2820,2831` |
| D16 | **CREATESTRUCT fields synthetic** | real CREATESTRUCT built from caller args + class defaults | shim hardcodes cx=1920/cy=1080, style=0x10CF0000, x=y=0 (`lib.rs:2810-2812`); lpCreateParams correctly forwarded from `[rsp+0x60]` (`lib.rs:2807-2808`) | **WRONG-DATA** (lpCreateParams right — the load-bearing field; dims/style ignore caller) | `win32u/window.c` cs build / `lib.rs:2807-2812` |
| D17 | **GetMessage blocks forever on empty queue** | GetMessage blocks in `wait_objects` until a msg matching mask arrives; wakes on server signal | shim `loop{ pop or sleep(50ms) }` forever on empty (`lib.rs:2917-2933`) — never delivers WM_QUIT, no wake source | **BLOCKER-CLASS**‡ (any GetMessage after queue drains hangs the thread) | `win32u/message.c:3634-3637` / `lib.rs:2917-2933` |
| D18 | **Class string A/W transcode + atom** | wndproc receives CREATESTRUCT transcoded to its charset; `lpszClass` may be atom (IS_INTRESOURCE) | shim doesn't populate lpszClass/lpszName in its synthetic CREATESTRUCT (offsets past cs[0] left 0 except dims/style) | **WRONG-DATA**‡ (wndproc reading cs->lpszClass gets 0) | `user32/win.c:410-540` / `lib.rs:2806-2812` |

**Divergence count: 18.**

---

## 3. CP2077 RELEVANCE — the 6s PeekMessageW(NOREMOVE) window-init join

The game pumps `PeekMessageW(NOREMOVE)` in a bounded window-init loop and
proceeds when it observes the expected post-create traffic. The failure wall is
"queue looks empty / wrong to Peek". Divergences that bite this join, ranked:

1. **D3 + D7 (the current crutch, and its risk).** The shim *deliberately* posts
   a SHOW/POS/SIZE/ACTIVATE/PAINT tail to `MSG_QUEUE` (`lib.rs:2827-2838`) so
   the game's `Peek(NOREMOVE)` sees a non-empty queue instead of the
   "2.68M-empty-Peek spin→timeout→DestroyWindow" documented in the shim
   comment (`lib.rs:2823-2826`). **This works around the symptom but is
   backwards vs real Windows:** on real Windows these are all `sent` and Peek
   sees *nothing* from creation except a synthesized `WM_PAINT`. If CP2077's
   init-join is satisfied by *any* non-empty Peek, the shim's crutch passes; if
   it validates the *message identity or WINDOWPOS payload* (D6), the NULL
   lParam on 0x46/0x47 will fault or fail the check.

2. **D6 (WINDOWPOS NULL).** If the game's Peek loop dispatches the queued
   `WM_WINDOWPOSCHANGING/CHANGED` and its wndproc reads `((WINDOWPOS*)lParam)->x`,
   it dereferences NULL (`lib.rs:2829-2830` post wParam=0,lParam=0). Real
   Windows always supplies a valid WINDOWPOS* and SWP flags. **Fix priority: give
   0x46/0x47 a real WINDOWPOS struct + SWP_SHOWWINDOW flags.**

3. **D13 (QS bits / MsgWaitForMultipleObjects).** If the init-join is *not* a
   busy Peek loop but a `MsgWaitForMultipleObjects(..., QS_PAINT|QS_ALLINPUT)`,
   the shim's total absence of QS state means the wait **never wakes** — a hard
   6s→timeout hang unaffected by the D3 queue crutch (MWMO waits on the server
   queue signal, not the shim VecDeque). This is the most dangerous *latent*
   blocker: the queue-stuffing fix does nothing for a QS-based waiter.

4. **D17 (GetMessage infinite block).** Once the game transitions from
   Peek-drain to a `GetMessage` pump and the synthetic queue empties, the shim
   spins `sleep(50ms)` forever with no WM_QUIT path (`lib.rs:2917-2933`). Any
   real message pump that reaches GetMessage after init hangs. The
   Peek-init-join may pass while the *next* stage deadlocks here.

5. **D12 (no filter / no priority).** CP2077's Peek may filter a message range
   (e.g. input-only `WM_KEYFIRST..WM_KEYLAST`). The shim ignores filterMin/Max
   (`lib.rs:2861-2884` reads rcx/r9 but never the range args) and returns
   front-of-queue regardless — so a filtered Peek returns a PAINT/POS message
   the caller didn't ask for, which its dispatch may reject as out-of-range.

**Bottom line for the join:** the empty-Peek→timeout→Destroy death is currently
masked by D3's queue-stuffing, but the mask is fragile: (D6) NULL WINDOWPOS,
(D13) QS-based waits, and (D17) the GetMessage handoff are the three ways the
6s init still fails *after* the Peek sees a non-empty queue. The
real-Windows-correct fix is to **model QS bits + a synthesized WM_PAINT** (so
Peek/MWMO agree) and **stop posting the sent-class lifecycle messages**, rather
than stuffing literal copies.

---

## Appendix — key source anchors

- Wine tables: `dlls/user32/tests/msg.c:186-196` (create), `474-517` (show),
  `729-746` (destroy), `748-789` (max-popup visible = fullest activation tail),
  `1102` (invisible popup), `1469-1488` (visible child).
- Wine create impl order: `dlls/win32u/window.c:6010` (GETMINMAXINFO), `6037`
  (NCCREATE), `6055-6065` (NCCALCSIZE wParam=FALSE), `6074` (CREATE),
  `6090-6091` (SIZE/MOVE), `6095-6110` (WS_VISIBLE→show).
- Wine peek order + QS: `server/queue.c:3320-3392`; QS mask build
  `dlls/win32u/message.c:3624-3640`; WM_PAINT synth `win32u/message.c:4074-4076`.
- Shim: `/tmp/alky-shims-lib.rs` create `2789-2846`, Peek `2861-2884`,
  GetMessage `2886-2934`, Dispatch `2937-2951`, Destroy `2957-2970`,
  MSG_QUEUE decl `340`.
