using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Intrinsics;
using JitBase;
using Sigil;

namespace CilJit; 

public class CilBuilder<AddrT, DelegateT> : IBuilder<AddrT> where AddrT : struct where DelegateT : Delegate {
	readonly Emit<DelegateT> Ilg;
	readonly TypeBuilder Tb;

	internal CilBuilder(Emit<DelegateT> ilg, TypeBuilder tb) {
		Ilg = ilg;
		Tb = tb;
	}
	
	internal CilRuntimeValue<U, DelegateT> TT<U>(IRuntimeValue<U> v) where U : struct => v as CilRuntimeValue<U, DelegateT>;
	
	internal CilRuntimeValue<U, DelegateT> C<U>(Action gen) where U : struct => new(Ilg, Tb, gen);

	public IRuntimeValue<T> Argument<T>(int index) where T : struct => C<T>(() => Ilg.LoadArgument((ushort) index));
	
	public IStructRef<T> StructRefArgument<T>(int index) where T : IJitStruct => new CilStructRef<AddrT, DelegateT, T>(this, Ilg, C<ulong>(() => {
		Ilg.LoadArgument((ushort) index);
		Ilg.Convert<ulong>();
	}));
	
	public IRuntimeValue<T> Zero<T>() where T : struct => LiteralValue(default(T));
	public IRuntimeValue<T> LiteralValue<T>(T value) where T : struct => C<T>(value switch {
		byte v => () => Ilg.LoadConstant(v), 
		ushort v => () => Ilg.LoadConstant(v), 
		uint v => () => Ilg.LoadConstant(v), 
		ulong v => () => Ilg.LoadConstant(v), 
		sbyte v => () => Ilg.LoadConstant(v), 
		short v => () => Ilg.LoadConstant(v), 
		int v => () => Ilg.LoadConstant(v), 
		long v => () => Ilg.LoadConstant(v), 
		float v => () => Ilg.LoadConstant(v), 
		double v => () => Ilg.LoadConstant(v), 
		_ => throw new NotImplementedException($"Unknown literal value type: {typeof(T).FullName}")
	});
	public IRuntimePointer<AddrT, T> Pointer<T>(IRuntimeValue<AddrT> pointer) where T : struct => throw new NotImplementedException();

	public IRuntimeValue<Vector128<float>> CreateVector(IRuntimeValue<byte> _00, IRuntimeValue<byte> _01, IRuntimeValue<byte> _02, IRuntimeValue<byte> _03,
		IRuntimeValue<byte> _04, IRuntimeValue<byte> _05, IRuntimeValue<byte> _06, IRuntimeValue<byte> _07, IRuntimeValue<byte> _08, IRuntimeValue<byte> _09,
		IRuntimeValue<byte> _10, IRuntimeValue<byte> _11, IRuntimeValue<byte> _12, IRuntimeValue<byte> _13, IRuntimeValue<byte> _14, IRuntimeValue<byte> _15) =>
		throw new NotImplementedException();

	public IRuntimeValue<Vector128<float>> CreateVector(IRuntimeValue<ushort> _00, IRuntimeValue<ushort> _01, IRuntimeValue<ushort> _02, IRuntimeValue<ushort> _03,
		IRuntimeValue<ushort> _04, IRuntimeValue<ushort> _05, IRuntimeValue<ushort> _06, IRuntimeValue<ushort> _07) =>
		throw new NotImplementedException();

	public IRuntimeValue<Vector128<float>> CreateVector(IRuntimeValue<uint> _00, IRuntimeValue<uint> _01, IRuntimeValue<uint> _02, IRuntimeValue<uint> _03) => throw new NotImplementedException();

	public IRuntimeValue<Vector128<float>> CreateVector(IRuntimeValue<float> _00, IRuntimeValue<float> _01, IRuntimeValue<float> _02, IRuntimeValue<float> _03) => throw new NotImplementedException();

	public IRuntimeValue<Vector128<float>> CreateVector(IRuntimeValue<double> _00, IRuntimeValue<double> _01) => throw new NotImplementedException();

	public ILocalVar<T> DefineLocal<T>() where T : struct => new CilLocalVar<T, DelegateT>(Ilg, Tb);

	internal void Emit<T>(IRuntimeValue<T> value) where T : struct => TT(value).Emit();
	
	public void Sink<T>(IRuntimeValue<T> value) where T : struct {
		Emit(value);
		Ilg.Pop();
	}
	public void Return<T>(IRuntimeValue<T> value) where T : struct {
		Emit(value);
		Ilg.Return();
	}
	public void If(IRuntimeValue<bool> cond, Action if_, Action else_) {
		var elseLabel = Ilg.DefineLabel();
		var endLabel = Ilg.DefineLabel();
		Emit(cond);
		Ilg.BranchIfFalse(elseLabel);
		if_();
		try {
			Ilg.Branch(endLabel);
		} catch(SigilVerificationException) {
		}
		Ilg.MarkLabel(elseLabel);
		else_();
		Ilg.MarkLabel(endLabel);
	}
	public void When(IRuntimeValue<bool> cond, Action when) {
		var endLabel = Ilg.DefineLabel();
		Emit(cond);
		Ilg.BranchIfFalse(endLabel);
		when();
		try {
			Ilg.Branch(endLabel);
		} catch(SigilVerificationException) {
		}
		Ilg.MarkLabel(endLabel);
	}
	public void Unless(IRuntimeValue<bool> cond, Action unless) {
		var endLabel = Ilg.DefineLabel();
		Emit(cond);
		Ilg.BranchIfTrue(endLabel);
		unless();
		try {
			Ilg.Branch(endLabel);
		} catch(SigilVerificationException) {
		}
		Ilg.MarkLabel(endLabel);
	}
	public void While(IRuntimeValue<bool> cond, Action body) {
		var start = Ilg.DefineLabel();
		var end = Ilg.DefineLabel();
		Ilg.MarkLabel(start);
		Emit(cond);
		Ilg.BranchIfFalse(end);
		body();
		Ilg.Branch(start);
		Ilg.MarkLabel(end);
	}
	public void DoWhile(Action body, IRuntimeValue<bool> cond) {
		var start = Ilg.DefineLabel();
		Ilg.MarkLabel(start);
		body();
		Emit(cond);
		Ilg.BranchIfTrue(start);
	}
	public IRuntimeValue<T> Ternary<T>(IRuntimeValue<bool> cond, IRuntimeValue<T> a, IRuntimeValue<T> b) where T : struct => C<T>(() => If(cond, () => Emit(a), () => Emit(b)));

	public readonly Dictionary<object, FieldBuilder> Fields = new();
	void StoreValue<T>(T value) {
		if(!Fields.TryGetValue(value, out var fld)) {
			var name = $"Field{Fields.Count}";
			fld = Fields[value] = Tb.DefineField(name, typeof(T), FieldAttributes.Public | FieldAttributes.Static);
		}
		Ilg.LoadField(fld);
	}
	
	public void CallVoid(Action func) {
		StoreValue(func);
		Ilg.CallVirtual(typeof(Action).GetMethod("Invoke"));
	}
	public void CallVoid<T1>(Action<T1> func, IRuntimeValue<T1> a1) where T1 : struct {
		StoreValue(func);
		Emit(a1);
		Ilg.CallVirtual(typeof(Action<T1>).GetMethod("Invoke"));
	}
	public void CallVoid<T1, T2>(Action<T1, T2> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2) where T1 : struct where T2 : struct {
		StoreValue(func);
		Emit(a1);
		Emit(a2);
		Ilg.CallVirtual(typeof(Action<T1, T2>).GetMethod("Invoke"));
	}
	public void CallVoid<T1, T2, T3>(Action<T1, T2, T3> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3) where T1 : struct where T2 : struct where T3 : struct {
		StoreValue(func);
		Emit(a1);
		Emit(a2);
		Emit(a3);
		Ilg.CallVirtual(typeof(Action<T1, T2, T3>).GetMethod("Invoke"));
	}
	public void CallVoid<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3, IRuntimeValue<T4> a4) where T1 : struct where T2 : struct where T3 : struct where T4 : struct {
		StoreValue(func);
		Emit(a1);
		Emit(a2);
		Emit(a3);
		Emit(a4);
		Ilg.CallVirtual(typeof(Action<T1, T2, T3, T4>).GetMethod("Invoke"));
	}
	public void CallVoid<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3, IRuntimeValue<T4> a4, IRuntimeValue<T5> a5) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct {
		StoreValue(func);
		Emit(a1);
		Emit(a2);
		Emit(a3);
		Emit(a4);
		Emit(a5);
		Ilg.CallVirtual(typeof(Action<T1, T2, T3, T4, T5>).GetMethod("Invoke"));
	}
	public void CallVoid<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3, IRuntimeValue<T4> a4, IRuntimeValue<T5> a5, IRuntimeValue<T6> a6) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct {
		StoreValue(func);
		Emit(a1);
		Emit(a2);
		Emit(a3);
		Emit(a4);
		Emit(a5);
		Emit(a6);
		Ilg.CallVirtual(typeof(Action<T1, T2, T3, T4, T5, T6>).GetMethod("Invoke"));
	}
	public IRuntimeValue<RetT> Call<RetT>(Func<RetT> func) where RetT : struct =>
		C<RetT>(() => {
			StoreValue(func);
			Ilg.CallVirtual(typeof(Func<RetT>).GetMethod("Invoke"));
		});
	public IRuntimeValue<RetT> Call<T1, RetT>(Func<T1, RetT> func, IRuntimeValue<T1> a1) where T1 : struct where RetT : struct =>
		C<RetT>(() => {
			StoreValue(func);
			Emit(a1);
			Ilg.CallVirtual(typeof(Func<T1, RetT>).GetMethod("Invoke"));
		});
	public IRuntimeValue<RetT> Call<T1, T2, RetT>(Func<T1, T2, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2) where T1 : struct where T2 : struct where RetT : struct =>
		C<RetT>(() => {
			StoreValue(func);
			Emit(a1);
			Emit(a2);
			Ilg.CallVirtual(typeof(Func<T1, T2, RetT>).GetMethod("Invoke"));
		});
	public IRuntimeValue<RetT> Call<T1, T2, T3, RetT>(Func<T1, T2, T3, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3) where T1 : struct where T2 : struct where T3 : struct where RetT : struct =>
		C<RetT>(() => {
			StoreValue(func);
			Emit(a1);
			Emit(a2);
			Emit(a3);
			Ilg.CallVirtual(typeof(Func<T1, T2, T3, RetT>).GetMethod("Invoke"));
		});
	public IRuntimeValue<RetT> Call<T1, T2, T3, T4, RetT>(Func<T1, T2, T3, T4, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3, IRuntimeValue<T4> a4) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where RetT : struct =>
		C<RetT>(() => {
			StoreValue(func);
			Emit(a1);
			Emit(a2);
			Emit(a3);
			Emit(a4);
			Ilg.CallVirtual(typeof(Func<T1, T2, T3, T4, RetT>).GetMethod("Invoke"));
		});
	public IRuntimeValue<RetT> Call<T1, T2, T3, T4, T5, RetT>(Func<T1, T2, T3, T4, T5, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3, IRuntimeValue<T4> a4, IRuntimeValue<T5> a5) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where RetT : struct =>
		C<RetT>(() => {
			StoreValue(func);
			Emit(a1);
			Emit(a2);
			Emit(a3);
			Emit(a4);
			Emit(a5);
			Ilg.CallVirtual(typeof(Func<T1, T2, T3, T4, T5, RetT>).GetMethod("Invoke"));
		});
	public IRuntimeValue<RetT> Call<T1, T2, T3, T4, T5, T6, RetT>(Func<T1, T2, T3, T4, T5, T6, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3, IRuntimeValue<T4> a4, IRuntimeValue<T5> a5, IRuntimeValue<T6> a6) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct where RetT : struct =>
		C<RetT>(() => {
			StoreValue(func);
			Emit(a1);
			Emit(a2);
			Emit(a3);
			Emit(a4);
			Emit(a5);
			Emit(a6);
			Ilg.CallVirtual(typeof(Func<T1, T2, T3, T4, T5, T6, RetT>).GetMethod("Invoke"));
		});
}