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
    /// Pre-loaded VALUE (C1 atomics): read returns `v`, write is a NO-OP.
    /// Used to re-run an unmodified template over the OLD value an atomic
    /// RMW already returned — flags compute exactly as the template says,
    /// zero transcription, while the memory side happened atomically.
    Val { v: V, width: u32 },
}

impl<V> Operand<V> {
}

/// Ⓗ: x86 float→signed-int with indefinite-integer semantics. SDM Vol 2A
/// (all CVT{,T}S{S,D}2SI + packed forms): "If a converted result cannot be
/// represented in the destination format, ... the indefinite integer value
/// (80000000H or 80000000_00000000H ...) is returned." Rust `as i32` gives
/// 0 for NaN and saturates for ±inf; aarch64 fcvtzs saturates (0x7FFF... for
/// +inf) — THREE-way divergence from silicon. Fix: check in-range first via
/// `fabs(v) < 2^(iw−1)` (NaN → lt=false → indef; the boundary v=−2^(iw−1)
/// "wrongly" fires but indefinite==INT_MIN==the correct value anyway). All
/// backends inherit via Builder primitives.
pub fn f_to_si_x86<B: Builder>(bd: &mut B, fv: B::Val, iw: u32, fw: u32) -> B::Val
    where B::Val: Copy
{
    // 2^(iw−1) as f{fw} — bit-patterns objdump/python-verified (not composed):
    //   2^31: f32=0x4F000000 f64=0x41E0000000000000
    //   2^63: f32=0x5F000000 f64=0x43E0000000000000
    let bound_bits: u128 = match (fw, iw) {
        (32, 32) => 0x4F000000,           (32, 64) => 0x5F000000,
        (64, 32) => 0x41E0000000000000,   (64, 64) => 0x43E0000000000000,
        _ => panic!("f_to_si_x86 fw={fw} iw={iw}"),
    };
    let fty = IlType::F{width: fw as u8};
    let ity = IlType::I{signed:true, width: iw as u8};
    let bound = bd.literal(fty, bound_bits);
    let av = bd.fabs(fv);
    // lt on F: interp (x<y = false for NaN), tier-0 FCMP+cset MI (N=0 on
    // unordered → false). Both give in_range=false for NaN/±inf. ✓
    let in_range = bd.lt(av, bound);
    let cvt = bd.cast(fv, ity);
    let indef = bd.literal(ity, 1u128 << (iw - 1));
    bd.ternary(in_range, cvt, indef)
}

impl<V> Operand<V> {
    pub fn width(&self) -> u32 {
        match self { Self::Reg{width,..} | Self::Mem{width,..}
                   | Self::Imm{width,..} | Self::Xmm{width,..}
                   | Self::Val{width,..} => *width }
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
        Operand::Val { v, .. } => v,
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
        Operand::Mem { addr, width } => {
            // Cast v to the operand's width first — mem_write's dispatch is on
            // v's TYPE, and callers may pass Bool (SETcc: `(= dst OF)` where dst
            // is Eb-mem → flag-read is Bool → tier-0 mem_write panics; wall #27
            // @0x14086b45e `setz [rbp+0x67]` inside CP2077's step-10 Renderer).
            // The reg-arms already cast (8/16 RMW does cast(v,U64); 32 does
            // cast(v,U32)); the mem-arm didn't. cast(Bool,U8) → cmp+cset (own
            // #117's fix). Also normalizes any ill-typed v to the store width.
            let vc = bd.cast(v, ilty(width as u32));
            bd.mem_write(addr, vc);
        }
        Operand::Imm { .. } => panic!("write to Imm operand"),
        Operand::Val { .. } => { /* C1 atomics: memory side already done
            atomically by the RMW node; the template's write is discarded. */ }
        Operand::Xmm { idx, width: 128 } => {
            bd.reg_write(XMM, idx as u32, v);
        }
        Operand::Xmm { idx, width } => {
            // Scalar SS/SD (width 32/64): write low `width` bits ONLY, upper
            // 128−width PRESERVED. SDM Vol 2A ADDSS: "The three high-order
            // doublewords of the destination operand remain unchanged."
            // Silicon-sweep phase-2 first-fire: ~1,350/1,600 diffs = this ONE
            // bug (ADDSS/SUBSS/MULSS/DIVSS/SQRTSS/CMPSS + all SD + MOVSS/SD
            // reg,reg + CVTSS2SD/SD2SS). addss xmm0,xmm0 w/ xmm0=[1,1,1,1]f:
            // silicon xmm0=[2,1,1,1]f (upper preserved), interp was [2,0,0,0]
            // (X86State::reg_write does self.xmm[idx]=v.bits, full replace).
            //
            // Also fixes MOVSS/MOVSD reg,reg (merges per SDM; the mem→reg
            // ZEROES-upper case is Wss-mem-form = phase-3 mem-arm, separate).
            let full = bd.reg_read(XMM, idx as u32, IlType::V128);
            let mask: u128 = if width >= 128 { u128::MAX } else { (1u128 << width) - 1 };
            let km = bd.literal(IlType::V128, !mask);
            let kept = bd.and(full, km);
            // v may be F32/F64/U32/U64 — bitcast to int (bit-preserve), then
            // cast int→V128 (zext-into-128, upper zero per interp cast :233).
            let vi = bd.bitcast(v, ilty(width));
            let vv = bd.cast(vi, IlType::V128);
            let lm = bd.literal(IlType::V128, mask);
            let vlo = bd.and(vv, lm);            // defensive: mask low width
            let merged = bd.or(kept, vlo);
            bd.reg_write(XMM, idx as u32, merged);
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

/// BT/BTS/BTR/BTC (Ev,Gv) mem-form: x86 BIT-STRING addressing. SDM Vol 2A
/// (BT): "the instruction may access a memory address ± offset from the
/// base" — with a REGISTER bit-index the offset is NOT masked to the operand
/// width; the effective byte address is adjusted by the signed word-index:
///     ea' = ea + (W/8) · floor(bitoff / W)      (bitoff signed, W=op width)
/// then the template's own `& (W−1)` selects the bit within the word.
/// Floor-division = arithmetic-shift-right by log2(W) (the QEMU/hardware
/// form; floor not trunc so bitoff=−1 → last bit of the PREVIOUS word).
/// Imm-form (Ev,Ib) DOES mask — imm8 is masked to width; no ea adjust.
/// Reg-form masks too (the reg-form sweep verified that path silicon-clean).
///
/// Silicon-sweep phase-3 first-fire caught this: 77 DIFF + 465 REJECT, ALL
/// in BT-family mem-form (mem[48]/mem[54] = silicon stepping bytes beyond
/// the dword; REJECTs = huge pre-val bit-indexes → wild ea → child SIGSEGV).
pub fn bind_modrm_rm_bitstring<B: Builder>(bd: &mut B, d: &DecodedInsn, pc: u64,
                                           mode: XMode, width: u32)
    -> Operand<B::Val>
    where B::Val: Copy
{
    if d.m.is_reg {
        return gpr(d.m.rm, width, d.p.rex != 0);
    }
    let ea = addr_from_modrm(bd, d, pc, mode);
    // bitoff = the Gv index register, at operand width, sign-extended to 64.
    // (Backends dispatch shr on the value's SIGNEDNESS — shr on I{signed}
    // = arithmetic shift, the same form the .isa's >>a lowers to.)
    let reg_idx = (d.m.reg | if d.p.rex_r() { 8 } else { 0 }) as u32;
    let full = bd.reg_read(GPR, reg_idx, IlType::U64);
    let bo_w = bd.cast(full, IlType::I{signed:true, width:width as u8});
    let bo = bd.cast(bo_w, IlType::I{signed:true, width:64});
    // word_idx = bo >>a log2(W)  (arith shift = floor); byte_off = word_idx << log2(W/8)
    let l2w = bd.literal(IlType::U8, width.trailing_zeros() as u128);
    let widx = bd.shr(bo, l2w);
    let l2b = bd.literal(IlType::U8, (width / 8).trailing_zeros() as u128);
    let boff = bd.shl(widx, l2b);
    let boff_u = bd.cast(boff, IlType::U64);
    let addr = bd.add(ea, boff_u);
    Operand::Mem { addr, width }
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
