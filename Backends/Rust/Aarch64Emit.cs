using ArchCompilerCore;
using LibSharpRetro;
using static Backends.Rust.RustEmit;

namespace Backends.Rust;

// aarch64 per-ISA heads (Rust emit). These are exactly the reg/mem/state/branch
// primitive-class the head-classification census named — they lower to `bd.reg_read/
// reg_write/mem_read/mem_write/call_intrinsic` on the Builder trait. The C# emit
// hardcodes `state->X[i]` etc; here they emit the primitive form.
//
// RegFile ids match Aarch64Scaffold's constants: GPR=0, VEC=1, NZCV=2, SR=3.

public static class Aarch64Emit {
    public static void Register() {
        // ── '=' — the assignment. Dispatches on lhs shape (which reg-file / which primitive). ──
        Statement("=", (c, list) => EmitAssign(c, list, rt: false),
                        (c, list) => EmitAssign(c, list, rt: true));

        // ── reg-file reads (rvalue position) ──
        Expression("gpr32", RegRead("GPR", "IlType::U32", zr: true), RegRead("GPR", "IlType::U32", zr: true));
        Expression("gpr64", RegRead("GPR", "IlType::U64", zr: true), RegRead("GPR", "IlType::U64", zr: true));
        Expression("gpr-or-sp32", RegRead("GPR", "IlType::U32", zr: false), RegRead("GPR", "IlType::U32", zr: false));
        Expression("gpr-or-sp64", RegRead("GPR", "IlType::U64", zr: false), RegRead("GPR", "IlType::U64", zr: false));
        Expression("vec", list => $"bd.reg_read(VEC, ({Ge(list[1])}) as u32, IlType::V128)");
        Expression("vec-b", list => VecLane(list, "IlType::U8"));
        Expression("vec-h", list => VecLane(list, "IlType::U16"));
        Expression("vec-s", list => VecLane(list, "IlType::F32"));
        Expression("vec-d", list => VecLane(list, "IlType::F64"));
        // (nzcv) whole-word read; (nzcv N) individual-flag read
        Expression("nzcv", list => list.Count == 1
            ? "bd.reg_read(NZCV, 0, IlType::U32)"
            : $"bd.reg_read(NZCV, {NzcvFlag(list[1])}, IlType::Bool)");
        Expression("sr", list => $"bd.call_intrinsic(IntrinsicId(0/*sr_read*/), &[{string.Join(", ", list.Skip(1).Select(Lift))}]).unwrap()");
        Expression("pc", _ => "pc");

        // ── memory ──
        Expression("load", list => $"bd.mem_read({Lift(list[1])}, {TypeShort(list.Type)})");
        Statement("store", (c, list) => c += $"bd.mem_write({Lift(list[1])}, {Lift(list[2])});");
        Expression("store", list => $"{{ bd.mem_write({Lift(list[1])}, {Lift(list[2])}); () }}");

        // ── branch / intrinsics ──
        Statement("branch-linked", (c, list) =>
            c += $"bd.branch({Lift(list[1])}, true);");
        Statement("branch-default", (c, list) =>
            c += $"bd.branch(bd.literal(IlType::U64, (pc + 4) as u128), false);");
        Expression("svc", list => "bd.call_intrinsic(IntrinsicId(1/*svc*/), &[]).map(|_|()).unwrap_or(())");
        Statement("breakpoint", (c, list) =>
            c += "bd.call_intrinsic(IntrinsicId(2/*breakpoint*/), &[]);");
        Expression("load-exclusive", list =>
            $"bd.call_intrinsic(IntrinsicId(3/*load_excl*/), &[{Lift(list[1])}]).unwrap()");
        Expression("store-exclusive", list =>
            $"bd.call_intrinsic(IntrinsicId(4/*store_excl*/), &[{Lift(list[1])}, {Lift(list[2])}]).unwrap()");
        Expression("float-to-fixed-point", list =>
            $"bd.call_intrinsic(IntrinsicId(5/*ftfp*/), &[{string.Join(", ", list.Skip(1).Select(Lift))}]).unwrap()");
        Expression("vector-insert", list =>
            $"bd.velement_write(bd.reg_read(VEC, ({Ge(list[1])}) as u32, IlType::V128), {Lift(list[2])}, {Lift(list[3])})");

        // ── compiletime-only (fold-out — but not folded yet in this pipeline; see ‡ below) ──
        // make-wmask/tmask are pure functions of encoded-immediate bits (all compiletime).
        // The RIGHT answer is ArchCompilerCore's compiletime-eval leg folds them to a u64
        // constant BEFORE emit (per DESIGN.md). For rung-4a: emit as a call to a Rust helper
        // fn the generated crate declares (the arch's contract-intrinsic form). ‡ Fold at rung-4b.
        Expression("make-wmask", list =>
            $"aarch64_wmask({U32(list[1])}, {U32(list[2])}, {U32(list[3])}, {U32(list[4])}, {U32(list[5])})");
        Expression("make-tmask", list =>
            $"aarch64_tmask({U32(list[1])}, {U32(list[2])}, {U32(list[3])}, {U32(list[4])}, {U32(list[5])})");
    }

    static string Ge(PTree t) => GenerateExpression(t);
    static string U32(PTree t) => $"({Ge(t)}) as u32";

    // (gprN idx) — reads GPR[idx] as W-typed. `zr` = idx==31 reads-as-zero (aarch64 XZR).
    // The "reads-as-zero" is arch-semantics that COULD be a macro over the primitive in
    // the .isa (per the redesign spec), but for rung-4a it's expressed here (matching the
    // legacy emit's `idx == 31 ? 0 : state.X[idx]` shape) via a ternary on the ct-known idx.
    // NB: idx is compiletime (an insn field), so the ternary is a Rust-compile-time branch.
    static Func<PList, string> RegRead(string file, string ilty, bool zr) => list => {
        var idx = $"({Ge(list[1])}) as u32";
        var read = $"bd.reg_read({file}, {idx}, {ilty})";
        return zr
            ? $"if {idx} == 31 {{ bd.literal({ilty}, 0) }} else {{ {read} }}"
            : read;  // gpr-or-sp: idx==31 reads SP, which the tier's reg_read handles
                     // (RegFile GPR idx=31 = SP by convention; the tier maps it).
    };

    static string VecLane(PList list, string elemTy) =>
        $"bd.velement_read(bd.reg_read(VEC, ({Ge(list[1])}) as u32, IlType::V128), "
        + $"bd.literal(IlType::U32, 0), {elemTy})";

    static string NzcvFlag(PTree p) => ((PName) p).Name switch {
        "n" => "1", "z" => "2", "c" => "3", "v" => "4",
        _ => throw new NotSupportedException($"nzcv flag {p}")
    };

    static void EmitAssign(CodeBuilder c, PList list, bool rt) {
        var rhs = Lift(list[2]);
        if(list[1] is PList sub && sub[0] is PName(var head)) {
            var idx = sub.Count > 1 ? $"({Ge(sub[1])}) as u32" : "0";
            switch(head) {
                case "gpr32" or "gpr64": {
                    var w = head == "gpr32" ? "IlType::U32" : "IlType::U64";
                    // idx==31 = XZR write = discard (aarch64). idx is compiletime.
                    c += $"if {idx} != 31 {{ bd.reg_write(GPR, {idx}, {(head == "gpr32" ? $"bd.cast({rhs}, {w})" : rhs)}); }}";
                    return;
                }
                case "gpr-or-sp32" or "gpr-or-sp64": {
                    var w = head.EndsWith("32") ? "IlType::U32" : "IlType::U64";
                    c += $"bd.reg_write(GPR, {idx}, {(head.EndsWith("32") ? $"bd.cast({rhs}, {w})" : rhs)});";
                    return;
                }
                case "vec":
                    c += $"bd.reg_write(VEC, {idx}, {rhs});";
                    return;
                case "vec-b" or "vec-h" or "vec-s" or "vec-d": {
                    // Write element 0 of a fresh V128 (matching legacy's `{v, 0, 0, ...}` semantics
                    // — the whole vector is replaced, not lane-inserted). ‡ verify: legacy zeros
                    // upper lanes (per the reinterpret_cast<...>{v,0,...}) — so vzero + velement_write.
                    var elemTy = head switch { "vec-b" => "IlType::U8", "vec-h" => "IlType::U16",
                                               "vec-s" => "IlType::F32", _ => "IlType::F64" };
                    c += $"bd.reg_write(VEC, {idx}, "
                       + $"bd.velement_write(bd.literal(IlType::V128, 0), bd.literal(IlType::U32, 0), "
                       + $"bd.cast({rhs}, {elemTy})));";
                    return;
                }
                case "nzcv" when sub.Count == 1:
                    c += $"bd.reg_write(NZCV, 0, {rhs});";
                    return;
                case "nzcv":
                    c += $"bd.reg_write(NZCV, {NzcvFlag(sub[1])}, {rhs});";
                    return;
                case "sr":
                    c += $"bd.call_intrinsic(IntrinsicId(6/*sr_write*/), "
                       + $"&[{string.Join(", ", sub.Skip(1).Select(Lift))}, {rhs}]);";
                    return;
            }
        }
        // bare-name lhs = a `let`-bound local being reassigned → local_write.
        // ‡ rung-4a: emit as a bare Rust `let name = ...;` shadow-rebind for now.
        c += $"let {SafeIdent(((PName) list[1]).Name)} = {rhs};";
    }
}
