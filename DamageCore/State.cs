namespace DamageCore; 

public class State {
	public byte[] Registers = new byte[8];
	public ushort SP, PC;
	public byte Flags;
	public bool InterruptsEnabled, InterruptsEnableScheduled, InterruptsEnabledPending;

	public readonly Memory Memory;
	
	public State(Memory memory) => Memory = memory;
}