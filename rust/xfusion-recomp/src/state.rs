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
/// Per-thread RrThread* (record/replay handle; rr.rs). 0 when rr off.
pub const OFF_RR: usize = 89;

/// Tier-0 StateLayout for x64-guest — flat[90] per the offsets above.
/// RegFile mapping: 0=GPR, 1=EFLAGS(idx=bit#), 2=SEG, 3=XMM.
/// x64 partial-write semantics live in operand.rs (write_operand does 32-zext /
/// 8-16-mask-insert BEFORE calling bd.reg_write with a u64), so gpr_w_zext=false.
#[cfg(target_arch = "aarch64")]
pub static X64_LAYOUT: sharpretro_jit::tier0::StateLayout = sharpretro_jit::tier0::StateLayout {
    state_words: STATE_WORDS_X64,
    off_pc: (OFF_RIP * 8) as u32,
    off_membase: (OFF_MEMBASE * 8) as u32,
    flag_file: 1,
    off_flags: (OFF_EFLAGS * 8) as u32,
    flag_bit: |idx| idx,   // eflags: idx IS the bit# directly (CF=0 PF=2 AF=4 ZF=6 SF=7 OF=11)
    reg_off: |f, idx| match f.0 {
        0 => (OFF_GPR as u32 + idx) * 8,       // GPR rax..r15
        2 => (OFF_SEG as u32 + idx) * 8,       // SEG es..gs
        3 => (OFF_XMM as u32 + idx * 2) * 8,   // XMM (2-word; ‡ tier-0 stores lo-only for now)
        _ => panic!("x64 tier-0: file {} not wired", f.0),
    },
    gpr_w_zext: false,
    off_rr: (OFF_RR * 8) as u32,
};

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

/// Read-set tracker — a RegState wrapper that records which (RegFile, idx) slots
/// an insn's eval READS, then delegates to an inner X86State. The libmoonage
/// TestGen.cs lazy-precondition-discovery loop's Rust equivalent: instead of
/// throw-on-missing (their MissingRegisterException), record the read-set on a
/// first pass with a zeroed inner state, then enumerate boundary-value pre-states
/// over exactly those slots. Also gives the corpus a principled exclusion: skip
/// if the read-set includes a mem_read (vs the v1-v3 mnemonic heuristic).
#[derive(Default)]
pub struct TrackingState {
    pub inner: X86State,
    /// (RegFile.0, idx) pairs read by the insn's eval. RefCell for &self reg_read.
    pub reads: std::cell::RefCell<Vec<(u8, u32)>>,
    /// (RegFile.0, idx) pairs written.
    pub writes: Vec<(u8, u32)>,
}

impl RegState for TrackingState {
    fn reg_read(&self, f: RegFile, idx: u32, ty: IlType) -> IVal {
        let mut r = self.reads.borrow_mut();
        if !r.contains(&(f.0, idx)) { r.push((f.0, idx)); }
        self.inner.reg_read(f, idx, ty)
    }
    fn reg_write(&mut self, f: RegFile, idx: u32, v: IVal) {
        if !self.writes.contains(&(f.0, idx)) { self.writes.push((f.0, idx)); }
        self.inner.reg_write(f, idx, v);
    }
    fn pc(&self) -> u64 { self.inner.pc() }
    fn set_pc(&mut self, pc: u64) { self.inner.set_pc(pc) }
    fn set_lr(&mut self, lr: u64) { self.inner.set_lr(lr) }
}

impl TrackingState {
    /// GPR indices this insn reads (RegFile 0 only).
    pub fn gpr_reads(&self) -> Vec<u32> {
        self.reads.borrow().iter().filter(|(f,_)| *f == 0).map(|(_,i)| *i).collect()
    }
    /// eflags bits this insn reads (RegFile 1).
    pub fn flag_reads(&self) -> Vec<u32> {
        self.reads.borrow().iter().filter(|(f,_)| *f == 1).map(|(_,i)| *i).collect()
    }
    /// Whether this insn reads XMM (RegFile 3) — v1 corpus excludes these until
    /// the stub loads xmm state (movdqu prologue at v2).
    pub fn reads_xmm(&self) -> bool {
        self.reads.borrow().iter().any(|(f,_)| *f == 3)
    }
    /// Which XMM indices this insn reads (RegFile 3). Phase-2 sweep uses this
    /// to drive the XMM pre-state boundary grid (like gpr_reads for GPR).
    pub fn xmm_reads(&self) -> Vec<u32> {
        let mut v: Vec<u32> = self.reads.borrow().iter()
            .filter(|(f,_)| *f == 3).map(|(_,i)| *i).collect();
        v.sort(); v.dedup(); v
    }
}

impl RegState for X86State {
    fn reg_read(&self, f: RegFile, idx: u32, ty: IlType) -> IVal {
        match f.0 {
            // Own #119 (interp half): GPR reads at width<64 must MASK. The
            // sext() xor-sub form assumes clean upper bits; unmasked, e.g.
            // reg_read(rax@I16) with rax=0x1234FFFB → sext(0x1234FFFB,16) =
            // ((v^0x8000)-0x8000) = 0x1233FFFB. Every partial-reg read into
            // sext was affected. tier-0's read_operand→Reg{width} path already
            // masks (via mask_to in the emitted code). Caught by the CWDE test.
            0 /*GPR*/ => {
                let v = self.gpr[idx as usize] as u128;
                let w = match ty { IlType::I{width,..} => width, _ => 64 };
                IVal { ty, bits: if w < 64 { v & ((1u128<<w)-1) } else { v } }
            },
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
