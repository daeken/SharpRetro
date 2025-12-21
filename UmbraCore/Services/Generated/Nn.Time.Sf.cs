using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Time.Sf;
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct CalendarAdditionalInfo {
	public uint Tm_wday;
	public int Tm_yday;
	public fixed byte Tz_name[8];
	public bool Is_daylight_saving_time;
	public int Utc_offset_seconds;
}
