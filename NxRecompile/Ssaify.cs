using StaticRecompilerBase;

namespace NxRecompile;

public class Ssaify {
    readonly Dictionary<string, Type> NameTypes = [];
    readonly Dictionary<string, int> NextIds = [];
    readonly Stack<Dictionary<string, int>> ScopeStack = new([new()]);
    
    int GetNextId(string name, Type type) {
        NameTypes[name] = type;
        return ScopeStack.Peek()[name] = NextIds[name] = NextIds.TryGetValue(name, out var id) ? id + 1 : 0;
    }

    int GetCurrentId(string name, Type type) {
        NameTypes[name] = type;
        return ScopeStack.Peek().TryGetValue(name, out var id) ? id : GetNextId(name, type);
    }

    void PushScope() => ScopeStack.Push(ScopeStack.Peek().ToDictionary());
    Dictionary<string, int> PopScope() => ScopeStack.Pop();
    Dictionary<string, int> PeekScope() => ScopeStack.Peek();

    static bool ScopeEqual(Dictionary<string, int> a, Dictionary<string, int> b) {
        if(a.Count != b.Count) return false;
        if(!a.Keys.Order().SequenceEqual(b.Keys.Order())) return false;
        return a.All(kv => b[kv.Key] == kv.Value);
    }

    StaticIRValue Transform(StaticIRValue value) =>
        value is StaticIRValue.Named(var name, var type) named ? named with { SsaId = GetCurrentId(name, type) } : value;

    public StaticIRStatement Transform(StaticIRStatement istmt) =>
        istmt switch {
            BreakpointStmt stmt => stmt.TransformValues(Transform),
            DebugStmt stmt => stmt.TransformValues(Transform),
            LinkedBranch stmt => stmt.TransformValues(Transform),
            SvcStmt stmt => Transform(stmt),
            WriteSrStmt stmt => stmt.TransformValues(Transform),
            StaticIRStatement.Assign stmt => Transform(stmt),
            StaticIRStatement.Body stmt => Transform(stmt),
            StaticIRStatement.Branch stmt => stmt.TransformValues(Transform),
            StaticIRStatement.Dereference stmt => stmt.TransformValues(Transform),
            StaticIRStatement.DoWhile stmt => Transform(stmt),
            StaticIRStatement.If stmt => Transform(stmt),
            StaticIRStatement.Return stmt => stmt.TransformValues(Transform),
            StaticIRStatement.SetField stmt => stmt.TransformValues(Transform),
            StaticIRStatement.SetFieldIndex stmt => stmt.TransformValues(Transform),
            StaticIRStatement.Sink stmt => stmt.TransformValues(Transform),
            StaticIRStatement.Unless stmt => Transform(stmt),
            StaticIRStatement.When stmt => Transform(stmt),
            StaticIRStatement.While stmt => Transform(stmt),
            _ => throw new NotImplementedException($"Unknown stmt for Ssaify: {istmt}"),
        };

    public StaticIRStatement.Body Transform(StaticIRStatement.Body body) => new(body.Stmts.Select(Transform).ToList());
    StaticIRStatement.Assign Transform(StaticIRStatement.Assign stmt) =>
        new(stmt.Name, stmt.Value.Transform(Transform)) { SsaId = GetNextId(stmt.Name, stmt.Value.Type) };

    StaticIRStatement WithPhi(StaticIRStatement stmt, Dictionary<string, int> aScope, Dictionary<string, int> bScope) {
        if(ScopeEqual(aScope, bScope)) return stmt;
        List<StaticIRStatement> body = [stmt];
        void AddPhi(string name, int a, int b) =>
            body.Add(new StaticIRStatement.Assign(name, new StaticIRValue.Phi([ 
                new StaticIRValue.Named(name, NameTypes[name]) { SsaId = a },
                new StaticIRValue.Named(name, NameTypes[name]) { SsaId = b },
            ])) { SsaId = GetNextId(name, NameTypes[name]) });
        foreach(var key in aScope.Keys.Union(bScope.Keys)) {
            if(!aScope.TryGetValue(key, out var aId))
                AddPhi(key, bScope[key], -2);
            else if(!bScope.TryGetValue(key, out var bId))
                AddPhi(key, aId, -2);
            else if(aId != bId)
                AddPhi(key, aId, bId);
        }
        return new StaticIRStatement.Body(body);
    }
    
    StaticIRStatement Transform(StaticIRStatement.If stmt) {
        var cond = stmt.Cond.Transform(Transform);
        PushScope();
        var then = Transform(stmt.Then);
        var thenScope = PopScope();
        PushScope();
        var _else =  Transform(stmt.Else);
        var elseScope = PopScope();
        return WithPhi(new StaticIRStatement.If(cond, then, _else), thenScope, elseScope);
    }
    StaticIRStatement Transform(StaticIRStatement.When stmt) {
        var cond = stmt.Cond.Transform(Transform);
        PushScope();
        var then = Transform(stmt.Then);
        var thenScope = PopScope();
        return WithPhi(new StaticIRStatement.When(cond, then), thenScope, PeekScope());
    }
    StaticIRStatement Transform(StaticIRStatement.Unless stmt) {
        var cond = stmt.Cond.Transform(Transform);
        PushScope();
        var then = Transform(stmt.Then);
        var thenScope = PopScope();
        return WithPhi(new StaticIRStatement.Unless(cond, then), thenScope, PeekScope());
    }
    StaticIRStatement Transform(StaticIRStatement.While stmt) {
        var cond = stmt.Cond.Transform(Transform);
        PushScope();
        var loop = Transform(stmt.Do);
        var thenScope = PopScope();
        return WithPhi(new StaticIRStatement.While(cond, loop), thenScope, PeekScope());
    }
    StaticIRStatement Transform(StaticIRStatement.DoWhile stmt) {
        PushScope();
        var loop = Transform(stmt.Do);
        var cond = stmt.Cond.Transform(Transform);
        var thenScope = PopScope();
        return WithPhi(new StaticIRStatement.DoWhile(loop, cond), thenScope, PeekScope());
    }
    SvcStmt Transform(SvcStmt stmt) {
        return stmt;
    }
}