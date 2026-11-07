using LiftIl;

namespace SharpRetro.Decompiler;

public sealed record NativePromotionPlan(
    IReadOnlySet<(long Offset,int Width)> Candidates,
    IReadOnlyDictionary<int,(long Offset,int Width)> Loads,
    IReadOnlyDictionary<ValueEffect,(long Offset,int Width)> Stores,
    IReadOnlyDictionary<(long Offset,int Width),string> Rejected);

/// Exact private cells which may be represented as values instead of addressable
/// storage. This is an overlay: the original ValueBlocks and load evaluation
/// points remain unchanged. The existing native-object profile is required:
/// external values cannot name a fresh frame by guessing its numeric address.
/// Frame-origin bits stay tainted through integer operations, including casts.
/// Unknown provenance/escapes, overlapping views and uninitialized reads retain
/// memory. A dynamic frame access excludes every cell in its possible range.
public static class NativeScalarPromotion {
    enum Origin { External, Frame, Unknown }
    readonly record struct Fact(Origin Origin,long Low=0,long High=0,ulong Minimum=0,ulong Maximum=ulong.MaxValue,bool Bounded=true) {
        public bool MayFrame => Origin!=Origin.External;
        public static Fact Frame(long offset) => new(Origin.Frame,offset,offset);
        public static Fact Scalar(int width) => new(Origin.External,Maximum:width<64 ? (1UL<<width)-1 : ulong.MaxValue,Bounded:width<=64);
    }
    readonly record struct Cell(long Offset,int Width) {
        public long End => Offset+Width/8;
    }
    sealed class State {
        public Dictionary<ValueSlot,Fact> Registers=[];
        public Dictionary<Cell,Fact> Memory=[];
        public HashSet<Cell> Initialized=[];
        public bool Exposed;
        public State Copy() => new() { Registers=new(Registers),Memory=new(Memory),Initialized=new(Initialized),Exposed=Exposed };
        public bool Meet(State other) {
            bool changed=false;
            foreach(var slot in Registers.Keys.Union(other.Registers.Keys).ToArray()) {
                var old=Registers.GetValueOrDefault(slot,new(Origin.Unknown));
                var next=Join(old,other.Registers.GetValueOrDefault(slot,new(Origin.Unknown)),widen:true);
                if(next!=old || !Registers.ContainsKey(slot)) { Registers[slot]=next; changed=true; }
            }
            foreach(var cell in Memory.Keys.ToArray()) {
                if(!other.Memory.TryGetValue(cell,out var fact)) { Memory.Remove(cell); changed=true; }
                else {
                    var next=Join(Memory[cell],fact,widen:true);
                    if(next!=Memory[cell]) { Memory[cell]=next; changed=true; }
                }
            }
            int count=Initialized.Count; Initialized.IntersectWith(other.Initialized); changed|=count!=Initialized.Count;
            if(other.Exposed && !Exposed) { Exposed=true; changed=true; }
            return changed;
        }
    }

    public static Dictionary<ulong,NativePromotionPlan> Analyze(LiftedProgram program,RecoveredFunctions recovered,
        IReadOnlyDictionary<ulong,NativeFunctionMemory> frames) {
        var result=new Dictionary<ulong,NativePromotionPlan>();
        foreach(var function in recovered.Functions.Values) result[function.Entry]=AnalyzeFunction(function,frames[function.Entry]);
        return result;

        NativePromotionPlan AnalyzeFunction(RecoveredFunction function,NativeFunctionMemory frame) {
            var cells=frame.Scalars.Keys.Select(c=>new Cell(c.Offset,c.Width)).ToHashSet();
            var rejected=new Dictionary<Cell,string>();
            var loads=new Dictionary<int,Cell>();
            var stores=new Dictionary<ValueEffect,Cell>(ReferenceEqualityComparer.Instance);
            if(frame.FrameSize>4096 || (long)frame.FrameSize*function.Blocks.Count>256*1024)
                return new(new HashSet<(long,int)>(),new Dictionary<int,(long,int)>(),
                    new Dictionary<ValueEffect,(long,int)>(ReferenceEqualityComparer.Instance),
                    cells.ToDictionary(c=>(c.Offset,c.Width),_=>"promotion analysis limit"));
            var start=new State();
            foreach(var slot in function.Parameters.Concat(function.Blocks.SelectMany(a=>recovered.Blocks[a].Inputs.Keys)).Distinct())
                start.Registers[slot]=Fact.Scalar(slot.Width);
            start.Registers[FunctionRecovery.Stack]=Fact.Frame(0);
            var entries=new Dictionary<ulong,State> { [function.Entry]=start };
            var pending=new Queue<ulong>(); pending.Enqueue(function.Entry);
            while(pending.TryDequeue(out var address)) {
                var block=recovered.Blocks[address];
                var next=Transfer(block,entries[address],collect:false);
                foreach(var target in block.Block.FlowSuccessors.Where(function.Blocks.Contains)) {
                    var edge=next.Copy(); Refine(block,target,edge);
                    if(!entries.TryGetValue(target,out var old)) { entries[target]=edge; pending.Enqueue(target); }
                    else if(old.Meet(edge)) pending.Enqueue(target);
                }
            }
            bool exposed=false;
            foreach(var address in function.Blocks) {
                if(!entries.TryGetValue(address,out var entry)) { RejectAll("unreachable promotion state"); continue; }
                exposed|=Transfer(recovered.Blocks[address],entry,collect:true).Exposed;
            }
            if(exposed) RejectAll("frame address or frame-origin bits may escape");
            var candidates=cells.Except(rejected.Keys).ToHashSet();
            return new(candidates.Select(c=>(c.Offset,c.Width)).ToHashSet(),
                loads.Where(p=>candidates.Contains(p.Value)).ToDictionary(p=>p.Key,p=>(p.Value.Offset,p.Value.Width)),
                stores.Where(p=>candidates.Contains(p.Value)).ToDictionary(p=>p.Key,p=>(p.Value.Offset,p.Value.Width),(IEqualityComparer<ValueEffect>)ReferenceEqualityComparer.Instance),
                rejected.ToDictionary(p=>(p.Key.Offset,p.Key.Width),p=>p.Value));

            void RejectAll(string reason) { foreach(var cell in cells) rejected.TryAdd(cell,reason); }
            void Touch(Fact address,ulong? bytes,Cell? exact,bool collect,string reason) {
                if(!collect || bytes==0 || address.Origin==Origin.External) return;
                if(address.Origin==Origin.Unknown || bytes==null) { RejectAll(reason); return; }
                Int128 low=address.Low, end=(Int128)address.High+bytes.Value;
                foreach(var cell in cells)
                    if(cell!=exact && low<cell.End && cell.Offset<end) rejected.TryAdd(cell,reason);
            }
            State Transfer(ValueBlock block,State entry,bool collect) {
                var state=entry.Copy(); var cache=new Dictionary<int,Fact>();
                Fact Eval(RecoveredValue value) {
                    if(cache.TryGetValue(value.Id,out var old)) return old;
                    Fact answer;
                    if(frame.Offset(block,value) is { } offset) answer=Fact.Frame(offset);
                    else if(value.Constant is { } bits) answer=bits<=ulong.MaxValue
                        ? new(Origin.External,Minimum:(ulong)bits,Maximum:(ulong)bits) : Fact.Scalar(value.Width);
                    else if(value.Slot is { } slot) answer=entry.Registers.GetValueOrDefault(slot,new(Origin.Unknown));
                    else if(value.Operation is IlLoad) {
                        var address=Eval(value.Inputs[0]);
                        Cell? exact=address.Origin==Origin.Frame && address.Low==address.High ? new(address.Low,value.Width) : null;
                        bool candidate=exact is { } cell && cells.Contains(cell);
                        Touch(address,(ulong)(value.Width/8),candidate ? exact : null,collect,$"overlapping or dynamic frame read at 0x{block.Block.Address:x}, value {value.Id}");
                        if(candidate && collect) {
                            loads[value.Id]=exact!.Value;
                            if(!state.Initialized.Contains(exact.Value)) rejected.TryAdd(exact.Value,"read before definite full-width initialization");
                        }
                        answer=new(Origin.Unknown);
                        if(exact is { } at && state.Memory.TryGetValue(at,out var stored)) answer=stored;
                        else if(exact is { } part && state.Memory.Any(p=>p.Key.Offset<=part.Offset && p.Key.End>=part.End && p.Value.Origin==Origin.External))
                            answer=Fact.Scalar(value.Width);
                        // Native frame storage is freshly value-initialized.
                        // Until frame-origin bits are stored/copied/exposed,
                        // even a dynamically selected in-frame word cannot
                        // contain such bits. This is provenance only: scalar
                        // promotion still requires a definite original store.
                        else if(address.Origin==Origin.Frame && !state.Exposed && address.Low>=-frame.FrameSize &&
                                (Int128)address.High+value.Width/8<=0) answer=Fact.Scalar(value.Width);
                        else if(address.Origin==Origin.External && !state.Exposed) answer=Fact.Scalar(value.Width);
                        if(answer.Origin==Origin.External) answer=Clamp(answer,value.Width);
                    } else if(value.Operation is IlCast cast) {
                        var input=Eval(value.Inputs[0]);
                        if(input.Origin==Origin.Frame && value.Width==64 && value.Inputs[0].Width==64) answer=input;
                        else if(input.MayFrame) answer=new(Origin.Unknown);
                        else if(cast.Kind==CastKind.Sext && value.Width>value.Inputs[0].Width &&
                                (!input.Bounded || value.Inputs[0].Width<=64 && input.Maximum>=(1UL<<(value.Inputs[0].Width-1)))) answer=Fact.Scalar(value.Width);
                        else answer=Clamp(input,value.Width);
                    } else if(value.Operation is IlIfV) {
                        var condition=Eval(value.Inputs[0]);
                        var yes=Eval(value.Inputs[1]); var no=Eval(value.Inputs[2]);
                        // Selecting otherwise external constants can encode a
                        // frame-derived bit. Preserve that implicit dependence.
                        answer=condition.MayFrame ? new(Origin.Unknown) : Join(yes,no,widen:false);
                    } else if(value.Operation is IlBin bin) answer=Binary(value,bin.Op,Eval(value.Inputs[0]),Eval(value.Inputs[1]));
                    else {
                        var inputs=value.Inputs.Select(Eval).ToArray();
                        answer=inputs.Any(f=>f.MayFrame) ? new(Origin.Unknown) : Fact.Scalar(value.Width);
                    }
                    cache[value.Id]=answer; return answer;
                }
                void Kill(Fact address,ulong? bytes) {
                    if(bytes==0 || address.Origin==Origin.External) return;
                    if(address.Origin==Origin.Unknown || bytes==null) { state.Memory.Clear(); state.Initialized.Clear(); return; }
                    Int128 end=(Int128)address.High+bytes.Value;
                    bool Overlap(Cell c) => address.Low<c.End && c.Offset<end;
                    foreach(var cell in state.Memory.Keys.Where(Overlap).ToArray()) state.Memory.Remove(cell);
                    state.Initialized.RemoveWhere(Overlap);
                }
                foreach(var effect in block.Effects) {
                    if(frame.RemovedEffects.Contains(effect)) continue;
                    var value=Eval(effect.Value);
                    if(effect.Kind!="store") continue;
                    var address=Eval(effect.Address!); ulong bytes=(ulong)(effect.Value.Width/8);
                    Cell? exact=address.Origin==Origin.Frame && address.Low==address.High ? new(address.Low,effect.Value.Width) : null;
                    bool candidate=exact is { } cell && cells.Contains(cell);
                    Touch(address,bytes,candidate ? exact : null,collect,"overlapping or dynamic frame write");
                    Kill(address,bytes);
                    if(exact is { } at) {
                        state.Memory[at]=value;
                        if(cells.Contains(at)) state.Initialized.Add(at);
                        if(candidate && collect) stores[effect]=at;
                    }
                    // Even an internal spill of a computed frame address makes
                    // later partial loads/reconstruction hard; retain the frame.
                    if(value.MayFrame) state.Exposed=true;
                }
                if(block.Condition!=null && Eval(block.Condition).MayFrame) {
                    // A successor can encode this bit into ordinary constants.
                    // Rather than track control dependence through the CFG,
                    // retain the complete physical frame on either path.
                    state.Exposed=true;
                }
                var outputs=block.Outputs.ToDictionary(p=>p.Key,p=>Eval(p.Value));
                foreach(var (slot,value) in outputs) state.Registers[slot]=value;
                if(block.Block.Kind==TerminatorKind.Return) {
                    if(function.Results.Any(s=>Eval(block.Output(s)).MayFrame)) state.Exposed=true;
                } else if(block.Block.Kind==TerminatorKind.Call) {
                    var inputs=recovered.Functions.TryGetValue(block.Block.Target,out var callee) ? callee.Parameters
                        : ApplicationImports.Inputs(program.Imports[block.Block.Target],recovered.Windows);
                    Fact Argument(int register) => state.Registers.GetValueOrDefault(new(SlotKind.Local,register),new(Origin.Unknown));
                    var arguments=inputs.Where(s=>s!=FunctionRecovery.Stack).Select(s=>state.Registers.GetValueOrDefault(s,new(Origin.Unknown))).ToArray();
                    string? symbol=program.Imports.GetValueOrDefault(block.Block.Target);
                    bool memory=recovered.Windows && symbol is "memcpy" or "memmove" or "memset";
                    Fact? returned=null;
                    if(memory) {
                        var count=Argument(8); ulong? bytes=count.Origin==Origin.External && count.Bounded ? count.Maximum : null;
                        if(count.MayFrame) state.Exposed=true;
                        var destination=Argument(1); var source=Argument(2);
                        returned=destination;
                        if(symbol!="memset") {
                            Touch(source,bytes,null,collect,"import reads frame bytes");
                            if(source.MayFrame && (source.Origin==Origin.Unknown || bytes==null ||
                               !FrameContentsExternal(source,bytes.Value,state))) state.Exposed=true;
                        } else if(source.MayFrame) state.Exposed=true;
                        Touch(destination,bytes,null,collect,"import writes frame bytes"); Kill(destination,bytes);
                    } else if(arguments.Any(a=>a.MayFrame)) {
                        state.Exposed=true; state.Memory.Clear(); state.Initialized.Clear();
                        if(collect) RejectAll("frame may be observed by an opaque call");
                    }
                    var writes=callee?.MayWrite ?? FunctionAbi.ImportWrites(recovered.Windows);
                    foreach(var slot in writes) state.Registers[slot]=state.Exposed ? new(Origin.Unknown) : Fact.Scalar(slot.Width);
                    if(returned is { } pointer) state.Registers[FunctionRecovery.Accumulator]=pointer;
                } else if(block.Block.FlowSuccessors.Any(t=>!function.Blocks.Contains(t))) {
                    state.Exposed=true; if(collect) RejectAll("tail-call frame observation is not modeled");
                }
                return state;
            }
            void Refine(ValueBlock block,ulong target,State state) {
                var condition=block.Condition; bool truth=target==block.Block.Target;
                if(block.Block.Target==block.Block.Alternative) return;
                while(condition?.Operation is IlUn { Op:UnOp.Not }) { truth=!truth; condition=condition.Inputs[0]; }
                if(condition?.Operation is not IlBin bin || condition.Inputs[1].Constant is not { } bound || bound>ulong.MaxValue) return;
                var operand=condition.Inputs[0]; ulong n=(ulong)bound;
                var op=bin.Op;
                if(!truth) op=op switch { BinOp.Ult=>BinOp.Uge,BinOp.Ule=>BinOp.Ugt,BinOp.Ugt=>BinOp.Ule,BinOp.Uge=>BinOp.Ult,BinOp.Eq=>BinOp.Ne,BinOp.Ne=>BinOp.Eq,_=>op };
                ulong low=0,high=operand.Width<64 ? (1UL<<operand.Width)-1 : ulong.MaxValue;
                switch(op) {
                    case BinOp.Ule: high=Math.Min(high,n); break;
                    case BinOp.Ult when n!=0: high=Math.Min(high,n-1); break;
                    case BinOp.Uge: low=n; break;
                    case BinOp.Ugt when n!=ulong.MaxValue: low=n+1; break;
                    case BinOp.Eq: low=n; high=n; break;
                    default: return;
                }
                foreach(var slot in block.Inputs.Keys.Concat(block.Outputs.Keys).Distinct().Where(s=>block.Output(s).Id==operand.Id)) {
                    var old=state.Registers.GetValueOrDefault(slot,new(Origin.Unknown));
                    if(old.Origin!=Origin.External) continue;
                    ulong min=old.Bounded ? Math.Max(old.Minimum,low) : low,max=old.Bounded ? Math.Min(old.Maximum,high) : high;
                    if(min<=max) state.Registers[slot]=old with { Minimum=min,Maximum=max,Bounded=true };
                }
            }
        }
    }

    static Fact Clamp(Fact fact,int width) {
        if(!fact.Bounded) return Fact.Scalar(width);
        ulong maximum=width<64 ? (1UL<<width)-1 : ulong.MaxValue;
        if(fact.Maximum>maximum) return Fact.Scalar(width);
        return fact;
    }
    static Fact Join(Fact a,Fact b,bool widen) {
        if(a==b) return a;
        if(a.Origin!=b.Origin || a.Origin==Origin.Unknown) return new(Origin.Unknown);
        if(a.Origin==Origin.Frame) {
            if(b.Low>=a.Low && b.High<=a.High) return a;
            return widen ? new(Origin.Unknown) : new(Origin.Frame,Math.Min(a.Low,b.Low),Math.Max(a.High,b.High));
        }
        if(!a.Bounded || !b.Bounded) return new(Origin.External,Bounded:false);
        if(b.Minimum>=a.Minimum && b.Maximum<=a.Maximum) return a;
        return widen ? new(Origin.External,Minimum:b.Minimum<a.Minimum ? 0 : a.Minimum,Maximum:b.Maximum>a.Maximum ? ulong.MaxValue : a.Maximum)
            : new(Origin.External,Minimum:Math.Min(a.Minimum,b.Minimum),Maximum:Math.Max(a.Maximum,b.Maximum));
    }
    static Fact Binary(RecoveredValue value,BinOp op,Fact a,Fact b) {
        Fact Move(Fact frame,Fact offset,bool subtract) {
            if(offset.Origin!=Origin.External || !offset.Bounded) return new(Origin.Unknown);
            Int128 low,high;
            if(offset.Minimum==offset.Maximum) {
                long delta=unchecked((long)offset.Minimum);
                low=(Int128)frame.Low+(subtract ? -(Int128)delta : delta);
                high=(Int128)frame.High+(subtract ? -(Int128)delta : delta);
            } else {
                if(offset.Maximum>long.MaxValue) return new(Origin.Unknown);
                low=(Int128)frame.Low+(subtract ? -(Int128)offset.Maximum : offset.Minimum);
                high=(Int128)frame.High+(subtract ? -(Int128)offset.Minimum : offset.Maximum);
            }
            return low>=long.MinValue && high<=long.MaxValue ? new(Origin.Frame,(long)low,(long)high) : new(Origin.Unknown);
        }
        if(value.Width==64 && op is BinOp.Add or BinOp.Sub && a.Origin==Origin.Frame) return Move(a,b,op==BinOp.Sub);
        if(value.Width==64 && op==BinOp.Add && b.Origin==Origin.Frame) return Move(b,a,false);
        if(a.MayFrame || b.MayFrame) return new(Origin.Unknown);
        if(!a.Bounded || !b.Bounded) {
            if(op==BinOp.And && (a.Bounded || b.Bounded)) return new(Origin.External,Maximum:a.Bounded ? a.Maximum : b.Maximum);
            return Fact.Scalar(value.Width);
        }
        ulong maximum=value.Width<64 ? (1UL<<value.Width)-1 : ulong.MaxValue;
        UInt128 low=0,high=maximum;
        switch(op) {
            case BinOp.Add: low=(UInt128)a.Minimum+b.Minimum; high=(UInt128)a.Maximum+b.Maximum; break;
            case BinOp.Sub when a.Minimum>=b.Maximum: low=a.Minimum-b.Maximum; high=a.Maximum-b.Minimum; break;
            case BinOp.Mul: low=(UInt128)a.Minimum*b.Minimum; high=(UInt128)a.Maximum*b.Maximum; break;
            case BinOp.And: high=Math.Min(a.Maximum,b.Maximum); break;
            case BinOp.Shl when b.Minimum==b.Maximum && b.Maximum<64:
                low=(UInt128)a.Minimum<<(int)b.Maximum; high=(UInt128)a.Maximum<<(int)b.Maximum; break;
            case BinOp.Shr when b.Minimum==b.Maximum && b.Maximum<64:
                low=a.Minimum>>(int)b.Maximum; high=a.Maximum>>(int)b.Maximum; break;
            default: return Fact.Scalar(value.Width);
        }
        return high<=maximum ? new(Origin.External,Minimum:(ulong)low,Maximum:(ulong)high) : Fact.Scalar(value.Width);
    }
    static bool FrameContentsExternal(Fact source,ulong bytes,State state) {
        if(bytes==0) return true;
        if(source.Origin!=Origin.Frame || source.Low!=source.High || bytes>4096) return false;
        for(long delta=0;delta<(long)bytes;delta++) {
            Int128 at=(Int128)source.Low+delta;
            if(!state.Memory.Any(p=>p.Key.Offset<=at && p.Key.End>at && p.Value.Origin==Origin.External)) return false;
        }
        return true;
    }
}
