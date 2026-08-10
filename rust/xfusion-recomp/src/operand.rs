//! x86 operand model — the runtime glue between `DecodedInsn` and the `Builder` trait.
//! Transcribed from `X86Lifter.Bind` + `IlLower.{ReadOperand,WriteOperand}` + `AddrExpr`.
//!
//! An x86 `.isa` template takes named params (`lval`/`rval`/`dst`/`src`); each encoding
//! binds those to concrete operands (Ev→ModRM.rm at v-width, Gv→ModRM.reg, Ib→imm0, …).
//! At lift-time, each param becomes an `Operand<B::Val>` (Reg/Mem/Imm), and the template
//! body reads/writes it via `read_operand`/`write_operand` — which emit the right
//! `bd.reg_read`/`bd.mem_read`/`bd.literal`/… calls including the x86 partial-write
//! semantics (32-bit zext, 8/16-bit masked-insert, AH/BH/CH/DH bits 8-15).
//!
//! RegFile layout (matches `state.rs`):
//!   GPR    = RegFile(0) — 16 × u64 (rax..r15)
//!   EFLAGS = RegFile(1) — idx=bit# (CF=0 PF=2 AF=4 ZF=6 SF=7 DF=10 OF=11); read/write Bool
//!   SEG    = RegFile(2) — 6 × u64 seg-base (es cs ss ds fs gs)
//!   XMM    = RegFile(3) — 32 × V128
//!   RIP is state.pc (via `Builder.branch` / passed as `pc` to lift_one).

use crate::decode::{DecodedInsn, XMode};
use sharpretro_jit::{Builder, IlType, RegFile};

pub const GPR: RegFile = RegFile(0);
pub const EFLAGS: RegFile = RegFile(1);
pub const SEG: RegFile = RegFile(2);
pub const XMM: RegFile = RegFile(3);

// eflags bit positions (idx into RegFile(1))
pub const CF: u32 = 0;
pub const PF: u32 = 2;
pub const AF: u32 = 4;
pub const ZF: u32 = 6;
pub const SF: u32 = 7;
pub const DF: u32 = 10;
pub const OF: u32 = 11;

/// A bound operand. `V` = `B::Val` (an SSA temp for Mem's pre-computed address).
#[derive(Clone, Copy)]
pub enum Operand<V> {
    /// GPR at `idx` (0..15), read/written at `width` bits. `high8` = the legacy
    /// AH/CH/DH/BH bank (bits 8-15 of gpr[idx], only when width==8 && no REX).
    Reg { idx: u8, width: u32, high8: bool },
    /// Memory at `addr` (already computed via `addr_from_modrm`), access at `width`.
    /// x86: address is evaluated ONCE per insn (matters for read-modify-write).
    Mem { addr: V, width: u32 },
    /// Immediate — already sign-extended by the decoder.
    Imm { value: i64, width: u32 },
    /// XMM/YMM/ZMM at ModRM.reg or ModRM.rm.
    Xmm { idx: u8, width: u32 },
}

impl<V> Operand<V> {
    pub fn width(&self) -> u32 {
        match self { Self::Reg{width,..} | Self::Mem{width,..}
                   | Self::Imm{width,..} | Self::Xmm{width,..} => *width }
    }
}

pub fn ilty(width: u32) -> IlType {
    match width {
        1 => IlType::Bool, 8 => IlType::U8, 16 => IlType::U16,
        32 => IlType::U32, 64 => IlType::U64, 128 => IlType::V128,
        w => IlType::I { signed: false, width: w as u8 },
    }
}

/// Read an operand's value into a Val at its width.
pub fn read_operand<B: Builder>(bd: &mut B, op: &Operand<B::Val>) -> B::Val
    where B::Val: Copy
{
    match *op {
        Operand::Reg { idx, width: 64, .. } =>
            bd.reg_read(GPR, idx as u32, IlType::U64),
        Operand::Reg { idx, width, high8: true } => {
            // AH/BH/CH/DH: bits 8-15 of gpr[idx] (idx already remapped 4-7→0-3 by binder).
            debug_assert_eq!(width, 8);
            let full = bd.reg_read(GPR, idx as u32, IlType::U64);
            let sh = bd.literal(IlType::U8, 8);
            let hi = bd.shr(full, sh);
            bd.cast(hi, IlType::U8)
        }
        Operand::Reg { idx, width, .. } => {
            let full = bd.reg_read(GPR, idx as u32, IlType::U64);
            bd.cast(full, ilty(width))
        }
        Operand::Mem { addr, width } =>
            bd.mem_read(addr, ilty(width)),
        Operand::Imm { value, width } =>
            bd.literal(ilty(width), value as u128),
        Operand::Xmm { idx, width } =>
            bd.reg_read(XMM, idx as u32, ilty(width)),
    }
}

/// Write `v` to an operand. x86 partial-write semantics:
///   64-bit: full replace.  32-bit: ZERO-extend to 64 (the x86-64 quirk).
///   8/16-bit: masked-insert (preserve upper bits).  high8: insert at bits 8-15.
pub fn write_operand<B: Builder>(bd: &mut B, op: &Operand<B::Val>, v: B::Val)
    where B::Val: Copy
{
    match *op {
        Operand::Reg { idx, width: 64, .. } => {
            bd.reg_write(GPR, idx as u32, v);
        }
        Operand::Reg { idx, width: 32, .. } => {
            // 32-bit write ZERO-extends to 64 (SDM 3.4.1.1). TRUNCATE to U32
            // first — most callers pass a U32 already (arith at op_w=32),
            // but LEA passes a U64 addr; without the truncate, cast(U64→U64)
            // is a no-op → upper 32 leak through. Found by the ptrace-silicon
            // diff: `lea r8d,[rdx-0x61]` at rdx=0 gave r8=0xFF..FF9F not
            // 0xFFFFFF9F; both interp and tier-0 agreed-wrong (only silicon
            // caught it — 129 sites in a real game's CRT hex-parse loop).
            let t = bd.cast(v, IlType::U32);
            let z = bd.cast(t, IlType::U64);
            bd.reg_write(GPR, idx as u32, z);
        }
        Operand::Reg { idx, width, high8 } => {
            // 8/16-bit low (or high8): read-modify-write masked-insert.
            let full = bd.reg_read(GPR, idx as u32, IlType::U64);
            let (mask, shift) = if high8 { (0xFF00u64, 8u64) }
                                else { ((1u64 << width) - 1, 0) };
            let keep_mask = bd.literal(IlType::U64, !mask as u128);
            let kept = bd.and(full, keep_mask);
            let vw = bd.cast(v, IlType::U64);
            let vs = if shift > 0 {
                let sh = bd.literal(IlType::U8, shift as u128);
                bd.shl(vw, sh)
            } else { vw };
            let vmask = bd.literal(IlType::U64, mask as u128);
            let vm = bd.and(vs, vmask);
            let merged = bd.or(kept, vm);
            bd.reg_write(GPR, idx as u32, merged);
        }
        Operand::Mem { addr, width: _ } => {
            bd.mem_write(addr, v);
        }
        Operand::Imm { .. } => panic!("write to Imm operand"),
        Operand::Xmm { idx, .. } => {
            bd.reg_write(XMM, idx as u32, v);
        }
    }
}

/// Compute the effective address from ModRM/SIB into a `Val` (u64).
/// Transcribed from `X86Lifter.AddrExpr`. Called ONCE per memory-operand at bind-time.
pub fn addr_from_modrm<B: Builder>(bd: &mut B, d: &DecodedInsn, pc: u64, mode: XMode) -> B::Val
    where B::Val: Copy
{
    let m = &d.m;
    let mut e: Option<B::Val> = None;

    if m.rip_relative {
        // [rip + disp32] — rip = NEXT insn's pc (pc + len).
        let base = bd.literal(IlType::U64, (pc.wrapping_add(d.len as u64)) as u128);
        e = Some(base);
    } else if m.base_reg >= 0 {
        e = Some(bd.reg_read(GPR, m.base_reg as u32, IlType::U64));
    }

    if m.index_reg >= 0 {
        let mut idx = bd.reg_read(GPR, m.index_reg as u32, IlType::U64);
        if m.scale > 1 {
            let sh = bd.literal(IlType::U8, (m.scale.trailing_zeros()) as u128);
            idx = bd.shl(idx, sh);
        }
        e = Some(match e { Some(base) => bd.add(base, idx), None => idx });
    }

    if m.disp != 0 || e.is_none() {
        let disp = bd.literal(IlType::U64, m.disp as u128);
        e = Some(match e { Some(base) => bd.add(base, disp), None => disp });
    }

    // Segment base: 64-bit mode → only fs(4)/gs(5) are live; 32/16 → any override.
    let seg_idx = match d.p.segment {
        0x26 => Some(0), 0x2E => Some(1), 0x36 => Some(2), 0x3E => Some(3),
        0x64 => Some(4), 0x65 => Some(5), _ => None,
    };
    if let Some(si) = seg_idx {
        if mode != XMode::Bits64 || si >= 4 {
            let seg = bd.reg_read(SEG, si, IlType::U64);
            let a = e.unwrap();
            e = Some(bd.add(seg, a));
        }
    }

    // ‡ 32/16-bit address-size masking (a_width < 64 → mask the effective addr).
    // Deferred — Bits64 primary. When needed: `bd.and(e, (1<<a_width)-1)`.
    e.unwrap()
}

// ── binder helpers: DecodedInsn field → Operand ────────────────────────────
// These mirror the OpClass arms of X86Lifter.Bind. The generated `lift.rs` calls
// these per-def_id per the encoding's operand specs.

/// GPR bind with the AH/BH/CH/DH remap (8-bit reg 4-7 WITHOUT REX = high-8 of gpr[idx-4]).
pub fn gpr<V>(raw_idx: u8, width: u32, has_rex: bool) -> Operand<V> {
    if width == 8 && !has_rex && (4..8).contains(&raw_idx) {
        Operand::Reg { idx: raw_idx - 4, width: 8, high8: true }
    } else {
        Operand::Reg { idx: raw_idx, width, high8: false }
    }
}

/// ModRM.rm → Reg or Mem depending on m.is_reg. Address computed here (once).
pub fn bind_modrm_rm<B: Builder>(bd: &mut B, d: &DecodedInsn, pc: u64, mode: XMode, width: u32) -> Operand<B::Val>
    where B::Val: Copy
{
    if d.m.is_reg {
        gpr(d.m.rm, width, d.p.rex != 0)
    } else {
        Operand::Mem { addr: addr_from_modrm(bd, d, pc, mode), width }
    }
}

pub fn bind_modrm_reg<V>(d: &DecodedInsn, width: u32) -> Operand<V> {
    gpr(d.m.reg, width, d.p.rex != 0)
}

/// +r opcodes (B8+r etc): reg = low 3 bits of opcode, REX.B extends.
pub fn bind_opcode_reg<V>(d: &DecodedInsn, width: u32) -> Operand<V> {
    let idx = (d.op & 7) | if d.p.rex_b() { 8 } else { 0 };
    gpr(idx, width, d.p.rex != 0)
}

pub fn bind_imm<V>(d: &DecodedInsn, slot: u8, width: u32) -> Operand<V> {
    Operand::Imm { value: if slot == 0 { d.imm0 } else { d.imm1 }, width }
}

/// RelBranch → Imm resolved to ABSOLUTE target (pc + len + rel, wrapped at mode IP width).
pub fn bind_rel_branch<V>(d: &DecodedInsn, slot: u8, pc: u64, mode: XMode) -> Operand<V> {
    let rel = if slot == 0 { d.imm0 } else { d.imm1 };
    let mut abs = pc.wrapping_add(d.len as u64).wrapping_add(rel as u64);
    match mode {
        XMode::Bits32 => abs &= 0xFFFF_FFFF,
        XMode::Bits16 => abs &= 0xFFFF,
        _ => {}
    }
    Operand::Imm { value: abs as i64, width: 64 }
}

pub fn bind_fixed_reg<V>(idx: u8, width: u32) -> Operand<V> {
    // Fixed regs (rAX etc) never hit the high-8 remap (they're specified explicitly).
    Operand::Reg { idx, width, high8: false }
}

pub fn bind_xmm_reg<V>(d: &DecodedInsn, width: u32) -> Operand<V> {
    Operand::Xmm { idx: d.m.reg, width }
}

pub fn bind_xmm_rm<B: Builder>(bd: &mut B, d: &DecodedInsn, pc: u64, mode: XMode, width: u32) -> Operand<B::Val>
    where B::Val: Copy
{
    if d.m.is_reg {
        Operand::Xmm { idx: d.m.rm, width }
    } else {
        Operand::Mem { addr: addr_from_modrm(bd, d, pc, mode), width }
    }
}
