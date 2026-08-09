using ArchCompilerCore;
using static Backends.Rust.RustEmit;

namespace Backends.Rust;

// Proof-of-form: 4 heads. The runtime-emit shape (rt) = `bd.method(lhs, rhs)`;
// the compiletime-emit (ct) = bare Rust expr. Rung-4 step ②a: prove the shape,
// then extend to all ~65 core heads.
public static class ScalarMathEmit {
    public static void Register() {
        // Binary arithmetic: ct = bare Rust op (e.g. `(a).wrapping_add(b)` for ints — but the
        // .isa's compiletime-eval already folds these to constants via ArchCompilerCore, so
        // ct here handles the FIELD-EXTRACT arithmetic that stays as generated-code arithmetic).
        // rt = `bd.add(Lift(l), Lift(r))` — Lift() ensures both args are runtime Vals.
        Expression(new[] { "+" },
            list => $"({GenerateExpression(list[1])}).wrapping_add({GenerateExpression(list[2])})",
            list => $"bd.add({Lift(list[1])}, {Lift(list[2])})");
        Expression(new[] { "-" },
            list => $"({GenerateExpression(list[1])}).wrapping_sub({GenerateExpression(list[2])})",
            list => $"bd.sub({Lift(list[1])}, {Lift(list[2])})");
        Expression(new[] { "&" },
            list => $"(({GenerateExpression(list[1])}) & ({GenerateExpression(list[2])}))",
            list => $"bd.and({Lift(list[1])}, {Lift(list[2])})");
        Expression(new[] { "==" },
            list => $"(({GenerateExpression(list[1])}) == ({GenerateExpression(list[2])}))",
            list => $"bd.eq({Lift(list[1])}, {Lift(list[2])})");
    }
}
