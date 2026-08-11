//! Tier-0 template JIT (aarch64-host). Each `Builder` method emits a FIXED aarch64
//! sequence via `Aarch64Enc`. No register allocation: `Val` = a spill-slot index;
//! every op loads args from spill, computes into x9/x10, stores result to spill.
//! ~4 host-insns per IL-op → ~40 per guest-insn. Correct-by-construction; tier-1
//! does regalloc.
//!
//! Calling convention: `entry(state: *mut u64, spill: *mut u64)`. Prologue moves
//! x0→x28 (state-base), x1→x27 (spill-base); block body indexes both; epilogue = ret.
//!
//! State layout (flat u64[]):
//!   [0..32]  = GPR x0..x30, x31=SP
//!   [32]     = NZCV (whole-word; individual-flag reads mask a bit)
//!   [33]     = pc (branch writes here)
//!   [34..66] = VEC v0..v31 low-64 (‡ full V128 at v2)
//!
//! Rung-4 step ③ tier-0. Oracle = InterpretingBuilder state-diff on the same
//! (insn, pre-state) triples the exec-truth fuzz already covers.

#![cfg(target_arch = "aarch64")]
#![allow(dead_code)]

use crate::{Builder, IlType, RegFile, LocalId, NativeSlot, IntrinsicId, RoundMode};
use crate::aarch64_enc::{Aarch64Enc, Cond};

const X_STATE: u32 = 28;   // x28 = state-base
const X_SPILL: u32 = 27;   // x27 = spill-base
const X_A: u32 = 9;        // scratch A
const X_B: u32 = 10;       // scratch B
const X_C: u32 = 11;       // scratch C (for 3-arg ops / temp)
const X_D: u32 = 12;       // scratch D (rare 4-value ops — wide-div guard)

// State offsets (in bytes from x28).
/// Per-guest-arch state layout — parameterizes Tier0's flat u64[] offsets so the
/// same emitter serves aarch64-guest AND x64-guest (and any future arch). Tier0
/// takes a `&'static StateLayout` at `new()`; the per-arch constant lives with the
/// arch's state.rs (AARCH64_LAYOUT here; X64_LAYOUT in xfusion-recomp/state.rs).
///
/// RegFile mapping is arch-specific:
///   aarch64: 0=GPR(x0-30), 1=VEC(v0-31), 2=NZCV(idx 1..4 = bit 31/30/29/28)
///   x64:     0=GPR(rax-r15), 1=EFLAGS(idx = bit# directly), 2=SEG, 3=XMM
/// The flag-file is bit-packed into ONE word; `flag_bit(idx)` maps idx→bit-position.
pub struct StateLayout {
    pub state_words: usize,
    pub off_pc: u32,          // byte-offset of the pc/rip word in flat[]
    pub off_membase: u32,     // byte-offset of the mem_base host-ptr word
    /// Which RegFile.0 is the bit-packed flags file (aarch64=2, x64=1).
    pub flag_file: u8,
    pub off_flags: u32,       // byte-offset of the flags word
    /// idx → bit-position in the flags word. aarch64: 1..4→31..28; x64: idx→idx.
    pub flag_bit: fn(u32) -> u32,
    /// Byte-offset of RegFile f at index idx, for NON-flag files. Panics on unknown file.
    /// (The flag file is handled via off_flags + flag_bit RMW, not here.)
    pub reg_off: fn(RegFile, u32) -> u32,
    /// aarch64 GPR W-writes zero-extend to 64 (Tier0 handles it here). x64 does
    /// partial-write semantics in operand.rs BEFORE calling reg_write (already
    /// zero-extended to u64), so tier-0's reg_write is a plain store. This flag
    /// gates the aarch64-specific W-zext-in-reg_write.
    pub gpr_w_zext: bool,
}

pub static AARCH64_LAYOUT: StateLayout = StateLayout {
    state_words: 68,
    off_pc: 33 * 8,
    off_membase: 66 * 8,
    flag_file: 2,             // NZCV
    off_flags: 32 * 8,
    flag_bit: |idx| 32 - idx, // idx 1..4 → N=31, Z=30, C=29, V=28
    reg_off: |f, idx| match f.0 {
        0 => idx * 8,         // GPR x0..x30
        1 => 34 * 8 + idx * 8, // VEC (‡ low-64 only, v0..v31)
        _ => panic!("aarch64 tier-0: file {} not wired", f.0),
    },
    gpr_w_zext: true,
};

/// Legacy alias — aarch64 harness code uses this. Same as AARCH64_LAYOUT.state_words.
pub const STATE_WORDS: usize = 68;

pub struct Tier0 {
    pub enc: Aarch64Enc,
    next_slot: u32,
    /// slot → IlType (for width-aware ops that need to know arg types).
    tys: Vec<IlType>,
    /// Set once branch() emits — subsequent ops are dead (unreachable). Tier-0
    /// still emits them (harmless — after the ret) to keep the trait contract simple.
    branched: bool,
    layout: &'static StateLayout,
}

impl Tier0 {
    pub fn new() -> Self { Self::with_layout(&AARCH64_LAYOUT) }

    pub fn with_layout(layout: &'static StateLayout) -> Self {
        let _ = layout;  // (prologue is layout-independent — x28=state, x27=spill)
        let mut enc = Aarch64Enc::new();
        // Prologue: save callee-saved x27/x28 (we clobber both), move args into place.
        enc.sub_i(31, 31, 32);          // sub sp, sp, #32
        enc.str_x(27, 31, 0);
        enc.str_x(28, 31, 8);
        enc.str_x(30, 31, 16);          // save lr (branch may clobber via bl later — ‡ v2)
        enc.mov_r(X_STATE, 0);          // x28 = state
        enc.mov_r(X_SPILL, 1);          // x27 = spill
        Self { enc, next_slot: 0, tys: vec![], branched: false, layout }
    }

    /// Whether this block emitted a `branch` (= it terminates itself; the driver
    /// doesn't append a fallthrough). NB: b.cond emits `cond(c, |b| branch(taken),
    /// |b| branch(fallthrough))` per the .isa — so BOTH arms branch, and the driver
    /// sees branched=true regardless of which arm fires at runtime.
    pub fn branched(&self) -> bool { self.branched }

    /// Finalize: emit epilogue, mmap RWX, return the callable block.
    pub fn finalize(mut self) -> CompiledBlock {
        // Epilogue: restore callee-saved, ret.
        self.enc.ldr_x(27, 31, 0);
        self.enc.ldr_x(28, 31, 8);
        self.enc.ldr_x(30, 31, 16);
        self.enc.add_i(31, 31, 32);
        self.enc.ret();
        compile_from_enc(self.enc, self.next_slot)
    }

    // ── helpers ────────────────────────────────────────────────────────────
    // width>64 (u128/i128 + V128) Vals occupy TWO consecutive spill-slots (lo@s, hi@s+1).
    // `tys` still keys by the LEADING slot; hi-slot gets a Unit placeholder to keep
    // indices monotone. is_wide(ty) drives 2-slot alloc + 2-word load/store.
    fn is_wide(ty: IlType) -> bool {
        matches!(ty, IlType::I{width, ..} if width > 64) || matches!(ty, IlType::V128)
    }
    fn slot(&mut self, ty: IlType) -> u32 {
        let s = self.next_slot;
        if Self::is_wide(ty) {
            self.next_slot += 2; self.tys.push(ty); self.tys.push(IlType::Unit);
        } else {
            self.next_slot += 1; self.tys.push(ty);
        }
        s
    }
    fn load(&mut self, xt: u32, slot: u32) { self.enc.ldr_x(xt, X_SPILL, slot * 8); }
    fn store(&mut self, xt: u32, slot: u32) { self.enc.str_x(xt, X_SPILL, slot * 8); }
    fn load2(&mut self, xt: u32, xt_hi: u32, slot: u32) {
        self.enc.ldr_x(xt, X_SPILL, slot * 8);
        self.enc.ldr_x(xt_hi, X_SPILL, (slot + 1) * 8);
    }
    fn store2(&mut self, xt: u32, xt_hi: u32, slot: u32) {
        self.enc.str_x(xt, X_SPILL, slot * 8);
        self.enc.str_x(xt_hi, X_SPILL, (slot + 1) * 8);
    }
    #[inline] fn state_off(&self, f: RegFile, idx: u32) -> u32 {
        if f.0 == self.layout.flag_file { self.layout.off_flags }
        else { (self.layout.reg_off)(f, idx) }
    }

    /// Post-op mask to `ty`'s width (matches interp's ibin mask). Skip for width≥64.
    fn mask_to(&mut self, ty: IlType) {
        if let IlType::I{width, ..} = ty {
            if width < 64 {
                // Logical-immediate AND: (1<<w)-1 encodes as N=1,immr=0,imms=w-1.
                // Was mov_imm64(mask)+and_r (2-3 insns) → 1 insn.
                self.enc.and_lowmask(X_A, X_A, width as u32);
            }
        }
    }
    /// Binary op template: ldr a, ldr b, <op x9,x9,x10>, mask-to-width, str result.
    fn bin(&mut self, a: u32, b: u32, ty: IlType, f: impl FnOnce(&mut Aarch64Enc)) -> u32 {
        let s = self.slot(ty);
        self.load(X_A, a); self.load(X_B, b);
        f(&mut self.enc);
        self.mask_to(ty);
        self.store(X_A, s);
        s
    }
    /// Float bin: bits stay in X-slots; fmov X→d0/d1, f(d0,d1)→d0, fmov d0→X.
    fn fbin(&mut self, a: u32, b: u32, ty: IlType, f: impl FnOnce(&mut Aarch64Enc)) -> u32 {
        let s = self.slot(ty);
        let f64 = matches!(ty, IlType::F{width:64});
        self.load(X_A, a);
        if f64 { self.enc.fmov_d_x(0, X_A); } else { self.enc.fmov_s_w(0, X_A); }
        self.load(X_A, b);
        if f64 { self.enc.fmov_d_x(1, X_A); } else { self.enc.fmov_s_w(1, X_A); }
        f(&mut self.enc);   // computes into d0/s0
        if f64 { self.enc.fmov_x_d(X_A, 0); } else { self.enc.fmov_w_s(X_A, 0); }
        self.store(X_A, s);
        s
    }
    /// 2-slot bin: a=(X_A,X_C) b=(X_B,X_D), f operates on both halves, store2.
    fn bin_wide(&mut self, a: u32, b: u32, f: impl FnOnce(&mut Aarch64Enc)) -> u32 {
        let ty = self.tys[a as usize];
        let s = self.slot(ty);
        self.load2(X_A, X_C, a); self.load2(X_B, X_D, b);
        f(&mut self.enc);
        self.store2(X_A, X_C, s);
        s
    }
    fn una(&mut self, a: u32, ty: IlType, f: impl FnOnce(&mut Aarch64Enc)) -> u32 {
        let s = self.slot(ty);
        self.load(X_A, a);
        f(&mut self.enc);
        self.mask_to(ty);
        self.store(X_A, s);
        s
    }
    fn cmp_op(&mut self, a: u32, b: u32, cond: Cond) -> u32 {
        let s = self.slot(IlType::Bool);
        self.load(X_A, a); self.load(X_B, b);
        self.enc.cmp_r(X_A, X_B);
        self.enc.cset(X_A, cond);
        self.store(X_A, s);
        s
    }
    /// Float compare → NZCV → cset(cond) → 0/1. FCMP NZCV: eq→0110 lt→1000
    /// gt→0010 unord→0011. So: EQ(Z=1)=eq, MI(N=1)=lt, GT(!Z&&N==V)=gt,
    /// VS(V=1)=unord, LS(!C||Z)=le-ordered, HI(C&&!Z)=gt-or-unord,
    /// LT(N!=V)=lt-or-unord (= x86 COMISS's CF).
    fn fcmp_op(&mut self, a: u32, b: u32, cond: Cond) -> u32 {
        let s = self.slot(IlType::Bool);
        let f64 = matches!(self.tys[a as usize], IlType::F{width:64});
        self.load(X_A, a);
        if f64 { self.enc.fmov_d_x(0, X_A); } else { self.enc.fmov_s_w(0, X_A); }
        self.load(X_A, b);
        if f64 { self.enc.fmov_d_x(1, X_A); } else { self.enc.fmov_s_w(1, X_A); }
        if f64 { self.enc.fcmp_d(0, 1); } else { self.enc.fcmp_s(0, 1); }
        self.enc.cset(X_A, cond);
        self.store(X_A, s);
        s
    }
}

impl Builder for Tier0 {
    type Val = u32;

    fn ty_of(&self, v: u32) -> IlType { self.tys[v as usize] }

    fn literal(&mut self, ty: IlType, bits: u128) -> u32 {
        let s = self.slot(ty);
        self.enc.mov_imm64(X_A, bits as u64);
        if Self::is_wide(ty) {
            self.enc.mov_imm64(X_C, (bits >> 64) as u64);
            self.store2(X_A, X_C, s);
        } else {
            self.store(X_A, s);
        }
        s
    }
    fn reg_read(&mut self, f: RegFile, idx: u32, ty: IlType) -> u32 {
        let s = self.slot(ty);
        let off = self.state_off(f, idx);
        // XMM/vector regfile at V128: 2-word read from state[off, off+8].
        if Self::is_wide(ty) {
            self.enc.ldr_x(X_A, X_STATE, off);
            self.enc.ldr_x(X_C, X_STATE, off + 8);
            self.store2(X_A, X_C, s);
            return s;
        }
        self.enc.ldr_x(X_A, X_STATE, off);
        if f.0 == 0 { self.mask_to(ty); }
        // Flag file: extract bit at layout.flag_bit(idx). aarch64 idx=0 = whole-word read.
        if f.0 == self.layout.flag_file && !(self.layout.flag_file == 2 && idx == 0) {
            let bit = (self.layout.flag_bit)(idx);
            self.enc.mov_imm64(X_B, bit as u64);
            self.enc.lsrv(X_A, X_A, X_B);
            self.enc.mov_imm64(X_B, 1);
            self.enc.and_r(X_A, X_A, X_B);
        }
        self.store(X_A, s);
        s
    }
    fn reg_write(&mut self, f: RegFile, idx: u32, v: u32) {
        // XMM/vector regfile at V128: 2-word store to state[off, off+8].
        if Self::is_wide(self.tys[v as usize]) {
            let off = self.state_off(f, idx);
            self.load2(X_A, X_C, v);
            self.enc.str_x(X_A, X_STATE, off);
            self.enc.str_x(X_C, X_STATE, off + 8);
            return;
        }
        self.load(X_A, v);
        // Flag-file bit-write: RMW the flags word at layout.flag_bit(idx). aarch64
        // idx=0 = whole-word write (falls through to plain str below).
        if f.0 == self.layout.flag_file && !(self.layout.flag_file == 2 && idx == 0) {
            let bit = (self.layout.flag_bit)(idx);
            let off_flags = self.layout.off_flags;
            self.enc.ldr_x(X_B, X_STATE, off_flags);
            self.enc.mov_imm64(X_C, !(1u64 << bit));
            self.enc.and_r(X_B, X_B, X_C);
            self.enc.mov_imm64(X_C, bit as u64);
            self.enc.lslv(X_A, X_A, X_C);
            self.enc.orr_r(X_A, X_B, X_A);
            self.enc.str_x(X_A, X_STATE, off_flags);
            return;
        }
        // aarch64 GPR W-write zero-extends here. x64 does partial-write semantics in
        // operand.rs BEFORE reg_write (already u64), so gpr_w_zext=false skips this.
        if self.layout.gpr_w_zext && f.0 == 0 && matches!(self.tys[v as usize], IlType::I{width:32, ..}) {
            self.enc.mov_imm64(X_B, 0xFFFF_FFFF);
            self.enc.and_r(X_A, X_A, X_B);
        }
        let off = self.state_off(f, idx);
        self.enc.str_x(X_A, X_STATE, off);
    }
    fn mem_read(&mut self, a: u32, ty: IlType) -> u32 {
        // Identity-map: host_addr = state.mem_base + guest_addr. Load at width.
        // ‡ v1: unchecked (mem_len=0). v2: bounds-check → fault-intrinsic on OOB
        //   (which is where the SMC write-protect fault also routes).
        let s = self.slot(ty);
        self.load(X_A, a);                            // guest addr
        self.enc.ldr_x(X_B, X_STATE, self.layout.off_membase);  // host base
        self.enc.add_r(X_A, X_B, X_A);                // host addr
        // Width-select the load. Encoder has ldr_x/ldr_w; add byte/half + Q for wide.
        match ty {
            IlType::I{width: 8, ..}  => self.enc.put_raw(0x38400000 | (X_A<<5) | X_A),  // ldrb w9,[x9]
            IlType::I{width: 16, ..} => self.enc.put_raw(0x78400000 | (X_A<<5) | X_A),  // ldrh w9,[x9]
            IlType::I{width: 32, ..} | IlType::F{width: 32} => self.enc.ldr_w(X_A, X_A, 0),
            IlType::I{width: 64, ..} | IlType::F{width: 64} => self.enc.ldr_x(X_A, X_A, 0),
            IlType::I{width: 128, ..} | IlType::V128 => {
                // 2-word load: lo, hi.
                self.enc.ldr_x(X_C, X_A, 8);
                self.enc.ldr_x(X_A, X_A, 0);
                self.store2(X_A, X_C, s);
                return s;
            }
            _ => panic!("tier-0 mem_read: {:?}", ty),
        }
        self.store(X_A, s);
        s
    }
    fn mem_write(&mut self, a: u32, v: u32) {
        let ty = self.tys[v as usize];
        self.load(X_A, a);
        self.enc.ldr_x(X_B, X_STATE, self.layout.off_membase);
        self.enc.add_r(X_A, X_B, X_A);
        match ty {
            IlType::I{width: 8, ..}  => { self.load(X_B, v); self.enc.put_raw(0x38000000 | (X_A<<5) | X_B); }  // strb
            IlType::I{width: 16, ..} => { self.load(X_B, v); self.enc.put_raw(0x78000000 | (X_A<<5) | X_B); }  // strh
            IlType::I{width: 32, ..} | IlType::F{width: 32} => { self.load(X_B, v); self.enc.str_w(X_B, X_A, 0); }
            IlType::I{width: 64, ..} | IlType::F{width: 64} => { self.load(X_B, v); self.enc.str_x(X_B, X_A, 0); }
            IlType::I{width: 128, ..} | IlType::V128 => {
                self.load2(X_B, X_C, v);
                self.enc.str_x(X_B, X_A, 0);
                self.enc.str_x(X_C, X_A, 8);
            }
            _ => panic!("tier-0 mem_write: {:?}", ty),
        }
    }

    fn add(&mut self, a: u32, b: u32) -> u32 {
        let t = self.tys[a as usize];
        if let IlType::F{width:fw} = t {
            return self.fbin(a, b, t, move |e| if fw==64 {e.fadd_d(0,0,1)} else {e.fadd_s(0,0,1)});
        }
        if Self::is_wide(t) {
            let s = self.slot(t);
            self.load2(X_A, X_C, a); self.load2(X_B, 12, b);
            self.enc.adds_r(X_A, X_A, X_B);
            self.enc.adc_r(X_C, X_C, 12);
            self.store2(X_A, X_C, s);
            return s;
        }
        self.bin(a, b, t, |e| e.add_r(X_A, X_A, X_B))
    }
    fn sub(&mut self, a: u32, b: u32) -> u32 {
        let t = self.tys[a as usize];
        if let IlType::F{width:fw} = t {
            return self.fbin(a, b, t, move |e| if fw==64 {e.fsub_d(0,0,1)} else {e.fsub_s(0,0,1)});
        }
        if Self::is_wide(t) {
            let s = self.slot(t);
            self.load2(X_A, X_C, a); self.load2(X_B, 12, b);
            self.enc.subs_r(X_A, X_A, X_B);
            self.enc.sbc_r(X_C, X_C, 12);
            self.store2(X_A, X_C, s);
            return s;
        }
        self.bin(a, b, t, |e| e.sub_r(X_A, X_A, X_B))
    }
    fn mul(&mut self, a: u32, b: u32) -> u32 { let t = self.tys[a as usize];
        if let IlType::F{width:fw} = t {
            return self.fbin(a, b, t, move |e| if fw==64 {e.fmul_d(0,0,1)} else {e.fmul_s(0,0,1)});
        }
        if Self::is_wide(t) {
            // u128 mul (128×128→low-128): lo = a.lo*b.lo; hi = umulh(a.lo,b.lo)
            //   + a.lo*b.hi + a.hi*b.lo (all mod 2^64 — the standard schoolbook low-128).
            // This is CORRECT for u128 (all bits unsigned). For i128 it's ALSO correct
            // — low-128 of a signed product = low-128 of unsigned product (two's-comp).
            // The prior version used smulh for the signed arm, which double-corrected
            // (smulh already applies the sign-correction that the a.hi/b.hi cross-terms
            // also encode when hi = sext-fill). Fixed: ALWAYS umulh + cross-terms.
            let s = self.slot(t);
            self.load2(X_A, X_C, a); self.load2(X_B, 12, b);
            self.enc.umulh(13, X_A, X_B);
            self.enc.mul_r(14, X_A, 12);   self.enc.add_r(13, 13, 14);
            self.enc.mul_r(14, X_C, X_B);  self.enc.add_r(13, 13, 14);
            self.enc.mul_r(X_A, X_A, X_B);
            self.enc.mov_r(X_C, 13);
            self.store2(X_A, X_C, s);
            return s;
        }
        self.bin(a, b, t, |e| e.mul_r(X_A, X_A, X_B)) }
    fn pair128(&mut self, hi: u32, lo: u32) -> u32 {
        // Direct slot placement: result is a 2-slot Val with lo@s, hi@s+1. No shl.
        let s = self.slot(IlType::I{signed:false, width:128});
        self.load(X_A, lo);
        self.load(X_C, hi);
        self.store2(X_A, X_C, s);
        s
    }
    fn hi64(&mut self, a: u32) -> u32 {
        let s = self.slot(IlType::U64);
        self.load2(X_A, X_C, a);
        self.store(X_C, s);
        s
    }
    fn vdpp(&mut self, a: u32, b: u32, imm: u32, ew: u32) -> u32 {
        // fmul q2 = a*b; zero src-unmasked lanes (INS from wzr); faddp×N → sum broadcast
        // to all lanes; zero dst-unmasked lanes. imm is compile-time-per-decode (Ib), so
        // the INS-zero emissions are conditional at codegen-time = optimal per call-site.
        let s = self.slot(IlType::V128);
        self.load2(X_A, X_C, a);
        self.enc.ins_vd_x(0, 0, X_A); self.enc.ins_vd_x(0, 1, X_C);
        self.load2(X_B, X_D, b);
        self.enc.ins_vd_x(1, 0, X_B); self.enc.ins_vd_x(1, 1, X_D);
        let (sz, nlanes) = if ew == 64 { (1u32, 2) } else { (0, 4) };
        self.enc.fmul_v(2, 0, 1, sz);
        // src-mask: zero lanes where imm[4+i]==0
        for i in 0..nlanes {
            if imm & (1 << (4+i)) == 0 {
                if ew == 64 { self.enc.ins_vd_x(2, i, 31); }  // xzr → .D[i]
                else { self.enc.ins_vs_w(2, i, 31); }         // wzr → .S[i]
            }
        }
        // horizontal sum → broadcast: faddp v2,v2,v2 twice for .4S (once for .2D).
        // .4S: [a,b,c,d]→[a+b,c+d,a+b,c+d]→[(a+b)+(c+d)]×4. .2D: [a,b]→[a+b,a+b].
        self.enc.faddp_v(2, 2, 2, sz);
        if ew != 64 { self.enc.faddp_v(2, 2, 2, sz); }
        // dst-mask: zero output lanes where imm[i]==0
        for i in 0..nlanes {
            if imm & (1 << i) == 0 {
                if ew == 64 { self.enc.ins_vd_x(2, i, 31); }
                else { self.enc.ins_vs_w(2, i, 31); }
            }
        }
        self.enc.umov_x_vd(X_A, 2, 0); self.enc.umov_x_vd(X_C, 2, 1);
        self.store2(X_A, X_C, s);
        s
    }
    fn bswap(&mut self, a: u32) -> u32 {
        let ty = self.tys[a as usize];
        let w = match ty { IlType::I{width,..} => width, _ => panic!("bswap non-int") };
        let s = self.slot(ty);
        self.load(X_A, a);
        match w { 64 => self.enc.rev_x(X_A, X_A), 32 => self.enc.rev_w(X_A, X_A),
                  _ => panic!("bswap width={w} (SDM-undefined at 16)") }
        self.store(X_A, s);
        s
    }
    fn vhadd(&mut self, a: u32, b: u32, ew: u32) -> u32 {
        // x86 HADDPS dst,src ≡ ARM FADDP Vd, q(dst), q(src) — SAME lane pairing.
        let s = self.slot(IlType::V128);
        self.load2(X_A, X_C, a);
        self.enc.ins_vd_x(0, 0, X_A); self.enc.ins_vd_x(0, 1, X_C);
        self.load2(X_B, X_D, b);
        self.enc.ins_vd_x(1, 0, X_B); self.enc.ins_vd_x(1, 1, X_D);
        let sz = if ew == 64 { 1 } else { 0 };
        self.enc.faddp_v(2, 0, 1, sz);
        self.enc.umov_x_vd(X_A, 2, 0); self.enc.umov_x_vd(X_C, 2, 1);
        self.store2(X_A, X_C, s);
        s
    }
    fn vfcmpp(&mut self, a: u32, b: u32, ew: u32, pred: u32) -> u32 {
        // NEON FCM* are ORDERED (NaN → 0). For x86 preds:
        //   0 EQ    = fcmeq(a,b)
        //   1 LT    = fcmgt(b,a)
        //   2 LE    = fcmge(b,a)
        //   3 UNORD = NOT(ordered) = mvn(fcmge(a,b) | fcmgt(b,a))
        //             — ordered iff a>=b OR b>a (both fail only when NaN in either)
        //   4-7     = NOT of 0-3 (SDM: NEQ/NLT/NLE/ORD are the exact inverses,
        //             including NaN behavior — NEQ w/ NaN = true, ORD = ordered).
        let s = self.slot(IlType::V128);
        self.load2(X_A, X_C, a);
        self.enc.ins_vd_x(0, 0, X_A); self.enc.ins_vd_x(0, 1, X_C);
        self.load2(X_B, X_D, b);
        self.enc.ins_vd_x(1, 0, X_B); self.enc.ins_vd_x(1, 1, X_D);
        let sz = if ew == 64 { 1 } else { 0 };
        let base = pred & 3;
        match base {
            0 => self.enc.fcmeq_v(2, 0, 1, sz),
            1 => self.enc.fcmgt_v(2, 1, 0, sz),   // a<b = b>a
            2 => self.enc.fcmge_v(2, 1, 0, sz),   // a<=b = b>=a
            3 => {
                // ordered = fcmge(a,b) | fcmgt(b,a); UNORD = NOT that
                self.enc.fcmge_v(2, 0, 1, sz);
                self.enc.fcmgt_v(3, 1, 0, sz);
                self.enc.orr_v16b(2, 2, 3);
                self.enc.mvn_v16b(2, 2);
            }
            _ => unreachable!(),
        }
        if pred & 4 != 0 {
            // 4-7 = NOT of 0-3
            self.enc.mvn_v16b(2, 2);
        }
        self.enc.umov_x_vd(X_A, 2, 0); self.enc.umov_x_vd(X_C, 2, 1);
        self.store2(X_A, X_C, s);
        s
    }
    fn vfminmax(&mut self, a: u32, b: u32, ew: u32, is_max: bool) -> u32 {
        // x86 semantics: dst[i] = (a[i] op b[i]) ? a[i] : b[i]. FCMGT is
        // ORDERED (NaN→0), matches x86 (NaN → cond false → b=src). For MIN
        // use FCMGT b,a (= a<b, ordered → NaN→false→b). Then BIT: q2 starts
        // as b, insert a where mask is set.
        let s = self.slot(IlType::V128);
        self.load2(X_A, X_C, a);
        self.enc.ins_vd_x(0, 0, X_A); self.enc.ins_vd_x(0, 1, X_C);
        self.load2(X_B, X_D, b);
        self.enc.ins_vd_x(1, 0, X_B); self.enc.ins_vd_x(1, 1, X_D);
        let sz = if ew == 64 { 1 } else { 0 };
        // mask q3: MAX → fcmgt(a,b); MIN → fcmgt(b,a) (= a<b ordered)
        if is_max { self.enc.fcmgt_v(3, 0, 1, sz); }
        else      { self.enc.fcmgt_v(3, 1, 0, sz); }
        // q2 = b; where mask, take a
        self.enc.mov_v(2, 1);
        self.enc.bit_v16b(2, 0, 3);
        self.enc.umov_x_vd(X_A, 2, 0); self.enc.umov_x_vd(X_C, 2, 1);
        self.store2(X_A, X_C, s);
        s
    }
    fn vfun(&mut self, a: u32, ew: u32, op: u32) -> u32 {
        let s = self.slot(IlType::V128);
        self.load2(X_A, X_C, a);
        self.enc.ins_vd_x(0, 0, X_A); self.enc.ins_vd_x(0, 1, X_C);
        let sz = if ew == 64 { 1 } else { 0 };
        match op {
            0 => self.enc.fsqrt_v(2, 0, sz),
            _ => panic!("vfun op={op}"),
        }
        self.enc.umov_x_vd(X_A, 2, 0); self.enc.umov_x_vd(X_C, 2, 1);
        self.store2(X_A, X_C, s);
        s
    }
    fn vmovmsk(&mut self, a: u32, ew: u32) -> u32 {
        // Load the V128 as two X-halves (X_A=lo, X_C=hi) — DON'T go through
        // Q-regs; the sign bits are at fixed X-reg positions. Extract each
        // via lsr_i + and_lowmask(1), pack via orr_lsl.
        let s = self.slot(IlType::I{signed:false, width:32});
        self.load2(X_A, X_C, a);
        match ew {
            8 => {
                // PMOVMSKB: 16 byte-signs → 16-bit mask. Per-half 8-bit
                // extract (bit i = bit 8i+7 of the u64), then combine
                // hi<<8 | lo. tier-0 dumb-correct (~48 insns); the smart
                // NEON path (sshr#7 + AND-mask-const + ADDV) is a later
                // opt (needs a rodata constant or movi cmode games).
                // Inline half-extract macro (can't be a self-method inside
                // the trait impl):
                macro_rules! half { ($xd:expr, $xs:expr) => {{
                    self.enc.lsr_i($xd, $xs, 7);
                    self.enc.and_lowmask($xd, $xd, 1);
                    for i in 1..8u32 {
                        self.enc.lsr_i(X_D, $xs, 8*i + 7);
                        self.enc.and_lowmask(X_D, X_D, 1);
                        self.enc.orr_lsl($xd, $xd, X_D, i);
                    }
                }}}
                half!(X_B, X_A);   // lo 8 bits → X_B
                half!(X_A, X_C);   // hi 8 bits → X_A (X_C source, X_A dest — X_A no longer needed as lo-src)
                self.enc.orr_lsl(X_A, X_B, X_A, 8); // X_A = lo | hi<<8
            }
            64 => {
                // bit0 = X_A>>63, bit1 = X_C>>63 → X_A | (X_C<<1)
                self.enc.lsr_i(X_A, X_A, 63);
                self.enc.lsr_i(X_C, X_C, 63);
                self.enc.orr_lsl(X_A, X_A, X_C, 1);
            }
            32 => {
                // Per half: bit@31 + bit@63. Extract to 2-bit, then combine
                // halves via orr_lsl #2.
                // lo half → X_A holds {b0,b1}:
                self.enc.lsr_i(X_D, X_A, 63);              // b1
                self.enc.lsr_i(X_A, X_A, 31);
                self.enc.and_lowmask(X_A, X_A, 1);         // b0
                self.enc.orr_lsl(X_A, X_A, X_D, 1);        // b0|b1<<1
                // hi half → X_C holds {b2,b3}:
                self.enc.lsr_i(X_D, X_C, 63);
                self.enc.lsr_i(X_C, X_C, 31);
                self.enc.and_lowmask(X_C, X_C, 1);
                self.enc.orr_lsl(X_C, X_C, X_D, 1);
                // combine:
                self.enc.orr_lsl(X_A, X_A, X_C, 2);
            }
            _ => panic!("vmovmsk ew={ew}"),
        }
        self.store(X_A, s);
        s
    }
    fn vishi(&mut self, a: u32, ew: u32, count: u32, dir: u32) -> u32 {
        let s = self.slot(IlType::V128);
        // x86 semantics: count >= ew → shl/lshr = 0, ashr = sign-fill (= sshr by ew-1).
        // NEON imm ranges: SHL 0..ew-1, USHR/SSHR 1..ew. count is compile-time-
        // known (Ib) so we branch in codegen, not at runtime.
        if count == 0 {
            // Identity — just copy through (all dirs).
            self.load2(X_A, X_C, a);
            self.store2(X_A, X_C, s);
            return s;
        }
        self.load2(X_A, X_C, a);
        self.enc.ins_vd_x(0, 0, X_A); self.enc.ins_vd_x(0, 1, X_C);
        if count >= ew {
            match dir {
                0 | 1 => self.enc.movi_zero(2),
                2 => self.enc.sshr_vi(2, 0, ew, ew - 1),  // fill with sign bit
                                                          // ‡ SDM: PSRA count>=ew → each lane = sign-bit-replicated.
                                                          // sshr #(ew-1) gives -1/0 per sign, but proper is #ew…
                                                          // Actually sshr by ew-1: e.g. 0x80000000>>31 = 0xFFFFFFFF, 0x7F..>>31=0. Correct.
                _ => panic!("vishi dir={dir}"),
            }
        } else {
            match dir {
                0 => self.enc.shl_vi(2, 0, ew, count),
                1 => self.enc.ushr_vi(2, 0, ew, count),
                2 => self.enc.sshr_vi(2, 0, ew, count),
                _ => panic!("vishi dir={dir}"),
            }
        }
        self.enc.umov_x_vd(X_A, 2, 0); self.enc.umov_x_vd(X_C, 2, 1);
        self.store2(X_A, X_C, s);
        s
    }
    fn vibin(&mut self, a: u32, b: u32, ew: u32, op: u32) -> u32 {
        let s = self.slot(IlType::V128);
        self.load2(X_A, X_C, a);
        self.enc.ins_vd_x(0, 0, X_A); self.enc.ins_vd_x(0, 1, X_C);
        self.load2(X_B, X_D, b);
        self.enc.ins_vd_x(1, 0, X_B); self.enc.ins_vd_x(1, 1, X_D);
        let size = match ew { 8=>0, 16=>1, 32=>2, 64=>3, _=>panic!("vibin ew={ew}") };
        match op {
            0 => self.enc.add_v(2, 0, 1, size),
            1 => self.enc.sub_v(2, 0, 1, size),
            2 => self.enc.mul_v(2, 0, 1, size),  // panics on ew=64 via debug_assert
            3 => self.enc.cmeq_v(2, 0, 1, size),
            4 => self.enc.cmgt_v(2, 0, 1, size), // signed (matches x86 PCMPGT)
            _ => panic!("vibin op={op}"),
        }
        self.enc.umov_x_vd(X_A, 2, 0); self.enc.umov_x_vd(X_C, 2, 1);
        self.store2(X_A, X_C, s);
        s
    }
    fn vfbin(&mut self, a: u32, b: u32, ew: u32, op: u32) -> u32 {
        // Same X↔Q dance as vzip: build q0/q1 from 2×X-word slots, NEON op → q2,
        // extract q2 → 2×X, store2. sz: 32→0(.4S), 64→1(.2D).
        let s = self.slot(IlType::V128);
        self.load2(X_A, X_C, a);
        self.enc.ins_vd_x(0, 0, X_A); self.enc.ins_vd_x(0, 1, X_C);
        self.load2(X_B, X_D, b);
        self.enc.ins_vd_x(1, 0, X_B); self.enc.ins_vd_x(1, 1, X_D);
        let sz = match ew { 32=>0, 64=>1, _=>panic!("vfbin ew={ew}") };
        match op {
            0 => self.enc.fadd_v(2, 0, 1, sz),
            1 => self.enc.fsub_v(2, 0, 1, sz),
            2 => self.enc.fmul_v(2, 0, 1, sz),
            3 => self.enc.fdiv_v(2, 0, 1, sz),
            _ => panic!("vfbin op={op}"),
        }
        self.enc.umov_x_vd(X_A, 2, 0); self.enc.umov_x_vd(X_C, 2, 1);
        self.store2(X_A, X_C, s);
        s
    }
    fn vcvt(&mut self, a: u32, kind: u32) -> u32 {
        // X↔Q + one/two NEON convert insns → q2 → extract → store2.
        let s = self.slot(IlType::V128);
        self.load2(X_A, X_C, a);
        self.enc.ins_vd_x(0, 0, X_A); self.enc.ins_vd_x(0, 1, X_C);
        match kind {
            0 => { self.enc.scvtf_v(2, 0, 0); }              // 4× i32→f32
            1 => { self.enc.fcvtzs_v(2, 0, 0); }             // 4× f32→i32 truncate
            2 => { self.enc.fcvtl_2d_2s(2, 0); }             // low 2× f32→f64
            3 => { self.enc.fcvtn_2s_2d(2, 0); }             // 2× f64→low 2× f32, hi=0
            4 => { self.enc.sxtl_2d_2s(2, 0);                // low 2× i32→i64
                   self.enc.scvtf_v(2, 2, 1); }              //   → 2× f64
            5 => { self.enc.fcvtzs_v(2, 0, 1);               // 2× f64→i64 truncate
                   self.enc.xtn_2s_2d(2, 2); }               //   → low 2× i32, hi=0
            6 => { self.enc.fcvtns_v(2, 0, 1);               // 2× f64→i64 round-nearest
                   self.enc.xtn_2s_2d(2, 2); }               //   → low 2× i32
            7 => { self.enc.fcvtns_v(2, 0, 0); }             // 4× f32→i32 round-nearest
            _ => panic!("vcvt kind={kind}"),
        }
        self.enc.umov_x_vd(X_A, 2, 0); self.enc.umov_x_vd(X_C, 2, 1);
        self.store2(X_A, X_C, s);
        s
    }
    fn vshufw(&mut self, src: u32, sel: u32, hi: bool) -> u32 {
        // Build q0=src, MOV q2←q0 (whole copy, preserves the un-shuffled half),
        // then 4× INS v2.H[base+i], v0.H[base+j] (j from sel bits).
        let s = self.slot(IlType::V128);
        self.load2(X_A, X_C, src);
        self.enc.ins_vd_x(0, 0, X_A); self.enc.ins_vd_x(0, 1, X_C);
        self.enc.mov_v(2, 0);
        let base = if hi { 4 } else { 0 };
        for i in 0..4u32 {
            let j = (sel >> (i*2)) & 3;
            self.enc.ins_vh_vh(2, base+i, 0, base+j);
        }
        self.enc.umov_x_vd(X_A, 2, 0); self.enc.umov_x_vd(X_C, 2, 1);
        self.store2(X_A, X_C, s);
        s
    }
    fn vshuf(&mut self, a: u32, b: u32, ew: u32, sel: u32) -> u32 {
        // Build q0=a, q1=b via X↔Q (same as vzip). Then N× INS q2.T[i], qS.T[j]
        // where S=(0 for i<N/2 else 1), j=(sel>>(i*bp))&mask. Extract q2 → store2.
        let s = self.slot(IlType::V128);
        self.load2(X_A, X_C, a);
        self.enc.ins_vd_x(0, 0, X_A); self.enc.ins_vd_x(0, 1, X_C);
        self.load2(X_B, X_D, b);
        self.enc.ins_vd_x(1, 0, X_B); self.enc.ins_vd_x(1, 1, X_D);
        match ew {
            32 => {
                for i in 0..4u32 {
                    let src = if i < 2 { 0 } else { 1 };
                    let j = (sel >> (i*2)) & 3;
                    self.enc.ins_vs_vs(2, i, src, j);
                }
            }
            64 => {
                for i in 0..2u32 {
                    let src = if i < 1 { 0 } else { 1 };
                    let j = (sel >> i) & 1;
                    self.enc.ins_vd_vd(2, i, src, j);
                }
            }
            _ => panic!("vshuf ew={ew}"),
        }
        self.enc.umov_x_vd(X_A, 2, 0); self.enc.umov_x_vd(X_C, 2, 1);
        self.store2(X_A, X_C, s);
        s
    }
    fn vzip(&mut self, a: u32, b: u32, ew: u32, hi: bool) -> u32 {
        // load2 a→(X_A,X_C), b→(X_B,X_D); INS q0.d[0/1]←X_A/X_C, q1←X_B/X_D;
        // zip1/2 q2, q0, q1; UMOV X_A←q2.d[0], X_C←q2.d[1]; store2.
        let s = self.slot(IlType::V128);
        self.load2(X_A, X_C, a);
        self.enc.ins_vd_x(0, 0, X_A); self.enc.ins_vd_x(0, 1, X_C);
        self.load2(X_B, X_D, b);
        self.enc.ins_vd_x(1, 0, X_B); self.enc.ins_vd_x(1, 1, X_D);
        let sz = match ew { 8=>0, 16=>1, 32=>2, 64=>3, _=>panic!("vzip ew={ew}") };
        if hi { self.enc.zip2_v(2, 0, 1, sz); } else { self.enc.zip1_v(2, 0, 1, sz); }
        self.enc.umov_x_vd(X_A, 2, 0); self.enc.umov_x_vd(X_C, 2, 1);
        self.store2(X_A, X_C, s);
        s
    }
    fn loop_n(&mut self, n: u32, body: &mut dyn FnMut(&mut Self)) {
        // In-block loop:
        //   ldr xD, [spill+n]      ; ctr = n
        //   head:
        //     cbz xD, exit
        //     str xD, [spill+ctr_slot]   ; save ctr (body may clobber X_D)
        //     <body>
        //     ldr xD, [spill+ctr_slot]
        //     sub xD, xD, #1
        //     b head
        //   exit:
        // Body's tmpl_N re-reads rdi/rsi from state each iter, so no cross-iter
        // Val dataflow needed. The ctr slot survives body's slot-alloc because
        // slots are monotone (body allocs after ctr_slot).
        let ctr_slot = self.slot(IlType::U64);
        self.load(X_D, n);
        let head = self.enc.buf.len();
        // cbz xD, exit — patched after body emits.
        let cbz_at = self.enc.buf.len();
        self.enc.put_raw(0xB4000000 | (X_D as u32));
        self.store(X_D, ctr_slot);
        body(self);
        self.load(X_D, ctr_slot);
        self.enc.sub_i(X_D, X_D, 1);
        // b head
        let off_back = ((head as i64 - self.enc.buf.len() as i64)) as i32;
        self.enc.put_raw(0x14000000 | ((off_back as u32) & 0x03FF_FFFF));
        // patch cbz imm19
        let off_fwd = (self.enc.buf.len() - cbz_at) as u32;
        let w0 = self.enc.buf[cbz_at]; self.enc.buf[cbz_at] = w0 | (off_fwd << 5);
    }
    fn loop_while(&mut self, n: u32, exit_on: bool, body: &mut dyn FnMut(&mut Self) -> u32) -> u32 {
        // Same shape as loop_n plus: body returns a Bool slot; after decrement,
        // load it → cbnz/cbz to exit. ctr_slot always holds the current count
        // (stored before body, re-stored after decrement) so exit-from-either-
        // branch reads the right value.
        let ctr_slot = self.slot(IlType::U64);
        self.load(X_D, n);
        self.store(X_D, ctr_slot);            // init (so count=0 → exit reads 0)
        let head = self.enc.buf.len();
        self.load(X_D, ctr_slot);
        let cbz1_at = self.enc.buf.len();
        self.enc.put_raw(0xB4000000 | (X_D as u32));   // cbz xD, exit — patched
        let flag = body(self);               // body writes ZF to state + returns it
        self.load(X_D, ctr_slot);
        self.enc.sub_i(X_D, X_D, 1);
        self.store(X_D, ctr_slot);
        self.load(X_A, flag);
        // exit_on=true → exit when flag≠0 → cbnz; exit_on=false → cbz.
        let cb2_base = if exit_on { 0xB5000000u32 } else { 0xB4000000 };
        let cbz2_at = self.enc.buf.len();
        self.enc.put_raw(cb2_base | (X_A as u32));     // cb[n]z xA, exit — patched
        // b head
        let off_back = (head as i64 - self.enc.buf.len() as i64) as i32;
        self.enc.put_raw(0x14000000 | ((off_back as u32) & 0x03FF_FFFF));
        // exit: patch both
        let exit_at = self.enc.buf.len();
        self.enc.buf[cbz1_at] |= ((exit_at - cbz1_at) as u32) << 5;
        self.enc.buf[cbz2_at] |= ((exit_at - cbz2_at) as u32) << 5;
        ctr_slot
    }
    fn div(&mut self, a: u32, b: u32) -> u32 { let t = self.tys[a as usize];
        if let IlType::F{width:fw} = t {
            return self.fbin(a, b, t, move |e| if fw==64 {e.fdiv_d(0,0,1)} else {e.fdiv_s(0,0,1)});
        }
        // Silicon udiv/sdiv: divide-by-0 → result 0 (no fault) — matches interp's
        // `if y==0 {0} else {x/y}`. So no zero-guard needed.
        let signed = matches!(t, IlType::I{signed:true, ..});
        let w32 = matches!(t, IlType::I{width, ..} if width <= 32);
        if Self::is_wide(t) {
            // 128÷N: aarch64 has no wide udiv. The x86 `div r64` common case is
            // `xor edx,edx; div rbx` (rdx=0 → dividend fits in 64) — do 64÷64 then.
            // If hi≠0 (or, signed: hi≠sext(lo)), we'd silently truncate → WRONG.
            // Die-loud instead (BRK #0xD1): the recon names it and we build the
            // full 128÷64 helper (bit-by-bit or __udivti3 callout) when it fires.
            let s = self.slot(t);
            self.load2(X_A, X_C, a);   // X_A=lo X_C=hi (dividend)
            self.load(X_B, b);         // divisor low-64 (b is also 128-wide here; hi ignored — divisor always fits op_w)
            // Guard: hi must be 0 (unsigned) or sign-fill of lo (signed) for the
            // dividend to fit in 64 bits. Signed: check hi == asr(lo, 63).
            // cbz/cbnz doesn't do eq-to-reg, so: eor tmp, hi, expected; cbz tmp, ok.
            if signed {
                self.enc.mov_imm64(X_B, 63);
                self.enc.asrv(X_D, X_A, X_B);         // X_D = asr(lo, 63) = 0 or -1
                self.enc.eor_r(X_D, X_C, X_D);        // X_D = hi ^ expected → 0 iff match
                self.load(X_B, b);                    // reload divisor (clobbered X_B)
            } else {
                // unsigned: expected hi = 0, so X_D = X_C directly.
                self.enc.mov_r(X_D, X_C);
            }
            let cbz_at = self.enc.buf.len();
            self.enc.put_raw(0xB4000000 | (X_D as u32));   // cbz xD, +? (patched below)
            self.enc.put_raw(0xD4200000 | (0xD1u32 << 5)); // brk #0xD1 (die-loud on wide dividend)
            let off = ((self.enc.buf.len() - cbz_at)) as u32;
            let w0 = self.enc.buf[cbz_at]; self.enc.buf[cbz_at] = w0 | (off << 5);
            // 64÷64 (correct since hi==0 verified).
            if signed { self.enc.sdiv(X_A, X_A, X_B); } else { self.enc.udiv(X_A, X_A, X_B); }
            self.enc.mov_imm64(X_C, 0);  // result-hi = 0
            self.store2(X_A, X_C, s);
            return s;
        }
        self.bin(a, b, t, move |e| match (signed, w32) {
            (true, true) => e.sdiv_w(X_A, X_A, X_B),
            (true, false) => e.sdiv(X_A, X_A, X_B),
            (false, true) => e.udiv_w(X_A, X_A, X_B),
            (false, false) => e.udiv(X_A, X_A, X_B),
        }) }
    fn rem(&mut self, a: u32, b: u32) -> u32 { let t = self.tys[a as usize];
        if Self::is_wide(t) {
            // Same guard as div: dividend must fit in 64 (hi==0 or ==sign-fill).
            // Under that, rem64 = a_lo - (a_lo/b_lo)*b_lo is correct.
            // ‡ emits the guard TWICE (div then rem in the DIV template) — fine
            //   for correctness; tier-1 CSE would collapse it. Alternatively the
            //   template could compute q then r=dvd-q*dvs — v2.
            let signed = matches!(t, IlType::I{signed:true, ..});
            let s = self.slot(t);
            self.load2(X_A, X_C, a); self.load(X_B, b);
            if signed {
                self.enc.mov_imm64(X_D, 63);
                self.enc.asrv(X_D, X_A, X_D);
                self.enc.eor_r(X_D, X_C, X_D);
            } else { self.enc.mov_r(X_D, X_C); }
            let cbz_at = self.enc.buf.len();
            self.enc.put_raw(0xB4000000 | (X_D as u32));
            self.enc.put_raw(0xD4200000 | (0xD1u32 << 5));
            let off = (self.enc.buf.len() - cbz_at) as u32;
            let w0 = self.enc.buf[cbz_at]; self.enc.buf[cbz_at] = w0 | (off << 5);
            if signed { self.enc.sdiv(X_C, X_A, X_B); } else { self.enc.udiv(X_C, X_A, X_B); }
            self.enc.msub(X_A, X_C, X_B, X_A);
            // Result-hi: signed → sign-fill of rem; unsigned → 0.
            if signed { self.enc.mov_imm64(X_C, 63); self.enc.asrv(X_C, X_A, X_C); }
            else { self.enc.mov_imm64(X_C, 0); }
            self.store2(X_A, X_C, s);
            return s;
        }
        // rem = a - (a/b)*b via msub. Same div-by-0 semantics (0 - 0*b = 0).
        // ‡ signed-rem sign convention: aarch64 sdiv truncates toward zero, so
        // a - trunc(a/b)*b = C's `%` semantics = interp's wrapping_rem. Matches.
        let signed = matches!(t, IlType::I{signed:true, ..});
        let w32 = matches!(t, IlType::I{width, ..} if width <= 32);
        self.bin(a, b, t, move |e| {
            // x11 = a/b (X_A,X_B still hold a,b after this since bin loaded them)
            // Actually bin's f gets called with X_A=a, X_B=b in-place. Compute q=x11=a/b,
            // then X_A = a - q*b via msub.
            match (signed, w32) {
                (true, true) => e.sdiv_w(X_C, X_A, X_B),
                (true, false) => e.sdiv(X_C, X_A, X_B),
                (false, true) => e.udiv_w(X_C, X_A, X_B),
                (false, false) => e.udiv(X_C, X_A, X_B),
            }
            e.msub(X_A, X_C, X_B, X_A);  // x9 = x9 - x11*x10
        }) }
    fn neg(&mut self, a: u32) -> u32 { let t = self.tys[a as usize]; self.una(a, t, |e| { e.mov_imm64(X_B, 0); e.sub_r(X_A, X_B, X_A); }) }
    fn and(&mut self, a: u32, b: u32) -> u32 { let t = self.tys[a as usize];
        if Self::is_wide(t) { return self.bin_wide(a, b, |e| { e.and_r(X_A,X_A,X_B); e.and_r(X_C,X_C,X_D); }); }
        self.bin(a, b, t, |e| e.and_r(X_A, X_A, X_B)) }
    fn or (&mut self, a: u32, b: u32) -> u32 { let t = self.tys[a as usize];
        if Self::is_wide(t) { return self.bin_wide(a, b, |e| { e.orr_r(X_A,X_A,X_B); e.orr_r(X_C,X_C,X_D); }); }
        self.bin(a, b, t, |e| e.orr_r(X_A, X_A, X_B)) }
    fn xor(&mut self, a: u32, b: u32) -> u32 { let t = self.tys[a as usize];
        if Self::is_wide(t) { return self.bin_wide(a, b, |e| { e.eor_r(X_A,X_A,X_B); e.eor_r(X_C,X_C,X_D); }); }
        self.bin(a, b, t, |e| e.eor_r(X_A, X_A, X_B)) }
    fn not(&mut self, a: u32) -> u32 { let t = self.tys[a as usize];
        if Self::is_wide(t) {
            let s = self.slot(t);
            self.load2(X_A, X_C, a);
            self.enc.mov_imm64(X_B, u64::MAX);
            self.enc.eor_r(X_A, X_A, X_B); self.enc.eor_r(X_C, X_C, X_B);
            self.store2(X_A, X_C, s);
            return s;
        }
        // Bool: eor #1 (logical negate — CSEL fuzz caught this: eor-all-1s on Bool gives
        // 0xFF..FE which is truthy). Int: bitwise complement within width.
        let mask = match t {
            IlType::Bool => 1u64,
            IlType::I{width, ..} if width < 64 => (1u64 << width) - 1,
            _ => u64::MAX,
        };
        self.una(a, t, move |e| { e.mov_imm64(X_B, mask); e.eor_r(X_A, X_A, X_B); }) }
    fn shl(&mut self, a: u32, b: u32) -> u32 { let t = self.tys[a as usize];
        if Self::is_wide(t) {
            // u128 << N. Mirror shr's structure: N<64 vs N>=64 branch.
            let s = self.slot(t);
            self.load2(X_A, X_C, a);
            self.load(X_B, b);
            self.enc.mov_imm64(12, 64);
            self.enc.cmp_r(X_B, 12);
            let bcond_at = self.enc.here();
            self.enc.nop();
            // <64: hi = (hi<<N)|(lo>>(64-N)); lo <<= N
            self.enc.sub_r(13, 12, X_B);
            self.enc.lslv(X_C, X_C, X_B);
            self.enc.lsrv(14, X_A, 13);
            self.enc.orr_r(X_C, X_C, 14);
            self.enc.lslv(X_A, X_A, X_B);
            let b_end_at = self.enc.here();
            self.enc.nop();
            // >=64: hi = lo<<(N-64); lo = 0
            let ge_at = self.enc.here();
            self.enc.sub_r(X_B, X_B, 12);
            self.enc.lslv(X_C, X_A, X_B);
            self.enc.mov_imm64(X_A, 0);
            let end_at = self.enc.here();
            self.enc.patch(bcond_at, 0x54000000 | ((((ge_at - bcond_at) as u32) & 0x7FFFF) << 5) | (Cond::CS as u32));
            self.enc.patch(b_end_at, 0x14000000 | (((end_at - b_end_at) as u32) & 0x03FFFFFF));
            self.store2(X_A, X_C, s);
            return s;
        }
        // Per interp: shl DOESN'T mask (int-promote semantics for the FCMP nzcv-shl).
        // So skip mask_to here — emit lslv only.
        let s = self.slot(t);
        self.load(X_A, a); self.load(X_B, b);
        self.enc.lslv(X_A, X_A, X_B);
        self.store(X_A, s);
        s
    }
    fn shr(&mut self, a: u32, b: u32) -> u32 { let t = self.tys[a as usize];
        let signed = matches!(t, IlType::I{signed:true, ..});
        if Self::is_wide(t) {
            // u128 >> N. The .isa's ONLY runtime u128-shr is by ct-const `bits` (32 or 64) to
            // extract carry-out. Emit both arms with a b.cond on N>=64.
            let s = self.slot(t);
            self.load2(X_A, X_C, a);
            self.load(X_B, b);
            self.enc.mov_imm64(12, 64);
            self.enc.cmp_r(X_B, 12);
            let bcond_at = self.enc.here();
            self.enc.nop();  // placeholder for b.hs → ge-arm
            // <64: lo = (lo>>N)|(hi<<(64-N)); hi >>= N
            self.enc.sub_r(13, 12, X_B);
            self.enc.lsrv(X_A, X_A, X_B);
            self.enc.lslv(14, X_C, 13);
            self.enc.orr_r(X_A, X_A, 14);
            if signed { self.enc.asrv(X_C, X_C, X_B); } else { self.enc.lsrv(X_C, X_C, X_B); }
            let b_end_at = self.enc.here();
            self.enc.nop();  // placeholder for b → end
            // >=64: lo = hi>>(N-64); hi = 0 (or asr 63 for signed)
            let ge_at = self.enc.here();
            self.enc.sub_r(X_B, X_B, 12);
            if signed { self.enc.asrv(X_A, X_C, X_B); } else { self.enc.lsrv(X_A, X_C, X_B); }
            if signed { self.enc.mov_imm64(X_B, 63); self.enc.asrv(X_C, X_C, X_B); }
            else { self.enc.mov_imm64(X_C, 0); }
            let end_at = self.enc.here();
            // Patch b.hs (CS = unsigned >=)
            self.enc.patch(bcond_at, 0x54000000 | ((((ge_at - bcond_at) as u32) & 0x7FFFF) << 5) | (Cond::CS as u32));
            self.enc.patch(b_end_at, 0x14000000 | (((end_at - b_end_at) as u32) & 0x03FFFFFF));
            self.store2(X_A, X_C, s);
            return s;
        }
        // Width-aware. For SIGNED at width<32: asrv_w's sign-bit is bit-31, but
        // the value's sign is bit-(w-1) with [31:w]=0 (caller-clean invariant) →
        // asrv_w is effectively logical. Sign-extend X_A from bit-(w-1) to full 64
        // first (lsl+asr pair, same as sext()), then 64-bit asrv; bin()'s mask_to
        // truncates back to w after. sar al,3 al=0x80 → tier0 was 0x10 (asrv_w on
        // zero-extended), interp/silicon 0xF0. For w=32 asrv_w is correct as-is
        // (sign-bit IS bit-31). Unsigned at w≤32 uses lsrv_w (correct — [31:w]=0).
        let w = match t { IlType::I{width, ..} => width as u32, _ => 64 };
        self.bin(a, b, t, move |e| match (signed, w) {
            (true, w) if w < 32 => {
                e.mov_imm64(X_C, (64 - w) as u64);
                e.lslv(X_A, X_A, X_C);
                e.asrv(X_A, X_A, X_C);
                e.asrv(X_A, X_A, X_B);
            }
            (true, 32) => e.asrv_w(X_A, X_A, X_B),
            (true, _)  => e.asrv(X_A, X_A, X_B),
            (false, w) if w <= 32 => e.lsrv_w(X_A, X_A, X_B),
            (false, _) => e.lsrv(X_A, X_A, X_B),
        }) }
    fn rotr(&mut self, a: u32, b: u32) -> u32 { let t = self.tys[a as usize];
        // Width-aware: rotr at 32-bit uses rorv_w (rotate within 32 bits, not 64).
        // Non-power-of-2 widths (the .isa's u1/u5/u6 etc) don't hit rotr in practice;
        // if they did, would need mod-width — ‡ assert for now.
        let w = match t { IlType::I{width, ..} => width, _ => 64 };
        self.bin(a, b, t, move |e| match w {
            32 => e.rorv_w(X_A, X_A, X_B),
            64 => e.rorv(X_A, X_A, X_B),
            _ => panic!("tier-0: rotr at width={w} (non-32/64)"),
        }) }
    fn rbit(&mut self, a: u32) -> u32 { let t = self.tys[a as usize];
        let w = match t { IlType::I{width, ..} => width as u32, _ => 64 };
        // width<32: rbit_w reverses full 32 → result lands in bits [31:32-w].
        // Shift down by (32-w) to put it in [w-1:0]. Note: TZCNT = clz(rbit(x))
        // used to CANCEL (rbit_w over-reverse ↔ clz_w over-count) for nonzero;
        // both fixed → still cancel, and zero-case now correct (was 32, now 16).
        self.una(a, t, move |e| {
            if w > 32 { e.rbit(X_A, X_A); }
            else {
                e.rbit_w(X_A, X_A);
                if w < 32 { e.lsr_i(X_A, X_A, 32 - w); }
            }
        }) }
    fn popcnt(&mut self, a: u32) -> u32 {
        // aarch64 has no scalar popcnt. Idiom: X→d0 (upper bytes zeroed by
        // fmov), CNT v0.8B (per-byte popcount), ADDV b0,v0.8B (sum → byte 0),
        // fmov w←s0. Result ≤64 always fits u8 → any dst width. For width<64,
        // the input is already masked to width by the caller (op_w-typed).
        let ty = self.tys[a as usize];
        let s = self.slot(ty);
        self.load(X_A, a);
        self.enc.fmov_d_x(0, X_A);
        self.enc.cnt_v8b(0, 0);
        self.enc.addv_b_8b(0, 0);
        self.enc.fmov_w_s(X_A, 0);
        self.store(X_A, s);
        s
    }
    fn clz(&mut self, a: u32) -> u32 { let t = self.tys[a as usize];
        let w = match t { IlType::I{width, ..} => width as u32, _ => 64 };
        // width<32: clz_w counts from bit-31 → off by (32-w). Emit clz_w then
        // sub #(32-w). Assumes input width-masked (bits [31:w]=0 — the caller-
        // clean invariant); dirty high bits → negative → the same audit ‡ as
        // the cast-identity note. LZCNT ax,ax at ax=0 was 32 (want 16); ax=0x8000
        // was clz_w=16 (want 0). Caught by tier-0-vs-interp on tzcnt/lzcnt tests.
        self.una(a, t, move |e| {
            if w > 32 { e.clz(X_A, X_A); }
            else {
                e.clz_w(X_A, X_A);
                if w < 32 { e.sub_i(X_A, X_A, 32 - w); }
            }
        }) }

    fn eq(&mut self, a: u32, b: u32) -> u32 {
        if matches!(self.tys[a as usize], IlType::F{..}) { return self.fcmp_op(a, b, Cond::EQ); }
        self.cmp_op(a, b, Cond::EQ) }
    fn ne(&mut self, a: u32, b: u32) -> u32 {
        if matches!(self.tys[a as usize], IlType::F{..}) { return self.fcmp_op(a, b, Cond::NE); }
        self.cmp_op(a, b, Cond::NE) }
    fn lt(&mut self, a: u32, b: u32) -> u32 {
        if matches!(self.tys[a as usize], IlType::F{..}) { return self.fcmp_op(a, b, Cond::MI); }
        let s = matches!(self.tys[a as usize], IlType::I{signed:true, ..});
        self.cmp_op(a, b, if s { Cond::LT } else { Cond::CC }) }
    fn le(&mut self, a: u32, b: u32) -> u32 {
        let s = matches!(self.tys[a as usize], IlType::I{signed:true, ..});
        self.cmp_op(a, b, if s { Cond::LE } else { Cond::LS }) }
    fn gt(&mut self, a: u32, b: u32) -> u32 {
        let s = matches!(self.tys[a as usize], IlType::I{signed:true, ..});
        self.cmp_op(a, b, if s { Cond::GT } else { Cond::HI }) }
    fn ge(&mut self, a: u32, b: u32) -> u32 {
        let s = matches!(self.tys[a as usize], IlType::I{signed:true, ..});
        self.cmp_op(a, b, if s { Cond::GE } else { Cond::CS }) }

    fn cast(&mut self, a: u32, to: IlType) -> u32 {
        let from = self.tys[a as usize];
        // Wide-identity fast-path: cast(V128,V128) previously fell through to the 1-word
        // int arm → hi-word LOST. The write_operand Mem-arm cast made every V128 mem-store
        // go through cast → MOVDQA-to-mem wrote only lo-64 → memory corruption. Caught by
        // MOVDQA-round-trip test + a full-app regression.
        //
        // NOT full identity: the fall-through I→I arm MASKS to width<64, and some aarch64
        // producers leave dirty bits above their declared width — cast(I32,I32) was
        // cleaning them. Full-identity is semantically-correct but exposes the dirty
        // producers (‡ they're the real bugs; a slot's bits should match its type — but
        // that's a whole audit). NARROW the fast-path to wide→wide only (V128/I128/U128 =
        // 2-slot values the 1-word fall-through can't handle). Narrow same-type still
        // masks (belt+braces).
        if from == to && Self::is_wide(to) { return a; }
        let s = self.slot(to);
        if Self::is_wide(to) && !Self::is_wide(from) {
            // u64→u128 widen: lo=a, hi=0 (or sext-fill for i128).
            self.load(X_A, a);
            if matches!(to, IlType::I{signed:true, ..}) {
                self.enc.mov_imm64(X_B, 63);
                self.enc.asrv(X_C, X_A, X_B);
            } else {
                self.enc.mov_imm64(X_C, 0);
            }
            self.store2(X_A, X_C, s);
            return s;
        }
        // I↔F (via d0/s0 scratch): the bits stay in an X-reg slot; the
        // conversion goes through fmov X↔D + scvtf/fcvtzs.
        match (from, to) {
            (IlType::I{signed, width:iw}, IlType::F{width:fw}) => {
                self.load(X_A, a);
                match (signed, iw > 32, fw) {
                    (_, true, 64) => self.enc.scvtf_d_x(0, X_A),
                    (_, true, 32) => self.enc.scvtf_s_x(0, X_A),
                    (_, false, 64) => self.enc.scvtf_d_w(0, X_A),
                    (_, false, 32) => self.enc.scvtf_s_w(0, X_A),
                    _ => panic!("cast I{iw}→F{fw}"),
                }
                // ‡ unsigned: SDM CVTSI2SD is signed-only, so signed=true always
                //   for x86 uses. If a template does unsigned int→float, ucvtf.
                if fw == 64 { self.enc.fmov_x_d(X_A, 0); } else { self.enc.fmov_w_s(X_A, 0); }
                self.store(X_A, s);
                return s;
            }
            (IlType::F{width:fw}, IlType::I{width:iw, ..}) => {
                self.load(X_A, a);
                if fw == 64 { self.enc.fmov_d_x(0, X_A); } else { self.enc.fmov_s_w(0, X_A); }
                match (iw > 32, fw) {
                    (true, 64) => self.enc.fcvtzs_x_d(X_A, 0),
                    (true, 32) => self.enc.fcvtzs_x_s(X_A, 0),
                    (false, 64) => self.enc.fcvtzs_w_d(X_A, 0),
                    (false, 32) => { self.enc.fcvtzs_x_s(X_A, 0); /* mask below */ }
                    _ => panic!("cast F{fw}→I{iw}"),
                }
                if iw < 64 {
                    self.enc.mov_imm64(X_B, (1u64 << iw) - 1);
                    self.enc.and_r(X_A, X_A, X_B);
                }
                self.store(X_A, s);
                return s;
            }
            (IlType::F{width:32}, IlType::F{width:64}) => {
                self.load(X_A, a);
                self.enc.fmov_s_w(0, X_A); self.enc.fcvt_d_s(0, 0); self.enc.fmov_x_d(X_A, 0);
                self.store(X_A, s); return s;
            }
            (IlType::F{width:64}, IlType::F{width:32}) => {
                self.load(X_A, a);
                self.enc.fmov_d_x(0, X_A); self.enc.fcvt_s_d(0, 0); self.enc.fmov_w_s(X_A, 0);
                self.store(X_A, s); return s;
            }
            _ => {}
        }
        // wide→narrow: read lo, mask; narrow→narrow: read, mask.
        // cast(_, Bool): reduce to 0/1 (cmp #0; cset ne). Was falling through
        //   unmasked → OF-flag's `(& … (1<<31))` value 0x80000000 stored as
        //   Bool then flag-RMW's `lslv v,bit` shifted it to bit 42 not bit 11
        //   → OF=0 always at the INT_MIN boundary → jle wrong → EVERY MSVC
        //   magic-static init skipped. The CP2077 691-vs-46K divergence root.
        //   interp's cast I→Bool = (bits!=0) → 1, so interp was correct.
        self.load(X_A, a);
        match to {
            IlType::Bool => {
                self.enc.cmp_r(X_A, 31);   // subs xzr, x9, xzr → Z=(x9==0)
                self.enc.cset(X_A, Cond::NE);
            }
            IlType::I{width, ..} if width < 64 => {
                self.enc.and_lowmask(X_A, X_A, width as u32);
            }
            _ => {}
        }
        self.store(X_A, s);
        s
    }
    fn bitcast(&mut self, a: u32, to: IlType) -> u32 {
        let s = self.slot(to);
        if Self::is_wide(to) || Self::is_wide(self.tys[a as usize]) {
            self.load2(X_A, X_C, a); self.store2(X_A, X_C, s);
        } else {
            self.load(X_A, a); self.store(X_A, s);
        }
        s
    }
    fn sext(&mut self, a: u32, to: IlType) -> u32 {
        // sbfm — encoder lacks it; template via shift-pair (shl to top, asr back).
        let sw = match self.tys[a as usize] { IlType::I{width,..} => width, _ => 64 };
        let s = self.slot(to);
        self.load(X_A, a);
        if sw < 64 {
            self.enc.mov_imm64(X_B, (64 - sw) as u64);
            self.enc.lslv(X_A, X_A, X_B);
            self.enc.asrv(X_A, X_A, X_B);
        }
        if Self::is_wide(to) {
            // sext to i128/u128: hi = sign-fill = asr(lo, 63). Was writing
            // lo-word only → hi-slot left garbage from a prior slot-user →
            // mul's load2 read stale hi → wrong cross-terms → IMUL1 negative
            // gave rdx=0x4 (garbage) not 0xFF..FF. Own #118.
            self.enc.mov_imm64(X_B, 63);
            self.enc.asrv(X_C, X_A, X_B);
            self.store2(X_A, X_C, s);
        } else {
            // Own #119 (tier-0 half): asrv sign-fills to full 64. If to.width
            // < 64, the slot is now typed I{to.width} but holds 0xFF..FF above
            // to.width. Consumers that DON'T re-mask (e.g. a raw reg_write)
            // leak the fill. Mask here so the slot's bits match its type.
            let tw = match to { IlType::I{width,..} => width, _ => 64 };
            if tw < 64 { self.enc.and_lowmask(X_A, X_A, tw as u32); }
            self.store(X_A, s);
        }
        s
    }

    fn fabs(&mut self, a: u32) -> u32 {
        let ty = self.tys[a as usize];
        let s = self.slot(ty);
        let f64 = matches!(ty, IlType::F{width:64});
        self.load(X_A, a);
        if f64 { self.enc.fmov_d_x(0, X_A); self.enc.fabs_d(0, 0); self.enc.fmov_x_d(X_A, 0); }
        else   { self.enc.fmov_s_w(0, X_A); self.enc.fabs_s(0, 0); self.enc.fmov_w_s(X_A, 0); }
        self.store(X_A, s);
        s
    }
    fn fsqrt(&mut self, a: u32) -> u32 {
        let ty = self.tys[a as usize];
        let s = self.slot(ty);
        let f64 = matches!(ty, IlType::F{width:64});
        self.load(X_A, a);
        if f64 { self.enc.fmov_d_x(0, X_A); self.enc.fsqrt_d(0, 0); self.enc.fmov_x_d(X_A, 0); }
        else   { self.enc.fmov_s_w(0, X_A); self.enc.fsqrt_s(0, 0); self.enc.fmov_w_s(X_A, 0); }
        self.store(X_A, s);
        s
    }
    fn fceil(&mut self, _: u32) -> u32 { panic!("tier-0 v1: float ops") }
    fn ffloor(&mut self, _: u32) -> u32 { panic!("tier-0 v1: float ops") }
    fn fisnan(&mut self, a: u32) -> u32 {
        // NaN is unordered vs itself → fcmp a,a sets V=1 iff NaN.
        self.fcmp_op(a, a, Cond::VS)
    }
    fn fminmax(&mut self, a: u32, b: u32, is_max: bool) -> u32 {
        // FCMP a,b → NZCV; FCSEL d0, a, b, cond. cond=GT for MAX (a>b→a,
        // else b — NaN/eq/±0 all → b=src per x86 SDM). cond=MI for MIN
        // (a<b→a, else b — MI=N-set, only ordered-lt sets N; NaN N=0→b).
        let ty = self.tys[a as usize];
        let s = self.slot(ty);
        let f64 = matches!(ty, IlType::F{width:64});
        self.load(X_A, a);
        if f64 { self.enc.fmov_d_x(0, X_A); } else { self.enc.fmov_s_w(0, X_A); }
        self.load(X_A, b);
        if f64 { self.enc.fmov_d_x(1, X_A); } else { self.enc.fmov_s_w(1, X_A); }
        if f64 { self.enc.fcmp_d(0, 1); } else { self.enc.fcmp_s(0, 1); }
        self.enc.fcsel(0, 0, 1, if is_max { Cond::GT } else { Cond::MI }, f64);
        if f64 { self.enc.fmov_x_d(X_A, 0); } else { self.enc.fmov_w_s(X_A, 0); }
        self.store(X_A, s);
        s
    }
    fn fcmpp(&mut self, a: u32, b: u32, pred: u32, w: u32) -> u32 {
        // x86 CMPSS/SD predicate → ARM FCMP+cond, 1:1 (verified against the
        // NZCV table: eq→0110 lt→1000 gt→0010 unord→0011). All 8 preds map
        // to a SINGLE cond — no compound needed:
        //   0 EQ→EQ   1 LT→MI   2 LE→LS   3 UNORD→VS
        //   4 NEQ→NE  5 NLT→PL  6 NLE→HI  7 ORD→VC
        // Then 0/1 → all-0/all-1 mask at width w via neg.
        let cond = match pred & 7 {
            0 => Cond::EQ, 1 => Cond::MI, 2 => Cond::LS, 3 => Cond::VS,
            4 => Cond::NE, 5 => Cond::PL, 6 => Cond::HI, 7 => Cond::VC,
            _ => unreachable!(),
        };
        let s = self.slot(IlType::I{signed:false, width: w as u8});
        let f64 = matches!(self.tys[a as usize], IlType::F{width:64});
        self.load(X_A, a);
        if f64 { self.enc.fmov_d_x(0, X_A); } else { self.enc.fmov_s_w(0, X_A); }
        self.load(X_A, b);
        if f64 { self.enc.fmov_d_x(1, X_A); } else { self.enc.fmov_s_w(1, X_A); }
        if f64 { self.enc.fcmp_d(0, 1); } else { self.enc.fcmp_s(0, 1); }
        self.enc.cset(X_A, cond);
        self.enc.sub_r(X_A, 31, X_A);   // neg: 0→0, 1→0xFF..FF (all-1s)
        if w < 64 { self.enc.and_lowmask(X_A, X_A, w); }
        self.store(X_A, s);
        s
    }
    fn fround(&mut self, _: u32, _: RoundMode) -> u32 { panic!("tier-0 v1: float ops") }
    fn velement_read(&mut self, _: u32, _: u32, _: IlType) -> u32 { panic!("tier-0 v1: vec") }
    fn velement_write(&mut self, _: u32, _: u32, _: u32) -> u32 { panic!("tier-0 v1: vec") }
    fn vzero_top(&mut self, _: u32) -> u32 { panic!("tier-0 v1: vec") }

    fn branch(&mut self, target: u32, link: bool) {
        if link {
            // ‡ aarch64-only (lr=x30, +4). x64 CALL emits push+branch via .isa — tier-0
            //   sees mem_write+branch(link=false), never link=true.
            debug_assert_eq!(self.layout.flag_file, 2, "branch(link=true) is aarch64-only");
            self.enc.ldr_x(X_A, X_STATE, self.layout.off_pc);
            self.enc.add_i(X_A, X_A, 4);
            self.enc.str_x(X_A, X_STATE, (self.layout.reg_off)(RegFile(0), 30));
        }
        self.load(X_A, target);
        self.enc.str_x(X_A, X_STATE, self.layout.off_pc);
        self.branched = true;
    }
    fn cond(&mut self, c: u32, then: &mut dyn FnMut(&mut Self), else_: &mut dyn FnMut(&mut Self)) {
        // Load cond, cbz over then, emit then, b over else, emit else, patch.
        self.load(X_A, c);
        let cbz_at = self.enc.here();
        self.enc.cbz(X_A, 0);            // → else (patched)
        then(self);
        let b_at = self.enc.here();
        self.enc.b(0);                   // → end (patched)
        let else_at = self.enc.here();
        else_(self);
        let end_at = self.enc.here();
        // Patch: cbz offset = (else_at - cbz_at) * 4
        self.enc.patch(cbz_at, 0xB4000000 | ((((else_at - cbz_at) as u32) & 0x7FFFF) << 5) | X_A);
        self.enc.patch(b_at, 0x14000000 | (((end_at - b_at) as u32) & 0x03FFFFFF));
    }
    fn ternary(&mut self, c: u32, a: u32, b: u32) -> u32 {
        let ty = self.tys[a as usize];
        let s = self.slot(ty);
        self.load(X_C, c); self.load(X_A, a); self.load(X_B, b);
        // csel: if c!=0 pick a else b. cmp c, #0; csel x9, x9(a), x10(b), NE.
        self.enc.mov_imm64(12, 0);       // x12 = 0 for cmp
        self.enc.cmp_r(X_C, 12);
        self.enc.csel(X_A, X_A, X_B, Cond::NE);
        self.store(X_A, s);
        s
    }

    fn local_new(&mut self, ty: IlType) -> LocalId { LocalId(self.slot(ty)) }
    fn local_read(&mut self, l: LocalId) -> u32 {
        // Locals ARE spill-slots (same array). Return a fresh slot copied from it
        // (so subsequent local_write doesn't retroactively change earlier reads).
        let ty = self.tys[l.0 as usize];
        let s = self.slot(ty);
        self.load(X_A, l.0); self.store(X_A, s);
        s
    }
    fn local_write(&mut self, l: LocalId, v: u32) {
        self.load(X_A, v); self.store(X_A, l.0);
    }

    fn call_native(&mut self, _: NativeSlot, _: &[u32]) -> Option<u32> { panic!("tier-0 v1: call_native") }
    fn call_intrinsic(&mut self, _: IntrinsicId, _: &[u32]) -> Option<u32> { panic!("tier-0 v1: call_intrinsic") }
    fn unimplemented(&mut self, name: &'static str) { panic!("tier-0: unimplemented insn {name}") }
}

// ─────────────────────────────────────────────────────────────────────────────

/// Shared finalization: mmap RWX, copy code, __clear_cache, wrap as CompiledBlock.
/// Tier-0 and tier-1 both produce an Aarch64Enc + n_slots; this seals it.
pub fn compile_from_enc(enc: Aarch64Enc, n_slots: u32) -> CompiledBlock {
    let words = enc.words();
    unsafe {
        let len = words.len() * 4;
        let page_len = (len + 4095) & !4095;
        let page = libc::mmap(std::ptr::null_mut(), page_len,
            libc::PROT_READ | libc::PROT_WRITE | libc::PROT_EXEC,
            libc::MAP_PRIVATE | libc::MAP_ANONYMOUS, -1, 0) as *mut u32;
        assert!(page as isize != -1, "mmap RWX failed");
        std::ptr::copy_nonoverlapping(words.as_ptr(), page, words.len());
        unsafe extern "C" { fn __clear_cache(s: *const u8, e: *const u8); }
        __clear_cache(page as *const u8, (page as *const u8).add(len));
        CompiledBlock {
            page: page as *mut u8, page_len, code_len: len,
            entry: std::mem::transmute(page),
            n_slots,
            link_sites: vec![], body_off: 0,
        }
    }
}

pub struct CompiledBlock {
    page: *mut u8,
    page_len: usize,
    pub code_len: usize,   // bytes of actual code (< page_len)
    entry: extern "C" fn(*mut u64, *mut u64),
    pub n_slots: u32,
    /// Block-linking (tier-1): (byte_off_of_8B_slot, guest_target_pc) per
    /// const-target exit thunk. Link A→B = write B's body address (page +
    /// body_off) into A's slot — one aligned volatile u64 store (data, not
    /// insn stream → no icache maintenance).
    pub link_sites: Vec<(usize, u64)>,
    /// Byte offset of the post-prologue body (uniform frame contract: chained
    /// entries jump here, reusing the predecessor block's frame). 0 = tier-0
    /// (not chainable).
    pub body_off: usize,
}

impl CompiledBlock {
    /// Dump the block's machine-code bytes (for objdump decode-back verification).
    pub fn code_bytes(&self) -> &[u8] {
        unsafe { std::slice::from_raw_parts(self.page, self.code_len) }
    }
    /// Raw entry fn (for a driver that manages its own shared spill area
    /// instead of exec_slice's per-call vec![] alloc).
    pub fn entry_fn(&self) -> extern "C" fn(*mut u64, *mut u64) { self.entry }
    /// Page base address (block-linking: slot patches + body-address compute).
    pub fn page_addr(&self) -> u64 { self.page as u64 }
    /// Chained-entry address = page + body_off (post-prologue body under the
    /// uniform tier-1 frame contract).
    pub fn body_addr(&self) -> u64 { self.page as u64 + self.body_off as u64 }
    /// Patch one link slot (byte offset from page) to jump to `target_addr`.
    /// Aligned 8-byte data store — the thunk's ldr-literal reads DATA, so no
    /// icache maintenance is required (ARMv8 data-side coherency suffices for
    /// a subsequent ldr on the same PE; cross-thread patching would want
    /// release ordering, which write_volatile+aligned gives us in practice —
    /// single-thread drivers today anyway).
    pub fn patch_link(&self, slot_byte_off: usize, target_addr: u64) {
        debug_assert_eq!(slot_byte_off % 8, 0, "link slot must be 8-aligned");
        unsafe {
            let p = self.page.add(slot_byte_off) as *mut u64;
            std::ptr::write_volatile(p, target_addr);
        }
    }
}

impl CompiledBlock {
    /// Execute against a flat u64[layout.state_words] state array.
    /// Also allocates the spill area (`n_slots` words) on the caller's stack behalf.
    /// Slice-based (not fixed-size array) so aarch64 (68) and x64 (90) both work.
    pub fn exec_slice(&self, flat: &mut [u64]) {
        let mut spill = vec![0u64; self.n_slots as usize + 1];
        (self.entry)(flat.as_mut_ptr(), spill.as_mut_ptr());
    }
    /// Legacy fixed-size aarch64 form.
    pub fn exec(&self, flat: &mut [u64; STATE_WORDS]) {
        let mut spill = vec![0u64; self.n_slots.max(1) as usize];
        (self.entry)(flat.as_mut_ptr(), spill.as_mut_ptr());
    }
}

impl Drop for CompiledBlock {
    fn drop(&mut self) { unsafe { libc::munmap(self.page as *mut _, self.page_len); } }
}
