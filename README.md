# SharpRetro

SharpRetro provides ISA-driven instruction lifting and execution infrastructure.
The new [Decompiler](Decompiler/README.md) project builds on the existing shared
`LiftIl` and XFusion lifter to recover compilable C++ from executable images,
with structured conditionals/loops and a dispatch baseline for comparison.
Application output additionally recovers block-wide SSA values and native storage,
with the machine-state representation retained as a diagnostic baseline.
It now emits separate C++ functions with explicit inputs/results, direct calls,
tail calls and recursion support, with reconstructed conditionals, loops and guard
clauses inside each function.
Native memory recovery replaces the emulated stack/address layer with automatic
frame objects, typed fields, byte buffers and ordinary C++ accesses; the checked
memory form remains available as a baseline.
Recovered source executes without instruction counters and passes through a C++
statement-tree cleanup for redundant scopes, empty branches and guard clauses.
Native functions use their recovered parameters directly and return results with
ordinary C++ calls, without a call-context object or runtime call bookkeeping.

The decompiler's goal is whole or selected-part binary recovery with matching
observable behavior, followed by progressively clearer source. Binary matching
is not required. Development and compiler round-trip checks run locally on macOS;
Windows is a later platform-integration target, not a development prerequisite.

```sh
dotnet build Decompiler/Decompiler.csproj -c Release
dotnet run --project Decompiler -c Release --no-build -- --help
python3 -m unittest discover -s Decompiler/tests -v
```

This is the first bounded translation slice, not a whole-program decompiler yet.
See the [current scope, execution model, checks, and next milestones](Decompiler/README.md).
Existing emulation and JIT projects remain in `SharpRetro.sln`.

The [CLI round-trip demonstration](Decompiler/examples/cli/README.md) compiles a
real x86-64 macOS command-line app, decompiles its executable, compiles the generated
C++ for ARM64, and compares both programs' stdout, stderr and exit status.
The [off-the-shelf b64 regression](Decompiler/examples/b64/README.md) repeats the
process with pinned, unmodified MIT-licensed sources and compares 292 cases,
including file contents, stdio/heap behavior, Base64 variants and error paths.
