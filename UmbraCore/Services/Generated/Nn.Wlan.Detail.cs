using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Wlan.Detail;
public partial class IInfraManager : _IInfraManager_Base;
public abstract class _IInfraManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // GetMacAddress
				break;
			case 0x3: // StartScan
				break;
			case 0x4: // StopScan
				break;
			case 0x5: // Connect
				break;
			case 0x6: // CancelConnect
				break;
			case 0x7: // Disconnect
				break;
			case 0x8: // Unknown8
				break;
			case 0x9: // Unknown9
				break;
			case 0xA: // GetState
				break;
			case 0xB: // GetScanResult
				break;
			case 0xC: // GetRssi
				break;
			case 0xD: // ChangeRxAntenna
				break;
			case 0xE: // Unknown14
				break;
			case 0xF: // Unknown15
				break;
			case 0x10: // RequestWakeUp
				break;
			case 0x11: // RequestIfUpDown
				break;
			case 0x12: // Unknown18
				break;
			case 0x13: // Unknown19
				break;
			case 0x14: // Unknown20
				break;
			case 0x15: // Unknown21
				break;
			case 0x16: // Unknown22
				break;
			case 0x17: // Unknown23
				break;
			case 0x18: // Unknown24
				break;
			case 0x19: // Unknown25
				break;
			case 0x1A: // Unknown26
				break;
			case 0x1B: // Unknown27
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Wlan.Detail.IInfraManager");
		}
	}
}

public partial class ILocalGetActionFrame : _ILocalGetActionFrame_Base;
public abstract class _ILocalGetActionFrame_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Wlan.Detail.ILocalGetActionFrame");
		}
	}
}

public partial class ILocalGetFrame : _ILocalGetFrame_Base;
public abstract class _ILocalGetFrame_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Wlan.Detail.ILocalGetFrame");
		}
	}
}

public partial class ILocalManager : _ILocalManager_Base;
public abstract class _ILocalManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			case 0x5: // Unknown5
				break;
			case 0x6: // GetMacAddress
				break;
			case 0x7: // CreateBss
				break;
			case 0x8: // DestroyBss
				break;
			case 0x9: // StartScan
				break;
			case 0xA: // StopScan
				break;
			case 0xB: // Connect
				break;
			case 0xC: // CancelConnect
				break;
			case 0xD: // Join
				break;
			case 0xE: // CancelJoin
				break;
			case 0xF: // Disconnect
				break;
			case 0x10: // SetBeaconLostCount
				break;
			case 0x11: // Unknown17
				break;
			case 0x12: // Unknown18
				break;
			case 0x13: // Unknown19
				break;
			case 0x14: // GetBssIndicationEvent
				break;
			case 0x15: // GetBssIndicationInfo
				break;
			case 0x16: // GetState
				break;
			case 0x17: // GetAllowedChannels
				break;
			case 0x18: // AddIe
				break;
			case 0x19: // DeleteIe
				break;
			case 0x1A: // Unknown26
				break;
			case 0x1B: // Unknown27
				break;
			case 0x1C: // CreateRxEntry
				break;
			case 0x1D: // DeleteRxEntry
				break;
			case 0x1E: // Unknown30
				break;
			case 0x1F: // Unknown31
				break;
			case 0x20: // AddMatchingDataToRxEntry
				break;
			case 0x21: // RemoveMatchingDataFromRxEntry
				break;
			case 0x22: // GetScanResult
				break;
			case 0x23: // Unknown35
				break;
			case 0x24: // SetActionFrameWithBeacon
				break;
			case 0x25: // CancelActionFrameWithBeacon
				break;
			case 0x26: // CreateRxEntryForActionFrame
				break;
			case 0x27: // DeleteRxEntryForActionFrame
				break;
			case 0x28: // Unknown40
				break;
			case 0x29: // Unknown41
				break;
			case 0x2A: // CancelGetActionFrame
				break;
			case 0x2B: // GetRssi
				break;
			case 0x2C: // Unknown44
				break;
			case 0x2D: // Unknown45
				break;
			case 0x2E: // Unknown46
				break;
			case 0x2F: // Unknown47
				break;
			case 0x30: // Unknown48
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Wlan.Detail.ILocalManager");
		}
	}
}

public partial class ISocketGetFrame : _ISocketGetFrame_Base;
public abstract class _ISocketGetFrame_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Wlan.Detail.ISocketGetFrame");
		}
	}
}

public partial class ISocketManager : _ISocketManager_Base;
public abstract class _ISocketManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			case 0x5: // Unknown5
				break;
			case 0x6: // GetMacAddress
				break;
			case 0x7: // SwitchTsfTimerFunction
				break;
			case 0x8: // Unknown8
				break;
			case 0x9: // Unknown9
				break;
			case 0xA: // Unknown10
				break;
			case 0xB: // Unknown11
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Wlan.Detail.ISocketManager");
		}
	}
}

