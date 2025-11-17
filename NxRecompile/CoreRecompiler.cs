using System.Diagnostics;
using Aarch64Cpu;
using CoreArchCompiler;
using JitBase;
using StaticRecompilerBase;

namespace NxRecompile;

public class CoreRecompiler : Recompiler {
    readonly ExeLoader ExeLoader;

    readonly List<StaticIRStatement>[][] AllInstructions;
    readonly HashSet<ulong> KnownBlocks = [];
    readonly HashSet<ulong> KnownFunctions = [];
    readonly Dictionary<ulong, (ulong End, List<StaticIRStatement> Statements, List<ulong> LeadsTo)> Blocks = [];
    
    ulong PC;
    StaticBuilder<ulong> Builder;
    StaticStructRef<ulong, CpuState> State;

    public CoreRecompiler(ExeLoader loader) {
        ExeLoader = loader;
        AllInstructions = ExeLoader.ExeModules.Select(module => {
            return Enumerable.Range(0, (int) ((module.TextEnd - module.TextStart) / 4))
                .Select(List<StaticIRStatement> (i) => null).ToArray();
        }).ToArray();
    }

    public string Disassemble(ulong addr, uint? insn = null) {
        try {
            return Disassembler.Disassemble(insn ?? ExeLoader.Load<uint>(addr, textOnly: true), addr);
        } catch { // We want a blanket catch here; if disassembly fails for any reason, it's a bad insn
            return null;
        }
    }

    public bool IsValidCodeAt(ulong addr) => Disassemble(addr) != null;

    public void Recompile() {
        KnownBlocks.Add(ExeLoader.EntryPoint);
        KnownFunctions.Add(ExeLoader.EntryPoint); // TODO: Should we be calling the ep a known *function*?
        LinearScan();
        BuildBlockGraph();
        Console.WriteLine($"Blocks: {Blocks.Count}");
        foreach(var (addr, (end, _, leadsTo)) in Blocks) {
            Console.WriteLine($"{addr:X}-{end:X} leads to [{string.Join(", ", leadsTo.Select(x => $"{x:X}"))}]");
        }
    }

    public void LinearScan() {
        foreach(var (arr, module) in AllInstructions.Zip(ExeLoader.ExeModules)) {
            var i = 0;
            for(var addr = module.TextStart; addr <= module.TextEnd; addr += 4, ++i) {
                Builder = new StaticBuilder<ulong>();
                PC = addr;
                State = new StaticStructRef<ulong, CpuState>(Builder, new StaticRuntimeValue<ulong>(new StaticIRValue.Named("State", typeof(ulong))));
                var insn = ExeLoader.Load<uint>(PC);
                var dasm = Disassemble(PC, insn);
                if(dasm == null) continue;
                Console.WriteLine($"{PC:X}: {dasm}");
                if(RecompileOne(Builder, State, insn, PC))
                    arr[i] = Builder.BodyStmts;
            }
        }
    }

    bool DoesBranch(List<StaticIRStatement> stmts) {
        var branched = false;
        foreach(var stmt in stmts) {
            stmt.Walk(x => branched |= x is StaticIRStatement.Branch);
            if(branched) return true;
        }
        return false;
    }
    
    void BuildBlockGraph() {
        foreach(var (arr, module) in AllInstructions.Zip(ExeLoader.ExeModules)) {
            var addr = module.TextStart;
            var curBlock = addr;
            var statements = new List<StaticIRStatement>();
            var didBranch = false;
            List<ulong> LeadingTo() {
                var leadsTo = new HashSet<ulong>();
                new StaticIRStatement.Body(statements).Walk(x => {
                    if(x is StaticIRStatement.Branch { Address: StaticIRValue.Literal(var laddr, var type) } && type == typeof(ulong))
                        leadsTo.Add((ulong) laddr);
                });
                return leadsTo.Order().ToList();
            }
            foreach(var insn in arr) {
                if(didBranch || insn == null || (curBlock != addr && KnownBlocks.Contains(addr))) {
                    didBranch = false;
                    if(curBlock != addr && statements.Count != 0) {
                        if(insn != null && !DoesBranch([statements.Last()]))
                            statements.Add(new StaticIRStatement.Branch(new StaticIRValue.Literal(addr, typeof(ulong))));
                        Blocks[curBlock] = (addr, statements, LeadingTo());
                        statements = [];
                    }
                    curBlock = addr;
                    if(insn == null) {
                        addr += 4;
                        continue;
                    }
                }
                statements.AddRange(insn);
                addr += 4;
                if(DoesBranch(insn))
                    didBranch = true;
            }
            if(statements.Count > 0)
                Blocks[curBlock] = (addr, statements, LeadingTo());
        }
    }

    public void CleanupIR() {
    }

    public void Output(CodeBuilder cb) {
        foreach(var (blockAddr, (_, stmts, _)) in Blocks.OrderBy(x => x.Key)) {
            cb += $"{(KnownFunctions.Contains(blockAddr) ? "function" : "block")} 0x{blockAddr:X} {{";
            cb++;
            Output(cb, new StaticIRStatement.Body(stmts));
            cb--;
            cb += "}";
        }
    }

    void Output(CodeBuilder cb, StaticIRStatement stmt) {
        switch(stmt) {
            case StaticIRStatement.Body(var stmts): {
                foreach(var sub in stmts)
                    Output(cb, sub);
                break;
            }
            case StaticIRStatement.If(var cond, var then, var @else): {
                cb += $"if({Output(cond)}) {{";
                cb++;
                Output(cb, then);
                cb--;
                cb += "} else {";
                cb++;
                Output(cb, @else);
                cb--;
                cb += "}";
                break;
            }
            case StaticIRStatement.Branch(var target): {
                cb += $"goto {Output(target)};";
                break;
            }
            case LinkedBranch(var target): {
                cb += $"{Output(target)}();";
                break;
            }
            case StaticIRStatement.Dereference(var addr, var value): {
                cb += $"*({Output(value.Type)} *) ({Output(addr)}) = {Output(value)};";
                break;
            }
            case StaticIRStatement.SetField(var addr, var field, var value): {
                cb += $"({Output(addr)})->{field} = {Output(value)};";
                break;
            }
            case StaticIRStatement.SetFieldIndex(var addr, var field, var index, var value): {
                cb += $"({Output(addr)})->{field}[{index}] = {Output(value)};";
                break;
            }
            case SvcStmt(var name, var inRegs, var outRegs): {
                if(outRegs.Length == 0)
                    cb += $"svc{name}({string.Join(", ", inRegs.Select(Output))});";
                else if(outRegs.Length == 1)
                    cb += $"{Output(outRegs[0])} = svc{name}({string.Join(", ", inRegs.Select(Output))});";
                else
                    cb += $"{Output(outRegs[0])} = svc{name}({string.Join(", ", inRegs.Select(Output))}{(inRegs.Length != 0 ? ", " : "")}{string.Join(", ", outRegs.Skip(1).Select(v => $"&({Output(v)})"))});";
                break;
            }
            default:
                cb += $"/* Unhandled stmt {stmt} */";
                break;
        }
    }

    string Output(StaticIRValue expr) {
        switch(expr) {
            case StaticIRValue.Literal(var value, var type): {
                if(type == typeof(ulong))
                    return $"0x{(ulong) value:X}";
                return value.ToString();
            }
            case StaticIRValue.Named(var name, _): {
                return name;
            }
            case StaticIRValue.Add(var left, var right): {
                return $"({Output(left)}) + ({Output(right)})";
            }
            case StaticIRValue.Sub(var left, var right): {
                return $"({Output(left)}) - ({Output(right)})";
            }
            case StaticIRValue.Mul(var left, var right): {
                return $"({Output(left)}) * ({Output(right)})";
            }
            case StaticIRValue.Div(var left, var right): {
                return $"({Output(left)}) / ({Output(right)})";
            }
            case StaticIRValue.Mod(var left, var right): {
                return $"({Output(left)}) % ({Output(right)})";
            }
            case StaticIRValue.And(var left, var right): {
                return $"({Output(left)}) & ({Output(right)})";
            }
            case StaticIRValue.Or(var left, var right): {
                return $"({Output(left)}) | ({Output(right)})";
            }
            case StaticIRValue.Xor(var left, var right): {
                return $"({Output(left)}) ^ ({Output(right)})";
            }
            case StaticIRValue.LeftShift(var left, var right): {
                return $"({Output(left)}) << ({Output(right)})";
            }
            case StaticIRValue.RightShift(var left, var right): {
                return $"({Output(left)}) >> ({Output(right)})";
            }
            case StaticIRValue.Negate(var value): {
                return $"!({Output(value)})";
            }
            case StaticIRValue.Not(var value): {
                return $"~({Output(value)})";
            }
            case StaticIRValue.EQ(var left, var right): {
                return $"({Output(left)}) == {Output(right)}";
            }
            case StaticIRValue.NE(var left, var right): {
                return $"({Output(left)}) != {Output(right)}";
            }
            case StaticIRValue.LT(var left, var right): {
                return $"({Output(left)}) < {Output(right)}";
            }
            case StaticIRValue.LTE(var left, var right): {
                return $"({Output(left)}) <= {Output(right)}";
            }
            case StaticIRValue.GT(var left, var right): {
                return $"({Output(left)}) > {Output(right)}";
            }
            case StaticIRValue.GTE(var left, var right): {
                return $"({Output(left)}) >= {Output(right)}";
            }
            case StaticIRValue.Dereference(var addr, var type): {
                return $"*({Output(type)} *) ({Output(addr)})";
            }
            case StaticIRValue.GetField(var addr, var field, _): {
                return $"({Output(addr)})->{field}";
            }
            case StaticIRValue.GetFieldIndex(var addr, var field, var index, _): {
                return $"({Output(addr)})->{field}[{index}]";
            }
            case StaticIRValue.Store(var value): {
                return Output(value);
            }
            case StaticIRValue.Cast(var value, var type): {
                return $"({Output(type)}) ({Output(value)})";
            }
            default:
                return $"/* Unhandled expr {expr} */";
        }
    }

    string Output(Type type) =>
        type switch {
            var x when x == typeof(byte) => "uint8_t",
            var x when x == typeof(ushort) => "uint16_t",
            var x when x == typeof(uint) => "uint32_t",
            var x when x == typeof(ulong) => "uint64_t",
            var x when x == typeof(UInt128) => "uint128_t",
            var x when x == typeof(sbyte) => "int8_t",
            var x when x == typeof(short) => "int16_t",
            var x when x == typeof(int) => "int32_t",
            var x when x == typeof(long) => "int64_t",
            var x when x == typeof(Int128) => "int128_t",
            var x when x == typeof(bool) => "bool",
            _ => type.ToString()
        };

    protected override void Branch(ulong addr) {
        Builder.Add(new StaticIRStatement.Branch(new  StaticIRValue.Literal(addr, typeof(ulong))));
        Console.WriteLine($"Branching to {addr:X}");
        if(IsValidCodeAt(addr))
            KnownBlocks.Add(addr);
    }

    protected override void Branch(IRuntimeValue<ulong> addr) {
        Builder.Add(new StaticIRStatement.Branch(StaticBuilder<ulong>.W(addr)));
        Console.WriteLine("Branching to a runtime address!");
    }

    protected override void BranchLinked(ulong addr) {
        Console.WriteLine($"Branching with link to {addr:X}");
        Builder.Add(new LinkedBranch(new  StaticIRValue.Literal(addr, typeof(ulong))));
        if(IsValidCodeAt(addr)) {
            KnownBlocks.Add(addr);
            KnownFunctions.Add(addr);
        }
    }

    protected override void BranchLinked(IRuntimeValue<ulong> addr) {
        Console.WriteLine("Branching with link to a runtime address!");
        Builder.Add(new LinkedBranch(StaticBuilder<ulong>.W(addr)));
        // TODO: Work out symbolic execution nonsense
        // Surely this won't be that hard.
    }

    protected override void SetNZCV(IStructRef<CpuState> state, IRuntimeValue<ulong> nzcv) {
        var one = Builder.LiteralValue(1UL);
        state.NZCV_N = ((nzcv >> Builder.LiteralValue(31UL)) & one) == one;
        state.NZCV_Z = ((nzcv >> Builder.LiteralValue(30UL)) & one) == one;
        state.NZCV_C = ((nzcv >> Builder.LiteralValue(29UL)) & one) == one;
        state.NZCV_V = ((nzcv >> Builder.LiteralValue(28UL)) & one) == one;
    }

    protected override IRuntimeValue<ulong> SR(uint op0, uint op1, uint crn, uint crm, uint op2) {
        Console.WriteLine($"Soooo... trying to get an SR value. That's fun! {op0} {op1} {crn} {crm} {op2}");
        return Builder.Zero<ulong>();
    }

    protected override void SR(uint op0, uint op1, uint crn, uint crm, uint op2, IRuntimeValue<ulong> value) {
        Console.WriteLine($"Soooo... trying to set an SR value. That's fun! {op0} {op1} {crn} {crm} {op2}");
    }

    protected override void CallSvc(ulong svc) {
        Console.WriteLine($"Calling Svc 0x{svc:X}!");
        Debug.Assert(Svcs.All.ContainsKey((int) svc));
        var (name, inRegs, outRegs) = Svcs.All[(int) svc];
        Builder.Add(new SvcStmt(
            name, 
            inRegs.Select(r => ((StaticRuntimeValue<ulong>) State.X[r]).Value).ToArray(), 
            outRegs.Select(r => ((StaticRuntimeValue<ulong>) State.X[r]).Value).ToArray()));
    }
}

record LinkedBranch(StaticIRValue Address) : StaticIRStatement {
    public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
        stmtFunc(this);
        Address.Walk(valueFunc);
    }
}

record SvcStmt(string Name, StaticIRValue[] InRegs, StaticIRValue[] OutRegs) : StaticIRStatement {
    public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) => stmtFunc(this);
}