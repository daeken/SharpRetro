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

## What this does not say

The 40 are verified at **link-1 only** — `.isa` formula against silicon, through
`interp.rs`. The C# `IlLower` transcription is verified by nothing; see
`DIFFERENTIAL-SCOPING.md`.

And the corpus is nine days stale. It remains a valid answer key for the rows it
contains (they were silicon-exact when generated, and no `.isa` semantics have changed
since — 0 `.isa` files touched in the vector-lowering arc). But a fresh sweep would
carry 3,328 more rows and its own def ids, so **any artifact keyed on `def_id` needs
regenerating alongside it**, and the frozen file is the reason a stale-index join was
possible at all.
