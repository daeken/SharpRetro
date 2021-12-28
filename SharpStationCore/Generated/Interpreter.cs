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
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            State->ReadAbsorb[rd] = 0;
            var temp_0 = rs switch { 0 => 0U, var temp_81 => State->Registers[temp_81] };
            var temp_1 = rt switch { 0 => 0U, var temp_82 => State->Registers[temp_82] };
            DoLds();
            var lhs = temp_0;
            var rhs = temp_1;
            var r = lhs + rhs;
            if((~(lhs ^ rhs) & (lhs ^ r) & 0x80000000U) != 0)
                throw new CpuException(ExceptionType.OV, pc, insn);
            var temp_83 = rd;
            if(temp_83 != 0)
                State->Registers[temp_83] = lhs + rhs;
            return true;
        }

        /* ADDI */
        if((insn & 0xFC000000) == 0x20000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<uint>(imm, 16);
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_2 = rs switch { 0 => 0U, var temp_84 => State->Registers[temp_84] };
            DoLds();
            var lhs = temp_2;
            var r = lhs + eimm;
            if((~(lhs ^ eimm) & (lhs ^ r) & 0x80000000U) != 0)
                throw new CpuException(ExceptionType.OV, pc, insn);
            var temp_85 = rt;
            if(temp_85 != 0)
                State->Registers[temp_85] = lhs + eimm;
            return true;
        }

        /* ADDIU */
        if((insn & 0xFC000000) == 0x24000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<uint>(imm, 16);
            State->ReadAbsorb[rt] = 0;
            State->ReadAbsorb[rs] = 0;
            var temp_3 = rs switch { 0 => 0U, var temp_86 => State->Registers[temp_86] };
            DoLds();
            var temp_87 = rt;
            if(temp_87 != 0)
                State->Registers[temp_87] = temp_3 + eimm;
            return true;
        }

        /* ADDU */
        if((insn & 0xFC00003F) == 0x00000021) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            State->ReadAbsorb[rd] = 0;
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_4 = rs switch { 0 => 0U, var temp_88 => State->Registers[temp_88] };
            var temp_5 = rt switch { 0 => 0U, var temp_89 => State->Registers[temp_89] };
            DoLds();
            var temp_90 = rd;
            if(temp_90 != 0)
                State->Registers[temp_90] = temp_4 + temp_5;
            return true;
        }

        /* AND */
        if((insn & 0xFC00003F) == 0x00000024) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            State->ReadAbsorb[rd] = 0;
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_6 = rs switch { 0 => 0U, var temp_91 => State->Registers[temp_91] };
            var temp_7 = rt switch { 0 => 0U, var temp_92 => State->Registers[temp_92] };
            DoLds();
            var temp_93 = rd;
            if(temp_93 != 0)
                State->Registers[temp_93] = temp_6 & temp_7;
            return true;
        }

        /* ANDI */
        if((insn & 0xFC000000) == 0x30000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<uint>(imm, 16);
            State->ReadAbsorb[rt] = 0;
            State->ReadAbsorb[rs] = 0;
            var temp_8 = rs switch { 0 => 0U, var temp_94 => State->Registers[temp_94] };
            DoLds();
            var temp_95 = rt;
            if(temp_95 != 0)
                State->Registers[temp_95] = temp_8 & eimm;
            return true;
        }

        /* BEQ */
        if((insn & 0xFC000000) == 0x10000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var target = pc + 4 + (SignExt<uint>(imm, 16) << 0x2);
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_9 = rs switch { 0 => 0U, var temp_96 => State->Registers[temp_96] };
            var temp_10 = rt switch { 0 => 0U, var temp_97 => State->Registers[temp_97] };
            DoLds();
            if((temp_9 == temp_10 ? 1U : 0U) != 0)
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
            State->ReadAbsorb[rs] = 0;
            var temp_11 = rs switch { 0 => 0U, var temp_98 => State->Registers[temp_98] };
            DoLds();
            if(((int) temp_11 >= 0x0 ? 1U : 0U) != 0)
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
            State->ReadAbsorb[0x1F] = 0;
            State->ReadAbsorb[rs] = 0;
            var temp_12 = rs switch { 0 => 0U, var temp_99 => State->Registers[temp_99] };
            DoLds();
            State->Registers[31] = pc + 0x8;
            if(((int) temp_12 >= 0x0 ? 1U : 0U) != 0)
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
            State->ReadAbsorb[rs] = 0;
            var temp_13 = rs switch { 0 => 0U, var temp_100 => State->Registers[temp_100] };
            DoLds();
            if(((int) temp_13 > 0x0 ? 1U : 0U) != 0)
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
            State->ReadAbsorb[rs] = 0;
            var temp_14 = rs switch { 0 => 0U, var temp_101 => State->Registers[temp_101] };
            DoLds();
            if(((int) temp_14 <= 0x0 ? 1U : 0U) != 0)
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
            State->ReadAbsorb[rs] = 0;
            var temp_15 = rs switch { 0 => 0U, var temp_102 => State->Registers[temp_102] };
            DoLds();
            if(((int) temp_15 < 0x0 ? 1U : 0U) != 0)
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
            State->ReadAbsorb[0x1F] = 0;
            State->ReadAbsorb[rs] = 0;
            var temp_16 = rs switch { 0 => 0U, var temp_103 => State->Registers[temp_103] };
            DoLds();
            State->Registers[31] = pc + 0x8;
            if(((int) temp_16 < 0x0 ? 1U : 0U) != 0)
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
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_17 = rs switch { 0 => 0U, var temp_104 => State->Registers[temp_104] };
            var temp_18 = rt switch { 0 => 0U, var temp_105 => State->Registers[temp_105] };
            DoLds();
            if((temp_17 != temp_18 ? 1U : 0U) != 0)
                Branch(target);
            else
                Branch(pc + 0x8);
            return true;
        }

        /* BREAK */
        if((insn & 0xFC00003F) == 0x0000000D) {
            var code = (insn >> 6) & 0xFFFFFU;
            DoLds();
            throw new CpuException(ExceptionType.Break, pc, insn);
            return true;
        }

        /* CFC */
        if((insn & 0xF3E00000) == 0x40400000) {
            var cop = (insn >> 26) & 0x3U;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var cofun = (insn >> 0) & 0x7FFU;
            State->ReadAbsorb[rt] = 0;
            DoLds();
            var temp_106 = rt;
            if(temp_106 != 0)
                State->Registers[temp_106] = Copcreg(cop, rd);
            return true;
        }

        /* COP */
        if((insn & 0xF2000000) == 0x42000000) {
            var cop = (insn >> 26) & 0x3U;
            var command = (insn >> 0) & 0x1FFFFFFU;
            DoLds();
            Copfun(cop, command);
            return true;
        }

        /* CTC */
        if((insn & 0xF3E00000) == 0x40C00000) {
            var cop = (insn >> 26) & 0x3U;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var cofun = (insn >> 0) & 0x7FFU;
            State->ReadAbsorb[rt] = 0;
            var temp_19 = rt switch { 0 => 0U, var temp_107 => State->Registers[temp_107] };
            DoLds();
            Copcreg(cop, rd, temp_19);
            return true;
        }

        /* DIV */
        if((insn & 0xFC00003F) == 0x0000001A) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_20 = rs switch { 0 => 0U, var temp_108 => State->Registers[temp_108] };
            var temp_21 = rt switch { 0 => 0U, var temp_109 => State->Registers[temp_109] };
            DoLds();
            var rsv = temp_20;
            var rtv = temp_21;
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
                    DivDelay();
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
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_22 = rs switch { 0 => 0U, var temp_110 => State->Registers[temp_110] };
            var temp_23 = rt switch { 0 => 0U, var temp_111 => State->Registers[temp_111] };
            DoLds();
            var rsv = temp_22;
            var rtv = temp_23;
            if((rtv == 0x0 ? 1U : 0U) != 0) {
                State->Lo = 0xFFFFFFFFU;
                State->Hi = rsv;
            }
            else {
                State->Lo = rsv / rtv;
                State->Hi = rsv % rtv;
                DivDelay();
            }

            return true;
        }

        /* J */
        if((insn & 0xFC000000) == 0x08000000) {
            var imm = (insn >> 0) & 0x3FFFFFFU;
            var target = ((pc + 4) & 0xF0000000U) +
                         (imm << 0x2);
            DoLds();
            Branch(target);
            return true;
        }

        /* JAL */
        if((insn & 0xFC000000) == 0x0C000000) {
            var imm = (insn >> 0) & 0x3FFFFFFU;
            var target = ((pc + 4) & 0xF0000000U) +
                         (imm << 0x2);
            State->ReadAbsorb[0x1F] = 0;
            DoLds();
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
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rd] = 0;
            var temp_24 = rs switch { 0 => 0U, var temp_112 => State->Registers[temp_112] };
            DoLds();
            var target = temp_24;
            var temp_113 = rd;
            if(temp_113 != 0)
                State->Registers[temp_113] = pc + 0x8;
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
            State->ReadAbsorb[rs] = 0;
            var temp_25 = rs switch { 0 => 0U, var temp_114 => State->Registers[temp_114] };
            DoLds();
            var target = temp_25;
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
            State->ReadAbsorb[rt] = 0;
            State->ReadAbsorb[rs] = 0;
            var temp_26 = rs switch { 0 => 0U, var temp_115 => State->Registers[temp_115] };
            DoLds();
            State->LdWhich = rt;
            State->LdValue = (uint) ReadMemory<sbyte>(temp_26 + (uint) offset);
            return true;
        }

        /* LBU */
        if((insn & 0xFC000000) == 0x90000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            State->ReadAbsorb[rt] = 0;
            State->ReadAbsorb[rs] = 0;
            var temp_27 = rs switch { 0 => 0U, var temp_116 => State->Registers[temp_116] };
            DoLds();
            State->LdWhich = rt;
            State->LdValue = ReadMemory<byte>(temp_27 + (uint) offset);
            return true;
        }

        /* LH */
        if((insn & 0xFC000000) == 0x84000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_28 = rs switch { 0 => 0U, var temp_117 => State->Registers[temp_117] };
            DoLds();
            var addr = temp_28 + (uint) offset;
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
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_29 = rs switch { 0 => 0U, var temp_118 => State->Registers[temp_118] };
            DoLds();
            var addr = temp_29 + (uint) offset;
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
            State->ReadAbsorb[rt] = 0;
            DoLds();
            var temp_119 = rt;
            if(temp_119 != 0)
                State->Registers[temp_119] = imm << 0x10;
            return true;
        }

        /* LW */
        if((insn & 0xFC000000) == 0x8C000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_30 = rs switch { 0 => 0U, var temp_120 => State->Registers[temp_120] };
            DoLds();
            var addr = temp_30 + (uint) offset;
            if((0x0 != (addr &
                        (32 / (uint) 0x8 -
                         0x1))
                   ? 1U
                   : 0U) != 0) throw new CpuException(ExceptionType.ADEL, pc, insn);
            State->LdWhich = rt;
            State->LdValue = ReadMemory<uint>(addr);
            return true;
        }

        /* LWL */
        if((insn & 0xFC000000) == 0x88000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_31 = rt switch { 0 => 0U, var temp_121 => State->Registers[temp_121] };
            var temp_32 = rs switch { 0 => 0U, var temp_122 => State->Registers[temp_122] };
            DoLoad(rt, ref temp_31);
            var addr = temp_32 + (uint) offset;
            var raddr = addr & 0xFFFFFFFCU;
            var ert = temp_31;
            State->LdWhich = rt;
            State->LdValue = (addr & 0x3) switch {
                0x0 => (ert & 0xFFFFFFU) |
                       ((uint) ReadMemory<byte>(raddr) <<
                        0x18),
                0x1 => (ert & 0xFFFF) |
                       ((uint) ReadMemory<ushort>(raddr) <<
                        0x10),
                0x2 => (ert & 0xFF) |
                       (ReadMemory<uint>(raddr) << 0x8),
                _ => ReadMemory<uint>(raddr),
            };
            return true;
        }

        /* LWR */
        if((insn & 0xFC000000) == 0x98000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_33 = rt switch { 0 => 0U, var temp_124 => State->Registers[temp_124] };
            var temp_34 = rs switch { 0 => 0U, var temp_125 => State->Registers[temp_125] };
            DoLoad(rt, ref temp_33);
            var addr = temp_34 + (uint) offset;
            var raddr = addr & 0xFFFFFFFCU;
            var ert = temp_33;
            State->LdWhich = rt;
            State->LdValue = (addr & 0x3) switch {
                0x0 => ReadMemory<uint>(raddr),
                0x1 => (ert & 0xFF000000U) |
                       (ReadMemory<uint>(addr) & 0xFFFFFFU),
                0x2 => (ert & 0xFFFF0000U) |
                       ReadMemory<ushort>(addr),
                _ => (ert & 0xFFFFFF00U) | ReadMemory<byte>(addr),
            };
            return true;
        }

        /* LWC2 */
        if((insn & 0xFC000000) == 0xC8000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            State->ReadAbsorb[rs] = 0;
            var temp_35 = rs switch { 0 => 0U, var temp_127 => State->Registers[temp_127] };
            DoLds();
            var addr = temp_35 + (uint) offset;
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
            State->ReadAbsorb[rt] = 0;
            DoLds();
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
            State->ReadAbsorb[rd] = 0;
            DoLds();
            var temp_128 = rd;
            if(temp_128 != 0)
                State->Registers[temp_128] = State->Hi;
            AbsorbMuldivDelay();
            return true;
        }

        /* MFLO */
        if((insn & 0xFC00003F) == 0x00000012) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            State->ReadAbsorb[rd] = 0;
            DoLds();
            var temp_129 = rd;
            if(temp_129 != 0)
                State->Registers[temp_129] = State->Lo;
            AbsorbMuldivDelay();
            return true;
        }

        /* MTC */
        if((insn & 0xF3E00000) == 0x40800000) {
            var cop = (insn >> 26) & 0x3U;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var cofun = (insn >> 0) & 0x7FFU;
            State->ReadAbsorb[rt] = 0;
            var temp_36 = rt switch { 0 => 0U, var temp_130 => State->Registers[temp_130] };
            DoLds();
            Copreg(cop, rd, temp_36);
            return true;
        }

        /* MTHI */
        if((insn & 0xFC00003F) == 0x00000011) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            State->ReadAbsorb[rs] = 0;
            var temp_37 = rs switch { 0 => 0U, var temp_131 => State->Registers[temp_131] };
            DoLds();
            State->Hi = temp_37;
            return true;
        }

        /* MTLO */
        if((insn & 0xFC00003F) == 0x00000013) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            State->ReadAbsorb[rs] = 0;
            var temp_38 = rs switch { 0 => 0U, var temp_132 => State->Registers[temp_132] };
            DoLds();
            State->Lo = temp_38;
            return true;
        }

        /* MULT */
        if((insn & 0xFC00003F) == 0x00000018) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_39 = rs switch { 0 => 0U, var temp_133 => State->Registers[temp_133] };
            var temp_40 = rt switch { 0 => 0U, var temp_134 => State->Registers[temp_134] };
            DoLds();
            var lhs = temp_39;
            var rhs = temp_40;
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
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_41 = rs switch { 0 => 0U, var temp_135 => State->Registers[temp_135] };
            var temp_42 = rt switch { 0 => 0U, var temp_136 => State->Registers[temp_136] };
            DoLds();
            var lhs = temp_41;
            var rhs = temp_42;
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
            State->ReadAbsorb[rd] = 0;
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_43 = rs switch { 0 => 0U, var temp_137 => State->Registers[temp_137] };
            var temp_44 = rt switch { 0 => 0U, var temp_138 => State->Registers[temp_138] };
            DoLds();
            var temp_139 = rd;
            if(temp_139 != 0)
                State->Registers[temp_139] = ~(temp_43 | temp_44);
            return true;
        }

        /* OR */
        if((insn & 0xFC00003F) == 0x00000025) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            State->ReadAbsorb[rd] = 0;
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_45 = rs switch { 0 => 0U, var temp_140 => State->Registers[temp_140] };
            var temp_46 = rt switch { 0 => 0U, var temp_141 => State->Registers[temp_141] };
            DoLds();
            var temp_142 = rd;
            if(temp_142 != 0)
                State->Registers[temp_142] = temp_45 | temp_46;
            return true;
        }

        /* ORI */
        if((insn & 0xFC000000) == 0x34000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            State->ReadAbsorb[rt] = 0;
            State->ReadAbsorb[rs] = 0;
            var temp_47 = rs switch { 0 => 0U, var temp_143 => State->Registers[temp_143] };
            DoLds();
            var temp_144 = rt;
            if(temp_144 != 0)
                State->Registers[temp_144] = temp_47 | imm;
            return true;
        }

        /* SB */
        if((insn & 0xFC000000) == 0xA0000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_48 = rs switch { 0 => 0U, var temp_145 => State->Registers[temp_145] };
            var temp_49 = rt switch { 0 => 0U, var temp_146 => State->Registers[temp_146] };
            DoLds();
            WriteMemory(temp_48 + (uint) offset, (byte) temp_49);
            return true;
        }

        /* SH */
        if((insn & 0xFC000000) == 0xA4000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_50 = rs switch { 0 => 0U, var temp_147 => State->Registers[temp_147] };
            var temp_51 = rt switch { 0 => 0U, var temp_148 => State->Registers[temp_148] };
            DoLds();
            var addr = temp_50 + (uint) offset;
            if((0x0 != (addr &
                        (16 / (uint) 0x8 -
                         0x1))
                   ? 1U
                   : 0U) != 0) throw new CpuException(ExceptionType.ADES, pc, insn);
            WriteMemory(addr, (ushort) temp_51);
            return true;
        }

        /* SLL */
        if((insn & 0xFC00003F) == 0x00000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            State->ReadAbsorb[rd] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_52 = rt switch { 0 => 0U, var temp_149 => State->Registers[temp_149] };
            DoLds();
            var temp_150 = rd;
            if(temp_150 != 0)
                State->Registers[temp_150] = temp_52 << (int) shamt;
            return true;
        }

        /* SLLV */
        if((insn & 0xFC00003F) == 0x00000004) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            State->ReadAbsorb[rd] = 0;
            State->ReadAbsorb[rt] = 0;
            State->ReadAbsorb[rs] = 0;
            var temp_53 = rt switch { 0 => 0U, var temp_151 => State->Registers[temp_151] };
            var temp_54 = rs switch { 0 => 0U, var temp_152 => State->Registers[temp_152] };
            DoLds();
            var temp_153 = rd;
            if(temp_153 != 0)
                State->Registers[temp_153] = temp_53 << (int) temp_54;
            return true;
        }

        /* SLT */
        if((insn & 0xFC00003F) == 0x0000002A) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            State->ReadAbsorb[rd] = 0;
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_55 = rs switch { 0 => 0U, var temp_154 => State->Registers[temp_154] };
            var temp_56 = rt switch { 0 => 0U, var temp_155 => State->Registers[temp_155] };
            DoLds();
            var temp_156 = rd;
            if(temp_156 != 0)
                State->Registers[temp_156] = (int) temp_55 < (int) temp_56 ? 1U : 0U;
            return true;
        }

        /* SLTI */
        if((insn & 0xFC000000) == 0x28000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<int>(imm, 16);
            State->ReadAbsorb[rt] = 0;
            State->ReadAbsorb[rs] = 0;
            var temp_57 = rs switch { 0 => 0U, var temp_157 => State->Registers[temp_157] };
            DoLds();
            var temp_158 = rt;
            if(temp_158 != 0)
                State->Registers[temp_158] = (int) temp_57 < eimm ? 1U : 0U;
            return true;
        }

        /* SLTIU */
        if((insn & 0xFC000000) == 0x2C000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var eimm = SignExt<uint>(imm, 16);
            State->ReadAbsorb[rt] = 0;
            State->ReadAbsorb[rs] = 0;
            var temp_58 = rs switch { 0 => 0U, var temp_159 => State->Registers[temp_159] };
            DoLds();
            var temp_160 = rt;
            if(temp_160 != 0)
                State->Registers[temp_160] = temp_58 < eimm ? 1U : 0U;
            return true;
        }

        /* SLTU */
        if((insn & 0xFC00003F) == 0x0000002B) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            State->ReadAbsorb[rd] = 0;
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_59 = rs switch { 0 => 0U, var temp_161 => State->Registers[temp_161] };
            var temp_60 = rt switch { 0 => 0U, var temp_162 => State->Registers[temp_162] };
            DoLds();
            var temp_163 = rd;
            if(temp_163 != 0)
                State->Registers[temp_163] = temp_59 < temp_60 ? 1U : 0U;
            return true;
        }

        /* SRA */
        if((insn & 0xFC00003F) == 0x00000003) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            State->ReadAbsorb[rd] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_61 = rt switch { 0 => 0U, var temp_164 => State->Registers[temp_164] };
            DoLds();
            var temp_165 = rd;
            if(temp_165 != 0)
                State->Registers[temp_165] = (uint) ((int) temp_61 >> (int) shamt);
            return true;
        }

        /* SRAV */
        if((insn & 0xFC00003F) == 0x00000007) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            State->ReadAbsorb[rd] = 0;
            State->ReadAbsorb[rt] = 0;
            State->ReadAbsorb[rs] = 0;
            var temp_62 = rt switch { 0 => 0U, var temp_166 => State->Registers[temp_166] };
            var temp_63 = rs switch { 0 => 0U, var temp_167 => State->Registers[temp_167] };
            DoLds();
            var temp_168 = rd;
            if(temp_168 != 0)
                State->Registers[temp_168] = (uint) ((int) temp_62 >> (int) temp_63);
            return true;
        }

        /* SRL */
        if((insn & 0xFC00003F) == 0x00000002) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            State->ReadAbsorb[rd] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_64 = rt switch { 0 => 0U, var temp_169 => State->Registers[temp_169] };
            DoLds();
            var temp_170 = rd;
            if(temp_170 != 0)
                State->Registers[temp_170] = temp_64 >> (int) shamt;
            return true;
        }

        /* SRLV */
        if((insn & 0xFC00003F) == 0x00000006) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            State->ReadAbsorb[rd] = 0;
            State->ReadAbsorb[rt] = 0;
            State->ReadAbsorb[rs] = 0;
            var temp_65 = rt switch { 0 => 0U, var temp_171 => State->Registers[temp_171] };
            var temp_66 = rs switch { 0 => 0U, var temp_172 => State->Registers[temp_172] };
            DoLds();
            var temp_173 = rd;
            if(temp_173 != 0)
                State->Registers[temp_173] = temp_65 >> (int) temp_66;
            return true;
        }

        /* SUB */
        if((insn & 0xFC00003F) == 0x00000022) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            State->ReadAbsorb[rd] = 0;
            var temp_67 = rs switch { 0 => 0U, var temp_174 => State->Registers[temp_174] };
            var temp_68 = rt switch { 0 => 0U, var temp_175 => State->Registers[temp_175] };
            DoLds();
            var lhs = temp_67;
            var rhs = temp_68;
            var r = lhs - rhs;
            if(((lhs ^ rhs) & (lhs ^ r) & 0x80000000U) != 0)
                throw new CpuException(ExceptionType.OV, pc, insn);
            var temp_176 = rd;
            if(temp_176 != 0)
                State->Registers[temp_176] = lhs - rhs;
            return true;
        }

        /* SUBU */
        if((insn & 0xFC00003F) == 0x00000023) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            State->ReadAbsorb[rd] = 0;
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_69 = rs switch { 0 => 0U, var temp_177 => State->Registers[temp_177] };
            var temp_70 = rt switch { 0 => 0U, var temp_178 => State->Registers[temp_178] };
            DoLds();
            var temp_179 = rd;
            if(temp_179 != 0)
                State->Registers[temp_179] = temp_69 - temp_70;
            return true;
        }

        /* SW */
        if((insn & 0xFC000000) == 0xAC000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_71 = rs switch { 0 => 0U, var temp_180 => State->Registers[temp_180] };
            var temp_72 = rt switch { 0 => 0U, var temp_181 => State->Registers[temp_181] };
            DoLds();
            var addr = temp_71 + (uint) offset;
            if((0x0 != (addr &
                        (32 / (uint) 0x8 -
                         0x1))
                   ? 1U
                   : 0U) != 0) throw new CpuException(ExceptionType.ADES, pc, insn);
            WriteMemory(addr, temp_72);
            return true;
        }

        /* SWC2 */
        if((insn & 0xFC000000) == 0xE8000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            State->ReadAbsorb[rs] = 0;
            var temp_73 = rs switch { 0 => 0U, var temp_182 => State->Registers[temp_182] };
            DoLds();
            var addr = temp_73 + (uint) offset;
            if((0x0 != (addr &
                        (32 / (uint) 0x8 -
                         0x1))
                   ? 1U
                   : 0U) != 0) throw new CpuException(ExceptionType.ADES, pc, insn);
            WriteMemory(addr, Copreg(0x2, rt));
            return true;
        }

        /* SWL */
        if((insn & 0xFC000000) == 0xA8000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_74 = rs switch { 0 => 0U, var temp_183 => State->Registers[temp_183] };
            var temp_75 = rt switch { 0 => 0U, var temp_184 => State->Registers[temp_184] };
            DoLds();
            var addr = temp_74 + (uint) offset;
            var raddr = addr & 0xFFFFFFFCU;
            var rtv = temp_75;
            switch(addr & 0x3) {
                case 0x0: {
                    WriteMemory(raddr, (byte) (rtv >> 0x18));
                    break;
                }
                case 0x1: {
                    WriteMemory(raddr, (ushort) (rtv >> 0x10));
                    break;
                }
                case 0x3: {
                    WriteMemory(raddr, rtv);
                    break;
                }
                default: {
                    WriteMemory(raddr, (ushort) (rtv >> 0x8));
                    WriteMemory(raddr + 0x2, (byte) (rtv >> 0x18));
                    break;
                }
            }

            return true;
        }

        /* SWR */
        if((insn & 0xFC000000) == 0xB8000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            var offset = SignExt<int>(imm, 16);
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_76 = rs switch { 0 => 0U, var temp_185 => State->Registers[temp_185] };
            var temp_77 = rt switch { 0 => 0U, var temp_186 => State->Registers[temp_186] };
            DoLds();
            var addr = temp_76 + (uint) offset;
            var raddr = addr & 0xFFFFFFFCU;
            var rtv = temp_77;
            switch(addr & 0x3) {
                case 0x0: {
                    WriteMemory(raddr, rtv);
                    break;
                }
                case 0x2: {
                    WriteMemory(raddr, (ushort) rtv);
                    break;
                }
                case 0x3: {
                    WriteMemory(raddr, (byte) rtv);
                    break;
                }
                default: {
                    WriteMemory(raddr, (ushort) rtv);
                    WriteMemory(raddr + 0x2, (byte) (rtv >> 0x10));
                    break;
                }
            }

            return true;
        }

        /* SYSCALL */
        if((insn & 0xFC00003F) == 0x0000000C) {
            var code = (insn >> 6) & 0xFFFFFU;
            DoLds();
            throw new CpuException(ExceptionType.Syscall, pc, insn);
            return true;
        }

        /* XOR */
        if((insn & 0xFC00003F) == 0x00000026) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var rd = (insn >> 11) & 0x1FU;
            var shamt = (insn >> 6) & 0x1FU;
            State->ReadAbsorb[rd] = 0;
            State->ReadAbsorb[rs] = 0;
            State->ReadAbsorb[rt] = 0;
            var temp_78 = rs switch { 0 => 0U, var temp_187 => State->Registers[temp_187] };
            var temp_79 = rt switch { 0 => 0U, var temp_188 => State->Registers[temp_188] };
            DoLds();
            var temp_189 = rd;
            if(temp_189 != 0)
                State->Registers[temp_189] = temp_78 ^ temp_79;
            return true;
        }

        /* XORI */
        if((insn & 0xFC000000) == 0x38000000) {
            var rs = (insn >> 21) & 0x1FU;
            var rt = (insn >> 16) & 0x1FU;
            var imm = (insn >> 0) & 0xFFFFU;
            State->ReadAbsorb[rt] = 0;
            State->ReadAbsorb[rs] = 0;
            var temp_80 = rs switch { 0 => 0U, var temp_190 => State->Registers[temp_190] };
            DoLds();
            var temp_191 = rt;
            if(temp_191 != 0)
                State->Registers[temp_191] = temp_80 ^ imm;
            return true;
        }

        return false;
    }
}