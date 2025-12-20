using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Fatalsrv;
public partial class IPrivateService : _IPrivateService_Base;
public abstract class _IPrivateService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetFatalEvent
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fatalsrv.IPrivateService");
		}
	}
}

public partial class IService : _IService_Base;
public abstract class _IService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // ThrowFatal
				break;
			case 0x1: // ThrowFatalWithPolicy
				break;
			case 0x2: // ThrowFatalWithCpuContext
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fatalsrv.IService");
		}
	}
}

