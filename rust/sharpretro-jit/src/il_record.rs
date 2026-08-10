//! Structured IL recorder — a `Builder` impl that records each call as an `IlOp`
//! with SSA-id inputs/output. This is tier-1's step-1 (record → allocate → emit):
//!   1. `lift_one` into `IlRecorder` → `Vec<IlOp>` (each Val = u32 SSA-id, single-assignment)
//!   2. Compute live-ranges: `[def_at, last_use_at]` per SSA-id (trivial scan)
//!   3. Linear-scan allocate: assign host-regs, spill when >N live
//!   4. Emit: replay the ops with allocated `{Reg|Spill}` instead of tier-0's always-spill
//!
//! Distinct from `RecordingBuilder` (recording.rs), which records to TEXT for the
//! rung-4b golden-diff instrument. This records STRUCTURE for the allocator.

use crate::{Builder, IlType, RegFile, LocalId, NativeSlot, IntrinsicId, RoundMode};
use std::collections::{HashMap, HashSet};

/// One recorded IL operation. `out` = the SSA-id this op produces (None for stmts
/// like reg_write/mem_write/branch). `args` = SSA-id inputs. `ty` = result type
/// (or the operand type for stmts).
#[derive(Debug, Clone)]
pub struct IlOp {
    pub kind: IlOpKind,
    pub out: Option<u32>,
    pub args: [u32; 3],   // most ops take ≤2; ternary takes 3. args[i]=u32::MAX = unused.
    pub n_args: u8,
    pub ty: IlType,
    /// Extra payload (bits for literal, RegFile+idx for reg_read/write, etc).
    pub imm: u128,
}

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum IlOpKind {
    // producers (out = Some)
    Literal, RegRead, MemRead,
    Add, Sub, Mul, Div, Rem, Neg,
    And, Or, Xor, Not, Shl, Shr, Rotr, Rbit, Clz,
    Eq, Ne, Lt, Le, Gt, Ge,
    Cast, Sext, Ternary,
    CallIntrinsic,       // ‡ arity>3 not modeled yet — args[0..3] + intrinsic-id in imm
    // stmts (out = None)
    RegWrite, MemWrite, Branch, BranchLink,
    // control (cond)
    CondBegin, CondElse, CondEnd,   // bd.cond(c, then, else) → CondBegin(c) .. CondElse .. CondEnd
    // markers
    Unimplemented,
}

const NA: u32 = u32::MAX;

#[derive(Default)]
pub struct IlRecorder {
    pub ops: Vec<IlOp>,
    next: u32,
    branched: bool,
    tys: Vec<IlType>,
    /// State-value-numbering cache: (regfile, idx) → (SSA val, ty). reg_read
    /// returns the cached val on exact-ty hit (no RegRead op emitted); reg_write
    /// updates it (write-forwarding: a subsequent read sees the just-written val
    /// directly, no state[] round-trip). Cleared at CondBegin (v1-conservative:
    /// vals written inside a cond-arm aren't valid after; proper phi-merge = v2).
    /// The LCG block: r8/r9 written once (mov imm), read 4× each = 8 state-ldr
    /// eliminated by write-forwarding alone; rdi/rax write-then-read chains also
    /// forward. env XF_SVN=0 disables (for A/B measurement).
    reg_cache: HashMap<(u8, u32), (u32, IlType)>,
    /// Snapshot stack for cond: (cache-at-CondBegin, regs-written-during-this-cond).
    /// CondBegin: push (clone, ∅). reg_write inside: mark written. CondElse:
    /// restore cache to snapshot (else-arm sees pre-cond, not then's writes).
    /// CondEnd: pop; cache = snapshot MINUS written-set (any reg written in
    /// either arm is stale post-cond; unwritten regs' pre-cond val is still
    /// valid). SHR-with-dead-flags: empty body → written=∅ → cache preserved.
    cond_snap: Vec<(HashMap<(u8,u32),(u32,IlType)>, HashSet<(u8,u32)>)>,
    svn: bool,
}

impl IlRecorder {
    pub fn new() -> Self {
        let mut r = Self::default();
        r.svn = std::env::var("XF_SVN").map(|v| v != "0").unwrap_or(true);
        r
    }
    pub fn reset(&mut self) {
        self.ops.clear(); self.next = 0; self.branched = false;
        self.tys.clear(); self.reg_cache.clear();
    }
    pub fn branched(&self) -> bool { self.branched }
    pub fn n_vals(&self) -> u32 { self.next }

    fn produce(&mut self, kind: IlOpKind, ty: IlType, args: &[u32], imm: u128) -> u32 {
        let out = self.next; self.next += 1;
        self.tys.push(ty);
        let mut a = [NA; 3];
        for (i, &v) in args.iter().enumerate() { a[i] = v; }
        self.ops.push(IlOp { kind, out: Some(out), args: a, n_args: args.len() as u8, ty, imm });
        out
    }
    /// Public marker-emit for wrappers that can't delegate cond/loop_n
    /// (Tier1 forwards Builder to IlRecorder, but its cond closures take
    /// &mut Tier1 — so Tier1 emits the markers directly and runs closures on self).
    pub fn stmt_marker(&mut self, kind: IlOpKind, ty: IlType, args: &[u32], imm: u128) {
        // Tier1's cond forwards via stmt_marker (not rec.cond) — mirror the SVN
        // snapshot/restore so both paths preserve cache through empty conds.
        if self.svn { match kind {
            IlOpKind::CondBegin => {
                self.cond_snap.push((self.reg_cache.clone(), HashSet::new()));
            }
            IlOpKind::CondElse => {
                self.reg_cache = self.cond_snap.last().unwrap().0.clone();
            }
            IlOpKind::CondEnd => {
                let (snap, dirty) = self.cond_snap.pop().unwrap();
                self.reg_cache = snap;
                for k in &dirty { self.reg_cache.remove(k); }
                if let Some((_, outer)) = self.cond_snap.last_mut() { outer.extend(dirty); }
            }
            _ => {}
        }}
        self.stmt(kind, ty, args, imm);
    }
    fn stmt(&mut self, kind: IlOpKind, ty: IlType, args: &[u32], imm: u128) {
        let mut a = [NA; 3];
        for (i, &v) in args.iter().enumerate() { a[i] = v; }
        self.ops.push(IlOp { kind, out: None, args: a, n_args: args.len() as u8, ty, imm });
    }

    /// Compute `[def_at, last_use_at]` per SSA-id. `def_at[v]` = op-index where v is
    /// produced; `last_use[v]` = highest op-index where v appears as an arg (or def_at
    /// if never used — dead value, but still needs a reg for its def).
    /// This is the linear-scan input.
    pub fn live_ranges(&self) -> (Vec<usize>, Vec<usize>) {
        let n = self.next as usize;
        let mut def_at = vec![usize::MAX; n];
        let mut last_use = vec![0usize; n];
        for (i, op) in self.ops.iter().enumerate() {
            if let Some(out) = op.out { def_at[out as usize] = i; last_use[out as usize] = i; }
            for k in 0..op.n_args as usize {
                let a = op.args[k] as usize;
                if a < n { last_use[a] = last_use[a].max(i); }
            }
        }
        (def_at, last_use)
    }
}

macro_rules! bin { ($n:ident, $k:ident) => {
    fn $n(&mut self, a: u32, b: u32) -> u32 {
        let ty = self.tys[a as usize];
        self.produce(IlOpKind::$k, ty, &[a, b], 0)
    }
}; }
macro_rules! cmp { ($n:ident, $k:ident) => {
    fn $n(&mut self, a: u32, b: u32) -> u32 {
        self.produce(IlOpKind::$k, IlType::Bool, &[a, b], 0)
    }
}; }
macro_rules! un { ($n:ident, $k:ident) => {
    fn $n(&mut self, a: u32) -> u32 {
        let ty = self.tys[a as usize];
        self.produce(IlOpKind::$k, ty, &[a], 0)
    }
}; }

impl Builder for IlRecorder {
    type Val = u32;

    fn ty_of(&self, v: u32) -> IlType { self.tys[v as usize] }

    fn literal(&mut self, ty: IlType, bits: u128) -> u32 {
        self.produce(IlOpKind::Literal, ty, &[], bits)
    }
    fn reg_read(&mut self, f: RegFile, idx: u32, ty: IlType) -> u32 {
        // SVN: cache hit (exact ty) → return cached val, NO op emitted.
        // v1: exact-ty match only. A U64-cached rax read as U8 (al) misses —
        //   could emit cast(cached, U8) instead of a state-ldr, but that needs
        //   knowing the arch's partial-read semantics (x86: low bits; ok). v2.
        // Flag-file (per-bit reads) doesn't participate — the flag-RMW compaction
        //   is a separate v1.2; caching individual bits here would be correct but
        //   the RegWrite-flag-RMW is where the cost lives.
        if self.svn && f.0 == 0 {   // GPR file only v1
            if let Some(&(cv, cty)) = self.reg_cache.get(&(f.0, idx)) {
                if cty == ty { return cv; }
            }
        }
        let v = self.produce(IlOpKind::RegRead, ty, &[], ((f.0 as u128) << 32) | idx as u128);
        if self.svn && f.0 == 0 { self.reg_cache.insert((f.0, idx), (v, ty)); }
        v
    }
    fn reg_write(&mut self, f: RegFile, idx: u32, v: u32) {
        let ty = self.tys[v as usize];
        self.stmt(IlOpKind::RegWrite, ty, &[v], ((f.0 as u128) << 32) | idx as u128);
        // Write-forward: subsequent reads of (f,idx) see v directly.
        // ‡ partial-width writes: for x86 GPR, write_operand upstream does the
        //   read-mask-insert for width<32 and passes the merged-to-full-width
        //   result here, so v is authoritative for the full reg. width=32 zeroes
        //   upper (also upstream). So caching (v, ty) is safe — but a subsequent
        //   read at DIFFERENT ty misses (v1 exact-match), which is correct-if-wasteful.
        if self.svn && f.0 == 0 {
            self.reg_cache.insert((f.0, idx), (v, ty));
            // Inside a cond: mark (f,idx) written so CondEnd drops it.
            if let Some((_, dirty)) = self.cond_snap.last_mut() { dirty.insert((f.0, idx)); }
        }
    }
    fn mem_read(&mut self, a: u32, ty: IlType) -> u32 { self.produce(IlOpKind::MemRead, ty, &[a], 0) }
    fn mem_write(&mut self, a: u32, v: u32) {
        let ty = self.tys[v as usize];
        self.stmt(IlOpKind::MemWrite, ty, &[a, v], 0)
    }
    fn cast(&mut self, v: u32, ty: IlType) -> u32 { self.produce(IlOpKind::Cast, ty, &[v], 0) }
    fn sext(&mut self, v: u32, ty: IlType) -> u32 { self.produce(IlOpKind::Sext, ty, &[v], 0) }
    fn pair128(&mut self, hi: u32, lo: u32) -> u32 {
        self.produce(IlOpKind::Cast, IlType::I{signed:false,width:128}, &[hi, lo], 2 /*pair marker*/)
    }
    fn hi64(&mut self, a: u32) -> u32 { self.produce(IlOpKind::Cast, IlType::U64, &[a], 3 /*hi64 marker*/) }
    fn vfbin(&mut self, _: u32, _: u32, _: u32, _: u32) -> u32 { panic!("tier-1 v1: vfbin (V128)") }
    fn vshuf(&mut self, _: u32, _: u32, _: u32, _: u32) -> u32 { panic!("tier-1 v1: vshuf (V128)") }
    fn vcvt(&mut self, _: u32, _: u32) -> u32 { panic!("tier-1 v1: vcvt (V128)") }
    fn fcmpp(&mut self, _: u32, _: u32, _: u32, _: u32) -> u32 { panic!("tier-1 v1: fcmpp (float)") }
    fn fminmax(&mut self, _: u32, _: u32, _: bool) -> u32 { panic!("tier-1 v1: fminmax (float)") }
    fn vzip(&mut self, a: u32, b: u32, ew: u32, hi: bool) -> u32 {
        self.produce(IlOpKind::Ternary, IlType::V128, &[a, b],
            (ew as u128) | if hi { 1<<8 } else { 0 } | 5<<16 /*vzip marker*/)
    }
    fn loop_n(&mut self, n: u32, body: &mut dyn FnMut(&mut Self)) {
        // ‡ v1 tier-1: treat as opaque (emit body once, mark loop-N in imm). Real
        //   handling (allocator sees ranges cross the loop-back-edge) at v2.
        self.stmt(IlOpKind::CondBegin, IlType::U64, &[n], 4 /*loop_n marker*/);
        body(self);
        self.stmt(IlOpKind::CondEnd, IlType::Unit, &[], 4);
    }

    bin!(add, Add); bin!(sub, Sub); bin!(mul, Mul); bin!(div, Div); bin!(rem, Rem);
    bin!(and, And); bin!(or, Or); bin!(xor, Xor);
    bin!(shl, Shl); bin!(shr, Shr); bin!(rotr, Rotr);
    un!(neg, Neg); un!(not, Not); un!(rbit, Rbit); un!(clz, Clz);
    cmp!(eq, Eq); cmp!(ne, Ne); cmp!(lt, Lt); cmp!(le, Le); cmp!(gt, Gt); cmp!(ge, Ge);

    fn ternary(&mut self, c: u32, a: u32, b: u32) -> u32 {
        let ty = self.tys[a as usize];
        self.produce(IlOpKind::Ternary, ty, &[c, a, b], 0)
    }
    fn branch(&mut self, target: u32, link: bool) {
        self.stmt(if link { IlOpKind::BranchLink } else { IlOpKind::Branch },
                  IlType::U64, &[target], 0);
        self.branched = true;
    }
    fn cond(&mut self, c: u32,
            then_: &mut dyn FnMut(&mut Self), else_: &mut dyn FnMut(&mut Self)) {
        // Flatten cond into a linear sequence with markers. The allocator treats
        // args live across CondBegin..CondEnd conservatively (values defined inside
        // one arm aren't visible outside — the SSA already guarantees that from
        // lift.rs's structure: cond bodies write to state, don't produce Vals used
        // after). ‡ v1: no cross-arm value-merge (phi). If a template needs one,
        // it's via ternary(), not cond().
        // SVN: snapshot at CondBegin; restore at CondElse (else sees pre-cond,
        //   not then's writes); at CondEnd restore snapshot MINUS regs written
        //   in either arm. Cache survives EMPTY conds (SHR w/ dead-flags = 4×
        //   in the LCG loop) untouched.
        if self.svn { self.cond_snap.push((self.reg_cache.clone(), HashSet::new())); }
        self.stmt(IlOpKind::CondBegin, IlType::Bool, &[c], 0);
        then_(self);
        if self.svn { self.reg_cache = self.cond_snap.last().unwrap().0.clone(); }
        self.stmt(IlOpKind::CondElse, IlType::Unit, &[], 0);
        else_(self);
        self.stmt(IlOpKind::CondEnd, IlType::Unit, &[], 0);
        if self.svn {
            let (snap, dirty) = self.cond_snap.pop().unwrap();
            self.reg_cache = snap;
            for k in &dirty { self.reg_cache.remove(k); }
            // Nested cond: propagate dirty-set upward.
            if let Some((_, outer)) = self.cond_snap.last_mut() { outer.extend(dirty); }
        }
    }
    fn bitcast(&mut self, a: u32, ty: IlType) -> u32 { self.produce(IlOpKind::Cast, ty, &[a], 1 /*bitcast marker*/) }
    fn call_intrinsic(&mut self, id: IntrinsicId, args: &[u32]) -> Option<u32> {
        let a: Vec<u32> = args.iter().take(3).copied().collect();
        Some(self.produce(IlOpKind::CallIntrinsic, IlType::U64, &a, id.0 as u128))
    }
    fn unimplemented(&mut self, _s: &'static str) {
        self.stmt(IlOpKind::Unimplemented, IlType::Unit, &[], 0);
    }

    // ── float/vec/local/native — panic for now (tier-1 v1 = scalar-int only,
    //    matching tier-0's coverage; the fuzz gate covers exactly this set) ──
    fn fabs(&mut self, _: u32) -> u32 { panic!("tier-1 v1: float") }
    fn fsqrt(&mut self, _: u32) -> u32 { panic!("tier-1 v1: float") }
    fn fround(&mut self, _: u32, _: RoundMode) -> u32 { panic!("tier-1 v1: float") }
    fn fceil(&mut self, _: u32) -> u32 { panic!("tier-1 v1: float") }
    fn ffloor(&mut self, _: u32) -> u32 { panic!("tier-1 v1: float") }
    fn fisnan(&mut self, _: u32) -> u32 { panic!("tier-1 v1: float") }
    fn velement_read(&mut self, _: u32, _: u32, _: IlType) -> u32 { panic!("tier-1 v1: vec") }
    fn velement_write(&mut self, _: u32, _: u32, _: u32) -> u32 { panic!("tier-1 v1: vec") }
    fn vzero_top(&mut self, _: u32) -> u32 { panic!("tier-1 v1: vec") }
    fn local_new(&mut self, _: IlType) -> LocalId { panic!("tier-1 v1: local") }
    fn local_read(&mut self, _: LocalId) -> u32 { panic!("tier-1 v1: local") }
    fn local_write(&mut self, _: LocalId, _: u32) { panic!("tier-1 v1: local") }
    fn call_native(&mut self, _: NativeSlot, _: &[u32]) -> Option<u32> { panic!("tier-1 v1: call_native") }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn record_add_shape() {
        // v0 = reg_read GPR[0]; v1 = reg_read GPR[3]; v2 = add v0 v1; reg_write GPR[0] v2
        let mut r = IlRecorder::new();
        let a = r.reg_read(RegFile(0), 0, IlType::U64);
        let b = r.reg_read(RegFile(0), 3, IlType::U64);
        let s = r.add(a, b);
        r.reg_write(RegFile(0), 0, s);
        assert_eq!(r.ops.len(), 4);
        assert_eq!(r.n_vals(), 3);
        let (def_at, last_use) = r.live_ranges();
        assert_eq!(def_at, vec![0, 1, 2]);
        assert_eq!(last_use, vec![2, 2, 3]);   // v0,v1 last-used at op[2](add); v2 at op[3](write)
    }
}
