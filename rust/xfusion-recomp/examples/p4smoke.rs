fn main() {
    use xfusion_recomp::sweep::*;
    use xfusion_recomp::decode::XMode;
    let defs = xfusion_recomp::sweep_defs::SWEEP_DEFS;
    let mut n = 0; let mut shown = 0;
    for d in defs {
        let (ok, _) = enumerate_p4(d, XMode::Bits64, |c, bytes| {
            if shown < 6 && bytes.len() < 8 {
                println!("{}: {:02x?}", d.mnem, bytes); shown += 1;
            }
        });
        n += ok;
    }
    println!("p4 encodings (64-bit): {n}");
}
