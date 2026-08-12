//! TIER-TOP: lower a recorded region (IlRecorder ops) to LLVM IR, -O2 JIT it,
//! and wrap the result in the same entry ABI as tier-0/1 blocks
//! (`extern "C" fn(*mut u64 /*state*/, *mut u64 /*spill*/)` — spill unused
//! here; LLVM's own regalloc replaces the slot machinery).
//!
//! SEMANTICS SOURCE: transcribed op-by-op from tier1.rs's Emitter arms (the
//! silicon-verified reference) — NOT composed from memory. Width discipline
//! mirrors tier-1 exactly: integer vals live ZERO-EXTENDED in an i64 (or
//! i128 for wide); ops that produce w<64 mask after (except Shl, matching
//! tier-1's mask-exemption which relies on consumers masking); signedness is
//! applied AT the op (asr/sdiv/scmp) from the val's IlType.
//!
//! Floats: F32/F64 vals are kept as BITS in the i64 (same as state[] and
//! tier-1's X-reg convention); float ops bitcast → fop → bitcast back.
//!
//! Feature-gated (`--features llvm`); see Cargo.toml for the toolchain notes.

use crate::il_record::{IlOp, IlOpKind, IlRecorder};
use crate::IlType;
use crate::tier0::StateLayout;
use inkwell::builder::Builder as LBuilder;
use inkwell::context::Context;
use inkwell::execution_engine::ExecutionEngine;
use inkwell::module::Module;
use inkwell::values::{FunctionValue, IntValue, PointerValue};
use inkwell::{AddressSpace, IntPredicate, OptimizationLevel};

/// A JIT'd region: the ExecutionEngine owns the code; `entry` matches the
/// CompiledBlock ABI. Keep the engine alive as long as the fn-ptr lives.
pub struct LlvmBlock {
    // FIELD ORDER = DROP ORDER: the engine must drop BEFORE the context it
    // references (first SIGSEGV of this module was exactly this, in teardown
    // after the test's assertions had already passed).
    _ee: ExecutionEngine<'static>,
    _ctx: Box<Context>,
    pub entry: extern "C" fn(*mut u64, *mut u64),
}

struct Lower<'ctx, 'a> {
    ctx: &'ctx Context,
    module: &'a Module<'ctx>,
    b: LBuilder<'ctx>,
    f: FunctionValue<'ctx>,
    layout: &'static StateLayout,
    rec: &'a IlRecorder,
    /// SSA val → LLVM value (i64 for w≤64 int/float-bits; i128 for wide).
    vals: Vec<Option<IntValue<'ctx>>>,
    state: PointerValue<'ctx>,
    /// Cond stack: (else_bb, end_bb, in_else).
    conds: Vec<(inkwell::basic_block::BasicBlock<'ctx>, inkwell::basic_block::BasicBlock<'ctx>, bool)>,
    exit_bb: inkwell::basic_block::BasicBlock<'ctx>,
}

fn width_bits(ty: IlType) -> u32 {
    match ty {
        IlType::I { width, .. } => width as u32,
        IlType::F { width } => width as u32,
        IlType::Bool => 1,
        IlType::V128 => 128,
        _ => 64,
    }
}
fn is_signed(ty: IlType) -> bool { matches!(ty, IlType::I { signed: true, .. }) }

impl<'ctx, 'a> Lower<'ctx, 'a> {
    fn i64t(&self) -> inkwell::types::IntType<'ctx> { self.ctx.i64_type() }

    fn get(&self, v: u32) -> IntValue<'ctx> {
        self.vals[v as usize].expect("llvm-tier: use of undefined val")
    }
    fn put(&mut self, v: u32, val: IntValue<'ctx>) {
        if self.vals.len() <= v as usize { self.vals.resize(v as usize + 1, None); }
        self.vals[v as usize] = Some(val);
    }
    /// mask a value to `w` bits (zero-extend convention, tier-1's mask_to).
    fn mask_to(&self, v: IntValue<'ctx>, w: u32) -> IntValue<'ctx> {
        if w >= 64 { return v; }
        let m = self.i64t().const_int((1u64 << w).wrapping_sub(1), false);
        self.b.build_and(v, m, "m").unwrap()
    }
    /// state[word_off/8] pointer (byte offset like tier-1's state_off).
    fn state_ptr(&self, byte_off: u32) -> PointerValue<'ctx> {
        let idx = self.ctx.i32_type().const_int((byte_off / 8) as u64, false);
        unsafe { self.b.build_gep(self.i64t(), self.state, &[idx], "sp").unwrap() }
    }
    fn ld_state(&self, byte_off: u32) -> IntValue<'ctx> {
        self.b.build_load(self.i64t(), self.state_ptr(byte_off), "sl").unwrap().into_int_value()
    }
    fn st_state(&self, byte_off: u32, v: IntValue<'ctx>) {
        self.b.build_store(self.state_ptr(byte_off), v).unwrap();
    }
    /// host pointer for a guest address: state[off_membase] + addr.
    fn guest_ptr(&self, addr: IntValue<'ctx>) -> PointerValue<'ctx> {
        let mb = self.ld_state(self.layout.off_membase);
        let ha = self.b.build_int_add(mb, addr, "ha").unwrap();
        self.b.build_int_to_ptr(ha, self.ctx.ptr_type(AddressSpace::default()), "hp").unwrap()
    }

    fn lower_op(&mut self, op: &IlOp) {
        use IlOpKind::*;
        let ty = op.ty;
        let w = width_bits(ty);
        let i64t = self.i64t();
        match op.kind {
            Literal => {
                // wide literal support: i128 for w>64.
                if w > 64 {
                    let v = self.ctx.i128_type().const_int_arbitrary_precision(
                        &[op.imm as u64, (op.imm >> 64) as u64]);
                    self.put(op.out.unwrap(), v);
                } else {
                    let v = i64t.const_int(op.imm as u64, false);
                    let v = if w < 64 { self.mask_to(v, w) } else { v };
                    self.put(op.out.unwrap(), v);
                }
            }
            RegRead => {
                let f = (op.imm >> 32) as u8;
                let idx = op.imm as u32;
                let off = (self.layout.reg_off)(crate::RegFile(f), idx);
                let mut v = self.ld_state(off);
                if f == self.layout.flag_file && !(self.layout.flag_file == 2 && idx == 0) {
                    let bit = (self.layout.flag_bit)(idx);
                    let sh = i64t.const_int(bit as u64, false);
                    v = self.b.build_right_shift(v, sh, false, "fb").unwrap();
                    v = self.mask_to(v, 1);
                } else if w < 64 && (f == 0 || f == 3) {
                    v = self.mask_to(v, w);
                }
                self.put(op.out.unwrap(), v);
            }
            RegWrite => {
                let f = (op.imm >> 32) as u8;
                let idx = op.imm as u32;
                let off = (self.layout.reg_off)(crate::RegFile(f), idx);
                let xs = self.get(op.args[0]);
                if f == self.layout.flag_file && !(self.layout.flag_file == 2 && idx == 0) {
                    let bit = (self.layout.flag_bit)(idx);
                    let old = self.ld_state(off);
                    let clr = i64t.const_int(!(1u64 << bit), false);
                    let cleared = self.b.build_and(old, clr, "fc").unwrap();
                    let b0 = self.mask_to(xs, 1);
                    let sh = i64t.const_int(bit as u64, false);
                    let bv = self.b.build_left_shift(b0, sh, "fs").unwrap();
                    let nv = self.b.build_or(cleared, bv, "fo").unwrap();
                    self.st_state(off, nv);
                } else {
                    self.st_state(off, xs);
                }
            }
            MemRead => {
                let addr = self.get(op.args[0]);
                let p = self.guest_ptr(addr);
                let lt = match w { 8 => self.ctx.i8_type(), 16 => self.ctx.i16_type(),
                                   32 => self.ctx.i32_type(), _ => i64t };
                let raw = self.b.build_load(lt, p, "ml").unwrap().into_int_value();
                let v = if w < 64 { self.b.build_int_z_extend(raw, i64t, "mz").unwrap() } else { raw };
                self.put(op.out.unwrap(), v);
            }
            MemWrite => {
                let addr = self.get(op.args[0]);
                let v = self.get(op.args[1]);
                let vw = width_bits(self.rec.val_type(op.args[1]));
                let p = self.guest_ptr(addr);
                let tv: IntValue = match vw {
                    8 => self.b.build_int_truncate(v, self.ctx.i8_type(), "t8").unwrap(),
                    16 => self.b.build_int_truncate(v, self.ctx.i16_type(), "t16").unwrap(),
                    32 => self.b.build_int_truncate(v, self.ctx.i32_type(), "t32").unwrap(),
                    _ => v,
                };
                self.b.build_store(p, tv).unwrap();
            }
            Fence => {
                self.b.build_fence(inkwell::AtomicOrdering::SequentiallyConsistent, false, "f").unwrap();
            }
            MemRmwAtomic => {
                use inkwell::AtomicRMWBinOp::*;
                let addr = self.get(op.args[0]);
                let v = self.get(op.args[1]);
                let p = self.guest_ptr(addr);
                let lt = match w { 8 => self.ctx.i8_type(), 16 => self.ctx.i16_type(),
                                   32 => self.ctx.i32_type(), _ => i64t };
                let tv = if w < 64 { self.b.build_int_truncate(v, lt, "at").unwrap() } else { v };
                let binop = match op.imm as u8 { 0 => Add, 1 => Or, 2 => And, 3 => Xor, 4 => Xchg,
                                                 o => panic!("llvm rmw op {o}") };
                // imm op 2 = AND-of-NOT (BIC semantics per tier-1's mvn+ldclral)?
                // NO — tier-1's op-2 does mvn THEN ldclr (clear = and-not), so the
                // COMBINED effect = and(mem, v). LLVM's And on the raw v matches
                // the INTERPRETER's op-2 (and) — verify against interp.rs: op 2 =
                // And on the raw value. tier-1's mvn+ldclral = and(mem, ~~v) = same.
                let old = self.b.build_atomicrmw(binop, p, tv,
                    inkwell::AtomicOrdering::SequentiallyConsistent).unwrap();
                let old64 = if w < 64 { self.b.build_int_z_extend(old, i64t, "az").unwrap() } else { old };
                self.put(op.out.unwrap(), old64);
            }
            MemCasAtomic => {
                let addr = self.get(op.args[0]);
                let exp = self.get(op.args[1]);
                let newv = self.get(op.args[2]);
                let p = self.guest_ptr(addr);
                let lt = match w { 8 => self.ctx.i8_type(), 16 => self.ctx.i16_type(),
                                   32 => self.ctx.i32_type(), _ => i64t };
                let (e, n) = if w < 64 {
                    (self.b.build_int_truncate(exp, lt, "ce").unwrap(),
                     self.b.build_int_truncate(newv, lt, "cn").unwrap())
                } else { (exp, newv) };
                let r = self.b.build_cmpxchg(p, e, n,
                    inkwell::AtomicOrdering::SequentiallyConsistent,
                    inkwell::AtomicOrdering::SequentiallyConsistent).unwrap();
                let old = self.b.build_extract_value(r, 0, "co").unwrap().into_int_value();
                let old64 = if w < 64 { self.b.build_int_z_extend(old, i64t, "cz").unwrap() } else { old };
                self.put(op.out.unwrap(), old64);
            }
            Add | Sub | Mul | Div | Rem | And | Or | Xor | Shl | Shr | Rotr => {
                let a = self.get(op.args[0]);
                let bb = self.get(op.args[1]);
                let signed = is_signed(ty);
                // Float arm: bits-in-i64 → bitcast → fop → bitcast back.
                if let IlType::F { width: fw } = ty {
                    let ft = if fw == 64 { self.ctx.f64_type() } else { self.ctx.f32_type() };
                    let (a, bb) = if fw == 32 {
                        (self.b.build_int_truncate(a, self.ctx.i32_type(), "fa").unwrap(),
                         self.b.build_int_truncate(bb, self.ctx.i32_type(), "fb").unwrap())
                    } else { (a, bb) };
                    let fa = self.b.build_bit_cast(a, ft, "bfa").unwrap().into_float_value();
                    let fb = self.b.build_bit_cast(bb, ft, "bfb").unwrap().into_float_value();
                    let fr = match op.kind {
                        Add => self.b.build_float_add(fa, fb, "fadd").unwrap(),
                        Sub => self.b.build_float_sub(fa, fb, "fsub").unwrap(),
                        Mul => self.b.build_float_mul(fa, fb, "fmul").unwrap(),
                        Div => self.b.build_float_div(fa, fb, "fdiv").unwrap(),
                        _ => panic!("llvm float bin {:?}", op.kind),
                    };
                    let it = if fw == 64 { i64t } else { self.ctx.i32_type() };
                    let ri = self.b.build_bit_cast(fr, it, "bfr").unwrap().into_int_value();
                    let r = if fw == 32 { self.b.build_int_z_extend(ri, i64t, "fz").unwrap() } else { ri };
                    self.put(op.out.unwrap(), r);
                    return;
                }
                // For sub-64 SIGNED div/rem/shr the operands must be
                // sign-extended FROM THEIR WIDTH first (vals live zext'd;
                // tier-1 gets this via asrv_w/sdiv on masked regs — the
                // 32-bit forms operate on w-registers = the same effect).
                let sext_in = |lo: IntValue<'ctx>| -> IntValue<'ctx> {
                    if signed && w < 64 {
                        let lt = self.ctx.custom_width_int_type(std::num::NonZeroU32::new(w).unwrap()).unwrap();
                        let t = self.b.build_int_truncate(lo, lt, "sx1").unwrap();
                        self.b.build_int_s_extend(t, i64t, "sx2").unwrap()
                    } else { lo }
                };
                let r = match op.kind {
                    Add => self.b.build_int_add(a, bb, "add").unwrap(),
                    Sub => self.b.build_int_sub(a, bb, "sub").unwrap(),
                    Mul => {
                        if w > 64 {
                            // wide mul: args are i128 already (Cast imm=1 pair or
                            // sext-to-128 produced them).
                            self.b.build_int_mul(a, bb, "mulw").unwrap()
                        } else { self.b.build_int_mul(a, bb, "mul").unwrap() }
                    }
                    And => self.b.build_and(a, bb, "and").unwrap(),
                    Or  => self.b.build_or(a, bb, "or").unwrap(),
                    Xor => self.b.build_xor(a, bb, "xor").unwrap(),
                    Shl => self.b.build_left_shift(a, bb, "shl").unwrap(),
                    Shr => {
                        let av = sext_in(a);
                        self.b.build_right_shift(av, bb, signed, "shr").unwrap()
                    }
                    Rotr => {
                        // rotr(a, b) at width w: (a >> b) | (a << (w - b)), masked.
                        // tier-1 uses rorv (64) / rorv_w (32); general form here.
                        let wv = i64t.const_int(w as u64, false);
                        let bm = self.b.build_and(bb, i64t.const_int((w - 1) as u64, false), "rm").unwrap();
                        let r1 = self.b.build_right_shift(a, bm, false, "rr").unwrap();
                        let ls = self.b.build_int_sub(wv, bm, "rl").unwrap();
                        let ls = self.b.build_and(ls, i64t.const_int((w - 1) as u64, false), "rlm").unwrap();
                        let r2 = self.b.build_left_shift(a, ls, "rls").unwrap();
                        self.b.build_or(r1, r2, "ror").unwrap()
                    }
                    Div => {
                        if signed {
                            let (av, bv) = (sext_in(a), sext_in(bb));
                            self.b.build_int_signed_div(av, bv, "sdiv").unwrap()
                        } else {
                            self.b.build_int_unsigned_div(a, bb, "udiv").unwrap()
                        }
                    }
                    Rem => {
                        if signed {
                            let (av, bv) = (sext_in(a), sext_in(bb));
                            self.b.build_int_signed_rem(av, bv, "srem").unwrap()
                        } else {
                            self.b.build_int_unsigned_rem(a, bb, "urem").unwrap()
                        }
                    }
                    _ => unreachable!(),
                };
                let r = if w < 64 && !matches!(op.kind, Shl) { self.mask_to(r, w) } else { r };
                self.put(op.out.unwrap(), r);
            }
            Neg | Not | Rbit | Clz => {
                let a = self.get(op.args[0]);
                let r = match op.kind {
                    Neg => self.b.build_int_sub(i64t.const_zero(), a, "neg").unwrap(),
                    Not => {
                        let mask = if matches!(ty, IlType::Bool) { 1u64 }
                                   else if w < 64 { (1u64 << w) - 1 } else { u64::MAX };
                        self.b.build_xor(a, i64t.const_int(mask, false), "not").unwrap()
                    }
                    Rbit => {
                        // llvm.bitreverse.iW on the truncated value, then zext.
                        let lt = self.ctx.custom_width_int_type(std::num::NonZeroU32::new(w).unwrap()).unwrap();
                        let t = self.b.build_int_truncate(a, lt, "rb1").unwrap();
                        let iname = format!("llvm.bitreverse.i{w}");
                        let f = self.intrinsic1(&iname, lt.into());
                        let r = self.b.build_call(f, &[t.into()], "rb").unwrap()
                            .try_as_basic_value().unwrap_basic().into_int_value();
                        self.b.build_int_z_extend(r, i64t, "rbz").unwrap()
                    }
                    Clz => {
                        let lt = self.ctx.custom_width_int_type(std::num::NonZeroU32::new(w).unwrap()).unwrap();
                        let t = self.b.build_int_truncate(a, lt, "cl1").unwrap();
                        let iname = format!("llvm.ctlz.i{w}");
                        let f = self.intrinsic_ctlz(&iname, lt.into());
                        let fz = self.ctx.bool_type().const_zero();
                        let r = self.b.build_call(f, &[t.into(), fz.into()], "clz").unwrap()
                            .try_as_basic_value().unwrap_basic().into_int_value();
                        self.b.build_int_z_extend(r, i64t, "clz2").unwrap()
                    }
                    _ => unreachable!(),
                };
                let r = if w < 64 && matches!(op.kind, Neg) { self.mask_to(r, w) } else { r };
                self.put(op.out.unwrap(), r);
            }
            Eq | Ne | Lt | Le | Gt | Ge => {
                let aty = self.rec.val_type(op.args[0]);
                let signed = is_signed(aty);
                let aw = width_bits(aty);
                let mut a = self.get(op.args[0]);
                let mut bb = self.get(op.args[1]);
                // Float compare arm (COMISS etc route through fcmpp upstream —
                // but Lt/Eq on F-typed args happens in scalar fcmp paths).
                if matches!(aty, IlType::F { .. }) {
                    let fw = aw;
                    let ft = if fw == 64 { self.ctx.f64_type() } else { self.ctx.f32_type() };
                    let (ai, bi) = if fw == 32 {
                        (self.b.build_int_truncate(a, self.ctx.i32_type(), "ca").unwrap(),
                         self.b.build_int_truncate(bb, self.ctx.i32_type(), "cb").unwrap())
                    } else { (a, bb) };
                    let fa = self.b.build_bit_cast(ai, ft, "cfa").unwrap().into_float_value();
                    let fb = self.b.build_bit_cast(bi, ft, "cfb").unwrap().into_float_value();
                    use inkwell::FloatPredicate::*;
                    let p = match op.kind { Eq => OEQ, Ne => UNE, Lt => OLT,
                                            Le => OLE, Gt => OGT, Ge => OGE, _ => unreachable!() };
                    let c = self.b.build_float_compare(p, fa, fb, "fcmp").unwrap();
                    let r = self.b.build_int_z_extend(c, i64t, "fce").unwrap();
                    self.put(op.out.unwrap(), r);
                    return;
                }
                if signed && aw < 64 {
                    let lt = self.ctx.custom_width_int_type(std::num::NonZeroU32::new(aw).unwrap()).unwrap();
                    let ta = self.b.build_int_truncate(a, lt, "sa").unwrap();
                    let tb = self.b.build_int_truncate(bb, lt, "sb").unwrap();
                    a = self.b.build_int_s_extend(ta, i64t, "sae").unwrap();
                    bb = self.b.build_int_s_extend(tb, i64t, "sbe").unwrap();
                }
                let p = match (op.kind, signed) {
                    (Eq, _) => IntPredicate::EQ, (Ne, _) => IntPredicate::NE,
                    (Lt, true) => IntPredicate::SLT, (Lt, false) => IntPredicate::ULT,
                    (Le, true) => IntPredicate::SLE, (Le, false) => IntPredicate::ULE,
                    (Gt, true) => IntPredicate::SGT, (Gt, false) => IntPredicate::UGT,
                    (Ge, true) => IntPredicate::SGE, (Ge, false) => IntPredicate::UGE,
                    _ => unreachable!(),
                };
                let c = self.b.build_int_compare(p, a, bb, "cmp").unwrap();
                let r = self.b.build_int_z_extend(c, i64t, "ce").unwrap();
                self.put(op.out.unwrap(), r);
            }
            Cast | Sext => {
                let a = self.get(op.args[0]);
                let fty = self.rec.val_type(op.args[0]);
                let fw = width_bits(fty);
                // Cast markers: imm=1 bitcast (mov-only), imm=2 pair128, imm=3 hi64.
                if op.imm == 2 {
                    // pair128(hi, lo) → i128
                    let hi = self.get(op.args[0]);
                    let lo = self.get(op.args[1]);
                    let i128t = self.ctx.i128_type();
                    let hie = self.b.build_int_z_extend(hi, i128t, "p1").unwrap();
                    let loe = self.b.build_int_z_extend(lo, i128t, "p2").unwrap();
                    let sh = self.b.build_left_shift(hie, i128t.const_int(64, false), "p3").unwrap();
                    let r = self.b.build_or(sh, loe, "p4").unwrap();
                    self.put(op.out.unwrap(), r);
                    return;
                }
                if op.imm == 3 {
                    // hi64 of an i128
                    let sh = self.b.build_right_shift(a, self.ctx.i128_type().const_int(64, false), false, "h1").unwrap();
                    let r = self.b.build_int_truncate(sh, i64t, "h2").unwrap();
                    self.put(op.out.unwrap(), r);
                    return;
                }
                // Bool target: != 0 (the cmp+cset fix from the magic-static bug).
                if matches!(ty, IlType::Bool) && matches!(op.kind, Cast) {
                    let c = self.b.build_int_compare(IntPredicate::NE, a, a.get_type().const_zero(), "b0").unwrap();
                    let r = self.b.build_int_z_extend(c, i64t, "b1").unwrap();
                    self.put(op.out.unwrap(), r);
                    return;
                }
                // I↔F numeric converts (imm=0, ty crosses I/F).
                if op.imm == 0 {
                    match (fty, ty) {
                        (IlType::I { signed: s, .. }, IlType::F { width: fwd }) => {
                            let ft = if fwd == 64 { self.ctx.f64_type() } else { self.ctx.f32_type() };
                            let av = if s && fw < 64 {
                                let lt = self.ctx.custom_width_int_type(std::num::NonZeroU32::new(fw).unwrap()).unwrap();
                                let t = self.b.build_int_truncate(a, lt, "if1").unwrap();
                                self.b.build_int_s_extend(t, i64t, "if2").unwrap()
                            } else { a };
                            let fv = if s { self.b.build_signed_int_to_float(av, ft, "sif").unwrap() }
                                     else { self.b.build_unsigned_int_to_float(av, ft, "uif").unwrap() };
                            let it = if fwd == 64 { i64t } else { self.ctx.i32_type() };
                            let ri = self.b.build_bit_cast(fv, it, "ifb").unwrap().into_int_value();
                            let r = if fwd == 32 { self.b.build_int_z_extend(ri, i64t, "ifz").unwrap() } else { ri };
                            self.put(op.out.unwrap(), r);
                            return;
                        }
                        (IlType::F { width: fws }, IlType::I { signed: s, width: iw }) => {
                            // NOTE: raw fcvt semantics (fcvtzs). The x86
                            // indefinite-integer behavior is handled UPSTREAM
                            // in the .isa's f_to_si_x86 (in-range ternary) —
                            // this op only sees in-range values on that path.
                            let ft = if fws == 64 { self.ctx.f64_type() } else { self.ctx.f32_type() };
                            let ai = if fws == 32 {
                                self.b.build_int_truncate(a, self.ctx.i32_type(), "fi1").unwrap()
                            } else { a };
                            let fa = self.b.build_bit_cast(ai, ft, "fi2").unwrap().into_float_value();
                            let it = self.ctx.custom_width_int_type(std::num::NonZeroU32::new(iw as u32).unwrap()).unwrap();
                            let iv = if s { self.b.build_float_to_signed_int(fa, it, "fis").unwrap() }
                                     else { self.b.build_float_to_unsigned_int(fa, it, "fiu").unwrap() };
                            // zext-to-64 (vals live zext'd; signed consumers sext at use).
                            let r = if (iw as u32) < 64 {
                                self.b.build_int_z_extend(iv, i64t, "fiz").unwrap()
                            } else { iv };
                            self.put(op.out.unwrap(), r);
                            return;
                        }
                        (IlType::F { width: 32 }, IlType::F { width: 64 }) => {
                            let ai = self.b.build_int_truncate(a, self.ctx.i32_type(), "ff1").unwrap();
                            let fa = self.b.build_bit_cast(ai, self.ctx.f32_type(), "ff2").unwrap().into_float_value();
                            let fd = self.b.build_float_ext(fa, self.ctx.f64_type(), "ff3").unwrap();
                            let r = self.b.build_bit_cast(fd, i64t, "ff4").unwrap().into_int_value();
                            self.put(op.out.unwrap(), r);
                            return;
                        }
                        (IlType::F { width: 64 }, IlType::F { width: 32 }) => {
                            let fa = self.b.build_bit_cast(a, self.ctx.f64_type(), "fg1").unwrap().into_float_value();
                            let fs = self.b.build_float_trunc(fa, self.ctx.f32_type(), "fg2").unwrap();
                            let ri = self.b.build_bit_cast(fs, self.ctx.i32_type(), "fg3").unwrap().into_int_value();
                            let r = self.b.build_int_z_extend(ri, i64t, "fg4").unwrap();
                            self.put(op.out.unwrap(), r);
                            return;
                        }
                        _ => {}
                    }
                }
                // Int-width casts. wide↔64 transitions handled via i128 vals.
                match op.kind {
                    Cast => {
                        if w > 64 && fw <= 64 {
                            // widen to i128 (zext; sext-to-128 comes as Sext).
                            let r = self.b.build_int_z_extend(a, self.ctx.i128_type(), "cw").unwrap();
                            self.put(op.out.unwrap(), r);
                        } else if w <= 64 && fw > 64 {
                            let r = self.b.build_int_truncate(a, i64t, "cn").unwrap();
                            let r = if w < 64 { self.mask_to(r, w) } else { r };
                            self.put(op.out.unwrap(), r);
                        } else if w >= fw {
                            // widening/equal zext-convention: already zext'd.
                            self.put(op.out.unwrap(), a);
                        } else {
                            let r = self.mask_to(a, w);
                            self.put(op.out.unwrap(), r);
                        }
                    }
                    Sext => {
                        if w > 64 {
                            // sext-to-128 from fw (≤64).
                            let lt = self.ctx.custom_width_int_type(std::num::NonZeroU32::new(fw.min(64)).unwrap()).unwrap();
                            let t = self.b.build_int_truncate(a, lt, "sw1").unwrap();
                            let r = self.b.build_int_s_extend(t, self.ctx.i128_type(), "sw2").unwrap();
                            self.put(op.out.unwrap(), r);
                        } else {
                            // sext fw→w within 64: trunc-to-fw, sext-to-w, zext-mask.
                            let lt = self.ctx.custom_width_int_type(std::num::NonZeroU32::new(fw).unwrap()).unwrap();
                            let t = self.b.build_int_truncate(a, lt, "se1").unwrap();
                            let wt = self.ctx.custom_width_int_type(std::num::NonZeroU32::new(w).unwrap()).unwrap();
                            let s = self.b.build_int_s_extend(t, wt, "se2").unwrap();
                            let r = self.b.build_int_z_extend(s, i64t, "se3").unwrap();
                            self.put(op.out.unwrap(), r);
                        }
                    }
                    _ => unreachable!(),
                }
            }
            Ternary => {
                let c = self.get(op.args[0]);
                let a = self.get(op.args[1]);
                let bb = self.get(op.args[2]);
                let cz = self.b.build_int_compare(IntPredicate::NE, c, c.get_type().const_zero(), "tc").unwrap();
                let r = self.b.build_select(cz, a, bb, "sel").unwrap().into_int_value();
                self.put(op.out.unwrap(), r);
            }
            Branch | BranchLink => {
                // store target into state[pc], jump to exit. (No linking/IC in
                // the LLVM tier v1 — the driver dispatches; regions are big
                // enough that the dispatch amortizes. v2: side-table exits.)
                let t = self.get(op.args[0]);
                self.st_state(self.layout.off_pc, t);
                self.b.build_unconditional_branch(self.exit_bb).unwrap();
                // continue lowering into a dead block (recorder may emit more
                // ops after branch inside cond arms — same as tier-1's shape).
                let dead = self.ctx.append_basic_block(self.f, "postbr");
                self.b.position_at_end(dead);
            }
            CondBegin => {
                let c = self.get(op.args[0]);
                let cz = self.b.build_int_compare(IntPredicate::NE, c, c.get_type().const_zero(), "cc").unwrap();
                let then_bb = self.ctx.append_basic_block(self.f, "then");
                let else_bb = self.ctx.append_basic_block(self.f, "else");
                let end_bb = self.ctx.append_basic_block(self.f, "endif");
                self.b.build_conditional_branch(cz, then_bb, else_bb).unwrap();
                self.b.position_at_end(then_bb);
                self.conds.push((else_bb, end_bb, false));
            }
            CondElse => {
                let (else_bb, end_bb, _) = *self.conds.last().unwrap();
                self.b.build_unconditional_branch(end_bb).unwrap();
                self.b.position_at_end(else_bb);
                self.conds.last_mut().unwrap().2 = true;
            }
            CondEnd => {
                let (else_bb, end_bb, in_else) = self.conds.pop().unwrap();
                self.b.build_unconditional_branch(end_bb).unwrap();
                if !in_else {
                    // no else arm: else_bb just falls to end.
                    self.b.position_at_end(else_bb);
                    self.b.build_unconditional_branch(end_bb).unwrap();
                }
                self.b.position_at_end(end_bb);
            }
            CallIntrinsic => panic!("llvm-tier: CallIntrinsic — block belongs to tier-0"),
            Unimplemented => panic!("llvm-tier: Unimplemented op"),
        }
    }

    fn intrinsic1(&self, name: &str, t: inkwell::types::BasicTypeEnum<'ctx>)
        -> FunctionValue<'ctx>
    {
        let m = self.module;
        m.get_function(name).unwrap_or_else(|| {
            let ft = match t {
                inkwell::types::BasicTypeEnum::IntType(it) => it.fn_type(&[t.into()], false),
                _ => unreachable!(),
            };
            m.add_function(name, ft, None)
        })
    }
    fn intrinsic_ctlz(&self, name: &str, t: inkwell::types::BasicTypeEnum<'ctx>)
        -> FunctionValue<'ctx>
    {
        let m = self.module;
        m.get_function(name).unwrap_or_else(|| {
            let ft = match t {
                inkwell::types::BasicTypeEnum::IntType(it) =>
                    it.fn_type(&[t.into(), self.ctx.bool_type().into()], false),
                _ => unreachable!(),
            };
            m.add_function(name, ft, None)
        })
    }
}

/// Lower + JIT a recorded region. Returns None if any op can't lower (caller
/// falls back to tier-1/2 exactly as tier-1 bails to tier-0).
pub fn compile_llvm(rec: &IlRecorder, layout: &'static StateLayout) -> Option<LlvmBlock> {
    let ctx = Box::new(Context::create());
    // SAFETY of the 'static transmute: LlvmBlock owns the Box<Context> and the
    // ExecutionEngine together; the engine never outlives the context.
    let ctx_ref: &'static Context = unsafe { &*(ctx.as_ref() as *const Context) };
    let module = ctx_ref.create_module("region");
    let i64p = ctx_ref.ptr_type(AddressSpace::default());
    let fnt = ctx_ref.void_type().fn_type(&[i64p.into(), i64p.into()], false);
    let f = module.add_function("region_entry", fnt, None);
    let entry_bb = ctx_ref.append_basic_block(f, "entry");
    let exit_bb = ctx_ref.append_basic_block(f, "exit");
    let b = ctx_ref.create_builder();
    b.position_at_end(exit_bb);
    b.build_return(None).unwrap();
    b.position_at_end(entry_bb);

    let state = f.get_nth_param(0)?.into_pointer_value();
    let mut lo = Lower {
        ctx: ctx_ref, module: &module, b, f, layout, rec,
        vals: vec![None; rec.next_val() as usize],
        state, conds: vec![], exit_bb,
    };
    // catch_unwind at the caller (block_cache) handles panics from unsupported
    // ops — same contract as the tier-1 bail class.
    for op in &rec.ops {
        lo.lower_op(op);
    }
    // If the last op wasn't a Branch, the current block dangles — terminate.
    let cur = lo.b.get_insert_block()?;
    if cur.get_terminator().is_none() {
        lo.b.build_unconditional_branch(exit_bb).unwrap();
    }

    if module.verify().is_err() {
        if std::env::var("XF_DBG").is_ok() {
            eprintln!("[llvm] module verify FAILED:\n{}", module.print_to_string().to_string());
        }
        return None;
    }
    let ee = module.create_jit_execution_engine(OptimizationLevel::Aggressive).ok()?;
    let addr = unsafe { ee.get_function_address("region_entry").ok()? };
    let entry: extern "C" fn(*mut u64, *mut u64) = unsafe { std::mem::transmute(addr) };
    // extend ee's lifetime to 'static alongside the owned ctx (see SAFETY).
    let ee: ExecutionEngine<'static> = unsafe { std::mem::transmute(ee) };
    Some(LlvmBlock { _ee: ee, _ctx: ctx, entry })
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::{Builder, RegFile};
    use crate::il_record::IlRecorder;
    use crate::tier0::AARCH64_LAYOUT;

    /// Record a small computation, JIT via LLVM, diff against tier-1 on the
    /// SAME recording + identical pre-state (the two-backend oracle — the
    /// XF-6 bet's form, now three-way with tier-0 via the existing gates).
    #[test]
    fn llvm_matches_tier1() {
        if !cfg!(target_arch = "aarch64") { return; }
        // ops: x2 = x0 + x1; x3 = x2 * x0; flags: write NZCV bit from (x3 != 0);
        // a cond writes x4 = 7 when x3 != 0 else 9; store x3 to guest mem[x5].
        let build = |b: &mut dyn FnMut(&mut IlRecorder)| -> IlRecorder {
            let mut r = IlRecorder::new();
            b(&mut r);
            r
        };
        let mut body = |r: &mut IlRecorder| {
            let t = IlType::I { width: 64, signed: false };
            let a = r.reg_read(RegFile(0), 0, t);
            let bb = r.reg_read(RegFile(0), 1, t);
            let s = r.add(a, bb);
            r.reg_write(RegFile(0), 2, s);
            let m = r.mul(s, a);
            r.reg_write(RegFile(0), 3, m);
            let z = r.literal(t, 0);
            let nz = r.ne(m, z);
            let seven = r.literal(t, 7);
            let nine = r.literal(t, 9);
            let sel = r.ternary(nz, seven, nine);
            r.reg_write(RegFile(0), 4, sel);
            // guest store: mem[x5] = m (identity-ish: membase set in state)
            let addr = r.reg_read(RegFile(0), 5, t);
            r.mem_write(addr, m);
            // 32-bit signed op coverage: x6 = (i32)x0 >> 3 (asr)
            let t32 = IlType::I { width: 32, signed: true };
            let a32 = r.reg_read(RegFile(0), 0, t32);
            let sh = r.literal(t32, 3);
            let sr = r.shr(a32, sh);
            r.reg_write(RegFile(0), 6, sr);
            let end = r.literal(t, 0x2000);
            r.branch(end, false);
        };
        let rec1 = build(&mut body);
        let rec2 = build(&mut body);

        // guest memory page both runs write into:
        let mut mem1 = vec![0u8; 4096];
        let mut mem2 = vec![0u8; 4096];

        let mk_state = |mem: &mut Vec<u8>| -> Vec<u64> {
            let mut st = vec![0u64; crate::tier0::STATE_WORDS];
            st[0] = 0xFFFF_FFFF_8000_0011;   // x0 (negative as i32 → asr coverage)
            st[1] = 0x1234_5678;             // x1
            st[5] = 0x100;                   // guest addr of the store
            let mb_idx = (AARCH64_LAYOUT.off_membase / 8) as usize;
            st[mb_idx] = mem.as_mut_ptr() as u64;
            st
        };

        // tier-1 arm
        let mut t1 = crate::tier1::Tier1::with_layout(&AARCH64_LAYOUT);
        replay(&rec1, &mut t1);
        let blk = t1.compile();
        let mut st1 = mk_state(&mut mem1);
        let mut spill = vec![0u64; blk.n_slots as usize + 1];
        (blk.entry_fn())(st1.as_mut_ptr(), spill.as_mut_ptr());

        // llvm arm
        let lb = compile_llvm(&rec2, &AARCH64_LAYOUT).expect("llvm compile");
        let mut st2 = mk_state(&mut mem2);
        let mut spill2 = vec![0u64; 8];
        (lb.entry)(st2.as_mut_ptr(), spill2.as_mut_ptr());

        for i in 0..crate::tier0::STATE_WORDS {
            // skip membase (different pointers by construction)
            if i == (AARCH64_LAYOUT.off_membase / 8) as usize { continue; }
            assert_eq!(st1[i], st2[i], "state[{i}] differs: t1={:#x} llvm={:#x}", st1[i], st2[i]);
        }
        assert_eq!(mem1, mem2, "guest memory differs");
        eprintln!("[llvm-test] state+mem identical across tier-1/llvm");
    }

    /// Feed a recording's ops back into another Builder (the replay glue the
    /// test needs — IlRecorder ops → Builder calls).
    fn replay(rec: &IlRecorder, b: &mut crate::tier1::Tier1) {
        use crate::il_record::IlOpKind::*;
        use std::collections::HashMap;
        let mut map: HashMap<u32, u32> = HashMap::new();
        for op in &rec.ops {
            let g = |m: &HashMap<u32, u32>, v: u32| *m.get(&v).unwrap();
            let out = match op.kind {
                Literal => Some(<crate::tier1::Tier1 as Builder>::literal(b, op.ty, op.imm)),
                RegRead => Some(b.reg_read(RegFile((op.imm >> 32) as u8), op.imm as u32, op.ty)),
                RegWrite => { b.reg_write(RegFile((op.imm >> 32) as u8), op.imm as u32, g(&map, op.args[0])); None }
                Add => Some(b.add(g(&map, op.args[0]), g(&map, op.args[1]))),
                Mul => Some(b.mul(g(&map, op.args[0]), g(&map, op.args[1]))),
                Ne => Some(b.ne(g(&map, op.args[0]), g(&map, op.args[1]))),
                Shr => Some(b.shr(g(&map, op.args[0]), g(&map, op.args[1]))),
                Ternary => Some(b.ternary(g(&map, op.args[0]), g(&map, op.args[1]), g(&map, op.args[2]))),
                MemWrite => { b.mem_write(g(&map, op.args[0]), g(&map, op.args[1])); None }
                Branch => { b.branch(g(&map, op.args[0]), false); None }
                k => panic!("replay: {k:?} not in test glue"),
            };
            if let (Some(o), Some(orig)) = (out, op.out) { map.insert(orig, o); }
        }
    }
}
