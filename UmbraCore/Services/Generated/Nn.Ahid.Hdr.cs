using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ahid.Hdr;
public partial class ISession : _ISession_Base {
	public readonly string ServiceName;
	public ISession(string serviceName) => ServiceName = serviceName;
}
public abstract class _ISession_Base : IpcInterface {
	protected virtual void Unknown0(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ahid.Hdr.ISession.Unknown0 not implemented");
	protected virtual void Unknown1(byte[] _0, Span<byte> _1, out byte[] _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Ahid.Hdr.ISession.Unknown1 not implemented");
	protected virtual void Unknown2(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ahid.Hdr.ISession.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ahid.Hdr.ISession.Unknown3 not implemented");
	protected virtual void Unknown4(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ahid.Hdr.ISession.Unknown4");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x5, 0), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetBytes(8, 0x4), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(im.GetSpan<byte>(0x5, 0), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ahid.Hdr.ISession");
		}
	}
}

