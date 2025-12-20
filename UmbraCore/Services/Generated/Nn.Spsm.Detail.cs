using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Spsm.Detail;
public partial class IPowerStateInterface : _IPowerStateInterface_Base;
public abstract class _IPowerStateInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetState
				break;
			case 0x1: // SleepSystemAndWaitAwake
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // GetNotificationMessageEventHandle
				break;
			case 0x5: // Unknown5
				break;
			case 0x6: // Unknown6
				break;
			case 0x7: // Unknown7
				break;
			case 0x8: // AnalyzePerformanceLogForLastSleepWakeSequence
				break;
			case 0x9: // ChangeHomeButtonLongPressingTime
				break;
			case 0xA: // Unknown10
				break;
			case 0xB: // Unknown11
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Spsm.Detail.IPowerStateInterface");
		}
	}
}

