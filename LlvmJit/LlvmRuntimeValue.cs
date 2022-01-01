using JitBase;
using LLVMSharp.Interop;

namespace LlvmJit; 

public unsafe class LlvmRuntimeValue<T> : IRuntimeValue<T> where T : struct {
	readonly Func<LLVMValueRef> Generate;
	readonly LLVMOpaqueBuilder* Builder;

	internal LlvmRuntimeValue(LLVMOpaqueBuilder* builder, Func<LLVMValueRef> gen) {
		Builder = builder;
		Generate = gen;
	}

	internal LLVMValueRef Emit() => Generate();

	public IRuntimeValue<OT> Cast<OT>() where OT : struct => throw new NotImplementedException();
	public IRuntimeValue<OT> Bitcast<OT>() where OT : struct => throw new NotImplementedException();
	public IRuntimeValue<T> Store() => throw new NotImplementedException();
	public IRuntimeValue<T> Add(IRuntimeValue<T> rhs) => throw new NotImplementedException();
	public IRuntimeValue<T> Sub(IRuntimeValue<T> rhs) => throw new NotImplementedException();
	public IRuntimeValue<T> Mul(IRuntimeValue<T> rhs) => throw new NotImplementedException();
	public IRuntimeValue<T> Div(IRuntimeValue<T> rhs) => throw new NotImplementedException();
	public IRuntimeValue<T> Mod(IRuntimeValue<T> rhs) => throw new NotImplementedException();
	public IRuntimeValue<T> Negate() => throw new NotImplementedException();
	public IRuntimeValue<T> And(IRuntimeValue<T> rhs) => throw new NotImplementedException();
	public IRuntimeValue<T> Or(IRuntimeValue<T> rhs) => throw new NotImplementedException();
	public IRuntimeValue<T> Xor(IRuntimeValue<T> rhs) => throw new NotImplementedException();
	public IRuntimeValue<T> LeftShift(IRuntimeValue<T> rhs) => throw new NotImplementedException();
	public IRuntimeValue<T> RightShift(IRuntimeValue<T> rhs) => throw new NotImplementedException();
	public IRuntimeValue<T> Not() => throw new NotImplementedException();
	public IRuntimeValue<bool> LT(IRuntimeValue<T> rhs) => throw new NotImplementedException();
	public IRuntimeValue<bool> LTE(IRuntimeValue<T> rhs) => throw new NotImplementedException();
	public IRuntimeValue<bool> EQ(IRuntimeValue<T> rhs) => throw new NotImplementedException();
	public IRuntimeValue<bool> NE(IRuntimeValue<T> rhs) => throw new NotImplementedException();
	public IRuntimeValue<bool> GTE(IRuntimeValue<T> rhs) => throw new NotImplementedException();
	public IRuntimeValue<bool> GT(IRuntimeValue<T> rhs) => throw new NotImplementedException();
	public IRuntimeValue<T> Abs() => throw new NotImplementedException();
	public IRuntimeValue<T> Sqrt() => throw new NotImplementedException();
	public IRuntimeValue<T> Round() => throw new NotImplementedException();
	public IRuntimeValue<T> RoundHalfDown() => throw new NotImplementedException();
	public IRuntimeValue<T> RoundHalfUp() => throw new NotImplementedException();
	public IRuntimeValue<T> Ceil() => throw new NotImplementedException();
	public IRuntimeValue<T> Floor() => throw new NotImplementedException();
	public IRuntimeValue<bool> IsNaN() => throw new NotImplementedException();
	public IRuntimeValue<ElementT> Element<ElementT>(IRuntimeValue<int> index) where ElementT : struct => throw new NotImplementedException();
	public IRuntimeValue<T> Element<ElementT>(IRuntimeValue<int> index, IRuntimeValue<ElementT> value) where ElementT : struct => throw new NotImplementedException();
}