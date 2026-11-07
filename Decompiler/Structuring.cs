namespace SharpRetro.Decompiler;

public abstract record Region;
public sealed record SequenceRegion(IReadOnlyList<Region> Body) : Region;
public sealed record BlockRegion(BasicBlock Block) : Region;
public sealed record IfRegion(BasicBlock Test, Region Then, Region Else) : Region;
public sealed record LoopExit(ulong Target, Region Body);
public sealed record LoopRegion(ulong Header, Region Body, IReadOnlyList<LoopExit>? Exits = null) : Region;
public sealed record BreakRegion(ulong? Target = null) : Region;
public sealed record ContinueRegion : Region;
public sealed record BoundaryRegion(ulong Address) : Region;
public sealed record StructuredProgram(BlockGraph Graph, Region Body, int Loops, int Conditionals, int DuplicatedBlocks = 0);

/// Natural loops + postdominator joins. This pass never guesses a jump's meaning:
/// unsupported graphs retain the dispatch baseline (or refuse in strict mode).
public sealed class Structuring {
    enum JoinPolicy { FirstShared, Postdominator, ClosestShared }
    sealed record NaturalLoop(ulong Header, HashSet<ulong> Members, ulong? Follow, ulong[] Exits);
    readonly BlockGraph graph;
    readonly bool allowMultipleLoopExits;
    readonly bool allowBlockDuplication;
    readonly JoinPolicy joinPolicy;
    readonly Dictionary<ulong, NaturalLoop> loops = [];
    readonly HashSet<ulong> emitted = [];
    readonly Dictionary<ulong, ulong?> joins = [];
    int conditionals, loopCount, duplicatedBlocks;

    Structuring(BlockGraph graph, bool allowMultipleLoopExits, bool allowBlockDuplication, JoinPolicy joinPolicy) {
        this.graph = graph; this.allowMultipleLoopExits = allowMultipleLoopExits;
        this.allowBlockDuplication = allowBlockDuplication;
        this.joinPolicy = joinPolicy;
    }

    public static StructuredProgram Recover(LiftedProgram program) {
        return Recover(BasicBlocks.Build(program));
    }

    public static StructuredProgram Recover(BlockGraph graph, bool allowMultipleLoopExits = false, bool allowBlockDuplication = false) {
        // Bound analysis cost independently from instruction discovery. Bigger
        // inputs can still use the existing baseline; no paths are dropped.
        if(graph.Blocks.Count > 512) throw new StructuringException("Structuring analysis limit: more than 512 basic blocks");
        if(!allowBlockDuplication) return Recover(graph,allowMultipleLoopExits,false,JoinPolicy.FirstShared);
        // A bounded search over representations, never over program behavior.
        // Each candidate starts fresh and must pass the same independent edge
        // validation and expansion bound. Keep the original policy on ties.
        StructuredProgram? best=null;
        (int Copies,int Nodes,int Depth) bestCost=(int.MaxValue,int.MaxValue,int.MaxValue);
        StructuringException? failure=null;
        foreach(var policy in Enum.GetValues<JoinPolicy>()) {
            try {
                var candidate=Recover(graph,allowMultipleLoopExits,true,policy);
                var shape=Shape(candidate.Body);
                var cost=(candidate.DuplicatedBlocks,shape.Nodes,shape.Depth);
                if(cost.CompareTo(bestCost)<0) { best=candidate; bestCost=cost; }
            } catch(StructuringException e) { failure??=e; }
        }
        return best ?? throw failure!;
    }

    static (int Nodes,int Depth) Shape(Region region) {
        IEnumerable<Region> children=region switch {
            SequenceRegion s => s.Body,
            IfRegion i => [i.Then,i.Else],
            LoopRegion l => new[] {l.Body}.Concat(l.Exits?.Select(e=>e.Body) ?? []),
            _ => []
        };
        var shapes=children.Select(Shape).ToArray();
        return (1+shapes.Sum(s=>s.Nodes),1+shapes.Select(s=>s.Depth).DefaultIfEmpty().Max());
    }

    static StructuredProgram Recover(BlockGraph graph,bool allowMultipleLoopExits,bool allowBlockDuplication,JoinPolicy policy) {
        var pass = new Structuring(graph,allowMultipleLoopExits,allowBlockDuplication,policy);
        pass.FindLoops(); pass.FindJoins();
        var body = pass.Sequence(graph.Entry, null, null, false, 0);
        if(pass.emitted.Count != graph.Blocks.Count)
            throw new StructuringException("Structured regions did not account for every reachable block");
        RegionValidation.Check(graph, body,allowBlockDuplication);
        return new(graph, body, pass.loopCount, pass.conditionals,pass.duplicatedBlocks);
    }

    void FindLoops() {
        var all = graph.Blocks.Keys.ToHashSet();
        var dom = all.ToDictionary(a => a, a => a == graph.Entry ? new HashSet<ulong> { a } : new HashSet<ulong>(all));
        bool changed;
        do {
            changed = false;
            foreach(var address in all.Where(a => a != graph.Entry)) {
                var preds = graph.Predecessors[address];
                if(preds.Count == 0) throw new StructuringException("Unreachable block in structuring graph");
                var next = new HashSet<ulong>(dom[preds.First()]);
                foreach(var pred in preds.Skip(1)) next.IntersectWith(dom[pred]);
                next.Add(address);
                if(!dom[address].SetEquals(next)) { dom[address] = next; changed = true; }
            }
        } while(changed);

        var members = new Dictionary<ulong, HashSet<ulong>>();
        foreach(var tail in graph.Blocks.Values)
            foreach(var head in tail.FlowSuccessors.Where(graph.Blocks.ContainsKey)) {
                if(!dom[tail.Address].Contains(head)) continue; // not a dominated back edge
                if(!members.TryGetValue(head, out var loop)) members[head] = loop = [head];
                var pending = new Stack<ulong>();
                if(tail.Address != head && loop.Add(tail.Address)) pending.Push(tail.Address);
                while(pending.TryPop(out var node))
                    foreach(var pred in graph.Predecessors[node])
                        if(pred != head && loop.Add(pred)) pending.Push(pred);
            }
        foreach(var (head, loop) in members) {
            if(loop.Any(a => !dom[a].Contains(head)))
                throw new StructuringException($"Multiple-entry loop at 0x{head:x}");
            var exits = loop.SelectMany(a => graph.Blocks[a].FlowSuccessors).Where(a => !loop.Contains(a)).Distinct().ToArray();
            if(exits.Length > 1 && !allowMultipleLoopExits) throw new StructuringException($"Loop at 0x{head:x} has multiple exit targets");
            loops.Add(head, new(head, loop, exits.Length == 1 ? exits[0] : null, exits));
        }
    }

    void FindJoins() {
        // One synthetic exit for RET and every translated-range boundary. Nodes
        // that cannot reach it must not acquire vacuous universal postdominators.
        var addresses = graph.Blocks.Keys.Order().ToArray();
        var index = addresses.Select((a, i) => (a, i)).ToDictionary(p => p.a, p => p.i);
        int exit = addresses.Length;
        var successors = new int[exit + 1][];
        for(int i = 0; i < exit; i++) {
            var block = graph.Blocks[addresses[i]];
            successors[i] = block.Kind == TerminatorKind.Return ? [exit]
                : block.FlowSuccessors.Select(a => index.GetValueOrDefault(a, exit)).Distinct().ToArray();
        }
        successors[exit] = [];
        var canExit = new HashSet<int> { exit };
        bool changed;
        do {
            changed = false;
            for(int i = 0; i < exit; i++)
                if(!canExit.Contains(i) && successors[i].Any(canExit.Contains)) changed |= canExit.Add(i);
        } while(changed);
        var post = Enumerable.Range(0, exit + 1).Select(i => i == exit || !canExit.Contains(i)
            ? new HashSet<int> { i } : new HashSet<int>(canExit)).ToArray();
        do {
            changed = false;
            for(int i = 0; i < exit; i++) {
                if(!canExit.Contains(i)) continue;
                var next = new HashSet<int>(post[successors[i][0]]);
                foreach(var successor in successors[i].Skip(1)) next.IntersectWith(post[successor]);
                next.Add(i);
                if(!post[i].SetEquals(next)) { post[i] = next; changed = true; }
            }
        } while(changed);
        for(int i = 0; i < exit; i++) {
            // Strict postdominators form a chain; the nearest one has the largest
            // postdominator set. No real join means both arms must finish locally.
            int nearest = post[i].Where(p => p != i).OrderByDescending(p => post[p].Count).FirstOrDefault(exit);
            joins[addresses[i]] = nearest == exit ? null : addresses[nearest];
        }
    }

    Region Sequence(ulong current, ulong? stop, NaturalLoop? context, bool enterHeader, int depth) {
        if(depth > 128) throw new StructuringException("Structured region nesting exceeds 128");
        var result = new List<Region>();
        while(current != stop) {
            if(context != null) {
                if(current == context.Header && !enterHeader) { result.Add(new ContinueRegion()); break; }
                if(!context.Members.Contains(current)) {
                    if(!context.Exits.Contains(current)) throw new StructuringException("Loop edge leaves its supported region");
                    result.Add(new BreakRegion(context.Exits.Length > 1 ? current : null)); break;
                }
            }
            enterHeader = false;
            if(!graph.Blocks.TryGetValue(current, out var block)) { result.Add(new BoundaryRegion(current)); break; }
            if(loops.TryGetValue(current, out var loop) && context?.Header != current) {
                loopCount++;
                var body = Sequence(current, null, loop, true, depth + 1);
                ulong? follow = loop.Follow;
                if(loop.Exits.Length > 1) {
                    // Honor the surrounding region's continuation. Some exits
                    // terminate before it (for example an early RET), so it need
                    // not be reachable from every exit of this loop.
                    follow = stop ?? CommonJoin(loop.Exits, stop, context, loop.Members);
                    var exits = loop.Exits.Select(target=>new LoopExit(target,Sequence(target,follow,context,false,depth+1))).ToArray();
                    result.Add(new LoopRegion(current,body,exits));
                } else result.Add(new LoopRegion(current,body));
                if(follow == null) break;
                current = follow.Value;
                continue;
            }
            if(!emitted.Add(current)) {
                if(!allowBlockDuplication) throw new StructuringException($"Shared or irreducible region at block 0x{current:x}");
                if(++duplicatedBlocks > graph.Blocks.Count * 3)
                    throw new StructuringException("Structured block duplication exceeds expansion limit");
            }
            if(block.Kind == TerminatorKind.Conditional) {
                conditionals++;
                ulong? join = ChooseJoin(block, stop, context);
                var whenTrue = Sequence(block.Target, join, context, false, depth + 1);
                var whenFalse = Sequence(block.Alternative, join, context, false, depth + 1);
                result.Add(new IfRegion(block, whenTrue, whenFalse));
                if(join == null) break;
                current = join.Value;
            } else {
                result.Add(new BlockRegion(block));
                if(block.Kind == TerminatorKind.Return) break;
                current = block.Kind == TerminatorKind.Call ? block.Alternative : block.Target;
            }
        }
        return new SequenceRegion(result);
    }

    ulong? ChooseJoin(BasicBlock block, ulong? stop, NaturalLoop? context) {
        // A break/return arm can bypass a shared continuation, so that continuation
        // need not be a strict postdominator. Find joins within this lexical region,
        // cutting back edges, previously emitted blocks and an enclosing stop.
        // Child regions still have to reach the join or terminate explicitly.
        // The function backend may copy shared exit paths with a bounded source
        // expansion; every copy retains the original effects and checked edges.
        return CommonJoin([block.Target,block.Alternative],stop,context,new HashSet<ulong> { block.Address },joins[block.Address]);
    }

    ulong? CommonJoin(ulong[] starts, ulong? stop, NaturalLoop? context, IReadOnlySet<ulong> excluded, ulong? preferred = null) {
        Dictionary<ulong, int> Reach(ulong start) {
            var distances = new Dictionary<ulong, int>();
            var pending = new Queue<(ulong Address, int Distance)>();
            pending.Enqueue((start, 0));
            while(pending.TryDequeue(out var item)) {
                ulong address = item.Address;
                if(excluded.Contains(address) || (!allowBlockDuplication && emitted.Contains(address)) || !graph.Blocks.ContainsKey(address)) continue;
                if(context != null && !context.Members.Contains(address) && address != context.Follow) continue;
                if(!distances.TryAdd(address, item.Distance)) continue;
                if(address == stop || (context != null && address == context.Follow)) continue;
                foreach(var next in graph.Blocks[address].FlowSuccessors) {
                    if(loops.TryGetValue(next,out var header) && header.Members.Contains(address)) continue;
                    pending.Enqueue((next, item.Distance + 1));
                }
            }
            return distances;
        }
        var distances = starts.Select(Reach).ToArray();
        var common = distances[0].Keys.Where(a=>distances.All(d=>d.ContainsKey(a))).ToHashSet();
        if(joinPolicy==JoinPolicy.Postdominator && preferred is { } post && common.Contains(post)) return post;
        // Prefer the first shared continuation, even when another arm can skip
        // it on its way to a later epilogue. Distance alone can choose the later
        // epilogue and cause the earlier shared block to be emitted twice.
        var downstream = common.ToDictionary(a=>a,a=>Reach(a).Keys.Count(common.Contains));
        ulong? candidate = common
            .OrderBy(a => context != null && !context.Members.Contains(a) ? 1 : 0)
            .ThenByDescending(a => joinPolicy==JoinPolicy.ClosestShared ? 0 : downstream[a])
            .ThenBy(a => distances.Max(d=>d[a]))
            .ThenBy(a => distances.Sum(d=>d[a]))
            .ThenBy(a => a == preferred ? 0 : 1)
            .ThenBy(a => a)
            .Select(a => (ulong?)a).FirstOrDefault();
        return candidate ?? stop;
    }
}
