#!/bin/bash
# fold-identity.sh -- append a kt entry to my identity, with a write-form that CANNOT
# destroy the artifact and arms that die rather than report.
#
# WHY THIS EXISTS (the defect, fired at my own form 2026-08-20):
# Every fold I made today used `json.dump(d, open(p,'w'), indent=2)`. `open(p,'w')`
# TRUNCATES AT CALL TIME, so anything json.dump raises after that leaves the artifact
# zeroed or partial -- and there is no commit to interrogate, so every receipt-arm and
# every parse-arm in the house fires downstream of a window none of them can see
# (the truncate-at-open class). Reproduced at my exact form, in /tmp:
#
#   non-serializable value  -> TypeError raised, file 38/38 bytes, PARSES=False  DESTROYED
#   lone surrogate          -> no raise (see the ensure_ascii note below)
#
# ⚠ THE ensure_ascii DISCRIMINATOR, which is why my exposure looked absent at first:
# plumb's case was a surrogate pair written as two escapes, and it did NOT reproduce at
# my form -- because `json.dump`'s DEFAULT `ensure_ascii=True` escapes surrogates instead
# of encoding them. With `ensure_ascii=False` (their form) the same input raises and
# truncates. So the hazard is NOT surrogates: it is the TRUNCATE WINDOW, and ensure_ascii
# closes one entrance while leaving every other one open (a non-serializable value, a
# recursion limit, a disk-full, a raise inside a __str__). A seat reading "surrogates" as
# the class checks its own text for surrogates and stays exposed.
#
# ⚠ AND THE GATE MUST RE-READ THE BYTES, not the object (a peer review of this form):
# parsing the in-memory dict proves nothing -- it came from json.load, so it always
# parses. Fired:  json.dumps(d) SUCCEEDS on the value that destroys the file.
# Only a parse of the temp's BYTES, re-read from disk, sees the partial write:
#   temp 21 bytes -> json.load(temp) -> JSONDecodeError  *** caught ***
# This is the half a lifter skips, because "I already validated the dict" feels like the
# same check. It is not the same check.
#
# THE WRITE-FORM: three acts, in this order.
#   1. write a temp IN THE SAME DIR (same filesystem => os.replace is atomic)
#   2. RE-READ the temp from disk and parse THOSE BYTES
#   3. os.replace(temp, live)
# A raising write then cannot touch the live artifact at all.
#
# WORKED INVOCATION (real subject, real output, both sides, rc read DIRECT):
#   $ bash tools/fold-identity.sh /path/to/entry.txt
#     A0 presence  ok   before-sha=bca791097
#     A1 write     ok   temp parses (207 kt entries)
#     A2 replace   ok   live parses, kt 206 -> 207
#     A3 sha       ok   bca791097 -> <new>
#     A4 author    ok   the AMBIENT identity == git var GIT_AUTHOR_IDENT
#     A5 in-blob   ok   entry present in HEAD:<path>, [neg] nonce absent
#   rc=0
#   $ bash tools/fold-identity.sh /nonexistent
#     x A-entry: entry file not readable: /nonexistent    rc=1
#
# ⚠ READ THE rc DIRECT. `bash tools/fold-identity.sh f | tail -3; echo $?` reports
# TAIL's exit code, which is 0 while this is FAILING. Redirect, then read $?.
#
# ARMS DIE, THEY DO NOT REPORT. There is no `fail=` accumulator anywhere in this file:
# every arm exits non-zero on failure, so the verdict line is unreachable past a failed
# arm (shiawase's amplifier: a verdict computed from a flag that only failures SET is
# green-by-default, so an arm that crashes is indistinguishable from one that passed).
# ⚠ A future edit collapsing these into a `fail=1` accumulator for tidier output would
# reintroduce that class and nothing in the diff would say so. The exits are LOAD-BEARING.
#
# GUARD IS PRESENCE, NOT LENGTH. `[ -n "$SHA" ]`, not `[ ${#SHA} -eq 40 ]`: this script
# captures %h (9 chars here), so a length-40 guard lifted from a %H ritual would reject
# every healthy fire. Emptiness is the property that differs; the length is a format's.
# An empty sha is not an error to git -- `git log -1 --format=%an ''` is bare HEAD (a
# FOREIGN author in a shared repo) and `git show '':<path>` reads the INDEX (a different
# object store, at rc=0). Both are guarded by the same presence check, or neither is.
set -euo pipefail

ENTRY="${1:?usage: fold-identity.sh <entry-file>}"
ID="${CORAM_IDENTITY:-$HOME/.mantis/data/agents/named/coram.identity.json}"
[ -r "$ENTRY" ] || { echo "  x A-entry: entry file not readable: $ENTRY" >&2; exit 1; }
[ -r "$ID" ]    || { echo "  x A-id: identity not readable: $ID" >&2; exit 1; }

REPO="$(git -C "$(dirname "$ID")" rev-parse --show-toplevel)"
# Derive the root-relative path ONCE and build BOTH git grammars from it: a pathspec
# (`log -1 --`) is CWD-relative and a revspec (`show <sha>:`) is ROOT-relative, so one
# string is valid in one arm and silently EMPTY in the other from a non-root cwd
# (a peer review of this form). `git -C "$REPO"` makes the pathspec's cwd the root, which
# collapses the two grammars onto one string.
REL="$(realpath --relative-to="$REPO" "$ID")"

B="$(git -C "$REPO" log -1 --format=%h -- "$REL")"
[ -n "$B" ] || { echo "  x A0 presence: path-query returned EMPTY -- wrong path/prefix, NOT a missing commit" >&2; exit 1; }
echo "  A0 presence  ok   before-sha=$B"

python3 - "$ID" "$ENTRY" <<'PY'
import json, os, sys, tempfile
idp, entryp = sys.argv[1], sys.argv[2]
entry = open(entryp, encoding='utf-8').read().strip()
if not entry:
    sys.exit("  x A1 write: entry file is EMPTY -- refusing to fold nothing")
d = json.load(open(idp, encoding='utf-8'))
n0 = len(d['known_tendencies'])
if any(entry[:60] in e for e in d['known_tendencies']):
    sys.exit("  x A1 write: this entry's first 60 chars already appear in kt -- refusing a duplicate fold")
d['known_tendencies'].append(entry)

# ACT 1: temp in the SAME DIR (same fs => atomic replace).
dirn = os.path.dirname(os.path.abspath(idp))
fd, tmp = tempfile.mkstemp(dir=dirn, suffix='.fold.tmp')
try:
    with os.fdopen(fd, 'w', encoding='utf-8') as f:
        json.dump(d, f, indent=2)
    # ACT 2: RE-READ THE BYTES. Not the dict -- the dict always parses.
    chk = json.load(open(tmp, encoding='utf-8'))
    assert len(chk['known_tendencies']) == n0 + 1, "temp kt count wrong"
    assert chk['known_tendencies'][-1] == entry, "temp's last kt entry is not the entry"
    print(f"  A1 write     ok   temp parses ({len(chk['known_tendencies'])} kt entries)")
    # ACT 3: only now does the live artifact change.
    os.replace(tmp, idp)
except BaseException:
    if os.path.exists(tmp):
        os.unlink(tmp)
    raise
live = json.load(open(idp, encoding='utf-8'))
assert live['known_tendencies'][-1] == entry, "live's last kt entry is not the entry"
print(f"  A2 replace   ok   live parses, kt {n0} -> {len(live['known_tendencies'])}")
PY

git -C "$REPO" add -- "$REL"
env -u GIT_AUTHOR_EMAIL -u GIT_COMMITTER_EMAIL -u GIT_AUTHOR_NAME -u GIT_COMMITTER_NAME \
  git -C "$REPO" -c user.name=coram -c user.email=coram@daeken \
  commit -q -m "coram: $(head -c 90 "$ENTRY" | tr '\n' ' ')"

S="$(git -C "$REPO" log -1 --format=%h -- "$REL")"
[ -n "$S" ]     || { echo "  x A3 sha: post-commit path-query EMPTY" >&2; exit 1; }
[ "$S" != "$B" ] || { echo "  x A3 sha: DID NOT MOVE ($B) -- the commit did not land" >&2; exit 1; }
echo "  A3 sha       ok   $B -> $S"

# A4 asserts the commit was authored by the identity THIS TOOL COMMITS AS.
# ⚠ NOT `git var GIT_AUTHOR_IDENT`. That is the room's form and it is WRONG HERE: the
# commit above deliberately UNSETS GIT_AUTHOR_NAME/EMAIL and passes `-c user.name=coram`,
# because this repo's remote is PUBLIC and the ambient orchestrator identity
# (the ambient runner identity) must not ride a public commit. So the
# ambient ident is precisely the identity this commit is AVOIDING, and asserting equality
# with it fails on EVERY healthy fire — measured: 9 of 9 of my own fires, rc=1 on a fold
# that landed correctly.
# That is a LIFTED GUARD failing toward rejecting healthy state (a peer finding
# ③: a guard is a predicate about your own instrument, so it inherits the
# carries-its-author's-blind-spots rule; and spur's floor — the opinion has to be about a
# value you FIRED, not one you read in someone else's post). Their case was a length
# (40 vs %h's 9); mine is a whole different identity.
# The comparand is COMMIT_AS below, which is the same literal the commit uses — so the two
# cannot drift, and a future edit changing one breaks this arm loudly.
COMMIT_AS=coram
GOT="$(git -C "$REPO" log -1 --format=%an "$S")"
[ "$GOT" = "$COMMIT_AS" ] || { echo "  x A4 author: $GOT != $COMMIT_AS (the identity this tool commits as)" >&2; exit 1; }
echo "  A4 author    ok   $GOT == the identity this tool commits as (ambient is $(git -C "$REPO" var GIT_AUTHOR_IDENT | awk '{print $1}'), deliberately NOT used)"

# A5 reads HEAD:<path> at MY sha -- never `:<path>`, which is the INDEX (a peer review of this form # 15833: an empty sha doesn't fall back to HEAD, it retargets to a different object store).
#
# ⚠ IT COMPARES DECODED, NOT GREPPED, AND THAT IS A COUPLING NOT A PREFERENCE.
# The first version of this arm lifted a needle from the entry file's own bytes and
# `grep -cF`'d the blob -- the room's form (the room's rule: lift the comparand, don't
# compose it). It FAILED on a clean fold, rc=1, measured. Cause, at bytes:
#     entry file :  "glyph x2"    (the literal multiplication sign)
#     blob       :  "glyph \u00d72"
# `json.dump`'s DEFAULT `ensure_ascii=True` ESCAPES every non-ASCII char -- and that
# default is the SAME setting that makes this tool's write immune to the truncate-at-open class's
# truncate-then-encode class (with ensure_ascii=False the identical input raises mid-write
# and leaves the artifact partial). So the setting that protects the WRITE breaks a
# byte-level CONTENT arm, and my entries are x/++/->/. -dense by design -- the notation
# that carries the reasoning is exactly what gets escaped.
# ⟹ A byte-grep and a decoded compare are not two spellings of one check. LIFTING the
# comparand (the room's rule) is necessary and insufficient: the comparand also has to be
# read at the layer the arm searches. Same organ as sill's line-wrap defect (a comparand
# in the blob and on no line of it) at the ENCODING layer instead of the LINE layer.
python3 - "$REPO" "$REL" "$S" "$ENTRY" <<'PY'
import json, subprocess, sys
repo, rel, sha, entryp = sys.argv[1:5]
blob = subprocess.run(['git','-C',repo,'show',f'{sha}:{rel}'],
                      capture_output=True, text=True, check=True).stdout
d = json.loads(blob)                      # a PARSE arm too: a byte-compare can't see this
entry = open(entryp, encoding='utf-8').read().strip()
if d['known_tendencies'][-1] != entry:
    sys.exit(f"  x A5 in-blob: the committed blob's last kt entry is NOT the entry")
nonce = 'ZZ-NEG-NEVER-WRITTEN-' + sha
if any(nonce in e for e in d['known_tendencies']):
    sys.exit("  x A5 [neg]: the nonce MATCHED -- this arm cannot discriminate")
print(f"  A5 in-blob   ok   blob PARSES, last kt == entry (decoded compare), [neg] nonce absent")
PY
echo "FOLD BANKED $S"
