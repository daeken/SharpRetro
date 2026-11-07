# Refinement in reverse difficulty order

Requested order: pointer/object types, shared regions, independent lifetimes,
loop headers, typed expression simplification. Each stage preserves the original
binary, comparator and corpus. Final validation runs against the integrated tree.

## 5. Native pointer and object types — implemented

- `NativePointerRecovery` infers full-width pointer roles and shallow constness,
  including anchored loop induction, loads, phis and call aliases. Ambiguous
  address expressions retain integers; numeric observations retain address bits.
- Native function parameters, SSA carriers and load temporaries use byte pointers.
  Constrained integral overloads preserve the existing machine-bit entry interface;
  return values retain their explicit machine-bit ABI.
- `byte_offset` retains unsigned modular address arithmetic. Pointer roles alone
  never authorize in-bounds C++ pointer arithmetic.
- `NativeObjectViews` names observed nonoverlapping external fields through modular
  accessors, without pretending that an unknown base is a C++ allocation boundary.
- `NativeFrameArrays` groups real contiguous frame storage; direct array indexing
  additionally requires exact SSA identity and a proven unsigned bound.

Published LZ4 checkpoint: const source/mutable destination, typed cursor values,
one 16-element `uint32_t` table array, four proved indexed accesses; all 509 raw-DLL
comparisons, sanitizers, source oracle and semantic mutation control pass.
Retained artifacts: `examples/lz4-pe/build/native-types-final/`.

Permanent coverage adds 33 pointer scenarios, 14 frame-array scenarios and four
compiled native type/view tests, including aliasing at `-O3 -fstrict-aliasing` with
ASan/UBSan. The combined native-types/shared-regions integration suite passes all
172 methods. An unused environment declaration found by the new strict fixture
is marked `maybe_unused`.

## 4. Shared control-flow regions — implemented

Implementation: `CppSharedRegions` extracts profitable repeated terminal or closed
normal regions after scope placement/boolean rendering. Explicit free locals use
generic references, so call construction reads no uninitialized values. Only
preexisting parameters/frame/views may be captured. External control transfers,
interior address/lifetime sensitivity and introspection prevent extraction.

Published LZ4: eight helpers replace 18 occurrences, saving 91 printed lines
(1,051 to 960) and 5,521 bytes (65,854 to 60,333). All 509 comparisons, sanitizers,
source oracle and mutation control pass. Eighteen compiled adversarial scenarios
add independent value/trace/throw oracles and lexical/type/control refusal checks.

## 3. Independent variable lifetimes — implemented

Build occurrence-level reaching definitions and definition/use components for
compiler-owned carriers. Split unrelated components; preserve actual loop/branch
joins and exposed storage. Run before declaration placement and region extraction.

Published LZ4: 42 reused carriers split into 50 additional locals; bare declarations
fall from 55 to 21 (17 in the main body, four in helpers), and the file from 960
to 909 lines. All 509 raw-DLL comparisons, sanitizers, source oracle and mutation
control pass. Twenty-four compiled adversarial scenarios preserve joins, loops,
scheduled swaps, snapshots, compound writes, aliases, types, literals and effects.

## 2. Conventional loop headers — implemented

Recover top-tested loops and induction headers with exact continue ownership.
Preserve do/while first-iteration behavior unless the entry test is proved true.
Exact matching guard + do/while can also become one pre-tested loop without an
extra test. Header declarations retain live-out/escaped storage; nontrivial
body lifetimes and same-loop continues prevent moving the update.
Seventeen compiled scenarios cover those proofs, including escaped references,
octal literal refusal and destructor effects. Existing native function fixtures
now produce conventional while/for headers. This LZ4 build's remaining loops
do not qualify, so this stage intentionally leaves its output unchanged.

## 1. Typed expression simplification — implemented

Use native shifts where operand width and constant count prove equivalence;
retain truncations before widening, signedness and fault/evaluation order.
Clean modular byte offsets and redundant conversions without moving loads.
Nested unmaterialized constant byte offsets compose modulo 2^64; materialized
values and load snapshots stop composition. A 510-shape compiled shift matrix
checks cross-width 1–128-bit results against an independent bit-position oracle,
including dynamic/oversized counts and small 128-bit literal operands.

The same shift-direction mutation controls support helper and native-operator
spellings, skipping comments/literals/templates. Their control inputs, original
binaries, corpora, comparators and normal semantic-mismatch requirement stay fixed.

## Integrated result

The complete LZ4 output falls from 1,042 lines / 62,884 bytes to 909 lines / 58,978
bytes. Bare declarations, counting helper interiors, fall from 54 to 21. It has
eight shared helpers at 16 call sites, typed input/output cursors and one bounded
frame array. Canonical raw-DLL509 comparison, source oracle, sanitizer repeat and
mutation control pass. The frozen integrated suite passes all 177 methods with no
skips in 209.555 seconds. Results and bound hashes are recorded in
`examples/lz4-pe/build/reverse-refinement-summary.json`.

Separate DAO-root checks built successfully with CMake. Its 233 Python tests
retain nine host-environment errors (four denied `ps`, four missing-Unicorn
imports and one failed original-runner subprocess),
with 29 skips. Both synthetic demos retain 59 reference-runner errors each from
Unicorn SIGILL; they are not decompiler validation passes.
