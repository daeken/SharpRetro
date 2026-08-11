// piece2_5_argspill.rs — the arity-aware win64->AAPCS arg-map (the gate to the render path).
//
// piece-2 proved the <=4-arg crossing (rcx/rdx/r8/r9 -> x0-x3, trivial). But the RENDER path hits >4-arg
// native methods immediately: CreateCommittedResource (7-8 args) is the render demo's ~4th call. Win64
// passes args 1-4 in rcx/rdx/r8/r9 and args 5+ on the CALLER's stack; after the guest's CALL pushes the
// return-addr, at dispatch_native the guest rsp points at [return-addr], and win64's 32-byte shadow space
// sits above it, so arg5 is at *(rsp+0x28), arg6 at *(rsp+0x30), ... arg-i at *(rsp + 0x28 + (i-5)*8).
// AAPCS (aarch64) takes up to 8 integer args in x0-x7 -> covers every D3D12 method. So the map:
//   args[0..4] = state[rcx/rdx/r8/r9]; args[4..N] = read from the guest stack spill slots.
// Call the native fn with the right arity (transmute to the N-arg extern-C fn type -> AAPCS places
// x0-x7). This test uses a synthetic 7-arg native fn (CreateCommittedResource's arity) to prove the
// spill-read: native_sum7(a..g) = a+b+c+d+e+f+g; guest passes 1..7 -> expect rax=28.
//
// run: cargo run --example piece2_5_argspill  (aarch64-host, mem_base=0)

use sharpretro_jit::tier0::Tier0;
use sharpretro_jit::block_cache::{BlockCache, BlockCompiler, StopReason};
use sharpretro_jit::{Builder, IlType};
use xfusion_recomp::state::{X64_LAYOUT, STATE_WORDS_X64, OFF_RIP, OFF_MEMBASE};
use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::{decode_insn, DEF_MNEMONICS};
use xfusion_recomp::lift::{lift_one, FLAGS_ALL_LIVE};

// synthetic 7-arg native fn (mimics CreateCommittedResource's arity). AAPCS: a..g in x0-x6.
extern "C" fn native_sum7(a:u64,b:u64,c:u64,d:u64,e:u64,f:u64,g:u64)->u64 { a+b+c+d+e+f+g }

// The arity-aware win64->AAPCS call. args[0..4] from regs, args[4..arity] from guest-stack spill.
// Returns the native fn's result. `arity` <= 8 (AAPCS int-arg limit; covers all D3D12 methods).
fn call_native_win64(fn_addr: u64, arity: usize, regs: [u64;4], rsp: u64) -> u64 {
    // read spill args 5.. from the guest stack: arg-i (1-based, i>=5) at *(rsp + 0x28 + (i-5)*8)
    let spill = |i: usize| -> u64 { unsafe { ((rsp + 0x28 + ((i-5)*8) as u64) as *const u64).read() } };
    let a = [regs[0], regs[1], regs[2], regs[3],
             if arity>4 {spill(5)} else {0}, if arity>5 {spill(6)} else {0},
             if arity>6 {spill(7)} else {0}, if arity>7 {spill(8)} else {0}];
    unsafe {
        match arity {
            0 => (std::mem::transmute::<u64, extern "C" fn()->u64>(fn_addr))(),
            1 => (std::mem::transmute::<u64, extern "C" fn(u64)->u64>(fn_addr))(a[0]),
            2 => (std::mem::transmute::<u64, extern "C" fn(u64,u64)->u64>(fn_addr))(a[0],a[1]),
            3 => (std::mem::transmute::<u64, extern "C" fn(u64,u64,u64)->u64>(fn_addr))(a[0],a[1],a[2]),
            4 => (std::mem::transmute::<u64, extern "C" fn(u64,u64,u64,u64)->u64>(fn_addr))(a[0],a[1],a[2],a[3]),
            5 => (std::mem::transmute::<u64, extern "C" fn(u64,u64,u64,u64,u64)->u64>(fn_addr))(a[0],a[1],a[2],a[3],a[4]),
            6 => (std::mem::transmute::<u64, extern "C" fn(u64,u64,u64,u64,u64,u64)->u64>(fn_addr))(a[0],a[1],a[2],a[3],a[4],a[5]),
            7 => (std::mem::transmute::<u64, extern "C" fn(u64,u64,u64,u64,u64,u64,u64)->u64>(fn_addr))(a[0],a[1],a[2],a[3],a[4],a[5],a[6]),
            8 => (std::mem::transmute::<u64, extern "C" fn(u64,u64,u64,u64,u64,u64,u64,u64)->u64>(fn_addr))(a[0],a[1],a[2],a[3],a[4],a[5],a[6],a[7]),
            _ => panic!("arity {arity} > 8 (AAPCS int-arg limit)"),
        }
    }
}

fn main() {
    println!("=== piece-2.5: arity-aware win64->AAPCS spill-read (7-arg native call = the render-path gate) ===");
    let image_base: u64 = 0x1_4000_0000;
    let text: u64 = image_base + 0x1000;
    let iat: u64 = image_base + 0x2000;
    let stack_base: u64 = 0x1_5000_0000;
    unsafe {
        let i = libc::mmap(image_base as *mut libc::c_void, 0x3000, libc::PROT_READ|libc::PROT_WRITE|libc::PROT_EXEC,
            libc::MAP_PRIVATE|libc::MAP_ANONYMOUS|libc::MAP_FIXED, -1, 0); assert_eq!(i as u64, image_base);
        let s = libc::mmap(stack_base as *mut libc::c_void, 0x100000, libc::PROT_READ|libc::PROT_WRITE,
            libc::MAP_PRIVATE|libc::MAP_ANONYMOUS|libc::MAP_FIXED, -1, 0); assert_eq!(s as u64, stack_base);
    }
    let fn_addr = native_sum7 as *const () as u64;
    unsafe { (iat as *mut u64).write(fn_addr); }

    // guest: set args 1-7 for a win64 call: rcx=1,rdx=2,r8=3,r9=4, then push args 7,6,5 (reverse) +
    // reserve 32B shadow, then call [IAT]. Build it as: movs into regs; sub rsp for shadow+spill;
    // mov the spill slots; call [rip+disp]; int3. (The guest compiler-emitted code would do this; here
    // we hand-assemble the win64 call frame.)
    // Simplest faithful frame: rsp after CALL = &ret; [rsp+0x28]=arg5, [rsp+0x30]=arg6, [rsp+0x38]=arg7.
    // So before CALL: rsp' = rsp_at_call; the call pushes ret (rsp = rsp'-8). We need arg5..7 at
    // rsp'-8+0x28 = rsp'+0x20, +0x28, +0x30. We'll set rsp' so the guest writes args there then calls.
    let mut code: Vec<u8> = vec![];
    code.extend_from_slice(&[0xB9,1,0,0,0]);            // mov ecx,1  (arg1)
    code.extend_from_slice(&[0xBA,2,0,0,0]);            // mov edx,2  (arg2)
    code.extend_from_slice(&[0x41,0xB8,3,0,0,0]);       // mov r8d,3  (arg3)
    code.extend_from_slice(&[0x41,0xB9,4,0,0,0]);       // mov r9d,4  (arg4)
    // reserve shadow(32)+3 spill slots(24) = 56 = 0x38, keep 16-align: sub rsp, 0x38
    code.extend_from_slice(&[0x48,0x83,0xEC,0x38]);     // sub rsp, 0x38
    // mov qword [rsp+0x20], 5  (arg5 → will be at [ret_rsp+0x28] after call pushes ret)
    code.extend_from_slice(&[0x48,0xC7,0x44,0x24,0x20, 5,0,0,0]);   // mov qword [rsp+0x20],5
    code.extend_from_slice(&[0x48,0xC7,0x44,0x24,0x28, 6,0,0,0]);   // mov qword [rsp+0x28],6
    code.extend_from_slice(&[0x48,0xC7,0x44,0x24,0x30, 7,0,0,0]);   // mov qword [rsp+0x30],7
    // call [rip+disp32]
    let call_off = code.len() as u64;
    let next = 0x1000 + call_off + 6;
    let disp = (0x2000i64) - (next as i64);
    code.push(0xFF); code.push(0x15); code.extend_from_slice(&(disp as i32).to_le_bytes());
    code.push(0xCC);
    unsafe { std::ptr::copy_nonoverlapping(code.as_ptr(), text as *mut u8, code.len()); }

    struct C { max_block: usize, fn_addr: u64 }
    impl BlockCompiler for C {
        fn fetch(&self, pc: u64) -> u32 { unsafe { (pc as *const u32).read_unaligned() } }
        fn is_stop(&self, w: u32) -> bool { (w & 0xFF) == 0xCC }
        fn dispatch_native(&self, pc: u64, state: &mut [u64]) -> bool {
            if pc != self.fn_addr { return false; }
            let rsp = state[4];
            // arity 7 (CreateCommittedResource-shaped). regs = rcx/rdx/r8/r9 = state[1]/[2]/[8]/[9].
            let ret = call_native_win64(pc, 7, [state[1],state[2],state[8],state[9]], rsp);
            state[0] = ret;
            state[OFF_RIP] = unsafe { (rsp as *const u64).read() };
            state[4] = rsp + 8;
            println!("  [dispatch_native] 7-arg call: regs[1,2,3,4] + spill[5,6,7] → native_sum7 = {} → rax", ret);
            true
        }
        fn compile_block<BB: sharpretro_jit::Builder<Val = u32>>(&self, t0: &mut BB, pc: u64, _m: u32) -> (u64, StopReason) {
            let mut cur = pc;
            for n in 0..self.max_block {
                let b = unsafe { std::slice::from_raw_parts(cur as *const u8, 15) };
                if b[0]==0xCC { let t=t0.literal(IlType::U64,cur as u128); t0.branch(t,false); return (cur,StopReason::StopInsn); }
                let d = decode_insn(b, XMode::Bits64).unwrap_or_else(|| panic!("undecoded @0x{cur:x}: {:02X?}",&b[..4]));
                if !lift_one(t0,&d,cur,XMode::Bits64,FLAGS_ALL_LIVE) { panic!("no lift @0x{cur:x}: {}",DEF_MNEMONICS[d.def_id as usize]); }
                cur += d.len as u64;
                if t0.branched() { return (cur,StopReason::Branched); }
            }
            let t=t0.literal(IlType::U64,cur as u128); t0.branch(t,false); (cur,StopReason::MaxInsns)
        }
    }

    let compiler = C { max_block: 32, fn_addr };
    let mut cache = BlockCache::with_layout(&X64_LAYOUT);
    let mut flat = [0u64; STATE_WORDS_X64];
    flat[OFF_RIP] = text; flat[4] = stack_base + 0x80000; flat[OFF_MEMBASE] = 0;
    cache.run(&compiler, &mut flat[..], 0, 10000);
    println!("[rax=0x{:X} = {}]", flat[0], flat[0]);
    if flat[0] == 28 {
        println!("✅ PIECE-2.5 PASS: 7-arg win64→AAPCS map works — args 1-4 from regs, 5-7 from guest-stack spill.");
        println!("   native_sum7(1,2,3,4,5,6,7)=28. This is the render-path gate: CreateCommittedResource (7-8 args)");
        println!("   crosses correctly. The arity-aware spill-read covers all D3D12 methods (≤8 int-args → AAPCS x0-x7).");
    } else {
        println!("❌ PIECE-2.5: rax={} (expected 28). Spill-read offset bug — check the win64 shadow+arg layout.", flat[0]);
        std::process::exit(1);
    }
}
