#!/bin/bash
# MACRO-ARM PARITY gate. A macro with per-arch arms is two implementations
# of one contract, and they are NOT symmetric in coverage: aarch64 is what every
# local test and golden runs under; x86_64 is what the game's boot runs under. So a
# patch that hits one arm PASSES EVERYTHING LOCALLY and is live-and-wrong on the
# deployed arm. Worked instance: a depth-guard patch hit x86_64 only and was caught
# by a hand-rolled anchor-count assert; this gate is that assert hoisted out of the
# patch, so the class is checked rather than the instance.
#
# It checks that load-bearing MARKERS appear the same number of times in every arm of
# each per-arch macro. Markers are the things whose absence is silent: the census
# prologue, the depth guard, the timing guard.
#
# BOTH SIDES SEEN is the acceptance bar: 'a gate never seen failing is untested' is
# right and not sufficient -- the pass path AND the fail path must each be watched
# once, because a gate can be broken only in the world where the news is good.
# WORKED INVOCATION — recorded because I fired this tool at the WRONG SUBJECT three hours
# after shipping it, and the wrong-subject result is indistinguishable from a real finding:
#
#   $ bash tools/arm-parity.sh <a-source-file-with-per-arch-macro-arms>
#     ok arm-parity: 1 per-arch macro pair(s), 4 markers, all arms agree      rc=0
#
#   $ bash tools/arm-parity.sh <a-file-with-NO-macros>
#     x arm-parity: found 0 per-arch macro pairs in … — no subject, read as FAIL   rc=1
#
# The second is the fire I made by accident. A file can be full of `cfg(target_arch)` and
# contain no `macro_rules!` at all (23 cfg-sites, 0 macros), so it has per-arch CODE without
# per-arch MACRO ARMS — which is not this tool's subject. The no-subject FAIL is correct and
# it reads exactly like "the arms disagree" if you skim the rc. Find a real subject with:
#   grep -rl 'macro_rules!' --include='*.rs' <tree>
#
# ⚠ AND READ THE rc DIRECT: `bash tools/arm-parity.sh f | tail -4; echo $?` reports TAIL's
# exit code, which is 0 on a FAILING gate. I did this on the same fire. Redirect, then read $?.
set -u
SRC="${1:-crates/alky-d3d12/src/com_macro.rs}"
MARKERS=("__SEAM_CPU_N" "__SEAM_CPU_NS" "Depth::enter" "seam_cpu_note")
fail=0

[ -f "$SRC" ] || { echo "  x arm-parity: $SRC absent — no subject, read as FAIL"; exit 1; }

# Find per-arch macro pairs: a `macro_rules! NAME` preceded (within 3 lines) by a
# cfg(target_arch). Derived from the file, not listed, so a new pair is covered.
mapfile -t starts < <(grep -n '^macro_rules! ' "$SRC" | cut -d: -f1)
declare -A arms
for ln in "${starts[@]}"; do
  name=$(sed -n "${ln}p" "$SRC" | sed 's/^macro_rules! \([a-z_]*\).*/\1/')
  head=$((ln-3)); [ $head -lt 1 ] && head=1
  sed -n "${head},${ln}p" "$SRC" | grep -q 'target_arch' || continue
  arms[$name]="${arms[$name]:-} $ln"
done

n_pairs=0
for name in "${!arms[@]}"; do
  read -ra lns <<< "${arms[$name]}"
  [ "${#lns[@]}" -ge 2 ] || continue      # single-arm macro: nothing to pair
  n_pairs=$((n_pairs+1))
  # each arm spans from its start to the next macro_rules! (or EOF)
  for i in "${!lns[@]}"; do
    s=${lns[$i]}
    e=$(awk -v s="$s" 'NR>s && /^macro_rules! /{print NR-1; exit}' "$SRC")
    [ -z "$e" ] && e=$(wc -l < "$SRC")
    for m in "${MARKERS[@]}"; do
      c=$(sed -n "${s},${e}p" "$SRC" | grep -c "$m" || true)
      eval "cnt_${i}_$(echo "$m" | tr -c 'a-zA-Z0-9' '_')=$c"
    done
  done
  for m in "${MARKERS[@]}"; do
    key=$(echo "$m" | tr -c 'a-zA-Z0-9' '_')
    a=$(eval echo "\$cnt_0_$key"); b=$(eval echo "\$cnt_1_$key")
    if [ "$a" != "$b" ]; then
      echo "  x $name: marker '$m' appears ${a}x in arm-1 but ${b}x in arm-2 — a one-arm patch"
      fail=1
    fi
  done
done

if [ "$n_pairs" -eq 0 ]; then
  echo "  x arm-parity: found 0 per-arch macro pairs in $SRC — no subject, read as FAIL"
  exit 1
fi
[ $fail -eq 0 ] && echo "  ok arm-parity: $n_pairs per-arch macro pair(s), ${#MARKERS[@]} markers, all arms agree"
exit $fail
