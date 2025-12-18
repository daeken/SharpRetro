using System.Diagnostics;
using LibSharpRetro;
using NxCommon;
using NxTranslate;

var exeLoader = new ExeLoader(args[0], includeRtld: true, doRelocate: false);
var macho = new MachoWriter();
var loadBase = 0UL;
foreach(var (i, mod) in exeLoader.ExeModules.Index()) {
    var problematic = new List<(ulong Addr, Instruction Instruction)>();
    for(var j = mod.TextStart; j < mod.TextEnd; j += 4) {
        var insn = Instruction.Decode(mod.Load<uint>(j));
        if(insn == null) continue;
        problematic.Add((j, insn));
    }

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

    if(problematic.Count != 0) {
        macho.AddSegment($".tramprw_{i}", loadBase, 0x4000, [], MemoryProtection.Read | MemoryProtection.Write);
        macho.AddSegment($".trampx_{i}", loadBase + 0x4000, 0xC000, [], MemoryProtection.Read | MemoryProtection.Execute);
        loadBase += 0x10000;
    }
    foreach(var symbol in mod.Symbols)
        if(symbol.Value != 0 && symbol.Name != "")
            macho.AddSymbol(symbol.Name, loadBase + symbol.Value);
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
macho.AddSegment(".glue", loadBase, 0x4000, [], MemoryProtection.Read | MemoryProtection.Execute);

macho.Write(args[1]);
Sh.Run("ldid", "-S", args[1]);

abstract record Instruction {
    public static Instruction Decode(uint insn) => insn switch {
        _ when (insn & 0xFFE0001F) == 0xD4200000 =>
            new Brk((insn >> 5) & 0xFFFFU),
        _ when (insn & 0xFFE0001F) == 0xD4000001 =>
            new Svc((insn >> 5) & 0xFFFFU),
        _ when (insn & 0xFFF00000) == 0xD5100000 =>
            new Msr(
                (insn >> 19) & 0x1U,
                (insn >> 16) & 0x7U,
                (insn >> 12) & 0xFU,
                (insn >> 8) & 0xFU,
                (insn >> 5) & 0x7U,
                (insn >> 0) & 0x1FU
            ),
        _ when (insn & 0xFFF00000) == 0xD5300000 =>
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