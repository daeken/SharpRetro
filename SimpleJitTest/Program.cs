using CilJit;
using LlvmJit;

//var jit = new CilJit<ulong>();
var jit = new LlvmJit<ulong>();

var ternaryTest = jit.CreateFunction<Func<bool, uint, uint, uint>>("ternaryTest", builder =>
	builder.Return(builder.Ternary(builder.Argument<bool>(0), builder.Argument<uint>(1), builder.Argument<uint>(2))));

Console.WriteLine(ternaryTest(true, 1, 2));
Console.WriteLine(ternaryTest(false, 1, 2));
