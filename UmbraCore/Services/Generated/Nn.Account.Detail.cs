using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Account.Detail;
public partial class IAsyncContext : _IAsyncContext_Base;
public abstract class _IAsyncContext_Base : IpcInterface {
	protected virtual KObject GetSystemEvent() =>
		throw new NotImplementedException("Nn.Account.Detail.IAsyncContext.GetSystemEvent not implemented");
	protected virtual void Cancel() =>
		"Stub hit for Nn.Account.Detail.IAsyncContext.Cancel".Log();
	protected virtual byte HasDone() =>
		throw new NotImplementedException("Nn.Account.Detail.IAsyncContext.HasDone not implemented");
	protected virtual void GetResult() =>
		"Stub hit for Nn.Account.Detail.IAsyncContext.GetResult".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetSystemEvent
				var _return = GetSystemEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // Cancel
				Cancel();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // HasDone
				var _return = HasDone();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // GetResult
				GetResult();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Detail.IAsyncContext");
		}
	}
}

public partial class INotifier : _INotifier_Base;
public abstract class _INotifier_Base : IpcInterface {
	protected virtual KObject GetSystemEvent() =>
		throw new NotImplementedException("Nn.Account.Detail.INotifier.GetSystemEvent not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetSystemEvent
				var _return = GetSystemEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Detail.INotifier");
		}
	}
}

public partial class ISessionObject : _ISessionObject_Base;
public abstract class _ISessionObject_Base : IpcInterface {
	protected virtual void Dummy() =>
		"Stub hit for Nn.Account.Detail.ISessionObject.Dummy".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x3E7: { // Dummy
				Dummy();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Detail.ISessionObject");
		}
	}
}

