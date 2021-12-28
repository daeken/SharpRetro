using System.Diagnostics;
using static SharpStationCore.Globals;

namespace SharpStationCore; 

public class Cop0 : ICoprocessor {
	enum Reg {
		BPC      = 3,  // PC breakpoint address
		BDA      = 5,  // Data load/store breakpoint address
		TAR      = 6,  // Target address
		DCIC     = 7,  // Cache control
		BADVADDR = 8, // Bad virtual address
		BDAM     = 9,  // Data load/store address mask
		BPCM     = 11, // PC breakpoint address mask
		SR       = 12, 
		CAUSE    = 13, 
		EPC      = 14, 
		PRID     = 15  // Product ID
	}
	
	public uint BreakpointAddress, DataBreakpointAddress, TargetAddress, CacheControl, DataAddressMask, 
		BreakpointAddressMask, StatusRegister = (1 << 22) | (1 << 21), Cause, EPC;

	public uint this[uint register] {
		get {
			switch((Reg) register) {
				case Reg.BPC:
					return BreakpointAddress;
				case Reg.BDA:
					return DataBreakpointAddress;
				case Reg.TAR:
					return TargetAddress;
				case Reg.DCIC:
					return CacheControl;
				case Reg.BADVADDR:
					return 0;
				case Reg.BDAM:
					return DataAddressMask;
				case Reg.BPCM:
					return BreakpointAddressMask;
				case Reg.SR:
					//$"Reading SR {StatusRegister:X}".Debug();
					return StatusRegister;
				case Reg.CAUSE:
					return Cause;
				case Reg.EPC:
					return EPC;
				case Reg.PRID:
					return 2;
				default:
					throw new NotImplementedException($"Get for COP0 register {(Reg) register} not implemented");
			}
		}
		set {
			switch((Reg) register) {
				case Reg.BPC:
					BreakpointAddress = value;
					break;
				case Reg.BDA:
					DataBreakpointAddress = value;
					break;
				case Reg.TAR:
					TargetAddress = value;
					break;
				case Reg.DCIC:
					CacheControl = value & 0xFF80003F;
					break;
				case Reg.BDAM:
					DataAddressMask = value;
					break;
				case Reg.BPCM:
					BreakpointAddressMask = value;
					break;
				case Reg.SR:
					//$"Writing SR {value:X}".Debug();
					StatusRegister = value & ~((0x3U << 26) | (0x3 << 23) | (0x3 << 6));
					Cpu.IsolateCache = ((value >> 16) & 1) == 1;
					Cpu.RecalcIPCache();
					break;
				case Reg.CAUSE:
					Cause &= ~0x300U;
					Cause |= value & 0x300U;
					break;
				case Reg.EPC:
					throw new NotSupportedException("Assignment to COP0.EPC");
				case Reg.PRID:
					throw new NotSupportedException("Assignment to COP0.PRID");
				default:
					throw new NotImplementedException($"Set for COP0 register {(Reg) register} not implemented");
			}
		}
	}

	public void Copcreg(uint reg, uint value) => throw new NotImplementedException();
	public uint Copcreg(uint reg) => throw new NotImplementedException();

	public void Call(uint func) {
		Debug.Assert(func == 16); // RFE
		var mode = StatusRegister & 0x3FU;
		StatusRegister &= ~0xFU;
		StatusRegister |= mode >> 2;
	}
}
