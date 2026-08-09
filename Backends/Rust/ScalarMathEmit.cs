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
        Bin("+", "wrapping_add", "add");
        Bin("-", "wrapping_sub", "sub");
        Bin("&", null, "and", ctop: "&");
        // Full binary-op set (transcribed from IRuntimeValue methods). {ct=Rust-op, rt=bd.method}.
        Bin("*", "wrapping_mul", "mul");
        Bin("/", "wrapping_div", "div");
        Bin("%", null, "rem", ctop: "%");
        Bin("|", null, "or", ctop: "|");
        Bin("^", null, "xor", ctop: "^");
        Bin("<<", null, "shl", ctop: "<<");
        Bin(">>", null, "shr", ctop: ">>");
        Bin(">>>", null, "shr", ctop: ">>");  // .isa rotate-right — ‡ needs rotr; shr for rung-4a
        // compares → Bool
        Cmp("==", "eq"); Cmp("!=", "ne");
        Cmp("<", "lt"); Cmp("<=", "le");
        Cmp(">", "gt"); Cmp(">=", "ge");
        // unary
        Expression(new[] { "!" },
            list => $"!{CtBool(list[1])}",
            list => $"bd.not({Lift(list[1])})");
        Expression(new[] { "~" },
            list => $"!({GenerateExpression(list[1])})",
            list => $"bd.not({Lift(list[1])})");
        Expression(new[] { "-!" },
            list => $"({GenerateExpression(list[1])}).wrapping_neg()",
            list => $"bd.neg({Lift(list[1])})");
        Expression("abs", list => $"({GenerateExpression(list[1])}).abs()",
                          list => $"bd.fabs({Lift(list[1])})");
        Expression("sqrt", list => $"({GenerateExpression(list[1])}).sqrt()",
                           list => $"bd.fsqrt({Lift(list[1])})");
        Expression("count-leading-zeros",
            list => $"({GenerateExpression(list[1])}).leading_zeros()",
            list => $"bd.clz({Lift(list[1])})");
        Expression("reverse-bits",
            list => $"({GenerateExpression(list[1])}).reverse_bits()",
            list => $"bd.rbit({Lift(list[1])})");

        // ':' — bit-concat: fold shift+or over children, widths from EType.
        Expression(":", list => {
            var offset = 0;
            var parts = list.Skip(1).Reverse().Select(x => {
                var w = x.Type switch { EInt(_, var wi) => wi, EBool => 1,
                    _ => throw new NotSupportedException($": elem type {x.Type}") };
                var e = $"(({GenerateExpression(x)} as u64) << {offset})";
                offset += w;
                return e;
            }).ToList();
            return $"({string.Join(" | ", parts)})";
        }, list => {
            var resultTy = TypeShort(list.Type);
            var offset = 0;
            var parts = list.Skip(1).Reverse().Select(x => {
                var w = x.Type switch { EInt(_, var wi) => wi, EBool => 1,
                    _ => throw new NotSupportedException($": elem type {x.Type}") };
                var e = $"bd.shl(bd.cast({Lift(x)}, {resultTy}), bd.literal({resultTy}, {offset}))";
                offset += w;
                return e;
            }).ToList();
            return parts.Aggregate((a, b) => $"bd.or({a}, {b})");
        });

        // (bitwidth ty-name) — pure compiletime: type → width integer.
        Expression("bitwidth", list => (BuiltinTypes.TypeFromName(list[1]) switch {
            EInt(_, var w) => w, EFloat(var w) => w, EVector => 128,
            var t => throw new NotSupportedException($"bitwidth {t}")
        }).ToString());

        // (literal expr) — force compiletime-eval. Legacy runs ExecutionState.Evaluate;
        // for rung-4a: since the arg is already ct (that's what `literal` asserts),
        // GenerateExpression on it produces a ct Rust expr. ‡ If the arg needs ACTUAL
        // exec-time folding (e.g. `(literal (+ 1 2))` → `3`), that's ArchCompilerCore's
        // compiletime-eval leg firing pre-emit — deferred to rung-4b's fold-pass.
        Expression("literal", list => GenerateExpression(list[1]));

        Expression("replicate", list =>
            $"aarch64_replicate(({GenerateExpression(list[1])}) as u64, "
            + $"({GenerateExpression(list[2])}) as u32, "
            + $"({(list.Count > 3 ? GenerateExpression(list[3]) : "1")}) as u32)");
    }

    // ct-emit: anchor both operands to the RESULT type (list.Type) via `as`. This kills
    // Rust's ambiguous-{integer} class (unsuffixed literals + wrapping_* need a concrete ty)
    // and normalizes mixed-width operands (the .isa's type-inference computed the result ty).
    static void Bin(string head, string ctMethod, string rtMethod, string ctop = null) =>
        Expression(new[] { head },
            list => {
                var ty = CtType(list.Type);
                var l = $"(({GenerateExpression(list[1])}) as {ty})";
                var r = $"(({GenerateExpression(list[2])}) as {ty})";
                return ctMethod != null ? $"{l}.{ctMethod}({r})" : $"({l} {ctop} {r})";
            },
            list => $"bd.{rtMethod}({Lift(list[1])}, {Lift(list[2])})");

    static void Cmp(string head, string rtMethod) =>
        Expression(new[] { head },
            list => {
                // EString compares (disasm-only, e.g. `r1 == "V"`) — no cast, Rust str-eq works.
                if(list[1].Type is EString || list[2].Type is EString)
                    return $"(({GenerateExpression(list[1])}) {head} ({GenerateExpression(list[2])}))";
                var ty = CtType(list[1].Type);
                return $"((({GenerateExpression(list[1])}) as {ty}) {head} (({GenerateExpression(list[2])}) as {ty}))";
            },
            list => $"bd.{rtMethod}({Lift(list[1])}, {Lift(list[2])})");
}
