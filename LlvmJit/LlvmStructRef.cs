using System.Runtime.InteropServices;
using JitBase;
using LLVMSharp.Interop;
using static LlvmJit.LlvmExtensions;

namespace LlvmJit; 

public unsafe class LlvmStructRef<AddrT, T> : IStructRef<T> where AddrT : struct where T : IJitStruct {
	readonly LlvmBuilder<AddrT> JBuilder;
	LLVMBuilderRef Builder;
	readonly LLVMValueRef Pointer;

	internal LlvmStructRef(LlvmBuilder<AddrT> jBuilder, LLVMBuilderRef builder, LLVMValueRef ptr) {
		JBuilder = jBuilder;
		Builder = builder;
		Pointer = ptr;
	}

	public override IRuntimeValue<U> GetField<U>(ulong offset) => JBuilder.C<U>(() => {
		var naddr = LLVM.BuildAdd(Builder, Pointer, LLVMValueRef.CreateConstInt(LlvmType<ulong>(), offset), EmptyString);
		var ptr = LLVM.BuildIntToPtr(Builder, naddr, LLVM.PointerType(LlvmType<U>(), 0), EmptyString);
		return LLVM.BuildLoad2(Builder, LlvmType<U>(), ptr, EmptyString);
	});
	public override void SetField<U>(ulong offset, IRuntimeValue<U> value) {
		var naddr = LLVM.BuildAdd(Builder, Pointer, LLVMValueRef.CreateConstInt(LlvmType<ulong>(), offset), EmptyString);
		var ptr = LLVM.BuildIntToPtr(Builder, naddr, LLVM.PointerType(LlvmType<U>(), 0), EmptyString);
		LLVM.BuildStore(Builder, JBuilder.Emit(value), ptr);
	}
	public override IRuntimeValue<U> GetFieldElement<U>(ulong offset, IRuntimeValue<int> index) => JBuilder.C<U>(() => {
		var naddr = LLVM.BuildAdd(Builder, Pointer, LLVMValueRef.CreateConstInt(LlvmType<ulong>(), offset), EmptyString);
		naddr = LLVM.BuildAdd(Builder, naddr, JBuilder.Emit((IRuntimeValue<ulong>) index * JBuilder.LiteralValue((ulong) Marshal.SizeOf<U>())), EmptyString);
		var ptr = LLVM.BuildIntToPtr(Builder, naddr, LLVM.PointerType(LlvmType<U>(), 0), EmptyString);
		return LLVM.BuildLoad2(Builder, LlvmType<U>(), ptr, EmptyString);
	});
	public override void SetFieldElement<U>(ulong offset, IRuntimeValue<int> index, IRuntimeValue<U> value) {
		var naddr = LLVM.BuildAdd(Builder, Pointer, LLVMValueRef.CreateConstInt(LlvmType<ulong>(), offset), EmptyString);
		naddr = LLVM.BuildAdd(Builder, naddr, JBuilder.Emit((IRuntimeValue<ulong>) index * JBuilder.LiteralValue((ulong) Marshal.SizeOf<U>())), EmptyString);
		var ptr = LLVM.BuildIntToPtr(Builder, naddr, LLVM.PointerType(LlvmType<U>(), 0), EmptyString);
		LLVM.BuildStore(Builder, JBuilder.Emit(value), ptr);
	}
}