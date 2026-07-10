using System.Runtime.InteropServices;
using JitBase;

namespace XFusionJit;
using XFusionCpu;

/// The JIT-visible state (IJitStruct mirror of X86Machine's fields).
/// GPR file, IP, Eflags word, segment bases. Fixed layout so
/// GetFieldElement offsets are stable.
[StructLayout(LayoutKind.Explicit)]
public unsafe struct X86State : IJitStruct {
	[FieldOffset(0x00)] public fixed ulong Gpr[16];
	// Named aliases for indexer sugar (RuntimeIndexer on Gpr suffices; these
	// help debugging + let a host read/write by name).
	[FieldOffset(0x00)] public ulong Rax;
	[FieldOffset(0x08)] public ulong Rcx;
	[FieldOffset(0x10)] public ulong Rdx;
	[FieldOffset(0x18)] public ulong Rbx;
	[FieldOffset(0x20)] public ulong Rsp;
	[FieldOffset(0x28)] public ulong Rbp;
	[FieldOffset(0x30)] public ulong Rsi;
	[FieldOffset(0x38)] public ulong Rdi;

	[FieldOffset(0x80)] public ulong Rip;
	[FieldOffset(0x88)] public ulong Flags;    // Eflags word (bit-indexed)
	[FieldOffset(0x90)] public fixed ulong SegBase[6];
	// XMM/x87 land here when the vector-IL conversation happens.
}
