namespace ArchCompilerCore;

// Extracted {sig, exec} from legacy: CoreArchCompiler/VectorMath.cs
// Emit-lambdas → Backends/CSharp (rung-2). Local helpers lifted VERBATIM from legacy.
public class VectorMath : Builtin {

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
        Expr("vector", _ => EType.Vector.AsRuntime());
        Expr("vector-all", list => EType.Vector.AsRuntime(),
            (list, state) => Vector128<byte>.FromDynamic(state.Evaluate(list[1])));
        Expr("vector-zero-top", list => EType.Vector.AsRuntime(),
            (list, state) => state.Evaluate(list[1]).ZeroTop());
        Expr("vector-element", list => TypeFromName(list[3]).AsRuntime(),
            (list, state) => state.Evaluate(list[1]).As(TypeFromName(list[3]))[(int) state.Evaluate(list[2])]);
        Expr("vector-extract", list => EType.Vector.AsRuntime(list[1].Type.Runtime || list[2].Type.Runtime));
        Expr("vector-count-bits", _ => EType.Vector);
        Expr("vector-sum-unsigned", _ => new EInt(false, 64),
            (list, state) => {
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
        Expr("vec-frsqrte", list => EType.Vector.AsRuntime(list.AnyRuntime),
            (list, state) => {
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
        Expr("vec+", list => EType.Vector.AsRuntime(list.AnyRuntime),
            (list, state) => (int) state.Evaluate(list[3]) switch {
			8 => Vector128<byte>.Ensure(state.Evaluate(list[1])) + Vector128<byte>.Ensure(state.Evaluate(list[2])), 
			16 => Vector128<ushort>.Ensure(state.Evaluate(list[1])) + Vector128<ushort>.Ensure(state.Evaluate(list[2])), 
			32 => Vector128<float>.Ensure(state.Evaluate(list[1])) + Vector128<float>.Ensure(state.Evaluate(list[2])), 
			64 => Vector128<double>.Ensure(state.Evaluate(list[1])) + Vector128<double>.Ensure(state.Evaluate(list[2])), 
			{} value => throw new NotSupportedException($"Size not supported in vec+: {value}")
		});
        Expr("vec-", list => EType.Vector.AsRuntime(list.AnyRuntime),
            (list, state) => (int) state.Evaluate(list[3]) switch {
			8 => Vector128<byte>.Ensure(state.Evaluate(list[1])) - Vector128<byte>.Ensure(state.Evaluate(list[2])), 
			16 => Vector128<ushort>.Ensure(state.Evaluate(list[1])) - Vector128<ushort>.Ensure(state.Evaluate(list[2])), 
			32 => Vector128<float>.Ensure(state.Evaluate(list[1])) - Vector128<float>.Ensure(state.Evaluate(list[2])), 
			64 => Vector128<double>.Ensure(state.Evaluate(list[1])) - Vector128<double>.Ensure(state.Evaluate(list[2])), 
			{} value => throw new NotSupportedException($"Size not supported in vec-: {value}")
		});
        Expr("vec*", list => EType.Vector.AsRuntime(list.AnyRuntime),
            (list, state) => (int) state.Evaluate(list[3]) switch {
			8 => Vector128<byte>.Ensure(state.Evaluate(list[1])) * Vector128<byte>.Ensure(state.Evaluate(list[2])), 
			16 => Vector128<ushort>.Ensure(state.Evaluate(list[1])) * Vector128<ushort>.Ensure(state.Evaluate(list[2])), 
			32 => Vector128<float>.Ensure(state.Evaluate(list[1])) * Vector128<float>.Ensure(state.Evaluate(list[2])), 
			64 => Vector128<double>.Ensure(state.Evaluate(list[1])) * Vector128<double>.Ensure(state.Evaluate(list[2])), 
			{} value => throw new NotSupportedException($"Size not supported in vec*: {value}")
		});
        Expr("vec-uint+", list => EType.Vector.AsRuntime(list.AnyRuntime),
            (list, state) => (int) state.Evaluate(list[3]) switch {
				8 => Vector128<byte>.Ensure(state.Evaluate(list[1])) + Vector128<byte>.Ensure(state.Evaluate(list[2])), 
				16 => Vector128<ushort>.Ensure(state.Evaluate(list[1])) + Vector128<ushort>.Ensure(state.Evaluate(list[2])), 
				32 => Vector128<uint>.Ensure(state.Evaluate(list[1])) + Vector128<uint>.Ensure(state.Evaluate(list[2])), 
				64 => Vector128<ulong>.Ensure(state.Evaluate(list[1])) + Vector128<ulong>.Ensure(state.Evaluate(list[2])), 
				{} value => throw new NotSupportedException($"Size not supported in vec-uint+: {value}")
			});
        Expr("vec-uint*", list => EType.Vector.AsRuntime(list.AnyRuntime),
            (list, state) => (int) state.Evaluate(list[3]) switch {
				8 => Vector128<byte>.Ensure(state.Evaluate(list[1])) * Vector128<byte>.Ensure(state.Evaluate(list[2])), 
				16 => Vector128<ushort>.Ensure(state.Evaluate(list[1])) * Vector128<ushort>.Ensure(state.Evaluate(list[2])), 
				32 => Vector128<uint>.Ensure(state.Evaluate(list[1])) * Vector128<uint>.Ensure(state.Evaluate(list[2])), 
				64 => Vector128<ulong>.Ensure(state.Evaluate(list[1])) * Vector128<ulong>.Ensure(state.Evaluate(list[2])), 
			{} value => throw new NotSupportedException($"Size not supported in vec-uint*: {value}")
		});
        Expr("vec/", list => EType.Vector.AsRuntime(list.AnyRuntime),
            (list, state) => (int) state.Evaluate(list[3]) switch {
			8 => Vector128<byte>.Ensure(state.Evaluate(list[1])) / Vector128<byte>.Ensure(state.Evaluate(list[2])), 
			16 => Vector128<ushort>.Ensure(state.Evaluate(list[1])) / Vector128<ushort>.Ensure(state.Evaluate(list[2])), 
			32 => Vector128<float>.Ensure(state.Evaluate(list[1])) / Vector128<float>.Ensure(state.Evaluate(list[2])), 
			64 => Vector128<double>.Ensure(state.Evaluate(list[1])) / Vector128<double>.Ensure(state.Evaluate(list[2])), 
			{} value => throw new NotSupportedException($"Size not supported in vec/: {value}")
		});
        Expr("vec&", list => list[1].Type,
            (list, state) => Vector128<byte>.Ensure(state.Evaluate(list[1])) & Vector128<byte>.Ensure(state.Evaluate(list[2])));
        Expr("vec&~", list => list[1].Type,
            (list, state) => Vector128<byte>.Ensure(state.Evaluate(list[1])) & ~Vector128<byte>.Ensure(state.Evaluate(list[2])));
        Expr("vec|", list => list[1].Type,
            (list, state) => Vector128<byte>.Ensure(state.Evaluate(list[1])) | Vector128<byte>.Ensure(state.Evaluate(list[2])));
        Expr("vec^", list => list[1].Type,
            (list, state) => Vector128<byte>.Ensure(state.Evaluate(list[1])) ^ Vector128<byte>.Ensure(state.Evaluate(list[2])));
        Expr("vec~", list => list[1].Type);
    }
}
