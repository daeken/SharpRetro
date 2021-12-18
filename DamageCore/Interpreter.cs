namespace DamageCore; 

public partial class Interpreter {
	public byte[] Registers = new byte[8];
	public ushort SP;
	public byte Flags;
	public bool InterruptsEnabled, InterruptsEnableScheduled;

	public void WriteMemory<T>(ushort addr, T value) {
	}

	public T ReadMemory<T>(ushort addr) {
		return default;
	}

	public void Branch(ushort addr) {
	}
}