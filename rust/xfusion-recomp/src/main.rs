// x64-guest harness. Phase-1: decode-only spot-checks + XED corpus-diff.
// Phase-2/3: --interp = execute via InterpretingBuilder × X86State × hand_lift.
use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::{decode_insn, DEF_MNEMONICS};
use xfusion_recomp::state::X86State;
// Generated lift.rs (278 templates × 506 defs). hand_lift stays as the reference
// (spot-checkable per-mnemonic) but the primary path is the generated one.
use xfusion_recomp::lift::lift_one;
#[allow(unused_imports)]
use xfusion_recomp::hand_lift as _;
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

    // --run-x64 <program> — the x64 block-driver: load a small x64 program into
    // guest memory, run pc-driven via interp until INT3. Proves the block-loop
    // shape for x64 (variable-length fetch + branch-following + RET-via-stack).
    // Tier-0-x64 needs a StateLayout refactor (Tier0's state offsets are aarch64-
    // shaped); this interp-driver proves the loop independent of that.
    if args.get(1).map(|s| s.as_str()) == Some("--run-x64") {
        // sum10: eax = Σ 1..10 = 55. mov/add/inc/cmp/jl/int3.
        let sum10: &[u8] = &[
            0xB8, 0x00,0,0,0,       // mov eax, 0
            0xB9, 0x01,0,0,0,       // mov ecx, 1
            0xBA, 0x0B,0,0,0,       // mov edx, 11
            // loop:
            0x01, 0xC8,             // add eax, ecx
            0xFF, 0xC1,             // inc ecx
            0x39, 0xD1,             // cmp ecx, edx
            0x7C, 0xF8,             // jl loop  (rel8 = -8)
            0xCC,                   // int3
        ];
        // fib(N) with call/ret — exercises push/pop/call/ret stack-mem.
        let fib: &[u8] = &[
            // main: mov edi, 12; call fib; int3
            0xBF, 0x0C,0,0,0,       // mov edi, 12
            0xE8, 0x01,0,0,0,       // call +1 (fib)
            0xCC,                   // int3
            // fib: rax=0 rcx=1; loop N-1 times: rdx=rax+rcx; rax=rcx; rcx=rdx; dec edi; jg loop; ret
            0xB8, 0x00,0,0,0,       // mov eax, 0
            0xB9, 0x01,0,0,0,       // mov ecx, 1
            // loop:
            0x48, 0x8D, 0x14, 0x08, // lea rdx, [rax+rcx]
            0x48, 0x89, 0xC8,       // mov rax, rcx
            0x48, 0x89, 0xD1,       // mov rcx, rdx
            0xFF, 0xCF,             // dec edi
            0x83, 0xFF, 0x01,       // cmp edi, 1
            0x7F, 0xEF,             // jg loop  (rel8 = -17)
            0xC3,                   // ret
        ];
        let prog: &[u8] = match args.get(2).map(|s| s.as_str()) {
            Some("sum10") | None => sum10,
            Some("fib") => fib,
            Some(hex) => Box::leak(hex.split(',')
                .map(|s| u8::from_str_radix(s.trim().trim_start_matches("0x"), 16).unwrap())
                .collect::<Vec<_>>().into_boxed_slice()),
        };
        let entry = 0x10000u64;
        let mut mem = FlatMem::new(0, 0x100000);
        // Load program at entry.
        for (i, &b) in prog.iter().enumerate() { mem.write(entry + i as u64, 8, b as u128); }

        let mut s = X86State::default();
        s.rip = entry;
        s.gpr[4] = 0x80000;  // rsp
        let max_insns = 10000;
        let mut n = 0;
        loop {
            let pc = s.rip;
            // Fetch up to 15 bytes from guest mem.
            let mut buf = [0u8; 15];
            for i in 0..15 { buf[i] = mem.read(pc + i as u64, 8) as u8; }
            // INT3 = stop.
            if buf[0] == 0xCC { break; }
            let (post, len, branched, handled) = interp_one_x64(&s, &mut mem, &buf, pc);
            if !handled {
                let mnem = decode_insn(&buf, XMode::Bits64)
                    .map(|d| DEF_MNEMONICS[d.def_id as usize]).unwrap_or("?");
                println!("  UNHANDLED @0x{pc:x}: {:02X?} {}", &buf[..len as usize], mnem);
                break;
            }
            s = post;
            if !branched { s.rip = pc + len as u64; }
            n += 1;
            if n > max_insns { println!("  max_insns hit"); break; }
        }
        println!("[run-x64: {} insns, rax=0x{:X} rcx=0x{:X} rip=0x{:X}]",
            n, s.gpr[0], s.gpr[1], s.rip);
        return;
    }

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

    // --fuzz-x64 [N] [--emit-corpus <file>] — for each of the 506 def-encodings:
    // synthesize N random-fielded valid encodings + random pre-state, execute via
    // interp (this box), optionally emit the {stub_bytes, pre_state, interp_post}
    // triple to a corpus-file for the Mac-side Rosetta-oracle runner.
    if args.get(1).map(|s| s.as_str()) == Some("--fuzz-x64") {
        use xfusion_recomp::x64_stub::emit_stub;
        use xfusion_recomp::state::X86State;
        use xfusion_recomp::lift::DEF_FLAGS_MASK;
        use std::io::Write;

        let n: usize = args.get(2).and_then(|s| s.parse().ok()).unwrap_or(3);
        let seed: u64 = args.iter().position(|a| a == "--seed")
            .and_then(|i| args.get(i+1)).and_then(|s| s.parse().ok()).unwrap_or(0xC0FFEE);
        let corpus_path = args.iter().position(|a| a == "--emit-corpus")
            .and_then(|i| args.get(i+1).cloned());
        let mut rng = seed;
        let mut rand = || { rng ^= rng<<13; rng ^= rng>>7; rng ^= rng<<17; rng };

        let mut mem = FlatMem::new(0, 0x100000);
        let mut corpus: Option<std::io::BufWriter<std::fs::File>> =
            corpus_path.as_ref().map(|p| std::io::BufWriter::new(std::fs::File::create(p).unwrap()));
        let mut n_triples = 0u32;
        // v3 header: X64D magic → per-triple defined_flags_mask (which eflags bits
        // this insn's template WRITES; runner ANDs into the eflags-diff so SDM-
        // undefined flags don't false-diff). Runner detects X64C vs X64D.
        if let Some(f) = &mut corpus {
            f.write_all(&0x44343658u32.to_le_bytes()).unwrap();  // 'X64D'
            f.write_all(&0u32.to_le_bytes()).unwrap();  // n_triples (patched)
        }

        // Enumerate encodings by walking DEF_MNEMONICS + a table of {def_id → sample bytes}.
        // Simplest approach: decode_insn is the decoder — feed it BEDROCK's own bytes and
        // collect one unique encoding per def_id. Then randomize operands per triple.
        // ‡ v1: use the Bedrock corpus as the encoding-sampler (real compiler-emitted forms;
        //   avoids me hand-synthesizing every encoding class). v2: proper per-def_id synth.
        let bedrock = std::fs::read("/tmp/Minecraft.Windows.exe").ok();
        let mut sample_by_def: std::collections::BTreeMap<u32, Vec<u8>> = Default::default();
        if let Some(all) = &bedrock {
            let bytes = &all[0x400..0x400+0x100000];
            let mut i = 0;
            while i < bytes.len() && sample_by_def.len() < 506 {
                if let Some(d) = decode_insn(&bytes[i..], XMode::Bits64) {
                    sample_by_def.entry(d.def_id).or_insert(bytes[i..i+d.len as usize].to_vec());
                    i += d.len as usize;
                } else { i += 1; }
            }
        }
        eprintln!("[fuzz-x64: {} distinct encodings sampled from Bedrock 1MB]", sample_by_def.len());

        // v1 exclusions (same rationale as aarch64 native-diff): mem-touching insns need
        // a controlled address (random regs → random guest-addr → segfault); branches
        // change rip; system insns. Filter by mnemonic + def_id class.
        let excluded_mnem = |m: &str| {
            m.starts_with("MOV") && m != "MOV" ||  // MOVS/MOVZX/MOVSX ok; MOVSB/MOVDQU = mem/xmm — actually keep MOVZX/SX
            matches!(m, "PUSH"|"POP"|"CALL"|"RET"|"JMP"|"LEAVE"|"ENTER"
                |"PUSHF"|"POPF"|"IRET"|"INT"|"INT3"|"HLT"|"SYSCALL"|"SYSRET"|"SYSENTER"
                |"IN"|"OUT"|"INS"|"OUTS"|"LODS"|"STOS"|"MOVS"|"CMPS"|"SCAS"
                |"LGDT"|"LIDT"|"LTR"|"LMSW"|"WRMSR"|"RDMSR"|"CPUID"|"RDTSC"
                |"XSAVE"|"XRSTOR"|"FXSAVE"|"FXRSTOR"|"CLFLUSH"|"PREFETCH"
                |"CMPXCHG"|"XADD"|"XCHG")
            || m.starts_with('J')  // Jcc, JMP handled above but J* = branches
            || m.starts_with("LOOP")
            || m.starts_with("SET")  // ‡ SETcc writes Eb — could be reg-only, but skip v1
        };

        let (mut n_ok, mut n_ipanic, mut n_skip) = (0, 0, 0);
        let mut skip_by: std::collections::BTreeMap<&str, usize> = Default::default();
        for (&def_id, sample) in &sample_by_def {
            let mnem = DEF_MNEMONICS[def_id as usize];
            if excluded_mnem(mnem) { *skip_by.entry("mnem").or_default() += n; n_skip += n; continue; }
            for _ in 0..n {
                // ‡ v1: use the sample bytes verbatim (real encoding), randomize pre-state.
                //   v2: mutate ModRM.reg/rm/imm within the sample to widen coverage.
                let insn_bytes = sample.clone();
                // Skip if the decoded insn's ModRM is a MEMORY form (random-addr segfault).
                let d = decode_insn(&insn_bytes, XMode::Bits64).unwrap();
                // Skip if this encoding has a ModRM memory-form (random regs → random
                // guest-addr → segfault under Rosetta). Detect: m fields populated AND
                // not is_reg. (An insn with no ModRM leaves m all-default = passes.)
                let has_modrm = d.m.mod_ != 0 || d.m.reg != 0 || d.m.rm != 0 || d.m.is_reg;
                if has_modrm && !d.m.is_reg {
                    *skip_by.entry("mem-form").or_default() += 1; n_skip += 1; continue;
                }
                // Also skip if the sample uses rsp as an operand (rsp is the anchor).
                if (has_modrm && (d.m.reg == 4 || d.m.rm == 4)) || (d.op & 0xF8) == 0x50 && (d.op & 7) == 4 {
                    *skip_by.entry("rsp-operand").or_default() += 1; n_skip += 1; continue;
                }

                let mut pre = X86State::default();
                for r in 0..16 { if r != 4 { pre.gpr[r] = rand(); } }
                pre.gpr[4] = 0x80000;  // rsp = mid-mem (unused, but sane)
                // Fixed pre-eflags (bit-1 reserved-1 + IF only) — DON'T randomize AF/PF.
                // Logical ops (OR/AND/XOR) leave AF SDM-undefined; interp preserves it,
                // Rosetta clears it. Random pre-AF → false diff. (First corpus emitted
                // before this + before the rsp-exclusion below applied — v2 fixes both.)
                pre.eflags = 0x202;

                // Interp side.
                let ir = std::panic::catch_unwind(std::panic::AssertUnwindSafe(||
                    interp_one_x64(&pre, &mut mem, &insn_bytes, 0x1000)));
                let (i_post, _len, _br, handled) = match ir {
                    Ok(r) => r, Err(_) => { n_ipanic += 1; continue; }
                };
                if !handled { *skip_by.entry("no-lift").or_default() += 1; n_skip += 1; continue; }

                n_ok += 1;
                // Emit triple to corpus.
                if let Some(f) = &mut corpus {
                    let (stub, _slot) = emit_stub(&insn_bytes);
                    let flags_mask = DEF_FLAGS_MASK.get(def_id as usize).copied().unwrap_or(0);
                    f.write_all(&(def_id as u32).to_le_bytes()).unwrap();
                    f.write_all(&flags_mask.to_le_bytes()).unwrap();
                    f.write_all(&(stub.len() as u32).to_le_bytes()).unwrap();
                    f.write_all(&stub).unwrap();
                    let pre_flat = pre.to_flat();
                    for w in &pre_flat { f.write_all(&w.to_le_bytes()).unwrap(); }
                    let post_flat = i_post.to_flat();
                    for w in &post_flat { f.write_all(&w.to_le_bytes()).unwrap(); }
                    n_triples += 1;
                }
            }
        }
        // Patch n_triples in header.
        if let Some(mut f) = corpus {
            f.flush().unwrap();
            drop(f);
            if let Some(p) = &corpus_path {
                let mut all = std::fs::read(p).unwrap();
                all[4..8].copy_from_slice(&n_triples.to_le_bytes());
                std::fs::write(p, all).unwrap();
            }
        }

        eprintln!("[fuzz-x64: {} defs sampled × {} = {} triples attempted]",
            sample_by_def.len(), n, sample_by_def.len() * n);
        eprintln!("  interp-ok={}  interp-panic={}  skip={}  emitted={}",
            n_ok, n_ipanic, n_skip, n_triples);
        eprintln!("  skip breakdown: {:?}", skip_by);
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
