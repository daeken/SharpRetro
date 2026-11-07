using LiftIl;

namespace SharpRetro.Decompiler;

/// Isolated Windows x64 API closure, not a Windows process/CRT loader. The
/// initial profile admits only read-only literal data before relocation slots,
/// four register arguments and explicitly bridged CRT memory operations.
public static class PeFunction {
    public static void ValidateImports(LiftedProgram program) {
        if(program.Image.Format!=BinaryFormat.Pe || program.Image.Architecture!=Architecture.X86_64)
            throw new NotSupportedException("PE function recovery requires Windows x64");
        foreach(var (address,symbol) in program.Imports) {
            if(symbol is not ("memcpy" or "memmove" or "memset") ||
               !program.Image.ImportLibraries.TryGetValue(address,out var library) || !library.Equals("msvcrt.dll",StringComparison.OrdinalIgnoreCase))
                throw new NotSupportedException("Unmodeled Windows import "+symbol);
        }
    }
    public static ImageRegion[] Validate(LiftedProgram program,RecoveredFunctions recovered) {
        ValidateImports(program);
        if(program.Image.PeImageSize==0) throw new NotSupportedException("PE function requires a declared mapped image size");
        if(program.Image.PeRelocations.Any(r=>r.Type!=10)) throw new NotSupportedException("PE function profile admits only DIR64 relocations");
        if(program.Instructions.Any(insn=>program.Image.PeRelocations.Any(r=>r.Address<insn.Next && insn.Address<r.Address+8)))
            throw new NotSupportedException("Relocation in selected PE instruction requires rebased lifting");
        var literals=new List<ImageRegion>();
        foreach(var r in program.Image.Regions.Where(r=>!r.Executable && !r.Writable && r.FileSize!=0 && r.Name==".rdata" && program.Image.IsPeReadable(r))) {
            ulong first=program.Image.PeRelocations.Where(p=>p.Address>=r.Address && p.Address-r.Address<r.FileSize)
                .Select(p=>p.Address-r.Address).DefaultIfEmpty(r.FileSize).Min();
            if(first!=0) literals.Add(r with { Size=first,FileSize=first });
        }
        var seen=new HashSet<int>();
        var imageInputs=recovered.Blocks.Keys.ToDictionary(a=>a,_=>new HashSet<ValueSlot>());
        bool changed;
        bool Derived(RecoveredValue value,IReadOnlySet<ValueSlot> inputs) => value.Operation is IlLoad ? false
            : value.Slot is { } slot ? inputs.Contains(slot)
            : value.PcRelative || value.Inputs.Any(v=>Derived(v,inputs));
        do {
            changed=false;
            foreach(var block in recovered.Blocks.Values) {
                var next=new HashSet<ValueSlot>(imageInputs[block.Block.Address]);
                foreach(var (slot,value) in block.Outputs) {
                    if(Derived(value,imageInputs[block.Block.Address])) next.Add(slot); else next.Remove(slot);
                }
                if(block.Block.Kind==TerminatorKind.Call)
                    next.ExceptWith(recovered.Functions.TryGetValue(block.Block.Target,out var called) ? called.MayWrite : FunctionAbi.ImportWrites(program.Image));
                foreach(var target in block.Block.FlowSuccessors.Where(imageInputs.ContainsKey)) {
                    int count=imageInputs[target].Count; imageInputs[target].UnionWith(next); changed|=count!=imageInputs[target].Count;
                }
            }
        } while(changed);
        void Visit(RecoveredValue value) {
            if(!seen.Add(value.Id)) return;
            if(value.Slot is { Kind:SlotKind.Segment }) throw new NotSupportedException("Segment/TLS-dependent PE function");
            if(value.Constant is { } absolute && !value.PcRelative && absolute<=ulong.MaxValue &&
               (ulong)absolute>=program.Image.ImageBase && (ulong)absolute-program.Image.ImageBase<program.Image.PeImageSize)
                throw new NotSupportedException("Absolute preferred-image constant requires PE relocation provenance");
            if(value.Constant is { } n && value.PcRelative && n<=ulong.MaxValue) {
                ulong address=(ulong)n;
                if(!literals.Any(r=>address>=r.Address && address-r.Address<r.Size))
                    throw new NotSupportedException($"PE function references nonliteral/relocated data 0x{address:x}");
            }
            if(value.Operation is IlLoad && value.Inputs[0].Constant is { } where && value.Inputs[0].PcRelative && where<=ulong.MaxValue &&
               !literals.Any(r=>(ulong)where>=r.Address && (ulong)where-r.Address<r.Size && (ulong)(value.Width/8)<=r.Size-((ulong)where-r.Address)))
                throw new NotSupportedException("PE literal load crosses admitted data");
            foreach(var child in value.Inputs) Visit(child);
        }
        foreach(var block in recovered.Blocks.Values) {
            var inputs=imageInputs[block.Block.Address];
            void CheckAddress(RecoveredValue pointer) {
                if(Derived(pointer,inputs) && (pointer.Constant==null || !pointer.PcRelative))
                    throw new NotSupportedException("Dynamic image-derived PE address needs a range proof");
            }
            var accessed=new HashSet<int>();
            void CheckLoads(RecoveredValue value) {
                if(!accessed.Add(value.Id)) return;
                if(value.Operation is IlLoad) CheckAddress(value.Inputs[0]);
                foreach(var child in value.Inputs) CheckLoads(child);
            }
            foreach(var effect in block.Effects) {
                // Canonical return-address stores are removed by native frame
                // recovery; their code addresses are not literal data accesses.
                if(effect.Kind=="store" && effect.Value.Width==64 && ReferenceEquals(effect,block.Effects.LastOrDefault()) &&
                   effect.Value.Constant==block.Block.Alternative && block.Block.Kind==TerminatorKind.Call) continue;
                if(effect.Kind=="store" && Derived(effect.Address!,inputs)) throw new NotSupportedException("PE closure writes image data");
                if(effect.Kind=="store" && Derived(effect.Value,inputs)) throw new NotSupportedException("PE closure spills an image-derived pointer");
                CheckLoads(effect.Value); if(effect.Address!=null) CheckLoads(effect.Address);
                Visit(effect.Value); if(effect.Address!=null) Visit(effect.Address);
            }
            if(block.Condition!=null) Visit(block.Condition);
            foreach(var slot in block.LiveOut.Where(s=>s!=FunctionRecovery.Stack)) { CheckLoads(block.Output(slot)); Visit(block.Output(slot)); }
            var owner=recovered.Functions[recovered.Owners[block.Block.Address]];
            if(block.Block.Kind==TerminatorKind.Return && owner.Entry!=program.Entry &&
               owner.Results.Any(slot=>Derived(block.Output(slot),inputs)))
                throw new NotSupportedException("Image-derived return escapes a PE helper");
            if(block.Block.Kind==TerminatorKind.Call) {
                if(recovered.Functions.TryGetValue(block.Block.Target,out var callee)) {
                    if(callee.Parameters.Where(s=>s!=FunctionRecovery.Stack).Any(s=>Derived(block.Output(s),inputs)))
                        throw new NotSupportedException("Image-derived argument crosses an internal PE call");
                } else {
                    if(Derived(block.Output(new(SlotKind.Local,1)),inputs)) throw new NotSupportedException("PE import writes image-derived destination");
                    var source=block.Output(new(SlotKind.Local,2));
                    if(Derived(source,inputs)) {
                        var length=block.Output(new(SlotKind.Local,8)).Constant;
                        if(program.Imports[block.Block.Target]=="memset" || source.Constant is not { } start || length is not { } bytes ||
                           start>ulong.MaxValue || bytes>ulong.MaxValue || !literals.Any(r=>(ulong)start>=r.Address && (ulong)start-r.Address<r.Size && (ulong)bytes<=r.Size-((ulong)start-r.Address)))
                            throw new NotSupportedException("Image-derived PE import source needs a bounded literal read");
                    }
                }
            }
        }
        return literals.ToArray();
    }
}
