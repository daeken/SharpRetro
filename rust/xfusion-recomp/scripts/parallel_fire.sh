#!/bin/bash
# Parallel X64D corpus fire: split into N shards, run oracle_runner_v2 on each
# concurrently, aggregate RESULT lines. Assumes split_x64d.py + runner on box.
set -e
CORPUS="$1"; N="${2:-16}"; RUNNER="${3:-/tmp/oracle_runner_v4}"
BASE="${CORPUS%.x64d}"
echo "[parallel-fire] $CORPUS → $N shards"
python3 /tmp/split_x64d.py "$CORPUS" "$N" "${BASE}_shard" | head -3
echo "[parallel-fire] launching $N runners..."
T0=$(date +%s)
declare -a PIDS
for k in $(seq -f '%02g' 0 $((N-1))); do
  "$RUNNER" "${BASE}_shard.${k}" > "${BASE}_shard.${k}.log" 2>&1 &
  PIDS[$k]=$!
done
# Own #182: `wait` alone discards per-child rc → a post-RESULT segfault (or
# any nonzero exit that isn't the intended DIFF→rc=1) is invisible. Report
# per-shard rc; treat rc>1 as CRASH (rc=1 = intentional "diffs found").
CRASH=0
for k in $(seq -f '%02g' 0 $((N-1))); do
  wait ${PIDS[$k]}; rc=$?
  if [ $rc -gt 1 ]; then
    echo "[parallel-fire] ⚠ shard $k rc=$rc (>1 = crash/signal, NOT the diff-found rc=1)"
    CRASH=1
  fi
done
T1=$(date +%s)
echo "[parallel-fire] all shards done in $((T1-T0))s"
[ $CRASH -eq 1 ] && echo "[parallel-fire] ⚠ ONE OR MORE SHARDS CRASHED (rc>1) — results below may be partial"
echo ""
echo "[parallel-fire] AGGREGATE:"
python3 - "$BASE" "$N" <<'PY'
import sys, re
base, N = sys.argv[1], int(sys.argv[2])
tot_ok=tot_diff=tot_rej=tot_n=0
tmpl_diff = {}
for k in range(N):
    for L in open(f"{base}_shard.{k:02d}.log"):
        m = re.search(r"RESULT: (\d+) match / (\d+) DIFF / (\d+) reject \(of (\d+)\)", L)
        if m:
            ok,d,r,n = map(int, m.groups())
            tot_ok+=ok; tot_diff+=d; tot_rej+=r; tot_n+=n
        m = re.match(r"\s*template (\d+): (\d+) diff", L)
        if m:
            tid,c = int(m.group(1)), int(m.group(2))
            tmpl_diff[tid] = tmpl_diff.get(tid,0) + c
print(f"  RESULT: {tot_ok} match / {tot_diff} DIFF / {tot_rej} reject (of {tot_n})")
if tmpl_diff:
    print(f"  DIFF by template:")
    for tid,c in sorted(tmpl_diff.items(), key=lambda x:-x[1]):
        print(f"    template {tid}: {c}")
PY
echo ""
echo "[parallel-fire] cleanup shards (keep .log for verbose re-fire)"
rm -f ${BASE}_shard.[0-9][0-9]
