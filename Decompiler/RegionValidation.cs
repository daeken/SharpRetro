namespace SharpRetro.Decompiler;

/// Check the recovered AST's block transitions independently of join selection.
/// Each lexical fallthrough, loop edge and conditional arm must have the exact
/// original destination, including every instance of an allowed copied block.
/// This checks a graph rewrite, not instruction semantics.
public static class RegionValidation {
    sealed record LoopContext(ulong Header, ulong? Follow, IReadOnlySet<ulong>? Exits = null);

    public static void Check(BlockGraph graph, Region body, bool allowBlockDuplication = false) {
        var seen = new HashSet<ulong>();
        ulong? Visit(Region region, ulong? next, LoopContext? loop) {
            void Block(BasicBlock b) {
                bool first=seen.Add(b.Address);
                if((!first && !allowBlockDuplication) || !graph.Blocks.TryGetValue(b.Address, out var original) || !ReferenceEquals(b, original))
                    throw new StructuringException("Structured region duplicates or replaces a basic block");
            }
            switch(region) {
                case SequenceRegion s:
                    for(int i = s.Body.Count - 1; i >= 0; i--) next = Visit(s.Body[i], next, loop);
                    return next;
                case BlockRegion b:
                    Block(b.Block);
                    if(b.Block.Kind != TerminatorKind.Return &&
                       (b.Block.Kind is not (TerminatorKind.Jump or TerminatorKind.Call) ||
                        (b.Block.Kind == TerminatorKind.Call ? b.Block.Alternative : b.Block.Target) != next))
                        throw new StructuringException($"Structured fallthrough changes edge at 0x{b.Block.Address:x}");
                    return b.Block.Address;
                case IfRegion i:
                    Block(i.Test);
                    if(i.Test.Kind != TerminatorKind.Conditional ||
                       Visit(i.Then, next, loop) != i.Test.Target || Visit(i.Else, next, loop) != i.Test.Alternative)
                        throw new StructuringException($"Structured conditional changes edge at 0x{i.Test.Address:x}");
                    return i.Test.Address;
                case LoopRegion l:
                    if(l.Exits != null) {
                        var targets = l.Exits.Select(e=>e.Target).ToHashSet();
                        if(targets.Count != l.Exits.Count || l.Exits.Count < 2)
                            throw new StructuringException("Invalid loop exit selection");
                        foreach(var e in l.Exits)
                            if(Visit(e.Body,next,loop) != e.Target)
                                throw new StructuringException("Structured loop changes exit destination");
                    }
                    if(Visit(l.Body, l.Header, new(l.Header, next,l.Exits?.Select(e=>e.Target).ToHashSet())) != l.Header)
                        throw new StructuringException("Structured loop changes entry block");
                    return l.Header;
                case BreakRegion { Target: { } target }:
                    if(loop?.Exits?.Contains(target) != true)
                        throw new StructuringException("Break selects an unknown loop exit");
                    return target;
                case BreakRegion:
                    return loop?.Follow ?? throw new StructuringException("Break has no matching loop exit");
                case ContinueRegion:
                    return loop?.Header ?? throw new StructuringException("Continue has no matching loop header");
                case BoundaryRegion b:
                    if(graph.Blocks.ContainsKey(b.Address)) throw new StructuringException("Boundary hides a translated block");
                    return b.Address;
                default: throw new StructuringException("Unknown structured region");
            }
        }
        if(Visit(body, null, null) != graph.Entry || !seen.SetEquals(graph.Blocks.Keys))
            throw new StructuringException("Structured graph changes entry or omits blocks");
    }
}
