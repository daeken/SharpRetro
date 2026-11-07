using LiftIl;
using XFusionCpu;

namespace SharpRetro.Decompiler;

public sealed record CodeRange(ulong Address, ulong Size) {
    public ulong End => checked(Address + Size);
    public bool Contains(ulong address) => address >= Address && address - Address < Size;
}
public sealed record LiftedInstruction(ulong Address, byte[] Bytes, IlBlock Body, string Text) {
    public ulong Next => checked(Address + (ulong)Bytes.Length);
}
public sealed record LiftedProgram(BinaryImage Image, ulong Entry, IReadOnlyList<CodeRange> Ranges,
    IReadOnlyList<LiftedInstruction> Instructions) {
    public bool AllowsCalls { get; init; }
    public IReadOnlyDictionary<ulong, string> Imports { get; init; } = new Dictionary<ulong, string>();
}

/// Recursive traversal from an explicit entry. Ranges are bounds, not guessed function extents.
public static class ControlFlow {
    public static LiftedProgram Recover(BinaryImage image, ulong entry, IReadOnlyList<CodeRange> ranges,
        int instructionLimit = 100_000, bool allowCalls = false) {
        var mode = image.Architecture switch {
            Architecture.X86 => XMode.Bits32, Architecture.X86_64 => XMode.Bits64,
            _ => throw new NotSupportedException("Initial frontend supports x86/x86-64 LiftIl; AArch64 is not wired yet")
        };
        if(instructionLimit <= 0) throw new ArgumentOutOfRangeException(nameof(instructionLimit));
        var bounds = ranges.OrderBy(r => r.Address).ToArray();
        if(bounds.Length == 0 || bounds.Any(r => r.Size == 0 || r.End <= r.Address))
            throw new ArgumentException("Nonempty, nonwrapping code ranges are required");
        for(int i = 1; i < bounds.Length; i++)
            if(bounds[i].Address < bounds[i - 1].End) throw new ArgumentException("Overlapping code ranges");
        if(!bounds.Any(r => r.Contains(entry))) throw new ArgumentException("Entry is outside selected ranges");
        var found = new SortedDictionary<ulong, LiftedInstruction>();
        var owners = new Dictionary<ulong, ulong>();
        var work = new Queue<ulong>();
        var queued = new HashSet<ulong>();
        var imports = new Dictionary<ulong, string>();
        void Enqueue(ulong pc) {
            if(allowCalls && image.ResolvePeImportThunk(pc,out string peImport)) { imports[pc]=peImport; return; }
            if(allowCalls && image.Imports.TryGetValue(pc, out string? name)) { imports[pc] = name; return; }
            if(bounds.Any(r => r.Contains(pc)) && queued.Add(pc)) work.Enqueue(pc);
        }
        Enqueue(entry);
        while(work.TryDequeue(out var pc)) {
            if(found.Count >= instructionLimit) throw new InvalidOperationException("Instruction discovery budget exhausted");
            if(owners.TryGetValue(pc, out var owner) && owner != pc)
                throw new NotSupportedException($"Overlapping instruction streams at 0x{pc:x} (inside 0x{owner:x})");
            var range = bounds.Single(r => r.Contains(pc));
            if(mode == XMode.Bits32 && (pc > uint.MaxValue || range.End > uint.MaxValue))
                throw new NotSupportedException("32-bit code range outside nonwrapping supported IP domain");
            var bytes = image.ReadExecutable(pc, (int)Math.Min(15UL, range.End - pc));
            if(!Disassembler.DecodeInsn(bytes, mode, out var decoded) || decoded.Len <= 0 || decoded.Len > bytes.Length)
                throw new NotSupportedException($"Cannot decode complete instruction at 0x{pc:x}");
            if(decoded.P.Lock || decoded.P.Rep || decoded.P.RepNz || decoded.P.VexValid || decoded.P.EvexValid)
                throw new NotSupportedException($"Prefix/atomic/vector execution model unsupported at 0x{pc:x}");
            IlBlock body;
            try { body = X86Lifter.Lift(in decoded, pc, mode) ?? throw new NotSupportedException("Lifter returned no IL"); }
            catch(Exception e) when(e is NotSupportedException or ArgumentException) {
                throw new NotSupportedException($"Lift failed at 0x{pc:x}: {e.Message}", e);
            }
            var insn = new LiftedInstruction(pc, bytes[..decoded.Len].ToArray(), body,
                Disassembler.Disassemble(bytes, pc, mode).Text);
            for(ulong a = pc; a < insn.Next; a++) {
                if(!owners.TryAdd(a, pc)) throw new NotSupportedException($"Overlapping instructions at 0x{a:x}");
            }
            found.Add(pc, insn);
            foreach(var branch in Branches(body)) {
                if(branch.Kind == BranchKind.Call && !allowCalls)
                    throw new NotSupportedException($"Call at 0x{pc:x}: interprocedural ABI recovery is not implemented yet");
                if(branch.Kind == BranchKind.Ret) continue;
                ulong? target = ConstantTarget(insn.Body, branch);
                if(target == null)
                    throw new NotSupportedException($"Indirect control flow at 0x{pc:x} requires target recovery");
                if(allowCalls && !bounds.Any(r => r.Contains(target.Value)) && !image.Imports.ContainsKey(target.Value) && !image.ResolvePeImportThunk(target.Value,out _))
                    throw new NotSupportedException($"Unresolved call/jump target 0x{target.Value:x} at 0x{pc:x}");
                Enqueue(target.Value);
            }
            bool terminal = body.Body.Any(n => n is IlBranch { Cond: null, Kind: BranchKind.Ret or BranchKind.Jmp });
            if(!terminal) Enqueue(insn.Next);
        }
        foreach(var address in imports.Keys)
            if(owners.TryGetValue(address,out ulong owner) && owner!=address)
                throw new NotSupportedException("Import thunk overlaps a recovered instruction");
        return new(image, entry, bounds, found.Values.ToArray()) { AllowsCalls = allowCalls, Imports = imports };
    }

    internal static ulong? ConstantTarget(IlBlock body, IlBranch branch) {
        // CALL captures even a direct constant in an instruction-local temporary
        // before changing SP. Resolve only constants/copies before this branch;
        // register-derived or memory-derived targets remain unknown.
        var constants = new Dictionary<int, ulong>();
        ulong? Value(Il value) => value switch {
            IlConst(_, var bits) when bits <= ulong.MaxValue => (ulong)bits,
            IlTmp t when constants.TryGetValue(t.Id, out var bits) => bits,
            IlCast { Kind: CastKind.Zext or CastKind.Trunc or CastKind.Bitcast, Ty: IlType.I { Bits: > 0 and <= 64 } type } c
                when Value(c.X) is { } bits => type.Bits == 64 ? bits : bits & ((1UL << type.Bits) - 1),
            _ => null
        };
        if(branch.Target is IlConst) return Value(branch.Target);
        foreach(var statement in body.Body) {
            if(ReferenceEquals(statement, branch)) return Value(branch.Target);
            if(statement is IlLet let) {
                if(Value(let.Val) is { } bits) constants[let.Id] = bits;
                else constants.Remove(let.Id);
            } else if(statement is IlIf) constants.Clear();
        }
        return null;
    }

    static IEnumerable<IlBranch> Branches(Il n) => n switch {
        IlBranch b => [b], IlBlock b => b.Body.SelectMany(Branches),
        IlIf i => i.Then.Concat(i.Else).SelectMany(Branches), _ => []
    };
}
