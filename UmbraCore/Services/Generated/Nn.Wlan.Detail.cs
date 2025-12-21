using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Wlan.Detail;
public partial class IInfraManager : _IInfraManager_Base;
public abstract class _IInfraManager_Base : IpcInterface {
	protected virtual void Unknown0() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Unknown0");
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Unknown1");
	protected virtual void GetMacAddress(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.IInfraManager.GetMacAddress not implemented");
	protected virtual void StartScan(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.StartScan");
	protected virtual void StopScan() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.StopScan");
	protected virtual void Connect(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Connect");
	protected virtual void CancelConnect() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.CancelConnect");
	protected virtual void Disconnect() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Disconnect");
	protected virtual KObject Unknown8(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.IInfraManager.Unknown8 not implemented");
	protected virtual void Unknown9(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.IInfraManager.Unknown9 not implemented");
	protected virtual void GetState(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.IInfraManager.GetState not implemented");
	protected virtual void GetScanResult(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.IInfraManager.GetScanResult not implemented");
	protected virtual void GetRssi(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.IInfraManager.GetRssi not implemented");
	protected virtual void ChangeRxAntenna(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.ChangeRxAntenna");
	protected virtual void Unknown14(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.IInfraManager.Unknown14 not implemented");
	protected virtual void Unknown15() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Unknown15");
	protected virtual void RequestWakeUp() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.RequestWakeUp");
	protected virtual void RequestIfUpDown(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.RequestIfUpDown");
	protected virtual void Unknown18(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.IInfraManager.Unknown18 not implemented");
	protected virtual void Unknown19(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Unknown19");
	protected virtual void Unknown20() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Unknown20");
	protected virtual void Unknown21(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.IInfraManager.Unknown21 not implemented");
	protected virtual void Unknown22(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Unknown22");
	protected virtual void Unknown23(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Unknown23");
	protected virtual void Unknown24(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.IInfraManager.Unknown24 not implemented");
	protected virtual void Unknown25(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Unknown25");
	protected virtual void Unknown26() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Unknown26");
	protected virtual void Unknown27() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.IInfraManager.Unknown27");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				break;
			}
			case 0x1: { // Unknown1
				break;
			}
			case 0x2: { // GetMacAddress
				break;
			}
			case 0x3: { // StartScan
				break;
			}
			case 0x4: { // StopScan
				break;
			}
			case 0x5: { // Connect
				break;
			}
			case 0x6: { // CancelConnect
				break;
			}
			case 0x7: { // Disconnect
				break;
			}
			case 0x8: { // Unknown8
				break;
			}
			case 0x9: { // Unknown9
				break;
			}
			case 0xA: { // GetState
				break;
			}
			case 0xB: { // GetScanResult
				break;
			}
			case 0xC: { // GetRssi
				break;
			}
			case 0xD: { // ChangeRxAntenna
				break;
			}
			case 0xE: { // Unknown14
				break;
			}
			case 0xF: { // Unknown15
				break;
			}
			case 0x10: { // RequestWakeUp
				break;
			}
			case 0x11: { // RequestIfUpDown
				break;
			}
			case 0x12: { // Unknown18
				break;
			}
			case 0x13: { // Unknown19
				break;
			}
			case 0x14: { // Unknown20
				break;
			}
			case 0x15: { // Unknown21
				break;
			}
			case 0x16: { // Unknown22
				break;
			}
			case 0x17: { // Unknown23
				break;
			}
			case 0x18: { // Unknown24
				break;
			}
			case 0x19: { // Unknown25
				break;
			}
			case 0x1A: { // Unknown26
				break;
			}
			case 0x1B: { // Unknown27
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Wlan.Detail.IInfraManager");
		}
	}
}

public partial class ILocalGetActionFrame : _ILocalGetActionFrame_Base;
public abstract class _ILocalGetActionFrame_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalGetActionFrame.Unknown0 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Wlan.Detail.ILocalGetActionFrame");
		}
	}
}

public partial class ILocalGetFrame : _ILocalGetFrame_Base;
public abstract class _ILocalGetFrame_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalGetFrame.Unknown0 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Wlan.Detail.ILocalGetFrame");
		}
	}
}

public partial class ILocalManager : _ILocalManager_Base;
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
	protected virtual void GetMacAddress(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.GetMacAddress not implemented");
	protected virtual void CreateBss(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.CreateBss");
	protected virtual void DestroyBss() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.DestroyBss");
	protected virtual void StartScan(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.StartScan");
	protected virtual void StopScan() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.StopScan");
	protected virtual void Connect(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Connect");
	protected virtual void CancelConnect() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.CancelConnect");
	protected virtual void Join(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Join");
	protected virtual void CancelJoin() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.CancelJoin");
	protected virtual void Disconnect(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Disconnect");
	protected virtual void SetBeaconLostCount(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.SetBeaconLostCount");
	protected virtual KObject Unknown17(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.Unknown17 not implemented");
	protected virtual void Unknown18(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.Unknown18 not implemented");
	protected virtual void Unknown19(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.Unknown19 not implemented");
	protected virtual KObject GetBssIndicationEvent() =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.GetBssIndicationEvent not implemented");
	protected virtual void GetBssIndicationInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.GetBssIndicationInfo not implemented");
	protected virtual void GetState(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.GetState not implemented");
	protected virtual void GetAllowedChannels(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.GetAllowedChannels not implemented");
	protected virtual void AddIe(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.AddIe not implemented");
	protected virtual void DeleteIe(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.DeleteIe");
	protected virtual void Unknown26(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown26");
	protected virtual void Unknown27(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown27");
	protected virtual void CreateRxEntry(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.CreateRxEntry not implemented");
	protected virtual void DeleteRxEntry(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.DeleteRxEntry");
	protected virtual void Unknown30(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown30");
	protected virtual void Unknown31(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.Unknown31 not implemented");
	protected virtual void AddMatchingDataToRxEntry(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.AddMatchingDataToRxEntry");
	protected virtual void RemoveMatchingDataFromRxEntry(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.RemoveMatchingDataFromRxEntry");
	protected virtual void GetScanResult(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.GetScanResult not implemented");
	protected virtual void Unknown35(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown35");
	protected virtual void SetActionFrameWithBeacon(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.SetActionFrameWithBeacon");
	protected virtual void CancelActionFrameWithBeacon() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.CancelActionFrameWithBeacon");
	protected virtual void CreateRxEntryForActionFrame(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.CreateRxEntryForActionFrame not implemented");
	protected virtual void DeleteRxEntryForActionFrame(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.DeleteRxEntryForActionFrame");
	protected virtual void Unknown40(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown40");
	protected virtual void Unknown41(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.Unknown41 not implemented");
	protected virtual void CancelGetActionFrame(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.CancelGetActionFrame");
	protected virtual void GetRssi(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ILocalManager.GetRssi not implemented");
	protected virtual void Unknown44(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown44");
	protected virtual void Unknown45() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown45");
	protected virtual void Unknown46() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown46");
	protected virtual void Unknown47() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown47");
	protected virtual void Unknown48() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ILocalManager.Unknown48");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				break;
			}
			case 0x1: { // Unknown1
				break;
			}
			case 0x2: { // Unknown2
				break;
			}
			case 0x3: { // Unknown3
				break;
			}
			case 0x4: { // Unknown4
				break;
			}
			case 0x5: { // Unknown5
				break;
			}
			case 0x6: { // GetMacAddress
				break;
			}
			case 0x7: { // CreateBss
				break;
			}
			case 0x8: { // DestroyBss
				break;
			}
			case 0x9: { // StartScan
				break;
			}
			case 0xA: { // StopScan
				break;
			}
			case 0xB: { // Connect
				break;
			}
			case 0xC: { // CancelConnect
				break;
			}
			case 0xD: { // Join
				break;
			}
			case 0xE: { // CancelJoin
				break;
			}
			case 0xF: { // Disconnect
				break;
			}
			case 0x10: { // SetBeaconLostCount
				break;
			}
			case 0x11: { // Unknown17
				break;
			}
			case 0x12: { // Unknown18
				break;
			}
			case 0x13: { // Unknown19
				break;
			}
			case 0x14: { // GetBssIndicationEvent
				break;
			}
			case 0x15: { // GetBssIndicationInfo
				break;
			}
			case 0x16: { // GetState
				break;
			}
			case 0x17: { // GetAllowedChannels
				break;
			}
			case 0x18: { // AddIe
				break;
			}
			case 0x19: { // DeleteIe
				break;
			}
			case 0x1A: { // Unknown26
				break;
			}
			case 0x1B: { // Unknown27
				break;
			}
			case 0x1C: { // CreateRxEntry
				break;
			}
			case 0x1D: { // DeleteRxEntry
				break;
			}
			case 0x1E: { // Unknown30
				break;
			}
			case 0x1F: { // Unknown31
				break;
			}
			case 0x20: { // AddMatchingDataToRxEntry
				break;
			}
			case 0x21: { // RemoveMatchingDataFromRxEntry
				break;
			}
			case 0x22: { // GetScanResult
				break;
			}
			case 0x23: { // Unknown35
				break;
			}
			case 0x24: { // SetActionFrameWithBeacon
				break;
			}
			case 0x25: { // CancelActionFrameWithBeacon
				break;
			}
			case 0x26: { // CreateRxEntryForActionFrame
				break;
			}
			case 0x27: { // DeleteRxEntryForActionFrame
				break;
			}
			case 0x28: { // Unknown40
				break;
			}
			case 0x29: { // Unknown41
				break;
			}
			case 0x2A: { // CancelGetActionFrame
				break;
			}
			case 0x2B: { // GetRssi
				break;
			}
			case 0x2C: { // Unknown44
				break;
			}
			case 0x2D: { // Unknown45
				break;
			}
			case 0x2E: { // Unknown46
				break;
			}
			case 0x2F: { // Unknown47
				break;
			}
			case 0x30: { // Unknown48
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Wlan.Detail.ILocalManager");
		}
	}
}

public partial class ISocketGetFrame : _ISocketGetFrame_Base;
public abstract class _ISocketGetFrame_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ISocketGetFrame.Unknown0 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Wlan.Detail.ISocketGetFrame");
		}
	}
}

public partial class ISocketManager : _ISocketManager_Base;
public abstract class _ISocketManager_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ISocketManager.Unknown0");
	protected virtual void Unknown1(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ISocketManager.Unknown1");
	protected virtual void Unknown2(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ISocketManager.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ISocketManager.Unknown3");
	protected virtual void Unknown4(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ISocketManager.Unknown4");
	protected virtual void Unknown5(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ISocketManager.Unknown5 not implemented");
	protected virtual void GetMacAddress(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ISocketManager.GetMacAddress not implemented");
	protected virtual void SwitchTsfTimerFunction(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ISocketManager.SwitchTsfTimerFunction");
	protected virtual void Unknown8(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Wlan.Detail.ISocketManager.Unknown8 not implemented");
	protected virtual void Unknown9(Span<byte> _0, KObject _1, KObject _2, KObject _3, KObject _4, KObject _5) =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ISocketManager.Unknown9");
	protected virtual void Unknown10() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ISocketManager.Unknown10");
	protected virtual void Unknown11() =>
		Console.WriteLine("Stub hit for Nn.Wlan.Detail.ISocketManager.Unknown11");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				break;
			}
			case 0x1: { // Unknown1
				break;
			}
			case 0x2: { // Unknown2
				break;
			}
			case 0x3: { // Unknown3
				break;
			}
			case 0x4: { // Unknown4
				break;
			}
			case 0x5: { // Unknown5
				break;
			}
			case 0x6: { // GetMacAddress
				break;
			}
			case 0x7: { // SwitchTsfTimerFunction
				break;
			}
			case 0x8: { // Unknown8
				break;
			}
			case 0x9: { // Unknown9
				break;
			}
			case 0xA: { // Unknown10
				break;
			}
			case 0xB: { // Unknown11
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Wlan.Detail.ISocketManager");
		}
	}
}

