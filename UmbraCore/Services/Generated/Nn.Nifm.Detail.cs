using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nifm.Detail;
public partial class IGeneralService : _IGeneralService_Base;
public abstract class _IGeneralService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: // GetClientId
				break;
			case 0x2: // CreateScanRequest
				break;
			case 0x4: // CreateRequest
				break;
			case 0x5: // GetCurrentNetworkProfile
				break;
			case 0x6: // EnumerateNetworkInterfaces
				break;
			case 0x7: // EnumerateNetworkProfiles
				break;
			case 0x8: // GetNetworkProfile
				break;
			case 0x9: // SetNetworkProfile
				break;
			case 0xA: // RemoveNetworkProfile
				break;
			case 0xB: // GetScanDataOld
				break;
			case 0xC: // GetCurrentIpAddress
				break;
			case 0xD: // GetCurrentAccessPointOld
				break;
			case 0xE: // CreateTemporaryNetworkProfile
				break;
			case 0xF: // GetCurrentIpConfigInfo
				break;
			case 0x10: // SetWirelessCommunicationEnabled
				break;
			case 0x11: // IsWirelessCommunicationEnabled
				break;
			case 0x12: // GetInternetConnectionStatus
				break;
			case 0x13: // SetEthernetCommunicationEnabled
				break;
			case 0x14: // IsEthernetCommunicationEnabled
				break;
			case 0x15: // IsAnyInternetRequestAccepted
				break;
			case 0x16: // IsAnyForegroundRequestAccepted
				break;
			case 0x17: // PutToSleep
				break;
			case 0x18: // WakeUp
				break;
			case 0x19: // GetSsidListVersion
				break;
			case 0x1A: // SetExclusiveClient
				break;
			case 0x1B: // GetDefaultIpSetting
				break;
			case 0x1C: // SetDefaultIpSetting
				break;
			case 0x1D: // SetWirelessCommunicationEnabledForTest
				break;
			case 0x1E: // SetEthernetCommunicationEnabledForTest
				break;
			case 0x1F: // GetTelemetorySystemEventReadableHandle
				break;
			case 0x20: // GetTelemetryInfo
				break;
			case 0x21: // ConfirmSystemAvailability
				break;
			case 0x22: // SetBackgroundRequestEnabled
				break;
			case 0x23: // GetScanData
				break;
			case 0x24: // GetCurrentAccessPoint
				break;
			case 0x25: // Shutdown
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nifm.Detail.IGeneralService");
		}
	}
}

public partial class INetworkProfile : _INetworkProfile_Base;
public abstract class _INetworkProfile_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Update
				break;
			case 0x1: // PersistOld
				break;
			case 0x2: // Persist
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nifm.Detail.INetworkProfile");
		}
	}
}

public partial class IRequest : _IRequest_Base;
public abstract class _IRequest_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetRequestState
				break;
			case 0x1: // GetResult
				break;
			case 0x2: // GetSystemEventReadableHandles
				break;
			case 0x3: // Cancel
				break;
			case 0x4: // Submit
				break;
			case 0x5: // SetRequirement
				break;
			case 0x6: // SetRequirementPreset
				break;
			case 0x8: // SetPriority
				break;
			case 0x9: // SetNetworkProfileId
				break;
			case 0xA: // SetRejectable
				break;
			case 0xB: // SetConnectionConfirmationOption
				break;
			case 0xC: // SetPersistent
				break;
			case 0xD: // SetInstant
				break;
			case 0xE: // SetSustainable
				break;
			case 0xF: // SetRawPriority
				break;
			case 0x10: // SetGreedy
				break;
			case 0x11: // SetSharable
				break;
			case 0x12: // SetRequirementByRevision
				break;
			case 0x13: // GetRequirement
				break;
			case 0x14: // GetRevision
				break;
			case 0x15: // GetAppletInfo
				break;
			case 0x16: // GetAdditionalInfo
				break;
			case 0x17: // SetKeptInSleep
				break;
			case 0x18: // RegisterSocketDescriptor
				break;
			case 0x19: // UnregisterSocketDescriptor
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nifm.Detail.IRequest");
		}
	}
}

public partial class IScanRequest : _IScanRequest_Base;
public abstract class _IScanRequest_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Submit
				break;
			case 0x1: // IsProcessing
				break;
			case 0x2: // GetResult
				break;
			case 0x3: // GetSystemEventReadableHandle
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nifm.Detail.IScanRequest");
		}
	}
}

public partial class IStaticService : _IStaticService_Base;
public abstract class _IStaticService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x4: // CreateGeneralServiceOld
				break;
			case 0x5: // CreateGeneralService
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nifm.Detail.IStaticService");
		}
	}
}

