using StaticRecompilerBase;

namespace NxRecompile;

public partial class CoreRecompiler {
    StaticIRStatement Parentify(StaticIRStatement istmt) {
        switch(istmt) {
            case StaticIRStatement.Body stmt:
                stmt = new(stmt.Stmts.Select(Parentify).ToList());
                foreach(var sub in stmt.Stmts)
                    sub.Parent = stmt;
                return stmt;
            case StaticIRStatement.If stmt:
                stmt = new(
                    stmt.Cond,
                    Parentify(stmt.Then),
                    Parentify(stmt.Else)
                );
                stmt.Then.Parent = stmt;
                stmt.Else.Parent = stmt;
                return stmt;
            case StaticIRStatement.Unless stmt:
                stmt = new(stmt.Cond, Parentify(stmt.Then));
                stmt.Then.Parent = stmt;
                return stmt;
            case StaticIRStatement.When stmt:
                stmt = new(stmt.Cond, Parentify(stmt.Then));
                stmt.Then.Parent = stmt;
                return stmt;
            case StaticIRStatement.While stmt:
                stmt = new(stmt.Cond, Parentify(stmt.Do));
                stmt.Do.Parent = stmt;
                return stmt;
            case StaticIRStatement.DoWhile stmt:
                stmt = new(Parentify(stmt.Do), stmt.Cond);
                stmt.Do.Parent = stmt;
                return stmt;
            default:
                return istmt with { };
        }
    }
}