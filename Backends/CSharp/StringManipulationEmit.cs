using System.Diagnostics;
using ArchCompilerCore;
using LibSharpRetro;
using PrettyPrinter;
using static Backends.CSharp.CSharpEmit;
using static ArchCompilerCore.BuiltinTypes;

namespace Backends.CSharp;

// Emit-lambdas extracted from legacy CoreArchCompiler/StringManipulation.cs
public static class StringManipulationEmit {
    public static void Register() {
        Expression("string-concat",
            list => string.Join(" + ", list.Skip(1).Select(x => x.Type is EString ? GenerateExpression(x) : $"({GenerateExpression(x)}).ToString()")),
            _ => "/*UNIMPLEMENTED*/");
        Expression("string-length",
            _ => "throw new NotImplementedException()");
        Expression("hex",
            list => list[1].Type is EInt(_, var bits) ? $"$\"0x{{({GenerateExpression(list[1])}):x0{bits / 4}}}\"" : throw new NotSupportedException());
 // TODO: Implement
        Expression("as-string",
            _ => "throw new NotImplementedException()");
    }
}
