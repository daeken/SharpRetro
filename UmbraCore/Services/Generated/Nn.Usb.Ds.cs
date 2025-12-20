using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Usb.Ds;
public partial class IDsEndpoint : _IDsEndpoint_Base;
public abstract class _IDsEndpoint_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // PostBufferAsync
				break;
			case 0x1: // Cancel
				break;
			case 0x2: // GetCompletionEvent
				break;
			case 0x3: // GetReportData
				break;
			case 0x4: // Stall
				break;
			case 0x5: // SetZlt
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Ds.IDsEndpoint");
		}
	}
}

public partial class IDsInterface : _IDsInterface_Base;
public abstract class _IDsInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // RegisterEndpoint
				break;
			case 0x1: // GetSetupEvent
				break;
			case 0x2: // GetSetupPacket
				break;
			case 0x3: // EnableInterface
				break;
			case 0x4: // DisableInterface
				break;
			case 0x5: // CtrlInPostBufferAsync
				break;
			case 0x6: // CtrlOutPostBufferAsync
				break;
			case 0x7: // GetCtrlInCompletionEvent
				break;
			case 0x8: // GetCtrlInReportData
				break;
			case 0x9: // GetCtrlOutCompletionEvent
				break;
			case 0xA: // GetCtrlOutReportData
				break;
			case 0xB: // StallCtrl
				break;
			case 0xC: // AppendConfigurationData
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Ds.IDsInterface");
		}
	}
}

public partial class IDsService : _IDsService_Base;
public abstract class _IDsService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // BindDevice
				break;
			case 0x1: // BindClientProcess
				break;
			case 0x2: // RegisterInterface
				break;
			case 0x3: // GetStateChangeEvent
				break;
			case 0x4: // GetState
				break;
			case 0x5: // ClearDeviceData
				break;
			case 0x6: // AddUsbStringDescriptor
				break;
			case 0x7: // DeleteUsbStringDescriptor
				break;
			case 0x8: // SetUsbDeviceDescriptor
				break;
			case 0x9: // SetBinaryObjectStore
				break;
			case 0xA: // Enable
				break;
			case 0xB: // Disable
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Ds.IDsService");
		}
	}
}

