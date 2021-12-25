// ReSharper disable CheckNamespace
// ReSharper disable ArrangeRedundantParentheses
// ReSharper disable RedundantCast
#pragma warning disable CS0164
namespace SharpStationCore;
using static LibSharpRetro.CpuHelpers.Math;

public unsafe partial class Interpreter {
    public bool Interpret(uint insn, uint pc) {
		/* ADDI */
		if((insn & 0xFC000000) == 0x20000000) {
			var rs = (insn >> 21) & 0x1FU;
			var rt = (insn >> 16) & 0x1FU;
			var imm = (insn >> 0) & 0xFFFFU;
			var eimm = (uint) ((uint) ((int) (SignExt<int>(imm, 16))));
			var lhs = (uint) ((rs) switch { 0 => 0U, var temp_0 => State->Registers[temp_0] });
			switch(rt) {
				case 0: break;
				case var temp_1: State->Registers[temp_1] = (uint) ((uint) (((uint) (uint) (lhs)) + ((uint) (uint) (eimm)))); break;
			}
			return true;
		}
		insn_1:

        return false;
    }
}
