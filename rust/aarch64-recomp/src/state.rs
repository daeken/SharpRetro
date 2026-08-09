//! aarch64 `RegState` impl for the InterpretingBuilder. Register-file layout matches
//! the RegFile ids the generated recompiler.rs declares (GPR=0/VEC=1/NZCV=2/SR=3).
//!
//! GPR: x[0..30] + SP at idx=31 (per the emit's convention: gpr-or-sp reads idx=31 as SP;
//!   plain gpr reads emit `if idx==31 { lit(0) }` at generated-code level, so reg_read
//!   here never sees the XZR case — idx=31 = SP).
//! NZCV: idx=0 → whole word (bits [31:28]=NZCV); idx=1..4 → individual N/Z/C/V flags.
//! VEC: v[0..31], full V128.
//! SR: system-register access → intrinsic (call_intrinsic id=0/6), never reg_read/write here.

use sharpretro_jit::{IlType, RegFile};
use sharpretro_jit::interp::{IVal, RegState};

#[derive(Debug, Clone, PartialEq)]
pub struct Aarch64State {
    pub x: [u64; 32],   // x[31] = SP
    pub v: [u128; 32],
    pub nzcv: u32,      // bits [31:28] = N,Z,C,V (matching PSTATE)
    pub pc: u64,
}

impl Default for Aarch64State {
    fn default() -> Self { Self { x: [0; 32], v: [0; 32], nzcv: 0, pc: 0 } }
}

impl Aarch64State {
    pub fn n(&self) -> bool { (self.nzcv >> 31) & 1 != 0 }
    pub fn z(&self) -> bool { (self.nzcv >> 30) & 1 != 0 }
    pub fn c(&self) -> bool { (self.nzcv >> 29) & 1 != 0 }
    pub fn vf(&self) -> bool { (self.nzcv >> 28) & 1 != 0 }
    fn set_flag(&mut self, bit: u32, v: bool) {
        self.nzcv = (self.nzcv & !(1 << bit)) | ((v as u32) << bit);
    }
}

impl RegState for Aarch64State {
    fn reg_read(&self, f: RegFile, idx: u32, ty: IlType) -> IVal {
        match f.0 {
            0 /*GPR*/ => {
                let raw = self.x[idx as usize];
                match ty {
                    IlType::I{width:32, ..} => IVal { ty, bits: (raw as u32) as u128 },
                    _ => IVal { ty, bits: raw as u128 },
                }
            }
            1 /*VEC*/ => IVal { ty, bits: self.v[idx as usize] },
            2 /*NZCV*/ => match idx {
                0 => IVal { ty, bits: self.nzcv as u128 },
                1 => IVal::b(self.n()), 2 => IVal::b(self.z()),
                3 => IVal::b(self.c()), 4 => IVal::b(self.vf()),
                _ => panic!("nzcv idx {idx}"),
            },
            _ => panic!("reg_read file={} not wired", f.0),
        }
    }
    fn reg_write(&mut self, f: RegFile, idx: u32, v: IVal) {
        match f.0 {
            0 /*GPR*/ => {
                // W-write zero-extends per aarch64 (the emit already casts to U32 for gpr32).
                self.x[idx as usize] = match v.ty {
                    IlType::I{width:32, ..} => (v.bits as u32) as u64,
                    _ => v.bits as u64,
                };
            }
            1 /*VEC*/ => self.v[idx as usize] = v.bits,
            2 /*NZCV*/ => match idx {
                0 => self.nzcv = v.bits as u32,
                1 => self.set_flag(31, v.as_bool()), 2 => self.set_flag(30, v.as_bool()),
                3 => self.set_flag(29, v.as_bool()), 4 => self.set_flag(28, v.as_bool()),
                _ => panic!("nzcv idx {idx}"),
            },
            _ => panic!("reg_write file={} not wired", f.0),
        }
    }
    fn pc(&self) -> u64 { self.pc }
    fn set_pc(&mut self, pc: u64) { self.pc = pc }
    fn set_lr(&mut self, lr: u64) { self.x[30] = lr }
}
