using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ldr.Detail;
public partial class IDebugMonitorInterface : _IDebugMonitorInterface_Base;
public abstract class _IDebugMonitorInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // AddProcessToDebugLaunchQueue
				break;
			case 0x1: // ClearDebugLaunchQueue
				break;
			case 0x2: // GetNsoInfos
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldr.Detail.IDebugMonitorInterface");
		}
	}
}

public partial class IProcessManagerInterface : _IProcessManagerInterface_Base;
public abstract class _IProcessManagerInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateProcess
				break;
			case 0x1: // GetProgramInfo
				break;
			case 0x2: // RegisterTitle
				break;
			case 0x3: // UnregisterTitle
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldr.Detail.IProcessManagerInterface");
		}
	}
}

public partial class IRoInterface : _IRoInterface_Base;
public abstract class _IRoInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // LoadNro
				break;
			case 0x1: // UnloadNro
				break;
			case 0x2: // LoadNrr
				break;
			case 0x3: // UnloadNrr
				break;
			case 0x4: // Initialize
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldr.Detail.IRoInterface");
		}
	}
}

public partial class IShellInterface : _IShellInterface_Base;
public abstract class _IShellInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // AddProcessToLaunchQueue
				break;
			case 0x1: // ClearLaunchQueue
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldr.Detail.IShellInterface");
		}
	}
}

