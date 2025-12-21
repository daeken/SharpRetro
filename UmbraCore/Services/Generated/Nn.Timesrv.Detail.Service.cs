using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Timesrv.Detail.Service;
public partial class IStaticService : _IStaticService_Base;
public abstract class _IStaticService_Base : IpcInterface {
	protected virtual Nn.Timesrv.Detail.Service.ISystemClock GetStandardUserSystemClock() =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.IStaticService.GetStandardUserSystemClock not implemented");
	protected virtual Nn.Timesrv.Detail.Service.ISystemClock GetStandardNetworkSystemClock() =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.IStaticService.GetStandardNetworkSystemClock not implemented");
	protected virtual Nn.Timesrv.Detail.Service.ISteadyClock GetStandardSteadyClock() =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.IStaticService.GetStandardSteadyClock not implemented");
	protected virtual Nn.Timesrv.Detail.Service.ITimeZoneService GetTimeZoneService() =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.IStaticService.GetTimeZoneService not implemented");
	protected virtual Nn.Timesrv.Detail.Service.ISystemClock GetStandardLocalSystemClock() =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.IStaticService.GetStandardLocalSystemClock not implemented");
	protected virtual Nn.Timesrv.Detail.Service.ISystemClock GetEphemeralNetworkSystemClock() =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.IStaticService.GetEphemeralNetworkSystemClock not implemented");
	protected virtual void SetStandardSteadyClockInternalOffset(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Timesrv.Detail.Service.IStaticService.SetStandardSteadyClockInternalOffset");
	protected virtual byte IsStandardUserSystemClockAutomaticCorrectionEnabled() =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.IStaticService.IsStandardUserSystemClockAutomaticCorrectionEnabled not implemented");
	protected virtual void SetStandardUserSystemClockAutomaticCorrectionEnabled(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Timesrv.Detail.Service.IStaticService.SetStandardUserSystemClockAutomaticCorrectionEnabled");
	protected virtual void GetStandardUserSystemClockInitialYear() =>
		Console.WriteLine("Stub hit for Nn.Timesrv.Detail.Service.IStaticService.GetStandardUserSystemClockInitialYear");
	protected virtual byte IsStandardNetworkSystemClockAccuracySufficient() =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.IStaticService.IsStandardNetworkSystemClockAccuracySufficient not implemented");
	protected virtual ulong CalculateMonotonicSystemClockBaseTimePoint(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.IStaticService.CalculateMonotonicSystemClockBaseTimePoint not implemented");
	protected virtual void GetClockSnapshot(byte _0) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.IStaticService.GetClockSnapshot not implemented");
	protected virtual void GetClockSnapshotFromSystemClockContext(byte _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.IStaticService.GetClockSnapshotFromSystemClockContext not implemented");
	protected virtual ulong CalculateStandardUserSystemClockDifferenceByUser(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.IStaticService.CalculateStandardUserSystemClockDifferenceByUser not implemented");
	protected virtual ulong CalculateSpanBetween(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.IStaticService.CalculateSpanBetween not implemented");
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
	protected virtual void GetCurrentTimePoint() =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ISteadyClock.GetCurrentTimePoint not implemented");
	protected virtual ulong GetTestOffset() =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ISteadyClock.GetTestOffset not implemented");
	protected virtual void SetTestOffset(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Timesrv.Detail.Service.ISteadyClock.SetTestOffset");
	protected virtual ulong GetRtcValue() =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ISteadyClock.GetRtcValue not implemented");
	protected virtual byte IsRtcResetDetected() =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ISteadyClock.IsRtcResetDetected not implemented");
	protected virtual uint GetSetupResultValue() =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ISteadyClock.GetSetupResultValue not implemented");
	protected virtual ulong GetInternalOffset() =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ISteadyClock.GetInternalOffset not implemented");
	protected virtual void SetInternalOffset(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Timesrv.Detail.Service.ISteadyClock.SetInternalOffset");
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
	protected virtual ulong GetCurrentTime() =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ISystemClock.GetCurrentTime not implemented");
	protected virtual void SetCurrentTime(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Timesrv.Detail.Service.ISystemClock.SetCurrentTime");
	protected virtual void GetSystemClockContext() =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ISystemClock.GetSystemClockContext not implemented");
	protected virtual void SetSystemClockContext(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Timesrv.Detail.Service.ISystemClock.SetSystemClockContext");
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
	protected virtual void GetDeviceLocationName() =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ITimeZoneService.GetDeviceLocationName not implemented");
	protected virtual void SetDeviceLocationName(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Timesrv.Detail.Service.ITimeZoneService.SetDeviceLocationName");
	protected virtual uint GetTotalLocationNameCount() =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ITimeZoneService.GetTotalLocationNameCount not implemented");
	protected virtual void LoadLocationNameList(uint _0) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ITimeZoneService.LoadLocationNameList not implemented");
	protected virtual void LoadTimeZoneRule(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ITimeZoneService.LoadTimeZoneRule not implemented");
	protected virtual void GetTimeZoneRuleVersion() =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ITimeZoneService.GetTimeZoneRuleVersion not implemented");
	protected virtual void ToCalendarTime(ulong _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ITimeZoneService.ToCalendarTime not implemented");
	protected virtual void ToCalendarTimeWithMyRule(ulong _0) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ITimeZoneService.ToCalendarTimeWithMyRule not implemented");
	protected virtual void ToPosixTime(Nn.Time.CalendarTime _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ITimeZoneService.ToPosixTime not implemented");
	protected virtual void ToPosixTimeWithMyRule(Nn.Time.CalendarTime _0) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ITimeZoneService.ToPosixTimeWithMyRule not implemented");
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

