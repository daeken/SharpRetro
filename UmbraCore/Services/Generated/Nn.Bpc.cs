using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Bpc;
public partial class IBoardPowerControlManager : _IBoardPowerControlManager_Base;
public abstract class _IBoardPowerControlManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // ShutdownSystem
				break;
			case 0x1: // RebootSystem
				break;
			case 0x2: // GetWakeupReason
				break;
			case 0x3: // GetShutdownReason
				break;
			case 0x4: // GetAcOk
				break;
			case 0x5: // GetBoardPowerControlEvent
				break;
			case 0x6: // GetSleepButtonState
				break;
			case 0x7: // GetPowerEvent
				break;
			case 0x8: // Unknown8
				break;
			case 0x9: // Unknown9
				break;
			case 0xA: // Unknown10
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bpc.IBoardPowerControlManager");
		}
	}
}

public partial class IPowerButtonManager : _IPowerButtonManager_Base;
public abstract class _IPowerButtonManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bpc.IPowerButtonManager");
		}
	}
}

public partial class IRtcManager : _IRtcManager_Base;
public abstract class _IRtcManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetExternalRtcValue
				break;
			case 0x1: // SetExternalRtcValue
				break;
			case 0x2: // ReadExternalRtcResetFlag
				break;
			case 0x3: // ClearExternalRtcResetFlag
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bpc.IRtcManager");
		}
	}
}

public partial class IWakeupConfigManager : _IWakeupConfigManager_Base;
public abstract class _IWakeupConfigManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bpc.IWakeupConfigManager");
		}
	}
}

