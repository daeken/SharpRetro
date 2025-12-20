using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Omm.Detail;
public partial class IOperationModeManager : _IOperationModeManager_Base;
public abstract class _IOperationModeManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetOperationMode
				break;
			case 0x1: // GetOperationModeChangeEvent
				break;
			case 0x2: // EnableAudioVisual
				break;
			case 0x3: // DisableAudioVisual
				break;
			case 0x4: // EnterSleepAndWait
				break;
			case 0x5: // GetCradleStatus
				break;
			case 0x6: // FadeInDisplay
				break;
			case 0x7: // FadeOutDisplay
				break;
			case 0x8: // Unknown8
				break;
			case 0x9: // Unknown9
				break;
			case 0xA: // Unknown10
				break;
			case 0xB: // Unknown11
				break;
			case 0xC: // Unknown12
				break;
			case 0xD: // Unknown13
				break;
			case 0xE: // Unknown14
				break;
			case 0xF: // Unknown15
				break;
			case 0x10: // Unknown16
				break;
			case 0x11: // Unknown17
				break;
			case 0x12: // Unknown18
				break;
			case 0x13: // Unknown19
				break;
			case 0x14: // Unknown20
				break;
			case 0x15: // Unknown21
				break;
			case 0x16: // Unknown22
				break;
			case 0x17: // Unknown23
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Omm.Detail.IOperationModeManager");
		}
	}
}

