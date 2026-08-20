# Scoping: a corpus differential for the C# lowering arm

The C# `IlLower` vector arms are correct-by-transcription (from `interp.rs`) and
**execution-unverified**. This is the scoping for the arm that would close that, with
every precondition fired rather than assumed. Nothing here is built.

## Which link needs it

```
.isa formula -> interp.rs     CO-BLIND: both derive from the one .isa.
                              Needs silicon; HAS it for 15 of 38 mnemonics
                              (SWEEP-VECTOR-COVERAGE.md).
interp.rs    -> IlLower.cs    HAND-TRANSCRIBED. So interp.rs is an INDEPENDENT
                              oracle for it, and nothing compares them.
```

A second C# evaluator written from the .isa would be co-blind with `IlLower` — same
reading, same author. The independence comes from the corpus rows, which carry
`interp.rs`'s post-state already silicon-verified for those 15.

## Preconditions, each fired

**Rows carry full 128-bit xmm.** Read at bytes from `sweep_p2_GOLDEN.x64d`:

```
xmm0 pre  = 3f800000_3f800000_3f800000_3f800000    hi word NONZERO
xmm: [u128; 32]   ·   to_flat writes both words   ·   STATE_WORDS_X64 = 90
```

The `lo-only` caveat on `X64_LAYOUT.reg_off` binds the tier-0 JIT, which has zero
callers today — not the row format. Scoped at `state.rs` so the next reader of that
line doesn't conclude the comparand is half-blind.

**The instruction is recoverable.** Fixed-length prologue, test bytes appended at
`SLOT_OFF`, which must be **derived from `stub_len`** and not composed:

```
stub_len 191 -> SLOT_OFF  82   (v1, GPR)
stub_len 479 -> SLOT_OFF 226   (v2, xmm: movdqu load/store around the slot)
stub_len  85 -> SLOT_OFF  29   (32-bit)
stub_len 213 -> SLOT_OFF  93   (32-bit xmm)
```

Composing 226 for a 191-byte row yields a 0-byte instruction that reads as an empty
field rather than as an error.

**No row in the 15 has an uninitialized post-xmm.** The v2-stub choice keys on
`def_has_xmm` (any operand in `{Vxmm, Wxmm, Uxmm}`, read *or* write side), not on the
read-set — so a write-only vector def still gets `movdqu` capture:

```
MOVMSKPS  Greg,Uxmm   v2      reads xmm, writes GPR
MOVD-X    Vxmm,Erm    v2      WRITE-only xmm, reads GPR
all 15    Vxmm,Wxmm   v2      17/17, none on v1
```

**The reader has an acceptance population that predates it.** Four scalar-SSE defs
have both a C# test case and corpus rows, so the reader can be verified against a
post-state the C# side already computes correctly:

```
mnem      corpus rows   C# ExecTests cases
ADDSS           6,448   1
MULSS           6,448   1
SUBSS           6,448   1
COMISS          6,448   1
DIVSS           6,448   *** 0 ***
UCOMISS         6,448   *** 0 ***
```

**⚠ CORRECTED 2026-08-20 (@cfe05bc's sibling).** An earlier version of this table said
`ADDSS 77,376 · MULSS 77,376 · DIVSS 3,328 · COMISS 510`, each with 1 C# test. Both
columns were wrong:

- The ROW counts were joined through `def_id` against today's `DEF_MNEMONICS`, and the
  corpus is frozen against an earlier def table — see SWEEP-VECTOR-COVERAGE.md. The
  numbers above are decoded from each row's own bytes. All six sit at 6,448, which is
  itself the tell I should have noticed: four *different* figures for four defs of the
  same encoding shape was implausible.
- DIVSS and UCOMISS have **zero** C# test cases. I asserted 1 each from the family
  rather than counting, so two of the six could not have served as an acceptance
  population at all.

So the acceptance set is **four** defs, not six. That is still enough (a reader needs one
known-good post-state, and four is a cross-check), but the claim as published was wrong
in the direction that made the next step look easier than it is.

## Where the reader goes (searched, 2026-08-20)

No C#-side X64D reader exists — `grep -rl 'X64D|stub_len|pre\[90\]' --include=*.cs` hits
only `XFusionGenerator/SweepGen.cs`, which WRITES the def facts rather than reading rows.

It belongs beside `oracle-baseline/instruments/XFCorpus/` (28 lines), which is the closest
sibling: a linear-sweep decode-boundary dumper whose output diffs against the Rust
decoder's on the same bytes — i.e. the phase-1-full gate. Same shape (a C# instrument
that consumes a binary and cross-checks the Rust arm), different subject (boundaries vs
state).

‡ A near-miss worth the line: `ls -d XF*` from the repo root shows five projects and NOT
XFCorpus, because it lives two levels down. I was one command from "correcting" a true
claim about it into a false one. The reference set derived from the artifact — every
`.csproj` naming `XFusionCpu` — is XFusionCensus / XFusionJit / XFusionTests /
oracle-baseline(XFCorpus), plus the `Pagentry.Lifter` consumer outside this tree.

## Sequencing

```
1. reader   read [u64; 90] pre/post, reconstruct, compare against ExecBlock's state.
            Acceptance: reproduce a known-good post for ADDSS/MULSS/DIVSS/COMISS.
            No carrier change needed -- those are scalar.
2. carrier  Eval returns ulong (17 call sites) and Xmm is ulong[32]. An IlVecElem at
            lane >= 2 is UNREPRESENTABLE until this widens. UInt128 is already used
            8x in the file.
3. arms     the 5 IlVec* Eval arms. Differential against the 15 silicon-clean families.
```

**Step 2 before step 3, and the reason is safety rather than correctness.** Today an
`IlVec*` node hits `default: throw new NotSupportedException` — a loud refusal naming
the type. Adding the arms over the `ulong` carrier converts that into a silent wrong
value on lanes 2-3. So arms-before-carrier is the one ordering worse than doing nothing.

(An earlier draft of this had it as `carrier -> reader -> arms` and treated the
widening as a cost to minimize. The reader is testable first and against a population
that already exists, which puts a self-checked comparand in front of the risky change.)

## The reader needs no fresh sweep (read the stub, not the def_id)

An X64D row is `{def_id:u32, flags_mask:u32, stub_len:u32, stub[stub_len],
pre[STATE_WORDS_X64], post[...]}` (`src/bin/sweep.rs:641-646`), and **the stub carries
the instruction bytes at a known offset** — SLOT_OFF, derived from stub_len (82 for the
64-bit v1 stub, 29 for 32-bit, 226 for the XMM v2 stub; `sweep.rs:95`, `:421`).

So a reader takes the insn from the stub and **never reads `def_id`**. That matters
because the corpus is frozen against a 2026-08-11 def table: a def_id lookup against
today's names mislabels ~74% of rows, and every mislabel is a plausible mnemonic (see
SWEEP-VECTOR-COVERAGE.md). Reading the bytes sidesteps the whole class, and it means the
frozen 4,088,162-row corpus is usable AS-IS — a fresh sweep is not a prerequisite for
step 1.

## What this still won't verify

Link-2. The corpus grades `interp.rs` against silicon; the C# `IlLower` lowerings are a
hand transcription verified by nothing. That is what the reader is for, and it is the
whole point of the sequence above.

(An earlier draft of this section claimed 23 mnemonics had no corpus rows and inferred an
unnamed "third gate" excluding them. There is no third gate: the figure came from a
def_id join against a drifted table, plus a `take(20)` truncation in the census output I
was reading. Measured by decoding each row's own bytes: **40 of 40** vector mnemonics
have silicon rows. An inferred mechanism explaining a wrong figure reads as rigor and is
the flattering direction twice over — recorded here because the correction landed in the
sibling file first and this one stayed stale for an hour.)
