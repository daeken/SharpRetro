// ReSharper disable CheckNamespace
namespace SharpStationCore;
using static LibSharpRetro.CpuHelpers.Math;

public partial class Disassembler {
    public static string Disassemble(uint insn, uint pc) {
		/* ADD */
		if((insn & 0xFC00003F) == 0x00000020) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("add %" + (rd).ToString() + ", %" + (rs).ToString() + ", %" + (rt).ToString());
		}
		insn_1:
		/* ADDI */
		if((insn & 0xFC000000) == 0x20000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var eimm = (uint) (SignExt<uint>(imm, 16));
			return (string) ("addi %" + (rt).ToString() + ", %" + (rs).ToString() + ", " + (string) ($"0x{(eimm):x08}"));
		}
		insn_2:
		/* ADDIU */
		if((insn & 0xFC000000) == 0x24000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var eimm = (uint) (SignExt<uint>(imm, 16));
			return (string) ("addiu %" + (rt).ToString() + ", %" + (rs).ToString() + ", " + (string) ($"0x{(eimm):x08}"));
		}
		insn_3:
		/* ADDU */
		if((insn & 0xFC00003F) == 0x00000021) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("addu %" + (rd).ToString() + ", %" + (rs).ToString() + ", %" + (rt).ToString());
		}
		insn_4:
		/* AND */
		if((insn & 0xFC00003F) == 0x00000024) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("and %" + (rd).ToString() + ", %" + (rs).ToString() + ", %" + (rt).ToString());
		}
		insn_5:
		/* ANDI */
		if((insn & 0xFC000000) == 0x30000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			return (string) ("andi %" + (rt).ToString() + ", %" + (rs).ToString() + ", " + (string) ($"0x{(imm):x04}"));
		}
		insn_6:
		/* BEQ */
		if((insn & 0xFC000000) == 0x10000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			return (string) ("beq %" + (rs).ToString() + ", %" + (rt).ToString() + ", " + (string) ($"0x{(target):x08}"));
		}
		insn_7:
		/* BGEZ */
		if((insn & 0xFC110000) == 0x04010000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			return (string) ("bgez %" + (rs).ToString() + ", " + (string) ($"0x{(target):x08}"));
		}
		insn_8:
		/* BGEZAL */
		if((insn & 0xFC110000) == 0x04110000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			return (string) ("bgezal %" + (rs).ToString() + ", " + (string) ($"0x{(target):x08}"));
		}
		insn_9:
		/* BGTZ */
		if((insn & 0xFC1F0000) == 0x1C000000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			return (string) ("bgtz %" + (rs).ToString() + ", " + (string) ($"0x{(target):x08}"));
		}
		insn_10:
		/* BLEZ */
		if((insn & 0xFC1F0000) == 0x18000000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			return (string) ("blez %" + (rs).ToString() + ", " + (string) ($"0x{(target):x08}"));
		}
		insn_11:
		/* BLTZ */
		if((insn & 0xFC110000) == 0x04000000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			return (string) ("bltz %" + (rs).ToString() + ", " + (string) ($"0x{(target):x08}"));
		}
		insn_12:
		/* BLTZAL */
		if((insn & 0xFC110000) == 0x04100000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			return (string) ("bltzal %" + (rs).ToString() + ", " + (string) ($"0x{(target):x08}"));
		}
		insn_13:
		/* BNE */
		if((insn & 0xFC000000) == 0x14000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			return (string) ("bne %" + (rs).ToString() + ", %" + (rt).ToString() + ", " + (string) ($"0x{(target):x08}"));
		}
		insn_14:
		/* BREAK */
		if((insn & 0xFC00003F) == 0x0000000D) {
			var code = (insn >> 6) & 0xFFFFFU;
			return (string) ("break " + (code).ToString());
		}
		insn_15:
		/* CFC */
		if((insn & 0xF3E00000) == 0x40400000) {
			var cop = (insn >> 26) & 0x3U;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var cofun = (insn >> 0) & 0x7FFU;
			return (string) ("cfc" + (cop).ToString() + " %" + (rt).ToString() + ", " + (rd).ToString());
		}
		insn_16:
		/* COP */
		if((insn & 0xF2000000) == 0x42000000) {
			var cop = (insn >> 26) & 0x3U;
			var command = (insn >> 0) & 0x1FFFFFFU;
			return (string) ("cop" + (cop).ToString() + " " + (string) ($"0x{(command):x06}"));
		}
		insn_17:
		/* CTC */
		if((insn & 0xF3E00000) == 0x40C00000) {
			var cop = (insn >> 26) & 0x3U;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var cofun = (insn >> 0) & 0x7FFU;
			return (string) ("ctc" + (cop).ToString() + " %" + (rt).ToString() + ", " + (rd).ToString());
		}
		insn_18:
		/* DIV */
		if((insn & 0xFC00003F) == 0x0000001A) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("div %" + (rs).ToString() + ", %" + (rt).ToString());
		}
		insn_19:
		/* DIVU */
		if((insn & 0xFC00003F) == 0x0000001B) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("divu %" + (rs).ToString() + ", %" + (rt).ToString());
		}
		insn_20:
		/* J */
		if((insn & 0xFC000000) == 0x08000000) {
			var imm = (insn >> 0) & 0x3FFFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((((uint) ((pc + 4))) & (0xF0000000U))))) + ((uint) (uint) ((uint) (((uint) ((uint) (imm))) << (int) ((byte) 0x2)))));
			return (string) ("j " + (string) ($"0x{(target):x08}"));
		}
		insn_21:
		/* JAL */
		if((insn & 0xFC000000) == 0x0C000000) {
			var imm = (insn >> 0) & 0x3FFFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((((uint) ((pc + 4))) & (0xF0000000U))))) + ((uint) (uint) ((uint) (((uint) ((uint) (imm))) << (int) ((byte) 0x2)))));
			return (string) ("jal " + (string) ($"0x{(target):x08}"));
		}
		insn_22:
		/* JALR */
		if((insn & 0xFC00003F) == 0x00000009) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("jalr %" + (rd).ToString() + ", %" + (rs).ToString());
		}
		insn_23:
		/* JR */
		if((insn & 0xFC00003F) == 0x00000008) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("jr %" + (rs).ToString());
		}
		insn_24:
		/* LB */
		if((insn & 0xFC000000) == 0x80000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return (string) ("lb %" + (rt).ToString() + ", " + (string) ($"0x{(offset):x08}") + "(%" + (rs).ToString() + ")");
		}
		insn_25:
		/* LBU */
		if((insn & 0xFC000000) == 0x90000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return (string) ("lbu %" + (rt).ToString() + ", " + (string) ($"0x{(offset):x08}") + "(%" + (rs).ToString() + ")");
		}
		insn_26:
		/* LH */
		if((insn & 0xFC000000) == 0x84000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return (string) ("lh %" + (rt).ToString() + ", " + (string) ($"0x{(offset):x08}") + "(%" + (rs).ToString() + ")");
		}
		insn_27:
		/* LHU */
		if((insn & 0xFC000000) == 0x94000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return (string) ("lhu %" + (rt).ToString() + ", " + (string) ($"0x{(offset):x08}") + "(%" + (rs).ToString() + ")");
		}
		insn_28:
		/* LUI */
		if((insn & 0xFC000000) == 0x3C000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			return (string) ("lui %" + (rt).ToString() + ", " + (string) ($"0x{(imm):x04}"));
		}
		insn_29:
		/* LW */
		if((insn & 0xFC000000) == 0x8C000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return (string) ("lw %" + (rt).ToString() + ", " + (string) ($"0x{(offset):x08}") + "(%" + (rs).ToString() + ")");
		}
		insn_30:
		/* LWL */
		if((insn & 0xFC000000) == 0x88000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return (string) ("lwl %" + (rt).ToString() + ", " + (string) ($"0x{(offset):x08}") + "(%" + (rs).ToString() + ")");
		}
		insn_31:
		/* LWR */
		if((insn & 0xFC000000) == 0x98000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return (string) ("lwr %" + (rt).ToString() + ", " + (string) ($"0x{(offset):x08}") + "(%" + (rs).ToString() + ")");
		}
		insn_32:
		/* LWC2 */
		if((insn & 0xFC000000) == 0xC8000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return (string) ("lwc2 " + (rt).ToString() + ", " + (string) ($"0x{(offset):x08}") + "(%" + (rs).ToString() + ")");
		}
		insn_33:
		/* MFC */
		if((insn & 0xF3E00000) == 0x40000000) {
			var cop = (insn >> 26) & 0x3U;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var cofun = (insn >> 0) & 0x7FFU;
			return (string) ("mfc" + (cop).ToString() + " %" + (rt).ToString() + ", " + (rd).ToString());
		}
		insn_34:
		/* MFHI */
		if((insn & 0xFC00003F) == 0x00000010) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("mfhi %" + (rd).ToString());
		}
		insn_35:
		/* MFLO */
		if((insn & 0xFC00003F) == 0x00000012) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("mflo %" + (rd).ToString());
		}
		insn_36:
		/* MTC */
		if((insn & 0xF3E00000) == 0x40800000) {
			var cop = (insn >> 26) & 0x3U;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var cofun = (insn >> 0) & 0x7FFU;
			return (string) ("mtc" + (cop).ToString() + " %" + (rt).ToString() + ", " + (rd).ToString());
		}
		insn_37:
		/* MTHI */
		if((insn & 0xFC00003F) == 0x00000011) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("mthi %" + (rs).ToString());
		}
		insn_38:
		/* MTLO */
		if((insn & 0xFC00003F) == 0x00000013) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("mtlo %" + (rs).ToString());
		}
		insn_39:
		/* MULT */
		if((insn & 0xFC00003F) == 0x00000018) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("mult %" + (rs).ToString() + ", %" + (rt).ToString());
		}
		insn_40:
		/* MULTU */
		if((insn & 0xFC00003F) == 0x00000019) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("multu %" + (rs).ToString() + ", %" + (rt).ToString());
		}
		insn_41:
		/* NOR */
		if((insn & 0xFC00003F) == 0x00000027) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("nor %" + (rd).ToString() + ", %" + (rs).ToString() + ", %" + (rt).ToString());
		}
		insn_42:
		/* OR */
		if((insn & 0xFC00003F) == 0x00000025) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("or %" + (rd).ToString() + ", %" + (rs).ToString() + ", %" + (rt).ToString());
		}
		insn_43:
		/* ORI */
		if((insn & 0xFC000000) == 0x34000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			return (string) ("ori %" + (rt).ToString() + ", %" + (rs).ToString() + ", " + (string) ($"0x{(imm):x04}"));
		}
		insn_44:
		/* SB */
		if((insn & 0xFC000000) == 0xA0000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return (string) ("sb %" + (rt).ToString() + ", " + (string) ($"0x{(offset):x08}") + "(%" + (rs).ToString() + ")");
		}
		insn_45:
		/* SH */
		if((insn & 0xFC000000) == 0xA4000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return (string) ("sh %" + (rt).ToString() + ", " + (string) ($"0x{(offset):x08}") + "(%" + (rs).ToString() + ")");
		}
		insn_46:
		/* SLL */
		if((insn & 0xFC00003F) == 0x00000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("sll %" + (rd).ToString() + ", %" + (rt).ToString() + ", " + (shamt).ToString());
		}
		insn_47:
		/* SLLV */
		if((insn & 0xFC00003F) == 0x00000004) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("sllv %" + (rd).ToString() + ", %" + (rt).ToString() + ", %" + (rs).ToString());
		}
		insn_48:
		/* SLT */
		if((insn & 0xFC00003F) == 0x0000002A) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("slt %" + (rd).ToString() + ", %" + (rs).ToString() + ", %" + (rt).ToString());
		}
		insn_49:
		/* SLTI */
		if((insn & 0xFC000000) == 0x28000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var eimm = (int) (SignExt<int>(imm, 16));
			return (string) ("slti %" + (rt).ToString() + ", %" + (rs).ToString() + ", " + (string) ($"0x{(eimm):x08}"));
		}
		insn_50:
		/* SLTIU */
		if((insn & 0xFC000000) == 0x2C000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var eimm = (uint) (SignExt<uint>(imm, 16));
			return (string) ("sltiu %" + (rt).ToString() + ", %" + (rs).ToString() + ", " + (string) ($"0x{(eimm):x08}"));
		}
		insn_51:
		/* SLTU */
		if((insn & 0xFC00003F) == 0x0000002B) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("tltu %" + (rd).ToString() + ", %" + (rs).ToString() + ", %" + (rt).ToString());
		}
		insn_52:
		/* SRA */
		if((insn & 0xFC00003F) == 0x00000003) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("sra %" + (rd).ToString() + ", %" + (rt).ToString() + ", " + (shamt).ToString());
		}
		insn_53:
		/* SRAV */
		if((insn & 0xFC00003F) == 0x00000007) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("srav %" + (rd).ToString() + ", %" + (rt).ToString() + ", %" + (rs).ToString());
		}
		insn_54:
		/* SRL */
		if((insn & 0xFC00003F) == 0x00000002) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("srl %" + (rd).ToString() + ", %" + (rt).ToString() + ", " + (shamt).ToString());
		}
		insn_55:
		/* SRLV */
		if((insn & 0xFC00003F) == 0x00000006) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("srlv %" + (rd).ToString() + ", %" + (rt).ToString() + ", %" + (rs).ToString());
		}
		insn_56:
		/* SUB */
		if((insn & 0xFC00003F) == 0x00000022) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("sub %" + (rd).ToString() + ", %" + (rs).ToString() + ", %" + (rt).ToString());
		}
		insn_57:
		/* SUBU */
		if((insn & 0xFC00003F) == 0x00000023) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("subu %" + (rd).ToString() + ", %" + (rs).ToString() + ", %" + (rt).ToString());
		}
		insn_58:
		/* SW */
		if((insn & 0xFC000000) == 0xAC000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return (string) ("sw %" + (rt).ToString() + ", " + (string) ($"0x{(offset):x08}") + "(%" + (rs).ToString() + ")");
		}
		insn_59:
		/* SWC2 */
		if((insn & 0xFC000000) == 0xE8000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return (string) ("swc2 " + (rt).ToString() + ", " + (string) ($"0x{(offset):x08}") + "(%" + (rs).ToString() + ")");
		}
		insn_60:
		/* SWL */
		if((insn & 0xFC000000) == 0xA8000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return (string) ("swl %" + (rt).ToString() + ", " + (string) ($"0x{(offset):x08}") + "(%" + (rs).ToString() + ")");
		}
		insn_61:
		/* SWR */
		if((insn & 0xFC000000) == 0xB8000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return (string) ("swr %" + (rt).ToString() + ", " + (string) ($"0x{(offset):x08}") + "(%" + (rs).ToString() + ")");
		}
		insn_62:
		/* SYSCALL */
		if((insn & 0xFC00003F) == 0x0000000C) {
			var code = (insn >> 6) & 0xFFFFFU;
			return (string) ("syscall " + (code).ToString());
		}
		insn_63:
		/* XOR */
		if((insn & 0xFC00003F) == 0x00000026) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return (string) ("xor %" + (rd).ToString() + ", %" + (rs).ToString() + ", %" + (rt).ToString());
		}
		insn_64:
		/* XORI */
		if((insn & 0xFC000000) == 0x38000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			return (string) ("xori %" + (rt).ToString() + ", %" + (rs).ToString() + ", " + (string) ($"0x{(imm):x04}"));
		}
		insn_65:

        return null;
    }

    public static string ClassifyInstruction(uint insn) {
        var pc = 0xDEADBEEFU; // Should never be actually used
		if((insn & 0xFC00003F) == 0x00000020) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "ADD";
		}
		insn_1:
		if((insn & 0xFC000000) == 0x20000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var eimm = (uint) (SignExt<uint>(imm, 16));
			return "ADDI";
		}
		insn_2:
		if((insn & 0xFC000000) == 0x24000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var eimm = (uint) (SignExt<uint>(imm, 16));
			return "ADDIU";
		}
		insn_3:
		if((insn & 0xFC00003F) == 0x00000021) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "ADDU";
		}
		insn_4:
		if((insn & 0xFC00003F) == 0x00000024) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "AND";
		}
		insn_5:
		if((insn & 0xFC000000) == 0x30000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			return "ANDI";
		}
		insn_6:
		if((insn & 0xFC000000) == 0x10000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			return "BEQ";
		}
		insn_7:
		if((insn & 0xFC110000) == 0x04010000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			return "BGEZ";
		}
		insn_8:
		if((insn & 0xFC110000) == 0x04110000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			return "BGEZAL";
		}
		insn_9:
		if((insn & 0xFC1F0000) == 0x1C000000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			return "BGTZ";
		}
		insn_10:
		if((insn & 0xFC1F0000) == 0x18000000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			return "BLEZ";
		}
		insn_11:
		if((insn & 0xFC110000) == 0x04000000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			return "BLTZ";
		}
		insn_12:
		if((insn & 0xFC110000) == 0x04100000) {
			var rs = (insn >> 21) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			return "BLTZAL";
		}
		insn_13:
		if((insn & 0xFC000000) == 0x14000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((pc + 4)))) + ((uint) (uint) ((uint) (((uint) (SignExt<uint>(imm, 16))) << (int) ((byte) 0x2)))));
			return "BNE";
		}
		insn_14:
		if((insn & 0xFC00003F) == 0x0000000D) {
			var code = (insn >> 6) & 0xFFFFFU;
			return "BREAK";
		}
		insn_15:
		if((insn & 0xF3E00000) == 0x40400000) {
			var cop = (insn >> 26) & 0x3U;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var cofun = (insn >> 0) & 0x7FFU;
			return "CFC";
		}
		insn_16:
		if((insn & 0xF2000000) == 0x42000000) {
			var cop = (insn >> 26) & 0x3U;
			var command = (insn >> 0) & 0x1FFFFFFU;
			return "COP";
		}
		insn_17:
		if((insn & 0xF3E00000) == 0x40C00000) {
			var cop = (insn >> 26) & 0x3U;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var cofun = (insn >> 0) & 0x7FFU;
			return "CTC";
		}
		insn_18:
		if((insn & 0xFC00003F) == 0x0000001A) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "DIV";
		}
		insn_19:
		if((insn & 0xFC00003F) == 0x0000001B) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "DIVU";
		}
		insn_20:
		if((insn & 0xFC000000) == 0x08000000) {
			var imm = (insn >> 0) & 0x3FFFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((((uint) ((pc + 4))) & (0xF0000000U))))) + ((uint) (uint) ((uint) (((uint) ((uint) (imm))) << (int) ((byte) 0x2)))));
			return "J";
		}
		insn_21:
		if((insn & 0xFC000000) == 0x0C000000) {
			var imm = (insn >> 0) & 0x3FFFFFFU;
			var target = (uint) (((uint) (uint) ((uint) ((((uint) ((pc + 4))) & (0xF0000000U))))) + ((uint) (uint) ((uint) (((uint) ((uint) (imm))) << (int) ((byte) 0x2)))));
			return "JAL";
		}
		insn_22:
		if((insn & 0xFC00003F) == 0x00000009) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "JALR";
		}
		insn_23:
		if((insn & 0xFC00003F) == 0x00000008) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "JR";
		}
		insn_24:
		if((insn & 0xFC000000) == 0x80000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return "LB";
		}
		insn_25:
		if((insn & 0xFC000000) == 0x90000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return "LBU";
		}
		insn_26:
		if((insn & 0xFC000000) == 0x84000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return "LH";
		}
		insn_27:
		if((insn & 0xFC000000) == 0x94000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return "LHU";
		}
		insn_28:
		if((insn & 0xFC000000) == 0x3C000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			return "LUI";
		}
		insn_29:
		if((insn & 0xFC000000) == 0x8C000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return "LW";
		}
		insn_30:
		if((insn & 0xFC000000) == 0x88000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return "LWL";
		}
		insn_31:
		if((insn & 0xFC000000) == 0x98000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return "LWR";
		}
		insn_32:
		if((insn & 0xFC000000) == 0xC8000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return "LWC2";
		}
		insn_33:
		if((insn & 0xF3E00000) == 0x40000000) {
			var cop = (insn >> 26) & 0x3U;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var cofun = (insn >> 0) & 0x7FFU;
			return "MFC";
		}
		insn_34:
		if((insn & 0xFC00003F) == 0x00000010) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "MFHI";
		}
		insn_35:
		if((insn & 0xFC00003F) == 0x00000012) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "MFLO";
		}
		insn_36:
		if((insn & 0xF3E00000) == 0x40800000) {
			var cop = (insn >> 26) & 0x3U;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var cofun = (insn >> 0) & 0x7FFU;
			return "MTC";
		}
		insn_37:
		if((insn & 0xFC00003F) == 0x00000011) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "MTHI";
		}
		insn_38:
		if((insn & 0xFC00003F) == 0x00000013) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "MTLO";
		}
		insn_39:
		if((insn & 0xFC00003F) == 0x00000018) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "MULT";
		}
		insn_40:
		if((insn & 0xFC00003F) == 0x00000019) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "MULTU";
		}
		insn_41:
		if((insn & 0xFC00003F) == 0x00000027) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "NOR";
		}
		insn_42:
		if((insn & 0xFC00003F) == 0x00000025) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "OR";
		}
		insn_43:
		if((insn & 0xFC000000) == 0x34000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			return "ORI";
		}
		insn_44:
		if((insn & 0xFC000000) == 0xA0000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return "SB";
		}
		insn_45:
		if((insn & 0xFC000000) == 0xA4000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return "SH";
		}
		insn_46:
		if((insn & 0xFC00003F) == 0x00000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "SLL";
		}
		insn_47:
		if((insn & 0xFC00003F) == 0x00000004) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "SLLV";
		}
		insn_48:
		if((insn & 0xFC00003F) == 0x0000002A) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "SLT";
		}
		insn_49:
		if((insn & 0xFC000000) == 0x28000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var eimm = (int) (SignExt<int>(imm, 16));
			return "SLTI";
		}
		insn_50:
		if((insn & 0xFC000000) == 0x2C000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var eimm = (uint) (SignExt<uint>(imm, 16));
			return "SLTIU";
		}
		insn_51:
		if((insn & 0xFC00003F) == 0x0000002B) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "SLTU";
		}
		insn_52:
		if((insn & 0xFC00003F) == 0x00000003) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "SRA";
		}
		insn_53:
		if((insn & 0xFC00003F) == 0x00000007) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "SRAV";
		}
		insn_54:
		if((insn & 0xFC00003F) == 0x00000002) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "SRL";
		}
		insn_55:
		if((insn & 0xFC00003F) == 0x00000006) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "SRLV";
		}
		insn_56:
		if((insn & 0xFC00003F) == 0x00000022) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "SUB";
		}
		insn_57:
		if((insn & 0xFC00003F) == 0x00000023) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "SUBU";
		}
		insn_58:
		if((insn & 0xFC000000) == 0xAC000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return "SW";
		}
		insn_59:
		if((insn & 0xFC000000) == 0xE8000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return "SWC2";
		}
		insn_60:
		if((insn & 0xFC000000) == 0xA8000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return "SWL";
		}
		insn_61:
		if((insn & 0xFC000000) == 0xB8000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var offset = (int) (SignExt<int>(imm, 16));
			return "SWR";
		}
		insn_62:
		if((insn & 0xFC00003F) == 0x0000000C) {
			var code = (insn >> 6) & 0xFFFFFU;
			return "SYSCALL";
		}
		insn_63:
		if((insn & 0xFC00003F) == 0x00000026) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var rd = (insn >> 11) & 0x1FU;
			var shamt = (insn >> 6) & 0x1FU;
			return "XOR";
		}
		insn_64:
		if((insn & 0xFC000000) == 0x38000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			return "XORI";
		}
		insn_65:

        return null;
    }

    public const int InstructionCount = 65 + 0;
}
