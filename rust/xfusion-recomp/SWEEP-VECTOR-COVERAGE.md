# Vector-family coverage in the p2 silicon corpus

Measured 2026-08-20 by tallying `def_id` directly out of `sweep_p2_GOLDEN.x64d`
(4,088,162 rows, the corpus that fired 0-diff on bare Intel), then joining against
`DEF_MNEMONICS` in `src/disassembler.rs`. Not read off `--census`: **both** of that
command's per-mnemonic lists truncate (`take(20)` for track-fail attribution at
`src/bin/sweep.rs:687`, `take(15)` for emitted rows at `:699`), and a truncated list
reads exactly like an absent entry.

## The number

**15 of 38** mnemonics served by the ten vector heads appear in the corpus.

| head | in silicon | rows | absent |
|---|---|---|---|
| vfminmax | 4/4 | 22,672 | — |
| vcvt | 5/8 | 9,980 | CVTDQ2PD, CVTTPD2DQ, CVTPD2DQ |
| vzip | 3/6 | 19,344 | PUNPCKHBW, UNPCKLPS, UNPCKHPS |
| vmovmsk | 2/3 | 30,090 | PMOVMSKB |
| vibin-mask | 1/6 | 6,448 | PCMPEQB/W/D, PCMPGTB/W/D |
| vhadd | 0/2 | 0 | HADDPS, HADDPD |
| vdpp | 0/2 | 0 | DPPS, DPPD |
| vshuf | 0/3 | 0 | SHUFPS, SHUFPD, PSHUFD |
| vshufw | 0/2 | 0 | PSHUFLW, PSHUFHW |
| vfcmpp | 0/2 | 0 | CMPPS, CMPPD |

`[pos]` ADD: 9 def_ids, 81,396 rows — the needle is live at this corpus.
`[neg]` a fabricated mnemonic: 0 def_ids.

## What this verifies, and what it does not

The corpus grades **interp.rs against silicon**. So for those 15, a wrong formula in
the .isa or a wrong Rust implementation of it would have shown as a diff, and did not.

It says nothing about the C# `IlLower` arm. That is a hand transcription *from*
interp.rs, so its errors are invisible here by construction: the sweep never builds,
runs, or reads the C# side. Two links, and only one is under an oracle:

```
.isa formula -> interp.rs    CO-BLIND (both derive from the one .isa)
                             => needs silicon, and has it for these 15
interp.rs    -> IlLower.cs   HAND-TRANSCRIBED
                             => interp.rs is itself an independent oracle for it,
                                and nothing currently compares them
```

## Open: eligible but not emitted

`phase1_skip` admits all ten families with zero skips (Vxmm/Wxmm/Uxmm occupy the same
ModRM fields as Greg/Erm — `src/sweep.rs:138-141`), and the absent 23 appear in
neither the emitted tally nor the track-fail list, nor in `DEF_IS_INTRINSIC`. So a
third gate excludes them and it is not yet named. Until it is, "eligible" is not
"covered": eligibility is a predicate on a def, and coverage is a row in a file, and
this measurement is the only one of the two that reads the file.
