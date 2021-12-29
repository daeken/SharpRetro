namespace JitBase; 

public interface IRuntimeFactory<AddrT> where AddrT : struct {
	IRuntimeValue<T> Zero<T>() where T : struct;
	IRuntimeValue<T> LiteralValue<T>(T value) where T : struct;
	IRuntimePointer<AddrT, T> Pointer<T>(IRuntimeValue<AddrT> pointer) where T : struct;

	void Sink<T>(IRuntimeValue<T> value) where T : struct;

	void If(IRuntimeValue<bool> cond, Action if_, Action else_);
	void While(IRuntimeValue<bool> cond, Action body);
	IRuntimeValue<T> Ternary<T>(IRuntimeValue<bool> cond, IRuntimeValue<T> a, IRuntimeValue<T> b) where T : struct;

	void Call(Action func);
	void Call<T1>(Action<T1> func, IRuntimeValue<T1> a1) where T1 : struct;
	void Call<T1, T2>(Action<T1, T2> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2) where T1 : struct where T2 : struct;
	void Call<T1, T2, T3>(Action<T1, T2> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3) where T1 : struct where T2 : struct where T3 : struct;
	void Call<T1, T2, T3, T4>(Action<T1, T2> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3, IRuntimeValue<T4> a4) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
	
	IRuntimeValue<RetT> Call<RetT>(Func<RetT> func) where RetT : struct;
	IRuntimeValue<RetT> Call<T1, RetT>(Func<T1, RetT> func, IRuntimeValue<T1> a1) where RetT : struct where T1 : struct;
	IRuntimeValue<RetT> Call<T1, T2, RetT>(Func<T1, T2, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2) where RetT : struct where T1 : struct where T2 : struct;
	IRuntimeValue<RetT> Call<T1, T2, T3, RetT>(Func<T1, T2, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3) where RetT : struct where T1 : struct where T2 : struct where T3 : struct;
	IRuntimeValue<RetT> Call<T1, T2, T3, T4, RetT>(Func<T1, T2, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3, IRuntimeValue<T4> a4) where RetT : struct where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
}