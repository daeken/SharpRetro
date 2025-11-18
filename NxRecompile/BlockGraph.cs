using StaticRecompilerBase;
namespace NxRecompile;
using Body = List<StaticIRStatement>;

public record Block(ulong Start, Body Body);

public abstract class BlockGraph(Block Block) {
    public Block Block = Block;
    
    // Immature
    public class Unconditional(Block Block, BlockGraph Next = null) : BlockGraph(Block) {
        public BlockGraph Next = Next;
        public override string ToString() => $"Unconditional(0x{(Next != null ? Next.Block.Start : 0):X})";
    }
    public class Conditional(Block Block, BlockGraph Taken = null, BlockGraph Not = null) : BlockGraph(Block) {
        public BlockGraph Taken = Taken;
        public BlockGraph Not = Not;
        public override string ToString() => $"Conditional(0x{(Taken != null ? Taken.Block.Start : 0):X}, 0x{(Not != null ? Not.Block.Start : 0):X})";
    }
    public class End(Block Block) : BlockGraph(Block);
    
    // Mature
    public class When(Block Block, BlockGraph Then, BlockGraph Next) : BlockGraph(Block) {
        public BlockGraph Then = Then;
        public BlockGraph Next = Next;
        public override string ToString() => $"When(0x{(Then != null ? Then.Block.Start : 0):X}, 0x{(Next != null ? Next.Block.Start : 0):X})";
    }
    public class If(Block Block, BlockGraph Then, BlockGraph Else, BlockGraph Next) : BlockGraph(Block) {
        public BlockGraph Then = Then;
        public BlockGraph Else = Else;
        public BlockGraph Next = Next;
        public override string ToString() => $"If(0x{(Then != null ? Then.Block.Start : 0):X}, 0x{(Else != null ? Else.Block.Start : 0):X}, 0x{(Next != null ? Next.Block.Start : 0):X})";
    }
    public class While(Block Block, BlockGraph Loop, BlockGraph Next) : BlockGraph(Block) {
        public BlockGraph Loop = Loop;
        public BlockGraph Next = Next;
        public override string ToString() => $"While(0x{(Loop != null ? Loop.Block.Start : 0):X}, 0x{(Next != null ? Next.Block.Start : 0):X})";
    }
    public class InverseWhile(Block Block, BlockGraph Loop, BlockGraph Next) : BlockGraph(Block) {
        public BlockGraph Loop = Loop;
        public BlockGraph Next = Next;
        public override string ToString() => $"InverseWhile(0x{(Loop != null ? Loop.Block.Start : 0):X}, 0x{(Next != null ? Next.Block.Start : 0):X})";
    }
    public class Node(Block Block) : BlockGraph(Block);

    static (bool Changed, Dictionary<ulong, BlockGraph> Blocks) Transform(Dictionary<ulong, BlockGraph> blocks, Func<BlockGraph, BlockGraph> func) {
        var changed = false;
        var didChange = false;
        do {
            changed = false;
            foreach(var (addr, block) in blocks) {
                var nblock = func(block);
                if(nblock == null || ReferenceEquals(block, nblock)) continue;
                foreach(var (raddr, rblock) in blocks) {
                    if(ReferenceEquals(rblock, block)) continue;
                    switch(rblock) {
                        case Unconditional node: {
                            if(ReferenceEquals(block, node.Next)) node.Next = nblock;
                            break;
                        }
                        case Conditional node: {
                            if(ReferenceEquals(block, node.Taken)) node.Taken = nblock;
                            if(ReferenceEquals(block, node.Not)) node.Not = nblock;
                            break;
                        }
                        case When node: {
                            if(ReferenceEquals(block, node.Then)) node.Then = nblock;
                            if(ReferenceEquals(block, node.Next)) node.Next = nblock;
                            break;
                        }
                        case If node: {
                            if(ReferenceEquals(block, node.Then)) node.Then = nblock;
                            if(ReferenceEquals(block, node.Else)) node.Else = nblock;
                            if(ReferenceEquals(block, node.Next)) node.Next = nblock;
                            break;
                        }
                        case While node: {
                            if(ReferenceEquals(block, node.Loop)) node.Loop = nblock;
                            if(ReferenceEquals(block, node.Next)) node.Next = nblock;
                            break;
                        }
                        case InverseWhile node: {
                            if(ReferenceEquals(block, node.Loop)) node.Loop = nblock;
                            if(ReferenceEquals(block, node.Next)) node.Next = nblock;
                            break;
                        }
                    }
                }
                blocks[addr] = nblock;
                changed = true;
                didChange = true;
                break;
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
                    If rnode => rnode.Next,
                    While rnode => rnode.Next,
                    InverseWhile rnode => rnode.Next,
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
        while(true) {
            (var foundWhen, blocks) = Transform(blocks, node => 
                node is Conditional cond && DominatingNext(cond.Taken, cond.Not) == cond.Not
                    ? new When(cond.Block, cond.Taken, cond.Not)
                    : null);
            (var foundWhile, blocks) = Transform(blocks, node => {
                if(node is not Conditional cond) return null;
                return DominatingNext(cond.Taken, cond.Not, node) == node ? new While(cond.Block, cond.Taken, cond.Not) : null;
            });
            (var foundInverseWhile, blocks) = Transform(blocks, node => {
                if(node is not Conditional cond) return null;
                return DominatingNext(cond.Not, cond.Taken, node) == node ? new InverseWhile(cond.Block, cond.Not, cond.Taken) : null;
            });
            (var foundIf, blocks) = Transform(blocks, node => {
                if(node is not Conditional cond) return null;
                var sharedNext = DominatingNext(cond.Taken, cond.Not);
                return sharedNext != null ? new If(cond.Block, cond.Taken, cond.Not, sharedNext) : null;
            });
            if(!foundWhen && !foundWhile && !foundInverseWhile && !foundIf)
                return blocks;
        }
    }
}