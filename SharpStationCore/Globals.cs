namespace SharpStationCore; 

public static unsafe class Globals {
	public static ulong Timestamp => Cpu.State->Timestamp;

	public static EventSystem Events;
	public static CoreCpu Cpu;
	public static CoreMemory Memory;

	public static CoreIrq Irq;
	public static CoreDma Dma;
	public static Cop0 CP0;

	public static void Reset() {
		Events = new();
		Cpu = new();
		Irq = new();
		Dma = new();
		CP0 = new();
		Memory = new();
	}
}