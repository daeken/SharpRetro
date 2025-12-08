using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using Aarch64Cpu;
using CoreArchCompiler;
using JitBase;
using StaticRecompilerBase;

namespace NxRecompile;

public partial class CoreRecompiler : Recompiler {
    readonly ExeLoader ExeLoader;

    List<StaticIRStatement>[][] AllInstructions;
    readonly HashSet<ulong> KnownBlocks = [];
    readonly HashSet<ulong> KnownFunctions = [];
    readonly Dictionary<ulong, ulong> ProbablePadding = [];
    readonly Dictionary<ulong, (List<StaticIRValue.Named> In, List<StaticIRValue.Named> Out)> FunctionSignatures = [];
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
        Console.WriteLine("Doing linear scan");
        LinearScan();
        Console.WriteLine("Rewriting stores");
        RewriteStores();
        Console.WriteLine("Ditching X31");
        DitchX31();
        Console.WriteLine("Building block graph");
        WholeBlockGraph = BuildBlockGraph();
        Console.WriteLine("Finding padding");
        FindPadding();
        //DumpDotGraph(0x7100005680);
        //Console.WriteLine("Reducing graph");
        //WholeBlockGraph = BlockGraph.Reduce(WholeBlockGraph, KnownFunctions);
        //DumpDotGraph(0x7100005680);
        Console.WriteLine("Folding");
        //while(FoldConstants() || RemoveRedundancies() || ResolveRoData()) {}
        /*Console.WriteLine("Rewriting functions");
        RewriteFunctions();
        Console.WriteLine("Folding");
        while(FoldConstants() || RemoveRedundancies() || ResolveRoData()) {}
        Console.WriteLine("Unregistering");
        Unregister();
        Console.WriteLine("Optimizing");
        SsaOpt();*/
        //FindSignatures();

        foreach(var (addr, taddr) in ProbablePadding.ToDictionary())
            if(!WholeBlockGraph.ContainsKey(taddr))
                ProbablePadding.Remove(addr);
        foreach(var (addr, taddr) in ProbablePadding)
            if(WholeBlockGraph.ContainsKey(taddr) || ProbablePadding.ContainsKey(taddr))
                WholeBlockGraph.Remove(addr);
    }

    void FindPadding() {
        foreach(var (addr, node) in WholeBlockGraph) {
            if(node.Block.Body.Count != 1 || 
               node.Block.Body[0] is not StaticIRStatement.Branch(var target) ||
               !IsConstant(target)
            ) continue;
            var taddr = (ulong) GetConstant(target)!.Value.Value;
            if(addr != taddr && WholeBlockGraph.ContainsKey(taddr))
                ProbablePadding[addr] = taddr;
        }
    }

    StaticIRStatement ReduceSink(StaticIRValue inv) {
        var needed = new List<StaticIRValue>();
        inv.Walk(value => {
            switch(value) {
                case StaticIRValue.GetField(StaticIRValue.Named("State", _), _, _):
                case StaticIRValue.GetFieldIndex(StaticIRValue.Named("State", _), _, _, _):
                    break;
                case StaticIRValue.GetField:
                case StaticIRValue.GetFieldIndex:
                case StaticIRValue.Dereference:
                    needed.Add(value);
                    break;
            }
        });
        return new StaticIRStatement.Body(
            needed.Select(StaticIRStatement (x) => new StaticIRStatement.Sink(x)).ToList());
    }

    void DitchX31() =>
        AllInstructions = 
            AllInstructions.Select(module =>
                module.Select(insn =>
                    insn == null ? null
                    : ((StaticIRStatement.Body) new StaticIRStatement.Body(insn)
                        .Transform(stmt => 
                            stmt is StaticIRStatement.SetFieldIndex(StaticIRValue.Named("State", _), "X", 31, var value)
                                ? ReduceSink(value)
                                : stmt)).Stmts.ToList()).ToArray()).ToArray();

    void DumpDotGraph(ulong addr) {
        var seen = new HashSet<ulong>();
        void Dump(BlockGraph node) {
            if(!seen.Add(node.Block.Start)) return;
            Console.WriteLine($"\"0x{node.Block.Start:X}\" [label=\"0x{node.Block.Start:X} - {node.GetType().Name}\"];");
            switch(node) {
                case BlockGraph.Unconditional n: {
                    Dump(n.Next);
                    Console.WriteLine($"\"0x{node.Block.Start:X}\" -> \"0x{n.Next.Block.Start:X}\";");
                    break;
                }
                case BlockGraph.Conditional n: {
                    Dump(n.Taken);
                    Dump(n.Not);
                    Console.WriteLine($"\"0x{node.Block.Start:X}\" -> \"0x{n.Taken.Block.Start:X}\" [color=green];");
                    Console.WriteLine($"\"0x{node.Block.Start:X}\" -> \"0x{n.Not.Block.Start:X}\" [color=red];");
                    break;
                }
                case BlockGraph.When n: {
                    Dump(n.Then);
                    Dump(n.Next);
                    Console.WriteLine($"\"0x{node.Block.Start:X}\" -> \"0x{n.Then.Block.Start:X}\" [color=green];");
                    Console.WriteLine($"\"0x{node.Block.Start:X}\" -> \"0x{n.Next.Block.Start:X}\" [color=red];");
                    break;
                }
                case BlockGraph.Unless n: {
                    Dump(n.Then);
                    Dump(n.Next);
                    Console.WriteLine($"\"0x{node.Block.Start:X}\" -> \"0x{n.Then.Block.Start:X}\" [color=green];");
                    Console.WriteLine($"\"0x{node.Block.Start:X}\" -> \"0x{n.Next.Block.Start:X}\" [color=red];");
                    break;
                }
                case BlockGraph.If n: {
                    Dump(n.Then);
                    Dump(n.Else);
                    Console.WriteLine($"\"0x{node.Block.Start:X}\" -> \"0x{n.Then.Block.Start:X}\" [color=green];");
                    Console.WriteLine($"\"0x{node.Block.Start:X}\" -> \"0x{n.Else.Block.Start:X}\" [color=red];");
                    break;
                }
                case BlockGraph.TerminalIf n: {
                    Dump(n.Then);
                    Dump(n.Else);
                    Console.WriteLine($"\"0x{node.Block.Start:X}\" -> \"0x{n.Then.Block.Start:X}\" [color=green];");
                    Console.WriteLine($"\"0x{node.Block.Start:X}\" -> \"0x{n.Else.Block.Start:X}\" [color=red];");
                    break;
                }
                case BlockGraph.DoWhile n: {
                    Dump(n.Loop);
                    Dump(n.Next);
                    Console.WriteLine($"\"0x{node.Block.Start:X}\" -> \"0x{n.Loop.Block.Start:X}\" [color=green];");
                    Console.WriteLine($"\"0x{node.Block.Start:X}\" -> \"0x{n.Next.Block.Start:X}\" [color=blue];");
                    break;
                }
                case BlockGraph.InverseDoWhile n: {
                    Dump(n.Loop);
                    Dump(n.Next);
                    Console.WriteLine($"\"0x{node.Block.Start:X}\" -> \"0x{n.Loop.Block.Start:X}\" [color=green];");
                    Console.WriteLine($"\"0x{node.Block.Start:X}\" -> \"0x{n.Next.Block.Start:X}\" [color=blue];");
                    break;
                }
                case BlockGraph.End:
                    break;
                default:
                    throw new NotImplementedException($"Can't output dot for {node}");
            }
        }
        Console.WriteLine("digraph G {");
        Dump(WholeBlockGraph[addr]);
        Console.WriteLine("}");
    }

    static StaticIRValue CombineMasks(StaticIRValue a, StaticIRValue b) =>
        FoldBinary(new StaticIRValue.And(a, b), a, b);

    StaticIRValue RemoveRedundancy(StaticIRValue value) => value switch {
        StaticIRValue.Add(var left, var right) when IsZero(right) => left,
        StaticIRValue.Add(var left, var right) when IsZero(left) => right,
        StaticIRValue.Sub(var left, var right) when IsZero(right) => left,
        StaticIRValue.And(var left, _) when IsZero(left) => left,
        StaticIRValue.And(_, var right) when IsZero(right) => right,
        StaticIRValue.And(var left, var right) when IsAllOnes(right) => left,
        StaticIRValue.And(var left, var right) when IsAllOnes(left) => right,
        StaticIRValue.And(StaticIRValue.And(var left, var middle), var right)
            when IsConstant(middle) && IsConstant(right)
            => new StaticIRValue.And(left, CombineMasks(middle, right)),
        StaticIRValue.And(StaticIRValue.And(var left, var middle), var right)
            when IsConstant(left) && IsConstant(right)
            => new StaticIRValue.And(middle, CombineMasks(left, right)),
        StaticIRValue.And(StaticIRValue.And(var left, var middle), var right)
                when IsConstant(left) && IsConstant(middle)
            => new StaticIRValue.And(right, CombineMasks(left, middle)),
        StaticIRValue.And(var left, StaticIRValue.And(var middle, var right))
                when IsConstant(middle) && IsConstant(right)
            => new StaticIRValue.And(left, CombineMasks(middle, right)),
        StaticIRValue.And(var left, StaticIRValue.And(var middle, var right))
                when IsConstant(left) && IsConstant(right)
            => new StaticIRValue.And(middle, CombineMasks(left, right)),
        StaticIRValue.And(var left, StaticIRValue.And(var middle, var right))
                when IsConstant(left) && IsConstant(middle)
            => new StaticIRValue.And(right, CombineMasks(left, middle)),
        StaticIRValue.Or(var left, var right) when IsZero(right) => left,
        StaticIRValue.Or(var left, var right) when IsZero(left) => right,
        StaticIRValue.Xor(var left, var right) when IsZero(right) => left,
        StaticIRValue.Xor(var left, var right) when IsZero(left) => right,
        StaticIRValue.LeftShift(var left, var right) when IsZero(right) => left,
        StaticIRValue.LeftShift(var left, var right)
                when GetInt(right) is {} lshift && lshift >= Marshal.SizeOf(left.Type) * 8 =>
            new StaticIRValue.Literal(Activator.CreateInstance(left.Type), left.Type),
        StaticIRValue.RightShift(var left, var right)
                when GetInt(right) is {} lshift && lshift >= Marshal.SizeOf(left.Type) * 8 =>
            new StaticIRValue.Literal(Activator.CreateInstance(left.Type), left.Type),
        StaticIRValue.RightShift(var left, var right) when IsZero(right) => left,
        StaticIRValue.Cast(var val, var type) when !type.IsConstructedGenericType && GetConstant(val) is var (_, cval) =>
            new StaticIRValue.Literal(Cast(cval, type), type),
        StaticIRValue.Not(StaticIRValue.Not(var val)) => val,
        _ => null,
    };

    bool RemoveRedundancies() {
        var didAny = false;
        foreach(var node in WholeBlockGraph.Values) {
            var block = node.Block;
            var body = block.Body;
            var did = false;
            var didSome = false;
            do {
                did = false;
                body = body.TransformValues(value => {
                    try {
                        var rep = RemoveRedundancy(value);
                        if(rep != null && !ReferenceEquals(value, rep))
                            did = didSome = didAny = true;
                        return rep;
                    } catch(Exception e) {
                        Console.WriteLine($"Removing redundancies failed for block 0x{block.Start:X}");
                        Console.WriteLine(value);
                        Console.WriteLine(e);
                        return null;
                    }
                });
            } while(did);
            if(didSome)
                node.Block = node.Block with { Body = body };
        }
        return didAny;
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
                        if(!IsRoDataAddr(ptrValue)) return null;
                        var nval = type switch {
                            _ when type == typeof(byte) => (object) ExeLoader.Load<byte>(ptrValue),
                            _ when type == typeof(ushort) => ExeLoader.Load<ushort>(ptrValue),
                            _ when type == typeof(uint) => ExeLoader.Load<uint>(ptrValue),
                            _ when type == typeof(ulong) => ExeLoader.Load<ulong>(ptrValue),
                            _ when type == typeof(sbyte) => ExeLoader.Load<sbyte>(ptrValue),
                            _ when type == typeof(short) => ExeLoader.Load<short>(ptrValue),
                            _ when type == typeof(int) => ExeLoader.Load<int>(ptrValue),
                            _ when type == typeof(long) => ExeLoader.Load<long>(ptrValue),
                            _ => null
                        };
                        if(nval != null) return new StaticIRValue.Literal(nval, type);
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
            Builder = new StaticBuilder<ulong>();
            State = new StaticStructRef<ulong, CpuState>(Builder, new StaticRuntimeValue<ulong>(new StaticIRValue.Named("State", typeof(ulong))));
            for(var addr = module.TextStart; addr <= module.TextEnd; addr += 4, ++i) {
                PC = addr;
                var insn = ExeLoader.Load<uint>(PC);
                /*var dasm = Disassemble(PC, insn);
                if(dasm == null) {
                    Console.WriteLine($"{PC:X}: Invalid instruction {insn & 0xFF:X02} {(insn >> 8) & 0xFF:X02} {(insn >> 16) & 0xFF:X02} {(insn >> 24) & 0xFF:X02}");
                    continue;
                }
                //Console.WriteLine($"{PC:X}: {dasm}");
                //Builder.Add(new DebugStmt(PC, dasm));*/
                var success = false;
                var stmts = Builder.ScopedStatements(() => {
                    success = RecompileOne(Builder, State, insn, PC);
                });
                if(success)
                    arr[i] = stmts;
                else
                    Console.WriteLine($"{PC:X}: Invalid instruction {insn & 0xFF:X02} {(insn >> 8) & 0xFF:X02} {(insn >> 16) & 0xFF:X02} {(insn >> 24) & 0xFF:X02}");
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
        return blockGraphs;
    }
    
    Dictionary<ulong, (ulong End, List<StaticIRStatement> Statements, List<ulong> LeadsTo)> BuildBlockList() {
        var blocks = new Dictionary<ulong, (ulong End, List<StaticIRStatement> Statements, List<ulong> LeadsTo)>();
        foreach(var (arr, module) in AllInstructions.Zip(ExeLoader.ExeModules)) {
            var addr = module.TextStart;
            var curBlock = addr;
            var statements = new List<StaticIRStatement>();
            var didBranch = false;
            List<ulong> LeadingTo() {
                var leadsTo = new List<ulong>();
                new StaticIRStatement.Body(statements).Walk(x => {
                    if(x is StaticIRStatement.Branch { Address: StaticIRValue.Literal(var laddr, var type) } && type == typeof(ulong))
                        leadsTo.Add((ulong) laddr);
                });
                return leadsTo;
            }
            foreach(var insn in arr) {
                if(didBranch || insn == null || (curBlock != addr && KnownBlocks.Contains(addr))) {
                    didBranch = false;
                    if(curBlock != addr) {
                        if(addr != module.TextEnd && (statements.Count == 0 || !DoesBranch([statements.Last()])))
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
            // Infinite loop at the end
            if(statements.Count == 0 || !DoesBranch([statements.Last()]))
                statements.Add(new StaticIRStatement.Branch(new StaticIRValue.Literal(curBlock, typeof(ulong))));
            blocks[curBlock] = (addr, statements, LeadingTo());
        }
        return blocks;
    }

    protected override void Branch(ulong addr) {
        Builder.Add(new StaticIRStatement.Branch(new  StaticIRValue.Literal(addr, typeof(ulong))));
        //Console.WriteLine($"Branching to {addr:X}");
        if(IsValidCodeAt(addr))
            KnownBlocks.Add(addr);
    }

    protected override void Branch(IRuntimeValue<ulong> addr) {
        Builder.Add(new StaticIRStatement.Branch(StaticBuilder<ulong>.W(addr)));
        //Console.WriteLine("Branching to a runtime address!");
    }

    protected override void BranchLinked(ulong addr) {
        //Console.WriteLine($"Branching with link to {addr:X}");
        State.X[30] = Builder.LiteralValue(PC + 4); // need this temporarily...
        Builder.Add(new LinkedBranch(new StaticIRValue.Literal(addr, typeof(ulong))));
        if(IsValidCodeAt(addr)) {
            KnownBlocks.Add(addr);
            KnownFunctions.Add(addr);
        }
    }

    protected override void BranchLinked(IRuntimeValue<ulong> addr) {
        //Console.WriteLine("Branching with link to a runtime address!");
        State.X[30] = Builder.LiteralValue(PC + 4); // need this temporarily...
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

    protected override IRuntimeValue<ulong> SR(uint op0, uint op1, uint crn, uint crm, uint op2) =>
        (((0b10 | op0) << 14) | (op1 << 11) | (crn << 7) | (crm << 3) | op2) switch {
            0b11_011_0000_0000_111 => Builder.LiteralValue(0UL), // DCZID_EL0
            0b11_011_1101_0000_011 => 
                Builder.GetField<ulong>(
                    new StaticRuntimeValue<ulong>(new StaticIRValue.Named("State", typeof(ulong))), 
                    "TlsBase"),
            _ => new StaticRuntimeValue<ulong>(new ReadSr(op0, op1, crn, crm, op2))
        };

    protected override void SR(uint op0, uint op1, uint crn, uint crm, uint op2, IRuntimeValue<ulong> value) =>
        Builder.Add(new WriteSrStmt(op0, op1, crn, crm, op2, StaticBuilder<ulong>.W(value)));

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

    protected override IRuntimeValue<uint> FloatToFixed32(IRuntimeValue<float> fvalue, int fbits) =>
        new StaticRuntimeValue<uint>(new FloatToFixed(StaticBuilder<ulong>.W(fvalue), fbits, typeof(uint)));
    protected override IRuntimeValue<uint> FloatToFixed32(IRuntimeValue<double> fvalue, int fbits) =>
        new StaticRuntimeValue<uint>(new FloatToFixed(StaticBuilder<ulong>.W(fvalue), fbits, typeof(uint)));
    protected override IRuntimeValue<ulong> FloatToFixed64(IRuntimeValue<float> fvalue, int fbits) =>
        new StaticRuntimeValue<ulong>(new FloatToFixed(StaticBuilder<ulong>.W(fvalue), fbits, typeof(ulong)));
    protected override IRuntimeValue<ulong> FloatToFixed64(IRuntimeValue<double> fvalue, int fbits) =>
        new StaticRuntimeValue<ulong>(new FloatToFixed(StaticBuilder<ulong>.W(fvalue), fbits, typeof(ulong)));

    protected override IRuntimeValue<byte> CompareAndSwap<T>(
        IRuntimePointer<ulong, T> pointer, IRuntimeValue<T> value,
        IRuntimeValue<T> comparand
    ) =>
        new StaticRuntimeValue<byte>(new CompareAndSwap(
            StaticBuilder<ulong>.W(((StaticRuntimePointer<ulong, T>) pointer).Value),
            StaticBuilder<ulong>.W(value),
            StaticBuilder<ulong>.W(comparand)
        ));
}

record LinkedBranch(StaticIRValue Address) : StaticIRStatement {
    public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
        Address.Walk(valueFunc);
        stmtFunc(this);
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
    public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
        stmtFunc(this);
        foreach(var reg in InRegs.Concat(OutRegs))
            reg.Walk(valueFunc);
    }

    public override StaticIRStatement Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc,
        Func<StaticIRValue, StaticIRValue> valueFunc) => stmtFunc(this);
}

record BreakpointStmt(uint Imm) : StaticIRStatement {
    public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) => stmtFunc(this);

    public override StaticIRStatement Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc,
        Func<StaticIRValue, StaticIRValue> valueFunc) => stmtFunc(this);
}

record ReadSr(uint Op0, uint Op1, uint Crn, uint Crm, uint Op2) : StaticIRValue(typeof(ulong)) {
    public override void Walk(Action<StaticIRValue> func) => func(this);
    public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) => func(this) ?? this;
}

record WriteSrStmt(uint Op0, uint Op1, uint Crn, uint Crm, uint Op2, StaticIRValue Value) : StaticIRStatement {
    public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) {
        Value.Walk(valueFunc);
        stmtFunc(this);
    }
    public override StaticIRStatement Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc, Func<StaticIRValue, StaticIRValue> valueFunc) {
        var value = Value.Transform(valueFunc);
        var nthis = value != null && !ReferenceEquals(value, Value)
            ? this with { Value = value }
            : this;
        return stmtFunc(nthis) ?? nthis;
    }
}

record DebugStmt(ulong PC, string Dasm) : StaticIRStatement {
    public override void Walk(Action<StaticIRStatement> stmtFunc, Action<StaticIRValue> valueFunc) => stmtFunc(this);
    public override StaticIRStatement Transform(Func<StaticIRStatement, StaticIRStatement> stmtFunc, Func<StaticIRValue, StaticIRValue> valueFunc) =>
        stmtFunc(this);
}

record FloatToFixed(StaticIRValue Value, int Fbits, Type OutType) : StaticIRValue(OutType) {
    public override void Walk(Action<StaticIRValue> func) {
        func(this);
        Value.Walk(func);
    }

    public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
        var value = Value.Transform(func);
        var nthis = value != null && !ReferenceEquals(value, Value)
            ? this with { Value = value }
            : this;
        return func(nthis) ?? nthis;
    }
}

record CompareAndSwap(StaticIRValue Pointer, StaticIRValue Value, StaticIRValue Comparand) : StaticIRValue(typeof(byte)) {
    public override void Walk(Action<StaticIRValue> func) {
        func(this);
        Pointer.Walk(func);
        Value.Walk(func);
        Comparand.Walk(func);
    }

    public override StaticIRValue Transform(Func<StaticIRValue, StaticIRValue> func) {
        var pointer = Pointer.Transform(func);
        var value = Value.Transform(func);
        var comparand = Comparand.Transform(func);
        var nthis = 
            (pointer != null && !ReferenceEquals(pointer, Pointer)) || 
            (value != null && !ReferenceEquals(value, Value)) || 
            (comparand != null && !ReferenceEquals(comparand, Comparand))
                ? this with {
                    Pointer = pointer != null && !ReferenceEquals(pointer, Pointer) ? pointer : Pointer, 
                    Value = value != null && !ReferenceEquals(value, Value) ? value : Value,
                    Comparand = comparand != null && !ReferenceEquals(comparand, Comparand) ? comparand : Comparand
                }
                : this;
        return func(nthis) ?? nthis;

    }
}
