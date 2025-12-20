using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Lm;
public partial class ILogService : _ILogService_Base;
public abstract class _ILogService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Initialize
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Lm.ILogService");
		}
	}
}

public partial class ILogger : _ILogger_Base;
public abstract class _ILogger_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Initialize
				break;
			case 0x1: // SetDestination
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Lm.ILogger");
		}
	}
}

public partial class ILogGetter : _ILogGetter_Base;
public abstract class _ILogGetter_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // StartLogging
				break;
			case 0x1: // StopLogging
				break;
			case 0x2: // GetLog
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Lm.ILogGetter");
		}
	}
}

