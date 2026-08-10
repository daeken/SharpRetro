//! Tier-1 scoping: record a real x64 block via IlRecorder, dump SSA-ops + live-ranges.
//! This is the record→allocate→emit step-1 verified against real lift.rs output.
use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::{decode_insn, DEF_MNEMONICS};
use xfusion_recomp::lift::{lift_one, FLAGS_ALL_LIVE, DEF_FLAGS_MASK, DEF_FLAGS_READ};
use sharpretro_jit::il_record::{IlRecorder, IlOpKind};

fn main() {
    // sum10 loop body under dead-flag-elim liveness (ADD/INC's flags dead, CMP's kept).
    let bytes: &[u8] = &[0x01,0xC8, 0xFF,0xC1, 0x39,0xD1, 0x7C,0xF8];  // add;inc;cmp;jl
    let mut insns = vec![]; let mut cur = 0usize;
    while cur < bytes.len() {
        let d = decode_insn(&bytes[cur..], XMode::Bits64).unwrap();
        cur += d.len as usize; insns.push(d);
    }
    // Backward liveness (same as compile_block's).
    let mut live = FLAGS_ALL_LIVE;
    let mut per = vec![0u32; insns.len()];
    for i in (0..insns.len()).rev() {
        let did = insns[i].def_id as usize;
        per[i] = live;
        live = (live & !DEF_FLAGS_MASK[did]) | DEF_FLAGS_READ[did];
    }
    // Record.
    let mut r = IlRecorder::new();
    let mut cur = 0u64;
    for (i, d) in insns.iter().enumerate() {
        lift_one(&mut r, d, 0x1000F+cur, XMode::Bits64, per[i]);
        cur += d.len as u64;
    }
    // Dump.
    let (def_at, last_use) = r.live_ranges();
    println!("=== sum10 loop-block (add;inc;cmp;jl w/ dead-flag): {} ops, {} SSA vals ===",
        r.ops.len(), r.n_vals());
    for (i, op) in r.ops.iter().enumerate() {
        let out = op.out.map(|v| format!("v{v} = ")).unwrap_or_default();
        let args: Vec<_> = (0..op.n_args).map(|k| format!("v{}", op.args[k as usize])).collect();
        println!("  [{i:3}] {}{:?} {} ty={:?} imm=0x{:X}",
            out, op.kind, args.join(","), op.ty, op.imm);
    }
    println!("=== live-ranges (def_at → last_use) ===");
    let mut max_alive = 0;
    for i in 0..r.ops.len() {
        let alive: Vec<_> = (0..r.n_vals()).filter(|&v| def_at[v as usize] <= i && i <= last_use[v as usize]).collect();
        max_alive = max_alive.max(alive.len());
    }
    for v in 0..r.n_vals() {
        println!("  v{v:3}: [{:3} → {:3}]  span={}",
            def_at[v as usize], last_use[v as usize], last_use[v as usize]-def_at[v as usize]);
    }
    println!("MAX SIMULTANEOUS LIVE = {max_alive}  (aarch64 has ~14 allocatable → {} spills needed)",
        if max_alive <= 14 { "ZERO".to_string() } else { format!("{}", max_alive-14) });
}
