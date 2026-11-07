# Binary-to-C++ decompiler

Goal: accept PE, ELF, and Mach-O images; decompile selected regions or ultimately
whole programs into compilable C++; reproduce the original observable behavior.
Use SharpRetro's existing instruction semantics, then improve source through
analysis and checked refactoring, including LLM-assisted cleanup.

This project is independent of the previous per-function recovery and compiler
matching workflows. It does not invoke msvcdecomp, an MCP service, a Windows VM,
or an original compiler. macOS, .NET 10, Python 3, and Clang are enough to develop
and test the current pipeline.

## Current vertical slice

| Layer | Implemented | Limits |
| --- | --- | --- |
| Image intake | PE32/PE32+, ELF32/ELF64, thin Mach-O64; preferred VAs, permissions, file backing, SHA-256; Mach-O function starts; PE exports/IAT/runtime ranges/DIR64 metadata | Little endian; linked executable/shared images; no fat Mach-O or object-file linking |
| ISA frontend | Existing XFusion decode and `X86Lifter` into shared `LiftIl`; native source supports ordinary legacy 128-bit XMM copies | x86 32/64-bit integer/copy subset; general SIMD/FP remains unsupported; AArch64 intake is not connected to this frontend |
| Control flow | Recursive traversal, basic blocks, dominators, natural loops and structured regions inside recovered functions; native calls/tail calls | Ordinary selected regions still reject calls; indirect targets refuse; unsupported function graphs retain block gotos |
| C++ emission | Separate functions, direct inputs/results, SSA/private scalar promotion, native pointer roles, bounded frame arrays, observed object accessors, lifetime splitting and shared regions | Machine-bit result ABI; ambiguous values retain integers; no general class/layout recovery |
| Runtime boundary | Explicit guest memory interface and returned/boundary/budget outcomes | Caller supplies valid state/memory; no OS loader, dynamic linker, imported APIs, threads or exception delivery |
| Local comparisons | Clang-built x86-64 Mach-O dylibs compared with generated host C++ | Rosetta reference on Apple Silicon; bounded fixtures, not an x86 hardware or game-equivalence claim |
| CLI application recovery | x86-64 Mach-O `LC_MAIN` to native ARM64; native storage, explicit libSystem bridges, classic dyld rebases/bindings and initialized data | Explicit compiler/loader profiles; no general dyld/libc runtime, weak/threaded binds or arbitrary imports |
| Windows x64 function recovery | Named PE export to native C++; Win64 register ABI, literal data, memory import thunks and XMM copies; published LZ4 decoder compared directly | Selected stateless export profile; no DllMain/TLS/CRT/unwind/Win32 process emulation or general PE relocation execution |

Container support is separate from operating-system runtime support. Reading a PE
and translating a leaf does not supply Win32. ELF segment mapping does not supply
Linux syscalls. Mach-O section mapping does not implement dyld. Relocations,
symbol/import recovery and runtime initialization remain explicit work items.

The first complete executable experiment is now available: [compile an x86-64
CLI, decompile its binary, and run the recovered C++ as native ARM64](examples/cli/README.md).
It includes argument parsing, formatting, hashing, numeric summaries, sequence
generation and stdin/stdout/stderr. The generated program contains the translated
application logic plus an explicit memory/I/O adapter, with no original source
or x86 executable needed at runtime.

The first [published Windows x64 experiment](examples/lz4-pe/README.md) recovers
`LZ4_decompress_safe` from the official prebuilt LZ4 v1.8.3 DLL. All 509 decoder
cases match actual DLL machine-code execution under Rosetta against recovered
ARM64 and ASan/UBSan output, with a same-tag source oracle and mutation control.

## Build and use

```sh
dotnet build Decompiler/Decompiler.csproj -c Release

# Inspect preferred virtual addresses and file-backed executable regions.
dotnet run --project Decompiler -c Release --no-build -- inspect program.dylib

# Inspect actual lifted instruction semantics before emitting C++.
dotnet run --project Decompiler -c Release --no-build -- \
  lift program.dylib --range 0x560:48 --entry 0x560

# Emit a self-contained C++ header from the same bounded entry.
dotnet run --project Decompiler -c Release --no-build -- \
  decompile program.dylib --range 0x560:48 --entry 0x560 --output recovered.hpp

# Require structured control flow (refuse unsupported graph shapes).
dotnet run --project Decompiler -c Release --no-build -- \
  decompile program.dylib --range 0x560:48 --control-flow structured --output recovered.hpp

# Select the original dispatch emitter for diagnosis/comparison.
dotnet run --project Decompiler -c Release --no-build -- \
  decompile program.dylib --range 0x560:48 --control-flow dispatch --output baseline.hpp

# Recover an x86-64 Mach-O CLI through its main/direct-call graph.
dotnet run --project Decompiler -c Release --no-build -- \
  decompile-app original-x86_64 --output recovered.cpp
clang++ -arch arm64 -std=c++20 -O2 recovered.cpp -o recovered-arm64

# Keep the original machine-state application as an independent comparison.
dotnet run --project Decompiler -c Release --no-build -- \
  decompile-app original-x86_64 --representation machine --output machine.cpp

# Preserve the earlier flat, recovered-value emitter for comparison.
dotnet run --project Decompiler -c Release --no-build -- \
  decompile-app original-x86_64 --representation blocks --output blocks.cpp

# Recover a selected Windows x64 export, without invoking DLL initialization.
dotnet run --project Decompiler -c Release --no-build -- \
  decompile-function library.dll --export LZ4_decompress_safe --output decoder.cpp
```

Addresses above are examples: get the actual mapping with `inspect` and establish
the entry from symbols or disassembly. Addresses are preferred VAs, not file offsets
or PE RVAs. Repeated `--range VA:SIZE` options provide disjoint traversal bounds.
`decompile-function` requires an explicit `--export` or `--entry`; it never
silently selects the PE's CRT/DllMain entry. Its generated adapter exposes four
Win64 register arguments plus any supported recovered entry-state inputs, with
explicit defaults rather than assuming saved registers cannot affect a result.
Without `--entry`, the first supplied range start is used. Range bounds do not assert
that every enclosed byte is code or reachable. The tool reports the number of
reachable translated instructions, not a function or whole-program coverage count.

`--instruction-limit N` bounds static discovery. Unsupported decoding, overlapping
instruction streams, unsupported IL, atomic/vector prefixes, unresolved calls or
indirect targets fail with diagnostics. Source is assembled in memory before any
output file is written. The original image is never executed by the CLI.

## Recovered values and native storage

`decompile-app` defaults to `--representation recovered`, which now emits separate
functions as described below. The
`ValueRecovery` pass operates above instruction IL, using shared basic blocks for
both analysis and emission. It gives each reaching definition an immutable value,
propagates register copies across instructions, folds constants, and computes
inter-block liveness to remove dead register and flag calculations. Only values
that must survive a block edge require mutable locals. Parallel edge assignments
are ordered safely, with a temporary for actual copy cycles.

Native recovered functions additionally use `FunctionSsa`: function-wide reaching
definitions, explicit join values and virtual private stack cells. Entry arguments
remain distinct from later uses of the same physical register. The native
statement tree coalesces same-type carriers only after checking interference in
the scheduled-copy CFG. It places remaining carriers immediately before their
first use in their smallest common lexical scope and combines eligible first
assignments with declarations; sole definitions become `const`. This analysis is
described below; checked/flat output retains the preceding block-value model.

After coalescing, predicate propagation replaces saved `branch_condition` uses
only when the same total pure expression reaches the test on every path and
its operands retain their values and lexical bindings. Scope exits, loop
backedges, writes and unknown calls participate in that analysis. Dead saved
booleans then disappear. Needed snapshots stay explicit. Exact-type return
forwarding removes adjacent result staging, including complete tail branches;
normal declaration placement omits carriers that no longer have mentions.

Final predicate rendering complements integer comparisons and applies De Morgan's
rules with precedence-aware parentheses. It preserves arithmetic grouping and
boolean conversions used as values; unordered floating comparisons are excluded
from ordered-comparison inversion. Unknown syntax retains the original expression.

Expression cleanup removes final casts supplied by an exact-width assignment,
renders wrapped negative offsets as unsigned subtraction, and spells small
bit-clear masks as `~mask`. Nested arithmetic retains conversions needed for
C++ integer promotion, machine-width wraparound, and boolean low-bit truncation.
These rewrites do not infer source pointer types or replace overlapping accesses
with guessed algorithms. The separate native type analysis described next infers
pointer roles. The [published DLL fixture](examples/lz4-pe/README.md)
records the latest adversarial review and measured output.

Native type recovery distinguishes byte pointers from numeric observations,
propagates shallow constness through loads/phis/calls, and keeps ambiguous values
numeric. Observed nonoverlapping fields get named accessors; real contiguous frame
storage becomes arrays, with direct indexing only after a bound proof. Pointer
offsets retain unsigned modular address arithmetic; pointer roles alone do not
claim allocation bounds. Numeric overload adapters preserve existing machine-bit
clients, and return values retain their explicit machine-bit ABI.

Reaching-definition webs separate independent carrier lifetimes before scope
placement. Real branch/loop joins and escaped storage remain connected. A late
header pass recovers conventional while/for forms while preserving first-iteration
and continue behavior. Profitable repeated closed regions become local helpers
after all scope/flow analyses; generic reference arguments avoid reading an
uninitialized carrier while constructing the call. Native constant shifts and
composed byte offsets preserve widths, masks and out-of-range behavior.
See [the five-pass implementation and evidence](REVERSE_REFINEMENT.md).

Early exit routing now replaces compiler-generated loop exit tags with closed
continuations on the exact exiting edge. Common terminal suffixes propagate
through conditionals and trivial scopes, preserving local bindings, destructor
order and nested break/continue ownership. Unproved or excessive expansion keeps
the original selector dispatch. Adjacent same-type copies can also disappear
inside pure return expressions, including pointer/address arithmetic; conversions,
escaped storage and calls that can change the source retain their snapshots.

The final native presentation pass removes redundant declaration modifiers,
keeps small code fragments inline, places larger helpers near their first use,
and formats expressions while retaining exact conversions and reference-sensitive
qualifiers. See the [three-round readability review](READABILITY_REVIEW.md).

Unrolled forward byte-copy staircases can now become bounded `for` loops. The
rewrite requires consecutive offsets, dead byte temporaries, the same unsigned
stop test and an identical terminal continuation. It preserves each load/store
pair and the original unroll cap, including overlapping matches. See
[copy-loop recovery and its checks](COPY_LOOP_RECOVERY.md).

A separate [agent cleanup experiment](examples/lz4-pe/idiomatic/README.md) takes
that generated C++ to a named, typed representation with consolidated match
continuations. It passes the original DLL comparisons and additional cases;
the experimental rewrite is kept separate from deterministic emitter output.

The output reconstructs `if`/`else`, loops and guard clauses within each recovered
function. Unsupported graphs retain labels and direct transfers per function.
Recovered functions use native C++ calls and returns. There is no register array or per-
instruction dispatcher. Shared expressions are materialized once; small single-use
expressions are inlined. Exact subtraction/zero-test identities simplify common
conditionals, including signed `SF xor OF` comparisons after a checked match of
the full lifted formula and operand widths.

The application source uses native `argv`, extracted image arrays and native
objects owned by each C++ function. The default `--memory auto` attempts the native
memory reconstruction described below; unsupported frame patterns retain the
checked address layer. RIP-relative literals point into extracted arrays.
Memory loads/stores retain their order; loads are not commoned across stores.
Undefined-flag consumption and arithmetic checks remain explicit effects.

This is an intermediate source form. ABI carrier locals still meet at block
boundaries; saved-register values remain visible. Source-level argument types,
semantic names for recovered objects and struct fields,
general source type recovery and further expression cleanup remain
work items. Native pointer values follow the host's 64-bit little-endian layout;
programs depending on raw numeric image addresses or pointer ordering across
unrelated objects need further analysis. This is not a general guest address-space
replacement for arbitrary binaries.

The recovered-value profile refuses variable instruction-internal control and
conditional memory effects it cannot preserve, rather than hoisting effects out
of their conditions. It limits analysis to 2,048 blocks and 1,024 instructions per
block. Recovered and flat-block source execute without instruction counters or
an instruction limit. The separate machine backend retains its explicit bounded
execution/interruption API for diagnostic comparisons.

`--representation blocks` retains the earlier single-function value emitter and
return-address dispatcher. `--representation machine` retains the original
application adapter and broader instruction-level handling. Ordinary `decompile` retains its machine-state API
and structured region workflow. Explicit `--control-flow` modes apply to both
machine and recovered representations. Recovered `auto` structures each function
independently, `structured` refuses if any function cannot be structured, and
`dispatch` selects the preceding function-local block/goto baseline. The flat
`blocks` representation retains its existing control flow.

At the preceding value-recovery checkpoint, 351 instructions became 103 blocks and generated source
shrinks from 384,358 to 69,393 bytes (81.9%). All 155 CLI cases agree with the x86-64
reference, the machine baseline and the ASan/UBSan recovered build. The full local
suite now has 29 passing pipeline tests plus the existing XFusion tests. New
synthetic checks cover all 65,536 byte operand pairs for each of signed `<` and
`>=`, 32/64-bit signed boundaries, partial-register writes, parallel-copy cycles,
unaligned accesses, overlapping stores, read-only objects and undefined flags.
These are optimization checks in stated domains, not whole-game evidence.

## Function recovery

`FunctionRecovery` discovers entries from the selected entry, direct CALL targets,
and reachable Mach-O `LC_FUNCTION_STARTS` metadata. Local function symbols and
decompiled source names are not required. The parser validates delta encoding,
termination, overflow and executable backing; an entry inside a recovered
instruction refuses. Blocks have one function owner. Transfers to another known
entry become tail calls. Ambiguous shared tails refuse instead of inventing an
ownership boundary; the flat representation remains available for those inputs.

Input and result contracts are inferred to a fixed point over the call graph,
including recursive and mutually recursive components. A function receives only
values required by its body or outputs. Callers retain values a callee does not
write. Modified values needed after a call are returned explicitly: no observed
result gives `void`, one gives a scalar, and several give a small result struct.
Flags and flag-definedness can cross a boundary as explicit boolean values.
Conservative may-write summaries preserve conditional and narrow-register outputs.
Bitwise truncation simplification removes false inputs from discarded high bits.

Every native function has its own C++ locals and forward declaration. Its
signature contains only recovered input values; results use ordinary C++ returns.
There is no call-context parameter, context allocation at call sites, hidden
thread-local context, runtime call-depth counter or cumulative frame-storage
counter. Bodies use their value parameters directly, including assignments to
those parameters, instead of copying them into a second set of carrier locals.
Parallel edge assignments and demanded multi-value returns retain their existing
contracts. `RET imm16` and tail cleanup are established by frame analysis.
Native recursion follows ordinary host C++ execution and stack limits.

`--memory checked` and automatic fallback retain the previous `CallFrame` contract:
an original stack pointer, numeric return observation and depth. Physical
stack operations retain aliasing and immediate cleanup. Each call must return to
its recorded continuation; redirected returns fail that profile. Tail calls reuse
their caller's original return slot. This diagnostic form retains the depth cap
of 512; it is independent of the native function interface.

Direct-call tests include nullary functions, scalar parameters modified by the
body, and void functions that mutate a pointer argument, called directly from
C++ without `recovered_program` or a context adapter. A finite recursive fixture
also checks inputs at depths 513, 1,024 and 2,048. The CLI fixture still matches
all 155 cases across the native, sanitizer and diagnostic builds.

These signatures are exact-bit value contracts for the emitted code, not claims
about original C++ prototypes. Pointer types, source names and saved-register
frame elimination remain unknown where not established. ABI carrier parameters
and multi-value results can therefore remain even for routines that originally
had a simple source signature.

The CLI now produces eight C++ functions across its 103 blocks. All 155 cases
match the x86-64 reference, function-based ARM64 output, its ASan/UBSan build,
flat block output and machine baseline. Tests separately cover scalar/void/multi-
value results, flags across calls, recursion, mutual recursion, metadata-only tail
entries, callee-saved values, immediate stack cleanup and explicit failure paths.
A Clang-built recursive Mach-O executable is also compared with its recovered
ARM64 build on 12 inputs. These are bounded integration observations, not a
universal function-equivalence proof.

## Native memory reconstruction

`--memory native` requires native reconstruction; `auto` tries it first and
reports any checked-memory fallback. These modes apply to recovered application
source. `blocks` and `machine` retain their preceding memory models.

`NativeMemoryRecovery` propagates frame-relative offsets through each function's
CFG. Stack heights must agree at joins. CALL/RET bookkeeping is removed after
checking canonical return frames, constant cleanup (including `RET imm16`) and
restored tail-call frames. Known accesses to return slots/incoming stack arguments,
dynamic stack allocation, stored/returned addresses of a function's own frame,
and inconsistent layouts cause fallback or strict refusal before publication.

Each needed frame becomes a native automatic `FrameStorage` object. Known slots
become unsigned typed fields; intervening buffers remain byte arrays. Compile-time
`offsetof` assertions check the original offsets. All views share that one object,
so a byte pointer passed to another function can modify a field later read by a
direct assignment. Narrow/overlapping views and unaligned accesses use direct
Clang/GCC `may_alias`, alignment-one integer views. There is no memory-span
registry, `read_memory`/`write_memory` layer, shared one-MiB byte stack, synthetic
return slot or emulated stack pointer in native output. Functions without frame
storage allocate none. Original entry alignment is retained modulo 16 bytes.

This is a native pointer execution profile: accesses require valid native object
bounds and lifetimes. General pointer bounds, indirect aliases to control slots
and raw numeric stack-address observations are not established by this analysis.
The checked form retains access/permission checks and numeric return validation
for investigations that require that execution model. Most pointer values still
use integer ABI carriers; semantic pointer/array/struct type inference remains.
Native frames retain the current deterministic zero initialization rather than
recovering uninitialized C++ source behavior. Frame-offset analysis is capped at
one MiB per function; emitted native calls do not track a cumulative storage limit.

The CLI fixture emits four frame objects with 15 typed stack fields across its
eight functions. All 155 cases agree with x64/Rosetta, native ARM64, ASan/UBSan,
checked function output, flat blocks and the machine baseline. New focused tests
cover field/byte-pointer aliases across calls, overlapping unaligned views,
recursive frame independence, tail cleanup and unsupported-layout refusal. A
separate Clang-built pointer-alias fixture matches x64 on 24 inputs under ARM64
ASan/UBSan. These observations are bounded checks, not a general memory-equivalence
proof.

## Off-the-shelf CLI regression

The [upstream b64 fixture](examples/b64/README.md) builds pinned, unmodified
MIT-licensed `iaseth/b64` sources for x64, decompiles the executable and builds
standalone ARM64, ASan/UBSan and checked-memory outputs. All 292 cases compare
stdout, stderr, exit status and files, with independent Base64 oracles and a
changed-codec mutation control. It exercises 542 reachable instructions and 158
basic blocks across four structured functions on the current compiler profile.

This added per-import libc contracts (including four-argument stdio and void
results), typed register-vararg formatting for `%s`/`%c`/`%%`, classic dyld fixup
streams, initialized image data, standard-stream bindings, and ordered conditional
division-fault guards. The fixture documents each limit and retains upstream
error-status quirks. Legacy machine/flat-block app adapters keep their narrower
import/data scope; new bridges do not silently change their execution model.

## Structured control flow in recovered functions

Function-local graphs use a call's recorded return continuation as its successor;
the callee remains a separate native function. The region pass recovers natural
loops, joins and conditional arms above the existing recovered values. Ordered
memory effects and input/result contracts stay intact.
Terminating arms become guard clauses where possible to avoid deep `else` nesting.

Loops with several distinct exits record an exit selector before `break` and
execute the selected continuation after the loop. Nested loop selectors have
separate scopes. Shared exit paths first attempt recovery without copies. Where
needed, bounded block duplication permits those paths to appear in separate arms;
each copy keeps its original operations, and only the selected arm executes.
The copying fallback tries three independent join policies: first shared
continuation, admissible immediate postdominator, and shortest shared distance.
Each complete candidate must pass the same edge validation and expansion bound.
The smallest valid result is selected by copied blocks, region nodes and depth;
exact ties retain the first policy. Invalid candidates cannot replace a valid
baseline, and no policy changes the recovered operations or input domain.
The checker verifies every copy's original block identity and exact incoming and
outgoing destinations, all branch arms, loop exits and native call continuations.
It also requires coverage of every original block. This validates the graph
rewrite, not the underlying instruction semantics.

Analysis is limited to 512 blocks per function, 128 region levels and a total
emitted block count at most four times the original count. Unsupported or excessive
expansion falls back to the complete block/goto form for that function in `auto`;
strict mode fails before publishing source. No blocks are silently discarded.

The CLI fixture structures all eight functions into 10 loops and 46 conditional
regions, with 19 copied block instances and no gotos. All 155 cases still agree
with x86-64/Rosetta, ARM64, ASan/UBSan, flat blocks and the machine baseline.
Reversing a generated branch is rejected by the comparison, as is changing its
arithmetic. Focused tests cover nested loops, break/continue, early returns, calls
and byte-memory effects inside loops, multiple exits with distinct effects,
irreducible-graph fallback and strict refusal.

## Source-tree cleanup

Structured function output is lowered into a separate C++ statement tree after
CFG-region validation. `CppSourceTree` flattens redundant sequences and scopes,
removes empty branches, inverts empty true arms, produces guard clauses and
`else if` chains, and discards statements after unconditional exits. Pure
conditions consume values computed by earlier statements; memory/flag effects
remain in place when a branch becomes empty.

Every generated temporary declaration carries its name as metadata. Scope
flattening reserves existing declarations (including later ones) and keeps a
scope when flattening would collide with another declaration. Guard rewrites
preserve an arm's scope before trying to flatten it, so hoisting a continuation
cannot silently change shadowing. This pass only flattens compiler-created
scopes containing trivial temporaries; frame objects and control-flow bodies
retain their meaningful scopes. Calls, stores and result temporaries have unique
names within each function. Goto fallback bodies keep their original scopes.

Cleanup also carries exact abrupt continuations through tail conditionals and
trivial scopes. Long one-arm staircases become inverted guards, and small arms
before a common exit can finish early while the large arm falls through. Only
the exit is duplicated; intervening effects prevent threading. Value-bearing
exits cannot cross declarations that could capture an identifier, and later
uses of ancestor names prevent unsafe scope flattening. Nested loops receive
their own continuation instead of inheriting an outer break or continue.
Adjacent one-arm pure conditions can combine using short-circuit `&&`.

Exact `while(true)` tail-test shapes become `do ... while` only when there are
no earlier continues targeting that loop and the test does not depend on a
body-local declaration. Existing statement, load/store and commit order stays
intact. In particular, a comparison snapshot is not moved past its register
updates. The [published PE example](examples/lz4-pe/README.md) records four
adversarial review rounds and the resulting size/nesting improvements.

The cleaned CLI fixture is about 1,030 lines versus 1,571 at the native-memory
checkpoint, with no instruction-budget bookkeeping or per-block wrappers.
Tests compile unmodified and cleaned trees under ASan/UBSan and compare explicit
oracles for declaration shadowing, sibling collisions, guard hoisting, ordered
effects, empty arms, nested conditionals, break and continue. A generated loop
also runs beyond the former 100-million-instruction ceiling in both recovered
and flat-block representations. The usual 155 executable comparisons and
arithmetic/branch mutation controls cover the integrated output.

## Function-wide SSA and private scalar promotion

`FunctionSsa` overlays immutable block values without changing their original
load, check, store or call evaluation points. Block-entry joins have explicit
predecessor-edge operands. The external entry is separate from a backedge into
the entry block, and recursive call targets are not intraprocedural predecessors.
Calls consume pre-call definitions and produce fresh modeled result projections.
Trivial joins collapse; remaining phi transfers execute only on the selected
edge and use parallel-copy scheduling, including snapshots for actual cycles.
Tail-edge functions currently keep their complete preceding value emitter.

`NativeScalarPromotion` proves which exact typed frame cells can become virtual
values. It tracks frame-derived bits, bounded address ranges, definite full-width
initialization, overlaps, sticky exposure and explicit memory-bridge spans.
Partial/dynamic views exclude intersecting cells. Opaque helper observations and
escaping frame bits veto promotion, including bits selected through a conditional.
Branch bounds apply only to the exact tested value; an update after the comparison
cannot inherit its old bound. Indexed tables with overlapping views retain native
storage. Promoted loads refer to the definition at their original read point,
so a later store cannot change an earlier snapshot.

This uses the existing native profile: valid external pointers do not designate
a fresh private frame through guessed numeric addresses. It does not establish
nonaliasing for arbitrary integer address fabrication. The baseline frame is
value-initialized; while unexposed, bounded frame reads cannot acquire frame-origin
bits unless such bits were stored. That provenance fact is separate from the
definite-initialization requirement for promotion; it does not invent zero-valued
SSA inputs or certify original uninitialized-stack behavior.

Private stores are not observable roots once the cell is proven isolated. Demand
flows backward from actual effects, checks, conditions, calls and result contracts
through value and memory joins. Dead spill-only parameters disappear through a
monotone call-graph fixed point. A genuinely observed nonargument machine input
stays explicit; this is not an ABI-preservation guess. A wholly unnecessary native
frame can disappear. Remaining frame fields preserve their original byte layout.

`CppVariableScopes` uses the complete statement tree to find the smallest common
lexical scope for each carrier. Only a sole direct assignment with no prior use
can become a const declaration. Address-taken values, including `memcpy` targets,
remain mutable. Duplicated regions and conditional definitions retain storage
where lexical dominance cannot be established.

Phi copies are tagged compiler-owned local transfers. Cleanup may thread an exact
ordered copy/exit bundle through a tail guard; arbitrary effects and cycle-breaking
declarations block this rewrite. Post-test recovery keeps copies conditional and
checks that they cannot change any operand of the saved loop test. Comparison
snapshots remain whenever an intervening update can change an operand or binding.

The published LZ4 decoder now takes four inputs instead of twelve, with 17 of its
21 typed frame cells promoted. Its four indexed 128-bit table fields remain memory.
Exact-width integer carriers, generic value names and 72 copied CFG blocks remain.
At that checkpoint, source typing and carrier coalescing were further work items. This is a value-recovery
foundation, not yet idiomatic algorithm-level reconstruction.

The full local decompiler suite passes 156 tests at this checkpoint, including
ten SSA/private-promotion fixtures and the published DLL's 509 decoder cases.
The source-tree harness checks 37 shapes before cleanup, once and twice, plus
an opaque address-taken write. Independent runtime oracles retain fault checks,
partial/overlapping memory behavior, selected loop exits and call observations.

## Direct native views and loop-copy cleanup

Native function bodies now use direct typed dereferences rather than aliasing
`memcpy` scaffolding. `u8`, `u16`, `u32`, `u64` and `u128` are integer typedefs with
`aligned(1)` and `may_alias` attributes, used directly at each `reinterpret_cast`.
Their widths and alignment are checked with static assertions. Recovered values
keep their ordinary exact-width carrier types; only the memory views carry these
attributes. This is an explicit Clang/GCC extension profile, not portable ISO C++.
The current execution evidence uses Apple Clang on ARM64 macOS.

Loads remain snapshots at their original evaluation points, before later aliasing
writes. Store width comes from the lifted access, independent of the C++ literal's
type. No sanitizer checks or strict-aliasing optimizations are disabled. A noinline
mixed-width test covers all five views at offsets 0–31, including overlapping
128-bit accesses, under `-O3 -fstrict-aliasing -fsanitize=address,undefined`.
Actual imported `memcpy` operations retain their library-call semantics; the
removed copies were implementation scaffolding for fixed-width accesses.

`CppLoopCopies` removes a final `if(C) { copies; }` from `do ... while(C)` when
the old destinations are dead on the false exit, copy inputs are definitely
initialized there, and no copy changes the condition. Its occurrence-based CFG
models nested loop tests, backedges, breaks, continues and returns. Backward
liveness includes later enclosing-loop iterations; forward must-definition meets
all incoming paths and respects lexical scopes. Only tagged compiler-owned pure
copies qualify, and their already scheduled order stays unchanged. Address-taken
destinations, uncertain initialization and unsupported labels/gotos retain the
preceding representation.

The LZ4 body has no aliasing `memcpy` scaffolding after this pass. Nine of its
17 guarded post-test copy bundles become ordinary tail updates; eight retain
their guard because the analysis cannot justify speculation on the false exit.
All 157 decompiler tests passed at that checkpoint, including 45 before/once/twice source-tree shapes,
the strict-optimization memory-view test and all 509 published decoder cases.

The [b64 review iterations](examples/b64/README.md#independent-review-iterations)
add typed removal of redundant arithmetic/boolean casts and unused native scalar
field reads, exact predicate canonicalization, common statement tails, redundant
loop continues, and call results filtered by continuation liveness. Calls and
checked return-context updates remain even when their results are discarded.
Width-changing conversions and narrow unsigned promotion remain;
dead-read eligibility is limited to initialized in-bounds nonvolatile frame fields
with no actual consumers. Generic memory evaluation and fault effects retain
their original handling. Snapshot/alias and arithmetic-width tests exercise these
boundaries independently of the executable comparison corpus.
Common-tail extraction additionally requires trivial compiler temporaries with
no escaping addresses; future source-buffer promotion must not silently weaken
that contract. The review record identifies the alias, type and region-ownership
proofs needed for larger improvements.

## Frame identities and private memory

Native function recovery now runs `FrameProofs` before contracting register
inputs/results. It tracks full-width entry identities, constant frame offsets,
constants, unsigned bounds, local stream validity and unknown values through the
complete lowered CFG. Exact frame cells
hold identities until an overlapping byte write invalidates them. Loads take
snapshots at their original evaluation point; register outputs commit in parallel.
Joins retain only identities agreed by all incoming paths and union sticky escape
history. Every reachable return must establish the same entry identity before a
written RBX, RBP or R12–R15 output can be omitted. Narrow restores do not qualify.

Interprocedural summaries start empty and grow only from established proofs, so
recursion cannot bootstrap preservation. A separate private-memory summary means
the callee neither writes nor captures caller-owned memory; absence of access to
its own frame is insufficient. Unknown stores invalidate frame cells. Unproved
internal calls do likewise. Explicit `_read`, `_write` and `_strcmp` bridge
contracts are noncapturing and callback-free: a bounded `_read` invalidates its
possible destination bytes, an unknown range invalidates all cells, and the two
read-only operations do not change guest memory. Imported volatile registers are
invalidated independently of their demanded result contract.

Opaque stdio is deliberately different. A `FILE*` can hide aliases or callbacks;
exposing a frame-derived or unknown argument invalidates its identities, and that
escape history survives later calls and CFG joins. Such a call can preserve an
unexposed fresh frame, but never earns a private-memory callee summary. Whole
program stream provenance is still missing, so the upstream b64 encoder/decoder
do not gain saved-register contractions from this pass. Tail transfers and
recursive cycles without an independent proof remain conservative too.

After contraction, `NativeDeadEffects` removes unused initialized scalar reads
using the emitter's exact existing eligibility rule. It then computes backward
byte liveness: a frame store can disappear only if none of its bytes can be read
later. Exact stores kill byte liveness, branches union it, and unknown loads,
internal calls, opaque calls and tails make the entire frame observable. `_write`
observes its exact buffer when known, otherwise the whole frame. `_read` never
kills old-byte liveness because it can return short or zero input. Calls, generic
loads and fault effects remain. A dead store containing division therefore still
evaluates its division and fault checks.

Each removal uses its ordinal in the original deterministic lowering, preventing
identical statements at different evaluation points from being conflated. Fresh
function contracts and native frame plans are rebuilt until no further effects
can disappear. Checked-memory recovery keeps its original contracts/effects.
The dead-effect pass conservatively skips functions with frames above 4 KiB or
`frame size * block count` above 256 KiB; this is an optimization limit, not an
execution limit. Ordinary execution has no new budget.

Twenty focused tests cover overlapping/unknown stores, full versus narrow
restores, parallel swaps, path joins, load snapshots, sticky escapes, recursive
and tail rejection, call-summary propagation, observed output, short input and
preserved faults. They also compile a contracted helper with no input parameters
under ASan/UBSan and compare its caller against an independent arithmetic oracle.
The normal 155-case CLI and 292-case upstream b64 round trips continue to check
the integrated native output against x86-64 execution through Rosetta. These are
bounded observations of these fixtures, not a proof of game behavior.
The initial frame-proof checkpoint passed 103 tests on macOS ARM64, including
20 new analysis tests. Local stream provenance extends that checkpoint below.

### Local stream provenance

An `fopen` result now carries a nullable stream-validity predicate through exact
64-bit copies, conditional joins and frame spills. It neither asserts nonzero
nor identifies an allocation. Entry `FILE*` parameters, standard-stream bindings
and streams returned by other recovered functions remain unknown. This first
scope requires no new precondition on recovered function inputs.

Calls on a locally proven stream have explicit effects: `fgetc` reads no guest
frame bytes, `fread` may overwrite its bounded buffer, `fwrite` observes its
possible buffer range, and `fprintf` may read its format/argument strings.
`fclose` invalidates every stream alias. Unknown stores, unknown-buffer writes,
unproved calls and `free` revoke all stream facts too. Revocation includes state
registers, spill cells, cached earlier loads and later reads of immutable block
inputs, so closing and reopening cannot revive an old handle. Narrow or partial
pointer overwrites cannot retain stream validity.

`fopen`/`malloc` use noncapturing guest-memory contracts. A factory's strings are
known not to read this fresh frame only when their pointer origins are nonframe
and the frame has not escaped; otherwise its read footprint stays unknown.
Trusted callsite observations are recorded only after CFG meets stabilize, then
consumed by the native byte-liveness pass. Opaque calls keep the conservative
handling above, and all calls/error paths remain present.

Completed `fread`/`fwrite` element counts have unsigned bounds `[0,count]`.
Narrow carriers and nonwrapping unsigned addition/multiplication can provide
further upper bounds; unsupported or overflowing arithmetic becomes unknown.
Byte bounds use a 128-bit size-times-count calculation and the existing transfer
adapter limit. Short/zero reads never overwrite old-byte liveness.

The [stdio CLI](examples/stdio/README.md) independently exercises two locally
opened-stream helpers. Its recovered signatures have only path inputs and scalar
results, with six written-register identities and 12 private dead effects
removed. All 32 file/exit comparisons pass on native ARM64, ASan/UBSan and checked
output; an intentionally corrupted pattern is rejected. New unit cases cover
close/reopen lifetime, cached aliases, partial pointer corruption, path joins,
unknown writes into stream objects, zero transfers and wrapped count arithmetic.
The upstream b64 streams remain unproved at their entry interfaces.
The local-stream checkpoint passes all 116 decompiler tests on macOS ARM64,
including 31 frame/stream analysis tests and the new executable fixture.

## Structured control flow in machine representation

`--control-flow auto` is the default. The pipeline first groups instructions into
basic blocks, computes dominators and natural loops, and finds branch joins using
postdominator information and bounded searches within each region. Shared loop
tails remain shared even when an early `break` bypasses them. It then builds an
explicit sequence/conditional/loop AST above the unchanged instruction IL.

Before emitting that AST, a separate checker verifies that every block appears
exactly once and that every lexical fallthrough, taken/untaken conditional edge,
loop back edge, loop exit, return, and untranslated boundary retains its original
destination. This is a graph-rewrite check; it does not validate instruction semantics.

Supported graphs emit nested `if`/`else` and `while(true)` with explicit `break` and
`continue`. Pre- and post-tested loops both use this form so original header/latch
effects and instruction budgets execute at the right times. Conditions still read
the lifted register/flag state; simplifying them to source expressions comes later.

The original instruction dispatcher remains available as `run_dispatch` in the
header. Normal `run` uses the structured body from the selected entry. If a caller
resumes at a different instruction PC, it delegates to dispatch without replaying
earlier instructions. Per-instruction budget exhaustion preserves the same state
and PC in either path.

Graphs with multiple-entry cycles, loops with multiple distinct exit targets,
instruction-internal transfers, unsupported shared regions, more than 512 blocks,
or more than 128 levels of region nesting retain complete dispatch output in
`auto` mode. `structured` mode refuses those shapes. `dispatch` mode explicitly
selects the baseline. The CLI and emitted header report the chosen form or fallback
reason. Structuring failure never drops blocks or suppresses unsupported semantics.

## Execution contract of the initial source form

The emitted header defines `sharp_retro::State`, `Memory`, `entry`, and
`Exit run(State&, Memory&, uint64_t instruction_budget)`.

* Registers use XFusion's numbering: RAX, RCX, RDX, RBX, RSP, RBP, RSI, RDI,
  R8 through R15. Flags and segment bases are separate state. Subregister merging
  and 32-bit zero-extension come from the existing lifter.
* Set `state.pc` to the desired translated entry. Supply a guest stack, including
  a return address, and initialize argument registers according to the actual
  guest calling convention. No signature or calling convention is guessed.
* Implement `Memory::load`/`store` using virtual guest addresses, widths in bits,
  and little-endian bit patterns. Memory implementations must enforce their own
  bounds and access permissions. Guest addresses are never host pointer casts.
* Integer values use unsigned bit patterns through 128 bits. Width masks,
  sign extension, arithmetic shifts and signed comparisons avoid C++ signed
  overflow and out-of-range shift operations. `unsigned __int128` currently
  requires Clang/GCC; an MSVC backend is not claimed.
* Operands and stores retain LiftIl evaluation order, including memory loads.
  Undefined flag writes invalidate those flags; reading one raises an explicit
  error. The backend does not silently choose a defined zero for undefined data.
* `Exit::returned` means a translated RET executed and records its target in
  `state.pc`. `Exit::boundary` means execution reached an untranslated address.
  `Exit::budget_exhausted` is an incomplete execution, not a successful return.
  Unsupported effects, traps and faults refuse translation rather than returning
  a fake success. Platform fault delivery and defined fault outcomes are pending.

The values are still intentionally in an executable low-level form. Keeping them
separate from the structured control-flow AST lets register recovery, ABI adapters,
dead-flag elimination and source expression recovery evolve as independent passes.
Compiler optimization can optimize the baseline, but is not evidence that the
translation was correct.

Application recovery is opt-in via `decompile-app`. It follows direct call targets
as well as their return continuations; instruction-local constant copies are
resolved without inventing indirect targets. Internal RETs resume translated
callers, and the main adapter requires the final return to its explicit sentinel.
Its stricter execution and import/data scope is documented in the
[CLI experiment](examples/cli/README.md#adapter-scope). Ordinary `decompile` keeps
its existing selected-region contract and still refuses calls.

## Reproducible checks

```sh
dotnet build Decompiler/Decompiler.csproj -c Release
python3 -m unittest discover -s Decompiler/tests -v
dotnet test XFusionTests/XFusionTests.csproj -c Release
```

The integration tests build a real x86-64 Mach-O dylib for each of four C++
fixtures: arithmetic/loop/rotate, loop/memory updates, nested loops with conditional
volatile memory accesses, and a loop with `break`/`continue`. A separate x86-64
harness invokes **the exact dylib whose bytes were translated**. Generated C++
is compiled for the host with Clang, AddressSanitizer and UndefinedBehaviorSanitizer.
The two paths use the same 256 deterministic input pairs per fixture, including
edge bit patterns. Tests compare returns and exposed memory and check guest stack
balance, return targets and nonvolatile registers. Every fixture must actually
produce structured source. Changed generated arithmetic and a reversed structured
conditional edge must each fail the same comparison.

Nine additional synthetic graph shapes exercise diamonds, nested conditionals,
pre-/post-tested loops, nested loops, `break`/`continue`, early returns, a divergent
path and a shared latch bypassed by an early break. For each shape, 54 input pairs
are compared against dispatch at each budget from 0 through 96, including every
register, flags and flag-definedness, PC, memory, and ordered memory effects.
Terminating cases also check independently specified results and instruction-by-
instruction resumption. A separate loop-memory fixture adds 808 interrupted-state
comparisons with ordered reads/writes. The total is 47,950 bounded state/trace
comparisons, in addition to the native fixture checks. Irreducible/multiple-exit
graphs and the analysis limit exercise complete fallback and strict refusal.

Other checks cover all five implemented format/bitness combinations with constructed
containers, malformed/truncated mappings, unsupported semantics, overlapping code,
static and dynamic budgets, partial exits, and undefined flag consumption.
Constructed containers exercise intake and emission; they are not native PE/ELF
platform execution. Native fixture tests explicitly skip outside macOS.

Structured-control-flow checkpoint (before app recovery): macOS ARM64, .NET SDK 10.0.101,
Apple Clang 21.0.0. Release build completed with zero warnings/errors. All 15
pipeline tests and all 444 existing XFusion tests passed, with no skipped tests
on this host. Four native fixtures agreed across 256 cases each (1,024 total),
and all arithmetic/conditional mutations were rejected. Original x86-64 execution
used Rosetta; generated C++ used native
ARM64 with ASan/UBSan. These are bounded observations, not universal equivalence.

The subsequent application checkpoint adds five integration tests. It compiles
the actual CLI to x86-64, recovers 351 reachable instructions, then compiles the
generated C++ to native ARM64. All 155 CLI cases match stdout/stderr/exit status
in normal and ASan/UBSan builds. Stripped-symbol recovery, unknown imports,
unresolved targets, modified import stubs, mismatched bindings and unsupported
initialized data are also tested. See the retained local `examples/cli/build/report.json`
and its [reproduction instructions](examples/cli/README.md).

While adding the undefined-flag check, we found that two-/three-operand IMUL's
shared lowering updated CF/OF but incorrectly left PF/AF/ZF/SF preserved. The
lowerer now marks those values `IlUndefined`, consistent with one-operand multiply
and Intel's [IMUL flags specification](https://www.intel.com/content/dam/www/public/us/en/documents/manuals/64-ia-32-architectures-software-developer-vol-2a-manual.pdf).
This is a model correction, not a measurement of hardware-dependent flag values.

## Next milestones

1. Extend pointer-role/observed-field recovery to aggregate values, richer
   signatures and object relationships with evidence for each layout claim.
2. Expand the current direct-call and ABI profiles to additional argument/result
   classes whose parameter/return/storage contracts can be established.
3. Propagate expressions while preserving load and condition snapshots; factor
   additional duplicated CFG regions and expand coverage to more exit shapes.
   Compare transformed builds with their baseline and runnable original binaries.
4. Connect additional ISA lifters, expand integer/vector/floating-point effects,
   and add independently compiled native-host fixtures. Matching the lifter does
   not establish that the lifter matches hardware.
5. Add symbols, relocations, imports, indirect-target analysis, initialized runtime
   boundaries and multi-entry/whole-image coverage accounting. Unknown remains
   explicit; incomplete selected ranges cannot become whole-program success.
6. Use LLMs on compilable recovered source with bounded transformation scopes and
   executable regression feedback. Keep regenerated baseline source reproducible.

The existing DAO and compiler projects can supply integration targets later.
Their Windows execution and game initialization are not prerequisites for these
decompiler passes or their macOS compiler round-trip tests.
