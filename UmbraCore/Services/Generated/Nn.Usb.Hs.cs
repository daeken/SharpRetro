using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Usb.Hs;
public partial class IClientEpSession : _IClientEpSession_Base;
public abstract class _IClientEpSession_Base : IpcInterface {
	protected virtual void Unknown0() =>
		Console.WriteLine("Stub hit for Nn.Usb.Hs.IClientEpSession.Unknown0");
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Usb.Hs.IClientEpSession.Unknown1");
	protected virtual KObject Unknown2() =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientEpSession.Unknown2 not implemented");
	protected virtual void Unknown3() =>
		Console.WriteLine("Stub hit for Nn.Usb.Hs.IClientEpSession.Unknown3");
	protected virtual void PostBufferAsync(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientEpSession.PostBufferAsync not implemented");
	protected virtual void Unknown5(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientEpSession.Unknown5 not implemented");
	protected virtual void Unknown6(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientEpSession.Unknown6 not implemented");
	protected virtual void Unknown7(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Usb.Hs.IClientEpSession.Unknown7");
	protected virtual void Unknown8(Span<byte> _0, KObject _1) =>
		Console.WriteLine("Stub hit for Nn.Usb.Hs.IClientEpSession.Unknown8");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				break;
			}
			case 0x1: { // Unknown1
				break;
			}
			case 0x2: { // Unknown2
				break;
			}
			case 0x3: { // Unknown3
				break;
			}
			case 0x4: { // PostBufferAsync
				break;
			}
			case 0x5: { // Unknown5
				break;
			}
			case 0x6: { // Unknown6
				break;
			}
			case 0x7: { // Unknown7
				break;
			}
			case 0x8: { // Unknown8
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
	protected virtual void Unknown1(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientIfSession.Unknown1 not implemented");
	protected virtual void Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientIfSession.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientIfSession.Unknown3 not implemented");
	protected virtual void Unknown4(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientIfSession.Unknown4 not implemented");
	protected virtual void CtrlXferAsync(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Usb.Hs.IClientIfSession.CtrlXferAsync");
	protected virtual KObject Unknown6() =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientIfSession.Unknown6 not implemented");
	protected virtual void GetCtrlXferReport(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientIfSession.GetCtrlXferReport not implemented");
	protected virtual void Unknown8() =>
		Console.WriteLine("Stub hit for Nn.Usb.Hs.IClientIfSession.Unknown8");
	protected virtual void GetClientEpSession(Span<byte> _0, Span<byte> _1, out Nn.Usb.Hs.IClientEpSession _2) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientIfSession.GetClientEpSession not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				break;
			}
			case 0x1: { // Unknown1
				break;
			}
			case 0x2: { // Unknown2
				break;
			}
			case 0x3: { // Unknown3
				break;
			}
			case 0x4: { // Unknown4
				break;
			}
			case 0x5: { // CtrlXferAsync
				break;
			}
			case 0x6: { // Unknown6
				break;
			}
			case 0x7: { // GetCtrlXferReport
				break;
			}
			case 0x8: { // Unknown8
				break;
			}
			case 0x9: { // GetClientEpSession
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Hs.IClientIfSession");
		}
	}
}

public partial class IClientRootSession : _IClientRootSession_Base;
public abstract class _IClientRootSession_Base : IpcInterface {
	protected virtual void BindClientProcess(KObject _0) =>
		Console.WriteLine("Stub hit for Nn.Usb.Hs.IClientRootSession.BindClientProcess");
	protected virtual void Unknown1(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientRootSession.Unknown1 not implemented");
	protected virtual void Unknown2(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientRootSession.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientRootSession.Unknown3 not implemented");
	protected virtual KObject Unknown4(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientRootSession.Unknown4 not implemented");
	protected virtual void Unknown5(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Usb.Hs.IClientRootSession.Unknown5");
	protected virtual KObject GetInterfaceStateChangeEvent() =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientRootSession.GetInterfaceStateChangeEvent not implemented");
	protected virtual void GetClientIfSession(Span<byte> _0, Span<byte> _1, Span<byte> _2, out Nn.Usb.Hs.IClientIfSession _3) =>
		throw new NotImplementedException("Nn.Usb.Hs.IClientRootSession.GetClientIfSession not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // BindClientProcess
				break;
			}
			case 0x1: { // Unknown1
				break;
			}
			case 0x2: { // Unknown2
				break;
			}
			case 0x3: { // Unknown3
				break;
			}
			case 0x4: { // Unknown4
				break;
			}
			case 0x5: { // Unknown5
				break;
			}
			case 0x6: { // GetInterfaceStateChangeEvent
				break;
			}
			case 0x7: { // GetClientIfSession
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Hs.IClientRootSession");
		}
	}
}

