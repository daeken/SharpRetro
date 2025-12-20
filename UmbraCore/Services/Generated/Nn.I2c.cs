using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.I2c;
public partial class IManager : _IManager_Base;
public abstract class _IManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // OpenSessionForDev
				break;
			case 0x1: // OpenSession
				break;
			case 0x2: // HasDevice
				break;
			case 0x3: // HasDeviceForDev
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.I2c.IManager");
		}
	}
}

public partial class ISession : _ISession_Base;
public abstract class _ISession_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Send
				break;
			case 0x1: // Receive
				break;
			case 0x2: // ExecuteCommandList
				break;
			case 0xA: // SendAuto
				break;
			case 0xB: // ReceiveAuto
				break;
			case 0xC: // ExecuteCommandListAuto
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.I2c.ISession");
		}
	}
}

