// ReSharper disable CheckNamespace
// ReSharper disable ArrangeRedundantParentheses
// ReSharper disable RedundantCast
#pragma warning disable CS0164
namespace DamageCore;

public partial class Disassembler {
    public static string Disassemble(Span<byte> insnBytes, ushort pc) {
		/* LD-rd-rs */
		if((insnBytes[0] & 0xC0) == 0x40) {
			var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			if(((uint) ((((uint) ((uint) (((rd) != (0x6U)) ? 1U : 0U))) & ((uint) ((uint) (((rs) != (0x6U)) ? 1U : 0U)))))) == 0)
				goto insn_1;
			pc += 1;
			return (string) ("ld " + (string) (rd switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }) + ", " + (string) (rs switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }));
		}
		insn_1:
		/* LD-rd-imm8 */
		if((insnBytes[0] & 0xC7) == 0x6) {
			var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			if(((uint) (((rd) != (0x6U)) ? 1U : 0U)) == 0)
				goto insn_2;
			pc += 2;
			return (string) ("ld " + (string) (rd switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }) + ", " + (string) ($"0x{(imm):x02}"));
		}
		insn_2:
		/* LD-rd-HL */
		if((insnBytes[0] & 0xC7) == 0x46) {
			var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			if(((uint) (((rd) != (0x6U)) ? 1U : 0U)) == 0)
				goto insn_3;
			pc += 1;
			return (string) ("ld " + (string) (rd switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }) + ", (HL)");
		}
		insn_3:
		/* LD-HL-rs */
		if((insnBytes[0] & 0xF8) == 0x70) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			if(((uint) (((rs) != (0x6U)) ? 1U : 0U)) == 0)
				goto insn_4;
			pc += 1;
			return (string) ("ld (HL), " + (string) (rs switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }));
		}
		insn_4:
		/* LD-HL-imm8 */
		if((insnBytes[0] & 0xFF) == 0x36) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			pc += 2;
			return (string) ("ld (HL), " + (string) ($"0x{(imm):x02}"));
		}
		insn_5:
		/* LD-A-BC */
		if((insnBytes[0] & 0xFF) == 0xA) {
			pc += 1;
			return "ld A, (BC)";
		}
		insn_6:
		/* LD-A-DE */
		if((insnBytes[0] & 0xFF) == 0x1A) {
			pc += 1;
			return "ld A, (DE)";
		}
		insn_7:
		/* LD-BC-A */
		if((insnBytes[0] & 0xFF) == 0x2) {
			pc += 1;
			return "ld (BC), A";
		}
		insn_8:
		/* LD-DE-A */
		if((insnBytes[0] & 0xFF) == 0x12) {
			pc += 1;
			return "ld (DE), A";
		}
		insn_9:
		/* LD-A-imm16 */
		if((insnBytes[0] & 0xFF) == 0xFA) {
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
			pc += 3;
			return (string) ("ld A, (" + (string) ($"0x{(addr):x04}") + ")");
		}
		insn_10:
		/* LD-imm16-A */
		if((insnBytes[0] & 0xFF) == 0xEA) {
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
			pc += 3;
			return (string) ("ld (" + (string) ($"0x{(addr):x04}") + "), A");
		}
		insn_11:
		/* LDH-A-C */
		if((insnBytes[0] & 0xFF) == 0xF2) {
			pc += 1;
			return "ldh A, (C)";
		}
		insn_12:
		/* LDH-C-A */
		if((insnBytes[0] & 0xFF) == 0xE2) {
			pc += 1;
			return "ldh (C), A";
		}
		insn_13:
		/* LDH-A-imm8 */
		if((insnBytes[0] & 0xFF) == 0xF0) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) ((ushort) (0xFF00U)))) | ((ushort) (imm))));
			pc += 2;
			return (string) ("ldh A, (" + (string) ($"0x{(addr):x04}") + ")");
		}
		insn_14:
		/* LDH-imm8-A */
		if((insnBytes[0] & 0xFF) == 0xE0) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) ((ushort) (0xFF00U)))) | ((ushort) (imm))));
			pc += 2;
			return (string) ("ldh (" + (string) ($"0x{(addr):x04}") + "), A");
		}
		insn_15:
		/* LD-A-HL- */
		if((insnBytes[0] & 0xFF) == 0x3A) {
			pc += 1;
			return "ld A, (HL-)";
		}
		insn_16:
		/* LD-HL--A */
		if((insnBytes[0] & 0xFF) == 0x32) {
			pc += 1;
			return "ld (HL-), A";
		}
		insn_17:
		/* LD-A-HL+ */
		if((insnBytes[0] & 0xFF) == 0x2A) {
			pc += 1;
			return "ld A, (HL+)";
		}
		insn_18:
		/* LD-HL+-A */
		if((insnBytes[0] & 0xFF) == 0x22) {
			pc += 1;
			return "ld (HL+), A";
		}
		insn_19:
		/* LD-rr-imm16 */
		if((insnBytes[0] & 0xCF) == 0x1) {
			var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var imm = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
			pc += 3;
			return (string) ("ld " + (string) (r switch { (byte) (0x0U) => "BC", (byte) (0x1U) => "DE", (byte) (0x2U) => "HL", _ => "SP" }) + ", " + (string) ($"0x{(imm):x04}"));
		}
		insn_20:
		/* LD-imm16-SP */
		if((insnBytes[0] & 0xFF) == 0x8) {
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
			pc += 3;
			return (string) ("ld (" + (string) ($"0x{(addr):x04}") + "), SP");
		}
		insn_21:
		/* LD-SP-HL */
		if((insnBytes[0] & 0xFF) == 0xF9) {
			pc += 1;
			return "ld SP, HL";
		}
		insn_22:
		/* PUSH-rr */
		if((insnBytes[0] & 0xCF) == 0xC5) {
			var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
			pc += 1;
			return (string) ("push " + (string) (((uint) (((r) == (0x3U)) ? 1U : 0U) != 0) ? (string) (("AF")) : (string) (((string) (r switch { (byte) (0x0U) => "BC", (byte) (0x1U) => "DE", (byte) (0x2U) => "HL", _ => "SP" })))));
		}
		insn_23:
		/* POP-rr */
		if((insnBytes[0] & 0xCF) == 0xC1) {
			var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
			pc += 1;
			return (string) ("pop " + (string) (((uint) (((r) == (0x3U)) ? 1U : 0U) != 0) ? (string) (("AF")) : (string) (((string) (r switch { (byte) (0x0U) => "BC", (byte) (0x1U) => "DE", (byte) (0x2U) => "HL", _ => "SP" })))));
		}
		insn_24:
		/* JP-nn */
		if((insnBytes[0] & 0xFF) == 0xC3) {
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
			pc += 3;
			return (string) ("jp " + (string) ($"0x{(addr):x04}"));
		}
		insn_25:
		/* JP-HL */
		if((insnBytes[0] & 0xFF) == 0xE9) {
			pc += 1;
			return "jp HL";
		}
		insn_26:
		/* JP-cc-imm16 */
		if((insnBytes[0] & 0xE7) == 0xC2) {
			var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
			pc += 3;
			return (string) ("jp " + (string) (cc switch { (byte) (0x0U) => "NZ", (byte) (0x1U) => "Z", (byte) (0x2U) => "NC", _ => "C" }) + ", " + (string) ($"0x{(addr):x04}"));
		}
		insn_27:
		/* JR-simm8 */
		if((insnBytes[0] & 0xFF) == 0x18) {
			var e = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var offset = (sbyte) ((sbyte) (e));
			pc += 2;
			return (string) ("jr " + (string) ($"0x{((ushort) (((ushort) (ushort) ((ushort) (pc))) + ((ushort) (sbyte) (offset)))):x04}"));
		}
		insn_28:
		/* JR-cc-simm8 */
		if((insnBytes[0] & 0xE7) == 0x20) {
			var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
			var e = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var offset = (sbyte) ((sbyte) (e));
			pc += 2;
			return (string) ("jr " + (string) (cc switch { (byte) (0x0U) => "NZ", (byte) (0x1U) => "Z", (byte) (0x2U) => "NC", _ => "C" }) + ", " + (string) ($"0x{((ushort) (((ushort) (ushort) ((ushort) (pc))) + ((ushort) (sbyte) (offset)))):x04}"));
		}
		insn_29:
		/* CALL-imm16 */
		if((insnBytes[0] & 0xFF) == 0xCD) {
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
			pc += 3;
			return (string) ("call " + (string) ($"0x{(addr):x04}"));
		}
		insn_30:
		/* CALL-cc-imm16 */
		if((insnBytes[0] & 0xE7) == 0xC4) {
			var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
			pc += 3;
			return (string) ("call " + (string) (cc switch { (byte) (0x0U) => "NZ", (byte) (0x1U) => "Z", (byte) (0x2U) => "NC", _ => "C" }) + ", " + (string) ($"0x{(addr):x04}"));
		}
		insn_31:
		/* RET */
		if((insnBytes[0] & 0xFF) == 0xC9) {
			pc += 1;
			return "ret";
		}
		insn_32:
		/* RET-cc */
		if((insnBytes[0] & 0xE7) == 0xC0) {
			var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
			pc += 1;
			return (string) ("ret " + (string) (cc switch { (byte) (0x0U) => "NZ", (byte) (0x1U) => "Z", (byte) (0x2U) => "NC", _ => "C" }));
		}
		insn_33:
		/* RETI */
		if((insnBytes[0] & 0xFF) == 0xD9) {
			pc += 1;
			return "reti";
		}
		insn_34:
		/* RST */
		if((insnBytes[0] & 0xC7) == 0xC7) {
			var n = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			var addr = (byte) ((n) << (int) (0x3U));
			pc += 1;
			return (string) ("rst " + (string) ($"0x{(addr):x00}"));
		}
		insn_35:
		/* DI */
		if((insnBytes[0] & 0xFF) == 0xF3) {
			pc += 1;
			return "di";
		}
		insn_36:
		/* EI */
		if((insnBytes[0] & 0xFF) == 0xFB) {
			pc += 1;
			return "ei";
		}
		insn_37:
		/* CCF */
		if((insnBytes[0] & 0xFF) == 0x3F) {
			pc += 1;
			return "ccf";
		}
		insn_38:
		/* SCF */
		if((insnBytes[0] & 0xFF) == 0x37) {
			pc += 1;
			return "scf";
		}
		insn_39:
		/* CPL */
		if((insnBytes[0] & 0xFF) == 0x2F) {
			pc += 1;
			return "cpl";
		}
		insn_40:
		/* NOP */
		if((insnBytes[0] & 0xFF) == 0x0) {
			pc += 1;
			return "nop";
		}
		insn_41:
		/* INC */
		if((insnBytes[0] & 0xC7) == 0x4) {
			var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			pc += 1;
			return (string) ("inc " + (string) (rd switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }));
		}
		insn_42:
		/* DEC */
		if((insnBytes[0] & 0xC7) == 0x5) {
			var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			pc += 1;
			return (string) ("dec " + (string) (rd switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }));
		}
		insn_43:
		/* INC-16 */
		if((insnBytes[0] & 0xCF) == 0x3) {
			var rd = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
			pc += 1;
			return (string) ("inc " + (string) (rd switch { (byte) (0x0U) => "BC", (byte) (0x1U) => "DE", (byte) (0x2U) => "HL", _ => "SP" }));
		}
		insn_44:
		/* DEC-16 */
		if((insnBytes[0] & 0xCF) == 0xB) {
			var rd = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
			pc += 1;
			return (string) ("dec " + (string) (rd switch { (byte) (0x0U) => "BC", (byte) (0x1U) => "DE", (byte) (0x2U) => "HL", _ => "SP" }));
		}
		insn_45:
		/* ADD-HL */
		if((insnBytes[0] & 0xCF) == 0x9) {
			var rs = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
			pc += 1;
			return (string) ("add HL, " + (string) (rs switch { (byte) (0x0U) => "BC", (byte) (0x1U) => "DE", (byte) (0x2U) => "HL", _ => "SP" }));
		}
		insn_46:
		/* ADD-SP-r8 */
		if((insnBytes[0] & 0xFF) == 0xE8) {
			var rimm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var imm = (sbyte) ((sbyte) (rimm));
			pc += 2;
			return (string) ("add SP, " + (string) ($"0x{(imm):x02}"));
		}
		insn_47:
		/* LD-HL-SP-r8 */
		if((insnBytes[0] & 0xFF) == 0xF8) {
			var rimm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var imm = (sbyte) ((sbyte) (rimm));
			pc += 2;
			return (string) ("ld HL, SP+" + (string) ($"0x{(imm):x02}"));
		}
		insn_48:
		/* ADD */
		if((insnBytes[0] & 0xF8) == 0x80) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			pc += 1;
			return (string) ("add A," + " " + (string) (rs switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }));
		}
		insn_49:
		/* ADD-imm8 */
		if((insnBytes[0] & 0xFF) == 0xC6) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			pc += 2;
			return (string) ("add A," + " " + (string) ($"0x{(imm):x02}"));
		}
		insn_50:
		/* ADC */
		if((insnBytes[0] & 0xF8) == 0x88) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			pc += 1;
			return (string) ("adc A," + " " + (string) (rs switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }));
		}
		insn_51:
		/* ADC-imm8 */
		if((insnBytes[0] & 0xFF) == 0xCE) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			pc += 2;
			return (string) ("adc A," + " " + (string) ($"0x{(imm):x02}"));
		}
		insn_52:
		/* SUB */
		if((insnBytes[0] & 0xF8) == 0x90) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			pc += 1;
			return (string) ("sub" + " " + (string) (rs switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }));
		}
		insn_53:
		/* SUB-imm8 */
		if((insnBytes[0] & 0xFF) == 0xD6) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			pc += 2;
			return (string) ("sub" + " " + (string) ($"0x{(imm):x02}"));
		}
		insn_54:
		/* SBC */
		if((insnBytes[0] & 0xF8) == 0x98) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			pc += 1;
			return (string) ("sbc A," + " " + (string) (rs switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }));
		}
		insn_55:
		/* SBC-imm8 */
		if((insnBytes[0] & 0xFF) == 0xDE) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			pc += 2;
			return (string) ("sbc A," + " " + (string) ($"0x{(imm):x02}"));
		}
		insn_56:
		/* AND */
		if((insnBytes[0] & 0xF8) == 0xA0) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			pc += 1;
			return (string) ("and" + " " + (string) (rs switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }));
		}
		insn_57:
		/* AND-imm8 */
		if((insnBytes[0] & 0xFF) == 0xE6) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			pc += 2;
			return (string) ("and" + " " + (string) ($"0x{(imm):x02}"));
		}
		insn_58:
		/* XOR */
		if((insnBytes[0] & 0xF8) == 0xA8) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			pc += 1;
			return (string) ("xor" + " " + (string) (rs switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }));
		}
		insn_59:
		/* XOR-imm8 */
		if((insnBytes[0] & 0xFF) == 0xEE) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			pc += 2;
			return (string) ("xor" + " " + (string) ($"0x{(imm):x02}"));
		}
		insn_60:
		/* OR */
		if((insnBytes[0] & 0xF8) == 0xB0) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			pc += 1;
			return (string) ("or" + " " + (string) (rs switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }));
		}
		insn_61:
		/* OR-imm8 */
		if((insnBytes[0] & 0xFF) == 0xF6) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			pc += 2;
			return (string) ("or" + " " + (string) ($"0x{(imm):x02}"));
		}
		insn_62:
		/* CP */
		if((insnBytes[0] & 0xF8) == 0xB8) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			pc += 1;
			return (string) ("cp" + " " + (string) (rs switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }));
		}
		insn_63:
		/* CP-imm8 */
		if((insnBytes[0] & 0xFF) == 0xFE) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			pc += 2;
			return (string) ("cp" + " " + (string) ($"0x{(imm):x02}"));
		}
		insn_64:
		/* RLCA */
		if((insnBytes[0] & 0xFF) == 0x7) {
			pc += 1;
			return "rlca";
		}
		insn_65:
		/* RLA */
		if((insnBytes[0] & 0xFF) == 0x17) {
			pc += 1;
			return "rla";
		}
		insn_66:
		/* RL */
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x10) {
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			pc += 2;
			return (string) ("rl " + (string) (reg switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }));
		}
		insn_67:
		/* RRA */
		if((insnBytes[0] & 0xFF) == 0x1F) {
			pc += 1;
			return "rra";
		}
		insn_68:
		/* RR */
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x18) {
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			pc += 2;
			return (string) ("rr " + (string) (reg switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }));
		}
		insn_69:
		/* SLA */
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x20) {
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			pc += 2;
			return (string) ("sla " + (string) (reg switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }));
		}
		insn_70:
		/* SRA */
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x28) {
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			pc += 2;
			return (string) ("sra " + (string) (reg switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }));
		}
		insn_71:
		/* SWAP */
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x30) {
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			pc += 2;
			return (string) ("swap " + (string) (reg switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }));
		}
		insn_72:
		/* SRL */
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x38) {
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			pc += 2;
			return (string) ("srl " + (string) (reg switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }));
		}
		insn_73:
		/* BIT */
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xC0) == 0x40) {
			var bit = (byte) ((byte) (insnBytes[1] >> 3) & 0x7);
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			pc += 2;
			return (string) ("bit " + (bit).ToString() + ", " + (string) (reg switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }));
		}
		insn_74:
		/* RES */
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xC0) == 0x80) {
			var bit = (byte) ((byte) (insnBytes[1] >> 3) & 0x7);
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			pc += 2;
			return (string) ("res " + (bit).ToString() + ", " + (string) (reg switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }));
		}
		insn_75:
		/* SET */
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xC0) == 0xC0) {
			var bit = (byte) ((byte) (insnBytes[1] >> 3) & 0x7);
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			pc += 2;
			return (string) ("set " + (bit).ToString() + ", " + (string) (reg switch { (byte) (0x0U) => "B", (byte) (0x1U) => "C", (byte) (0x2U) => "D", (byte) (0x3U) => "E", (byte) (0x4U) => "H", (byte) (0x5U) => "L", (byte) (0x7U) => "A", _ => "(HL)" }));
		}
		insn_76:
		/* DAA */
		if((insnBytes[0] & 0xFF) == 0x27) {
			pc += 1;
			return "daa";
		}
		insn_77:

        return null;
    }

    public static string ClassifyInstruction(Span<byte> insnBytes) {
		if((insnBytes[0] & 0xC0) == 0x40) {
			var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			if(((uint) ((((uint) ((uint) (((rd) != (0x6U)) ? 1U : 0U))) & ((uint) ((uint) (((rs) != (0x6U)) ? 1U : 0U)))))) == 0)
				goto insn_1;
			return "LD-rd-rs";
		}
		insn_1:
		if((insnBytes[0] & 0xC7) == 0x6) {
			var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			if(((uint) (((rd) != (0x6U)) ? 1U : 0U)) == 0)
				goto insn_2;
			return "LD-rd-imm8";
		}
		insn_2:
		if((insnBytes[0] & 0xC7) == 0x46) {
			var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			if(((uint) (((rd) != (0x6U)) ? 1U : 0U)) == 0)
				goto insn_3;
			return "LD-rd-HL";
		}
		insn_3:
		if((insnBytes[0] & 0xF8) == 0x70) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			if(((uint) (((rs) != (0x6U)) ? 1U : 0U)) == 0)
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
			var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
			return "LD-A-imm16";
		}
		insn_10:
		if((insnBytes[0] & 0xFF) == 0xEA) {
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
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
			var addr = (ushort) ((((ushort) ((ushort) ((ushort) (0xFF00U)))) | ((ushort) (imm))));
			return "LDH-A-imm8";
		}
		insn_14:
		if((insnBytes[0] & 0xFF) == 0xE0) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) ((ushort) (0xFF00U)))) | ((ushort) (imm))));
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
		if((insnBytes[0] & 0xCF) == 0x1) {
			var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var imm = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
			return "LD-rr-imm16";
		}
		insn_20:
		if((insnBytes[0] & 0xFF) == 0x8) {
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
			return "LD-imm16-SP";
		}
		insn_21:
		if((insnBytes[0] & 0xFF) == 0xF9) {
			return "LD-SP-HL";
		}
		insn_22:
		if((insnBytes[0] & 0xCF) == 0xC5) {
			var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
			return "PUSH-rr";
		}
		insn_23:
		if((insnBytes[0] & 0xCF) == 0xC1) {
			var r = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
			return "POP-rr";
		}
		insn_24:
		if((insnBytes[0] & 0xFF) == 0xC3) {
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
			return "JP-nn";
		}
		insn_25:
		if((insnBytes[0] & 0xFF) == 0xE9) {
			return "JP-HL";
		}
		insn_26:
		if((insnBytes[0] & 0xE7) == 0xC2) {
			var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
			return "JP-cc-imm16";
		}
		insn_27:
		if((insnBytes[0] & 0xFF) == 0x18) {
			var e = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var offset = (sbyte) ((sbyte) (e));
			return "JR-simm8";
		}
		insn_28:
		if((insnBytes[0] & 0xE7) == 0x20) {
			var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
			var e = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var offset = (sbyte) ((sbyte) (e));
			return "JR-cc-simm8";
		}
		insn_29:
		if((insnBytes[0] & 0xFF) == 0xCD) {
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
			return "CALL-imm16";
		}
		insn_30:
		if((insnBytes[0] & 0xE7) == 0xC4) {
			var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
			var lsb = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var msb = (byte) ((byte) (insnBytes[2] >> 0) & 0xFF);
			var addr = (ushort) ((((ushort) ((ushort) (((ushort) ((ushort) (msb))) << (int) (0x8U)))) | ((ushort) (lsb))));
			return "CALL-cc-imm16";
		}
		insn_31:
		if((insnBytes[0] & 0xFF) == 0xC9) {
			return "RET";
		}
		insn_32:
		if((insnBytes[0] & 0xE7) == 0xC0) {
			var cc = (byte) ((byte) (insnBytes[0] >> 3) & 0x3);
			return "RET-cc";
		}
		insn_33:
		if((insnBytes[0] & 0xFF) == 0xD9) {
			return "RETI";
		}
		insn_34:
		if((insnBytes[0] & 0xC7) == 0xC7) {
			var n = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			var addr = (byte) ((n) << (int) (0x3U));
			return "RST";
		}
		insn_35:
		if((insnBytes[0] & 0xFF) == 0xF3) {
			return "DI";
		}
		insn_36:
		if((insnBytes[0] & 0xFF) == 0xFB) {
			return "EI";
		}
		insn_37:
		if((insnBytes[0] & 0xFF) == 0x3F) {
			return "CCF";
		}
		insn_38:
		if((insnBytes[0] & 0xFF) == 0x37) {
			return "SCF";
		}
		insn_39:
		if((insnBytes[0] & 0xFF) == 0x2F) {
			return "CPL";
		}
		insn_40:
		if((insnBytes[0] & 0xFF) == 0x0) {
			return "NOP";
		}
		insn_41:
		if((insnBytes[0] & 0xC7) == 0x4) {
			var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			return "INC";
		}
		insn_42:
		if((insnBytes[0] & 0xC7) == 0x5) {
			var rd = (byte) ((byte) (insnBytes[0] >> 3) & 0x7);
			return "DEC";
		}
		insn_43:
		if((insnBytes[0] & 0xCF) == 0x3) {
			var rd = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
			return "INC-16";
		}
		insn_44:
		if((insnBytes[0] & 0xCF) == 0xB) {
			var rd = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
			return "DEC-16";
		}
		insn_45:
		if((insnBytes[0] & 0xCF) == 0x9) {
			var rs = (byte) ((byte) (insnBytes[0] >> 4) & 0x3);
			return "ADD-HL";
		}
		insn_46:
		if((insnBytes[0] & 0xFF) == 0xE8) {
			var rimm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var imm = (sbyte) ((sbyte) (rimm));
			return "ADD-SP-r8";
		}
		insn_47:
		if((insnBytes[0] & 0xFF) == 0xF8) {
			var rimm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			var imm = (sbyte) ((sbyte) (rimm));
			return "LD-HL-SP-r8";
		}
		insn_48:
		if((insnBytes[0] & 0xF8) == 0x80) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			return "ADD";
		}
		insn_49:
		if((insnBytes[0] & 0xFF) == 0xC6) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			return "ADD-imm8";
		}
		insn_50:
		if((insnBytes[0] & 0xF8) == 0x88) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			return "ADC";
		}
		insn_51:
		if((insnBytes[0] & 0xFF) == 0xCE) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			return "ADC-imm8";
		}
		insn_52:
		if((insnBytes[0] & 0xF8) == 0x90) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			return "SUB";
		}
		insn_53:
		if((insnBytes[0] & 0xFF) == 0xD6) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			return "SUB-imm8";
		}
		insn_54:
		if((insnBytes[0] & 0xF8) == 0x98) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			return "SBC";
		}
		insn_55:
		if((insnBytes[0] & 0xFF) == 0xDE) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			return "SBC-imm8";
		}
		insn_56:
		if((insnBytes[0] & 0xF8) == 0xA0) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			return "AND";
		}
		insn_57:
		if((insnBytes[0] & 0xFF) == 0xE6) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			return "AND-imm8";
		}
		insn_58:
		if((insnBytes[0] & 0xF8) == 0xA8) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			return "XOR";
		}
		insn_59:
		if((insnBytes[0] & 0xFF) == 0xEE) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			return "XOR-imm8";
		}
		insn_60:
		if((insnBytes[0] & 0xF8) == 0xB0) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			return "OR";
		}
		insn_61:
		if((insnBytes[0] & 0xFF) == 0xF6) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			return "OR-imm8";
		}
		insn_62:
		if((insnBytes[0] & 0xF8) == 0xB8) {
			var rs = (byte) ((byte) (insnBytes[0] >> 0) & 0x7);
			return "CP";
		}
		insn_63:
		if((insnBytes[0] & 0xFF) == 0xFE) {
			var imm = (byte) ((byte) (insnBytes[1] >> 0) & 0xFF);
			return "CP-imm8";
		}
		insn_64:
		if((insnBytes[0] & 0xFF) == 0x7) {
			return "RLCA";
		}
		insn_65:
		if((insnBytes[0] & 0xFF) == 0x17) {
			return "RLA";
		}
		insn_66:
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x10) {
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			return "RL";
		}
		insn_67:
		if((insnBytes[0] & 0xFF) == 0x1F) {
			return "RRA";
		}
		insn_68:
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x18) {
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			return "RR";
		}
		insn_69:
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x20) {
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			return "SLA";
		}
		insn_70:
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x28) {
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			return "SRA";
		}
		insn_71:
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x30) {
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			return "SWAP";
		}
		insn_72:
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xF8) == 0x38) {
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			return "SRL";
		}
		insn_73:
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xC0) == 0x40) {
			var bit = (byte) ((byte) (insnBytes[1] >> 3) & 0x7);
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			return "BIT";
		}
		insn_74:
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xC0) == 0x80) {
			var bit = (byte) ((byte) (insnBytes[1] >> 3) & 0x7);
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			return "RES";
		}
		insn_75:
		if((insnBytes[0] & 0xFF) == 0xCB && (insnBytes[1] & 0xC0) == 0xC0) {
			var bit = (byte) ((byte) (insnBytes[1] >> 3) & 0x7);
			var reg = (byte) ((byte) (insnBytes[1] >> 0) & 0x7);
			return "SET";
		}
		insn_76:
		if((insnBytes[0] & 0xFF) == 0x27) {
			return "DAA";
		}
		insn_77:

        return null;
    }

    public const int InstructionCount = 77 + 0;
}
