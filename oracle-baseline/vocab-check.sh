#!/usr/bin/env bash
# Pre-commit house-vocab check over the STAGED ADDITIONS only.
#
# WHY added-only: `git diff --cached | grep -f patterns` matches a '-' row when you
# DELETE a vocab line, so scrubbing trips the check that told you to scrub. That
# happened twice in one session and both times the operator (me) printed
# "empty = clean" underneath non-empty output and pushed anyway -- a gate that
# cries wolf on the fix teaches its caller to read past it, which is worse than
# no gate. Only ADDED lines can publish anything.
#
# WHY a file and not an inline one-liner: an inline pipeline gets retyped per commit
# and the whole-diff form is what fingers reach for. A file can be fixed once.
#
# Exit: 0 = clean, 1 = a hit (printed), 2 = no subject (the pattern file is missing,
# which is NOT clean -- a gate with no subject must fail loudly).
set -u
# ── SIBLING SPELLINGS, added 2026-08-21 (a peer's finding: kaisa@seratb via xaphania). ──
# THE CLASS: a literal needle returns a CLEAN ZERO on an ENCODED spelling of the same
# fact, and a clean zero here means CLEAN, which is the PUSH direction. Their instance
# was one SELECT over a policy store: bare token 2,391 · URL-encoded 2,390 ·
# literal-colon ZERO -- so the colon needle would have "proven" an absence over a
# population of 2,390.
#
# AT THIS GATE, measured rather than assumed: of 13 patterns exactly ONE is
# case/encoding-fragile -- the channel-cite, whose middot a default-escaping writer
# (System.Text.Json, python json.dumps) stores as \u00B7 with UPPER-case hex. Fired it:
#   literal-needle hits on the encoded form ... 0
#   sibling-needle (u00b7) hits ............... 1
# Reachability: 1 tracked .json in this repo and 0 escaped-unicode occurrences today, so
# the hole is REAL with nothing currently in it -- the state that reads as "no problem"
# until a staged JSON fills it.
#
# THE FIX IS THE SIBLING, NOT A BETTER [pos]: a nonzero [pos] on a known-present string
# proves the arm is not DEAD; a SIBLING SPELLING proves it is reading the RIGHT BYTES,
# which no [pos] can. The pattern file now carries both spellings and grep runs -i so
# both sides fold. Plant-verified three ways: encoded -> rc=1 · literal -> rc=1 (no
# regression) · clean -> rc=0.
PAT="${1:-oracle-baseline/.vocab-patterns.txt}"
if [ ! -f "$PAT" ]; then
  echo "  x vocab: pattern file absent ($PAT) -- NO SUBJECT, treat as FAIL not as clean"
  exit 2
fi
# Added lines only, minus the '+++' file header (which carries paths, not content).
HITS=$(git diff --cached | grep '^+' | grep -v '^+++' | grep -nf "$PAT" || true)
if [ -n "$HITS" ]; then
  echo "  x vocab: house-vocab in STAGED ADDITIONS -- scrub before commit"
  printf '%s\n' "$HITS" | head -8 | sed 's/^/      /'
  exit 1
fi
echo "  ok vocab: no house-vocab in staged additions"
exit 0
