# XFusion design — x86-family .isa → generated decoder/interpreter/recompiler

Status: XF-1 sketch, for review. XF-0 (include/feature-flags/variadic-defm/comments) landed.

## The shape of the problem

x86 differs from every existing SharpRetro core in four ways:

1. **Variable length**: `prefix* + opcode[1..3] + ModRM? + SIB? + disp? + imm?`
   (1–15 bytes). sm83 is byte-oriented but per-def fixed-size; aarch64 is fixed-32.
2. **Redundant encodings**: one mnemonic ↔ many encodings (ADD has 6+ forms;
   `add r/m8, r8` and `add r8, r/m8` overlap for reg-reg). Semantics must be
   written ONCE per mnemonic, instantiated per encoding.
3. **Prefix-dependent width**: `Ev` means r/m16/32/64 depending on 0x66 / REX.W /
   mode. The same opcode byte is three instructions.
4. **Six architectural flags** (CF PF AF ZF SF OF) written per-instruction with
   per-instruction rules (ADD writes all six; MOV none; logic ops leave AF undefined).

## Layer split

### instruction / encoding (the Coppermine insight, rehomed)

```lisp
(instruction ADD (lval rval)
    "add $lval, $rval"
    (mlet (_lval lval _rval rval tval (+ _lval _rval))
        (= lval tval)
        (= CF (| (< tval _lval) (< tval _rval)))
        ...))

(encoding ADD (Eb Gb)  (0x00))
(encoding ADD (Ev Gv)  (0x01))
(encoding ADD (Gb Eb)  (0x02))
(encoding ADD (Gv Ev)  (0x03))
(encoding ADD (AL Ib)  (0x04))
(encoding ADD (rAX Iz) (0x05))
(encoding ADD (Eb Ib)  (0x80 /0))   ; ModRM.reg=0 selects the /0 row
(encoding ADD (Ev Iz)  (0x81 /0))
(encoding ADD (Ev Ib-sx) (0x83 /0))
```

NOT defm-based: MacroProcessor collects defms once, pre-expansion, with literal
names (MacroProcessor.cs:12-22) — a defm-emitting-defm with computed names can't
work there, and making it work buys nothing. Instead `instruction` and `encoding`
are **top-level forms XFusionDef.ParseAll collects directly**: instructions are
semantics templates (operand params + dasm + eval), encodings join to their
template by mnemonic and produce one XFusionDef each (template instantiated with
operand accessor expressions substituted for the params).

### Operand vocabulary = Intel SDM Appendix A notation

The encoding line's operand specs ARE the decode spec:

| spec | meaning | decode consequence |
|------|---------|--------------------|
| `Eb/Ev/Ew/...` | r/m, byte/prefix-sized/word/... | ModRM present; r/m side |
| `Gb/Gv/...` | general reg from ModRM.reg | ModRM present; reg side |
| `Ib/Iw/Iz/Iv` | immediate byte/word/z-sized(16or32)/v-sized | imm bytes follow |
| `Ib-sx` | imm8 sign-extended to v | 1 imm byte |
| `AL/rAX/CL/...` | fixed register | no bytes |
| `/0../7` | opcode extension | ModRM present; ModRM.reg must match |
| `Jb/Jz` | rel branch displacement | disp bytes follow |
| `M/Mp/...` | memory-only r/m | ModRM present; mod≠11 required |
| `Sw` | segment reg from ModRM.reg | ModRM present |
| (later) `Vx/Wx/Hx` | xmm reg / xmm r/m / vvvv | SSE/AVX tiers |

XFusionDef.Parse derives, per encoding: has-ModRM, imm size fn, disp size fn,
reg-extension constraint — mechanically, from the spec list. No hand-written
bit tables per instruction.

### Prefix scan is SHARED, not per-def

Generated decoder structure:

```
1. prefix loop: 0x66 0x67 0xF0 0xF2 0xF3, seg overrides, REX (64-bit mode, must
   be last) → PrefixState { opsize, adsize, rex.wrxb, seg, rep, lock }
2. opcode dispatch: 1-byte map / 0F map / 0F38 / 0F3A (map = which escape)
3. per-def tail: ModRM+SIB+disp decode (shared helper), imm fetch per spec,
   length = cursor position
```

Defs carry (map, opcode byte, /r constraint, operand specs); the mask/match
dispatch is over (map, opcode) — NOT raw byte positions like sm83, because
prefixes shift everything.

### Width genericity: expand at generation time

`(encoding ADD (Ev Gv) (0x01))` generates **three concrete defs** internally
(w=16/32/64 selected by decoded PrefixState), sharing one semantics template.
Types stay static per generated def — no runtime-width IL values, which keeps
the interpreter fast and the recompiler/lifter typed. The .isa never writes a
width-variant by hand. x86-16 flips the default (Ev → 16 default, 32 via 0x66)
via feature flags; the expansion machinery is identical.

### Flags → IL shape (the shared-IL constraint)

Pagentry's IL (Il.cs) has RegKind.Nzcv as a single flags word for aarch64.
x86 gets the same treatment: **eflags is ONE architectural register**
(RegKind extension on the Pagentry side), individual flags = bit
extract/insert expressions over it — same IL node vocabulary (IlBin shifts/
masks), zero new node types. The .isa exposes `CF/ZF/...` as expression forms
that compile to those bit ops, so semantics read naturally while the IL stays
arch-agnostic. Register file mapping: 16 GPRs by index (+ 8/16-bit partial
views = extract/insert over the containing register — REX changes AH/BH/CH/DH
to SPL/BPL/SIL/DIL, a decode-time detail), eflags word, rip, 6 segment regs,
xmm regs on the existing 128-bit V lane (YMM/ZMM = a later IL conversation,
flagged not designed).

Flag-semantics discipline: templates write flags explicitly (per sera's
Coppermine ADD — incl. the 0x6996-nibble PF trick, which is plain shift/xor/and
IL). No implicit flag magic in the generator. "Undefined per SDM" flags are
NOT written (documented per-insn); a test-tier decision later whether to model
silicon behavior for them (qemu diffs will surface it).

## M1 lowering map (Il.cs read 2026-07-09 — Pagentry.Lifter/Il.cs, NOT the
## MaxwellShader/Il.cs fork in this repo; they diverged at the ×149 split)

The consumer's IL: typed records, one insn → one IlBlock, SSA-ish via IlLet/
IlTmp, archregs by (RegKind, idx), flat memory, IlUnimpl as lifter self-‡.
Extension precedent exists (Maxwell added Gpr/Pred kinds + shader nodes).

x86 mapping, no new node SHAPES needed:

| .isa construct | IL |
|---|---|
| GPR read/write (rax..r15) | new RegKind (x86 64-bit file, 16 regs) + IlReadReg/IlWriteReg |
| partial reg (eax/ax/al/ah) | extract = IlCast.Trunc(IlReadReg); insert = IlWriteReg(masked IlBin.Or) — settled ·44 |
| CF/ZF/... | new RegKind.Eflags, SAME pattern as Nzcv (idx=-1 whole word, else bit#: CF=0 PF=2 AF=4 ZF=6 SF=7 DF=10 OF=11) |
| segment base (fs:/gs:) | RegKind.Sys read folded into addr expr — settled ·44 |
| mem operand | addr expr from (base index scale disp) 4-tuple; RIP-rel = IlReadPc-based; load/store = IlLoad/IlStore |
| (= lval expr) | IlWriteReg / IlStore by operand class |
| flags exprs (the 0x6996 PF trick) | IlBin shift/xor/and chains — plain |
| xmm | RegKind.V exists (128-bit); IlVecBin/IlVecElem lane ops fit SSE |
| ymm/zmm | ‡ IlType.Vec(128) is hardcoded in vector records — the ONE type-level gap; needs Vec(256/512) or lane-splitting. Flag for the table. |
| REP MOVSB etc | IlIntrin (exists! "the escape hatch" — barrow's ·44 intrinsic answer is already a node) |
| Jcc/JMP/CALL/RET | IlBranch(CondJmp/Jmp/Call/Ret) |
| CPUID/syscall | IlIntrin / IlSyscall |
| undefined-per-SDM flags | IlNote annotation or leave unwritten + document |

Repo-shape question (barrow ·51, sera call): shared IL lives where?
(a) SharpRetro→Pagentry dep (backwards), (b) IL type-tree moves INTO SharpRetro
as shared lib (barrow leans b; CoreArchCompiler is already the shared home),
(c) per-arch IL + normalize at load. XFusion assumes (b)-or-equivalent.

## Lift wiring (designed 2026-07-09, implementation next session)

v1 = RUNTIME lowering: Pagentry lifts each insn once and caches db.fns[].il,
so per-lift walker cost is fine; generator-time specialization (parametric
trees with operand holes) is an optimization with its own risks — later.

Pieces:
1. IlLower.cs becomes a SHARED-SOURCE compile (<Compile Include>) into both
   XFusionGenerator and XFusionCpu — no project ref either direction (the
   generator must never depend on XFusionCpu: bootstrap rule; XFusionCpu
   depending on the generator drags codegen into the runtime). CoreArchCompiler
   ref from XFusionCpu is fine (runtime-safe lib, Damage precedent =
   DamageCore→LibSharpRetro).
2. Generated/LiftTables.cs: per-template {mnemonic, params[], eval S-expr}
   serialized back out by the generator; runtime parses once via ListParser.
3. THE FORK, decided: refactor the generated decoder to return a structured
   DecodedInsn {def-id, operand binds, length} and make BOTH disasm-string
   rendering AND lifting consumers of it — instead of generating a second
   parallel dispatch for lift (duplication rots) or bolting binds onto the
   string path. This churns the corpus-verified decoder ⟹ fresh-session arc
   with census re-verification at every step, not a session-tail change.
4. Then: Lift(bytes, pc, mode) → DecodedInsn → binds → IlLower → tree;
   renderer swaps text→real Il ctors when the shared-IL repo location lands.

## Test plan (3 layers, design review)

1. **compiler tests** — landed with XF-0 (19 green).
2. **decode tests** — per-encoding byte→(mnemonic, operands, length) rows,
   incl. redundant-encoding disambiguation and prefix interactions
   (0x66+REX.W precedence, REX-not-last invalidation, LOCK on non-lockable = #UD).
   Oracle: XED batch-diff on a glibc .so corpus (XF-2).
3. **semantics tests** — (pre-state, bytes) → post-state rows, format designed
   to be runnable by: generated interpreter (now), qemu-user diff (next),
   real hardware (sera's aarch64 harness precedent).

## VEX/EVEX tier (designed 2026-07-09, implementation next)

Corpus population (glibc x86-64, 10,503 remaining undecoded): VEX = AVX-promoted
forms of instructions we already define (vmovdqu 1208, vpmovmskb 1061, vpcmpeqb
1017, vpaddb 460...) + AVX-512 EVEX forms (vmovdqu64, kmovd, vpcmp* with mask
regs). Almost no *new* semantics — new *encoding* of existing rows.

Mechanism (auto-promotion, not hand-duplication):

1. **Prefix scanner** learns C5 (2-byte: [C5, R̄vvvvLpp]) and C4 (3-byte:
   [C4, R̄X̄B̄mmmmm, WvvvvLpp]) → PrefixState.Vex { Map (mmmmm: 1=0F/2=0F38/3=0F3A),
   Pp (0=none/1=66/2=F3/3=F2), Vvvv (inverted), L, W, and R/X/B folded into the
   REX-equivalent bits }. EVEX (62) same shape, 4-byte, +mask/z/L'/b fields.
   ⚠ 32-bit mode: C4/C5/62 are LES/LDS/BOUND unless next-byte mod==11 —
   lookahead required (64-bit unconditional).
2. **Dispatch**: VEX-encoded lookups hit the SAME (map, opcode) switch; Pp
   substitutes for the mandatory-prefix condition. Defs are vex-capable by
   default in vector space; a `vex` / `vex-only` / `novex` encoding token
   overrides (vzeroupper = vex-only; new defs like vpermd live in avx*.isa).
3. **Promotion rule**: a 2-operand SSE row (Vdq Wdq) under VEX gains vvvv as
   second source → renders 3-operand (vpaddb xmm1, xmm2, xmm3). MOV-class rows
   (movdqu/movaps...) do NOT gain vvvv (vvvv must be 1111 else #UD). The .isa
   marks which: `(vex 3src)` vs `(vex 2src)` on the encoding, or a promote-table
   in avx.isa keyed by mnemonic. L=1 renders ymm. W selects 64-bit lane forms.
4. **EVEX adds**: mask-reg operand ({k1}), zeroing ({z}), disp8*N compression
   (memory disp scales by element size — LENGTH-affecting, must be exact),
   opmask registers k0-k7 as a new reg file.

Sequencing: VEX first (C5 then C4), corpus-verify, then EVEX. kmovd/vpcmpb
(mask-reg forms) land with EVEX since that's where the corpus uses them.

## Ladder

ia32-base (~100 insns, one-byte map core) → verify M0 vs XED on corpus →
0F map basics (Jcc, SETcc, MOVZX/MOVSX, IMUL) → x86-64 (REX, new defaults) →
x86-16 (mode flip) → MMX/SSE tiers → cpuid.isa reports compiled feature set.
Walls-ladder discipline: census which missing opcode blocks the most corpus
bytes; fix worst; re-baseline.

## Open questions — SETTLED (consumer review, 2026-07-09)

- **eflags**: new `RegKind.Eflags` (RegKind is arch-extensible; overloading Nzcv
  would make cross-arch predicates lie). Bit-extract/insert over it.
- **segment bases**: `RegKind.Sys` reads (`FS_BASE` etc.), folded into addr exprs —
  same treatment as aarch64 TPIDR_EL0.
- **partial regs**: insert/extract over containing reg (unchallenged).
- **REP strings**: **intrinsic node** (`rep_movsb rdi rsi rcx`), NOT an IL loop —
  reversed from my proposal; the consumer's argument wins: the RCX/DF loop is reg-alloc
  noise that hides "this is memcpy" from the working tier; the intrinsic IS the
  semantic unit, and find/pattern-match keys on well-known names. aarch64 DC ZVA
  precedent.
