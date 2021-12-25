// ReSharper disable CheckNamespace
// ReSharper disable ArrangeRedundantParentheses
// ReSharper disable RedundantCast
#pragma warning disable CS0164
namespace SharpStationCore;

public partial class Disassembler {
    public static string Disassemble(uint insn, uint pc) {
		/* ADDI */
		if((insn & 0xFC000000) == 0x20000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			return (string) ("addi %" + (rt).ToString() + ", %" + (rs).ToString() + ", " + (string) ($"0x{(imm):x04}"));
		}
		insn_1:

        return null;
    }

    public static string ClassifyInstruction(uint insn) {
		if((insn & 0xFC000000) == 0x20000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			return "ADDI";
		}
		insn_1:

        return null;
    }

    public const int InstructionCount = 1 + 0;
}
