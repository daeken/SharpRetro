using LiftIl;

namespace XFusionCpu;

/// x86 SSE floating-point results at the bit level, for the IL evaluator.
///
/// Host float arithmetic gets the finite cases right but not NaN production: x86 has
/// its own rules, and they are observable in every packed/scalar SSE result.
///   - A NaN operand propagates QUIETED (quiet bit set, payload kept); with two NaN
///     operands the FIRST source wins. (SDM Vol 1, 4.8.3.5 / table 4-7.)
///   - An invalid operation on non-NaN operands (inf-inf, 0*inf, 0/0, sqrt(-x)) yields
///     the "QNaN floating-point indefinite": sign SET (0xFFC00000 / 0xFFF8000000000000),
///     where ARM and .NET produce the positive default NaN.
///   - MIN/MAX are not IEEE minNum/maxNum: `a > b ? a : b` on the raw operands, so a NaN
///     in either position or equal operands (+0/-0) return the SECOND source unchanged
///     (an SNaN is NOT quieted).
public static class X86Float {
	const uint QuietBit32 = 0x00400000;
	const ulong QuietBit64 = 0x0008000000000000;
	const uint Indefinite32 = 0xFFC00000;
	const ulong Indefinite64 = 0xFFF8000000000000;

	static bool IsNan32(uint v) => (v & 0x7F800000) == 0x7F800000 && (v & 0x007FFFFF) != 0;
	static bool IsNan64(ulong v) => (v & 0x7FF0000000000000) == 0x7FF0000000000000 && (v & 0x000FFFFFFFFFFFFF) != 0;

	/// Binary op at width 32 or 64 on raw bit patterns. Returns null for ops this
	/// helper does not own (compares, bitwise), so the caller keeps its own arms.
	public static ulong? Bin(BinOp op, ulong a, ulong b, int bits) {
		if(op is BinOp.FMax or BinOp.FMin) {
			var gt = op == BinOp.FMax ? Gt(a, b, bits) : Gt(b, a, bits);
			return gt ? a : b;
		}
		if(op is not (BinOp.Add or BinOp.Sub or BinOp.Mul or BinOp.UDiv or BinOp.SDiv)) return null;
		if(bits == 32) {
			uint x = (uint) a, y = (uint) b;
			if(IsNan32(x)) return x | QuietBit32;
			if(IsNan32(y)) return y | QuietBit32;
			float fx = BitConverter.UInt32BitsToSingle(x), fy = BitConverter.UInt32BitsToSingle(y);
			var r = op switch {
				BinOp.Add => fx + fy, BinOp.Sub => fx - fy, BinOp.Mul => fx * fy, _ => fx / fy,
			};
			return float.IsNaN(r) ? Indefinite32 : BitConverter.SingleToUInt32Bits(r);
		} else {
			if(IsNan64(a)) return a | QuietBit64;
			if(IsNan64(b)) return b | QuietBit64;
			double fx = BitConverter.UInt64BitsToDouble(a), fy = BitConverter.UInt64BitsToDouble(b);
			var r = op switch {
				BinOp.Add => fx + fy, BinOp.Sub => fx - fy, BinOp.Mul => fx * fy, _ => fx / fy,
			};
			return double.IsNaN(r) ? Indefinite64 : BitConverter.DoubleToUInt64Bits(r);
		}
	}

	static bool Gt(ulong a, ulong b, int bits) => bits == 32
		? BitConverter.UInt32BitsToSingle((uint) a) > BitConverter.UInt32BitsToSingle((uint) b)
		: BitConverter.UInt64BitsToDouble(a) > BitConverter.UInt64BitsToDouble(b);

	/// Unary float op. Returns null for ops not owned here.
	public static ulong? Un(UnOp op, ulong a, int bits) {
		if(bits == 32) {
			var x = (uint) a;
			var f = BitConverter.UInt32BitsToSingle(x);
			switch(op) {
				case UnOp.Abs: return x & 0x7FFFFFFF;
				case UnOp.Neg: return x ^ 0x80000000;
				case UnOp.Sqrt:
					if(IsNan32(x)) return x | QuietBit32;
					var s = MathF.Sqrt(f);
					return float.IsNaN(s) ? Indefinite32 : BitConverter.SingleToUInt32Bits(s);
				case UnOp.Round or UnOp.Floor or UnOp.Ceil:
					if(IsNan32(x)) return x | QuietBit32;
					return BitConverter.SingleToUInt32Bits(op switch {
						UnOp.Round => MathF.Round(f, MidpointRounding.ToEven),
						UnOp.Floor => MathF.Floor(f),
						_ => MathF.Ceiling(f),
					});
			}
		} else {
			var d = BitConverter.UInt64BitsToDouble(a);
			switch(op) {
				case UnOp.Abs: return a & 0x7FFFFFFFFFFFFFFF;
				case UnOp.Neg: return a ^ 0x8000000000000000;
				case UnOp.Sqrt:
					if(IsNan64(a)) return a | QuietBit64;
					var s = Math.Sqrt(d);
					return double.IsNaN(s) ? Indefinite64 : BitConverter.DoubleToUInt64Bits(s);
				case UnOp.Round or UnOp.Floor or UnOp.Ceil:
					if(IsNan64(a)) return a | QuietBit64;
					return BitConverter.DoubleToUInt64Bits(op switch {
						UnOp.Round => Math.Round(d, MidpointRounding.ToEven),
						UnOp.Floor => Math.Floor(d),
						_ => Math.Ceiling(d),
					});
			}
		}
		return null;
	}

	/// f64 -> f32 narrowing (CVTSD2SS/CVTPD2PS): a NaN keeps its sign and the top
	/// payload bits and is quieted; everything else rounds per IEEE.
	public static uint Narrow(ulong a) {
		if(IsNan64(a))
			return (uint) (a >> 32 & 0x80000000) | 0x7FC00000 | (uint) (a >> 29 & 0x003FFFFF);
		return BitConverter.SingleToUInt32Bits((float) BitConverter.UInt64BitsToDouble(a));
	}

	/// f32 -> f64 widening (CVTSS2SD/CVTPS2PD): NaN payload shifts up, quieted.
	public static ulong Widen(uint a) {
		if(IsNan32(a))
			return ((ulong) (a & 0x80000000) << 32) | 0x7FF8000000000000 | ((ulong) (a & 0x003FFFFF) << 29);
		return BitConverter.DoubleToUInt64Bits(BitConverter.UInt32BitsToSingle(a));
	}
}
