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
