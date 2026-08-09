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

/// Flat u64-array state layout — the marshalling contract shared by:
///   (1) the Rosetta-NativeStub (Mac-side x64 silicon oracle — the stub's prologue/
///       epilogue loads/stores THESE offsets around the TEST_INSN slot), and
///   (2) tier-0-x64's `entry(state, spill)` (when it lands — same layout as
///       aarch64's flat[STATE_WORDS], different offsets).
/// Both diff against `X86State` via `to_flat`/`from_flat`.
///
/// Layout (u64 words):
///   [0..16]   = gpr[0..15]  (rax rcx rdx rbx rsp rbp rsi rdi r8..r15)
///   [16]      = eflags (low 32 bits; upper zero)
///   [17]      = rip
///   [18..24]  = seg_base[0..5]  (es cs ss ds fs gs)
///   [24..88]  = xmm[0..31] (2 words each: lo, hi)
///   [88]      = mem_base (host ptr; identity-map for tier-0 mem_read/write)
///   [89]      = mem_len
pub const STATE_WORDS_X64: usize = 90;
pub const OFF_GPR: usize = 0;
pub const OFF_EFLAGS: usize = 16;
pub const OFF_RIP: usize = 17;
pub const OFF_SEG: usize = 18;
pub const OFF_XMM: usize = 24;
pub const OFF_MEMBASE: usize = 88;

impl X86State {
    pub fn to_flat(&self) -> [u64; STATE_WORDS_X64] {
        let mut f = [0u64; STATE_WORDS_X64];
        for i in 0..16 { f[OFF_GPR + i] = self.gpr[i]; }
        f[OFF_EFLAGS] = self.eflags as u64;
        f[OFF_RIP] = self.rip;
        for i in 0..6 { f[OFF_SEG + i] = self.seg_base[i]; }
        for i in 0..32 {
            f[OFF_XMM + i*2] = self.xmm[i] as u64;
            f[OFF_XMM + i*2 + 1] = (self.xmm[i] >> 64) as u64;
        }
        f
    }
    pub fn from_flat(f: &[u64; STATE_WORDS_X64]) -> Self {
        let mut s = Self::default();
        for i in 0..16 { s.gpr[i] = f[OFF_GPR + i]; }
        s.eflags = f[OFF_EFLAGS] as u32;
        s.rip = f[OFF_RIP];
        for i in 0..6 { s.seg_base[i] = f[OFF_SEG + i]; }
        for i in 0..32 {
            s.xmm[i] = (f[OFF_XMM + i*2] as u128) | ((f[OFF_XMM + i*2 + 1] as u128) << 64);
        }
        s
    }
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
