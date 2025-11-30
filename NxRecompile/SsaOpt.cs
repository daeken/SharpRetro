using StaticRecompilerBase;

namespace NxRecompile;

public partial class CoreRecompiler {
    void SsaOpt() {
        foreach(var (addr, node) in WholeBlockGraph) {
            // We only want to operate on single blocks
            if(node is not BlockGraph.End) continue;

            var body = new Ssaify().Transform(new StaticIRStatement.Body(node.Block.Body));
            body = (StaticIRStatement.Body) Ssaify.CullPhi(body);
            node.Block = node.Block with { Body = body.Stmts };
        }
        //CullPhi();
    }
}