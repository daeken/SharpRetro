// ReSharper disable CheckNamespace
namespace SharpStationCore;
using static LibSharpRetro.CpuHelpers.Math;

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
			var temp_0 = (uint) ((rs) switch { 0 => 0U, var temp_81 => State->Registers[temp_81] });
			var temp_1 = (uint) ((rt) switch { 0 => 0U, var temp_82 => State->Registers[temp_82] });
			DoLds();
			var lhs = temp_0;
			var rhs = temp_1;
			var r = (uint) ((uint) (((uint) (uint) ((uint) (lhs))) + ((uint) (uint) ((uint) (rhs)))));
			if((bool) (((uint) (((((uint) ((uint) ((uint) (~((uint) ((((uint) ((uint) (lhs))) ^ ((uint) ((uint) (rhs)))))))))) & ((uint) ((uint) ((uint) ((((uint) ((uint) (lhs))) ^ ((uint) ((uint) (r))))))))) & ((uint) ((uint) (0x80000000U)))))) != 0)) {
				throw new CpuException(ExceptionType.OV, pc, insn);
			}
			var temp_83 = rd;
			if(temp_83 != 0)
				State->Registers[temp_83] = (uint) ((uint) (((uint) (uint) ((uint) (lhs))) + ((uint) (uint) ((uint) (rhs)))));
			return true;
		}
		insn_1:
		/* ADDI */
		if((insn & 0xFC000000) == 0x20000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var eimm = (uint) (SignExt<uint>(imm, 16));
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_2 = (uint) ((rs) switch { 0 => 0U, var temp_84 => State->Registers[temp_84] });
			DoLds();
			var lhs = temp_2;
			var r = (uint) ((uint) (((uint) (uint) ((uint) (lhs))) + ((uint) (uint) ((uint) (eimm)))));
			if((bool) (((uint) (((((uint) ((uint) ((uint) (~((uint) ((((uint) ((uint) (lhs))) ^ ((uint) ((uint) (eimm)))))))))) & ((uint) ((uint) ((uint) ((((uint) ((uint) (lhs))) ^ ((uint) ((uint) (r))))))))) & ((uint) ((uint) (0x80000000U)))))) != 0)) {
				throw new CpuException(ExceptionType.OV, pc, insn);
			}
			var temp_85 = rt;
			if(temp_85 != 0)
				State->Registers[temp_85] = (uint) ((uint) (((uint) (uint) ((uint) (lhs))) + ((uint) (uint) ((uint) (eimm)))));
			return true;
		}
		insn_2:
		/* ADDIU */
		if((insn & 0xFC000000) == 0x24000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var eimm = (uint) (SignExt<uint>(imm, 16));
			State->ReadAbsorb[rt] = 0;
			State->ReadAbsorb[rs] = 0;
			var temp_3 = (uint) ((rs) switch { 0 => 0U, var temp_86 => State->Registers[temp_86] });
			DoLds();
			var temp_87 = rt;
			if(temp_87 != 0)
				State->Registers[temp_87] = (uint) ((uint) (((uint) (uint) ((uint) (temp_3))) + ((uint) (uint) ((uint) (eimm)))));
			return true;
		}
		insn_3:
		/* ADDU */
		if((insn & 0xFC00003F) == 0x00000021) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rd] = 0;
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_4 = (uint) ((rs) switch { 0 => 0U, var temp_88 => State->Registers[temp_88] });
			var temp_5 = (uint) ((rt) switch { 0 => 0U, var temp_89 => State->Registers[temp_89] });
			DoLds();
			var temp_90 = rd;
			if(temp_90 != 0)
				State->Registers[temp_90] = (uint) ((uint) (((uint) (uint) ((uint) (temp_4))) + ((uint) (uint) ((uint) (temp_5)))));
			return true;
		}
		insn_4:
		/* AND */
		if((insn & 0xFC00003F) == 0x00000024) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rd] = 0;
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_6 = (uint) ((rs) switch { 0 => 0U, var temp_91 => State->Registers[temp_91] });
			var temp_7 = (uint) ((rt) switch { 0 => 0U, var temp_92 => State->Registers[temp_92] });
			DoLds();
			var temp_93 = rd;
			if(temp_93 != 0)
				State->Registers[temp_93] = (uint) ((uint) ((((uint) ((uint) (temp_6))) & ((uint) ((uint) (temp_7))))));
			return true;
		}
		insn_5:
		/* ANDI */
		if((insn & 0xFC000000) == 0x30000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			State->ReadAbsorb[rt] = 0;
			State->ReadAbsorb[rs] = 0;
			var temp_8 = (uint) ((rs) switch { 0 => 0U, var temp_94 => State->Registers[temp_94] });
			DoLds();
			var temp_95 = rt;
			if(temp_95 != 0)
				State->Registers[temp_95] = (uint) ((uint) ((((uint) ((uint) (temp_8))) & ((uint) ((ushort) (imm))))));
			return true;
		}
		insn_6:
		/* BEQ */
		if((insn & 0xFC000000) == 0x10000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_9 = (uint) ((rs) switch { 0 => 0U, var temp_96 => State->Registers[temp_96] });
			var temp_10 = (uint) ((rt) switch { 0 => 0U, var temp_97 => State->Registers[temp_97] });
			DoLds();
			if((bool) ((bool) (((uint) (temp_9)) == ((uint) (temp_10))))) {
				Branch(target);
			} else {
				Branch((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
			}
			return true;
		}
		insn_7:
		/* BGEZ */
		if((insn & 0xFC110000) == 0x04010000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			State->ReadAbsorb[rs] = 0;
			var temp_11 = (uint) ((rs) switch { 0 => 0U, var temp_98 => State->Registers[temp_98] });
			DoLds();
			if((bool) ((bool) (((int) ((int) ((int) (temp_11)))) >= ((int) ((sbyte) ((byte) ((byte) 0x0))))))) {
				Branch(target);
			} else {
				Branch((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
			}
			return true;
		}
		insn_8:
		/* BGEZAL */
		if((insn & 0xFC110000) == 0x04110000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			State->ReadAbsorb[(byte) 0x1F] = 0;
			State->ReadAbsorb[rs] = 0;
			var temp_12 = (uint) ((rs) switch { 0 => 0U, var temp_99 => State->Registers[temp_99] });
			DoLds();
			State->Registers[31] = (uint) ((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
			if((bool) ((bool) (((int) ((int) ((int) (temp_12)))) >= ((int) ((sbyte) ((byte) ((byte) 0x0))))))) {
				Branch(target);
			} else {
				Branch((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
			}
			return true;
		}
		insn_9:
		/* BGTZ */
		if((insn & 0xFC1F0000) == 0x1C000000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			State->ReadAbsorb[rs] = 0;
			var temp_13 = (uint) ((rs) switch { 0 => 0U, var temp_100 => State->Registers[temp_100] });
			DoLds();
			if((bool) ((bool) (((int) ((int) ((int) (temp_13)))) > ((int) ((sbyte) ((byte) ((byte) 0x0))))))) {
				Branch(target);
			} else {
				Branch((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
			}
			return true;
		}
		insn_10:
		/* BLEZ */
		if((insn & 0xFC1F0000) == 0x18000000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			State->ReadAbsorb[rs] = 0;
			var temp_14 = (uint) ((rs) switch { 0 => 0U, var temp_101 => State->Registers[temp_101] });
			DoLds();
			if((bool) ((bool) (((int) ((int) ((int) (temp_14)))) <= ((int) ((sbyte) ((byte) ((byte) 0x0))))))) {
				Branch(target);
			} else {
				Branch((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
			}
			return true;
		}
		insn_11:
		/* BLTZ */
		if((insn & 0xFC110000) == 0x04000000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			State->ReadAbsorb[rs] = 0;
			var temp_15 = (uint) ((rs) switch { 0 => 0U, var temp_102 => State->Registers[temp_102] });
			DoLds();
			if((bool) ((bool) (((int) ((int) ((int) (temp_15)))) < ((int) ((sbyte) ((byte) ((byte) 0x0))))))) {
				Branch(target);
			} else {
				Branch((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
			}
			return true;
		}
		insn_12:
		/* BLTZAL */
		if((insn & 0xFC110000) == 0x04100000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			State->ReadAbsorb[(byte) 0x1F] = 0;
			State->ReadAbsorb[rs] = 0;
			var temp_16 = (uint) ((rs) switch { 0 => 0U, var temp_103 => State->Registers[temp_103] });
			DoLds();
			State->Registers[31] = (uint) ((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
			if((bool) ((bool) (((int) ((int) ((int) (temp_16)))) < ((int) ((sbyte) ((byte) ((byte) 0x0))))))) {
				Branch(target);
			} else {
				Branch((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
			}
			return true;
		}
		insn_13:
		/* BNE */
		if((insn & 0xFC000000) == 0x14000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_17 = (uint) ((rs) switch { 0 => 0U, var temp_104 => State->Registers[temp_104] });
			var temp_18 = (uint) ((rt) switch { 0 => 0U, var temp_105 => State->Registers[temp_105] });
			DoLds();
			if((bool) ((bool) (((uint) (temp_17)) != ((uint) (temp_18))))) {
				Branch(target);
			} else {
				Branch((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
			}
			return true;
		}
		insn_14:
		/* BREAK */
		if((insn & 0xFC00003F) == 0x0000000D) {
			var code = (insn >> 6) & 0xFFFFFU;
			DoLds();
			throw new CpuException(ExceptionType.Break, pc, insn);
			return true;
		}
		insn_15:
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
				State->Registers[temp_106] = (uint) ((uint) (Copcreg(cop, rd)));
			return true;
		}
		insn_16:
		/* COP */
		if((insn & 0xF2000000) == 0x42000000) {
			var cop = (insn >> 26) & 0x3U;
			var command = (insn >> 0) & 0x1FFFFFFU;
			DoLds();
			Copfun(cop, command);
			return true;
		}
		insn_17:
		/* CTC */
		if((insn & 0xF3E00000) == 0x40C00000) {
			var cop = (insn >> 26) & 0x3U;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var cofun = (insn >> 0) & 0x7FFU;
			State->ReadAbsorb[rt] = 0;
			var temp_19 = (uint) ((rt) switch { 0 => 0U, var temp_107 => State->Registers[temp_107] });
			DoLds();
			Copcreg(cop, rd, temp_19);
			return true;
		}
		insn_18:
		/* DIV */
		if((insn & 0xFC00003F) == 0x0000001A) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_20 = (uint) ((rs) switch { 0 => 0U, var temp_108 => State->Registers[temp_108] });
			var temp_21 = (uint) ((rt) switch { 0 => 0U, var temp_109 => State->Registers[temp_109] });
			DoLds();
			var rsv = temp_20;
			var rtv = temp_21;
			if((bool) ((bool) (((uint) (rtv)) == ((uint) ((byte) ((byte) 0x0)))))) {
				State->Lo = (uint) ((uint) ((byte) (((bool) ((bool) (((uint) ((uint) ((((uint) ((uint) (rsv))) & ((uint) ((uint) (0x80000000U))))))) != ((uint) ((byte) ((byte) 0x0)))))) ? (uint) (((byte) 0x1)) : (uint) ((0xFFFFFFFFU)))));
				State->Hi = (uint) (rsv);
			} else {
				if((bool) ((bool) ((((bool) ((bool) ((bool) (((uint) (rsv)) == ((uint) (0x80000000U)))))) & ((bool) ((bool) ((bool) (((uint) (rtv)) == ((uint) (0xFFFFFFFFU)))))))))) {
					State->Lo = (uint) (0x80000000U);
					State->Hi = (uint) ((uint) ((byte) 0x0));
				} else {
					State->Lo = (uint) ((uint) ((int) (((int) (int) ((int) ((int) ((int) (rsv))))) / ((int) (int) ((int) ((int) ((int) (rtv))))))));
					State->Hi = (uint) ((uint) ((int) (((int) (int) ((int) ((int) ((int) (rsv))))) % ((int) (int) ((int) ((int) ((int) (rtv))))))));
					DivDelay();
				}
			}
			return true;
		}
		insn_19:
		/* DIVU */
		if((insn & 0xFC00003F) == 0x0000001B) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_22 = (uint) ((rs) switch { 0 => 0U, var temp_110 => State->Registers[temp_110] });
			var temp_23 = (uint) ((rt) switch { 0 => 0U, var temp_111 => State->Registers[temp_111] });
			DoLds();
			var rsv = temp_22;
			var rtv = temp_23;
			if((bool) ((bool) (((uint) (rtv)) == ((uint) ((byte) ((byte) 0x0)))))) {
				State->Lo = (uint) (0xFFFFFFFFU);
				State->Hi = (uint) (rsv);
			} else {
				State->Lo = (uint) ((uint) (((uint) (uint) ((uint) (rsv))) / ((uint) (uint) ((uint) (rtv)))));
				State->Hi = (uint) ((uint) (((uint) (uint) ((uint) (rsv))) % ((uint) (uint) ((uint) (rtv)))));
				DivDelay();
			}
			return true;
		}
		insn_20:
		/* J */
		if((insn & 0xFC000000) == 0x08000000) {
			var imm = (insn >> 0) & 0x3FFFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((((uint) ((uint) ((pc + 4)))) & ((uint) (0xF0000000U)))))) + ((uint) (uint) ((uint) (((uint) ((uint) (imm))) << (int) ((byte) 0x2)))));
			DoLds();
			Branch(target);
			return true;
		}
		insn_21:
		/* JAL */
		if((insn & 0xFC000000) == 0x0C000000) {
			var imm = (insn >> 0) & 0x3FFFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((((uint) ((uint) ((pc + 4)))) & ((uint) (0xF0000000U)))))) + ((uint) (uint) ((uint) (((uint) ((uint) (imm))) << (int) ((byte) 0x2)))));
			State->ReadAbsorb[(byte) 0x1F] = 0;
			DoLds();
			State->Registers[31] = (uint) ((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
			Branch(target);
			return true;
		}
		insn_22:
		/* JALR */
		if((insn & 0xFC00003F) == 0x00000009) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rd] = 0;
			var temp_24 = (uint) ((rs) switch { 0 => 0U, var temp_112 => State->Registers[temp_112] });
			DoLds();
			var target = temp_24;
			var temp_113 = rd;
			if(temp_113 != 0)
				State->Registers[temp_113] = (uint) ((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
			if((bool) ((bool) (((uint) ((byte) ((byte) 0x0))) != ((uint) ((uint) ((((uint) ((uint) (target))) & ((uint) ((uint) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (32))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1))))))))))))) {
				throw new CpuException(ExceptionType.ADEL, pc, insn);
			}
			Branch(target);
			return true;
		}
		insn_23:
		/* JR */
		if((insn & 0xFC00003F) == 0x00000008) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rs] = 0;
			var temp_25 = (uint) ((rs) switch { 0 => 0U, var temp_114 => State->Registers[temp_114] });
			DoLds();
			var target = temp_25;
			if((bool) ((bool) (((uint) ((byte) ((byte) 0x0))) != ((uint) ((uint) ((((uint) ((uint) (target))) & ((uint) ((uint) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (32))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1))))))))))))) {
				throw new CpuException(ExceptionType.ADEL, pc, insn);
			}
			Branch(target);
			return true;
		}
		insn_24:
		/* LB */
		if((insn & 0xFC000000) == 0x80000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			State->ReadAbsorb[rt] = 0;
			State->ReadAbsorb[rs] = 0;
			var temp_26 = (uint) ((rs) switch { 0 => 0U, var temp_115 => State->Registers[temp_115] });
			DoLds();
			State->LdWhich = (uint) (rt);
			State->LdValue = (uint) ((sbyte) (ReadMemory<sbyte>((uint) (((uint) (uint) ((uint) (temp_26))) + ((uint) (int) ((int) (offset)))))));
			return true;
		}
		insn_25:
		/* LBU */
		if((insn & 0xFC000000) == 0x90000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			State->ReadAbsorb[rt] = 0;
			State->ReadAbsorb[rs] = 0;
			var temp_27 = (uint) ((rs) switch { 0 => 0U, var temp_116 => State->Registers[temp_116] });
			DoLds();
			State->LdWhich = (uint) (rt);
			State->LdValue = (uint) ((byte) (ReadMemory<byte>((uint) (((uint) (uint) ((uint) (temp_27))) + ((uint) (int) ((int) (offset)))))));
			return true;
		}
		insn_26:
		/* LH */
		if((insn & 0xFC000000) == 0x84000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_28 = (uint) ((rs) switch { 0 => 0U, var temp_117 => State->Registers[temp_117] });
			DoLds();
			var addr = (uint) (((uint) (uint) ((uint) (temp_28))) + ((uint) (int) ((int) (offset))));
			if((bool) ((bool) (((uint) ((byte) ((byte) 0x0))) != ((uint) ((uint) ((((uint) ((uint) (addr))) & ((uint) ((uint) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (16))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1))))))))))))) {
				throw new CpuException(ExceptionType.ADEL, pc, insn);
			}
			State->LdWhich = (uint) (rt);
			State->LdValue = (uint) ((short) (ReadMemory<short>(addr)));
			return true;
		}
		insn_27:
		/* LHU */
		if((insn & 0xFC000000) == 0x94000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_29 = (uint) ((rs) switch { 0 => 0U, var temp_118 => State->Registers[temp_118] });
			DoLds();
			var addr = (uint) (((uint) (uint) ((uint) (temp_29))) + ((uint) (int) ((int) (offset))));
			if((bool) ((bool) (((uint) ((byte) ((byte) 0x0))) != ((uint) ((uint) ((((uint) ((uint) (addr))) & ((uint) ((uint) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (16))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1))))))))))))) {
				throw new CpuException(ExceptionType.ADEL, pc, insn);
			}
			State->LdWhich = (uint) (rt);
			State->LdValue = (uint) ((ushort) (ReadMemory<ushort>(addr)));
			return true;
		}
		insn_28:
		/* LUI */
		if((insn & 0xFC000000) == 0x3C000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			State->ReadAbsorb[rt] = 0;
			DoLds();
			var temp_119 = rt;
			if(temp_119 != 0)
				State->Registers[temp_119] = (uint) ((uint) (((uint) ((uint) (imm))) << (int) ((byte) 0x10)));
			return true;
		}
		insn_29:
		/* LW */
		if((insn & 0xFC000000) == 0x8C000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_30 = (uint) ((rs) switch { 0 => 0U, var temp_120 => State->Registers[temp_120] });
			DoLds();
			var addr = (uint) (((uint) (uint) ((uint) (temp_30))) + ((uint) (int) ((int) (offset))));
			if((bool) ((bool) (((uint) ((byte) ((byte) 0x0))) != ((uint) ((uint) ((((uint) ((uint) (addr))) & ((uint) ((uint) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (32))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1))))))))))))) {
				throw new CpuException(ExceptionType.ADEL, pc, insn);
			}
			State->LdWhich = (uint) (rt);
			State->LdValue = (uint) ((uint) (ReadMemory<uint>(addr)));
			return true;
		}
		insn_30:
		/* LWL */
		if((insn & 0xFC000000) == 0x88000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_31 = (uint) ((rt) switch { 0 => 0U, var temp_121 => State->Registers[temp_121] });
			var temp_32 = (uint) ((rs) switch { 0 => 0U, var temp_122 => State->Registers[temp_122] });
			DoLoad(rt, ref temp_31);
			var addr = (uint) (((uint) (uint) ((uint) (temp_32))) + ((uint) (int) ((int) (offset))));
			var raddr = (uint) ((((uint) ((uint) (addr))) & ((uint) ((uint) (0xFFFFFFFCU)))));
			var ert = temp_31;
			State->LdWhich = (uint) (rt);
			State->LdValue = (uint) ((uint) ((uint) ((((uint) ((uint) (addr))) & ((uint) ((byte) ((byte) 0x3))))) switch { (uint) ((byte) 0x0) => (uint) ((((uint) ((uint) ((uint) ((((uint) ((uint) (ert))) & ((uint) ((uint) (0xFFFFFFU)))))))) | ((uint) ((uint) ((uint) (((uint) ((uint) ((byte) (ReadMemory<byte>(raddr))))) << (int) ((byte) 0x18))))))), (uint) ((byte) 0x1) => (uint) ((((uint) ((uint) ((uint) ((((uint) ((uint) (ert))) & ((uint) ((ushort) ((ushort) 0xFFFF)))))))) | ((uint) ((uint) ((uint) (((uint) ((uint) ((ushort) (ReadMemory<ushort>(raddr))))) << (int) ((byte) 0x10))))))), (uint) ((byte) 0x2) => (uint) ((((uint) ((uint) ((uint) ((((uint) ((uint) (ert))) & ((uint) ((byte) ((byte) 0xFF)))))))) | ((uint) ((uint) ((uint) (((uint) (ReadMemory<uint>(raddr))) << (int) ((byte) 0x8))))))), _ => (uint) (ReadMemory<uint>(raddr)) }));
			return true;
		}
		insn_31:
		/* LWR */
		if((insn & 0xFC000000) == 0x98000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_33 = (uint) ((rt) switch { 0 => 0U, var temp_124 => State->Registers[temp_124] });
			var temp_34 = (uint) ((rs) switch { 0 => 0U, var temp_125 => State->Registers[temp_125] });
			DoLoad(rt, ref temp_33);
			var addr = (uint) (((uint) (uint) ((uint) (temp_34))) + ((uint) (int) ((int) (offset))));
			var raddr = (uint) ((((uint) ((uint) (addr))) & ((uint) ((uint) (0xFFFFFFFCU)))));
			var ert = temp_33;
			State->LdWhich = (uint) (rt);
			State->LdValue = (uint) ((uint) ((uint) ((((uint) ((uint) (addr))) & ((uint) ((byte) ((byte) 0x3))))) switch { (uint) ((byte) 0x0) => (uint) (ReadMemory<uint>(raddr)), (uint) ((byte) 0x1) => (uint) ((((uint) ((uint) ((uint) ((((uint) ((uint) (ert))) & ((uint) ((uint) (0xFF000000U)))))))) | ((uint) ((uint) ((uint) ((((uint) ((uint) ((uint) (ReadMemory<uint>(addr))))) & ((uint) ((uint) (0xFFFFFFU)))))))))), (uint) ((byte) 0x2) => (uint) ((((uint) ((uint) ((uint) ((((uint) ((uint) (ert))) & ((uint) ((uint) (0xFFFF0000U)))))))) | ((uint) ((ushort) ((ushort) (ReadMemory<ushort>(addr))))))), _ => (uint) ((((uint) ((uint) ((uint) ((((uint) ((uint) (ert))) & ((uint) ((uint) (0xFFFFFF00U)))))))) | ((uint) ((byte) ((byte) (ReadMemory<byte>(addr))))))) }));
			return true;
		}
		insn_32:
		/* LWC2 */
		if((insn & 0xFC000000) == 0xC8000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			State->ReadAbsorb[rs] = 0;
			var temp_35 = (uint) ((rs) switch { 0 => 0U, var temp_127 => State->Registers[temp_127] });
			DoLds();
			var addr = (uint) (((uint) (uint) ((uint) (temp_35))) + ((uint) (int) ((int) (offset))));
			if((bool) ((bool) (((uint) ((byte) ((byte) 0x0))) != ((uint) ((uint) ((((uint) ((uint) (addr))) & ((uint) ((uint) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (32))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1))))))))))))) {
				throw new CpuException(ExceptionType.ADEL, pc, insn);
			}
			Copreg((byte) 0x2, rt, (uint) (ReadMemory<uint>(addr)));
			return true;
		}
		insn_33:
		/* MFC */
		if((insn & 0xF3E00000) == 0x40000000) {
			var cop = (insn >> 26) & 0x3U;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var cofun = (insn >> 0) & 0x7FFU;
			State->ReadAbsorb[rt] = 0;
			DoLds();
			State->LdWhich = (uint) (rt);
			State->LdValue = (uint) ((uint) (Copreg(cop, rd)));
			return true;
		}
		insn_34:
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
				State->Registers[temp_128] = (uint) ((uint) (State->Hi));
			AbsorbMuldivDelay();
			return true;
		}
		insn_35:
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
				State->Registers[temp_129] = (uint) ((uint) (State->Lo));
			AbsorbMuldivDelay();
			return true;
		}
		insn_36:
		/* MTC */
		if((insn & 0xF3E00000) == 0x40800000) {
			var cop = (insn >> 26) & 0x3U;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var cofun = (insn >> 0) & 0x7FFU;
			State->ReadAbsorb[rt] = 0;
			var temp_36 = (uint) ((rt) switch { 0 => 0U, var temp_130 => State->Registers[temp_130] });
			DoLds();
			Copreg(cop, rd, temp_36);
			return true;
		}
		insn_37:
		/* MTHI */
		if((insn & 0xFC00003F) == 0x00000011) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rs] = 0;
			var temp_37 = (uint) ((rs) switch { 0 => 0U, var temp_131 => State->Registers[temp_131] });
			DoLds();
			State->Hi = (uint) (temp_37);
			return true;
		}
		insn_38:
		/* MTLO */
		if((insn & 0xFC00003F) == 0x00000013) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rs] = 0;
			var temp_38 = (uint) ((rs) switch { 0 => 0U, var temp_132 => State->Registers[temp_132] });
			DoLds();
			State->Lo = (uint) (temp_38);
			return true;
		}
		insn_39:
		/* MULT */
		if((insn & 0xFC00003F) == 0x00000018) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_39 = (uint) ((rs) switch { 0 => 0U, var temp_133 => State->Registers[temp_133] });
			var temp_40 = (uint) ((rt) switch { 0 => 0U, var temp_134 => State->Registers[temp_134] });
			DoLds();
			var lhs = temp_39;
			var rhs = temp_40;
			var result = (ulong) ((ulong) ((long) (((long) (long) ((long) ((long) ((long) (lhs))))) * ((long) (long) ((long) ((long) ((long) (rhs))))))));
			State->Lo = (uint) ((uint) (result));
			State->Hi = (uint) ((uint) ((ulong) ((result) >> (int) ((byte) 0x20))));
			MulDelay(lhs, rhs, (byte) 0x1 != 0);
			return true;
		}
		insn_40:
		/* MULTU */
		if((insn & 0xFC00003F) == 0x00000019) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_41 = (uint) ((rs) switch { 0 => 0U, var temp_135 => State->Registers[temp_135] });
			var temp_42 = (uint) ((rt) switch { 0 => 0U, var temp_136 => State->Registers[temp_136] });
			DoLds();
			var lhs = temp_41;
			var rhs = temp_42;
			var result = (ulong) (((ulong) (ulong) ((ulong) ((ulong) ((ulong) (lhs))))) * ((ulong) (ulong) ((ulong) ((ulong) ((ulong) (rhs))))));
			State->Lo = (uint) ((uint) (result));
			State->Hi = (uint) ((uint) ((ulong) ((result) >> (int) ((byte) 0x20))));
			MulDelay(lhs, rhs, (byte) 0x0 != 0);
			return true;
		}
		insn_41:
		/* NOR */
		if((insn & 0xFC00003F) == 0x00000027) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rd] = 0;
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_43 = (uint) ((rs) switch { 0 => 0U, var temp_137 => State->Registers[temp_137] });
			var temp_44 = (uint) ((rt) switch { 0 => 0U, var temp_138 => State->Registers[temp_138] });
			DoLds();
			var temp_139 = rd;
			if(temp_139 != 0)
				State->Registers[temp_139] = (uint) ((uint) (~((uint) ((((uint) ((uint) (temp_43))) | ((uint) ((uint) (temp_44))))))));
			return true;
		}
		insn_42:
		/* OR */
		if((insn & 0xFC00003F) == 0x00000025) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rd] = 0;
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_45 = (uint) ((rs) switch { 0 => 0U, var temp_140 => State->Registers[temp_140] });
			var temp_46 = (uint) ((rt) switch { 0 => 0U, var temp_141 => State->Registers[temp_141] });
			DoLds();
			var temp_142 = rd;
			if(temp_142 != 0)
				State->Registers[temp_142] = (uint) ((uint) ((((uint) ((uint) (temp_45))) | ((uint) ((uint) (temp_46))))));
			return true;
		}
		insn_43:
		/* ORI */
		if((insn & 0xFC000000) == 0x34000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			State->ReadAbsorb[rt] = 0;
			State->ReadAbsorb[rs] = 0;
			var temp_47 = (uint) ((rs) switch { 0 => 0U, var temp_143 => State->Registers[temp_143] });
			DoLds();
			var temp_144 = rt;
			if(temp_144 != 0)
				State->Registers[temp_144] = (uint) ((uint) ((((uint) ((uint) (temp_47))) | ((uint) ((uint) ((uint) ((uint) (imm))))))));
			return true;
		}
		insn_44:
		/* SB */
		if((insn & 0xFC000000) == 0xA0000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_48 = (uint) ((rs) switch { 0 => 0U, var temp_145 => State->Registers[temp_145] });
			var temp_49 = (uint) ((rt) switch { 0 => 0U, var temp_146 => State->Registers[temp_146] });
			DoLds();
			WriteMemory((uint) (((uint) (uint) ((uint) (temp_48))) + ((uint) (int) ((int) (offset)))), (byte) ((byte) (temp_49)));
			return true;
		}
		insn_45:
		/* SH */
		if((insn & 0xFC000000) == 0xA4000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_50 = (uint) ((rs) switch { 0 => 0U, var temp_147 => State->Registers[temp_147] });
			var temp_51 = (uint) ((rt) switch { 0 => 0U, var temp_148 => State->Registers[temp_148] });
			DoLds();
			var addr = (uint) (((uint) (uint) ((uint) (temp_50))) + ((uint) (int) ((int) (offset))));
			if((bool) ((bool) (((uint) ((byte) ((byte) 0x0))) != ((uint) ((uint) ((((uint) ((uint) (addr))) & ((uint) ((uint) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (16))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1))))))))))))) {
				throw new CpuException(ExceptionType.ADES, pc, insn);
			}
			WriteMemory(addr, (ushort) ((ushort) (temp_51)));
			return true;
		}
		insn_46:
		/* SLL */
		if((insn & 0xFC00003F) == 0x00000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rd] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_52 = (uint) ((rt) switch { 0 => 0U, var temp_149 => State->Registers[temp_149] });
			DoLds();
			var temp_150 = rd;
			if(temp_150 != 0)
				State->Registers[temp_150] = (uint) ((uint) ((temp_52) << (int) (shamt)));
			return true;
		}
		insn_47:
		/* SLLV */
		if((insn & 0xFC00003F) == 0x00000004) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rd] = 0;
			State->ReadAbsorb[rt] = 0;
			State->ReadAbsorb[rs] = 0;
			var temp_53 = (uint) ((rt) switch { 0 => 0U, var temp_151 => State->Registers[temp_151] });
			var temp_54 = (uint) ((rs) switch { 0 => 0U, var temp_152 => State->Registers[temp_152] });
			DoLds();
			var temp_153 = rd;
			if(temp_153 != 0)
				State->Registers[temp_153] = (uint) ((uint) ((temp_53) << (int) (temp_54)));
			return true;
		}
		insn_48:
		/* SLT */
		if((insn & 0xFC00003F) == 0x0000002A) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rd] = 0;
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_55 = (uint) ((rs) switch { 0 => 0U, var temp_154 => State->Registers[temp_154] });
			var temp_56 = (uint) ((rt) switch { 0 => 0U, var temp_155 => State->Registers[temp_155] });
			DoLds();
			var temp_156 = rd;
			if(temp_156 != 0)
				State->Registers[temp_156] = (uint) ((uint) (((bool) (((int) ((int) ((int) (temp_55)))) < ((int) ((int) ((int) (temp_56)))))) ? 1U : 0U));
			return true;
		}
		insn_49:
		/* SLTI */
		if((insn & 0xFC000000) == 0x28000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var eimm = (int) (SignExt<int>(imm, 16));
			State->ReadAbsorb[rt] = 0;
			State->ReadAbsorb[rs] = 0;
			var temp_57 = (uint) ((rs) switch { 0 => 0U, var temp_157 => State->Registers[temp_157] });
			DoLds();
			var temp_158 = rt;
			if(temp_158 != 0)
				State->Registers[temp_158] = (uint) ((uint) (((bool) (((int) ((int) ((int) (temp_57)))) < ((int) (eimm)))) ? 1U : 0U));
			return true;
		}
		insn_50:
		/* SLTIU */
		if((insn & 0xFC000000) == 0x2C000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var eimm = (uint) (SignExt<uint>(imm, 16));
			State->ReadAbsorb[rt] = 0;
			State->ReadAbsorb[rs] = 0;
			var temp_58 = (uint) ((rs) switch { 0 => 0U, var temp_159 => State->Registers[temp_159] });
			DoLds();
			var temp_160 = rt;
			if(temp_160 != 0)
				State->Registers[temp_160] = (uint) ((uint) (((bool) (((uint) (temp_58)) < ((uint) (eimm)))) ? 1U : 0U));
			return true;
		}
		insn_51:
		/* SLTU */
		if((insn & 0xFC00003F) == 0x0000002B) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rd] = 0;
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_59 = (uint) ((rs) switch { 0 => 0U, var temp_161 => State->Registers[temp_161] });
			var temp_60 = (uint) ((rt) switch { 0 => 0U, var temp_162 => State->Registers[temp_162] });
			DoLds();
			var temp_163 = rd;
			if(temp_163 != 0)
				State->Registers[temp_163] = (uint) ((uint) (((bool) (((uint) (temp_59)) < ((uint) (temp_60)))) ? 1U : 0U));
			return true;
		}
		insn_52:
		/* SRA */
		if((insn & 0xFC00003F) == 0x00000003) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rd] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_61 = (uint) ((rt) switch { 0 => 0U, var temp_164 => State->Registers[temp_164] });
			DoLds();
			var temp_165 = rd;
			if(temp_165 != 0)
				State->Registers[temp_165] = (uint) ((uint) ((int) (((int) ((int) (temp_61))) >> (int) (shamt))));
			return true;
		}
		insn_53:
		/* SRAV */
		if((insn & 0xFC00003F) == 0x00000007) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rd] = 0;
			State->ReadAbsorb[rt] = 0;
			State->ReadAbsorb[rs] = 0;
			var temp_62 = (uint) ((rt) switch { 0 => 0U, var temp_166 => State->Registers[temp_166] });
			var temp_63 = (uint) ((rs) switch { 0 => 0U, var temp_167 => State->Registers[temp_167] });
			DoLds();
			var temp_168 = rd;
			if(temp_168 != 0)
				State->Registers[temp_168] = (uint) ((uint) ((int) (((int) ((int) (temp_62))) >> (int) ((int) ((int) (temp_63))))));
			return true;
		}
		insn_54:
		/* SRL */
		if((insn & 0xFC00003F) == 0x00000002) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rd] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_64 = (uint) ((rt) switch { 0 => 0U, var temp_169 => State->Registers[temp_169] });
			DoLds();
			var temp_170 = rd;
			if(temp_170 != 0)
				State->Registers[temp_170] = (uint) ((uint) ((temp_64) >> (int) (shamt)));
			return true;
		}
		insn_55:
		/* SRLV */
		if((insn & 0xFC00003F) == 0x00000006) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rd] = 0;
			State->ReadAbsorb[rt] = 0;
			State->ReadAbsorb[rs] = 0;
			var temp_65 = (uint) ((rt) switch { 0 => 0U, var temp_171 => State->Registers[temp_171] });
			var temp_66 = (uint) ((rs) switch { 0 => 0U, var temp_172 => State->Registers[temp_172] });
			DoLds();
			var temp_173 = rd;
			if(temp_173 != 0)
				State->Registers[temp_173] = (uint) ((uint) ((temp_65) >> (int) (temp_66)));
			return true;
		}
		insn_56:
		/* SUB */
		if((insn & 0xFC00003F) == 0x00000022) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			State->ReadAbsorb[rd] = 0;
			var temp_67 = (uint) ((rs) switch { 0 => 0U, var temp_174 => State->Registers[temp_174] });
			var temp_68 = (uint) ((rt) switch { 0 => 0U, var temp_175 => State->Registers[temp_175] });
			DoLds();
			var lhs = temp_67;
			var rhs = temp_68;
			var r = (uint) ((uint) (((uint) (uint) ((uint) (lhs))) - ((uint) (uint) ((uint) (rhs)))));
			if((bool) (((uint) (((((uint) ((uint) ((uint) ((((uint) ((uint) (lhs))) ^ ((uint) ((uint) (rhs)))))))) & ((uint) ((uint) ((uint) ((((uint) ((uint) (lhs))) ^ ((uint) ((uint) (r))))))))) & ((uint) ((uint) (0x80000000U)))))) != 0)) {
				throw new CpuException(ExceptionType.OV, pc, insn);
			}
			var temp_176 = rd;
			if(temp_176 != 0)
				State->Registers[temp_176] = (uint) ((uint) (((uint) (uint) ((uint) (lhs))) - ((uint) (uint) ((uint) (rhs)))));
			return true;
		}
		insn_57:
		/* SUBU */
		if((insn & 0xFC00003F) == 0x00000023) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rd] = 0;
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_69 = (uint) ((rs) switch { 0 => 0U, var temp_177 => State->Registers[temp_177] });
			var temp_70 = (uint) ((rt) switch { 0 => 0U, var temp_178 => State->Registers[temp_178] });
			DoLds();
			var temp_179 = rd;
			if(temp_179 != 0)
				State->Registers[temp_179] = (uint) ((uint) (((uint) (uint) ((uint) (temp_69))) - ((uint) (uint) ((uint) (temp_70)))));
			return true;
		}
		insn_58:
		/* SW */
		if((insn & 0xFC000000) == 0xAC000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_71 = (uint) ((rs) switch { 0 => 0U, var temp_180 => State->Registers[temp_180] });
			var temp_72 = (uint) ((rt) switch { 0 => 0U, var temp_181 => State->Registers[temp_181] });
			DoLds();
			var addr = (uint) (((uint) (uint) ((uint) (temp_71))) + ((uint) (int) ((int) (offset))));
			if((bool) ((bool) (((uint) ((byte) ((byte) 0x0))) != ((uint) ((uint) ((((uint) ((uint) (addr))) & ((uint) ((uint) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (32))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1))))))))))))) {
				throw new CpuException(ExceptionType.ADES, pc, insn);
			}
			WriteMemory(addr, temp_72);
			return true;
		}
		insn_59:
		/* SWC2 */
		if((insn & 0xFC000000) == 0xE8000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			State->ReadAbsorb[rs] = 0;
			var temp_73 = (uint) ((rs) switch { 0 => 0U, var temp_182 => State->Registers[temp_182] });
			DoLds();
			var addr = (uint) (((uint) (uint) ((uint) (temp_73))) + ((uint) (int) ((int) (offset))));
			if((bool) ((bool) (((uint) ((byte) ((byte) 0x0))) != ((uint) ((uint) ((((uint) ((uint) (addr))) & ((uint) ((uint) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (32))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1))))))))))))) {
				throw new CpuException(ExceptionType.ADES, pc, insn);
			}
			WriteMemory(addr, (uint) (Copreg((byte) 0x2, rt)));
			return true;
		}
		insn_60:
		/* SWL */
		if((insn & 0xFC000000) == 0xA8000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_74 = (uint) ((rs) switch { 0 => 0U, var temp_183 => State->Registers[temp_183] });
			var temp_75 = (uint) ((rt) switch { 0 => 0U, var temp_184 => State->Registers[temp_184] });
			DoLds();
			var addr = (uint) (((uint) (uint) ((uint) (temp_74))) + ((uint) (int) ((int) (offset))));
			var raddr = (uint) ((((uint) ((uint) (addr))) & ((uint) ((uint) (0xFFFFFFFCU)))));
			var rtv = temp_75;
			switch((uint) ((((uint) ((uint) (addr))) & ((uint) ((byte) ((byte) 0x3)))))) {
				case (uint) ((byte) 0x0): {
					WriteMemory(raddr, (byte) ((byte) ((uint) ((rtv) >> (int) ((byte) 0x18)))));
					break;
				}
				case (uint) ((byte) 0x1): {
					WriteMemory(raddr, (ushort) ((ushort) ((uint) ((rtv) >> (int) ((byte) 0x10)))));
					break;
				}
				case (uint) ((byte) 0x3): {
					WriteMemory(raddr, rtv);
					break;
				}
				default: {
					WriteMemory(raddr, (ushort) ((ushort) ((uint) ((rtv) >> (int) ((byte) 0x8)))));
					WriteMemory((uint) (((uint) (uint) ((uint) (raddr))) + ((uint) (byte) ((byte) ((byte) 0x2)))), (byte) ((byte) ((uint) ((rtv) >> (int) ((byte) 0x18)))));
					break;
				}
			}
			return true;
		}
		insn_61:
		/* SWR */
		if((insn & 0xFC000000) == 0xB8000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_76 = (uint) ((rs) switch { 0 => 0U, var temp_185 => State->Registers[temp_185] });
			var temp_77 = (uint) ((rt) switch { 0 => 0U, var temp_186 => State->Registers[temp_186] });
			DoLds();
			var addr = (uint) (((uint) (uint) ((uint) (temp_76))) + ((uint) (int) ((int) (offset))));
			var raddr = (uint) ((((uint) ((uint) (addr))) & ((uint) ((uint) (0xFFFFFFFCU)))));
			var rtv = temp_77;
			switch((uint) ((((uint) ((uint) (addr))) & ((uint) ((byte) ((byte) 0x3)))))) {
				case (uint) ((byte) 0x0): {
					WriteMemory(raddr, rtv);
					break;
				}
				case (uint) ((byte) 0x2): {
					WriteMemory(raddr, (ushort) ((ushort) (rtv)));
					break;
				}
				case (uint) ((byte) 0x3): {
					WriteMemory(raddr, (byte) ((byte) (rtv)));
					break;
				}
				default: {
					WriteMemory(raddr, (ushort) ((ushort) (rtv)));
					WriteMemory((uint) (((uint) (uint) ((uint) (raddr))) + ((uint) (byte) ((byte) ((byte) 0x2)))), (byte) ((byte) ((uint) ((rtv) >> (int) ((byte) 0x10)))));
					break;
				}
			}
			return true;
		}
		insn_62:
		/* SYSCALL */
		if((insn & 0xFC00003F) == 0x0000000C) {
			var code = (insn >> 6) & 0xFFFFFU;
			DoLds();
			throw new CpuException(ExceptionType.Syscall, pc, insn);
			return true;
		}
		insn_63:
		/* XOR */
		if((insn & 0xFC00003F) == 0x00000026) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->ReadAbsorb[rd] = 0;
			State->ReadAbsorb[rs] = 0;
			State->ReadAbsorb[rt] = 0;
			var temp_78 = (uint) ((rs) switch { 0 => 0U, var temp_187 => State->Registers[temp_187] });
			var temp_79 = (uint) ((rt) switch { 0 => 0U, var temp_188 => State->Registers[temp_188] });
			DoLds();
			var temp_189 = rd;
			if(temp_189 != 0)
				State->Registers[temp_189] = (uint) ((uint) ((((uint) ((uint) (temp_78))) ^ ((uint) ((uint) (temp_79))))));
			return true;
		}
		insn_64:
		/* XORI */
		if((insn & 0xFC000000) == 0x38000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			State->ReadAbsorb[rt] = 0;
			State->ReadAbsorb[rs] = 0;
			var temp_80 = (uint) ((rs) switch { 0 => 0U, var temp_190 => State->Registers[temp_190] });
			DoLds();
			var temp_191 = rt;
			if(temp_191 != 0)
				State->Registers[temp_191] = (uint) ((uint) ((((uint) ((uint) (temp_80))) ^ ((uint) ((uint) ((uint) ((uint) (imm))))))));
			return true;
		}
		insn_65:

        return false;
    }
}
