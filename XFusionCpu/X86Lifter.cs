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
        // Only the plain one-byte CC encoding is modeled here. Other software
        // interrupts, prefixes and delivery environments retain their own limits.
        if(LiftTables.Templates[tid].Mnemonic == "INT3") {
            if(mode is not (XMode.Bits16 or XMode.Bits32 or XMode.Bits64) ||
                d.Op != 0xcc || d.Len != 1 || d.P.Lock || d.P.Rep || d.P.RepNz ||
                d.P.OpSize || d.P.AdSize || d.P.Segment != 0 || d.P.Rex != 0 ||
                d.P.VexValid || d.P.EvexValid)
                throw new NotSupportedException("INT3 mode/prefix/encoding outside supported trap model");
            var next = unchecked(pc + 1);
            if(mode == XMode.Bits16) next &= 0xffff;
            else if(mode == XMode.Bits32) next &= 0xffffffff;
            return new IlBlock([new IlTrap("x86", 3,
                new IlConst(IlType.U64, pc), new IlConst(IlType.U64, next))]);
        }
        if(LiftTables.Templates[tid].Mnemonic == "IMUL1" &&
            (mode is not (XMode.Bits16 or XMode.Bits32 or XMode.Bits64) || d.P.Rep || d.P.RepNz))
            throw new NotSupportedException("IMUL1 mode/REP prefix outside supported ISA model");
		if(LiftTables.Templates[tid].Mnemonic == "DIV" &&
            (mode is not (XMode.Bits16 or XMode.Bits32 or XMode.Bits64) ||
             d.P.Lock || d.P.Rep || d.P.RepNz || d.P.VexValid || d.P.EvexValid))
            throw new NotSupportedException("DIV mode/prefix outside supported ISA model");
        var nearCall = IsNearCall(in d);
		var stackOp = LiftTables.Templates[tid].Mnemonic is "CALL" or "PUSH" or "POP" or "RET" or "RETI";
		var stackWidth = mode == XMode.Bits16 ? 16 : mode == XMode.Bits32 ? 32 : 64;
		if(nearCall && (d.P.Lock || d.P.RepNz))
			throw new NotSupportedException("LOCK/BND near CALL is outside the supported near-call model");
		var specs = SpecCache[d.DefId] ??= specsText.Length == 0 ? []
			: specsText.Split(' ').Select(OperandSpec.Parse).ToArray();

		var vw = nearCall && mode == XMode.Bits64 ? 64
			: d64 ? d.P.VWidthD64(mode) : d.P.VWidth(mode);

		var binds = new Dictionary<string, OperandBind>();
		var immSlot = 0;
		for(var k = 0; k < specs.Length && k < params_.Count; k++)
			binds[params_[k]] = Bind(specs[k], in d, mode, vw, pc, ref immSlot);
		if(nearCall && specs[0].Class == OpClass.RelBranch) {
			var abs = unchecked(pc + (ulong)d.Len + (ulong)d.Imm0);
			if(vw < 64) abs &= (1UL << vw) - 1;
			binds[params_[0]] = new OperandBind.Imm((long)abs, vw);
		}

		binds["%nextpc"] = new OperandBind.Imm((long) (pc + (ulong) d.Len), nearCall ? vw : 64);
		binds["%vexl"] = new OperandBind.Imm(d.P.VexL ? 1 : 0, 8);
		if(stackOp) binds["SP"] = new OperandBind.Reg(4, stackWidth);
		var opWidth = stackOp ? vw : specs.Length > 0 ? WidthOf(specs[0], in d, mode, vw) : vw;
		var block = IlLower.Lower(params_, eval, binds, opWidth,
			// API models a mode-default stack (no independent SS.B state).
			stackOp ? stackWidth : 64,
			stackOp && mode != XMode.Bits64 ? new IlReadReg(IlType.U64, RegKind.X86Seg, 2) : null,
			vex: d.P.VexValid);

		// rep/repe/repne on the string family → intrinsic-name prefix (·62 naming).
		// movs/stos/lods take plain rep; scas/cmps distinguish repe (F3) / repne (F2).
		var (rep, repNz) = (d.P.Rep, d.P.RepNz);
		if((rep || repNz) && block.Body.Any(st => st is IlIntrin))
			block = new IlBlock(block.Body.Select(st => st is IlIntrin(var ty, var nm, var args) && IsStringOp(nm)
				? new IlIntrin(ty, (repNz ? "repne_" : nm is "scas" or "cmps" ? "repe_" : "rep_") + nm, args.ToArray())
				: st).ToList());
		return block;
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
		// The `|| WCode.z` half is NOT redundant, and omitting it was the bug: the
		// DISASSEMBLER generator decides sign-extension with
		//     var sx = spec.SignExtended || spec.Width == WCode.z;   (DisassemblerGenerator.cs:425)
		// so a plain `Iz` (no -sx suffix, spec.SignExtended==false) IS extended at decode
		// — `Decode.ReadImm(..., p.ZWidth(mode), true)` then masked to VWidth. Binding it
		// at ZWidth afterwards truncates the extension straight back off.
		// Only divergent under REX.W (vw=64, ZWidth=32); in Bits32 and in Bits64 without
		// REX.W both are 32, which is why 59,925 rows passed before one didn't.
		// Found by XFReader: `add rax, 0xaaaaaaaa` (48 05 aa aa aa aa) gave
		// 0x00000000aaaaaaaa where silicon recorded 0xffffffffaaaaaaaa, and the lowered IL
		// read `(let %1 = (u32 #aaaaaaaa))` — bound at 32.
		// Same defect the Rust arm had 8 days ago (RustLiftGen.cs:858 passes op_w, not the
		// encoded width). Two arms, one rule, and the C# half kept the encoded width.
		_ when spec.SignExtended || spec.Width == WCode.z => vw,
		WCode.b => 8,
		WCode.w => 16,
		WCode.v => vw,
		WCode.y => d.P.YWidth(mode),
		WCode.z => d.P.ZWidth(mode),
		WCode.d => 32,
		WCode.q => 64,
		WCode.ss => 32, WCode.sd => 64,
		WCode.ps or WCode.pd or WCode.dq or WCode.x => d.P.VexL ? 256 : 128,
		WCode.o => 128,
		WCode.h => d.P.VexL ? 128 : 64,
		_ => vw
	};

	// CALL uses an Intel near-call model: long-mode 66 is ignored. Legacy
	// stack address size equals XMode; independent SS.B and faults are not modeled.
	internal static bool IsNearCall(in DecodedInsn d) =>
		LiftTables.Templates[LiftTables.Defs[d.DefId].Item1].Mnemonic == "CALL";

	internal static bool IsNearReturn(in DecodedInsn d) =>
		LiftTables.Templates[LiftTables.Defs[d.DefId].Item1].Mnemonic is "RET" or "RETI";

	static bool IsStringOp(string n) => n is "movs" or "stos" or "lods" or "scas" or "cmps";

	/// Legacy high-8 rule: 8-bit reg idx 4-7 WITHOUT any REX = AH/CH/DH/BH
	/// (bits 8-15 of gpr idx-4). Any REX (incl bare 40) switches to spl/bpl/sil/dil.
	static OperandBind GprBind(int idx, int w, in DecodedInsn d) =>
		w == 8 && idx is >= 4 and <= 7 && d.P.Rex == 0
			? new OperandBind.Reg(idx - 4, 8, High8: true)
			: new OperandBind.Reg(idx, w);

	static OperandBind Bind(OperandSpec spec, in DecodedInsn d, XMode mode, int vw, ulong pc, ref int immSlot) {
		var w = WidthOf(spec, in d, mode, vw);
		switch(spec.Class) {
			case OpClass.ModRmRm when d.M.IsReg:
				return GprBind(d.M.Rm, w, in d);
			case OpClass.ModRmRm:
				return new OperandBind.Mem(AddrExpr(in d, mode), w);
			case OpClass.ModRmReg:
				return GprBind(d.M.Reg, w, in d);
			case OpClass.OpcodeReg:
				return GprBind((d.Op & 7) | ((d.P.Rex & 1) << 3), w, in d);
			case OpClass.Imm:
				return new OperandBind.Imm(immSlot++ == 0 ? d.Imm0 : d.Imm1, w);
			case OpClass.RelBranch: {
				// resolve to ABSOLUTE at bind time (pc + len + rel, wrapped at mode IP
				// width) — the IL contract carries absolutes (aarch64 BL shape); raw
				// rels would couple every consumer to x86 encoding.
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
			// AH/CH/DH/BH are bits 15:8 of registers 0-3 (OperandSpec keeps their
			// encoding index 4-7); binding index 4 at width 8 named SPL.
			case OpClass.FixedReg when spec.FixedRegByte && spec.Text is "AH" or "CH" or "DH" or "BH":
				return new OperandBind.Reg(spec.FixedRegIndex - 4, 8, High8: true);
			case OpClass.FixedReg:
				return new OperandBind.Reg(spec.FixedRegIndex,
					spec.FixedRegByte ? 8 : spec.FixedRegVSized ? vw : 16);
			// These bind to their OWN register files. They used to bind to RegKind.X86
			// (the bind vocabulary had one file), which was survivable only while every
			// such template was intrinsic-bodied — the binds were dataflow placeholders
			// that never reached a real IlWriteReg. Once a template lowered to real IL
			// the alias became a silent GPR clobber: `movdqa xmm0, xmm1` wrote RAX.
			case OpClass.XmmReg:
			case OpClass.MmxReg:
				return new OperandBind.Reg(d.M.Reg, w, File: RegKind.Xmm);
			case OpClass.XmmRm or OpClass.XmmRmReg or OpClass.MmxRm when d.M.IsReg:
				return new OperandBind.Reg(d.M.Rm, w, File: RegKind.Xmm);
			case OpClass.XmmRm or OpClass.MmxRm:
				return new OperandBind.Mem(AddrExpr(in d, mode), w);
			case OpClass.XmmVvvv:
				return new OperandBind.Reg(d.P.VexVvvv, w, File: RegKind.Xmm);
			case OpClass.GprVvvv:
				return new OperandBind.Reg(d.P.VexVvvv, w);
			case OpClass.MaskReg:
				return new OperandBind.Reg(d.M.Reg, w, File: RegKind.Sys);
			case OpClass.MaskRm:
				return new OperandBind.Reg(d.M.Rm, w, File: RegKind.Sys);
			case OpClass.FpuTop:
				return new OperandBind.Reg(0, w, File: RegKind.St);
			case OpClass.FpuSti:
				return new OperandBind.Reg(d.M.Rm & 7, w, File: RegKind.St);
			case OpClass.StrSrc:
				return new OperandBind.Mem(new IlReadReg(IlType.U64, RegKind.X86, 6), w);  // [rSI]
			case OpClass.StrDst:
				return new OperandBind.Mem(new IlReadReg(IlType.U64, RegKind.X86, 7), w);  // [rDI]
			case OpClass.ModRmSeg:
				return new OperandBind.Reg(d.M.Reg, w, File: RegKind.X86Seg);
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
		// Effective offsets wrap at ADDRESS size, independently of operand width.
		// Do this after base+index*scale+signed displacement (including EIP-relative
		// forms), but BEFORE adding a segment base: FS/GS may be above 4 GiB.
		// Keep the address carrier u64; a narrow truncation followed by zext makes
		// the modulo operation explicit to every LiftIl consumer.
		var addressWidth = d.P.AWidth(mode);
		if(addressWidth < 64)
			e = new IlCast(IlType.U64, CastKind.Zext,
				new IlCast(new IlType.I(false, addressWidth), CastKind.Trunc, e));
		// segment-base context (64-bit: only fs/gs are live)
		if(d.P.Segment is 0x64 or 0x65 || (mode != XMode.Bits64 && d.P.Segment != 0)) {
			var segIdx = d.P.Segment switch { 0x26 => 0, 0x2E => 1, 0x36 => 2, 0x3E => 3, 0x64 => 4, 0x65 => 5, _ => 3 };
			e = new IlBin(IlType.U64, BinOp.Add, new IlReadReg(IlType.U64, RegKind.X86Seg, segIdx), e);
		}
		return e;
	}
}
