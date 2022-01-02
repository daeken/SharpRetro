using CilJit;
using LlvmJit;

//var jit = new CilJit<ulong>();
var jit = new LlvmJit<ulong>();

void Test() {
	Console.WriteLine("Called!");
}

var test = jit.CreateFunction<Action>("test", builder => {
	builder.Call(Test);
});
test();

void Test2(int foo) {
	Console.WriteLine($"Called with {foo}");
}

var test2 = jit.CreateFunction<Action>("test2", builder => {
	builder.Call(Test2, builder.LiteralValue(123));
});
test2();
