namespace XFusionCpu;

/// x86 operating mode of the decoder instance.
public enum XMode { Bits16, Bits32, Bits64 }

/// Which opcode table an instruction lives in (escape-byte prefix).
public enum OpcodeMap { OneByte, TwoByte0F, ThreeByte0F38, ThreeByte0F3A }

/// Result of the shared prefix scan. One per decoded instruction.
public struct PrefixState {
	public bool OpSize;    // 0x66
	public bool AdSize;    // 0x67
	public bool Lock;      // 0xF0
	public bool RepNz;     // 0xF2
	public bool Rep;       // 0xF3
	public byte Segment;   // 0=none, else 0x26/0x2E/0x36/0x3E/0x64/0x65
	public byte Rex;       // 0 if none; else the raw REX byte (0x40-0x4F), 64-bit mode only
	                       // (VEX folds R̄X̄B̄W into a synthetic Rex so ModRM/GprName code is shared)
	public bool VexValid;  // C4/C5/62 seen — opcode map comes from VexMap, pp folded into OpSize/Rep/RepNz
	public byte VexMap;    // 1=0F, 2=0F38, 3=0F3A (mmmmm / EVEX.mm)
	public byte VexVvvv;   // 2nd-source register (already un-inverted; EVEX: 5 bits via V')
	public bool VexL;      // 0=xmm(128), 1=ymm(256) — EVEX uses VecLen instead
	public bool EvexValid; // 62 seen (implies VexValid)
	public byte VecLen;    // EVEX L'L: 0=128, 1=256, 2=512
	public byte EvexMask;  // aaa: 0=none, 1-7=k1-k7
	public bool EvexZ;     // zeroing-masking
	public bool EvexB;     // broadcast/rc/sae
	public bool EvexRp;    // R' — ModRM.reg bit 4

	public bool RexW => (Rex & 8) != 0;
	public bool RexR => (Rex & 4) != 0;
	public bool RexX => (Rex & 2) != 0;
	public bool RexB => (Rex & 1) != 0;

	/// Effective operand width in bits for a v-sized operand. Under VEX/EVEX the
	/// pp field folds into OpSize as a ROW SELECTOR only — v-operands there size
	/// by W alone (C5F96E = vmovd xmm, r32 not r16).
	public int VWidth(XMode mode) => VexValid
		? RexW ? 64 : 32
		: mode switch {
			XMode.Bits16 => OpSize ? 32 : 16,
			XMode.Bits32 => OpSize ? 16 : 32,
			_ => RexW ? 64 : OpSize ? 16 : 32,
		};

	/// z-sized: like v but capped at 32 (immediates in 64-bit ops are imm32 sign-extended).
	public int ZWidth(XMode mode) => Math.Min(VWidth(mode), 32);

	/// v-width for D64-class ops (PUSH/POP/CALL/JMP near, etc.): in 64-bit mode the
	/// default operand size is 64 and cannot be 32 (SDM Vol-2 D64/F64 attributes);
	/// 0x66 still selects 16.
	public int VWidthD64(XMode mode) => mode == XMode.Bits64 ? (OpSize ? 16 : 64) : VWidth(mode);

	/// Effective address width in bits.
	public int AWidth(XMode mode) => mode switch {
		XMode.Bits16 => AdSize ? 32 : 16,
		XMode.Bits32 => AdSize ? 16 : 32,
		_ => AdSize ? 32 : 64,
	};
}

/// Decoded ModRM (+SIB +disp) — the memory-or-register operand.
public struct ModRm {
	public byte Mod, Reg, Rm;      // raw fields (Reg/Rm REX-extended already)
	public bool IsReg;             // mod == 11
	// memory form:
	public sbyte BaseReg;          // GPR index or -1
	public sbyte IndexReg;         // GPR index or -1 (never 4/RSP)
	public byte Scale;             // 1/2/4/8
	public long Disp;
	public bool RipRelative;       // 64-bit mode mod=00 rm=101
}

public static class Decode {
	/// Scan legacy prefixes + REX + VEX. Returns bytes consumed. REX must be the last
	/// legacy-class prefix before the opcode — a legacy prefix after REX cancels it
	/// (SDM 2.2.1). VEX (C4/C5) terminates the scan — the opcode byte follows directly
	/// and no legacy/REX prefixes may precede... they MAY precede but combining with
	/// 66/F2/F3/REX/LOCK is #UD (SDM 2.3.2); we decode permissively and let pp rule.
	/// 32-bit mode: C4/C5 are LES/LDS unless the NEXT byte's top 2 bits are 11.
	public static int ScanPrefixes(ReadOnlySpan<byte> code, XMode mode, out PrefixState p) {
		p = default;
		var i = 0;
		while(i < code.Length) {
			var b = code[i];
			switch(b) {
				case 0x66: p.OpSize = true; p.Rex = 0; break;
				case 0x67: p.AdSize = true; p.Rex = 0; break;
				case 0xF0: p.Lock = true; p.Rex = 0; break;
				case 0xF2: p.RepNz = true; p.Rex = 0; break;
				case 0xF3: p.Rep = true; p.Rex = 0; break;
				case 0x26 or 0x2E or 0x36 or 0x3E or 0x64 or 0x65: p.Segment = b; p.Rex = 0; break;
				case >= 0x40 and <= 0x4F when mode == XMode.Bits64: p.Rex = b; break;
				case 0xC5 when i + 2 < code.Length && (mode == XMode.Bits64 || (code[i + 1] & 0xC0) == 0xC0): {
					// 2-byte VEX: [C5][R̄vvvvLpp] — map=0F, X̄=B̄=1(clear), W=0
					var b1 = code[i + 1];
					p.VexValid = true;
					p.VexMap = 1;
					p.VexVvvv = (byte) ((~b1 >> 3) & 0xF);
					p.VexL = (b1 & 4) != 0;
					p.Rex = (byte) (0x40 | ((b1 & 0x80) == 0 ? 4 : 0));  // R̄ inverted → REX.R
					ApplyVexPp((byte) (b1 & 3), ref p);
					return i + 2;
				}
				case 0x62 when i + 4 < code.Length && (mode == XMode.Bits64 || (code[i + 1] & 0xC0) == 0xC0): {
					// EVEX: [62][R̄X̄B̄R̄'00mm][Wvvvv1pp][zL'Lb V'aaa] (32-bit mode: 62=BOUND unless mod11)
					var e1 = code[i + 1];
					var e2 = code[i + 2];
					var e3 = code[i + 3];
					p.VexValid = true;
					p.EvexValid = true;
					p.VexMap = (byte) (e1 & 3);
					p.VexVvvv = (byte) ((((~e2 >> 3) & 0xF)) | ((e3 & 8) == 0 ? 16 : 0));  // V' inverted, bit 4
					p.VecLen = (byte) ((e3 >> 5) & 3);
					p.VexL = p.VecLen == 1;  // keep VexL coherent for shared render paths
					p.EvexMask = (byte) (e3 & 7);
					p.EvexZ = (e3 & 0x80) != 0;
					p.EvexB = (e3 & 0x10) != 0;
					p.EvexRp = (e1 & 0x10) == 0;  // R' inverted
					p.Rex = (byte) (0x40
						| ((e2 & 0x80) != 0 ? 8 : 0)      // W
						| ((e1 & 0x80) == 0 ? 4 : 0)      // R̄
						| ((e1 & 0x40) == 0 ? 2 : 0)      // X̄
						| ((e1 & 0x20) == 0 ? 1 : 0));    // B̄
					ApplyVexPp((byte) (e2 & 3), ref p);
					return i + 4;
				}
				case 0xC4 when i + 3 < code.Length && (mode == XMode.Bits64 || (code[i + 1] & 0xC0) == 0xC0): {
					// 3-byte VEX: [C4][R̄X̄B̄mmmmm][WvvvvLpp]
					var b1 = code[i + 1];
					var b2 = code[i + 2];
					p.VexValid = true;
					p.VexMap = (byte) (b1 & 0x1F);
					p.VexVvvv = (byte) ((~b2 >> 3) & 0xF);
					p.VexL = (b2 & 4) != 0;
					p.Rex = (byte) (0x40
						| ((b2 & 0x80) != 0 ? 8 : 0)      // W
						| ((b1 & 0x80) == 0 ? 4 : 0)      // R̄
						| ((b1 & 0x40) == 0 ? 2 : 0)      // X̄
						| ((b1 & 0x20) == 0 ? 1 : 0));    // B̄
					ApplyVexPp((byte) (b2 & 3), ref p);
					return i + 3;
				}
				default: return i;
			}
			i++;
		}
		return i;
	}

	static void ApplyVexPp(byte pp, ref PrefixState p) {
		// pp plays the mandatory-prefix role — fold into the same fields the
		// non-VEX dispatch already discriminates on.
		switch(pp) {
			case 1: p.OpSize = true; break;
			case 2: p.Rep = true; break;
			case 3: p.RepNz = true; break;
		}
	}

	/// Decode ModRM byte + SIB + displacement. Returns bytes consumed from `code`
	/// (which must start AT the ModRM byte).
	public static int ReadModRm(ReadOnlySpan<byte> code, XMode mode, in PrefixState p, out ModRm m) {
		m = default;
		var modrm = code[0];
		var i = 1;
		m.Mod = (byte) (modrm >> 6);
		m.Reg = (byte) (((modrm >> 3) & 7) | (p.RexR ? 8 : 0));
		m.Rm = (byte) ((modrm & 7) | (p.RexB ? 8 : 0));
		m.BaseReg = -1;
		m.IndexReg = -1;
		m.Scale = 1;

		if(m.Mod == 3) {
			m.IsReg = true;
			return i;
		}

		var aw = p.AWidth(mode);
		if(aw == 16) {
			// 16-bit addressing table (SDM 2.1.5 Table 2-1)
			var rm = modrm & 7;
			(m.BaseReg, m.IndexReg) = rm switch {
				0 => ((sbyte) 3, (sbyte) 6),   // BX+SI
				1 => ((sbyte) 3, (sbyte) 7),   // BX+DI
				2 => ((sbyte) 5, (sbyte) 6),   // BP+SI
				3 => ((sbyte) 5, (sbyte) 7),   // BP+DI
				4 => ((sbyte) 6, (sbyte) -1),  // SI
				5 => ((sbyte) 7, (sbyte) -1),  // DI
				6 => ((sbyte) 5, (sbyte) -1),  // BP (or disp16 if mod==00)
				_ => ((sbyte) 3, (sbyte) -1),  // BX
			};
			if(m.Mod == 0 && rm == 6) { m.BaseReg = -1; m.Disp = (short) (code[i] | (code[i + 1] << 8)); i += 2; }
			else if(m.Mod == 1) { m.Disp = (sbyte) code[i]; i += 1; }
			else if(m.Mod == 2) { m.Disp = (short) (code[i] | (code[i + 1] << 8)); i += 2; }
			return i;
		}

		// 32/64-bit addressing
		var rmLow = modrm & 7;
		if(rmLow == 4) {
			// SIB
			var sib = code[i++];
			m.Scale = (byte) (1 << (sib >> 6));
			var idx = ((sib >> 3) & 7) | (p.RexX ? 8 : 0);
			m.IndexReg = idx == 4 ? (sbyte) -1 : (sbyte) idx;  // index=100 (no REX.X) = none
			var bse = (sib & 7) | (p.RexB ? 8 : 0);
			if((sib & 7) == 5 && m.Mod == 0) {
				m.BaseReg = -1;  // disp32 base
				m.Disp = ReadI32(code, ref i);
			} else
				m.BaseReg = (sbyte) bse;
		} else if(rmLow == 5 && m.Mod == 0) {
			if(mode == XMode.Bits64) {
				m.RipRelative = true;
				m.Disp = ReadI32(code, ref i);
			} else {
				m.BaseReg = -1;
				m.Disp = ReadI32(code, ref i);
			}
		} else
			m.BaseReg = (sbyte) m.Rm;

		if(m.Mod == 1) m.Disp = (sbyte) code[i++];
		else if(m.Mod == 2) m.Disp = ReadI32(code, ref i);
		return i;
	}

	static int ReadI32(ReadOnlySpan<byte> code, ref int i) {
		var v = code[i] | (code[i + 1] << 8) | (code[i + 2] << 16) | (code[i + 3] << 24);
		i += 4;
		return v;
	}

	/// Mask a (possibly sign-extended) value to its effective width for rendering.
	public static ulong MaskToWidth(long v, int bits) =>
		bits >= 64 ? (ulong) v : (ulong) v & ((1UL << bits) - 1);

	public static long ReadImm(ReadOnlySpan<byte> code, ref int i, int bits, bool signExtend) {
		long v = 0;
		for(var b = 0; b < bits / 8; b++)
			v |= (long) code[i + b] << (b * 8);
		i += bits / 8;
		if(signExtend || bits < 64) {
			var shift = 64 - bits;
			if(signExtend) return (v << shift) >> shift;
		}
		return v;
	}

	static readonly string[] Reg64 = ["rax", "rcx", "rdx", "rbx", "rsp", "rbp", "rsi", "rdi", "r8", "r9", "r10", "r11", "r12", "r13", "r14", "r15"];
	static readonly string[] Reg32 = ["eax", "ecx", "edx", "ebx", "esp", "ebp", "esi", "edi", "r8d", "r9d", "r10d", "r11d", "r12d", "r13d", "r14d", "r15d"];
	static readonly string[] Reg16 = ["ax", "cx", "dx", "bx", "sp", "bp", "si", "di", "r8w", "r9w", "r10w", "r11w", "r12w", "r13w", "r14w", "r15w"];
	static readonly string[] Reg8Rex = ["al", "cl", "dl", "bl", "spl", "bpl", "sil", "dil", "r8b", "r9b", "r10b", "r11b", "r12b", "r13b", "r14b", "r15b"];
	static readonly string[] Reg8Legacy = ["al", "cl", "dl", "bl", "ah", "ch", "dh", "bh"];
	static readonly string[] SegNames = ["es", "cs", "ss", "ds", "fs", "gs"];

	public static string GprName(int idx, int bits, bool rexPresent) => bits switch {
		64 => Reg64[idx],
		32 => Reg32[idx],
		16 => Reg16[idx],
		8 => rexPresent || idx >= 8 ? Reg8Rex[idx] : Reg8Legacy[idx & 7],
		_ => throw new ArgumentOutOfRangeException(nameof(bits))
	};

	public static string SegRegName(int idx) => SegNames[idx & 7];

	public static string XmmName(int idx, bool ymm = false) => ymm ? $"ymm{idx}" : $"xmm{idx}";

	/// EVEX-aware vector reg name: VecLen 0/1/2 = xmm/ymm/zmm (idx 0-31).
	public static string VecName(int idx, int vecLen) => vecLen switch {
		2 => $"zmm{idx}", 1 => $"ymm{idx}", _ => $"xmm{idx}"
	};

	public static string MaskName(int idx) => $"k{idx & 7}";

	/// EVEX operand decoration: {k1} mask + {z} zeroing suffix on the destination.
	public static string EvexDecoration(in PrefixState p) =>
		p.EvexMask != 0 ? $"{{k{p.EvexMask}}}" + (p.EvexZ ? "{z}" : "") : "";
	public static string MmxName(int idx) => $"mm{idx & 7}";  // no REX extension for mmx

	/// Prefix text rendered before the mnemonic. Stray F3/F2 on non-string, non-SSE
	/// ops are dropped by XED (F3 C3 rep-ret renders plain 'ret'); string ops render
	/// rep through their own templates when they land. lock always renders.
	/// TODO(cet-tier): 3E on indirect branch = 'notrack ' (XED renders it; we drop —
	/// 2 corpus hits, accepted-mismatch class until CET vocabulary lands).
	public static string MnemonicPrefix(in PrefixState p) =>
		p.Lock ? "lock " : "";

	public static string SegPrefixName(byte seg) => seg switch {
		0x26 => "es", 0x2E => "cs", 0x36 => "ss", 0x3E => "ds", 0x64 => "fs", 0x65 => "gs",
		_ => ""
	};

	/// EVEX disp8*N (SDM 2.7.5): mod==01 disp8 is compressed — scale by tuple-N.
	/// Call after ReadModRm on EVEX defs; N per the def's tuple class (full-vector
	/// moves/logic: vector byte width; element tuples: element size).
	public static void ScaleDisp8(ref ModRm m, int n) {
		if(m.Mod == 1) m.Disp *= n;
	}

	/// moffs operand (A0-A3): absolute address, sized + segment-prefixed.
	public static string MoffsString(long addr, int ptrBits, in PrefixState p, XMode mode) {
		var size = ptrBits switch { 8 => "byte ptr ", 16 => "word ptr ", 32 => "dword ptr ", 64 => "qword ptr ", _ => "" };
		var segIgnored = mode == XMode.Bits64 && p.Segment is 0x26 or 0x2E or 0x36 or 0x3E;
		var seg = p.Segment != 0 && !segIgnored ? SegPrefixName(p.Segment) + ":" : "";
		return $"{size}{seg}[0x{(ulong) addr:x}]";
	}

	/// String-op implicit operand (X=DS:[rSI] reg=6, Y=ES:[rDI] reg=7). XED shape:
	/// 'movsb byte ptr [rdi], byte ptr [rsi]' — no segment shown in 64-bit mode.
	public static string StrOperand(int reg, int ptrBits, in PrefixState p, XMode mode) {
		var size = ptrBits switch {
			8 => "byte ptr ", 16 => "word ptr ", 32 => "dword ptr ", 64 => "qword ptr ", _ => ""
		};
		var seg = mode != XMode.Bits64 && p.Segment != 0 && reg == 6 ? SegPrefixName(p.Segment) + ":" : "";
		return $"{size}{seg}[{GprName(reg, p.AWidth(mode), false)}]";
	}

	/// AT&T-free Intel-syntax memory operand rendering, XED-compatible shape.
	public static string MemOperandString(in ModRm m, in PrefixState p, XMode mode, int ptrBits, ulong nextRip) {
		var aw = p.AWidth(mode);
		var size = ptrBits switch {
			8 => "byte ptr ", 16 => "word ptr ", 32 => "dword ptr ", 64 => "qword ptr ",
			48 => "fword ptr ", 128 => "xmmword ptr ", 256 => "ymmword ptr ", 512 => "zmmword ptr ", 0 => "ptr ",  // 0 = address-only (LEA)
			_ => ""
		};
		// ES/CS/SS/DS overrides are architecturally null in 64-bit mode (only FS/GS apply);
		// XED drops them from rendering (e.g. 66 2E 0F 1F = nop word ptr [rax+rax*1], no cs:).
		var segIgnored = mode == XMode.Bits64 && p.Segment is 0x26 or 0x2E or 0x36 or 0x3E;
		var seg = p.Segment != 0 && !segIgnored ? SegPrefixName(p.Segment) + ":" : "";
		if(m.RipRelative)
			return m.Disp < 0 ? $"{size}{seg}[rip-0x{-m.Disp:x}]" : $"{size}{seg}[rip+0x{m.Disp:x}]";
		var parts = new List<string>();
		if(m.BaseReg >= 0) parts.Add(GprName(m.BaseReg, aw, p.Rex != 0));
		if(m.IndexReg >= 0) parts.Add($"{GprName(m.IndexReg, aw, p.Rex != 0)}*{m.Scale}");  // XED prints *1 explicitly
		var body = string.Join("+", parts);
		if(m.Disp != 0 || parts.Count == 0)
			body += parts.Count == 0 ? $"0x{(ulong) m.Disp:x}" : m.Disp < 0 ? $"-0x{-m.Disp:x}" : $"+0x{m.Disp:x}";
		return $"{size}{seg}[{body}]";
	}
}
