using StaticRecompilerBase;

namespace NxRecompile;

public partial class CoreRecompiler {
    void FindSignatures() {
        foreach(var (addr, node) in WholeBlockGraph) {
            // We only want to operate on single blocks && functions
            if(node is not BlockGraph.End || !KnownFunctions.Contains(addr)) continue;

            var temp = new StaticIRStatement.Body(node.Block.Body);
            temp = Ssaify.StripSsa(temp);
            var body = new Ssaify().Transform(temp).Stmts;
            body = FindSignature(addr, body);
            body = ((StaticIRStatement.Body) Ssaify.CullPhi(new StaticIRStatement.Body(body))).Stmts;
            node.Block = node.Block with { Body = body };
        }
    }

    bool IsSaneFunction(ulong addr) {
        if(
            !KnownFunctions.Contains(addr) || 
            !WholeBlockGraph.TryGetValue(addr, out var node) ||
            node is not BlockGraph.End
        )
            return false;
        var eligible = true;
        node.Block.Body.Walk(stmt => {
            if(!eligible || stmt is not StaticIRStatement.Branch branch) return;
            if(branch.Address is not StaticIRValue.Named("X30", _))
                eligible = false;
        });
        return eligible;
    }
    
    List<StaticIRStatement> FindSignature(ulong addr, List<StaticIRStatement> body) {
        var eligible = true;
        body.Walk(stmt => {
            if(!eligible || stmt is not StaticIRStatement.Branch branch) return;
            if(branch.Address is not StaticIRValue.Named("X30", _))
                eligible = false;
        });
        if(!eligible) return body;

        var inputs = new HashSet<string>();
        var outputs = new HashSet<string>();
        var leftLoads = false;
        body.Walk(stmt => {
            if(leftLoads || stmt is IStaticControlFlowStatement) return;
            if(stmt is StaticIRStatement.Assign(var name, 
               StaticIRValue.GetField(StaticIRValue.Named("State", _), _, _)))
                inputs.Add(name);
            else if(stmt is StaticIRStatement.Assign(var fname,
                      StaticIRValue.GetFieldIndex(StaticIRValue.Named("State", _), _, _, _)))
                inputs.Add(fname);
            else
                leftLoads = true;
        });
        Console.WriteLine($"f_{addr:X}");
        foreach(var input in inputs)
            Console.WriteLine($"\t- In: {input}");
        
        return body;
    }
}