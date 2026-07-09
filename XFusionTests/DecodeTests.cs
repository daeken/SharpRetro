using XFusionCpu;

namespace XFusionTests;

/// Layer-2 decode tests (3-layer test plan): bytes → (text, length) rows.
/// EVERY expected string here was verified against Intel XED (obj/wkit/examples/obj/xed,
/// v2026.06.29, aarch64 host build) on 2026-07-09 — not written from memory.
/// Convention: XED "SHORT" Intel-syntax rendering is our canonical disasm format.
public class DecodeTests {
	static void Row(string hex, XMode mode, string expected) {
		var bytes = Convert.FromHexString(hex.Replace(" ", ""));
		var (text, len) = Disassembler.Disassemble(bytes, 0, mode);
		Assert.That(text, Is.EqualTo(expected), $"bytes {hex} ({mode})");
		Assert.That(len, Is.EqualTo(bytes.Length), $"length of {hex} ({mode})");
	}

	// --- ADD: all 9 encodings, 32-bit mode ---
	[TestCase("00d8", "add al, bl")]                       // 00 /r  Eb Gb
	[TestCase("0045f0", "add byte ptr [ebp-0x10], al")]    // 00 /r  mem form
	[TestCase("01d8", "add eax, ebx")]                     // 01 /r  Ev Gv
	[TestCase("02d8", "add bl, al")]                       // 02 /r  Gb Eb
	[TestCase("03042544332211", "add eax, dword ptr [0x11223344]")]  // 03 /r + SIB disp32
	[TestCase("04fb", "add al, 0xfb")]                     // 04 ib
	[TestCase("0500000080", "add eax, 0x80000000")]        // 05 iz
	[TestCase("80c105", "add cl, 0x5")]                    // 80 /0 ib
	[TestCase("81c078563412", "add eax, 0x12345678")]      // 81 /0 iz
	[TestCase("83c0fb", "add eax, 0xfffffffb")]            // 83 /0 ib-sx (mask-to-v verified vs XED)
	public void Add32(string hex, string expected) => Row(hex, XMode.Bits32, expected);

	// --- operand-size prefix ---
	[TestCase("6601d8", "add ax, bx")]
	[TestCase("6683c0fb", "add ax, 0xfffb")]
	public void Add32OpSize(string hex, string expected) => Row(hex, XMode.Bits32, expected);

	// --- MOV ---
	[TestCase("88e1", "mov cl, ah")]
	[TestCase("8b4c2408", "mov ecx, dword ptr [esp+0x8]")]
	[TestCase("c7042478563412", "mov dword ptr [esp], 0x12345678")]
	[TestCase("89d8", "mov eax, ebx")]
	public void Mov32(string hex, string expected) => Row(hex, XMode.Bits32, expected);

	// --- PUSH/POP r/m ---
	[TestCase("ff75fc", "push dword ptr [ebp-0x4]")]
	[TestCase("8f45f8", "pop dword ptr [ebp-0x8]")]
	public void Stack32(string hex, string expected) => Row(hex, XMode.Bits32, expected);

	// --- 16-bit addressing via 0x67 in 32-bit mode ---
	[TestCase("678b00", "mov eax, dword ptr [bx+si*1]")]  // XED renders *1 explicitly here
	public void AdSize32(string hex, string expected) => Row(hex, XMode.Bits32, expected);

	// --- 64-bit mode: REX ---
	[TestCase("4801d8", "add rax, rbx")]                   // REX.W
	[TestCase("4883c005", "add rax, 0x5")]                 // REX.W + 83 /0
	[TestCase("4883c0fb", "add rax, 0xfffffffffffffffb")]  // sx to 64
	[TestCase("4d8b4108", "mov r8, qword ptr [r9+0x8]")]   // REX.WRB
	[TestCase("8b0544332211", "mov eax, dword ptr [rip+0x11223344]")]  // RIP-relative
	[TestCase("4088e1", "mov cl, spl")]                    // bare REX flips AH→SPL
	[TestCase("88e1", "mov cl, ah")]                       // no REX keeps AH
	[TestCase("6741034c9d20", "add ecx, dword ptr [r13d+ebx*4+0x20]")]  // 67 + REX.B SIB
	[TestCase("4c8b142500104000", "mov r10, qword ptr [0x401000]")]     // SIB base=101 mod=00 disp32
	[TestCase("83c005", "add eax, 0x5")]                   // default 32 in long mode
	public void Long64(string hex, string expected) => Row(hex, XMode.Bits64, expected);

	// --- wave-2: +r forms, extends, shifts, unary group (XED-verified 2026-07-09) ---
	[TestCase("55", "push rbp")]                     // 50+r
	[TestCase("4155", "push r13")]                   // REX.B extends +r
	[TestCase("5d", "pop rbp")]
	[TestCase("b80e000000", "mov eax, 0xe")]         // B8+r Iv
	[TestCase("48b8efbeaddeefbeadde", "mov rax, 0xdeadbeefdeadbeef")]  // REX.W = true imm64
	[TestCase("41bc01000000", "mov r12d, 0x1")]
	[TestCase("480fb6c3", "movzx rax, bl")]          // dest/src different widths
	[TestCase("0fb7c0", "movzx eax, ax")]
	[TestCase("4863c8", "movsxd rcx, eax")]
	[TestCase("48c1e03f", "shl rax, 0x3f")]
	[TestCase("d1e8", "shr eax, 0x1")]               // by-1 form renders 0x1
	[TestCase("48f7d8", "neg rax")]
	[TestCase("f7e1", "mul ecx")]
	[TestCase("480fafc1", "imul rax, rcx")]
	[TestCase("6bc00a", "imul eax, eax, 0xa")]       // 3-operand
	[TestCase("4898", "cdqe")]                       // wname: REX.W selects cdqe
	[TestCase("98", "cwde")]
	[TestCase("0f95c0", "setnz al")]
	[TestCase("490f44c4", "cmovz rax, r12")]
	[TestCase("f390", "pause")]                      // mandatory prefix consumed
	[TestCase("f3c3", "ret")]                        // stray rep dropped (rep-ret idiom)
	public void Wave2(string hex, string expected) => Row(hex, XMode.Bits64, expected);

	[TestCase("66b84523", "mov ax, 0x2345")]         // B8+r under opsize: Iv = imm16
	public void Wave2Mode32(string hex, string expected) => Row(hex, XMode.Bits32, expected);

	// --- undecodable / boundary ---
	[Test]
	public void UnknownOpcodeReturnsNull() {
		var (text, len) = Disassembler.Disassemble(Convert.FromHexString("0F05"), 0, XMode.Bits32);  // syscall not defined yet
		Assert.That(text, Is.Null);
		Assert.That(len, Is.EqualTo(0));
	}

	[Test]
	public void TruncatedModRmReturnsNull() {
		var (text, _) = Disassembler.Disassemble(Convert.FromHexString("01"), 0, XMode.Bits32);  // 01 needs ModRM
		Assert.That(text, Is.Null);
	}

	[Test]
	public void EmptyReturnsNull() {
		var (text, _) = Disassembler.Disassemble(ReadOnlySpan<byte>.Empty, 0, XMode.Bits32);
		Assert.That(text, Is.Null);
	}
}
