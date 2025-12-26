using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Fatalsrv;
public partial class IPrivateService : _IPrivateService_Base {
	public readonly string ServiceName;
	public IPrivateService(string serviceName) => ServiceName = serviceName;
}
public abstract class _IPrivateService_Base : IpcInterface {
	protected virtual KObject GetFatalEvent() =>
		throw new NotImplementedException("Nn.Fatalsrv.IPrivateService.GetFatalEvent not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetFatalEvent
				var _return = GetFatalEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fatalsrv.IPrivateService");
		}
	}
}

public partial class IService : _IService_Base {
	public readonly string ServiceName;
	public IService(string serviceName) => ServiceName = serviceName;
}
public abstract class _IService_Base : IpcInterface {
	protected virtual void ThrowFatal(ulong _0, ulong _1, ulong _2) =>
		"Stub hit for Nn.Fatalsrv.IService.ThrowFatal".Log();
	protected virtual void ThrowFatalWithPolicy(ulong _0, ulong _1, ulong _2) =>
		"Stub hit for Nn.Fatalsrv.IService.ThrowFatalWithPolicy".Log();
	protected virtual void ThrowFatalWithCpuContext(ulong errorCode, ulong _1, Span<byte> errorBuf, ulong _3) =>
		"Stub hit for Nn.Fatalsrv.IService.ThrowFatalWithCpuContext".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // ThrowFatal
				ThrowFatal(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // ThrowFatalWithPolicy
				ThrowFatalWithPolicy(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // ThrowFatalWithCpuContext
				ThrowFatalWithCpuContext(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x15, 0), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fatalsrv.IService");
		}
	}
}

