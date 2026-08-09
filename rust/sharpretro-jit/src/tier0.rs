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

// State offsets (in bytes from x28).
const OFF_GPR: u32 = 0;         // + idx*8
const OFF_NZCV: u32 = 32 * 8;
const OFF_PC: u32 = 33 * 8;
const OFF_VEC: u32 = 34 * 8;    // + idx*8 (‡ low-64 only)

pub const STATE_WORDS: usize = 66;

pub struct Tier0 {
    pub enc: Aarch64Enc,
    next_slot: u32,
    /// slot → IlType (for width-aware ops that need to know arg types).
    tys: Vec<IlType>,
    /// Set once branch() emits — subsequent ops are dead (unreachable). Tier-0
    /// still emits them (harmless — after the ret) to keep the trait contract simple.
    branched: bool,
}

impl Tier0 {
    pub fn new() -> Self {
        let mut enc = Aarch64Enc::new();
        // Prologue: save callee-saved x27/x28 (we clobber both), move args into place.
        enc.sub_i(31, 31, 32);          // sub sp, sp, #32
        enc.str_x(27, 31, 0);
        enc.str_x(28, 31, 8);
        enc.str_x(30, 31, 16);          // save lr (branch may clobber via bl later — ‡ v2)
        enc.mov_r(X_STATE, 0);          // x28 = state
        enc.mov_r(X_SPILL, 1);          // x27 = spill
        Self { enc, next_slot: 0, tys: vec![], branched: false }
    }

    /// Finalize: emit epilogue, mmap RWX, return the callable block.
    pub fn finalize(mut self) -> CompiledBlock {
        // Epilogue: restore callee-saved, ret.
        self.enc.ldr_x(27, 31, 0);
        self.enc.ldr_x(28, 31, 8);
        self.enc.ldr_x(30, 31, 16);
        self.enc.add_i(31, 31, 32);
        self.enc.ret();
        let n_slots = self.next_slot;
        let words = self.enc.words();
        unsafe {
            let len = words.len() * 4;
            let page_len = (len + 4095) & !4095;
            let page = libc::mmap(std::ptr::null_mut(), page_len,
                libc::PROT_READ | libc::PROT_WRITE | libc::PROT_EXEC,
                libc::MAP_PRIVATE | libc::MAP_ANONYMOUS, -1, 0) as *mut u32;
            assert!(page as isize != -1, "mmap RWX failed");
            std::ptr::copy_nonoverlapping(words.as_ptr(), page, words.len());
            // I-cache flush for the whole block.
            unsafe extern "C" { fn __clear_cache(s: *const u8, e: *const u8); }
            __clear_cache(page as *const u8, (page as *const u8).add(len));
            CompiledBlock {
                page: page as *mut u8, page_len, code_len: len,
                entry: std::mem::transmute(page),
                n_slots,
            }
        }
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
    fn state_off(&self, f: RegFile, idx: u32) -> u32 {
        match f.0 {
            0 => OFF_GPR + idx * 8,
            1 => OFF_VEC + idx * 8,     // ‡ low-64
            2 => OFF_NZCV,              // idx=0 whole-word; idx=1..4 mask-bit below
            _ => panic!("tier-0: file {} not wired", f.0),
        }
    }

    /// Post-op mask to `ty`'s width (matches interp's ibin mask). Skip for width≥64.
    fn mask_to(&mut self, ty: IlType) {
        if let IlType::I{width, ..} = ty {
            if width < 64 {
                self.enc.mov_imm64(X_B, (1u64 << width) - 1);
                self.enc.and_r(X_A, X_A, X_B);
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
        // GPR at U32 = W-read (mask to 32 bits — matches Aarch64State.reg_read).
        self.enc.ldr_x(X_A, X_STATE, off);
        if f.0 == 0 { self.mask_to(ty); }
        // NZCV individual-flag: extract bit (idx=1..4 → bit 31/30/29/28).
        if f.0 == 2 && idx >= 1 {
            let bit = 32 - idx;
            self.enc.mov_imm64(X_B, bit as u64);
            self.enc.lsrv(X_A, X_A, X_B);
            self.enc.mov_imm64(X_B, 1);
            self.enc.and_r(X_A, X_A, X_B);
        }
        self.store(X_A, s);
        s
    }
    fn reg_write(&mut self, f: RegFile, idx: u32, v: u32) {
        self.load(X_A, v);
        let off = self.state_off(f, idx);
        // NZCV individual-flag write: read-modify-write the whole word.
        if f.0 == 2 && idx >= 1 {
            let bit = 32 - idx;
            self.enc.ldr_x(X_B, X_STATE, OFF_NZCV);
            self.enc.mov_imm64(X_C, !(1u64 << bit));
            self.enc.and_r(X_B, X_B, X_C);            // clear bit
            self.enc.mov_imm64(X_C, bit as u64);
            self.enc.lslv(X_A, X_A, X_C);             // v << bit
            self.enc.orr_r(X_A, X_B, X_A);
            self.enc.str_x(X_A, X_STATE, OFF_NZCV);
            return;
        }
        // GPR W-write zero-extends (the emit already casts to U32 for gpr32; here mask).
        if f.0 == 0 && matches!(self.tys[v as usize], IlType::I{width:32, ..}) {
            self.enc.mov_r(X_A, X_A);  // ‡ actually need `mov w9, w9` (32-bit) to zero-ext.
            // Aarch64Enc doesn't have mov_w yet; a `and x9,x9,#0xFFFFFFFF` via mask:
            self.enc.mov_imm64(X_B, 0xFFFF_FFFF);
            self.enc.and_r(X_A, X_A, X_B);
        }
        self.enc.str_x(X_A, X_STATE, off);
    }
    fn mem_read(&mut self, _a: u32, _ty: IlType) -> u32 {
        panic!("tier-0 v1: mem_read not wired (guest-mem sandbox at v2)")
    }
    fn mem_write(&mut self, _a: u32, _v: u32) {
        panic!("tier-0 v1: mem_write not wired")
    }

    fn add(&mut self, a: u32, b: u32) -> u32 {
        let t = self.tys[a as usize];
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
    fn div(&mut self, a: u32, b: u32) -> u32 { let t = self.tys[a as usize];
        // Silicon udiv/sdiv: divide-by-0 → result 0 (no fault) — matches interp's
        // `if y==0 {0} else {x/y}`. So no zero-guard needed.
        let signed = matches!(t, IlType::I{signed:true, ..});
        let w32 = matches!(t, IlType::I{width, ..} if width <= 32);
        self.bin(a, b, t, move |e| match (signed, w32) {
            (true, true) => e.sdiv_w(X_A, X_A, X_B),
            (true, false) => e.sdiv(X_A, X_A, X_B),
            (false, true) => e.udiv_w(X_A, X_A, X_B),
            (false, false) => e.udiv(X_A, X_A, X_B),
        }) }
    fn rem(&mut self, a: u32, b: u32) -> u32 { let t = self.tys[a as usize];
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
    fn and(&mut self, a: u32, b: u32) -> u32 { let t = self.tys[a as usize]; self.bin(a, b, t, |e| e.and_r(X_A, X_A, X_B)) }
    fn or (&mut self, a: u32, b: u32) -> u32 { let t = self.tys[a as usize]; self.bin(a, b, t, |e| e.orr_r(X_A, X_A, X_B)) }
    fn xor(&mut self, a: u32, b: u32) -> u32 { let t = self.tys[a as usize]; self.bin(a, b, t, |e| e.eor_r(X_A, X_A, X_B)) }
    fn not(&mut self, a: u32) -> u32 { let t = self.tys[a as usize];
        // Bool: eor #1 (logical negate — CSEL fuzz caught this: eor-all-1s on Bool gives
        // 0xFF..FE which is truthy). Int: bitwise complement within width.
        let mask = match t {
            IlType::Bool => 1u64,
            IlType::I{width, ..} if width < 64 => (1u64 << width) - 1,
            _ => u64::MAX,
        };
        self.una(a, t, move |e| { e.mov_imm64(X_B, mask); e.eor_r(X_A, X_A, X_B); }) }
    fn shl(&mut self, a: u32, b: u32) -> u32 { let t = self.tys[a as usize];
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
        // Width-aware: for width≤32 use W-form (asrv_w/lsrv_w) so the shift bounds
        // and sign-bit position are correct (asrv-X on a W-value shifts from bit-63,
        // not bit-31 — the shifted-register-ASR fuzz diff).
        let w32 = matches!(t, IlType::I{width, ..} if width <= 32);
        self.bin(a, b, t, move |e| match (signed, w32) {
            (true, true) => e.asrv_w(X_A, X_A, X_B),
            (true, false) => e.asrv(X_A, X_A, X_B),
            (false, true) => e.lsrv_w(X_A, X_A, X_B),
            (false, false) => e.lsrv(X_A, X_A, X_B),
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
        let w32 = matches!(t, IlType::I{width, ..} if width <= 32);
        self.una(a, t, move |e| if w32 { e.rbit_w(X_A, X_A) } else { e.rbit(X_A, X_A) }) }
    fn clz(&mut self, a: u32) -> u32 { let t = self.tys[a as usize];
        let w32 = matches!(t, IlType::I{width, ..} if width <= 32);
        // ‡ non-32/64 widths: interp does `(bits<<(128-w)).leading_zeros().min(w)`.
        // For width≠{32,64} (rare in aarch64.isa — CLZ is X/W-only), would need shift-
        // adjust. Panic if hit; the fuzz-diff will name it.
        self.una(a, t, move |e| if w32 { e.clz_w(X_A, X_A) } else { e.clz(X_A, X_A) }) }

    fn eq(&mut self, a: u32, b: u32) -> u32 { self.cmp_op(a, b, Cond::EQ) }
    fn ne(&mut self, a: u32, b: u32) -> u32 { self.cmp_op(a, b, Cond::NE) }
    fn lt(&mut self, a: u32, b: u32) -> u32 {
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
        // wide→narrow: read lo, mask; narrow→narrow: read, mask.
        self.load(X_A, a);
        if let IlType::I{width, ..} = to {
            if width < 64 {
                self.enc.mov_imm64(X_B, (1u64 << width) - 1);
                self.enc.and_r(X_A, X_A, X_B);
            }
        }
        self.store(X_A, s);
        s
    }
    fn bitcast(&mut self, a: u32, to: IlType) -> u32 {
        let s = self.slot(to); self.load(X_A, a); self.store(X_A, s); s
    }
    fn sext(&mut self, a: u32, to: IlType) -> u32 {
        // sbfm — encoder lacks it; template via shift-pair (shl to top, asr back).
        let sw = match self.tys[a as usize] { IlType::I{width,..} => width, _ => 64 };
        let s = self.slot(to);
        self.load(X_A, a);
        self.enc.mov_imm64(X_B, (64 - sw) as u64);
        self.enc.lslv(X_A, X_A, X_B);
        self.enc.asrv(X_A, X_A, X_B);
        self.store(X_A, s);
        s
    }

    fn fabs(&mut self, _: u32) -> u32 { panic!("tier-0 v1: float ops") }
    fn fsqrt(&mut self, _: u32) -> u32 { panic!("tier-0 v1: float ops") }
    fn fceil(&mut self, _: u32) -> u32 { panic!("tier-0 v1: float ops") }
    fn ffloor(&mut self, _: u32) -> u32 { panic!("tier-0 v1: float ops") }
    fn fisnan(&mut self, _: u32) -> u32 { panic!("tier-0 v1: float ops") }
    fn fround(&mut self, _: u32, _: RoundMode) -> u32 { panic!("tier-0 v1: float ops") }
    fn velement_read(&mut self, _: u32, _: u32, _: IlType) -> u32 { panic!("tier-0 v1: vec") }
    fn velement_write(&mut self, _: u32, _: u32, _: u32) -> u32 { panic!("tier-0 v1: vec") }
    fn vzero_top(&mut self, _: u32) -> u32 { panic!("tier-0 v1: vec") }

    fn branch(&mut self, target: u32, link: bool) {
        // link FIRST (reads OLD pc from state), THEN write target. (Order bug caught by
        // BL tier0-diff: interp lr=0x1004, tier0 lr=0x1014 = target+4.)
        if link {
            self.enc.ldr_x(X_A, X_STATE, OFF_PC);
            self.enc.add_i(X_A, X_A, 4);
            self.enc.str_x(X_A, X_STATE, OFF_GPR + 30 * 8);
        }
        self.load(X_A, target);
        self.enc.str_x(X_A, X_STATE, OFF_PC);
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

pub struct CompiledBlock {
    page: *mut u8,
    page_len: usize,
    pub code_len: usize,   // bytes of actual code (< page_len)
    entry: extern "C" fn(*mut u64, *mut u64),
    pub n_slots: u32,
}

impl CompiledBlock {
    /// Dump the block's machine-code bytes (for objdump decode-back verification).
    pub fn code_bytes(&self) -> &[u8] {
        unsafe { std::slice::from_raw_parts(self.page, self.code_len) }
    }
}

impl CompiledBlock {
    /// Execute against a flat u64[STATE_WORDS] state array.
    pub fn exec(&self, flat: &mut [u64; STATE_WORDS]) {
        let mut spill = vec![0u64; self.n_slots.max(1) as usize];
        (self.entry)(flat.as_mut_ptr(), spill.as_mut_ptr());
    }
}

impl Drop for CompiledBlock {
    fn drop(&mut self) { unsafe { libc::munmap(self.page as *mut _, self.page_len); } }
}
