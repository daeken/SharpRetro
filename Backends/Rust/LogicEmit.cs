using ArchCompilerCore;
using static Backends.Rust.RustEmit;

namespace Backends.Rust;

public static class LogicEmit {
    public static void Register() {
        // (cast x u32) — value-preserving cast. ct = Rust `as`, rt = bd.cast(x, ty).
        Expression("cast",
            list => {
                var to = list.Type;
                // Rust `as` between int widths/signedness works; bool→int needs `as u32`.
                var rustTy = to switch {
                    EInt(var s, var w) => $"{(s ? "i" : "u")}{(w<=8?8:w<=16?16:w<=32?32:w<=64?64:128)}",
                    EFloat(32) => "f32", EFloat(64) => "f64",
                    EBool => "bool",
                    _ => throw new NotSupportedException($"cast to {to}")
                };
                return $"(({GenerateExpression(list[1])}) as {rustTy})";
            },
            list => $"bd.cast({Lift(list[1])}, {TypeShort(list.Type)})");

        // (bitcast x ty) — reinterpret bits, same width. ct: transmute via the fixed-width
        // unsigned intermediate. For f32↔u32 use to_bits/from_bits; for int↔int it's `as`
        // (Rust `as` between same-width int is bit-preserving); for anything else fall to rt.
        Expression("bitcast",
            list => {
                var (from, to) = (list[1].Type, list.Type);
                var e = GenerateExpression(list[1]);
                return (from, to) switch {
                    (EFloat(32), EInt _) => $"(({e}).to_bits() as {CtType(to)})",
                    (EFloat(64), EInt _) => $"(({e}).to_bits() as {CtType(to)})",
                    (EInt _, EFloat(32)) => $"f32::from_bits(({e}) as u32)",
                    (EInt _, EFloat(64)) => $"f64::from_bits(({e}) as u64)",
                    (EInt _, EInt _) => $"(({e}) as {CtType(to)})",
                    _ => throw new NotImplementedException($"ct bitcast {from}→{to}")
                };
            },
            list => $"bd.bitcast({Lift(list[1])}, {TypeShort(list.Type)})");

        Expression("signext",
            list => {
                // ct: sign-extend from src-width to target-width. Rust: cast to signed src-width,
                // then to target signed, then to target unsigned if needed.
                var srcW = list[1].Type is EInt(_, var w) ? w : throw new();
                var (ts, tw) = list.Type is EInt(var s, var wi) ? (s, wi) : throw new();
                var srcRust = $"i{(srcW<=8?8:srcW<=16?16:srcW<=32?32:64)}";
                var tgtRust = $"{(ts?"i":"u")}{(tw<=8?8:tw<=16?16:tw<=32?32:tw<=64?64:128)}";
                return $"((({GenerateExpression(list[1])}) as {srcRust}) as {tgtRust})";
            },
            list => $"bd.sext({Lift(list[1])}, {TypeShort(list.Type)})");

        // (requires cond) can appear as expression too? — no, statement only.
        // NaN? / round-* / ceil / floor
        Expression("NaN?", list => $"({GenerateExpression(list[1])}).is_nan()",
                            list => $"bd.fisnan({Lift(list[1])})");
        Expression("round", RtRound("Nearest"));
        Expression("round-half-down", RtRound("HalfDown"));
        Expression("round-half-up", RtRound("HalfUp"));
        Expression("round-toward-zero", RtRound("TowardZero"));
        Expression("ceil", list => $"({GenerateExpression(list[1])}).ceil()",
                            list => $"bd.fceil({Lift(list[1])})");
        Expression("floor", list => $"({GenerateExpression(list[1])}).floor()",
                             list => $"bd.ffloor({Lift(list[1])})");
    }

    static Func<PList, string> RtRound(string mode) =>
        list => $"bd.fround({Lift(list[1])}, RoundMode::{mode})";
}
