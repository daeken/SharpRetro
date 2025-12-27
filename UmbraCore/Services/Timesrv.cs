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
    protected override void ToCalendarTimeWithMyRule(ulong _0, out CalendarTime _1, out CalendarAdditionalInfo _2) {
        $"ToCalendarTimeWithMyRule(0x{_0:X})".Log();
        _1 = new CalendarTime {
            Year = 2025,
            Month = 12,
            Day = 25,
            Hour = 13,
            Minute = 14,
            Second = 15,
        };
        _2 = new CalendarAdditionalInfo();
    }
}