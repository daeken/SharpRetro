#!/bin/bash
# Rung-graded acceptance vs the frozen legacy compiler. Run from repo root.
set -e
cd "$(dirname "$0")/.."
FAIL=0

# Build ONCE up front so `dotnet run --no-build` output is pure program-output
# (a cold `dotnet run` emits build warnings to stdout → contaminates the sha256).
echo "=== build ==="
dotnet build ArchCompiler/ArchCompiler.csproj -v q 2>&1 | tail -3
dotnet build oracle-baseline/instruments/LegacyR1b/LegacyR1b.csproj -v q 2>&1 | tail -3
RUN="dotnet run --no-build --project"

echo "=== rung-1a: parse+macro (aarch64/mips/dmg) ==="
for isa in Aarch64Generator/aarch64.isa SharpStationGenerator/mips-r3051.isa DamageGenerator/sm83.isa; do
  # (legacy-side dump instrument would need to exist per-isa; for now, use the checked-in sha256s)
  h=$($RUN ArchCompiler -- $isa 2>/dev/null | sha256sum | cut -d' ' -f1)
  echo "  $isa → $h"
done
echo "  aarch64 expected: a5739063032c2d21e0f2c2c15f1f1a7bfd2d4a3f79d9637acb054674573e9e33"

echo ""
echo "=== rung-1b: typed trees (aarch64) ==="
$RUN oracle-baseline/instruments/LegacyR1b -- $(pwd)/Aarch64Generator/aarch64.isa 2>/dev/null | sed 's/CoreArchCompiler\.//g' > /tmp/c.typed
$RUN ArchCompiler -- Aarch64Generator/aarch64.isa --stage typed 2>/dev/null | sed 's/ArchCompilerCore\.//g' > /tmp/n.typed
if diff -q /tmp/c.typed /tmp/n.typed > /dev/null; then
  echo "  ✓ byte-identical ($(grep -c '^# ' /tmp/n.typed) defs)"
else
  echo "  ✗ DIFFERS"; diff /tmp/c.typed /tmp/n.typed | head -20; FAIL=1
fi
echo ""
echo "=== rung-2: emit aarch64 (Disassembler.cs + Recompiler.cs) ==="
$RUN ArchCompiler -- Aarch64Generator/aarch64.isa --stage emit --arch aarch64 --out /tmp/ac-out 2>/dev/null >/dev/null
for f in Disassembler.cs Recompiler.cs; do
  if diff -q oracle-baseline/aarch64/$f /tmp/ac-out/$f >/dev/null 2>&1; then
    echo "  ✓ aarch64 $f byte-identical"
  else
    echo "  ✗ aarch64 $f DIFFERS"; diff oracle-baseline/aarch64/$f /tmp/ac-out/$f | head -10; FAIL=1
  fi
done

echo ""
echo "=== rung-3b: emit mips (Disassembler.cs + Interpreter.cs + Recompiler.cs) ==="
$RUN ArchCompiler -- SharpStationGenerator/mips-r3051.isa --stage emit --arch mips --out /tmp/ac-mips 2>/dev/null >/dev/null
for f in Disassembler.cs Interpreter.cs Recompiler.cs; do
  if diff -q oracle-baseline/mips/$f /tmp/ac-mips/$f >/dev/null 2>&1; then
    echo "  ✓ mips $f byte-identical"
  else
    echo "  ✗ mips $f DIFFERS"; diff oracle-baseline/mips/$f /tmp/ac-mips/$f | head -10; FAIL=1
  fi
done

echo ""
echo "=== rung-3: emit dmg (Disassembler.cs + Interpreter.cs) ==="
$RUN ArchCompiler -- DamageGenerator/sm83.isa --stage emit --arch dmg --out /tmp/ac-dmg 2>/dev/null >/dev/null
for f in Disassembler.cs Interpreter.cs; do
  if diff -q oracle-baseline/dmg/$f /tmp/ac-dmg/$f >/dev/null 2>&1; then
    echo "  ✓ dmg $f byte-identical"
  else
    echo "  ✗ dmg $f DIFFERS"; diff oracle-baseline/dmg/$f /tmp/ac-dmg/$f | head -10; FAIL=1
  fi
done

echo ""
echo "=== rung-4 gate-(a): recompiler.rs compiles against Builder trait ==="
$RUN ArchCompiler -- Aarch64Generator/aarch64.isa --stage emit --arch aarch64-rust --out /tmp/ac-rust 2>/dev/null >/dev/null
mkdir -p rust/aarch64-recomp/src
cp /tmp/ac-rust/recompiler.rs rust/aarch64-recomp/src/lib.rs
if [ ! -f rust/aarch64-recomp/Cargo.toml ]; then
  printf '[package]\nname = "aarch64-recomp"\nversion = "0.1.0"\nedition = "2024"\n[dependencies]\nsharpretro-jit = { path = "../sharpretro-jit" }\n' > rust/aarch64-recomp/Cargo.toml
fi
# NB: verify via direct exit-code (a timeout+grep-count reads a truncated stream as 0).
(cd rust/aarch64-recomp && cargo check 2>/tmp/r4.err)
r4=$?
if [ $r4 -eq 0 ]; then
  echo "  ✓ recompiler.rs cargo check clean ($(wc -l < /tmp/ac-rust/recompiler.rs) lines)"
else
  echo "  ✗ cargo check FAILED (exit $r4):"
  grep -E "^error" /tmp/r4.err | sort | uniq -c | sort -rn | head -5
  FAIL=1
fi

echo ""
echo "=== pre-push: house-vocab check (public repo — no seat-names/channel-cites/kt-refs) ==="
if grep -rn 'barrow\|fuchi\|coram\|kt\[\|own #\|·[0-9]\|#alky\|corpse' \
     ArchCompilerCore/ ArchCompiler/ Frontends/ Backends/ rust/ oracle-baseline/README.md 2>/dev/null \
   | grep -v '/obj/\|/bin/\|/target/'; then
  echo "  ✗ house-vocab present in tracked source — scrub before push"
  FAIL=1
else
  echo "  ✓ clean"
fi
if git log origin/main..HEAD --format='%s%n%b' 2>/dev/null | grep -E '·[0-9]+|barrow|fuchi|kt\[|own #|#alky|corpse'; then
  echo "  ✗ house-vocab in unpushed commit messages — reword before push"
  FAIL=1
fi

# The gate must actually GATE. A hit above sets FAIL=1; callers do:
#   bash oracle-baseline/rung-check.sh && git push
# so a non-zero exit blocks the push structurally. (A prior version printed the hit,
# then the caller read past it to their own "empty = clean" echo and pushed anyway —
# the guard's-own-output-not-consumed failure. The && form removes the reading step.)
exit $FAIL
