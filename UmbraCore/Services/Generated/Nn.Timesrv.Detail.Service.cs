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
	protected virtual ulong CalculateMonotonicSystemClockBaseTimePoint(byte[] _0) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.IStaticService.CalculateMonotonicSystemClockBaseTimePoint not implemented");
	protected virtual void GetClockSnapshot(byte _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.IStaticService.GetClockSnapshot not implemented");
	protected virtual void GetClockSnapshotFromSystemClockContext(byte _0, byte[] _1, byte[] _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.IStaticService.GetClockSnapshotFromSystemClockContext not implemented");
	protected virtual ulong CalculateStandardUserSystemClockDifferenceByUser(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.IStaticService.CalculateStandardUserSystemClockDifferenceByUser not implemented");
	protected virtual ulong CalculateSpanBetween(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.IStaticService.CalculateSpanBetween not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetStandardUserSystemClock
				om.Initialize(1, 0, 0);
				var _return = GetStandardUserSystemClock();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetStandardNetworkSystemClock
				om.Initialize(1, 0, 0);
				var _return = GetStandardNetworkSystemClock();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // GetStandardSteadyClock
				om.Initialize(1, 0, 0);
				var _return = GetStandardSteadyClock();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // GetTimeZoneService
				om.Initialize(1, 0, 0);
				var _return = GetTimeZoneService();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x4: { // GetStandardLocalSystemClock
				om.Initialize(1, 0, 0);
				var _return = GetStandardLocalSystemClock();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x5: { // GetEphemeralNetworkSystemClock
				om.Initialize(1, 0, 0);
				var _return = GetEphemeralNetworkSystemClock();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x32: { // SetStandardSteadyClockInternalOffset
				om.Initialize(0, 0, 0);
				SetStandardSteadyClockInternalOffset(im.GetData<ulong>(8));
				break;
			}
			case 0x64: { // IsStandardUserSystemClockAutomaticCorrectionEnabled
				om.Initialize(0, 0, 1);
				var _return = IsStandardUserSystemClockAutomaticCorrectionEnabled();
				om.SetData(8, _return);
				break;
			}
			case 0x65: { // SetStandardUserSystemClockAutomaticCorrectionEnabled
				om.Initialize(0, 0, 0);
				SetStandardUserSystemClockAutomaticCorrectionEnabled(im.GetData<byte>(8));
				break;
			}
			case 0x66: { // GetStandardUserSystemClockInitialYear
				om.Initialize(0, 0, 0);
				GetStandardUserSystemClockInitialYear();
				break;
			}
			case 0xC8: { // IsStandardNetworkSystemClockAccuracySufficient
				om.Initialize(0, 0, 1);
				var _return = IsStandardNetworkSystemClockAccuracySufficient();
				om.SetData(8, _return);
				break;
			}
			case 0x12C: { // CalculateMonotonicSystemClockBaseTimePoint
				om.Initialize(0, 0, 8);
				var _return = CalculateMonotonicSystemClockBaseTimePoint(im.GetBytes(8, 0x20));
				om.SetData(8, _return);
				break;
			}
			case 0x190: { // GetClockSnapshot
				om.Initialize(0, 0, 0);
				GetClockSnapshot(im.GetData<byte>(8), im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x191: { // GetClockSnapshotFromSystemClockContext
				om.Initialize(0, 0, 0);
				GetClockSnapshotFromSystemClockContext(im.GetData<byte>(8), im.GetBytes(16, 0x20), im.GetBytes(48, 0x20), im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x1F4: { // CalculateStandardUserSystemClockDifferenceByUser
				om.Initialize(0, 0, 8);
				var _return = CalculateStandardUserSystemClockDifferenceByUser(im.GetSpan<byte>(0x19, 0), im.GetSpan<byte>(0x19, 1));
				om.SetData(8, _return);
				break;
			}
			case 0x1F5: { // CalculateSpanBetween
				om.Initialize(0, 0, 8);
				var _return = CalculateSpanBetween(im.GetSpan<byte>(0x19, 0), im.GetSpan<byte>(0x19, 1));
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Timesrv.Detail.Service.IStaticService");
		}
	}
}

public partial class ISteadyClock : _ISteadyClock_Base;
public abstract class _ISteadyClock_Base : IpcInterface {
	protected virtual void GetCurrentTimePoint(out byte[] _0) =>
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetCurrentTimePoint
				om.Initialize(0, 0, 24);
				GetCurrentTimePoint(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // GetTestOffset
				om.Initialize(0, 0, 8);
				var _return = GetTestOffset();
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // SetTestOffset
				om.Initialize(0, 0, 0);
				SetTestOffset(im.GetData<ulong>(8));
				break;
			}
			case 0x64: { // GetRtcValue
				om.Initialize(0, 0, 8);
				var _return = GetRtcValue();
				om.SetData(8, _return);
				break;
			}
			case 0x65: { // IsRtcResetDetected
				om.Initialize(0, 0, 1);
				var _return = IsRtcResetDetected();
				om.SetData(8, _return);
				break;
			}
			case 0x66: { // GetSetupResultValue
				om.Initialize(0, 0, 4);
				var _return = GetSetupResultValue();
				om.SetData(8, _return);
				break;
			}
			case 0xC8: { // GetInternalOffset
				om.Initialize(0, 0, 8);
				var _return = GetInternalOffset();
				om.SetData(8, _return);
				break;
			}
			case 0xC9: { // SetInternalOffset
				om.Initialize(0, 0, 0);
				SetInternalOffset(im.GetData<ulong>(8));
				break;
			}
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
	protected virtual void GetSystemClockContext(out byte[] _0) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ISystemClock.GetSystemClockContext not implemented");
	protected virtual void SetSystemClockContext(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Timesrv.Detail.Service.ISystemClock.SetSystemClockContext");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetCurrentTime
				om.Initialize(0, 0, 8);
				var _return = GetCurrentTime();
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // SetCurrentTime
				om.Initialize(0, 0, 0);
				SetCurrentTime(im.GetData<ulong>(8));
				break;
			}
			case 0x2: { // GetSystemClockContext
				om.Initialize(0, 0, 32);
				GetSystemClockContext(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // SetSystemClockContext
				om.Initialize(0, 0, 0);
				SetSystemClockContext(im.GetBytes(8, 0x20));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Timesrv.Detail.Service.ISystemClock");
		}
	}
}

public partial class ITimeZoneService : _ITimeZoneService_Base;
public abstract class _ITimeZoneService_Base : IpcInterface {
	protected virtual void GetDeviceLocationName(out byte[] _0) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ITimeZoneService.GetDeviceLocationName not implemented");
	protected virtual void SetDeviceLocationName(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Timesrv.Detail.Service.ITimeZoneService.SetDeviceLocationName");
	protected virtual uint GetTotalLocationNameCount() =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ITimeZoneService.GetTotalLocationNameCount not implemented");
	protected virtual void LoadLocationNameList(uint _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ITimeZoneService.LoadLocationNameList not implemented");
	protected virtual void LoadTimeZoneRule(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ITimeZoneService.LoadTimeZoneRule not implemented");
	protected virtual void GetTimeZoneRuleVersion(out byte[] _0) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ITimeZoneService.GetTimeZoneRuleVersion not implemented");
	protected virtual void ToCalendarTime(ulong _0, Span<byte> _1, out Nn.Time.CalendarTime _2, out Nn.Time.Sf.CalendarAdditionalInfo _3) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ITimeZoneService.ToCalendarTime not implemented");
	protected virtual void ToCalendarTimeWithMyRule(ulong _0, out Nn.Time.CalendarTime _1, out Nn.Time.Sf.CalendarAdditionalInfo _2) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ITimeZoneService.ToCalendarTimeWithMyRule not implemented");
	protected virtual void ToPosixTime(Nn.Time.CalendarTime _0, Span<byte> _1, out uint _2, Span<ulong> _3) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ITimeZoneService.ToPosixTime not implemented");
	protected virtual void ToPosixTimeWithMyRule(Nn.Time.CalendarTime _0, out uint _1, Span<ulong> _2) =>
		throw new NotImplementedException("Nn.Timesrv.Detail.Service.ITimeZoneService.ToPosixTimeWithMyRule not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetDeviceLocationName
				om.Initialize(0, 0, 36);
				GetDeviceLocationName(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // SetDeviceLocationName
				om.Initialize(0, 0, 0);
				SetDeviceLocationName(im.GetBytes(8, 0x24));
				break;
			}
			case 0x2: { // GetTotalLocationNameCount
				om.Initialize(0, 0, 4);
				var _return = GetTotalLocationNameCount();
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // LoadLocationNameList
				om.Initialize(0, 0, 4);
				LoadLocationNameList(im.GetData<uint>(8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x4: { // LoadTimeZoneRule
				om.Initialize(0, 0, 0);
				LoadTimeZoneRule(im.GetBytes(8, 0x24), im.GetSpan<byte>(0x16, 0));
				break;
			}
			case 0x5: { // GetTimeZoneRuleVersion
				om.Initialize(0, 0, 16);
				GetTimeZoneRuleVersion(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x64: { // ToCalendarTime
				om.Initialize(0, 0, 0);
				ToCalendarTime(im.GetData<ulong>(8), im.GetSpan<byte>(0x15, 0), out var _0, out var _1);
				*(Nn.Time.CalendarTime*) om.GetDataPointer(8) = _0;
				*(Nn.Time.Sf.CalendarAdditionalInfo*) om.GetDataPointer(8) = _1;
				break;
			}
			case 0x65: { // ToCalendarTimeWithMyRule
				om.Initialize(0, 0, 0);
				ToCalendarTimeWithMyRule(im.GetData<ulong>(8), out var _0, out var _1);
				*(Nn.Time.CalendarTime*) om.GetDataPointer(8) = _0;
				*(Nn.Time.Sf.CalendarAdditionalInfo*) om.GetDataPointer(8) = _1;
				break;
			}
			case 0xC9: { // ToPosixTime
				om.Initialize(0, 0, 4);
				ToPosixTime(*(Nn.Time.CalendarTime*) im.GetDataPointer(8), im.GetSpan<byte>(0x15, 0), out var _0, im.GetSpan<ulong>(0xA, 0));
				om.SetData(8, _0);
				break;
			}
			case 0xCA: { // ToPosixTimeWithMyRule
				om.Initialize(0, 0, 4);
				ToPosixTimeWithMyRule(*(Nn.Time.CalendarTime*) im.GetDataPointer(8), out var _0, im.GetSpan<ulong>(0xA, 0));
				om.SetData(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Timesrv.Detail.Service.ITimeZoneService");
		}
	}
}

