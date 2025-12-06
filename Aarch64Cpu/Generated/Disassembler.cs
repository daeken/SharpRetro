// ReSharper disable CheckNamespace
#pragma warning disable 162, 1633, 164, 219
namespace Aarch64Cpu;
using Math = LibSharpRetro.CpuHelpers.Math;
using static Aarch64Common.Common;

public partial class Disassembler {
    public static string Disassemble(uint insn, ulong pc) {
		/* ADCS */
		if((insn & 0x7FE0FC00) == 0x3A000000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("adcs " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString());
		}
		insn_1:
		/* ADD-extended-register */
		if((insn & 0x7FE00000) == 0x0B200000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var imm = (insn >> 10) & 0x7U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) 0x4))))
				goto insn_2;
			var r1 = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var r2 = (string) (((bool) (((byte) ((byte) (((option) & ((byte) ((byte) ((byte) 0x3))))))) == ((byte) 0x3))) ? (string) ("X") : (string) ("W"));
			var extend = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ((string) (option switch { (byte) ((byte) 0x0) => "UXTB", (byte) ((byte) 0x1) => "UXTH", (byte) ((byte) 0x2) => "LSL", (byte) ((byte) 0x3) => "UXTX", (byte) ((byte) 0x4) => "SXTB", (byte) ((byte) 0x5) => "SXTH", (byte) ((byte) 0x6) => "SXTW", _ => "SXTX" })) : (string) ((string) (option switch { (byte) ((byte) 0x0) => "UXTB", (byte) ((byte) 0x1) => "UXTH", (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x4) => "SXTB", (byte) ((byte) 0x5) => "SXTH", (byte) ((byte) 0x6) => "SXTW", _ => "SXTX" })));
			return (string) ("add " + r1 + (rd).ToString() + ", " + r1 + (rn).ToString() + ", " + r2 + (rm).ToString() + ", " + extend + " #" + (imm).ToString());
		}
		insn_2:
		/* ADD-immediate */
		if((insn & 0x7F800000) == 0x11000000) {
			var size = (insn >> 31) & 0x1U;
			var sh = (insn >> 22) & 0x1U;
			var imm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shift = (byte) (((bool) (((byte) (sh)) == ((byte) 0x0))) ? (byte) ((byte) 0x0) : (byte) ((byte) 0xC));
			var simm = (uint) (((uint) ((uint) (imm))) << (int) (shift));
			return (string) ("add " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", #" + (imm).ToString() + ", LSL #" + (shift).ToString());
		}
		insn_3:
		/* ADD-shifted-register */
		if((insn & 0x7F200000) == 0x0B000000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_4;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return (string) ("add " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", " + shiftstr + " #" + (imm).ToString());
		}
		insn_4:
		/* ADD-vector */
		if((insn & 0xBF20FC00) == 0x0E208400) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var ts = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("add V" + (rd).ToString() + "." + ts + ", V" + (rn).ToString() + "." + ts + ", V" + (rm).ToString() + "." + ts);
		}
		insn_5:
		/* ADDP-vector */
		if((insn & 0xBF20FC00) == 0x0E20BC00) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var ts = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("addp V" + (rd).ToString() + "." + ts + ", V" + (rn).ToString() + "." + ts + ", V" + (rm).ToString() + "." + ts);
		}
		insn_6:
		/* ADDS-extended-register */
		if((insn & 0x7FE00000) == 0x2B200000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var imm = (insn >> 10) & 0x7U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) 0x4))))
				goto insn_7;
			var r1 = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var r2 = (string) (((bool) (((byte) ((byte) (((option) & ((byte) ((byte) ((byte) 0x3))))))) == ((byte) 0x3))) ? (string) ("X") : (string) ("W"));
			var extend = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ((string) (option switch { (byte) ((byte) 0x0) => "UXTB", (byte) ((byte) 0x1) => "UXTH", (byte) ((byte) 0x2) => "LSL", (byte) ((byte) 0x3) => "UXTX", (byte) ((byte) 0x4) => "SXTB", (byte) ((byte) 0x5) => "SXTH", (byte) ((byte) 0x6) => "SXTW", _ => "SXTX" })) : (string) ((string) (option switch { (byte) ((byte) 0x0) => "UXTB", (byte) ((byte) 0x1) => "UXTH", (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x4) => "SXTB", (byte) ((byte) 0x5) => "SXTH", (byte) ((byte) 0x6) => "SXTW", _ => "SXTX" })));
			return (string) ("adds " + r1 + (rd).ToString() + ", " + r1 + (rn).ToString() + ", " + r2 + (rm).ToString() + ", " + extend + " #" + (imm).ToString());
		}
		insn_7:
		/* ADDS-immediate */
		if((insn & 0x7F800000) == 0x31000000) {
			var size = (insn >> 31) & 0x1U;
			var sh = (insn >> 22) & 0x1U;
			var imm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shift = (byte) (((bool) (((byte) (sh)) == ((byte) 0x0))) ? (byte) ((byte) 0x0) : (byte) ((byte) 0xC));
			var simm = (uint) (((uint) ((uint) (imm))) << (int) (shift));
			return (string) ("adds " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", #" + (imm).ToString() + ", LSL #" + (shift).ToString());
		}
		insn_8:
		/* ADDS-shifted-register */
		if((insn & 0x7F200000) == 0x2B000000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_9;
			if(!((bool) (((byte) (shift)) != ((byte) 0x3))))
				goto insn_9;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return (string) ("adds " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", " + shiftstr + " #" + (imm).ToString());
		}
		insn_9:
		/* ADR */
		if((insn & 0x9F000000) == 0x10000000) {
			var immlo = (insn >> 29) & 0x3U;
			var immhi = (insn >> 5) & 0x7FFFFU;
			var rd = (insn >> 0) & 0x1FU;
			var imm = (long) (Math.SignExt<long>((uint) ((uint) (((uint) (((uint) (immlo)) << 0)) | ((uint) (((uint) (immhi)) << 2)))), 21));
			var addr = (ulong) (((ulong) (ulong) ((ulong) (pc))) + ((ulong) (long) (imm)));
			return (string) ("adr X" + (rd).ToString() + ", #" + (addr).ToString());
		}
		insn_10:
		/* ADRP */
		if((insn & 0x9F000000) == 0x90000000) {
			var immlo = (insn >> 29) & 0x3U;
			var immhi = (insn >> 5) & 0x7FFFFU;
			var rd = (insn >> 0) & 0x1FU;
			var imm = (long) (Math.SignExt<long>((ulong) ((ulong) (((ulong) (ulong) (((ulong) (((ulong) ((ushort) ((ushort) ((byte) 0x0)))) << 0)) | ((ulong) (((ulong) (immlo)) << 12)))) | ((ulong) (((ulong) (immhi)) << 14)))), 33));
			var addr = (ulong) (((ulong) (ulong) ((ulong) ((ulong) (((ulong) (((ulong) ((ushort) ((ushort) ((byte) 0x0)))) << 0)) | ((ulong) (((ulong) ((ulong) ((ulong) ((ulong) (((ulong) (pc)) >> (int) ((byte) 0xC)))))) << 12)))))) + ((ulong) (long) (imm)));
			return (string) ("adrp X" + (rd).ToString() + ", #" + (addr).ToString());
		}
		insn_11:
		/* AND-immediate */
		if((insn & 0x7F800000) == 0x12000000) {
			var size = (insn >> 31) & 0x1U;
			var up = (insn >> 22) & 0x1U;
			var immr = (insn >> 16) & 0x3FU;
			var imms = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var imm = (ulong) (MakeWMask(up, imms, immr, (byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x20) : (byte) ((byte) 0x40)), (byte) 0x1));
			return (string) ("and " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", #" + (imm).ToString());
		}
		insn_12:
		/* AND-shifted-register */
		if((insn & 0x7F200000) == 0x0A000000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_13;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return (string) ("and " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", " + shiftstr + " #" + (imm).ToString());
		}
		insn_13:
		/* AND-vector */
		if((insn & 0xBFE0FC00) == 0x0E201C00) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var ts = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"));
			return (string) ("and V" + (rd).ToString() + "." + ts + ", V" + (rn).ToString() + "." + ts + ", V" + (rm).ToString() + "." + ts);
		}
		insn_14:
		/* ANDS-shifted-register */
		if((insn & 0x7F200000) == 0x6A000000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_15;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return (string) ("ands " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", " + shiftstr + " #" + (imm).ToString());
		}
		insn_15:
		/* ANDS-immediate */
		if((insn & 0x7F800000) == 0x72000000) {
			var size = (insn >> 31) & 0x1U;
			var up = (insn >> 22) & 0x1U;
			var immr = (insn >> 16) & 0x3FU;
			var imms = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var imm = (ulong) (MakeWMask(up, imms, immr, (byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x20) : (byte) ((byte) 0x40)), (byte) 0x1));
			return (string) ("ands " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", #" + (imm).ToString());
		}
		insn_16:
		/* ASRV */
		if((insn & 0x7FE0FC00) == 0x1AC02800) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("asrv " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString());
		}
		insn_17:
		/* B */
		if((insn & 0xFC000000) == 0x14000000) {
			var imm = (insn >> 0) & 0x3FFFFFFU;
			var addr = (ulong) (((ulong) (ulong) ((ulong) (pc))) + ((ulong) (long) ((long) (Math.SignExt<long>((uint) (((uint) ((uint) (imm))) << (int) ((byte) 0x2)), 28)))));
			return (string) ("b #" + (addr).ToString());
		}
		insn_18:
		/* B.cond */
		if((insn & 0xFF000010) == 0x54000000) {
			var imm = (insn >> 5) & 0x7FFFFU;
			var cond = (insn >> 0) & 0xFU;
			var addr = (ulong) (((ulong) (ulong) ((ulong) (pc))) + ((ulong) (long) ((long) (Math.SignExt<long>((uint) (((uint) ((uint) (imm))) << (int) ((byte) 0x2)), 21)))));
			var condstr = (string) (cond switch { (byte) ((byte) 0x0) => "EQ", (byte) ((byte) 0x1) => "NE", (byte) ((byte) 0x2) => "CS", (byte) ((byte) 0x3) => "CC", (byte) ((byte) 0x4) => "MI", (byte) ((byte) 0x5) => "PL", (byte) ((byte) 0x6) => "VS", (byte) ((byte) 0x7) => "VC", (byte) ((byte) 0x8) => "HI", (byte) ((byte) 0x9) => "LS", (byte) ((byte) 0xA) => "GE", (byte) ((byte) 0xB) => "LT", (byte) ((byte) 0xC) => "GT", (byte) ((byte) 0xD) => "LE", _ => "AL" });
			return (string) ("b." + condstr + " #" + (addr).ToString());
		}
		insn_19:
		/* BFM */
		if((insn & 0x7F800000) == 0x33000000) {
			var size = (insn >> 31) & 0x1U;
			var N = (insn >> 22) & 0x1U;
			var immr = (insn >> 16) & 0x3FU;
			var imms = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("bfm " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", #" + (immr).ToString() + ", #" + (imms).ToString());
		}
		insn_20:
		/* BIC */
		if((insn & 0x7F200000) == 0x0A200000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_21;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return (string) ("bic " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", " + shiftstr + " #" + (imm).ToString());
		}
		insn_21:
		/* BIC-vector-register */
		if((insn & 0xBFE0FC00) == 0x0E601C00) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) (((bool) (((byte) (Q)) == ((byte) 0x1))) ? (string) ("16B") : (string) ("8B"));
			return (string) ("bic V" + (rd).ToString() + "." + T + ", V" + (rn).ToString() + "." + T + ", V" + (rm).ToString() + "." + T);
		}
		insn_22:
		/* BIC-vector-immediate-16bit */
		if((insn & 0xBFF8DC00) == 0x2F009400) {
			var Q = (insn >> 30) & 0x1U;
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var cmode = (insn >> 13) & 0x1U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) (((bool) (((byte) (Q)) == ((byte) 0x1))) ? (string) ("16B") : (string) ("8B"));
			var amount = (byte) (((bool) ((cmode) != ((byte) 0x0))) ? (byte) ((byte) 0x8) : (byte) ((byte) 0x0));
			var imm = (byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (h)) << 0)) | ((byte) (((byte) (g)) << 1)))) | ((byte) (((byte) (f)) << 2)))) | ((byte) (((byte) (e)) << 3)))) | ((byte) (((byte) (d)) << 4)))) | ((byte) (((byte) (c)) << 5)))) | ((byte) (((byte) (b)) << 6)))) | ((byte) (((byte) (a)) << 7))));
			return (string) ("bic V" + (rd).ToString() + "." + T + ", #" + (imm).ToString() + ", LSL #" + (amount).ToString());
		}
		insn_23:
		/* BIC-vector-immediate-32bit */
		if((insn & 0xBFF89C00) == 0x2F001400) {
			var Q = (insn >> 30) & 0x1U;
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var cmode = (insn >> 13) & 0x3U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) (((bool) (((byte) (Q)) == ((byte) 0x1))) ? (string) ("16B") : (string) ("8B"));
			var amount = (byte) ((cmode) << (int) ((byte) 0x3));
			var imm = (byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (h)) << 0)) | ((byte) (((byte) (g)) << 1)))) | ((byte) (((byte) (f)) << 2)))) | ((byte) (((byte) (e)) << 3)))) | ((byte) (((byte) (d)) << 4)))) | ((byte) (((byte) (c)) << 5)))) | ((byte) (((byte) (b)) << 6)))) | ((byte) (((byte) (a)) << 7))));
			return (string) ("bic V" + (rd).ToString() + "." + T + ", #" + (imm).ToString() + ", LSL #" + (amount).ToString());
		}
		insn_24:
		/* BICS */
		if((insn & 0x7F200000) == 0x6A200000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_25;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return (string) ("bics " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", " + shiftstr + " #" + (imm).ToString());
		}
		insn_25:
		/* BIF */
		if((insn & 0xBFE0FC00) == 0x2EE01C00) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"));
			return (string) ("bif V" + (rd).ToString() + "." + T + ", V" + (rn).ToString() + "." + T + ", V" + (rm).ToString() + "." + T);
		}
		insn_26:
		/* BIT */
		if((insn & 0xBFE0FC00) == 0x2EA01C00) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"));
			return (string) ("bit V" + (rd).ToString() + "." + T + ", V" + (rn).ToString() + "." + T + ", V" + (rm).ToString() + "." + T);
		}
		insn_27:
		/* BL */
		if((insn & 0xFC000000) == 0x94000000) {
			var imm = (insn >> 0) & 0x3FFFFFFU;
			var offset = (long) (Math.SignExt<long>((uint) (((uint) ((uint) (imm))) << (int) ((byte) 0x2)), 28));
			var addr = (ulong) (((ulong) (ulong) ((ulong) (pc))) + ((ulong) (long) (offset)));
			return (string) ("bl #" + (addr).ToString());
		}
		insn_28:
		/* BLR */
		if((insn & 0xFFFFFC1F) == 0xD63F0000) {
			var rn = (insn >> 5) & 0x1FU;
			return (string) ("blr X" + (rn).ToString());
		}
		insn_29:
		/* BR */
		if((insn & 0xFFFFFC1F) == 0xD61F0000) {
			var rn = (insn >> 5) & 0x1FU;
			return (string) ("br X" + (rn).ToString());
		}
		insn_30:
		/* BRK */
		if((insn & 0xFFE0001F) == 0xD4200000) {
			var imm = (insn >> 5) & 0xFFFFU;
			return (string) ("brk #" + (imm).ToString());
		}
		insn_31:
		/* BSL */
		if((insn & 0xBFE0FC00) == 0x2E601C00) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"));
			return (string) ("bsl V" + (rd).ToString() + "." + T + ", V" + (rn).ToString() + "." + T + ", V" + (rm).ToString() + "." + T);
		}
		insn_32:
		/* CASP */
		if((insn & 0xBFE0FC00) == 0x08207C00) {
			var size = (insn >> 30) & 0x1U;
			var rs = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var rs2 = (byte) (((byte) (byte) (rs)) + ((byte) (byte) ((byte) 0x1)));
			var rt2 = (byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1)));
			return (string) ("casp " + r + (rs).ToString() + ", " + r + (rs2).ToString() + ", " + r + (rt).ToString() + ", " + r + (rt2).ToString() + ", [X" + (rn).ToString() + "]");
		}
		insn_33:
		/* CASPA */
		if((insn & 0xBFE0FC00) == 0x08607C00) {
			var size = (insn >> 30) & 0x1U;
			var rs = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var rs2 = (byte) (((byte) (byte) (rs)) + ((byte) (byte) ((byte) 0x1)));
			var rt2 = (byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1)));
			return (string) ("caspa " + r + (rs).ToString() + ", " + r + (rs2).ToString() + ", " + r + (rt).ToString() + ", " + r + (rt2).ToString() + ", [X" + (rn).ToString() + "]");
		}
		insn_34:
		/* CASPAL */
		if((insn & 0xBFE0FC00) == 0x0860FC00) {
			var size = (insn >> 30) & 0x1U;
			var rs = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var rs2 = (byte) (((byte) (byte) (rs)) + ((byte) (byte) ((byte) 0x1)));
			var rt2 = (byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1)));
			return (string) ("caspal " + r + (rs).ToString() + ", " + r + (rs2).ToString() + ", " + r + (rt).ToString() + ", " + r + (rt2).ToString() + ", [X" + (rn).ToString() + "]");
		}
		insn_35:
		/* CASPL */
		if((insn & 0xBFE0FC00) == 0x0820FC00) {
			var size = (insn >> 30) & 0x1U;
			var rs = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var rs2 = (byte) (((byte) (byte) (rs)) + ((byte) (byte) ((byte) 0x1)));
			var rt2 = (byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1)));
			return (string) ("caspl " + r + (rs).ToString() + ", " + r + (rs2).ToString() + ", " + r + (rt).ToString() + ", " + r + (rt2).ToString() + ", [X" + (rn).ToString() + "]");
		}
		insn_36:
		/* CBNZ */
		if((insn & 0x7F000000) == 0x35000000) {
			var size = (insn >> 31) & 0x1U;
			var imm = (insn >> 5) & 0x7FFFFU;
			var rs = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var addr = (ulong) (((ulong) (ulong) ((ulong) (pc))) + ((ulong) (long) ((long) (Math.SignExt<long>((uint) ((uint) ((uint) ((imm) << (int) ((byte) 0x2)))), 21)))));
			return (string) ("cbnz " + r + (rs).ToString() + ", #" + (addr).ToString());
		}
		insn_37:
		/* CBZ */
		if((insn & 0x7F000000) == 0x34000000) {
			var size = (insn >> 31) & 0x1U;
			var imm = (insn >> 5) & 0x7FFFFU;
			var rs = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var addr = (ulong) (((ulong) (ulong) ((ulong) (pc))) + ((ulong) (long) ((long) (Math.SignExt<long>((uint) ((uint) ((uint) ((imm) << (int) ((byte) 0x2)))), 21)))));
			return (string) ("cbz " + r + (rs).ToString() + ", #" + (addr).ToString());
		}
		insn_38:
		/* CCMN-immediate */
		if((insn & 0x7FE00C10) == 0x3A400800) {
			var size = (insn >> 31) & 0x1U;
			var imm = (insn >> 16) & 0x1FU;
			var cond = (insn >> 12) & 0xFU;
			var rn = (insn >> 5) & 0x1FU;
			var nzcv = (insn >> 0) & 0xFU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var condstr = (string) (cond switch { (byte) ((byte) 0x0) => "EQ", (byte) ((byte) 0x1) => "NE", (byte) ((byte) 0x2) => "CS", (byte) ((byte) 0x3) => "CC", (byte) ((byte) 0x4) => "MI", (byte) ((byte) 0x5) => "PL", (byte) ((byte) 0x6) => "VS", (byte) ((byte) 0x7) => "VC", (byte) ((byte) 0x8) => "HI", (byte) ((byte) 0x9) => "LS", (byte) ((byte) 0xA) => "GE", (byte) ((byte) 0xB) => "LT", (byte) ((byte) 0xC) => "GT", (byte) ((byte) 0xD) => "LE", _ => "AL" });
			return (string) ("ccmn " + r + (rn).ToString() + ", #" + (imm).ToString() + ", #" + (nzcv).ToString() + ", " + condstr);
		}
		insn_39:
		/* CCMP-immediate */
		if((insn & 0x7FE00C10) == 0x7A400800) {
			var size = (insn >> 31) & 0x1U;
			var imm = (insn >> 16) & 0x1FU;
			var cond = (insn >> 12) & 0xFU;
			var rn = (insn >> 5) & 0x1FU;
			var nzcv = (insn >> 0) & 0xFU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var condstr = (string) (cond switch { (byte) ((byte) 0x0) => "EQ", (byte) ((byte) 0x1) => "NE", (byte) ((byte) 0x2) => "CS", (byte) ((byte) 0x3) => "CC", (byte) ((byte) 0x4) => "MI", (byte) ((byte) 0x5) => "PL", (byte) ((byte) 0x6) => "VS", (byte) ((byte) 0x7) => "VC", (byte) ((byte) 0x8) => "HI", (byte) ((byte) 0x9) => "LS", (byte) ((byte) 0xA) => "GE", (byte) ((byte) 0xB) => "LT", (byte) ((byte) 0xC) => "GT", (byte) ((byte) 0xD) => "LE", _ => "AL" });
			return (string) ("ccmp " + r + (rn).ToString() + ", #" + (imm).ToString() + ", #" + (nzcv).ToString() + ", " + condstr);
		}
		insn_40:
		/* CCMP-register */
		if((insn & 0x7FE00C10) == 0x7A400000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var cond = (insn >> 12) & 0xFU;
			var rn = (insn >> 5) & 0x1FU;
			var nzcv = (insn >> 0) & 0xFU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var condstr = (string) (cond switch { (byte) ((byte) 0x0) => "EQ", (byte) ((byte) 0x1) => "NE", (byte) ((byte) 0x2) => "CS", (byte) ((byte) 0x3) => "CC", (byte) ((byte) 0x4) => "MI", (byte) ((byte) 0x5) => "PL", (byte) ((byte) 0x6) => "VS", (byte) ((byte) 0x7) => "VC", (byte) ((byte) 0x8) => "HI", (byte) ((byte) 0x9) => "LS", (byte) ((byte) 0xA) => "GE", (byte) ((byte) 0xB) => "LT", (byte) ((byte) 0xC) => "GT", (byte) ((byte) 0xD) => "LE", _ => "AL" });
			return (string) ("ccmp " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", #" + (nzcv).ToString() + ", " + condstr);
		}
		insn_41:
		/* CLREX */
		if((insn & 0xFFFFF0FF) == 0xD503305F) {
			var crm = (insn >> 8) & 0xFU;
			return "clrex";
		}
		insn_42:
		/* CLZ */
		if((insn & 0x7FFFFC00) == 0x5AC01000) {
			var size = (insn >> 31) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("clz " + r + (rd).ToString() + ", " + r + (rn).ToString());
		}
		insn_43:
		/* CMEQ-register-scalar */
		if((insn & 0xFF20FC00) == 0x7E208C00) {
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var V = (string) (size switch { (byte) ((byte) 0x3) => "D", _ => throw new NotImplementedException() });
			return (string) ("cmeq " + V + (rd).ToString() + ", " + V + (rn).ToString() + ", " + V + (rm).ToString());
		}
		insn_44:
		/* CMEQ-register-vector */
		if((insn & 0xBF20FC00) == 0x2E208C00) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("cmeq V" + (rd).ToString() + "." + T + ", V" + (rn).ToString() + "." + T + ", V" + (rm).ToString() + "." + T);
		}
		insn_45:
		/* CMEQ-zero-scalar */
		if((insn & 0xFF3FFC00) == 0x5E209800) {
			var size = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var V = (string) (size switch { (byte) ((byte) 0x3) => "D", _ => throw new NotImplementedException() });
			return (string) ("cmeq " + V + (rd).ToString() + ", " + V + (rn).ToString() + ", #0");
		}
		insn_46:
		/* CMEQ-zero-vector */
		if((insn & 0xBF3FFC00) == 0x0E209800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("cmeq V" + (rd).ToString() + "." + T + ", V" + (rn).ToString() + "." + T + ", #0");
		}
		insn_47:
		/* CMGT-register-scalar */
		if((insn & 0xFF20FC00) == 0x5E203400) {
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var V = (string) (size switch { (byte) ((byte) 0x3) => "D", _ => throw new NotImplementedException() });
			return (string) ("cmgt " + V + (rd).ToString() + ", " + V + (rn).ToString() + ", " + V + (rm).ToString());
		}
		insn_48:
		/* CMGT-register-vector */
		if((insn & 0xBF20FC00) == 0x0E203400) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("cmgt V" + (rd).ToString() + "." + T + ", V" + (rn).ToString() + "." + T + ", V" + (rm).ToString() + "." + T);
		}
		insn_49:
		/* CMGT-zero-scalar */
		if((insn & 0xFF3FFC00) == 0x5E208800) {
			var size = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var V = (string) (size switch { (byte) ((byte) 0x3) => "D", _ => throw new NotImplementedException() });
			return (string) ("cmgt " + V + (rd).ToString() + ", " + V + (rn).ToString() + ", #0");
		}
		insn_50:
		/* CMGT-zero-vector */
		if((insn & 0xBF3FFC00) == 0x0E208800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("cmeq V" + (rd).ToString() + "." + T + ", V" + (rn).ToString() + "." + T + ", #0");
		}
		insn_51:
		/* CMHS-register-scalar */
		if((insn & 0xFF20FC00) == 0x7E203C00) {
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var V = (string) (size switch { (byte) ((byte) 0x3) => "D", _ => throw new NotImplementedException() });
			return (string) ("cmhs " + V + (rd).ToString() + ", " + V + (rn).ToString() + ", " + V + (rm).ToString());
		}
		insn_52:
		/* CMHS-register-vector */
		if((insn & 0xBF20FC00) == 0x2E203C00) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("cmhs V" + (rd).ToString() + "." + T + ", V" + (rn).ToString() + "." + T + ", V" + (rm).ToString() + "." + T);
		}
		insn_53:
		/* CNT */
		if((insn & 0xBF3FFC00) == 0x0E205800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", _ => throw new NotImplementedException() });
			return (string) ("cnt V" + (rd).ToString() + "." + t + ", V" + (rn).ToString() + "." + t);
		}
		insn_54:
		/* CSEL */
		if((insn & 0x7FE00C00) == 0x1A800000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var cond = (insn >> 12) & 0xFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var condstr = (string) (cond switch { (byte) ((byte) 0x0) => "EQ", (byte) ((byte) 0x1) => "NE", (byte) ((byte) 0x2) => "CS", (byte) ((byte) 0x3) => "CC", (byte) ((byte) 0x4) => "MI", (byte) ((byte) 0x5) => "PL", (byte) ((byte) 0x6) => "VS", (byte) ((byte) 0x7) => "VC", (byte) ((byte) 0x8) => "HI", (byte) ((byte) 0x9) => "LS", (byte) ((byte) 0xA) => "GE", (byte) ((byte) 0xB) => "LT", (byte) ((byte) 0xC) => "GT", (byte) ((byte) 0xD) => "LE", _ => "AL" });
			return (string) ("csel " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", " + condstr);
		}
		insn_55:
		/* CSINC */
		if((insn & 0x7FE00C00) == 0x1A800400) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var cond = (insn >> 12) & 0xFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var condstr = (string) (cond switch { (byte) ((byte) 0x0) => "EQ", (byte) ((byte) 0x1) => "NE", (byte) ((byte) 0x2) => "CS", (byte) ((byte) 0x3) => "CC", (byte) ((byte) 0x4) => "MI", (byte) ((byte) 0x5) => "PL", (byte) ((byte) 0x6) => "VS", (byte) ((byte) 0x7) => "VC", (byte) ((byte) 0x8) => "HI", (byte) ((byte) 0x9) => "LS", (byte) ((byte) 0xA) => "GE", (byte) ((byte) 0xB) => "LT", (byte) ((byte) 0xC) => "GT", (byte) ((byte) 0xD) => "LE", _ => "AL" });
			return (string) ("csinc " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", " + condstr);
		}
		insn_56:
		/* CSINV */
		if((insn & 0x7FE00C00) == 0x5A800000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var cond = (insn >> 12) & 0xFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var condstr = (string) (cond switch { (byte) ((byte) 0x0) => "EQ", (byte) ((byte) 0x1) => "NE", (byte) ((byte) 0x2) => "CS", (byte) ((byte) 0x3) => "CC", (byte) ((byte) 0x4) => "MI", (byte) ((byte) 0x5) => "PL", (byte) ((byte) 0x6) => "VS", (byte) ((byte) 0x7) => "VC", (byte) ((byte) 0x8) => "HI", (byte) ((byte) 0x9) => "LS", (byte) ((byte) 0xA) => "GE", (byte) ((byte) 0xB) => "LT", (byte) ((byte) 0xC) => "GT", (byte) ((byte) 0xD) => "LE", _ => "AL" });
			return (string) ("csinv " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", " + condstr);
		}
		insn_57:
		/* CSNEG */
		if((insn & 0x7FE00C00) == 0x5A800400) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var cond = (insn >> 12) & 0xFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var condstr = (string) (cond switch { (byte) ((byte) 0x0) => "EQ", (byte) ((byte) 0x1) => "NE", (byte) ((byte) 0x2) => "CS", (byte) ((byte) 0x3) => "CC", (byte) ((byte) 0x4) => "MI", (byte) ((byte) 0x5) => "PL", (byte) ((byte) 0x6) => "VS", (byte) ((byte) 0x7) => "VC", (byte) ((byte) 0x8) => "HI", (byte) ((byte) 0x9) => "LS", (byte) ((byte) 0xA) => "GE", (byte) ((byte) 0xB) => "LT", (byte) ((byte) 0xC) => "GT", (byte) ((byte) 0xD) => "LE", _ => "AL" });
			return (string) ("csneg " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", " + condstr);
		}
		insn_58:
		/* DMB */
		if((insn & 0xFFFFF0FF) == 0xD50330BF) {
			var m = (insn >> 8) & 0xFU;
			var option = (string) (m switch { (byte) ((byte) 0xF) => "SY", (byte) ((byte) 0xE) => "ST", (byte) ((byte) 0xD) => "LD", (byte) ((byte) 0xB) => "ISH", (byte) ((byte) 0xA) => "ISHST", (byte) ((byte) 0x9) => "ISHLD", (byte) ((byte) 0x7) => "NSH", (byte) ((byte) 0x6) => "NSHST", (byte) ((byte) 0x5) => "NSHLD", (byte) ((byte) 0x3) => "OSH", (byte) ((byte) 0x2) => "OSHST", _ => "OSHLD" });
			return (string) ("DMB " + option);
		}
		insn_59:
		/* DSB */
		if((insn & 0xFFFFF0FF) == 0xD503309F) {
			var crm = (insn >> 8) & 0xFU;
			var option = (string) (crm switch { (byte) ((byte) 0xF) => "SY", (byte) ((byte) 0xE) => "ST", (byte) ((byte) 0xD) => "LD", (byte) ((byte) 0xB) => "ISH", (byte) ((byte) 0xA) => "ISHST", (byte) ((byte) 0x9) => "ISHLD", (byte) ((byte) 0x7) => "NSH", (byte) ((byte) 0x6) => "NSHST", (byte) ((byte) 0x5) => "NSHLD", (byte) ((byte) 0x3) => "OSH", (byte) ((byte) 0x2) => "OSHST", _ => "OSHLD" });
			return (string) ("DSB " + option);
		}
		insn_60:
		/* DUP-element-scalar */
		if((insn & 0xFFE0FC00) == 0x5E000400) {
			var imm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = "";
			var index = (byte) 0x0;
			var size = (byte) 0x0;
			if((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0xF))))))) == ((byte) 0x0))) {
				throw new NotImplementedException();
			} else {
				if((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x1))))))) == ((byte) 0x1))) {
					T = "B";
					index = (byte) ((imm) >> (int) ((byte) 0x1));
					size = (byte) 0x1;
				} else {
					if((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x3))))))) == ((byte) 0x2))) {
						T = "H";
						index = (byte) ((imm) >> (int) ((byte) 0x2));
						size = (byte) 0x2;
					} else {
						if((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x7))))))) == ((byte) 0x4))) {
							T = "S";
							index = (byte) ((imm) >> (int) ((byte) 0x3));
							size = (byte) 0x4;
						} else {
							T = "D";
							index = (byte) ((imm) >> (int) ((byte) 0x4));
							size = (byte) 0x8;
						}
					}
				}
			}
			return (string) ("dup " + T + (rd).ToString() + ", V" + (rn).ToString() + "." + T + "[" + (index).ToString() + "]");
		}
		insn_61:
		/* DUP-element-vector */
		if((insn & 0xBFE0FC00) == 0x0E000400) {
			var Q = (insn >> 30) & 0x1U;
			var imm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var Ts = "";
			var T = "";
			var index = (byte) 0x0;
			var size = (byte) 0x0;
			if((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0xF))))))) == ((byte) 0x0))) {
				throw new NotImplementedException();
			} else {
				if((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x1))))))) == ((byte) 0x1))) {
					Ts = "B";
					T = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"));
					index = (byte) ((imm) >> (int) ((byte) 0x1));
					size = (byte) 0x1;
				} else {
					if((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x3))))))) == ((byte) 0x2))) {
						Ts = "H";
						T = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("8H") : (string) ("4H"));
						index = (byte) ((imm) >> (int) ((byte) 0x2));
						size = (byte) 0x2;
					} else {
						if((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x7))))))) == ((byte) 0x4))) {
							Ts = "S";
							T = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("4S") : (string) ("2S"));
							index = (byte) ((imm) >> (int) ((byte) 0x3));
							size = (byte) 0x4;
						} else {
							Ts = "D";
							T = (string) (((bool) ((Q) != ((byte) 0x0))) ? ("2D") : throw new NotImplementedException());
							index = (byte) ((imm) >> (int) ((byte) 0x4));
							size = (byte) 0x8;
						}
					}
				}
			}
			return (string) ("dup V" + (rd).ToString() + "." + T + ", V" + (rn).ToString() + "." + Ts + "[" + (index).ToString() + "]");
		}
		insn_62:
		/* DUP-general */
		if((insn & 0xBFE0FC00) == 0x0E000C00) {
			var Q = (insn >> 30) & 0x1U;
			var imm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var size = ((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0xF))))))) == ((byte) 0x0))) ? throw new NotImplementedException() : ((byte) (((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0xF))))))) == ((byte) 0x8))) ? (byte) ((byte) 0x40) : (byte) ((byte) 0x20)));
			var r = (string) (((bool) ((size) == ((byte) 0x40))) ? (string) ("X") : (string) ("W"));
			var T = ((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0xF))))))) == ((byte) 0x0))) ? throw new NotImplementedException() : ((string) (((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x1))))))) == ((byte) 0x1))) ? (string) ((string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"))) : (string) ((string) (((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x3))))))) == ((byte) 0x2))) ? (string) ((string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("8H") : (string) ("4H"))) : (string) ((string) (((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x7))))))) == ((byte) 0x4))) ? (string) ((string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("4S") : (string) ("2S"))) : (string) ((string) (((bool) ((Q) != ((byte) 0x0))) ? ("2D") : throw new NotImplementedException()))))))));
			return (string) ("dup V" + (rd).ToString() + "." + (T).ToString() + ", " + r + (rn).ToString());
		}
		insn_63:
		/* EON-shifted-register */
		if((insn & 0x7F200000) == 0x4A200000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_64;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return (string) ("eon " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", " + shiftstr + " #" + (imm).ToString());
		}
		insn_64:
		/* EOR-immediate */
		if((insn & 0x7F800000) == 0x52000000) {
			var size = (insn >> 31) & 0x1U;
			var up = (insn >> 22) & 0x1U;
			var immr = (insn >> 16) & 0x3FU;
			var imms = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var imm = (ulong) (MakeWMask(up, imms, immr, (byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x20) : (byte) ((byte) 0x40)), (byte) 0x1));
			return (string) ("and " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", #" + (imm).ToString());
		}
		insn_65:
		/* EOR-shifted-register */
		if((insn & 0x7F200000) == 0x4A000000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_66;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return (string) ("eor " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", " + shiftstr + " #" + (imm).ToString());
		}
		insn_66:
		/* EOR-vector */
		if((insn & 0xBFE0FC00) == 0x2E201C00) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"));
			return (string) ("eor V" + (rd).ToString() + "." + T + ", V" + (rn).ToString() + "." + T + ", V" + (rm).ToString() + "." + T);
		}
		insn_67:
		/* EXT */
		if((insn & 0xBFE08400) == 0x2E000000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var index = (insn >> 11) & 0xFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var ts = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"));
			return (string) ("ext V" + (rd).ToString() + "." + ts + ", V" + (rn).ToString() + "." + ts + ", V" + (rm).ToString() + "." + ts + ", #" + (index).ToString());
		}
		insn_68:
		/* EXTR */
		if((insn & 0x7FA00000) == 0x13800000) {
			var size = (insn >> 31) & 0x1U;
			var o = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var lsb = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (lsb)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_69;
			if(!((bool) ((size) == (o))))
				goto insn_69;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("extr " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", #" + (lsb).ToString());
		}
		insn_69:
		/* FABD-scalar */
		if((insn & 0xFFA0FC00) == 0x7EA0D400) {
			var size = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) ((size) != ((byte) 0x0))) ? (string) ("D") : (string) ("S"));
			return (string) ("fabd " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString());
		}
		insn_70:
		/* FABS-scalar */
		if((insn & 0xFF3FFC00) == 0x1E20C000) {
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return (string) ("fabs " + r + (rd).ToString() + ", " + r + (rn).ToString());
		}
		insn_71:
		/* FABS-vector */
		if((insn & 0xBFBFFC00) == 0x0EA0F800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return (string) ("fabs V" + (rd).ToString() + "." + t + ", V" + (rn).ToString() + "." + t);
		}
		insn_72:
		/* FADD-scalar */
		if((insn & 0xFF20FC00) == 0x1E202800) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return (string) ("fadd " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString());
		}
		insn_73:
		/* FADD-vector */
		if((insn & 0xBFA0FC00) == 0x0E20D400) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var ts = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return (string) ("fadd V" + (rd).ToString() + "." + ts + ", V" + (rn).ToString() + "." + ts + ", V" + (rm).ToString() + "." + ts);
		}
		insn_74:
		/* FADDP-scalar */
		if((insn & 0xFFBFFC00) == 0x7E30D800) {
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("S") : (string) ("D"));
			return (string) ("faddp " + r + (rd).ToString() + ", V" + (rn).ToString() + ".2" + r);
		}
		insn_75:
		/* FADDP-vector */
		if((insn & 0xBFA0FC00) == 0x2E20D400) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return (string) ("faddp V" + (rd).ToString() + "." + t + ", V" + (rn).ToString() + "." + t + ", V" + (rm).ToString() + "." + t);
		}
		insn_76:
		/* FCCMP */
		if((insn & 0xFF200C10) == 0x1E200400) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var cond = (insn >> 12) & 0xFU;
			var rn = (insn >> 5) & 0x1FU;
			var nzcv = (insn >> 0) & 0xFU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			var condstr = (string) (cond switch { (byte) ((byte) 0x0) => "EQ", (byte) ((byte) 0x1) => "NE", (byte) ((byte) 0x2) => "CS", (byte) ((byte) 0x3) => "CC", (byte) ((byte) 0x4) => "MI", (byte) ((byte) 0x5) => "PL", (byte) ((byte) 0x6) => "VS", (byte) ((byte) 0x7) => "VC", (byte) ((byte) 0x8) => "HI", (byte) ((byte) 0x9) => "LS", (byte) ((byte) 0xA) => "GE", (byte) ((byte) 0xB) => "LT", (byte) ((byte) 0xC) => "GT", (byte) ((byte) 0xD) => "LE", _ => "AL" });
			return (string) ("fccmp " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", #" + (nzcv).ToString() + ", " + condstr);
		}
		insn_77:
		/* FCMxx-register-vector */
		if((insn & 0x9F20F400) == 0x0E20E400) {
			var Q = (insn >> 30) & 0x1U;
			var U = (insn >> 29) & 0x1U;
			var E = (insn >> 23) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var ac = (insn >> 11) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var top = (string) ((byte) ((byte) (((byte) (byte) (((byte) (((byte) (ac)) << 0)) | ((byte) (((byte) (U)) << 1)))) | ((byte) (((byte) (E)) << 2)))) switch { (byte) ((byte) 0x0) => "EQ", (byte) ((byte) 0x2) => "GE", (byte) ((byte) 0x3) => "GE", (byte) ((byte) 0x6) => "GT", (byte) ((byte) 0x7) => "GT", _ => throw new NotImplementedException() });
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return (string) ("FCM" + top + " V" + (rd).ToString() + "." + t + ", V" + (rn).ToString() + "." + t + ", V" + (rm).ToString() + "." + t);
		}
		insn_78:
		/* FCMxx-zero-vector */
		if((insn & 0x9FBFEC00) == 0x0EA0C800) {
			var Q = (insn >> 30) & 0x1U;
			var U = (insn >> 29) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var op = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var top = (string) ((byte) ((byte) (((byte) (((byte) (U)) << 0)) | ((byte) (((byte) (op)) << 1)))) switch { (byte) ((byte) 0x0) => "GT", (byte) ((byte) 0x1) => "GE", (byte) ((byte) 0x2) => "EQ", _ => "LE" });
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return (string) ("FCM" + top + " V" + (rd).ToString() + "." + t + ", V" + (rn).ToString() + "." + t + ", #0.0");
		}
		insn_79:
		/* FCMLT-zero-vector */
		if((insn & 0xBFBFFC00) == 0x0EA0E800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return (string) ("FCMLT V" + (rd).ToString() + "." + t + ", V" + (rn).ToString() + "." + t + ", #0.0");
		}
		insn_80:
		/* FCMP */
		if((insn & 0xFF20FC17) == 0x1E202000) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var opc = (insn >> 3) & 0x1U;
			var r = (string) (type switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			var zero = (string) (((bool) (((byte) (opc)) == ((byte) 0x1))) ? (string) ("/0") : (string) (""));
			return (string) ("fcmp " + r + (rn).ToString() + ", " + r + (rm).ToString() + " " + zero);
		}
		insn_81:
		/* FCMPE */
		if((insn & 0xFF20FC17) == 0x1E202010) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var opc = (insn >> 3) & 0x1U;
			var r = (string) (type switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			var zero = (string) (((bool) (((byte) (opc)) == ((byte) 0x1))) ? (string) ("/0") : (string) (""));
			return (string) ("fcmpe " + r + (rn).ToString() + ", " + r + (rm).ToString() + " " + zero);
		}
		insn_82:
		/* FCSEL */
		if((insn & 0xFF200C00) == 0x1E200C00) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var cond = (insn >> 12) & 0xFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			var condstr = (string) (cond switch { (byte) ((byte) 0x0) => "EQ", (byte) ((byte) 0x1) => "NE", (byte) ((byte) 0x2) => "CS", (byte) ((byte) 0x3) => "CC", (byte) ((byte) 0x4) => "MI", (byte) ((byte) 0x5) => "PL", (byte) ((byte) 0x6) => "VS", (byte) ((byte) 0x7) => "VC", (byte) ((byte) 0x8) => "HI", (byte) ((byte) 0x9) => "LS", (byte) ((byte) 0xA) => "GE", (byte) ((byte) 0xB) => "LT", (byte) ((byte) 0xC) => "GT", (byte) ((byte) 0xD) => "LE", _ => "AL" });
			return (string) ("fcsel " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", " + condstr);
		}
		insn_83:
		/* FCVT */
		if((insn & 0xFF3E7C00) == 0x1E224000) {
			var type = (insn >> 22) & 0x3U;
			var opc = (insn >> 15) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r1 = "";
			var r2 = "";
			var tf = (byte) ((byte) (((byte) (((byte) (opc)) << 0)) | ((byte) (((byte) (type)) << 2))));
			switch(tf) {
				case (byte) ((byte) 0xC): {
					r1 = "S";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0xD): {
					r1 = "D";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x3): {
					r1 = "H";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x1): {
					r1 = "D";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "H";
					r2 = "D";
					break;
				}
				case (byte) ((byte) 0x4): {
					r1 = "S";
					r2 = "D";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return (string) ("fcvt " + r1 + (rd).ToString() + ", " + r2 + (rn).ToString());
		}
		insn_84:
		/* FCVTAS-scalar-integer */
		if((insn & 0x7F3FFC00) == 0x1E240000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var st = (byte) ((byte) (((byte) (((byte) (type)) << 0)) | ((byte) (((byte) (size)) << 2))));
			var r1 = "";
			var r2 = "";
			switch(st) {
				case (byte) ((byte) 0x3): {
					r1 = "W";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "X";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x0): {
					r1 = "W";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x4): {
					r1 = "X";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x1): {
					r1 = "W";
					r2 = "D";
					break;
				}
				case (byte) ((byte) 0x5): {
					r1 = "X";
					r2 = "D";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return (string) ("fcvtas " + r1 + (rd).ToString() + ", " + r2 + (rn).ToString());
		}
		insn_85:
		/* FCVTAU-scalar-integer */
		if((insn & 0x7F3FFC00) == 0x1E250000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var st = (byte) ((byte) (((byte) (((byte) (type)) << 0)) | ((byte) (((byte) (size)) << 2))));
			var r1 = "";
			var r2 = "";
			switch(st) {
				case (byte) ((byte) 0x3): {
					r1 = "W";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "X";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x0): {
					r1 = "W";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x4): {
					r1 = "X";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x1): {
					r1 = "W";
					r2 = "D";
					break;
				}
				case (byte) ((byte) 0x5): {
					r1 = "X";
					r2 = "D";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return (string) ("fcvtau " + r1 + (rd).ToString() + ", " + r2 + (rn).ToString());
		}
		insn_86:
		/* FCVTL[2] */
		if((insn & 0xBFBFFC00) == 0x0E217800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var o2 = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("2") : (string) (""));
			var ta = (string) (((bool) ((size) != ((byte) 0x0))) ? (string) ("2D") : (string) ("4S"));
			var tb = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "4H", (byte) ((byte) 0x1) => "8H", (byte) ((byte) 0x2) => "2S", _ => "4S" });
			return (string) ("fcvtl" + o2 + " V" + (rd).ToString() + "." + ta + ", V" + (rn).ToString() + "." + tb);
		}
		insn_87:
		/* FCVTMS-scalar-integer */
		if((insn & 0x7F3FFC00) == 0x1E300000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var st = (byte) ((byte) (((byte) (((byte) (type)) << 0)) | ((byte) (((byte) (size)) << 2))));
			var r1 = "";
			var r2 = "";
			switch(st) {
				case (byte) ((byte) 0x3): {
					r1 = "W";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "X";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x0): {
					r1 = "W";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x4): {
					r1 = "X";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x1): {
					r1 = "W";
					r2 = "D";
					break;
				}
				case (byte) ((byte) 0x5): {
					r1 = "X";
					r2 = "D";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return (string) ("fcvtms " + r1 + (rd).ToString() + ", " + r2 + (rn).ToString());
		}
		insn_88:
		/* FCVTMU-scalar-integer */
		if((insn & 0x7F3FFC00) == 0x1E310000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var st = (byte) ((byte) (((byte) (((byte) (type)) << 0)) | ((byte) (((byte) (size)) << 2))));
			var r1 = "";
			var r2 = "";
			switch(st) {
				case (byte) ((byte) 0x3): {
					r1 = "W";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "X";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x0): {
					r1 = "W";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x4): {
					r1 = "X";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x1): {
					r1 = "W";
					r2 = "D";
					break;
				}
				case (byte) ((byte) 0x5): {
					r1 = "X";
					r2 = "D";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return (string) ("fcvtmu " + r1 + (rd).ToString() + ", " + r2 + (rn).ToString());
		}
		insn_89:
		/* FCVTN */
		if((insn & 0xFFBFFC00) == 0x0E216800) {
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var Ta = "";
			var Tb = "";
			switch(size) {
				case (byte) ((byte) 0x0): {
					Ta = "4S";
					Tb = "4H";
					break;
				}
				case (byte) ((byte) 0x1): {
					Ta = "2D";
					Tb = "2S";
					break;
				}
			}
			return (string) ("fcvtn V" + (rd).ToString() + "." + Tb + ", V" + (rn).ToString() + "." + Ta);
		}
		insn_90:
		/* FCVTN2 */
		if((insn & 0xFFBFFC00) == 0x4E216800) {
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var Ta = "";
			var Tb = "";
			switch(size) {
				case (byte) ((byte) 0x0): {
					Ta = "4S";
					Tb = "8H";
					break;
				}
				case (byte) ((byte) 0x1): {
					Ta = "2D";
					Tb = "4S";
					break;
				}
			}
			return (string) ("fcvtn2 V" + (rd).ToString() + "." + Tb + ", V" + (rn).ToString() + "." + Ta);
		}
		insn_91:
		/* FCVTPS-scalar-integer */
		if((insn & 0x7F3FFC00) == 0x1E280000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var st = (byte) ((byte) (((byte) (((byte) (type)) << 0)) | ((byte) (((byte) (size)) << 2))));
			var r1 = "";
			var r2 = "";
			switch(st) {
				case (byte) ((byte) 0x3): {
					r1 = "W";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "X";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x0): {
					r1 = "W";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x4): {
					r1 = "X";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x1): {
					r1 = "W";
					r2 = "D";
					break;
				}
				case (byte) ((byte) 0x5): {
					r1 = "X";
					r2 = "D";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return (string) ("fcvtps " + r1 + (rd).ToString() + ", " + r2 + (rn).ToString());
		}
		insn_92:
		/* FCVTPU-scalar-integer */
		if((insn & 0x7F3FFC00) == 0x1E290000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var st = (byte) ((byte) (((byte) (((byte) (type)) << 0)) | ((byte) (((byte) (size)) << 2))));
			var r1 = "";
			var r2 = "";
			switch(st) {
				case (byte) ((byte) 0x3): {
					r1 = "W";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "X";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x0): {
					r1 = "W";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x4): {
					r1 = "X";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x1): {
					r1 = "W";
					r2 = "D";
					break;
				}
				case (byte) ((byte) 0x5): {
					r1 = "X";
					r2 = "D";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return (string) ("fcvtpu " + r1 + (rd).ToString() + ", " + r2 + (rn).ToString());
		}
		insn_93:
		/* FCVTZS-scalar */
		if((insn & 0xFFBFFC00) == 0x5EA1B800) {
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) ((size) != ((byte) 0x0))) ? (string) ("D") : (string) ("S"));
			return (string) ("fcvtzs " + r + (rd).ToString() + ", " + r + (rn).ToString());
		}
		insn_94:
		/* FCVTZS-scalar-fixedpoint */
		if((insn & 0x7F3F0000) == 0x1E180000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var scale = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var fbits = (byte) (((byte) (byte) ((byte) 0x40)) - ((byte) (byte) (scale)));
			if(!((bool) ((fbits) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_95;
			var r1 = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var r2 = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return (string) ("fcvtzs " + r1 + (rd).ToString() + ", " + r2 + (rn).ToString() + ", #" + (fbits).ToString());
		}
		insn_95:
		/* FCVTZS-scalar-integer */
		if((insn & 0x7F3FFC00) == 0x1E380000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var st = (byte) ((byte) (((byte) (((byte) (type)) << 0)) | ((byte) (((byte) (size)) << 2))));
			var r1 = "";
			var r2 = "";
			switch(st) {
				case (byte) ((byte) 0x3): {
					r1 = "W";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "X";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x0): {
					r1 = "W";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x4): {
					r1 = "X";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x1): {
					r1 = "W";
					r2 = "D";
					break;
				}
				case (byte) ((byte) 0x5): {
					r1 = "X";
					r2 = "D";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return (string) ("fcvtzs " + r1 + (rd).ToString() + ", " + r2 + (rn).ToString());
		}
		insn_96:
		/* FCVTZU-scalar-fixedpoint */
		if((insn & 0x7F3F0000) == 0x1E190000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var scale = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var fbits = (byte) (((byte) (byte) ((byte) 0x40)) - ((byte) (byte) (scale)));
			if(!((bool) ((fbits) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_97;
			var r1 = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var r2 = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return (string) ("fcvtzu " + r1 + (rd).ToString() + ", " + r2 + (rn).ToString() + ", #" + (fbits).ToString());
		}
		insn_97:
		/* FCVTZU-scalar-integer */
		if((insn & 0x7F3FFC00) == 0x1E390000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var st = (byte) ((byte) (((byte) (((byte) (type)) << 0)) | ((byte) (((byte) (size)) << 2))));
			var r1 = "";
			var r2 = "";
			switch(st) {
				case (byte) ((byte) 0x3): {
					r1 = "W";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "X";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x0): {
					r1 = "W";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x4): {
					r1 = "X";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x1): {
					r1 = "W";
					r2 = "D";
					break;
				}
				case (byte) ((byte) 0x5): {
					r1 = "X";
					r2 = "D";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return (string) ("fcvtzu " + r1 + (rd).ToString() + ", " + r2 + (rn).ToString());
		}
		insn_98:
		/* FDIV-scalar */
		if((insn & 0xFF20FC00) == 0x1E201800) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return (string) ("fdiv " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString());
		}
		insn_99:
		/* FDIV-vector */
		if((insn & 0xBFA0FC00) == 0x2E20FC00) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var ts = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return (string) ("fdiv V" + (rd).ToString() + "." + ts + ", V" + (rn).ToString() + "." + ts + ", V" + (rm).ToString() + "." + ts);
		}
		insn_100:
		/* FMADD */
		if((insn & 0xFF208000) == 0x1F000000) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var ra = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) (type switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", (byte) ((byte) 0x3) => "H", _ => throw new NotImplementedException() });
			return (string) ("fmadd " + t + (rd).ToString() + ", " + t + (rn).ToString() + ", " + t + (rm).ToString() + ", " + t + (ra).ToString());
		}
		insn_101:
		/* FMAX-scalar */
		if((insn & 0xFF20FC00) == 0x1E204800) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return (string) ("fmax " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString());
		}
		insn_102:
		/* FMAXNM-scalar */
		if((insn & 0xFF20FC00) == 0x1E206800) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return (string) ("fmaxnm " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString());
		}
		insn_103:
		/* FMIN-scalar */
		if((insn & 0xFF20FC00) == 0x1E205800) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return (string) ("fmin " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString());
		}
		insn_104:
		/* FMINNM-scalar */
		if((insn & 0xFF20FC00) == 0x1E207800) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return (string) ("fminnm " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString());
		}
		insn_105:
		/* FMLA-by-element-vector-spdp */
		if((insn & 0xBF80F400) == 0x0F801000) {
			var Q = (insn >> 30) & 0x1U;
			var sz = (insn >> 22) & 0x1U;
			var L = (insn >> 21) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var H = (insn >> 11) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (sz)) << 0)) | ((byte) (((byte) (Q)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x2) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			var Ts = (string) (((bool) ((sz) != ((byte) 0x0))) ? (string) ("D") : (string) ("S"));
			var index = (uint) ((byte) ((byte) (((byte) (((byte) (L)) << 0)) | ((byte) (((byte) (sz)) << 1)))) switch { (byte) ((byte) 0x2) => (uint) ((uint) (H)), (byte) ((byte) 0x3) => throw new NotImplementedException(), _ => (uint) ((uint) ((uint) ((byte) ((byte) (((byte) (((byte) (L)) << 0)) | ((byte) (((byte) (H)) << 1))))))) });
			return (string) ("fmla V" + (rd).ToString() + "." + T + ", V" + (rn).ToString() + "." + T + ", V" + (rm).ToString() + "." + Ts + "[" + (index).ToString() + "]");
		}
		insn_106:
		/* FMLA-vector */
		if((insn & 0xBFA0FC00) == 0x0E20CC00) {
			var Q = (insn >> 30) & 0x1U;
			var sz = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (sz)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return (string) ("fmla V" + (rd).ToString() + "." + T + ", V" + (rn).ToString() + "." + T + ", V" + (rm).ToString() + "." + T);
		}
		insn_107:
		/* FMLS-by-element-vector-spdp */
		if((insn & 0xBF80F400) == 0x0F805000) {
			var Q = (insn >> 30) & 0x1U;
			var sz = (insn >> 22) & 0x1U;
			var L = (insn >> 21) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var H = (insn >> 11) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (sz)) << 0)) | ((byte) (((byte) (Q)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x2) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			var Ts = (string) (((bool) ((sz) != ((byte) 0x0))) ? (string) ("D") : (string) ("S"));
			var index = (uint) ((byte) ((byte) (((byte) (((byte) (L)) << 0)) | ((byte) (((byte) (sz)) << 1)))) switch { (byte) ((byte) 0x2) => (uint) ((uint) (H)), (byte) ((byte) 0x3) => throw new NotImplementedException(), _ => (uint) ((uint) ((uint) ((byte) ((byte) (((byte) (((byte) (L)) << 0)) | ((byte) (((byte) (H)) << 1))))))) });
			return (string) ("fmls V" + (rd).ToString() + "." + T + ", V" + (rn).ToString() + "." + T + ", V" + (rm).ToString() + "." + Ts + "[" + (index).ToString() + "]");
		}
		insn_108:
		/* FMLS-vector */
		if((insn & 0xBFA0FC00) == 0x0EA0CC00) {
			var Q = (insn >> 30) & 0x1U;
			var sz = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (sz)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return (string) ("fmls V" + (rd).ToString() + "." + T + ", V" + (rn).ToString() + "." + T + ", V" + (rm).ToString() + "." + T);
		}
		insn_109:
		/* FMOV-general */
		if((insn & 0x7F36FC00) == 0x1E260000) {
			var sf = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var mode = (insn >> 19) & 0x1U;
			var ropc = (insn >> 16) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var opc = (byte) ((byte) (((byte) (((byte) (ropc)) << 0)) | ((byte) (((byte) ((byte) ((byte) ((byte) 0x3)))) << 1))));
			var tf = (byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (opc)) << 0)) | ((byte) (((byte) ((byte) ((byte) (mode)))) << 3)))) | ((byte) (((byte) (type)) << 5)))) | ((byte) (((byte) (sf)) << 7))));
			var r1 = "";
			var r2 = "";
			switch(tf) {
				case (byte) ((byte) 0x66): {
					r1 = "W";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0xE6): {
					r1 = "X";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x67): {
					r1 = "H";
					r2 = "W";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "S";
					r2 = "W";
					break;
				}
				case (byte) ((byte) 0x6): {
					r1 = "W";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0xE7): {
					r1 = "H";
					r2 = "X";
					break;
				}
				case (byte) ((byte) 0xA7): {
					r1 = "D";
					r2 = "X";
					break;
				}
				case (byte) ((byte) 0xCF): {
					r1 = "V";
					r2 = "X";
					break;
				}
				case (byte) ((byte) 0xCE): {
					r1 = "X";
					r2 = "V";
					break;
				}
				case (byte) ((byte) 0xA6): {
					r1 = "X";
					r2 = "D";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			var index1 = (string) (((bool) ((r1) == ("V"))) ? (string) (".D[1]") : (string) (""));
			var index2 = (string) (((bool) ((r2) == ("V"))) ? (string) (".D[1]") : (string) (""));
			return (string) ("fmov " + r1 + (rd).ToString() + index1 + ", " + r2 + (rn).ToString() + index2);
		}
		insn_110:
		/* FMOV-register */
		if((insn & 0xFF3FFC00) == 0x1E204000) {
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", (byte) ((byte) 0x3) => "H", _ => throw new NotImplementedException() });
			return (string) ("fmov " + r + (rd).ToString() + ", " + r + (rn).ToString());
		}
		insn_111:
		/* FMOV-scalar-immediate */
		if((insn & 0xFF201FE0) == 0x1E201000) {
			var type = (insn >> 22) & 0x3U;
			var imm = (insn >> 13) & 0xFFU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			var sv = (float) (Math.Bitcast<uint, float>((uint) ((uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (((uint) ((uint) ((uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 0)) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 1)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 2)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 3)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 4)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 5)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 6)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 7)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 8)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 9)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 10)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 11)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 12)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 13)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 14)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 15)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 16)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 17)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 18)))))) << 0)) | ((uint) (((uint) ((byte) ((byte) ((byte) (((imm) & ((byte) 0xF))))))) << 19)))) | ((uint) (((uint) ((byte) ((byte) ((byte) ((((byte) ((imm) >> (int) ((byte) 0x4))) & ((byte) 0x3))))))) << 23)))) | ((uint) (((uint) ((byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) ((byte) ((byte) ((byte) ((((byte) ((imm) >> (int) ((byte) 0x6))) & ((byte) 0x1))))))) << 0)) | ((byte) (((byte) ((byte) ((byte) ((byte) ((((byte) ((imm) >> (int) ((byte) 0x6))) & ((byte) 0x1))))))) << 1)))) | ((byte) (((byte) ((byte) ((byte) ((byte) ((((byte) ((imm) >> (int) ((byte) 0x6))) & ((byte) 0x1))))))) << 2)))) | ((byte) (((byte) ((byte) ((byte) ((byte) ((((byte) ((imm) >> (int) ((byte) 0x6))) & ((byte) 0x1))))))) << 3)))) | ((byte) (((byte) ((byte) ((byte) ((byte) ((((byte) ((imm) >> (int) ((byte) 0x6))) & ((byte) 0x1))))))) << 4)))))) << 25)))) | ((uint) (((uint) (((bool) (!((bool) (((byte) ((((byte) ((imm) >> (int) ((byte) 0x6))) & ((byte) 0x1)))) != ((byte) 0x0))))) ? 1U : 0U)) << 30)))) | ((uint) (((uint) ((byte) ((byte) ((byte) ((imm) >> (int) ((byte) 0x7)))))) << 31))))));
			return (string) ("fmov " + r + (rd).ToString() + ", #" + (sv).ToString());
		}
		insn_112:
		/* FMOV-vector-immediate-single */
		if((insn & 0xBFF8FC00) == 0x0F00F400) {
			var Q = (insn >> 30) & 0x1U;
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("4S") : (string) ("2S"));
			var sv = (float) (Math.Bitcast<uint, float>((uint) ((((uint) ((uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (((uint) ((uint) ((uint) ((byte) 0x0)))) << 0)) | ((uint) (((uint) (h)) << 19)))) | ((uint) (((uint) (g)) << 20)))) | ((uint) (((uint) (f)) << 21)))) | ((uint) (((uint) (e)) << 22)))) | ((uint) (((uint) (d)) << 23)))) | ((uint) (((uint) (c)) << 24)))) | ((uint) (((uint) (b)) << 25)))) | ((uint) (((uint) (b)) << 26)))) | ((uint) (((uint) (b)) << 27)))) | ((uint) (((uint) (b)) << 28)))) | ((uint) (((uint) (b)) << 29)))) | ((uint) (((uint) (b)) << 30)))) | ((uint) (((uint) (a)) << 31))))) ^ ((uint) (((uint) ((uint) ((byte) 0x1))) << (int) ((byte) 0x1E)))))));
			return (string) ("fmov V" + (rd).ToString() + "." + T + ", #" + (sv).ToString());
		}
		insn_113:
		/* FMOV-vector-immediate-double */
		if((insn & 0xFFF8FC00) == 0x6F00F400) {
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var sv = (double) (Math.Bitcast<ulong, double>((ulong) ((((ulong) ((ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (((ulong) ((ulong) ((ulong) ((byte) 0x0)))) << 0)) | ((ulong) (((ulong) (h)) << 48)))) | ((ulong) (((ulong) (g)) << 49)))) | ((ulong) (((ulong) (f)) << 50)))) | ((ulong) (((ulong) (e)) << 51)))) | ((ulong) (((ulong) (d)) << 52)))) | ((ulong) (((ulong) (c)) << 53)))) | ((ulong) (((ulong) (b)) << 54)))) | ((ulong) (((ulong) (b)) << 55)))) | ((ulong) (((ulong) (b)) << 56)))) | ((ulong) (((ulong) (b)) << 57)))) | ((ulong) (((ulong) (b)) << 58)))) | ((ulong) (((ulong) (b)) << 59)))) | ((ulong) (((ulong) (b)) << 60)))) | ((ulong) (((ulong) (b)) << 61)))) | ((ulong) (((ulong) (b)) << 62)))) | ((ulong) (((ulong) (a)) << 63))))) ^ ((ulong) (((ulong) ((ulong) ((byte) 0x1))) << (int) ((byte) 0x3E)))))));
			return (string) ("fmov V" + (rd).ToString() + ".2D, #" + (sv).ToString());
		}
		insn_114:
		/* FMSUB */
		if((insn & 0xFF208000) == 0x1F008000) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var ra = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) (type switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", (byte) ((byte) 0x3) => "H", _ => throw new NotImplementedException() });
			return (string) ("fmsub " + t + (rd).ToString() + ", " + t + (rn).ToString() + ", " + t + (rm).ToString() + ", " + t + (ra).ToString());
		}
		insn_115:
		/* FMUL-by-element-scalar-spdp */
		if((insn & 0xFF80F400) == 0x5F809000) {
			var sz = (insn >> 22) & 0x1U;
			var L = (insn >> 21) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var H = (insn >> 11) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var Ts = (string) (((bool) ((sz) != ((byte) 0x0))) ? (string) ("D") : (string) ("S"));
			var index = (uint) ((byte) ((byte) (((byte) (((byte) (L)) << 0)) | ((byte) (((byte) (sz)) << 1)))) switch { (byte) ((byte) 0x2) => (uint) ((uint) (H)), (byte) ((byte) 0x3) => throw new NotImplementedException(), _ => (uint) ((uint) ((uint) ((byte) ((byte) (((byte) (((byte) (L)) << 0)) | ((byte) (((byte) (H)) << 1))))))) });
			return (string) ("fmul " + Ts + (rd).ToString() + ", " + Ts + (rn).ToString() + ", V" + (rm).ToString() + "." + Ts + "[" + (index).ToString() + "]");
		}
		insn_116:
		/* FMUL-by-element-vector-spdp */
		if((insn & 0xBF80F400) == 0x0F809000) {
			var Q = (insn >> 30) & 0x1U;
			var sz = (insn >> 22) & 0x1U;
			var L = (insn >> 21) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var H = (insn >> 11) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (sz)) << 0)) | ((byte) (((byte) (Q)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x2) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			var Ts = (string) (((bool) ((sz) != ((byte) 0x0))) ? (string) ("D") : (string) ("S"));
			var index = (uint) ((byte) ((byte) (((byte) (((byte) (L)) << 0)) | ((byte) (((byte) (sz)) << 1)))) switch { (byte) ((byte) 0x2) => (uint) ((uint) (H)), (byte) ((byte) 0x3) => throw new NotImplementedException(), _ => (uint) ((uint) ((uint) ((byte) ((byte) (((byte) (((byte) (L)) << 0)) | ((byte) (((byte) (H)) << 1))))))) });
			return (string) ("fmul V" + (rd).ToString() + "." + T + ", V" + (rn).ToString() + "." + T + ", V" + (rm).ToString() + "." + Ts + "[" + (index).ToString() + "]");
		}
		insn_117:
		/* FMUL-scalar */
		if((insn & 0xFF20FC00) == 0x1E200800) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return (string) ("fmul " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString());
		}
		insn_118:
		/* FMUL-vector */
		if((insn & 0xBFA0FC00) == 0x2E20DC00) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var ts = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return (string) ("fmul V" + (rd).ToString() + "." + ts + ", V" + (rn).ToString() + "." + ts + ", V" + (rm).ToString() + "." + ts);
		}
		insn_119:
		/* FNEG-scalar */
		if((insn & 0xFF3FFC00) == 0x1E214000) {
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return (string) ("fneg " + r + (rd).ToString() + ", " + r + (rn).ToString());
		}
		insn_120:
		/* FNEG-vector */
		if((insn & 0xBFBFFC00) == 0x2EA0F800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return (string) ("fneg V" + (rd).ToString() + "." + T + ", V" + (rn).ToString() + "." + T);
		}
		insn_121:
		/* FNMADD */
		if((insn & 0xFF208000) == 0x1F200000) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var ra = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return (string) ("fnmadd " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", " + r + (ra).ToString());
		}
		insn_122:
		/* FNMSUB */
		if((insn & 0xFF208000) == 0x1F208000) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var ra = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return (string) ("fnmsub " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", " + r + (ra).ToString());
		}
		insn_123:
		/* FNMUL-scalar */
		if((insn & 0xFF20FC00) == 0x1E208800) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return (string) ("fnmul " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString());
		}
		insn_124:
		/* FRINTA-scalar */
		if((insn & 0xFF3FFC00) == 0x1E264000) {
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return (string) ("frinta " + r + (rd).ToString() + ", " + r + (rn).ToString());
		}
		insn_125:
		/* FRINTI-scalar */
		if((insn & 0xFF3FFC00) == 0x1E27C000) {
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return (string) ("frinti " + r + (rd).ToString() + ", " + r + (rn).ToString());
		}
		insn_126:
		/* FRINTM-scalar */
		if((insn & 0xFF3FFC00) == 0x1E254000) {
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return (string) ("frintm " + r + (rd).ToString() + ", " + r + (rn).ToString());
		}
		insn_127:
		/* FRINTP-scalar */
		if((insn & 0xFF3FFC00) == 0x1E24C000) {
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return (string) ("frintp " + r + (rd).ToString() + ", " + r + (rn).ToString());
		}
		insn_128:
		/* FRINTX-scalar */
		if((insn & 0xFF3FFC00) == 0x1E274000) {
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return (string) ("frintx " + r + (rd).ToString() + ", " + r + (rn).ToString());
		}
		insn_129:
		/* FRINTZ-scalar */
		if((insn & 0xFF3FFC00) == 0x1E25C000) {
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return (string) ("frintz " + r + (rd).ToString() + ", " + r + (rn).ToString());
		}
		insn_130:
		/* FRSQRTE-vector */
		if((insn & 0xBFBFFC00) == 0x2EA1D800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return (string) ("frsqrte V" + (rd).ToString() + "." + t + ", V" + (rn).ToString() + "." + t);
		}
		insn_131:
		/* FRSQRTS-vector */
		if((insn & 0xBFA0FC00) == 0x0EA0FC00) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return (string) ("frsqrts V" + (rd).ToString() + "." + t + ", V" + (rn).ToString() + "." + t + ", V" + (rm).ToString() + "." + t);
		}
		insn_132:
		/* FSQRT-scalar */
		if((insn & 0xFF3FFC00) == 0x1E21C000) {
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return (string) ("fsqrt " + r + (rd).ToString() + ", " + r + (rn).ToString());
		}
		insn_133:
		/* FSUB-scalar */
		if((insn & 0xFF20FC00) == 0x1E203800) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return (string) ("fsub " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString());
		}
		insn_134:
		/* FSUB-vector */
		if((insn & 0xBFA0FC00) == 0x0EA0D400) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var ts = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return (string) ("fsub V" + (rd).ToString() + "." + ts + ", V" + (rn).ToString() + "." + ts + ", V" + (rm).ToString() + "." + ts);
		}
		insn_135:
		/* HINT */
		if((insn & 0xFFFFF01F) == 0xD503201F) {
			var crm = (insn >> 8) & 0xFU;
			var op2 = (insn >> 5) & 0x7U;
			var hint = (byte) ((byte) (((byte) (((byte) (op2)) << 0)) | ((byte) (((byte) (crm)) << 3))));
			return (string) ("HINT " + (hint).ToString());
		}
		insn_136:
		/* INS-general */
		if((insn & 0xFFE0FC00) == 0x4E001C00) {
			var imm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0xF))))))) != ((byte) 0x0))))
				goto insn_137;
			var ts = "";
			var index = (uint) ((uint) ((byte) 0x0));
			var r = "W";
			if((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x1))))))) == ((byte) 0x1))) {
				ts = "B";
				index = (byte) ((imm) >> (int) ((byte) 0x1));
			} else {
				if((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x2))))))) == ((byte) 0x2))) {
					ts = "H";
					index = (byte) ((imm) >> (int) ((byte) 0x2));
				} else {
					if((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x4))))))) == ((byte) 0x4))) {
						ts = "S";
						index = (byte) ((imm) >> (int) ((byte) 0x3));
					} else {
						ts = "D";
						index = (byte) ((imm) >> (int) ((byte) 0x4));
						r = "X";
					}
				}
			}
			return (string) ("ins V" + (rd).ToString() + "." + ts + "[" + (index).ToString() + "], " + r + (rn).ToString());
		}
		insn_137:
		/* INS-vector */
		if((insn & 0xFFE08400) == 0x6E000400) {
			var imm5 = (insn >> 16) & 0x1FU;
			var imm4 = (insn >> 11) & 0xFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) ((byte) (((imm5) & ((byte) ((byte) ((byte) 0xF))))))) != ((byte) 0x0))))
				goto insn_138;
			var ts = "";
			var index1 = (uint) ((uint) ((byte) 0x0));
			var index2 = (uint) ((uint) ((byte) 0x0));
			if((bool) (((byte) ((byte) (((imm5) & ((byte) ((byte) ((byte) 0x1))))))) == ((byte) 0x1))) {
				ts = "B";
				index1 = (byte) ((imm5) >> (int) ((byte) 0x1));
				index2 = imm4;
			} else {
				if((bool) (((byte) ((byte) (((imm5) & ((byte) ((byte) ((byte) 0x2))))))) == ((byte) 0x2))) {
					ts = "H";
					index1 = (byte) ((imm5) >> (int) ((byte) 0x2));
					index2 = (byte) ((imm4) >> (int) ((byte) 0x1));
				} else {
					if((bool) (((byte) ((byte) (((imm5) & ((byte) ((byte) ((byte) 0x4))))))) == ((byte) 0x4))) {
						ts = "S";
						index1 = (byte) ((imm5) >> (int) ((byte) 0x3));
						index2 = (byte) ((imm4) >> (int) ((byte) 0x2));
					} else {
						ts = "D";
						index1 = (byte) ((imm5) >> (int) ((byte) 0x4));
						index2 = (byte) ((imm4) >> (int) ((byte) 0x3));
					}
				}
			}
			return (string) ("ins V" + (rd).ToString() + "." + ts + "[" + (index1).ToString() + "], V" + (rn).ToString() + "." + ts + "[" + (index2).ToString() + "]");
		}
		insn_138:
		/* LD1-multi-no-offset-one-register */
		if((insn & 0xBFFFF000) == 0x0C407000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x6) => "1D", _ => "2D" });
			return (string) ("ld1 { V" + (rt).ToString() + "." + T + " }, [X" + (rn).ToString() + "]");
		}
		insn_139:
		/* LD1-multi-no-offset-one-register-postindex-immediate */
		if((insn & 0xBFFFF000) == 0x0CDF7000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x6) => "1D", _ => "2D" });
			var imm = (byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x10) : (byte) ((byte) 0x8));
			return (string) ("ld1 { V" + (rt).ToString() + "." + T + " }, [X" + (rn).ToString() + "], #" + (imm).ToString());
		}
		insn_140:
		/* LD1-multi-no-offset-two-registers */
		if((insn & 0xBFFFF000) == 0x0C40A000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x6) => "1D", _ => "2D" });
			return (string) ("ld1 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + " }, [X" + (rn).ToString() + "]");
		}
		insn_141:
		/* LD1-multi-no-offset-two-registers-postindex-immediate */
		if((insn & 0xBFFFF000) == 0x0CDFA000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var imm = (byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x20) : (byte) ((byte) 0x10));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x6) => "1D", _ => "2D" });
			return (string) ("ld1 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + " }, [X" + (rn).ToString() + "], #" + (imm).ToString());
		}
		insn_142:
		/* LD1-multi-no-offset-three-registers */
		if((insn & 0xBFFFF000) == 0x0C406000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x6) => "1D", _ => "2D" });
			return (string) ("ld1 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + ", V" + (rt3).ToString() + "." + T + " }, [X" + (rn).ToString() + "]");
		}
		insn_143:
		/* LD1-multi-no-offset-three-registers-postindex-immediate */
		if((insn & 0xBFFFF000) == 0x0CDF6000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var imm = (byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x30) : (byte) ((byte) 0x18));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x6) => "1D", _ => "2D" });
			return (string) ("ld1 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + ", V" + (rt3).ToString() + "." + T + " }, [X" + (rn).ToString() + "], #" + (imm).ToString());
		}
		insn_144:
		/* LD1-multi-no-offset-four-registers */
		if((insn & 0xBFFFF000) == 0x0C402000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var rt4 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x3))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x6) => "1D", _ => "2D" });
			return (string) ("ld1 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + ", V" + (rt3).ToString() + "." + T + ", V" + (rt4).ToString() + "." + T + " }, [X" + (rn).ToString() + "]");
		}
		insn_145:
		/* LD1-multi-no-offset-four-registers-postindex-immediate */
		if((insn & 0xBFFFF000) == 0x0C5F2000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var rt4 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x3))))) % ((byte) (byte) ((byte) 0x20)));
			var imm = (byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x40) : (byte) ((byte) 0x20));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x6) => "1D", _ => "2D" });
			return (string) ("ld1 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + ", V" + (rt3).ToString() + "." + T + ", V" + (rt4).ToString() + "." + T + " }, [X" + (rn).ToString() + "], #" + (imm).ToString());
		}
		insn_146:
		/* LD1-single-no-offset */
		if((insn & 0xBFFF2000) == 0x0D400000) {
			var Q = (insn >> 30) & 0x1U;
			var opc = (insn >> 14) & 0x3U;
			var S = (insn >> 12) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (opc)) != ((byte) 0x3))))
				goto insn_147;
			var t = (string) (((bool) (((byte) (opc)) == ((byte) 0x0))) ? (string) ("B") : (string) ((string) (((bool) ((((bool) (((byte) (opc)) == ((byte) 0x1))) & ((bool) (((byte) ((byte) (((size) & ((byte) ((byte) ((byte) 0x1))))))) == ((byte) 0x0)))))) ? (string) ("H") : (string) ((string) (((bool) (((byte) (opc)) == ((byte) 0x2))) ? ((string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("S") : (string) ((string) (((bool) ((((bool) (((byte) (size)) == ((byte) 0x1))) & ((bool) (((byte) (S)) == ((byte) 0x0)))))) ? ("D") : throw new NotImplementedException())))) : throw new NotImplementedException())))));
			var index = (uint) (opc switch { (byte) ((byte) 0x0) => (uint) ((uint) ((byte) ((byte) (((byte) (byte) (((byte) (((byte) (size)) << 0)) | ((byte) (((byte) (S)) << 2)))) | ((byte) (((byte) (Q)) << 3)))))), (byte) ((byte) 0x1) => (uint) ((uint) (((uint) ((uint) ((byte) ((byte) (((byte) (byte) (((byte) (((byte) (size)) << 0)) | ((byte) (((byte) (S)) << 2)))) | ((byte) (((byte) (Q)) << 3))))))) >> (int) ((byte) 0x1))), (byte) ((byte) 0x2) => (uint) ((uint) (((bool) (((byte) ((byte) (((size) & ((byte) ((byte) ((byte) 0x1))))))) == ((byte) 0x0))) ? (uint) ((uint) ((uint) ((byte) ((byte) (((byte) (((byte) (S)) << 0)) | ((byte) (((byte) (Q)) << 1))))))) : (uint) (Q))), _ => throw new NotImplementedException() });
			return (string) ("ld1 { V" + (rt).ToString() + "." + t + " }[" + (index).ToString() + "], [X" + (rn).ToString() + "]");
		}
		insn_147:
		/* LD1R-single-no-offset */
		if((insn & 0xBFFFF000) == 0x0D40C000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x6) => "1D", _ => "2D" });
			return (string) ("ld1r { V" + (rt).ToString() + "." + t + " }, [X" + (rn).ToString() + "]");
		}
		insn_148:
		/* LD1R-single-postindex-immediate */
		if((insn & 0xBFE0F000) == 0x0DC0C000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (rm)) == ((byte) 0x1F))))
				goto insn_149;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x6) => "1D", _ => "2D" });
			var imm = (byte) (size switch { (byte) ((byte) 0x0) => (byte) 0x1, (byte) ((byte) 0x1) => (byte) ((byte) 0x2), (byte) ((byte) 0x2) => (byte) ((byte) 0x4), _ => (byte) ((byte) 0x8) });
			return (string) ("ld1r { V" + (rt).ToString() + "." + t + " }, [X" + (rn).ToString() + "], #" + (imm).ToString());
		}
		insn_149:
		/* LD1R-single-postindex-register */
		if((insn & 0xBFE0F000) == 0x0DC0C000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (rm)) != ((byte) 0x1F))))
				goto insn_150;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x6) => "1D", _ => "2D" });
			return (string) ("ld1r { V" + (rt).ToString() + "." + t + " }, [X" + (rn).ToString() + "], X" + (rm).ToString());
		}
		insn_150:
		/* LD2-multi-no-offset */
		if((insn & 0xBFFFF000) == 0x0C408000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("ld2 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + " }, [X" + (rn).ToString() + "]");
		}
		insn_151:
		/* LD2-multi-postindex-immediate */
		if((insn & 0xBFE0F000) == 0x0CC08000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var imm = (byte) ((byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x20) : (byte) ((byte) 0x10)));
			if(!((bool) (((byte) (rm)) == ((byte) 0x1F))))
				goto insn_152;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("ld2 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + " }, [X" + (rn).ToString() + "], #" + (imm).ToString());
		}
		insn_152:
		/* LD2-multi-postindex-register */
		if((insn & 0xBFE0F000) == 0x0CC08000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) != ((byte) 0x1F))))
				goto insn_153;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("ld2 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + " }, [X" + (rn).ToString() + "], X" + (rm).ToString());
		}
		insn_153:
		/* LD3-multi-no-offset */
		if((insn & 0xBFFFF000) == 0x0C404000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("ld3 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + ", V" + (rt3).ToString() + "." + T + " }, [X" + (rn).ToString() + "]");
		}
		insn_154:
		/* LD3-multi-postindex-immediate */
		if((insn & 0xBFE0F000) == 0x0CC04000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var imm = (byte) ((byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x30) : (byte) ((byte) 0x18)));
			if(!((bool) (((byte) (rm)) == ((byte) 0x1F))))
				goto insn_155;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("ld3 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + ", V" + (rt3).ToString() + "." + T + " }, [X" + (rn).ToString() + "], #" + (imm).ToString());
		}
		insn_155:
		/* LD3-multi-postindex-register */
		if((insn & 0xBFE0F000) == 0x0CC04000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) != ((byte) 0x1F))))
				goto insn_156;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("ld3 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + ", V" + (rt3).ToString() + "." + T + " }, [X" + (rn).ToString() + "], X" + (rm).ToString());
		}
		insn_156:
		/* LD4-multi-no-offset */
		if((insn & 0xBFFFF000) == 0x0C400000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var rt4 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x3))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("ld4 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + ", V" + (rt3).ToString() + "." + T + ", V" + (rt4).ToString() + "." + T + " }, [X" + (rn).ToString() + "]");
		}
		insn_157:
		/* LD4-multi-postindex-immediate */
		if((insn & 0xBFE0F000) == 0x0CC00000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var rt4 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x3))))) % ((byte) (byte) ((byte) 0x20)));
			var imm = (byte) ((byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x40) : (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) == ((byte) 0x1F))))
				goto insn_158;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("ld4 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + ", V" + (rt3).ToString() + "." + T + ", V" + (rt4).ToString() + "." + T + " }, [X" + (rn).ToString() + "], #" + (imm).ToString());
		}
		insn_158:
		/* LD4-multi-postindex-register */
		if((insn & 0xBFE0F000) == 0x0CC00000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var rt4 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x3))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) != ((byte) 0x1F))))
				goto insn_159;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("ld4 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + ", V" + (rt3).ToString() + "." + T + ", V" + (rt4).ToString() + "." + T + " }, [X" + (rn).ToString() + "], X" + (rm).ToString());
		}
		insn_159:
		/* LDAR */
		if((insn & 0xBFFFFC00) == 0x88DFFC00) {
			var size = (insn >> 30) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("ldar " + r + (rt).ToString() + ", [X" + (rn).ToString() + "]");
		}
		insn_160:
		/* LDARB */
		if((insn & 0xFFFFFC00) == 0x08DFFC00) {
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return (string) ("ldarb W" + (rt).ToString() + ", [X" + (rn).ToString() + "]");
		}
		insn_161:
		/* LDARH */
		if((insn & 0xFFFFFC00) == 0x48DFFC00) {
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return (string) ("ldarh W" + (rt).ToString() + ", [X" + (rn).ToString() + "]");
		}
		insn_162:
		/* LDAXB */
		if((insn & 0xBFFFFC00) == 0x885FFC00) {
			var size = (insn >> 30) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return (string) ("ldaxb W" + (rt).ToString() + ", [X" + (rn).ToString() + "]");
		}
		insn_163:
		/* LDAXRB */
		if((insn & 0xFFFFFC00) == 0x085FFC00) {
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return (string) ("ldaxrb W" + (rt).ToString() + ", [X" + (rn).ToString() + "]");
		}
		insn_164:
		/* LDAXRH */
		if((insn & 0xFFFFFC00) == 0x485FFC00) {
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return (string) ("ldaxrh W" + (rt).ToString() + ", [X" + (rn).ToString() + "]");
		}
		insn_165:
		/* LDP-immediate-postindex */
		if((insn & 0x7FC00000) == 0x28C00000) {
			var size = (insn >> 31) & 0x1U;
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt1) != (rt2))))
				goto insn_166;
			if(!((bool) ((rt1) != (rn))))
				goto insn_166;
			if(!((bool) ((rt2) != (rn))))
				goto insn_166;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x3))));
			return (string) ("ldp " + r + (rt1).ToString() + ", " + r + (rt2).ToString() + ", [X" + (rn).ToString() + "], #" + (simm).ToString());
		}
		insn_166:
		/* LDP-immediate-signed-offset */
		if((insn & 0x7FC00000) == 0x29400000) {
			var size = (insn >> 31) & 0x1U;
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt1) != (rt2))))
				goto insn_167;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x3))));
			return (string) ("ldp " + r + (rt1).ToString() + ", " + r + (rt2).ToString() + ", [X" + (rn).ToString() + ", #" + (simm).ToString() + "]");
		}
		insn_167:
		/* LDP-simd-postindex */
		if((insn & 0x3FC00000) == 0x2CC00000) {
			var opc = (insn >> 30) & 0x3U;
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt1) != (rt2))))
				goto insn_168;
			var r = (string) (opc switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => "Q" });
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) (opc switch { (byte) ((byte) 0x0) => (byte) 0x2, (byte) ((byte) 0x1) => (byte) ((byte) 0x3), _ => (byte) ((byte) 0x4) })));
			return (string) ("ldp " + r + (rt1).ToString() + ", " + r + (rt2).ToString() + ", [X" + (rn).ToString() + "], #" + (simm).ToString());
		}
		insn_168:
		/* LDP-simd-preindex */
		if((insn & 0x3FC00000) == 0x2DC00000) {
			var opc = (insn >> 30) & 0x3U;
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt1) != (rt2))))
				goto insn_169;
			var r = (string) (opc switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => "Q" });
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) (opc switch { (byte) ((byte) 0x0) => (byte) 0x2, (byte) ((byte) 0x1) => (byte) ((byte) 0x3), _ => (byte) ((byte) 0x4) })));
			return (string) ("ldp " + r + (rt1).ToString() + ", " + r + (rt2).ToString() + ", [X" + (rn).ToString() + ", #" + (simm).ToString() + "]!");
		}
		insn_169:
		/* LDP-simd-signed-offset */
		if((insn & 0x3FC00000) == 0x2D400000) {
			var opc = (insn >> 30) & 0x3U;
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt1) != (rt2))))
				goto insn_170;
			var r = (string) (opc switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => "Q" });
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) (opc switch { (byte) ((byte) 0x0) => (byte) 0x2, (byte) ((byte) 0x1) => (byte) ((byte) 0x3), _ => (byte) ((byte) 0x4) })));
			return (string) ("ldp " + r + (rt1).ToString() + ", " + r + (rt2).ToString() + ", [X" + (rn).ToString() + ", #" + (simm).ToString() + "]");
		}
		insn_170:
		/* LDPSW-immediate-signed-offset */
		if((insn & 0xFFC00000) == 0x69400000) {
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt1) != (rt2))))
				goto insn_171;
			if(!((bool) ((rt1) != (rn))))
				goto insn_171;
			if(!((bool) ((rt2) != (rn))))
				goto insn_171;
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) 0x2));
			return (string) ("ldpsw X" + (rt1).ToString() + ", X" + (rt2).ToString() + ", [X" + (rn).ToString() + ", #" + (simm).ToString() + "]");
		}
		insn_171:
		/* LDR-immediate-preindex */
		if((insn & 0xBFE00C00) == 0xB8400C00) {
			var size = (insn >> 30) & 0x1U;
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) ((rd) != (rn))))
				goto insn_172;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return (string) ("ldr " + r + (rd).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]!");
		}
		insn_172:
		/* LDR-immediate-postindex */
		if((insn & 0xBFE00C00) == 0xB8400400) {
			var size = (insn >> 30) & 0x1U;
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) ((rd) != (rn))))
				goto insn_173;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return (string) ("ldr " + r + (rd).ToString() + ", [X" + (rn).ToString() + "], #" + (imm).ToString());
		}
		insn_173:
		/* LDR-immediate-unsigned-offset */
		if((insn & 0xBFC00000) == 0xB9400000) {
			var size = (insn >> 30) & 0x1U;
			var rawimm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var imm = (ushort) ((rawimm) << (int) ((byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x3))));
			return (string) ("ldr " + r + (rd).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]");
		}
		insn_174:
		/* LDR-literal */
		if((insn & 0xBF000000) == 0x18000000) {
			var size = (insn >> 30) & 0x1U;
			var rawimm = (insn >> 5) & 0x7FFFFU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var offset = (long) (Math.SignExt<long>((uint) ((uint) ((uint) ((rawimm) << (int) ((byte) 0x2)))), 21));
			var addr = (ulong) (((ulong) (ulong) ((ulong) (pc))) + ((ulong) (long) (offset)));
			return (string) ("ldr " + r + (rt).ToString() + ", #" + (addr).ToString());
		}
		insn_175:
		/* LDR-simd-immediate-postindex */
		if((insn & 0x3F600C00) == 0x3C400400) {
			var size = (insn >> 30) & 0x3U;
			var opc = (insn >> 23) & 0x1U;
			var imm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var simm = (long) (Math.SignExt<long>(imm, 9));
			var r = (string) ((byte) ((byte) (((byte) (((byte) (opc)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "B", (byte) ((byte) 0x2) => "H", (byte) ((byte) 0x4) => "S", (byte) ((byte) 0x6) => "D", (byte) ((byte) 0x1) => "Q", _ => throw new NotImplementedException() });
			return (string) ("ldr " + r + (rt).ToString() + ", [X" + (rn).ToString() + "], #" + (simm).ToString());
		}
		insn_176:
		/* LDR-simd-immediate-preindex */
		if((insn & 0x3F600C00) == 0x3C400C00) {
			var size = (insn >> 30) & 0x3U;
			var opc = (insn >> 23) & 0x1U;
			var imm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var simm = (long) (Math.SignExt<long>(imm, 9));
			var r = (string) ((byte) ((byte) (((byte) (((byte) (opc)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "B", (byte) ((byte) 0x2) => "H", (byte) ((byte) 0x4) => "S", (byte) ((byte) 0x6) => "D", (byte) ((byte) 0x1) => "Q", _ => throw new NotImplementedException() });
			return (string) ("ldr " + r + (rt).ToString() + ", [X" + (rn).ToString() + ", #" + (simm).ToString() + "]!");
		}
		insn_177:
		/* LDR-simd-immediate-unsigned-offset */
		if((insn & 0x3F400000) == 0x3D400000) {
			var size = (insn >> 30) & 0x3U;
			var ropc = (insn >> 23) & 0x1U;
			var rawimm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var opc = (byte) ((byte) (((byte) (((byte) ((byte) ((byte) ((byte) 0x1)))) << 0)) | ((byte) (((byte) (ropc)) << 1))));
			var m = (byte) ((byte) (((byte) (((byte) (opc)) << 0)) | ((byte) (((byte) (size)) << 2))));
			var r = (string) (m switch { (byte) ((byte) 0x1) => "B", (byte) ((byte) 0x5) => "H", (byte) ((byte) 0x9) => "S", (byte) ((byte) 0xD) => "D", _ => "Q" });
			var imm = (uint) (((uint) ((uint) (rawimm))) << (int) ((byte) (m switch { (byte) ((byte) 0x1) => (byte) 0x0, (byte) ((byte) 0x5) => (byte) ((byte) 0x1), (byte) ((byte) 0x9) => (byte) ((byte) 0x2), (byte) ((byte) 0xD) => (byte) ((byte) 0x3), _ => (byte) ((byte) 0x4) })));
			return (string) ("ldr " + r + (rt).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]");
		}
		insn_178:
		/* LDR-simd-literal */
		if((insn & 0x3F000000) == 0x1C000000) {
			var size = (insn >> 30) & 0x3U;
			var imm = (insn >> 5) & 0x7FFFFU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (size switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", (byte) ((byte) 0x2) => "Q", _ => throw new NotImplementedException() });
			var addr = (ulong) (((ulong) (ulong) ((ulong) (pc))) + ((ulong) (long) ((long) (Math.SignExt<long>((uint) ((uint) (((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 0)) | ((uint) (((uint) (imm)) << 2)))), 21)))));
			return (string) ("ldr " + r + (rt).ToString() + ", #" + (addr).ToString());
		}
		insn_179:
		/* LDR-simd-register */
		if((insn & 0x3F600C00) == 0x3C600800) {
			var size = (insn >> 30) & 0x3U;
			var opc = (insn >> 23) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var scale = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r1 = (string) (((bool) ((((bool) (((byte) (size)) == ((byte) 0x0))) & ((bool) (((byte) (opc)) == ((byte) 0x1)))))) ? (string) ("Q") : (string) ((string) (size switch { (byte) ((byte) 0x0) => "B", (byte) ((byte) 0x1) => "H", (byte) ((byte) 0x2) => "S", (byte) ((byte) 0x3) => "D", _ => throw new NotImplementedException() })));
			var r2 = (string) (((bool) (((byte) (((option) & ((byte) ((byte) ((byte) 0x1)))))) != ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var extend = (string) (option switch { (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x6) => "SXTW", (byte) ((byte) 0x7) => "SXTX", _ => throw new NotImplementedException() });
			var amount = (byte) (((byte) (byte) (scale)) * ((byte) (byte) ((byte) (((bool) ((((bool) (((byte) (size)) == ((byte) 0x0))) & ((bool) (((byte) (opc)) == ((byte) 0x1)))))) ? (byte) ((byte) 0x4) : (byte) ((byte) (size switch { (byte) ((byte) 0x0) => (byte) 0x1, (byte) ((byte) 0x1) => (byte) ((byte) 0x1), (byte) ((byte) 0x2) => (byte) ((byte) 0x2), (byte) ((byte) 0x3) => (byte) ((byte) 0x3), _ => throw new NotImplementedException() }))))));
			return (string) ("ldr " + r1 + (rt).ToString() + ", [X" + (rn).ToString() + ", " + r2 + (rm).ToString() + ", " + extend + " " + (amount).ToString() + "]");
		}
		insn_180:
		/* LDR-register */
		if((insn & 0xBFE00C00) == 0xB8600800) {
			var size = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var scale = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r1 = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var r2 = (string) (((bool) (((byte) (((option) & ((byte) ((byte) ((byte) 0x1)))))) != ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var amount = (byte) (((bool) (((byte) (scale)) == ((byte) 0x0))) ? (byte) ((byte) 0x0) : (byte) ((byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x3))));
			var extend = (string) (option switch { (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x6) => "SXTW", (byte) ((byte) 0x7) => "SXTX", (byte) ((byte) 0x3) => "LSL", _ => throw new NotImplementedException() });
			return (string) ("ldr " + r1 + (rt).ToString() + ", [X" + (rn).ToString() + ", " + r2 + (rm).ToString() + ", " + extend + " " + (amount).ToString() + "]");
		}
		insn_181:
		/* LDRB-immediate-postindex */
		if((insn & 0xFFE00C00) == 0x38400400) {
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt) != (rn))))
				goto insn_182;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return (string) ("ldrb W" + (rt).ToString() + ", [X" + (rn).ToString() + "], #" + (imm).ToString());
		}
		insn_182:
		/* LDRB-immediate-preindex */
		if((insn & 0xFFE00C00) == 0x38400C00) {
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt) != (rn))))
				goto insn_183;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return (string) ("ldrb W" + (rt).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]!");
		}
		insn_183:
		/* LDRB-immediate-unsigned-offset */
		if((insn & 0xFFC00000) == 0x39400000) {
			var imm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return (string) ("ldrb W" + (rt).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]");
		}
		insn_184:
		/* LDRB-register */
		if((insn & 0xFFE00C00) == 0x38600800) {
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var amount = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (((option) & ((byte) ((byte) ((byte) 0x1)))))) != ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var str = (string) (option switch { (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x6) => "SXTW", (byte) ((byte) 0x7) => "SXTX", _ => throw new NotImplementedException() });
			return (string) ("ldrb W" + (rt).ToString() + ", [X" + (rn).ToString() + ", " + r + (rm).ToString() + ", " + str + " " + (amount).ToString() + "]");
		}
		insn_185:
		/* LDRH-immediate-postindex */
		if((insn & 0xFFE00C00) == 0x78400400) {
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt) != (rn))))
				goto insn_186;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return (string) ("ldrh W" + (rt).ToString() + ", [X" + (rn).ToString() + "], #" + (imm).ToString());
		}
		insn_186:
		/* LDRH-immediate-preindex */
		if((insn & 0xFFE00C00) == 0x78400C00) {
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt) != (rn))))
				goto insn_187;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return (string) ("ldrh W" + (rt).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]!");
		}
		insn_187:
		/* LDRH-immediate-unsigned-offset */
		if((insn & 0xFFC00000) == 0x79400000) {
			var rawimm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var imm = (ushort) ((rawimm) << (int) ((byte) 0x1));
			return (string) ("ldrh W" + (rt).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]");
		}
		insn_188:
		/* LDRH-register */
		if((insn & 0xFFE00C00) == 0x78600800) {
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var amount = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (((option) & ((byte) ((byte) ((byte) 0x1)))))) != ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var str = (string) (option switch { (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x6) => "SXTW", (byte) ((byte) 0x7) => "SXTX", _ => throw new NotImplementedException() });
			return (string) ("ldrh W" + (rt).ToString() + ", [X" + (rn).ToString() + ", " + r + (rm).ToString() + ", " + str + " " + (amount).ToString() + "]");
		}
		insn_189:
		/* LDRSB-immediate-postindex */
		if((insn & 0xFFA00C00) == 0x38800400) {
			var opc = (insn >> 22) & 0x1U;
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt) != (rn))))
				goto insn_190;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			var r = (string) (((bool) (((byte) (opc)) == ((byte) 0x1))) ? (string) ("W") : (string) ("X"));
			return (string) ("ldrsb " + r + (rt).ToString() + ", [X" + (rn).ToString() + "], #" + (imm).ToString());
		}
		insn_190:
		/* LDRSB-immediate-preindex */
		if((insn & 0xFFA00C00) == 0x38800C00) {
			var opc = (insn >> 22) & 0x1U;
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt) != (rn))))
				goto insn_191;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			var r = (string) (((bool) (((byte) (opc)) == ((byte) 0x1))) ? (string) ("W") : (string) ("X"));
			return (string) ("ldrsb " + r + (rt).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]!");
		}
		insn_191:
		/* LDRSB-immediate-unsigned-offset */
		if((insn & 0xFF800000) == 0x39800000) {
			var opc = (insn >> 22) & 0x1U;
			var imm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (opc)) == ((byte) 0x1))) ? (string) ("W") : (string) ("X"));
			return (string) ("ldrsb " + r + (rt).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]");
		}
		insn_192:
		/* LDRSB-register */
		if((insn & 0xFFA00C00) == 0x38A00800) {
			var opc = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var amount = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (opc)) == ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var str = (string) (option switch { (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x6) => "SXTW", (byte) ((byte) 0x7) => "SXTX", _ => throw new NotImplementedException() });
			return (string) ("ldrsb " + r + (rt).ToString() + ", [X" + (rn).ToString() + ", " + r + (rm).ToString() + ", " + str + " " + (amount).ToString() + "]");
		}
		insn_193:
		/* LDRSH-immediate-postindex */
		if((insn & 0xFFA00C00) == 0x78800400) {
			var opc = (insn >> 22) & 0x1U;
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt) != (rn))))
				goto insn_194;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			var r = (string) (((bool) (((byte) (opc)) == ((byte) 0x1))) ? (string) ("W") : (string) ("X"));
			return (string) ("ldrsh " + r + (rt).ToString() + ", [X" + (rn).ToString() + "], #" + (imm).ToString());
		}
		insn_194:
		/* LDRSH-immediate-preindex */
		if((insn & 0xFFA00C00) == 0x78800C00) {
			var opc = (insn >> 22) & 0x1U;
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt) != (rn))))
				goto insn_195;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			var r = (string) (((bool) (((byte) (opc)) == ((byte) 0x1))) ? (string) ("W") : (string) ("X"));
			return (string) ("ldrsh " + r + (rt).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]!");
		}
		insn_195:
		/* LDRSH-immediate-unsigned-offset */
		if((insn & 0xFF800000) == 0x79800000) {
			var opc = (insn >> 22) & 0x1U;
			var rawimm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (opc)) == ((byte) 0x1))) ? (string) ("W") : (string) ("X"));
			var imm = (ushort) ((rawimm) << (int) ((byte) 0x1));
			return (string) ("ldrsh " + r + (rt).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]");
		}
		insn_196:
		/* LDRSH-register */
		if((insn & 0xFFA00C00) == 0x78A00800) {
			var opc = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var amount = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (opc)) == ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var str = (string) (option switch { (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x6) => "SXTW", (byte) ((byte) 0x7) => "SXTX", _ => throw new NotImplementedException() });
			return (string) ("ldrsh " + r + (rt).ToString() + ", [X" + (rn).ToString() + ", " + r + (rm).ToString() + ", " + str + " " + (amount).ToString() + "]");
		}
		insn_197:
		/* LDRSW-immediate-postindex */
		if((insn & 0xFFE00C00) == 0xB8800400) {
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt) != (rn))))
				goto insn_198;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return (string) ("ldrsw X" + (rt).ToString() + ", [X" + (rn).ToString() + "], #" + (imm).ToString());
		}
		insn_198:
		/* LDRSW-immediate-preindex */
		if((insn & 0xFFE00C00) == 0xB8800C00) {
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt) != (rn))))
				goto insn_199;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return (string) ("ldrsw X" + (rt).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]!");
		}
		insn_199:
		/* LDRSW-immediate-unsigned-offset */
		if((insn & 0xFFC00000) == 0xB9800000) {
			var rawimm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var imm = (ushort) ((rawimm) << (int) ((byte) 0x2));
			return (string) ("ldrsw X" + (rt).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]");
		}
		insn_200:
		/* LDRSW-literal */
		if((insn & 0xFF000000) == 0x98000000) {
			var imm = (insn >> 5) & 0x7FFFFU;
			var rt = (insn >> 0) & 0x1FU;
			var addr = (ulong) (((ulong) (ulong) ((ulong) (pc))) + ((ulong) (long) ((long) (Math.SignExt<long>((uint) ((uint) (((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 0)) | ((uint) (((uint) (imm)) << 2)))), 21)))));
			return (string) ("ldrsw X" + (rt).ToString() + ", #" + (addr).ToString());
		}
		insn_201:
		/* LDRSW-register */
		if((insn & 0xFFE00C00) == 0xB8A00800) {
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var scale = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (((option) & ((byte) ((byte) ((byte) 0x1)))))) != ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var amount = (byte) (((bool) (((byte) (scale)) == ((byte) 0x0))) ? (byte) ((byte) 0x0) : (byte) ((byte) 0x2));
			var extend = (string) (option switch { (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x6) => "SXTW", (byte) ((byte) 0x7) => "SXTX", _ => throw new NotImplementedException() });
			return (string) ("ldrsw X" + (rt).ToString() + ", [X" + (rn).ToString() + ", " + r + (rm).ToString() + ", " + extend + " " + (amount).ToString() + "]");
		}
		insn_202:
		/* LDUR */
		if((insn & 0xBFE00C00) == 0xB8400000) {
			var size = (insn >> 30) & 0x1U;
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return (string) ("ldur " + r + (rd).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]");
		}
		insn_203:
		/* LDURB */
		if((insn & 0xFFE00C00) == 0x38400000) {
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return (string) ("ldurb W" + (rd).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]");
		}
		insn_204:
		/* LDURH */
		if((insn & 0xFFE00C00) == 0x78400000) {
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return (string) ("ldurh W" + (rd).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]");
		}
		insn_205:
		/* LDURSB */
		if((insn & 0xFFA00C00) == 0x38800000) {
			var opc = (insn >> 22) & 0x1U;
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (opc)) == ((byte) 0x1))) ? (string) ("W") : (string) ("X"));
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return (string) ("ldursb " + r + (rd).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]");
		}
		insn_206:
		/* LDURSH */
		if((insn & 0xFFA00C00) == 0x78800000) {
			var opc = (insn >> 22) & 0x1U;
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (opc)) == ((byte) 0x1))) ? (string) ("W") : (string) ("X"));
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return (string) ("ldursh " + r + (rd).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]");
		}
		insn_207:
		/* LDURSW */
		if((insn & 0xFFE00C00) == 0xB8800000) {
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return (string) ("ldursw X" + (rt).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]");
		}
		insn_208:
		/* LDUR-simd */
		if((insn & 0x3F600C00) == 0x3C400000) {
			var size = (insn >> 30) & 0x3U;
			var opc = (insn >> 23) & 0x1U;
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) ((byte) ((byte) (((byte) (((byte) (opc)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "B", (byte) ((byte) 0x2) => "H", (byte) ((byte) 0x4) => "S", (byte) ((byte) 0x6) => "D", (byte) ((byte) 0x1) => "Q", _ => throw new NotImplementedException() });
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return (string) ("ldur " + r + (rt).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]");
		}
		insn_209:
		/* LDXR */
		if((insn & 0xBFFFFC00) == 0x885F7C00) {
			var size = (insn >> 30) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("ldxr " + r + (rt).ToString() + ", [X" + (rn).ToString() + "]");
		}
		insn_210:
		/* LDXRB */
		if((insn & 0xFFFFFC00) == 0x085F7C00) {
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return (string) ("ldxrb W" + (rt).ToString() + ", [X" + (rn).ToString() + "]");
		}
		insn_211:
		/* LDXRH */
		if((insn & 0xFFFFFC00) == 0x485F7C00) {
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return (string) ("ldxrh W" + (rt).ToString() + ", [X" + (rn).ToString() + "]");
		}
		insn_212:
		/* LDXP */
		if((insn & 0xBFFF8000) == 0x887F0000) {
			var size = (insn >> 30) & 0x1U;
			var rt2 = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("ldxp " + r + (rt).ToString() + ", " + r + (rt2).ToString() + ", [X" + (rn).ToString() + "]");
		}
		insn_213:
		/* LSL-register */
		if((insn & 0x7FE0FC00) == 0x1AC02000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("lsl " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString());
		}
		insn_214:
		/* LSRV */
		if((insn & 0x7FE0FC00) == 0x1AC02400) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("lsrv " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString());
		}
		insn_215:
		/* MADD */
		if((insn & 0x7FE08000) == 0x1B000000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var ra = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("madd " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", " + r + (ra).ToString());
		}
		insn_216:
		/* MOVI-scalar-64bit */
		if((insn & 0xFFF8FC00) == 0x2F00E400) {
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var imm8a = (byte) ((byte) (((bool) ((a) != ((byte) 0x0))) ? (byte) ((byte) 0xFF) : (byte) ((byte) 0x0)));
			var imm8b = (byte) ((byte) (((bool) ((b) != ((byte) 0x0))) ? (byte) ((byte) 0xFF) : (byte) ((byte) 0x0)));
			var imm8c = (byte) ((byte) (((bool) ((c) != ((byte) 0x0))) ? (byte) ((byte) 0xFF) : (byte) ((byte) 0x0)));
			var imm8d = (byte) ((byte) (((bool) ((d) != ((byte) 0x0))) ? (byte) ((byte) 0xFF) : (byte) ((byte) 0x0)));
			var imm8e = (byte) ((byte) (((bool) ((e) != ((byte) 0x0))) ? (byte) ((byte) 0xFF) : (byte) ((byte) 0x0)));
			var imm8f = (byte) ((byte) (((bool) ((f) != ((byte) 0x0))) ? (byte) ((byte) 0xFF) : (byte) ((byte) 0x0)));
			var imm8g = (byte) ((byte) (((bool) ((g) != ((byte) 0x0))) ? (byte) ((byte) 0xFF) : (byte) ((byte) 0x0)));
			var imm8h = (byte) ((byte) (((bool) ((h) != ((byte) 0x0))) ? (byte) ((byte) 0xFF) : (byte) ((byte) 0x0)));
			var imm = (ulong) ((ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (((ulong) (imm8h)) << 0)) | ((ulong) (((ulong) (imm8g)) << 8)))) | ((ulong) (((ulong) (imm8f)) << 16)))) | ((ulong) (((ulong) (imm8e)) << 24)))) | ((ulong) (((ulong) (imm8d)) << 32)))) | ((ulong) (((ulong) (imm8c)) << 40)))) | ((ulong) (((ulong) (imm8b)) << 48)))) | ((ulong) (((ulong) (imm8a)) << 56))));
			return (string) ("movi D" + (rd).ToString() + ", #" + (imm).ToString());
		}
		insn_217:
		/* MOVI-vector-8bit */
		if((insn & 0xBFF8FC00) == 0x0F00E400) {
			var Q = (insn >> 30) & 0x1U;
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"));
			var imm = (byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (h)) << 0)) | ((byte) (((byte) (g)) << 1)))) | ((byte) (((byte) (f)) << 2)))) | ((byte) (((byte) (e)) << 3)))) | ((byte) (((byte) (d)) << 4)))) | ((byte) (((byte) (c)) << 5)))) | ((byte) (((byte) (b)) << 6)))) | ((byte) (((byte) (a)) << 7))));
			return (string) ("movi V" + (rd).ToString() + "." + t + ", #" + (imm).ToString());
		}
		insn_218:
		/* MOVI-vector-16bit */
		if((insn & 0xBFF8DC00) == 0x0F008400) {
			var Q = (insn >> 30) & 0x1U;
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var cmode = (insn >> 13) & 0x1U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("8H") : (string) ("4H"));
			var imm = (byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (h)) << 0)) | ((byte) (((byte) (g)) << 1)))) | ((byte) (((byte) (f)) << 2)))) | ((byte) (((byte) (e)) << 3)))) | ((byte) (((byte) (d)) << 4)))) | ((byte) (((byte) (c)) << 5)))) | ((byte) (((byte) (b)) << 6)))) | ((byte) (((byte) (a)) << 7))));
			return (string) ("movi V" + (rd).ToString() + "." + t + ", #" + (imm).ToString());
		}
		insn_219:
		/* MOVI-vector-32bit */
		if((insn & 0xBFF89C00) == 0x0F000400) {
			var Q = (insn >> 30) & 0x1U;
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var cmode = (insn >> 13) & 0x3U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("4S") : (string) ("2S"));
			var amount = (byte) (cmode switch { (byte) ((byte) 0x0) => (byte) 0x0, (byte) ((byte) 0x1) => (byte) ((byte) 0x8), (byte) ((byte) 0x2) => (byte) ((byte) 0x10), (byte) ((byte) 0x3) => (byte) ((byte) 0x18), _ => throw new NotImplementedException() });
			var imm = (byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (h)) << 0)) | ((byte) (((byte) (g)) << 1)))) | ((byte) (((byte) (f)) << 2)))) | ((byte) (((byte) (e)) << 3)))) | ((byte) (((byte) (d)) << 4)))) | ((byte) (((byte) (c)) << 5)))) | ((byte) (((byte) (b)) << 6)))) | ((byte) (((byte) (a)) << 7))));
			return (string) ("movi V" + (rd).ToString() + "." + t + ", #" + (imm).ToString() + ", LSL #" + (amount).ToString());
		}
		insn_220:
		/* MOVI-Vx.2D */
		if((insn & 0xFFF8FC00) == 0x6F00E400) {
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var imm = (ulong) ((ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (((ulong) ((byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (h)) << 0)) | ((byte) (((byte) (h)) << 1)))) | ((byte) (((byte) (h)) << 2)))) | ((byte) (((byte) (h)) << 3)))) | ((byte) (((byte) (h)) << 4)))) | ((byte) (((byte) (h)) << 5)))) | ((byte) (((byte) (h)) << 6)))) | ((byte) (((byte) (h)) << 7)))))) << 0)) | ((ulong) (((ulong) ((byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (g)) << 0)) | ((byte) (((byte) (g)) << 1)))) | ((byte) (((byte) (g)) << 2)))) | ((byte) (((byte) (g)) << 3)))) | ((byte) (((byte) (g)) << 4)))) | ((byte) (((byte) (g)) << 5)))) | ((byte) (((byte) (g)) << 6)))) | ((byte) (((byte) (g)) << 7)))))) << 8)))) | ((ulong) (((ulong) ((byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (f)) << 0)) | ((byte) (((byte) (f)) << 1)))) | ((byte) (((byte) (f)) << 2)))) | ((byte) (((byte) (f)) << 3)))) | ((byte) (((byte) (f)) << 4)))) | ((byte) (((byte) (f)) << 5)))) | ((byte) (((byte) (f)) << 6)))) | ((byte) (((byte) (f)) << 7)))))) << 16)))) | ((ulong) (((ulong) ((byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (e)) << 0)) | ((byte) (((byte) (e)) << 1)))) | ((byte) (((byte) (e)) << 2)))) | ((byte) (((byte) (e)) << 3)))) | ((byte) (((byte) (e)) << 4)))) | ((byte) (((byte) (e)) << 5)))) | ((byte) (((byte) (e)) << 6)))) | ((byte) (((byte) (e)) << 7)))))) << 24)))) | ((ulong) (((ulong) ((byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (d)) << 0)) | ((byte) (((byte) (d)) << 1)))) | ((byte) (((byte) (d)) << 2)))) | ((byte) (((byte) (d)) << 3)))) | ((byte) (((byte) (d)) << 4)))) | ((byte) (((byte) (d)) << 5)))) | ((byte) (((byte) (d)) << 6)))) | ((byte) (((byte) (d)) << 7)))))) << 32)))) | ((ulong) (((ulong) ((byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (c)) << 0)) | ((byte) (((byte) (c)) << 1)))) | ((byte) (((byte) (c)) << 2)))) | ((byte) (((byte) (c)) << 3)))) | ((byte) (((byte) (c)) << 4)))) | ((byte) (((byte) (c)) << 5)))) | ((byte) (((byte) (c)) << 6)))) | ((byte) (((byte) (c)) << 7)))))) << 40)))) | ((ulong) (((ulong) ((byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (b)) << 0)) | ((byte) (((byte) (b)) << 1)))) | ((byte) (((byte) (b)) << 2)))) | ((byte) (((byte) (b)) << 3)))) | ((byte) (((byte) (b)) << 4)))) | ((byte) (((byte) (b)) << 5)))) | ((byte) (((byte) (b)) << 6)))) | ((byte) (((byte) (b)) << 7)))))) << 48)))) | ((ulong) (((ulong) ((byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (a)) << 0)) | ((byte) (((byte) (a)) << 1)))) | ((byte) (((byte) (a)) << 2)))) | ((byte) (((byte) (a)) << 3)))) | ((byte) (((byte) (a)) << 4)))) | ((byte) (((byte) (a)) << 5)))) | ((byte) (((byte) (a)) << 6)))) | ((byte) (((byte) (a)) << 7)))))) << 56))));
			return (string) ("movi V" + (rd).ToString() + ", #" + (imm).ToString());
		}
		insn_221:
		/* MOVK */
		if((insn & 0x7F800000) == 0x72800000) {
			var size = (insn >> 31) & 0x1U;
			var hw = (insn >> 21) & 0x3U;
			var imm = (insn >> 5) & 0xFFFFU;
			var rd = (insn >> 0) & 0x1FU;
			if((bool) (((byte) (size)) == ((byte) 0x0))) {
				if(!((bool) (((byte) (hw)) < ((byte) 0x2))))
					goto insn_222;
			}
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shift = (uint) (((uint) ((uint) (hw))) << (int) ((byte) 0x4));
			return (string) ("movk " + r + (rd).ToString() + ", #" + (imm).ToString() + ", LSL #" + (shift).ToString());
		}
		insn_222:
		/* MOVN */
		if((insn & 0x7F800000) == 0x12800000) {
			var size = (insn >> 31) & 0x1U;
			var hw = (insn >> 21) & 0x3U;
			var imm = (insn >> 5) & 0xFFFFU;
			var rd = (insn >> 0) & 0x1FU;
			if((bool) (((byte) (size)) == ((byte) 0x0))) {
				if(!((bool) (((byte) (hw)) < ((byte) 0x2))))
					goto insn_223;
			}
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shift = (uint) (((uint) ((uint) (hw))) << (int) ((byte) 0x4));
			return (string) ("movn " + r + (rd).ToString() + ", #" + (imm).ToString() + ", LSL #" + (shift).ToString());
		}
		insn_223:
		/* MOVZ */
		if((insn & 0x7F800000) == 0x52800000) {
			var size = (insn >> 31) & 0x1U;
			var hw = (insn >> 21) & 0x3U;
			var imm = (insn >> 5) & 0xFFFFU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shift = (uint) (((uint) ((uint) (hw))) << (int) ((byte) 0x4));
			return (string) ("movz " + r + (rd).ToString() + ", #" + (imm).ToString() + ", LSL #" + (shift).ToString());
		}
		insn_224:
		/* MRS */
		if((insn & 0xFFF00000) == 0xD5300000) {
			var op0 = (insn >> 19) & 0x1U;
			var op1 = (insn >> 16) & 0x7U;
			var cn = (insn >> 12) & 0xFU;
			var cm = (insn >> 8) & 0xFU;
			var op2 = (insn >> 5) & 0x7U;
			var rt = (insn >> 0) & 0x1FU;
			return (string) ("mrs S" + (op0).ToString() + " " + (op1).ToString() + " " + (cn).ToString() + " " + (cm).ToString() + " " + (op2).ToString() + ", X" + (rt).ToString());
		}
		insn_225:
		/* MSR-register */
		if((insn & 0xFFF00000) == 0xD5100000) {
			var op0 = (insn >> 19) & 0x1U;
			var op1 = (insn >> 16) & 0x7U;
			var cn = (insn >> 12) & 0xFU;
			var cm = (insn >> 8) & 0xFU;
			var op2 = (insn >> 5) & 0x7U;
			var rt = (insn >> 0) & 0x1FU;
			return (string) ("msr S" + (op0).ToString() + " " + (op1).ToString() + " " + (cn).ToString() + " " + (cm).ToString() + " " + (op2).ToString() + ", X" + (rt).ToString());
		}
		insn_226:
		/* MSUB */
		if((insn & 0x7FE08000) == 0x1B008000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var ra = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("msub " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", " + r + (ra).ToString());
		}
		insn_227:
		/* MUL-by-element */
		if((insn & 0xBF00F400) == 0x0F008000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var L = (insn >> 21) & 0x1U;
			var M = (insn >> 20) & 0x1U;
			var rv = (insn >> 16) & 0xFU;
			var H = (insn >> 11) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var rm = (byte) (((bool) (((byte) (size)) == ((byte) 0x2))) ? (byte) ((byte) ((byte) (((byte) (((byte) (rv)) << 0)) | ((byte) (((byte) (M)) << 4))))) : (byte) (rv));
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", _ => throw new NotImplementedException() });
			var ts = (string) (size switch { (byte) ((byte) 0x1) => "H", (byte) ((byte) 0x2) => "S", _ => throw new NotImplementedException() });
			var index = (byte) (size switch { (byte) ((byte) 0x1) => (byte) ((byte) (((byte) (byte) (((byte) (((byte) (M)) << 0)) | ((byte) (((byte) (L)) << 1)))) | ((byte) (((byte) (H)) << 2)))), (byte) ((byte) 0x2) => (byte) ((byte) ((byte) (((byte) (((byte) (L)) << 0)) | ((byte) (((byte) (H)) << 1))))), _ => throw new NotImplementedException() });
			return (string) ("mul V" + (rd).ToString() + "." + t + ", V" + (rn).ToString() + "." + t + ", V" + (rm).ToString() + "." + ts + "[" + (index).ToString() + "]");
		}
		insn_228:
		/* MUL-vector */
		if((insn & 0xBF20FC00) == 0x0E209C00) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", _ => throw new NotImplementedException() });
			return (string) ("mul V" + (rd).ToString() + "." + t + ", V" + (rn).ToString() + "." + t + ", V" + (rm).ToString() + "." + t);
		}
		insn_229:
		/* MVNI-vector-16bit */
		if((insn & 0xBFF8DC00) == 0x2F008400) {
			var Q = (insn >> 30) & 0x1U;
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var cmode = (insn >> 13) & 0x1U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("8H") : (string) ("4H"));
			var imm = (byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (h)) << 0)) | ((byte) (((byte) (g)) << 1)))) | ((byte) (((byte) (f)) << 2)))) | ((byte) (((byte) (e)) << 3)))) | ((byte) (((byte) (d)) << 4)))) | ((byte) (((byte) (c)) << 5)))) | ((byte) (((byte) (b)) << 6)))) | ((byte) (((byte) (a)) << 7))));
			var amount = (byte) (((bool) ((cmode) != ((byte) 0x0))) ? (byte) ((byte) 0x8) : (byte) ((byte) 0x0));
			return (string) ("mvni V" + (rd).ToString() + "." + t + ", #" + (imm).ToString() + ", LSL #" + (amount).ToString());
		}
		insn_230:
		/* MVNI-vector-32bit-LSL */
		if((insn & 0xBFF89C00) == 0x2F000400) {
			var Q = (insn >> 30) & 0x1U;
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var cmode = (insn >> 13) & 0x3U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("4S") : (string) ("2S"));
			var amount = (byte) (cmode switch { (byte) ((byte) 0x0) => (byte) 0x0, (byte) ((byte) 0x1) => (byte) ((byte) 0x8), (byte) ((byte) 0x2) => (byte) ((byte) 0x10), (byte) ((byte) 0x3) => (byte) ((byte) 0x18), _ => throw new NotImplementedException() });
			var imm = (byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (h)) << 0)) | ((byte) (((byte) (g)) << 1)))) | ((byte) (((byte) (f)) << 2)))) | ((byte) (((byte) (e)) << 3)))) | ((byte) (((byte) (d)) << 4)))) | ((byte) (((byte) (c)) << 5)))) | ((byte) (((byte) (b)) << 6)))) | ((byte) (((byte) (a)) << 7))));
			return (string) ("mvni V" + (rd).ToString() + "." + t + ", #" + (imm).ToString() + ", LSL #" + (amount).ToString());
		}
		insn_231:
		/* MVNI-vector-32bit-MSL */
		if((insn & 0xBFF8EC00) == 0x2F00C400) {
			var Q = (insn >> 30) & 0x1U;
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var cmode = (insn >> 12) & 0x1U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("4S") : (string) ("2S"));
			var amount = (byte) (((bool) ((cmode) != ((byte) 0x0))) ? (byte) ((byte) 0x10) : (byte) ((byte) 0x8));
			var imm = (byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (h)) << 0)) | ((byte) (((byte) (g)) << 1)))) | ((byte) (((byte) (f)) << 2)))) | ((byte) (((byte) (e)) << 3)))) | ((byte) (((byte) (d)) << 4)))) | ((byte) (((byte) (c)) << 5)))) | ((byte) (((byte) (b)) << 6)))) | ((byte) (((byte) (a)) << 7))));
			return (string) ("mvni V" + (rd).ToString() + "." + t + ", #" + (imm).ToString() + ", MSL #" + (amount).ToString());
		}
		insn_232:
		/* NEG-vector */
		if((insn & 0xBF3FFC00) == 0x2E20B800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("neg V" + (rd).ToString() + "." + t + ", V" + (rn).ToString() + "." + t);
		}
		insn_233:
		/* NOP */
		if((insn & 0xFFFFFFFF) == 0xD503201F) {
			return "nop";
		}
		insn_234:
		/* ORN-shifted-register */
		if((insn & 0x7F200000) == 0x2A200000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_235;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return (string) ("orn " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", " + shiftstr + " #" + (imm).ToString());
		}
		insn_235:
		/* ORR-immediate */
		if((insn & 0x7F800000) == 0x32000000) {
			var size = (insn >> 31) & 0x1U;
			var up = (insn >> 22) & 0x1U;
			var immr = (insn >> 16) & 0x3FU;
			var imms = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var imm = (ulong) (MakeWMask(up, imms, immr, (byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x20) : (byte) ((byte) 0x40)), (byte) 0x1));
			return (string) ("orr " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", #" + (imm).ToString());
		}
		insn_236:
		/* ORR-shifted-register */
		if((insn & 0x7F200000) == 0x2A000000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_237;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return (string) ("orr " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", " + shiftstr + " #" + (imm).ToString());
		}
		insn_237:
		/* ORR-simd-register */
		if((insn & 0xBFE0FC00) == 0x0EA01C00) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) (((bool) (((byte) (Q)) == ((byte) 0x0))) ? (string) ("8B") : (string) ("16B"));
			return (string) ("orr V" + (rd).ToString() + "." + t + ", V" + (rn).ToString() + "." + t + ", V" + (rm).ToString() + "." + t);
		}
		insn_238:
		/* PMULL[2] */
		if((insn & 0xBF20FC00) == 0x0E20E000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var h = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("2") : (string) (""));
			var Ta = (string) (size switch { (byte) ((byte) 0x0) => "8H", (byte) ((byte) 0x3) => "1Q", _ => throw new NotImplementedException() });
			var Tb = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x6) => "1D", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("pmull" + h + " V" + (rd).ToString() + "." + Ta + ", V" + (rn).ToString() + "." + Tb + ", V" + (rm).ToString() + "." + Tb);
		}
		insn_239:
		/* PRFM-immediate */
		if((insn & 0xFFC00000) == 0xF9800000) {
			var imm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var imm5 = (insn >> 0) & 0x1FU;
			var pimm = (ushort) (((ushort) (ushort) (imm)) * ((ushort) (byte) ((byte) 0x8)));
			return (string) ("prfm #" + (imm5).ToString() + ", [X" + (rn).ToString() + ", #" + (pimm).ToString() + "]");
		}
		insn_240:
		/* PRFM-literal */
		if((insn & 0xFF000000) == 0xD8000000) {
			var imm = (insn >> 5) & 0x7FFFFU;
			var rt = (insn >> 0) & 0x1FU;
			return "prfm TODO";
		}
		insn_241:
		/* PRFM-register */
		if((insn & 0xFFE00C00) == 0xF8A00800) {
			var rm = (insn >> 16) & 0x1FU;
			var opt = (insn >> 13) & 0x7U;
			var S = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return "prfm TODO";
		}
		insn_242:
		/* PRFUM */
		if((insn & 0xFFE00C00) == 0xF8800000) {
			var imm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return "prfum TODO";
		}
		insn_243:
		/* RBIT */
		if((insn & 0x7FFFFC00) == 0x5AC00000) {
			var size = (insn >> 31) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("rbit " + r + (rd).ToString() + ", " + r + (rn).ToString());
		}
		insn_244:
		/* RET */
		if((insn & 0xFFFFFC1F) == 0xD65F0000) {
			var rn = (insn >> 5) & 0x1FU;
			return (string) ("ret X" + (rn).ToString());
		}
		insn_245:
		/* REV */
		if((insn & 0x7FFFF800) == 0x5AC00800) {
			var size = (insn >> 31) & 0x1U;
			var opc = (insn >> 10) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("rev " + r + (rd).ToString() + ", " + r + (rn).ToString());
		}
		insn_246:
		/* REV16 */
		if((insn & 0x7FFFFC00) == 0x5AC00400) {
			var size = (insn >> 31) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("rev16 " + r + (rd).ToString() + ", " + r + (rn).ToString());
		}
		insn_247:
		/* RORV */
		if((insn & 0x7FE0FC00) == 0x1AC02C00) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("rorv " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString());
		}
		insn_248:
		/* SBCS */
		if((insn & 0x7FE0FC00) == 0x7A000000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("sbcs " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString());
		}
		insn_249:
		/* SBFM */
		if((insn & 0x7F800000) == 0x13000000) {
			var size = (insn >> 31) & 0x1U;
			var N = (insn >> 22) & 0x1U;
			var immr = (insn >> 16) & 0x3FU;
			var imms = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imms)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_250;
			if(!((bool) (((byte) (immr)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_250;
			if((bool) ((size) != ((byte) 0x0))) {
				if(!((bool) ((N) != ((byte) 0x0))))
					goto insn_250;
			} else {
				if(!((bool) (((byte) (N)) == ((byte) 0x0))))
					goto insn_250;
			}
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("sbfm " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", #" + (immr).ToString() + ", #" + (imms).ToString());
		}
		insn_250:
		/* SCVTF-scalar-integer */
		if((insn & 0x7F3FFC00) == 0x1E220000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var st = (byte) ((byte) (((byte) (((byte) (type)) << 0)) | ((byte) (((byte) (size)) << 2))));
			var r1 = "";
			var r2 = "";
			switch(st) {
				case (byte) ((byte) 0x3): {
					r1 = "H";
					r2 = "W";
					break;
				}
				case (byte) ((byte) 0x0): {
					r1 = "S";
					r2 = "W";
					break;
				}
				case (byte) ((byte) 0x1): {
					r1 = "D";
					r2 = "W";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "H";
					r2 = "X";
					break;
				}
				case (byte) ((byte) 0x4): {
					r1 = "S";
					r2 = "X";
					break;
				}
				case (byte) ((byte) 0x5): {
					r1 = "D";
					r2 = "X";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return (string) ("scvtf " + r1 + (rd).ToString() + ", " + r2 + (rn).ToString());
		}
		insn_251:
		/* SCVTF-scalar */
		if((insn & 0xFFBFFC00) == 0x5E21D800) {
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("S") : (string) ("D"));
			return (string) ("scvtf " + r + (rd).ToString() + ", " + r + (rn).ToString());
		}
		insn_252:
		/* SCVTF-vector */
		if((insn & 0xBFBFFC00) == 0x0E21D800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return (string) ("scvtf V" + (rd).ToString() + "." + t + ", V" + (rn).ToString() + "." + t);
		}
		insn_253:
		/* SDIV */
		if((insn & 0x7FE0FC00) == 0x1AC00C00) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("sdiv " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString());
		}
		insn_254:
		/* SHL-scalar */
		if((insn & 0xFFC0FC00) == 0x5F405400) {
			var imm = (insn >> 16) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var shift = (ushort) (((ushort) (ushort) ((ushort) ((ushort) (((ushort) (((ushort) (imm)) << 0)) | ((ushort) (((ushort) ((byte) 0x1)) << 6)))))) - ((ushort) (byte) ((byte) 0x40)));
			return (string) ("shl D" + (rd).ToString() + ", D" + (rn).ToString() + ", #" + (shift).ToString());
		}
		insn_255:
		/* SHL-vector */
		if((insn & 0xBF80FC00) == 0x0F005400) {
			var Q = (insn >> 30) & 0x1U;
			var immh = (insn >> 19) & 0xFU;
			var immb = (insn >> 16) & 0x7U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = "";
			var size = (byte) 0x0;
			var shift = (byte) 0x0;
			if(!((bool) (((byte) (immh)) != ((byte) 0x0))))
				goto insn_256;
			if((bool) (((byte) (immh)) == ((byte) 0x1))) {
				T = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"));
				size = (byte) 0x1;
				shift = (byte) (((byte) (byte) ((byte) ((byte) ((byte) ((byte) (((byte) (((byte) (immb)) << 0)) | ((byte) (((byte) (immh)) << 3)))))))) - ((byte) (byte) ((byte) 0x8)));
			} else {
				if((bool) (((byte) ((byte) (((immh) & ((byte) ((byte) ((byte) 0xE))))))) == ((byte) 0x2))) {
					T = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("8H") : (string) ("4H"));
					size = (byte) 0x2;
					shift = (byte) (((byte) (byte) ((byte) ((byte) ((byte) ((byte) (((byte) (((byte) (immb)) << 0)) | ((byte) (((byte) (immh)) << 3)))))))) - ((byte) (byte) ((byte) 0x10)));
				} else {
					if((bool) (((byte) ((byte) (((immh) & ((byte) ((byte) ((byte) 0xC))))))) == ((byte) 0x4))) {
						T = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("4S") : (string) ("2S"));
						size = (byte) 0x4;
						shift = (byte) (((byte) (byte) ((byte) ((byte) ((byte) ((byte) (((byte) (((byte) (immb)) << 0)) | ((byte) (((byte) (immh)) << 3)))))))) - ((byte) (byte) ((byte) 0x20)));
					} else {
						T = (string) (((bool) ((Q) != ((byte) 0x0))) ? ("2D") : throw new NotImplementedException());
						size = (byte) 0x8;
						shift = (byte) (((byte) (byte) ((byte) ((byte) ((byte) ((byte) (((byte) (((byte) (immb)) << 0)) | ((byte) (((byte) (immh)) << 3)))))))) - ((byte) (byte) ((byte) 0x40)));
					}
				}
			}
			return (string) ("shl V" + (rd).ToString() + "." + T + ", V" + (rn).ToString() + "." + T + ", #" + (shift).ToString());
		}
		insn_256:
		/* SMADDL */
		if((insn & 0xFFE08000) == 0x9B200000) {
			var rm = (insn >> 16) & 0x1FU;
			var ra = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			return (string) ("smaddl X" + (rd).ToString() + ", W" + (rn).ToString() + ", W" + (rm).ToString() + ", X" + (ra).ToString());
		}
		insn_257:
		/* SMULH */
		if((insn & 0xFFE0FC00) == 0x9B407C00) {
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			return (string) ("smulh X" + (rd).ToString() + ", X" + (rn).ToString() + ", X" + (rm).ToString());
		}
		insn_258:
		/* SSHLL */
		if((insn & 0xBF80FC00) == 0x0F00A400) {
			var Q = (insn >> 30) & 0x1U;
			var immh = (insn >> 19) & 0xFU;
			var immb = (insn >> 16) & 0x7U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var variant = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("2") : (string) (""));
			var ta = "";
			var tb = "";
			var shift = (ulong) ((ulong) ((byte) 0x0));
			if((bool) (((byte) (immh)) == ((byte) 0x1))) {
				ta = "8H";
				tb = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"));
				shift = (byte) (((byte) (byte) ((byte) ((byte) (((byte) (((byte) (immb)) << 0)) | ((byte) (((byte) (immh)) << 3)))))) - ((byte) (byte) ((byte) 0x8)));
			} else {
				if((bool) (((byte) ((byte) (((immh) & ((byte) ((byte) ((byte) 0xE))))))) == ((byte) 0x2))) {
					ta = "4S";
					tb = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("8H") : (string) ("4H"));
					shift = (byte) (((byte) (byte) ((byte) ((byte) (((byte) (((byte) (immb)) << 0)) | ((byte) (((byte) (immh)) << 3)))))) - ((byte) (byte) ((byte) 0x10)));
				} else {
					if((bool) (((byte) ((byte) (((immh) & ((byte) ((byte) ((byte) 0xC))))))) == ((byte) 0x4))) {
						ta = "2D";
						tb = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("4S") : (string) ("2S"));
						shift = (byte) (((byte) (byte) ((byte) ((byte) (((byte) (((byte) (immb)) << 0)) | ((byte) (((byte) (immh)) << 3)))))) - ((byte) (byte) ((byte) 0x20)));
					} else {
						throw new NotImplementedException();
					}
				}
			}
			return (string) ("sshll" + variant + " V" + (rd).ToString() + "." + ta + ", V" + (rn).ToString() + "." + tb + ", #" + (shift).ToString());
		}
		insn_259:
		/* ST1-multi-no-offset-one-register */
		if((insn & 0xBFFFF000) == 0x0C007000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("st1 { V" + (rt).ToString() + "." + T + " }, [X" + (rn).ToString() + "]");
		}
		insn_260:
		/* ST1-multi-postindex-immediate-one-register */
		if((insn & 0xBFE0F000) == 0x0C807000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var imm = (byte) ((byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x10) : (byte) ((byte) 0x8)));
			if(!((bool) (((byte) (rm)) == ((byte) 0x1F))))
				goto insn_261;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("st1 { V" + (rt).ToString() + "." + T + " }, [X" + (rn).ToString() + "], #" + (imm).ToString());
		}
		insn_261:
		/* ST1-multi-postindex-register-one-register */
		if((insn & 0xBFE0F000) == 0x0C807000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (rm)) != ((byte) 0x1F))))
				goto insn_262;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("st1 { V" + (rt).ToString() + "." + T + " }, [X" + (rn).ToString() + "], X" + (rm).ToString());
		}
		insn_262:
		/* ST1-multi-no-offset-two-registers */
		if((insn & 0xBFFFF000) == 0x0C00A000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("st1 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + " }, [X" + (rn).ToString() + "]");
		}
		insn_263:
		/* ST1-multi-postindex-immediate-two-registers */
		if((insn & 0xBFE0F000) == 0x0C80A000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var imm = (byte) ((byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x20) : (byte) ((byte) 0x10)));
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) == ((byte) 0x1F))))
				goto insn_264;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("st1 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + " }, [X" + (rn).ToString() + "], #" + (imm).ToString());
		}
		insn_264:
		/* ST1-multi-postindex-register-two-registers */
		if((insn & 0xBFE0F000) == 0x0C80A000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) != ((byte) 0x1F))))
				goto insn_265;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("st1 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + " }, [X" + (rn).ToString() + "], X" + (rm).ToString());
		}
		insn_265:
		/* ST1-multi-no-offset-three-registers */
		if((insn & 0xBFFFF000) == 0x0C006000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("st1 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + ", V" + (rt3).ToString() + "." + T + " }, [X" + (rn).ToString() + "]");
		}
		insn_266:
		/* ST1-multi-postindex-immediate-three-registers */
		if((insn & 0xBFE0F000) == 0x0C806000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var imm = (byte) ((byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x30) : (byte) ((byte) 0x18)));
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) == ((byte) 0x1F))))
				goto insn_267;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("st1 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + ", V" + (rt3).ToString() + "." + T + " }, [X" + (rn).ToString() + "], #" + (imm).ToString());
		}
		insn_267:
		/* ST1-multi-postindex-register-three-registers */
		if((insn & 0xBFE0F000) == 0x0C806000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) != ((byte) 0x1F))))
				goto insn_268;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("st1 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + ", V" + (rt3).ToString() + "." + T + " }, [X" + (rn).ToString() + "], X" + (rm).ToString());
		}
		insn_268:
		/* ST1-multi-no-offset-four-registers */
		if((insn & 0xBFFFF000) == 0x0C002000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var rt4 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x3))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("st1 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + ", V" + (rt3).ToString() + "." + T + ", V" + (rt4).ToString() + "." + T + " }, [X" + (rn).ToString() + "]");
		}
		insn_269:
		/* ST1-multi-postindex-immediate-four-registers */
		if((insn & 0xBFE0F000) == 0x0C802000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var imm = (byte) ((byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x40) : (byte) ((byte) 0x20)));
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var rt4 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x3))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) == ((byte) 0x1F))))
				goto insn_270;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("st1 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + ", V" + (rt3).ToString() + "." + T + ", V" + (rt4).ToString() + "." + T + " }, [X" + (rn).ToString() + "], #" + (imm).ToString());
		}
		insn_270:
		/* ST1-multi-postindex-register-four-registers */
		if((insn & 0xBFE0F000) == 0x0C802000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var rt4 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x3))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) != ((byte) 0x1F))))
				goto insn_271;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("st1 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + ", V" + (rt3).ToString() + "." + T + ", V" + (rt4).ToString() + "." + T + " }, [X" + (rn).ToString() + "], X" + (rm).ToString());
		}
		insn_271:
		/* ST1-single-no-offset */
		if((insn & 0xBFFF2000) == 0x0D000000) {
			var Q = (insn >> 30) & 0x1U;
			var opc = (insn >> 14) & 0x3U;
			var S = (insn >> 12) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (opc)) != ((byte) 0x3))))
				goto insn_272;
			var t = (string) (((bool) (((byte) (opc)) == ((byte) 0x0))) ? (string) ("B") : (string) ((string) (((bool) ((((bool) (((byte) (opc)) == ((byte) 0x1))) & ((bool) (((byte) ((byte) (((size) & ((byte) ((byte) ((byte) 0x1))))))) == ((byte) 0x0)))))) ? (string) ("H") : (string) ((string) (((bool) (((byte) (opc)) == ((byte) 0x2))) ? ((string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("S") : (string) ((string) (((bool) ((((bool) (((byte) (size)) == ((byte) 0x1))) & ((bool) (((byte) (S)) == ((byte) 0x0)))))) ? ("D") : throw new NotImplementedException())))) : throw new NotImplementedException())))));
			var index = (uint) (opc switch { (byte) ((byte) 0x0) => (uint) ((uint) ((byte) ((byte) (((byte) (byte) (((byte) (((byte) (size)) << 0)) | ((byte) (((byte) (S)) << 2)))) | ((byte) (((byte) (Q)) << 3)))))), (byte) ((byte) 0x1) => (uint) ((uint) (((uint) ((uint) ((byte) ((byte) (((byte) (byte) (((byte) (((byte) (size)) << 0)) | ((byte) (((byte) (S)) << 2)))) | ((byte) (((byte) (Q)) << 3))))))) >> (int) ((byte) 0x1))), (byte) ((byte) 0x2) => (uint) ((uint) (((bool) (((byte) ((byte) (((size) & ((byte) ((byte) ((byte) 0x1))))))) == ((byte) 0x0))) ? (uint) ((uint) ((uint) ((byte) ((byte) (((byte) (((byte) (S)) << 0)) | ((byte) (((byte) (Q)) << 1))))))) : (uint) (Q))), _ => throw new NotImplementedException() });
			return (string) ("st1 { V" + (rt).ToString() + "." + t + " }[" + (index).ToString() + "], [X" + (rn).ToString() + "]");
		}
		insn_272:
		/* ST2-multi-no-offset */
		if((insn & 0xBFFFF000) == 0x0C008000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("st2 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + " }, [X" + (rn).ToString() + "]");
		}
		insn_273:
		/* ST2-multi-postindex-immediate */
		if((insn & 0xBFE0F000) == 0x0C808000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var imm = (byte) ((byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x20) : (byte) ((byte) 0x10)));
			if(!((bool) (((byte) (rm)) == ((byte) 0x1F))))
				goto insn_274;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("st2 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + " }, [X" + (rn).ToString() + "], #" + (imm).ToString());
		}
		insn_274:
		/* ST2-multi-postindex-register */
		if((insn & 0xBFE0F000) == 0x0C808000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) != ((byte) 0x1F))))
				goto insn_275;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("st2 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + " }, [X" + (rn).ToString() + "], X" + (rm).ToString());
		}
		insn_275:
		/* ST3-multi-no-offset */
		if((insn & 0xBFFFF000) == 0x0C004000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("st3 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + ", V" + (rt3).ToString() + "." + T + " }, [X" + (rn).ToString() + "]");
		}
		insn_276:
		/* ST3-multi-postindex-immediate */
		if((insn & 0xBFE0F000) == 0x0C804000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var imm = (byte) ((byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x30) : (byte) ((byte) 0x18)));
			if(!((bool) (((byte) (rm)) == ((byte) 0x1F))))
				goto insn_277;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("st3 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + ", V" + (rt3).ToString() + "." + T + " }, [X" + (rn).ToString() + "], #" + (imm).ToString());
		}
		insn_277:
		/* ST3-multi-postindex-register */
		if((insn & 0xBFE0F000) == 0x0C804000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) != ((byte) 0x1F))))
				goto insn_278;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("st3 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + ", V" + (rt3).ToString() + "." + T + " }, [X" + (rn).ToString() + "], X" + (rm).ToString());
		}
		insn_278:
		/* ST4-multi-no-offset */
		if((insn & 0xBFFFF000) == 0x0C000000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var rt4 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x3))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("st4 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + ", V" + (rt3).ToString() + "." + T + ", V" + (rt4).ToString() + "." + T + " }, [X" + (rn).ToString() + "]");
		}
		insn_279:
		/* ST4-multi-postindex-immediate */
		if((insn & 0xBFE0F000) == 0x0C800000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var rt4 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x3))))) % ((byte) (byte) ((byte) 0x20)));
			var imm = (byte) ((byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x40) : (byte) ((byte) 0x2B)));
			if(!((bool) (((byte) (rm)) == ((byte) 0x1F))))
				goto insn_280;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("st4 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + ", V" + (rt3).ToString() + "." + T + ", V" + (rt4).ToString() + "." + T + " }, [X" + (rn).ToString() + "], #" + (imm).ToString());
		}
		insn_280:
		/* ST4-multi-postindex-register */
		if((insn & 0xBFE0F000) == 0x0C800000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var rt4 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x3))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) != ((byte) 0x1F))))
				goto insn_281;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("st4 { V" + (rt).ToString() + "." + T + ", V" + (rt2).ToString() + "." + T + ", V" + (rt3).ToString() + "." + T + ", V" + (rt4).ToString() + "." + T + " }, [X" + (rn).ToString() + "], X" + (rm).ToString());
		}
		insn_281:
		/* STLR */
		if((insn & 0xBFFFFC00) == 0x889FFC00) {
			var size = (insn >> 30) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("stlr " + r + (rt).ToString() + ", [X" + (rn).ToString() + "]");
		}
		insn_282:
		/* STLRB */
		if((insn & 0xFFFFFC00) == 0x089FFC00) {
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return (string) ("stlrb W" + (rt).ToString() + ", [X" + (rn).ToString() + "]");
		}
		insn_283:
		/* STLRH */
		if((insn & 0xFFFFFC00) == 0x489FFC00) {
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return (string) ("stlrh W" + (rt).ToString() + ", [X" + (rn).ToString() + "]");
		}
		insn_284:
		/* STLXR */
		if((insn & 0xBFE0FC00) == 0x8800FC00) {
			var size = (insn >> 30) & 0x1U;
			var rs = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("stlxr W" + (rs).ToString() + ", " + r + (rt).ToString() + ", [X" + (rn).ToString() + "]");
		}
		insn_285:
		/* STLXRB */
		if((insn & 0xFFE0FC00) == 0x0800FC00) {
			var rs = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return (string) ("stlxrr W" + (rs).ToString() + ", W" + (rt).ToString() + ", [X" + (rn).ToString() + "]");
		}
		insn_286:
		/* STP-postindex */
		if((insn & 0x7FC00000) == 0x28800000) {
			var size = (insn >> 31) & 0x1U;
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rd = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x3))));
			return (string) ("stp " + r + (rt1).ToString() + ", " + r + (rt2).ToString() + ", [X" + (rd).ToString() + "], #" + (simm).ToString());
		}
		insn_287:
		/* STP-preindex */
		if((insn & 0x7FC00000) == 0x29800000) {
			var size = (insn >> 31) & 0x1U;
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rd = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x3))));
			return (string) ("stp " + r + (rt1).ToString() + ", " + r + (rt2).ToString() + ", [X" + (rd).ToString() + ", #" + (simm).ToString() + "]!");
		}
		insn_288:
		/* STP-signed-offset */
		if((insn & 0x7FC00000) == 0x29000000) {
			var size = (insn >> 31) & 0x1U;
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rd = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x3))));
			return (string) ("stp " + r + (rt1).ToString() + ", " + r + (rt2).ToString() + ", [X" + (rd).ToString() + ", #" + (simm).ToString() + "]");
		}
		insn_289:
		/* STP-simd-postindex */
		if((insn & 0x3FC00000) == 0x2C800000) {
			var opc = (insn >> 30) & 0x3U;
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rd = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			var r = (string) (opc switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", (byte) ((byte) 0x2) => "Q", _ => throw new NotImplementedException() });
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) (opc switch { (byte) ((byte) 0x0) => (byte) 0x2, (byte) ((byte) 0x1) => (byte) ((byte) 0x3), (byte) ((byte) 0x2) => (byte) ((byte) 0x4), _ => throw new NotImplementedException() })));
			return (string) ("stp " + r + (rt1).ToString() + ", " + r + (rt2).ToString() + ", [X" + (rd).ToString() + "], #" + (simm).ToString());
		}
		insn_290:
		/* STP-simd-preindex */
		if((insn & 0x3FC00000) == 0x2D800000) {
			var opc = (insn >> 30) & 0x3U;
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rd = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			var r = (string) (opc switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", (byte) ((byte) 0x2) => "Q", _ => throw new NotImplementedException() });
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) (opc switch { (byte) ((byte) 0x0) => (byte) 0x2, (byte) ((byte) 0x1) => (byte) ((byte) 0x3), (byte) ((byte) 0x2) => (byte) ((byte) 0x4), _ => throw new NotImplementedException() })));
			return (string) ("stp " + r + (rt1).ToString() + ", " + r + (rt2).ToString() + ", [X" + (rd).ToString() + ", #" + (simm).ToString() + "]!");
		}
		insn_291:
		/* STP-simd-signed-offset */
		if((insn & 0x3FC00000) == 0x2D000000) {
			var opc = (insn >> 30) & 0x3U;
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rd = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			var r = (string) (opc switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", (byte) ((byte) 0x2) => "Q", _ => throw new NotImplementedException() });
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) (opc switch { (byte) ((byte) 0x0) => (byte) 0x2, (byte) ((byte) 0x1) => (byte) ((byte) 0x3), (byte) ((byte) 0x2) => (byte) ((byte) 0x4), _ => throw new NotImplementedException() })));
			return (string) ("stp " + r + (rt1).ToString() + ", " + r + (rt2).ToString() + ", [X" + (rd).ToString() + ", #" + (simm).ToString() + "]");
		}
		insn_292:
		/* STR-immediate-postindex */
		if((insn & 0xBFE00C00) == 0xB8000400) {
			var size = (insn >> 30) & 0x1U;
			var imm = (insn >> 12) & 0x1FFU;
			var rd = (insn >> 5) & 0x1FU;
			var rs = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var simm = (long) (Math.SignExt<long>(imm, 9));
			return (string) ("str " + r + (rs).ToString() + ", [X" + (rd).ToString() + "], #" + (simm).ToString());
		}
		insn_293:
		/* STR-immediate-preindex */
		if((insn & 0xBFE00C00) == 0xB8000C00) {
			var size = (insn >> 30) & 0x1U;
			var imm = (insn >> 12) & 0x1FFU;
			var rd = (insn >> 5) & 0x1FU;
			var rs = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var simm = (long) (Math.SignExt<long>(imm, 9));
			return (string) ("str " + r + (rs).ToString() + ", [X" + (rd).ToString() + ", #" + (simm).ToString() + "]!");
		}
		insn_294:
		/* STR-immediate-unsigned-offset */
		if((insn & 0xBFC00000) == 0xB9000000) {
			var size = (insn >> 30) & 0x1U;
			var imm = (insn >> 10) & 0xFFFU;
			var rd = (insn >> 5) & 0x1FU;
			var rs = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var pimm = (ulong) (((ulong) ((ulong) (imm))) << (int) ((byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x3))));
			return (string) ("str " + r + (rs).ToString() + ", [X" + (rd).ToString() + ", #" + (pimm).ToString() + "]");
		}
		insn_295:
		/* STR-register */
		if((insn & 0xBFE00C00) == 0xB8200800) {
			var size = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var scale = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r1 = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var r2 = (string) (((bool) (((byte) (((option) & ((byte) ((byte) ((byte) 0x1)))))) != ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var amount = (byte) (((bool) (((byte) (scale)) == ((byte) 0x0))) ? (byte) ((byte) 0x0) : (byte) ((byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x3))));
			var extend = (string) (option switch { (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x6) => "SXTW", (byte) ((byte) 0x7) => "SXTX", (byte) ((byte) 0x3) => "LSL", _ => throw new NotImplementedException() });
			return (string) ("str " + r1 + (rt).ToString() + ", [X" + (rn).ToString() + ", " + r2 + (rm).ToString() + ", " + extend + " " + (amount).ToString() + "]");
		}
		insn_296:
		/* STR-simd-postindex */
		if((insn & 0x3F600C00) == 0x3C000400) {
			var size = (insn >> 30) & 0x3U;
			var opc = (insn >> 23) & 0x1U;
			var imm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rop = (byte) ((byte) (((byte) (byte) (((byte) (((byte) ((byte) ((byte) ((byte) 0x0)))) << 0)) | ((byte) (((byte) (opc)) << 1)))) | ((byte) (((byte) (size)) << 2))));
			var r = (string) (rop switch { (byte) ((byte) 0x0) => "B", (byte) ((byte) 0x4) => "H", (byte) ((byte) 0x8) => "S", (byte) ((byte) 0xC) => "D", (byte) ((byte) 0x2) => "Q", _ => throw new NotImplementedException() });
			var simm = (long) (Math.SignExt<long>(imm, 9));
			return (string) ("str " + r + (rt).ToString() + ", [X" + (rn).ToString() + "], #" + (simm).ToString());
		}
		insn_297:
		/* STR-simd-preindex */
		if((insn & 0x3F600C00) == 0x3C000C00) {
			var size = (insn >> 30) & 0x3U;
			var opc = (insn >> 23) & 0x1U;
			var imm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rop = (byte) ((byte) (((byte) (byte) (((byte) (((byte) ((byte) ((byte) ((byte) 0x0)))) << 0)) | ((byte) (((byte) (opc)) << 1)))) | ((byte) (((byte) (size)) << 2))));
			var r = (string) (rop switch { (byte) ((byte) 0x0) => "B", (byte) ((byte) 0x4) => "H", (byte) ((byte) 0x8) => "S", (byte) ((byte) 0xC) => "D", (byte) ((byte) 0x2) => "Q", _ => throw new NotImplementedException() });
			var scale = (byte) ((byte) (((byte) (((byte) (size)) << 0)) | ((byte) (((byte) (opc)) << 2))));
			var simm = (long) (Math.SignExt<long>(imm, 9));
			return (string) ("str " + r + (rt).ToString() + ", [X" + (rn).ToString() + ", #" + (simm).ToString() + "]!");
		}
		insn_298:
		/* STR-simd-unsigned-offset */
		if((insn & 0x3F400000) == 0x3D000000) {
			var size = (insn >> 30) & 0x3U;
			var opc = (insn >> 23) & 0x1U;
			var imm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rop = (byte) ((byte) (((byte) (byte) (((byte) (((byte) ((byte) ((byte) ((byte) 0x0)))) << 0)) | ((byte) (((byte) (opc)) << 1)))) | ((byte) (((byte) (size)) << 2))));
			var r = (string) (rop switch { (byte) ((byte) 0x0) => "B", (byte) ((byte) 0x4) => "H", (byte) ((byte) 0x8) => "S", (byte) ((byte) 0xC) => "D", (byte) ((byte) 0x2) => "Q", _ => throw new NotImplementedException() });
			var scale = (byte) ((byte) (((byte) (((byte) (size)) << 0)) | ((byte) (((byte) (opc)) << 2))));
			return (string) ("str " + r + (rt).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]");
		}
		insn_299:
		/* STR-simd-register */
		if((insn & 0x3F600C00) == 0x3C200800) {
			var size = (insn >> 30) & 0x3U;
			var opc = (insn >> 23) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var scale = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rop = (byte) ((byte) (((byte) (byte) (((byte) (((byte) ((byte) ((byte) ((byte) 0x0)))) << 0)) | ((byte) (((byte) (opc)) << 1)))) | ((byte) (((byte) (size)) << 2))));
			var r1 = (string) (rop switch { (byte) ((byte) 0x0) => "B", (byte) ((byte) 0x4) => "H", (byte) ((byte) 0x8) => "S", (byte) ((byte) 0xC) => "D", (byte) ((byte) 0x2) => "Q", _ => throw new NotImplementedException() });
			var r2 = (string) (((bool) (((byte) (((option) & ((byte) ((byte) ((byte) 0x1)))))) != ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var amount = (byte) (((bool) (((byte) (scale)) == ((byte) 0x0))) ? (byte) ((byte) 0x0) : (byte) ((byte) (size switch { (byte) ((byte) 0x1) => (byte) 0x1, (byte) ((byte) 0x2) => (byte) ((byte) 0x2), (byte) ((byte) 0x3) => (byte) ((byte) 0x3), _ => (byte) ((byte) (((bool) (((byte) (opc)) == ((byte) 0x1))) ? (byte) ((byte) 0x4) : (byte) ((byte) 0x0))) })));
			var extend = (string) (option switch { (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x6) => "SXTW", (byte) ((byte) 0x7) => "SXTX", (byte) ((byte) 0x3) => (string) (((bool) (((byte) (rop)) != ((byte) 0x0))) ? ("LSL") : throw new NotImplementedException()), _ => throw new NotImplementedException() });
			return (string) ("str " + r1 + (rt).ToString() + ", [X" + (rn).ToString() + ", " + r2 + (rm).ToString() + ", " + extend + " " + (amount).ToString() + "]");
		}
		insn_300:
		/* STRB-immediate-postindex */
		if((insn & 0xFFE00C00) == 0x38000400) {
			var imm = (insn >> 12) & 0x1FFU;
			var rd = (insn >> 5) & 0x1FU;
			var rs = (insn >> 0) & 0x1FU;
			var simm = (long) (Math.SignExt<long>(imm, 9));
			return (string) ("strb W" + (rs).ToString() + ", [X" + (rd).ToString() + "], #" + (simm).ToString());
		}
		insn_301:
		/* STRB-immediate-preindex */
		if((insn & 0xFFE00C00) == 0x38000C00) {
			var imm = (insn >> 12) & 0x1FFU;
			var rd = (insn >> 5) & 0x1FU;
			var rs = (insn >> 0) & 0x1FU;
			var simm = (long) (Math.SignExt<long>(imm, 9));
			return (string) ("strb W" + (rs).ToString() + ", [X" + (rd).ToString() + ", #" + (simm).ToString() + "]!");
		}
		insn_302:
		/* STRB-immediate-unsigned-offset */
		if((insn & 0xFFC00000) == 0x39000000) {
			var imm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return (string) ("strb W" + (rt).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]");
		}
		insn_303:
		/* STRB-register */
		if((insn & 0xFFE00C00) == 0x38200800) {
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var amount = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (((option) & ((byte) ((byte) ((byte) 0x1)))))) != ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var str = (string) (option switch { (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x6) => "SXTW", (byte) ((byte) 0x7) => "SXTX", _ => throw new NotImplementedException() });
			return (string) ("strb W" + (rt).ToString() + ", [X" + (rn).ToString() + ", " + r + (rm).ToString() + ", " + str + " " + (amount).ToString() + "]");
		}
		insn_304:
		/* STRH-immediate-postindex */
		if((insn & 0xFFE00C00) == 0x78000400) {
			var imm = (insn >> 12) & 0x1FFU;
			var rd = (insn >> 5) & 0x1FU;
			var rs = (insn >> 0) & 0x1FU;
			var simm = (long) (Math.SignExt<long>(imm, 9));
			return (string) ("strh W" + (rs).ToString() + ", [X" + (rd).ToString() + "], #" + (simm).ToString());
		}
		insn_305:
		/* STRH-immediate-preindex */
		if((insn & 0xFFE00C00) == 0x78000C00) {
			var imm = (insn >> 12) & 0x1FFU;
			var rd = (insn >> 5) & 0x1FU;
			var rs = (insn >> 0) & 0x1FU;
			var simm = (long) (Math.SignExt<long>(imm, 9));
			return (string) ("strh W" + (rs).ToString() + ", [X" + (rd).ToString() + ", #" + (simm).ToString() + "]!");
		}
		insn_306:
		/* STRH-immediate-unsigned-offset */
		if((insn & 0xFFC00000) == 0x79000000) {
			var rawimm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var imm = (ushort) ((rawimm) << (int) ((byte) 0x1));
			return (string) ("strh W" + (rt).ToString() + ", [X" + (rn).ToString() + ", #" + (imm).ToString() + "]");
		}
		insn_307:
		/* STRH-register */
		if((insn & 0xFFE00C00) == 0x78200800) {
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var amount = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (((option) & ((byte) ((byte) ((byte) 0x1)))))) != ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var str = (string) (option switch { (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x6) => "SXTW", (byte) ((byte) 0x7) => "SXTX", _ => throw new NotImplementedException() });
			return (string) ("strh W" + (rt).ToString() + ", [X" + (rn).ToString() + ", " + r + (rm).ToString() + ", " + str + " " + (amount).ToString() + "]");
		}
		insn_308:
		/* STUR */
		if((insn & 0xBFE00C00) == 0xB8000000) {
			var size = (insn >> 30) & 0x1U;
			var imm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var offset = (long) (Math.SignExt<long>(imm, 9));
			return (string) ("stur " + r + (rt).ToString() + ", [X" + (rn).ToString() + ", #" + (offset).ToString() + "]");
		}
		insn_309:
		/* STUR-simd */
		if((insn & 0x3F600C00) == 0x3C000000) {
			var size = (insn >> 30) & 0x3U;
			var opc = (insn >> 23) & 0x1U;
			var imm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rop = (byte) ((byte) (((byte) (byte) (((byte) (((byte) ((byte) ((byte) ((byte) 0x0)))) << 0)) | ((byte) (((byte) (opc)) << 1)))) | ((byte) (((byte) (size)) << 2))));
			var r = (string) (rop switch { (byte) ((byte) 0x0) => "B", (byte) ((byte) 0x4) => "H", (byte) ((byte) 0x8) => "S", (byte) ((byte) 0xC) => "D", (byte) ((byte) 0x2) => "Q", _ => throw new NotImplementedException() });
			var simm = (long) (Math.SignExt<long>(imm, 9));
			return (string) ("stur " + r + (rt).ToString() + ", [X" + (rn).ToString() + ", #" + (simm).ToString() + "]");
		}
		insn_310:
		/* STURB */
		if((insn & 0xFFE00C00) == 0x38000000) {
			var imm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var offset = (long) (Math.SignExt<long>(imm, 9));
			return (string) ("sturb W" + (rt).ToString() + ", [X" + (rn).ToString() + ", #" + (offset).ToString() + "]");
		}
		insn_311:
		/* STURH */
		if((insn & 0xFFE00C00) == 0x78000000) {
			var imm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var offset = (long) (Math.SignExt<long>(imm, 9));
			return (string) ("sturh W" + (rt).ToString() + ", [X" + (rn).ToString() + ", #" + (offset).ToString() + "]");
		}
		insn_312:
		/* STXRB */
		if((insn & 0xFFE0FC00) == 0x08007C00) {
			var rs = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return (string) ("stxrb W" + (rs).ToString() + ", W" + (rt).ToString() + ", [X" + (rn).ToString() + "]");
		}
		insn_313:
		/* STXR */
		if((insn & 0xBFE0FC00) == 0x88007C00) {
			var size = (insn >> 30) & 0x1U;
			var rs = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("stxr W" + (rs).ToString() + ", " + r + (rt).ToString() + ", [X" + (rn).ToString() + "]");
		}
		insn_314:
		/* STXP */
		if((insn & 0xBFE08000) == 0x88200000) {
			var size = (insn >> 30) & 0x1U;
			var rs = (insn >> 16) & 0x1FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("stxp W" + (rs).ToString() + ", " + r + (rt).ToString() + ", " + r + (rt2).ToString() + ", [X" + (rn).ToString() + "]");
		}
		insn_315:
		/* SUB-immediate */
		if((insn & 0x7F800000) == 0x51000000) {
			var size = (insn >> 31) & 0x1U;
			var sh = (insn >> 22) & 0x1U;
			var imm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shift = (byte) (((bool) (((byte) (sh)) == ((byte) 0x0))) ? (byte) ((byte) 0x0) : (byte) ((byte) 0xC));
			return (string) ("sub " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", #" + (imm).ToString() + ", LSL #" + (shift).ToString());
		}
		insn_316:
		/* SUB-extended-register */
		if((insn & 0x7FE00000) == 0x4B200000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var imm = (insn >> 10) & 0x7U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) 0x4))))
				goto insn_317;
			var r1 = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var r2 = (string) (((bool) (((byte) ((byte) (((option) & ((byte) ((byte) ((byte) 0x3))))))) == ((byte) 0x3))) ? (string) ("X") : (string) ("W"));
			var extend = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ((string) (option switch { (byte) ((byte) 0x0) => "UXTB", (byte) ((byte) 0x1) => "UXTH", (byte) ((byte) 0x2) => "LSL", (byte) ((byte) 0x3) => "UXTX", (byte) ((byte) 0x4) => "SXTB", (byte) ((byte) 0x5) => "SXTH", (byte) ((byte) 0x6) => "SXTW", _ => "SXTX" })) : (string) ((string) (option switch { (byte) ((byte) 0x0) => "UXTB", (byte) ((byte) 0x1) => "UXTH", (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x4) => "SXTB", (byte) ((byte) 0x5) => "SXTH", (byte) ((byte) 0x6) => "SXTW", _ => "SXTX" })));
			return (string) ("sub " + r1 + (rd).ToString() + ", " + r1 + (rn).ToString() + ", " + r2 + (rm).ToString() + ", " + extend + " #" + (imm).ToString());
		}
		insn_317:
		/* SUB-shifted-register */
		if((insn & 0x7F200000) == 0x4B000000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_318;
			if(!((bool) (((byte) (shift)) != ((byte) 0x3))))
				goto insn_318;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return (string) ("sub " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", " + shiftstr + " #" + (imm).ToString());
		}
		insn_318:
		/* SUBS-extended-register */
		if((insn & 0x7FE00000) == 0x6B200000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var imm = (insn >> 10) & 0x7U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) 0x4))))
				goto insn_319;
			var r1 = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var r2 = (string) (((bool) (((byte) ((byte) (((option) & ((byte) ((byte) ((byte) 0x3))))))) == ((byte) 0x3))) ? (string) ("X") : (string) ("W"));
			var extend = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ((string) (option switch { (byte) ((byte) 0x0) => "UXTB", (byte) ((byte) 0x1) => "UXTH", (byte) ((byte) 0x2) => "LSL", (byte) ((byte) 0x3) => "UXTX", (byte) ((byte) 0x4) => "SXTB", (byte) ((byte) 0x5) => "SXTH", (byte) ((byte) 0x6) => "SXTW", _ => "SXTX" })) : (string) ((string) (option switch { (byte) ((byte) 0x0) => "UXTB", (byte) ((byte) 0x1) => "UXTH", (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x4) => "SXTB", (byte) ((byte) 0x5) => "SXTH", (byte) ((byte) 0x6) => "SXTW", _ => "SXTX" })));
			return (string) ("subs " + r1 + (rd).ToString() + ", " + r1 + (rn).ToString() + ", " + r2 + (rm).ToString() + ", " + extend + " #" + (imm).ToString());
		}
		insn_319:
		/* SUBS-shifted-register */
		if((insn & 0x7F200000) == 0x6B000000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_320;
			if(!((bool) (((byte) (shift)) != ((byte) 0x3))))
				goto insn_320;
			var mode32 = (bool) (((byte) (size)) == ((byte) 0x0));
			var r = (string) ((mode32) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return (string) ("subs " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString() + ", " + shiftstr + " #" + (imm).ToString());
		}
		insn_320:
		/* SUBS-immediate */
		if((insn & 0x7F800000) == 0x71000000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x1U;
			var imm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL #0", (byte) ((byte) 0x1) => "LSL #12", _ => throw new NotImplementedException() });
			return (string) ("subs " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", #" + (imm).ToString() + ", " + shiftstr);
		}
		insn_321:
		/* SVC */
		if((insn & 0xFFE0001F) == 0xD4000001) {
			var imm = (insn >> 5) & 0xFFFFU;
			return (string) ("svc #" + (imm).ToString());
		}
		insn_322:
		/* SYS */
		if((insn & 0xFFF80000) == 0xD5080000) {
			var op1 = (insn >> 16) & 0x7U;
			var cn = (insn >> 12) & 0xFU;
			var cm = (insn >> 8) & 0xFU;
			var op2 = (insn >> 5) & 0x7U;
			var rt = (insn >> 0) & 0x1FU;
			return (string) ("sys #" + (op1).ToString() + ", " + (cn).ToString() + ", " + (cm).ToString() + ", #" + (op2).ToString() + ", X" + (rt).ToString());
		}
		insn_323:
		/* TBZ */
		if((insn & 0x7F000000) == 0x36000000) {
			var upper = (insn >> 31) & 0x1U;
			var bottom = (insn >> 19) & 0x1FU;
			var offset = (insn >> 5) & 0x3FFFU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (upper)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var imm = (uint) ((((uint) (((uint) ((uint) (upper))) << (int) ((byte) 0x5))) | ((uint) ((uint) (bottom)))));
			var addr = (ulong) (((ulong) (ulong) ((ulong) (pc))) + ((ulong) (long) ((long) (Math.SignExt<long>((ushort) (((ushort) ((ushort) (offset))) << (int) ((byte) 0x2)), 16)))));
			return (string) ("tbz " + r + (rt).ToString() + ", #" + (imm).ToString() + ", " + (addr).ToString());
		}
		insn_324:
		/* TBNZ */
		if((insn & 0x7F000000) == 0x37000000) {
			var upper = (insn >> 31) & 0x1U;
			var bottom = (insn >> 19) & 0x1FU;
			var offset = (insn >> 5) & 0x3FFFU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (upper)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var imm = (uint) ((((uint) (((uint) ((uint) (upper))) << (int) ((byte) 0x5))) | ((uint) ((uint) (bottom)))));
			var addr = (ulong) (((ulong) (ulong) ((ulong) (pc))) + ((ulong) (long) ((long) (Math.SignExt<long>((ushort) (((ushort) ((ushort) (offset))) << (int) ((byte) 0x2)), 16)))));
			return (string) ("tbnz " + r + (rt).ToString() + ", #" + (imm).ToString() + ", " + (addr).ToString());
		}
		insn_325:
		/* UADDLV */
		if((insn & 0xBF3FFC00) == 0x2E303800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (size switch { (byte) ((byte) 0x0) => "H", (byte) ((byte) 0x1) => "S", (byte) ((byte) 0x2) => "D", _ => throw new NotImplementedException() });
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x5) => "4S", _ => throw new NotImplementedException() });
			var esize = (byte) (((byte) 0x8) << (int) (size));
			var count = (byte) (((byte) (byte) ((byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x80) : (byte) ((byte) 0x40)))) / ((byte) (byte) (esize)));
			return (string) ("uaddlv " + r + (rd).ToString() + ", V" + (rn).ToString() + "." + t);
		}
		insn_326:
		/* UADDW[2] */
		if((insn & 0xBF20FC00) == 0x2E201000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var o2 = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("2") : (string) (""));
			var Ta = "";
			var Tb = "";
			switch((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1))))) {
				case (byte) ((byte) 0x0): {
					Ta = "8H";
					Tb = "8B";
					break;
				}
				case (byte) ((byte) 0x1): {
					Ta = "8H";
					Tb = "16B";
					break;
				}
				case (byte) ((byte) 0x2): {
					Ta = "4S";
					Tb = "4H";
					break;
				}
				case (byte) ((byte) 0x3): {
					Ta = "4S";
					Tb = "8H";
					break;
				}
				case (byte) ((byte) 0x4): {
					Ta = "2D";
					Tb = "2S";
					break;
				}
				case (byte) ((byte) 0x5): {
					Ta = "2D";
					Tb = "4S";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return (string) ("uaddw" + o2 + " V" + (rd).ToString() + "." + Ta + ", V" + (rn).ToString() + "." + Ta + ", V" + (rm).ToString() + "." + Tb);
		}
		insn_327:
		/* UBFM */
		if((insn & 0x7F800000) == 0x53000000) {
			var size = (insn >> 31) & 0x1U;
			var N = (insn >> 22) & 0x1U;
			var immr = (insn >> 16) & 0x3FU;
			var imms = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imms)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_328;
			if(!((bool) (((byte) (immr)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_328;
			if((bool) ((size) != ((byte) 0x0))) {
				if(!((bool) ((N) != ((byte) 0x0))))
					goto insn_328;
			} else {
				if(!((bool) (((byte) (N)) == ((byte) 0x0))))
					goto insn_328;
			}
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("ubfm " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", #" + (immr).ToString() + ", #" + (imms).ToString());
		}
		insn_328:
		/* UCVTF-scalar-gpr-integer */
		if((insn & 0x7F3FFC00) == 0x1E230000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var st = (byte) ((byte) (((byte) (((byte) (type)) << 0)) | ((byte) (((byte) (size)) << 2))));
			var r1 = "";
			var r2 = "";
			switch(st) {
				case (byte) ((byte) 0x3): {
					r1 = "H";
					r2 = "W";
					break;
				}
				case (byte) ((byte) 0x0): {
					r1 = "S";
					r2 = "W";
					break;
				}
				case (byte) ((byte) 0x1): {
					r1 = "D";
					r2 = "W";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "H";
					r2 = "X";
					break;
				}
				case (byte) ((byte) 0x4): {
					r1 = "S";
					r2 = "X";
					break;
				}
				case (byte) ((byte) 0x5): {
					r1 = "D";
					r2 = "X";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return (string) ("ucvtf " + r1 + (rd).ToString() + ", " + r2 + (rn).ToString());
		}
		insn_329:
		/* UCVTF-scalar-integer */
		if((insn & 0xFFBFFC00) == 0x7E21D800) {
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("S") : (string) ("D"));
			return (string) ("ucvtf " + r + (rd).ToString() + ", " + r + (rn).ToString());
		}
		insn_330:
		/* UCVTF-vector */
		if((insn & 0xBFBFFC00) == 0x2E21D800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return (string) ("ucvtf V" + (rd).ToString() + "." + t + ", V" + (rn).ToString() + "." + t);
		}
		insn_331:
		/* UDIV */
		if((insn & 0x7FE0FC00) == 0x1AC00800) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return (string) ("udiv " + r + (rd).ToString() + ", " + r + (rn).ToString() + ", " + r + (rm).ToString());
		}
		insn_332:
		/* UMADDL */
		if((insn & 0xFFE08000) == 0x9BA00000) {
			var rm = (insn >> 16) & 0x1FU;
			var ra = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			return (string) ("umaddl X" + (rd).ToString() + ", W" + (rn).ToString() + ", W" + (rm).ToString() + ", X" + (ra).ToString());
		}
		insn_333:
		/* UMAXP-UMINP */
		if((insn & 0xBF20F400) == 0x2E20A400) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var min = (insn >> 11) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var mnem = (string) (((bool) ((min) != ((byte) 0x0))) ? (string) ("uminp") : (string) ("umaxp"));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", _ => throw new NotImplementedException() });
			return (string) (mnem + " V" + (rd).ToString() + "." + T + ", V" + (rn).ToString() + "." + T + ", V" + (rm).ToString() + "." + T);
		}
		insn_334:
		/* UMOV */
		if((insn & 0xBFE0FC00) == 0x0E003C00) {
			var Q = (insn >> 30) & 0x1U;
			var imm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = "";
			var index = (byte) 0x0;
			var r = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var comb = (byte) ((byte) (((byte) (((byte) (imm)) << 0)) | ((byte) (((byte) (Q)) << 5))));
			var size = (byte) (((bool) (((byte) ((byte) (((comb) & ((byte) ((byte) ((byte) 0x21))))))) == ((byte) 0x1))) ? (byte) ((byte) 0x8) : (byte) ((byte) (((bool) (((byte) ((byte) (((comb) & ((byte) ((byte) ((byte) 0x23))))))) == ((byte) 0x2))) ? (byte) ((byte) 0x10) : (byte) ((byte) (((bool) (((byte) ((byte) (((comb) & ((byte) ((byte) ((byte) 0x27))))))) == ((byte) 0x4))) ? (byte) ((byte) 0x20) : (byte) ((byte) (((bool) (((byte) ((byte) (((comb) & ((byte) ((byte) ((byte) 0x2F))))))) == ((byte) 0x28))) ? ((byte) 0x40) : throw new NotImplementedException())))))));
			switch(size) {
				case (byte) ((byte) 0x8): {
					T = "B";
					index = (byte) ((imm) >> (int) ((byte) 0x1));
					break;
				}
				case (byte) ((byte) 0x10): {
					T = "H";
					index = (byte) ((imm) >> (int) ((byte) 0x2));
					break;
				}
				case (byte) ((byte) 0x20): {
					T = "S";
					index = (byte) ((imm) >> (int) ((byte) 0x3));
					break;
				}
				default: {
					T = "D";
					index = (byte) ((imm) >> (int) ((byte) 0x4));
					break;
				}
			}
			return (string) ("umov " + r + (rd).ToString() + ", V" + (rn).ToString() + "." + T + "[" + (index).ToString() + "]");
		}
		insn_335:
		/* UMULH */
		if((insn & 0xFFE0FC00) == 0x9BC07C00) {
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			return (string) ("umulh X" + (rd).ToString() + ", X" + (rn).ToString() + ", X" + (rm).ToString());
		}
		insn_336:
		/* USHL-vector */
		if((insn & 0xBF20FC00) == 0x2E204400) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("ushl V" + (rd).ToString() + "." + t + ", V" + (rn).ToString() + "." + t + ", V" + (rm).ToString() + "." + t);
		}
		insn_337:
		/* USHLL-vector */
		if((insn & 0xBF80FC00) == 0x2F00A400) {
			var Q = (insn >> 30) & 0x1U;
			var immh = (insn >> 19) & 0xFU;
			var immb = (insn >> 16) & 0x7U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var Ta = "";
			var Tb = "";
			var size = (byte) 0x0;
			var shift = (byte) 0x0;
			if(!((bool) (((byte) (immh)) != ((byte) 0x0))))
				goto insn_338;
			var i2 = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("2") : (string) (""));
			if((bool) (((byte) (immh)) == ((byte) 0x1))) {
				Ta = "8H";
				Tb = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"));
				size = (byte) 0x0;
				shift = (byte) (((byte) (byte) ((byte) ((byte) ((byte) ((byte) (((byte) (((byte) (immb)) << 0)) | ((byte) (((byte) (immh)) << 3)))))))) - ((byte) (byte) ((byte) 0x8)));
			} else {
				if((bool) (((byte) ((byte) (((immh) & ((byte) ((byte) ((byte) 0xE))))))) == ((byte) 0x2))) {
					Ta = "4S";
					Tb = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("8H") : (string) ("4H"));
					size = (byte) 0x1;
					shift = (byte) (((byte) (byte) ((byte) ((byte) ((byte) ((byte) (((byte) (((byte) (immb)) << 0)) | ((byte) (((byte) (immh)) << 3)))))))) - ((byte) (byte) ((byte) 0x10)));
				} else {
					if((bool) (((byte) ((byte) (((immh) & ((byte) ((byte) ((byte) 0xC))))))) == ((byte) 0x4))) {
						Ta = "2D";
						Tb = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("4S") : (string) ("2S"));
						size = (byte) 0x2;
						shift = (byte) (((byte) (byte) ((byte) ((byte) ((byte) ((byte) (((byte) (((byte) (immb)) << 0)) | ((byte) (((byte) (immh)) << 3)))))))) - ((byte) (byte) ((byte) 0x20)));
					} else {
						throw new NotImplementedException();
					}
				}
			}
			return (string) ("ushll" + i2 + " V" + (rd).ToString() + "." + Ta + ", V" + (rn).ToString() + "." + Tb + ", #" + (shift).ToString());
		}
		insn_338:
		/* XTN */
		if((insn & 0xFF3FFC00) == 0x0E212800) {
			var size = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var tb = (string) (size switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "4H", (byte) ((byte) 0x2) => "2S", _ => throw new NotImplementedException() });
			var ta = (string) (size switch { (byte) ((byte) 0x0) => "8H", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x2) => "2D", _ => throw new NotImplementedException() });
			return (string) ("xtn V" + (rd).ToString() + "." + tb + ", V" + (rn).ToString() + "." + ta);
		}
		insn_339:
		/* XTN2 */
		if((insn & 0xFF3FFC00) == 0x4E212800) {
			var size = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var tb = (string) (size switch { (byte) ((byte) 0x0) => "16B", (byte) ((byte) 0x1) => "8H", (byte) ((byte) 0x2) => "4S", _ => throw new NotImplementedException() });
			var ta = (string) (size switch { (byte) ((byte) 0x0) => "8H", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x2) => "2D", _ => throw new NotImplementedException() });
			return (string) ("xtn2 V" + (rd).ToString() + "." + tb + ", V" + (rn).ToString() + "." + ta);
		}
		insn_340:
		/* YIELD */
		if((insn & 0xFFFFFFFF) == 0xD503203F) {
			return "yield";
		}
		insn_341:
		/* ZIP */
		if((insn & 0xBF20BC00) == 0x0E003800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var op = (insn >> 14) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var i = (byte) ((byte) (((byte) (byte) (op)) + ((byte) (byte) ((byte) 0x1))));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return (string) ("zip" + (i).ToString() + " V" + (rd).ToString() + "." + T + ", V" + (rn).ToString() + "." + T + ", V" + (rm).ToString() + "." + T);
		}
		insn_342:

        return null;
    }

    public static string ClassifyInstruction(uint insn) {
        var pc = 0xDEADBEEFCAFEBABEUL; // Should never be actually used
		if((insn & 0x7FE0FC00) == 0x3A000000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "ADCS";
		}
		insn_1:
		if((insn & 0x7FE00000) == 0x0B200000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var imm = (insn >> 10) & 0x7U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) 0x4))))
				goto insn_2;
			var r1 = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var r2 = (string) (((bool) (((byte) ((byte) (((option) & ((byte) ((byte) ((byte) 0x3))))))) == ((byte) 0x3))) ? (string) ("X") : (string) ("W"));
			var extend = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ((string) (option switch { (byte) ((byte) 0x0) => "UXTB", (byte) ((byte) 0x1) => "UXTH", (byte) ((byte) 0x2) => "LSL", (byte) ((byte) 0x3) => "UXTX", (byte) ((byte) 0x4) => "SXTB", (byte) ((byte) 0x5) => "SXTH", (byte) ((byte) 0x6) => "SXTW", _ => "SXTX" })) : (string) ((string) (option switch { (byte) ((byte) 0x0) => "UXTB", (byte) ((byte) 0x1) => "UXTH", (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x4) => "SXTB", (byte) ((byte) 0x5) => "SXTH", (byte) ((byte) 0x6) => "SXTW", _ => "SXTX" })));
			return "ADD-extended-register";
		}
		insn_2:
		if((insn & 0x7F800000) == 0x11000000) {
			var size = (insn >> 31) & 0x1U;
			var sh = (insn >> 22) & 0x1U;
			var imm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shift = (byte) (((bool) (((byte) (sh)) == ((byte) 0x0))) ? (byte) ((byte) 0x0) : (byte) ((byte) 0xC));
			var simm = (uint) (((uint) ((uint) (imm))) << (int) (shift));
			return "ADD-immediate";
		}
		insn_3:
		if((insn & 0x7F200000) == 0x0B000000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_4;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return "ADD-shifted-register";
		}
		insn_4:
		if((insn & 0xBF20FC00) == 0x0E208400) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var ts = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ADD-vector";
		}
		insn_5:
		if((insn & 0xBF20FC00) == 0x0E20BC00) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var ts = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ADDP-vector";
		}
		insn_6:
		if((insn & 0x7FE00000) == 0x2B200000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var imm = (insn >> 10) & 0x7U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) 0x4))))
				goto insn_7;
			var r1 = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var r2 = (string) (((bool) (((byte) ((byte) (((option) & ((byte) ((byte) ((byte) 0x3))))))) == ((byte) 0x3))) ? (string) ("X") : (string) ("W"));
			var extend = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ((string) (option switch { (byte) ((byte) 0x0) => "UXTB", (byte) ((byte) 0x1) => "UXTH", (byte) ((byte) 0x2) => "LSL", (byte) ((byte) 0x3) => "UXTX", (byte) ((byte) 0x4) => "SXTB", (byte) ((byte) 0x5) => "SXTH", (byte) ((byte) 0x6) => "SXTW", _ => "SXTX" })) : (string) ((string) (option switch { (byte) ((byte) 0x0) => "UXTB", (byte) ((byte) 0x1) => "UXTH", (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x4) => "SXTB", (byte) ((byte) 0x5) => "SXTH", (byte) ((byte) 0x6) => "SXTW", _ => "SXTX" })));
			return "ADDS-extended-register";
		}
		insn_7:
		if((insn & 0x7F800000) == 0x31000000) {
			var size = (insn >> 31) & 0x1U;
			var sh = (insn >> 22) & 0x1U;
			var imm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shift = (byte) (((bool) (((byte) (sh)) == ((byte) 0x0))) ? (byte) ((byte) 0x0) : (byte) ((byte) 0xC));
			var simm = (uint) (((uint) ((uint) (imm))) << (int) (shift));
			return "ADDS-immediate";
		}
		insn_8:
		if((insn & 0x7F200000) == 0x2B000000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_9;
			if(!((bool) (((byte) (shift)) != ((byte) 0x3))))
				goto insn_9;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return "ADDS-shifted-register";
		}
		insn_9:
		if((insn & 0x9F000000) == 0x10000000) {
			var immlo = (insn >> 29) & 0x3U;
			var immhi = (insn >> 5) & 0x7FFFFU;
			var rd = (insn >> 0) & 0x1FU;
			var imm = (long) (Math.SignExt<long>((uint) ((uint) (((uint) (((uint) (immlo)) << 0)) | ((uint) (((uint) (immhi)) << 2)))), 21));
			var addr = (ulong) (((ulong) (ulong) ((ulong) (pc))) + ((ulong) (long) (imm)));
			return "ADR";
		}
		insn_10:
		if((insn & 0x9F000000) == 0x90000000) {
			var immlo = (insn >> 29) & 0x3U;
			var immhi = (insn >> 5) & 0x7FFFFU;
			var rd = (insn >> 0) & 0x1FU;
			var imm = (long) (Math.SignExt<long>((ulong) ((ulong) (((ulong) (ulong) (((ulong) (((ulong) ((ushort) ((ushort) ((byte) 0x0)))) << 0)) | ((ulong) (((ulong) (immlo)) << 12)))) | ((ulong) (((ulong) (immhi)) << 14)))), 33));
			var addr = (ulong) (((ulong) (ulong) ((ulong) ((ulong) (((ulong) (((ulong) ((ushort) ((ushort) ((byte) 0x0)))) << 0)) | ((ulong) (((ulong) ((ulong) ((ulong) ((ulong) (((ulong) (pc)) >> (int) ((byte) 0xC)))))) << 12)))))) + ((ulong) (long) (imm)));
			return "ADRP";
		}
		insn_11:
		if((insn & 0x7F800000) == 0x12000000) {
			var size = (insn >> 31) & 0x1U;
			var up = (insn >> 22) & 0x1U;
			var immr = (insn >> 16) & 0x3FU;
			var imms = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var imm = (ulong) (MakeWMask(up, imms, immr, (byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x20) : (byte) ((byte) 0x40)), (byte) 0x1));
			return "AND-immediate";
		}
		insn_12:
		if((insn & 0x7F200000) == 0x0A000000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_13;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return "AND-shifted-register";
		}
		insn_13:
		if((insn & 0xBFE0FC00) == 0x0E201C00) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var ts = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"));
			return "AND-vector";
		}
		insn_14:
		if((insn & 0x7F200000) == 0x6A000000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_15;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return "ANDS-shifted-register";
		}
		insn_15:
		if((insn & 0x7F800000) == 0x72000000) {
			var size = (insn >> 31) & 0x1U;
			var up = (insn >> 22) & 0x1U;
			var immr = (insn >> 16) & 0x3FU;
			var imms = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var imm = (ulong) (MakeWMask(up, imms, immr, (byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x20) : (byte) ((byte) 0x40)), (byte) 0x1));
			return "ANDS-immediate";
		}
		insn_16:
		if((insn & 0x7FE0FC00) == 0x1AC02800) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "ASRV";
		}
		insn_17:
		if((insn & 0xFC000000) == 0x14000000) {
			var imm = (insn >> 0) & 0x3FFFFFFU;
			var addr = (ulong) (((ulong) (ulong) ((ulong) (pc))) + ((ulong) (long) ((long) (Math.SignExt<long>((uint) (((uint) ((uint) (imm))) << (int) ((byte) 0x2)), 28)))));
			return "B";
		}
		insn_18:
		if((insn & 0xFF000010) == 0x54000000) {
			var imm = (insn >> 5) & 0x7FFFFU;
			var cond = (insn >> 0) & 0xFU;
			var addr = (ulong) (((ulong) (ulong) ((ulong) (pc))) + ((ulong) (long) ((long) (Math.SignExt<long>((uint) (((uint) ((uint) (imm))) << (int) ((byte) 0x2)), 21)))));
			var condstr = (string) (cond switch { (byte) ((byte) 0x0) => "EQ", (byte) ((byte) 0x1) => "NE", (byte) ((byte) 0x2) => "CS", (byte) ((byte) 0x3) => "CC", (byte) ((byte) 0x4) => "MI", (byte) ((byte) 0x5) => "PL", (byte) ((byte) 0x6) => "VS", (byte) ((byte) 0x7) => "VC", (byte) ((byte) 0x8) => "HI", (byte) ((byte) 0x9) => "LS", (byte) ((byte) 0xA) => "GE", (byte) ((byte) 0xB) => "LT", (byte) ((byte) 0xC) => "GT", (byte) ((byte) 0xD) => "LE", _ => "AL" });
			return "B.cond";
		}
		insn_19:
		if((insn & 0x7F800000) == 0x33000000) {
			var size = (insn >> 31) & 0x1U;
			var N = (insn >> 22) & 0x1U;
			var immr = (insn >> 16) & 0x3FU;
			var imms = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "BFM";
		}
		insn_20:
		if((insn & 0x7F200000) == 0x0A200000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_21;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return "BIC";
		}
		insn_21:
		if((insn & 0xBFE0FC00) == 0x0E601C00) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) (((bool) (((byte) (Q)) == ((byte) 0x1))) ? (string) ("16B") : (string) ("8B"));
			return "BIC-vector-register";
		}
		insn_22:
		if((insn & 0xBFF8DC00) == 0x2F009400) {
			var Q = (insn >> 30) & 0x1U;
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var cmode = (insn >> 13) & 0x1U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) (((bool) (((byte) (Q)) == ((byte) 0x1))) ? (string) ("16B") : (string) ("8B"));
			var amount = (byte) (((bool) ((cmode) != ((byte) 0x0))) ? (byte) ((byte) 0x8) : (byte) ((byte) 0x0));
			var imm = (byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (h)) << 0)) | ((byte) (((byte) (g)) << 1)))) | ((byte) (((byte) (f)) << 2)))) | ((byte) (((byte) (e)) << 3)))) | ((byte) (((byte) (d)) << 4)))) | ((byte) (((byte) (c)) << 5)))) | ((byte) (((byte) (b)) << 6)))) | ((byte) (((byte) (a)) << 7))));
			return "BIC-vector-immediate-16bit";
		}
		insn_23:
		if((insn & 0xBFF89C00) == 0x2F001400) {
			var Q = (insn >> 30) & 0x1U;
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var cmode = (insn >> 13) & 0x3U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) (((bool) (((byte) (Q)) == ((byte) 0x1))) ? (string) ("16B") : (string) ("8B"));
			var amount = (byte) ((cmode) << (int) ((byte) 0x3));
			var imm = (byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (h)) << 0)) | ((byte) (((byte) (g)) << 1)))) | ((byte) (((byte) (f)) << 2)))) | ((byte) (((byte) (e)) << 3)))) | ((byte) (((byte) (d)) << 4)))) | ((byte) (((byte) (c)) << 5)))) | ((byte) (((byte) (b)) << 6)))) | ((byte) (((byte) (a)) << 7))));
			return "BIC-vector-immediate-32bit";
		}
		insn_24:
		if((insn & 0x7F200000) == 0x6A200000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_25;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return "BICS";
		}
		insn_25:
		if((insn & 0xBFE0FC00) == 0x2EE01C00) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"));
			return "BIF";
		}
		insn_26:
		if((insn & 0xBFE0FC00) == 0x2EA01C00) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"));
			return "BIT";
		}
		insn_27:
		if((insn & 0xFC000000) == 0x94000000) {
			var imm = (insn >> 0) & 0x3FFFFFFU;
			var offset = (long) (Math.SignExt<long>((uint) (((uint) ((uint) (imm))) << (int) ((byte) 0x2)), 28));
			var addr = (ulong) (((ulong) (ulong) ((ulong) (pc))) + ((ulong) (long) (offset)));
			return "BL";
		}
		insn_28:
		if((insn & 0xFFFFFC1F) == 0xD63F0000) {
			var rn = (insn >> 5) & 0x1FU;
			return "BLR";
		}
		insn_29:
		if((insn & 0xFFFFFC1F) == 0xD61F0000) {
			var rn = (insn >> 5) & 0x1FU;
			return "BR";
		}
		insn_30:
		if((insn & 0xFFE0001F) == 0xD4200000) {
			var imm = (insn >> 5) & 0xFFFFU;
			return "BRK";
		}
		insn_31:
		if((insn & 0xBFE0FC00) == 0x2E601C00) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"));
			return "BSL";
		}
		insn_32:
		if((insn & 0xBFE0FC00) == 0x08207C00) {
			var size = (insn >> 30) & 0x1U;
			var rs = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var rs2 = (byte) (((byte) (byte) (rs)) + ((byte) (byte) ((byte) 0x1)));
			var rt2 = (byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1)));
			return "CASP";
		}
		insn_33:
		if((insn & 0xBFE0FC00) == 0x08607C00) {
			var size = (insn >> 30) & 0x1U;
			var rs = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var rs2 = (byte) (((byte) (byte) (rs)) + ((byte) (byte) ((byte) 0x1)));
			var rt2 = (byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1)));
			return "CASPA";
		}
		insn_34:
		if((insn & 0xBFE0FC00) == 0x0860FC00) {
			var size = (insn >> 30) & 0x1U;
			var rs = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var rs2 = (byte) (((byte) (byte) (rs)) + ((byte) (byte) ((byte) 0x1)));
			var rt2 = (byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1)));
			return "CASPAL";
		}
		insn_35:
		if((insn & 0xBFE0FC00) == 0x0820FC00) {
			var size = (insn >> 30) & 0x1U;
			var rs = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var rs2 = (byte) (((byte) (byte) (rs)) + ((byte) (byte) ((byte) 0x1)));
			var rt2 = (byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1)));
			return "CASPL";
		}
		insn_36:
		if((insn & 0x7F000000) == 0x35000000) {
			var size = (insn >> 31) & 0x1U;
			var imm = (insn >> 5) & 0x7FFFFU;
			var rs = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var addr = (ulong) (((ulong) (ulong) ((ulong) (pc))) + ((ulong) (long) ((long) (Math.SignExt<long>((uint) ((uint) ((uint) ((imm) << (int) ((byte) 0x2)))), 21)))));
			return "CBNZ";
		}
		insn_37:
		if((insn & 0x7F000000) == 0x34000000) {
			var size = (insn >> 31) & 0x1U;
			var imm = (insn >> 5) & 0x7FFFFU;
			var rs = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var addr = (ulong) (((ulong) (ulong) ((ulong) (pc))) + ((ulong) (long) ((long) (Math.SignExt<long>((uint) ((uint) ((uint) ((imm) << (int) ((byte) 0x2)))), 21)))));
			return "CBZ";
		}
		insn_38:
		if((insn & 0x7FE00C10) == 0x3A400800) {
			var size = (insn >> 31) & 0x1U;
			var imm = (insn >> 16) & 0x1FU;
			var cond = (insn >> 12) & 0xFU;
			var rn = (insn >> 5) & 0x1FU;
			var nzcv = (insn >> 0) & 0xFU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var condstr = (string) (cond switch { (byte) ((byte) 0x0) => "EQ", (byte) ((byte) 0x1) => "NE", (byte) ((byte) 0x2) => "CS", (byte) ((byte) 0x3) => "CC", (byte) ((byte) 0x4) => "MI", (byte) ((byte) 0x5) => "PL", (byte) ((byte) 0x6) => "VS", (byte) ((byte) 0x7) => "VC", (byte) ((byte) 0x8) => "HI", (byte) ((byte) 0x9) => "LS", (byte) ((byte) 0xA) => "GE", (byte) ((byte) 0xB) => "LT", (byte) ((byte) 0xC) => "GT", (byte) ((byte) 0xD) => "LE", _ => "AL" });
			return "CCMN-immediate";
		}
		insn_39:
		if((insn & 0x7FE00C10) == 0x7A400800) {
			var size = (insn >> 31) & 0x1U;
			var imm = (insn >> 16) & 0x1FU;
			var cond = (insn >> 12) & 0xFU;
			var rn = (insn >> 5) & 0x1FU;
			var nzcv = (insn >> 0) & 0xFU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var condstr = (string) (cond switch { (byte) ((byte) 0x0) => "EQ", (byte) ((byte) 0x1) => "NE", (byte) ((byte) 0x2) => "CS", (byte) ((byte) 0x3) => "CC", (byte) ((byte) 0x4) => "MI", (byte) ((byte) 0x5) => "PL", (byte) ((byte) 0x6) => "VS", (byte) ((byte) 0x7) => "VC", (byte) ((byte) 0x8) => "HI", (byte) ((byte) 0x9) => "LS", (byte) ((byte) 0xA) => "GE", (byte) ((byte) 0xB) => "LT", (byte) ((byte) 0xC) => "GT", (byte) ((byte) 0xD) => "LE", _ => "AL" });
			return "CCMP-immediate";
		}
		insn_40:
		if((insn & 0x7FE00C10) == 0x7A400000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var cond = (insn >> 12) & 0xFU;
			var rn = (insn >> 5) & 0x1FU;
			var nzcv = (insn >> 0) & 0xFU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var condstr = (string) (cond switch { (byte) ((byte) 0x0) => "EQ", (byte) ((byte) 0x1) => "NE", (byte) ((byte) 0x2) => "CS", (byte) ((byte) 0x3) => "CC", (byte) ((byte) 0x4) => "MI", (byte) ((byte) 0x5) => "PL", (byte) ((byte) 0x6) => "VS", (byte) ((byte) 0x7) => "VC", (byte) ((byte) 0x8) => "HI", (byte) ((byte) 0x9) => "LS", (byte) ((byte) 0xA) => "GE", (byte) ((byte) 0xB) => "LT", (byte) ((byte) 0xC) => "GT", (byte) ((byte) 0xD) => "LE", _ => "AL" });
			return "CCMP-register";
		}
		insn_41:
		if((insn & 0xFFFFF0FF) == 0xD503305F) {
			var crm = (insn >> 8) & 0xFU;
			return "CLREX";
		}
		insn_42:
		if((insn & 0x7FFFFC00) == 0x5AC01000) {
			var size = (insn >> 31) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "CLZ";
		}
		insn_43:
		if((insn & 0xFF20FC00) == 0x7E208C00) {
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var V = (string) (size switch { (byte) ((byte) 0x3) => "D", _ => throw new NotImplementedException() });
			return "CMEQ-register-scalar";
		}
		insn_44:
		if((insn & 0xBF20FC00) == 0x2E208C00) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "CMEQ-register-vector";
		}
		insn_45:
		if((insn & 0xFF3FFC00) == 0x5E209800) {
			var size = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var V = (string) (size switch { (byte) ((byte) 0x3) => "D", _ => throw new NotImplementedException() });
			return "CMEQ-zero-scalar";
		}
		insn_46:
		if((insn & 0xBF3FFC00) == 0x0E209800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "CMEQ-zero-vector";
		}
		insn_47:
		if((insn & 0xFF20FC00) == 0x5E203400) {
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var V = (string) (size switch { (byte) ((byte) 0x3) => "D", _ => throw new NotImplementedException() });
			return "CMGT-register-scalar";
		}
		insn_48:
		if((insn & 0xBF20FC00) == 0x0E203400) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "CMGT-register-vector";
		}
		insn_49:
		if((insn & 0xFF3FFC00) == 0x5E208800) {
			var size = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var V = (string) (size switch { (byte) ((byte) 0x3) => "D", _ => throw new NotImplementedException() });
			return "CMGT-zero-scalar";
		}
		insn_50:
		if((insn & 0xBF3FFC00) == 0x0E208800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "CMGT-zero-vector";
		}
		insn_51:
		if((insn & 0xFF20FC00) == 0x7E203C00) {
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var V = (string) (size switch { (byte) ((byte) 0x3) => "D", _ => throw new NotImplementedException() });
			return "CMHS-register-scalar";
		}
		insn_52:
		if((insn & 0xBF20FC00) == 0x2E203C00) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "CMHS-register-vector";
		}
		insn_53:
		if((insn & 0xBF3FFC00) == 0x0E205800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", _ => throw new NotImplementedException() });
			return "CNT";
		}
		insn_54:
		if((insn & 0x7FE00C00) == 0x1A800000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var cond = (insn >> 12) & 0xFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var condstr = (string) (cond switch { (byte) ((byte) 0x0) => "EQ", (byte) ((byte) 0x1) => "NE", (byte) ((byte) 0x2) => "CS", (byte) ((byte) 0x3) => "CC", (byte) ((byte) 0x4) => "MI", (byte) ((byte) 0x5) => "PL", (byte) ((byte) 0x6) => "VS", (byte) ((byte) 0x7) => "VC", (byte) ((byte) 0x8) => "HI", (byte) ((byte) 0x9) => "LS", (byte) ((byte) 0xA) => "GE", (byte) ((byte) 0xB) => "LT", (byte) ((byte) 0xC) => "GT", (byte) ((byte) 0xD) => "LE", _ => "AL" });
			return "CSEL";
		}
		insn_55:
		if((insn & 0x7FE00C00) == 0x1A800400) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var cond = (insn >> 12) & 0xFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var condstr = (string) (cond switch { (byte) ((byte) 0x0) => "EQ", (byte) ((byte) 0x1) => "NE", (byte) ((byte) 0x2) => "CS", (byte) ((byte) 0x3) => "CC", (byte) ((byte) 0x4) => "MI", (byte) ((byte) 0x5) => "PL", (byte) ((byte) 0x6) => "VS", (byte) ((byte) 0x7) => "VC", (byte) ((byte) 0x8) => "HI", (byte) ((byte) 0x9) => "LS", (byte) ((byte) 0xA) => "GE", (byte) ((byte) 0xB) => "LT", (byte) ((byte) 0xC) => "GT", (byte) ((byte) 0xD) => "LE", _ => "AL" });
			return "CSINC";
		}
		insn_56:
		if((insn & 0x7FE00C00) == 0x5A800000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var cond = (insn >> 12) & 0xFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var condstr = (string) (cond switch { (byte) ((byte) 0x0) => "EQ", (byte) ((byte) 0x1) => "NE", (byte) ((byte) 0x2) => "CS", (byte) ((byte) 0x3) => "CC", (byte) ((byte) 0x4) => "MI", (byte) ((byte) 0x5) => "PL", (byte) ((byte) 0x6) => "VS", (byte) ((byte) 0x7) => "VC", (byte) ((byte) 0x8) => "HI", (byte) ((byte) 0x9) => "LS", (byte) ((byte) 0xA) => "GE", (byte) ((byte) 0xB) => "LT", (byte) ((byte) 0xC) => "GT", (byte) ((byte) 0xD) => "LE", _ => "AL" });
			return "CSINV";
		}
		insn_57:
		if((insn & 0x7FE00C00) == 0x5A800400) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var cond = (insn >> 12) & 0xFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var condstr = (string) (cond switch { (byte) ((byte) 0x0) => "EQ", (byte) ((byte) 0x1) => "NE", (byte) ((byte) 0x2) => "CS", (byte) ((byte) 0x3) => "CC", (byte) ((byte) 0x4) => "MI", (byte) ((byte) 0x5) => "PL", (byte) ((byte) 0x6) => "VS", (byte) ((byte) 0x7) => "VC", (byte) ((byte) 0x8) => "HI", (byte) ((byte) 0x9) => "LS", (byte) ((byte) 0xA) => "GE", (byte) ((byte) 0xB) => "LT", (byte) ((byte) 0xC) => "GT", (byte) ((byte) 0xD) => "LE", _ => "AL" });
			return "CSNEG";
		}
		insn_58:
		if((insn & 0xFFFFF0FF) == 0xD50330BF) {
			var m = (insn >> 8) & 0xFU;
			var option = (string) (m switch { (byte) ((byte) 0xF) => "SY", (byte) ((byte) 0xE) => "ST", (byte) ((byte) 0xD) => "LD", (byte) ((byte) 0xB) => "ISH", (byte) ((byte) 0xA) => "ISHST", (byte) ((byte) 0x9) => "ISHLD", (byte) ((byte) 0x7) => "NSH", (byte) ((byte) 0x6) => "NSHST", (byte) ((byte) 0x5) => "NSHLD", (byte) ((byte) 0x3) => "OSH", (byte) ((byte) 0x2) => "OSHST", _ => "OSHLD" });
			return "DMB";
		}
		insn_59:
		if((insn & 0xFFFFF0FF) == 0xD503309F) {
			var crm = (insn >> 8) & 0xFU;
			var option = (string) (crm switch { (byte) ((byte) 0xF) => "SY", (byte) ((byte) 0xE) => "ST", (byte) ((byte) 0xD) => "LD", (byte) ((byte) 0xB) => "ISH", (byte) ((byte) 0xA) => "ISHST", (byte) ((byte) 0x9) => "ISHLD", (byte) ((byte) 0x7) => "NSH", (byte) ((byte) 0x6) => "NSHST", (byte) ((byte) 0x5) => "NSHLD", (byte) ((byte) 0x3) => "OSH", (byte) ((byte) 0x2) => "OSHST", _ => "OSHLD" });
			return "DSB";
		}
		insn_60:
		if((insn & 0xFFE0FC00) == 0x5E000400) {
			var imm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = "";
			var index = (byte) 0x0;
			var size = (byte) 0x0;
			if((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0xF))))))) == ((byte) 0x0))) {
				throw new NotImplementedException();
			} else {
				if((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x1))))))) == ((byte) 0x1))) {
					T = "B";
					index = (byte) ((imm) >> (int) ((byte) 0x1));
					size = (byte) 0x1;
				} else {
					if((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x3))))))) == ((byte) 0x2))) {
						T = "H";
						index = (byte) ((imm) >> (int) ((byte) 0x2));
						size = (byte) 0x2;
					} else {
						if((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x7))))))) == ((byte) 0x4))) {
							T = "S";
							index = (byte) ((imm) >> (int) ((byte) 0x3));
							size = (byte) 0x4;
						} else {
							T = "D";
							index = (byte) ((imm) >> (int) ((byte) 0x4));
							size = (byte) 0x8;
						}
					}
				}
			}
			return "DUP-element-scalar";
		}
		insn_61:
		if((insn & 0xBFE0FC00) == 0x0E000400) {
			var Q = (insn >> 30) & 0x1U;
			var imm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var Ts = "";
			var T = "";
			var index = (byte) 0x0;
			var size = (byte) 0x0;
			if((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0xF))))))) == ((byte) 0x0))) {
				throw new NotImplementedException();
			} else {
				if((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x1))))))) == ((byte) 0x1))) {
					Ts = "B";
					T = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"));
					index = (byte) ((imm) >> (int) ((byte) 0x1));
					size = (byte) 0x1;
				} else {
					if((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x3))))))) == ((byte) 0x2))) {
						Ts = "H";
						T = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("8H") : (string) ("4H"));
						index = (byte) ((imm) >> (int) ((byte) 0x2));
						size = (byte) 0x2;
					} else {
						if((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x7))))))) == ((byte) 0x4))) {
							Ts = "S";
							T = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("4S") : (string) ("2S"));
							index = (byte) ((imm) >> (int) ((byte) 0x3));
							size = (byte) 0x4;
						} else {
							Ts = "D";
							T = (string) (((bool) ((Q) != ((byte) 0x0))) ? ("2D") : throw new NotImplementedException());
							index = (byte) ((imm) >> (int) ((byte) 0x4));
							size = (byte) 0x8;
						}
					}
				}
			}
			return "DUP-element-vector";
		}
		insn_62:
		if((insn & 0xBFE0FC00) == 0x0E000C00) {
			var Q = (insn >> 30) & 0x1U;
			var imm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var size = ((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0xF))))))) == ((byte) 0x0))) ? throw new NotImplementedException() : ((byte) (((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0xF))))))) == ((byte) 0x8))) ? (byte) ((byte) 0x40) : (byte) ((byte) 0x20)));
			var r = (string) (((bool) ((size) == ((byte) 0x40))) ? (string) ("X") : (string) ("W"));
			var T = ((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0xF))))))) == ((byte) 0x0))) ? throw new NotImplementedException() : ((string) (((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x1))))))) == ((byte) 0x1))) ? (string) ((string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"))) : (string) ((string) (((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x3))))))) == ((byte) 0x2))) ? (string) ((string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("8H") : (string) ("4H"))) : (string) ((string) (((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x7))))))) == ((byte) 0x4))) ? (string) ((string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("4S") : (string) ("2S"))) : (string) ((string) (((bool) ((Q) != ((byte) 0x0))) ? ("2D") : throw new NotImplementedException()))))))));
			return "DUP-general";
		}
		insn_63:
		if((insn & 0x7F200000) == 0x4A200000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_64;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return "EON-shifted-register";
		}
		insn_64:
		if((insn & 0x7F800000) == 0x52000000) {
			var size = (insn >> 31) & 0x1U;
			var up = (insn >> 22) & 0x1U;
			var immr = (insn >> 16) & 0x3FU;
			var imms = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var imm = (ulong) (MakeWMask(up, imms, immr, (byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x20) : (byte) ((byte) 0x40)), (byte) 0x1));
			return "EOR-immediate";
		}
		insn_65:
		if((insn & 0x7F200000) == 0x4A000000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_66;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return "EOR-shifted-register";
		}
		insn_66:
		if((insn & 0xBFE0FC00) == 0x2E201C00) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"));
			return "EOR-vector";
		}
		insn_67:
		if((insn & 0xBFE08400) == 0x2E000000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var index = (insn >> 11) & 0xFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var ts = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"));
			return "EXT";
		}
		insn_68:
		if((insn & 0x7FA00000) == 0x13800000) {
			var size = (insn >> 31) & 0x1U;
			var o = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var lsb = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (lsb)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_69;
			if(!((bool) ((size) == (o))))
				goto insn_69;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "EXTR";
		}
		insn_69:
		if((insn & 0xFFA0FC00) == 0x7EA0D400) {
			var size = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) ((size) != ((byte) 0x0))) ? (string) ("D") : (string) ("S"));
			return "FABD-scalar";
		}
		insn_70:
		if((insn & 0xFF3FFC00) == 0x1E20C000) {
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return "FABS-scalar";
		}
		insn_71:
		if((insn & 0xBFBFFC00) == 0x0EA0F800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return "FABS-vector";
		}
		insn_72:
		if((insn & 0xFF20FC00) == 0x1E202800) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return "FADD-scalar";
		}
		insn_73:
		if((insn & 0xBFA0FC00) == 0x0E20D400) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var ts = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return "FADD-vector";
		}
		insn_74:
		if((insn & 0xFFBFFC00) == 0x7E30D800) {
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("S") : (string) ("D"));
			return "FADDP-scalar";
		}
		insn_75:
		if((insn & 0xBFA0FC00) == 0x2E20D400) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return "FADDP-vector";
		}
		insn_76:
		if((insn & 0xFF200C10) == 0x1E200400) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var cond = (insn >> 12) & 0xFU;
			var rn = (insn >> 5) & 0x1FU;
			var nzcv = (insn >> 0) & 0xFU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			var condstr = (string) (cond switch { (byte) ((byte) 0x0) => "EQ", (byte) ((byte) 0x1) => "NE", (byte) ((byte) 0x2) => "CS", (byte) ((byte) 0x3) => "CC", (byte) ((byte) 0x4) => "MI", (byte) ((byte) 0x5) => "PL", (byte) ((byte) 0x6) => "VS", (byte) ((byte) 0x7) => "VC", (byte) ((byte) 0x8) => "HI", (byte) ((byte) 0x9) => "LS", (byte) ((byte) 0xA) => "GE", (byte) ((byte) 0xB) => "LT", (byte) ((byte) 0xC) => "GT", (byte) ((byte) 0xD) => "LE", _ => "AL" });
			return "FCCMP";
		}
		insn_77:
		if((insn & 0x9F20F400) == 0x0E20E400) {
			var Q = (insn >> 30) & 0x1U;
			var U = (insn >> 29) & 0x1U;
			var E = (insn >> 23) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var ac = (insn >> 11) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var top = (string) ((byte) ((byte) (((byte) (byte) (((byte) (((byte) (ac)) << 0)) | ((byte) (((byte) (U)) << 1)))) | ((byte) (((byte) (E)) << 2)))) switch { (byte) ((byte) 0x0) => "EQ", (byte) ((byte) 0x2) => "GE", (byte) ((byte) 0x3) => "GE", (byte) ((byte) 0x6) => "GT", (byte) ((byte) 0x7) => "GT", _ => throw new NotImplementedException() });
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return "FCMxx-register-vector";
		}
		insn_78:
		if((insn & 0x9FBFEC00) == 0x0EA0C800) {
			var Q = (insn >> 30) & 0x1U;
			var U = (insn >> 29) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var op = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var top = (string) ((byte) ((byte) (((byte) (((byte) (U)) << 0)) | ((byte) (((byte) (op)) << 1)))) switch { (byte) ((byte) 0x0) => "GT", (byte) ((byte) 0x1) => "GE", (byte) ((byte) 0x2) => "EQ", _ => "LE" });
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return "FCMxx-zero-vector";
		}
		insn_79:
		if((insn & 0xBFBFFC00) == 0x0EA0E800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return "FCMLT-zero-vector";
		}
		insn_80:
		if((insn & 0xFF20FC17) == 0x1E202000) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var opc = (insn >> 3) & 0x1U;
			var r = (string) (type switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			var zero = (string) (((bool) (((byte) (opc)) == ((byte) 0x1))) ? (string) ("/0") : (string) (""));
			return "FCMP";
		}
		insn_81:
		if((insn & 0xFF20FC17) == 0x1E202010) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var opc = (insn >> 3) & 0x1U;
			var r = (string) (type switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			var zero = (string) (((bool) (((byte) (opc)) == ((byte) 0x1))) ? (string) ("/0") : (string) (""));
			return "FCMPE";
		}
		insn_82:
		if((insn & 0xFF200C00) == 0x1E200C00) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var cond = (insn >> 12) & 0xFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			var condstr = (string) (cond switch { (byte) ((byte) 0x0) => "EQ", (byte) ((byte) 0x1) => "NE", (byte) ((byte) 0x2) => "CS", (byte) ((byte) 0x3) => "CC", (byte) ((byte) 0x4) => "MI", (byte) ((byte) 0x5) => "PL", (byte) ((byte) 0x6) => "VS", (byte) ((byte) 0x7) => "VC", (byte) ((byte) 0x8) => "HI", (byte) ((byte) 0x9) => "LS", (byte) ((byte) 0xA) => "GE", (byte) ((byte) 0xB) => "LT", (byte) ((byte) 0xC) => "GT", (byte) ((byte) 0xD) => "LE", _ => "AL" });
			return "FCSEL";
		}
		insn_83:
		if((insn & 0xFF3E7C00) == 0x1E224000) {
			var type = (insn >> 22) & 0x3U;
			var opc = (insn >> 15) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r1 = "";
			var r2 = "";
			var tf = (byte) ((byte) (((byte) (((byte) (opc)) << 0)) | ((byte) (((byte) (type)) << 2))));
			switch(tf) {
				case (byte) ((byte) 0xC): {
					r1 = "S";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0xD): {
					r1 = "D";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x3): {
					r1 = "H";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x1): {
					r1 = "D";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "H";
					r2 = "D";
					break;
				}
				case (byte) ((byte) 0x4): {
					r1 = "S";
					r2 = "D";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return "FCVT";
		}
		insn_84:
		if((insn & 0x7F3FFC00) == 0x1E240000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var st = (byte) ((byte) (((byte) (((byte) (type)) << 0)) | ((byte) (((byte) (size)) << 2))));
			var r1 = "";
			var r2 = "";
			switch(st) {
				case (byte) ((byte) 0x3): {
					r1 = "W";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "X";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x0): {
					r1 = "W";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x4): {
					r1 = "X";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x1): {
					r1 = "W";
					r2 = "D";
					break;
				}
				case (byte) ((byte) 0x5): {
					r1 = "X";
					r2 = "D";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return "FCVTAS-scalar-integer";
		}
		insn_85:
		if((insn & 0x7F3FFC00) == 0x1E250000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var st = (byte) ((byte) (((byte) (((byte) (type)) << 0)) | ((byte) (((byte) (size)) << 2))));
			var r1 = "";
			var r2 = "";
			switch(st) {
				case (byte) ((byte) 0x3): {
					r1 = "W";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "X";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x0): {
					r1 = "W";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x4): {
					r1 = "X";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x1): {
					r1 = "W";
					r2 = "D";
					break;
				}
				case (byte) ((byte) 0x5): {
					r1 = "X";
					r2 = "D";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return "FCVTAU-scalar-integer";
		}
		insn_86:
		if((insn & 0xBFBFFC00) == 0x0E217800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var o2 = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("2") : (string) (""));
			var ta = (string) (((bool) ((size) != ((byte) 0x0))) ? (string) ("2D") : (string) ("4S"));
			var tb = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "4H", (byte) ((byte) 0x1) => "8H", (byte) ((byte) 0x2) => "2S", _ => "4S" });
			return "FCVTL[2]";
		}
		insn_87:
		if((insn & 0x7F3FFC00) == 0x1E300000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var st = (byte) ((byte) (((byte) (((byte) (type)) << 0)) | ((byte) (((byte) (size)) << 2))));
			var r1 = "";
			var r2 = "";
			switch(st) {
				case (byte) ((byte) 0x3): {
					r1 = "W";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "X";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x0): {
					r1 = "W";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x4): {
					r1 = "X";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x1): {
					r1 = "W";
					r2 = "D";
					break;
				}
				case (byte) ((byte) 0x5): {
					r1 = "X";
					r2 = "D";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return "FCVTMS-scalar-integer";
		}
		insn_88:
		if((insn & 0x7F3FFC00) == 0x1E310000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var st = (byte) ((byte) (((byte) (((byte) (type)) << 0)) | ((byte) (((byte) (size)) << 2))));
			var r1 = "";
			var r2 = "";
			switch(st) {
				case (byte) ((byte) 0x3): {
					r1 = "W";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "X";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x0): {
					r1 = "W";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x4): {
					r1 = "X";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x1): {
					r1 = "W";
					r2 = "D";
					break;
				}
				case (byte) ((byte) 0x5): {
					r1 = "X";
					r2 = "D";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return "FCVTMU-scalar-integer";
		}
		insn_89:
		if((insn & 0xFFBFFC00) == 0x0E216800) {
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var Ta = "";
			var Tb = "";
			switch(size) {
				case (byte) ((byte) 0x0): {
					Ta = "4S";
					Tb = "4H";
					break;
				}
				case (byte) ((byte) 0x1): {
					Ta = "2D";
					Tb = "2S";
					break;
				}
			}
			return "FCVTN";
		}
		insn_90:
		if((insn & 0xFFBFFC00) == 0x4E216800) {
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var Ta = "";
			var Tb = "";
			switch(size) {
				case (byte) ((byte) 0x0): {
					Ta = "4S";
					Tb = "8H";
					break;
				}
				case (byte) ((byte) 0x1): {
					Ta = "2D";
					Tb = "4S";
					break;
				}
			}
			return "FCVTN2";
		}
		insn_91:
		if((insn & 0x7F3FFC00) == 0x1E280000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var st = (byte) ((byte) (((byte) (((byte) (type)) << 0)) | ((byte) (((byte) (size)) << 2))));
			var r1 = "";
			var r2 = "";
			switch(st) {
				case (byte) ((byte) 0x3): {
					r1 = "W";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "X";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x0): {
					r1 = "W";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x4): {
					r1 = "X";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x1): {
					r1 = "W";
					r2 = "D";
					break;
				}
				case (byte) ((byte) 0x5): {
					r1 = "X";
					r2 = "D";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return "FCVTPS-scalar-integer";
		}
		insn_92:
		if((insn & 0x7F3FFC00) == 0x1E290000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var st = (byte) ((byte) (((byte) (((byte) (type)) << 0)) | ((byte) (((byte) (size)) << 2))));
			var r1 = "";
			var r2 = "";
			switch(st) {
				case (byte) ((byte) 0x3): {
					r1 = "W";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "X";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x0): {
					r1 = "W";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x4): {
					r1 = "X";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x1): {
					r1 = "W";
					r2 = "D";
					break;
				}
				case (byte) ((byte) 0x5): {
					r1 = "X";
					r2 = "D";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return "FCVTPU-scalar-integer";
		}
		insn_93:
		if((insn & 0xFFBFFC00) == 0x5EA1B800) {
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) ((size) != ((byte) 0x0))) ? (string) ("D") : (string) ("S"));
			return "FCVTZS-scalar";
		}
		insn_94:
		if((insn & 0x7F3F0000) == 0x1E180000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var scale = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var fbits = (byte) (((byte) (byte) ((byte) 0x40)) - ((byte) (byte) (scale)));
			if(!((bool) ((fbits) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_95;
			var r1 = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var r2 = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return "FCVTZS-scalar-fixedpoint";
		}
		insn_95:
		if((insn & 0x7F3FFC00) == 0x1E380000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var st = (byte) ((byte) (((byte) (((byte) (type)) << 0)) | ((byte) (((byte) (size)) << 2))));
			var r1 = "";
			var r2 = "";
			switch(st) {
				case (byte) ((byte) 0x3): {
					r1 = "W";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "X";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x0): {
					r1 = "W";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x4): {
					r1 = "X";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x1): {
					r1 = "W";
					r2 = "D";
					break;
				}
				case (byte) ((byte) 0x5): {
					r1 = "X";
					r2 = "D";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return "FCVTZS-scalar-integer";
		}
		insn_96:
		if((insn & 0x7F3F0000) == 0x1E190000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var scale = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var fbits = (byte) (((byte) (byte) ((byte) 0x40)) - ((byte) (byte) (scale)));
			if(!((bool) ((fbits) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_97;
			var r1 = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var r2 = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return "FCVTZU-scalar-fixedpoint";
		}
		insn_97:
		if((insn & 0x7F3FFC00) == 0x1E390000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var st = (byte) ((byte) (((byte) (((byte) (type)) << 0)) | ((byte) (((byte) (size)) << 2))));
			var r1 = "";
			var r2 = "";
			switch(st) {
				case (byte) ((byte) 0x3): {
					r1 = "W";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "X";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x0): {
					r1 = "W";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x4): {
					r1 = "X";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0x1): {
					r1 = "W";
					r2 = "D";
					break;
				}
				case (byte) ((byte) 0x5): {
					r1 = "X";
					r2 = "D";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return "FCVTZU-scalar-integer";
		}
		insn_98:
		if((insn & 0xFF20FC00) == 0x1E201800) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return "FDIV-scalar";
		}
		insn_99:
		if((insn & 0xBFA0FC00) == 0x2E20FC00) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var ts = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return "FDIV-vector";
		}
		insn_100:
		if((insn & 0xFF208000) == 0x1F000000) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var ra = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) (type switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", (byte) ((byte) 0x3) => "H", _ => throw new NotImplementedException() });
			return "FMADD";
		}
		insn_101:
		if((insn & 0xFF20FC00) == 0x1E204800) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return "FMAX-scalar";
		}
		insn_102:
		if((insn & 0xFF20FC00) == 0x1E206800) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return "FMAXNM-scalar";
		}
		insn_103:
		if((insn & 0xFF20FC00) == 0x1E205800) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return "FMIN-scalar";
		}
		insn_104:
		if((insn & 0xFF20FC00) == 0x1E207800) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return "FMINNM-scalar";
		}
		insn_105:
		if((insn & 0xBF80F400) == 0x0F801000) {
			var Q = (insn >> 30) & 0x1U;
			var sz = (insn >> 22) & 0x1U;
			var L = (insn >> 21) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var H = (insn >> 11) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (sz)) << 0)) | ((byte) (((byte) (Q)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x2) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			var Ts = (string) (((bool) ((sz) != ((byte) 0x0))) ? (string) ("D") : (string) ("S"));
			var index = (uint) ((byte) ((byte) (((byte) (((byte) (L)) << 0)) | ((byte) (((byte) (sz)) << 1)))) switch { (byte) ((byte) 0x2) => (uint) ((uint) (H)), (byte) ((byte) 0x3) => throw new NotImplementedException(), _ => (uint) ((uint) ((uint) ((byte) ((byte) (((byte) (((byte) (L)) << 0)) | ((byte) (((byte) (H)) << 1))))))) });
			return "FMLA-by-element-vector-spdp";
		}
		insn_106:
		if((insn & 0xBFA0FC00) == 0x0E20CC00) {
			var Q = (insn >> 30) & 0x1U;
			var sz = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (sz)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return "FMLA-vector";
		}
		insn_107:
		if((insn & 0xBF80F400) == 0x0F805000) {
			var Q = (insn >> 30) & 0x1U;
			var sz = (insn >> 22) & 0x1U;
			var L = (insn >> 21) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var H = (insn >> 11) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (sz)) << 0)) | ((byte) (((byte) (Q)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x2) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			var Ts = (string) (((bool) ((sz) != ((byte) 0x0))) ? (string) ("D") : (string) ("S"));
			var index = (uint) ((byte) ((byte) (((byte) (((byte) (L)) << 0)) | ((byte) (((byte) (sz)) << 1)))) switch { (byte) ((byte) 0x2) => (uint) ((uint) (H)), (byte) ((byte) 0x3) => throw new NotImplementedException(), _ => (uint) ((uint) ((uint) ((byte) ((byte) (((byte) (((byte) (L)) << 0)) | ((byte) (((byte) (H)) << 1))))))) });
			return "FMLS-by-element-vector-spdp";
		}
		insn_108:
		if((insn & 0xBFA0FC00) == 0x0EA0CC00) {
			var Q = (insn >> 30) & 0x1U;
			var sz = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (sz)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return "FMLS-vector";
		}
		insn_109:
		if((insn & 0x7F36FC00) == 0x1E260000) {
			var sf = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var mode = (insn >> 19) & 0x1U;
			var ropc = (insn >> 16) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var opc = (byte) ((byte) (((byte) (((byte) (ropc)) << 0)) | ((byte) (((byte) ((byte) ((byte) ((byte) 0x3)))) << 1))));
			var tf = (byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (opc)) << 0)) | ((byte) (((byte) ((byte) ((byte) (mode)))) << 3)))) | ((byte) (((byte) (type)) << 5)))) | ((byte) (((byte) (sf)) << 7))));
			var r1 = "";
			var r2 = "";
			switch(tf) {
				case (byte) ((byte) 0x66): {
					r1 = "W";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0xE6): {
					r1 = "X";
					r2 = "H";
					break;
				}
				case (byte) ((byte) 0x67): {
					r1 = "H";
					r2 = "W";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "S";
					r2 = "W";
					break;
				}
				case (byte) ((byte) 0x6): {
					r1 = "W";
					r2 = "S";
					break;
				}
				case (byte) ((byte) 0xE7): {
					r1 = "H";
					r2 = "X";
					break;
				}
				case (byte) ((byte) 0xA7): {
					r1 = "D";
					r2 = "X";
					break;
				}
				case (byte) ((byte) 0xCF): {
					r1 = "V";
					r2 = "X";
					break;
				}
				case (byte) ((byte) 0xCE): {
					r1 = "X";
					r2 = "V";
					break;
				}
				case (byte) ((byte) 0xA6): {
					r1 = "X";
					r2 = "D";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			var index1 = (string) (((bool) ((r1) == ("V"))) ? (string) (".D[1]") : (string) (""));
			var index2 = (string) (((bool) ((r2) == ("V"))) ? (string) (".D[1]") : (string) (""));
			return "FMOV-general";
		}
		insn_110:
		if((insn & 0xFF3FFC00) == 0x1E204000) {
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", (byte) ((byte) 0x3) => "H", _ => throw new NotImplementedException() });
			return "FMOV-register";
		}
		insn_111:
		if((insn & 0xFF201FE0) == 0x1E201000) {
			var type = (insn >> 22) & 0x3U;
			var imm = (insn >> 13) & 0xFFU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			var sv = (float) (Math.Bitcast<uint, float>((uint) ((uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (((uint) ((uint) ((uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 0)) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 1)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 2)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 3)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 4)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 5)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 6)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 7)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 8)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 9)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 10)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 11)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 12)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 13)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 14)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 15)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 16)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 17)))) | ((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 18)))))) << 0)) | ((uint) (((uint) ((byte) ((byte) ((byte) (((imm) & ((byte) 0xF))))))) << 19)))) | ((uint) (((uint) ((byte) ((byte) ((byte) ((((byte) ((imm) >> (int) ((byte) 0x4))) & ((byte) 0x3))))))) << 23)))) | ((uint) (((uint) ((byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) ((byte) ((byte) ((byte) ((((byte) ((imm) >> (int) ((byte) 0x6))) & ((byte) 0x1))))))) << 0)) | ((byte) (((byte) ((byte) ((byte) ((byte) ((((byte) ((imm) >> (int) ((byte) 0x6))) & ((byte) 0x1))))))) << 1)))) | ((byte) (((byte) ((byte) ((byte) ((byte) ((((byte) ((imm) >> (int) ((byte) 0x6))) & ((byte) 0x1))))))) << 2)))) | ((byte) (((byte) ((byte) ((byte) ((byte) ((((byte) ((imm) >> (int) ((byte) 0x6))) & ((byte) 0x1))))))) << 3)))) | ((byte) (((byte) ((byte) ((byte) ((byte) ((((byte) ((imm) >> (int) ((byte) 0x6))) & ((byte) 0x1))))))) << 4)))))) << 25)))) | ((uint) (((uint) (((bool) (!((bool) (((byte) ((((byte) ((imm) >> (int) ((byte) 0x6))) & ((byte) 0x1)))) != ((byte) 0x0))))) ? 1U : 0U)) << 30)))) | ((uint) (((uint) ((byte) ((byte) ((byte) ((imm) >> (int) ((byte) 0x7)))))) << 31))))));
			return "FMOV-scalar-immediate";
		}
		insn_112:
		if((insn & 0xBFF8FC00) == 0x0F00F400) {
			var Q = (insn >> 30) & 0x1U;
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("4S") : (string) ("2S"));
			var sv = (float) (Math.Bitcast<uint, float>((uint) ((((uint) ((uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (uint) (((uint) (((uint) ((uint) ((uint) ((byte) 0x0)))) << 0)) | ((uint) (((uint) (h)) << 19)))) | ((uint) (((uint) (g)) << 20)))) | ((uint) (((uint) (f)) << 21)))) | ((uint) (((uint) (e)) << 22)))) | ((uint) (((uint) (d)) << 23)))) | ((uint) (((uint) (c)) << 24)))) | ((uint) (((uint) (b)) << 25)))) | ((uint) (((uint) (b)) << 26)))) | ((uint) (((uint) (b)) << 27)))) | ((uint) (((uint) (b)) << 28)))) | ((uint) (((uint) (b)) << 29)))) | ((uint) (((uint) (b)) << 30)))) | ((uint) (((uint) (a)) << 31))))) ^ ((uint) (((uint) ((uint) ((byte) 0x1))) << (int) ((byte) 0x1E)))))));
			return "FMOV-vector-immediate-single";
		}
		insn_113:
		if((insn & 0xFFF8FC00) == 0x6F00F400) {
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var sv = (double) (Math.Bitcast<ulong, double>((ulong) ((((ulong) ((ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (((ulong) ((ulong) ((ulong) ((byte) 0x0)))) << 0)) | ((ulong) (((ulong) (h)) << 48)))) | ((ulong) (((ulong) (g)) << 49)))) | ((ulong) (((ulong) (f)) << 50)))) | ((ulong) (((ulong) (e)) << 51)))) | ((ulong) (((ulong) (d)) << 52)))) | ((ulong) (((ulong) (c)) << 53)))) | ((ulong) (((ulong) (b)) << 54)))) | ((ulong) (((ulong) (b)) << 55)))) | ((ulong) (((ulong) (b)) << 56)))) | ((ulong) (((ulong) (b)) << 57)))) | ((ulong) (((ulong) (b)) << 58)))) | ((ulong) (((ulong) (b)) << 59)))) | ((ulong) (((ulong) (b)) << 60)))) | ((ulong) (((ulong) (b)) << 61)))) | ((ulong) (((ulong) (b)) << 62)))) | ((ulong) (((ulong) (a)) << 63))))) ^ ((ulong) (((ulong) ((ulong) ((byte) 0x1))) << (int) ((byte) 0x3E)))))));
			return "FMOV-vector-immediate-double";
		}
		insn_114:
		if((insn & 0xFF208000) == 0x1F008000) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var ra = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) (type switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", (byte) ((byte) 0x3) => "H", _ => throw new NotImplementedException() });
			return "FMSUB";
		}
		insn_115:
		if((insn & 0xFF80F400) == 0x5F809000) {
			var sz = (insn >> 22) & 0x1U;
			var L = (insn >> 21) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var H = (insn >> 11) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var Ts = (string) (((bool) ((sz) != ((byte) 0x0))) ? (string) ("D") : (string) ("S"));
			var index = (uint) ((byte) ((byte) (((byte) (((byte) (L)) << 0)) | ((byte) (((byte) (sz)) << 1)))) switch { (byte) ((byte) 0x2) => (uint) ((uint) (H)), (byte) ((byte) 0x3) => throw new NotImplementedException(), _ => (uint) ((uint) ((uint) ((byte) ((byte) (((byte) (((byte) (L)) << 0)) | ((byte) (((byte) (H)) << 1))))))) });
			return "FMUL-by-element-scalar-spdp";
		}
		insn_116:
		if((insn & 0xBF80F400) == 0x0F809000) {
			var Q = (insn >> 30) & 0x1U;
			var sz = (insn >> 22) & 0x1U;
			var L = (insn >> 21) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var H = (insn >> 11) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (sz)) << 0)) | ((byte) (((byte) (Q)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x2) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			var Ts = (string) (((bool) ((sz) != ((byte) 0x0))) ? (string) ("D") : (string) ("S"));
			var index = (uint) ((byte) ((byte) (((byte) (((byte) (L)) << 0)) | ((byte) (((byte) (sz)) << 1)))) switch { (byte) ((byte) 0x2) => (uint) ((uint) (H)), (byte) ((byte) 0x3) => throw new NotImplementedException(), _ => (uint) ((uint) ((uint) ((byte) ((byte) (((byte) (((byte) (L)) << 0)) | ((byte) (((byte) (H)) << 1))))))) });
			return "FMUL-by-element-vector-spdp";
		}
		insn_117:
		if((insn & 0xFF20FC00) == 0x1E200800) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return "FMUL-scalar";
		}
		insn_118:
		if((insn & 0xBFA0FC00) == 0x2E20DC00) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var ts = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return "FMUL-vector";
		}
		insn_119:
		if((insn & 0xFF3FFC00) == 0x1E214000) {
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return "FNEG-scalar";
		}
		insn_120:
		if((insn & 0xBFBFFC00) == 0x2EA0F800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return "FNEG-vector";
		}
		insn_121:
		if((insn & 0xFF208000) == 0x1F200000) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var ra = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return "FNMADD";
		}
		insn_122:
		if((insn & 0xFF208000) == 0x1F208000) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var ra = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return "FNMSUB";
		}
		insn_123:
		if((insn & 0xFF20FC00) == 0x1E208800) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return "FNMUL-scalar";
		}
		insn_124:
		if((insn & 0xFF3FFC00) == 0x1E264000) {
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return "FRINTA-scalar";
		}
		insn_125:
		if((insn & 0xFF3FFC00) == 0x1E27C000) {
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return "FRINTI-scalar";
		}
		insn_126:
		if((insn & 0xFF3FFC00) == 0x1E254000) {
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return "FRINTM-scalar";
		}
		insn_127:
		if((insn & 0xFF3FFC00) == 0x1E24C000) {
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return "FRINTP-scalar";
		}
		insn_128:
		if((insn & 0xFF3FFC00) == 0x1E274000) {
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return "FRINTX-scalar";
		}
		insn_129:
		if((insn & 0xFF3FFC00) == 0x1E25C000) {
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return "FRINTZ-scalar";
		}
		insn_130:
		if((insn & 0xBFBFFC00) == 0x2EA1D800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return "FRSQRTE-vector";
		}
		insn_131:
		if((insn & 0xBFA0FC00) == 0x0EA0FC00) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return "FRSQRTS-vector";
		}
		insn_132:
		if((insn & 0xFF3FFC00) == 0x1E21C000) {
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return "FSQRT-scalar";
		}
		insn_133:
		if((insn & 0xFF20FC00) == 0x1E203800) {
			var type = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (type switch { (byte) ((byte) 0x3) => "H", (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => throw new NotImplementedException() });
			return "FSUB-scalar";
		}
		insn_134:
		if((insn & 0xBFA0FC00) == 0x0EA0D400) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var ts = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return "FSUB-vector";
		}
		insn_135:
		if((insn & 0xFFFFF01F) == 0xD503201F) {
			var crm = (insn >> 8) & 0xFU;
			var op2 = (insn >> 5) & 0x7U;
			var hint = (byte) ((byte) (((byte) (((byte) (op2)) << 0)) | ((byte) (((byte) (crm)) << 3))));
			return "HINT";
		}
		insn_136:
		if((insn & 0xFFE0FC00) == 0x4E001C00) {
			var imm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0xF))))))) != ((byte) 0x0))))
				goto insn_137;
			var ts = "";
			var index = (uint) ((uint) ((byte) 0x0));
			var r = "W";
			if((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x1))))))) == ((byte) 0x1))) {
				ts = "B";
				index = (byte) ((imm) >> (int) ((byte) 0x1));
			} else {
				if((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x2))))))) == ((byte) 0x2))) {
					ts = "H";
					index = (byte) ((imm) >> (int) ((byte) 0x2));
				} else {
					if((bool) (((byte) ((byte) (((imm) & ((byte) ((byte) ((byte) 0x4))))))) == ((byte) 0x4))) {
						ts = "S";
						index = (byte) ((imm) >> (int) ((byte) 0x3));
					} else {
						ts = "D";
						index = (byte) ((imm) >> (int) ((byte) 0x4));
						r = "X";
					}
				}
			}
			return "INS-general";
		}
		insn_137:
		if((insn & 0xFFE08400) == 0x6E000400) {
			var imm5 = (insn >> 16) & 0x1FU;
			var imm4 = (insn >> 11) & 0xFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) ((byte) (((imm5) & ((byte) ((byte) ((byte) 0xF))))))) != ((byte) 0x0))))
				goto insn_138;
			var ts = "";
			var index1 = (uint) ((uint) ((byte) 0x0));
			var index2 = (uint) ((uint) ((byte) 0x0));
			if((bool) (((byte) ((byte) (((imm5) & ((byte) ((byte) ((byte) 0x1))))))) == ((byte) 0x1))) {
				ts = "B";
				index1 = (byte) ((imm5) >> (int) ((byte) 0x1));
				index2 = imm4;
			} else {
				if((bool) (((byte) ((byte) (((imm5) & ((byte) ((byte) ((byte) 0x2))))))) == ((byte) 0x2))) {
					ts = "H";
					index1 = (byte) ((imm5) >> (int) ((byte) 0x2));
					index2 = (byte) ((imm4) >> (int) ((byte) 0x1));
				} else {
					if((bool) (((byte) ((byte) (((imm5) & ((byte) ((byte) ((byte) 0x4))))))) == ((byte) 0x4))) {
						ts = "S";
						index1 = (byte) ((imm5) >> (int) ((byte) 0x3));
						index2 = (byte) ((imm4) >> (int) ((byte) 0x2));
					} else {
						ts = "D";
						index1 = (byte) ((imm5) >> (int) ((byte) 0x4));
						index2 = (byte) ((imm4) >> (int) ((byte) 0x3));
					}
				}
			}
			return "INS-vector";
		}
		insn_138:
		if((insn & 0xBFFFF000) == 0x0C407000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x6) => "1D", _ => "2D" });
			return "LD1-multi-no-offset-one-register";
		}
		insn_139:
		if((insn & 0xBFFFF000) == 0x0CDF7000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x6) => "1D", _ => "2D" });
			var imm = (byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x10) : (byte) ((byte) 0x8));
			return "LD1-multi-no-offset-one-register-postindex-immediate";
		}
		insn_140:
		if((insn & 0xBFFFF000) == 0x0C40A000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x6) => "1D", _ => "2D" });
			return "LD1-multi-no-offset-two-registers";
		}
		insn_141:
		if((insn & 0xBFFFF000) == 0x0CDFA000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var imm = (byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x20) : (byte) ((byte) 0x10));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x6) => "1D", _ => "2D" });
			return "LD1-multi-no-offset-two-registers-postindex-immediate";
		}
		insn_142:
		if((insn & 0xBFFFF000) == 0x0C406000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x6) => "1D", _ => "2D" });
			return "LD1-multi-no-offset-three-registers";
		}
		insn_143:
		if((insn & 0xBFFFF000) == 0x0CDF6000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var imm = (byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x30) : (byte) ((byte) 0x18));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x6) => "1D", _ => "2D" });
			return "LD1-multi-no-offset-three-registers-postindex-immediate";
		}
		insn_144:
		if((insn & 0xBFFFF000) == 0x0C402000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var rt4 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x3))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x6) => "1D", _ => "2D" });
			return "LD1-multi-no-offset-four-registers";
		}
		insn_145:
		if((insn & 0xBFFFF000) == 0x0C5F2000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var rt4 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x3))))) % ((byte) (byte) ((byte) 0x20)));
			var imm = (byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x40) : (byte) ((byte) 0x20));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x6) => "1D", _ => "2D" });
			return "LD1-multi-no-offset-four-registers-postindex-immediate";
		}
		insn_146:
		if((insn & 0xBFFF2000) == 0x0D400000) {
			var Q = (insn >> 30) & 0x1U;
			var opc = (insn >> 14) & 0x3U;
			var S = (insn >> 12) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (opc)) != ((byte) 0x3))))
				goto insn_147;
			var t = (string) (((bool) (((byte) (opc)) == ((byte) 0x0))) ? (string) ("B") : (string) ((string) (((bool) ((((bool) (((byte) (opc)) == ((byte) 0x1))) & ((bool) (((byte) ((byte) (((size) & ((byte) ((byte) ((byte) 0x1))))))) == ((byte) 0x0)))))) ? (string) ("H") : (string) ((string) (((bool) (((byte) (opc)) == ((byte) 0x2))) ? ((string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("S") : (string) ((string) (((bool) ((((bool) (((byte) (size)) == ((byte) 0x1))) & ((bool) (((byte) (S)) == ((byte) 0x0)))))) ? ("D") : throw new NotImplementedException())))) : throw new NotImplementedException())))));
			var index = (uint) (opc switch { (byte) ((byte) 0x0) => (uint) ((uint) ((byte) ((byte) (((byte) (byte) (((byte) (((byte) (size)) << 0)) | ((byte) (((byte) (S)) << 2)))) | ((byte) (((byte) (Q)) << 3)))))), (byte) ((byte) 0x1) => (uint) ((uint) (((uint) ((uint) ((byte) ((byte) (((byte) (byte) (((byte) (((byte) (size)) << 0)) | ((byte) (((byte) (S)) << 2)))) | ((byte) (((byte) (Q)) << 3))))))) >> (int) ((byte) 0x1))), (byte) ((byte) 0x2) => (uint) ((uint) (((bool) (((byte) ((byte) (((size) & ((byte) ((byte) ((byte) 0x1))))))) == ((byte) 0x0))) ? (uint) ((uint) ((uint) ((byte) ((byte) (((byte) (((byte) (S)) << 0)) | ((byte) (((byte) (Q)) << 1))))))) : (uint) (Q))), _ => throw new NotImplementedException() });
			return "LD1-single-no-offset";
		}
		insn_147:
		if((insn & 0xBFFFF000) == 0x0D40C000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x6) => "1D", _ => "2D" });
			return "LD1R-single-no-offset";
		}
		insn_148:
		if((insn & 0xBFE0F000) == 0x0DC0C000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (rm)) == ((byte) 0x1F))))
				goto insn_149;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x6) => "1D", _ => "2D" });
			var imm = (byte) (size switch { (byte) ((byte) 0x0) => (byte) 0x1, (byte) ((byte) 0x1) => (byte) ((byte) 0x2), (byte) ((byte) 0x2) => (byte) ((byte) 0x4), _ => (byte) ((byte) 0x8) });
			return "LD1R-single-postindex-immediate";
		}
		insn_149:
		if((insn & 0xBFE0F000) == 0x0DC0C000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (rm)) != ((byte) 0x1F))))
				goto insn_150;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x6) => "1D", _ => "2D" });
			return "LD1R-single-postindex-register";
		}
		insn_150:
		if((insn & 0xBFFFF000) == 0x0C408000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "LD2-multi-no-offset";
		}
		insn_151:
		if((insn & 0xBFE0F000) == 0x0CC08000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var imm = (byte) ((byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x20) : (byte) ((byte) 0x10)));
			if(!((bool) (((byte) (rm)) == ((byte) 0x1F))))
				goto insn_152;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "LD2-multi-postindex-immediate";
		}
		insn_152:
		if((insn & 0xBFE0F000) == 0x0CC08000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) != ((byte) 0x1F))))
				goto insn_153;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "LD2-multi-postindex-register";
		}
		insn_153:
		if((insn & 0xBFFFF000) == 0x0C404000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "LD3-multi-no-offset";
		}
		insn_154:
		if((insn & 0xBFE0F000) == 0x0CC04000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var imm = (byte) ((byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x30) : (byte) ((byte) 0x18)));
			if(!((bool) (((byte) (rm)) == ((byte) 0x1F))))
				goto insn_155;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "LD3-multi-postindex-immediate";
		}
		insn_155:
		if((insn & 0xBFE0F000) == 0x0CC04000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) != ((byte) 0x1F))))
				goto insn_156;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "LD3-multi-postindex-register";
		}
		insn_156:
		if((insn & 0xBFFFF000) == 0x0C400000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var rt4 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x3))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "LD4-multi-no-offset";
		}
		insn_157:
		if((insn & 0xBFE0F000) == 0x0CC00000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var rt4 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x3))))) % ((byte) (byte) ((byte) 0x20)));
			var imm = (byte) ((byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x40) : (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) == ((byte) 0x1F))))
				goto insn_158;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "LD4-multi-postindex-immediate";
		}
		insn_158:
		if((insn & 0xBFE0F000) == 0x0CC00000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var rt4 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x3))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) != ((byte) 0x1F))))
				goto insn_159;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "LD4-multi-postindex-register";
		}
		insn_159:
		if((insn & 0xBFFFFC00) == 0x88DFFC00) {
			var size = (insn >> 30) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "LDAR";
		}
		insn_160:
		if((insn & 0xFFFFFC00) == 0x08DFFC00) {
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return "LDARB";
		}
		insn_161:
		if((insn & 0xFFFFFC00) == 0x48DFFC00) {
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return "LDARH";
		}
		insn_162:
		if((insn & 0xBFFFFC00) == 0x885FFC00) {
			var size = (insn >> 30) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return "LDAXB";
		}
		insn_163:
		if((insn & 0xFFFFFC00) == 0x085FFC00) {
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return "LDAXRB";
		}
		insn_164:
		if((insn & 0xFFFFFC00) == 0x485FFC00) {
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return "LDAXRH";
		}
		insn_165:
		if((insn & 0x7FC00000) == 0x28C00000) {
			var size = (insn >> 31) & 0x1U;
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt1) != (rt2))))
				goto insn_166;
			if(!((bool) ((rt1) != (rn))))
				goto insn_166;
			if(!((bool) ((rt2) != (rn))))
				goto insn_166;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x3))));
			return "LDP-immediate-postindex";
		}
		insn_166:
		if((insn & 0x7FC00000) == 0x29400000) {
			var size = (insn >> 31) & 0x1U;
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt1) != (rt2))))
				goto insn_167;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x3))));
			return "LDP-immediate-signed-offset";
		}
		insn_167:
		if((insn & 0x3FC00000) == 0x2CC00000) {
			var opc = (insn >> 30) & 0x3U;
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt1) != (rt2))))
				goto insn_168;
			var r = (string) (opc switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => "Q" });
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) (opc switch { (byte) ((byte) 0x0) => (byte) 0x2, (byte) ((byte) 0x1) => (byte) ((byte) 0x3), _ => (byte) ((byte) 0x4) })));
			return "LDP-simd-postindex";
		}
		insn_168:
		if((insn & 0x3FC00000) == 0x2DC00000) {
			var opc = (insn >> 30) & 0x3U;
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt1) != (rt2))))
				goto insn_169;
			var r = (string) (opc switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => "Q" });
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) (opc switch { (byte) ((byte) 0x0) => (byte) 0x2, (byte) ((byte) 0x1) => (byte) ((byte) 0x3), _ => (byte) ((byte) 0x4) })));
			return "LDP-simd-preindex";
		}
		insn_169:
		if((insn & 0x3FC00000) == 0x2D400000) {
			var opc = (insn >> 30) & 0x3U;
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt1) != (rt2))))
				goto insn_170;
			var r = (string) (opc switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", _ => "Q" });
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) (opc switch { (byte) ((byte) 0x0) => (byte) 0x2, (byte) ((byte) 0x1) => (byte) ((byte) 0x3), _ => (byte) ((byte) 0x4) })));
			return "LDP-simd-signed-offset";
		}
		insn_170:
		if((insn & 0xFFC00000) == 0x69400000) {
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt1) != (rt2))))
				goto insn_171;
			if(!((bool) ((rt1) != (rn))))
				goto insn_171;
			if(!((bool) ((rt2) != (rn))))
				goto insn_171;
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) 0x2));
			return "LDPSW-immediate-signed-offset";
		}
		insn_171:
		if((insn & 0xBFE00C00) == 0xB8400C00) {
			var size = (insn >> 30) & 0x1U;
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) ((rd) != (rn))))
				goto insn_172;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return "LDR-immediate-preindex";
		}
		insn_172:
		if((insn & 0xBFE00C00) == 0xB8400400) {
			var size = (insn >> 30) & 0x1U;
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) ((rd) != (rn))))
				goto insn_173;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return "LDR-immediate-postindex";
		}
		insn_173:
		if((insn & 0xBFC00000) == 0xB9400000) {
			var size = (insn >> 30) & 0x1U;
			var rawimm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var imm = (ushort) ((rawimm) << (int) ((byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x3))));
			return "LDR-immediate-unsigned-offset";
		}
		insn_174:
		if((insn & 0xBF000000) == 0x18000000) {
			var size = (insn >> 30) & 0x1U;
			var rawimm = (insn >> 5) & 0x7FFFFU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var offset = (long) (Math.SignExt<long>((uint) ((uint) ((uint) ((rawimm) << (int) ((byte) 0x2)))), 21));
			var addr = (ulong) (((ulong) (ulong) ((ulong) (pc))) + ((ulong) (long) (offset)));
			return "LDR-literal";
		}
		insn_175:
		if((insn & 0x3F600C00) == 0x3C400400) {
			var size = (insn >> 30) & 0x3U;
			var opc = (insn >> 23) & 0x1U;
			var imm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var simm = (long) (Math.SignExt<long>(imm, 9));
			var r = (string) ((byte) ((byte) (((byte) (((byte) (opc)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "B", (byte) ((byte) 0x2) => "H", (byte) ((byte) 0x4) => "S", (byte) ((byte) 0x6) => "D", (byte) ((byte) 0x1) => "Q", _ => throw new NotImplementedException() });
			return "LDR-simd-immediate-postindex";
		}
		insn_176:
		if((insn & 0x3F600C00) == 0x3C400C00) {
			var size = (insn >> 30) & 0x3U;
			var opc = (insn >> 23) & 0x1U;
			var imm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var simm = (long) (Math.SignExt<long>(imm, 9));
			var r = (string) ((byte) ((byte) (((byte) (((byte) (opc)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "B", (byte) ((byte) 0x2) => "H", (byte) ((byte) 0x4) => "S", (byte) ((byte) 0x6) => "D", (byte) ((byte) 0x1) => "Q", _ => throw new NotImplementedException() });
			return "LDR-simd-immediate-preindex";
		}
		insn_177:
		if((insn & 0x3F400000) == 0x3D400000) {
			var size = (insn >> 30) & 0x3U;
			var ropc = (insn >> 23) & 0x1U;
			var rawimm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var opc = (byte) ((byte) (((byte) (((byte) ((byte) ((byte) ((byte) 0x1)))) << 0)) | ((byte) (((byte) (ropc)) << 1))));
			var m = (byte) ((byte) (((byte) (((byte) (opc)) << 0)) | ((byte) (((byte) (size)) << 2))));
			var r = (string) (m switch { (byte) ((byte) 0x1) => "B", (byte) ((byte) 0x5) => "H", (byte) ((byte) 0x9) => "S", (byte) ((byte) 0xD) => "D", _ => "Q" });
			var imm = (uint) (((uint) ((uint) (rawimm))) << (int) ((byte) (m switch { (byte) ((byte) 0x1) => (byte) 0x0, (byte) ((byte) 0x5) => (byte) ((byte) 0x1), (byte) ((byte) 0x9) => (byte) ((byte) 0x2), (byte) ((byte) 0xD) => (byte) ((byte) 0x3), _ => (byte) ((byte) 0x4) })));
			return "LDR-simd-immediate-unsigned-offset";
		}
		insn_178:
		if((insn & 0x3F000000) == 0x1C000000) {
			var size = (insn >> 30) & 0x3U;
			var imm = (insn >> 5) & 0x7FFFFU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (size switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", (byte) ((byte) 0x2) => "Q", _ => throw new NotImplementedException() });
			var addr = (ulong) (((ulong) (ulong) ((ulong) (pc))) + ((ulong) (long) ((long) (Math.SignExt<long>((uint) ((uint) (((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 0)) | ((uint) (((uint) (imm)) << 2)))), 21)))));
			return "LDR-simd-literal";
		}
		insn_179:
		if((insn & 0x3F600C00) == 0x3C600800) {
			var size = (insn >> 30) & 0x3U;
			var opc = (insn >> 23) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var scale = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r1 = (string) (((bool) ((((bool) (((byte) (size)) == ((byte) 0x0))) & ((bool) (((byte) (opc)) == ((byte) 0x1)))))) ? (string) ("Q") : (string) ((string) (size switch { (byte) ((byte) 0x0) => "B", (byte) ((byte) 0x1) => "H", (byte) ((byte) 0x2) => "S", (byte) ((byte) 0x3) => "D", _ => throw new NotImplementedException() })));
			var r2 = (string) (((bool) (((byte) (((option) & ((byte) ((byte) ((byte) 0x1)))))) != ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var extend = (string) (option switch { (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x6) => "SXTW", (byte) ((byte) 0x7) => "SXTX", _ => throw new NotImplementedException() });
			var amount = (byte) (((byte) (byte) (scale)) * ((byte) (byte) ((byte) (((bool) ((((bool) (((byte) (size)) == ((byte) 0x0))) & ((bool) (((byte) (opc)) == ((byte) 0x1)))))) ? (byte) ((byte) 0x4) : (byte) ((byte) (size switch { (byte) ((byte) 0x0) => (byte) 0x1, (byte) ((byte) 0x1) => (byte) ((byte) 0x1), (byte) ((byte) 0x2) => (byte) ((byte) 0x2), (byte) ((byte) 0x3) => (byte) ((byte) 0x3), _ => throw new NotImplementedException() }))))));
			return "LDR-simd-register";
		}
		insn_180:
		if((insn & 0xBFE00C00) == 0xB8600800) {
			var size = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var scale = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r1 = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var r2 = (string) (((bool) (((byte) (((option) & ((byte) ((byte) ((byte) 0x1)))))) != ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var amount = (byte) (((bool) (((byte) (scale)) == ((byte) 0x0))) ? (byte) ((byte) 0x0) : (byte) ((byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x3))));
			var extend = (string) (option switch { (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x6) => "SXTW", (byte) ((byte) 0x7) => "SXTX", (byte) ((byte) 0x3) => "LSL", _ => throw new NotImplementedException() });
			return "LDR-register";
		}
		insn_181:
		if((insn & 0xFFE00C00) == 0x38400400) {
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt) != (rn))))
				goto insn_182;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return "LDRB-immediate-postindex";
		}
		insn_182:
		if((insn & 0xFFE00C00) == 0x38400C00) {
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt) != (rn))))
				goto insn_183;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return "LDRB-immediate-preindex";
		}
		insn_183:
		if((insn & 0xFFC00000) == 0x39400000) {
			var imm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return "LDRB-immediate-unsigned-offset";
		}
		insn_184:
		if((insn & 0xFFE00C00) == 0x38600800) {
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var amount = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (((option) & ((byte) ((byte) ((byte) 0x1)))))) != ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var str = (string) (option switch { (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x6) => "SXTW", (byte) ((byte) 0x7) => "SXTX", _ => throw new NotImplementedException() });
			return "LDRB-register";
		}
		insn_185:
		if((insn & 0xFFE00C00) == 0x78400400) {
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt) != (rn))))
				goto insn_186;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return "LDRH-immediate-postindex";
		}
		insn_186:
		if((insn & 0xFFE00C00) == 0x78400C00) {
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt) != (rn))))
				goto insn_187;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return "LDRH-immediate-preindex";
		}
		insn_187:
		if((insn & 0xFFC00000) == 0x79400000) {
			var rawimm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var imm = (ushort) ((rawimm) << (int) ((byte) 0x1));
			return "LDRH-immediate-unsigned-offset";
		}
		insn_188:
		if((insn & 0xFFE00C00) == 0x78600800) {
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var amount = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (((option) & ((byte) ((byte) ((byte) 0x1)))))) != ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var str = (string) (option switch { (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x6) => "SXTW", (byte) ((byte) 0x7) => "SXTX", _ => throw new NotImplementedException() });
			return "LDRH-register";
		}
		insn_189:
		if((insn & 0xFFA00C00) == 0x38800400) {
			var opc = (insn >> 22) & 0x1U;
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt) != (rn))))
				goto insn_190;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			var r = (string) (((bool) (((byte) (opc)) == ((byte) 0x1))) ? (string) ("W") : (string) ("X"));
			return "LDRSB-immediate-postindex";
		}
		insn_190:
		if((insn & 0xFFA00C00) == 0x38800C00) {
			var opc = (insn >> 22) & 0x1U;
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt) != (rn))))
				goto insn_191;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			var r = (string) (((bool) (((byte) (opc)) == ((byte) 0x1))) ? (string) ("W") : (string) ("X"));
			return "LDRSB-immediate-preindex";
		}
		insn_191:
		if((insn & 0xFF800000) == 0x39800000) {
			var opc = (insn >> 22) & 0x1U;
			var imm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (opc)) == ((byte) 0x1))) ? (string) ("W") : (string) ("X"));
			return "LDRSB-immediate-unsigned-offset";
		}
		insn_192:
		if((insn & 0xFFA00C00) == 0x38A00800) {
			var opc = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var amount = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (opc)) == ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var str = (string) (option switch { (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x6) => "SXTW", (byte) ((byte) 0x7) => "SXTX", _ => throw new NotImplementedException() });
			return "LDRSB-register";
		}
		insn_193:
		if((insn & 0xFFA00C00) == 0x78800400) {
			var opc = (insn >> 22) & 0x1U;
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt) != (rn))))
				goto insn_194;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			var r = (string) (((bool) (((byte) (opc)) == ((byte) 0x1))) ? (string) ("W") : (string) ("X"));
			return "LDRSH-immediate-postindex";
		}
		insn_194:
		if((insn & 0xFFA00C00) == 0x78800C00) {
			var opc = (insn >> 22) & 0x1U;
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt) != (rn))))
				goto insn_195;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			var r = (string) (((bool) (((byte) (opc)) == ((byte) 0x1))) ? (string) ("W") : (string) ("X"));
			return "LDRSH-immediate-preindex";
		}
		insn_195:
		if((insn & 0xFF800000) == 0x79800000) {
			var opc = (insn >> 22) & 0x1U;
			var rawimm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (opc)) == ((byte) 0x1))) ? (string) ("W") : (string) ("X"));
			var imm = (ushort) ((rawimm) << (int) ((byte) 0x1));
			return "LDRSH-immediate-unsigned-offset";
		}
		insn_196:
		if((insn & 0xFFA00C00) == 0x78A00800) {
			var opc = (insn >> 22) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var amount = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (opc)) == ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var str = (string) (option switch { (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x6) => "SXTW", (byte) ((byte) 0x7) => "SXTX", _ => throw new NotImplementedException() });
			return "LDRSH-register";
		}
		insn_197:
		if((insn & 0xFFE00C00) == 0xB8800400) {
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt) != (rn))))
				goto insn_198;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return "LDRSW-immediate-postindex";
		}
		insn_198:
		if((insn & 0xFFE00C00) == 0xB8800C00) {
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) ((rt) != (rn))))
				goto insn_199;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return "LDRSW-immediate-preindex";
		}
		insn_199:
		if((insn & 0xFFC00000) == 0xB9800000) {
			var rawimm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var imm = (ushort) ((rawimm) << (int) ((byte) 0x2));
			return "LDRSW-immediate-unsigned-offset";
		}
		insn_200:
		if((insn & 0xFF000000) == 0x98000000) {
			var imm = (insn >> 5) & 0x7FFFFU;
			var rt = (insn >> 0) & 0x1FU;
			var addr = (ulong) (((ulong) (ulong) ((ulong) (pc))) + ((ulong) (long) ((long) (Math.SignExt<long>((uint) ((uint) (((uint) (((uint) ((byte) ((byte) ((byte) 0x0)))) << 0)) | ((uint) (((uint) (imm)) << 2)))), 21)))));
			return "LDRSW-literal";
		}
		insn_201:
		if((insn & 0xFFE00C00) == 0xB8A00800) {
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var scale = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (((option) & ((byte) ((byte) ((byte) 0x1)))))) != ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var amount = (byte) (((bool) (((byte) (scale)) == ((byte) 0x0))) ? (byte) ((byte) 0x0) : (byte) ((byte) 0x2));
			var extend = (string) (option switch { (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x6) => "SXTW", (byte) ((byte) 0x7) => "SXTX", _ => throw new NotImplementedException() });
			return "LDRSW-register";
		}
		insn_202:
		if((insn & 0xBFE00C00) == 0xB8400000) {
			var size = (insn >> 30) & 0x1U;
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return "LDUR";
		}
		insn_203:
		if((insn & 0xFFE00C00) == 0x38400000) {
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return "LDURB";
		}
		insn_204:
		if((insn & 0xFFE00C00) == 0x78400000) {
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return "LDURH";
		}
		insn_205:
		if((insn & 0xFFA00C00) == 0x38800000) {
			var opc = (insn >> 22) & 0x1U;
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (opc)) == ((byte) 0x1))) ? (string) ("W") : (string) ("X"));
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return "LDURSB";
		}
		insn_206:
		if((insn & 0xFFA00C00) == 0x78800000) {
			var opc = (insn >> 22) & 0x1U;
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (opc)) == ((byte) 0x1))) ? (string) ("W") : (string) ("X"));
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return "LDURSH";
		}
		insn_207:
		if((insn & 0xFFE00C00) == 0xB8800000) {
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return "LDURSW";
		}
		insn_208:
		if((insn & 0x3F600C00) == 0x3C400000) {
			var size = (insn >> 30) & 0x3U;
			var opc = (insn >> 23) & 0x1U;
			var rawimm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) ((byte) ((byte) (((byte) (((byte) (opc)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "B", (byte) ((byte) 0x2) => "H", (byte) ((byte) 0x4) => "S", (byte) ((byte) 0x6) => "D", (byte) ((byte) 0x1) => "Q", _ => throw new NotImplementedException() });
			var imm = (long) (Math.SignExt<long>(rawimm, 9));
			return "LDUR-simd";
		}
		insn_209:
		if((insn & 0xBFFFFC00) == 0x885F7C00) {
			var size = (insn >> 30) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "LDXR";
		}
		insn_210:
		if((insn & 0xFFFFFC00) == 0x085F7C00) {
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return "LDXRB";
		}
		insn_211:
		if((insn & 0xFFFFFC00) == 0x485F7C00) {
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return "LDXRH";
		}
		insn_212:
		if((insn & 0xBFFF8000) == 0x887F0000) {
			var size = (insn >> 30) & 0x1U;
			var rt2 = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "LDXP";
		}
		insn_213:
		if((insn & 0x7FE0FC00) == 0x1AC02000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "LSL-register";
		}
		insn_214:
		if((insn & 0x7FE0FC00) == 0x1AC02400) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "LSRV";
		}
		insn_215:
		if((insn & 0x7FE08000) == 0x1B000000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var ra = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "MADD";
		}
		insn_216:
		if((insn & 0xFFF8FC00) == 0x2F00E400) {
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var imm8a = (byte) ((byte) (((bool) ((a) != ((byte) 0x0))) ? (byte) ((byte) 0xFF) : (byte) ((byte) 0x0)));
			var imm8b = (byte) ((byte) (((bool) ((b) != ((byte) 0x0))) ? (byte) ((byte) 0xFF) : (byte) ((byte) 0x0)));
			var imm8c = (byte) ((byte) (((bool) ((c) != ((byte) 0x0))) ? (byte) ((byte) 0xFF) : (byte) ((byte) 0x0)));
			var imm8d = (byte) ((byte) (((bool) ((d) != ((byte) 0x0))) ? (byte) ((byte) 0xFF) : (byte) ((byte) 0x0)));
			var imm8e = (byte) ((byte) (((bool) ((e) != ((byte) 0x0))) ? (byte) ((byte) 0xFF) : (byte) ((byte) 0x0)));
			var imm8f = (byte) ((byte) (((bool) ((f) != ((byte) 0x0))) ? (byte) ((byte) 0xFF) : (byte) ((byte) 0x0)));
			var imm8g = (byte) ((byte) (((bool) ((g) != ((byte) 0x0))) ? (byte) ((byte) 0xFF) : (byte) ((byte) 0x0)));
			var imm8h = (byte) ((byte) (((bool) ((h) != ((byte) 0x0))) ? (byte) ((byte) 0xFF) : (byte) ((byte) 0x0)));
			var imm = (ulong) ((ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (((ulong) (imm8h)) << 0)) | ((ulong) (((ulong) (imm8g)) << 8)))) | ((ulong) (((ulong) (imm8f)) << 16)))) | ((ulong) (((ulong) (imm8e)) << 24)))) | ((ulong) (((ulong) (imm8d)) << 32)))) | ((ulong) (((ulong) (imm8c)) << 40)))) | ((ulong) (((ulong) (imm8b)) << 48)))) | ((ulong) (((ulong) (imm8a)) << 56))));
			return "MOVI-scalar-64bit";
		}
		insn_217:
		if((insn & 0xBFF8FC00) == 0x0F00E400) {
			var Q = (insn >> 30) & 0x1U;
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"));
			var imm = (byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (h)) << 0)) | ((byte) (((byte) (g)) << 1)))) | ((byte) (((byte) (f)) << 2)))) | ((byte) (((byte) (e)) << 3)))) | ((byte) (((byte) (d)) << 4)))) | ((byte) (((byte) (c)) << 5)))) | ((byte) (((byte) (b)) << 6)))) | ((byte) (((byte) (a)) << 7))));
			return "MOVI-vector-8bit";
		}
		insn_218:
		if((insn & 0xBFF8DC00) == 0x0F008400) {
			var Q = (insn >> 30) & 0x1U;
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var cmode = (insn >> 13) & 0x1U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("8H") : (string) ("4H"));
			var imm = (byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (h)) << 0)) | ((byte) (((byte) (g)) << 1)))) | ((byte) (((byte) (f)) << 2)))) | ((byte) (((byte) (e)) << 3)))) | ((byte) (((byte) (d)) << 4)))) | ((byte) (((byte) (c)) << 5)))) | ((byte) (((byte) (b)) << 6)))) | ((byte) (((byte) (a)) << 7))));
			return "MOVI-vector-16bit";
		}
		insn_219:
		if((insn & 0xBFF89C00) == 0x0F000400) {
			var Q = (insn >> 30) & 0x1U;
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var cmode = (insn >> 13) & 0x3U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("4S") : (string) ("2S"));
			var amount = (byte) (cmode switch { (byte) ((byte) 0x0) => (byte) 0x0, (byte) ((byte) 0x1) => (byte) ((byte) 0x8), (byte) ((byte) 0x2) => (byte) ((byte) 0x10), (byte) ((byte) 0x3) => (byte) ((byte) 0x18), _ => throw new NotImplementedException() });
			var imm = (byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (h)) << 0)) | ((byte) (((byte) (g)) << 1)))) | ((byte) (((byte) (f)) << 2)))) | ((byte) (((byte) (e)) << 3)))) | ((byte) (((byte) (d)) << 4)))) | ((byte) (((byte) (c)) << 5)))) | ((byte) (((byte) (b)) << 6)))) | ((byte) (((byte) (a)) << 7))));
			return "MOVI-vector-32bit";
		}
		insn_220:
		if((insn & 0xFFF8FC00) == 0x6F00E400) {
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var imm = (ulong) ((ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (ulong) (((ulong) (((ulong) ((byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (h)) << 0)) | ((byte) (((byte) (h)) << 1)))) | ((byte) (((byte) (h)) << 2)))) | ((byte) (((byte) (h)) << 3)))) | ((byte) (((byte) (h)) << 4)))) | ((byte) (((byte) (h)) << 5)))) | ((byte) (((byte) (h)) << 6)))) | ((byte) (((byte) (h)) << 7)))))) << 0)) | ((ulong) (((ulong) ((byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (g)) << 0)) | ((byte) (((byte) (g)) << 1)))) | ((byte) (((byte) (g)) << 2)))) | ((byte) (((byte) (g)) << 3)))) | ((byte) (((byte) (g)) << 4)))) | ((byte) (((byte) (g)) << 5)))) | ((byte) (((byte) (g)) << 6)))) | ((byte) (((byte) (g)) << 7)))))) << 8)))) | ((ulong) (((ulong) ((byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (f)) << 0)) | ((byte) (((byte) (f)) << 1)))) | ((byte) (((byte) (f)) << 2)))) | ((byte) (((byte) (f)) << 3)))) | ((byte) (((byte) (f)) << 4)))) | ((byte) (((byte) (f)) << 5)))) | ((byte) (((byte) (f)) << 6)))) | ((byte) (((byte) (f)) << 7)))))) << 16)))) | ((ulong) (((ulong) ((byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (e)) << 0)) | ((byte) (((byte) (e)) << 1)))) | ((byte) (((byte) (e)) << 2)))) | ((byte) (((byte) (e)) << 3)))) | ((byte) (((byte) (e)) << 4)))) | ((byte) (((byte) (e)) << 5)))) | ((byte) (((byte) (e)) << 6)))) | ((byte) (((byte) (e)) << 7)))))) << 24)))) | ((ulong) (((ulong) ((byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (d)) << 0)) | ((byte) (((byte) (d)) << 1)))) | ((byte) (((byte) (d)) << 2)))) | ((byte) (((byte) (d)) << 3)))) | ((byte) (((byte) (d)) << 4)))) | ((byte) (((byte) (d)) << 5)))) | ((byte) (((byte) (d)) << 6)))) | ((byte) (((byte) (d)) << 7)))))) << 32)))) | ((ulong) (((ulong) ((byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (c)) << 0)) | ((byte) (((byte) (c)) << 1)))) | ((byte) (((byte) (c)) << 2)))) | ((byte) (((byte) (c)) << 3)))) | ((byte) (((byte) (c)) << 4)))) | ((byte) (((byte) (c)) << 5)))) | ((byte) (((byte) (c)) << 6)))) | ((byte) (((byte) (c)) << 7)))))) << 40)))) | ((ulong) (((ulong) ((byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (b)) << 0)) | ((byte) (((byte) (b)) << 1)))) | ((byte) (((byte) (b)) << 2)))) | ((byte) (((byte) (b)) << 3)))) | ((byte) (((byte) (b)) << 4)))) | ((byte) (((byte) (b)) << 5)))) | ((byte) (((byte) (b)) << 6)))) | ((byte) (((byte) (b)) << 7)))))) << 48)))) | ((ulong) (((ulong) ((byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (a)) << 0)) | ((byte) (((byte) (a)) << 1)))) | ((byte) (((byte) (a)) << 2)))) | ((byte) (((byte) (a)) << 3)))) | ((byte) (((byte) (a)) << 4)))) | ((byte) (((byte) (a)) << 5)))) | ((byte) (((byte) (a)) << 6)))) | ((byte) (((byte) (a)) << 7)))))) << 56))));
			return "MOVI-Vx.2D";
		}
		insn_221:
		if((insn & 0x7F800000) == 0x72800000) {
			var size = (insn >> 31) & 0x1U;
			var hw = (insn >> 21) & 0x3U;
			var imm = (insn >> 5) & 0xFFFFU;
			var rd = (insn >> 0) & 0x1FU;
			if((bool) (((byte) (size)) == ((byte) 0x0))) {
				if(!((bool) (((byte) (hw)) < ((byte) 0x2))))
					goto insn_222;
			}
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shift = (uint) (((uint) ((uint) (hw))) << (int) ((byte) 0x4));
			return "MOVK";
		}
		insn_222:
		if((insn & 0x7F800000) == 0x12800000) {
			var size = (insn >> 31) & 0x1U;
			var hw = (insn >> 21) & 0x3U;
			var imm = (insn >> 5) & 0xFFFFU;
			var rd = (insn >> 0) & 0x1FU;
			if((bool) (((byte) (size)) == ((byte) 0x0))) {
				if(!((bool) (((byte) (hw)) < ((byte) 0x2))))
					goto insn_223;
			}
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shift = (uint) (((uint) ((uint) (hw))) << (int) ((byte) 0x4));
			return "MOVN";
		}
		insn_223:
		if((insn & 0x7F800000) == 0x52800000) {
			var size = (insn >> 31) & 0x1U;
			var hw = (insn >> 21) & 0x3U;
			var imm = (insn >> 5) & 0xFFFFU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shift = (uint) (((uint) ((uint) (hw))) << (int) ((byte) 0x4));
			return "MOVZ";
		}
		insn_224:
		if((insn & 0xFFF00000) == 0xD5300000) {
			var op0 = (insn >> 19) & 0x1U;
			var op1 = (insn >> 16) & 0x7U;
			var cn = (insn >> 12) & 0xFU;
			var cm = (insn >> 8) & 0xFU;
			var op2 = (insn >> 5) & 0x7U;
			var rt = (insn >> 0) & 0x1FU;
			return "MRS";
		}
		insn_225:
		if((insn & 0xFFF00000) == 0xD5100000) {
			var op0 = (insn >> 19) & 0x1U;
			var op1 = (insn >> 16) & 0x7U;
			var cn = (insn >> 12) & 0xFU;
			var cm = (insn >> 8) & 0xFU;
			var op2 = (insn >> 5) & 0x7U;
			var rt = (insn >> 0) & 0x1FU;
			return "MSR-register";
		}
		insn_226:
		if((insn & 0x7FE08000) == 0x1B008000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var ra = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "MSUB";
		}
		insn_227:
		if((insn & 0xBF00F400) == 0x0F008000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var L = (insn >> 21) & 0x1U;
			var M = (insn >> 20) & 0x1U;
			var rv = (insn >> 16) & 0xFU;
			var H = (insn >> 11) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var rm = (byte) (((bool) (((byte) (size)) == ((byte) 0x2))) ? (byte) ((byte) ((byte) (((byte) (((byte) (rv)) << 0)) | ((byte) (((byte) (M)) << 4))))) : (byte) (rv));
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", _ => throw new NotImplementedException() });
			var ts = (string) (size switch { (byte) ((byte) 0x1) => "H", (byte) ((byte) 0x2) => "S", _ => throw new NotImplementedException() });
			var index = (byte) (size switch { (byte) ((byte) 0x1) => (byte) ((byte) (((byte) (byte) (((byte) (((byte) (M)) << 0)) | ((byte) (((byte) (L)) << 1)))) | ((byte) (((byte) (H)) << 2)))), (byte) ((byte) 0x2) => (byte) ((byte) ((byte) (((byte) (((byte) (L)) << 0)) | ((byte) (((byte) (H)) << 1))))), _ => throw new NotImplementedException() });
			return "MUL-by-element";
		}
		insn_228:
		if((insn & 0xBF20FC00) == 0x0E209C00) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", _ => throw new NotImplementedException() });
			return "MUL-vector";
		}
		insn_229:
		if((insn & 0xBFF8DC00) == 0x2F008400) {
			var Q = (insn >> 30) & 0x1U;
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var cmode = (insn >> 13) & 0x1U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("8H") : (string) ("4H"));
			var imm = (byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (h)) << 0)) | ((byte) (((byte) (g)) << 1)))) | ((byte) (((byte) (f)) << 2)))) | ((byte) (((byte) (e)) << 3)))) | ((byte) (((byte) (d)) << 4)))) | ((byte) (((byte) (c)) << 5)))) | ((byte) (((byte) (b)) << 6)))) | ((byte) (((byte) (a)) << 7))));
			var amount = (byte) (((bool) ((cmode) != ((byte) 0x0))) ? (byte) ((byte) 0x8) : (byte) ((byte) 0x0));
			return "MVNI-vector-16bit";
		}
		insn_230:
		if((insn & 0xBFF89C00) == 0x2F000400) {
			var Q = (insn >> 30) & 0x1U;
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var cmode = (insn >> 13) & 0x3U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("4S") : (string) ("2S"));
			var amount = (byte) (cmode switch { (byte) ((byte) 0x0) => (byte) 0x0, (byte) ((byte) 0x1) => (byte) ((byte) 0x8), (byte) ((byte) 0x2) => (byte) ((byte) 0x10), (byte) ((byte) 0x3) => (byte) ((byte) 0x18), _ => throw new NotImplementedException() });
			var imm = (byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (h)) << 0)) | ((byte) (((byte) (g)) << 1)))) | ((byte) (((byte) (f)) << 2)))) | ((byte) (((byte) (e)) << 3)))) | ((byte) (((byte) (d)) << 4)))) | ((byte) (((byte) (c)) << 5)))) | ((byte) (((byte) (b)) << 6)))) | ((byte) (((byte) (a)) << 7))));
			return "MVNI-vector-32bit-LSL";
		}
		insn_231:
		if((insn & 0xBFF8EC00) == 0x2F00C400) {
			var Q = (insn >> 30) & 0x1U;
			var a = (insn >> 18) & 0x1U;
			var b = (insn >> 17) & 0x1U;
			var c = (insn >> 16) & 0x1U;
			var cmode = (insn >> 12) & 0x1U;
			var d = (insn >> 9) & 0x1U;
			var e = (insn >> 8) & 0x1U;
			var f = (insn >> 7) & 0x1U;
			var g = (insn >> 6) & 0x1U;
			var h = (insn >> 5) & 0x1U;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("4S") : (string) ("2S"));
			var amount = (byte) (((bool) ((cmode) != ((byte) 0x0))) ? (byte) ((byte) 0x10) : (byte) ((byte) 0x8));
			var imm = (byte) ((byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (byte) (((byte) (((byte) (h)) << 0)) | ((byte) (((byte) (g)) << 1)))) | ((byte) (((byte) (f)) << 2)))) | ((byte) (((byte) (e)) << 3)))) | ((byte) (((byte) (d)) << 4)))) | ((byte) (((byte) (c)) << 5)))) | ((byte) (((byte) (b)) << 6)))) | ((byte) (((byte) (a)) << 7))));
			return "MVNI-vector-32bit-MSL";
		}
		insn_232:
		if((insn & 0xBF3FFC00) == 0x2E20B800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "NEG-vector";
		}
		insn_233:
		if((insn & 0xFFFFFFFF) == 0xD503201F) {
			return "NOP";
		}
		insn_234:
		if((insn & 0x7F200000) == 0x2A200000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_235;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return "ORN-shifted-register";
		}
		insn_235:
		if((insn & 0x7F800000) == 0x32000000) {
			var size = (insn >> 31) & 0x1U;
			var up = (insn >> 22) & 0x1U;
			var immr = (insn >> 16) & 0x3FU;
			var imms = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var imm = (ulong) (MakeWMask(up, imms, immr, (byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x20) : (byte) ((byte) 0x40)), (byte) 0x1));
			return "ORR-immediate";
		}
		insn_236:
		if((insn & 0x7F200000) == 0x2A000000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_237;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return "ORR-shifted-register";
		}
		insn_237:
		if((insn & 0xBFE0FC00) == 0x0EA01C00) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) (((bool) (((byte) (Q)) == ((byte) 0x0))) ? (string) ("8B") : (string) ("16B"));
			return "ORR-simd-register";
		}
		insn_238:
		if((insn & 0xBF20FC00) == 0x0E20E000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var h = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("2") : (string) (""));
			var Ta = (string) (size switch { (byte) ((byte) 0x0) => "8H", (byte) ((byte) 0x3) => "1Q", _ => throw new NotImplementedException() });
			var Tb = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x6) => "1D", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "PMULL[2]";
		}
		insn_239:
		if((insn & 0xFFC00000) == 0xF9800000) {
			var imm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var imm5 = (insn >> 0) & 0x1FU;
			var pimm = (ushort) (((ushort) (ushort) (imm)) * ((ushort) (byte) ((byte) 0x8)));
			return "PRFM-immediate";
		}
		insn_240:
		if((insn & 0xFF000000) == 0xD8000000) {
			var imm = (insn >> 5) & 0x7FFFFU;
			var rt = (insn >> 0) & 0x1FU;
			return "PRFM-literal";
		}
		insn_241:
		if((insn & 0xFFE00C00) == 0xF8A00800) {
			var rm = (insn >> 16) & 0x1FU;
			var opt = (insn >> 13) & 0x7U;
			var S = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return "PRFM-register";
		}
		insn_242:
		if((insn & 0xFFE00C00) == 0xF8800000) {
			var imm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return "PRFUM";
		}
		insn_243:
		if((insn & 0x7FFFFC00) == 0x5AC00000) {
			var size = (insn >> 31) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "RBIT";
		}
		insn_244:
		if((insn & 0xFFFFFC1F) == 0xD65F0000) {
			var rn = (insn >> 5) & 0x1FU;
			return "RET";
		}
		insn_245:
		if((insn & 0x7FFFF800) == 0x5AC00800) {
			var size = (insn >> 31) & 0x1U;
			var opc = (insn >> 10) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "REV";
		}
		insn_246:
		if((insn & 0x7FFFFC00) == 0x5AC00400) {
			var size = (insn >> 31) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "REV16";
		}
		insn_247:
		if((insn & 0x7FE0FC00) == 0x1AC02C00) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "RORV";
		}
		insn_248:
		if((insn & 0x7FE0FC00) == 0x7A000000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "SBCS";
		}
		insn_249:
		if((insn & 0x7F800000) == 0x13000000) {
			var size = (insn >> 31) & 0x1U;
			var N = (insn >> 22) & 0x1U;
			var immr = (insn >> 16) & 0x3FU;
			var imms = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imms)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_250;
			if(!((bool) (((byte) (immr)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_250;
			if((bool) ((size) != ((byte) 0x0))) {
				if(!((bool) ((N) != ((byte) 0x0))))
					goto insn_250;
			} else {
				if(!((bool) (((byte) (N)) == ((byte) 0x0))))
					goto insn_250;
			}
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "SBFM";
		}
		insn_250:
		if((insn & 0x7F3FFC00) == 0x1E220000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var st = (byte) ((byte) (((byte) (((byte) (type)) << 0)) | ((byte) (((byte) (size)) << 2))));
			var r1 = "";
			var r2 = "";
			switch(st) {
				case (byte) ((byte) 0x3): {
					r1 = "H";
					r2 = "W";
					break;
				}
				case (byte) ((byte) 0x0): {
					r1 = "S";
					r2 = "W";
					break;
				}
				case (byte) ((byte) 0x1): {
					r1 = "D";
					r2 = "W";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "H";
					r2 = "X";
					break;
				}
				case (byte) ((byte) 0x4): {
					r1 = "S";
					r2 = "X";
					break;
				}
				case (byte) ((byte) 0x5): {
					r1 = "D";
					r2 = "X";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return "SCVTF-scalar-integer";
		}
		insn_251:
		if((insn & 0xFFBFFC00) == 0x5E21D800) {
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("S") : (string) ("D"));
			return "SCVTF-scalar";
		}
		insn_252:
		if((insn & 0xBFBFFC00) == 0x0E21D800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return "SCVTF-vector";
		}
		insn_253:
		if((insn & 0x7FE0FC00) == 0x1AC00C00) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "SDIV";
		}
		insn_254:
		if((insn & 0xFFC0FC00) == 0x5F405400) {
			var imm = (insn >> 16) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var shift = (ushort) (((ushort) (ushort) ((ushort) ((ushort) (((ushort) (((ushort) (imm)) << 0)) | ((ushort) (((ushort) ((byte) 0x1)) << 6)))))) - ((ushort) (byte) ((byte) 0x40)));
			return "SHL-scalar";
		}
		insn_255:
		if((insn & 0xBF80FC00) == 0x0F005400) {
			var Q = (insn >> 30) & 0x1U;
			var immh = (insn >> 19) & 0xFU;
			var immb = (insn >> 16) & 0x7U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = "";
			var size = (byte) 0x0;
			var shift = (byte) 0x0;
			if(!((bool) (((byte) (immh)) != ((byte) 0x0))))
				goto insn_256;
			if((bool) (((byte) (immh)) == ((byte) 0x1))) {
				T = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"));
				size = (byte) 0x1;
				shift = (byte) (((byte) (byte) ((byte) ((byte) ((byte) ((byte) (((byte) (((byte) (immb)) << 0)) | ((byte) (((byte) (immh)) << 3)))))))) - ((byte) (byte) ((byte) 0x8)));
			} else {
				if((bool) (((byte) ((byte) (((immh) & ((byte) ((byte) ((byte) 0xE))))))) == ((byte) 0x2))) {
					T = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("8H") : (string) ("4H"));
					size = (byte) 0x2;
					shift = (byte) (((byte) (byte) ((byte) ((byte) ((byte) ((byte) (((byte) (((byte) (immb)) << 0)) | ((byte) (((byte) (immh)) << 3)))))))) - ((byte) (byte) ((byte) 0x10)));
				} else {
					if((bool) (((byte) ((byte) (((immh) & ((byte) ((byte) ((byte) 0xC))))))) == ((byte) 0x4))) {
						T = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("4S") : (string) ("2S"));
						size = (byte) 0x4;
						shift = (byte) (((byte) (byte) ((byte) ((byte) ((byte) ((byte) (((byte) (((byte) (immb)) << 0)) | ((byte) (((byte) (immh)) << 3)))))))) - ((byte) (byte) ((byte) 0x20)));
					} else {
						T = (string) (((bool) ((Q) != ((byte) 0x0))) ? ("2D") : throw new NotImplementedException());
						size = (byte) 0x8;
						shift = (byte) (((byte) (byte) ((byte) ((byte) ((byte) ((byte) (((byte) (((byte) (immb)) << 0)) | ((byte) (((byte) (immh)) << 3)))))))) - ((byte) (byte) ((byte) 0x40)));
					}
				}
			}
			return "SHL-vector";
		}
		insn_256:
		if((insn & 0xFFE08000) == 0x9B200000) {
			var rm = (insn >> 16) & 0x1FU;
			var ra = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			return "SMADDL";
		}
		insn_257:
		if((insn & 0xFFE0FC00) == 0x9B407C00) {
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			return "SMULH";
		}
		insn_258:
		if((insn & 0xBF80FC00) == 0x0F00A400) {
			var Q = (insn >> 30) & 0x1U;
			var immh = (insn >> 19) & 0xFU;
			var immb = (insn >> 16) & 0x7U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var variant = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("2") : (string) (""));
			var ta = "";
			var tb = "";
			var shift = (ulong) ((ulong) ((byte) 0x0));
			if((bool) (((byte) (immh)) == ((byte) 0x1))) {
				ta = "8H";
				tb = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"));
				shift = (byte) (((byte) (byte) ((byte) ((byte) (((byte) (((byte) (immb)) << 0)) | ((byte) (((byte) (immh)) << 3)))))) - ((byte) (byte) ((byte) 0x8)));
			} else {
				if((bool) (((byte) ((byte) (((immh) & ((byte) ((byte) ((byte) 0xE))))))) == ((byte) 0x2))) {
					ta = "4S";
					tb = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("8H") : (string) ("4H"));
					shift = (byte) (((byte) (byte) ((byte) ((byte) (((byte) (((byte) (immb)) << 0)) | ((byte) (((byte) (immh)) << 3)))))) - ((byte) (byte) ((byte) 0x10)));
				} else {
					if((bool) (((byte) ((byte) (((immh) & ((byte) ((byte) ((byte) 0xC))))))) == ((byte) 0x4))) {
						ta = "2D";
						tb = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("4S") : (string) ("2S"));
						shift = (byte) (((byte) (byte) ((byte) ((byte) (((byte) (((byte) (immb)) << 0)) | ((byte) (((byte) (immh)) << 3)))))) - ((byte) (byte) ((byte) 0x20)));
					} else {
						throw new NotImplementedException();
					}
				}
			}
			return "SSHLL";
		}
		insn_259:
		if((insn & 0xBFFFF000) == 0x0C007000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ST1-multi-no-offset-one-register";
		}
		insn_260:
		if((insn & 0xBFE0F000) == 0x0C807000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var imm = (byte) ((byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x10) : (byte) ((byte) 0x8)));
			if(!((bool) (((byte) (rm)) == ((byte) 0x1F))))
				goto insn_261;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ST1-multi-postindex-immediate-one-register";
		}
		insn_261:
		if((insn & 0xBFE0F000) == 0x0C807000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (rm)) != ((byte) 0x1F))))
				goto insn_262;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ST1-multi-postindex-register-one-register";
		}
		insn_262:
		if((insn & 0xBFFFF000) == 0x0C00A000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ST1-multi-no-offset-two-registers";
		}
		insn_263:
		if((insn & 0xBFE0F000) == 0x0C80A000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var imm = (byte) ((byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x20) : (byte) ((byte) 0x10)));
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) == ((byte) 0x1F))))
				goto insn_264;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ST1-multi-postindex-immediate-two-registers";
		}
		insn_264:
		if((insn & 0xBFE0F000) == 0x0C80A000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) != ((byte) 0x1F))))
				goto insn_265;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ST1-multi-postindex-register-two-registers";
		}
		insn_265:
		if((insn & 0xBFFFF000) == 0x0C006000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ST1-multi-no-offset-three-registers";
		}
		insn_266:
		if((insn & 0xBFE0F000) == 0x0C806000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var imm = (byte) ((byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x30) : (byte) ((byte) 0x18)));
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) == ((byte) 0x1F))))
				goto insn_267;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ST1-multi-postindex-immediate-three-registers";
		}
		insn_267:
		if((insn & 0xBFE0F000) == 0x0C806000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) != ((byte) 0x1F))))
				goto insn_268;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ST1-multi-postindex-register-three-registers";
		}
		insn_268:
		if((insn & 0xBFFFF000) == 0x0C002000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var rt4 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x3))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ST1-multi-no-offset-four-registers";
		}
		insn_269:
		if((insn & 0xBFE0F000) == 0x0C802000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var imm = (byte) ((byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x40) : (byte) ((byte) 0x20)));
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var rt4 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x3))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) == ((byte) 0x1F))))
				goto insn_270;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ST1-multi-postindex-immediate-four-registers";
		}
		insn_270:
		if((insn & 0xBFE0F000) == 0x0C802000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var rt4 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x3))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) != ((byte) 0x1F))))
				goto insn_271;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ST1-multi-postindex-register-four-registers";
		}
		insn_271:
		if((insn & 0xBFFF2000) == 0x0D000000) {
			var Q = (insn >> 30) & 0x1U;
			var opc = (insn >> 14) & 0x3U;
			var S = (insn >> 12) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (opc)) != ((byte) 0x3))))
				goto insn_272;
			var t = (string) (((bool) (((byte) (opc)) == ((byte) 0x0))) ? (string) ("B") : (string) ((string) (((bool) ((((bool) (((byte) (opc)) == ((byte) 0x1))) & ((bool) (((byte) ((byte) (((size) & ((byte) ((byte) ((byte) 0x1))))))) == ((byte) 0x0)))))) ? (string) ("H") : (string) ((string) (((bool) (((byte) (opc)) == ((byte) 0x2))) ? ((string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("S") : (string) ((string) (((bool) ((((bool) (((byte) (size)) == ((byte) 0x1))) & ((bool) (((byte) (S)) == ((byte) 0x0)))))) ? ("D") : throw new NotImplementedException())))) : throw new NotImplementedException())))));
			var index = (uint) (opc switch { (byte) ((byte) 0x0) => (uint) ((uint) ((byte) ((byte) (((byte) (byte) (((byte) (((byte) (size)) << 0)) | ((byte) (((byte) (S)) << 2)))) | ((byte) (((byte) (Q)) << 3)))))), (byte) ((byte) 0x1) => (uint) ((uint) (((uint) ((uint) ((byte) ((byte) (((byte) (byte) (((byte) (((byte) (size)) << 0)) | ((byte) (((byte) (S)) << 2)))) | ((byte) (((byte) (Q)) << 3))))))) >> (int) ((byte) 0x1))), (byte) ((byte) 0x2) => (uint) ((uint) (((bool) (((byte) ((byte) (((size) & ((byte) ((byte) ((byte) 0x1))))))) == ((byte) 0x0))) ? (uint) ((uint) ((uint) ((byte) ((byte) (((byte) (((byte) (S)) << 0)) | ((byte) (((byte) (Q)) << 1))))))) : (uint) (Q))), _ => throw new NotImplementedException() });
			return "ST1-single-no-offset";
		}
		insn_272:
		if((insn & 0xBFFFF000) == 0x0C008000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ST2-multi-no-offset";
		}
		insn_273:
		if((insn & 0xBFE0F000) == 0x0C808000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var imm = (byte) ((byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x20) : (byte) ((byte) 0x10)));
			if(!((bool) (((byte) (rm)) == ((byte) 0x1F))))
				goto insn_274;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ST2-multi-postindex-immediate";
		}
		insn_274:
		if((insn & 0xBFE0F000) == 0x0C808000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) != ((byte) 0x1F))))
				goto insn_275;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ST2-multi-postindex-register";
		}
		insn_275:
		if((insn & 0xBFFFF000) == 0x0C004000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ST3-multi-no-offset";
		}
		insn_276:
		if((insn & 0xBFE0F000) == 0x0C804000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var imm = (byte) ((byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x30) : (byte) ((byte) 0x18)));
			if(!((bool) (((byte) (rm)) == ((byte) 0x1F))))
				goto insn_277;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ST3-multi-postindex-immediate";
		}
		insn_277:
		if((insn & 0xBFE0F000) == 0x0C804000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) != ((byte) 0x1F))))
				goto insn_278;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ST3-multi-postindex-register";
		}
		insn_278:
		if((insn & 0xBFFFF000) == 0x0C000000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var rt4 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x3))))) % ((byte) (byte) ((byte) 0x20)));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ST4-multi-no-offset";
		}
		insn_279:
		if((insn & 0xBFE0F000) == 0x0C800000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var rt4 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x3))))) % ((byte) (byte) ((byte) 0x20)));
			var imm = (byte) ((byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x40) : (byte) ((byte) 0x2B)));
			if(!((bool) (((byte) (rm)) == ((byte) 0x1F))))
				goto insn_280;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ST4-multi-postindex-immediate";
		}
		insn_280:
		if((insn & 0xBFE0F000) == 0x0C800000) {
			var Q = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var size = (insn >> 10) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rt2 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x1))))) % ((byte) (byte) ((byte) 0x20)));
			var rt3 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x2))))) % ((byte) (byte) ((byte) 0x20)));
			var rt4 = (byte) (((byte) (byte) ((byte) (((byte) (byte) (rt)) + ((byte) (byte) ((byte) 0x3))))) % ((byte) (byte) ((byte) 0x20)));
			if(!((bool) (((byte) (rm)) != ((byte) 0x1F))))
				goto insn_281;
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ST4-multi-postindex-register";
		}
		insn_281:
		if((insn & 0xBFFFFC00) == 0x889FFC00) {
			var size = (insn >> 30) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "STLR";
		}
		insn_282:
		if((insn & 0xFFFFFC00) == 0x089FFC00) {
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return "STLRB";
		}
		insn_283:
		if((insn & 0xFFFFFC00) == 0x489FFC00) {
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return "STLRH";
		}
		insn_284:
		if((insn & 0xBFE0FC00) == 0x8800FC00) {
			var size = (insn >> 30) & 0x1U;
			var rs = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "STLXR";
		}
		insn_285:
		if((insn & 0xFFE0FC00) == 0x0800FC00) {
			var rs = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return "STLXRB";
		}
		insn_286:
		if((insn & 0x7FC00000) == 0x28800000) {
			var size = (insn >> 31) & 0x1U;
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rd = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x3))));
			return "STP-postindex";
		}
		insn_287:
		if((insn & 0x7FC00000) == 0x29800000) {
			var size = (insn >> 31) & 0x1U;
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rd = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x3))));
			return "STP-preindex";
		}
		insn_288:
		if((insn & 0x7FC00000) == 0x29000000) {
			var size = (insn >> 31) & 0x1U;
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rd = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x3))));
			return "STP-signed-offset";
		}
		insn_289:
		if((insn & 0x3FC00000) == 0x2C800000) {
			var opc = (insn >> 30) & 0x3U;
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rd = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			var r = (string) (opc switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", (byte) ((byte) 0x2) => "Q", _ => throw new NotImplementedException() });
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) (opc switch { (byte) ((byte) 0x0) => (byte) 0x2, (byte) ((byte) 0x1) => (byte) ((byte) 0x3), (byte) ((byte) 0x2) => (byte) ((byte) 0x4), _ => throw new NotImplementedException() })));
			return "STP-simd-postindex";
		}
		insn_290:
		if((insn & 0x3FC00000) == 0x2D800000) {
			var opc = (insn >> 30) & 0x3U;
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rd = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			var r = (string) (opc switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", (byte) ((byte) 0x2) => "Q", _ => throw new NotImplementedException() });
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) (opc switch { (byte) ((byte) 0x0) => (byte) 0x2, (byte) ((byte) 0x1) => (byte) ((byte) 0x3), (byte) ((byte) 0x2) => (byte) ((byte) 0x4), _ => throw new NotImplementedException() })));
			return "STP-simd-preindex";
		}
		insn_291:
		if((insn & 0x3FC00000) == 0x2D000000) {
			var opc = (insn >> 30) & 0x3U;
			var imm = (insn >> 15) & 0x7FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rd = (insn >> 5) & 0x1FU;
			var rt1 = (insn >> 0) & 0x1FU;
			var r = (string) (opc switch { (byte) ((byte) 0x0) => "S", (byte) ((byte) 0x1) => "D", (byte) ((byte) 0x2) => "Q", _ => throw new NotImplementedException() });
			var simm = (long) (((long) (Math.SignExt<long>(imm, 7))) << (int) ((byte) (opc switch { (byte) ((byte) 0x0) => (byte) 0x2, (byte) ((byte) 0x1) => (byte) ((byte) 0x3), (byte) ((byte) 0x2) => (byte) ((byte) 0x4), _ => throw new NotImplementedException() })));
			return "STP-simd-signed-offset";
		}
		insn_292:
		if((insn & 0xBFE00C00) == 0xB8000400) {
			var size = (insn >> 30) & 0x1U;
			var imm = (insn >> 12) & 0x1FFU;
			var rd = (insn >> 5) & 0x1FU;
			var rs = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var simm = (long) (Math.SignExt<long>(imm, 9));
			return "STR-immediate-postindex";
		}
		insn_293:
		if((insn & 0xBFE00C00) == 0xB8000C00) {
			var size = (insn >> 30) & 0x1U;
			var imm = (insn >> 12) & 0x1FFU;
			var rd = (insn >> 5) & 0x1FU;
			var rs = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var simm = (long) (Math.SignExt<long>(imm, 9));
			return "STR-immediate-preindex";
		}
		insn_294:
		if((insn & 0xBFC00000) == 0xB9000000) {
			var size = (insn >> 30) & 0x1U;
			var imm = (insn >> 10) & 0xFFFU;
			var rd = (insn >> 5) & 0x1FU;
			var rs = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var pimm = (ulong) (((ulong) ((ulong) (imm))) << (int) ((byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x3))));
			return "STR-immediate-unsigned-offset";
		}
		insn_295:
		if((insn & 0xBFE00C00) == 0xB8200800) {
			var size = (insn >> 30) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var scale = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r1 = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var r2 = (string) (((bool) (((byte) (((option) & ((byte) ((byte) ((byte) 0x1)))))) != ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var amount = (byte) (((bool) (((byte) (scale)) == ((byte) 0x0))) ? (byte) ((byte) 0x0) : (byte) ((byte) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (byte) ((byte) 0x2) : (byte) ((byte) 0x3))));
			var extend = (string) (option switch { (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x6) => "SXTW", (byte) ((byte) 0x7) => "SXTX", (byte) ((byte) 0x3) => "LSL", _ => throw new NotImplementedException() });
			return "STR-register";
		}
		insn_296:
		if((insn & 0x3F600C00) == 0x3C000400) {
			var size = (insn >> 30) & 0x3U;
			var opc = (insn >> 23) & 0x1U;
			var imm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rop = (byte) ((byte) (((byte) (byte) (((byte) (((byte) ((byte) ((byte) ((byte) 0x0)))) << 0)) | ((byte) (((byte) (opc)) << 1)))) | ((byte) (((byte) (size)) << 2))));
			var r = (string) (rop switch { (byte) ((byte) 0x0) => "B", (byte) ((byte) 0x4) => "H", (byte) ((byte) 0x8) => "S", (byte) ((byte) 0xC) => "D", (byte) ((byte) 0x2) => "Q", _ => throw new NotImplementedException() });
			var simm = (long) (Math.SignExt<long>(imm, 9));
			return "STR-simd-postindex";
		}
		insn_297:
		if((insn & 0x3F600C00) == 0x3C000C00) {
			var size = (insn >> 30) & 0x3U;
			var opc = (insn >> 23) & 0x1U;
			var imm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rop = (byte) ((byte) (((byte) (byte) (((byte) (((byte) ((byte) ((byte) ((byte) 0x0)))) << 0)) | ((byte) (((byte) (opc)) << 1)))) | ((byte) (((byte) (size)) << 2))));
			var r = (string) (rop switch { (byte) ((byte) 0x0) => "B", (byte) ((byte) 0x4) => "H", (byte) ((byte) 0x8) => "S", (byte) ((byte) 0xC) => "D", (byte) ((byte) 0x2) => "Q", _ => throw new NotImplementedException() });
			var scale = (byte) ((byte) (((byte) (((byte) (size)) << 0)) | ((byte) (((byte) (opc)) << 2))));
			var simm = (long) (Math.SignExt<long>(imm, 9));
			return "STR-simd-preindex";
		}
		insn_298:
		if((insn & 0x3F400000) == 0x3D000000) {
			var size = (insn >> 30) & 0x3U;
			var opc = (insn >> 23) & 0x1U;
			var imm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rop = (byte) ((byte) (((byte) (byte) (((byte) (((byte) ((byte) ((byte) ((byte) 0x0)))) << 0)) | ((byte) (((byte) (opc)) << 1)))) | ((byte) (((byte) (size)) << 2))));
			var r = (string) (rop switch { (byte) ((byte) 0x0) => "B", (byte) ((byte) 0x4) => "H", (byte) ((byte) 0x8) => "S", (byte) ((byte) 0xC) => "D", (byte) ((byte) 0x2) => "Q", _ => throw new NotImplementedException() });
			var scale = (byte) ((byte) (((byte) (((byte) (size)) << 0)) | ((byte) (((byte) (opc)) << 2))));
			return "STR-simd-unsigned-offset";
		}
		insn_299:
		if((insn & 0x3F600C00) == 0x3C200800) {
			var size = (insn >> 30) & 0x3U;
			var opc = (insn >> 23) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var scale = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rop = (byte) ((byte) (((byte) (byte) (((byte) (((byte) ((byte) ((byte) ((byte) 0x0)))) << 0)) | ((byte) (((byte) (opc)) << 1)))) | ((byte) (((byte) (size)) << 2))));
			var r1 = (string) (rop switch { (byte) ((byte) 0x0) => "B", (byte) ((byte) 0x4) => "H", (byte) ((byte) 0x8) => "S", (byte) ((byte) 0xC) => "D", (byte) ((byte) 0x2) => "Q", _ => throw new NotImplementedException() });
			var r2 = (string) (((bool) (((byte) (((option) & ((byte) ((byte) ((byte) 0x1)))))) != ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var amount = (byte) (((bool) (((byte) (scale)) == ((byte) 0x0))) ? (byte) ((byte) 0x0) : (byte) ((byte) (size switch { (byte) ((byte) 0x1) => (byte) 0x1, (byte) ((byte) 0x2) => (byte) ((byte) 0x2), (byte) ((byte) 0x3) => (byte) ((byte) 0x3), _ => (byte) ((byte) (((bool) (((byte) (opc)) == ((byte) 0x1))) ? (byte) ((byte) 0x4) : (byte) ((byte) 0x0))) })));
			var extend = (string) (option switch { (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x6) => "SXTW", (byte) ((byte) 0x7) => "SXTX", (byte) ((byte) 0x3) => (string) (((bool) (((byte) (rop)) != ((byte) 0x0))) ? ("LSL") : throw new NotImplementedException()), _ => throw new NotImplementedException() });
			return "STR-simd-register";
		}
		insn_300:
		if((insn & 0xFFE00C00) == 0x38000400) {
			var imm = (insn >> 12) & 0x1FFU;
			var rd = (insn >> 5) & 0x1FU;
			var rs = (insn >> 0) & 0x1FU;
			var simm = (long) (Math.SignExt<long>(imm, 9));
			return "STRB-immediate-postindex";
		}
		insn_301:
		if((insn & 0xFFE00C00) == 0x38000C00) {
			var imm = (insn >> 12) & 0x1FFU;
			var rd = (insn >> 5) & 0x1FU;
			var rs = (insn >> 0) & 0x1FU;
			var simm = (long) (Math.SignExt<long>(imm, 9));
			return "STRB-immediate-preindex";
		}
		insn_302:
		if((insn & 0xFFC00000) == 0x39000000) {
			var imm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return "STRB-immediate-unsigned-offset";
		}
		insn_303:
		if((insn & 0xFFE00C00) == 0x38200800) {
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var amount = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (((option) & ((byte) ((byte) ((byte) 0x1)))))) != ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var str = (string) (option switch { (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x6) => "SXTW", (byte) ((byte) 0x7) => "SXTX", _ => throw new NotImplementedException() });
			return "STRB-register";
		}
		insn_304:
		if((insn & 0xFFE00C00) == 0x78000400) {
			var imm = (insn >> 12) & 0x1FFU;
			var rd = (insn >> 5) & 0x1FU;
			var rs = (insn >> 0) & 0x1FU;
			var simm = (long) (Math.SignExt<long>(imm, 9));
			return "STRH-immediate-postindex";
		}
		insn_305:
		if((insn & 0xFFE00C00) == 0x78000C00) {
			var imm = (insn >> 12) & 0x1FFU;
			var rd = (insn >> 5) & 0x1FU;
			var rs = (insn >> 0) & 0x1FU;
			var simm = (long) (Math.SignExt<long>(imm, 9));
			return "STRH-immediate-preindex";
		}
		insn_306:
		if((insn & 0xFFC00000) == 0x79000000) {
			var rawimm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var imm = (ushort) ((rawimm) << (int) ((byte) 0x1));
			return "STRH-immediate-unsigned-offset";
		}
		insn_307:
		if((insn & 0xFFE00C00) == 0x78200800) {
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var amount = (insn >> 12) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (((option) & ((byte) ((byte) ((byte) 0x1)))))) != ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var str = (string) (option switch { (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x6) => "SXTW", (byte) ((byte) 0x7) => "SXTX", _ => throw new NotImplementedException() });
			return "STRH-register";
		}
		insn_308:
		if((insn & 0xBFE00C00) == 0xB8000000) {
			var size = (insn >> 30) & 0x1U;
			var imm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var offset = (long) (Math.SignExt<long>(imm, 9));
			return "STUR";
		}
		insn_309:
		if((insn & 0x3F600C00) == 0x3C000000) {
			var size = (insn >> 30) & 0x3U;
			var opc = (insn >> 23) & 0x1U;
			var imm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var rop = (byte) ((byte) (((byte) (byte) (((byte) (((byte) ((byte) ((byte) ((byte) 0x0)))) << 0)) | ((byte) (((byte) (opc)) << 1)))) | ((byte) (((byte) (size)) << 2))));
			var r = (string) (rop switch { (byte) ((byte) 0x0) => "B", (byte) ((byte) 0x4) => "H", (byte) ((byte) 0x8) => "S", (byte) ((byte) 0xC) => "D", (byte) ((byte) 0x2) => "Q", _ => throw new NotImplementedException() });
			var simm = (long) (Math.SignExt<long>(imm, 9));
			return "STUR-simd";
		}
		insn_310:
		if((insn & 0xFFE00C00) == 0x38000000) {
			var imm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var offset = (long) (Math.SignExt<long>(imm, 9));
			return "STURB";
		}
		insn_311:
		if((insn & 0xFFE00C00) == 0x78000000) {
			var imm = (insn >> 12) & 0x1FFU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var offset = (long) (Math.SignExt<long>(imm, 9));
			return "STURH";
		}
		insn_312:
		if((insn & 0xFFE0FC00) == 0x08007C00) {
			var rs = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			return "STXRB";
		}
		insn_313:
		if((insn & 0xBFE0FC00) == 0x88007C00) {
			var size = (insn >> 30) & 0x1U;
			var rs = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "STXR";
		}
		insn_314:
		if((insn & 0xBFE08000) == 0x88200000) {
			var size = (insn >> 30) & 0x1U;
			var rs = (insn >> 16) & 0x1FU;
			var rt2 = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "STXP";
		}
		insn_315:
		if((insn & 0x7F800000) == 0x51000000) {
			var size = (insn >> 31) & 0x1U;
			var sh = (insn >> 22) & 0x1U;
			var imm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shift = (byte) (((bool) (((byte) (sh)) == ((byte) 0x0))) ? (byte) ((byte) 0x0) : (byte) ((byte) 0xC));
			return "SUB-immediate";
		}
		insn_316:
		if((insn & 0x7FE00000) == 0x4B200000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var imm = (insn >> 10) & 0x7U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) 0x4))))
				goto insn_317;
			var r1 = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var r2 = (string) (((bool) (((byte) ((byte) (((option) & ((byte) ((byte) ((byte) 0x3))))))) == ((byte) 0x3))) ? (string) ("X") : (string) ("W"));
			var extend = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ((string) (option switch { (byte) ((byte) 0x0) => "UXTB", (byte) ((byte) 0x1) => "UXTH", (byte) ((byte) 0x2) => "LSL", (byte) ((byte) 0x3) => "UXTX", (byte) ((byte) 0x4) => "SXTB", (byte) ((byte) 0x5) => "SXTH", (byte) ((byte) 0x6) => "SXTW", _ => "SXTX" })) : (string) ((string) (option switch { (byte) ((byte) 0x0) => "UXTB", (byte) ((byte) 0x1) => "UXTH", (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x4) => "SXTB", (byte) ((byte) 0x5) => "SXTH", (byte) ((byte) 0x6) => "SXTW", _ => "SXTX" })));
			return "SUB-extended-register";
		}
		insn_317:
		if((insn & 0x7F200000) == 0x4B000000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_318;
			if(!((bool) (((byte) (shift)) != ((byte) 0x3))))
				goto insn_318;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return "SUB-shifted-register";
		}
		insn_318:
		if((insn & 0x7FE00000) == 0x6B200000) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var option = (insn >> 13) & 0x7U;
			var imm = (insn >> 10) & 0x7U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) 0x4))))
				goto insn_319;
			var r1 = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var r2 = (string) (((bool) (((byte) ((byte) (((option) & ((byte) ((byte) ((byte) 0x3))))))) == ((byte) 0x3))) ? (string) ("X") : (string) ("W"));
			var extend = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ((string) (option switch { (byte) ((byte) 0x0) => "UXTB", (byte) ((byte) 0x1) => "UXTH", (byte) ((byte) 0x2) => "LSL", (byte) ((byte) 0x3) => "UXTX", (byte) ((byte) 0x4) => "SXTB", (byte) ((byte) 0x5) => "SXTH", (byte) ((byte) 0x6) => "SXTW", _ => "SXTX" })) : (string) ((string) (option switch { (byte) ((byte) 0x0) => "UXTB", (byte) ((byte) 0x1) => "UXTH", (byte) ((byte) 0x2) => "UXTW", (byte) ((byte) 0x3) => "LSL", (byte) ((byte) 0x4) => "SXTB", (byte) ((byte) 0x5) => "SXTH", (byte) ((byte) 0x6) => "SXTW", _ => "SXTX" })));
			return "SUBS-extended-register";
		}
		insn_319:
		if((insn & 0x7F200000) == 0x6B000000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var imm = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imm)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_320;
			if(!((bool) (((byte) (shift)) != ((byte) 0x3))))
				goto insn_320;
			var mode32 = (bool) (((byte) (size)) == ((byte) 0x0));
			var r = (string) ((mode32) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL", (byte) ((byte) 0x1) => "LSR", (byte) ((byte) 0x2) => "ASR", _ => "ROR" });
			return "SUBS-shifted-register";
		}
		insn_320:
		if((insn & 0x7F800000) == 0x71000000) {
			var size = (insn >> 31) & 0x1U;
			var shift = (insn >> 22) & 0x1U;
			var imm = (insn >> 10) & 0xFFFU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var shiftstr = (string) (shift switch { (byte) ((byte) 0x0) => "LSL #0", (byte) ((byte) 0x1) => "LSL #12", _ => throw new NotImplementedException() });
			return "SUBS-immediate";
		}
		insn_321:
		if((insn & 0xFFE0001F) == 0xD4000001) {
			var imm = (insn >> 5) & 0xFFFFU;
			return "SVC";
		}
		insn_322:
		if((insn & 0xFFF80000) == 0xD5080000) {
			var op1 = (insn >> 16) & 0x7U;
			var cn = (insn >> 12) & 0xFU;
			var cm = (insn >> 8) & 0xFU;
			var op2 = (insn >> 5) & 0x7U;
			var rt = (insn >> 0) & 0x1FU;
			return "SYS";
		}
		insn_323:
		if((insn & 0x7F000000) == 0x36000000) {
			var upper = (insn >> 31) & 0x1U;
			var bottom = (insn >> 19) & 0x1FU;
			var offset = (insn >> 5) & 0x3FFFU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (upper)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var imm = (uint) ((((uint) (((uint) ((uint) (upper))) << (int) ((byte) 0x5))) | ((uint) ((uint) (bottom)))));
			var addr = (ulong) (((ulong) (ulong) ((ulong) (pc))) + ((ulong) (long) ((long) (Math.SignExt<long>((ushort) (((ushort) ((ushort) (offset))) << (int) ((byte) 0x2)), 16)))));
			return "TBZ";
		}
		insn_324:
		if((insn & 0x7F000000) == 0x37000000) {
			var upper = (insn >> 31) & 0x1U;
			var bottom = (insn >> 19) & 0x1FU;
			var offset = (insn >> 5) & 0x3FFFU;
			var rt = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (upper)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			var imm = (uint) ((((uint) (((uint) ((uint) (upper))) << (int) ((byte) 0x5))) | ((uint) ((uint) (bottom)))));
			var addr = (ulong) (((ulong) (ulong) ((ulong) (pc))) + ((ulong) (long) ((long) (Math.SignExt<long>((ushort) (((ushort) ((ushort) (offset))) << (int) ((byte) 0x2)), 16)))));
			return "TBNZ";
		}
		insn_325:
		if((insn & 0xBF3FFC00) == 0x2E303800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (size switch { (byte) ((byte) 0x0) => "H", (byte) ((byte) 0x1) => "S", (byte) ((byte) 0x2) => "D", _ => throw new NotImplementedException() });
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x5) => "4S", _ => throw new NotImplementedException() });
			var esize = (byte) (((byte) 0x8) << (int) (size));
			var count = (byte) (((byte) (byte) ((byte) (((bool) ((Q) != ((byte) 0x0))) ? (byte) ((byte) 0x80) : (byte) ((byte) 0x40)))) / ((byte) (byte) (esize)));
			return "UADDLV";
		}
		insn_326:
		if((insn & 0xBF20FC00) == 0x2E201000) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var o2 = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("2") : (string) (""));
			var Ta = "";
			var Tb = "";
			switch((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1))))) {
				case (byte) ((byte) 0x0): {
					Ta = "8H";
					Tb = "8B";
					break;
				}
				case (byte) ((byte) 0x1): {
					Ta = "8H";
					Tb = "16B";
					break;
				}
				case (byte) ((byte) 0x2): {
					Ta = "4S";
					Tb = "4H";
					break;
				}
				case (byte) ((byte) 0x3): {
					Ta = "4S";
					Tb = "8H";
					break;
				}
				case (byte) ((byte) 0x4): {
					Ta = "2D";
					Tb = "2S";
					break;
				}
				case (byte) ((byte) 0x5): {
					Ta = "2D";
					Tb = "4S";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return "UADDW[2]";
		}
		insn_327:
		if((insn & 0x7F800000) == 0x53000000) {
			var size = (insn >> 31) & 0x1U;
			var N = (insn >> 22) & 0x1U;
			var immr = (insn >> 16) & 0x3FU;
			var imms = (insn >> 10) & 0x3FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			if(!((bool) (((byte) (imms)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_328;
			if(!((bool) (((byte) (immr)) <= ((byte) (((bool) ((size) != ((byte) 0x0))) ? (byte) ((byte) 0x3F) : (byte) ((byte) 0x1F))))))
				goto insn_328;
			if((bool) ((size) != ((byte) 0x0))) {
				if(!((bool) ((N) != ((byte) 0x0))))
					goto insn_328;
			} else {
				if(!((bool) (((byte) (N)) == ((byte) 0x0))))
					goto insn_328;
			}
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "UBFM";
		}
		insn_328:
		if((insn & 0x7F3FFC00) == 0x1E230000) {
			var size = (insn >> 31) & 0x1U;
			var type = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var st = (byte) ((byte) (((byte) (((byte) (type)) << 0)) | ((byte) (((byte) (size)) << 2))));
			var r1 = "";
			var r2 = "";
			switch(st) {
				case (byte) ((byte) 0x3): {
					r1 = "H";
					r2 = "W";
					break;
				}
				case (byte) ((byte) 0x0): {
					r1 = "S";
					r2 = "W";
					break;
				}
				case (byte) ((byte) 0x1): {
					r1 = "D";
					r2 = "W";
					break;
				}
				case (byte) ((byte) 0x7): {
					r1 = "H";
					r2 = "X";
					break;
				}
				case (byte) ((byte) 0x4): {
					r1 = "S";
					r2 = "X";
					break;
				}
				case (byte) ((byte) 0x5): {
					r1 = "D";
					r2 = "X";
					break;
				}
				default: {
					throw new NotImplementedException();
					break;
				}
			}
			return "UCVTF-scalar-gpr-integer";
		}
		insn_329:
		if((insn & 0xFFBFFC00) == 0x7E21D800) {
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("S") : (string) ("D"));
			return "UCVTF-scalar-integer";
		}
		insn_330:
		if((insn & 0xBFBFFC00) == 0x2E21D800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "2S", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x3) => "2D", _ => throw new NotImplementedException() });
			return "UCVTF-vector";
		}
		insn_331:
		if((insn & 0x7FE0FC00) == 0x1AC00800) {
			var size = (insn >> 31) & 0x1U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var r = (string) (((bool) (((byte) (size)) == ((byte) 0x0))) ? (string) ("W") : (string) ("X"));
			return "UDIV";
		}
		insn_332:
		if((insn & 0xFFE08000) == 0x9BA00000) {
			var rm = (insn >> 16) & 0x1FU;
			var ra = (insn >> 10) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			return "UMADDL";
		}
		insn_333:
		if((insn & 0xBF20F400) == 0x2E20A400) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var min = (insn >> 11) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var mnem = (string) (((bool) ((min) != ((byte) 0x0))) ? (string) ("uminp") : (string) ("umaxp"));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", _ => throw new NotImplementedException() });
			return "UMAXP-UMINP";
		}
		insn_334:
		if((insn & 0xBFE0FC00) == 0x0E003C00) {
			var Q = (insn >> 30) & 0x1U;
			var imm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var T = "";
			var index = (byte) 0x0;
			var r = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("X") : (string) ("W"));
			var comb = (byte) ((byte) (((byte) (((byte) (imm)) << 0)) | ((byte) (((byte) (Q)) << 5))));
			var size = (byte) (((bool) (((byte) ((byte) (((comb) & ((byte) ((byte) ((byte) 0x21))))))) == ((byte) 0x1))) ? (byte) ((byte) 0x8) : (byte) ((byte) (((bool) (((byte) ((byte) (((comb) & ((byte) ((byte) ((byte) 0x23))))))) == ((byte) 0x2))) ? (byte) ((byte) 0x10) : (byte) ((byte) (((bool) (((byte) ((byte) (((comb) & ((byte) ((byte) ((byte) 0x27))))))) == ((byte) 0x4))) ? (byte) ((byte) 0x20) : (byte) ((byte) (((bool) (((byte) ((byte) (((comb) & ((byte) ((byte) ((byte) 0x2F))))))) == ((byte) 0x28))) ? ((byte) 0x40) : throw new NotImplementedException())))))));
			switch(size) {
				case (byte) ((byte) 0x8): {
					T = "B";
					index = (byte) ((imm) >> (int) ((byte) 0x1));
					break;
				}
				case (byte) ((byte) 0x10): {
					T = "H";
					index = (byte) ((imm) >> (int) ((byte) 0x2));
					break;
				}
				case (byte) ((byte) 0x20): {
					T = "S";
					index = (byte) ((imm) >> (int) ((byte) 0x3));
					break;
				}
				default: {
					T = "D";
					index = (byte) ((imm) >> (int) ((byte) 0x4));
					break;
				}
			}
			return "UMOV";
		}
		insn_335:
		if((insn & 0xFFE0FC00) == 0x9BC07C00) {
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			return "UMULH";
		}
		insn_336:
		if((insn & 0xBF20FC00) == 0x2E204400) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var t = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "USHL-vector";
		}
		insn_337:
		if((insn & 0xBF80FC00) == 0x2F00A400) {
			var Q = (insn >> 30) & 0x1U;
			var immh = (insn >> 19) & 0xFU;
			var immb = (insn >> 16) & 0x7U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var Ta = "";
			var Tb = "";
			var size = (byte) 0x0;
			var shift = (byte) 0x0;
			if(!((bool) (((byte) (immh)) != ((byte) 0x0))))
				goto insn_338;
			var i2 = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("2") : (string) (""));
			if((bool) (((byte) (immh)) == ((byte) 0x1))) {
				Ta = "8H";
				Tb = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("16B") : (string) ("8B"));
				size = (byte) 0x0;
				shift = (byte) (((byte) (byte) ((byte) ((byte) ((byte) ((byte) (((byte) (((byte) (immb)) << 0)) | ((byte) (((byte) (immh)) << 3)))))))) - ((byte) (byte) ((byte) 0x8)));
			} else {
				if((bool) (((byte) ((byte) (((immh) & ((byte) ((byte) ((byte) 0xE))))))) == ((byte) 0x2))) {
					Ta = "4S";
					Tb = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("8H") : (string) ("4H"));
					size = (byte) 0x1;
					shift = (byte) (((byte) (byte) ((byte) ((byte) ((byte) ((byte) (((byte) (((byte) (immb)) << 0)) | ((byte) (((byte) (immh)) << 3)))))))) - ((byte) (byte) ((byte) 0x10)));
				} else {
					if((bool) (((byte) ((byte) (((immh) & ((byte) ((byte) ((byte) 0xC))))))) == ((byte) 0x4))) {
						Ta = "2D";
						Tb = (string) (((bool) ((Q) != ((byte) 0x0))) ? (string) ("4S") : (string) ("2S"));
						size = (byte) 0x2;
						shift = (byte) (((byte) (byte) ((byte) ((byte) ((byte) ((byte) (((byte) (((byte) (immb)) << 0)) | ((byte) (((byte) (immh)) << 3)))))))) - ((byte) (byte) ((byte) 0x20)));
					} else {
						throw new NotImplementedException();
					}
				}
			}
			return "USHLL-vector";
		}
		insn_338:
		if((insn & 0xFF3FFC00) == 0x0E212800) {
			var size = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var tb = (string) (size switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "4H", (byte) ((byte) 0x2) => "2S", _ => throw new NotImplementedException() });
			var ta = (string) (size switch { (byte) ((byte) 0x0) => "8H", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x2) => "2D", _ => throw new NotImplementedException() });
			return "XTN";
		}
		insn_339:
		if((insn & 0xFF3FFC00) == 0x4E212800) {
			var size = (insn >> 22) & 0x3U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var tb = (string) (size switch { (byte) ((byte) 0x0) => "16B", (byte) ((byte) 0x1) => "8H", (byte) ((byte) 0x2) => "4S", _ => throw new NotImplementedException() });
			var ta = (string) (size switch { (byte) ((byte) 0x0) => "8H", (byte) ((byte) 0x1) => "4S", (byte) ((byte) 0x2) => "2D", _ => throw new NotImplementedException() });
			return "XTN2";
		}
		insn_340:
		if((insn & 0xFFFFFFFF) == 0xD503203F) {
			return "YIELD";
		}
		insn_341:
		if((insn & 0xBF20BC00) == 0x0E003800) {
			var Q = (insn >> 30) & 0x1U;
			var size = (insn >> 22) & 0x3U;
			var rm = (insn >> 16) & 0x1FU;
			var op = (insn >> 14) & 0x1U;
			var rn = (insn >> 5) & 0x1FU;
			var rd = (insn >> 0) & 0x1FU;
			var i = (byte) ((byte) (((byte) (byte) (op)) + ((byte) (byte) ((byte) 0x1))));
			var T = (string) ((byte) ((byte) (((byte) (((byte) (Q)) << 0)) | ((byte) (((byte) (size)) << 1)))) switch { (byte) ((byte) 0x0) => "8B", (byte) ((byte) 0x1) => "16B", (byte) ((byte) 0x2) => "4H", (byte) ((byte) 0x3) => "8H", (byte) ((byte) 0x4) => "2S", (byte) ((byte) 0x5) => "4S", (byte) ((byte) 0x7) => "2D", _ => throw new NotImplementedException() });
			return "ZIP";
		}
		insn_342:

        return null;
    }

    public const int InstructionCount = 342 + 0;
}
