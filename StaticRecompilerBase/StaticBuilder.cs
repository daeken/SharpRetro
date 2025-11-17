using JitBase;

namespace StaticRecompilerBase;

public class StaticBuilder<AddrT> : IBuilder<AddrT> where AddrT : struct {
    public readonly List<StaticIRStatement> BodyStmts = [];
    readonly Stack<List<StaticIRStatement>> ScopeStack = new();
    public StaticBuilder() => ScopeStack.Push(BodyStmts);
    public StaticIRStatement.Body Body => new(BodyStmts);

    StaticIRStatement.Body Scoped(Action func) {
        ScopeStack.Push([]);
        func();
        return new(ScopeStack.Pop());
    }
    public void Add(StaticIRStatement stmt) => ScopeStack.Peek().Add(stmt);

    public static StaticIRValue W<T>(IRuntimeValue<T> irv) where T : struct =>
        irv is StaticRuntimeValue<T> srv ? srv.Value : throw new();
    public static StaticRuntimeValue<T> W<T>(StaticIRValue siv) where T : struct => new(siv);
    
    public IRuntimeValue<T> Argument<T>(int index) where T : struct => throw new NotImplementedException();

    public IStructRef<T> StructRefArgument<T>(int index) where T : IJitStruct => throw new NotImplementedException();

    public IRuntimeValue<T> Zero<T>() where T : struct => LiteralValue(default(T));
    public IRuntimeValue<T> LiteralValue<T>(T value) where T : struct =>
        new StaticRuntimeValue<T>(new StaticIRValue.Literal(value, typeof(T)));

    public IRuntimePointer<AddrT, T> Pointer<T>(IRuntimeValue<AddrT> pointer) where T : struct =>
        new StaticRuntimePointer<AddrT, T>(this, pointer);

    public ILocalVar<T> DefineLocal<T>() where T : struct => throw new NotImplementedException();

    public void Sink<T>(IRuntimeValue<T> value) where T : struct => Add(new StaticIRStatement.Sink(W(value)));

    public void Return<T>(IRuntimeValue<T> value) where T : struct => Add(new StaticIRStatement.Return(W(value)));

    public void If(IRuntimeValue<bool> cond, Action if_, Action else_) =>
        Add(new StaticIRStatement.If(
            W(cond),
            Scoped(if_),
            Scoped(else_)
        ));

    public void When(IRuntimeValue<bool> cond, Action when) =>
        Add(new StaticIRStatement.When(
            W(cond),
            Scoped(when)
        ));

    public void Unless(IRuntimeValue<bool> cond, Action unless) =>
        Add(new StaticIRStatement.Unless(
            W(cond),
            Scoped(unless)
        ));

    public void While(IRuntimeValue<bool> cond, Action body) =>
        Add(new StaticIRStatement.While(
            W(cond),
            Scoped(body)
        ));

    public void DoWhile(Action body, IRuntimeValue<bool> cond) =>
        Add(new StaticIRStatement.DoWhile(
            Scoped(body),
            W(cond)
        ));

    public IRuntimeValue<T> Ternary<T>(IRuntimeValue<bool> cond, IRuntimeValue<T> a, IRuntimeValue<T> b) where T : struct =>
        W<T>(new StaticIRValue.Ternary(W(cond), W(a), W(b)));

    public void CallVoid(Action func) {
        throw new NotImplementedException();
    }

    public void CallVoid<T1>(Action<T1> func, IRuntimeValue<T1> a1) where T1 : struct {
        throw new NotImplementedException();
    }

    public void CallVoid<T1, T2>(Action<T1, T2> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2) where T1 : struct where T2 : struct {
        throw new NotImplementedException();
    }

    public void CallVoid<T1, T2, T3>(Action<T1, T2, T3> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3) where T1 : struct where T2 : struct where T3 : struct {
        throw new NotImplementedException();
    }

    public void CallVoid<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3, IRuntimeValue<T4> a4) where T1 : struct where T2 : struct where T3 : struct where T4 : struct {
        throw new NotImplementedException();
    }

    public void CallVoid<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3, IRuntimeValue<T4> a4,
        IRuntimeValue<T5> a5) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct {
        throw new NotImplementedException();
    }

    public void CallVoid<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3,
        IRuntimeValue<T4> a4, IRuntimeValue<T5> a5, IRuntimeValue<T6> a6) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct {
        throw new NotImplementedException();
    }

    public IRuntimeValue<RetT> Call<RetT>(Func<RetT> func) where RetT : struct => throw new NotImplementedException();

    public IRuntimeValue<RetT> Call<T1, RetT>(Func<T1, RetT> func, IRuntimeValue<T1> a1) where T1 : struct where RetT : struct => throw new NotImplementedException();

    public IRuntimeValue<RetT> Call<T1, T2, RetT>(Func<T1, T2, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2) where T1 : struct where T2 : struct where RetT : struct => throw new NotImplementedException();

    public IRuntimeValue<RetT> Call<T1, T2, T3, RetT>(Func<T1, T2, T3, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3) where T1 : struct where T2 : struct where T3 : struct where RetT : struct => throw new NotImplementedException();

    public IRuntimeValue<RetT> Call<T1, T2, T3, T4, RetT>(Func<T1, T2, T3, T4, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3,
        IRuntimeValue<T4> a4) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where RetT : struct =>
        throw new NotImplementedException();

    public IRuntimeValue<RetT> Call<T1, T2, T3, T4, T5, RetT>(Func<T1, T2, T3, T4, T5, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3,
        IRuntimeValue<T4> a4, IRuntimeValue<T5> a5) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where RetT : struct =>
        throw new NotImplementedException();

    public IRuntimeValue<RetT> Call<T1, T2, T3, T4, T5, T6, RetT>(Func<T1, T2, T3, T4, T5, T6, RetT> func, IRuntimeValue<T1> a1, IRuntimeValue<T2> a2, IRuntimeValue<T3> a3,
        IRuntimeValue<T4> a4, IRuntimeValue<T5> a5, IRuntimeValue<T6> a6) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct where RetT : struct =>
        throw new NotImplementedException();

    public IRuntimeValue<T> Dereference<T>(IRuntimeValue<AddrT> addr) where T : struct =>
        new StaticRuntimeValue<T>(new StaticIRValue.Dereference(W(addr), typeof(T)));
    public void Dereference<T>(IRuntimeValue<AddrT> addr, IRuntimeValue<T> value) where T : struct =>
        Add(new StaticIRStatement.Dereference(W(addr), W(value)));

    public StaticRuntimeValue<T> GetField<T>(IRuntimeValue<AddrT> addr, string name) where T : struct =>
        W<T>(new StaticIRValue.GetField(W(addr), name, typeof(T)));
    public StaticRuntimeValue<T> GetFieldIndex<T>(IRuntimeValue<AddrT> addr, string name, int index) where T : struct =>
        W<T>(new StaticIRValue.GetFieldIndex(W(addr), name, index, typeof(T)));

    public void SetField<T>(IRuntimeValue<AddrT> addr, string name, IRuntimeValue<T> value) where T : struct =>
        Add(new StaticIRStatement.SetField(W(addr), name, W(value)));
    public void SetFieldIndex<T>(IRuntimeValue<AddrT> addr, string name, int index, IRuntimeValue<T> value) where T : struct =>
        Add(new StaticIRStatement.SetFieldIndex(W(addr), name, index, W(value)));
}