using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
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
    Dictionary<ulong, BlockGraph> WholeBlockGraph;
    
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

    public bool IsRoDataAddr(ulong addr) => ExeLoader.ExeModules.Any(module => module.RoStart <= addr && addr < module.RoEnd);

    public void Recompile() {
        KnownBlocks.Add(ExeLoader.EntryPoint);
        KnownFunctions.Add(ExeLoader.EntryPoint); // TODO: Should we be calling the ep a known *function*?
        LinearScan();
        WholeBlockGraph = BuildBlockGraph();
        while(true) {
            if(!FoldConstants() && !ResolveRoData())
                break;
        }
    }

    static bool IsSigned(Type type) =>
        type switch {
            _ when type == typeof(sbyte) => true,
            _ when type == typeof(short) => true,
            _ when type == typeof(int) => true,
            _ when type == typeof(long) => true,
            _ when type == typeof(Int128) => true,
            _ => false
        };
    static object ToSigned(object obj) =>
        IsSigned(obj.GetType()) ? obj : obj switch {
            byte v => (object) unchecked((sbyte) v),
            ushort v => unchecked((short) v),
            uint v => unchecked((int) v),
            ulong v => unchecked((long) v),
            UInt128 v => unchecked((Int128) v),
            _ => throw new NotImplementedException($"Can't make signed value for {obj.GetType()}")
        };
    static object ToUnsigned(object obj) =>
        !IsSigned(obj.GetType()) ? obj : obj switch {
            sbyte v => (object) unchecked((byte) v),
            short v => unchecked((ushort) v),
            int v => unchecked((uint) v),
            long v => unchecked((ulong) v),
            Int128 v => unchecked((UInt128) v),
            _ => throw new NotImplementedException($"Can't make unsigned value for {obj.GetType()}")
        };
    static object Cast(object value, Type castTo) {
        if(IsSigned(value.GetType()) != IsSigned(castTo))
            value = IsSigned(castTo)
                ? ToSigned(value)
                : ToUnsigned(value);
        if(value.GetType() == castTo) return value;
        if(Marshal.SizeOf(castTo) > Marshal.SizeOf(value.GetType()))
            return Convert.ChangeType(value, castTo);
        var temp = (ulong) Convert.ChangeType(value, typeof(ulong));
        var mask = (1UL << (Marshal.SizeOf(castTo) * 8)) - 1;
        return Convert.ChangeType(temp & mask, castTo);
    }

    static (Type Type, object Value)? GetConstant(StaticIRValue value) =>
        value switch {
            StaticIRValue.Literal(var lit, var type) => (type, lit),
            StaticIRValue.Cast(var lval, var type) when GetConstant(lval) is var (_, oval) => (type, Cast(oval, type)),
            _ => null
        };

    static T DoBinaryOp<T>(StaticIRValue bin, T left, T right) where T : IBinaryNumber<T> =>
        bin switch {
            StaticIRValue.Add => left + right,
            StaticIRValue.Sub => left + right,
            StaticIRValue.And => left + right,
            StaticIRValue.Or => left + right,
            StaticIRValue.Xor => left + right,
            StaticIRValue.LeftShift => left switch {
                byte v => (T) (object) (byte) (v << (int) Convert.ChangeType(right, typeof(int))),
                ushort v => (T) (object) (ushort) (v << (int) Convert.ChangeType(right, typeof(int))),
                uint v => (T) (object) (v << (int) Convert.ChangeType(right, typeof(int))),
                ulong v => (T) (object) (v << (int) Convert.ChangeType(right, typeof(int))),
                _ => throw new NotImplementedException($"Attempted left shift on {bin} -- {left} and {right}")
            },
            StaticIRValue.RightShift => left switch {
                byte v => (T) (object) (byte) (v >> (int) Convert.ChangeType(right, typeof(int))),
                ushort v => (T) (object) (ushort) (v >> (int) Convert.ChangeType(right, typeof(int))),
                uint v => (T) (object) (v >> (int) Convert.ChangeType(right, typeof(int))),
                ulong v => (T) (object) (v >> (int) Convert.ChangeType(right, typeof(int))),
                _ => throw new NotImplementedException($"Attempted right shift on {bin} -- {left} and {right}")
            },
            _ => throw new NotImplementedException($"Attempted {bin} on {left} and {right}")
        };

    bool FoldConstants() {
        var foldedAny = false;
        foreach(var node in WholeBlockGraph.Values) {
            var block = node.Block;
            var body = block.Body;
            try {
                var folded = false;
                do {
                    folded = false;
                    var regs = new StaticIRValue[32];
                    for(var i = 0; i < body.Count; ++i) {
                        var stmt = body[i];
                        if(stmt is SvcStmt(_, _, var outRegs)) {
                            foreach(var outReg in outRegs)
                                if(outReg is StaticIRValue.GetFieldIndex(_, _, var regIndex, _))
                                    regs[regIndex] = null; // Can't know the results of svc calls
                            continue;
                        }

                        if(stmt is not StaticIRStatement.SetFieldIndex(StaticIRValue.Named("State", _) ptr, "X", var reg
                           , var val)) continue;

                        StaticIRValue FoldBinary(StaticIRValue bin, StaticIRValue left, StaticIRValue right) {
                            if(GetConstant(left) is not var (leftType, leftValue) ||
                               GetConstant(right) is not var (rightType, rightValue)) return null;

                            if(leftType != rightType) return null;
                            var nlit = leftType switch {
                                { } x when x == typeof(byte) => DoBinaryOp(bin, (byte) leftValue, (byte) rightValue),
                                { } x when x == typeof(ushort) => DoBinaryOp(bin, (ushort) leftValue,
                                    (ushort) rightValue),
                                { } x when x == typeof(uint) => DoBinaryOp(bin, (uint) leftValue, (uint) rightValue),
                                { } x when x == typeof(ulong) => DoBinaryOp(bin, (ulong) leftValue, (ulong) rightValue),
                                _ => (object) null
                            };
                            if(nlit != null) return new StaticIRValue.Literal(nlit, leftType);
                            return null;
                        }

                        var foldedVal = val.Transform(x => {
                            return x switch {
                                StaticIRValue.Add(var left, var right) => FoldBinary(x, left, right),
                                StaticIRValue.Sub(var left, var right) => FoldBinary(x, left, right),
                                StaticIRValue.And(var left, var right) => FoldBinary(x, left, right),
                                StaticIRValue.Or(var left, var right) => FoldBinary(x, left, right),
                                StaticIRValue.Xor(var left, var right) => FoldBinary(x, left, right),
                                StaticIRValue.LeftShift(var left, var right) => FoldBinary(x, left, right),
                                StaticIRValue.RightShift(var left, var right) => FoldBinary(x, left, right),
                                StaticIRValue.GetFieldIndex(StaticIRValue.Named("State", _), "X", var regIndex, _) =>
                                    regs[regIndex] switch {
                                        StaticIRValue.Literal lit => lit,
                                        _ => null
                                    },
                                _ => null
                            };
                        });
                        if(foldedVal != null && !ReferenceEquals(foldedVal, val)) {
                            folded = true;
                            foldedAny = true;
                            body[i] = new StaticIRStatement.SetFieldIndex(ptr, "X", reg, foldedVal);
                        }

                        regs[reg] = val;
                    }
                } while(folded);
            } catch(Exception e) {
                Console.WriteLine($"Constant folding failed for block 0x{block.Start:X}");
                Console.WriteLine(e);
            }
        }
        return foldedAny;
    }

    bool ResolveRoData() {
        var foundData = false;
        foreach(var node in WholeBlockGraph.Values) {
            var block = node.Block;
            var body = block.Body;
            for(var i = 0; i < body.Count; ++i) {
                var stmt = body[i];
                var nstmt = stmt.TransformValues(x => {
                    if(x is StaticIRValue.Dereference(var pointer, var type) && GetConstant(pointer) is var (ptrType, _ptrValue)) {
                        if(ptrType != typeof(ulong)) {
                            Console.WriteLine($"Weird -- pointer has non-ulong type...? {x}");
                            return null;
                        }
                        var ptrValue = (ulong) _ptrValue;
                        if(IsRoDataAddr(ptrValue)) {
                            Console.WriteLine($"Ptr to rodata! 0x{ptrValue:X}");
                        }
                    }
                    return null;
                });
                if(nstmt != null && stmt != nstmt) {
                    foundData = true;
                    body[i] = nstmt;
                }
            }
        }
        return foundData;
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

    Dictionary<ulong, BlockGraph> BuildBlockGraph() {
        var blocks = BuildBlockList();
        var blockGraphs = new Dictionary<ulong, BlockGraph>();
        foreach(var (addr, (_, stmts, leadsTo)) in blocks) {
            var adjLeadsTo = leadsTo.Where(x => !KnownFunctions.Contains(x)).ToList();
            var block = new Block(addr, stmts);
            blockGraphs[addr] = adjLeadsTo.Count switch {
                0 => new BlockGraph.End(block),
                1 => new BlockGraph.Unconditional(block),
                2 => new BlockGraph.Conditional(block),
                _ => throw new NotSupportedException($"WTF? 0x{addr:X} leads to {leadsTo.Count} total, {adjLeadsTo.Count} adjusted leads to {blocks.Count}"),
            };
        }

        foreach(var (addr, (_, _, leadsTo)) in blocks) {
            var node = blockGraphs[addr];
            var adjLeadsTo = leadsTo.Where(x => !KnownFunctions.Contains(x)).ToList();
            switch(adjLeadsTo) {
                case [var next]: {
                    ((BlockGraph.Unconditional) node).Next = blockGraphs[next];
                    break;
                }
                case [var taken, var not]: {
                    var cnode = (BlockGraph.Conditional) node;
                    cnode.Taken = blockGraphs[taken];
                    cnode.Not = blockGraphs[not];
                    break;
                }
            }
        }

        return BlockGraph.Reduce(blockGraphs, KnownFunctions);
    }
    
    Dictionary<ulong, (ulong End, List<StaticIRStatement> Statements, List<ulong> LeadsTo)> BuildBlockList() {
        var blocks = new Dictionary<ulong, (ulong End, List<StaticIRStatement> Statements, List<ulong> LeadsTo)>();
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
                    if(curBlock != addr) {
                        if(insn != null && (statements.Count == 0 || !DoesBranch([statements.Last()])))
                            statements.Add(new StaticIRStatement.Branch(new StaticIRValue.Literal(addr, typeof(ulong))));
                        blocks[curBlock] = (addr, statements, LeadingTo());
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
            if(statements.Count > 0) blocks[curBlock] = (addr, statements, LeadingTo());
        }
        return blocks;
    }

    public void CleanupIR() {
    }

    public void Output(CodeBuilder cb) {
        foreach(var (blockAddr, node) in WholeBlockGraph.OrderBy(x => x.Key)) {
            cb += $"{(KnownFunctions.Contains(blockAddr) ? "function" : "block")} 0x{blockAddr:X} {{";
            cb++;
            cb += $"/**** {node} ****/";
            Output(cb, new StaticIRStatement.Body(node.Block.Body));
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

    protected override void Breakpoint(uint imm) => Builder.Add(new BreakpointStmt(imm));
}

record LinkedBranch(StaticIRValue Address) : StaticIRStatement {
    public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
        stmtFunc(this);
        Address.Walk(valueFunc);
    }

    public override StaticIRStatement Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc, Func<StaticIRValue, StaticIRValue> valueFunc) {
        var address = Address.Transform(valueFunc);
        var nthis = address != null && !ReferenceEquals(address, Address)
            ? new LinkedBranch(address)
            : this;
        return stmtFunc(nthis) ?? nthis;
    }
}

record SvcStmt(string Name, StaticIRValue[] InRegs, StaticIRValue[] OutRegs) : StaticIRStatement {
    public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) => stmtFunc(this);

    public override StaticIRStatement Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc,
        Func<StaticIRValue, StaticIRValue> valueFunc) => stmtFunc(this);
}

record BreakpointStmt(uint Imm) : StaticIRStatement {
    public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) => stmtFunc(this);

    public override StaticIRStatement Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc,
        Func<StaticIRValue, StaticIRValue> valueFunc) => stmtFunc(this);
}