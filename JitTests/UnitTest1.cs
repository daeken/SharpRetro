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

	[TestCaseSource(nameof(Jits32))]
	public void ControlFlow(IJit<uint> jit) {
		var ifTest = jit.CreateFunction<Func<uint, uint>>("ifTest", builder => {
			var arg = builder.Argument<uint>(0);
			builder.If(arg.EQ(builder.LiteralValue(25U)), () => builder.Return(arg), () => builder.Return(arg * arg));
		});
		
		Assert.AreEqual(25, ifTest(25));
		Assert.AreEqual(0, ifTest(0));
		Assert.AreEqual(4, ifTest(2));
	}
	
	[TestCaseSource(nameof(Jits32))]
	public void NoReturnCalls(IJit<uint> jit) {
		var temp = false;
		void ActionTest() => temp = true;
		var callAction = jit.CreateFunction<Action>("action", builder => builder.Call(ActionTest));
		callAction();
		Assert.True(temp);

		temp = false;
		void ActionTest1(int a) => temp = a == 15;
		var callAction1 = jit.CreateFunction<Action>("action1", builder => builder.Call(ActionTest1, builder.LiteralValue(15)));
		callAction1();
		Assert.True(temp);

		temp = false;
		void ActionTest2(int a, int b) => temp = a == 15 && b == 27;
		var callAction2 = jit.CreateFunction<Action>("action2", builder => builder.Call(ActionTest2, builder.LiteralValue(15), builder.LiteralValue(27)));
		callAction2();
		Assert.True(temp);

		temp = false;
		void ActionTest3(int a, int b, int c) => temp = a == 15 && b == 27 && c == 123;
		var callAction3 = jit.CreateFunction<Action>("action3", builder => builder.Call(ActionTest3, builder.LiteralValue(15), builder.LiteralValue(27), builder.LiteralValue(123)));
		callAction3();
		Assert.True(temp);

		temp = false;
		void ActionTest4(int a, int b, int c, int d) => temp = a == 15 && b == 27 && c == 123 && d == -1;
		var callAction4 = jit.CreateFunction<Action>("action4", builder => builder.Call(ActionTest4, builder.LiteralValue(15), builder.LiteralValue(27), builder.LiteralValue(123), builder.LiteralValue(-1)));
		callAction4();
		Assert.True(temp);
	}

	[TestCaseSource(nameof(Jits32))]
	public void WithReturnCalls(IJit<uint> jit) {
		int FuncTest() => 5;
		var callFunc = jit.CreateFunction<Func<int>>("func", builder => builder.Return(builder.Call(FuncTest)));
		Assert.AreEqual(5, callFunc());
		
		int FuncTest1(int a) => a;
		var callFunc1 = jit.CreateFunction<Func<int>>("func1", builder => builder.Return(builder.Call(FuncTest1, builder.LiteralValue(123))));
		Assert.AreEqual(123, callFunc1());
		
		int FuncTest2(int a, int b) => a + b;
		var callFunc2 = jit.CreateFunction<Func<int>>("func2", builder => builder.Return(builder.Call(FuncTest2, builder.LiteralValue(123), builder.LiteralValue(-1))));
		Assert.AreEqual(122, callFunc2());
		
		int FuncTest3(int a, int b, int c) => a + b + c;
		var callFunc3 = jit.CreateFunction<Func<int>>("func3", builder => builder.Return(builder.Call(FuncTest3, builder.LiteralValue(123), builder.LiteralValue(-1), builder.LiteralValue(-2))));
		Assert.AreEqual(120, callFunc3());
		
		int FuncTest4(int a, int b, int c, int d) => (a + b + c) * d;
		var callFunc4 = jit.CreateFunction<Func<int>>("func4", builder => builder.Return(builder.Call(FuncTest4, builder.LiteralValue(123), builder.LiteralValue(-1), builder.LiteralValue(-2), builder.LiteralValue(2))));
		Assert.AreEqual(240, callFunc4());
	}
}