using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Audioctrl.Detail;
public partial class IAudioController : _IAudioController_Base;
public abstract class _IAudioController_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetTargetVolume
				break;
			case 0x1: // SetTargetVolume
				break;
			case 0x2: // GetTargetVolumeMin
				break;
			case 0x3: // GetTargetVolumeMax
				break;
			case 0x4: // IsTargetMute
				break;
			case 0x5: // SetTargetMute
				break;
			case 0x6: // IsTargetConnected
				break;
			case 0x7: // SetDefaultTarget
				break;
			case 0x8: // GetDefaultTarget
				break;
			case 0x9: // GetAudioOutputMode
				break;
			case 0xA: // SetAudioOutputMode
				break;
			case 0xB: // SetForceMutePolicy
				break;
			case 0xC: // GetForceMutePolicy
				break;
			case 0xD: // GetOutputModeSetting
				break;
			case 0xE: // SetOutputModeSetting
				break;
			case 0xF: // SetOutputTarget
				break;
			case 0x10: // SetInputTargetForceEnabled
				break;
			case 0x11: // SetHeadphoneOutputLevelMode
				break;
			case 0x12: // GetHeadphoneOutputLevelMode
				break;
			case 0x13: // AcquireAudioVolumeUpdateEventForPlayReport
				break;
			case 0x14: // AcquireAudioOutputDeviceUpdateEventForPlayReport
				break;
			case 0x15: // GetAudioOutputTargetForPlayReport
				break;
			case 0x16: // NotifyHeadphoneVolumeWarningDisplayedEvent
				break;
			case 0x17: // SetSystemOutputMasterVolume
				break;
			case 0x18: // GetSystemOutputMasterVolume
				break;
			case 0x19: // GetAudioVolumeDataForPlayReport
				break;
			case 0x1A: // UpdateHeadphoneSettings
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audioctrl.Detail.IAudioController");
		}
	}
}

