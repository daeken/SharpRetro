// ReSharper disable NotAccessedPositionalProperty.Global

using System.Runtime.Intrinsics;

namespace StaticRecompilerBase;

public abstract record StaticIRValue(Type Type) {
    public abstract void Walk(Action<StaticIRValue> func);

    public record Named(string Name, Type Type) : StaticIRValue(Type) {
        public override void Walk(Action<StaticIRValue> func) => func(this);
    }

    public record Literal(object Value, Type Type) : StaticIRValue(Type) {
        public override void Walk(Action<StaticIRValue> func) => func(this);
    }
    public record Cast(StaticIRValue Value, Type OT) : StaticIRValue(OT) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }
    }
    public record Bitcast(StaticIRValue Value, Type OT) : StaticIRValue(OT) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }
    }
    public record SignExt(StaticIRValue Value, int Width, Type OT) : StaticIRValue(OT) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }
    }
    public record Store(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }
    }
    public record ZeroTop(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }
    }

    public record Add(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }
    }
    public record Sub(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }
    }
    public record Mul(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }
    }
    public record Div(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }
    }
    public record Mod(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }
    }
    
    public record Negate(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }
    }
    
    public record And(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }
    }
    public record Or(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }
    }
    public record Xor(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }
    }
    public record LeftShift(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }
    }
    public record RightShift(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }
    }
    
    public record Not(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }
    }
    
    public record ReverseBits(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }
    }
    public record CountLeadingZeros(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }
    }

    public record LT(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(typeof(bool)) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }
    }
    public record LTE(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(typeof(bool)) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }
    }
    public record EQ(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(typeof(bool)) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }
    }
    public record NE(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(typeof(bool)) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }
    }
    public record GTE(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(typeof(bool)) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }
    }
    public record GT(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(typeof(bool)) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }
    }
    
    public record Abs(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }
    }
    public record Sqrt(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }
    }
    public record Round(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }
    }
    public record RoundHalfDown(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }
    }
    public record RoundHalfUp(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }
    }
    public record Ceil(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }
    }
    public record Floor(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }
    }
    public record IsNaN(StaticIRValue Value) : StaticIRValue(typeof(bool)) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }
    }
    
    public record Dereference(StaticIRValue Pointer, Type Type) : StaticIRValue(Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Pointer.Walk(func);
        }
    }
    public record GetField(StaticIRValue Pointer, string Name, Type Type) : StaticIRValue(Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Pointer.Walk(func);
        }
    }
    public record GetFieldIndex(StaticIRValue Pointer, string Name, int Index, Type Type) : StaticIRValue(Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Pointer.Walk(func);
        }
    }

    public record CreateVector(StaticIRValue Value) : StaticIRValue(typeof(Vector128<>).MakeGenericType(Value.Type)) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }
    }
    public record GetElement(StaticIRValue Vector, StaticIRValue Index, Type ElementType) : StaticIRValue(ElementType) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Vector.Walk(func);
            Index.Walk(func);
        }
    }
    public record SetElement(StaticIRValue Vector, StaticIRValue Index, StaticIRValue Element) : StaticIRValue(Vector.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Vector.Walk(func);
            Index.Walk(func);
            Element.Walk(func);
        }
    }
    
    public record Ternary(StaticIRValue Condition, StaticIRValue True, StaticIRValue False) : StaticIRValue(True.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Condition.Walk(func);
            True.Walk(func);
            False.Walk(func);
        }
    }
}