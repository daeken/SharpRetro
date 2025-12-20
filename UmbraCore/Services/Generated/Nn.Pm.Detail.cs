using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pm.Detail;
public partial class IBootModeInterface : _IBootModeInterface_Base;
public abstract class _IBootModeInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetBootMode
				break;
			case 0x1: // SetMaintenanceBoot
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pm.Detail.IBootModeInterface");
		}
	}
}

public partial class IDebugMonitorInterface : _IDebugMonitorInterface_Base;
public abstract class _IDebugMonitorInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetDebugProcesses
				break;
			case 0x1: // StartDebugProcess
				break;
			case 0x2: // GetTitlePid
				break;
			case 0x3: // EnableDebugForTitleId
				break;
			case 0x4: // GetApplicationPid
				break;
			case 0x5: // EnableDebugForApplication
				break;
			case 0x6: // DisableDebug
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pm.Detail.IDebugMonitorInterface");
		}
	}
}

public partial class IInformationInterface : _IInformationInterface_Base;
public abstract class _IInformationInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetTitleId
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pm.Detail.IInformationInterface");
		}
	}
}

public partial class IShellInterface : _IShellInterface_Base;
public abstract class _IShellInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // LaunchProcess
				break;
			case 0x1: // TerminateProcessByPid
				break;
			case 0x2: // TerminateProcessByTitleId
				break;
			case 0x3: // GetProcessEventWaiter
				break;
			case 0x4: // GetProcessEventType
				break;
			case 0x5: // NotifyBootFinished
				break;
			case 0x6: // GetApplicationPid
				break;
			case 0x7: // BoostSystemMemoryResourceLimit
				break;
			case 0x8: // GetApplicationPid
				break;
			case 0x9: // BoostSystemMemoryResourceLimit
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pm.Detail.IShellInterface");
		}
	}
}

