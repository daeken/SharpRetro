// ReSharper disable CheckNamespace

#region

using Math = LibSharpRetro.CpuHelpers.Math;

#endregion

namespace SharpStationCore;

    #region

using static Math;

#endregion

public unsafe partial class Interpreter {
    public bool Interpret(uint insn, uint pc) {
        /* ADD */
        if((insn & 0xFC00003F) == 0x00000020) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var lhs = rs switch { 0 => 0U, var temp_0 => State->Registers[temp_0] };
            var rhs = rt switch { 0 => 0U, var temp_1 => State->Registers[temp_1] };
            var temp_2 = rd;
            if(temp_2 != 0)
                State->Registers[temp_2] = lhs + rhs;
            return true;
        }

        /* ADDI */
        if((insn & 0xFC000000) == 0x20000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<uint>(imm, 16);
            var lhs = rs switch { 0 => 0U, var temp_3 => State->Registers[temp_3] };
            var temp_4 = rt;
            if(temp_4 != 0)
                State->Registers[temp_4] = lhs + eimm;
            return true;
        }

        /* ADDIU */
        if((insn & 0xFC000000) == 0x24000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<uint>(imm, 16);
            var temp_5 = rt;
            if(temp_5 != 0)
                State->Registers[temp_5] =
                    rs switch { 0 => 0U, var temp_6 => State->Registers[temp_6] } + eimm;
            return true;
        }

        /* ADDU */
        if((insn & 0xFC00003F) == 0x00000021) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var temp_7 = rd;
            if(temp_7 != 0)
                State->Registers[temp_7] =
                    rs switch { 0 => 0U, var temp_8 => State->Registers[temp_8] } +
                    rt switch { 0 => 0U, var temp_9 => State->Registers[temp_9] };
            return true;
        }

        /* AND */
        if((insn & 0xFC00003F) == 0x00000024) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var temp_10 = rd;
            if(temp_10 != 0)
                State->Registers[temp_10] = rs switch { 0 => 0U, var temp_11 => State->Registers[temp_11] } &
                                            rt switch { 0 => 0U, var temp_12 => State->Registers[temp_12] };
            return true;
        }

        /* ANDI */
        if((insn & 0xFC000000) == 0x30000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<uint>(imm, 16);
            var temp_13 = rt;
            if(temp_13 != 0)
                State->Registers[temp_13] = rs switch { 0 => 0U, var temp_14 => State->Registers[temp_14] } & eimm;
            return true;
        }

        /* BEQ */
        if((insn & 0xFC000000) == 0x10000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            if((rs switch { 0 => 0U, var temp_15 => State->Registers[temp_15] } ==
                rt switch { 0 => 0U, var temp_16 => State->Registers[temp_16] }
                   ? 1U
                   : 0U) != 0)
                Branch(target);
            else
                Branch(pc + 0x8);
            return true;
        }

        /* BGEZ */
        if((insn & 0xFC110000) == 0x04010000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            if(((int) (rs switch { 0 => 0U, var temp_17 => State->Registers[temp_17] }) >= 0x0 ? 1U : 0U) != 0)
                Branch(target);
            else
                Branch(pc + 0x8);
            return true;
        }

        /* BGEZAL */
        if((insn & 0xFC110000) == 0x04110000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            State->Registers[31] = pc + 0x8;
            if(((int) (rs switch { 0 => 0U, var temp_18 => State->Registers[temp_18] }) >= 0x0 ? 1U : 0U) != 0)
                Branch(target);
            else
                Branch(pc + 0x8);
            return true;
        }

        /* BGTZ */
        if((insn & 0xFC1F0000) == 0x1C000000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            if(((int) (rs switch { 0 => 0U, var temp_19 => State->Registers[temp_19] }) > 0x0 ? 1U : 0U) != 0)
                Branch(target);
            else
                Branch(pc + 0x8);
            return true;
        }

        /* BLEZ */
        if((insn & 0xFC1F0000) == 0x18000000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            if(((int) (rs switch { 0 => 0U, var temp_20 => State->Registers[temp_20] }) <= 0x0 ? 1U : 0U) != 0)
                Branch(target);
            else
                Branch(pc + 0x8);
            return true;
        }

        /* BLTZ */
        if((insn & 0xFC110000) == 0x04000000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            if(((int) (rs switch { 0 => 0U, var temp_21 => State->Registers[temp_21] }) < 0x0 ? 1U : 0U) != 0)
                Branch(target);
            else
                Branch(pc + 0x8);
            return true;
        }

        /* BLTZAL */
        if((insn & 0xFC110000) == 0x04100000) {
            var rs = (insn >> 21) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            State->Registers[31] = pc + 0x8;
            if(((int) (rs switch { 0 => 0U, var temp_22 => State->Registers[temp_22] }) < 0x0 ? 1U : 0U) != 0)
                Branch(target);
            else
                Branch(pc + 0x8);
            return true;
        }

        /* BNE */
        if((insn & 0xFC000000) == 0x14000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            if((rs switch { 0 => 0U, var temp_23 => State->Registers[temp_23] } !=
                rt switch { 0 => 0U, var temp_24 => State->Registers[temp_24] }
                   ? 1U
                   : 0U) != 0)
                Branch(target);
            else
                Branch(pc + 0x8);
            return true;
        }

        return false;
    }
}