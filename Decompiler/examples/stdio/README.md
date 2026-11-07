# Locally opened streams: x86-64 to ARM64

This separate synthetic CLI exercises two recovered helpers that each own one
locally opened stream. One writes a binary pattern; the other reads a file and
returns its byte checksum modulo 128. Failure to open and close remains part of
the recovered control flow. The original source is compiled normally for x86-64;
only the resulting binary is supplied to the decompiler.

```sh
DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1 dotnet build Decompiler/Decompiler.csproj -c Release
DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1 python3 Decompiler/examples/stdio/roundtrip.py
```

On Apple Silicon macOS, the runner executes the x86-64 original through Rosetta
and compiles the generated C++ separately for native ARM64, ASan/UBSan and checked
memory. All 32 cases compare exit status, stdout, stderr and every case-local file.
Independent Python oracles verify output bytes, checksums and failure statuses.
Cases include empty, binary and 64 KiB inputs, deterministic random inputs,
missing input/output paths and overwriting an existing output file. A mutation
of the generated binary pattern must produce a file mismatch without crashing.
An isolated copy of the recovered executable runs without the original source,
binary or .NET runtime.

The checkpoint has three recovered functions, 29 basic blocks, two loops and
nine conditional regions. Six written register outputs are proven identities,
12 private dead effects disappear, and seven stdio callsites have established
provenance/read-footprint facts (including two `fopen` factories). Both recovered
helpers take only a path parameter and return a scalar; their original saved
registers no longer appear as artificial inputs or result structs. The checked
backend retains its conservative contracts as a separate comparison target.

The proof tracks nullable `fopen` results through exact full-width copies and
spills. It does not infer that opening succeeds, compare handle identities or
trust arbitrary incoming `FILE*` values. Unknown writes, unproved calls and close
invalidate stream provenance, including cached aliases. Bounds on completed
`fread`/`fwrite` elements describe a range, never an exact return count.

`build/report.json` records source/binary/generated-source hashes, compiler flags,
host information, per-case results and mutation control. These are bounded
observations of this synthetic program. The unchanged upstream
[b64 fixture](../b64/README.md) remains a separate regression corpus; its incoming
streams still need interprocedural provenance before equivalent contraction.

The complete local-stream checkpoint passes all 116 decompiler tests, including
31 focused frame/stream analysis tests. The independent reviewer found no
remaining code concern within this scope.
