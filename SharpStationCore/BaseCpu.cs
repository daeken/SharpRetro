using static SharpStationCore.Globals;

namespace SharpStationCore; 

public interface ICoprocessor {
	uint this[uint register] { get; set; }
	void Call(uint func, uint inst);
}

public class BaseCpu {
	public uint Copreg(uint cop, uint reg) => cop switch {
		0 => CP0[reg], 
		//2 => CP2[reg], 
		_ => throw new NotSupportedException()
	};
	public void Copreg(uint cop, uint reg, uint value) {
		switch(cop) {
			case 0:
				CP0[reg] = value;
				break;
			default:
				throw new NotSupportedException();
		}
	}

	public uint Copcreg(uint cop, uint reg) => throw new NotImplementedException();
	public void Copcreg(uint cop, uint reg, uint value) => throw new NotImplementedException();

	public void Copfun(uint cop, uint command) => throw new NotImplementedException();

	public T ReadMemory<T>(uint addr) => (T) (object) (default(T) switch {
		byte => Memory.Load8(addr), 
		ushort => Memory.Load16(addr), 
		uint => Memory.Load32(addr), 
		_ => throw new NotSupportedException(),
	});
	public void WriteMemory<T>(uint addr, T value) {
		switch(value) {
			case byte v:
				Memory.Store8(addr, v);
				break;
			case ushort v:
				Memory.Store16(addr, v);
				break;
			case uint v:
				Memory.Store32(addr, v);
				break;
			default:
				throw new NotImplementedException();
		}
	}
}