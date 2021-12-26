namespace SharpStationCore; 

public enum ExceptionType {
	INT  = 0, // Interrupt
	ADEL = 4, // Address error, Data load or Instruction fetch
	ADES = 5, // Address error, Data store
	// The address errors occur when attempting to read
	// outside of KUseg in user mode and when the address
	// is misaligned. (See also: BadVaddr register)
	IBE  = 6, // Bus error on Instruction fetch
	DBE  = 7, // Bus error on Data load/store
	Syscall , // Generated unconditionally by syscall instruction
	Break   , // Breakpoint - break instruction
	RI      , // Reserved instruction
	CPU     , // Coprocessor Unusable
	OV      , // Arithmetic overflow
}

public class CpuException : Exception {
	public readonly ExceptionType Type;
	public readonly uint PC, Inst;
		
	public CpuException(ExceptionType type, uint pc, uint inst) {
		Type = type;
		PC = pc;
		Inst = inst;
	}
}