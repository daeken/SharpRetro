using LiftIl;

namespace SharpRetro.Decompiler;

/// A byte-exact storage grouping, not a reconstruction of source object bounds.
public sealed record NativeFrameArray(long Offset,int Bytes,int ElementWidth,string Name);
public sealed record NativeFrameElement(NativeFrameArray Array,RecoveredValue? Index,ulong BiasElements);
public sealed class NativeFrameArrayPlan {
    public IReadOnlyList<NativeFrameArray> Arrays { get; }
    readonly Func<ulong,RecoveredValue,int,NativeFrameElement?> element;
    internal NativeFrameArrayPlan(IReadOnlyList<NativeFrameArray> arrays,Func<ulong,RecoveredValue,int,NativeFrameElement?> element) {
        Arrays=arrays; this.element=element;
    }
    public NativeFrameArray? Find(long offset,int width) => width>0 && width%8==0
        ? Arrays.FirstOrDefault(a=>offset>=a.Offset && (Int128)offset+width/8<=(Int128)a.Offset+a.Bytes) : null;
    public NativeFrameElement? Element(ulong block,RecoveredValue address,int width) => element(block,address,width);
}

/// Groups adjacent physical frame fields while retaining their exact coordinate
/// and zero initialization. Only Element authorizes C++ array indexing: it needs
/// a same-value unsigned bound that proves the entire access lies in the group.
/// Every unproven dynamic access continues to use the existing modular byte view.
public static class NativeFrameArrays {
    readonly record struct Affine(long Offset,RecoveredValue? Index=null,ulong Scale=0);
    sealed record Access(ulong Block,RecoveredValue Address,int Width);
    public static IReadOnlyDictionary<ulong,NativeFrameArrayPlan> Analyze(RecoveredFunctions recovered,
        IReadOnlyDictionary<ulong,NativeFunctionMemory> frames,IReadOnlyDictionary<ulong,FunctionSsa> ssas,
        IReadOnlyDictionary<ulong,NativePromotionPlan>? promotions=null) {
        var result=new Dictionary<ulong,NativeFrameArrayPlan>();
        foreach(var (entry,frame) in frames) {
            var function=recovered.Functions[entry];
            var ssa=ssas.GetValueOrDefault(entry);
            var promoted=promotions?.GetValueOrDefault(entry)?.Candidates ?? ssa?.Promoted ?? new HashSet<(long,int)>();
            var cells=frame.Scalars.Keys.Where(c=>c.Width>=32 && !promoted.Contains(c)).OrderBy(c=>c.Offset).ToArray();
            var arrays=new List<NativeFrameArray>();
            for(int i=0;i<cells.Length;) {
                int end=i+1;
                while(end<cells.Length && cells[end].Width==cells[i].Width && cells[end].Offset==cells[end-1].Offset+cells[end-1].Width/8) ++end;
                if(end-i>=2) arrays.Add(new(cells[i].Offset,checked((end-i)*(cells[i].Width/8)),cells[i].Width,$"stack_array_{-cells[i].Offset:x}"));
                i=end;
            }
            var affineFacts=new Dictionary<(ulong,int),Affine?>();
            var numericFacts=new Dictionary<int,ulong>();
            Affine? Parse(ulong address,RecoveredValue value) {
                var key=(address,value.Id);
                if(affineFacts.TryGetValue(key,out var old)) return old;
                var next=ParseCore(address,value); affineFacts[key]=next; return next;
            }
            Affine? ParseCore(ulong address,RecoveredValue value) {
                var block=recovered.Blocks[address];
                try { if(frame.Offset(block,value) is { } exact) return new(exact); } catch(OverflowException) { return null; }
                if(value.Width!=64) return null;
                if(value.Operation is IlCast { Kind:CastKind.Bitcast } && value.Inputs[0].Width==64) return Parse(address,value.Inputs[0]);
                if(value.Operation is not IlBin { Op:BinOp.Add or BinOp.Sub } bin) return null;
                var left=Parse(address,value.Inputs[0]);
                var right=bin.Op==BinOp.Add ? Parse(address,value.Inputs[1]) : null;
                if(left!=null && right!=null) return null;
                var basis=left??right;
                if(basis is not { } at) return null;
                var offset=left!=null ? value.Inputs[1] : value.Inputs[0];
                if(offset.Constant is { } bits && bits<=ulong.MaxValue) {
                    try { return at with { Offset=bin.Op==BinOp.Sub ? checked(at.Offset-unchecked((long)(ulong)bits)) : checked(at.Offset+unchecked((long)(ulong)bits)) }; }
                    catch(OverflowException) { return null; }
                }
                if(bin.Op==BinOp.Sub || at.Index!=null) return null;
                var scaled=Scale(offset);
                return scaled is { } dynamic ? at with { Index=dynamic.Index,Scale=dynamic.Scale } : null;
            }
            var accesses=new List<Access>();
            foreach(var address in function.Blocks) {
                var block=recovered.Blocks[address]; var seen=new HashSet<int>();
                void Visit(RecoveredValue value) {
                    if(!seen.Add(value.Id)) return;
                    if(value.Operation is IlLoad && ssa?.Load(value)==null) accesses.Add(new(address,value.Inputs[0],value.Width));
                    foreach(var child in value.Inputs) Visit(child);
                }
                foreach(var effect in block.Effects.Where(e=>!frame.RemovedEffects.Contains(e))) {
                    Visit(effect.Value);
                    if(effect.Address is { } target) { if(effect.Kind=="store" && ssa?.IsPromotedStore(effect)!=true) accesses.Add(new(address,target,effect.Value.Width)); Visit(target); }
                }
            }
            // A narrower dynamic access reveals a useful element granularity,
            // even when its index is unbounded and must retain byte addressing.
            for(int i=0;i<arrays.Count;++i) {
                var array=arrays[i]; int width=array.ElementWidth;
                foreach(var access in accesses) if(access.Width>=32 && access.Width<width && access.Width%8==0 &&
                    Parse(access.Block,access.Address) is { Index:not null } affine && affine.Offset>=array.Offset &&
                    (Int128)affine.Offset+access.Width/8<=(Int128)array.Offset+array.Bytes && affine.Scale==(ulong)(access.Width/8) &&
                    (affine.Offset-array.Offset)%(access.Width/8)==0 && array.Bytes%(access.Width/8)==0) width=access.Width;
                arrays[i]=array with { ElementWidth=width };
            }
            var dominates=new Dictionary<(ulong,ulong,ulong),bool>();
            bool EdgeDominates(ulong guard,ulong successor,ulong access) {
                var key=(guard,successor,access);
                if(dominates.TryGetValue(key,out bool old)) return old;
                var seen=new HashSet<ulong>(); var queue=new Queue<ulong>(); queue.Enqueue(function.Entry);
                while(queue.TryDequeue(out var current)) {
                    if(!seen.Add(current)) continue;
                    foreach(var next in recovered.Blocks[current].Block.FlowSuccessors.Where(function.Blocks.Contains))
                        if(current!=guard || next!=successor) queue.Enqueue(next);
                }
                return dominates[key]=!seen.Contains(access);
            }
            bool Same(ulong a,RecoveredValue x,ulong b,RecoveredValue y) {
                // Zero extension is lossless for the original narrow value.
                // Truncation and sign extension deliberately remain distinct.
                while(x.Operation is IlCast { Kind:CastKind.Zext } && x.Inputs[0].Width<x.Width) x=x.Inputs[0];
                while(y.Operation is IlCast { Kind:CastKind.Zext } && y.Inputs[0].Width<y.Width) y=y.Inputs[0];
                if(x.Width!=y.Width) return false;
                if(x.Id==y.Id) return true;
                var xd=ssa?.Resolve(a,x); var yd=ssa?.Resolve(b,y);
                return xd!=null && yd!=null && xd.Root==yd.Root && xd.Width==x.Width && yd.Width==y.Width;
            }
            ulong Maximum(ulong block,RecoveredValue index) {
                ulong maximum=NumericMaximum(index,numericFacts);
                if(ssa==null) return maximum;
                foreach(var address in function.Blocks.Where(a=>a!=block)) {
                    var guard=recovered.Blocks[address];
                    if(guard.Block.Kind!=TerminatorKind.Conditional || guard.Block.Target==guard.Block.Alternative) continue;
                    foreach(bool truth in new[]{true,false}) {
                        var condition=guard.Condition; bool sense=truth;
                        while(condition?.Operation is IlUn { Op:UnOp.Not }) { sense=!sense; condition=condition.Inputs[0]; }
                        if(condition?.Operation is not IlBin comparison || condition.Inputs[1].Constant is not { } bits || bits>ulong.MaxValue) continue;
                        var tested=condition.Inputs[0];
                        if(!Same(block,index,address,tested)) continue;
                        var op=comparison.Op;
                        if(!sense) op=op switch { BinOp.Ult=>BinOp.Uge,BinOp.Ule=>BinOp.Ugt,BinOp.Ugt=>BinOp.Ule,BinOp.Uge=>BinOp.Ult,BinOp.Eq=>BinOp.Ne,BinOp.Ne=>BinOp.Eq,_=>op };
                        ulong? bound=op switch { BinOp.Ule or BinOp.Eq=>(ulong)bits,BinOp.Ult when bits!=0=>(ulong)bits-1,_=>null };
                        if(bound is { } n && EdgeDominates(address,truth?guard.Block.Target:guard.Block.Alternative,block)) maximum=Math.Min(maximum,n);
                    }
                }
                return maximum;
            }
            NativeFrameElement? Element(ulong block,RecoveredValue address,int width) {
                if(Parse(block,address) is not { } affine) return null;
                var array=arrays.FirstOrDefault(a=>width==a.ElementWidth && affine.Offset>=a.Offset &&
                    (Int128)affine.Offset+width/8<=(Int128)a.Offset+a.Bytes && (affine.Offset-a.Offset)%(width/8)==0);
                if(array==null || width<=0) return null;
                ulong bias=(ulong)((affine.Offset-array.Offset)/(width/8));
                if(affine.Index==null) return new(array,null,bias);
                if(affine.Scale!=(ulong)(width/8)) return null;
                ulong maximum=Maximum(block,affine.Index);
                // UInt128 prevents the analysis itself from wrapping. These
                // bounds also prove the emitted index addition cannot wrap.
                if((UInt128)bias+maximum>=(ulong)(array.Bytes/(width/8))) return null;
                return new(array,affine.Index,bias);
            }
            result[entry]=new(arrays,Element);
        }
        return result;
    }
    static (RecoveredValue Index,ulong Scale)? Scale(RecoveredValue value) {
        if(value.Width!=64) return null;
        if(value.Operation is IlBin { Op:BinOp.Mul } && value.Inputs.Count==2) {
            if(value.Inputs[1].Constant is { } right && right>0 && right<=ulong.MaxValue && value.Inputs[0].Width==64) return (value.Inputs[0],(ulong)right);
            if(value.Inputs[0].Constant is { } left && left>0 && left<=ulong.MaxValue && value.Inputs[1].Width==64) return (value.Inputs[1],(ulong)left);
        }
        if(value.Operation is IlBin { Op:BinOp.Shl } && value.Inputs[1].Constant is { } shift && shift<64 && value.Inputs[0].Width==64) return (value.Inputs[0],1UL<<(int)shift);
        return (value,1);
    }
    static ulong NumericMaximum(RecoveredValue value,Dictionary<int,ulong> facts) {
        if(facts.TryGetValue(value.Id,out var old)) return old;
        var maximum=NumericMaximumCore(value,facts); facts[value.Id]=maximum; return maximum;
    }
    static ulong NumericMaximumCore(RecoveredValue value,Dictionary<int,ulong> facts) {
        // A saturated 64-bit bound is not a bound on a 128-bit value: applying
        // a subsequent wide shift could turn that saturation into a false
        // small bound. Keep wide arithmetic opaque until explicit narrowing.
        if(value.Width>64) return ulong.MaxValue;
        ulong maximum=value.Width<64 ? (1UL<<value.Width)-1 : ulong.MaxValue;
        if(value.Constant is { } constant) return constant<=maximum ? (ulong)constant : maximum;
        if(value.Operation is IlCast { Kind:CastKind.Zext } && value.Inputs[0].Width<value.Width) return NumericMaximum(value.Inputs[0],facts);
        if(value.Operation is IlCast { Kind:CastKind.Trunc }) return Math.Min(maximum,NumericMaximum(value.Inputs[0],facts));
        if(value.Operation is IlBin { Op:BinOp.And }) return Math.Min(NumericMaximum(value.Inputs[0],facts),NumericMaximum(value.Inputs[1],facts));
        if(value.Operation is IlBin { Op:BinOp.Shr } && value.Inputs[1].Constant is { } bits && bits<64) return NumericMaximum(value.Inputs[0],facts)>>(int)bits;
        return maximum;
    }
}
