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
    pub fn mov_imm64(&mut self, xd: u32, v: u64) {
        let mut first = true;
        for hw in 0..4 {
            let h = ((v >> (hw*16)) & 0xFFFF) as u32;
            if first { self.movz(xd, h, hw); first = false; }
            else if h != 0 { self.movk(xd, h, hw); }
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
    pub fn eor_r(&mut self, xd: u32, xn: u32, xm: u32) { self.put(0xCA000000 | (xm<<16) | (xn<<5) | xd); }
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
