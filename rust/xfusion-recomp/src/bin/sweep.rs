//! `--gen-sweep`: exhaustive silicon-sweep corpus generator.
//!
//! Consumes SWEEP_DEFS + the encoder in sweep.rs, enumerates every phase-1
//! def × encoding-choice × pre-state boundary-grid, emits X64D rows for the
//! EC2 ptrace runner. Every emitted encoding is round-trip-verified.
//!
//! Output rows carry the interp's expected-post-state; the silicon runner
//! diffs actual-post vs that. A silicon-diff = either an interp bug (emit-
//! tier or .isa-tier) OR a hardware-behavior the .isa doesn't capture.

use std::io::Write;
use std::collections::BTreeMap;

use xfusion_recomp::sweep_defs::{SWEEP_DEFS, SwCls, SwW};
use xfusion_recomp::sweep::{phase1_skip, enumerate_p1, EncChoice, phase3_skip, enumerate_p3, verify_rt};
use xfusion_recomp::disassembler::{decode_insn, DEF_MNEMONICS};
use xfusion_recomp::decode::XMode;
use xfusion_recomp::state::{X86State, TrackingState};
use xfusion_recomp::x64_stub::{emit_stub, emit_stub_xmm, emit_stub_32, emit_stub_32_xmm};
use xfusion_recomp::sweep::MemChoice;
use xfusion_recomp::lift::{lift_one, DEF_FLAGS_MASK, FLAGS_ALL_LIVE};
use sharpretro_jit::interp::{InterpretingBuilder, FlatMem};

/// Boundary-value pre-state grid for GPR reads. Chosen for edge-coverage:
/// zero, one, all-ones, sign-bit per width (8/16/32/64), max-positive per
/// width, plus a couple mid-values so 2-arg diffs aren't degenerate.
static PRE_VALS: &[u64] = &[
    0, 1, 2, 0x7F, 0x80, 0xFF, 0x7FFF, 0x8000, 0xFFFF,
    0x7FFF_FFFF, 0x8000_0000, 0xFFFF_FFFF,
    0x7FFF_FFFF_FFFF_FFFF, 0x8000_0000_0000_0000, u64::MAX,
    0x1122_3344_5566_7788, 0xDEAD_BEEF_CAFE_BABE,
];

/// XMM pre-state boundary grid, as u128 bit-patterns. Each 128-bit value
/// packs 4×f32 (or 2×f64) at boundary points. Chosen so per-lane values
/// hit: zero, ±1, ±inf, NaN, denormal, sign-bit, largest-normal, and non-
/// float integer patterns (for PADD*/PXOR/etc). One anchor + one sweep-
/// value per XMM read, mirroring the GPR pre-state shape.
static PRE_VALS_XMM: &[u128] = &[
    0,                                                          // all-zero
    0x3F800000_3F800000_3F800000_3F800000,                      // 4× f32 1.0
    0xBF800000_BF800000_BF800000_BF800000,                      // 4× f32 -1.0
    0x7F800000_FF800000_7FC00000_00000001,                      // +inf,-inf,qNaN,denorm-min (f32 lanes)
    0x7F7FFFFF_00800000_80000000_40490FDB,                      // f32 max-norm,min-norm,-0,pi
    0x3FF00000_00000000_3FF00000_00000000,                      // 2× f64 1.0
    0x7FF00000_00000000_FFF00000_00000000,                      // f64 +inf,-inf
    0x7FF80000_00000000_00000000_00000001,                      // f64 qNaN, denorm-min
    0x7FEFFFFF_FFFFFFFF_80000000_00000000,                      // f64 max-norm, -0
    0x00010203_04050607_08090A0B_0C0D0E0F,                      // int-pattern (byte-ladder)
    0x80808080_80808080_80808080_80808080,                      // int sign-bit-per-byte
    0xFFFFFFFF_FFFFFFFF_FFFFFFFF_FFFFFFFF,                      // all-ones
    0x00000001_00000002_00000003_00000004,                      // 4× small-int-per-lane
];
/// XMM anchor: DIFFERENT from every PRE_VALS_XMM value + non-degenerate
/// per-lane (so 2-arg ops don't fold to 0).
const ANCHOR_XMM: u128 = 0x40800000_40400000_40000000_3F800000;  // f32 [1,2,3,4]

/// Interp one insn from a given pre-state. Returns (post_state, def_id) on
/// success; None if the lift panics (intrinsic/unwired/mem — phase-1 skip).
/// Phase-3 DATA_PAGE: the fixed low address every mem-form ea targets. Below
/// rsp=0x8FED8, MAP_32BIT-reachable, in the interp's FlatMem 0..0x90000.
pub const DATA_PAGE: u64 = 0x60000;

/// Fault-record rows (the fault-addr-record deliverable): fmask bit-31 set
/// ⟹ the row's EXPECTED outcome is a FAULT, not a post-state. post[88] =
/// expected signal (SIGFPE=8), post[89] = expected fault-rip offset from the
/// stub page (= SLOT_OFF: the fault fires AT the test insn). The runner's
/// child installs handlers, records {si_signo, rip−stub_page} into
/// state[88..90], and exits; parent compares those two slots ONLY.
/// v1 = #DE (DIV/IDIV divisor=0 + INT_MIN/−1) — deterministic, well-defined.
/// #PF/si_addr rows (mem-fault addresses) = v2 once mechanics prove.
pub const FMASK_FAULT_ROW: u32 = 0x8000_0000;
pub const SLOT_SIGNO: usize = 88;
pub const SLOT_FAULT_OFF: usize = 89;
/// mem window captured per-row (pre+post) = the WHOLE data page. First-fire
/// taught why 64B isn't enough: BT-family bit-string addressing reaches
/// ±bytes far beyond the operand (bts WORD w/ bitoff=0x7788 → ea+0xEF0,
/// in-page but outside a 64B window) → the runner's page kept stale bits
/// across triples → phantom CF diffs. Whole-page: runner memcpy resets ALL
/// 4096 per triple, memcmp catches ANY in-page write.
pub const MEM_LEN: usize = 4096;

/// The fault-addr-record generator (v1: #DE). For each DIV/IDIV encoding ×
/// op_w × rm-reg (a few, incl an r8-15), emit rows whose pre-state makes the
/// insn FAULT deterministically:
///   • divisor = 0                      → #DE (both DIV + IDIV)
///   • IDIV: dividend = INT_MIN, divisor = −1 → #DE (quotient unrepresentable)
///   • DIV: dividend ≥ divisor<<W (quotient overflow) → #DE
/// Expected outcome: SIGFPE at the test insn (fault_off = SLOT_OFF — x86
/// faults report the FAULTING insn's rip, not next-rip). fmask bit-31 marks
/// the row; post[88]=8 (SIGFPE), post[89]=SLOT_OFF.
fn gen_fault_rows(out_path: &str, mode: XMode) -> std::io::Result<()> {
    use std::io::Write;
    let is64 = mode == XMode::Bits64;
    let slot_off: u64 = if is64 { 82 } else { 29 };  // v1-stub / 32-bit-stub
    let mut w = std::io::BufWriter::new(std::fs::File::create(out_path)?);
    w.write_all(&0x44343658u32.to_le_bytes())?;  // X64D (state-layout rows)
    w.write_all(&0u32.to_le_bytes())?;           // count patched at end
    let mut n_rows = 0u32;

    // ── #PF rows: mem access to GUARD_VA (0x70000 — a page the runner's
    // child never maps; DATA_PAGE's neighbor +0x10000 so a data-page mapping
    // can't accidentally cover it). Expected: SIGSEGV w/ si_addr == the
    // exact ea. Three access kinds: load (MOV r,[m]), store (MOV [m],r),
    // rmw (ADD [m],r). slot89 = si_addr ABSOLUTE for SEGV/BUS (vs rip-offset
    // for FPE/ILL) — the runner's handler keys on the signal class.
    const GUARD_VA: u64 = 0x70000;
    let pf_defs: &[(&str, u8)] = &[("MOV", 0x8B), ("MOV", 0x89), ("ADD", 0x01), ("ADD", 0x03)];
    for &(mn, opc) in pf_defs {
        let sd = SWEEP_DEFS.iter()
            .find(|d| d.mnem == mn && d.opcode == opc && d.map == 0).unwrap();
        let opws: &[u8] = if is64 { &[16, 32, 64] } else { &[16, 32] };
        for &op_w in opws {
            // Two shapes: [base] and [base+idx*4+disp8] (SIB path faults too).
            let shapes = [
                MemChoice{base:1, index:-1, scale:1, disp:0, rip_rel:false},
                MemChoice{base:1, index:3,  scale:4, disp:8, rip_rel:false},
            ];
            for mc in &shapes {
                let c = EncChoice { mode, op_w, reg: 0, rm: 0, zopc: 0, imm: 0, mem: Some(*mc) };
                let insn = match verify_rt(sd, &c) { Ok(b) => b, Err(_) => continue };
                // Solve base/idx so ea = GUARD_VA exactly.
                let (bv, iv): (u64, u64) = if mc.index >= 0 {
                    let iv = 2u64;
                    (GUARD_VA - iv*(mc.scale as u64) - mc.disp as u64, iv)
                } else {
                    (GUARD_VA - mc.disp as u64, 0)
                };
                let mut pre = X86State::default();
                pre.gpr[4] = 0x8FED8;
                pre.eflags = 0x202;
                pre.gpr[0] = 0x1122_3344_5566_7788;   // src for stores/rmw
                pre.gpr[1] = bv;
                if mc.index >= 0 { pre.gpr[3] = iv; }
                if !is64 { for r in 0..8 { pre.gpr[r] &= 0xFFFF_FFFF; } }
                let mut post_flat = pre.to_flat();
                post_flat[SLOT_SIGNO] = 11;          // SIGSEGV
                post_flat[SLOT_FAULT_OFF] = GUARD_VA; // si_addr, absolute
                let (stub, _slot) = if is64 { emit_stub(&insn) } else { emit_stub_32(&insn) };
                let def_id = decode_insn(&insn, mode).map(|d| d.def_id).unwrap_or(0);
                w.write_all(&def_id.to_le_bytes())?;
                w.write_all(&FMASK_FAULT_ROW.to_le_bytes())?;
                w.write_all(&(stub.len() as u32).to_le_bytes())?;
                w.write_all(&stub)?;
                for v in &pre.to_flat() { w.write_all(&v.to_le_bytes())?; }
                for v in &post_flat { w.write_all(&v.to_le_bytes())?; }
                n_rows += 1;
            }
        }
    }

    for sd in SWEEP_DEFS {
        if !(sd.mnem == "DIV" || sd.mnem == "IDIV") { continue; }
        let signed = sd.mnem == "IDIV";
        let opws: &[u8] = if sd.ops[0].w == SwW::B { &[8] }
                          else if is64 { &[16, 32, 64] } else { &[16, 32] };
        let rms: &[u8] = if is64 { &[1, 3, 9] } else { &[1, 3] };  // rcx/rbx/r9
        for &op_w in opws {
            for &rm in rms {
                // op_w drives the ENCODING via EncChoice (Eb rows have op_w
                // fixed by the encoder's B-width path; pass 32 there).
                let c = EncChoice {
                    mode, op_w: if op_w == 8 { 32 } else { op_w },
                    reg: 0, rm, zopc: 0, imm: 0, mem: None,
                };
                let insn = match verify_rt(sd, &c) { Ok(b) => b, Err(_) => continue };
                // Fault-classes: (divisor_val, rdx, rax) triples.
                // Divisor lives in gpr[rm] (Eb: the byte of it; fine — 0 is 0).
                let mut cases: Vec<(u64, u64, u64)> = vec![
                    (0, 0x5, 0x1234),                    // ÷0
                ];
                if signed {
                    // INT_MIN / −1 → #DE. dividend = rdx:rax sign-pattern:
                    // op_w=8: AX=0x8000? No — Eb: dividend=AX, INT_MIN=0x80,
                    // AH:AL... keep it simple: full-width INT_MIN via rdx:rax.
                    let (rdx, rax) = match op_w {
                        8  => (0, 0x8000u64),            // AX = i16::MIN? No:
                        // Eb IDIV: dividend = AX (16b), quotient → AL (8b).
                        // INT_MIN case: AX = 0x8000? / −1 → +0x8000 > i8 → #DE ✓
                        16 => (0x8000, 0),               // DX:AX = 0x8000_0000? No:
                        // Ev16: dividend = DX:AX (32b); DX=0x8000 AX=0 →
                        // 0x8000_0000 / −1 → +2^31 > i16::MAX → #DE ✓
                        32 => (0x8000_0000, 0),
                        64 => (0x8000_0000_0000_0000, 0),
                        _ => unreachable!(),
                    };
                    cases.push((u64::MAX, rdx, rax));    // ÷ −1 w/ min-dividend
                } else {
                    // DIV quotient-overflow: dividend = divisor << W exactly
                    // (quotient = 1<<W > max). divisor=1: rdx:rax = 1<<W ⟹
                    // rdx=1 rax=0 (or AH=1 for Eb — approximate w/ rdx=1;
                    // Eb's dividend is AX: AH=1 AL=0, divisor=1 → q=256 > 255 → #DE ✓
                    let (rdx, rax) = match op_w {
                        8  => (0, 0x100),                // AX = 0x100, ÷1 → 256 → #DE
                        _  => (1, 0),                    // rdx:rax = 1<<W ÷ 1 → #DE
                    };
                    cases.push((1, rdx, rax));
                }
                for &(dv, rdx, rax) in &cases {
                    let mut pre = X86State::default();
                    pre.gpr[4] = 0x8FED8;
                    pre.eflags = 0x202;
                    pre.gpr[0] = rax;
                    pre.gpr[2] = rdx;
                    pre.gpr[rm as usize] = dv;
                    if !is64 {
                        for r in 0..8 { pre.gpr[r] &= 0xFFFF_FFFF; }
                        for r in 8..16 { pre.gpr[r] = 0; }
                    }
                    let mut post = pre.clone();  // regs unchanged at fault
                    let mut post_flat = post.to_flat();
                    post_flat[SLOT_SIGNO] = 8;             // SIGFPE
                    post_flat[SLOT_FAULT_OFF] = slot_off;  // fault AT the insn
                    let _ = &mut post;
                    let (stub, _slot) = if is64 { emit_stub(&insn) } else { emit_stub_32(&insn) };
                    let def_id = decode_insn(&insn, mode).map(|d| d.def_id).unwrap_or(0);
                    w.write_all(&def_id.to_le_bytes())?;
                    w.write_all(&FMASK_FAULT_ROW.to_le_bytes())?;
                    w.write_all(&(stub.len() as u32).to_le_bytes())?;
                    w.write_all(&stub)?;
                    for v in &pre.to_flat() { w.write_all(&v.to_le_bytes())?; }
                    for v in &post_flat { w.write_all(&v.to_le_bytes())?; }
                    n_rows += 1;
                }
            }
        }
    }
    let mut f = w.into_inner()?;
    use std::io::Seek;
    f.seek(std::io::SeekFrom::Start(4))?;
    f.write_all(&n_rows.to_le_bytes())?;
    println!("  fault rows emitted: {n_rows}");
    println!("  → {out_path}");
    Ok(())
}

fn interp_one(pre: &X86State, insn: &[u8], mode: XMode,
              pre_mem: Option<&[u8; MEM_LEN]>)
    -> Option<(X86State, u32, [u8; MEM_LEN])>
{
    let d = decode_insn(insn, mode)?;
    let mut st = pre.clone();
    st.rip = 0x1000;
    // Reg-form rows: FlatMem covers 0..0x90000 (PUSH/POP touch stack even
    // in reg-form; any access beyond → index-panic → caught → skip).
    // Mem-form rows: FlatMem = EXACTLY the silicon mapping (the one data
    // page at 0x60000). Any ea outside it — wild bit-string offsets, huge
    // pre-val indexes, negative reaches below the page — panics → row
    // SKIPPED → silicon never sees an unmappable address. First-fire's 465
    // REJECTs (child SIGSEGV/hang on wild ea) were rows interp emitted
    // because its 0x90000 arena accepted addresses silicon had no mapping
    // for. The interp arena must model the silicon mapping exactly.
    let mut mem = if let Some(pm) = pre_mem {
        let mut m = FlatMem::new(DATA_PAGE, MEM_LEN);
        m.bytes.copy_from_slice(pm);
        m
    } else {
        FlatMem::new(0, 0x90000)
    };
    let handled = std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
        let mut b = InterpretingBuilder::new(&mut st, &mut mem, 0x1000);
        b.intrinsic = |_,_,id,_| panic!("intrinsic {id}");
        lift_one(&mut b, &d, 0x1000, mode, FLAGS_ALL_LIVE)
    })).ok()?;
    if !handled { return None; }
    st.rip = 0x1000 + d.len as u64;
    let mut post_mem = [0u8; MEM_LEN];
    if pre_mem.is_some() {
        post_mem.copy_from_slice(&mem.bytes);   // narrow arena = the page itself
    }
    Some((st, d.def_id, post_mem))
}

/// Phase-3 solve-backward: given a MemChoice + target offset K in DATA_PAGE,
/// derive (base_val, index_val) such that effective_addr = DATA_PAGE + K.
/// Returns None for silicon-unfireable shapes (base/idx = rsp — stub doesn't
/// load rsp; rip_rel — stub page addr non-deterministic v1).
///
/// ea = (base_val if base>=0 else 0) + (index_val*scale if index>=0 else 0) + disp
fn solve_mem(mc: &MemChoice, k: u32) -> Option<(u64, u64)> {
    // rsp(4) not loaded by any stub (both modes' anchor). idx=4 unencodable
    // anyway (bare 100=none), but base=4 IS encodable — just not fireable.
    if mc.base == 4 || mc.index == 4 { return None; }
    // rip-rel: ea = rip_after + disp; rip on silicon = stub_page + SLOT_OFF
    // + insn_len (varies by mmap); on interp = 0x1000 + insn_len. Diverge.
    // ‡ Fireable via MAP_FIXED stub page — v2.
    if mc.rip_rel { return None; }
    let target = DATA_PAGE.wrapping_add(k as u64);
    let (bv, iv) = match (mc.base >= 0, mc.index >= 0) {
        (true, true) => {
            // Pick a small index_val; solve base_val.
            let iv = 3u64;
            let bv = target
                .wrapping_sub(iv.wrapping_mul(mc.scale as u64))
                .wrapping_sub(mc.disp as i64 as u64);
            (bv, iv)
        }
        (true, false) => {
            (target.wrapping_sub(mc.disp as i64 as u64), 0)
        }
        (false, true) => {
            // ea = iv*scale + disp. Solve iv = (target − disp)/scale.
            let num = target.wrapping_sub(mc.disp as i64 as u64);
            if num % (mc.scale as u64) != 0 { return None; }
            (0, num / (mc.scale as u64))
        }
        (false, false) => {
            // ea = disp (absolute). Only fireable if disp already = target.
            // The shape table's two no-base-no-idx entries have disp=0x60000
            // = DATA_PAGE, so K=0 works; other K don't (encoding-fixed disp).
            if mc.disp as i64 as u64 != target { return None; }
            (0, 0)
        }
    };
    // base==index (e.g. [rax+rax*2]): both must equal the SAME value. Only
    // works if bv==iv from the solve above; usually won't. For v1, refuse
    // (the shape-table doesn't emit base==index anyway; a full base×idx
    // sweep would need this).
    if mc.base >= 0 && mc.base == mc.index && bv != iv { return None; }
    Some((bv, iv))
}

/// Discover an insn's read-set via TrackingState (which GPRs + which flags).
/// Phase-1: skip if reads XMM/mem (reg-form GPR only).
fn discover(insn: &[u8], allow_xmm: bool, mode: XMode)
    -> Option<(Vec<u32>, Vec<u32>, Vec<u32>, u32)>
{
    let d = decode_insn(insn, mode)?;
    let mut ts = TrackingState::default();
    ts.inner.gpr[4] = 0x8FED8;
    ts.inner.rip = 0x1000;
    // Non-degenerate XMM anchors so ops that read xmm don't fold to 0/NaN
    // during discovery (some lifts branch on the value — e.g. CMPPS pred).
    for x in ts.inner.xmm.iter_mut() { *x = ANCHOR_XMM; }
    let mut mem = FlatMem::new(0, 0x90000);
    let handled = std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
        let mut b = InterpretingBuilder::new(&mut ts, &mut mem, 0x1000);
        b.intrinsic = |_,_,id,_| panic!("intrinsic {id}");
        lift_one(&mut b, &d, 0x1000, mode, FLAGS_ALL_LIVE)
    })).ok()?;
    if !handled { return None; }
    let xr = ts.xmm_reads();
    if !allow_xmm && !xr.is_empty() { return None; }
    Some((ts.gpr_reads(), ts.flag_reads(), xr, d.def_id))
}

/// Mnemonic-based exec-skip: defs unsafe to fire in the stub REGARDLESS of
/// choice. Checked BEFORE discover() (many of these panic in the interp,
/// which would otherwise land in track-fail with no attribution).
fn mnem_exec_skip(mnem: &str) -> Option<&'static str> {
    match mnem {
        // rip/rsp-changing: stub doesn't survive.
        "JMP" | "CALL" | "RET" | "RETI" | "RETF" | "IRET" | "LEAVE"
        | "ENTER" | "PUSH" | "POP" | "PUSHF" | "POPF" => Some("stack/branch"),
        // div/idiv: divisor=0 or INT_MIN/-1 → #DE. ‡ phase-2 fault-record
        // will let these through with a signal-catch; v1 skips.
        "DIV" | "IDIV" => Some("div-fault-v1"),
        // MXCSR not modeled (STMXCSR stores constant 0x1F80; LDMXCSR from a
        // pattern-page #GPs on silicon — reserved bits set). MXCSR isn't in
        // the compared state, so the row proves nothing. Was the p3-full
        // 525-reject residue (mem-form LDMXCSR of 0xCCCCCCCC).
        "LDMXCSR" | "STMXCSR" => Some("mxcsr-unmodeled"),
        // deliberate fault / trap
        "INT" | "INT3" | "INTO" | "UD0" | "UD1" | "UD2" | "ICEBP" => Some("trap"),
        // privileged / host-query / model-specific
        "IN" | "OUT" | "INS" | "OUTS" | "HLT" | "CLI" | "STI" | "WAIT"
        | "RDMSR" | "WRMSR" | "CPUID" | "RDTSC" | "RDTSCP" | "RDPMC"
        | "XGETBV" | "XSETBV" | "SYSCALL" | "SYSRET" | "SYSENTER" | "SYSEXIT"
        | "INVLPG" | "INVD" | "WBINVD" | "CLTS" | "LGDT" | "LIDT" | "LLDT"
        | "LTR" | "LMSW" | "SGDT" | "SIDT" | "SLDT" | "STR" | "SMSW"
        | "VERR" | "VERW" | "LAR" | "LSL" | "ARPL" | "CLAC" | "STAC"
        | "SWAPGS" | "RDGSBASE" | "RDFSBASE" | "WRGSBASE" | "WRFSBASE"
            => Some("priv/hostq"),
        // Not-yet-lifted (intrinsic in .isa — the sweep tests them when they
        // ARE lifted). Attributing here rather than track-fail so the census
        // reads honest. SHLD/SHRD/RCL/RCR = 50,048 of the 51,600 track-fails.
        "SHLD" | "SHRD" | "RCL" | "RCR" => Some("intrinsic-not-lifted"),
        // XCHG mem-form is implicitly LOCK'd; reg-form is fine but with
        // rm=rsp it touches the anchor. Handled by rsp-read check below.
        _ => None,
    }
}

/// Post-discover exec-skip: choice-dependent exclusions (need the read-set).
///   - rsp(4) as an operand: stub uses rsp as anchor; rsp-write breaks it.
///     ‡ phase-1.5: rsp-safe stub variant (state-ptr in TLS not stack).
fn exec_skip(_d: &xfusion_recomp::sweep_defs::SwDef, _c: &EncChoice,
             gpr_reads: &[u32]) -> Option<&'static str> {
    if gpr_reads.contains(&4) { return Some("rsp-read"); }
    None
}

fn main() {
    // Silence panic-noise: intrinsic/mem-touch panics are CAUGHT and counted;
    // the default hook prints to stderr regardless (thousands of lines).
    std::panic::set_hook(Box::new(|_| {}));
    let args: Vec<_> = std::env::args().collect();
    let out_path = args.iter().position(|a| a == "-o")
        .and_then(|i| args.get(i+1)).cloned()
        .unwrap_or("/tmp/sweep_p1.x64d".into());
    // Density knobs (for a fast smoke vs the full corpus). Full = both None.
    let max_pre: Option<usize> = args.iter().position(|a| a == "--max-pre")
        .and_then(|i| args.get(i+1)).and_then(|s| s.parse().ok());
    // --limit N: cap total rows (for first-fire pipeline-verify; runner is
    // fork-per-stub w/ ~2ms poll granularity → 10K rows ≈ 20-40s smoke).
    let row_limit: Option<u32> = args.iter().position(|a| a == "--limit")
        .and_then(|i| args.get(i+1)).and_then(|s| s.parse().ok());
    // --stride N: sample every Nth encoding (breadth-over-depth: hits every
    // def with fewer rows per; --limit alone stops early = only first defs).
    let stride: u32 = args.iter().position(|a| a == "--stride")
        .and_then(|i| args.get(i+1)).and_then(|s| s.parse().ok()).unwrap_or(1);
    let census_only = args.iter().any(|a| a == "--census");
    // --faults: the fault-addr-record arm (standalone generator — the row
    // space is small + the outcome contract is different). v1 = #DE.
    if args.iter().any(|a| a == "--faults") {
        let mode = if args.iter().any(|a| a == "--bits32") { XMode::Bits32 } else { XMode::Bits64 };
        gen_fault_rows(&out_path, mode).expect("fault-gen");
        return;
    }
    // --xmm: phase-2 mode. Enables XMM-reading defs (v1 skips via discover),
    // uses emit_stub_xmm (v2, SLOT_OFF=226, movdqu load/store around slot),
    // sweeps xmm_reads through PRE_VALS_XMM. Off = phase-1 (v1 stub, GPR only).
    let phase2_xmm = args.iter().any(|a| a == "--xmm");
    // --bits32: ① 32-bit-mode arm. Encodes/decodes/lifts at Bits32 (no REX,
    // reg 0..8, opws {16,32}, byte-idx 4-7 = AH/CH/DH/BH). Uses emit_stub_32
    // (85B, edi=anchor); runner detects by stub_len<100 → run_child_32().
    let mode = if args.iter().any(|a| a == "--bits32") { XMode::Bits32 } else { XMode::Bits64 };
    // --bits32 --xmm: p2-32. emit_stub_32_xmm (213B, movdqu xmm0-7 around
    // slot). xmm8-15 unreachable (no REX) — enumerate caps reg range at 8.
    // --mem: phase-3 mem-form. Walks enumerate_p3 (addressing-mode shapes)
    // instead of enumerate_p1; rows carry [pre_mem:64][post_mem:64] after the
    // state blocks (X64E magic so the runner knows). GPR-only v1 (no --xmm
    // composition yet); Bits64 + Bits32 both work (32-bit stub loads the
    // solved base/idx values from state like any other GPR).
    let phase3_mem = args.iter().any(|a| a == "--mem");
    if phase3_mem && phase2_xmm {
        eprintln!("--mem --xmm not yet composed (xmm-mem rows = later)");
        std::process::exit(1);
    }

    let mut n_defs_p1 = 0u32;
    let mut n_enc = 0u32;
    let mut n_rows = 0u32;
    let mut skip_by: BTreeMap<&str, u32> = BTreeMap::new();
    let mut per_def_rows: BTreeMap<&str, u32> = BTreeMap::new();
    let mut tf_by: BTreeMap<&str, u32> = BTreeMap::new();
    let mut tf_ex: BTreeMap<&str, Vec<u8>> = BTreeMap::new();

    let mut fw: Option<std::io::BufWriter<std::fs::File>> = if !census_only {
        let f = std::fs::File::create(&out_path).expect("create corpus");
        let mut w = std::io::BufWriter::new(f);
        // 'X64D' = state-only rows; 'X64E' = rows carry [pre_mem][post_mem].
        let magic: u32 = if phase3_mem { 0x45343658 } else { 0x44343658 };
        w.write_all(&magic.to_le_bytes()).unwrap();
        // Runner header = [u32 magic][u32 n]. We don't know n up-front (skip
        // predicates prune), so write a placeholder and seek-back to patch it.
        w.write_all(&0u32.to_le_bytes()).unwrap();
        Some(w)
    } else { None };

    for (i, sd) in SWEEP_DEFS.iter().enumerate() {
        let pskip = if phase3_mem { phase3_skip(sd, mode) } else { phase1_skip(sd, mode) };
        if let Some(r) = pskip { *skip_by.entry(r).or_default() += 1; continue; }
        if let Some(r) = mnem_exec_skip(sd.mnem) { *skip_by.entry(r).or_default() += 1; continue; }
        // Def has ANY xmm-class operand? (read OR write side). Own #171:
        // the encoder change let XMM defs pass phase1_skip; without --xmm
        // they'd enumerate then reject at discover (94K track-fail noise),
        // and GPR→XMM defs (MOVD-X/CVTSI2SD: read GPR, WRITE xmm) would slip
        // through onto v1 stubs that can't capture the XMM write. Def-level
        // skip when !--xmm keeps phase-1 corpus byte-identical to pre-change.
        let def_has_xmm = sd.ops.iter().any(|o|
            matches!(o.cls, SwCls::Vxmm | SwCls::Wxmm | SwCls::Uxmm));
        if def_has_xmm && !phase2_xmm {
            *skip_by.entry("xmm(use --xmm)").or_default() += 1; continue;
        }
        // Phase-3 v1 extra skips: LEA (Erm w=N mem_only — computes the ea,
        // no mem access; interp handles it but its "mem" is address-arith —
        // include: it READS no mem so pre/post-mem trivially match; actually
        // KEEP it, ea-arith is exactly what mem-form sweeps test).
        n_defs_p1 += 1;

        // For each ENCODING (reg-choice + opsize + imm), verify_rt-checked:
        if row_limit.map(|l| n_rows >= l).unwrap_or(false) { break; }
        let mut enc_i = 0u32;
        let enum_fn: fn(&xfusion_recomp::sweep_defs::SwDef, XMode,
                        &mut dyn FnMut(&EncChoice, &[u8])) -> (u32, u32) =
            if phase3_mem {
                |d, m, f| enumerate_p3(d, m, f)
            } else {
                |d, m, f| enumerate_p1(d, m, f)
            };
        let (ok, _fail) = enum_fn(sd, mode, &mut |c: &EncChoice, insn: &[u8]| {
            n_enc += 1;
            enc_i += 1;
            if stride > 1 && (enc_i % stride) != 1 { return; }
            if row_limit.map(|l| n_rows >= l).unwrap_or(false) { return; }
            // Discover read-set for THIS specific encoding (reg-choice determines
            // which GPRs the insn reads — add r8,r9 reads {r8,r9}).
            let Some((gpr_reads, flag_reads, xmm_reads, def_id)) = discover(insn, phase2_xmm, mode) else {
                *skip_by.entry("track-fail").or_default() += 1;
                *tf_by.entry(sd.mnem).or_default() += 1;
                if !tf_ex.contains_key(sd.mnem) { tf_ex.insert(sd.mnem, insn.to_vec()); }
                return;
            };
            if let Some(r) = exec_skip(sd, c, &gpr_reads) {
                *skip_by.entry(r).or_default() += 1; return;
            }
            let flags_mask = DEF_FLAGS_MASK.get(def_id as usize).copied().unwrap_or(0);

            // Pre-state grid: sweep ONE read-reg through PRE_VALS, others=anchor.
            // Same shape as --boundary v4. Plus flag-state variation if flag_reads.
            let anchor = 0x1122_3344_5566_7788u64;
            let flag_states: &[u32] = if flag_reads.is_empty() { &[0x202] }
                                       else { &[0x202, 0x202 | 0x8D5] };
            let pre_vals = &PRE_VALS[..max_pre.unwrap_or(PRE_VALS.len())];
            let xmm_pre_vals: &[u128] = if xmm_reads.is_empty() { &[0] }
                else { &PRE_VALS_XMM[..max_pre.unwrap_or(PRE_VALS_XMM.len()).min(PRE_VALS_XMM.len())] };
            // Stub choice: v2 (movdqu-wrapped) whenever XMM state matters —
            // read OR write. Own #172: !xmm_reads.is_empty() misses write-only
            // defs (MOVD xmm,r32 reads GPR only → xmm_reads=[] → v1 stub →
            // silicon xmm-write uncaptured). def_has_xmm covers both.
            let use_v2_stub = phase2_xmm && def_has_xmm;

            for &fs in flag_states {
                for &sweep_reg in gpr_reads.iter().chain(if gpr_reads.is_empty() {
                    // 0-read insns (CLC/STC/CLD etc) still get one row.
                    [0u32].iter().take(1)
                } else { [0u32].iter().take(0) }) {
                    for &bv in pre_vals {
                        let mut pre = X86State::default();
                        for &r in &gpr_reads { pre.gpr[r as usize] = anchor; }
                        if !gpr_reads.is_empty() { pre.gpr[sweep_reg as usize] = bv; }
                        // rsp = value with NONZERO low-16 (0xFED8): a 16-bit
                        // sp write of ANY value 0..0xFED7 or 0xFED9.. changes
                        // rsp → guard catches it. Fire-7's 30 hangs = bsf/bsr
                        // sp,X where result=0 → sp=0 → rsp=0x80000 unchanged
                        // (low-16 already 0) → guard passed → silicon hung.
                        pre.gpr[4] = 0x8FED8;
                        pre.eflags = fs;
                        // Bits32: mask pre-GPRs to u32 (r8-r15 don't exist →
                        // zero). Both silicon (32-bit stores low-32 only,
                        // high-32 stays = pre = 0) and interp (32-zext →
                        // high=0) then produce identical u64 post-values.
                        // Also: interp at Bits32 with a 16-bit dest mask-
                        // inserts into a u64 whose high-32 = pre-high-32 = 0.
                        if mode == XMode::Bits32 {
                            for r in 0..8 { pre.gpr[r] &= 0xFFFF_FFFF; }
                            for r in 8..16 { pre.gpr[r] = 0; }
                        }

                    // XMM sweep dimension: for each xmm-read, sweep it through
                    // PRE_VALS_XMM with others=ANCHOR_XMM. If no xmm reads,
                    // this loop runs once (bxv=0 sentinel; xmm[] left default).
                    let xmm_sweep: Vec<(u32,u128)> = if xmm_reads.is_empty() {
                        vec![(u32::MAX, 0)]
                    } else {
                        xmm_reads.iter().flat_map(|&xr|
                            xmm_pre_vals.iter().map(move |&xv| (xr, xv))).collect()
                    };
                    for &(sweep_xr, bxv) in &xmm_sweep {
                        // Load XMM pre-state (only when phase-2 + xmm reads exist).
                        if !xmm_reads.is_empty() {
                            for &xr in &xmm_reads { pre.xmm[xr as usize] = ANCHOR_XMM; }
                            pre.xmm[sweep_xr as usize] = bxv;
                        }

                        // Phase-3 mem-form: solve base/idx values so ea =
                        // DATA_PAGE + K, override the anchor-filled GPRs, and
                        // build the pre-mem window. K sweeps a few offsets
                        // (0 / 8 / 0x30) — page-interior; boundary-cross = v2.
                        // Mem pre-bytes: two patterns (0xCC-fill, and the
                        // PRE_VALS-derived u64 at K) — the mem-side analogue
                        // of the GPR pre-grid, kept small (encoding-space ×
                        // shapes is already the phase-3 dimension).
                        let mem_rows: Vec<(u32, Box<[u8; MEM_LEN]>)> = if let Some(mc) = &c.mem {
                            let ks: &[u32] = if mc.base < 0 && mc.index < 0 { &[0] }
                                             else { &[0, 8, 0x30] };
                            let mut v = vec![];
                            for &k in ks {
                                if solve_mem(mc, k).is_none() { continue; }
                                let mut pm = Box::new([0xCCu8; MEM_LEN]);
                                pm[k as usize..k as usize+8]
                                    .copy_from_slice(&bv.to_le_bytes());
                                v.push((k, pm));
                            }
                            v
                        } else {
                            vec![(0, Box::new([0u8; MEM_LEN]))]  // reg-form: no mem window
                        };
                        if c.mem.is_some() && mem_rows.is_empty() {
                            *skip_by.entry("mem-unsolvable").or_default() += 1; continue;
                        }

                        for &(k, ref pm) in &mem_rows {
                        let mut pre = pre.clone();
                        let pre_mem_opt = if c.mem.is_some() {
                            let mc = c.mem.as_ref().unwrap();
                            let (bvv, ivv) = solve_mem(mc, k).unwrap();
                            if mc.base >= 0  { pre.gpr[mc.base as usize] = bvv; }
                            if mc.index >= 0 { pre.gpr[mc.index as usize] = ivv; }
                            // Bits32: solved values must fit u32 (DATA_PAGE
                            // =0x60000 does; negative-disp solves wrap — mask
                            // and verify the ea still lands right as u32 math).
                            if mode == XMode::Bits32 {
                                if mc.base >= 0  { pre.gpr[mc.base as usize] &= 0xFFFF_FFFF; }
                                if mc.index >= 0 { pre.gpr[mc.index as usize] &= 0xFFFF_FFFF; }
                            }
                            Some(pm)
                        } else { None };

                        let Some((post, _, post_mem)) = interp_one(&pre, insn, mode, pre_mem_opt.map(|v| &**v)) else {
                            *skip_by.entry("interp-panic").or_default() += 1; continue;
                        };
                        if post.gpr[4] != pre.gpr[4] {
                            *skip_by.entry("rsp-write").or_default() += 1; continue;
                        }

                        n_rows += 1;
                        *per_def_rows.entry(sd.mnem).or_default() += 1;

                        if let Some(f) = &mut fw {
                            let (stub, _slot) = if mode == XMode::Bits32 {
                                if use_v2_stub { emit_stub_32_xmm(insn) }
                                else { emit_stub_32(insn) }
                            } else if use_v2_stub {
                                emit_stub_xmm(insn)
                            } else {
                                emit_stub(insn)
                            };
                            f.write_all(&(def_id as u32).to_le_bytes()).unwrap();
                            f.write_all(&flags_mask.to_le_bytes()).unwrap();
                            f.write_all(&(stub.len() as u32).to_le_bytes()).unwrap();
                            f.write_all(&stub).unwrap();
                            for w in &pre.to_flat()  { f.write_all(&w.to_le_bytes()).unwrap(); }
                            for w in &post.to_flat() { f.write_all(&w.to_le_bytes()).unwrap(); }
                            if phase3_mem {
                                // X64E: [pre_mem:64][post_mem:64] after states.
                                f.write_all(pre_mem_opt.map(|p| &p[..]).unwrap_or(&[0u8; MEM_LEN])).unwrap();
                                f.write_all(&post_mem).unwrap();
                            }
                        }
                        }  // mem_rows
                    }  // xmm_sweep
                        // 0-read insns: one row is enough.
                        if gpr_reads.is_empty() { break; }
                    }
                }
            }
        });
        let _ = ok;
        if i % 40 == 0 && !census_only {
            eprintln!("  [{i}/{}] {} — n_enc={n_enc} n_rows={n_rows}",
                      SWEEP_DEFS.len(), sd.mnem);
        }
    }

    if let Some(mut f) = fw {
        use std::io::Seek;
        f.flush().unwrap();
        // Patch the row-count into the header @offset 4.
        let mut inner = f.into_inner().unwrap();
        inner.seek(std::io::SeekFrom::Start(4)).unwrap();
        inner.write_all(&n_rows.to_le_bytes()).unwrap();
        inner.sync_all().unwrap();
    }

    println!("── phase-1 sweep census ──");
    println!("  defs: {n_defs_p1}/{} phase-1-eligible", SWEEP_DEFS.len());
    println!("  encodings enumerated: {n_enc}");
    println!("  corpus rows emitted: {n_rows}");
    for (r, n) in &skip_by { println!("    skip {r}: {n}"); }
    if !tf_by.is_empty() {
        println!("  track-fail by mnem (§4 attribution):");
        let mut v: Vec<_> = tf_by.iter().collect();
        v.sort_by(|a,b| b.1.cmp(a.1));
        for (m, n) in v.iter().take(20) {
            let ex = tf_ex.get(**m).map(|b| format!("{b:02X?}")).unwrap_or_default();
            println!("    {m}: {n}  first-fail={ex}");
        }
    }
    if !census_only {
        let sz = std::fs::metadata(&out_path).map(|m| m.len()).unwrap_or(0);
        println!("  → {out_path} ({} bytes = {} MB)", sz, sz / (1<<20));
    }
    println!("  top defs by row-count:");
    let mut v: Vec<_> = per_def_rows.iter().collect();
    v.sort_by(|a,b| b.1.cmp(a.1));
    for (m, n) in v.iter().take(15) { println!("    {m}: {n}"); }
    let _ = DEF_MNEMONICS;
}
