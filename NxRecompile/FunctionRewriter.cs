using System.Diagnostics;
using StaticRecompilerBase;

namespace NxRecompile;

public partial class CoreRecompiler {
    void RewriteFunctions() {
        foreach(var addr in KnownFunctions.Order()) {
            var firstBlock = WholeBlockGraph[addr];
            if(!IsMature(firstBlock) || firstBlock is BlockGraph.End) continue;
            var temp = Rewrite(firstBlock, []);
            // TODO: Remove this bullshit
            temp.Block = temp.Block with { Body = (new List<StaticIRStatement> {
                new DebugStmt(temp.Block.Start, "REWROTE THIS FUNCTION")
            }).Concat(temp.Block.Body).ToList() };
            WholeBlockGraph[addr] = temp;
        }
    }

    BlockGraph Rewrite(BlockGraph node, HashSet<BlockGraph> nexts) {
        if(!WholeBlockGraph.Remove(node.Block.Start))
            return node; // We already have seen it in the rewrite pass; must be a bad loop
        switch(node) {
            case BlockGraph.End: return node;
            case BlockGraph.Unconditional dnode: {
                var body = dnode.Block.Body.ToList();
                body.RemoveAt(body.Count - 1);
                if(!nexts.Contains(dnode.Next))
                    body.AddRange(Rewrite(dnode.Next, nexts).Block.Body);
                return new BlockGraph.End(node.Block with { Body = body });
            }
            case BlockGraph.When dnode: {
                var body = dnode.Block.Body.ToList();
                var ifStmt = (StaticIRStatement.If) body.Last();
                body.RemoveAt(body.Count - 1);
                var newStmt = new StaticIRStatement.When(
                    ifStmt.Cond, 
                    new StaticIRStatement.Body(Rewrite(dnode.Then, nexts.Concat([dnode.Next]).ToHashSet()).Block.Body));
                body.Add(newStmt);
                if(!nexts.Contains(dnode.Next))
                    body.AddRange(Rewrite(dnode.Next, nexts).Block.Body);
                return new BlockGraph.End(node.Block with { Body = body });
            }
            case BlockGraph.Unless dnode: {
                var body = dnode.Block.Body.ToList();
                var ifStmt = (StaticIRStatement.If) body.Last();
                body.RemoveAt(body.Count - 1);
                var newStmt = new StaticIRStatement.Unless(
                    ifStmt.Cond, 
                    new StaticIRStatement.Body(Rewrite(dnode.Then, nexts.Concat([dnode.Next]).ToHashSet()).Block.Body));
                body.Add(newStmt);
                if(!nexts.Contains(dnode.Next))
                    body.AddRange(Rewrite(dnode.Next, nexts).Block.Body);
                return new BlockGraph.End(node.Block with { Body = body });
            }
            case BlockGraph.If dnode: {
                var body = dnode.Block.Body.ToList();
                var ifStmt = (StaticIRStatement.If) body.Last();
                body.RemoveAt(body.Count - 1);
                var newStmt = new StaticIRStatement.If(
                    ifStmt.Cond, 
                    new StaticIRStatement.Body(Rewrite(dnode.Then, nexts.Concat([dnode.Next]).ToHashSet()).Block.Body),
                    new StaticIRStatement.Body(Rewrite(dnode.Else, nexts.Concat([dnode.Next]).ToHashSet()).Block.Body));
                body.Add(newStmt);
                if(!nexts.Contains(dnode.Next))
                    body.AddRange(Rewrite(dnode.Next, nexts).Block.Body);
                return new BlockGraph.End(node.Block with { Body = body });
            }
            default:
                return node;
        }
    }

    bool IsMature(BlockGraph node) {
        var seen = new HashSet<BlockGraph>();
        var checkQueue = new Queue<BlockGraph>();
        checkQueue.Enqueue(node);
        void Check(BlockGraph next) {
            if(!seen.Contains(next) && !checkQueue.Contains(next))
                checkQueue.Enqueue(next);
        }
        while(checkQueue.TryDequeue(out node)) {
            if(seen.Contains(node)) continue;
            seen.Add(node);
            switch(node) {
                case BlockGraph.Conditional: return false;
                case BlockGraph.Unconditional uc:
                    Check(uc.Next);
                    break;
                case BlockGraph.If _if:
                    Check(_if.Then);
                    Check(_if.Else);
                    Check(_if.Next);
                    break;
                case BlockGraph.When _when:
                    Check(_when.Then);
                    Check(_when.Next);
                    break;
                case BlockGraph.Unless _unless:
                    Check(_unless.Then);
                    Check(_unless.Next);
                    break;
                case BlockGraph.While _while:
                    Check(_while.Loop);
                    Check(_while.Next);
                    break;
                case BlockGraph.InverseWhile iwhile:
                    Check(iwhile.Loop);
                    Check(iwhile.Next);
                    break;
            }
        }
        return true;
    }
}