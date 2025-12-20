using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Usb.Hs;
public partial class IClientEpSession : _IClientEpSession_Base;
public abstract class _IClientEpSession_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // PostBufferAsync
				break;
			case 0x5: // Unknown5
				break;
			case 0x6: // Unknown6
				break;
			case 0x7: // Unknown7
				break;
			case 0x8: // Unknown8
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Hs.IClientEpSession");
		}
	}
}

public partial class IClientIfSession : _IClientIfSession_Base;
public abstract class _IClientIfSession_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			case 0x5: // CtrlXferAsync
				break;
			case 0x6: // Unknown6
				break;
			case 0x7: // GetCtrlXferReport
				break;
			case 0x8: // Unknown8
				break;
			case 0x9: // GetClientEpSession
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Hs.IClientIfSession");
		}
	}
}

public partial class IClientRootSession : _IClientRootSession_Base;
public abstract class _IClientRootSession_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // BindClientProcess
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			case 0x5: // Unknown5
				break;
			case 0x6: // GetInterfaceStateChangeEvent
				break;
			case 0x7: // GetClientIfSession
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Hs.IClientRootSession");
		}
	}
}

