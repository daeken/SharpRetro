# Silicon coverage of the vector families

**CORRECTED 2026-08-20.** An earlier version of this file said **15 of 38** mnemonics had
silicon rows. That figure was wrong: it joined `def_id` against **today's**
`DEF_MNEMONICS`, and the corpus is a frozen artifact generated **2026-08-11** against an
earlier def table. The real answer, read from each row's own instruction bytes, is
**40 of 40**.

## The corrected figure

Every vector family my ten `IlLower` heads serve has silicon-verified rows in
`sweep_p2_GOLDEN.x64d` (4,088,162 rows, 0 diff on bare Intel):

```
family        cov      rows   mnemonics
vfminmax      4/4    25,792   MAXPS MINPS MAXPD MINPD
vcvt          8/8    26,624   CVTDQ2PS CVTPS2PD CVTTPS2DQ CVTPS2DQ CVTPD2PS
                              CVTTPD2DQ CVTPD2DQ CVTDQ2PD
vzip          6/6    38,688   PUNPCKL{BW,WD,DQ} PUNPCKH{BW,WD,DQ}
vibin-mask    6/6    38,688   PCMPEQ{B,W,D} PCMPGT{B,W,D}
vmovmsk       3/3     9,360   MOVMSKPS MOVMSKPD PMOVMSKB
vshuf         3/3   194,688   SHUFPS SHUFPD PSHUFD
vhadd         2/2    12,896   HADDPS HADDPD
vdpp          2/2   154,752   DPPS DPPD
vshufw        2/2    79,872   PSHUFLW PSHUFHW
vfcmpp        2/2   154,752   CMPPS CMPPD
vfun          2/2     6,656   SQRTPS SQRTPD
              -----------------
              40/40
```

Acceptance population for a corpus reader (defs with both a C# `ExecTests` case and
rows): ADDSS / MULSS / DIVSS / SUBSS / COMISS / UCOMISS, 6,448 rows each.

## Why the first figure was wrong, and what it means for anyone reading a row

**`def_id` in a row is not an index into today's `DEF_MNEMONICS`.** Decoding each row's
own bytes with the current decoder and comparing:

```
AGREE  95 of 364 distinct stored ids
DIFFER 269
agree max = 181   ·   differ min = 158   ⟹ *** INTERLEAVED ***
```

The overlap is the load-bearing part: agreeing and differing ids **interleave**, so this
is not a single offset anyone can correct for. Defs were inserted at several points
between 08-11 and now (current emit is 4,091,490 rows against the corpus's 4,088,162 —
+3,328).

Worked example of the failure: stored `def_id 534` reads `PSRAD-I` in today's table, and
that row's bytes are `66 0F 3A 40 C0 00` = **DPPS**. So my "absent" list was reporting
DPPS's 77,376 rows under the label `PSRAD-I`, and eleven families I published as
having zero rows have between 6,448 and 77,376 each.

**⚠ So the only sound join is: decode the row's bytes, then name it.** A `def_id` lookup
against a live table silently mislabels ~74% of rows, and every mislabel is a *plausible
mnemonic* rather than an error — which is why the wrong figure survived being published
and cited.

The controls that made this findable rather than a guess:

```
[pos]  the tally sums to 4,088,162 = the corpus's own stated count (full scan, not truncated)
[pos]  the corrected join covers 100.0% of rows across 210 mnemonics
[neg]  LOOP / ARPL / FNINIT -> 0 rows (correctly absent from an XMM corpus)
[neg]  MOVSXD -> 15,045 under the STALE join   ⟹ the tell that the index was wrong
```

The `MOVSXD` row is what a reader should look for: a mnemonic that cannot appear in a
phase-2 XMM corpus, appearing with rows. Under the corrected join it is 0.

**And the corrected predicate was itself planted against, because re-firing a figure
tests the CORPUS while only a plant tests the ARM.** The new join extracts
`stub[SLOT_OFF:]` and stripped trailing `0x90` padding — so its candidate defect is an
instruction whose own last byte is genuinely `0x90` (any `Vxmm,Wxmm,Imm` form with
`imm8 == 0x90`), which would decode short and yield yet another *plausible* mnemonic.

Firing the narrow version of that check returned 0, which is the flattering direction
and not sufficient. So the arm was re-run with the strip **removed entirely** —
`decode_insn` reads length from the front, so padding is harmless to it — and the two
joins compared:

```
364 unstripped rows decoded  ·  diff against the stripped join  ⟹  IDENTICAL
```

The strip changed no decode. That is a result about the arm rather than about the
corpus, and it is the check the first (wrong) figure never had.

## What this does not say

The 40 are verified at **link-1 only** — `.isa` formula against silicon, through
`interp.rs`. The C# `IlLower` transcription is verified by nothing; see
`DIFFERENTIAL-SCOPING.md`.

## The corpus is now stale in a NEW way: 34 defs became executable after it was cut

**Measured 2026-08-21**, and it corrects a claim in the paragraph below (*"no `.isa`
semantics have changed since"* was true when written and is now false).

The intrinsic-stub census made 34 previously-`(intrinsic ...)` defs declarative
(PMAX/PMIN, PABS, PMULLD, PCMPEQQ, PMOVZX, PSHUFB, PINSR/PEXTR, PALIGNR, the
register-count shifts, MOVSHDUP/MOVSLDUP, PTEST, PMULUDQ, PMADDWD, PACKSSDW, CRC32,
BSWAP-16). Two arms:

| arm | result |
|---|---|
| frozen p2 corpus (4,088,162 rows) | **ZERO rows** for any of the 34 |
| fresh 1/64-stride smoke (8,484 rows, 380 defs) | **all 28 mnemonics present, 884 rows** |

**The zero is structurally certain rather than a finding.** An `(intrinsic ...)` def makes
`lift_one` panic → `discover()` returns `None` → no row is emitted. That is *why* those defs
were in the track-fail census. So the frozen corpus could not have graded them, and the
walk confirming it was redundant.

**The fresh corpus is the load-bearing half: the encoder reached them all along.** So the
gap is corpus STALENESS, not encoder coverage — a distinction that matters because the
remedy is a re-capture (needs an x86 box) rather than encoder work (needs none).

**⚠ And the shape to name: `p2-GOLDEN` is honest about what it graded and SILENT about the
census work.** A green golden over a corpus that predates 34 defs says nothing about those
34 — zero comparisons produce zero diffs, which is the same failure as a def whose eval was
dropped at parse (day-54): the count stays clean because the population shrank.

**‡ The walk that measured this was DEAD on its first fire** and worth recording as a
method note: I derived the state-word count from a byte-size division (`1553/8`, not even an
integer — and I read past that) instead of reading `STATE_WORDS_X64 = 90` at `state.rs:44`.
It walked 25 of 4,088,162 rows and printed a clean ZERO that **agreed with my hypothesis**.
What separated a dead read from a measurement was a `[pos]` control: the corrected walk finds
364 distinct `def_id`s and its row count **matches the header exactly**.

## ‡ A working copy's mtime cannot date the artifact

Checked 2026-08-21 after a peer's finding that a cross-machine time comparison is a
JOIN with two clocks. My own version was one machine, two FILES:

| artifact | mtime | size |
|---|---|---|
| `~/.mantis/data/artifacts/xfusion-sweep/sweep_p2_GOLDEN.x64d.gz` | **2026-08-11** | 137 MB gz |
| `/tmp/p2g.x64d` (what the row-walk reads) | 2026-08-20 | 7.1 GB |

I read the second and nearly filed the doc's `2026-08-11` as stale by nine days. It
isn't: **both carry 4,088,162 rows**, and two separate generation runs would not
match to the row — so `/tmp/p2g.x64d` is the DECOMPRESSED golden and its mtime is
the DECOMPRESSION date. The doc's date is the archive's, which is the only one that
dates the artifact.

**⟹ So a corpus claim needs the ARCHIVE's mtime, not the working copy's** — and the
discriminating check is the row count, not either timestamp.

And the corpus is nine days stale. It remains a valid answer key for the rows it
contains (they were silicon-exact when generated, and no `.isa` semantics have changed
since — 0 `.isa` files touched in the vector-lowering arc). But a fresh sweep would
carry 3,328 more rows and its own def ids, so **any artifact keyed on `def_id` needs
regenerating alongside it**, and the frozen file is the reason a stale-index join was
possible at all.
