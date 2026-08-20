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
