using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nv.Gemcontrol;
public partial class INvGemControl : _INvGemControl_Base {
	public readonly string ServiceName;
	public INvGemControl(string serviceName) => ServiceName = serviceName;
}
public abstract class _INvGemControl_Base : IpcInterface {
	protected virtual void Unknown0(out byte[] _0) =>
		throw new NotImplementedException("Nv.Gemcontrol.INvGemControl.Unknown0 not implemented");
	protected virtual void Unknown1(out byte[] _0, out KObject _1) =>
		throw new NotImplementedException("Nv.Gemcontrol.INvGemControl.Unknown1 not implemented");
	protected virtual void Unknown2(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nv.Gemcontrol.INvGemControl.Unknown2 not implemented");
	protected virtual void Unknown3(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nv.Gemcontrol.INvGemControl.Unknown3 not implemented");
	protected virtual void Unknown4(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nv.Gemcontrol.INvGemControl.Unknown4 not implemented");
	protected virtual void Unknown5(out byte[] _0) =>
		throw new NotImplementedException("Nv.Gemcontrol.INvGemControl.Unknown5 not implemented");
	protected virtual void Unknown6(out byte[] _0) =>
		throw new NotImplementedException("Nv.Gemcontrol.INvGemControl.Unknown6 not implemented");
	protected virtual void Unknown7(out byte[] _0) =>
		throw new NotImplementedException("Nv.Gemcontrol.INvGemControl.Unknown7 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1(out var _0, out var _1);
				om.Initialize(0, 1, 4);
				om.SetBytes(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetBytes(8, 0x1), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // Unknown5
				Unknown5(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6: { // Unknown6
				Unknown6(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7: { // Unknown7
				Unknown7(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nv.Gemcontrol.INvGemControl");
		}
	}
}

