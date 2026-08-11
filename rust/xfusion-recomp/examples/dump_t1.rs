use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::{decode_insn, DEF_MNEMONICS};
use xfusion_recomp::lift::{lift_one, FLAGS_ALL_LIVE, DEF_FLAGS_MASK, DEF_FLAGS_READ};
use sharpretro_jit::tier1::Tier1;
use sharpretro_jit::tier0::Tier0;
use sharpretro_jit::{Builder, IlType};
fn main() {
    // Which block: sum10 loop (default) or LCG loop (from /tmp/elfbench_x64).
    let which = std::env::args().nth(1).unwrap_or("sum10".into());
    let bytes: Vec<u8> = if which == "bb" {
        // branchbench hot block @0x2011ab: imul rdx,r8; add rdx,rsi;
        // mov rax,rdx; test dl,1; jne -0x28  (5 insns, 2 flag-writers, Jcc)
        vec![0x49,0x0F,0xAF,0xD0, 0x48,0x01,0xF2, 0x48,0x89,0xD0,
             0xF6,0xC2,0x01, 0x75,0xD6]
    } else if which == "lcg" {
        // Extract the LCG loop-body from elfbench_x64 (same as lcg_measure's extraction).
        let d = std::fs::read("/tmp/elfbench_x64").unwrap();
        let b = &d[0x230..0x230+170];
        let ns = b.iter().position(|&x| x==0x90).unwrap() + 1;
        let mut i = ns;
        while !(b[i]==0x75 && b[i+1]>=0x80) { i+=1; }
        b[ns..i+2].to_vec()
    } else {
        // sum10 loop body: add rax,rcx; inc rcx; cmp rcx,11; jl -11
        vec![0x48,0x01,0xC8, 0x48,0xFF,0xC1, 0x48,0x83,0xF9,0x0B, 0x7C,0xF5]
    };
    let bytes = &bytes[..];
    let layout = &xfusion_recomp::state::X64_LAYOUT;
    let mut t1 = Tier1::with_layout(layout);
    let mut insns = vec![]; let mut cur = 0usize;
    while cur < bytes.len() {
        let d = decode_insn(&bytes[cur..], XMode::Bits64).unwrap();
        cur += d.len as usize; insns.push(d);
    }
    let mut per = vec![0u32; insns.len()]; let mut live = FLAGS_ALL_LIVE;
    for i in (0..insns.len()).rev() {
        let did = insns[i].def_id as usize;
        per[i] = live;
        live = (live & !DEF_FLAGS_MASK[did]) | DEF_FLAGS_READ[did];
    }
    let mut ipc = 0x10000u64;
    for (i,d) in insns.iter().enumerate() {
        lift_one(&mut t1, d, ipc, XMode::Bits64, per[i]);
        ipc += d.len as u64;
    }
    if !t1.rec.branched() {
        let tv = t1.literal(IlType::U64, ipc as u128);
        t1.branch(tv, false);
    }
    println!("recorded: {} ops, {} vals", t1.rec.ops.len(), t1.rec.n_vals());
    let cb = t1.compile();
    println!("tier-1 code_len = {} bytes ({} words)", cb.code_len, cb.code_len/4);
    std::fs::write(format!("/tmp/t1_{which}.bin"), cb.code_bytes()).unwrap();

    // Also emit through tier-0 for the code-size comparison.
    let mut t0 = Tier0::with_layout(layout);
    let mut ipc0 = 0x10000u64;
    for (i,d) in insns.iter().enumerate() {
        lift_one(&mut t0, d, ipc0, XMode::Bits64, per[i]);
        ipc0 += d.len as u64;
    }
    if !t0.branched() {
        let tv = t0.literal(IlType::U64, ipc0 as u128);
        t0.branch(tv, false);
    }
    let cb0 = t0.finalize();
    println!("tier-0 code_len = {} bytes ({} words)", cb0.code_len, cb0.code_len/4);
    println!("ratio: tier-1/tier-0 = {:.2}×", cb.code_len as f64 / cb0.code_len as f64);
}
