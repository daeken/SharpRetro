using System.Runtime.InteropServices;
using JitBase;
using LLVMSharp.Interop;
using static JitBase.Helpers;
using static LlvmJit.LlvmExtensions;

namespace LlvmJit; 

public unsafe class LlvmRuntimeValue<T> : IRuntimeValue<T> where T : struct {
	readonly Func<LLVMValueRef> Generate;
	readonly LLVMOpaqueBuilder* Builder;

	internal LlvmRuntimeValue(LLVMOpaqueBuilder* builder, Func<LLVMValueRef> gen) {
		Builder = builder;
		Generate = gen;
	}

	internal LLVMValueRef Emit() => Generate();
	internal static LLVMValueRef Emit<U>(IRuntimeValue<U> rv) where U : struct => ((LlvmRuntimeValue<U>) rv).Emit();
	IRuntimeValue<U> C<U>(Func<LLVMValueRef> gen) where U : struct => new LlvmRuntimeValue<U>(Builder, gen);

	public IRuntimeValue<OT> Cast<OT>() where OT : struct => throw new NotImplementedException();
	public IRuntimeValue<OT> Bitcast<OT>() where OT : struct => throw new NotImplementedException();
	public IRuntimeValue<T> Store() {
		var val = Emit();
		return C<T>(() => val);
	}

	public IRuntimeValue<T> Add(IRuntimeValue<T> rhs) => C<T>(() => LLVM.BuildAdd(Builder, Emit(), Emit(rhs), EmptyString));
	public IRuntimeValue<T> Sub(IRuntimeValue<T> rhs) => C<T>(() => LLVM.BuildSub(Builder, Emit(), Emit(rhs), EmptyString));
	public IRuntimeValue<T> Mul(IRuntimeValue<T> rhs) => C<T>(() => LLVM.BuildMul(Builder, Emit(), Emit(rhs), EmptyString));
	public IRuntimeValue<T> Div(IRuntimeValue<T> rhs) => C<T>(() => IsSigned<T>()
		? LLVM.BuildSDiv(Builder, Emit(), Emit(rhs), EmptyString)
		: LLVM.BuildUDiv(Builder, Emit(), Emit(rhs), EmptyString));
	public IRuntimeValue<T> Mod(IRuntimeValue<T> rhs) => C<T>(() => IsSigned<T>()
		? LLVM.BuildSRem(Builder, Emit(), Emit(rhs), EmptyString)
		: LLVM.BuildURem(Builder, Emit(), Emit(rhs), EmptyString));
	public IRuntimeValue<T> Negate() => throw new NotImplementedException();
	public IRuntimeValue<T> And(IRuntimeValue<T> rhs) => C<T>(() => LLVM.BuildAnd(Builder, Emit(), Emit(rhs), EmptyString));
	public IRuntimeValue<T> Or(IRuntimeValue<T> rhs) => C<T>(() => LLVM.BuildOr(Builder, Emit(), Emit(rhs), EmptyString));
	public IRuntimeValue<T> Xor(IRuntimeValue<T> rhs) => C<T>(() => LLVM.BuildXor(Builder, Emit(), Emit(rhs), EmptyString));
	public IRuntimeValue<T> LeftShift(IRuntimeValue<T> rhs) => throw new NotImplementedException();
	public IRuntimeValue<T> RightShift(IRuntimeValue<T> rhs) => throw new NotImplementedException();
	public IRuntimeValue<T> Not() => throw new NotImplementedException();
	public IRuntimeValue<bool> LT(IRuntimeValue<T> rhs) => C<bool>(() => LLVM.BuildICmp(Builder, IsSigned<T>() ? LLVMIntPredicate.LLVMIntSLT : LLVMIntPredicate.LLVMIntULT, Emit(), Emit(rhs), EmptyString));
	public IRuntimeValue<bool> LTE(IRuntimeValue<T> rhs) => C<bool>(() => LLVM.BuildICmp(Builder, IsSigned<T>() ? LLVMIntPredicate.LLVMIntSLE : LLVMIntPredicate.LLVMIntULE, Emit(), Emit(rhs), EmptyString));
	public IRuntimeValue<bool> EQ(IRuntimeValue<T> rhs) => C<bool>(() => LLVM.BuildICmp(Builder, LLVMIntPredicate.LLVMIntEQ, Emit(), Emit(rhs), EmptyString));
	public IRuntimeValue<bool> NE(IRuntimeValue<T> rhs) => C<bool>(() => LLVM.BuildICmp(Builder, LLVMIntPredicate.LLVMIntNE, Emit(), Emit(rhs), EmptyString));
	public IRuntimeValue<bool> GTE(IRuntimeValue<T> rhs) => C<bool>(() => LLVM.BuildICmp(Builder, IsSigned<T>() ? LLVMIntPredicate.LLVMIntSGE : LLVMIntPredicate.LLVMIntUGE, Emit(), Emit(rhs), EmptyString));
	public IRuntimeValue<bool> GT(IRuntimeValue<T> rhs) => C<bool>(() => LLVM.BuildICmp(Builder, IsSigned<T>() ? LLVMIntPredicate.LLVMIntSGT : LLVMIntPredicate.LLVMIntUGT, Emit(), Emit(rhs), EmptyString));
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