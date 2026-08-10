//! An INTERPRETING `Builder` — each method computes the IL-op directly on a `GuestState`.
//! No machine-code emit; `Val` carries the actual value. This is:
//!   (1) the first EXECUTING path for generated `recompile_one` (proves semantics, not
//!       just typecheck / IL-shape);
//!   (2) the state-diff ORACLE every JIT tier compares against (per DESIGN.md §Oracles:
//!       "tier-0 vs interpreter → state diff = 0").
//!
//! Rung-4 step ③ ordering: InterpretingBuilder first (this file, cheap, no encoder) →
//! then Tier0<Aarch64Emit> (template JIT) diffs against it → then tier-1/2.
//!
//! `Val` = a tagged u128 (type + bits). Copy, cheap. V128 lanes packed into the u128
//! (aarch64 V-regs are exactly 128b). The tag drives sign/width for arithmetic + casts.

use crate::{Builder, IlType, RegFile, LocalId, NativeSlot, IntrinsicId, RoundMode};

/// A concrete value: the type tag + the bits (sign-extended for signed I; f32/f64 in
/// low bits; V128 packed; Bool in bit-0).
#[derive(Debug, Clone, Copy, PartialEq)]
pub struct IVal { pub ty: IlType, pub bits: u128 }

impl IVal {
    pub fn u(w: u8, v: u128) -> Self { Self { ty: IlType::I{signed:false, width:w}, bits: mask(v, w) } }
    pub fn i(w: u8, v: i128) -> Self { Self { ty: IlType::I{signed:true,  width:w}, bits: mask(v as u128, w) } }
    pub fn b(v: bool) -> Self { Self { ty: IlType::Bool, bits: v as u128 } }
    pub fn f32(v: f32) -> Self { Self { ty: IlType::F{width:32}, bits: v.to_bits() as u128 } }
    pub fn f64(v: f64) -> Self { Self { ty: IlType::F{width:64}, bits: v.to_bits() as u128 } }
    pub fn v128(v: u128) -> Self { Self { ty: IlType::V128, bits: v } }

    pub fn as_u64(&self) -> u64 { self.bits as u64 }
    pub fn as_bool(&self) -> bool { self.bits & 1 != 0 }
    pub fn as_i(&self) -> i128 {
        // sign-extend from width for signed types
        match self.ty {
            IlType::I{signed:true, width} => sext(self.bits, width),
            _ => self.bits as i128,
        }
    }
    pub fn as_f32(&self) -> f32 { f32::from_bits(self.bits as u32) }
    pub fn as_f64(&self) -> f64 { f64::from_bits(self.bits as u64) }
}

fn mask(v: u128, w: u8) -> u128 { if w >= 128 { v } else { v & ((1u128 << w) - 1) } }
fn sext(v: u128, w: u8) -> i128 {
    if w >= 128 { return v as i128; }
    let m = 1u128 << (w - 1);
    ((v ^ m).wrapping_sub(m)) as i128
}

/// Guest memory model — a flat sparse map for tests. A real host supplies its own
/// (e.g. Alky's loader-mapped image); the trait keeps the interp generic over it.
pub trait GuestMem {
    fn read(&self, addr: u64, w: u8) -> u128;
    fn write(&mut self, addr: u64, w: u8, bits: u128);
}

/// Per-arch state layout. The generated recompiler.rs declares RegFile ids (GPR=0/VEC=1/…);
/// the InterpretingBuilder needs to know how to index them. A trait keeps this arch-neutral
/// (aarch64 vs x86 supply different impls).
pub trait RegState {
    fn reg_read(&self, file: RegFile, idx: u32, ty: IlType) -> IVal;
    fn reg_write(&mut self, file: RegFile, idx: u32, v: IVal);
    fn pc(&self) -> u64;
    fn set_pc(&mut self, pc: u64);
    fn set_lr(&mut self, lr: u64);
}

/// Intrinsic hook — svc/load-exclusive/etc route here. For tests: panic with the id;
/// a real host wires the arch's intrinsic-impl table.
pub type IntrinsicFn<S, M> = fn(&mut S, &mut M, id: u32, args: &[IVal]) -> Option<IVal>;

pub struct InterpretingBuilder<'a, S: RegState, M: GuestMem> {
    pub state: &'a mut S,
    pub mem: &'a mut M,
    pub locals: Vec<IVal>,
    pub intrinsic: IntrinsicFn<S, M>,
    pub branched: bool,
    /// pc of the CURRENT insn (recompile_one's `pc` arg). branch() reads it for link.
    pub insn_pc: u64,
    /// Optional memory-write watchpoint: (target_addr, range_bytes). When a
    /// mem_write's [addr, addr+width) overlaps [target, target+range), prints
    /// {insn_pc, addr, width, value} to stderr. Insn-grain — the exact
    /// instruction that touched the address. For arena-init/who-wrote-this
    /// tracing. Set via the constructor or directly on the struct.
    pub watch: Option<(u64, u64)>,
}

impl<'a, S: RegState, M: GuestMem> InterpretingBuilder<'a, S, M> {
    pub fn new(state: &'a mut S, mem: &'a mut M, insn_pc: u64) -> Self {
        Self { state, mem, locals: vec![], intrinsic: |_,_,id,_| panic!("intrinsic {id} not wired"),
               branched: false, insn_pc, watch: None }
    }
    /// Set a memory-write watchpoint on [addr, addr+range). Prints {insn_pc,
    /// addr, width, value} to stderr on any overlapping mem_write. Insn-grain.
    pub fn set_watch(&mut self, addr: u64, range: u64) { self.watch = Some((addr, range)); }
}

// ── the arithmetic core ────────────────────────────────────────────────────
// Width-aware, sign-aware. Result type = a's type (matches the .isa's inference:
// binops homogenize to the first operand's type, which the emit already casts to).

fn ibin(a: IVal, b: IVal, fu: impl Fn(u128,u128)->u128, fi: impl Fn(i128,i128)->i128,
        ff32: impl Fn(f32,f32)->f32, ff64: impl Fn(f64,f64)->f64) -> IVal {
    match a.ty {
        IlType::I{signed:false, width} => IVal { ty: a.ty, bits: mask(fu(a.bits, b.bits), width) },
        IlType::I{signed:true,  width} => IVal { ty: a.ty, bits: mask(fi(a.as_i(), b.as_i()) as u128, width) },
        IlType::F{width:32} => IVal::f32(ff32(a.as_f32(), b.as_f32())),
        IlType::F{width:64} => IVal::f64(ff64(a.as_f64(), b.as_f64())),
        IlType::Bool => IVal::b(fu(a.bits, b.bits) & 1 != 0),
        // V128 bitwise (PXOR/PAND/POR): treat as u128 (bits are already u128).
        IlType::V128 => IVal { ty: IlType::V128, bits: fu(a.bits, b.bits) },
        _ => panic!("ibin on {:?}", a.ty),
    }
}
fn icmp(a: IVal, b: IVal, fu: impl Fn(u128,u128)->bool, fi: impl Fn(i128,i128)->bool,
        ff: impl Fn(f64,f64)->bool) -> IVal {
    IVal::b(match a.ty {
        IlType::I{signed:true, ..} => fi(a.as_i(), b.as_i()),
        IlType::I{..} | IlType::Bool => fu(a.bits, b.bits),
        IlType::F{width:32} => ff(a.as_f32() as f64, b.as_f32() as f64),
        IlType::F{width:64} => ff(a.as_f64(), b.as_f64()),
        _ => panic!("icmp on {:?}", a.ty),
    })
}

impl<'a, S: RegState, M: GuestMem> Builder for InterpretingBuilder<'a, S, M> {
    type Val = IVal;

    fn ty_of(&self, v: IVal) -> IlType { v.ty }

    fn literal(&mut self, ty: IlType, bits: u128) -> IVal { IVal { ty, bits: match ty {
        IlType::I{width, ..} => mask(bits, width), _ => bits } } }

    fn reg_read(&mut self, f: RegFile, idx: u32, ty: IlType) -> IVal { self.state.reg_read(f, idx, ty) }
    fn reg_write(&mut self, f: RegFile, idx: u32, v: IVal) { self.state.reg_write(f, idx, v) }
    fn mem_read(&mut self, a: IVal, ty: IlType) -> IVal {
        let w = match ty { IlType::I{width,..} => width, IlType::F{width} => width, IlType::V128 => 128, _ => 64 };
        IVal { ty, bits: self.mem.read(a.as_u64(), w) }
    }
    fn mem_write(&mut self, a: IVal, v: IVal) {
        let w = match v.ty { IlType::I{width,..} => width, IlType::F{width} => width, IlType::V128 => 128, _ => 64 };
        let addr = a.as_u64();
        if let Some((wa, wr)) = self.watch {
            let wb = (w / 8) as u64;
            // Overlap: [addr, addr+wb) ∩ [wa, wa+wr) ≠ ∅
            if addr < wa + wr && wa < addr + wb {
                eprintln!("[MEM-WATCH] pc=0x{:x} write [0x{:x}..+{}] = 0x{:x} (ty={:?})",
                    self.insn_pc, addr, wb, v.bits, v.ty);
            }
        }
        self.mem.write(addr, w, v.bits)
    }

    fn add(&mut self, a: IVal, b: IVal) -> IVal { ibin(a,b, |x,y|x.wrapping_add(y), |x,y|x.wrapping_add(y), |x,y|x+y, |x,y|x+y) }
    fn sub(&mut self, a: IVal, b: IVal) -> IVal { ibin(a,b, |x,y|x.wrapping_sub(y), |x,y|x.wrapping_sub(y), |x,y|x-y, |x,y|x-y) }
    fn mul(&mut self, a: IVal, b: IVal) -> IVal { ibin(a,b, |x,y|x.wrapping_mul(y), |x,y|x.wrapping_mul(y), |x,y|x*y, |x,y|x*y) }
    fn div(&mut self, a: IVal, b: IVal) -> IVal { ibin(a,b,
        |x,y| if y==0 {0} else {x/y}, |x,y| if y==0 {0} else {x.wrapping_div(y)}, |x,y|x/y, |x,y|x/y) }
    fn rem(&mut self, a: IVal, b: IVal) -> IVal { ibin(a,b,
        |x,y| if y==0 {0} else {x%y}, |x,y| if y==0 {0} else {x.wrapping_rem(y)}, |x,y|x%y, |x,y|x%y) }
    fn neg(&mut self, a: IVal) -> IVal { match a.ty {
        IlType::F{width:32} => IVal::f32(-a.as_f32()), IlType::F{width:64} => IVal::f64(-a.as_f64()),
        IlType::I{width, ..} => IVal { ty: a.ty, bits: mask((a.bits as i128).wrapping_neg() as u128, width) },
        _ => panic!("neg {:?}", a.ty) } }

    fn and(&mut self, a: IVal, b: IVal) -> IVal { ibin(a,b, |x,y|x&y, |x,y|x&y, |_,_|panic!(), |_,_|panic!()) }
    fn or (&mut self, a: IVal, b: IVal) -> IVal { ibin(a,b, |x,y|x|y, |x,y|x|y, |_,_|panic!(), |_,_|panic!()) }
    fn xor(&mut self, a: IVal, b: IVal) -> IVal { ibin(a,b, |x,y|x^y, |x,y|x^y, |_,_|panic!(), |_,_|panic!()) }
    fn not(&mut self, a: IVal) -> IVal { match a.ty {
        IlType::Bool => IVal::b(!a.as_bool()),
        IlType::V128 => IVal { ty: a.ty, bits: !a.bits },
        IlType::I{width, ..} => IVal { ty: a.ty, bits: mask(!a.bits, width) },
        _ => panic!("not {:?}", a.ty) } }
    fn shl(&mut self, a: IVal, b: IVal) -> IVal { let s = (b.bits & 127) as u32;
        // NO mask-to-a.width: the .isa (and C# `IRuntimeValue<byte>.LeftShift`, and silicon)
        // treat shift as int-promoting — `(u8)6 << 28` shifts into a wider space, and the
        // consumer casts to destination width. Masking here loses the high bits before
        // the cast can see them (FCMP: `(cast (<< u8 28) u32)` → 0 instead of 0x60000000).
        // The result TYPE stays a.ty (per FirstType sig); the bits carry the full shift.
        // A JIT tier that DOES need width-bounded shift emits `and dst, mask` after.
        IVal { ty: a.ty, bits: a.bits.wrapping_shl(s) } }
    fn shr(&mut self, a: IVal, b: IVal) -> IVal { let s = (b.bits & 127) as u32;
        match a.ty {
            IlType::I{signed:true, width} => IVal { ty: a.ty, bits: mask((sext(a.bits, width) >> s) as u128, width) },
            _ => IVal { ty: a.ty, bits: a.bits >> s },
        } }
    fn rotr(&mut self, a: IVal, b: IVal) -> IVal { let w = width_of(a.ty) as u32;
        let s = (b.bits as u32) % w.max(1);
        if s == 0 { return a; }
        let m = mask(a.bits, w as u8);
        IVal { ty: a.ty, bits: mask((m >> s) | (m << (w - s)), w as u8) } }
    fn rbit(&mut self, a: IVal) -> IVal { let w = width_of(a.ty);
        IVal { ty: a.ty, bits: a.bits.reverse_bits() >> (128 - w) } }
    fn clz(&mut self, a: IVal) -> IVal { let w = width_of(a.ty);
        IVal::u(w, (a.bits << (128 - w)).leading_zeros().min(w as u32) as u128) }

    fn eq(&mut self, a: IVal, b: IVal) -> IVal { icmp(a,b, |x,y|x==y, |x,y|x==y, |x,y|x==y) }
    fn ne(&mut self, a: IVal, b: IVal) -> IVal { icmp(a,b, |x,y|x!=y, |x,y|x!=y, |x,y|x!=y) }
    fn lt(&mut self, a: IVal, b: IVal) -> IVal { icmp(a,b, |x,y|x< y, |x,y|x< y, |x,y|x< y) }
    fn le(&mut self, a: IVal, b: IVal) -> IVal { icmp(a,b, |x,y|x<=y, |x,y|x<=y, |x,y|x<=y) }
    fn gt(&mut self, a: IVal, b: IVal) -> IVal { icmp(a,b, |x,y|x> y, |x,y|x> y, |x,y|x> y) }
    fn ge(&mut self, a: IVal, b: IVal) -> IVal { icmp(a,b, |x,y|x>=y, |x,y|x>=y, |x,y|x>=y) }

    fn cast(&mut self, a: IVal, to: IlType) -> IVal {
        // Value-preserving. int→int: mask/sext to target. int↔float: numeric convert. bool→int: 0/1.
        match (a.ty, to) {
            (IlType::I{..}, IlType::I{signed:false, width}) => IVal { ty: to, bits: mask(a.bits, width) },
            (IlType::I{..}, IlType::I{signed:true,  width}) => {
                // Preserve VALUE: sext from src width, then mask to target.
                let sw = width_of(a.ty);
                IVal { ty: to, bits: mask(sext(a.bits, sw) as u128, width) }
            }
            (IlType::Bool, IlType::I{..}) => IVal { ty: to, bits: a.bits & 1 },
            (IlType::I{..}, IlType::Bool) => IVal::b(a.bits != 0),
            (IlType::I{signed, ..}, IlType::F{width:32}) =>
                IVal::f32(if signed { a.as_i() as f32 } else { a.bits as f32 }),
            (IlType::I{signed, ..}, IlType::F{width:64}) =>
                IVal::f64(if signed { a.as_i() as f64 } else { a.bits as f64 }),
            (IlType::F{width:fw}, IlType::I{signed:false, width}) => {
                let v = if fw == 32 { a.as_f32() as f64 } else { a.as_f64() };
                IVal { ty: to, bits: mask(v as u128, width) }
            }
            (IlType::F{width:fw}, IlType::I{signed:true, width}) => {
                let v = if fw == 32 { a.as_f32() as f64 } else { a.as_f64() };
                IVal { ty: to, bits: mask(v as i128 as u128, width) }
            }
            (IlType::F{width:32}, IlType::F{width:64}) => IVal::f64(a.as_f32() as f64),
            (IlType::F{width:64}, IlType::F{width:32}) => IVal::f32(a.as_f64() as f32),
            // I↔V128: V128 is a raw 128-bit bag — same bits, retyped (zext for narrower).
            // (SSE MOVD-X: gpr → zext(u64) → V128 upper-zeroed; MOVD-XS: V128 → u64 → gpr.)
            (IlType::I{..}, IlType::V128) => IVal { ty: IlType::V128, bits: a.bits },
            (IlType::V128, IlType::I{width, ..}) => IVal { ty: to, bits: mask(a.bits, width) },
            (IlType::V128, IlType::Bool) => IVal::b(a.bits != 0),
            (from, to) if from == to => a,
            _ => panic!("cast {:?} → {:?}", a.ty, to),
        }
    }
    fn pair128(&mut self, hi: IVal, lo: IVal) -> IVal {
        IVal { ty: IlType::I{signed:false, width:128}, bits: (hi.bits << 64) | (lo.bits & u64::MAX as u128) }
    }
    fn hi64(&mut self, a: IVal) -> IVal { IVal { ty: IlType::U64, bits: a.bits >> 64 } }
    fn vfbin(&mut self, a: IVal, b: IVal, ew: u32, op: u32) -> IVal {
        // Per-lane float arith on the u128 bit-storage. ew=32→4×f32, ew=64→2×f64.
        let n = 128 / ew;
        let m = if ew < 128 { (1u128 << ew) - 1 } else { u128::MAX };
        let mut r = 0u128;
        for k in 0..n {
            let ea = ((a.bits >> (k*ew)) & m) as u64;
            let eb = ((b.bits >> (k*ew)) & m) as u64;
            let er = if ew == 32 {
                let (fa, fb) = (f32::from_bits(ea as u32), f32::from_bits(eb as u32));
                (match op { 0=>fa+fb, 1=>fa-fb, 2=>fa*fb, 3=>fa/fb, _=>panic!() }).to_bits() as u64
            } else {
                let (fa, fb) = (f64::from_bits(ea), f64::from_bits(eb));
                (match op { 0=>fa+fb, 1=>fa-fb, 2=>fa*fb, 3=>fa/fb, _=>panic!() }).to_bits()
            };
            r |= (er as u128 & m) << (k*ew);
        }
        IVal { ty: IlType::V128, bits: r }
    }
    fn vzip(&mut self, a: IVal, b: IVal, ew: u32, hi: bool) -> IVal {
        let n = 128 / ew;   // total lanes at this elem-width
        let m = if ew < 128 { (1u128 << ew) - 1 } else { u128::MAX };
        let base = if hi { n/2 } else { 0 };
        let mut r = 0u128;
        for k in 0..(n/2) {
            let ea = (a.bits >> ((base+k) * ew)) & m;
            let eb = (b.bits >> ((base+k) * ew)) & m;
            r |= ea << (2*k * ew);
            r |= eb << ((2*k+1) * ew);
        }
        IVal { ty: IlType::V128, bits: r }
    }
    fn loop_n(&mut self, n: IVal, body: &mut dyn FnMut(&mut Self)) {
        for _ in 0..n.bits as u64 { body(self); }
    }
    fn bitcast(&mut self, a: IVal, to: IlType) -> IVal { IVal { ty: to, bits: a.bits } }
    fn sext(&mut self, a: IVal, to: IlType) -> IVal {
        let sw = width_of(a.ty); let tw = width_of(to);
        IVal { ty: to, bits: mask(sext(a.bits, sw) as u128, tw) }
    }

    fn fabs(&mut self, a: IVal) -> IVal { match a.ty {
        IlType::F{width:32} => IVal::f32(a.as_f32().abs()), _ => IVal::f64(a.as_f64().abs()) } }
    fn fsqrt(&mut self, a: IVal) -> IVal { match a.ty {
        IlType::F{width:32} => IVal::f32(a.as_f32().sqrt()), _ => IVal::f64(a.as_f64().sqrt()) } }
    fn fceil(&mut self, a: IVal) -> IVal { match a.ty {
        IlType::F{width:32} => IVal::f32(a.as_f32().ceil()), _ => IVal::f64(a.as_f64().ceil()) } }
    fn ffloor(&mut self, a: IVal) -> IVal { match a.ty {
        IlType::F{width:32} => IVal::f32(a.as_f32().floor()), _ => IVal::f64(a.as_f64().floor()) } }
    fn fisnan(&mut self, a: IVal) -> IVal { IVal::b(match a.ty {
        IlType::F{width:32} => a.as_f32().is_nan(), _ => a.as_f64().is_nan() }) }
    fn fround(&mut self, a: IVal, m: RoundMode) -> IVal {
        // ‡ RoundMode variants map to specific IEEE modes (Nearest/HalfDown/HalfUp/TowardZero).
        // Rust std has round() (half-away-from-zero) + trunc(). For rung-4b: implement the
        // exact tie-break variants; for now use the closest std fn.
        let f = |x: f64| match m {
            RoundMode::Nearest | RoundMode::HalfUp => x.round(),
            RoundMode::HalfDown => -(-x).round(),   // ‡ approx
            RoundMode::TowardZero => x.trunc(),
        };
        match a.ty { IlType::F{width:32} => IVal::f32(f(a.as_f32() as f64) as f32),
                     _ => IVal::f64(f(a.as_f64())) }
    }

    fn velement_read(&mut self, v: IVal, i: IVal, et: IlType) -> IVal {
        let ew = width_of(et); let idx = i.as_u64() as u32;
        IVal { ty: et, bits: mask(v.bits >> (idx * ew as u32), ew) }
    }
    fn velement_write(&mut self, v: IVal, i: IVal, e: IVal) -> IVal {
        let ew = width_of(e.ty); let idx = i.as_u64() as u32; let sh = idx * ew as u32;
        let m = if ew >= 128 { u128::MAX } else { ((1u128 << ew) - 1) << sh };
        IVal::v128((v.bits & !m) | ((mask(e.bits, ew) << sh) & m))
    }
    fn vzero_top(&mut self, v: IVal) -> IVal { IVal::v128(v.bits & 0xFFFF_FFFF_FFFF_FFFF) }

    fn branch(&mut self, t: IVal, link: bool) {
        if link { self.state.set_lr(self.insn_pc + 4); }
        self.state.set_pc(t.as_u64());
        self.branched = true;
    }
    fn cond(&mut self, c: IVal, then: &mut dyn FnMut(&mut Self), else_: &mut dyn FnMut(&mut Self)) {
        if c.as_bool() { then(self) } else { else_(self) }
    }
    fn ternary(&mut self, c: IVal, a: IVal, b: IVal) -> IVal { if c.as_bool() { a } else { b } }

    fn local_new(&mut self, ty: IlType) -> LocalId {
        let id = self.locals.len() as u32; self.locals.push(IVal { ty, bits: 0 }); LocalId(id) }
    fn local_read(&mut self, l: LocalId) -> IVal { self.locals[l.0 as usize] }
    fn local_write(&mut self, l: LocalId, v: IVal) { self.locals[l.0 as usize] = v }

    fn call_native(&mut self, _s: NativeSlot, _args: &[IVal]) -> Option<IVal> {
        panic!("call_native not wired in InterpretingBuilder")
    }
    fn call_intrinsic(&mut self, id: IntrinsicId, args: &[IVal]) -> Option<IVal> {
        (self.intrinsic)(self.state, self.mem, id.0, args)
    }
    fn unimplemented(&mut self, name: &'static str) { panic!("unimplemented insn: {name}") }
}

fn width_of(t: IlType) -> u8 {
    match t { IlType::I{width,..} => width, IlType::F{width} => width,
              IlType::V128 => 128, IlType::Bool => 1, IlType::Unit => 0 }
}

// ─────────────────────────────────────────────────────────────────────────────
// A minimal GuestMem for tests — flat Vec<u8> at a fixed base.
// ─────────────────────────────────────────────────────────────────────────────

/// GuestMem for mem_base=0 (shared-VA): guest addresses ARE host addresses.
/// Reads/writes go direct to host memory via raw ptr. For interp-mode runs
/// against a real mmap'd PE/ELF (the loader's mem_base=0 model). UNSAFE by
/// nature — the caller guarantees the guest's whole address space is mapped.
pub struct HostMem;
impl GuestMem for HostMem {
    fn read(&self, addr: u64, w: u8) -> u128 {
        unsafe { match w {
            8  => *(addr as *const u8)  as u128,
            16 => (addr as *const u16).read_unaligned() as u128,
            32 => (addr as *const u32).read_unaligned() as u128,
            64 => (addr as *const u64).read_unaligned() as u128,
            128 => { let lo = (addr as *const u64).read_unaligned() as u128;
                     let hi = ((addr+8) as *const u64).read_unaligned() as u128;
                     (hi << 64) | lo }
            _ => panic!("HostMem read w={w}"),
        } }
    }
    fn write(&mut self, addr: u64, w: u8, v: u128) {
        unsafe { match w {
            8  => *(addr as *mut u8) = v as u8,
            16 => (addr as *mut u16).write_unaligned(v as u16),
            32 => (addr as *mut u32).write_unaligned(v as u32),
            64 => (addr as *mut u64).write_unaligned(v as u64),
            128 => { (addr as *mut u64).write_unaligned(v as u64);
                     ((addr+8) as *mut u64).write_unaligned((v>>64) as u64); }
            _ => panic!("HostMem write w={w}"),
        } }
    }
}

pub struct FlatMem { pub base: u64, pub bytes: Vec<u8> }
impl FlatMem {
    pub fn new(base: u64, size: usize) -> Self { Self { base, bytes: vec![0; size] } }
}
impl GuestMem for FlatMem {
    fn read(&self, addr: u64, w: u8) -> u128 {
        let off = (addr - self.base) as usize; let n = (w as usize + 7) / 8;
        let mut v = 0u128;
        for i in 0..n { v |= (self.bytes[off+i] as u128) << (i*8); }
        v
    }
    fn write(&mut self, addr: u64, w: u8, bits: u128) {
        let off = (addr - self.base) as usize; let n = (w as usize + 7) / 8;
        for i in 0..n { self.bytes[off+i] = (bits >> (i*8)) as u8; }
    }
}
