//! A `Builder` impl that records every call as text — the rung-4 gate-(b) instrument.
//! Proves generated `recompile_one` RUNS (not just typechecks) + gives per-insn IL-seq
//! for spot-checking / diffing against the C# side's recording-IBuilder.
//!
//! `Val` = a u32 index into the recorded log (the SSA temp id). Every method appends
//! a line and returns the next id. `dump()` yields the log; `reset()` clears for the
//! next insn.

use crate::{Builder, IlType, RegFile, LocalId, NativeSlot, IntrinsicId, RoundMode};

#[derive(Default)]
pub struct RecordingBuilder {
    pub log: Vec<String>,
    next: u32,
}

impl RecordingBuilder {
    pub fn new() -> Self { Self::default() }
    pub fn dump(&self) -> String { self.log.join("\n") }
    pub fn reset(&mut self) { self.log.clear(); self.next = 0; }
    fn v(&mut self, s: String) -> u32 { self.log.push(format!("v{} = {}", self.next, s)); let n = self.next; self.next += 1; n }
    fn stmt(&mut self, s: String) { self.log.push(s); }
}

fn ty(t: IlType) -> &'static str {
    match t {
        IlType::I{signed:false, width:8} => "u8", IlType::I{signed:true, width:8} => "i8",
        IlType::I{signed:false, width:16} => "u16", IlType::I{signed:true, width:16} => "i16",
        IlType::I{signed:false, width:32} => "u32", IlType::I{signed:true, width:32} => "i32",
        IlType::I{signed:false, width:64} => "u64", IlType::I{signed:true, width:64} => "i64",
        IlType::I{signed:false, width:128} => "u128", IlType::I{signed:true, width:128} => "i128",
        IlType::I{signed, width} => Box::leak(format!("{}int{}", if signed {"s"} else {"u"}, width).into_boxed_str()),
        IlType::F{width:32} => "f32", IlType::F{width:64} => "f64",
        IlType::F{width} => Box::leak(format!("f{}", width).into_boxed_str()),
        IlType::V128 => "v128", IlType::Bool => "bool", IlType::Unit => "unit",
    }
}

macro_rules! bin { ($n:ident) => {
    fn $n(&mut self, a: u32, b: u32) -> u32 { self.v(format!(concat!(stringify!($n), " v{} v{}"), a, b)) }
}; }
macro_rules! un { ($n:ident) => {
    fn $n(&mut self, a: u32) -> u32 { self.v(format!(concat!(stringify!($n), " v{}"), a)) }
}; }

impl Builder for RecordingBuilder {
    type Val = u32;

    fn ty_of(&self, _v: u32) -> IlType { IlType::U64 /* recording doesn't track types */ }

    fn literal(&mut self, t: IlType, bits: u128) -> u32 {
        self.v(format!("lit {} 0x{:X}", ty(t), bits))
    }
    fn reg_read(&mut self, f: RegFile, idx: u32, t: IlType) -> u32 {
        self.v(format!("reg_read file={} idx={} {}", f.0, idx, ty(t)))
    }
    fn reg_write(&mut self, f: RegFile, idx: u32, v: u32) {
        self.stmt(format!("reg_write file={} idx={} v{}", f.0, idx, v))
    }
    fn mem_read(&mut self, a: u32, t: IlType) -> u32 { self.v(format!("mem_read v{} {}", a, ty(t))) }
    fn mem_write(&mut self, a: u32, v: u32) { self.stmt(format!("mem_write v{} v{}", a, v)) }

    bin!(add); bin!(sub); bin!(mul); bin!(div); bin!(rem); un!(neg);
    bin!(and); bin!(or); bin!(xor); un!(not); bin!(shl); bin!(shr); bin!(rotr); un!(rbit); un!(clz);
    bin!(eq); bin!(ne); bin!(lt); bin!(le); bin!(gt); bin!(ge);

    fn cast(&mut self, a: u32, to: IlType) -> u32 { self.v(format!("cast v{} → {}", a, ty(to))) }
    fn bitcast(&mut self, a: u32, to: IlType) -> u32 { self.v(format!("bitcast v{} → {}", a, ty(to))) }
    fn sext(&mut self, a: u32, to: IlType) -> u32 { self.v(format!("sext v{} → {}", a, ty(to))) }
    fn pair128(&mut self, hi: u32, lo: u32) -> u32 { self.v(format!("pair128 v{hi}:v{lo}")) }
    fn hi64(&mut self, a: u32) -> u32 { self.v(format!("hi64 v{a}")) }
    fn loop_n(&mut self, n: u32, body: &mut dyn FnMut(&mut Self)) {
        self.stmt(format!("loop_n v{n} {{"));
        body(self);
        self.stmt("}".into());
    }

    un!(fabs); un!(fsqrt); un!(fceil); un!(ffloor); un!(fisnan);
    fn fround(&mut self, a: u32, m: RoundMode) -> u32 { self.v(format!("fround v{} {:?}", a, m)) }

    fn velement_read(&mut self, v: u32, i: u32, et: IlType) -> u32 {
        self.v(format!("velem_r v{} v{} {}", v, i, ty(et)))
    }
    fn velement_write(&mut self, v: u32, i: u32, e: u32) -> u32 {
        self.v(format!("velem_w v{} v{} v{}", v, i, e))
    }
    un!(vzero_top);

    fn branch(&mut self, t: u32, link: bool) { self.stmt(format!("branch v{} link={}", t, link)) }
    fn cond(&mut self, c: u32, then: &mut dyn FnMut(&mut Self), else_: &mut dyn FnMut(&mut Self)) {
        self.stmt(format!("cond v{} {{", c));
        then(self);
        self.stmt("} else {".into());
        else_(self);
        self.stmt("}".into());
    }
    fn ternary(&mut self, c: u32, a: u32, b: u32) -> u32 {
        self.v(format!("ternary v{} ? v{} : v{}", c, a, b))
    }
    fn local_new(&mut self, t: IlType) -> LocalId { let n = self.next; self.next += 1; self.stmt(format!("local L{} : {}", n, ty(t))); LocalId(n) }
    fn local_read(&mut self, l: LocalId) -> u32 { self.v(format!("local_read L{}", l.0)) }
    fn local_write(&mut self, l: LocalId, v: u32) { self.stmt(format!("local_write L{} v{}", l.0, v)) }

    fn call_native(&mut self, s: NativeSlot, args: &[u32]) -> Option<u32> {
        Some(self.v(format!("call_native slot={} args={:?}", s.0, args)))
    }
    fn call_intrinsic(&mut self, id: IntrinsicId, args: &[u32]) -> Option<u32> {
        Some(self.v(format!("call_intrinsic id={} args={:?}", id.0, args)))
    }
    fn unimplemented(&mut self, name: &'static str) { self.stmt(format!("UNIMPL {}", name)) }
}
