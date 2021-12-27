// ReSharper disable CheckNamespace

#region

using Math = LibSharpRetro.CpuHelpers.Math;

#endregion

namespace SharpStationCore;

    #region

using static Math;

#endregion

public class Disassembler {
    public const int InstructionCount = 65 + 0;

    public static string Disassemble(uint insn, uint pc) {
        /* ADD */
        if((insn & 0xFC00003F) == 0x00000020) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "add %" + rd + ", %" + rs + ", %" + rt;
        }

        /* ADDI */
        if((insn & 0xFC000000) == 0x20000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<uint>(imm, 16);
            return "addi %" + rt + ", %" + rs + ", " + $"0x{eimm:x08}";
        }

        /* ADDIU */
        if((insn & 0xFC000000) == 0x24000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<uint>(imm, 16);
            return "addiu %" + rt + ", %" + rs + ", " + $"0x{eimm:x08}";
        }

        /* ADDU */
        if((insn & 0xFC00003F) == 0x00000021) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "addu %" + rd + ", %" + rs + ", %" + rt;
        }

        /* AND */
        if((insn & 0xFC00003F) == 0x00000024) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "and %" + rd + ", %" + rs + ", %" + rt;
        }

        /* ANDI */
        if((insn & 0xFC000000) == 0x30000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<uint>(imm, 16);
            return "andi %" + rt + ", %" + rs + ", " + $"0x{eimm:x08}";
        }

        /* BEQ */
        if((insn & 0xFC000000) == 0x10000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            return "beq %" + rs + ", %" + rt + ", " + $"0x{target:x08}";
        }

        /* BGEZ */
        if((insn & 0xFC110000) == 0x04010000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            return "bgez %" + rs + ", " + $"0x{target:x08}";
        }

        /* BGEZAL */
        if((insn & 0xFC110000) == 0x04110000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            return "bgezal %" + rs + ", " + $"0x{target:x08}";
        }

        /* BGTZ */
        if((insn & 0xFC1F0000) == 0x1C000000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            return "bgtz %" + rs + ", " + $"0x{target:x08}";
        }

        /* BLEZ */
        if((insn & 0xFC1F0000) == 0x18000000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            return "blez %" + rs + ", " + $"0x{target:x08}";
        }

        /* BLTZ */
        if((insn & 0xFC110000) == 0x04000000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            return "bltz %" + rs + ", " + $"0x{target:x08}";
        }

        /* BLTZAL */
        if((insn & 0xFC110000) == 0x04100000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            return "bltzal %" + rs + ", " + $"0x{target:x08}";
        }

        /* BNE */
        if((insn & 0xFC000000) == 0x14000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            return "bne %" + rs + ", %" + rt + ", " + $"0x{target:x08}";
        }

        /* BREAK */
        if((insn & 0xFC00003F) == 0x0000000D) {
            var code = (insn >> 6) & 0xFFFFFU;
            return "break " + code;
        }

        /* CFC */
        if((insn & 0xF3E00000) == 0x40400000) {
            var cop = (insn >> 26) & 0x3U;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var cofun = (insn >> 0) & 0x7FFU;
            return "cfc" + cop + " %" + rt + ", " + rd;
        }

        /* COP */
        if((insn & 0xF2000000) == 0x42000000) {
            var cop = (insn >> 26) & 0x3U;
            var command = (insn >> 0) & 0x1FFFFFFU;
            return "cop" + cop + " " + $"0x{command:x06}";
        }

        /* CTC */
        if((insn & 0xF3E00000) == 0x40C00000) {
            var cop = (insn >> 26) & 0x3U;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var cofun = (insn >> 0) & 0x7FFU;
            return "ctc" + cop + " %" + rt + ", " + rd;
        }

        /* DIV */
        if((insn & 0xFC00003F) == 0x0000001A) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "div %" + rs + ", %" + rt;
        }

        /* DIVU */
        if((insn & 0xFC00003F) == 0x0000001B) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "divu %" + rs + ", %" + rt;
        }

        /* J */
        if((insn & 0xFC000000) == 0x08000000) {
            var imm = (insn >> 0) & 0x3FFFFFFU;
            var target = ((pc + 4) & 0xF0000000U) +
                         (imm << 0x2);
            return "j " + $"0x{target:x08}";
        }

        /* JAL */
        if((insn & 0xFC000000) == 0x0C000000) {
            var imm = (insn >> 0) & 0x3FFFFFFU;
            var target = ((pc + 4) & 0xF0000000U) +
                         (imm << 0x2);
            return "jal " + $"0x{target:x08}";
        }

        /* JALR */
        if((insn & 0xFC00003F) == 0x00000009) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "jalr %" + rd + ", %" + rs;
        }

        /* JR */
        if((insn & 0xFC00003F) == 0x00000008) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "jr %" + rs;
        }

        /* LB */
        if((insn & 0xFC000000) == 0x80000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "lb %" + rt + ", " + $"0x{offset:x08}" + "(%" + rs + ")";
        }

        /* LBU */
        if((insn & 0xFC000000) == 0x90000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "lbu %" + rt + ", " + $"0x{offset:x08}" + "(%" + rs + ")";
        }

        /* LH */
        if((insn & 0xFC000000) == 0x84000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "lh %" + rt + ", " + $"0x{offset:x08}" + "(%" + rs + ")";
        }

        /* LHU */
        if((insn & 0xFC000000) == 0x94000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "lhu %" + rt + ", " + $"0x{offset:x08}" + "(%" + rs + ")";
        }

        /* LUI */
        if((insn & 0xFC000000) == 0x3C000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            return "lui %" + rt + ", " + $"0x{imm:x04}";
        }

        /* LW */
        if((insn & 0xFC000000) == 0x8C000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "lw %" + rt + ", " + $"0x{offset:x08}" + "(%" + rs + ")";
        }

        /* LWL */
        if((insn & 0xFC000000) == 0x88000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "lwl %" + rt + ", " + $"0x{offset:x08}" + "(%" + rs + ")";
        }

        /* LWR */
        if((insn & 0xFC000000) == 0x98000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "lwr %" + rt + ", " + $"0x{offset:x08}" + "(%" + rs + ")";
        }

        /* LWC2 */
        if((insn & 0xFC000000) == 0xC8000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "lwc2 " + rt + ", " + $"0x{offset:x08}" + "(%" + rs + ")";
        }

        /* MFC */
        if((insn & 0xF3E00000) == 0x40000000) {
            var cop = (insn >> 26) & 0x3U;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var cofun = (insn >> 0) & 0x7FFU;
            return "mfc" + cop + " %" + rt + ", " + rd;
        }

        /* MFHI */
        if((insn & 0xFC00003F) == 0x00000010) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "mfhi %" + rd;
        }

        /* MFLO */
        if((insn & 0xFC00003F) == 0x00000012) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "mflo %" + rd;
        }

        /* MTC */
        if((insn & 0xF3E00000) == 0x40800000) {
            var cop = (insn >> 26) & 0x3U;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var cofun = (insn >> 0) & 0x7FFU;
            return "mtc" + cop + " %" + rt + ", " + rd;
        }

        /* MTHI */
        if((insn & 0xFC00003F) == 0x00000011) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "mthi %" + rs;
        }

        /* MTLO */
        if((insn & 0xFC00003F) == 0x00000013) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "mtlo %" + rs;
        }

        /* MULT */
        if((insn & 0xFC00003F) == 0x00000018) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "mult %" + rs + ", %" + rt;
        }

        /* MULTU */
        if((insn & 0xFC00003F) == 0x00000019) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "multu %" + rs + ", %" + rt;
        }

        /* NOR */
        if((insn & 0xFC00003F) == 0x00000027) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "nor %" + rd + ", %" + rs + ", %" + rt;
        }

        /* OR */
        if((insn & 0xFC00003F) == 0x00000025) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "or %" + rd + ", %" + rs + ", %" + rt;
        }

        /* ORI */
        if((insn & 0xFC000000) == 0x34000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            return "ori %" + rt + ", %" + rs + ", " + $"0x{imm:x04}";
        }

        /* SB */
        if((insn & 0xFC000000) == 0xA0000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "sb %" + rt + ", " + $"0x{offset:x08}" + "(%" + rs + ")";
        }

        /* SH */
        if((insn & 0xFC000000) == 0xA4000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "sh %" + rt + ", " + $"0x{offset:x08}" + "(%" + rs + ")";
        }

        /* SLL */
        if((insn & 0xFC00003F) == 0x00000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "sll %" + rd + ", %" + rt + ", " + shamt;
        }

        /* SLLV */
        if((insn & 0xFC00003F) == 0x00000004) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "sllv %" + rd + ", %" + rt + ", %" + rs;
        }

        /* SLT */
        if((insn & 0xFC00003F) == 0x0000002A) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "slt %" + rd + ", %" + rs + ", %" + rt;
        }

        /* SLTI */
        if((insn & 0xFC000000) == 0x28000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<int>(imm, 16);
            return "slti %" + rt + ", %" + rs + ", " + $"0x{eimm:x08}";
        }

        /* SLTIU */
        if((insn & 0xFC000000) == 0x2C000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<uint>(imm, 16);
            return "sltiu %" + rt + ", %" + rs + ", " + $"0x{eimm:x08}";
        }

        /* SLTU */
        if((insn & 0xFC00003F) == 0x0000002B) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "tltu %" + rd + ", %" + rs + ", %" + rt;
        }

        /* SRA */
        if((insn & 0xFC00003F) == 0x00000003) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "sra %" + rd + ", %" + rt + ", " + shamt;
        }

        /* SRAV */
        if((insn & 0xFC00003F) == 0x00000007) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "srav %" + rd + ", %" + rt + ", %" + rs;
        }

        /* SRL */
        if((insn & 0xFC00003F) == 0x00000002) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "srl %" + rd + ", %" + rt + ", " + shamt;
        }

        /* SRLV */
        if((insn & 0xFC00003F) == 0x00000006) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "srlv %" + rd + ", %" + rt + ", %" + rs;
        }

        /* SUB */
        if((insn & 0xFC00003F) == 0x00000022) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "sub %" + rd + ", %" + rs + ", %" + rt;
        }

        /* SUBU */
        if((insn & 0xFC00003F) == 0x00000023) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "subu %" + rd + ", %" + rs + ", %" + rt;
        }

        /* SW */
        if((insn & 0xFC000000) == 0xAC000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "sw %" + rt + ", " + $"0x{offset:x08}" + "(%" + rs + ")";
        }

        /* SWC2 */
        if((insn & 0xFC000000) == 0xE8000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "swc2 " + rt + ", " + $"0x{offset:x08}" + "(%" + rs + ")";
        }

        /* SWL */
        if((insn & 0xFC000000) == 0xA8000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "swl %" + rt + ", " + $"0x{offset:x08}" + "(%" + rs + ")";
        }

        /* SWR */
        if((insn & 0xFC000000) == 0xB8000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "swr %" + rt + ", " + $"0x{offset:x08}" + "(%" + rs + ")";
        }

        /* SYSCALL */
        if((insn & 0xFC00003F) == 0x0000000C) {
            var code = (insn >> 6) & 0xFFFFFU;
            return "syscall " + code;
        }

        /* XOR */
        if((insn & 0xFC00003F) == 0x00000026) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "xor %" + rd + ", %" + rs + ", %" + rt;
        }

        /* XORI */
        if((insn & 0xFC000000) == 0x38000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            return "xori %" + rt + ", %" + rs + ", " + $"0x{imm:x04}";
        }

        return null;
    }

    public static string ClassifyInstruction(uint insn) {
        var pc = 0xDEADBEEFU; // Should never be actually used
        if((insn & 0xFC00003F) == 0x00000020) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "ADD";
        }

        if((insn & 0xFC000000) == 0x20000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<uint>(imm, 16);
            return "ADDI";
        }

        if((insn & 0xFC000000) == 0x24000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<uint>(imm, 16);
            return "ADDIU";
        }

        if((insn & 0xFC00003F) == 0x00000021) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "ADDU";
        }

        if((insn & 0xFC00003F) == 0x00000024) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "AND";
        }

        if((insn & 0xFC000000) == 0x30000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<uint>(imm, 16);
            return "ANDI";
        }

        if((insn & 0xFC000000) == 0x10000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            return "BEQ";
        }

        if((insn & 0xFC110000) == 0x04010000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            return "BGEZ";
        }

        if((insn & 0xFC110000) == 0x04110000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            return "BGEZAL";
        }

        if((insn & 0xFC1F0000) == 0x1C000000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            return "BGTZ";
        }

        if((insn & 0xFC1F0000) == 0x18000000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            return "BLEZ";
        }

        if((insn & 0xFC110000) == 0x04000000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            return "BLTZ";
        }

        if((insn & 0xFC110000) == 0x04100000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            return "BLTZAL";
        }

        if((insn & 0xFC000000) == 0x14000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            return "BNE";
        }

        if((insn & 0xFC00003F) == 0x0000000D) {
            var code = (insn >> 6) & 0xFFFFFU;
            return "BREAK";
        }

        if((insn & 0xF3E00000) == 0x40400000) {
            var cop = (insn >> 26) & 0x3U;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var cofun = (insn >> 0) & 0x7FFU;
            return "CFC";
        }

        if((insn & 0xF2000000) == 0x42000000) {
            var cop = (insn >> 26) & 0x3U;
            var command = (insn >> 0) & 0x1FFFFFFU;
            return "COP";
        }

        if((insn & 0xF3E00000) == 0x40C00000) {
            var cop = (insn >> 26) & 0x3U;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var cofun = (insn >> 0) & 0x7FFU;
            return "CTC";
        }

        if((insn & 0xFC00003F) == 0x0000001A) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "DIV";
        }

        if((insn & 0xFC00003F) == 0x0000001B) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "DIVU";
        }

        if((insn & 0xFC000000) == 0x08000000) {
            var imm = (insn >> 0) & 0x3FFFFFFU;
            var target = ((pc + 4) & 0xF0000000U) +
                         (imm << 0x2);
            return "J";
        }

        if((insn & 0xFC000000) == 0x0C000000) {
            var imm = (insn >> 0) & 0x3FFFFFFU;
            var target = ((pc + 4) & 0xF0000000U) +
                         (imm << 0x2);
            return "JAL";
        }

        if((insn & 0xFC00003F) == 0x00000009) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "JALR";
        }

        if((insn & 0xFC00003F) == 0x00000008) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "JR";
        }

        if((insn & 0xFC000000) == 0x80000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "LB";
        }

        if((insn & 0xFC000000) == 0x90000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "LBU";
        }

        if((insn & 0xFC000000) == 0x84000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "LH";
        }

        if((insn & 0xFC000000) == 0x94000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "LHU";
        }

        if((insn & 0xFC000000) == 0x3C000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            return "LUI";
        }

        if((insn & 0xFC000000) == 0x8C000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "LW";
        }

        if((insn & 0xFC000000) == 0x88000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "LWL";
        }

        if((insn & 0xFC000000) == 0x98000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "LWR";
        }

        if((insn & 0xFC000000) == 0xC8000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "LWC2";
        }

        if((insn & 0xF3E00000) == 0x40000000) {
            var cop = (insn >> 26) & 0x3U;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var cofun = (insn >> 0) & 0x7FFU;
            return "MFC";
        }

        if((insn & 0xFC00003F) == 0x00000010) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "MFHI";
        }

        if((insn & 0xFC00003F) == 0x00000012) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "MFLO";
        }

        if((insn & 0xF3E00000) == 0x40800000) {
            var cop = (insn >> 26) & 0x3U;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var cofun = (insn >> 0) & 0x7FFU;
            return "MTC";
        }

        if((insn & 0xFC00003F) == 0x00000011) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "MTHI";
        }

        if((insn & 0xFC00003F) == 0x00000013) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "MTLO";
        }

        if((insn & 0xFC00003F) == 0x00000018) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "MULT";
        }

        if((insn & 0xFC00003F) == 0x00000019) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "MULTU";
        }

        if((insn & 0xFC00003F) == 0x00000027) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "NOR";
        }

        if((insn & 0xFC00003F) == 0x00000025) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "OR";
        }

        if((insn & 0xFC000000) == 0x34000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            return "ORI";
        }

        if((insn & 0xFC000000) == 0xA0000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "SB";
        }

        if((insn & 0xFC000000) == 0xA4000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "SH";
        }

        if((insn & 0xFC00003F) == 0x00000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "SLL";
        }

        if((insn & 0xFC00003F) == 0x00000004) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "SLLV";
        }

        if((insn & 0xFC00003F) == 0x0000002A) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "SLT";
        }

        if((insn & 0xFC000000) == 0x28000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<int>(imm, 16);
            return "SLTI";
        }

        if((insn & 0xFC000000) == 0x2C000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<uint>(imm, 16);
            return "SLTIU";
        }

        if((insn & 0xFC00003F) == 0x0000002B) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "SLTU";
        }

        if((insn & 0xFC00003F) == 0x00000003) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "SRA";
        }

        if((insn & 0xFC00003F) == 0x00000007) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "SRAV";
        }

        if((insn & 0xFC00003F) == 0x00000002) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "SRL";
        }

        if((insn & 0xFC00003F) == 0x00000006) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "SRLV";
        }

        if((insn & 0xFC00003F) == 0x00000022) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "SUB";
        }

        if((insn & 0xFC00003F) == 0x00000023) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "SUBU";
        }

        if((insn & 0xFC000000) == 0xAC000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "SW";
        }

        if((insn & 0xFC000000) == 0xE8000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "SWC2";
        }

        if((insn & 0xFC000000) == 0xA8000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "SWL";
        }

        if((insn & 0xFC000000) == 0xB8000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            return "SWR";
        }

        if((insn & 0xFC00003F) == 0x0000000C) {
            var code = (insn >> 6) & 0xFFFFFU;
            return "SYSCALL";
        }

        if((insn & 0xFC00003F) == 0x00000026) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "XOR";
        }

        if((insn & 0xFC000000) == 0x38000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            return "XORI";
        }

        return null;
    }
}