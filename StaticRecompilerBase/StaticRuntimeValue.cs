using System.Runtime.Intrinsics;
using JitBase;
// ReSharper disable MemberCanBePrivate.Global

namespace StaticRecompilerBase;

public class StaticRuntimeValue<T>(StaticIRValue value) : IRuntimeValue<T> where T : struct {
    public readonly StaticIRValue Value = value.Type == typeof(T) ? value : throw new InvalidCastException();

    public static implicit operator StaticIRValue(StaticRuntimeValue<T> srv) => srv.Value;
    public static implicit operator StaticRuntimeValue<T>(StaticIRValue siv) => new(siv);
    static StaticRuntimeValue<OT> W<OT>(IRuntimeValue<OT> irv) where OT : struct => irv as StaticRuntimeValue<OT> ?? throw new();
    static StaticRuntimeValue<T> W(StaticIRValue siv) => new(siv);
    static StaticRuntimeValue<OT> W<OT>(StaticIRValue siv) where OT : struct => new(siv);
    
    public override IRuntimeValue<T> ToConstant(T value) => throw new NotImplementedException();
    
    public override IRuntimeValue<OT> Cast<OT>() => W<OT>(new StaticIRValue.Cast(this, typeof(OT)));
    public override IRuntimeValue<OT> Bitcast<OT>() => W<OT>(new StaticIRValue.Bitcast(this, typeof(OT)));
    public override IRuntimeValue<T> Store() => W(new StaticIRValue.Store(this));

    public override IRuntimeValue<T> Add(IRuntimeValue<T> rhs) => W(new StaticIRValue.Add(this, W(rhs)));
    public override IRuntimeValue<T> Sub(IRuntimeValue<T> rhs) => W(new StaticIRValue.Sub(this, W(rhs)));
    public override IRuntimeValue<T> Mul(IRuntimeValue<T> rhs) => W(new StaticIRValue.Mul(this, W(rhs)));
    public override IRuntimeValue<T> Div(IRuntimeValue<T> rhs) => W(new StaticIRValue.Div(this, W(rhs)));
    public override IRuntimeValue<T> Mod(IRuntimeValue<T> rhs) => W(new StaticIRValue.Mod(this, W(rhs)));

    public override IRuntimeValue<T> Negate() => W(new StaticIRValue.Negate(this));

    public override IRuntimeValue<T> And(IRuntimeValue<T> rhs) => W(new StaticIRValue.And(this, W(rhs)));
    public override IRuntimeValue<T> Or(IRuntimeValue<T> rhs) => W(new StaticIRValue.Or(this, W(rhs)));
    public override IRuntimeValue<T> Xor(IRuntimeValue<T> rhs) => W(new StaticIRValue.Xor(this, W(rhs)));
    public override IRuntimeValue<T> LeftShift(IRuntimeValue<T> rhs) => W(new StaticIRValue.LeftShift(this, W(rhs)));
    public override IRuntimeValue<T> RightShift(IRuntimeValue<T> rhs) => W(new StaticIRValue.RightShift(this, W(rhs)));

    public override IRuntimeValue<T> Not() => W(new StaticIRValue.Not(this));

    public override IRuntimeValue<bool> LT(IRuntimeValue<T> rhs) => W<bool>(new StaticIRValue.LT(this, W(rhs)));
    public override IRuntimeValue<bool> LTE(IRuntimeValue<T> rhs) => W<bool>(new StaticIRValue.LTE(this, W(rhs)));
    public override IRuntimeValue<bool> EQ(IRuntimeValue<T> rhs) => W<bool>(new StaticIRValue.EQ(this, W(rhs)));
    public override IRuntimeValue<bool> NE(IRuntimeValue<T> rhs) => W<bool>(new StaticIRValue.NE(this, W(rhs)));
    public override IRuntimeValue<bool> GTE(IRuntimeValue<T> rhs) => W<bool>(new StaticIRValue.GTE(this, W(rhs)));
    public override IRuntimeValue<bool> GT(IRuntimeValue<T> rhs) => W<bool>(new StaticIRValue.GT(this, W(rhs)));

    public override IRuntimeValue<T> Abs() => W(new StaticIRValue.Abs(this));
    public override IRuntimeValue<T> Sqrt() => W(new StaticIRValue.Sqrt(this));
    public override IRuntimeValue<T> Round() => W(new StaticIRValue.Round(this));
    public override IRuntimeValue<T> RoundHalfDown() => W(new StaticIRValue.RoundHalfDown(this));
    public override IRuntimeValue<T> RoundHalfUp() => W(new StaticIRValue.RoundHalfUp(this));
    public override IRuntimeValue<T> Ceil() => W(new StaticIRValue.Ceil(this));
    public override IRuntimeValue<T> Floor() => W(new StaticIRValue.Floor(this));
    public override IRuntimeValue<bool> IsNaN() => W<bool>(new StaticIRValue.IsNaN(this));
    public override IRuntimeValue<U> SignExt<U>(int width) => throw new NotImplementedException();

    public override IRuntimeValue<ElementT> Element<ElementT>(int index) => throw new NotImplementedException();
    public override IRuntimeValue<ElementT> Element<ElementT>(IRuntimeValue<int> index) =>
        W<ElementT>(new StaticIRValue.GetElement(this, W(index), typeof(ElementT)));

    public override IRuntimeValue<T> Element<ElementT>(int index, IRuntimeValue<ElementT> value) =>
        throw new NotImplementedException();
    public override IRuntimeValue<T> Element<ElementT>(IRuntimeValue<int> index, IRuntimeValue<ElementT> value) =>
        W(new StaticIRValue.SetElement(this, W(index), W(value)));

    public override IRuntimeValue<T> ZeroTop() => throw new NotImplementedException();
    
    public override IRuntimeValue<Vector128<T>> CreateVector() => throw new NotImplementedException();
}