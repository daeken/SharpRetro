using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Sf.Hipc.Detail;
public partial class IHipcManager : _IHipcManager_Base;
public abstract class _IHipcManager_Base : IpcInterface {
	protected virtual void Unknown0(out byte[] _0) =>
		throw new NotImplementedException("Nn.Sf.Hipc.Detail.IHipcManager.Unknown0 not implemented");
	protected virtual IpcInterface Unknown1(byte[] _0) =>
		throw new NotImplementedException("Nn.Sf.Hipc.Detail.IHipcManager.Unknown1 not implemented");
	protected virtual IpcInterface Unknown2() =>
		throw new NotImplementedException("Nn.Sf.Hipc.Detail.IHipcManager.Unknown2 not implemented");
	protected virtual void Unknown3(out byte[] _0) =>
		throw new NotImplementedException("Nn.Sf.Hipc.Detail.IHipcManager.Unknown3 not implemented");
	protected virtual IpcInterface Unknown4(byte[] _0) =>
		throw new NotImplementedException("Nn.Sf.Hipc.Detail.IHipcManager.Unknown4 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				var _return = Unknown1(im.GetBytes(8, 0x4));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // Unknown2
				var _return = Unknown2();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(out var _0);
				om.Initialize(0, 0, 2);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // Unknown4
				var _return = Unknown4(im.GetBytes(8, 0x4));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Sf.Hipc.Detail.IHipcManager");
		}
	}
}

