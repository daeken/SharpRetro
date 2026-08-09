//! x64 Rosetta-oracle stub: hand-encoded x86-64 machine-code that loads all
//! guest state from `state[]` (rdi), leaves a 16-byte TEST_INSN slot, stores
//! all state back, returns. This box EMITS the bytes; the Mac-side runner
//! (tools/rosetta_oracle_runner.c on the Mac) mmaps + executes them under Rosetta.
//!
//! Contract (matches state.rs u64[90] layout):
//!   entry: extern "C" fn(state: *mut u64)   — SysV: state in rdi
//!   prologue: save callee-saved (rbx rbp r12-r15) → r15 = state-ptr
//!             → load rax..r14 from state[0..15] → push state[16] → popfq
//!             → load r15 from state[15] LAST (loses state-ptr; saved on stack)
//!   TEST_INSN slot: 16 bytes (padded with NOP; x86 max insn = 15 bytes)
//!   epilogue: push guest-r15 → reload state-ptr from stack → store rax..r14
//!             → pushfq → pop into state[16] → store guest-r15 (from stack)
//!             → restore callee-saved → ret
//!
//! v1: GPR + eflags only. XMM (movdqu xmm[i], [state+192+i*16]) at v2.
//! rsp is NOT loaded/stored (it's the anchor — insns touching rsp are excluded
//! by the fuzz-corpus; same as aarch64's SP exclusion).

use sharpretro_jit::x64_enc::X64Enc;
use crate::state::{OFF_GPR, OFF_EFLAGS};

/// Emit the stub with `test_insn` in the slot. Returns (bytes, slot_off).
/// `slot_off` is the byte-offset of the TEST_INSN within the returned bytes
/// (constant across calls; asserted).
pub fn emit_stub(test_insn: &[u8]) -> (Vec<u8>, usize) {
    debug_assert!(test_insn.len() <= 15, "x86 insn max 15 bytes");
    let mut e = X64Enc::new();

    // ── prologue ──
    // Callee-saved per SysV: rbx rbp r12-r15. Save on stack.
    for r in [3, 5, 12, 13, 14, 15] { e.push_r(r); }
    // r15 = state-ptr (rdi). We keep r15 as the anchor until it's the last reg loaded.
    e.mov_r_r(15, 7);
    // Load eflags: push [r15+OFF_EFLAGS*8]; popfq. Do this BEFORE loading GPRs so
    // the loads (mov) don't clobber flags — actually mov doesn't touch flags, but
    // popfq itself uses rsp, and we haven't loaded guest-rsp anyway. Order-safe.
    e.push_m(15, (OFF_EFLAGS * 8) as i32);
    e.popfq();
    // Load rax..r14 from state[0..15]. Skip rsp(4) — anchor. Skip r15(15) — do LAST.
    for r in (0..15u32).filter(|&r| r != 4) {
        e.mov_r_m(r, 15, (OFF_GPR + r as usize) as i32 * 8);
    }
    // Save state-ptr on stack (we're about to lose r15).
    e.push_r(15);
    // Load r15 LAST.
    e.mov_r_m(15, 15, (OFF_GPR + 15) as i32 * 8);

    let slot_off = e.here();
    // ── TEST_INSN slot (16 bytes, NOP-padded) ──
    e.buf.extend_from_slice(test_insn);
    e.pad_to(slot_off + 16);
    debug_assert_eq!(e.here(), slot_off + 16);

    // ── epilogue ──
    // Save guest-r15 on stack; reload state-ptr into r15 from where prologue pushed it.
    // Stack right now: [state-ptr][callee-saved×6]. push guest-r15 → [gr15][sptr][cs×6].
    // Then: mov r15, [rsp+8] to get state-ptr (rsp+0=gr15, rsp+8=sptr).
    e.push_r(15);
    e.mov_r_m(15, 4 /*rsp*/, 8);
    // Store rax..r14 back. Skip rsp(4). r15 handled separately.
    for r in (0..15u32).filter(|&r| r != 4) {
        e.mov_m_r(15, (OFF_GPR + r as usize) as i32 * 8, r);
    }
    // Store eflags: pushfq; pop [r15+OFF_EFLAGS*8].
    e.pushfq();
    e.pop_m(15, (OFF_EFLAGS * 8) as i32);
    // Store guest-r15: it's at [rsp+0] (we pushed it above the state-ptr).
    e.mov_r_m(0, 4 /*rsp*/, 0);          // rax = guest-r15 (rax already saved above)
    e.mov_m_r(15, (OFF_GPR + 15) as i32 * 8, 0);
    // Clean stack: pop guest-r15 + state-ptr (2×8=16 bytes).
    e.add_rsp_i8(16);
    // Restore callee-saved (reverse order).
    for r in [15, 14, 13, 12, 5, 3] { e.pop_r(r); }
    e.ret();

    (e.buf, slot_off)
}

/// The stub's SLOT_OFF is a compile-time constant (prologue is fixed-length).
/// Assert it once against emit_stub's actual output — the encode-then-decode-back
/// discipline: derive it from the emitted bytes, don't compose it in-head.
pub fn stub_slot_off() -> usize {
    let (_, off) = emit_stub(&[0x90]);  // NOP
    off
}

#[cfg(test)]
mod tests {
    use super::*;
    use std::process::Command;

    #[test]
    fn stub_decode_back() {
        // Emit with a marker insn (int3 = 0xCC) so we can see the slot boundary.
        let (bytes, slot) = emit_stub(&[0xCC]);
        eprintln!("stub: {} bytes, SLOT_OFF = {}", bytes.len(), slot);
        std::fs::write("/tmp/x64stub.bin", &bytes).unwrap();
        let out = Command::new("objdump")
            .args(["-D", "-b", "binary", "-m", "i386:x86-64", "-M", "intel", "/tmp/x64stub.bin"])
            .output().unwrap();
        let d = String::from_utf8_lossy(&out.stdout);
        eprintln!("{d}");
        // Sanity checks: prologue/epilogue mnemonics present, int3 at slot.
        assert!(d.contains("push   rbx"));
        assert!(d.contains("push   r15"));
        assert!(d.contains("mov    r15,rdi"));
        assert!(d.contains("popf"));
        assert!(d.contains("int3"));
        assert!(d.contains("pushf"));
        assert!(d.contains("pop    r15"));  // the callee-restore
        assert!(d.contains("ret"));
        // Verify no `.byte` (undecoded) — every byte the stub emits should disassemble.
        assert!(!d.contains(".byte"), ".byte = mis-encoded stub insn");
        // eflags-slot offset verify: OFF_EFLAGS*8=128 must use disp32 (0x80 in disp8=signed=-128)
        assert!(d.contains("[r15+0x80]"), "eflags at [r15+0x80] should decode via disp32");
    }
}
