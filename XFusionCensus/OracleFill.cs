namespace XFusionCensus;

/// Deterministic vector/memory contents for oracle states, shared (as source) with
/// XFusionTests' golden replay, which regenerates the same bytes from the recorded
/// seed. A mix of raw bits and plausible float lanes, so float ops see ordinary values
/// (raw bits alone are NaN/denormal-heavy and mostly test those). Only seeded Random
/// and exact arithmetic: no Math.Pow, whose last bit may differ between hosts.
public static class OracleFill {
	static readonly double[] Pow10 = [1e-4, 1e-3, 1e-2, 1e-1, 1, 1e1, 1e2, 1e3, 1e4];

	public static void Fill(Random rng, Span<byte> buf) {
		var mode = rng.Next(4);
		for(var o = 0; o + 8 <= buf.Length; o += 8) {
			ulong v = mode switch {
				0 => (ulong) rng.NextInt64() ^ ((ulong) rng.NextInt64() << 1),
				1 => (ulong) BitConverter.SingleToUInt32Bits(RandFloat(rng))
				     | ((ulong) BitConverter.SingleToUInt32Bits(RandFloat(rng)) << 32),
				2 => BitConverter.DoubleToUInt64Bits(RandFloat(rng) * (double) RandFloat(rng)),
				_ => (ulong) rng.Next(-300, 300) & 0xFFFF | ((ulong) rng.Next(-300, 300) & 0xFFFF) << 16
				     | ((ulong) rng.Next(0, 70000)) << 32,
			};
			BitConverter.TryWriteBytes(buf[o..], v);
		}
	}

	/// All 16 ymm registers (32 bytes each). One state in three draws every dword from a
	/// small shared pool, so operands are often EQUAL lane-for-lane -- independent
	/// random lanes almost never are, and then LE vs LT, min/max ties and equal-each
	/// string compares are untested whatever the case count.
	public static void FillRegs(Random rng, Span<byte> ymm) {
		if(rng.Next(3) != 0) {
			for(var r = 0; r < 16; r++) Fill(rng, ymm.Slice(r * 32, 32));
			return;
		}
		Span<byte> pool = stackalloc byte[32];
		Fill(rng, pool);
		for(var o = 0; o + 4 <= ymm.Length; o += 4)
			pool.Slice(rng.Next(6) * 4, 4).CopyTo(ymm[o..]);
	}

	static float RandFloat(Random rng) => rng.Next(10) switch {
		0 => 0f,
		1 => rng.Next(2) == 0 ? 1f : -1f,
		2 => rng.Next(-1000, 1000),
		_ => (float) ((rng.NextDouble() * 2 - 1) * Pow10[rng.Next(0, 9)]),
	};

	/// Stable (process-independent) seed for an encoding: string.GetHashCode is
	/// randomized per process, so the oracle's cases would not be reproducible.
	public static int SeedOf(string hex) {
		var h = 2166136261u;
		foreach(var c in hex) h = (h ^ c) * 16777619u;
		return (int) (h & 0x7FFFFFFF);
	}
}
