namespace SharpStationCore; 

public static class Globals {
	public static ulong Timestamp;

	public static CoreCpu Cpu;
	public static CoreMemory Memory;

	public static void Reset() {
		Cpu = new();
		Memory = new();
	}
}