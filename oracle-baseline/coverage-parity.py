#!/usr/bin/env python3
"""Coverage parity between the .isa's backends.

The compile matrix asks "does each backend BUILD". This asks the adjacent
question: "do they cover the SAME defs, and does either STUB one the other
implements?" A backend that silently stubs a def the sibling lifts is exactly
the class that hid the def[5] break for five days -- byte-equivalence and
compilability both pass over it, and only an execution oracle (the fuzz)
catches it, and only on the arm that has one.

Zero divergence today; the arm exists so a future .isa change can't reintroduce
it quietly. Prints counts always -- a parity claim with no denominators is not
a measurement.
"""
import re, sys

def named(path, pat=r'/\* (.+?) \*/'):
    try: return set(re.findall(pat, open(path).read()))
    except FileNotFoundError: return None

def stubbed(path, markers):
    try: t = open(path).read()
    except FileNotFoundError: return None
    rx = r'/\* (.+?) \*/[^/]{0,400}?(?:' + '|'.join(markers) + ')'
    return set(re.findall(rx, t, re.S))

isa, cs_path, rs_path = sys.argv[1], sys.argv[2], sys.argv[3]
defs = [m.group(1) for m in re.finditer(r'^\(def\s+(\S+)', open(isa).read(), re.M)]
if not defs:
    print("  x coverage-parity: 0 defs parsed from %s -- no subject, read as FAIL" % isa); sys.exit(1)

cs, rs = named(cs_path), named(rs_path)
if cs is None or rs is None:
    print("  x coverage-parity: a backend artifact is absent (cs=%s rs=%s)" % (cs is not None, rs is not None)); sys.exit(1)

mc = [d for d in defs if d not in cs]
mr = [d for d in defs if d not in rs]
cstub = stubbed(cs_path, ['NotSupportedException']) or set()
rstub = stubbed(rs_path, [r'todo!', r'unimplemented!']) or set()
only_rs = sorted(rstub - cstub); only_cs = sorted(cstub - rstub)

fail = 0
print("  defs=%d  named-C#=%d  named-Rust=%d  stub-C#=%d  stub-Rust=%d"
      % (len(defs), len(defs)-len(mc), len(defs)-len(mr), len(cstub), len(rstub)))
for label, miss in (("C#", mc), ("Rust", mr)):
    if miss:
        print("  x %s emits no arm for %d defs: %s" % (label, len(miss), miss[:5])); fail = 1
if only_rs:
    print("  x Rust STUBS %d defs C# implements: %s" % (len(only_rs), only_rs[:5])); fail = 1
if only_cs:
    print("  x C# STUBS %d defs Rust implements: %s" % (len(only_cs), only_cs[:5])); fail = 1
if not fail: print("  ok coverage parity: both backends cover all %d defs, no one-sided stubs" % len(defs))
sys.exit(fail)
