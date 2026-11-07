using LiftIl;

namespace SharpRetro.Decompiler;

/// Ordinal in the original, deterministic block lowering, before any removals.
public readonly record struct NativeEffectPoint(ulong Block,int Index);

/// Backward must-overwrite/may-read analysis over bytes of native frame objects.
/// Calls are retained; unknown observations make every byte live. A short read
/// never counts as overwriting its full buffer. Checked memory is not eligible.
public static class NativeDeadEffects {
    public static HashSet<NativeEffectPoint> Find(LiftedProgram program,RecoveredFunctions recovered,
        IReadOnlyDictionary<ulong,NativeFunctionMemory> frames,IReadOnlySet<NativeEffectPoint> alreadyRemoved,
        IReadOnlyDictionary<ulong,TrustedStdioCall>? stdioCalls=null) {
        var removed=new HashSet<NativeEffectPoint>();
        foreach(var function in recovered.Functions.Values) {
            var frame=frames[function.Entry];
            if(frame.FrameSize>4096 || (long)frame.FrameSize*function.Blocks.Count>256*1024) continue;
            var unused=new HashSet<ValueEffect>(ReferenceEqualityComparer.Instance);
            foreach(var address in function.Blocks) {
                var block=recovered.Blocks[address]; var uses=new Dictionary<int,int>(); var seen=new HashSet<int>();
                void Count(RecoveredValue value) {
                    uses[value.Id]=uses.GetValueOrDefault(value.Id)+1;
                    if(seen.Add(value.Id)) foreach(var input in value.Inputs) Count(input);
                }
                foreach(var effect in block.Effects) { Count(effect.Value); if(effect.Address!=null) Count(effect.Address); }
                if(block.Condition!=null) Count(block.Condition);
                if(block.Block.Kind==TerminatorKind.Return) Count(block.Target!);
                foreach(var slot in block.LiveOut) Count(block.Output(slot));
                foreach(var effect in block.Effects)
                    if(!frame.RemovedEffects.Contains(effect) && effect.Kind=="evaluate" && effect.Value.Operation is IlLoad &&
                       uses.GetValueOrDefault(effect.Value.Id)==1 && frame.Offset(block,effect.Value.Inputs[0]) is { } offset &&
                       frame.Scalars.ContainsKey((offset,effect.Value.Width))) unused.Add(effect);
            }
            var entries=function.Blocks.ToDictionary(a=>a,_=>new HashSet<long>());
            bool changed;
            do {
                changed=false;
                foreach(var address in function.Blocks.OrderDescending()) {
                    var block=recovered.Blocks[address]; var live=new HashSet<long>();
                    foreach(var target in block.Block.FlowSuccessors.Where(function.Blocks.Contains)) live.UnionWith(entries[target]);
                    Walk(block,live,collect:false);
                    if(!live.SetEquals(entries[address])) { entries[address]=live; changed=true; }
                }
            } while(changed);
            foreach(var address in function.Blocks) {
                var block=recovered.Blocks[address]; var live=new HashSet<long>();
                foreach(var target in block.Block.FlowSuccessors.Where(function.Blocks.Contains)) live.UnionWith(entries[target]);
                Walk(block,live,collect:true);
            }

            void Walk(ValueBlock block,HashSet<long> live,bool collect) {
                var address=block.Block.Address;
                void All() { for(long offset=-frame.FrameSize;offset<0;offset++) live.Add(offset); }
                bool Range(RecoveredValue pointer,ulong bytes,out long start) {
                    start=frame.Offset(block,pointer) ?? long.MaxValue;
                    return bytes<=(ulong)frame.FrameSize && start>=-frame.FrameSize && start<=0 && bytes<=(ulong)(-start);
                }
                void Read(RecoveredValue pointer,ulong? count) {
                    if(count==0) return;
                    if(count is not { } bytes || !Range(pointer,bytes,out long start)) { All(); return; }
                    for(long offset=start;offset<start+(long)bytes;offset++) live.Add(offset);
                }
                if(block.Block.Kind==TerminatorKind.Call) {
                    program.Imports.TryGetValue(block.Block.Target,out string? symbol);
                    if(stdioCalls?.TryGetValue(address,out var stdio)==true) {
                        if(stdio.Read is not { } range) All();
                        else if(range.Bytes!=0) {
                            if(range.Offset>=-frame.FrameSize && range.Offset<=0 && range.Bytes<=(ulong)(-range.Offset))
                                for(long offset=range.Offset;offset<range.Offset+(long)range.Bytes;offset++) live.Add(offset);
                            else All();
                        }
                    } else if(symbol=="_read") {
                        // Kernel writes may be partial, including zero bytes.
                    } else if(symbol=="_write") {
                        var count=block.Output(new(SlotKind.Local,2));
                        Read(block.Output(new(SlotKind.Local,6)),count.Constant is { } n && n<=ulong.MaxValue ? (ulong)n : null);
                    } else All(); // Includes private callees: they may read inputs.
                } else if(block.Block.FlowSuccessors.Any(t=>!function.Blocks.Contains(t))) All();
                // Translate current positions back to original effect ordinals.
                var indices=new int[block.Effects.Count]; int original=0;
                for(int i=0;i<indices.Length;i++) {
                    while(alreadyRemoved.Contains(new(address,original))) original++;
                    indices[i]=original++;
                }
                for(int i=block.Effects.Count-1;i>=0;i--) {
                    var effect=block.Effects[i];
                    if(frame.RemovedEffects.Contains(effect)) continue;
                    if(unused.Contains(effect)) { if(collect) removed.Add(new(address,indices[i])); continue; }
                    if(effect.Kind=="evaluate" && effect.Value.Operation is IlLoad)
                        Read(effect.Value.Inputs[0],(ulong)(effect.Value.Width/8));
                    else if(effect.Kind=="store" && Range(effect.Address!,(ulong)(effect.Value.Width/8),out long start)) {
                        long end=start+effect.Value.Width/8;
                        bool observed=live.Any(offset=>offset>=start && offset<end);
                        if(!observed && collect) removed.Add(new(address,indices[i]));
                        live.RemoveWhere(offset=>offset>=start && offset<end);
                    }
                }
            }
        }
        return removed;
    }
}
