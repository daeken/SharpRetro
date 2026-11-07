namespace SharpRetro.Decompiler;

public enum SsaDefinitionKind { Parameter, Constant, Value, Phi, CallResult, Uninitialized, FrameAddress }
public sealed class SsaDefinition(int id,SsaDefinitionKind kind,ulong block,ValueSlot slot,RecoveredValue? value=null,int width=0) {
    public int Id { get; }=id;
    public SsaDefinitionKind Kind { get; }=kind;
    public ulong Block { get; }=block;
    public ValueSlot Slot { get; }=slot;
    public RecoveredValue? Value { get; }=value;
    public int Width => width!=0 ? width : Value?.Width ?? Slot.Width;
    public Dictionary<ulong,SsaDefinition> Incoming { get; }=[];
    internal SsaDefinition? Alias;
    public long? FrameOffset { get; internal set; }
    public SsaDefinition Root {
        get {
            if(Alias==null) return this;
            return Alias=Alias.Root;
        }
    }
    internal int DisplayIndex;
    public string Name => $"variable_{DisplayIndex}";
}

/// Function-wide reaching definitions above the immutable block lowering. Phi
/// operands describe selected CFG edges; loads, checks and calls stay at their
/// original evaluation points. No expression is hoisted or speculated here.
public sealed class FunctionSsa {
    readonly RecoveredFunctions recovered;
    readonly RecoveredFunction function;
    readonly LiftedProgram program;
    readonly IReadOnlyDictionary<ulong,IReadOnlySet<ValueSlot>>? calleeParameters;
    readonly NativeFunctionMemory? frame;
    readonly NativePromotionPlan? promotion;
    readonly Dictionary<(ulong,ValueSlot),SsaDefinition> inputs=[];
    readonly Dictionary<(ulong,ValueSlot),SsaDefinition> outputs=[];
    readonly Dictionary<(ulong,ValueSlot),SsaDefinition> calls=[];
    readonly Dictionary<int,SsaDefinition> values=[];
    readonly Dictionary<ValueSlot,SsaDefinition> parameters=[];
    readonly List<SsaDefinition> definitions=[];
    readonly Dictionary<(ulong,(long Offset,int Width)),SsaDefinition> memoryInputs=[], memoryOutputs=[];
    readonly Dictionary<int,SsaDefinition> memoryLoads=[];
    readonly Dictionary<ValueEffect,SsaDefinition> memoryStores=new(ReferenceEqualityComparer.Instance);
    readonly HashSet<SsaDefinition> live=[];
    public IReadOnlyList<SsaDefinition> Storage => definitions.Where(d=>d.Root==d &&
        live.Contains(d) && d.Kind is SsaDefinitionKind.Value or SsaDefinitionKind.Phi or SsaDefinitionKind.CallResult).ToArray();
    public int PhiCount => Storage.Count(d=>d.Kind==SsaDefinitionKind.Phi);
    public bool NeedsFrame { get; private set; }
    public IReadOnlySet<ValueSlot> Parameters => parameters.Where(p=>live.Contains(p.Value.Root)).Select(p=>p.Key).ToHashSet();
    public IReadOnlySet<(long Offset,int Width)> Promoted => promotion?.Candidates ?? new HashSet<(long,int)>();
    public IReadOnlyList<SsaDefinition> AllDefinitions => definitions.Select(d=>d.Root).Distinct().ToArray();
    public SsaDefinition? Parameter(ValueSlot slot) => parameters.GetValueOrDefault(slot)?.Root;
    public SsaDefinition? Resolve(ulong block,RecoveredValue value) {
        if(memoryLoads.TryGetValue(value.Id,out var loaded)) return loaded.Root;
        if(value.Slot is { } slot) return inputs.GetValueOrDefault((block,slot))?.Root;
        return values.GetValueOrDefault(value.Id)?.Root;
    }

    FunctionSsa(LiftedProgram program,RecoveredFunctions recovered,RecoveredFunction function,NativeFunctionMemory? frame,NativePromotionPlan? promotion,
        IReadOnlyDictionary<ulong,IReadOnlySet<ValueSlot>>? calleeParameters) {
        this.program=program; this.recovered=recovered; this.function=function; this.frame=frame; this.promotion=promotion; this.calleeParameters=calleeParameters;
    }

    public static FunctionSsa? Recover(LiftedProgram program,RecoveredFunctions recovered,RecoveredFunction function,NativeFunctionMemory? frame=null,NativePromotionPlan? promotion=null,
        IReadOnlyDictionary<ulong,IReadOnlySet<ValueSlot>>? calleeParameters=null) {
        // Tail edges retain the existing explicit result contract until their
        // edge-specific call projections are represented by this overlay.
        if(function.Blocks.Any(a=>recovered.Blocks[a].Block.Kind!=TerminatorKind.Call &&
            recovered.Blocks[a].Block.FlowSuccessors.Any(t=>!function.Blocks.Contains(t)))) return null;
        var pass=new FunctionSsa(program,recovered,function,frame,promotion,calleeParameters);
        pass.Build(); return pass;
    }

    SsaDefinition New(SsaDefinitionKind kind,ulong block,ValueSlot slot,RecoveredValue? value=null,int width=0) {
        var definition=new SsaDefinition(definitions.Count,kind,block,slot,value,width);
        definitions.Add(definition); return definition;
    }

    void Build() {
        foreach(var slot in function.Parameters.Where(s=>s!=FunctionRecovery.Stack).OrderBy(s=>s.Kind).ThenBy(s=>s.Index))
            parameters[slot]=New(SsaDefinitionKind.Parameter,function.Entry,slot);
        foreach(var address in function.Blocks.Order()) {
            var block=recovered.Blocks[address];
            foreach(var slot in block.LiveIn.Where(s=>s!=FunctionRecovery.Stack).OrderBy(s=>s.Kind).ThenBy(s=>s.Index))
                inputs[(address,slot)]=New(SsaDefinitionKind.Phi,address,slot);
        }
        if(promotion!=null) {
            foreach(var address in function.Blocks.Order()) foreach(var cell in promotion.Candidates.OrderBy(c=>c.Offset).ThenBy(c=>c.Width))
                memoryInputs[(address,cell)]=New(SsaDefinitionKind.Phi,address,default,width:cell.Width);
            foreach(var address in function.Blocks.Order()) {
                var current=promotion.Candidates.ToDictionary(c=>c,c=>memoryInputs[(address,c)]);
                foreach(var effect in recovered.Blocks[address].Effects) {
                    if(frame?.RemovedEffects.Contains(effect)==true) continue;
                    if(effect.Kind=="evaluate" && promotion.Loads.TryGetValue(effect.Value.Id,out var loaded)) memoryLoads[effect.Value.Id]=current[loaded];
                    if(promotion.Stores.TryGetValue(effect,out var stored)) {
                        var value=Value(address,default,effect.Value);
                        memoryStores[effect]=value; current[stored]=value;
                    }
                }
                foreach(var (cell,value) in current) memoryOutputs[(address,cell)]=value;
            }
            foreach(var ((address,cell),phi) in memoryInputs) {
                if(address==function.Entry) phi.Incoming[ulong.MaxValue]=New(SsaDefinitionKind.Uninitialized,address,default,width:cell.Width);
                foreach(var predecessor in recovered.Graph.Predecessors[address].Where(p=>function.Blocks.Contains(p) && recovered.Blocks[p].Block.FlowSuccessors.Contains(address)))
                    phi.Incoming[predecessor]=memoryOutputs[(predecessor,cell)];
            }
        }
        foreach(var address in function.Blocks.Order()) {
            var block=recovered.Blocks[address];
            foreach(var slot in block.LiveOut.Where(s=>s!=FunctionRecovery.Stack).OrderBy(s=>s.Kind).ThenBy(s=>s.Index))
                outputs[(address,slot)]=Value(address,slot,block.Output(slot));
        }
        foreach(var ((address,slot),phi) in inputs) {
            // The pseudo predecessor represents the external function entry,
            // separate from real backedges into that same entry block.
            if(address==function.Entry) phi.Incoming[ulong.MaxValue]=parameters[slot];
            foreach(var predecessor in recovered.Graph.Predecessors[address].Where(p=>function.Blocks.Contains(p) && recovered.Blocks[p].Block.FlowSuccessors.Contains(address))) {
                var source=recovered.Blocks[predecessor];
                bool call=source.Block.Kind==TerminatorKind.Call && Writes(source.Block.Target).Contains(slot);
                if(call) {
                    if(!calls.TryGetValue((predecessor,slot),out var result))
                        calls[(predecessor,slot)]=result=New(SsaDefinitionKind.CallResult,predecessor,slot);
                    phi.Incoming[predecessor]=result;
                } else if(outputs.TryGetValue((predecessor,slot),out var output)) phi.Incoming[predecessor]=output;
                else throw new NotSupportedException("Missing function SSA reaching definition");
            }
        }
        bool changed;
        do {
            changed=false;
            foreach(var phi in definitions.Where(d=>d.Kind==SsaDefinitionKind.Phi)) {
                if(phi.Root!=phi) continue;
                var incoming=phi.Incoming.Values.Select(d=>d.Root).Where(d=>d!=phi).Distinct().ToArray();
                if(incoming.Length==1) { phi.Alias=incoming[0]; changed=true; }
            }
        } while(changed);
        foreach(var phi in definitions.Where(d=>d.Root==d && d.Kind==SsaDefinitionKind.Phi))
            if(phi.Incoming.Count==0) throw new NotSupportedException("Uninitialized function SSA join");
        Demand();
        int display=0;
        foreach(var definition in Storage) definition.DisplayIndex=display++;
    }

    IReadOnlySet<ValueSlot> Writes(ulong target) => recovered.Functions.TryGetValue(target,out var callee)
        ? callee.MayWrite : FunctionAbi.ImportWrites(recovered.Windows);

    SsaDefinition Value(ulong address,ValueSlot slot,RecoveredValue value) {
        if(memoryLoads.TryGetValue(value.Id,out var loaded)) return loaded;
        if(frame?.Offset(recovered.Blocks[address],value) is { } offset) {
            if(!values.TryGetValue(value.Id,out var pointer)) {
                pointer=New(SsaDefinitionKind.FrameAddress,address,slot,value);
                pointer.FrameOffset=offset; values[value.Id]=pointer;
            }
            return pointer;
        }
        if(value.Slot is { } input) return Input(address,input);
        if(values.TryGetValue(value.Id,out var old)) return old;
        return values[value.Id]=New(value.Constant!=null ? SsaDefinitionKind.Constant : SsaDefinitionKind.Value,address,slot,value);
    }

    public SsaDefinition Input(ulong block,ValueSlot slot) => inputs[(block,slot)].Root;
    public SsaDefinition Output(ulong block,ValueSlot slot) => outputs[(block,slot)].Root;
    public SsaDefinition CallResult(ulong block,ValueSlot slot) => calls[(block,slot)].Root;
    public SsaDefinition? Definition(RecoveredValue value) => values.TryGetValue(value.Id,out var definition) &&
        definition.Root.Kind==SsaDefinitionKind.Value && live.Contains(definition.Root) ? definition.Root : null;
    public SsaDefinition? Load(RecoveredValue value) => memoryLoads.TryGetValue(value.Id,out var loaded) ? loaded.Root : null;
    public bool IsPromotedStore(ValueEffect effect) => memoryStores.ContainsKey(effect);
    public bool StoreIsLive(ValueEffect effect) => memoryStores.TryGetValue(effect,out var stored) && live.Contains(stored.Root);
    public bool CallResultIsLive(ulong block,ValueSlot slot) => calls.TryGetValue((block,slot),out var value) && live.Contains(value.Root);
    public IReadOnlyList<(SsaDefinition Definition,RecoveredValue Value)> Definitions(ulong block) =>
        outputs.Where(p=>p.Key.Item1==block).Select(p=>(Definition:p.Value.Root,Value:recovered.Blocks[block].Output(p.Key.Item2)))
            .Where(p=>p.Definition.Kind==SsaDefinitionKind.Value && p.Definition.Block==block && live.Contains(p.Definition)).DistinctBy(p=>p.Definition.Id).ToArray();

    public IReadOnlyList<(SsaDefinition Phi,SsaDefinition Value)> Edge(ulong from,ulong to) =>
        inputs.Where(p=>p.Key.Item1==to).Select(p=>p.Value).Concat(memoryInputs.Where(p=>p.Key.Item1==to).Select(p=>p.Value))
            .Where(p=>p.Root==p && p.Kind==SsaDefinitionKind.Phi && live.Contains(p))
            .Select(p=>(Phi:p,Value:p.Incoming[from].Root)).Where(p=>p.Phi!=p.Value).ToArray();

    public IReadOnlyList<(SsaDefinition Phi,SsaDefinition Value)> Entry =>
        inputs.Where(p=>p.Key.Item1==function.Entry && p.Value.Root==p.Value && p.Value.Kind==SsaDefinitionKind.Phi && live.Contains(p.Value))
            .Select(p=>(Phi:p.Value,Value:p.Value.Incoming[ulong.MaxValue].Root)).ToArray();

    void Demand() {
        var seen=new HashSet<(ulong,int)>();
        void Use(ulong address,RecoveredValue value) {
            if(!seen.Add((address,value.Id))) return;
            if(memoryLoads.TryGetValue(value.Id,out var loaded)) Mark(loaded);
            else if(frame?.Offset(recovered.Blocks[address],value)!=null) NeedsFrame=true;
            else if(value.Slot is { } slot) { if(slot!=FunctionRecovery.Stack) Mark(Input(address,slot)); }
            else foreach(var child in value.Inputs) Use(address,child);
        }
        void Mark(SsaDefinition value) {
            value=value.Root;
            if(!live.Add(value)) return;
            if(value.Kind==SsaDefinitionKind.Uninitialized) throw new NotSupportedException("Promoted scalar has an uninitialized reaching definition");
            if(value.Kind==SsaDefinitionKind.FrameAddress) NeedsFrame=true;
            if(value.Kind==SsaDefinitionKind.Phi) foreach(var incoming in value.Incoming.Values) Mark(incoming);
            else if(value.Kind==SsaDefinitionKind.Value) Use(value.Block,value.Value!);
        }
        foreach(var address in function.Blocks) {
            var block=recovered.Blocks[address];
            var uses=new Dictionary<int,int>(); var counted=new HashSet<int>();
            void Count(RecoveredValue value) {
                uses[value.Id]=uses.GetValueOrDefault(value.Id)+1;
                if(counted.Add(value.Id)) foreach(var child in value.Inputs) Count(child);
            }
            foreach(var effect in block.Effects) { Count(effect.Value); if(effect.Address!=null) Count(effect.Address); }
            if(block.Condition!=null) Count(block.Condition);
            foreach(var slot in block.LiveOut) Count(block.Output(slot));
            foreach(var effect in block.Effects) {
                if(frame?.RemovedEffects.Contains(effect)==true || IsPromotedStore(effect)) continue;
                if(effect.Kind=="evaluate" && effect.Value.Operation is LiftIl.IlLoad && frame!=null &&
                   (memoryLoads.ContainsKey(effect.Value.Id) || uses.GetValueOrDefault(effect.Value.Id)==1 &&
                    frame.Offset(block,effect.Value.Inputs[0]) is { } offset && frame.Scalars.ContainsKey((offset,effect.Value.Width)))) continue;
                Use(address,effect.Value); if(effect.Address!=null) Use(address,effect.Address);
            }
            if(block.Condition!=null) Use(address,block.Condition);
            if(block.Block.Kind==TerminatorKind.Return) foreach(var slot in function.Results) Mark(Output(address,slot));
            if(block.Block.Kind==TerminatorKind.Call) {
                var arguments=recovered.Functions.TryGetValue(block.Block.Target,out var callee) ? callee.Parameters
                    : ApplicationImports.Inputs(program.Imports[block.Block.Target],recovered.Windows);
                if(calleeParameters?.TryGetValue(block.Block.Target,out var effective)==true) arguments=effective;
                foreach(var slot in arguments.Where(s=>s!=FunctionRecovery.Stack)) Mark(Output(address,slot));
            }
        }
    }
}
