use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::{decode_insn, DEF_MNEMONICS};
use xfusion_recomp::lift::{lift_one, FLAGS_ALL_LIVE, DEF_FLAGS_MASK, DEF_FLAGS_READ};
use sharpretro_jit::il_record::IlRecorder;
use sharpretro_jit::regalloc::{linear_scan, Loc};

fn main() {
    let hex = std::env::args().nth(1).unwrap();
    let bytes: Vec<u8> = hex.split(',').map(|s| u8::from_str_radix(s.trim_start_matches("0x"),16).unwrap()).collect();
    let mut insns = vec![]; let mut cur = 0usize;
    while cur < bytes.len() {
        let d = decode_insn(&bytes[cur..], XMode::Bits64).unwrap();
        cur += d.len as usize; insns.push(d);
    }
    println!("block: {} guest insns", insns.len());
    for d in &insns { print!("{} ", DEF_MNEMONICS[d.def_id as usize]); }
    println!();
    // Backward liveness
    let mut live = FLAGS_ALL_LIVE; let mut per = vec![0u32; insns.len()];
    for i in (0..insns.len()).rev() {
        let did = insns[i].def_id as usize;
        per[i] = live;
        live = (live & !DEF_FLAGS_MASK[did]) | DEF_FLAGS_READ[did];
    }
    let mut r = IlRecorder::new();
    let mut cur = 0u64;
    for (i,d) in insns.iter().enumerate() {
        lift_one(&mut r, d, 0x201258+cur, XMode::Bits64, per[i]);
        cur += d.len as u64;
    }
    println!("recorded: {} IlOps, {} SSA vals", r.ops.len(), r.n_vals());
    for n_regs in [8, 10, 12, 14, 16] {
        let a = linear_scan(&r, n_regs);
        let n_reg = a.locs.iter().filter(|l| matches!(l, Loc::Reg(_))).count();
        println!("  n_regs={n_regs:2}: {n_reg} in-reg, {} spilled, {} dead, max_alive={}",
            a.n_spilled, a.n_dead, a.max_alive);
    }
}
