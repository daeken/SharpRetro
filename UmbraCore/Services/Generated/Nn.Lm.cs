using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Lm;
public partial class ILogService : _ILogService_Base;
public abstract class _ILogService_Base : IpcInterface {
	protected virtual Nn.Lm.ILogger Initialize(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Lm.ILogService.Initialize not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				om.Initialize(1, 0, 0);
				var _return = Initialize(im.GetData<ulong>(8), im.Pid);
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
		Console.WriteLine("Stub hit for Nn.Lm.ILogger.Initialize");
	protected virtual void SetDestination(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Lm.ILogger.SetDestination");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				om.Initialize(0, 0, 0);
				Initialize(im.GetSpan<byte>(0x21, 0));
				break;
			}
			case 0x1: { // SetDestination
				om.Initialize(0, 0, 0);
				SetDestination(im.GetData<uint>(8));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Lm.ILogger");
		}
	}
}

public partial class ILogGetter : _ILogGetter_Base;
public abstract class _ILogGetter_Base : IpcInterface {
	protected virtual void StartLogging() =>
		Console.WriteLine("Stub hit for Nn.Lm.ILogGetter.StartLogging");
	protected virtual void StopLogging() =>
		Console.WriteLine("Stub hit for Nn.Lm.ILogGetter.StopLogging");
	protected virtual void GetLog() =>
		Console.WriteLine("Stub hit for Nn.Lm.ILogGetter.GetLog");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // StartLogging
				om.Initialize(0, 0, 0);
				StartLogging();
				break;
			}
			case 0x1: { // StopLogging
				om.Initialize(0, 0, 0);
				StopLogging();
				break;
			}
			case 0x2: { // GetLog
				om.Initialize(0, 0, 0);
				GetLog();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Lm.ILogGetter");
		}
	}
}

