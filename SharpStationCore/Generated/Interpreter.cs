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
			var lhs = (uint) ((rs) switch { 0 => 0U, var temp_0 => State->Registers[temp_0] });
			var rhs = (uint) ((rt) switch { 0 => 0U, var temp_1 => State->Registers[temp_1] });
			var temp_2 = rd;
			if(temp_2 != 0)
				State->Registers[temp_2] = (uint) ((uint) (((uint) (uint) (lhs)) + ((uint) (uint) (rhs))));
			return true;
		}
		insn_1:
		/* ADDI */
		if((insn & 0xFC000000) == 0x20000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var eimm = (uint) (SignExt<uint>(imm, 16));
			var lhs = (uint) ((rs) switch { 0 => 0U, var temp_3 => State->Registers[temp_3] });
			var temp_4 = rt;
			if(temp_4 != 0)
				State->Registers[temp_4] = (uint) ((uint) (((uint) (uint) (lhs)) + ((uint) (uint) (eimm))));
			return true;
		}
		insn_2:
		/* ADDIU */
		if((insn & 0xFC000000) == 0x24000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var eimm = (uint) (SignExt<uint>(imm, 16));
			var temp_5 = rt;
			if(temp_5 != 0)
				State->Registers[temp_5] = (uint) ((uint) (((uint) (uint) ((uint) ((rs) switch { 0 => 0U, var temp_6 => State->Registers[temp_6] }))) + ((uint) (uint) (eimm))));
			return true;
		}
		insn_3:
		/* ADDU */
		if((insn & 0xFC00003F) == 0x00000021) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			var temp_7 = rd;
			if(temp_7 != 0)
				State->Registers[temp_7] = (uint) ((uint) (((uint) (uint) ((uint) ((rs) switch { 0 => 0U, var temp_8 => State->Registers[temp_8] }))) + ((uint) (uint) ((uint) ((rt) switch { 0 => 0U, var temp_9 => State->Registers[temp_9] })))));
			return true;
		}
		insn_4:
		/* AND */
		if((insn & 0xFC00003F) == 0x00000024) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			var temp_10 = rd;
			if(temp_10 != 0)
				State->Registers[temp_10] = (uint) ((uint) ((((uint) ((uint) ((rs) switch { 0 => 0U, var temp_11 => State->Registers[temp_11] }))) & ((uint) ((uint) ((rt) switch { 0 => 0U, var temp_12 => State->Registers[temp_12] }))))));
			return true;
		}
		insn_5:
		/* ANDI */
		if((insn & 0xFC000000) == 0x30000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var eimm = (uint) (SignExt<uint>(imm, 16));
			var temp_13 = rt;
			if(temp_13 != 0)
				State->Registers[temp_13] = (uint) ((uint) ((((uint) ((uint) ((rs) switch { 0 => 0U, var temp_14 => State->Registers[temp_14] }))) & ((uint) (eimm)))));
			return true;
		}
		insn_6:
		/* BEQ */
		if((insn & 0xFC000000) == 0x10000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			if(((uint) ((((uint) ((rs) switch { 0 => 0U, var temp_15 => State->Registers[temp_15] })) == ((uint) ((rt) switch { 0 => 0U, var temp_16 => State->Registers[temp_16] })) ? 1U : 0U))) != 0) {
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
			if(((uint) ((((int) ((int) ((uint) ((rs) switch { 0 => 0U, var temp_17 => State->Registers[temp_17] })))) >= ((int) ((sbyte) ((byte) 0x0))) ? 1U : 0U))) != 0) {
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
			State->Registers[31] = (uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8)));
			if(((uint) ((((int) ((int) ((uint) ((rs) switch { 0 => 0U, var temp_18 => State->Registers[temp_18] })))) >= ((int) ((sbyte) ((byte) 0x0))) ? 1U : 0U))) != 0) {
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
			if(((uint) ((((int) ((int) ((uint) ((rs) switch { 0 => 0U, var temp_19 => State->Registers[temp_19] })))) > ((int) ((sbyte) ((byte) 0x0))) ? 1U : 0U))) != 0) {
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
			if(((uint) ((((int) ((int) ((uint) ((rs) switch { 0 => 0U, var temp_20 => State->Registers[temp_20] })))) <= ((int) ((sbyte) ((byte) 0x0))) ? 1U : 0U))) != 0) {
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
			if(((uint) ((((int) ((int) ((uint) ((rs) switch { 0 => 0U, var temp_21 => State->Registers[temp_21] })))) < ((int) ((sbyte) ((byte) 0x0))) ? 1U : 0U))) != 0) {
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
			State->Registers[31] = (uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8)));
			if(((uint) ((((int) ((int) ((uint) ((rs) switch { 0 => 0U, var temp_22 => State->Registers[temp_22] })))) < ((int) ((sbyte) ((byte) 0x0))) ? 1U : 0U))) != 0) {
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
			if(((uint) ((((uint) ((rs) switch { 0 => 0U, var temp_23 => State->Registers[temp_23] })) != ((uint) ((rt) switch { 0 => 0U, var temp_24 => State->Registers[temp_24] })) ? 1U : 0U))) != 0) {
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
			var temp_25 = rt;
			if(temp_25 != 0)
				State->Registers[temp_25] = (uint) ((uint) (Copcreg(cop, rd)));
			return true;
		}
		insn_16:
		/* COP */
		if((insn & 0xF2000000) == 0x42000000) {
			var cop = (insn >> 26) & 0x3U;
			var command = (insn >> 0) & 0x1FFFFFFU;
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
			Copcreg(cop, rd, (uint) ((rt) switch { 0 => 0U, var temp_26 => State->Registers[temp_26] }));
			return true;
		}
		insn_18:
		/* DIV */
		if((insn & 0xFC00003F) == 0x0000001A) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			var rsv = (uint) ((rs) switch { 0 => 0U, var temp_27 => State->Registers[temp_27] });
			var rtv = (uint) ((rt) switch { 0 => 0U, var temp_28 => State->Registers[temp_28] });
			if(((uint) (((rtv) == ((uint) ((byte) 0x0)) ? 1U : 0U))) != 0) {
				State->Lo = (uint) ((byte) (((uint) ((((uint) ((((uint) (rsv)) & ((uint) (0x80000000U))))) != ((uint) ((byte) 0x0)) ? 1U : 0U)) != 0) ? (uint) (((byte) 0x1)) : (uint) ((0xFFFFFFFFU))));
				State->Hi = (uint) (rsv);
			} else {
				if(((uint) ((((uint) ((uint) (((rsv) == (0x80000000U) ? 1U : 0U)))) & ((uint) ((uint) (((rtv) == (0xFFFFFFFFU) ? 1U : 0U))))))) != 0) {
					State->Lo = (uint) (0x80000000U);
					State->Hi = (uint) ((byte) 0x0);
				} else {
					State->Lo = (uint) ((int) (((int) (int) ((int) ((int) (rsv)))) / ((int) (int) ((int) ((int) (rtv))))));
					State->Hi = (uint) ((int) (((int) (int) ((int) ((int) (rsv)))) % ((int) (int) ((int) ((int) (rtv))))));
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
			var rsv = (uint) ((rs) switch { 0 => 0U, var temp_29 => State->Registers[temp_29] });
			var rtv = (uint) ((rt) switch { 0 => 0U, var temp_30 => State->Registers[temp_30] });
			if(((uint) (((rtv) == ((uint) ((byte) 0x0)) ? 1U : 0U))) != 0) {
				State->Lo = (uint) (0xFFFFFFFFU);
				State->Hi = (uint) (rsv);
			} else {
				State->Lo = (uint) ((uint) (((uint) (uint) (rsv)) / ((uint) (uint) (rtv))));
				State->Hi = (uint) ((uint) (((uint) (uint) (rsv)) % ((uint) (uint) (rtv))));
			}
			return true;
		}
		insn_20:
		/* J */
		if((insn & 0xFC000000) == 0x08000000) {
			var imm = (insn >> 0) & 0x3FFFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((((uint) ((uint) ((pc + 4)))) & ((uint) (0xF0000000U)))))) + ((uint) (uint) ((uint) (((uint) ((uint) (imm))) << (int) ((byte) 0x2)))));
			Branch(target);
			return true;
		}
		insn_21:
		/* JAL */
		if((insn & 0xFC000000) == 0x0C000000) {
			var imm = (insn >> 0) & 0x3FFFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((((uint) ((uint) ((pc + 4)))) & ((uint) (0xF0000000U)))))) + ((uint) (uint) ((uint) (((uint) ((uint) (imm))) << (int) ((byte) 0x2)))));
			State->Registers[31] = (uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8)));
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
			var target = (uint) ((rs) switch { 0 => 0U, var temp_31 => State->Registers[temp_31] });
			var temp_32 = rd;
			if(temp_32 != 0)
				State->Registers[temp_32] = (uint) ((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
			if(((uint) ((((uint) ((byte) 0x0)) != ((uint) ((((uint) (target)) & ((uint) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (32))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1)))))))) ? 1U : 0U))) != 0) {
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
			var target = (uint) ((rs) switch { 0 => 0U, var temp_33 => State->Registers[temp_33] });
			if(((uint) ((((uint) ((byte) 0x0)) != ((uint) ((((uint) (target)) & ((uint) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (32))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1)))))))) ? 1U : 0U))) != 0) {
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
			State->LdWhich = (uint) (rt);
			State->LdValue = (uint) ((sbyte) (ReadMemory<sbyte>((uint) (((uint) (uint) ((uint) ((rs) switch { 0 => 0U, var temp_34 => State->Registers[temp_34] }))) + ((uint) (int) (offset))))));
			return true;
		}
		insn_25:
		/* LBU */
		if((insn & 0xFC000000) == 0x90000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			State->LdWhich = (uint) (rt);
			State->LdValue = (uint) ((byte) (ReadMemory<byte>((uint) (((uint) (uint) ((uint) ((rs) switch { 0 => 0U, var temp_35 => State->Registers[temp_35] }))) + ((uint) (int) (offset))))));
			return true;
		}
		insn_26:
		/* LH */
		if((insn & 0xFC000000) == 0x84000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			var addr = (uint) (((uint) (uint) ((uint) ((rs) switch { 0 => 0U, var temp_36 => State->Registers[temp_36] }))) + ((uint) (int) (offset)));
			if(((uint) ((((uint) ((byte) 0x0)) != ((uint) ((((uint) (addr)) & ((uint) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (16))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1)))))))) ? 1U : 0U))) != 0) {
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
			var addr = (uint) (((uint) (uint) ((uint) ((rs) switch { 0 => 0U, var temp_37 => State->Registers[temp_37] }))) + ((uint) (int) (offset)));
			if(((uint) ((((uint) ((byte) 0x0)) != ((uint) ((((uint) (addr)) & ((uint) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (16))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1)))))))) ? 1U : 0U))) != 0) {
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
			var temp_38 = rt;
			if(temp_38 != 0)
				State->Registers[temp_38] = (uint) ((ushort) ((imm) << (int) ((byte) 0x10)));
			return true;
		}
		insn_29:
		/* LW */
		if((insn & 0xFC000000) == 0x8C000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			var addr = (uint) (((uint) (uint) ((uint) ((rs) switch { 0 => 0U, var temp_39 => State->Registers[temp_39] }))) + ((uint) (int) (offset)));
			if(((uint) ((((uint) ((byte) 0x0)) != ((uint) ((((uint) (addr)) & ((uint) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (32))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1)))))))) ? 1U : 0U))) != 0) {
				throw new CpuException(ExceptionType.ADEL, pc, insn);
			}
			State->LdWhich = (uint) (rt);
			State->LdValue = (uint) ((uint) (ReadMemory<uint>(addr)));
			return true;
		}
		insn_30:
		/* LWC2 */
		if((insn & 0xFC000000) == 0xC8000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			var addr = (uint) (((uint) (uint) ((uint) ((rs) switch { 0 => 0U, var temp_40 => State->Registers[temp_40] }))) + ((uint) (int) (offset)));
			if(((uint) ((((uint) ((byte) 0x0)) != ((uint) ((((uint) (addr)) & ((uint) ((uint) (((uint) (uint) ((uint) (((uint) (int) ((int) (32))) / ((uint) (byte) ((byte) 0x8))))) - ((uint) (byte) ((byte) 0x1)))))))) ? 1U : 0U))) != 0) {
				throw new CpuException(ExceptionType.ADEL, pc, insn);
			}
			Copreg((byte) 0x2, rt, (uint) (ReadMemory<uint>(addr)));
			return true;
		}
		insn_31:
		/* MFC */
		if((insn & 0xF3E00000) == 0x40000000) {
			var cop = (insn >> 26) & 0x3U;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var cofun = (insn >> 0) & 0x7FFU;
			State->LdWhich = (uint) (rt);
			State->LdValue = (uint) ((uint) (Copreg(cop, rd)));
			return true;
		}
		insn_32:
		/* MFHI */
		if((insn & 0xFC00003F) == 0x00000010) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			var temp_41 = rd;
			if(temp_41 != 0)
				State->Registers[temp_41] = (uint) ((uint) (State->Hi));
			AbsorbMuldivDelay();
			return true;
		}
		insn_33:
		/* MFLO */
		if((insn & 0xFC00003F) == 0x00000012) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			var temp_42 = rd;
			if(temp_42 != 0)
				State->Registers[temp_42] = (uint) ((uint) (State->Lo));
			AbsorbMuldivDelay();
			return true;
		}
		insn_34:
		/* MTC */
		if((insn & 0xF3E00000) == 0x40800000) {
			var cop = (insn >> 26) & 0x3U;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var cofun = (insn >> 0) & 0x7FFU;
			Copreg(cop, rd, (uint) ((rt) switch { 0 => 0U, var temp_43 => State->Registers[temp_43] }));
			return true;
		}
		insn_35:
		/* MTHI */
		if((insn & 0xFC00003F) == 0x00000011) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->Hi = (uint) ((uint) ((rs) switch { 0 => 0U, var temp_44 => State->Registers[temp_44] }));
			return true;
		}
		insn_36:
		/* MTLO */
		if((insn & 0xFC00003F) == 0x00000013) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			State->Lo = (uint) ((uint) ((rs) switch { 0 => 0U, var temp_45 => State->Registers[temp_45] }));
			return true;
		}
		insn_37:
		/* MULT */
		if((insn & 0xFC00003F) == 0x00000018) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			var lhs = (uint) ((rs) switch { 0 => 0U, var temp_46 => State->Registers[temp_46] });
			var rhs = (uint) ((rt) switch { 0 => 0U, var temp_47 => State->Registers[temp_47] });
			var result = (ulong) ((ulong) ((long) (((long) (long) ((long) ((long) (lhs)))) * ((long) (long) ((long) ((long) (rhs)))))));
			State->Lo = (uint) (result);
			State->Hi = (uint) ((ulong) ((result) >> (int) ((byte) 0x20)));
			MulDelay(lhs, rhs, (byte) 0x1 != 0);
			return true;
		}
		insn_38:
		/* MULTU */
		if((insn & 0xFC00003F) == 0x00000019) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			var lhs = (uint) ((rs) switch { 0 => 0U, var temp_48 => State->Registers[temp_48] });
			var rhs = (uint) ((rt) switch { 0 => 0U, var temp_49 => State->Registers[temp_49] });
			var result = (ulong) (((ulong) (ulong) ((ulong) ((ulong) (lhs)))) * ((ulong) (ulong) ((ulong) ((ulong) (rhs)))));
			State->Lo = (uint) (result);
			State->Hi = (uint) ((ulong) ((result) >> (int) ((byte) 0x20)));
			MulDelay(lhs, rhs, (byte) 0x0 != 0);
			return true;
		}
		insn_39:
		/* NOR */
		if((insn & 0xFC00003F) == 0x00000027) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			var temp_50 = rd;
			if(temp_50 != 0)
				State->Registers[temp_50] = (uint) ((uint) (~((uint) ((((uint) ((uint) ((rs) switch { 0 => 0U, var temp_51 => State->Registers[temp_51] }))) | ((uint) ((uint) ((rt) switch { 0 => 0U, var temp_52 => State->Registers[temp_52] }))))))));
			return true;
		}
		insn_40:
		/* OR */
		if((insn & 0xFC00003F) == 0x00000025) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			var temp_53 = rd;
			if(temp_53 != 0)
				State->Registers[temp_53] = (uint) ((uint) ((((uint) ((uint) ((rs) switch { 0 => 0U, var temp_54 => State->Registers[temp_54] }))) | ((uint) ((uint) ((rt) switch { 0 => 0U, var temp_55 => State->Registers[temp_55] }))))));
			return true;
		}
		insn_41:
		/* ORI */
		if((insn & 0xFC000000) == 0x34000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var temp_56 = rt;
			if(temp_56 != 0)
				State->Registers[temp_56] = (uint) ((uint) ((((uint) ((uint) ((rs) switch { 0 => 0U, var temp_57 => State->Registers[temp_57] }))) | ((uint) ((uint) ((uint) (imm)))))));
			return true;
		}
		insn_42:

        return false;
    }
}
