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
ADDSS   77,376 rows   ·  1 C# test
MULSS   77,376        ·  1
DIVSS    3,328        ·  1
COMISS     510        ·  1
```

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

## What this still won't verify

The 23 mnemonics with no corpus rows. `phase1_skip` admits all ten vector families
with zero skips, yet those 23 appear in neither the emitted tally, the track-fail list,
nor `DEF_IS_INTRINSIC` — a third gate excludes them and it is unnamed. Until it is,
*eligible* is not *covered*.
