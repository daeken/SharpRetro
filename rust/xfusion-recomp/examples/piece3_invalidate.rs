// piece3_invalidate.rs — the invalidate(range) contract, end-to-end (Path-2 integration piece-3).
//
// The loader-JIT contract's 3rd piece (per DESIGN.md §invalidate): when guest code changes after it's
// been compiled+cached, the stale CompiledBlock must be dropped so the next execution re-compiles the
// new bytes. The 3-time ledger of callers:
//   (1) bulk-map: rides free — the whole image is placed BEFORE any block compiles, so no cached block
//       is ever stale w.r.t. the initial map. No invalidate call needed.
//   (2) runtime-patch: the loader patches guest memory after execution began (a hot-patch, a relocation
//       applied late, a self-modifying trampoline the loader writes) → the loader calls invalidate(range)
//       explicitly. Enumerable, a handful of sites.
//   (3) guest-SMC: the guest itself writes to code (JIT'd game code, packers) → the loader write-protects
//       the code pages, the fault handler calls invalidate(range) for the touched page. (Not demoed here —
//       needs the fault-handler wiring; this test covers (2), the explicit-caller case, which is the
//       mechanism (3) reuses.)
//
// This test: map a program, execute it (compiles+caches a block), PATCH one instruction (a runtime-patch),
// invalidate the patched range, re-execute → verify the block re-compiled with the NEW instruction (the
// result changes). Without invalidate, the stale cached block would run the OLD bytes → wrong result.
// That's the contract: invalidate(range) drops every cached block whose guest_range intersects.
//
// run: cargo run --example piece3_invalidate  (aarch64-host, mem_base=0)

use sharpretro_jit::tier0::Tier0;
use sharpretro_jit::block_cache::{BlockCache, BlockCompiler, StopReason};
use sharpretro_jit::{Builder, IlType};
use xfusion_recomp::state::{X64_LAYOUT, STATE_WORDS_X64, OFF_RIP, OFF_MEMBASE};
use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::{decode_insn, DEF_MNEMONICS};
use xfusion_recomp::lift::lift_one;

struct X64Compiler { max_block: usize }
impl BlockCompiler for X64Compiler {
    fn fetch(&self, pc: u64) -> u32 { unsafe { (pc as *const u32).read_unaligned() } }
    fn is_stop(&self, w: u32) -> bool { (w & 0xFF) == 0xCC }
    fn compile_block(&self, t0: &mut Tier0, pc: u64, _mode: u32) -> (usize, StopReason) {
        let mut cur = pc;
        for n in 0..self.max_block {
            let bytes = unsafe { std::slice::from_raw_parts(cur as *const u8, 15) };
            if bytes[0] == 0xCC {
                let t = t0.literal(IlType::U64, cur as u128); t0.branch(t, false);
                return (n, StopReason::StopInsn);
            }
            let d = decode_insn(bytes, XMode::Bits64)
                .unwrap_or_else(|| panic!("undecoded @0x{cur:x}"));
            if !lift_one(t0, &d, cur, XMode::Bits64) { panic!("no lift @0x{cur:x}: {}", DEF_MNEMONICS[d.def_id as usize]); }
            cur += d.len as u64;
            if t0.branched() { return (n + 1, StopReason::Branched); }
        }
        let t = t0.literal(IlType::U64, cur as u128); t0.branch(t, false);
        (self.max_block, StopReason::MaxInsns)
    }
}

fn run_once(cache: &mut BlockCache, compiler: &X64Compiler, entry: u64, rsp: u64) -> u64 {
    let mut flat = [0u64; STATE_WORDS_X64];
    flat[OFF_RIP] = entry; flat[4] = rsp; flat[OFF_MEMBASE] = 0;
    cache.run(compiler, &mut flat[..], 0, 10000);
    flat[0] // rax
}

fn main() {
    println!("=== piece-3: invalidate(range) contract — runtime-patch drops the stale cached block ===");
    let image_base: u64 = 0x1_4000_0000;
    let text: u64 = image_base + 0x1000;
    let stack_base: u64 = 0x1_5000_0000;
    unsafe {
        let i = libc::mmap(image_base as *mut libc::c_void, 0x2000, libc::PROT_READ|libc::PROT_WRITE|libc::PROT_EXEC,
            libc::MAP_PRIVATE|libc::MAP_ANONYMOUS|libc::MAP_FIXED, -1, 0); assert_eq!(i as u64, image_base);
        let s = libc::mmap(stack_base as *mut libc::c_void, 0x100000, libc::PROT_READ|libc::PROT_WRITE,
            libc::MAP_PRIVATE|libc::MAP_ANONYMOUS|libc::MAP_FIXED, -1, 0); assert_eq!(s as u64, stack_base);
    }

    // program: mov eax, 10 ; int3   → rax = 10.  The imm32 (0x0A) is at text+1.
    let prog: &[u8] = &[0xB8, 0x0A,0,0,0, 0xCC];
    unsafe { std::ptr::copy_nonoverlapping(prog.as_ptr(), text as *mut u8, prog.len()); }

    let compiler = X64Compiler { max_block: 32 };
    let mut cache = BlockCache::with_layout(&X64_LAYOUT);
    let rsp = stack_base + 0x80000;

    // (1) first run — compiles + caches the block (bulk-map rode free, no invalidate needed)
    let r1 = run_once(&mut cache, &compiler, text, rsp);
    println!("run 1: rax={} (compiles={}, execs={})  [expect 10]", r1, cache.n_compiles, cache.n_execs);

    // (2) RUNTIME-PATCH: the loader hot-patches the imm32 10 → 99 (a late relocation / hot-patch).
    // The cached block still holds the OLD bytes' compilation.
    unsafe { ((text + 1) as *mut u32).write(99); }
    println!("runtime-patch: imm32 at 0x{:x} : 10 -> 99", text + 1);

    // run WITHOUT invalidate — the stale cached block runs the OLD bytes (rax=10, the bug the contract prevents)
    let r_stale = run_once(&mut cache, &compiler, text, rsp);
    println!("run 2 (NO invalidate): rax={} (compiles={})  [stale cache → still 10, the bug]", r_stale, cache.n_compiles);

    // (3) the CONTRACT: the loader's runtime-patch caller invokes invalidate(range) for the patched range.
    let dropped = cache.invalidate(text, text + prog.len() as u64);
    println!("invalidate(0x{:x}, 0x{:x}) → dropped {} cached block(s)", text, text + prog.len() as u64, dropped);

    // run AFTER invalidate — re-compiles the NEW bytes → rax=99
    let r3 = run_once(&mut cache, &compiler, text, rsp);
    println!("run 3 (after invalidate): rax={} (compiles={})  [expect 99 = re-compiled new bytes]", r3, cache.n_compiles);

    if r1 == 10 && r_stale == 10 && dropped >= 1 && r3 == 99 {
        println!("✅ PIECE-3 PASS: invalidate(range) contract works.");
        println!("   run1=10 (compiled) → patch imm→99 → run2=10 (STALE cache proves the hazard is real) →");
        println!("   invalidate dropped {} block → run3=99 (re-compiled new bytes). The 3-time invalidate-ledger's", dropped);
        println!("   caller-(2) [runtime-patch → explicit invalidate] demonstrated; (1) bulk-map rode free; (3) SMC reuses this.");
    } else {
        println!("❌ PIECE-3: r1={} r_stale={} dropped={} r3={} (want 10/10/≥1/99)", r1, r_stale, dropped, r3);
        std::process::exit(1);
    }
}
