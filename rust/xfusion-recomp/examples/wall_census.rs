use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::{decode_insn, DEF_MNEMONICS};
use std::collections::HashMap;
fn main() {
    // Which def_ids are intrinsic-stubs? Read from lift.rs at build-time is
    // fragile; instead, take a comma-list from stdin/env.
    let stub_defs: std::collections::HashSet<u32> = std::env::var("STUB_DEFS")
        .unwrap_or_default().split(',').filter_map(|s| s.trim().parse().ok()).collect();
    let d = std::fs::read("/tmp/cp2077/Cyberpunk2077.exe").unwrap();
    let pe = u32::from_le_bytes(d[0x3C..0x40].try_into().unwrap()) as usize;
    let optsz = u16::from_le_bytes(d[pe+20..pe+22].try_into().unwrap()) as usize;
    let sec0 = pe+24+optsz;
    let (vsz, _va, rsz, ptr) = (
        u32::from_le_bytes(d[sec0+8..sec0+12].try_into().unwrap()) as usize,
        u32::from_le_bytes(d[sec0+12..sec0+16].try_into().unwrap()) as usize,
        u32::from_le_bytes(d[sec0+16..sec0+20].try_into().unwrap()) as usize,
        u32::from_le_bytes(d[sec0+20..sec0+24].try_into().unwrap()) as usize);
    let text = &d[ptr..ptr+rsz.min(vsz)];
    let mut hist: HashMap<u32, u64> = HashMap::new();
    let mut n_total = 0u64;
    let mut i = 0usize;
    while i + 15 <= text.len() {
        match decode_insn(&text[i..i+15], XMode::Bits64) {
            Some(dd) => {
                n_total += 1;
                if stub_defs.is_empty() || stub_defs.contains(&(dd.def_id as u32)) {
                    *hist.entry(dd.def_id as u32).or_default() += 1;
                }
                i += dd.len as usize;
            }
            None => { i += 1; }
        }
    }
    println!("total: {n_total}");
    let mut v: Vec<_> = hist.into_iter().collect();
    v.sort_by_key(|&(_,c)| std::cmp::Reverse(c));
    for (did, c) in v.iter().take(40) {
        println!("  {c:8}  def={did:3}  {}", DEF_MNEMONICS[*did as usize]);
    }
}
