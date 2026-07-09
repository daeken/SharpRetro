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
	public void IntrinsicThroughDecode() {  // F3 48 0F BC C1 = tzcnt rax, rcx
		var il = LiftText("f3480fbcc1");
		Assert.That(il, Does.Contain("(void intrin.tzcnt (u64 RAX) (u64 RCX))"));
	}

	[Test]
	public void CmovThroughDecode() {  // 0F 42 C1 = cmovb eax, ecx → IlIfV
		var il = LiftText("0f42c1");
		Assert.That(il, Does.Contain("if (u1 EFLAGS.C)"));
	}

	// --- 16-bit mode (sera ·76: "could we run DOS?") — XED -16 verified ---
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

	// --- the IL branch contract (barrow step-2a ·NOTES 689c): arch-neutral
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
