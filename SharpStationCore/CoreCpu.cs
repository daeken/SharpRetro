using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpStationCore; 

public unsafe class CoreCpu {
	public CpuState* State;
	public readonly Interpreter Interpreter;

	public CoreCpu() {
		State = (CpuState*) Marshal.AllocHGlobal(Marshal.SizeOf<CpuState>());
		Unsafe.InitBlockUnaligned(State, 0, (uint) Marshal.SizeOf<CpuState>());
		State->PC = 0xBFC00000;
		Interpreter = new Interpreter(State);
	}
	
	public void Run() {
		Interpreter.RunOne();
	}

	public void Invalidate(uint addr) {
	}
}