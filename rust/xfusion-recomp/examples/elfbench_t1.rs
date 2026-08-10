//! elfbench_t1 — the FEX-comparable wall-clock through TIER-1. Loads the same
//! static x64 ELF (mmap PT_LOAD @ p_vaddr, mem_base=0, entry=e_entry) as the
//! tier-0 elfbench, but drives blocks through Tier1 (record → linear_scan →
//! reg-allocated emit). Standalone block-cache (HashMap<pc, CompiledBlock>);
//! BlockCache-the-crate-type is Tier0-specific for now.
//!
//! run: cargo run --release --example elfbench_t1 -- /tmp/elfbench_x64
//! Compare to bench_gate.sh's tier0+deadflag column (1.96s baseline).

use sharpretro_jit::tier1::Tier1;
use sharpretro_jit::tier0::{Tier0, CompiledBlock};
use sharpretro_jit::{Builder, IlType};
use xfusion_recomp::state::{X64_LAYOUT, STATE_WORDS_X64, OFF_RIP, OFF_MEMBASE};
use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::{decode_insn, DEF_MNEMONICS};
use xfusion_recomp::lift::{lift_one, FLAGS_ALL_LIVE, DEF_FLAGS_MASK, DEF_FLAGS_READ};
use std::time::Instant;
use std::collections::HashMap;

/// Load a static x64 ELF at its p_vaddr addresses (mem_base=0). Returns e_entry.
unsafe fn load_elf(path: &str) -> u64 {
    let elf = std::fs::read(path).expect("read ELF");
    assert_eq!(&elf[0..4], b"\x7fELF");
    assert_eq!(elf[4], 2, "not ELF64");
    let e_entry = u64::from_le_bytes(elf[24..32].try_into().unwrap());
    let e_phoff = u64::from_le_bytes(elf[32..40].try_into().unwrap()) as usize;
    let e_phentsize = u16::from_le_bytes(elf[54..56].try_into().unwrap()) as usize;
    let e_phnum = u16::from_le_bytes(elf[56..58].try_into().unwrap()) as usize;
    for i in 0..e_phnum {
        let ph = e_phoff + i * e_phentsize;
        if u32::from_le_bytes(elf[ph..ph+4].try_into().unwrap()) != 1 { continue; } // PT_LOAD
        let p_offset = u64::from_le_bytes(elf[ph+8..ph+16].try_into().unwrap()) as usize;
        let p_vaddr  = u64::from_le_bytes(elf[ph+16..ph+24].try_into().unwrap());
        let p_filesz = u64::from_le_bytes(elf[ph+32..ph+40].try_into().unwrap()) as usize;
        let p_memsz  = u64::from_le_bytes(elf[ph+40..ph+48].try_into().unwrap()) as usize;
        let page_lo = p_vaddr & !0xFFF;
        let map_sz = (((p_vaddr + p_memsz as u64) - page_lo + 0xFFF) & !0xFFF) as usize;
        let a = libc::mmap(page_lo as *mut libc::c_void, map_sz,
            libc::PROT_READ|libc::PROT_WRITE|libc::PROT_EXEC,
            libc::MAP_PRIVATE|libc::MAP_ANONYMOUS|libc::MAP_FIXED, -1, 0);
        assert_eq!(a as u64, page_lo, "PT_LOAD mmap @{:#x} failed", page_lo);
        std::ptr::copy_nonoverlapping(elf[p_offset..].as_ptr(), p_vaddr as *mut u8, p_filesz);
    }
    // Also map a stack region.
    let stk_top = 0x7fff_0000_0000u64;
    let stk_sz = 0x100000usize;
    libc::mmap((stk_top - stk_sz as u64) as *mut _, stk_sz,
        libc::PROT_READ|libc::PROT_WRITE, libc::MAP_PRIVATE|libc::MAP_ANONYMOUS|libc::MAP_FIXED, -1, 0);
    e_entry
}

fn is_branch(m: &str) -> bool {
    m.starts_with('J') || m == "CALL" || m == "RET" || m == "RETI" || m == "RETF" || m.starts_with("LOOP")
}

fn compile_t1(pc: u64) -> CompiledBlock {
    let mut t1 = Tier1::with_layout(&X64_LAYOUT);
    // Decode-collect until branch or int3, up to a reasonable cap.
    let mut insns = vec![]; let mut cur = pc;
    for _ in 0..64 {
        let bytes = unsafe { std::slice::from_raw_parts(cur as *const u8, 15) };
        if bytes[0] == 0xCC { break; }
        let d = match decode_insn(bytes, XMode::Bits64) {
            Some(d) => d,
            None => panic!("elfbench_t1: undecoded @{:#x}: {:02x?}", cur, &bytes[..8]),
        };
        let mnem = DEF_MNEMONICS[d.def_id as usize];
        cur += d.len as u64;
        let br = is_branch(mnem);
        insns.push((d, cur - d.len as u64));
        if br { break; }
    }
    // Backward liveness (dead-flag-elim).
    let mut per = vec![0u32; insns.len()];
    let mut live = FLAGS_ALL_LIVE;
    for i in (0..insns.len()).rev() {
        let did = insns[i].0.def_id as usize;
        per[i] = live;
        live = (live & !DEF_FLAGS_MASK.get(did).copied().unwrap_or(0))
             | DEF_FLAGS_READ.get(did).copied().unwrap_or(0);
    }
    // Forward emit into Tier1's recorder.
    for (i, (d, ipc)) in insns.iter().enumerate() {
        lift_one(&mut t1, d, *ipc, XMode::Bits64, per[i]);
    }
    if !t1.rec.branched() {
        let tv = t1.literal(IlType::U64, cur as u128);
        t1.branch(tv, false);
    }
    t1.compile()
}

/// Compile the same block through tier-0 (for lockstep diff).
fn compile_t0(pc: u64) -> CompiledBlock {
    let mut t0 = Tier0::with_layout(&X64_LAYOUT);
    let mut insns = vec![]; let mut cur = pc;
    for _ in 0..64 {
        let bytes = unsafe { std::slice::from_raw_parts(cur as *const u8, 15) };
        if bytes[0] == 0xCC { break; }
        let d = decode_insn(bytes, XMode::Bits64).unwrap();
        let mnem = DEF_MNEMONICS[d.def_id as usize];
        cur += d.len as u64;
        let br = is_branch(mnem);
        insns.push((d, cur - d.len as u64));
        if br { break; }
    }
    let mut per = vec![0u32; insns.len()];
    let mut live = FLAGS_ALL_LIVE;
    for i in (0..insns.len()).rev() {
        let did = insns[i].0.def_id as usize;
        per[i] = live;
        live = (live & !DEF_FLAGS_MASK.get(did).copied().unwrap_or(0))
             | DEF_FLAGS_READ.get(did).copied().unwrap_or(0);
    }
    for (i, (d, ipc)) in insns.iter().enumerate() {
        lift_one(&mut t0, d, *ipc, XMode::Bits64, per[i]);
    }
    if !t0.branched() {
        let tv = t0.literal(IlType::U64, cur as u128);
        t0.branch(tv, false);
    }
    t0.finalize()
}

fn main() {
    let path = std::env::args().nth(1).unwrap_or("/tmp/elfbench_x64".into());
    let entry = unsafe { load_elf(&path) };
    println!("[elfbench_t1] loaded {path}, entry={:#x}", entry);

    // LOCKSTEP=1: run tier-0 and tier-1 in parallel on identical state, diff
    // after each block-exec. Names the first-divergent register + block-pc.
    if std::env::var("LOCKSTEP").is_ok() {
        let mut f0 = [0u64; STATE_WORDS_X64];
        let mut f1 = [0u64; STATE_WORDS_X64];
        f0[OFF_RIP] = entry; f1[OFF_RIP] = entry;
        f0[4] = 0x7fff_0000_0000u64 - 0x100; f1[4] = f0[4];
        // ‡ shared mem_base=0 → both tiers write the SAME host memory. For a
        //   pure-compute bench (no stores that later loads read), that's fine;
        //   for a bench that stores-then-loads, tier-0's store could contaminate
        //   tier-1's load. LCG is register-only (no mem in the loop body), so ok.
        let mut b0: HashMap<u64, CompiledBlock> = HashMap::new();
        let mut b1: HashMap<u64, CompiledBlock> = HashMap::new();
        let mut n = 0u64;
        loop {
            let pc = f0[OFF_RIP];
            if pc != f1[OFF_RIP] {
                println!("PC-DIVERGE @exec#{n}: t0-pc={:#x} t1-pc={:#x}", pc, f1[OFF_RIP]);
                break;
            }
            if unsafe { *(pc as *const u8) } == 0xCC {
                println!("both reached int3 @exec#{n}, no divergence. rax: t0=0x{:x} t1=0x{:x}",
                    f0[0], f1[0]);
                break;
            }
            b0.entry(pc).or_insert_with(|| compile_t0(pc)).exec_slice(&mut f0[..]);
            b1.entry(pc).or_insert_with(|| compile_t1(pc)).exec_slice(&mut f1[..]);
            n += 1;
            // Diff GPRs + eflags (mask to the flags that matter).
            for r in 0..16 {
                if f0[r] != f1[r] {
                    println!("REG-DIVERGE @exec#{n} block-pc={:#x} r{r}({}): t0=0x{:x} t1=0x{:x}",
                        pc, ["rax","rcx","rdx","rbx","rsp","rbp","rsi","rdi",
                             "r8","r9","r10","r11","r12","r13","r14","r15"][r],
                        f0[r], f1[r]);
                    // Dump the block's insns for context.
                    let mut cur = pc;
                    for _ in 0..64 {
                        let bytes = unsafe { std::slice::from_raw_parts(cur as *const u8, 15) };
                        if bytes[0] == 0xCC { break; }
                        let d = decode_insn(bytes, XMode::Bits64).unwrap();
                        let m = DEF_MNEMONICS[d.def_id as usize];
                        println!("    {:#x}: {:02x?} {}", cur, &bytes[..d.len as usize], m);
                        cur += d.len as u64;
                        if is_branch(m) { break; }
                    }
                    // continue to dump all diverging regs
                }
            }
            if (0..16).any(|r| f0[r]!=f1[r]) { println!("(all divergent regs above)"); return; }
            if n > 1_000_000 { println!("1M execs no diverge; stopping"); break; }
        }
        return;
    }

    let mut flat = [0u64; STATE_WORDS_X64];
    flat[OFF_RIP] = entry;
    flat[4] = 0x7fff_0000_0000u64 - 0x100;   // rsp
    flat[OFF_MEMBASE] = 0;                   // mem_base=0

    let mut blocks: HashMap<u64, CompiledBlock> = HashMap::new();
    let mut n_execs = 0u64; let mut n_compiles = 0u64;
    let max_execs: u64 = std::env::var("MAX_EXECS").ok()
        .and_then(|s| s.parse().ok()).unwrap_or(5_000_000_000);

    // FASTLOOP=1: hoist spill-vec alloc + HashMap-lookup out of the hot loop.
    // Pre-compiles all reachable blocks (walks until a repeat pc), then runs
    // via a Vec<(pc, entry_fn, n_slots)> lookup with a single reused spill area.
    // Delta vs default = the driver-loop overhead (alloc + hash + exec_slice indirection).
    let fastloop = std::env::var("FASTLOOP").is_ok();
    // Shared spill area (max n_slots across all blocks; +64 headroom).
    let mut spill = vec![0u64; 256];

    // NOHASH=1 (implies FASTLOOP): after warm-up, cache pc→entry_fn in a small
    // linear-probed Vec<(pc, fn)>. 3 blocks → linear-scan is ~free. Isolates
    // HashMap-lookup cost from the residual (which is then prologue/epilogue +
    // the block-body itself = the block-linking floor).
    let nohash = std::env::var("NOHASH").is_ok();
    let t0 = Instant::now();
    if nohash {
        // Warm-up: compile all blocks first (run until n_execs > 100 covers all pcs).
        let sp = flat.as_mut_ptr(); let spp = spill.as_mut_ptr();
        let mut fast: Vec<(u64, extern "C" fn(*mut u64,*mut u64))> = Vec::with_capacity(8);
        loop {
            let pc = flat[OFF_RIP];
            if unsafe { *(pc as *const u8) } == 0xCC { break; }
            match fast.iter().find(|(p,_)| *p==pc) {
                Some(&(_,f)) => f(sp, spp),
                None => {
                    let cb = blocks.entry(pc).or_insert_with(|| { n_compiles+=1; compile_t1(pc) });
                    let f = cb.entry_fn(); fast.push((pc, f)); f(sp, spp);
                }
            }
            n_execs += 1;
            if n_execs > max_execs { break; }
        }
    } else {
        loop {
            let pc = flat[OFF_RIP];
            let b0 = unsafe { *(pc as *const u8) };
            if b0 == 0xCC { break; }
            let cb = blocks.entry(pc).or_insert_with(|| { n_compiles += 1; if std::env::var("T0").is_ok() { compile_t0(pc) } else { compile_t1(pc) } });
            if fastloop {
                (cb.entry_fn())(flat.as_mut_ptr(), spill.as_mut_ptr());
            } else {
                cb.exec_slice(&mut flat[..]);
            }
            n_execs += 1;
            if n_execs > max_execs { println!("  max_execs hit"); break; }
        }
    }
    let wall = t0.elapsed().as_secs_f64();
    println!("[elfbench_t1] wall = {:.4}s  ({} block-execs, {} compiles)  rax=0x{:x}",
        wall, n_execs, n_compiles, flat[0]);
}
