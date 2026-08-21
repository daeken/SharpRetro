using XFusionCpu;

namespace XFusionTests;

/// M4: the eval bodies EXECUTE for the first time. Flag values are hand-derived
/// from the SDM (and cross-checkable against qemu when the harness lands — until
/// then these are the semantics goldens, † source-derived).
[TestFixture]
public class ExecTests {
	static X86Machine M64(string hex, ulong pc = 0x1000) {
		var m = new X86Machine { Mode = XMode.Bits64, Mem = new byte[0x20000], Ip = pc };
		Convert.FromHexString(hex).CopyTo(m.Mem, (int) pc);
		return m;
	}

	const int CF = 0, PF = 2, AF = 4, ZF = 6, SF = 7, OF = 11;
	static bool F(X86Machine m, int bit) => ((m.Flags >> bit) & 1) != 0;

	// --- SSE scalar float: EXECUTION, not just lowering ---
	// A corpus census over 11.3M insns of real .text put MULSS/ADDSS/SUBSS/COMISS at
	// 93,637 insns = 56% of everything IlLower couldn't lower. Those now lift (the
	// float-conversion cluster) — but lifting is not executing: this evaluator had
	// ZERO F-aware sites and its Cast arm threw on SToF, so the cluster would have
	// been lift-clean and exec-dead. These tests are the arm that separates those.
	//
	// The carrier convention is MaxwellEval's (Il.cs's shared IL, MaxwellEval:119):
	// Eval returns ulong, so an F-typed value rides as its IEEE BIT PATTERN. Hence
	// the expectations below are bit-exact rather than approximate.
	static X86Machine MF(string hex, uint xa, uint xb) {
		var m = M64(hex);
		// XMM operands live in the XMM file. They USED to bind to RegKind.X86 (the
		// bind vocabulary had no File field) so an xmm write clobbered a GPR -- see
		// XmmWriteDoesNotClobberGpr, which is the regression arm for that.
		m.Xmm[0] = xa; m.Xmm[1] = xb;
		return m;
	}

	[Test]
	public void XmmWriteDoesNotClobberGpr() {   // 66 0F 6F C1 = movdqa xmm0, xmm1
		// THE REGRESSION ARM. Before OperandBind.Reg carried a File, every xmm/mmx/
		// mask/x87/seg operand bound to RegKind.X86 -- so this instruction wrote RAX.
		// Survivable while those templates were intrinsic-bodied (their binds were
		// dataflow placeholders that never reached a real IlWriteReg); the float
		// cluster made 14 heads lower to real IL and the alias went live.
		var m = M64("660f6fc1");
		m.Gpr[0] = 0xAAAAAAAAAAAAAAAA; m.Gpr[1] = 0xBBBBBBBBBBBBBBBB;
		m.Xmm[0] = 0x1111; m.Xmm[1] = 0x2222;
		m.Step();
		Assert.That(m.Gpr[0], Is.EqualTo(0xAAAAAAAAAAAAAAAA), "RAX must be UNTOUCHED");
		Assert.That(m.Gpr[1], Is.EqualTo(0xBBBBBBBBBBBBBBBB), "RCX must be UNTOUCHED");
		Assert.That(m.Xmm[0], Is.EqualTo((UInt128) 0x2222UL), "xmm0 := xmm1");
		Assert.That(m.Xmm[1], Is.EqualTo((UInt128) 0x2222UL), "xmm1 unchanged");
	}
	static uint Fb(float f) => BitConverter.SingleToUInt32Bits(f);

	[Test]
	public void PackedWideningAndPackExecute() {
		// PMULUDQ (vmulw), PMADDWD (vmadd), PACKSSDW (vpacks). All three were on my
		// "genuinely needs a new primitive" list and none of them did: a widening multiply
		// is IlCast's own type plus a halved lane count, a multiply-then-pairwise-add is
		// two sext-mul pairs and an add, and a saturating narrow is the mask-then-blend
		// clamp the PMAX/PMIN lowering already uses.
		//
		// PMULUDQ 66 0F F4: r[i:64] = zext(a[2i:32]) * zext(b[2i:32]). The discriminator
		// is a product that OVERFLOWS 32 bits -- 0x10000 * 0x10000 = 2^32, which a
		// same-width multiply would give as 0. Also 0xFFFFFFFF^2 to catch a SIGNED widen.
		var m1 = M64("660ff4c1");
		m1.Xmm[0] = (UInt128) 0x00010000UL | ((UInt128) 0xFFFFFFFFUL << 64);
		m1.Xmm[1] = (UInt128) 0x00010000UL | ((UInt128) 0xFFFFFFFFUL << 64);
		Assert.That(m1.Step(), Is.True, "pmuludq did not step");
		Assert.That((ulong) m1.Xmm[0], Is.EqualTo(0x100000000UL), "pmuludq lane0 = 2^32");
		Assert.That((ulong) (m1.Xmm[0] >> 64), Is.EqualTo(0xFFFFFFFE00000001UL),
			"pmuludq lane1 must be UNSIGNED (a signed widen gives 1)");

		// PMADDWD 66 0F F5: r[i:32] = a[2i]*b[2i] + a[2i+1]*b[2i+1], SIGNED 16x16.
		// Discriminator: a NEGATIVE word. -2 * 3 + 4 * 5 = 14; an unsigned read of
		// 0xFFFE would give 0xFFFE*3 + 20 = 196,634.
		var m2 = M64("660ff5c1");
		m2.Xmm[0] = (UInt128) 0x0004FFFEUL;   // words lo->hi: FFFE(-2), 0004
		m2.Xmm[1] = (UInt128) 0x00050003UL;   // words lo->hi: 0003,    0005
		Assert.That(m2.Step(), Is.True, "pmaddwd did not step");
		Assert.That((uint) m2.Xmm[0], Is.EqualTo(14u), "pmaddwd -2*3 + 4*5 = 14");

		// PACKSSDW 66 0F 6B: signed saturate 32->16, low half from dst, high from src.
		// Discriminators are the two SATURATION directions plus one in-range value:
		//   0x00010000 (65536) -> 0x7FFF     0xFFFF0000 (-65536) -> 0x8000     5 -> 5
		var m3 = M64("660f6bc1");
		m3.Xmm[0] = (UInt128) 0x00000005_00010000UL | ((UInt128) 0xFFFF0000UL << 64);
		m3.Xmm[1] = 0;
		Assert.That(m3.Step(), Is.True, "packssdw did not step");
		var got = (ulong) m3.Xmm[0];
		Assert.That(got & 0xFFFF, Is.EqualTo(0x7FFFUL), "packssdw +65536 saturates to 0x7FFF");
		Assert.That((got >> 16) & 0xFFFF, Is.EqualTo(5UL), "packssdw 5 passes through");
		Assert.That((got >> 32) & 0xFFFF, Is.EqualTo(0x8000UL), "packssdw -65536 saturates to 0x8000");
	}

	[Test]
	public void PtestFlagsExecute() {
		// PTEST a, b: ZF = ((a & b) == 0), CF = ((a & ~b) == 0), and AF/OF/PF/SF = 0.
		// Fully declarative from generic heads that already lower -- (&), (~), (==) --
		// so this needed no head at all, only the .isa row.
		//
		// THE THREE CASES ARE CHOSEN SO ZF AND CF DISAGREE, because an implementation
		// that computed one and copied it to the other would pass any test where they
		// happen to match:
		//   a=0x0F b=0xF0   a&b = 0     -> ZF=1     a&~b = 0x0F -> CF=0
		//   a=0x0F b=0x0F   a&b = 0x0F  -> ZF=0     a&~b = 0    -> CF=1
		//   a=0x0F b=0x01   a&b = 0x01  -> ZF=0     a&~b = 0x0E -> CF=0
		var cases = new (UInt128 a, UInt128 b, bool zf, bool cf)[] {
			(0x0F, 0xF0, true,  false),
			(0x0F, 0x0F, false, true),
			(0x0F, 0x01, false, false),
		};
		foreach(var (av, bv, wzf, wcf) in cases) {
			var m = M64("660f3817c1");         // PTEST 66 0F 38 17 /r
			m.Xmm[0] = av; m.Xmm[1] = bv;
			Assert.That(m.Step(), Is.True, $"ptest a={av} b={bv} did not step");
			Assert.That((m.Flags & (1u << 6)) != 0, Is.EqualTo(wzf), $"ptest ZF a={av} b={bv}");
			Assert.That((m.Flags & 1u) != 0, Is.EqualTo(wcf), $"ptest CF a={av} b={bv}");
			Assert.That(m.Flags & ((1u << 11) | (1u << 7) | (1u << 4) | (1u << 2)), Is.EqualTo(0u),
				$"ptest must clear OF/SF/AF/PF a={av} b={bv}");
		}
	}

	[Test]
	public void PackedLaneDupExecutes() {
		// MOVSHDUP/MOVSLDUP (vdup): duplicate the odd (or even) 32-bit lanes into both
		// halves of each pair. Four constant-index picks plus a build -- no new node, and
		// unlike valign/vishr the lane pattern is fixed by the OPCODE, so both arms use
		// compile-time indices.
		//
		// THE FOUR LANES ARE ALL DISTINCT VALUES, which is what makes the odd-vs-even
		// split testable: with any repeated lane value the two instructions could agree
		// on data they should differ on.
		var src = (UInt128) 0xBBBBBBBBAAAAAAAAUL | ((UInt128) 0xDDDDDDDDCCCCCCCCUL << 64);

		// MOVSHDUP F3 0F 16 -- odd lanes: r = [B,B,D,D]
		var h = M64("f30f16c1"); h.Xmm[1] = src;
		Assert.That(h.Step(), Is.True, "movshdup did not step");
		Assert.That((ulong) h.Xmm[0], Is.EqualTo(0xBBBBBBBBBBBBBBBBUL), "movshdup lo");
		Assert.That((ulong) (h.Xmm[0] >> 64), Is.EqualTo(0xDDDDDDDDDDDDDDDDUL), "movshdup hi");

		// MOVSLDUP F3 0F 12 -- even lanes: r = [A,A,C,C]
		var lo = M64("f30f12c1"); lo.Xmm[1] = src;
		Assert.That(lo.Step(), Is.True, "movsldup did not step");
		Assert.That((ulong) lo.Xmm[0], Is.EqualTo(0xAAAAAAAAAAAAAAAAUL), "movsldup lo");
		Assert.That((ulong) (lo.Xmm[0] >> 64), Is.EqualTo(0xCCCCCCCCCCCCCCCCUL), "movsldup hi");
	}

	[Test]
	public void PackedRegisterCountShiftsExecute() {
		// PSLLW/PSRLW/PSRAW etc with a REGISTER count (vishr) -- the count is the source
		// XMM's low 64 bits, not an immediate, which is what separates these from the -I
		// forms that lower via vishi.
		//
		// THE SATURATION IS THE WHOLE TEST. SDM: count >= ew gives 0 for SHL/SHR, and the
		// SIGN-FILL for SAR. An implementation that masked the count to (ew-1) -- the
		// natural thing to write, and what a host shift instruction does -- passes every
		// in-range case and fails ONLY here, so each op is tested at cnt=1 AND cnt>=ew.
		var v = (UInt128) 0x8001800180018001UL | ((UInt128) 0x8001800180018001UL << 64);

		// PSLLW 66 0F F1 -- cnt=1: 0x8001 << 1 = 0x0002 per word
		var s1 = M64("660ff1c1"); s1.Xmm[0] = v; s1.Xmm[1] = 1;
		Assert.That(s1.Step(), Is.True, "psllw did not step");
		Assert.That((ulong) s1.Xmm[0], Is.EqualTo(0x0002000200020002UL), "psllw cnt=1");

		// PSLLW cnt=16 -> ZERO (a count-mask impl would shift by 0 and return v unchanged)
		var s2 = M64("660ff1c1"); s2.Xmm[0] = v; s2.Xmm[1] = 16;
		Assert.That(s2.Step(), Is.True, "psllw sat did not step");
		Assert.That(s2.Xmm[0], Is.EqualTo((UInt128) 0), "psllw cnt=16 must be ZERO");

		// PSRLW cnt=99 -> ZERO (well past ew; also tests that the test is on the FULL
		// 64-bit count rather than a narrowed one)
		var s3 = M64("660fd1c1"); s3.Xmm[0] = v; s3.Xmm[1] = 99;
		Assert.That(s3.Step(), Is.True, "psrlw sat did not step");
		Assert.That(s3.Xmm[0], Is.EqualTo((UInt128) 0), "psrlw cnt=99 must be ZERO");

		// PSRAW cnt=1: 0x8001 is negative -> 0xC000 per word (arithmetic, not logical)
		var s4 = M64("660fe1c1"); s4.Xmm[0] = v; s4.Xmm[1] = 1;
		Assert.That(s4.Step(), Is.True, "psraw did not step");
		Assert.That((ulong) s4.Xmm[0], Is.EqualTo(0xC000C000C000C000UL), "psraw cnt=1");

		// PSRAW cnt=16 -> SIGN-FILL, not zero. The one case where SHL/SHR and SAR differ.
		var s5 = M64("660fe1c1"); s5.Xmm[0] = v; s5.Xmm[1] = 16;
		Assert.That(s5.Step(), Is.True, "psraw sat did not step");
		Assert.That((ulong) s5.Xmm[0], Is.EqualTo(0xFFFFFFFFFFFFFFFFUL), "psraw cnt=16 must be sign-fill");
	}

	[Test]
	public void PackedAlignRightExecutes() {
		// PALIGNR dst, src, imm8 (valign): concatenate src:dst as 32 bytes (SRC is the LOW
		// half per the SDM) and take 16 starting at imm8.
		//
		// THREE REGIONS AND EACH IS A SEPARATE ASSERT, because a single imm8 exercises at
		// most two of them:
		//   imm8 = 4   every byte comes from src+dst straddle (idx 4..19)
		//   imm8 = 20  every byte comes from dst, ZERO-FILLED past idx 31
		//   imm8 = 32  ALL ZERO (the out-of-range rule; an implementation that masked the
		//              immediate to 0x1F instead would return src unchanged here)
		var dst = (UInt128) 0xD7D6D5D4D3D2D1D0UL | ((UInt128) 0xDFDEDDDCDBDAD9D8UL << 64);
		var src = (UInt128) 0x5756555453525150UL | ((UInt128) 0x5F5E5D5C5B5A5958UL << 64);

		// imm8=4: bytes src[4..15] then dst[0..3]
		var a1 = M64("660f3a0fc104"); a1.Xmm[0] = dst; a1.Xmm[1] = src;
		Assert.That(a1.Step(), Is.True, "palignr imm=4 did not step");
		Assert.That((ulong) a1.Xmm[0], Is.EqualTo(0x5B5A595857565554UL), "palignr imm=4 lo");
		Assert.That((ulong) (a1.Xmm[0] >> 64), Is.EqualTo(0xD3D2D1D05F5E5D5CUL), "palignr imm=4 hi");

		// imm8=20: idx 20..31 -> dst[4..15], idx 32..35 -> ZERO
		var a2 = M64("660f3a0fc114"); a2.Xmm[0] = dst; a2.Xmm[1] = src;
		Assert.That(a2.Step(), Is.True, "palignr imm=20 did not step");
		Assert.That((ulong) a2.Xmm[0], Is.EqualTo(0xDBDAD9D8D7D6D5D4UL), "palignr imm=20 lo");
		Assert.That((ulong) (a2.Xmm[0] >> 64), Is.EqualTo(0x00000000DFDEDDDCUL), "palignr imm=20 hi zero-fill");

		// imm8=32: ALL ZERO -- the assert that catches a 0x1F mask
		var a3 = M64("660f3a0fc120"); a3.Xmm[0] = dst; a3.Xmm[1] = src;
		Assert.That(a3.Step(), Is.True, "palignr imm=32 did not step");
		Assert.That(a3.Xmm[0], Is.EqualTo((UInt128) 0), "palignr imm=32 must be all zero");
	}

	[Test]
	public void PackedLaneGetSetExecutes() {
		// PEXTRB/PEXTRD (vlane-get) and PINSRB/PINSRW (vlane-set). NO NEW NODE: an
		// extract with a compile-time index is what IlVecElem already is, and an insert
		// is a rebuild of every lane substituting one -- the vzext shape.
		//
		// THE DISCRIMINATOR IS THE SELECTOR MASK, and it differs per width: PEXTRB takes
		// sel & 0xF (16 byte lanes) while PEXTRD takes sel & 0x3 (4 dword lanes). A single
		// hardcoded mask would pass one and fail the other, so both are tested with a
		// selector ABOVE the lane count: 0x11 must wrap to lane 1 for PEXTRB, and 0x06
		// must wrap to lane 2 for PEXTRD.
		var src = (UInt128) 0x0706050403020100UL | ((UInt128) 0x0F0E0D0C0B0A0908UL << 64);

		// PEXTRB 66 0F 3A 14 /r ib -- sel 0x11 & 0xF = lane 1 -> 0x01
		var e1 = M64("660f3a14c811"); e1.Xmm[1] = src;
		Assert.That(e1.Step(), Is.True, "pextrb did not step");
		Assert.That((ulong) e1.Gpr[0] & 0xFF, Is.EqualTo(1UL), "pextrb sel wraps to lane 1");

		// PEXTRD 66 0F 3A 16 /r ib -- sel 0x06 & 0x3 = lane 2 -> 0x0B0A0908
		var e2 = M64("660f3a16c806"); e2.Xmm[1] = src;
		Assert.That(e2.Step(), Is.True, "pextrd did not step");
		Assert.That((uint) e2.Gpr[0], Is.EqualTo(0x0B0A0908u), "pextrd sel wraps to lane 2");

		// PINSRB 66 0F 3A 20 /r ib -- write 0xAA into lane (0x13 & 0xF) = 3
		var i1 = M64("660f3a20c113"); i1.Xmm[0] = src; i1.Gpr[1] = 0xFFFFFFAAUL;
		Assert.That(i1.Step(), Is.True, "pinsrb did not step");
		Assert.That((ulong) i1.Xmm[0], Is.EqualTo(0x07060504AA020100UL), "pinsrb lane 3");

		// PINSRW 66 0F C4 /r ib -- write 0xBBBB into word lane (0x09 & 0x7) = 1
		var i2 = M64("660fc4c109"); i2.Xmm[0] = src; i2.Gpr[1] = 0xFFFFBBBBUL;
		Assert.That(i2.Step(), Is.True, "pinsrw did not step");
		Assert.That((ulong) i2.Xmm[0], Is.EqualTo(0x07060504BBBB0100UL), "pinsrw word lane 1");
	}

	[Test]
	public void PackedShuffleBytesExecutes() {
		// PSHUFB (vshufv): a DATA-DEPENDENT shuffle -- the per-lane index comes from a
		// register, and bit 7 of an index ZEROES its output lane.
		//
		// THREE DISCRIMINATING PROPERTIES, all in one source:
		//   index 0x80 / 0xFF  -> lane must be ZERO (the bit-7 rule; an implementation
		//                         that ignored bit 7 would index lane 0 and lane 15)
		//   index 0x1F         -> lane 0xF (the index WRAPS: only the low 4 bits select,
		//                         and 0x1F has bit 7 CLEAR so it must NOT zero)
		//   a runtime index at all -> IlVecElem.Idx is an Il, not a constant
		var dst = (UInt128) 0x1716151413121110UL | ((UInt128) 0x1F1E1D1C1B1A1918UL << 64);
		var src = (UInt128) 0x071F05FF03800F00UL;   // lanes lo->hi: 00 0F 80 03 FF 05 1F 07

		var m = M64("660f3800c1"); m.Xmm[0] = dst; m.Xmm[1] = src;   // PSHUFB 66 0F 38 00
		Assert.That(m.Step(), Is.True, "pshufb did not step");
		// lanes lo->hi: 00->0x10  0F->0x1F  80->ZERO  03->0x13  FF->ZERO  05->0x15
		//               1F->0x1F  07->0x17     packed little-endian = 0x171F150013001F10
		// (My first expectation was 0x171F001500131F10 -- the same eight VALUES with the
		// two zero lanes one position off. The comment above it listed the source bytes
		// correctly and I packed them wrong, so the prose and the constant disagreed and
		// only the constant was checked. DERIVED here from the source qword rather than
		// composed: bytes = [(src >> 8i) & 0xFF], lane_i = bit7 ? 0 : dst[idx & 0xF].)
		Assert.That((ulong) m.Xmm[0], Is.EqualTo(0x171F150013001F10UL), "pshufb lo 8 lanes");
		// the source's upper 8 index bytes are all 0x00 -> every high lane selects dst[0]
		Assert.That((ulong) (m.Xmm[0] >> 64), Is.EqualTo(0x1010101010101010UL), "pshufb hi 8 lanes");
	}

	[Test]
	public void PackedZeroExtendExecutes() {
		// PMOVZXBW/PMOVZXWD (vzext): take the LOW 128/dew lanes at width sew and
		// zero-extend each into a dew-wide lane.
		//
		// THE DISCRIMINATING LANES ARE THE HIGH-BIT ONES. 0xFF must become 0x00FF, not
		// 0xFFFF -- a SIGN-extending implementation differs on exactly those and nowhere
		// else, so a source whose bytes are all <0x80 cannot test this at all. And the
		// UPPER half of the source must be IGNORED: I put 0xEE there, which would show up
		// as a lane if the extraction walked all 16 bytes.
		var src = (UInt128) 0x7F01_80FFUL | ((UInt128) 0xEEEEEEEEEEEEEEEEUL << 64);

		var m = M64("660f3830c1"); m.Xmm[1] = src;        // PMOVZXBW 66 0F 38 30
		Assert.That(m.Step(), Is.True, "pmovzxbw did not step");
		// bytes FF 80 01 7F -> words 00FF 0080 0001 007F
		Assert.That((ulong) m.Xmm[0], Is.EqualTo(0x007F_0001_0080_00FFUL), "pmovzxbw lo");
		Assert.That((ulong) (m.Xmm[0] >> 64), Is.EqualTo(0UL), "pmovzxbw hi lanes come from bytes 4-7");

		var w = M64("660f3833c1"); w.Xmm[1] = src;        // PMOVZXWD 66 0F 38 33
		Assert.That(w.Step(), Is.True, "pmovzxwd did not step");
		// words 80FF 7F01 -> dwords 000080FF 00007F01
		Assert.That((ulong) w.Xmm[0], Is.EqualTo(0x00007F01_000080FFUL), "pmovzxwd lo");
	}

	[Test]
	public void PackedMulldAndCmpeqqExecute() {
		// PMULLD = vibin ew=32 op=2 (Mul), PCMPEQQ = vibin ew=64 op=3 (Eq). Both map to
		// ops that already existed; the point of the test is that a cleared track-fail
		// means the def LIFTS, not that it computes the right thing.
		//
		// PMULLD keeps the LOW 32 bits of each 32x32 product, which is what a wrapping
		// multiply gives -- so the discriminating lane is one that OVERFLOWS. 0x10000 *
		// 0x10000 = 2^32, whose low 32 bits are ZERO: a non-wrapping implementation
		// cannot produce that, and a widening one would need a lane it doesn't have.
		var m = M64("660f3840c1");                        // PMULLD 66 0F 38 40
		m.Xmm[0] = ((UInt128) 0x00010000UL) | ((UInt128) 3UL << 32);
		m.Xmm[1] = ((UInt128) 0x00010000UL) | ((UInt128) 5UL << 32);
		Assert.That(m.Step(), Is.True, "pmulld did not step");
		Assert.That((uint) m.Xmm[0], Is.EqualTo(0u), "pmulld lane0 must WRAP to 0");
		Assert.That((uint) (m.Xmm[0] >> 32), Is.EqualTo(15u), "pmulld lane1");

		// PCMPEQQ at ew=64: all-1s per QWORD lane on equality. The discriminating pair
		// has one lane equal and one not -- a compare done at the wrong width (32) would
		// see lane0's halves as two separate equal lanes and give a different answer.
		var q = M64("660f3829c1");                        // PCMPEQQ 66 0F 38 29
		q.Xmm[0] = ((UInt128) 0xAAAAAAAABBBBBBBBUL) | ((UInt128) 0x1111UL << 64);
		q.Xmm[1] = ((UInt128) 0xAAAAAAAABBBBBBBBUL) | ((UInt128) 0x2222UL << 64);
		Assert.That(q.Step(), Is.True, "pcmpeqq did not step");
		Assert.That((ulong) q.Xmm[0], Is.EqualTo(ulong.MaxValue), "pcmpeqq lane0 equal");
		Assert.That((ulong) (q.Xmm[0] >> 64), Is.EqualTo(0UL), "pcmpeqq lane1 differs");
	}

	[Test]
	public void PackedAbsExecutes() {
		// PABSB/PABSW/PABSD via the sign-mask identity (viabs), NO new node and no
		// UnOp.Abs on an integer lane:
		//     sign = Sar(x, ew-1)          all-1s if negative, 0 if not
		//     abs  = Sub(Xor(x, sign), sign)
		//
		// THE DISCRIMINATING LANE IS INT_MIN, and it looks like a bug: 0x80 (-128)
		// abs'es to 0x80, because INT_MIN has no positive representation at the width.
		// That is what x86 does (SDM: the result is INT_MIN) and a composed "clamp to
		// 0x7F" would be the wrong fix for a correct answer. A lane arm that computed
		// abs via a compare-and-negate WITHOUT the wrap would differ here and nowhere
		// else, so this lane is the whole test.
		var A = (UInt128) 0x7F01FF80UL;   // lanes lo->hi: 80 FF 01 7F  = -128, -1, 1, 127

		var m = M64("660f381cc1"); m.Xmm[1] = A;            // PABSB 66 0F 38 1C
		Assert.That(m.Step(), Is.True, "pabsb did not step");
		Assert.That((uint) m.Xmm[0], Is.EqualTo(0x7F0101_80u), "pabsb");

		// PABSW 66 0F 38 1D -- word lanes: 0xFF80 = -128 -> 0x0080, 0x7F01 -> 0x7F01
		var w = M64("660f381dc1"); w.Xmm[1] = A;
		Assert.That(w.Step(), Is.True, "pabsw did not step");
		Assert.That((uint) w.Xmm[0], Is.EqualTo(0x7F01_0080u), "pabsw");

		// PABSD 66 0F 38 1E -- one dword lane: 0x7F01FF80 is positive, unchanged
		var dd = M64("660f381ec1"); dd.Xmm[1] = A;
		Assert.That(dd.Step(), Is.True, "pabsd did not step");
		Assert.That((uint) dd.Xmm[0], Is.EqualTo(0x7F01FF80u), "pabsd");
	}

	[Test]
	public void PackedIntMinMaxExecutes() {
		// PMAXSB/PMINSB/PMAXUB/PMINUB via the mask-then-blend idiom (vibin ops 5-8):
		//   mask = a >s b or a >u b (all-1s per lane), res = (keep & mask) | (other & ~mask)
		// No new BinOp -- integer min/max is expressible with And/Or/Not, and the
		// SIGNEDNESS rides the compare's ElemTy alone.
		//
		// THE OPERANDS ARE CHOSEN SO EVERY LANE DISCRIMINATES, and my first pair did not:
		// I used A=0x7F80017F / B=0x01FF7F02, where maxs and maxu are the SAME VALUE
		// (0x7FFF7F7F) because lane 2's maxs(-128,-1) = -1 = 0xFF = maxu(0x80,0xFF).
		// Both asserts passed and the signed/unsigned split was never tested -- the
		// test-topology-excludes-the-failure-mode class at n=5 at this bench (the IC unit
		// test's 2-block loop, the LLVM oracle that never wrote a flag, the x87 gate's
		// symmetric operands, the mulss lo-only compare).
		//
		// Here each lane has ONE operand >=0x80 and one <0x80, so maxs != maxu and
		// mins != minu in EVERY lane. A wrong signedness cannot pass.
		var A = (UInt128) 0x7FFF0180UL;   // lanes lo->hi: 80 01 FF 7F
		var B = (UInt128) 0xFF7F8001UL;   // lanes lo->hi: 01 80 7F FF

		var m = M64("660f383cc1"); m.Xmm[0] = A; m.Xmm[1] = B;   // PMAXSB 66 0F 38 3C
		Assert.That(m.Step(), Is.True, "pmaxsb did not step");
		Assert.That((uint) m.Xmm[0], Is.EqualTo(0x7F7F0101u), "pmaxsb");

		var n2 = M64("660f3838c1"); n2.Xmm[0] = A; n2.Xmm[1] = B; // PMINSB 66 0F 38 38
		Assert.That(n2.Step(), Is.True, "pminsb did not step");
		Assert.That((uint) n2.Xmm[0], Is.EqualTo(0xFFFF8080u), "pminsb");

		var u = M64("660fdec1"); u.Xmm[0] = A; u.Xmm[1] = B;      // PMAXUB 66 0F DE
		Assert.That(u.Step(), Is.True, "pmaxub did not step");
		Assert.That((uint) u.Xmm[0], Is.EqualTo(0xFFFF8080u), "pmaxub");

		var v = M64("660fdac1"); v.Xmm[0] = A; v.Xmm[1] = B;      // PMINUB 66 0F DA
		Assert.That(v.Step(), Is.True, "pminub did not step");
		Assert.That((uint) v.Xmm[0], Is.EqualTo(0x7F7F0101u), "pminub");
	}

	[Test]
	public void ScalarSsePreservesUpperBits() {  // F3 0F 59 C1 = mulss xmm0, xmm1
		// SDM Vol 2A, MULSS: "The three high-order doublewords of the destination operand
		// remain unchanged." IlLower emits the merge for that (IlLower.cs:442-451) and the
		// oracle could not grade it: XFReader seeded and compared the LO WORD ONLY, so a
		// machine that zeroed the upper 96 bits agreed with silicon on 3.17M rows.
		//
		// This is the acceptance case for the UInt128 carrier. Pre-widening it fails at the
		// upper-96 assert; the corpus row that named it (p2 #2886576, CVTSI2SS) has
		// pre=3f8000003f8000003f8000003f800000 and post=3f8000003f8000003f80000000000000 --
		// silicon keeping the top three doublewords while only lane 0 changes.
		var hi = ((UInt128) 0x3f8000003f800000UL << 64) | 0x3f80000000000000UL;
		var m = M64("f30f59c1");
		m.Xmm[0] = hi | Fb(6.0f);
		m.Xmm[1] = Fb(7.0f);
		Assert.That(m.Step(), Is.True, "mulss did not step");
		Assert.That((uint) m.Xmm[0], Is.EqualTo(Fb(42.0f)), "lane 0 = 6*7 bit-exact");
		Assert.That(m.Xmm[0] >> 32, Is.EqualTo(hi >> 32),
			"upper 96 bits MUST be preserved -- this is what the lo-only oracle could not see");
	}

	[Test]
	public void SseScalarFloatArithExecutes() {  // F3 0F 59 C1 = mulss xmm0, xmm1
		var m = MF("f30f59c1", Fb(6.0f), Fb(7.0f));
		Assert.That(m.Step(), Is.True, "mulss did not step");
		Assert.That((uint) m.Xmm[0], Is.EqualTo(Fb(42.0f)), "6*7 bit-exact");

		var a = MF("f30f58c1", Fb(1.5f), Fb(2.25f));   // addss
		a.Step();
		Assert.That((uint) a.Xmm[0], Is.EqualTo(Fb(3.75f)), "1.5+2.25");

		var s = MF("f30f5cc1", Fb(10.0f), Fb(3.5f));   // subss
		s.Step();
		Assert.That((uint) s.Xmm[0], Is.EqualTo(Fb(6.5f)), "10-3.5");

		var d = MF("f30f5ec1", Fb(9.0f), Fb(2.0f));    // divss
		d.Step();
		Assert.That((uint) d.Xmm[0], Is.EqualTo(Fb(4.5f)), "9/2");
	}

	[Test]
	public void SseMinMaxUsesX86NaNRule() {  // F3 0F 5D C1 = minss / 5F = maxss
		// x86 MIN/MAX return the SECOND source when either operand is NaN — NOT
		// ARM's FMAX/FMIN (which propagate the NaN) and NOT MathF.Max (same). If
		// the evaluator used MathF.Max this test fails, which is why it exists.
		var nan = Fb(float.NaN);
		var mx = MF("f30f5fc1", nan, Fb(3.0f));
		mx.Step();
		Assert.That((uint) mx.Xmm[0], Is.EqualTo(Fb(3.0f)), "maxss NaN,3 → 3 (2nd src)");

		var mn = MF("f30f5dc1", nan, Fb(3.0f));
		mn.Step();
		Assert.That((uint) mn.Xmm[0], Is.EqualTo(Fb(3.0f)), "minss NaN,3 → 3 (2nd src)");

		// and the ordinary ordering still works
		var ok = MF("f30f5fc1", Fb(2.0f), Fb(5.0f));
		ok.Step();
		Assert.That((uint) ok.Xmm[0], Is.EqualTo(Fb(5.0f)), "maxss 2,5 → 5");
	}

	[Test]
	public void ComissSetsUnorderedFlags() {  // 0F 2F C1 = comiss xmm0, xmm1
		// The .isa body is the reason this is the interesting one: PF=unord,
		// CF=(lt|unord), ZF=(eq|unord), and OF/SF/AF forced 0. A NaN operand must
		// set all three of PF/CF/ZF — which only works if fisnan, flt and feq all
		// evaluate on FLOAT operands.
		var eq = MF("0f2fc1", Fb(1.0f), Fb(1.0f));
		eq.Step();
		Assert.That(F(eq, ZF), Is.True,  "equal → ZF");
		Assert.That(F(eq, CF), Is.False, "equal → no CF");
		Assert.That(F(eq, PF), Is.False, "ordered → no PF");

		var lt = MF("0f2fc1", Fb(1.0f), Fb(2.0f));
		lt.Step();
		Assert.That(F(lt, CF), Is.True,  "less → CF");
		Assert.That(F(lt, ZF), Is.False, "less → no ZF");

		var un = MF("0f2fc1", Fb(float.NaN), Fb(1.0f));
		un.Step();
		Assert.That(F(un, PF), Is.True, "unordered → PF");
		Assert.That(F(un, CF), Is.True, "unordered → CF");
		Assert.That(F(un, ZF), Is.True, "unordered → ZF");
		Assert.That(F(un, OF), Is.False, "OF forced 0");
		Assert.That(F(un, SF), Is.False, "SF forced 0");
	}

	[Test]
	public void CvtsiToFloatAndBackExecutes() {  // F3 48 0F 2A = cvtsi2ss xmm0, rcx
		var c = M64("f3480f2ac1");
		c.Gpr[1] = 84;                 // src IS a GPR here (cvtsi2ss xmm, r/m64)
		c.Step();
		Assert.That((uint) c.Xmm[0], Is.EqualTo(Fb(84.0f)), "int 84 → 84.0f (dst = xmm)");
		Assert.That(c.Gpr[1], Is.EqualTo(84UL), "the source GPR is untouched");

		// and the x86 INDEFINITE-INTEGER guard on the way back: cvttss2si of NaN
		// must give 0x80000000, not 0 and not a saturate. The silicon sweep measured
		// a three-way divergence here when this was a bare F→I cast.
		var t = M64("f30f2cc1");       // cvttss2si eax, xmm1 -- dst GPR, src xmm
		t.Xmm[1] = Fb(float.NaN);
		t.Step();
		Assert.That((uint) t.Gpr[0], Is.EqualTo(0x80000000u), "NaN → indefinite integer");

		var v = M64("f30f2cc1");
		v.Xmm[1] = Fb(-7.9f);
		v.Step();
		Assert.That((int) (uint) v.Gpr[0], Is.EqualTo(-7), "truncate toward zero");
	}

	[Test]
	public void AddCarryAndOverflow() {  // add eax, ebx with 0xFFFFFFFF + 1 → CF=1 ZF=1 OF=0
		var m = M64("01d8");
		m.Gpr[0] = 0xFFFFFFFF; m.Gpr[3] = 1;
		Assert.That(m.Step(), Is.True);
		Assert.That(m.Gpr[0], Is.EqualTo(0UL));           // 32-bit write zexts
		Assert.That(F(m, CF), Is.True,  "CF");
		Assert.That(F(m, ZF), Is.True,  "ZF");
		Assert.That(F(m, OF), Is.False, "OF");
		Assert.That(m.Ip, Is.EqualTo(0x1002UL));
	}

	[Test]
	public void AddSignedOverflow() {  // 0x7FFFFFFF + 1 → OF=1 SF=1 CF=0
		var m = M64("01d8");
		m.Gpr[0] = 0x7FFFFFFF; m.Gpr[3] = 1;
		m.Step();
		Assert.That(m.Gpr[0], Is.EqualTo(0x80000000UL));
		Assert.That(F(m, OF), Is.True,  "OF");
		Assert.That(F(m, SF), Is.True,  "SF");
		Assert.That(F(m, CF), Is.False, "CF");
	}

	[Test]
	public void SubBorrowAndParity() {  // sub eax, ebx: 1 - 2 → CF=1 SF=1; result 0xFFFFFFFF → PF(FF)=1
		var m = M64("29d8");
		m.Gpr[0] = 1; m.Gpr[3] = 2;
		m.Step();
		Assert.That(m.Gpr[0], Is.EqualTo(0xFFFFFFFFUL));
		Assert.That(F(m, CF), Is.True, "CF (borrow)");
		Assert.That(F(m, SF), Is.True, "SF");
		Assert.That(F(m, PF), Is.True, "PF (0xFF has 8 set bits = even)");
	}

	[Test]
	public void PushPopRoundTrip() {  // push rbp; pop rcx
		var m = M64("55" + "59");
		m.Gpr[5] = 0xDEADBEEFCAFE; m.Gpr[4] = 0x8000;
		m.Step();
		Assert.That(m.Gpr[4], Is.EqualTo(0x7FF8UL), "RSP after push");
		m.Step();
		Assert.That(m.Gpr[1], Is.EqualTo(0xDEADBEEFCAFEUL), "popped value");
		Assert.That(m.Gpr[4], Is.EqualTo(0x8000UL), "RSP restored");
	}

	[Test]
	public void JzTakenAndNot() {  // xor eax,eax; jz +2 → taken. Then dec-like: or eax,1; jz → not
		var m = M64("31c0" + "7402" + "90" + "90");  // xor; jz 0x1006; nop; nop
		m.Step();  // xor → ZF=1
		Assert.That(F(m, ZF), Is.True);
		m.Step();  // jz taken
		Assert.That(m.Ip, Is.EqualTo(0x1006UL), "taken → skips nops");

		var m2 = M64("83c801" + "7402" + "90");  // or eax,1; jz +2; nop
		m2.Step();
		Assert.That(F(m2, ZF), Is.False);
		m2.Step();  // not taken
		Assert.That(m2.Ip, Is.EqualTo(0x1005UL), "fallthrough");
	}

	[Test]
	public void CallRetRoundTrip() {  // call +3; (skipped: nop nop nop); ret at target? — layout: call 0x1008; nops; @1008 ret
		var m = M64("e803000000" + "909090" + "c3");
		m.Gpr[4] = 0x8000;
		m.Step();  // call
		Assert.That(m.Ip, Is.EqualTo(0x1008UL), "call target");
		Assert.That(m.Gpr[4], Is.EqualTo(0x7FF8UL), "return addr pushed");
		m.Step();  // ret
		Assert.That(m.Ip, Is.EqualTo(0x1005UL), "returned to after-call");
		Assert.That(m.Gpr[4], Is.EqualTo(0x8000UL));
	}

	[Test]
	public void CmovTakenAndNot() {  // cmp eax,ebx (equal → ZF); cmovz ecx, edx
		var m = M64("39d8" + "0f44ca");
		m.Gpr[0] = 5; m.Gpr[3] = 5; m.Gpr[1] = 111; m.Gpr[2] = 222;
		m.Step(); m.Step();
		Assert.That(m.Gpr[1], Is.EqualTo(222UL), "cmovz taken");

		var m2 = M64("39d8" + "0f44ca");
		m2.Gpr[0] = 5; m2.Gpr[3] = 6; m2.Gpr[1] = 111; m2.Gpr[2] = 222;
		m2.Step(); m2.Step();
		Assert.That(m2.Gpr[1], Is.EqualTo(111UL), "cmovz not taken preserves");
	}

	[Test]
	public void MemRmwAdd() {  // add [rbp-0x10], ebx
		var m = M64("015df0");
		m.Gpr[5] = 0x9000; m.Gpr[3] = 7;
		m.Mem[0x8FF0] = 40;
		m.Step();
		Assert.That(m.Mem[0x8FF0], Is.EqualTo((byte) 47));
	}

	[Test]
	public void ShlFlagsGuarded() {  // shl eax, 0 must NOT touch flags (the (if (!= c 0)) guard)
		var m = M64("c1e000");  // shl eax, 0
		m.Gpr[0] = 5;
		m.Flags |= 1UL << ZF;  // pre-set ZF
		m.Step();
		Assert.That(F(m, ZF), Is.True, "shift-by-0 preserves flags");

		var m2 = M64("c1e001");  // shl eax, 1: 0x80000000 → 0, CF=1 ZF=1
		m2.Gpr[0] = 0x80000000;
		m2.Step();
		Assert.That(m2.Gpr[0], Is.EqualTo(0UL));
		Assert.That(F(m2, CF), Is.True, "CF = last bit out");
		Assert.That(F(m2, ZF), Is.True);
	}

	[Test]
	public void RealMode16WrapAndSeg() {  // 16-bit: mov ax, [0x10] with DS=0x100 → linear 0x1010
		var m = new X86Machine { Mode = XMode.Bits16, Mem = new byte[0x20000], Ip = 0x100 };
		m.SegSel[3] = 0x100; m.SegBase[3] = 0x1000;  // DS
		Convert.FromHexString("A11000").CopyTo(m.Mem, 0x100);  // mov ax, [0x10]
		m.Mem[0x1010] = 0x34; m.Mem[0x1011] = 0x12;
		Assert.That(m.Step(), Is.True);
		Assert.That((ushort) m.Gpr[0], Is.EqualTo((ushort) 0x1234));
	}

	// --- string family (machine-native) ---
	[Test]
	public void RepMovsbCopies() {  // 64-bit flat: rep movsb, rsi→rdi, rcx=5
		var m = M64("f3a4");
		m.Gpr[6] = 0x5000; m.Gpr[7] = 0x6000; m.Gpr[1] = 5;
		"HELLO"u8.ToArray().CopyTo(m.Mem, 0x5000);
		m.Step();
		Assert.That(System.Text.Encoding.ASCII.GetString(m.Mem, 0x6000, 5), Is.EqualTo("HELLO"));
		Assert.That(m.Gpr[1], Is.EqualTo(0UL), "CX exhausted");
		Assert.That(m.Gpr[6], Is.EqualTo(0x5005UL));
		Assert.That(m.Gpr[7], Is.EqualTo(0x6005UL));
	}

	[Test]
	public void RepStoswFills() {  // rep stosw: AX pattern × 3
		var m = M64("66f3ab");  // rep stosw (66 = 16-bit op)
		m.Gpr[0] = 0xABCD; m.Gpr[7] = 0x7000; m.Gpr[1] = 3;
		m.Step();
		for(var i = 0; i < 3; i++) {
			Assert.That(m.Mem[0x7000 + i * 2], Is.EqualTo((byte) 0xCD));
			Assert.That(m.Mem[0x7001 + i * 2], Is.EqualTo((byte) 0xAB));
		}
		Assert.That(m.Gpr[7], Is.EqualTo(0x7006UL));
	}

	[Test]
	public void RepneScasbStrlen() {  // the strlen idiom: AL=0, CX=max, repne scasb
		var m = M64("f2ae");
		m.Gpr[0] = 0; m.Gpr[7] = 0x5000; m.Gpr[1] = 0xFFFF;
		"abc\0"u8.ToArray().CopyTo(m.Mem, 0x5000);
		m.Step();
		// stops AFTER matching the NUL: DI = 0x5004, len = 0xFFFF - CX - 1 = 3
		Assert.That(m.Gpr[7], Is.EqualTo(0x5004UL));
		Assert.That(0xFFFFUL - m.Gpr[1] - 1, Is.EqualTo(3UL), "strlen");
		Assert.That(F(m, ZF), Is.True, "ZF set on match");
	}

	[Test]
	public void MovsbRespectDf() {  // std; movsb → SI/DI decrement
		var m = M64("fd" + "a4");
		m.Gpr[6] = 0x5000; m.Gpr[7] = 0x6000;
		m.Mem[0x5000] = 0x77;
		m.Step(); m.Step();
		Assert.That(m.Mem[0x6000], Is.EqualTo((byte) 0x77));
		Assert.That(m.Gpr[6], Is.EqualTo(0x4FFFUL), "SI decremented");
		Assert.That(m.Gpr[7], Is.EqualTo(0x5FFFUL), "DI decremented");
	}

	[Test]
	public void CmpsbSetsFlags() {  // cmpsb equal → ZF; then differing → CF per compare
		var m = M64("a6");
		m.Gpr[6] = 0x5000; m.Gpr[7] = 0x6000;
		m.Mem[0x5000] = 5; m.Mem[0x6000] = 5;
		m.Step();
		Assert.That(F(m, ZF), Is.True, "equal bytes");

		var m2 = M64("a6");
		m2.Gpr[6] = 0x5000; m2.Gpr[7] = 0x6000;
		m2.Mem[0x5000] = 3; m2.Mem[0x6000] = 7;  // 3-7 borrows
		m2.Step();
		Assert.That(F(m2, ZF), Is.False);
		Assert.That(F(m2, CF), Is.True, "borrow");
	}

	// --- loop family ---
	[Test]
	public void LoopCountsDown() {  // mov ecx,3; L: inc eax; loop L
		var m = M64("B903000000" + "FFC0" + "E2FC");
		m.Step();
		for(var i = 0; i < 20 && m.Ip != 0x1009; i++) m.Step();
		Assert.That(m.Gpr[0], Is.EqualTo(3UL), "body ran CX times");
		Assert.That(m.Gpr[1], Is.EqualTo(0UL));
	}

	[Test]
	public void JcxzBranchesOnZero() {
		var m = M64("E302" + "9090");  // jrcxz +2
		m.Gpr[1] = 0;
		m.Step();
		Assert.That(m.Ip, Is.EqualTo(0x1004UL), "taken on rcx==0");

		var m2 = M64("E302" + "9090");
		m2.Gpr[1] = 5;
		m2.Step();
		Assert.That(m2.Ip, Is.EqualTo(0x1002UL), "not taken");
		Assert.That(m2.Gpr[1], Is.EqualTo(5UL), "jcxz never decs");
	}

	[Test]
	public void LoopeStopsOnZfClear() {  // loope: continue while CX!=0 AND ZF=1
		var m = M64("E2FE");  // loop self — but as loope via E1
		var me = M64("E1FE");
		me.Gpr[1] = 5;
		me.Flags &= ~(1UL << ZF);  // ZF=0 → loope falls through immediately
		me.Step();
		Assert.That(me.Ip, Is.EqualTo(0x1002UL), "ZF=0 ends loope");
		Assert.That(me.Gpr[1], Is.EqualTo(4UL), "but CX still dec'd");
	}

	// --- wide mul/div (F7 family) ---
	[Test]
	public void MulWideSetsDxAndCf() {  // mul ebx: eax=0x80000000 * 4 → edx:eax = 2:0, CF/OF=1
		var m = M64("f7e3");
		m.Gpr[0] = 0x80000000; m.Gpr[3] = 4;
		m.Step();
		Assert.That(m.Gpr[0], Is.EqualTo(0UL));
		Assert.That(m.Gpr[2], Is.EqualTo(2UL), "high half in edx");
		Assert.That(F(m, CF), Is.True, "CF: high half nonzero");

		var m2 = M64("f7e3");  // 3*5 fits → CF=0
		m2.Gpr[0] = 3; m2.Gpr[3] = 5;
		m2.Step();
		Assert.That(m2.Gpr[0], Is.EqualTo(15UL));
		Assert.That(F(m2, CF), Is.False);
	}

	[Test]
	public void DivWideQuotientRemainder() {  // div ebx: edx:eax = 0:100 / 7 → q=14 r=2
		var m = M64("f7f3");
		m.Gpr[0] = 100; m.Gpr[2] = 0; m.Gpr[3] = 7;
		m.Step();
		Assert.That(m.Gpr[0], Is.EqualTo(14UL));
		Assert.That(m.Gpr[2], Is.EqualTo(2UL));
	}

	[Test]
	public void IdivSigned() {  // idiv ebx: -100 / 7 → q=-14 r=-2 (C truncation semantics)
		var m = M64("f7fb");
		m.Gpr[0] = unchecked((ulong) -100L) & 0xFFFFFFFF; m.Gpr[2] = 0xFFFFFFFF; m.Gpr[3] = 7;
		m.Step();
		Assert.That((int) m.Gpr[0], Is.EqualTo(-14));
		Assert.That((int) m.Gpr[2], Is.EqualTo(-2));
	}

	[Test]
	public void DivByZeroThrows() {
		var m = M64("f7f3");
		m.Gpr[0] = 5; m.Gpr[3] = 0;
		Assert.Throws<DivideByZeroException>(() => m.Step());
	}

	[Test]
	public void Mul8UsesAx() {  // mul bl: al=20 * 30 → ax=600
		var m = M64("f6e3");
		m.Gpr[0] = 20; m.Gpr[3] = 30;
		m.Step();
		Assert.That(m.Gpr[0] & 0xFFFF, Is.EqualTo(600UL));
		Assert.That(F(m, CF), Is.True, "AH nonzero → CF");
	}

	[Test]
	public void FetchHookExecFilter() {  // FetchHook distinct from LoadHook (‡)
		byte[] code = [0x48, 0x8B, 0x03, 0xC3];  // mov rax,[rbx] ; ret
		var m = new X86Machine {
			Mode = XMode.Bits64, Ip = 0x1000,
			FetchHook = (a, buf) => {
				if(a < 0x1000 || a >= 0x2000) return false;  // exec-region check
				var off = (int) (a - 0x1000);
				for(var i = 0; i < buf.Length && off + i < code.Length; i++) buf[i] = code[off + i];
				return true;
			},
			LoadHook = (a, w) => a == 0x9999 ? 0xFEEDUL : 0,  // data ONLY — fetch never reaches here
		};
		m.Gpr[3] = 0x9999; m.Gpr[4] = 0x8000;
		Assert.That(m.Step(), Is.True);   // mov rax,[rbx]
		Assert.That(m.Gpr[0], Is.EqualTo(0xFEEDUL), "data via LoadHook, fetch via FetchHook");
		// jump outside exec region → FetchHook returns false → Step()=false
		m.Ip = 0x5000;
		Assert.That(m.Step(), Is.False, "not-exec → Step false");
	}

	[Test]
	public void MemHooksFallback() {  // no Mem[] — fetch + load + store all via hooks (the X86Env pattern)
		var stores = new Dictionary<ulong, (ulong v, int w)>();
		byte[] code = [0x48, 0x8B, 0x03, 0x50];  // mov rax,[rbx] ; push rax
		int loadW = 0;
		var m = new X86Machine {
			Mode = XMode.Bits64, Ip = 0x1000,
			LoadHook = (a, w) => {
				if(a >= 0x1000 && a < 0x1000 + (ulong) code.Length) return code[a - 0x1000];  // fetch (byte-at-a-time)
				if(a == 0xC0FFEE) { loadW = w; return 0x1234; }  // data
				return 0;
			},
			StoreHook = (a, v, w) => { stores[a] = (v, w); return true; },
		};
		m.Gpr[3] = 0xC0FFEE; m.Gpr[4] = 0x8000;
		m.Step();
		Assert.That(m.Gpr[0], Is.EqualTo(0x1234UL));
		Assert.That(loadW, Is.EqualTo(64));  // Ev width = 64 (REX.W)
		m.Step();
		Assert.That(stores[0x7FF8].v, Is.EqualTo(0x1234UL));
		Assert.That(stores[0x7FF8].w, Is.EqualTo(64));
	}

	[Test]
	public void IntrinsicDispatch() {  // int 21h routes to the handler with the imm arg
		var m = M64("cd21");
		string got = null; ulong gotArg = 0;
		m.OnIntrin = (mm, name, args) => { got = name; gotArg = args.Length > 0 ? args[0] : 0; return true; };
		m.Step();
		Assert.That(got, Is.EqualTo("int"));
		Assert.That(gotArg, Is.EqualTo(0x21UL));
	}
}
