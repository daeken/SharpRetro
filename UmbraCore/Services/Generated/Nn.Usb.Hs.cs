using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Usb.Hs;
public partial class IClientEpSession : _IClientEpSession_Base;
public abstract class _IClientEpSession_Base : IpcInterface {
	protected virtual void Unknown0() =>
		"Stub hit for Nn.Usb.Hs.IClientEpSession.Unknown0".Log();
	protected virtual void Unknown1() =>
		"Stub hit for Nn.Usb.Hs.IClientEpSession.Unknown1".Log();
	protected virtual KObject Unknown2() =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientEpSession.Unknown2 not implemented");
	protected virtual void Unknown3() =>
		"Stub hit for Nn.Usb.Hs.IClientEpSession.Unknown3".Log();
	protected virtual void PostBufferAsync(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientEpSession.PostBufferAsync not implemented");
	protected virtual void Unknown5(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientEpSession.Unknown5 not implemented");
	protected virtual void Unknown6(byte[] _0, Span<byte> _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientEpSession.Unknown6 not implemented");
	protected virtual void Unknown7(byte[] _0) =>
		"Stub hit for Nn.Usb.Hs.IClientEpSession.Unknown7".Log();
	protected virtual void Unknown8(byte[] _0, KObject _1) =>
		"Stub hit for Nn.Usb.Hs.IClientEpSession.Unknown8".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				var _return = Unknown2();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x3: { // Unknown3
				Unknown3();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // PostBufferAsync
				PostBufferAsync(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // Unknown5
				Unknown5(im.GetBytes(8, 0x4), out var _0, im.GetSpan<byte>(0x22, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6: { // Unknown6
				Unknown6(im.GetBytes(8, 0x20), im.GetSpan<byte>(0x21, 0), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7: { // Unknown7
				Unknown7(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // Unknown8
				Unknown8(im.GetBytes(8, 0x4), Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Hs.IClientEpSession");
		}
	}
}

public partial class IClientIfSession : _IClientIfSession_Base;
public abstract class _IClientIfSession_Base : IpcInterface {
	protected virtual KObject Unknown0() =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientIfSession.Unknown0 not implemented");
	protected virtual void Unknown1(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientIfSession.Unknown1 not implemented");
	protected virtual void Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientIfSession.Unknown2 not implemented");
	protected virtual void Unknown3(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientIfSession.Unknown3 not implemented");
	protected virtual void Unknown4(out byte[] _0) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientIfSession.Unknown4 not implemented");
	protected virtual void CtrlXferAsync(byte[] _0) =>
		"Stub hit for Nn.Usb.Hs.IClientIfSession.CtrlXferAsync".Log();
	protected virtual KObject Unknown6() =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientIfSession.Unknown6 not implemented");
	protected virtual void GetCtrlXferReport(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientIfSession.GetCtrlXferReport not implemented");
	protected virtual void Unknown8() =>
		"Stub hit for Nn.Usb.Hs.IClientIfSession.Unknown8".Log();
	protected virtual void GetClientEpSession(byte[] _0, out byte[] _1, out Nn.Usb.Hs.IClientEpSession _2) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientIfSession.GetClientEpSession not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				var _return = Unknown0();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // Unknown1
				Unknown1(im.GetBytes(8, 0x1), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(im.GetBytes(8, 0x1), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // CtrlXferAsync
				CtrlXferAsync(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // Unknown6
				var _return = Unknown6();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x7: { // GetCtrlXferReport
				GetCtrlXferReport(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // Unknown8
				Unknown8();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // GetClientEpSession
				GetClientEpSession(im.GetBytes(8, 0x14), out var _0, out var _1);
				om.Initialize(1, 0, 7);
				om.SetBytes(8, _0);
				om.Move(0, CreateHandle(_1));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Hs.IClientIfSession");
		}
	}
}

public partial class IClientRootSession : _IClientRootSession_Base {
	public readonly string ServiceName;
	public IClientRootSession(string serviceName) => ServiceName = serviceName;
}
public abstract class _IClientRootSession_Base : IpcInterface {
	protected virtual void BindClientProcess(KObject _0) =>
		"Stub hit for Nn.Usb.Hs.IClientRootSession.BindClientProcess".Log();
	protected virtual void Unknown1(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientRootSession.Unknown1 not implemented");
	protected virtual void Unknown2(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientRootSession.Unknown2 not implemented");
	protected virtual void Unknown3(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientRootSession.Unknown3 not implemented");
	protected virtual KObject Unknown4(byte[] _0) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientRootSession.Unknown4 not implemented");
	protected virtual void Unknown5(byte[] _0) =>
		"Stub hit for Nn.Usb.Hs.IClientRootSession.Unknown5".Log();
	protected virtual KObject GetInterfaceStateChangeEvent() =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientRootSession.GetInterfaceStateChangeEvent not implemented");
	protected virtual void GetClientIfSession(byte[] _0, Span<byte> _1, Span<byte> _2, out Nn.Usb.Hs.IClientIfSession _3) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientRootSession.GetClientIfSession not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // BindClientProcess
				BindClientProcess(Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // Unknown4
				var _return = Unknown4(im.GetBytes(8, 0x12));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x5: { // Unknown5
				Unknown5(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // GetInterfaceStateChangeEvent
				var _return = GetInterfaceStateChangeEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x7: { // GetClientIfSession
				GetClientIfSession(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x6, 0), im.GetSpan<byte>(0x6, 1), out var _2);
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_2));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Hs.IClientRootSession");
		}
	}
}

