using System.Diagnostics;
using System.Runtime.InteropServices;
using Aarch64Cpu;
using LibSharpRetro;
using NxCommon;
using NxTranslate;

void SaveLoadAll(Assembler asm, Action<Assembler> func) {
    asm.StpPreindex(R.X29, R.X30, R.SP, -256); // Save all GPRs
    asm.Stp(R.X27, R.X28, R.SP, 16 * 1);
    asm.Stp(R.X25, R.X26, R.SP, 16 * 2);
    asm.Stp(R.X23, R.X24, R.SP, 16 * 3);
    asm.Stp(R.X21, R.X22, R.SP, 16 * 4);
    asm.Stp(R.X19, R.X20, R.SP, 16 * 5);
    asm.Stp(R.X17, R.XZR, R.SP, 16 * 6);
    asm.Stp(R.X15, R.X16, R.SP, 16 * 7);
    asm.Stp(R.X13, R.X14, R.SP, 16 * 8);
    asm.Stp(R.X11, R.X12, R.SP, 16 * 9);
    asm.Stp(R.X9, R.X10, R.SP, 16 * 10);
    asm.Stp(R.X7, R.X8, R.SP, 16 * 11);
    asm.Stp(R.X5, R.X6, R.SP, 16 * 12);
    asm.Stp(R.X3, R.X4, R.SP, 16 * 13);
    asm.Stp(R.X1, R.X2, R.SP, 16 * 14);
    asm.ReadNzcv(R.X8);
    asm.Stp(R.X0, R.X8, R.SP, 16 * 15);
    asm.Mov(R.X29, R.SP);
    func(asm);
    asm.Ldp(R.X0, R.X8, R.SP, 16 * 15);
    asm.WriteNzcv(R.X8);
    asm.Ldp(R.X1, R.X2, R.SP, 16 * 14);
    asm.Ldp(R.X3, R.X4, R.SP, 16 * 13);
    asm.Ldp(R.X5, R.X6, R.SP, 16 * 12);
    asm.Ldp(R.X7, R.X8, R.SP, 16 * 11);
    asm.Ldp(R.X9, R.X10, R.SP, 16 * 10);
    asm.Ldp(R.X11, R.X12, R.SP, 16 * 9);
    asm.Ldp(R.X13, R.X14, R.SP, 16 * 8);
    asm.Ldp(R.X15, R.X16, R.SP, 16 * 7);
    asm.Ldp(R.X17, R.XZR, R.SP, 16 * 6);
    asm.Ldp(R.X19, R.X20, R.SP, 16 * 5);
    asm.Ldp(R.X21, R.X22, R.SP, 16 * 4);
    asm.Ldp(R.X23, R.X24, R.SP, 16 * 3);
    asm.Ldp(R.X25, R.X26, R.SP, 16 * 2);
    asm.Ldp(R.X27, R.X28, R.SP, 16 * 1);
    asm.LdpPostindex(R.X29, R.X30, R.SP, 256);
}

byte[] BuildTrampolines(ulong textBase, ulong trampBase, ulong trampRwBase, List<(ulong Addr, Instruction Instruction)> insns, Memory<byte> text) {
    insns = insns.OrderByDescending(x => x.Addr).ToList(); // Work back to front to maximize odds of being in range
    var asm = new Assembler(0xC000);
    foreach(var (i, (addr, insn)) in insns.Index()) {
        var pc = asm.PC;
        asm.PC = 0xC000 - (ulong) (i + 1) * 4;
        var tasm = new Assembler(4);
        tasm.B((long) (trampBase + asm.PC) - (long) (textBase + addr));
        tasm.AsBytes.CopyTo(text[(int) addr..].Span);
        asm.B((long) pc - (long) asm.PC);
        
        asm.PC = pc;
        SaveLoadAll(asm, _ => {
            asm.BlSelf();
            asm.Mov(R.X9, asm.PC + trampBase - trampRwBase);
            asm.Sub(R.X9, R.X30, R.X9);
            asm.Ldr(R.X8, R.X9); // callbacks pointer
            asm.Mov(R.X0, R.X29);
            asm.Mov(R.X4, addr);
            switch(insn) {
                case Instruction.Brk(var imm):
                    asm.Mov(R.X1, 0);
                    asm.Mov(R.X2, imm);
                    break;
                case Instruction.Svc(var svc):
                    asm.Mov(R.X1, 1);
                    asm.Mov(R.X2, svc);
                    break;
                case Instruction.Msr(var op0, var op1, var crn, var crm, var op2, var reg):
                    asm.Mov(R.X1, 2);
                    asm.Mov(R.X2, ((op0 & 0b1) << 14) | ((op1 & 0b111) << 11) | ((crn & 0b1111) << 7) | ((crm & 0b1111) << 3) | (op2 & 0b111));
                    asm.Mov(R.X3, reg);
                    break;
                case Instruction.Mrs(var op0, var op1, var crn, var crm, var op2, var reg):
                    asm.Mov(R.X1, 3);
                    asm.Mov(R.X2, ((op0 & 0b1) << 14) | ((op1 & 0b111) << 11) | ((crn & 0b1111) << 7) | ((crm & 0b1111) << 3) | (op2 & 0b111));
                    asm.Mov(R.X3, reg);
                    break;
            }
            asm.Add(R.X8, R.X8, (ushort) Marshal.OffsetOf<CallbackTableOffsets>("nativeReentry"));
            asm.Ldr(R.X8, R.X8);
            asm.Blr(R.X8);
        });
        asm.B((long) (textBase + addr + 4) - (long) (trampBase + asm.PC));
    }
    return asm.AsBytes;
}

var exeLoader = new ExeLoader(args[0], includeRtld: false, doRelocate: false);
var macho = new MachoWriter();
var loadBase = 0UL;
var modules = new List<(ulong TrampRW, ulong TrampX, ulong Start, ulong Size)>();
foreach(var (i, mod) in exeLoader.ExeModules.Index()) {
    var problematic = new List<(ulong Addr, Instruction Instruction)>();
    for(var j = mod.TextStart; j < mod.TextEnd; j += 4) {
        var insn = Instruction.Decode(mod.Load<uint>(j));
        if(insn == null) continue;
        problematic.Add((j - mod.TextStart, insn));
    }

    var x18Usage = new List<(ulong Addr, List<int> Shifts)>();
    for(var j = mod.TextStart; j < mod.TextEnd; j += 4) {
        var insn = mod.Load<uint>(j);
        var shifts = Disassembler.GetGprMasks(insn)
            .Select(x => x.Shift)
            .Where(shift => ((insn >> shift) & 0b11111) == 18)
            .ToList();
        if(shifts.Count != 0)
            x18Usage.Add((j, shifts));
    }

    var groupRange = 25UL;
    var regRange = 100UL;
    var groupedUsages = x18Usage.GroupByProximity(groupRange * 4UL, x => x.Addr).ToList();
    var safeGroups = groupedUsages.Select(group => {
        var start = Math.Max(mod.TextStart, group.MinBy(x => x.Addr).Addr - regRange * 4);
        var end = Math.Min(mod.TextEnd, group.MaxBy(x => x.Addr).Addr + regRange * 4);
        var regsUsed = new HashSet<int>();
        for(var j = start; j < end; j += 4) {
            var insn = mod.Load<uint>(j);
            var regs = Disassembler.GetGprMasks(insn)
                .Select(x => (int) ((insn >> x.Shift) & 0b11111));
            foreach(var reg in regs)
                regsUsed.Add(reg);
        }
        var safeReg = 0;
        for(var reg = 28; reg >= 19; reg--)
            if(!regsUsed.Contains(reg)) {
                safeReg = reg;
                break;
            }
        return safeReg;
    }).Where(x => x != 0).ToList();
    Console.WriteLine($"Found X18 usages: {x18Usage.Count} / {groupedUsages.Count} / {safeGroups.Count}");

    var textStart = mod.TextStart - mod.LoadBase;
    var textEnd = mod.TextEnd - mod.LoadBase;
    var roStart = mod.RoStart - mod.LoadBase;
    var roEnd = mod.RoEnd - mod.LoadBase;
    var dataStart = mod.DataStart - mod.LoadBase;
    var dataEnd = mod.DataEnd - mod.LoadBase;
    var bssStart = mod.BssStart - mod.LoadBase;
    var bssEnd = mod.BssEnd - mod.LoadBase;
    Debug.Assert(bssStart >= dataEnd);
    Debug.Assert(dataStart >= roEnd);
    Debug.Assert(roStart >= textEnd);
    dataEnd = bssEnd;
    
    if(textEnd % 0x4000 != 0) textEnd = textEnd - (textEnd % 0x4000) + 0x4000;
    if(textEnd > roStart) roStart = textEnd;
    if(textEnd > roEnd) roEnd = textEnd;
    if(roEnd % 0x4000 != 0) roEnd = roEnd - (roEnd % 0x4000) + 0x4000;
    Debug.Assert(dataStart > textEnd);
    if(dataStart % 0x4000 != 0) {
        roEnd -= 0x4000;
        dataStart = roEnd;
    }
    Debug.Assert(dataStart >= textEnd);
    Debug.Assert(roEnd >= textEnd);
    if(dataEnd % 0x4000 != 0) dataEnd = dataEnd - (dataEnd % 0x4000) + 0x4000;
    
    Console.WriteLine($".text {mod.TextStart - mod.LoadBase:X}-{mod.TextEnd - mod.LoadBase:X} -> {textStart:X}-{textEnd:X}");
    if(roStart == roEnd)
        Console.WriteLine(".rodata goes away");
    else
        Console.WriteLine($".rodata {mod.RoStart - mod.LoadBase:X}-{mod.RoEnd - mod.LoadBase:X} -> {roStart:X}-{roEnd:X}");
    Console.WriteLine($".data {mod.DataStart - mod.LoadBase:X}-{mod.BssEnd - mod.LoadBase:X} -> {dataStart:X}-{dataEnd:X}");
    
    Debug.Assert(textStart % 0x4000 == 0);
    Debug.Assert(textEnd % 0x4000 == 0);
    Debug.Assert(roStart % 0x4000 == 0);
    Debug.Assert(roEnd % 0x4000 == 0);
    Debug.Assert(dataStart % 0x4000 == 0);
    Debug.Assert(dataEnd % 0x4000 == 0);

    var tramprw = ulong.MaxValue;
    var trampx = ulong.MaxValue;
    if(problematic.Count != 0) {
        tramprw = loadBase;
        trampx = loadBase + 0x4000;
        var trampolines = BuildTrampolines(loadBase + 0x10000, loadBase + 0x4000, loadBase, problematic, mod.Binary[(int) textStart..]);
        macho.AddSegment($".tramprw_{i}", loadBase, 0x4000, [], MemoryProtection.Read | MemoryProtection.Write);
        macho.AddSegment($".trampx_{i}", loadBase + 0x4000, 0xC000, trampolines, MemoryProtection.Read | MemoryProtection.Execute);
        loadBase += 0x10000;
    }
    modules.Add((tramprw, trampx, loadBase, dataEnd - textStart));
    foreach(var symbol in mod.Symbols)
        if(symbol.Value != 0 && symbol.Name != "")
            macho.AddSymbol("_" + symbol.Name, loadBase + symbol.Value);
    macho.AddSegment($".text_{i}", 
        loadBase + textStart, textEnd - textStart, 
        mod.Binary[(int) textStart..(int) textEnd].ToArray(),
        MemoryProtection.Read | MemoryProtection.Execute);
    if(roStart != roEnd)
        macho.AddSegment($".rodata_{i}", 
            loadBase + roStart, roEnd - roStart, 
            mod.Binary[(int) roStart..(int) roEnd].ToArray(),
            MemoryProtection.Read);
    macho.AddSegment($".data_{i}", 
        loadBase + dataStart, dataEnd - dataStart, 
        mod.Binary[(int) dataStart..Math.Min((int) dataEnd, mod.Binary.Length)].ToArray(),
        MemoryProtection.Read | MemoryProtection.Write);
    loadBase += dataEnd;
}

var glue = new Assembler(0x4000);
macho.AddSymbol("_setup", loadBase + glue.PC);
glue.StpPreindex(R.X29, R.X30, R.SP, -16);
glue.Mov(R.X29, R.SP);
glue.BlSelf();
var curPos = loadBase + glue.PC;
glue.Mov(R.X19, R.X0); // Save our callbacks pointer here
glue.Mov(R.X20, curPos);
glue.Sub(R.X20, R.X30, R.X20); // Slide

foreach(var (tramprw, trampx, start, size) in modules) {
    if(tramprw != ulong.MaxValue) {
        glue.Mov(R.X0, tramprw);
        glue.Add(R.X0, R.X0, R.X20);
        glue.Str(R.X19, R.X0);
    }
    glue.Mov(R.X0, start);
    glue.Add(R.X0, R.X0, R.X20);
    glue.Mov(R.X1, size);
    glue.Add(R.X2, R.X19, (ushort) Marshal.OffsetOf<CallbackTableOffsets>("initModule"));
    glue.Ldr(R.X2, R.X2);
    glue.Blr(R.X2);
}

glue.LdpPostindex(R.X29, R.X30, R.SP, 16);
glue.Ret();
macho.AddSymbol("_runFrom", loadBase + glue.PC);
SaveLoadAll(glue, asm => {
    asm.Mov(R.X30, R.X0);
    asm.Mov(R.X8, R.X1);
    asm.Ldp(R.X0, R.X1, R.X30, 16 * 1);
    asm.Ldp(R.X2, R.X3, R.X30, 16 * 2);
    asm.Ldp(R.X4, R.X5, R.X30, 16 * 3);
    asm.Ldp(R.X6, R.X7, R.X30, 16 * 4);
    asm.Ldp(R.XZR, R.X9, R.X30, 16 * 5);
    asm.Ldp(R.X10, R.X11, R.X30, 16 * 6);
    asm.Ldp(R.X12, R.X13, R.X30, 16 * 7);
    asm.Ldp(R.X14, R.X15, R.X30, 16 * 8);
    asm.Ldp(R.X16, R.X17, R.X30, 16 * 9);
    asm.Ldp(R.XZR, R.X19, R.X30, 16 * 10);
    asm.Ldp(R.X20, R.X21, R.X30, 16 * 11);
    asm.Ldp(R.X22, R.X23, R.X30, 16 * 12);
    asm.Ldp(R.X24, R.X25, R.X30, 16 * 13);
    asm.Ldp(R.X26, R.X27, R.X30, 16 * 14);
    asm.Ldp(R.X28, R.X29, R.X30, 16 * 15);
    asm.Blr(R.X8);
});
glue.Ret();
macho.AddSegment(".glue", loadBase, glue.Size, glue.AsBytes, MemoryProtection.Read | MemoryProtection.Execute);

macho.Write(args[1]);
Sh.Run("ldid", "-S", args[1]);

abstract record Instruction {
    public static Instruction Decode(uint insn) => insn switch {
        _ when (insn & 0xFFE0001F) == 0xD4200000 =>
            new Brk((insn >> 5) & 0xFFFFU),
        _ when (insn & 0xFFE0001F) == 0xD4000001 =>
            new Svc((insn >> 5) & 0xFFFFU),
        _ when (insn & 0xFFF00000) == 0xD5100000 && (insn & 0xFFFFFFE0) != 0xd51bd040 => // Allow native TPIDR_EL0
            new Msr(
                (insn >> 19) & 0x1U,
                (insn >> 16) & 0x7U,
                (insn >> 12) & 0xFU,
                (insn >> 8) & 0xFU,
                (insn >> 5) & 0x7U,
                (insn >> 0) & 0x1FU
            ),
        _ when (insn & 0xFFF00000) == 0xD5300000 && (insn & 0xFFFFFFE0) != 0xd53bd040 => // Allow native TPIDR_EL0
            new Mrs(
                (insn >> 19) & 0x1U,
                (insn >> 16) & 0x7U,
                (insn >> 12) & 0xFU,
                (insn >> 8) & 0xFU,
                (insn >> 5) & 0x7U,
                (insn >> 0) & 0x1FU
            ),
        _ => null,
    };
    
    internal record Svc(uint Imm) : Instruction;
    internal record Brk(uint Imm) : Instruction;
    internal record Mrs(uint Op0, uint Op1, uint Crn, uint Crm, uint Op2, uint Reg) : Instruction;
    internal record Msr(uint Op0, uint Op1, uint Crn, uint Crm, uint Op2, uint Reg) : Instruction;
}