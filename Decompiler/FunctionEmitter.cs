namespace SharpRetro.Decompiler;

/// Explicit C++ function contracts and native calls. No shared register context:
/// live-in values are parameters and demanded modified values are return values.
public sealed class FunctionEmitter : SourceEmitter {
    RecoveredFunctions recovered = null!;
    LiftedProgram program = null!;
    Dictionary<ulong,NativeFunctionMemory>? native;
    NativeFunctionMemory? frame;
    ValueBlock? activeBlock;
    int callIndex, resultIndex, returnIndex;
    List<CppStatement>? statements;
    RecoveredFunction? currentFunction;
    FunctionSsa? ssa;
    Dictionary<ulong,FunctionSsa?> ssaPlans=[];
    IReadOnlyDictionary<ulong,NativePointerPlan> pointerPlans=new Dictionary<ulong,NativePointerPlan>();
    NativePointerPlan? pointers;
    IReadOnlyDictionary<ulong,NativeFrameArrayPlan> frameArrayPlans=new Dictionary<ulong,NativeFrameArrayPlan>();
    NativeFrameArrayPlan? frameArrays;
    IReadOnlyDictionary<ulong,NativeObjectViews> objectPlans=new Dictionary<ulong,NativeObjectViews>();
    NativeObjectViews? objects;
    int transferIndex;
    int[] ArgumentRegisters = [];
    public override string Emit(LiftedProgram program) => Emit(program, ControlFlowStyle.Auto);
    public string Emit(LiftedProgram program, ControlFlowStyle style, string memory = "auto",bool functionOnly=false) {
        this.program = program; ArgumentRegisters=FunctionAbi.Arguments(program.Image);
        VectorCopyValidation.ValidateKinds(program);
        if(functionOnly) PeFunction.ValidateImports(program);
        recovered = FunctionRecovery.Recover(program);
        data=functionOnly ? PeFunction.Validate(program,recovered) : MachOApplication.Validate(program,libraryImports:true);
        if(functionOnly && memory=="checked") throw new NotSupportedException("PE function profile currently requires native memory");
        if(memory=="checked" && VectorCopyValidation.HasAlignedMoves(program)) throw new NotSupportedException("Checked backend does not model aligned XMM faults");
        native=null;
        string memorySummary="checked address layer";
        if(memory!="checked") {
            try {
                native=NativeMemoryRecovery.Recover(recovered,program.Entry);
                VectorCopyValidation.Validate(program,recovered,native);
                var proofs=FrameProofs.Analyze(program,recovered,native);
                var optimized=FunctionRecovery.Recover(program,proofs.Preserved);
                // New lowering creates new effect objects; native bookkeeping
                // removal is keyed by reference and must be rebuilt as well.
                var optimizedNative=NativeMemoryRecovery.Recover(optimized,program.Entry);
                var dead=new HashSet<NativeEffectPoint>();
                while(true) {
                    var next=NativeDeadEffects.Find(program,optimized,optimizedNative,dead,proofs.StdioCalls);
                    if(next.Count==0) break;
                    dead.UnionWith(next);
                    optimized=FunctionRecovery.Recover(program,proofs.Preserved,dead);
                    optimizedNative=NativeMemoryRecovery.Recover(optimized,program.Entry);
                }
                recovered=optimized; native=optimizedNative;
                memorySummary=$"native objects, {native.Values.Sum(p=>p.Scalars.Count)} typed stack fields analyzed";
                memorySummary+=$"; {proofs.Preserved.Values.Sum(s=>s.Count)} proven register identities";
                memorySummary+=$"; {dead.Count} private dead effects removed";
                memorySummary+=$"; {proofs.StdioCalls.Count} provenance-checked stdio calls";
            } catch(Exception e) when(e is NotSupportedException or OverflowException) {
                native=null;
                if(memory=="native" || functionOnly || VectorCopyValidation.HasAlignedMoves(program)) throw new NotSupportedException("Cannot reconstruct native memory: "+e.Message,e);
                memorySummary="checked memory fallback: "+e.Message;
            }
        }
        ssaPlans=[];
        pointerPlans=new Dictionary<ulong,NativePointerPlan>();
        frameArrayPlans=new Dictionary<ulong,NativeFrameArrayPlan>();
        objectPlans=new Dictionary<ulong,NativeObjectViews>();
        if(native!=null) {
            var promotions=NativeScalarPromotion.Analyze(program,recovered,native);
            IReadOnlyDictionary<ulong,IReadOnlySet<ValueSlot>> parameters=recovered.Functions.ToDictionary(p=>p.Key,p=>(IReadOnlySet<ValueSlot>)p.Value.Parameters);
            // Contracts only shrink. Each changing round removes at least one
            // parameter, including through recursive call-graph components.
            int rounds=parameters.Values.Sum(p=>p.Count)+1;
            for(int iteration=0;iteration<rounds;iteration++) {
                ssaPlans=recovered.Functions.ToDictionary(p=>p.Key,p=>FunctionSsa.Recover(program,recovered,p.Value,native[p.Key],promotions[p.Key],parameters));
                var next=ssaPlans.ToDictionary(p=>p.Key,p=>(IReadOnlySet<ValueSlot>)(p.Value?.Parameters ?? recovered.Functions[p.Key].Parameters));
                if(next.All(p=>p.Value.SetEquals(parameters[p.Key]))) break;
                parameters=next;
            }
            memorySummary+=$"; {ssaPlans.Values.Sum(p=>p?.Promoted.Count ?? 0)} private stack cells promoted";
            memorySummary+=$"; {ssaPlans.Values.Count(p=>p!=null)} function SSA graphs";
            pointerPlans=NativePointerRecovery.Analyze(program,recovered,
                ssaPlans.Where(p=>p.Value!=null).ToDictionary(p=>p.Key,p=>p.Value!),native);
            frameArrayPlans=NativeFrameArrays.Analyze(recovered,native,ssaPlans.Where(p=>p.Value!=null).ToDictionary(p=>p.Key,p=>p.Value!));
            objectPlans=pointerPlans.ToDictionary(p=>p.Key,p=>NativeObjectViews.Analyze(p.Key,p.Value));
            memorySummary+=$"; {pointerPlans.Values.Sum(p=>p.Definitions.Count)} pointer roles";
        }
        var regions = new Dictionary<ulong, StructuredProgram>();
        var fallback = new Dictionary<ulong, string>();
        foreach(var function in recovered.Functions.Values) {
            if(style == ControlFlowStyle.Dispatch) { fallback[function.Entry] = "basic blocks requested"; continue; }
            var blocks = function.Blocks.ToDictionary(a=>a,a=>recovered.Graph.Blocks[a]);
            var predecessors = blocks.Keys.ToDictionary(a=>a,_=>new HashSet<ulong>());
            foreach(var block in blocks.Values)
                foreach(var target in block.FlowSuccessors)
                    if(predecessors.TryGetValue(target,out var set)) set.Add(block.Address);
            var graph = new BlockGraph(function.Entry,blocks,predecessors);
            try {
                StructuredProgram structured;
                try { structured = Structuring.Recover(graph,allowMultipleLoopExits:true); }
                catch(StructuringException) {
                    structured = Structuring.Recover(graph,allowMultipleLoopExits:true,allowBlockDuplication:true);
                }
                regions.Add(function.Entry,structured);
            }
            catch(StructuringException e) {
                if(style == ControlFlowStyle.Structured) throw new StructuringException($"Cannot structure {function.Name}: {e.Message}");
                fallback[function.Entry] = e.Message;
            }
        }
        output.Clear(); indent = 0;
        Line(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"IntegerRuntime.hpp")).Replace("#pragma once\n",""));
        Line(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,native==null ? "NativeRuntime.hpp" : "NativeObjectsRuntime.hpp")));
        bool include=true;
        foreach(string line in File.ReadLines(Path.Combine(AppContext.BaseDirectory,"HostImportsRuntime.hpp"))) {
            if(line=="#ifdef SHARP_RETRO_CHECKED_MEMORY") include=native==null;
            else if(line=="#endif") include=true;
            else if(include) Line(line);
        }
        Line("namespace sharp_retro::source {");
        Line($"// Input SHA-256: {program.Image.Sha256}");
        Summary = $"{recovered.Functions.Count} functions, {recovered.Blocks.Count} basic blocks; {regions.Count} structured functions, {regions.Values.Sum(r=>r.Loops)} loops, {regions.Values.Sum(r=>r.Conditionals)} conditionals; {memorySummary}; explicit inputs/results, direct calls and tail calls";
        Line("// Recovered source: " + Summary);
        for(int i=0;i<data.Count;i++) {
            var region=data[i]; Line($"// {region.Name}, original VA 0x{region.Address:x}");
            if(region.FileSize==0) Line($"static std::array<std::byte,{region.Size}> image_{i}{{}};");
            else {
                bool patched=program.Image.Rebases.Keys.Concat(program.Image.Bindings.Keys).Any(a=>a>=region.Address && a-region.Address<region.Size);
                Line($"static {(region.Writable || patched ? "" : "const ")}char image_{i}[] ="); indent++;
                foreach(var chunk in program.Image.ReadRegion(region).Chunk(64)) Line(Literal(chunk));
                indent--; Line(";");
            }
        }
        foreach(var plan in objectPlans.Values) foreach(string declaration in plan.Declarations()) Line(declaration);
        foreach(var function in recovered.Functions.Values) {
            var results=Results(function.Entry);
            if(results.Length>1) {
                Line($"struct result_{function.Entry:x} {{"); indent++;
                foreach(var slot in results) Line($"{Type(slot.Width)} {Field(slot)};");
                indent--; Line("};");
            }
            Line(Signature(function) + ";");
        }
        if(native!=null) foreach(var function in recovered.Functions.Values) EmitNumericAdapter(function);
        foreach(var (address,symbol) in program.Imports.OrderBy(i=>i.Key)) {
            var spec=ApplicationImports.Get(symbol);
            var parameters=Inputs(address).Select(s=>$"std::uint64_t a_{s.Index}");
            if(native==null) parameters=parameters.Append("CallFrame& call");
            Line($"static {(spec.Void ? "void" : "std::uint64_t")} import_{address:x}({string.Join(",",parameters)}) {{"); indent++;
            if(native==null) Line("if(call.depth>512) throw std::runtime_error(\"recovered call depth exhausted\");");
            Line(spec.Void ? spec.Expression+";" : $"auto value={spec.Expression};");
            if(native==null) {
                Line("call.return_address=read_memory<std::uint64_t>(call.stack_pointer);");
                Line("call.stack_pointer+=8;");
            }
            Line(spec.Void ? "return;" : "return value;"); indent--; Line("}");
        }
        Line("// Recovered function bodies");
        foreach(var function in recovered.Functions.Values)
            EmitFunction(function,regions.GetValueOrDefault(function.Entry),fallback.GetValueOrDefault(function.Entry));
        if(functionOnly) EmitExport(recovered.Functions[program.Entry]);
        else EmitEntry(recovered.Functions[program.Entry]);
        Line("} // namespace sharp_retro::source");
        if(!functionOnly) Line("""
            int main(int argc,char** argv) {
                try { return sharp_retro::source::recovered_program(argc,argv); }
                catch(const std::exception& error) {
                    std::fprintf(stderr,"recovered application: %s\n",error.what()); return 125;
                }
            }
            """);
        return output.ToString();
    }

    void EmitFunction(RecoveredFunction function, StructuredProgram? structured, string? fallback) {
        int functionStart=output.Length;
        currentFunction=function;
        frame=native?.GetValueOrDefault(function.Entry);
        ssa=ssaPlans.GetValueOrDefault(function.Entry);
        pointers=pointerPlans.GetValueOrDefault(function.Entry);
        frameArrays=frameArrayPlans.GetValueOrDefault(function.Entry);
        objects=objectPlans.GetValueOrDefault(function.Entry);
        Line(Signature(function)+" {"); indent++;
        Line(structured == null ? "// Control flow: basic-block fallback: " + fallback
            : $"// Control flow: structured; {structured.Loops} loops, {structured.Conditionals} conditionals, {structured.DuplicatedBlocks} duplicated blocks; edges validated");
        if(native==null) Line("if(call.depth>512) throw std::runtime_error(\"recovered call depth exhausted\");");
        callIndex=0; resultIndex=0; returnIndex=0; transferIndex=0;
        if(frame!=null) {
            if(frame.FrameSize!=0 && (ssa==null || ssa.NeedsFrame)) EmitNativeFrame();
        }
        if(objects!=null) foreach(var view in objects.Views)
            Line($"const {view.Name} {view.Variable}{{{RawSsaName(view.Base)}}};");
        if(structured==null && function.Blocks.Any(a=>recovered.Blocks[a].Block.Kind==TerminatorKind.Conditional)) Line("bool branch_condition;");
        if(ssa!=null) {
            Line($"// Function SSA: {ssa.Storage.Count} value carriers, {ssa.PhiCount} joins; {ssa.Promoted.Count} private cells promoted; selected-edge parallel transfers.");
            if(structured==null) {
                foreach(var definition in ssa.Storage)
                    Line($"[[maybe_unused]] {StorageType(definition)} {definition.Name};");
                Assign(ssa.Entry.Select(Transfer));
            }
        }
        var slots=function.Blocks.SelectMany(a=>recovered.Blocks[a].LiveIn.Concat(recovered.Blocks[a].LiveOut))
            .Concat(function.Results).ToHashSet();
        foreach(ulong address in function.Blocks) {
            var block=recovered.Blocks[address].Block;
            if(block.Kind==TerminatorKind.Call) slots.UnionWith(Results(block.Target));
        }
        slots.Add(FunctionRecovery.Stack);
        foreach(var slot in slots.OrderBy(s=>s.Kind).ThenBy(s=>s.Index)) {
            if(ssa!=null) continue;
            if(frame!=null && slot==FunctionRecovery.Stack) continue;
            if(frame!=null && function.Parameters.Contains(slot)) continue;
            string initializer=slot==FunctionRecovery.Stack ? " = call.stack_pointer"
                : function.Parameters.Contains(slot) ? " = "+ParameterName(slot) : "";
            Line($"[[maybe_unused]] {Type(slot.Width)} {slot.Name}{initializer};");
        }
        if(structured != null) {
            IReadOnlyList<CppSharedHelper> sharedHelpers=[];
            var body=new CppSequence([
                Capture(()=> { if(ssa!=null) Assign(ssa.Entry.Select(Transfer)); }),
                BuildRegion(function,structured.Body),new CppLine("throw std::logic_error(\"recovered function fell through\");")]);
            body=CppLoopExits.Lower(body);
            body=CppSourceTree.Clean(body);
            // First expose trivial edge-transfer scopes, then thread their
            // exact copy/exit continuations through the cleaned tail regions.
            if(ssa!=null) body=CppSourceTree.Clean(body);
            if(ssa!=null) {
                var types=ssa.Storage.ToDictionary(d=>d.Name,StorageType);
                body=CppLoopCopies.Simplify(body,types.Keys.ToHashSet());
                var coalesced=CppCarrierCoalescing.Reduce(body,types);
                body=CppSourceTree.Clean(coalesced.Body);
                body=CppLoopCopies.Simplify(body,coalesced.Types.Keys.ToHashSet());
                Line($"// Carriers: {types.Count} -> {coalesced.Types.Count}; {coalesced.CopiesRemoved} redundant copies removed.");
                var conditionTypes=coalesced.Types.Concat(Inputs(function.Entry).Select(s=>new KeyValuePair<string,string>(ParameterName(s),ParameterType(function.Entry,s)))).ToDictionary();
                var valueCalls=recovered.Functions.Values.Select(f=>f.Name).Concat(program.Imports.Keys.Select(a=>$"import_{a:x}"))
                    .Concat(objects?.Views.SelectMany(v=>v.Fields.Select(f=>f.Name)) ?? []).ToHashSet();
                var conditions=CppConditionPropagation.Reduce(body,conditionTypes,valueCalls);
                body=conditions.Body;
                Line($"// Predicates: {conditions.ForwardedUses} uses forwarded; {conditions.RemovedDefinitions} snapshots removed.");
                body=CppSourceTree.Clean(CppReturnForwarding.Reduce(body,coalesced.Types,ReturnType(function.Entry)));
                var lifetimes=CppVariableLifetimes.Split(body,coalesced.Types);
                body=lifetimes.Body;
                Line($"// Lifetimes: {lifetimes.SplitVariables} reused carriers separated into {lifetimes.AddedLifetimes} additional locals.");
                var placedTypes=lifetimes.Types.Append(new KeyValuePair<string,string>("branch_condition","bool")).ToDictionary();
                body=CppVariableScopes.Place(body,placedTypes);
                // This backend admits integer/boolean predicates only. Keep
                // ordered floating comparisons out of complement rewriting.
                body=CppBooleanExpressions.Clean(body,integerComparisons:true);
                body=CppLoopHeaders.Rewrite(body);
                var parameters=Inputs(function.Entry).ToDictionary(ParameterName,s=>ParameterType(function.Entry,s));
                var callTypes=recovered.Functions.Values.ToDictionary(f=>f.Name,f=>ReturnType(f.Entry));
                foreach(var target in program.Imports.Keys) callTypes[$"import_{target:x}"]=ReturnType(target);
                body=CppByteCopyLoops.Reduce(body,CppLocalTypes.Collect(body,parameters,callTypes),valueCalls).Body;
                var captures=parameters.Keys.ToHashSet();
                if(frame!=null && frame.FrameSize!=0 && ssa.NeedsFrame) { captures.Add("frame");captures.Add("frame_bytes"); }
                if(objects!=null) captures.UnionWith(objects.Views.Select(v=>v.Variable));
                var shared=CppSharedRegions.Factor(body,CppLocalTypes.Collect(body,parameters,callTypes),ReturnType(function.Entry),captures,favorReadability:true);
                sharedHelpers=shared.Helpers.Select(h=>h with {Body=CppReadability.Clean(h.Body,valueCalls)}).ToArray();
                body=CppReadability.Clean(shared.Body,valueCalls);
            }
            else if(function.Blocks.Any(a=>recovered.Blocks[a].Block.Kind==TerminatorKind.Conditional)) Line("bool branch_condition;");
            var pending=sharedHelpers.ToList();
            for(int i=0;i<body.Body.Count;i++) {
                var due=pending.Where(h=>CppReadability.Mentions(body.Body[i],h.Name)).ToArray();
                foreach(var helper in due) {
                    base.Line("");
                    base.Line($"auto {helper.Name} = [{string.Join(", ",helper.Captures.Select(n=>"&"+n))}]({string.Join(", ",helper.Params.Select(p=>"auto& "+p.Name))}) -> {helper.ReturnType} {{");
                    indent++;
                    CppSourceTree.Emit(helper.Body,(text,depth)=>base.Line(new string(' ',depth*4)+CppReadability.Commas(text)));
                    indent--;base.Line("};");pending.Remove(helper);
                }
                if(due.Length!=0)base.Line("");
                CppSourceTree.Emit(new([body.Body[i]]),(text,depth)=>base.Line(new string(' ',depth*4)+CppReadability.Commas(text)));
            }
            if(pending.Count!=0)throw new InvalidOperationException("Shared helper has no recovered call site");
        }
        else {
            Line($"goto block_{function.Entry:x};");
            foreach(ulong address in function.Blocks.Order()) EmitFunctionBlock(function,recovered.Blocks[address]);
        }
        if(structured==null) Line("throw std::logic_error(\"recovered function fell through\");");
        indent--; Line("}");
        string rendered=output.ToString(functionStart,output.Length-functionStart);
        output.Length=functionStart;output.Append(CppReadability.FrameAlias(rendered));
    }

    void EmitFunctionBlock(RecoveredFunction function,ValueBlock block) {
        var b=block.Block;
        Line($"block_{b.Address:x}: {{ // {b.Instructions.Count} original instructions"); indent++;
        EmitBlockBody(function,block);
        if(b.Kind==TerminatorKind.Call) { PhiEdge(b.Address,b.Alternative); Line($"goto block_{b.Alternative:x};"); }
        else if(b.Kind==TerminatorKind.Conditional) {
            Line("if(branch_condition) {"); indent++; Edge(function,b.Target); indent--; Line("} else {"); indent++;
            Edge(function,b.Alternative); indent--; Line("}");
        } else if(b.Kind!=TerminatorKind.Return) Edge(function,b.Target);
        indent--; Line("}");
    }

    protected override void Line(string text) {
        if(statements!=null) statements.Add(new CppLine(text));
        else base.Line(text);
    }

    protected override void Declaration(string name,string text) {
        if(statements!=null) statements.Add(new CppLine(text,name));
        else base.Declaration(name,text);
    }

    CppSequence Capture(Action emit) {
        var previous=statements;
        statements=[];
        try { emit(); return new CppSequence(statements); }
        finally { statements=previous; }
    }

    CppStatement BuildRegion(RecoveredFunction function,Region region,LoopRegion? context=null) {
        CppSequence AsSequence(CppStatement child) => child as CppSequence ?? new CppSequence([child]);
        switch(region) {
            case SequenceRegion sequence:
                return new CppSequence(sequence.Body.Select(child=>BuildRegion(function,child,context)).ToArray());
            case BlockRegion block:
                return new CppScope(Capture(()=> {
                    EmitBlockBody(function,recovered.Blocks[block.Block.Address]);
                    var b=block.Block;
                    if(b.Kind!=TerminatorKind.Return) PhiEdge(b.Address,b.Kind==TerminatorKind.Call ? b.Alternative : b.Target);
                }));
            case IfRegion conditional:
                return new CppSequence([
                    new CppScope(Capture(()=>EmitBlockBody(function,recovered.Blocks[conditional.Test.Address]))),
                    new CppIf(new("branch_condition"),new CppSequence([
                        new CppScope(Capture(()=>PhiEdge(conditional.Test.Address,conditional.Test.Target))),
                        BuildRegion(function,conditional.Then,context)]),
                        new CppSequence([
                        new CppScope(Capture(()=>PhiEdge(conditional.Test.Address,conditional.Test.Alternative))),
                        BuildRegion(function,conditional.Else,context)]))
                ]);
            case LoopRegion loop: {
                var body=AsSequence(BuildRegion(function,loop.Body,loop));
                return loop.Exits==null ? new CppWhile("true",body)
                    : new CppMultiExitLoop(loop.Header,body,loop.Exits.Select(exit=>new CppExitArm(exit.Target,AsSequence(BuildRegion(function,exit.Body,context)))).ToArray());
            }
            case BreakRegion { Target: { } target }:
                return new CppExitLoop(context!.Header,target);
            case BreakRegion: return new CppLine("break;");
            case ContinueRegion: return new CppLine("continue;");
            case BoundaryRegion boundary: return new CppScope(Capture(()=>Edge(function,boundary.Address)));
            default: throw new StructuringException("Unknown function region");
        }
    }

    void EmitBlockBody(RecoveredFunction function,ValueBlock block) {
        activeBlock=block;
        PrepareBlock(block); var b=block.Block;
        foreach(var effect in block.Effects) {
            if(frame?.RemovedEffects.Contains(effect)==true) continue;
            if(ssa?.IsPromotedStore(effect)==true) {
                if(ssa.StoreIsLive(effect)) Declare(effect.Value);
                continue;
            }
            if(effect.Kind=="evaluate" && ssa?.Load(effect.Value)!=null) continue;
            // One use is the evaluation effect itself. A proven native scalar
            // field is initialized, in-bounds and nonvolatile, so an otherwise
            // unused read has no observable effect. Generic loads and checked
            // accesses retain their evaluation/fault behavior.
            if(effect.Kind=="evaluate" && effect.Value.Operation is LiftIl.IlLoad && frame!=null &&
               uses.GetValueOrDefault(effect.Value.Id)==1 &&
               frame.Offset(block,effect.Value.Inputs[0]) is { } offset &&
               frame.Scalars.ContainsKey((offset,effect.Value.Width))) continue;
            if(effect.Kind=="evaluate") Declare(effect.Value);
            else if(effect.Kind=="fault") EmitFault(effect);
            else if(frame!=null) StoreNative(effect);
            else { string address=Atom(effect.Address!), value=Atom(effect.Value); Line($"write_memory<{Type(effect.Value.Width)}>({address},{value});"); }
        }
        if(block.Condition!=null) {
            string text=$"branch_condition={Atom(block.Condition)};";
            if(statements!=null) statements.Add(new CppLine(text,IsConditionDefinition:true));
            else Line(text);
        }
        string returnAddress=$"return_address_{returnIndex++}";
        if(b.Kind==TerminatorKind.Return && frame==null) Declaration(returnAddress,$"const auto {returnAddress}={Atom(block.Target!)};");
        if(ssa!=null) foreach(var definition in ssa.Definitions(b.Address)) Declare(definition.Value);
        else Commit(block.LiveOut.OrderBy(s=>s.Kind).ThenBy(s=>s.Index).Where(s=>(frame==null || s!=FunctionRecovery.Stack) && block.Output(s).Slot!=s).ToArray(),block);
        if(b.Kind==TerminatorKind.Return) {
            if(frame==null) { Line("call.stack_pointer=stack_pointer;"); Line($"call.return_address={returnAddress};"); }
            Return(function);
        } else if(b.Kind==TerminatorKind.Call) {
            var observed=recovered.Blocks[b.Alternative].LiveIn;
            if(frame==null) {
                string context=$"call_{callIndex++}";
                Declaration(context,$"CallFrame {context}{{stack_pointer,0,call.depth+1}};");
                Invoke(b.Target,context,observed);
                Line($"if({context}.return_address!=0x{b.Alternative:x}ULL) throw std::runtime_error(\"noncanonical recovered call return\");");
                Line($"stack_pointer={context}.stack_pointer;");
            } else Invoke(b.Target,observed:observed);
        }
    }

    void Edge(RecoveredFunction function,ulong target) {
        if(function.Blocks.Contains(target)) { PhiEdge(activeBlock!.Block.Address,target); Line($"goto block_{target:x};"); return; }
        Line("// Tail call: return the callee's results.");
        if(frame==null) { Line("call.stack_pointer=stack_pointer;"); Line("++call.depth;"); }
        // Tail calls return their callee's outputs; untouched caller outputs stay
        // in local variables. Result contracts are inferred to cover both cases.
        Invoke(target,frame==null ? "call" : null,function.Results);
        Return(function);
    }

    void Invoke(ulong target,string? context=null,IReadOnlySet<ValueSlot>? observed=null) {
        var results=Results(target);
        string call=Call(target,context);
        // Liveness describes values used after this call, independently of its
        // memory effects. Keep the call even when all its results are discarded.
        var used=results.Where(s=>(observed==null || observed.Contains(s)) &&
            (ssa==null || ssa.CallResultIsLive(activeBlock!.Block.Address,s))).ToArray();
        if(used.Length==0) { Line(call+";"); return; }
        string result=$"call_result_{resultIndex++}";
        Declaration(result,$"const auto {result}={call};");
        foreach(var slot in used) {
            string value=results.Length==1 ? result : result+"."+Field(slot);
            var definition=ssa?.CallResult(activeBlock!.Block.Address,slot);
            if(definition!=null && pointers?.Pointer(definition)==true) value=PointerBits(value,pointers.IsReadOnly(definition));
            Line($"{(definition!=null ? definition.Name : SlotName(slot))}={value};");
        }
    }

    void Return(RecoveredFunction function) {
        var results=Results(function.Entry);
        Line(results.Length==0 ? "return;" : results.Length==1 ? $"return {SlotName(results[0])};"
            : $"return {{{string.Join(",",results.Select(SlotName))}}};");
    }
    string Call(ulong target,string? context=null) {
        string Argument(ValueSlot slot) {
            if(pointerPlans.GetValueOrDefault(target)?.Parameters.GetValueOrDefault(slot) is { } role && activeBlock!=null) {
                // Calls consume the already committed output state. Rebuilding
                // a block expression here can refer to an old temporary after
                // lowering has captured it in an ABI/SSA carrier.
                if(ssa!=null) {
                    var value=ssa.Output(activeBlock.Block.Address,slot);
                    return pointers?.Pointer(value)==true ? AdaptPointer(RawSsaName(value),pointers.IsReadOnly(value),role.IsReadOnly)
                        : PointerBits(SsaName(value),role.IsReadOnly);
                }
                return PointerBits(SlotName(slot),role.IsReadOnly);
            }
            return SlotName(slot);
        }
        return $"{(recovered.Functions.TryGetValue(target,out var function) ? function.Name : $"import_{target:x}")}({string.Join(",",WithContext(Inputs(target).Select(Argument),context))})";
    }
    string Signature(RecoveredFunction function) => $"static {ReturnType(function.Entry)} {function.Name}({string.Join(",",WithContext(Inputs(function.Entry).Select(s=>$"{ParameterType(function.Entry,s)} {ParameterName(s)}"),"CallFrame& call"))})";
    static string PointerType(bool readOnly) => readOnly ? "const u8*" : "u8*";
    static string PointerBits(string expression,bool readOnly) => $"{(readOnly ? "const_byte_pointer" : "byte_pointer")}({expression})";
    string StorageType(SsaDefinition definition) => pointers?.Pointer(definition)==true ? PointerType(pointers.IsReadOnly(definition)) : Type(definition.Width);
    string ParameterType(ulong entry,ValueSlot slot) => pointerPlans.GetValueOrDefault(entry)?.Parameters.GetValueOrDefault(slot) is { } role ? PointerType(role.IsReadOnly) : Type(slot.Width);
    void EmitNumericAdapter(RecoveredFunction function) {
        var inputs=Inputs(function.Entry);
        var plan=pointerPlans.GetValueOrDefault(function.Entry);
        var pointerSlots=inputs.Select((s,i)=>(Slot:s,Index:i)).Where(p=>plan?.Parameters.ContainsKey(p.Slot)==true).ToArray();
        if(pointerSlots.Length==0) return;
        // Preserve the explicit machine-bit entry interface for existing callers.
        // Internal calls select the native pointer overload directly.
        Line($"template<{string.Join(",",pointerSlots.Select(p=>$"class T{p.Index}"))}>");
        Line($"requires ({string.Join(" && ",pointerSlots.Select(p=>$"std::is_integral_v<T{p.Index}>"))})");
        Line($"static {ReturnType(function.Entry)} {function.Name}({string.Join(",",inputs.Select((s,i)=>$"{(plan!.Parameters.ContainsKey(s) ? "T"+i : Type(s.Width))} bits_{i}"))}) {{"); indent++;
        var arguments=inputs.Select((s,i)=>plan!.Parameters.TryGetValue(s,out var role) ? PointerBits($"std::uint64_t(bits_{i})",role.IsReadOnly) : "bits_"+i);
        Line($"{(ReturnType(function.Entry)=="void" ? "" : "return ")}{function.Name}({string.Join(",",arguments)});");
        indent--;Line("}");
    }
    IEnumerable<string> WithContext(IEnumerable<string> inputs,string? context) => native!=null ? inputs
        : inputs.Append(context ?? throw new InvalidOperationException("Checked call requires its frame"));
    protected override string SlotName(ValueSlot slot) {
        if(ssa!=null && activeBlock!=null) {
            if(frame!.Offset(activeBlock,activeBlock.Output(slot)) is { } offset) return $"address({frame.Pointer(offset)})";
            return SsaName(ssa.Output(activeBlock.Block.Address,slot));
        }
        return native!=null && currentFunction?.Parameters.Contains(slot)==true ? ParameterName(slot) : slot.Name;
    }
    string ReturnType(ulong target) {
        var results=Results(target);
        return results.Length==0 ? "void" : results.Length==1 ? Type(results[0].Width) : $"result_{target:x}";
    }
    ValueSlot[] Results(ulong target) => (recovered.Functions.TryGetValue(target,out var function) ? (IEnumerable<ValueSlot>)function.Results : ApplicationImports.Results(program.Imports[target]))
        .OrderBy(s=>s.Kind).ThenBy(s=>s.Index).ToArray();
    ValueSlot[] Inputs(ulong target) => (recovered.Functions.TryGetValue(target,out var function) ? (IEnumerable<ValueSlot>)(ssaPlans.GetValueOrDefault(target)?.Parameters ?? function.Parameters) : ApplicationImports.Inputs(program.Imports[target],FunctionAbi.Windows(program.Image)))
        .Where(s=>s!=FunctionRecovery.Stack).OrderBy(s=>s.Kind==SlotKind.Local && Array.IndexOf(ArgumentRegisters,s.Index)>=0 ? Array.IndexOf(ArgumentRegisters,s.Index) : 100+(int)s.Kind*64+s.Index).ToArray();
    static string Field(ValueSlot slot) => slot==FunctionRecovery.Accumulator ? "value" : slot.Name;
    string ParameterName(ValueSlot slot) => slot.Kind==SlotKind.Local && Array.IndexOf(ArgumentRegisters,slot.Index) is var index && index>=0
        ? $"argument_{index}" : "incoming_"+slot.Name;

    void EmitExport(RecoveredFunction entry) {
        var inputs=Inputs(entry.Entry); var preserved=FunctionAbi.Preserved(program.Image);
        if(Results(entry.Entry).Length!=1 || Results(entry.Entry)[0]!=FunctionRecovery.Accumulator)
            throw new NotSupportedException("PE API profile requires an RAX result");
        if(inputs.Any(s=>s.Kind!=SlotKind.Local || !ArgumentRegisters.Contains(s.Index) && !preserved.Contains(s)))
            throw new NotSupportedException("PE API closure has unsupported live-in state");
        Line("// Win64 register arguments plus explicit recovered entry-state inputs; no DLL initialization.");
        var extra=inputs.Where(s=>!ArgumentRegisters.Contains(s.Index)).Select(s=>$"std::uint64_t {ParameterName(s)}=0");
        Line($"std::uint64_t recovered_export(std::uint64_t a0=0,std::uint64_t a1=0,std::uint64_t a2=0,std::uint64_t a3=0{(extra.Any() ? ","+string.Join(",",extra) : "")}) {{");
        indent++;
        var args=inputs.Select(s=>Array.IndexOf(ArgumentRegisters,s.Index) is var i && i>=0 ? "a"+i : ParameterName(s));
        Line($"return {entry.Name}({string.Join(",",args)});"); indent--; Line("}");
    }

    protected override string Atom(RecoveredValue value) {
        if(ssa?.Load(value) is { } loaded) return SsaName(loaded);
        if(frame!=null && activeBlock!=null && frame.Offset(activeBlock,value) is { } offset)
            return $"address({frame.Pointer(offset)})";
        if(ssa!=null && value.Slot is { } slot) return SsaName(ssa.Input(activeBlock!.Block.Address,slot));
        return base.Atom(value);
    }

    string RawSsaName(SsaDefinition definition) {
        definition=definition.Root;
        return definition.Kind switch {
            SsaDefinitionKind.Parameter => ParameterName(definition.Slot),
            SsaDefinitionKind.Constant => pointers?.Pointer(definition)==true ? PointerBits(base.Atom(definition.Value!),pointers.IsReadOnly(definition)) : base.Atom(definition.Value!),
            SsaDefinitionKind.FrameAddress => $"reinterpret_cast<u8*>({frame!.Pointer(definition.FrameOffset!.Value)})",
            _ => definition.Name
        };
    }
    string SsaName(SsaDefinition definition) {
        definition=definition.Root;
        if(definition.Kind==SsaDefinitionKind.Constant) return base.Atom(definition.Value!);
        if(definition.Kind==SsaDefinitionKind.FrameAddress) return $"address({frame!.Pointer(definition.FrameOffset!.Value)})";
        string name=RawSsaName(definition);
        return pointers?.Pointer(definition)==true ? $"address({name})" : name;
    }
    (string Target,string Value) Transfer((SsaDefinition Phi,SsaDefinition Value) transfer) {
        var (target,value)=transfer;
        string source=pointers?.Pointer(target)==true
            ? pointers.Pointer(value) ? AdaptPointer(RawSsaName(value),pointers.IsReadOnly(value),pointers.IsReadOnly(target)) : PointerBits(SsaName(value),pointers.IsReadOnly(target))
            : SsaName(value);
        return (target.Name,source);
    }
    static string AdaptPointer(string expression,bool sourceReadOnly,bool targetReadOnly) => sourceReadOnly && !targetReadOnly ? PointerBits($"address({expression})",false) : expression;

    void PhiEdge(ulong from,ulong to) {
        if(ssa!=null && currentFunction!.Blocks.Contains(to))
            Assign(ssa.Edge(from,to).Select(Transfer));
    }

    void Assign(IEnumerable<(string Target,string Value)> transfers) {
        // All RHS expressions are captured from the same pre-transfer state.
        // Break real cycles with immutable temporaries, including loop swaps.
        var pending=transfers.Where(p=>p.Target!=p.Value).ToDictionary(p=>p.Target,p=>p.Value);
        bool Uses(string expression,string name) => System.Text.RegularExpressions.Regex.Matches(expression,@"[A-Za-z_]\w*").Any(m=>m.Value==name);
        while(pending.Count!=0) {
            var ready=pending.Keys.FirstOrDefault(name=>!pending.Any(p=>p.Key!=name && Uses(p.Value,name)));
            if(ready!=null) {
                string text=$"{ready} = {pending[ready]};";
                if(statements!=null) statements.Add(new CppLine(text,IsEdgeTransfer:true)); else Line(text);
                pending.Remove(ready); continue;
            }
            var target=pending.Keys.First(); string saved=$"edge_value_{transferIndex++}";
            Declaration(saved,$"const auto {saved} = {pending[target]};"); pending[target]=saved;
        }
    }

    NativePointerRole? Role(RecoveredValue value) => activeBlock==null ? null : pointers?.Role(activeBlock.Block.Address,value);
    string PointerValue(RecoveredValue value,bool readOnly) {
        if(frame!=null && activeBlock!=null && frame.Offset(activeBlock,value) is { } coordinate)
            return $"reinterpret_cast<{PointerType(readOnly)}>({frame.Pointer(coordinate)})";
        if(value.Constant==0 && !value.PcRelative) return "nullptr";
        if(ssa!=null && activeBlock!=null && ssa.Resolve(activeBlock.Block.Address,value) is { } definition && pointers?.Pointer(definition)==true &&
           (value.Slot!=null || ssa.Load(value)!=null || declared.Contains(value.Id)))
            return AdaptPointer(RawSsaName(definition),pointers.IsReadOnly(definition),readOnly);
        if(declared.Contains(value.Id) && Role(value) is { } declaredRole)
            return AdaptPointer(base.Name(value),declaredRole.IsReadOnly,readOnly);
        if(value.Width==64 && value.Operation is LiftIl.IlBin {Op:LiftIl.BinOp.Add or LiftIl.BinOp.Sub} binary) {
            int pointer=Role(value.Inputs[0])!=null && Role(value.Inputs[1])==null ? 0
                : binary.Op==LiftIl.BinOp.Add && Role(value.Inputs[1])!=null && Role(value.Inputs[0])==null ? 1 : -1;
            if(pointer>=0) {
                string delta=Atom(value.Inputs[1-pointer]);
                var pointerBase=value.Inputs[pointer];
                if(value.Inputs[1-pointer] is {Constant:{ } constant,PcRelative:false,Width:<=64}) {
                    ulong offset=(ulong)constant;
                    if(binary.Op==LiftIl.BinOp.Sub)offset=unchecked(0UL-offset);
                    // Compose only unmaterialized constant address expressions.
                    // byte_offset is modular integer arithmetic; no object bounds
                    // or native pointer-arithmetic assumptions are introduced.
                    for(int depth=0;depth<32 && pointerBase.Slot==null && !declared.Contains(pointerBase.Id) && ssa?.Load(pointerBase)==null &&
                        pointerBase.Width==64 && pointerBase.Operation is LiftIl.IlBin {Op:LiftIl.BinOp.Add or LiftIl.BinOp.Sub} nested;depth++) {
                        int nestedPointer=Role(pointerBase.Inputs[0])!=null && Role(pointerBase.Inputs[1])==null ? 0
                            : nested.Op==LiftIl.BinOp.Add && Role(pointerBase.Inputs[1])!=null && Role(pointerBase.Inputs[0])==null ? 1 : -1;
                        if(nestedPointer<0 || pointerBase.Inputs[1-nestedPointer] is not {Constant:{ } part,PcRelative:false,Width:<=64})break;
                        offset=unchecked(offset+(nested.Op==LiftIl.BinOp.Sub ? 0UL-(ulong)part : (ulong)part));
                        pointerBase=pointerBase.Inputs[nestedPointer];
                    }
                    if(offset==0)return PointerValue(pointerBase,readOnly);
                    delta=offset>long.MaxValue ? $"-{PointerOffsetLiteral(unchecked(0UL-offset))}" : PointerOffsetLiteral(offset);
                } else if(binary.Op==LiftIl.BinOp.Sub) delta=$"-std::uint64_t({delta})";
                return $"byte_offset({PointerValue(pointerBase,readOnly)},{delta})";
            }
        }
        if(value.Operation is LiftIl.IlIfV && Role(value)!=null)
            return $"({Atom(value.Inputs[0])} ? {PointerValue(value.Inputs[1],readOnly)} : {PointerValue(value.Inputs[2],readOnly)})";
        if(value.Operation is LiftIl.IlCast {Kind:LiftIl.CastKind.Bitcast} && value.Width==64 && value.Inputs[0].Width==64)
            return PointerValue(value.Inputs[0],readOnly);
        return PointerBits(Atom(value),readOnly);
    }
    static string PointerOffsetLiteral(ulong value)=>value<10 ? $"{value}ULL" : $"0x{value:x}ULL";
    string NativePointer(RecoveredValue value,bool write=false) {
        if(frame!.Offset(activeBlock!,value) is { } offset) {
            if(frameArrays?.Find(offset,8) is { } array)
                return $"reinterpret_cast<u8*>(frame.{array.Name}.data())+{offset-array.Offset}";
            return frame.Pointer(offset);
        }
        if(value.PcRelative && value.Constant is { } bits && bits<=ulong.MaxValue)
            for(int i=0;i<data.Count;i++)
                if((ulong)bits>=data[i].Address && (ulong)bits-data[i].Address<data[i].Size)
                    return $"{ImagePointer(i)}+{(ulong)bits-data[i].Address}";
        return Role(value) is { } role ? PointerValue(value,write ? false : role.IsReadOnly) : $"pointer({Atom(value)})";
    }

    static string AccessType(int width) => width switch {
        8=>"u8",16=>"u16",32=>"u32",64=>"u64",128=>"u128",
        _=>throw new NotSupportedException("Native access width")
    };

    protected override void Declare(RecoveredValue value) {
        if(ssa?.Load(value)!=null) return;
        if(ssa?.Definition(value) is { } definition) {
            if(declared.Contains(value.Id)) return;
            if(value.Operation is not LiftIl.IlLoad) {
                string expression=pointers?.Pointer(definition)==true ? PointerValue(value,pointers.IsReadOnly(definition)) : Expression(value,destinationTyped:true);
                Line($"{definition.Name} = {expression};"); declared.Add(value.Id); return;
            }
            string loaded=LoadNative(value.Inputs[0],value.Width);
            if(pointers?.Pointer(definition)==true) loaded=PointerBits(loaded,pointers.IsReadOnly(definition));
            Line($"{definition.Name} = {loaded};");
            declared.Add(value.Id); return;
        }
        if(Role(value) is { } role && !declared.Contains(value.Id)) {
            string expression=value.Operation is LiftIl.IlLoad ? PointerBits(LoadNative(value.Inputs[0],value.Width),role.IsReadOnly) : PointerValue(value,role.IsReadOnly);
            Declaration(base.Name(value),$"{PointerType(role.IsReadOnly)} const {base.Name(value)}={expression};");
            declared.Add(value.Id);return;
        }
        if(frame==null || value.Operation is not LiftIl.IlLoad) { base.Declare(value); return; }
        if(declared.Contains(value.Id)) return;
        Declaration(Name(value),$"const {Type(value.Width)} {Name(value)}={LoadNative(value.Inputs[0],value.Width)};");
        declared.Add(value.Id);
    }

    string LoadNative(RecoveredValue where,int width) {
        if(ArrayElement(where,width) is { } element) return element;
        if(objects?.Access(activeBlock!.Block.Address,where,width) is { } member) return member;
        if(frame!.Offset(activeBlock!,where) is { } offset && frameArrays?.Find(offset,width)==null && frame.Scalars.TryGetValue((offset,width),out string? scalar)) return $"frame.{scalar}";
        if(width==8 && Role(where)!=null) return $"*{ParenthesizePointer(NativePointer(where))}";
        return $"*reinterpret_cast<const {AccessType(width)}*>({NativePointer(where)})";
    }
    static string ParenthesizePointer(string text) => text.All(c=>char.IsLetterOrDigit(c)||c=='_') ? text : "("+text+")";
    protected override string Name(RecoveredValue value) => ssa?.Definition(value) is { } d ? SsaName(d)
        : Role(value)!=null ? $"address({base.Name(value)})" : base.Name(value);

    void StoreNative(ValueEffect effect) {
        var where=effect.Address!; var value=effect.Value;
        string atom=Atom(value);
        if(ArrayElement(where,value.Width) is { } element) Line($"{element}={atom};");
        else if(objects?.Access(activeBlock!.Block.Address,where,value.Width) is { } member) Line($"{member}={atom};");
        else if(frame!.Offset(activeBlock!,where) is { } offset && frameArrays?.Find(offset,value.Width)==null && frame.Scalars.TryGetValue((offset,value.Width),out string? scalar))
            Line($"frame.{scalar}={atom};");
        else if(value.Width==8 && Role(where)!=null) Line($"*{ParenthesizePointer(NativePointer(where,write:true))}={atom};");
        else Line($"*reinterpret_cast<{AccessType(value.Width)}*>({NativePointer(where,write:true)})={atom};");
    }

    void EmitNativeFrame() {
        var fields=frame!.Scalars.Where(p=>ssa?.Promoted.Contains(p.Key)!=true).OrderBy(p=>p.Key.Offset).ToArray();
        int alignment=fields.Select(p=>p.Key.Width/8).DefaultIfEmpty(1).Max();
        Line($"struct alignas({alignment}) FrameStorage {{"); indent++;
        long position=0;
        var items=fields.Where(p=>frameArrays?.Find(p.Key.Offset,p.Key.Width)==null)
            .Select(p=>(Offset:p.Key.Offset,Bytes:p.Key.Width/8,Name:p.Value,Type:Type(p.Key.Width)))
            .Concat((frameArrays?.Arrays ?? []).Select(a=>(Offset:a.Offset,Bytes:a.Bytes,Name:a.Name,Type:$"std::array<{Type(a.ElementWidth)},{a.Bytes/(a.ElementWidth/8)}>")))
            .OrderBy(p=>p.Offset).ToArray();
        foreach(var item in items) {
            long start=frame.FrameSize+item.Offset;
            if(start>position) Line($"std::array<std::byte,{start-position}> bytes_{position:x};");
            Line($"{item.Type} {item.Name};");
            position=start+item.Bytes;
        }
        if(position<frame.FrameSize) Line($"std::array<std::byte,{frame.FrameSize-position}> bytes_{position:x};");
        indent--; Line("};");
        foreach(var item in items)
            Line($"static_assert(offsetof(FrameStorage,{item.Name})=={frame.FrameSize+item.Offset});");
        foreach(var array in frameArrays?.Arrays ?? [])
            Line($"static_assert(sizeof(std::array<{Type(array.ElementWidth)},{array.Bytes/(array.ElementWidth/8)}>)=={array.Bytes});");
        Line($"static_assert(sizeof(FrameStorage)>={frame.FrameSize});");
        Line("alignas(16) FrameStorage frame{};");
        Line("[[maybe_unused]] auto* frame_bytes=reinterpret_cast<std::byte*>(&frame);");
    }
    string? ArrayElement(RecoveredValue address,int width) {
        if(activeBlock==null || frameArrays?.Element(activeBlock.Block.Address,address,width) is not { } element) return null;
        string index=element.Index==null ? element.BiasElements.ToString()+"ULL"
            : element.BiasElements==0 ? Atom(element.Index) : $"({Atom(element.Index)})+{element.BiasElements}ULL";
        return $"frame.{element.Array.Name}[{index}]";
    }

    void EmitEntry(RecoveredFunction entry) {
        frame=null; activeBlock=null; currentFunction=null; ssa=null;pointers=null;frameArrays=null;objects=null;
        Line("int recovered_program(int argc,char** argv) {"); indent++;
        if(native!=null) { EmitNativeEntry(entry); indent--; Line("}"); return; }
        Line("alignas(16) std::array<std::byte,1024*1024> stack{};");
        Line("std::array<char*,1> empty_environment{};"); Line("accessible.clear();");
        Line("expose(stack.data(),stack.size(),true);"); Line("expose(empty_environment.data(),sizeof(empty_environment),true);");
        if(program.Image.HasClassicFixups) Line("expose(&stdin,sizeof(stdin),false); expose(&stdout,sizeof(stdout),false); expose(&stderr,sizeof(stderr),false);");
        if(entry.Parameters.Contains(new(SlotKind.Local,6))) {
            Line("expose(argv,(std::size_t(argc)+1)*sizeof(char*),true);");
            Line("std::size_t argument_bytes=(std::size_t(argc)+2)*sizeof(char*);");
            Line("for(int i=0;i<argc;++i) { auto n=std::strlen(argv[i])+1; argument_bytes+=n; expose(argv[i],n,true); }");
            Line("if(argument_bytes>1024*1024) throw std::runtime_error(\"native argument storage exceeds adapter limit\");");
        }
        for(int i=0;i<data.Count;i++) Line($"expose({ImagePointer(i)},{data[i].Size}ULL,{(data[i].Writable ? "true" : "false")});");
        EmitFixups();
        foreach(var slot in Inputs(entry.Entry)) {
            string initial=slot.Kind switch {
                SlotKind.DefinedFlag=>"true", SlotKind.Flag when slot.Index==1=>"true",
                SlotKind.Local when slot.Index==7=>"std::uint64_t(argc)", SlotKind.Local when slot.Index==6=>"address(argv)",
                SlotKind.Local when slot.Index is 1 or 2=>"address(empty_environment.data())", _=>"0"
            };
            Line($"[[maybe_unused]] {Type(slot.Width)} {slot.Name}={initial};");
        }
        Line("CallFrame call{address(stack.data()+stack.size()-8),0,0};");
        Line("write_memory<std::uint64_t>(call.stack_pointer,main_return);");
        Line($"const auto result={Call(entry.Entry,"call")};");
        Line("if(call.return_address!=main_return || call.stack_pointer!=address(stack.data()+stack.size())) throw std::runtime_error(\"unbalanced recovered main return\");");
        Line($"return int({(Results(entry.Entry).Length==1 ? "result" : "result.value")} & 255);");
        indent--; Line("}");
    }

    void EmitNativeEntry(RecoveredFunction entry) {
        if(Inputs(entry.Entry).Any(s=>s.Kind==SlotKind.Local&&s.Index is 1 or 2))Line("std::array<char*,1> empty_environment{};");
        EmitFixups();
        foreach(var slot in Inputs(entry.Entry)) {
            string initial=slot.Kind switch {
                SlotKind.DefinedFlag=>"true", SlotKind.Flag when slot.Index==1=>"true",
                SlotKind.Local when slot.Index==7=>"std::uint64_t(argc)", SlotKind.Local when slot.Index==6=>"address(argv)",
                SlotKind.Local when slot.Index is 1 or 2=>"address(empty_environment.data())", _=>"0"
            };
            Line($"{Type(slot.Width)} {slot.Name}={initial};");
        }
        Line($"const auto result={Call(entry.Entry)};");
        Line($"return int({(Results(entry.Entry).Length==1 ? "result" : "result.value")} & 255);");
    }

    void EmitFixups() {
        if(!program.Image.HasClassicFixups) return;
        string PointerAt(ulong address) {
            for(int i=0;i<data.Count;i++)
                if(address>=data[i].Address && address-data[i].Address<data[i].Size)
                    return $"{ImagePointer(i)}+{address-data[i].Address}";
            throw new NotSupportedException($"Fixup outside emitted data: 0x{address:x}");
        }
        void Patch(ulong slot,string expression) {
            Line($"{{ const std::uint64_t binding={expression}; std::memcpy({PointerAt(slot)},&binding,8); }}");
        }
        foreach(var (slot,target) in program.Image.Rebases.Where(r=>!program.Image.Bindings.ContainsKey(r.Key)))
            Patch(slot,$"address({PointerAt(target)})");
        foreach(var (slot,binding) in program.Image.Bindings)
            Patch(slot,ApplicationImports.BindingExpression(binding.Symbol));
    }
}
