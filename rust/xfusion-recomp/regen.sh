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
# x87 IS included (562 arms). It was excluded originally on the grounds below,
# and that reasoning was corpus-specific and turned out to be wrong for a second
# guest: MSVC-x64 doesn't emit x87, but a guest that JITs its own code can —
# the QVM x86_64 JIT emits x87 and was the first customer. Retired claim, kept
# so the reversal is legible rather than looking like drift:
#   "NOT included: x87 (0 in CP2077 corpus, MSVC-x64 doesn't emit)"
# The lesson generalizes past x87: a feature-set justified by ONE corpus is a
# claim about that corpus, not about the ISA.
#
# NOT included: avx/avx2/avx512 (cpuid_table under-advertises; adding these
# would decode VEX/EVEX forms but a guest held to an SSE-baseline cpuid never
# reaches them). Same caveat applies — that's a claim about our cpuid_table.
FEATURES="ia32 x86-64 sse sse2 sse3 ssse3 sse4.1 sse4.2 x87"
touch XFusionGenerator/Program.cs
dotnet run --project XFusionGenerator -- $FEATURES --rust rust/xfusion-recomp/src
echo "regen: $(grep -c '=> {' rust/xfusion-recomp/src/disassembler.rs) def-arms, $(grep -c '^fn tmpl_' rust/xfusion-recomp/src/lift.rs) templates"
