using System.Diagnostics;
using ArchCompilerCore;
using LibSharpRetro;
using PrettyPrinter;
using static Backends.CSharp.CSharpEmit;
using static ArchCompilerCore.BuiltinTypes;

namespace Backends.CSharp;

// Emit-lambdas extracted from legacy CoreArchCompiler/TopLevelProcessing.cs
public static class TopLevelProcessingEmit {
    public static void Register() {
        Expression("defm",
            _ => throw new NotSupportedException());
        Expression("def",
            _ => throw new NotSupportedException());
        Expression("print",
            list => GenerateExpression(list[1]));
        Expression("print-hex",
            list => GenerateExpression(list[1]));
    }
}
