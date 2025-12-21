using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Usb.Pd.Detail;
public partial class IPdCradleManager : _IPdCradleManager_Base {
	public readonly string ServiceName;
	public IPdCradleManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IPdCradleManager_Base : IpcInterface {
	protected virtual Nn.Usb.Pd.Detail.IPdCradleSession GetPdCradleSession() =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdCradleManager.GetPdCradleSession not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetPdCradleSession
				var _return = GetPdCradleSession();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Pd.Detail.IPdCradleManager");
		}
	}
}

public partial class IPdCradleSession : _IPdCradleSession_Base;
public abstract class _IPdCradleSession_Base : IpcInterface {
	protected virtual void VdmUserWrite(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Usb.Pd.Detail.IPdCradleSession.VdmUserWrite");
	protected virtual void VdmUserRead(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdCradleSession.VdmUserRead not implemented");
	protected virtual void Vdm20Init() =>
		Console.WriteLine("Stub hit for Nn.Usb.Pd.Detail.IPdCradleSession.Vdm20Init");
	protected virtual void GetFwType(out byte[] _0) =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdCradleSession.GetFwType not implemented");
	protected virtual void GetFwRevision(out byte[] _0) =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdCradleSession.GetFwRevision not implemented");
	protected virtual void GetManufacturerId(out byte[] _0) =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdCradleSession.GetManufacturerId not implemented");
	protected virtual void GetDeviceId(out byte[] _0) =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdCradleSession.GetDeviceId not implemented");
	protected virtual void Unknown7(out byte[] _0) =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdCradleSession.Unknown7 not implemented");
	protected virtual void Unknown8(out byte[] _0) =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdCradleSession.Unknown8 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // VdmUserWrite
				VdmUserWrite(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // VdmUserRead
				VdmUserRead(im.GetBytes(8, 0x4), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // Vdm20Init
				Vdm20Init();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // GetFwType
				GetFwType(out var _0);
				om.Initialize(0, 0, 2);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // GetFwRevision
				GetFwRevision(out var _0);
				om.Initialize(0, 0, 2);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // GetManufacturerId
				GetManufacturerId(out var _0);
				om.Initialize(0, 0, 2);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6: { // GetDeviceId
				GetDeviceId(out var _0);
				om.Initialize(0, 0, 2);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7: { // Unknown7
				Unknown7(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x8: { // Unknown8
				Unknown8(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Pd.Detail.IPdCradleSession");
		}
	}
}

public partial class IPdManager : _IPdManager_Base {
	public readonly string ServiceName;
	public IPdManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IPdManager_Base : IpcInterface {
	protected virtual Nn.Usb.Pd.Detail.IPdSession GetPdSession() =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdManager.GetPdSession not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetPdSession
				var _return = GetPdSession();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Pd.Detail.IPdManager");
		}
	}
}

public partial class IPdManufactureManager : _IPdManufactureManager_Base;
public abstract class _IPdManufactureManager_Base : IpcInterface {
	protected virtual Nn.Usb.Pd.Detail.IPdManufactureSession Unknown0() =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdManufactureManager.Unknown0 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				var _return = Unknown0();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Pd.Detail.IPdManufactureManager");
		}
	}
}

public partial class IPdManufactureSession : _IPdManufactureSession_Base;
public abstract class _IPdManufactureSession_Base : IpcInterface {
	protected virtual void Unknown0(out byte[] _0) =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdManufactureSession.Unknown0 not implemented");
	protected virtual void Unknown1(out byte[] _0) =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdManufactureSession.Unknown1 not implemented");
	protected virtual void Unknown2(out byte[] _0) =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdManufactureSession.Unknown2 not implemented");
	protected virtual void Unknown3(out byte[] _0) =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdManufactureSession.Unknown3 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(out var _0);
				om.Initialize(0, 0, 2);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1(out var _0);
				om.Initialize(0, 0, 2);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(out var _0);
				om.Initialize(0, 0, 2);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(out var _0);
				om.Initialize(0, 0, 2);
				om.SetBytes(8, _0);
				break;
			}
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
	protected virtual void GetStatus(out byte[] _0) =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdSession.GetStatus not implemented");
	protected virtual void GetNotice(out byte[] _0) =>
		throw new NotImplementedException("Nn.Usb.Pd.Detail.IPdSession.GetNotice not implemented");
	protected virtual void Unknown4() =>
		Console.WriteLine("Stub hit for Nn.Usb.Pd.Detail.IPdSession.Unknown4");
	protected virtual void Unknown5() =>
		Console.WriteLine("Stub hit for Nn.Usb.Pd.Detail.IPdSession.Unknown5");
	protected virtual void ReplyPowerRequest(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Usb.Pd.Detail.IPdSession.ReplyPowerRequest");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // BindNoticeEvent
				var _return = BindNoticeEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // Unknown1
				Unknown1();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GetStatus
				GetStatus(out var _0);
				om.Initialize(0, 0, 20);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // GetNotice
				GetNotice(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // Unknown5
				Unknown5();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // ReplyPowerRequest
				ReplyPowerRequest(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Pd.Detail.IPdSession");
		}
	}
}

