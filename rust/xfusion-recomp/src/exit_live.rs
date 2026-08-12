//! Exit-liveness via successor-peek: which EFLAGS bits are actually LIVE at a
//! block's exit, computed by decoding a few instructions at each const branch
//! target. Flags every successor OVERWRITES before reading are DEAD at our
//! exit → the block's last flag-writers get liveness-stripped (the same
//! live_flags gating dead-flag-elim uses within a block, extended across the
//! edge). The remaining branchbench tax was exactly this: TEST's 4 flag-chains
//! + ADD's AF materialized at every block exit for successors that begin with
//! IMUL/ADD/TEST (all-overwriters).
//!
//! SOUNDNESS CONTRACT (same one FEX's deferred-flags accept): state[eflags]
//! is stale wherever a successor provably overwrites before reading. Anything
//! that OBSERVES flags outside the decoded def-universe must not exist on the
//! path — and it can't: an UNDECODED successor insn or one past the peek
//! horizon seeds ALL-LIVE (conservative stop). PUSHF/LAHF aren't in the def
//! set at all (they'd fail decode → block-stop → ALL-LIVE seed at that edge).
//! Signal handlers / debugger reads of state[eflags] see stale bits — the
//! explicit tradeoff, env-gated (XF_EXITLIVE=0 restores ALL-LIVE).
//!
//! Depth: peek up to PEEK_INSNS insns per target, following NO edges (a
//! conditional successor's own branch ends the walk with the residual live
//! mask folded in as-if-read — i.e. whatever we haven't resolved stays live).

use crate::decode::XMode;
use crate::disassembler::decode_insn;
use crate::lift::{DEF_FLAGS_MASK, DEF_FLAGS_READ, FLAGS_ALL_LIVE};
use crate::disassembler::DEF_MNEMONICS;  // (decode_insn imported above)

const PEEK_INSNS: usize = 12;

/// Liveness at entry of the code at `pc`: walk forward; a flag is dead once
/// written, live once read; anything unresolved at the walk's end stays LIVE.
///
/// # Safety
/// `pc` must be readable guest memory (identity-mapped). Caller guarantees
/// this the same way the block compiler does for its own decode.
pub unsafe fn entry_liveness(pc: u64, mode: XMode) -> u32 {
    entry_liveness_at(0, pc, mode)
}

/// entry_liveness with a host-base (guest bytes at `base + pc`; base=0 =
/// identity-map, the bench form).
pub unsafe fn entry_liveness_at(base: u64, pc: u64, mode: XMode) -> u32 {
    let mut resolved_dead = 0u32;   // bits proven overwritten-before-read
    let mut resolved_read = 0u32;   // bits proven read first
    let mut cur = pc;
    for _ in 0..PEEK_INSNS {
        let bytes = std::slice::from_raw_parts((base + cur) as *const u8, 15);
        if bytes[0] == 0xCC { break; }  // stop-insn: nothing beyond reads flags
        let d = match decode_insn(bytes, mode) { Some(d) => d, None => return FLAGS_ALL_LIVE };
        let did = d.def_id as usize;
        let rd = DEF_FLAGS_READ.get(did).copied().unwrap_or(FLAGS_ALL_LIVE);
        let wr = DEF_FLAGS_MASK.get(did).copied().unwrap_or(0);
        resolved_read |= rd & !resolved_dead;
        resolved_dead |= wr & !resolved_read;
        cur += d.len as u64;
        // A branch ends the linear walk: unresolved bits stay live (we don't
        // follow edges — one level of peek only). Jcc READS its flag, which
        // the DEF_FLAGS_READ row above already captured.
        let m = DEF_MNEMONICS.get(did).copied().unwrap_or("");
        if m.starts_with('j') || matches!(m, "call"|"ret"|"loop"|"loope"|"loopne"|"jecxz"|"jrcxz") { break; }
    }
    // live = read-first bits + everything never resolved.
    resolved_read | (FLAGS_ALL_LIVE & !resolved_dead)
}

/// Exit-liveness for a block whose FINAL decoded insn is `last` ending at
/// `next_pc`: union of entry_liveness over every const successor. Jcc = taken
/// (next_pc+imm0) ∪ fallthrough (next_pc); rel-JMP = one target; everything
/// else (RET / indirect JMP / CALL / undecoded) = ALL-LIVE. Rel-vs-indirect
/// discriminated by EXACT def_id (probed: 236=E9 rel32, 237=EB rel8,
/// 263=FF/4 indirect — decode-verified in-tree), not mnemonic strings.
/// CALL stays ALL-LIVE: the return path's flag-context isn't visible here.
///
/// # Safety
/// Successor pcs must be readable guest memory (identity-mapped), same
/// contract as `entry_liveness`.
pub unsafe fn block_exit_liveness(last_def_id: u32, imm0: i64, next_pc: u64, mode: XMode) -> u32 {
    block_exit_liveness_at(0, last_def_id, imm0, next_pc, mode)
}

/// block_exit_liveness with a host-base (see entry_liveness_at).
pub unsafe fn block_exit_liveness_at(base: u64, last_def_id: u32, imm0: i64, next_pc: u64, mode: XMode) -> u32 {
    const JMP_REL32: u32 = 236;
    const JMP_REL8: u32 = 237;
    if last_def_id == JMP_REL32 || last_def_id == JMP_REL8 {
        return entry_liveness_at(base, (next_pc as i64 + imm0) as u64, mode);
    }
    let m = DEF_MNEMONICS.get(last_def_id as usize).copied().unwrap_or("");
    // Jcc family: mnemonic starts with 'J' and it's not one of the JMP defs
    // (all Jcc are rel-only in x86 — no indirect Jcc exists, so this is safe).
    if m.starts_with('J') && !m.starts_with("JMP") {
        let t = entry_liveness_at(base, (next_pc as i64 + imm0) as u64, mode);
        let f = entry_liveness_at(base, next_pc, mode);
        return t | f;
    }
    FLAGS_ALL_LIVE
}
