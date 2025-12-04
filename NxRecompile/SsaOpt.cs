using StaticRecompilerBase;

namespace NxRecompile;

public partial class CoreRecompiler {
    void SsaOpt() {
        foreach(var (addr, node) in WholeBlockGraph) {
            // We only want to operate on single blocks && functions
            if(node is not BlockGraph.End || !KnownFunctions.Contains(addr)) continue;

            var temp = new StaticIRStatement.Body(node.Block.Body);
            temp = Ssaify.StripSsa(temp);
            var body = new Ssaify().Transform(temp).Stmts;
            body = PropagateConstants(body);
            body = DeadCodeElimination(body, node.Block.Start);
            body = ((StaticIRStatement.Body) Ssaify.CullPhi(new StaticIRStatement.Body(body))).Stmts;
            node.Block = node.Block with { Body = body };
        }
    }

    List<StaticIRStatement> PropagateConstants(List<StaticIRStatement> body) {
        var values = new Dictionary<(string Name, int Id), StaticIRValue>();
        body.Walk(stmt => {
            if(stmt is not StaticIRStatement.Assign(var name, var value) { SsaId: var id } || 
               id == -1 || value is not StaticIRValue.Literal) return;
            values[(name, id)] = value;
        });

        StaticIRValue Transform(StaticIRValue value) =>
            value is StaticIRValue.Named(var name, _) { SsaId: var id } ? values.GetValueOrDefault((name, id)) : null;
        return body.Transform(stmt => stmt switch {
            StaticIRStatement.SetField => null,
            StaticIRStatement.SetFieldIndex => null,
            StaticIRStatement.Assign or StaticIRStatement.Return or StaticIRStatement.Dereference or
            SvcStmt or LinkedBranch or WriteSrStmt =>
                stmt.TransformValues(Transform),
            StaticIRStatement.While sub => sub with { Cond = sub.Cond.Transform(Transform) ?? sub.Cond },
            StaticIRStatement.DoWhile sub => sub with { Cond = sub.Cond.Transform(Transform) ?? sub.Cond },
            StaticIRStatement.If sub => sub with { Cond = sub.Cond.Transform(Transform) ?? sub.Cond },
            StaticIRStatement.When sub => sub with { Cond = sub.Cond.Transform(Transform) ?? sub.Cond },
            StaticIRStatement.Unless sub => sub with { Cond = sub.Cond.Transform(Transform) ?? sub.Cond },
            _ => null,
        });
    }

    static bool IsEmpty(StaticIRStatement stmt) {
        var any = false;
        stmt.Walk(sub => any |= sub is not StaticIRStatement.Body);
        return !any;
    }

    List<StaticIRStatement> DeadCodeElimination(List<StaticIRStatement> body, ulong addr) {
        while(true) {
            var used = new Dictionary<string, HashSet<int>>();
            var assigned = new Dictionary<string, HashSet<int>>();
            var fromLoad = new Dictionary<string, HashSet<int>>();
            var toStore = new Dictionary<string, HashSet<int>>();
            body.Walk(stmt => {
                if(stmt is StaticIRStatement.Assign(var name, var value) { SsaId: var id }) {
                    var skipRest = false;
                    if(
                        value is StaticIRValue.GetField(StaticIRValue.Named("State", _), _, _) or
                        StaticIRValue.GetFieldIndex(StaticIRValue.Named("State", _), _, _, _)
                    ) {
                        if(!fromLoad.ContainsKey(name)) fromLoad[name] = [];
                        fromLoad[name].Add(id);
                        skipRest = true;
                    }

                    if(!assigned.ContainsKey(name)) assigned[name] = [];
                    assigned[name].Add(id);
                    if(skipRest) return;
                }

                void ValueFunc(StaticIRValue value) {
                    switch(value) {
                        case StaticIRValue.NamedOut(var name, _) { SsaId: var id }:
                            if(!assigned.ContainsKey(name)) assigned[name] = [];
                            assigned[name].Add(id);
                            break;
                        case StaticIRValue.Named(var name, _) { SsaId: var id }:
                            if(!used.ContainsKey(name)) used[name] = [];
                            used[name].Add(id);
                            break;
                    }
                }

                switch(stmt) {
                    case StaticIRStatement.If sub:
                        sub.Cond.Walk(ValueFunc);
                        break;
                    case StaticIRStatement.When sub:
                        sub.Cond.Walk(ValueFunc);
                        break;
                    case StaticIRStatement.Unless sub:
                        sub.Cond.Walk(ValueFunc);
                        break;
                    case StaticIRStatement.While sub:
                        sub.Cond.Walk(ValueFunc);
                        break;
                    case StaticIRStatement.DoWhile sub:
                        sub.Cond.Walk(ValueFunc);
                        break;
                    case IStaticControlFlowStatement:
                        break;
                    case StaticIRStatement.SetField(StaticIRValue.Named("State", _), _, var fvalue): {
                        if(fvalue is not StaticIRValue.Named(var fname, _) { SsaId: var fid })
                            throw new NotSupportedException();
                        if(!toStore.ContainsKey(fname)) toStore[fname] = [];
                        toStore[fname].Add(fid);
                        break;
                    }
                    case StaticIRStatement.SetFieldIndex(StaticIRValue.Named("State", _), _, _, var fvalue): {
                        if(fvalue is not StaticIRValue.Named(var fname, _) { SsaId: var fid })
                            throw new NotSupportedException();
                        if(!toStore.ContainsKey(fname)) toStore[fname] = [];
                        toStore[fname].Add(fid);
                        break;
                    }
                    default:
                        stmt.WalkValues(ValueFunc);
                        break;
                }
            });
            if(false && addr == 0x7100000690) {
                Console.WriteLine("Used:");
                foreach(var (name, set) in used)
                    Console.WriteLine($"\t{name}: {string.Join(", ", set.Order())}");
                Console.WriteLine("Assigned:");
                foreach(var (name, set) in assigned)
                    Console.WriteLine($"\t{name}: {string.Join(", ", set.Order())}");
                Console.WriteLine("To store:");
                foreach(var (name, set) in toStore)
                    Console.WriteLine($"\t{name}: {string.Join(", ", set.Order())}");
            }

            bool IsUsed(string name, int id) => used.ContainsKey(name) && used[name].Contains(id);
            bool IsAssigned(string name, int id) => assigned.ContainsKey(name) && assigned[name].Contains(id);
            bool IsToStore(string name, int id) => toStore.ContainsKey(name) && toStore[name].Contains(id);
            bool IsFromLoad(string name, int id) => fromLoad.ContainsKey(name) && fromLoad[name].Contains(id);
            if(false && addr == 0x7100000690)
                Console.WriteLine(
                    $"temp_103/0: used {IsUsed("temp_103", 0)} assigned {IsAssigned("temp_103", 0)} toStore {IsToStore("temp_103", 0)} fromLoad {IsFromLoad("temp_103", 0)}");
            var obody = body;
            body = body.Transform(stmt => {
                switch(stmt) {
                    case StaticIRStatement.Assign(var name, _) { SsaId: var id } when !IsUsed(name, id): {
                        if(
                            (IsToStore(name, id) && IsFromLoad(name, id)) ||
                            !IsToStore(name, id)
                        )
                            return new StaticIRStatement.Body([]);
                        break;
                    }
                    case StaticIRStatement.SetField(StaticIRValue.Named("State", _), _, StaticIRValue.Named(var name, _) {
                        SsaId: var id
                    }): {
                        if(IsFromLoad(name, id) || !IsAssigned(name, id))
                            return new StaticIRStatement.Body([]);
                        break;
                    }
                    case StaticIRStatement.SetFieldIndex(StaticIRValue.Named("State", _), _, _,
                        StaticIRValue.Named(var name, _) { SsaId: var id }): {
                        if(IsFromLoad(name, id) || !IsAssigned(name, id))
                            return new StaticIRStatement.Body([]);
                        break;
                    }
                    case StaticIRStatement.If(StaticIRValue.Literal(true, _), var leg, _):
                        return leg;
                    case StaticIRStatement.If(StaticIRValue.Literal(false, _), _, var leg):
                        return leg;
                    case StaticIRStatement.When(StaticIRValue.Literal(true, _), var leg):
                        return leg;
                    case StaticIRStatement.When(StaticIRValue.Literal(false, _), _):
                        return new StaticIRStatement.Body([]);
                    case StaticIRStatement.Unless(StaticIRValue.Literal(false, _), var leg):
                        return leg;
                    case StaticIRStatement.Unless(StaticIRValue.Literal(true, _), _):
                        return new StaticIRStatement.Body([]);
                    case StaticIRStatement.While(StaticIRValue.Literal(false, _), _):
                        return new StaticIRStatement.Body([]);
                    case StaticIRStatement.If(_, var left, var right)
                            when IsEmpty(left) && IsEmpty(right):
                        return new StaticIRStatement.Body([]);
                    case StaticIRStatement.If(var cond, var left, var right)
                            when IsEmpty(left):
                        return new StaticIRStatement.Unless(cond, right);
                    case StaticIRStatement.If(var cond, var left, var right)
                            when IsEmpty(right):
                        return new StaticIRStatement.When(cond, left);
                    case StaticIRStatement.When(_, var leg) when IsEmpty(leg):
                        return new StaticIRStatement.Body([]);
                    case StaticIRStatement.Unless(_, var leg) when IsEmpty(leg):
                        return new StaticIRStatement.Body([]);
                }
                return null;
            });
            if(ReferenceEquals(body, obody))
                break;
        }
        return body;
    }
}