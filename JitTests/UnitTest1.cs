using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CilJit;
using JitBase;
using LlvmJit;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace JitTests;

public unsafe class Tests {
	static IJit<uint>[] Jits32() => [
		new CilJit<uint>(), 
		//new LlvmJit<uint>(),
	];

	static readonly byte[] U8values = [0, 1, 2, 0xFF, 0xFE];
	static readonly sbyte[] I8values = [0, 1, 2, -1, -2];
	static readonly ushort[] U16values = [0, 1, 2, 0xFFFF, 0xFFFE];
	static readonly short[] I16values = [0, 1, 2, -1, -2];
	static readonly uint[] U32values = [0, 1, 2, 0xFFFFFFFF, 0xFFFFFFFE];
	static readonly int[] I32values = [0, 1, 2, -1, -2];
	static readonly ulong[] U64values = [0, 1, 2, 0xFFFFFFFFFFFFFFFF, 0xFFFFFFFFFFFFFFFE];
	static readonly long[] I64values = [0, 1, 2, -1, -2];

	[TestCaseSource(nameof(Jits32))]
	public void EmptyFunction(IJit<uint> jit) {
		var func = jit.CreateFunction<Action>("empty", builder => {});
		func();
	}
	
	[TestCaseSource(nameof(Jits32))]
	public void ReturnArgument(IJit<uint> jit) {
		var func = jit.CreateFunction<Func<uint, uint>>("ret", builder => builder.Return(builder.Argument<uint>(0)));
		ClassicAssert.AreEqual(0, func(0));
		ClassicAssert.AreEqual(123, func(123));
	}
	
	[TestCaseSource(nameof(Jits32))]
	public void MathOperations(IJit<uint> jit) {
		void Test<T>(T[] values, Func<IRuntimeValue<T>, IRuntimeValue<T>, IRuntimeValue<T>> jitGen, Func<T, T, T> knownGood, bool isDivMod = false) where T : struct {
			var jitFunc = jit.CreateFunction<Func<T, T, T>>("test", builder =>
				builder.Return(jitGen(builder.Argument<T>(0), builder.Argument<T>(1))));
			foreach(var a in values)
				foreach(var b in values)
					if(!EqualityComparer<T>.Default.Equals(b, default) || !isDivMod)
						ClassicAssert.AreEqual(knownGood(a, b), jitFunc(a, b));
		}
		
		
		Test(U8values, (a, b) => a + b, (a, b) => (byte) (a + b));
		Test(I8values, (a, b) => a + b, (a, b) => (sbyte) (a + b));
		Test(U16values, (a, b) => a + b, (a, b) => (ushort) (a + b));
		Test(I16values, (a, b) => a + b, (a, b) => (short) (a + b));
		Test(U32values, (a, b) => a + b, (a, b) => a + b);
		Test(I32values, (a, b) => a + b, (a, b) => a + b);
		Test(U64values, (a, b) => a + b, (a, b) => a + b);
		Test(I64values, (a, b) => a + b, (a, b) => a + b);
		
		Test(U8values, (a, b) => a - b, (a, b) => (byte) (a - b));
		Test(I8values, (a, b) => a - b, (a, b) => (sbyte) (a - b));
		Test(U16values, (a, b) => a - b, (a, b) => (ushort) (a - b));
		Test(I16values, (a, b) => a - b, (a, b) => (short) (a - b));
		Test(U32values, (a, b) => a - b, (a, b) => a - b);
		Test(I32values, (a, b) => a - b, (a, b) => a - b);
		Test(U64values, (a, b) => a - b, (a, b) => a - b);
		Test(I64values, (a, b) => a - b, (a, b) => a - b);
		
		Test(U8values, (a, b) => a * b, (a, b) => (byte) (a * b));
		Test(I8values, (a, b) => a * b, (a, b) => (sbyte) (a * b));
		Test(U16values, (a, b) => a * b, (a, b) => (ushort) (a * b));
		Test(I16values, (a, b) => a * b, (a, b) => (short) (a * b));
		Test(U32values, (a, b) => a * b, (a, b) => a * b);
		Test(I32values, (a, b) => a * b, (a, b) => a * b);
		Test(U64values, (a, b) => a * b, (a, b) => a * b);
		Test(I64values, (a, b) => a * b, (a, b) => a * b);
		
		Test(U8values, (a, b) => a / b, (a, b) => (byte) (a / b), true);
		Test(I8values, (a, b) => a / b, (a, b) => (sbyte) (a / b), true);
		Test(U16values, (a, b) => a / b, (a, b) => (ushort) (a / b), true);
		Test(I16values, (a, b) => a / b, (a, b) => (short) (a / b), true);
		Test(U32values, (a, b) => a / b, (a, b) => a / b, true);
		Test(I32values, (a, b) => a / b, (a, b) => a / b, true);
		Test(U64values, (a, b) => a / b, (a, b) => a / b, true);
		Test(I64values, (a, b) => a / b, (a, b) => a / b, true);
		
		Test(U8values, (a, b) => a % b, (a, b) => (byte) (a % b), true);
		Test(I8values, (a, b) => a % b, (a, b) => (sbyte) (a % b), true);
		Test(U16values, (a, b) => a % b, (a, b) => (ushort) (a % b), true);
		Test(I16values, (a, b) => a % b, (a, b) => (short) (a % b), true);
		Test(U32values, (a, b) => a % b, (a, b) => a % b, true);
		Test(I32values, (a, b) => a % b, (a, b) => a % b, true);
		Test(U64values, (a, b) => a % b, (a, b) => a % b, true);
		Test(I64values, (a, b) => a % b, (a, b) => a % b, true);
		
		Test(U8values, (a, b) => a & b, (a, b) => (byte) (a & b));
		Test(I8values, (a, b) => a & b, (a, b) => (sbyte) (a & b));
		Test(U16values, (a, b) => a & b, (a, b) => (ushort) (a & b));
		Test(I16values, (a, b) => a & b, (a, b) => (short) (a & b));
		Test(U32values, (a, b) => a & b, (a, b) => a & b);
		Test(I32values, (a, b) => a & b, (a, b) => a & b);
		Test(U64values, (a, b) => a & b, (a, b) => a & b);
		Test(I64values, (a, b) => a & b, (a, b) => a & b);
		
		Test(U8values, (a, b) => a | b, (a, b) => (byte) (a | b));
		Test(I8values, (a, b) => a | b, (a, b) => (sbyte) (a | b));
		Test(U16values, (a, b) => a | b, (a, b) => (ushort) (a | b));
		Test(I16values, (a, b) => a | b, (a, b) => (short) (a | b));
		Test(U32values, (a, b) => a | b, (a, b) => a | b);
		Test(I32values, (a, b) => a | b, (a, b) => a | b);
		Test(U64values, (a, b) => a | b, (a, b) => a | b);
		Test(I64values, (a, b) => a | b, (a, b) => a | b);
		
		Test(U8values, (a, b) => a ^ b, (a, b) => (byte) (a ^ b));
		Test(I8values, (a, b) => a ^ b, (a, b) => (sbyte) (a ^ b));
		Test(U16values, (a, b) => a ^ b, (a, b) => (ushort) (a ^ b));
		Test(I16values, (a, b) => a ^ b, (a, b) => (short) (a ^ b));
		Test(U32values, (a, b) => a ^ b, (a, b) => a ^ b);
		Test(I32values, (a, b) => a ^ b, (a, b) => a ^ b);
		Test(U64values, (a, b) => a ^ b, (a, b) => a ^ b);
		Test(I64values, (a, b) => a ^ b, (a, b) => a ^ b);
	}

	[TestCaseSource(nameof(Jits32))]
	public void CompareOperations(IJit<uint> jit) {
		void Test<T>(T[] values, Func<IRuntimeValue<T>, IRuntimeValue<T>, IRuntimeValue<bool>> jitGen, Func<T, T, bool> knownGood) where T : struct {
			var jitFunc = jit.CreateFunction<Func<T, T, bool>>("test", builder =>
				builder.Return(jitGen(builder.Argument<T>(0), builder.Argument<T>(1))));
			foreach(var a in values)
				foreach(var b in values)
					ClassicAssert.AreEqual(knownGood(a, b), jitFunc(a, b));
		}
		
		Test(U8values, (a, b) => a < b, (a, b) => a < b);
		Test(I8values, (a, b) => a < b, (a, b) => a < b);
		Test(U16values, (a, b) => a < b, (a, b) => a < b);
		Test(I16values, (a, b) => a < b, (a, b) => a < b);
		Test(U32values, (a, b) => a < b, (a, b) => a < b);
		Test(I32values, (a, b) => a < b, (a, b) => a < b);
		Test(U64values, (a, b) => a < b, (a, b) => a < b);
		Test(I64values, (a, b) => a < b, (a, b) => a < b);

		Test(U8values, (a, b) => a <= b, (a, b) => a <= b);
		Test(I8values, (a, b) => a <= b, (a, b) => a <= b);
		Test(U16values, (a, b) => a <= b, (a, b) => a <= b);
		Test(I16values, (a, b) => a <= b, (a, b) => a <= b);
		Test(U32values, (a, b) => a <= b, (a, b) => a <= b);
		Test(I32values, (a, b) => a <= b, (a, b) => a <= b);
		Test(U64values, (a, b) => a <= b, (a, b) => a <= b);
		Test(I64values, (a, b) => a <= b, (a, b) => a <= b);

		Test(U8values, (a, b) => a == b, (a, b) => a == b);
		Test(I8values, (a, b) => a == b, (a, b) => a == b);
		Test(U16values, (a, b) => a == b, (a, b) => a == b);
		Test(I16values, (a, b) => a == b, (a, b) => a == b);
		Test(U32values, (a, b) => a == b, (a, b) => a == b);
		Test(I32values, (a, b) => a == b, (a, b) => a == b);
		Test(U64values, (a, b) => a == b, (a, b) => a == b);
		Test(I64values, (a, b) => a == b, (a, b) => a == b);

		Test(U8values, (a, b) => a != b, (a, b) => a != b);
		Test(I8values, (a, b) => a != b, (a, b) => a != b);
		Test(U16values, (a, b) => a != b, (a, b) => a != b);
		Test(I16values, (a, b) => a != b, (a, b) => a != b);
		Test(U32values, (a, b) => a != b, (a, b) => a != b);
		Test(I32values, (a, b) => a != b, (a, b) => a != b);
		Test(U64values, (a, b) => a != b, (a, b) => a != b);
		Test(I64values, (a, b) => a != b, (a, b) => a != b);

		Test(U8values, (a, b) => a >= b, (a, b) => a >= b);
		Test(I8values, (a, b) => a >= b, (a, b) => a >= b);
		Test(U16values, (a, b) => a >= b, (a, b) => a >= b);
		Test(I16values, (a, b) => a >= b, (a, b) => a >= b);
		Test(U32values, (a, b) => a >= b, (a, b) => a >= b);
		Test(I32values, (a, b) => a >= b, (a, b) => a >= b);
		Test(U64values, (a, b) => a >= b, (a, b) => a >= b);
		Test(I64values, (a, b) => a >= b, (a, b) => a >= b);

		Test(U8values, (a, b) => a > b, (a, b) => a > b);
		Test(I8values, (a, b) => a > b, (a, b) => a > b);
		Test(U16values, (a, b) => a > b, (a, b) => a > b);
		Test(I16values, (a, b) => a > b, (a, b) => a > b);
		Test(U32values, (a, b) => a > b, (a, b) => a > b);
		Test(I32values, (a, b) => a > b, (a, b) => a > b);
		Test(U64values, (a, b) => a > b, (a, b) => a > b);
		Test(I64values, (a, b) => a > b, (a, b) => a > b);
	}

	[TestCaseSource(nameof(Jits32))]
	public void ControlFlow(IJit<uint> jit) {
		var ifTest = jit.CreateFunction<Func<uint, uint>>("ifTest", builder => {
			var arg = builder.Argument<uint>(0);
			builder.If(arg.EQ(builder.LiteralValue(25U)), () => builder.Return(arg), () => builder.Return(arg * arg));
		});
		
		ClassicAssert.AreEqual(25, ifTest(25));
		ClassicAssert.AreEqual(0, ifTest(0));
		ClassicAssert.AreEqual(4, ifTest(2));

		var ternaryTest = jit.CreateFunction<Func<bool, uint, uint, uint>>("ternaryTest", builder =>
			builder.Return(builder.Ternary(builder.Argument<bool>(0), builder.Argument<uint>(1), builder.Argument<uint>(2))));
		ClassicAssert.AreEqual(0, ternaryTest(true, 0, 0));
		ClassicAssert.AreEqual(0, ternaryTest(false, 0, 0));
		ClassicAssert.AreEqual(0, ternaryTest(true, 0, 1));
		ClassicAssert.AreEqual(0, ternaryTest(false, 1, 0));
		ClassicAssert.AreEqual(123, ternaryTest(true, 123, 321));
		ClassicAssert.AreEqual(321, ternaryTest(false, 123, 321));

		var whileTest = jit.CreateFunction<Func<uint>>("whileTest", builder => {
			var local = builder.DefineLocal<uint>();
			local.Value = builder.LiteralValue(0U);
			builder.While(local.Value.NE(builder.LiteralValue(5U)), () => local.Value += builder.LiteralValue(1U));
			builder.Return(local.Value);
		});
		ClassicAssert.AreEqual(5, whileTest());

		var doWhileTest = jit.CreateFunction<Func<uint>>("doWhileTest", builder => {
			var local = builder.DefineLocal<uint>();
			local.Value = builder.LiteralValue(0U);
			builder.DoWhile(() => local.Value += builder.LiteralValue(1U), local.Value.NE(builder.LiteralValue(5U)));
			builder.Return(local.Value);
		});
		ClassicAssert.AreEqual(5, doWhileTest());

		var whenTest = jit.CreateFunction<Func<uint, uint>>("whenTest", builder => {
			builder.When(builder.Argument<uint>(0) == builder.LiteralValue(0U), () => builder.Return(builder.LiteralValue(123U)));
			builder.Return(builder.Argument<uint>(0));
		});
		ClassicAssert.AreEqual(123, whenTest(0));
		ClassicAssert.AreEqual(1, whenTest(1));
		ClassicAssert.AreEqual(3, whenTest(3));

		var unlessTest = jit.CreateFunction<Func<uint, uint>>("unlessTest", builder => {
			builder.Unless(builder.Argument<uint>(0) == builder.LiteralValue(0U), () => builder.Return(builder.LiteralValue(123U)));
			builder.Return(builder.Argument<uint>(0));
		});
		ClassicAssert.AreEqual(0, unlessTest(0));
		ClassicAssert.AreEqual(123, unlessTest(1));
		ClassicAssert.AreEqual(123, unlessTest(3));
	}
	
	[TestCaseSource(nameof(Jits32))]
	public void NoReturnCalls(IJit<uint> jit) {
		var temp = false;
		void ActionTest() => temp = true;
		var callAction = jit.CreateFunction<Action>("action", builder => builder.Call(ActionTest));
		callAction();
		ClassicAssert.True(temp);

		temp = false;
		void ActionTest1(int a) => temp = a == 15;
		var callAction1 = jit.CreateFunction<Action>("action1", builder => builder.Call(ActionTest1, builder.LiteralValue(15)));
		callAction1();
		ClassicAssert.True(temp);

		temp = false;
		void ActionTest2(int a, int b) => temp = a == 15 && b == 27;
		var callAction2 = jit.CreateFunction<Action>("action2", builder => builder.Call(ActionTest2, builder.LiteralValue(15), builder.LiteralValue(27)));
		callAction2();
		ClassicAssert.True(temp);

		temp = false;
		void ActionTest3(int a, int b, int c) => temp = a == 15 && b == 27 && c == 123;
		var callAction3 = jit.CreateFunction<Action>("action3", builder => builder.Call(ActionTest3, builder.LiteralValue(15), builder.LiteralValue(27), builder.LiteralValue(123)));
		callAction3();
		ClassicAssert.True(temp);

		temp = false;
		void ActionTest4(int a, int b, int c, int d) => temp = a == 15 && b == 27 && c == 123 && d == -1;
		var callAction4 = jit.CreateFunction<Action>("action4", builder => builder.Call(ActionTest4, builder.LiteralValue(15), builder.LiteralValue(27), builder.LiteralValue(123), builder.LiteralValue(-1)));
		callAction4();
		ClassicAssert.True(temp);
	}

	[TestCaseSource(nameof(Jits32))]
	public void WithReturnCalls(IJit<uint> jit) {
		int FuncTest() => 5;
		var callFunc = jit.CreateFunction<Func<int>>("func", builder => builder.Return(builder.Call(FuncTest)));
		ClassicAssert.AreEqual(5, callFunc());
		
		int FuncTest1(int a) => a;
		var callFunc1 = jit.CreateFunction<Func<int>>("func1", builder => builder.Return(builder.Call(FuncTest1, builder.LiteralValue(123))));
		ClassicAssert.AreEqual(123, callFunc1());
		
		int FuncTest2(int a, int b) => a + b;
		var callFunc2 = jit.CreateFunction<Func<int>>("func2", builder => builder.Return(builder.Call(FuncTest2, builder.LiteralValue(123), builder.LiteralValue(-1))));
		ClassicAssert.AreEqual(122, callFunc2());
		
		int FuncTest3(int a, int b, int c) => a + b + c;
		var callFunc3 = jit.CreateFunction<Func<int>>("func3", builder => builder.Return(builder.Call(FuncTest3, builder.LiteralValue(123), builder.LiteralValue(-1), builder.LiteralValue(-2))));
		ClassicAssert.AreEqual(120, callFunc3());
		
		int FuncTest4(int a, int b, int c, int d) => (a + b + c) * d;
		var callFunc4 = jit.CreateFunction<Func<int>>("func4", builder => builder.Return(builder.Call(FuncTest4, builder.LiteralValue(123), builder.LiteralValue(-1), builder.LiteralValue(-2), builder.LiteralValue(2))));
		ClassicAssert.AreEqual(240, callFunc4());
	}

	[TestCaseSource(nameof(Jits32))]
	public void LocalVars(IJit<uint> jit) {
		void Test<T>(T[] values) where T : struct {
			var func = jit.CreateFunction<Func<T, T>>("func", builder => {
				var local = builder.DefineLocal<T>();
				local.Value = builder.Argument<T>(0);
				builder.Return(local.Value);
			});
			foreach(var a in values)
				ClassicAssert.AreEqual(a, func(a));
		}
		
		Test(U8values);
		Test(I8values);
		Test(U16values);
		Test(I16values);
		Test(U32values);
		Test(I32values);
		Test(U64values);
		Test(I64values);
	}

	[TestCaseSource(nameof(Jits32))]
	public void Store(IJit<uint> jit) {
		var i = 0;
		int Inc() => i++;
		var func = jit.CreateFunction<Func<int>>("func", builder => {
			var val = builder.Call(Inc);
			builder.Return(val + val);
		});
		ClassicAssert.AreEqual(1, func());
		ClassicAssert.AreEqual(2, i);

		i = 0;
		func = jit.CreateFunction<Func<int>>("func", builder => {
			var val = builder.Call(Inc);
			val = val.Store();
			builder.Return(val + val);
		});
		ClassicAssert.AreEqual(0, func());
		ClassicAssert.AreEqual(1, i);
	}

	[TestCaseSource(nameof(Jits32))]
	public void Cast(IJit<uint> jit) {
		void Test<T, OT>(T[] values, Func<T, OT> knownGood) where T : struct where OT : struct {
			var jitFunc = jit.CreateFunction<Func<T, OT>>("test", builder =>
				builder.Return(builder.Argument<T>(0).Cast<OT>()));
			foreach(var a in values)
				ClassicAssert.AreEqual(knownGood(a), jitFunc(a));
		}
		
		Test(U8values, x => (byte) x);
		Test(U8values, x => (sbyte) x);
		Test(U8values, x => (ushort) x);
		Test(U8values, x => (short) x);
		Test(U8values, x => (uint) x);
		Test(U8values, x => (int) x);
		Test(U8values, x => (ulong) x);
		Test(U8values, x => (long) x);

		Test(I8values, x => (byte) x);
		Test(I8values, x => (sbyte) x);
		Test(I8values, x => (ushort) x);
		Test(I8values, x => (short) x);
		Test(I8values, x => (uint) x);
		Test(I8values, x => (int) x);
		Test(I8values, x => (ulong) x);
		Test(I8values, x => (long) x);

		Test(U16values, x => (byte) x);
		Test(U16values, x => (sbyte) x);
		Test(U16values, x => (ushort) x);
		Test(U16values, x => (short) x);
		Test(U16values, x => (uint) x);
		Test(U16values, x => (int) x);
		Test(U16values, x => (ulong) x);
		Test(U16values, x => (long) x);

		Test(I16values, x => (byte) x);
		Test(I16values, x => (sbyte) x);
		Test(I16values, x => (ushort) x);
		Test(I16values, x => (short) x);
		Test(I16values, x => (uint) x);
		Test(I16values, x => (int) x);
		Test(I16values, x => (ulong) x);
		Test(I16values, x => (long) x);

		Test(U32values, x => (byte) x);
		Test(U32values, x => (sbyte) x);
		Test(U32values, x => (ushort) x);
		Test(U32values, x => (short) x);
		Test(U32values, x => (uint) x);
		Test(U32values, x => (int) x);
		Test(U32values, x => (ulong) x);
		Test(U32values, x => (long) x);

		Test(I32values, x => (byte) x);
		Test(I32values, x => (sbyte) x);
		Test(I32values, x => (ushort) x);
		Test(I32values, x => (short) x);
		Test(I32values, x => (uint) x);
		Test(I32values, x => (int) x);
		Test(I32values, x => (ulong) x);
		Test(I32values, x => (long) x);

		Test(U64values, x => (byte) x);
		Test(U64values, x => (sbyte) x);
		Test(U64values, x => (ushort) x);
		Test(U64values, x => (short) x);
		Test(U64values, x => (uint) x);
		Test(U64values, x => (int) x);
		Test(U64values, x => (ulong) x);
		Test(U64values, x => (long) x);

		Test(I64values, x => (byte) x);
		Test(I64values, x => (sbyte) x);
		Test(I64values, x => (ushort) x);
		Test(I64values, x => (short) x);
		Test(I64values, x => (uint) x);
		Test(I64values, x => (int) x);
		Test(I64values, x => (ulong) x);
		Test(I64values, x => (long) x);
	}

	[TestCaseSource(nameof(Jits32))]
	public void Shift(IJit<uint> jit) {
		void TestLeft<T>(T[] values, Func<T, T, T> knownGood) where T : struct {
			var jitFunc = jit.CreateFunction<Func<T, T, T>>("test", builder => builder.Return(builder.Argument<T>(0).LeftShift(builder.Argument<T>(1))));
			foreach(var a in values)
				foreach(var b in values)
					ClassicAssert.AreEqual(knownGood(a, b), jitFunc(a, b), $"{a} << {b}");
		}
		
		TestLeft(U8values, (a, b) => (byte) (a << (int) b));
		TestLeft(I8values, (a, b) => (sbyte) (a << (int) b));
		TestLeft(U16values, (a, b) => (ushort) (a << (int) b));
		TestLeft(I16values, (a, b) => (short) (a << (int) b));
		TestLeft(U32values, (a, b) => (uint) (a << (int) b));
		TestLeft(I32values, (a, b) => (int) (a << (int) b));
		TestLeft(U64values, (a, b) => (ulong) (a << (int) b));
		TestLeft(I64values, (a, b) => (long) (a << (int) b));
		
		void TestRight<T>(T[] values, Func<T, T, T> knownGood) where T : struct {
			var jitFunc = jit.CreateFunction<Func<T, T, T>>("test", builder => builder.Return(builder.Argument<T>(0).RightShift(builder.Argument<T>(1))));
			foreach(var a in values)
				foreach(var b in values)
					ClassicAssert.AreEqual(knownGood(a, b), jitFunc(a, b), $"{a} >> {b}");
		}
		
		TestRight(U8values, (a, b) => (byte) (a >> (int) b));
		TestRight(I8values, (a, b) => (sbyte) (a >> (int) b));
		TestRight(U16values, (a, b) => (ushort) (a >> (int) b));
		TestRight(I16values, (a, b) => (short) (a >> (int) b));
		TestRight(U32values, (a, b) => (uint) (a >> (int) b));
		TestRight(I32values, (a, b) => (int) (a >> (int) b));
		TestRight(U64values, (a, b) => (ulong) (a >> (int) b));
		TestRight(I64values, (a, b) => (long) (a >> (int) b));
	}

	[StructLayout(LayoutKind.Explicit)]
	public unsafe struct FooStruct : IJitStruct {
		[FieldOffset(0)] public uint Foo;
		[FieldOffset(4)] public uint Bar;
		[FieldOffset(8)] public uint Baz;
		[FieldOffset(12)] public uint Hax;
		[FieldOffset(16)] public fixed uint Arr[16];
	}

	delegate uint StructTest(ref FooStruct foo);
	
	[TestCaseSource(nameof(Jits32))]
	public void Structs(IJit<uint> jit) {
		var foo = new FooStruct { Foo = 1, Bar = 2, Baz = 3, Hax = 4 };
		for(var i = 0; i < 16; ++i)
			foo.Arr[i] = (uint) i;
		var func = jit.CreateFunction<StructTest>("test", builder => {
			var foor = builder.StructRefArgument<FooStruct>(0);
			foor.Foo = builder.LiteralValue(5U);
			foor.Bar = builder.LiteralValue(6U);
			foor.Baz = builder.LiteralValue(7U);
			foor.Arr[builder.LiteralValue(0)] = builder.LiteralValue(123U);
			foor.Arr[builder.LiteralValue(6)] = builder.LiteralValue(321U);
			builder.Return(foor.Hax);
		});
		ClassicAssert.AreEqual(4, func(ref foo));
		ClassicAssert.AreEqual(5, foo.Foo);
		ClassicAssert.AreEqual(6, foo.Bar);
		ClassicAssert.AreEqual(7, foo.Baz);
		for(var i = 0; i < 16; ++i)
			ClassicAssert.AreEqual(i switch {
				0 => 123U, 
				6 => 321U, 
				_ => i
			}, foo.Arr[i]);
	}

	[TestCaseSource(nameof(Jits32))]
	public void SwitchStmt(IJit<uint> jit) {
		var func = jit.CreateFunction<Func<uint, uint>>("func", builder => {
			builder.Switch(builder.Argument<uint>(0), 
				(builder.LiteralValue(0U), () => builder.Return(builder.LiteralValue(123U))),
				(builder.LiteralValue(1U), () => builder.Return(builder.LiteralValue(7U))) 
			);
			builder.Return(builder.Zero<uint>());
		});
		ClassicAssert.AreEqual(123, func(0));
		ClassicAssert.AreEqual(7, func(1));
		ClassicAssert.AreEqual(0, func(2));
		ClassicAssert.AreEqual(0, func(3));
		
		var funcwd = jit.CreateFunction<Func<uint, uint>>("func", builder => {
			builder.Switch(builder.Argument<uint>(0), 
				(builder.LiteralValue(0U), () => builder.Return(builder.LiteralValue(123U))),
				(builder.LiteralValue(1U), () => builder.Return(builder.LiteralValue(7U))), 
				(null, () => builder.Return(builder.Zero<uint>()))
			);
		});
		ClassicAssert.AreEqual(123, funcwd(0));
		ClassicAssert.AreEqual(7, funcwd(1));
		ClassicAssert.AreEqual(0, funcwd(2));
		ClassicAssert.AreEqual(0, funcwd(3));

		ClassicAssert.Catch(() =>
			jit.CreateFunction<Func<uint, uint>>("func", builder => {
				builder.Switch(builder.Argument<uint>(0),
					(builder.LiteralValue(0U), () => builder.Return(builder.LiteralValue(123U))),
					(null, () => builder.Return(builder.Zero<uint>())), 
					(builder.LiteralValue(1U), () => builder.Return(builder.LiteralValue(7U)))
				);
			}));
	}

	[TestCaseSource(nameof(Jits32))]
	public void SwitchExpr(IJit<uint> jit) {
		var func = jit.CreateFunction<Func<uint, uint>>("func", builder => {
			builder.Return(builder.Switch(builder.Argument<uint>(0), 
				(builder.LiteralValue(0U), () => builder.LiteralValue(123U)),
				(builder.LiteralValue(1U), () => builder.LiteralValue(7U)), 
				(null, () => builder.LiteralValue(0U))
			));
		});
		ClassicAssert.AreEqual(123, func(0));
		ClassicAssert.AreEqual(7, func(1));
		ClassicAssert.AreEqual(0, func(2));
		ClassicAssert.AreEqual(0, func(3));
	}
}