using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Account.Detail;
public partial class IAsyncContext : _IAsyncContext_Base;
public abstract class _IAsyncContext_Base : IpcInterface {
	protected virtual KObject GetSystemEvent() =>
		throw new NotImplementedException("Nn.Account.Detail.IAsyncContext.GetSystemEvent not implemented");
	protected virtual void Cancel() =>
		Console.WriteLine("Stub hit for Nn.Account.Detail.IAsyncContext.Cancel");
	protected virtual byte HasDone() =>
		throw new NotImplementedException("Nn.Account.Detail.IAsyncContext.HasDone not implemented");
	protected virtual void GetResult() =>
		Console.WriteLine("Stub hit for Nn.Account.Detail.IAsyncContext.GetResult");
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
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Detail.IAsyncContext");
		}
	}
}

public partial class INotifier : _INotifier_Base;
public abstract class _INotifier_Base : IpcInterface {
	protected virtual KObject GetSystemEvent() =>
		throw new NotImplementedException("Nn.Account.Detail.INotifier.GetSystemEvent not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetSystemEvent
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Detail.INotifier");
		}
	}
}

public partial class ISessionObject : _ISessionObject_Base;
public abstract class _ISessionObject_Base : IpcInterface {
	protected virtual void Dummy() =>
		Console.WriteLine("Stub hit for Nn.Account.Detail.ISessionObject.Dummy");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x3E7: // Dummy
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Detail.ISessionObject");
		}
	}
}

