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
        Expression("vec", list => Rt($"bd.reg_read(VEC, ({Ge(list[1])}) as u32, IlType::V128)"));
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
        Statement("branch-default", (c, list) => {
            var t = Rt("bd.literal(IlType::U64, (pc + 4) as u128)");
            c += $"bd.branch({t}, false);";
        });
        Expression("svc", list => "bd.call_intrinsic(IntrinsicId(1/*svc*/), &[]).map(|_|()).unwrap_or(())");
        Statement("breakpoint", (c, list) =>
            c += "bd.call_intrinsic(IntrinsicId(2/*breakpoint*/), &[]);");
        Expression("load-exclusive", list =>
            $"bd.call_intrinsic(IntrinsicId(3/*load_excl*/), &[{Lift(list[1])}]).unwrap()");
        Expression("store-exclusive", list =>
            $"bd.call_intrinsic(IntrinsicId(4/*store_excl*/), &[{Lift(list[1])}, {Lift(list[2])}]).unwrap()");
        Expression("float-to-fixed-point", list =>
            $"bd.call_intrinsic(IntrinsicId(5/*ftfp*/), &[{string.Join(", ", list.Skip(1).Select(Lift))}]).unwrap()");
        Expression("vector-insert", list => {
            // READ-MODIFY-WRITE: velement_write RETURNS the modified V128 — it does not
            // store. The v1 emit dropped the result (`let _ = …`), making every
            // lane-insert (LD1-single, INS-class) a silent no-op in the interp; the v3
            // silicon fuzz caught it as interp==pre one byte off. The C# backend's form
            // is an assignment (state.V[rt] = …Element(…)); this mirrors it.
            var rt = Rt($"({Ge(list[1])}) as u32");
            var v = Rt($"bd.reg_read(VEC, {rt}, IlType::V128)");
            var nv = Rt($"bd.velement_write({v}, {Lift(list[2])}, {Lift(list[3])})");
            return $"bd.reg_write(VEC, {rt}, {nv})";
        });

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
        // Linearize: emit both arms into RtSink under a ct-if (idx is a bit-field = ct),
        // return the chosen temp-name. Rust's `if` is an expression so we pick which
        // temp; both bd.* calls go via Rt() so they're on their own lines.
        if(!zr) return Rt($"bd.reg_read({file}, {idx}, {ilty})");
        // XZR case: emit `let _tN = if idx==31 { bd.literal(0) } else { bd.reg_read(...) };`
        // — but that STILL nests bd.* inside `if{}`. Instead: since idx is ct, emit the
        // ct-if to RtSink at STATEMENT grain (like Statement-if does), each arm Rt()s
        // its own call, converge on a shared temp via `let _tN;` predeclare + assign.
        var t = RtName();
        RtSink += $"let {t};";
        RtSink += $"if {idx} == 31 {{";
        RtSink += $"    {t} = bd.literal({ilty}, 0);";
        RtSink += $"}} else {{";
        RtSink += $"    {t} = bd.reg_read({file}, {idx}, {ilty});";
        RtSink += $"}}";
        return t;
    };

    static string VecLane(PList list, string elemTy) {
        var v = Rt($"bd.reg_read(VEC, ({Ge(list[1])}) as u32, IlType::V128)");
        var i0 = Rt("bd.literal(IlType::U32, 0)");
        return $"bd.velement_read({v}, {i0}, {elemTy})";
    }

    static string NzcvFlag(PTree p) => ((PName) p).Name switch {
        "n" => "1", "z" => "2", "c" => "3", "v" => "4",
        _ => throw new NotSupportedException($"nzcv flag {p}")
    };

    static void EmitAssign(CodeBuilder c, PList list, bool rt) {
        // CT-assignment to a bare name short-circuits BEFORE Lift — Lift() on a ct rhs
        // emits a fake runtime literal into the sink as a side effect (`(= T "B")` minted
        // `bd.literal(U32, 0 /* non-numeric */)`), and the value must stay compiletime.
        if(list[1] is PName ctn && !list[2].Type.Runtime && !rt) {
            // `as _`: the shadow-rebind re-TYPED each binding (u8 expr shadowing a u32
            // let); a true assignment can't (E0308), so re-cast to the declared type via
            // inference. Strings/bools assign bare (`as _` is numeric-only).
            var cast = list[2].Type is EInt ? " as _" : "";
            c += $"{SafeIdent(ctn.Name)} = ({Ge(list[2])}){cast};";
            return;
        }
        var rhs = Lift(list[2]);
        if(list[1] is PList sub && sub[0] is PName(var head)) {
            var idx = sub.Count > 1 ? $"({Ge(sub[1])}) as u32" : "0";
            switch(head) {
                case "gpr32" or "gpr64": {
                    var w = head == "gpr32" ? "IlType::U32" : "IlType::U64";
                    var v = head == "gpr32" ? Rt($"bd.cast({rhs}, {w})") : rhs;
                    c += $"if {idx} != 31 {{ bd.reg_write(GPR, {idx}, {v}); }}";
                    return;
                }
                case "gpr-or-sp32" or "gpr-or-sp64": {
                    var w = head.EndsWith("32") ? "IlType::U32" : "IlType::U64";
                    var v = head.EndsWith("32") ? Rt($"bd.cast({rhs}, {w})") : rhs;
                    c += $"bd.reg_write(GPR, {idx}, {v});";
                    return;
                }
                case "vec":
                    c += $"bd.reg_write(VEC, {idx}, {rhs});";
                    return;
                case "vec-b" or "vec-h" or "vec-s" or "vec-d": {
                    var elemTy = head switch { "vec-b" => "IlType::U8", "vec-h" => "IlType::U16",
                                               "vec-s" => "IlType::F32", _ => "IlType::F64" };
                    var z = Rt("bd.literal(IlType::V128, 0)");
                    var i0 = Rt("bd.literal(IlType::U32, 0)");
                    var e = Rt($"bd.cast({rhs}, {elemTy})");
                    var v = Rt($"bd.velement_write({z}, {i0}, {e})");
                    c += $"bd.reg_write(VEC, {idx}, {v});";
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
        // bare-name lhs = a `let`-bound local being REASSIGNED. The rung-4a shadow-rebind
        // (`let name = ...`) was WRONG inside conditional arms: the shadow dies at the
        // arm's brace, so the outer name kept its declaration value — DUP-element-scalar
        // read size=0 through every branch (silicon-diff caught it; the fuzz's v3 arm).
        // let/mlet now declare `let mut`, so this is a true assignment. For a CT rhs the
        // Lift() at the top of this fn was already wrong (it minted a fake runtime
        // literal for `(= T "B")`) — re-derive the ct expression instead.
        c += $"{SafeIdent(((PName) list[1]).Name)} = {rhs};";
    }
}
