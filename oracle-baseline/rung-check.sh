#!/bin/bash
# Rung-graded acceptance vs the frozen legacy compiler. Run from repo root.
set -e
cd "$(dirname "$0")/.."
FAIL=0

echo "=== rung-1a: parse+macro (aarch64/mips/dmg) ==="
for isa in Aarch64Generator/aarch64.isa SharpStationGenerator/mips-r3051.isa DamageGenerator/sm83.isa; do
  # (legacy-side dump instrument would need to exist per-isa; for now, use the checked-in sha256s)
  h=$(dotnet run --project ArchCompiler -- $isa 2>/dev/null | sha256sum | cut -d' ' -f1)
  echo "  $isa → $h"
done
echo "  aarch64 expected: a5739063032c2d21e0f2c2c15f1f1a7bfd2d4a3f79d9637acb054674573e9e33"

echo ""
echo "=== rung-1b: typed trees (aarch64) ==="
dotnet run --project oracle-baseline/instruments/LegacyR1b -- $(pwd)/Aarch64Generator/aarch64.isa 2>/dev/null | sed 's/CoreArchCompiler\.//g' > /tmp/c.typed
dotnet run --project ArchCompiler -- Aarch64Generator/aarch64.isa --stage typed 2>/dev/null | sed 's/ArchCompilerCore\.//g' > /tmp/n.typed
if diff -q /tmp/c.typed /tmp/n.typed > /dev/null; then
  echo "  ✓ byte-identical ($(grep -c '^# ' /tmp/n.typed) defs)"
else
  echo "  ✗ DIFFERS"; diff /tmp/c.typed /tmp/n.typed | head -20; FAIL=1
fi
exit $FAIL
