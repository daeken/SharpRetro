using LiftIl;

namespace XFusionCpu;

/// M4 (B)-arm: execute lifted LiftIl blocks against mutable x86 state.
/// One semantics source: the SAME IlBlocks Pagentry lifts are what runs —
/// decode→lift→exec per instruction (per-DefId caches make lift cheap-ish;
/// the generated-C# interpreter is a later perf call, oracled by this).
///
/// Scope: the 16 node kinds x86 blocks emit (Const/ReadReg/ReadPc/Tmp/Bin/Un/
/// Cast/Load/IfV/Intrin + Block/Let/WriteReg/Store/Branch/If). Vector nodes =
/// not yet (x86 vector semantics are IlIntrin-opaque anyway). Intrinsics
/// dispatch to a handler the host installs (the DOS int-21 shim path).
public class X86Machine {
	// GPR file: RAX..RDI, R8..R15 (RegKind.X86 index space). IP separate.
	public readonly ulong[] Gpr = new ulong[16];
	public ulong Ip;
	// Eflags bits (architectural positions: CF=0 PF=2 AF=4 ZF=6 SF=7 DF=10 OF=11)
	public ulong Flags = 0x2;  // bit 1 always set
	// Segment BASES (real mode: selector<<4 updated on seg write; protected: flat 0)
	public readonly ulong[] SegBase = new ulong[6];
	public readonly ushort[] SegSel = new ushort[6];

	public byte[] Mem;
	/// XMM scalar-lane values, as bit patterns. 32 slots per RegKind.Xmm's documented
	/// range. THE FULL 128 BITS: the carrier was widened at de0c231 and both silicon
	/// goldens now grade the high half (p1 2,473,490 / p2 3,700,000, diff=0 skip=0).
	/// Before that this said "NOT the full 128 bits: Eval's carrier is ulong" -- true
	/// when written, false for ~an hour after, and a peer reading it against what I'd
	/// told them is what made them check ancestry rather than trust either.
	public readonly UInt128[] Xmm = new UInt128[32];
	/// x87 stack slots, same carrier caveat (f64 bit patterns, no 80-bit extended).
	public readonly ulong[] St = new ulong[8];
	/// Instruction-fetch hook — DISTINCT from LoadHook so a host can
	/// exec-filter (the X86Env consumer's step-1.5(c) ‡: LoadHook serves both
	/// fetch and data, so his Segment.Exec check can't discriminate).
	/// Fill up to 15 bytes at addr; return false if addr isn't executable
	/// (Step() returns false = clean [not-exec] fault). Null → Step falls
	/// through to Mem[]/Load() (current behavior — behaviorally transparent).
	public delegate bool FetchDelegate(ulong addr, Span<byte> into);
	public FetchDelegate FetchHook;
	/// Overlay-miss fallback: the X86Env consumer sets this to read Image bytes
	/// underneath a write-overlay when Mem is null / addr out-of-range.
	/// Called by Load() when Mem doesn't cover; return the value at addr.
	public Func<ulong, int, ulong> LoadHook;
	/// Write hook (MMIO / dirty-tracking); default null = write to Mem[].
	/// Return true to consume (Mem[] not written).
	public Func<ulong, ulong, int, bool> StoreHook;

	public ulong Load(ulong a, int w) {
		if(Mem == null || a + (ulong) (w / 8) > (ulong) Mem.Length)
			return LoadHook?.Invoke(a, w)
				?? throw new IndexOutOfRangeException($"Load @{a:X} w={w} (no Mem, no LoadHook)");
		var v = 0UL;
		for(var b = 0; b < w / 8; b++) v |= (ulong) Mem[(int) (a + (ulong) b)] << (b * 8);
		return v;
	}
	public void Store(ulong a, ulong val, int w) {
		if(StoreHook?.Invoke(a, val, w) == true) return;
		if(Mem == null) throw new IndexOutOfRangeException($"Store @{a:X} w={w} (no Mem)");
		for(var b = 0; b < w / 8; b++) Mem[(int) (a + (ulong) b)] = (byte) (val >> (b * 8));
	}
	public XMode Mode = XMode.Bits16;

	/// Intrinsic handler: (machine, name, evaluated-args) → handled?
	/// The DOS shim installs itself here (int, in/out, string ops...).
	public Func<X86Machine, string, ulong[], bool> OnIntrin;

	/// Branch taken during the last Step (consumed by Step to set IP).
	ulong? _branchTo;
	bool _halted;
	public bool Halted => _halted;

	readonly Dictionary<int, UInt128> _tmps = [];

	public void Halt() => _halted = true;

	/// Execute one instruction at IP. Returns false if undecodable or halted.
	public bool Step() {
		if(_halted) return false;
		var lin = SegBase[1] + Ip;  // CS base + IP (flat modes: SegBase[1]=0)
		DecodedInsn d;
		if(FetchHook != null) {
			Span<byte> fbuf = stackalloc byte[15];
			if(!FetchHook(lin, fbuf)) return false;   // host says not-executable
			if(!Disassembler.DecodeInsn(fbuf, Mode, out d)) return false;
		} else if(Mem != null && lin + 15 <= (ulong) Mem.Length) {
			if(!Disassembler.DecodeInsn(Mem.AsSpan((int) lin, 15), Mode, out d)) return false;
		} else {  // hook-backed fetch fallback via Load() (no distinct FetchHook set)
			Span<byte> fbuf = stackalloc byte[15];
			for(var i = 0; i < 15; i++) fbuf[i] = (byte) Load(lin + (ulong) i, 8);
			if(!Disassembler.DecodeInsn(fbuf, Mode, out d)) return false;
		}
		var block = X86Lifter.Lift(in d, Ip, Mode);
		if(block == null) return false;
		_branchTo = null;
		_tmps.Clear();
		ExecBlock(block.Body);
		Ip = _branchTo ?? Ip + (ulong) d.Len;
		if(Mode == XMode.Bits16) Ip &= 0xFFFF;
		else if(Mode == XMode.Bits32) Ip &= 0xFFFFFFFF;
		return !_halted;
	}

	void ExecBlock(IReadOnlyList<Il> body) {
		foreach(var stmt in body) {
			Exec(stmt);
			if(_branchTo != null || _halted) return;  // branch/halt ends the insn
		}
	}

	void Exec(Il s) {
		switch(s) {
			case IlLet(var id, var v): _tmps[id] = Eval(v); break;
			case IlWriteReg(RegKind.X86, var i, var v): Gpr[i] = N64(Eval(v)); break;
			case IlWriteReg(RegKind.Eflags, var bit, var v):
				if(bit < 0) Flags = N64(Eval(v)) | 2;
				else Flags = (Flags & ~(1UL << bit)) | ((N64(Eval(v)) & 1) << bit);
				break;
			// XMM/x87: REAL files. They used to alias RegKind.X86 via OperandBind.Reg's
			// missing File field, so an xmm write clobbered a GPR. Scalar lanes only --
			// The carrier is UInt128 (de0c231) and the four IlVec* kinds IlLower emits
			// are evaluated per-lane (b808cc3), so a packed op computes rather than
			// dying at the default. This said "a v128 op still dies loud... nothing
			// emits or evaluates them yet" -- both halves now false: IlLower emits at
			// 16 sites and this file has the arms.
			case IlWriteReg(RegKind.Xmm, var i, var v): Xmm[i] = Eval(v); break;
			case IlWriteReg(RegKind.St, var i, var v): St[i] = N64(Eval(v)); break;
			case IlWriteReg(RegKind.X86Seg, var i, var v): {
				var sel = (ushort) N64(Eval(v));
				SegSel[i] = sel;
				if(Mode == XMode.Bits16) SegBase[i] = (ulong) sel << 4;  // real mode
				break;
			}
			case IlStore(var addr, var v):
				Store(N64(Eval(addr)), N64(Eval(v)), (v.Ty as IlType.I)?.Bits ?? 64);
				break;
			case IlBranch(var kind, var target, var cond):
				if(cond == null || (Eval(cond) & 1) != 0)
					_branchTo = N64(Eval(target));
				break;
			case IlIf(var c, var then, var els):
				ExecBlock((Eval(c) & 1) != 0 ? then : els);
				break;
			case IlIntrin(_, var name, var args): {
				// RunIntrinsic takes ulong[] -- every intrinsic here is 64-bit-or-less by
				// contract (x87 stack ops, string ops, the host-query family). A V128
				// intrinsic would need the wide overload, and there is none to bind.
				var vals = args.Select(a => N64(Eval(a))).ToArray();
				RunIntrinsic(name, vals);
				break;
			}
			case IlNote: break;
			default: throw new NotSupportedException($"exec {s.GetType().Name}");
		}
	}

	/// The single intrinsic dispatch — StringOp (machine-native: mul/div/
	/// loop/string) then OnIntrin (host shim) then throw. Public so the
	/// recompiler bridge can B.Call into the SAME handler code (one
	/// semantics source; TestRecompiler was no-op'ing intrinsics = silent
	/// divergence, my identity's open_questions[0]). branchTo out for the
	/// loop/jcxz family (they set _branchTo internally).
	public void RunIntrinsic(string name, ulong[] args, out ulong? branchTo) {
		var save = _branchTo;
		_branchTo = null;
		if(Ordering(name)) { branchTo = null; _branchTo = save; return; }
		var handled = StringOp(name, args);
		branchTo = _branchTo;
		_branchTo = save ?? _branchTo;  // preserve any prior branch; loop's branch wins if set
		if(handled) return;
		if(OnIntrin == null || !OnIntrin(this, name, args))
			throw new NotSupportedException($"unhandled intrinsic {name}");
	}
	// Overload for the exec path (branchTo → _branchTo directly).
	void RunIntrinsic(string name, ulong[] args) {
		if(Ordering(name)) return;
		if(StringOp(name, args)) return;
		if(OnIntrin == null || !OnIntrin(this, name, args))
			throw new NotSupportedException($"unhandled intrinsic {name}");
	}

	/// MEMORY-ORDERING BARRIERS ARE A NO-OP HERE, BY CONSTRUCTION, NOT BY OMISSION.
	/// MFENCE/LFENCE/SFENCE constrain the ORDER two observers see writes in; this machine
	/// has one thread and executes statements in program order, so there is no reordering
	/// for a barrier to forbid. The Rust arm lowers the same head to a real `dmb ish`
	/// because it JITs and runs multi-threaded guests -- the divergence is correct and is
	/// a property of the consumer, not of the .isa.
	///
	/// This threw `unhandled intrinsic fence` and was the ENTIRE p1 residual after the
	/// carrier widening -- 3 rows, one per mnemonic. A count could not say that: "3
	/// unlowered" and "99,198 unlowered" read identically, which is why XF_SKIPNAMES
	/// prints the per-def tally. Naming them turned a residual into a disposition.
	static bool Ordering(string name) => name == "fence";

	/// The string family, machine-native. Convention from the .isa: args[0] =
	/// width (literal or bitwidth-const) — but we re-derive everything from
	/// state; the NAME (incl rep_/repe_/repne_ prefix) + the width arg drive it.
	/// Real-mode segment bases honored (DS:SI src, ES:DI dst). DF advances.
	bool StringOp(string name, ulong[] args) {
		var baseName = name;
		var rep = RepKind.None;
		if(name.StartsWith("rep_")) { rep = RepKind.Rep; baseName = name[4..]; }
		else if(name.StartsWith("repe_")) { rep = RepKind.RepE; baseName = name[5..]; }
		else if(name.StartsWith("repne_")) { rep = RepKind.RepNe; baseName = name[6..]; }
		// wide mul/div (F6/F7 /4-/7): args = [width, src]. 8-bit uses AX (AL*src→AX,
		// AX/src→AL:AH); wider uses rDX:rAX. Div overflow/zero = #DE (throw — the
		// honest v1; interrupt vectoring is a later real-mode feature).
		if(baseName is "mul-wide" or "imul-wide" or "div-wide" or "idiv-wide") {
			var mw = (int) args[0];
			var src = args[1];
			void SetCfOf(bool v) {
				Flags = (Flags & ~((1UL << 0) | (1UL << 11))) | (v ? (1UL << 0) | (1UL << 11) : 0);
			}
			if(mw == 8) {
				var al = Gpr[0] & 0xFF;
				switch(baseName) {
					case "mul-wide": {
						var r = al * (src & 0xFF);
						Gpr[0] = (Gpr[0] & ~0xFFFFUL) | (r & 0xFFFF);
						SetCfOf((r >> 8) != 0);
						break;
					}
					case "imul-wide": {
						var r = (long) (sbyte) al * (sbyte) src;
						Gpr[0] = (Gpr[0] & ~0xFFFFUL) | ((ulong) r & 0xFFFF);
						SetCfOf(r != (sbyte) r);
						break;
					}
					case "div-wide": {
						var ax = Gpr[0] & 0xFFFF;
						var d0 = src & 0xFF;
						if(d0 == 0 || ax / d0 > 0xFF) throw new DivideByZeroException("#DE");
						Gpr[0] = (Gpr[0] & ~0xFFFFUL) | (ax / d0 & 0xFF) | ((ax % d0 & 0xFF) << 8);
						break;
					}
					case "idiv-wide": {
						var ax = (long) (short) (Gpr[0] & 0xFFFF);
						var d1 = (long) (sbyte) src;
						if(d1 == 0) throw new DivideByZeroException("#DE");
						var q = ax / d1;
						if(q != (sbyte) q) throw new DivideByZeroException("#DE overflow");
						Gpr[0] = (Gpr[0] & ~0xFFFFUL) | ((ulong) q & 0xFF) | (((ulong) (ax % d1) & 0xFF) << 8);
						break;
					}
				}
				return true;
			}
			// 16/32/64: rDX:rAX conventions. 128-bit via UInt128.
			var a2 = MaskW(Gpr[0], mw);
			var hi = MaskW(Gpr[2], mw);
			var sM = MaskW(src, mw);
			void WrAx(ulong v) => Gpr[0] = mw == 64 ? v : mw == 32 ? MaskW(v, 32) : (Gpr[0] & ~0xFFFFUL) | (v & 0xFFFF);
			void WrDx(ulong v) => Gpr[2] = mw == 64 ? v : mw == 32 ? MaskW(v, 32) : (Gpr[2] & ~0xFFFFUL) | (v & 0xFFFF);
			switch(baseName) {
				case "mul-wide": {
					var r = (UInt128) a2 * sM;
					WrAx((ulong) r); WrDx((ulong) (r >> mw));
					SetCfOf((ulong) (r >> mw) != 0);
					break;
				}
				case "imul-wide": {
					var r = (Int128) SignEx(a2, mw) * SignEx(sM, mw);
					WrAx((ulong) (UInt128) r); WrDx((ulong) ((UInt128) r >> mw));
					SetCfOf(r != SignEx((ulong) (UInt128) r, mw));
					break;
				}
				case "div-wide": {
					var num = ((UInt128) hi << mw) | a2;
					if(sM == 0) throw new DivideByZeroException("#DE");
					var q = num / sM;
					if(q > MaskW(ulong.MaxValue, mw)) throw new DivideByZeroException("#DE overflow");
					WrAx((ulong) q); WrDx((ulong) (num % sM));
					break;
				}
				case "idiv-wide": {
					var num = (Int128) (((UInt128) hi << mw) | a2);
					// sign: interpret the 2w-bit value
					num = (Int128) ((UInt128) num << (128 - 2 * mw)) >> (128 - 2 * mw);
					var dv = (Int128) SignEx(sM, mw);
					if(dv == 0) throw new DivideByZeroException("#DE");
					var q = num / dv;
					var lim = (Int128) 1 << (mw - 1);
					if(q >= lim || q < -lim) throw new DivideByZeroException("#DE overflow");
					WrAx((ulong) (UInt128) q & MaskW(ulong.MaxValue, mw)); WrDx((ulong) (UInt128) (num % dv) & MaskW(ulong.MaxValue, mw));
					break;
				}
			}
			return true;
		}

		// loop family: args[0] = pre-resolved absolute target
		if(baseName is "loop" or "loope" or "loopne" or "jcxz") {
			ulong CxA() => Mode == XMode.Bits16 ? Gpr[1] & 0xFFFF : Mode == XMode.Bits32 ? Gpr[1] & 0xFFFFFFFF : Gpr[1];
			if(baseName == "jcxz") {
				if(CxA() == 0) _branchTo = args[0];
				return true;
			}
			// dec confined to the address width (16-bit loop decs CX; upper bits untouched)
			if(Mode == XMode.Bits16) Gpr[1] = (Gpr[1] & ~0xFFFFUL) | ((Gpr[1] - 1) & 0xFFFF);
			else if(Mode == XMode.Bits32) Gpr[1] = (Gpr[1] - 1) & 0xFFFFFFFF;  // 32-bit dec zexts (reg-write rule)
			else Gpr[1]--;
			var zf = ((Flags >> 6) & 1) != 0;
			var take = CxA() != 0 && baseName switch { "loope" => zf, "loopne" => !zf, _ => true };
			if(take) _branchTo = args[0];
			return true;
		}
		if(baseName is not ("movs" or "stos" or "lods" or "scas" or "cmps")) return false;

		var w = args.Length > 0 ? (int) args[0] : 16;  // .isa convention: args[0] = width
		var step = (ulong) (w / 8);
		var down = ((Flags >> 10) & 1) != 0;  // DF

		ulong AddrMask(ulong a) => Mode == XMode.Bits16 ? a & 0xFFFF : Mode == XMode.Bits32 ? a & 0xFFFFFFFF : a;
		ulong Si() => SegBase[3] + AddrMask(Gpr[6]);
		ulong Di() => SegBase[0] + AddrMask(Gpr[7]);
		void Adv(int reg) => Gpr[reg] = down ? Gpr[reg] - step : Gpr[reg] + step;
		ulong Rd(ulong a) => Load(a, (int) step * 8);
		void Wr(ulong a, ulong v) => Store(a, v, (int) step * 8);

		while(true) {
			if(rep != RepKind.None) {
				var cx = Mode == XMode.Bits16 ? Gpr[1] & 0xFFFF : Mode == XMode.Bits32 ? Gpr[1] & 0xFFFFFFFF : Gpr[1];
				if(cx == 0) break;
			}
			switch(baseName) {
				case "movs": Wr(Di(), Rd(Si())); Adv(6); Adv(7); break;
				case "stos": Wr(Di(), MaskW(Gpr[0], w)); Adv(7); break;
				case "lods": Gpr[0] = w == 64 ? Rd(Si()) : (Gpr[0] & ~((1UL << w) - 1)) | Rd(Si()); Adv(6); break;
				case "scas": SubFlags(MaskW(Gpr[0], w), Rd(Di()), w); Adv(7); break;
				case "cmps": { var a = Rd(Si()); var b = Rd(Di()); SubFlags(a, b, w); Adv(6); Adv(7); break; }
			}
			if(rep == RepKind.None) break;
			Gpr[1]--;
			// repe/repne termination on ZF (scas/cmps only)
			if(rep == RepKind.RepE && ((Flags >> 6) & 1) == 0) break;
			if(rep == RepKind.RepNe && ((Flags >> 6) & 1) != 0) break;
		}
		return true;
	}

	enum RepKind { None, Rep, RepE, RepNe }

	/// CMP-shape flags (scas/cmps): CF/ZF/SF/OF/PF from a-b at width w.
	void SubFlags(ulong a, ulong b, int w) {
		var r = MaskW(a - b, w);
		void Set(int bit, bool v) => Flags = (Flags & ~(1UL << bit)) | ((v ? 1UL : 0) << bit);
		Set(0, MaskW(a, w) < MaskW(b, w));                       // CF borrow
		Set(6, r == 0);                                          // ZF
		Set(7, (r >> (w - 1) & 1) != 0);                         // SF
		var of = ((a ^ b) & (a ^ r)) >> (w - 1) & 1;             // OF (sub form)
		Set(11, of != 0);
		var p = (byte) r; p ^= (byte) (p >> 4);                  // PF (even parity, 0x9669)
		Set(2, ((0x9669 >> (p & 0xF)) & 1) != 0);
	}

	UInt128 Eval(Il e) {
		switch(e) {
			// A 128-BIT LITERAL SURVIVES AT 128. This was MaskTy(e.Ty, (ulong) bits) -- an
			// unconditional narrowing INSIDE the arm that produces the mask, and MaskTy itself
			// carries a sixth copy of the guess-a-width expression. IlConst.Bits has been
			// UInt128 the whole time; this arm threw the top half away before any width test
			// could see it.
			//
			// The scalar-preserve merge is IlBin(Vec(128), Or, kept, vlo) where `kept` is
			// IlBin(And, full, IlConst(v128, ~mask)) and ~mask = 0xFFFFFFFF_FFFFFFFF_FFFFFFFF_00000000.
			// Truncated to 64 that constant is ZERO, so `kept` was zero and the merge preserved
			// nothing -- 374,544 p2 rows, every scalar SSE write, invisible until the reader
			// started grading the high word. Five arms above this one were correct; this is
			// where the bits actually died.
			case IlConst(var cty, var bits): {
				var cw = WOf(e);
				return cw >= 128 ? bits : bits & ((UInt128.One << cw) - 1);
			}
			case IlReadReg(_, RegKind.X86, var i): return Gpr[i];
			case IlReadReg(_, RegKind.Eflags, var bit):
				return bit < 0 ? Flags : (Flags >> bit) & 1;
			case IlReadReg(_, RegKind.Xmm, var i): return Xmm[i];
			case IlReadReg(_, RegKind.St, var i): return St[i];
			case IlReadReg(_, RegKind.X86Seg, var i): return SegBase[i];
			case IlReadPc: return SegBase[1] + Ip;  // linear pc (RIP-rel math wants linear)
			case IlTmp(_, var id): return _tmps[id];
			case IlBin(var ty, var op, var l, var r): {
				var (a, b) = (Eval(l), Eval(r));
				// WIDTH VIA WOf, NOT AN INLINE GUESS. Fourth copy of one defect: IlLower.W(),
				// X86Machine.WOf(), the IlCast site, and here -- all answered 64 for a type they
				// could not measure, so every Vec(128) op computed at 64 and lost its high half.
				// The p2 scalar-preserve merge is emitted as IlBin(Vec(128), Or, kept, vlo), so
				// this one line zeroed the preserved upper bits of every scalar SSE write --
				// 374,544 rows once the reader started grading the high word at all.
				var w = WOf(new IlConst(ty, 0));
				// WIDTH > 64 IS COMPUTED, at UInt128 (de0c231). This comment used to say it
				// DIED LOUD because the carrier was ulong; the wide arm below is what
				// replaced that. The historical note is kept because the defect it
				// describes is the one to avoid: `w` was used as a MASK WIDTH, so an i128 op computed at 64
				// and truncated, silently.
				//
				// That is not academic. IMUL-Gv-Ev lowers to exactly this shape:
				//     (let %0 = (i128 mul (i128 sext a) (i128 sext b)))
				//     (let %2 = (u64 trunc (i128 shr (i128 %0) (u128 #40))))
				//     CF = OF = (%2 != sar(lo, 63))
				// With the product truncated to 64, the high half is always the same as the
				// sign-extension of the low half, so CF=OF read ZERO for every operand pair
				// -- including the ones where the multiply genuinely overflowed. XFReader
				// caught it at p1 row 580,040 (39,315 rows of the golden corpus): silicon
				// CF|OF = 1, ours = 0.
				//
				// The mul-wide arm at :226 has always had a real UInt128/Int128 path, which
				// is why MUL/IMUL's rDX:rAX forms are correct -- so this is the one-operand
				// GENERIC path, reached by the two-operand IMUL that keeps only the low half
				// and reports overflow in the flags. Dying loud makes the reader report it
				// as `unlowered` (a stated gap) instead of `ok` (a false green), which is the
				// difference between a known hole and a wrong answer.
				//
				// ‡ The fix is a UInt128 carrier for Eval (DIFFERENTIAL-SCOPING step 2, the
				// same widening the XMM lanes need); until then this is honest rather than
				// correct, and the 39,315 rows are a NAMED gap.
				// CLOSED 2026-08-21: the carrier IS UInt128 now, so the wide arms compute
				// for real instead of throwing. IMUL-Gv-Ev's CF=OF is the acceptance case.
				if(w > 64) {
					UInt128 wa = a, wb = b;
					var sw = 128;   // the only width above 64 the .isa emits
					UInt128 SxW(UInt128 v, int fw) {
						if(fw >= sw) return v;
						var sb = UInt128.One << (fw - 1);
						return (v & sb) != 0 ? v | ~((UInt128.One << fw) - 1) : v & ((UInt128.One << fw) - 1);
					}
					UInt128 la = SxW(wa, WOf(l)), lb = SxW(wb, WOf(r));
					return op switch {
						BinOp.Add => wa + wb, BinOp.Sub => wa - wb, BinOp.Mul => wa * wb,
						BinOp.And => wa & wb, BinOp.Or => wa | wb, BinOp.Xor => wa ^ wb,
						BinOp.Shl => wb >= (UInt128) sw ? UInt128.Zero : wa << (int) wb,
						BinOp.Shr => wb >= (UInt128) sw ? UInt128.Zero : wa >> (int) wb,
						BinOp.Sar => wb >= (UInt128) sw
							? ((Int128) la < 0 ? ~UInt128.Zero : UInt128.Zero)
							: (UInt128) ((Int128) la >> (int) wb),
						BinOp.UDiv => wb == 0 ? UInt128.Zero : wa / wb,
						BinOp.URem => wb == 0 ? UInt128.Zero : wa % wb,
						BinOp.SDiv => lb == 0 ? UInt128.Zero : (UInt128) ((Int128) la / (Int128) lb),
						BinOp.SRem => lb == 0 ? UInt128.Zero : (UInt128) ((Int128) la % (Int128) lb),
						BinOp.Eq => wa == wb ? UInt128.One : UInt128.Zero,  BinOp.Ne => wa != wb ? UInt128.One : UInt128.Zero,
						BinOp.Ult => wa < wb ? UInt128.One : UInt128.Zero,  BinOp.Ule => wa <= wb ? UInt128.One : UInt128.Zero,
						BinOp.Ugt => wa > wb ? UInt128.One : UInt128.Zero,  BinOp.Uge => wa >= wb ? UInt128.One : UInt128.Zero,
						BinOp.Slt => (Int128) la <  (Int128) lb ? UInt128.One : UInt128.Zero,
						BinOp.Sle => (Int128) la <= (Int128) lb ? UInt128.One : UInt128.Zero,
						BinOp.Sgt => (Int128) la >  (Int128) lb ? UInt128.One : UInt128.Zero,
						BinOp.Sge => (Int128) la >= (Int128) lb ? UInt128.One : UInt128.Zero,
						_ => throw new NotSupportedException(
							$"IlBin {op} at width {w}: no wide arm. The carrier holds it; this op " +
							$"has no 128-bit implementation here. NOT a carrier limit -- a missing arm."),
					};
				}
				var (a64, b64) = (N64(a), N64(b));
				// FLOAT ARMS. Dispatch on the OPERAND type, not the result type: a64
				// float compare has an integer (u1) result but float inputs, which
				// is exactly the convention MaxwellEval:245-247 uses (BinOp.Slt =>
				// fa < fb when the operands are F-typed). So `flt`/`feq` lowering to
				// Slt/Eq is right by that contract rather than by coincidence.
				if(l.Ty is IlType.F lf) {
					if(lf.Bits == 32) {
						var (fa, fb) = (BitConverter.UInt32BitsToSingle((uint) a64),
						                BitConverter.UInt32BitsToSingle((uint) b64));
						// x86 MIN/MAX return the SECOND source on NaN or when both are
						// zero -- NOT ARM's FMAX/FMIN, which propagate the NaN. MathF.Max
						// propagates NaN too, so it can't be used here; the silicon sweep
						// verified this shape on the Rust side (bd.fminmax via FCMP+FCSEL).
						return op switch {
							BinOp.Add => BitConverter.SingleToUInt32Bits(fa + fb),
							BinOp.Sub => BitConverter.SingleToUInt32Bits(fa - fb),
							BinOp.Mul => BitConverter.SingleToUInt32Bits(fa * fb),
							BinOp.UDiv or BinOp.SDiv => BitConverter.SingleToUInt32Bits(fa / fb),
							BinOp.FMax => BitConverter.SingleToUInt32Bits(fa > fb ? fa : fb),
							BinOp.FMin => BitConverter.SingleToUInt32Bits(fa < fb ? fa : fb),
							BinOp.Eq => fa == fb ? 1UL : 0, BinOp.Ne => fa != fb ? 1UL : 0,
							BinOp.Slt or BinOp.Ult => fa < fb ? 1UL : 0,
							BinOp.Sle or BinOp.Ule => fa <= fb ? 1UL : 0,
							BinOp.Sgt or BinOp.Ugt => fa > fb ? 1UL : 0,
							BinOp.Sge or BinOp.Uge => fa >= fb ? 1UL : 0,
							_ => throw new NotSupportedException($"f32 binop {op}")
						};
					} else {
						var (fa, fb) = (BitConverter.UInt64BitsToDouble(a64),
						                BitConverter.UInt64BitsToDouble(b64));
						return op switch {
							BinOp.Add => BitConverter.DoubleToUInt64Bits(fa + fb),
							BinOp.Sub => BitConverter.DoubleToUInt64Bits(fa - fb),
							BinOp.Mul => BitConverter.DoubleToUInt64Bits(fa * fb),
							BinOp.UDiv or BinOp.SDiv => BitConverter.DoubleToUInt64Bits(fa / fb),
							BinOp.FMax => BitConverter.DoubleToUInt64Bits(fa > fb ? fa : fb),
							BinOp.FMin => BitConverter.DoubleToUInt64Bits(fa < fb ? fa : fb),
							BinOp.Eq => fa == fb ? 1UL : 0, BinOp.Ne => fa != fb ? 1UL : 0,
							BinOp.Slt or BinOp.Ult => fa < fb ? 1UL : 0,
							BinOp.Sle or BinOp.Ule => fa <= fb ? 1UL : 0,
							BinOp.Sgt or BinOp.Ugt => fa > fb ? 1UL : 0,
							BinOp.Sge or BinOp.Uge => fa >= fb ? 1UL : 0,
							_ => throw new NotSupportedException($"f64 binop {op}")
						};
					}
				}
				var v = op switch {
					BinOp.Add => a64 + b64, BinOp.Sub => a64 - b64, BinOp.Mul => a64 * b64,
					BinOp.UDiv => b64 == 0 ? throw new DivideByZeroException() : a64 / b64,
					BinOp.SDiv => (ulong) ((long) SignEx(a64, WOf(l)) / (long) SignEx(b64, WOf(r))),
					BinOp.URem => a64 % b64,
					BinOp.SRem => (ulong) ((long) SignEx(a64, WOf(l)) % (long) SignEx(b64, WOf(r))),
					BinOp.And => a64 & b64, BinOp.Or => a64 | b64, BinOp.Xor => a64 ^ b64,
					BinOp.Shl => b64 >= 64 ? 0 : a64 << (int) b64,
					BinOp.Shr => b64 >= 64 ? 0 : MaskW(a64, WOf(l)) >> (int) b64,
					BinOp.Sar => (ulong) (SignEx(a64, WOf(l)) >> (int) Math.Min(b64, 63)),
					BinOp.Ror => Ror(MaskW(a64, WOf(l)), (int) b64, WOf(l)),
					BinOp.Eq => MaskW(a64, WOf(l)) == MaskW(b64, WOf(l)) ? 1UL : 0,
					BinOp.Ne => MaskW(a64, WOf(l)) != MaskW(b64, WOf(l)) ? 1UL : 0,
					BinOp.Ult => MaskW(a64, WOf(l)) < MaskW(b64, WOf(l)) ? 1UL : 0,
					BinOp.Ule => MaskW(a64, WOf(l)) <= MaskW(b64, WOf(l)) ? 1UL : 0,
					BinOp.Ugt => MaskW(a64, WOf(l)) > MaskW(b64, WOf(l)) ? 1UL : 0,
					BinOp.Uge => MaskW(a64, WOf(l)) >= MaskW(b64, WOf(l)) ? 1UL : 0,
					BinOp.Slt => SignEx(a64, WOf(l)) < SignEx(b64, WOf(r)) ? 1UL : 0,
					BinOp.Sle => SignEx(a64, WOf(l)) <= SignEx(b64, WOf(r)) ? 1UL : 0,
					BinOp.Sgt => SignEx(a64, WOf(l)) > SignEx(b64, WOf(r)) ? 1UL : 0,
					BinOp.Sge => SignEx(a64, WOf(l)) >= SignEx(b64, WOf(r)) ? 1UL : 0,
					_ => throw new NotSupportedException($"binop {op}")
				};
				return MaskW(v, w);
			}
			case IlUn(var ty, var op, var x): {
				// WIDE ARM FIRST, because the narrowing below happens BEFORE any width test.
				// `var a = N64(Eval(x))` threw the top half away unconditionally, so a
				// Vec(128) Not computed ~ at 64 bits and the upper half came back zero.
				//
				// ANDNPS/ANDNPD/PANDN are (& (~ dst) src) -- the whole family routes through
				// here, 16,560 p2 rows, and they were the entire residual after the IlConst
				// fix. Only Neg and Not are meaningful at 128 (a packed float abs/sqrt is
				// per-lane and belongs to the IlVec* nodes, which nothing emits yet), so the
				// arm is deliberately narrow and everything else still narrows and dies loud.
				var uw = WOf(new IlConst(ty, 0));
				if(uw > 64) {
					UInt128 wx = Eval(x);
					return op switch {
						UnOp.Not => ~wx,
						UnOp.Neg => UInt128.Zero - wx,
						_ => throw new NotSupportedException($"IlUn {op} at width {uw}"),
					};
				}
				var a = N64(Eval(x));
				// WIDTH VIA WOf, NOT AN INLINE GUESS. Fourth copy of one defect: IlLower.W(),
				// X86Machine.WOf(), the IlCast site, and here -- all answered 64 for a type they
				// could not measure, so every Vec(128) op computed at 64 and lost its high half.
				// The p2 scalar-preserve merge is emitted as IlBin(Vec(128), Or, kept, vlo), so
				// this one line zeroed the preserved upper bits of every scalar SSE write --
				// 374,544 rows once the reader started grading the high word at all.
				var w = WOf(new IlConst(ty, 0));
				var v = op switch {
					UnOp.Neg => 0 - a,
					UnOp.Not => ~a,
					UnOp.Popcnt => (ulong) System.Numerics.BitOperations.PopCount(MaskW(a, WOf(x))),
					// float unops -- carrier is the bit pattern (see the Cast arm's note)
					UnOp.Abs when x.Ty is IlType.F fa2 => fa2.Bits == 32
						? BitConverter.SingleToUInt32Bits(MathF.Abs(BitConverter.UInt32BitsToSingle((uint) a)))
						: BitConverter.DoubleToUInt64Bits(Math.Abs(BitConverter.UInt64BitsToDouble(a))),
					UnOp.Sqrt when x.Ty is IlType.F fs => fs.Bits == 32
						? BitConverter.SingleToUInt32Bits(MathF.Sqrt(BitConverter.UInt32BitsToSingle((uint) a)))
						: BitConverter.DoubleToUInt64Bits(Math.Sqrt(BitConverter.UInt64BitsToDouble(a))),
					// BSF/BSR/LZCNT/TZCNT lower to clz / clz(rbit(x)) — the .isa says so
					// in its own comment ("BSF = position of LOWEST set bit =
					// clz(rbit(src)); aarch64 has no ctz"). The lowerer emits IlUn and
					// this evaluator only knew Popcnt, so those four were unexecutable.
					// Width-relative by necessity: LeadingZeroCount is defined on 64-bit,
					// so a w-bit clz is the 64-bit count minus the (64-w) padding zeros.
					UnOp.Clz => (ulong) (System.Numerics.BitOperations.LeadingZeroCount(MaskW(a, WOf(x)))
						- (64 - WOf(x))),
					// ReverseBits already exists in LibSharpRetro.CpuHelpers.Math (:127/:134)
					// — reversing at 64 then shifting down is the w-bit reversal, matching
					// the Rust interp's `bits.reverse_bits() >> (128 - w)` form exactly.
					UnOp.Rbit => LibSharpRetro.CpuHelpers.Math.ReverseBits(MaskW(a, WOf(x)))
						>> (64 - WOf(x)),
					_ => throw new NotSupportedException($"unop {op}")
				};
				return MaskW(v, w);
			}
			case IlCast(var ty, var kind, var x): {
				var aw = Eval(x); var a = N64(aw);
				// WIDTH FROM WOf, NOT FROM AN INLINE `as IlType.I ?? 64`. Third instance of one
				// defect tonight: IlLower.W(), X86Machine.WOf(), and this inline copy of the same
				// expression all answered 64 for a type they could not measure.
				//
				// PSRLDQ is what caught it. The lowering emits a THREE-node chain and only the
				// last one has a Vec target:
				//   IlCast(I(128), Bitcast, <Vec(128) read>)   -> wide arm fires, passes through
				//   IlBin (I(128), Shr, .., 8)                 -> wide arm fires, shifts at 128
				//   IlCast(Vec(128), Bitcast, ..)              -> w=64 HERE, so the wide arm did
				//                                                 NOT fire and the result was
				//                                                 masked to 64 bits
				// got = want >> 8 on 304 rows of the p2 golden -- the high half fell off at the
				// LAST node, after two arms had computed it correctly.
				//
				// The lesson is the placement, not the expression: I fixed the two NAMED helpers
				// and left an unnamed copy of the same logic inline, where no grep for the
				// helper's name would ever find it.
				// VEC ONLY, and the narrowing is MEASURED not reasoned: swapping this for the
				// full WOf() (which also measures F) took the p2 diff count from 304 to 800.
				// The float arms below read `w` too, and they were correct at the old 64 -- an
				// F(32) target is a 32-bit BIT PATTERN in a 64-bit carrier slot, not a 32-bit
				// value to mask. So the fix is the Vec case alone; F keeps 64.
				// One variable, one A/B, and the wider change looked more principled.
				var w = ty switch { IlType.I(_, var tb) => tb, IlType.Vec(var vb) => vb, _ => 64 };
				// FLOAT CASTS. An F-typed value rides in the carrier (now UInt128) as
				// its IEEE BIT PATTERN — the same convention MaxwellEval uses
				// (MaxwellEval.cs:119, UInt32BitsToSingle on an F-typed read). A
				// Bitcast is therefore a no-op on the carrier, which is why the
				// (as-f32)/(as-f64) heads need no arm here and only the CONVERTING
				// kinds do.
				if(ty is IlType.F ft) {
					switch(kind) {
						case CastKind.SToF:   // int -> float (CVTSI2SD/SS, and (f32 x))
							return ft.Bits == 32
								? BitConverter.SingleToUInt32Bits((float) (long) SignEx(a, WOf(x)))
								: BitConverter.DoubleToUInt64Bits((double) (long) SignEx(a, WOf(x)));
						case CastKind.UToF:
							return ft.Bits == 32
								? BitConverter.SingleToUInt32Bits((float) a)
								: BitConverter.DoubleToUInt64Bits((double) a);
						case CastKind.FTrunc:   // f64 -> f32 (CVTSD2SS)
							return BitConverter.SingleToUInt32Bits(
								(float) BitConverter.UInt64BitsToDouble(a));
						case CastKind.FExt:     // f32 -> f64 (CVTSS2SD)
							return BitConverter.DoubleToUInt64Bits(
								(double) BitConverter.UInt32BitsToSingle((uint) a));
						case CastKind.Bitcast:  // reinterpret: carrier unchanged
							return ft.Bits == 32 ? a & 0xFFFFFFFF : a;
					}
				}
				// float -> int. The .isa's (int-of) head wraps this in the
				// x86 indefinite-integer guard, so this arm only sees in-range
				// values; a bare FToSI on NaN would be the caller's bug, not ours.
				if(kind is CastKind.FToSI or CastKind.FToI or CastKind.FToUI) {
					// same trap as IlLower's: WOf() falls back to 64 on a non-I type,
					// so the F width must be read off the F type.
					var fw = x.Ty is IlType.F fx ? fx.Bits : WOf(x);
					var d = fw == 32 ? BitConverter.UInt32BitsToSingle((uint) a)
					                 : BitConverter.UInt64BitsToDouble(a);
					return kind == CastKind.FToUI ? MaskW((ulong) d, w)
					                              : MaskW((ulong) (long) d, w);
				}
				// WIDE TARGETS use the un-narrowed `aw`. Before the carrier widened, every
				// arm here went through MaskW(a, w) with `a` already truncated to 64, so an
				// (i128 sext a) produced a 64-bit value whose high half was zero -- and
				// IMUL's CF=OF, which compares the product's high half against the sign of
				// its low half, read that as "no overflow" for every operand pair.
				//
				// The inverse is just as wrong and is what I hit first: computing the
				// product at 128 while STILL sign-extending through a 64-bit path gives a
				// high half that disagrees the OTHER way, so CF|OF read SET for every pair
				// (28,785 rows of the golden corpus, got=801 want=0). One truncation, two
				// opposite wrong answers -- which is why the acceptance case is a corpus
				// and not a hand-written expectation.
				if(w > 64) {
					var sw = WOf(x);
					return kind switch {
						CastKind.Zext or CastKind.Trunc or CastKind.Bitcast =>
							sw >= 128 ? aw : aw & ((UInt128.One << sw) - 1),
						CastKind.Sext => SignEx128(aw, sw),
						_ => throw new NotSupportedException(
							$"IlCast {kind} to width {w}: no wide arm."),
					};
				}
				return kind switch {
					CastKind.Zext or CastKind.Trunc or CastKind.Bitcast => MaskW(a, w),
					CastKind.Sext => MaskW((ulong) SignEx(a, WOf(x)), w),
					_ => throw new NotSupportedException($"cast {kind}")
				};
			}
			case IlLoad(var ty, var addr):
				return Load(N64(Eval(addr)), WOf(new IlConst(ty, 0)));
			case IlIfV(_, var c, var t, var f):
				return (Eval(c) & 1) != 0 ? Eval(t) : Eval(f);
			// PACKED-VECTOR NODES. IlLower emits four kinds at 16 sites, all at 128 bits with
			// a COMPILE-TIME lane type, so lane count = 128/ElemTy.Bits and every index is
			// static. That bound is what makes this arm small: no dynamic shuffles, no
			// variable lane widths.
			//
			// Until now these threw at the default and XFReader counted them as `unlowered`:
			// 651,633 p2 rows, i.e. the lowering was verified as STRUCTURE by the def-set gate
			// and unverified as SEMANTICS by anything. The lane convention is transcribed from
			// the emit sites (IlLower.cs:766/845/895/1019) rather than composed -- an IlVecBin
			// over I(true,ew) is signed per-lane, over F(ew) is float per-lane, and a mask
			// result is ALL-ONES per lane (sse2.isa:158 declares it, interp.rs:484 implements
			// it), NOT 1.
			case IlVecBuild(var vbits, var bet, var els): {
				var lw = bet is IlType.F bf ? bf.Bits : ((IlType.I) bet).Bits;
				UInt128 acc = 0;
				for(var li = 0; li < els.Count; li++) {
					var lv = Eval(els[li]) & LaneMask(lw);
					acc |= lv << (li * lw);
				}
				return acc;
			}
			case IlVecElem(var eet, var vv, var vi): {
				var lw = eet is IlType.F ef ? ef.Bits : ((IlType.I) eet).Bits;
				var idx = (int) N64(Eval(vi));
				return (Eval(vv) >> (idx * lw)) & LaneMask(lw);
			}
			case IlVecBin(var vbits2, var bet2, var bop, var bl, var br): {
				var lw = bet2 is IlType.F bf2 ? bf2.Bits : ((IlType.I) bet2).Bits;
				var (av, bv) = (Eval(bl), Eval(br));
				var n = vbits2 / lw;
				// THE SHIFT COUNT IS A SCALAR, NOT A VECTOR. IlLower emits the packed shifts as
				//   IlVecBin(128, I(signed,ew), Shl|Shr|Sar, vec, IlConst(I(false,32), cnt))
				// (IlLower.cs:895) -- the RHS is one count broadcast to every lane, and its own
				// type is I(false,32) rather than the vector's element type. Slicing it per-lane
				// like a vector operand gives cnt in lane 0 and ZERO everywhere else, so a
				// PSRAW-by-1 returned its input almost unchanged: 8,096 p2 rows, entirely the
				// shift-by-immediate family, and got==input is exactly what a zero shift looks
				// like. The RHS's OWN width is the discriminator and it is in the node.
				var rhsIsScalar = bop is BinOp.Shl or BinOp.Shr or BinOp.Sar;
				UInt128 acc = 0;
				for(var li = 0; li < n; li++) {
					var la = (av >> (li * lw)) & LaneMask(lw);
					var lb = rhsIsScalar ? bv : (bv >> (li * lw)) & LaneMask(lw);
					acc |= (LaneBin(bop, la, lb, bet2, lw) & LaneMask(lw)) << (li * lw);
				}
				return acc;
			}
			case IlVecUn(var vbits3, var bet3, var uop, var ux): {
				var lw = bet3 is IlType.F uf ? uf.Bits : ((IlType.I) bet3).Bits;
				var xv = Eval(ux);
				var n = vbits3 / lw;
				UInt128 acc = 0;
				for(var li = 0; li < n; li++) {
					var lx = (xv >> (li * lw)) & LaneMask(lw);
					acc |= (LaneUn(uop, lx, bet3, lw) & LaneMask(lw)) << (li * lw);
				}
				return acc;
			}
			default: throw new NotSupportedException($"eval {e.GetType().Name}");
		}
	}

	// MEASURE, don't guess -- the exact sibling of IlLower.W(), and the same defect for
	// the same reason: this answered 64 for any non-I type, so a Vec-typed operand read as
	// 64 bits wide. PSRLDQ is the acceptance case. IlLower emits
	//     IlCast(I(128), Bitcast, <a V128-typed IlReadReg>)
	// and my wide Bitcast arm masks by WOf(x) -- which returned 64 for the Vec, so a
	// 128-bit byte-shift got its source truncated to the low 8 bytes and the result came
	// back shifted one byte too far (got = want >> 8, 304 rows of the p2 golden).
	//
	// That path was UNLOWERED before the carrier widened, so this bug is not new -- it was
	// unreachable. Widening a carrier makes previously-dead arms live, and a helper that
	// guesses a width is exactly what greets them. Fixed at BOTH helpers now; F and Vec
	// both know their width and there was never a reason to answer for them.
	// PER-LANE HELPERS for the IlVec* arms. Kept separate so a reader can see the lane
	// convention in one place: a lane is `lw` bits wide, masked on the way in and on the
	// way out, and the ELEM TYPE decides signedness/floatness -- never the op alone.
	static UInt128 LaneMask(int lw) => lw >= 128 ? ~UInt128.Zero : (UInt128.One << lw) - 1;

	// A COMPARE RESULT IS ALL-ONES PER LANE, NOT 1. sse2.isa:158 declares the convention
	// and interp.rs:484 implements it; PAND-after-PCMPEQ depends on it, so a boolean 1
	// would be silently wrong on every mask-then-blend idiom.
	static UInt128 LaneBin(BinOp op, UInt128 a, UInt128 b, IlType et, int lw) {
		UInt128 T = LaneMask(lw), F = 0;
		if(et is IlType.F fe) {
			double x = fe.Bits == 32 ? BitConverter.UInt32BitsToSingle((uint) a) : BitConverter.UInt64BitsToDouble((ulong) a);
			double y = fe.Bits == 32 ? BitConverter.UInt32BitsToSingle((uint) b) : BitConverter.UInt64BitsToDouble((ulong) b);
			double r;
			switch(op) {
				case BinOp.Add: r = x + y; break;
				case BinOp.Sub: r = x - y; break;
				case BinOp.Mul: r = x * y; break;
				case BinOp.UDiv: case BinOp.SDiv: r = x / y; break;
				// x86-EXACT, NOT ARM's FMAX: on a NaN operand or +-0 the SECOND source wins.
				// Transcribed from this file's own scalar arm (:466/:483, `fa > fb ? fa : fb`)
				// rather than composed -- the C# ternary gives exactly that, because any
				// comparison with NaN is false so the false-branch (y) is taken. MAXPS/MINPS/
				// MAXPD/MINPD, 25,792 p2 rows.
				case BinOp.FMax: r = x > y ? x : y; break;
				case BinOp.FMin: r = x < y ? x : y; break;
				// Float COMPARES return a lane MASK, not a float.
				case BinOp.Eq: return x == y ? T : F;
				case BinOp.Ne: return x != y ? T : F;
				case BinOp.Slt: case BinOp.Ult: return x < y ? T : F;
				case BinOp.Sle: case BinOp.Ule: return x <= y ? T : F;
				case BinOp.Sgt: case BinOp.Ugt: return x > y ? T : F;
				case BinOp.Sge: case BinOp.Uge: return x >= y ? T : F;
				// Bitwise ops on an F-typed vector act on the BIT PATTERN (ANDPS/ORPS/XORPS).
				case BinOp.And: return a & b;
				case BinOp.Or: return a | b;
				case BinOp.Xor: return a ^ b;
				default: throw new NotSupportedException($"vec-f lane op {op}");
			}
			return fe.Bits == 32 ? BitConverter.SingleToUInt32Bits((float) r) : BitConverter.DoubleToUInt64Bits(r);
		}
		var ie = (IlType.I) et;
		// SIGNED lanes sign-extend to Int128 before the op; unsigned stay as-is.
		Int128 sa = SxLane(a, lw), sb = SxLane(b, lw);
		return op switch {
			BinOp.Add => a + b,
			BinOp.Sub => a - b,
			BinOp.Mul => ie.Signed ? (UInt128) (sa * sb) : a * b,
			BinOp.And => a & b,
			BinOp.Or => a | b,
			BinOp.Xor => a ^ b,
			BinOp.Eq => a == b ? T : F,
			BinOp.Ne => a != b ? T : F,
			BinOp.Slt => sa < sb ? T : F,
			BinOp.Sle => sa <= sb ? T : F,
			BinOp.Sgt => sa > sb ? T : F,
			BinOp.Sge => sa >= sb ? T : F,
			BinOp.Ult => a < b ? T : F,
			BinOp.Ule => a <= b ? T : F,
			BinOp.Ugt => a > b ? T : F,
			BinOp.Uge => a >= b ? T : F,
			// A SHIFT COUNT >= LANE WIDTH GIVES ZERO (SDM: the packed-shift forms saturate
			// to 0 rather than masking the count, unlike the scalar forms).
			BinOp.Shl => b >= (UInt128) lw ? 0 : a << (int) b,
			BinOp.Shr => b >= (UInt128) lw ? 0 : a >> (int) b,
			BinOp.Sar => b >= (UInt128) lw
				? (sa < 0 ? T : F)
				: (UInt128) (sa >> (int) b),
			_ => throw new NotSupportedException($"vec-i lane op {op}"),
		};
	}

	static Int128 SxLane(UInt128 v, int lw) {
		if(lw >= 128) return (Int128) v;
		var sb = UInt128.One << (lw - 1);
		return (Int128) ((v & sb) != 0 ? v | ~LaneMask(lw) : v & LaneMask(lw));
	}

	static UInt128 LaneUn(UnOp op, UInt128 a, IlType et, int lw) {
		if(et is IlType.F fe) {
			double x = fe.Bits == 32 ? BitConverter.UInt32BitsToSingle((uint) a) : BitConverter.UInt64BitsToDouble((ulong) a);
			// NOT ON AN F-TYPED LANE IS BITWISE, and it is the ONLY reason CMPPS/CMPPD were
			// still unlowered after every other vector arm landed. FloatPred's Invert() emits
			//     IlVecUn(128, F(ew), UnOp.Not, <a mask>)
			// (IlLower.cs:119) for predicates 4-7 (NEQ/NLT/NLE/ORD) -- the operand is already
			// an all-ones-or-zero MASK, not a float, so the node's F element type describes the
			// COMPARE's operands rather than what Not acts on. Routing it through the float
			// switch would reinterpret a mask as an IEEE bit pattern and negate it.
			// 64,480 p2 rows, the last two templates.
			if(op == UnOp.Not) return ~a;
			double r = op switch {
				UnOp.Sqrt => Math.Sqrt(x),
				UnOp.Abs => Math.Abs(x),
				UnOp.Neg => -x,
				_ => throw new NotSupportedException($"vec-f lane un {op}"),
			};
			return fe.Bits == 32 ? BitConverter.SingleToUInt32Bits((float) r) : BitConverter.DoubleToUInt64Bits(r);
		}
		return op switch {
			UnOp.Not => ~a,
			UnOp.Neg => UInt128.Zero - a,
			_ => throw new NotSupportedException($"vec-i lane un {op}"),
		};
	}

	static int WOf(Il e) => e.Ty switch {
		IlType.I(_, var b) => b,
		IlType.F(var fb) => fb,
		IlType.Vec(var vb) => vb,
		_ => 64,
	};
	static ulong MaskTy(IlType t, ulong v) => MaskW(v, (t as IlType.I)?.Bits ?? 64);
	// NARROW BY CONSTRUCTION. Eval returns UInt128 since the carrier widened; these are
	// the sites where the consumer is a 64-bit-or-less thing BY THE IL'S OWN TYPING -- a
	// GPR, a flag bit, an address, a branch target, a segment selector. Kept as a named
	// helper rather than a scatter of (ulong) casts so `grep N64` enumerates every place
	// this evaluator throws bits away, which is the question a reader of a widened carrier
	// actually has.
	static ulong N64(UInt128 v) => (ulong) v;

	// Sign-extend a value of `fw` bits to the full 128-bit carrier. The 64-bit sibling
	// (SignEx) returns a ulong and therefore cannot express this -- which is the exact
	// site where IMUL's high half was being lost.
	static UInt128 SignEx128(UInt128 v, int fw) {
		if(fw >= 128) return v;
		var m = (UInt128.One << fw) - 1;
		var sb = UInt128.One << (fw - 1);
		v &= m;
		return (v & sb) != 0 ? v | ~m : v;
	}

	static ulong MaskW(ulong v, int w) => w >= 64 ? v : v & ((1UL << w) - 1);
	static long SignEx(ulong v, int w) => w >= 64 ? (long) v : ((long) (v << (64 - w))) >> (64 - w);
	static ulong Ror(ulong v, int n, int w) { n %= w; return n == 0 ? v : MaskW((v >> n) | (v << (w - n)), w); }
}
