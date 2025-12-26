using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Migration.Detail;
public partial class IAsyncContext : _IAsyncContext_Base;
public abstract class _IAsyncContext_Base : IpcInterface {
	protected virtual KObject Unknown0() =>
		throw new NotImplementedException("Nn.Migration.Detail.IAsyncContext.Unknown0 not implemented");
	protected virtual void Unknown1() =>
		"Stub hit for Nn.Migration.Detail.IAsyncContext.Unknown1".Log();
	protected virtual void Unknown2(out byte[] _0) =>
		throw new NotImplementedException("Nn.Migration.Detail.IAsyncContext.Unknown2 not implemented");
	protected virtual void Unknown3() =>
		"Stub hit for Nn.Migration.Detail.IAsyncContext.Unknown3".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				var _return = Unknown0();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // Unknown1
				Unknown1();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Migration.Detail.IAsyncContext");
		}
	}
}

