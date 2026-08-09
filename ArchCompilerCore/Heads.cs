namespace ArchCompilerCore;

// The core head-registry: the language-NEUTRAL half of what the legacy compiler stored as
// Core.Expressions/Statements = (Signature, CompileTime, RunTime, Execute).
// Here: {Signature, Execute} only. Backends supply emit separately, keyed on the same head-name.
//
// Design per the redesign spec + the head-classification census:
//   - Signature: PList → EType. Type-inference dispatch. Language-neutral (computes EType from
//     child types via promotion rules; no C# leaks — verified at ScalarMath.LogicalType etc).
//   - Execute: (PList, ExecutionState) → dynamic. The compiletime-eval leg (what .Interpret()
//     registered in the legacy compiler). Runs in ArchCompilerCore during const-fold; backends never see it.
//   - IsStatement: void-vs-value. Corpse split this into two dicts; here it's metadata
//     (equivalently: Signature returns EUnit ⟹ statement).
//
// The ~65 core heads + ~6 primitives get registered here by ArchCompilerCore's own modules
// (ScalarMath/Logic/VectorMath/ControlFlow/... — rung-1b work). Frontends may add heads
// (contract-intrinsics) but those declare Signature only; their Execute throws
// (they're runtime-only by definition).

public record Head(
    Func<PList, EType> Signature,
    Func<PList, ExecutionState, dynamic> Execute,
    bool IsStatement = false
);

public static class Heads {
    public static readonly Dictionary<string, Head> All = new();

    public static void Register(string name, Func<PList, EType> sig,
            Func<PList, ExecutionState, dynamic> exec = null, bool stmt = false) {
        // Refuse silent overwrite (throw-on-unhandled discipline). Rung-1b surfaced 3 dual-registered heads
        // (if/match/block — legacy holds separate Stmt+Expr sigs, dispatches Stmt-first at
        // InferType). Here: first registration wins (Stmt lands first per legacy order),
        // second is refused-loud rather than overwriting silently.
        if(All.ContainsKey(name))
            throw new NotSupportedException($"Head '{name}' already registered (as {(All[name].IsStatement ? "stmt" : "expr")}). "
                + "Corpse's dual stmt+expr registration collapses to Stmt-sig at InferType (Def.cs:64-69 checks Statements first). "
                + "The Expr-sig was only used at emit-time (GenerateExpression) — that lives in the Backend now.");
        All[name] = new(sig, exec ?? ((_, _) => throw new NotImplementedException($"No compiletime-eval for '{name}'")), stmt);
    }

    public static void Register(string[] names, Func<PList, EType> sig,
            Func<PList, ExecutionState, dynamic> exec = null, bool stmt = false) {
        foreach(var n in names) Register(n, sig, exec, stmt);
    }

    // throw-on-unhandled = the enforcement that the head-vocabulary is CLOSED.
    public static Head Get(string name) =>
        All.TryGetValue(name, out var h) ? h
            : throw new NotSupportedException($"Unknown head '{name}' — not in the core vocabulary. "
                + "If this is a per-arch reg/mem/state form, it should be a MACRO over core primitives; "
                + "if it's an irreducible arch-algorithm, register it as a contract-intrinsic.");
}
