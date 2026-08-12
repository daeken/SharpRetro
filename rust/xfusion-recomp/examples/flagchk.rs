// flag check: lock xaddb [mem],al with seed 0x7F, addend 1 → OF SF ZF CF PF
use xfusion_recomp::state::{X86State, X64_LAYOUT, STATE_WORDS_X64, OFF_RIP, OFF_MEMBASE};
use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::{decode_insn, DEF_MNEMONICS};
use xfusion_recomp::lift::{lift_one, FLAGS_ALL_LIVE};
use sharpretro_jit::interp::{InterpretingBuilder, HostMem};
use sharpretro_jit::tier0::Tier0;
use sharpretro_jit::{Builder, IlType, RegFile};

fn main() {
    // mmap a page for the seed
    unsafe { libc::mmap(0x600000 as *mut _, 0x1000, libc::PROT_READ|libc::PROT_WRITE,
        libc::MAP_PRIVATE|libc::MAP_ANONYMOUS|libc::MAP_FIXED, -1, 0); }
    for (name, bytes, seed, expect) in [
        ("xaddb 7F+1", vec![0xF0u8,0x0F,0xC0,0x07], 0x7Fu64, "OF=1 SF=1 ZF=0 CF=0 PF=0"),
        ("xaddb FF+1", vec![0xF0,0x0F,0xC0,0x07], 0xFF, "OF=0 SF=0 ZF=1 CF=1 PF=1"),
        ("xaddb 80+FF", vec![0xF0,0x0F,0xC0,0x07], 0x80, "(addend FF) OF=1 CF=1"),
    ] {
        // INTERP arm
        let mut st = X86State::default();
        st.rip = 0;
        st.gpr[0] = if name.contains("80+FF") { 0xFF } else { 1 };  // al = addend
        st.gpr[7] = 0x600000;                                        // rdi
        unsafe { *(0x600000 as *mut u64) = seed; }
        let d = decode_insn(&bytes, XMode::Bits64).unwrap();
        let mut mem = HostMem;
        let mut b = InterpretingBuilder::new(&mut st, &mut mem, 0);
        lift_one(&mut b, &d, 0, XMode::Bits64, FLAGS_ALL_LIVE);
        let fl = st.eflags;
        println!("{name}: interp OF={} SF={} ZF={} CF={} PF={}   (want {expect})  mem={:#x} al={:#x}",
            (fl>>11)&1, (fl>>7)&1, (fl>>6)&1, fl&1, (fl>>2)&1,
            unsafe { *(0x600000 as *const u8) }, st.gpr[0] & 0xFF);
    }
}
