using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Timesrv.Detail.Service;
public partial class IStaticService : _IStaticService_Base;
public abstract class _IStaticService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetStandardUserSystemClock
				break;
			case 0x1: // GetStandardNetworkSystemClock
				break;
			case 0x2: // GetStandardSteadyClock
				break;
			case 0x3: // GetTimeZoneService
				break;
			case 0x4: // GetStandardLocalSystemClock
				break;
			case 0x5: // GetEphemeralNetworkSystemClock
				break;
			case 0x32: // SetStandardSteadyClockInternalOffset
				break;
			case 0x64: // IsStandardUserSystemClockAutomaticCorrectionEnabled
				break;
			case 0x65: // SetStandardUserSystemClockAutomaticCorrectionEnabled
				break;
			case 0x66: // GetStandardUserSystemClockInitialYear
				break;
			case 0xC8: // IsStandardNetworkSystemClockAccuracySufficient
				break;
			case 0x12C: // CalculateMonotonicSystemClockBaseTimePoint
				break;
			case 0x190: // GetClockSnapshot
				break;
			case 0x191: // GetClockSnapshotFromSystemClockContext
				break;
			case 0x1F4: // CalculateStandardUserSystemClockDifferenceByUser
				break;
			case 0x1F5: // CalculateSpanBetween
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Timesrv.Detail.Service.IStaticService");
		}
	}
}

public partial class ISteadyClock : _ISteadyClock_Base;
public abstract class _ISteadyClock_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetCurrentTimePoint
				break;
			case 0x2: // GetTestOffset
				break;
			case 0x3: // SetTestOffset
				break;
			case 0x64: // GetRtcValue
				break;
			case 0x65: // IsRtcResetDetected
				break;
			case 0x66: // GetSetupResultValue
				break;
			case 0xC8: // GetInternalOffset
				break;
			case 0xC9: // SetInternalOffset
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Timesrv.Detail.Service.ISteadyClock");
		}
	}
}

public partial class ISystemClock : _ISystemClock_Base;
public abstract class _ISystemClock_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetCurrentTime
				break;
			case 0x1: // SetCurrentTime
				break;
			case 0x2: // GetSystemClockContext
				break;
			case 0x3: // SetSystemClockContext
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Timesrv.Detail.Service.ISystemClock");
		}
	}
}

public partial class ITimeZoneService : _ITimeZoneService_Base;
public abstract class _ITimeZoneService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetDeviceLocationName
				break;
			case 0x1: // SetDeviceLocationName
				break;
			case 0x2: // GetTotalLocationNameCount
				break;
			case 0x3: // LoadLocationNameList
				break;
			case 0x4: // LoadTimeZoneRule
				break;
			case 0x5: // GetTimeZoneRuleVersion
				break;
			case 0x64: // ToCalendarTime
				break;
			case 0x65: // ToCalendarTimeWithMyRule
				break;
			case 0xC9: // ToPosixTime
				break;
			case 0xCA: // ToPosixTimeWithMyRule
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Timesrv.Detail.Service.ITimeZoneService");
		}
	}
}

