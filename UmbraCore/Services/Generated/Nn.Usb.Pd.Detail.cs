using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Usb.Pd.Detail;
public partial class IPdCradleManager : _IPdCradleManager_Base;
public abstract class _IPdCradleManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetPdCradleSession
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Pd.Detail.IPdCradleManager");
		}
	}
}

public partial class IPdCradleSession : _IPdCradleSession_Base;
public abstract class _IPdCradleSession_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // VdmUserWrite
				break;
			case 0x1: // VdmUserRead
				break;
			case 0x2: // Vdm20Init
				break;
			case 0x3: // GetFwType
				break;
			case 0x4: // GetFwRevision
				break;
			case 0x5: // GetManufacturerId
				break;
			case 0x6: // GetDeviceId
				break;
			case 0x7: // Unknown7
				break;
			case 0x8: // Unknown8
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Pd.Detail.IPdCradleSession");
		}
	}
}

public partial class IPdManager : _IPdManager_Base;
public abstract class _IPdManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetPdSession
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Pd.Detail.IPdManager");
		}
	}
}

public partial class IPdManufactureManager : _IPdManufactureManager_Base;
public abstract class _IPdManufactureManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Pd.Detail.IPdManufactureManager");
		}
	}
}

public partial class IPdManufactureSession : _IPdManufactureSession_Base;
public abstract class _IPdManufactureSession_Base : IpcInterface {
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
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Pd.Detail.IPdManufactureSession");
		}
	}
}

public partial class IPdSession : _IPdSession_Base;
public abstract class _IPdSession_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // BindNoticeEvent
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // GetStatus
				break;
			case 0x3: // GetNotice
				break;
			case 0x4: // Unknown4
				break;
			case 0x5: // Unknown5
				break;
			case 0x6: // ReplyPowerRequest
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Pd.Detail.IPdSession");
		}
	}
}

