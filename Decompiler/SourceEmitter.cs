using System.Text;
using LiftIl;

namespace SharpRetro.Decompiler;

/// Source-oriented block SSA backend. Only live cross-block definitions require
/// mutable locals; instruction temporaries, register copies and dead flags vanish.
public class SourceEmitter {
    protected readonly StringBuilder output = new();
    protected readonly HashSet<int> declared = [];
    protected readonly Dictionary<int, int> uses = [];
    protected IReadOnlyList<ImageRegion> data = [];
    protected int indent;
    public string Summary { get; protected set; } = "";
    protected virtual void Line(string text) => output.Append(' ', indent * 4).AppendLine(text);
    protected virtual void Declaration(string name,string text) => Line(text);
    protected virtual string SlotName(ValueSlot slot) => slot.Name;
    protected static string Type(int width) => width switch {
        1 => "bool", <= 8 => "std::uint8_t", <= 16 => "std::uint16_t", <= 32 => "std::uint32_t",
        <= 64 => "std::uint64_t", <= 128 => "bits", _ => throw new NotSupportedException("Source integer width")
    };
    static string Convert(string expression, int width) => width switch {
        1 => $"bool(({expression}) & 1)", 8 or 16 or 32 or 64 or 128 => $"static_cast<{Type(width)}>({expression})",
        _ => $"static_cast<{Type(width)}>(({expression}) & mask({width}))"
    };
    protected virtual string Name(RecoveredValue value) => $"value_{value.Id}";

    public virtual string Emit(LiftedProgram program) {
        VectorCopyValidation.ValidateKinds(program);
        if(VectorCopyValidation.HasAlignedMoves(program)) throw new NotSupportedException("Flat checked backend does not model aligned XMM faults");
        data = MachOApplication.Validate(program);
        var values = ValueRecovery.Recover(program);
        output.Clear(); indent = 0;
        Line(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "IntegerRuntime.hpp")).Replace("#pragma once\n", ""));
        Line(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "NativeRuntime.hpp")));
        Line("namespace sharp_retro::source {");
        Line($"// Input SHA-256: {program.Image.Sha256}");
        Summary = $"{program.Instructions.Count} instructions -> {values.Blocks.Count} basic blocks; block SSA, copy propagation, live edge values, native storage";
        Line("// Recovered source: " + Summary);
        for(int i = 0; i < data.Count; i++) {
            var region = data[i];
            Line($"// {region.Name}, original VA 0x{region.Address:x}");
            if(region.FileSize == 0) Line($"static std::array<std::byte,{region.Size}> image_{i}{{}};");
            else {
                byte[] bytes = program.Image.ReadRegion(region);
                Line($"static {(region.Writable ? "" : "const ")}char image_{i}[] ="); indent++;
                foreach(var chunk in bytes.Chunk(64)) Line(Literal(chunk));
                indent--; Line(";");
            }
        }
        Line("int recovered_program(int argc, char** argv) {"); indent++;
        Line("alignas(16) std::array<std::byte,1024*1024> stack{};");
        Line("std::array<char*,1> empty_environment{};");
        Line("accessible.clear();");
        Line("expose(stack.data(),stack.size(),true);");
        Line("expose(empty_environment.data(),sizeof(empty_environment),true);");
        if(values.Blocks[program.Entry].LiveIn.Contains(new(SlotKind.Local, 6))) {
            Line("expose(argv,(std::size_t(argc)+1)*sizeof(char*),true);");
            Line("std::size_t argument_bytes=(std::size_t(argc)+2)*sizeof(char*);");
            Line("for(int i=0;i<argc;++i) { auto n=std::strlen(argv[i])+1; argument_bytes+=n; expose(argv[i],n,true); }");
            Line("if(argument_bytes>1024*1024) throw std::runtime_error(\"native argument storage exceeds adapter limit\");");
        }
        for(int i = 0; i < data.Count; i++) Line($"expose({ImagePointer(i)},{data[i].Size}ULL,{(data[i].Writable ? "true" : "false")});");
        foreach(var slot in values.Slots.OrderBy(s => s.Kind).ThenBy(s => s.Index)) {
            string initial = slot.Kind switch {
                SlotKind.DefinedFlag => "true", SlotKind.Flag when slot.Index == 1 => "true",
                SlotKind.Local when slot.Index == 4 => "address(stack.data()+stack.size()-8)",
                SlotKind.Local when slot.Index == 7 => "std::uint64_t(argc)", SlotKind.Local when slot.Index == 6 => "address(argv)",
                SlotKind.Local when slot.Index is 1 or 2 => "address(empty_environment.data())", _ => "0"
            };
            Line($"[[maybe_unused]] {Type(slot.Width)} {slot.Name} = {initial};");
        }
        Line("std::uint64_t return_address = 0;");
        Line("write_memory<std::uint64_t>(stack_pointer,main_return);");
        Line($"goto block_{program.Entry:x};");
        foreach(var block in values.Blocks.Values.OrderBy(b => b.Block.Address)) EmitBlock(block);
        foreach(var (address, symbol) in program.Imports.OrderBy(i => i.Key)) {
            Line($"import_{address:x}: {{ // {symbol}"); indent++;
            Line($"local_0 = input_output({(symbol == "_read" ? "true" : "false")},local_7,local_6,local_2);");
            Line("return_address = read_memory<std::uint64_t>(stack_pointer);");
            Line("stack_pointer += 8;"); Line("goto return_dispatch;"); indent--; Line("}");
        }
        Line("return_dispatch:");
        Line("if(return_address == main_return) {"); indent++;
        Line("if(stack_pointer != address(stack.data()+stack.size())) throw std::runtime_error(\"unbalanced recovered stack\");");
        Line("return int(local_0 & 255);"); indent--; Line("}");
        Line("switch(return_address) {"); indent++;
        foreach(ulong address in values.Returns) Line($"case 0x{address:x}ULL: goto block_{address:x};");
        Line("default: throw std::runtime_error(\"unresolved recovered return\");"); indent--; Line("}");
        indent--; Line("}"); Line("} // namespace sharp_retro::source");
        Line("""
            int main(int argc, char** argv) {
                try { return sharp_retro::source::recovered_program(argc,argv); }
                catch(const std::exception& error) {
                    std::fprintf(stderr,"recovered application: %s\n",error.what()); return 125;
                }
            }
            """);
        return output.ToString();

        void EmitBlock(ValueBlock block) {
            PrepareBlock(block);
            var b = block.Block;
            Line($"block_{b.Address:x}: {{ // {b.Instructions.Count} original instructions"); indent++;
            foreach(var effect in block.Effects) {
                if(effect.Kind == "evaluate") Declare(effect.Value);
                else if(effect.Kind == "fault") EmitFault(effect);
                else {
                    string address = Atom(effect.Address!), value = Atom(effect.Value);
                    Line($"write_memory<{Type(effect.Value.Width)}>({address},{value});");
                }
            }
            if(block.Condition != null) Line($"const bool branch_condition = {Atom(block.Condition)};");
            if(b.Kind == TerminatorKind.Return) Line($"return_address = {Atom(block.Target!)};");
            var exports = block.LiveOut.OrderBy(s => s.Kind).ThenBy(s => s.Index)
                .Where(s => block.Output(s).Slot != s).ToArray();
            Commit(exports, block);
            if(b.Kind == TerminatorKind.Return) Line("goto return_dispatch;");
            else if(b.Kind == TerminatorKind.Conditional) {
                Line($"if(branch_condition) goto {Label(b.Target)};"); Line($"goto {Label(b.Alternative)};");
            } else Line($"goto {Label(b.Target)};");
            indent--; Line("}");
        }
        string Label(ulong address) => program.Imports.ContainsKey(address) ? $"import_{address:x}" : $"block_{address:x}";
    }

    protected void PrepareBlock(ValueBlock block) {
        declared.Clear(); uses.Clear(); var seen = new HashSet<int>();
        void Count(RecoveredValue value) {
            uses[value.Id] = uses.GetValueOrDefault(value.Id) + 1;
            if(seen.Add(value.Id)) foreach(var child in value.Inputs) Count(child);
        }
        foreach(var effect in block.Effects) { Count(effect.Value); if(effect.Address != null) Count(effect.Address); }
        if(block.Condition != null) Count(block.Condition);
        if(block.Block.Kind == TerminatorKind.Return) Count(block.Target!);
        foreach(var slot in block.LiveOut) Count(block.Output(slot));
    }

    protected void EmitFault(ValueEffect effect) => Line($"if({Atom(effect.Value)}) throw std::runtime_error(\"x86 fault vector {effect.Fault!.Vector}\");");

    protected void Commit(IReadOnlyList<ValueSlot> exports, ValueBlock block) {
        var pending = exports.ToDictionary(slot => slot, slot => block.Output(slot));
        var captured = new Dictionary<ValueSlot, string>();
        HashSet<ValueSlot> Dependencies(RecoveredValue value) {
            var slots = new HashSet<ValueSlot>(); var seen = new HashSet<int>(); var work = new Stack<RecoveredValue>(); work.Push(value);
            while(work.TryPop(out var current)) {
                if(!seen.Add(current.Id) || declared.Contains(current.Id)) continue;
                if(current.Slot is { } slot) slots.Add(slot);
                foreach(var child in current.Inputs) work.Push(child);
            }
            return slots;
        }
        while(pending.Count != 0) {
            ValueSlot? ready = pending.Keys.Where(slot => !pending.Any(other => other.Key != slot && !captured.ContainsKey(other.Key)
                && Dependencies(other.Value).Contains(slot))).Select(slot => (ValueSlot?)slot).FirstOrDefault();
            if(ready == null) {
                // Break an actual parallel-copy cycle with one immutable value.
                var slot = pending.Keys.First(s => !captured.ContainsKey(s));
                string temporary = "next_" + slot.Name;
                Declaration(temporary,$"const auto {temporary} = {Atom(pending[slot])};"); captured[slot] = temporary;
            } else {
                var slot = ready.Value;
                Line($"{SlotName(slot)} = {(captured.TryGetValue(slot, out string? saved) ? saved : Atom(pending[slot]))};");
                pending.Remove(slot);
            }
        }
    }

    protected string ImagePointer(int index) => data[index].FileSize == 0 ? $"image_{index}.data()" : $"image_{index}";
    protected static string Literal(byte[] bytes) {
        var text = new StringBuilder("\"");
        foreach(byte b in bytes) text.Append(b switch {
            0 => "\\0\"\"", 10 => "\\n", 13 => "\\r", 9 => "\\t", 34 => "\\\"", 92 => "\\\\",
            >= 32 and < 127 => ((char)b).ToString(), _ => $"\\x{b:x2}\"\""
        });
        return text.Append('"').ToString();
    }
    string Constant(RecoveredValue value) {
        UInt128 bits = value.Constant!.Value;
        if(value.PcRelative && bits <= ulong.MaxValue)
            for(int i = 0; i < data.Count; i++)
                if((ulong)bits >= data[i].Address && (ulong)bits - data[i].Address < data[i].Size)
                    return $"address({ImagePointer(i)}+{(ulong)bits-data[i].Address})";
        if(value.Width == 1) return bits == 0 ? "false" : "true";
        return UnsignedLiteral(bits);
    }
    static string UnsignedLiteral(UInt128 bits) => bits <= ulong.MaxValue ? bits < 10 ? $"{bits}ULL" : $"0x{(ulong)bits:x}ULL"
            : $"((bits(0x{(ulong)(bits>>64):x}ULL)<<64)|bits(0x{(ulong)bits:x}ULL))";
    protected virtual string Atom(RecoveredValue value) {
        if(value.Constant != null) return Constant(value);
        if(value.Slot is { } slot) return SlotName(slot);
        if(declared.Contains(value.Id)) return Name(value);
        if(uses.GetValueOrDefault(value.Id) <= 1 && value.Operation is IlBin or IlUn or IlCast or IlIfV && Complexity(value) <= 12)
            return "(" + Expression(value) + ")";
        Declare(value); return Name(value);
    }
    static int Complexity(RecoveredValue value) {
        if(value.Constant != null || value.Slot != null) return 0;
        int size = 1;
        foreach(var input in value.Inputs) { size += Complexity(input); if(size > 12) break; }
        return size;
    }
    protected virtual void Declare(RecoveredValue value) {
        if(value.Constant != null || value.Slot != null || declared.Contains(value.Id)) return;
        string expression = Expression(value,destinationTyped:true);
        Declaration(Name(value),$"{(uses.GetValueOrDefault(value.Id) <= 1 ? "[[maybe_unused]] " : "")}const {Type(value.Width)} {Name(value)} = {expression};");
        declared.Add(value.Id);
    }
    protected string Expression(RecoveredValue value,bool destinationTyped=false) {
        var args = value.Inputs.Select(Atom).ToArray();
        string expression = value.Operation switch {
            IlBin binary => Binary(value, binary.Op, args),
            IlUn unary => unary.Op switch {
                UnOp.Neg => $"std::uint64_t(0)-{args[0]}", UnOp.Not => value.Width == 1 ? $"!{args[0]}" : $"~{args[0]}",
                UnOp.Popcnt => $"popcount({args[0]},{value.Inputs[0].Width})",
                UnOp.Clz => $"leading_zeroes({args[0]},{value.Inputs[0].Width})",
                UnOp.Rbit => $"reverse_bits({args[0]},{value.Inputs[0].Width})",
                _ => throw new NotSupportedException("Source unary operation " + unary.Op)
            },
            IlCast cast => cast.Kind == CastKind.Sext ? $"sext({args[0]},{value.Inputs[0].Width},{value.Width})" : args[0],
            IlIfV => $"{args[0]} ? {args[1]} : {args[2]}",
            IlLoad => $"read_memory<{Type(value.Width)}>({args[0]})",
            IlIntrin { Name: "checked_flag" } => $"checked_flag({args[0]},{args[1]})",
            _ => throw new NotSupportedException("Unsupported source value operation")
        };
        bool alreadyTyped = value.Operation is IlLoad or IlIntrin || value.Operation is IlBin { Op: BinOp.Eq or BinOp.Ne or BinOp.Ult or BinOp.Ule or BinOp.Ugt or BinOp.Uge or BinOp.Slt or BinOp.Sle or BinOp.Sgt or BinOp.Sge };
        // All 64-bit atoms have an unsigned 64-bit C++ carrier. Arithmetic
        // explicitly promotes its left operand when needed; its result already
        // has that carrier width. Narrow results still require their truncation.
        alreadyTyped |= value.Operation switch {
            IlBin { Op:BinOp.Add or BinOp.Sub or BinOp.Mul } =>
                value.Width==128 || value.Width==64 && value.Inputs.All(v=>v.Width<=64),
            IlBin { Op:BinOp.And or BinOp.Or or BinOp.Xor } =>
                value.Width==64 && value.Inputs.All(v=>v.Width==64) ||
                value.Width==1 && value.Inputs.All(v=>v.Width==1),
            IlUn { Op:UnOp.Not } => value.Width==1 || value.Width==64 && value.Inputs[0].Width==64,
            _ => false
        };
        alreadyTyped |= NativeShift(value) && value.Width is 64 or 128;
        // An exact-width assignment already performs the final conversion.
        // Nested operands still keep their casts: their C++ promotions matter.
        // Boolean low-bit truncation and nonstandard widths still need masks.
        return alreadyTyped || destinationTyped && value.Width is 8 or 16 or 32 or 64 or 128
            ? expression : Convert(expression,value.Width);
    }
    static string Binary(RecoveredValue value, BinOp op, string[] args) {
        string a=args[0], b=args[1]; int width=value.Width, operandWidth=value.Inputs[0].Width;
        string carrier = width > 64 ? "bits" : "std::uint64_t";
        string promoted=carrier=="std::uint64_t" && value.Inputs[0].Width==64 ? a : $"static_cast<{carrier}>({a})";
        if(NativeShift(value)) {
            // Left shifts use an unsigned carrier large enough for the result;
            // their existing destination conversion retains low-width wrap.
            // Right shifts must discard high input bits BEFORE shifting.
            string left=op==BinOp.Shl ? promoted : operandWidth>width || width is not (1 or 8 or 16 or 32 or 64 or 128) ? Convert(a,width) : a;
            if(op==BinOp.Shr && width>32 && (operandWidth!=width || width is not (64 or 128)))
                left=$"static_cast<{carrier}>({left})";
            // Small 128-bit literals are spelled ULL by Constant(); their
            // declared IR width alone does not give that atom a bits carrier.
            if(op==BinOp.Shr && width==128 && value.Inputs[0].Constant!=null)
                left=$"static_cast<bits>({left})";
            return $"{left}{(op==BinOp.Shl ? "<<" : ">>")}{b}";
        }
        if(op==BinOp.Add && width is 64 or 128 && value.Inputs[1].Width==width &&
           !value.Inputs[1].PcRelative && value.Inputs[1].Constant is { } offset &&
           offset>=(UInt128.One<<(width-1))) {
            UInt128 magnitude=unchecked(UInt128.Zero-offset);
            if(width==64) magnitude&=ulong.MaxValue;
            return $"{promoted}-{UnsignedLiteral(magnitude)}";
        }
        if(op==BinOp.And && width is 64 or 128 && value.Inputs[1].Width==width &&
           !value.Inputs[1].PcRelative && value.Inputs[1].Constant is { } mask) {
            UInt128 cleared=(width==64 ? (UInt128)ulong.MaxValue : UInt128.MaxValue)^mask;
            if(cleared<=ushort.MaxValue) b=width==64 ? $"~{UnsignedLiteral(cleared)}" : $"~bits({UnsignedLiteral(cleared)})";
        }
        return op switch {
            BinOp.Add => $"{promoted}+{b}", BinOp.Sub => $"{promoted}-{b}", BinOp.Mul => $"{promoted}*{b}",
            BinOp.And => $"{a}&{b}", BinOp.Or => $"{a}|{b}", BinOp.Xor => $"{a}^{b}",
            BinOp.Shl => $"shift_left({a},{b},{width})", BinOp.Shr => $"shift_right({a},{b},{width})",
            BinOp.Sar => $"shift_arithmetic(sext({a},{operandWidth},{width}),{b},{width})", BinOp.Ror => $"rotate_right({a},{b},{width})",
            BinOp.UDiv => $"unsigned_divide({a},{b},false)", BinOp.URem => $"unsigned_divide({a},{b},true)",
            BinOp.SDiv => $"signed_divide({a},{b},{width},false)", BinOp.SRem => $"signed_divide({a},{b},{width},true)",
            BinOp.Eq => $"{a}=={b}", BinOp.Ne => $"{a}!={b}", BinOp.Ult => $"{a}<{b}", BinOp.Ule => $"{a}<={b}",
            BinOp.Ugt => $"{a}>{b}", BinOp.Uge => $"{a}>={b}",
            BinOp.Slt => $"signed_key({a},{operandWidth})<signed_key({b},{operandWidth})",
            BinOp.Sle => $"signed_key({a},{operandWidth})<=signed_key({b},{operandWidth})",
            BinOp.Sgt => $"signed_key({a},{operandWidth})>signed_key({b},{operandWidth})",
            BinOp.Sge => $"signed_key({a},{operandWidth})>=signed_key({b},{operandWidth})",
            _ => throw new NotSupportedException("Source binary operation " + op)
        };
    }
    static bool NativeShift(RecoveredValue value)=>value.Width is >0 and <=128 &&
        value.Operation is IlBin {Op:BinOp.Shl or BinOp.Shr} && value.Inputs.Count==2 &&
        !value.Inputs[1].PcRelative && value.Inputs[1].Constant is { } count && count<(UInt128)value.Width;
}
