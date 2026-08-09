using ArchCompilerCore;
using LibSharpRetro;
using static Backends.Rust.RustEmit;

namespace Backends.Rust;

public static class ControlFlowEmit {
    public static void Register() {
        // (block e1 e2 ... eN) → Rust `{ s1; s2; ...; eN }` (last expr = value if used).
        // Statement-position (the top-level def bodies): each child is a statement.
        Statement("block", (c, list) => {
            foreach(var e in list.Skip(1))
                if(e is PList pl) GenerateStatement(c, pl);
        });
        // Expression-position: emit `{ s1; ...; eN }` inline.
        Expression("block", list => {
            var lines = list.Skip(1).ToList();
            var stmts = lines.Take(lines.Count - 1)
                .Select(e => e is PList pl ? StmtToString(pl) : "");
            return $"{{ {string.Join(" ", stmts)} {GenerateExpression(lines.Last())} }}";
        });

        // (let name value body...) → `let name = value; body...`
        // Compiletime-name binding. In Rust, `let name = <expr>;` then continue.
        Statement("let", (c, list) => {
            var name = RustEmit.SafeIdent(((PName) list[1]).Name);
            c += $"let {name} = {GenerateExpression(list[2])};";
            foreach(var e in list.Skip(3))
                if(e is PList pl) GenerateStatement(c, pl);
        });
        Expression("let", list => {
            var name = RustEmit.SafeIdent(((PName) list[1]).Name);
            var body = list.Skip(3).ToList();
            var stmts = body.Take(body.Count - 1)
                .Select(e => e is PList pl ? StmtToString(pl) : "");
            return $"{{ let {name} = {GenerateExpression(list[2])}; "
                 + $"{string.Join(" ", stmts)} {GenerateExpression(body.Last())} }}";
        });

        // (mlet (n1 v1 n2 v2 ...) body...) → `let n1=v1; let n2=v2; ...; body...`
        // Flat alternating name/value list (not nested pairs).
        Statement("mlet", (c, list) => {
            var binds = (PList) list[1];
            for(var i = 0; i < binds.Count; i += 2)
                c += $"let {RustEmit.SafeIdent(((PName) binds[i]).Name)} = {GenerateExpression(binds[i+1])};";
            foreach(var e in list.Skip(2))
                if(e is PList pl) GenerateStatement(c, pl);
        });
        Expression("mlet", list => {
            var binds = (PList) list[1];
            var bs = new List<string>();
            for(var i = 0; i < binds.Count; i += 2)
                bs.Add($"let {RustEmit.SafeIdent(((PName) binds[i]).Name)} = {GenerateExpression(binds[i+1])};");
            var body = list.Skip(2).ToList();
            var stmts = body.Take(body.Count - 1)
                .Select(e => e is PList pl ? StmtToString(pl) : "");
            return $"{{ {string.Join(" ", bs)} "
                 + $"{string.Join(" ", stmts)} {GenerateExpression(body.Last())} }}";
        });

        // (if cond then else) — statement-form. Compiletime cond → Rust `if`; runtime cond
        // → `bd.cond(c, |b| {then}, |b| {else})`.
        Statement("if", (c, list) => {
            if(list[1].Type.Runtime) {
                c += $"bd.cond({Lift(list[1])},";
                c += $"    &mut |bd| {{ {StmtToString((PList) list[2])} }},";
                var els = list.Count > 3 ? StmtToString((PList) list[3]) : "";
                c += $"    &mut |bd| {{ {els} }});";
            } else {
                c += $"if {CtBool(list[1])} {{";
                c++;
                if(list[2] is PList tl) GenerateStatement(c, tl);
                c--;
                if(list.Count > 3) {
                    c += "} else {";
                    c++;
                    if(list[3] is PList el) GenerateStatement(c, el);
                    c--;
                }
                c += "}";
            }
        });
        Expression("if",
            list => {
                // Both arms coerced to the result type (fixes if/else incompatible-types when
                // arms have different int widths — Rust doesn't auto-widen).
                var els = list.Count > 3 ? list[3] : list[2];
                if(list.Type.Runtime)
                    return $"bd.ternary({Lift(list[1])}, {Lift(list[2])}, {Lift(els)})";
                if(list.Type is EInt or EFloat) {
                    var ty = CtType(list.Type);
                    return $"if {CtBool(list[1])} {{ ({GenerateExpression(list[2])}) as {ty} }} "
                         + $"else {{ ({GenerateExpression(els)}) as {ty} }}";
                }
                // EString/other — no cast (str/unit)
                return $"if {CtBool(list[1])} {{ {GenerateExpression(list[2])} }} "
                     + $"else {{ {GenerateExpression(els)} }}";
            },
            list => $"bd.ternary({Lift(list[1])}, {Lift(list[2])}, "
                  + $"{Lift(list.Count > 3 ? list[3] : list[2])})");

        // (match val (k1 v1) (k2 v2) ... default?) — compiletime dispatch (val is always ct here).
        Statement("match", (c, list) => {
            c += $"match {GenerateExpression(list[1])} {{";
            c++;
            for(var i = 2; i < list.Count; i += 2) {
                if(i + 1 < list.Count) {
                    c += $"{GenerateExpression(list[i])} => {{";
                    c++;
                    if(list[i+1] is PList arm) GenerateStatement(c, arm);
                    c--;
                    c += "}";
                } else {
                    c += "_ => {";
                    c++;
                    if(list[i] is PList arm) GenerateStatement(c, arm);
                    c--;
                    c += "}";
                }
            }
            if(list.Count % 2 == 0)
                c += "_ => unreachable!(),";
            c--;
            c += "}";
        });
        Expression("match", list => {
            // The match KEY is compiletime (a bit-field); the ARMS may be runtime (e.g. the
            // aarch64 condition-decode: `match cond>>1 { 0=>bd.reg_read(NZCV,z), ..., _=>1 }`).
            // If list.Type.Runtime, Lift each arm so all → B::Val (fixes match-arms-incompatible).
            var lift = list.Type.Runtime;
            string Arm(PTree v) => lift ? Lift(v) : GenerateExpression(v);
            var arms = new List<string>();
            for(var i = 2; i < list.Count; i += 2) {
                if(i + 1 < list.Count)
                    arms.Add($"{GenerateExpression(list[i])} => {Arm(list[i+1])}");
                else
                    arms.Add($"_ => {Arm(list[i])}");
            }
            // .isa match may lack a default (all encodable values covered by domain
            // knowledge). Rust requires exhaustive — add an unreachable! arm.
            if(list.Count % 2 == 0)  // even = all arms are (key,val) pairs, no default
                arms.Add(lift ? "_ => { bd.unimplemented(\"match-default\"); unreachable!() }"
                              : "_ => unreachable!()");
            return $"match {GenerateExpression(list[1])} {{ {string.Join(", ", arms)} }}";
        });

        // (when cond body) / (unless cond body) = one-armed if.
        Statement("when", (c, list) => {
            var cond = GenerateExpression(list[1]);
            if(list[1].Type.Runtime) {
                c += $"bd.cond({Lift(list[1])}, &mut |bd| {{ {StmtToString((PList) list[2])} }}, &mut |bd| {{}});";
            } else {
                c += $"if {CtBool(list[1])} {{";
                c++; if(list[2] is PList tl) GenerateStatement(c, tl); c--;
                c += "}";
            }
        });
        Statement("unless", (c, list) => {
            if(list[1].Type.Runtime) {
                c += $"bd.cond({Lift(list[1])}, &mut |bd| {{}}, &mut |bd| {{ {StmtToString((PList) list[2])} }});";
            } else {
                c += $"if !{CtBool(list[1])} {{";
                c++; if(list[2] is PList tl) GenerateStatement(c, tl); c--;
                c += "}";
            }
        });

        Statement("branch", (c, list) => c += $"bd.branch({Lift(list[1])}, false);");

        // (requires cond1 cond2 ...) — decode-time constraint. If any cond fails, this
        // insn's mask/match was a false-positive → fall through to the next candidate.
        // C# emits `goto NextLabel`; Rust has no goto. Since (requires ...) always sits
        // at the top of a decode-block (verified: every occurrence in aarch64.isa is the
        // first stmt(s) of `(block (requires ...) ...)`), and my scaffold's per-insn
        // pattern is `if mask/match { fields; decode; eval; return true; }` with
        // fall-through-on-no-return — a failing requires just needs to skip the rest of
        // the block WITHOUT returning true. Mechanism: wrap the whole insn body in a
        // `'insn: loop { ...; break 'insn; }` and requires emits `break 'insn` on fail.
        // ‡ Simpler for rung-4a: since decode-block is emitted before eval, and requires
        //   is at decode-top: emit a NEGATED early-exit that lets the outer if-block
        //   fall through. But there's no goto — so use the labeled-block trick.
        // Actually simplest: change the scaffold so each insn-block body is inside a
        // labeled block `'d: { ... return true; }` and requires-fail = `break 'd`.
        Statement("requires", (c, list) => {
            var conds = list.Skip(1).Select(x => $"!{CtBool(x)}");
            c += $"if {string.Join(" || ", conds)} {{ break 'decode; }}";
        });
        Expression("unimplemented",
            list => "unreachable!()",
            list => "{ bd.unimplemented(\"(unimplemented)\"); unreachable!() }");
        Statement("assert", (c, list) => { /* decode-time asserts don't emit — they'd be
            debug_assert!() in generated code, but the Rust typechecker + Def parsing
            already gates. Skip for rung-4a; add if a decode-assert failure surfaces. */ });
    }

    // Emit a statement to a string (for closure-body positions like bd.cond arms).
    static string StmtToString(PList pl) {
        var sb = new CodeBuilder();
        GenerateStatement(sb, pl);
        return sb.Code.Trim().Replace("\n", " ").Replace("\t", "");
    }
}
