using System.Runtime.InteropServices;
using JitBase;

namespace SharpStationCore; 

[StructLayout(LayoutKind.Explicit)]
public unsafe struct CpuState : IJitStruct {
	[FieldOffset(0)] public uint PC;
	[FieldOffset(4)] public uint Hi;
	[FieldOffset(8)] public uint Lo;
	[FieldOffset(12)] public uint LdWhich;
	[FieldOffset(16)] public uint LdValue;
	[FieldOffset(20)] public uint LdAbsorb;
	[FieldOffset(24)] public uint ReadAbsorbWhich;
	[FieldOffset(28)] public uint ReadFudge;
	[FieldOffset(32)] public fixed uint ReadAbsorb[36];
	[FieldOffset(176)] public fixed uint Registers[36];
}