using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Lm;
public partial class ILogService : _ILogService_Base {
	public readonly string ServiceName;
	public ILogService(string serviceName) => ServiceName = serviceName;
}
public abstract class _ILogService_Base : IpcInterface {
	protected virtual Nn.Lm.ILogger Initialize(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Lm.ILogService.Initialize not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				var _return = Initialize(im.GetData<ulong>(8), im.Pid);
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Lm.ILogService");
		}
	}
}

public partial class ILogger : _ILogger_Base;
public abstract class _ILogger_Base : IpcInterface {
	protected virtual void Initialize(Span<byte> _0) =>
		"Stub hit for Nn.Lm.ILogger.Initialize".Log();
	protected virtual void SetDestination(uint _0) =>
		"Stub hit for Nn.Lm.ILogger.SetDestination".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				Initialize(im.GetSpan<byte>(0x21, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // SetDestination
				SetDestination(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Lm.ILogger");
		}
	}
}

public partial class ILogGetter : _ILogGetter_Base {
	public readonly string ServiceName;
	public ILogGetter(string serviceName) => ServiceName = serviceName;
}
public abstract class _ILogGetter_Base : IpcInterface {
	protected virtual void StartLogging() =>
		"Stub hit for Nn.Lm.ILogGetter.StartLogging".Log();
	protected virtual void StopLogging() =>
		"Stub hit for Nn.Lm.ILogGetter.StopLogging".Log();
	protected virtual void GetLog() =>
		"Stub hit for Nn.Lm.ILogGetter.GetLog".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // StartLogging
				StartLogging();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // StopLogging
				StopLogging();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GetLog
				GetLog();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Lm.ILogGetter");
		}
	}
}

