// piece2_native_call.rs — FIRST guest→native crossing through the loader↔JIT boundary (Path-2 core).
//
// A minted PE whose .text does: mov ecx,7; mov edx,35; call [__imp_native_add]; int3.
// The IAT slot holds the host address of a native aarch64 fn `native_add(a,b,_,_) -> a+b`.
// Under mem_base=0 (shared address space, proven in the piece-1.5 test), the guest's
// `call [rip+disp]` reads the IAT slot = the native fn's real host addr → block ends with
// OFF_RIP = native_add's addr → the driver's dispatch_native fires: pc ∈ native_targets →
// call native_add(rcx, rdx, r8, r9) [the win64→AAPCS map: a normal aarch64 fn call with the
// guest's arg-registers] → rax = result → pop return-addr → guest continues → int3 → stop.
// Verify rax = 7+35 = 42. That proves the guest→native call crossing end-to-end.
//
// win64→AAPCS map (v1): the native shim is a normal aarch64 `extern "C" fn(u64,u64,u64,u64)->u64`
// (AAPCS: args in x0-x3, ret x0). The guest's win64 arg-regs are rcx(state[1])/rdx(state[2])/
// r8(state[8])/r9(state[9]); calling the aarch64 fn with those IS the ABI-map (no hand-asm for
// the 4-int-arg case — Rust's call places them in x0-x3). Return → rax(state[0]).
//
// run: cargo run --example piece2_native_call  (aarch64-host; mem_base=0 shared-VA)

use sharpretro_jit::tier0::Tier0;
use sharpretro_jit::block_cache::{BlockCache, BlockCompiler, StopReason};
use sharpretro_jit::{Builder, IlType};
use xfusion_recomp::state::{X64_LAYOUT, STATE_WORDS_X64, OFF_RIP, OFF_MEMBASE};
use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::{decode_insn, DEF_MNEMONICS};
use xfusion_recomp::lift::lift_one;

// ── the native shim (the "library code" the guest calls). Normal aarch64 fn, AAPCS x0-x3→x0. ──
extern "C" fn native_add(a: u64, b: u64, _c: u64, _d: u64) -> u64 { a + b }

fn main() {
    println!("=== piece-2: FIRST guest→native crossing (call [IAT] → native_add(7,35) → rax=42) ===");

    // ── layout (mem_base=0, real host VAs, MAP_FIXED) ──
    let image_base: u64 = 0x1_4000_0000;
    let text_rva: u64 = 0x1000;
    let iat_rva: u64 = 0x2000;                 // IAT slot on its own page
    let stack_base: u64 = 0x1_5000_0000;

    unsafe {
        let img = libc::mmap(image_base as *mut libc::c_void, 0x3000,
            libc::PROT_READ|libc::PROT_WRITE|libc::PROT_EXEC,
            libc::MAP_PRIVATE|libc::MAP_ANONYMOUS|libc::MAP_FIXED, -1, 0);
        assert_eq!(img as u64, image_base, "img mmap");
        let stk = libc::mmap(stack_base as *mut libc::c_void, 0x100000,
            libc::PROT_READ|libc::PROT_WRITE, libc::MAP_PRIVATE|libc::MAP_ANONYMOUS|libc::MAP_FIXED, -1, 0);
        assert_eq!(stk as u64, stack_base, "stk mmap");
    }

    // ── the IAT slot holds the native fn's host address (this is what the loader's import-resolver
    //    does under mem_base=0: patch IAT[slot] = shim host-addr). native_targets = {that addr}. ──
    let native_fn_addr = native_add as usize as u64;
    unsafe { ((image_base + iat_rva) as *mut u64).write(native_fn_addr); }

    // ── the guest .text: mov ecx,7 ; mov edx,35 ; call [rip+disp to IAT] ; int3 ──
    // call [rip+disp32] = FF 15 <disp32>; disp32 = IAT_addr - (addr_of_next_insn).
    let text_addr = image_base + text_rva;
    let mut code: Vec<u8> = vec![];
    code.extend_from_slice(&[0xB9, 7,0,0,0]);          // mov ecx, 7
    code.extend_from_slice(&[0xBA, 35,0,0,0]);         // mov edx, 35
    // call [rip+disp32] — 6 bytes (FF 15 dd dd dd dd); rip after this insn = text+10+6=text+16
    let call_insn_off = code.len() as u64;             // = 10
    let next_after_call = text_rva + call_insn_off + 6;
    let disp = (iat_rva as i64) - (next_after_call as i64);
    code.push(0xFF); code.push(0x15);
    code.extend_from_slice(&(disp as i32).to_le_bytes());
    code.push(0xCC);                                    // int3
    unsafe { std::ptr::copy_nonoverlapping(code.as_ptr(), text_addr as *mut u8, code.len()); }

    println!("guest .text @0x{:x}: mov ecx,7; mov edx,35; call [0x{:x}]; int3", text_addr, image_base+iat_rva);
    println!("IAT[0x{:x}] = native_add @0x{:x}", image_base+iat_rva, native_fn_addr);

    // ── the loader-side compiler: dispatch_native does the crossing ──
    struct X64Compiler { max_block: usize, native_targets: Vec<u64> }
    impl BlockCompiler for X64Compiler {
        fn fetch(&self, pc: u64) -> u32 { unsafe { (pc as *const u32).read_unaligned() } }
        fn is_stop(&self, w: u32) -> bool { (w & 0xFF) == 0xCC }
        fn dispatch_native(&self, pc: u64, state: &mut [u64]) -> bool {
            if self.native_targets.binary_search(&pc).is_err() { return false; }
            // win64→AAPCS: guest rcx/rdx/r8/r9 = state[1]/[2]/[8]/[9] → the aarch64 fn's x0-x3.
            let f: extern "C" fn(u64,u64,u64,u64)->u64 = unsafe { std::mem::transmute(pc) };
            let ret = f(state[1], state[2], state[8], state[9]);
            state[0] = ret;                              // rax ← return value
            // pop the return-addr the guest's CALL pushed: rip = *(rsp); rsp += 8.
            let rsp = state[4];
            state[OFF_RIP] = unsafe { (rsp as *const u64).read() };
            state[4] = rsp + 8;
            println!("  [dispatch_native] pc=0x{:x} in native set → native_add({},{})={} → rax; ret to 0x{:x} rsp 0x{:x}->0x{:x}",
                     pc, state[1], state[2], ret, state[OFF_RIP], rsp, state[4]);
            true
        }
        fn compile_block(&self, t0: &mut Tier0, pc: u64, _mode: u32) -> (usize, StopReason) {
            let mut cur = pc;
            for n in 0..self.max_block {
                let bytes = unsafe { std::slice::from_raw_parts(cur as *const u8, 15) };
                if bytes[0] == 0xCC {
                    let t = t0.literal(IlType::U64, cur as u128); t0.branch(t, false);
                    return (n, StopReason::StopInsn);
                }
                let d = decode_insn(bytes, XMode::Bits64)
                    .unwrap_or_else(|| panic!("undecoded @0x{cur:x}: {:02X?}", &bytes[..4]));
                if !lift_one(t0, &d, cur, XMode::Bits64) {
                    panic!("no lift @0x{cur:x}: {} def_id={}", DEF_MNEMONICS[d.def_id as usize], d.def_id);
                }
                cur += d.len as u64;
                if t0.branched() { return (n + 1, StopReason::Branched); }
            }
            let t = t0.literal(IlType::U64, cur as u128); t0.branch(t, false);
            (self.max_block, StopReason::MaxInsns)
        }
    }

    let compiler = X64Compiler { max_block: 32, native_targets: vec![native_fn_addr] };
    let mut cache = BlockCache::with_layout(&X64_LAYOUT);
    let mut flat = [0u64; STATE_WORDS_X64];
    flat[OFF_RIP] = text_addr;
    flat[4] = stack_base + 0x80000;   // rsp
    flat[OFF_MEMBASE] = 0;            // shared mode
    let result = cache.run(&compiler, &mut flat[..], 0, 10000);
    println!("[result: {} execs, {} compiles, rax=0x{:X} rip=0x{:X}, {:?}]",
             cache.n_execs, cache.n_compiles, flat[0], flat[OFF_RIP], result);
    if flat[0] == 42 {
        println!("✅ PIECE-2 PASS: guest called native_add via [IAT], got rax=42 back, returned to guest.");
        println!("   → the guest→native crossing WORKS end-to-end. This is Path-2's core: emulated guest");
        println!("     code calls native library code (the win64→AAPCS ABI-map + return). NativeTable-from-IAT.");
    } else {
        println!("❌ PIECE-2: rax=0x{:X} (expected 42). Crossing or CALL-stack issue — see dispatch log.", flat[0]);
        std::process::exit(1);
    }
}
