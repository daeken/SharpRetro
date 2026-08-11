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
//! v1: GPR + eflags only. v2 (`emit_stub_xmm`): + xmm0-15 via movdqu.
//! rsp is NOT loaded/stored (it's the anchor — insns touching rsp are excluded
//! by the fuzz-corpus; same as aarch64's SP exclusion).

use sharpretro_jit::x64_enc::X64Enc;
use crate::state::{OFF_GPR, OFF_EFLAGS, OFF_XMM};

/// Emit the v1 stub (GPR+eflags only). Returns (bytes, slot_off).
/// `slot_off` is the byte-offset of the TEST_INSN within the returned bytes
/// (constant across calls; asserted). Kept for phase-1 corpus compat
/// (SLOT_OFF=82; the runner + isa_diff assume it).
pub fn emit_stub(test_insn: &[u8]) -> (Vec<u8>, usize) {
    emit_stub_impl(test_insn, false)
}

/// Emit the v2 stub: v1 + xmm0-15 load/store via movdqu around the slot.
/// Larger stub (≈ SLOT_OFF+16 movdqu×2×16); a phase-2 XMM sweep uses this,
/// with a distinct format tag so runners know which SLOT_OFF applies.
/// ① 32-bit-mode stub (part c). PURE 32-bit code — the runner does the
/// 64→32 mode-switch (ljmp far *[m16:32] to CS=0x23) then this executes;
/// runner sets edi=state (a MAP_32BIT page → fits u32) + esp→[retf_eip:u32]
/// [retf_cs:u32] (also MAP_32BIT). edi = anchor (mirrors 64-bit's r15).
/// Loads eax-esi, pushes edi, loads guest-edi last; slot; pushes guest-edi,
/// reloads state-ptr from [esp+4], stores eax-esi+eflags+edi, add esp,8, retf.
/// esp NEVER loaded/stored (like rsp in 64-bit — anchor-preserving). r8-r15
/// don't exist. State layout unchanged (u64 slots, 8-byte stride) — stub
/// touches low-32 of each u64; corpus-gen masks pre.gpr[i]&=0xFFFF_FFFF and
/// zeroes gpr[8..16] so both silicon (writes low-32, high stays 0 from pre)
/// and interp (32-zext → high=0) produce identical u64s.
///
/// Every byte objdump-i386-verified before transcription (encode-then-
/// decode-back discipline). SLOT_OFF=29, total 85B (69 + 16 slot).
pub const STUB32_SLOT_OFF: usize = 29;
/// XMM variant: 8 movdqu loads (8B each) before the guest-edi load → slot
/// shifts +64. xmm0-7 only (no REX in 32-bit → xmm8-15 unreachable).
pub const STUB32_XMM_SLOT_OFF: usize = 29 + 64;

pub fn emit_stub_32(test_insn: &[u8]) -> (Vec<u8>, usize) {
    emit_stub_32_impl(test_insn, false)
}
pub fn emit_stub_32_xmm(test_insn: &[u8]) -> (Vec<u8>, usize) {
    emit_stub_32_impl(test_insn, true)
}

fn emit_stub_32_impl(test_insn: &[u8], xmm: bool) -> (Vec<u8>, usize) {
    debug_assert!(test_insn.len() <= 15);
    debug_assert_eq!(OFF_GPR, 0);
    debug_assert_eq!(OFF_EFLAGS, 16);  // byte offset 128 = 0x80 (disp32)
    debug_assert_eq!(OFF_XMM, 24);     // byte offset 192 = 0xC0 (disp32)
    let mut b: Vec<u8> = Vec::with_capacity(256);
    // ── prologue (29B) ──
    b.extend_from_slice(&[
        0xFF,0xB7,0x80,0x00,0x00,0x00,   // push dword [edi+0x80]  (state[16]=eflags)
        0x9D,                             // popfd
        0x8B,0x47,0x00,                   // mov eax,[edi+0x00]
        0x8B,0x4F,0x08,                   // mov ecx,[edi+0x08]
        0x8B,0x57,0x10,                   // mov edx,[edi+0x10]
        0x8B,0x5F,0x18,                   // mov ebx,[edi+0x18]
        0x8B,0x6F,0x28,                   // mov ebp,[edi+0x28]
        0x8B,0x77,0x30,                   // mov esi,[edi+0x30]
    ]);
    if xmm {
        // movdqu xmmN,[edi + 0xC0 + N*16]  (F3 0F 6F modrm disp32; objdump-
        // verified). state[OFF_XMM + N*2] = byte 192 + N*16.
        for x in 0..8u8 {
            b.extend_from_slice(&[0xF3,0x0F,0x6F, 0x87 | (x<<3)]);
            b.extend_from_slice(&((0xC0 + x as i32*16).to_le_bytes()));
        }
    }
    b.extend_from_slice(&[
        0x57,                             // push edi              (state-ptr → [sptr][retf...])
        0x8B,0x7F,0x38,                   // mov edi,[edi+0x38]    (guest-edi LAST; anchor gone)
    ]);
    let slot = b.len();
    debug_assert_eq!(slot, if xmm { STUB32_XMM_SLOT_OFF } else { STUB32_SLOT_OFF });
    // ── slot (16B, NOP-padded) ──
    b.extend_from_slice(test_insn);
    b.resize(slot + 16, 0x90);
    // ── epilogue (40B) ──
    b.extend_from_slice(&[
        0x57,                             // push edi              (guest-edi → [gedi][sptr][retf...])
        0x8B,0x7C,0x24,0x04,              // mov edi,[esp+4]       (reload state-ptr; SIB for esp)
        0x89,0x47,0x00,                   // mov [edi+0x00],eax
        0x89,0x4F,0x08,                   // mov [edi+0x08],ecx
        0x89,0x57,0x10,                   // mov [edi+0x10],edx
        0x89,0x5F,0x18,                   // mov [edi+0x18],ebx
        0x89,0x6F,0x28,                   // mov [edi+0x28],ebp
        0x89,0x77,0x30,                   // mov [edi+0x30],esi
    ]);
    if xmm {
        // movdqu [edi + 0xC0 + N*16],xmmN  (F3 0F 7F modrm disp32)
        for x in 0..8u8 {
            b.extend_from_slice(&[0xF3,0x0F,0x7F, 0x87 | (x<<3)]);
            b.extend_from_slice(&((0xC0 + x as i32*16).to_le_bytes()));
        }
    }
    b.extend_from_slice(&[
        0x9C,                             // pushfd
        0x8F,0x87,0x80,0x00,0x00,0x00,   // pop dword [edi+0x80]  (eflags → state[16])
        0x8B,0x04,0x24,                   // mov eax,[esp]         (= guest-edi; eax already saved)
        0x89,0x47,0x38,                   // mov [edi+0x38],eax
        0x83,0xC4,0x08,                   // add esp,8             (drop gedi+sptr → esp→retf-frame)
        0xCB,                             // retf                  → back to 64-bit trampoline
    ]);
    debug_assert_eq!(b.len(), if xmm { 85 + 128 } else { 85 });
    (b, slot)
}

pub fn emit_stub_xmm(test_insn: &[u8]) -> (Vec<u8>, usize) {
    emit_stub_impl(test_insn, true)
}

fn emit_stub_impl(test_insn: &[u8], xmm: bool) -> (Vec<u8>, usize) {
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
    if xmm {
        // Load xmm0..15 from state[OFF_XMM..] (2 words each = 16-byte stride).
        // BEFORE saving state-ptr (r15 still points at state[]). movdqu = no
        // alignment req (state[] is u64-aligned only).
        for x in 0..16u32 {
            e.movdqu_load(x, 15, ((OFF_XMM + x as usize * 2) * 8) as i32);
        }
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
    if xmm {
        // Store xmm0..15 back. r15 = state-ptr again by this point.
        for x in 0..16u32 {
            e.movdqu_store(15, ((OFF_XMM + x as usize * 2) * 8) as i32, x);
        }
    }
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

    #[test]
    fn stub_v2_xmm_decode_back() {
        let (bytes, slot) = emit_stub_xmm(&[0xCC]);
        eprintln!("stub-v2: {} bytes, SLOT_OFF = {}", bytes.len(), slot);
        // v1 SLOT_OFF=82; v2 adds 16 movdqu-load before slot. Each movdqu:
        // xmm0-3 [r15+0xC0..0xF0] disp32 (>0x7F) = 9 bytes; xmm4-7 same 9;
        // xmm8-15 add REX = 10 bytes. Actually just verify against emitted.
        std::fs::write("/tmp/x64stub_v2.bin", &bytes).unwrap();
        let out = Command::new("objdump")
            .args(["-D", "-b", "binary", "-m", "i386:x86-64", "-M", "intel", "/tmp/x64stub_v2.bin"])
            .output().unwrap();
        let d = String::from_utf8_lossy(&out.stdout);
        // 32 movdqu total (16 load + 16 store).
        let n_movdqu = d.lines().filter(|l| l.contains("movdqu")).count();
        assert_eq!(n_movdqu, 32, "expected 16 load + 16 store movdqu");
        // Spot-check both directions + REX.
        assert!(d.contains("movdqu xmm0,"),  "xmm0 load");
        assert!(d.contains("movdqu xmm15,"), "xmm15 load (REX.R)");
        // Store direction: 'movdqu XMMWORD PTR [r15+...],xmmN'
        assert!(d.lines().any(|l| l.contains("movdqu XMMWORD PTR [r15+") && l.contains("],xmm9")),
                "xmm9 store (REX.R+B, verified vs objdump earlier)");
        // No misdecodes.
        assert!(!d.contains(".byte"), ".byte = mis-encoded stub-v2 insn");
        // slot > v1's 82 (16 movdqu × 9-10 bytes ≈ 152 more).
        assert!(slot > 82 + 140 && slot < 82 + 170,
                "v2 slot_off {} outside expected 82+[140,170] range", slot);
        // XMM offsets: state[OFF_XMM=24]*8 = 0xC0; xmm15 at (24+30)*8 = 0x1B0.
        assert!(d.contains("[r15+0xc0]"),  "xmm0 at OFF_XMM*8=0xC0");
        assert!(d.contains("[r15+0x1b0]"), "xmm15 at (OFF_XMM+30)*8=0x1B0");
        // v2 SLOT_OFF derived from emit, not composed. Print for phase-2 corpus format.
        assert_eq!(slot, 226, "v2 SLOT_OFF (if this fails, update phase-2 corpus reader)");
    }
}

#[cfg(test)]
mod stub32_tests {
    use super::*;
    #[test]
    fn stub32_decode_back() {
        // Encode-then-decode-back: every hardcoded byte-string objdump-verified.
        // Emit with a 3-byte insn (add eax,ecx = 01 C8) → objdump the WHOLE
        // stub at i386 → verify structure (line count, slot at offset 29,
        // starts push/popfd, ends add-esp/retf, no misdecodes = no .byte).
        let (stub, slot) = emit_stub_32(&[0x01, 0xC8]);
        assert_eq!(stub.len(), 85);
        assert_eq!(slot, 29);
        std::fs::write("/tmp/stub32_test.bin", &stub).unwrap();
        let out = std::process::Command::new("objdump")
            .args(["-D","-b","binary","-m","i386","-M","intel","/tmp/stub32_test.bin"])
            .output().unwrap();
        let d = String::from_utf8_lossy(&out.stdout);
        let insns: Vec<&str> = d.lines().filter(|l| l.contains(":\t")).collect();
        eprintln!("  stub32 objdump ({} insns):", insns.len());
        for l in &insns { eprintln!("    {}", l.trim()); }
        // Structural checks. Line-count with a 2-byte test insn:
        // 10 prologue + 1 test + 14 nop + 14 epilogue = 39.
        assert_eq!(insns.len(), 39, "objdump line-count (misdecodes split lines)");
        assert!(d.contains("push   DWORD PTR [edi+0x80]"), "prologue eflags load");
        assert!(d.contains("popf"), "popfd");
        assert!(d.contains("1d:\t01 c8"), "test insn at slot offset 0x1d=29");
        assert!(d.contains("mov    edi,DWORD PTR [esp+0x4]"), "epilogue state-ptr reload");
        assert!(d.contains("add    esp,0x8"), "epilogue stack adjust");
        assert!(d.contains("retf"), "epilogue far-ret");
        assert!(!d.contains(".byte"), "no undecoded bytes → no misencoding");
    }
}

#[cfg(test)]
mod stub32_xmm_tests {
    use super::*;
    #[test]
    fn stub32_xmm_decode_back() {
        let (stub, slot) = emit_stub_32_xmm(&[0x0F, 0x58, 0xCA]);  // addps xmm1,xmm2
        assert_eq!(stub.len(), 213);
        assert_eq!(slot, STUB32_XMM_SLOT_OFF);
        std::fs::write("/tmp/stub32x_test.bin", &stub).unwrap();
        let out = std::process::Command::new("objdump")
            .args(["-D","-b","binary","-m","i386","-M","intel","/tmp/stub32x_test.bin"])
            .output().unwrap();
        let d = String::from_utf8_lossy(&out.stdout);
        let insns: Vec<&str> = d.lines().filter(|l| l.contains(":\t")).collect();
        // 10 prologue-gpr + 8 movdqu-load + 2 (push edi, mov edi) + 1 test +
        // 13 nop + 6 gpr-stores + 8 movdqu-store + 8 epilogue-tail = 56
        eprintln!("  stub32-xmm objdump: {} insns", insns.len());
        assert!(!d.contains(".byte"), "no undecoded bytes");
        assert!(d.contains("movdqu xmm0,XMMWORD PTR [edi+0xc0]"), "xmm load arm");
        assert!(d.contains("movdqu xmm7,XMMWORD PTR [edi+0x130]"), "xmm7 load");
        assert!(d.contains("movdqu XMMWORD PTR [edi+0xc0],xmm0"), "xmm store arm");
        assert!(d.contains("5d:\t0f 58 ca"), "test insn at slot 0x5d=93");
        assert!(d.contains("retf"), "far-ret");
    }
}
