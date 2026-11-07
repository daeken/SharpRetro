# Idiomatic recovered decoder experiment

The candidate was derived only from `build/lz4-decompress-safe.cpp`; `driver.cpp`
was read for its inclusion/interface contract. No upstream implementation or
algorithm description was consulted. The input file SHA-256 is
`b40eba292c0446559fabb53397e7a2ebc05a3d1d782431154ab60b7028a76099`.
Its embedded binary identity remains unchanged in the candidate prelude.
The same input is archived as `recovered-input.cpp` for fresh checkouts.

Line counts use `len(path.read_text().splitlines())`: the pinned input has 717
lines and the frozen candidate has 467. Counting raw LF bytes gives the same
717 → 467 result for these exact files. The candidate SHA-256 is
`097fd6bb5a335b8ca1943cab65b82ac6bd74e59617bfe5c36c6b3229af6dcd53`.
For the suffix obtained by splitting once on `// Recovered function bodies`,
`splitlines()` gives 527 → 277, including the initial empty line after the
marker text and the final entry adapter. These counts are presentation measures,
not counts of executable statements.

## Changes

- Replaced numbered carriers with input/output cursors, literal and match lengths,
  limits, distance, and source/destination names.
- Consolidated the two duplicated general match continuations. Both now reach the
  same seed, bounds checks, near-end tail, and long-copy code. The separate fast
  8+8+2-byte match path remains because its write footprint differs.
- Replaced generic reference-argument `shared_region_*` lambdas with typed helpers.
  The seed returns its next source pointer. The near-end prefix returns a
  `CopyPosition`, whose source and destination are both definitely assigned.
  No helper receives an uninitialized output carrier as an input value.
- Extracted the repeated eight-byte and sixteen-byte transfer patterns. Their
  loads remain separate snapshots before their corresponding stores. The four
  initial short-distance bytes became a forward loop with one load/store pair per
  iteration. This retains overlap-dependent reads from preceding stores.
- Collapsed identical literal-extension branches and used guard returns for final
  literals and failures. Pure boolean/narrow-carrier scaffolding became ordinary
  boolean conditions. The token high nibble remains `>> 4ULL` for the unchanged
  mutation control.
- Preserved the runtime prelude, image data, entry overload, import adapter, and
  final Win64 adapter byte-for-byte. The frame keeps its layout, value
  initialization, four original-width table transfers, and signed table retreat.

## Deliberate residual structure

`byte_offset` and numeric address comparisons remain. Pointer roles do not prove
array bounds, and ordinary pointer subtraction/addition would discard the
recovered unsigned modular arithmetic. Input size and output capacity still use
their low 32 bits with the original sign extension when computing end pointers;
this candidate adds no argument validity preconditions.

The wide-copy overlap guards and rounded transfer counts remain explicit. A copy
may issue one eight-byte transfer for zero literals, or write past the logical
literal/match end. Sixteen-byte grouping is selected only by the recovered tests.
The existing imported `memcpy` is retained only on the original final-literal
path; no recovered transfer was replaced with a library copy or fill.

Error timing is intentional: invalid back-references fail after the literal copy;
the four-byte zero store precedes match-length extension validation; extension
bytes are read before their recovered post-read limit check; the match seed is
written before the late match-end rejection. Literal extension stops at equality
with its recovered limit even when its last byte is 255. These choices preserve
partial/error writes and read ordering rather than imposing a cleaner format
contract.

Local copy cursors now advance unconditionally after the final transfer, where
the original updated them only when taking its loop backedge. Those final cursor
values were dead. Removing the conditional does not add another memory access.
The former literal-copy helper also mutated the outer output cursor; that value
was dead before the match end replaced it, so the helper now uses local cursors.

## Checks and remaining uncertainty

The candidate compiled against the existing driver with Apple Clang using
`-std=c++20 -O2 -Wall -Wextra -Werror`, with no warning suppressions. The hello
smoke input returned 5, wrote `68656c6c6f`, and preserved its canaries. A byte check
confirmed identical prelude and final adapter. The coordinator reports that all 509 unchanged published-DLL cases pass on
native ARM64 and ASan/UBSan builds, with the source oracle and original mutation
control also passing. These checks compare the complete output allocation,
including extra-copy and partial-error bytes. The coordinator owns the detailed
validation report and any supplemental stress results; the source is frozen
after this first passing candidate.

This is a manually reasoned transformation of recovered source, not an
independent reconstruction from machine instructions or a universal equivalence
proof. The unusual bounds and wrap tests are retained rather than interpreted as
a complete format specification. The existing native memory-view/runtime profile
continues to determine which pointer states can execute meaningfully.

## General lessons

Typed helper results and one common continuation can remove much of the visible
SSA/region machinery without changing data operations. Readable names are most
useful when they identify the changing cursor separately from its logical end.
Copy helpers need a precise operational contract: source/destination/length alone
can hide access width, overrun footprint, forward-overlap behavior, and validation
order. Those details can remain explicit in a small number of readable helpers.
