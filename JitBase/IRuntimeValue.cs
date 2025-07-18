// ReSharper disable MemberCanBeProtected.Global
#pragma warning disable CS0660, CS0661
namespace JitBase;

public abstract class IRuntimeValue<T> where T : struct {
	public abstract IRuntimeValue<OT> Cast<OT>() where OT : struct;
	public abstract IRuntimeValue<OT> Bitcast<OT>() where OT : struct;
	public abstract IRuntimeValue<T> Store();
	public abstract IRuntimeValue<T> Add(IRuntimeValue<T> rhs);
	public abstract IRuntimeValue<T> Sub(IRuntimeValue<T> rhs);
	public abstract IRuntimeValue<T> Mul(IRuntimeValue<T> rhs);
	public abstract IRuntimeValue<T> Div(IRuntimeValue<T> rhs);
	public abstract IRuntimeValue<T> Mod(IRuntimeValue<T> rhs);
	public abstract IRuntimeValue<T> Negate();
	public abstract IRuntimeValue<T> And(IRuntimeValue<T> rhs);
	public abstract IRuntimeValue<T> Or(IRuntimeValue<T> rhs);
	public abstract IRuntimeValue<T> Xor(IRuntimeValue<T> rhs);
	public abstract IRuntimeValue<T> LeftShift(IRuntimeValue<T> rhs);
	public abstract IRuntimeValue<T> RightShift(IRuntimeValue<T> rhs);
	public abstract IRuntimeValue<T> Not();
	public abstract IRuntimeValue<bool> LT(IRuntimeValue<T> rhs);
	public abstract IRuntimeValue<bool> LTE(IRuntimeValue<T> rhs);
	public abstract IRuntimeValue<bool> EQ(IRuntimeValue<T> rhs);
	public abstract IRuntimeValue<bool> NE(IRuntimeValue<T> rhs);
	public abstract IRuntimeValue<bool> GTE(IRuntimeValue<T> rhs);
	public abstract IRuntimeValue<bool> GT(IRuntimeValue<T> rhs);
	public abstract IRuntimeValue<T> Abs();
	public abstract IRuntimeValue<T> Sqrt();
	public abstract IRuntimeValue<T> Round();
	public abstract IRuntimeValue<T> RoundHalfDown();
	public abstract IRuntimeValue<T> RoundHalfUp();
	public abstract IRuntimeValue<T> Ceil();
	public abstract IRuntimeValue<T> Floor();
	public abstract IRuntimeValue<bool> IsNaN();
	public abstract IRuntimeValue<ElementT> Element<ElementT>(int index) where ElementT : struct;
	public abstract IRuntimeValue<ElementT> Element<ElementT>(IRuntimeValue<int> index) where ElementT : struct;
	public abstract IRuntimeValue<T> Element<ElementT>(int index, IRuntimeValue<ElementT> value) where ElementT : struct;
	public abstract IRuntimeValue<T> Element<ElementT>(IRuntimeValue<int> index, IRuntimeValue<ElementT> value) where ElementT : struct;
	public static IRuntimeValue<T> operator +(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.Add(rhs);
	public static IRuntimeValue<T> operator -(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.Sub(rhs);
	public static IRuntimeValue<T> operator *(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.Mul(rhs);
	public static IRuntimeValue<T> operator /(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.Div(rhs);
	public static IRuntimeValue<T> operator %(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.Mod(rhs);
	public static IRuntimeValue<T> operator -(IRuntimeValue<T> v) => v.Negate();
	public static IRuntimeValue<T> operator &(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.And(rhs);
	public static IRuntimeValue<T> operator |(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.Or(rhs);
	public static IRuntimeValue<T> operator ^(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.Xor(rhs);
	public static IRuntimeValue<T> operator ~(IRuntimeValue<T> v) => v.Not();
	public static IRuntimeValue<bool> operator <(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.LT(rhs);
	public static IRuntimeValue<bool> operator <=(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.LTE(rhs);
	public static IRuntimeValue<bool> operator ==(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs?.EQ(rhs);
	public static IRuntimeValue<bool> operator !=(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs?.NE(rhs);
	public static IRuntimeValue<bool> operator >=(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.GTE(rhs);
	public static IRuntimeValue<bool> operator >(IRuntimeValue<T> lhs, IRuntimeValue<T> rhs) => lhs.GT(rhs);

	public static explicit operator IRuntimeValue<byte>(IRuntimeValue<T> val) => val.Cast<byte>();
	public static explicit operator IRuntimeValue<sbyte>(IRuntimeValue<T> val) => val.Cast<sbyte>();
	public static explicit operator IRuntimeValue<ushort>(IRuntimeValue<T> val) => val.Cast<ushort>();
	public static explicit operator IRuntimeValue<short>(IRuntimeValue<T> val) => val.Cast<short>();
	public static explicit operator IRuntimeValue<uint>(IRuntimeValue<T> val) => val.Cast<uint>();
	public static explicit operator IRuntimeValue<int>(IRuntimeValue<T> val) => val.Cast<int>();
	public static explicit operator IRuntimeValue<ulong>(IRuntimeValue<T> val) => val.Cast<ulong>();
	public static explicit operator IRuntimeValue<long>(IRuntimeValue<T> val) => val.Cast<long>();
	public static explicit operator IRuntimeValue<UInt128>(IRuntimeValue<T> val) => val.Cast<UInt128>();
	public static explicit operator IRuntimeValue<Int128>(IRuntimeValue<T> val) => val.Cast<Int128>();
	public static explicit operator IRuntimeValue<float>(IRuntimeValue<T> val) => val.Cast<float>();
	public static explicit operator IRuntimeValue<double>(IRuntimeValue<T> val) => val.Cast<double>();
}