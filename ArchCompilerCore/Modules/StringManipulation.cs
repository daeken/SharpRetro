namespace ArchCompilerCore;

// Corpse: CoreArchCompiler/StringManipulation.cs — 4 heads.
// Emit-lambdas moved to Backends/CSharp/StringManipulation.cs (rung-2).
public class StringManipulation : Builtin {
    public override void Define() {
        Expr("string-concat", list => EType.String.AsRuntime(list.AnyRuntime),
            (list, state) => list.Skip(1).Aggregate("",
                (cur, e) => (string) (cur + state.Evaluate(e).ToString())));

        Expr("string-length", _ => EType.Unit,
            (list, state) => state.Evaluate(list[1]).Length);

        Expr("hex", list => EType.String.AsRuntime(list.AnyRuntime));
            // legacy: .NoInterpret() — no exec.

        Expr("as-string", _ => EType.String,
            (list, state) => list[1] switch {
                PName(var name) => name,
                {} x => state.Evaluate(x)
            });
    }
}
