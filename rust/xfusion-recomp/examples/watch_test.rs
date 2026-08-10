use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::decode_insn;
use xfusion_recomp::lift::{lift_one, FLAGS_ALL_LIVE};
use xfusion_recomp::state::X86State;
use sharpretro_jit::interp::{InterpretingBuilder, FlatMem, GuestMem};

fn main() {
    // rep stosq: mov rax,imm; mov rdi,0x10800; mov rcx,4; rep stosq; int3.
    // Watch [0x10810, +8) — the 3rd qword of the fill. Should print exactly one hit.
    let prog: &[u8] = &[
        0x48,0xB8,0xBE,0xBA,0xFE,0xCA,0xEF,0xBE,0xAD,0xDE,  // mov rax,0xDEADBEEFCAFEBABE
        0x48,0xC7,0xC7,0x00,0x08,0x01,0x00,                 // mov rdi,0x10800
        0x48,0xC7,0xC1,0x04,0x00,0x00,0x00,                 // mov rcx,4
        0xF3,0x48,0xAB,                                     // rep stosq
        0xCC,
    ];
    let mut mem = FlatMem { base: 0, bytes: vec![0u8; 0x20000] };
    for (i,&b) in prog.iter().enumerate() { mem.write(0x1000+i as u64, 8, b as u128); }
    let mut st = X86State::default();
    let mut ib = InterpretingBuilder::new(&mut st, &mut mem, 0);
    ib.set_watch(0x10810, 8);
    let mut pc = 0x1000u64;
    loop {
        let bs: Vec<u8> = (0..15).map(|k| ib.mem.read(pc+k, 8) as u8).collect();
        if bs[0] == 0xCC { break; }
        let d = decode_insn(&bs, XMode::Bits64).unwrap();
        ib.insn_pc = pc;
        ib.branched = false;
        lift_one(&mut ib, &d, pc, XMode::Bits64, FLAGS_ALL_LIVE);
        pc += d.len as u64;
    }
    println!("done, rdi=0x{:x} rcx=0x{:x}", ib.state.gpr[7], ib.state.gpr[1]);
}
