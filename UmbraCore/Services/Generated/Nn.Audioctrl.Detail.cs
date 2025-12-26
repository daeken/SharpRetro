using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Audioctrl.Detail;
public partial class IAudioController : _IAudioController_Base {
	public readonly string ServiceName;
	public IAudioController(string serviceName) => ServiceName = serviceName;
}
public abstract class _IAudioController_Base : IpcInterface {
	protected virtual uint GetTargetVolume(uint _0) =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetTargetVolume not implemented");
	protected virtual void SetTargetVolume(uint _0, uint _1) =>
		"Stub hit for Nn.Audioctrl.Detail.IAudioController.SetTargetVolume".Log();
	protected virtual uint GetTargetVolumeMin() =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetTargetVolumeMin not implemented");
	protected virtual uint GetTargetVolumeMax() =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetTargetVolumeMax not implemented");
	protected virtual byte IsTargetMute(uint _0) =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.IsTargetMute not implemented");
	protected virtual void SetTargetMute(ulong _0) =>
		"Stub hit for Nn.Audioctrl.Detail.IAudioController.SetTargetMute".Log();
	protected virtual byte IsTargetConnected(uint _0) =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.IsTargetConnected not implemented");
	protected virtual void SetDefaultTarget(byte[] _0) =>
		"Stub hit for Nn.Audioctrl.Detail.IAudioController.SetDefaultTarget".Log();
	protected virtual uint GetDefaultTarget() =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetDefaultTarget not implemented");
	protected virtual uint GetAudioOutputMode(uint _0) =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetAudioOutputMode not implemented");
	protected virtual void SetAudioOutputMode(uint _0, uint _1) =>
		"Stub hit for Nn.Audioctrl.Detail.IAudioController.SetAudioOutputMode".Log();
	protected virtual void SetForceMutePolicy(uint _0) =>
		"Stub hit for Nn.Audioctrl.Detail.IAudioController.SetForceMutePolicy".Log();
	protected virtual uint GetForceMutePolicy() =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetForceMutePolicy not implemented");
	protected virtual uint GetOutputModeSetting(uint _0) =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetOutputModeSetting not implemented");
	protected virtual void SetOutputModeSetting(uint _0, uint _1) =>
		"Stub hit for Nn.Audioctrl.Detail.IAudioController.SetOutputModeSetting".Log();
	protected virtual void SetOutputTarget(uint _0) =>
		"Stub hit for Nn.Audioctrl.Detail.IAudioController.SetOutputTarget".Log();
	protected virtual void SetInputTargetForceEnabled(byte _0) =>
		"Stub hit for Nn.Audioctrl.Detail.IAudioController.SetInputTargetForceEnabled".Log();
	protected virtual void SetHeadphoneOutputLevelMode(uint _0) =>
		"Stub hit for Nn.Audioctrl.Detail.IAudioController.SetHeadphoneOutputLevelMode".Log();
	protected virtual uint GetHeadphoneOutputLevelMode() =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetHeadphoneOutputLevelMode not implemented");
	protected virtual KObject AcquireAudioVolumeUpdateEventForPlayReport() =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.AcquireAudioVolumeUpdateEventForPlayReport not implemented");
	protected virtual KObject AcquireAudioOutputDeviceUpdateEventForPlayReport() =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.AcquireAudioOutputDeviceUpdateEventForPlayReport not implemented");
	protected virtual uint GetAudioOutputTargetForPlayReport() =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetAudioOutputTargetForPlayReport not implemented");
	protected virtual void NotifyHeadphoneVolumeWarningDisplayedEvent() =>
		"Stub hit for Nn.Audioctrl.Detail.IAudioController.NotifyHeadphoneVolumeWarningDisplayedEvent".Log();
	protected virtual void SetSystemOutputMasterVolume(uint _0) =>
		"Stub hit for Nn.Audioctrl.Detail.IAudioController.SetSystemOutputMasterVolume".Log();
	protected virtual uint GetSystemOutputMasterVolume() =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetSystemOutputMasterVolume not implemented");
	protected virtual void GetAudioVolumeDataForPlayReport(out byte[] _0) =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetAudioVolumeDataForPlayReport not implemented");
	protected virtual void UpdateHeadphoneSettings(byte[] _0) =>
		"Stub hit for Nn.Audioctrl.Detail.IAudioController.UpdateHeadphoneSettings".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetTargetVolume
				var _return = GetTargetVolume(im.GetData<uint>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // SetTargetVolume
				SetTargetVolume(im.GetData<uint>(8), im.GetData<uint>(12));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GetTargetVolumeMin
				var _return = GetTargetVolumeMin();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // GetTargetVolumeMax
				var _return = GetTargetVolumeMax();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x4: { // IsTargetMute
				var _return = IsTargetMute(im.GetData<uint>(8));
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x5: { // SetTargetMute
				SetTargetMute(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // IsTargetConnected
				var _return = IsTargetConnected(im.GetData<uint>(8));
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x7: { // SetDefaultTarget
				SetDefaultTarget(im.GetBytes(8, 0x18));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // GetDefaultTarget
				var _return = GetDefaultTarget();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x9: { // GetAudioOutputMode
				var _return = GetAudioOutputMode(im.GetData<uint>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // SetAudioOutputMode
				SetAudioOutputMode(im.GetData<uint>(8), im.GetData<uint>(12));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // SetForceMutePolicy
				SetForceMutePolicy(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // GetForceMutePolicy
				var _return = GetForceMutePolicy();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xD: { // GetOutputModeSetting
				var _return = GetOutputModeSetting(im.GetData<uint>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xE: { // SetOutputModeSetting
				SetOutputModeSetting(im.GetData<uint>(8), im.GetData<uint>(12));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xF: { // SetOutputTarget
				SetOutputTarget(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // SetInputTargetForceEnabled
				SetInputTargetForceEnabled(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x11: { // SetHeadphoneOutputLevelMode
				SetHeadphoneOutputLevelMode(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12: { // GetHeadphoneOutputLevelMode
				var _return = GetHeadphoneOutputLevelMode();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x13: { // AcquireAudioVolumeUpdateEventForPlayReport
				var _return = AcquireAudioVolumeUpdateEventForPlayReport();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x14: { // AcquireAudioOutputDeviceUpdateEventForPlayReport
				var _return = AcquireAudioOutputDeviceUpdateEventForPlayReport();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x15: { // GetAudioOutputTargetForPlayReport
				var _return = GetAudioOutputTargetForPlayReport();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x16: { // NotifyHeadphoneVolumeWarningDisplayedEvent
				NotifyHeadphoneVolumeWarningDisplayedEvent();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x17: { // SetSystemOutputMasterVolume
				SetSystemOutputMasterVolume(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x18: { // GetSystemOutputMasterVolume
				var _return = GetSystemOutputMasterVolume();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x19: { // GetAudioVolumeDataForPlayReport
				GetAudioVolumeDataForPlayReport(out var _0);
				om.Initialize(0, 0, 7);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1A: { // UpdateHeadphoneSettings
				UpdateHeadphoneSettings(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audioctrl.Detail.IAudioController");
		}
	}
}

