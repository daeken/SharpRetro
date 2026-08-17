// Rung-4 gate-(b) harness: feed each aarch64.isa def's synthetic insn through
// `recompile_one` with a RecordingBuilder → dump the IL-seq. Proves the generated
// code RUNS (not just typechecks) + gives spot-checkable per-insn traces.
//
// Corpus mode (default): iterate the .isa's mask/match set, synthesize one insn per
// def (match-bits + zero-fields), record + count. This exercises every emit path once.
//
// Insn mode: `aarch64-recomp <hex-insn> [pc]` → record + dump that one insn's IL-seq.

use aarch64_recomp::recompile_one;
use sharpretro_jit::recording::RecordingBuilder;
use sharpretro_jit::interp::{InterpretingBuilder, FlatMem, GuestMem, IVal};

mod state;
use state::Aarch64State;

#[cfg(target_arch = "aarch64")]
mod native_oracle;

/// Execute one insn via InterpretingBuilder → return post-state.

/// Env-var gate: SET and not "0"/"" = ON. The bare is_ok() idiom reads "0" as ON
/// (the 0-is-truthy class: a sibling project wrote GBs of logs under GATE=0 for
/// exactly this). Population-changing gates (XF_NOMEM, XF_CASP) MUST use this;
/// debug prints too, for one idiom not two.
fn env_on(name: &str) -> bool {
    match std::env::var(name) { Ok(v) => !v.is_empty() && v != "0", Err(_) => false }
}

fn interp_one<M: GuestMem>(pre: &Aarch64State, mem: &mut M, insn: u32, pc: u64) -> (Aarch64State, bool) {
    let mut s = pre.clone();
    s.pc = pc;
    let branched;
    {
        let mut b = InterpretingBuilder::new(&mut s, mem, pc);
        // Intrinsic policy for the EXEC-TRUTH ORACLE. Three classes, and the split is
        // about what a single-threaded fuzz harness can honestly model:
        //
        //   WIRED — semantics fully determined without an OS or a second thread:
        //     id=3 load_excl  (LDXR/LDXRB/LDXRH/LDAXR…) — with no contention an exclusive
        //       load IS a plain load; the monitor's only observable effect is on a later
        //       store-exclusive, which we also model. The intrinsic gets NO width arg (the
        //       generator drops the .isa's `u8`/`u32`/… token), so read the widest the def
        //       can want and let the generated downstream `cast` narrow — which is exactly
        //       what the emitted code already does (`cast(_t1, U32)` after the call).
        //     id=4 store_excl (STXR/STXRB/…) — with no contention it always SUCCEEDS, so:
        //       store, return 0. Width IS available here and IS load-bearing: the value arg
        //       is pre-cast by the generator (`cast(_t1, U8)` before the call), so
        //       args[1].ty carries it. Writing 8 bytes for a byte-store would corrupt the
        //       neighbours and the diff would surface as a mystery in an unrelated def.
        //
        //   UNWIRED, DELIBERATELY — the insn's meaning LEAVES the instruction:
        //     id=0 sr_read (system registers: no MSR/MRS state model in the fuzz state),
        //     id=1 svc (a syscall — the effect is the OS's), id=2 breakpoint (a trap).
        //     Panicking is the honest answer: a fabricated value would be diffed against
        //     silicon that actually has the register/handler, and the diff would be OURS.
        //
        //   UNWIRED, NOT YET — id≥100, the vector intrinsics. Named per-id in the panic so
        //     the tally says which, rather than "some intrinsic".
        b.intrinsic = |_s, m, id, a| match id {
            3 => {
                if env_on("XF_EXCL_DBG") {
                    eprintln!("  [excl] load_excl addr={:#x}", a[0].bits);
                }
                let addr = a[0].bits as u64;
                // 64 = BITS. GuestMem::read/write take a BIT width (interp.rs:738, n=(w+7)/8);
                // passing 8 reads ONE BYTE. That produced x2=0xef on a 64-bit LDXR and looked
                // exactly like an unwired intrinsic — predicted before the fire, confirmed by it.
                Some(IVal::u(64, m.read(addr, 64)))
            }
            4 => {
                if env_on("XF_EXCL_DBG") {
                    eprintln!("  [excl] store_excl addr={:#x} val={:#x} ty={:?}", a[0].bits, a[1].bits, a[1].ty);
                }
                let addr = a[0].bits as u64;
                // width via the same match-idiom interp.rs:155 uses — IlType has no
                // width() accessor and inventing one would be a second API to keep in sync.
                let w = match a[1].ty { sharpretro_jit::IlType::I{width,..} => width, _ => 64 };
                m.write(addr, w, a[1].bits);
                Some(IVal::u(32, 0))     // 0 = store-exclusive SUCCEEDED
            }
            101 => {
                // vec_extract (EXT): result = concat(b:a) >> (index*8), i.e. lanes
                // a[index..count] then b[0..index]. Q selects 8/16-byte width; the
                // 8-byte form's top half zeroes. SDM semantics — NOT transcribed from
                // C#'s Math.VectorExtract, whose second loop reads `a` where EXT wants
                // `b` (silicon arbitrates; if the fuzz diffs, re-read HERE first).
                let (av, bv, q, idx) = (a[0].bits, a[1].bits, a[2].bits as u32, a[3].bits as usize);
                let count = if q == 0 { 8usize } else { 16 };
                let byte = |v: u128, i: usize| -> u128 { (v >> (8*i)) & 0xFF };
                let mut r: u128 = 0;
                for out in 0..count {
                    let src = idx + out;
                    let b8 = if src < count { byte(av, src) } else { byte(bv, src - count) };
                    r |= b8 << (8*out);
                }
                Some(IVal { ty: sharpretro_jit::IlType::V128, bits: r })
            }
            102 => {
                // vec_popcnt (CNT): per-BYTE popcount over the FIRST `count` bytes
                // (arg-2 = the .isa's lane-COUNT: 8 for the 8B form, 16 for 16B; the
                // v1 read it as width and counted all 16 — CNT.8B must zero-top,
                // fuzz caught it first fire).
                let v = a[0].bits;
                let count = if a.len() > 1 { (a[1].bits as usize).min(16) } else { 16 };
                let mut r: u128 = 0;
                for i in 0..count {
                    r |= (((v >> (8*i)) & 0xFF).count_ones() as u128) << (8*i);
                }
                Some(IVal { ty: sharpretro_jit::IlType::V128, bits: r })
            }
            103 => {
                // vec_sum_u (UADDLV): unsigned sum of `count` lanes of `esize` bits.
                // args = [vec, esize]; count isn't passed on all sites — derive from
                // the .isa's own contract when present (3rd arg), else 128/esize.
                let v = a[0].bits;
                let esize = a[1].bits as u32;
                let count = if a.len() > 2 { a[2].bits as u32 } else { 128 / esize };
                let mask = (1u128 << esize) - 1;
                let mut sum: u128 = 0;
                for i in 0..count { sum += (v >> (esize*i)) & mask; }
                Some(IVal::u(64, sum))
            }
            100 => {
                // vec_broadcast (the .isa's `vector-all`): replicate the scalar arg
                // across V128, lane width = the arg's OWN IlType width (the C# twin is
                // typed CreateVector). 51 call-sites — the whole MOVI family + every
                // `one/zero`-mlet compare macro that broadcasts.
                let w = match a[0].ty {
                    sharpretro_jit::IlType::I{width,..} => width,
                    sharpretro_jit::IlType::F{width} => width,   // LD1R.4S loads F32 —
                    // the bare I-match sent floats to the 64 fallback (fuzz caught it
                    // in the same fire that unblocked the family: 4S replicated at
                    // 64-bit stride). Bool/V128/Unit keep the 64 fallback.
                    _ => 64,
                } as u32;
                let lane = a[0].bits & (if w >= 128 { u128::MAX } else { (1u128 << w) - 1 });
                let mut v: u128 = 0;
                let mut off = 0;
                while off + w <= 128 { v |= lane << off; off += w; }
                Some(IVal { ty: sharpretro_jit::IlType::V128, bits: v })
            }
            _ => panic!("intrinsic id={id} not wired"),
        };
        let ok = recompile_one(&mut b, insn, pc);
        assert!(ok, "insn 0x{insn:08X} not decoded");
        branched = b.branched;
    }
    if !branched { s.pc = pc + 4; }
    (s, branched)
}

/// Defs whose GENERATED body contains a memory op — derived from `lib.rs` itself rather than
/// from def-NAMES, because a name-guess is a claim about the .isa and the emitted code is the
/// artifact. Re-derive with:
///
///   python3 -c "import re;s=open('src/lib.rs').read();
///     m=[(x.start(),x.group(1)) for x in re.finditer(r'/\\* ([A-Za-z0-9_.-]+) \\*/',s)];
///     print([n for i,(p,n) in enumerate(m) if 'mem_read' in s[p:(m[i+1][0] if i+1<len(m) else len(s))]
///            or 'mem_write' in s[p:(m[i+1][0] if i+1<len(m) else len(s))]])"
///
/// ‡ Name-based guessing would have been wrong in BOTH directions: STP-simd-signed-offset reads
/// FALSE here and is correct — its body decodes and `return true`s with no semantics (one of the
/// 8 known bare-body defs), so it touches no memory to diff.
const LDST_DEFS: &[&str] = &[
    "CASP",
    "CASPA",
    "CASPAL",
    "CASPL",
    "LD1-multi-no-offset-four-registers",
    "LD1-multi-no-offset-four-registers-postindex-immediate",
    "LD1-multi-no-offset-one-register",
    "LD1-multi-no-offset-one-register-postindex-immediate",
    "LD1-multi-no-offset-three-registers",
    "LD1-multi-no-offset-three-registers-postindex-immediate",
    "LD1-multi-no-offset-two-registers",
    "LD1-multi-no-offset-two-registers-postindex-immediate",
    "LD1-single-no-offset",
    "LD1R-single-no-offset",
    "LD1R-single-postindex-immediate",
    "LD1R-single-postindex-register",
    "LD2-multi-postindex-immediate",
    "LD2-multi-postindex-register",
    "LD3-multi-no-offset",
    "LD3-multi-postindex-immediate",
    "LD3-multi-postindex-register",
    "LD4-multi-postindex-immediate",
    "LD4-multi-postindex-register",
    "LDAR",
    "LDARB",
    "LDARH",
    "LDP-immediate-postindex",
    "LDP-immediate-preindex",
    "LDP-immediate-signed-offset",
    "LDP-simd-postindex",
    "LDP-simd-preindex",
    "LDP-simd-signed-offset",
    "LDPSW-immediate-signed-offset",
    "LDR-immediate-postindex",
    "LDR-immediate-preindex",
    "LDR-immediate-unsigned-offset",
    "LDR-literal",
    "LDR-register",
    "LDR-simd-immediate-postindex",
    "LDR-simd-immediate-preindex",
    "LDR-simd-immediate-unsigned-offset",
    "LDR-simd-literal",
    "LDR-simd-register",
    "LDRB-immediate-postindex",
    "LDRB-immediate-preindex",
    "LDRB-immediate-unsigned-offset",
    "LDRB-register",
    "LDRH-immediate-postindex",
    "LDRH-immediate-preindex",
    "LDRH-immediate-unsigned-offset",
    "LDRH-register",
    "LDRSB-immediate-postindex",
    "LDRSB-immediate-preindex",
    "LDRSB-immediate-unsigned-offset",
    "LDRSB-register",
    "LDRSH-immediate-postindex",
    "LDRSH-immediate-preindex",
    "LDRSH-immediate-unsigned-offset",
    "LDRSH-register",
    "LDRSW-immediate-postindex",
    "LDRSW-immediate-preindex",
    "LDRSW-immediate-unsigned-offset",
    "LDRSW-literal",
    "LDRSW-register",
    "LDUR",
    "LDUR-simd",
    "LDURB",
    "LDURH",
    "LDURSB",
    "LDURSH",
    "LDURSW",
    "ST1-multi-no-offset-four-registers",
    "ST1-multi-no-offset-three-registers",
    "ST1-multi-no-offset-two-registers",
    "ST1-multi-postindex-immediate-four-registers",
    "ST1-multi-postindex-immediate-one-register",
    "ST1-multi-postindex-immediate-three-registers",
    "ST1-multi-postindex-immediate-two-registers",
    "ST1-multi-postindex-register-four-registers",
    "ST1-multi-postindex-register-one-register",
    "ST1-multi-postindex-register-three-registers",
    "ST1-multi-postindex-register-two-registers",
    "ST1-single-no-offset",
    "ST2-multi-no-offset",
    "ST2-multi-postindex-immediate",
    "ST2-multi-postindex-register",
    "ST3-multi-no-offset",
    "ST3-multi-postindex-immediate",
    "ST3-multi-postindex-register",
    "ST4-multi-postindex-immediate",
    "ST4-multi-postindex-register",
    "STLR",
    "STLRB",
    "STLRH",
    "STLXR",
    "STLXRB",
    "STP-postindex",
    "STP-preindex",
    "STP-signed-offset",
    "STP-simd-postindex",
    "STP-simd-preindex",
    "STP-simd-signed-offset",
    "STR-immediate-postindex",
    "STR-immediate-preindex",
    "STR-immediate-unsigned-offset",
    "STR-register",
    "STR-simd-postindex",
    "STR-simd-preindex",
    "STR-simd-register",
    "STR-simd-unsigned-offset",
    "STRB-immediate-postindex",
    "STRB-immediate-preindex",
    "STRB-immediate-unsigned-offset",
    "STRB-register",
    "STRH-immediate-postindex",
    "STRH-immediate-preindex",
    "STRH-immediate-unsigned-offset",
    "STRH-register",
    "STUR",
    "STUR-simd",
    "STURB",
    "STURH",
];

/// Every def whose generated body is LD/ST-SHAPED — direct `mem_read`/`mem_write` (99) PLUS the
/// forms whose memory access is intrinsic-lowered (LDXR/LDAXB/LDXP — exclusives become
/// `intrinsic(...)`, never `mem_read`) and the PC-relative `*-literal` pool loads. 110 total.
///
/// This is the BASE-PLACEMENT set, not the diff set: a def in here gets its base registers
/// pointed into the arena so the interp doesn't index FlatMem with a random u64 and panic
/// `arena-oob`. Whether SILICON also runs is `is_ldst() && arena_ok()` — narrower on purpose.
///
/// Re-derive (never hand-edit — the generated body is the authority):
///   python3 - <<'PY'  (see LDST_DEFS above; add `intrinsic(` w/ an LD|ST|CAS|SWP name, and
///                      names ending `-literal`)
static LDST_SHAPED: &[&str] = &[
    "CASP",
    "CASPA",
    "CASPAL",
    "CASPL",
    "LD1-multi-no-offset-four-registers",
    "LD1-multi-no-offset-four-registers-postindex-immediate",
    "LD1-multi-no-offset-one-register",
    "LD1-multi-no-offset-one-register-postindex-immediate",
    "LD1-multi-no-offset-three-registers",
    "LD1-multi-no-offset-three-registers-postindex-immediate",
    "LD1-multi-no-offset-two-registers",
    "LD1-multi-no-offset-two-registers-postindex-immediate",
    "LD1-single-no-offset",
    "LD1R-single-no-offset",
    "LD1R-single-postindex-immediate",
    "LD1R-single-postindex-register",
    "LD2-multi-postindex-immediate",
    "LD2-multi-postindex-register",
    "LD3-multi-no-offset",
    "LD3-multi-postindex-immediate",
    "LD3-multi-postindex-register",
    "LD4-multi-postindex-immediate",
    "LD4-multi-postindex-register",
    "LDAR",
    "LDARB",
    "LDARH",
    "LDAXB",
    "LDAXRB",
    "LDAXRH",
    "LDP-immediate-postindex",
    "LDP-immediate-preindex",
    "LDP-immediate-signed-offset",
    "LDP-simd-postindex",
    "LDP-simd-preindex",
    "LDP-simd-signed-offset",
    "LDPSW-immediate-signed-offset",
    "LDR-immediate-postindex",
    "LDR-immediate-preindex",
    "LDR-immediate-unsigned-offset",
    "LDR-literal",
    "LDR-register",
    "LDR-simd-immediate-postindex",
    "LDR-simd-immediate-preindex",
    "LDR-simd-immediate-unsigned-offset",
    "LDR-simd-literal",
    "LDR-simd-register",
    "LDRB-immediate-postindex",
    "LDRB-immediate-preindex",
    "LDRB-immediate-unsigned-offset",
    "LDRB-register",
    "LDRH-immediate-postindex",
    "LDRH-immediate-preindex",
    "LDRH-immediate-unsigned-offset",
    "LDRH-register",
    "LDRSB-immediate-postindex",
    "LDRSB-immediate-preindex",
    "LDRSB-immediate-unsigned-offset",
    "LDRSB-register",
    "LDRSH-immediate-postindex",
    "LDRSH-immediate-preindex",
    "LDRSH-immediate-unsigned-offset",
    "LDRSH-register",
    "LDRSW-immediate-postindex",
    "LDRSW-immediate-preindex",
    "LDRSW-immediate-unsigned-offset",
    "LDRSW-literal",
    "LDRSW-register",
    "LDUR",
    "LDUR-simd",
    "LDURB",
    "LDURH",
    "LDURSB",
    "LDURSH",
    "LDURSW",
    "LDXP",
    "LDXR",
    "LDXRB",
    "LDXRH",
    "PRFM-literal",
    "ST1-multi-no-offset-four-registers",
    "ST1-multi-no-offset-three-registers",
    "ST1-multi-no-offset-two-registers",
    "ST1-multi-postindex-immediate-four-registers",
    "ST1-multi-postindex-immediate-one-register",
    "ST1-multi-postindex-immediate-three-registers",
    "ST1-multi-postindex-immediate-two-registers",
    "ST1-multi-postindex-register-four-registers",
    "ST1-multi-postindex-register-one-register",
    "ST1-multi-postindex-register-three-registers",
    "ST1-multi-postindex-register-two-registers",
    "ST1-single-no-offset",
    "ST2-multi-no-offset",
    "ST2-multi-postindex-immediate",
    "ST2-multi-postindex-register",
    "ST3-multi-no-offset",
    "ST3-multi-postindex-immediate",
    "ST3-multi-postindex-register",
    "ST4-multi-postindex-immediate",
    "ST4-multi-postindex-register",
    "STLR",
    "STLRB",
    "STLRH",
    "STLXR",
    "STLXRB",
    "STP-postindex",
    "STP-preindex",
    "STP-signed-offset",
    "STP-simd-postindex",
    "STP-simd-preindex",
    "STP-simd-signed-offset",
    "STR-immediate-postindex",
    "STR-immediate-preindex",
    "STR-immediate-unsigned-offset",
    "STR-register",
    "STR-simd-postindex",
    "STR-simd-preindex",
    "STR-simd-register",
    "STR-simd-unsigned-offset",
    "STRB-immediate-postindex",
    "STRB-immediate-preindex",
    "STRB-immediate-unsigned-offset",
    "STRB-register",
    "STRH-immediate-postindex",
    "STRH-immediate-preindex",
    "STRH-immediate-unsigned-offset",
    "STRH-register",
    "STUR",
    "STUR-simd",
    "STURB",
    "STURH",
    "STXP",
    "STXR",
    "STXRB",
];

/// Is this def LD/ST-shaped (incl. intrinsic-lowered exclusives and literal-pool loads)?
fn ldst_shaped(name: &str) -> bool { LDST_SHAPED.binary_search(&name).is_ok() }

/// Does this def's generated body touch guest memory?
fn is_ldst(name: &str) -> bool { LDST_DEFS.binary_search(&name).is_ok() }

fn main() {
    let args: Vec<String> = std::env::args().collect();

    // --run <program> — the block-driver: load a small program into guest memory,
    // run via interp AND tier-0-block-cache, diff final state. THE step-④ oracle.
    // <program> = a name ("sum10") or a comma-sep hex-insn list.
    #[cfg(target_arch = "aarch64")]
    // --excl-test — direct proof that the load/store-exclusive intrinsics EXECUTE, rather
    // than inferring it from a fuzz tally. The tally can't show it: def-names are recorded
    // only on diff/reject, so a working def is indistinguishable from one that was never
    // selected. Addresses are CHOSEN (inside the arena) because the fuzz's random regs put
    // them outside it 598-of-835 times, which is an arena limit rather than a semantics gap.
    if args.get(1).map(|s| s.as_str()) == Some("--excl-test") {
        // FlatMem::new(BASE, size) — base=0x10000, so valid guest addrs are 0x10000..0x20000.
        // My first pass chose 0x1000/0x2000 (below base): `addr - self.base` underflows to
        // ~2^64 and indexes out of bounds. The panic read as "intrinsic not wired" because
        // that's what the test's own message said — the arm was fine, my ADDRESSES were.
        let mut mem = FlatMem::new(0x10000, 0x10000);
        let mut fail = 0usize;
        // STXR w0, [x1] then LDXR w2, [x1]: store 0xDEADBEEF, read it back.
        // Encodings hand-built and decode-back-verified below against DEF_MNEMONICS.
        for (name, insn, pre_x, want) in [
            // STXR Ws, Wt, [Xn]: sf=0 -> 32-bit. 88 1F 7C 20 = stxr w31?,... build explicitly:
            ("STXR-32", 0x8802_7C20u32, [0x11000u64, 0xDEAD_BEEFu64], 0xDEAD_BEEFu64),
        ] {
            let mut st = Aarch64State::default();
            // objdump: `stxr w2, w0, [x1]` — Ws=w2 (STATUS out), Wt=w0 (VALUE in), Xn=x1 (ADDR).
            // Setting x2 as the value would store 0 from w0 and read as a working store of the
            // wrong number: decode-back gave the encoding, and the OPERAND ORDER still had to
            // be read off it rather than assumed from the mnemonic's argument order.
            st.x[1] = pre_x[0]; st.x[0] = pre_x[1];
            let r = std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
                interp_one(&st, &mut mem, insn, 0x1000)
            }));
            match r {
                Ok(_) => {
                    // NB: GuestMem::read/write take w in BITS, not bytes (interp.rs:738, n=(w+7)/8).
                    // My first version passed 4 and read ONE byte — the 0xef that made a
                    // correctly-wired intrinsic look broken.
                    let got = mem.read(0x11000, 32);
                    let ok = got == want as u128;
                    println!("  {name}: mem[0x11000]={got:#x} want={want:#x} {}", if ok {"OK"} else {"FAIL"});
                    if !ok { fail += 1; }
                }
                Err(_) => { println!("  {name}: PANICKED (intrinsic not wired?)"); fail += 1; }
            }
        }
        // LDXR must read back what a plain store put there.
        mem.write(0x12000, 64, 0x0123_4567_89AB_CDEF);   // 64 = BITS
        // §4: read it back through the SAME api before trusting the store — a wrong-unit or
        // wrong-base write is what made the STXR arm read 0xef.
        let sanity = mem.read(0x12000, 64);
        println!("  [sanity] mem[0x12000]={sanity:#x} (want 0x123456789abcdef)");
        let mut st = Aarch64State::default();
        st.x[3] = 0x12000;
        let r = std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            interp_one(&st, &mut mem, 0xC85F_7C62u32, 0x1000)   // LDXR x2, [x3]
        }));
        match r {
            Ok((post, _)) => {
                let ok = post.x[2] == 0x0123_4567_89AB_CDEF;
                println!("  LDXR-64: x2={:#x} {}", post.x[2], if ok {"OK"} else {"FAIL"});
                if !ok { fail += 1; }
            }
            Err(_) => { println!("  LDXR-64: PANICKED (intrinsic not wired?)"); fail += 1; }
        }
        println!("{}", if fail == 0 { "excl-test: PASS" } else { "excl-test: FAIL" });
        std::process::exit(if fail == 0 { 0 } else { 1 });
    }
    if args.get(1).map(|s| s.as_str()) == Some("--run") {
        // ── the test program(s) ────────────────────────────────────────────
        // sum10: x0 = Σ 1..10 = 55. Exercises MOVZ, ADD-reg, ADD-imm, SUBS(CMP),
        // B.cond, and the block-driver's branch-following.
        let sum10: &[u32] = &[
            0xD2800000,  // mov x0, #0        (sum)
            0xD2800021,  // mov x1, #1        (i)
            0xD2800162,  // mov x2, #11       (N+1, so loop runs 1..10 inclusive)
            // loop:
            0x8B010000,  // add x0, x0, x1
            0x91000421,  // add x1, x1, #1
            0xEB02003F,  // cmp x1, x2  (subs xzr, x1, x2)
            0x54FFFFAB,  // b.lt loop  (-3 insns = -12 bytes)
            0xD4200000,  // brk #0  (stop signal)
        ];
        // memsum: x0 = Σ mem[x1..x1+N*8] as u64s. Exercises LDR (load) + branch loop.
        // Setup: x1=array_base, x3=N. Data at guest_base+0x100.
        let memsum: &[u32] = &[
            0xD2800000,  // mov x0, #0
            0xD2802001,  // mov x1, #0x100  (array base = guest+0x100)
            0xD28000A3,  // mov x3, #5
            // loop:
            0xF8408424,  // ldr x4, [x1], #8   (post-index: load + x1+=8)
            0x8B040000,  // add x0, x0, x4
            0xD1000463,  // sub x3, x3, #1
            0xB5FFFFA3,  // cbnz x3, loop  (-3 insns)
            0xD4200000,  // brk #0
        ];
        // fib: x0 = fib(N) via iterative loop. x3=N.
        let fib: &[u32] = &[
            0xD2800000,  // mov x0, #0  (a)
            0xD2800021,  // mov x1, #1  (b)
            0xD2800183,  // mov x3, #12 (N)
            // loop:
            0x8B010002,  // add x2, x0, x1
            0xAA0103E0,  // mov x0, x1
            0xAA0203E1,  // mov x1, x2
            0xD1000463,  // sub x3, x3, #1
            0xF100047F,  // cmp x3, #1  (subs xzr, x3, #1)
            0x54FFFF6C,  // b.gt loop  (-5 insns)
            0xD4200000,  // brk #0
        ];
        let prog = match args.get(2).map(|s| s.as_str()) {
            Some("sum10") | None => sum10,
            Some("fib") => fib,
            Some("memsum") => memsum,
            Some(hex) => {
                // Parse comma-sep hex insns
                let v: Vec<u32> = hex.split(',').map(|s|
                    u32::from_str_radix(s.trim().trim_start_matches("0x"), 16).unwrap()).collect();
                Box::leak(v.into_boxed_slice())
            }
        };
        let entry: u64 = 0x10000;
        let max_insns = 1000;

        // ── interp driver ──────────────────────────────────────────────────
        // GuestMem: FlatMem at base=0 (guest addresses ARE offsets into the vec).
        // The tier-0 side sets mem_base = the same vec's host ptr, so both sides
        // read the same bytes at the same guest addrs.
        let mut guest_bytes = vec![0u8; 0x20000];
        // Load program at `entry`.
        for (i, &w) in prog.iter().enumerate() {
            guest_bytes[entry as usize + i*4 .. entry as usize + i*4 + 4]
                .copy_from_slice(&w.to_le_bytes());
        }
        // memsum test data: 5 u64s at guest+0x100 = {10,20,30,40,50} → sum=150.
        for (i, &v) in [10u64, 20, 30, 40, 50].iter().enumerate() {
            guest_bytes[0x100 + i*8 .. 0x100 + i*8 + 8].copy_from_slice(&v.to_le_bytes());
        }
        struct SharedMem<'a>(&'a mut [u8]);
        impl<'a> GuestMem for SharedMem<'a> {
            fn read(&self, addr: u64, w: u8) -> u128 {
                let n = ((w as usize)+7)/8; let off = addr as usize;
                let mut v = 0u128;
                for i in 0..n { v |= (self.0[off+i] as u128) << (i*8); }
                v
            }
            fn write(&mut self, addr: u64, w: u8, bits: u128) {
                let n = ((w as usize)+7)/8; let off = addr as usize;
                for i in 0..n { self.0[off+i] = (bits >> (i*8)) as u8; }
            }
        }
        let host_base = guest_bytes.as_mut_ptr() as u64;
        let mut mem_i = SharedMem(&mut guest_bytes);
        let mut si = Aarch64State::default();
        si.pc = entry;
        let mut n_i = 0;
        loop {
            let insn = mem_i.read(si.pc, 32) as u32;
            // BRK = stop.
            if (insn & 0xFFE00000) == 0xD4200000 { break; }
            let (post, branched) = interp_one(&si, &mut mem_i, insn, si.pc);
            let next = if branched { post.pc } else { si.pc + 4 };
            si = post; si.pc = next;
            n_i += 1;
            if n_i > max_insns { println!("interp: max_insns hit"); break; }
        }
        println!("[interp: {} insns, x0=0x{:X} x1=0x{:X} pc=0x{:X}]", n_i, si.x[0], si.x[1], si.pc);

        // ── tier-0 via BlockCache (crate-level; the DESIGN.md step-④ shape) ─
        use sharpretro_jit::tier0::{Tier0, STATE_WORDS};
        use sharpretro_jit::block_cache::{BlockCache, BlockCompiler, StopReason};
        use sharpretro_jit::{Builder, IlType};

        // Aarch64Compiler: BlockCompiler impl over the shared guest-bytes.
        // fetch = read u32 from host_base+pc; compile_block = recompile_one in a loop
        // until branched()/BRK/max. This is the arch-specific glue between BlockCache
        // (arch-neutral) and the generated recompiler.rs.
        struct Aarch64Compiler { host_base: u64, max_block: usize }
        impl BlockCompiler for Aarch64Compiler {
            fn fetch(&self, pc: u64) -> u32 {
                unsafe { ((self.host_base + pc) as *const u32).read_unaligned() }
            }
            fn is_stop(&self, insn: u32) -> bool {
                (insn & 0xFFE00000) == 0xD4200000  // BRK
            }
            fn compile_block<BB: sharpretro_jit::Builder<Val = u32>>(&self, t0: &mut BB, pc: u64, _mode: u32) -> (u64, StopReason) {
                let mut cur = pc;
                for n in 0..self.max_block {
                    let insn = self.fetch(cur);
                    if self.is_stop(insn) {
                        // Emit branch-to-cur so pc=cur; driver's next-iter stop-check catches it.
                        let t = t0.literal(IlType::U64, cur as u128);
                        t0.branch(t, false);
                        return (cur, StopReason::StopInsn);
                    }
                    if !recompile_one(t0, insn, cur) {
                        panic!("block@0x{pc:X}+{n}: insn 0x{insn:08X} not decoded");
                    }
                    if t0.branched() { return (cur + 4, StopReason::Branched); }
                    cur += 4;
                }
                (cur, StopReason::MaxInsns)
            }
        }

        let compiler = Aarch64Compiler { host_base, max_block: 32 };
        let mut cache = BlockCache::new();
        let mut flat = [0u64; STATE_WORDS];
        flat[33] = entry;
        flat[66] = host_base;
        let result = cache.run(&compiler, &mut flat[..], 0, max_insns);
        println!("[tier0: {} block-execs, {} compiles, x0=0x{:X} x1=0x{:X} pc=0x{:X}, {:?}]",
            cache.n_execs, cache.n_compiles, flat[0], flat[1], flat[33], result);

        // ── diff ───────────────────────────────────────────────────────────
        let mut d = vec![];
        for r in 0..31 { if si.x[r] != flat[r] {
            d.push(format!("x{r}: interp=0x{:X} tier0=0x{:X}", si.x[r], flat[r])); } }
        if d.is_empty() {
            println!("✓ MATCH");
        } else {
            println!("✗ DIFF:");
            for l in &d { println!("    {l}"); }
        }
        return;
    }

    // --interp <hex-insn> [<hex-insn>...] — execute a sequence via InterpretingBuilder,
    // dump changed regs. Optional `x<N>=<hex>` args set initial state.
    if args.get(1).map(|s| s.as_str()) == Some("--interp") {
        let mut s = Aarch64State::default();
        let mut mem = FlatMem::new(0x10000, 0x10000);
        let mut insns = vec![];
        for a in &args[2..] {
            if let Some((r, v)) = a.split_once('=') {
                let val = u64::from_str_radix(v.trim_start_matches("0x"), 16).unwrap();
                if let Some(n) = r.strip_prefix('x') { s.x[n.parse::<usize>().unwrap()] = val; }
                else if r == "sp" { s.x[31] = val; }
                else if r == "nzcv" { s.nzcv = val as u32; }
            } else {
                insns.push(u32::from_str_radix(a.trim_start_matches("0x"), 16).unwrap());
            }
        }
        let pre = s.clone();
        let mut pc = 0x1000u64;
        for &insn in &insns {
            println!("→ 0x{pc:X}: 0x{insn:08X}");
            let (post, branched) = interp_one(&s, &mut mem, insn, pc);
            s = post;
            pc = if branched { s.pc } else { pc + 4 };
        }
        println!("─── final state (changed only) ───");
        for i in 0..32 { if s.x[i] != pre.x[i] {
            println!("  x{i:2} = 0x{:016X}  (was 0x{:X})", s.x[i], pre.x[i]); } }
        if s.nzcv != pre.nzcv { println!("  nzcv= 0x{:08X}  N={} Z={} C={} V={}",
            s.nzcv, s.n() as u8, s.z() as u8, s.c() as u8, s.vf() as u8); }
        println!("  pc  = 0x{:X}", s.pc);
        return;
    }

    // --tier0-fuzz [N] — the tier-0 GATE: same corpus/pre-state as --fuzz, but
    // tier-0-JIT'd-machine-code vs interp (instead of native-silicon vs interp).
    // Per DESIGN.md §Oracles: "tier-0 vs interpreter → state diff = 0".
    #[cfg(target_arch = "aarch64")]
    if args.get(1).map(|s| s.as_str()) == Some("--tier0-fuzz") {
        use sharpretro_jit::tier0::{Tier0, STATE_WORDS};
        let n: usize = args.get(2).and_then(|s| s.parse().ok()).unwrap_or(3);
        let seed: u64 = args.get(3).and_then(|s| s.parse().ok()).unwrap_or(0xC0FFEE);
        let mut mem = FlatMem::new(0x10000, 0x10000);
        let src = include_str!("lib.rs");
        let mut defs = vec![];
        let mut cur_name = "";
        for line in src.lines() {
            if let Some(n) = line.trim().strip_prefix("/* ").and_then(|s| s.strip_suffix(" */")) {
                cur_name = n;
            }
            if let Some(rest) = line.trim().strip_prefix("if (insn & 0x") {
                let mask_end = rest.find(')').unwrap();
                let mask = u32::from_str_radix(&rest[..mask_end], 16).unwrap();
                let ms = rest[mask_end..].find("0x").unwrap() + mask_end + 2;
                let me = rest[ms..].find(' ').unwrap() + ms;
                let mat = u32::from_str_radix(&rest[ms..me], 16).unwrap();
                defs.push((cur_name.to_string(), mask, mat));
            }
        }
        let mut rng = seed;
        let mut rand = || { rng ^= rng<<13; rng ^= rng>>7; rng ^= rng<<17; rng };
        let (mut n_ok, mut n_diff, mut n_ipanic, mut n_t0panic, mut n_skip) = (0usize, 0usize, 0usize, 0usize, 0usize);
        let mut diff_by_def: std::collections::BTreeMap<String, usize> = Default::default();
        let mut t0panic_by: std::collections::BTreeMap<String, usize> = Default::default();
        // Defs whose emit contains bd.mem_read/write — random reg-values give random
        // guest-addrs → interp FlatMem panics (caught), tier-0 segfaults (not). SKIP;
        // mem is covered by --run memsum (which controls addresses). Extract from
        // the generated lib.rs directly.
        let mem_defs: std::collections::HashSet<String> = {
            let mut cur = ""; let mut set = std::collections::HashSet::new();
            for line in src.lines() {
                if let Some(n) = line.trim().strip_prefix("/* ").and_then(|s| s.strip_suffix(" */")) {
                    cur = n;
                }
                if line.contains("bd.mem_read") || line.contains("bd.mem_write") {
                    set.insert(cur.to_string());
                }
            }
            set
        };
        for (name, mask, mat) in &defs {
            if mem_defs.contains(name) { n_skip += n; continue; }
            for _ in 0..n {
                let mut fields = (rand() as u32) & !mask;
                for sh in [0, 5, 10, 16] {
                    if (fields >> sh) & 0x1F == 31 { fields &= !(1u32 << sh); }
                }
                let insn = mat | fields;
                let mut pre = Aarch64State::default();
                for r in 1..=28 { pre.x[r] = rand(); }
                pre.nzcv = ((rand() as u32) & 0xF) << 28;
                // interp side
                let ir = std::panic::catch_unwind(std::panic::AssertUnwindSafe(||
                    interp_one(&pre, &mut mem, insn, 0x1000).0));
                let i_post = match ir { Ok(s) => s, Err(_) => { n_ipanic += 1; continue; } };
                // tier-0 side
                let t0r = std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
                    let mut t0 = Tier0::new();
                    if !recompile_one(&mut t0, insn, 0x1000) { panic!("not decoded"); }
                    let block = t0.finalize();
                    let mut flat = [0u64; STATE_WORDS];
                    for i in 0..32 { flat[i] = pre.x[i]; }
                    flat[32] = pre.nzcv as u64;
                    flat[33] = 0x1000;
                    block.exec(&mut flat);
                    flat
                }));
                let flat = match t0r {
                    Ok(f) => f,
                    Err(e) => {
                        n_t0panic += 1;
                        let msg = e.downcast_ref::<String>().map(|s| s.as_str())
                            .or_else(|| e.downcast_ref::<&str>().copied()).unwrap_or("?");
                        // Tally by panic-reason (which tier-0 op is unwired) — the coverage frontier.
                        let key = msg.split(':').next().unwrap_or(msg).to_string();
                        *t0panic_by.entry(key).or_default() += 1;
                        continue;
                    }
                };
                let mut d = false;
                for r in 0..31 { if i_post.x[r] != flat[r] { d = true; break; } }
                if (i_post.nzcv & 0xF0000000) != ((flat[32] as u32) & 0xF0000000) { d = true; }
                let interp_branched = i_post.pc != 0x1004;
                if interp_branched && i_post.pc != flat[33] { d = true; }
                if d {
                    n_diff += 1; *diff_by_def.entry(name.clone()).or_default() += 1;
                    if diff_by_def[name] == 1 {
                        eprintln!("DIFF {name} insn=0x{insn:08X}:");
                        for r in 0..31 { if i_post.x[r] != flat[r] {
                            eprintln!("    x{r}: interp=0x{:X} tier0=0x{:X} (pre=0x{:X})",
                                i_post.x[r], flat[r], pre.x[r]); } }
                        if (i_post.nzcv & 0xF0000000) != ((flat[32] as u32) & 0xF0000000) {
                            eprintln!("    nzcv: interp=0x{:08X} tier0=0x{:08X}",
                                i_post.nzcv, flat[32] as u32); }
                    }
                } else { n_ok += 1; }
            }
        }
        println!("[tier0-fuzz: {} defs × {} = {} triples]", defs.len(), n, defs.len()*n);
        println!("  ok={n_ok}  diff={n_diff}  interp-panic={n_ipanic}  tier0-panic={n_t0panic}  skip(mem)={n_skip}");
        if n_t0panic > 0 {
            println!("  ── tier-0 unwired ops (the coverage frontier) ──");
            for (msg, c) in &t0panic_by { println!("    {c:4}× {msg}"); }
        }
        if n_diff > 0 {
            println!("  ── diffs by def ──");
            for (name, c) in &diff_by_def { println!("    {c:4}× {name}"); }
        }
        return;
    }

    // --fuzz [N] — for each of the 344 defs' mask/match: synthesize N random-fielded
    // valid encodings + random pre-state, diff interp vs silicon. The exec-truth ladder
    // (my day-1's census-diff loop, applied to semantics instead of decode).
    #[cfg(target_arch = "aarch64")]
    if args.get(1).map(|s| s.as_str()) == Some("--fuzz") {
        let n: usize = args.get(2).and_then(|s| s.parse().ok()).unwrap_or(3);
        let seed: u64 = args.get(3).and_then(|s| s.parse().ok()).unwrap_or(0xC0FFEE);
        let stub = native_oracle::NativeStub::new();
        let mut mem = FlatMem::new(0x10000, 0x10000);
        // Walk mask/match set from lib.rs (same as corpus mode) + capture def NAME too
        let src = include_str!("lib.rs");
        let mut defs = vec![];
        let mut cur_name = "";
        for line in src.lines() {
            if let Some(n) = line.trim().strip_prefix("/* ").and_then(|s| s.strip_suffix(" */")) {
                cur_name = n;
            }
            if let Some(rest) = line.trim().strip_prefix("if (insn & 0x") {
                let mask_end = rest.find(')').unwrap();
                let mask = u32::from_str_radix(&rest[..mask_end], 16).unwrap();
                let ms = rest[mask_end..].find("0x").unwrap() + mask_end + 2;
                let me = rest[ms..].find(' ').unwrap() + ms;
                let mat = u32::from_str_radix(&rest[ms..me], 16).unwrap();
                defs.push((cur_name.to_string(), mask, mat));
            }
        }
        // Reproducible PRNG (xorshift64 — no dep). Seeded → same corpus every run.
        let mut rng = seed;
        let mut rand = || { rng ^= rng<<13; rng ^= rng>>7; rng ^= rng<<17; rng };
        let (mut n_ok, mut n_diff, mut n_skip, mut n_ipanic, mut n_reject) = (0usize, 0usize, 0usize, 0usize, 0usize);
        let mut ipanic_by: std::collections::BTreeMap<String, usize> = Default::default();
        let mut diff_by_def: std::collections::BTreeMap<String, usize> = Default::default();
        let mut reject_by_def: std::collections::BTreeMap<String, usize> = Default::default();
        // v1 def-level exclusions (oracle-limitations, not semantics bugs):
        //   - vec-* / F* defs: stub doesn't load V-regs yet (‡ v2: LDR/STR Q0-Q31)
        //   - defs whose gpr-or-sp operand can be rn=31: native reads real host SP.
        //     Coarse filter: exclude any triple where ANY 5-bit field == 31.
        let vec_def = |n: &str| n.starts_with('F') || n.contains("vector") || n.contains("VEC")
            || n.contains("SIMD") || matches!(n, "DUP-general"|"UMOV"|"INS-general"|"INS-element"
                |"MOVI"|"MVNI"|"SCVTF-scalar-integer"|"UCVTF-scalar-integer")
            // SIMD LOADS/STORES: Rt is a V-register. HISTORY: before the stub saved d8-d15
            // (AAPCS64 callee-saved low-64), executing LD1 on silicon clobbered release-mode
            // Rust's own locals — the NEXT def's `(0..0x1000).collect()` returned an EMPTY
            // Vec while a fresh `.count()` in the same panic message said 4096. That host-
            // safety hole is FIXED in the stub (native_oracle.rs, str_d/ldr_d prologue pair).
            // v3: the stub NOW marshals full V0-V31 (ldr_q/str_q, flat[32..96]) and the
            // diff compares all 32 as u128 — so the SIMD LD/ST families are UN-excluded.
            // The F-prefix/vector-arith exclusions above remain pending an interp-semantics
            // census (two-step deliberately: this fire's diffs attribute to ONE family).
            // (the "bare-body" exclusion that lived here was STALE the day it landed:
            // all 10 families were among the parser-drop's 30 restored defs — their
            // semantics sat at def[7] all along, and "bare" was the DROP's symptom,
            // not the .isa's state. Un-excluded once the fold fix landed; the fuzz
            // census is the arbiter now.)
            // BARE STORE-EXCLUSIVE: without a paired load-exclusive the status result is
            // architecturally UNPREDICTABLE (ARM ARM: software must not rely on it). The
            // interp models always-succeed (right for real paired code, the only shape the
            // JIT emits); this silicon fires them bare. Measured on this core: STLXR/STLXRB
            // fail ~21/24 bare while STXR/STXRB happen to succeed — micro-arch luck, not
            // contract, so ALL four are excluded rather than keeping the two lucky ones.
            || matches!(n, "STLXR"|"STLXRB"|"STXR"|"STXRB");
        // XF_ONLY=<def-name>: run a single def in isolation. Bisect lever for state carried
        // across defs (longjmp residue, V-reg clobber, allocator damage): if a def fails in
        // the full run and passes alone, the killer is upstream, not the def.
        let only = std::env::var("XF_ONLY").ok();
        // XF_SKIP_TO=<def>: skip everything alphabetically before <def>. With XF_ONLY's
        // isolation result (LD1-alone = clean) this pair binary-searches the poisoning def.
        let skip_to = std::env::var("XF_SKIP_TO").ok();
        let mut skipping = skip_to.is_some();
        for (name, mask, mat) in &defs {
            if skipping {
                if Some(name.as_str()) == skip_to.as_deref() { skipping = false; }
                else { continue; }
            }
            if let Some(o) = &only { if name != o { continue; } }
            if vec_def(name) { n_skip += n; continue; }  // ‡ v2: enable when stub loads V-regs
            for _ in 0..n {
                let mut fields = (rand() as u32) & !mask;
                // Force any 5-bit-aligned field ==31 → 30 (avoids SP-anchor collision).
                // Coarse; misses non-aligned reg-fields, but covers the common rd@0/rn@5/rm@16.
                for sh in [0, 5, 10, 16] {
                    if (fields >> sh) & 0x1F == 31 { fields &= !(1u32 << sh); }
                }
                let insn = mat | fields;
                // Random pre-state (x1-x28; leave x0/x29-x30/SP as 0 to reduce accidental
                // stub-frame corruption if a def slips the exclusion; NZCV random top-4).
                let mut pre = Aarch64State::default();
                for r in 1..=28 { pre.x[r] = rand(); }
                pre.nzcv = ((rand() as u32) & 0xF) << 28;
                // V pre-state (v3): random full-128. Read-modify lanes (INS-element, the
                // MLA/MLS families, narrowing-top forms writing the hi half) are invisible
                // against a zero pre-state — the day-3 law: every operand class needs its
                // own pre-state, or agreement is agreement-about-zero.
                for r in 0..32 { pre.v[r] = (rand() as u128) | ((rand() as u128) << 64); }
                // MEM ARM: for a load/store def, a RANDOM register is outside the 64KB arena
                // 598-of-835 times — which is why 623 panics over 109 distinct ld/st defs read
                // as a "harness limit" instead of as the coverage hole they are. So place EVERY
                // register at an arena-interior, 16-byte-aligned address: any of them can be the
                // base (the .isa's rn field varies per def) and pre/post-indexed forms write the
                // base back, so the arithmetic must stay in range afterwards too. Values still
                // vary — the low bits are random — so this constrains ADDRESSES, not DATA.
                // XF_NOMEM=1 disables the mem arm entirely = pre-change behaviour (ld/st stay
                // excluded). The bisect lever: a crash that survives XF_NOMEM is not mine.
                // The CASP family (CASP/CASPA/CASPL/CASPAL) is BARE-BODY: the .isa declares the
                // encoding and writes no semantics, so the interp panics while silicon happily
                // executes a real 16-byte pair-CAS against the arena. Running the mem arm on a
                // def whose interp side cannot participate buys no diff and costs a crash.
                // XF_CASP=1 re-includes them (for when the semantics land).
                // TWO SEPARATE DECISIONS, conflated in v1 and that conflation left 148 arena-oob:
                //  place_in_arena — should this case's BASE REGISTERS point into the arena?
                //  mem_arm        — should the SILICON side execute and be diffed?
                // A def whose interp side can't participate (bare-body CASP*, intrinsic-lowered
                // LDXR/LDAXB, PC-relative *-literal) still needs in-arena bases, or the interp
                // indexes FlatMem with a random u64 and panics `arena-oob` — which reads as a
                // coverage gap when it is only an unplaced base.
                let casp = name.starts_with("CASP");
                let place_in_arena = ldst_shaped(name) && !env_on("XF_NOMEM");
                // PC-RELATIVE LITERALS never silicon-exec: EA = stub-pc + imm19, unmapped
                // no matter where the arena sits (the sig=11 "rejects" were this — a
                // harness limit mislabeled as .isa over-permissiveness). They still get
                // arena bases (the interp side needs them); the third population the
                // TWO-DECISIONS comment names, now encoded.
                let literal = name.ends_with("-literal");
                // CASP* re-included by default (2026-08-17): all four siblings carry
                // the CASPAL-transcribed body now; XF_CASP=0 excludes if ever needed.
                let mem_arm = place_in_arena && is_ldst(name) && stub.arena_ok() && !literal
                    && !(casp && std::env::var("XF_CASP").map(|v| v=="0").unwrap_or(false));
                // MEASURED (--fuzz 6, ×2): gating this on `place_in_arena` instead of `mem_arm`
                // recovers 53 arena-oob and costs +276 diff / -292 ok. The extra 11 defs are not
                // the cost — pointing a base into the arena changes what SILICON does for the 99
                // ALREADY-WORKING defs as well, because a base that used to be random (and got
                // rejected) now resolves. So the residual 148 arena-oob is HONEST: those defs'
                // interp side cannot participate (bare-body CASP*, intrinsic-lowered exclusives,
                // PC-relative literals), and forcing them into the arena buys diffs, not coverage.
                if mem_arm {
                    // Keep well inside on BOTH ends. The headroom must cover the WIDEST access
                    // any ld/st def makes from a base, not just an imm9: LD4-multi touches 64
                    // bytes, LDP-simd a 16-byte pair at base+imm7*16 (±1008), and postindex forms
                    // WRITE THE BASE BACK, so the next access can start further along. An 0x1000
                    // reservation at the top let an interp read hit index==len exactly (a panic
                    // at interp.rs:746, my own harness rather than the .isa). 0x2000 both ends.
                    const LO: u64 = native_oracle::ARENA_BASE + 0x2000;
                    const HI: u64 = native_oracle::ARENA_BASE + native_oracle::ARENA_SIZE as u64 - 0x2000;
                    for r in 1..=28 { pre.x[r] = LO + ((rand() % ((HI - LO) / 16)) * 16); }
                    // REGISTER-OFFSET forms (LDR/STR/LDRB/…-register): EA = base + INDEX-reg
                    // (possibly extended/scaled ≤×8). With every reg arena-interior, base+index
                    // ≈ 2×arena = ALWAYS out of range — the whole family verified ZERO triples
                    // (ok=0 hides inside a global diff=0; the per-def census is the only view
                    // that sees it). The rm FIELD is bits 16-20 uniformly across the family:
                    // pin that register to a small even index (0..255, ×8-scale-safe wherever
                    // the base sits ≥0x2000 from either end). SXTW/UXTW extends of a small
                    // positive value are identity, so every option arm stays in range.
                    if name.ends_with("-register") {
                        let rm = ((insn >> 16) & 0x1F) as usize;
                        if (1..=28).contains(&rm) { pre.x[rm] = (rand() % 128) * 2; }
                    }
                    // SEED BOTH SIDES **BEFORE** EITHER RUNS. This was below the interp call for
                    // one fire and produced 51 GPR-only "diffs" (plain LDR/LDP/LDAR): the interp
                    // loaded a zeroed FlatMem while silicon loaded the pattern, so the diff was
                    // about seed-ORDER, not semantics. `pat` is derived from the insn, so a real
                    // diff stays reproducible from the seed.
                    let pat: Vec<u8> = (0..0x1000u32).map(|i| (i.wrapping_mul(31) ^ insn) as u8).collect();
                    stub.reset_arena(&pat);
                    // TILE THE INTERP SIDE THE SAME WAY reset_arena TILES THE SILICON SIDE.
                    // This loop used to write only `pat.len()` (0x1000) bytes while reset_arena
                    // tiles all of ARENA_SIZE (0x10000) — so every byte above 0x1000 held pattern
                    // on silicon and ZERO in the interp. Bases sit at +0x2000 and above, so the
                    // whole diff-window was in the asymmetric region: 104 GPR-only "diffs" on
                    // STUR/STLR/STP/STRB where the dump showed hundreds of bytes differing for a
                    // 4-byte store, none of them at the store's own address. Same defect as the
                    // reset_arena seed-window, on the other side of the pair.
                    // GUARD with a record: this once returned EMPTY in release builds only —
                    // guest SIMD ops (LD1/CMEQ-scalar/CNT) clobbered host d8-d15 (AAPCS64
                    // callee-saved), where LLVM kept the collect's live state. Fixed in the
                    // stub (save/restore d8-d15, native_oracle.rs). If this ever fires again,
                    // suspect a NEW class of host-state the stub doesn't preserve (FPCR? SVE?).
                    assert!(!pat.is_empty(),
                        "pat EMPTY after {name} 0x{insn:08X}: host V-reg/FP state clobbered?");
                    for i in 0..native_oracle::ARENA_SIZE {
                        // w=8 is EIGHT BITS = one byte (interp.rs:744-746, n=(w+7)/8); v is u128.
                        mem.write(native_oracle::ARENA_BASE + i as u64, 8, pat[i % pat.len()] as u128);
                    }
                }
                // Interp side (may panic on unwired intrinsic / unreachable-match / todo-wmask).
                let ir = std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
                    interp_one(&pre, &mut mem, insn, 0x1000).0
                }));
                let i_post = match ir {
                    Ok(s) => s,
                    Err(e) => {
                        // Tally the panic's CLASS per def. A bare count can't distinguish
                        // "1 def x130" from "130 defs x1", and the classes have different
                        // owners: arena-oob = the 64KB fuzz arena (a HARNESS limit),
                        // unreachable = a generated match with no arm for a field value
                        // (an .isa coverage question), intrinsic-unwired = deliberate policy,
                        // not-decoded = a decode gap. Payloads are &str OR String; check both.
                        let msg = e.downcast_ref::<&str>().map(|s| s.to_string())
                            .or_else(|| e.downcast_ref::<String>().cloned())
                            .unwrap_or_default();
                        if env_on("XF_PANICS") {
                            eprintln!("PANIC {name} insn=0x{insn:08X} :: {}", msg.lines().next().unwrap_or(""));
                        }
                        let class = if msg.contains("index out of bounds") { "arena-oob" }
                            else if msg.contains("unreachable") { "unreachable" }
                            else if msg.contains("not wired") { "intrinsic-unwired" }
                            else if msg.contains("not decoded") { "not-decoded" }
                            else { "other" };
                        *ipanic_by.entry(format!("{class}\t{name}")).or_insert(0usize) += 1;
                        n_ipanic += 1; continue;
                    }
                };
                let mut n_post = pre.clone();
                // Silicon's arena must start byte-identical to the interp's, or a load-diff is
                // about the arena's history instead of the semantics. `pat` is deterministic
                // per-case (derived from the insn) so a diff is reproducible from the seed.
                // (arena seeded above, BEFORE interp_one — see the ordering note there)
                // XF_TRACE=1 names the def on STDERR before the silicon call. A segfault discards
                // buffered STDOUT, so the per-def tally cannot report which case killed the run —
                // the last stderr line can. (Two wrong guesses at this crash before I did this.)
                if env_on("XF_TRACE") {
                    eprintln!("TRACE {name} insn=0x{insn:08X} mem_arm={mem_arm}");
                }
                // Assert the addressability contract for exactly this case, then restore — a
                // leaked assertion would let a RANDOM-register case through excluded()'s ld/st
                // gate and store to a wild address on real silicon.
                let prev_mem_ok = native_oracle::set_mem_addressable(mem_arm);
                // Name the insn BEFORE feeding it to silicon — if the stub segfaults,
                // the last stderr line names the killer (v1 debug; v2 = signal handler).
                let nr = stub.exec_one(&mut n_post, insn);
                native_oracle::set_mem_addressable(prev_mem_ok);
                match nr {
                    native_oracle::NativeResult::Excluded => { n_skip += 1; continue; }
                    native_oracle::NativeResult::SiliconRejects(sig) => {
                        // .isa accepted (interp didn't panic) but silicon trapped = a
                        // missing `requires` in the .isa. Tally by def.
                        n_reject += 1;
                        *reject_by_def.entry(format!("{name} (sig={sig})")).or_default() += 1;
                        if env_on("XF_REJECTS") {
                            eprintln!("REJECT {name} insn=0x{insn:08X} sig={sig}");
                        }
                        continue;
                    }
                    native_oracle::NativeResult::Ran => {}
                }
                let mut d = false;
                for r in 0..31 { if i_post.x[r] != n_post.x[r] { d = true; break; } }
                if (i_post.nzcv & 0xF0000000) != (n_post.nzcv & 0xF0000000) { d = true; }
                // V-REG post-state (v3): without this an LD1/SIMD-op writing the wrong
                // lanes passes — nothing downstream reads the V it wrote. Full u128.
                if !d { for r in 0..32 { if i_post.v[r] != n_post.v[r] { d = true; break; } } }
                // MEMORY post-state. A GPR-only compare passes a STORE that wrote the wrong
                // bytes to the right address, or the right bytes to the wrong one — the exact
                // class this tier exists to catch, and invisible for as long as ld/st was
                // excluded wholesale. Compare the whole arena: a narrow window lets a stale
                // byte outside it masquerade as agreement (the sweep's own harness-lesson).
                if mem_arm && !d {
                    let n_arena = stub.arena_snapshot();
                    for i in 0..n_arena.len() {
                        let ib = mem.read(native_oracle::ARENA_BASE + i as u64, 8) as u8;
                        if ib != n_arena[i] { d = true; break; }
                    }
                }
                if d {
                    n_diff += 1; *diff_by_def.entry(name.clone()).or_default() += 1;
                    // First diff for this def → dump the reproducer.
                    if diff_by_def[name] == 1 {
                        eprintln!("DIFF {name} insn=0x{insn:08X}:");
                        for r in 0..31 { if i_post.x[r] != n_post.x[r] {
                            eprintln!("    x{r}: interp=0x{:X} native=0x{:X} (pre=0x{:X})",
                                i_post.x[r], n_post.x[r], pre.x[r]); } }
                        if (i_post.nzcv & 0xF0000000) != (n_post.nzcv & 0xF0000000) {
                            eprintln!("    nzcv: interp=0x{:08X} native=0x{:08X} (pre=0x{:08X})",
                                i_post.nzcv, n_post.nzcv, pre.nzcv); }
                        for r in 0..32 { if i_post.v[r] != n_post.v[r] {
                            eprintln!("    v{r}: interp=0x{:032X} native=0x{:032X} (pre=0x{:032X})",
                                i_post.v[r], n_post.v[r], pre.v[r]); } }
                        // MEMORY detail. Without this a store-diff prints NO field at all — the
                        // registers agree by construction for a store, so the dump was empty and
                        // "104 GPR-only diffs" was unreadable. Print the first disagreeing bytes
                        // with their address, and say how many differ in total: one byte at one
                        // address is a value bug, a run of them is a width or endianness bug, and
                        // two addresses far apart is the wrong-address class.
                        if mem_arm {
                            let n_arena = stub.arena_snapshot();
                            let mut shown = 0; let mut total = 0;
                            for i in 0..n_arena.len() {
                                let ib = mem.read(native_oracle::ARENA_BASE + i as u64, 8) as u8;
                                if ib != n_arena[i] {
                                    total += 1;
                                    if shown < 8 {
                                        eprintln!("    mem[0x{:X}]: interp=0x{:02X} native=0x{:02X}",
                                            native_oracle::ARENA_BASE + i as u64, ib, n_arena[i]);
                                        shown += 1;
                                    }
                                }
                            }
                            if total > shown { eprintln!("    … {} bytes differ in total", total); }
                        }
                        // dump pre-state args for repro
                        let regs: Vec<_> = (1..=28).map(|r| format!("x{r}=0x{:X}", pre.x[r])).collect();
                        eprintln!("    repro: --native-diff {} nzcv=0x{:X} 0x{insn:08X}",
                            regs.join(" "), pre.nzcv);
                    }
                }
                else { n_ok += 1; }
            }
        }
        // BUILD-STAMP: a runtime string, not a comment. Comments never reach a release binary, so
        // `strings <bin> | grep <comment>` returns 0 for a FRESH build too — it cannot answer "is
        // this binary my source?". This line can: bump it with any behavioural change and a stale
        // binary is visible in its own first line of output.
        println!("[fuzz build: memarm-v3-tiled-interp-seed]");
        println!("[fuzz: {} defs × {} = {} triples]", defs.len(), n, defs.len()*n);
        println!("  ok={n_ok}  diff={n_diff}  silicon-rejects={n_reject}  skip(v1-excl)={n_skip}  interp-panic={n_ipanic}");
        if env_on("XF_PANIC_BY") {
            let mut by_class: std::collections::BTreeMap<&str, usize> = Default::default();
            for (k, v) in &ipanic_by { *by_class.entry(k.split('\t').next().unwrap()).or_insert(0) += v; }
            println!("  interp-panic by CLASS:");
            for (c, n) in &by_class { println!("    {n:5}  {c}"); }
            // Per-def breakdown for EVERY class, not just `unreachable`. The v1 form hardcoded
            // that one filter, so any claim about another class (e.g. "the 330 arena-oob are the
            // mem defs, a harness limit") had no per-def evidence available and could not be
            // checked at all — a tally whose decomposition the printer refuses to emit is a
            // number, not a finding. XF_PANIC_CLASS=<substr> narrows; default prints all.
            let want = std::env::var("XF_PANIC_CLASS").unwrap_or_default();
            for (cls, _) in &by_class {
                if !want.is_empty() && !cls.contains(&want) { continue; }
                let mut v: Vec<_> = ipanic_by.iter()
                    .filter(|(k, _)| k.split('\t').next().unwrap() == *cls).collect();
                v.sort_by_key(|(_, n)| std::cmp::Reverse(**n));
                println!("  {} — {} distinct defs (top 20):", cls, v.len());
                for (k, n) in v.iter().take(20) { println!("    {n:5}  {}", k.split('\t').nth(1).unwrap()); }
            }
        }
        if n_reject > 0 {
            println!("  ── silicon-rejects (.isa over-permissive) ──");
            for (name, c) in &reject_by_def { println!("    {c:4}× {name}"); }
        }
        if n_diff > 0 {
            println!("  ── diffs by def ──");
            for (name, c) in &diff_by_def { println!("    {c:4}× {name}"); }
        }
        return;
    }

    // --tier0-diff <hex-insn> [x<N>=<hex>...] — compile one insn via Tier0, execute the
    // JIT'd machine-code, diff post-state vs InterpretingBuilder. THE tier-0 oracle
    // (per DESIGN.md: "tier-0 vs interpreter → state diff = 0").
    #[cfg(target_arch = "aarch64")]
    if args.get(1).map(|s| s.as_str()) == Some("--tier0-diff") {
        use sharpretro_jit::tier0::{Tier0, STATE_WORDS};
        let mut pre = Aarch64State::default();
        let mut insns = vec![];
        for a in &args[2..] {
            if let Some((r, v)) = a.split_once('=') {
                let val = u64::from_str_radix(v.trim_start_matches("0x"), 16).unwrap();
                if let Some(n) = r.strip_prefix('x') { pre.x[n.parse::<usize>().unwrap()] = val; }
                else if r == "nzcv" { pre.nzcv = val as u32; }
            } else {
                insns.push(u32::from_str_radix(a.trim_start_matches("0x"), 16).unwrap());
            }
        }
        let mut mem = FlatMem::new(0x10000, 0x10000);
        let (mut ok, mut diffs) = (0, 0);
        for &insn in &insns {
            // interp side
            let (i_post, _) = interp_one(&pre, &mut mem, insn, 0x1000);
            // tier-0 side: compile the ONE insn, exec against a flat state array
            let mut t0 = Tier0::new();
            let decoded = std::panic::catch_unwind(std::panic::AssertUnwindSafe(||
                recompile_one(&mut t0, insn, 0x1000)));
            match decoded {
                Ok(true) => {},
                Ok(false) => { println!("0x{insn:08X}  not decoded"); continue; }
                Err(_) => { println!("0x{insn:08X}  tier-0 PANIC (unwired op)"); continue; }
            }
            let block = t0.finalize();
            let mut flat = [0u64; STATE_WORDS];
            for i in 0..32 { flat[i] = pre.x[i]; }
            flat[32] = pre.nzcv as u64;
            flat[33] = 0x1000;  // pc
            block.exec(&mut flat);
            // diff GPR + nzcv + pc
            let mut d = vec![];
            for r in 0..31 { if i_post.x[r] != flat[r] {
                d.push(format!("x{r}: interp=0x{:X} tier0=0x{:X}", i_post.x[r], flat[r])); } }
            if (i_post.nzcv & 0xF0000000) != ((flat[32] as u32) & 0xF0000000) {
                d.push(format!("nzcv: interp=0x{:08X} tier0=0x{:08X}", i_post.nzcv, flat[32] as u32));
            }
            // tier-0 doesn't advance pc for non-branching insns (that's the block-driver's
            // job — recompile_one compiles ONE insn; the driver bumps pc if !branched).
            // Only diff pc when the interp branched (= .isa emitted a `branch` head).
            let interp_branched = i_post.pc != 0x1004;  // interp_one sets pc=pc+4 if !branched
            if interp_branched && i_post.pc != flat[33] {
                d.push(format!("pc: interp=0x{:X} tier0=0x{:X}", i_post.pc, flat[33]));
            }
            if d.is_empty() {
                println!("0x{insn:08X}  ✓ (tier0 == interp)  [{} host-insns, {} slots]",
                    block.code_len / 4, block.n_slots);
                if std::env::var("TIER0_DUMP").is_ok() {
                    std::fs::write("/tmp/tier0_block.bin", block.code_bytes()).unwrap();
                    eprintln!("(dumped {} bytes → /tmp/tier0_block.bin)", block.code_len);
                }
                ok += 1;
            } else {
                println!("0x{insn:08X}  ✗ DIFF:");
                for l in &d { println!("    {l}"); }
                diffs += 1;
            }
        }
        println!("[tier0-diff: {ok} match, {diffs} diff]");
        return;
    }

    #[cfg(target_arch = "aarch64")]

    // --native-diff <hex-insn> [x<N>=<hex>...] — run one insn on BOTH the
    // InterpretingBuilder AND real silicon (NativeStub), diff the post-states.
    // The exec-truth oracle: silicon = the independent verifier (interp+recompiler
    // are co-blind to .isa/emit bugs; silicon isn't).
    #[cfg(target_arch = "aarch64")]
    if args.get(1).map(|s| s.as_str()) == Some("--native-diff") {
        let mut pre = Aarch64State::default();
        let mut insns = vec![];
        for a in &args[2..] {
            if let Some((r, v)) = a.split_once('=') {
                let val = u64::from_str_radix(v.trim_start_matches("0x"), 16).unwrap();
                if let Some(n) = r.strip_prefix('x') { pre.x[n.parse::<usize>().unwrap()] = val; }
                else if r == "nzcv" { pre.nzcv = val as u32; }
            } else {
                insns.push(u32::from_str_radix(a.trim_start_matches("0x"), 16).unwrap());
            }
        }
        let stub = native_oracle::NativeStub::new();
        let mut mem = FlatMem::new(0x10000, 0x10000);
        let mut ok = 0; let mut skip = 0; let mut diffs = 0;
        for &insn in &insns {
            let (i_post, _) = interp_one(&pre, &mut mem, insn, 0x1000);
            let mut n_post = pre.clone();
            match stub.exec_one(&mut n_post, insn) {
                native_oracle::NativeResult::Excluded => {
                    println!("0x{insn:08X}  SKIP (branch/load-store/system/pc-dep — v1 exclusion)");
                    skip += 1; continue;
                }
                native_oracle::NativeResult::SiliconRejects(sig) => {
                    println!("0x{insn:08X}  SILICON-REJECTS (sig={sig}) — .isa accepted, silicon trapped");
                    skip += 1; continue;
                }
                native_oracle::NativeResult::Ran => {}
            }
            // diff x[0..31] + nzcv (SP/pc excluded — stub doesn't model them)
            let mut d = vec![];
            for r in 0..31 { if i_post.x[r] != n_post.x[r] {
                d.push(format!("x{r}: interp=0x{:X} native=0x{:X}", i_post.x[r], n_post.x[r])); } }
            if (i_post.nzcv & 0xF0000000) != (n_post.nzcv & 0xF0000000) {
                d.push(format!("nzcv: interp=0x{:08X} native=0x{:08X}", i_post.nzcv, n_post.nzcv));
            }
            if d.is_empty() {
                println!("0x{insn:08X}  ✓ (match)");
                ok += 1;
            } else {
                println!("0x{insn:08X}  ✗ DIFF:");
                for l in &d { println!("    {l}"); }
                diffs += 1;
            }
        }
        println!("[native-diff: {ok} match, {diffs} diff, {skip} skip]");
        return;
    }

    if args.len() >= 2 {
        let insn = u32::from_str_radix(args[1].trim_start_matches("0x"), 16).expect("hex insn");
        let pc = args.get(2).map(|s| u64::from_str_radix(s.trim_start_matches("0x"), 16).unwrap()).unwrap_or(0x1000);
        let mut b = RecordingBuilder::new();
        let ok = recompile_one(&mut b, insn, pc);
        println!("insn=0x{:08X} pc=0x{:X} decoded={}", insn, pc, ok);
        println!("{}", b.dump());
        return;
    }

    // Corpus mode: walk the source of lib.rs itself for `if (insn & MASK) == MATCH` lines
    // (each = one def). Cheap, no separate table needed — the mask/match set is right there.
    let src = include_str!("lib.rs");
    let mut n_defs = 0; let mut n_ok = 0; let mut n_lines = 0usize;
    for line in src.lines() {
        if let Some(rest) = line.trim().strip_prefix("if (insn & 0x") {
            // parse `MASK) == 0xMATCH { 'decode: {`
            let mask_end = rest.find(')').unwrap();
            let _mask = u32::from_str_radix(&rest[..mask_end], 16).unwrap();
            let match_start = rest[mask_end..].find("0x").unwrap() + mask_end + 2;
            let match_end = rest[match_start..].find(' ').unwrap() + match_start;
            let mat = u32::from_str_radix(&rest[match_start..match_end], 16).unwrap();
            n_defs += 1;
            let mut b = RecordingBuilder::new();
            // synthesize: the match bits alone (fields all-zero). Some defs will `break 'decode`
            // on a `requires` (all-zero fields fail their constraint) — that's still a RUN.
            let ok = std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
                recompile_one(&mut b, mat, 0x1000)
            }));
            match ok {
                Ok(true) => { n_ok += 1; n_lines += b.log.len(); }
                Ok(false) => { /* mask/match dispatch fell through — shouldn't happen for match-bits */ }
                Err(_) => { eprintln!("PANIC on def#{} insn=0x{:08X}", n_defs, mat); }
            }
        }
    }
    println!("[gate-(b) corpus: {} defs, {} decoded-ok, {} IL-lines total, avg {:.1}/def]",
        n_defs, n_ok, n_lines, n_lines as f64 / n_ok.max(1) as f64);
}
