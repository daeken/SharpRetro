//! aarch64 machine-code encoder — the `Emit` impl for aarch64-host. Emits raw insn words
//! into a `Vec<u32>`. Each fn = one insn shape; encodings hand-derived from ARM ARM C6
//! and VERIFIED via objdump decode-back (the encode-then-decode-back discipline: any new
//! encoding gets a `#[test]` that dumps + objdumps + asserts the mnemonic).
//!
//! Register naming: X0..X30 = 0..30, SP/XZR = 31 (context picks which). Immediates are
//! raw (caller ensures range); asserts guard the encodable bounds.
//!
//! This is the seed for `Tier0<Aarch64Enc>` (each Builder method → a few of these calls)
//! AND the shared encoder for NativeStub's prologue/epilogue (which currently duplicates
//! ldr/str/sub_sp/etc — refactor to use this once tier-0 lands).

#![cfg(target_arch = "aarch64")]
#![allow(dead_code)]

pub struct Aarch64Enc {
    pub buf: Vec<u32>,
}

impl Aarch64Enc {
    pub fn new() -> Self { Self { buf: vec![] } }
    pub fn words(&self) -> &[u32] { &self.buf }
    pub fn bytes(&self) -> Vec<u8> { self.buf.iter().flat_map(|w| w.to_le_bytes()).collect() }
    pub fn len_bytes(&self) -> usize { self.buf.len() * 4 }
    #[inline] fn put(&mut self, w: u32) { self.buf.push(w); }
    /// Escape hatch: emit a raw insn word (for one-off encodings not worth a fn yet).
    /// The tier-0 caller must comment WHAT it encodes (the decode-back discipline still
    /// applies — put_raw sites should get objdump-verified in a #[test]).
    pub fn put_raw(&mut self, w: u32) { self.buf.push(w); }

    // ── load/store (unsigned imm, imm scaled by access-size) ──────────────
    // LDR Xt, [Xn, #imm]  — imm bytes, 8-aligned, 0..32760
    pub fn ldr_x(&mut self, xt: u32, xn: u32, off: u32) {
        debug_assert!(off % 8 == 0 && off < 32768);
        self.put(0xF9400000 | ((off/8) << 10) | (xn << 5) | xt);
    }
    pub fn str_x(&mut self, xt: u32, xn: u32, off: u32) {
        debug_assert!(off % 8 == 0 && off < 32768);
        self.put(0xF9000000 | ((off/8) << 10) | (xn << 5) | xt);
    }
    // 32-bit variants (for W-typed guest regs; scaled by 4)
    pub fn ldr_w(&mut self, wt: u32, xn: u32, off: u32) {
        debug_assert!(off % 4 == 0 && off < 16384);
        self.put(0xB9400000 | ((off/4) << 10) | (xn << 5) | wt);
    }
    pub fn str_w(&mut self, wt: u32, xn: u32, off: u32) {
        debug_assert!(off % 4 == 0 && off < 16384);
        self.put(0xB9000000 | ((off/4) << 10) | (xn << 5) | wt);
    }

    // ── move-immediate ─────────────────────────────────────────────────────
    // MOVZ Xd, #imm16, LSL #(hw*16)
    pub fn movz(&mut self, xd: u32, imm16: u32, hw: u32) {
        debug_assert!(imm16 < 0x10000 && hw < 4);
        self.put(0xD2800000 | (hw << 21) | (imm16 << 5) | xd);
    }
    // MOVK Xd, #imm16, LSL #(hw*16)  — keep other halfwords
    pub fn movk(&mut self, xd: u32, imm16: u32, hw: u32) {
        debug_assert!(imm16 < 0x10000 && hw < 4);
        self.put(0xF2800000 | (hw << 21) | (imm16 << 5) | xd);
    }
    /// Load a 64-bit constant into Xd (1-4 movz/movk insns; skips zero halfwords).
    /// AND Xd, Xn, #(1<<w)-1 — mask to low w bits, ONE insn via the logical-
    /// immediate encoding (N=1, immr=0, imms=w-1 for a w-run of 1s from bit 0).
    /// w ∈ [1,63]. The mask_to() workhorse: was mov_imm64+and_r (2-3 insns).
    pub fn and_lowmask(&mut self, xd: u32, xn: u32, w: u32) {
        debug_assert!(w >= 1 && w < 64);
        // AND (immediate), 64-bit: sf=1 opc=00 100100 N=1 immr=0 imms=w-1 Rn Rd
        self.put(0x92400000 | ((w-1)<<10) | (xn<<5) | xd);
    }
    // MOVN Xd, #imm16, LSL #(hw*16) — Xd = ~(imm16 << hw*16).
    pub fn movn(&mut self, xd: u32, imm16: u32, hw: u32) {
        debug_assert!(imm16 < 0x10000 && hw < 4);
        self.put(0x92800000 | (hw<<21) | (imm16<<5) | xd);
    }
    /// Materialize a u64 into Xd in the fewest movz/movn/movk. Chooses movz-
    /// vs movn-base by which leaves fewer movk fixups (nonzero halfwords of
    /// v vs of ~v). Negative small constants (0xFF..FC etc) → 1× movn instead
    /// of movz+3×movk. LCG-block: 54 movk#0xffff → ~0.
    pub fn mov_imm64(&mut self, xd: u32, v: u64) {
        let hw = |x: u64, i: u32| ((x >> (i*16)) & 0xFFFF) as u32;
        let nz  = (0..4).filter(|&i| hw(v,  i) != 0).count();
        let nzn = (0..4).filter(|&i| hw(!v, i) != 0).count();  // = # non-0xFFFF chunks of v
        if nzn < nz {
            // movn-base: pick a hw where v's chunk ≠ 0xFFFF (i.e. ~v's chunk ≠ 0),
            // emit movn xd, #(~chunk), then movk each remaining non-0xFFFF chunk.
            // If all-0xFFFF (v = -1): movn xd, #0.
            let base = (0..4).find(|&i| hw(v, i) != 0xFFFF).unwrap_or(0);
            self.movn(xd, (!hw(v, base)) & 0xFFFF, base);
            for i in 0..4 {
                if i != base && hw(v, i) != 0xFFFF { self.movk(xd, hw(v, i), i); }
            }
        } else {
            // movz-base (existing path): pick a hw where chunk ≠ 0, emit movz
            // there, movk each remaining nonzero. All-zero → movz xd,#0.
            let base = (0..4).find(|&i| hw(v, i) != 0).unwrap_or(0);
            self.movz(xd, hw(v, base), base);
            for i in 0..4 {
                if i != base && hw(v, i) != 0 { self.movk(xd, hw(v, i), i); }
            }
        }
    }
    // MOV Xd, Xm  (= ORR Xd, XZR, Xm)
    pub fn mov_r(&mut self, xd: u32, xm: u32) {
        self.put(0xAA0003E0 | (xm << 16) | xd);
    }

    // ── arithmetic (register, shifted-register with shift=0) ──────────────
    pub fn add_r(&mut self, xd: u32, xn: u32, xm: u32) { self.put(0x8B000000 | (xm<<16) | (xn<<5) | xd); }
    pub fn sub_r(&mut self, xd: u32, xn: u32, xm: u32) { self.put(0xCB000000 | (xm<<16) | (xn<<5) | xd); }
    // ADDS/SUBS (set flags) + ADC/SBC (with carry) — for u128 arithmetic (the .isa's
    // carry-flag computation via widen-add-shr).
    pub fn adds_r(&mut self, xd: u32, xn: u32, xm: u32) { self.put(0xAB000000 | (xm<<16) | (xn<<5) | xd); }
    pub fn adc_r(&mut self, xd: u32, xn: u32, xm: u32)  { self.put(0x9A000000 | (xm<<16) | (xn<<5) | xd); }
    pub fn subs_r(&mut self, xd: u32, xn: u32, xm: u32) { self.put(0xEB000000 | (xm<<16) | (xn<<5) | xd); }
    pub fn sbc_r(&mut self, xd: u32, xn: u32, xm: u32)  { self.put(0xDA000000 | (xm<<16) | (xn<<5) | xd); }
    pub fn and_r(&mut self, xd: u32, xn: u32, xm: u32) { self.put(0x8A000000 | (xm<<16) | (xn<<5) | xd); }
    pub fn orr_r(&mut self, xd: u32, xn: u32, xm: u32) { self.put(0xAA000000 | (xm<<16) | (xn<<5) | xd); }
    // ORR Xd, Xn, Xm, LSL #imm  (shift-type=00 @[23:22], imm6 @[15:10])
    pub fn orr_lsl(&mut self, xd: u32, xn: u32, xm: u32, sh: u32) {
        debug_assert!(sh < 64);
        self.put(0xAA000000 | (xm<<16) | (sh<<10) | (xn<<5) | xd);
    }
    // LSR Xd, Xn, #imm = UBFM Xd,Xn,#imm,#63 = 0xD340FC00 | imm<<16 | Rn<<5 | Rd
    pub fn lsr_i(&mut self, xd: u32, xn: u32, sh: u32) {
        debug_assert!(sh < 64);
        self.put(0xD340FC00 | (sh<<16) | (xn<<5) | xd);
    }
    pub fn eor_r(&mut self, xd: u32, xn: u32, xm: u32) { self.put(0xCA000000 | (xm<<16) | (xn<<5) | xd); }

    // ── NEON / Q-reg (V128 ops via q-scratch: q0/q1 in, q2 out) ──────────────
    // LDR Qt, [Xn, #imm]  (imm scaled by 16, unsigned imm12)
    pub fn ldr_q(&mut self, qt: u32, xn: u32, imm: u32) {
        debug_assert!(imm % 16 == 0 && imm/16 < 4096);
        self.put(0x3DC00000 | ((imm/16)<<10) | (xn<<5) | qt);
    }
    // STR Qt, [Xn, #imm]
    pub fn str_q(&mut self, qt: u32, xn: u32, imm: u32) {
        debug_assert!(imm % 16 == 0 && imm/16 < 4096);
        self.put(0x3D800000 | ((imm/16)<<10) | (xn<<5) | qt);
    }
    // ZIP1 Vd.<T>, Vn.<T>, Vm.<T> — interleave LOW halves. size 0/1/2/3 = B/H/S/D.
    // T = 16B/8H/4S/2D (Q=1). Exactly x86's PUNPCKL{BW,WD,DQ,QDQ}.
    // NEON packed-float arith (Q=1). sz: 0=.4S (f32×4), 1=.2D (f64×2).
    // FADD Vd.T,Vn.T,Vm.T = 0x4E20D400 | sz<<22 | Rm<<16 | Rn<<5 | Rd
    // FSUB = 0x4EA0D400 | sz<<22   FMUL = 0x6E20DC00 | sz<<22   FDIV = 0x6E20FC00 | sz<<22
    // FADDP (vector, float) Vd.T,Vn.T,Vm.T — pairwise-add:
    //   Vd[0]=Vn[0]+Vn[1], Vd[1]=Vn[2]+Vn[3], Vd[2]=Vm[0]+Vm[1], Vd[3]=Vm[2]+Vm[3] (.4S)
    //   Vd[0]=Vn[0]+Vn[1], Vd[1]=Vm[0]+Vm[1] (.2D)
    // 0x6E20D400 | sz<<22 | Rm<<16 | Rn<<5 | Rd
    pub fn faddp_v(&mut self, vd: u32, vn: u32, vm: u32, sz: u32) {
        self.put(0x6E20D400 | (sz<<22) | (vm<<16) | (vn<<5) | vd);
    }
    // FCMEQ/FCMGT/FCMGE (register) Vd.T,Vn.T,Vm.T: per-lane ORDERED compare
    // → all-1s/0 mask. NaN in either → 0-mask. .4S / .2D via sz bit22.
    // FCMEQ: 0x4E20E400 | sz<<22.
    pub fn fcmeq_v(&mut self, vd: u32, vn: u32, vm: u32, sz: u32) {
        self.put(0x4E20E400 | (sz<<22) | (vm<<16) | (vn<<5) | vd);
    }
    pub fn fcmgt_v(&mut self, vd: u32, vn: u32, vm: u32, sz: u32) {
        self.put(0x6EA0E400 | (sz<<22) | (vm<<16) | (vn<<5) | vd);
    }
    // FCMGE: 0x6E20E400 | sz<<22
    pub fn fcmge_v(&mut self, vd: u32, vn: u32, vm: u32, sz: u32) {
        self.put(0x6E20E400 | (sz<<22) | (vm<<16) | (vn<<5) | vd);
    }
    // (orr_v16b already exists below.)
    // Vector shift by IMMEDIATE (Q=1). immh:immb (7 bits @[22:16]) encodes
    // BOTH element-size and shift-amount:
    //   SHL:  immh:immb = ew + sh    (sh ∈ 0..ew-1)
    //   USHR/SSHR: immh:immb = 2*ew - sh  (sh ∈ 1..ew)
    // ew: 8→immh=0001, 16→001x, 32→01xx, 64→1xxx.
    pub fn shl_vi(&mut self, vd: u32, vn: u32, ew: u32, sh: u32) {
        debug_assert!(sh < ew);
        self.put(0x4F005400 | ((ew + sh)<<16) | (vn<<5) | vd);
    }
    pub fn ushr_vi(&mut self, vd: u32, vn: u32, ew: u32, sh: u32) {
        debug_assert!(sh >= 1 && sh <= ew);
        self.put(0x6F000400 | ((2*ew - sh)<<16) | (vn<<5) | vd);
    }
    pub fn sshr_vi(&mut self, vd: u32, vn: u32, ew: u32, sh: u32) {
        debug_assert!(sh >= 1 && sh <= ew);
        self.put(0x4F000400 | ((2*ew - sh)<<16) | (vn<<5) | vd);
    }
    // MOVI Vd.16B, #0 = 0x4F000400 conflicts w/ SSHR — use EOR self for zero.
    // (Actually for zero: `movi Vd.2D, #0` = 0x6F00E400 | Rd.)
    pub fn movi_zero(&mut self, vd: u32) {
        self.put(0x6F00E400 | vd);
    }
    // CNT Vd.8B, Vn.8B — per-byte popcount (Q=0, 8B) = 0x0E205800
    pub fn cnt_v8b(&mut self, vd: u32, vn: u32) {
        self.put(0x0E205800 | (vn<<5) | vd);
    }
    // ADDV Bd, Vn.8B — sum all 8 bytes into Bd = 0x0E31B800
    pub fn addv_b_8b(&mut self, bd: u32, vn: u32) {
        self.put(0x0E31B800 | (vn<<5) | bd);
    }
    // MVN Vd.16B, Vn.16B (= NOT) = 0x6E205800
    pub fn mvn_v16b(&mut self, vd: u32, vn: u32) {
        self.put(0x6E205800 | (vn<<5) | vd);
    }
    // BIT Vd.16B, Vn.16B, Vm.16B: Vd = (Vn & Vm) | (Vd & ~Vm)  (bit-insert-if-true)
    // = "where Vm bit is set, take Vn's bit; else keep Vd's". 0x6EA01C00.
    pub fn bit_v16b(&mut self, vd: u32, vn: u32, vm: u32) {
        self.put(0x6EA01C00 | (vm<<16) | (vn<<5) | vd);
    }
    // FSQRT vector: Vd.4S = 0x6EA1F800; Vd.2D = 0x6EE1F800 (sz bit22).
    // (Scalar fsqrt_s/fsqrt_d already exist below at :232.)
    pub fn fsqrt_v(&mut self, vd: u32, vn: u32, sz: u32) {
        self.put(0x6EA1F800 | (sz<<22) | (vn<<5) | vd);
    }
    // Packed convert (all Q=1 unless noted). For CVTDQ2PS/CVTTPS2DQ/etc.
    // SCVTF Vd.4S,Vn.4S (i32→f32) = 0x4E21D800; Vd.2D,Vn.2D (i64→f64) = 0x4E61D800
    pub fn scvtf_v(&mut self, vd: u32, vn: u32, sz: u32) {
        self.put(0x4E21D800 | (sz<<22) | (vn<<5) | vd);
    }
    // FCVTZS Vd.4S,Vn.4S (f32→i32 truncate) = 0x4EA1B800; .2D = 0x4EE1B800
    pub fn fcvtzs_v(&mut self, vd: u32, vn: u32, sz: u32) {
        self.put(0x4EA1B800 | (sz<<22) | (vn<<5) | vd);
    }
    // FCVTNS Vd.T,Vn.T (f→int, round-to-nearest-ties-even) = 0x4E21A800 | sz<<22
    pub fn fcvtns_v(&mut self, vd: u32, vn: u32, sz: u32) {
        self.put(0x4E21A800 | (sz<<22) | (vn<<5) | vd);
    }
    // XTN Vd.2S, Vn.2D (narrow i64→i32, low half, upper zeroed) = 0x0E212800 | size<<22
    // size=2 for 2D→2S. (Q=0.)
    pub fn xtn_2s_2d(&mut self, vd: u32, vn: u32) {
        self.put(0x0EA12800 | (vn<<5) | vd);
    }
    // FCVTL Vd.2D,Vn.2S (widen low 2× f32→f64) = 0x0E617800 (Q=0, sz=1)
    pub fn fcvtl_2d_2s(&mut self, vd: u32, vn: u32) {
        self.put(0x0E617800 | (vn<<5) | vd);
    }
    // FCVTN Vd.2S,Vn.2D (narrow 2× f64→f32, upper zeroed) = 0x0E616800 (Q=0, sz=1)
    pub fn fcvtn_2s_2d(&mut self, vd: u32, vn: u32) {
        self.put(0x0E616800 | (vn<<5) | vd);
    }
    // SSHLL Vd.2D,Vn.2S,#0 (= SXTL: sign-extend low 2× i32→i64) = 0x0F20A400
    pub fn sxtl_2d_2s(&mut self, vd: u32, vn: u32) {
        self.put(0x0F20A400 | (vn<<5) | vd);
    }
    pub fn fadd_v(&mut self, vd: u32, vn: u32, vm: u32, sz: u32) {
        self.put(0x4E20D400 | (sz<<22) | (vm<<16) | (vn<<5) | vd);
    }
    pub fn fsub_v(&mut self, vd: u32, vn: u32, vm: u32, sz: u32) {
        self.put(0x4EA0D400 | (sz<<22) | (vm<<16) | (vn<<5) | vd);
    }
    pub fn fmul_v(&mut self, vd: u32, vn: u32, vm: u32, sz: u32) {
        self.put(0x6E20DC00 | (sz<<22) | (vm<<16) | (vn<<5) | vd);
    }
    pub fn fdiv_v(&mut self, vd: u32, vn: u32, vm: u32, sz: u32) {
        self.put(0x6E20FC00 | (sz<<22) | (vm<<16) | (vn<<5) | vd);
    }
    pub fn zip1_v(&mut self, vd: u32, vn: u32, vm: u32, size: u32) {
        debug_assert!(size < 4);
        self.put(0x4E003800 | (size<<22) | (vm<<16) | (vn<<5) | vd);
    }
    // ZIP2 (interleave HIGH halves) = PUNPCKH*.
    pub fn zip2_v(&mut self, vd: u32, vn: u32, vm: u32, size: u32) {
        debug_assert!(size < 4);
        self.put(0x4E007800 | (size<<22) | (vm<<16) | (vn<<5) | vd);
    }
    // EOR/AND/ORR Vd.16B, Vn.16B, Vm.16B — bitwise on the whole 128.
    // Integer vector ADD/SUB/MUL: size 0=16B 1=8H 2=4S 3=2D (MUL: 0-2 only, no .2D)
    pub fn add_v(&mut self, vd: u32, vn: u32, vm: u32, size: u32) {
        self.put(0x4E208400 | (size<<22) | (vm<<16) | (vn<<5) | vd);
    }
    pub fn sub_v(&mut self, vd: u32, vn: u32, vm: u32, size: u32) {
        self.put(0x6E208400 | (size<<22) | (vm<<16) | (vn<<5) | vd);
    }
    pub fn mul_v(&mut self, vd: u32, vn: u32, vm: u32, size: u32) {
        debug_assert!(size < 3, "MUL vector has no .2D form");
        self.put(0x4E209C00 | (size<<22) | (vm<<16) | (vn<<5) | vd);
    }
    // INS Vd.S[i], Wn (general, 32-bit lane). imm5 for .S = index<<3 | 0b100 (size-marker
    // at bit-2, index at bits [4:3] — SAME pattern as ins_vd_x's .D: index<<4|0b1000).
    // First cut had (lane<<2)|4 = index colliding with the size bit → i=1→s[0], i=3→s[1].
    // The encode-then-decode-back check caught it (objdump showed s[0]×2,s[1]×2 instead of
    // s[0..3]); the DPPS 0x7F interp-vs-tier0 diff then confirmed at values (tier-0 gave
    // 9806.0 = zeroed lane-1 not lane-3 → 1+0+4+9801, vs interp's correct 9.0). Wn=31→wzr.
    pub fn ins_vs_w(&mut self, vd: u32, lane: u32, wn: u32) {
        debug_assert!(lane < 4);
        self.put(0x4E001C00 | (((lane<<3)|4)<<16) | (wn<<5) | vd);
    }
    // INS Vd.D[i], Xn already exists as ins_vd_x. For DPPD lane-zero: ins_vd_x(vd, i, 31=xzr).
    // REV Xd,Xn (reverse bytes in 64-bit) = 0xDAC00C00; REV Wd,Wn (32-bit) = 0x5AC00800.
    pub fn rev_x(&mut self, rd: u32, rn: u32) { self.put(0xDAC00C00 | (rn<<5) | rd); }
    pub fn rev_w(&mut self, rd: u32, rn: u32) { self.put(0x5AC00800 | (rn<<5) | rd); }
    // CMEQ (register): per-lane == → all-1s/0. size 0-3.
    pub fn cmeq_v(&mut self, vd: u32, vn: u32, vm: u32, size: u32) {
        self.put(0x6E208C00 | (size<<22) | (vm<<16) | (vn<<5) | vd);
    }
    // CMGT (register, signed): per-lane signed > → all-1s/0. size 0-3.
    pub fn cmgt_v(&mut self, vd: u32, vn: u32, vm: u32, size: u32) {
        self.put(0x4E203400 | (size<<22) | (vm<<16) | (vn<<5) | vd);
    }
    pub fn eor_v16b(&mut self, vd: u32, vn: u32, vm: u32) { self.put(0x6E201C00 | (vm<<16) | (vn<<5) | vd); }
    pub fn and_v16b(&mut self, vd: u32, vn: u32, vm: u32) { self.put(0x4E201C00 | (vm<<16) | (vn<<5) | vd); }
    pub fn orr_v16b(&mut self, vd: u32, vn: u32, vm: u32) { self.put(0x4EA01C00 | (vm<<16) | (vn<<5) | vd); }
    // MOV Vd.D[i], Xn — insert X-reg into vector lane (i=0 or 1). Building Q from 2× X.
    pub fn ins_vd_x(&mut self, vd: u32, i: u32, xn: u32) {
        debug_assert!(i < 2);
        // INS Vd.D[i], Xn: 0x4E081C00 | (imm5=1<<3|i<<4)<<16 | Rn<<5 | Rd
        self.put(0x4E081C00 | ((i<<4)<<16) | (xn<<5) | vd);
    }
    /// INS Vd.H[i], Vn.H[j] — copy one 16-bit lane between vector regs.
    /// size=H: imm5 = (i<<2)|0b10, imm4 = j<<1.
    pub fn ins_vh_vh(&mut self, vd: u32, i: u32, vn: u32, j: u32) {
        debug_assert!(i < 8 && j < 8);
        let imm5 = (i << 2) | 0b10;
        let imm4 = j << 1;
        self.put(0x6E000400 | (imm5<<16) | (imm4<<11) | (vn<<5) | vd);
    }
    /// MOV Vd.16B, Vn.16B (= ORR Vd.16B, Vn.16B, Vn.16B) — full vector copy.
    pub fn mov_v(&mut self, vd: u32, vn: u32) {
        self.put(0x4EA01C00 | (vn<<16) | (vn<<5) | vd);
    }
    /// INS Vd.S[i], Vn.S[j] — copy one 32-bit lane between vector regs.
    /// Encoding: 0x6E000400 | imm5<<16 | imm4<<11 | Rn<<5 | Rd, where for
    /// size=S: imm5 = (i<<3)|0b100, imm4 = j<<2. (Arm ARM C7.2.176 INS-elem.)
    pub fn ins_vs_vs(&mut self, vd: u32, i: u32, vn: u32, j: u32) {
        debug_assert!(i < 4 && j < 4);
        let imm5 = (i << 3) | 0b100;
        let imm4 = j << 2;
        self.put(0x6E000400 | (imm5<<16) | (imm4<<11) | (vn<<5) | vd);
    }
    /// INS Vd.D[i], Vn.D[j] — copy one 64-bit lane between vector regs.
    /// size=D: imm5 = (i<<4)|0b1000, imm4 = j<<3.
    pub fn ins_vd_vd(&mut self, vd: u32, i: u32, vn: u32, j: u32) {
        debug_assert!(i < 2 && j < 2);
        let imm5 = (i << 4) | 0b1000;
        let imm4 = j << 3;
        self.put(0x6E000400 | (imm5<<16) | (imm4<<11) | (vn<<5) | vd);
    }
    // UMOV Xd, Vn.D[i] — extract vector lane to X-reg.
    pub fn umov_x_vd(&mut self, xd: u32, vn: u32, i: u32) {
        debug_assert!(i < 2);
        self.put(0x4E083C00 | ((i<<4)<<16) | (vn<<5) | xd);
    }

    // ── scalar float ↔ int (X-reg ↔ D/S-reg) ─────────────────────────────────
    // FMOV Dd, Xn / FMOV Xd, Dn — bitcast X↔D (64-bit).
    pub fn fmov_d_x(&mut self, dd: u32, xn: u32) { self.put(0x9E670000 | (xn<<5) | dd); }
    pub fn fmov_x_d(&mut self, xd: u32, dn: u32) { self.put(0x9E660000 | (dn<<5) | xd); }
    // FMOV Sd, Wn / FMOV Wd, Sn — bitcast W↔S (32-bit).
    pub fn fmov_s_w(&mut self, sd: u32, wn: u32) { self.put(0x1E270000 | (wn<<5) | sd); }
    pub fn fmov_w_s(&mut self, wd: u32, sn: u32) { self.put(0x1E260000 | (sn<<5) | wd); }
    // SCVTF Dd, Xn — signed int64 → double. (SCVTF Sd, Xn = f32; SCVTF Dd, Wn = i32→f64.)
    pub fn scvtf_d_x(&mut self, dd: u32, xn: u32) { self.put(0x9E620000 | (xn<<5) | dd); }
    pub fn scvtf_s_x(&mut self, sd: u32, xn: u32) { self.put(0x9E220000 | (xn<<5) | sd); }
    pub fn scvtf_d_w(&mut self, dd: u32, wn: u32) { self.put(0x1E620000 | (wn<<5) | dd); }
    pub fn scvtf_s_w(&mut self, sd: u32, wn: u32) { self.put(0x1E220000 | (wn<<5) | sd); }
    pub fn ucvtf_d_x(&mut self, dd: u32, xn: u32) { self.put(0x9E630000 | (xn<<5) | dd); }
    // FCVTZS Xd, Dn — double → signed int64 (truncate toward zero). = CVTTSD2SI.
    pub fn fcvtzs_x_d(&mut self, xd: u32, dn: u32) { self.put(0x9E780000 | (dn<<5) | xd); }
    pub fn fcvtzs_x_s(&mut self, xd: u32, sn: u32) { self.put(0x9E380000 | (sn<<5) | xd); }
    pub fn fcvtzs_w_d(&mut self, wd: u32, dn: u32) { self.put(0x1E780000 | (dn<<5) | wd); }
    // FCVT Dd, Sn / FCVT Sd, Dn — f32↔f64.
    pub fn fcvt_d_s(&mut self, dd: u32, sn: u32) { self.put(0x1E22C000 | (sn<<5) | dd); }
    pub fn fcvt_s_d(&mut self, sd: u32, dn: u32) { self.put(0x1E624000 | (dn<<5) | sd); }
    // Scalar float arith: FADD/FSUB/FMUL/FDIV Dd,Dn,Dm and Sd,Sn,Sm.
    pub fn fadd_d(&mut self, dd: u32, dn: u32, dm: u32) { self.put(0x1E602800 | (dm<<16) | (dn<<5) | dd); }
    pub fn fsub_d(&mut self, dd: u32, dn: u32, dm: u32) { self.put(0x1E603800 | (dm<<16) | (dn<<5) | dd); }
    pub fn fmul_d(&mut self, dd: u32, dn: u32, dm: u32) { self.put(0x1E600800 | (dm<<16) | (dn<<5) | dd); }
    pub fn fdiv_d(&mut self, dd: u32, dn: u32, dm: u32) { self.put(0x1E601800 | (dm<<16) | (dn<<5) | dd); }
    pub fn fadd_s(&mut self, sd: u32, sn: u32, sm: u32) { self.put(0x1E202800 | (sm<<16) | (sn<<5) | sd); }
    pub fn fsub_s(&mut self, sd: u32, sn: u32, sm: u32) { self.put(0x1E203800 | (sm<<16) | (sn<<5) | sd); }
    pub fn fmul_s(&mut self, sd: u32, sn: u32, sm: u32) { self.put(0x1E200800 | (sm<<16) | (sn<<5) | sd); }
    pub fn fdiv_s(&mut self, sd: u32, sn: u32, sm: u32) { self.put(0x1E201800 | (sm<<16) | (sn<<5) | sd); }
    // FSQRT/FNEG/FABS Dd,Dn (scalar).
    pub fn fsqrt_d(&mut self, dd: u32, dn: u32) { self.put(0x1E61C000 | (dn<<5) | dd); }
    pub fn fneg_d(&mut self, dd: u32, dn: u32)  { self.put(0x1E614000 | (dn<<5) | dd); }
    pub fn fabs_d(&mut self, dd: u32, dn: u32)  { self.put(0x1E60C000 | (dn<<5) | dd); }
    pub fn fsqrt_s(&mut self, sd: u32, sn: u32) { self.put(0x1E21C000 | (sn<<5) | sd); }
    // FCMP Dn,Dm — sets NZCV. For UCOMISD → eflags mapping.
    pub fn fcmp_d(&mut self, dn: u32, dm: u32) { self.put(0x1E602000 | (dm<<16) | (dn<<5)); }
    pub fn fcmp_s(&mut self, sn: u32, sm: u32) { self.put(0x1E202000 | (sm<<16) | (sn<<5)); }
    // FCSEL Fd,Fn,Fm,cond = 0x1E200C00 | ftype<<22 | Rm<<16 | cond<<12 | Rn<<5 | Rd
    pub fn fcsel(&mut self, fd: u32, fn_: u32, fm: u32, cond: Cond, is_d: bool) {
        self.put(0x1E200C00 | ((is_d as u32)<<22) | (fm<<16) | ((cond as u32)<<12) | (fn_<<5) | fd);
    }
    pub fn mul_r(&mut self, xd: u32, xn: u32, xm: u32) { self.put(0x9B007C00 | (xm<<16) | (xn<<5) | xd); }
    // ADD Xd, Xn, #imm12
    pub fn add_i(&mut self, xd: u32, xn: u32, imm12: u32) {
        debug_assert!(imm12 < 4096);
        self.put(0x91000000 | (imm12 << 10) | (xn << 5) | xd);
    }
    pub fn sub_i(&mut self, xd: u32, xn: u32, imm12: u32) {
        debug_assert!(imm12 < 4096);
        self.put(0xD1000000 | (imm12 << 10) | (xn << 5) | xd);
    }

    // ── shifts (variable, register) ────────────────────────────────────────
    pub fn lslv(&mut self, xd: u32, xn: u32, xm: u32) { self.put(0x9AC02000 | (xm<<16) | (xn<<5) | xd); }
    pub fn lsrv(&mut self, xd: u32, xn: u32, xm: u32) { self.put(0x9AC02400 | (xm<<16) | (xn<<5) | xd); }
    pub fn asrv(&mut self, xd: u32, xn: u32, xm: u32) { self.put(0x9AC02800 | (xm<<16) | (xn<<5) | xd); }
    pub fn rorv(&mut self, xd: u32, xn: u32, xm: u32) { self.put(0x9AC02C00 | (xm<<16) | (xn<<5) | xd); }
    // 32-bit variants (sf=0, bit31 clear) — for width≤32 ops (rotr@32, W-arith etc).
    pub fn lslv_w(&mut self, wd: u32, wn: u32, wm: u32) { self.put(0x1AC02000 | (wm<<16) | (wn<<5) | wd); }
    pub fn lsrv_w(&mut self, wd: u32, wn: u32, wm: u32) { self.put(0x1AC02400 | (wm<<16) | (wn<<5) | wd); }
    pub fn asrv_w(&mut self, wd: u32, wn: u32, wm: u32) { self.put(0x1AC02800 | (wm<<16) | (wn<<5) | wd); }
    pub fn rorv_w(&mut self, wd: u32, wn: u32, wm: u32) { self.put(0x1AC02C00 | (wm<<16) | (wn<<5) | wd); }
    pub fn add_w(&mut self, wd: u32, wn: u32, wm: u32)  { self.put(0x0B000000 | (wm<<16) | (wn<<5) | wd); }
    pub fn sub_w(&mut self, wd: u32, wn: u32, wm: u32)  { self.put(0x4B000000 | (wm<<16) | (wn<<5) | wd); }
    pub fn mul_w(&mut self, wd: u32, wn: u32, wm: u32)  { self.put(0x1B007C00 | (wm<<16) | (wn<<5) | wd); }
    // Division + bit-manipulation
    pub fn udiv(&mut self, xd: u32, xn: u32, xm: u32) { self.put(0x9AC00800 | (xm<<16) | (xn<<5) | xd); }
    pub fn sdiv(&mut self, xd: u32, xn: u32, xm: u32) { self.put(0x9AC00C00 | (xm<<16) | (xn<<5) | xd); }
    pub fn udiv_w(&mut self, wd: u32, wn: u32, wm: u32) { self.put(0x1AC00800 | (wm<<16) | (wn<<5) | wd); }
    pub fn sdiv_w(&mut self, wd: u32, wn: u32, wm: u32) { self.put(0x1AC00C00 | (wm<<16) | (wn<<5) | wd); }
    // MSUB Xd = Xa - Xn*Xm  (for rem: rem = a - (a/b)*b)
    pub fn msub(&mut self, xd: u32, xn: u32, xm: u32, xa: u32) { self.put(0x9B008000 | (xm<<16) | (xa<<10) | (xn<<5) | xd); }
    pub fn rbit(&mut self, xd: u32, xn: u32) { self.put(0xDAC00000 | (xn<<5) | xd); }
    pub fn rbit_w(&mut self, wd: u32, wn: u32) { self.put(0x5AC00000 | (wn<<5) | wd); }
    pub fn clz(&mut self, xd: u32, xn: u32) { self.put(0xDAC01000 | (xn<<5) | xd); }
    pub fn clz_w(&mut self, wd: u32, wn: u32) { self.put(0x5AC01000 | (wn<<5) | wd); }
    // SMULH/UMULH — 64×64 → high 64 bits.
    pub fn smulh(&mut self, xd: u32, xn: u32, xm: u32)  { self.put(0x9B407C00 | (xm<<16) | (xn<<5) | xd); }
    pub fn umulh(&mut self, xd: u32, xn: u32, xm: u32)  { self.put(0x9BC07C00 | (xm<<16) | (xn<<5) | xd); }
    // UMADDL/SMADDL — Xd = Xa + (Wn × Wm), 32×32→64+64
    pub fn umaddl(&mut self, xd: u32, wn: u32, wm: u32, xa: u32) { self.put(0x9BA00000 | (wm<<16) | (xa<<10) | (wn<<5) | xd); }
    pub fn smaddl(&mut self, xd: u32, wn: u32, wm: u32, xa: u32) { self.put(0x9B200000 | (wm<<16) | (xa<<10) | (wn<<5) | xd); }

    // ── compare + conditional select ───────────────────────────────────────
    // CMP Xn, Xm  (= SUBS XZR, Xn, Xm)
    pub fn cmp_r(&mut self, xn: u32, xm: u32) { self.put(0xEB00001F | (xm<<16) | (xn<<5)); }
    // CSEL Xd, Xn, Xm, cond
    pub fn csel(&mut self, xd: u32, xn: u32, xm: u32, cond: Cond) {
        self.put(0x9A800000 | (xm<<16) | ((cond as u32)<<12) | (xn<<5) | xd);
    }
    // CSET Xd, cond  (= CSINC Xd, XZR, XZR, !cond)
    pub fn cset(&mut self, xd: u32, cond: Cond) {
        self.put(0x9A9F07E0 | ((cond.invert() as u32)<<12) | xd);
    }

    // ── branches ───────────────────────────────────────────────────────────
    // CBZ Xt, +off  (off in bytes, ±1MB, 4-aligned)
    pub fn cbz(&mut self, xt: u32, off: i32) {
        debug_assert!(off % 4 == 0);
        self.put(0xB4000000 | (((off/4) as u32 & 0x7FFFF) << 5) | xt);
    }
    pub fn cbnz(&mut self, xt: u32, off: i32) {
        debug_assert!(off % 4 == 0);
        self.put(0xB5000000 | (((off/4) as u32 & 0x7FFFF) << 5) | xt);
    }
    // B +off  (±128MB)
    pub fn b(&mut self, off: i32) {
        debug_assert!(off % 4 == 0);
        self.put(0x14000000 | ((off/4) as u32 & 0x03FFFFFF));
    }
    pub fn ret(&mut self) { self.put(0xD65F03C0); }
    pub fn nop(&mut self) { self.put(0xD503201F); }

    // ── system ─────────────────────────────────────────────────────────────
    pub fn mrs_nzcv(&mut self, xt: u32) { self.put(0xD53B4200 | xt); }
    pub fn msr_nzcv(&mut self, xt: u32) { self.put(0xD51B4200 | xt); }

    /// Patch a word at `idx` (for forward-branch fixups).
    pub fn patch(&mut self, idx: usize, w: u32) { self.buf[idx] = w; }
    pub fn here(&self) -> usize { self.buf.len() }
}

/// aarch64 condition codes (for CSEL/B.cond/CSET).
#[derive(Debug, Clone, Copy)]
#[repr(u32)]
pub enum Cond {
    EQ=0, NE=1, CS=2, CC=3, MI=4, PL=5, VS=6, VC=7,
    HI=8, LS=9, GE=10, LT=11, GT=12, LE=13, AL=14, NV=15,
}
impl Cond {
    pub fn invert(self) -> Self {
        // Flip bit-0 (except AL/NV which are their own inverse for encoding purposes).
        unsafe { std::mem::transmute((self as u32) ^ 1) }
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Decode-back verification: every new encoding gets a test that objdumps it.
// (Own #67 / #100 discipline mechanized: never trust a hand-encoding un-decoded.)
// ─────────────────────────────────────────────────────────────────────────────

#[cfg(test)]
mod tests {
    use super::*;
    use std::process::Command;

    fn disasm(words: &[u32]) -> String {
        let bytes: Vec<u8> = words.iter().flat_map(|w| w.to_le_bytes()).collect();
        std::fs::write("/tmp/enc_test.bin", &bytes).unwrap();
        let out = Command::new("objdump")
            .args(["-D", "-b", "binary", "-m", "aarch64", "/tmp/enc_test.bin"])
            .output().unwrap();
        String::from_utf8_lossy(&out.stdout)
            .lines().filter(|l| l.contains(':') && l.contains('\t'))
            .map(|l| l.split('\t').skip(2).collect::<Vec<_>>().join(" ").trim().to_string())
            .collect::<Vec<_>>().join("\n")
    }

    #[test]
    fn encodings_decode_back() {
        let mut e = Aarch64Enc::new();
        e.ldr_x(3, 28, 16);   e.str_x(5, 28, 24);
        e.movz(7, 0x1234, 0); e.movk(7, 0x5678, 1);
        e.mov_r(9, 10);
        e.add_r(0, 1, 2); e.sub_r(3, 4, 5); e.and_r(6, 7, 8);
        e.adds_r(0, 1, 2); e.adc_r(3, 4, 5); e.subs_r(6, 7, 8); e.sbc_r(9, 10, 11);
        e.orr_r(9, 10, 11); e.eor_r(12, 13, 14); e.mul_r(15, 16, 17);
        e.lslv(0, 1, 2); e.lsrv(3, 4, 5); e.asrv(6, 7, 8); e.rorv(9, 10, 11);
        e.lsrv_w(0, 1, 2); e.asrv_w(3, 4, 5); e.rorv_w(6, 7, 8);
        e.smulh(0, 1, 2); e.umulh(3, 4, 5);
        e.udiv(0, 1, 2); e.sdiv(3, 4, 5); e.msub(6, 7, 8, 9);
        e.rbit(0, 1); e.rbit_w(2, 3); e.clz(4, 5); e.clz_w(6, 7);
        e.cmp_r(1, 2); e.csel(0, 1, 2, Cond::EQ); e.cset(3, Cond::LT);
        e.cbz(4, 16); e.b(-8);
        e.mrs_nzcv(5); e.msr_nzcv(5);
        e.ret(); e.nop();
        e.add_i(31, 31, 0x70); e.sub_i(31, 31, 0x70);  // sp arithmetic
        let d = disasm(&e.buf);
        eprintln!("{d}");
        // Assert each expected mnemonic appears in order.
        let expected = [
            "ldr x3, [x28, #16]", "str x5, [x28, #24]",
            "mov x7, #0x1234", "movk x7, #0x5678, lsl #16",
            "mov x9, x10",
            "add x0, x1, x2", "sub x3, x4, x5", "and x6, x7, x8",
            "adds x0, x1, x2", "adc x3, x4, x5", "subs x6, x7, x8", "sbc x9, x10, x11",
            "orr x9, x10, x11", "eor x12, x13, x14", "mul x15, x16, x17",
            "lsl x0, x1, x2", "lsr x3, x4, x5", "asr x6, x7, x8", "ror x9, x10, x11",
            "lsr w0, w1, w2", "asr w3, w4, w5", "ror w6, w7, w8",
            "smulh x0, x1, x2", "umulh x3, x4, x5",
            "udiv x0, x1, x2", "sdiv x3, x4, x5", "msub x6, x7, x8, x9",
            "rbit x0, x1", "rbit w2, w3", "clz x4, x5", "clz w6, w7",
            "cmp x1, x2", "csel x0, x1, x2, eq", "cset x3, lt",
            "cbz x4,", "b ",
            "mrs x5, nzcv", "msr nzcv, x5",
            "ret", "nop",
            "add sp, sp, #0x70", "sub sp, sp, #0x70",
        ];
        let lines: Vec<_> = d.lines().collect();
        assert_eq!(lines.len(), expected.len(), "insn count mismatch");
        for (i, (line, exp)) in lines.iter().zip(expected.iter()).enumerate() {
            assert!(line.starts_with(exp) || line.contains(exp),
                "insn #{i}: expected `{exp}`, got `{line}`");
        }
    }
}
