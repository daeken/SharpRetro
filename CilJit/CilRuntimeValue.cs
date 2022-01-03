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
	void EmitThen(Action next) {
		Generate();
		next();
	}

	static CilRuntimeValue<U, DelegateT> TT<U>(IRuntimeValue<U> v) where U : struct => v as CilRuntimeValue<U, DelegateT>;

	CilRuntimeValue<U, DelegateT> C<U>(Action gen) where U : struct => new(Ilg, Tb, gen);

	public override IRuntimeValue<OT> Cast<OT>() where OT : struct {
		if(typeof(OT) == typeof(T)) return (IRuntimeValue<OT>) (object) this;
		return C<OT>(() => EmitThen(() => {
			if(BitWidth<OT>() > BitWidth<T>()) {
				if(IsSigned<T>() && !IsSigned<OT>())
					Ilg.Convert(ToSigned<OT>());
				else if(!IsSigned<T>() && IsSigned<OT>())
					Ilg.Convert(ToUnsigned<OT>());
			}

			Ilg.Convert<OT>();
		}));
	}
	public override IRuntimeValue<OT> Bitcast<OT>() where OT : struct => throw new NotImplementedException();
	public override IRuntimeValue<T> Store() {
		var local = Ilg.DeclareLocal<T>();
		Emit();
		Ilg.StoreLocal(local);
		return C<T>(() => Ilg.LoadLocal(local));
	}
	public override IRuntimeValue<T> Add(IRuntimeValue<T> rhs) => C<T>(() => EmitThen(() => TT(rhs).EmitThen(() => Ilg.Add())));
	public override IRuntimeValue<T> Sub(IRuntimeValue<T> rhs) => C<T>(() => EmitThen(() => TT(rhs).EmitThen(() => Ilg.Subtract())));
	public override IRuntimeValue<T> Mul(IRuntimeValue<T> rhs) => C<T>(() => EmitThen(() => TT(rhs).EmitThen(() => Ilg.Multiply())));
	public override IRuntimeValue<T> Div(IRuntimeValue<T> rhs) => C<T>(() => EmitThen(() => TT(rhs).EmitThen(() => IsSigned<T>(() => Ilg.Divide(), () => Ilg.UnsignedDivide()))));
	public override IRuntimeValue<T> Mod(IRuntimeValue<T> rhs) => C<T>(() => EmitThen(() => TT(rhs).EmitThen(() => IsSigned<T>(() => Ilg.Remainder(), () => Ilg.UnsignedRemainder()))));
	public override IRuntimeValue<T> Negate() {
		if(!IsSigned<T>()) throw new NotSupportedException();
		return C<T>(() => EmitThen(() => Ilg.Negate()));
	}
	public override IRuntimeValue<T> And(IRuntimeValue<T> rhs) => C<T>(() => EmitThen(() => TT(rhs).EmitThen(() => Ilg.And())));
	public override IRuntimeValue<T> Or(IRuntimeValue<T> rhs) => C<T>(() => EmitThen(() => TT(rhs).EmitThen(() => Ilg.Or())));
	public override IRuntimeValue<T> Xor(IRuntimeValue<T> rhs) => C<T>(() => EmitThen(() => TT(rhs).EmitThen(() => Ilg.Xor())));
	public override IRuntimeValue<T> LeftShift(IRuntimeValue<T> rhs) => C<T>(() => EmitThen(() => TT((IRuntimeValue<int>) rhs).EmitThen(() => Ilg.ShiftLeft())));
	public override IRuntimeValue<T> RightShift(IRuntimeValue<T> rhs) => C<T>(() => EmitThen(() => TT((IRuntimeValue<int>) rhs).EmitThen(() => IsSigned<T>(() => Ilg.ShiftRight(), () => Ilg.UnsignedShiftRight()))));
	public override IRuntimeValue<T> Not() => default(T) switch {
		bool => C<T>(() => EmitThen(() => {
			Ilg.LoadConstant(0);
			Ilg.CompareEqual();
		})),
		_ => C<T>(() => EmitThen(() => Ilg.Not()))
	};
	public override IRuntimeValue<bool> LT(IRuntimeValue<T> rhs) => C<bool>(() => EmitThen(() => TT(rhs).EmitThen(() => IsSigned<T>(() => Ilg.CompareLessThan(), () => Ilg.UnsignedCompareLessThan()))));
	public override IRuntimeValue<bool> LTE(IRuntimeValue<T> rhs) => GT(rhs).Not();
	public override IRuntimeValue<bool> EQ(IRuntimeValue<T> rhs) => C<bool>(() => EmitThen(() => TT(rhs).EmitThen(() => Ilg.CompareEqual())));
	public override IRuntimeValue<bool> NE(IRuntimeValue<T> rhs) => EQ(rhs).Not();
	public override IRuntimeValue<bool> GTE(IRuntimeValue<T> rhs) => LT(rhs).Not();
	public override IRuntimeValue<bool> GT(IRuntimeValue<T> rhs) => C<bool>(() => EmitThen(() => TT(rhs).EmitThen(() => IsSigned<T>(() => Ilg.CompareGreaterThan(), () => Ilg.UnsignedCompareGreaterThan()))));
	public override IRuntimeValue<T> Abs() => throw new NotImplementedException();
	public override IRuntimeValue<T> Sqrt() => throw new NotImplementedException();
	public override IRuntimeValue<T> Round() => throw new NotImplementedException();
	public override IRuntimeValue<T> RoundHalfDown() => throw new NotImplementedException();
	public override IRuntimeValue<T> RoundHalfUp() => throw new NotImplementedException();
	public override IRuntimeValue<T> Ceil() => throw new NotImplementedException();
	public override IRuntimeValue<T> Floor() => throw new NotImplementedException();
	public override IRuntimeValue<bool> IsNaN() => throw new NotImplementedException();
	public override IRuntimeValue<ElementT> Element<ElementT>(IRuntimeValue<int> index) where ElementT : struct => throw new NotImplementedException();
	public override IRuntimeValue<T> Element<ElementT>(IRuntimeValue<int> index, IRuntimeValue<ElementT> value) where ElementT : struct => throw new NotImplementedException();
}