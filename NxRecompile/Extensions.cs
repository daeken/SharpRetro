using StaticRecompilerBase;

namespace NxRecompile;

public static class Extensions {
    extension(List<StaticIRStatement> stmts) {
        public List<StaticIRStatement> Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc, Func<StaticIRValue, StaticIRValue> valueFunc) =>
            ((StaticIRStatement.Body) new StaticIRStatement.Body(stmts).Transform(stmtFunc, valueFunc)).Stmts;

        public List<StaticIRStatement> Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc) =>
            ((StaticIRStatement.Body) new StaticIRStatement.Body(stmts).Transform(stmtFunc)).Stmts;
    }
}