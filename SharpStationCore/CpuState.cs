using System.Runtime.InteropServices;

namespace SharpStationCore; 

[StructLayout(LayoutKind.Explicit)]
public unsafe struct CpuState {
	[FieldOffset(0)] public uint PC;
	[FieldOffset(4)] public uint Hi;
	[FieldOffset(8)] public uint Lo;
	[FieldOffset(16)] public fixed uint Registers[32];
}