﻿// ReSharper disable NotAccessedPositionalProperty.Global
namespace StaticRecompilerBase;

public abstract record StaticIRValue(Type Type) {
    public record Cast(StaticIRValue Value, Type OT) : StaticIRValue(OT);
    public record Bitcast(StaticIRValue Value, Type OT) : StaticIRValue(OT);
    public record Store(StaticIRValue Value) : StaticIRValue(Value.Type);

    public record Add(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type);
    public record Sub(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type);
    public record Mul(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type);
    public record Div(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type);
    public record Mod(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type);
    
    public record Negate(StaticIRValue Value) : StaticIRValue(Value.Type);
    
    public record And(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type);
    public record Or(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type);
    public record Xor(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type);
    public record LeftShift(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type);
    public record RightShift(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type);
    
    public record Not(StaticIRValue Value) : StaticIRValue(Value.Type);

    public record LT(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(typeof(bool));
    public record LTE(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(typeof(bool));
    public record EQ(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(typeof(bool));
    public record NE(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(typeof(bool));
    public record GTE(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(typeof(bool));
    public record GT(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(typeof(bool));
    
    public record Abs(StaticIRValue Value) : StaticIRValue(Value.Type);
    public record Sqrt(StaticIRValue Value) : StaticIRValue(Value.Type);
    public record Round(StaticIRValue Value) : StaticIRValue(Value.Type);
    public record RoundHalfDown(StaticIRValue Value) : StaticIRValue(Value.Type);
    public record RoundHalfUp(StaticIRValue Value) : StaticIRValue(Value.Type);
    public record Ceil(StaticIRValue Value) : StaticIRValue(Value.Type);
    public record Floor(StaticIRValue Value) : StaticIRValue(Value.Type);
    public record IsNaN(StaticIRValue Value) : StaticIRValue(typeof(bool));

    public record GetElement(StaticIRValue Vector, StaticIRValue Index, Type ElementType) : StaticIRValue(ElementType);
    public record SetElement(StaticIRValue Vector, StaticIRValue Index, StaticIRValue Element) : StaticIRValue(Vector.Type);
}