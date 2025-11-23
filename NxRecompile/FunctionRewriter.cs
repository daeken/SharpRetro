using System.Diagnostics;
using StaticRecompilerBase;

namespace NxRecompile;

public partial class CoreRecompiler {
    void RewriteFunctions() {
        foreach(var addr in KnownFunctions.Order()) {
            var firstBlock = WholeBlockGraph[addr];
            if(!firstBlock.IsMature() || firstBlock is BlockGraph.End) continue;
            var temp = Rewrite(firstBlock, []);
            // TODO: Remove this bullshit
            temp.Block = temp.Block with { Body = (new List<StaticIRStatement> {
                new DebugStmt(temp.Block.Start, "REWROTE THIS FUNCTION")
            }).Concat(temp.Block.Body).ToList() };
            WholeBlockGraph[addr] = temp;
        }
    }

    BlockGraph Rewrite(BlockGraph node, HashSet<BlockGraph> nexts) {
        if(!WholeBlockGraph.Remove(node.Block.Start)) {
            //Console.WriteLine($"Potentially duplicated block? 0x{node.Block.Start:X}");
            return node; // We already have seen it in the rewrite pass; must be a bad loop
        }
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
            case BlockGraph.TerminalIf dnode: {
                var body = dnode.Block.Body.ToList();
                var ifStmt = (StaticIRStatement.If) body.Last();
                body.RemoveAt(body.Count - 1);
                var newStmt = new StaticIRStatement.If(
                    ifStmt.Cond, 
                    new StaticIRStatement.Body(Rewrite(dnode.Then, nexts).Block.Body),
                    new StaticIRStatement.Body(Rewrite(dnode.Else, nexts).Block.Body));
                body.Add(newStmt);
                return new BlockGraph.End(node.Block with { Body = body });
            }
            case BlockGraph.DoWhile dnode: {
                var body = dnode.Block.Body.ToList();
                var ifStmt = (StaticIRStatement.If) body.Last();
                body.RemoveAt(body.Count - 1);
                if(dnode == dnode.Loop) {
                    body = [new StaticIRStatement.DoWhile(
                                new StaticIRStatement.Body(body),
                                ifStmt.Cond
                            )];
                } else {
                    var newStmt = new StaticIRStatement.DoWhile(
                        new StaticIRStatement.Body(Rewrite(dnode.Loop, nexts.Concat([dnode.Next]).ToHashSet()).Block.Body),
                        ifStmt.Cond);
                    body.Add(newStmt);
                }
                if(!nexts.Contains(dnode.Next))
                    body.AddRange(Rewrite(dnode.Next, nexts).Block.Body);
                return new BlockGraph.End(node.Block with { Body = body });
            }
            default:
                return node;
        }
    }
}