# 05 — LOCALE_TEXT: locale queries, codepage conversion, string classification/mapping/comparison

Family: GetLocaleInfoEx, GetUserDefaultLocaleName, LCMapStringW/Ex, GetStringTypeW,
GetACP, CompareStringW/Ex/Ordinal, MultiByteToWideChar, WideCharToMultiByte, GetCPInfo
(+ adjacent touched fns: IsValidCodePage, GetOEMCP, GetUserDefaultLCID, EnumSystemLocalesEx,
GetLocaleInfoW — they share the same trust chain).

CP2077 touched-list cross-check (`touched_apis_cp2077.txt`): GetACP:38, GetLocaleInfoEx:57,
GetUserDefaultLocaleName:78, IsValidCodePage:94, MultiByteToWideChar:100, WideCharToMultiByte:153.
CompareString\*/LCMapString\*/GetStringTypeW/GetCPInfo are NOT in the game's import-touch list but are
reached via CRT locale init and via GetProcAddress (shim GPA list, `alky-shims-lib.rs:124-136`), so they
stay in scope.

Wine sources: `dlls/kernelbase/locale.c` (impl), `dlls/kernel32/tests/locale.c` + `tests/codepage.c`
(conformance = truth). Shim: `/tmp/alky-shims-lib.rs`. † = source-only, ‡ = inference.

---

## 0. The two load-bearing framings

**(a) The fake-0 propagation chain.** Four of our arms return constant 0 = "call failed"
(`GetLocaleInfoEx`, `GetLocaleInfoW`, `LCMapStringW/Ex`, `GetStringTypeW`). Unlike the
version-info family (where fake-0 meant "no version resource" → DLL rejected), these zeros
route the caller into its *fallback locale path*. For the MSVC CRT that path is concrete:
`__acrt_update_locale` / ctype-table init calls `GetStringTypeW` + `LCMapStringW` to build the
`iswalpha`/`towlower` tables — a 0 means the CRT keeps the static ASCII "C"-locale tables, so
every non-ASCII classification/casing silently degrades. That is invisible for an
English-language boot and wrong the moment CP2077's text pipeline touches localized content ‡.
For the game's own region machinery: it reads `GetUserDefaultLocaleName` (we answer "en-US" —
honest), then refines via `GetLocaleInfoEx` (we answer 0 → its own defaults). The
censorship-config chain therefore resolves to the game's *default region*, not a wrong region —
degraded, not divergent-fatal ‡.

**(b) The UTF-8-everywhere posture.** Shim `GetACP()=65001` (`alky-shims-lib.rs:2384`) declares a
UTF-8 ANSI codepage. That is a *real Windows answer* (Win10 1803+ with "Beta: UTF-8 worldwide
language support" — wine's own tests accept `CP_UTF8` wherever they check ACP,
`tests/codepage.c:1178-1182`), and it makes the shim's conversion helpers (which hard-assume
UTF-8, see §1.7/§1.8) self-consistent for `CP_ACP`. The inconsistency is `GetCPInfo`, which
answers `MaxCharSize=1` — the answer for a *single-byte* codepage — while claiming ACP 65001
whose real CPINFO is `MaxCharSize=4` (wine `locale.c:2236`, `utf8_cpinfo = { CP_UTF8, 4, '?', 0xfffd, ... }`).
A CRT sizing multibyte buffers off `GetCPInfo(GetACP()).MaxCharSize` under-allocates 4× for
non-ASCII ‡. See divergence D8.

---

## 1. SPEC — real-Windows contracts as Wine encodes them

### 1.1 GetLocaleInfoEx — `locale.c:6118-6131`

```
INT GetLocaleInfoEx( const WCHAR *name, LCTYPE info, WCHAR *buffer, INT len )
```
- Locale resolution: `get_locale_by_name` (`locale.c:641-658`) — `NULL` = `LOCALE_NAME_USER_DEFAULT`
  → user locale; `"!sys-default-locale"` → system locale; else table lookup via
  `find_lcname_entry`. Unknown name → `SetLastError(ERROR_INVALID_PARAMETER)`, return 0
  (`locale.c:6126-6130`).
- **Name matching folds case AND `'_'`→`'-'`** (`compare_locale_names`, `locale.c:563-576`):
  `"en_US"`, `"EN-us"`, `"en-US"` all resolve to the same locale. So the en-US/en_US format
  question is a non-issue *on input*; on *output* the canonical form is always hyphenated
  (`LOCALE_SNAME` returns the table's `sname` = `"en-US"`, `locale.c:1441-1448`). LCID-as-string
  (`"0x0409"`) is NOT accepted — that's `LocaleNameToLCID`'s domain and even there only real
  names map (`locale.c:7030-7043`).
- Buffer contract (`locale_return_data`, `locale.c:766-783`): `len==0` → return required size in
  WCHARs **including the NUL** and write nothing; `0 < len < required` →
  `ERROR_INSUFFICIENT_BUFFER`, return 0; else copy and return copied count incl NUL.
  String LCTYPE + `LOCALE_RETURN_NUMBER` → `ERROR_INVALID_FLAGS`, return 0.
- Numeric LCTYPEs (`locale_return_number`, `locale.c:857-880`): without `LOCALE_RETURN_NUMBER`
  the value is rendered as a *decimal string* (`LOCALE_ILANGUAGE`/`IDEFAULTLANGUAGE` specially as
  `"%04x"` → `"0409"`); with it, a binary `UINT` is written into the buffer and the return is
  `sizeof(UINT)/sizeof(WCHAR)` = 2.
- Values for en-US that CP2077's region machinery can read:
  `LOCALE_SNAME` → `"en-US"` (ret 6); `LOCALE_SISO639LANGNAME` → `"en"` (ret 3,
  `locale.c:1432-1433`); `LOCALE_SISO3166CTRYNAME` → `"US"` (ret 3, `locale.c:1435-1436`);
  `LOCALE_ILANGUAGE` → `"0409"` / numeric 0x0409 (`locale.c:1142-1145`);
  `LOCALE_IDEFAULTANSICODEPAGE` → `"1252"` — with the quirk that a UTF-8 locale answers
  `CP_ACP` (0), not 65001 (`locale.c:1557-1558`).
- Conformance anchors (`tests/locale.c:5799-5860`): neutral `"en"` is a valid locale
  (SNAME → `"en"` ret 3); `len` too small → 0 + `ERROR_INSUFFICIENT_BUFFER`; `(NULL,0)` query →
  required size; `SPARENT("en-US")` → `"en"`; `SABBREVLANGNAME("en")` → `"ENU"`.

### 1.2 GetUserDefaultLocaleName — `locale.c:6439-6442`

Literally `get_locale_info(user_locale, user_lcid, LOCALE_SNAME, name, len)` — i.e. the exact
buffer contract of §1.1 applies: `len==0` → required count; too small →
`ERROR_INSUFFICIENT_BUFFER` + 0; success → chars incl NUL (6 for `"en-US"`). Sibling
`GetSystemDefaultLocaleName` `locale.c:6292-6295`. `GetUserDefaultLCID` returns `user_lcid`
(`locale.c:6421-6424`).

### 1.3 LCMapStringW / LCMapStringEx — `locale.c:6996-7024` / `6872-6894`

- `LCMapStringW`: LCID→locale-name mapping. Pseudo-LCIDs (`LOCALE_NEUTRAL`, `LOCALE_USER_DEFAULT`,
  `LOCALE_SYSTEM_DEFAULT`, `LOCALE_CUSTOM_*`) → user default; unknown LCID →
  `ERROR_INVALID_PARAMETER` + 0; else forwards to `LCMapStringEx` (`locale.c:7001-7023`).
- `LCMapStringEx` validation order (`locale.c:6886-6916`): `!src || !srclen || dstlen<0` →
  `ERROR_INVALID_PARAMETER` + 0; `srclen<0` → `lstrlenW(src)+1` (NUL participates and is counted);
  `src==dst` with any flag beyond `LCMAP_LOWERCASE|LCMAP_UPPERCASE` → `ERROR_INVALID_FLAGS` + 0;
  case/sortkey flags require a resolvable sort locale (`get_language_sort`, else 0);
  `LCMAP_HASH`/`LCMAP_SORTHANDLE` → FIXME + 0 (wine itself diverges here †);
  `LCMAP_SORTKEY` → `get_sortkey` path (returns **bytes**, dst is a BYTE buffer).
- Mapping core `lcmap_string` (`locale.c:4171-4245`): casing via sort casemap tables
  (`LCMAP_LINGUISTIC_CASING` selects locale-specific table); kana/width/Chinese charmaps;
  `NORM_IGNORENONSPACE|NORM_IGNORESYMBOLS` removal; `LCMAP_TITLECASE` unsupported →
  `ERROR_INVALID_FLAGS` (wine FIXME †); **unknown flag combinations → `ERROR_INVALID_FLAGS` + 0**.
- Return: mapped length in WCHARs (needed length when `dstlen==0`); `dstlen` too small →
  `ERROR_INSUFFICIENT_BUFFER` + 0 †(tail of lcmap_string/casemap_string).
- **Fake-0 semantics for a caller**: 0 is only ever an *error* on real Windows — callers treat it
  as "string unmappable / args bad". The MSVC CRT's `LCMapStringW`-based `_towlower_l` family
  falls back to per-char ASCII casing; a game-side case-insensitive key builder (path hashing,
  config keys ‡) that checks the return will take its error branch on every call.

### 1.4 GetStringTypeW — `locale.c:6239-6257`

```
BOOL GetStringTypeW( DWORD type, const WCHAR *src, INT count, WORD *chartype )
```
- `!src` → `ERROR_INVALID_PARAMETER` + FALSE; `type` must be exactly `CT_CTYPE1|2|3` →
  else `ERROR_INVALID_PARAMETER` + FALSE; `count==-1` → `lstrlenW(src)+1` (the NUL gets a
  chartype WORD too); fills one WORD per char via `get_char_type`, **always returns TRUE** after
  validation — there is no partial-failure mode. `GetStringTypeExW` ignores the locale entirely
  and forwards (`locale.c:6263-6268`).
- Consumer: CRT ctype init (`__acrt_locale_initialize_ctype` uses GetStringTypeW to build the
  256/64K-entry classification tables ‡). FALSE → static ASCII tables.

### 1.5 GetACP / GetCPInfo — `locale.c:5583-5586` / `5592-5606`

- `GetACP()` returns `ansi_cpinfo.CodePage` — no failure mode. 1252 on a classic en-US install;
  65001 on a UTF-8 system locale (both real; tests accept either, `tests/codepage.c:1178-1182`).
- `GetCPInfo(cp, out)`: `!cpinfo` or unknown cp (`get_codepage_table` NULL) →
  `ERROR_INVALID_PARAMETER` + FALSE. Fills `MaxCharSize`, `DefaultChar[2]`, `LeadByte[12]`.
  For CP_UTF8: `{ MaxCharSize=4, DefaultChar='?' }` from the static `utf8_cpinfo`
  (`locale.c:2236`). For 1252: MaxCharSize=1, no lead bytes. `CP_ACP`(0) resolves to the ANSI
  table (`locale.c:2233-2260`).

### 1.6 CompareStringW / CompareStringEx / CompareStringOrdinal — `locale.c:4889-4917` / `4764-4802` / `4923-4940`

- `CompareStringW`: same LCID pseudo-value handling as LCMapStringW; unknown LCID →
  `ERROR_INVALID_PARAMETER` + 0; forwards to Ex.
- `CompareStringEx`: flags outside the supported set (`NORM_IGNORECASE|NORM_IGNORENONSPACE|
  NORM_IGNORESYMBOLS|SORT_STRINGSORT|NORM_IGNOREKANATYPE|NORM_IGNOREWIDTH|NORM_LINGUISTIC_CASING|
  LINGUISTIC_IGNORECASE|LINGUISTIC_IGNOREDIACRITIC|SORT_DIGITSASNUMBERS|0x10000000|LOCALE_USE_CP_ACP`)
  → `ERROR_INVALID_FLAGS` + 0; unresolvable sort locale → 0; `!str1||!str2` →
  `ERROR_INVALID_PARAMETER` + 0; `len<0` → `lstrlenW` (NUL *not* included — contrast MB/WC/LCMap);
  result is **CSTR_LESS_THAN=1 / CSTR_EQUAL=2 / CSTR_GREATER_THAN=3, 0 = error** — the +2-biased
  ternary. Comparison is *linguistic* (sortkey-based `compare_string`): the conformance table
  `tests/locale.c:3461-3479` pins en-US orderings where code-point order is the WRONG answer
  (e.g. `L"A" > L"a"` linguistically = CSTR_GREATER_THAN while ordinal says 'A'(0x41) < 'a'(0x61)).
- `CompareStringOrdinal`: binary `RtlCompareUnicodeStrings` with optional case-fold; same
  1/2/3 return, `ERROR_INVALID_PARAMETER` + 0 on NULL strings.

### 1.7 MultiByteToWideChar — `locale.c:7049-7092`

```
INT MultiByteToWideChar( UINT cp, DWORD flags, const char *src, INT srclen, WCHAR *dst, INT dstlen )
```
- Validation: `!src || !srclen || (!dst && dstlen) || dstlen<0` → `ERROR_INVALID_PARAMETER` + 0.
  `srclen<0` → `strlen(src)+1` — **the NUL is converted and counted in the return**.
- Unknown cp → `ERROR_INVALID_PARAMETER`; flags outside
  `MB_PRECOMPOSED|MB_COMPOSITE|MB_USEGLYPHCHARS|MB_ERR_INVALID_CHARS` → `ERROR_INVALID_FLAGS`
  (per-cp; CP_SYMBOL/CP_UTF7 take no flags).
- CP_UTF8 core `mbstowcs_utf8` (`locale.c:2523-2542`): `RtlUTF8ToUnicodeN`;
  `STATUS_SOME_NOT_MAPPED` + `MB_ERR_INVALID_CHARS` → `ERROR_NO_UNICODE_TRANSLATION` + 0;
  without the flag, invalid sequences become U+FFFD and the call *succeeds*.
- **Return value is CHARS not bytes** (`reslen/sizeof(WCHAR)`, `locale.c:2541`). `dstlen==0` =
  query mode → required chars. Buffer too small → `ERROR_INSUFFICIENT_BUFFER` + **0** (via
  `set_ntstatus` on `STATUS_BUFFER_TOO_SMALL`) — real Windows does NOT return a truncated count.

### 1.8 WideCharToMultiByte — `locale.c:7405-7449`

- Validation mirror of §1.7 (`lstrlenW(src)+1` for `srclen<0`); flags outside
  `WC_DISCARDNS|WC_SEPCHARS|WC_DEFAULTCHAR|WC_ERR_INVALID_CHARS|WC_COMPOSITECHECK|WC_NO_BEST_FIT_CHARS`
  → `ERROR_INVALID_FLAGS` + 0.
- CP_UTF8 core `wcstombs_utf8` (`locale.c:2926-2946`): `*used=FALSE` written up front when
  provided; unpaired surrogate + `WC_ERR_INVALID_CHARS` → `ERROR_NO_UNICODE_TRANSLATION` + 0;
  without it → replacement + `*used=TRUE`. Return = **bytes**.
- Test-truth (`tests/codepage.c:258-320`): `defchar`/`used` with CP_UTF8 →
  `ERROR_INVALID_PARAMETER` on pre-1709 Windows, *accepted* since Win10 1709 (test tolerates
  both, so either behavior is conformant †); an unrecognized flag (0x100) with `used!=NULL` on
  CP_UTF8 → `ERROR_INVALID_PARAMETER`.

### 1.9 Adjacent: IsValidCodePage / GetOEMCP / EnumSystemLocalesEx

- `IsValidCodePage` (`locale.c:6716-6726`): CP_UTF7/CP_UTF8 → TRUE; else TRUE iff a codepage
  table exists; pseudo-CPs (CP_ACP=0 etc) are NOT valid inputs †.
- `GetOEMCP` (`locale.c:6190`): `oem_cpinfo.CodePage` (437 on en-US).
- `EnumSystemLocalesEx`: enumerates the full locale table, one callback per name; return TRUE.

---

## 2. DIVERGENCE table (Wine spec ⟷ Alky shim)

| # | fn | real Windows (wine cite) | ours (shim cite) | severity |
|---|-----|--------------------------|------------------|----------|
| D1 | GetLocaleInfoEx | data for any valid name, canonical `"en-US"` forms; 0 only on bad args (`locale.c:6118-6131`) | **constant 0**, no SetLastError (`alky:1670`) | **TRUST-CHAIN** — CP2077 language-selection + CRT locale init read SNAME/ISO639 → every read takes the fallback branch; game resolves to default region ‡ |
| D2 | GetLocaleInfoW | same via LCID (`locale.c:6094-6112`) | **constant 0** (`alky:2391`) | TRUST-CHAIN (same chain as D1, CRT-facing) |
| D3 | LCMapStringW | real casing/sortkey; 0 = error only (`locale.c:6996`, `4171-4245`) | **constant 0** (`alky:2495`) | **TRUST-CHAIN** — caller treats every string as unmappable; CRT `_towlower_l` degrades to ASCII; case-insensitive key builders take error branch ‡ |
| D4 | LCMapStringEx | id. (`locale.c:6872-6894`) | **constant 0** (`alky:2955`) | TRUST-CHAIN (GPA-reached, `alky:124`) |
| D5 | GetStringTypeW | TRUE + per-char WORDs; FALSE only bad args (`locale.c:6239-6257`) | **constant 0** (`alky:2495`) | **TRUST-CHAIN** — CRT ctype tables stay ASCII-only; `iswalpha`/`towupper` wrong for all non-ASCII ‡ |
| D6 | MultiByteToWideChar | honors cp + `MB_ERR_INVALID_CHARS` (`ERROR_NO_UNICODE_TRANSLATION`+0); small buffer → `ERROR_INSUFFICIENT_BUFFER`+**0**; chars-not-bytes (`locale.c:7049-7092`, `2523-2542`) | cp+flags **ignored**, always UTF-8-lossy (U+FFFD, never fails); small buffer → **truncated count, no error**; NULL/args → 0 w/o last-error; return unit chars ✓, `srclen<0` incl-NUL ✓ (`alky:903-923`, dispatch `2494`) | **WRONG-DATA** — `MB_ERR_INVALID_CHARS` callers (validation-by-conversion) never see the reject; truncation reads as success ‡ |
| D7 | WideCharToMultiByte | honors cp + `WC_ERR_INVALID_CHARS`; `*used` always written when given; small buffer → error+0; bytes (`locale.c:7405-7449`, `2926-2946`) | cp+flags ignored, UTF-16-lossy; **defchar/used never touched** (stack args 7/8 unread); truncated count on small buffer (`alky:883-902`, dispatch `2493`) | **WRONG-DATA** — a caller reading `*used` gets uninitialized stack ‡; non-UTF-8 cp requests silently produce UTF-8 bytes |
| D8 | GetCPInfo | unknown cp → FALSE; CP_UTF8 → `MaxCharSize=4` (`locale.c:5592-5606`, `2236`) | always TRUE, `MaxCharSize=1`, DefaultChar `'?'` (`alky:2387-2389`) | **WRONG-DATA / TRUST-CHAIN-adjacent** — contradicts our own GetACP=65001; CRT buffer sizing off MaxCharSize under-allocates 4× for non-ASCII ‡ |
| D9 | CompareStringEx | linguistic sortkey order (tests pin `"A">"a"` at `tests/locale.c:3469`); flags validated; explicit lens honored, NUL-terminated read only when len<0 (`locale.c:4764-4802`) | code-point compare of `read_wstr` NUL-terminated strings: **flags ignored (NORM_IGNORECASE dropped), explicit len1/len2 ignored → over-read past unterminated slices**; 1/2/3 shape ✓ (`alky:1664-1669`) | **WRONG-DATA** — sort order diverges from linguistic; ignoring lengths is a correctness+OOB-read hazard ‡ |
| D10 | CompareStringW, CompareStringOrdinal | `locale.c:4889-4917`, `4923-4940` | **no dispatch arm** → `_ => handled_real=false` (`alky:3207`) → die-loud | BLOCKER-if-called — honest by policy (die-loud > fake), but CompareStringW is the classic CRT/game import; absent from CP2077 touch-list so ungraded at runtime † |
| D11 | GetUserDefaultLocaleName | buffer contract of §1.1: `len==0` → required, no write; small → `ERROR_INSUFFICIENT_BUFFER`+0 (`locale.c:6439-6442`) | writes `"en-US\0"` unconditionally when ptr valid, **cchLocaleName (rdx) never checked** → 6-WCHAR write into smaller buffers; ret 6 ✓ value ✓ hyphen-format ✓ (`alky:1671-1676`) | WRONG-DATA (overflow hazard when cch<6 ‡; value itself PASS) |
| D12 | EnumSystemLocalesEx | full enumeration, one callback per locale | ret 1 with **zero callbacks** unless `ALKY_LOCALE_CB` env set (then en-US only) (`alky:1651-1662`) | TRUST-CHAIN — "which languages exist" enumerators see an empty world; success-with-nothing is a lie shaped exactly like the census class |
| D13 | IsValidCodePage | FALSE for unknown cps (`locale.c:6716-6726`) | constant 1 (`alky:2386`) | BENIGN-leaning — fake-success; only bites a caller probing cp support before conversion (our converter "supports" everything anyway per D6/D7) |
| D14 | GetACP / GetOEMCP / GetUserDefaultLCID / GetUserDefaultUILanguage | 1252-or-65001 / 437 / 0x0409 / 0x0409 (`locale.c:5583`, `6190`, `6421`, `6447`) | 65001 / 437 / 0x0409 / 0x0409 (`alky:2384-2385`, `2390`, `1677`) | **PASS** — all are honest real-Windows answers for an en-US UTF-8-mode system; tests accept ACP=65001 (`tests/codepage.c:1178-1182`) |

Also PASS: `GetUserDefaultLocaleName`'s *value and format* ("en-US", hyphen, canonical casing —
matches `locale->sname` exactly), and both converters' return-unit (chars for MB→WC, bytes for
WC→MB) and include-the-NUL-when-srclen<0 behavior.

Note on D10-adjacent: `IsValidLocaleName` sits in the GPA_SHIMMED name list (`alky:131`) but has
no dispatch arm → GetProcAddress hands out a thunk whose call dies loud ‡. Same for
`GetTimeFormatEx`/`GetDateFormatEx` (fixed "12:00:00"/"2026-08-11" — out of family scope but
noted, `alky:1640-1650`).

---

## 3. RET-0 GRADING (census suspects prioritized)

| arm (shim cite) | grade | honest fix |
|---|---|---|
| `GetLocaleInfoEx → 0` (`1670`) — **census suspect** | fake-FAILURE (never the real answer for a valid name) | **implement-real, small**: static en-US answer table keyed on LOWORD(lctype) — SNAME→"en-US", SISO639LANGNAME→"en", SISO3166CTRYNAME→"US", ILANGUAGE→"0409", IDEFAULTANSICODEPAGE→"1252"-or-CP_ACP-quirk (§1.1), SDECIMAL→".", SLIST→",", honoring the three-mode buffer contract (query/insufficient/copy) + `LOCALE_RETURN_NUMBER` binary mode. Accept any name that case-folds/underscore-folds to en-US or "en"; die-loud on other locales (real divergence signal, not fallback noise) |
| `GetLocaleInfoW → 0` (`2391`) — census suspect | fake-FAILURE | same table via LCID 0x0409/pseudo-LCIDs |
| `LCMapStringW/Ex → 0` (`2495`, `2955`) — **census suspect** | fake-FAILURE | **implement-real for the 90% flags**: LCMAP_UPPERCASE/LOWERCASE via per-char simple case map (Rust `char::to_uppercase` first-char is adequate for the CRT's use ‡); LCMAP_BYTEREV memcpy+swap; honor srclen<0 = incl-NUL, dstlen=0 = query; **die-loud on LCMAP_SORTKEY** (a wrong sortkey silently corrupts sorted containers — worse than a crash) |
| `GetStringTypeW → 0` (`2495`) — **census suspect** | fake-FAILURE | **implement-real**: CT_CTYPE1 classifier from `char` properties (C1_ALPHA/UPPER/LOWER/DIGIT/SPACE/PUNCT/CNTRL/XDIGIT/BLANK — Rust `char::is_*` covers all bits the CRT reads); CT_CTYPE2/3 can die-loud until observed |
| `GetACP → 65001` (`2384`) | **fully-correct-constant** (real Win10-1803+ answer, test-accepted) | none — but it CONTRACTS us into UTF-8 answers everywhere: keep converters UTF-8 (they are) and fix GetCPInfo (next row) |
| `GetCPInfo → 1, MaxCharSize=1` (`2387-2389`) | fake-success with wrong payload | 6-line fix: cp∈{0,65001} → MaxCharSize=4, DefaultChar='?' (mirror `utf8_cpinfo`, wine `locale.c:2236`); cp==437/1252 → MaxCharSize=1; unknown cp → ret 0 |
| `IsValidCodePage → 1` (`2386`) | fake-success | whitelist {437, 1200, 1252, 65000, 65001} → 1 else 0; low stakes given D6/D7 accept everything anyway |
| `EnumSystemLocalesEx → 1, no callbacks` (`1651-1662`) | fake-success (empty enumeration) | un-gate the en-US callback (drop the `ALKY_LOCALE_CB` env condition) — success-with-one-locale is the honest shape of this machine; keep the crash-window note as a comment |
| `GetUserDefaultLocaleName → writes+6` (`1671-1676`) | correct-constant value, missing buffer contract | check rdx: 0 → return 6 without writing; <6 → return 0 (+ set ERROR_INSUFFICIENT_BUFFER in LAST_ERROR, the mutex exists at `alky:2274-2275`) |
| `GetUserDefaultLCID/UILanguage → 0x0409` (`1677`, `2390`) | fully-correct-constant | none |
| `CompareStringEx` (real-cmp arm, `1664-1669`) | not a constant anymore (upgraded from the const-2 noted in its own comment) — graded here for its residue | honor len1 (r9) / len2 (stack+0x30) instead of read-to-NUL; honor NORM_IGNORECASE (case-fold before cmp); binary-vs-linguistic residue is acceptable ‡ (CRT/game mostly wants equality/inequality, not collation) |
| `MultiByteToWideChar`/`WideCharToMultiByte` helpers (`883-923`) | not constants — graded for error-contract | three fixes in priority order: (1) honor `MB_ERR_INVALID_CHARS`/`WC_ERR_INVALID_CHARS` → return 0 + `ERROR_NO_UNICODE_TRANSLATION` on lossy decode (validation-by-conversion callers exist in every engine ‡); (2) small-buffer → 0 + `ERROR_INSUFFICIENT_BUFFER`, never a truncated count; (3) write `*used=FALSE` when arg 8 present (WC→MB) — reading it is UB today |

---

## 4. CP2077 relevance — where the fake-0s land

- **Language selection**: `GetUserDefaultLocaleName` → honest `"en-US"` ✓. The refinement reads
  (`GetLocaleInfoEx` LOCALE_SNAME/ISO639, census-flagged) → 0 → the game's own
  default-region path. Consequence: region/censorship config resolves to defaults — *stable but
  input-deaf*: changing the host locale can never change the game's answer ‡. With the D1 fix the
  chain becomes end-to-end honest for en-US.
- **Text pipeline**: the game is UTF-8-native (REDengine string tables); its
  `MultiByteToWideChar(CP_UTF8, MB_ERR_INVALID_CHARS, ...)`-shaped validation calls currently
  *cannot reject* malformed input (D6) — corrupt localization data flows through as U+FFFD
  instead of triggering the game's error branch ‡. LCMapString-0 (D3) means any case-insensitive
  compare/hashing the game routes through NLS treats input as unmappable; whether CP2077 branches
  fatally on that is unobserved † — the CRT path (ctype tables, D5) is the confirmed consumer.
- **The GetACP=65001 posture** is the right call: it makes CP_ACP≡CP_UTF8, matching both the
  shim's converters and modern-Windows reality, and wine's conformance tests bless it. The one
  incoherence (GetCPInfo MaxCharSize=1, D8) is the cheapest high-value fix in the family.

---

## Appendix — source anchors

| what | where |
|---|---|
| locale-name fold (case + `_`→`-`) | wine `locale.c:563-576` |
| get_locale_by_name / user-default | wine `locale.c:641-658` |
| buffer contract (query/insufficient/copy) | wine `locale.c:766-783` |
| numeric LCTYPE rendering / RETURN_NUMBER | wine `locale.c:857-880` |
| SNAME/ISO639/ISO3166 answers | wine `locale.c:1425-1448` |
| utf8 CPINFO (MaxCharSize=4) | wine `locale.c:2236` |
| mbstowcs_utf8 / MB_ERR_INVALID_CHARS | wine `locale.c:2523-2542` |
| wcstombs_utf8 / WC_ERR_INVALID_CHARS / *used | wine `locale.c:2926-2946` |
| lcmap_string flag matrix | wine `locale.c:4171-4245` |
| CompareStringEx flags + 1/2/3 | wine `locale.c:4764-4802` |
| linguistic-order test truth | wine `tests/locale.c:3461-3479` |
| GetLocaleInfoEx test truth | wine `tests/locale.c:5799-5860` |
| CP_UTF8 defchar/used test truth | wine `tests/codepage.c:258-320` |
| ACP-may-be-UTF8 test truth | wine `tests/codepage.c:1178-1182` |
| shim converters | alky `883-923` (dispatch `2493-2494`) |
| shim fake-0 arms | alky `1670` `2391` `2495` `2955` |
| shim locale constants | alky `1671-1677` `2384-2390` |
| shim CompareStringEx real-cmp | alky `1664-1669` |
| shim GPA_SHIMMED list | alky `124-136` |
| shim die-loud default | alky `3207-3210` |
