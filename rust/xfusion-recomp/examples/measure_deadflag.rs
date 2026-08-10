use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::{decode_insn, DEF_MNEMONICS};
use xfusion_recomp::lift::{lift_one, FLAGS_ALL_LIVE, DEF_FLAGS_MASK, DEF_FLAGS_READ};
use xfusion_recomp::state::X64_LAYOUT;
use sharpretro_jit::tier0::Tier0;

fn main() {
    // sum10 loop body: add eax,ecx / inc ecx / cmp ecx,edx / jl -8
    let bytes: &[u8] = &[0x01,0xC8, 0xFF,0xC1, 0x39,0xD1, 0x7C,0xF8];
    let mut insns = vec![];
    let mut cur = 0usize;
    while cur < bytes.len() {
        let d = decode_insn(&bytes[cur..], XMode::Bits64).unwrap();
        cur += d.len as usize;
        insns.push(d);
    }
    // Backward liveness
    let mut live = FLAGS_ALL_LIVE;
    let mut per = vec![0u32; insns.len()];
    for i in (0..insns.len()).rev() {
        let did = insns[i].def_id as usize;
        per[i] = live;
        live = (live & !DEF_FLAGS_MASK[did]) | DEF_FLAGS_READ[did];
    }
    for (mode, name) in [(false, "ALL_LIVE"), (true, "liveness")] {
        let mut t0 = Tier0::with_layout(&X64_LAYOUT);
        let mut cur = 0u64;
        for (i, d) in insns.iter().enumerate() {
            let lf = if mode { per[i] } else { FLAGS_ALL_LIVE };
            lift_one(&mut t0, d, 0x1000F+cur, XMode::Bits64, lf);
            cur += d.len as u64;
        }
        eprintln!("  {} : {} bytes ({} words)",
            name, t0.enc.buf.len(), t0.enc.buf.len()/4);
    }
    eprintln!("  live_out per-insn: {:?}", per.iter().map(|x| format!("0x{x:03X}")).collect::<Vec<_>>());
    for (i,d) in insns.iter().enumerate() {
        let did = d.def_id as usize;
        let dead = DEF_FLAGS_MASK[did] & !per[i];
        eprintln!("    {}: writes=0x{:03X} live_out=0x{:03X} DEAD=0x{:03X} ({} flags dropped)",
            DEF_MNEMONICS[did], DEF_FLAGS_MASK[did], per[i], dead, dead.count_ones());
    }
}
