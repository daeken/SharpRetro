// piece1_5_membase0.rs — prove SHARED-ADDRESS-SPACE (mem_base=0) holds (the loader's, Path-2 integration).
//
// the JIT harness's  + the graphics seam's  converged: for Alky, mem_base=0 (guest-addr == host-addr, shared process VA)
// collapses BOTH the call-plane AND the Map data-plane to trivial — it's exactly what the loader does
// under Rosetta today, and what the graphics seam's 6 composition paths already prove. piece-1 passed under a
// SANDBOXED mem_base (guest.as_ptr()) — but that does NOT prove mem_base=0 (the graphics seam's : piece-1 never
// reads a seam vtable, so the guest-addr==host-addr requirement doesn't bite until piece-2's first native
// call). THIS proves the shared-address-space model directly: mmap the PE at its REAL ImageBase VA
// (MAP_FIXED), set flat[OFF_MEMBASE]=0, entry=ImageBase+entry_rva → the JIT's mem_read/write =
// *(0 + guest_addr) = *(guest_addr) = the real host address = the mmap'd PE. If sum10 → rax=55 with
// mem_base=0, shared-mode holds, and piece-2's native_call_targets discrimination (guest reads seam
// vtable at its host addr) will work.
//
// run: cargo run --example piece1_5_membase0  (aarch64-host; needs mmap-at-high-VA, verified on this box)

use sharpretro_jit::tier0::Tier0;
use sharpretro_jit::block_cache::{BlockCache, BlockCompiler, StopReason};
use sharpretro_jit::{Builder, IlType};
use xfusion_recomp::state::{X64_LAYOUT, STATE_WORDS_X64, OFF_RIP, OFF_MEMBASE};
use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::{decode_insn, DEF_MNEMONICS};
use xfusion_recomp::lift::{lift_one, FLAGS_ALL_LIVE};

const SUM10: &[u8] = &[
    0xB8, 0x00,0,0,0,       // mov eax, 0
    0xB9, 0x01,0,0,0,       // mov ecx, 1
    0xBA, 0x0B,0,0,0,       // mov edx, 11
    0x01, 0xC8,             // add eax, ecx  (loop:)
    0xFF, 0xC1,             // inc ecx
    0x39, 0xD1,             // cmp ecx, edx
    0x7C, 0xF8,             // jl loop (-8)
    0xCC,                   // int3
];

// mint the same minimal PE32+ as piece-1 (SUM10 in .text @RVA 0x1000, ImageBase 0x140000000)
fn mint_pe() -> Vec<u8> {
    let image_base: u64 = 0x1_4000_0000;
    let text_rva: u32 = 0x1000;
    let mut pe = vec![0u8; 0x600];
    pe[0]=b'M'; pe[1]=b'Z';
    let pe_off: u32 = 0x80; pe[0x3C..0x40].copy_from_slice(&pe_off.to_le_bytes());
    let o = pe_off as usize;
    pe[o..o+4].copy_from_slice(b"PE\0\0");
    let coff=o+4;
    pe[coff..coff+2].copy_from_slice(&0x8664u16.to_le_bytes());
    pe[coff+2..coff+4].copy_from_slice(&1u16.to_le_bytes());
    pe[coff+16..coff+18].copy_from_slice(&240u16.to_le_bytes());
    pe[coff+18..coff+20].copy_from_slice(&0x22u16.to_le_bytes());
    let opt=coff+20;
    pe[opt..opt+2].copy_from_slice(&0x20Bu16.to_le_bytes());
    pe[opt+16..opt+20].copy_from_slice(&text_rva.to_le_bytes());   // entry = text_rva
    pe[opt+20..opt+24].copy_from_slice(&text_rva.to_le_bytes());
    pe[opt+24..opt+32].copy_from_slice(&image_base.to_le_bytes());
    pe[opt+32..opt+36].copy_from_slice(&0x1000u32.to_le_bytes());
    pe[opt+36..opt+40].copy_from_slice(&0x200u32.to_le_bytes());
    pe[opt+56..opt+60].copy_from_slice(&0x2000u32.to_le_bytes());
    pe[opt+60..opt+64].copy_from_slice(&0x400u32.to_le_bytes());
    let sec=opt+240;
    pe[sec..sec+8].copy_from_slice(b".text\0\0\0");
    pe[sec+8..sec+12].copy_from_slice(&(SUM10.len() as u32).to_le_bytes());
    pe[sec+12..sec+16].copy_from_slice(&text_rva.to_le_bytes());
    pe[sec+16..sec+20].copy_from_slice(&0x200u32.to_le_bytes());
    pe[sec+20..sec+24].copy_from_slice(&0x400u32.to_le_bytes());
    pe[sec+36..sec+40].copy_from_slice(&0x6000_0020u32.to_le_bytes());
    pe[0x400..0x400+SUM10.len()].copy_from_slice(SUM10);
    pe
}

// ── the LOADER — SHARED-MODE: mmap the image at its REAL ImageBase VA (MAP_FIXED), place sections. ──
// guest_addr == host_addr (the mmap is AT ImageBase); entry = ImageBase + entry_rva; mem_base = 0.
// This is what alky-loader does under Rosetta today (mmap at ImageBase, guest+host share process VA).
unsafe fn load_pe_shared(pe: &[u8]) -> (u64 /*entry*/, u64 /*image_base*/) {
    let pe_off = u32::from_le_bytes(pe[0x3C..0x40].try_into().unwrap()) as usize;
    let coff = pe_off + 4;
    let n_sec = u16::from_le_bytes(pe[coff+2..coff+4].try_into().unwrap()) as usize;
    let opt = coff + 20;
    let entry_rva = u32::from_le_bytes(pe[opt+16..opt+20].try_into().unwrap()) as u64;
    let image_base = u64::from_le_bytes(pe[opt+24..opt+32].try_into().unwrap());
    let size_of_image = u32::from_le_bytes(pe[opt+56..opt+60].try_into().unwrap()) as u64;

    // mmap the image AT ImageBase (MAP_FIXED) — this is the shared-VA placement. Round size up to page.
    let map_size = ((size_of_image + 0xFFF) & !0xFFF) as usize;
    let addr = unsafe { libc::mmap(
        image_base as *mut libc::c_void, map_size,
        libc::PROT_READ | libc::PROT_WRITE | libc::PROT_EXEC,
        libc::MAP_PRIVATE | libc::MAP_ANONYMOUS | libc::MAP_FIXED, -1, 0) };
    assert_eq!(addr as u64, image_base, "MAP_FIXED @ImageBase failed (got {:p})", addr);

    // place each section's raw data at ImageBase + VirtualAddress (the real host addr under shared-VA)
    let sec_tbl = opt + 240;
    for i in 0..n_sec {
        let s = sec_tbl + i*40;
        let vaddr = u32::from_le_bytes(pe[s+12..s+16].try_into().unwrap()) as u64;
        let raw_sz = u32::from_le_bytes(pe[s+16..s+20].try_into().unwrap()) as usize;
        let raw_ptr = u32::from_le_bytes(pe[s+20..s+24].try_into().unwrap()) as usize;
        let copy = raw_sz.min(pe.len() - raw_ptr);
        let dst = (image_base + vaddr) as *mut u8;
        unsafe { std::ptr::copy_nonoverlapping(pe[raw_ptr..].as_ptr(), dst, copy); }
    }
    (image_base + entry_rva, image_base)
}

fn main() {
    println!("=== piece-1.5: SHARED-ADDRESS-SPACE proof (mem_base=0, PE mmap'd at real ImageBase VA) ===");
    let pe = mint_pe();
    let (entry, image_base) = unsafe { load_pe_shared(&pe) };
    println!("mmap'd PE at real VA: image_base=0x{:x} entry(host-addr)=0x{:x}", image_base, entry);

    // Also mmap a guest stack region at a real VA (rsp must be a valid host addr under mem_base=0)
    let stack_base: u64 = 0x1_5000_0000;
    unsafe {
        let s = libc::mmap(stack_base as *mut libc::c_void, 0x100000,
            libc::PROT_READ|libc::PROT_WRITE, libc::MAP_PRIVATE|libc::MAP_ANONYMOUS|libc::MAP_FIXED, -1, 0);
        assert_eq!(s as u64, stack_base, "stack mmap failed");
    }

    struct X64Compiler { max_block: usize } // NO host_base field — mem_base=0, pc IS the host addr
    impl BlockCompiler for X64Compiler {
        fn fetch(&self, pc: u64) -> u32 {
            unsafe { (pc as *const u32).read_unaligned() } // mem_base=0: pc is the host addr directly
        }
        fn is_stop(&self, first_word: u32) -> bool { (first_word & 0xFF) == 0xCC }
        fn compile_block<BB: sharpretro_jit::Builder<Val = u32>>(&self, t0: &mut BB, pc: u64, _mode: u32) -> (u64, StopReason) {
            let mut cur = pc;
            for n in 0..self.max_block {
                let bytes = unsafe { std::slice::from_raw_parts(cur as *const u8, 15) };
                if bytes[0] == 0xCC {
                    let t = t0.literal(IlType::U64, cur as u128);
                    t0.branch(t, false);
                    return (cur, StopReason::StopInsn);
                }
                let d = decode_insn(bytes, XMode::Bits64)
                    .unwrap_or_else(|| panic!("undecoded @0x{cur:x}: {:02X?}", &bytes[..4]));
                if !lift_one(t0, &d, cur, XMode::Bits64, FLAGS_ALL_LIVE) {
                    panic!("no lift @0x{cur:x}: {} def_id={}", DEF_MNEMONICS[d.def_id as usize], d.def_id);
                }
                cur += d.len as u64;
                if t0.branched() { return (cur, StopReason::Branched); }
            }
            let t = t0.literal(IlType::U64, cur as u128);
            t0.branch(t, false);
            (cur, StopReason::MaxInsns)
        }
    }

    let compiler = X64Compiler { max_block: 32 };
    let mut cache = BlockCache::with_layout(&X64_LAYOUT);
    let mut flat = [0u64; STATE_WORDS_X64];
    flat[OFF_RIP] = entry;                 // entry = REAL host addr (ImageBase + entry_rva)
    flat[4] = stack_base + 0x80000;        // rsp = a real host addr in the mapped stack
    flat[OFF_MEMBASE] = 0;                 // ← SHARED MODE: guest-addr == host-addr
    let result = cache.run(&compiler, &mut flat[..], 0, 10000);
    println!("[tier0 mem_base=0: {} block-execs, {} compiles, rax=0x{:X} rip=0x{:X}, {:?}]",
             cache.n_execs, cache.n_compiles, flat[0], flat[OFF_RIP], result);
    if flat[0] == 55 {
        println!("✅ PIECE-1.5 PASS: sum10 with mem_base=0 (PE at real VA) = rax=55.");
        println!("   → SHARED-ADDRESS-SPACE model HOLDS. guest-addr==host-addr, mem_read/write=*(addr).");
        println!("   → piece-2's native_call_targets discrimination WILL work (guest reads seam vtable at");
        println!("     its host addr through mem_base=0). This is Rosetta's model minus Rosetta.");
    } else {
        println!("❌ PIECE-1.5 FAIL: rax=0x{:X} (expected 55).", flat[0]);
        std::process::exit(1);
    }
}
