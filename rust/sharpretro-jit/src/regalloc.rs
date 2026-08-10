//! Linear-scan register allocator (Poletto & Sarkar '99) over `IlRecorder`'s
//! SSA live-ranges. Tier-1 step-2 (record → **allocate** → emit).
//!
//! Input: `(def_at[], last_use[])` per SSA-id from `IlRecorder::live_ranges()`.
//! Output: `Loc[]` mapping each SSA-id to `Reg(host_reg)` or `Spill(slot)`.
//!
//! The allocator is deliberately simple (linear-scan v1, no splitting, no
//! pre-coloring): SSA-ids are already in def-order (allocated as `next++` in
//! IlRecorder), so no interval-sort needed. Walk ids 0..N: expire everything
//! whose last_use < this def_at (free their regs); assign a free reg if one
//! exists, else spill THIS interval (spilling the furthest-last_use active
//! interval instead is the textbook improvement — v1 spills current since
//! measured max-alive on real blocks is typically ≤ n_regs anyway).
//!
//! Correctness constraints the emitter must honor:
//!   - A Reg-assigned val's host-reg is exclusive for [def_at, last_use].
//!   - A Spill-assigned val gets a spill-slot; the emitter loads to a scratch
//!     reg at each use, stores at def (same as tier-0 does for everything).
//!   - args[i] with last_use == this-op-index → their reg becomes free
//!     IMMEDIATELY AFTER this op (the emitter can reuse it for `out` — but v1
//!     doesn't; v2 does the "prefer arg's dying reg for out" hint).

use crate::il_record::IlRecorder;

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum Loc {
    /// Host register (aarch64 x0-x30; the emitter maps this index into its
    /// allocatable set, e.g. idx 0 → x13, idx 1 → x14, …).
    Reg(u8),
    /// Spill slot (word-index into the spill area, same as tier-0's slot).
    Spill(u16),
    /// Never assigned (dead value with span=0 that nothing reads — a `let`
    /// binding whose body was gated-off by dead-flag-elim). The emitter
    /// SKIPS the producing op entirely (DCE-at-alloc).
    Dead,
}

pub struct AllocResult {
    pub locs: Vec<Loc>,
    pub n_spilled: usize,
    pub n_dead: usize,
    pub max_alive: usize,
    pub n_spill_slots: u16,
}

/// Linear-scan allocate over `n_regs` allocatable host registers.
pub fn linear_scan(rec: &IlRecorder, n_regs: usize) -> AllocResult {
    let (def_at, last_use) = rec.live_ranges();
    let n = rec.n_vals() as usize;
    let mut locs = vec![Loc::Dead; n];

    // active[reg_idx] = Some(ssa_id) currently holding this reg.
    let mut active: Vec<Option<u32>> = vec![None; n_regs];
    let mut next_spill: u16 = 0;
    let (mut n_spilled, mut n_dead, mut max_alive) = (0, 0, 0);

    for v in 0..n {
        let (d, u) = (def_at[v], last_use[v]);
        // Dead value (span=0 AND nothing reads it): skip. Note span=0 alone
        // isn't sufficient — a val defined at op i and used at op i (rare but
        // possible if an op reads its own output, which SSA forbids) would be
        // span=0 but live. In practice last_use[v] > def_at[v] iff v is used;
        // last_use[v] == def_at[v] means only the def touched it → dead.
        if u == d {
            // Actually: the def itself sets last_use=def_at. If NOTHING reads it,
            // last_use stays there. So u==d ⟹ never-read ⟹ dead.
            locs[v] = Loc::Dead;
            n_dead += 1;
            continue;
        }
        // Expire: free any reg whose holder's last_use < d (strictly-before —
        // a val whose last_use == d is used AT the op that defines v, so it
        // must still be in a reg through that op; free it after).
        // Actually for SSA-id v, def_at[v] = the op-index where v is PRODUCED.
        // An arg a with last_use[a] == def_at[v] is consumed BY the op that
        // produces v → a's reg is needed as an INPUT to that op → can't
        // reuse for v's OUTPUT until after the op. So expire uses `< d` here,
        // and the "reuse dying-arg's reg for out" is an emitter-time hint (v2).
        for r in 0..n_regs {
            if let Some(h) = active[r] {
                if last_use[h as usize] < d { active[r] = None; }
            }
        }
        // Track max-alive (diagnostic — matches tier1_scope's number).
        let alive_now = active.iter().filter(|x| x.is_some()).count() + 1;
        max_alive = max_alive.max(alive_now);
        // Assign a free reg, else spill.
        if let Some(r) = active.iter().position(|x| x.is_none()) {
            active[r] = Some(v as u32);
            locs[v] = Loc::Reg(r as u8);
        } else {
            // v1: spill THIS interval. v2: spill the active interval with the
            // furthest last_use (Belady) and give v its reg — better when v
            // is short-lived and the spilled one is long-lived.
            locs[v] = Loc::Spill(next_spill);
            next_spill += 1;
            n_spilled += 1;
        }
    }

    AllocResult { locs, n_spilled, n_dead, max_alive, n_spill_slots: next_spill }
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::{Builder, IlType, RegFile};

    #[test]
    fn alloc_simple_add() {
        // v0=reg_read; v1=reg_read; v2=add v0 v1; reg_write v2
        // ranges: v0[0,2] v1[1,2] v2[2,3]. max-alive=3 (at op 2: v0,v1 in, v2 out).
        // With 3 regs: all Reg. With 2 regs: v2 spills? No — at op 2, v0/v1
        // consumed; but expire uses `< d`, and last_use[v0]=last_use[v1]=2=d,
        // so neither expires before v2's def → 2 regs full → v2 spills.
        // (This is the "reuse dying-arg reg" v2 improvement's exact case.)
        let mut r = IlRecorder::new();
        let a = r.reg_read(RegFile(0), 0, IlType::U64);
        let b = r.reg_read(RegFile(0), 3, IlType::U64);
        let s = r.add(a, b);
        r.reg_write(RegFile(0), 0, s);

        let a3 = linear_scan(&r, 3);
        assert_eq!(a3.n_spilled, 0);
        assert_eq!(a3.max_alive, 3);
        assert!(matches!(a3.locs[0], Loc::Reg(_)));
        assert!(matches!(a3.locs[2], Loc::Reg(_)));

        let a2 = linear_scan(&r, 2);
        assert_eq!(a2.n_spilled, 1);   // v2 spills (v1 case)
        assert!(matches!(a2.locs[2], Loc::Spill(0)));
    }

    #[test]
    fn alloc_dead_val() {
        // v0=literal; v1=literal; reg_write v1  → v0 never read → Dead.
        let mut r = IlRecorder::new();
        let _dead = r.literal(IlType::U64, 42);
        let live = r.literal(IlType::U64, 7);
        r.reg_write(RegFile(0), 0, live);
        let a = linear_scan(&r, 4);
        assert_eq!(a.n_dead, 1);
        assert_eq!(a.locs[0], Loc::Dead);
        assert!(matches!(a.locs[1], Loc::Reg(_)));
    }
}
