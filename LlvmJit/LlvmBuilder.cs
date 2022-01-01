using JitBase;
using LLVMSharp;
using LLVMSharp.Interop;

namespace LlvmJit; 

public unsafe class LlvmBuilder<AddrT> : IBuilder<AddrT> where AddrT : struct {
	readonly LLVMValueRef Function;
	readonly LLVMOpaqueBuilder* Builder;

	internal bool ReturnedThisBlock;
	
	internal LlvmBuilder(LLVMValueRef function, LLVMOpaqueBuilder* builder) {
		Function = function;
		Builder = builder;
	}

	IRuntimeValue<T> C<T>(Func<LLVMValueRef> gen) where T : struct => new LlvmRuntimeValue<T>(Builder, gen);
	static LLVMValueRef Emit<T>(IRuntimeValue<T> rv) where T : struct => ((LlvmRuntimeValue<T>) rv).Emit();

	public IRuntimeValue<T> Argument<T>(int index) where T : struct => C<T>(() => LLVM.GetParam(Function, (uint) index));
	public IRuntimeValue<T> Zero<T>() where T : struct => throw new NotImplementedException();
	public IRuntimeValue<T> LiteralValue<T>(T value) where T : struct => throw new NotImplementedException();
	public IRuntimePointer<AddrT, T> Pointer<T>(IRuntimeValue<AddrT> pointer) where T : struct => throw new NotImplementedException();
	public ILocalVar<T> DefineLocal<T>() where T : struct => throw new NotImplementedException();
	public void Sink<T>(IRuntimeValue<T> value) where T : struct {
		throw new NotImplementedException();
	}
	public void Return<T>(IRuntimeValue<T> value) where T : struct {
		LLVM.BuildRet(Builder, Emit(value));
		ReturnedThisBlock = true;
	}
	public void If(IRuntimeValue<bool> cond, Action if_, Action else_) {
		throw new NotImplementedException();
	}
	public void While(IRuntimeValue<bool> cond, Action body) {
		throw new NotImplementedException();
	}
	public void DoWhile(Action body, IRuntimeValue<bool> cond) {
		throw new NotImplementedException();
	}
	public IRuntimeValue<T> Ternary<T>(IRuntimeValue<bool> cond, IRuntimeValue<T> a, IRuntimeValue<T> b) where T : struct => throw new NotImplementedException();
	public void Call(Action func) {
		throw new NotImplementedException();
	}
	public void Call<T1>(Action<T1> func, IRuntimeValue<T1> a1) where T1 : struct {
		throw new NotImplementedException();
	}
	public void Call<T1, T2>(Action<T1, T2> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2) where T1 : struct where T2 : struct {
		throw new NotImplementedException();
	}
	public void Call<T1, T2, T3>(Action<T1, T2, T3> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3) where T1 : struct where T2 : struct where T3 : struct {
		throw new NotImplementedException();
	}
	public void Call<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3, IRuntimeValue<T4> a4) where T1 : struct where T2 : struct where T3 : struct where T4 : struct {
		throw new NotImplementedException();
	}
	public IRuntimeValue<RetT> Call<RetT>(Func<RetT> func) where RetT : struct => throw new NotImplementedException();
	public IRuntimeValue<RetT> Call<T1, RetT>(Func<T1, RetT> func, IRuntimeValue<T1> a1) where T1 : struct where RetT : struct => throw new NotImplementedException();
	public IRuntimeValue<RetT> Call<T1, T2, RetT>(Func<T1, T2, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2) where T1 : struct where T2 : struct where RetT : struct => throw new NotImplementedException();
	public IRuntimeValue<RetT> Call<T1, T2, T3, RetT>(Func<T1, T2, T3, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3) where T1 : struct where T2 : struct where T3 : struct where RetT : struct => throw new NotImplementedException();
	public IRuntimeValue<RetT> Call<T1, T2, T3, T4, RetT>(Func<T1, T2, T3, T4, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3, IRuntimeValue<T4> a4) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where RetT : struct => throw new NotImplementedException();
}