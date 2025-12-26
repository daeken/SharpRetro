using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Migration.User;
public partial class IClient : _IClient_Base;
public abstract class _IClient_Base : IpcInterface {
	protected virtual void GetClientProfile(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Migration.User.IClient.GetClientProfile not implemented");
	protected virtual void CreateLoginSession(out byte[] _0) =>
		throw new NotImplementedException("Nn.Migration.User.IClient.CreateLoginSession not implemented");
	protected virtual void GetNetworkServiceAccountId(out byte[] _0) =>
		throw new NotImplementedException("Nn.Migration.User.IClient.GetNetworkServiceAccountId not implemented");
	protected virtual void GetUserNickname(out byte[] _0) =>
		throw new NotImplementedException("Nn.Migration.User.IClient.GetUserNickname not implemented");
	protected virtual void GetUserProfileImage(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Migration.User.IClient.GetUserProfileImage not implemented");
	protected virtual Nn.Migration.Detail.IAsyncContext PrepareAsync() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.PrepareAsync not implemented");
	protected virtual void GetConnectionRequirement(out byte[] _0) =>
		throw new NotImplementedException("Nn.Migration.User.IClient.GetConnectionRequirement not implemented");
	protected virtual Nn.Migration.Detail.IAsyncContext ScanServersAsync() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.ScanServersAsync not implemented");
	protected virtual void ListServers(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Migration.User.IClient.ListServers not implemented");
	protected virtual Nn.Migration.Detail.IAsyncContext ConnectByServerIdAsync(byte[] _0) =>
		throw new NotImplementedException("Nn.Migration.User.IClient.ConnectByServerIdAsync not implemented");
	protected virtual void GetStorageShortfall(out byte[] _0) =>
		throw new NotImplementedException("Nn.Migration.User.IClient.GetStorageShortfall not implemented");
	protected virtual void GetTotalTransferInfo(out byte[] _0) =>
		throw new NotImplementedException("Nn.Migration.User.IClient.GetTotalTransferInfo not implemented");
	protected virtual void GetImmigrantUid(out byte[] _0) =>
		throw new NotImplementedException("Nn.Migration.User.IClient.GetImmigrantUid not implemented");
	protected virtual void GetCurrentTransferInfo(out byte[] _0) =>
		throw new NotImplementedException("Nn.Migration.User.IClient.GetCurrentTransferInfo not implemented");
	protected virtual void GetCurrentRelatedApplications(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Migration.User.IClient.GetCurrentRelatedApplications not implemented");
	protected virtual Nn.Migration.Detail.IAsyncContext TransferNextAsync() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.TransferNextAsync not implemented");
	protected virtual Nn.Migration.Detail.IAsyncContext SuspendAsync() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.SuspendAsync not implemented");
	protected virtual Nn.Migration.Detail.IAsyncContext CompleteAsync() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.CompleteAsync not implemented");
	protected virtual void Abort() =>
		"Stub hit for Nn.Migration.User.IClient.Abort".Log();
	protected virtual Nn.Migration.Detail.IAsyncContext DebugSynchronizeStateInFinalizationAsync() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.DebugSynchronizeStateInFinalizationAsync not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetClientProfile
				GetClientProfile(im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // CreateLoginSession
				CreateLoginSession(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0xB: { // GetNetworkServiceAccountId
				GetNetworkServiceAccountId(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC: { // GetUserNickname
				GetUserNickname(out var _0);
				om.Initialize(0, 0, 33);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD: { // GetUserProfileImage
				GetUserProfileImage(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x64: { // PrepareAsync
				var _return = PrepareAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x65: { // GetConnectionRequirement
				GetConnectionRequirement(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC8: { // ScanServersAsync
				var _return = ScanServersAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xC9: { // ListServers
				ListServers(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD2: { // ConnectByServerIdAsync
				var _return = ConnectByServerIdAsync(im.GetBytes(8, 0x10));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x12C: { // GetStorageShortfall
				GetStorageShortfall(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x12D: { // GetTotalTransferInfo
				GetTotalTransferInfo(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x12E: { // GetImmigrantUid
				GetImmigrantUid(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x136: { // GetCurrentTransferInfo
				GetCurrentTransferInfo(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x137: { // GetCurrentRelatedApplications
				GetCurrentRelatedApplications(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x140: { // TransferNextAsync
				var _return = TransferNextAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x15E: { // SuspendAsync
				var _return = SuspendAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x190: { // CompleteAsync
				var _return = CompleteAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F4: { // Abort
				Abort();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3E7: { // DebugSynchronizeStateInFinalizationAsync
				var _return = DebugSynchronizeStateInFinalizationAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Migration.User.IClient");
		}
	}
}

public partial class IServer : _IServer_Base;
public abstract class _IServer_Base : IpcInterface {
	protected virtual void GetUid(out byte[] _0) =>
		throw new NotImplementedException("Nn.Migration.User.IServer.GetUid not implemented");
	protected virtual void GetServerProfile(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Migration.User.IServer.GetServerProfile not implemented");
	protected virtual Nn.Migration.Detail.IAsyncContext PrepareAsync() =>
		throw new NotImplementedException("Nn.Migration.User.IServer.PrepareAsync not implemented");
	protected virtual void GetConnectionRequirement(out byte[] _0) =>
		throw new NotImplementedException("Nn.Migration.User.IServer.GetConnectionRequirement not implemented");
	protected virtual Nn.Migration.Detail.IAsyncContext WaitConnectionAsync() =>
		throw new NotImplementedException("Nn.Migration.User.IServer.WaitConnectionAsync not implemented");
	protected virtual void GetClientProfile(Span<byte> _0) =>
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
		"Stub hit for Nn.Migration.User.IServer.Abort".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetUid
				GetUid(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // GetServerProfile
				GetServerProfile(im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x64: { // PrepareAsync
				var _return = PrepareAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x65: { // GetConnectionRequirement
				GetConnectionRequirement(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC8: { // WaitConnectionAsync
				var _return = WaitConnectionAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xC9: { // GetClientProfile
				GetClientProfile(im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCA: { // AcceptConnectionAsync
				var _return = AcceptConnectionAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xCB: { // DeclineConnectionAsync
				var _return = DeclineConnectionAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x12C: { // ProcessTransferAsync
				var _return = ProcessTransferAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x190: { // CompleteAsync
				var _return = CompleteAsync();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F4: { // Abort
				Abort();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Migration.User.IServer");
		}
	}
}

public partial class IService : _IService_Base {
	public readonly string ServiceName;
	public IService(string serviceName) => ServiceName = serviceName;
}
public abstract class _IService_Base : IpcInterface {
	protected virtual void TryGetLastMigrationInfo(out byte[] _0) =>
		throw new NotImplementedException("Nn.Migration.User.IService.TryGetLastMigrationInfo not implemented");
	protected virtual Nn.Migration.User.IServer CreateServer(byte[] _0, KObject _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Migration.User.IService.CreateServer not implemented");
	protected virtual Nn.Migration.User.IServer ResumeServer(byte[] _0, KObject _1) =>
		throw new NotImplementedException("Nn.Migration.User.IService.ResumeServer not implemented");
	protected virtual Nn.Migration.User.IClient CreateClient(byte[] _0, KObject _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Migration.User.IService.CreateClient not implemented");
	protected virtual Nn.Migration.User.IClient ResumeClient(byte[] _0, KObject _1) =>
		throw new NotImplementedException("Nn.Migration.User.IService.ResumeClient not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0xA: { // TryGetLastMigrationInfo
				TryGetLastMigrationInfo(out var _0);
				om.Initialize(0, 0, 12);
				om.SetBytes(8, _0);
				break;
			}
			case 0x64: { // CreateServer
				var _return = CreateServer(im.GetBytes(8, 0x18), Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<byte>(0x19, 0));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x65: { // ResumeServer
				var _return = ResumeServer(im.GetBytes(8, 0x4), Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xC8: { // CreateClient
				var _return = CreateClient(im.GetBytes(8, 0x4), Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<byte>(0x19, 0));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xC9: { // ResumeClient
				var _return = ResumeClient(im.GetBytes(8, 0x4), Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Migration.User.IService");
		}
	}
}

public partial class IAsyncContext : _IAsyncContext_Base;
public abstract class _IAsyncContext_Base : IpcInterface {
	protected virtual void GetSystemEvent() =>
		"Stub hit for Nn.Migration.User.IAsyncContext.GetSystemEvent".Log();
	protected virtual void Cancel() =>
		"Stub hit for Nn.Migration.User.IAsyncContext.Cancel".Log();
	protected virtual void HasDone() =>
		"Stub hit for Nn.Migration.User.IAsyncContext.HasDone".Log();
	protected virtual void GetResult() =>
		"Stub hit for Nn.Migration.User.IAsyncContext.GetResult".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetSystemEvent
				GetSystemEvent();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Cancel
				Cancel();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // HasDone
				HasDone();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // GetResult
				GetResult();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Migration.User.IAsyncContext");
		}
	}
}

