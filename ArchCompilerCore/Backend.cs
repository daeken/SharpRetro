using LibSharpRetro; // CodeBuilder
namespace ArchCompilerCore;

// The backend contract per the redesign spec's four-project split. ArchCompilerCore's tree-walker
// (Emitter.Emit, below) dispatches into these; a backend supplies per-head emit lambdas
// keyed by the same head-names Heads uses. One IBackend impl per output-language.
//
// The legacy compiler fused walker+emit (Core.GenerateExpression called Expressions[name].CompileTime
// which held C# strings). Here: the walker is language-neutral (in Core), the backend
// supplies only the per-node rendering. Two-pass shape: PTree-as-IR (the post-macro-
// expansion, type-annotated tree = the IR); backends visit it. "Promote the implicit IR
// to contractual" — not a new IR inserted, but the existing lowered form named + frozen.
//
// [disasm/interp/recompiler] = the target dimension. Corpse's compiletime/runtime lambda
// split (census: distinct-count = 0) collapses; the target-dimension lives in
// EmitContext.Target (backends may vary output per-target, but most heads emit identically
// across all three — the walker doesn't care).

public enum Target { Disassembler, Interpreter, Recompiler }

public record EmitContext(IBackend Backend, Target Target) {
    // Recursive emit — backends' per-head lambdas call this on children.
    public string Expr(PTree t) => Emitter.EmitExpr(t, this);
    public void Stmt(CodeBuilder c, PList list) => Emitter.EmitStmt(c, list, this);
}

public interface IBackend {
    string Name { get; }
    // The three fixed-shape arms of the legacy compiler's GenerateExpression/GenerateType:
    string TypeName(EType t);           // legacy Core.GenerateType — EInt(u,32) → "uint" etc
    string IntLiteral(PInt i);          // legacy GenerateExpression PInt-arm — 0xFF → "(byte) 0xFF"
    string StringLiteral(PString s);    // legacy: s.String.ToPrettyString()

    // Per-head emit. Backends hold their own head→emit dict; the walker calls this for
    // any PList whose head isn't a leaf. If the backend has no emit for the head, throw
    // (throw-on-unhandled: a missing emit is a loud gap, not a silent skip).
    string EmitExpr(string head, PList list, EmitContext ctx);
    void EmitStmt(string head, PList list, CodeBuilder c, EmitContext ctx);

    // The scaffold emit (legacy compiler's per-arch Program.cs work): mask/match dispatch,
    // per-target file structure. Rung-2 phase-2; the interface stub lands now.
    void EmitScaffold(IReadOnlyList<Def> defs, IArchFrontend arch, Target target, string outPath);
}

// Frontend metadata a backend needs to emit the scaffold (mask/match/fields per def,
// register-file declarations, the arch's contract-intrinsic set). Frontends implement this.
public interface IArchFrontend {
    string Name { get; }
    // ... rung-2 phase-2 fills this in (the aarch64 mask/match/fields shape as data).
}

// The language-neutral tree-walker. Corresponds to legacy compiler's Core.GenerateExpression/
// GenerateStatement, but dispatches into backend instead of into a C#-string dict.
public static class Emitter {
    public static string EmitExpr(PTree t, EmitContext ctx) => t switch {
        PName n => n.Name,               // locals/field-refs render as-is (backend may
                                         //   post-process; legacy compiler's GenerateExpression did this)
        PInt i => ctx.Backend.IntLiteral(i),
        PString s => ctx.Backend.StringLiteral(s),
        PList l when l[0] is PName(var head) => ctx.Backend.EmitExpr(head, l, ctx),
        _ => throw new NotSupportedException($"Cannot emit expression from {t}")
    };

    public static void EmitStmt(CodeBuilder c, PList list, EmitContext ctx) {
        if(list[0] is not PName(var head))
            throw new NotSupportedException($"Statement head is not a name: {list[0]}");
        ctx.Backend.EmitStmt(head, list, c, ctx);
    }
}
