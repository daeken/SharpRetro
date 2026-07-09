# M1 golden: ADD — hand-lowered target IL

> Width doctrine (barrow ·60 + the PF-shrink find): the consumer IL's IlConst
> carries explicit IlType — width is never node-inferred there. The walker's
> adopt-sibling-width is a LOWERING heuristic for arithmetic constants (masks,
> shift amounts); semantically-widthed constants (the 0x6996 u16 table) keep
> their own width and any narrowing must be an explicit cast. Shifts: result
> width = left operand width, always. If a future template diverges under the
> heuristic, the fix is per-.isa explicit width forms ((u16 0x6996)), not more
> heuristic.

The .isa semantics template (ia32-base.isa ADD) lowered by hand to the consumer
IL, for one concrete encoding each of reg-reg and mem-reg. Next session's
eval-body→Il walker is built against these as the acceptance rows (layer-3 test
form: the golden IS the test; the walker must reproduce it modulo tmp-numbering).

Conventions (per DESIGN.md M1 map + consumer answers ·44/·53):
- RegKind.Gpr64 = x86 64-bit GPR file by index (rax=0 … r15=15) — name TBD at
  shared-IL landing; `X86` prefix assumed below.
- RegKind.Eflags idx: CF=0 PF=2 AF=4 ZF=6 SF=7 OF=11 (bit positions in the
  architectural word; idx=-1 = whole word).
- Widths: `add eax, ebx` (01 D8, 32-bit op in 64-bit mode) writes the 32-bit
  result ZERO-EXTENDED to 64 (the x86-64 32-bit-write rule — NOT insert/extract;
  that's 8/16-bit writes only).

## Row 1: `01 D8` = add eax, ebx (mode=64)

```
(block
  (let %0 = (u32 trunc (u64 X86_RAX)))          ; _lval
  (let %1 = (u32 trunc (u64 X86_RBX)))          ; _rval
  (let %2 = (u32 add (u32 %0) (u32 %1)))        ; tval
  (X86_RAX := (u64 zext (u32 %2)))              ; 32-bit write zero-extends
  (EFLAGS.C := (u1 or (u1 ult (u32 %2) (u32 %0)) (u1 ult (u32 %2) (u32 %1))))
  (EFLAGS.S := (u1 trunc (u32 shr (u32 %2) (u32 #1f))))
  (EFLAGS.O := (u1 ne (u32 and (u32 xor (u32 %2) (u32 %0))
                                (u32 xor (u32 %2) (u32 %1))
                                (u32 #80000000)) (u32 #0)))
  (EFLAGS.A := (u1 trunc (u32 and (u32 shr (u32 xor (u32 %0) (u32 %1) (u32 %2)) (u32 #4)) (u32 #1))))
  (let %3 = (u8 trunc (u32 %2)))                ; low8
  (EFLAGS.P := (u1 trunc (u32 and (u32 shr (u32 #6996) (u8 and (u8 xor (u8 %3) (u8 shr (u8 %3) (u8 #4))) (u8 #f))) (u32 #1))))
  (EFLAGS.Z := (u1 eq (u32 %2) (u32 #0))))
```

Notes:
- `xor a b c` 3-arg in the .isa folds to nested IlBin(Xor, Xor(a,b), c).
- OF: the .isa writes `(& (^ tval _lval) (^ tval _rval) (<< 1 31))` = nonzero-
  test of the sign-bit AND; IL form compares against #0 to produce u1 (or
  extracts bit 31 — either canonicalization fine, pick ONE in the walker).
- SF: shr-31 then trunc-to-u1 (or Ne-#0; same canonicalization choice).

## Row 2: `01 5D F0` = add dword ptr [rbp-0x10], ebx (mode=64)

Same flag block; the lval is memory — decode-time addr expr, single-eval:

```
(block
  (let %addr = (u64 add (u64 X86_RBP) (u64 #fffffffffffffff0)))  ; base + sext disp
  (let %0 = (u32 load (u64 %addr)))             ; _lval
  (let %1 = (u32 trunc (u64 X86_RBX)))          ; _rval
  (let %2 = (u32 add (u32 %0) (u32 %1)))
  (store (u64 %addr) (u32 %2))                  ; write-back to SAME addr eval
  ... flag block identical to row 1 ...)
```

Notes:
- The addr expr is computed ONCE into %addr — load and store share it. x86
  semantics: the memory operand's address is evaluated once per instruction.
- disp -16 = sign-extended u64 constant (not a sub node — decode already
  produced the signed disp; the 4-tuple lowers to adds of consts).
- LOCK-prefixed form: same tree + an atomicity annotation — mechanism TBD at
  the table (IlNote vs a flag on IlStore vs intrinsic; NOT settled, don't
  build ahead of it).

## Non-goals pinned (so the walker doesn't over-build)

- AF: modeled (cheap), even though almost nothing reads it. If it costs
  interpreter speed later, drop behind a feature-flag — the .isa stays honest.
- Undefined-per-SDM flags (logic-op AF): NOT written, not even as IlNote.
  Census/qemu-diff will tell us if silicon-behavior modeling is ever needed.
- The `(mlet ...)` .isa form binds ALL its pairs before the body evaluates
  (Coppermine convention); the walker maps it to sequential IlLet — correct
  because .isa mlet pairs are already in dependency order by construction.
