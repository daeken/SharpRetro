//! x86-64 machine-code encoder — the `Emit` for the Rosetta-oracle stub
//! (and eventually a tier-0 x64-host arm). Minimal: only the shapes the
//! stub prologue/epilogue needs. Every encoding decode-back-verified via
//! `objdump -M intel` in `#[test]` (the encode-then-decode-back discipline).
//!
//! REX = 0x40 | W<<3 | R<<2 | X<<1 | B. R extends ModRM.reg, B extends
//! ModRM.rm / opcode+r. push/pop are d64 (no REX.W needed).

#![allow(dead_code)]

pub struct X64Enc {
    pub buf: Vec<u8>,
}

impl Default for X64Enc { fn default() -> Self { Self::new() } }

impl X64Enc {
    pub fn new() -> Self { Self { buf: vec![] } }
    pub fn bytes(&self) -> &[u8] { &self.buf }
    pub fn len(&self) -> usize { self.buf.len() }
    pub fn here(&self) -> usize { self.buf.len() }

    #[inline] fn b(&mut self, x: u8) { self.buf.push(x); }
    #[inline] fn i32_(&mut self, v: i32) { self.buf.extend_from_slice(&v.to_le_bytes()); }

    fn rex(&mut self, w: bool, r: u32, x: u32, b: u32) {
        let byte = 0x40 | ((w as u8) << 3) | (((r>>3)&1) as u8) << 2
                        | (((x>>3)&1) as u8) << 1 | ((b>>3)&1) as u8;
        // REX is optional when all bits 0 AND not accessing spl/bpl/sil/dil.
        // For the stub (all 64-bit regs), always emit when W or any hi-bit.
        if byte != 0x40 { self.b(byte); }
    }
    fn rex_wb(&mut self, b: u32) { self.rex(true, 0, 0, b); }
    fn rex_wrb(&mut self, r: u32, b: u32) { self.rex(true, r, 0, b); }

    /// ModRM for [rn + disp] with reg=rr. Handles the rsp-SIB and rbp-disp0 quirks.
    fn modrm_mem(&mut self, rr: u32, rn: u32, disp: i32) {
        let rn7 = (rn & 7) as u8;
        let rr7 = ((rr & 7) << 3) as u8;
        // rn low-3 == 4 (rsp/r12) → must emit SIB with base=rn, index=none(4).
        let need_sib = rn7 == 4;
        // rn low-3 == 5 (rbp/r13) with mod=00 → RIP-rel; force disp8=0 instead.
        let (modb, dsize) = if disp == 0 && rn7 != 5 { (0x00, 0) }
                       else if (-128..=127).contains(&disp) { (0x40, 1) }
                       else { (0x80, 4) };
        self.b(modb | rr7 | if need_sib { 4 } else { rn7 });
        if need_sib { self.b(0x20 | rn7); }  // scale=00 index=100(none) base=rn7
        if dsize == 1 { self.b(disp as u8); }
        else if dsize == 4 { self.i32_(disp); }
    }

    // ── the stub vocabulary ────────────────────────────────────────────────

    /// push r64  (50+r; d64 so no REX.W, but REX.B for r8-r15)
    pub fn push_r(&mut self, r: u32) {
        if r >= 8 { self.b(0x41); }
        self.b(0x50 | (r & 7) as u8);
    }
    /// pop r64
    pub fn pop_r(&mut self, r: u32) {
        if r >= 8 { self.b(0x41); }
        self.b(0x58 | (r & 7) as u8);
    }
    /// mov r64, [rn + disp]
    pub fn mov_r_m(&mut self, rt: u32, rn: u32, disp: i32) {
        self.rex_wrb(rt, rn);
        self.b(0x8B);
        self.modrm_mem(rt, rn, disp);
    }
    /// mov [rn + disp], r64
    pub fn mov_m_r(&mut self, rn: u32, disp: i32, rt: u32) {
        self.rex_wrb(rt, rn);
        self.b(0x89);
        self.modrm_mem(rt, rn, disp);
    }
    /// mov r64, r64
    pub fn mov_r_r(&mut self, rd: u32, rs: u32) {
        self.rex_wrb(rs, rd);
        self.b(0x89);
        self.b(0xC0 | (((rs & 7) << 3) | (rd & 7)) as u8);
    }
    /// push qword [rn + disp]  (FF /6)
    pub fn push_m(&mut self, rn: u32, disp: i32) {
        if rn >= 8 { self.b(0x41); }
        self.b(0xFF);
        self.modrm_mem(6, rn, disp);
    }
    /// pop qword [rn + disp]  (8F /0)
    pub fn pop_m(&mut self, rn: u32, disp: i32) {
        if rn >= 8 { self.b(0x41); }
        self.b(0x8F);
        self.modrm_mem(0, rn, disp);
    }
    /// add rsp, imm8  (48 83 C4 ib)
    pub fn add_rsp_i8(&mut self, imm: i8) {
        self.b(0x48); self.b(0x83); self.b(0xC4); self.b(imm as u8);
    }
    pub fn pushfq(&mut self) { self.b(0x9C); }
    pub fn popfq(&mut self) { self.b(0x9D); }
    pub fn ret(&mut self) { self.b(0xC3); }
    pub fn nop(&mut self) { self.b(0x90); }

    /// Pad to `align` bytes with single-byte NOPs (for the fixed-size TEST_INSN slot).
    pub fn pad_to(&mut self, off: usize) {
        while self.buf.len() < off { self.nop(); }
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Decode-back verification via objdump -M intel.
// ─────────────────────────────────────────────────────────────────────────────

#[cfg(test)]
mod tests {
    use super::*;
    use std::process::Command;

    fn disasm(bytes: &[u8]) -> String {
        std::fs::write("/tmp/x64enc_test.bin", bytes).unwrap();
        let out = Command::new("objdump")
            .args(["-D", "-b", "binary", "-m", "i386:x86-64", "-M", "intel",
                   "/tmp/x64enc_test.bin"])
            .output().unwrap();
        String::from_utf8_lossy(&out.stdout)
            .lines().filter(|l| l.contains(':') && l.contains('\t'))
            .map(|l| {
                // objdump line: "   0:  48 8b 07  mov    rax,QWORD PTR [rdi]"
                // → take from the mnemonic onward, collapse whitespace.
                let parts: Vec<_> = l.splitn(3, '\t').collect();
                parts.get(2).unwrap_or(&"").split_whitespace().collect::<Vec<_>>().join(" ")
            })
            .filter(|s| !s.is_empty())
            .collect::<Vec<_>>().join("\n")
    }

    #[test]
    fn encodings_decode_back() {
        let mut e = X64Enc::new();
        e.push_r(3);          // push rbx
        e.push_r(15);         // push r15
        e.pop_r(12);          // pop r12
        e.mov_r_m(0, 7, 0);   // mov rax, [rdi]
        e.mov_r_m(8, 7, 64);  // mov r8, [rdi+0x40]
        e.mov_r_m(2, 4, 8);   // mov rdx, [rsp+8]  (SIB path)
        e.mov_r_m(1, 5, 0);   // mov rcx, [rbp+0]  (rbp forces disp8=0)
        e.mov_r_m(3, 15, 256);// mov rbx, [r15+0x100]  (disp32)
        e.mov_m_r(7, 16, 9);  // mov [rdi+0x10], r9
        e.mov_r_r(15, 7);     // mov r15, rdi
        e.push_m(7, 128);     // push qword [rdi+0x80]
        e.pop_m(0, 128);      // pop qword [rax+0x80]
        e.pushfq();
        e.popfq();
        e.add_rsp_i8(8);
        e.ret();
        e.nop();

        let d = disasm(&e.buf);
        eprintln!("{d}");
        let expected = [
            "push rbx",
            "push r15",
            "pop r12",
            "mov rax,QWORD PTR [rdi]",
            "mov r8,QWORD PTR [rdi+0x40]",
            "mov rdx,QWORD PTR [rsp+0x8]",
            "mov rcx,QWORD PTR [rbp+0x0]",
            "mov rbx,QWORD PTR [r15+0x100]",
            "mov QWORD PTR [rdi+0x10],r9",
            "mov r15,rdi",
            "push QWORD PTR [rdi+0x80]",
            "pop QWORD PTR [rax+0x80]",
            "pushf",
            "popf",
            "add rsp,0x8",
            "ret",
            "nop",
        ];
        let lines: Vec<_> = d.lines().collect();
        assert_eq!(lines.len(), expected.len(), "insn count: got {} want {}\n{d}",
            lines.len(), expected.len());
        for (i, (got, want)) in lines.iter().zip(expected.iter()).enumerate() {
            assert!(got == want || got.starts_with(want),
                "insn #{i}: want `{want}`, got `{got}`");
        }
    }
}
