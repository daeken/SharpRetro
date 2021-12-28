using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static SharpStationCore.Globals;

namespace SharpStationCore; 

public unsafe class CoreCpu {
	public CpuState* State;
	public uint IPCache;
	public bool IsolateCache;
	public uint BIU;

	public bool Halted;
	
	public readonly Interpreter Interpreter;

	public CoreCpu() {
		State = (CpuState*) Marshal.AllocHGlobal(Marshal.SizeOf<CpuState>());
		Unsafe.InitBlockUnaligned(State, 0, (uint) Marshal.SizeOf<CpuState>());
		State->PC = 0xBFC00000;
		Interpreter = new(State);
	}
	
	public void Run() {
		Interpreter.RunOne();
	}

	public void Invalidate(uint addr) {
	}
	
	public void RecalcIPCache() {
		IPCache = (CP0.StatusRegister & CP0.Cause & 0xFF00) != 0 && (CP0.StatusRegister & 1) != 0 || Halted
			? 0x80U
			: 0;
	}
}