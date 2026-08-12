//! Tier-2 v1: TRACE compilation — "a trace is just a bigger block."
//!
//! Extends block compilation THROUGH control flow instead of stopping at it:
//!   - unconditional rel-JMP (def 236/237): follow the target, emit nothing
//!     (the recorder elides the branch; the next lifted insn IS the target).
//!   - Jcc: follow the FALLTHROUGH; the taken arm stays as a side-exit
//!     (cond{state-writes + Branch}, nothing in the other arm). ‡ v1 follows
//!     fallthrough only — no taken-arm traces / no profile input yet.
//!   - CALL/RET/indirect-JMP/int3: end the trace (ALL-LIVE exit rules apply).
//!   - BACKWARD rel-JMP or Jcc whose target is INSIDE the already-collected
//!     trace: end the trace at that insn (emit it normally — the linker turns
//!     the back-edge into a chain; a loop body thus becomes ONE trace whose
//!     final branch self-links).
//!
//! Soundness: reg_write always EMITS its store op (the SVN cache forwards
//! reads only), so guest state[] is fully materialized on every surviving
//! branch path — a side-exit sees exactly the state a block boundary would
//! have written. Flag-liveness at side-exits: the backward pass ORs the
//! side-exit target's entry_liveness into `live` at the Jcc position, so a
//! flag consumed only by the exit path still materializes before it.
//!
//! What makes this tier-2: the SVN/DSE caches persist across the former
//! block boundaries (a reg_read after a former-boundary hits the cache; a
//! dead store before it dies), and the linker's per-block dispatch cost for
//! interior edges drops to ZERO (they're not even chains — they're straight-
//! line code).

use sharpretro_jit::{Builder, IlType};
use crate::decode::{DecodedInsn, XMode};
use crate::disassembler::{decode_insn, DEF_MNEMONICS};
use crate::exit_live::{entry_liveness, block_exit_liveness};
use crate::lift::{lift_one, FLAGS_ALL_LIVE, DEF_FLAGS_MASK, DEF_FLAGS_READ};

const JMP_REL32: u32 = 236;
const JMP_REL8: u32 = 237;

fn is_jcc(def_id: u32) -> bool {
    let m = DEF_MNEMONICS.get(def_id as usize).copied().unwrap_or("");
    m.starts_with('J') && !m.starts_with("JMP")
}
fn is_jmp_rel(def_id: u32) -> bool { def_id == JMP_REL32 || def_id == JMP_REL8 }
fn is_trace_stop(def_id: u32) -> bool {
    // CALL/RET/indirect-JMP/LOOP*: end trace. (LOOP could follow-fallthrough
    // too, but it's rare + rcx-coupled; keep v1 simple.)
    let m = DEF_MNEMONICS.get(def_id as usize).copied().unwrap_or("");
    m == "CALL" || m.starts_with("RET") || m.starts_with("LOOP")
        || (m.starts_with("JMP") && !is_jmp_rel(def_id))
}

pub struct TraceInsn {
    pub d: DecodedInsn,
    pub pc: u64,
    pub next_pc: u64,
    /// This insn's fallthrough/target is followed by the trace (Jcc: fall-
    /// through followed, taken = side-exit; JMP-rel: target followed, no code).
    pub followed: Option<u64>,
}

/// Decode-collect a trace starting at `pc`. Follows rel-JMPs and Jcc fall-
/// throughs; stops at CALL/RET/indirect, int3, undecodable, max_insns, or a
/// branch targeting INSIDE the collected trace (back-edge → loop close).
/// Returns (insns, end_pc, stopped_at_branch).
pub unsafe fn collect_trace(pc: u64, mode: XMode, max_insns: usize,
                            fetch_ok: &dyn Fn(u64) -> bool)
    -> (Vec<TraceInsn>, u64, bool)
{
    let mut insns: Vec<TraceInsn> = vec![];
    let mut seen = std::collections::HashSet::new();
    let mut cur = pc;
    let mut branched = false;
    while insns.len() < max_insns {
        if !fetch_ok(cur) { break; }
        let bytes = std::slice::from_raw_parts(cur as *const u8, 15);
        if bytes[0] == 0xCC { break; }
        let Some(d) = decode_insn(bytes, mode) else { break };
        let next = cur + d.len as u64;
        let did = d.def_id;
        if is_jmp_rel(did) {
            let tgt = (next as i64 + d.imm0) as u64;
            // Back-edge (target inside trace) or self: emit as real branch,
            // end trace — the linker chains it (self-link for loops).
            if seen.contains(&tgt) || tgt == pc {
                insns.push(TraceInsn { d, pc: cur, next_pc: next, followed: None });
                branched = true;
                cur = next;
                break;
            }
            // Follow: the JMP itself emits nothing (recorder elides).
            insns.push(TraceInsn { d, pc: cur, next_pc: next, followed: Some(tgt) });
            seen.insert(cur);
            cur = tgt;
            continue;
        }
        if is_jcc(did) {
            let tgt = (next as i64 + d.imm0) as u64;
            // Taken-arm targeting inside the trace (incl. self-loop Jcc): stop
            // here and emit normally — cond{branch tgt}/else{branch next}; the
            // linker chains both arms.
            if seen.contains(&tgt) || tgt == pc || seen.contains(&next) {
                insns.push(TraceInsn { d, pc: cur, next_pc: next, followed: None });
                branched = true;
                cur = next;
                break;
            }
            // Follow fallthrough; taken = side-exit.
            insns.push(TraceInsn { d, pc: cur, next_pc: next, followed: Some(next) });
            seen.insert(cur);
            cur = next;
            continue;
        }
        if is_trace_stop(did) {
            insns.push(TraceInsn { d, pc: cur, next_pc: next, followed: None });
            branched = true;
            cur = next;
            break;
        }
        insns.push(TraceInsn { d, pc: cur, next_pc: next, followed: None });
        seen.insert(cur);
        cur = next;
    }
    (insns, cur, branched)
}

/// Backward flag-liveness over a collected trace. At a FOLLOWED Jcc, the
/// side-exit target's entry_liveness ORs in (a flag consumed only on the exit
/// path must materialize). Exit liveness at the trace end = the usual
/// block_exit_liveness rules (or ALL-LIVE when disabled/no-branch).
pub unsafe fn trace_liveness(insns: &[TraceInsn], mode: XMode, exitlive: bool) -> Vec<u32> {
    let n = insns.len();
    let mut per = vec![0u32; n];
    if n == 0 { return per; }
    let last = &insns[n - 1];
    let mut live = if exitlive {
        block_exit_liveness(last.d.def_id, last.d.imm0, last.next_pc, mode)
    } else { FLAGS_ALL_LIVE };
    for i in (0..n).rev() {
        let t = &insns[i];
        let did = t.d.def_id as usize;
        if t.followed.is_some() && is_jcc(t.d.def_id) {
            // Side-exit: its target's entry liveness joins the flow.
            let tgt = (t.next_pc as i64 + t.d.imm0) as u64;
            let exit_live = if exitlive { entry_liveness(tgt, mode) } else { FLAGS_ALL_LIVE };
            live |= exit_live;
        }
        per[i] = live;
        live = (live & !DEF_FLAGS_MASK.get(did).copied().unwrap_or(0))
             | DEF_FLAGS_READ.get(did).copied().unwrap_or(0);
    }
    per
}

/// Lift a collected trace into `b`. For followed insns, trace mode elides the
/// fallthrough/target branch (Builder::set_trace_next / trace_take_elided —
/// no-op defaults on non-tracing builders, so an elide-miss returns false and
/// the caller falls back to block-grain compilation).
pub fn lift_trace<B: Builder<Val = u32>>(
    b: &mut B, insns: &[TraceInsn], per: &[u32], mode: XMode) -> bool
{
    for (i, t) in insns.iter().enumerate() {
        b.set_trace_next(t.followed);
        lift_one(b, &t.d, t.pc, mode, per[i]);
        if t.followed.is_some() {
            if !b.trace_take_elided() { b.set_trace_next(None); return false; }
        }
    }
    b.set_trace_next(None);
    if !b.branched() {
        // Fell off the end (max_insns / int3 / undecodable at cur): branch to
        // the end pc so the driver resumes there.
        let end = insns.last().map(|t| t.next_pc).unwrap_or(0);
        let t = b.literal(IlType::U64, end as u128);
        b.branch(t, false);
    }
    true
}
