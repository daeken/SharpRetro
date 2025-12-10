using CilJit;
using LlvmJit;

//var jit = new CilJit<ulong>();
var jit = new LlvmJit<ulong>();

var test = jit.CreateFunction<Func<sbyte, sbyte, sbyte>>("test2", builder => {
	builder.DefineLocal<ulong>();
	builder.Return(builder.Argument<sbyte>(0).LeftShift(builder.Argument<sbyte>(1)));
});
Console.WriteLine(test(-1, -1));
