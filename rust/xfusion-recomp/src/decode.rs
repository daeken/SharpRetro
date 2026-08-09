//! x86 instruction decode primitives — hand-port of `XFusionCpu/Decode.cs`.
//!
//! Freeze-law: this is TRANSCRIBED from the C# source (which is itself day-4-verified
//! against XED at 99.87%/100% on real corpora), not composed. The generated
//! `disassembler.rs` (from XFusionScaffold) calls these helpers exactly as the C#
//! generated Disassembler.cs calls Decode.cs.
//!
//! Surface: `scan_prefixes` (legacy/REX/VEX/EVEX prefix loop) + `read_modrm` (ModRM +
//! SIB + displacement) + `read_imm` + `mask_to_width`. The disasm-string helpers
//! (GprName/MemOperandString etc) are NOT ported — the recompiler doesn't need them.

#![allow(dead_code)]

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum XMode { Bits16, Bits32, Bits64 }

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum OpcodeMap { OneByte, TwoByte0F, ThreeByte0F38, ThreeByte0F3A }

#[derive(Debug, Clone, Copy, Default)]
pub struct PrefixState {
    pub op_size: bool,     // 0x66
    pub ad_size: bool,     // 0x67
    pub lock: bool,        // 0xF0
    pub rep_nz: bool,      // 0xF2
    pub rep: bool,         // 0xF3
    pub segment: u8,       // 0=none, else 0x26/0x2E/0x36/0x3E/0x64/0x65
    pub rex: u8,           // 0 if none; else raw REX byte 0x40-0x4F (64-bit mode only)
    pub vex_valid: bool,   // C4/C5/62 seen — map from vex_map, pp folded into op_size/rep/rep_nz
    pub vex_map: u8,       // 1=0F, 2=0F38, 3=0F3A
    pub vex_vvvv: u8,      // 2nd-source register (already un-inverted; EVEX: 5 bits via V')
    pub vex_l: bool,       // 0=xmm(128), 1=ymm(256) — EVEX uses vec_len instead
    pub evex_valid: bool,  // 62 seen (implies vex_valid)
    pub vec_len: u8,       // EVEX L'L: 0=128, 1=256, 2=512
    pub evex_mask: u8,     // aaa: 0=none, 1-7=k1-k7
    pub evex_z: bool,      // zeroing-masking
    pub evex_b: bool,      // broadcast/rc/sae
    pub evex_rp: bool,     // R' — ModRM.reg bit 4
}

impl PrefixState {
    #[inline] pub fn rex_w(&self) -> bool { self.rex & 8 != 0 }
    #[inline] pub fn rex_r(&self) -> bool { self.rex & 4 != 0 }
    #[inline] pub fn rex_x(&self) -> bool { self.rex & 2 != 0 }
    #[inline] pub fn rex_b(&self) -> bool { self.rex & 1 != 0 }

    /// Operand v-width (the SDM's `v` size letter): 16/32/64 selected by mode +
    /// 0x66 + REX.W. VEX-mode: L selects 128/256 (or EVEX L'L for 512).
    /// Transcribed from PrefixState.VWidth — verify at bytes vs C# on the corpus.
    pub fn v_width(&self, mode: XMode) -> u32 {
        if self.vex_valid {
            // VEX/EVEX vector width
            return if self.evex_valid {
                match self.vec_len { 0 => 128, 1 => 256, _ => 512 }
            } else if self.vex_l { 256 } else { 128 };
        }
        match mode {
            XMode::Bits16 => if self.op_size { 32 } else { 16 },
            XMode::Bits32 => if self.op_size { 16 } else { 32 },
            XMode::Bits64 => if self.rex_w() { 64 } else if self.op_size { 16 } else { 32 },
        }
    }
    /// z-width = min(v-width, 32) — Iz-immediates are 32-bit even at REX.W=1.
    pub fn z_width(&self, mode: XMode) -> u32 { self.v_width(mode).min(32) }
    /// d64: default-64 in long-mode (push/pop/near-branch); 0x66 → 16.
    pub fn v_width_d64(&self, mode: XMode) -> u32 {
        if mode == XMode::Bits64 { if self.op_size { 16 } else { 64 } }
        else { self.v_width(mode) }
    }
    /// Near-branch displacement width in long-mode is 32 (rel32), not z.
    pub fn branch_z_width(&self, mode: XMode) -> u32 {
        if mode == XMode::Bits64 { 32 } else { self.z_width(mode) }
    }
    /// Address size: mode default flipped by 0x67.
    pub fn a_width(&self, mode: XMode) -> u32 {
        match mode {
            XMode::Bits16 => if self.ad_size { 32 } else { 16 },
            XMode::Bits32 => if self.ad_size { 16 } else { 32 },
            XMode::Bits64 => if self.ad_size { 32 } else { 64 },
        }
    }
}

#[derive(Debug, Clone, Copy, Default)]
pub struct ModRm {
    pub mod_: u8,          // raw fields
    pub reg: u8,           // REX.R-extended already
    pub rm: u8,            // REX.B-extended already
    pub is_reg: bool,      // mod == 11
    // memory form:
    pub base_reg: i8,      // GPR index or -1
    pub index_reg: i8,     // GPR index or -1 (never 4/RSP)
    pub scale: u8,         // 1/2/4/8
    pub disp: i64,
    pub rip_relative: bool, // 64-bit mode mod=00 rm=101
}

#[derive(Debug, Clone, Copy, Default)]
pub struct DecodedInsn {
    pub def_id: u32,
    pub len: u32,          // total instruction length (prefixes + opcode + operands)
    pub op: u8,            // final opcode byte (for +r reg extraction)
    pub p: PrefixState,
    pub m: ModRm,          // valid only if the def carries ModRM
    pub imm0: i64,
    pub imm1: i64,
}

// ─────────────────────────────────────────────────────────────────────────────

/// Scan legacy prefixes + REX + VEX/EVEX. Returns bytes consumed.
/// Transcribed from Decode.ScanPrefixes.
pub fn scan_prefixes(code: &[u8], mode: XMode) -> (usize, PrefixState) {
    let mut p = PrefixState::default();
    let mut i = 0;
    while i < code.len() {
        let b = code[i];
        match b {
            0x66 => { p.op_size = true; p.rex = 0; }
            0x67 => { p.ad_size = true; p.rex = 0; }
            0xF0 => { p.lock = true; p.rex = 0; }
            0xF2 => { p.rep_nz = true; p.rex = 0; }
            0xF3 => { p.rep = true; p.rex = 0; }
            0x26 | 0x2E | 0x36 | 0x3E | 0x64 | 0x65 => { p.segment = b; p.rex = 0; }
            0x40..=0x4F if mode == XMode::Bits64 => { p.rex = b; }
            0xC5 if i + 2 < code.len() && (mode == XMode::Bits64 || (code[i+1] & 0xC0) == 0xC0) => {
                // 2-byte VEX: [C5][R̄vvvvLpp] — map=0F, X̄=B̄=1(clear), W=0
                let b1 = code[i+1];
                p.vex_valid = true;
                p.vex_map = 1;
                p.vex_vvvv = (!b1 >> 3) & 0xF;
                p.vex_l = b1 & 4 != 0;
                p.rex = 0x40 | if b1 & 0x80 == 0 { 4 } else { 0 };  // R̄ inverted → REX.R
                apply_vex_pp(b1 & 3, &mut p);
                return (i + 2, p);
            }
            0x62 if i + 4 < code.len() && (mode == XMode::Bits64 || (code[i+1] & 0xC0) == 0xC0) => {
                // EVEX: [62][R̄X̄B̄R̄'00mm][Wvvvv1pp][zL'Lb V'aaa]
                let (e1, e2, e3) = (code[i+1], code[i+2], code[i+3]);
                p.vex_valid = true;
                p.evex_valid = true;
                p.vex_map = e1 & 3;
                p.vex_vvvv = ((!e2 >> 3) & 0xF) | if e3 & 8 == 0 { 16 } else { 0 };  // V' inverted, bit 4
                p.vec_len = (e3 >> 5) & 3;
                p.vex_l = p.vec_len == 1;
                p.evex_mask = e3 & 7;
                p.evex_z = e3 & 0x80 != 0;
                p.evex_b = e3 & 0x10 != 0;
                p.evex_rp = e1 & 0x10 == 0;  // R' inverted
                p.rex = 0x40
                    | if e2 & 0x80 != 0 { 8 } else { 0 }   // W
                    | if e1 & 0x80 == 0 { 4 } else { 0 }   // R̄
                    | if e1 & 0x40 == 0 { 2 } else { 0 }   // X̄
                    | if e1 & 0x20 == 0 { 1 } else { 0 };  // B̄
                apply_vex_pp(e2 & 3, &mut p);
                return (i + 4, p);
            }
            0xC4 if i + 3 < code.len() && (mode == XMode::Bits64 || (code[i+1] & 0xC0) == 0xC0) => {
                // 3-byte VEX: [C4][R̄X̄B̄mmmmm][WvvvvLpp]
                let (b1, b2) = (code[i+1], code[i+2]);
                p.vex_valid = true;
                p.vex_map = b1 & 0x1F;
                p.vex_vvvv = (!b2 >> 3) & 0xF;
                p.vex_l = b2 & 4 != 0;
                p.rex = 0x40
                    | if b2 & 0x80 != 0 { 8 } else { 0 }   // W
                    | if b1 & 0x80 == 0 { 4 } else { 0 }   // R̄
                    | if b1 & 0x40 == 0 { 2 } else { 0 }   // X̄
                    | if b1 & 0x20 == 0 { 1 } else { 0 };  // B̄
                apply_vex_pp(b2 & 3, &mut p);
                return (i + 3, p);
            }
            _ => return (i, p),
        }
        i += 1;
    }
    (i, p)
}

fn apply_vex_pp(pp: u8, p: &mut PrefixState) {
    // pp plays the mandatory-prefix role — fold into the same fields the non-VEX
    // dispatch already discriminates on.
    match pp {
        1 => p.op_size = true,
        2 => p.rep = true,
        3 => p.rep_nz = true,
        _ => {}
    }
}

/// Decode ModRM + SIB + displacement. `code` starts AT the ModRM byte.
/// Returns Some(bytes_consumed) or None if truncated. Transcribed from Decode.ReadModRm.
pub fn read_modrm(code: &[u8], mode: XMode, p: &PrefixState) -> Option<(usize, ModRm)> {
    // ‡ TRANSCRIBE from Decode.cs:183-260 next. Stub for scaffold-compiles.
    if code.is_empty() { return None; }
    let b = code[0];
    let mut m = ModRm {
        mod_: b >> 6,
        reg: ((b >> 3) & 7) | if p.rex_r() { 8 } else { 0 },
        rm:  (b & 7) | if p.rex_b() { 8 } else { 0 },
        is_reg: (b >> 6) == 3,
        base_reg: -1, index_reg: -1, scale: 1, disp: 0, rip_relative: false,
    };
    if m.is_reg { return Some((1, m)); }
    let _ = mode;
    // memory form: SIB + disp — full body next commit.
    todo!("read_modrm memory-form: transcribe Decode.cs:183-260")
}

pub fn mask_to_width(v: i64, bits: u32) -> u64 {
    if bits >= 64 { v as u64 } else { (v as u64) & ((1u64 << bits) - 1) }
}

/// Read an immediate of `bits` from code at `*i`, advance `*i`. `sign_extend` = the
/// `Iz`/`Jz` case (32-bit imm sign-extended to 64 at REX.W).
pub fn read_imm(code: &[u8], i: &mut usize, bits: u32, sign_extend: bool) -> i64 {
    let n = (bits as usize) / 8;
    let mut v: u64 = 0;
    for k in 0..n { v |= (code[*i + k] as u64) << (k * 8); }
    *i += n;
    if sign_extend && bits < 64 {
        let sh = 64 - bits;
        (((v << sh) as i64) >> sh)
    } else {
        v as i64
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Verification: prefix/REX spot-checks. Full corpus-diff vs C# Decode via a
// harness that runs BOTH decoders on the same bytes → per-field diff.
// ─────────────────────────────────────────────────────────────────────────────

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn prefix_basics() {
        // No prefixes, Bits64
        let (n, p) = scan_prefixes(&[0x89, 0xC8], XMode::Bits64);
        assert_eq!(n, 0);
        assert_eq!(p.v_width(XMode::Bits64), 32);  // default 32 in long-mode

        // REX.W → v-width 64
        let (n, p) = scan_prefixes(&[0x48, 0x89, 0xC8], XMode::Bits64);
        assert_eq!(n, 1);
        assert!(p.rex_w());
        assert_eq!(p.v_width(XMode::Bits64), 64);

        // 66 + REX.W: REX.W wins (SDM: REX.W overrides 0x66 for operand-size)
        let (n, p) = scan_prefixes(&[0x66, 0x48, 0x89], XMode::Bits64);
        assert_eq!(n, 2);
        assert_eq!(p.v_width(XMode::Bits64), 64);

        // 66 alone → 16
        let (n, p) = scan_prefixes(&[0x66, 0x89], XMode::Bits64);
        assert_eq!(n, 1);
        assert_eq!(p.v_width(XMode::Bits64), 16);

        // Legacy-then-REX: legacy prefix clears REX (SDM: REX must be LAST prefix).
        // 48 66 89 → REX seen first, then 66 clears it → v_width=16.
        let (n, p) = scan_prefixes(&[0x48, 0x66, 0x89], XMode::Bits64);
        assert_eq!(n, 2);
        assert_eq!(p.rex, 0);
        assert_eq!(p.v_width(XMode::Bits64), 16);

        // REX in Bits32 = INC/DEC opcode, NOT a prefix.
        let (n, _p) = scan_prefixes(&[0x48, 0x89], XMode::Bits32);
        assert_eq!(n, 0);
    }

    #[test]
    fn vex_2byte() {
        // vzeroupper = C5 F8 77
        let (n, p) = scan_prefixes(&[0xC5, 0xF8, 0x77], XMode::Bits64);
        assert_eq!(n, 2);
        assert!(p.vex_valid);
        assert_eq!(p.vex_map, 1);
        assert_eq!(p.vex_vvvv, 0);  // 1111 inverted
        assert!(!p.vex_l);
    }

    #[test]
    fn modrm_reg_form() {
        // 89 C8 = mov eax, ecx → ModRM = C8 = mod=11 reg=001(rcx) rm=000(rax)
        let p = PrefixState::default();
        let (n, m) = read_modrm(&[0xC8], XMode::Bits64, &p).unwrap();
        assert_eq!(n, 1);
        assert!(m.is_reg);
        assert_eq!(m.reg, 1);
        assert_eq!(m.rm, 0);
    }
}
