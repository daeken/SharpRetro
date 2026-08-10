//! Tier-1: register-allocated JIT. Records the block via IlRecorder (the
//! structured Builder impl → Vec<IlOp>), runs linear_scan (Poletto-Sarkar '99)
//! to assign each SSA val a host register or spill-slot, then WALKS the IlOp
//! sequence emitting per-op with args/out already in registers.
//!
//! v1 SCOPE: scalar-int only (u8-u64, Bool). Matches IlRecorder's own coverage
//! (float/vec/wide panic in the recorder). Cond blocks not yet handled (loop_n
//! and the CMPXCHG cond-arm route through — die-loud on CondBegin for v1).
//! Every gap panics NAMED (die-loud, per the no-fake-success discipline).
//!
//! Register model:
//!   x28 = state-base (from arg x0, same as tier-0)
//!   x27 = spill-base (from arg x1)
//!   x9-x12 = materialize scratch (loading a Spill arg / mov_imm64 for Literal)
//!   x13-x26 = allocatable pool (14 regs; Loc::Reg(i) → x(13+i))
//!   x19-x28 callee-saved → prologue saves x19-x28 (10 regs) + lr.

use crate::{Builder, IlType, RegFile};
use crate::aarch64_enc::{Aarch64Enc, Cond};
use crate::il_record::{IlRecorder, IlOp, IlOpKind};
use crate::regalloc::{linear_scan, Loc, AllocResult};
use crate::tier0::{StateLayout, CompiledBlock};

const X_STATE: u32 = 28;
const X_SPILL: u32 = 27;
const X_S0: u32 = 9;    // scratch for materializing Spill/Literal arg 0
const X_S1: u32 = 10;
const X_S2: u32 = 11;
const REG_BASE: u32 = 13;   // Loc::Reg(0) → x13, .. Reg(13) → x26.
pub const N_ALLOC_REGS: usize = 14;   // x13..x26

pub struct Tier1 {
    /// The recorder — client emits into this via the Builder trait, THEN
    /// calls compile() which allocates + walks + finalizes.
    pub rec: IlRecorder,
    layout: &'static StateLayout,
}

impl Tier1 {
    pub fn with_layout(layout: &'static StateLayout) -> Self {
        Self { rec: IlRecorder::new(), layout }
    }

    /// After the block's been emitted into `.rec` (via the Builder trait forward
    /// below), allocate + emit + finalize into a CompiledBlock.
    pub fn compile(self) -> CompiledBlock {
        // Dead-RegWrite elimination (v1.2): for each GPR (f=0,idx), a RegWrite
        // followed by a LATER RegWrite(same) with no state-observing RegRead
        // and no cond-boundary between = the earlier one is dead. Post-SVN,
        // write-forwarding eliminated intermediate RegReads, so this is common
        // (LCG: rax written 12×, rdi 9× — only the last-per-reg reaches exit).
        // v1-conservative: cond boundaries flush last_write (RegWrites inside
        // conds never dropped; a write BEFORE a cond isn't dropped either since
        // a post-cond read of a cond-dirtied reg hits state[]). XF_DSE=0 disables.
        let dse = std::env::var("XF_DSE").map(|v| v != "0").unwrap_or(true);
        let mut dead_ops: std::collections::HashSet<usize> = std::collections::HashSet::new();
        if dse {
            let mut cond_depth = 0i32;
            let mut last_write: std::collections::HashMap<(u8, u32), usize> =
                std::collections::HashMap::new();
            for (i, op) in self.rec.ops.iter().enumerate() {
                match op.kind {
                    IlOpKind::CondBegin => { cond_depth += 1; last_write.clear(); }
                    IlOpKind::CondEnd   => { cond_depth -= 1; last_write.clear(); }
                    IlOpKind::RegWrite if cond_depth == 0 => {
                        let f = (op.imm >> 32) as u8; let idx = op.imm as u32;
                        if f == 0 {   // GPR only v1 (flag-RMW compaction is separate)
                            if let Some(&prev) = last_write.get(&(f, idx)) {
                                dead_ops.insert(prev);
                            }
                            last_write.insert((f, idx), i);
                        }
                    }
                    IlOpKind::RegRead => {
                        // A surviving RegRead(f,idx) observes state[] — the last
                        // write to it is live (the read needs it). Post-SVN this
                        // only fires on cross-cond reads or ty-mismatch misses.
                        let f = (op.imm >> 32) as u8; let idx = op.imm as u32;
                        last_write.remove(&(f, idx));
                    }
                    _ => {}
                }
            }
        }
        let alloc = linear_scan(&self.rec, N_ALLOC_REGS);
        let mut e = Emitter::new(self.layout, &self.rec, &alloc);
        e.dead_ops = dead_ops;
        for (i, op) in self.rec.ops.iter().enumerate() {
            e.emit_op(i, op);
        }
        e.finalize(alloc.n_spill_slots as u32)
    }
}

// ── the Builder impl: forward everything to self.rec ───────────────────────
// Client code (lift.rs's tmpl_N / compile_block) emits via `Builder for Tier1`,
// which just delegates to IlRecorder. compile() does the actual codegen after.

impl Builder for Tier1 {
    type Val = u32;
    fn ty_of(&self, v: u32) -> IlType { self.rec.ty_of(v) }
    fn literal(&mut self, ty: IlType, bits: u128) -> u32 { self.rec.literal(ty, bits) }
    fn reg_read(&mut self, f: RegFile, idx: u32, ty: IlType) -> u32 { self.rec.reg_read(f, idx, ty) }
    fn reg_write(&mut self, f: RegFile, idx: u32, v: u32) { self.rec.reg_write(f, idx, v) }
    fn mem_read(&mut self, addr: u32, ty: IlType) -> u32 { self.rec.mem_read(addr, ty) }
    fn mem_write(&mut self, addr: u32, v: u32) { self.rec.mem_write(addr, v) }
    fn add(&mut self, a: u32, b: u32) -> u32 { self.rec.add(a, b) }
    fn sub(&mut self, a: u32, b: u32) -> u32 { self.rec.sub(a, b) }
    fn mul(&mut self, a: u32, b: u32) -> u32 { self.rec.mul(a, b) }
    fn div(&mut self, a: u32, b: u32) -> u32 { self.rec.div(a, b) }
    fn rem(&mut self, a: u32, b: u32) -> u32 { self.rec.rem(a, b) }
    fn neg(&mut self, a: u32) -> u32 { self.rec.neg(a) }
    fn and(&mut self, a: u32, b: u32) -> u32 { self.rec.and(a, b) }
    fn or (&mut self, a: u32, b: u32) -> u32 { self.rec.or(a, b) }
    fn xor(&mut self, a: u32, b: u32) -> u32 { self.rec.xor(a, b) }
    fn not(&mut self, a: u32) -> u32 { self.rec.not(a) }
    fn shl(&mut self, a: u32, b: u32) -> u32 { self.rec.shl(a, b) }
    fn shr(&mut self, a: u32, b: u32) -> u32 { self.rec.shr(a, b) }
    fn rotr(&mut self, a: u32, b: u32) -> u32 { self.rec.rotr(a, b) }
    fn rbit(&mut self, a: u32) -> u32 { self.rec.rbit(a) }
    fn clz(&mut self, a: u32) -> u32 { self.rec.clz(a) }
    fn eq(&mut self, a: u32, b: u32) -> u32 { self.rec.eq(a, b) }
    fn ne(&mut self, a: u32, b: u32) -> u32 { self.rec.ne(a, b) }
    fn lt(&mut self, a: u32, b: u32) -> u32 { self.rec.lt(a, b) }
    fn le(&mut self, a: u32, b: u32) -> u32 { self.rec.le(a, b) }
    fn gt(&mut self, a: u32, b: u32) -> u32 { self.rec.gt(a, b) }
    fn ge(&mut self, a: u32, b: u32) -> u32 { self.rec.ge(a, b) }
    fn cast(&mut self, v: u32, ty: IlType) -> u32 { self.rec.cast(v, ty) }
    fn bitcast(&mut self, v: u32, ty: IlType) -> u32 { self.rec.bitcast(v, ty) }
    fn sext(&mut self, v: u32, ty: IlType) -> u32 { self.rec.sext(v, ty) }
    fn pair128(&mut self, hi: u32, lo: u32) -> u32 { self.rec.pair128(hi, lo) }
    fn hi64(&mut self, a: u32) -> u32 { self.rec.hi64(a) }
    fn vzip(&mut self, a: u32, b: u32, ew: u32, hi: bool) -> u32 { self.rec.vzip(a, b, ew, hi) }
    fn vfbin(&mut self, a: u32, b: u32, ew: u32, op: u32) -> u32 { self.rec.vfbin(a, b, ew, op) }
    fn vibin(&mut self, a: u32, b: u32, ew: u32, op: u32) -> u32 { self.rec.vibin(a, b, ew, op) }
    fn vmovmsk(&mut self, a: u32, ew: u32) -> u32 { self.rec.vmovmsk(a, ew) }
    fn vfun(&mut self, a: u32, ew: u32, op: u32) -> u32 { self.rec.vfun(a, ew, op) }
    fn vfminmax(&mut self, a: u32, b: u32, ew: u32, m: bool) -> u32 { self.rec.vfminmax(a, b, ew, m) }
    fn vshuf(&mut self, a: u32, b: u32, ew: u32, sel: u32) -> u32 { self.rec.vshuf(a, b, ew, sel) }
    fn vshufw(&mut self, a: u32, sel: u32, hi: bool) -> u32 { self.rec.vshufw(a, sel, hi) }
    fn vcvt(&mut self, a: u32, k: u32) -> u32 { self.rec.vcvt(a, k) }
    fn fcmpp(&mut self, a: u32, b: u32, p: u32, w: u32) -> u32 { self.rec.fcmpp(a, b, p, w) }
    fn fminmax(&mut self, a: u32, b: u32, m: bool) -> u32 { self.rec.fminmax(a, b, m) }
    fn ternary(&mut self, c: u32, a: u32, b: u32) -> u32 { self.rec.ternary(c, a, b) }
    fn cond(&mut self, c: u32, then: &mut dyn FnMut(&mut Self), els: &mut dyn FnMut(&mut Self)) {
        // Record CondBegin/Else/End markers around the arms. Closures take
        // &mut Tier1, so we can't delegate to rec.cond directly — emit the
        // markers via rec's stmt-emit, run the arms on self.
        self.rec.stmt_marker(IlOpKind::CondBegin, IlType::Bool, &[c], 0);
        then(self);
        self.rec.stmt_marker(IlOpKind::CondElse, IlType::Unit, &[], 0);
        els(self);
        self.rec.stmt_marker(IlOpKind::CondEnd, IlType::Unit, &[], 0);
    }
    fn loop_n(&mut self, n: u32, body: &mut dyn FnMut(&mut Self)) {
        panic!("tier-1 v1: loop_n not yet emitted — this block needs tier-0. n=v{n}");
    }
    fn branch(&mut self, target: u32, link: bool) { self.rec.branch(target, link) }
    fn local_new(&mut self, ty: IlType) -> crate::LocalId { self.rec.local_new(ty) }
    fn local_read(&mut self, l: crate::LocalId) -> u32 { self.rec.local_read(l) }
    fn local_write(&mut self, l: crate::LocalId, v: u32) { self.rec.local_write(l, v) }
    fn call_intrinsic(&mut self, id: crate::IntrinsicId, args: &[u32]) -> Option<u32> {
        panic!("tier-1 v1: call_intrinsic {} — this block needs tier-0", id.0);
    }
    fn call_native(&mut self, slot: crate::NativeSlot, args: &[u32]) -> Option<u32> {
        self.rec.call_native(slot, args)
    }
    fn unimplemented(&mut self, name: &'static str) { panic!("tier-1: unimplemented insn {name}") }
    // Float ops — IlRecorder handles them (as ops); the EMITTER panics on F-type
    // in emit_op (v1 scalar-int only). So these forward but blocks using them
    // die-loud at emit-time.
    fn fabs(&mut self, a: u32) -> u32 { self.rec.fabs(a) }
    fn fsqrt(&mut self, a: u32) -> u32 { self.rec.fsqrt(a) }
    fn fround(&mut self, a: u32, m: crate::RoundMode) -> u32 { self.rec.fround(a, m) }
    fn fceil(&mut self, a: u32) -> u32 { self.rec.fceil(a) }
    fn ffloor(&mut self, a: u32) -> u32 { self.rec.ffloor(a) }
    fn fisnan(&mut self, a: u32) -> u32 { self.rec.fisnan(a) }
    fn velement_read(&mut self, v: u32, i: u32, et: IlType) -> u32 { self.rec.velement_read(v, i, et) }
    fn velement_write(&mut self, v: u32, i: u32, e: u32) -> u32 { self.rec.velement_write(v, i, e) }
    fn vzero_top(&mut self, v: u32) -> u32 { self.rec.vzero_top(v) }
}

// ── the emitter: walk IlOps, emit per-kind with allocated Locs ─────────────

struct Emitter<'a> {
    enc: Aarch64Enc,
    layout: &'static StateLayout,
    rec: &'a IlRecorder,
    alloc: &'a AllocResult,
    /// Cond patch-stack: (cbz_at, xc_reg, b_at). See CondBegin/Else/End.
    cond_stack: Vec<(usize, u32, usize)>,
    /// Op-indices to skip (dead-RegWrite-elim marked them; emit_op checks first).
    dead_ops: std::collections::HashSet<usize>,
}

impl<'a> Emitter<'a> {
    fn new(layout: &'static StateLayout, rec: &'a IlRecorder, alloc: &'a AllocResult) -> Self {
        let mut enc = Aarch64Enc::new();
        // Prologue: save callee-saved x19-x28 + lr, install state/spill bases.
        // 12 regs × 8 = 96 bytes → 96 (already 16-aligned).
        enc.sub_i(31, 31, 96);
        for (i, r) in (19..=28).enumerate() { enc.str_x(r, 31, (i as u32) * 8); }
        enc.str_x(30, 31, 80);
        enc.mov_r(X_STATE, 0);
        enc.mov_r(X_SPILL, 1);
        Self { enc, layout, rec, alloc, cond_stack: vec![], dead_ops: Default::default() }
    }

    fn finalize(mut self, n_spill_slots: u32) -> CompiledBlock {
        // Epilogue: restore callee-saved + lr, ret.
        for (i, r) in (19..=28).enumerate() { self.enc.ldr_x(r, 31, (i as u32) * 8); }
        self.enc.ldr_x(30, 31, 80);
        self.enc.add_i(31, 31, 96);
        self.enc.ret();
        // Reuse tier-0's CompiledBlock finalization (mmap RWX + __clear_cache).
        crate::tier0::compile_from_enc(self.enc, n_spill_slots)
    }

    /// Materialize SSA val `v` into host reg `into` (a scratch x9-x11 usually,
    /// or the destination reg for a mov). If Loc::Reg → return that reg directly
    /// (no move needed); Spill → ldr into scratch; Dead → shouldn't be read
    /// (linear_scan only marks vals with last_use==def_at Dead = never-read).
    fn get(&mut self, v: u32, scratch: u32) -> u32 {
        match self.alloc.locs[v as usize] {
            Loc::Reg(i) => REG_BASE + i as u32,
            Loc::Spill(s) => { self.enc.ldr_x(scratch, X_SPILL, (s as u32) * 8); scratch }
            Loc::Dead => panic!("tier-1: read of Dead val v{v}"),
        }
    }

    /// Where does `v`'s result go? Reg(i) → x(13+i) (emit directly there);
    /// Spill(s) → emit into `scratch` then str; Dead → skip (caller checks first).
    fn dest(&self, v: u32) -> (u32, Option<u16>) {
        match self.alloc.locs[v as usize] {
            Loc::Reg(i) => (REG_BASE + i as u32, None),
            Loc::Spill(s) => (X_S0, Some(s)),
            Loc::Dead => unreachable!(),  // caller skips Dead outs
        }
    }

    fn put(&mut self, out: u32, xd: u32, spill_to: Option<u16>) {
        if let Some(s) = spill_to {
            self.enc.str_x(xd, X_SPILL, (s as u32) * 8);
        }
        // Reg case: xd IS the dest reg, nothing to store.
        let _ = out;
    }

    #[inline] fn state_off(&self, f: RegFile, idx: u32) -> u32 {
        if f.0 == self.layout.flag_file { self.layout.off_flags }
        else { (self.layout.reg_off)(f, idx) }
    }

    fn width_bits(&self, ty: IlType) -> u32 {
        match ty { IlType::Bool => 1, IlType::I{width,..} => width as u32,
                   IlType::F{width} => width as u32, IlType::V128 => 128, _ => 64 }
    }

    fn mask_to(&mut self, xd: u32, w: u32) {
        if w >= 64 { return; }
        // (1<<w)-1 encodes as a single logical-immediate AND (N=1,immr=0,imms=w-1).
        // Was mov_imm64+and_r (2-3 insns).
        self.enc.and_lowmask(xd, xd, w);
    }

    /// Float-bin: xa/xb hold the F-typed bits in X-regs; fmov X→d0/d1, op→d0,
    /// fmov d0→xd. Same pattern as tier-0's fbin. F32 uses s0/s1 via fmov_s_w.
    fn emit_fbin(&mut self, xd: u32, xa: u32, xb: u32, fw: u32,
                 f: impl FnOnce(&mut Aarch64Enc)) {
        if fw == 64 { self.enc.fmov_d_x(0, xa); self.enc.fmov_d_x(1, xb); }
        else { self.enc.fmov_s_w(0, xa); self.enc.fmov_s_w(1, xb); }
        f(&mut self.enc);
        if fw == 64 { self.enc.fmov_x_d(xd, 0); } else { self.enc.fmov_w_s(xd, 0); }
    }

    fn emit_op(&mut self, op_idx: usize, op: &IlOp) {
        use IlOpKind::*;
        // Dead-store-elim marked this RegWrite dead → skip.
        if self.dead_ops.contains(&op_idx) { return; }
        // If this op produces a value that's Dead → skip entirely (DCE-at-emit).
        if let Some(out) = op.out {
            if matches!(self.alloc.locs[out as usize], Loc::Dead) { return; }
        }
        let ty = op.ty;
        let w = self.width_bits(ty);
        // Wide/vec: v1 is scalar-only (F32/F64 handled below, w≤64). RegRead/
        // RegWrite of XMM at width≤64 (Vsd/Vss lane-0) IS a single state[]
        // word — handled below. V128 xmm access + u128/i128 arith → die-loud
        // (block routes to tier-0 via tier-selection).
        // v2 float-arm: F32/F64 handled below (w≤64). V128/u128 arith → die-loud.
        // RegRead xmm@V128: emit as LO64-only (single ldr_x from state[xmm_off]).
        //   ‡ UNSAFE for any consumer that reads hi64 — but every V128-arith op
        //   (Xor/And/Or/vzip) has w>64 on the OP itself → dies below. So the
        //   only V128-RegRead consumers that survive are Cast V128→U64
        //   (movq r,xmm = take lo64, correct) and RegWrite lane-0. The guard on
        //   binops/Cast catches everything hi64-consuming.
        if w > 64 {
            match op.kind {
                RegRead => { /* lo64-only ldr below; consumer's w>64 guard catches hi64 uses */ }
                RegWrite if self.width_bits(self.rec.ty_of(op.args[0])) <= 64 => { /* lane-0 str_x */ }
                Cast if op.imm == 0 && w <= 64 => unreachable!(),  // handled by fw>64 in Cast below
                _ => panic!("tier-1 v1: wide ty {ty:?} at op {:?} — use tier-0 for this block", op.kind),
            }
        }

        match op.kind {
            Literal => {
                let out = op.out.unwrap();
                let (xd, sp) = self.dest(out);
                self.enc.mov_imm64(xd, op.imm as u64);
                if w < 64 { self.mask_to(xd, w); }
                self.put(out, xd, sp);
            }
            RegRead => {
                let out = op.out.unwrap();
                let (xd, sp) = self.dest(out);
                let f = RegFile((op.imm >> 32) as u8);
                let idx = op.imm as u32;
                let off = self.state_off(f, idx);
                self.enc.ldr_x(xd, X_STATE, off);
                // Flag-file: extract bit (aarch64's file=2 idx=0 = whole word, no extract).
                if f.0 == self.layout.flag_file && !(self.layout.flag_file == 2 && idx == 0) {
                    let bit = (self.layout.flag_bit)(idx);
                    self.enc.mov_imm64(X_S1, bit as u64);
                    self.enc.lsrv(xd, xd, X_S1);
                    self.enc.mov_imm64(X_S1, 1);
                    self.enc.and_r(xd, xd, X_S1);
                } else if f.0 == 3 {
                    // XMM lane-0 read at width≤64 (Vsd/Vss). state[xmm_off] = lo64.
                    // reg_off(3, idx) already points at word 0. w=32 (Vss) needs mask.
                    if w < 64 { self.mask_to(xd, w); }
                    // ‡ V128 read (w=128) would need 2 words → wide-guard catches above.
                } else if w < 64 {
                    // GPR at op_w<64: mask (partial-reg read).
                    if f.0 == 0 { self.mask_to(xd, w); }
                }
                self.put(out, xd, sp);
            }
            RegWrite => {
                let v = op.args[0];
                let f = RegFile((op.imm >> 32) as u8);
                let idx = op.imm as u32;
                let xs = self.get(v, X_S0);
                let off = self.state_off(f, idx);
                if f.0 == self.layout.flag_file && !(self.layout.flag_file == 2 && idx == 0) {
                    // RMW the flags word: read, clear bit, or in (v&1)<<bit, write.
                    let bit = (self.layout.flag_bit)(idx);
                    self.enc.ldr_x(X_S1, X_STATE, off);
                    self.enc.mov_imm64(X_S2, !(1u64 << bit));
                    self.enc.and_r(X_S1, X_S1, X_S2);
                    self.enc.mov_imm64(X_S2, 1);
                    self.enc.and_r(X_S2, xs, X_S2);
                    // shl S2 by bit
                    self.enc.mov_imm64(X_S0, bit as u64);   // ‡ clobbers X_S0 — xs may be X_S0!
                    // Guard: if xs was X_S0 (spill materialize), we've already and'd its
                    // low bit into X_S2, so clobbering X_S0 is safe now.
                    self.enc.lslv(X_S2, X_S2, X_S0);
                    self.enc.orr_r(X_S1, X_S1, X_S2);
                    self.enc.str_x(X_S1, X_STATE, off);
                } else {
                    // GPR write: x86 partial-write handled by write_operand upstream
                    // (which does read-mask-insert BEFORE reg_write for width<32);
                    // for width=32, x86 zeroes upper (gpr_w_zext) — that's ALSO
                    // done upstream in operand.rs. So here: full 64-bit str_x.
                    // ‡ v1: aarch64's RegFile(1)=SIMD/wide — panics at width>64 above.
                    self.enc.str_x(xs, X_STATE, off);
                }
            }
            MemRead => {
                let out = op.out.unwrap();
                let (xd, sp) = self.dest(out);
                let addr = self.get(op.args[0], X_S1);
                // host_addr = state[off_membase] + guest_addr
                self.enc.ldr_x(X_S2, X_STATE, self.layout.off_membase);
                self.enc.add_r(X_S2, X_S2, addr);
                match w {
                    8  => self.enc.put_raw(0x38400000 | (X_S2<<5) | xd),  // ldrb wD,[xS2]
                    16 => self.enc.put_raw(0x78400000 | (X_S2<<5) | xd),  // ldrh wD,[xS2]
                    32 => self.enc.ldr_w(xd, X_S2, 0),
                    64 => self.enc.ldr_x(xd, X_S2, 0),
                    _ => panic!("tier-1 mem_read w={w}"),
                }
                self.put(out, xd, sp);
            }
            MemWrite => {
                let addr = self.get(op.args[0], X_S0);
                let v = self.get(op.args[1], X_S1);
                self.enc.ldr_x(X_S2, X_STATE, self.layout.off_membase);
                self.enc.add_r(X_S2, X_S2, addr);
                let vw = self.width_bits(self.rec.ty_of(op.args[1]));
                match vw {
                    8  => self.enc.put_raw(0x38000000 | (X_S2<<5) | v),  // strb wV,[xS2]
                    16 => self.enc.put_raw(0x78000000 | (X_S2<<5) | v),  // strh wV,[xS2]
                    32 => self.enc.str_w(v, X_S2, 0),
                    64 => self.enc.str_x(v, X_S2, 0),
                    _ => panic!("tier-1 mem_write w={vw}"),
                }
            }
            Add | Sub | Mul | Div | Rem | And | Or | Xor | Shl | Shr | Rotr => {
                let out = op.out.unwrap();
                let (xd, sp) = self.dest(out);
                let xa = self.get(op.args[0], X_S0);
                let xb = self.get(op.args[1], X_S1);
                // Float arm: F32/F64 route through fadd_d/s etc via emit_fbin.
                if let IlType::F{width: fw} = ty {
                    self.emit_fbin(xd, xa, xb, fw as u32, |e| match (op.kind, fw) {
                        (Add, 64) => e.fadd_d(0,0,1), (Add, 32) => e.fadd_s(0,0,1),
                        (Sub, 64) => e.fsub_d(0,0,1), (Sub, 32) => e.fsub_s(0,0,1),
                        (Mul, 64) => e.fmul_d(0,0,1), (Mul, 32) => e.fmul_s(0,0,1),
                        (Div, 64) => e.fdiv_d(0,0,1), (Div, 32) => e.fdiv_s(0,0,1),
                        _ => panic!("tier-1 float bin: {:?} F{fw}", op.kind),
                    });
                    self.put(out, xd, sp);
                    return;
                }
                let signed = matches!(ty, IlType::I{signed:true,..});
                let w32 = w <= 32;
                match op.kind {
                    Add => self.enc.add_r(xd, xa, xb),
                    Sub => self.enc.sub_r(xd, xa, xb),
                    Mul => self.enc.mul_r(xd, xa, xb),
                    And => self.enc.and_r(xd, xa, xb),
                    Or  => self.enc.orr_r(xd, xa, xb),
                    Xor => self.enc.eor_r(xd, xa, xb),
                    Shl => self.enc.lslv(xd, xa, xb),
                    Shr => if signed {
                        if w32 { self.enc.asrv_w(xd, xa, xb) } else { self.enc.asrv(xd, xa, xb) }
                    } else {
                        if w32 { self.enc.lsrv_w(xd, xa, xb) } else { self.enc.lsrv(xd, xa, xb) }
                    },
                    Rotr => if w32 { self.enc.rorv_w(xd, xa, xb) } else { self.enc.rorv(xd, xa, xb) },
                    Div => if signed { self.enc.sdiv(xd, xa, xb) } else { self.enc.udiv(xd, xa, xb) },
                    Rem => {
                        // rem = a - (a/b)*b via msub. X_S2 = quot.
                        if signed { self.enc.sdiv(X_S2, xa, xb) } else { self.enc.udiv(X_S2, xa, xb) }
                        self.enc.msub(xd, X_S2, xb, xa);
                    }
                    _ => unreachable!(),
                }
                if w < 64 && !matches!(op.kind, Shl) { self.mask_to(xd, w); }
                self.put(out, xd, sp);
            }
            Neg | Not | Rbit | Clz => {
                let out = op.out.unwrap();
                let (xd, sp) = self.dest(out);
                let xa = self.get(op.args[0], X_S0);
                let w32 = w <= 32;
                match op.kind {
                    Neg => { self.enc.mov_imm64(X_S1, 0); self.enc.sub_r(xd, X_S1, xa); }
                    Not => {
                        let mask = if matches!(ty, IlType::Bool) { 1u64 }
                                   else if w < 64 { (1u64 << w) - 1 } else { u64::MAX };
                        self.enc.mov_imm64(X_S1, mask);
                        self.enc.eor_r(xd, xa, X_S1);
                    }
                    Rbit => if w32 { self.enc.rbit_w(xd, xa) } else { self.enc.rbit(xd, xa) },
                    Clz  => if w32 { self.enc.clz_w(xd, xa) } else { self.enc.clz(xd, xa) },
                    _ => unreachable!(),
                }
                if w < 64 && matches!(op.kind, Neg) { self.mask_to(xd, w); }
                self.put(out, xd, sp);
            }
            Eq | Ne | Lt | Le | Gt | Ge => {
                let out = op.out.unwrap();
                let (xd, sp) = self.dest(out);
                let xa = self.get(op.args[0], X_S0);
                let xb = self.get(op.args[1], X_S1);
                let signed = matches!(self.rec.ty_of(op.args[0]), IlType::I{signed:true,..});
                self.enc.cmp_r(xa, xb);
                let cond = match (op.kind, signed) {
                    (Eq,_) => Cond::EQ, (Ne,_) => Cond::NE,
                    (Lt,true) => Cond::LT, (Lt,false) => Cond::CC,
                    (Le,true) => Cond::LE, (Le,false) => Cond::LS,
                    (Gt,true) => Cond::GT, (Gt,false) => Cond::HI,
                    (Ge,true) => Cond::GE, (Ge,false) => Cond::CS,
                    _ => unreachable!(),
                };
                // cset xd, cond  (= csinc xd, xzr, xzr, !cond)
                self.enc.cset(xd, cond);
                self.put(out, xd, sp);
            }
            Cast | Sext => {
                let out = op.out.unwrap();
                let (xd, sp) = self.dest(out);
                let xa = self.get(op.args[0], X_S0);
                let from_ty = self.rec.ty_of(op.args[0]);
                let fw = self.width_bits(from_ty);
                // imm=1 → BITCAST (retype only, no bit-change): just move.
                //   Covers (as-f64)/(as-f32)/(signed W v). Bits unchanged; the
                //   type-tag is compile-time-only. imm=2/3 (pair128/hi64) are
                //   wide → caught by the w>64 guard above.
                if op.imm == 1 {
                    if xd != xa { self.enc.mov_r(xd, xa); }
                    self.put(out, xd, sp);
                    return;
                }
                // V128 → U64 (movq r,xmm's Cast): take lo64. RegRead V128 above
                //   emitted lo64-only into xa; here just move (the val's already
                //   the lo64 in a 1-reg Loc).
                if matches!(from_ty, IlType::V128) && matches!(ty, IlType::I{width:64,..}) {
                    if xd != xa { self.enc.mov_r(xd, xa); }
                    self.put(out, xd, sp);
                    return;
                }
                // I↔F numerical converts (imm=0 Cast, ty crosses I/F).
                let from_f = matches!(from_ty, IlType::F{..});
                let to_f = matches!(ty, IlType::F{..});
                if from_f || to_f {
                    match (from_f, to_f, from_ty, ty) {
                        // I → F: scvtf. iw≤32→_w form, fw picks d/s.
                        (false, true, IlType::I{width:iw,..}, IlType::F{width:fwd}) => {
                            match (iw > 32, fwd) {
                                (true, 64) => self.enc.scvtf_d_x(0, xa),
                                (true, 32) => self.enc.scvtf_s_x(0, xa),
                                (false,64) => self.enc.scvtf_d_w(0, xa),
                                (false,32) => self.enc.scvtf_s_w(0, xa),
                                _ => panic!("tier-1 cast I{iw}→F{fwd}"),
                            }
                            if fwd==64 { self.enc.fmov_x_d(xd,0); } else { self.enc.fmov_w_s(xd,0); }
                        }
                        // F → I: fcvtzs (truncate). Matches tier-0.
                        (true, false, IlType::F{width:fws}, IlType::I{width:iw,..}) => {
                            if fws==64 { self.enc.fmov_d_x(0,xa); } else { self.enc.fmov_s_w(0,xa); }
                            match (iw > 32, fws) {
                                (true, 64) => self.enc.fcvtzs_x_d(xd, 0),
                                (true, 32) => self.enc.fcvtzs_x_s(xd, 0),
                                (false,64) => self.enc.fcvtzs_w_d(xd, 0),
                                (false,32) => { self.enc.fcvtzs_x_s(xd, 0); },
                                _ => panic!("tier-1 cast F{fws}→I{iw}"),
                            }
                            if iw < 64 { self.mask_to(xd, iw as u32); }
                        }
                        // F32↔F64: fcvt.
                        (true, true, IlType::F{width:32}, IlType::F{width:64}) => {
                            self.enc.fmov_s_w(0, xa); self.enc.fcvt_d_s(0, 0); self.enc.fmov_x_d(xd, 0);
                        }
                        (true, true, IlType::F{width:64}, IlType::F{width:32}) => {
                            self.enc.fmov_d_x(0, xa); self.enc.fcvt_s_d(0, 0); self.enc.fmov_w_s(xd, 0);
                        }
                        // F→F same-width or Bool→F etc: die-loud.
                        _ => panic!("tier-1 cast {:?}→{:?}", from_ty, ty),
                    }
                    self.put(out, xd, sp);
                    return;
                }
                match (op.kind, fw, w) {
                    (Cast, _, _) if matches!(ty, IlType::Bool) => {
                        // to-Bool: reduce to 0/1 via cmp+cset. NOT mask-to-1 (that
                        // tests bit-0; we want !=0 — same class as tier-0's cast).
                        self.enc.cmp_r(xa, 31);
                        self.enc.cset(xd, Cond::NE);
                    }
                    (Cast, _, tw) if tw >= fw => {
                        // Widen (zext) or same: value's already zero-above-fw (invariant),
                        // so just move.
                        if xd != xa { self.enc.mov_r(xd, xa); }
                    }
                    (Cast, _, tw) => {
                        // Narrow: mask to tw.
                        if xd != xa { self.enc.mov_r(xd, xa); }
                        self.mask_to(xd, tw);
                    }
                    (Sext, sw, tw) => {
                        // Sign-extend from sw bits to tw. sbfm xd, xa, immr=0, imms=sw-1.
                        // 0x93400000 = SBFM Xd,Xn,#immr,#imms (N=1, sf=1).
                        self.enc.put_raw(0x93400000 | ((sw-1)<<10) | (xa<<5) | xd);
                        if tw < 64 { self.mask_to(xd, tw); }
                    }
                    _ => unreachable!(),
                }
                // ‡ Cast marker imm=2 (pair128) / imm=3 (hi64) — v1 doesn't handle
                //   (would need wide-arm; those are only used by DIV-wide which
                //   panics at w>64 above anyway).
                self.put(out, xd, sp);
            }
            Ternary => {
                // csel: if c!=0 pick a else b. cmp xc, xzr; csel xd, xa, xb, NE.
                // cmp_r(xc, 31) = subs xzr, xc, xzr = cmp-to-0, no 4th scratch needed.
                // ‡ vzip marker (imm bit 16..) rides Ternary in IlRecorder — those
                //   are V128, caught by the w>64 guard above already.
                let out = op.out.unwrap();
                let (xd, sp) = self.dest(out);
                let xc = self.get(op.args[0], X_S0);
                let xa = self.get(op.args[1], X_S1);
                let xb = self.get(op.args[2], X_S2);
                self.enc.cmp_r(xc, 31);   // subs xzr, xc, xzr → Z=(xc==0)
                self.enc.csel(xd, xa, xb, Cond::NE);
                self.put(out, xd, sp);
            }
            Branch | BranchLink => {
                // Write state[pc] = target, then RET (via inline epilogue).
                // Jcc emits Branch INSIDE a cond-then, followed by the
                // fallthrough Branch outside — both need to reach the epilogue.
                // v1: emit the epilogue INLINE at each branch site (2 branches
                // per Jcc-block = 2 epilogues; small, correct). tier-0 does the
                // same (finalize appends epilogue, but its cond arm never
                // reaches it since Jcc's then-branch str_x's pc then falls
                // through to the b-over-else which... actually tier-0's Branch
                // just str_x's pc and the SINGLE epilogue at end serves both
                // paths since both fall through). Same here: just str_x pc;
                // both paths converge at the post-walk epilogue via cond's b.
                let target = self.get(op.args[0], X_S0);
                self.enc.str_x(target, X_STATE, self.layout.off_pc);
                // ‡ BranchLink: link semantics done upstream (x86 CALL = push
                //   next_pc as separate ops before this Branch).
            }
            // Cond: the walk sees CondBegin(c) / then-ops / CondElse / else-ops / CondEnd
            // linearly. Emit like tier-0's cond: cbz over then, b over else, patch.
            // Track a small stack of pending patch-sites (nested conds work).
            CondBegin => {
                let xc = self.get(op.args[0], X_S0);
                let cbz_at = self.enc.here();
                self.enc.cbz(xc, 0);   // → else-label (patched at CondElse)
                self.cond_stack.push((cbz_at, xc, 0));  // (cbz_at, xc-reg, b_at fills at Else)
            }
            CondElse => {
                let (cbz_at, xc, _) = self.cond_stack.pop().unwrap();
                let b_at = self.enc.here();
                self.enc.b(0);         // → end (patched at CondEnd)
                let else_at = self.enc.here();
                self.enc.patch(cbz_at, 0xB4000000 | ((((else_at - cbz_at) as u32) & 0x7FFFF) << 5) | xc);
                self.cond_stack.push((0, 0, b_at));  // repurpose: only b_at matters at CondEnd
            }
            CondEnd => {
                let (_, _, b_at) = self.cond_stack.pop().unwrap();
                let end_at = self.enc.here();
                self.enc.patch(b_at, 0x14000000 | (((end_at - b_at) as u32) & 0x03FFFFFF));
            }
            CallIntrinsic => {
                panic!("tier-1 v1: call_intrinsic (id={}) — use tier-0", op.imm);
            }
            Unimplemented => {
                panic!("tier-1: unimplemented insn (recorder saw it)");
            }
        }
    }
}
