using System.Reflection.Emit;
using JitBase;
using Sigil;
using static JitBase.Helpers;

namespace CilJit; 

public class CilRuntimeValue<T, DelegateT> : IRuntimeValue<T> where T : struct {
	readonly Emit<DelegateT> Ilg;
	readonly TypeBuilder Tb;
	readonly Action Generate;

	internal CilRuntimeValue(Emit<DelegateT> ilg, TypeBuilder tb, Action generate) {
		Ilg = ilg;
		Tb = tb;
		Generate = generate;
	}
	internal void Emit() => Generate();
	internal void EmitThen(Action next) {
		Generate();
		next();
	}

	static CilRuntimeValue<U, DelegateT> TT<U>(IRuntimeValue<U> v) where U : struct => v as CilRuntimeValue<U, DelegateT>;

	IRuntimeValue<U> C<U>(Action gen) where U : struct => new CilRuntimeValue<U, DelegateT>(Ilg, Tb, gen);

	public IRuntimeValue<OT> Cast<OT>() where OT : struct => throw new NotImplementedException();
	public IRuntimeValue<OT> Bitcast<OT>() where OT : struct => throw new NotImplementedException();
	public IRuntimeValue<T> Store() => throw new NotImplementedException();
	public IRuntimeValue<T> Add(IRuntimeValue<T> rhs) => C<T>(() => EmitThen(() => TT(rhs).EmitThen(() => Ilg.Add())));
	public IRuntimeValue<T> Sub(IRuntimeValue<T> rhs) => C<T>(() => EmitThen(() => TT(rhs).EmitThen(() => Ilg.Subtract())));
	public IRuntimeValue<T> Mul(IRuntimeValue<T> rhs) => C<T>(() => EmitThen(() => TT(rhs).EmitThen(() => Ilg.Multiply())));
	public IRuntimeValue<T> Div(IRuntimeValue<T> rhs) => C<T>(() => EmitThen(() => TT(rhs).EmitThen(() => IsSigned<T>(() => Ilg.Divide(), () => Ilg.UnsignedDivide()))));
	public IRuntimeValue<T> Mod(IRuntimeValue<T> rhs) => C<T>(() => EmitThen(() => TT(rhs).EmitThen(() => IsSigned<T>(() => Ilg.Remainder(), () => Ilg.UnsignedRemainder()))));
	public IRuntimeValue<T> Negate() => throw new NotImplementedException();
	public IRuntimeValue<T> And(IRuntimeValue<T> rhs) => C<T>(() => EmitThen(() => TT(rhs).EmitThen(() => Ilg.And())));
	public IRuntimeValue<T> Or(IRuntimeValue<T> rhs) => C<T>(() => EmitThen(() => TT(rhs).EmitThen(() => Ilg.Or())));
	public IRuntimeValue<T> Xor(IRuntimeValue<T> rhs) => C<T>(() => EmitThen(() => TT(rhs).EmitThen(() => Ilg.Xor())));
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