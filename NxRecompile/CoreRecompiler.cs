using System.Diagnostics;
using Aarch64Cpu;
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

    public CoreRecompiler(ExeLoader loader) {
        ExeLoader = loader;
        Coverage = loader.ExeModules.Select(mod => {
            var size = mod.TextEnd - mod.TextStart;
            size = size % 4 != 0 ? size / 4 + 1 : size / 4; // To instructions
            size = size % 64 != 0 ? size / 64 + 1 : size / 64; // To bits
            return new ulong[size];
        }).ToArray();
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

    public string Disassemble(ulong addr) => Disassembler.Disassemble(Fetch(addr), addr);
    public bool IsValidCodeAt(ulong addr) => Disassemble(addr) != null;

    void Recompile(ulong addr) {
        if(Seen.Contains(addr)) return;
        RecompileQueue.Enqueue(addr);
        Seen.Add(addr);
    }

    public void Recompile() {
        Recompile(ExeLoader.EntryPoint);
        while(RecompileQueue.TryDequeue(out var addr))
            RecompileBlock(addr);
        CheckCoverage();
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
        if(!IsValidCodeAt(addr)) return;

        Builder = KnownBlocks[addr] = new StaticBuilder<ulong>();
        PC = addr;
        State = new StaticStructRef<ulong, CpuState>(Builder, new StaticRuntimeValue<ulong>(new StaticIRValue.Literal(0UL, typeof(ulong))));
        while(true) {
            var insn = Fetch(PC);
            var dasm = Disassembler.Disassemble(insn, PC);
            if(dasm == null) // TODO: What do we do when we reach an invalid instruction mid-block...?
                break;
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
        Console.WriteLine($"Branching to {addr:X}");
        Recompile(addr);
    }

    protected override void Branch(IRuntimeValue<ulong> addr) {
        Branched = true;
        Console.WriteLine("Branching to a runtime address!");
    }

    protected override void BranchLinked(ulong addr) {
        Console.WriteLine($"Branching with link to {addr:X}");
        Recompile(addr);
        KnownFunctions.Add(addr);
    }

    protected override void BranchLinked(IRuntimeValue<ulong> addr) {
        Console.WriteLine("Branching with link to a runtime address!");
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

    protected override void CallSvc(ulong svc) {
        Console.WriteLine($"Calling Svc 0x{svc:X}!");
    }
}