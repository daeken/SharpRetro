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
	public void FetchHookExecFilter() {  // FetchHook distinct from LoadHook (barrow's step-1.5(c) ‡)
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
	public void MemHooksFallback() {  // no Mem[] — fetch + load + store all via hooks (barrow's X86Env pattern ·124)
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
