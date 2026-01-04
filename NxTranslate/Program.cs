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

byte[] BuildTrampolines(ulong textBase, ulong trampBase, ulong trampRwBase, List<(ulong Addr, Instruction Instruction)> insns, List<(ulong Addr, uint Insn)> x18Usage, Memory<byte> text, Dictionary<string, ulong> symbols) {
    insns = insns.OrderByDescending(x => x.Addr).ToList(); // Work back to front to maximize odds of being in range
    var size = textBase - trampBase;
    var asm = new Assembler((int) size);
    var jumpBase = size;
    var loadX18Cache = new Dictionary<(R.RX, R.RX), ulong>();
    var storeX18Cache = new Dictionary<(R.RX, R.RX), ulong>();
    foreach(var (addr, insn) in insns) {
        jumpBase -= 4;
        var pc = asm.PC;
        asm.PC = jumpBase;
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
        Debug.Assert(asm.PC < jumpBase);
    }
    foreach(var (addr, insn) in x18Usage) {
        void Sub() {
            jumpBase -= 4;
            var pc = asm.PC;
            asm.PC = jumpBase;
            symbols[$"tramp_tramp_{textBase + addr:X}"] = asm.PC;
            var tasm = new Assembler(4);
            tasm.B((long) (trampBase + asm.PC) - (long) (textBase + addr));
            tasm.AsBytes.CopyTo(text[(int) addr..].Span);
            asm.B((long) pc - (long) asm.PC);
            asm.PC = pc;
            symbols[$"tramp_{textBase + addr:X}"] = asm.PC;

            var regInfo = Disassembler.GetGprMasks(insn)
                .Select(x => (x.PossiblySp, x.IsRead, x.IsWritten, x.Shift,
                    Register: (int) ((insn >> x.Shift) & 0b11111)))
                .ToList();
            var writesSp = regInfo.Any(x => x is { PossiblySp: true, IsWritten: true, Register: 0b11111 });
            var usesSp = regInfo.Any(x => x is { PossiblySp: true, Register: 0b11111 });
            var specialCase = writesSp || Disassembler.IsPcDependent(insn) || Disassembler.ClassifyInstruction(insn) is "BR" or "BLR" or "RET";

            var regsUsed = regInfo.Select(x => x.Register).ToHashSet();
            var safeA = -1;
            var safeB = -1;
            for(var i = 0; i < 18; ++i) {
                if(regsUsed.Contains(i)) continue;
                if(safeA == -1)
                    safeA = i;
                else {
                    safeB = i;
                    break;
                }
            }
            var sregA = new R.RX(safeA);
            var sregB = new R.RX(safeB);

            const int spOffset = 48;
            void Prologue() {
                asm.StpPreindex(R.X29, R.X30, R.SP, -32);
                asm.Stp(sregA, sregB, R.SP, 16);
                asm.Mov(R.X29, R.SP);
                asm.AddrOf(R.X30, trampBase + asm.PC, textBase + addr + 4);
                asm.StpPreindex(R.X29, R.X30, R.SP, -16);
                asm.Mov(R.X29, R.SP);
            }

            const int epilogueInsns = 3;
            void Epilogue() {
                asm.Add(R.SP, R.SP, 16);
                asm.Ldp(sregA, sregB, R.SP, 16);
                asm.LdpPostindex(R.X29, R.X30, R.SP, 32);
            }

            void SaveBut(Action func) {
                R.RX E(R.RX x) => x == sregA || x == sregB ? R.XZR : x;

                void Stp(R.RX a, R.RX b, R.RSP sp, int imm) {
                    if(a == R.XZR && b == R.XZR) return;
                    asm.Stp(a, b, sp, imm);
                }
                void Ldp(R.RX a, R.RX b, R.RSP sp, int imm) {
                    if(a == R.XZR && b == R.XZR) return;
                    asm.Ldp(a, b, sp, imm);
                }
                asm.Sub(R.SP, R.SP, 1024);
                Stp(E(R.X29), E(R.X30), R.SP, 16 * 0);
                Stp(E(R.X27), E(R.X28), R.SP, 16 * 1);
                Stp(E(R.X25), E(R.X26), R.SP, 16 * 2);
                Stp(E(R.X23), E(R.X24), R.SP, 16 * 3);
                Stp(E(R.X21), E(R.X22), R.SP, 16 * 4);
                Stp(E(R.X19), E(R.X20), R.SP, 16 * 5);
                Stp(E(R.X17), E(R.XZR), R.SP, 16 * 6);
                Stp(E(R.X15), E(R.X16), R.SP, 16 * 7);
                Stp(E(R.X13), E(R.X14), R.SP, 16 * 8);
                Stp(E(R.X11), E(R.X12), R.SP, 16 * 9);
                Stp(E(R.X9), E(R.X10), R.SP, 16 * 10);
                Stp(E(R.X7), E(R.X8), R.SP, 16 * 11);
                Stp(E(R.X5), E(R.X6), R.SP, 16 * 12);
                Stp(E(R.X3), E(R.X4), R.SP, 16 * 13);
                Stp(E(R.X1), E(R.X2), R.SP, 16 * 14);
                asm.ReadNzcv(R.X24);
                asm.Stp(E(R.X0), R.X24, R.SP, 16 * 15);
                for(var i = 0; i < 16; ++i)
                    asm.Stp(new V(i * 2), new V(i * 2 + 1), R.SP, 32 * (8 + i));
                asm.Mov(R.X29, R.SP);
                func();
                for(var i = 0; i < 16; ++i)
                    asm.Ldp(new V(i * 2), new V(i * 2 + 1), R.SP, 32 * (8 + i));
                asm.Ldp(E(R.X0), R.X24, R.SP, 16 * 15);
                asm.WriteNzcv(R.X24);
                Ldp(E(R.X1), E(R.X2), R.SP, 16 * 14);
                Ldp(E(R.X3), E(R.X4), R.SP, 16 * 13);
                Ldp(E(R.X5), E(R.X6), R.SP, 16 * 12);
                Ldp(E(R.X7), E(R.X8), R.SP, 16 * 11);
                Ldp(E(R.X9), E(R.X10), R.SP, 16 * 10);
                Ldp(E(R.X11), E(R.X12), R.SP, 16 * 9);
                Ldp(E(R.X13), E(R.X14), R.SP, 16 * 8);
                Ldp(E(R.X15), E(R.X16), R.SP, 16 * 7);
                Ldp(E(R.X17), E(R.XZR), R.SP, 16 * 6);
                Ldp(E(R.X19), E(R.X20), R.SP, 16 * 5);
                Ldp(E(R.X21), E(R.X22), R.SP, 16 * 4);
                Ldp(E(R.X23), E(R.X24), R.SP, 16 * 3);
                Ldp(E(R.X25), E(R.X26), R.SP, 16 * 2);
                Ldp(E(R.X27), E(R.X28), R.SP, 16 * 1);
                Ldp(E(R.X29), E(R.X30), R.SP, 16 * 0);
                asm.Add(R.SP, R.SP, 1024);
                asm.Ret();
            }

            void LoadX18(R.RX reg = null) {
                reg ??= sregA;
                if(!loadX18Cache.ContainsKey((reg, sregB))) {
                    var prev = asm.PC;
                    asm.PC += 4;
                    loadX18Cache[(reg, sregB)] = asm.PC;
                    SaveBut(() => {
                        asm.AddrOf(sregB, trampBase + asm.PC, trampRwBase + 0x8);
                        asm.Ldr(sregB, sregB);
                        asm.Blr(sregB);
                        if(reg != R.X0)
                            asm.Mov(reg, R.X0);
                    });
                    var end = asm.PC;
                    asm.PC = prev;
                    asm.B((long) (end - prev));
                    asm.PC = end;
                }
                var func = loadX18Cache[(reg, sregB)];
                asm.BL((long) func - (long) asm.PC);
            }

            void StoreX18() {
                if(!storeX18Cache.ContainsKey((sregA, sregB))) {
                    var prev = asm.PC;
                    asm.PC += 4;
                    storeX18Cache[(sregA, sregB)] = asm.PC;
                    SaveBut(() => {
                        asm.AddrOf(sregB, trampBase + asm.PC, trampRwBase + 0x10);
                        asm.Ldr(sregB, sregB);
                        if(sregA != R.X0)
                            asm.Mov(R.X0, sregA);
                        asm.Blr(sregB);
                    });
                    var end = asm.PC;
                    asm.PC = prev;
                    asm.B((long) (end - prev));
                    asm.PC = end;
                }
                var func = storeX18Cache[(sregA, sregB)];
                asm.BL((long) func - (long) asm.PC);
            }

            if(specialCase) {
                switch(Disassembler.ClassifyInstruction(insn)) {
                    case "ADRP": {
                        Prologue();
                        var immlo = (insn >> 29) & 0x3U;
                        var immhi = (insn >> 5) & 0x7FFFFU;
                        var uimm = ((ulong) immlo << 12) | ((ulong) immhi << 14);
                        var simm = LibSharpRetro.CpuHelpers.Math.SignExt<long>(uimm, 33);
                        var taddr = unchecked((((textBase + addr) >> 12) << 12) + (ulong) simm);

                        var baddr = ((trampBase + asm.PC) >> 12) << 12;
                        simm = unchecked((long) (taddr - baddr));
                        var temp = simm >> (12 + 21);
                        if(temp is not 0 and not -1)
                            throw new Exception("ADRP OUT OF RANGE!");
                        uimm = unchecked((ulong) (simm >> 12));
                        asm.Adrp(sregA, uimm);
                        StoreX18();
                        Epilogue();
                        asm.B((long) (textBase + addr + 4) - (long) (trampBase + asm.PC));
                        return;
                    }
                    case "CBZ":
                    case "CBNZ": {
                        Prologue();
                        var imm = ((insn >> 5) & 0x7FFFFU) << 2;
                        var simm = LibSharpRetro.CpuHelpers.Math.SignExt<long>(imm, 21);
                        var taddr = unchecked(textBase + addr + (ulong) simm);
                        LoadX18();
                        var reinsn = insn & 0b1_111111_1_0000000000000000000_00000;
                        reinsn |= (uint) safeA;
                        asm.Raw(reinsn | (((uint) epilogueInsns + 2) << 5));
                        // not taken
                        Epilogue();
                        asm.B((long) (textBase + addr + 4) - (long) (trampBase + asm.PC));
                        // taken
                        Epilogue();
                        asm.B((long) taddr - (long) (trampBase + asm.PC));
                        return;
                    }
                    case "TBZ":
                    case "TBNZ": {
                        Prologue();
                        var imm = ((insn >> 5) & 0x3FFFU) << 2;
                        var simm = LibSharpRetro.CpuHelpers.Math.SignExt<long>(imm, 16);
                        var taddr = unchecked(textBase + addr + (ulong) simm);
                        LoadX18();
                        var reinsn = insn & 0b1_111111_1_11111_00000000000000_00000;
                        reinsn |= (uint) safeA;
                        asm.Raw(reinsn | (((uint) epilogueInsns + 2) << 5));
                        // not taken
                        Epilogue();
                        asm.B((long) (textBase + addr + 4) - (long) (trampBase + asm.PC));
                        // taken
                        Epilogue();
                        asm.B((long) taddr - (long) (trampBase + asm.PC));
                        return;
                    }
                    case "STP-preindex": {
                        var size = (insn >> 31) & 0x1U;
                        var imm = (insn >> 15) & 0x7FU;
                        var simm = LibSharpRetro.CpuHelpers.Math.SignExt<long>(imm, 7) << (size == 1 ? 3 : 2);
                        if(simm >= 0)
                            asm.Add(R.SP, R.SP, unchecked((ushort) (ulong) simm));
                        else
                            asm.Sub(R.SP, R.SP, unchecked((ushort) (ulong) -simm));
                        Prologue();
                        LoadX18();
                        var reinsn = insn;
                        reinsn ^= 0b00_000_0_001_0_0000000_00000_00000_00000; // no more preindex
                        reinsn &= 0b11_111_1_111_1_0000000_11111_11111_11111; // nuke the immediate
                        reinsn |= ((uint) spOffset >> (size == 1 ? 3 : 2)) << 15; // shift back to 'real sp'
                        foreach(var (_, _, _, shift, reg) in regInfo)
                            if(reg == 18) {
                                reinsn &= 0xFFFFFFFFU ^ (0b11111U << shift);
                                reinsn |= (uint) safeA << shift;
                            }
                        asm.Raw(reinsn);
                        Epilogue();
                        asm.B((long) (textBase + addr + 4) - (long) (trampBase + asm.PC));
                        return;
                    }
                    case "LDP-immediate-postindex": {
                        Prologue();
                        var size = (insn >> 31) & 0x1U;
                        var imm = (insn >> 15) & 0x7FU;
                        var simm = LibSharpRetro.CpuHelpers.Math.SignExt<long>(imm, 7) << (size == 1 ? 3 : 2);
                        var reinsn = insn;
                        reinsn ^= 0b00_000_0_011_0_0000000_00000_00000_00000; // no more postindex
                        reinsn &= 0b11_111_1_111_1_0000000_11111_11111_11111; // nuke the immediate
                        reinsn |= ((uint) spOffset >> (size == 1 ? 3 : 2)) << 15; // shift back to 'real sp'
                        foreach(var (_, _, _, shift, reg) in regInfo)
                            if(reg == 18) {
                                reinsn &= 0xFFFFFFFFU ^ (0b11111U << shift);
                                reinsn |= (uint) safeA << shift;
                            }
                        asm.Raw(reinsn);
                        StoreX18();
                        Epilogue();
                        if(simm >= 0)
                            asm.Add(R.SP, R.SP, unchecked((ushort) (ulong) simm));
                        else
                            asm.Sub(R.SP, R.SP, unchecked((ushort) (ulong) -simm));
                        asm.B((long) (textBase + addr + 4) - (long) (trampBase + asm.PC));
                        return;
                    }
                    case "BR": {
                        asm.StpPreindex(sregA, sregB, R.SP, -16);
                        LoadX18();
                        asm.Mov(R.X30, sregA); // Force overwrite LR -- fuck your frame consistency
                        asm.LdpPostindex(sregA, sregB, R.SP, 16);
                        asm.Br(R.X30);
                        return;
                    }
                    case "BLR": {
                        asm.StpPreindex(sregA, sregB, R.SP, -16);
                        LoadX18();
                        asm.Mov(R.X30, sregA);
                        asm.LdpPostindex(sregA, sregB, R.SP, 16);
                        asm.Blr(R.X30);
                        asm.B((long) (textBase + addr + 4) - (long) (trampBase + asm.PC));
                        return;
                    }
                    default:
                        Console.WriteLine(
                            $"Unhandled instruction classification {Disassembler.ClassifyInstruction(insn)} for r18 rewrite: {Disassembler.Disassemble(insn, addr)}");
                        break;
                }
            }
            Prologue();
            var reinsnn = insn;
            var hasRead = false;
            foreach(var (_, read, _, shift, reg) in regInfo)
                if(reg == 18) {
                    reinsnn &= 0xFFFFFFFFU ^ (0b11111U << shift);
                    reinsnn |= (uint) safeA << shift;
                    if(read && !hasRead) {
                        LoadX18();
                        hasRead = true;
                    }
                }
            if(usesSp)
                asm.Add(R.SP, R.SP, spOffset);
            asm.Raw(reinsnn);
            if(usesSp)
                asm.Sub(R.SP, R.SP, spOffset);
            var hasWritten = false;
            foreach(var (_, _, write, _, reg) in regInfo)
                if(reg == 18 && write && !hasWritten) {
                    StoreX18();
                    hasWritten = true;
                }
            Epilogue();
            asm.B((long) (textBase + addr + 4) - (long) (trampBase + asm.PC));
        }
        Sub();
        Debug.Assert(asm.PC < jumpBase);
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

    var x18Usage = new List<(ulong Addr, uint Insn)>();
    for(var j = mod.TextStart; j < mod.TextEnd; j += 4) {
        var insn = mod.Load<uint>(j);
        var usesX18 = Disassembler.GetGprMasks(insn)
            .Select(x => x.Shift)
            .Any(shift => ((insn >> shift) & 0b11111) == 18);
        if(usesX18) x18Usage.Add((j - mod.TextStart, insn));
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

    var fullTrampSize = 0x10000UL;
    fullTrampSize += (ulong) (128 * x18Usage.Count); // Give ourselves some space
    while(fullTrampSize % 0x10000 != 0) fullTrampSize++;

    var tramprw = ulong.MaxValue;
    var trampx = ulong.MaxValue;
    if(problematic.Count != 0 || x18Usage.Count != 0) {
        tramprw = loadBase;
        trampx = loadBase + 0x4000;
        var trampSymbols = new Dictionary<string, ulong>();
        var trampolines = BuildTrampolines(loadBase + fullTrampSize, loadBase + 0x4000, loadBase, problematic, x18Usage, mod.Binary[(int) textStart..], trampSymbols);
        foreach(var (name, addr) in trampSymbols)
            macho.AddSymbol(name, trampx + addr);
        macho.AddSegment($".tramprw_{i}", loadBase, 0x4000, [], MemoryProtection.Read | MemoryProtection.Write);
        macho.AddSegment($".trampx_{i}", loadBase + 0x4000, fullTrampSize - 0x4000, trampolines, MemoryProtection.Read | MemoryProtection.Execute);
        loadBase += fullTrampSize;
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
    glue.Mov(R.X0, R.X20);
    glue.Mov(R.X1, start);
    glue.Mov(R.X2, tramprw);
    glue.Mov(R.X3, size);
    glue.Add(R.X4, R.X19, (ushort) Marshal.OffsetOf<CallbackTableOffsets>("initModule"));
    glue.Ldr(R.X4, R.X4);
    glue.Blr(R.X4);
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
        _ when (insn & 0xFFF00000) == 0xD5100000 => // NO LONGER && (insn & 0xFFFFFFE0) != 0xd51bd040 => // Allow native TPIDR_EL0
            new Msr(
                (insn >> 19) & 0x1U,
                (insn >> 16) & 0x7U,
                (insn >> 12) & 0xFU,
                (insn >> 8) & 0xFU,
                (insn >> 5) & 0x7U,
                (insn >> 0) & 0x1FU
            ),
        _ when (insn & 0xFFF00000) == 0xD5300000 => // NO LONGER && (insn & 0xFFFFFFE0) != 0xd53bd040 => // Allow native TPIDR_EL0
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