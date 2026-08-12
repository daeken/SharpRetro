// dump the JIT'd code for one x64 block (args: hex bytes) — objdump-able bin out
use sharpretro_jit::tier1::Tier1;
use sharpretro_jit::block_cache::StopReason;
use sharpretro_jit::{Builder, IlType};
use xfusion_recomp::state::X64_LAYOUT;
use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::{decode_insn, DEF_MNEMONICS};
use xfusion_recomp::lift::{lift_one, FLAGS_ALL_LIVE, DEF_FLAGS_MASK, DEF_FLAGS_READ};
fn main() {
    // v4 role-0 straightline: mov qword [rdi],1 ; mfence ; mov rax,[rdi+8] ; mov [rdi+16],rax ; int3
    let code: Vec<u8> = vec![
        0x48,0xC7,0x07,0x01,0x00,0x00,0x00, // mov qword [rdi],1 (7B: REX.W C7 /0 imm32)
        0x0F,0xAE,0xF0,                    // mfence
        0x48,0x8B,0x47,0x08,               // mov rax,[rdi+8]
        0x48,0x89,0x47,0x10,               // mov [rdi+16],rax
        0xCC];
    let mut t1 = Tier1::with_layout(&X64_LAYOUT);
    let mut cur = 0usize;
    let mut insns = vec![];
    while code[cur] != 0xCC {
        let d = decode_insn(&code[cur..], XMode::Bits64).unwrap();
        println!("; {} len={}", DEF_MNEMONICS[d.def_id as usize], d.len);
        cur += d.len as usize;
        insns.push((d, cur));
    }
    let mut per = vec![0u32; insns.len()];
    let mut live = FLAGS_ALL_LIVE;
    for i in (0..insns.len()).rev() {
        let did = insns[i].0.def_id as usize;
        per[i] = live;
        live = (live & !DEF_FLAGS_MASK.get(did).copied().unwrap_or(0))
             | DEF_FLAGS_READ.get(did).copied().unwrap_or(0);
    }
    for (i, (d, next)) in insns.iter().enumerate() {
        lift_one(&mut t1, d, (next - d.len as usize) as u64, XMode::Bits64, per[i]);
    }
    let t = t1.literal(IlType::U64, 0x2000);
    t1.branch(t, false);
    for op in &t1.rec.ops { println!("{:?} imm={:#x} args={:?}", op.kind, op.imm, &op.args[..op.n_args as usize]); }
    let blk = t1.compile();
    std::fs::write("/tmp/fdump.bin", blk.code_bytes()).unwrap();
    println!("wrote /tmp/fdump.bin ({}B)", blk.code_bytes().len());
}
