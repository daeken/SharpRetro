using System.Runtime.InteropServices;

namespace SharpStationCore; 

[StructLayout(LayoutKind.Explicit)]
public unsafe struct CpuState {
	[FieldOffset(0)] public uint PC;
	[FieldOffset(4)] public uint Hi;
	[FieldOffset(8)] public uint Lo;
	[FieldOffset(12)] public uint LdWhich;
	[FieldOffset(16)] public uint LdValue;
	[FieldOffset(20)] public uint LdAbsorb;
	[FieldOffset(24)] public fixed uint Registers[32];
}