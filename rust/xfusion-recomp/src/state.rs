//! `X86State` — RegState impl for InterpretingBuilder. RegFile layout matches
//! `operand.rs` (GPR=0/EFLAGS=1/SEG=2/XMM=3).

use sharpretro_jit::{IlType, RegFile};
use sharpretro_jit::interp::{IVal, RegState};

#[derive(Debug, Clone, PartialEq)]
pub struct X86State {
    pub gpr: [u64; 16],   // rax rcx rdx rbx rsp rbp rsi rdi r8..r15
    pub eflags: u32,      // whole word; individual bits via RegFile(1) idx=bit#
    pub seg_base: [u64; 6],  // es cs ss ds fs gs
    pub xmm: [u128; 32],
    pub rip: u64,
}

impl Default for X86State {
    fn default() -> Self {
        Self { gpr: [0; 16], eflags: 0x202, seg_base: [0; 6], xmm: [0; 32], rip: 0 }
    }
}

impl X86State {
    pub fn cf(&self) -> bool { (self.eflags >> 0) & 1 != 0 }
    pub fn zf(&self) -> bool { (self.eflags >> 6) & 1 != 0 }
    pub fn sf(&self) -> bool { (self.eflags >> 7) & 1 != 0 }
    pub fn of(&self) -> bool { (self.eflags >> 11) & 1 != 0 }
}

impl RegState for X86State {
    fn reg_read(&self, f: RegFile, idx: u32, ty: IlType) -> IVal {
        match f.0 {
            0 /*GPR*/ => IVal { ty, bits: self.gpr[idx as usize] as u128 },
            1 /*EFLAGS*/ => IVal::b((self.eflags >> idx) & 1 != 0),
            2 /*SEG*/ => IVal { ty, bits: self.seg_base[idx as usize] as u128 },
            3 /*XMM*/ => IVal { ty, bits: self.xmm[idx as usize] },
            _ => panic!("reg_read file={} not wired", f.0),
        }
    }
    fn reg_write(&mut self, f: RegFile, idx: u32, v: IVal) {
        match f.0 {
            0 /*GPR*/ => self.gpr[idx as usize] = v.bits as u64,
            1 /*EFLAGS*/ => {
                self.eflags = (self.eflags & !(1 << idx)) | ((v.as_bool() as u32) << idx);
            }
            2 /*SEG*/ => self.seg_base[idx as usize] = v.bits as u64,
            3 /*XMM*/ => self.xmm[idx as usize] = v.bits,
            _ => panic!("reg_write file={} not wired", f.0),
        }
    }
    fn pc(&self) -> u64 { self.rip }
    fn set_pc(&mut self, pc: u64) { self.rip = pc }
    fn set_lr(&mut self, _: u64) { /* x86 has no link reg — CALL pushes to stack */ }
}
