using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nim.Detail;
public partial class IAsyncData : _IAsyncData_Base;
public abstract class _IAsyncData_Base : IpcInterface {
	protected virtual void Unknown0() =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.IAsyncData.Unknown0");
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.IAsyncData.Unknown1");
	protected virtual void Unknown2() =>
		throw new NotImplementedException("Nn.Nim.Detail.IAsyncData.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IAsyncData.Unknown3 not implemented");
	protected virtual void Unknown4() =>
		throw new NotImplementedException("Nn.Nim.Detail.IAsyncData.Unknown4 not implemented");
	protected virtual void Unknown5() =>
		throw new NotImplementedException("Nn.Nim.Detail.IAsyncData.Unknown5 not implemented");
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
	protected virtual void Unknown0() =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.IAsyncProgressResult.Unknown0");
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.IAsyncProgressResult.Unknown1");
	protected virtual void Unknown2() =>
		throw new NotImplementedException("Nn.Nim.Detail.IAsyncProgressResult.Unknown2 not implemented");
	protected virtual void Unknown3() =>
		throw new NotImplementedException("Nn.Nim.Detail.IAsyncProgressResult.Unknown3 not implemented");
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
	protected virtual void Unknown0() =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.IAsyncResult.Unknown0");
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.IAsyncResult.Unknown1");
	protected virtual void Unknown2() =>
		throw new NotImplementedException("Nn.Nim.Detail.IAsyncResult.Unknown2 not implemented");
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
	protected virtual void Unknown0() =>
		throw new NotImplementedException("Nn.Nim.Detail.IAsyncValue.Unknown0 not implemented");
	protected virtual void Unknown1() =>
		throw new NotImplementedException("Nn.Nim.Detail.IAsyncValue.Unknown1 not implemented");
	protected virtual void Unknown2() =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.IAsyncValue.Unknown2");
	protected virtual void Unknown3() =>
		throw new NotImplementedException("Nn.Nim.Detail.IAsyncValue.Unknown3 not implemented");
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
	protected virtual void CreateSystemUpdateTask(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.CreateSystemUpdateTask not implemented");
	protected virtual void DestroySystemUpdateTask(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.INetworkInstallManager.DestroySystemUpdateTask");
	protected virtual void ListSystemUpdateTask() =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.ListSystemUpdateTask not implemented");
	protected virtual void RequestSystemUpdateTaskRun(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestSystemUpdateTaskRun not implemented");
	protected virtual void GetSystemUpdateTaskInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.GetSystemUpdateTaskInfo not implemented");
	protected virtual void CommitSystemUpdateTask(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.INetworkInstallManager.CommitSystemUpdateTask");
	protected virtual void CreateNetworkInstallTask(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.CreateNetworkInstallTask not implemented");
	protected virtual void DestroyNetworkInstallTask(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.INetworkInstallManager.DestroyNetworkInstallTask");
	protected virtual void ListNetworkInstallTask() =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.ListNetworkInstallTask not implemented");
	protected virtual void RequestNetworkInstallTaskRun(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestNetworkInstallTaskRun not implemented");
	protected virtual void GetNetworkInstallTaskInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.GetNetworkInstallTaskInfo not implemented");
	protected virtual void CommitNetworkInstallTask(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.INetworkInstallManager.CommitNetworkInstallTask");
	protected virtual void RequestLatestSystemUpdateMeta() =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestLatestSystemUpdateMeta not implemented");
	protected virtual void ListApplicationNetworkInstallTask(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.ListApplicationNetworkInstallTask not implemented");
	protected virtual void ListNetworkInstallTaskContentMeta(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.ListNetworkInstallTaskContentMeta not implemented");
	protected virtual void RequestLatestVersion(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestLatestVersion not implemented");
	protected virtual void SetNetworkInstallTaskAttribute(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.INetworkInstallManager.SetNetworkInstallTaskAttribute");
	protected virtual void AddNetworkInstallTaskContentMeta(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.INetworkInstallManager.AddNetworkInstallTaskContentMeta");
	protected virtual void GetDownloadedSystemDataPath(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.GetDownloadedSystemDataPath not implemented");
	protected virtual void CalculateNetworkInstallTaskRequiredSize(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.CalculateNetworkInstallTaskRequiredSize not implemented");
	protected virtual void IsExFatDriverIncluded(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.IsExFatDriverIncluded not implemented");
	protected virtual void GetBackgroundDownloadStressTaskInfo() =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.GetBackgroundDownloadStressTaskInfo not implemented");
	protected virtual void RequestDeviceAuthenticationToken() =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestDeviceAuthenticationToken not implemented");
	protected virtual void RequestGameCardRegistrationStatus(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestGameCardRegistrationStatus not implemented");
	protected virtual void RequestRegisterGameCard(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestRegisterGameCard not implemented");
	protected virtual void RequestRegisterNotificationToken(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestRegisterNotificationToken not implemented");
	protected virtual void RequestDownloadTaskList(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestDownloadTaskList not implemented");
	protected virtual void RequestApplicationControl(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestApplicationControl not implemented");
	protected virtual void RequestLatestApplicationControl(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestLatestApplicationControl not implemented");
	protected virtual void RequestVersionList(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestVersionList not implemented");
	protected virtual void CreateApplyDeltaTask(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.CreateApplyDeltaTask not implemented");
	protected virtual void DestroyApplyDeltaTask(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.INetworkInstallManager.DestroyApplyDeltaTask");
	protected virtual void ListApplicationApplyDeltaTask(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.ListApplicationApplyDeltaTask not implemented");
	protected virtual void RequestApplyDeltaTaskRun(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.RequestApplyDeltaTaskRun not implemented");
	protected virtual void GetApplyDeltaTaskInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.GetApplyDeltaTaskInfo not implemented");
	protected virtual void ListApplyDeltaTask(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.ListApplyDeltaTask not implemented");
	protected virtual void CommitApplyDeltaTask(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.INetworkInstallManager.CommitApplyDeltaTask");
	protected virtual void CalculateApplyDeltaTaskRequiredSize(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.CalculateApplyDeltaTaskRequiredSize not implemented");
	protected virtual void PrepareShutdown() =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.INetworkInstallManager.PrepareShutdown");
	protected virtual void ListApplyDeltaTask2() =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.ListApplyDeltaTask2 not implemented");
	protected virtual void ClearNotEnoughSpaceStateOfApplyDeltaTask(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.INetworkInstallManager.ClearNotEnoughSpaceStateOfApplyDeltaTask");
	protected virtual void Unknown42(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.Unknown42 not implemented");
	protected virtual void Unknown43() =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.Unknown43 not implemented");
	protected virtual void Unknown44(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.Unknown44 not implemented");
	protected virtual void Unknown45(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.INetworkInstallManager.Unknown45 not implemented");
	protected virtual void Unknown46() =>
		Console.WriteLine("Stub hit for Nn.Nim.Detail.INetworkInstallManager.Unknown46");
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
	protected virtual void RequestDeviceAuthenticationToken() =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestDeviceAuthenticationToken not implemented");
	protected virtual void RequestCachedDeviceAuthenticationToken(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestCachedDeviceAuthenticationToken not implemented");
	protected virtual void RequestRegisterDeviceAccount() =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestRegisterDeviceAccount not implemented");
	protected virtual void RequestUnregisterDeviceAccount() =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestUnregisterDeviceAccount not implemented");
	protected virtual void RequestDeviceAccountStatus() =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestDeviceAccountStatus not implemented");
	protected virtual void GetDeviceAccountInfo() =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.GetDeviceAccountInfo not implemented");
	protected virtual void RequestDeviceRegistrationInfo() =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestDeviceRegistrationInfo not implemented");
	protected virtual void RequestTransferDeviceAccount() =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestTransferDeviceAccount not implemented");
	protected virtual void RequestSyncRegistration() =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestSyncRegistration not implemented");
	protected virtual void IsOwnDeviceId(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.IsOwnDeviceId not implemented");
	protected virtual void RequestRegisterNotificationToken(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestRegisterNotificationToken not implemented");
	protected virtual void RequestUnlinkDevice(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestUnlinkDevice not implemented");
	protected virtual void RequestUnlinkDeviceIntegrated(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestUnlinkDeviceIntegrated not implemented");
	protected virtual void RequestLinkDevice(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestLinkDevice not implemented");
	protected virtual void HasDeviceLink(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.HasDeviceLink not implemented");
	protected virtual void RequestUnlinkDeviceAll() =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestUnlinkDeviceAll not implemented");
	protected virtual void RequestCreateVirtualAccount(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestCreateVirtualAccount not implemented");
	protected virtual void RequestDeviceLinkStatus(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestDeviceLinkStatus not implemented");
	protected virtual void GetAccountByVirtualAccount(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.GetAccountByVirtualAccount not implemented");
	protected virtual void RequestSyncTicket() =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestSyncTicket not implemented");
	protected virtual void RequestDownloadTicket(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestDownloadTicket not implemented");
	protected virtual void RequestDownloadTicketForPrepurchasedContents(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nim.Detail.IShopServiceManager.RequestDownloadTicketForPrepurchasedContents not implemented");
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

