// Rung-4 gate-(b) harness: feed each aarch64.isa def's synthetic insn through
// `recompile_one` with a RecordingBuilder → dump the IL-seq. Proves the generated
// code RUNS (not just typechecks) + gives spot-checkable per-insn traces.
//
// Corpus mode (default): iterate the .isa's mask/match set, synthesize one insn per
// def (match-bits + zero-fields), record + count. This exercises every emit path once.
//
// Insn mode: `aarch64-recomp <hex-insn> [pc]` → record + dump that one insn's IL-seq.

use aarch64_recomp::recompile_one;
use sharpretro_jit::recording::RecordingBuilder;
use sharpretro_jit::interp::{InterpretingBuilder, FlatMem, GuestMem};

mod state;
use state::Aarch64State;

#[cfg(target_arch = "aarch64")]
mod native_oracle;

/// Execute one insn via InterpretingBuilder → return post-state.
fn interp_one<M: GuestMem>(pre: &Aarch64State, mem: &mut M, insn: u32, pc: u64) -> (Aarch64State, bool) {
    let mut s = pre.clone();
    s.pc = pc;
    let branched;
    {
        let mut b = InterpretingBuilder::new(&mut s, mem, pc);
        // Vector intrinsics (id≥100) → panic-stub for now (‡ rung-4b: implement or route
        // via call_intrinsic hook). SR/svc/breakpoint (id<10) → panic named.
        b.intrinsic = |_s, _m, id, _a| panic!("intrinsic id={id} not wired");
        let ok = recompile_one(&mut b, insn, pc);
        assert!(ok, "insn 0x{insn:08X} not decoded");
        branched = b.branched;
    }
    if !branched { s.pc = pc + 4; }
    (s, branched)
}

fn main() {
    let args: Vec<String> = std::env::args().collect();

    // --run <program> — the block-driver: load a small program into guest memory,
    // run via interp AND tier-0-block-cache, diff final state. THE step-④ oracle.
    // <program> = a name ("sum10") or a comma-sep hex-insn list.
    #[cfg(target_arch = "aarch64")]
    if args.get(1).map(|s| s.as_str()) == Some("--run") {
        // ── the test program(s) ────────────────────────────────────────────
        // sum10: x0 = Σ 1..10 = 55. Exercises MOVZ, ADD-reg, ADD-imm, SUBS(CMP),
        // B.cond, and the block-driver's branch-following.
        let sum10: &[u32] = &[
            0xD2800000,  // mov x0, #0        (sum)
            0xD2800021,  // mov x1, #1        (i)
            0xD2800162,  // mov x2, #11       (N+1, so loop runs 1..10 inclusive)
            // loop:
            0x8B010000,  // add x0, x0, x1
            0x91000421,  // add x1, x1, #1
            0xEB02003F,  // cmp x1, x2  (subs xzr, x1, x2)
            0x54FFFFAB,  // b.lt loop  (-3 insns = -12 bytes)
            0xD4200000,  // brk #0  (stop signal)
        ];
        // memsum: x0 = Σ mem[x1..x1+N*8] as u64s. Exercises LDR (load) + branch loop.
        // Setup: x1=array_base, x3=N. Data at guest_base+0x100.
        let memsum: &[u32] = &[
            0xD2800000,  // mov x0, #0
            0xD2802001,  // mov x1, #0x100  (array base = guest+0x100)
            0xD28000A3,  // mov x3, #5
            // loop:
            0xF8408424,  // ldr x4, [x1], #8   (post-index: load + x1+=8)
            0x8B040000,  // add x0, x0, x4
            0xD1000463,  // sub x3, x3, #1
            0xB5FFFFA3,  // cbnz x3, loop  (-3 insns)
            0xD4200000,  // brk #0
        ];
        // fib: x0 = fib(N) via iterative loop. x3=N.
        let fib: &[u32] = &[
            0xD2800000,  // mov x0, #0  (a)
            0xD2800021,  // mov x1, #1  (b)
            0xD2800183,  // mov x3, #12 (N)
            // loop:
            0x8B010002,  // add x2, x0, x1
            0xAA0103E0,  // mov x0, x1
            0xAA0203E1,  // mov x1, x2
            0xD1000463,  // sub x3, x3, #1
            0xF100047F,  // cmp x3, #1  (subs xzr, x3, #1)
            0x54FFFF6C,  // b.gt loop  (-5 insns)
            0xD4200000,  // brk #0
        ];
        let prog = match args.get(2).map(|s| s.as_str()) {
            Some("sum10") | None => sum10,
            Some("fib") => fib,
            Some("memsum") => memsum,
            Some(hex) => {
                // Parse comma-sep hex insns
                let v: Vec<u32> = hex.split(',').map(|s|
                    u32::from_str_radix(s.trim().trim_start_matches("0x"), 16).unwrap()).collect();
                Box::leak(v.into_boxed_slice())
            }
        };
        let entry: u64 = 0x10000;
        let max_insns = 1000;

        // ── interp driver ──────────────────────────────────────────────────
        // GuestMem: FlatMem at base=0 (guest addresses ARE offsets into the vec).
        // The tier-0 side sets mem_base = the same vec's host ptr, so both sides
        // read the same bytes at the same guest addrs.
        let mut guest_bytes = vec![0u8; 0x20000];
        // Load program at `entry`.
        for (i, &w) in prog.iter().enumerate() {
            guest_bytes[entry as usize + i*4 .. entry as usize + i*4 + 4]
                .copy_from_slice(&w.to_le_bytes());
        }
        // memsum test data: 5 u64s at guest+0x100 = {10,20,30,40,50} → sum=150.
        for (i, &v) in [10u64, 20, 30, 40, 50].iter().enumerate() {
            guest_bytes[0x100 + i*8 .. 0x100 + i*8 + 8].copy_from_slice(&v.to_le_bytes());
        }
        struct SharedMem<'a>(&'a mut [u8]);
        impl<'a> GuestMem for SharedMem<'a> {
            fn read(&self, addr: u64, w: u8) -> u128 {
                let n = ((w as usize)+7)/8; let off = addr as usize;
                let mut v = 0u128;
                for i in 0..n { v |= (self.0[off+i] as u128) << (i*8); }
                v
            }
            fn write(&mut self, addr: u64, w: u8, bits: u128) {
                let n = ((w as usize)+7)/8; let off = addr as usize;
                for i in 0..n { self.0[off+i] = (bits >> (i*8)) as u8; }
            }
        }
        let host_base = guest_bytes.as_mut_ptr() as u64;
        let mut mem_i = SharedMem(&mut guest_bytes);
        let mut si = Aarch64State::default();
        si.pc = entry;
        let mut n_i = 0;
        loop {
            let insn = mem_i.read(si.pc, 32) as u32;
            // BRK = stop.
            if (insn & 0xFFE00000) == 0xD4200000 { break; }
            let (post, branched) = interp_one(&si, &mut mem_i, insn, si.pc);
            let next = if branched { post.pc } else { si.pc + 4 };
            si = post; si.pc = next;
            n_i += 1;
            if n_i > max_insns { println!("interp: max_insns hit"); break; }
        }
        println!("[interp: {} insns, x0=0x{:X} x1=0x{:X} pc=0x{:X}]", n_i, si.x[0], si.x[1], si.pc);

        // ── tier-0 via BlockCache (crate-level; the DESIGN.md step-④ shape) ─
        use sharpretro_jit::tier0::{Tier0, STATE_WORDS};
        use sharpretro_jit::block_cache::{BlockCache, BlockCompiler, StopReason};
        use sharpretro_jit::{Builder, IlType};

        // Aarch64Compiler: BlockCompiler impl over the shared guest-bytes.
        // fetch = read u32 from host_base+pc; compile_block = recompile_one in a loop
        // until branched()/BRK/max. This is the arch-specific glue between BlockCache
        // (arch-neutral) and the generated recompiler.rs.
        struct Aarch64Compiler { host_base: u64, max_block: usize }
        impl BlockCompiler for Aarch64Compiler {
            fn fetch(&self, pc: u64) -> u32 {
                unsafe { ((self.host_base + pc) as *const u32).read_unaligned() }
            }
            fn is_stop(&self, insn: u32) -> bool {
                (insn & 0xFFE00000) == 0xD4200000  // BRK
            }
            fn compile_block(&self, t0: &mut Tier0, pc: u64, _mode: u32) -> (usize, StopReason) {
                let mut cur = pc;
                for n in 0..self.max_block {
                    let insn = self.fetch(cur);
                    if self.is_stop(insn) {
                        // Emit branch-to-cur so pc=cur; driver's next-iter stop-check catches it.
                        let t = t0.literal(IlType::U64, cur as u128);
                        t0.branch(t, false);
                        return (n, StopReason::StopInsn);
                    }
                    if !recompile_one(t0, insn, cur) {
                        panic!("block@0x{pc:X}+{n}: insn 0x{insn:08X} not decoded");
                    }
                    if t0.branched() { return (n + 1, StopReason::Branched); }
                    cur += 4;
                }
                (self.max_block, StopReason::MaxInsns)
            }
        }

        let compiler = Aarch64Compiler { host_base, max_block: 32 };
        let mut cache = BlockCache::new();
        let mut flat = [0u64; STATE_WORDS];
        flat[33] = entry;
        flat[66] = host_base;
        let result = cache.run(&compiler, &mut flat, 0, max_insns);
        println!("[tier0: {} block-execs, {} compiles, x0=0x{:X} x1=0x{:X} pc=0x{:X}, {:?}]",
            cache.n_execs, cache.n_compiles, flat[0], flat[1], flat[33], result);

        // ── diff ───────────────────────────────────────────────────────────
        let mut d = vec![];
        for r in 0..31 { if si.x[r] != flat[r] {
            d.push(format!("x{r}: interp=0x{:X} tier0=0x{:X}", si.x[r], flat[r])); } }
        if d.is_empty() {
            println!("✓ MATCH");
        } else {
            println!("✗ DIFF:");
            for l in &d { println!("    {l}"); }
        }
        return;
    }

    // --interp <hex-insn> [<hex-insn>...] — execute a sequence via InterpretingBuilder,
    // dump changed regs. Optional `x<N>=<hex>` args set initial state.
    if args.get(1).map(|s| s.as_str()) == Some("--interp") {
        let mut s = Aarch64State::default();
        let mut mem = FlatMem::new(0x10000, 0x10000);
        let mut insns = vec![];
        for a in &args[2..] {
            if let Some((r, v)) = a.split_once('=') {
                let val = u64::from_str_radix(v.trim_start_matches("0x"), 16).unwrap();
                if let Some(n) = r.strip_prefix('x') { s.x[n.parse::<usize>().unwrap()] = val; }
                else if r == "sp" { s.x[31] = val; }
                else if r == "nzcv" { s.nzcv = val as u32; }
            } else {
                insns.push(u32::from_str_radix(a.trim_start_matches("0x"), 16).unwrap());
            }
        }
        let pre = s.clone();
        let mut pc = 0x1000u64;
        for &insn in &insns {
            println!("→ 0x{pc:X}: 0x{insn:08X}");
            let (post, branched) = interp_one(&s, &mut mem, insn, pc);
            s = post;
            pc = if branched { s.pc } else { pc + 4 };
        }
        println!("─── final state (changed only) ───");
        for i in 0..32 { if s.x[i] != pre.x[i] {
            println!("  x{i:2} = 0x{:016X}  (was 0x{:X})", s.x[i], pre.x[i]); } }
        if s.nzcv != pre.nzcv { println!("  nzcv= 0x{:08X}  N={} Z={} C={} V={}",
            s.nzcv, s.n() as u8, s.z() as u8, s.c() as u8, s.vf() as u8); }
        println!("  pc  = 0x{:X}", s.pc);
        return;
    }

    // --tier0-fuzz [N] — the tier-0 GATE: same corpus/pre-state as --fuzz, but
    // tier-0-JIT'd-machine-code vs interp (instead of native-silicon vs interp).
    // Per DESIGN.md §Oracles: "tier-0 vs interpreter → state diff = 0".
    #[cfg(target_arch = "aarch64")]
    if args.get(1).map(|s| s.as_str()) == Some("--tier0-fuzz") {
        use sharpretro_jit::tier0::{Tier0, STATE_WORDS};
        let n: usize = args.get(2).and_then(|s| s.parse().ok()).unwrap_or(3);
        let seed: u64 = args.get(3).and_then(|s| s.parse().ok()).unwrap_or(0xC0FFEE);
        let mut mem = FlatMem::new(0x10000, 0x10000);
        let src = include_str!("lib.rs");
        let mut defs = vec![];
        let mut cur_name = "";
        for line in src.lines() {
            if let Some(n) = line.trim().strip_prefix("/* ").and_then(|s| s.strip_suffix(" */")) {
                cur_name = n;
            }
            if let Some(rest) = line.trim().strip_prefix("if (insn & 0x") {
                let mask_end = rest.find(')').unwrap();
                let mask = u32::from_str_radix(&rest[..mask_end], 16).unwrap();
                let ms = rest[mask_end..].find("0x").unwrap() + mask_end + 2;
                let me = rest[ms..].find(' ').unwrap() + ms;
                let mat = u32::from_str_radix(&rest[ms..me], 16).unwrap();
                defs.push((cur_name.to_string(), mask, mat));
            }
        }
        let mut rng = seed;
        let mut rand = || { rng ^= rng<<13; rng ^= rng>>7; rng ^= rng<<17; rng };
        let (mut n_ok, mut n_diff, mut n_ipanic, mut n_t0panic, mut n_skip) = (0usize, 0usize, 0usize, 0usize, 0usize);
        let mut diff_by_def: std::collections::BTreeMap<String, usize> = Default::default();
        let mut t0panic_by: std::collections::BTreeMap<String, usize> = Default::default();
        // Defs whose emit contains bd.mem_read/write — random reg-values give random
        // guest-addrs → interp FlatMem panics (caught), tier-0 segfaults (not). SKIP;
        // mem is covered by --run memsum (which controls addresses). Extract from
        // the generated lib.rs directly.
        let mem_defs: std::collections::HashSet<String> = {
            let mut cur = ""; let mut set = std::collections::HashSet::new();
            for line in src.lines() {
                if let Some(n) = line.trim().strip_prefix("/* ").and_then(|s| s.strip_suffix(" */")) {
                    cur = n;
                }
                if line.contains("bd.mem_read") || line.contains("bd.mem_write") {
                    set.insert(cur.to_string());
                }
            }
            set
        };
        for (name, mask, mat) in &defs {
            if mem_defs.contains(name) { n_skip += n; continue; }
            for _ in 0..n {
                let mut fields = (rand() as u32) & !mask;
                for sh in [0, 5, 10, 16] {
                    if (fields >> sh) & 0x1F == 31 { fields &= !(1u32 << sh); }
                }
                let insn = mat | fields;
                let mut pre = Aarch64State::default();
                for r in 1..=28 { pre.x[r] = rand(); }
                pre.nzcv = ((rand() as u32) & 0xF) << 28;
                // interp side
                let ir = std::panic::catch_unwind(std::panic::AssertUnwindSafe(||
                    interp_one(&pre, &mut mem, insn, 0x1000).0));
                let i_post = match ir { Ok(s) => s, Err(_) => { n_ipanic += 1; continue; } };
                // tier-0 side
                let t0r = std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
                    let mut t0 = Tier0::new();
                    if !recompile_one(&mut t0, insn, 0x1000) { panic!("not decoded"); }
                    let block = t0.finalize();
                    let mut flat = [0u64; STATE_WORDS];
                    for i in 0..32 { flat[i] = pre.x[i]; }
                    flat[32] = pre.nzcv as u64;
                    flat[33] = 0x1000;
                    block.exec(&mut flat);
                    flat
                }));
                let flat = match t0r {
                    Ok(f) => f,
                    Err(e) => {
                        n_t0panic += 1;
                        let msg = e.downcast_ref::<String>().map(|s| s.as_str())
                            .or_else(|| e.downcast_ref::<&str>().copied()).unwrap_or("?");
                        // Tally by panic-reason (which tier-0 op is unwired) — the coverage frontier.
                        let key = msg.split(':').next().unwrap_or(msg).to_string();
                        *t0panic_by.entry(key).or_default() += 1;
                        continue;
                    }
                };
                let mut d = false;
                for r in 0..31 { if i_post.x[r] != flat[r] { d = true; break; } }
                if (i_post.nzcv & 0xF0000000) != ((flat[32] as u32) & 0xF0000000) { d = true; }
                let interp_branched = i_post.pc != 0x1004;
                if interp_branched && i_post.pc != flat[33] { d = true; }
                if d {
                    n_diff += 1; *diff_by_def.entry(name.clone()).or_default() += 1;
                    if diff_by_def[name] == 1 {
                        eprintln!("DIFF {name} insn=0x{insn:08X}:");
                        for r in 0..31 { if i_post.x[r] != flat[r] {
                            eprintln!("    x{r}: interp=0x{:X} tier0=0x{:X} (pre=0x{:X})",
                                i_post.x[r], flat[r], pre.x[r]); } }
                        if (i_post.nzcv & 0xF0000000) != ((flat[32] as u32) & 0xF0000000) {
                            eprintln!("    nzcv: interp=0x{:08X} tier0=0x{:08X}",
                                i_post.nzcv, flat[32] as u32); }
                    }
                } else { n_ok += 1; }
            }
        }
        println!("[tier0-fuzz: {} defs × {} = {} triples]", defs.len(), n, defs.len()*n);
        println!("  ok={n_ok}  diff={n_diff}  interp-panic={n_ipanic}  tier0-panic={n_t0panic}  skip(mem)={n_skip}");
        if n_t0panic > 0 {
            println!("  ── tier-0 unwired ops (the coverage frontier) ──");
            for (msg, c) in &t0panic_by { println!("    {c:4}× {msg}"); }
        }
        if n_diff > 0 {
            println!("  ── diffs by def ──");
            for (name, c) in &diff_by_def { println!("    {c:4}× {name}"); }
        }
        return;
    }

    // --fuzz [N] — for each of the 344 defs' mask/match: synthesize N random-fielded
    // valid encodings + random pre-state, diff interp vs silicon. The exec-truth ladder
    // (my day-1's census-diff loop, applied to semantics instead of decode).
    #[cfg(target_arch = "aarch64")]
    if args.get(1).map(|s| s.as_str()) == Some("--fuzz") {
        let n: usize = args.get(2).and_then(|s| s.parse().ok()).unwrap_or(3);
        let seed: u64 = args.get(3).and_then(|s| s.parse().ok()).unwrap_or(0xC0FFEE);
        let stub = native_oracle::NativeStub::new();
        let mut mem = FlatMem::new(0x10000, 0x10000);
        // Walk mask/match set from lib.rs (same as corpus mode) + capture def NAME too
        let src = include_str!("lib.rs");
        let mut defs = vec![];
        let mut cur_name = "";
        for line in src.lines() {
            if let Some(n) = line.trim().strip_prefix("/* ").and_then(|s| s.strip_suffix(" */")) {
                cur_name = n;
            }
            if let Some(rest) = line.trim().strip_prefix("if (insn & 0x") {
                let mask_end = rest.find(')').unwrap();
                let mask = u32::from_str_radix(&rest[..mask_end], 16).unwrap();
                let ms = rest[mask_end..].find("0x").unwrap() + mask_end + 2;
                let me = rest[ms..].find(' ').unwrap() + ms;
                let mat = u32::from_str_radix(&rest[ms..me], 16).unwrap();
                defs.push((cur_name.to_string(), mask, mat));
            }
        }
        // Reproducible PRNG (xorshift64 — no dep). Seeded → same corpus every run.
        let mut rng = seed;
        let mut rand = || { rng ^= rng<<13; rng ^= rng>>7; rng ^= rng<<17; rng };
        let (mut n_ok, mut n_diff, mut n_skip, mut n_ipanic, mut n_reject) = (0usize, 0usize, 0usize, 0usize, 0usize);
        let mut diff_by_def: std::collections::BTreeMap<String, usize> = Default::default();
        let mut reject_by_def: std::collections::BTreeMap<String, usize> = Default::default();
        // v1 def-level exclusions (oracle-limitations, not semantics bugs):
        //   - vec-* / F* defs: stub doesn't load V-regs yet (‡ v2: LDR/STR Q0-Q31)
        //   - defs whose gpr-or-sp operand can be rn=31: native reads real host SP.
        //     Coarse filter: exclude any triple where ANY 5-bit field == 31.
        let vec_def = |n: &str| n.starts_with('F') || n.contains("vector") || n.contains("VEC")
            || n.contains("SIMD") || matches!(n, "DUP-general"|"UMOV"|"INS-general"|"INS-element"
                |"MOVI"|"MVNI"|"SCVTF-scalar-integer"|"UCVTF-scalar-integer");
        for (name, mask, mat) in &defs {
            if vec_def(name) { n_skip += n; continue; }  // ‡ v2: enable when stub loads V-regs
            for _ in 0..n {
                let mut fields = (rand() as u32) & !mask;
                // Force any 5-bit-aligned field ==31 → 30 (avoids SP-anchor collision).
                // Coarse; misses non-aligned reg-fields, but covers the common rd@0/rn@5/rm@16.
                for sh in [0, 5, 10, 16] {
                    if (fields >> sh) & 0x1F == 31 { fields &= !(1u32 << sh); }
                }
                let insn = mat | fields;
                // Random pre-state (x1-x28; leave x0/x29-x30/SP as 0 to reduce accidental
                // stub-frame corruption if a def slips the exclusion; NZCV random top-4).
                let mut pre = Aarch64State::default();
                for r in 1..=28 { pre.x[r] = rand(); }
                pre.nzcv = ((rand() as u32) & 0xF) << 28;
                // Interp side (may panic on unwired intrinsic / unreachable-match / todo-wmask).
                let ir = std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
                    interp_one(&pre, &mut mem, insn, 0x1000).0
                }));
                let i_post = match ir { Ok(s) => s, Err(_) => { n_ipanic += 1; continue; } };
                let mut n_post = pre.clone();
                // Name the insn BEFORE feeding it to silicon — if the stub segfaults,
                // the last stderr line names the killer (v1 debug; v2 = signal handler).
                match stub.exec_one(&mut n_post, insn) {
                    native_oracle::NativeResult::Excluded => { n_skip += 1; continue; }
                    native_oracle::NativeResult::SiliconRejects(sig) => {
                        // .isa accepted (interp didn't panic) but silicon trapped = a
                        // missing `requires` in the .isa. Tally by def.
                        n_reject += 1;
                        *reject_by_def.entry(format!("{name} (sig={sig})")).or_default() += 1;
                        continue;
                    }
                    native_oracle::NativeResult::Ran => {}
                }
                let mut d = false;
                for r in 0..31 { if i_post.x[r] != n_post.x[r] { d = true; break; } }
                if (i_post.nzcv & 0xF0000000) != (n_post.nzcv & 0xF0000000) { d = true; }
                if d {
                    n_diff += 1; *diff_by_def.entry(name.clone()).or_default() += 1;
                    // First diff for this def → dump the reproducer.
                    if diff_by_def[name] == 1 {
                        eprintln!("DIFF {name} insn=0x{insn:08X}:");
                        for r in 0..31 { if i_post.x[r] != n_post.x[r] {
                            eprintln!("    x{r}: interp=0x{:X} native=0x{:X} (pre=0x{:X})",
                                i_post.x[r], n_post.x[r], pre.x[r]); } }
                        if (i_post.nzcv & 0xF0000000) != (n_post.nzcv & 0xF0000000) {
                            eprintln!("    nzcv: interp=0x{:08X} native=0x{:08X} (pre=0x{:08X})",
                                i_post.nzcv, n_post.nzcv, pre.nzcv); }
                        // dump pre-state args for repro
                        let regs: Vec<_> = (1..=28).map(|r| format!("x{r}=0x{:X}", pre.x[r])).collect();
                        eprintln!("    repro: --native-diff {} nzcv=0x{:X} 0x{insn:08X}",
                            regs.join(" "), pre.nzcv);
                    }
                }
                else { n_ok += 1; }
            }
        }
        println!("[fuzz: {} defs × {} = {} triples]", defs.len(), n, defs.len()*n);
        println!("  ok={n_ok}  diff={n_diff}  silicon-rejects={n_reject}  skip(v1-excl)={n_skip}  interp-panic={n_ipanic}");
        if n_reject > 0 {
            println!("  ── silicon-rejects (.isa over-permissive) ──");
            for (name, c) in &reject_by_def { println!("    {c:4}× {name}"); }
        }
        if n_diff > 0 {
            println!("  ── diffs by def ──");
            for (name, c) in &diff_by_def { println!("    {c:4}× {name}"); }
        }
        return;
    }

    // --tier0-diff <hex-insn> [x<N>=<hex>...] — compile one insn via Tier0, execute the
    // JIT'd machine-code, diff post-state vs InterpretingBuilder. THE tier-0 oracle
    // (per DESIGN.md: "tier-0 vs interpreter → state diff = 0").
    #[cfg(target_arch = "aarch64")]
    if args.get(1).map(|s| s.as_str()) == Some("--tier0-diff") {
        use sharpretro_jit::tier0::{Tier0, STATE_WORDS};
        let mut pre = Aarch64State::default();
        let mut insns = vec![];
        for a in &args[2..] {
            if let Some((r, v)) = a.split_once('=') {
                let val = u64::from_str_radix(v.trim_start_matches("0x"), 16).unwrap();
                if let Some(n) = r.strip_prefix('x') { pre.x[n.parse::<usize>().unwrap()] = val; }
                else if r == "nzcv" { pre.nzcv = val as u32; }
            } else {
                insns.push(u32::from_str_radix(a.trim_start_matches("0x"), 16).unwrap());
            }
        }
        let mut mem = FlatMem::new(0x10000, 0x10000);
        let (mut ok, mut diffs) = (0, 0);
        for &insn in &insns {
            // interp side
            let (i_post, _) = interp_one(&pre, &mut mem, insn, 0x1000);
            // tier-0 side: compile the ONE insn, exec against a flat state array
            let mut t0 = Tier0::new();
            let decoded = std::panic::catch_unwind(std::panic::AssertUnwindSafe(||
                recompile_one(&mut t0, insn, 0x1000)));
            match decoded {
                Ok(true) => {},
                Ok(false) => { println!("0x{insn:08X}  not decoded"); continue; }
                Err(_) => { println!("0x{insn:08X}  tier-0 PANIC (unwired op)"); continue; }
            }
            let block = t0.finalize();
            let mut flat = [0u64; STATE_WORDS];
            for i in 0..32 { flat[i] = pre.x[i]; }
            flat[32] = pre.nzcv as u64;
            flat[33] = 0x1000;  // pc
            block.exec(&mut flat);
            // diff GPR + nzcv + pc
            let mut d = vec![];
            for r in 0..31 { if i_post.x[r] != flat[r] {
                d.push(format!("x{r}: interp=0x{:X} tier0=0x{:X}", i_post.x[r], flat[r])); } }
            if (i_post.nzcv & 0xF0000000) != ((flat[32] as u32) & 0xF0000000) {
                d.push(format!("nzcv: interp=0x{:08X} tier0=0x{:08X}", i_post.nzcv, flat[32] as u32));
            }
            // tier-0 doesn't advance pc for non-branching insns (that's the block-driver's
            // job — recompile_one compiles ONE insn; the driver bumps pc if !branched).
            // Only diff pc when the interp branched (= .isa emitted a `branch` head).
            let interp_branched = i_post.pc != 0x1004;  // interp_one sets pc=pc+4 if !branched
            if interp_branched && i_post.pc != flat[33] {
                d.push(format!("pc: interp=0x{:X} tier0=0x{:X}", i_post.pc, flat[33]));
            }
            if d.is_empty() {
                println!("0x{insn:08X}  ✓ (tier0 == interp)  [{} host-insns, {} slots]",
                    block.code_len / 4, block.n_slots);
                if std::env::var("TIER0_DUMP").is_ok() {
                    std::fs::write("/tmp/tier0_block.bin", block.code_bytes()).unwrap();
                    eprintln!("(dumped {} bytes → /tmp/tier0_block.bin)", block.code_len);
                }
                ok += 1;
            } else {
                println!("0x{insn:08X}  ✗ DIFF:");
                for l in &d { println!("    {l}"); }
                diffs += 1;
            }
        }
        println!("[tier0-diff: {ok} match, {diffs} diff]");
        return;
    }

    #[cfg(target_arch = "aarch64")]

    // --native-diff <hex-insn> [x<N>=<hex>...] — run one insn on BOTH the
    // InterpretingBuilder AND real silicon (NativeStub), diff the post-states.
    // The exec-truth oracle: silicon = the independent verifier (interp+recompiler
    // are co-blind to .isa/emit bugs; silicon isn't).
    #[cfg(target_arch = "aarch64")]
    if args.get(1).map(|s| s.as_str()) == Some("--native-diff") {
        let mut pre = Aarch64State::default();
        let mut insns = vec![];
        for a in &args[2..] {
            if let Some((r, v)) = a.split_once('=') {
                let val = u64::from_str_radix(v.trim_start_matches("0x"), 16).unwrap();
                if let Some(n) = r.strip_prefix('x') { pre.x[n.parse::<usize>().unwrap()] = val; }
                else if r == "nzcv" { pre.nzcv = val as u32; }
            } else {
                insns.push(u32::from_str_radix(a.trim_start_matches("0x"), 16).unwrap());
            }
        }
        let stub = native_oracle::NativeStub::new();
        let mut mem = FlatMem::new(0x10000, 0x10000);
        let mut ok = 0; let mut skip = 0; let mut diffs = 0;
        for &insn in &insns {
            let (i_post, _) = interp_one(&pre, &mut mem, insn, 0x1000);
            let mut n_post = pre.clone();
            match stub.exec_one(&mut n_post, insn) {
                native_oracle::NativeResult::Excluded => {
                    println!("0x{insn:08X}  SKIP (branch/load-store/system/pc-dep — v1 exclusion)");
                    skip += 1; continue;
                }
                native_oracle::NativeResult::SiliconRejects(sig) => {
                    println!("0x{insn:08X}  SILICON-REJECTS (sig={sig}) — .isa accepted, silicon trapped");
                    skip += 1; continue;
                }
                native_oracle::NativeResult::Ran => {}
            }
            // diff x[0..31] + nzcv (SP/pc excluded — stub doesn't model them)
            let mut d = vec![];
            for r in 0..31 { if i_post.x[r] != n_post.x[r] {
                d.push(format!("x{r}: interp=0x{:X} native=0x{:X}", i_post.x[r], n_post.x[r])); } }
            if (i_post.nzcv & 0xF0000000) != (n_post.nzcv & 0xF0000000) {
                d.push(format!("nzcv: interp=0x{:08X} native=0x{:08X}", i_post.nzcv, n_post.nzcv));
            }
            if d.is_empty() {
                println!("0x{insn:08X}  ✓ (match)");
                ok += 1;
            } else {
                println!("0x{insn:08X}  ✗ DIFF:");
                for l in &d { println!("    {l}"); }
                diffs += 1;
            }
        }
        println!("[native-diff: {ok} match, {diffs} diff, {skip} skip]");
        return;
    }

    if args.len() >= 2 {
        let insn = u32::from_str_radix(args[1].trim_start_matches("0x"), 16).expect("hex insn");
        let pc = args.get(2).map(|s| u64::from_str_radix(s.trim_start_matches("0x"), 16).unwrap()).unwrap_or(0x1000);
        let mut b = RecordingBuilder::new();
        let ok = recompile_one(&mut b, insn, pc);
        println!("insn=0x{:08X} pc=0x{:X} decoded={}", insn, pc, ok);
        println!("{}", b.dump());
        return;
    }

    // Corpus mode: walk the source of lib.rs itself for `if (insn & MASK) == MATCH` lines
    // (each = one def). Cheap, no separate table needed — the mask/match set is right there.
    let src = include_str!("lib.rs");
    let mut n_defs = 0; let mut n_ok = 0; let mut n_lines = 0usize;
    for line in src.lines() {
        if let Some(rest) = line.trim().strip_prefix("if (insn & 0x") {
            // parse `MASK) == 0xMATCH { 'decode: {`
            let mask_end = rest.find(')').unwrap();
            let _mask = u32::from_str_radix(&rest[..mask_end], 16).unwrap();
            let match_start = rest[mask_end..].find("0x").unwrap() + mask_end + 2;
            let match_end = rest[match_start..].find(' ').unwrap() + match_start;
            let mat = u32::from_str_radix(&rest[match_start..match_end], 16).unwrap();
            n_defs += 1;
            let mut b = RecordingBuilder::new();
            // synthesize: the match bits alone (fields all-zero). Some defs will `break 'decode`
            // on a `requires` (all-zero fields fail their constraint) — that's still a RUN.
            let ok = std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
                recompile_one(&mut b, mat, 0x1000)
            }));
            match ok {
                Ok(true) => { n_ok += 1; n_lines += b.log.len(); }
                Ok(false) => { /* mask/match dispatch fell through — shouldn't happen for match-bits */ }
                Err(_) => { eprintln!("PANIC on def#{} insn=0x{:08X}", n_defs, mat); }
            }
        }
    }
    println!("[gate-(b) corpus: {} defs, {} decoded-ok, {} IL-lines total, avg {:.1}/def]",
        n_defs, n_ok, n_lines, n_lines as f64 / n_ok.max(1) as f64);
}
