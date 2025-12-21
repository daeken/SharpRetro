using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Fatalsrv;
public partial class IPrivateService : _IPrivateService_Base;
public abstract class _IPrivateService_Base : IpcInterface {
	protected virtual KObject GetFatalEvent() =>
		throw new NotImplementedException("Nn.Fatalsrv.IPrivateService.GetFatalEvent not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetFatalEvent
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fatalsrv.IPrivateService");
		}
	}
}

public partial class IService : _IService_Base;
public abstract class _IService_Base : IpcInterface {
	protected virtual void ThrowFatal(ulong _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Fatalsrv.IService.ThrowFatal");
	protected virtual void ThrowFatalWithPolicy(ulong _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Fatalsrv.IService.ThrowFatalWithPolicy");
	protected virtual void ThrowFatalWithCpuContext(ulong errorCode, ulong _1, Span<byte> errorBuf, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Fatalsrv.IService.ThrowFatalWithCpuContext");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // ThrowFatal
				break;
			}
			case 0x1: { // ThrowFatalWithPolicy
				break;
			}
			case 0x2: { // ThrowFatalWithCpuContext
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fatalsrv.IService");
		}
	}
}

