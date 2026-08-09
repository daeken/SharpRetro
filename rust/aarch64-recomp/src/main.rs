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
use sharpretro_jit::interp::{InterpretingBuilder, FlatMem};

mod state;
use state::Aarch64State;

#[cfg(target_arch = "aarch64")]
mod native_oracle;

/// Execute one insn via InterpretingBuilder → return post-state.
fn interp_one(pre: &Aarch64State, mem: &mut FlatMem, insn: u32, pc: u64) -> (Aarch64State, bool) {
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
        let (mut n_ok, mut n_diff, mut n_skip, mut n_ipanic) = (0usize, 0usize, 0usize, 0usize);
        let mut diff_by_def: std::collections::BTreeMap<String, usize> = Default::default();
        for (name, mask, mat) in &defs {
            for _ in 0..n {
                // Random field-bits in the un-masked positions (= a valid encoding for THIS def).
                let insn = mat | ((rand() as u32) & !mask);
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
                if !stub.exec_one(&mut n_post, insn) { n_skip += 1; continue; }
                let mut d = false;
                for r in 0..31 { if i_post.x[r] != n_post.x[r] { d = true; break; } }
                if (i_post.nzcv & 0xF0000000) != (n_post.nzcv & 0xF0000000) { d = true; }
                if d { n_diff += 1; *diff_by_def.entry(name.clone()).or_default() += 1; }
                else { n_ok += 1; }
            }
        }
        println!("[fuzz: {} defs × {} = {} triples]", defs.len(), n, defs.len()*n);
        println!("  ok={n_ok}  diff={n_diff}  skip(v1-excl)={n_skip}  interp-panic={n_ipanic}");
        if n_diff > 0 {
            println!("  ── diffs by def ──");
            for (name, c) in &diff_by_def { println!("    {c:4}× {name}"); }
        }
        return;
    }

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
            if !stub.exec_one(&mut n_post, insn) {
                println!("0x{insn:08X}  SKIP (branch/load-store/system — v1 exclusion)");
                skip += 1; continue;
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
