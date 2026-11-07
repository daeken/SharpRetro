using LiftIl;

namespace SharpRetro.Decompiler;

public readonly record struct FrameReadRange(long Offset,ulong Bytes);
// A null read range means unknown/all-frame observation. A zero-byte range
// means no guest-frame reads, not that the call itself can be removed.
public sealed record TrustedStdioCall(FrameReadRange? Read,bool Factory=false);
public sealed record FrameProofResult(IReadOnlyDictionary<ulong,IReadOnlySet<ValueSlot>> Preserved,
    IReadOnlySet<ulong> PrivateMemory,IReadOnlyDictionary<ulong,TrustedStdioCall> StdioCalls);

/// Must facts over fresh native frames, not an ABI-preservation heuristic.
/// Unknown writes discard memory identities; exposures are sticky across joins
/// and calls. Only the explicit noncapturing bridge effects below are trusted.
public static class FrameProofs {
    enum Kind { Unknown, Entry, Frame, Constant, Count, Stream }
    readonly record struct Origin(Kind Kind,ValueSlot Slot=default,long Offset=0,ulong Bits=0) {
        public bool MayFrame => Kind is Kind.Unknown or Kind.Frame or Kind.Count;
        public ulong? Maximum => Kind is Kind.Constant or Kind.Count ? Bits : null;
    }
    readonly record struct Cell(long Offset,int Bytes);
    sealed class State {
        public Dictionary<ValueSlot,Origin> Registers=[];
        public Dictionary<Cell,Origin> Memory=[];
        public bool Escaped;
        public State Copy() => new() { Registers=new(Registers),Memory=new(Memory),Escaped=Escaped };
        public bool Meet(State other) {
            bool changed=false;
            foreach(var key in Registers.Keys.ToArray())
                if(!other.Registers.TryGetValue(key,out var value) || value!=Registers[key]) { Registers.Remove(key); changed=true; }
            foreach(var key in Memory.Keys.ToArray())
                if(!other.Memory.TryGetValue(key,out var value) || value!=Memory[key]) { Memory.Remove(key); changed=true; }
            if(other.Escaped && !Escaped) { Escaped=true; changed=true; }
            return changed;
        }
    }

    public static FrameProofResult Analyze(LiftedProgram program,RecoveredFunctions recovered,
        IReadOnlyDictionary<ulong,NativeFunctionMemory> frames) {
        var preserved=recovered.Functions.Keys.ToDictionary(a=>a,_=>new HashSet<ValueSlot>());
        var candidates=FunctionAbi.Preserved(program.Image);
        var privateMemory=new HashSet<ulong>();
        var stdioCalls=new Dictionary<ulong,TrustedStdioCall>();
        var entrySlots=Enumerable.Range(0,16).Select(i=>new ValueSlot(SlotKind.Local,i))
            .Concat(recovered.Blocks.Values.SelectMany(b=>b.Inputs.Keys)).Distinct().ToArray();
        // Start with no callee facts. Recursive cycles cannot prove themselves.
        bool changed;
        do {
            changed=false;
            foreach(var function in recovered.Functions.Values.Reverse()) {
                var (identities,isPrivate)=AnalyzeFunction(function);
                int count=preserved[function.Entry].Count;
                preserved[function.Entry].UnionWith(identities);
                changed|=count!=preserved[function.Entry].Count;
                if(isPrivate) changed|=privateMemory.Add(function.Entry);
            }
        } while(changed);
        return new(preserved.ToDictionary(p=>p.Key,p=>(IReadOnlySet<ValueSlot>)p.Value),privateMemory,stdioCalls);

        (HashSet<ValueSlot>,bool) AnalyzeFunction(RecoveredFunction function) {
            var frame=frames[function.Entry];
            var start=new State();
            foreach(var slot in entrySlots)
                start.Registers[slot]=new(Kind.Entry,Slot:slot);
            start.Registers[FunctionRecovery.Stack]=new(Kind.Frame);
            var entries=new Dictionary<ulong,State> { [function.Entry]=start };
            var pending=new Queue<ulong>(); pending.Enqueue(function.Entry);
            while(pending.TryDequeue(out var address)) {
                var next=Transfer(recovered.Blocks[address],entries[address]).State;
                foreach(var target in recovered.Blocks[address].Block.FlowSuccessors.Where(function.Blocks.Contains)) {
                    if(!entries.TryGetValue(target,out var old)) { entries[target]=next.Copy(); pending.Enqueue(target); }
                    else if(old.Meet(next)) pending.Enqueue(target);
                }
            }
            var result=candidates.Where(function.MayWrite.Contains).ToHashSet(); bool hasReturn=false,privateEffects=true;
            foreach(var address in function.Blocks) {
                if(!entries.TryGetValue(address,out var entry)) { result.Clear(); privateEffects=false; continue; }
                var block=recovered.Blocks[address]; var next=Transfer(block,entry);
                if(next.Stdio is { } stdio) stdioCalls[address]=stdio;
                privateEffects&=next.Private;
                if(block.Block.Kind==TerminatorKind.Return) {
                    hasReturn=true;
                    result.RemoveWhere(s=>next.State.Registers.GetValueOrDefault(s)!=new Origin(Kind.Entry,Slot:s));
                } else if(block.Block.FlowSuccessors.Any(t=>!function.Blocks.Contains(t))) {
                    // Tail contracts require a separate proof of argument mapping.
                    result.Clear(); privateEffects=false;
                }
            }
            if(!hasReturn) result.Clear();
            return (result,privateEffects);

            (State State,bool Private,TrustedStdioCall? Stdio) Transfer(ValueBlock block,State entry) {
                var state=entry.Copy(); var cache=new Dictionary<int,Origin>(); bool isPrivate=true;
                bool revoked=false; TrustedStdioCall? stdio=null;
                void RevokeStreams() {
                    // Stream is only a nullable validity predicate, never an
                    // allocation identity. Closing/mutation revokes ALL aliases.
                    revoked=true;
                    foreach(var slot in state.Registers.Where(p=>p.Value.Kind==Kind.Stream).Select(p=>p.Key).ToArray()) state.Registers.Remove(slot);
                    foreach(var cell in state.Memory.Where(p=>p.Value.Kind==Kind.Stream).Select(p=>p.Key).ToArray()) state.Memory.Remove(cell);
                }
                Origin Eval(RecoveredValue value) {
                    Origin Valid(Origin origin) => revoked && origin.Kind==Kind.Stream ? default : origin;
                    if(cache.TryGetValue(value.Id,out var cached)) return Valid(cached);
                    Origin answer=default;
                    if(value.Constant is { } bits && bits<=ulong.MaxValue) answer=new(Kind.Constant,Bits:(ulong)bits);
                    else if(value.Slot is { } slot) answer=Valid(entry.Registers.GetValueOrDefault(slot));
                    else if(value.Operation is IlLoad) {
                        var address=Eval(value.Inputs[0]);
                        if(address.Kind==Kind.Frame && InFrame(address.Offset,(ulong)(value.Width/8)))
                            answer=state.Memory.GetValueOrDefault(new(address.Offset,value.Width/8));
                    } else if(value.Operation is IlCast cast) {
                        var input=Eval(value.Inputs[0]);
                        if(value.Width==64 && value.Inputs[0].Width==64) answer=input;
                        else if(cast.Kind!=CastKind.Sext && input.Maximum is { } max)
                            answer=new(Kind.Count,Bits:value.Width>=64 ? max : Math.Min(max,(1UL<<value.Width)-1));
                        else if(cast.Kind==CastKind.Zext && value.Inputs[0].Width<64)
                            answer=new(Kind.Count,Bits:(1UL<<value.Inputs[0].Width)-1);
                    } else if(value.Operation is IlBin { Op:BinOp.Add or BinOp.Sub or BinOp.Mul } op) {
                        var a=Eval(value.Inputs[0]); var b=Eval(value.Inputs[1]);
                        if(value.Width==64 && a.Kind==Kind.Frame && b.Kind==Kind.Constant && op.Op!=BinOp.Mul) answer=Add(a,b.Bits,op.Op==BinOp.Sub);
                        else if(value.Width==64 && op.Op==BinOp.Add && b.Kind==Kind.Frame && a.Kind==Kind.Constant) answer=Add(b,a.Bits,false);
                        else if(value.Width<=64 && a.Maximum is { } left && b.Maximum is { } right && op.Op is BinOp.Add or BinOp.Mul) {
                            UInt128 max=op.Op==BinOp.Add ? (UInt128)left+right : (UInt128)left*right;
                            if(max<=(value.Width==64 ? ulong.MaxValue : (UInt128.One<<value.Width)-1)) answer=new(Kind.Count,Bits:(ulong)max);
                        }
                    } else if(value.Operation is IlIfV) {
                        var a=Eval(value.Inputs[1]); var b=Eval(value.Inputs[2]); if(a==b) answer=a;
                    }
                    // One-bit predicates and narrow integer carriers provide
                    // unsigned upper bounds, without any equality/nonzero claim.
                    if(answer.Kind==Kind.Unknown && value.Width<=32) answer=new(Kind.Count,Bits:(1UL<<value.Width)-1);
                    cache[value.Id]=answer; return Valid(answer);
                }
                Origin Add(Origin a,ulong bits,bool subtract) {
                    try { return new(Kind.Frame,Offset:subtract ? checked(a.Offset-unchecked((long)bits)) : checked(a.Offset+unchecked((long)bits))); }
                    catch(OverflowException) { return default; }
                }
                bool InFrame(long offset,ulong bytes) => bytes<=(ulong)frame.FrameSize && offset>=-frame.FrameSize && offset<=0 && bytes<=(ulong)(-offset);
                void Kill(Origin address,ulong? bytes) {
                    if(bytes==0) return;
                    if(address.Kind!=Kind.Frame || bytes is not { } size || !InFrame(address.Offset,size)) { state.Memory.Clear(); return; }
                    long end=address.Offset+(long)size;
                    foreach(var cell in state.Memory.Keys.Where(c=>c.Offset<end && address.Offset<c.Offset+c.Bytes).ToArray()) state.Memory.Remove(cell);
                }
                foreach(var effect in block.Effects) {
                    if(frame.RemovedEffects.Contains(effect)) continue;
                    var value=Eval(effect.Value);
                    if(effect.Kind!="store") continue;
                    var address=Eval(effect.Address!); ulong bytes=(ulong)(effect.Value.Width/8);
                    bool own=address.Kind==Kind.Frame && InFrame(address.Offset,bytes);
                    Kill(address,bytes);
                    if(own && value.Kind!=Kind.Unknown) state.Memory[new(address.Offset,(int)bytes)]=value;
                    if(!own) { isPrivate=false; if(value.MayFrame) state.Escaped=true; RevokeStreams(); }
                }
                // All outputs read the same pre-commit register state, including
                // cached load snapshots taken before intervening stores.
                var outputs=block.Outputs.ToDictionary(p=>p.Key,p=>Eval(p.Value));
                foreach(var (slot,value) in outputs) {
                    if(value.Kind==Kind.Unknown) state.Registers.Remove(slot); else state.Registers[slot]=value;
                }
                if(block.Block.Kind==TerminatorKind.Call) {
                    ulong target=block.Block.Target;
                    var inputs=recovered.Functions.TryGetValue(target,out var callee) ? callee.Parameters : ApplicationImports.Inputs(program.Imports[target],FunctionAbi.Windows(program.Image));
                    Origin Argument(int index) => state.Registers.GetValueOrDefault(new(SlotKind.Local,index));
                    bool exposes=inputs.Where(s=>s!=FunctionRecovery.Stack).Any(s=>state.Registers.GetValueOrDefault(s).MayFrame);
                    bool noncapturing=false;
                    bool freshStream=false;
                    ulong? returnedCount=null;
                    if(callee!=null) {
                        noncapturing=privateMemory.Contains(target);
                        if(!noncapturing) { state.Memory.Clear(); isPrivate=false; RevokeStreams(); }
                    } else {
                        string symbol=program.Imports[target];
                        noncapturing=symbol is "_read" or "_write" or "_strcmp" or "_fopen" or "_malloc";
                        freshStream=symbol=="_fopen";
                        // Fresh-frame entry pointers and literal addresses
                        // cannot designate this new frame. fopen only reads
                        // those strings and captures neither pointer.
                        if(freshStream && !exposes && !state.Escaped) stdio=new(new FrameReadRange(0,0),Factory:true);
                        if(symbol=="_read") {
                            var count=Argument(2); var buffer=Argument(6);
                            ulong? bytes=count.Maximum;
                            Kill(buffer,bytes);
                            if(bytes!=0 && (buffer.Kind!=Kind.Frame || bytes is not { } size || !InFrame(buffer.Offset,size))) { isPrivate=false; RevokeStreams(); }
                        } else if((symbol is "_fread" or "_fwrite") && Argument(1).Kind==Kind.Stream) {
                            noncapturing=true;
                            returnedCount=Argument(2).Maximum;
                            ulong? bytes=TransferBytes(Argument(6),Argument(2)); var buffer=Argument(7);
                            bool own=bytes==0 || buffer.Kind==Kind.Frame && bytes is { } size && InFrame(buffer.Offset,size);
                            if(symbol=="_fread") {
                                Kill(buffer,bytes); stdio=new(new FrameReadRange(0,0));
                                if(!own) { isPrivate=false; RevokeStreams(); }
                            } else stdio=new(own ? new FrameReadRange(buffer.Offset,bytes!.Value) : null);
                        } else if((symbol is "_fgetc" or "_fclose" or "_fprintf") && Argument(7).Kind==Kind.Stream) {
                            noncapturing=true;
                            stdio=new(symbol=="_fprintf" ? null : new FrameReadRange(0,0));
                            if(symbol=="_fclose") RevokeStreams();
                        } else if(!noncapturing) {
                            // A FILE object can hide callbacks/aliases. Fresh
                            // unexposed frames are unreachable; exposure stays
                            // sticky even if this call has no frame argument.
                            if(state.Escaped || exposes) state.Memory.Clear();
                            isPrivate=false; RevokeStreams();
                        }
                    }
                    if(!noncapturing && exposes) state.Escaped=true;
                    var sp=state.Registers.GetValueOrDefault(FunctionRecovery.Stack);
                    IEnumerable<ValueSlot> writes=callee!=null ? callee.MayWrite.Except(preserved[target])
                        : FunctionAbi.Windows(program.Image) ? FunctionAbi.ImportWrites(program.Image)
                        : Enumerable.Range(0,16).Where(i=>i is not (3 or 4 or 5 or 12 or 13 or 14 or 15)).Select(i=>new ValueSlot(SlotKind.Local,i))
                            .Concat(state.Registers.Keys.Where(s=>s.Kind is SlotKind.Flag or SlotKind.DefinedFlag));
                    foreach(var slot in writes.ToArray()) state.Registers.Remove(slot);
                    state.Registers[FunctionRecovery.Stack]=sp.Kind==Kind.Frame ? Add(sp,(ulong)(8+(callee!=null ? frames[target].Cleanup : 0)),false) : default;
                    // Calls end blocks. New validity belongs only to this new
                    // result, so reopening cannot resurrect earlier aliases.
                    if(freshStream) state.Registers[FunctionRecovery.Accumulator]=new(Kind.Stream);
                    // fread/fwrite return completed elements, at most count.
                    if(returnedCount is { } max) state.Registers[FunctionRecovery.Accumulator]=new(Kind.Count,Bits:max);
                }
                return (state,isPrivate,stdio);
            }
        }
    }

    static ulong? TransferBytes(Origin size,Origin count) {
        if(size.Maximum is not { } a || count.Maximum is not { } b) return null;
        UInt128 bytes=(UInt128)a*b;
        return bytes<=16*1024*1024 ? (ulong)bytes : null;
    }
}
