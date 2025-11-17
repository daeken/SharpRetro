namespace StaticRecompilerBase;

public abstract record StaticIRStatement {
    public void Walk(Action<StaticIRStatement> stmtFunc) => Walk(stmtFunc, _ => {});
    public void WalkValues(Action<StaticIRValue> valueFunc) => Walk(_ => {}, valueFunc);
    public abstract void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc);
    
    public record Body(IReadOnlyList<StaticIRStatement> Stmts) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            foreach(var stmt in Stmts)
                stmt.Walk(stmtFunc, valueFunc);
        }
    }

    public record Sink(StaticIRValue Value) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Value.Walk(valueFunc);
        }
    }

    public record Return(StaticIRValue Value) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Value.Walk(valueFunc);
        }
    }

    public record If(StaticIRValue Cond, StaticIRStatement Then, StaticIRStatement Else) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Cond.Walk(valueFunc);
            Then.Walk(stmtFunc, valueFunc);
            Else.Walk(stmtFunc, valueFunc);
        }
    }
    public record When(StaticIRValue Cond, StaticIRStatement Then) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Cond.Walk(valueFunc);
            Then.Walk(stmtFunc, valueFunc);
        }
    }
    public record Unless(StaticIRValue Cond, StaticIRStatement Then) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Cond.Walk(valueFunc);
            Then.Walk(stmtFunc, valueFunc);
        }
    }
    public record While(StaticIRValue Cond, StaticIRStatement Do) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Cond.Walk(valueFunc);
            Do.Walk(stmtFunc, valueFunc);
        }
    }
    public record DoWhile(StaticIRStatement Do, StaticIRValue Cond) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Do.Walk(stmtFunc, valueFunc);
            Cond.Walk(valueFunc);
        }
    }
    
    public record Dereference(StaticIRValue Pointer, StaticIRValue Value) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Pointer.Walk(valueFunc);
            Value.Walk(valueFunc);
        }
    }
    public record SetField(StaticIRValue Pointer, string Name, StaticIRValue Value) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Pointer.Walk(valueFunc);
            Value.Walk(valueFunc);
        }
    }
    public record SetFieldIndex(StaticIRValue Pointer, string Name, int Index, StaticIRValue Value) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Pointer.Walk(valueFunc);
            Value.Walk(valueFunc);
        }
    }
    
    public record Branch(StaticIRValue Address) : StaticIRStatement {
        public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
            stmtFunc(this);
            Address.Walk(valueFunc);
        }
    }
}