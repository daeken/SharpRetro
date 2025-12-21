using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Migration.User;
public partial class IClient : _IClient_Base;
public abstract class _IClient_Base : IpcInterface {
	protected virtual void GetClientProfile() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.GetClientProfile not implemented");
	protected virtual void CreateLoginSession() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.CreateLoginSession not implemented");
	protected virtual void GetNetworkServiceAccountId() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.GetNetworkServiceAccountId not implemented");
	protected virtual void GetUserNickname() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.GetUserNickname not implemented");
	protected virtual void GetUserProfileImage() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.GetUserProfileImage not implemented");
	protected virtual Nn.Migration.Detail.IAsyncContext PrepareAsync() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.PrepareAsync not implemented");
	protected virtual void GetConnectionRequirement() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.GetConnectionRequirement not implemented");
	protected virtual Nn.Migration.Detail.IAsyncContext ScanServersAsync() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.ScanServersAsync not implemented");
	protected virtual void ListServers() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.ListServers not implemented");
	protected virtual Nn.Migration.Detail.IAsyncContext ConnectByServerIdAsync(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Migration.User.IClient.ConnectByServerIdAsync not implemented");
	protected virtual void GetStorageShortfall() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.GetStorageShortfall not implemented");
	protected virtual void GetTotalTransferInfo() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.GetTotalTransferInfo not implemented");
	protected virtual void GetImmigrantUid() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.GetImmigrantUid not implemented");
	protected virtual void GetCurrentTransferInfo() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.GetCurrentTransferInfo not implemented");
	protected virtual void GetCurrentRelatedApplications() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.GetCurrentRelatedApplications not implemented");
	protected virtual Nn.Migration.Detail.IAsyncContext TransferNextAsync() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.TransferNextAsync not implemented");
	protected virtual Nn.Migration.Detail.IAsyncContext SuspendAsync() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.SuspendAsync not implemented");
	protected virtual Nn.Migration.Detail.IAsyncContext CompleteAsync() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.CompleteAsync not implemented");
	protected virtual void Abort() =>
		Console.WriteLine("Stub hit for Nn.Migration.User.IClient.Abort");
	protected virtual Nn.Migration.Detail.IAsyncContext DebugSynchronizeStateInFinalizationAsync() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.DebugSynchronizeStateInFinalizationAsync not implemented");
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
	protected virtual void GetUid() =>
		throw new NotImplementedException("Nn.Migration.User.IServer.GetUid not implemented");
	protected virtual void GetServerProfile() =>
		throw new NotImplementedException("Nn.Migration.User.IServer.GetServerProfile not implemented");
	protected virtual Nn.Migration.Detail.IAsyncContext PrepareAsync() =>
		throw new NotImplementedException("Nn.Migration.User.IServer.PrepareAsync not implemented");
	protected virtual void GetConnectionRequirement() =>
		throw new NotImplementedException("Nn.Migration.User.IServer.GetConnectionRequirement not implemented");
	protected virtual Nn.Migration.Detail.IAsyncContext WaitConnectionAsync() =>
		throw new NotImplementedException("Nn.Migration.User.IServer.WaitConnectionAsync not implemented");
	protected virtual void GetClientProfile() =>
		throw new NotImplementedException("Nn.Migration.User.IServer.GetClientProfile not implemented");
	protected virtual Nn.Migration.Detail.IAsyncContext AcceptConnectionAsync() =>
		throw new NotImplementedException("Nn.Migration.User.IServer.AcceptConnectionAsync not implemented");
	protected virtual Nn.Migration.Detail.IAsyncContext DeclineConnectionAsync() =>
		throw new NotImplementedException("Nn.Migration.User.IServer.DeclineConnectionAsync not implemented");
	protected virtual Nn.Migration.Detail.IAsyncContext ProcessTransferAsync() =>
		throw new NotImplementedException("Nn.Migration.User.IServer.ProcessTransferAsync not implemented");
	protected virtual Nn.Migration.Detail.IAsyncContext CompleteAsync() =>
		throw new NotImplementedException("Nn.Migration.User.IServer.CompleteAsync not implemented");
	protected virtual void Abort() =>
		Console.WriteLine("Stub hit for Nn.Migration.User.IServer.Abort");
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
	protected virtual void TryGetLastMigrationInfo() =>
		throw new NotImplementedException("Nn.Migration.User.IService.TryGetLastMigrationInfo not implemented");
	protected virtual Nn.Migration.User.IServer CreateServer(Span<byte> _0, KObject _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Migration.User.IService.CreateServer not implemented");
	protected virtual Nn.Migration.User.IServer ResumeServer(Span<byte> _0, KObject _1) =>
		throw new NotImplementedException("Nn.Migration.User.IService.ResumeServer not implemented");
	protected virtual Nn.Migration.User.IClient CreateClient(Span<byte> _0, KObject _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Migration.User.IService.CreateClient not implemented");
	protected virtual Nn.Migration.User.IClient ResumeClient(Span<byte> _0, KObject _1) =>
		throw new NotImplementedException("Nn.Migration.User.IService.ResumeClient not implemented");
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
	protected virtual void GetSystemEvent() =>
		Console.WriteLine("Stub hit for Nn.Migration.User.IAsyncContext.GetSystemEvent");
	protected virtual void Cancel() =>
		Console.WriteLine("Stub hit for Nn.Migration.User.IAsyncContext.Cancel");
	protected virtual void HasDone() =>
		Console.WriteLine("Stub hit for Nn.Migration.User.IAsyncContext.HasDone");
	protected virtual void GetResult() =>
		Console.WriteLine("Stub hit for Nn.Migration.User.IAsyncContext.GetResult");
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

