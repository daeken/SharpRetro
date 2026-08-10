//! isa_diff — the rr-oracle diff harness (interp-side). Reads a per-insn
//! ground-truth trace from rr_trace.py (silicon: pc + 16 GPRs + eflags,
//! state BEFORE executing pc) and, for each consecutive pair (N, N+1):
//! seeds a fresh interp from line N's state, executes ONE insn at N.pc,
//! compares result to line N+1. First mismatch = the .isa bug.
//!
//! STATELESS (·1079): the interp never accumulates state across insns —
//! each check re-seeds from silicon. So shim-return-values, host-pointers,
//! stack-base differences don't propagate; only the SEMANTICS of one insn
//! are under test.
//!
//! v1: reads insn bytes from the local PE (via rva→file-offset). Insns that
//! touch memory (load/store operands, push/pop, call/ret) are MEM-SKIP —
//! the interp can't know what silicon's memory held. Re-seed from N+1
//! regardless, so skips don't accumulate error. Own #117 (cmp reg,reg; jle)
//! is pure-GPR → caught by v1. v2 = fuchi adds `mem:{addr:val}` inline.
//!
//! usage: isa_diff <trace-file> [<pe-path>=/tmp/cp2077/Cyberpunk2077.exe]
//!        [--image-base 0x140000000] [--max N]

use xfusion_recomp::decode::XMode;
use xfusion_recomp::disassembler::{decode_insn, DEF_MNEMONICS};
use xfusion_recomp::lift::{lift_one, FLAGS_ALL_LIVE};
use xfusion_recomp::state::X86State;
use sharpretro_jit::interp::{InterpretingBuilder, GuestMem};
use std::io::{BufRead, BufReader};

const IMAGE_BASE: u64 = 0x1_4000_0000;
// SizeOfImage upper bound (CP2077 ~0x400_0000 code+data). Any branch target
// outside [ImageBase, ImageBase+IMAGE_SIZE) = a native shim/thunk address.
const IMAGE_SIZE: u64 = 0x1_0000_0000;   // generous; anything guest is < this.

/// A GuestMem that FLAGS any access (v1: mem-touching insns → skip).
struct FlagMem { touched: std::cell::Cell<bool> }
impl GuestMem for FlagMem {
    fn read(&self, _addr: u64, _w: u8) -> u128 { self.touched.set(true); 0 }
    fn write(&mut self, _addr: u64, _w: u8, _v: u128) { self.touched.set(true); }
}

#[derive(Clone, Debug)]
struct TraceLine {
    seq: u64,
    pc_rva: u64,
    gpr: [u64; 16],
    eflags: u32,
    bytes: Vec<u8>,   // raw insn bytes at pc (self-contained, per ·1080)
}

fn parse_line(s: &str) -> Option<TraceLine> {
    // {seq}\t{pc_rva:x}\t{rax rbx rcx rdx rsi rdi rsp rbp r8..r15 hex space-sep}\t{eflags:x}
    let mut parts = s.split('\t');
    let seq = parts.next()?.trim().parse().ok()?;
    let pc_rva = u64::from_str_radix(parts.next()?.trim(), 16).ok()?;
    let gprs_s = parts.next()?;
    let eflags = u32::from_str_radix(parts.next()?.trim(), 16).ok()?;
    // trace r-order: rax rbx rcx rdx rsi rdi rsp rbp r8..r15 (·1078).
    // x64 encoding-order idx: rax=0 rcx=1 rdx=2 rbx=3 rsp=4 rbp=5 rsi=6 rdi=7 r8..=8..
    // gpr[enc_idx] = vals[trace_pos]:
    //   gpr[0]rax←vals[0]  gpr[1]rcx←vals[2]  gpr[2]rdx←vals[3]  gpr[3]rbx←vals[1]
    //   gpr[4]rsp←vals[6]  gpr[5]rbp←vals[7]  gpr[6]rsi←vals[4]  gpr[7]rdi←vals[5]
    let vals: Vec<u64> = gprs_s.split_whitespace()
        .map(|v| u64::from_str_radix(v, 16).unwrap_or(0)).collect();
    if vals.len() != 16 { return None; }
    let from_trace = [0usize, 2, 3, 1, 6, 7, 4, 5, 8, 9, 10, 11, 12, 13, 14, 15];
    let mut gpr = [0u64; 16];
    for i in 0..16 { gpr[i] = vals[from_trace[i]]; }
    // 5th field: raw insn bytes hex-concat (up to 15). Optional (v0 traces lack it).
    let bytes: Vec<u8> = parts.next().map(|bs| {
        let bs = bs.trim();
        (0..bs.len()/2).filter_map(|k| u8::from_str_radix(&bs[2*k..2*k+2], 16).ok()).collect()
    }).unwrap_or_default();
    Some(TraceLine { seq, pc_rva, gpr, eflags, bytes })
}

/// Minimal PE .text reader: rva → &bytes[..].
struct PeText { data: Vec<u8>, sec_va: u32, sec_ptr: u32, sec_end: u32 }
impl PeText {
    fn open(path: &str) -> Self {
        let d = std::fs::read(path).expect("read PE");
        let pe = u32::from_le_bytes(d[0x3C..0x40].try_into().unwrap()) as usize;
        assert_eq!(&d[pe..pe+4], b"PE\0\0");
        let optsz = u16::from_le_bytes(d[pe+20..pe+22].try_into().unwrap()) as usize;
        let s = pe + 24 + optsz;   // first section = .text
        let vsz = u32::from_le_bytes(d[s+8..s+12].try_into().unwrap());
        let va  = u32::from_le_bytes(d[s+12..s+16].try_into().unwrap());
        let rsz = u32::from_le_bytes(d[s+16..s+20].try_into().unwrap());
        let ptr = u32::from_le_bytes(d[s+20..s+24].try_into().unwrap());
        Self { data: d, sec_va: va, sec_ptr: ptr, sec_end: va + vsz.min(rsz) }
    }
    fn bytes_at(&self, rva: u64) -> Option<&[u8]> {
        let r = rva as u32;
        if r < self.sec_va || r >= self.sec_end { return None; }
        let fo = (self.sec_ptr + (r - self.sec_va)) as usize;
        Some(&self.data[fo..(fo+16).min(self.data.len())])
    }
}

const GPR_NAMES: [&str; 16] = ["rax","rcx","rdx","rbx","rsp","rbp","rsi","rdi",
                                "r8","r9","r10","r11","r12","r13","r14","r15"];

fn main() {
    let mut args = std::env::args().skip(1);
    let trace_path = args.next().expect("usage: isa_diff <trace> [pe]");
    let pe_path = args.next().unwrap_or_else(|| "/tmp/cp2077/Cyberpunk2077.exe".into());
    let max_n: u64 = std::env::var("ISA_DIFF_MAX").ok()
        .and_then(|s| s.parse().ok()).unwrap_or(u64::MAX);

    let pe = PeText::open(&pe_path);
    let f = std::fs::File::open(&trace_path).expect("open trace");
    let mut lines = BufReader::new(f).lines()
        .filter_map(|l| l.ok())
        .filter(|l| !l.is_empty() && !l.starts_with('#'))
        .filter_map(|l| parse_line(&l));

    let mut prev = match lines.next() { Some(l) => l, None => { eprintln!("empty trace"); return; } };
    let (mut n_ok, mut n_skip_mem, mut n_skip_other, mut n_checked, mut n_diverge) =
        (0u64, 0u64, 0u64, 0u64, 0u64);

    // Which flag bits to compare. Default = CF|ZF|SF|OF (0x8C1, the load-bearing
    // four — branch-driving). PF (bit2) is SDM-defined for arith/shift but rarely
    // read + several .isa templates don't compute it (real gap, low priority);
    // AF (bit4) is UNDEFINED for shifts/logical/inc/etc → excluded by default.
    // ISA_DIFF_FLAGMASK=0x8C5 to include PF, =0x8D5 for the full six.
    let eflags_mask: u32 = std::env::var("ISA_DIFF_FLAGMASK").ok()
        .and_then(|s| u32::from_str_radix(s.trim_start_matches("0x"), 16).ok())
        .unwrap_or(0x8C1);
    // ISA_DIFF_CONTINUE=1: report each divergence + keep going (landscape view,
    // tallied by mnemonic). Default = bail at first (the sharp-tool mode).
    let kontinue = std::env::var("ISA_DIFF_CONTINUE").is_ok();
    let mut tally: std::collections::HashMap<(&str, &'static str), u64> =
        std::collections::HashMap::new();

    for cur in lines {
        if n_checked >= max_n { break; }
        n_checked += 1;
        let pc_rva = prev.pc_rva;
        // Prefer trace-supplied bytes (self-contained, ·1080); fall back to PE.
        let pe_bytes;
        let bytes: &[u8] = if !prev.bytes.is_empty() {
            &prev.bytes
        } else if let Some(b) = pe.bytes_at(pc_rva) {
            pe_bytes = b; pe_bytes
        } else {
            n_skip_other += 1; prev = cur; continue;
        };
        let d = match decode_insn(bytes, XMode::Bits64) {
            Some(d) => d,
            None => {
                eprintln!("[seq {}] UNDECODED @rva {:#x}: {:02x?}", prev.seq, pc_rva, &bytes[..8.min(bytes.len())]);
                n_skip_other += 1;
                prev = cur; continue;
            }
        };
        let mnem = DEF_MNEMONICS[d.def_id as usize];

        // Seed interp state from silicon (line N).
        let mut st = X86State::default();
        st.gpr = prev.gpr;
        st.eflags = prev.eflags;
        st.rip = IMAGE_BASE + pc_rva;
        // seg_base[GS] doesn't matter for pure-GPR insns; gs-relative loads → mem → skip.

        // Silence catch_unwind's default panic-print (we report cleanly).
        let saved_hook = std::panic::take_hook();
        std::panic::set_hook(Box::new(|_| {}));
        let mut mem = FlagMem { touched: false.into() };
        let insn_pc = IMAGE_BASE + pc_rva;
        let mut ib = InterpretingBuilder::new(&mut st, &mut mem, insn_pc);
        // Silence intrinsic-panic backtraces (we catch + report cleanly).
        ib.intrinsic = |_,_,_,_| panic!("intrinsic");

        // Execute one insn. lift_one may panic on intrinsic-stubs — catch.
        let panicked = std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            lift_one(&mut ib, &d, insn_pc, XMode::Bits64, FLAGS_ALL_LIVE);
        })).is_err();

        let branched = ib.branched;
        drop(ib);
        std::panic::set_hook(saved_hook);
        let mem_touched = mem.touched.get();

        if panicked {
            // Intrinsic-stub / unimplemented. Report + skip (this is a WALL,
            // not a silent-wrong — the walls-ladder handles these separately).
            if n_skip_other < 5 || std::env::var("ISA_DIFF_VERBOSE").is_ok() {
                eprintln!("[seq {}] INTRINSIC-STUB @rva {:#x}: {} {:02x?}",
                    prev.seq, pc_rva, mnem, &bytes[..d.len as usize]);
            }
            n_skip_other += 1;
            prev = cur; continue;
        }
        if mem_touched {
            n_skip_mem += 1;
            prev = cur; continue;
        }

        // Compare: GPRs + eflags(masked) + next-pc. branch() wrote st.rip via set_pc.
        let interp_next_pc = if branched { st.rip } else { insn_pc + d.len as u64 };
        let expect_next_pc = IMAGE_BASE + cur.pc_rva;

        // Two skip-conditions:
        //   (a) seq-gap: rr_trace.py skipped native shim-frames → cur = guest
        //       return-site, regs changed by shim.
        //   (b) shim-tail-call: `jmp rax` into an IAT thunk (rax = native host
        //       addr, e.g. 0x7f21..) — seq is CONTIGUOUS (rr_trace steps through
        //       native + emits next guest line at the return-site) but the
        //       interp's next-pc = the host addr = correct, silicon's = the
        //       return-site. Detect via interp_next_pc outside guest VA range.
        let seq_gap = cur.seq != prev.seq + 1;
        let shim_xfer = branched && !(IMAGE_BASE..IMAGE_BASE+IMAGE_SIZE).contains(&interp_next_pc);
        let skip_shim = seq_gap || shim_xfer;

        let mut diverged = false;
        if !skip_shim && interp_next_pc != expect_next_pc {
            eprintln!("\n=== DIVERGE @seq {} rva {:#x} [{}] {:02x?} ===",
                prev.seq, pc_rva, mnem, &bytes[..d.len as usize]);
            eprintln!("  next-pc: interp={:#x}  silicon={:#x}", interp_next_pc, expect_next_pc);
            diverged = true;
        }
        for r in 0..16 {
            if st.gpr[r] != cur.gpr[r] && !skip_shim {
                if !diverged {
                    eprintln!("\n=== DIVERGE @seq {} rva {:#x} [{}] {:02x?} ===",
                        prev.seq, pc_rva, mnem, &bytes[..d.len as usize]);
                }
                eprintln!("  {}: interp={:#x}  silicon={:#x}  (pre={:#x})",
                    GPR_NAMES[r], st.gpr[r], cur.gpr[r], prev.gpr[r]);
                diverged = true;
            }
        }
        let mut flag_diverge_bits = 0u32;
        if !skip_shim && (st.eflags & eflags_mask) != (cur.eflags & eflags_mask) {
            flag_diverge_bits = (st.eflags ^ cur.eflags) & eflags_mask;
            if !diverged {
                eprintln!("\n=== DIVERGE @seq {} rva {:#x} [{}] {:02x?} ===",
                    prev.seq, pc_rva, mnem, &bytes[..d.len as usize]);
            }
            eprintln!("  eflags: interp={:#x}  silicon={:#x}  (pre={:#x}, diff-bits={:#x})",
                st.eflags & eflags_mask, cur.eflags & eflags_mask, prev.eflags, flag_diverge_bits);
            diverged = true;
        }

        if diverged {
            n_diverge += 1;
            // Tally by (mnemonic, which-field) — landscape view for continue-mode.
            let field: &'static str = if flag_diverge_bits != 0 && (0..16).all(|r| st.gpr[r]==cur.gpr[r]) {
                match flag_diverge_bits {
                    b if b & !0x04 == 0 => "PF",
                    b if b & !0x10 == 0 => "AF",
                    b if b & !0x01 == 0 => "CF",
                    b if b & !0x800 == 0 => "OF",
                    _ => "eflags",
                }
            } else { "gpr/pc" };
            *tally.entry((mnem, field)).or_default() += 1;
            eprintln!("  pre-state: {}", (0..16).map(|i| format!("{}={:x}", GPR_NAMES[i], prev.gpr[i]))
                .collect::<Vec<_>>().join(" "));
            if !kontinue {
                eprintln!("\n[isa_diff] BAIL at first divergence (seq {}, {} checked, {} ok, {} mem-skip, {} other-skip)",
                    prev.seq, n_checked, n_ok, n_skip_mem, n_skip_other);
                std::process::exit(1);
            }
        }

        if skip_shim { n_skip_other += 1; } else { n_ok += 1; }
        prev = cur;
    }

    if n_diverge > 0 {
        println!("\n[isa_diff] {} DIVERGENCES in {} checked ({} ok, {} mem-skip, {} other-skip). Tally:",
            n_diverge, n_checked, n_ok, n_skip_mem, n_skip_other);
        let mut v: Vec<_> = tally.into_iter().collect();
        v.sort_by_key(|&(_,c)| std::cmp::Reverse(c));
        for ((m, f), c) in v { println!("  {c:6}  {m:12} {f}"); }
        std::process::exit(1);
    }
    println!("[isa_diff] CLEAN: {} checked, {} ok, {} mem-skip, {} other-skip (intrinsic/gap/non-.text)",
        n_checked, n_ok, n_skip_mem, n_skip_other);
}
