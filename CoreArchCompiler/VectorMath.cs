using System;
using System.Linq;

namespace CoreArchCompiler; 

class VectorMath : Builtin {
	unsafe float FastInvsqrt(float number) {
		var i = *(uint*) &number;
		i = 0x5f3759df - (i >> 1);
		var f = *(float*) &i;
		f *= 1.5f - 0.5f * f * f;
		return f;
	}

	unsafe double FastInvsqrt(double number) {
		var x2 = number * 0.5;
		var i = *(long*) &number;
		i = 0x5fe6eb50c7b537a9 - (i >> 1);
		var y = *(double*) &i;
		y *= 1.5 - x2 * y * y;
		return y;
	}
			
	public override void Define() {
		Expression("vector", _ => EType.Vector.AsRuntime(),
				list => throw new NotImplementedException(),
				list => $"builder.CreateVector({string.Join(", ", list.Skip(1).Select(x => $"builder.EnsureRuntime({GenerateExpression(x)})"))})")
			.NoInterpret(); // TODO: Implement
		Expression("vector-all", list => EType.Vector.AsRuntime(),
				list => $"reinterpret_cast<Vector128<float>>(({GenerateExpression(list[1])}) - (Vector128<{GenerateType(list[1].Type)}>) {{}})",
				list => $"(({GenerateType(list[1].Type.AsRuntime())}) builder.EnsureRuntime({GenerateExpression(list[1])})).CreateVector()")
			.Interpret((list, state) => Vector128<byte>.FromDynamic(state.Evaluate(list[1])));
		Expression("vector-zero-top", list => EType.Vector.AsRuntime(), 
				list => $"reinterpret_cast<Vector128<float>>(reinterpret_cast<Vector128<uint64_t>>({GenerateExpression(list[1])})[0] - (Vector128<uint64_t>) {{}})", 
				list => $"({GenerateExpression(list[1])}).ZeroTop()")
			.Interpret((list, state) => state.Evaluate(list[1]).ZeroTop());
		Expression("vector-element", list => TypeFromName(list[3]).AsRuntime(),
				list => $"reinterpret_cast<Vector128<{GenerateType(list.Type.AsCompiletime())}>>({GenerateExpression(list[1])})[{GenerateExpression(list[2])}]",
				list => $"({GenerateExpression(list[1])}).Element<{GenerateType(list.Type.AsCompiletime())}>({GenerateExpression(list[2].Cast<int>())})")
			.Interpret((list, state) => state.Evaluate(list[1]).As(TypeFromName(list[3]))[(int) state.Evaluate(list[2])]);
		Expression("vector-extract", list => EType.Vector.AsRuntime(list[1].Type.Runtime || list[2].Type.Runtime), 
				list => $"Math.VectorExtract({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])}, {GenerateExpression(list[4])})", 
				list => $"({GenerateExpression(list[1])}).VectorExtract({GenerateExpression(list[2])}, (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(list[3])}), (IRuntimeValue<uint>) builder.EnsureRuntime({GenerateExpression(list[4])}))")
			.NoInterpret(); // TODO: Implement

		Expression("vector-count-bits", _ => EType.Vector, 
				list => $"Math.VectorCountBits({GenerateExpression(list[1])}, {GenerateExpression(list[2])})", 
				list => $"({GenerateExpression(list[1])}).VectorCountBits((IRuntimeValue<long>) builder.EnsureRuntime({GenerateExpression(list[2])}))")
			.NoInterpret(); // TODO: Implement

		Expression("vector-sum-unsigned", _ => new EInt(false, 64),
				list => $"Math.VectorSumUnsigned({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])})", 
				list => $"builder.EnsureRuntime({GenerateExpression(list[1])}).VectorSumUnsigned((IRuntimeValue<byte>) builder.EnsureRuntime({GenerateExpression(list[2])}), (IRuntimeValue<byte>) builder.EnsureRuntime({GenerateExpression(list[3])}))")
			.Interpret((list, state) => {
				var esize = (int) state.Evaluate(list[2]);
				var count = (int) state.Evaluate(list[3]);
				switch(esize) {
					case 8: {
						var vector = state.Evaluate(list[1]).As<byte>();
						var sum = 0UL;
						for(var i = 0; i < count; ++i)
							sum += (ulong) vector[i];
						return sum;
					}
					case 16: {
						var vector = state.Evaluate(list[1]).As<ushort>();
						var sum = 0UL;
						for(var i = 0; i < count; ++i)
							sum += (ulong) vector[i];
						return sum;
					}
					case 32: {
						var vector = state.Evaluate(list[1]).As<uint>();
						var sum = 0UL;
						for(var i = 0; i < count; ++i)
							sum += (ulong) vector[i];
						return sum;
					}
					case 64: {
						var vector = state.Evaluate(list[1]).As<ulong>();
						var sum = 0UL;
						for(var i = 0; i < count; ++i)
							sum += (ulong) vector[i];
						return sum;
					}
					default:
						throw new BailoutException();
				}
			});

		Expression("vec-frsqrte", list => EType.Vector.AsRuntime(list.AnyRuntime), 
				list => $"Math.VectorFrsqrte({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])})", 
				list => $"({GenerateExpression(list[1])}).VectorFrsqrte((IRuntimeValue<int>) builder.EnsureRuntime({GenerateExpression(list[2])}), (IRuntimeValue<int>) builder.EnsureRuntime({GenerateExpression(list[3])}))")
			.Interpret((list, state) => {
				var vector = state.Evaluate(list[1]);
				switch((int) state.Evaluate(list[2])) {
					case 64:
						return ((Vector128<double>) Vector128<double>.Ensure(vector)).Map(FastInvsqrt);
					case 32:
						var count = (int) state.Evaluate(list[3]);
						return ((Vector128<float>) Vector128<float>.Ensure(vector)).Map((i, x) => i < count ? FastInvsqrt(x) : x);
					default:
						throw new NotSupportedException($"Only 32- and 64-bit frsqrte is supported");
				}
			});
			
		Expression("vec+", list => EType.Vector.AsRuntime(list.AnyRuntime), 
			list => list[3] switch {
				PInt(32) => $"({GenerateExpression(list[1])}) + ({GenerateExpression(list[2])})", 
				PInt(64) => $"reinterpret_cast<Vector128<float>>(reinterpret_cast<Vector128<double>>({GenerateExpression(list[1])}) + reinterpret_cast<Vector128<double>>({GenerateExpression(list[2])}))",
				_ => throw new NotSupportedException()
			}, 
			list => list[3] switch {
				PInt(32) => $"({GenerateExpression(list[1])}) + ({GenerateExpression(list[2])})", 
				PInt(64) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<double>>) ({GenerateExpression(list[1])}) + (IRuntimeValue<Vector128<double>>) ({GenerateExpression(list[2])}))",
				_ => throw new NotSupportedException()
			}).Interpret((list, state) => (int) state.Evaluate(list[3]) switch {
			8 => Vector128<byte>.Ensure(state.Evaluate(list[1])) + Vector128<byte>.Ensure(state.Evaluate(list[2])), 
			16 => Vector128<ushort>.Ensure(state.Evaluate(list[1])) + Vector128<ushort>.Ensure(state.Evaluate(list[2])), 
			32 => Vector128<float>.Ensure(state.Evaluate(list[1])) + Vector128<float>.Ensure(state.Evaluate(list[2])), 
			64 => Vector128<double>.Ensure(state.Evaluate(list[1])) + Vector128<double>.Ensure(state.Evaluate(list[2])), 
			{} value => throw new NotSupportedException($"Size not supported in vec+: {value}")
		});
			
		Expression("vec-", list => EType.Vector.AsRuntime(list.AnyRuntime), 
			list => list[3] switch {
				PInt(32) => $"({GenerateExpression(list[1])}) - ({GenerateExpression(list[2])})", 
				PInt(64) => $"reinterpret_cast<Vector128<float>>(reinterpret_cast<Vector128<double>>({GenerateExpression(list[1])}) - reinterpret_cast<Vector128<double>>({GenerateExpression(list[2])}))",
				_ => throw new NotSupportedException()
			}, 
			list => list[3] switch {
				PInt(32) => $"({GenerateExpression(list[1])}) - ({GenerateExpression(list[2])})", 
				PInt(64) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<double>>) ({GenerateExpression(list[1])}) - (IRuntimeValue<Vector128<double>>) ({GenerateExpression(list[2])}))",
				_ => throw new NotSupportedException()
			}).Interpret((list, state) => (int) state.Evaluate(list[3]) switch {
			8 => Vector128<byte>.Ensure(state.Evaluate(list[1])) - Vector128<byte>.Ensure(state.Evaluate(list[2])), 
			16 => Vector128<ushort>.Ensure(state.Evaluate(list[1])) - Vector128<ushort>.Ensure(state.Evaluate(list[2])), 
			32 => Vector128<float>.Ensure(state.Evaluate(list[1])) - Vector128<float>.Ensure(state.Evaluate(list[2])), 
			64 => Vector128<double>.Ensure(state.Evaluate(list[1])) - Vector128<double>.Ensure(state.Evaluate(list[2])), 
			{} value => throw new NotSupportedException($"Size not supported in vec-: {value}")
		});
			
		Expression("vec*", list => EType.Vector.AsRuntime(list.AnyRuntime), 
			list => list[3] switch {
				PInt(32) => $"({GenerateExpression(list[1])}) * ({GenerateExpression(list[2])})", 
				PInt(64) => $"reinterpret_cast<Vector128<float>>(reinterpret_cast<Vector128<double>>({GenerateExpression(list[1])}) * reinterpret_cast<Vector128<double>>({GenerateExpression(list[2])}))",
				_ => throw new NotSupportedException()
			}, 
			list => list[3] switch {
				PInt(32) => $"({GenerateExpression(list[1])}) * ({GenerateExpression(list[2])})", 
				PInt(64) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<double>>) ({GenerateExpression(list[1])}) * (IRuntimeValue<Vector128<double>>) ({GenerateExpression(list[2])}))",
				_ => throw new NotSupportedException()
			}).Interpret((list, state) => (int) state.Evaluate(list[3]) switch {
			8 => Vector128<byte>.Ensure(state.Evaluate(list[1])) * Vector128<byte>.Ensure(state.Evaluate(list[2])), 
			16 => Vector128<ushort>.Ensure(state.Evaluate(list[1])) * Vector128<ushort>.Ensure(state.Evaluate(list[2])), 
			32 => Vector128<float>.Ensure(state.Evaluate(list[1])) * Vector128<float>.Ensure(state.Evaluate(list[2])), 
			64 => Vector128<double>.Ensure(state.Evaluate(list[1])) * Vector128<double>.Ensure(state.Evaluate(list[2])), 
			{} value => throw new NotSupportedException($"Size not supported in vec*: {value}")
		});

		string CastVector(PTree elem, string type) =>
			elem.Type is EVector
				? $"(IRuntimeValue<Vector128<{type}>>) ({GenerateExpression(elem)})"
				: $"({GenerateExpression(elem)})";
		string RuntimeCastVector(PTree elem, string type) =>
			elem.Type is EVector
				? $"((IRuntimeValue<Vector128<{type}>>) ({GenerateExpression(elem)}))"
				: $"({GenerateExpression(elem)})";
			
		Expression("vec-uint+", list => EType.Vector.AsRuntime(list.AnyRuntime), 
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
			}).Interpret((list, state) => (int) state.Evaluate(list[3]) switch {
				8 => Vector128<byte>.Ensure(state.Evaluate(list[1])) + Vector128<byte>.Ensure(state.Evaluate(list[2])), 
				16 => Vector128<ushort>.Ensure(state.Evaluate(list[1])) + Vector128<ushort>.Ensure(state.Evaluate(list[2])), 
				32 => Vector128<uint>.Ensure(state.Evaluate(list[1])) + Vector128<uint>.Ensure(state.Evaluate(list[2])), 
				64 => Vector128<ulong>.Ensure(state.Evaluate(list[1])) + Vector128<ulong>.Ensure(state.Evaluate(list[2])), 
				{} value => throw new NotSupportedException($"Size not supported in vec-uint+: {value}")
			});
			
		Expression("vec-uint*", list => EType.Vector.AsRuntime(list.AnyRuntime), 
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
			}).Interpret((list, state) => (int) state.Evaluate(list[3]) switch {
				8 => Vector128<byte>.Ensure(state.Evaluate(list[1])) * Vector128<byte>.Ensure(state.Evaluate(list[2])), 
				16 => Vector128<ushort>.Ensure(state.Evaluate(list[1])) * Vector128<ushort>.Ensure(state.Evaluate(list[2])), 
				32 => Vector128<uint>.Ensure(state.Evaluate(list[1])) * Vector128<uint>.Ensure(state.Evaluate(list[2])), 
				64 => Vector128<ulong>.Ensure(state.Evaluate(list[1])) * Vector128<ulong>.Ensure(state.Evaluate(list[2])), 
			{} value => throw new NotSupportedException($"Size not supported in vec-uint*: {value}")
		});
			
		Expression("vec/", list => EType.Vector.AsRuntime(list.AnyRuntime), 
			list => list[3] switch {
				PInt(32) => $"({GenerateExpression(list[1])}) / ({GenerateExpression(list[2])})", 
				PInt(64) => $"(({GenerateExpression(list[1])}).As<float, double>() / ({GenerateExpression(list[2])}).As<float, double>()).As<double, float>()",
				_ => throw new NotSupportedException()
			}, 
			list => list[3] switch {
				PInt(32) => $"({GenerateExpression(list[1])}) / ({GenerateExpression(list[2])})", 
				PInt(64) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<double>>) ({GenerateExpression(list[1])}) / (IRuntimeValue<Vector128<double>>) ({GenerateExpression(list[2])}))",
				_ => throw new NotSupportedException()
			}).Interpret((list, state) => (int) state.Evaluate(list[3]) switch {
			8 => Vector128<byte>.Ensure(state.Evaluate(list[1])) / Vector128<byte>.Ensure(state.Evaluate(list[2])), 
			16 => Vector128<ushort>.Ensure(state.Evaluate(list[1])) / Vector128<ushort>.Ensure(state.Evaluate(list[2])), 
			32 => Vector128<float>.Ensure(state.Evaluate(list[1])) / Vector128<float>.Ensure(state.Evaluate(list[2])), 
			64 => Vector128<double>.Ensure(state.Evaluate(list[1])) / Vector128<double>.Ensure(state.Evaluate(list[2])), 
			{} value => throw new NotSupportedException($"Size not supported in vec/: {value}")
		});
			
		Expression("vec&", list => list[1].Type, 
				list => $"Vector128.BitwiseAnd({GenerateExpression(list[1])}, {GenerateExpression(list[2])})",
				list => $"({GenerateExpression(list[1])}) & ({GenerateExpression(list[2])})")
			.Interpret((list, state) => Vector128<byte>.Ensure(state.Evaluate(list[1])) & Vector128<byte>.Ensure(state.Evaluate(list[2])));
			
		Expression("vec&~", list => list[1].Type, 
				list => $"Vector128.AndNot({GenerateExpression(list[1])}, {GenerateExpression(list[2])})",
				list => $"({GenerateExpression(list[1])}) & ~({GenerateExpression(list[2])})")
			.Interpret((list, state) => Vector128<byte>.Ensure(state.Evaluate(list[1])) & ~Vector128<byte>.Ensure(state.Evaluate(list[2])));
			
		Expression("vec|", list => list[1].Type, 
				list => $"Vector128.BitwiseOr({GenerateExpression(list[1])}, {GenerateExpression(list[2])})",
				list => $"({GenerateExpression(list[1])}) | ({GenerateExpression(list[2])})")
			.Interpret((list, state) => Vector128<byte>.Ensure(state.Evaluate(list[1])) | Vector128<byte>.Ensure(state.Evaluate(list[2])));
		
		Expression("vec^", list => list[1].Type, 
				list => $"Vector128.Xor({GenerateExpression(list[1])}, {GenerateExpression(list[2])})",
				list => $"({GenerateExpression(list[1])}) ^ ({GenerateExpression(list[2])})")
			.Interpret((list, state) => Vector128<byte>.Ensure(state.Evaluate(list[1])) ^ Vector128<byte>.Ensure(state.Evaluate(list[2])));
		
		Expression("vec~", list => list[1].Type, 
				list => $"Vector128.Not({GenerateExpression(list[1])})",
				list => $"~({GenerateExpression(list[1])})")
			.NoInterpret(); // TODO
	}
}