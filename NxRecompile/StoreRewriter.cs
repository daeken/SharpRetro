using System.Diagnostics;
using StaticRecompilerBase;

namespace NxRecompile;

public partial class CoreRecompiler {
    void RewriteStores() =>
        AllInstructions =
            AllInstructions.Select(module =>
                module.Select(insn =>
                    insn == null ? null : RewriteStores(insn)).ToArray()).ToArray();

    int TempI;
    List<StaticIRStatement> RewriteStores(List<StaticIRStatement> stmts) {
        var stores = new Dictionary<StaticIRValue.Store, (string Name, StaticIRValue.Named Value)>();
        stmts.WalkValues(value => {
            if(value is not StaticIRValue.Store store || stores.ContainsKey(store)) return;
            var name = $"temp_{TempI++}";
            stores[store] = (name, new StaticIRValue.Named(name, value.Type));
        });
        if(stores.Count == 0) return stmts;
        var usage = stores.Select(x => (x.Key, 0)).ToDictionary();
        stmts.WalkValues(value => {
            if(value is StaticIRValue.Store store) usage[store]++;
        });
        foreach(var (store, count) in usage)
            if(count == 1)
                stores.Remove(store);
        var assigned = new HashSet<string>();
        stmts = stmts.Transform(stmt => {
            if(stmt is IStaticControlFlowStatement) return null;
            var containsStore = false;
            stmt.WalkValues(value => { if(value is StaticIRValue.Store) containsStore = true; });
            if(!containsStore) return null;
            var assignments = new List<StaticIRStatement>();
            stmt = stmt.TransformValues(value => {
                if(value is not StaticIRValue.Store store) return null;
                if(!stores.TryGetValue(store, out var temp)) return store.Value;
                var (name, named) = temp;
                if(assigned.Add(name))
                    assignments.Add(new StaticIRStatement.Assign(name, store.Value));
                return named;
            });
            return assignments.Count == 0 ? stmt : new StaticIRStatement.Body(assignments.Concat([stmt]).ToList());
        });
        return stmts;
    }
}