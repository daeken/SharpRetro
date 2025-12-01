using StaticRecompilerBase;

namespace NxRecompile;

public static class Extensions {
    extension(List<StaticIRStatement> stmts) {
        public void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) =>
            new StaticIRStatement.Body(stmts).Walk(stmtFunc, valueFunc);
        public void Walk(Action<StaticIRStatement> stmtFunc) =>
            new StaticIRStatement.Body(stmts).Walk(stmtFunc);
        public void WalkValues(Action<StaticIRValue> valueFunc) =>
            new StaticIRStatement.Body(stmts).WalkValues(valueFunc);
        public List<StaticIRStatement> Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc, Func<StaticIRValue, StaticIRValue> valueFunc) =>
            ((StaticIRStatement.Body) new StaticIRStatement.Body(stmts).Transform(stmtFunc, valueFunc)).Stmts;

        public List<StaticIRStatement> Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc) =>
            ((StaticIRStatement.Body) new StaticIRStatement.Body(stmts).Transform(stmtFunc)).Stmts;
    }
}