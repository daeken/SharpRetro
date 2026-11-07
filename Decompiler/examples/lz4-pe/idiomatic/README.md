# Agent rewrite of recovered C++

This is a separate experiment, not a change to the decompiler's generated output.
An agent received the complete recovered C++ and the compilation interface, with
instructions not to consult upstream LZ4 code or algorithm descriptions. It
submitted one candidate. That candidate passed the existing comparison on its
first full run; no failed-case-driven source revisions were needed.

The frozen generated input is included as [recovered-input.cpp](recovered-input.cpp), SHA-256
`b40eba292c0446559fabb53397e7a2ebc05a3d1d782431154ab60b7028a76099`.
The candidate is [recovered-idiomatic.cpp](recovered-idiomatic.cpp), SHA-256
`097fd6bb5a335b8ca1943cab65b82ac6bd74e59617bfe5c36c6b3229af6dcd53`.
The input was originally `../build/lz4-decompress-safe.cpp`; its tracked copy
makes this experiment reproducible without a previous local build. The candidate's
runtime prelude, image data, declarations/import adapter, and final entry
adapter are byte-identical to the input. The emitter is unchanged.

| Measured source property | Generated input | Agent candidate |
| --- | ---: | ---: |
| Complete file lines | 717 | 467 |
| File bytes | 46,458 | 26,278 |
| Lines after recovered-body marker | 527 | 277 |
| Numbered shared-region lambdas | 5 | 0 |
| Distinct numbered temporary/carrier names in bodies | 172 | 0 |
| `if` statements in bodies | 61 | 32 |

Lines use Python `splitlines()`; the body suffix includes its leading empty line,
helpers, entry adapter, and namespace close. Smaller output is a presentation
observation, not the acceptance criterion.

The agent merged duplicated match continuations, named input/output cursors and
limits, and replaced generic reference interfaces with typed copy operations and
returned positions. Wide accesses, short-distance forward overlap, unsigned
address calculations, and the timing of partial/error writes remain explicit.
See [the agent's transformation notes](TRANSFORMATIONS.md) for reasoning and
deliberately retained details.

## Validation

Run from the SharpRetro checkout root on Apple Silicon macOS with Rosetta:

```sh
python3 Decompiler/examples/lz4-pe/idiomatic/validate.py
python3 Decompiler/examples/lz4-pe/idiomatic/stress.py
```

The wrapper calls the existing parent fixture's `build()` and unmodified
`verify()` function. It rejects changes to the frozen input, runtime prelude or
entry adapter, and checks fresh generation against the frozen input. The 509
existing cases compare repeated original Windows-DLL execution against native
ARM64 and ASan/UBSan candidate builds, including full output allocations and
canaries. The same source oracle and shift-mutation control pass unchanged.

An additional, separate deterministic corpus has 444 cases: new compressed
payloads with tight/roomy/truncated output capacities, mutations and truncations
of earlier inputs, and independently random malformed input bytes. Every case
matches repeated original-DLL execution, ARM64 and ASan/UBSan. Known plaintexts
also check the successful decoded result. The supplemental run neither changes
the frozen corpus nor discards failing cases. Its report binds exact runner
artifacts and records all input bytes for replay.

The candidate also compiles with Apple Clang 21 using
`-arch arm64 -std=c++20 -O2 -Wall -Wextra -Werror`, with no warning suppressions.
Current reports and source/binary hashes are in `../build/idiomatic/report.json`,
`stress-report.json`, and `experiment-summary.json`.

These are 953 observed export cases and a manually reviewed source rewrite,
not a universal equivalence proof. The existing caller-memory, runtime and
native pointer-view profile still applies; arbitrary initial states and
whole-DLL/process behavior remain outside the experiment.

The separate DAO harness CMake build passes. Its 233-test run retains nine
environment errors and 29 skips (denied `ps`, unavailable Unicorn imports and
one failed original-runner subprocess); its two synthetic demos each retain
59 Unicorn reference-runner failures. Those results are recorded separately
and do not contribute to this export's passing evidence.

## What this suggests

The largest simplification was recognizing a common continuation across
specialized paths, rather than giving arbitrary repeated fragments cleaner
names. Typed operation contracts and recovered roles then made helper extraction
readable. This supports experimenting with an agent cleanup stage after reliable
machine-level emission, while keeping the validated input available for review.
It does not yet establish that the same improvements will generalize to unrelated
programs or that the inferred operation names match the original source.
