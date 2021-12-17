// ReSharper disable CheckNamespace
#pragma warning disable CS0164
namespace DamageCore;

public partial class Interpreter {
    public bool Interpret(Span<byte> insnBytes, ref ushort pc) {
	/* LD-rd-rs */
	if((insnBytes[0] & 0xC0) == 0x40) {
		var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
		var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
		if(((uint) (((rs) != (0x6U)) ? 1U : 0U)) == 0)
			goto insn_1;
		pc += 1;
		Registers[(int) rd] = (byte) ((byte) ((rs) switch { 0b110 => throw new NotSupportedException(), {} i => Registers[i] }));
		return true;
	}
	insn_1:
	/* LD-rd-imm8 */
	if((insnBytes[0] & 0xC7) == 0x6) {
		var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
		var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		if(((uint) (((rd) != (0x6U)) ? 1U : 0U)) == 0)
			goto insn_2;
		pc += 2;
		Registers[(int) rd] = (byte) (imm);
		return true;
	}
	insn_2:
	/* LD-rd-HL */
	if((insnBytes[0] & 0xC7) == 0x46) {
		var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
		if(((uint) (((rd) != (0x6U)) ? 1U : 0U)) == 0)
			goto insn_3;
		pc += 1;
		Registers[(int) rd] = (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) Registers[0b100]) << 8) | (ushort) Registers[0b101])))));
		return true;
	}
	insn_3:
	/* LD-HL-rs */
	if((insnBytes[0] & 0xF8) == 0x70) {
		var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
		if(((uint) (((rs) != (0x6U)) ? 1U : 0U)) == 0)
			goto insn_4;
		pc += 1;
		WriteMemory((ushort) (((((ushort) Registers[0b100]) << 8) | (ushort) Registers[0b101])), (byte) ((rs) switch { 0b110 => throw new NotSupportedException(), {} i => Registers[i] }));
		return true;
	}
	insn_4:
	/* LD-HL-imm8 */
	if((insnBytes[0] & 0xFF) == 0x36) {
		var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		pc += 2;
		WriteMemory((ushort) (((((ushort) Registers[0b100]) << 8) | (ushort) Registers[0b101])), imm);
		return true;
	}
	insn_5:
	/* LD-A-BC */
	if((insnBytes[0] & 0xFF) == 0xA) {
		pc += 1;
		Registers[(int) 0x7U] = (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) Registers[0b000]) << 8) | (ushort) Registers[0b001])))));
		return true;
	}
	insn_6:
	/* LD-A-DE */
	if((insnBytes[0] & 0xFF) == 0x1A) {
		pc += 1;
		Registers[(int) 0x7U] = (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) Registers[0b010]) << 8) | (ushort) Registers[0b011])))));
		return true;
	}
	insn_7:
	/* LD-BC-A */
	if((insnBytes[0] & 0xFF) == 0x2) {
		pc += 1;
		WriteMemory((ushort) (((((ushort) Registers[0b000]) << 8) | (ushort) Registers[0b001])), (byte) ((0x7U) switch { 0b110 => throw new NotSupportedException(), {} i => Registers[i] }));
		return true;
	}
	insn_8:
	/* LD-DE-A */
	if((insnBytes[0] & 0xFF) == 0x12) {
		pc += 1;
		WriteMemory((ushort) (((((ushort) Registers[0b010]) << 8) | (ushort) Registers[0b011])), (byte) ((0x7U) switch { 0b110 => throw new NotSupportedException(), {} i => Registers[i] }));
		return true;
	}
	insn_9:
	/* LD-A-imm16 */
	if((insnBytes[0] & 0xFF) == 0xFA) {
		var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
		var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
		pc += 3;
		Registers[(int) 0x7U] = (byte) ((byte) (ReadMemory<byte>(addr)));
		return true;
	}
	insn_10:
	/* LD-imm16-A */
	if((insnBytes[0] & 0xFF) == 0xEA) {
		var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
		var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
		pc += 3;
		WriteMemory(addr, (byte) ((0x7U) switch { 0b110 => throw new NotSupportedException(), {} i => Registers[i] }));
		return true;
	}
	insn_11:
	/* LDH-A-C */
	if((insnBytes[0] & 0xFF) == 0xF2) {
		pc += 1;
		Registers[(int) 0x7U] = (byte) ((byte) (ReadMemory<byte>((ushort) ((((ushort) ((ushort) ((ushort) (0xFF00U)))) | ((ushort) ((byte) ((0x1U) switch { 0b110 => throw new NotSupportedException(), {} i => Registers[i] }))))))));
		return true;
	}
	insn_12:
	/* LDH-C-A */
	if((insnBytes[0] & 0xFF) == 0xE2) {
		pc += 1;
		return true;
	}
	insn_13:
	/* LDH-A-imm8 */
	if((insnBytes[0] & 0xFF) == 0xF0) {
		var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		var addr = (ushort) ((((ushort) ((ushort) ((ushort) (0xFF00U)))) | ((ushort) (imm))));
		pc += 2;
		Registers[(int) 0x7U] = (byte) ((byte) (ReadMemory<byte>(addr)));
		return true;
	}
	insn_14:
	/* LDH-imm8-A */
	if((insnBytes[0] & 0xFF) == 0xE0) {
		var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		var addr = (ushort) ((((ushort) ((ushort) ((ushort) (0xFF00U)))) | ((ushort) (imm))));
		pc += 2;
		WriteMemory(addr, (byte) ((0x7U) switch { 0b110 => throw new NotSupportedException(), {} i => Registers[i] }));
		return true;
	}
	insn_15:
	/* LD-A-HL- */
	if((insnBytes[0] & 0xFF) == 0x3A) {
		pc += 1;
		var hl = (ushort) (((((ushort) Registers[0b100]) << 8) | (ushort) Registers[0b101]));
		Registers[(int) 0x7U] = (byte) ((byte) (ReadMemory<byte>(hl)));
		var temp_9 = (ushort) (ushort) (((ushort) (ushort) (hl)) - ((ushort) (byte) (0x1U)));
		Registers[0b100] = (byte) (temp_9 >> 8);
		Registers[0b101] = (byte) (temp_9 & 0xFF);
		return true;
	}
	insn_16:
	/* LD-HL--A */
	if((insnBytes[0] & 0xFF) == 0x32) {
		pc += 1;
		var hl = (ushort) (((((ushort) Registers[0b100]) << 8) | (ushort) Registers[0b101]));
		WriteMemory(hl, (byte) ((0x7U) switch { 0b110 => throw new NotSupportedException(), {} i => Registers[i] }));
		var temp_10 = (ushort) (ushort) (((ushort) (ushort) (hl)) - ((ushort) (byte) (0x1U)));
		Registers[0b100] = (byte) (temp_10 >> 8);
		Registers[0b101] = (byte) (temp_10 & 0xFF);
		return true;
	}
	insn_17:
	/* LD-A-HL+ */
	if((insnBytes[0] & 0xFF) == 0x2A) {
		pc += 1;
		var hl = (ushort) (((((ushort) Registers[0b100]) << 8) | (ushort) Registers[0b101]));
		Registers[(int) 0x7U] = (byte) ((byte) (ReadMemory<byte>(hl)));
		var temp_11 = (ushort) (ushort) (((ushort) (ushort) (hl)) + ((ushort) (byte) (0x1U)));
		Registers[0b100] = (byte) (temp_11 >> 8);
		Registers[0b101] = (byte) (temp_11 & 0xFF);
		return true;
	}
	insn_18:
	/* LD-HL+-A */
	if((insnBytes[0] & 0xFF) == 0x22) {
		pc += 1;
		var hl = (ushort) (((((ushort) Registers[0b100]) << 8) | (ushort) Registers[0b101]));
		WriteMemory(hl, (byte) ((0x7U) switch { 0b110 => throw new NotSupportedException(), {} i => Registers[i] }));
		var temp_12 = (ushort) (ushort) (((ushort) (ushort) (hl)) + ((ushort) (byte) (0x1U)));
		Registers[0b100] = (byte) (temp_12 >> 8);
		Registers[0b101] = (byte) (temp_12 & 0xFF);
		return true;
	}
	insn_19:
	/* LD-rr-imm16 */
	if((insnBytes[0] & 0xCF) == 0x1) {
		var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
		var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
		var imm = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
		pc += 3;
		switch(r) {
			case (byte) (0x0U): {
				var temp_13 = (ushort) imm;
				Registers[0b000] = (byte) (temp_13 >> 8);
				Registers[0b001] = (byte) (temp_13 & 0xFF);
				break;
			}
			case (byte) (0x1U): {
				var temp_14 = (ushort) imm;
				Registers[0b010] = (byte) (temp_14 >> 8);
				Registers[0b011] = (byte) (temp_14 & 0xFF);
				break;
			}
			case (byte) (0x2U): {
				var temp_15 = (ushort) imm;
				Registers[0b100] = (byte) (temp_15 >> 8);
				Registers[0b101] = (byte) (temp_15 & 0xFF);
				break;
			}
			default: {
				SP = (ushort) imm;
				break;
			}
		}
		return true;
	}
	insn_20:
	/* LD-imm16-SP */
	if((insnBytes[0] & 0xFF) == 0x8) {
		var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
		var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
		pc += 3;
		WriteMemory(addr, (ushort) (SP));
		return true;
	}
	insn_21:
	/* LD-SP-HL */
	if((insnBytes[0] & 0xFF) == 0xF9) {
		pc += 1;
		SP = (ushort) (ushort) (((((ushort) Registers[0b100]) << 8) | (ushort) Registers[0b101]));
		return true;
	}
	insn_22:
	/* PUSH-rr */
	if((insnBytes[0] & 0xCF) == 0xC5) {
		var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
		pc += 1;
		var sp = (ushort) (((ushort) (ushort) ((ushort) (SP))) - ((ushort) (byte) (0x2U)));
		SP = (ushort) sp;
		WriteMemory(sp, (ushort) (r switch { (byte) (0x0U) => (ushort) (((((ushort) Registers[0b000]) << 8) | (ushort) Registers[0b001])), (byte) (0x1U) => (ushort) (((((ushort) Registers[0b010]) << 8) | (ushort) Registers[0b011])), (byte) (0x2U) => (ushort) (((((ushort) Registers[0b100]) << 8) | (ushort) Registers[0b101])), _ => (ushort) (((((ushort) Registers[0b111]) << 8) | (ushort) Flags)) }));
		return true;
	}
	insn_23:
	/* POP-rr */
	if((insnBytes[0] & 0xCF) == 0xC1) {
		var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
		pc += 1;
		var sp = (ushort) (SP);
		var v = (ushort) (ReadMemory<ushort>(sp));
		switch(r) {
			case (byte) (0x0U): {
				var temp_17 = (ushort) v;
				Registers[0b000] = (byte) (temp_17 >> 8);
				Registers[0b001] = (byte) (temp_17 & 0xFF);
				break;
			}
			case (byte) (0x1U): {
				var temp_18 = (ushort) v;
				Registers[0b010] = (byte) (temp_18 >> 8);
				Registers[0b011] = (byte) (temp_18 & 0xFF);
				break;
			}
			case (byte) (0x2U): {
				var temp_19 = (ushort) v;
				Registers[0b100] = (byte) (temp_19 >> 8);
				Registers[0b101] = (byte) (temp_19 & 0xFF);
				break;
			}
			default: {
				var temp_20 = (ushort) v;
				Registers[0b111] = (byte) (temp_20 >> 8);
				Flags = (byte) (temp_20 & 0xFF);
				break;
			}
		}
		SP = (ushort) (ushort) (((ushort) (ushort) (sp)) + ((ushort) (byte) (0x2U)));
		return true;
	}
	insn_24:
	/* JP-nn */
	if((insnBytes[0] & 0xFF) == 0xC3) {
		var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
		var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
		pc += 3;
		Branch(addr);
		return true;
	}
	insn_25:
	/* JP-HL */
	if((insnBytes[0] & 0xFF) == 0xE9) {
		pc += 1;
		Branch((ushort) (((((ushort) Registers[0b100]) << 8) | (ushort) Registers[0b101])));
		return true;
	}
	insn_26:
	/* JP-cc-imm16 */
	if((insnBytes[0] & 0xE7) == 0xC2) {
		var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
		var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
		var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
		pc += 3;
		if(((uint) ((byte) ((cc) >> (int) (0x1U)) switch { (byte) (0x0U) => (uint) ((((byte) (((byte) (Flags)) >> (int) (0x7U))) == ((byte) ((((byte) (cc)) & ((byte) (0x1U)))))) ? 1U : 0U), _ => (uint) ((((byte) ((((byte) ((byte) (((byte) (Flags)) >> (int) (0x6U)))) & ((byte) (0x1U))))) == ((byte) ((((byte) (cc)) & ((byte) (0x1U)))))) ? 1U : 0U) })) != 0) {
			Branch(addr);
		} else {
			Branch(pc);
		}
		return true;
	}
	insn_27:
	/* JR-simm8 */
	if((insnBytes[0] & 0xFF) == 0x18) {
		var e = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		var offset = (sbyte) ((sbyte) (e));
		pc += 2;
		Branch((ushort) (((ushort) (ushort) ((ushort) (pc))) + ((ushort) (sbyte) (offset))));
		return true;
	}
	insn_28:

        return false;
    }
}
