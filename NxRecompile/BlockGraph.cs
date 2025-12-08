using StaticRecompilerBase;
namespace NxRecompile;
using Body = List<StaticIRStatement>;

public record Block(ulong Start, Body Body);

public abstract class BlockGraph(Block Block) {
    public Block Block = Block;
    public readonly HashSet<BlockGraph> Prevs = [];
    public abstract IEnumerable<BlockGraph> LeadsTo { get; }

    public void Walk(Action<BlockGraph> func) => Walk(func, []);
    public abstract void Walk(Action<BlockGraph> func, HashSet<ulong> seen);
    
    public abstract void SwapNode(BlockGraph prev, BlockGraph next);
    
    // Immature
    public class Unconditional(Block Block, BlockGraph Next = null) : BlockGraph(Block) {
        public BlockGraph Next = Next;
        public override string ToString() => $"Unconditional(0x{(Next != null ? Next.Block.Start : 0):X})";
        public override IEnumerable<BlockGraph> LeadsTo {
            get { yield return Next; }
        }
        public override void Walk(Action<BlockGraph> func, HashSet<ulong> seen) {
            if(!seen.Add(Block.Start)) return;
            func(this);
            Next.Walk(func, seen);
        }
        public override void SwapNode(BlockGraph prev, BlockGraph next) {
            if(ReferenceEquals(Next, prev)) Next = next;
        }
    }
    public class Conditional(Block Block, BlockGraph Taken = null, BlockGraph Not = null) : BlockGraph(Block) {
        public BlockGraph Taken = Taken;
        public BlockGraph Not = Not;
        public override string ToString() => $"Conditional(0x{(Taken != null ? Taken.Block.Start : 0):X}, 0x{(Not != null ? Not.Block.Start : 0):X})";
        public override IEnumerable<BlockGraph> LeadsTo {
            get {
                yield return Taken;
                yield return Not;
            }
        }
        public override void Walk(Action<BlockGraph> func, HashSet<ulong> seen) {
            if(!seen.Add(Block.Start)) return;
            func(this);
            Taken.Walk(func, seen);
            Not.Walk(func, seen);
        }
        public override void SwapNode(BlockGraph prev, BlockGraph next) {
            if(ReferenceEquals(Taken, prev)) Taken = next;
            if(ReferenceEquals(Not, prev)) Not = next;
        }
    }

    public class End(Block Block) : BlockGraph(Block) {
        public override IEnumerable<BlockGraph> LeadsTo {
            get { yield break; }
        }
        public override void Walk(Action<BlockGraph> func, HashSet<ulong> seen) {
            if(!seen.Add(Block.Start)) return;
            func(this);
        }
        public override void SwapNode(BlockGraph prev, BlockGraph next) {}
    }
    
    // Mature
    public class When(Block Block, BlockGraph Then, BlockGraph Next) : BlockGraph(Block) {
        public BlockGraph Then = Then;
        public BlockGraph Next = Next;
        public override string ToString() => $"When(0x{(Then != null ? Then.Block.Start : 0):X}, 0x{(Next != null ? Next.Block.Start : 0):X})";
        public override IEnumerable<BlockGraph> LeadsTo {
            get {
                yield return Then;
                yield return Next;
            }
        }
        public override void Walk(Action<BlockGraph> func, HashSet<ulong> seen) {
            if(!seen.Add(Block.Start)) return;
            func(this);
            Then.Walk(func, seen);
            Next.Walk(func, seen);
        }
        public override void SwapNode(BlockGraph prev, BlockGraph next) {
            if(ReferenceEquals(Then, prev)) Then = next;
            if(ReferenceEquals(Next, prev)) Next = next;
        }
    }
    public class Unless(Block Block, BlockGraph Then, BlockGraph Next) : BlockGraph(Block) {
        public BlockGraph Then = Then;
        public BlockGraph Next = Next;
        public override IEnumerable<BlockGraph> LeadsTo {
            get {
                yield return Then;
                yield return Next;
            }
        }
        public override string ToString() => $"Unless(0x{(Then != null ? Then.Block.Start : 0):X}, 0x{(Next != null ? Next.Block.Start : 0):X})";
        public override void Walk(Action<BlockGraph> func, HashSet<ulong> seen) {
            if(!seen.Add(Block.Start)) return;
            func(this);
            Then.Walk(func, seen);
            Next.Walk(func, seen);
        }
        public override void SwapNode(BlockGraph prev, BlockGraph next) {
            if(ReferenceEquals(Then, prev)) Then = next;
            if(ReferenceEquals(Next, prev)) Next = next;
        }
    }
    public class If(Block Block, BlockGraph Then, BlockGraph Else, BlockGraph Next) : BlockGraph(Block) {
        public BlockGraph Then = Then;
        public BlockGraph Else = Else;
        public BlockGraph Next = Next;
        public override string ToString() => $"If(0x{(Then != null ? Then.Block.Start : 0):X}, 0x{(Else != null ? Else.Block.Start : 0):X}, 0x{(Next != null ? Next.Block.Start : 0):X})";
        public override IEnumerable<BlockGraph> LeadsTo {
            get {
                yield return Then;
                yield return Else;
                yield return Next;
            }
        }
        public override void Walk(Action<BlockGraph> func, HashSet<ulong> seen) {
            if(!seen.Add(Block.Start)) return;
            func(this);
            Then.Walk(func, seen);
            Else.Walk(func, seen);
            Next.Walk(func, seen);
        }
        public override void SwapNode(BlockGraph prev, BlockGraph next) {
            if(ReferenceEquals(Then, prev)) Then = next;
            if(ReferenceEquals(Else, prev)) Else = next;
            if(ReferenceEquals(Next, prev)) Next = next;
        }
    }
    public class TerminalIf(Block Block, BlockGraph Then, BlockGraph Else) : BlockGraph(Block) {
        public BlockGraph Then = Then;
        public BlockGraph Else = Else;
        public override string ToString() => $"TerminalIf(0x{(Then != null ? Then.Block.Start : 0):X}, 0x{(Else != null ? Else.Block.Start : 0):X})";
        public override IEnumerable<BlockGraph> LeadsTo {
            get {
                yield return Then;
                yield return Else;
            }
        }
        public override void Walk(Action<BlockGraph> func, HashSet<ulong> seen) {
            if(!seen.Add(Block.Start)) return;
            func(this);
            Then.Walk(func, seen);
            Else.Walk(func, seen);
        }
        public override void SwapNode(BlockGraph prev, BlockGraph next) {
            if(ReferenceEquals(Then, prev)) Then = next;
            if(ReferenceEquals(Else, prev)) Else = next;
        }
    }
    public class DoWhile(Block Block, BlockGraph Loop, BlockGraph Next) : BlockGraph(Block) {
        public BlockGraph Loop = Loop;
        public BlockGraph Next = Next;
        public override string ToString() => $"DoWhile(0x{(Loop != null ? Loop.Block.Start : 0):X}, 0x{(Next != null ? Next.Block.Start : 0):X})";
        public override IEnumerable<BlockGraph> LeadsTo {
            get {
                yield return Loop;
                yield return Next;
            }
        }
        public override void Walk(Action<BlockGraph> func, HashSet<ulong> seen) {
            if(!seen.Add(Block.Start)) return;
            func(this);
            Loop.Walk(func, seen);
            Next.Walk(func, seen);
        }
        public override void SwapNode(BlockGraph prev, BlockGraph next) {
            if(ReferenceEquals(Loop, prev)) Loop = next;
            if(ReferenceEquals(Next, prev)) Next = next;
        }
    }
    public class InverseDoWhile(Block Block, BlockGraph Loop, BlockGraph Next) : BlockGraph(Block) {
        public BlockGraph Loop = Loop;
        public BlockGraph Next = Next;
        public override string ToString() => $"InverseDoWhile(0x{(Loop != null ? Loop.Block.Start : 0):X}, 0x{(Next != null ? Next.Block.Start : 0):X})";
        public override IEnumerable<BlockGraph> LeadsTo {
            get {
                yield return Loop;
                yield return Next;
            }
        }
        public override void Walk(Action<BlockGraph> func, HashSet<ulong> seen) {
            if(!seen.Add(Block.Start)) return;
            func(this);
            Loop.Walk(func, seen);
            Next.Walk(func, seen);
        }
        public override void SwapNode(BlockGraph prev, BlockGraph next) {
            if(ReferenceEquals(Loop, prev)) Loop = next;
            if(ReferenceEquals(Next, prev)) Next = next;
        }
    }
    
    public bool IsMature() {
        var seen = new HashSet<BlockGraph>();
        var checkQueue = new Queue<BlockGraph>();
        checkQueue.Enqueue(this);
        void Check(BlockGraph next) {
            if(!seen.Contains(next) && !checkQueue.Contains(next))
                checkQueue.Enqueue(next);
        }
        while(checkQueue.TryDequeue(out var node)) {
            if(!seen.Add(node)) continue;
            switch(node) {
                case Conditional: return false;
                case Unconditional uc:
                    Check(uc.Next);
                    break;
                case If _if:
                    Check(_if.Then);
                    Check(_if.Else);
                    Check(_if.Next);
                    break;
                case TerminalIf tif:
                    Check(tif.Then);
                    Check(tif.Else);
                    break;
                case When _when:
                    Check(_when.Then);
                    Check(_when.Next);
                    break;
                case Unless _unless:
                    Check(_unless.Then);
                    Check(_unless.Next);
                    break;
                case DoWhile _while:
                    Check(_while.Loop);
                    Check(_while.Next);
                    break;
                case InverseDoWhile iwhile:
                    Check(iwhile.Loop);
                    Check(iwhile.Next);
                    break;
            }
        }
        return true;
    }

    public bool IsTerminal() {
        var seen = new HashSet<BlockGraph>();
        var checkQueue = new Queue<BlockGraph>();
        checkQueue.Enqueue(this);
        void Check(BlockGraph next) {
            if(!seen.Contains(next) && !checkQueue.Contains(next))
                checkQueue.Enqueue(next);
        }
        while(checkQueue.TryDequeue(out var node)) {
            if(!seen.Add(node)) continue;
            switch(node) {
                case End: return true;
                case Conditional c:
                    Check(c.Taken);
                    Check(c.Not);
                    break;
                case Unconditional uc:
                    Check(uc.Next);
                    break;
                case If _if:
                    Check(_if.Then);
                    Check(_if.Else);
                    Check(_if.Next);
                    break;
                case TerminalIf tif:
                    Check(tif.Then);
                    Check(tif.Else);
                    break;
                case When _when:
                    Check(_when.Then);
                    Check(_when.Next);
                    break;
                case Unless _unless:
                    Check(_unless.Then);
                    Check(_unless.Next);
                    break;
                case DoWhile _while:
                    Check(_while.Loop);
                    Check(_while.Next);
                    break;
                case InverseDoWhile iwhile:
                    Check(iwhile.Loop);
                    Check(iwhile.Next);
                    break;
            }
        }
        return false;
    }

    static void AssignPrevs(IEnumerable<BlockGraph> blocks) {
        foreach(var block in blocks)
            foreach(var lead in block.LeadsTo)
                if(!ReferenceEquals(lead, block))
                    lead.Prevs.Add(block);
    }

    static (bool Changed, Dictionary<ulong, BlockGraph> Blocks) Transform(Dictionary<ulong, BlockGraph> blocks, Func<BlockGraph, BlockGraph> func) {
        var changed = false;
        var didChange = false;
        do {
            changed = false;
            foreach(var (addr, block) in blocks) {
                var nblock = func(block);
                if(nblock == null || ReferenceEquals(block, nblock)) continue;
                foreach(var prev in block.Prevs)
                    nblock.Prevs.Add(prev);
                foreach(var next in nblock.LeadsTo)
                    if(!ReferenceEquals(next, block)) {
                        next.Prevs.Remove(block);
                        next.Prevs.Add(nblock);
                    }
                foreach(var rblock in block.Prevs.Concat([nblock]))
                    rblock.SwapNode(block, nblock);
                blocks[addr] = nblock;
                changed = true;
                didChange = true;
            }
        } while(changed);
        return (didChange, blocks);
    }

    static BlockGraph DominatingNext(BlockGraph left, BlockGraph right, BlockGraph includeOnRight = null) {
        List<BlockGraph> AllNext(BlockGraph node) {
            var all = new List<BlockGraph> { node };
            while(true) {
                node = node switch {
                    Unconditional rnode => rnode.Next,
                    Conditional rnode when !ReferenceEquals(node, rnode) => DominatingNext(rnode.Taken, rnode.Not),
                    When rnode => rnode.Next,
                    Unless rnode => rnode.Next,
                    If rnode => rnode.Next,
                    DoWhile rnode => rnode.Next,
                    InverseDoWhile rnode => rnode.Next,
                    _ => null,
                };
                if(node == null || all.Contains(node)) break;
                all.Add(node);
            }
            return all;
        }

        var leftNexts = AllNext(left);
        var rightNexts = AllNext(right);
        if(includeOnRight != null) rightNexts.Add(includeOnRight);
        return leftNexts.FirstOrDefault(leftNext => rightNexts.Any(rightNext => leftNext == rightNext));
    }

    public static Dictionary<ulong, BlockGraph> Reduce(Dictionary<ulong, BlockGraph> blocks, HashSet<ulong> funcs) {
        AssignPrevs(blocks.Values);
        bool IsPeninsula(BlockGraph node) {
            var subGraph = new List<ulong>();
            node.Walk(sub => subGraph.Add(sub.Block.Start));
            int Check(BlockGraph other) => subGraph.Contains(other.Block.Start) ? 1 : 0;

            var found = 0;
            var subPrevs = subGraph
                .Select(x => blocks[x].Prevs)
                .Aggregate(new HashSet<BlockGraph>(), (acc, b) => {
                    acc.UnionWith(b);
                    return acc;
                });
            foreach(var block in subPrevs) {
                if(subGraph.Contains(block.Block.Start)) continue;
                found += block switch {
                    Unconditional v => Check(v.Next),
                    Conditional v => Check(v.Taken) + Check(v.Not),
                    If v => Check(v.Then) + Check(v.Else) + Check(v.Next),
                    TerminalIf v => Check(v.Then) + Check(v.Else),
                    When v => Check(v.Then) + Check(v.Next),
                    Unless v => Check(v.Then) + Check(v.Next),
                    DoWhile v => Check(v.Loop) + Check(v.Next),
                    InverseDoWhile v => Check(v.Loop) + Check(v.Next),
                    End => 0,
                    _ => throw new NotImplementedException($"Can't check for peninsula in {block}"),
                };
                if(found > 1) return false;
            }
            return true;
        }

        (_, blocks) = Transform(blocks, node => {
            if(node is not Conditional cond) return null;
            if(DominatingNext(cond.Taken, cond.Not) == cond.Not || (cond.Taken.IsMature() && cond.Not.IsMature() && IsPeninsula(cond.Taken)))
                return new When(cond.Block, cond.Taken, cond.Not);
            if(DominatingNext(cond.Taken, cond.Not) == cond.Taken || (cond.Taken.IsMature() && cond.Not.IsMature() && IsPeninsula(cond.Not)))
                return new Unless(cond.Block, cond.Not, cond.Taken);
            if(DominatingNext(cond.Taken, cond.Not, node) == node)
                return new DoWhile(cond.Block, cond.Taken, cond.Not);
            if(DominatingNext(cond.Not, cond.Taken, node) == node)
                return new InverseDoWhile(cond.Block, cond.Not, cond.Taken);
            var sharedNext = DominatingNext(cond.Taken, cond.Not);
            if(sharedNext == null && cond.Taken.IsMature() && cond.Not.IsMature())
                return new TerminalIf(cond.Block, cond.Taken, cond.Not);
            return sharedNext != null && sharedNext != cond.Taken && sharedNext != cond.Not
                ? new If(cond.Block, cond.Taken, cond.Not, sharedNext)
                : null;
        });
        return blocks;
    }
}