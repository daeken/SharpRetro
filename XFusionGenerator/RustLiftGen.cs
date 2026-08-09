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
                var e = Expr(l[2]);
                if(Flags.TryGetValue(target, out var fbit)) {
                    // Flag write: coerce to Bool (canonicalize like IlLower.CanonFlag).
                    Emit($"bd.reg_write(EFLAGS, {fbit}, {CanonFlag(e)});");
                    FlagsWritten |= 1u << fbit;
                } else if(Params.Contains(target)) {
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
                // General: emit cond closure. Nested Stmts write into a fresh RtSink.
                Emit($"bd.cond({cond},");
                var savedSink = new List<string>(RtSink); RtSink.Clear();
                var savedInd = Ind; Ind = savedInd + "    ";
                Emit("&mut |bd| {");
                foreach(var f in l.Skip(2)) Stmt(f);
                Emit("},");
                Emit("&mut |bd| { });");
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
            case PName("call"): {
                // push next_pc; branch. (link=false — x86 has no lr; the pushed ret-addr IS the link.)
                var np = Rt("bd.literal(IlType::U64, next_pc as u128)");
                var rsp = Rt("bd.reg_read(GPR, 4, IlType::U64)");
                var eight = Rt("bd.literal(IlType::U64, 8)");
                var rsp2 = Rt($"bd.sub({rsp}, {eight})");
                Emit($"bd.reg_write(GPR, 4, {rsp2});");
                Emit($"bd.mem_write({rsp2}, {np});");
                Emit($"bd.branch({Expr(l[1], 64)}, false);");
                break;
            }
            case PName("ret"): {
                // The .isa's RET body is `(ret (pop))` — pop already yields the target;
                // this stmt just branches to it.
                Emit($"bd.branch({Expr(l[1], 64)}, false);");
                break;
            }
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
                if(Flags.TryGetValue(n, out var fb)) return Rt($"bd.reg_read(EFLAGS, {fb}, IlType::Bool)");
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
            "&" => ("and", false), "|" => ("or", false), "^" => ("xor", false),
            ">>" => ("shr", true), "<<" => ("shl", true), ">>a" => ("shr", true), "rotr" => ("rotr", true),
            "rotl" => ("rotl", true),
            _ => throw new NotSupportedException($"op {head}")
        };
        // >>a: cast operand to signed then shr (Builder.shr picks asr on signed type).
        // ‡ this needs the interp to see a signed IlType — for now emit shr and note.
        var acc = Expr(l[1], ctxW);
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
        var tid = 0;
        var nUnhandled = 0;
        foreach(var t in templates) {
            var g = new RustLiftGen();
            g.Params.AddRange(t.Params);
            g.RtN = 0;
            tmplId[(t.Mnemonic, t.Params.Count)] = tid;

            sb.AppendLine($"/// {t.Mnemonic} ({string.Join(", ", t.Params)})");
            sb.AppendLine($"fn tmpl_{tid}<B: Builder>(bd: &mut B, ops: &[Operand<B::Val>], op_w: u32, next_pc: u64)");
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
            tid++;
        }

        // Dispatch: def_id → bind operands + call template.
        // BodyOrder from RustDisasmGen (must have run FIRST in this Generate call —
        // Program.cs drives disasm before lift).
        sb.AppendLine("pub fn lift_one<B: Builder>(bd: &mut B, d: &DecodedInsn, pc: u64, mode: XMode) -> bool");
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
                    OpClass.Imm => $"bind_imm(d, {immSlot++}, {w})",
                    OpClass.RelBranch => $"bind_rel_branch(d, {immSlot++}, pc, mode)",
                    OpClass.MemOffset => $"Operand::Mem{{addr: {{let a=bd.literal(IlType::U64,d.imm{immSlot++} as u128); a}}, width:{w}}}",
                    OpClass.XmmReg => $"bind_xmm_reg(d, {w})",
                    OpClass.XmmRm or OpClass.XmmRmReg => $"bind_xmm_rm(bd, d, pc, mode, {w})",
                    OpClass.MmxReg => $"bind_xmm_reg(d, 64)",  // ‡ MMX = xmm[i] low-64 approximation for now
                    OpClass.MmxRm => $"bind_xmm_rm(bd, d, pc, mode, 64)",
                    OpClass.ModRmSeg => $"Operand::Reg{{idx: d.m.reg, width:16, high8:false}}",  // ‡ seg-reg via SEG file at v2
                    OpClass.FarPtr => $"bind_imm(d, {(immSlot += 2) - 2}, {w})",  // ‡ far-ptr = imm0 offset only
                    OpClass.FixedInt => $"Operand::Imm{{value:{spec.FixedRegIndex},width:{w}}}",
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
            sb.AppendLine($"            tmpl_{tt}(bd, ops, op_w, next_pc);");
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
        sb.AppendLine("pub const DEF_FLAGS_MASK: &[u32] = &[");
        sb.Append("    0,");
        var col = 1;
        foreach(var def in RustDisasmGen.BodyOrder) {
            var key = (def.Mnemonic, def.Operands.Count);
            var mask = tmplId.TryGetValue(key, out var tt) ? tmplFlagsMask[tt] : 0u;
            sb.Append($" 0x{mask:X3},");
            if(++col % 12 == 0) { sb.AppendLine(); sb.Append("   "); }
        }
        sb.AppendLine();
        sb.AppendLine("];");

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
