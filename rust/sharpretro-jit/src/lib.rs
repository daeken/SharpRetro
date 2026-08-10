//! sharpretro-jit — the tiered-JIT runtime crate.
//!
//! Consumes ArchCompiler's `Backends/Rust`-generated `recompiler.rs` (per guest-ISA,
//! tier-agnostic) via the `Builder` trait defined here. See `../DESIGN.md` for the
//! full design; this file is the trait skeleton + core types (rung-4 step ①).
//!
//! Reference shape = the C# `JitBase/IBuilder.cs` + `IRuntimeValue.cs`. The method
//! set below is transcribed from those, not composed — the freeze-oracle discipline
//! applies here too (the generated recompiler.rs must call the SAME operations the
//! C# Recompiler.cs calls, so the trait's vocabulary must match IBuilder's exactly).

#![allow(dead_code)]

pub mod recording;
pub mod il_record;
pub mod regalloc;
pub mod interp;
#[cfg(target_arch = "aarch64")]
pub mod aarch64_enc;
pub mod x64_enc;
#[cfg(target_arch = "aarch64")]
pub mod tier0;
pub mod tier1;
#[cfg(target_arch = "aarch64")]
pub mod block_cache;

// ─────────────────────────────────────────────────────────────────────────────
// Types
// ─────────────────────────────────────────────────────────────────────────────

/// IL type lattice — mirrors ArchCompilerCore's `EType`. Every `Val` carries one.
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum IlType {
    /// Integer: signed?, width in bits (1..=128).
    I { signed: bool, width: u8 },
    /// Float: width in bits (16/32/64).
    F { width: u8 },
    /// 128-bit vector (lanes typed at operation-time, matching C# Vector128<T>).
    V128,
    Bool,
    /// Void — statement-typed nodes only; never a Val's type.
    Unit,
}

impl IlType {
    pub const U8:  Self = Self::I { signed: false, width: 8 };
    pub const U16: Self = Self::I { signed: false, width: 16 };
    pub const U32: Self = Self::I { signed: false, width: 32 };
    pub const U64: Self = Self::I { signed: false, width: 64 };
    pub const I8:  Self = Self::I { signed: true,  width: 8 };
    pub const I16: Self = Self::I { signed: true,  width: 16 };
    pub const I32: Self = Self::I { signed: true,  width: 32 };
    pub const I64: Self = Self::I { signed: true,  width: 64 };
    pub const F32: Self = Self::F { width: 32 };
    pub const F64: Self = Self::F { width: 64 };
}

/// Guest register-file identifier. Concrete files declared per-guest-ISA (aarch64: Gpr/Vec/
/// Nzcv/Sr; x86: Gpr/Eflags/Seg/Xmm). The tier is opaque to which files exist — it just
/// indexes `state.regs[file as usize][idx]` at the offset the frontend declared.
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub struct RegFile(pub u8);

/// A block-local mutable slot (the `let`/`mlet` binding in the .isa; `IBuilder.DefineLocal`).
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub struct LocalId(pub u32);

/// Index into the `NativeTable` (guest imports / IAT slots — loader-populated).
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub struct NativeSlot(pub u32);

/// Index into the `IntrinsicTable` (arch contract-intrinsics — svc/load-exclusive/etc).
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub struct IntrinsicId(pub u32);

/// Guest calling convention — how the GUEST placed args (matters at `call_native` when
/// the call site is a guest `call [IAT+N]` and args are already in guest-ABI reg slots).
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum GuestAbi {
    MsX64,   // rcx, rdx, r8, r9, [rsp+0x20..]
    SysV,    // rdi, rsi, rdx, rcx, r8, r9, [rsp..]
    Aapcs64, // x0-x7, [sp..]
}

#[derive(Debug, Clone)]
pub struct Signature {
    pub args: Vec<IlType>,
    pub ret: Option<IlType>,
    pub guest_abi: GuestAbi,
}

// ─────────────────────────────────────────────────────────────────────────────
// The Builder trait
// ─────────────────────────────────────────────────────────────────────────────

/// The tier-agnostic emit interface. Generated `recompiler.rs` calls these; each tier
/// (template / linear-scan / LLVM) implements them.
///
/// `Val` is opaque per-tier: a scratch-reg slot index (tier-0), a vreg id (tier-1),
/// an LLVMValueRef (tier-2). Every `Val` carries an `IlType` retrievable via `ty_of()`.
///
/// Method vocabulary transcribed from `JitBase/IRuntimeValue.cs` (the ~33 value-ops)
/// + `JitBase/IBuilder.cs` (control-flow + call). Rust convention: methods on the
/// builder taking Val args (not on Val itself), so tier state is always reachable.
pub trait Builder {
    type Val: Copy;

    fn ty_of(&self, v: Self::Val) -> IlType;

    // ── leaves ──────────────────────────────────────────────────────────────
    fn literal(&mut self, ty: IlType, bits: u128) -> Self::Val;
    fn reg_read(&mut self, file: RegFile, idx: u32, ty: IlType) -> Self::Val;
    fn reg_write(&mut self, file: RegFile, idx: u32, v: Self::Val);
    fn mem_read(&mut self, addr: Self::Val, ty: IlType) -> Self::Val;
    fn mem_write(&mut self, addr: Self::Val, v: Self::Val);

    // ── arithmetic (IRuntimeValue: Add/Sub/Mul/Div/Mod/Negate) ──────────────
    fn add(&mut self, a: Self::Val, b: Self::Val) -> Self::Val;
    fn sub(&mut self, a: Self::Val, b: Self::Val) -> Self::Val;
    fn mul(&mut self, a: Self::Val, b: Self::Val) -> Self::Val;
    fn div(&mut self, a: Self::Val, b: Self::Val) -> Self::Val;
    fn rem(&mut self, a: Self::Val, b: Self::Val) -> Self::Val;
    fn neg(&mut self, a: Self::Val) -> Self::Val;

    // ── bitwise (And/Or/Xor/Not/LeftShift/RightShift/ReverseBits/CountLeadingZeros) ──
    fn and(&mut self, a: Self::Val, b: Self::Val) -> Self::Val;
    fn or(&mut self, a: Self::Val, b: Self::Val) -> Self::Val;
    fn xor(&mut self, a: Self::Val, b: Self::Val) -> Self::Val;
    fn not(&mut self, a: Self::Val) -> Self::Val;
    fn shl(&mut self, a: Self::Val, b: Self::Val) -> Self::Val;
    /// Right-shift. Signedness of `a`'s IlType selects arithmetic vs logical.
    fn shr(&mut self, a: Self::Val, b: Self::Val) -> Self::Val;
    /// Rotate-right by `b` bits within `a`'s width.
    fn rotr(&mut self, a: Self::Val, b: Self::Val) -> Self::Val;
    fn rbit(&mut self, a: Self::Val) -> Self::Val;
    fn clz(&mut self, a: Self::Val) -> Self::Val;

    // ── compares (LT/LTE/EQ/NE/GTE/GT — signedness from operand IlType) → Bool ──
    fn eq(&mut self, a: Self::Val, b: Self::Val) -> Self::Val;
    fn ne(&mut self, a: Self::Val, b: Self::Val) -> Self::Val;
    fn lt(&mut self, a: Self::Val, b: Self::Val) -> Self::Val;
    fn le(&mut self, a: Self::Val, b: Self::Val) -> Self::Val;
    fn gt(&mut self, a: Self::Val, b: Self::Val) -> Self::Val;
    fn ge(&mut self, a: Self::Val, b: Self::Val) -> Self::Val;

    // ── casts (Cast/Bitcast/SignExt — the DSL's `cast`/`bitcast`/`signext` heads) ──
    /// Value-preserving cast (int↔int width/sign, int↔float, float↔float).
    fn cast(&mut self, a: Self::Val, to: IlType) -> Self::Val;
    /// Bit-pattern reinterpret (same width; e.g. f32↔u32).
    fn bitcast(&mut self, a: Self::Val, to: IlType) -> Self::Val;
    /// Sign-extend from `a`'s current width to `to` (which must be wider signed int).
    fn sext(&mut self, a: Self::Val, to: IlType) -> Self::Val;
    /// Assemble a u128 from two u64 halves (hi<<64 | lo). The Builder-level form
    /// of the `.isa`'s `(:` bit-concat, specialized to the 64+64→128 case that
    /// x86 DIV/MUL need. tier-0: 2-slot with hi@slot+1, lo@slot (no shl needed).
    fn pair128(&mut self, hi: Self::Val, lo: Self::Val) -> Self::Val;
    /// Extract low-64 of a u128. (cast to U64 does this too; explicit for symmetry.)
    fn lo64(&mut self, a: Self::Val) -> Self::Val { self.cast(a, IlType::U64) }
    /// Extract high-64 of a u128.
    fn hi64(&mut self, a: Self::Val) -> Self::Val;
    /// V128 interleave-low: for elem_bits ∈ {8,16,32,64}, result[2i]=a[i],
    /// result[2i+1]=b[i] over the LOW half's elements. = x86 PUNPCKL{BW,WD,DQ,QDQ}
    /// = aarch64 ZIP1. `hi=true` gives interleave-high (PUNPCKH*/ZIP2).
    fn vzip(&mut self, a: Self::Val, b: Self::Val, elem_bits: u32, hi: bool) -> Self::Val;
    /// Packed-float binary: per-lane float op on V128, elem_bits ∈ {32,64}
    /// (= 4×f32 or 2×f64), op ∈ {0=add,1=sub,2=mul,3=div}. MULPS/ADDPD/etc.
    fn vfbin(&mut self, a: Self::Val, b: Self::Val, elem_bits: u32, op: u32) -> Self::Val;
    /// Lane shuffle on V128: n=128/elem_bits lanes, result[i] = pick lane
    /// `(sel >> (i*bits_per_sel)) & mask` from `a` (i < n/2) or `b` (i ≥ n/2).
    /// SHUFPS: elem_bits=32, bits_per_sel=2, dst=a, src=b.
    /// SHUFPD: elem_bits=64, bits_per_sel=1 (2 lanes, sel is 2 bits).
    /// PSHUFD: elem_bits=32, a==b (both from src), 4 lanes.
    /// sel is compile-time-constant (Ib) — tier-0/1 branch on it in codegen.
    fn vshuf(&mut self, a: Self::Val, b: Self::Val, elem_bits: u32, sel: u32) -> Self::Val;
    /// PSHUFLW/PSHUFHW: shuffle 4× u16 words in ONE half of src per sel
    /// (4×2-bit lane indices, each ∈0..3 within that half). Other half
    /// COPIED from src unchanged. hi=false→PSHUFLW (shuffle words 0-3),
    /// hi=true→PSHUFHW (words 4-7). All from src (not RMW).
    fn vshufw(&mut self, src: Self::Val, sel: u32, hi: bool) -> Self::Val;
    /// Packed convert on V128. `kind`:
    ///   0 = CVTDQ2PS  (4× i32 → 4× f32)
    ///   1 = CVTTPS2DQ (4× f32 → 4× i32, truncate)
    ///   2 = CVTPS2PD  (low 2× f32 → 2× f64)
    ///   3 = CVTPD2PS  (2× f64 → low 2× f32, upper 64 zeroed)
    ///   4 = CVTDQ2PD  (low 2× i32 → 2× f64)
    /// ‡ CVTPS2DQ (round-per-MXCSR, not truncate) parked — needs FCVTNS or
    ///   FPCR-mode; games rarely rely on it (compilers emit CVTTPS2DQ for
    ///   int-cast). ‡ Overflow saturation differs (x86 → 0x80000000 indefinite,
    ///   ARM FCVTZS → saturate) — matters only at |f|>2^31.
    fn vcvt(&mut self, a: Self::Val, kind: u32) -> Self::Val;

    // ── float (Abs/Sqrt/Round*/Ceil/Floor/IsNaN) ────────────────────────────
    fn fabs(&mut self, a: Self::Val) -> Self::Val;
    fn fsqrt(&mut self, a: Self::Val) -> Self::Val;
    fn fround(&mut self, a: Self::Val, mode: RoundMode) -> Self::Val;
    fn fceil(&mut self, a: Self::Val) -> Self::Val;
    fn ffloor(&mut self, a: Self::Val) -> Self::Val;
    fn fisnan(&mut self, a: Self::Val) -> Self::Val;
    /// x86 CMPSS/CMPSD/CMPPS/CMPPD predicate compare → all-1s/all-0 mask
    /// at width `w` (32 or 64). pred (imm8[2:0]): 0=EQ 1=LT 2=LE 3=UNORD
    /// 4=NEQ 5=NLT 6=NLE 7=ORD. Preds 0-2 are ordered (NaN→false), 4-6
    /// are their inverses (NaN→true). a,b are F{w}-typed scalars.
    fn fcmpp(&mut self, a: Self::Val, b: Self::Val, pred: u32, w: u32) -> Self::Val;
    /// x86 MAXSS/MINSS semantics: `(a op b) ? a : b` with IEEE compare
    /// (NaN→false → returns b=src; ±0 equal → returns b=src). NOT the same
    /// as ARM FMAX/FMIN (which propagate NaN). is_max: true=MAX, false=MIN.
    fn fminmax(&mut self, a: Self::Val, b: Self::Val, is_max: bool) -> Self::Val;

    // ── vector (Element/ZeroTop/VectorSumUnsigned + the VectorMath heads) ───
    /// Read lane `i` of a V128 as `elem_ty`.
    fn velement_read(&mut self, v: Self::Val, i: Self::Val, elem_ty: IlType) -> Self::Val;
    /// Write lane `i` of a V128; returns the new V128.
    fn velement_write(&mut self, v: Self::Val, i: Self::Val, e: Self::Val) -> Self::Val;
    fn vzero_top(&mut self, v: Self::Val) -> Self::Val;
    // (The full VectorMath head-set — vector-{add,sub,mul,and,or,xor,not,shl,shr,cmp*,
    //  cvt*, min/max, abs, neg} — expands here at the same grain. Deferred to the
    //  first Backends/Rust generation pass so the trait-set matches what the .isa
    //  actually EMITS, per the throw-on-unhandled discipline: don't over-declare.)

    // ── control flow ────────────────────────────────────────────────────────
    /// Ends the current block. `link` = record return-address (BL/CALL).
    fn branch(&mut self, target: Self::Val, link: bool);
    /// If/else — both arms emitted. `then`/`else_` receive the same builder.
    /// Bounded-count loop: execute `body` `n` times. tier-0 emits an in-block
    /// loop (mov ctr,n; head: cbz ctr,exit; body; sub ctr,#1; b head; exit:).
    /// interp: for _ in 0..n { body(self) }. Body writes to state (rdi/rsi/mem);
    /// no cross-iter Val dataflow (each iter re-reads rdi from state).
    fn loop_n(&mut self, n: Self::Val, body: &mut dyn FnMut(&mut Self));
    fn cond(&mut self, c: Self::Val,
            then: &mut dyn FnMut(&mut Self), else_: &mut dyn FnMut(&mut Self));
    /// Value-typed conditional (the `ternary`/`if-expr` form).
    fn ternary(&mut self, c: Self::Val, a: Self::Val, b: Self::Val) -> Self::Val;

    fn local_new(&mut self, ty: IlType) -> LocalId;
    fn local_read(&mut self, l: LocalId) -> Self::Val;
    fn local_write(&mut self, l: LocalId, v: Self::Val);

    // ── guest→native boundary (§ DESIGN.md) ─────────────────────────────────
    fn call_native(&mut self, slot: NativeSlot, args: &[Self::Val]) -> Option<Self::Val>;
    fn call_intrinsic(&mut self, id: IntrinsicId, args: &[Self::Val]) -> Option<Self::Val>;

    // ── the escape hatch ────────────────────────────────────────────────────
    /// A guest instruction the recompiler declines to lower (marked `unimplemented` in
    /// the .isa, or a mode/feature not compiled in). Tier decides: bail-to-interpreter,
    /// or trap. Ends the block.
    fn unimplemented(&mut self, insn_name: &'static str);
}

/// Rounding mode for `fround` — collapses IRuntimeValue's Round/RoundTowardZero/
/// RoundHalfDown/RoundHalfUp into one method + a mode enum.
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum RoundMode {
    Nearest,       // RoundHalfUp equivalent (ties-to-even? — ‡ verify against C# Round)
    TowardZero,
    HalfDown,
    HalfUp,
}

// ─────────────────────────────────────────────────────────────────────────────
// GuestState
// ─────────────────────────────────────────────────────────────────────────────

/// Per-guest-thread execution state. `Regs` is guest-ISA-specific (declared by the
/// frontend's generated code alongside `recompiler.rs`); the runtime treats it as
/// an opaque byte-blob indexed by the `RegFile` offsets the frontend declared.
///
/// Under emulator-only (per DESIGN.md), guest never touches real host CPU state.
/// `gs_base`/`fs_base` are STRUCT FIELDS — guest `gs:[N]` compiles to
/// `mem_read(state.gs_base + N)`. No context-swap at the native boundary; only ABI-remap.
#[repr(C)]
pub struct GuestState<Regs> {
    pub regs: Regs,
    pub pc: u64,
    pub mode: u32,
    pub gs_base: u64,
    pub fs_base: u64,
    // exec_count for the CURRENT block, checked at block-tail for hotspot promotion.
    // (Design ‡: does the counter live here or in BlockEntry? BlockEntry per DESIGN.md;
    //  this field may drop. Kept as a placeholder for the tier-0 tail-check to bump.)
    pub _block_exec_hint: u32,
}

// ─────────────────────────────────────────────────────────────────────────────
// Placeholders (rung-4 steps ③-⑤ populate)
// ─────────────────────────────────────────────────────────────────────────────

/// Host-arch machine-code encoder. `Aarch64Emit` first; `X64Emit` when a consumer wants it.
pub trait Emit {
    // fn mov(&mut self, dst: HostReg, src: HostReg);  — etc. Populated at tier-0 build.
}

pub struct NativeTable {
    // slot → (fn_ptr, Signature). Loader-populated. See DESIGN.md §call_native.
}

pub struct IntrinsicTable {
    // id → (fn_ptr, Signature). Arch-runtime-populated (the contract-intrinsic impls).
}

pub struct BlockCache {
    // (pc, mode) → BlockEntry. See DESIGN.md §BlockCache.
}
