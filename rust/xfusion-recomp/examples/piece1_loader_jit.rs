// piece1_loader_jit.rs — the FIRST loader↔JIT handshake (the loader's, Path-2 integration piece-1).
//
// Proves the guest-memory handoff: instead of the JIT harness's hand-setup (write `prog` bytes at a hand-picked
// entry), a MINIMAL PE is minted + "loaded" the way alky-loader does (parse PE headers → ImageBase +
// entry-RVA + section table → place .text at its VA in a guest_bytes buffer) → hand the JIT
// {mem_base=guest_bytes ptr, entry=ImageBase+entry_rva, rsp} → BlockCache runs it → verify rax=55
// (same as the JIT harness's --run-x64 sum10 hand-setup). If it matches, the loader→JIT memory-handoff works in
// isolation, and pieces 2 (NativeTable) + 3 (invalidate) build on it.
//
// This is the LOADER's minimal job for the JIT: {parse PE → place sections at VAs → mem_base+entry}.
// The full alky-loader does more (imports, relocs, TLS) — piece-1 is the memory-handoff core.
//
// run: cargo run --example piece1_loader_jit  (on the aarch64 box — the JIT is aarch64-host)

use sharpretro_jit::tier0::Tier0;
use sharpretro_jit::block_cache::{BlockCache, BlockCompiler, StopReason};
use sharpretro_jit::{Builder, IlType};
use xfusion_recomp::state::{X64_LAYOUT, STATE_WORDS_X64, OFF_RIP, OFF_MEMBASE};
use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::{decode_insn, DEF_MNEMONICS};
use xfusion_recomp::lift::lift_one;

// ── the guest program: the JIT harness's sum10 (eax = Σ1..10 = 55). Goes in the PE's .text. ──
const SUM10: &[u8] = &[
    0xB8, 0x00,0,0,0,       // mov eax, 0
    0xB9, 0x01,0,0,0,       // mov ecx, 1
    0xBA, 0x0B,0,0,0,       // mov edx, 11
    0x01, 0xC8,             // add eax, ecx   (loop:)
    0xFF, 0xC1,             // inc ecx
    0x39, 0xD1,             // cmp ecx, edx
    0x7C, 0xF8,             // jl loop (-8)
    0xCC,                   // int3
];

// ── mint a MINIMAL PE32+ with SUM10 in a single .text section at RVA 0x1000, ImageBase 0x140000000 ──
// This is the alky-pemint shape (kt2): DOS stub + PE sig + COFF header + optional header (PE32+) +
// one section header + the .text bytes. Just enough that a PE parser reads ImageBase/entry/sections.
fn mint_pe() -> Vec<u8> {
    let image_base: u64 = 0x1_4000_0000;
    let text_rva: u32 = 0x1000;
    let entry_rva: u32 = text_rva; // entry = start of .text
    let mut pe = vec![0u8; 0x400 + SUM10.len().max(0x200)];

    // DOS header: "MZ" + e_lfanew @0x3C → PE header at 0x80
    pe[0] = b'M'; pe[1] = b'Z';
    let pe_off: u32 = 0x80;
    pe[0x3C..0x40].copy_from_slice(&pe_off.to_le_bytes());

    let o = pe_off as usize;
    // PE signature "PE\0\0"
    pe[o..o+4].copy_from_slice(b"PE\0\0");
    // COFF header (20 bytes) @ o+4
    let coff = o + 4;
    pe[coff..coff+2].copy_from_slice(&0x8664u16.to_le_bytes());   // Machine = AMD64
    pe[coff+2..coff+4].copy_from_slice(&1u16.to_le_bytes());      // NumberOfSections = 1
    pe[coff+16..coff+18].copy_from_slice(&240u16.to_le_bytes());  // SizeOfOptionalHeader (PE32+ = 240)
    pe[coff+18..coff+20].copy_from_slice(&0x22u16.to_le_bytes()); // Characteristics (EXECUTABLE|LARGE_ADDRESS_AWARE)
    // Optional header (PE32+) @ coff+20
    let opt = coff + 20;
    pe[opt..opt+2].copy_from_slice(&0x20Bu16.to_le_bytes());      // Magic = PE32+
    pe[opt+16..opt+20].copy_from_slice(&entry_rva.to_le_bytes()); // AddressOfEntryPoint
    pe[opt+20..opt+24].copy_from_slice(&text_rva.to_le_bytes());  // BaseOfCode
    pe[opt+24..opt+32].copy_from_slice(&image_base.to_le_bytes());// ImageBase (PE32+ = 8 bytes @ +24)
    pe[opt+32..opt+36].copy_from_slice(&0x1000u32.to_le_bytes()); // SectionAlignment
    pe[opt+36..opt+40].copy_from_slice(&0x200u32.to_le_bytes());  // FileAlignment
    pe[opt+56..opt+60].copy_from_slice(&0x2000u32.to_le_bytes()); // SizeOfImage (headers + 1 page)
    pe[opt+60..opt+64].copy_from_slice(&0x400u32.to_le_bytes());  // SizeOfHeaders
    // Section header (40 bytes) @ opt+240
    let sec = opt + 240;
    pe[sec..sec+8].copy_from_slice(b".text\0\0\0");
    pe[sec+8..sec+12].copy_from_slice(&(SUM10.len() as u32).to_le_bytes()); // VirtualSize
    pe[sec+12..sec+16].copy_from_slice(&text_rva.to_le_bytes());  // VirtualAddress
    pe[sec+16..sec+20].copy_from_slice(&0x200u32.to_le_bytes());  // SizeOfRawData
    pe[sec+20..sec+24].copy_from_slice(&0x400u32.to_le_bytes());  // PointerToRawData (file offset)
    pe[sec+36..sec+40].copy_from_slice(&0x6000_0020u32.to_le_bytes()); // Characteristics (CODE|EXEC|READ)
    // .text raw bytes @ file offset 0x400
    pe[0x400..0x400+SUM10.len()].copy_from_slice(SUM10);
    pe
}

// ── the LOADER: parse the PE + place its sections into a guest address-space buffer at their VAs ──
// Returns (guest_bytes, entry_guest_va). This is alky-loader's memory-handoff core (minimal form).
struct Loaded { guest: Vec<u8>, entry: u64, image_base: u64 }
fn load_pe(pe: &[u8]) -> Loaded {
    let pe_off = u32::from_le_bytes(pe[0x3C..0x40].try_into().unwrap()) as usize;
    assert_eq!(&pe[pe_off..pe_off+4], b"PE\0\0", "not a PE");
    let coff = pe_off + 4;
    let n_sec = u16::from_le_bytes(pe[coff+2..coff+4].try_into().unwrap()) as usize;
    let opt = coff + 20;
    assert_eq!(u16::from_le_bytes(pe[opt..opt+2].try_into().unwrap()), 0x20B, "not PE32+");
    let entry_rva = u32::from_le_bytes(pe[opt+16..opt+20].try_into().unwrap()) as u64;
    let image_base = u64::from_le_bytes(pe[opt+24..opt+32].try_into().unwrap());
    let size_of_image = u32::from_le_bytes(pe[opt+56..opt+60].try_into().unwrap()) as usize;

    // guest address space = a buffer big enough for the whole image + stack/heap headroom.
    // We map the image at RVA-space (guest_addr = RVA); entry = image_base + entry_rva, but the JIT's
    // fetch does host_base + pc, so we need pc to be an offset into `guest`. Two conventions:
    //  - map at RVA 0 (guest_addr = RVA): entry = entry_rva, and mem_base = guest.as_ptr().
    //  - map at ImageBase (guest_addr = image_base + RVA): entry = image_base+entry_rva, guest buffer
    //    must be image_base+size_of_image big (huge). For piece-1 (a tiny PE), RVA-space is right +
    //    matches the JIT harness's convention (entry=0x10000 was an RVA-ish offset). We map at RVA-space.
    let guest_size = (size_of_image + 0x100000).max(0x200000); // image + 1MB stack/heap headroom
    let mut guest = vec![0u8; guest_size];

    // place each section's raw data at its VirtualAddress (RVA)
    let sec_tbl = opt + 240;
    for i in 0..n_sec {
        let s = sec_tbl + i*40;
        let vaddr = u32::from_le_bytes(pe[s+12..s+16].try_into().unwrap()) as usize;
        let raw_sz = u32::from_le_bytes(pe[s+16..s+20].try_into().unwrap()) as usize;
        let raw_ptr = u32::from_le_bytes(pe[s+20..s+24].try_into().unwrap()) as usize;
        let copy = raw_sz.min(pe.len() - raw_ptr);
        guest[vaddr..vaddr+copy].copy_from_slice(&pe[raw_ptr..raw_ptr+copy]);
    }
    // headers also mapped at RVA 0 (some code reads them); copy SizeOfHeaders
    let hdr_sz = u32::from_le_bytes(pe[opt+60..opt+64].try_into().unwrap()) as usize;
    guest[0..hdr_sz.min(pe.len())].copy_from_slice(&pe[0..hdr_sz.min(pe.len())]);

    Loaded { guest, entry: entry_rva, image_base } // entry in RVA-space
}

fn main() {
    println!("=== piece-1: loader↔JIT guest-memory handoff (sum10 via a minted+loaded PE) ===");
    let pe = mint_pe();
    println!("minted PE: {} bytes", pe.len());
    let mut loaded = load_pe(&pe);
    println!("loaded: entry(rva)=0x{:x} image_base=0x{:x} guest-space={} bytes",
             loaded.entry, loaded.image_base, loaded.guest.len());

    let host_base = loaded.guest.as_mut_ptr() as u64;

    struct X64Compiler { host_base: u64, max_block: usize }
    impl BlockCompiler for X64Compiler {
        fn fetch(&self, pc: u64) -> u32 {
            unsafe { ((self.host_base + pc) as *const u32).read_unaligned() }
        }
        fn is_stop(&self, first_word: u32) -> bool { (first_word & 0xFF) == 0xCC }
        fn compile_block(&self, t0: &mut Tier0, pc: u64, _mode: u32) -> (usize, StopReason) {
            let mut cur = pc;
            for n in 0..self.max_block {
                let bytes = unsafe { std::slice::from_raw_parts((self.host_base + cur) as *const u8, 15) };
                if bytes[0] == 0xCC {
                    let t = t0.literal(IlType::U64, cur as u128);
                    t0.branch(t, false);
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
            let t = t0.literal(IlType::U64, cur as u128);
            t0.branch(t, false);
            (self.max_block, StopReason::MaxInsns)
        }
    }

    let compiler = X64Compiler { host_base, max_block: 32 };
    let mut cache = BlockCache::with_layout(&X64_LAYOUT);
    let mut flat = [0u64; STATE_WORDS_X64];
    flat[OFF_RIP] = loaded.entry;      // entry in guest RVA-space (host_base + entry = the .text)
    flat[4] = 0x80000;                 // rsp (guest stack, inside the mapped headroom)
    flat[OFF_MEMBASE] = host_base;
    let result = cache.run(&compiler, &mut flat[..], 0, 10000);
    println!("[tier0 via LOADED PE: {} block-execs, {} compiles, rax=0x{:X} rip=0x{:X}, {:?}]",
             cache.n_execs, cache.n_compiles, flat[0], flat[OFF_RIP], result);
    let rax = flat[0];
    if rax == 55 {
        println!("✅ PIECE-1 PASS: sum10 through the LOADED PE = rax=55 (matches the JIT harness's hand-setup)");
        println!("   → the loader→JIT guest-memory handoff WORKS: a real PE, parsed+placed by the loader,");
        println!("     executed by the JIT via mem_base+entry. Pieces 2 (NativeTable) + 3 (invalidate) build on this.");
    } else {
        println!("❌ PIECE-1 FAIL: rax=0x{:X} (expected 55). Loader/handoff bug.", rax);
        std::process::exit(1);
    }
}
