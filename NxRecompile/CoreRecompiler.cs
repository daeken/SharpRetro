using System.Diagnostics;
using Aarch64Cpu;
using CoreArchCompiler;
using JitBase;
using StaticRecompilerBase;

namespace NxRecompile;

public class CoreRecompiler : Recompiler {
    public readonly ExeLoader ExeLoader;

    readonly HashSet<ulong> Seen = [];
    readonly Queue<ulong> RecompileQueue = [];
    readonly Dictionary<ulong, StaticBuilder<ulong>> KnownBlocks = [];
    readonly HashSet<ulong> KnownFunctions = [];
    readonly ulong[][] Coverage; // For each module, an instruction-level coverage map

    ulong PC;
    StaticBuilder<ulong> Builder;
    bool Branched = false;
    StaticStructRef<ulong, CpuState> State;

    ulong LinearScanAddr = 0;
    int LinearScanModule = 0;

    public CoreRecompiler(ExeLoader loader) {
        ExeLoader = loader;
        Coverage = loader.ExeModules.Select(mod => {
            var size = mod.TextEnd - mod.TextStart;
            size = size % 4 != 0 ? size / 4 + 1 : size / 4; // To instructions
            size = size % 64 != 0 ? size / 64 + 1 : size / 64; // To bits
            return new ulong[size];
        }).ToArray();
    }

    bool HaveCovered(ulong addr) {
        if(addr < ExeLoader.InitialLoadBase || addr > ExeLoader.InitialLoadBase + 0x1_0000_0000UL * (ulong) ExeLoader.ExeModules.Count)
            throw new ArgumentOutOfRangeException(nameof(addr));
        var modOff = (int) ((addr - ExeLoader.InitialLoadBase) >> 32);
        var module = ExeLoader.ExeModules[modOff];
        Debug.Assert(addr >= module.TextStart && addr < module.TextEnd);
        Debug.Assert((addr & 3) == 0); // Instructions must be 4-byte aligned
        var taddr = addr - module.TextStart;
        taddr >>= 2;
        var shift = taddr & 0x3F;
        taddr >>= 6;
        return (Coverage[modOff][taddr] & (1UL << (int) shift)) != 0;
    }

    public uint Fetch(ulong addr) {
        if(addr < ExeLoader.InitialLoadBase || addr > ExeLoader.InitialLoadBase + 0x1_0000_0000UL * (ulong) ExeLoader.ExeModules.Count)
            throw new ArgumentOutOfRangeException(nameof(addr));
        var modOff = (int) ((addr - ExeLoader.InitialLoadBase) >> 32);
        var module = ExeLoader.ExeModules[modOff];
        if(addr >= module.TextStart && addr < module.TextEnd) {
            Debug.Assert((addr & 3) == 0); // Instructions must be 4-byte aligned
            var taddr = addr - module.TextStart;
            taddr >>= 2;
            var shift = taddr & 0x3F;
            taddr >>= 6;
            Coverage[modOff][taddr] |= 1UL << (int) shift;
        }
        return module.Load<uint>(addr);
    }

    public string Disassemble(ulong addr, uint? insn = null) {
        try {
            return Disassembler.Disassemble(insn ?? Fetch(addr), addr);
        } catch { // We want a blanket catch here; if disassembly fails for any reason, it's a bad insn
            return null;
        }
    }

    public bool IsValidCodeAt(ulong addr) => Disassemble(addr) != null;

    void Recompile(ulong addr) {
        Console.WriteLine($"Attempting to queue recompile {addr:X}");
        if(Seen.Contains(addr)) return;
        RecompileQueue.Enqueue(addr);
        Seen.Add(addr);
    }

    public void Recompile() {
        KnownFunctions.Add(ExeLoader.EntryPoint);
        Recompile(ExeLoader.EntryPoint);
        while(true) {
            while(RecompileQueue.TryDequeue(out var addr))
                RecompileBlock(addr);
            CheckCoverage();
            if(!LinearScan())
                break;
        }
        Console.WriteLine($"Found {KnownFunctions.Count} functions, {KnownBlocks.Count} blocks");
    }

    public void CleanupIR() {
    }

    public void Output(CodeBuilder cb) {
        foreach(var (blockAddr, block) in KnownBlocks.OrderBy(x => x.Key)) {
            cb += $"{(KnownFunctions.Contains(blockAddr) ? "function" : "block")} 0x{blockAddr:X} {{";
            cb++;
            var body = block.Body;
            Output(cb, body);
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
            var x when x == typeof(sbyte) => "int8_t",
            var x when x == typeof(short) => "int16_t",
            var x when x == typeof(int) => "int32_t",
            var x when x == typeof(long) => "int64_t",
            var x when x == typeof(bool) => "bool",
            _ => type.ToString()
        };

    bool LinearScan() {
        if(LinearScanModule == ExeLoader.ExeModules.Count) return false;
        
        var module = ExeLoader.ExeModules[LinearScanModule];
        if(LinearScanAddr == 0) LinearScanAddr = module.TextStart;

        while(LinearScanAddr < module.TextEnd) {
            if(HaveCovered(LinearScanAddr) || !IsValidCodeAt(LinearScanAddr)) {
                LinearScanAddr += 4;
                continue;
            }
            Recompile(LinearScanAddr);
            LinearScanAddr += 4;
            break;
        }

        if(LinearScanAddr >= module.TextEnd) {
            LinearScanModule++;
            LinearScanAddr = 0;
        }

        return true;
    }

    void CheckCoverage() {
        foreach(var module in Coverage) {
            var totalBits = module.Length * 64;
            var seenBits = 0;
            foreach(var val in module)
                for(var i = 0; i < 64; i++)
                    seenBits += (int) ((val >> i) & 1);
            Console.WriteLine($"Seen {seenBits}/{totalBits} instructions");
        }
    }

    public void RecompileBlock(ulong addr) {
        if(!IsValidCodeAt(addr)) {
            if(KnownFunctions.Contains(addr)) {
                Console.WriteLine($"Uh oh! {addr:X} {Fetch(addr):X}");
                throw new NotImplementedException();
            }
            return;
        }

        Builder = KnownBlocks[addr] = new StaticBuilder<ulong>();
        PC = addr;
        State = new StaticStructRef<ulong, CpuState>(Builder, new StaticRuntimeValue<ulong>(new StaticIRValue.Named("State", typeof(ulong))));
        while(true) {
            var insn = Fetch(PC);
            var dasm = Disassemble(PC, insn);
            if(dasm == null) // TODO: What do we do when we reach an invalid instruction mid-block...?
                break; // We should probably assume this is a no-return kind of thing, if it's after a BLR...
            Console.WriteLine($"{PC:X}: {dasm}");
            Branched = false;
            if(!RecompileOne(Builder, State, insn, PC)) {
                // This should never be hit, because if this fails, then the dasm should've failed too...
                break;
            }

            if(Branched) break;
            PC += 4;
        }
    }

    protected override void Branch(ulong addr) {
        Branched = true;
        Builder.Add(new StaticIRStatement.Branch(new  StaticIRValue.Literal(addr, typeof(ulong))));
        Console.WriteLine($"Branching to {addr:X}");
        Recompile(addr);
    }

    protected override void Branch(IRuntimeValue<ulong> addr) {
        Branched = true;
        Builder.Add(new StaticIRStatement.Branch(StaticBuilder<ulong>.W(addr)));
        Console.WriteLine("Branching to a runtime address!");
    }

    protected override void BranchLinked(ulong addr) {
        Console.WriteLine($"Branching with link to {addr:X}");
        State.X30 = new StaticRuntimeValue<ulong>(new StaticIRValue.Literal(PC + 4, typeof(ulong)));
        Builder.Add(new StaticIRStatement.Branch(new  StaticIRValue.Literal(addr, typeof(ulong))));
        Recompile(addr);
        KnownFunctions.Add(addr);
    }

    protected override void BranchLinked(IRuntimeValue<ulong> addr) {
        Console.WriteLine("Branching with link to a runtime address!");
        State.X30 = new StaticRuntimeValue<ulong>(new StaticIRValue.Literal(PC + 4, typeof(ulong)));
        Builder.Add(new StaticIRStatement.Branch(StaticBuilder<ulong>.W(addr)));
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

record SvcStmt(string Name, StaticIRValue[] InRegs, StaticIRValue[] OutRegs) : StaticIRStatement;