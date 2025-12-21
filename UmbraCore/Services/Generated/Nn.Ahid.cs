using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ahid;
public partial class ICtrlSession : _ICtrlSession_Base;
public abstract class _ICtrlSession_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ahid.ICtrlSession.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ahid.ICtrlSession.Unknown1 not implemented");
	protected virtual void Unknown2(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ahid.ICtrlSession.Unknown2 not implemented");
	protected virtual void Unknown3(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Ahid.ICtrlSession.Unknown3");
	protected virtual void Unknown4(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ahid.ICtrlSession.Unknown4 not implemented");
	protected virtual void Unknown5(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ahid.ICtrlSession.Unknown5");
	protected virtual void Unknown6(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ahid.ICtrlSession.Unknown6 not implemented");
	protected virtual void Unknown7(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ahid.ICtrlSession.Unknown7");
	protected virtual void Unknown8(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ahid.ICtrlSession.Unknown8 not implemented");
	protected virtual void Unknown9(byte[] _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Ahid.ICtrlSession.Unknown9");
	protected virtual KObject Unknown10() =>
		throw new NotImplementedException("Nn.Ahid.ICtrlSession.Unknown10 not implemented");
	protected virtual void Unknown11() =>
		Console.WriteLine("Stub hit for Nn.Ahid.ICtrlSession.Unknown11");
	protected virtual void Unknown12(byte[] _0, Span<byte> _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Ahid.ICtrlSession.Unknown12 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(im.GetBytes(8, 0x1), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetBytes(8, 0x2), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(im.GetBytes(8, 0x2), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4(im.GetBytes(8, 0x1), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // Unknown5
				Unknown5(im.GetBytes(8, 0x2));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // Unknown6
				Unknown6(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // Unknown7
				Unknown7(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // Unknown8
				Unknown8(im.GetBytes(8, 0x6), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // Unknown9
				Unknown9(im.GetBytes(8, 0x6), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // Unknown10
				var _return = Unknown10();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xB: { // Unknown11
				Unknown11();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // Unknown12
				Unknown12(im.GetBytes(8, 0x8), im.GetSpan<byte>(0x5, 0), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ahid.ICtrlSession");
		}
	}
}

public partial class IReadSession : _IReadSession_Base;
public abstract class _IReadSession_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ahid.IReadSession.Unknown0 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(im.GetBytes(8, 0x8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ahid.IReadSession");
		}
	}
}

public partial class IServerSession : _IServerSession_Base {
	public readonly string ServiceName;
	public IServerSession(string serviceName) => ServiceName = serviceName;
}
public abstract class _IServerSession_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ahid.IServerSession.Unknown0");
	protected virtual void Unknown1(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ahid.IServerSession.Unknown1");
	protected virtual Nn.Ahid.ICtrlSession Unknown2(byte[] _0) =>
		throw new NotImplementedException("Nn.Ahid.IServerSession.Unknown2 not implemented");
	protected virtual Nn.Ahid.IReadSession Unknown3(byte[] _0) =>
		throw new NotImplementedException("Nn.Ahid.IServerSession.Unknown3 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				var _return = Unknown2(im.GetBytes(8, 0x4));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // Unknown3
				var _return = Unknown3(im.GetBytes(8, 0x4));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ahid.IServerSession");
		}
	}
}

public partial class IWriteSession : _IWriteSession_Base;
public abstract class _IWriteSession_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ahid.IWriteSession.Unknown0 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(im.GetSpan<byte>(0x5, 0), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ahid.IWriteSession");
		}
	}
}

