using LiftIl;

namespace SharpRetro.Decompiler;

public enum SlotKind { Local, Flag, DefinedFlag, Segment, Vector }
public readonly record struct ValueSlot(SlotKind Kind, int Index) {
    public int Width => Kind==SlotKind.Vector ? 128 : Kind is SlotKind.Flag or SlotKind.DefinedFlag ? 1 : 64;
    public string Name => Kind switch {
        SlotKind.Local => Index == 4 ? "stack_pointer" : $"local_{Index}",
        SlotKind.Flag => $"flag_{Index}", SlotKind.DefinedFlag => $"defined_{Index}", SlotKind.Vector=>$"xmm_{Index}", _ => $"segment_{Index}"
    };
}
public sealed record RecoveredValue(int Id, int Width, Il? Operation, IReadOnlyList<RecoveredValue> Inputs,
    UInt128? Constant = null, ValueSlot? Slot = null, bool PcRelative = false);
public sealed record ValueEffect(string Kind, RecoveredValue Value, RecoveredValue? Address = null, IlFault? Fault = null);
public sealed class ValueBlock(BasicBlock block) {
    public BasicBlock Block { get; } = block;
    public List<ValueEffect> Effects { get; } = [];
    public Dictionary<ValueSlot, RecoveredValue> Inputs { get; } = [];
    public Dictionary<ValueSlot, RecoveredValue> Outputs { get; } = [];
    public RecoveredValue? Condition { get; set; }
    public RecoveredValue? Target { get; set; }
    public HashSet<ValueSlot> LiveIn { get; set; } = [];
    public HashSet<ValueSlot> LiveOut { get; set; } = [];
    public RecoveredValue Output(ValueSlot slot) => Outputs.TryGetValue(slot, out var value) ? value : Inputs[slot];
}
public sealed record RecoveredValues(BlockGraph Graph, IReadOnlyDictionary<ulong, ValueBlock> Blocks,
    IReadOnlyList<ulong> Returns, IReadOnlySet<ValueSlot> ImportLive, IReadOnlySet<ValueSlot> Slots, int ValueCount);

/// Instruction-local temporaries become block-wide immutable values. Register
/// copies disappear into reaching definitions; only live edge values need slots.
public sealed class ValueRecovery {
    readonly ValueBlock block;
    readonly Dictionary<int, RecoveredValue> temporaries = [];
    readonly Dictionary<string, RecoveredValue> expressions = [];
    readonly Dictionary<(UInt128 Bits, int Width, bool PcRelative), RecoveredValue> constants = [];
    static readonly ValueSlot Accumulator = new(SlotKind.Local, 0), Stack = new(SlotKind.Local, 4);
    int nextValue;
    ulong pc;

    ValueRecovery(BasicBlock block, int nextValue) { this.block = new(block); this.nextValue = nextValue; }
    static int Width(Il value) => value.Ty switch {
        IlType.I { Bits: >0 and <=128 } type=>type.Bits,
        IlType.Vec { Bits:128 }=>128,
        _=>throw new NotSupportedException($"Recovered values require integer/copy IL, got {value.Ty}")
    };
    static UInt128 Mask(int width) => width == 128 ? UInt128.MaxValue : (UInt128.One << width) - 1;
    RecoveredValue Constant(UInt128 bits, int width, bool pcRelative = false) {
        var key = (bits & Mask(width), width, pcRelative);
        if(!constants.TryGetValue(key, out var value)) constants[key] = value = new(nextValue++, width, null, [], key.Item1, PcRelative: pcRelative);
        return value;
    }
    RecoveredValue Input(ValueSlot slot) {
        if(!block.Inputs.TryGetValue(slot, out var value)) block.Inputs[slot] = value = new(nextValue++, slot.Width, null, [], Slot: slot);
        return value;
    }
    RecoveredValue Read(ValueSlot slot) => block.Outputs.TryGetValue(slot, out var value) ? value : Input(slot);
    void Write(ValueSlot slot, RecoveredValue value) => block.Outputs[slot] = Cast(value, slot.Width);

    RecoveredValue Make(Il operation, params RecoveredValue[] inputs) {
        int width = Width(operation);
        if(width==1 && operation is IlUn { Op:UnOp.Not } && inputs[0].Width==1) {
            var operand=inputs[0];
            if(operand.Operation is IlUn { Op:UnOp.Not } && operand.Inputs[0].Width==1) return operand.Inputs[0];
            if(operand.Operation is IlBin comparison) {
                BinOp? inverse=comparison.Op switch {
                    BinOp.Eq=>BinOp.Ne, BinOp.Ne=>BinOp.Eq,
                    BinOp.Ult=>BinOp.Uge, BinOp.Ule=>BinOp.Ugt, BinOp.Ugt=>BinOp.Ule, BinOp.Uge=>BinOp.Ult,
                    BinOp.Slt=>BinOp.Sge, BinOp.Sle=>BinOp.Sgt, BinOp.Sgt=>BinOp.Sle, BinOp.Sge=>BinOp.Slt,
                    _=>null
                };
                if(inverse is { } op) return Make(comparison with { Op=op },operand.Inputs.ToArray());
            }
        }
        // CSE applies only to pure typed operations. Loads and flag checks retain
        // their exact evaluation points and are never merged or moved over stores.
        string key = operation switch {
            IlBin b => $"b:{b.Op}:{width}", IlUn u => $"u:{u.Op}:{width}",
            IlCast c => $"c:{c.Kind}:{width}", IlIfV => $"select:{width}", _ => ""
        };
        if(key.Length != 0) {
            key += ":" + string.Join(',', inputs.Select(i => i.Id));
            if(expressions.TryGetValue(key, out var old)) return old;
        }
        var value = new RecoveredValue(nextValue++, width, operation, inputs);
        if(key.Length != 0) expressions[key] = value;
        return value;
    }

    RecoveredValue Cast(RecoveredValue value, int width, bool sign = false) {
        if(width == value.Width) return value;
        if(value.Constant is { } bits) {
            if(sign && width > value.Width && (bits & (UInt128.One << (value.Width - 1))) != 0) bits |= ~Mask(value.Width);
            return Constant(bits, width, value.PcRelative);
        }
        if(width<value.Width && value.Operation is IlCast cast)
            return Cast(value.Inputs[0],width,cast.Kind==CastKind.Sext && width>value.Inputs[0].Width);
        if(width<value.Width && value.Operation is IlBin { Op: BinOp.And or BinOp.Or or BinOp.Xor } bitwise) {
            // Low bits do not depend on the discarded high half of a register
            // merge. This removes false live-ins from AL/AH and SETcc updates.
            var ty=new IlType.I(false,width);
            return Binary(new IlBin(ty,bitwise.Op,new IlConst(ty,0),new IlConst(ty,0)),
                Cast(value.Inputs[0],width),Cast(value.Inputs[1],width));
        }
        return Make(new IlCast(new IlType.I(false, width), sign ? CastKind.Sext : width < value.Width ? CastKind.Trunc : CastKind.Zext,
            new IlConst(new IlType.I(false, value.Width), 0)), value);
    }

    RecoveredValue Binary(IlBin binary, RecoveredValue a, RecoveredValue b) {
        int width = Width(binary);
        // Pure strict comparison OR equality is an inclusive comparison only
        // when both predicates compare precisely the same typed operands.
        if(width==1 && binary.Op==BinOp.Or && a.Width==1 && b.Width==1) {
            if((Inclusive(a,b) ?? Inclusive(b,a)) is { } inclusive)
                return Make(inclusive.Comparison,inclusive.Inputs);
        }
        bool relative = (binary.Op is BinOp.Add or BinOp.Sub) && (a.PcRelative || b.PcRelative);
        if(a.Constant is { } x && b.Constant is { } y) {
            UInt128? folded = binary.Op switch {
                BinOp.Add => unchecked(x + y), BinOp.Sub => unchecked(x - y), BinOp.Mul => unchecked(x * y),
                BinOp.And => x & y, BinOp.Or => x | y, BinOp.Xor => x ^ y,
                BinOp.Shl => y >= (UInt128)width ? 0 : x << (int)y,
                BinOp.Shr => y >= (UInt128)width ? 0 : x >> (int)y,
                BinOp.Eq => x == y ? 1u : 0u, BinOp.Ne => x != y ? 1u : 0u,
                BinOp.Ult => x < y ? 1u : 0u, BinOp.Ule => x <= y ? 1u : 0u,
                BinOp.Ugt => x > y ? 1u : 0u, BinOp.Uge => x >= y ? 1u : 0u,
                _ => null
            };
            if(folded != null) return Constant(folded.Value, width, relative);
        }
        if(a.Id == b.Id && binary.Op is BinOp.Xor or BinOp.Sub or BinOp.Ne) return Constant(0, width);
        if(a.Id == b.Id && binary.Op == BinOp.Eq) return Constant(1, width);
        if(a.Id == b.Id && binary.Op is BinOp.And or BinOp.Or) return Cast(a, width);
        if(binary.Op==BinOp.And && (PossibleOnes(a)&PossibleOnes(b))==0) return Constant(0,width);
        if(binary.Op is BinOp.Eq or BinOp.Ne && b.Constant == 0 && b.Width == a.Width && a.Operation is IlBin { Op: BinOp.Sub }
            && a.Inputs.All(v => v.Width == a.Width))
            return Binary(binary, a.Inputs[0], a.Inputs[1]);
        if(binary.Op is BinOp.Eq or BinOp.Ne && a.Width == 1 && b.Width == 1) {
            if(b.Constant is { } boolean) {
                bool identity = binary.Op == BinOp.Eq ? boolean == 1 : boolean == 0;
                return identity ? a : Make(new IlUn(IlType.U1, UnOp.Not, new IlConst(IlType.U1, 0)), a);
            }
            // Recover the exact subtraction flag identity SF xor OF = signed
            // less-than. All widths and operands must match the lifted formula.
            if(TrySignedSubtract(a, b, out var left, out var right) || TrySignedSubtract(b, a, out left, out right))
                return Make(new IlBin(IlType.U1, binary.Op == BinOp.Ne ? BinOp.Slt : BinOp.Sge,
                    new IlConst(new IlType.I(false, left!.Width), 0), new IlConst(new IlType.I(false, right!.Width), 0)), left!, right!);
        }
        if(b.Constant == 0 && binary.Op is BinOp.Add or BinOp.Sub or BinOp.Or or BinOp.Xor) return Cast(a, width);
        if(b.Constant == 0 && binary.Op is BinOp.And or BinOp.Mul) return Constant(0, width);
        if(b.Constant == Mask(a.Width) && binary.Op == BinOp.And) return Cast(a, width);
        if(a.Constant == 0 && binary.Op is BinOp.Add or BinOp.Or or BinOp.Xor) return Cast(b, width);
        var result = Make(binary, a, b);
        if(binary.Op is BinOp.UDiv or BinOp.SDiv or BinOp.URem or BinOp.SRem)
            block.Effects.Add(new("evaluate", result)); // zero/overflow errors cannot disappear as dead code
        return result;
    }

    static (IlBin Comparison,RecoveredValue[] Inputs)? Inclusive(RecoveredValue strict,RecoveredValue equal) {
        if(strict.Operation is not IlBin comparison || equal.Operation is not IlBin { Op:BinOp.Eq } ||
           strict.Inputs.Count!=2 || equal.Inputs.Count!=2 ||
           strict.Inputs[0].Id!=equal.Inputs[0].Id || strict.Inputs[1].Id!=equal.Inputs[1].Id ||
           strict.Inputs[0].Width!=strict.Inputs[1].Width || equal.Inputs[0].Width!=equal.Inputs[1].Width ||
           strict.Inputs[0].Width!=equal.Inputs[0].Width) return null;
        BinOp? operation=comparison.Op switch {
            BinOp.Ult=>BinOp.Ule, BinOp.Ugt=>BinOp.Uge,
            BinOp.Slt=>BinOp.Sle, BinOp.Sgt=>BinOp.Sge, _=>null
        };
        return operation is { } op ? (comparison with { Op=op },strict.Inputs.ToArray()) : null;
    }

    static UInt128 PossibleOnes(RecoveredValue value,int depth=0) {
        if(value.Constant is { } constant) return constant;
        UInt128 mask=Mask(value.Width);
        if(depth>16) return mask;
        return value.Operation switch {
            IlCast { Kind: CastKind.Zext or CastKind.Trunc or CastKind.Bitcast } => PossibleOnes(value.Inputs[0],depth+1)&mask,
            IlBin { Op: BinOp.And } => PossibleOnes(value.Inputs[0],depth+1)&PossibleOnes(value.Inputs[1],depth+1)&mask,
            IlBin { Op: BinOp.Or or BinOp.Xor } => (PossibleOnes(value.Inputs[0],depth+1)|PossibleOnes(value.Inputs[1],depth+1))&mask,
            _ => mask
        };
    }

    static bool TrySignedSubtract(RecoveredValue sign, RecoveredValue overflow,
        out RecoveredValue? left, out RecoveredValue? right) {
        left = right = null;
        if(sign.Operation is not IlCast { Kind: CastKind.Trunc } || sign.Width != 1) return false;
        var shift = sign.Inputs[0];
        if(shift.Operation is not IlBin { Op: BinOp.Shr } || shift.Inputs[1].Constant != (UInt128)(shift.Width - 1)) return false;
        var difference = shift.Inputs[0];
        if(difference.Operation is not IlBin { Op: BinOp.Sub } || difference.Width != shift.Width) return false;
        if(overflow.Operation is not IlBin { Op: BinOp.Ne } || overflow.Inputs[1].Constant != 0) return false;
        var masked = overflow.Inputs[0];
        if(masked.Operation is not IlBin { Op: BinOp.And } || masked.Width != difference.Width ||
           masked.Inputs[1].Constant != (UInt128.One << (difference.Width - 1))) return false;
        var conjunction = masked.Inputs[0];
        if(conjunction.Operation is not IlBin { Op: BinOp.And } || conjunction.Width != difference.Width) return false;
        var first = conjunction.Inputs[0]; var second = conjunction.Inputs[1];
        if(first.Operation is not IlBin { Op: BinOp.Xor } || second.Operation is not IlBin { Op: BinOp.Xor } ||
           first.Width != difference.Width || second.Width != difference.Width) return false;
        var a = difference.Inputs[0]; var b = difference.Inputs[1];
        if(a.Width != difference.Width || b.Width != difference.Width || first.Inputs[0].Id != a.Id ||
           first.Inputs[1].Id != b.Id || second.Inputs[0].Id != a.Id || second.Inputs[1].Id != difference.Id) return false;
        left = a; right = b; return true;
    }

    RecoveredValue Evaluate(Il expression) {
        int width = Width(expression);
        switch(expression) {
            case IlConst c: return Constant(c.Bits, width);
            case IlTmp t: return temporaries.TryGetValue(t.Id, out var value) ? value : throw new NotSupportedException("Unbound instruction temporary");
            case IlReadPc: return Constant(pc, width, true);
            case IlReadReg { Kind: RegKind.X86, Idx: >= 0 and < 16 } r: return Cast(Read(new(SlotKind.Local, r.Idx)), width);
            case IlReadReg { Kind: RegKind.Xmm, Idx: >=0 and <16 } r: return Cast(Read(new(SlotKind.Vector,r.Idx)),width);
            case IlReadReg { Kind: RegKind.X86Seg, Idx: >= 0 and < 6 } r: return Cast(Read(new(SlotKind.Segment, r.Idx)), width);
            case IlReadReg { Kind: RegKind.Eflags, Idx: >= 0 and < 64 } r: {
                var defined = Read(new(SlotKind.DefinedFlag, r.Idx));
                var flag = Read(new(SlotKind.Flag, r.Idx));
                if(defined.Constant == 1) return flag;
                var check = Make(new IlIntrin(IlType.U1, "checked_flag"), flag, defined);
                block.Effects.Add(new("evaluate", check)); return check;
            }
            case IlBin b: return Binary(b, Evaluate(b.L), Evaluate(b.R));
            case IlUn u: {
                var operand = Evaluate(u.X);
                if(operand.Constant is { } bits && u.Op is UnOp.Neg or UnOp.Not)
                    return Constant(u.Op == UnOp.Neg ? unchecked(UInt128.Zero - bits) : ~bits, width);
                return Make(u, operand);
            }
            case IlCast c when c.Kind is CastKind.Zext or CastKind.Trunc or CastKind.Bitcast or CastKind.Sext:
                return Cast(Evaluate(c.X), width, c.Kind == CastKind.Sext);
            case IlLoad l when width % 8 == 0: {
                var load = Make(l, Evaluate(l.Addr)); block.Effects.Add(new("evaluate", load)); return load;
            }
            case IlIfV conditional: {
                var condition = Evaluate(conditional.Cond);
                if(condition.Constant is { } known) return Evaluate((known & 1) != 0 ? conditional.Then : conditional.Else);
                int effects = block.Effects.Count;
                var whenTrue = Evaluate(conditional.Then); var whenFalse = Evaluate(conditional.Else);
                if(block.Effects.Count != effects) throw new NotSupportedException("Source values cannot hoist conditional memory/flag effects");
                if(whenTrue.Id == whenFalse.Id) return whenTrue;
                return Make(conditional, condition, whenTrue, whenFalse);
            }
            default: throw new NotSupportedException("Unsupported recovered-value expression: " + expression);
        }
    }

    void Statements(IEnumerable<Il> statements) {
        foreach(var statement in statements) switch(statement) {
            case IlBlock b: Statements(b.Body); break;
            case IlLet let: temporaries[let.Id] = Evaluate(let.Val); break;
            case IlWriteReg { Kind: RegKind.X86, Idx: >= 0 and < 16 } write: Write(new(SlotKind.Local, write.Idx), Evaluate(write.Val)); break;
            case IlWriteReg { Kind: RegKind.Xmm, Idx: >=0 and <16 } write: Write(new(SlotKind.Vector,write.Idx),Evaluate(write.Val)); break;
            case IlWriteReg { Kind: RegKind.Eflags, Idx: >= 0 and < 64, Val: IlUndefined } write:
                Write(new(SlotKind.DefinedFlag, write.Idx), Constant(0, 1)); break;
            case IlWriteReg { Kind: RegKind.Eflags, Idx: >= 0 and < 64 } write:
                Write(new(SlotKind.Flag, write.Idx), Cast(Evaluate(write.Val), 1));
                Write(new(SlotKind.DefinedFlag, write.Idx), Constant(1, 1)); break;
            case IlStore store: {
                var address = Evaluate(store.Addr); var value = Evaluate(store.Val);
                if(value.Width % 8 != 0) throw new NotSupportedException("Non-byte source store");
                block.Effects.Add(new("store", value, address)); break;
            }
            case IlBranch branch:
                if(branch.Cond != null) block.Condition = Evaluate(branch.Cond);
                block.Target = Evaluate(branch.Target); break;
            case IlIf conditional: {
                var condition = Evaluate(conditional.Cond);
                if(condition.Constant is not { } known) {
                    static IEnumerable<Il> Flatten(IEnumerable<Il> body) => body.SelectMany(n=>n is IlBlock b ? Flatten(b.Body) : new[] { n });
                    var yes=Flatten(conditional.Then).ToArray(); var no=Flatten(conditional.Else).ToArray();
                    if(yes is [IlFault { Architecture:"x86",Vector:0 } fault] && no.Length==0)
                        block.Effects.Add(new("fault",condition,Fault:fault));
                    else if(no is [IlFault { Architecture:"x86",Vector:0 } other] && yes.Length==0)
                        block.Effects.Add(new("fault",Make(new IlUn(IlType.U1,UnOp.Not,new IlConst(IlType.U1,0)),condition),Fault:other));
                    else throw new NotSupportedException("Variable instruction-internal control requires a nested value region");
                    break;
                }
                Statements((known & 1) != 0 ? conditional.Then : conditional.Else); break;
            }
            case IlFault { Architecture:"x86",Vector:0 } fault:
                block.Effects.Add(new("fault",Constant(1,1),Fault:fault)); break;
            default: throw new NotSupportedException("Unsupported recovered-value effect: " + statement);
        }
    }

    public static RecoveredValues Recover(LiftedProgram program) {
        var graph = BasicBlocks.Build(program, includeCalls: true);
        var (blocks, initialNext) = Lower(graph);
        int next = initialNext;
        var returns = graph.Blocks.Values.Where(b => b.Kind == TerminatorKind.Call).Select(b => b.Alternative).Distinct().Order().ToArray();
        var importLive = new HashSet<ValueSlot>();
        var importReads = new[] { new ValueSlot(SlotKind.Local, 7), new ValueSlot(SlotKind.Local, 6), new ValueSlot(SlotKind.Local, 2), Stack };
        bool changed;
        do {
            changed = false;
            var returnLive = new HashSet<ValueSlot> { Accumulator, Stack };
            foreach(ulong address in returns) returnLive.UnionWith(blocks[address].LiveIn);
            var nextImports = new HashSet<ValueSlot>(returnLive);
            nextImports.Remove(Accumulator); nextImports.Remove(Stack); nextImports.UnionWith(importReads);
            if(!importLive.SetEquals(nextImports)) { importLive = nextImports; changed = true; }
            foreach(var block in blocks.Values.Reverse()) {
                var liveOut = block.Block.Kind == TerminatorKind.Return ? new HashSet<ValueSlot>(returnLive) : new HashSet<ValueSlot>();
                if(block.Block.Kind != TerminatorKind.Return)
                    foreach(var target in block.Block.Kind == TerminatorKind.Call ? new[] { block.Block.Target } : block.Block.Successors) {
                        if(blocks.TryGetValue(target, out var successor)) liveOut.UnionWith(successor.LiveIn);
                        else if(program.Imports.ContainsKey(target)) liveOut.UnionWith(importLive);
                        else throw new NotSupportedException("Source CFG has an unresolved boundary");
                    }
                var roots = Roots(block);
                foreach(var slot in liveOut) roots.Add(EnsureOutput(block, slot, ref next));
                var liveIn = Dependencies(roots);
                if(!block.LiveIn.SetEquals(liveIn) || !block.LiveOut.SetEquals(liveOut)) {
                    block.LiveIn = liveIn; block.LiveOut = liveOut; changed = true;
                }
            }
        } while(changed);
        var slots = blocks.Values.SelectMany(b => b.LiveIn.Concat(b.LiveOut)).Concat(importLive).ToHashSet();
        return new(graph, blocks, returns, importLive, slots, next);
    }

    internal static (Dictionary<ulong, ValueBlock> Blocks, int Next) Lower(BlockGraph graph) {
        if(graph.Blocks.Count > 2048 || graph.Blocks.Values.Any(b => b.Instructions.Count > 1024))
            throw new NotSupportedException("Recovered-value analysis limit: at most 2048 blocks and 1024 instructions per block");
        var blocks = new Dictionary<ulong, ValueBlock>();
        int next = 0;
        foreach(var b in graph.Blocks.Values.OrderBy(b => b.Address)) {
            var pass = new ValueRecovery(b, next);
            foreach(var instruction in b.Instructions) {
                pass.pc = instruction.Address; pass.temporaries.Clear();
                try { pass.Statements(instruction.Body.Body); }
                catch(NotSupportedException error) { throw new NotSupportedException($"Value recovery at 0x{instruction.Address:x}: {error.Message}", error); }
            }
            blocks[b.Address] = pass.block; next = pass.nextValue;
        }
        return (blocks, next);
    }

    internal static RecoveredValue EnsureOutput(ValueBlock block, ValueSlot slot, ref int next) => block.Outputs.TryGetValue(slot, out var value) ? value
        : block.Inputs.TryGetValue(slot, out value) ? value : block.Inputs[slot] = new(next++, slot.Width, null, [], Slot: slot);

    internal static List<RecoveredValue> Roots(ValueBlock block) {
        var roots = block.Effects.Select(e => e.Value).Concat(block.Effects.Where(e => e.Address != null).Select(e => e.Address!)).ToList();
        if(block.Condition != null) roots.Add(block.Condition);
        if(block.Block.Kind == TerminatorKind.Return && block.Target != null) roots.Add(block.Target);
        return roots;
    }

    internal static HashSet<ValueSlot> Dependencies(IEnumerable<RecoveredValue> roots) {
        var result = new HashSet<ValueSlot>(); var seen = new HashSet<int>();
        var pending = new Stack<RecoveredValue>(roots);
        while(pending.TryPop(out var value)) {
            if(!seen.Add(value.Id)) continue;
            if(value.Slot is { } slot) result.Add(slot);
            foreach(var input in value.Inputs) pending.Push(input);
        }
        return result;
    }
}
