using System.Diagnostics;
using ArchCompilerCore;
using LibSharpRetro;
using PrettyPrinter;
using static Backends.CSharp.CSharpEmit;
using static ArchCompilerCore.BuiltinTypes;

namespace Backends.CSharp;

// Emit-lambdas extracted from legacy CoreArchCompiler/VectorMath.cs
public static class VectorMathEmit {

	static unsafe float FastInvsqrt(float number) {
		var i = *(uint*) &number;
		i = 0x5f3759df - (i >> 1);
		var f = *(float*) &i;
		f *= 1.5f - 0.5f * f * f;
		return f;
	}

	static unsafe double FastInvsqrt(double number) {
		var x2 = number * 0.5;
		var i = *(long*) &number;
		i = 0x5fe6eb50c7b537a9 - (i >> 1);
		var y = *(double*) &i;
		y *= 1.5 - x2 * y * y;
		return y;
	}

    public static void Register() {
        Expression("vector",
            list => throw new NotImplementedException(),
            list => $"builder.CreateVector({string.Join(", ", list.Skip(1).Select(x => $"builder.EnsureRuntime({GenerateExpression(x)})"))})");
 // TODO: Implement
        Expression("vector-all",
            list => $"reinterpret_cast<Vector128<float>>(({GenerateExpression(list[1])}) - (Vector128<{GenerateType(list[1].Type)}>) {{}})",
            list => $"(({GenerateType(list[1].Type.AsRuntime())}) builder.EnsureRuntime({GenerateExpression(list[1])})).CreateVector()");
        Expression("vector-zero-top",
            list => $"reinterpret_cast<Vector128<float>>(reinterpret_cast<Vector128<uint64_t>>({GenerateExpression(list[1])})[0] - (Vector128<uint64_t>) {{}})",
            list => $"({GenerateExpression(list[1])}).ZeroTop()");
        Expression("vector-element",
            list => $"reinterpret_cast<Vector128<{GenerateType(list.Type.AsCompiletime())}>>({GenerateExpression(list[1])})[{GenerateExpression(list[2])}]",
            list => $"({GenerateExpression(list[1])}).Element<{GenerateType(list.Type.AsCompiletime())}>({GenerateExpression(list[2].Cast<int>())})");
        Expression("vector-extract",
            list => $"Math.VectorExtract({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])}, {GenerateExpression(list[4])})",
            list => $"({GenerateExpression(list[1])}).VectorExtract({GenerateExpression(list[2])}, (uint) {GenerateExpression(list[3])}, (uint) {GenerateExpression(list[4])})");
 // TODO: Implement

        Expression("vector-count-bits",
            list => $"Math.VectorCountBits({GenerateExpression(list[1])}, {GenerateExpression(list[2])})",
            list => $"({GenerateExpression(list[1])}).VectorCountBits((IRuntimeValue<long>) builder.EnsureRuntime({GenerateExpression(list[2])}))");
 // TODO: Implement

        Expression("vector-sum-unsigned",
            list => $"Math.VectorSumUnsigned({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])})",
            list => $"builder.EnsureRuntime({GenerateExpression(list[1])}).VectorSumUnsigned({GenerateExpression(list[2])}, {GenerateExpression(list[3])})");
        Expression("vec-frsqrte",
            list => $"Math.VectorFrsqrte({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])})",
            list => $"({GenerateExpression(list[1])}).VectorFrsqrte({GenerateExpression(list[2])}, {GenerateExpression(list[3])})");
        Expression("vec+",
            list => list[3] switch {
				PInt(32) => $"({GenerateExpression(list[1])}) + ({GenerateExpression(list[2])})", 
				PInt(64) => $"reinterpret_cast<Vector128<float>>(reinterpret_cast<Vector128<double>>({GenerateExpression(list[1])}) + reinterpret_cast<Vector128<double>>({GenerateExpression(list[2])}))",
				_ => throw new NotSupportedException()
			},
            list => list[3] switch {
				PInt(32) => $"({GenerateExpression(list[1])}) + ({GenerateExpression(list[2])})", 
				PInt(64) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<double>>) ({GenerateExpression(list[1])}) + (IRuntimeValue<Vector128<double>>) ({GenerateExpression(list[2])}))",
				_ => throw new NotSupportedException()
			});
        Expression("vec-",
            list => list[3] switch {
				PInt(32) => $"({GenerateExpression(list[1])}) - ({GenerateExpression(list[2])})", 
				PInt(64) => $"reinterpret_cast<Vector128<float>>(reinterpret_cast<Vector128<double>>({GenerateExpression(list[1])}) - reinterpret_cast<Vector128<double>>({GenerateExpression(list[2])}))",
				_ => throw new NotSupportedException()
			},
            list => list[3] switch {
				PInt(32) => $"({GenerateExpression(list[1])}) - ({GenerateExpression(list[2])})", 
				PInt(64) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<double>>) ({GenerateExpression(list[1])}) - (IRuntimeValue<Vector128<double>>) ({GenerateExpression(list[2])}))",
				_ => throw new NotSupportedException()
			});
        Expression("vec*",
            list => list[3] switch {
				PInt(32) => $"({GenerateExpression(list[1])}) * ({GenerateExpression(list[2])})", 
				PInt(64) => $"reinterpret_cast<Vector128<float>>(reinterpret_cast<Vector128<double>>({GenerateExpression(list[1])}) * reinterpret_cast<Vector128<double>>({GenerateExpression(list[2])}))",
				_ => throw new NotSupportedException()
			},
            list => list[3] switch {
				PInt(32) => $"({GenerateExpression(list[1])}) * ({GenerateExpression(list[2])})", 
				PInt(64) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<double>>) ({GenerateExpression(list[1])}) * (IRuntimeValue<Vector128<double>>) ({GenerateExpression(list[2])}))",
				_ => throw new NotSupportedException()
			});


		string CastVector(PTree elem, string type) =>
			elem.Type is EVector
				? $"(IRuntimeValue<Vector128<{type}>>) ({GenerateExpression(elem)})"
				: $"({GenerateExpression(elem)})";
		string RuntimeCastVector(PTree elem, string type) =>
			elem.Type is EVector
				? $"((IRuntimeValue<Vector128<{type}>>) ({GenerateExpression(elem)}))"
				: $"({GenerateExpression(elem)})";
			
        Expression("vec-uint+",
            list => list[3] switch {
				PInt(8) => $"(({GenerateExpression(list[1])}).As<float, byte>() + ({CastVector(list[2], "byte")})).As<byte, float>()",
				PInt(16) => $"(({GenerateExpression(list[1])}).As<float, ushort>() + ({CastVector(list[2], "ushort")})).As<ushort, float>()",
				PInt(32) => $"(({GenerateExpression(list[1])}).As<float, uint>() + ({CastVector(list[2], "uint")})).As<uint, float>()",
				PInt(64) => $"(({GenerateExpression(list[1])}).As<float, ulong>() + ({CastVector(list[2], "ulong")})).As<ulong, float>()",
				_ => throw new NotSupportedException()
			},
            list => list[3] switch {
				PInt(8) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<byte>>) ({GenerateExpression(list[1])}) + {RuntimeCastVector(list[2], "byte")})",
				PInt(16) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<ushort>>) ({GenerateExpression(list[1])}) + {RuntimeCastVector(list[2], "ushort")})",
				PInt(32) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<uint>>) ({GenerateExpression(list[1])}) + {RuntimeCastVector(list[2], "uint")})",
				PInt(64) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<ulong>>) ({GenerateExpression(list[1])}) + {RuntimeCastVector(list[2], "ulong")})",
				_ => throw new NotSupportedException()
			});
        Expression("vec-uint*",
            list => list[3] switch {
				PInt(8) => $"(({GenerateExpression(list[1])}).As<float, byte>() * ({CastVector(list[2], "byte")})).As<byte, float>()",
				PInt(16) => $"(({GenerateExpression(list[1])}).As<float, ushort>() * ({CastVector(list[2], "ushort")})).As<ushort, float>()",
				PInt(32) => $"(({GenerateExpression(list[1])}).As<float, uint>() * ({CastVector(list[2], "uint")})).As<uint, float>()",
				PInt(64) => $"(({GenerateExpression(list[1])}).As<float, ulong>() * ({CastVector(list[2], "ulong")})).As<ulong, float>()",
				_ => throw new NotSupportedException()
			},
            list => list[3] switch {
				PInt(8) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<byte>>) ({GenerateExpression(list[1])}) * {RuntimeCastVector(list[2], "byte")})",
				PInt(16) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<ushort>>) ({GenerateExpression(list[1])}) * {RuntimeCastVector(list[2], "ushort")})",
				PInt(32) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<uint>>) ({GenerateExpression(list[1])}) * {RuntimeCastVector(list[2], "uint")})",
				PInt(64) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<ulong>>) ({GenerateExpression(list[1])}) * {RuntimeCastVector(list[2], "ulong")})",
				_ => throw new NotSupportedException()
			});
        Expression("vec/",
            list => list[3] switch {
				PInt(32) => $"({GenerateExpression(list[1])}) / ({GenerateExpression(list[2])})", 
				PInt(64) => $"(({GenerateExpression(list[1])}).As<float, double>() / ({GenerateExpression(list[2])}).As<float, double>()).As<double, float>()",
				_ => throw new NotSupportedException()
			},
            list => list[3] switch {
				PInt(32) => $"({GenerateExpression(list[1])}) / ({GenerateExpression(list[2])})", 
				PInt(64) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<double>>) ({GenerateExpression(list[1])}) / (IRuntimeValue<Vector128<double>>) ({GenerateExpression(list[2])}))",
				_ => throw new NotSupportedException()
			});
        Expression("vec&",
            list => $"Vector128.BitwiseAnd({GenerateExpression(list[1])}, {GenerateExpression(list[2])})",
            list => $"({GenerateExpression(list[1])}) & ({GenerateExpression(list[2])})");
        Expression("vec&~",
            list => $"Vector128.AndNot({GenerateExpression(list[1])}, {GenerateExpression(list[2])})",
            list => $"({GenerateExpression(list[1])}) & ~({GenerateExpression(list[2])})");
        Expression("vec|",
            list => $"Vector128.BitwiseOr({GenerateExpression(list[1])}, {GenerateExpression(list[2])})",
            list => $"({GenerateExpression(list[1])}) | ({GenerateExpression(list[2])})");
        Expression("vec^",
            list => $"Vector128.Xor({GenerateExpression(list[1])}, {GenerateExpression(list[2])})",
            list => $"({GenerateExpression(list[1])}) ^ ({GenerateExpression(list[2])})");
        Expression("vec~",
            list => $"Vector128.Not({GenerateExpression(list[1])})",
            list => $"~({GenerateExpression(list[1])})");
 // TODO
	    }
}
