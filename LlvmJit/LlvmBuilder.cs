using System.Runtime.InteropServices;
using JitBase;
using LLVMSharp;
using LLVMSharp.Interop;
using static LlvmJit.LlvmExtensions;

namespace LlvmJit; 

public unsafe class LlvmBuilder<AddrT> : IBuilder<AddrT> where AddrT : struct {
	LLVMValueRef Function;
	LLVMBuilderRef Builder;

	LLVMBasicBlockRef CurrentBlock;

	internal bool ReturnedThisBlock;
	
	internal LlvmBuilder(LLVMValueRef function, LLVMBuilderRef builder) {
		Function = function;
		Builder = builder;
	}

	void PositionBuilderAtEnd(LLVMBasicBlockRef block) => Builder.PositionAtEnd(CurrentBlock = block);

	public IRuntimeValue<T> C<T>(Func<LLVMValueRef> gen) where T : struct => new LlvmRuntimeValue<AddrT, T>(Builder, this, gen);
	public LLVMValueRef Emit<T>(IRuntimeValue<T> rv) where T : struct => ((LlvmRuntimeValue<AddrT, T>) rv).Emit();

	public IRuntimeValue<T> Argument<T>(int index) where T : struct => C<T>(() => {
		var parm = LLVM.GetParam(Function, (uint) index);
		return typeof(T) == typeof(bool) ? LLVM.BuildIntCast(Builder, parm, LLVMTypeRef.Int1, EmptyString) : parm;
	});

	public IStructRef<T> StructRefArgument<T>(int index) where T : IJitStruct => new LlvmStructRef<AddrT, T>(this, Builder, LLVM.GetParam(Function, (uint) index));

	public IRuntimeValue<T> Zero<T>() where T : struct => LiteralValue(default(T));
	public IRuntimeValue<T> LiteralValue<T>(T value) where T : struct => C<T>(() => value switch {
		sbyte v => LLVMValueRef.CreateConstInt(LLVMTypeRef.Int8, (ulong) v),
		byte v => LLVMValueRef.CreateConstInt(LLVMTypeRef.Int8, v),
		short v => LLVMValueRef.CreateConstInt(LLVMTypeRef.Int16, (ulong) v),
		ushort v => LLVMValueRef.CreateConstInt(LLVMTypeRef.Int16, v),
		int v => LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, (ulong) v),
		uint v => LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, v),
		long v => LLVMValueRef.CreateConstInt(LLVMTypeRef.Int64, (ulong) v),
		ulong v => LLVMValueRef.CreateConstInt(LLVMTypeRef.Int64, v),
		_ => throw new NotImplementedException(typeof(T).FullName)
	});
	public IRuntimePointer<AddrT, T> Pointer<T>(IRuntimeValue<AddrT> pointer) where T : struct => throw new NotImplementedException();
	public ILocalVar<T> DefineLocal<T>() where T : struct => new LlvmLocalVar<AddrT, T>(Builder, this);
	public void Sink<T>(IRuntimeValue<T> value) where T : struct {
		throw new NotImplementedException();
	}
	public void Return<T>(IRuntimeValue<T> value) where T : struct {
		var ret = Emit(value);
		if(typeof(T) == typeof(bool))
			ret = LLVM.BuildIntCast(Builder, ret, LlvmType<uint>(), EmptyString);
		LLVM.BuildRet(Builder, ret);
		ReturnedThisBlock = true;
	}
	public void If(IRuntimeValue<bool> cond, Action if_, Action else_) {
		if(ReturnedThisBlock)
			return;
		
		var ifLabel = Function.AppendBasicBlock("");
		var elseLabel = Function.AppendBasicBlock("");
		LLVMBasicBlockRef? endLabel = null;

		LLVM.BuildCondBr(Builder, Emit(cond), ifLabel, elseLabel);
		PositionBuilderAtEnd(ifLabel);
		if_();
		if(!ReturnedThisBlock)
			LLVM.BuildBr(Builder, (endLabel = Function.AppendBasicBlock("")).Value);
		PositionBuilderAtEnd(elseLabel);
		ReturnedThisBlock = false;
		else_();
		if(!ReturnedThisBlock)
			LLVM.BuildBr(Builder, endLabel ??= Function.AppendBasicBlock(""));
		if(endLabel != null) {
			PositionBuilderAtEnd(endLabel.Value);
			ReturnedThisBlock = false;
		}
	}
	public void When(IRuntimeValue<bool> cond, Action when) {
		if(ReturnedThisBlock)
			return;
		
		var ifLabel = Function.AppendBasicBlock("");
		var endLabel = Function.AppendBasicBlock("");

		LLVM.BuildCondBr(Builder, Emit(cond), ifLabel, endLabel);
		PositionBuilderAtEnd(ifLabel);
		when();
		if(!ReturnedThisBlock)
			LLVM.BuildBr(Builder, endLabel);
		PositionBuilderAtEnd(endLabel);
		ReturnedThisBlock = false;
	}
	public void Unless(IRuntimeValue<bool> cond, Action unless) {
		if(ReturnedThisBlock)
			return;
		
		var ifLabel = Function.AppendBasicBlock("");
		var endLabel = Function.AppendBasicBlock("");

		LLVM.BuildCondBr(Builder, Emit(cond), endLabel, ifLabel);
		PositionBuilderAtEnd(ifLabel);
		unless();
		if(!ReturnedThisBlock)
			LLVM.BuildBr(Builder, endLabel);
		PositionBuilderAtEnd(endLabel);
		ReturnedThisBlock = false;
	}
	public void While(IRuntimeValue<bool> cond, Action body) {
		if(ReturnedThisBlock)
			return;
		
		var startLabel = Function.AppendBasicBlock("");
		var bodyLabel = Function.AppendBasicBlock("");
		var endLabel = Function.AppendBasicBlock("");

		LLVM.BuildBr(Builder, startLabel);
		
		PositionBuilderAtEnd(startLabel);
		LLVM.BuildCondBr(Builder, Emit(cond), bodyLabel, endLabel);
		PositionBuilderAtEnd(bodyLabel);
		body();
		if(!ReturnedThisBlock)
			LLVM.BuildBr(Builder, startLabel);
		
		PositionBuilderAtEnd(endLabel);
		ReturnedThisBlock = false;
	}
	public void DoWhile(Action body, IRuntimeValue<bool> cond) {
		if(ReturnedThisBlock)
			return;
		
		var bodyLabel = Function.AppendBasicBlock("");

		LLVM.BuildBr(Builder, bodyLabel);
		
		PositionBuilderAtEnd(bodyLabel);
		body();
		if(ReturnedThisBlock)
			return;
		
		var endLabel = Function.AppendBasicBlock("");
		LLVM.BuildCondBr(Builder, Emit(cond), bodyLabel, endLabel);
		
		PositionBuilderAtEnd(endLabel);
	}
	public IRuntimeValue<T> Ternary<T>(IRuntimeValue<bool> cond, IRuntimeValue<T> a, IRuntimeValue<T> b) where T : struct => C<T>(() => {
		var ifLabel = Function.AppendBasicBlock("");
		var elseLabel = Function.AppendBasicBlock("");
		var endLabel = Function.AppendBasicBlock("");

		LLVM.BuildCondBr(Builder, Emit(cond), ifLabel, elseLabel);
		
		PositionBuilderAtEnd(ifLabel);
		var left = Emit(a);
		var leftBlockEnd = CurrentBlock;
		LLVM.BuildBr(Builder, endLabel);

		PositionBuilderAtEnd(elseLabel);
		var right = Emit(b);
		var rightBlockEnd = CurrentBlock;
		LLVM.BuildBr(Builder, endLabel);
		
		PositionBuilderAtEnd(endLabel);
		var phi = (LLVMValueRef) LLVM.BuildPhi(Builder, LlvmType<T>(), EmptyString);
		phi.AddIncoming(new[] { left, right }, new[] { leftBlockEnd, rightBlockEnd }, 2);
		return phi;
	});

	LLVMValueRef DelegateToFP<DelegateT>(DelegateT del) {
		var fptr = Helpers.GetFunctionPointerForAnyDelegate(del);
		return Builder.BuildIntToPtr(LLVMValueRef.CreateConstInt(LLVMTypeRef.Int64, (ulong) fptr), LLVMTypeRef.CreatePointer(LlvmType<DelegateT>(), 0));
	}

	LLVMValueRef PrepArg<T>(IRuntimeValue<T> arg) where T : struct {
		var ret = Emit(arg);
		if(typeof(T) == typeof(bool))
			ret = LLVM.BuildIntCast(Builder, ret, LlvmType<uint>(), EmptyString);
		return ret;
	}

	LLVMValueRef PrepRet<T>(LLVMValueRef ret) where T : struct => typeof(T) == typeof(bool) ? LLVM.BuildIntCast(Builder, ret, LLVMTypeRef.Int1, EmptyString) : ret;

	public void CallVoid(Action func) => 
		Builder.BuildCall2(LlvmType<Action>(), DelegateToFP(func), []);
	public void CallVoid<T1>(Action<T1> func, IRuntimeValue<T1> a1) where T1 : struct => 
		Builder.BuildCall2(LlvmType<Action<T1>>(), DelegateToFP(func), [PrepArg(a1)]);
	public void CallVoid<T1, T2>(Action<T1, T2> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2) where T1 : struct where T2 : struct => 
		Builder.BuildCall2(LlvmType<Action<T1, T2>>(), DelegateToFP(func), [PrepArg(a1), PrepArg(a2)]);
	public void CallVoid<T1, T2, T3>(Action<T1, T2, T3> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3) where T1 : struct where T2 : struct where T3 : struct => 
		Builder.BuildCall2(LlvmType<Action<T1, T2, T3>>(), DelegateToFP(func), [PrepArg(a1), PrepArg(a2), PrepArg(a3)]);
	public void CallVoid<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3, IRuntimeValue<T4> a4) where T1 : struct where T2 : struct where T3 : struct where T4 : struct =>
		Builder.BuildCall2(LlvmType<Action<T1, T2, T3, T4>>(), DelegateToFP(func), [PrepArg(a1), PrepArg(a2), PrepArg(a3), PrepArg(a4)]);
	public IRuntimeValue<RetT> Call<RetT>(Func<RetT> func) where RetT : struct =>
		C<RetT>(() => PrepRet<RetT>(Builder.BuildCall2(LlvmType<Func<RetT>>(), DelegateToFP(func), [])));
	public IRuntimeValue<RetT> Call<T1, RetT>(Func<T1, RetT> func, IRuntimeValue<T1> a1) where T1 : struct where RetT : struct =>
		C<RetT>(() => PrepRet<RetT>(Builder.BuildCall2(LlvmType<Func<T1, RetT>>(), DelegateToFP(func), [PrepArg(a1)])));
	public IRuntimeValue<RetT> Call<T1, T2, RetT>(Func<T1, T2, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2) where T1 : struct where T2 : struct where RetT : struct => 
		C<RetT>(() => PrepRet<RetT>(Builder.BuildCall2(LlvmType<Func<T1, T2, RetT>>(), DelegateToFP(func), [PrepArg(a1), PrepArg(a2)])));
	public IRuntimeValue<RetT> Call<T1, T2, T3, RetT>(Func<T1, T2, T3, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3) where T1 : struct where T2 : struct where T3 : struct where RetT : struct =>
		C<RetT>(() => PrepRet<RetT>(Builder.BuildCall2(LlvmType<Func<T1, T2, T3, RetT>>(), DelegateToFP(func), [PrepArg(a1), PrepArg(a2), PrepArg(a3)])));
	public IRuntimeValue<RetT> Call<T1, T2, T3, T4, RetT>(Func<T1, T2, T3, T4, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3, IRuntimeValue<T4> a4) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where RetT : struct =>
		C<RetT>(() => PrepRet<RetT>(Builder.BuildCall2(LlvmType<Func<T1, T2, T3, T4, RetT>>(), DelegateToFP(func), [PrepArg(a1), PrepArg(a2), PrepArg(a3), PrepArg(a4),
		])));
}