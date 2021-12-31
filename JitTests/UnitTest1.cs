using System;
using CilJit;
using JitBase;
using NUnit.Framework;

namespace JitTests;

public class Tests {
	static IJit<byte>[] Jits8() => new IJit<byte>[] {
		new CilJit<byte>()
	};
	static IJit<uint>[] Jits32() => new IJit<uint>[] {
		new CilJit<uint>()
	};
	static IJit<ulong>[] Jits64() => new IJit<ulong>[] {
		new CilJit<ulong>()
	};

	[TestCaseSource(nameof(Jits8))]
	public void Test8_Basic(IJit<byte> jit) {
		var func = jit.CreateFunction<Func<uint, uint, uint>>("test1", b => {
			b.Return(b.Argument<uint>(0) + b.Argument<uint>(1));
		});
		Assert.AreEqual(func(0, 1), 1);
		Assert.AreEqual(func(1, 1), 2);
		Assert.AreEqual(func(2, 1), 3);
		Assert.AreEqual(func(5, 0xFFFFFFFF), 4);
	}
}