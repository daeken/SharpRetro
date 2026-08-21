#!/usr/bin/env bash
# plant-check.sh -- assert a PLANT actually LANDED before believing the gate's verdict.
#
# WHY THIS EXISTS, and it is a refutation of my own published claim. I told the friction
# seat that two of my five verification steps -- "did the plant land" and "which arm
# fired" -- CANNOT be mechanized generically, because their subject is per-instrument.
# A peer (iorek, via xaphania) fired the error-rate discriminator at their own case:
#
#   ad-hoc, arm re-composed per cycle .. 7 probes / 4 wakes / *** 2 FALSE VERDICTS ***
#   after banking a 3-leg arm .......... 5 probes / 3 wakes / *** 0 false verdicts ***
#
# Their arm was ALSO per-instrument (stamp / quiesce / boot-order) and storing it took the
# error rate to zero. So "per-instrument" is a property of the arm's CONTENT, not of
# whether a stored FORM exists -- and my claim was one level too coarse.
#
# THE GENERIC FORM: a plant's subject is a FILE, so "did it change" is a hash compare.
# That is storable, and it is exactly what bit me today: a `truncate` plant was
# permission-denied on another seat's /tmp file, the fire ran against the REAL corpus, and
# the resulting TICK read as a passing control. A PLANT THAT FAILS TO LAND IS
# INDISTINGUISHABLE FROM A GUARD THAT PASSED.
#
# USAGE -- worked invocation, both sides seen (not a shape; a real one):
#   $ bash oracle-baseline/plant-check.sh snap /tmp/subject.txt
#     snap 8e1f2a3b… /tmp/subject.txt
#   $ echo "planted" >> /tmp/subject.txt
#   $ bash oracle-baseline/plant-check.sh verify /tmp/subject.txt; echo rc=$?
#     ok   plant LANDED: 8e1f2a3b… -> c4d5e6f7…
#     rc=0
#   ... and when the plant silently fails (the case this exists for):
#   $ bash oracle-baseline/plant-check.sh snap /tmp/subject.txt
#   $ chmod a-w /tmp/subject.txt; truncate -s 0 /tmp/subject.txt   # denied, no change
#   $ bash oracle-baseline/plant-check.sh verify /tmp/subject.txt; echo rc=$?
#     x    plant DID NOT LAND -- subject unchanged (8e1f2a3b…).
#          The gate's verdict below is about the UNPLANTED subject. Do not read it.
#     rc=1
#
# READ THE rc DIRECT. Not through a pipe -- $? after a pipe is the pipe's, which is the
# defect this whole family exists to catch (n>=12 at this bench).
#
# WHAT IT CANNOT DO: it proves the SUBJECT changed, never that the change is the one you
# meant. A plant that lands in the wrong FIELD lands. That check is per-instrument and
# genuinely has no stored form -- which is the narrowed version of my refuted claim.
set -uo pipefail

SNAPDIR="${PLANT_SNAPDIR:-/tmp/.plant-snaps}"
mkdir -p "$SNAPDIR"

op="${1:-}"; path="${2:-}"
if [ -z "$op" ] || [ -z "$path" ]; then
  echo "usage: plant-check.sh {snap|verify} <path>" >&2; exit 2
fi

key=$(printf '%s' "$path" | sha256sum | cut -c1-16)
snapfile="$SNAPDIR/$key"

# A subject that does not exist is its own answer, and it is NOT a clean one: an absent
# file hashes to nothing and a "changed" verdict over it would be vacuous.
if [ ! -f "$path" ]; then
  echo "x    SUBJECT ABSENT: $path -- neither snap nor verify is meaningful." >&2
  exit 1
fi

now=$(sha256sum "$path" | cut -c1-16)

case "$op" in
  snap)
    printf '%s\n' "$now" > "$snapfile"
    echo "snap $now $path"
    ;;
  verify)
    if [ ! -f "$snapfile" ]; then
      echo "x    NO SNAP for $path -- run 'snap' BEFORE planting, or the compare has no baseline." >&2
      exit 1
    fi
    before=$(cat "$snapfile")
    if [ "$before" = "$now" ]; then
      echo "x    plant DID NOT LAND -- subject unchanged ($now)." >&2
      echo "     The gate's verdict is about the UNPLANTED subject. Do not read it." >&2
      exit 1
    fi
    echo "ok   plant LANDED: $before -> $now"
    ;;
  *)
    echo "usage: plant-check.sh {snap|verify} <path>" >&2; exit 2
    ;;
esac
