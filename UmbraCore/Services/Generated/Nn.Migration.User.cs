using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Migration.User;
public partial class IClient : _IClient_Base;
public abstract class _IClient_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetClientProfile
				break;
			case 0xA: // CreateLoginSession
				break;
			case 0xB: // GetNetworkServiceAccountId
				break;
			case 0xC: // GetUserNickname
				break;
			case 0xD: // GetUserProfileImage
				break;
			case 0x64: // PrepareAsync
				break;
			case 0x65: // GetConnectionRequirement
				break;
			case 0xC8: // ScanServersAsync
				break;
			case 0xC9: // ListServers
				break;
			case 0xD2: // ConnectByServerIdAsync
				break;
			case 0x12C: // GetStorageShortfall
				break;
			case 0x12D: // GetTotalTransferInfo
				break;
			case 0x12E: // GetImmigrantUid
				break;
			case 0x136: // GetCurrentTransferInfo
				break;
			case 0x137: // GetCurrentRelatedApplications
				break;
			case 0x140: // TransferNextAsync
				break;
			case 0x15E: // SuspendAsync
				break;
			case 0x190: // CompleteAsync
				break;
			case 0x1F4: // Abort
				break;
			case 0x3E7: // DebugSynchronizeStateInFinalizationAsync
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Migration.User.IClient");
		}
	}
}

public partial class IServer : _IServer_Base;
public abstract class _IServer_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetUid
				break;
			case 0x1: // GetServerProfile
				break;
			case 0x64: // PrepareAsync
				break;
			case 0x65: // GetConnectionRequirement
				break;
			case 0xC8: // WaitConnectionAsync
				break;
			case 0xC9: // GetClientProfile
				break;
			case 0xCA: // AcceptConnectionAsync
				break;
			case 0xCB: // DeclineConnectionAsync
				break;
			case 0x12C: // ProcessTransferAsync
				break;
			case 0x190: // CompleteAsync
				break;
			case 0x1F4: // Abort
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Migration.User.IServer");
		}
	}
}

public partial class IService : _IService_Base;
public abstract class _IService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0xA: // TryGetLastMigrationInfo
				break;
			case 0x64: // CreateServer
				break;
			case 0x65: // ResumeServer
				break;
			case 0xC8: // CreateClient
				break;
			case 0xC9: // ResumeClient
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Migration.User.IService");
		}
	}
}

public partial class IAsyncContext : _IAsyncContext_Base;
public abstract class _IAsyncContext_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetSystemEvent
				break;
			case 0x1: // Cancel
				break;
			case 0x2: // HasDone
				break;
			case 0x3: // GetResult
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Migration.User.IAsyncContext");
		}
	}
}

