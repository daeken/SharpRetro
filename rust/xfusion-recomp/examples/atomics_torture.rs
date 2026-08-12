// C1 ACCEPTANCE: N host threads run the SAME guest code (lock inc/add/dec,
// xchg-mem, cmpxchg spinlock protecting a PLAIN counter) against ONE shared
// struct through per-thread BlockCaches (the cp2077 worker-driver shape).
// Exact final counts ⟺ the lock-prefix lowering is genuinely atomic + the
// CAS spinlock provides real mutual exclusion (the plain counter is the
// canary: without atomicity/exclusion it loses updates).
//
// Usage: atomics_torture <elf> [n_threads]   (default 8)

use sharpretro_jit::block_cache::{BlockCache, BlockCompiler, StopReason};
use sharpretro_jit::{Builder, IlType};
use xfusion_recomp::state::{X64_LAYOUT, STATE_WORDS_X64, OFF_RIP, OFF_MEMBASE};
use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::{decode_insn, DEF_MNEMONICS};
use xfusion_recomp::lift::{lift_one, FLAGS_ALL_LIVE, DEF_FLAGS_MASK, DEF_FLAGS_READ};

fn is_branch(m: &str) -> bool {
    m.starts_with('J') || m == "CALL" || m.starts_with("RET") || m.starts_with("LOOP")
}

struct C;
impl BlockCompiler for C {
    fn fetch(&self, pc: u64) -> u32 {
        u32::from_le_bytes(unsafe { *(pc as *const [u8; 4]) })
    }
    fn is_stop(&self, insn: u32) -> bool { insn as u8 == 0xCC }
    fn compile_block<B: Builder<Val = u32>>(&self, b: &mut B, pc: u64, _m: u32)
        -> (u64, StopReason)
    {
        let mut insns = vec![]; let mut cur = pc; let mut stop = StopReason::MaxInsns;
        for _ in 0..64 {
            let bytes = unsafe { std::slice::from_raw_parts(cur as *const u8, 15) };
            if bytes[0] == 0xCC { stop = StopReason::StopInsn; break; }
            let d = decode_insn(bytes, XMode::Bits64).unwrap();
            let m = DEF_MNEMONICS[d.def_id as usize];
            cur += d.len as u64;
            let br = is_branch(m);
            insns.push((d, cur));
            if br { stop = StopReason::Branched; break; }
        }
        if insns.is_empty() {
            let t = b.literal(IlType::U64, cur as u128);
            b.branch(t, false);
            return (cur, stop);
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
            lift_one(b, d, next - d.len as u64, XMode::Bits64, per[i]);
        }
        if !b.branched() {
            let t = b.literal(IlType::U64, cur as u128);
            b.branch(t, false);
        }
        (cur, stop)
    }
}

fn load_elf(path: &str) -> u64 {
    let data = std::fs::read(path).unwrap();
    let e_entry = u64::from_le_bytes(data[24..32].try_into().unwrap());
    let e_phoff = u64::from_le_bytes(data[32..40].try_into().unwrap()) as usize;
    let e_phnum = u16::from_le_bytes(data[56..58].try_into().unwrap()) as usize;
    for i in 0..e_phnum {
        let p = e_phoff + i * 56;
        let p_type = u32::from_le_bytes(data[p..p+4].try_into().unwrap());
        if p_type != 1 { continue; }
        let p_offset = u64::from_le_bytes(data[p+8..p+16].try_into().unwrap()) as usize;
        let p_vaddr = u64::from_le_bytes(data[p+16..p+24].try_into().unwrap());
        let p_filesz = u64::from_le_bytes(data[p+32..p+40].try_into().unwrap()) as usize;
        let p_memsz = u64::from_le_bytes(data[p+40..p+48].try_into().unwrap()) as usize;
        unsafe {
            let base = p_vaddr & !0xFFF;
            let len = ((p_vaddr + p_memsz as u64 + 0xFFF) & !0xFFF) - base;
            libc::mmap(base as *mut _, len as usize,
                libc::PROT_READ | libc::PROT_WRITE,
                libc::MAP_PRIVATE | libc::MAP_ANONYMOUS | libc::MAP_FIXED, -1, 0);
            std::ptr::copy_nonoverlapping(data.as_ptr().add(p_offset),
                p_vaddr as *mut u8, p_filesz);
        }
    }
    e_entry
}

fn main() {
    let args: Vec<String> = std::env::args().collect();
    let entry = load_elf(&args[1]);
    let n_threads: usize = args.get(2).map(|s| s.parse().unwrap()).unwrap_or(8);
    const ITERS: u64 = 200_000;

    // Shared guest struct + per-thread stacks.
    let shm = 0x600000u64;
    let stk_base = 0x7f0000000000u64;
    unsafe {
        libc::mmap(shm as *mut _, 0x1000, libc::PROT_READ | libc::PROT_WRITE,
            libc::MAP_PRIVATE | libc::MAP_ANONYMOUS | libc::MAP_FIXED, -1, 0);
        libc::mmap((stk_base - 0x100000 * n_threads as u64) as *mut _,
            0x100000 * n_threads,
            libc::PROT_READ | libc::PROT_WRITE,
            libc::MAP_PRIVATE | libc::MAP_ANONYMOUS | libc::MAP_FIXED, -1, 0);
    }

    let t0 = std::time::Instant::now();
    std::thread::scope(|sc| {
        for t in 0..n_threads {
            sc.spawn(move || {
                let mut state = vec![0u64; STATE_WORDS_X64];
                state[OFF_RIP] = entry;
                state[OFF_MEMBASE] = 0;                       // identity map
                // X64_LAYOUT: GPR = words 0..15 (state.rs OFF_GPR=0).
                state[7] = shm;                                    // rdi = arg0
                state[4] = stk_base - 0x100000 * t as u64 - 0x4000; // rsp
                let mut cache = BlockCache::with_layout(&X64_LAYOUT);
                cache.max_block_insns = 64;
                cache.run(&C, &mut state[..], 0, usize::MAX);
            });
        }
    });
    let wall = t0.elapsed().as_secs_f64();

    let rd = |off: u64| unsafe { *((shm + off) as *const u64) };
    let n = n_threads as u64;
    let want = [
        ("xadd_sum", rd(0),  ITERS * n),
        ("incs",     rd(8),  ITERS * n),
        ("adds",     rd(16), 2 * ITERS * n),
        ("decs",     rd(24), (ITERS * n).wrapping_neg()),
        ("protected",rd(48), ITERS * n),
    ];
    let mut ok = true;
    for (name, got, exp) in want {
        let m = if got == exp { "✓" } else { ok = false; "✗ LOST-UPDATES" };
        println!("{m} {name}: got={got:#x} want={exp:#x}");
    }
    println!("(swap_last={:#x} — any thread's last i, unchecked)", rd(32));
    println!("lock word final = {} (want 0)", rd(40) as u32);
    if rd(40) as u32 != 0 { ok = false; }
    println!("[atomics_torture] {} threads × {} iters, {:.2}s — {}",
        n_threads, ITERS, wall, if ok { "PASS" } else { "FAIL" });
    std::process::exit(if ok { 0 } else { 9 });
}
