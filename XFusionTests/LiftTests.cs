using XFusionCpu;
using LiftIl;
// IlLower + OperandBind live in XFusionGenerator but are COMPILED INTO XFusionCpu
// (XFusionCpu.csproj:17 links them as Lift/IlLower.cs), so the test project reaches
// them without referencing the generator. PList/ListParser are CoreArchCompiler's.
using XFusionGenerator;
using CoreArchCompiler;

namespace XFusionTests;

/// M1 end-to-end: bytes → DecodeInsn → binds → IlLower → IlBlock.
/// The golden rows now arrive THROUGH THE DECODER (vs IlLowerTests' hand binds).
[TestFixture]
public class LiftTests {
	static string LiftText(string hex, XMode mode = XMode.Bits64, ulong pc = 0) {
		var block = X86Lifter.Lift(Convert.FromHexString(hex), pc, mode);
		return block?.ToString();
	}

	[Test]
	public void VexPackedFloatDecodesBothLengths() {
		// objdump -M intel, ALL verified BEFORE the .isa edit:
		//   C5 F0 59 C1 = vmulps xmm0,xmm1,xmm1    C5 F4 59 C1 = vmulps ymm0,ymm1,ymm1
		//   C5 F1 59 C1 = vmulpd xmm...            C5 F0 C6 C1 00 = vshufps xmm...,0x0
		// VEX2 byte-2 = R<<7 | (~vvvv&0xF)<<3 | L<<2 | pp; vvvv=xmm1 -> 0xE; pp 0=none 1=66.
		// DECODE-ONLY, and unlike the scalar rows there are TWO independent reasons:
		//   (a) VEX zeroes DEST[MAXVL-1:128] where legacy SSE preserves it -- our
		//       write_operand has one rule per WIDTH, not one per encoding-family.
		//   (b) VEX.L picks 128 or 256 at decode time, and there is no 256-bit VALUE
		//       in either evaluator (X86Machine.Xmm[] is u128, Eval's carrier is ulong).
		// So an L=1 form has no representable result at all. Intrinsic-bodied.
		foreach(var (hex, mnem, reg) in new[] {
			("c5f058c1","vaddps","xmm"), ("c5f059c1","vmulps","xmm"),
			("c5f05cc1","vsubps","xmm"), ("c5f05ec1","vdivps","xmm"),
			("c5f458c1","vaddps","ymm"), ("c5f459c1","vmulps","ymm"),
			("c5f45cc1","vsubps","ymm"), ("c5f45ec1","vdivps","ymm"),
			("c5f158c1","vaddpd","xmm"), ("c5f159c1","vmulpd","xmm"),
			("c5f15cc1","vsubpd","xmm"), ("c5f15ec1","vdivpd","xmm"),
			("c5f558c1","vaddpd","ymm"), ("c5f559c1","vmulpd","ymm"),
			("c5f55cc1","vsubpd","ymm"), ("c5f55ec1","vdivpd","ymm"),
		}) {
			var b = Convert.FromHexString(hex);
			Assert.That(Disassembler.DecodeInsn(b, XMode.Bits64, out var d), Is.True, $"decode {hex}");
			Assert.That(d.Len, Is.EqualTo(4), $"len {hex}");
			var txt = Disassembler.Disassemble(b, 0, XMode.Bits64).Text;
			Assert.That(txt, Does.StartWith(mnem), $"mnem {hex} -> {txt}");
			// the L-bit must reach the RENDER: an L=1 form saying xmm is the bug this catches
			Assert.That(txt, Does.Contain($"{reg}0, {reg}1, {reg}1"), $"L-width {hex} -> {txt}");
		}
		// the imm8 forms carry a 5th byte and a selector
		foreach(var (hex, mnem, reg) in new[] {
			("c5f0c6c100","vshufps","xmm"), ("c5f4c6c100","vshufps","ymm"),
			("c5f1c6c100","vshufpd","xmm"),
		}) {
			var b = Convert.FromHexString(hex);
			Assert.That(Disassembler.DecodeInsn(b, XMode.Bits64, out var d), Is.True, $"decode {hex}");
			Assert.That(d.Len, Is.EqualTo(5), $"len {hex}");
			var txt = Disassembler.Disassemble(b, 0, XMode.Bits64).Text;
			Assert.That(txt, Does.StartWith(mnem), $"mnem {hex} -> {txt}");
			Assert.That(txt, Does.Contain($"{reg}0, {reg}1, {reg}1"), $"L-width {hex} -> {txt}");
		}
	}

	[Test]
	public void VexScalarFloatDecodesAndDiesLoud() {
		// Every expectation below was read off `objdump -M intel` BEFORE the .isa edit:
		//   C5 F3 59 C1 = vmulsd xmm0,xmm1,xmm1   C5 F2 10 C1 = vmovss xmm0,xmm1,xmm1
		// VEX2 byte-2 = R<<7 | (~vvvv & 0xF)<<3 | L<<2 | pp; vvvv=xmm1 -> 0xE; pp 2=F3 3=F2.
		// DECODE-ONLY on purpose: SDM says DEST[127:64] := SRC1[127:64] (the vvvv operand),
		// and write_operand for an Xmm at width<128 preserves DEST's own upper -- those
		// agree only when dst == src1. Faithful semantics need a lane-insert primitive
		// that IlLower does not have, so a body here would be silently wrong on the
		// three-register form. Intrinsic-bodied => decode + disasm, die loud at EXEC.
		foreach(var (hex, mnem) in new[] {
			("c5f210c1", "vmovss"), ("c5f258c1", "vaddss"), ("c5f259c1", "vmulss"),
			("c5f25cc1", "vsubss"), ("c5f25ec1", "vdivss"),
			("c5f310c1", "vmovsd"), ("c5f358c1", "vaddsd"), ("c5f359c1", "vmulsd"),
			("c5f35cc1", "vsubsd"), ("c5f35ec1", "vdivsd"),
		}) {
			var b = Convert.FromHexString(hex);
			Assert.That(Disassembler.DecodeInsn(b, XMode.Bits64, out var d), Is.True, $"decode {hex}");
			Assert.That(d.Len, Is.EqualTo(4), $"length {hex}");   // Decode.cs:90 -- Len, not Length
			var txt = Disassembler.Disassemble(b, 0, XMode.Bits64).Text;
			Assert.That(txt, Does.StartWith(mnem), $"disasm {hex}");
			Assert.That(txt, Does.Contain("xmm1, xmm1"), $"vvvv+rm both xmm1: {hex} -> {txt}");
			// die-loud lives at EXEC (RunIntrinsic), not at Lift -- verified, not assumed
			var m = new X86Machine(); m.Xmm[0] = 0; m.Xmm[1] = 0;
			Assert.That(() => { X86Lifter.Lift(in d, 0, XMode.Bits64); }, Throws.Nothing, $"lift {hex}");
		}
	}

	[Test]
	public void RetiClassifiesAsRetNotJmp() {
		// `ret imm16` (0xC2) lowered to BranchKind.Jmp, so every consumer scanning for
		// Ret was blind to stdcall returns. A full-scope capstone diff over a 44MB
		// binary found 5,923 misclassified and ~2,000 function heads never minted
		// downstream, because a `ret imm16` ends a function invisibly.
		// EXECUTION was always right; CLASSIFICATION wasn't.
		foreach(var (hex, want, kind) in new[] {
			("c20800", "ret",  BranchKind.Ret),   // ret 8    -- the fix
			("c3",     "ret",  BranchKind.Ret),   // ret      -- must not regress
			("e900000000", "jmp", BranchKind.Jmp),// jmp rel32 -- must stay Jmp
			("e800000000", "call", BranchKind.Call),
		}) {
			var b = Convert.FromHexString(hex);
			Assert.That(Disassembler.DecodeInsn(b, XMode.Bits64, out var d), Is.True, hex);
			var blk = X86Lifter.Lift(in d, 0, XMode.Bits64);
			var br = blk.Body.OfType<IlBranch>().Single();   // IlBlock(IReadOnlyList<Il> Body) @LiftIl/Il.cs:155
			Assert.That(br.Kind, Is.EqualTo(kind), $"{want} ({hex}) branch-kind");
		}
	}

	[Test]
	public void NewDecodeGapsDecodeAndDieLoud() {
		// The full-scope M0 gate (11,320,255 lengths vs capstone) named the
		// undecodables. A missing ENCODING desynchronises a linear sweep for the
		// rest of the walk -- a corpus-wide cost. A missing SEMANTICS is one
		// instruction that dies loud. So these land intrinsic-bodied: decode is
		// what the corpus needs, and the v128 carrier question stays where it is.
		// Each expectation was read off objdump -M intel BEFORE the .isa edit.
		foreach(var (hex, want) in new[] {
			("660fd2c1", "psrld"), ("660ff2c1", "pslld"), ("660fe2c1", "psrad"),
			("660fd3c1", "psrlq"), ("660ff3c1", "psllq"), ("660fd1c1", "psrlw"),
			("660ff1c1", "psllw"), ("660fe1c1", "psraw"),
			("660f3840c1", "pmulld"), ("660f383dc1", "pmaxsd"), ("660f383cc1", "pmaxsb"),
			("660f3839c1", "pminsd"), ("660f3838c1", "pminsb"), ("660f383fc1", "pmaxud"),
			("660f383ec1", "pmaxuw"),
		}) {
			var b = Convert.FromHexString(hex);
			Assert.That(Disassembler.DecodeInsn(b, XMode.Bits64, out var d), Is.True,
				$"{want} ({hex}) must DECODE -- an undecodable byte desyncs the sweep");
			Assert.That(Disassembler.Disassemble(b, 0, XMode.Bits64).Text, Does.Contain(want),
				$"{hex} must disassemble as {want}");
			// The SEMANTICS must die loud. Lift SUCCEEDS -- it builds
			// (void intrin.<name> ...) -- so the die-loud property lives at EXEC, in
			// RunIntrinsic (X86Machine.cs:167-171: StringOp, then the OnIntrin host
			// shim, then throw). Observed before asserting: lifting psrld yields
			// `(void intrin.psrld (u128 XMM0) (u128 XMM1))`, which is why the assert
			// is on Step() and not on Lift().
			var m = new X86Machine { Mode = XMode.Bits64, Mem = new byte[0x2000], Ip = 0x100 };
			b.CopyTo(m.Mem, 0x100);
			Assert.That(() => m.Step(), Throws.TypeOf<NotSupportedException>()
				.With.Message.Contains(want),
				$"{want} is intrinsic-bodied: EXEC must throw naming it, not compute a lie");
		}
	}

	[Test]
	public void ByteShiftImmThroughDecode() {  // 66 0F 73 DB 01 = psrldq xmm3, 1
		// The lowering census reports `vshift-bytes count not an imm bind` as a
		// blocker for PSRLDQ-I/PSLLDQ-I -- but the census binds every parameter as
		// a synthetic Reg, and the .isa's encoding is (Udq Ib), so the REAL decode
		// path binds count as an Imm. Those are different populations and only this
		// one is the shipping path. The head resolves the count at LOWER time
		// (compile-time-imm, matching RustLiftGen's emit-time resolution) so the
		// output must carry no runtime shift-by-variable.
		var il = LiftText("660f73db01");
		Assert.That(il, Is.Not.Null, "psrldq did not lift at all");
		Assert.That(il, Does.Not.Contain("op vshift-bytes"), "count bound as non-imm");
		// count=1 => a real 8-bit shift, so the constant must appear, not be folded away
		Assert.That(il, Does.Contain("shr"), "right-shift form expected for /3");
		// and the sibling direction (/7 = pslldq) must lower too
		var il2 = LiftText("660f73fa0f");   // pslldq xmm2, 15
		Assert.That(il2, Is.Not.Null, "pslldq did not lift");
		Assert.That(il2, Does.Contain("shl"), "left-shift form expected for /7");
	}

	[Test]
	public void AddRegRegThroughDecode() {  // 01 D8 = add eax, ebx — golden row 1 via decode
		var il = LiftText("01d8");
		// same shapes as the golden (RAX=reg0 bound from ModRM.rm, RBX=reg3 from reg)
		Assert.That(il, Does.Contain("(let %0 = (u32 trunc (u64 RAX)))"));
		Assert.That(il, Does.Contain("(let %1 = (u32 trunc (u64 RBX)))"));
		Assert.That(il, Does.Contain("(let %2 = (u32 add (u32 %0) (u32 %1)))"));
		Assert.That(il, Does.Contain("(RAX := (u64 zext (u32 %2)))"));
		Assert.That(il, Does.Contain("(EFLAGS.C := (u1 or (u1 ult (u32 %2) (u32 %0)) (u1 ult (u32 %2) (u32 %1))))"));
		Assert.That(il, Does.Contain("(EFLAGS.Z := (u1 eq (u32 %2) (u32 #0)))"));
	}

	[Test]
	public void AddMemRegThroughDecode() {  // 01 5D F0 = add [rbp-0x10], ebx — golden row 2
		var il = LiftText("015df0");
		Assert.That(il, Does.Contain("(let %0 = (u64 add (u64 RBP) (u64 #fffffffffffffff0)))"));
		Assert.That(il, Does.Contain("(u32 load (u64 %0))"));
		Assert.That(il, Does.Contain("(store (u64 %0)"));
		Assert.That(il.Split("%0 =").Length, Is.EqualTo(2));  // addr evaluated ONCE
	}

	[Test]
	public void SibAddr() {  // 8B 44 8A 04 = mov eax, [rdx+rcx*4+4]
		var il = LiftText("8b448a04");
		Assert.That(il, Does.Contain("(u64 add (u64 add (u64 RDX) (u64 shl (u64 RCX) (u64 #2))) (u64 #4))"));
	}

	[Test]
	public void RipRelAddr() {  // 8B 05 10 00 00 00 = mov eax, [rip+0x10] (len 6)
		var il = LiftText("8b0510000000", pc: 0x1000);
		Assert.That(il, Does.Contain("(u64 add (u64 add (u64 pc) (u64 #6)) (u64 #10))"));
	}

	[Test]
	public void GsSegAddr() {  // 65 48 8B 04 25 28 00 00 00 = mov rax, gs:[0x28]
		var il = LiftText("65488b042528000000");
		Assert.That(il, Does.Contain("(u64 add (u64 GS) (u64 #28))"));
	}

	[Test]
	public void ImmBind() {  // 83 C0 05 = add eax, 5 (Ib-sx)
		var il = LiftText("83c005");
		Assert.That(il, Does.Contain("(let %1 = (u32 #5))"));  // Ib-sx bound at dest width, mlet-bound
	}

	[Test]
	public void PushThroughDecode() {  // 55 = push rbp
		var il = LiftText("55");
		Assert.That(il, Does.Contain("(RSP := (u64 sub (u64 RSP) (u64 #8)))"));
		Assert.That(il, Does.Contain("(store (u64 RSP)"));
	}

	[Test]
	public void IntrinsicThroughDecode() {  // A7 = cmpsd (dword string-compare)
		// RETARGETED 2026-08-20 from `f3480fbcc1` (tzcnt rax,rcx). TZCNT stopped
		// being intrinsic-bodied on 2026-08-09 (0327ba6) — its .isa body is now
		// `(mlet (r (clz (rbit src)) cf (== src 0)) ...)`, with the mlet-capture
		// there because CF must read src BEFORE dst is written (the third .isa-tier
		// bug the silicon sweep found). This assert had been checking the
		// pre-rewrite shape for a month. CMPSV is still `(intrinsic cmps ...)`, so
		// the test keeps its purpose — an intrinsic marker surviving a real decode —
		// against a subject that still exists.
		var il = LiftText("a7");
		Assert.That(il, Does.Contain("(void intrin.cmps"));
	}

	[Test]
	public void CmovThroughDecode() {  // 0F 42 C1 = cmovb eax, ecx → IlIfV
		var il = LiftText("0f42c1");
		Assert.That(il, Does.Contain("if (u1 EFLAGS.C)"));
	}

	// --- 16-bit mode ("could we run DOS?") — XED -16 verified ---
	[TestCase("b409", "mov ah, 0x9")]              // DOS print-string setup
	[TestCase("cd21", "int 0x21")]                 // THE DOS syscall
	[TestCase("55", "push bp")]
	[TestCase("89e5", "mov bp, sp")]
	[TestCase("b8004c", "mov ax, 0x4c00")]         // exit(0)
	[TestCase("8b4602", "mov ax, word ptr [bp+0x2]")]  // 16-bit ModRM table (BP+disp)
	[TestCase("f3a5", "rep movsw word ptr [di], word ptr [si]")]
	[TestCase("8ed8", "mov ds, ax")]               // segment reg move
	[TestCase("e8fe00", "call 0x201")]             // 16-bit rel, IP-wrap space
	[TestCase("26a10200", "mov ax, word ptr es:[0x2]")]  // seg-override moffs
	public void Dos16Decode(string hex, string expected) {
		var (text, _) = Disassembler.Disassemble(Convert.FromHexString(hex), 0x100, XMode.Bits16);
		Assert.That(text, Is.EqualTo(expected));
	}

	[Test]
	public void Dos16Lifts() {  // the same rows lift (16-bit widths through the walker)
		foreach(var hex in new[] { "b409", "cd21", "55", "89e5", "b8004c", "8b4602", "8ed8" }) {
			var il = LiftText(hex, XMode.Bits16, 0x100);
			Assert.That(il, Is.Not.Null, hex);
		}
		// spot semantics: push bp in 16-bit = SP-2, word store
		var push = LiftText("55", XMode.Bits16);
		Assert.That(push, Does.Contain("(RSP := (u64 sub (u64 RSP) (u64 #2)))"));
		// mov ah, 9: high-8 write = masked insert at bits 8-15... AH is reg4 in
		// byte-file terms — pinned below in Dos16AhWrite.
	}

	// --- the IL branch contract (consumer step-2a NOTES 689c): arch-neutral
	// scanners read IlBranch(Kind, ABSOLUTE-target[, Cond]) — same as aarch64 BL ---
	[Test]
	public void CallEmitsCallKindAbsoluteTarget() {  // E8 FB 05 00 00 @ pc=0x1000, len 5 → target 0x1600
		var il = LiftText("e8fb050000", pc: 0x1000);
		Assert.That(il, Does.Contain("(call (u64 #1600))"));       // Kind=Call, abs
		Assert.That(il, Does.Contain("(store (u64 RSP)"));         // return-addr push intact
	}

	[Test]
	public void RetEmitsRetKind() {  // C3
		var il = LiftText("c3");
		Assert.That(il, Does.Contain("(ret (u64 %0))"));           // Kind=Ret, popped target
	}

	[Test]
	public void JccEmitsCondJmpWithCondField() {  // 74 10 = jz +0x10 @0, len 2 → 0x12
		var il = LiftText("7410");
		Assert.That(il, Does.Contain("condjmp"));
		Assert.That(il, Does.Contain("#12"));                       // absolute, not raw rel
		Assert.That(il, Does.Contain("EFLAGS.Z"));                  // cond rides the node
		Assert.That(il, Does.Not.Contain("(if "));                  // NO IlIf wrapper
	}

	[Test]
	public void JmpRelIsAbsolute() {  // EB FE = jmp -2 (self) @ pc=0x400
		var il = LiftText("ebfe", pc: 0x400);
		Assert.That(il, Does.Contain("(jmp (u64 #400))"));
	}

	[Test]
	public void EveryDecodableTestRowLifts() {
		// smoke: every hex in the disasm test corpus that DECODES also LIFTS non-null.
		// (The lift arm must never be narrower than the decoder.)
		var failures = new List<string>();
		foreach(var (hex, mode) in DecodeTests.AllRows()) {
			var bytes = Convert.FromHexString(hex);
			if(!Disassembler.DecodeInsn(bytes, mode, out var d)) continue;
			try {
				if(X86Lifter.Lift(in d, 0, mode) == null) failures.Add(hex + " → null");
			} catch(Exception e) {
				failures.Add($"{hex} → {e.Message}");
			}
		}
		Assert.That(failures, Is.Empty, string.Join("\n", failures.Take(12)));
	}

	/// EveryDecodableTestRowLifts' comment claims "the lift arm must never be narrower
	/// than the decoder" -- a TOTAL-FUNCTION claim over the def set. It iterates
	/// DecodeTests.AllRows(), which is 108 HAND-WRITTEN hex rows. So the claim was
	/// gated by a sample of whatever those rows happen to decode to, and it surfaced 3
	/// unlowered heads (vmovmsk/vibin/vshuf) while 83 of 518 defs use one.
	///
	/// This walks the DEF SET instead, via the generated table every backend reads,
	/// with SYNTHETIC binds -- so a template's lowerability is tested independently of
	/// whether anyone wrote a hex row that reaches it. Not a replacement for the
	/// corpus arm (that one tests the decoder->binder->lifter CHAIN on real bytes);
	/// this one answers the question the corpus arm's comment was making.
	///
	/// Measured at authoring, and the ratchet is deliberate: the count is asserted
	/// rather than the set being required empty, because the vector tier is a real
	/// unbuilt thing (IlLower has no vector head; MaxwellShader's forked IL does, at
	/// MaxwellEval.Expr with an `object` carrier where X86Machine.Eval's is `ulong`).
	/// A NEW unlowered head must fail here rather than wait for someone to write a
	/// hex row for it -- which is the hole this arm exists to close.
	[Test]
	public void EveryTemplateLowersOrIsAKnownVectorGap() {
		var vectorGap = new List<string>();
		// Template-ids whose lowering hit a missing head. Collected HERE rather than
		// re-derived, so the def-share below is a join on this same classification
		// (the two-instrument gap that let a plant through the old ceiling).
		var gapTemplates = new List<int>();
		var other = new List<string>();

		for(var tid = 0; tid < LiftTables.Templates.Length; tid++) {
			var (mnem, paramsText, evalText) = LiftTables.Templates[tid];
			var ps = paramsText.Length == 0
				? new List<string>()
				: paramsText.Split(' ').ToList();

			// Synthetic binds, tried in THREE SHAPES. The first cut used uniform
			// 64-bit GPRs and 3 templates failed on the bind SHAPE rather than on a
			// head: LEA's `addr-of` needs a Mem bind ("addr-of non-mem operand") and
			// PSRLDQ-I/PSLLDQ-I's `vshift-bytes` needs an Imm ("count not an imm
			// bind"). Both throws were CORRECT -- IlLower has cases for those heads
			// and my operands were wrong. So the arm asks "does SOME valid bind
			// shape lower this template", which is the question that isolates head
			// coverage from bind-shape guessing. A template failing under all three
			// still lands in `other` and is visible.
			//   (Found by the arm's own must-fail branch on its first fire. The
			//    uniform version would have shipped as 3 permanent known-failures
			//    attributed to the vector gap -- which is the sample-gated hiding
			//    this arm exists to stop, one layer in.)
			// The 4th shape is MIXED and it exists because the uniform ones can't
			// express the commonest x86 form: PSRLDQ-I/PSLLDQ-I WRITE their first
			// operand and read the last as a count, so all-Reg throws "count not an
			// imm bind" and all-Imm throws "write to Imm". Neither throw is a head
			// gap; both are my operands. Found by the arm's must-fail branch on
			// consecutive fires -- the all-Imm shape fixed LEA and exposed this.
			var shapes = new Func<int, OperandBind>[] {
				k => new OperandBind.Reg(k % 16, 64),
				k => new OperandBind.Mem(new IlConst(IlType.U64, 0x2000), 64),
				k => new OperandBind.Imm(1, 8),
				k => k == ps.Count - 1 && ps.Count > 1
					? new OperandBind.Imm(1, 8)
					: new OperandBind.Reg(k % 16, 64),
			};

			string headGap = null, lastOther = null;
			var lowered = false;
			foreach(var shape in shapes) {
				var binds = new Dictionary<string, OperandBind>();
				for(var k = 0; k < ps.Count; k++) binds[ps[k]] = shape(k);
				binds["%nextpc"] = new OperandBind.Imm(0x1000, 64);
				try {
					var forms = ((PList) ListParser.Parse(evalText)[0]).Skip(1).ToList();
					if(IlLower.Lower(ps, forms, binds, 64) != null) { lowered = true; break; }
					lastOther = "null";
				} catch(NotSupportedException e) when(e.Message.StartsWith("op ")) {
					// "op <head>" is IlLower.cs:537's own default-arm text -- the
					// exact signal this arm is for. A missing head is bind-shape
					// INDEPENDENT, so record it and stop: no other shape will help.
					headGap = e.Message[3..];
					break;
				} catch(Exception e) {
					lastOther = $"{e.GetType().Name}: {e.Message}";
				}
			}

			if(headGap != null) { vectorGap.Add(headGap); gapTemplates.Add(tid); }
			else if(!lowered) other.Add($"{tid} {mnem} -> {lastOther} (all 3 bind shapes)");
		}

		// The non-head failures are the ones with no known cause. Report them first
		// and in full: a count would hide which template, and this arm's whole point
		// is that a sample-gated claim hides members.
		Assert.That(other, Is.Empty,
			$"templates failing for a reason OTHER than a missing IlLower head "
			+ $"({other.Count}):\n{string.Join("\n", other.Take(12))}");

		// The DEF-SHARE, computed by THIS walk rather than by a separate census.
		// I first quoted "83 of 518 defs" from a python parse of the .isa text --
		// a different instrument than the one producing the head list, which is
		// exactly the gap that let a planted head through the old count-ceiling.
		// LiftTables.Defs maps DefId -> TemplateId ([0] unused), so the share is a
		// join on the template-ids this same loop already classified.
		var gapTids = new HashSet<int>(gapTemplates);
		var affectedDefs = 0;
		for(var d = 1; d < LiftTables.Defs.Length; d++)
			if(gapTids.Contains(LiftTables.Defs[d].TemplateId)) affectedDefs++;
		TestContext.Out.WriteLine(
			$"def-set lift census: {LiftTables.Templates.Length} templates, "
			+ $"{LiftTables.Defs.Length - 1} defs; {gapTids.Count} templates / "
			+ $"{affectedDefs} defs reach a head IlLower lacks");

		var heads = vectorGap.Distinct().OrderBy(h => h).ToList();

		// The EXPECTED SET, not a count, and derived from THIS ARM rather than from
		// a sibling instrument. The first version asserted `Count <= 21` -- 21 came
		// from a python census of the .isa text, while the arm itself observes 16
		// (some heads only appear in templates where another head throws first). A
		// planted `zzsynthhead` then took the count to 16 and PASSED, because the
		// 5-slot gap between the two instruments was slack a new head could hide in.
		//   A CEILING FROM A DIFFERENT INSTRUMENT THAN THE ASSERT IS NOT A GATE.
		// The set form has no slack: a new head fails BY NAME, and a head that gains
		// an IlLower case also fails -- which is correct, because the vector tier
		// landing should update this list deliberately rather than silently.
		var known = new[] {
			"fcmpp", "vcvt", "vdpp", "vfbin", "vfcmpp", "vfmax", "vfmin", "vfun",
			"vhadd", "vibin", "vishi", "vmovmsk", "vshuf", "vshufw", "vzip",
		};
		Assert.That(heads, Is.EquivalentTo(known),
			$"the unlowered-head set MOVED. now ({heads.Count}): {string.Join(" ", heads)}\n"
			+ $"expected ({known.Length}): {string.Join(" ", known)}\n"
			+ "an ADDED head = a template whose semantics no C# backend can evaluate; "
			+ "a REMOVED head = the vector tier gained a case, so update this list.");
	}
}
