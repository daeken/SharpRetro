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
    protected override ulong GetCurrentTime() => 0xCAFEBABE;
}

public partial class ITimeZoneService {
    protected override void ToCalendarTimeWithMyRule(ulong _0, out CalendarTime _1, out CalendarAdditionalInfo _2) {
        _1 = new CalendarTime();
        _2 = new CalendarAdditionalInfo();
    }
}