// probe: lift mfence via tier-1 recorder, print op kinds
use sharpretro_jit::tier1::Tier1;
use sharpretro_jit::Builder;
use xfusion_recomp::state::X64_LAYOUT;
use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::{decode_insn, DEF_MNEMONICS};
use xfusion_recomp::lift::{lift_one, FLAGS_ALL_LIVE};
fn main() {
    let bytes = [0x0F, 0xAE, 0xF0, 0xCC];
    let d = decode_insn(&bytes, XMode::Bits64).unwrap();
    println!("def_id={} mnem={}", d.def_id, DEF_MNEMONICS[d.def_id as usize]);
    let mut t1 = Tier1::with_layout(&X64_LAYOUT);
    lift_one(&mut t1, &d, 0x1000, XMode::Bits64, FLAGS_ALL_LIVE);
    for op in &t1.rec.ops { println!("{:?} imm={:#x}", op.kind, op.imm); }
}
