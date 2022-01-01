namespace JitBase; 

public interface IRuntimeValue<T> where T : struct {
	IRuntimeValue<OT> Cast<OT>() where OT : struct;
	IRuntimeValue<OT> Bitcast<OT>() where OT : struct;
	IRuntimeValue<T> Store();

	IRuntimeValue<T> Add(IRuntimeValue<T> rhs);
	IRuntimeValue<T> Sub(IRuntimeValue<T> rhs);
	IRuntimeValue<T> Mul(IRuntimeValue<T> rhs);
	IRuntimeValue<T> Div(IRuntimeValue<T> rhs);
	IRuntimeValue<T> Mod(IRuntimeValue<T> rhs);
	IRuntimeValue<T> Negate();

	IRuntimeValue<T> And(IRuntimeValue<T> rhs);
	IRuntimeValue<T> Or(IRuntimeValue<T> rhs);
	IRuntimeValue<T> Xor(IRuntimeValue<T> rhs);
	IRuntimeValue<T> LeftShift(IRuntimeValue<T> rhs);
	IRuntimeValue<T> RightShift(IRuntimeValue<T> rhs); // Logical vs arithmetic is decided by signedness of T
	IRuntimeValue<T> Not();

	IRuntimeValue<bool> LT(IRuntimeValue<T> rhs);
	IRuntimeValue<bool> LTE(IRuntimeValue<T> rhs);
	IRuntimeValue<bool> EQ(IRuntimeValue<T> rhs);
	IRuntimeValue<bool> NE(IRuntimeValue<T> rhs);
	IRuntimeValue<bool> GTE(IRuntimeValue<T> rhs);
	IRuntimeValue<bool> GT(IRuntimeValue<T> rhs);

	IRuntimeValue<T> Abs();
	IRuntimeValue<T> Sqrt();
	IRuntimeValue<T> Round();
	IRuntimeValue<T> RoundHalfDown();
	IRuntimeValue<T> RoundHalfUp();
	IRuntimeValue<T> Ceil();
	IRuntimeValue<T> Floor();
	IRuntimeValue<bool> IsNaN();
	
	IRuntimeValue<ElementT> Element<ElementT>(IRuntimeValue<int> index) where ElementT : struct;
	IRuntimeValue<T> Element<ElementT>(IRuntimeValue<int> index, IRuntimeValue<ElementT> value) where ElementT : struct;

	static IRuntimeValue<T> operator +(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.Add(rhs);
	static IRuntimeValue<T> operator -(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.Sub(rhs);
	static IRuntimeValue<T> operator *(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.Mul(rhs);
	static IRuntimeValue<T> operator /(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.Div(rhs);
	static IRuntimeValue<T> operator %(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.Mod(rhs);
	static IRuntimeValue<T> operator -(IRuntimeValue<T> v) => v.Negate();

	static IRuntimeValue<T> operator &(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.And(rhs);
	static IRuntimeValue<T> operator |(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.Or(rhs);
	static IRuntimeValue<T> operator ^(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.Xor(rhs);
	static IRuntimeValue<T> operator ~(IRuntimeValue<T> v) => v.Not();

	static IRuntimeValue<bool> operator <(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.LT(rhs);
	static IRuntimeValue<bool> operator <=(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.LTE(rhs);
	static IRuntimeValue<bool> operator >=(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.GTE(rhs);
	static IRuntimeValue<bool> operator >(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.GT(rhs);
}