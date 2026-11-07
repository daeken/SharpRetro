# x86-64 CLI to native ARM64

This is an executable round-trip on Apple Silicon macOS:

1. Clang compiles `original.cpp` into an ordinary x86-64 Mach-O executable.
2. `decompile-app` reads that binary, traverses code from `LC_MAIN` through direct
   calls and branches, and emits `recovered.cpp`.
3. Clang compiles **only that generated translation unit** into an ARM64 executable.
4. The runner compares stdout, stderr and exit status against the original x86-64
   executable through Rosetta. The ARM64 executable runs directly on the host.

Application parsing, string comparison, number formatting, hashing, numeric
aggregation and sequence generation are all recovered from the binary. The host
adapter provides native storage, `argc`/`argv` and the two libSystem I/O calls; it does
not implement the application's algorithms. No x86 machine-code interpreter,
original source, original binary, .NET runtime or Windows VM is used by the
recovered executable at runtime.

## Reproduce

From the SharpRetro checkout, with .NET 10, Python 3, Clang and Rosetta installed:

```sh
dotnet build Decompiler/Decompiler.csproj -c Release
python3 Decompiler/examples/cli/roundtrip.py

Decompiler/examples/cli/build/recovered-arm64 stats 17 5 99
# count=3 sum=121 min=5 max=99

Decompiler/examples/cli/build/recovered-arm64 hash hello
# 11831194018420276491

printf 'hello' | Decompiler/examples/cli/build/recovered-arm64 hash
# 11831194018420276491

Decompiler/examples/cli/build/recovered-arm64 seq 1 3
# 1082269761
# 1152992998833853505
# 11177516664432764457
```

To perform the stages individually after building the decompiler:

```sh
# Use the exact declared subset flags from roundtrip.py for the reference build.
python3 -c 'import sys; sys.path.insert(0,"Decompiler/examples/cli"); import roundtrip; roundtrip.build("Decompiler/examples/cli/build")'

dotnet Decompiler/bin/Release/net10.0/Decompiler.dll inspect \
  Decompiler/examples/cli/build/original-x86_64
dotnet Decompiler/bin/Release/net10.0/Decompiler.dll decompile-app \
  Decompiler/examples/cli/build/original-x86_64 \
  --output Decompiler/examples/cli/build/recovered.cpp
clang++ -arch arm64 -std=c++20 -O2 \
  Decompiler/examples/cli/build/recovered.cpp \
  -o Decompiler/examples/cli/build/recovered-arm64
```

`roundtrip.py` records all original compiler flags, input/source/generated-source/
output hashes, host/compiler versions, architectures, library dependencies and
per-case observation digests in `build/report.json`. A mismatch preserves its
complete replayable input and observations in `build/failure.json`.

The runner also emits `build/machine.cpp`/`build/machine-arm64` using the original
machine-state backend and `build/blocks.cpp`/`build/blocks-arm64` using the earlier
flat value backend. Every CLI case compares the x86-64 reference, recovered native
functions, their ASan/UBSan build, flat blocks and the machine baseline.

`decompile-app` defaults to `--representation recovered`. Use
`--representation machine` to select the diagnostic backend or
`--representation blocks` for the flat recovered-value form.

## Observed checkpoint

On macOS ARM64 with .NET SDK 10.0.101 and Apple Clang 21.0.0:

* The original is a thin `x86_64` Mach-O executable; the generated executable and
  its ASan/UBSan build are thin `arm64` Mach-O executables.
* 351 reachable x86 instructions are recovered into eight C++ functions and 103
  basic blocks. Register definitions become SSA values within each block, with
  live locals between blocks and explicit parameters/results between functions.
  All eight function bodies now have structured control flow: 10 loops and 46
  conditional regions, without block gotos. Shared exit paths use 19 bounded
  copies of original blocks; every instance's edges are checked.
* Dead flags, instruction temporaries, register copies and per-instruction
  dispatch disappear. The preceding flat source checkpoint was 69,393 bytes versus
  384,358 bytes for the machine backend. Function definitions add explicit
  contracts and return checks; current sizes for all three forms are recorded
  in `report.json` rather than treated as a monotonic size target.
* All 155 deterministic CLI cases agree in stdout, stderr and exit status on both
  normal and ASan/UBSan ARM64 builds, flat blocks and the machine baseline. Cases include text/stdin hashing, embedded
  zero bytes, Unicode, multi-read input, 64-bit limits, wraparound sums, invalid
  arguments, sequence bounds and a closed-stdin read error.
* Changing translated XOR arithmetic or reversing a structured branch causes
  the same CLI comparison to fail.
* A copied ARM64 executable works from an isolated directory. Its dependencies
  are only `/usr/lib/libc++.1.dylib` and `/usr/lib/libSystem.B.dylib`.
* Separate tests strip local function/debug symbols from the input, recover it
  again and compare text hashing, stdin hashing and sequence output. Traversal
  does not depend on local function names or the original C++ source.

The frame-proof checkpoint emits 863 lines of recovered C++. Six written
register outputs are independently proven to equal their incoming values;
contract recovery omits those artificial results. Nine private dead effects are
removed before input inference. The source still contains conservative carrier
parameters where aliases or unknown call observations block elimination. See
[the analysis contract](../../README.md#frame-identities-and-private-memory)
for the distinction between register identity, private memory and byte liveness.

These are bounded observations of this app and execution configuration, not a
universal equivalence result or evidence that an arbitrary macOS application works.

## Adapter scope

The fixture deliberately uses an integer-only compiler profile: no builtins,
automatic vectorization, SSE, stack protector, C++ exceptions or RTTI. This profile
is recorded in `roundtrip.py`; additional ISA/runtime coverage remains separate work.

The application command uses the preferred `LC_MAIN` address and SysV x86-64
integer/pointer calling convention. Recovered source borrows the native argument
vector and strings, uses extracted image arrays and per-function native frame
objects with typed fields and byte buffers. Direct field assignments, byte
pointers and inline `memcpy` preserve ordinary aliasing and unaligned accesses.
The native profile assumes valid object bounds/lifetimes; the preceding checked
address model remains available with `--memory checked`. Direct CALL/RET
semantics come from LiftIl, using native C++ calls/returns and reconstructed
conditionals, loops, guards, break and continue within each function. Direct call targets and reachable Mach-O function-
start metadata establish entries; transfers to another entry become tail calls.
Indirect calls/jumps remain unsupported. The round-trip runner requires
`--control-flow structured`; normal `auto` may retain the complete block/goto
form for an unsupported function. `--control-flow dispatch` explicitly selects
that baseline while preserving native function calls. Loops with several exits
use a scoped exit selector and separate continuations after `break`.

The current source form retains ABI carrier locals at block boundaries and
saved-register values. Each function has inferred inputs and scalar, void or
multi-value results. Native signatures take only the recovered inputs and use
those parameters directly, without copying them into register-named locals.
There is no context argument or context object at calls or tail calls, and no
hidden replacement context or runtime depth/storage counters. The native form has no
stack pointer, synthetic return slots or address lookup layer. Original semantic
types and names remain unknown; 15 stack slots become typed fields in four frame
objects. Pointer
relocation assumes normal native address use on a 64-bit little-endian host; raw
numeric address dependencies require separate analysis. Variable instruction-
internal control and conditional memory effects outside this value profile refuse;
the machine representation remains available for those inputs.

The runner requires `--memory native` and also builds the preceding checked-memory
function output. Every one of the 155 cases must match that baseline as well.
`--memory auto` falls back to checked storage for unsupported frame layouts;
`native` refuses them before writing source. General pointer bounds/lifetime
inference and pointer-typed function signatures remain future work.

Mach-O import names come from the symbol/indirect-symbol tables. For x86-64 stubs,
the parser checks the actual `ff 25 disp32` jump bytes and their pointer-table
symbol binding. The I/O adapter requires an explicit `/usr/lib/libSystem.B.dylib`
binding for this fixture's `_read` and `_write` calls. Recovered functions also
have the explicit libc bridges exercised by the [upstream b64 fixture](../b64/README.md);
the machine/flat-block adapters retain their read/write-only scope. It models integer/
pointer arguments, byte transfers, return values and stack return. It does not
model library-internal volatile-register values, TLS errno or a complete dyld runtime.

Only literal-string and zero-filled image data are mapped by this adapter.
Initialized writable data, pointer rebasing, general relocations, startup
constructors and arbitrary globals are not supported. An unmapped data access or
unbound import is an explicit error. Import resolution assumes normal linker
metadata and loader binding; a full chained-fixup loader is not implemented.

Native frame-offset analysis is limited to 1 MiB per function; the checked stack
buffer and its accepted argument storage are 1 MiB each. Image objects are limited
to 16 MiB and host I/O transfers to 1 MiB per call. Recovered and flat-block source
have no instruction counters or execution limit. The diagnostic machine
representation retains its per-instruction budget API. Native calls and recursion
use ordinary host C++ stack behavior; checked-memory calls retain their depth cap
of 512 and validate recorded continuations. Checked depth/storage
exhaustion, ambiguous shared function ownership, an untranslated path or an unexpected
return cannot count as success. All generated files and binaries stay in ignored
`build/`; the source fixture and replay runner are versioned project source.

Structured bodies also pass through a C++ statement-tree cleanup. Redundant block
scopes and empty arms disappear, terminating arms become guards, and `else if`
chains replace unnecessary nesting. Declaration metadata keeps scopes that are
needed for temporary-name collisions and preserves shadowing during guard
hoisting. Native pointer/memory effects stay in their original order.
