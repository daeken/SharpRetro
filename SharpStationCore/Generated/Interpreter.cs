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

        /* BREAK */
        if((insn & 0xFC00003F) == 0x0000000D) {
            var code = (insn >> 6) & 0xFFFFFU;
            throw new CpuException(ExceptionType.Break, pc, insn);
            return true;
        }

        /* CFC */
        if((insn & 0xF3E00000) == 0x40400000) {
            var cop = (insn >> 26) & 0x3U;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var cofun = (insn >> 0) & 0x7FFU;
            var temp_25 = rt;
            if(temp_25 != 0)
                State->Registers[temp_25] = Copcreg(cop, rd);
            return true;
        }

        /* COP */
        if((insn & 0xF2000000) == 0x42000000) {
            var cop = (insn >> 26) & 0x3U;
            var command = (insn >> 0) & 0x1FFFFFFU;
            Copfun(cop, command);
            return true;
        }

        /* CTC */
        if((insn & 0xF3E00000) == 0x40C00000) {
            var cop = (insn >> 26) & 0x3U;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var cofun = (insn >> 0) & 0x7FFU;
            Copcreg(cop, rd, rt switch { 0 => 0U, var temp_26 => State->Registers[temp_26] });
            return true;
        }

        /* DIV */
        if((insn & 0xFC00003F) == 0x0000001A) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var rsv = rs switch { 0 => 0U, var temp_27 => State->Registers[temp_27] };
            var rtv = rt switch { 0 => 0U, var temp_28 => State->Registers[temp_28] };
            if((rtv == 0x0 ? 1U : 0U) != 0) {
                State->Lo = (byte) (((rsv & 0x80000000U) != 0x0 ? 1U : 0U) != 0
                    ? (uint) 0x1
                    : 0xFFFFFFFFU);
                State->Hi = rsv;
            }
            else {
                if(((rsv == 0x80000000U ? 1U : 0U) & (rtv == 0xFFFFFFFFU ? 1U : 0U)) != 0) {
                    State->Lo = 0x80000000U;
                    State->Hi = 0x0;
                }
                else {
                    State->Lo = (uint) ((int) rsv / (int) rtv);
                    State->Hi = (uint) ((int) rsv % (int) rtv);
                }
            }

            return true;
        }

        /* DIVU */
        if((insn & 0xFC00003F) == 0x0000001B) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var rsv = rs switch { 0 => 0U, var temp_29 => State->Registers[temp_29] };
            var rtv = rt switch { 0 => 0U, var temp_30 => State->Registers[temp_30] };
            if((rtv == 0x0 ? 1U : 0U) != 0) {
                State->Lo = 0xFFFFFFFFU;
                State->Hi = rsv;
            }
            else {
                State->Lo = rsv / rtv;
                State->Hi = rsv % rtv;
            }

            return true;
        }

        /* J */
        if((insn & 0xFC000000) == 0x08000000) {
            var imm = (insn >> 0) & 0x3FFFFFFU;
            var target = ((pc + 4) & 0xF0000000U) +
                         (imm << 0x2);
            Branch(target);
            return true;
        }

        /* JAL */
        if((insn & 0xFC000000) == 0x0C000000) {
            var imm = (insn >> 0) & 0x3FFFFFFU;
            var target = ((pc + 4) & 0xF0000000U) +
                         (imm << 0x2);
            State->Registers[31] = pc + 0x8;
            Branch(target);
            return true;
        }

        /* JALR */
        if((insn & 0xFC00003F) == 0x00000009) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var target = rs switch { 0 => 0U, var temp_31 => State->Registers[temp_31] };
            var temp_32 = rd;
            if(temp_32 != 0)
                State->Registers[temp_32] = pc + 0x8;
            if((0x0 != (target &
                        (32 / (uint) 0x8 -
                         0x1))
                   ? 1U
                   : 0U) != 0) throw new CpuException(ExceptionType.ADEL, pc, insn);
            Branch(target);
            return true;
        }

        /* JR */
        if((insn & 0xFC00003F) == 0x00000008) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var target = rs switch { 0 => 0U, var temp_33 => State->Registers[temp_33] };
            if((0x0 != (target &
                        (32 / (uint) 0x8 -
                         0x1))
                   ? 1U
                   : 0U) != 0) throw new CpuException(ExceptionType.ADEL, pc, insn);
            Branch(target);
            return true;
        }

        /* LB */
        if((insn & 0xFC000000) == 0x80000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            State->LdWhich = rt;
            State->LdValue =
                (uint) ReadMemory<sbyte>(rs switch { 0 => 0U, var temp_34 => State->Registers[temp_34] } +
                                         (uint) offset);
            return true;
        }

        /* LBU */
        if((insn & 0xFC000000) == 0x90000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            State->LdWhich = rt;
            State->LdValue =
                ReadMemory<byte>(rs switch { 0 => 0U, var temp_35 => State->Registers[temp_35] } +
                                 (uint) offset);
            return true;
        }

        /* LH */
        if((insn & 0xFC000000) == 0x84000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            var addr = rs switch { 0 => 0U, var temp_36 => State->Registers[temp_36] } + (uint) offset;
            if((0x0 != (addr &
                        (16 / (uint) 0x8 -
                         0x1))
                   ? 1U
                   : 0U) != 0) throw new CpuException(ExceptionType.ADEL, pc, insn);
            State->LdWhich = rt;
            State->LdValue = (uint) ReadMemory<short>(addr);
            return true;
        }

        /* LHU */
        if((insn & 0xFC000000) == 0x94000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            var addr = rs switch { 0 => 0U, var temp_37 => State->Registers[temp_37] } + (uint) offset;
            if((0x0 != (addr &
                        (16 / (uint) 0x8 -
                         0x1))
                   ? 1U
                   : 0U) != 0) throw new CpuException(ExceptionType.ADEL, pc, insn);
            State->LdWhich = rt;
            State->LdValue = ReadMemory<ushort>(addr);
            return true;
        }

        /* LUI */
        if((insn & 0xFC000000) == 0x3C000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var temp_38 = rt;
            if(temp_38 != 0)
                State->Registers[temp_38] = (ushort) (imm << 0x10);
            return true;
        }

        /* LW */
        if((insn & 0xFC000000) == 0x8C000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            var addr = rs switch { 0 => 0U, var temp_39 => State->Registers[temp_39] } + (uint) offset;
            if((0x0 != (addr &
                        (32 / (uint) 0x8 -
                         0x1))
                   ? 1U
                   : 0U) != 0) throw new CpuException(ExceptionType.ADEL, pc, insn);
            State->LdWhich = rt;
            State->LdValue = ReadMemory<uint>(addr);
            return true;
        }

        /* LWC2 */
        if((insn & 0xFC000000) == 0xC8000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            var addr = rs switch { 0 => 0U, var temp_40 => State->Registers[temp_40] } + (uint) offset;
            if((0x0 != (addr &
                        (32 / (uint) 0x8 -
                         0x1))
                   ? 1U
                   : 0U) != 0) throw new CpuException(ExceptionType.ADEL, pc, insn);
            Copreg(0x2, rt, ReadMemory<uint>(addr));
            return true;
        }

        /* MFC */
        if((insn & 0xF3E00000) == 0x40000000) {
            var cop = (insn >> 26) & 0x3U;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var cofun = (insn >> 0) & 0x7FFU;
            State->LdWhich = rt;
            State->LdValue = Copreg(cop, rd);
            return true;
        }

        /* MFHI */
        if((insn & 0xFC00003F) == 0x00000010) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var temp_41 = rd;
            if(temp_41 != 0)
                State->Registers[temp_41] = State->Hi;
            AbsorbMuldivDelay();
            return true;
        }

        /* MFLO */
        if((insn & 0xFC00003F) == 0x00000012) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var temp_42 = rd;
            if(temp_42 != 0)
                State->Registers[temp_42] = State->Lo;
            AbsorbMuldivDelay();
            return true;
        }

        /* MTC */
        if((insn & 0xF3E00000) == 0x40800000) {
            var cop = (insn >> 26) & 0x3U;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var cofun = (insn >> 0) & 0x7FFU;
            Copreg(cop, rd, rt switch { 0 => 0U, var temp_43 => State->Registers[temp_43] });
            return true;
        }

        /* MTHI */
        if((insn & 0xFC00003F) == 0x00000011) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            State->Hi = rs switch { 0 => 0U, var temp_44 => State->Registers[temp_44] };
            return true;
        }

        /* MTLO */
        if((insn & 0xFC00003F) == 0x00000013) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            State->Lo = rs switch { 0 => 0U, var temp_45 => State->Registers[temp_45] };
            return true;
        }

        /* MULT */
        if((insn & 0xFC00003F) == 0x00000018) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var lhs = rs switch { 0 => 0U, var temp_46 => State->Registers[temp_46] };
            var rhs = rt switch { 0 => 0U, var temp_47 => State->Registers[temp_47] };
            var result = (ulong) (lhs * (long) rhs);
            State->Lo = (uint) result;
            State->Hi = (uint) (result >> 0x20);
            MulDelay(lhs, rhs, 0x1 != 0);
            return true;
        }

        /* MULTU */
        if((insn & 0xFC00003F) == 0x00000019) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var lhs = rs switch { 0 => 0U, var temp_48 => State->Registers[temp_48] };
            var rhs = rt switch { 0 => 0U, var temp_49 => State->Registers[temp_49] };
            var result = lhs * (ulong) rhs;
            State->Lo = (uint) result;
            State->Hi = (uint) (result >> 0x20);
            MulDelay(lhs, rhs, 0x0 != 0);
            return true;
        }

        /* NOR */
        if((insn & 0xFC00003F) == 0x00000027) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var temp_50 = rd;
            if(temp_50 != 0)
                State->Registers[temp_50] =
                    ~(rs switch { 0 => 0U, var temp_51 => State->Registers[temp_51] } |
                      rt switch { 0 => 0U, var temp_52 => State->Registers[temp_52] });
            return true;
        }

        /* OR */
        if((insn & 0xFC00003F) == 0x00000025) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var temp_53 = rd;
            if(temp_53 != 0)
                State->Registers[temp_53] = rs switch { 0 => 0U, var temp_54 => State->Registers[temp_54] } |
                                            rt switch { 0 => 0U, var temp_55 => State->Registers[temp_55] };
            return true;
        }

        /* ORI */
        if((insn & 0xFC000000) == 0x34000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var temp_56 = rt;
            if(temp_56 != 0)
                State->Registers[temp_56] =
                    rs switch { 0 => 0U, var temp_57 => State->Registers[temp_57] } | imm;
            return true;
        }

        /* SB */
        if((insn & 0xFC000000) == 0xA0000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            WriteMemory(rs switch { 0 => 0U, var temp_58 => State->Registers[temp_58] } + (uint) offset,
                (byte) (rt switch { 0 => 0U, var temp_59 => State->Registers[temp_59] }));
            return true;
        }

        /* SH */
        if((insn & 0xFC000000) == 0xA4000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            var addr = rs switch { 0 => 0U, var temp_60 => State->Registers[temp_60] } + (uint) offset;
            if((0x0 != (addr &
                        (16 / (uint) 0x8 -
                         0x1))
                   ? 1U
                   : 0U) != 0) throw new CpuException(ExceptionType.ADES, pc, insn);
            WriteMemory(addr, (ushort) (rt switch { 0 => 0U, var temp_61 => State->Registers[temp_61] }));
            return true;
        }

        /* SLL */
        if((insn & 0xFC00003F) == 0x00000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var temp_62 = rd;
            if(temp_62 != 0)
                State->Registers[temp_62] =
                    rt switch { 0 => 0U, var temp_63 => State->Registers[temp_63] } << (int) shamt;
            return true;
        }

        /* SLLV */
        if((insn & 0xFC00003F) == 0x00000004) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var temp_64 = rd;
            if(temp_64 != 0)
                State->Registers[temp_64] = rt switch { 0 => 0U, var temp_65 => State->Registers[temp_65] } <<
                                            (int) (rs switch { 0 => 0U, var temp_66 => State->Registers[temp_66] });
            return true;
        }

        /* SLT */
        if((insn & 0xFC00003F) == 0x0000002A) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var temp_67 = rd;
            if(temp_67 != 0)
                State->Registers[temp_67] =
                    (int) (rs switch { 0 => 0U, var temp_68 => State->Registers[temp_68] }) <
                    (int) (rt switch { 0 => 0U, var temp_69 => State->Registers[temp_69] })
                        ? 1U
                        : 0U;
            return true;
        }

        /* SLTI */
        if((insn & 0xFC000000) == 0x28000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<int>(imm, 16);
            var temp_70 = rt;
            if(temp_70 != 0)
                State->Registers[temp_70] =
                    (int) (rs switch { 0 => 0U, var temp_71 => State->Registers[temp_71] }) < eimm ? 1U : 0U;
            return true;
        }

        /* SLTIU */
        if((insn & 0xFC000000) == 0x2C000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<uint>(imm, 16);
            var temp_72 = rt;
            if(temp_72 != 0)
                State->Registers[temp_72] =
                    rs switch { 0 => 0U, var temp_73 => State->Registers[temp_73] } < eimm ? 1U : 0U;
            return true;
        }

        /* SLTU */
        if((insn & 0xFC00003F) == 0x0000002B) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var temp_74 = rd;
            if(temp_74 != 0)
                State->Registers[temp_74] = rs switch { 0 => 0U, var temp_75 => State->Registers[temp_75] } <
                                            rt switch { 0 => 0U, var temp_76 => State->Registers[temp_76] }
                    ? 1U
                    : 0U;
            return true;
        }

        /* SRA */
        if((insn & 0xFC00003F) == 0x00000003) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var temp_77 = rd;
            if(temp_77 != 0)
                State->Registers[temp_77] =
                    (uint) ((int) (rt switch { 0 => 0U, var temp_78 => State->Registers[temp_78] }) >> (int) shamt);
            return true;
        }

        /* SRAV */
        if((insn & 0xFC00003F) == 0x00000007) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var temp_79 = rd;
            if(temp_79 != 0)
                State->Registers[temp_79] =
                    (uint) ((int) (rt switch { 0 => 0U, var temp_80 => State->Registers[temp_80] }) >>
                            (int) (rs switch { 0 => 0U, var temp_81 => State->Registers[temp_81] }));
            return true;
        }

        /* SRL */
        if((insn & 0xFC00003F) == 0x00000002) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var temp_82 = rd;
            if(temp_82 != 0)
                State->Registers[temp_82] =
                    rt switch { 0 => 0U, var temp_83 => State->Registers[temp_83] } >> (int) shamt;
            return true;
        }

        /* SRLV */
        if((insn & 0xFC00003F) == 0x00000006) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var temp_84 = rd;
            if(temp_84 != 0)
                State->Registers[temp_84] = rt switch { 0 => 0U, var temp_85 => State->Registers[temp_85] } >>
                                            (int) (rs switch { 0 => 0U, var temp_86 => State->Registers[temp_86] });
            return true;
        }

        /* SUB */
        if((insn & 0xFC00003F) == 0x00000022) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var lhs = rs switch { 0 => 0U, var temp_87 => State->Registers[temp_87] };
            var rhs = rt switch { 0 => 0U, var temp_88 => State->Registers[temp_88] };
            var temp_89 = rd;
            if(temp_89 != 0)
                State->Registers[temp_89] = lhs - rhs;
            return true;
        }

        /* SUBU */
        if((insn & 0xFC00003F) == 0x00000023) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var temp_90 = rd;
            if(temp_90 != 0)
                State->Registers[temp_90] =
                    rs switch { 0 => 0U, var temp_91 => State->Registers[temp_91] } -
                    rt switch { 0 => 0U, var temp_92 => State->Registers[temp_92] };
            return true;
        }

        /* SW */
        if((insn & 0xFC000000) == 0xAC000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            var addr = rs switch { 0 => 0U, var temp_93 => State->Registers[temp_93] } + (uint) offset;
            if((0x0 != (addr &
                        (32 / (uint) 0x8 -
                         0x1))
                   ? 1U
                   : 0U) != 0) throw new CpuException(ExceptionType.ADES, pc, insn);
            WriteMemory(addr, rt switch { 0 => 0U, var temp_94 => State->Registers[temp_94] });
            return true;
        }

        /* SWC2 */
        if((insn & 0xFC000000) == 0xE8000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            var addr = rs switch { 0 => 0U, var temp_95 => State->Registers[temp_95] } + (uint) offset;
            if((0x0 != (addr &
                        (32 / (uint) 0x8 -
                         0x1))
                   ? 1U
                   : 0U) != 0) throw new CpuException(ExceptionType.ADES, pc, insn);
            WriteMemory(addr, Copreg(0x2, rt));
            return true;
        }

        /* SYSCALL */
        if((insn & 0xFC00003F) == 0x0000000C) {
            var code = (insn >> 6) & 0xFFFFFU;
            throw new CpuException(ExceptionType.Syscall, pc, insn);
            return true;
        }

        /* XOR */
        if((insn & 0xFC00003F) == 0x00000026) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            var temp_96 = rd;
            if(temp_96 != 0)
                State->Registers[temp_96] = rs switch { 0 => 0U, var temp_97 => State->Registers[temp_97] } ^
                                            rt switch { 0 => 0U, var temp_98 => State->Registers[temp_98] };
            return true;
        }

        /* XORI */
        if((insn & 0xFC000000) == 0x38000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var temp_99 = rt;
            if(temp_99 != 0)
                State->Registers[temp_99] =
                    rs switch { 0 => 0U, var temp_100 => State->Registers[temp_100] } ^ imm;
            return true;
        }

        return false;
    }
}