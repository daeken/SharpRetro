// x64-guest harness. Phase-1: decode-only spot-checks + XED corpus-diff.
use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::{decode_insn, DEF_MNEMONICS};

fn main() {
    let args: Vec<String> = std::env::args().collect();

    // <hex-bytes> — decode one insn, dump DecodedInsn.
    if args.len() >= 2 && args[1] != "--corpus" {
        let bytes: Vec<u8> = args[1].split(|c| c == ',' || c == ' ')
            .filter(|s| !s.is_empty())
            .map(|s| u8::from_str_radix(s.trim_start_matches("0x"), 16).unwrap())
            .collect();
        let mode = if args.get(2).map(|s| s.as_str()) == Some("32") { XMode::Bits32 } else { XMode::Bits64 };
        match decode_insn(&bytes, mode) {
            Some(d) => {
                println!("decoded: {} (def_id={}) len={}", DEF_MNEMONICS[d.def_id as usize], d.def_id, d.len);
                println!("  op=0x{:02X} rex=0x{:02X} v_width={}", d.op, d.p.rex, d.p.v_width(mode));
                if d.m.mod_ != 0 || d.m.reg != 0 || d.m.rm != 0 || d.m.is_reg {
                    println!("  modrm: mod={} reg={} rm={} is_reg={} base={} idx={} scale={} disp={} rip_rel={}",
                        d.m.mod_, d.m.reg, d.m.rm, d.m.is_reg, d.m.base_reg, d.m.index_reg, d.m.scale, d.m.disp, d.m.rip_relative);
                }
                if d.imm0 != 0 || d.imm1 != 0 { println!("  imm0=0x{:X} imm1=0x{:X}", d.imm0, d.imm1); }
            }
            None => println!("NOT DECODED"),
        }
        return;
    }

    // Default: spot-check a fixed set (bytes → expected mnemonic + len).
    let cases: &[(&[u8], &str, u32)] = &[
        (&[0x48, 0xB8, 0x2A,0,0,0, 0,0,0,0], "MOV", 10),   // mov rax, imm64
        (&[0x89, 0xC8], "MOV", 2),                          // mov eax, ecx
        (&[0x48, 0x89, 0xC8], "MOV", 3),                    // mov rax, rcx (REX.W)
        (&[0x48, 0x01, 0xD8], "ADD", 3),                    // add rax, rbx
        (&[0x83, 0xC0, 0x05], "ADD", 3),                    // add eax, 5 (Ib)
        (&[0xEB, 0xFE], "JMP", 2),                          // jmp -2 (rel8)
        (&[0xE8, 0x00,0,0,0], "CALL", 5),                   // call rel32
        (&[0x48, 0x8B, 0x05, 0x34,0x12,0,0], "MOV", 7),     // mov rax, [rip+0x1234]
        (&[0x48, 0x8B, 0x44, 0x88, 0x10], "MOV", 5),        // mov rax, [rax+rcx*4+0x10]
        (&[0x0F, 0x84, 0x00,0,0,0], "JZ", 6),               // jz rel32 (0F 84)
        (&[0xC3], "RET", 1),                                // ret
        (&[0x66, 0x0F, 0x6F, 0xC1], "MOVDQA", 4),           // movdqa xmm0, xmm1 (66 0F 6F)
    ];
    let mut ok = 0; let mut fail = 0;
    for (bytes, exp_mnem, exp_len) in cases {
        match decode_insn(bytes, XMode::Bits64) {
            Some(d) => {
                let mnem = DEF_MNEMONICS[d.def_id as usize];
                let mok = mnem.eq_ignore_ascii_case(exp_mnem);
                let lok = d.len == *exp_len;
                if mok && lok {
                    println!("  ✓ {:02X?} → {} len={}", bytes, mnem, d.len);
                    ok += 1;
                } else {
                    println!("  ✗ {:02X?} → {} len={} (expected {} len={})",
                        bytes, mnem, d.len, exp_mnem, exp_len);
                    fail += 1;
                }
            }
            None => { println!("  ✗ {:02X?} → NOT DECODED (expected {})", bytes, exp_mnem); fail += 1; }
        }
    }
    println!("[spot-checks: {ok} ok, {fail} fail]");
}
