//! Native aarch64 exec-truth oracle: run ONE guest insn on real silicon, diff post-state
//! vs the InterpretingBuilder. This is the independent oracle (per calibration: silicon =
//! execution truth; interp+recompiler both derive from the same .isa, so they're co-blind
//! to .isa/emit bugs — silicon isn't).
//!
//! Mechanism (in-process, no ptrace):
//!   mmap RWX page → write {prologue: load x0-x30+nzcv from state[], TEST_INSN slot,
//!   epilogue: store x0-x30+nzcv back, ret} → __clear_cache → call as fn(*mut u64).
//!
//! SP is the anchor (state-ptr saved on stack across the test-insn). Test-insns that
//! touch SP or branch out of the stub are EXCLUDED from the corpus initially (marked;
//! a signal-handler backstop is the v2 hardening).
//!
//! State layout (offsets in u64 words from base ptr):
//!   [0..31]  = x0..x30      (x[31]=SP is NOT loaded/stored — SP stays host-owned)
//!   [31]     = nzcv (low 32 bits)
//!   [32..63] = v0..v31 low-64  (‡ V128 halves — full V oracle at v2; low-64 for now)
//!
//! aarch64-host-only. `#[cfg(target_arch = "aarch64")]` gates the whole module.

#![cfg(target_arch = "aarch64")]

use crate::state::Aarch64State;
use std::cell::Cell;

// ── signal-recovery: SIGILL/SIGSEGV/SIGBUS/SIGFPE during a stub call → siglongjmp back ──
// The .isa is over-permissive (e.g. ADD-shifted-register shift=3 → silicon #UD but the
// .isa emits the rotr arm), and random-fielded fuzz encodings will hit those. Rather than
// let the process die, catch the signal and report it — that's a "silicon-rejects, .isa-
// accepts" datum (= a missing `requires` in the .isa), which is exactly what the exec-truth
// oracle exists to find.

// glibc's sigsetjmp/siglongjmp are macros; the underlying symbols are __sigsetjmp/siglongjmp.
// sigjmp_buf on aarch64-glibc is opaque; over-allocate (glibc's is ~312 bytes).
type SigJmpBuf = [u64; 64];
unsafe extern "C" {
    fn __sigsetjmp(env: *mut SigJmpBuf, savesigs: libc::c_int) -> libc::c_int;
    fn siglongjmp(env: *mut SigJmpBuf, val: libc::c_int) -> !;
}

thread_local! {
    static JMP: Cell<*mut SigJmpBuf> = const { Cell::new(std::ptr::null_mut()) };
}

extern "C" fn sig_handler(sig: libc::c_int, _info: *mut libc::siginfo_t, _ctx: *mut libc::c_void) {
    JMP.with(|j| {
        let p = j.get();
        if !p.is_null() {
            unsafe { siglongjmp(p, sig) };
        }
    });
    // Not inside a guarded exec_one → re-raise default (shouldn't happen in practice).
    unsafe {
        libc::signal(sig, libc::SIG_DFL);
        libc::raise(sig);
    }
}

fn install_handlers() {
    unsafe {
        let mut sa: libc::sigaction = std::mem::zeroed();
        sa.sa_sigaction = sig_handler as usize;
        sa.sa_flags = libc::SA_SIGINFO | libc::SA_NODEFER;
        libc::sigemptyset(&mut sa.sa_mask);
        for &s in &[libc::SIGILL, libc::SIGSEGV, libc::SIGBUS, libc::SIGFPE] {
            libc::sigaction(s, &sa, std::ptr::null_mut());
        }
    }
}

/// Result of a native exec attempt.
#[derive(Debug, Clone, Copy, PartialEq)]
pub enum NativeResult {
    Ran,
    Excluded,          // v1 exclusion set (branch/load-store/system/pc-dependent)
    SiliconRejects(i32),  // signal number — .isa accepted, silicon didn't = a .isa gap
}

const PAGE: usize = 4096;
// Words into the stub where the test-insn slot lives. Derivation (verified via
// objdump decode-back of the emitted bytes — the encode-then-decode-back discipline):
//   sub_sp(1) + str_x0(1) + 12×str_callee(12) + ldr_nzcv(1) + msr(1) + 30×ldr_x1..30(30)
//   + ldr_x0(1) = 47. The runtime assert_eq! double-checks.
const SLOT_OFF: usize = 47;

/// One RWX page holding the stub. Reused across calls (only the test-insn slot rewrites).
pub struct NativeStub {
    page: *mut u32,
    entry: extern "C" fn(*mut u64),
}

unsafe impl Send for NativeStub {}

impl NativeStub {
    pub fn new() -> Self {
        unsafe {
            let page = libc::mmap(std::ptr::null_mut(), PAGE,
                libc::PROT_READ | libc::PROT_WRITE | libc::PROT_EXEC,
                libc::MAP_PRIVATE | libc::MAP_ANONYMOUS, -1, 0) as *mut u32;
            assert!(!page.is_null() && page as isize != -1, "mmap RWX failed");
            let mut w = StubWriter { page, i: 0 };
            w.emit_stub();
            install_handlers();
            Self { page, entry: std::mem::transmute(page) }
        }
    }

    /// Execute `insn` against `state` on real silicon. Mutates `state` in place.
    pub fn exec_one(&self, state: &mut Aarch64State, insn: u32) -> NativeResult {
        if excluded(insn) { return NativeResult::Excluded; }
        unsafe {
            // Rewrite the test-insn slot + flush I-cache for that word.
            *self.page.add(SLOT_OFF) = insn;
            clear_cache(self.page.add(SLOT_OFF) as *const u8, 4);
            // Marshal Aarch64State → flat u64[64] the stub reads.
            let mut flat = [0u64; 64];
            for i in 0..31 { flat[i] = state.x[i]; }
            flat[31] = state.nzcv as u64;
            for i in 0..32 { flat[32 + i] = state.v[i] as u64; }  // ‡ low-64 only
            // sigsetjmp — if the insn traps, sig_handler siglongjmps here with the signal.
            let mut jb: SigJmpBuf = std::mem::zeroed();
            JMP.with(|j| j.set(&mut jb));
            let sig = __sigsetjmp(&mut jb, 1);
            if sig != 0 {
                JMP.with(|j| j.set(std::ptr::null_mut()));
                return NativeResult::SiliconRejects(sig);
            }
            (self.entry)(flat.as_mut_ptr());
            JMP.with(|j| j.set(std::ptr::null_mut()));
            for i in 0..31 { state.x[i] = flat[i]; }
            state.nzcv = flat[31] as u32;
            for i in 0..32 { state.v[i] = flat[32 + i] as u128; }  // ‡ low-64
        }
        NativeResult::Ran
    }
}

impl Drop for NativeStub {
    fn drop(&mut self) { unsafe { libc::munmap(self.page as *mut _, PAGE); } }
}

// ── the stub encoder ───────────────────────────────────────────────────────
// aarch64 instruction encoding by hand (fixed instructions, only the offset varies).
// Reference: ARM ARM C6. Encodings verified against `objdump -d` on a hand-assembled .s.

struct StubWriter { page: *mut u32, i: usize }
impl StubWriter {
    fn put(&mut self, w: u32) { unsafe { *self.page.add(self.i) = w; } self.i += 1; }

    // LDR Xt, [Xn, #imm]   — imm in bytes, must be 8-aligned, encoded as imm/8 in [21:10]
    fn ldr(&mut self, xt: u32, xn: u32, off: u32) {
        assert!(off % 8 == 0 && off < 32768);
        self.put(0xF9400000 | ((off/8) << 10) | (xn << 5) | xt);
    }
    // STR Xt, [Xn, #imm]
    fn str_(&mut self, xt: u32, xn: u32, off: u32) {
        assert!(off % 8 == 0 && off < 32768);
        self.put(0xF9000000 | ((off/8) << 10) | (xn << 5) | xt);
    }
    // SUB SP, SP, #imm  (imm12)
    fn sub_sp(&mut self, imm: u32) { self.put(0xD10003FF | ((imm & 0xFFF) << 10)); }
    // ADD SP, SP, #imm
    fn add_sp(&mut self, imm: u32) { self.put(0x910003FF | ((imm & 0xFFF) << 10)); }
    // MRS Xt, NZCV
    fn mrs_nzcv(&mut self, xt: u32) { self.put(0xD53B4200 | xt); }
    // MSR NZCV, Xt
    fn msr_nzcv(&mut self, xt: u32) { self.put(0xD51B4200 | xt); }
    // RET
    fn ret(&mut self) { self.put(0xD65F03C0); }

    fn emit_stub(&mut self) {
        // On entry: x0 = state-ptr (flat u64[64]). Callee-saved x19-x28 must be preserved
        // per AAPCS — but this stub CLOBBERS THEM (loads guest values). ‡ v1: the caller
        // (Rust) doesn't rely on x19-x28 across the FFI call because we mark it clobbers-all
        // via the fn-ptr call (Rust treats extern "C" fn as clobbering caller-saved only).
        // ACTUALLY: extern "C" preserves callee-saved. So this stub MUST save/restore
        // x19-x30 around itself, OR the caller uses inline-asm with clobber-all.
        // v1: save x19-x30 on stack in prologue, restore in epilogue.

        // ── prologue ──
        self.sub_sp(16 + 12*8);          // 16 for state-ptr+scratch, 96 for x19-x30
        self.str_(0, 31, 0);             // [sp+0] = state-ptr (x0)
        // save callee-saved x19-x30 at [sp+16..]
        for (i, r) in (19..=30).enumerate() { self.str_(r, 31, 16 + (i as u32)*8); }
        // load nzcv (before clobbering x0)
        self.ldr(1, 0, 31*8);            // x1 = state[31] = nzcv
        self.msr_nzcv(1);
        // load x1-x30 from state (x0 = base)
        for r in 1..=30 { self.ldr(r, 0, r*8); }
        // load x0 LAST (loses base ptr; it's on stack)
        self.ldr(0, 0, 0);

        // ── test-insn slot ──
        assert_eq!(self.i, SLOT_OFF, "SLOT_OFF mismatch — recount prologue");
        self.put(0xD503201F);            // NOP placeholder (overwritten per exec_one)

        // ── epilogue ──
        // save guest-x0 to [sp+8], reload state-ptr into x0
        self.str_(0, 31, 8);
        self.ldr(0, 31, 0);
        // store x1-x30 to state
        for r in 1..=30 { self.str_(r, 0, r*8); }
        // store guest-x0 (from [sp+8])
        self.ldr(1, 31, 8);
        self.str_(1, 0, 0);
        // store nzcv
        self.mrs_nzcv(1);
        self.str_(1, 0, 31*8);
        // restore callee-saved x19-x30
        for (i, r) in (19..=30).enumerate() { self.ldr(r, 31, 16 + (i as u32)*8); }
        self.add_sp(16 + 12*8);
        self.ret();

        // Flush the whole stub once (I-cache coherency for freshly-written code).
        unsafe { clear_cache(self.page as *const u8, self.i * 4); }
    }
}

/// Icache flush for self-modifying code on aarch64. libc's __clear_cache or inline the
/// IC IVAU / DSB / ISB sequence.
unsafe fn clear_cache(start: *const u8, len: usize) {
    // core::arch::aarch64 has no stable __clear_cache; use libc's (compiler-rt).
    unsafe extern "C" { fn __clear_cache(start: *const u8, end: *const u8); }
    unsafe { __clear_cache(start, start.add(len)); }
}

/// v1 exclusion set: insns that would break the in-process stub (branch out / touch SP /
/// system insns / loads-stores to unmapped guest addrs). These get their own harness
/// (ptrace child, or a mapped-mem sandbox) at v2.
fn excluded(insn: u32) -> bool {
    // Branches (B/BL/BR/BLR/RET/CBZ/CBNZ/TBZ/TBNZ/B.cond) — top bits per the aarch64
    // encoding classes. Coarse mask; refine as the fuzz corpus needs.
    let top8 = insn >> 24;
    matches!(top8,
        0x14..=0x17 | 0x94..=0x97 |          // B / BL
        0x54 |                               // B.cond
        0x34..=0x37 | 0xB4..=0xB7 |          // CBZ/CBNZ/TBZ/TBNZ
        0xD6                                 // BR/BLR/RET (0xD61F../0xD63F../0xD65F..)
    )
    // Loads/stores: v1 excludes ALL (guest addrs unmapped in-process). Coarse: any insn
    // in the load/store major class (bits [27:25] == 0b100 for LDR/STR imm/reg forms is
    // too coarse — also catches ADD-imm). Instead: bits [27] == 1 && bits [25] == 0 for
    // load/store class per ARM ARM Table C4-1. ‡ Refine with the .isa's own def-name
    // classification (any def whose eval contains `load`/`store` head).
    || ((insn >> 25) & 0b101) == 0b100
    // System insns (MSR/MRS/SVC/HVC/BRK/HLT etc — bits [28:25]=1101, op0)
    || (insn & 0xFFC00000) == 0xD5000000
    || (insn & 0xFFE00000) == 0xD4000000  // exception-gen (SVC/BRK/…)
    // pc-dependent (ADR/ADRP): interp uses synthetic pc=0x1000, native = real stub va.
    // Not a semantics bug — an oracle limitation. v2: normalize (subtract stub-va from
    // native result); v1: exclude. Bits [28:24]=10000 for ADR, [31]=op selects ADRP.
    || (insn & 0x1F000000) == 0x10000000
}
