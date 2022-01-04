using System;

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
		Expression("vector-all", list => EType.Vector.AsRuntime(),
				list => $"reinterpret_cast<Vector128<float>>(({GenerateExpression(list[1])}) - (Vector128<{GenerateType(list[1].Type)}>) {{}})",
				list => $"(({GenerateType(list[1].Type.AsRuntime())}) ({GenerateExpression(list[1])})).CreateVector()")
			.Interpret((list, state) => Vector128<byte>.FromDynamic(state.Evaluate(list[1])));
		Expression("vector-zero-top", list => EType.Vector.AsRuntime(), 
				list => $"reinterpret_cast<Vector128<float>>(reinterpret_cast<Vector128<uint64_t>>({GenerateExpression(list[1])})[0] - (Vector128<uint64_t>) {{}})", 
				list => $"({GenerateExpression(list[1])}).ZeroTop()")
			.Interpret((list, state) => state.Evaluate(list[1]).ZeroTop());
		Expression("vector-insert", _ => EType.Unit,
				list => $"reinterpret_cast<Vector128<{GenerateType(list[3].Type)}>*>(&(state->V[(int) ({GenerateExpression(list[1])})]))[0][{GenerateExpression(list[2])}] = {GenerateExpression(list[3])}",
				list => $"VR[(int) ({GenerateExpression(list[1])})] = VR[(int) ({GenerateExpression(list[1])})]().Insert({GenerateExpression(list[2])}, {GenerateExpression(list[3])})")
			.Interpret((list, state) => {
				var name = $"V{state.Evaluate(list[1])}";
				var vector = state.GetRegister(name).As(list[3].Type).Copy();
				var value = state.Evaluate(list[3]);
				value = list[3].Type switch {
					EInt(false, 8) => (byte) value, 
					EInt(true, 8) => (sbyte) value, 
					EInt(false, 16) => (ushort) value, 
					EInt(true, 16) => (short) value, 
					EInt(false, 32) => (uint) value, 
					EInt(true, 32) => (int) value, 
					EInt(false, 64) => (ulong) value, 
					EInt(true, 64) => (long) value,
					EFloat(32) => (float) value, 
					EFloat(64) => (double) value, 
					_ => throw new NotSupportedException()
				};
				vector[(int) state.Evaluate(list[2])] = value;
				state.Registers[name] = vector;
				return null;
			});
		Expression("vector-element", list => TypeFromName(list[3]).AsRuntime(),
				list => $"reinterpret_cast<Vector128<{GenerateType(list.Type.AsCompiletime())}>>({GenerateExpression(list[1])})[{GenerateExpression(list[2])}]",
				list => $"({GenerateExpression(list[1])}).Element<{GenerateType(list.Type.AsCompiletime())}>({GenerateExpression(list[2])})")
			.Interpret((list, state) => state.Evaluate(list[1]).As(TypeFromName(list[3]))[(int) state.Evaluate(list[2])]);
		Expression("vector-extract", list => EType.Vector.AsRuntime(list[1].Type.Runtime || list[2].Type.Runtime), 
				list => $"VectorExtract({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])}, {GenerateExpression(list[4])})", 
				list => $"Call<Vector128<float>, Vector128<float>, Vector128<float>, uint, uint>(VectorExtract, {GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])}, {GenerateExpression(list[4])})")
			.NoInterpret(); // TODO: Implement

		Expression("vector-count-bits", _ => EType.Vector, 
				list => $"VectorCountBits({GenerateExpression(list[1])}, {GenerateExpression(list[2])})", 
				list => $"Call<Vector128<float>, Vector128<float>, long>(VectorCountBits, {GenerateExpression(list[1])}, {GenerateExpression(list[2])})")
			.NoInterpret(); // TODO: Implement

		Expression("vector-sum-unsigned", _ => new EInt(false, 64),
				list => $"VectorSumUnsigned({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])})", 
				list => $"Call<ulong, Vector128<float>, long, long>(VectorSumUnsigned, {GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])})")
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
				list => $"VectorFrsqrte({GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])})", 
				list => $"Call<Vector128<float>, Vector128<float>, int, int>(VectorFrsqrte, {GenerateExpression(list[1])}, {GenerateExpression(list[2])}, {GenerateExpression(list[3])})")
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
				? $"reinterpret_cast<Vector128<{type}>>({GenerateExpression(elem)})"
				: $"({GenerateExpression(elem)})";
		string RuntimeCastVector(PTree elem, string type) =>
			elem.Type is EVector
				? $"((IRuntimeValue<Vector128<{type}>>) ({GenerateExpression(elem)}))"
				: $"({GenerateExpression(elem)})";
			
		Expression("vec-uint+", list => EType.Vector.AsRuntime(list.AnyRuntime), 
			list => list[3] switch {
				PInt(8) => $"reinterpret_cast<Vector128<float>>(reinterpret_cast<Vector128<uint8_t>>({GenerateExpression(list[1])}) + ({CastVector(list[2], "uint8_t")}))",
				PInt(16) => $"reinterpret_cast<Vector128<float>>(reinterpret_cast<Vector128<uint16_t>>({GenerateExpression(list[1])}) + ({CastVector(list[2], "uint16_t")}))",
				PInt(32) => $"reinterpret_cast<Vector128<float>>(reinterpret_cast<Vector128<uint32_t>>({GenerateExpression(list[1])}) + ({CastVector(list[2], "uint32_t")}))",
				PInt(64) => $"reinterpret_cast<Vector128<float>>(reinterpret_cast<Vector128<uint64_t>>({GenerateExpression(list[1])}) + ({CastVector(list[2], "uint64_t")}))",
				_ => throw new NotSupportedException()
			}, 
			list => list[3] switch {
				PInt(8) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<uint8_t>>) ({GenerateExpression(list[1])}) + {RuntimeCastVector(list[2], "uint8_t")})",
				PInt(16) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<uint16_t>>) ({GenerateExpression(list[1])}) + {RuntimeCastVector(list[2], "uint16_t")})",
				PInt(32) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<uint32_t>>) ({GenerateExpression(list[1])}) + {RuntimeCastVector(list[2], "uint32_t")})",
				PInt(64) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<uint64_t>>) ({GenerateExpression(list[1])}) + {RuntimeCastVector(list[2], "uint64_t")})",
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
				PInt(8) => $"reinterpret_cast<Vector128<float>>(reinterpret_cast<Vector128<uint8_t>>({GenerateExpression(list[1])}) * ({CastVector(list[2], "uint8_t")}))",
				PInt(16) => $"reinterpret_cast<Vector128<float>>(reinterpret_cast<Vector128<uint16_t>>({GenerateExpression(list[1])}) * ({CastVector(list[2], "uint16_t")}))",
				PInt(32) => $"reinterpret_cast<Vector128<float>>(reinterpret_cast<Vector128<uint32_t>>({GenerateExpression(list[1])}) * ({CastVector(list[2], "uint32_t")}))",
				PInt(64) => $"reinterpret_cast<Vector128<float>>(reinterpret_cast<Vector128<uint64_t>>({GenerateExpression(list[1])}) * ({CastVector(list[2], "uint64_t")}))",
				_ => throw new NotSupportedException()
			}, 
			list => list[3] switch {
				PInt(8) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<uint8_t>>) ({GenerateExpression(list[1])}) * {RuntimeCastVector(list[2], "uint8_t")})",
				PInt(16) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<uint16_t>>) ({GenerateExpression(list[1])}) * {RuntimeCastVector(list[2], "uint16_t")})",
				PInt(32) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<uint32_t>>) ({GenerateExpression(list[1])}) * {RuntimeCastVector(list[2], "uint32_t")})",
				PInt(64) => $"(IRuntimeValue<Vector128<float>>) ((IRuntimeValue<Vector128<uint64_t>>) ({GenerateExpression(list[1])}) * {RuntimeCastVector(list[2], "uint64_t")})",
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
				PInt(64) => $"reinterpret_cast<Vector128<float>>(reinterpret_cast<Vector128<double>>({GenerateExpression(list[1])}) / reinterpret_cast<Vector128<double>>({GenerateExpression(list[2])}))",
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
				list => $"reinterpret_cast<Vector128<float>>((reinterpret_cast<Vector128<uint8_t>>({GenerateExpression(list[1])}) & reinterpret_cast<Vector128<uint8_t>>({GenerateExpression(list[2])})))",
				list => $"(IRuntimeValue<Vector128<float>>) ((((IRuntimeValue<Vector128<uint8_t>>) ({GenerateExpression(list[1])})) & ((IRuntimeValue<Vector128<uint8_t>>) ({GenerateExpression(list[2])}))))")
			.Interpret((list, state) => Vector128<byte>.Ensure(state.Evaluate(list[1])) & Vector128<byte>.Ensure(state.Evaluate(list[2])));
			
		Expression("vec&~", list => list[1].Type, 
				list => $"reinterpret_cast<Vector128<float>>(reinterpret_cast<Vector128<uint8_t>>({GenerateExpression(list[1])}) & ~reinterpret_cast<Vector128<uint8_t>>({GenerateExpression(list[2])}))",
				list => $"(IRuntimeValue<Vector128<float>>) (((IRuntimeValue<Vector128<uint8_t>>) ({GenerateExpression(list[1])})) & ~((IRuntimeValue<Vector128<uint8_t>>) ({GenerateExpression(list[2])})))")
			.Interpret((list, state) => Vector128<byte>.Ensure(state.Evaluate(list[1])) & ~Vector128<byte>.Ensure(state.Evaluate(list[2])));
			
		Expression("vec|", list => list[1].Type, 
				list => $"reinterpret_cast<Vector128<float>>((reinterpret_cast<Vector128<uint8_t>>({GenerateExpression(list[1])}) | reinterpret_cast<Vector128<uint8_t>>({GenerateExpression(list[2])})))",
				list => $"(IRuntimeValue<Vector128<float>>) ((((IRuntimeValue<Vector128<uint8_t>>) ({GenerateExpression(list[1])})) | ((IRuntimeValue<Vector128<uint8_t>>) ({GenerateExpression(list[2])}))))")
			.Interpret((list, state) => Vector128<byte>.Ensure(state.Evaluate(list[1])) | Vector128<byte>.Ensure(state.Evaluate(list[2])));
		
		Expression("vec^", list => list[1].Type, 
				list => $"reinterpret_cast<Vector128<float>>((reinterpret_cast<Vector128<uint8_t>>({GenerateExpression(list[1])}) ^ reinterpret_cast<Vector128<uint8_t>>({GenerateExpression(list[2])})))",
				list => $"(IRuntimeValue<Vector128<float>>) ((((IRuntimeValue<Vector128<uint8_t>>) ({GenerateExpression(list[1])})) ^ ((IRuntimeValue<Vector128<uint8_t>>) ({GenerateExpression(list[2])}))))")
			.Interpret((list, state) => Vector128<byte>.Ensure(state.Evaluate(list[1])) ^ Vector128<byte>.Ensure(state.Evaluate(list[2])));
	}
}