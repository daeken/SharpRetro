using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nim.Detail;
public partial class IAsyncData : _IAsyncData_Base;
public abstract class _IAsyncData_Base : IpcInterface {
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
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nim.Detail.IAsyncData");
		}
	}
}

public partial class IAsyncProgressResult : _IAsyncProgressResult_Base;
public abstract class _IAsyncProgressResult_Base : IpcInterface {
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
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nim.Detail.IAsyncProgressResult");
		}
	}
}

public partial class IAsyncResult : _IAsyncResult_Base;
public abstract class _IAsyncResult_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nim.Detail.IAsyncResult");
		}
	}
}

public partial class IAsyncValue : _IAsyncValue_Base;
public abstract class _IAsyncValue_Base : IpcInterface {
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
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nim.Detail.IAsyncValue");
		}
	}
}

public partial class INetworkInstallManager : _INetworkInstallManager_Base;
public abstract class _INetworkInstallManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateSystemUpdateTask
				break;
			case 0x1: // DestroySystemUpdateTask
				break;
			case 0x2: // ListSystemUpdateTask
				break;
			case 0x3: // RequestSystemUpdateTaskRun
				break;
			case 0x4: // GetSystemUpdateTaskInfo
				break;
			case 0x5: // CommitSystemUpdateTask
				break;
			case 0x6: // CreateNetworkInstallTask
				break;
			case 0x7: // DestroyNetworkInstallTask
				break;
			case 0x8: // ListNetworkInstallTask
				break;
			case 0x9: // RequestNetworkInstallTaskRun
				break;
			case 0xA: // GetNetworkInstallTaskInfo
				break;
			case 0xB: // CommitNetworkInstallTask
				break;
			case 0xC: // RequestLatestSystemUpdateMeta
				break;
			case 0xE: // ListApplicationNetworkInstallTask
				break;
			case 0xF: // ListNetworkInstallTaskContentMeta
				break;
			case 0x10: // RequestLatestVersion
				break;
			case 0x11: // SetNetworkInstallTaskAttribute
				break;
			case 0x12: // AddNetworkInstallTaskContentMeta
				break;
			case 0x13: // GetDownloadedSystemDataPath
				break;
			case 0x14: // CalculateNetworkInstallTaskRequiredSize
				break;
			case 0x15: // IsExFatDriverIncluded
				break;
			case 0x16: // GetBackgroundDownloadStressTaskInfo
				break;
			case 0x17: // RequestDeviceAuthenticationToken
				break;
			case 0x18: // RequestGameCardRegistrationStatus
				break;
			case 0x19: // RequestRegisterGameCard
				break;
			case 0x1A: // RequestRegisterNotificationToken
				break;
			case 0x1B: // RequestDownloadTaskList
				break;
			case 0x1C: // RequestApplicationControl
				break;
			case 0x1D: // RequestLatestApplicationControl
				break;
			case 0x1E: // RequestVersionList
				break;
			case 0x1F: // CreateApplyDeltaTask
				break;
			case 0x20: // DestroyApplyDeltaTask
				break;
			case 0x21: // ListApplicationApplyDeltaTask
				break;
			case 0x22: // RequestApplyDeltaTaskRun
				break;
			case 0x23: // GetApplyDeltaTaskInfo
				break;
			case 0x24: // ListApplyDeltaTask
				break;
			case 0x25: // CommitApplyDeltaTask
				break;
			case 0x26: // CalculateApplyDeltaTaskRequiredSize
				break;
			case 0x27: // PrepareShutdown
				break;
			case 0x28: // ListApplyDeltaTask2
				break;
			case 0x29: // ClearNotEnoughSpaceStateOfApplyDeltaTask
				break;
			case 0x2A: // Unknown42
				break;
			case 0x2B: // Unknown43
				break;
			case 0x2C: // Unknown44
				break;
			case 0x2D: // Unknown45
				break;
			case 0x2E: // Unknown46
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nim.Detail.INetworkInstallManager");
		}
	}
}

public partial class IShopServiceManager : _IShopServiceManager_Base;
public abstract class _IShopServiceManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // RequestDeviceAuthenticationToken
				break;
			case 0x1: // RequestCachedDeviceAuthenticationToken
				break;
			case 0x64: // RequestRegisterDeviceAccount
				break;
			case 0x65: // RequestUnregisterDeviceAccount
				break;
			case 0x66: // RequestDeviceAccountStatus
				break;
			case 0x67: // GetDeviceAccountInfo
				break;
			case 0x68: // RequestDeviceRegistrationInfo
				break;
			case 0x69: // RequestTransferDeviceAccount
				break;
			case 0x6A: // RequestSyncRegistration
				break;
			case 0x6B: // IsOwnDeviceId
				break;
			case 0xC8: // RequestRegisterNotificationToken
				break;
			case 0x12C: // RequestUnlinkDevice
				break;
			case 0x12D: // RequestUnlinkDeviceIntegrated
				break;
			case 0x12E: // RequestLinkDevice
				break;
			case 0x12F: // HasDeviceLink
				break;
			case 0x130: // RequestUnlinkDeviceAll
				break;
			case 0x131: // RequestCreateVirtualAccount
				break;
			case 0x132: // RequestDeviceLinkStatus
				break;
			case 0x190: // GetAccountByVirtualAccount
				break;
			case 0x1F4: // RequestSyncTicket
				break;
			case 0x1F5: // RequestDownloadTicket
				break;
			case 0x1F6: // RequestDownloadTicketForPrepurchasedContents
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nim.Detail.IShopServiceManager");
		}
	}
}

