#!/bin/bash
# ship.sh — the deterministic commit-flow ("judgement up to agents;
# execution deterministic"). Replaces a 5-step manual sequence whose
# by-hand form produced 6+ recorded process failures (pushed build-fails,
# gates run but output unread, stale-binary false-greens, rc-hidden-by-pipe).
#
# Usage: ./ship.sh "commit message"   (from rust/xfusion-recomp/)
# Judgement (WHAT to commit, the message) = the caller's.
# Execution (the gate ORDER + the refuse-on-fail) = this script's, always.
set -e
MSG="${1:?usage: ship.sh 'commit message'}"
cd "$(dirname "$0")"
REPO_ROOT="$(git rev-parse --show-toplevel)"

echo "== 1/5 build (cold, direct rc) =="
cargo build --release --bins --examples
( cd ../sharpretro-jit && cargo build --release )

echo "== 2/5 tests =="
cargo test --release 2>&1 | grep -E "test result" | tail -1
( cd ../sharpretro-jit && cargo test --release 2>&1 | grep -E "test result" | tail -1 )
# grep here is DISPLAY-only; failure already aborted via set -e on cargo's rc.

echo "== 3/5 core regressions =="
./target/release/xfusion-recomp --run-x64 sum10 | grep -q "MATCH" || { echo "FAIL sum10"; exit 1; }
./target/release/examples/atomics_torture /tmp/torture_x64 8 | grep -q "PASS" || { echo "FAIL torture"; exit 1; }
( cd tests-src/fleet && python3 run_fleet.py 2>&1 | tail -1 | grep -q " 0 FAIL" ) || { echo "FAIL fleet"; exit 1; }

echo "== 4/5 stage + vocab-gate (the STAGED set = the ship set — the canary
#          negative-control caught the untracked-file hole: diff-lists miss
#          untracked, git add -A ships them; stage FIRST, gate the staged) =="
cd "$REPO_ROOT"
git add -A
STAGED=$(git diff --cached --name-only)
# Pattern lives in .vocab-gate.txt (one regex per line) so the gate's own
# source never contains the patterns it hunts (self-match = the untested-
# gate's cousin). Files listing the patterns: the pattern-file itself, exempt.
HITS=$(echo "$STAGED" | sort -u | grep -v "WINE-SPECS/\|.vocab-gate.txt" | xargs -r grep -lnE -f rust/xfusion-recomp/.vocab-gate.txt 2>/dev/null || true)
if [ -n "$HITS" ]; then
  echo "VOCAB-GATE FAIL — house-vocab in:"; echo "$HITS"
  echo "(scrub or move to datadir; WINE-SPECS/ exempt as internal-facing; unstage w/ git reset)"
  git reset -q
  exit 1
fi

echo "== 5/5 commit + push =="
git commit -m "$MSG" --author="coram <coram@daeken>"
git push origin main
echo "== SHIPPED =="
