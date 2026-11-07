using LiftIl;

namespace SharpRetro.Decompiler;

/// Legacy XMM copy admission. The lifter represents bits but does not express
/// aligned-move faults: allow these forms only at proven aligned literals/frame
/// offsets. Dynamic movaps/movdqa addresses require a future explicit fault guard.
public static class VectorCopyValidation {
    public static void ValidateKinds(LiftedProgram program) {
        bool Vector(Il node) => node switch {
            IlReadReg { Kind:RegKind.Xmm } or IlWriteReg { Kind:RegKind.Xmm }=>true,
            IlBlock b=>b.Body.Any(Vector), IlLet l=>Vector(l.Val),
            IlBin b=>Vector(b.L)||Vector(b.R), IlUn u=>Vector(u.X), IlCast c=>Vector(c.X),
            IlLoad l=>Vector(l.Addr), IlStore s=>Vector(s.Addr)||Vector(s.Val),
            IlIf i=>Vector(i.Cond)||i.Then.Any(Vector)||i.Else.Any(Vector),
            IlIfV i=>Vector(i.Cond)||Vector(i.Then)||Vector(i.Else),
            IlWriteReg r=>Vector(r.Val), IlBranch b=>Vector(b.Target)||(b.Cond!=null && Vector(b.Cond)),
            _=>node.Ty is IlType.Vec
        };
        foreach(var insn in program.Instructions)
            if(Vector(insn.Body) && insn.Text.Split(' ',2)[0] is not ("movdqu" or "movdqa" or "movups" or "movaps" or "movupd" or "movapd"))
                throw new NotSupportedException($"Only ordinary legacy 128-bit XMM copies are admitted (0x{insn.Address:x}: {insn.Text})");
    }
    public static bool HasAlignedMoves(LiftedProgram program) => program.Instructions.Any(insn=>
        insn.Text.StartsWith("movaps ") || insn.Text.StartsWith("movapd ") || insn.Text.StartsWith("movdqa "));
    public static void Validate(LiftedProgram program,RecoveredFunctions recovered,IReadOnlyDictionary<ulong,NativeFunctionMemory> frames) {
        foreach(var function in recovered.Functions.Values) {
            var frame=frames[function.Entry];
            foreach(var address in function.Blocks) {
                var block=recovered.Blocks[address].Block;
                var state=new Dictionary<ValueSlot,long>(frame.Entries[address]);
                foreach(var insn in block.Instructions) {
                    var one=block with { Address=insn.Address,Instructions=[insn] };
                    var value=ValueRecovery.Lower(new BlockGraph(insn.Address,new Dictionary<ulong,BasicBlock> { [insn.Address]=one },new Dictionary<ulong,HashSet<ulong>>())).Blocks[insn.Address];
                    bool aligned=insn.Text.StartsWith("movaps ") || insn.Text.StartsWith("movapd ") || insn.Text.StartsWith("movdqa ");
                    if(aligned) {
                        void Check(RecoveredValue pointer) {
                            bool valid=pointer.Constant is { } bits && pointer.PcRelative && bits%16==0;
                            valid|=NativeMemoryRecovery.Offset(pointer,state) is { } offset && ((offset+8)&15)==0;
                            if(!valid) throw new NotSupportedException($"Unproved aligned XMM memory access at 0x{insn.Address:x}");
                        }
                        foreach(var effect in value.Effects) {
                            if(effect.Value.Operation is IlLoad) Check(effect.Value.Inputs[0]);
                            if(effect.Kind=="store") Check(effect.Address!);
                        }
                    }
                    var next=new Dictionary<ValueSlot,long>(state);
                    foreach(var (slot,output) in value.Outputs) {
                        if(NativeMemoryRecovery.Offset(output,state) is { } offset) next[slot]=offset;
                        else next.Remove(slot);
                    }
                    state=next;
                }
            }
        }
    }
}
