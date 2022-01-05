using JitBase;

namespace SharpStationCore; 

public partial class Recompiler : BaseCpu {
	public Recompiler(IJit<uint> jit) {
		
	}

	void DoLds() {
	}

	void DoLoad(uint reg, ref IRuntimeValue<uint> value) {
	}

	void ThrowCpuException(ExceptionType etype, uint pc, uint insn) => throw new CpuException(etype, pc, insn);
	
	void Branch(uint pc) {}
	void Branch(IRuntimeValue<uint> pc) {}
	
	void AbsorbMuldivDelay() {}
	
	void MulDelay(IRuntimeValue<uint> a, IRuntimeValue<uint> b, bool signed) {}
	void DivDelay() {}
}