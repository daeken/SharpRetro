using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nns.Nvdrv;
public partial class INvDrvDebugFSServices : _INvDrvDebugFSServices_Base;
public abstract class _INvDrvDebugFSServices_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // OpenLog
				break;
			case 0x1: // CloseLog
				break;
			case 0x2: // ReadLog
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nns.Nvdrv.INvDrvDebugFSServices");
		}
	}
}

public partial class INvDrvServices : _INvDrvServices_Base;
public abstract class _INvDrvServices_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Open
				break;
			case 0x1: // Ioctl
				break;
			case 0x2: // Close
				break;
			case 0x3: // Initialize
				break;
			case 0x4: // QueryEvent
				break;
			case 0x5: // MapSharedMem
				break;
			case 0x6: // GetStatus
				break;
			case 0x7: // ForceSetClientPID
				break;
			case 0x8: // SetClientPID
				break;
			case 0x9: // DumpGraphicsMemoryInfo
				break;
			case 0xA: // Unknown10
				break;
			case 0xB: // Ioctl2
				break;
			case 0xC: // Ioctl3
				break;
			case 0xD: // Unknown13
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nns.Nvdrv.INvDrvServices");
		}
	}
}

