//! C1 atomics: `lock`-prefixed guest RMW (+ implicitly-locked XCHG-mem).
//!
//! Contract (SYNC-AUDIT-ATOMICS.md C1): the MEMORY side of the RMW happens
//! atomically with full-fence semantics via Builder::mem_rmw_atomic /
//! mem_cas_atomic (LSE `AL`-forms on tier-0/1, host atomics in the interp).
//! Then the UNMODIFIED generated template re-runs with the memory operand
//! replaced by `Operand::Val(OLD)`:
//!   - reads of the dst see OLD (exactly what a locked RMW's flag math uses),
//!   - the template's ALU recomputes new-value + ALL FLAGS exactly as the
//!     .isa says (zero transcription — own-#91/92 law),
//!   - the template's mem-write is discarded (Val write = no-op; the atomic
//!     already stored the new value).
//!
//! Per-mnemonic memory-op mapping (what the ATOMIC must store):
//!   ADD/XADD → fetch_add(src)         INC → fetch_add(1)
//!   SUB      → fetch_add(-src)        DEC → fetch_add(-1)
//!   AND      → fetch_and(src)         OR → fetch_or(src)   XOR → fetch_xor(src)
//!   XCHG     → swap(src)  (+ the template writes OLD to the reg side ✓)
//!   CMPXCHG  → cas(expected=RAX_al_ax_eax_rax, new=src); template re-derives
//!              ZF from OLD==RAX and the RAX/dst writes from the same OLD ✓
//!
//! ‡ SUB/DEC via two's-complement fetch_add is exact at width w (mod 2^w).
//! ‡ NOT in v1 (0 in the CP2077 census): ADC/SBB (need CF in the atomic),
//!   NEG/NOT (unary-store forms), BTS/BTR/BTC mem (bit-string ea), and
//!   CMPXCHG8B/16B (absent from corpus). The generator doesn't route them.

use sharpretro_jit::{Builder, IlType, RegFile};
use crate::operand::{Operand, read_operand, ilty};

const GPR: RegFile = RegFile(0);

/// Do the atomic memory side for `mnem` on `ops` (ops[0] MUST be Mem — the
/// generated guard checks); return the ops array with ops[0] replaced by
/// Operand::Val(OLD value). op widths: the dst operand's width.
pub fn atomic_pre<B: Builder>(bd: &mut B, mnem: &str, ops: &[Operand<B::Val>],
                              _op_w: u32) -> Vec<Operand<B::Val>>
    where B::Val: Copy
{
    // XF_NOATOMIC=1: NEGATIVE CONTROL — return ops unchanged (the template
    // does its plain ldr/op/str = pre-C1 behavior). The torture test MUST
    // FAIL under this (lost updates) or the test proves nothing.
    if std::env::var("XF_NOATOMIC").map(|v| v == "1").unwrap_or(false) {
        return ops.to_vec();
    }
    let (addr, w) = match ops[0] {
        Operand::Mem { addr, width } => (addr, width),
        _ => unreachable!("atomic_pre on non-mem dst"),
    };
    let ty = ilty(w);
    let old = match mnem {
        "ADD" | "XADD" => {
            let src = read_operand(bd, &ops[1]);
            bd.mem_rmw_atomic(0, addr, src, ty)
        }
        "SUB" => {
            let src = read_operand(bd, &ops[1]);
            let z = bd.literal(ty, 0);
            let neg = bd.sub(z, src);
            bd.mem_rmw_atomic(0, addr, neg, ty)
        }
        "INC" => { let one = bd.literal(ty, 1); bd.mem_rmw_atomic(0, addr, one, ty) }
        "DEC" => {
            let m1 = bd.literal(ty, (1u128 << w).wrapping_sub(1)); // -1 at width
            bd.mem_rmw_atomic(0, addr, m1, ty)
        }
        "AND" => {
            let src = read_operand(bd, &ops[1]);
            bd.mem_rmw_atomic(2, addr, src, ty)
        }
        "OR" => {
            let src = read_operand(bd, &ops[1]);
            bd.mem_rmw_atomic(1, addr, src, ty)
        }
        "XOR" => {
            let src = read_operand(bd, &ops[1]);
            bd.mem_rmw_atomic(3, addr, src, ty)
        }
        "XCHG" => {
            let src = read_operand(bd, &ops[1]);
            bd.mem_rmw_atomic(4, addr, src, ty)
        }
        "CMPXCHG" => {
            // expected = accumulator (al/ax/eax/rax at w), new = src reg.
            let rax = bd.reg_read(GPR, 0, IlType::U64);
            let expected = bd.cast(rax, ty);
            let newv = read_operand(bd, &ops[1]);
            bd.mem_cas_atomic(addr, expected, newv, ty)
        }
        m => panic!("atomic_pre: unmapped mnemonic {m}"),
    };
    let mut out: Vec<Operand<B::Val>> = Vec::with_capacity(ops.len());
    out.push(Operand::Val { v: old, width: w });
    out.extend(ops.iter().skip(1).copied());
    out
}
