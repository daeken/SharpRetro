namespace StaticRecompilerBase;

public abstract record StaticIRStatement {
    public void Walk(Action<StaticIRStatement> stmtFunc) => Walk(stmtFunc, _ => {});
    public void WalkValues(Action<StaticIRValue> valueFunc) => Walk(_ => {}, valueFunc);
    public abstract void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc);

    public StaticIRStatement Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc) => Transform(stmtFunc, _ => null);
    public StaticIRStatement TransformValues(Func<StaticIRValue, StaticIRValue> valueFunc) => Transform(_ => null, valueFunc);
    public abstract StaticIRStatement Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc, Func<StaticIRValue, StaticIRValue> valueFunc);

    public StaticIRStatement BiTransform(
        Func<StaticIRStatement, StaticIRStatement> stmtEntryFunc,
        Func<StaticIRStatement, StaticIRStatement> stmtExitFunc
    ) => BiTransform(stmtEntryFunc, stmtExitFunc, _ => null);
    public StaticIRStatement BiTransform(
        Func<StaticIRStatement, StaticIRStatement> stmtEntryFunc,
        Func<StaticIRStatement, StaticIRStatement> stmtExitFunc,
        Func<StaticIRValue, StaticIRValue> valueFunc
    ) => (stmtEntryFunc(this) ?? this).Transform(stmtExitFunc, valueFunc);

    public record Body(IReadOnlyList<StaticIRStatement> Stmts) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            foreach(var stmt in Stmts)
                stmt.Walk(stmtFunc, valueFunc);
        }

        public override StaticIRStatement Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc, Func<StaticIRValue, StaticIRValue> valueFunc) {
            var changed = false;
            var stmts = Stmts.Select(stmt => {
                var tstmt = stmt.Transform(stmtFunc, valueFunc);
                if(tstmt == null || ReferenceEquals(stmt, tstmt)) return stmt;
                changed = true;
                return tstmt;
            }).ToList();
            var nthis = changed ? new Body(stmts) : this;
            return stmtFunc(nthis) ?? nthis;
        }
    }

    public record Sink(StaticIRValue Value) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Value.Walk(valueFunc);
        }

        public override StaticIRStatement Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc, Func<StaticIRValue, StaticIRValue> valueFunc) {
            var value = Value.Transform(valueFunc);
            var nthis = value != null && !ReferenceEquals(value, Value)
                ? new Sink(value)
                : this;
            return stmtFunc(nthis) ?? nthis;
        }
    }

    public record Return(StaticIRValue Value) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Value.Walk(valueFunc);
        }

        public override StaticIRStatement Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc, Func<StaticIRValue, StaticIRValue> valueFunc) {
            var value = Value.Transform(valueFunc);
            var nthis = value != null && !ReferenceEquals(value, Value)
                ? new Return(value)
                : this;
            return stmtFunc(nthis) ?? nthis;
        }
    }

    public record If(StaticIRValue Cond, StaticIRStatement Then, StaticIRStatement Else) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Cond.Walk(valueFunc);
            Then.Walk(stmtFunc, valueFunc);
            Else.Walk(stmtFunc, valueFunc);
        }

        public override StaticIRStatement Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc, Func<StaticIRValue, StaticIRValue> valueFunc) {
            var cond = Cond.Transform(valueFunc);
            var then = Then.Transform(stmtFunc, valueFunc);
            var _else = Else.Transform(stmtFunc, valueFunc);
            var nthis =
                    (cond != null && !ReferenceEquals(cond, Cond)) ||
                    (then != null && !ReferenceEquals(then, Then)) ||
                    (_else != null && !ReferenceEquals(_else, Else))
                ? new (
                        cond != null && !ReferenceEquals(cond, Cond) ? cond : Cond,
                        then != null && !ReferenceEquals(then, Then) ? then : Then,
                        _else != null && !ReferenceEquals(_else, Else) ? _else : Else
                    )
                : this;
            return stmtFunc(nthis) ?? nthis;
        }
    }
    public record When(StaticIRValue Cond, StaticIRStatement Then) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Cond.Walk(valueFunc);
            Then.Walk(stmtFunc, valueFunc);
        }

        public override StaticIRStatement Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc, Func<StaticIRValue, StaticIRValue> valueFunc) {
            var cond = Cond.Transform(valueFunc);
            var then = Then.Transform(stmtFunc, valueFunc);
            var nthis =
                    (cond != null && !ReferenceEquals(cond, Cond)) ||
                    (then != null && !ReferenceEquals(then, Then))
                ? new When(
                        cond != null && !ReferenceEquals(cond, Cond) ? cond : Cond,
                        then != null && !ReferenceEquals(then, Then) ? then : Then
                    )
                : this;
            return stmtFunc(nthis) ?? nthis;
        }
    }
    public record Unless(StaticIRValue Cond, StaticIRStatement Then) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Cond.Walk(valueFunc);
            Then.Walk(stmtFunc, valueFunc);
        }

        public override StaticIRStatement Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc, Func<StaticIRValue, StaticIRValue> valueFunc) {
            var cond = Cond.Transform(valueFunc);
            var then = Then.Transform(stmtFunc, valueFunc);
            var nthis =
                    (cond != null && !ReferenceEquals(cond, Cond)) ||
                    (then != null && !ReferenceEquals(then, Then))
                ? new Unless(
                        cond != null && !ReferenceEquals(cond, Cond) ? cond : Cond,
                        then != null && !ReferenceEquals(then, Then) ? then : Then
                    )
                : this;
            return stmtFunc(nthis) ?? nthis;
        }
    }
    public record While(StaticIRValue Cond, StaticIRStatement Do) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Cond.Walk(valueFunc);
            Do.Walk(stmtFunc, valueFunc);
        }

        public override StaticIRStatement Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc, Func<StaticIRValue, StaticIRValue> valueFunc) {
            var cond = Cond.Transform(valueFunc);
            var _do = Do.Transform(stmtFunc, valueFunc);
            var nthis =
                    (cond != null && !ReferenceEquals(cond, Cond)) ||
                    (_do != null && !ReferenceEquals(_do, Do))
                ? new While(
                        cond != null && !ReferenceEquals(cond, Cond) ? cond : Cond, 
                        _do != null && !ReferenceEquals(_do, Do) ? _do : Do
                    )
                : this;
            return stmtFunc(nthis) ?? nthis;
        }
    }
    public record DoWhile(StaticIRStatement Do, StaticIRValue Cond) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Do.Walk(stmtFunc, valueFunc);
            Cond.Walk(valueFunc);
        }

        public override StaticIRStatement Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc, Func<StaticIRValue, StaticIRValue> valueFunc) {
            var _do = Do.Transform(stmtFunc, valueFunc);
            var cond = Cond.Transform(valueFunc);
            var nthis =
                    (_do != null && !ReferenceEquals(_do, Do)) ||
                    (cond != null && !ReferenceEquals(cond, Cond))
                ? new DoWhile(
                        _do != null && !ReferenceEquals(_do, Do) ? _do : Do, 
                        cond != null && !ReferenceEquals(cond, Cond) ? cond : Cond
                    )
                : this;
            return stmtFunc(nthis) ?? nthis;
        }
    }
    
    public record Dereference(StaticIRValue Pointer, StaticIRValue Value) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Pointer.Walk(valueFunc);
            Value.Walk(valueFunc);
        }

        public override StaticIRStatement Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc, Func<StaticIRValue, StaticIRValue> valueFunc) {
            var pointer = Pointer.Transform(valueFunc);
            var value = Value.Transform(valueFunc);
            var nthis =
                (pointer != null && !ReferenceEquals(pointer, Pointer)) ||
                (value != null && !ReferenceEquals(value, Value))
                    ? new Dereference(
                            pointer != null && !ReferenceEquals(pointer, Pointer) ? pointer : Pointer,
                            value != null && !ReferenceEquals(value, Value) ? value : Value
                        )
                    : this;
            return stmtFunc(nthis) ?? nthis;
        }
    }
    public record SetField(StaticIRValue Pointer, string Name, StaticIRValue Value) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Pointer.Walk(valueFunc);
            Value.Walk(valueFunc);
        }

        public override StaticIRStatement Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc, Func<StaticIRValue, StaticIRValue> valueFunc) {
            var pointer = Pointer.Transform(valueFunc);
            var value = Value.Transform(valueFunc);
            var nthis =
                    (pointer != null && !ReferenceEquals(pointer, Pointer)) ||
                    (value != null && !ReferenceEquals(value, Value))
                ? new SetField(
                        pointer != null && !ReferenceEquals(pointer, Pointer) ? pointer : Pointer,
                        Name,
                        value != null && !ReferenceEquals(value, Value) ? value : Value
                    )
                : this;
            return stmtFunc(nthis) ?? nthis;
        }
    }
    public record SetFieldIndex(StaticIRValue Pointer, string Name, int Index, StaticIRValue Value) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Pointer.Walk(valueFunc);
            Value.Walk(valueFunc);
        }

        public override StaticIRStatement Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc, Func<StaticIRValue, StaticIRValue> valueFunc) {
            var pointer = Pointer.Transform(valueFunc);
            var value = Value.Transform(valueFunc);
            var nthis =
                    (pointer != null && !ReferenceEquals(pointer, Pointer)) ||
                    (value != null && !ReferenceEquals(value, Value))
                ? new SetFieldIndex(
                        pointer != null && !ReferenceEquals(pointer, Pointer) ? pointer : Pointer,
                        Name,
                        Index, 
                        value != null && !ReferenceEquals(value, Value) ? value : Value
                    )
                : this;
            return stmtFunc(nthis) ?? nthis;
        }
    }

    public record Assign(string Name, StaticIRValue Value) : StaticIRStatement {
        public int SsaId = -1;
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Value.Walk(valueFunc);
        }

        public override StaticIRStatement Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc, Func<StaticIRValue, StaticIRValue> valueFunc) {
            var value = Value.Transform(valueFunc);
            var nthis = value != null && !ReferenceEquals(value, Value)
                ? new Assign(Name, value)
                : this;
            return stmtFunc(nthis) ?? nthis;
        }
    }
    
    public record Branch(StaticIRValue Address) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Address.Walk(valueFunc);
        }

        public override StaticIRStatement Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc, Func<StaticIRValue, StaticIRValue> valueFunc) {
            var address = Address.Transform(valueFunc);
            var nthis = address != null && !ReferenceEquals(address, Address)
                ? new Branch(address)
                : this;
            return stmtFunc(nthis) ?? nthis;
        }
    }
}