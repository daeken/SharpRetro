using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ntc.Detail.Service;
public partial class IEnsureNetworkClockAvailabilityService : _IEnsureNetworkClockAvailabilityService_Base;
public abstract class _IEnsureNetworkClockAvailabilityService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // StartTask
				break;
			case 0x1: // GetFinishNotificationEvent
				break;
			case 0x2: // GetResult
				break;
			case 0x3: // Cancel
				break;
			case 0x4: // IsProcessing
				break;
			case 0x5: // GetServerTime
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ntc.Detail.Service.IEnsureNetworkClockAvailabilityService");
		}
	}
}

public partial class IStaticService : _IStaticService_Base;
public abstract class _IStaticService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // OpenEnsureNetworkClockAvailabilityService
				break;
			case 0x64: // SuspendAutonomicTimeCorrection
				break;
			case 0x65: // ResumeAutonomicTimeCorrection
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ntc.Detail.Service.IStaticService");
		}
	}
}

