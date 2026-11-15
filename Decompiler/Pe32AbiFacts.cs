using LiftIl;

namespace SharpRetro.Decompiler;

public sealed record Pe32Transfer(ulong InstructionRva,string Kind,ulong? DirectTarget,ulong? ImportSlot,ulong? Continuation);
public sealed record Pe32AbiReport(IReadOnlyList<int> CalleeCleanupBytes,IReadOnlyList<int> EntryRegisterReads,
    IReadOnlyList<Pe32StackAccess> StackAccesses,IReadOnlyList<Pe32Transfer> Transfers,
    string CallingConvention,string SourceReturnType,string SourceParameterTypes,string Completeness);

/// Report binary/IR observations without equating a stack slot or ECX use with
/// a proved source prototype. RET immediates are facts; convention labels aren't.
public static class Pe32AbiFacts {
    public static Pe32AbiReport Analyze(LiftedProgram program,Pe32ObjectPlan objects) {
        var cleanup=new HashSet<int>();
        var transfers=new List<Pe32Transfer>();
        IEnumerable<IlBranch> Branches(Il node) => node switch {
            IlBranch branch=>[branch],IlBlock block=>block.Body.SelectMany(Branches),
            IlIf conditional=>conditional.Then.Concat(conditional.Else).SelectMany(Branches),_=>[]
        };
        foreach(var instruction in program.Instructions) {
            foreach(var branch in Branches(instruction.Body)) {
                if(branch.Kind==BranchKind.Ret) {
                    if(instruction.Bytes is [0xc3] or [0xf3,0xc3]) cleanup.Add(0);
                    else if(instruction.Bytes is [0xc2,var low,var high]) cleanup.Add(low|(high<<8));
                    continue;
                }
                ulong? slot=null;
                var temporaries=new Dictionary<int,Il>();
                foreach(var statement in instruction.Body.Body) {
                    if(ReferenceEquals(statement,branch)) break;
                    if(statement is IlLet let) temporaries[let.Id]=let.Val;
                }
                Il Resolve(Il value,int depth=0) => depth>64 ? value : value switch {
                    IlTmp tmp when temporaries.TryGetValue(tmp.Id,out var definition)=>Resolve(definition,depth+1),
                    IlCast cast when cast.Kind is CastKind.Zext or CastKind.Trunc or CastKind.Bitcast=>Cast(cast,depth+1),
                    _=>value
                };
                Il Cast(IlCast cast,int depth) {
                    var value=Resolve(cast.X,depth);
                    return value is IlConst constant && cast.Ty is IlType.I {Bits:>0 and <=128} type
                        ? new IlConst(cast.Ty,constant.Bits & (type.Bits==128 ? UInt128.MaxValue : (UInt128.One<<type.Bits)-1)) : value;
                }
                if(Resolve(branch.Target) is IlLoad load && Resolve(load.Addr) is IlConst constant && constant.Bits<=uint.MaxValue &&
                   program.Image.PeImportSlots.ContainsKey((ulong)constant.Bits)) slot=(ulong)constant.Bits;
                transfers.Add(new(instruction.Address-program.Image.ImageBase,branch.Kind.ToString(),
                    ControlFlow.ConstantTarget(instruction.Body,branch),slot,
                    branch.Kind==BranchKind.Call ? instruction.Next : null));
            }
        }
        return new(cleanup.Order().ToArray(),objects.EntryRegisterReads,objects.StackAccesses,transfers,
            "unknown; RET cleanup and entry input uses do not uniquely determine cdecl/stdcall/thiscall/fastcall",
            "unknown; machine EAX bits remain explicit", "unknown; stack spans and register reads are observations, not a source parameter list",
            transfers.Any(t=>t.Kind==nameof(BranchKind.Call)) ? "partial; post-call register facts conservatively invalidated" : "bounded selected control-flow analysis, not a source signature proof");
    }
}
