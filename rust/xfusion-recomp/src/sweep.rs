//! Exhaustive silicon-sweep encoder + enumerator.
//!
//! Consumes the generated SWEEP_DEFS table (per-XFusionDef encoding facts)
//! and synthesizes every valid encoding × register-combo × opsize. Every
//! synthesized byte-string is round-trip-verified against decode_insn (the
//! XED-verified decoder = the encoder's oracle). Then TrackingState +
//! boundary-value pre-states → X64D corpus rows for the EC2 ptrace runner.
//!
//! Phase-1 scope (this file's initial cut): LEGACY (vex==0) rows, GPR/scalar
//! ops only (Erm/Greg/Zopc/FixR/FixI/Imm classes), REG-FORM only (mod=11).
//! Mem-form + XMM + VEX = phase-2/3 (each is a distinct encoding-branch).

use crate::sweep_defs::{SwDef, SwOp, SwCls, SwW};
#[cfg(test)] use crate::sweep_defs::SWEEP_DEFS;
use crate::disassembler::{decode_insn, DEF_MNEMONICS};
use crate::decode::XMode;

/// Rows that exist in ia32-base for 32-bit mode but are UNENCODABLE in long
/// mode (their opcode range is repurposed as prefix bytes). The decoder's
/// scan_prefixes consumes them before dispatch → correct decode-fail.
/// Not encoder bugs — mode-invalid rows. Skip in the 64-bit sweep.
/// Defs invalid in Bits32 (long-mode-only). Populated from objdump-i386
/// cross-check (round-trip = self-oracle only — our decoder doesn't
/// mode-gate 0x63, so encode MOVSXD → 63 C1 → decode(Bits32) → MOVSXD passes
/// round-trip; objdump -m i386 says ARPL). ‡ DECODER-TIER gap: the .isa has
/// no `only64` encoding-flag; 40+r INC/DEC work by accident (prefix scanner
/// eats 0x40-0x4F as REX in Bits64 before dispatch). Proper fix = add an
/// only64 flag + gate in both disasm-gens; for the sweep, skip here.
pub fn bits32_invalid(d: &SwDef) -> bool {
    // From objdump -m i386 cross-check (bits32_encode_spot_objdump test):
    match d.mnem {
        // 0x63 = ARPL in Bits32 (16-bit-era priv insn). Alky is 64-bit only
        // → the decoder gap doesn't affect the consumer; ‡ filed regardless.
        "MOVSXD" => true,
        _ => false,
    }
}

pub fn long_mode_invalid(d: &SwDef) -> bool {
    // 40+r INC / 48+r DEC = the REX prefix range.
    if d.map == 0 && d.plus_r && (d.opcode == 0x40 || d.opcode == 0x48) { return true; }
    // Other 32-bit-only one-byte rows (PUSH/POP seg, LES/LDS, ARPL, BOUND, into,
    // AAA/DAA/AAS/DAS/AAM/AAD, PUSHA/POPA) — add as the round-trip census names
    // them. For now: only INC/DEC surfaced (the rest may already be feature-
    // gated out or in phase-1-skip categories).
    false
}

/// One point in the sweep space: a specific encoding with all fields chosen.
/// The encoder produces bytes from this; the caller iterates the choice space.
#[derive(Copy, Clone, Debug)]
pub struct EncChoice {
    pub mode: XMode,    // Bits64 (default) | Bits32 (① 32-bit sweep arm)
    pub op_w: u8,       // effective operand width for V/Z-coded operands: 16|32|64
    pub reg: u8,        // ModRM.reg field value (0..15 in Bits64, 0..8 in Bits32)
    pub rm: u8,         // ModRM.rm field value  (same range)
    pub zopc: u8,       // +r opcode-embedded reg (same range)
    pub imm: u64,       // immediate value (if any Imm operand)
    pub mem: Option<MemChoice>,  // phase-3: mem-form addressing (None = reg-form mod=11)
    pub lock: bool,     // phase-4: F0 prefix (lockable mnemonics, mem-dst forms only)
}
impl Default for EncChoice {
    fn default() -> Self {
        Self { mode: XMode::Bits64, op_w: 32, reg: 0, rm: 0, zopc: 0, imm: 0,
               mem: None, lock: false }
    }
}

/// Phase-3 mem-form: the addressing-mode choice. When `Some`, `encode()`
/// emits ModRM at mod≠11 (+SIB+disp as needed) instead of the reg-form
/// mod=11. `EncChoice.rm` is IGNORED in mem-form (rm is derived from the
/// mem-shape: rm=4 when SIB, rm=5 when rip-rel/no-base, else base&7).
#[derive(Copy, Clone, Debug, PartialEq, Eq)]
pub struct MemChoice {
    pub base:  i8,      // GPR index 0..15, or -1 = no base
    pub index: i8,      // GPR index 0..15, or -1 = no index (SIB idx=4)
    pub scale: u8,      // 1|2|4|8 (only meaningful if index>=0)
    pub disp:  i32,     // displacement (0 = try no-disp form)
    pub rip_rel: bool,  // Bits64: mod=00 rm=5 [rip+disp32]. Bits32: [disp32] absolute.
}
impl MemChoice {
    /// Choose the ModRM.mod field (0/1/2) + whether a SIB byte is needed.
    /// The x86 special-index rules:
    ///   • rm=4 (rsp/r12) ALWAYS needs SIB, at any mod≠11
    ///   • mod=00 rm=5 = [rip+d32] (64) / [d32] (32), NOT [rbp]/[r13] →
    ///     encoding [rbp]/[r13] with disp=0 requires mod=01 disp8=0
    ///   • SIB base=5 mod=00 = no-base [d32 + idx*s]; encoding rbp/r13 as
    ///     SIB base with disp=0 requires mod=01 disp8=0
    ///   • SIB idx=4 = no-index. rsp-as-index UNENCODABLE (idx-field=100
    ///     bare = none). r12-as-index IS encodable (REX.X=1 idx=100 = r12,
    ///     objdump-verified: 42 01 0C 20 = add [rax+r12*1],ecx). SDM Table
    ///     2-5 is explicit: REX.X extends SIB.index; only bare 100 is none.
    fn plan(&self) -> (u8 /*mod*/, bool /*need_sib*/, u8 /*disp_bytes*/) {
        if self.rip_rel {
            // mod=00 rm=5, always disp32. No SIB.
            return (0, false, 4);
        }
        // Need SIB if: index present, OR base is rsp/r12 (rm-field would be
        // 4 which means "SIB follows"), OR base is absent (SIB base=5 mod=00).
        let need_sib = self.index >= 0
            || (self.base >= 0 && (self.base & 7) == 4)
            || self.base < 0;
        // The "base=5 problem": both direct-rm=5 and SIB-base=5 at mod=00
        // mean something OTHER than [rbp]/[r13]. If base&7==5 and disp==0,
        // must use mod=01 disp8=0. Also if base<0 (no-base), mod=00 SIB
        // base=5 disp32.
        let base5 = self.base >= 0 && (self.base & 7) == 5;
        let (mod_, dbytes) = if self.base < 0 {
            (0, 4)                                   // no-base → mod=00 disp32
        } else if self.disp == 0 && !base5 {
            (0, 0)                                   // [base] no disp
        } else if self.disp as i8 as i32 == self.disp {
            (1, 1)                                   // disp8
        } else {
            (2, 4)                                   // disp32
        };
        (mod_, need_sib, dbytes)
    }
}

/// What phase-1 will and won't encode. A def is phase-1-eligible if ALL its
/// operands are in the phase-1 class-set AND it's legacy (vex==0). Returns
/// the reason for skipping (for the census).
pub fn phase1_skip(d: &SwDef, mode: XMode) -> Option<&'static str> {
    if d.vex != 0 { return Some("vex/evex"); }
    // 40+r/48+r INC/DEC etc: invalid in Bits64 (REX range), VALID in Bits32.
    if mode == XMode::Bits64 && long_mode_invalid(d) { return Some("64bit-invalid"); }
    // Bits32-invalid rows (MOVSXD, SYSCALL, CDQE/CQO, SWAPGS, …): let the
    // round-trip CENSUS name them first (day-1 method) rather than composing
    // the list from memory. bits32_invalid() below grows from census output.
    if mode == XMode::Bits32 && bits32_invalid(d) { return Some("32bit-invalid"); }
    for o in d.ops {
        match o.cls {
            SwCls::Erm | SwCls::Greg | SwCls::Zopc | SwCls::FixR | SwCls::FixI
            | SwCls::Imm => {}
            SwCls::Rel => return Some("branch"),      // rip-changing, phase-2
            SwCls::StrS | SwCls::StrD => return Some("string-op"),
            // Phase-2: Vxmm/Wxmm/Uxmm occupy the SAME ModRM.reg/.rm fields
            // as Greg/Erm at the byte level (REX.R/B extend to xmm8-15
            // identically). encode() treats them via has_vreg/has_wrm below.
            SwCls::Vxmm | SwCls::Wxmm | SwCls::Uxmm => {}
            // Hxmm = VEX.vvvv 3rd operand — needs VEX encoding (phase-3).
            SwCls::Hxmm => return Some("vex-vvvv"),
            SwCls::Sreg => return Some("segreg"),
            SwCls::Moff => return Some("moffs"),
            SwCls::Preg | SwCls::Qrm => return Some("mmx"),
            SwCls::FpuT | SwCls::FpuI => return Some("x87"),
            SwCls::Kreg | SwCls::Krm => return Some("mask"),
            SwCls::Hgpr => return Some("bmi-vvvv"),
            SwCls::FarP => return Some("far-ptr"),
        }
    }
    // mem_only operands can't be reg-form → phase-2
    if d.ops.iter().any(|o| o.mem_only) { return Some("mem-only"); }
    // mod11==0 means the def is explicitly mem-form-only
    if d.mod11 == 0 { return Some("mod11-mem"); }
    None
}

/// Immediate width in bytes for a given SwW at a given effective op_w.
/// Z-code = 16→2, 32/64→4 (imm never widens to 8; 64-bit ops use Iz=32-bit sext).
fn imm_bytes(w: SwW, op_w: u8) -> u8 {
    match w {
        SwW::B => 1,
        SwW::W => 2,
        SwW::D => 4,
        SwW::Q => 8,   // rare (mov r64,imm64 = the one Iq)
        SwW::V => match op_w { 16 => 2, 32 => 4, 64 => 8, _ => unreachable!() },
        SwW::Y => match op_w { 32 => 4, 64 => 8, _ => unreachable!() },
        SwW::Z => match op_w { 16 => 2, _ => 4 },
        _ => panic!("imm width {w:?}"),
    }
}

/// Does this def have V/Z-coded operands? (⟹ opsize dimension matters)
pub fn has_v_operand(d: &SwDef) -> bool {
    d.ops.iter().any(|o| matches!(o.w, SwW::V | SwW::Z))
}

/// Encode one legacy x86-64 instruction from a SwDef + choices.
/// Phase-1: reg-form only (mod=11 when ModRM present). Emits: [mprefix]
/// [66 opsize] [REX] [0F [38|3A]] opcode [ModRM] [imm].
///
/// REX bits: W=1 iff op_w==64 (and def isn't d64 — d64 defaults to 64 without
/// REX.W; PUSH/POP/CALL class). R = reg[3], B = rm[3] (or zopc[3] for +r).
/// X unused in reg-form. REX byte omitted entirely when all bits clear AND
/// no b-width operand needs SPL/BPL/SIL/DIL selection (a bare REX 0x40 selects
/// the low-byte view of rsp/rbp/rsi/rdi over AH/CH/DH/BH — one of the x64
/// special-register-index cases; phase-1 emits REX for those explicitly).
pub fn encode(d: &SwDef, c: &EncChoice) -> Vec<u8> {
    let mut b = Vec::with_capacity(15);

    // Which operands drive which encoding fields. Vxmm/Uxmm occupy ModRM.reg
    // like Greg; Wxmm occupies ModRM.rm like Erm — same byte-level encoding,
    // same REX.R/B extension for idx≥8. The XMM/GPR distinction is purely
    // in the .isa's operand-class dispatch, not in the instruction bytes.
    // ModRM field mapping: Vxmm=ModRM.reg (like Greg); Wxmm|Uxmm=ModRM.rm
    // (Uxmm = XMM-in-rm mod=11-only, e.g. PSRLW-I `(Uxmm Ib) /2` — reg_ext
    // in reg-field, XMM idx in rm). First-cut had Uxmm on the reg side →
    // enumerated c.reg uselessly (reg_ext overrides) + rm always 0.
    let has_greg = d.ops.iter().any(|o| matches!(o.cls, SwCls::Greg));
    let has_vreg = d.ops.iter().any(|o| matches!(o.cls, SwCls::Vxmm));
    let has_erm  = d.ops.iter().any(|o| matches!(o.cls, SwCls::Erm));
    let has_wrm  = d.ops.iter().any(|o| matches!(o.cls, SwCls::Wxmm | SwCls::Uxmm));
    let has_zopc = d.plus_r || d.ops.iter().any(|o| matches!(o.cls, SwCls::Zopc));
    let imm_op   = d.ops.iter().find(|o| matches!(o.cls, SwCls::Imm));
    let uses_reg = has_greg || has_vreg;
    let uses_rm  = has_erm || has_wrm;
    let has_modrm = uses_reg || uses_rm || d.reg_ext >= 0;

    // ── prefix bytes ──
    // LOCK (F0, group 1) first — phase-4 atomic forms.
    if c.lock { b.push(0xF0); }
    // Mandatory prefix (66/F2/F3) is a REAL prefix byte, before REX.
    if d.mprefix != 0 { b.push(d.mprefix); }
    // Operand-size 66 for op_w==16 on V-coded rows (unless mprefix already 66,
    // in which case 16-bit isn't reachable via legacy — those are SSE rows and
    // 66 there means "opsize" as the SSE discriminator, not width-16).
    if c.op_w == 16 && d.mprefix != 0x66 && has_v_operand(d) { b.push(0x66); }

    // REX byte. W = op_w==64 unless d64 (d64 rows are 64-bit by default in long
    // mode, no REX.W needed). R/B/X from high bits of reg/rm/zopc.
    // REX.W: for V- or Y-coded rows at op_w=64. XMM-only rows never set
    // REX.W from op_w (some SSE defs like MOVQ-X use REX.W as an OPCODE
    // discriminator — that's captured in the def's own d64/mprefix, not here).
    let has_y = d.ops.iter().any(|o| matches!(o.w, SwW::Y));
    let rex_w = c.op_w == 64 && !d.d64 && (has_v_operand(d) || has_y);
    let rex_r = uses_reg && (c.reg & 8) != 0;
    // REX.B: extends rm in reg-form; extends BASE in mem-form (or zopc).
    // REX.X: extends SIB.index (mem-form only).
    let (rex_b, rex_x) = if has_zopc {
        ((c.zopc & 8) != 0, false)
    } else if let Some(m) = &c.mem {
        // rip-rel: rm=5 no REX.B; base<0: SIB base=5 no REX.B either.
        (m.base >= 0 && (m.base & 8) != 0,
         m.index >= 0 && (m.index & 8) != 0)
    } else if uses_rm {
        ((c.rm & 8) != 0, false)
    } else {
        (false, false)
    };
    // Byte-reg SPL/BPL/SIL/DIL selection: any b-width GPR operand at index 4-7
    // needs a bare REX (0x40) to select the low-byte view instead of AH/CH/DH/BH.
    // Applies to Greg-b, Erm-b (reg-form), Zopc-b, and FixR-b at those indices.
    // Conversely: AH/CH/DH/BH are UNENCODABLE with any REX present — the caller's
    // enumerator handles that (skips reg∈4..8 for b-width when REX would be forced
    // by another field, or emits both variants where legal).
    let byte_op_at_hi4 = |o: &SwOp, idx: u8| {
        matches!(o.w, SwW::B) && (4..8).contains(&idx)
    };
    let need_rex40 = c.mode == XMode::Bits64 && d.ops.iter().any(|o| match o.cls {
        SwCls::Greg => byte_op_at_hi4(o, c.reg),
        SwCls::Erm  => byte_op_at_hi4(o, c.rm),
        SwCls::Zopc => byte_op_at_hi4(o, c.zopc),
        SwCls::FixR => o.fix_idx >= 4 && o.fix_idx < 8 && matches!(o.w, SwW::B),
        _ => false,
    });
    let rex_bits = ((rex_w as u8)<<3) | ((rex_r as u8)<<2)
                 | ((rex_x as u8)<<1) | (rex_b as u8);
    // REX is Bits64-ONLY (0x40-0x4F = INC/DEC in Bits32). Enumerate caps reg/
    // rm/zopc to 0..8 and op_w to {16,32} in Bits32 → rex_bits SHOULD be 0;
    // this assert catches an enumerator that leaks a 64-bit choice into a
    // 32-bit encode. In Bits32, byte-reg idx 4-7 = AH/CH/DH/BH (no SPL/BPL/
    // SIL/DIL exist without REX) → need_rex40 doesn't apply.
    if c.mode != XMode::Bits64 {
        debug_assert_eq!(rex_bits, 0, "REX bits set in {:?}: {c:?}", c.mode);
    } else if rex_bits != 0 || need_rex40 {
        b.push(0x40 | rex_bits);
    }

    // ── opcode map + opcode byte ──
    match d.map {
        0 => {}
        1 => b.push(0x0F),
        2 => { b.push(0x0F); b.push(0x38); }
        3 => { b.push(0x0F); b.push(0x3A); }
        _ => unreachable!(),
    }
    let opc = if has_zopc { d.opcode | (c.zopc & 7) } else { d.opcode };
    b.push(opc);

    // ── ModRM (+SIB+disp for mem-form) ──
    if has_modrm {
        let reg3 = if d.reg_ext >= 0 { d.reg_ext as u8 }
                   else if uses_reg { c.reg & 7 }
                   else { 0 };
        if let Some(m) = &c.mem {
            // Phase-3 mem-form. See MemChoice::plan() for the special-index
            // rules (rm=4→SIB, rm=5@mod=00→rip/abs, base=5@mod=00→no-base).
            let (mod_, need_sib, dbytes) = m.plan();
            let rm3 = if m.rip_rel { 5 }
                      else if need_sib { 4 }
                      else { (m.base & 7) as u8 };
            b.push((mod_ << 6) | (reg3 << 3) | rm3);
            if need_sib {
                let ss = match m.scale { 1=>0, 2=>1, 4=>2, 8=>3, _=>unreachable!() };
                let idx3 = if m.index >= 0 { (m.index & 7) as u8 } else { 4 };
                let base3 = if m.base >= 0 { (m.base & 7) as u8 } else { 5 };
                b.push((ss << 6) | (idx3 << 3) | base3);
            }
            match dbytes {
                0 => {}
                1 => b.push(m.disp as i8 as u8),
                4 => b.extend_from_slice(&(m.disp as i32).to_le_bytes()),
                _ => unreachable!(),
            }
        } else {
            // Reg-form: mod=11. uses_reg/uses_rm (not has_greg/has_erm) —
            // Vxmm/Wxmm occupy the same ModRM.reg/.rm fields.
            let rm3 = if uses_rm { c.rm & 7 } else { 0 };
            b.push(0xC0 | (reg3 << 3) | rm3);
        }
    }

    // ── immediate ──
    if let Some(io) = imm_op {
        let n = imm_bytes(io.w, c.op_w) as usize;
        let bytes = c.imm.to_le_bytes();
        b.extend_from_slice(&bytes[..n]);
    }

    b
}

/// Round-trip verify: encode → decode_insn → check mnemonic + length match.
/// Returns Ok(len) or Err(reason). This is the encoder's ORACLE — every
/// synthesized encoding must survive the XED-verified decoder.
pub fn verify_rt(d: &SwDef, c: &EncChoice) -> Result<Vec<u8>, String> {
    let bytes = encode(d, c);
    match decode_insn(&bytes, c.mode) {
        None => Err(format!("decode-fail: {} bytes {:02X?} choice {c:?}",
                            d.mnem, bytes)),
        Some(di) if di.len as usize != bytes.len() =>
            Err(format!("len-mismatch: {} enc={} dec={} bytes {:02X?}",
                        d.mnem, bytes.len(), di.len, bytes)),
        Some(di) => {
            // Mnemonic-match: the decoded def_id's mnem must equal the SwDef's.
            // This catches the misdecode-beats-undecode danger (redundant
            // encodings decoding to the WRONG def — the day-1 kt).
            let dm = DEF_MNEMONICS[di.def_id as usize];
            if dm != d.mnem {
                return Err(format!("mnem-mismatch: encoded {} decoded {} bytes {:02X?} choice {c:?}",
                                   d.mnem, dm, bytes));
            }
            // Operand-match: len+mnem alone missed the ModRM=0xC0 bug (every
            // XMM row encoded reg=0,rm=0; still same mnem+len — objdump caught
            // it, this gate didn't until strengthened).
            // Verify decoded ModRM.reg/.rm/plus_r-idx match the choice.
            let uses_reg = d.ops.iter().any(|o|
                matches!(o.cls, SwCls::Greg | SwCls::Vxmm));
            let uses_rm = d.ops.iter().any(|o|
                matches!(o.cls, SwCls::Erm | SwCls::Wxmm | SwCls::Uxmm));
            if uses_reg && di.m.reg != c.reg {
                return Err(format!("reg-mismatch: {} enc reg={} dec reg={} bytes {:02X?}",
                                   d.mnem, c.reg, di.m.reg, bytes));
            }
            if let Some(mc) = &c.mem {
                // Mem-form: verify the decoder recovered the SAME mem-shape.
                // This catches encoder+decoder mem-form bugs the reg-form
                // gate can't (own-#165 lesson: len+mnem alone is blind to
                // ModRM misencoding).
                if di.m.is_reg {
                    return Err(format!("mem-mismatch: {} encoded mem-form, decoded is_reg=true bytes {:02X?} mc={mc:?}",
                                       d.mnem, bytes));
                }
                if mc.rip_rel != di.m.rip_relative {
                    return Err(format!("mem-mismatch: {} rip_rel enc={} dec={} bytes {:02X?}",
                                       d.mnem, mc.rip_rel, di.m.rip_relative, bytes));
                }
                if !mc.rip_rel {
                    if di.m.base_reg != mc.base {
                        return Err(format!("mem-mismatch: {} base enc={} dec={} bytes {:02X?} mc={mc:?}",
                                           d.mnem, mc.base, di.m.base_reg, bytes));
                    }
                    if di.m.index_reg != mc.index {
                        return Err(format!("mem-mismatch: {} index enc={} dec={} bytes {:02X?} mc={mc:?}",
                                           d.mnem, mc.index, di.m.index_reg, bytes));
                    }
                    if mc.index >= 0 && di.m.scale != mc.scale {
                        return Err(format!("mem-mismatch: {} scale enc={} dec={} bytes {:02X?}",
                                           d.mnem, mc.scale, di.m.scale, bytes));
                    }
                }
                if di.m.disp != mc.disp as i64 {
                    return Err(format!("mem-mismatch: {} disp enc={} dec={} bytes {:02X?} mc={mc:?}",
                                       d.mnem, mc.disp, di.m.disp, bytes));
                }
            } else if uses_rm && di.m.rm != c.rm {
                return Err(format!("rm-mismatch: {} enc rm={} dec rm={} bytes {:02X?}",
                                   d.mnem, c.rm, di.m.rm, bytes));
            }
            if d.plus_r {
                let dz = (di.op & 7) | if di.p.rex_b() { 8 } else { 0 };
                if dz != c.zopc {
                    return Err(format!("zopc-mismatch: {} enc zopc={} dec={} bytes {:02X?}",
                                       d.mnem, c.zopc, dz, bytes));
                }
            }
            Ok(bytes)
        }
    }
}

/// Enumerate the phase-1 choice space for one def. Yields EncChoice per point.
/// Dimensions: op_w × reg × rm × zopc × imm-boundary. Not all dims apply to
/// all defs — absent dims collapse to a single value.
///
/// Register sweep: 0..16 for each present ModRM field. rsp(4) is INCLUDED
/// (phase-1 reg-form has no SIB, so rsp-in-rm is fine; the corpus emitter
/// separately excludes rsp from RUNTIME pre-state randomization since it's
/// the stub anchor — but rsp as a reg OPERAND is testable with rsp=known).
///
/// Imm boundary sweep per width: {0, 1, MAX, ~0>>1 (max-positive if signed),
/// 1<<(w-1) (min-negative), and 1<<k for k in a sparse set}. sign_ext operands
/// get the imm's own width's boundaries (the value that lands in the reg is
/// the sign-extended result — that's what the pre-state grid then exercises).
pub fn enumerate_p1<F: FnMut(&EncChoice, &[u8])>(d: &SwDef, mode: XMode, f: F) -> (u32, u32) {
    enumerate_p1_debug(d, mode, f, |_| {})
}

/// Phase-3 mem-form addressing-mode SHAPES to enumerate. This is the
/// x86 special-index case-table from PHASE3-MEMFORM.md — each row exercises
/// a distinct encoding path (mod/rm/SIB rules), NOT the full base×idx×scale
/// cartesian product. The corpus dimension expands base/idx values later;
/// this is the encoder-correctness dimension.
///
/// idx=4(rsp) unencodable as index (bare 100=none). idx=12(r12) IS encodable
/// (REX.X extends). base=4(rsp)/12(r12) forces SIB. base=5(rbp)/13(r13) at
/// disp=0 needs mod=01 disp8=0.
pub fn mem_shapes_p3(mode: XMode) -> Vec<MemChoice> {
    let is64 = mode == XMode::Bits64;
    let mut v = vec![
        // ── plain [base], mod=00 rm≠4≠5 ──
        MemChoice{base:0,  index:-1, scale:1, disp:0,      rip_rel:false},  // [rax]
        MemChoice{base:3,  index:-1, scale:1, disp:0,      rip_rel:false},  // [rbx]
        // ── [base+disp8], mod=01 ──
        MemChoice{base:1,  index:-1, scale:1, disp:0x10,   rip_rel:false},  // [rcx+0x10]
        MemChoice{base:2,  index:-1, scale:1, disp:-8,     rip_rel:false},  // [rdx-8]
        // ── [base+disp32], mod=10 ──
        MemChoice{base:0,  index:-1, scale:1, disp:0x1000, rip_rel:false},
        // ── the base=5 problem: [rbp]/[r13] w/ disp=0 → mod=01 disp8=0 ──
        MemChoice{base:5,  index:-1, scale:1, disp:0,      rip_rel:false},
        // ── the rm=4 problem: [rsp]/[r12] → SIB idx=none base=4 ──
        MemChoice{base:4,  index:-1, scale:1, disp:0,      rip_rel:false},  // [rsp]
        MemChoice{base:4,  index:-1, scale:1, disp:0x20,   rip_rel:false},  // [rsp+0x20]
        // ── SIB [base+idx*scale] ──
        MemChoice{base:0,  index:1,  scale:1, disp:0,      rip_rel:false},  // [rax+rcx]
        MemChoice{base:0,  index:2,  scale:4, disp:0,      rip_rel:false},  // [rax+rdx*4]
        MemChoice{base:3,  index:6,  scale:8, disp:8,      rip_rel:false},  // [rbx+rsi*8+8]
        // ── SIB base=5 (rbp) w/ disp=0 → mod=01 disp8=0 ──
        MemChoice{base:5,  index:1,  scale:2, disp:0,      rip_rel:false},
        // ── no-base [idx*s+disp32], mod=00 SIB base=5 ──
        MemChoice{base:-1, index:2,  scale:2, disp:0x60000,rip_rel:false},
        // ── no-base no-idx = pure [disp32] absolute (SIB idx=4 base=5) ──
        MemChoice{base:-1, index:-1, scale:1, disp:0x60000,rip_rel:false},
    ];
    if is64 {
        v.extend_from_slice(&[
            // ── rip-relative (mod=00 rm=5, 64-bit only; 32-bit=[disp32]) ──
            MemChoice{base:0,  index:-1, scale:1, disp:0x400, rip_rel:true},
            // ── r8-r15 as base (REX.B) ──
            MemChoice{base:8,  index:-1, scale:1, disp:0,      rip_rel:false},
            MemChoice{base:15, index:-1, scale:1, disp:0x10,   rip_rel:false},
            // ── r13 = base=5 problem w/ REX.B ──
            MemChoice{base:13, index:-1, scale:1, disp:0,      rip_rel:false},
            // ── r12 = rm=4 problem w/ REX.B (SIB required) ──
            MemChoice{base:12, index:-1, scale:1, disp:0,      rip_rel:false},
            // ── r8-r15 as index (REX.X) — incl r12 (encodable, verified) ──
            MemChoice{base:0,  index:9,  scale:8, disp:8,      rip_rel:false},
            MemChoice{base:0,  index:12, scale:1, disp:0,      rip_rel:false},  // r12-as-idx
            // ── mixed REX.B+REX.X ──
            MemChoice{base:10, index:11, scale:4, disp:-4,     rip_rel:false},
        ]);
    }
    v
}

/// Phase-3 enumerate: for defs with an Erm/Wxmm operand (and mod11≠1 i.e.
/// mem-form legal), walk mem_shapes_p3() × op_w × reg. rm/zopc fixed (mem-form
/// uses c.mem, not c.rm). Round-trip via verify_rt (which now checks
/// base/index/scale/disp/rip_rel against decoded ModRm).
pub fn enumerate_p3_debug<F: FnMut(&EncChoice, &[u8]), E: FnMut(&str)>(
    d: &SwDef, mode: XMode, mut f: F, mut on_fail: E) -> (u32, u32)
{
    // Only defs where Erm/Wxmm exists AND mem-form is legal (mod11 != 1).
    let has_mem_op = d.ops.iter().any(|o|
        matches!(o.cls, SwCls::Erm | SwCls::Wxmm) && !matches!(o.cls, SwCls::Uxmm));
    if !has_mem_op || d.mod11 == 1 { return (0, 0); }
    let has_greg = d.ops.iter().any(|o| matches!(o.cls, SwCls::Greg));
    let has_vreg = d.ops.iter().any(|o| matches!(o.cls, SwCls::Vxmm));
    let uses_reg = has_greg || has_vreg;
    let is64 = mode == XMode::Bits64;
    let has_y = d.ops.iter().any(|o| matches!(o.w, SwW::Y));

    // op_w: same rules as enumerate_p1
    let opws: &[u8] = if is64 {
        if d.d64 { &[64] }
        else if has_v_operand(d) { &[16, 32, 64] }
        else if has_y { &[32, 64] }
        else { &[32] }
    } else {
        if has_v_operand(d) || d.d64 { &[16, 32] } else { &[32] }
    };
    // reg: if uses_reg, sweep a few (0,1,7 and in Bits64 also 8,15 for REX.R
    // interaction w/ REX.B/X). Not the full 0..16 — the reg-form sweep already
    // covers that; this dimension exists to catch REX.R+REX.B/X composition.
    let regs: &[u8] = if uses_reg {
        if is64 { &[0, 1, 7, 8, 15] } else { &[0, 1, 7] }
    } else { &[0] };
    let shapes = mem_shapes_p3(mode);
    let imm_op = d.ops.iter().find(|o| matches!(o.cls, SwCls::Imm));
    // imm: single value (imm-space already covered by phase-1)
    let imm: u64 = if imm_op.is_some() { 0x11 } else { 0 };

    let mut n_ok = 0u32; let mut n_fail = 0u32;
    for &op_w in opws {
        for &reg in regs {
            for mc in &shapes {
                let c = EncChoice { mode, op_w, reg, rm: 0, zopc: 0, imm, mem: Some(*mc), lock: false };
                match verify_rt(d, &c) {
                    Ok(bytes) => { n_ok += 1; f(&c, &bytes); }
                    Err(e) => { n_fail += 1; on_fail(&e); }
                }
            }
        }
    }
    (n_ok, n_fail)
}

pub fn enumerate_p3<F: FnMut(&EncChoice, &[u8])>(d: &SwDef, mode: XMode, f: F) -> (u32, u32) {
    enumerate_p3_debug(d, mode, f, |_| {})
}

/// Phase-4: LOCK-prefix (F0) forms — the lockable set with a MEM dst, both
/// modes. Same mem-shape × op_w × reg walk as phase-3, lock=true. Purpose:
/// silicon-verifies the ATOMIC ROUTING's semantics (atomic_pre's
/// Val-substitution + the XADD/CMPXCHG flag blocks) — single-threaded, a
/// locked op's ARCHITECTURAL effect equals the unlocked one, so the same
/// interp-derived expected-post applies; what the F0 rows add is (a) our
/// decoder accepting every F0 encoding shape, (b) the lifted atomic path
/// producing bare-silicon-exact results+flags.
pub const LOCKABLE: &[&str] = &["ADD","AND","OR","XOR","SUB","INC","DEC","XADD","XCHG","CMPXCHG"];

pub fn enumerate_p4<F: FnMut(&EncChoice, &[u8])>(d: &SwDef, mode: XMode, mut f: F) -> (u32, u32) {
    if !LOCKABLE.contains(&d.mnem) { return (0, 0); }
    // LOCK requires a mem destination: Erm operand + mem-form legal.
    enumerate_p3(d, mode, |c, _| {
        let mut c2 = *c;
        c2.lock = true;
        if let Ok(bytes) = verify_rt(d, &c2) { f(&c2, &bytes); }
    })
}

/// Phase-3 skip: defs with NO mem-form encoding path.
pub fn phase3_skip(d: &SwDef, mode: XMode) -> Option<&'static str> {
    if d.vex != 0 { return Some("vex/evex"); }
    if mode == XMode::Bits64 && long_mode_invalid(d) { return Some("64bit-invalid"); }
    if mode == XMode::Bits32 && bits32_invalid(d) { return Some("32bit-invalid"); }
    // No Erm/Wxmm operand → no mem-form at all.
    let has_mem_op = d.ops.iter().any(|o| matches!(o.cls, SwCls::Erm | SwCls::Wxmm));
    if !has_mem_op { return Some("no-mem-op"); }
    // mod11==1 = reg-form-only encoding row (e.g. some mprefix-mod-split rows).
    if d.mod11 == 1 { return Some("reg-only-row"); }
    None
}

/// Known encoding aliases: choice-points where the encoding is VALID but
/// decodes to a DIFFERENT mnemonic by architectural design. Not encoder bugs
/// — redundant encodings (day-1 lesson: misdecode beats undecode). Enumerator
/// SKIPS these — they'd execute as the decoded mnem on silicon, so testing
/// them under the encoded mnem's expected semantics would be wrong.
fn is_known_alias(d: &SwDef, c: &EncChoice) -> bool {
    // XCHG rAX,rAX (90+r at zopc=0) IS the canonical NOP. All widths.
    if d.map == 0 && d.opcode == 0x90 && d.plus_r && c.zopc == 0 { return true; }
    false
}

/// Same as enumerate_p1 but calls on_fail(err) for each verify_rt failure.
pub fn enumerate_p1_debug<F: FnMut(&EncChoice, &[u8]), E: FnMut(&str)>(
    d: &SwDef, mode: XMode, mut f: F, mut on_fail: E) -> (u32, u32)
{
    let has_greg = d.ops.iter().any(|o| matches!(o.cls, SwCls::Greg));
    let has_vreg = d.ops.iter().any(|o| matches!(o.cls, SwCls::Vxmm));
    let has_erm  = d.ops.iter().any(|o| matches!(o.cls, SwCls::Erm));
    let has_wrm  = d.ops.iter().any(|o| matches!(o.cls, SwCls::Wxmm | SwCls::Uxmm));
    let has_zopc = d.plus_r || d.ops.iter().any(|o| matches!(o.cls, SwCls::Zopc));
    let imm_op   = d.ops.iter().find(|o| matches!(o.cls, SwCls::Imm));
    let is64 = mode == XMode::Bits64;

    // op_w dimension. Bits64: V/Z⇒{16,32,64}, Y⇒{32,64}, d64⇒{64}.
    // Bits32: no REX.W → V/Z⇒{16,32}, Y⇒{32}; d64 ("default-64 in long mode")
    // has no effect in Bits32 — those rows fall back to v_width ⇒ {16,32}.
    let has_y = d.ops.iter().any(|o| matches!(o.w, SwW::Y));
    let opws: &[u8] = if is64 {
        if d.d64 { &[64] }
        else if has_v_operand(d) { &[16, 32, 64] }
        else if has_y { &[32, 64] }
        else { &[32] }
    } else {
        // Bits32 (Bits16 = phase-4, not yet)
        if has_v_operand(d) || d.d64 { &[16, 32] }
        else { &[32] }  // Y and fixed-width both → 32
    };
    // reg/rm/zopc range: Bits64 → 0..16 (REX.R/B extends); Bits32 → 0..8 (no
    // REX; xmm8-15 also unreachable). Byte-reg idx 4-7 in Bits32 = AH/CH/DH/BH
    // (bind_modrm_reg keys high8 on d.p.rex≠0 — rex=0 in Bits32 → high8 ✓).
    let hi: u8 = if is64 { 16 } else { 8 };
    let regs: Vec<u8> = if has_greg || has_vreg { (0..hi).collect() } else { vec![0] };
    let rms:  Vec<u8> = if has_erm || has_wrm  { (0..hi).collect() } else { vec![0] };
    let zops: Vec<u8> = if has_zopc { (0..hi).collect() } else { vec![0] };
    let (mut n_ok, mut n_fail) = (0u32, 0u32);
    for &op_w in opws {
        // Imm boundary set PER ENCODED WIDTH (varies with op_w for V/Z-code).
        // For an 8-bit Ib: {0,1,0x7F,0x80,0xFF,0x55,0xAA} = 7 values, not 30
        // truncated 64-bit points (which alias massively at 8-bit). Kills the
        // IMUL3 25%-of-corpus bloat. Per-width: 0, 1, max, sign-bit, max-pos,
        // alternating-bits both polarities, plus a mid-value.
        let imms: Vec<u64> = match imm_op {
            None => vec![0],
            Some(io) => {
                let wb = imm_bytes(io.w, op_w) as u32 * 8;
                let m = if wb == 64 { u64::MAX } else { (1u64 << wb) - 1 };
                let mut v = vec![0, 1, m, m>>1, 1u64<<(wb-1),
                                 0x5555_5555_5555_5555 & m, 0xAAAA_AAAA_AAAA_AAAA & m];
                // 1<<k sweep at width-quarters (catches nibble/byte-boundary bugs)
                for k in (0..wb).step_by((wb/4).max(1) as usize) { v.push(1u64 << k); }
                v.push(2); v.push(m - 1);  // near-boundary
                v.sort(); v.dedup();
                v
            }
        };
        for &reg in &regs {
            for &rm in &rms {
                for &zopc in &zops {
                    for &imm in &imms {
                        let c = EncChoice { mode, op_w, reg, rm, zopc, imm, mem: None, lock: false };
                        if is_known_alias(d, &c) { continue; }
                        match verify_rt(d, &c) {
                            Ok(bytes) => { n_ok += 1; f(&c, &bytes); }
                            Err(e) => { n_fail += 1; on_fail(&e); }
                        }
                    }
                }
            }
        }
    }
    (n_ok, n_fail)
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn add_eb_gb_encodes() {
        // ADD Eb,Gb (opcode 00) at reg=0(al) rm=1(cl) → 00 C1 (mod=11 reg=0 rm=1)
        let d = SWEEP_DEFS.iter().find(|d| d.mnem == "ADD" && d.opcode == 0x00).unwrap();
        let c = EncChoice { op_w: 32, reg: 0, rm: 1, ..Default::default() };
        let b = encode(d, &c);
        assert_eq!(b, vec![0x00, 0xC1], "add cl,al");
        assert!(verify_rt(d, &c).is_ok());
    }

    #[test]
    fn add_ev_gv_at_64() {
        // ADD Ev,Gv (opcode 01) at op_w=64 reg=8(r8) rm=9(r9) → REX 4D 01 C1
        let d = SWEEP_DEFS.iter().find(|d| d.mnem == "ADD" && d.opcode == 0x01).unwrap();
        let c = EncChoice { op_w: 64, reg: 8, rm: 9, ..Default::default() };
        let b = encode(d, &c);
        assert_eq!(b, vec![0x4D, 0x01, 0xC1], "add r9,r8");
    }

    #[test]
    fn mov_r64_imm64() {
        // MOV Zv,Iv at op_w=64: B8+r + imm64. reg=r10 → REX.WB 49 BA <8>
        let d = SWEEP_DEFS.iter().find(|d| d.mnem == "MOV" && d.plus_r
                                        && d.ops.iter().any(|o| o.cls == SwCls::Imm)
                                        && d.ops[0].w == SwW::V).unwrap();
        let c = EncChoice { op_w: 64, zopc: 10, imm: 0xDEADBEEF_CAFEBABE, ..Default::default() };
        let b = encode(d, &c);
        assert_eq!(&b[..2], &[0x49, 0xBA]);
        assert_eq!(b.len(), 10);
    }

    #[test]
    fn phase1_census() {
        // How many defs are phase-1-eligible? Print the skip-reason census.
        use std::collections::BTreeMap;
        let mut skip: BTreeMap<&str, u32> = BTreeMap::new();
        let mut n_p1 = 0;
        for d in SWEEP_DEFS {
            match phase1_skip(d, XMode::Bits64) { None => n_p1 += 1, Some(r) => *skip.entry(r).or_default() += 1 }
        }
        println!("  phase-1 eligible: {n_p1} / {} defs", SWEEP_DEFS.len());
        for (r, n) in &skip { println!("    skip {r}: {n}"); }
        assert!(n_p1 > 100, "expected >100 phase-1-eligible defs");
    }

    #[test]
    fn xmm_encode_spot_objdump() {
        // Own #67 discipline: round-trip through decode_insn = self-oracle.
        // Verify a few XMM encodings against objdump (independent decoder).
        use std::process::Command;
        let cases: &[(&str, u8, u8, u8, u8, &str)] = &[
            // (mnem, mprefix, opcode-in-map1, reg, rm, want-objdump-mnem)
            ("ADDPS",  0x00, 0x58, 1, 2,  "addps"),   // 0F 58 CA
            ("ADDPS",  0x00, 0x58, 9, 10, "addps"),   // 45 0F 58 CA
            ("ADDSD",  0xF2, 0x58, 0, 7,  "addsd"),   // F2 0F 58 C7
            ("MULPD",  0x66, 0x59, 3, 11, "mulpd"),   // 66 41 0F 59 DB
            ("PXOR",   0x66, 0xEF, 15, 8, "pxor"),    // 66 45 0F EF F8
        ];
        let mut all = vec![];
        for &(mnem, mp, op, reg, rm, _) in cases {
            let d = SWEEP_DEFS.iter().find(|d|
                d.mnem == mnem && d.mprefix == mp && d.opcode == op && d.map == 1
            ).unwrap_or_else(|| panic!("no SwDef for {mnem}"));
            let c = EncChoice { op_w: 32, reg, rm, ..Default::default() };
            let bytes = encode(d, &c);
            eprintln!("  {} reg={} rm={} → {:02X?}", mnem, reg, rm, &bytes);
            all.extend_from_slice(&bytes);
        }
        std::fs::write("/tmp/sweep_xmm_spot.bin", &all).unwrap();
        let out = Command::new("objdump")
            .args(["-D","-b","binary","-m","i386:x86-64","-M","intel","/tmp/sweep_xmm_spot.bin"])
            .output().unwrap();
        let d = String::from_utf8_lossy(&out.stdout);
        let insns: Vec<_> = d.lines().filter(|l| l.contains(":\t")).collect();
        eprintln!("  objdump ({} insns):", insns.len());
        for l in &insns { eprintln!("    {}", l.trim()); }
        // One insn per case (no misdecodes = no split), each matches its want-mnem.
        assert_eq!(insns.len(), cases.len(), "objdump insn count ≠ case count → misencoded");
        for (i, &(mnem, _, _, reg, rm, want)) in cases.iter().enumerate() {
            let l = insns[i];
            assert!(l.contains(want), "{mnem}: objdump `{}` doesn't contain `{want}`", l.trim());
            assert!(l.contains(&format!("xmm{reg}")) && l.contains(&format!("xmm{rm}")),
                    "{mnem}: xmm{reg}/xmm{rm} not in `{}`", l.trim());
        }
    }

    #[test]
    fn round_trip_all_p1() {
        round_trip_at_mode(XMode::Bits64, true);
    }

    #[test]
    fn round_trip_all_bits32() {
        // ① 32-bit arm: gated (was census-only until bits32_invalid populated).
        round_trip_at_mode(XMode::Bits32, true);
    }

    #[test]
    fn round_trip_p3_mem() {
        // Phase-3 mem-form: every mem-eligible def × mem-shape must round-
        // trip through decode_insn (base/index/scale/disp/rip_rel checked).
        for mode in [XMode::Bits64, XMode::Bits32] {
            let mut n_defs = 0; let mut tot_ok = 0u32; let mut tot_fail = 0u32;
            let mut first: Vec<String> = vec![];
            for d in SWEEP_DEFS {
                if phase3_skip(d, mode).is_some() { continue; }
                n_defs += 1;
                let (ok, fail) = enumerate_p3_debug(d, mode, |_,_|{}, |e| {
                    if first.len() < 20 { first.push(e.to_string()); }
                });
                tot_ok += ok; tot_fail += fail;
            }
            for e in &first { println!("    fail: {e}"); }
            println!("  round-trip-p3 [{mode:?}]: {n_defs} defs, {tot_ok} ok, {tot_fail} FAIL");
            assert_eq!(tot_fail, 0, "phase-3 mem-form encoder round-trip must be clean");
        }
    }

    #[test]
    fn mem_encode_spot_objdump() {
        // Independent-decoder verify for the mem-form encoder (round-trip is
        // self-oracle only — the reg-form's ModRM=0xC0 bug + the Bits32 0x63
        // gap both passed round-trip). All 14 special-index cases on ADD Ev,Gv.
        use std::process::Command;
        let d = SWEEP_DEFS.iter().find(|d| d.mnem=="ADD" && d.opcode==0x01 && d.map==0).unwrap();
        let cases: &[(MemChoice, &str)] = &[
            (MemChoice{base:0, index:-1, scale:1, disp:0,      rip_rel:false}, "[rax]"),
            (MemChoice{base:3, index:-1, scale:1, disp:0x10,   rip_rel:false}, "[rbx+0x10]"),
            (MemChoice{base:2, index:-1, scale:1, disp:0x1234, rip_rel:false}, "[rdx+0x1234]"),
            (MemChoice{base:5, index:-1, scale:1, disp:0,      rip_rel:false}, "[rbp+0x0]"),
            (MemChoice{base:13,index:-1, scale:1, disp:0,      rip_rel:false}, "[r13+0x0]"),
            (MemChoice{base:4, index:-1, scale:1, disp:0,      rip_rel:false}, "[rsp]"),
            (MemChoice{base:12,index:-1, scale:1, disp:0,      rip_rel:false}, "[r12]"),
            (MemChoice{base:0, index:2,  scale:4, disp:0,      rip_rel:false}, "[rax+rdx*4]"),
            (MemChoice{base:0, index:9,  scale:8, disp:8,      rip_rel:false}, "[rax+r9*8+0x8]"),
            (MemChoice{base:-1,index:2,  scale:2, disp:0x60000,rip_rel:false}, "[rdx*2+0x60000]"),
            (MemChoice{base:-1,index:-1, scale:1, disp:0x60000,rip_rel:false}, "ds:0x60000"),
            (MemChoice{base:0, index:-1, scale:1, disp:0x333,  rip_rel:true},  "[rip+0x333]"),
            (MemChoice{base:8, index:-1, scale:1, disp:0,      rip_rel:false}, "[r8]"),
            (MemChoice{base:5, index:11, scale:1, disp:-8,     rip_rel:false}, "[rbp+r11*1-0x8]"),
            (MemChoice{base:0, index:12, scale:1, disp:0,      rip_rel:false}, "[rax+r12*1]"),
        ];
        let mut all = vec![];
        for (mc, want) in cases {
            let c = EncChoice { op_w:32, reg:1, mem:Some(*mc), ..Default::default() };
            let bytes = verify_rt(d, &c)
                .unwrap_or_else(|e| panic!("verify_rt failed for {want}: {e}"));
            eprintln!("  {:24} → {:02X?}", want, &bytes);
            all.extend_from_slice(&bytes);
        }
        std::fs::write("/tmp/sweep_p3_spot.bin", &all).unwrap();
        let out = Command::new("objdump")
            .args(["-D","-b","binary","-m","i386:x86-64","-M","intel","/tmp/sweep_p3_spot.bin"])
            .output().unwrap();
        let d = String::from_utf8_lossy(&out.stdout);
        let insns: Vec<&str> = d.lines().filter(|l| l.contains(":\t")).collect();
        assert_eq!(insns.len(), cases.len(),
                   "objdump insn-count ≠ case-count → misencoding");
        for (i, (_, want)) in cases.iter().enumerate() {
            let got = insns[i].splitn(3,'\t').last().unwrap().trim();
            assert!(got.contains(want),
                    "case {i} objdump `{got}` doesn't contain `{want}`");
            eprintln!("  ✓ objdump: `{got}`");
        }
    }

    #[test]
    fn bits32_encode_spot_objdump() {
        // Independent-decoder verify for the Bits32 encoder (round-trip is
        // self-oracle only — our decoder doesn't mode-gate 0x63 → MOVSXD
        // "passes" round-trip in Bits32; objdump -m i386 says ARPL).
        use std::process::Command;
        // (mnem, opcode, map, op_w, reg, rm, zopc, want-objdump-mnem)
        let cases: &[(&str, u8, u8, u8, u8, u8, u8, &str)] = &[
            ("ADD",   0x01, 0, 32, 1, 2, 0, "add"),      // 01 CA add edx,ecx
            ("ADD",   0x01, 0, 16, 1, 2, 0, "add"),      // 66 01 CA add dx,cx
            ("INC",   0x40, 0, 32, 0, 0, 3, "inc"),      // 43 inc ebx (40+r zopc)
            ("DEC",   0x48, 0, 16, 0, 0, 5, "dec"),      // 66 4D dec bp
            ("MOV",   0x88, 0, 32, 4, 1, 0, "mov"),      // 88 E1 mov cl,ah (byte idx=4 no-REX = high8)
            ("ADDPS", 0x58, 1, 32, 3, 6, 0, "addps"),    // 0F 58 DE (SSE works in 32-bit)
            ("IMUL3", 0x69, 0, 32, 2, 5, 0, "imul"),     // 69 D5 .. imul edx,ebp,imm32
        ];
        let mut all = vec![];
        let mut lens = vec![];
        for &(mnem, op, map, op_w, reg, rm, zopc, _) in cases {
            let d = SWEEP_DEFS.iter().find(|d|
                d.mnem == mnem && d.opcode == op && d.map == map
            ).unwrap_or_else(|| panic!("no SwDef for {mnem} m{map}/0x{op:02X}"));
            let c = EncChoice { mode: XMode::Bits32, op_w, reg, rm, zopc, imm: 0x11, mem: None };
            let bytes = encode(d, &c);
            eprintln!("  {} op_w={} reg={} rm={} zopc={} → {:02X?}", mnem, op_w, reg, rm, zopc, &bytes);
            lens.push(bytes.len());
            all.extend_from_slice(&bytes);
        }
        std::fs::write("/tmp/sweep_b32_spot.bin", &all).unwrap();
        let out = Command::new("objdump")
            .args(["-D","-b","binary","-m","i386","-M","intel","/tmp/sweep_b32_spot.bin"])
            .output().unwrap();
        let dump = String::from_utf8_lossy(&out.stdout);
        let insns: Vec<_> = dump.lines().filter(|l| l.contains(":\t")).collect();
        eprintln!("  objdump -m i386 ({} insns):", insns.len());
        for l in &insns { eprintln!("    {}", l.trim()); }
        assert_eq!(insns.len(), cases.len(),
                   "objdump insn count ≠ case count → misencoded (or objdump split one)");
        for (i, &(mnem, .., want)) in cases.iter().enumerate() {
            let l = insns[i];
            assert!(l.contains(want), "{mnem}: objdump `{}` doesn't contain `{want}`", l.trim());
        }
    }

    fn round_trip_at_mode(mode: XMode, gate: bool) {
        // The load-bearing test: every phase-1 def × every enumerated choice
        // must round-trip through decode_insn. n_fail should be ZERO.
        let mut total_ok = 0u32;
        let mut total_fail = 0u32;
        let mut n_defs = 0u32;
        let mut fail_by = std::collections::BTreeMap::<String, u32>::new();
        let mut first_fails: Vec<String> = vec![];
        for d in SWEEP_DEFS {
            if phase1_skip(d, mode).is_some() { continue; }
            n_defs += 1;
            // Print the FIRST failure case per def (the §4 datum).
            let mut df = 0u32;
            let (ok, fail) = enumerate_p1_debug(d, mode, |_c, _b| {}, |e| {
                if df == 0 && first_fails.len() < 40 {
                    first_fails.push(e.to_string());
                }
                df += 1;
            });
            total_ok += ok; total_fail += fail;
            if fail > 0 {
                let key = format!("{} m{}/0x{:02X}", d.mnem, d.map, d.opcode);
                *fail_by.entry(key).or_default() += fail;
            }
        }
        for e in &first_fails { println!("    fail-case: {e}"); }
        println!("  round-trip [{mode:?}]: {n_defs} defs, {total_ok} ok, {total_fail} FAIL");
        for (m, n) in &fail_by { println!("    {m}: {n} fail"); }
        if gate {
            assert_eq!(total_fail, 0, "phase-1 encoder round-trip must be clean");
        }
    }
}
