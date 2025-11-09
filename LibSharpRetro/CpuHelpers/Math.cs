using System.Runtime.Intrinsics.X86;

namespace LibSharpRetro.CpuHelpers; 

using System.Runtime.Intrinsics;

public static class Math {
	public static uint CountLeadingZeros(this ushort v) {
		var c = 0U;
		for(var i = 0x8000; i != 0; i >>= 1, ++c) {
			if((v & i) != 0)
				return c;
		}
		return 0;
	}

	public static uint CountLeadingZeros(this uint v) {
		var c = 0U;
		for(var i = 0x80000000U; i != 0; i >>= 1, ++c) {
			if((v & i) != 0)
				return c;
		}
		return 0;
	}

	public static ulong CountLeadingZeros(this ulong v) {
		for(var i = 0; i < 64; ++i)
			if(((v >> (63 - i)) & 1) == 1)
				return (uint) i;
		return 64;
	}
	
	public static T SignExt<T>(uint imm, int size) {
		T Conv(int value) => (T) (object) (default(T) switch {
			byte => (byte) (sbyte) value,
			sbyte => (sbyte) value,
			ushort => (ushort) (short) value,
			short => (short) value,
			uint => (uint) value,
			int => value,
			ulong => (ulong) value,
			long => (long) value,
			_ => throw new NotSupportedException(),
		});
		unchecked {
			return Conv(size switch {
				8 => (sbyte) (byte) imm,
				16 => (short) (ushort) imm,
				32 => (int) imm,
				{} when (imm & (1 << (size - 1))) != 0 => (int) imm - (1 << size),
				_ => (int) imm,
			});
		}
	}

	public static T SignExt<T>(ulong imm, int size) {
		T Conv(long value) => (T) (object) (default(T) switch {
			byte => (byte) (sbyte) value,
			sbyte => (sbyte) value,
			ushort => (ushort) (short) value,
			short => (short) value,
			uint => (uint) value,
			int => (int) value,
			ulong => (ulong) value,
			long => value,
			_ => throw new NotSupportedException(),
		});
		unchecked {
			return Conv(size switch {
				8 => (sbyte) (byte) imm,
				16 => (short) (ushort) imm,
				32 => (int) imm,
				{} when (imm & (1UL << (size - 1))) != 0 => (int) imm - (1 << size),
				_ => (long) imm,
			});
		}
	}

	public static unsafe U Bitcast<T, U>(T v) where T : unmanaged where U : unmanaged => *(U*) &v;
	
	public static ulong VectorSumUnsigned(Vector128<float> vec, byte esize, byte count) {
		switch(esize) {
			case 8: {
				var bvec = vec.As<float, byte>();
				var sum = 0UL;
				for(var i = (byte) 0; i < count; ++i)
					sum += bvec.GetElement(i);
				return sum;
			}
			default: throw new NotSupportedException($"Unknown size for VectorSumUnsigned: {esize}");
		}
	}
	
	public static Vector128<float> VectorCountBits(Vector128<float> vec, long elems) {
		var ret = Vector128<byte>.Zero;
		var ivec = vec.As<float, byte>();
		for(var i = 0; i < elems; ++i)
			ret = ret.WithElement(i, (byte) Popcnt.PopCount(ivec.GetElement(i)));
		return ret.As<byte, float>();
	}

	public static Vector128<float> VectorExtract(Vector128<float> _a, Vector128<float> _b, uint Q, uint _index) {
		var index = (int) _index;
		var a = _a.As<float, byte>();
		var b = _b.As<float, byte>();
			
		var r = new Vector128<byte>();
		var count = Q == 0 ? 8 : 16;

		if(count == 8) {
			for(var i = index; i < 8; ++i)
				r = r.WithElement(i - index, a.GetElement(i));
			var offset = 8 - index;
			for(var i = offset; i < 8; ++i)
				r = r.WithElement(i, a.GetElement(i - offset));
		} else {
			for(var i = index; i < 16; ++i)
				r = r.WithElement(i - index, a.GetElement(i));
			var offset = 16 - index;
			for(var i = offset; i < 16; ++i)
				r = r.WithElement(i, a.GetElement(i - offset));
		}
			
		return r.As<byte, float>();
	}
	
	public static uint ReverseBits(uint v) {
		var x = 0U;
		for(var i = 0; i < 32; ++i)
			x |= ((v >> i) & 1) << (31 - i);
		return x;
	}

	public static ulong ReverseBits(ulong v) {
		var x = 0UL;
		for(var i = 0; i < 64; ++i)
			x |= ((v >> i) & 1) << (63 - i);
		return x;
	}
	
	public static Vector128<float> VectorFrsqrte(Vector128<float> input, int bits, int elements) {
		if(bits == 64) {
			var vec = input.As<float, double>();
			vec = vec.WithElement(0, FastInvsqrt(vec.GetElement(0)));
			vec = vec.WithElement(1, FastInvsqrt(vec.GetElement(1)));
			return vec.As<double, float>();
		}
		for(var i = 0; i < elements; ++i)
			input = input.WithElement(i, FastInvsqrt(input.GetElement(i)));
		return input;
	}

	public static unsafe float FastInvsqrt(float number) {
		var i = *(uint*) &number;
		i = 0x5f3759df - (i >> 1);
		var f = *(float*) &i;
		f *= 1.5f - 0.5f * f * f;
		return f;
	}

	public static unsafe double FastInvsqrt(double number) {
		var x2 = number * 0.5;
		var i = *(long*) &number;
		i = 0x5fe6eb50c7b537a9 - (i >> 1);
		var y = *(double*) &i;
		y *= 1.5 - x2 * y * y;
		return y;
	}
}
