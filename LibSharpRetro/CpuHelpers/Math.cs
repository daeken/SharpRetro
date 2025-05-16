namespace LibSharpRetro.CpuHelpers; 

public static class Math {
	public static int CountLeadingZeros(this ushort v) {
		var c = 0;
		for(var i = 0x8000; i != 0; i >>= 1, ++c) {
			if((v & i) != 0)
				return c;
		}
		return 0;
	}

	public static int CountLeadingZeros(this uint v) {
		var c = 0;
		for(var i = 0x80000000U; i != 0; i >>= 1, ++c) {
			if((v & i) != 0)
				return c;
		}
		return 0;
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
}