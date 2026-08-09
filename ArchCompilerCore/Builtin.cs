namespace ArchCompilerCore;

// Module base class — each core module (ScalarMath, Logic, ControlFlow, VectorMath,
// StringManipulation, TopLevelProcessing) subclasses this and registers its heads
// via the Expr/Stmt helpers. Corpse's Builtin.Expression()/Statement() split into:
//   HERE: Expr/Stmt(name, signature, execute?) → Heads.Register (language-neutral)
//   Backends/*: Emit(name, compiletime, runtime?) → per-backend registry (C#-strings live there)
//
// The .Interpret() fluent-chain from the legacy compiler becomes the exec: parameter directly.

public abstract class Builtin {
    // NOTE: signature-helpers like LogicalType/LogicalBool/FirstType are module-LOCAL in
    // the legacy compiler (ScalarMath.cs carries its own, subtly different from
    // BuiltinTypes.cs's — e.g. || vs && on signedness). Modules carry their helpers
    // VERBATIM from the legacy module they came from so the byte-identical oracle holds;
    // Builtin.cs holds ONLY registration plumbing + genuinely-shared helpers.

    protected static EType TypeFromName(PTree expr) => BuiltinTypes.TypeFromName(expr);

    // Registration helpers — return `this` so a module could still fluent-chain if wanted,
    // but the .Interpret() sidecar becomes an exec: parameter directly.
    protected void Expr(string name, Func<PList, EType> sig,
            Func<PList, ExecutionState, dynamic> exec = null) =>
        Heads.Register(name, sig, exec, stmt: false);

    protected void Expr(string[] names, Func<PList, EType> sig,
            Func<PList, ExecutionState, dynamic> exec = null) =>
        Heads.Register(names, sig, exec, stmt: false);

    protected void Stmt(string name, Func<PList, EType> sig,
            Func<PList, ExecutionState, dynamic> exec = null) =>
        Heads.Register(name, sig, exec, stmt: true);

    public abstract void Define();

    public static void DefineAll() {
        // Reflection over Builtin subclasses in this assembly (matches legacy compiler's Core.Register)
        foreach(var t in typeof(Builtin).Assembly.GetTypes())
            if(t.IsSubclassOf(typeof(Builtin)) && !t.IsAbstract)
                ((Builtin) Activator.CreateInstance(t)).Define();
    }
}
