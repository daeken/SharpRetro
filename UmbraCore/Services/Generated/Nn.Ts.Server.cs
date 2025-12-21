using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ts.Server;
public partial class IMeasurementServer : _IMeasurementServer_Base;
public abstract class _IMeasurementServer_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ts.Server.IMeasurementServer.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ts.Server.IMeasurementServer.Unknown1 not implemented");
	protected virtual void Unknown2(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ts.Server.IMeasurementServer.Unknown2");
	protected virtual void Unknown3(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ts.Server.IMeasurementServer.Unknown3 not implemented");
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
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ts.Server.IMeasurementServer");
		}
	}
}

