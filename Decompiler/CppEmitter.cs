using System.Text;
using LiftIl;

namespace SharpRetro.Decompiler;

public enum ControlFlowStyle { Auto, Structured, Dispatch }

/// First, conservative source form: architectural state and explicit control flow.
/// Later ABI/SSA/structuring passes can replace this form without replacing the lifter.
public sealed class CppEmitter {
    readonly StringBuilder output = new();
    HashSet<int> defined = [];
    Action<IlBranch>? branchWriter;
    bool allowsCalls;
    public string ControlFlowSummary { get; private set; } = "";
    int indent;
    void Line(string text) => output.Append(' ', indent * 4).AppendLine(text);
    static int Width(Il n) => n.Ty is IlType.I { Bits: > 0 and <= 128 } i ? i.Bits
        : throw new NotSupportedException($"C++ integer backend does not support {n.Ty}");
    static NotSupportedException Unsupported(Il n) => new($"Unsupported C++ IL: {n}");
    static string Narrow(string expression, int width) => $"narrow(({expression}), {width})";

    public string Emit(LiftedProgram program, ControlFlowStyle style = ControlFlowStyle.Auto) {
        output.Clear(); indent = 0;
        branchWriter = null;
        allowsCalls = program.AllowsCalls;
        StructuredProgram? structured = null;
        string flow = "dispatch requested";
        if(style != ControlFlowStyle.Dispatch) {
            try {
                if(allowsCalls) throw new StructuringException("Interprocedural call/return execution currently uses dispatch");
                structured = Structuring.Recover(program);
                flow = $"structured; {structured.Graph.Blocks.Count} blocks, {structured.Loops} loops, {structured.Conditionals} conditionals";
            } catch(StructuringException e) {
                if(style == ControlFlowStyle.Structured) throw new NotSupportedException("Cannot structure control flow: " + e.Message, e);
                flow = "dispatch fallback: " + e.Message;
            }
        }
        ControlFlowSummary = flow;
        output.AppendLine(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "CppRuntime.hpp"))
            .Replace("#include \"IntegerRuntime.hpp\"", File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "IntegerRuntime.hpp"))));
        Line("namespace sharp_retro {");
        Line($"// Input SHA-256: {program.Image.Sha256}");
        Line($"// {program.Image.Format} / {program.Image.Architecture}; reachable instructions: {program.Instructions.Count}");
        Line($"// Control flow: {flow}");
        Line($"inline constexpr std::uint64_t entry = 0x{program.Entry:x}ULL;");
        Line("inline Exit run_dispatch(State& s, Memory& memory, std::uint64_t instruction_budget) {"); indent++;
        if(allowsCalls) Line("bool returning = false;");
        Line("goto dispatch;");
        foreach(var insn in program.Instructions) {
            Instruction(insn, $"L_{insn.Address:x}: ");
        }
        Line("dispatch:");
        Line("switch(s.pc) {"); indent++;
        foreach(var insn in program.Instructions) Line($"case 0x{insn.Address:x}ULL: goto L_{insn.Address:x};");
        foreach(var (address, symbol) in program.Imports.OrderBy(i => i.Key)) {
            Line($"case 0x{address:x}ULL:"); indent++;
            Line($"if(!memory.invoke_import({CppString(symbol)}, s)) throw std::runtime_error(\"unbound guest import\");");
            if(allowsCalls) Line("returning = true;");
            Line("goto dispatch;"); indent--;
        }
        Line(allowsCalls ? "default: return returning ? Exit::returned : Exit::boundary;" : "default: return Exit::boundary;");
        indent--; Line("}");
        indent--; Line("}");
        Line("inline Exit run(State& s, Memory& memory, std::uint64_t instruction_budget) {"); indent++;
        if(structured == null) Line("return run_dispatch(s, memory, instruction_budget);");
        else {
            // Keep the existing API's arbitrary-PC entry and budget-resume behavior.
            // A resumed middle instruction uses the baseline, without replaying a
            // structured region's earlier instructions or effects.
            Line("if(s.pc != entry) return run_dispatch(s, memory, instruction_budget);");
            EmitRegion(structured.Body);
            Line("throw std::logic_error(\"structured control flow fell through\");");
        }
        indent--; Line("}"); Line("} // namespace sharp_retro");
        return output.ToString();
    }

    void Instruction(LiftedInstruction insn, string label = "") {
        try {
            defined = [];
            Line($"{label}{{ // 0x{insn.Address:x}: {Convert.ToHexStringLower(insn.Bytes)}  {insn.Text}"); indent++;
            if(allowsCalls) Line("returning = false;");
            Line("if(instruction_budget == 0) return Exit::budget_exhausted;");
            Line("--instruction_budget;");
            foreach(int id in Temps(insn.Body).Distinct().Order()) Line($"[[maybe_unused]] bits t{id};");
            Statements(insn.Body.Body);
            if(branchWriter == null || insn.Body.Body.LastOrDefault() is not IlBranch) {
                Line($"s.pc = 0x{insn.Next:x}ULL;");
                if(branchWriter == null) Line("goto dispatch;");
            }
            indent--; Line("}");
        } catch(NotSupportedException e) {
            throw new NotSupportedException($"C++ emission failed at 0x{insn.Address:x} ({insn.Text}): {e.Message}", e);
        }
    }

    static string CppString(string value) => string.Join("", Encoding.UTF8.GetBytes(value).Select(b => $"\"\\x{b:x2}\""));

    void EmitBlock(BasicBlock block, string? condition = null) {
        var previous = branchWriter;
        branchWriter = branch => {
            if(branch.Kind == BranchKind.Ret) {
                Line($"s.pc = std::uint64_t({Expr(branch.Target)});");
                Line("return Exit::returned;");
            } else if(branch.Cond == null) Line($"s.pc = std::uint64_t({Expr(branch.Target)});");
            else {
                if(condition == null) throw new InvalidOperationException("Missing structured branch condition");
                Line($"{condition} = ({Expr(branch.Cond)} & 1) != 0;");
                Line($"s.pc = {condition} ? std::uint64_t({Expr(branch.Target)}) : 0x{block.Alternative:x}ULL;");
            }
        };
        foreach(var insn in block.Instructions) Instruction(insn);
        branchWriter = previous;
    }

    void EmitRegion(Region region) {
        switch(region) {
            case SequenceRegion s: foreach(var child in s.Body) EmitRegion(child); break;
            case BlockRegion b: EmitBlock(b.Block); break;
            case IfRegion i: {
                string condition = $"condition_{i.Test.Address:x}";
                Line($"bool {condition};");
                EmitBlock(i.Test, condition);
                Line($"if({condition}) {{"); indent++;
                EmitRegion(i.Then); indent--; Line("} else {"); indent++;
                EmitRegion(i.Else); indent--; Line("}"); break;
            }
            case LoopRegion l:
                Line($"while(true) {{ // loop at 0x{l.Header:x}"); indent++;
                EmitRegion(l.Body); indent--; Line("}"); break;
            case BreakRegion: Line("break;"); break;
            case ContinueRegion: Line("continue;"); break;
            case BoundaryRegion: Line("return Exit::boundary;"); break;
            default: throw new InvalidOperationException("Unknown structured region");
        }
    }

    static IEnumerable<int> Temps(Il n) => n switch {
        IlLet l => new[] { l.Id }, IlBlock b => b.Body.SelectMany(Temps),
        IlIf i => i.Then.Concat(i.Else).SelectMany(Temps), _ => []
    };

    void Statements(IEnumerable<Il> nodes) {
        foreach(var n in nodes) switch(n) {
            case IlBlock b: Statements(b.Body); break;
            case IlLet l:
                Line($"t{l.Id} = {Expr(l.Val)};"); defined.Add(l.Id); break;
            case IlWriteReg { Kind: RegKind.X86, Idx: >= 0 and < 16 } r:
                Line($"s.gpr[{r.Idx}] = std::uint64_t({Expr(r.Val)});"); break;
            case IlWriteReg { Kind: RegKind.Eflags, Idx: >= 0 and < 64, Val: IlUndefined } r:
                Line($"s.defined_flags &= ~(std::uint64_t(1) << {r.Idx});"); break;
            case IlWriteReg { Kind: RegKind.Eflags, Idx: >= -1 and < 64 } r:
                Line($"write_flag(s, {r.Idx}, {Expr(r.Val)});"); break;
            case IlStore store: {
                // Sequence address before value, as in LiftIl execution. Do not
                // rely on unspecified C++ argument evaluation order for loads.
                string address = Expr(store.Addr), value = Expr(store.Val);
                int width = Width(store.Val);
                if(width % 8 != 0) throw Unsupported(n);
                Line($"{{ auto address = std::uint64_t({address}); auto value = {value}; memory.store(address, value, {width}); }}");
                break;
            }
            case IlBranch b: {
                if(branchWriter != null) { branchWriter(b); break; }
                if(b.Kind == BranchKind.Call && !allowsCalls) throw Unsupported(n);
                if(b.Cond != null) { Line($"if(({Expr(b.Cond)} & 1) != 0) {{"); indent++; }
                Line($"s.pc = std::uint64_t({Expr(b.Target)});");
                if(b.Kind == BranchKind.Ret && allowsCalls) Line("returning = true;");
                Line(b.Kind == BranchKind.Ret && !allowsCalls ? "return Exit::returned;" : "goto dispatch;");
                if(b.Cond != null) { indent--; Line("}"); }
                break;
            }
            case IlIf conditional: {
                Line($"if(({Expr(conditional.Cond)} & 1) != 0) {{"); indent++;
                var before = new HashSet<int>(defined);
                Statements(conditional.Then); var afterThen = new HashSet<int>(defined);
                indent--; Line("} else {"); indent++;
                defined = before; Statements(conditional.Else);
                defined.IntersectWith(afterThen);
                indent--; Line("}"); break;
            }
            default: throw Unsupported(n);
        }
    }

    string Expr(Il n) {
        int width = Width(n);
        string value = n switch {
            IlConst c => $"((bits(0x{(ulong)(c.Bits >> 64):x}ULL) << 64) | bits(0x{(ulong)c.Bits:x}ULL))",
            IlReadReg { Kind: RegKind.X86, Idx: >= 0 and < 16 } r => $"bits(s.gpr[{r.Idx}])",
            IlReadReg { Kind: RegKind.Eflags, Idx: >= -1 and < 64 } r => $"read_flags(s, {r.Idx})",
            IlReadReg { Kind: RegKind.X86Seg, Idx: >= 0 and < 6 } r => $"bits(s.segments[{r.Idx}])",
            IlReadPc => "bits(s.pc)",
            IlTmp t when defined.Contains(t.Id) => $"t{t.Id}",
            IlBin b => Binary(b),
            IlUn u => Unary(u),
            IlCast c when c.Kind is CastKind.Zext or CastKind.Trunc or CastKind.Bitcast => Expr(c.X),
            IlCast { Kind: CastKind.Sext } c => $"sext({Expr(c.X)}, {Width(c.X)}, {width})",
            IlLoad l when width % 8 == 0 => $"memory.load(std::uint64_t({Expr(l.Addr)}), {width})",
            IlIfV c => $"(({Expr(c.Cond)} & 1) != 0 ? {Expr(c.Then)} : {Expr(c.Else)})",
            _ => throw Unsupported(n)
        };
        return Narrow(value, width);
    }

    string Binary(IlBin b) {
        int width = Width(b), leftWidth = Width(b.L), rightWidth = Width(b.R);
        bool comparison = b.Op is BinOp.Eq or BinOp.Ne or BinOp.Ult or BinOp.Ule or BinOp.Ugt or BinOp.Uge
            or BinOp.Slt or BinOp.Sle or BinOp.Sgt or BinOp.Sge;
        if(comparison && leftWidth != rightWidth) throw Unsupported(b);
        string a = Expr(b.L), c = Expr(b.R);
        // Capture left then right: nested loads must keep LiftIl effect order.
        string expression = b.Op switch {
            BinOp.Add => "a + b", BinOp.Sub => "a - b", BinOp.Mul => "a * b",
            BinOp.And => "a & b", BinOp.Or => "a | b", BinOp.Xor => "a ^ b",
            BinOp.Shl => $"shift_left(a, b, {width})", BinOp.Shr => $"shift_right(a, b, {width})",
            BinOp.Sar => $"shift_arithmetic(sext(a, {leftWidth}, {width}), b, {width})",
            BinOp.Ror => $"rotate_right(a, b, {width})",
            BinOp.UDiv => "unsigned_divide(a, b, false)", BinOp.URem => "unsigned_divide(a, b, true)",
            BinOp.SDiv => $"signed_divide(a, b, {width}, false)", BinOp.SRem => $"signed_divide(a, b, {width}, true)",
            BinOp.Eq => "a == b", BinOp.Ne => "a != b",
            BinOp.Ult => "a < b", BinOp.Ule => "a <= b", BinOp.Ugt => "a > b", BinOp.Uge => "a >= b",
            BinOp.Slt => $"signed_key(a, {leftWidth}) < signed_key(b, {rightWidth})",
            BinOp.Sle => $"signed_key(a, {leftWidth}) <= signed_key(b, {rightWidth})",
            BinOp.Sgt => $"signed_key(a, {leftWidth}) > signed_key(b, {rightWidth})",
            BinOp.Sge => $"signed_key(a, {leftWidth}) >= signed_key(b, {rightWidth})",
            _ => throw Unsupported(b)
        };
        return $"([&]() -> bits {{ auto a = {a}; auto b = {c}; return {expression}; }}())";
    }

    string Unary(IlUn u) {
        string x = Expr(u.X);
        return u.Op switch {
            UnOp.Neg => $"bits(0) - ({x})", UnOp.Not => $"~({x})",
            UnOp.Popcnt => $"popcount({x}, {Width(u.X)})",
            UnOp.Clz => $"leading_zeroes({x}, {Width(u.X)})",
            UnOp.Rbit => $"reverse_bits({x}, {Width(u.X)})", _ => throw Unsupported(u)
        };
    }
}
