using JitBase;

namespace SharpStationCore; 

public unsafe partial class Recompiler : BaseCpu {
	readonly IJit<uint> Jit;
	readonly CpuState* StatePtr;
	IBuilder<uint> Builder;
	IStructRef<CpuState> State;

	delegate void BlockFunc(CpuState* state);
	
	public Recompiler(IJit<uint> jit, CpuState* state) {
		Jit = jit;
		StatePtr = state;
	}

	BlockFunc RecompileBlock(uint pc) => Jit.CreateFunction<BlockFunc>($"block_{pc:X8}", builder => {
		Builder = builder;
		State = Builder.StructRefArgument<CpuState>(0);
	});

	void Branch(uint pc) {}
	void Branch(IRuntimeValue<uint> pc) {}
	
	void DoLds() {
		var which = State.LdWhich();
		var iwhich = (IRuntimeValue<int>) which;
		Builder.When(iwhich != Builder.Zero<int>(), () =>
			State.Registers(iwhich, State.LdValue()));
		State.ReadAbsorb(iwhich, State.LdAbsorb());
		State.ReadFudge(which);
		State.ReadAbsorbWhich(State.ReadAbsorbWhich() | Builder.Ternary(which != Builder.LiteralValue(35U), which & Builder.LiteralValue(0x1FU), Builder.Zero<uint>()));
		State.LdWhich(Builder.LiteralValue(35U));
	}

	void DoLoad(uint reg, ref IRuntimeValue<uint> value) {
		var cond = (State.LdWhich() == Builder.LiteralValue(reg)).Store();
		Builder.If(cond,
			() => State.ReadFudge(Builder.Zero<uint>()), 
			DoLds);
		value = Builder.Ternary(cond, State.LdValue(), value);
	}

	void ThrowCpuException(ExceptionType etype, uint pc, uint insn) => throw new CpuException(etype, pc, insn);
	
	void AbsorbMuldivDelay() {}
	
	void MulDelay(IRuntimeValue<uint> a, IRuntimeValue<uint> b, bool signed) {}
	void DivDelay() {}
}