// x64-guest harness. Phase-1: decode-only spot-checks + XED corpus-diff.
// Phase-2/3: --interp = execute via InterpretingBuilder × X86State × hand_lift.
use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::{decode_insn, DEF_MNEMONICS};
use xfusion_recomp::state::X86State;
use xfusion_recomp::hand_lift::lift_one;
use sharpretro_jit::interp::{InterpretingBuilder, FlatMem, GuestMem};

/// Execute one x64 insn via decode_insn + lift_one + InterpretingBuilder.
/// Returns (post-state, insn-len, branched, handled). Mirrors aarch64 interp_one shape.
fn interp_one_x64(pre: &X86State, mem: &mut impl GuestMem, code: &[u8], pc: u64)
    -> (X86State, u32, bool, bool)
{
    let d = match decode_insn(code, XMode::Bits64) {
        Some(d) => d,
        None => panic!("undecoded @0x{pc:x}: {:02X?}", &code[..code.len().min(4)]),
    };
    let mut s = pre.clone();
    s.rip = pc;
    let (branched, handled);
    {
        let mut b = InterpretingBuilder::new(&mut s, mem, pc);
        b.intrinsic = |_,_,id,_| panic!("intrinsic id={id} not wired");
        handled = lift_one(&mut b, &d, pc, XMode::Bits64);
        branched = b.branched;
    }
    if !branched { s.rip = pc + d.len as u64; }
    (s, d.len, branched, handled)
}

fn main() {
    let args: Vec<String> = std::env::args().collect();

    // --interp <hex-bytes> [reg=val ...] — decode + lift + execute one insn (or a
    // sequence separated by /), dump changed regs. Phase-2/3 first-execute.
    if args.get(1).map(|s| s.as_str()) == Some("--interp") {
        let mut s = X86State::default();
        let mut mem = FlatMem::new(0, 0x100000);
        s.gpr[4] = 0x80000;  // rsp = mid-mem so PUSH/CALL/RET have a stack
        let mut prog: Vec<Vec<u8>> = vec![];
        for a in &args[2..] {
            if let Some((r, v)) = a.split_once('=') {
                let val = u64::from_str_radix(v.trim_start_matches("0x"), 16).unwrap();
                match r {
                    "rax" => s.gpr[0] = val, "rcx" => s.gpr[1] = val, "rdx" => s.gpr[2] = val,
                    "rbx" => s.gpr[3] = val, "rsp" => s.gpr[4] = val, "rbp" => s.gpr[5] = val,
                    "rsi" => s.gpr[6] = val, "rdi" => s.gpr[7] = val,
                    _ => if let Some(n) = r.strip_prefix('r') { s.gpr[n.parse::<usize>().unwrap()] = val; }
                }
            } else {
                for insn_str in a.split('/') {
                    let bytes: Vec<u8> = insn_str.split(',')
                        .filter(|x| !x.is_empty())
                        .map(|x| u8::from_str_radix(x.trim().trim_start_matches("0x"), 16).unwrap())
                        .collect();
                    prog.push(bytes);
                }
            }
        }
        let pre = s.clone();
        let mut pc = 0x1000u64;
        for bytes in &prog {
            let (post, len, branched, handled) = interp_one_x64(&s, &mut mem, bytes, pc);
            let mnem = decode_insn(bytes, XMode::Bits64)
                .map(|d| DEF_MNEMONICS[d.def_id as usize]).unwrap_or("?");
            println!("→ 0x{pc:x}: {:02X?}  {} len={} {}",
                bytes, mnem, len, if handled { "" } else { "  ✗ NOT-HANDLED (no lift)" });
            s = post;
            pc = if branched { s.rip } else { pc + len as u64 };
        }
        println!("─── final state (changed only) ───");
        let names = ["rax","rcx","rdx","rbx","rsp","rbp","rsi","rdi",
                     "r8","r9","r10","r11","r12","r13","r14","r15"];
        for i in 0..16 { if s.gpr[i] != pre.gpr[i] {
            println!("  {:4} = 0x{:016X}  (was 0x{:X})", names[i], s.gpr[i], pre.gpr[i]); } }
        if s.eflags != pre.eflags {
            println!("  eflags = 0x{:08X}  CF={} ZF={} SF={} OF={}",
                s.eflags, s.cf() as u8, s.zf() as u8, s.sf() as u8, s.of() as u8);
        }
        println!("  rip  = 0x{:X}", s.rip);
        return;
    }

    // --corpus <file> [<hex-off> <hex-len>] — linear-sweep bytes through decode_insn,
    // count decoded/undecoded + dump per-insn (offset, len, mnem) for C#-diff.
    // With --dump: print `offset len def_id mnem` per insn (the diff-target format).
    if args.get(1).map(|s| s.as_str()) == Some("--corpus") {
        let path = &args[2];
        let all = std::fs::read(path).expect("read corpus");
        let off = args.get(3).map(|s| usize::from_str_radix(s.trim_start_matches("0x"), 16).unwrap()).unwrap_or(0);
        let len = args.get(4).map(|s| usize::from_str_radix(s.trim_start_matches("0x"), 16).unwrap()).unwrap_or(all.len() - off);
        let dump = args.iter().any(|a| a == "--dump");
        let bytes = &all[off..off+len];
        let mut i = 0usize;
        let (mut n_ok, mut n_fail, mut fail_at) = (0usize, 0usize, vec![]);
        while i < bytes.len() {
            match decode_insn(&bytes[i..], XMode::Bits64) {
                Some(d) if d.len > 0 => {
                    if dump { println!("{:x} {} {} {}", off+i, d.len, d.def_id, DEF_MNEMONICS[d.def_id as usize]); }
                    n_ok += 1;
                    i += d.len as usize;
                }
                _ => {
                    // Undecoded byte — record + skip 1 (linear-sweep resync).
                    if fail_at.len() < 20 { fail_at.push((off+i, bytes[i])); }
                    n_fail += 1;
                    i += 1;
                }
            }
        }
        eprintln!("[corpus {}: off=0x{:x} len=0x{:x}]", path, off, len);
        eprintln!("  decoded: {} insns  undecoded: {} bytes  ({:.2}% coverage by count)",
            n_ok, n_fail, 100.0 * n_ok as f64 / (n_ok + n_fail).max(1) as f64);
        if !fail_at.is_empty() {
            eprintln!("  first undecoded bytes:");
            for (o, b) in &fail_at { eprintln!("    0x{:x}: 0x{:02X}", o, b); }
        }
        return;
    }

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
