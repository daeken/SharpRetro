using System.Text;
using CoreArchCompiler;

namespace XFusionGenerator;

/// Rust-target sibling of `IlLower` — walks each template's eval PTree and emits Rust
/// code that calls the `Builder` trait via `read_operand`/`write_operand`/`bd.*`.
/// One generated fn per TEMPLATE (`fn tmpl_NNN<B: Builder>(bd, ops: &[Operand<B::Val>], op_w, next_pc)`)
/// plus one dispatch (`lift_one(bd, &DecodedInsn, pc, mode)`) that binds operands
/// per-def_id then calls the template fn.
///
/// Head vocabulary mirrors IlLower.Stmt/ListExpr (kept in lockstep — that's the
/// day-4-verified reference). SSA-linearize via RtSink (same discipline as the
/// aarch64 backend's RustEmit): every bd.* → `let _tN = ...;` own-line, return `_tN`.
public class RustLiftGen {
    readonly StringBuilder Sb = new();
    readonly List<string> RtSink = new();
    int RtN;
    readonly Dictionary<string, string> Env = new();  // let/mlet-bound name → Rust expr (usually a _tN)
    readonly List<string> Params = new();             // template param names (positional → ops[i])
    /// Which eflags bits this template's eval body WRITES (via `(= FLAG ...)`).
    /// Emitted as DEF_FLAGS_MASK[def_id] — the per-triple diff-mask (only compare
    /// flags the insn DEFINES; SDM-undefined flags may differ interp vs silicon).
    uint FlagsWritten;
    /// Which eflags bits this template's eval body READS (via bare flag-name in Expr).
    /// Emitted as DEF_FLAGS_READ[def_id] — for block-local dead-flag liveness:
    /// backward-scan `live = (live & ~WRITTEN[i]) | READ[i]`; a flag-write whose bit
    /// isn't in the successor's live-set is DEAD (skip its emit → kills ~5/6 of the
    /// eflags-RMW tax on straight-line code, tier-agnostic).
    uint FlagsRead;
    string OpW = "op_w";                               // the operand v-width (Rust var name)
    string Ind = "        ";

    // ── output emit ────────────────────────────────────────────────────────
    string Rt(string call) {
        var t = $"_t{RtN++}";
        RtSink.Add($"{Ind}let {t} = {call};");
        return t;
    }
    void Emit(string line) { RtSink.Add($"{Ind}{line}"); }
    void Flush() { foreach(var l in RtSink) Sb.AppendLine(l); RtSink.Clear(); }

    string ParamOp(string name) {
        var i = Params.IndexOf(name);
        return i >= 0 ? $"&ops[{i}]" : throw new NotSupportedException($"unbound param {name}");
    }

    static readonly Dictionary<string, int> Flags = new() {
        {"CF",0},{"PF",2},{"AF",4},{"ZF",6},{"SF",7},{"DF",10},{"OF",11}
    };
    static readonly Dictionary<string, int> ArchRegs = new() {
        {"AX",0},{"CX",1},{"DX",2},{"BX",3},{"SP",4},{"BP",5},{"SI",6},{"DI",7}
    };

    // ── PTree walk: statements ─────────────────────────────────────────────
    void Stmt(PTree t) {
        if(t is not PList l || l.Count == 0) throw new NotSupportedException($"stmt {t}");
        switch(l[0]) {
            case PName("mlet"): {
                var pairs = (PList) l[1];
                for(var i = 0; i + 1 < pairs.Count; i += 2) {
                    var nm = ((PName) pairs[i]).Name;
                    Env[nm] = Rt(Expr(pairs[i + 1]));
                }
                foreach(var body in l.Skip(2)) Stmt(body);
                break;
            }
            case PName("let"): {
                var nm = ((PName) l[1]).Name;
                Env[nm] = Rt(Expr(l[2]));
                foreach(var body in l.Skip(3)) Stmt(body);
                break;
            }
            case PName("block"):
                foreach(var f in l.Skip(1)) Stmt(f);
                break;
            case PName("="): {
                var target = ((PName) l[1]).Name;
                if(Flags.TryGetValue(target, out var fbit)) {
                    // Flag write: gate the WHOLE compute on live_flags — if this bit
                    // isn't live at the successor, skip the compute AND the RMW-store.
                    // (Dead-flag-elim: on straight-line code ~5/6 flag-writes are dead
                    //  by the next flag-writer; this kills the biggest tier-0 tax.)
                    FlagsWritten |= 1u << fbit;
                    Emit($"if live_flags & 0x{1u << fbit:X} != 0 {{");
                    var si = Ind; Ind = si + "    ";
                    var ef = Expr(l[2]);
                    Emit($"bd.reg_write(EFLAGS, {fbit}, {CanonFlag(ef)});");
                    Ind = si;
                    Emit("}");
                    break;
                }
                var e = Expr(l[2]);
                if(Params.Contains(target)) {
                    Emit($"write_operand(bd, {ParamOp(target)}, {e});");
                } else if(ArchRegs.TryGetValue(target, out var ar)) {
                    // Direct arch-reg write (SP/AX etc named in .isa) — always at op_w
                    // for these forms; write via a synthetic Reg operand so partial-write
                    // semantics apply.
                    Emit($"write_operand(bd, &Operand::Reg{{idx:{ar}, width:{OpW}, high8:false}}, {e});");
                } else {
                    throw new NotSupportedException($"write target {target}");
                }
                break;
            }
            case PName("if"): {
                // Two forms: CMOVcc-shape (single (= dst src) body) → cond+ternary+write;
                // general → bd.cond closure (like aarch64's rt-if).
                var cond = CanonFlag(Expr(l[1]));
                if(l.Count == 3 && l[2] is PList { Count: 3 } asn && asn[0] is PName("=")
                    && asn[1] is PName(var dn) && Params.Contains(dn)) {
                    var cur = Rt($"read_operand(bd, {ParamOp(dn)})");
                    var val = Expr(asn[2]);
                    var sel = Rt($"bd.ternary({cond}, {val}, {cur})");
                    Emit($"write_operand(bd, {ParamOp(dn)}, {sel});");
                    break;
                }
                // General: emit cond closure. Two forms:
                //   (if cond then)       — then-only; else empty.
                //   (if cond then else)  — both arms (l.Count==4).
                // Multi-stmt bodies wrap in (block ...).
                var thenS = l[2];
                var elseS = l.Count > 3 ? l[3] : null;
                Emit($"bd.cond({cond},");
                var savedSink = new List<string>(RtSink); RtSink.Clear();
                var savedInd = Ind; Ind = savedInd + "    ";
                Emit("&mut |bd| {");
                Stmt(thenS);
                Emit("},");
                Emit("&mut |bd| {");
                if(elseS != null) Stmt(elseS);
                Emit("});");
                var innerSink = new List<string>(RtSink);
                RtSink.Clear(); RtSink.AddRange(savedSink); RtSink.AddRange(innerSink);
                Ind = savedInd;
                break;
            }
            case PName("push"): {
                // Value BEFORE the adjust (push rsp = OLD rsp).
                var v = Rt(Expr(l[1]));
                var rsp = Rt("bd.reg_read(GPR, 4, IlType::U64)");
                var sz = Rt($"bd.literal(IlType::U64, ({OpW} / 8) as u128)");
                var rsp2 = Rt($"bd.sub({rsp}, {sz})");
                Emit($"bd.reg_write(GPR, 4, {rsp2});");
                Emit($"bd.mem_write({rsp2}, {v});");
                break;
            }
            case PName("branch"):
                Emit($"bd.branch({Expr(l[1], 64)}, false);");
                break;
            case PName("vshift-bytes"): {
                // PSRLDQ/PSLLDQ: whole-128 byte-shift by a COMPILE-TIME imm8 (Ib operand).
                // count>15 → 0 per SDM. Since count is known at emit-time (from d.imm),
                // do a Rust-side match: >=16 → literal 0; ==0 → dst unchanged; ==8 → the
                // hi/lo64 swap (common — the wall insn is imm=8); else bd.shr on u128.
                var target = ((PName)l[1]).Name;
                var right = ((PName)l[3]).Name == "r";
                // Get the imm as a Rust-side u8 (Ib operand → d.imm as u8).
                Emit($"let _cnt = if let Operand::Imm{{value,..}} = {ParamOp(((PName)l[2]).Name)} {{ *value as u32 }} else {{ unreachable!() }};");
                var dstE = Expr(l[1]);
                Emit($"let _rv: B::Val = if _cnt >= 16 {{");
                Emit($"    bd.literal(IlType::V128, 0)");
                Emit($"}} else if _cnt == 0 {{ {dstE} }} else {{");
                Emit($"    let vd = bd.bitcast({dstE}, IlType::I{{signed:false,width:128}});");
                Emit($"    let sh = bd.literal(IlType::U64, (_cnt as u128) * 8);");
                Emit($"    let r = bd.{(right?"shr":"shl")}(vd, sh);");
                Emit($"    bd.bitcast(r, IlType::V128)");
                Emit($"}};");
                Emit($"write_operand(bd, {ParamOp(target)}, _rv);");
                break;
            }
            case PName("cdq-cwde"): {
                // (cdq-cwde 0) = CBW/CWDE/CDQE: RAX@op_w = sext(RAX@op_w/2, op_w).
                // (cdq-cwde 1) = CWD/CDQ/CQO: RDX@op_w = RAX >>a (op_w-1) = sign-fill.
                // op_w-parameterized (16/32/64 via prefix/REX.W).
                var which = ((PInt)l[1]).Value;
                if (which == 0) {
                    // Read RAX at half-width, sext to op_w, write RAX at op_w.
                    Emit("let _hw = op_w / 2;");
                    Emit("let _al = bd.reg_read(GPR, 0, ilty(_hw));");
                    Emit("let _sx = bd.sext(_al, ilty(op_w));");
                    Emit("write_operand(bd, &gpr(0, op_w, false), _sx);");
                } else {
                    // RDX@op_w = RAX@op_w >>arith (op_w-1).
                    Emit("let _ra = bd.reg_read(GPR, 0, ilty(op_w));");
                    Emit("let _sn = bd.bitcast(_ra, IlType::I{signed:true, width:op_w as u8});");
                    Emit("let _sh = bd.literal(ilty(op_w), (op_w-1) as u128);");
                    Emit("let _fd = bd.shr(_sn, _sh);");
                    Emit("let _fu = bd.bitcast(_fd, ilty(op_w));");
                    Emit("write_operand(bd, &gpr(2, op_w, false), _fu);");
                }
                return;
            }
            case PName("str-op"): {
                // String-op ONE ITERATION body (movs/stos/lods). rsi/rdi read at u64
                // (address, not op_w); the value is at op_w. DF: step = ±(op_w/8).
                // The REP-wrap happens at def-arm dispatch (below), which emits
                // bd.loop_n(rcx, |bd| tmpl_N(bd, ...)) when d.p.rep, then rcx=0.
                var kind = ((PName) l[1]).Name;
                Emit("let _rdi = bd.reg_read(GPR, 7, IlType::U64);");
                if(kind is "movs" or "lods")
                    Emit("let _rsi = bd.reg_read(GPR, 6, IlType::U64);");
                Emit("let _df = bd.reg_read(EFLAGS, 10, IlType::Bool);");
                Emit($"let _step_p = bd.literal(IlType::U64, (op_w/8) as u128);");
                Emit($"let _step_n = bd.neg(_step_p);");
                Emit("let _step = bd.ternary(_df, _step_n, _step_p);");
                switch(kind) {
                    case "stos":
                        Emit($"let _val = read_operand(bd, &Operand::Reg{{idx:0, width:op_w, high8:false}});");
                        Emit("bd.mem_write(_rdi, _val);");
                        Emit("let _rdi2 = bd.add(_rdi, _step);");
                        Emit("bd.reg_write(GPR, 7, _rdi2);");
                        break;
                    case "movs":
                        Emit($"let _val = bd.mem_read(_rsi, ilty(op_w));");
                        Emit("bd.mem_write(_rdi, _val);");
                        Emit("let _rdi2 = bd.add(_rdi, _step);");
                        Emit("let _rsi2 = bd.add(_rsi, _step);");
                        Emit("bd.reg_write(GPR, 7, _rdi2);");
                        Emit("bd.reg_write(GPR, 6, _rsi2);");
                        break;
                    case "lods":
                        Emit($"let _val = bd.mem_read(_rsi, ilty(op_w));");
                        Emit($"write_operand(bd, &Operand::Reg{{idx:0, width:op_w, high8:false}}, _val);");
                        Emit("let _rsi2 = bd.add(_rsi, _step);");
                        Emit("bd.reg_write(GPR, 6, _rsi2);");
                        break;
                }
                break;
            }
            case PName("div-wide"): {
                // rDX:rAX / src → rAX=quot rDX=rem. 2×op_w dividend. l[2]=#t/#f = signed.
                var signed = l[2] is PName("#t");
                var w2s = $"IlType::I{{signed:{(signed?"true":"false")}, width:(op_w*2) as u8}}";
                Emit("let _dax = bd.reg_read(GPR, 0, IlType::U64);");
                Emit("let _ddx = bd.reg_read(GPR, 2, IlType::U64);");
                // op_w=64: pair128(rdx,rax) directly. op_w=32: dividend is edx:eax = a
                // 64-bit value, fits in u64 → shl(edx,32)|eax then cast to 2×op_w=64.
                // op_w=16 similar. Handle both via a match on op_w.
                Emit($"let _dvd = if op_w == 64 {{");
                Emit($"    let p = bd.pair128(_ddx, _dax);");
                Emit(signed ? $"    bd.bitcast(p, {w2s})" : "    p");
                Emit($"}} else {{");
                Emit($"    let sh = bd.literal(IlType::U64, op_w as u128);");
                Emit($"    let hs = bd.shl(_ddx, sh);");
                Emit($"    let d64 = bd.or(hs, _dax);");
                Emit($"    bd.cast(d64, {w2s})");
                Emit($"}};");
                var srcE = Expr(l[1]);
                var srcW = signed ? $"bd.sext({srcE}, {w2s})" : $"bd.cast({srcE}, {w2s})";
                Emit($"let _dvs = {srcW};");
                Emit($"let _q = bd.div(_dvd, _dvs);");
                Emit($"let _r = bd.rem(_dvd, _dvs);");
                Emit($"let _qn = bd.cast(_q, ilty(op_w));");
                Emit($"let _rn = bd.cast(_r, ilty(op_w));");
                Emit("bd.reg_write(GPR, 0, _qn);");
                Emit("bd.reg_write(GPR, 2, _rn);");
                // ‡ #DE on div-by-0 / overflow not raised — result 0 (aarch64 udiv semantics).
                break;
            }
            case PName("mul-wide"): {
                // rAX × src → rDX:rAX (2×op_w product). CF=OF = hi≠0.
                var signed = l[2] is PName("#t");
                var w2 = $"IlType::I{{signed:{(signed?"true":"false")}, width:(op_w*2) as u8}}";
                var wu = $"ilty(op_w)";
                Emit($"let _max = bd.reg_read(GPR, 0, {wu});");
                var maxW = signed ? $"bd.sext(_max, {w2})" : $"bd.cast(_max, {w2})";
                Emit($"let _mA = {maxW};");
                var srcE = Expr(l[1]);
                var srcW = signed ? $"bd.sext({srcE}, {w2})" : $"bd.cast({srcE}, {w2})";
                Emit($"let _mB = {srcW};");
                Emit($"let _p = bd.mul(_mA, _mB);");
                // Extract hi/lo: at op_w=64 use hi64/lo64 (2-slot direct); at op_w<64
                // the product fits in u64 → shr by op_w for hi.
                Emit($"let (_lo, _hi) = if op_w == 64 {{");
                Emit($"    let l = bd.lo64(_p); let h = bd.hi64(_p); (l, h)");
                Emit($"}} else {{");
                Emit($"    let l = bd.cast(_p, {wu});");
                Emit($"    let sh = bd.literal({w2}, op_w as u128);");
                Emit($"    let ph = bd.shr(_p, sh);");
                Emit($"    let h = bd.cast(ph, {wu}); (l, h)");
                Emit($"}};");
                Emit("bd.reg_write(GPR, 0, _lo);");
                Emit("bd.reg_write(GPR, 2, _hi);");
                // CF=OF = hi≠0 (unsigned) or hi≠sext(lo>>op_w-1, op_w) (signed — meaning the
                // full product doesn't fit in op_w). ‡ v1: unsigned form only; IMUL-1arg later.
                Emit($"let _z = bd.literal({wu}, 0);");
                Emit($"let _hnz = bd.ne(_hi, _z);");
                Emit($"if live_flags & 0x1 != 0 {{ bd.reg_write(EFLAGS, 0, _hnz); }}");
                Emit($"if live_flags & 0x800 != 0 {{ bd.reg_write(EFLAGS, 11, _hnz); }}");
                FlagsWritten |= 0x801;
                break;
            }
            case PName("call"):
            case PName("ret"):
                // Per IlLower: call/ret are BRANCH MARKERS only (BranchKind.Call/Ret for
                // the arch-neutral CFG scanner). The .isa body does the push/pop as a
                // SEPARATE `(push (next-pc))` / `(pop)` stmt. Composed-from-memory bug
                // (had this arm doing push+branch → double-push, rsp-16 not -8; caught
                // by the loader integration's rsp-delta observation).
                Emit($"bd.branch({Expr(l[1], 64)}, false);");
                break;
            case PName("branch-if"): {
                var cond = CanonFlag(Expr(l[1]));
                var tgt = Expr(l[2], 64);
                var ft = Rt("bd.literal(IlType::U64, next_pc as u128)");
                // bd.cond(c, |b|b.branch(tgt), |b|b.branch(next_pc)) — mirrors aarch64 B.cond emit.
                Emit($"bd.cond({cond},");
                Emit($"    &mut |bd| {{ let _tt = {tgt}; bd.branch(_tt, false); }},");
                Emit($"    &mut |bd| {{ bd.branch({ft}, false); }});");
                break;
            }
            case PName("intrinsic"): {
                var name = ((PName) l[1]).Name;
                var args = l.Skip(2).Select(a => Expr(a)).ToList();
                var argsRust = args.Count == 0 ? "&[]" : $"&[{string.Join(", ", args)}]";
                // ‡ intrinsic-id assignment: hash the name for now (v2: a proper enum).
                var id = Math.Abs(name.GetHashCode()) % 1000;
                Emit($"bd.call_intrinsic(IntrinsicId({id} /*{name}*/), {argsRust});");
                break;
            }
            default:
                throw new NotSupportedException($"stmt head {l[0]}");
        }
    }

    // ── expressions ────────────────────────────────────────────────────────
    string Expr(PTree t) => Expr(t, OpW);

    string Expr(PTree t, object ctxW) {
        switch(t) {
            case PInt(var v): {
                // Literal width = wide enough to HOLD the value, not ctxW. The .isa relies
                // on C#-style int-promotion (e.g. PF's `(>> 0x9669 idx)` — a 16-bit lookup
                // table). Emitting 0x9669 at ilty(op_w=8) → literal(U8) truncates to 0x69
                // → PF wrong on all byte-form insns. Caught by the Rosetta silicon-oracle
                // (SUB cl,dl → PF=0 vs silicon PF=1); invisible to interp-vs-C# only if C#
                // shares the truncation (it doesn't — IlLower uses IlInt at natural width).
                // Fix: literals ≥256 → U32 (fits every .isa constant); <256 → ctxW (so
                // small immediates still participate at operand-width for cmp/etc).
                var lv = (long) v;
                var w = (ulong)lv < 256 ? $"ilty({ctxW})"
                      : (ulong)lv <= uint.MaxValue ? "IlType::U32" : "IlType::U64";
                return Rt($"bd.literal({w}, {(ulong)lv}u128)");
            }
            case PName(var n):
                if(Env.TryGetValue(n, out var bound)) return bound;
                if(Params.Contains(n)) return Rt($"read_operand(bd, {ParamOp(n)})");
                if(Flags.TryGetValue(n, out var fb)) {
                    FlagsRead |= 1u << fb;
                    return Rt($"bd.reg_read(EFLAGS, {fb}, IlType::Bool)");
                }
                if(ArchRegs.TryGetValue(n, out var ar))
                    return Rt($"read_operand(bd, &Operand::Reg{{idx:{ar}, width:{OpW}, high8:false}})");
                throw new NotSupportedException($"name {n}");
            case PList l when l.Count >= 1: return ListExpr(l, ctxW);
            default: throw new NotSupportedException($"expr {t}");
        }
    }

    string ListExpr(PList l, object ctxW) {
        var head = l[0] is PName(var h) ? h : throw new NotSupportedException(l[0].ToString());
        switch(head) {
            case "u8":  return Rt($"bd.cast({Expr(l[1])}, IlType::U8)");
            case "clz": return Rt($"bd.clz({Expr(l[1])})");
            case "rbit": return Rt($"bd.rbit({Expr(l[1])})");
            case "f64": return Rt($"bd.cast({Expr(l[1])}, IlType::F{{width:64}})");
            case "f32": return Rt($"bd.cast({Expr(l[1])}, IlType::F{{width:32}})");
            // (as-f64 x): reinterpret u64 bits AS f64 (bitcast, no conversion).
            // For reading Wsd/Wss operands (xmm-lane bits) as float before fcvtzs/fcvt.
            case "fsqrt": return Rt($"bd.fsqrt({Expr(l[1])})");
            case "fisnan": return Rt($"bd.fisnan({Expr(l[1])})");
            case "fcmpp": {
                // (fcmpp a b pred-op w) — CMPSS/SD 8-predicate compare → mask.
                // pred-op is the Ib operand; extract its .value at Rust-runtime.
                var a = Expr(l[1]); var b = Expr(l[2]);
                var predOp = ParamOp(((PName)l[3]).Name);
                var w = ((PInt)l[4]).Value;
                var predv = Rt($"if let Operand::Imm{{value,..}} = {predOp} {{ *value as u32 }} else {{ unreachable!() }}");
                return Rt($"bd.fcmpp({a}, {b}, {predv}, {w})");
            }
            case "fmax": return Rt($"bd.fminmax({Expr(l[1])}, {Expr(l[2])}, true)");
            case "fmin": return Rt($"bd.fminmax({Expr(l[1])}, {Expr(l[2])}, false)");
            case "flt": return Rt($"bd.lt({Expr(l[1])}, {Expr(l[2])})");
            case "feq": return Rt($"bd.eq({Expr(l[1])}, {Expr(l[2])})");
            case "as-f64": return Rt($"bd.bitcast({Expr(l[1])}, IlType::F{{width:64}})");
            case "as-f32": return Rt($"bd.bitcast({Expr(l[1])}, IlType::F{{width:32}})");
            case "int-of": {
                // (int-of W v) — float→signed-int-of-W-bits (truncate). CVTTSD2SI etc.
                var w = l[1] is PInt(var wv3) ? wv3.ToString() : OpW;
                return Rt($"bd.cast({Expr(l[2])}, IlType::I{{signed:true, width:{w} as u8}})");
            }
            case "signed": {
                // (signed W v) — reinterpret v as signed int of W bits (no bit change,
                //  just type; used before div/rem for IDIV, and before f64 for CVTSI2SD).
                var w = l[1] is PInt(var wv4) ? wv4.ToString() : OpW;
                return Rt($"bd.bitcast({Expr(l[2])}, IlType::I{{signed:true, width:{w} as u8}})");
            }
            case "u16": return Rt($"bd.cast({Expr(l[1])}, IlType::U16)");
            case "u32": return Rt($"bd.cast({Expr(l[1])}, IlType::U32)");
            case "u64": return Rt($"bd.cast({Expr(l[1])}, IlType::U64)");

            case "pop": {
                var rsp = Rt("bd.reg_read(GPR, 4, IlType::U64)");
                var v = Rt($"bd.mem_read({rsp}, ilty({OpW}))");
                var sz = Rt($"bd.literal(IlType::U64, ({OpW} / 8) as u128)");
                var rsp2 = Rt($"bd.add({rsp}, {sz})");
                Emit($"bd.reg_write(GPR, 4, {rsp2});");
                return v;
            }
            case "next-pc":
                return Rt("bd.literal(IlType::U64, next_pc as u128)");
            case "addr-of": {
                var opName = ((PName) l[1]).Name;
                // The Operand::Mem's addr field. Requires the operand to be Mem — assert at runtime.
                return Rt($"match *{ParamOp(opName)} {{ Operand::Mem{{addr,..}} => addr, _ => panic!(\"addr-of non-mem\") }}");
            }
            case "sext": {
                var a = Expr(l[1]);
                var w = l.Count > 2 && l[2] is PInt(var wv) ? wv.ToString() : OpW;
                return Rt($"bd.sext({a}, ilty({w}))");
            }
            case ":": {
                // Bit-concat, MSB-first: (: hi lo) → (widen(hi) << width(lo)) | widen(lo).
                // Widths from the type-inferred sum (CoreArchCompiler ScalarMath's ':'
                // sig = EInt(false, sum-of-widths)). For DIV: (: DX AX) at op_w=64 → u128.
                // Emit iteratively L→R (each: acc = (widen(acc) << w_i) | widen(elem_i)).
                var totalW = ((EInt) l.Type).Width;
                var wideT = totalW switch { <=32 => "IlType::U32", <=64 => "IlType::U64",
                                            _ => "IlType::I{signed:false, width:128}" };
                var cacc = Rt($"bd.cast({Expr(l[1])}, {wideT})");
                for(var i = 2; i < l.Count; i++) {
                    var elemW = l[i].Type is EInt(_, var ew) ? ew : 1;
                    var sh = Rt($"bd.literal({wideT}, {elemW})");
                    var elem = Rt($"bd.cast({Expr(l[i])}, {wideT})");
                    cacc = Rt($"bd.or(bd.shl({cacc}, {sh}), {elem})");
                }
                return cacc;
            }
            case "vzip": {
                var a = Expr(l[1]); var b = Expr(l[2]);
                var ew = ((PInt)l[3]).Value;
                var hi = l[4] is PName("#t") ? "true" : "false";
                return Rt($"bd.vzip({a}, {b}, {ew}, {hi})");
            }
            case "vshufw": {
                // (vshufw src sel-op hi) — PSHUFLW/PSHUFHW.
                var a = Expr(l[1]);
                var selOp = ParamOp(((PName)l[2]).Name);
                var hi = l[3] is PName("#t") ? "true" : "false";
                var selv = Rt($"if let Operand::Imm{{value,..}} = {selOp} {{ *value as u32 }} else {{ unreachable!() }}");
                return Rt($"bd.vshufw({a}, {selv}, {hi})");
            }
            case "vshuf": {
                // (vshuf a b sel-op elw) — SHUFPS/PSHUFD/SHUFPD lane-select.
                // sel-op is the Ib operand (compile-time-known); extract its
                // .value at RUST-runtime (per-decode) and pass as u32.
                // a supplies low-half lanes, b supplies high-half. PSHUFD passes
                // src as BOTH a and b (all lanes from src). SHUFPS passes dst,src.
                var a = Expr(l[1]); var b = Expr(l[2]);
                var selOp = ParamOp(((PName)l[3]).Name);
                var ew = ((PInt)l[4]).Value;
                var selv = Rt($"if let Operand::Imm{{value,..}} = {selOp} {{ *value as u32 }} else {{ unreachable!() }}");
                return Rt($"bd.vshuf({a}, {b}, {ew}, {selv})");
            }
            case "vcvt": {
                // (vcvt a kind) — packed convert on V128. kind 0..4.
                var a = Expr(l[1]); var kind = ((PInt)l[2]).Value;
                return Rt($"bd.vcvt({a}, {kind})");
            }
            case "vfmax": case "vfmin": {
                var a = Expr(l[1]); var b = Expr(l[2]); var ew = ((PInt)l[3]).Value;
                var m = head == "vfmax" ? "true" : "false";
                return Rt($"bd.vfminmax({a}, {b}, {ew}, {m})");
            }
            case "vfun": {
                var a = Expr(l[1]); var ew = ((PInt)l[2]).Value; var op = ((PInt)l[3]).Value;
                return Rt($"bd.vfun({a}, {ew}, {op})");
            }
            case "vmovmsk": {
                // (vmovmsk src ew) → U32 bitmask of per-lane sign bits.
                var a = Expr(l[1]); var ew = ((PInt)l[2]).Value;
                return Rt($"bd.vmovmsk({a}, {ew})");
            }
            case "vibin": {
                // (vibin a b ew op) — packed-int add/sub/mul.
                var a = Expr(l[1]); var b = Expr(l[2]);
                var ew = ((PInt)l[3]).Value; var op = ((PInt)l[4]).Value;
                return Rt($"bd.vibin({a}, {b}, {ew}, {op})");
            }
            case "vfbin": {
                // (vfbin a b elw op) — packed-float per-lane arith on V128 → V128.
                // Expression head; .isa does (= dst (vfbin dst src elw op)) for RMW.
                // elw ∈ {32,64}, op ∈ {0=add,1=sub,2=mul,3=div}.
                var a = Expr(l[1]); var b = Expr(l[2]);
                var ew = ((PInt)l[3]).Value; var op = ((PInt)l[4]).Value;
                return Rt($"bd.vfbin({a}, {b}, {ew}, {op})");
            }
            case "zext": {
                var a = Expr(l[1]);
                var w = l.Count > 2 && l[2] is PInt(var wv2) ? wv2.ToString() : OpW;
                return Rt($"bd.cast({a}, ilty({w}))");
            }
            case "~": return Rt($"bd.not({Expr(l[1], ctxW)})");
            case "!": {
                var x = Expr(l[1]);
                return Rt($"bd.not({x})");  // ‡ Bool-not (interp handles Bool→!bool). If x
                                            //   isn't Bool, IlLower does ==0 — coerce here too:
                                            //   actually: cast to Bool first for safety.
            }
            case "bitwidth": {
                // Operand's width as a Val (u32 literal). ops[i].width() is a Rust-side u32.
                var opName = l[1] is PName(var pn) ? pn : throw new();
                var wexpr = Params.Contains(opName) ? $"ops[{Params.IndexOf(opName)}].width()" : OpW;
                return Rt($"bd.literal(IlType::U32, {wexpr} as u128)");
            }
        }
        // comparisons → Bool
        if(head is "<" or "==" or "!=" or ">") {
            var a = Expr(l[1], ctxW);
            var b = Expr(l[2], ctxW);
            var op = head switch { "<" => "lt", "==" => "eq", "!=" => "ne", ">" => "gt", _ => "" };
            return Rt($"bd.{op}({a}, {b})");
        }
        // binary/nary
        var (rtop, isShift) = head switch {
            "+" => ("add", false), "-" => ("sub", false), "*" => ("mul", false),
            "/" => ("div", false), "%" => ("rem", false),
            "&" => ("and", false), "|" => ("or", false), "^" => ("xor", false),
            ">>" => ("shr", true), "<<" => ("shl", true), ">>a" => ("shr", true), "rotr" => ("rotr", true),
            "rotl" => ("rotl", true),
            _ => throw new NotSupportedException($"op {head}")
        };
        // >>a: arithmetic right-shift. Builder.shr does asr when the operand's IlType is
        // signed — so cast to signed at op_w, shr, cast back. (SAR Ev,Ib silicon-caught:
        // was stubbed as logical shr → top bits zero-filled instead of sign-filled.)
        var acc = Expr(l[1], ctxW);
        if(head == ">>a") {
            var sty = $"IlType::I{{signed:true, width:{OpW} as u8}}";
            acc = Rt($"bd.cast({acc}, {sty})");
        }
        for(var i = 2; i < l.Count; i++) {
            var rhs = Expr(l[i], ctxW);
            if(rtop == "rotl") {
                // rotl x n = rotr x (w-n). Width from bd.ty_of(acc)? Simpler: use op_w.
                var w = Rt($"bd.literal(IlType::U8, {OpW} as u128)");
                var d = Rt($"bd.sub({w}, {rhs})");
                acc = Rt($"bd.rotr({acc}, {d})");
            } else {
                acc = Rt($"bd.{rtop}({acc}, {rhs})");
            }
        }
        if(head == ">>a") {
            // Cast back to unsigned so downstream ops see the expected type.
            acc = Rt($"bd.cast({acc}, ilty({OpW}))");
        }
        return acc;
    }

    string CanonFlag(string e) {
        // If e is already a Bool-typed temp (from a cmp or flag-read), pass through.
        // Otherwise cast to Bool. ‡ We don't track types here (RustEmit does via list.Type);
        // for now always cast — bd.cast(Bool→Bool) is a no-op in interp.
        return Rt($"bd.cast({e}, IlType::Bool)");
    }

    // ── template + dispatch generation ────────────────────────────────────
    public static string Generate(List<XFusionDef.Template> templates, List<XFusionDef> defs) {
        var sb = new StringBuilder();
        sb.AppendLine("// GENERATED by XFusionGenerator/RustLiftGen — do not edit.");
        sb.AppendLine("#![allow(unused_variables, unused_parens, non_snake_case, unused_mut, dead_code)]");
        sb.AppendLine("use crate::decode::{DecodedInsn, XMode};");
        sb.AppendLine("use crate::operand::*;");
        sb.AppendLine("use sharpretro_jit::{Builder, IlType, RegFile, IntrinsicId};");
        sb.AppendLine();

        // Per-template body fns. Key by (Mnemonic, arity) — templates with the same
        // mnemonic but different param-counts (rare) get separate bodies.
        var tmplId = new Dictionary<(string, int), int>();
        var tmplFlagsMask = new Dictionary<int, uint>();
        var tmplFlagsRead = new Dictionary<int, uint>();
        var tid = 0;
        var nUnhandled = 0;
        foreach(var t in templates) {
            var g = new RustLiftGen();
            g.Params.AddRange(t.Params);
            g.RtN = 0;
            tmplId[(t.Mnemonic, t.Params.Count)] = tid;

            sb.AppendLine($"/// {t.Mnemonic} ({string.Join(", ", t.Params)})");
            sb.AppendLine($"fn tmpl_{tid}<B: Builder>(bd: &mut B, ops: &[Operand<B::Val>], op_w: u32, next_pc: u64, live_flags: u32)");
            sb.AppendLine($"    where B::Val: Copy");
            sb.AppendLine($"{{");
            try {
                if(t.Eval.Count > 0 && t.Eval[0] is PName("block"))
                    foreach(var f in t.Eval.Skip(1)) g.Stmt(f);
                else
                    g.Stmt(t.Eval);
            } catch(Exception ex) {
                nUnhandled++;
                g.RtSink.Clear();
                g.Emit($"// UNHANDLED: {ex.Message.Replace('\n',' ').Replace('"',' ')}");
                g.Emit($"bd.unimplemented(\"{t.Mnemonic}\");");
            }
            g.Flush();
            sb.Append(g.Sb);
            sb.AppendLine("}");
            sb.AppendLine();
            tmplFlagsMask[tid] = g.FlagsWritten;
            tmplFlagsRead[tid] = g.FlagsRead;
            tid++;
        }

        // Dispatch: def_id → bind operands + call template.
        // BodyOrder from RustDisasmGen (must have run FIRST in this Generate call —
        // Program.cs drives disasm before lift).
        // `live_flags`: which eflags bits are LIVE at this insn's exit (i.e. read by
        // some successor before overwritten). A flag-write whose bit isn't live is
        // dead → its compute+store is skipped. Callers that don't do liveness pass
        // FLAGS_ALL_LIVE (= emit everything, the pre-dead-flag-elim behavior).
        sb.AppendLine("pub const FLAGS_ALL_LIVE: u32 = 0xFFF;");
        sb.AppendLine();
        sb.AppendLine("pub fn lift_one<B: Builder>(bd: &mut B, d: &DecodedInsn, pc: u64, mode: XMode, live_flags: u32) -> bool");
        sb.AppendLine("    where B::Val: Copy");
        sb.AppendLine("{");
        sb.AppendLine("    let next_pc = pc.wrapping_add(d.len as u64);");
        sb.AppendLine("    match d.def_id {");
        foreach(var (def, defId) in RustDisasmGen.BodyOrder.Select((d, i) => (d, i + 1))) {
            var key = (def.Mnemonic, def.Operands.Count);
            if(!tmplId.TryGetValue(key, out var tt)) continue;  // no template for this arity — skip
            sb.AppendLine($"        {defId} => {{  // {def.Mnemonic} {string.Join(",", def.Operands.Select(o => o.Text))}");
            // op_w: the encoding's principal width (per D64/v-width).
            var opw = def.D64 ? "d.p.v_width_d64(mode)" : "d.p.v_width(mode)";
            // Byte-form encodings (Eb/Gb/Ib etc): op_w=8 regardless. Detect: any operand
            // with width==8 or all operands byte-form.
            if(def.Operands.Any() && def.Operands.All(o => o.ByteWidth() == 8))
                opw = "8";
            sb.AppendLine($"            let op_w = {opw};");
            // Bind each operand per its OpClass → Operand<B::Val>.
            var immSlot = 0;
            var binds = new List<string>();
            foreach(var spec in def.Operands) {
                var w = spec.ByteWidth() switch {
                    8 => "8", 16 => "16", 32 => "32", 64 => "64", 128 => "128",
                    _ => "op_w",  // v-width parameterized
                };
                var b = spec.Class switch {
                    OpClass.ModRmRm => $"bind_modrm_rm(bd, d, pc, mode, {w})",
                    OpClass.ModRmReg => $"bind_modrm_reg(d, {w})",
                    OpClass.OpcodeReg => $"bind_opcode_reg(d, {w})",
                    OpClass.FixedReg => $"bind_fixed_reg({spec.FixedRegIndex}, {w})",
                    // Imm operand: bind at op_w, NOT the encoded imm width. d.imm0/1 are
                    // already sign-extended to i64 by the decoder (Ib-sx in an Ev context
                    // per SDM: opcode-83 arith, C1 shifts, etc). The template consumes the
                    // imm at the DESTINATION's width (add/cmp/mov to op_w-wide reg), so the
                    // Operand::Imm.width should be op_w — otherwise bd.literal(U8, sext'd-i64)
                    // masks to 8 bits and loses the sign (add r64, -4 → adds 0xFC not
                    // 0xFF..FC). tier-0 was correct-by-accident (its literal doesn't mask);
                    // interp+tier-1 mask and were both silently wrong on negative imm8.
                    // Byte-form encodings (Eb,Ib) have op_w=8 anyway → no change there.
                    OpClass.Imm => $"bind_imm(d, {immSlot++}, op_w)",
                    OpClass.RelBranch => $"bind_rel_branch(d, {immSlot++}, pc, mode)",
                    OpClass.MemOffset => $"Operand::Mem{{addr: {{let a=bd.literal(IlType::U64,d.imm{immSlot++} as u128); a}}, width:{w}}}",
                    OpClass.XmmReg => $"bind_xmm_reg(d, {w})",
                    OpClass.XmmRm or OpClass.XmmRmReg => $"bind_xmm_rm(bd, d, pc, mode, {w})",
                    OpClass.MmxReg => $"bind_xmm_reg(d, 64)",  // ‡ MMX = xmm[i] low-64 approximation for now
                    OpClass.MmxRm => $"bind_xmm_rm(bd, d, pc, mode, 64)",
                    OpClass.ModRmSeg => $"Operand::Reg{{idx: d.m.reg, width:16, high8:false}}",  // ‡ seg-reg via SEG file at v2
                    OpClass.FarPtr => $"bind_imm(d, {(immSlot += 2) - 2}, {w})",  // ‡ far-ptr = imm0 offset only
                    OpClass.FixedInt => $"Operand::Imm{{value:{spec.FixedValue},width:{w}}}",
                    OpClass.StrSrc or OpClass.StrDst => $"Operand::Mem{{addr: bd.reg_read(GPR, {(spec.Class==OpClass.StrSrc?6:7)}, IlType::U64), width:{w}}}",
                    OpClass.XmmVvvv => $"Operand::Xmm{{idx: d.p.vex_vvvv, width:{w}}}",
                    OpClass.GprVvvv => $"gpr(d.p.vex_vvvv, {w}, true)",
                    OpClass.FpuTop or OpClass.FpuSti or OpClass.MaskReg or OpClass.MaskRm =>
                        $"{{bd.unimplemented(\"{spec.Class}\"); Operand::Imm{{value:0,width:8}}}}",
                    _ => $"panic!(\"bind {spec.Class}\") /* unhandled OpClass */"
                };
                binds.Add(b);
            }
            sb.AppendLine($"            let ops: &[Operand<B::Val>] = &[{string.Join(", ", binds)}];");
            // String-ops: wrap the single-iter template in a rcx-loop when d.p.rep.
            // (movs/stos/lods take plain rep. scas/cmps still intrinsic — repe/repne
            //  need ZF early-exit; wire when the recon surfaces one.)
            var isStringOp = def.Operands.Any(o => o.Class is OpClass.StrSrc or OpClass.StrDst);
            if(isStringOp) {
                sb.AppendLine($"            if d.p.rep || d.p.rep_nz {{");
                sb.AppendLine($"                let rcx = bd.reg_read(GPR, 1, IlType::U64);");
                sb.AppendLine($"                bd.loop_n(rcx, &mut |bd| {{ tmpl_{tt}(bd, ops, op_w, next_pc, live_flags); }});");
                sb.AppendLine($"                let z = bd.literal(IlType::U64, 0);");
                sb.AppendLine($"                bd.reg_write(GPR, 1, z);");
                sb.AppendLine($"            }} else {{");
                sb.AppendLine($"                tmpl_{tt}(bd, ops, op_w, next_pc, live_flags);");
                sb.AppendLine($"            }}");
            } else {
                sb.AppendLine($"            tmpl_{tt}(bd, ops, op_w, next_pc, live_flags);");
            }
            sb.AppendLine($"            true");
            sb.AppendLine($"        }}");
        }
        sb.AppendLine("        _ => false,");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        sb.AppendLine();

        // Per-def_id defined-flags mask: which eflags bits the insn's template
        // WRITES. The Rosetta-oracle diff-mask — only compare flags the insn
        // DEFINES (SDM-undefined flags may legitimately differ interp vs silicon;
        // e.g. AF after AND/OR/XOR, OF after shift-by-N≠1, SF/ZF/AF/PF after MUL).
        // [0] unused (def_ids are 1-based).
        void EmitFlagsTable(string name, Dictionary<int, uint> src) {
            sb.AppendLine($"pub const {name}: &[u32] = &[");
            sb.Append("    0,");
            var col = 1;
            foreach(var def in RustDisasmGen.BodyOrder) {
                var key = (def.Mnemonic, def.Operands.Count);
                var mask = tmplId.TryGetValue(key, out var tt) ? src[tt] : 0u;
                sb.Append($" 0x{mask:X3},");
                if(++col % 12 == 0) { sb.AppendLine(); sb.Append("   "); }
            }
            sb.AppendLine();
            sb.AppendLine("];");
        }
        EmitFlagsTable("DEF_FLAGS_MASK", tmplFlagsMask);
        // Which eflags bits each def's template READS. For block-local backward liveness:
        //   live_out = ALL (conservative at block boundary);
        //   for i in (n-1)..=0: live_in[i] = (live_out & !MASK[i]) | READ[i]; live_out = live_in[i];
        // A flag-write whose bit isn't in live_out (the NEXT insn's live_in) is dead → skip emit.
        EmitFlagsTable("DEF_FLAGS_READ", tmplFlagsRead);

        Console.Error.WriteLine($"[lift-gen: {tid} templates ({nUnhandled} UNHANDLED), {RustDisasmGen.BodyOrder.Count} def-arms]");
        return sb.ToString();
    }
}

// ── OperandSpec extensions the generator needs ────────────────────────────
public static class OperandSpecExt {
    /// Concrete byte-width for this spec, or 0 if v-width-parameterized.
    public static int ByteWidth(this OperandSpec s) => s.Width switch {
        WCode.b => 8, WCode.w => 16, WCode.d => 32, WCode.q => 64,
        WCode.dq or WCode.ps or WCode.pd or WCode.x => 128,
        WCode.ss => 32, WCode.sd => 64,
        _ => 0,  // v/z/y = mode+prefix-parameterized → op_w
    };
}
