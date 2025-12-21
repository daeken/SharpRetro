using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Fatalsrv;
public partial class IPrivateService : _IPrivateService_Base;
public abstract class _IPrivateService_Base : IpcInterface {
	protected virtual KObject GetFatalEvent() =>
		throw new NotImplementedException("Nn.Fatalsrv.IPrivateService.GetFatalEvent not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetFatalEvent
				om.Initialize(0, 1, 0);
				var _return = GetFatalEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // ThrowFatal
				om.Initialize(0, 0, 0);
				ThrowFatal(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x1: { // ThrowFatalWithPolicy
				om.Initialize(0, 0, 0);
				ThrowFatalWithPolicy(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x2: { // ThrowFatalWithCpuContext
				om.Initialize(0, 0, 0);
				ThrowFatalWithCpuContext(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x15, 0), im.Pid);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fatalsrv.IService");
		}
	}
}

