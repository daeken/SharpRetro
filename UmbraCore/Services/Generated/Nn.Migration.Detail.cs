using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Migration.Detail;
public partial class IAsyncContext : _IAsyncContext_Base;
public abstract class _IAsyncContext_Base : IpcInterface {
	protected virtual KObject Unknown0() =>
		throw new NotImplementedException("Nn.Migration.Detail.IAsyncContext.Unknown0 not implemented");
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Migration.Detail.IAsyncContext.Unknown1");
	protected virtual void Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Migration.Detail.IAsyncContext.Unknown2 not implemented");
	protected virtual void Unknown3() =>
		Console.WriteLine("Stub hit for Nn.Migration.Detail.IAsyncContext.Unknown3");
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
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Migration.Detail.IAsyncContext");
		}
	}
}

