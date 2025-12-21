using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nv.Gemcontrol;
public partial class INvGemControl : _INvGemControl_Base;
public abstract class _INvGemControl_Base : IpcInterface {
	protected virtual void Unknown0() =>
		throw new NotImplementedException("Nv.Gemcontrol.INvGemControl.Unknown0 not implemented");
	protected virtual void Unknown1() =>
		throw new NotImplementedException("Nv.Gemcontrol.INvGemControl.Unknown1 not implemented");
	protected virtual void Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nv.Gemcontrol.INvGemControl.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0) =>
		throw new NotImplementedException("Nv.Gemcontrol.INvGemControl.Unknown3 not implemented");
	protected virtual void Unknown4(Span<byte> _0) =>
		throw new NotImplementedException("Nv.Gemcontrol.INvGemControl.Unknown4 not implemented");
	protected virtual void Unknown5() =>
		throw new NotImplementedException("Nv.Gemcontrol.INvGemControl.Unknown5 not implemented");
	protected virtual void Unknown6() =>
		throw new NotImplementedException("Nv.Gemcontrol.INvGemControl.Unknown6 not implemented");
	protected virtual void Unknown7() =>
		throw new NotImplementedException("Nv.Gemcontrol.INvGemControl.Unknown7 not implemented");
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
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nv.Gemcontrol.INvGemControl");
		}
	}
}

