# Unmodified upstream b64 round trip

This fixture vendors [iaseth/b64](https://github.com/iaseth/b64), an MIT-licensed
Base64 CLI, at commit `20b0bf244e785443ae27bea06dc6f67b4639d880`. The five vendored
files are byte-for-byte upstream copies, including the license. `upstream.json`
records their Git blob IDs; the runner verifies those IDs before compiling.

```sh
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet build Decompiler/Decompiler.csproj -c Release
python3 Decompiler/examples/b64/roundtrip.py
```

The runner compiles the original C files into an x86-64 Mach-O executable, reads
that executable with the decompiler, and compiles only the generated C++ into
native ARM64 and ASan/UBSan binaries. A second generated build uses the checked
memory model. The original x64 executable runs through Rosetta. No original
algorithm source, object or executable is linked into the recovered programs.
Host libc implements the named imported library APIs; Base64 encoding/decoding
itself remains entirely binary-derived C++.

The application build uses the existing scalar integer profile (no vectorization,
SSE, builtins or stack protector), plus `-Wl,-no_fixup_chains` to select classic
Mach-O rebase/bind streams. Compile-date macros are pinned for reproducible version
output. The flags and all input/source hashes are recorded in `build/report.json`.
This is one explicit compiler/loader profile, not coverage of arbitrary binaries.

## Comparisons

All 292 deterministic cases compare stdout, stderr, exit status and complete file
contents across x64, native ARM64, ASan/UBSan and checked-memory builds. Cases cover:

* Standard and URL-safe encoding, decoding and 80-column wrapping.
* Empty inputs, byte/padding boundaries, all byte values, Unicode, random bytes,
  and inputs through 64 KiB.
* stdin, multiple input files, output files, help/list/version and option errors.
* Whitespace, malformed characters, partial blocks, invalid padding, missing files,
  invalid output paths, and closed stdin.

Valid outputs are also checked against Python's Base64 implementation, including
output files. A deliberate left-shift/right-shift mutation in recovered function
bodies must fail comparison. The native binary also runs from an isolated
directory with only its executable and test inputs.

The upstream tool has an error-status quirk: decoding bad stdin may print an error
but still exit zero, while file decoding propagates the error. Its ignore-garbage
flag is also parsed but not implemented. Comparisons retain these upstream
behaviors; they do not quietly repair the source.

## Gaps exposed and implemented

* Per-import integer/pointer contracts for `strcmp`, `malloc`, `free`, `fopen`,
  `fclose`, `fread`, `fwrite`, `fgetc`, `perror`, `printf` and `fprintf`, alongside
  the earlier `read`/`write` bridges. Four-argument stdio calls receive RCX inputs;
  void imports have no invented return value.
* Classic Mach-O rebase/bind streams, initialized data and constant tables,
  relocated image pointers, and libSystem standard-stream data bindings. Import
  stubs must agree with both indirect-symbol and dyld binding metadata.
* Typed `%s`, `%c` and `%%` formatting. Untyped x64 varargs are decoded rather
  than passed through ARM64's different variadic ABI. Other conversions, vector
  arguments and stack-passed variadic arguments remain unsupported.
* Conditional x86 division faults, including divide-by-zero and quotient overflow.
  Guards retain their evaluation position even if the division result is dead.
  They raise an explicit execution error; host signal delivery is not emulated.

Weak/special-ordinal bindings, nonzero binding addends, threaded binding opcodes,
mixed classic/chained streams and unsupported rebase targets refuse. The legacy
machine/flat-block app backends retain their narrower import/data profiles.

On the current Clang build, 542 reachable instructions become 158 basic blocks
and four structured functions. The generated C++ still has conservative ABI
carriers and shared-tail duplication; this fixture is now a regression target
for improving both. It has native frame objects, no call context, and no block
gotos. These comparisons are bounded observations, not a general equivalence proof.

## Independent review iterations

A separate reviewer examined immutable generated C++ snapshots, supplied prioritized
feedback, and reviewed the regenerated output after each implementation round.

| Checkpoint | Lines | Bytes | `static_cast` | Call-result temporaries |
| --- | ---: | ---: | ---: | ---: |
| Before review | 2,317 | 174,113 | 859 | 78 |
| Typed-expression cleanup | 2,317 | 164,315 | 528 | 78 |
| Native dead-read cleanup | 2,199 | 154,987 | 528 | 78 |
| Boolean inversion and exact tails | 2,198 | 155,969 | 528 | 78 |
| Observed call results | 2,138 | 151,611 | 528 | 43 |
| Boolean carriers and loop tails | 2,130 | 151,155 | 528 | 43 |
| Inclusive comparisons | 2,118 | 149,944 | 528 | 43 |

The structured CFG still reports 203 duplicated block copies and maximum source
indentation remains 88 spaces. These local rewrites do not solve region ownership.

The first round removes redundant exact-width arithmetic/boolean casts and casts
in typed native stores. Narrow unsigned promotion and truncation remain explicit.
Focused tests cover 32-bit wraparound, 64-bit wraparound and the high half of a
128-bit product.

The second round removes 118 unused reads of initialized native scalar fields.
Eligibility requires an exact known `(frame offset, width)` and no use beyond the
load's own evaluation effect. Stores, field storage, generic loads, checked-memory
reads and fault effects remain. Tests preserve pre-write snapshots, post-call
alias mutations, live return values and division-fault dependencies. Both rounds
retain all 292 comparisons and the changed-codec mutation control.

Subsequent rounds invert exact integer comparisons under one-bit NOT and remove
double complements. Strict comparison OR equality becomes an inclusive comparison
only when both predicates use identical operand IDs, order and widths. One-bit
bitwise expressions keep eager `&`, `|` and `^`; actual integer-to-bit truncations
retain their low-bit mask. Tests exercise all signed/unsigned byte pairs and
reject merging comparisons with different operands.

Common-tail extraction shares only identical nondeclaring statements with no
reference to either branch's recursively collected local names. The source tree
contains compiler-created trivial temporaries whose addresses cannot escape their
scopes; this pass is not applicable to arbitrary C++ with RAII or escaping locals.
Tests cover branch-local declarations, shadowing, shared side effects, an early
return bypassing the tail, and unchanged break/continue targets. A direct final
`continue` is removed only from its own loop body.

Calls assign only results observed by their continuation's liveness set. Calls
with no observed results remain bare calls, preserving memory effects. Checked
return-address and stack updates remain, and tail calls use the caller's result
contract. Tests include a mutating callee with an ignored first result and two
call sites observing different members of one result struct.

The initial six-round review stopped at a concrete proof boundary. At that
checkpoint, an incomplete saved-register identity prototype was removed without
enabling it: stream callbacks and escaped aliases can mutate saved frame slots,
so buffer bounds alone cannot justify shrinking those contracts. The subsequent
enabled analysis is described below. Larger b64 improvements still require:

- Stronger stream/callback provenance for these exposed buffers before removing
  their saved-register carriers; broader alias facts before promoting their
  memory accesses into independent locals.
- Stronger join/region ownership and declaration dominance before merging the
  remaining duplicated regions or substantially reducing nesting.
- Coherent range and type evidence before selecting pointer, array or signed
  source types.

The reviewer found no regression in the applied rounds. This was a stopping point
for the then-available proof facts, not a claim that the output is optimal or a universal
equivalence proof. All checkpoints retain the 292 executable comparisons and the
changed-codec mutation control.

Initial review validation: all 83 decompiler tests passed, including 16 focused review
rewrite tests, 13 source-tree oracle cases, this tool's 292 comparisons and the
original CLI fixture's 155 comparisons with ASan/UBSan builds.

The subsequent [frame-proof pass](../../README.md#frame-identities-and-private-memory)
now implements exact frame identities, sticky escape tracking and private byte
liveness. It intentionally proves zero saved-register identities for this
fixture: the `FILE*` helpers still lack callback/stream provenance. Nine dead
effects are removed before contract inference; their loads were already omitted
at emission in the preceding checkpoint, so the generated output remains 2,118
lines. The 292 comparisons and codec mutation control continue to pass.
The complete frame-proof checkpoint passes all 103 decompiler tests, including
20 new alias/identity/private-effect tests.

The following local-`fopen` checkpoint passes 116 decompiler tests and the new
[32-case stdio fixture](../stdio/README.md). It does not infer provenance for this
tool's incoming or standard streams; these helpers remain conservative and all
292 upstream comparisons still pass.
