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
	protected virtual void SetDefaultTarget(byte[] _0) =>
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
	protected virtual void GetAudioVolumeDataForPlayReport(out byte[] _0) =>
		throw new NotImplementedException("Nn.Audioctrl.Detail.IAudioController.GetAudioVolumeDataForPlayReport not implemented");
	protected virtual void UpdateHeadphoneSettings(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Audioctrl.Detail.IAudioController.UpdateHeadphoneSettings");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetTargetVolume
				om.Initialize(0, 0, 4);
				var _return = GetTargetVolume(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // SetTargetVolume
				om.Initialize(0, 0, 0);
				SetTargetVolume(im.GetData<uint>(8), im.GetData<uint>(12));
				break;
			}
			case 0x2: { // GetTargetVolumeMin
				om.Initialize(0, 0, 4);
				var _return = GetTargetVolumeMin();
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // GetTargetVolumeMax
				om.Initialize(0, 0, 4);
				var _return = GetTargetVolumeMax();
				om.SetData(8, _return);
				break;
			}
			case 0x4: { // IsTargetMute
				om.Initialize(0, 0, 1);
				var _return = IsTargetMute(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x5: { // SetTargetMute
				om.Initialize(0, 0, 0);
				SetTargetMute(im.GetData<ulong>(8));
				break;
			}
			case 0x6: { // IsTargetConnected
				om.Initialize(0, 0, 1);
				var _return = IsTargetConnected(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x7: { // SetDefaultTarget
				om.Initialize(0, 0, 0);
				SetDefaultTarget(im.GetBytes(8, 0x18));
				break;
			}
			case 0x8: { // GetDefaultTarget
				om.Initialize(0, 0, 4);
				var _return = GetDefaultTarget();
				om.SetData(8, _return);
				break;
			}
			case 0x9: { // GetAudioOutputMode
				om.Initialize(0, 0, 4);
				var _return = GetAudioOutputMode(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // SetAudioOutputMode
				om.Initialize(0, 0, 0);
				SetAudioOutputMode(im.GetData<uint>(8), im.GetData<uint>(12));
				break;
			}
			case 0xB: { // SetForceMutePolicy
				om.Initialize(0, 0, 0);
				SetForceMutePolicy(im.GetData<uint>(8));
				break;
			}
			case 0xC: { // GetForceMutePolicy
				om.Initialize(0, 0, 4);
				var _return = GetForceMutePolicy();
				om.SetData(8, _return);
				break;
			}
			case 0xD: { // GetOutputModeSetting
				om.Initialize(0, 0, 4);
				var _return = GetOutputModeSetting(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			case 0xE: { // SetOutputModeSetting
				om.Initialize(0, 0, 0);
				SetOutputModeSetting(im.GetData<uint>(8), im.GetData<uint>(12));
				break;
			}
			case 0xF: { // SetOutputTarget
				om.Initialize(0, 0, 0);
				SetOutputTarget(im.GetData<uint>(8));
				break;
			}
			case 0x10: { // SetInputTargetForceEnabled
				om.Initialize(0, 0, 0);
				SetInputTargetForceEnabled(im.GetData<byte>(8));
				break;
			}
			case 0x11: { // SetHeadphoneOutputLevelMode
				om.Initialize(0, 0, 0);
				SetHeadphoneOutputLevelMode(im.GetData<uint>(8));
				break;
			}
			case 0x12: { // GetHeadphoneOutputLevelMode
				om.Initialize(0, 0, 4);
				var _return = GetHeadphoneOutputLevelMode();
				om.SetData(8, _return);
				break;
			}
			case 0x13: { // AcquireAudioVolumeUpdateEventForPlayReport
				om.Initialize(0, 1, 0);
				var _return = AcquireAudioVolumeUpdateEventForPlayReport();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x14: { // AcquireAudioOutputDeviceUpdateEventForPlayReport
				om.Initialize(0, 1, 0);
				var _return = AcquireAudioOutputDeviceUpdateEventForPlayReport();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x15: { // GetAudioOutputTargetForPlayReport
				om.Initialize(0, 0, 4);
				var _return = GetAudioOutputTargetForPlayReport();
				om.SetData(8, _return);
				break;
			}
			case 0x16: { // NotifyHeadphoneVolumeWarningDisplayedEvent
				om.Initialize(0, 0, 0);
				NotifyHeadphoneVolumeWarningDisplayedEvent();
				break;
			}
			case 0x17: { // SetSystemOutputMasterVolume
				om.Initialize(0, 0, 0);
				SetSystemOutputMasterVolume(im.GetData<uint>(8));
				break;
			}
			case 0x18: { // GetSystemOutputMasterVolume
				om.Initialize(0, 0, 4);
				var _return = GetSystemOutputMasterVolume();
				om.SetData(8, _return);
				break;
			}
			case 0x19: { // GetAudioVolumeDataForPlayReport
				om.Initialize(0, 0, 7);
				GetAudioVolumeDataForPlayReport(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1A: { // UpdateHeadphoneSettings
				om.Initialize(0, 0, 0);
				UpdateHeadphoneSettings(im.GetBytes(8, 0x1));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Audioctrl.Detail.IAudioController");
		}
	}
}

