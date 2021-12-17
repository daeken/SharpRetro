// ReSharper disable CheckNamespace
#pragma warning disable CS0164
namespace DamageCore;

public partial class Interpreter {
    public bool Interpret(Span<byte> insnBytes, ushort pc) {
	/* LD-rd-rs */
	if((insnBytes[0] & 0xC0) == 0x40) {
		var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
		var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
		if(((uint) (((rs) != (0x6)) ? 1U : 0U)) != 0)
			goto insn_1;
		Registers[(int) rd] = (byte) ((byte) ((rs) switch { 0b110 => throw new NotSupportedException(), {} i => Registers[i] }));
		return true;
	}
	insn_1:
	/* LD-rd-imm8 */
	if((insnBytes[0] & 0xC7) == 0x6) {
		var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
		var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		if(((uint) (((rd) != (0x6)) ? 1U : 0U)) != 0)
			goto insn_2;
		Registers[(int) rd] = (byte) (imm);
		return true;
	}
	insn_2:
	/* LD-rd-HL */
	if((insnBytes[0] & 0xC7) == 0x46) {
		var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
		if(((uint) (((rd) != (0x6)) ? 1U : 0U)) != 0)
			goto insn_3;
		Registers[(int) rd] = (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) Registers[0b100]) << 8) | (ushort) Registers[0b101])))));
		return true;
	}
	insn_3:
	/* LD-HL-rs */
	if((insnBytes[0] & 0xF8) == 0x70) {
		var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
		if(((uint) (((rs) != (0x6)) ? 1U : 0U)) != 0)
			goto insn_4;
		WriteMemory((ushort) (((((ushort) Registers[0b100]) << 8) | (ushort) Registers[0b101])), (byte) ((rs) switch { 0b110 => throw new NotSupportedException(), {} i => Registers[i] }));
		return true;
	}
	insn_4:
	/* LD-HL-imm8 */
	if((insnBytes[0] & 0xFF) == 0x36) {
		var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		WriteMemory((ushort) (((((ushort) Registers[0b100]) << 8) | (ushort) Registers[0b101])), imm);
		return true;
	}
	insn_5:
	/* LD-A-BC */
	if((insnBytes[0] & 0xFF) == 0xA) {
		Registers[(int) 0x7] = (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) Registers[0b000]) << 8) | (ushort) Registers[0b001])))));
		return true;
	}
	insn_6:
	/* LD-A-DE */
	if((insnBytes[0] & 0xFF) == 0x1A) {
		Registers[(int) 0x7] = (byte) ((byte) (ReadMemory<byte>((ushort) (((((ushort) Registers[0b010]) << 8) | (ushort) Registers[0b011])))));
		return true;
	}
	insn_7:
	/* LD-BC-A */
	if((insnBytes[0] & 0xFF) == 0x2) {
		WriteMemory((ushort) (((((ushort) Registers[0b000]) << 8) | (ushort) Registers[0b001])), (byte) ((0x7) switch { 0b110 => throw new NotSupportedException(), {} i => Registers[i] }));
		return true;
	}
	insn_8:
	/* LD-DE-A */
	if((insnBytes[0] & 0xFF) == 0x12) {
		WriteMemory((ushort) (((((ushort) Registers[0b010]) << 8) | (ushort) Registers[0b011])), (byte) ((0x7) switch { 0b110 => throw new NotSupportedException(), {} i => Registers[i] }));
		return true;
	}
	insn_9:
	/* LD-A-imm16 */
	if((insnBytes[0] & 0xFF) == 0xFA) {
		var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
		var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8)))) | ((ushort) (lsb))));
		Registers[(int) 0x7] = (byte) ((byte) (ReadMemory<byte>(addr)));
		return true;
	}
	insn_10:
	/* LD-imm16-A */
	if((insnBytes[0] & 0xFF) == 0xEA) {
		var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
		var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8)))) | ((ushort) (lsb))));
		WriteMemory(addr, (byte) ((0x7) switch { 0b110 => throw new NotSupportedException(), {} i => Registers[i] }));
		return true;
	}
	insn_11:
	/* LDH-A-C */
	if((insnBytes[0] & 0xFF) == 0xF2) {
		Registers[(int) 0x7] = (byte) ((byte) (ReadMemory<byte>((ushort) ((((ushort) ((ushort) ((ushort) (0xFF00)))) | ((ushort) ((byte) ((0x1) switch { 0b110 => throw new NotSupportedException(), {} i => Registers[i] }))))))));
		return true;
	}
	insn_12:
	/* LDH-C-A */
	if((insnBytes[0] & 0xFF) == 0xE2) {
		return true;
	}
	insn_13:
	/* LDH-A-imm8 */
	if((insnBytes[0] & 0xFF) == 0xF0) {
		var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		var addr = (ushort) ((((ushort) ((ushort) ((ushort) (0xFF00)))) | ((ushort) (imm))));
		Registers[(int) 0x7] = (byte) ((byte) (ReadMemory<byte>(addr)));
		return true;
	}
	insn_14:
	/* LDH-imm8-A */
	if((insnBytes[0] & 0xFF) == 0xE0) {
		var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
		var addr = (ushort) ((((ushort) ((ushort) ((ushort) (0xFF00)))) | ((ushort) (imm))));
		WriteMemory(addr, (byte) ((0x7) switch { 0b110 => throw new NotSupportedException(), {} i => Registers[i] }));
		return true;
	}
	insn_15:
	/* LD-A-HL- */
	if((insnBytes[0] & 0xFF) == 0x3A) {
		var hl = (ushort) (((((ushort) Registers[0b100]) << 8) | (ushort) Registers[0b101]));
		Registers[(int) 0x7] = (byte) ((byte) (ReadMemory<byte>(hl)));
		var temp_5 = (ushort) (ulong) (((ulong) (ushort) (hl)) - ((ulong) (long) (0x1)));
		Registers[0b100] = (byte) (temp_5 >> 8);
		Registers[0b101] = (byte) (temp_5 & 0xFF);
		return true;
	}
	insn_16:
	/* LD-HL--A */
	if((insnBytes[0] & 0xFF) == 0x32) {
		var hl = (ushort) (((((ushort) Registers[0b100]) << 8) | (ushort) Registers[0b101]));
		WriteMemory(hl, (byte) ((0x7) switch { 0b110 => throw new NotSupportedException(), {} i => Registers[i] }));
		var temp_6 = (ushort) (ulong) (((ulong) (ushort) (hl)) - ((ulong) (long) (0x1)));
		Registers[0b100] = (byte) (temp_6 >> 8);
		Registers[0b101] = (byte) (temp_6 & 0xFF);
		return true;
	}
	insn_17:
	/* LD-A-HL+ */
	if((insnBytes[0] & 0xFF) == 0x2A) {
		var hl = (ushort) (((((ushort) Registers[0b100]) << 8) | (ushort) Registers[0b101]));
		Registers[(int) 0x7] = (byte) ((byte) (ReadMemory<byte>(hl)));
		var temp_7 = (ushort) (ulong) (((ulong) (ushort) (hl)) + ((ulong) (long) (0x1)));
		Registers[0b100] = (byte) (temp_7 >> 8);
		Registers[0b101] = (byte) (temp_7 & 0xFF);
		return true;
	}
	insn_18:
	/* LD-HL+-A */
	if((insnBytes[0] & 0xFF) == 0x22) {
		var hl = (ushort) (((((ushort) Registers[0b100]) << 8) | (ushort) Registers[0b101]));
		WriteMemory(hl, (byte) ((0x7) switch { 0b110 => throw new NotSupportedException(), {} i => Registers[i] }));
		var temp_8 = (ushort) (ulong) (((ulong) (ushort) (hl)) + ((ulong) (long) (0x1)));
		Registers[0b100] = (byte) (temp_8 >> 8);
		Registers[0b101] = (byte) (temp_8 & 0xFF);
		return true;
	}
	insn_19:

        return false;
    }
}
