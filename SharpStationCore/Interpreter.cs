using static SharpStationCore.Globals;

namespace SharpStationCore; 

public unsafe partial class Interpreter : BaseCpu {
	public readonly CpuState* State;

	const uint NoBranch = ~0U;
	uint BranchTo = NoBranch, DeferBranch = NoBranch;

	public Interpreter(CpuState* state) => State = state;

	public void RunOne() {
		var pc = State->PC;
		var insn = Memory.Load32(pc);
		$"{pc:X8}: {Disassembler.Disassemble(insn, pc)}".Debug();
		
		BranchTo = NoBranch;
		Timestamp++;
		if(!Interpret(insn, pc))
			throw new($"Unknown instruction 0x{insn:X8} @ {pc:X8}");

		State->PC += 4;

		if(BranchTo != NoBranch && DeferBranch == NoBranch)
			DeferBranch = BranchTo;
		else if(DeferBranch != NoBranch) {
			State->PC = DeferBranch;
			DeferBranch = NoBranch;
		}
	}

	void Branch(uint target) => BranchTo = target;

	void AbsorbMuldivDelay() {}
	
	void MulDelay(uint a, uint b, bool signed) {}
	void DivDelay() {}

	void DoLds() {
		var which = State->LdWhich;
		if(which != 0) State->Registers[which] = State->LdValue;
		State->ReadAbsorb[which] = State->LdAbsorb;
		State->ReadFudge = which;
		State->ReadAbsorbWhich |= which != 35 ? which & 0x1F : 0;
		State->LdWhich = 35;
	}
	void DoLoad(uint reg, ref uint rval) {
		if(State->LdWhich == reg) {
			State->ReadFudge = 0;
			rval = State->LdValue;
		} else
			DoLds();
	}
}