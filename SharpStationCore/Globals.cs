namespace SharpStationCore; 

public static class Globals {
	public static ulong Timestamp;

	public static CoreCpu Cpu;
	public static CoreMemory Memory;

	public static Cop0 CP0;

	public static void Reset() {
		Cpu = new();
		Memory = new();
		CP0 = new();
	}
}