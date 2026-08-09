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

fn main() {
    let args: Vec<String> = std::env::args().collect();
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
