namespace JitBase; 

public interface IBuilder<AddrT> where AddrT : struct {
	IRuntimeValue<T> Argument<T>(int index) where T : struct;
	IStructRef<T> StructRefArgument<T>(int index) where T : IJitStruct;

	IRuntimeValue<T> Zero<T>() where T : struct;
	IRuntimeValue<T> LiteralValue<T>(T value) where T : struct;
	IRuntimeValue<T> EnsureRuntime<T>(T value) where T : struct => LiteralValue(value);
	IRuntimeValue<T> EnsureRuntime<T>(IRuntimeValue<T> value) where T : struct => value;
	IRuntimePointer<AddrT, T> Pointer<T>(IRuntimeValue<AddrT> pointer) where T : struct;

	ILocalVar<T> DefineLocal<T>() where T : struct;

	void Sink<T>(IRuntimeValue<T> value) where T : struct;
	void Return<T>(IRuntimeValue<T> value) where T : struct;

	void If(IRuntimeValue<bool> cond, Action if_, Action else_);
	void When(IRuntimeValue<bool> cond, Action when);
	void Unless(IRuntimeValue<bool> cond, Action unless);
	void While(IRuntimeValue<bool> cond, Action body);
	void DoWhile(Action body, IRuntimeValue<bool> cond);
	IRuntimeValue<T> Ternary<T>(IRuntimeValue<bool> cond, IRuntimeValue<T> a, IRuntimeValue<T> b) where T : struct;
	public void Switch<T>(IRuntimeValue<T> matchee, params (IRuntimeValue<T> Match, Action Body)[] matchers) where T : struct {
		matchee = matchee.Store();
		void Recur(Queue<(IRuntimeValue<T> Match, Action Body)> elems) {
			if(elems.Count == 0) return;
			var (match, body) = elems.Dequeue();
			if(match == (object) null) {
				if(elems.Count != 0) throw new();
				body();
			} else
				If(matchee == match, 
					body,
					() => Recur(elems));
		}
		Recur(new(matchers));
	}
	IRuntimeValue<U> Switch<T, U>(IRuntimeValue<T> matchee, params (IRuntimeValue<T> Match, Func<IRuntimeValue<U>> Body)[] matchers) where T : struct where U : struct {
		matchee = matchee.Store();
		IRuntimeValue<U> Recur(Queue<(IRuntimeValue<T> Match, Func<IRuntimeValue<U>> Body)> elems) {
			var (match, body) = elems.Dequeue();
			if(match == (object) null) {
				if(elems.Count != 0) throw new();
				return body();
			}
			return Ternary(matchee == match, 
				body(),
				Recur(elems));
		}
		return Recur(new(matchers));
	}

	void CallVoid(Action func);
	void CallVoid<T1>(Action<T1> func, IRuntimeValue<T1> a1) where T1 : struct;
	void CallVoid<T1, T2>(Action<T1, T2> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2) where T1 : struct where T2 : struct;
	void CallVoid<T1, T2, T3>(Action<T1, T2, T3> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3) where T1 : struct where T2 : struct where T3 : struct;
	void CallVoid<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3, IRuntimeValue<T4> a4) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
	
	IRuntimeValue<RetT> Call<RetT>(Func<RetT> func) where RetT : struct;
	IRuntimeValue<RetT> Call<T1, RetT>(Func<T1, RetT> func, IRuntimeValue<T1> a1) where RetT : struct where T1 : struct;
	IRuntimeValue<RetT> Call<T1, T2, RetT>(Func<T1, T2, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2) where RetT : struct where T1 : struct where T2 : struct;
	IRuntimeValue<RetT> Call<T1, T2, T3, RetT>(Func<T1, T2, T3, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3) where RetT : struct where T1 : struct where T2 : struct where T3 : struct;
	IRuntimeValue<RetT> Call<T1, T2, T3, T4, RetT>(Func<T1, T2, T3, T4, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3, IRuntimeValue<T4> a4) where RetT : struct where T1 : struct where T2 : struct where T3 : struct where T4 : struct;

}