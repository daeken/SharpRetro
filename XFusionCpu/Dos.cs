using System.Text;

namespace XFusionCpu;

/// The M4 acceptance vehicle (sera: "could we run DOS?"): load a .COM, shim
/// int 21h, run until exit. Deliberately tiny — the point is byte-exact
/// known-good programs exercising the interpreter, not DOS completeness.
public class DosMachine {
	public readonly X86Machine M;
	public readonly StringBuilder Output = new();
	public int? ExitCode;

	public DosMachine(byte[] com, ushort loadSeg = 0x100) {
		M = new X86Machine { Mode = XMode.Bits16, Mem = new byte[1 << 20] };
		// segments: CS=DS=ES=SS=loadSeg (all point at the one 64K image segment)
		for(var s = 0; s < 6; s++) { M.SegSel[s] = loadSeg; M.SegBase[s] = (ulong) loadSeg << 4; }
		var baseLin = (int) ((ulong) loadSeg << 4);
		com.CopyTo(M.Mem, baseLin + 0x100);
		// PSP essentials: int-20 at PSP:0 (CD 20), empty command tail at 0x80
		M.Mem[baseLin] = 0xCD; M.Mem[baseLin + 1] = 0x20;
		M.Mem[baseLin + 0x80] = 0;
		M.Ip = 0x100;
		M.Gpr[4] = 0xFFFE;  // SP
		// .COM convention: word 0 pushed (ret-to-PSP:0 = int 20 exit)
		M.Mem[baseLin + 0xFFFE] = 0; M.Mem[baseLin + 0xFFFF] = 0;
		M.OnIntrin = Intrin;
	}

	bool Intrin(X86Machine m, string name, ulong[] args) {
		switch(name) {
			case "int" when args[0] == 0x21: return Int21();
			case "int" when args[0] == 0x20: ExitCode = 0; m.Halt(); return true;
			case "int": return false;  // unshimmed interrupt = honest stop
			// string ops arrive as intrinsics; DOS-tier programs use movs/stos —
			// implemented here over machine state (DF honored, CX rep count is in
			// the intrinsic's REP variants... v1: single-shot forms only).
			default: return false;
		}
	}

	bool Int21() {
		var ah = (byte) (M.Gpr[0] >> 8);
		switch(ah) {
			case 0x02:  // print char in DL
				Output.Append((char) (byte) M.Gpr[2]);
				return true;
			case 0x09: {  // print $-terminated string at DS:DX
				var a = M.SegBase[3] + (M.Gpr[2] & 0xFFFF);
				for(var i = 0; i < 0xFFFF; i++) {
					var c = M.Mem[(int) (a + (ulong) i)];
					if(c == (byte) '$') break;
					Output.Append((char) c);
				}
				M.Gpr[0] = (M.Gpr[0] & ~0xFFUL) | (byte) '$';  // AL = '$' per DOS
				return true;
			}
			case 0x4C:  // exit, AL = code
				ExitCode = (int) (M.Gpr[0] & 0xFF);
				M.Halt();
				return true;
			default:
				return false;  // unshimmed function = honest stop
		}
	}

	/// Run to exit / unshimmed-stop / step cap. Returns true on clean exit.
	public bool Run(int maxSteps = 1_000_000) {
		for(var i = 0; i < maxSteps; i++) {
			if(ExitCode != null) return true;
			if(!M.Step()) return ExitCode != null;
		}
		return false;
	}
}
