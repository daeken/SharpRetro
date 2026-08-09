namespace ArchCompilerCore;

// Corpse: CoreArchCompiler/TopLevelProcessing.cs
public class TopLevelProcessing : Builtin {
    public override void Define() {
        // defm/def are consumed by MacroProcessor/Def.ParseAll — never reach a backend.
        // Registered so InferType doesn't throw on them at top-level.
        Expr("defm", _ => EType.Unit);
        Expr("def", _ => EType.Unit);

        Expr("print", list => list[1].Type,
            (list, state) => { var v = state.Evaluate(list[1]); Console.WriteLine(v); return v; });
        Expr("print-hex", list => list[1].Type,
            (list, state) => { var v = state.Evaluate(list[1]); Console.WriteLine($"0x{v:X}"); return v; });
    }
}
