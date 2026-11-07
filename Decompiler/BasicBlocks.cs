using LiftIl;

namespace SharpRetro.Decompiler;

public enum TerminatorKind { Jump, Conditional, Return, Call }
public sealed record BasicBlock(ulong Address, IReadOnlyList<LiftedInstruction> Instructions,
    TerminatorKind Kind, ulong Target, ulong Alternative, IlBranch? Branch) {
    // Intraprocedural flow resumes after a call; the callee is a separate function.
    public IEnumerable<ulong> FlowSuccessors => Kind == TerminatorKind.Call ? [Alternative] : Successors;
    public IEnumerable<ulong> Successors => Kind switch {
        TerminatorKind.Return => [], TerminatorKind.Conditional or TerminatorKind.Call => new[] { Target, Alternative }.Distinct(),
        _ => [Target]
    };
}
public sealed record BlockGraph(ulong Entry, IReadOnlyDictionary<ulong, BasicBlock> Blocks,
    IReadOnlyDictionary<ulong, HashSet<ulong>> Predecessors);
public sealed class StructuringException(string message) : Exception(message);

/// Preserve instruction IL verbatim; recover block/edge topology above it.
public static class BasicBlocks {
    public static BlockGraph Build(LiftedProgram program, bool includeCalls = false, IReadOnlySet<ulong>? functionEntries = null) {
        var instructions = program.Instructions.ToDictionary(i => i.Address);
        var terms = instructions.Values.ToDictionary(i => i.Address, i => Describe(i, includeCalls));
        var leaders = new HashSet<ulong> { program.Entry };
        if(functionEntries != null) leaders.UnionWith(functionEntries.Where(instructions.ContainsKey));
        foreach(var term in terms.Values.Where(t => t.Branch != null))
            foreach(var target in term.Successors.Where(instructions.ContainsKey)) leaders.Add(target);
        var blocks = new Dictionary<ulong, BasicBlock>();
        var consumed = new HashSet<ulong>();
        foreach(var leader in leaders.Order()) {
            var body = new List<LiftedInstruction>();
            ulong current = leader;
            BasicBlock terminal;
            while(true) {
                if(!consumed.Add(current)) throw new StructuringException("Ambiguous basic-block ownership");
                var insn = instructions[current];
                body.Add(insn); terminal = terms[current];
                if(terminal.Branch != null || !instructions.ContainsKey(insn.Next) || leaders.Contains(insn.Next)) break;
                current = insn.Next;
            }
            blocks.Add(leader, terminal with { Address = leader, Instructions = body });
        }
        if(consumed.Count != instructions.Count) throw new StructuringException("Basic blocks did not account for every instruction");
        var predecessors = blocks.Keys.ToDictionary(a => a, _ => new HashSet<ulong>());
        foreach(var block in blocks.Values)
            foreach(ulong target in block.Successors)
                if(predecessors.TryGetValue(target, out var set)) set.Add(block.Address);
        return new(program.Entry, blocks, predecessors);
    }

    static BasicBlock Describe(LiftedInstruction insn, bool includeCalls) {
        var branches = Branches(insn.Body).ToArray();
        if(branches.Length == 0) return new(insn.Address, [insn], TerminatorKind.Jump, insn.Next, 0, null);
        if(branches.Length != 1 || !ReferenceEquals(insn.Body.Body.LastOrDefault(), branches[0]))
            throw new StructuringException($"Instruction-internal control transfer at 0x{insn.Address:x}");
        var branch = branches[0];
        if(branch.Kind == BranchKind.Ret && branch.Cond == null)
            return new(insn.Address, [insn], TerminatorKind.Return, 0, 0, branch);
        if(includeCalls && branch.Kind == BranchKind.Call && branch.Cond == null && ControlFlow.ConstantTarget(insn.Body, branch) is { } target)
            return new(insn.Address, [insn], TerminatorKind.Call, target, insn.Next, branch);
        if(branch.Kind is not (BranchKind.Jmp or BranchKind.CondJmp or BranchKind.Fallthrough) ||
           branch.Target is not IlConst(_, var bits) || bits > ulong.MaxValue)
            throw new StructuringException($"Unsupported block terminator at 0x{insn.Address:x}");
        return new(insn.Address, [insn], branch.Cond == null ? TerminatorKind.Jump : TerminatorKind.Conditional,
            (ulong)bits, insn.Next, branch);
    }

    static IEnumerable<IlBranch> Branches(Il n) => n switch {
        IlBranch b => [b], IlBlock b => b.Body.SelectMany(Branches),
        IlIf i => i.Then.Concat(i.Else).SelectMany(Branches), _ => []
    };
}
