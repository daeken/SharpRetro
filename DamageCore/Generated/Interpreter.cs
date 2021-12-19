// ReSharper disable CheckNamespace
// ReSharper disable ArrangeRedundantParentheses
// ReSharper disable RedundantCast
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
		State.Registers[(int) rd] = (byte) ((byte) ((rs) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }));
		AddCycles(0x1U);
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
		State.Registers[(int) rd] = (byte) (imm);
		AddCycles(0x2U);
		return true;
	}
	insn_2:
	/* LD-rd-HL */
	if((insnBytes[0] & 0xC7) == 0x46) {
		var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
		if(((uint) (((rd) != (0x6U)) ? 1U : 0U)) == 0)
			goto insn_3;
		pc += 1;
		State.Registers[(int) rd] = (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])))));
		AddCycles(0x2U);
		return true;
	}
	insn_3:
	/* LD-HL-rs */
	if((insnBytes[0] & 0xF8) == 0x70) {
		var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
		if(((uint) (((rs) != (0x6U)) ? 1U : 0U)) == 0)
			goto insn_4;
		pc += 1;
		WriteMemory((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])), (byte) ((rs) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }));
		AddCycles(0x2U);
		return true;
	}
	insn_4:
	/* LD-HL-imm8 */
	if((insnBytes[0] & 0xFF) == 0x36) {
		var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		pc += 2;
		WriteMemory((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])), imm);
		AddCycles(0x3U);
		return true;
	}
	insn_5:
	/* LD-A-BC */
	if((insnBytes[0] & 0xFF) == 0xA) {
		pc += 1;
		State.Registers[(int) 0x7U] = (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b000]) << 8) | (ushort) State.Registers[0b001])))));
		AddCycles(0x2U);
		return true;
	}
	insn_6:
	/* LD-A-DE */
	if((insnBytes[0] & 0xFF) == 0x1A) {
		pc += 1;
		State.Registers[(int) 0x7U] = (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b010]) << 8) | (ushort) State.Registers[0b011])))));
		AddCycles(0x2U);
		return true;
	}
	insn_7:
	/* LD-BC-A */
	if((insnBytes[0] & 0xFF) == 0x2) {
		pc += 1;
		WriteMemory((ushort) (((((ushort) State.Registers[0b000]) << 8) | (ushort) State.Registers[0b001])), (byte) ((0x7U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }));
		AddCycles(0x2U);
		return true;
	}
	insn_8:
	/* LD-DE-A */
	if((insnBytes[0] & 0xFF) == 0x12) {
		pc += 1;
		WriteMemory((ushort) (((((ushort) State.Registers[0b010]) << 8) | (ushort) State.Registers[0b011])), (byte) ((0x7U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }));
		AddCycles(0x2U);
		return true;
	}
	insn_9:
	/* LD-A-imm16 */
	if((insnBytes[0] & 0xFF) == 0xFA) {
		var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
		var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
		pc += 3;
		State.Registers[(int) 0x7U] = (byte) ((byte) (ReadMemory<byte>(addr)));
		AddCycles(0x4U);
		return true;
	}
	insn_10:
	/* LD-imm16-A */
	if((insnBytes[0] & 0xFF) == 0xEA) {
		var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
		var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
		pc += 3;
		WriteMemory(addr, (byte) ((0x7U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }));
		AddCycles(0x4U);
		return true;
	}
	insn_11:
	/* LDH-A-C */
	if((insnBytes[0] & 0xFF) == 0xF2) {
		pc += 1;
		State.Registers[(int) 0x7U] = (byte) ((byte) (ReadMemory<byte>((ushort) ((((ushort) ((ushort) ((ushort) (0xFF00U)))) | ((ushort) ((byte) ((0x1U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }))))))));
		AddCycles(0x2U);
		return true;
	}
	insn_12:
	/* LDH-C-A */
	if((insnBytes[0] & 0xFF) == 0xE2) {
		pc += 1;
		AddCycles(0x2U);
		return true;
	}
	insn_13:
	/* LDH-A-imm8 */
	if((insnBytes[0] & 0xFF) == 0xF0) {
		var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		var addr = (ushort) ((((ushort) ((ushort) ((ushort) (0xFF00U)))) | ((ushort) (imm))));
		pc += 2;
		State.Registers[(int) 0x7U] = (byte) ((byte) (ReadMemory<byte>(addr)));
		AddCycles(0x3U);
		return true;
	}
	insn_14:
	/* LDH-imm8-A */
	if((insnBytes[0] & 0xFF) == 0xE0) {
		var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		var addr = (ushort) ((((ushort) ((ushort) ((ushort) (0xFF00U)))) | ((ushort) (imm))));
		pc += 2;
		WriteMemory(addr, (byte) ((0x7U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }));
		AddCycles(0x3U);
		return true;
	}
	insn_15:
	/* LD-A-HL- */
	if((insnBytes[0] & 0xFF) == 0x3A) {
		pc += 1;
		var hl = (ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]));
		State.Registers[(int) 0x7U] = (byte) ((byte) (ReadMemory<byte>(hl)));
		var temp_16 = (ushort) (ushort) (((ushort) (ushort) (hl)) - ((ushort) (byte) (0x1U)));
		State.Registers[0b100] = (byte) (temp_16 >> 8);
		State.Registers[0b101] = (byte) (temp_16 & 0xFF);
		AddCycles(0x2U);
		return true;
	}
	insn_16:
	/* LD-HL--A */
	if((insnBytes[0] & 0xFF) == 0x32) {
		pc += 1;
		var hl = (ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]));
		WriteMemory(hl, (byte) ((0x7U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }));
		var temp_17 = (ushort) (ushort) (((ushort) (ushort) (hl)) - ((ushort) (byte) (0x1U)));
		State.Registers[0b100] = (byte) (temp_17 >> 8);
		State.Registers[0b101] = (byte) (temp_17 & 0xFF);
		AddCycles(0x2U);
		return true;
	}
	insn_17:
	/* LD-A-HL+ */
	if((insnBytes[0] & 0xFF) == 0x2A) {
		pc += 1;
		var hl = (ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]));
		State.Registers[(int) 0x7U] = (byte) ((byte) (ReadMemory<byte>(hl)));
		var temp_18 = (ushort) (ushort) (((ushort) (ushort) (hl)) + ((ushort) (byte) (0x1U)));
		State.Registers[0b100] = (byte) (temp_18 >> 8);
		State.Registers[0b101] = (byte) (temp_18 & 0xFF);
		AddCycles(0x2U);
		return true;
	}
	insn_18:
	/* LD-HL+-A */
	if((insnBytes[0] & 0xFF) == 0x22) {
		pc += 1;
		var hl = (ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]));
		WriteMemory(hl, (byte) ((0x7U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }));
		var temp_19 = (ushort) (ushort) (((ushort) (ushort) (hl)) + ((ushort) (byte) (0x1U)));
		State.Registers[0b100] = (byte) (temp_19 >> 8);
		State.Registers[0b101] = (byte) (temp_19 & 0xFF);
		AddCycles(0x2U);
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
				var temp_20 = (ushort) imm;
				State.Registers[0b000] = (byte) (temp_20 >> 8);
				State.Registers[0b001] = (byte) (temp_20 & 0xFF);
				break;
			}
			case (byte) (0x1U): {
				var temp_21 = (ushort) imm;
				State.Registers[0b010] = (byte) (temp_21 >> 8);
				State.Registers[0b011] = (byte) (temp_21 & 0xFF);
				break;
			}
			case (byte) (0x2U): {
				var temp_22 = (ushort) imm;
				State.Registers[0b100] = (byte) (temp_22 >> 8);
				State.Registers[0b101] = (byte) (temp_22 & 0xFF);
				break;
			}
			default: {
				State.SP = (ushort) imm;
				break;
			}
		}
		AddCycles(0x3U);
		return true;
	}
	insn_20:
	/* LD-imm16-SP */
	if((insnBytes[0] & 0xFF) == 0x8) {
		var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
		var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
		pc += 3;
		WriteMemory(addr, (ushort) (State.SP));
		AddCycles(0x5U);
		return true;
	}
	insn_21:
	/* LD-SP-HL */
	if((insnBytes[0] & 0xFF) == 0xF9) {
		pc += 1;
		State.SP = (ushort) (ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]));
		AddCycles(0x2U);
		return true;
	}
	insn_22:
	/* PUSH-rr */
	if((insnBytes[0] & 0xCF) == 0xC5) {
		var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
		pc += 1;
		var sp = (ushort) (((ushort) (ushort) ((ushort) (State.SP))) - ((ushort) (byte) (0x2U)));
		State.SP = (ushort) sp;
		WriteMemory(sp, (ushort) (r switch { (byte) (0x0U) => (ushort) (((((ushort) State.Registers[0b000]) << 8) | (ushort) State.Registers[0b001])), (byte) (0x1U) => (ushort) (((((ushort) State.Registers[0b010]) << 8) | (ushort) State.Registers[0b011])), (byte) (0x2U) => (ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])), _ => (ushort) (((((ushort) State.Registers[0b111]) << 8) | (ushort) State.Flags)) }));
		AddCycles(0x4U);
		return true;
	}
	insn_23:
	/* POP-rr */
	if((insnBytes[0] & 0xCF) == 0xC1) {
		var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
		pc += 1;
		var sp = (ushort) (State.SP);
		var v = (ushort) (ReadMemory<ushort>(sp));
		switch(r) {
			case (byte) (0x0U): {
				var temp_24 = (ushort) v;
				State.Registers[0b000] = (byte) (temp_24 >> 8);
				State.Registers[0b001] = (byte) (temp_24 & 0xFF);
				break;
			}
			case (byte) (0x1U): {
				var temp_25 = (ushort) v;
				State.Registers[0b010] = (byte) (temp_25 >> 8);
				State.Registers[0b011] = (byte) (temp_25 & 0xFF);
				break;
			}
			case (byte) (0x2U): {
				var temp_26 = (ushort) v;
				State.Registers[0b100] = (byte) (temp_26 >> 8);
				State.Registers[0b101] = (byte) (temp_26 & 0xFF);
				break;
			}
			default: {
				var temp_27 = (ushort) v;
				State.Registers[0b111] = (byte) (temp_27 >> 8);
				State.Flags = (byte) (temp_27 & 0xFF);
				break;
			}
		}
		State.SP = (ushort) (ushort) (((ushort) (ushort) (sp)) + ((ushort) (byte) (0x2U)));
		AddCycles(0x4U);
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
		AddCycles(0x4U);
		return true;
	}
	insn_25:
	/* JP-HL */
	if((insnBytes[0] & 0xFF) == 0xE9) {
		pc += 1;
		Branch((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])));
		AddCycles(0x1U);
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
		if(((uint) ((byte) ((cc) >> (int) (0x1U)) switch { (byte) (0x0U) => (uint) ((((byte) (((byte) (State.Flags)) >> (int) (0x7U))) == ((byte) ((((byte) (cc)) & ((byte) (0x1U)))))) ? 1U : 0U), _ => (uint) ((((byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) (0x6U)))) & ((byte) (0x1U))))) == ((byte) ((((byte) (cc)) & ((byte) (0x1U)))))) ? 1U : 0U) })) != 0) {
			Branch(addr);
		} else {
			Branch(pc);
		}
		AddCycles((byte) (((uint) ((byte) ((cc) >> (int) (0x1U)) switch { (byte) (0x0U) => (uint) ((((byte) (((byte) (State.Flags)) >> (int) (0x7U))) == ((byte) ((((byte) (cc)) & ((byte) (0x1U)))))) ? 1U : 0U), _ => (uint) ((((byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) (0x6U)))) & ((byte) (0x1U))))) == ((byte) ((((byte) (cc)) & ((byte) (0x1U)))))) ? 1U : 0U) }) != 0) ? (0x4U) : (0x3U)));
		return true;
	}
	insn_27:
	/* JR-simm8 */
	if((insnBytes[0] & 0xFF) == 0x18) {
		var e = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		var offset = (sbyte) ((sbyte) (e));
		pc += 2;
		Branch((ushort) (((ushort) (ushort) ((ushort) (pc))) + ((ushort) (sbyte) (offset))));
		AddCycles(0x3U);
		return true;
	}
	insn_28:
	/* JR-cc-simm8 */
	if((insnBytes[0] & 0xE7) == 0x20) {
		var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
		var e = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		var offset = (sbyte) ((sbyte) (e));
		pc += 2;
		if(((uint) ((byte) ((cc) >> (int) (0x1U)) switch { (byte) (0x0U) => (uint) ((((byte) (((byte) (State.Flags)) >> (int) (0x7U))) == ((byte) ((((byte) (cc)) & ((byte) (0x1U)))))) ? 1U : 0U), _ => (uint) ((((byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) (0x6U)))) & ((byte) (0x1U))))) == ((byte) ((((byte) (cc)) & ((byte) (0x1U)))))) ? 1U : 0U) })) != 0) {
			Branch((ushort) (((ushort) (ushort) ((ushort) (pc))) + ((ushort) (sbyte) (offset))));
		} else {
			Branch(pc);
		}
		AddCycles((byte) (((uint) ((byte) ((cc) >> (int) (0x1U)) switch { (byte) (0x0U) => (uint) ((((byte) (((byte) (State.Flags)) >> (int) (0x7U))) == ((byte) ((((byte) (cc)) & ((byte) (0x1U)))))) ? 1U : 0U), _ => (uint) ((((byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) (0x6U)))) & ((byte) (0x1U))))) == ((byte) ((((byte) (cc)) & ((byte) (0x1U)))))) ? 1U : 0U) }) != 0) ? (0x3U) : (0x2U)));
		return true;
	}
	insn_29:
	/* CALL-imm16 */
	if((insnBytes[0] & 0xFF) == 0xCD) {
		var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
		var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
		pc += 3;
		var sp = (ushort) (((ushort) (ushort) ((ushort) (State.SP))) - ((ushort) (byte) (0x2U)));
		State.SP = (ushort) sp;
		WriteMemory(sp, (ushort) (pc));
		Branch(addr);
		AddCycles(0x6U);
		return true;
	}
	insn_30:
	/* CALL-cc-imm16 */
	if((insnBytes[0] & 0xE7) == 0xC4) {
		var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
		var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
		var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
		pc += 3;
		if(((uint) ((byte) ((cc) >> (int) (0x1U)) switch { (byte) (0x0U) => (uint) ((((byte) (((byte) (State.Flags)) >> (int) (0x7U))) == ((byte) ((((byte) (cc)) & ((byte) (0x1U)))))) ? 1U : 0U), _ => (uint) ((((byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) (0x6U)))) & ((byte) (0x1U))))) == ((byte) ((((byte) (cc)) & ((byte) (0x1U)))))) ? 1U : 0U) })) != 0) {
			var sp = (ushort) (((ushort) (ushort) ((ushort) (State.SP))) - ((ushort) (byte) (0x2U)));
			State.SP = (ushort) sp;
			WriteMemory(sp, (ushort) (pc));
			Branch(addr);
		} else {
			Branch(pc);
		}
		AddCycles((byte) (((uint) ((byte) ((cc) >> (int) (0x1U)) switch { (byte) (0x0U) => (uint) ((((byte) (((byte) (State.Flags)) >> (int) (0x7U))) == ((byte) ((((byte) (cc)) & ((byte) (0x1U)))))) ? 1U : 0U), _ => (uint) ((((byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) (0x6U)))) & ((byte) (0x1U))))) == ((byte) ((((byte) (cc)) & ((byte) (0x1U)))))) ? 1U : 0U) }) != 0) ? (0x6U) : (0x3U)));
		return true;
	}
	insn_31:
	/* RET */
	if((insnBytes[0] & 0xFF) == 0xC9) {
		pc += 1;
		var sp = (ushort) (State.SP);
		var ra = (ushort) (ReadMemory<ushort>(sp));
		State.SP = (ushort) (ushort) (((ushort) (ushort) (sp)) + ((ushort) (byte) (0x2U)));
		Branch(ra);
		AddCycles(0x4U);
		return true;
	}
	insn_32:
	/* RET-cc */
	if((insnBytes[0] & 0xE7) == 0xC0) {
		var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
		pc += 1;
		if(((uint) ((byte) ((cc) >> (int) (0x1U)) switch { (byte) (0x0U) => (uint) ((((byte) (((byte) (State.Flags)) >> (int) (0x7U))) == ((byte) ((((byte) (cc)) & ((byte) (0x1U)))))) ? 1U : 0U), _ => (uint) ((((byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) (0x6U)))) & ((byte) (0x1U))))) == ((byte) ((((byte) (cc)) & ((byte) (0x1U)))))) ? 1U : 0U) })) != 0) {
			var sp = (ushort) (State.SP);
			var ra = (ushort) (ReadMemory<ushort>(sp));
			State.SP = (ushort) (ushort) (((ushort) (ushort) (sp)) + ((ushort) (byte) (0x2U)));
			Branch(ra);
		} else {
			Branch(pc);
		}
		AddCycles((byte) (((uint) ((byte) ((cc) >> (int) (0x1U)) switch { (byte) (0x0U) => (uint) ((((byte) (((byte) (State.Flags)) >> (int) (0x7U))) == ((byte) ((((byte) (cc)) & ((byte) (0x1U)))))) ? 1U : 0U), _ => (uint) ((((byte) ((((byte) ((byte) (((byte) (State.Flags)) >> (int) (0x6U)))) & ((byte) (0x1U))))) == ((byte) ((((byte) (cc)) & ((byte) (0x1U)))))) ? 1U : 0U) }) != 0) ? (0x5U) : (0x2U)));
		return true;
	}
	insn_33:
	/* RETI */
	if((insnBytes[0] & 0xFF) == 0xD9) {
		pc += 1;
		var sp = (ushort) (State.SP);
		var ra = (ushort) (ReadMemory<ushort>(sp));
		State.SP = (ushort) (ushort) (((ushort) (ushort) (sp)) + ((ushort) (byte) (0x2U)));
		State.InterruptsEnabled = true;
		Branch(ra);
		AddCycles(0x4U);
		return true;
	}
	insn_34:
	/* RST */
	if((insnBytes[0] & 0xC7) == 0xC7) {
		var n = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
		var addr = (byte) ((n) << (int) (0x3U));
		pc += 1;
		var sp = (ushort) (((ushort) (ushort) ((ushort) (State.SP))) - ((ushort) (byte) (0x2U)));
		State.SP = (ushort) sp;
		WriteMemory(sp, (ushort) (pc));
		Branch(addr);
		AddCycles(0x4U);
		return true;
	}
	insn_35:
	/* DI */
	if((insnBytes[0] & 0xFF) == 0xF3) {
		pc += 1;
		State.InterruptsEnabled = false;
		State.InterruptsEnableScheduled = false;
		AddCycles(0x1U);
		return true;
	}
	insn_36:
	/* EI */
	if((insnBytes[0] & 0xFF) == 0xFB) {
		pc += 1;
		State.InterruptsEnableScheduled = true;
		AddCycles(0x1U);
		return true;
	}
	insn_37:
	/* CCF */
	if((insnBytes[0] & 0xFF) == 0x3F) {
		pc += 1;
		var flags = (byte) (State.Flags);
		flags = (byte) ((((byte) (flags)) & ((byte) (0x9FU))));
		State.Flags = (byte) ((((byte) (flags)) ^ ((byte) (0x10U))));
		AddCycles(0x1U);
		return true;
	}
	insn_38:
	/* SCF */
	if((insnBytes[0] & 0xFF) == 0x37) {
		pc += 1;
		var flags = (byte) (State.Flags);
		flags = (byte) ((((byte) (flags)) & ((byte) (0x8FU))));
		State.Flags = (byte) ((((byte) (flags)) | ((byte) (0x10U))));
		AddCycles(0x1U);
		return true;
	}
	insn_39:
	/* CPL */
	if((insnBytes[0] & 0xFF) == 0x2F) {
		pc += 1;
		State.Flags = (byte) ((((byte) ((byte) (State.Flags))) | ((byte) (0x60U))));
		State.Registers[(int) 0x7U] = (byte) ((byte) (~((byte) ((0x7U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }))));
		AddCycles(0x1U);
		return true;
	}
	insn_40:
	/* NOP */
	if((insnBytes[0] & 0xFF) == 0x0) {
		pc += 1;
		AddCycles(0x1U);
		return true;
	}
	insn_41:
	/* INC-8 */
	if((insnBytes[0] & 0xC7) == 0x4) {
		var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
		pc += 1;
		var lhs = (byte) (((uint) (((rd) == (0x6U)) ? 1U : 0U) != 0) ? ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))) : ((byte) ((rd) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })));
		var result = (byte) (((byte) (byte) (lhs)) + ((byte) (byte) (0x1U)));
		State.Flags = (byte) ((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) (0x1FU)))))) | ((byte) ((byte) (((byte) ((byte) ((uint) ((((byte) ((byte) (result))) == ((byte) ((byte) (0x0U)))) ? 1U : 0U)))) << (int) (0x7U))))) | ((byte) ((byte) (((byte) ((byte) (0x0U))) << (int) (0x6U))))) | ((byte) ((byte) (((byte) ((byte) ((uint) ((((byte) (((byte) (byte) ((byte) ((((byte) (lhs)) & ((byte) (0xFU)))))) + ((byte) (byte) ((byte) ((((byte) (0x1U)) & ((byte) (0xFU)))))))) > (0xFU)) ? 1U : 0U)))) << (int) (0x5U))))));
		if(((uint) (((rd) == (0x6U)) ? 1U : 0U)) != 0) {
			WriteMemory((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])), result);
		} else {
			State.Registers[(int) rd] = (byte) (result);
		}
		AddCycles((byte) (((uint) (((rd) == (0x6U)) ? 1U : 0U) != 0) ? (0x3U) : (0x1U)));
		return true;
	}
	insn_42:
	/* DEC-8 */
	if((insnBytes[0] & 0xC7) == 0x5) {
		var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
		pc += 1;
		var lhs = (byte) (((uint) (((rd) == (0x6U)) ? 1U : 0U) != 0) ? ((byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101]))))) : ((byte) ((rd) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] })));
		var result = (byte) (((byte) (byte) (lhs)) - ((byte) (byte) (0x1U)));
		State.Flags = (byte) ((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) (0x1FU)))))) | ((byte) ((byte) (((byte) ((byte) ((uint) ((((byte) ((byte) (result))) == ((byte) ((byte) (0x0U)))) ? 1U : 0U)))) << (int) (0x7U))))) | ((byte) ((byte) (((byte) ((byte) (0x0U))) << (int) (0x6U))))) | ((byte) ((byte) (((byte) ((byte) ((uint) ((((byte) ((((byte) (lhs)) & ((byte) (0xFU))))) < ((byte) ((((byte) (0x1U)) & ((byte) (0xFU)))))) ? 1U : 0U)))) << (int) (0x5U))))));
		if(((uint) (((rd) == (0x6U)) ? 1U : 0U)) != 0) {
			WriteMemory((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])), result);
		} else {
			State.Registers[(int) rd] = (byte) (result);
		}
		AddCycles((byte) (((uint) (((rd) == (0x6U)) ? 1U : 0U) != 0) ? (0x3U) : (0x1U)));
		return true;
	}
	insn_43:
	/* ADD */
	if((insnBytes[0] & 0xF8) == 0x80) {
		var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
		pc += 1;
		var lhs = (byte) ((0x7U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
		var rhs = (byte) (rs switch { (byte) (0x0U) => (byte) ((0x0U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }), (byte) (0x1U) => (byte) ((0x1U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }), (byte) (0x2U) => (byte) ((0x2U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }), (byte) (0x3U) => (byte) ((0x3U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }), (byte) (0x4U) => (byte) ((0x4U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }), (byte) (0x5U) => (byte) ((0x5U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }), (byte) (0x6U) => (byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])))), _ => (byte) ((0x7U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }) });
		var result = (ushort) (((ushort) (ushort) ((ushort) ((ushort) (lhs)))) + ((ushort) (ushort) ((ushort) ((ushort) (rhs)))));
		State.Flags = (byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) (0xFU)))))) | ((byte) ((byte) (((byte) ((byte) ((uint) ((((byte) ((byte) (result))) == ((byte) ((byte) (0x0U)))) ? 1U : 0U)))) << (int) (0x7U))))) | ((byte) ((byte) (((byte) ((byte) (0x0U))) << (int) (0x6U))))) | ((byte) ((byte) (((byte) ((byte) ((uint) ((((byte) (((byte) (byte) ((byte) ((((byte) (lhs)) & ((byte) (0xFU)))))) + ((byte) (byte) ((byte) ((((byte) (rhs)) & ((byte) (0xFU)))))))) > (0xFU)) ? 1U : 0U)))) << (int) (0x5U))))) | ((byte) ((byte) (((byte) ((byte) ((uint) (((result) >= (0x100U)) ? 1U : 0U)))) << (int) (0x4U))))));
		State.Registers[(int) 0x7U] = (byte) ((byte) ((byte) (result)));
		AddCycles((byte) (((uint) (((rs) == (0x6U)) ? 1U : 0U) != 0) ? (0x2U) : (0x1U)));
		return true;
	}
	insn_44:
	/* XOR */
	if((insnBytes[0] & 0xF8) == 0xA8) {
		var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
		pc += 1;
		var lhs = (byte) ((0x7U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] });
		var rhs = (byte) (rs switch { (byte) (0x0U) => (byte) ((0x0U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }), (byte) (0x1U) => (byte) ((0x1U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }), (byte) (0x2U) => (byte) ((0x2U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }), (byte) (0x3U) => (byte) ((0x3U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }), (byte) (0x4U) => (byte) ((0x4U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }), (byte) (0x5U) => (byte) ((0x5U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }), (byte) (0x6U) => (byte) (ReadMemory<byte>((ushort) (((((ushort) State.Registers[0b100]) << 8) | (ushort) State.Registers[0b101])))), _ => (byte) ((0x7U) switch { 0b110 => throw new NotSupportedException(), {} i => State.Registers[i] }) });
		var result = (byte) ((((byte) (lhs)) ^ ((byte) (rhs))));
		State.Flags = (byte) (((((((byte) ((byte) ((((byte) ((byte) (State.Flags))) & ((byte) (0xFU)))))) | ((byte) ((byte) (((byte) ((byte) ((uint) ((((byte) ((byte) (result))) == ((byte) ((byte) (0x0U)))) ? 1U : 0U)))) << (int) (0x7U))))) | ((byte) ((byte) (((byte) ((byte) (0x0U))) << (int) (0x6U))))) | ((byte) ((byte) (((byte) ((byte) (0x0U))) << (int) (0x5U))))) | ((byte) ((byte) (((byte) ((byte) (0x0U))) << (int) (0x4U))))));
		State.Registers[(int) 0x7U] = (byte) ((byte) ((byte) (result)));
		AddCycles((byte) (((uint) (((rs) == (0x6U)) ? 1U : 0U) != 0) ? (0x2U) : (0x1U)));
		return true;
	}
	insn_45:

        return false;
    }
}
