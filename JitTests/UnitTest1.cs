using System;
using System.Collections.Generic;
using CilJit;
using JitBase;
using NUnit.Framework;

namespace JitTests;

public class Tests {
	static IJit<uint>[] Jits32() => new IJit<uint>[] {
		new CilJit<uint>()
	};

	[TestCaseSource(nameof(Jits32))]
	public void Operations(IJit<uint> jit) {
		var u8values = new byte[] { 0, 1, 2, 0xFF, 0xFE };
		var i8values = new sbyte[] { 0, 1, 2, -1, -2 };
		var u16values = new ushort[] { 0, 1, 2, 0xFFFF, 0xFFFE };
		var i16values = new short[] { 0, 1, 2, -1, -2 };
		var u32values = new uint[] { 0, 1, 2, 0xFFFFFFFF, 0xFFFFFFFE };
		var i32values = new int[] { 0, 1, 2, -1, -2 };
		var u64values = new ulong[] { 0, 1, 2, 0xFFFFFFFFFFFFFFFF, 0xFFFFFFFFFFFFFFFE };
		var i64values = new long[] { 0, 1, 2, -1, -2 };

		void Test<T>(T[] values, Func<IRuntimeValue<T>, IRuntimeValue<T>, IRuntimeValue<T>> jitGen, Func<T, T, T> knownGood, bool isDivMod = false) where T : struct {
			var jitFunc = jit.CreateFunction<Func<T, T, T>>("test", builder =>
				builder.Return(jitGen(builder.Argument<T>(0), builder.Argument<T>(1))));
			foreach(var a in values)
				foreach(var b in values)
					if(!EqualityComparer<T>.Default.Equals(b, default) || !isDivMod)
						Assert.AreEqual(knownGood(a, b), jitFunc(a, b));
		}
		
		
		Test(u8values, (a, b) => a + b, (a, b) => (byte) (a + b));
		Test(i8values, (a, b) => a + b, (a, b) => (sbyte) (a + b));
		Test(u16values, (a, b) => a + b, (a, b) => (ushort) (a + b));
		Test(i16values, (a, b) => a + b, (a, b) => (short) (a + b));
		Test(u32values, (a, b) => a + b, (a, b) => a + b);
		Test(i32values, (a, b) => a + b, (a, b) => a + b);
		Test(u64values, (a, b) => a + b, (a, b) => a + b);
		Test(i64values, (a, b) => a + b, (a, b) => a + b);
		
		Test(u8values, (a, b) => a - b, (a, b) => (byte) (a - b));
		Test(i8values, (a, b) => a - b, (a, b) => (sbyte) (a - b));
		Test(u16values, (a, b) => a - b, (a, b) => (ushort) (a - b));
		Test(i16values, (a, b) => a - b, (a, b) => (short) (a - b));
		Test(u32values, (a, b) => a - b, (a, b) => a - b);
		Test(i32values, (a, b) => a - b, (a, b) => a - b);
		Test(u64values, (a, b) => a - b, (a, b) => a - b);
		Test(i64values, (a, b) => a - b, (a, b) => a - b);
		
		Test(u8values, (a, b) => a * b, (a, b) => (byte) (a * b));
		Test(i8values, (a, b) => a * b, (a, b) => (sbyte) (a * b));
		Test(u16values, (a, b) => a * b, (a, b) => (ushort) (a * b));
		Test(i16values, (a, b) => a * b, (a, b) => (short) (a * b));
		Test(u32values, (a, b) => a * b, (a, b) => a * b);
		Test(i32values, (a, b) => a * b, (a, b) => a * b);
		Test(u64values, (a, b) => a * b, (a, b) => a * b);
		Test(i64values, (a, b) => a * b, (a, b) => a * b);
		
		Test(u8values, (a, b) => a / b, (a, b) => (byte) (a / b), true);
		Test(i8values, (a, b) => a / b, (a, b) => (sbyte) (a / b), true);
		Test(u16values, (a, b) => a / b, (a, b) => (ushort) (a / b), true);
		Test(i16values, (a, b) => a / b, (a, b) => (short) (a / b), true);
		Test(u32values, (a, b) => a / b, (a, b) => a / b, true);
		Test(i32values, (a, b) => a / b, (a, b) => a / b, true);
		Test(u64values, (a, b) => a / b, (a, b) => a / b, true);
		Test(i64values, (a, b) => a / b, (a, b) => a / b, true);
		
		Test(u8values, (a, b) => a % b, (a, b) => (byte) (a % b), true);
		Test(i8values, (a, b) => a % b, (a, b) => (sbyte) (a % b), true);
		Test(u16values, (a, b) => a % b, (a, b) => (ushort) (a % b), true);
		Test(i16values, (a, b) => a % b, (a, b) => (short) (a % b), true);
		Test(u32values, (a, b) => a % b, (a, b) => a % b, true);
		Test(i32values, (a, b) => a % b, (a, b) => a % b, true);
		Test(u64values, (a, b) => a % b, (a, b) => a % b, true);
		Test(i64values, (a, b) => a % b, (a, b) => a % b, true);
		
		Test(u8values, (a, b) => a & b, (a, b) => (byte) (a & b));
		Test(i8values, (a, b) => a & b, (a, b) => (sbyte) (a & b));
		Test(u16values, (a, b) => a & b, (a, b) => (ushort) (a & b));
		Test(i16values, (a, b) => a & b, (a, b) => (short) (a & b));
		Test(u32values, (a, b) => a & b, (a, b) => a & b);
		Test(i32values, (a, b) => a & b, (a, b) => a & b);
		Test(u64values, (a, b) => a & b, (a, b) => a & b);
		Test(i64values, (a, b) => a & b, (a, b) => a & b);
		
		Test(u8values, (a, b) => a | b, (a, b) => (byte) (a | b));
		Test(i8values, (a, b) => a | b, (a, b) => (sbyte) (a | b));
		Test(u16values, (a, b) => a | b, (a, b) => (ushort) (a | b));
		Test(i16values, (a, b) => a | b, (a, b) => (short) (a | b));
		Test(u32values, (a, b) => a | b, (a, b) => a | b);
		Test(i32values, (a, b) => a | b, (a, b) => a | b);
		Test(u64values, (a, b) => a | b, (a, b) => a | b);
		Test(i64values, (a, b) => a | b, (a, b) => a | b);
		
		Test(u8values, (a, b) => a ^ b, (a, b) => (byte) (a ^ b));
		Test(i8values, (a, b) => a ^ b, (a, b) => (sbyte) (a ^ b));
		Test(u16values, (a, b) => a ^ b, (a, b) => (ushort) (a ^ b));
		Test(i16values, (a, b) => a ^ b, (a, b) => (short) (a ^ b));
		Test(u32values, (a, b) => a ^ b, (a, b) => a ^ b);
		Test(i32values, (a, b) => a ^ b, (a, b) => a ^ b);
		Test(u64values, (a, b) => a ^ b, (a, b) => a ^ b);
		Test(i64values, (a, b) => a ^ b, (a, b) => a ^ b);
	}
}