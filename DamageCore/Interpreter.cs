namespace DamageCore; 

public partial class Interpreter {
	public byte[] Registers = new byte[8];

	public void WriteMemory<T>(ushort addr, T value) {
	}

	public T ReadMemory<T>(ushort addr) {
		return default;
	}
}