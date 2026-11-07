using LiftIl;

namespace SharpRetro.Decompiler;

public sealed class NativeFunctionMemory {
    public Dictionary<ulong, Dictionary<ValueSlot,long>> Entries { get; } = [];
    public HashSet<ValueEffect> RemovedEffects { get; } = new(ReferenceEqualityComparer.Instance);
    public Dictionary<(long Offset,int Width),string> Scalars { get; } = [];
    public long Minimum { get; set; }
    public int Cleanup { get; set; }
    public int FrameSize => Minimum == 0 ? 0 : checked((int)((-Minimum+15)/16*16+8));
    public string Pointer(long offset) => $"frame_bytes+{FrameSize+offset}";
    public long? Offset(ValueBlock block,RecoveredValue value) => NativeMemoryRecovery.Offset(value,Entries[block.Block.Address]);
}

/// Establish a constant frame coordinate at every block before replacing the
/// emulated call stack. CALL/RET bookkeeping disappears only with canonical
/// frame heights/returns and no direct frame accesses to the numeric return slot
/// or incoming stack arguments. General pointer accesses use native C++ memory
/// semantics; unknown/dynamic frame layouts retain the checked representation.
public static class NativeMemoryRecovery {
    public static Dictionary<ulong,NativeFunctionMemory> Recover(RecoveredFunctions recovered,ulong entry) {
        var plans = recovered.Functions.Keys.ToDictionary(a=>a,_=>new NativeFunctionMemory());
        foreach(var function in recovered.Functions.Values) {
            var cleanups = new HashSet<int>();
            foreach(ulong address in function.Blocks) {
                var block = recovered.Blocks[address].Block;
                if(block.Kind != TerminatorKind.Return) continue;
                var ret = block with { Instructions = [block.Instructions[^1]] };
                var single = ValueRecovery.Lower(new BlockGraph(address,new Dictionary<ulong,BasicBlock> { [address]=ret },new Dictionary<ulong,HashSet<ulong>>())).Blocks[address];
                var state = new Dictionary<ValueSlot,long> { [FunctionRecovery.Stack]=0 };
                long? after = Offset(single.Output(FunctionRecovery.Stack),state);
                if(after is not (>=8 and <=65543)) throw new NotSupportedException("Nonconstant RET stack cleanup");
                cleanups.Add(checked((int)after.Value-8));
            }
            if(cleanups.Count>1) throw new NotSupportedException($"Inconsistent RET cleanup in {function.Name}");
            plans[function.Entry].Cleanup=cleanups.SingleOrDefault();
        }
        // A tail-only function inherits the cleanup of its eventual callee.
        for(int iteration=0;iteration<plans.Count;iteration++)
            foreach(var function in recovered.Functions.Values) {
                var tailCleanups=function.Blocks.SelectMany(a=>recovered.Blocks[a].Block.Kind is TerminatorKind.Jump or TerminatorKind.Conditional
                    ? recovered.Blocks[a].Block.Successors.Where(t=>!function.Blocks.Contains(t)).Select(t=>plans.TryGetValue(t,out var p) ? p.Cleanup : 0) : []).ToHashSet();
                if(!function.Blocks.Any(a=>recovered.Blocks[a].Block.Kind==TerminatorKind.Return) && tailCleanups.Count!=0)
                    plans[function.Entry].Cleanup=tailCleanups.Max();
            }
        foreach(var function in recovered.Functions.Values)
            foreach(var target in function.Blocks.SelectMany(a=>recovered.Blocks[a].Block.Kind is TerminatorKind.Jump or TerminatorKind.Conditional
                ? recovered.Blocks[a].Block.Successors.Where(t=>!function.Blocks.Contains(t)) : []))
                if((plans.TryGetValue(target,out var tail) ? tail.Cleanup : 0)!=plans[function.Entry].Cleanup)
                    throw new NotSupportedException("Tail call changes RET cleanup");
        if(plans[entry].Cleanup!=0) throw new NotSupportedException("Main has noncanonical stack cleanup");
        foreach(var function in recovered.Functions.Values) Analyze(function,plans[function.Entry]);
        return plans;

        void Analyze(RecoveredFunction function,NativeFunctionMemory plan) {
            plan.Entries[function.Entry]=new() { [FunctionRecovery.Stack]=0 };
            var work=new Queue<ulong>(); work.Enqueue(function.Entry);
            while(work.TryDequeue(out ulong address)) {
                var block=recovered.Blocks[address];
                var state=plan.Entries[address];
                var next=new Dictionary<ValueSlot,long>(state);
                foreach(var (slot,value) in block.Outputs) {
                    if(Offset(value,state) is { } offset) next[slot]=offset;
                    else next.Remove(slot);
                }
                if(!next.TryGetValue(FunctionRecovery.Stack,out long sp)) throw new NotSupportedException($"Dynamic stack layout in {function.Name}");
                if(block.Block.Kind==TerminatorKind.Call) {
                    var writes=recovered.Functions.TryGetValue(block.Block.Target,out var callee) ? callee.MayWrite :
                        FunctionAbi.ImportWrites(recovered.Windows);
                    foreach(var slot in writes) next.Remove(slot);
                    next[FunctionRecovery.Stack]=checked(sp+8+(plans.TryGetValue(block.Block.Target,out var p) ? p.Cleanup : 0));
                }
                foreach(ulong target in block.Block.FlowSuccessors.Where(function.Blocks.Contains)) {
                    if(!plan.Entries.TryGetValue(target,out var old)) { plan.Entries[target]=new(next); work.Enqueue(target); continue; }
                    if(old[FunctionRecovery.Stack]!=next[FunctionRecovery.Stack]) throw new NotSupportedException($"Stack height differs at join 0x{target:x}");
                    var remove=old.Keys.Where(s=>!next.TryGetValue(s,out long offset) || offset!=old[s]).ToArray();
                    if(remove.Length!=0) { foreach(var slot in remove) old.Remove(slot); work.Enqueue(target); }
                }
            }
            var accesses=new List<(long Offset,int Width)>();
            foreach(ulong address in function.Blocks) {
                var block=recovered.Blocks[address]; var state=plan.Entries[address];
                long? At(RecoveredValue value) => Offset(value,state);
                long sp=At(block.Output(FunctionRecovery.Stack)) ?? throw new NotSupportedException("Unknown stack output");
                if(block.Block.Kind==TerminatorKind.Call) {
                    var push=block.Effects.LastOrDefault();
                    if(push is not { Kind:"store",Value.Width:64 } || push.Value.Constant!=block.Block.Alternative || At(push.Address!)!=sp)
                        throw new NotSupportedException("Noncanonical CALL return slot");
                    plan.RemovedEffects.Add(push);
                    sp=checked(sp+8);
                }
                if(block.Block.Kind==TerminatorKind.Return) {
                    if(block.Target is not { Operation:IlLoad,Width:64 } load || At(load.Inputs[0])!=0 || sp!=8+plan.Cleanup)
                        throw new NotSupportedException($"Noncanonical return frame in {function.Name}");
                    foreach(var effect in block.Effects.Where(e=>e.Kind=="evaluate" && ReferenceEquals(e.Value,load))) plan.RemovedEffects.Add(effect);
                    if(function.Results.Any(s=>At(block.Output(s))!=null)) throw new NotSupportedException("Function returns its own frame address");
                } else {
                    if(sp>0) throw new NotSupportedException("Stack allocation crosses incoming return slot");
                    plan.Minimum=Math.Min(plan.Minimum,sp);
                }
                foreach(ulong target in block.Block.FlowSuccessors.Where(t=>!function.Blocks.Contains(t))) {
                    if(sp!=0 || plans.GetValueOrDefault(target)?.Cleanup is { } cleanup && cleanup!=plan.Cleanup)
                        throw new NotSupportedException("Tail call does not restore entry stack");
                }
                var seen=new HashSet<int>();
                void Visit(RecoveredValue value) {
                    if(!seen.Add(value.Id)) return;
                    if(value.Operation is IlLoad) Access(value.Inputs[0],value.Width);
                    foreach(var child in value.Inputs) Visit(child);
                }
                void Access(RecoveredValue value,int width) {
                    if(width is not (8 or 16 or 32 or 64 or 128)) throw new NotSupportedException("Unsupported native memory access width");
                    if(At(value) is not { } offset) return;
                    if(offset>=0 || offset+width/8>0) throw new NotSupportedException("Access to return slot or incoming stack arguments");
                    plan.Minimum=Math.Min(plan.Minimum,offset);
                    accesses.Add((offset,width));
                }
                foreach(var effect in block.Effects.Where(e=>!plan.RemovedEffects.Contains(e))) {
                    if(effect.Address!=null) { Access(effect.Address,effect.Value.Width); Visit(effect.Address); }
                    // Storing a frame pointer can hide an escaping alias in memory.
                    if(effect.Kind=="store" && At(effect.Value)!=null) throw new NotSupportedException("Frame address stored in memory");
                    Visit(effect.Value);
                }
                foreach(var slot in block.LiveOut.Where(s=>s!=FunctionRecovery.Stack)) {
                    var value=block.Output(slot); Visit(value);
                    if(At(value) is { } offset) {
                        if(offset>=0) throw new NotSupportedException("Frame address reaches incoming return slot");
                        plan.Minimum=Math.Min(plan.Minimum,offset);
                    }
                }
                if(block.Condition!=null) Visit(block.Condition);
                plan.Minimum=Math.Min(plan.Minimum,state[FunctionRecovery.Stack]);
                if(plan.Minimum==0 && block.Inputs.TryGetValue(FunctionRecovery.Stack,out var input) && seen.Contains(input.Id)) plan.Minimum=-8;
            }
            if(plan.Minimum < -1024*1024) throw new NotSupportedException("Native frame exceeds one MiB");
            // Typed fields share the frame's byte representation. Escaped
            // pointers and narrow/overlapping views therefore still alias the
            // exact storage read by direct field assignments.
            foreach(var access in accesses.Distinct().OrderByDescending(a=>a.Width).ThenBy(a=>a.Offset)) {
                if((plan.FrameSize+access.Offset) % (access.Width/8)!=0) continue;
                if(plan.Scalars.Keys.Any(other=>access.Offset<other.Offset+other.Width/8 && other.Offset<access.Offset+access.Width/8)) continue;
                plan.Scalars[access]=$"stack_local_{-access.Offset:x}";
            }
        }
    }

    internal static long? Offset(RecoveredValue value,IReadOnlyDictionary<ValueSlot,long> state) {
        if(value.Slot is { } slot) return state.TryGetValue(slot,out long offset) ? offset : null;
        if(value.Width!=64) return null;
        if(value.Operation is IlCast && value.Inputs[0].Width==64) return Offset(value.Inputs[0],state);
        if(value.Operation is IlBin { Op:BinOp.Add or BinOp.Sub } op) {
            var a=Offset(value.Inputs[0],state); var b=Offset(value.Inputs[1],state);
            if(a!=null && value.Inputs[1].Constant is { } right && right<=ulong.MaxValue)
                return op.Op==BinOp.Add ? checked(a.Value+unchecked((long)(ulong)right)) : checked(a.Value-unchecked((long)(ulong)right));
            if(op.Op==BinOp.Add && b!=null && value.Inputs[0].Constant is { } left && left<=ulong.MaxValue)
                return checked(b.Value+unchecked((long)(ulong)left));
        }
        return null;
    }
}
