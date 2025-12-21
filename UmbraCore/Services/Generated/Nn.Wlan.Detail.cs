using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Wlan.Detail;
public partial class IInfraManager : _IInfraManager_Base {
	public readonly string ServiceName;
	public IInfraManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IInfraManager_Base : IpcInterface {
	protected virtual void Unknown0() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Unknown0");
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Unknown1");
	protected virtual void GetMacAddress(out byte[] _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.IInfraManager.GetMacAddress not implemented");
	protected virtual void StartScan(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.StartScan");
	protected virtual void StopScan() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.StopScan");
	protected virtual void Connect(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Connect");
	protected virtual void CancelConnect() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.CancelConnect");
	protected virtual void Disconnect() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Disconnect");
	protected virtual KObject Unknown8(byte[] _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.IInfraManager.Unknown8 not implemented");
	protected virtual void Unknown9(out byte[] _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.IInfraManager.Unknown9 not implemented");
	protected virtual void GetState(out byte[] _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.IInfraManager.GetState not implemented");
	protected virtual void GetScanResult(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.IInfraManager.GetScanResult not implemented");
	protected virtual void GetRssi(out byte[] _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.IInfraManager.GetRssi not implemented");
	protected virtual void ChangeRxAntenna(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.ChangeRxAntenna");
	protected virtual void Unknown14(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.IInfraManager.Unknown14 not implemented");
	protected virtual void Unknown15() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Unknown15");
	protected virtual void RequestWakeUp() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.RequestWakeUp");
	protected virtual void RequestIfUpDown(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.RequestIfUpDown");
	protected virtual void Unknown18(out byte[] _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.IInfraManager.Unknown18 not implemented");
	protected virtual void Unknown19(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Unknown19");
	protected virtual void Unknown20() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Unknown20");
	protected virtual void Unknown21(out byte[] _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.IInfraManager.Unknown21 not implemented");
	protected virtual void Unknown22(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Unknown22");
	protected virtual void Unknown23(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Unknown23");
	protected virtual void Unknown24(out byte[] _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.IInfraManager.Unknown24 not implemented");
	protected virtual void Unknown25(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Unknown25");
	protected virtual void Unknown26() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Unknown26");
	protected virtual void Unknown27() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Unknown27");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GetMacAddress
				GetMacAddress(out var _0);
				om.Initialize(0, 0, 6);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // StartScan
				StartScan(im.GetSpan<byte>(0x15, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // StopScan
				StopScan();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // Connect
				Connect(im.GetBytes(8, 0x80));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // CancelConnect
				CancelConnect();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // Disconnect
				Disconnect();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // Unknown8
				var _return = Unknown8(im.GetBytes(8, 0x4));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x9: { // Unknown9
				Unknown9(out var _0);
				om.Initialize(0, 0, 60);
				om.SetBytes(8, _0);
				break;
			}
			case 0xA: { // GetState
				GetState(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0xB: { // GetScanResult
				GetScanResult(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // GetRssi
				GetRssi(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD: { // ChangeRxAntenna
				ChangeRxAntenna(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // Unknown14
				Unknown14(im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xF: { // Unknown15
				Unknown15();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // RequestWakeUp
				RequestWakeUp();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x11: { // RequestIfUpDown
				RequestIfUpDown(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12: { // Unknown18
				Unknown18(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x13: { // Unknown19
				Unknown19(im.GetBytes(8, 0x18));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x14: { // Unknown20
				Unknown20();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x15: { // Unknown21
				Unknown21(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x16: { // Unknown22
				Unknown22(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x17: { // Unknown23
				Unknown23(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x18: { // Unknown24
				Unknown24(out var _0);
				om.Initialize(0, 0, 92);
				om.SetBytes(8, _0);
				break;
			}
			case 0x19: { // Unknown25
				Unknown25(im.GetBytes(8, 0x2));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1A: { // Unknown26
				Unknown26();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1B: { // Unknown27
				Unknown27();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Wlan.Detail.IInfraManager");
		}
	}
}

public partial class ILocalGetActionFrame : _ILocalGetActionFrame_Base {
	public readonly string ServiceName;
	public ILocalGetActionFrame(string serviceName) => ServiceName = serviceName;
}
public abstract class _ILocalGetActionFrame_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalGetActionFrame.Unknown0 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(im.GetBytes(8, 0x4), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 12);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Wlan.Detail.ILocalGetActionFrame");
		}
	}
}

public partial class ILocalGetFrame : _ILocalGetFrame_Base {
	public readonly string ServiceName;
	public ILocalGetFrame(string serviceName) => ServiceName = serviceName;
}
public abstract class _ILocalGetFrame_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalGetFrame.Unknown0 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(im.GetBytes(8, 0x4), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Wlan.Detail.ILocalGetFrame");
		}
	}
}

public partial class ILocalManager : _ILocalManager_Base {
	public readonly string ServiceName;
	public ILocalManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _ILocalManager_Base : IpcInterface {
	protected virtual void Unknown0() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown0");
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown1");
	protected virtual void Unknown2() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown2");
	protected virtual void Unknown3() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown3");
	protected virtual void Unknown4() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown4");
	protected virtual void Unknown5() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown5");
	protected virtual void GetMacAddress(out byte[] _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.GetMacAddress not implemented");
	protected virtual void CreateBss(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.CreateBss");
	protected virtual void DestroyBss() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.DestroyBss");
	protected virtual void StartScan(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.StartScan");
	protected virtual void StopScan() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.StopScan");
	protected virtual void Connect(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Connect");
	protected virtual void CancelConnect() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.CancelConnect");
	protected virtual void Join(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Join");
	protected virtual void CancelJoin() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.CancelJoin");
	protected virtual void Disconnect(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Disconnect");
	protected virtual void SetBeaconLostCount(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.SetBeaconLostCount");
	protected virtual KObject Unknown17(byte[] _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.Unknown17 not implemented");
	protected virtual void Unknown18(out byte[] _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.Unknown18 not implemented");
	protected virtual void Unknown19(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.Unknown19 not implemented");
	protected virtual KObject GetBssIndicationEvent() =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.GetBssIndicationEvent not implemented");
	protected virtual void GetBssIndicationInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.GetBssIndicationInfo not implemented");
	protected virtual void GetState(out byte[] _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.GetState not implemented");
	protected virtual void GetAllowedChannels(out byte[] _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.GetAllowedChannels not implemented");
	protected virtual void AddIe(byte[] _0, Span<byte> _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.AddIe not implemented");
	protected virtual void DeleteIe(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.DeleteIe");
	protected virtual void Unknown26(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown26");
	protected virtual void Unknown27(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown27");
	protected virtual void CreateRxEntry(byte[] _0, Span<byte> _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.CreateRxEntry not implemented");
	protected virtual void DeleteRxEntry(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.DeleteRxEntry");
	protected virtual void Unknown30(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown30");
	protected virtual void Unknown31(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.Unknown31 not implemented");
	protected virtual void AddMatchingDataToRxEntry(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.AddMatchingDataToRxEntry");
	protected virtual void RemoveMatchingDataFromRxEntry(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.RemoveMatchingDataFromRxEntry");
	protected virtual void GetScanResult(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.GetScanResult not implemented");
	protected virtual void Unknown35(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown35");
	protected virtual void SetActionFrameWithBeacon(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.SetActionFrameWithBeacon");
	protected virtual void CancelActionFrameWithBeacon() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.CancelActionFrameWithBeacon");
	protected virtual void CreateRxEntryForActionFrame(byte[] _0, Span<byte> _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.CreateRxEntryForActionFrame not implemented");
	protected virtual void DeleteRxEntryForActionFrame(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.DeleteRxEntryForActionFrame");
	protected virtual void Unknown40(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown40");
	protected virtual void Unknown41(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.Unknown41 not implemented");
	protected virtual void CancelGetActionFrame(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.CancelGetActionFrame");
	protected virtual void GetRssi(out byte[] _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.GetRssi not implemented");
	protected virtual void Unknown44(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown44");
	protected virtual void Unknown45() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown45");
	protected virtual void Unknown46() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown46");
	protected virtual void Unknown47() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown47");
	protected virtual void Unknown48() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown48");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // Unknown5
				Unknown5();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // GetMacAddress
				GetMacAddress(out var _0);
				om.Initialize(0, 0, 6);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7: { // CreateBss
				CreateBss(im.GetBytes(8, 0x84));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // DestroyBss
				DestroyBss();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // StartScan
				StartScan(im.GetSpan<byte>(0x15, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // StopScan
				StopScan();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // Connect
				Connect(im.GetBytes(8, 0x84));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // CancelConnect
				CancelConnect();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD: { // Join
				Join(im.GetBytes(8, 0x84));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // CancelJoin
				CancelJoin();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xF: { // Disconnect
				Disconnect(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // SetBeaconLostCount
				SetBeaconLostCount(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x11: { // Unknown17
				var _return = Unknown17(im.GetBytes(8, 0x4));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x12: { // Unknown18
				Unknown18(out var _0);
				om.Initialize(0, 0, 60);
				om.SetBytes(8, _0);
				break;
			}
			case 0x13: { // Unknown19
				Unknown19(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x14: { // GetBssIndicationEvent
				var _return = GetBssIndicationEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x15: { // GetBssIndicationInfo
				GetBssIndicationInfo(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x16: { // GetState
				GetState(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x17: { // GetAllowedChannels
				GetAllowedChannels(out var _0);
				om.Initialize(0, 0, 80);
				om.SetBytes(8, _0);
				break;
			}
			case 0x18: { // AddIe
				AddIe(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x5, 0), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x19: { // DeleteIe
				DeleteIe(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1A: { // Unknown26
				Unknown26(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1B: { // Unknown27
				Unknown27(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1C: { // CreateRxEntry
				CreateRxEntry(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x9, 0), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1D: { // DeleteRxEntry
				DeleteRxEntry(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1E: { // Unknown30
				Unknown30(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1F: { // Unknown31
				Unknown31(im.GetBytes(8, 0x2), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x20: { // AddMatchingDataToRxEntry
				AddMatchingDataToRxEntry(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x21: { // RemoveMatchingDataFromRxEntry
				RemoveMatchingDataFromRxEntry(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x22: { // GetScanResult
				GetScanResult(im.GetSpan<byte>(0x19, 0), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x23: { // Unknown35
				Unknown35(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x24: { // SetActionFrameWithBeacon
				SetActionFrameWithBeacon(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x25: { // CancelActionFrameWithBeacon
				CancelActionFrameWithBeacon();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x26: { // CreateRxEntryForActionFrame
				CreateRxEntryForActionFrame(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x9, 0), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x27: { // DeleteRxEntryForActionFrame
				DeleteRxEntryForActionFrame(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x28: { // Unknown40
				Unknown40(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x29: { // Unknown41
				Unknown41(im.GetBytes(8, 0x4), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2A: { // CancelGetActionFrame
				CancelGetActionFrame(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2B: { // GetRssi
				GetRssi(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2C: { // Unknown44
				Unknown44(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2D: { // Unknown45
				Unknown45();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2E: { // Unknown46
				Unknown46();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2F: { // Unknown47
				Unknown47();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x30: { // Unknown48
				Unknown48();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Wlan.Detail.ILocalManager");
		}
	}
}

public partial class ISocketGetFrame : _ISocketGetFrame_Base {
	public readonly string ServiceName;
	public ISocketGetFrame(string serviceName) => ServiceName = serviceName;
}
public abstract class _ISocketGetFrame_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ISocketGetFrame.Unknown0 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(im.GetBytes(8, 0x4), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Wlan.Detail.ISocketGetFrame");
		}
	}
}

public partial class ISocketManager : _ISocketManager_Base {
	public readonly string ServiceName;
	public ISocketManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _ISocketManager_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ISocketManager.Unknown0");
	protected virtual void Unknown1(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ISocketManager.Unknown1");
	protected virtual void Unknown2(byte[] _0, Span<byte> _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ISocketManager.Unknown2 not implemented");
	protected virtual void Unknown3(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ISocketManager.Unknown3");
	protected virtual void Unknown4(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ISocketManager.Unknown4");
	protected virtual void Unknown5(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ISocketManager.Unknown5 not implemented");
	protected virtual void GetMacAddress(out byte[] _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ISocketManager.GetMacAddress not implemented");
	protected virtual void SwitchTsfTimerFunction(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ISocketManager.SwitchTsfTimerFunction");
	protected virtual void Unknown8(out byte[] _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ISocketManager.Unknown8 not implemented");
	protected virtual void Unknown9(byte[] _0, KObject _1, KObject _2, KObject _3, KObject _4, KObject _5) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ISocketManager.Unknown9");
	protected virtual void Unknown10() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ISocketManager.Unknown10");
	protected virtual void Unknown11() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ISocketManager.Unknown11");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x9, 0), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // Unknown5
				Unknown5(im.GetBytes(8, 0x2), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6: { // GetMacAddress
				GetMacAddress(out var _0);
				om.Initialize(0, 0, 6);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7: { // SwitchTsfTimerFunction
				SwitchTsfTimerFunction(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // Unknown8
				Unknown8(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x9: { // Unknown9
				Unknown9(im.GetBytes(8, 0x4), Kernel.Get<KObject>(im.GetCopy(0)), Kernel.Get<KObject>(im.GetCopy(1)), Kernel.Get<KObject>(im.GetCopy(2)), Kernel.Get<KObject>(im.GetCopy(3)), Kernel.Get<KObject>(im.GetCopy(4)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // Unknown10
				Unknown10();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // Unknown11
				Unknown11();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Wlan.Detail.ISocketManager");
		}
	}
}

