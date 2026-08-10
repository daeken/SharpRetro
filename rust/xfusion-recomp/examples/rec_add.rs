use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::decode_insn;
use xfusion_recomp::lift::{lift_one, FLAGS_ALL_LIVE};
use sharpretro_jit::il_record::IlRecorder;
fn main() {
    let bytes = [0x49u8,0x83,0xC2,0xFC];  // add r10, -4
    let d = decode_insn(&bytes, XMode::Bits64).unwrap();
    println!("d.def_id = {}, d.imm0 = 0x{:x} (as i64 = {})", d.def_id, d.imm0, d.imm0 as i64);
    let mut r = IlRecorder::new();
    lift_one(&mut r, &d, 0x1000, XMode::Bits64, FLAGS_ALL_LIVE);
    for (i,op) in r.ops.iter().enumerate() {
        println!("  [{i}] {:?} out={:?} args={:?} ty={:?} imm=0x{:x}",
            op.kind, op.out, &op.args[..op.n_args as usize], op.ty, op.imm);
    }
}
