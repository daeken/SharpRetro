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
	/// Instruction-fetch hook — DISTINCT from LoadHook so a host can
	/// exec-filter (barrow's X86Env step-1.5(c) ‡: LoadHook serves both
	/// fetch and data, so his Segment.Exec check can't discriminate).
	/// Fill up to 15 bytes at addr; return false if addr isn't executable
	/// (Step() returns false = clean [not-exec] fault). Null → Step falls
	/// through to Mem[]/Load() (current behavior — behaviorally transparent).
	public delegate bool FetchDelegate(ulong addr, Span<byte> into);
	public FetchDelegate FetchHook;
	/// Overlay-miss fallback: barrow's X86Env sets this to read Image bytes
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

	readonly Dictionary<int, ulong> _tmps = [];

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
			case IlWriteReg(RegKind.X86, var i, var v): Gpr[i] = Eval(v); break;
			case IlWriteReg(RegKind.Eflags, var bit, var v):
				if(bit < 0) Flags = Eval(v) | 2;
				else Flags = (Flags & ~(1UL << bit)) | ((Eval(v) & 1) << bit);
				break;
			case IlWriteReg(RegKind.X86Seg, var i, var v): {
				var sel = (ushort) Eval(v);
				SegSel[i] = sel;
				if(Mode == XMode.Bits16) SegBase[i] = (ulong) sel << 4;  // real mode
				break;
			}
			case IlStore(var addr, var v):
				Store(Eval(addr), Eval(v), (v.Ty as IlType.I)?.Bits ?? 64);
				break;
			case IlBranch(var kind, var target, var cond):
				if(cond == null || (Eval(cond) & 1) != 0)
					_branchTo = Eval(target);
				break;
			case IlIf(var c, var then, var els):
				ExecBlock((Eval(c) & 1) != 0 ? then : els);
				break;
			case IlIntrin(_, var name, var args): {
				var vals = args.Select(Eval).ToArray();
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
		var handled = StringOp(name, args);
		branchTo = _branchTo;
		_branchTo = save ?? _branchTo;  // preserve any prior branch; loop's branch wins if set
		if(handled) return;
		if(OnIntrin == null || !OnIntrin(this, name, args))
			throw new NotSupportedException($"unhandled intrinsic {name}");
	}
	// Overload for the exec path (branchTo → _branchTo directly).
	void RunIntrinsic(string name, ulong[] args) {
		if(StringOp(name, args)) return;
		if(OnIntrin == null || !OnIntrin(this, name, args))
			throw new NotSupportedException($"unhandled intrinsic {name}");
	}

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

	ulong Eval(Il e) {
		switch(e) {
			case IlConst(_, var bits): return MaskTy(e.Ty, (ulong) bits);
			case IlReadReg(_, RegKind.X86, var i): return Gpr[i];
			case IlReadReg(_, RegKind.Eflags, var bit):
				return bit < 0 ? Flags : (Flags >> bit) & 1;
			case IlReadReg(_, RegKind.X86Seg, var i): return SegBase[i];
			case IlReadPc: return SegBase[1] + Ip;  // linear pc (RIP-rel math wants linear)
			case IlTmp(_, var id): return _tmps[id];
			case IlBin(var ty, var op, var l, var r): {
				var (a, b) = (Eval(l), Eval(r));
				var w = (ty as IlType.I)?.Bits ?? 64;
				var v = op switch {
					BinOp.Add => a + b, BinOp.Sub => a - b, BinOp.Mul => a * b,
					BinOp.UDiv => b == 0 ? throw new DivideByZeroException() : a / b,
					BinOp.SDiv => (ulong) ((long) SignEx(a, WOf(l)) / (long) SignEx(b, WOf(r))),
					BinOp.URem => a % b,
					BinOp.SRem => (ulong) ((long) SignEx(a, WOf(l)) % (long) SignEx(b, WOf(r))),
					BinOp.And => a & b, BinOp.Or => a | b, BinOp.Xor => a ^ b,
					BinOp.Shl => b >= 64 ? 0 : a << (int) b,
					BinOp.Shr => b >= 64 ? 0 : MaskW(a, WOf(l)) >> (int) b,
					BinOp.Sar => (ulong) (SignEx(a, WOf(l)) >> (int) Math.Min(b, 63)),
					BinOp.Ror => Ror(MaskW(a, WOf(l)), (int) b, WOf(l)),
					BinOp.Eq => MaskW(a, WOf(l)) == MaskW(b, WOf(l)) ? 1UL : 0,
					BinOp.Ne => MaskW(a, WOf(l)) != MaskW(b, WOf(l)) ? 1UL : 0,
					BinOp.Ult => MaskW(a, WOf(l)) < MaskW(b, WOf(l)) ? 1UL : 0,
					BinOp.Ule => MaskW(a, WOf(l)) <= MaskW(b, WOf(l)) ? 1UL : 0,
					BinOp.Ugt => MaskW(a, WOf(l)) > MaskW(b, WOf(l)) ? 1UL : 0,
					BinOp.Uge => MaskW(a, WOf(l)) >= MaskW(b, WOf(l)) ? 1UL : 0,
					BinOp.Slt => SignEx(a, WOf(l)) < SignEx(b, WOf(r)) ? 1UL : 0,
					BinOp.Sle => SignEx(a, WOf(l)) <= SignEx(b, WOf(r)) ? 1UL : 0,
					BinOp.Sgt => SignEx(a, WOf(l)) > SignEx(b, WOf(r)) ? 1UL : 0,
					BinOp.Sge => SignEx(a, WOf(l)) >= SignEx(b, WOf(r)) ? 1UL : 0,
					_ => throw new NotSupportedException($"binop {op}")
				};
				return MaskW(v, w);
			}
			case IlUn(var ty, var op, var x): {
				var a = Eval(x);
				var w = (ty as IlType.I)?.Bits ?? 64;
				var v = op switch {
					UnOp.Neg => 0 - a,
					UnOp.Not => ~a,
					UnOp.Popcnt => (ulong) System.Numerics.BitOperations.PopCount(MaskW(a, WOf(x))),
					_ => throw new NotSupportedException($"unop {op}")
				};
				return MaskW(v, w);
			}
			case IlCast(var ty, var kind, var x): {
				var a = Eval(x);
				var w = (ty as IlType.I)?.Bits ?? 64;
				return kind switch {
					CastKind.Zext or CastKind.Trunc or CastKind.Bitcast => MaskW(a, w),
					CastKind.Sext => MaskW((ulong) SignEx(a, WOf(x)), w),
					_ => throw new NotSupportedException($"cast {kind}")
				};
			}
			case IlLoad(var ty, var addr):
				return Load(Eval(addr), (ty as IlType.I)?.Bits ?? 64);
			case IlIfV(_, var c, var t, var f):
				return (Eval(c) & 1) != 0 ? Eval(t) : Eval(f);
			default: throw new NotSupportedException($"eval {e.GetType().Name}");
		}
	}

	static int WOf(Il e) => (e.Ty as IlType.I)?.Bits ?? 64;
	static ulong MaskTy(IlType t, ulong v) => MaskW(v, (t as IlType.I)?.Bits ?? 64);
	static ulong MaskW(ulong v, int w) => w >= 64 ? v : v & ((1UL << w) - 1);
	static long SignEx(ulong v, int w) => w >= 64 ? (long) v : ((long) (v << (64 - w))) >> (64 - w);
	static ulong Ror(ulong v, int n, int w) { n %= w; return n == 0 ? v : MaskW((v >> n) | (v << (w - n)), w); }
}
