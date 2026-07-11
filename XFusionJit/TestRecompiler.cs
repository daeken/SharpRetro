using JitBase;
using LiftIl;
using CilJit;

namespace XFusionJit;

/// The XF-6 acceptance harness: compile a byte sequence via CilJit, run
/// against an X86State + flat memory buffer, diff vs X86Machine on the
/// same inputs. Hooks: Load/Store index the mem arg (arg 1); Branch writes
/// Rip and returns (single-block v0 — the block-driver loop is C0's next
/// half); Intrinsic = call into a host-set delegate.
public unsafe class TestRecompiler : X86Recompiler {
	public delegate void Compiled(X86State* state);

	byte[] _mem;
	X86State* _st;    // set at delegate-invoke time via a fixed(&st) pin from RecompileTests
	readonly XFusionCpu.X86Machine _mirror;  // shared machine-native intrinsic handlers
	readonly List<(string name, int nargs)> _intrinNames = [];
	// Delegates captured PER-INSTANCE and passed to B.Call — the compiled
	// code invokes these; the instance holds the mem[] they read.
	readonly Func<ulong, int, ulong> _load;
	readonly Action<ulong, ulong, int> _store;
	readonly Action<int, ulong, ulong, ulong, ulong> _intrin;  // (nameId, a0..a3)

	public void BindState(X86State* p) => _st = p;

	TestRecompiler(byte[] mem) {
		_mem = mem;
		_mirror = new XFusionCpu.X86Machine { Mode = XFusionCpu.XMode.Bits64, Mem = mem };
		_load = (a, w) => {
			var v = 0UL;
			for(var b = 0; b < w / 8; b++) v |= (ulong) _mem[(int) (a + (ulong) b)] << (b * 8);
			return v;
		};
		_store = (a, val, w) => {
			for(var b = 0; b < w / 8; b++) _mem[(int) (a + (ulong) b)] = (byte) (val >> (b * 8));
		};
		_intrin = (id, a0, a1, a2, a3) => {
			var (name, n) = _intrinNames[id];
			// sync X86State* → mirror
			for(var i = 0; i < 16; i++) _mirror.Gpr[i] = _st->Gpr[i];
			_mirror.Flags = _st->Flags;
			_mirror.Ip = _st->Rip;
			for(var i = 0; i < 6; i++) _mirror.SegBase[i] = _st->SegBase[i];
			var av = new[] { a0, a1, a2, a3 }[..n];
			_mirror.RunIntrinsic(name, av, out var br);
			// sync back
			for(var i = 0; i < 16; i++) _st->Gpr[i] = _mirror.Gpr[i];
			_st->Flags = _mirror.Flags;
			for(var i = 0; i < 6; i++) _st->SegBase[i] = _mirror.SegBase[i];
			// loop/jcxz: br=target or null (not-taken); a3 carries NextPc.
			if(name is "loop" or "loope" or "loopne" or "jcxz")
				_st->Rip = br ?? a3;
			else if(br != null) _st->Rip = br.Value;
		};
	}

	/// Compile a straight-line byte sequence (single block, first branch ends
	/// it). mem[] captured in the returned delegate's closure via the callbacks.
	public static (TestRecompiler, Compiled) Compile(byte[] code, ulong pc, XFusionCpu.XMode mode, byte[] mem) {
		var jit = new CilJit<ulong>();
		var self = new TestRecompiler(mem);
		var fn = jit.CreateFunction<Compiled>("blk", builder => {
			var state = builder.StructRefArgument<X86State>(0);
			self.Begin(builder, state);
			var ip = pc;
			for(var i = 0; i < code.Length && !self.Branched;) {
				var n = self.RecompileOne(code.AsSpan(i), ip, mode);
				if(n == 0) break;
				i += n;
				ip += (ulong) n;
			}
		});
		return (self, fn);
	}

	protected override void Branch(BranchKind kind, IRuntimeValue<ulong> target) {
		S.SetField("Rip", 0x80, target);
		// v0: single-block — the block-driver re-enters at Rip. Emit-side just
		// records the target; the RETURN happens at end-of-body.
	}

	// Bridge into X86Machine.RunIntrinsic — the SAME handler code the
	// interpreter runs. One semantics source; closes open_questions[0]
	// (was no-op = silent divergence).
	protected override void Intrinsic(string name, IRuntimeValue<ulong>[] args) {
		var id = _intrinNames.Count;
		_intrinNames.Add((name, args.Length));
		var z = B.LiteralValue(0UL);
		var a = new[] { z, z, z, z };
		for(var i = 0; i < args.Length && i < 4; i++) a[i] = args[i];
		// loop/jcxz: data-dependent branch. End the block; bridge sets Rip
		// (target or fallthrough); driver re-enters. NextPc rides args slot 3
		// (loop's own args are [target] = 1 real arg, so slot 3 is free).
		var branchy = name is "loop" or "loope" or "loopne" or "jcxz";
		if(branchy) { a[3] = B.LiteralValue(NextPc); Branched = true; }
		B.CallVoid(_intrin, B.LiteralValue(id), a[0], a[1], a[2], a[3]);
	}

	// CilJit stubs Pointer<T> (LlvmJit too) — go via delegate calls on the
	// instance's mem[]. Slower than raw ldind but correct + JIT-backend-portable.
	protected override IRuntimeValue<ulong> Load(IRuntimeValue<ulong> addr, int w) =>
		B.Call(_load, addr, B.LiteralValue(w));
	protected override void Store(IRuntimeValue<ulong> addr, IRuntimeValue<ulong> v, int w) =>
		B.CallVoid(_store, addr, v, B.LiteralValue(w));
}
