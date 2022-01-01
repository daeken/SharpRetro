using System.Runtime.InteropServices;
using JitBase;
using LLVMSharp.Interop;

namespace LlvmJit;

public unsafe class LlvmJit<AddrT> : IJit<AddrT> where AddrT : struct {
	static LlvmJit() {
		LLVM.LinkInMCJIT();
		LLVM.InitializeX86TargetInfo();
		LLVM.InitializeX86Target();
		LLVM.InitializeX86TargetMC();
		LLVM.InitializeX86AsmParser();
		LLVM.InitializeX86AsmPrinter();

		var options = new LLVMMCJITCompilerOptions { NoFramePointerElim = 1 };
		LLVM.InitializeMCJITCompilerOptions(&options, (UIntPtr) Marshal.SizeOf<LLVMMCJITCompilerOptions>());
	}
	
	static LLVMTypeRef LlvmType<T>() => typeof(T).ToLLVMType();

	public DelegateT CreateFunction<DelegateT>(string name, Action<IBuilder<AddrT>> body) where DelegateT : Delegate {
		var module = LLVMModuleRef.CreateWithName("SupercellNXLLVM");
		var builder = LLVM.CreateBuilder();

		var passManager = LLVM.CreateFunctionPassManagerForModule(module);
		LLVM.AddInstructionCombiningPass(passManager);
		LLVM.AddReassociatePass(passManager);
		LLVM.AddCFGSimplificationPass(passManager);
		LLVM.AddGVNPass(passManager);
		LLVM.AddPromoteMemoryToRegisterPass(passManager);
		LLVM.AddSLPVectorizePass(passManager);
		LLVM.AddDeadStoreEliminationPass(passManager);
		LLVM.AddAggressiveDCEPass(passManager);
		LLVM.AddPartiallyInlineLibCallsPass(passManager);
		LLVM.InitializeFunctionPassManager(passManager);

		var function = module.AddFunction(name, LlvmType<DelegateT>());
		LLVM.SetLinkage(function, LLVMLinkage.LLVMExternalLinkage);

		LLVM.PositionBuilderAtEnd(builder, function.AppendBasicBlock("entrypoint"));

		var lbuilder = new LlvmBuilder<AddrT>(function, builder);
		body(lbuilder);

		if(!lbuilder.ReturnedThisBlock)
			LLVM.BuildRetVoid(builder);
		
		LLVM.VerifyFunction(function, LLVMVerifierFailureAction.LLVMPrintMessageAction);
		if(!function.VerifyFunction(LLVMVerifierFailureAction.LLVMReturnStatusAction))
			throw new Exception("Program verification failed");
		LLVM.RunFunctionPassManager(passManager, function);
		
		if(!module.TryCreateExecutionEngine(out var executionEngine, out var errorMessage))
			throw new Exception(errorMessage);
		
		return Helpers.GetAnyDelegateForFunctionPointer<DelegateT>(executionEngine.GetPointerToGlobal(function));
	}
}