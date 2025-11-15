namespace StaticRecompilerBase;

public abstract record StaticIRStatement {
    public record Body(IReadOnlyList<StaticIRStatement> Stmts) : StaticIRStatement;
    
    public record Sink(StaticIRValue Value) : StaticIRStatement;
    public record Return(StaticIRValue Value) : StaticIRStatement;

    public record If(StaticIRValue Cond, StaticIRStatement Then, StaticIRStatement Else) : StaticIRStatement;
    public record When(StaticIRValue Cond, StaticIRStatement Then) : StaticIRStatement;
    public record Unless(StaticIRValue Cond, StaticIRStatement Then) : StaticIRStatement;
    public record While(StaticIRValue Cond, StaticIRStatement Do) : StaticIRStatement;
    public record DoWhile(StaticIRStatement Do, StaticIRValue Cond) : StaticIRStatement;
    
    public record Dereference(StaticIRValue Pointer, StaticIRValue Value) : StaticIRStatement;
}