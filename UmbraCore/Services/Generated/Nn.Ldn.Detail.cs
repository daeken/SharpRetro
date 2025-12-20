using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ldn.Detail;
public partial class IMonitorService : _IMonitorService_Base;
public abstract class _IMonitorService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetStateForMonitor
				break;
			case 0x1: // GetNetworkInfoForMonitor
				break;
			case 0x2: // GetIpv4AddressForMonitor
				break;
			case 0x3: // GetDisconnectReasonForMonitor
				break;
			case 0x4: // GetSecurityParameterForMonitor
				break;
			case 0x5: // GetNetworkConfigForMonitor
				break;
			case 0x64: // InitializeMonitor
				break;
			case 0x65: // FinalizeMonitor
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldn.Detail.IMonitorService");
		}
	}
}

public partial class IMonitorServiceCreator : _IMonitorServiceCreator_Base;
public abstract class _IMonitorServiceCreator_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateMonitorService
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldn.Detail.IMonitorServiceCreator");
		}
	}
}

public partial class ISystemLocalCommunicationService : _ISystemLocalCommunicationService_Base;
public abstract class _ISystemLocalCommunicationService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetState
				break;
			case 0x1: // GetNetworkInfo
				break;
			case 0x2: // GetIpv4Address
				break;
			case 0x3: // GetDisconnectReason
				break;
			case 0x4: // GetSecurityParameter
				break;
			case 0x5: // GetNetworkConfig
				break;
			case 0x64: // AttachStateChangeEvent
				break;
			case 0x65: // GetNetworkInfoLatestUpdate
				break;
			case 0x66: // Scan
				break;
			case 0x67: // ScanPrivate
				break;
			case 0xC8: // OpenAccessPoint
				break;
			case 0xC9: // CloseAccessPoint
				break;
			case 0xCA: // CreateNetwork
				break;
			case 0xCB: // CreateNetworkPrivate
				break;
			case 0xCC: // DestroyNetwork
				break;
			case 0xCD: // Reject
				break;
			case 0xCE: // SetAdvertiseData
				break;
			case 0xCF: // SetStationAcceptPolicy
				break;
			case 0xD0: // AddAcceptFilterEntry
				break;
			case 0xD1: // ClearAcceptFilter
				break;
			case 0x12C: // OpenStation
				break;
			case 0x12D: // CloseStation
				break;
			case 0x12E: // Connect
				break;
			case 0x12F: // ConnectPrivate
				break;
			case 0x130: // Disconnect
				break;
			case 0x190: // InitializeSystem
				break;
			case 0x191: // FinalizeSystem
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldn.Detail.ISystemLocalCommunicationService");
		}
	}
}

public partial class ISystemServiceCreator : _ISystemServiceCreator_Base;
public abstract class _ISystemServiceCreator_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateSystemLocalCommunicationService
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldn.Detail.ISystemServiceCreator");
		}
	}
}

public partial class IUserLocalCommunicationService : _IUserLocalCommunicationService_Base;
public abstract class _IUserLocalCommunicationService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetState
				break;
			case 0x1: // GetNetworkInfo
				break;
			case 0x2: // GetIpv4Address
				break;
			case 0x3: // GetDisconnectReason
				break;
			case 0x4: // GetSecurityParameter
				break;
			case 0x5: // GetNetworkConfig
				break;
			case 0x64: // AttachStateChangeEvent
				break;
			case 0x65: // GetNetworkInfoLatestUpdate
				break;
			case 0x66: // Scan
				break;
			case 0x67: // ScanPrivate
				break;
			case 0xC8: // OpenAccessPoint
				break;
			case 0xC9: // CloseAccessPoint
				break;
			case 0xCA: // CreateNetwork
				break;
			case 0xCB: // CreateNetworkPrivate
				break;
			case 0xCC: // DestroyNetwork
				break;
			case 0xCD: // Reject
				break;
			case 0xCE: // SetAdvertiseData
				break;
			case 0xCF: // SetStationAcceptPolicy
				break;
			case 0xD0: // AddAcceptFilterEntry
				break;
			case 0xD1: // ClearAcceptFilter
				break;
			case 0x12C: // OpenStation
				break;
			case 0x12D: // CloseStation
				break;
			case 0x12E: // Connect
				break;
			case 0x12F: // ConnectPrivate
				break;
			case 0x130: // Disconnect
				break;
			case 0x190: // Initialize
				break;
			case 0x191: // Finalize
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldn.Detail.IUserLocalCommunicationService");
		}
	}
}

public partial class IUserServiceCreator : _IUserServiceCreator_Base;
public abstract class _IUserServiceCreator_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateUserLocalCommunicationService
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldn.Detail.IUserServiceCreator");
		}
	}
}

