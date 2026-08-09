using System.Diagnostics;
using ArchCompilerCore;
using LibSharpRetro;
using PrettyPrinter;

namespace Backends.CSharp;

// The emit-half of the legacy Core.cs, relocated verbatim behind the backend seam.
// Emit-lambdas stay Func<PList, string> / Action<CodeBuilder, PList> exactly as they were —
// they call the STATIC GenerateExpression/GenerateType here (mirroring legacy Core's statics)
// so they don't need per-lambda rewriting. The rung-2 goal is byte-identical Aarch64Cpu/
// output; the seam-refinement (making lambdas take an EmitContext) happens at rung-4 when
// the Rust backend forces it. Freeze-law: don't redesign the emit path while chasing its bytes.
//
// State that was Core.{Context, NextLabel, HasBranch} lives here as statics for the same reason.

public enum ContextTypes { Disassembler, Interpreter, Recompiler }

public static class CSharpEmit {
    public static ContextTypes Context;
    public static string NextLabel;
    public static bool HasBranch;

    // Per-head emit registry. Keyed on the same head-names as Heads.All.
    // (Signature is in Heads; only emit lives here.)
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
    // Forwards to ArchCompilerCore.Temp.Name() — legacy Core.cs had ONE counter shared by
    // frontend tree-rewriting (MipsDef branch-slot reg-defer) AND backend emit (mlet lowering).
    // Both must draw from the same counter in the same order for byte-identical output.
    public static string TempName() => Temp.Name();

    // Standalone Interpret("name", exec) calls in legacy Define() bodies are the exec-half —
    // they now live in ArchCompilerCore/Modules/*.cs via Heads.Register(name, sig, exec).
    // This no-op keeps extracted emit-modules compilable where the gap-text carried them through.
    public static void Interpret(string name, Func<PList, ExecutionState, dynamic> _) { }

    public static void BranchExpression(string name, Func<PList, string> ct, Func<PList, string> rt = null) {
        Statement(name, (c, list) => { HasBranch = true; c += $"{ct(list)};"; },
                        (c, list) => { HasBranch = true; c += $"{(rt ?? ct)(list)};"; });
    }

    // === legacy Core.cs :230-252, verbatim ===
    public static string ToHex(long value) => value < 0 ? $"-0x{-value:X}" : $"0x{value:X}";

    public static string GenerateExpression(PTree v, bool lhs = false) => v switch {
        PName name => name.Name,
        PInt value => value.Type switch {
            EInt(false, <= 8) => $"(byte) {ToHex(value.Value)}",
            EInt(true, <= 8) => $"(sbyte) {ToHex(value.Value)}",
            EInt(false, <= 16) => $"(ushort) {ToHex(value.Value)}",
            EInt(true, <= 16) => $"(short) {ToHex(value.Value)}",
            EInt(false, <= 32) => $"{ToHex(value.Value)}U",
            EInt(true, <= 32) => $"{ToHex(value.Value)}",
            EInt(false, <= 64) => ToHex(value.Value) + "UL",
            EInt(true, <= 64) => ToHex(value.Value) + "L",
            _ => throw new NotImplementedException()
        },
        PString str => str.String.ToPrettyString(),
        PList list => GenerateListExpression(list, lhs: lhs),
        _ => throw new NotImplementedException()
    };

    // === legacy Core.cs :282-306 ===
    public static string GenerateListExpression(PList list, bool lhs = false) {
        if(Context == ContextTypes.Recompiler && list.Type.Runtime) {
            var expr = GenerateBaseListRuntimeExpression(list);
            return lhs || list.Type is EUnit ? expr : $"({GenerateType(list.Type)}) ({expr})";
        } else {
            var expr = GenerateBaseListExpression(list);
            return lhs || list.Type is EUnit ? expr : $"({GenerateType(list.Type)}) ({expr})";
        }
    }
    static string GenerateBaseListExpression(PList list) => list[0] switch {
        PName(var name) when Expressions.ContainsKey(name) => Expressions[name].CompileTime(list),
        PName name => throw new NotImplementedException($"Unknown name for GenerateListExpression: {name}"),
        _ => throw new NotSupportedException($"Non-name for first element of list {list.ToPrettyString()}")
    };
    static string GenerateBaseListRuntimeExpression(PList list) {
        Debug.Assert(Context == ContextTypes.Recompiler);
        return list[0] switch {
            PName(var name) when Expressions.ContainsKey(name) => Expressions[name].RunTime(list),
            PName name => throw new NotImplementedException($"Unknown name for GenerateRuntimeListExpression: {name}"),
            _ => throw new NotSupportedException($"Non-name for first element of list {list.ToPrettyString()}")
        };
    }

    // === legacy Core.cs :197-227 ===
    public static void GenerateStatement(CodeBuilder c, PList list) {
        if(Context == ContextTypes.Recompiler && list.Type.Runtime) { GenerateRuntimeStatement(c, list); return; }
        switch(list[0]) {
            case PName(var name) when Statements.ContainsKey(name): Statements[name].CompileTime(c, list); break;
            case PName: c += $"{GenerateExpression(list)};"; break;
            default: throw new NotSupportedException($"Non-name for first element of list {list.ToPrettyString()}");
        }
    }
    static void GenerateRuntimeStatement(CodeBuilder c, PList list) {
        Debug.Assert(Context == ContextTypes.Recompiler);
        switch(list[0]) {
            case PName(var name) when Statements.ContainsKey(name): Statements[name].RunTime(c, list); break;
            case PName: c += $"{GenerateExpression(list)};"; break;
            default: throw new NotSupportedException($"Non-name for first element of list {list.ToPrettyString()}");
        }
    }

    // === legacy Core.cs :254-279, verbatim ===
    public static string GenerateType(EType type) {
        string __GenerateType() {
            switch(type) {
                case null: return "void";
                case EUnit: return "void";
                case EBool: return "bool";
                case EString: return "string";
                case EInt i:
                    switch(i.Width) {
                        case > 64: return i.Signed ? "Int128" : "UInt128";
                        case > 32: return i.Signed ? "long" : "ulong";
                        case > 16: return i.Signed ? "int" : "uint";
                        case > 8: return i.Signed ? "short" : "ushort";
                        default: return i.Signed ? "sbyte" : "byte";
                    }
                case EFloat f:
                    switch(f.Width) {
                        case > 64: return "Vector128<float>";
                        case > 32: return "double";
                        default: return "float";
                    }
                case EVector: return "Vector128<float>";
                default: throw new NotImplementedException($"Type {type}");
            }
        }
        return Context == ContextTypes.Recompiler && type.Runtime
            ? $"IRuntimeValue<{__GenerateType()}>"
            : __GenerateType();
    }
}
