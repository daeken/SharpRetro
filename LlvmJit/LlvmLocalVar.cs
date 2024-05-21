using JitBase;
using LLVMSharp.Interop;
using static LlvmJit.LlvmExtensions;

namespace LlvmJit; 

public unsafe class LlvmLocalVar<AddrT, T> : ILocalVar<T> where AddrT : struct where T : struct {
	readonly LLVMBuilderRef Builder;
	readonly LLVMValueRef Pointer;
	readonly LlvmRuntimeValue<AddrT, T> Getter;

	internal LlvmLocalVar(LLVMBuilderRef builder, LlvmBuilder<AddrT> jBuilder) {
		Builder = builder;
		Pointer = LLVM.BuildAlloca(Builder, LlvmType<T>(), EmptyString);
		Getter = new(Builder, jBuilder, () => LLVM.BuildLoad2(Builder, LlvmType<T>(), Pointer, EmptyString));
	}

	public IRuntimeValue<T> Value { get => Getter; set => LLVM.BuildStore(Builder, LlvmRuntimeValue<AddrT, T>.Emit(value), Pointer); }
}