namespace DamageCore; 

public partial class Interpreter {
	public readonly Core Core;
	public readonly State State;
	public readonly Timing Timing;
	ushort? BranchTo;

	public Interpreter(Core core, State state) {
		Core = core;
		Timing = Core.Timing;
		State = state;
	}

	public void RunOne() {
		var pc = State.PC;
		//Console.WriteLine($"Attempting to run from 0x{pc:X04}");
		var insns = State.Memory.ReadBlock(pc, 8);
		var dasm = Disassembler.Disassemble(insns, pc);
		if(dasm == null)
			throw new NotImplementedException($"Unsupported instruction at 0x{pc:X04}: {string.Join(' ', insns.ToArray().Select(n => $"{n:X02}"))}");
		//Console.WriteLine($"Disassembly: {dasm}    {Disassembler.ClassifyInstruction(insns)}");
		if(!Interpret(insns, ref pc))
			throw new NotImplementedException($"Interpretation failed at 0x{pc:X04} (or 0x{State.PC:X04} ?)");
		State.PC = BranchTo ?? pc;
		BranchTo = null;
	}

	public void WriteMemory<T>(ushort addr, T value) {
		switch(value) {
			case byte v:
				State.Memory.Write(addr, v);
				break;
			case ushort v:
				State.Memory.Write16(addr, v);
				break;
			default:
				throw new NotImplementedException();
		}
	}

	public T ReadMemory<T>(ushort addr) => default(T) switch {
		byte => (T) (object) State.Memory.Read(addr),
		ushort => (T) (object) State.Memory.Read16(addr),
		_ => throw new NotImplementedException()
	};

	public void Branch(ushort addr) => BranchTo = addr;

	public void AddCycles(ulong count) => Timing.AddCycles(count * 4);

	void Halt() => Core.Cpu.Halt();
}