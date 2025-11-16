using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using JitBase;

namespace Aarch64Cpu;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct CpuState : IJitStruct {
    [FieldOffset(0x0000)] public ulong PC;
    [FieldOffset(0x0008)] public ulong SP;

    [FieldOffset(0x0010)] public fixed ulong X[32];
    [FieldOffset(0x0010)] public ulong X0;
    [FieldOffset(0x0018)] public ulong X1;
    [FieldOffset(0x0020)] public ulong X2;
    [FieldOffset(0x0028)] public ulong X3;
    [FieldOffset(0x0030)] public ulong X4;
    [FieldOffset(0x0038)] public ulong X5;
    [FieldOffset(0x0040)] public ulong X6;
    [FieldOffset(0x0048)] public ulong X7;
    [FieldOffset(0x0050)] public ulong X8;
    [FieldOffset(0x0058)] public ulong X9;
    [FieldOffset(0x0060)] public ulong X10;
    [FieldOffset(0x0068)] public ulong X11;
    [FieldOffset(0x0070)] public ulong X12;
    [FieldOffset(0x0078)] public ulong X13;
    [FieldOffset(0x0080)] public ulong X14;
    [FieldOffset(0x0088)] public ulong X15;
    [FieldOffset(0x0090)] public ulong X16;
    [FieldOffset(0x0098)] public ulong X17;
    [FieldOffset(0x00A0)] public ulong X18;
    [FieldOffset(0x00A8)] public ulong X19;
    [FieldOffset(0x00B0)] public ulong X20;
    [FieldOffset(0x00B8)] public ulong X21;
    [FieldOffset(0x00C0)] public ulong X22;
    [FieldOffset(0x00C8)] public ulong X23;
    [FieldOffset(0x00D0)] public ulong X24;
    [FieldOffset(0x00D8)] public ulong X25;
    [FieldOffset(0x00E0)] public ulong X26;
    [FieldOffset(0x00E8)] public ulong X27;
    [FieldOffset(0x00F0)] public ulong X28;
    [FieldOffset(0x00F8)] public ulong X29;
    [FieldOffset(0x0100)] public ulong X30;
    [FieldOffset(0x0108)] public ulong X31;
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    [FieldOffset(0x0110)] public Vector128<float>[] V;
    /*[FieldOffset(0x0110)] public Vector128<float> V0;
    [FieldOffset(0x0120)] public Vector128<float> V1;
    [FieldOffset(0x0130)] public Vector128<float> V2;
    [FieldOffset(0x0140)] public Vector128<float> V3;
    [FieldOffset(0x0150)] public Vector128<float> V4;
    [FieldOffset(0x0160)] public Vector128<float> V5;
    [FieldOffset(0x0170)] public Vector128<float> V6;
    [FieldOffset(0x0180)] public Vector128<float> V7;
    [FieldOffset(0x0190)] public Vector128<float> V8;
    [FieldOffset(0x01a0)] public Vector128<float> V9;
    [FieldOffset(0x01b0)] public Vector128<float> V10;
    [FieldOffset(0x01c0)] public Vector128<float> V11;
    [FieldOffset(0x01d0)] public Vector128<float> V12;
    [FieldOffset(0x01e0)] public Vector128<float> V13;
    [FieldOffset(0x01f0)] public Vector128<float> V14;
    [FieldOffset(0x0200)] public Vector128<float> V15;
    [FieldOffset(0x0210)] public Vector128<float> V16;
    [FieldOffset(0x0220)] public Vector128<float> V17;
    [FieldOffset(0x0230)] public Vector128<float> V18;
    [FieldOffset(0x0240)] public Vector128<float> V19;
    [FieldOffset(0x0250)] public Vector128<float> V20;
    [FieldOffset(0x0260)] public Vector128<float> V21;
    [FieldOffset(0x0270)] public Vector128<float> V22;
    [FieldOffset(0x0280)] public Vector128<float> V23;
    [FieldOffset(0x0290)] public Vector128<float> V24;
    [FieldOffset(0x02a0)] public Vector128<float> V25;
    [FieldOffset(0x02b0)] public Vector128<float> V26;
    [FieldOffset(0x02c0)] public Vector128<float> V27;
    [FieldOffset(0x02d0)] public Vector128<float> V28;
    [FieldOffset(0x02e0)] public Vector128<float> V29;
    [FieldOffset(0x02f0)] public Vector128<float> V30;
    [FieldOffset(0x0300)] public Vector128<float> V31;*/

    [FieldOffset(0x310)] public ulong TlsBase;
    [FieldOffset(0x318)] public ulong BranchTo;

    [FieldOffset(0x320)] public byte Exclusive8;
    [FieldOffset(0x322)] public ushort Exclusive16;
    [FieldOffset(0x324)] public uint Exclusive32;
    [FieldOffset(0x328)] public ulong Exclusive64;

    [FieldOffset(0x330)] public bool NZCV_N;
    [FieldOffset(0x338)] public bool NZCV_Z;
    [FieldOffset(0x340)] public bool NZCV_C;
    [FieldOffset(0x348)] public bool NZCV_V;
}