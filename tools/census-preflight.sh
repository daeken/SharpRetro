#!/bin/bash
# CENSUS PREFLIGHT — before you count occurrences of a string in a log, count the
# PRINT-SITES that can produce it.
#
# A log census is an instrument whose operand is a regex, and the regex's population is
# decided by the source, not by the log. Two failure modes, both silent:
#
#   >1 printer : the count SUMS populations whose only shared property is a substring.
#                Worst case one of them is a setup/arming line that fires unconditionally
#                at startup, so every count carries a constant that reads as an event.
#                (Live instance this was built from: a pattern matching both a real
#                failure print AND an "ARMED: … autopsy on <PATTERN>" startup line, which
#                turned "1 arm + 0 real events" into "routine failures in every run".)
#    0 printers: the count is structurally 0 forever, and 0 is the most plausible-looking
#                value a counter can have — nothing about it invites suspicion. A zero
#                from a pattern no code can emit is byte-identical to a zero from a
#                healthy system.
#
# So: exactly-one printer is the only state in which a count means one thing.
#
# Usage:  census-preflight.sh <source-root> <pattern> [<pattern> ...]
#         (takes the tree as an argument so it can gate a tree it does not live in)
#
# WORKED INVOCATION — a usage line names the SHAPE of a subject; this names a real one,
# because a tool whose subject has to be guessed gets fired at the wrong one (I did that
# to this file's sibling three hours after shipping it):
#
#   $ bash tools/census-preflight.sh <a-rust-crates-tree> 'vkAllocateMemory FAILED'
#     ok /vkAllocateMemory FAILED/ — 1 print-site (a count of this pattern means one thing)
#   rc=0
#
#   $ bash tools/census-preflight.sh <same-tree> 'DEVICE_LOST'
#     x /DEVICE_LOST/ — 2 print-sites: AMBIGUOUS. A count sums these populations:
#         …/device_fault.rs:28:  eprintln!("[device-fault] ARMED: … autopsy on DEVICE_LOST"
#         …/device_fault.rs:36:  eprintln!("[device-fault] DEVICE_LOST (no fault-ext armed…"
#         → if any of them is a setup/arming line, every count carries a constant.
#   rc=1
#
# That second case is the live instance this was built from: site :28 fires ONCE PER RUN,
# unconditionally, at startup — so every count of /DEVICE_LOST/ carries a guaranteed +1 that
# reads as an event, which turned "1 arm + 0 real events" into "routine failures in every run."
#
# ⚠ READ THE rc DIRECT. `census-preflight.sh … | tail -2; echo $?` reports TAIL's exit code,
# which is 0 while the gate is FAILING. Redirect to a file, then read $?. (I made exactly this
# mistake reading this tool's own output — the pipe-hides-rc class, at the gate that checks
# for a different flavour of the same thing.)
#
# BOTH SIDES SEEN is the acceptance bar. A gate whose pass path has never run is as
# untested as one whose fail path hasn't: a threshold check can be broken only in the
# world where the news is good (a `grep -c` under `set -e` exits 1 at zero, so it dies
# exactly when the count it reports is clean).
#
# ⚠ KNOWN BOUND, stated rather than hidden: this counts print-sites whose LINE carries
# both the pattern and a print macro. A multi-line print whose format string sits below
# its macro is MISSED, so a `1` result means "one site I can see" while a `>1` result is
# a hard finding. The asymmetry is the right way round (it under-reports ambiguity rather
# than inventing it), but a census whose pattern lives in a wrapped print wants the block
# read by hand.
set -u

ROOT="${1:-}"; shift || true
[ -n "$ROOT" ] && [ -d "$ROOT" ] || { echo "  x census-preflight: source root '$ROOT' is not a directory — no subject, read as FAIL"; exit 1; }
[ "$#" -gt 0 ] || { echo "  x census-preflight: no patterns given — a gate with no subject is not a clean gate"; exit 1; }

# Print-emitting constructs. Derived per-language rather than assumed: extend this list
# when gating a tree that prints some other way (the list is the gate's own operand, and
# a pattern that matches nothing because the PRINT vocabulary is wrong reads as 0 sites,
# which this gate reports as a FAIL for exactly that reason).
PRINTERS='eprintln!|println!|eprint!|print!|write!|writeln!|log::|tracing::|seam_log|Console\.|printf|fprintf|std::cerr|std::cout'

fail=0
for pat in "$@"; do
  # -a: a NUL byte anywhere in a file makes grep declare it binary and SKIP IT SILENTLY,
  # with the tell on stderr where nobody reads it. A gate that reports clean over an
  # unread input is worse than no gate.
  hits=$(grep -ranE "$pat" "$ROOT" --include='*.rs' --include='*.cs' --include='*.c' \
           --include='*.cpp' --include='*.h' --include='*.py' 2>/dev/null \
         | grep -E "$PRINTERS" | grep -vE ':[[:space:]]*//' || true)
  n=$(printf '%s' "$hits" | grep -c . || true)

  if [ "$n" = "1" ]; then
    echo "  ok /$pat/ — 1 print-site (a count of this pattern means one thing)"
  elif [ "$n" = "0" ]; then
    echo "  x /$pat/ — ZERO print-sites: no code in $ROOT can emit this. A census on it"
    echo "      returns 0 forever, which is indistinguishable from a healthy zero."
    fail=1
  else
    echo "  x /$pat/ — $n print-sites: AMBIGUOUS. A count sums these populations:"
    printf '%s\n' "$hits" | sed 's/^/      /' | cut -c1-140
    echo "      → if any of them is a setup/arming line, every count carries a constant."
    fail=1
  fi
done

exit $fail
