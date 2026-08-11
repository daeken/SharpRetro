# SWEEP phase-3: mem-form

Spec: mem insns = fixed-page pre-write + post-capture + fault-addr-record.
The 32-bit sweep's real territory (32-bit addressing modes diverge from
64-bit: no RIP-rel, 67→16-bit-addr).

## Scope

Every Erm/Wxmm operand at mod≠11 (mem-form). ~364 defs × addressing-mode
space. The x86 special-index cases are the load-bearing sweep dimension:

| case | encoding | why special |
|---|---|---|
| `[reg]` | mod=00 rm=reg | baseline |
| `[reg+d8]` | mod=01 rm=reg disp8 | disp8 sign-extend |
| `[reg+d32]` | mod=10 rm=reg disp32 | |
| `[rsp]` / `[r12]` | mod=00 rm=4 SIB base=4 idx=4 | rm=4 ALWAYS needs SIB (idx=4=none) |
| `[rbp]` / `[r13]` | mod=01 rm=5 disp8=0 | mod=00 rm=5 = [rip+d32]/[d32], NOT [rbp] |
| `[rip+d32]` (64) | mod=00 rm=5 disp32 | 64-bit only; 32-bit = [d32] absolute |
| `[base+idx*s]` | mod=00 rm=4 SIB | scale∈{1,2,4,8}; idx=4=none |
| `[base+idx*s+d]` | mod=01/10 rm=4 SIB disp | full form |
| `[idx*s+d32]` | mod=00 rm=4 SIB base=5 | base=5 mod=00 = no-base (d32 absolute) |

## Design (encoder-first, per method)

**(a) Encoder** (`encode()` mem-arm): EncChoice gains
`{mem: Option<MemChoice{mod_, base, idx, scale, disp}>}`. When `mem.is_some()`
→ emit ModRM w/ mod≠11 + SIB (if rm=4) + disp. Round-trip via decode_insn's
existing ModRm.mem parsing (already works — decoder handles all mem forms).
`enumerate_p3()` walks the special-case table above × a few reg/idx values.
**Round-trip gate = the encoder oracle** (own-#165 lesson: + objdump spot).

**(b) Corpus + interp**: All mem-form addresses must resolve to ONE data
page at a **fixed low address** `DATA_PAGE = 0x60000` (below rsp=0x8FED8,
MAP_32BIT-reachable). Constraint: `effective_addr(base_val, idx_val, scale,
disp) ∈ [DATA_PAGE, DATA_PAGE+4096)`. Solve backward: pick target
`ea = DATA_PAGE + K` for K∈{0, 8, 0x80, 0xFF8}, pick idx_val + scale + disp
from a small grid, derive `base_val = ea − idx_val*scale − disp`. Pre-write
`page[K] = PRE_VALS_MEM[j]` (same 17-value grid). Post-capture whole page.
interp: FlatMem covers 0..0x90000 already; pre-write same bytes at DATA_PAGE.

**(c) Stub**: v3 stub = v1 + one `mov r14, DATA_PAGE` before the reg-loads
is NOT enough — the base/idx registers themselves must hold the derived
values (they're already loaded from state.gpr[]). So stub is UNCHANGED;
corpus-gen sets `pre.gpr[base_reg] = base_val`, `pre.gpr[idx_reg] = idx_val`.
The stub loads them naturally. **Runner** needs: (i) g_data_page at
DATA_PAGE via MAP_FIXED (parent writes pre-content per-triple, child
executes, parent reads post-content); (ii) row format gains
`pre_mem[N]`/`post_mem[N]` bytes; (iii) SIGSEGV handler records si_addr →
new state slot (the fault-addr-record deliverable).

**(d) Row format**: X64D v4 = header gains `mem_len:u16` (0=no-mem-arm =
back-compat); when >0, row carries `+mem_len pre-page bytes + mem_len
post-page bytes` after the state blocks. Runner: memcpy pre→g_data_page
before fork; memcmp post vs g_data_page after.

## Ordering (walls-ladder)

1. Encoder mem-arm + round-trip gate + objdump-spot (pure, testable now)
2. enumerate_p3 special-case table → census
3. Corpus row-format v4 + interp mem pre/post
4. Runner g_data_page MAP_FIXED + row-format v4 read
5. Fire smoke → decompose diffs
6. fault-addr (SIGSEGV si_addr) — separate arm, after 5 is clean

## Where 32-bit diverges

- mod=00 rm=5: [rip+d32] in 64-bit vs [d32] absolute in 32-bit
- 67 prefix: 32-bit-addr in 64-bit vs **16-bit-addr** in 32-bit (bx/bp/si/di
  combos, no SIB — a whole separate ModRM table)
- No r8-r15 as base/idx in 32-bit
