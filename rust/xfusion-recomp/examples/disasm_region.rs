use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::{decode_insn, DEF_MNEMONICS};
fn main() {
    let base_rva: u64 = 0x1929c80;
    let d = std::fs::read("/tmp/cp_workfn.bin").unwrap();
    let mut i = 0usize;
    while i < d.len() {
        let bytes = &d[i..(i+15).min(d.len())];
        match decode_insn(bytes, XMode::Bits64) {
            Some(dd) => {
                let m = DEF_MNEMONICS[dd.def_id as usize];
                let hex: String = bytes[..dd.len as usize].iter().map(|b|format!("{b:02X}")).collect::<Vec<_>>().join(" ");
                println!("0x{:x}:  {:32}  {} (def={})", 0x140000000+base_rva+i as u64, hex, m, dd.def_id);
                i += dd.len as usize;
            }
            None => {
                println!("0x{:x}:  {:02X}  ?? UNDECODED", 0x140000000+base_rva+i as u64, bytes[0]);
                i += 1;
            }
        }
    }
}
