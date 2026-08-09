//! HAND-WRITTEN lift for a small set of insns — proves the operand.rs + state.rs
//! + InterpretingBuilder pipeline end-to-end BEFORE the generator exists.
//!
//! The generator (RustLiftGen, phase-2b) will emit exactly this shape per template:
//!   - bind operands from DecodedInsn per the encoding's spec-list
//!   - execute the eval-body via read/write_operand + Builder ops
//!
//! This is the walls-ladder step-0: get MOV working through the whole stack
//! (decode → bind → interp → correct state), THEN generate the other 482.

use crate::decode::{DecodedInsn, XMode};
use crate::disassembler::DEF_MNEMONICS;
use crate::operand::*;
use sharpretro_jit::{Builder, IlType};

/// Lift one decoded x64 insn through `bd`. Returns true if handled.
/// (Generated `lift.rs` has the same signature; this hand-version covers
///  ~5 insns for pipeline-proof.)
pub fn lift_one<B: Builder>(bd: &mut B, d: &DecodedInsn, pc: u64, mode: XMode) -> bool
    where B::Val: Copy
{
    let mnem = DEF_MNEMONICS[d.def_id as usize];
    let vw = d.p.v_width(mode);  // ‡ D64 defs use v_width_d64 — the generator knows per-def_id
    let _next_pc = pc.wrapping_add(d.len as u64);

    // ── operand-binding dispatch — the generator emits this per-def_id from
    //    the encoding's spec-list. Here: hand-cased by mnemonic + encoding shape.
    match mnem {
        "MOV" => {
            // Determine (dst, src) operands from the encoding shape via def_id →
            // spec-list. Hand-approximate by opcode-byte class:
            //   88/89: Ev, Gv (rm←reg)     8A/8B: Gv, Ev (reg←rm)
            //   B0+r: r8, Ib               B8+r: rGv, Iv
            //   C6/0: Eb, Ib               C7/0: Ev, Iz
            let (dst, src) = match d.op {
                0x88 | 0x89 => {
                    let w = if d.op == 0x88 { 8 } else { vw };
                    (bind_modrm_rm(bd, d, pc, mode, w), bind_modrm_reg::<B::Val>(d, w))
                }
                0x8A | 0x8B => {
                    let w = if d.op == 0x8A { 8 } else { vw };
                    (bind_modrm_reg::<B::Val>(d, w), bind_modrm_rm(bd, d, pc, mode, w))
                }
                0xB0..=0xB7 => (bind_opcode_reg(d, 8), bind_imm(d, 0, 8)),
                0xB8..=0xBF => (bind_opcode_reg(d, vw), bind_imm(d, 0, vw)),
                0xC6 => (bind_modrm_rm(bd, d, pc, mode, 8), bind_imm(d, 0, 8)),
                0xC7 => (bind_modrm_rm(bd, d, pc, mode, vw), bind_imm(d, 0, vw)),  // Iz→vw sext handled by decoder
                _ => return false,  // seg-moves, moffs — later
            };
            let v = read_operand(bd, &src);
            write_operand(bd, &dst, v);
            true
        }
        "ADD" | "SUB" => {
            // Ev,Gv (01/29) / Gv,Ev (03/2B) / Ev,Iz (81/0,/5) / Ev,Ib (83/0,/5) / rAX,Iz (05/2D)
            let is_add = mnem == "ADD";
            let (lval, rval) = match d.op {
                0x00 | 0x01 | 0x28 | 0x29 => {
                    let w = if d.op & 1 == 0 { 8 } else { vw };
                    (bind_modrm_rm(bd, d, pc, mode, w), bind_modrm_reg::<B::Val>(d, w))
                }
                0x02 | 0x03 | 0x2A | 0x2B => {
                    let w = if d.op & 1 == 0 { 8 } else { vw };
                    (bind_modrm_reg::<B::Val>(d, w), bind_modrm_rm(bd, d, pc, mode, w))
                }
                0x04 | 0x05 | 0x2C | 0x2D => {
                    let w = if d.op & 1 == 0 { 8 } else { vw };
                    (bind_fixed_reg(0 /*rAX*/, w), bind_imm(d, 0, w))
                }
                0x80 | 0x81 | 0x83 => {
                    let w = if d.op == 0x80 { 8 } else { vw };
                    (bind_modrm_rm(bd, d, pc, mode, w), bind_imm(d, 0, w))
                }
                _ => return false,
            };
            // Template body — from ia32-base.isa's ADD (transcribed to Builder calls).
            // (mlet _lval _rval tval = op(_lval,_rval) → write lval → set flags)
            let a = read_operand(bd, &lval);
            let b = read_operand(bd, &rval);
            let t = if is_add { bd.add(a, b) } else { bd.sub(a, b) };
            write_operand(bd, &lval, t);
            // Flags (subset — CF/ZF/SF for now; OF/AF/PF at generator).
            let z = bd.literal(ilty(lval.width()), 0);
            let zf = bd.eq(t, z);
            bd.reg_write(EFLAGS, ZF, zf);
            let sh = bd.literal(IlType::U8, (lval.width() - 1) as u128);
            let sf = bd.shr(t, sh);
            let sfb = bd.cast(sf, IlType::Bool);
            bd.reg_write(EFLAGS, SF, sfb);
            let cf = if is_add { bd.lt(t, a) } else { bd.lt(a, b) };  // unsigned cmp
            bd.reg_write(EFLAGS, CF, cf);
            true
        }
        "RET" => {
            // pop rip: rip = mem[rsp]; rsp += 8. Then branch to it.
            let rsp_op = bind_fixed_reg::<B::Val>(4, 64);
            let rsp = read_operand(bd, &rsp_op);
            let target = bd.mem_read(rsp, IlType::U64);
            let eight = bd.literal(IlType::U64, 8);
            let rsp2 = bd.add(rsp, eight);
            write_operand(bd, &rsp_op, rsp2);
            bd.branch(target, false);
            true
        }
        "JMP" => {
            // EB rel8 / E9 rel32 (near). Absolute target already resolved by binder.
            match d.op {
                0xEB | 0xE9 => {
                    let tgt = bind_rel_branch::<B::Val>(d, 0, pc, mode);
                    let t = read_operand(bd, &tgt);
                    bd.branch(t, false);
                    true
                }
                0xFF => {  // FF /4 = jmp Ev (indirect)
                    let ev = bind_modrm_rm(bd, d, pc, mode, 64);
                    let t = read_operand(bd, &ev);
                    bd.branch(t, false);
                    true
                }
                _ => false,
            }
        }
        "NOP" => true,
        _ => false,
    }
    // ‡ next_pc fallthrough is the block-driver's job (like aarch64) — this fn
    //   only emits a branch when the insn IS a branch.
}


