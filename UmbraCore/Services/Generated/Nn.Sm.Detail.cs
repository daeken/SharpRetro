using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Sm.Detail;
public partial class IManagerInterface : _IManagerInterface_Base;
public abstract class _IManagerInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // RegisterProcess
				break;
			case 0x1: // UnregisterProcess
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Sm.Detail.IManagerInterface");
		}
	}
}

public partial class IUserInterface : _IUserInterface_Base;
public abstract class _IUserInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Initialize
				break;
			case 0x1: // GetService
				break;
			case 0x2: // RegisterService
				break;
			case 0x3: // UnregisterService
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Sm.Detail.IUserInterface");
		}
	}
}

