namespace SharpRetro.Decompiler;

public sealed class RecoveredFunction(ulong entry) {
    public ulong Entry { get; } = entry;
    public string Name => $"fn_{Entry:x}";
    public HashSet<ulong> Blocks { get; } = [];
    public HashSet<ValueSlot> MayWrite { get; } = [];
    public HashSet<ValueSlot> Parameters { get; set; } = [];
    public HashSet<ValueSlot> Results { get; } = [];
}
public sealed record RecoveredFunctions(BlockGraph Graph, IReadOnlyDictionary<ulong, ValueBlock> Blocks,
    IReadOnlyDictionary<ulong, RecoveredFunction> Functions, IReadOnlyDictionary<ulong, ulong> Owners) {
    public bool Windows { get; init; }
}

/// Function entries come from direct CALL targets and the selected entry. Blocks
/// are owned by one function; a transfer to another entry is a tail call. Contract
/// inference is a call-graph fixed point, not a guessed source prototype.
public static class FunctionRecovery {
    public static readonly ValueSlot Accumulator = new(SlotKind.Local, 0), Stack = new(SlotKind.Local, 4);
    public static readonly IReadOnlySet<ValueSlot> ImportWrites = new HashSet<ValueSlot> { Accumulator, Stack };
    public static readonly IReadOnlySet<ValueSlot> ImportInputs = new HashSet<ValueSlot> {
        new(SlotKind.Local,7), new(SlotKind.Local,6), new(SlotKind.Local,2), Stack
    };
    public static readonly IReadOnlySet<ValueSlot> ImportResults = new HashSet<ValueSlot> { Accumulator };

    public static RecoveredFunctions Recover(LiftedProgram program,IReadOnlyDictionary<ulong,IReadOnlySet<ValueSlot>>? preserved=null,
        IReadOnlySet<NativeEffectPoint>? removedEffects=null) {
        var code = program.Instructions.Select(i => i.Address).ToHashSet();
        var entries = new HashSet<ulong> { program.Entry };
        foreach(ulong address in program.Image.FunctionStarts) {
            if(program.Instructions.Any(i=>address>i.Address && address<i.Next))
                throw new NotSupportedException($"Function start 0x{address:x} lies inside a recovered instruction");
            if(code.Contains(address)) entries.Add(address);
        }
        foreach(var instruction in program.Instructions) {
            if(instruction.Body.Body.LastOrDefault() is LiftIl.IlBranch { Kind: LiftIl.BranchKind.Call } branch &&
               ControlFlow.ConstantTarget(instruction.Body, branch) is { } target && code.Contains(target)) entries.Add(target);
        }
        var graph = BasicBlocks.Build(program, includeCalls:true, functionEntries:entries);
        var functions = entries.Order().ToDictionary(e => e, e => new RecoveredFunction(e));
        var owners = new Dictionary<ulong, ulong>();
        foreach(var function in functions.Values) {
            var work = new Stack<ulong>(); work.Push(function.Entry);
            while(work.TryPop(out ulong address)) {
                if(address != function.Entry && entries.Contains(address)) continue;
                if(program.Imports.ContainsKey(address)) continue;
                if(!graph.Blocks.TryGetValue(address, out var block)) throw new NotSupportedException($"Function reaches untranslated block 0x{address:x}");
                if(owners.TryGetValue(address, out ulong owner)) {
                    if(owner != function.Entry) throw new NotSupportedException($"Shared function block 0x{address:x} has ambiguous ownership");
                    continue;
                }
                owners[address] = function.Entry; function.Blocks.Add(address);
                foreach(var next in block.Kind == TerminatorKind.Call ? new[] { block.Alternative } : block.Successors) work.Push(next);
            }
        }
        if(owners.Count != graph.Blocks.Count) throw new NotSupportedException("Function partition omits recovered blocks");
        var (blocks, nextValue) = ValueRecovery.Lower(graph);
        if(removedEffects!=null)
            foreach(var (address,block) in blocks) {
                var keep=block.Effects.Where((_,index)=>!removedEffects.Contains(new(address,index))).ToArray();
                block.Effects.Clear(); block.Effects.AddRange(keep);
            }
        foreach(var function in functions.Values)
            foreach(ulong address in function.Blocks) function.MayWrite.UnionWith(blocks[address].Outputs.Keys);
        // Establish the allowed set before monotone call-summary propagation;
        // removal inside the loop alone can hide an equal-size set change.
        if(preserved!=null)
            foreach(var function in functions.Values)
                if(preserved.TryGetValue(function.Entry,out var identities)) function.MayWrite.ExceptWith(identities);

        // A caller's writes include its callees' writes. Keep conservative may-
        // write summaries so narrow/conditional outputs are never assumed preserved.
        bool changed;
        do {
            changed = false;
            foreach(var function in functions.Values) {
                int count = function.MayWrite.Count;
                foreach(ulong target in Callees(function)) function.MayWrite.UnionWith(Writes(target));
                if(preserved?.TryGetValue(function.Entry,out var identities)==true) function.MayWrite.ExceptWith(identities);
                changed |= count != function.MayWrite.Count;
            }
        } while(changed);

        functions[program.Entry].Results.Add(Accumulator);
        do {
            changed = false;
            foreach(var function in functions.Values.Reverse()) {
                foreach(ulong address in function.Blocks.OrderDescending()) {
                    var block = blocks[address]; var liveOut = new HashSet<ValueSlot>();
                    if(block.Block.Kind == TerminatorKind.Return) {
                        liveOut.UnionWith(function.Results); liveOut.Add(Stack);
                    } else if(block.Block.Kind == TerminatorKind.Call) {
                        if(!function.Blocks.Contains(block.Block.Alternative)) throw new NotSupportedException("Call continuation leaves its function");
                        var continuation = blocks[block.Block.Alternative].LiveIn;
                        ulong target = block.Block.Target;
                        Demand(target, continuation);
                        liveOut.UnionWith(continuation.Where(slot => !Writes(target).Contains(slot)));
                        liveOut.UnionWith(Inputs(target));
                    } else {
                        foreach(ulong target in block.Block.Successors) {
                            if(function.Blocks.Contains(target)) liveOut.UnionWith(blocks[target].LiveIn);
                            else {
                                Demand(target, function.Results);
                                liveOut.UnionWith(function.Results.Where(slot => !Writes(target).Contains(slot)));
                                liveOut.UnionWith(Inputs(target));
                            }
                        }
                    }
                    var roots = ValueRecovery.Roots(block);
                    foreach(var slot in liveOut) roots.Add(ValueRecovery.EnsureOutput(block,slot,ref nextValue));
                    var liveIn = ValueRecovery.Dependencies(roots);
                    if(!block.LiveIn.SetEquals(liveIn) || !block.LiveOut.SetEquals(liveOut)) {
                        block.LiveIn = liveIn; block.LiveOut = liveOut; changed = true;
                    }
                }
                var parameters = blocks[function.Entry].LiveIn;
                if(!function.Parameters.SetEquals(parameters)) { function.Parameters = new(parameters); changed = true; }
            }
        } while(changed);
        return new(graph,blocks,functions,owners) { Windows=FunctionAbi.Windows(program.Image) };

        IEnumerable<ulong> Callees(RecoveredFunction function) {
            foreach(ulong address in function.Blocks) {
                var block = graph.Blocks[address];
                if(block.Kind == TerminatorKind.Call) yield return block.Target;
                else foreach(ulong target in block.Successors.Where(t => !function.Blocks.Contains(t))) yield return target;
            }
        }
        IReadOnlySet<ValueSlot> Writes(ulong target) => functions.TryGetValue(target,out var function) ? function.MayWrite
            : program.Imports.ContainsKey(target) ? FunctionAbi.ImportWrites(program.Image) : throw new NotSupportedException($"Unresolved function edge 0x{target:x}");
        IReadOnlySet<ValueSlot> Inputs(ulong target) => functions.TryGetValue(target,out var function) ? function.Parameters : ApplicationImports.Inputs(program.Imports[target],FunctionAbi.Windows(program.Image));
        void Demand(ulong target, IEnumerable<ValueSlot> observed) {
            if(!functions.TryGetValue(target,out var function)) {
                if(!FunctionAbi.Windows(program.Image) && observed.Any(s=>s.Kind==SlotKind.Vector))
                    throw new NotSupportedException("Observed volatile vector after host import "+program.Imports[target]);
                if(FunctionAbi.Windows(program.Image) && observed.Any(s=>s!=Accumulator && s!=Stack && FunctionAbi.ImportWrites(program.Image).Contains(s)))
                    throw new NotSupportedException("Observed volatile register after Windows import "+program.Imports[target]);
                if(ApplicationImports.Get(program.Imports[target]).Void && observed.Contains(Accumulator))
                    throw new NotSupportedException("Observed volatile result of void import "+program.Imports[target]);
                return;
            }
            int count = function.Results.Count;
            function.Results.UnionWith(observed.Where(slot => slot != Stack && function.MayWrite.Contains(slot)));
            changed |= count != function.Results.Count;
        }
    }
}
