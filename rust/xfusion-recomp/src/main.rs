// x64-guest harness. Phase-1: decode-only spot-checks + XED corpus-diff.
// Phase-2/3: --interp = execute via InterpretingBuilder × X86State × hand_lift.
use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::{decode_insn, DEF_MNEMONICS};
use xfusion_recomp::state::X86State;
// Generated lift.rs (278 templates × 506 defs). hand_lift stays as the reference
// (spot-checkable per-mnemonic) but the primary path is the generated one.
use xfusion_recomp::lift::{lift_one, FLAGS_ALL_LIVE};
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
        handled = lift_one(&mut b, &d, pc, XMode::Bits64, FLAGS_ALL_LIVE);
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
        println!("[interp: {} insns, rax=0x{:X} rcx=0x{:X} rip=0x{:X}]",
            n, s.gpr[0], s.gpr[1], s.rip);

        // ── tier-0 via BlockCache (x64-guest, aarch64-host) ────────────────
        #[cfg(target_arch = "aarch64")]
        {
            use sharpretro_jit::tier0::Tier0;
            use sharpretro_jit::block_cache::{BlockCache, BlockCompiler, StopReason};
            use sharpretro_jit::{Builder, IlType};
            use xfusion_recomp::state::{X64_LAYOUT, STATE_WORDS_X64, OFF_RIP, OFF_MEMBASE};

            // Fresh guest bytes for tier-0 (interp side may have mutated mem via CALL push).
            let mut guest_bytes = vec![0u8; 0x100000];
            for (i, &b) in prog.iter().enumerate() { guest_bytes[entry as usize + i] = b; }
            let host_base = guest_bytes.as_mut_ptr() as u64;

            struct X64Compiler { host_base: u64, max_block: usize }
            impl BlockCompiler for X64Compiler {
                fn fetch(&self, pc: u64) -> u32 {
                    // Return first 4 bytes packed — is_stop only needs byte-0.
                    unsafe { ((self.host_base + pc) as *const u32).read_unaligned() }
                }
                fn is_stop(&self, first_word: u32) -> bool {
                    (first_word & 0xFF) == 0xCC  // INT3
                }
                fn compile_block(&self, t0: &mut Tier0, pc: u64, _mode: u32) -> (usize, StopReason) {
                    use xfusion_recomp::lift::{DEF_FLAGS_MASK, DEF_FLAGS_READ};
                    // Pass 1: decode the block, collect (DecodedInsn, pc) until stop/branch.
                    // (Branch detection: any def whose template calls bd.branch — for x64
                    //  that's Jcc/JMP/CALL/RET/branch-if. Detect via mnemonic class since
                    //  we can't lift-into-tier-0 twice. ‡ v2: DEF_IS_BRANCH[def_id] table.)
                    let is_branch = |m: &str| m.starts_with('J') || m == "CALL" || m == "RET"
                        || m == "RETI" || m == "RETF" || m.starts_with("LOOP");
                    let mut insns: Vec<(xfusion_recomp::decode::DecodedInsn, u64)> = vec![];
                    let mut cur = pc;
                    let mut stop_reason = StopReason::MaxInsns;
                    for _ in 0..self.max_block {
                        let bytes = unsafe {
                            std::slice::from_raw_parts((self.host_base + cur) as *const u8, 15)
                        };
                        if bytes[0] == 0xCC { stop_reason = StopReason::StopInsn; break; }
                        let d = decode_insn(bytes, XMode::Bits64)
                            .unwrap_or_else(|| panic!("undecoded @0x{cur:x}: {:02X?}", &bytes[..4]));
                        let mnem = DEF_MNEMONICS[d.def_id as usize];
                        cur += d.len as u64;
                        let br = is_branch(mnem);
                        insns.push((d, cur - d.len as u64));  // (d, pc-of-this-insn)
                        if br { stop_reason = StopReason::Branched; break; }
                    }
                    // Pass 2: BACKWARD liveness. Block-exit = ALL live (conservative — the
                    // successor block may read anything). For each insn i (last→first):
                    //   live_out[i] = live_in[i+1] (or ALL at block-exit)
                    //   live_in[i]  = (live_out[i] & !WRITTEN[i]) | READ[i]
                    // A flag-write in insn i is DEAD if its bit ∉ live_out[i].
                    let mut live_flags_per: Vec<u32> = vec![0; insns.len()];
                    let mut live: u32 = FLAGS_ALL_LIVE;  // block-exit
                    for i in (0..insns.len()).rev() {
                        let did = insns[i].0.def_id as usize;
                        live_flags_per[i] = live;  // = live_out[i]
                        let w = DEF_FLAGS_MASK.get(did).copied().unwrap_or(0);
                        let r = DEF_FLAGS_READ.get(did).copied().unwrap_or(0);
                        live = (live & !w) | r;
                    }
                    // Pass 3: forward emit with per-insn live_flags.
                    for (i, (d, ipc)) in insns.iter().enumerate() {
                        if !lift_one(t0, d, *ipc, XMode::Bits64, live_flags_per[i]) {
                            panic!("no lift @0x{ipc:x}: {} def_id={}",
                                DEF_MNEMONICS[d.def_id as usize], d.def_id);
                        }
                    }
                    // If the block didn't end on a branch (StopInsn / MaxInsns), emit the
                    // fallthrough-branch to `cur`.
                    if !t0.branched() {
                        let t = t0.literal(IlType::U64, cur as u128);
                        t0.branch(t, false);
                    }
                    (insns.len(), stop_reason)
                }
            }

            let compiler = X64Compiler { host_base, max_block: 32 };
            let mut cache = BlockCache::with_layout(&X64_LAYOUT);
            let mut flat = [0u64; STATE_WORDS_X64];
            flat[OFF_RIP] = entry;
            flat[4] = 0x80000;               // rsp
            flat[OFF_MEMBASE] = host_base;
            let result = cache.run(&compiler, &mut flat[..], 0, max_insns);
            println!("[tier0: {} block-execs, {} compiles, rax=0x{:X} rcx=0x{:X} rip=0x{:X}, {:?}]",
                cache.n_execs, cache.n_compiles, flat[0], flat[1], flat[OFF_RIP], result);

            // Diff.
            let mut d = vec![];
            for r in 0..16 { if s.gpr[r] != flat[r] {
                d.push(format!("r{r}: interp=0x{:X} tier0=0x{:X}", s.gpr[r], flat[r])); } }
            if (s.eflags & 0x8D5) != ((flat[16] as u32) & 0x8D5) {
                d.push(format!("eflags: interp=0x{:X} tier0=0x{:X}", s.eflags, flat[16]));
            }
            if d.is_empty() { println!("✓ MATCH"); }
            else { println!("✗ DIFF:"); for l in &d { println!("    {l}"); } }

            // ── tier-1 (register-allocated) via XF_TIER1=1 env ──────────────
            // Same compile_block logic but drives Tier1 (which records via
            // IlRecorder, then linear_scan+emit at .compile()). No BlockCache
            // yet (v1 test drives blocks by hand); when this MATCHes on the
            // scalar-int programs (sum10/fib), tier-1 emit is proven.
            if std::env::var("XF_TIER1").is_ok() {
                use sharpretro_jit::tier1::Tier1;
                use sharpretro_jit::tier0::CompiledBlock;
                use xfusion_recomp::lift::{DEF_FLAGS_MASK, DEF_FLAGS_READ};

                let mut guest_bytes = vec![0u8; 0x100000];
                for (i, &b) in prog.iter().enumerate() { guest_bytes[entry as usize + i] = b; }
                let host_base = guest_bytes.as_mut_ptr() as u64;

                let is_branch = |m: &str| m.starts_with('J') || m == "CALL" || m == "RET"
                    || m == "RETI" || m == "RETF" || m.starts_with("LOOP");

                // Compile-and-cache blocks by hand (no BlockCache — Tier0-specific for now).
                let mut blocks: std::collections::HashMap<u64, CompiledBlock> =
                    std::collections::HashMap::new();
                let mut flat1 = [0u64; STATE_WORDS_X64];
                flat1[OFF_RIP] = entry;
                flat1[4] = 0x80000;
                flat1[OFF_MEMBASE] = host_base;

                let mut n_execs = 0usize; let mut n_compiles = 0usize;
                loop {
                    let pc = flat1[OFF_RIP];
                    let b0 = unsafe { *((host_base + pc) as *const u8) };
                    if b0 == 0xCC { break; }
                    let cb = blocks.entry(pc).or_insert_with(|| {
                        n_compiles += 1;
                        let mut t1 = Tier1::with_layout(&X64_LAYOUT);
                        // 3-pass: decode-collect / backward-liveness / forward-emit.
                        let mut insns = vec![]; let mut cur = pc;
                        for _ in 0..32 {
                            let bytes = unsafe {
                                std::slice::from_raw_parts((host_base + cur) as *const u8, 15) };
                            if bytes[0] == 0xCC { break; }
                            let d = decode_insn(bytes, XMode::Bits64).unwrap();
                            let mnem = DEF_MNEMONICS[d.def_id as usize];
                            cur += d.len as u64;
                            let br = is_branch(mnem);
                            insns.push((d, cur - d.len as u64));
                            if br { break; }
                        }
                        let mut per = vec![0u32; insns.len()];
                        let mut live = FLAGS_ALL_LIVE;
                        for i in (0..insns.len()).rev() {
                            let did = insns[i].0.def_id as usize;
                            per[i] = live;
                            live = (live & !DEF_FLAGS_MASK.get(did).copied().unwrap_or(0))
                                 | DEF_FLAGS_READ.get(did).copied().unwrap_or(0);
                        }
                        for (i, (d, ipc)) in insns.iter().enumerate() {
                            lift_one(&mut t1, d, *ipc, XMode::Bits64, per[i]);
                        }
                        if !t1.rec.branched() {
                            let tv = t1.literal(IlType::U64, cur as u128);
                            t1.branch(tv, false);
                        }
                        t1.compile()
                    });
                    cb.exec_slice(&mut flat1[..]);
                    n_execs += 1;
                    if n_execs > max_insns { println!("  tier-1: max_execs hit"); break; }
                }
                println!("[tier1: {} block-execs, {} compiles, rax=0x{:X} rcx=0x{:X} rip=0x{:X}]",
                    n_execs, n_compiles, flat1[0], flat1[1], flat1[OFF_RIP]);
                let mut d1 = vec![];
                for r in 0..16 { if s.gpr[r] != flat1[r] {
                    d1.push(format!("r{r}: interp=0x{:X} tier1=0x{:X}", s.gpr[r], flat1[r])); } }
                if (s.eflags & 0x8D5) != ((flat1[16] as u32) & 0x8D5) {
                    d1.push(format!("eflags: interp=0x{:X} tier1=0x{:X}", s.eflags, flat1[16]));
                }
                if d1.is_empty() { println!("  ✓ TIER-1 MATCH"); }
                else { println!("  ✗ TIER-1 DIFF:"); for l in &d1 { println!("    {l}"); } }
            }
        }
        return;
    }

    // --track <hex-bytes> — dump the insn's read/write-set via TrackingState.
    // (The libmoonage lazy-precondition-discovery foundation: first pass discovers
    //  which regs an insn READS, then v4 corpus enumerates boundary-values over
    //  exactly those regs.)
    if args.get(1).map(|s| s.as_str()) == Some("--track") {
        use xfusion_recomp::state::TrackingState;
        let bytes: Vec<u8> = args[2].split(',')
            .map(|s| u8::from_str_radix(s.trim().trim_start_matches("0x"), 16).unwrap())
            .collect();
        let d = decode_insn(&bytes, XMode::Bits64).unwrap();
        let mut ts = TrackingState::default();
        let mut mem = FlatMem::new(0, 0x1000);
        {
            let mut b = InterpretingBuilder::new(&mut ts, &mut mem, 0x1000);
            b.intrinsic = |_,_,id,_| panic!("intrinsic {id}");
            lift_one(&mut b, &d, 0x1000, XMode::Bits64, FLAGS_ALL_LIVE);
        }
        println!("{} (def_id={}):", DEF_MNEMONICS[d.def_id as usize], d.def_id);
        println!("  reads:  {:?}", ts.reads.borrow());
        println!("  writes: {:?}", ts.writes);
        println!("  gpr_reads: {:?}  flag_reads: {:?}  reads_xmm: {}",
            ts.gpr_reads(), ts.flag_reads(), ts.reads_xmm());
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

        // v4 boundary-value corpus (--boundary): TrackingState discovers each insn's
        // read-set, then sweep boundary values over EXACTLY those inputs (per libmoonage
        // TestGen.cs:121-193 lazy-precondition-discovery). Denser bug-surface than random:
        // {0, 1, 1<<(i*16), ~that, sign-bits} = the edges where flag/carry/sext bugs live.
        let use_boundary = args.iter().any(|a| a == "--boundary");
        // Boundary values per libmoonage (X-reg set adapted for x64 GPRs):
        const BOUNDARY_VALS: &[u64] = &[
            0, 1, 0xFF, 0xFFFF, 0xFFFF_FFFF, u64::MAX,           // width edges
            1u64<<7, 1u64<<15, 1u64<<31, 1u64<<63,               // sign bits per width
            0x8000_0000_0000_0000, 0x7FFF_FFFF_FFFF_FFFF,        // i64 min/max
            0x0F, 0x10,                                          // AF nibble-carry edge
            0xDEAD_BEEF_CAFE_BABE,                               // one dense random
        ];

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
                |"CMPXCHG"|"XADD"|"XCHG"|"RETI"|"RETF"|"RETFI"
                |"BOUND"|"ARPL"|"LDS"|"LES"|"LFS"|"LGS"|"LSS")
            || m.starts_with('J')  // Jcc, JMP handled above but J* = branches
            || m.starts_with("LOOP")
            || m.starts_with("SET")  // ‡ SETcc writes Eb — could be reg-only, but skip v1
        };

        let (mut n_ok, mut n_ipanic, mut n_skip) = (0, 0, 0);
        let mut skip_by: std::collections::BTreeMap<&str, usize> = Default::default();
        // v4 boundary-mode helper: discover an insn's GPR read-set via TrackingState.
        // Returns None if the tracking pass panics/hits an unwired intrinsic (= skip).
        let discover_reads = |insn_bytes: &[u8]| -> Option<(Vec<u32>, Vec<u32>, bool)> {
            use xfusion_recomp::state::TrackingState;
            let d = decode_insn(insn_bytes, XMode::Bits64)?;
            let mut ts = TrackingState::default();
            ts.inner.gpr[4] = 0x80000;
            let mut mem = FlatMem::new(0, 0x1000);
            let r = std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
                let mut b = InterpretingBuilder::new(&mut ts, &mut mem, 0x1000);
                b.intrinsic = |_,_,id,_| panic!("intrinsic {id}");
                lift_one(&mut b, &d, 0x1000, XMode::Bits64, FLAGS_ALL_LIVE)
            }));
            if r.is_err() { return None; }
            Some((ts.gpr_reads(), ts.flag_reads(), ts.reads_xmm()))
        };

        for (&def_id, sample) in &sample_by_def {
            let mnem = DEF_MNEMONICS[def_id as usize];
            if excluded_mnem(mnem) { *skip_by.entry("mnem").or_default() += n; n_skip += n; continue; }

            // ── v4 boundary-mode: discover read-set once, sweep boundary values ──
            if use_boundary {
                let insn_bytes = sample.clone();
                let d = decode_insn(&insn_bytes, XMode::Bits64).unwrap();
                let has_modrm = d.m.mod_ != 0 || d.m.reg != 0 || d.m.rm != 0 || d.m.is_reg;
                if has_modrm && !d.m.is_reg { *skip_by.entry("mem-form").or_default() += 1; n_skip += 1; continue; }
                if has_modrm && (d.m.reg == 4 || d.m.rm == 4) { *skip_by.entry("rsp").or_default() += 1; n_skip += 1; continue; }
                let Some((gpr_reads, flag_reads, reads_xmm)) = discover_reads(&insn_bytes)
                    else { *skip_by.entry("track-panic").or_default() += 1; n_skip += 1; continue; };
                if reads_xmm { *skip_by.entry("xmm").or_default() += 1; n_skip += 1; continue; }
                if gpr_reads.iter().any(|&r| r == 4) { *skip_by.entry("rsp-read").or_default() += 1; n_skip += 1; continue; }

                // Anchor state: all read-regs = a mid-value; then sweep ONE reg at a time
                // through BOUNDARY_VALS. Per libmoonage's shape (one dimension varied,
                // rest fixed) — not full cartesian (that's O(vals^n_regs)).
                let anchor: u64 = 0x1122_3344_5566_7788;
                // Also sweep flag-reads if any (ADC/SBB/CMOVcc) — {0, all-1s} for eflags.
                let flag_states: &[u32] = if flag_reads.is_empty() { &[0x202] }
                                          else { &[0x202, 0x202 | 0x8D5] };

                for &fs in flag_states {
                    for (sweep_i, &sweep_reg) in gpr_reads.iter().enumerate() {
                        for &bv in BOUNDARY_VALS {
                            let mut pre = X86State::default();
                            for &r in &gpr_reads { pre.gpr[r as usize] = anchor; }
                            pre.gpr[sweep_reg as usize] = bv;
                            pre.gpr[4] = 0x80000;
                            pre.eflags = fs;
                            // If sweeping the FIRST read-reg: also sweep the anchor for
                            // the OTHERS across a couple of values (0, MAX) so 2-arg edges
                            // like (0,0)/(MAX,MAX)/(MAX,0) get hit.
                            // v4.0: skip that; v4.1 adds the pairwise pass.

                            let ir = std::panic::catch_unwind(std::panic::AssertUnwindSafe(||
                                interp_one_x64(&pre, &mut mem, &insn_bytes, 0x1000)));
                            let (i_post, _len, _br, handled) = match ir {
                                Ok(r) => r, Err(_) => { n_ipanic += 1; continue; }
                            };
                            if !handled { *skip_by.entry("no-lift").or_default() += 1; n_skip += 1; continue; }
                            n_ok += 1;
                            if let Some(f) = &mut corpus {
                                let (stub, _slot) = emit_stub(&insn_bytes);
                                let flags_mask = DEF_FLAGS_MASK.get(def_id as usize).copied().unwrap_or(0);
                                f.write_all(&(def_id as u32).to_le_bytes()).unwrap();
                                f.write_all(&flags_mask.to_le_bytes()).unwrap();
                                f.write_all(&(stub.len() as u32).to_le_bytes()).unwrap();
                                f.write_all(&stub).unwrap();
                                for w in &pre.to_flat() { f.write_all(&w.to_le_bytes()).unwrap(); }
                                for w in &i_post.to_flat() { f.write_all(&w.to_le_bytes()).unwrap(); }
                                n_triples += 1;
                            }
                        }
                        // For 1-reg-read insns (INC/DEC/NOT etc), sweep_i loop is 1 iter.
                        let _ = sweep_i;
                    }
                }
                continue;
            }

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

