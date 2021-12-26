// ReSharper disable CheckNamespace
// ReSharper disable ArrangeRedundantParentheses
// ReSharper disable RedundantCast

#region

using Math = LibSharpRetro.CpuHelpers.Math;

#endregion

#pragma warning disable CS0164
namespace SharpStationCore;

    #region

using static Math;

#endregion

public class Disassembler {
    public const int InstructionCount = 14 + 0;

    public static string Disassemble(uint insn, uint pc) {
        /* ADD */
        if((insn & 0xFC00003F) == 0x00000020) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return (string) ("add %" + (rd) + ", %" + (rs) + ", %" + (rt));
        }

        insn_1:
        /* ADDI */
        if((insn & 0xFC000000) == 0x20000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = (uint) (SignExt<uint>(imm, 16));
            return (string) ("addi %" + (rt) + ", %" + (rs) + ", " + (string) ($"0x{(eimm):x08}"));
        }

        insn_2:
        /* ADDIU */
        if((insn & 0xFC000000) == 0x24000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = (uint) (SignExt<uint>(imm, 16));
            return (string) ("addiu %" + (rt) + ", %" + (rs) + ", " + (string) ($"0x{(eimm):x08}"));
        }

        insn_3:
        /* ADDU */
        if((insn & 0xFC00003F) == 0x00000021) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return (string) ("addu %" + (rd) + ", %" + (rs) + ", %" + (rt));
        }

        insn_4:
        /* AND */
        if((insn & 0xFC00003F) == 0x00000024) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return (string) ("and %" + (rd) + ", %" + (rs) + ", %" + (rt));
        }

        insn_5:
        /* ANDI */
        if((insn & 0xFC000000) == 0x30000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = (uint) (SignExt<uint>(imm, 16));
            return (string) ("andi %" + (rt) + ", %" + (rs) + ", " + (string) ($"0x{(eimm):x08}"));
        }

        insn_6:
        /* BEQ */
        if((insn & 0xFC000000) == 0x10000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) +
                                 ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
            return (string) ("beq %" + (rs) + ", %" + (rt) + ", " + (string) ($"0x{(target):x08}"));
        }

        insn_7:
        /* BGEZ */
        if((insn & 0xFC110000) == 0x04010000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) +
                                 ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
            return (string) ("bgez %" + (rs) + ", " + (string) ($"0x{(target):x08}"));
        }

        insn_8:
        /* BGEZAL */
        if((insn & 0xFC110000) == 0x04110000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) +
                                 ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
            return (string) ("bgezal %" + (rs) + ", " + (string) ($"0x{(target):x08}"));
        }

        insn_9:
        /* BGTZ */
        if((insn & 0xFC1F0000) == 0x1C000000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) +
                                 ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
            return (string) ("bgtz %" + (rs) + ", " + (string) ($"0x{(target):x08}"));
        }

        insn_10:
        /* BLEZ */
        if((insn & 0xFC1F0000) == 0x18000000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) +
                                 ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
            return (string) ("blez %" + (rs) + ", " + (string) ($"0x{(target):x08}"));
        }

        insn_11:
        /* BLTZ */
        if((insn & 0xFC110000) == 0x04000000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) +
                                 ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
            return (string) ("bltz %" + (rs) + ", " + (string) ($"0x{(target):x08}"));
        }

        insn_12:
        /* BLTZAL */
        if((insn & 0xFC110000) == 0x04100000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) +
                                 ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
            return (string) ("bltzal %" + (rs) + ", " + (string) ($"0x{(target):x08}"));
        }

        insn_13:
        /* BNE */
        if((insn & 0xFC000000) == 0x14000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) +
                                 ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
            return (string) ("bne %" + (rs) + ", %" + (rt) + ", " + (string) ($"0x{(target):x08}"));
        }

        insn_14:

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

        insn_1:
        if((insn & 0xFC000000) == 0x20000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = (uint) (SignExt<uint>(imm, 16));
            return "ADDI";
        }

        insn_2:
        if((insn & 0xFC000000) == 0x24000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = (uint) (SignExt<uint>(imm, 16));
            return "ADDIU";
        }

        insn_3:
        if((insn & 0xFC00003F) == 0x00000021) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "ADDU";
        }

        insn_4:
        if((insn & 0xFC00003F) == 0x00000024) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            return "AND";
        }

        insn_5:
        if((insn & 0xFC000000) == 0x30000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = (uint) (SignExt<uint>(imm, 16));
            return "ANDI";
        }

        insn_6:
        if((insn & 0xFC000000) == 0x10000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) +
                                 ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
            return "BEQ";
        }

        insn_7:
        if((insn & 0xFC110000) == 0x04010000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) +
                                 ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
            return "BGEZ";
        }

        insn_8:
        if((insn & 0xFC110000) == 0x04110000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) +
                                 ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
            return "BGEZAL";
        }

        insn_9:
        if((insn & 0xFC1F0000) == 0x1C000000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) +
                                 ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
            return "BGTZ";
        }

        insn_10:
        if((insn & 0xFC1F0000) == 0x18000000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) +
                                 ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
            return "BLEZ";
        }

        insn_11:
        if((insn & 0xFC110000) == 0x04000000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) +
                                 ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
            return "BLTZ";
        }

        insn_12:
        if((insn & 0xFC110000) == 0x04100000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) +
                                 ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
            return "BLTZAL";
        }

        insn_13:
        if((insn & 0xFC000000) == 0x14000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) +
                                 ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
            return "BNE";
        }

        insn_14:

        return null;
    }
}