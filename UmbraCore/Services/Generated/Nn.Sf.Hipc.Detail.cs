using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Sf.Hipc.Detail;
public partial class IHipcManager : _IHipcManager_Base;
public abstract class _IHipcManager_Base : IpcInterface {
	protected virtual void Unknown0() =>
		throw new NotImplementedException("Nn.Sf.Hipc.Detail.IHipcManager.Unknown0 not implemented");
	protected virtual IpcInterface Unknown1(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Sf.Hipc.Detail.IHipcManager.Unknown1 not implemented");
	protected virtual IpcInterface Unknown2() =>
		throw new NotImplementedException("Nn.Sf.Hipc.Detail.IHipcManager.Unknown2 not implemented");
	protected virtual void Unknown3() =>
		throw new NotImplementedException("Nn.Sf.Hipc.Detail.IHipcManager.Unknown3 not implemented");
	protected virtual IpcInterface Unknown4(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Sf.Hipc.Detail.IHipcManager.Unknown4 not implemented");
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
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Sf.Hipc.Detail.IHipcManager");
		}
	}
}

