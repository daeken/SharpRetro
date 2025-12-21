using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Time;
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct CalendarTime {
	public ushort Year;
	public byte Month;
	public byte Day;
	public byte Hour;
	public byte Minute;
	public byte Second;
}
