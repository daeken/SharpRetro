using CilJit;

var jit = new CilJit<ulong>();

var add = jit.CreateFunction<Func<uint, uint, uint>>("add", b => {
	b.Return(b.Argument<uint>(0) + b.Argument<uint>(1));
});

Console.WriteLine($"5 + 6 == {add(5, 6)}");
