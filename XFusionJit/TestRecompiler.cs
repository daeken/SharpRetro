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
	// Delegates captured PER-INSTANCE and passed to B.Call — the compiled
	// code invokes these; the instance holds the mem[] they read.
	readonly Func<ulong, int, ulong> _load;
	readonly Action<ulong, ulong, int> _store;

	TestRecompiler(byte[] mem) {
		_mem = mem;
		_load = (a, w) => {
			var v = 0UL;
			for(var b = 0; b < w / 8; b++) v |= (ulong) _mem[(int) (a + (ulong) b)] << (b * 8);
			return v;
		};
		_store = (a, val, w) => {
			for(var b = 0; b < w / 8; b++) _mem[(int) (a + (ulong) b)] = (byte) (val >> (b * 8));
		};
	}

	/// Compile a straight-line byte sequence (single block, first branch ends
	/// it). mem[] captured in the returned delegate's closure via the callbacks.
	public static Compiled Compile(byte[] code, ulong pc, XFusionCpu.XMode mode, byte[] mem) {
		var jit = new CilJit<ulong>();
		var self = new TestRecompiler(mem);
		return jit.CreateFunction<Compiled>("blk", builder => {
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
	}

	protected override void Branch(BranchKind kind, IRuntimeValue<ulong> target) {
		S.SetField("Rip", 0x80, target);
		// v0: single-block — the block-driver re-enters at Rip. Emit-side just
		// records the target; the RETURN happens at end-of-body.
	}

	protected override void Intrinsic(string name, IRuntimeValue<ulong>[] args) {
		// v0: unhandled intrinsics = no-op in the harness (X86Machine's
		// StringOp/loop/muldiv semantics are machine-native, not IL —
		// the diff catches divergence naturally). Real host wires callbacks.
	}

	// CilJit stubs Pointer<T> (LlvmJit too) — go via delegate calls on the
	// instance's mem[]. Slower than raw ldind but correct + JIT-backend-portable.
	protected override IRuntimeValue<ulong> Load(IRuntimeValue<ulong> addr, int w) =>
		B.Call(_load, addr, B.LiteralValue(w));
	protected override void Store(IRuntimeValue<ulong> addr, IRuntimeValue<ulong> v, int w) =>
		B.CallVoid(_store, addr, v, B.LiteralValue(w));
}
