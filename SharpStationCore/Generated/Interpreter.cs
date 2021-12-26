// ReSharper disable CheckNamespace
// ReSharper disable ArrangeRedundantParentheses
// ReSharper disable RedundantCast
#pragma warning disable CS0164
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
			var temp_18 = (byte) 0x1F;
			if(temp_18 != 0)
				State->Registers[temp_18] = (uint) ((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
			if(((uint) ((((int) ((int) ((uint) ((rs) switch { 0 => 0U, var temp_19 => State->Registers[temp_19] })))) >= ((int) ((sbyte) ((byte) 0x0))) ? 1U : 0U))) != 0) {
				Branch(target);
			} else {
				Branch((uint) (((uint) (uint) ((uint) (pc))) + ((uint) (byte) ((byte) 0x8))));
			}
			return true;
		}
		insn_9:

        return false;
    }
}
