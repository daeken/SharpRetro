using CoreArchCompiler;
using LiftIl;
using XFusionGenerator;

namespace XFusionCpu;

/// M1: lift(bytes) → IlBlock. DecodeInsn (the single decoder) → operand binds
/// (LiftTables spec rows + DecodedInsn fields) → IlLower (shared-source walker)
/// → the arch-neutral LiftIl tree Pagentry consumes.
///
/// Per-DefId caches: parsed eval forms + parsed spec lists (lazy, once).
/// Intrinsic-bodied templates lower to IlIntrin per the settled convention —
/// callers get an honest opaque node, real semantics land in the .isa over time.
public static class X86Lifter {
	static readonly (List<string> Params, List<PTree> Eval)?[] TemplateCache
		= new (List<string>, List<PTree>)?[LiftTables.Templates.Length];
	static readonly OperandSpec[][] SpecCache = new OperandSpec[LiftTables.Defs.Length][];

	public static IlBlock Lift(ReadOnlySpan<byte> code, ulong pc, XMode mode) {
		if(!Disassembler.DecodeInsn(code, mode, out var d)) return null;
		return Lift(in d, pc, mode);
	}

	public static IlBlock Lift(in DecodedInsn d, ulong pc, XMode mode) {
		var (tid, specsText, d64) = LiftTables.Defs[d.DefId];
		var (params_, eval) = Template(tid);
		var specs = SpecCache[d.DefId] ??= specsText.Length == 0 ? []
			: specsText.Split(' ').Select(OperandSpec.Parse).ToArray();

		var vw = d64 ? d.P.VWidthD64(mode) : d.P.VWidth(mode);

		var binds = new Dictionary<string, OperandBind>();
		var immSlot = 0;
		for(var k = 0; k < specs.Length && k < params_.Count; k++)
			binds[params_[k]] = Bind(specs[k], in d, mode, vw, pc, ref immSlot);

		binds["%nextpc"] = new OperandBind.Imm((long) (pc + (ulong) d.Len), 64);
		var opWidth = specs.Length > 0 ? WidthOf(specs[0], in d, mode, vw) : vw;
		return IlLower.Lower(params_, eval, binds, opWidth);
	}

	static (List<string> Params, List<PTree> Eval) Template(int tid) {
		if(TemplateCache[tid] is { } hit) return hit;
		var (_, paramsText, evalText) = LiftTables.Templates[tid];
		var ps = paramsText.Length == 0 ? new List<string>() : paramsText.Split(' ').ToList();
		// evalText = "(block form...)" — parse, take the block's forms.
		var block = (PList) ListParser.Parse(evalText)[0];
		var entry = (ps, block.Skip(1).ToList());
		TemplateCache[tid] = entry;
		return entry;
	}

	static int WidthOf(OperandSpec spec, in DecodedInsn d, XMode mode, int vw) => spec.Width switch {
		// sign-extended imms: the decoder already extended + masked Imm to the
		// v-sized destination — bind at dest width so template arith is width-clean.
		_ when spec.SignExtended => vw,
		WCode.b => 8,
		WCode.w => 16,
		WCode.v => vw,
		WCode.z => d.P.ZWidth(mode),
		WCode.d => 32,
		WCode.q => 64,
		WCode.ss => 32, WCode.sd => 64,
		WCode.ps or WCode.pd or WCode.dq or WCode.x => d.P.VexL ? 256 : 128,
		_ => vw
	};

	static OperandBind Bind(OperandSpec spec, in DecodedInsn d, XMode mode, int vw, ulong pc, ref int immSlot) {
		var w = WidthOf(spec, in d, mode, vw);
		switch(spec.Class) {
			case OpClass.ModRmRm when d.M.IsReg:
				return new OperandBind.Reg(d.M.Rm, w);
			case OpClass.ModRmRm:
				return new OperandBind.Mem(AddrExpr(in d, mode), w);
			case OpClass.ModRmReg:
				return new OperandBind.Reg(d.M.Reg, w);
			case OpClass.OpcodeReg:
				return new OperandBind.Reg((d.Op & 7) | ((d.P.Rex & 1) << 3), w);
			case OpClass.Imm:
				return new OperandBind.Imm(immSlot++ == 0 ? d.Imm0 : d.Imm1, w);
			case OpClass.RelBranch: {
				// resolve to ABSOLUTE at bind time (pc + len + rel, wrapped at mode IP
				// width) — the IL contract carries absolutes (aarch64 BL shape); raw
				// rels would couple every consumer to x86 encoding. barrow step-2a find.
				var rel = immSlot++ == 0 ? d.Imm0 : d.Imm1;
				var abs = pc + (ulong) d.Len + (ulong) rel;
				if(mode == XMode.Bits32) abs &= 0xFFFFFFFF;
				else if(mode == XMode.Bits16) abs &= 0xFFFF;
				return new OperandBind.Imm((long) abs, 64);
			}
			case OpClass.MemOffset: {  // moffs = MEMORY at seg:offset (A0-A3; DS unless overridden)
				Il a = new IlConst(IlType.U64, (ulong) (immSlot++ == 0 ? d.Imm0 : d.Imm1));
				var segIdx = d.P.Segment switch { 0x26 => 0, 0x2E => 1, 0x36 => 2, 0x3E => 3, 0x64 => 4, 0x65 => 5, _ => 3 };
				if(segIdx != 3 || mode != XMode.Bits64)  // 64-bit: only fs/gs live
					a = new IlBin(IlType.U64, BinOp.Add, new IlReadReg(IlType.U64, RegKind.X86Seg, segIdx), a);
				return new OperandBind.Mem(a, w);
			}
			case OpClass.FixedInt:
				return new OperandBind.Imm(spec.FixedValue, 8);
			case OpClass.FixedReg:
				return new OperandBind.Reg(spec.FixedRegIndex,
					spec.FixedRegByte ? 8 : spec.FixedRegVSized ? vw : 16);
			// xmm/mmx/mask/segment/x87/string operand binds land with the vector-IL
			// design conversation — their templates are intrinsic-bodied, and IlLower
			// reads intrinsic args through these binds as dataflow placeholders.
			case OpClass.XmmReg:
			case OpClass.MmxReg:
				return new OperandBind.Reg(d.M.Reg, w);
			case OpClass.XmmRm or OpClass.XmmRmReg or OpClass.MmxRm when d.M.IsReg:
				return new OperandBind.Reg(d.M.Rm, w);
			case OpClass.XmmRm or OpClass.MmxRm:
				return new OperandBind.Mem(AddrExpr(in d, mode), w);
			case OpClass.XmmVvvv or OpClass.GprVvvv:
				return new OperandBind.Reg(d.P.VexVvvv, w);
			case OpClass.MaskReg:
				return new OperandBind.Reg(d.M.Reg, w);
			case OpClass.MaskRm:
				return new OperandBind.Reg(d.M.Rm, w);
			case OpClass.FpuTop:
				return new OperandBind.Reg(0, w);
			case OpClass.FpuSti:
				return new OperandBind.Reg(d.M.Rm & 7, w);
			case OpClass.StrSrc:
				return new OperandBind.Mem(new IlReadReg(IlType.U64, RegKind.X86, 6), w);  // [rSI]
			case OpClass.StrDst:
				return new OperandBind.Mem(new IlReadReg(IlType.U64, RegKind.X86, 7), w);  // [rDI]
			case OpClass.ModRmSeg:
				return new OperandBind.Reg(d.M.Reg, w);
			case OpClass.FarPtr:
				return new OperandBind.Imm(immSlot++ == 0 ? d.Imm0 : d.Imm1, w);
			default:
				throw new NotSupportedException($"bind {spec.Class} ({spec.Text})");
		}
	}

	/// ModRm → address expression per the settled (base index scale disp) 4-tuple:
	/// base=-1 = absent; RIP-rel = IlReadPc + len + disp (pc is the INSN's pc);
	/// segment base (fs/gs in 64-bit) = RegKind.X86Seg read added in.
	static Il AddrExpr(in DecodedInsn d, XMode mode) {
		var m = d.M;
		Il e = null;
		if(m.RipRelative)
			e = new IlBin(IlType.U64, BinOp.Add, new IlReadPc(IlType.U64),
				new IlConst(IlType.U64, (ulong) d.Len));
		else if(m.BaseReg >= 0)
			e = new IlReadReg(IlType.U64, RegKind.X86, m.BaseReg);
		if(m.IndexReg >= 0) {
			Il idx = new IlReadReg(IlType.U64, RegKind.X86, m.IndexReg);
			if(m.Scale > 1)
				idx = new IlBin(IlType.U64, BinOp.Shl, idx,
					new IlConst(IlType.U64, (ulong) (m.Scale == 2 ? 1 : m.Scale == 4 ? 2 : 3)));
			e = e == null ? idx : new IlBin(IlType.U64, BinOp.Add, e, idx);
		}
		if(m.Disp != 0 || e == null) {
			var disp = new IlConst(IlType.U64, (ulong) (long) m.Disp);
			e = e == null ? disp : new IlBin(IlType.U64, BinOp.Add, e, disp);
		}
		// segment-base context (64-bit: only fs/gs are live)
		if(d.P.Segment is 0x64 or 0x65 || (mode != XMode.Bits64 && d.P.Segment != 0)) {
			var segIdx = d.P.Segment switch { 0x26 => 0, 0x2E => 1, 0x36 => 2, 0x3E => 3, 0x64 => 4, 0x65 => 5, _ => 3 };
			e = new IlBin(IlType.U64, BinOp.Add, new IlReadReg(IlType.U64, RegKind.X86Seg, segIdx), e);
		}
		return e;
	}
}
