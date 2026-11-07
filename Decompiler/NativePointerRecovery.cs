using LiftIl;

namespace SharpRetro.Decompiler;

/// Address roles describe observed uses of machine bits, never object bounds or
/// allocation identity. Arithmetic and comparisons must still use uintptr bits.
public sealed record NativePointerRole(bool IsReadOnly,int Width,bool Escapes);
public sealed record NativePointerAddress(SsaDefinition Base,long? Offset,RecoveredValue Value);
public sealed record NativePointerAccess(ulong Block,RecoveredValue Address,SsaDefinition Base,long? Offset,int Width,bool Write);
public sealed class NativePointerPlan {
    public IReadOnlyDictionary<SsaDefinition,NativePointerRole> Definitions { get; }
    public IReadOnlyDictionary<ValueSlot,NativePointerRole> Parameters { get; }
    public IReadOnlyDictionary<ValueSlot,NativePointerRole> Results { get; }
    public IReadOnlyList<NativePointerAccess> Accesses { get; }
    readonly IReadOnlyDictionary<(ulong,int),NativePointerAddress> addresses;
    readonly IReadOnlyDictionary<(ulong,int),NativePointerRole> roles;
    internal NativePointerPlan(Dictionary<SsaDefinition,NativePointerRole> definitions,Dictionary<ValueSlot,NativePointerRole> parameters,
        Dictionary<ValueSlot,NativePointerRole> results,List<NativePointerAccess> accesses,Dictionary<(ulong,int),NativePointerAddress> addresses,Dictionary<(ulong,int),NativePointerRole> roles) {
        Definitions=definitions; Parameters=parameters; Results=results; Accesses=accesses; this.addresses=addresses; this.roles=roles;
    }
    public bool Pointer(SsaDefinition definition) => Definitions.ContainsKey(definition.Root);
    public bool IsReadOnly(SsaDefinition definition) => Definitions.TryGetValue(definition.Root,out var role) && role.IsReadOnly;
    public NativePointerAddress? Address(ulong block,RecoveredValue value) => addresses.GetValueOrDefault((block,value.Id));
    public NativePointerRole? Role(ulong block,RecoveredValue value) => roles.GetValueOrDefault((block,value.Id));
}

/// Conservative native pointer-role overlay above immutable SSA. All recovered
/// loads remain snapshots. Const is a restriction on this access path, not an
/// assertion that the pointed-to bytes cannot change through another alias.
public static class NativePointerRecovery {
    sealed class Node(ulong function,ulong block,int width,SsaDefinition? definition=null,RecoveredValue? value=null) {
        public readonly ulong Function=function, Block=block;
        public readonly int Width=width;
        public readonly SsaDefinition? Definition=definition;
        public RecoveredValue? Value=value;
        public readonly List<Node> Inputs=[];
        public bool Seed, Pointer, Write, Escapes;
        public bool? ScalarFact;
        public readonly HashSet<int> Widths=[];
        public readonly HashSet<Node> Aliases=[];
    }
    sealed record Access(ulong Block,RecoveredValue Address,Node Node,int Width,bool Write);
    sealed record Call(ulong Function,ValueBlock Block,Dictionary<ValueSlot,Node> Arguments,string? Import);

    public static IReadOnlyDictionary<ulong,NativePointerPlan> Analyze(LiftedProgram program,RecoveredFunctions recovered,
        IReadOnlyDictionary<ulong,FunctionSsa> functions,IReadOnlyDictionary<ulong,NativeFunctionMemory>? frames=null) {
        var nodes=new List<Node>();
        var definitions=new Dictionary<SsaDefinition,Node>();
        var expressions=new Dictionary<(ulong,ulong,int),Node>();
        var originals=new Dictionary<(ulong,ulong,int),RecoveredValue>();
        var accesses=new Dictionary<ulong,List<Access>>();
        var calls=new List<Call>();
        foreach(var (entry,ssa) in functions) foreach(var d in ssa.AllDefinitions) {
            var n=new Node(entry,d.Block,d.Width,d,d.Value); definitions[d]=n; nodes.Add(n);
        }
        Node Expression(ulong entry,ulong block,RecoveredValue value) {
            var key=(entry,block,value.Id);
            originals[key]=value;
            if(expressions.TryGetValue(key,out var old)) return old;
            var resolved=functions[entry].Resolve(block,value);
            if(resolved!=null && definitions.TryGetValue(resolved.Root,out var definition)) {
                expressions[key]=definition; return definition;
            }
            var n=new Node(entry,block,value.Width,value:value); nodes.Add(n); expressions[key]=n;
            foreach(var child in value.Inputs) n.Inputs.Add(Expression(entry,block,child));
            return n;
        }
        foreach(var (d,n) in definitions) {
            if(d.Kind==SsaDefinitionKind.Phi) foreach(var incoming in d.Incoming.Values) n.Inputs.Add(definitions[incoming.Root]);
            else if(d.Value is { } v) foreach(var child in v.Inputs) n.Inputs.Add(Expression(n.Function,n.Block,child));
            n.Seed=d.Width==64 && (d.Kind==SsaDefinitionKind.FrameAddress || d.Kind==SsaDefinitionKind.Constant && d.Value?.PcRelative==true);
        }
        foreach(var (entry,ssa) in functions) {
            accesses[entry]=[];
            var seen=new HashSet<(ulong,int)>();
            void Visit(ulong address,RecoveredValue v) {
                if(!seen.Add((address,v.Id))) return;
                var n=Expression(entry,address,v);
                if(v.Operation is IlLoad && ssa.Load(v)==null && v.Inputs.Count==1)
                    accesses[entry].Add(new(address,v.Inputs[0],Expression(entry,address,v.Inputs[0]),v.Width,false));
                foreach(var input in v.Inputs) Visit(address,input);
            }
            foreach(var address in recovered.Functions[entry].Blocks) {
                var block=recovered.Blocks[address];
                foreach(var effect in block.Effects) {
                    if(frames?.GetValueOrDefault(entry)?.RemovedEffects.Contains(effect)==true) continue;
                    Visit(address,effect.Value);
                    if(effect.Address is { } target) {
                        Visit(address,target);
                        if(effect.Kind=="store" && !ssa.IsPromotedStore(effect)) accesses[entry].Add(new(address,target,Expression(entry,address,target),effect.Value.Width,true));
                    }
                }
                if(block.Condition is { } condition) Visit(address,condition);
                foreach(var output in block.Outputs.Values) Visit(address,output);
                if(block.Block.Kind!=TerminatorKind.Call) continue;
                string? import=program.Imports.GetValueOrDefault(block.Block.Target);
                IEnumerable<ValueSlot> slots=recovered.Functions.TryGetValue(block.Block.Target,out var callee) ? callee.Parameters
                    : import!=null ? ApplicationImports.Inputs(import,recovered.Windows) : block.LiveOut;
                var args=new Dictionary<ValueSlot,Node>();
                foreach(var slot in slots.Where(s=>s!=FunctionRecovery.Stack))
                    if(block.Outputs.ContainsKey(slot)||block.Inputs.ContainsKey(slot)) args[slot]=Expression(entry,address,block.Output(slot));
                calls.Add(new(entry,block,args,import));
            }
        }
        // Numeric induction families are distinct from address families.
        // A full-width unknown parameter/load is never a scalar-offset anchor.
        // Constants and narrow numerical values anchor closed phi/add cycles.
        {
            var dependencies=new Dictionary<Node,List<Node>>();
            var reverse=nodes.ToDictionary(n=>n,_=>new List<Node>());
            var anchors=new HashSet<Node>();
            foreach(var n in nodes) {
                List<Node>? inputs=null;
                var v=n.Value;
                if(n.Width<64 || v?.Constant!=null && !v.PcRelative || v?.Operation is IlCast { Kind:CastKind.Zext or CastKind.Sext } && n.Inputs.Count==1 && n.Inputs[0].Width<64) {
                    inputs=[]; anchors.Add(n);
                } else if(n.Definition?.Kind==SsaDefinitionKind.Phi) inputs=n.Inputs.ToList();
                else if(v?.Operation is IlIfV) inputs=n.Inputs.Skip(1).ToList();
                else if(v?.Operation is IlBin { Op:BinOp.Add or BinOp.Sub or BinOp.Mul or BinOp.Shl or BinOp.Shr or BinOp.And or BinOp.Or or BinOp.Xor }) inputs=n.Inputs.ToList();
                if(inputs==null) continue;
                dependencies[n]=inputs;
                foreach(var input in inputs) reverse[input].Add(n);
            }
            var eligible=dependencies.Keys.ToHashSet();
            var queue=new Queue<Node>(nodes.Where(n=>!eligible.Contains(n)));
            while(queue.TryDequeue(out var invalid)) foreach(var parent in reverse[invalid])
                if(eligible.Remove(parent)) queue.Enqueue(parent);
            anchors.IntersectWith(eligible); queue=new(anchors);
            while(queue.TryDequeue(out var anchor)) foreach(var parent in reverse[anchor])
                if(eligible.Contains(parent) && anchors.Add(parent)) queue.Enqueue(parent);
            foreach(var n in nodes) n.ScalarFact=anchors.Contains(n);
        }
        bool changed;
        bool Family(Node n) {
            var roots=new HashSet<Node>(); var seen=new HashSet<Node>();
            bool Visit(Node value) {
                if(value.Width!=64) return false;
                if(Null(value)) return true;
                if(!seen.Add(value)) return true;
                if(value.Pointer || value.Seed || value.Definition?.Kind is SsaDefinitionKind.Parameter or SsaDefinitionKind.CallResult or SsaDefinitionKind.FrameAddress || value.Value?.Operation is IlLoad || value.Value?.PcRelative==true) {
                    roots.Add(value); return roots.Count<=1;
                }
                if(value.Definition?.Kind==SsaDefinitionKind.Phi) return value.Inputs.All(Visit);
                if(value.Value?.Operation is IlIfV) return value.Inputs.Skip(1).All(Visit);
                if(value.Value?.Operation is IlBin { Op:BinOp.Add or BinOp.Sub } bin && value.Inputs.Count==2) {
                    if(Scalar(value.Inputs[1])) return Visit(value.Inputs[0]);
                    if(bin.Op==BinOp.Add && Scalar(value.Inputs[0])) return Visit(value.Inputs[1]);
                }
                if(value.Value?.Operation is IlCast { Kind:CastKind.Bitcast } && value.Inputs.Count==1 && value.Inputs[0].Width==64) return Visit(value.Inputs[0]);
                return false;
            }
            if(!Visit(n) || roots.Count!=1) return false;
            var root=roots.Single();
            if(root.Seed || root.Pointer) return false;
            root.Seed=true; return true;
        }
        bool Seed(Node n) {
            if(n.Width!=64 || n.Seed) return false;
            if(n.Definition?.Kind is SsaDefinitionKind.Parameter or SsaDefinitionKind.CallResult or SsaDefinitionKind.FrameAddress || n.Value?.Operation is IlLoad) {
                n.Seed=true; return true;
            }
            if(n.Value?.PcRelative==true) { n.Seed=true; return true; }
            if(n.Definition?.Kind==SsaDefinitionKind.Phi || n.Value?.Operation is IlIfV) return Family(n);
            if(n.Value?.Operation is IlBin { Op:BinOp.Add or BinOp.Sub } bin && n.Inputs.Count==2) {
                if(Scalar(n.Inputs[1])) return Seed(n.Inputs[0]);
                if(bin.Op==BinOp.Add && Scalar(n.Inputs[0])) return Seed(n.Inputs[1]);
            }
            if(n.Value?.Operation is IlCast { Kind:CastKind.Bitcast } && n.Inputs.Count==1 && n.Inputs[0].Width==64) return Seed(n.Inputs[0]);
            return false;
        }
        // Solve eligible pointer families as a graph, then propagate real
        // anchors. This admits anchored induction cycles without recursively
        // expanding shared diamond DAGs or accepting an unanchored phi cycle.
        void Infer() {
            var dependencies=new Dictionary<Node,List<Node>>();
            var reverse=nodes.ToDictionary(n=>n,_=>new List<Node>());
            foreach(var n in nodes) {
                List<Node>? inputs=null;
                if(n.Width==64) {
                    if(n.Seed) inputs=[];
                    else if(n.Definition?.Kind==SsaDefinitionKind.Phi) inputs=n.Inputs.Where(i=>!Null(i)).ToList();
                    else if(n.Value?.Operation is IlIfV) inputs=n.Inputs.Skip(1).Where(i=>!Null(i)).ToList();
                    else if(n.Value?.Operation is IlBin { Op:BinOp.Add or BinOp.Sub } bin && n.Inputs.Count==2) {
                        if(Scalar(n.Inputs[1])) inputs=[n.Inputs[0]];
                        else if(bin.Op==BinOp.Add && Scalar(n.Inputs[0])) inputs=[n.Inputs[1]];
                    } else if(n.Value?.Operation is IlCast { Kind:CastKind.Bitcast } && n.Inputs.Count==1 && n.Inputs[0].Width==64) inputs=[n.Inputs[0]];
                }
                if(inputs==null) continue;
                dependencies[n]=inputs;
                foreach(var input in inputs) reverse[input].Add(n);
            }
            var eligible=dependencies.Keys.ToHashSet();
            var queue=new Queue<Node>(nodes.Where(n=>!eligible.Contains(n)));
            while(queue.TryDequeue(out var invalid)) foreach(var parent in reverse[invalid])
                if(eligible.Remove(parent)) queue.Enqueue(parent);
            var anchored=eligible.Where(n=>n.Seed).ToHashSet();
            queue=new(anchored);
            while(queue.TryDequeue(out var anchor)) foreach(var parent in reverse[anchor])
                if(eligible.Contains(parent) && anchored.Add(parent)) queue.Enqueue(parent);
            foreach(var n in anchored) if(!n.Pointer) { n.Pointer=true; changed=true; }
        }
        do {
            changed=false;
            foreach(var access in accesses.Values.SelectMany(a=>a)) changed|=Seed(access.Node);
            foreach(var call in calls) {
                foreach(var (slot,arg) in call.Arguments) {
                    if(ImportRole(call.Import,recovered.Windows,slot) is { } role && role.Pointer) changed|=Seed(arg);
                    if(functions.TryGetValue(call.Block.Block.Target,out var target) && target.Parameter(slot) is { } parameter) {
                        var formal=definitions[parameter];
                        if(formal.Pointer) changed|=Seed(arg);
                        if(arg.Pointer && !formal.Seed) { formal.Seed=true; changed=true; }
                    }
                }
                if(ImportReturnsPointer(call.Import)) foreach(var result in definitions.Values.Where(n=>n.Function==call.Function && n.Block==call.Block.Block.Address && n.Definition?.Kind==SsaDefinitionKind.CallResult && n.Definition.Slot==FunctionRecovery.Accumulator))
                    if(!result.Seed) { result.Seed=true; changed=true; }
                if(functions.ContainsKey(call.Block.Block.Target)) foreach(var result in definitions.Values.Where(n=>n.Function==call.Function && n.Block==call.Block.Block.Address && n.Definition?.Kind==SsaDefinitionKind.CallResult)) {
                    var returns=Returns(call.Block.Block.Target,result.Definition!.Slot);
                    if(returns.Count!=0 && returns.All(n=>n.Pointer||Null(n)) && returns.Any(n=>n.Pointer) && !result.Seed) { result.Seed=true; changed=true; }
                }
            }
            Infer();
        } while(changed);

        List<Node> Returns(ulong entry,ValueSlot slot) {
            if(!recovered.Functions[entry].Results.Contains(slot)) return [];
            return recovered.Functions[entry].Blocks.Where(b=>recovered.Blocks[b].Block.Kind==TerminatorKind.Return)
                .Select(b=>definitions[functions[entry].Output(b,slot)]).ToList();
        }
        void Alias(Node a,Node b) { if(a.Pointer && b.Pointer) { a.Aliases.Add(b); b.Aliases.Add(a); } }
        foreach(var n in nodes.Where(n=>n.Pointer)) {
            if(n.Definition?.Kind==SsaDefinitionKind.Phi) foreach(var input in n.Inputs) Alias(n,input);
            else if(n.Value?.Operation is IlIfV) foreach(var input in n.Inputs.Skip(1)) Alias(n,input);
            else if(n.Value?.Operation is IlBin { Op:BinOp.Add or BinOp.Sub } or IlCast { Kind:CastKind.Bitcast }) foreach(var input in n.Inputs) Alias(n,input);
        }
        // Numeric transformations are deliberately not pointer propagation, but
        // storing/passing their bits can still expose the original pointer.
        void Expose(Node n,HashSet<Node> seen) {
            if(!seen.Add(n)) return;
            if(n.Pointer) { n.Write=true; n.Escapes=true; }
            // Loaded contents and their pointee are independent of the
            // containing object address. Promoted loads already resolve the
            // original stored SSA definition instead of an IlLoad node.
            if(n.Value?.Operation is IlLoad) return;
            foreach(var child in n.Inputs) Expose(child,seen);
        }
        void ReadOrigins(Node n,int width,HashSet<Node> seen) {
            if(!seen.Add(n)) return;
            if(n.Pointer) n.Widths.Add(width);
            if(n.Value?.Operation is IlLoad) return;
            foreach(var child in n.Inputs) ReadOrigins(child,width,seen);
        }
        foreach(var (entry,list) in accesses) foreach(var access in list) {
            if(access.Node.Pointer) { access.Node.Write|=access.Write; access.Node.Widths.Add(access.Width); }
            else if(access.Write) Expose(access.Node,[]);
            else ReadOrigins(access.Node,access.Width,[]);
        }
        foreach(var (entry,ssa) in functions) foreach(var block in recovered.Functions[entry].Blocks.Select(b=>recovered.Blocks[b]))
            foreach(var effect in block.Effects.Where(e=>e.Kind=="store" && !ssa.IsPromotedStore(e))) Expose(Expression(entry,block.Block.Address,effect.Value),[]);
        foreach(var call in calls) {
            foreach(var (slot,arg) in call.Arguments) {
                if(functions.TryGetValue(call.Block.Block.Target,out var target) && target.Parameter(slot) is { } parameter) {
                    var formal=definitions[parameter]; Alias(arg,formal);
                    if(formal.Pointer && !arg.Pointer) Expose(arg,[]);
                }
                else if(ImportRole(call.Import,recovered.Windows,slot) is { } role) { if(role.Write) { if(arg.Pointer) { arg.Write=true; if(role.Escape) arg.Escapes=true; } else Expose(arg,[]); } }
                else Expose(arg,[]);
            }
            foreach(var result in definitions.Values.Where(n=>n.Function==call.Function && n.Block==call.Block.Block.Address && n.Definition?.Kind==SsaDefinitionKind.CallResult)) {
                if(functions.ContainsKey(call.Block.Block.Target)) foreach(var returned in Returns(call.Block.Block.Target,result.Definition!.Slot)) {
                    Alias(result,returned);
                    if(result.Pointer && !returned.Pointer) Expose(returned,[]);
                }
                if(call.Import is "memcpy" or "memmove" or "memset" && call.Arguments.TryGetValue(new(SlotKind.Local,1),out var destination)) Alias(result,destination);
            }
        }
        do {
            changed=false;
            foreach(var n in nodes.Where(n=>n.Pointer)) foreach(var alias in n.Aliases) {
                if(n.Write && !alias.Write) { alias.Write=true; changed=true; }
                if(n.Escapes && !alias.Escapes) { alias.Escapes=true; changed=true; }
                int count=alias.Widths.Count; alias.Widths.UnionWith(n.Widths); changed|=count!=alias.Widths.Count;
            }
        } while(changed);
        NativePointerRole Role(Node n) => new(!n.Write,n.Widths.Count==1?n.Widths.Single():0,n.Escapes);
        NativePointerAddress? Address(Node n,RecoveredValue original,HashSet<Node> seen) {
            if(!n.Pointer || !seen.Add(n)) return null;
            if(n.Value?.Operation is IlBin { Op:BinOp.Add or BinOp.Sub } bin && n.Inputs.Count==2) {
                Node? baseNode=null,delta=null; bool subtract=false;
                if(n.Inputs[0].Pointer && Scalar(n.Inputs[1])) { baseNode=n.Inputs[0]; delta=n.Inputs[1]; subtract=bin.Op==BinOp.Sub; }
                else if(bin.Op==BinOp.Add && n.Inputs[1].Pointer && Scalar(n.Inputs[0])) { baseNode=n.Inputs[1]; delta=n.Inputs[0]; }
                if(baseNode!=null) {
                    var prior=Address(baseNode,original,seen);
                    if(prior!=null) {
                        long? offset=null;
                        if(prior.Offset is { } before && delta!.Value?.Constant is { } constant) {
                            long bits=unchecked((long)(ulong)constant);
                            try { offset=subtract ? checked(before-bits) : checked(before+bits); } catch(OverflowException) { }
                        }
                        return prior with { Offset=offset };
                    }
                }
            }
            if(n.Value?.Operation is IlCast { Kind:CastKind.Bitcast } && n.Inputs.Count==1) return Address(n.Inputs[0],original,seen);
            return n.Definition is { } d ? new(d,0,original) : null;
        }
        var plans=new Dictionary<ulong,NativePointerPlan>();
        foreach(var (entry,ssa) in functions) {
            var defs=definitions.Where(p=>p.Value.Function==entry && p.Value.Pointer).ToDictionary(p=>p.Key,p=>Role(p.Value));
            var parameters=ssa.Parameters.Where(s=>ssa.Parameter(s) is { } p && defs.ContainsKey(p)).ToDictionary(s=>s,s=>defs[ssa.Parameter(s)!]);
            var results=new Dictionary<ValueSlot,NativePointerRole>();
            foreach(var slot in recovered.Functions[entry].Results) {
                var returned=Returns(entry,slot);
                if(returned.Count!=0 && returned.All(n=>n.Pointer||Null(n)) && returned.Any(n=>n.Pointer)) {
                    var pointers=returned.Where(n=>n.Pointer).ToArray();
                    results[slot]=new(pointers.All(n=>!n.Write),0,pointers.Any(n=>n.Escapes));
                }
            }
            var normalized=new Dictionary<(ulong,int),NativePointerAddress>();
            foreach(var (key,n) in expressions.Where(p=>p.Key.Item1==entry)) if(Address(n,originals[key],[]) is { } a) normalized[(key.Item2,key.Item3)]=a;
            var observed=new List<NativePointerAccess>();
            foreach(var access in accesses[entry]) if(Address(access.Node,access.Address,[]) is { } a) {
                normalized[(access.Block,access.Address.Id)]=a;
                observed.Add(new(access.Block,access.Address,a.Base,a.Offset,access.Width,access.Write));
            }
            var roles=expressions.Where(p=>p.Key.Item1==entry && p.Value.Pointer).ToDictionary(p=>(p.Key.Item2,p.Key.Item3),p=>Role(p.Value));
            plans[entry]=new(defs,parameters,results,observed,normalized,roles);
        }
        return plans;
    }

    static bool Null(Node n) => n.Width==64 && n.Value?.Constant==UInt128.Zero && n.Value.PcRelative==false;
    static bool Scalar(Node n) => n.ScalarFact==true;
    static bool ImportReturnsPointer(string? name) => name is "memcpy" or "memmove" or "memset" or "_malloc" or "_fopen";
    readonly record struct ImportPointer(bool Pointer,bool Write,bool Escape=false);
    static ImportPointer? ImportRole(string? name,bool windows,ValueSlot slot) {
        if(slot.Kind!=SlotKind.Local) return null;
        var regs=windows ? new[]{1,2,8,9} : new[]{7,6,2,1,8,9};
        int arg=Array.IndexOf(regs,slot.Index);
        if(arg<0) return null;
        return name switch {
            "memcpy" or "memmove" when arg==0=>new(true,true),
            "memcpy" or "memmove" when arg==1=>new(true,false),
            "memset" when arg==0=>new(true,true),
            "_read" when arg==1=>new(true,true),
            "_write" when arg==1=>new(true,false),
            "_strcmp" or "_fopen" when arg<2=>new(true,false),
            "_free" or "_fclose" or "_fgetc" when arg==0=>new(true,true,true),
            "_fread" when arg==0 || arg==3=>new(true,true),
            "_fwrite" when arg==0=>new(true,false),
            "_fwrite" when arg==3=>new(true,true),
            "_perror" or "_printf" when arg==0=>new(true,false),
            "_fprintf" when arg==0=>new(true,true),
            "_fprintf" when arg==1=>new(true,false),
            _=>null
        };
    }
}
