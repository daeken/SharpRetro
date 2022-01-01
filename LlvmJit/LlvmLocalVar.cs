using JitBase;
using LLVMSharp.Interop;
using static LlvmJit.LlvmExtensions;

namespace LlvmJit; 

public unsafe class LlvmLocalVar<T> : ILocalVar<T> where T : struct {
	readonly LLVMOpaqueBuilder* Builder;
	readonly LLVMValueRef Pointer;
	readonly LlvmRuntimeValue<T> Getter;

	internal LlvmLocalVar(LLVMOpaqueBuilder* builder) {
		Builder = builder;
		Pointer = LLVM.BuildAlloca(Builder, LlvmType<T>(), EmptyString);
		Getter = new LlvmRuntimeValue<T>(Builder, () => LLVM.BuildLoad(Builder, Pointer, EmptyString));
	}

	public IRuntimeValue<T> Value { get => Getter; set => LLVM.BuildStore(Builder, LlvmRuntimeValue<T>.Emit(value), Pointer); }
}