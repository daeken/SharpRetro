using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Audio;
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct AudioInBuffer {
	public ulong Next;
	public ulong Samples;
	public ulong Capacity;
	public ulong Size;
	public ulong Offset;
}
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct AudioOutBuffer {
	public ulong Next;
	public ulong Samples;
	public ulong Capacity;
	public ulong Size;
	public ulong Offset;
}
