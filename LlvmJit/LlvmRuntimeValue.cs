using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using JitBase;
using LLVMSharp.Interop;
using static JitBase.Helpers;
using static LlvmJit.LlvmExtensions;

namespace LlvmJit; 

public unsafe class LlvmRuntimeValue<AddrT, T> : IRuntimeValue<T> where AddrT : struct where T : struct {
	readonly Func<LLVMValueRef> Generate;
	readonly LLVMBuilderRef Builder;
	readonly LlvmBuilder<AddrT> JBuilder;

	internal LlvmRuntimeValue(LLVMBuilderRef builder, LlvmBuilder<AddrT> jBuilder, Func<LLVMValueRef> gen) {
		Builder = builder;
		JBuilder = jBuilder;
		Generate = gen;
	}

	internal LLVMValueRef Emit() => Generate();
	internal static LLVMValueRef Emit<U>(IRuntimeValue<U> rv) where U : struct => ((LlvmRuntimeValue<AddrT, U>) rv).Emit();
	IRuntimeValue<U> C<U>(Func<LLVMValueRef> gen) where U : struct => new LlvmRuntimeValue<AddrT, U>(Builder, JBuilder, gen);
	
	public override IRuntimeValue<T> ToConstant(T value) => C<T>(() => {
		throw new NotImplementedException();
	});

	public override IRuntimeValue<OT> Cast<OT>() where OT : struct {
		if(typeof(OT) == typeof(T)) return (IRuntimeValue<OT>) (object) this;
		return C<OT>(() => {
			var val = Emit();

			var fw = BitWidth<T>();
			var sw = BitWidth<OT>();
			var fs = IsSigned<T>();
			var ss = IsSigned<OT>();

			var opcode = LLVMOpcode.LLVMTrunc;
			if(fw == sw)
				opcode = LLVMOpcode.LLVMBitCast;
			else if(fw < sw)
				opcode = (fs && !ss) || (fs && ss) ? LLVMOpcode.LLVMSExt : LLVMOpcode.LLVMZExt;

			return LLVM.BuildCast(Builder, opcode, val, LlvmType<OT>(), EmptyString);
		});
	}
	public override IRuntimeValue<OT> Bitcast<OT>() where OT : struct {
		if(typeof(OT) == typeof(T)) return (IRuntimeValue<OT>) (object) this;
		if(BitWidth<OT>() != BitWidth<T>()) throw new();
		return C<OT>(() => LLVM.BuildBitCast(Builder, Emit(), LlvmType<OT>(), EmptyString));
	}
	public override IRuntimeValue<T> Store() {
		var val = Emit();
		return C<T>(() => val);
	}

	public override IRuntimeValue<T> Add(IRuntimeValue<T> rhs) => C<T>(() => LLVM.BuildAdd(Builder, Emit(), Emit(rhs), EmptyString));
	public override IRuntimeValue<T> Add<U>(IRuntimeValue<U> rhs) where U : struct => throw new NotImplementedException();
	public override IRuntimeValue<T> Sub(IRuntimeValue<T> rhs) => C<T>(() => LLVM.BuildSub(Builder, Emit(), Emit(rhs), EmptyString));
	public override IRuntimeValue<T> Mul(IRuntimeValue<T> rhs) => C<T>(() => LLVM.BuildMul(Builder, Emit(), Emit(rhs), EmptyString));
	public override IRuntimeValue<T> Mul<U>(IRuntimeValue<U> rhs) where U : struct => throw new NotImplementedException();
	public override IRuntimeValue<T> Div(IRuntimeValue<T> rhs) => C<T>(() => IsSigned<T>()
		? LLVM.BuildSDiv(Builder, Emit(), Emit(rhs), EmptyString)
		: LLVM.BuildUDiv(Builder, Emit(), Emit(rhs), EmptyString));
	public override IRuntimeValue<T> Mod(IRuntimeValue<T> rhs) => C<T>(() => IsSigned<T>()
		? LLVM.BuildSRem(Builder, Emit(), Emit(rhs), EmptyString)
		: LLVM.BuildURem(Builder, Emit(), Emit(rhs), EmptyString));
	public override IRuntimeValue<T> Negate() => !IsSigned<T>() ? throw new NotSupportedException() :
		C<T>(() => LLVM.BuildNeg(Builder, Emit(), EmptyString));
	public override IRuntimeValue<T> And(IRuntimeValue<T> rhs) => C<T>(() => LLVM.BuildAnd(Builder, Emit(), Emit(rhs), EmptyString));
	public override IRuntimeValue<T> Or(IRuntimeValue<T> rhs) => C<T>(() => LLVM.BuildOr(Builder, Emit(), Emit(rhs), EmptyString));
	public override IRuntimeValue<T> Xor(IRuntimeValue<T> rhs) => C<T>(() => LLVM.BuildXor(Builder, Emit(), Emit(rhs), EmptyString));
	public override IRuntimeValue<T> LeftShift(IRuntimeValue<T> rhs) => 
		C<T>(() => LLVM.BuildShl(Builder, Emit(), Emit(rhs & JBuilder.LiteralValue(BitWidth<T>() <= 32 ? 0x1F : 0x3F).Cast<T>()), EmptyString));
	public override IRuntimeValue<T> RightShift(IRuntimeValue<T> rhs) =>
		C<T>(() => {
			var shift = Emit(rhs & JBuilder.LiteralValue(BitWidth<T>() <= 32 ? 0x1F : 0x3F).Cast<T>());
			return IsSigned<T>()
				? LLVM.BuildAShr(Builder, Emit(), shift, EmptyString)
				: LLVM.BuildLShr(Builder, Emit(), shift, EmptyString);
		});
	public override IRuntimeValue<T> Not() => throw new NotImplementedException();
	public override IRuntimeValue<T> ReverseBits() => throw new NotImplementedException();
	public override IRuntimeValue<T> CountLeadingZeros() => throw new NotImplementedException();
	public override IRuntimeValue<bool> LT(IRuntimeValue<T> rhs) => C<bool>(() => LLVM.BuildICmp(Builder, IsSigned<T>() ? LLVMIntPredicate.LLVMIntSLT : LLVMIntPredicate.LLVMIntULT, Emit(), Emit(rhs), EmptyString));
	public override IRuntimeValue<bool> LTE(IRuntimeValue<T> rhs) => C<bool>(() => LLVM.BuildICmp(Builder, IsSigned<T>() ? LLVMIntPredicate.LLVMIntSLE : LLVMIntPredicate.LLVMIntULE, Emit(), Emit(rhs), EmptyString));
	public override IRuntimeValue<bool> EQ(IRuntimeValue<T> rhs) => C<bool>(() => LLVM.BuildICmp(Builder, LLVMIntPredicate.LLVMIntEQ, Emit(), Emit(rhs), EmptyString));
	public override IRuntimeValue<bool> NE(IRuntimeValue<T> rhs) => C<bool>(() => LLVM.BuildICmp(Builder, LLVMIntPredicate.LLVMIntNE, Emit(), Emit(rhs), EmptyString));
	public override IRuntimeValue<bool> GTE(IRuntimeValue<T> rhs) => C<bool>(() => LLVM.BuildICmp(Builder, IsSigned<T>() ? LLVMIntPredicate.LLVMIntSGE : LLVMIntPredicate.LLVMIntUGE, Emit(), Emit(rhs), EmptyString));
	public override IRuntimeValue<bool> GT(IRuntimeValue<T> rhs) => C<bool>(() => LLVM.BuildICmp(Builder, IsSigned<T>() ? LLVMIntPredicate.LLVMIntSGT : LLVMIntPredicate.LLVMIntUGT, Emit(), Emit(rhs), EmptyString));
	public override IRuntimeValue<T> Abs() => throw new NotImplementedException();
	public override IRuntimeValue<T> Sqrt() => throw new NotImplementedException();
	public override IRuntimeValue<T> Round() => throw new NotImplementedException();
	public override IRuntimeValue<T> RoundHalfDown() => throw new NotImplementedException();
	public override IRuntimeValue<T> RoundHalfUp() => throw new NotImplementedException();
	public override IRuntimeValue<T> Ceil() => throw new NotImplementedException();
	public override IRuntimeValue<T> Floor() => throw new NotImplementedException();
	public override IRuntimeValue<bool> IsNaN() => throw new NotImplementedException();
	public override IRuntimeValue<U> SignExt<U>(int width) => throw new NotImplementedException();
	public override IRuntimeValue<ElementT> Element<ElementT>(int index) where ElementT : struct => throw new NotImplementedException();
	public override IRuntimeValue<ElementT> Element<ElementT>(IRuntimeValue<int> index) where ElementT : struct => throw new NotImplementedException();
	public override IRuntimeValue<T> Element<ElementT>(int index, IRuntimeValue<ElementT> value) where ElementT : struct => throw new NotImplementedException();
	public override IRuntimeValue<T> Element<ElementT>(IRuntimeValue<int> index, IRuntimeValue<ElementT> value) where ElementT : struct => throw new NotImplementedException();
	public override IRuntimeValue<T> ZeroTop() => throw new NotImplementedException();
	public override IRuntimeValue<Vector128<T>> CreateVector() => throw new NotImplementedException();
}