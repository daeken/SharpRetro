using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ahid.Hdr;
public partial class ISession : _ISession_Base;
public abstract class _ISession_Base : IpcInterface {
	protected virtual void Unknown0() =>
		throw new NotImplementedException("Nn.Ahid.Hdr.ISession.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ahid.Hdr.ISession.Unknown1 not implemented");
	protected virtual void Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ahid.Hdr.ISession.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ahid.Hdr.ISession.Unknown3 not implemented");
	protected virtual void Unknown4(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ahid.Hdr.ISession.Unknown4");
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
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ahid.Hdr.ISession");
		}
	}
}

