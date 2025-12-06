using System.Runtime.Intrinsics;

namespace JitBase; 

public interface IBuilder<AddrT> where AddrT : struct {
	IRuntimeValue<T> Argument<T>(int index) where T : struct;
	IStructRef<T> StructRefArgument<T>(int index) where T : IJitStruct;

	IRuntimeValue<T> Zero<T>() where T : struct;
	IRuntimeValue<T> LiteralValue<T>(T value) where T : struct;
	IRuntimeValue<T> EnsureRuntime<T>(T value) where T : struct => LiteralValue(value);
	IRuntimeValue<T> EnsureRuntime<T>(IRuntimeValue<T> value) where T : struct => value;
	IRuntimePointer<AddrT, T> Pointer<T>(IRuntimeValue<AddrT> pointer) where T : struct;

	IRuntimeValue<Vector128<float>> CreateVector(
		IRuntimeValue<byte> _00, IRuntimeValue<byte> _01, IRuntimeValue<byte> _02, IRuntimeValue<byte> _03,
		IRuntimeValue<byte> _04, IRuntimeValue<byte> _05, IRuntimeValue<byte> _06, IRuntimeValue<byte> _07,
		IRuntimeValue<byte> _08, IRuntimeValue<byte> _09, IRuntimeValue<byte> _10, IRuntimeValue<byte> _11,
		IRuntimeValue<byte> _12, IRuntimeValue<byte> _13, IRuntimeValue<byte> _14, IRuntimeValue<byte> _15
	);
	IRuntimeValue<Vector128<float>> CreateVector(
		IRuntimeValue<ushort> _00, IRuntimeValue<ushort> _01, IRuntimeValue<ushort> _02, IRuntimeValue<ushort> _03,
		IRuntimeValue<ushort> _04, IRuntimeValue<ushort> _05, IRuntimeValue<ushort> _06, IRuntimeValue<ushort> _07
	);
	IRuntimeValue<Vector128<float>> CreateVector(
		IRuntimeValue<float> _00, IRuntimeValue<float> _01, IRuntimeValue<float> _02, IRuntimeValue<float> _03
	);
	IRuntimeValue<Vector128<float>> CreateVector(
		IRuntimeValue<double> _00, IRuntimeValue<double> _01
	);

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
	void CallVoid<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3, IRuntimeValue<T4> a4, IRuntimeValue<T5> a5) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct;
	void CallVoid<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3, IRuntimeValue<T4> a4, IRuntimeValue<T5> a5, IRuntimeValue<T6> a6) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct;
	
	IRuntimeValue<RetT> Call<RetT>(Func<RetT> func) where RetT : struct;
	IRuntimeValue<RetT> Call<T1, RetT>(Func<T1, RetT> func, IRuntimeValue<T1> a1) where RetT : struct where T1 : struct;
	IRuntimeValue<RetT> Call<T1, T2, RetT>(Func<T1, T2, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2) where RetT : struct where T1 : struct where T2 : struct;
	IRuntimeValue<RetT> Call<T1, T2, T3, RetT>(Func<T1, T2, T3, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3) where RetT : struct where T1 : struct where T2 : struct where T3 : struct;
	IRuntimeValue<RetT> Call<T1, T2, T3, T4, RetT>(Func<T1, T2, T3, T4, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3, IRuntimeValue<T4> a4) where RetT : struct where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
	IRuntimeValue<RetT> Call<T1, T2, T3, T4, T5, RetT>(Func<T1, T2, T3, T4, T5, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3, IRuntimeValue<T4> a4, IRuntimeValue<T5> a5) where RetT : struct where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct;
	IRuntimeValue<RetT> Call<T1, T2, T3, T4, T5, T6, RetT>(Func<T1, T2, T3, T4, T5, T6, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3, IRuntimeValue<T4> a4, IRuntimeValue<T5> a5, IRuntimeValue<T6> a6) where RetT : struct where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct;

}