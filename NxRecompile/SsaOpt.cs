using StaticRecompilerBase;

namespace NxRecompile;

public partial class CoreRecompiler {
    void SsaOpt() {
        foreach(var (addr, node) in WholeBlockGraph) {
            // We only want to operate on single blocks && functions
            if(node is not BlockGraph.End || !KnownFunctions.Contains(addr)) continue;

            var body = new Ssaify().Transform(new StaticIRStatement.Body(node.Block.Body)).Stmts;
            body = DeadCodeElimination(body, node.Block.Start);
            body = ((StaticIRStatement.Body) Ssaify.CullPhi(new StaticIRStatement.Body(body))).Stmts;
            node.Block = node.Block with { Body = body };
        }
    }

    List<StaticIRStatement> DeadCodeElimination(List<StaticIRStatement> body, ulong addr) {
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
        if(addr == 0x7100000E10) {
            Console.WriteLine("Used:");
            foreach(var (name, set) in used)
                Console.WriteLine($"\t{name}: {string.Join(", ", set.Order())}");
            Console.WriteLine("Assigned:");
            foreach(var (name, set) in assigned)
                Console.WriteLine($"\t{name}: {string.Join(", ", set.Order())}");
        }
        body = body.Transform(stmt => {
            switch(stmt) {
                case StaticIRStatement.Assign(var name, var avalue) { SsaId: var id }: {
                    if(
                        /*(
                            avalue is StaticIRValue.GetField(StaticIRValue.Named("State", _), _, _) or
                                StaticIRValue.GetFieldIndex(StaticIRValue.Named("State", _), _, _, _)
                            && toStore.TryGetValue(name, out var value) && value.Contains(id)
                        ) ||*/ 
                        !used.TryGetValue(name, out var set) || !set.Contains(id)
                    )
                        return new StaticIRStatement.Body([]);
                    break;
                }
                case StaticIRStatement.SetField(StaticIRValue.Named("State", _), _, StaticIRValue.Named(var name, _) { SsaId: var id }): {
                    if(
                        (fromLoad.TryGetValue(name, out var value) && value.Contains(id)) ||
                        !assigned.TryGetValue(name, out var set) || !set.Contains(id)
                    )
                        return new StaticIRStatement.Body([]);
                    break;
                }
                case StaticIRStatement.SetFieldIndex(StaticIRValue.Named("State", _), _, _, StaticIRValue.Named(var name, _) { SsaId: var id }): {
                    if(
                        (fromLoad.TryGetValue(name, out var value) && value.Contains(id)) ||
                        !assigned.TryGetValue(name, out var set) || !set.Contains(id)
                    )
                        return new StaticIRStatement.Body([]);
                    break;
                }
            }
            return null;
        });
        return body;
    }
}