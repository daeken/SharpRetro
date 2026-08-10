#!/bin/bash
# Regenerate disassembler.rs + lift.rs from the .isa sources.
# These files are gitignored (rust/.gitignore) — they're derived artifacts;
# the .isa + RustLiftGen.cs/RustDisasmGen.cs are the tracked source.
# This script captures the FEATURE SET so a fresh clone regenerates the same
# decoder (was passed ad-hoc on the CLI before; the sse3+ tier was silently
# dependent on remembering to pass it).
set -e
cd "$(dirname "$0")/../.."
# Feature set for the Rust x64 recompiler. Governs which .isa insns are
# included in disassembler.rs (decode) + lift.rs (semantics). Everything
# here that's still an intrinsic-stub will DECODE + die-loud-NAMED at first
# contact (the walls-ladder), which is strictly better than DECODE-STOP.
#
# NOT included: x87 (0 in CP2077 corpus, MSVC-x64 doesn't emit), avx/avx2/
# avx512 (cpuid_table under-advertises; adding these would decode VEX/EVEX
# forms but CP2077-with-SSE-baseline-cpuid never reaches them).
FEATURES="ia32 x86-64 sse sse2 sse3 ssse3 sse4.1 sse4.2"
touch XFusionGenerator/Program.cs
dotnet run --project XFusionGenerator -- $FEATURES --rust rust/xfusion-recomp/src
echo "regen: $(grep -c '=> {' rust/xfusion-recomp/src/disassembler.rs) def-arms, $(grep -c '^fn tmpl_' rust/xfusion-recomp/src/lift.rs) templates"
