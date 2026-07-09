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

	public bool RexW => (Rex & 8) != 0;
	public bool RexR => (Rex & 4) != 0;
	public bool RexX => (Rex & 2) != 0;
	public bool RexB => (Rex & 1) != 0;

	/// Effective operand width in bits for a v-sized operand.
	public int VWidth(XMode mode) => mode switch {
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
	/// Scan legacy prefixes + REX. Returns bytes consumed. REX must be the last
	/// prefix before the opcode — a legacy prefix after REX cancels it (SDM 2.2.1).
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
				default: return i;
			}
			i++;
		}
		return i;
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

	/// AT&T-free Intel-syntax memory operand rendering, XED-compatible shape.
	public static string MemOperandString(in ModRm m, in PrefixState p, XMode mode, int ptrBits, ulong nextRip) {
		var aw = p.AWidth(mode);
		var size = ptrBits switch {
			8 => "byte ptr ", 16 => "word ptr ", 32 => "dword ptr ", 64 => "qword ptr ",
			48 => "fword ptr ", 128 => "xmmword ptr ", 0 => "ptr ",  // 0 = address-only (LEA)
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
