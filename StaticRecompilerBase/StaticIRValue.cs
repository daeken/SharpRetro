// ReSharper disable NotAccessedPositionalProperty.Global

using System.Runtime.Intrinsics;

namespace StaticRecompilerBase;

public abstract record StaticIRValue(Type Type) {
    public abstract void Walk(Action<StaticIRValue> func);
    public abstract StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func);

    public record Named(string Name, Type Type) : StaticIRValue(Type) {
        public int SsaId = -1;
        public override void Walk(Action<StaticIRValue> func) => func(this);
        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) => func(this) ?? this;
    }

    public record NamedOut(string Name, Type Type) : StaticIRValue(Type) {
        public int SsaId = -1;
        public override void Walk(Action<StaticIRValue> func) => func(this);
        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) => func(this) ?? this;
    }
    
    public record Literal(object Value, Type Type) : StaticIRValue(Type) {
        public override void Walk(Action<StaticIRValue> func) => func(this);
        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) => func(this) ?? this;
    }
    public record Cast(StaticIRValue Value, Type OT) : StaticIRValue(OT) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var value = Value.Transform(func);
            var nthis = value != null && !ReferenceEquals(value, Value)
                ? this with { Value = value }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record Bitcast(StaticIRValue Value, Type OT) : StaticIRValue(OT) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var value = Value.Transform(func);
            var nthis = value != null && !ReferenceEquals(value, Value)
                ? this with { Value = value }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record SignExt(StaticIRValue Value, int Width, Type OT) : StaticIRValue(OT) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var value = Value.Transform(func);
            var nthis = value != null && !ReferenceEquals(value, Value)
                ? this with { Value = value }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record Store(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var value = Value.Transform(func);
            var nthis = value != null && !ReferenceEquals(value, Value)
                ? this with { Value = value }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record ZeroTop(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var value = Value.Transform(func);
            var nthis = value != null && !ReferenceEquals(value, Value)
                ? this with { Value = value }
                : this;
            return func(nthis) ?? nthis;
        }
    }

    public record Add(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var left = Left.Transform(func);
            var right = Right.Transform(func);
            var nthis = (left != null && !ReferenceEquals(left, Left)) || (right != null && !ReferenceEquals(right, Right))
                ? this with {
                    Left = left != null && !ReferenceEquals(left, Left) ? left : Left, 
                    Right = right != null && !ReferenceEquals(right, Right) ? right : Right
                }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record Sub(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var left = Left.Transform(func);
            var right = Right.Transform(func);
            var nthis = (left != null && !ReferenceEquals(left, Left)) || (right != null && !ReferenceEquals(right, Right))
                ? this with {
                    Left = left != null && !ReferenceEquals(left, Left) ? left : Left, 
                    Right = right != null && !ReferenceEquals(right, Right) ? right : Right
                }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record Mul(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var left = Left.Transform(func);
            var right = Right.Transform(func);
            var nthis = (left != null && !ReferenceEquals(left, Left)) || (right != null && !ReferenceEquals(right, Right))
                ? this with {
                    Left = left != null && !ReferenceEquals(left, Left) ? left : Left, 
                    Right = right != null && !ReferenceEquals(right, Right) ? right : Right
                }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record Div(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var left = Left.Transform(func);
            var right = Right.Transform(func);
            var nthis = (left != null && !ReferenceEquals(left, Left)) || (right != null && !ReferenceEquals(right, Right))
                ? this with {
                    Left = left != null && !ReferenceEquals(left, Left) ? left : Left, 
                    Right = right != null && !ReferenceEquals(right, Right) ? right : Right
                }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record Mod(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var left = Left.Transform(func);
            var right = Right.Transform(func);
            var nthis = (left != null && !ReferenceEquals(left, Left)) || (right != null && !ReferenceEquals(right, Right))
                ? this with {
                    Left = left != null && !ReferenceEquals(left, Left) ? left : Left, 
                    Right = right != null && !ReferenceEquals(right, Right) ? right : Right
                }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    
    public record Negate(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var value = Value.Transform(func);
            var nthis = value != null && !ReferenceEquals(value, Value)
                ? this with { Value = value }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    
    public record And(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var left = Left.Transform(func);
            var right = Right.Transform(func);
            var nthis = (left != null && !ReferenceEquals(left, Left)) || (right != null && !ReferenceEquals(right, Right))
                ? this with {
                    Left = left != null && !ReferenceEquals(left, Left) ? left : Left, 
                    Right = right != null && !ReferenceEquals(right, Right) ? right : Right
                }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record Or(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var left = Left.Transform(func);
            var right = Right.Transform(func);
            var nthis = (left != null && !ReferenceEquals(left, Left)) || (right != null && !ReferenceEquals(right, Right))
                ? this with {
                    Left = left != null && !ReferenceEquals(left, Left) ? left : Left, 
                    Right = right != null && !ReferenceEquals(right, Right) ? right : Right
                }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record Xor(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var left = Left.Transform(func);
            var right = Right.Transform(func);
            var nthis = (left != null && !ReferenceEquals(left, Left)) || (right != null && !ReferenceEquals(right, Right))
                ? this with {
                    Left = left != null && !ReferenceEquals(left, Left) ? left : Left, 
                    Right = right != null && !ReferenceEquals(right, Right) ? right : Right
                }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record LeftShift(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var left = Left.Transform(func);
            var right = Right.Transform(func);
            var nthis = (left != null && !ReferenceEquals(left, Left)) || (right != null && !ReferenceEquals(right, Right))
                ? this with {
                    Left = left != null && !ReferenceEquals(left, Left) ? left : Left, 
                    Right = right != null && !ReferenceEquals(right, Right) ? right : Right
                }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record RightShift(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(Left.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var left = Left.Transform(func);
            var right = Right.Transform(func);
            var nthis = (left != null && !ReferenceEquals(left, Left)) || (right != null && !ReferenceEquals(right, Right))
                ? this with {
                    Left = left != null && !ReferenceEquals(left, Left) ? left : Left, 
                    Right = right != null && !ReferenceEquals(right, Right) ? right : Right
                }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    
    public record Not(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var value = Value.Transform(func);
            var nthis = value != null && !ReferenceEquals(value, Value)
                ? this with { Value = value }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    
    public record ReverseBits(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var value = Value.Transform(func);
            var nthis = value != null && !ReferenceEquals(value, Value)
                ? this with { Value = value }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record CountLeadingZeros(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var value = Value.Transform(func);
            var nthis = value != null && !ReferenceEquals(value, Value)
                ? this with { Value = value }
                : this;
            return func(nthis) ?? nthis;
        }
    }

    public record LT(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(typeof(bool)) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var left = Left.Transform(func);
            var right = Right.Transform(func);
            var nthis = (left != null && !ReferenceEquals(left, Left)) || (right != null && !ReferenceEquals(right, Right))
                ? this with {
                    Left = left != null && !ReferenceEquals(left, Left) ? left : Left, 
                    Right = right != null && !ReferenceEquals(right, Right) ? right : Right
                }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record LTE(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(typeof(bool)) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var left = Left.Transform(func);
            var right = Right.Transform(func);
            var nthis = (left != null && !ReferenceEquals(left, Left)) || (right != null && !ReferenceEquals(right, Right))
                ? this with {
                    Left = left != null && !ReferenceEquals(left, Left) ? left : Left, 
                    Right = right != null && !ReferenceEquals(right, Right) ? right : Right
                }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record EQ(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(typeof(bool)) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var left = Left.Transform(func);
            var right = Right.Transform(func);
            var nthis = (left != null && !ReferenceEquals(left, Left)) || (right != null && !ReferenceEquals(right, Right))
                ? this with {
                    Left = left != null && !ReferenceEquals(left, Left) ? left : Left, 
                    Right = right != null && !ReferenceEquals(right, Right) ? right : Right
                }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record NE(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(typeof(bool)) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var left = Left.Transform(func);
            var right = Right.Transform(func);
            var nthis = (left != null && !ReferenceEquals(left, Left)) || (right != null && !ReferenceEquals(right, Right))
                ? this with {
                    Left = left != null && !ReferenceEquals(left, Left) ? left : Left, 
                    Right = right != null && !ReferenceEquals(right, Right) ? right : Right
                }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record GTE(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(typeof(bool)) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var left = Left.Transform(func);
            var right = Right.Transform(func);
            var nthis = (left != null && !ReferenceEquals(left, Left)) || (right != null && !ReferenceEquals(right, Right))
                ? this with {
                    Left = left != null && !ReferenceEquals(left, Left) ? left : Left, 
                    Right = right != null && !ReferenceEquals(right, Right) ? right : Right
                }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record GT(StaticIRValue Left, StaticIRValue Right) : StaticIRValue(typeof(bool)) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Left.Walk(func);
            Right.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var left = Left.Transform(func);
            var right = Right.Transform(func);
            var nthis = (left != null && !ReferenceEquals(left, Left)) || (right != null && !ReferenceEquals(right, Right))
                ? this with {
                    Left = left != null && !ReferenceEquals(left, Left) ? left : Left, 
                    Right = right != null && !ReferenceEquals(right, Right) ? right : Right
                }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    
    public record Abs(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var value = Value.Transform(func);
            var nthis = value != null && !ReferenceEquals(value, Value)
                ? this with { Value = value }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record Sqrt(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var value = Value.Transform(func);
            var nthis = value != null && !ReferenceEquals(value, Value)
                ? this with { Value = value }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record Round(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var value = Value.Transform(func);
            var nthis = value != null && !ReferenceEquals(value, Value)
                ? this with { Value = value }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record RoundHalfDown(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var value = Value.Transform(func);
            var nthis = value != null && !ReferenceEquals(value, Value)
                ? this with { Value = value }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record RoundHalfUp(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var value = Value.Transform(func);
            var nthis = value != null && !ReferenceEquals(value, Value)
                ? this with { Value = value }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record Ceil(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var value = Value.Transform(func);
            var nthis = value != null && !ReferenceEquals(value, Value)
                ? this with { Value = value }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record Floor(StaticIRValue Value) : StaticIRValue(Value.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var value = Value.Transform(func);
            var nthis = value != null && !ReferenceEquals(value, Value)
                ? this with { Value = value }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record IsNaN(StaticIRValue Value) : StaticIRValue(typeof(bool)) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var value = Value.Transform(func);
            var nthis = value != null && !ReferenceEquals(value, Value)
                ? this with { Value = value }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    
    public record Dereference(StaticIRValue Pointer, Type Type) : StaticIRValue(Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Pointer.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var pointer = Pointer.Transform(func);
            var nthis = pointer != null && !ReferenceEquals(pointer, Pointer)
                ? this with { Pointer = pointer }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record GetField(StaticIRValue Pointer, string Name, Type Type) : StaticIRValue(Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Pointer.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var pointer = Pointer.Transform(func);
            var nthis = pointer != null && !ReferenceEquals(pointer, Pointer)
                ? this with { Pointer = pointer }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record GetFieldIndex(StaticIRValue Pointer, string Name, int Index, Type Type) : StaticIRValue(Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Pointer.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var pointer = Pointer.Transform(func);
            var nthis = pointer != null && !ReferenceEquals(pointer, Pointer)
                ? this with { Pointer = pointer }
                : this;
            return func(nthis) ?? nthis;
        }
    }

    public record CreateVector(StaticIRValue Value) : StaticIRValue(typeof(Vector128<>).MakeGenericType(Value.Type)) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Value.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var value = Value.Transform(func);
            var nthis = value != null && !ReferenceEquals(value, Value)
                ? this with { Value = value }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record GetElement(StaticIRValue Vector, StaticIRValue Index, Type ElementType) : StaticIRValue(ElementType) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Vector.Walk(func);
            Index.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var vector = Vector.Transform(func);
            var nthis = vector != null && !ReferenceEquals(vector, Vector)
                ? this with { Vector = vector }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record SetElement(StaticIRValue Vector, StaticIRValue Index, StaticIRValue Element) : StaticIRValue(Vector.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Vector.Walk(func);
            Index.Walk(func);
            Element.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var vector = Vector.Transform(func);
            var index = Index.Transform(func);
            var element = Element.Transform(func);
            var nthis = 
                    (vector != null && !ReferenceEquals(vector, Vector)) || 
                    (index != null && !ReferenceEquals(index, Index)) || 
                    (element != null && !ReferenceEquals(element, Element))
                ? this with {
                    Vector = vector != null && !ReferenceEquals(vector, Vector) ? vector : Vector, 
                    Index = index != null && !ReferenceEquals(index, Index) ? index : Index,
                    Element = element != null && !ReferenceEquals(element, Element) ? element : Element
                }
                : this;
            return func(nthis) ?? nthis;
        }
    }

    public record VectorSumUnsigned(StaticIRValue Vector, StaticIRValue ESize, StaticIRValue Count) : StaticIRValue(typeof(ulong)) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Vector.Walk(func);
            ESize.Walk(func);
            Count.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var vector = Vector.Transform(func);
            var esize = ESize.Transform(func);
            var count = Count.Transform(func);
            var nthis = 
                    (vector != null && !ReferenceEquals(vector, Vector)) || 
                    (esize != null && !ReferenceEquals(esize, ESize)) || 
                    (count != null && !ReferenceEquals(count, Count))
                ? this with {
                        Vector = vector != null && !ReferenceEquals(vector, Vector) ? vector : Vector, 
                        ESize = esize != null && !ReferenceEquals(esize, ESize) ? esize : ESize,
                        Count = count != null && !ReferenceEquals(count, Count) ? count : Count
                    }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record VectorCountBits(StaticIRValue Vector, StaticIRValue Elems) : StaticIRValue(typeof(Vector128<float>)) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Vector.Walk(func);
            Elems.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var vector = Vector.Transform(func);
            var elems = Elems.Transform(func);
            var nthis = 
                    (vector != null && !ReferenceEquals(vector, Vector)) || 
                    (elems != null && !ReferenceEquals(elems, Elems))
                ? this with {
                        Vector = vector != null && !ReferenceEquals(vector, Vector) ? vector : Vector, 
                        Elems = elems != null && !ReferenceEquals(elems, Elems) ? elems : Elems
                    }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record VectorExtract(StaticIRValue A, StaticIRValue B, StaticIRValue Q, StaticIRValue Index) : StaticIRValue(typeof(Vector128<float>)) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            A.Walk(func);
            B.Walk(func);
            Q.Walk(func);
            Index.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var a = A.Transform(func);
            var b = B.Transform(func);
            var q = Q.Transform(func);
            var index = Index.Transform(func);
            var nthis = 
                    (a != null && !ReferenceEquals(a, A)) || 
                    (b != null && !ReferenceEquals(b, B)) || 
                    (q != null && !ReferenceEquals(q, Q)) ||
                    (index != null && !ReferenceEquals(index, Index))
                ? this with {
                        A = a != null && !ReferenceEquals(a, A) ? a : A, 
                        B = b != null && !ReferenceEquals(b, B) ? b : B,
                        Q = q != null && !ReferenceEquals(q, Q) ? q : Q,
                        Index = index != null && !ReferenceEquals(index, Index) ? index : Index
                    }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    public record VectorFrsqrte(StaticIRValue Vector, StaticIRValue Bits, StaticIRValue Elems) : StaticIRValue(typeof(Vector128<float>)) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Vector.Walk(func);
            Bits.Walk(func);
            Elems.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var vector = Vector.Transform(func);
            var bits = Bits.Transform(func);
            var elems = Elems.Transform(func);
            var nthis = 
                    (vector != null && !ReferenceEquals(vector, Vector)) ||
                    (bits != null && !ReferenceEquals(bits, Bits)) ||
                    (elems != null && !ReferenceEquals(elems, Elems))
                ? this with {
                        Vector = vector != null && !ReferenceEquals(vector, Vector) ? vector : Vector,
                        Bits = bits != null && !ReferenceEquals(bits, Bits) ? bits : Bits,
                        Elems = elems != null && !ReferenceEquals(elems, Elems) ? elems : Elems
                    }
                : this;
            return func(nthis) ?? nthis;
        }
    }
    
    public record Ternary(StaticIRValue Condition, StaticIRValue True, StaticIRValue False) : StaticIRValue(True.Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            Condition.Walk(func);
            True.Walk(func);
            False.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var condition = Condition.Transform(func);
            var _true = True.Transform(func);
            var _false = False.Transform(func);
            var nthis = 
                (condition != null && !ReferenceEquals(condition, Condition)) || 
                (_true != null && !ReferenceEquals(_true, True)) || 
                (_false != null && !ReferenceEquals(_false, False))
                    ? this with {
                        Condition = condition != null && !ReferenceEquals(condition, Condition) ? condition : Condition, 
                        True = _true != null && !ReferenceEquals(_true, True) ? _true : True,
                        False = _false != null && !ReferenceEquals(_false, False) ? _false : False
                    }
                    : this;
            return func(nthis) ?? nthis;
        }
    }

    public record Phi(IReadOnlyList<StaticIRValue> Values) : StaticIRValue(Values[0].Type) {
        public override void Walk(Action<StaticIRValue> func) {
            func(this);
            foreach(var value in Values)
                value.Walk(func);
        }

        public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
            var changed = false;
            var values = Values.Select(v => {
                var value = func(v);
                if(value == null || ReferenceEquals(value, v)) return v;
                changed = true;
                return value;
            }).ToList();
            var nthis = changed ? new Phi(values) : this;
            return func(nthis) ?? nthis;
        }

        public override string ToString() => $"Phi({string.Join(", ", Values)})";
    }
}