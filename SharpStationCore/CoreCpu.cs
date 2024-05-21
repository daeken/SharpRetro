using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static SharpStationCore.Globals;

namespace SharpStationCore; 

public unsafe class CoreCpu {
	public CpuState* State;
	public uint IPCache;
	public bool IsolateCache;
	public uint BIU;

	public bool Halted;
	
	public readonly Interpreter Interpreter;

	public CoreCpu() {
		State = (CpuState*) Marshal.AllocHGlobal(Marshal.SizeOf<CpuState>());
		Unsafe.InitBlockUnaligned(State, 0, (uint) Marshal.SizeOf<CpuState>());
		State->PC = 0xBFC00000;
		Interpreter = new(State);
	}
	
	public void Run() {
		while(Core.Running && (Timestamp < Events.NextTimestamp || Events.RunEvents()))
			try {
				if(IPCache != 0) {
					if(Halted) {
					} else if((CP0.StatusRegister & 1) != 0) {
						Dispatch(new(ExceptionType.INT, State->PC, 0));
						continue;
					}
				}
				Interpreter.RunOne();
			} catch(CpuException ce) {
				Dispatch(ce);
			}
	}

	void Dispatch(CpuException exc) {
		$"Dispatching exception -- {exc.Type} {exc.PC:X8}".Debug();
		var handler = 0x80000080;

		if((CP0.StatusRegister & (1 << 22)) != 0) // BEV
			handler = 0xBFC00180;

		CP0.EPC = exc.PC;

		// "Push" IEc and KUc(so that the new IEc and KUc are 0)
		CP0.StatusRegister = (CP0.StatusRegister & ~0x3FU) | ((CP0.StatusRegister << 2) & 0x3FU);
			
		// Setup cause register
		CP0.Cause &= 0x0000FF00;
		CP0.Cause |= (uint) exc.Type << 2;

		CP0.Cause |= (exc.Inst << 2) & (3 << 28); // CE
			
		RecalcIPCache();

		State->PC = handler;
	}

	public void Invalidate(uint addr) {
	}
	
	public void RecalcIPCache() {
		IPCache = (CP0.StatusRegister & CP0.Cause & 0xFF00) != 0 && (CP0.StatusRegister & 1) != 0 || Halted
			? 0x80U
			: 0;
	}
	
	public void AssertIrq(bool asserted) {
		const uint mask = 1U << 10;
		CP0.Cause &= ~mask;
		if(asserted)
			CP0.Cause |= mask;

		RecalcIPCache();
	}
	
	string TtyBuf = "";
	public uint Intercept(uint pc) {
		string ReadString(uint addr) {
			var data = "";
			while(true) {
				var c = (char) Memory.Load8(addr++);
				if(c == '\0') break;
				data += c;
			}
			return data;
		}

		string Format(string fmt) {
			var pi = 5;
			uint GetArg() => pi <= 7 ? State->Registers[pi++] : Memory.Load32((uint) (State->Registers[29] + 0x10 + (pi++ - 8) * 4));

			var ret = "";
			for(var i = 0; i < fmt.Length; ++i) {
				if(fmt[i] != '%') {
					ret += fmt[i];
					continue;
				}

				i++;
				var length = -1;
				while(fmt[i] >= '0' && fmt[i] <= '9')
					length = length == -1 ? fmt[i++] - '0' : length * 10 + (fmt[i++] - '0');
				switch(fmt[i]) {
					case 'd': case 'u':
						ret += GetArg().ToString().PadLeft(length == -1 ? 0 : length, '0');
						break;
					case 's':
						ret += ReadString(GetArg());
						break;
					case 'x':
						ret += GetArg().ToString("x").PadLeft(length == -1 ? 0 : length, '0');
						break;
					case 'X':
						ret += GetArg().ToString("X").PadLeft(length == -1 ? 0 : length, '0');
						break;
					default:
						throw new NotImplementedException($"Unknown format char '{fmt[i]}' in '{fmt}'");
				}
			}
			return ret;
		}
			
		switch(pc & 0x0FFFFFFF) {
			case 0x2C94 when State->Registers[4] == 1:
				TtyBuf += string.Join("", Enumerable.Range(0, (int) State->Registers[6]).Select(i => (char) Memory.Load8((uint) (State->Registers[5] + i))));
				if(TtyBuf.Contains('\n')) {
					var lines = TtyBuf.Split('\n');
					TtyBuf = lines.Last();
					foreach(var line in lines.SkipLast(1)) {
						$"TTY: {line}".Debug();
						//if(line.Contains("SCUS_949"))
						//	DebugMemory = true;
					}
				}
				break;
			case 0x138EC:
				$"Print: {Format(ReadString(State->Registers[4])).Trim()}".Debug();
				return State->Registers[31];
		}
		return pc;
	}
}