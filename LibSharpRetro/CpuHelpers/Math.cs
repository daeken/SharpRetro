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
		T Conv(object value) => (T) Convert.ChangeType(value, typeof(T));
		unchecked {
			return Conv(size switch {
				8 => (sbyte) (byte) imm,
				16 => (short) (ushort) imm,
				32 => (int) imm,
				{} when (imm & (1 << (size - 1))) != 0 => (int) imm - (1 << size),
				_ => (int) imm
			});
		}
	}
}