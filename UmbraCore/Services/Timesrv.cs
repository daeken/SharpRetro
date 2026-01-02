using System.Runtime.CompilerServices;
using UmbraCore.Services.Nn.Time.Sf;
using UmbraCore.Services.Nn.Time;

// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Timesrv.Detail.Service;

public partial class IStaticService {
    protected override ISystemClock GetStandardUserSystemClock() => new();
    protected override ISystemClock GetStandardNetworkSystemClock() => new();
    protected override ISteadyClock GetStandardSteadyClock() => new();
    protected override ITimeZoneService GetTimeZoneService() => new();
}

public partial class ISystemClock {
    protected override ulong GetCurrentTime() => (ulong) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}

public partial class ITimeZoneService {
    protected override unsafe void ToCalendarTimeWithMyRule(ulong _0, out CalendarTime _1, out CalendarAdditionalInfo _2) {
        $"ToCalendarTimeWithMyRule(0x{_0:X})".Log();
        _1 = new CalendarTime {
            Year = 2026,
            Month = 1,
            Day = 1,
            Hour = 13,
            Minute = 37,
            Second = 00,
        };
        _2 = new CalendarAdditionalInfo {
            Tm_wday = 1,
            Tm_yday = 1,
            Utc_offset_seconds = 1,
            Is_daylight_saving_time = false,
        };
        fixed(byte* ptr = _2.Tz_name)
            "GMT"u8.CopyTo(new Span<byte>(ptr, 8));
    }
}