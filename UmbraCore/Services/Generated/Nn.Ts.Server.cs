using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ts.Server;
public partial class IMeasurementServer : _IMeasurementServer_Base {
	public readonly string ServiceName;
	public IMeasurementServer(string serviceName) => ServiceName = serviceName;
}
public abstract class _IMeasurementServer_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ts.Server.IMeasurementServer.Unknown0 not implemented");
	protected virtual void Unknown1(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ts.Server.IMeasurementServer.Unknown1 not implemented");
	protected virtual void Unknown2(byte[] _0) =>
		"Stub hit for Nn.Ts.Server.IMeasurementServer.Unknown2".Log();
	protected virtual void Unknown3(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Ts.Server.IMeasurementServer.Unknown3 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(im.GetBytes(8, 0x1), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1(im.GetBytes(8, 0x1), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetBytes(8, 0x2));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(im.GetBytes(8, 0x1), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ts.Server.IMeasurementServer");
		}
	}
}

