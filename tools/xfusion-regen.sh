#!/bin/bash
# Regenerate the C# XFusion decoder + lift tables (XFusionCpu/Generated/) from the .isa.
# The feature list is the one the committed Generated/ files were produced with; pass
# different features as arguments to override.
set -e
cd "$(dirname "$0")/.."
FEATURES=("$@")
[ ${#FEATURES[@]} -eq 0 ] && FEATURES=(ia32 x86-64 x87 sse sse2 sse3 ssse3 sse4.1 sse4.2 avx avx2 avx512)
dotnet build XFusionGenerator -c Release -v q -nologo | grep -E "error|rror\(s\)" || true
(cd XFusionGenerator && dotnet bin/Release/net10.0/XFusionGenerator.dll "${FEATURES[@]}")
