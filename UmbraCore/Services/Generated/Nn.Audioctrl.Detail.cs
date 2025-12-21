using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Audioctrl.Detail;
public partial class IAudioController : _IAudioController_Base;
public abstract class _IAudioController_Base : IpcInterface {
	protected virtual uint GetTargetVolume(uint _0) =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetTargetVolume not implemented");
	protected virtual void SetTargetVolume(uint _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Audioctrl.Detail.IAudioController.SetTargetVolume");
	protected virtual uint GetTargetVolumeMin() =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetTargetVolumeMin not implemented");
	protected virtual uint GetTargetVolumeMax() =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetTargetVolumeMax not implemented");
	protected virtual byte IsTargetMute(uint _0) =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.IsTargetMute not implemented");
	protected virtual void SetTargetMute(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Audioctrl.Detail.IAudioController.SetTargetMute");
	protected virtual byte IsTargetConnected(uint _0) =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.IsTargetConnected not implemented");
	protected virtual void SetDefaultTarget(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Audioctrl.Detail.IAudioController.SetDefaultTarget");
	protected virtual uint GetDefaultTarget() =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetDefaultTarget not implemented");
	protected virtual uint GetAudioOutputMode(uint _0) =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetAudioOutputMode not implemented");
	protected virtual void SetAudioOutputMode(uint _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Audioctrl.Detail.IAudioController.SetAudioOutputMode");
	protected virtual void SetForceMutePolicy(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Audioctrl.Detail.IAudioController.SetForceMutePolicy");
	protected virtual uint GetForceMutePolicy() =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetForceMutePolicy not implemented");
	protected virtual uint GetOutputModeSetting(uint _0) =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetOutputModeSetting not implemented");
	protected virtual void SetOutputModeSetting(uint _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Audioctrl.Detail.IAudioController.SetOutputModeSetting");
	protected virtual void SetOutputTarget(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Audioctrl.Detail.IAudioController.SetOutputTarget");
	protected virtual void SetInputTargetForceEnabled(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Audioctrl.Detail.IAudioController.SetInputTargetForceEnabled");
	protected virtual void SetHeadphoneOutputLevelMode(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Audioctrl.Detail.IAudioController.SetHeadphoneOutputLevelMode");
	protected virtual uint GetHeadphoneOutputLevelMode() =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetHeadphoneOutputLevelMode not implemented");
	protected virtual KObject AcquireAudioVolumeUpdateEventForPlayReport() =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.AcquireAudioVolumeUpdateEventForPlayReport not implemented");
	protected virtual KObject AcquireAudioOutputDeviceUpdateEventForPlayReport() =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.AcquireAudioOutputDeviceUpdateEventForPlayReport not implemented");
	protected virtual uint GetAudioOutputTargetForPlayReport() =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetAudioOutputTargetForPlayReport not implemented");
	protected virtual void NotifyHeadphoneVolumeWarningDisplayedEvent() =>
		Console.WriteLine("Stub hit for Nn.Audioctrl.Detail.IAudioController.NotifyHeadphoneVolumeWarningDisplayedEvent");
	protected virtual void SetSystemOutputMasterVolume(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Audioctrl.Detail.IAudioController.SetSystemOutputMasterVolume");
	protected virtual uint GetSystemOutputMasterVolume() =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetSystemOutputMasterVolume not implemented");
	protected virtual void GetAudioVolumeDataForPlayReport() =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetAudioVolumeDataForPlayReport not implemented");
	protected virtual void UpdateHeadphoneSettings(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Audioctrl.Detail.IAudioController.UpdateHeadphoneSettings");
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

