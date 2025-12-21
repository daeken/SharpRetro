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
		Console.WriteLine("Stub hit for Nn.Migration.User.IClient.Abort");
	protected virtual Nn.Migration.Detail.IAsyncContext DebugSynchronizeStateInFinalizationAsync() =>
		throw new NotImplementedException("Nn.Migration.User.IClient.DebugSynchronizeStateInFinalizationAsync not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetClientProfile
				om.Initialize(0, 0, 0);
				GetClientProfile(im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0xA: { // CreateLoginSession
				om.Initialize(0, 0, 16);
				CreateLoginSession(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xB: { // GetNetworkServiceAccountId
				om.Initialize(0, 0, 8);
				GetNetworkServiceAccountId(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC: { // GetUserNickname
				om.Initialize(0, 0, 33);
				GetUserNickname(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD: { // GetUserProfileImage
				om.Initialize(0, 0, 4);
				GetUserProfileImage(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x64: { // PrepareAsync
				om.Initialize(1, 0, 0);
				var _return = PrepareAsync();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x65: { // GetConnectionRequirement
				om.Initialize(0, 0, 1);
				GetConnectionRequirement(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC8: { // ScanServersAsync
				om.Initialize(1, 0, 0);
				var _return = ScanServersAsync();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xC9: { // ListServers
				om.Initialize(0, 0, 4);
				ListServers(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0xD2: { // ConnectByServerIdAsync
				om.Initialize(1, 0, 0);
				var _return = ConnectByServerIdAsync(im.GetBytes(8, 0x10));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x12C: { // GetStorageShortfall
				om.Initialize(0, 0, 8);
				GetStorageShortfall(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x12D: { // GetTotalTransferInfo
				om.Initialize(0, 0, 16);
				GetTotalTransferInfo(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x12E: { // GetImmigrantUid
				om.Initialize(0, 0, 16);
				GetImmigrantUid(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x136: { // GetCurrentTransferInfo
				om.Initialize(0, 0, 16);
				GetCurrentTransferInfo(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x137: { // GetCurrentRelatedApplications
				om.Initialize(0, 0, 4);
				GetCurrentRelatedApplications(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x140: { // TransferNextAsync
				om.Initialize(1, 0, 0);
				var _return = TransferNextAsync();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x15E: { // SuspendAsync
				om.Initialize(1, 0, 0);
				var _return = SuspendAsync();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x190: { // CompleteAsync
				om.Initialize(1, 0, 0);
				var _return = CompleteAsync();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F4: { // Abort
				om.Initialize(0, 0, 0);
				Abort();
				break;
			}
			case 0x3E7: { // DebugSynchronizeStateInFinalizationAsync
				om.Initialize(1, 0, 0);
				var _return = DebugSynchronizeStateInFinalizationAsync();
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
		Console.WriteLine("Stub hit for Nn.Migration.User.IServer.Abort");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetUid
				om.Initialize(0, 0, 16);
				GetUid(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // GetServerProfile
				om.Initialize(0, 0, 0);
				GetServerProfile(im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x64: { // PrepareAsync
				om.Initialize(1, 0, 0);
				var _return = PrepareAsync();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x65: { // GetConnectionRequirement
				om.Initialize(0, 0, 1);
				GetConnectionRequirement(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC8: { // WaitConnectionAsync
				om.Initialize(1, 0, 0);
				var _return = WaitConnectionAsync();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xC9: { // GetClientProfile
				om.Initialize(0, 0, 0);
				GetClientProfile(im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0xCA: { // AcceptConnectionAsync
				om.Initialize(1, 0, 0);
				var _return = AcceptConnectionAsync();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xCB: { // DeclineConnectionAsync
				om.Initialize(1, 0, 0);
				var _return = DeclineConnectionAsync();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x12C: { // ProcessTransferAsync
				om.Initialize(1, 0, 0);
				var _return = ProcessTransferAsync();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x190: { // CompleteAsync
				om.Initialize(1, 0, 0);
				var _return = CompleteAsync();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1F4: { // Abort
				om.Initialize(0, 0, 0);
				Abort();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Migration.User.IServer");
		}
	}
}

public partial class IService : _IService_Base;
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
				om.Initialize(0, 0, 12);
				TryGetLastMigrationInfo(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x64: { // CreateServer
				om.Initialize(1, 0, 0);
				var _return = CreateServer(im.GetBytes(8, 0x18), Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<byte>(0x19, 0));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x65: { // ResumeServer
				om.Initialize(1, 0, 0);
				var _return = ResumeServer(im.GetBytes(8, 0x4), Kernel.Get<KObject>(im.GetCopy(0)));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xC8: { // CreateClient
				om.Initialize(1, 0, 0);
				var _return = CreateClient(im.GetBytes(8, 0x4), Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<byte>(0x19, 0));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xC9: { // ResumeClient
				om.Initialize(1, 0, 0);
				var _return = ResumeClient(im.GetBytes(8, 0x4), Kernel.Get<KObject>(im.GetCopy(0)));
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
		Console.WriteLine("Stub hit for Nn.Migration.User.IAsyncContext.GetSystemEvent");
	protected virtual void Cancel() =>
		Console.WriteLine("Stub hit for Nn.Migration.User.IAsyncContext.Cancel");
	protected virtual void HasDone() =>
		Console.WriteLine("Stub hit for Nn.Migration.User.IAsyncContext.HasDone");
	protected virtual void GetResult() =>
		Console.WriteLine("Stub hit for Nn.Migration.User.IAsyncContext.GetResult");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetSystemEvent
				om.Initialize(0, 0, 0);
				GetSystemEvent();
				break;
			}
			case 0x1: { // Cancel
				om.Initialize(0, 0, 0);
				Cancel();
				break;
			}
			case 0x2: { // HasDone
				om.Initialize(0, 0, 0);
				HasDone();
				break;
			}
			case 0x3: { // GetResult
				om.Initialize(0, 0, 0);
				GetResult();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Migration.User.IAsyncContext");
		}
	}
}

