using CoreArchCompiler;

namespace XFusionGenerator;

public enum OpClass {
	ModRmRm,     // E* / M* — r/m side of ModRM (register or memory)
	ModRmReg,    // G* — reg side of ModRM
	ModRmSeg,    // Sw — segment register from ModRM.reg
	Imm,         // I* — immediate follows
	RelBranch,   // J* — signed displacement relative to next insn
	FixedReg,    // AL, CL, rAX, DX ... — no bytes consumed
	FixedInt,    // literal constant operand (e.g. "1" in shift-by-1 forms)
	OpcodeReg,   // Z* — GPR embedded in opcode low 3 bits (+r forms), REX.B-extended
	StrSrc,      // X* — string-op source DS:[rSI] (rSI = si/esi/rsi by address size)
	StrDst,      // Y* — string-op dest ES:[rDI]
	MemOffset,   // O* — moffs: address-sized immediate offset, no ModRM (A0-A3)
	FarPtr,      // Ap — ptr16:16/16:32 direct far address
}

/// Operand width code per SDM Appendix A.2.
public enum WCode {
	b,     // byte
	w,     // word
	v,     // 16/32/64 by effective operand size
	z,     // 16/32 (64-bit opsize still uses 32) — imm and some regs
	d,     // dword
	q,     // qword
	p,     // 32/48-bit far pointer
	none,  // width not applicable (fixed regs carry their own)
}

public class OperandSpec {
	public string Text;          // as written in the .isa: "Eb", "Gv", "rAX", "/0"...
	public OpClass Class;
	public WCode Width;
	public bool MemOnly;         // M* — mod==11 is invalid
	public bool SignExtended;    // Ib-sx (0x83 family): imm8 sign-extended to v
	public int FixedRegIndex;    // for FixedReg: GPR index (rAX=0, rCX=1, ...)
	public bool FixedRegByte;    // AL/CL/DL/BL: byte view of the fixed reg
	public bool FixedRegVSized;  // rAX..rDI: v-sized view (16/32/64)
	public long FixedValue;      // for FixedInt
	public string SegName;       // for fixed segment-reg operands (ES/CS/SS/DS/FS/GS)

	public bool NeedsModRm => Class is OpClass.ModRmRm or OpClass.ModRmReg or OpClass.ModRmSeg;

	static readonly Dictionary<string, int> FixedGpr = new() {
		["AL"] = 0, ["CL"] = 1, ["DL"] = 2, ["BL"] = 3,
		["AH"] = 4, ["CH"] = 5, ["DH"] = 6, ["BH"] = 7,
		["rAX"] = 0, ["rCX"] = 1, ["rDX"] = 2, ["rBX"] = 3,
		["rSP"] = 4, ["rBP"] = 5, ["rSI"] = 6, ["rDI"] = 7,
		["AX"] = 0, ["DX"] = 2,  // 16-bit-exact forms (in/out use DX)
	};
	static readonly HashSet<string> SegRegs = ["ES", "CS", "SS", "DS", "FS", "GS"];

	public static OperandSpec Parse(PTree t) {
		switch(t) {
			case PInt(var v):
				return new() { Text = v.ToString(), Class = OpClass.FixedInt, FixedValue = v, Width = WCode.none };
			case PName(var name):
				return Parse(name);
			default:
				throw new NotSupportedException($"operand spec: {t}");
		}
	}

	public static OperandSpec Parse(string s) {
		var spec = new OperandSpec { Text = s };

		if(SegRegs.Contains(s)) {
			spec.Class = OpClass.FixedReg;
			spec.SegName = s;
			spec.Width = WCode.none;
			return spec;
		}
		if(FixedGpr.TryGetValue(s, out var idx)) {
			spec.Class = OpClass.FixedReg;
			spec.FixedRegIndex = idx;
			spec.FixedRegByte = s.Length == 2 && (s[1] is 'L' or 'H') && s[0] != 'r';
			spec.FixedRegVSized = s[0] == 'r';
			spec.Width = spec.FixedRegByte ? WCode.b : spec.FixedRegVSized ? WCode.v : WCode.w;
			if(s is "AH" or "CH" or "DH" or "BH") spec.FixedRegIndex = idx;  // high-byte views
			return spec;
		}

		var sx = s.EndsWith("-sx");
		var core = sx ? s[..^3] : s;
		if(core.Length is < 2 or > 2) throw new NotSupportedException($"operand spec: {s}");
		var (cls, w) = (core[0], core[1]);
		spec.SignExtended = sx;
		spec.Width = w switch {
			'b' => WCode.b, 'w' => WCode.w, 'v' => WCode.v, 'z' => WCode.z,
			'd' => WCode.d, 'q' => WCode.q, 'p' => WCode.p,
			_ => throw new NotSupportedException($"width code in {s}")
		};
		spec.Class = cls switch {
			'E' => OpClass.ModRmRm,
			'M' => OpClass.ModRmRm,
			'G' => OpClass.ModRmReg,
			'S' => OpClass.ModRmSeg,
			'I' => OpClass.Imm,
			'J' => OpClass.RelBranch,
			'O' => OpClass.MemOffset,
			'A' => OpClass.FarPtr,
			'Z' => OpClass.OpcodeReg,
			'X' => OpClass.StrSrc,
			'Y' => OpClass.StrDst,
			_ => throw new NotSupportedException($"operand class in {s}")
		};
		spec.MemOnly = cls == 'M';
		return spec;
	}
}
