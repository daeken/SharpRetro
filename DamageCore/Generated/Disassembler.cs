// ReSharper disable CheckNamespace
#pragma warning disable CS0164
namespace DamageCore;

public partial class Disassembler {
    public static string Disassemble(Span<byte> insnBytes, ushort pc) {
		/* LD-rd-rs */
		if((insnBytes[0] & 0xC0) == 0x40) {
			var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			if(((uint) (((rs) != (0x6)) ? 1U : 0U)) != 0)
				goto insn_1;
			return (string) ("ld " + (string) (rd switch { 0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H", 0x5 => "L", 0x7 => "A", _ => throw new NotImplementedException() }) + ", " + (string) (rs switch { 0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H", 0x5 => "L", 0x7 => "A", _ => throw new NotImplementedException() }));
		}
		insn_1:
		/* LD-rd-imm8 */
		if((insnBytes[0] & 0xC7) == 0x6) {
			var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			if(((uint) (((rd) != (0x6)) ? 1U : 0U)) != 0)
				goto insn_2;
			return (string) ("ld " + (string) (rd switch { 0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H", 0x5 => "L", 0x7 => "A", _ => throw new NotImplementedException() }) + ", " + (imm).ToString());
		}
		insn_2:
		/* LD-rd-HL */
		if((insnBytes[0] & 0xC7) == 0x46) {
			var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			if(((uint) (((rd) != (0x6)) ? 1U : 0U)) != 0)
				goto insn_3;
			return (string) ("ld " + (string) (rd switch { 0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H", 0x5 => "L", 0x7 => "A", _ => throw new NotImplementedException() }) + ", (HL)");
		}
		insn_3:
		/* LD-HL-rs */
		if((insnBytes[0] & 0xF8) == 0x70) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			if(((uint) (((rs) != (0x6)) ? 1U : 0U)) != 0)
				goto insn_4;
			return (string) ("ld (HL), " + (string) (rs switch { 0x0 => "B", 0x1 => "C", 0x2 => "D", 0x3 => "E", 0x4 => "H", 0x5 => "L", 0x7 => "A", _ => throw new NotImplementedException() }));
		}
		insn_4:
		/* LD-HL-imm8 */
		if((insnBytes[0] & 0xFF) == 0x36) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			return (string) ("ld (HL), " + (imm).ToString());
		}
		insn_5:
		/* LD-A-BC */
		if((insnBytes[0] & 0xFF) == 0xA) {
			return "ld A, (BC)";
		}
		insn_6:
		/* LD-A-DE */
		if((insnBytes[0] & 0xFF) == 0x1A) {
			return "ld A, (DE)";
		}
		insn_7:
		/* LD-BC-A */
		if((insnBytes[0] & 0xFF) == 0x2) {
			return "ld (BC), A";
		}
		insn_8:
		/* LD-DE-A */
		if((insnBytes[0] & 0xFF) == 0x12) {
			return "ld (DE), A";
		}
		insn_9:
		/* LD-A-imm16 */
		if((insnBytes[0] & 0xFF) == 0xFA) {
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8)))) | ((ushort) (lsb))));
			return (string) ("ld A, (" + (addr).ToString() + ")");
		}
		insn_10:
		/* LD-imm16-A */
		if((insnBytes[0] & 0xFF) == 0xEA) {
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8)))) | ((ushort) (lsb))));
			return (string) ("ld (" + (addr).ToString() + "), A");
		}
		insn_11:
		/* LDH-A-C */
		if((insnBytes[0] & 0xFF) == 0xF2) {
			return "ldh A, (C)";
		}
		insn_12:
		/* LDH-C-A */
		if((insnBytes[0] & 0xFF) == 0xE2) {
			return "ldh (C), A";
		}
		insn_13:
		/* LDH-A-imm8 */
		if((insnBytes[0] & 0xFF) == 0xF0) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) ((ushort) (0xFF00)))) | ((ushort) (imm))));
			return (string) ("ldh A, (" + (addr).ToString() + ")");
		}
		insn_14:
		/* LDH-imm8-A */
		if((insnBytes[0] & 0xFF) == 0xE0) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) ((ushort) (0xFF00)))) | ((ushort) (imm))));
			return (string) ("ldh (" + (addr).ToString() + "), A");
		}
		insn_15:
		/* LD-A-HL- */
		if((insnBytes[0] & 0xFF) == 0x3A) {
			return "ld A, (HL-)";
		}
		insn_16:
		/* LD-HL--A */
		if((insnBytes[0] & 0xFF) == 0x32) {
			return "ld (HL-), A";
		}
		insn_17:
		/* LD-A-HL+ */
		if((insnBytes[0] & 0xFF) == 0x2A) {
			return "ld A, (HL+)";
		}
		insn_18:
		/* LD-HL+-A */
		if((insnBytes[0] & 0xFF) == 0x22) {
			return "ld (HL+), A";
		}
		insn_19:

        return null;
    }

    public static string ClassifyInstruction(Span<byte> insnBytes) {
		if((insnBytes[0] & 0xC0) == 0x40) {
			var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			if(((uint) (((rs) != (0x6)) ? 1U : 0U)) != 0)
				goto insn_1;
			return "LD-rd-rs";
		}
		insn_1:
		if((insnBytes[0] & 0xC7) == 0x6) {
			var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			if(((uint) (((rd) != (0x6)) ? 1U : 0U)) != 0)
				goto insn_2;
			return "LD-rd-imm8";
		}
		insn_2:
		if((insnBytes[0] & 0xC7) == 0x46) {
			var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			if(((uint) (((rd) != (0x6)) ? 1U : 0U)) != 0)
				goto insn_3;
			return "LD-rd-HL";
		}
		insn_3:
		if((insnBytes[0] & 0xF8) == 0x70) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			if(((uint) (((rs) != (0x6)) ? 1U : 0U)) != 0)
				goto insn_4;
			return "LD-HL-rs";
		}
		insn_4:
		if((insnBytes[0] & 0xFF) == 0x36) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			return "LD-HL-imm8";
		}
		insn_5:
		if((insnBytes[0] & 0xFF) == 0xA) {
			return "LD-A-BC";
		}
		insn_6:
		if((insnBytes[0] & 0xFF) == 0x1A) {
			return "LD-A-DE";
		}
		insn_7:
		if((insnBytes[0] & 0xFF) == 0x2) {
			return "LD-BC-A";
		}
		insn_8:
		if((insnBytes[0] & 0xFF) == 0x12) {
			return "LD-DE-A";
		}
		insn_9:
		if((insnBytes[0] & 0xFF) == 0xFA) {
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8)))) | ((ushort) (lsb))));
			return "LD-A-imm16";
		}
		insn_10:
		if((insnBytes[0] & 0xFF) == 0xEA) {
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8)))) | ((ushort) (lsb))));
			return "LD-imm16-A";
		}
		insn_11:
		if((insnBytes[0] & 0xFF) == 0xF2) {
			return "LDH-A-C";
		}
		insn_12:
		if((insnBytes[0] & 0xFF) == 0xE2) {
			return "LDH-C-A";
		}
		insn_13:
		if((insnBytes[0] & 0xFF) == 0xF0) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) ((ushort) (0xFF00)))) | ((ushort) (imm))));
			return "LDH-A-imm8";
		}
		insn_14:
		if((insnBytes[0] & 0xFF) == 0xE0) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) ((ushort) (0xFF00)))) | ((ushort) (imm))));
			return "LDH-imm8-A";
		}
		insn_15:
		if((insnBytes[0] & 0xFF) == 0x3A) {
			return "LD-A-HL-";
		}
		insn_16:
		if((insnBytes[0] & 0xFF) == 0x32) {
			return "LD-HL--A";
		}
		insn_17:
		if((insnBytes[0] & 0xFF) == 0x2A) {
			return "LD-A-HL+";
		}
		insn_18:
		if((insnBytes[0] & 0xFF) == 0x22) {
			return "LD-HL+-A";
		}
		insn_19:

        return null;
    }

    public const int InstructionCount = 19 + 0;
}
