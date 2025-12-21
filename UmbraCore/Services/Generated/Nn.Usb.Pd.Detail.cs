using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Usb.Pd.Detail;
public partial class IPdCradleManager : _IPdCradleManager_Base;
public abstract class _IPdCradleManager_Base : IpcInterface {
	protected virtual Nn.Usb.Pd.Detail.IPdCradleSession GetPdCradleSession() =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdCradleManager.GetPdCradleSession not implemented");
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
	protected virtual void VdmUserWrite(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Usb.Pd.Detail.IPdCradleSession.VdmUserWrite");
	protected virtual void VdmUserRead(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdCradleSession.VdmUserRead not implemented");
	protected virtual void Vdm20Init() =>
		Console.WriteLine("Stub hit for Nn.Usb.Pd.Detail.IPdCradleSession.Vdm20Init");
	protected virtual void GetFwType() =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdCradleSession.GetFwType not implemented");
	protected virtual void GetFwRevision() =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdCradleSession.GetFwRevision not implemented");
	protected virtual void GetManufacturerId() =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdCradleSession.GetManufacturerId not implemented");
	protected virtual void GetDeviceId() =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdCradleSession.GetDeviceId not implemented");
	protected virtual void Unknown7() =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdCradleSession.Unknown7 not implemented");
	protected virtual void Unknown8() =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdCradleSession.Unknown8 not implemented");
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
	protected virtual Nn.Usb.Pd.Detail.IPdSession GetPdSession() =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdManager.GetPdSession not implemented");
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
	protected virtual Nn.Usb.Pd.Detail.IPdManufactureSession Unknown0() =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdManufactureManager.Unknown0 not implemented");
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
	protected virtual void Unknown0() =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdManufactureSession.Unknown0 not implemented");
	protected virtual void Unknown1() =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdManufactureSession.Unknown1 not implemented");
	protected virtual void Unknown2() =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdManufactureSession.Unknown2 not implemented");
	protected virtual void Unknown3() =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdManufactureSession.Unknown3 not implemented");
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
	protected virtual KObject BindNoticeEvent() =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdSession.BindNoticeEvent not implemented");
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Usb.Pd.Detail.IPdSession.Unknown1");
	protected virtual void GetStatus() =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdSession.GetStatus not implemented");
	protected virtual void GetNotice() =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdSession.GetNotice not implemented");
	protected virtual void Unknown4() =>
		Console.WriteLine("Stub hit for Nn.Usb.Pd.Detail.IPdSession.Unknown4");
	protected virtual void Unknown5() =>
		Console.WriteLine("Stub hit for Nn.Usb.Pd.Detail.IPdSession.Unknown5");
	protected virtual void ReplyPowerRequest(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Usb.Pd.Detail.IPdSession.ReplyPowerRequest");
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

