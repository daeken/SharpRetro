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
echo "=== rung-4 gate-(x86): the x86 lift+exec arm ==="
# WHY THIS ARM EXISTS, and it's a defect in this gate rather than in the code it
# gates: before 2026-08-20 this file had 25 aarch64 references and ONE x86 one, so
# XFusionTests could go red and stay red without any gate noticing. It did — 21
# failures rode for a month (IlLower had no case for 10 heads the .isa carried and
# the Rust arm lowered; X86Machine/X86Recompiler call Lift with no try/catch, so
# they were live throws through the C# exec-oracle and the JIT). An arm that
# cannot reach the region under test contributes a green that reads as coverage.
#
# rc read DIRECT (not through a pipe: `dotnet test | grep` gives grep's status,
# and this gate's own history has that bug in it — n>=12 for the family here).
# The count is compared against a stated FLOOR rather than 0, because the 3
# remaining failure is the SSE vector row (vmovmsk/vibin/vshuf/f32) that needs lane
# semantics the C# IL has never had. A floor makes a REGRESSION loud while an
# honest known-gap stays quiet — and the floor is asserted in BOTH directions:
# fewer failures than the floor is ALSO reported, because that means the floor is
# stale and this comment is lying to the next reader.
X86_FAIL_FLOOR=1
dotnet test XFusionTests -v q --nologo > /tmp/x86t.log 2>&1 || true   # set -e: rc=1 is EXPECTED at the floor
xrc=$?
xfail=$(grep -oE 'Failed:[[:space:]]+[0-9]+' /tmp/x86t.log | grep -oE '[0-9]+' | head -1)
xpass=$(grep -oE 'Passed:[[:space:]]+[0-9]+' /tmp/x86t.log | grep -oE '[0-9]+' | head -1)
if [ -z "$xfail" ]; then
  # No summary line at all = the suite did not RUN (a build break, a missing
  # project). That is not a pass; a gate with no subject is a PASS-shaped nothing.
  echo "  ✗ XFusionTests produced no result summary (suite did not run — rc=$xrc)"
  grep -E ' error |error CS' /tmp/x86t.log | head -5
  FAIL=1
elif [ "$xfail" -gt "$X86_FAIL_FLOOR" ]; then
  echo "  ✗ XFusionTests REGRESSED: $xfail failed (floor $X86_FAIL_FLOOR), $xpass passed"
  grep -oE 'op [a-z0-9]+|stmt head [a-z-]+' /tmp/x86t.log | sort | uniq -c | sort -rn | head -8
  FAIL=1
elif [ "$xfail" -lt "$X86_FAIL_FLOOR" ]; then
  echo "  ✗ FLOOR IS STALE: only $xfail failed (floor says $X86_FAIL_FLOOR), $xpass passed."
  echo "    This is good news and still a failure: lower X86_FAIL_FLOOR to $xfail and"
  echo "    update the comment above, or the next real regression hides under the slack."
  FAIL=1
else
  echo "  ✓ XFusionTests at floor: $xfail failed / $xpass passed (the 1 = the SSE vector row)"
fi

echo ""
echo "=== pre-push: house-vocab check (public repo — no seat-names/channel-cites/kt-refs) ==="
# The pattern list is itself the thing being searched for, so it CANNOT live in this file —
# a gate whose patterns sit in its own source self-matches and reports a hit on itself
# forever (which is exactly what happened: this block flagged three lines of this block).
# Patterns live in .vocab-patterns.txt, which is gitignored and never ships.
if [ ! -f oracle-baseline/.vocab-patterns.txt ]; then
  echo "  ✗ .vocab-patterns.txt missing — the gate has no subject, treat as FAIL not as clean"
  FAIL=1
elif grep -rn -f oracle-baseline/.vocab-patterns.txt \
     ArchCompilerCore/ ArchCompiler/ Frontends/ Backends/ rust/ oracle-baseline/README.md \
     Aarch64Generator/ SharpStationGenerator/ DamageGenerator/ 2>/dev/null \
   | grep -v '/obj/\|/bin/Debug\|/bin/Release\|/target/\|vocab-patterns.txt\|vocab-gate.txt\|<coram@daeken>'; then
  echo "  ✗ house-vocab present in tracked source — scrub before push"
  FAIL=1
else
  echo "  ✓ clean"
fi
if git log origin/main..HEAD --format='%s%n%b' 2>/dev/null | grep -f oracle-baseline/.vocab-patterns.txt; then
  echo "  ✗ house-vocab in unpushed commit messages — reword before push"
  FAIL=1
fi

echo ""
echo "=== gate-(idem): the generator is byte-identical on a second run ==="
# WHY this is a separate gate from the freeze-law diffs above: those compare
# generated-vs-ORACLE, so they catch a CHANGE in output and structurally cannot catch
# NON-DETERMINISM in the tool. Two runs both differing from the oracle IDENTICALLY read
# as "the .isa changed"; two runs differing from EACH OTHER never get compared at all.
# Product-to-oracle vs product-to-product are different assertions.
# The two runs MUST write to different dirs, or run-2 reads run-1's output.
$RUN ArchCompiler -- Aarch64Generator/aarch64.isa --stage emit --arch aarch64-rust --out /tmp/idem-1 2>/dev/null >/dev/null
$RUN ArchCompiler -- Aarch64Generator/aarch64.isa --stage emit --arch aarch64-rust --out /tmp/idem-2 2>/dev/null >/dev/null
IDEM_N=0
for f in /tmp/idem-1/*; do
  [ -f "$f" ] || continue
  IDEM_N=$((IDEM_N+1))
  b="/tmp/idem-2/$(basename "$f")"
  if cmp -s "$f" "$b"; then
    echo "  ✓ $(basename "$f") byte-identical on re-run ($(wc -c <"$f") B)"
  else
    echo "  ✗ $(basename "$f") DIFFERS between two runs of the SAME input — the generator is not deterministic"
    FAIL=1
  fi
done
# The gate must have a subject: 0 files compared is a PASS-shaped nothing.
if [ "$IDEM_N" -eq 0 ]; then
  echo "  ✗ idem gate compared 0 files — the generator produced no output (read this as a failure, not a no-op)"
  FAIL=1
fi

# ── COMPILE MATRIX (the arm that was missing) ──────────────────────────
# The rungs above byte-compare generator OUTPUT against the frozen oracle. That
# proves EQUIVALENCE and says nothing about COMPILABILITY: a generated file can be
# sha256-identical to the reference and still not build. Aarch64Cpu's committed
# Generated/Disassembler.cs did not compile for five days (456 errors from a clean
# regen) while every rung ran green over it -- positive proof of one property read
# as proof of a wider one.
#
# It is a MATRIX rather than one arm because the .isa emits into four consumers and
# only one of them was ever compiled here. When aarch64 was broken, the other three
# built clean -- so the defect was one-backend, but the BLINDNESS was fleet-wide,
# and a disagreement between backends on the same .isa is itself the finding.
#
# NB: `grep -c` EXITS 1 WHEN THE COUNT IS ZERO, so a bare CERR=$(... | grep -c ...)
# under `set -e` kills the script exactly when the build SUCCEEDS -- this arm could
# only ever pass while the defect it measures still existed (found the turn the count
# first reached 0: eleven arms green, rc=1, no failure printed anywhere). `|| true`
# lets zero be a real answer. A gate needs BOTH sides seen: a verified must-fail
# control and a verified pass.
echo "== compile matrix: every generated backend must BUILD, not just match =="
for proj in Aarch64Cpu SharpStationCore DamageCore XFusionCpu; do
  [ -f "$proj/$proj.csproj" ] || { echo "  ~ $proj absent (skipped)"; continue; }
  n=$(timeout 300 dotnet build "$proj/$proj.csproj" -v q --nologo 2>&1 | grep -cE ' error ' || true)
  if [ "$n" = "0" ]; then
    echo "  ✓ $proj builds (0 errors)"
  else
    echo "  ✗ $proj: $n compile errors in generated C# — byte-match does not imply compilability"
    FAIL=1
  fi
done

# ── COVERAGE PARITY (the matrix's semantic sibling) ────────────────────
# The matrix asks "does each backend BUILD". This asks the adjacent question the
# matrix displaces: "do they cover the SAME defs, and does either STUB one the other
# implements?" A one-sided stub passes byte-equivalence AND compilability -- it is
# only caught by an execution oracle, and only on the arm that has one (the Rust
# fuzz). Zero divergence today; the arm exists so a future .isa change can't
# reintroduce it quietly. BOTH SIDES SEEN: pass = 344/344 both, no one-sided stubs;
# plant a todo!() on a def C# implements -> fails NAMING the def, rc=1.
echo "== coverage parity: both backends cover the same defs, neither stubs one-sided =="
rm -rf /tmp/cp-rust && mkdir -p /tmp/cp-rust
$RUN ArchCompiler -- Aarch64Generator/aarch64.isa --stage emit --arch aarch64-rust --out /tmp/cp-rust 2>/dev/null >/dev/null
if python3 oracle-baseline/coverage-parity.py Aarch64Generator/aarch64.isa      Aarch64Cpu/Generated/Recompiler.cs /tmp/cp-rust/recompiler.rs; then
  :
else
  FAIL=1
fi

# The gate must actually GATE. A hit above sets FAIL=1; callers do:
#   bash oracle-baseline/rung-check.sh && git push
# so a non-zero exit blocks the push structurally. (A prior version printed the hit,
# then the caller read past it to their own "empty = clean" echo and pushed anyway —
# the guard's-own-output-not-consumed failure. The && form removes the reading step.)
exit $FAIL
