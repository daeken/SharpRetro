using XFusionCpu;
using LiftIl;

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
}
