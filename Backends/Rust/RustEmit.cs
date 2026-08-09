using System.Diagnostics;
using ArchCompilerCore;
using LibSharpRetro;

namespace Backends.Rust;

// The Rust-emit backend — the rung-4 payoff. Emits `recompiler.rs` calling into the
// `Builder` trait (rust/sharpretro-jit/src/lib.rs). Same structure as Backends/CSharp/
// CSharpEmit.cs (per-head Expressions/Statements dicts + GenerateExpression/Type/Statement
// dispatchers), but the string-templates emit Rust syntax targeting the trait.
//
// Key differences from the C# backend (from the census of oracle-baseline/aarch64/Recompiler.cs):
//   - No `(IRuntimeValue<T>)` cast-noise: `Builder::Val` is opaque + carries IlType.
//   - No `builder.EnsureRuntime(x)`: compiletime values → `b.literal(ty, bits)`.
//   - Operator overloads (`+`, `==`, `&`) → explicit `b.add(l, r)` etc (Rust doesn't
//     overload on trait-associated types cleanly for this shape).
//   - `state.X[i]` reg-access → `b.reg_read(RegFile::GPR, i, W64)` (the primitive form).
//   - `Funcify(() => {...})()` block-emit → Rust `{ let x = ...; result }` (native).
//
// Rung-4's oracle is NOT a byte-diff (there's no legacy recompiler.rs). Two gates instead:
//   (a) `cargo build` on the generated file against the Builder trait (typechecker = gate).
//   (b) IL-call-sequence diff: instrument BOTH backends with a recording-Builder that logs
//       {method, args-types}; per-insn, the sequences must match modulo language-shape.

public static class RustEmit {
    // Same three targets as C# backend; recompiler.rs = the primary rung-4 output.
    public enum ContextTypes { Disassembler, Interpreter, Recompiler }
    public static ContextTypes Context;
    public static string NextLabel;

    public static readonly Dictionary<string, (Func<PList, string> CompileTime, Func<PList, string> RunTime)>
        Expressions = new();
    public static readonly Dictionary<string, (Action<CodeBuilder, PList> CompileTime, Action<CodeBuilder, PList> RunTime)>
        Statements = new();

    public static void Expression(string name, Func<PList, string> ct, Func<PList, string> rt = null) =>
        Expressions[name] = (ct, rt ?? ct);
    public static void Expression(string[] names, Func<PList, string> ct, Func<PList, string> rt = null) {
        foreach(var n in names) Expression(n, ct, rt);
    }
    public static void Statement(string name, Action<CodeBuilder, PList> ct, Action<CodeBuilder, PList> rt = null) =>
        Statements[name] = (ct, rt ?? ct);

    // ── the fixed leaves (Rust syntax) ─────────────────────────────────────

    /// EType → Rust IlType constructor expr (matches rust/sharpretro-jit's IlType).
    public static string GenerateType(EType type) => type switch {
        null or EUnit => "IlType::Unit",
        EBool => "IlType::Bool",
        EInt(var s, var w) => $"IlType::I {{ signed: {(s ? "true" : "false")}, width: {w} }}",
        EFloat(var w) => $"IlType::F {{ width: {w} }}",
        EVector => "IlType::V128",
        EString => throw new NotSupportedException("string types don't reach the recompiler"),
        _ => throw new NotImplementedException($"GenerateType {type}")
    };

    /// EType → the Rust IlType const shorthand where one exists (U32/I64/etc), else the full form.
    public static string TypeShort(EType type) => type switch {
        EInt(false, 8) => "IlType::U8", EInt(true, 8) => "IlType::I8",
        EInt(false, 16) => "IlType::U16", EInt(true, 16) => "IlType::I16",
        EInt(false, 32) => "IlType::U32", EInt(true, 32) => "IlType::I32",
        EInt(false, 64) => "IlType::U64", EInt(true, 64) => "IlType::I64",
        EFloat(32) => "IlType::F32", EFloat(64) => "IlType::F64",
        EBool => "IlType::Bool",
        EVector => "IlType::V128",
        _ => GenerateType(type)
    };

    // Rust keywords the .isa uses as identifiers (fields/locals) → prefix `r#` (raw-ident)
    // or rename. `type` is the common one (aarch64 uses `let type = ...` heavily).
    static readonly HashSet<string> RustKeywords = [
        "type", "as", "fn", "let", "mut", "ref", "match", "if", "else", "for", "while",
        "loop", "return", "true", "false", "in", "move", "box", "self", "Self", "use",
        "mod", "pub", "impl", "trait", "struct", "enum", "const", "static", "where",
    ];
    public static string SafeIdent(string name) =>
        RustKeywords.Contains(name) ? $"r#{name}" : name.Replace("-", "_");

    public static string GenerateExpression(PTree v, bool lhs = false) => v switch {
        PName name => SafeIdent(name.Name),   // locals/fields — the scaffold declares them as `let name = ...`
        PInt i => IntLiteral(i),
        // string literals appear in decode-blocks for disasm-name pieces (e.g. `let r "W"`).
        // The recompiler doesn't consume them, but the binding is still in the tree — emit
        // as a Rust &'static str so `let r = "W";` typechecks and gets DCE'd.
        PString s => $"\"{s.String}\"",
        PList list => GenerateListExpression(list, lhs),
        _ => throw new NotImplementedException()
    };

    static string IntLiteral(PInt i) {
        // Compile-time integer → an UNSUFFIXED Rust literal. All insn-fields extract as u32
        // (`(insn >> N) & M`), and Rust doesn't auto-widen (u8 vs u32 = type error), so let
        // the ct-arithmetic infer from the field's type. Runtime lifts (`Lift()`) use
        // TypeShort() explicitly, so the type is carried there.
        return i.Value < 0 ? $"(-0x{-i.Value:X}i64 as _)" : $"0x{i.Value:X}";
    }

    public static string GenerateListExpression(PList list, bool lhs = false) {
        if(list[0] is not PName(var name))
            throw new NotSupportedException($"Non-name head: {list[0]}");
        if(!Expressions.TryGetValue(name, out var e))
            throw new NotImplementedException(
                $"No Rust-emit for head '{name}' — add it to Backends/Rust/*Emit.cs. "
                + "(throw-on-unhandled: a missing emit is a loud gap, not a silent skip.)");
        // Runtime-vs-compiletime: at Recompiler context, .Type.Runtime nodes emit the RunTime
        // lambda (which produces `b.method(...)` calls); compiletime nodes emit bare Rust exprs
        // that fold at generated-code compile-time.
        return Context == ContextTypes.Recompiler && list.Type.Runtime
            ? e.RunTime(list) : e.CompileTime(list);
    }

    public static void GenerateStatement(CodeBuilder c, PList list) {
        if(list[0] is not PName(var name))
            throw new NotSupportedException($"Non-name head: {list[0]}");
        if(Statements.TryGetValue(name, out var s)) {
            (Context == ContextTypes.Recompiler && list.Type.Runtime ? s.RunTime : s.CompileTime)(c, list);
        } else {
            // Expression-in-statement-position → emit + discard (`let _ = ...;`)
            c += $"let _ = {GenerateExpression(list)};";
        }
    }

    // Compile-time value → runtime Val (the Rust equivalent of `builder.EnsureRuntime(x)`).
    // Emit-lambdas call this on any child that's compiletime but the parent needs a runtime Val.
    public static string Lift(PTree child) =>
        child.Type.Runtime
            ? GenerateExpression(child)
            : $"bd.literal({TypeShort(child.Type)}, ({GenerateExpression(child)}) as u128)";

    public static string TempName() => Temp.Name();
}
