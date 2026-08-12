// rr acceptance: record a racy interleaving, replay it EXACTLY.
//
// Guest (tests-src/rrguest.c, per thread): ITERS × { slot = lock
// xadd(cursor,1); array[slot] = tid }. The array's tid-sequence IS the
// interleaving of the atomic ops — nondeterministic across plain runs, but
// XF_RR=record captures the order and XF_RR=replay must reproduce it
// byte-exact via the wait-for-turn machinery.
//
// Modes (XF_RR env, same run-shape as atomics_torture — harness transcribed
// from it verbatim per the freeze-law):
//   (off)     control: two plain runs → arrays should DIFFER (genuinely racy)
//   record    run + save /tmp/rr_test.{xfrr,arr}
//   replay    run against the log → array must equal the recorded one

use sharpretro_jit::block_cache::{BlockCache, BlockCompiler, StopReason};
use sharpretro_jit::{Builder, IlType};
use sharpretro_jit::rr;
use xfusion_recomp::state::{X64_LAYOUT, STATE_WORDS_X64, OFF_RIP, OFF_MEMBASE, OFF_RR};
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

const SHM: u64 = 0x600000;
const STK_BASE: u64 = 0x7f0000000000;

/// One full run. rr_handles[tid] = Some(RrThread box) in record/replay mode.
/// Returns (interleaving-array, drained per-thread logs if recording).
fn run(entry: u64, n_threads: usize, mut rr_handles: Vec<Option<Box<rr::RrThread>>>)
    -> (Vec<u8>, Vec<(u64, Vec<(u64, u64)>)>)
{
    unsafe {
        libc::mmap(SHM as *mut _, 0x10000, libc::PROT_READ | libc::PROT_WRITE,
            libc::MAP_PRIVATE | libc::MAP_ANONYMOUS | libc::MAP_FIXED, -1, 0);
        libc::mmap((STK_BASE - 0x100000 * n_threads as u64) as *mut _,
            0x100000 * n_threads, libc::PROT_READ | libc::PROT_WRITE,
            libc::MAP_PRIVATE | libc::MAP_ANONYMOUS | libc::MAP_FIXED, -1, 0);
    }
    let handles: Vec<(u64, u64)> = rr_handles.iter().map(|h|
        (h.as_ref().map(|b| &**b as *const rr::RrThread as u64).unwrap_or(0), 0)).collect();
    std::thread::scope(|sc| {
        for t in 0..n_threads {
            let rr_ptr = handles[t].0;
            sc.spawn(move || {
                let mut state = vec![0u64; STATE_WORDS_X64];
                state[OFF_RIP] = entry;
                state[OFF_MEMBASE] = 0;
                state[OFF_RR] = rr_ptr;
                state[7] = SHM;                                     // rdi
                state[6] = t as u64;                                // rsi = tid
                state[4] = STK_BASE - 0x100000 * t as u64 - 0x4000; // rsp
                let mut cache = BlockCache::with_layout(&X64_LAYOUT);
                cache.max_block_insns = 64;
                cache.run(&C, &mut state[..], 0, usize::MAX);
            });
        }
    });
    let n = unsafe { *(SHM as *const u64) } as usize;
    let arr = unsafe { std::slice::from_raw_parts((SHM + 0x100) as *const u8, n) }.to_vec();
    let mut logs = vec![];
    for (t, h) in rr_handles.iter_mut().enumerate() {
        if let Some(b) = h { logs.push((t as u64, rr::drain(b))); }
    }
    (arr, logs)
}

fn main() {
    let args: Vec<String> = std::env::args().collect();
    let elf = &args[1];
    let n: usize = args.get(2).map(|s| s.parse().unwrap()).unwrap_or(4);
    let entry = load_elf(elf);

    match rr::rr_mode() {
        rr::RrMode::Record => {
            let handles: Vec<_> = (0..n as u64).map(|t| Some(rr::record_thread(t))).collect();
            let (arr, logs) = run(entry, n, handles);
            rr::save_log("/tmp/rr_test.xfrr", &logs).unwrap();
            std::fs::write("/tmp/rr_test.arr", &arr).unwrap();
            let total: usize = logs.iter().map(|(_, e)| e.len()).sum();
            println!("[rr_test] RECORDED {} events / {} threads; arr={} entries; saved",
                total, n, arr.len());
        }
        rr::RrMode::Replay => {
            let log = rr::load_log("/tmp/rr_test.xfrr").unwrap();
            let want = std::fs::read("/tmp/rr_test.arr").unwrap();
            rr::RR_CURSOR.store(0, std::sync::atomic::Ordering::SeqCst);
            let handles: Vec<_> = (0..n as u64).map(|t| {
                let evs = log.iter().find(|(tid, _)| *tid == t)
                    .map(|(_, e)| e.as_slice()).unwrap_or(&[]);
                Some(rr::replay_thread(t, evs))
            }).collect();
            let (arr, _) = run(entry, n, handles);
            let ok = arr == want;
            println!("[rr_test] REPLAY: {} entries vs {} recorded — {}",
                arr.len(), want.len(),
                if ok { "IDENTICAL ✓ (interleaving reproduced)" } else { "DIVERGED ✗" });
            if !ok {
                println!("  first diff at {:?}",
                    arr.iter().zip(&want).position(|(a, b)| a != b));
                std::process::exit(1);
            }
        }
        rr::RrMode::Off => {
            let (a1, _) = run(entry, n, (0..n).map(|_| None).collect());
            let (a2, _) = run(entry, n, (0..n).map(|_| None).collect());
            println!("[rr_test] control: {} vs {} entries — {}",
                a1.len(), a2.len(),
                if a1 == a2 { "identical (need racier guest?)" }
                else { "DIFFER ✓ (genuinely racy — the thing rr must pin)" });
        }
    }
}
