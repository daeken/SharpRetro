using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Htc.Tenv;
public partial class IService : _IService_Base;
public abstract class _IService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetVariable
				break;
			case 0x1: // GetVariableLength
				break;
			case 0x2: // WaitUntilVariableAvailable
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Htc.Tenv.IService");
		}
	}
}

public partial class IServiceManager : _IServiceManager_Base;
public abstract class _IServiceManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetServiceInterface
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Htc.Tenv.IServiceManager");
		}
	}
}

