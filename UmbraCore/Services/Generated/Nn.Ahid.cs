using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ahid;
public partial class ICtrlSession : _ICtrlSession_Base;
public abstract class _ICtrlSession_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ahid.ICtrlSession.Unknown0 not implemented");
	protected virtual void Unknown1() =>
		throw new NotImplementedException("Nn.Ahid.ICtrlSession.Unknown1 not implemented");
	protected virtual void Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ahid.ICtrlSession.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Ahid.ICtrlSession.Unknown3");
	protected virtual void Unknown4(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ahid.ICtrlSession.Unknown4 not implemented");
	protected virtual void Unknown5(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ahid.ICtrlSession.Unknown5");
	protected virtual void Unknown6() =>
		throw new NotImplementedException("Nn.Ahid.ICtrlSession.Unknown6 not implemented");
	protected virtual void Unknown7(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ahid.ICtrlSession.Unknown7");
	protected virtual void Unknown8(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ahid.ICtrlSession.Unknown8 not implemented");
	protected virtual void Unknown9(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Ahid.ICtrlSession.Unknown9");
	protected virtual KObject Unknown10() =>
		throw new NotImplementedException("Nn.Ahid.ICtrlSession.Unknown10 not implemented");
	protected virtual void Unknown11() =>
		Console.WriteLine("Stub hit for Nn.Ahid.ICtrlSession.Unknown11");
	protected virtual void Unknown12(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ahid.ICtrlSession.Unknown12 not implemented");
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
			case 0x5: // Unknown5
				break;
			case 0x6: // Unknown6
				break;
			case 0x7: // Unknown7
				break;
			case 0x8: // Unknown8
				break;
			case 0x9: // Unknown9
				break;
			case 0xA: // Unknown10
				break;
			case 0xB: // Unknown11
				break;
			case 0xC: // Unknown12
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ahid.ICtrlSession");
		}
	}
}

public partial class IReadSession : _IReadSession_Base;
public abstract class _IReadSession_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ahid.IReadSession.Unknown0 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ahid.IReadSession");
		}
	}
}

public partial class IServerSession : _IServerSession_Base;
public abstract class _IServerSession_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ahid.IServerSession.Unknown0");
	protected virtual void Unknown1(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ahid.IServerSession.Unknown1");
	protected virtual Nn.Ahid.ICtrlSession Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ahid.IServerSession.Unknown2 not implemented");
	protected virtual Nn.Ahid.IReadSession Unknown3(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ahid.IServerSession.Unknown3 not implemented");
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
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ahid.IServerSession");
		}
	}
}

public partial class IWriteSession : _IWriteSession_Base;
public abstract class _IWriteSession_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ahid.IWriteSession.Unknown0 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ahid.IWriteSession");
		}
	}
}

