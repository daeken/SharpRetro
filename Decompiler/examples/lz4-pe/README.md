# Published Windows x64 LZ4 to ARM64

This fixture decompiles `LZ4_decompress_safe` from the **published Windows x64
LZ4 v1.8.3 DLL**, then compiles the recovered C++ for native ARM64 macOS. The input
is `dll/liblz4.so.1.8.3.dll` inside the project's official
[release archive](https://github.com/lz4/lz4/releases/tag/v1.8.3).

The archive and DLL are unmodified. `upstream.json` pins their SHA-256 values and
the same-tag source at commit `641b453d9db536ee020851bfcb1dc39f61006f0a`.
The DLL hash is
`1731d54f5ec77effa24beda1075105395aa9b9ff0a042a618a17204253c8a960`.
Source and its BSD license are vendored separately for an independent oracle.

## Reproduce

Requires Apple Silicon macOS, Rosetta, .NET 10, Python 3 and Clang:

```sh
DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1 dotnet build Decompiler/Decompiler.csproj -c Release
DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1 python3 Decompiler/examples/lz4-pe/roundtrip.py
```

The runner downloads the pinned archive if absent, verifies every input hash and
keeps the downloaded binary under ignored `build/`. It does not compile a
replacement Windows input. To emit the function after downloading:

```sh
dotnet Decompiler/bin/Release/net10.0/Decompiler.dll decompile-function \
  Decompiler/examples/lz4-pe/build/liblz4-published-x64.dll \
  --export LZ4_decompress_safe --control-flow structured \
  --output Decompiler/examples/lz4-pe/build/recovered.cpp
```

The selected export is at preferred VA `0x61493c20`, RVA `0x13c20`. Its closure
has 451 instructions and 94 basic blocks, one generated C++ function, 128-bit XMM
copies and a `memcpy` import. It emits a callable `recovered_export` adapter,
not a Windows process entry point. `driver.cpp` provides only input/output
marshalling, allocation and canaries; the decoder algorithm is generated.

## Three independent executions

1. `pe_reference.cpp`, compiled for x86-64 Mach-O, maps the **original DLL bytes**
   and invokes its exported machine code under Rosetta. It applies DIR64 rebases,
   patches IAT slots to explicitly declared Win64 `ms_abi` memory bridges, and
   traps every unsupported import. A call gate supplies RCX/RDX/R8/R9, aligned
   stack/shadow space, and zero values in the eight nonargument preserved GPRs.
2. The recovered C++ is compiled separately for ARM64, normally and with
   ASan/UBSan. It needs no Windows binary, source, Rosetta or .NET at runtime.
3. The unchanged same-tag source is compiled natively as a compression generator
   and an independent decoder oracle.

The adapter exposes extra recovered entry-state GPR inputs when they are demanded;
generic tests pass nonzero entry state and verify that a function depending on
RBX observes it. In this decoder, private scalar/value recovery now eliminates
all eight spill-only inputs, leaving the four Win64 register arguments. The raw
reference gate still supplies zero preserved-register entry state, and the
fixture remains an observation of that explicitly recorded domain.

Each primary case executes the raw PE twice in separate processes, then the
normal ARM64 and sanitizer builds. It compares the signed 32-bit API return,
**every initialized destination byte**, source preservation and outer canaries.
The reference also verifies that mapped readable image bytes remain unchanged.
This includes partial error writes and extra wild-copy bytes beyond the returned
length but within capacity. The source oracle independently checks API
success/failure and returned decoded bytes; its compiler's partial/wild writes
are not used as a substitute for the published PE's behavior.

All 509 cases match: 196 successful decodes and 313 error returns. Cases cover
empty blocks, random/repeated/patterned data up to 16 KiB, long 255-byte length
extensions, offsets 1–8, overlapping matches, truncated/corrupted blocks, exact,
short and padded output capacities, and 16/32-byte fast-path boundaries. An
intentional decoder-shift mutation must produce a normal-runner semantic mismatch.
Published version and compression-bound exports are additionally checked against
the source oracle, including signed limits.

The preceding control-flow checkpoint passed all 146 tests, including 26 PE
metadata/ABI/vector/refusal tests and two published-DLL integration tests. Run it
from the repository root with:

```sh
DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1 python3 -m unittest discover -s Decompiler/tests -v
```

The published-DLL tests require Apple Silicon macOS and the cached, hash-verified
release archive; they skip explicitly when that fixture is unavailable.

## Adversarial control-flow review

Four implementation/review rounds improve the emitter for arbitrary supported
functions, rather than hand-editing this decoder or inferring a copy algorithm:

1. Thread exact loop/function continuations through tail conditionals so the
   deeply unrolled byte-copy staircase becomes guard clauses. Preserve local
   bindings; do not skip intervening effects or cross nested-loop targets.
2. Recover exact post-test loops as `do ... while`, keeping comparison snapshots
   before their original commits. Earlier continues or body-local test names
   prevent the rewrite.
3. Finish small exceptional arms at the existing common exit, flatten the large
   arm, and combine adjacent pure one-arm conditions with short-circuit `&&`.
4. Try three bounded, independent join policies and select the smallest valid
   CFG representation. Every candidate passes the unchanged region-edge checker
   and duplication bound; no instruction effects are replaced or omitted.

| Measured generated artifact | Before review | After review |
| --- | ---: | ---: |
| C++ lines, including runtime/adapter | 1,861 | 1,194 |
| File bytes | 158,983 | 77,041 |
| Maximum function indentation, spaces | 116 | 40 |
| Copied CFG block instances | 194 | 72 |
| Emitted conditional regions, including copies | 205 | 108 |

The same 451 instructions and 94 original blocks remain covered. All 509 primary
cases, source-oracle checks, sanitizers and the semantic mutation control pass
after each round. Source-tree tests additionally compare 33 hand-built trees
before cleanup, after cleanup, and after a second cleanup against explicit
oracles for all four boolean input combinations under ASan/UBSan. They include
ancestor shadowing, conditional faults, short-circuit RHS loads, effects after
guards, mutable condition snapshots and nested-loop break/continue targets.
Machine-code fixtures separately check shared tails bypassed by early returns
and nested loops with several exit destinations.

That checkpoint left 72 shared-region copies and machine-value names visible.
The following value-recovery stage separates register lifetimes. Neither stage
replaces ordered overlapping accesses with a guessed library operation.

## Function-wide value recovery

A later pass builds register and proven-private-memory SSA across the function.
The selected decoder's signature shrinks from twelve machine inputs to four;
saved-register values used only by unobserved private spills disappear. Seventeen
of the 21 typed frame cells become values, including private limits and the saved
register cells. The four indexed 128-bit table fields keep their shared native
storage because narrow indexed views overlap them.

Parameters retain their entry identities instead of becoming reusable register
carriers. Each load keeps its original snapshot. Joins have selected-edge parallel
copies, and calls produce fresh result definitions. Carriers move to their smallest
common lexical scope; eligible single definitions become `const`. Completely
promoted frames can disappear in smaller fixtures.

Tests include real machine-code fixtures for entry backedges, simultaneous loop
swaps, private-cell joins, reads before later stores, partial aliases, missing
initialization, helper observations, frame-bit escape/conditional reconstruction,
and a comparison whose index changes before the branch. The last fixture makes
an incorrect stale bound observably read a promoted cell through its byte storage.
Source-tree fixtures also protect address-taken carriers and loop-copy effects on
the condition snapshot. Generic loads and division-fault checks retain their
behavior even when a private memory read is forwarded to a value.

This depends on the documented native object/provenance profile: external inputs
cannot fabricate this fresh frame's numeric address. Promotion never treats an
uninitialized read as a zero SSA entry. Raw frame-bit observations retain memory;
those classification probes are not cross-binary address-equivalence claims.

This stage improves identities and removes machine bookkeeping. It still uses
exact-width integer carriers and generic value names. At that checkpoint, source
type recovery and interference-checked variable coalescing remained later steps; expanded explicit
join copies can increase line count even as the data flow becomes clearer.

| Value-recovery checkpoint | Before | After |
| --- | ---: | ---: |
| Recovered function inputs | 12 | 4 |
| Physical typed frame fields | 21 | 4 |
| Original instructions / blocks | 451 / 94 | 451 / 94 |
| C++ lines, including runtime/adapter | 1,194 | 1,369 |
| Maximum function indentation, spaces | 40 | 44 |

All 156 decompiler tests pass at this checkpoint, including ten focused SSA and
promotion tests, 37 source-tree shapes checked before/once/twice on four boolean
input combinations, and an address-taken carrier fixture. All 509 published DLL
cases still match complete destination state, return values and canaries in the
normal ARM64 and ASan/UBSan builds; the source oracle and mutation control pass.
`build/function-ssa-summary.json` records exact source/implementation identities,
the regression log identity and these measurements.

## Native access readability

Fixed-width loads/stores now emit direct `u8`/`u16`/`u32`/`u64`/`u128` pointer
dereferences. These typedefs carry Clang/GCC alignment-one and `may_alias`
attributes at the access itself. They are an explicit compiler extension profile;
the measured target remains Apple Clang/ARM64 macOS. Load snapshots and store
widths remain exact, including unaligned 128-bit and mixed-width overlaps.
Sanitizers and strict aliasing remain enabled.

The decoder body goes from 72 aliasing `memcpy` operations to zero. Its actual
imported `memcpy` call remains a library call. Nine of the 17 final guarded
post-test copy bundles become unconditional tail updates. A structured CFG checks
false-exit liveness, later enclosing-loop iterations, definite initialization,
condition dependencies and copy order; the other eight guards remain conservative.

The complete artifact falls from 1,369 to 1,289 lines. This change does not yet
recover source-level pointer types, variable names or eliminate the 72 copied CFG
block instances. Those are separate from removing fixed-width access scaffolding.

All 157 decompiler tests pass at this checkpoint. The 45 source-tree shapes
include false-exit observations, later enclosing-loop reads, conditional
uninitialized copy inputs, address-taken destinations, ordered swaps and early
breaks. The memory-view test exercises every width at offsets 0–31 under strict
aliasing, `-O3` and ASan/UBSan. The normal ARM64, sanitizer, source-oracle and
mutation-control checks still pass all 509 published DLL cases.
`build/readable-native-summary.json` binds the final artifact and validation.

## Carrier and expression refinement

Three further rounds use an independent adversarial reviewer and an implementation
agent. Changes apply to the emitter and its structured statement tree; the DLL,
reference runner, corpus and comparator remain unchanged.

1. Remove final casts already supplied by exact-width assignments, and render
   wrapped negative offsets as unsigned subtraction. Nested casts still preserve
   narrow arithmetic, integer promotion and truncation before widening.
2. Coalesce same-type compiler-owned carriers using the occurrence CFG's liveness
   and interference graph. Check whole merged groups and distinct edge-copy
   destinations; consume the already scheduled copies without reordering them.
   Address escapes, unsupported syntax and analysis limits preserve the original
   representation. Place remaining declarations near first use in their existing
   lexical scope.
3. Combine eligible first assignments with declarations and render small bit-clear
   masks as `~mask`, with an explicit 128-bit complement where required. Recompute
   loop-copy facts after renaming. Keep mutable values mutable, including compound
   updates, increments and indirect writes.

| Generated artifact | Before this review | After |
| --- | ---: | ---: |
| C++ lines, including runtime/adapter | 1,289 | 1,175 |
| File bytes | 79,842 | 72,230 |
| Mutable SSA carriers | 112 | 81 |
| Redundant copies removed by coalescing | 0 | 89 |
| Explicit casts in recovered function bodies | 119 | 80 |
| Wrapped negative-offset additions | 22 | 0 |

All three rounds pass the unchanged 509-case published DLL comparison on ARM64
and ASan/UBSan, source-oracle checks and mutation control. Focused arithmetic
tests observe complete 64/128-bit results, including 32-bit wrap before widening,
16-bit multiplication promotions, boolean low bits and nonstandard-width masks.
Adversarial AST fixtures exercise declaration placement and carrier interference
independently of this decoder.

That checkpoint's regression validation passed 162 methods: 161 in the full discovery run
(180.326 seconds), plus the new coalescing harness run separately after it landed
(1.766 seconds). The latter executes 19 before/after C++ cases against explicit
oracles under ASan/UBSan and checks five structural refusal cases. No tests skip.
The existing 45 source-tree shapes still run before/once/twice over all four
boolean input pairs; five compiled scope fixtures and a comparison-parsing
refusal separately cover declaration and constification boundaries.

At that checkpoint, generic value names, integer pointer carriers, eight guarded post-test tails and
72 copied CFG block instances remain. Further source typing, object inference or
algorithm reconstruction needs additional analysis. The review stops at that
boundary instead of assigning meaning from this one decoder's apparent intent.
`build/refinement-review-summary.json` records the current regression results,
per-round measurements and exact source, implementation and runner identities.

## Predicate propagation and return staging

The next pass builds an occurrence CFG over the final structured tree. A saved
boolean can become its original expression only if that same definition reaches
the test on every path, every operand retains its value and lexical binding,
and recomputation is total and pure. Ordinary writes, joins, loop backedges,
scope entry/exit and abrupt loop exits participate in the analysis. Raw loads,
division/undefined-flag checks and unknown calls are not delayed. Opaque argument
captures and reference aliases block forwarding; native calls have an explicit
catalog of verified by-value parameters. Embedded uses preserve the original
boolean conversion. A liveness pass then removes dead predicate assignments.

Exact-type return forwarding removes adjacent `carrier = expression; return
carrier;` pairs and ordered copy chains. A shared return can move into complete
tail branches without changing conditional effects or abrupt loop destinations.
Return conversion replaces only the identical assignment conversion; inner
32-bit truncations and load snapshots stay intact. Subsequent scope placement
omits unused carriers and retains initializers at their original execution point.

| Generated artifact | Before | After |
| --- | ---: | ---: |
| C++ lines, including runtime/adapter | 1,175 | 1,042 |
| File bytes | 72,230 | 62,753 |
| Saved `branch_condition` assignments | 108 | 0 |
| Predicate uses forwarded | 0 | 116 |
| Adjacent carrier assignment/return pairs | 6 | 0 |
| Carrier declarations | 81 | 80 |
| Bare carrier declarations | 55 | 54 |

The remaining bare declarations mostly carry real branch/loop joins or shared
names across duplicated CFG regions. Splitting independent reaching-definition
webs could make more declarations local and initialized; the current pass does
not infer that from syntax or speculate an initializer on another path.

Twenty-nine predicate fixtures compare original/transformed C++ against explicit
oracles under ASan/UBSan, covering operand writes, joins, mutable loop tests,
normal/abrupt scope exit, shadowing, escaped storage and bool conversion.
Thirty-five return fixtures additionally check ordered calls, exceptions,
truncation, copy chains and complete conditional returns across four input pairs.
Declaration merging now refuses top-level comma expressions, and reference-bound
carriers remain mutable. The branch mutation controls negate the actual recovered
predicate instead of depending on a temporary's spelling; the escaped-field
fixture still checks both the post-call read and complete 64-bit runtime result.

All 164 decompiler test methods pass in the final full discovery run, with no
skips. All 509 published DLL cases still match normal ARM64 and ASan/UBSan execution;
the independent source oracle and deliberate semantic mutation control pass.
`build/condition-propagation-summary.json` binds the final regression results,
source/implementation/runner hashes and both checkpoints. The preceding artifact
is retained as `build/before-condition-propagation-recovered.cpp`.

## Boolean expression rendering

The final predicate renderer complements all six integer comparisons, applies
De Morgan's rules and removes parentheses using C++ precedence. For example:

```cpp
// Before
if((!(variable_13==0xfULL)) && (!(variable_14<=7ULL)))
// After
if(variable_13 != 0xfULL && variable_14 > 7ULL)
```

Ordered complements require the emitter's explicit integer profile. Floating
comparisons retain their negation, including unordered/NaN behavior. Short-circuit
order and eager bitwise operations stay intact. Boolean truth contexts can omit
`!!x`; comparisons and arithmetic retain its 0/1 conversion. Arithmetic grouping,
casts and unary token boundaries remain exact. Unsupported syntax, quoted literals,
comments and ambiguous function-template calls remain unchanged.

Thirty-one integer expressions run before/after over boundary-value pairs under
ASan/UBSan; four floating predicates include infinities and NaNs, and an effect
trace checks short-circuit evaluation order. Formatting is checked for idempotence.
The complete decompiler suite passes 165 methods; all 509 published DLL cases,
the source oracle and semantic mutation control still pass. Generated source
keeps 1,042 lines; negation wrappers in the recovered bodies drop from 28 to zero.
`build/predicate-format-summary.json` records exact identities and validation.

## Pointer types, shared regions and independent lifetimes

The next five passes were implemented in reverse difficulty order: native pointer
and object types, closed shared regions, independent definition/use lifetimes,
conventional loop headers and typed expression cleanup. Their implementation and
proof boundaries are described in [the refinement record](../../REVERSE_REFINEMENT.md).

| Complete generated file | Before | After |
| --- | ---: | ---: |
| Lines | 1,042 | 909 |
| Bytes | 62,884 | 58,978 |
| Bare declarations, including helper interiors | 54 | 21 |

The decoder now has const source/mutable destination pointers, typed cursors,
one bounded 16-element uint32 frame array and eight shared helpers at 16 call
sites. Forty-two reused carriers split into 50 additional lifetimes; more locals
now have local initialized declarations. Constant shifts use native operators,
and cancelling constant byte offsets disappear without moving a load snapshot.
This decoder's remaining post-test loops lack the current entry proof, so the
header pass preserves them; independent fixtures exercise its while/for recovery.

All 177 decompiler test methods pass with no skips. All 509 raw-DLL comparisons,
sanitizer repeats, source oracle and semantic mutation control pass. The mutation
generators accept native operators as well as helper
calls, with the same control inputs and failure requirements. Final test results,
baseline/stage/source hashes and exact implementation identities are retained in
`build/reverse-refinement-summary.json` and the current `build/pe-recovery-summary.json`.

## Direct loop exits and return expressions

The next cleanup lowers owner-tagged loop exits before value/CFG analyses. It
routes closed continuations directly on their selected edges and threads shared
terminal code through conditionals and trivial scopes. An enclosing break or
continue cannot become an inner-loop transfer. Shadowing, nontrivial loop-local
lifetimes, raw control flow and the expansion bound preserve a complete fallback.

Same-type adjacent copies can now feed a pure return expression without a staging
variable. Unknown calls, mutations, escaped destinations, type/CV conversions and
introspection retain their snapshots. A parenthesized `return (...)` is correctly
recognized as a statement rather than an opaque function call.

| Complete generated file | Previous | Current |
| --- | ---: | ---: |
| Lines | 909 | 859 |
| Bytes | 58,978 | 58,547 |
| `loop_exit_` references | 35 | 0 |
| Bare declarations, including helpers | 21 | 15 |

All 509 original-DLL comparisons still pass, including full destination bytes,
source/canary checks, ASan/UBSan, the independent source oracle and normal semantic
mutation rejection. All 179 decompiler test methods pass with no skips. Twenty-one
compiled routing scenarios and 17 return-expression scenarios additionally check nested ownership,
common returns, effect order, scope/shadowing, destructors, references, width/CV
conversions and parenthesized pointer returns. The exact final regression outcome,
source, binary and runner hashes are recorded in `build/exit-routing-summary.json`;
`build/pe-recovery-summary.json` identifies the current checkpoint. The preceding
source and report remain under `build/before-exit-routing-*`.

## Readability review

Three subsequent reviewer rounds target declaration noise, helper locality and
expression presentation. Recovered-body `maybe_unused` markers fall from 103 to
zero and `const` tokens from 218 to 128; read-only pointees and reference-sensitive
qualifiers remain. Five substantial helpers replace the previous eight, with
definitions below the input early returns at their first containing call site.
Small fragments stay inline. Known cast/call arguments and scalar expressions
lose redundant parentheses and gain consistent spacing while preserving widths,
evaluation, numeric bool values and comma arity.

The complete file is 871 lines / 56,979 bytes, compared with 859 / 58,547. This
batch deliberately favors locality and scanability over the smallest line count.
All 180 decompiler test methods pass with no skips. The same 509 DLL cases,
sanitizer runs, source oracle and mutation control pass.
See [the review record](../../READABILITY_REVIEW.md) and the final bound results in
`build/readability-summary.json`; `build/before-readability-*` retains the
checkpoint preceding that review.

## Unrolled copy recovery

Both 15-byte tail-copy staircases now become bounded forward `for` loops. Loads
and stores retain their byte order for overlapping LZ4 matches, the original
unsigned stop comparison, and the exact 15-byte cap. Outer exit transfers remain
outside each new loop. Changed offsets, live byte temporaries, escaped/volatile
bases or mismatched exits retain their original structure.

The complete file is now 717 lines / 46,458 bytes, down from 871 / 56,979.
All 181 decompiler test methods pass with no skips. The 509 original-DLL cases,
ARM64 sanitizer comparisons, source oracle and mutation
control pass unchanged. See [the implementation and regression coverage](../../COPY_LOOP_RECOVERY.md).
`build/copy-loops-summary.json` and `build/pe-recovery-summary.json` bind this
checkpoint; `build/before-copy-loops-*` preserves its predecessor.

## Agent cleanup experiment

A separate agent rewrote the recovered C++ without consulting the upstream
implementation. The candidate is 467 lines instead of the generated input's 717,
with typed copy helpers and one common match continuation replacing numbered
regions and duplicated paths. The existing 509 cases and an additional 444 cases
match the original Windows DLL on ARM64 and ASan/UBSan. The source oracle,
existing mutation control, and strict warning build also pass. The emitter and
canonical generated checkpoint remain unchanged. See the
[candidate, transformation notes and reproducible checks](idiomatic/README.md).

## Explicit scope

This is an isolated export closure, **not whole-DLL or Win32 process emulation**.
The DLL contains TLS metadata; this selected closure does not access it. The
reference does not run DllMain/TLS callbacks, initialize the CRT, register Windows
unwind handlers or deliver Windows exceptions. Unsupported imports terminate its
subprocess instead of pretending to succeed or throwing across raw PE frames.
Buffers are separate valid host allocations; image-address arguments, arbitrary
initial machine state, fifth/stack arguments and whole-process behavior are not
certified by this corpus.

The first native PE profile admits ordinary four-register integer/pointer calls,
explicit saved-GPR entry inputs, read-only `.rdata` literals before relocation
slots and modeled `msvcrt.dll` memory imports. It refuses TLS/segment reads,
unsupported imports/relocations, relocated instructions, unproven absolute image
constants, dynamic image-derived addresses, image writes and unsupported pointer
propagation between helpers. Ordinary XMM copies retain all 128 bits; aligned
copies require proven literal/frame alignment. Non-temporal, masked and other
SIMD operations remain unadmitted. Checked/flat emission does not silently ignore
aligned-copy fault semantics.

`build/report.json` contains the exact identities, entry state, host, per-case
hashes/results and mutation result; failures preserve a replayable
`build/failure.json`. These are bounded observations within this declared model,
not a universal equivalence proof or game validation.
