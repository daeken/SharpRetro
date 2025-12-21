using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Hid;
public partial class IActiveVibrationDeviceList : _IActiveVibrationDeviceList_Base;
public abstract class _IActiveVibrationDeviceList_Base : IpcInterface {
	protected virtual void ActivateVibrationDevice(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IActiveVibrationDeviceList.ActivateVibrationDevice");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // ActivateVibrationDevice
				om.Initialize(0, 0, 0);
				ActivateVibrationDevice(im.GetData<uint>(8));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Hid.IActiveVibrationDeviceList");
		}
	}
}

public partial class IAppletResource : _IAppletResource_Base;
public abstract class _IAppletResource_Base : IpcInterface {
	protected virtual KObject GetSharedMemoryHandle() =>
		throw new NotImplementedException("Nn.Hid.IAppletResource.GetSharedMemoryHandle not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetSharedMemoryHandle
				om.Initialize(0, 1, 0);
				var _return = GetSharedMemoryHandle();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Hid.IAppletResource");
		}
	}
}

public partial class IHidDebugServer : _IHidDebugServer_Base;
public abstract class _IHidDebugServer_Base : IpcInterface {
	protected virtual void DeactivateDebugPad() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.DeactivateDebugPad");
	protected virtual void SetDebugPadAutoPilotState(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.SetDebugPadAutoPilotState");
	protected virtual void UnsetDebugPadAutoPilotState() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.UnsetDebugPadAutoPilotState");
	protected virtual void DeactivateTouchScreen() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.DeactivateTouchScreen");
	protected virtual void SetTouchScreenAutoPilotState(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.SetTouchScreenAutoPilotState");
	protected virtual void UnsetTouchScreenAutoPilotState() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.UnsetTouchScreenAutoPilotState");
	protected virtual void DeactivateMouse() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.DeactivateMouse");
	protected virtual void SetMouseAutoPilotState(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.SetMouseAutoPilotState");
	protected virtual void UnsetMouseAutoPilotState() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.UnsetMouseAutoPilotState");
	protected virtual void DeactivateKeyboard() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.DeactivateKeyboard");
	protected virtual void SetKeyboardAutoPilotState(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.SetKeyboardAutoPilotState");
	protected virtual void UnsetKeyboardAutoPilotState() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.UnsetKeyboardAutoPilotState");
	protected virtual void DeactivateXpad(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.DeactivateXpad");
	protected virtual void SetXpadAutoPilotState(uint _0, byte[] _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.SetXpadAutoPilotState");
	protected virtual void UnsetXpadAutoPilotState(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.UnsetXpadAutoPilotState");
	protected virtual void DeactivateJoyXpad(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.DeactivateJoyXpad");
	protected virtual void DeactivateGesture() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.DeactivateGesture");
	protected virtual void DeactivateHomeButton() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.DeactivateHomeButton");
	protected virtual void SetHomeButtonAutoPilotState(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.SetHomeButtonAutoPilotState");
	protected virtual void UnsetHomeButtonAutoPilotState() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.UnsetHomeButtonAutoPilotState");
	protected virtual void DeactivateSleepButton() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.DeactivateSleepButton");
	protected virtual void SetSleepButtonAutoPilotState(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.SetSleepButtonAutoPilotState");
	protected virtual void UnsetSleepButtonAutoPilotState() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.UnsetSleepButtonAutoPilotState");
	protected virtual void DeactivateInputDetector() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.DeactivateInputDetector");
	protected virtual void DeactivateCaptureButton() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.DeactivateCaptureButton");
	protected virtual void SetCaptureButtonAutoPilotState(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.SetCaptureButtonAutoPilotState");
	protected virtual void UnsetCaptureButtonAutoPilotState() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.UnsetCaptureButtonAutoPilotState");
	protected virtual void SetShiftAccelerometerCalibrationValue(uint _0, float _1, float _2, ulong _3, ulong _4) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.SetShiftAccelerometerCalibrationValue");
	protected virtual void GetShiftAccelerometerCalibrationValue(uint _0, ulong _1, ulong _2, out float _3, out float _4) =>
		throw new NotImplementedException("Nn.Hid.IHidDebugServer.GetShiftAccelerometerCalibrationValue not implemented");
	protected virtual void SetShiftGyroscopeCalibrationValue(uint _0, float _1, float _2, ulong _3, ulong _4) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.SetShiftGyroscopeCalibrationValue");
	protected virtual void GetShiftGyroscopeCalibrationValue(uint _0, ulong _1, ulong _2, out float _3, out float _4) =>
		throw new NotImplementedException("Nn.Hid.IHidDebugServer.GetShiftGyroscopeCalibrationValue not implemented");
	protected virtual void DeactivateConsoleSixAxisSensor() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.DeactivateConsoleSixAxisSensor");
	protected virtual void GetConsoleSixAxisSensorSamplingFrequency() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.GetConsoleSixAxisSensorSamplingFrequency");
	protected virtual void DeactivateSevenSixAxisSensor() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.DeactivateSevenSixAxisSensor");
	protected virtual void ActivateFirmwareUpdate() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.ActivateFirmwareUpdate");
	protected virtual void DeactivateFirmwareUpdate() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.DeactivateFirmwareUpdate");
	protected virtual void StartFirmwareUpdate(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.StartFirmwareUpdate");
	protected virtual void GetFirmwareUpdateStage(out ulong _0, out ulong _1) =>
		throw new NotImplementedException("Nn.Hid.IHidDebugServer.GetFirmwareUpdateStage not implemented");
	protected virtual void GetFirmwareVersion(uint _0, uint _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Hid.IHidDebugServer.GetFirmwareVersion not implemented");
	protected virtual void GetDestinationFirmwareVersion(uint _0, uint _1, out byte[] _2) =>
		throw new NotImplementedException("Nn.Hid.IHidDebugServer.GetDestinationFirmwareVersion not implemented");
	protected virtual void DiscardFirmwareInfoCacheForRevert() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.DiscardFirmwareInfoCacheForRevert");
	protected virtual void StartFirmwareUpdateForRevert(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.StartFirmwareUpdateForRevert");
	protected virtual void GetAvailableFirmwareVersionForRevert(ulong _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Hid.IHidDebugServer.GetAvailableFirmwareVersionForRevert not implemented");
	protected virtual byte IsFirmwareUpdatingDevice(ulong _0) =>
		throw new NotImplementedException("Nn.Hid.IHidDebugServer.IsFirmwareUpdatingDevice not implemented");
	protected virtual void UpdateControllerColor(byte[] _0, byte[] _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.UpdateControllerColor");
	protected virtual void ConnectUsbPadsAsync() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.ConnectUsbPadsAsync");
	protected virtual void DisconnectUsbPadsAsync() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.DisconnectUsbPadsAsync");
	protected virtual void UpdateDesignInfo() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.UpdateDesignInfo");
	protected virtual void GetUniquePadDriverState() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.GetUniquePadDriverState");
	protected virtual void GetSixAxisSensorDriverStates() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.GetSixAxisSensorDriverStates");
	protected virtual void GetAbstractedPadHandles() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.GetAbstractedPadHandles");
	protected virtual void GetAbstractedPadState() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.GetAbstractedPadState");
	protected virtual void GetAbstractedPadsState() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.GetAbstractedPadsState");
	protected virtual void SetAutoPilotVirtualPadState() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.SetAutoPilotVirtualPadState");
	protected virtual void UnsetAutoPilotVirtualPadState() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.UnsetAutoPilotVirtualPadState");
	protected virtual void UnsetAllAutoPilotVirtualPadState() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.UnsetAllAutoPilotVirtualPadState");
	protected virtual void AddRegisteredDevice() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.AddRegisteredDevice");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // DeactivateDebugPad
				om.Initialize(0, 0, 0);
				DeactivateDebugPad();
				break;
			}
			case 0x1: { // SetDebugPadAutoPilotState
				om.Initialize(0, 0, 0);
				SetDebugPadAutoPilotState(im.GetBytes(8, 0x18));
				break;
			}
			case 0x2: { // UnsetDebugPadAutoPilotState
				om.Initialize(0, 0, 0);
				UnsetDebugPadAutoPilotState();
				break;
			}
			case 0xA: { // DeactivateTouchScreen
				om.Initialize(0, 0, 0);
				DeactivateTouchScreen();
				break;
			}
			case 0xB: { // SetTouchScreenAutoPilotState
				om.Initialize(0, 0, 0);
				SetTouchScreenAutoPilotState(im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0xC: { // UnsetTouchScreenAutoPilotState
				om.Initialize(0, 0, 0);
				UnsetTouchScreenAutoPilotState();
				break;
			}
			case 0x14: { // DeactivateMouse
				om.Initialize(0, 0, 0);
				DeactivateMouse();
				break;
			}
			case 0x15: { // SetMouseAutoPilotState
				om.Initialize(0, 0, 0);
				SetMouseAutoPilotState(im.GetBytes(8, 0x1C));
				break;
			}
			case 0x16: { // UnsetMouseAutoPilotState
				om.Initialize(0, 0, 0);
				UnsetMouseAutoPilotState();
				break;
			}
			case 0x1E: { // DeactivateKeyboard
				om.Initialize(0, 0, 0);
				DeactivateKeyboard();
				break;
			}
			case 0x1F: { // SetKeyboardAutoPilotState
				om.Initialize(0, 0, 0);
				SetKeyboardAutoPilotState(im.GetBytes(8, 0x28));
				break;
			}
			case 0x20: { // UnsetKeyboardAutoPilotState
				om.Initialize(0, 0, 0);
				UnsetKeyboardAutoPilotState();
				break;
			}
			case 0x32: { // DeactivateXpad
				om.Initialize(0, 0, 0);
				DeactivateXpad(im.GetData<uint>(8));
				break;
			}
			case 0x33: { // SetXpadAutoPilotState
				om.Initialize(0, 0, 0);
				SetXpadAutoPilotState(im.GetData<uint>(8), im.GetBytes(12, 0x1C));
				break;
			}
			case 0x34: { // UnsetXpadAutoPilotState
				om.Initialize(0, 0, 0);
				UnsetXpadAutoPilotState(im.GetData<uint>(8));
				break;
			}
			case 0x3C: { // DeactivateJoyXpad
				om.Initialize(0, 0, 0);
				DeactivateJoyXpad(im.GetData<uint>(8));
				break;
			}
			case 0x5B: { // DeactivateGesture
				om.Initialize(0, 0, 0);
				DeactivateGesture();
				break;
			}
			case 0x6E: { // DeactivateHomeButton
				om.Initialize(0, 0, 0);
				DeactivateHomeButton();
				break;
			}
			case 0x6F: { // SetHomeButtonAutoPilotState
				om.Initialize(0, 0, 0);
				SetHomeButtonAutoPilotState(im.GetData<ulong>(8));
				break;
			}
			case 0x70: { // UnsetHomeButtonAutoPilotState
				om.Initialize(0, 0, 0);
				UnsetHomeButtonAutoPilotState();
				break;
			}
			case 0x78: { // DeactivateSleepButton
				om.Initialize(0, 0, 0);
				DeactivateSleepButton();
				break;
			}
			case 0x79: { // SetSleepButtonAutoPilotState
				om.Initialize(0, 0, 0);
				SetSleepButtonAutoPilotState(im.GetData<ulong>(8));
				break;
			}
			case 0x7A: { // UnsetSleepButtonAutoPilotState
				om.Initialize(0, 0, 0);
				UnsetSleepButtonAutoPilotState();
				break;
			}
			case 0x7B: { // DeactivateInputDetector
				om.Initialize(0, 0, 0);
				DeactivateInputDetector();
				break;
			}
			case 0x82: { // DeactivateCaptureButton
				om.Initialize(0, 0, 0);
				DeactivateCaptureButton();
				break;
			}
			case 0x83: { // SetCaptureButtonAutoPilotState
				om.Initialize(0, 0, 0);
				SetCaptureButtonAutoPilotState(im.GetData<ulong>(8));
				break;
			}
			case 0x84: { // UnsetCaptureButtonAutoPilotState
				om.Initialize(0, 0, 0);
				UnsetCaptureButtonAutoPilotState();
				break;
			}
			case 0x85: { // SetShiftAccelerometerCalibrationValue
				om.Initialize(0, 0, 0);
				SetShiftAccelerometerCalibrationValue(im.GetData<uint>(8), im.GetData<float>(12), im.GetData<float>(16), im.GetData<ulong>(24), im.Pid);
				break;
			}
			case 0x86: { // GetShiftAccelerometerCalibrationValue
				om.Initialize(0, 0, 8);
				GetShiftAccelerometerCalibrationValue(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid, out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x87: { // SetShiftGyroscopeCalibrationValue
				om.Initialize(0, 0, 0);
				SetShiftGyroscopeCalibrationValue(im.GetData<uint>(8), im.GetData<float>(12), im.GetData<float>(16), im.GetData<ulong>(24), im.Pid);
				break;
			}
			case 0x88: { // GetShiftGyroscopeCalibrationValue
				om.Initialize(0, 0, 8);
				GetShiftGyroscopeCalibrationValue(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid, out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x8C: { // DeactivateConsoleSixAxisSensor
				om.Initialize(0, 0, 0);
				DeactivateConsoleSixAxisSensor();
				break;
			}
			case 0x8D: { // GetConsoleSixAxisSensorSamplingFrequency
				om.Initialize(0, 0, 0);
				GetConsoleSixAxisSensorSamplingFrequency();
				break;
			}
			case 0x8E: { // DeactivateSevenSixAxisSensor
				om.Initialize(0, 0, 0);
				DeactivateSevenSixAxisSensor();
				break;
			}
			case 0xC9: { // ActivateFirmwareUpdate
				om.Initialize(0, 0, 0);
				ActivateFirmwareUpdate();
				break;
			}
			case 0xCA: { // DeactivateFirmwareUpdate
				om.Initialize(0, 0, 0);
				DeactivateFirmwareUpdate();
				break;
			}
			case 0xCB: { // StartFirmwareUpdate
				om.Initialize(0, 0, 0);
				StartFirmwareUpdate(im.GetData<ulong>(8));
				break;
			}
			case 0xCC: { // GetFirmwareUpdateStage
				om.Initialize(0, 0, 16);
				GetFirmwareUpdateStage(out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0xCD: { // GetFirmwareVersion
				om.Initialize(0, 0, 16);
				GetFirmwareVersion(im.GetData<uint>(8), im.GetData<uint>(12), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xCE: { // GetDestinationFirmwareVersion
				om.Initialize(0, 0, 16);
				GetDestinationFirmwareVersion(im.GetData<uint>(8), im.GetData<uint>(12), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xCF: { // DiscardFirmwareInfoCacheForRevert
				om.Initialize(0, 0, 0);
				DiscardFirmwareInfoCacheForRevert();
				break;
			}
			case 0xD0: { // StartFirmwareUpdateForRevert
				om.Initialize(0, 0, 0);
				StartFirmwareUpdateForRevert(im.GetData<ulong>(8));
				break;
			}
			case 0xD1: { // GetAvailableFirmwareVersionForRevert
				om.Initialize(0, 0, 16);
				GetAvailableFirmwareVersionForRevert(im.GetData<ulong>(8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD2: { // IsFirmwareUpdatingDevice
				om.Initialize(0, 0, 1);
				var _return = IsFirmwareUpdatingDevice(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0xDD: { // UpdateControllerColor
				om.Initialize(0, 0, 0);
				UpdateControllerColor(im.GetBytes(8, 0x4), im.GetBytes(12, 0x4), im.GetData<ulong>(16));
				break;
			}
			case 0xDE: { // ConnectUsbPadsAsync
				om.Initialize(0, 0, 0);
				ConnectUsbPadsAsync();
				break;
			}
			case 0xDF: { // DisconnectUsbPadsAsync
				om.Initialize(0, 0, 0);
				DisconnectUsbPadsAsync();
				break;
			}
			case 0xE0: { // UpdateDesignInfo
				om.Initialize(0, 0, 0);
				UpdateDesignInfo();
				break;
			}
			case 0xE1: { // GetUniquePadDriverState
				om.Initialize(0, 0, 0);
				GetUniquePadDriverState();
				break;
			}
			case 0xE2: { // GetSixAxisSensorDriverStates
				om.Initialize(0, 0, 0);
				GetSixAxisSensorDriverStates();
				break;
			}
			case 0x12D: { // GetAbstractedPadHandles
				om.Initialize(0, 0, 0);
				GetAbstractedPadHandles();
				break;
			}
			case 0x12E: { // GetAbstractedPadState
				om.Initialize(0, 0, 0);
				GetAbstractedPadState();
				break;
			}
			case 0x12F: { // GetAbstractedPadsState
				om.Initialize(0, 0, 0);
				GetAbstractedPadsState();
				break;
			}
			case 0x141: { // SetAutoPilotVirtualPadState
				om.Initialize(0, 0, 0);
				SetAutoPilotVirtualPadState();
				break;
			}
			case 0x142: { // UnsetAutoPilotVirtualPadState
				om.Initialize(0, 0, 0);
				UnsetAutoPilotVirtualPadState();
				break;
			}
			case 0x143: { // UnsetAllAutoPilotVirtualPadState
				om.Initialize(0, 0, 0);
				UnsetAllAutoPilotVirtualPadState();
				break;
			}
			case 0x15E: { // AddRegisteredDevice
				om.Initialize(0, 0, 0);
				AddRegisteredDevice();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Hid.IHidDebugServer");
		}
	}
}

public partial class IHidServer : _IHidServer_Base;
public abstract class _IHidServer_Base : IpcInterface {
	protected virtual Nn.Hid.IAppletResource CreateAppletResource(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.CreateAppletResource not implemented");
	protected virtual void ActivateDebugPad(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ActivateDebugPad");
	protected virtual void ActivateTouchScreen(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ActivateTouchScreen");
	protected virtual void ActivateMouse(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ActivateMouse");
	protected virtual void ActivateKeyboard(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ActivateKeyboard");
	protected virtual void Unknown32(ulong _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.Unknown32");
	protected virtual KObject AcquireXpadIdEventHandle(ulong _0) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.AcquireXpadIdEventHandle not implemented");
	protected virtual void ReleaseXpadIdEventHandle(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ReleaseXpadIdEventHandle");
	protected virtual void ActivateXpad(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ActivateXpad");
	protected virtual void GetXpadIds(out long _0, Span<uint> _1) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetXpadIds not implemented");
	protected virtual void ActivateJoyXpad(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ActivateJoyXpad");
	protected virtual KObject GetJoyXpadLifoHandle(uint _0) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetJoyXpadLifoHandle not implemented");
	protected virtual void GetJoyXpadIds(out long _0, Span<uint> _1) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetJoyXpadIds not implemented");
	protected virtual void ActivateSixAxisSensor(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ActivateSixAxisSensor");
	protected virtual void DeactivateSixAxisSensor(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.DeactivateSixAxisSensor");
	protected virtual KObject GetSixAxisSensorLifoHandle(uint _0) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetSixAxisSensorLifoHandle not implemented");
	protected virtual void ActivateJoySixAxisSensor(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ActivateJoySixAxisSensor");
	protected virtual void DeactivateJoySixAxisSensor(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.DeactivateJoySixAxisSensor");
	protected virtual KObject GetJoySixAxisSensorLifoHandle(uint _0) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetJoySixAxisSensorLifoHandle not implemented");
	protected virtual void StartSixAxisSensor(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.StartSixAxisSensor");
	protected virtual void StopSixAxisSensor(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.StopSixAxisSensor");
	protected virtual bool IsSixAxisSensorFusionEnabled(uint _0, ulong _1, ulong _2) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.IsSixAxisSensorFusionEnabled not implemented");
	protected virtual void EnableSixAxisSensorFusion(bool _0, uint _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.EnableSixAxisSensorFusion");
	protected virtual void SetSixAxisSensorFusionParameters(uint _0, float _1, float _2, ulong _3, ulong _4) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SetSixAxisSensorFusionParameters");
	protected virtual void GetSixAxisSensorFusionParameters(uint _0, ulong _1, ulong _2, out float _3, out float _4) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetSixAxisSensorFusionParameters not implemented");
	protected virtual void ResetSixAxisSensorFusionParameters(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ResetSixAxisSensorFusionParameters");
	protected virtual void SetAccelerometerParameters(uint _0, float _1, float _2, ulong _3, ulong _4) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SetAccelerometerParameters");
	protected virtual void GetAccelerometerParameters(uint _0, ulong _1, ulong _2, out float _3, out float _4) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetAccelerometerParameters not implemented");
	protected virtual void ResetAccelerometerParameters(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ResetAccelerometerParameters");
	protected virtual void SetAccelerometerPlayMode(uint _0, uint _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SetAccelerometerPlayMode");
	protected virtual uint GetAccelerometerPlayMode(uint _0, ulong _1, ulong _2) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetAccelerometerPlayMode not implemented");
	protected virtual void ResetAccelerometerPlayMode(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ResetAccelerometerPlayMode");
	protected virtual void SetGyroscopeZeroDriftMode(uint _0, uint _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SetGyroscopeZeroDriftMode");
	protected virtual uint GetGyroscopeZeroDriftMode(uint _0, ulong _1, ulong _2) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetGyroscopeZeroDriftMode not implemented");
	protected virtual void ResetGyroscopeZeroDriftMode(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ResetGyroscopeZeroDriftMode");
	protected virtual bool IsSixAxisSensorAtRest(uint _0, ulong _1, ulong _2) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.IsSixAxisSensorAtRest not implemented");
	protected virtual bool Unknown83(ulong _0, ulong _1, ulong _2) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.Unknown83 not implemented");
	protected virtual void ActivateGesture(int _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ActivateGesture");
	protected virtual void SetSupportedNpadStyleSet(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SetSupportedNpadStyleSet");
	protected virtual uint GetSupportedNpadStyleSet(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetSupportedNpadStyleSet not implemented");
	protected virtual void SetSupportedNpadIdType(ulong _0, ulong _1, Span<uint> _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SetSupportedNpadIdType");
	protected virtual void ActivateNpad(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ActivateNpad");
	protected virtual void DeactivateNpad(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.DeactivateNpad");
	protected virtual KObject AcquireNpadStyleSetUpdateEventHandle(uint _0, ulong _1, ulong _2, ulong _3) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.AcquireNpadStyleSetUpdateEventHandle not implemented");
	protected virtual void DisconnectNpad(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.DisconnectNpad");
	protected virtual ulong GetPlayerLedPattern(uint _0) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetPlayerLedPattern not implemented");
	protected virtual void SetNpadJoyHoldType(ulong _0, long _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SetNpadJoyHoldType");
	protected virtual long GetNpadJoyHoldType(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetNpadJoyHoldType not implemented");
	protected virtual void SetNpadJoyAssignmentModeSingleByDefault(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SetNpadJoyAssignmentModeSingleByDefault");
	protected virtual void SetNpadJoyAssignmentModeSingle(uint _0, ulong _1, long _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SetNpadJoyAssignmentModeSingle");
	protected virtual void SetNpadJoyAssignmentModeDual(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SetNpadJoyAssignmentModeDual");
	protected virtual void MergeSingleJoyAsDualJoy(uint _0, uint _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.MergeSingleJoyAsDualJoy");
	protected virtual void StartLrAssignmentMode(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.StartLrAssignmentMode");
	protected virtual void StopLrAssignmentMode(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.StopLrAssignmentMode");
	protected virtual void SetNpadHandheldActivationMode(ulong _0, long _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SetNpadHandheldActivationMode");
	protected virtual long GetNpadHandheldActivationMode(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetNpadHandheldActivationMode not implemented");
	protected virtual void SwapNpadAssignment(uint _0, uint _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SwapNpadAssignment");
	protected virtual bool IsUnintendedHomeButtonInputProtectionEnabled(uint _0, ulong _1, ulong _2) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.IsUnintendedHomeButtonInputProtectionEnabled not implemented");
	protected virtual void EnableUnintendedHomeButtonInputProtection(bool _0, uint _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.EnableUnintendedHomeButtonInputProtection");
	protected virtual void SetNpadJoyAssignmentModeSingleWithDestination(uint _0, ulong _1, ulong _2, ulong _3, out bool _4, out uint _5) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.SetNpadJoyAssignmentModeSingleWithDestination not implemented");
	protected virtual void GetVibrationDeviceInfo(uint _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetVibrationDeviceInfo not implemented");
	protected virtual void SendVibrationValue(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SendVibrationValue");
	protected virtual void GetActualVibrationValue(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.GetActualVibrationValue");
	protected virtual Nn.Hid.IActiveVibrationDeviceList CreateActiveVibrationDeviceList() =>
		throw new NotImplementedException("Nn.Hid.IHidServer.CreateActiveVibrationDeviceList not implemented");
	protected virtual void PermitVibration(bool _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.PermitVibration");
	protected virtual bool IsVibrationPermitted() =>
		throw new NotImplementedException("Nn.Hid.IHidServer.IsVibrationPermitted not implemented");
	protected virtual void SendVibrationValues(ulong _0, Span<uint> _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SendVibrationValues");
	protected virtual void SendVibrationGcErmCommand(uint _0, ulong _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SendVibrationGcErmCommand");
	protected virtual ulong GetActualVibrationGcErmCommand(uint _0, ulong _1, ulong _2) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetActualVibrationGcErmCommand not implemented");
	protected virtual void BeginPermitVibrationSession(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.BeginPermitVibrationSession");
	protected virtual void EndPermitVibrationSession() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.EndPermitVibrationSession");
	protected virtual void ActivateConsoleSixAxisSensor(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ActivateConsoleSixAxisSensor");
	protected virtual void StartConsoleSixAxisSensor(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.StartConsoleSixAxisSensor");
	protected virtual void StopConsoleSixAxisSensor(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.StopConsoleSixAxisSensor");
	protected virtual void ActivateSevenSixAxisSensor(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ActivateSevenSixAxisSensor");
	protected virtual void StartSevenSixAxisSensor(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.StartSevenSixAxisSensor");
	protected virtual void StopSevenSixAxisSensor(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.StopSevenSixAxisSensor");
	protected virtual void InitializeSevenSixAxisSensor(uint _0, ulong _1, uint _2, ulong _3, ulong _4, ulong _5) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.InitializeSevenSixAxisSensor");
	protected virtual void FinalizeSevenSixAxisSensor(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.FinalizeSevenSixAxisSensor");
	protected virtual void SetSevenSixAxisSensorFusionStrength(float _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SetSevenSixAxisSensorFusionStrength");
	protected virtual float GetSevenSixAxisSensorFusionStrength(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetSevenSixAxisSensorFusionStrength not implemented");
	protected virtual void Unknown310(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.Unknown310");
	protected virtual bool IsUsbFullKeyControllerEnabled() =>
		throw new NotImplementedException("Nn.Hid.IHidServer.IsUsbFullKeyControllerEnabled not implemented");
	protected virtual void EnableUsbFullKeyController(bool _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.EnableUsbFullKeyController");
	protected virtual bool IsUsbFullKeyControllerConnected(uint _0) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.IsUsbFullKeyControllerConnected not implemented");
	protected virtual bool HasBattery(uint _0) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.HasBattery not implemented");
	protected virtual void HasLeftRightBattery(uint _0, out bool _1, out bool _2) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.HasLeftRightBattery not implemented");
	protected virtual byte GetNpadInterfaceType(uint _0) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetNpadInterfaceType not implemented");
	protected virtual void GetNpadLeftRightInterfaceType(uint _0, out byte _1, out byte _2) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetNpadLeftRightInterfaceType not implemented");
	protected virtual ulong GetPalmaConnectionHandle(uint _0, ulong _1, ulong _2) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetPalmaConnectionHandle not implemented");
	protected virtual void InitializePalma(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.InitializePalma");
	protected virtual KObject AcquirePalmaOperationCompleteEvent(ulong _0) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.AcquirePalmaOperationCompleteEvent not implemented");
	protected virtual void GetPalmaOperationInfo(ulong _0, out ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetPalmaOperationInfo not implemented");
	protected virtual void PlayPalmaActivity(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.PlayPalmaActivity");
	protected virtual void SetPalmaFrModeType(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SetPalmaFrModeType");
	protected virtual void ReadPalmaStep(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ReadPalmaStep");
	protected virtual void EnablePalmaStep(bool _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.EnablePalmaStep");
	protected virtual void ResetPalmaStep(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ResetPalmaStep");
	protected virtual void ReadPalmaApplicationSection(ulong _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ReadPalmaApplicationSection");
	protected virtual void WritePalmaApplicationSection(ulong _0, ulong _1, Span<byte> _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.WritePalmaApplicationSection");
	protected virtual void ReadPalmaUniqueCode(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ReadPalmaUniqueCode");
	protected virtual void SetPalmaUniqueCodeInvalid(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SetPalmaUniqueCodeInvalid");
	protected virtual void WritePalmaActivityEntry(ulong _0, ulong _1, ulong _2, ulong _3, ulong _4) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.WritePalmaActivityEntry");
	protected virtual void WritePalmaRgbLedPatternEntry(ulong _0, Span<byte> _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.WritePalmaRgbLedPatternEntry");
	protected virtual void WritePalmaWaveEntry(ulong _0, ulong _1, KObject _2, ulong _3, ulong _4, ulong _5) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.WritePalmaWaveEntry");
	protected virtual void SetPalmaDataBaseIdentificationVersion(uint _0, ulong _1, int _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SetPalmaDataBaseIdentificationVersion");
	protected virtual void GetPalmaDataBaseIdentificationVersion(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.GetPalmaDataBaseIdentificationVersion");
	protected virtual void SuspendPalmaFeature(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SuspendPalmaFeature");
	protected virtual void GetPalmaOperationResult(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.GetPalmaOperationResult");
	protected virtual void ReadPalmaPlayLog(ulong _0, ushort _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ReadPalmaPlayLog");
	protected virtual void ResetPalmaPlayLog(ulong _0, ushort _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ResetPalmaPlayLog");
	protected virtual void SetIsPalmaAllConnectable(ulong _0, bool _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SetIsPalmaAllConnectable");
	protected virtual void SetIsPalmaPairedConnectable(ulong _0, bool _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SetIsPalmaPairedConnectable");
	protected virtual void PairPalma(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.PairPalma");
	protected virtual void SetPalmaBoostMode(bool _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SetPalmaBoostMode");
	protected virtual void SetNpadCommunicationMode(ulong _0, long _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SetNpadCommunicationMode");
	protected virtual long GetNpadCommunicationMode() =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetNpadCommunicationMode not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateAppletResource
				om.Initialize(1, 0, 0);
				var _return = CreateAppletResource(im.GetData<ulong>(8), im.Pid);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // ActivateDebugPad
				om.Initialize(0, 0, 0);
				ActivateDebugPad(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0xB: { // ActivateTouchScreen
				om.Initialize(0, 0, 0);
				ActivateTouchScreen(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x15: { // ActivateMouse
				om.Initialize(0, 0, 0);
				ActivateMouse(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x1F: { // ActivateKeyboard
				om.Initialize(0, 0, 0);
				ActivateKeyboard(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x20: { // Unknown32
				om.Initialize(0, 0, 0);
				Unknown32(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x28: { // AcquireXpadIdEventHandle
				om.Initialize(0, 1, 0);
				var _return = AcquireXpadIdEventHandle(im.GetData<ulong>(8));
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x29: { // ReleaseXpadIdEventHandle
				om.Initialize(0, 0, 0);
				ReleaseXpadIdEventHandle(im.GetData<ulong>(8));
				break;
			}
			case 0x33: { // ActivateXpad
				om.Initialize(0, 0, 0);
				ActivateXpad(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x37: { // GetXpadIds
				om.Initialize(0, 0, 8);
				GetXpadIds(out var _0, im.GetSpan<uint>(0xA, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x38: { // ActivateJoyXpad
				om.Initialize(0, 0, 0);
				ActivateJoyXpad(im.GetData<uint>(8));
				break;
			}
			case 0x3A: { // GetJoyXpadLifoHandle
				om.Initialize(0, 1, 0);
				var _return = GetJoyXpadLifoHandle(im.GetData<uint>(8));
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x3B: { // GetJoyXpadIds
				om.Initialize(0, 0, 8);
				GetJoyXpadIds(out var _0, im.GetSpan<uint>(0xA, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x3C: { // ActivateSixAxisSensor
				om.Initialize(0, 0, 0);
				ActivateSixAxisSensor(im.GetData<uint>(8));
				break;
			}
			case 0x3D: { // DeactivateSixAxisSensor
				om.Initialize(0, 0, 0);
				DeactivateSixAxisSensor(im.GetData<uint>(8));
				break;
			}
			case 0x3E: { // GetSixAxisSensorLifoHandle
				om.Initialize(0, 1, 0);
				var _return = GetSixAxisSensorLifoHandle(im.GetData<uint>(8));
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x3F: { // ActivateJoySixAxisSensor
				om.Initialize(0, 0, 0);
				ActivateJoySixAxisSensor(im.GetData<uint>(8));
				break;
			}
			case 0x40: { // DeactivateJoySixAxisSensor
				om.Initialize(0, 0, 0);
				DeactivateJoySixAxisSensor(im.GetData<uint>(8));
				break;
			}
			case 0x41: { // GetJoySixAxisSensorLifoHandle
				om.Initialize(0, 1, 0);
				var _return = GetJoySixAxisSensorLifoHandle(im.GetData<uint>(8));
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x42: { // StartSixAxisSensor
				om.Initialize(0, 0, 0);
				StartSixAxisSensor(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x43: { // StopSixAxisSensor
				om.Initialize(0, 0, 0);
				StopSixAxisSensor(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x44: { // IsSixAxisSensorFusionEnabled
				om.Initialize(0, 0, 4);
				var _return = IsSixAxisSensorFusionEnabled(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				om.SetData(8, _return);
				break;
			}
			case 0x45: { // EnableSixAxisSensorFusion
				om.Initialize(0, 0, 0);
				EnableSixAxisSensorFusion(im.GetData<bool>(8), im.GetData<uint>(12), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x46: { // SetSixAxisSensorFusionParameters
				om.Initialize(0, 0, 0);
				SetSixAxisSensorFusionParameters(im.GetData<uint>(8), im.GetData<float>(12), im.GetData<float>(16), im.GetData<ulong>(24), im.Pid);
				break;
			}
			case 0x47: { // GetSixAxisSensorFusionParameters
				om.Initialize(0, 0, 8);
				GetSixAxisSensorFusionParameters(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid, out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x48: { // ResetSixAxisSensorFusionParameters
				om.Initialize(0, 0, 0);
				ResetSixAxisSensorFusionParameters(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x49: { // SetAccelerometerParameters
				om.Initialize(0, 0, 0);
				SetAccelerometerParameters(im.GetData<uint>(8), im.GetData<float>(12), im.GetData<float>(16), im.GetData<ulong>(24), im.Pid);
				break;
			}
			case 0x4A: { // GetAccelerometerParameters
				om.Initialize(0, 0, 8);
				GetAccelerometerParameters(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid, out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x4B: { // ResetAccelerometerParameters
				om.Initialize(0, 0, 0);
				ResetAccelerometerParameters(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x4C: { // SetAccelerometerPlayMode
				om.Initialize(0, 0, 0);
				SetAccelerometerPlayMode(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x4D: { // GetAccelerometerPlayMode
				om.Initialize(0, 0, 4);
				var _return = GetAccelerometerPlayMode(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				om.SetData(8, _return);
				break;
			}
			case 0x4E: { // ResetAccelerometerPlayMode
				om.Initialize(0, 0, 0);
				ResetAccelerometerPlayMode(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x4F: { // SetGyroscopeZeroDriftMode
				om.Initialize(0, 0, 0);
				SetGyroscopeZeroDriftMode(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x50: { // GetGyroscopeZeroDriftMode
				om.Initialize(0, 0, 4);
				var _return = GetGyroscopeZeroDriftMode(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				om.SetData(8, _return);
				break;
			}
			case 0x51: { // ResetGyroscopeZeroDriftMode
				om.Initialize(0, 0, 0);
				ResetGyroscopeZeroDriftMode(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x52: { // IsSixAxisSensorAtRest
				om.Initialize(0, 0, 4);
				var _return = IsSixAxisSensorAtRest(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				om.SetData(8, _return);
				break;
			}
			case 0x53: { // Unknown83
				om.Initialize(0, 0, 4);
				var _return = Unknown83(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid);
				om.SetData(8, _return);
				break;
			}
			case 0x5B: { // ActivateGesture
				om.Initialize(0, 0, 0);
				ActivateGesture(im.GetData<int>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x64: { // SetSupportedNpadStyleSet
				om.Initialize(0, 0, 0);
				SetSupportedNpadStyleSet(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x65: { // GetSupportedNpadStyleSet
				om.Initialize(0, 0, 4);
				var _return = GetSupportedNpadStyleSet(im.GetData<ulong>(8), im.Pid);
				om.SetData(8, _return);
				break;
			}
			case 0x66: { // SetSupportedNpadIdType
				om.Initialize(0, 0, 0);
				SetSupportedNpadIdType(im.GetData<ulong>(8), im.Pid, im.GetSpan<uint>(0x9, 0));
				break;
			}
			case 0x67: { // ActivateNpad
				om.Initialize(0, 0, 0);
				ActivateNpad(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x68: { // DeactivateNpad
				om.Initialize(0, 0, 0);
				DeactivateNpad(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x6A: { // AcquireNpadStyleSetUpdateEventHandle
				om.Initialize(0, 1, 0);
				var _return = AcquireNpadStyleSetUpdateEventHandle(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), im.Pid);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x6B: { // DisconnectNpad
				om.Initialize(0, 0, 0);
				DisconnectNpad(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x6C: { // GetPlayerLedPattern
				om.Initialize(0, 0, 8);
				var _return = GetPlayerLedPattern(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x78: { // SetNpadJoyHoldType
				om.Initialize(0, 0, 0);
				SetNpadJoyHoldType(im.GetData<ulong>(8), im.GetData<long>(16), im.Pid);
				break;
			}
			case 0x79: { // GetNpadJoyHoldType
				om.Initialize(0, 0, 8);
				var _return = GetNpadJoyHoldType(im.GetData<ulong>(8), im.Pid);
				om.SetData(8, _return);
				break;
			}
			case 0x7A: { // SetNpadJoyAssignmentModeSingleByDefault
				om.Initialize(0, 0, 0);
				SetNpadJoyAssignmentModeSingleByDefault(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x7B: { // SetNpadJoyAssignmentModeSingle
				om.Initialize(0, 0, 0);
				SetNpadJoyAssignmentModeSingle(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<long>(24), im.Pid);
				break;
			}
			case 0x7C: { // SetNpadJoyAssignmentModeDual
				om.Initialize(0, 0, 0);
				SetNpadJoyAssignmentModeDual(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x7D: { // MergeSingleJoyAsDualJoy
				om.Initialize(0, 0, 0);
				MergeSingleJoyAsDualJoy(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x7E: { // StartLrAssignmentMode
				om.Initialize(0, 0, 0);
				StartLrAssignmentMode(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x7F: { // StopLrAssignmentMode
				om.Initialize(0, 0, 0);
				StopLrAssignmentMode(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x80: { // SetNpadHandheldActivationMode
				om.Initialize(0, 0, 0);
				SetNpadHandheldActivationMode(im.GetData<ulong>(8), im.GetData<long>(16), im.Pid);
				break;
			}
			case 0x81: { // GetNpadHandheldActivationMode
				om.Initialize(0, 0, 8);
				var _return = GetNpadHandheldActivationMode(im.GetData<ulong>(8), im.Pid);
				om.SetData(8, _return);
				break;
			}
			case 0x82: { // SwapNpadAssignment
				om.Initialize(0, 0, 0);
				SwapNpadAssignment(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x83: { // IsUnintendedHomeButtonInputProtectionEnabled
				om.Initialize(0, 0, 4);
				var _return = IsUnintendedHomeButtonInputProtectionEnabled(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				om.SetData(8, _return);
				break;
			}
			case 0x84: { // EnableUnintendedHomeButtonInputProtection
				om.Initialize(0, 0, 0);
				EnableUnintendedHomeButtonInputProtection(im.GetData<bool>(8), im.GetData<uint>(12), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x85: { // SetNpadJoyAssignmentModeSingleWithDestination
				om.Initialize(0, 0, 8);
				SetNpadJoyAssignmentModeSingleWithDestination(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), im.Pid, out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0xC8: { // GetVibrationDeviceInfo
				om.Initialize(0, 0, 8);
				GetVibrationDeviceInfo(im.GetData<uint>(8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC9: { // SendVibrationValue
				om.Initialize(0, 0, 0);
				SendVibrationValue(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0xCA: { // GetActualVibrationValue
				om.Initialize(0, 0, 0);
				GetActualVibrationValue(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0xCB: { // CreateActiveVibrationDeviceList
				om.Initialize(1, 0, 0);
				var _return = CreateActiveVibrationDeviceList();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xCC: { // PermitVibration
				om.Initialize(0, 0, 0);
				PermitVibration(im.GetData<bool>(8));
				break;
			}
			case 0xCD: { // IsVibrationPermitted
				om.Initialize(0, 0, 4);
				var _return = IsVibrationPermitted();
				om.SetData(8, _return);
				break;
			}
			case 0xCE: { // SendVibrationValues
				om.Initialize(0, 0, 0);
				SendVibrationValues(im.GetData<ulong>(8), im.GetSpan<uint>(0x9, 0), im.GetSpan<byte>(0x9, 1));
				break;
			}
			case 0xCF: { // SendVibrationGcErmCommand
				om.Initialize(0, 0, 0);
				SendVibrationGcErmCommand(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), im.Pid);
				break;
			}
			case 0xD0: { // GetActualVibrationGcErmCommand
				om.Initialize(0, 0, 8);
				var _return = GetActualVibrationGcErmCommand(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				om.SetData(8, _return);
				break;
			}
			case 0xD1: { // BeginPermitVibrationSession
				om.Initialize(0, 0, 0);
				BeginPermitVibrationSession(im.GetData<ulong>(8));
				break;
			}
			case 0xD2: { // EndPermitVibrationSession
				om.Initialize(0, 0, 0);
				EndPermitVibrationSession();
				break;
			}
			case 0x12C: { // ActivateConsoleSixAxisSensor
				om.Initialize(0, 0, 0);
				ActivateConsoleSixAxisSensor(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x12D: { // StartConsoleSixAxisSensor
				om.Initialize(0, 0, 0);
				StartConsoleSixAxisSensor(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x12E: { // StopConsoleSixAxisSensor
				om.Initialize(0, 0, 0);
				StopConsoleSixAxisSensor(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x12F: { // ActivateSevenSixAxisSensor
				om.Initialize(0, 0, 0);
				ActivateSevenSixAxisSensor(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x130: { // StartSevenSixAxisSensor
				om.Initialize(0, 0, 0);
				StartSevenSixAxisSensor(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x131: { // StopSevenSixAxisSensor
				om.Initialize(0, 0, 0);
				StopSevenSixAxisSensor(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x132: { // InitializeSevenSixAxisSensor
				om.Initialize(0, 0, 0);
				InitializeSevenSixAxisSensor(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<uint>(24), im.GetData<ulong>(32), im.GetData<ulong>(40), im.Pid);
				break;
			}
			case 0x133: { // FinalizeSevenSixAxisSensor
				om.Initialize(0, 0, 0);
				FinalizeSevenSixAxisSensor(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x134: { // SetSevenSixAxisSensorFusionStrength
				om.Initialize(0, 0, 0);
				SetSevenSixAxisSensorFusionStrength(im.GetData<float>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x135: { // GetSevenSixAxisSensorFusionStrength
				om.Initialize(0, 0, 4);
				var _return = GetSevenSixAxisSensorFusionStrength(im.GetData<ulong>(8), im.Pid);
				om.SetData(8, _return);
				break;
			}
			case 0x136: { // Unknown310
				om.Initialize(0, 0, 0);
				Unknown310(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x190: { // IsUsbFullKeyControllerEnabled
				om.Initialize(0, 0, 4);
				var _return = IsUsbFullKeyControllerEnabled();
				om.SetData(8, _return);
				break;
			}
			case 0x191: { // EnableUsbFullKeyController
				om.Initialize(0, 0, 0);
				EnableUsbFullKeyController(im.GetData<bool>(8));
				break;
			}
			case 0x192: { // IsUsbFullKeyControllerConnected
				om.Initialize(0, 0, 4);
				var _return = IsUsbFullKeyControllerConnected(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x193: { // HasBattery
				om.Initialize(0, 0, 4);
				var _return = HasBattery(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x194: { // HasLeftRightBattery
				om.Initialize(0, 0, 8);
				HasLeftRightBattery(im.GetData<uint>(8), out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x195: { // GetNpadInterfaceType
				om.Initialize(0, 0, 1);
				var _return = GetNpadInterfaceType(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x196: { // GetNpadLeftRightInterfaceType
				om.Initialize(0, 0, 2);
				GetNpadLeftRightInterfaceType(im.GetData<uint>(8), out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(9, _1);
				break;
			}
			case 0x1F4: { // GetPalmaConnectionHandle
				om.Initialize(0, 0, 8);
				var _return = GetPalmaConnectionHandle(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				om.SetData(8, _return);
				break;
			}
			case 0x1F5: { // InitializePalma
				om.Initialize(0, 0, 0);
				InitializePalma(im.GetData<ulong>(8));
				break;
			}
			case 0x1F6: { // AcquirePalmaOperationCompleteEvent
				om.Initialize(0, 1, 0);
				var _return = AcquirePalmaOperationCompleteEvent(im.GetData<ulong>(8));
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1F7: { // GetPalmaOperationInfo
				om.Initialize(0, 0, 8);
				GetPalmaOperationInfo(im.GetData<ulong>(8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x1F8: { // PlayPalmaActivity
				om.Initialize(0, 0, 0);
				PlayPalmaActivity(im.GetData<ulong>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x1F9: { // SetPalmaFrModeType
				om.Initialize(0, 0, 0);
				SetPalmaFrModeType(im.GetData<ulong>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x1FA: { // ReadPalmaStep
				om.Initialize(0, 0, 0);
				ReadPalmaStep(im.GetData<ulong>(8));
				break;
			}
			case 0x1FB: { // EnablePalmaStep
				om.Initialize(0, 0, 0);
				EnablePalmaStep(im.GetData<bool>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x1FC: { // ResetPalmaStep
				om.Initialize(0, 0, 0);
				ResetPalmaStep(im.GetData<ulong>(8));
				break;
			}
			case 0x1FD: { // ReadPalmaApplicationSection
				om.Initialize(0, 0, 0);
				ReadPalmaApplicationSection(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetData<ulong>(24));
				break;
			}
			case 0x1FE: { // WritePalmaApplicationSection
				om.Initialize(0, 0, 0);
				WritePalmaApplicationSection(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetSpan<byte>(0x19, 0), im.GetData<ulong>(24));
				break;
			}
			case 0x1FF: { // ReadPalmaUniqueCode
				om.Initialize(0, 0, 0);
				ReadPalmaUniqueCode(im.GetData<ulong>(8));
				break;
			}
			case 0x200: { // SetPalmaUniqueCodeInvalid
				om.Initialize(0, 0, 0);
				SetPalmaUniqueCodeInvalid(im.GetData<ulong>(8));
				break;
			}
			case 0x201: { // WritePalmaActivityEntry
				om.Initialize(0, 0, 0);
				WritePalmaActivityEntry(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), im.GetData<ulong>(32), im.GetData<ulong>(40));
				break;
			}
			case 0x202: { // WritePalmaRgbLedPatternEntry
				om.Initialize(0, 0, 0);
				WritePalmaRgbLedPatternEntry(im.GetData<ulong>(8), im.GetSpan<byte>(0x5, 0), im.GetData<ulong>(16));
				break;
			}
			case 0x203: { // WritePalmaWaveEntry
				om.Initialize(0, 0, 0);
				WritePalmaWaveEntry(im.GetData<ulong>(8), im.GetData<ulong>(16), Kernel.Get<KObject>(im.GetCopy(0)), im.GetData<ulong>(24), im.GetData<ulong>(32), im.GetData<ulong>(40));
				break;
			}
			case 0x204: { // SetPalmaDataBaseIdentificationVersion
				om.Initialize(0, 0, 0);
				SetPalmaDataBaseIdentificationVersion(im.GetData<uint>(8), im.GetData<ulong>(16), im.GetData<int>(24));
				break;
			}
			case 0x205: { // GetPalmaDataBaseIdentificationVersion
				om.Initialize(0, 0, 0);
				GetPalmaDataBaseIdentificationVersion(im.GetData<ulong>(8));
				break;
			}
			case 0x206: { // SuspendPalmaFeature
				om.Initialize(0, 0, 0);
				SuspendPalmaFeature(im.GetData<ulong>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x207: { // GetPalmaOperationResult
				om.Initialize(0, 0, 0);
				GetPalmaOperationResult(im.GetData<ulong>(8));
				break;
			}
			case 0x208: { // ReadPalmaPlayLog
				om.Initialize(0, 0, 0);
				ReadPalmaPlayLog(im.GetData<ulong>(8), im.GetData<ushort>(16));
				break;
			}
			case 0x209: { // ResetPalmaPlayLog
				om.Initialize(0, 0, 0);
				ResetPalmaPlayLog(im.GetData<ulong>(8), im.GetData<ushort>(16));
				break;
			}
			case 0x20A: { // SetIsPalmaAllConnectable
				om.Initialize(0, 0, 0);
				SetIsPalmaAllConnectable(im.GetData<ulong>(8), im.GetData<bool>(16), im.Pid);
				break;
			}
			case 0x20B: { // SetIsPalmaPairedConnectable
				om.Initialize(0, 0, 0);
				SetIsPalmaPairedConnectable(im.GetData<ulong>(8), im.GetData<bool>(16), im.Pid);
				break;
			}
			case 0x20C: { // PairPalma
				om.Initialize(0, 0, 0);
				PairPalma(im.GetData<ulong>(8));
				break;
			}
			case 0x20D: { // SetPalmaBoostMode
				om.Initialize(0, 0, 0);
				SetPalmaBoostMode(im.GetData<bool>(8));
				break;
			}
			case 0x3E8: { // SetNpadCommunicationMode
				om.Initialize(0, 0, 0);
				SetNpadCommunicationMode(im.GetData<ulong>(8), im.GetData<long>(16), im.Pid);
				break;
			}
			case 0x3E9: { // GetNpadCommunicationMode
				om.Initialize(0, 0, 8);
				var _return = GetNpadCommunicationMode();
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Hid.IHidServer");
		}
	}
}

public partial class IHidSystemServer : _IHidSystemServer_Base;
public abstract class _IHidSystemServer_Base : IpcInterface {
	protected virtual void SendKeyboardLockKeyEvent(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.SendKeyboardLockKeyEvent");
	protected virtual KObject AcquireHomeButtonEventHandle(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.AcquireHomeButtonEventHandle not implemented");
	protected virtual void ActivateHomeButton(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.ActivateHomeButton");
	protected virtual KObject AcquireSleepButtonEventHandle(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.AcquireSleepButtonEventHandle not implemented");
	protected virtual void ActivateSleepButton(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.ActivateSleepButton");
	protected virtual KObject AcquireCaptureButtonEventHandle(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.AcquireCaptureButtonEventHandle not implemented");
	protected virtual void ActivateCaptureButton(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.ActivateCaptureButton");
	protected virtual KObject AcquireNfcDeviceUpdateEventHandle() =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.AcquireNfcDeviceUpdateEventHandle not implemented");
	protected virtual void GetNpadsWithNfc(out ulong _0, Span<uint> _1) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetNpadsWithNfc not implemented");
	protected virtual KObject AcquireNfcActivateEventHandle(uint _0) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.AcquireNfcActivateEventHandle not implemented");
	protected virtual void ActivateNfc(byte _0, uint _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.ActivateNfc");
	protected virtual ulong GetXcdHandleForNpadWithNfc(uint _0, ulong _1) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetXcdHandleForNpadWithNfc not implemented");
	protected virtual byte IsNfcActivated(uint _0) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.IsNfcActivated not implemented");
	protected virtual KObject AcquireIrSensorEventHandle(uint _0) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.AcquireIrSensorEventHandle not implemented");
	protected virtual void ActivateIrSensor(byte _0, uint _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.ActivateIrSensor");
	protected virtual void ActivateNpadSystem(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.ActivateNpadSystem");
	protected virtual void ApplyNpadSystemCommonPolicy(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.ApplyNpadSystemCommonPolicy");
	protected virtual void EnableAssigningSingleOnSlSrPress(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.EnableAssigningSingleOnSlSrPress");
	protected virtual void DisableAssigningSingleOnSlSrPress(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.DisableAssigningSingleOnSlSrPress");
	protected virtual uint GetLastActiveNpad() =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetLastActiveNpad not implemented");
	protected virtual void GetNpadSystemExtStyle(uint _0, out ulong _1, out ulong _2) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetNpadSystemExtStyle not implemented");
	protected virtual void ApplyNpadSystemCommonPolicyFull() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.ApplyNpadSystemCommonPolicyFull");
	protected virtual void GetNpadFullKeyGripColor() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.GetNpadFullKeyGripColor");
	protected virtual void SetNpadPlayerLedBlinkingDevice(uint _0, uint _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.SetNpadPlayerLedBlinkingDevice");
	protected virtual void GetUniquePadsFromNpad(uint _0, out ulong _1, Span<ulong> _2) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetUniquePadsFromNpad not implemented");
	protected virtual ulong GetIrSensorState(uint _0, ulong _1, ulong _2) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetIrSensorState not implemented");
	protected virtual ulong GetXcdHandleForNpadWithIrSensor(uint _0, ulong _1, ulong _2) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetXcdHandleForNpadWithIrSensor not implemented");
	protected virtual void SetAppletResourceUserId(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.SetAppletResourceUserId");
	protected virtual void RegisterAppletResourceUserId(byte _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.RegisterAppletResourceUserId");
	protected virtual void UnregisterAppletResourceUserId(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.UnregisterAppletResourceUserId");
	protected virtual void EnableAppletToGetInput(byte _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.EnableAppletToGetInput");
	protected virtual void SetAruidValidForVibration(byte _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.SetAruidValidForVibration");
	protected virtual void EnableAppletToGetSixAxisSensor(byte _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.EnableAppletToGetSixAxisSensor");
	protected virtual void SetVibrationMasterVolume(float _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.SetVibrationMasterVolume");
	protected virtual float GetVibrationMasterVolume() =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetVibrationMasterVolume not implemented");
	protected virtual void BeginPermitVibrationSession(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.BeginPermitVibrationSession");
	protected virtual void EndPermitVibrationSession() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.EndPermitVibrationSession");
	protected virtual void EnableHandheldHids() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.EnableHandheldHids");
	protected virtual void DisableHandheldHids() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.DisableHandheldHids");
	protected virtual KObject AcquirePlayReportControllerUsageUpdateEvent() =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.AcquirePlayReportControllerUsageUpdateEvent not implemented");
	protected virtual void GetPlayReportControllerUsages(out ulong _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetPlayReportControllerUsages not implemented");
	protected virtual KObject AcquirePlayReportRegisteredDeviceUpdateEvent() =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.AcquirePlayReportRegisteredDeviceUpdateEvent not implemented");
	protected virtual void GetRegisteredDevicesOld(out ulong _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetRegisteredDevicesOld not implemented");
	protected virtual KObject AcquireConnectionTriggerTimeoutEvent() =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.AcquireConnectionTriggerTimeoutEvent not implemented");
	protected virtual void SendConnectionTrigger(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.SendConnectionTrigger");
	protected virtual KObject AcquireDeviceRegisteredEventForControllerSupport() =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.AcquireDeviceRegisteredEventForControllerSupport not implemented");
	protected virtual ulong GetAllowedBluetoothLinksCount() =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetAllowedBluetoothLinksCount not implemented");
	protected virtual void GetRegisteredDevices() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.GetRegisteredDevices");
	protected virtual void ActivateUniquePad(ulong _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.ActivateUniquePad");
	protected virtual KObject AcquireUniquePadConnectionEventHandle() =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.AcquireUniquePadConnectionEventHandle not implemented");
	protected virtual void GetUniquePadIds(out ulong _0, Span<ulong> _1) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetUniquePadIds not implemented");
	protected virtual KObject AcquireJoyDetachOnBluetoothOffEventHandle(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.AcquireJoyDetachOnBluetoothOffEventHandle not implemented");
	protected virtual void ListSixAxisSensorHandles(ulong _0, out ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.ListSixAxisSensorHandles not implemented");
	protected virtual byte IsSixAxisSensorUserCalibrationSupported() =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.IsSixAxisSensorUserCalibrationSupported not implemented");
	protected virtual void ResetSixAxisSensorCalibrationValues() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.ResetSixAxisSensorCalibrationValues");
	protected virtual void StartSixAxisSensorUserCalibration() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.StartSixAxisSensorUserCalibration");
	protected virtual void CancelSixAxisSensorUserCalibration() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.CancelSixAxisSensorUserCalibration");
	protected virtual void GetUniquePadBluetoothAddress(ulong _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetUniquePadBluetoothAddress not implemented");
	protected virtual void DisconnectUniquePad(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.DisconnectUniquePad");
	protected virtual void GetUniquePadType() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.GetUniquePadType");
	protected virtual void GetUniquePadInterface() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.GetUniquePadInterface");
	protected virtual void GetUniquePadSerialNumber() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.GetUniquePadSerialNumber");
	protected virtual void GetUniquePadControllerNumber() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.GetUniquePadControllerNumber");
	protected virtual void GetSixAxisSensorUserCalibrationStage() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.GetSixAxisSensorUserCalibrationStage");
	protected virtual void StartAnalogStickManualCalibration(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.StartAnalogStickManualCalibration");
	protected virtual void RetryCurrentAnalogStickManualCalibrationStage(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.RetryCurrentAnalogStickManualCalibrationStage");
	protected virtual void CancelAnalogStickManualCalibration(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.CancelAnalogStickManualCalibration");
	protected virtual void ResetAnalogStickManualCalibration(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.ResetAnalogStickManualCalibration");
	protected virtual void GetAnalogStickState() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.GetAnalogStickState");
	protected virtual void GetAnalogStickManualCalibrationStage() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.GetAnalogStickManualCalibrationStage");
	protected virtual void IsAnalogStickButtonPressed() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.IsAnalogStickButtonPressed");
	protected virtual void IsAnalogStickInReleasePosition() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.IsAnalogStickInReleasePosition");
	protected virtual void IsAnalogStickInCircumference() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.IsAnalogStickInCircumference");
	protected virtual byte IsUsbFullKeyControllerEnabled() =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.IsUsbFullKeyControllerEnabled not implemented");
	protected virtual void EnableUsbFullKeyController(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.EnableUsbFullKeyController");
	protected virtual byte IsUsbConnected(ulong _0) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.IsUsbConnected not implemented");
	protected virtual void ActivateInputDetector(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.ActivateInputDetector");
	protected virtual void NotifyInputDetector(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.NotifyInputDetector");
	protected virtual void InitializeFirmwareUpdate() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.InitializeFirmwareUpdate");
	protected virtual void GetFirmwareVersion(ulong _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetFirmwareVersion not implemented");
	protected virtual void GetAvailableFirmwareVersion(ulong _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetAvailableFirmwareVersion not implemented");
	protected virtual byte IsFirmwareUpdateAvailable(ulong _0) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.IsFirmwareUpdateAvailable not implemented");
	protected virtual ulong CheckFirmwareUpdateRequired(ulong _0) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.CheckFirmwareUpdateRequired not implemented");
	protected virtual ulong StartFirmwareUpdate(ulong _0) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.StartFirmwareUpdate not implemented");
	protected virtual void AbortFirmwareUpdate() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.AbortFirmwareUpdate");
	protected virtual void GetFirmwareUpdateState(ulong _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetFirmwareUpdateState not implemented");
	protected virtual void ActivateAudioControl() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.ActivateAudioControl");
	protected virtual KObject AcquireAudioControlEventHandle() =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.AcquireAudioControlEventHandle not implemented");
	protected virtual void GetAudioControlStates(out ulong _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetAudioControlStates not implemented");
	protected virtual void DeactivateAudioControl() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.DeactivateAudioControl");
	protected virtual void IsSixAxisSensorAccurateUserCalibrationSupported() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.IsSixAxisSensorAccurateUserCalibrationSupported");
	protected virtual void StartSixAxisSensorAccurateUserCalibration() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.StartSixAxisSensorAccurateUserCalibration");
	protected virtual void CancelSixAxisSensorAccurateUserCalibration() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.CancelSixAxisSensorAccurateUserCalibration");
	protected virtual void GetSixAxisSensorAccurateUserCalibrationState() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.GetSixAxisSensorAccurateUserCalibrationState");
	protected virtual void GetHidbusSystemServiceObject() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.GetHidbusSystemServiceObject");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1F: { // SendKeyboardLockKeyEvent
				om.Initialize(0, 0, 0);
				SendKeyboardLockKeyEvent(im.GetData<uint>(8));
				break;
			}
			case 0x65: { // AcquireHomeButtonEventHandle
				om.Initialize(0, 1, 0);
				var _return = AcquireHomeButtonEventHandle(im.GetData<ulong>(8), im.Pid);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x6F: { // ActivateHomeButton
				om.Initialize(0, 0, 0);
				ActivateHomeButton(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x79: { // AcquireSleepButtonEventHandle
				om.Initialize(0, 1, 0);
				var _return = AcquireSleepButtonEventHandle(im.GetData<ulong>(8), im.Pid);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x83: { // ActivateSleepButton
				om.Initialize(0, 0, 0);
				ActivateSleepButton(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x8D: { // AcquireCaptureButtonEventHandle
				om.Initialize(0, 1, 0);
				var _return = AcquireCaptureButtonEventHandle(im.GetData<ulong>(8), im.Pid);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x97: { // ActivateCaptureButton
				om.Initialize(0, 0, 0);
				ActivateCaptureButton(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0xD2: { // AcquireNfcDeviceUpdateEventHandle
				om.Initialize(0, 1, 0);
				var _return = AcquireNfcDeviceUpdateEventHandle();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xD3: { // GetNpadsWithNfc
				om.Initialize(0, 0, 8);
				GetNpadsWithNfc(out var _0, im.GetSpan<uint>(0xA, 0));
				om.SetData(8, _0);
				break;
			}
			case 0xD4: { // AcquireNfcActivateEventHandle
				om.Initialize(0, 1, 0);
				var _return = AcquireNfcActivateEventHandle(im.GetData<uint>(8));
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xD5: { // ActivateNfc
				om.Initialize(0, 0, 0);
				ActivateNfc(im.GetData<byte>(8), im.GetData<uint>(12), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0xD6: { // GetXcdHandleForNpadWithNfc
				om.Initialize(0, 0, 8);
				var _return = GetXcdHandleForNpadWithNfc(im.GetData<uint>(8), im.GetData<ulong>(16));
				om.SetData(8, _return);
				break;
			}
			case 0xD7: { // IsNfcActivated
				om.Initialize(0, 0, 1);
				var _return = IsNfcActivated(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			case 0xE6: { // AcquireIrSensorEventHandle
				om.Initialize(0, 1, 0);
				var _return = AcquireIrSensorEventHandle(im.GetData<uint>(8));
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xE7: { // ActivateIrSensor
				om.Initialize(0, 0, 0);
				ActivateIrSensor(im.GetData<byte>(8), im.GetData<uint>(12), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x12D: { // ActivateNpadSystem
				om.Initialize(0, 0, 0);
				ActivateNpadSystem(im.GetData<uint>(8));
				break;
			}
			case 0x12F: { // ApplyNpadSystemCommonPolicy
				om.Initialize(0, 0, 0);
				ApplyNpadSystemCommonPolicy(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x130: { // EnableAssigningSingleOnSlSrPress
				om.Initialize(0, 0, 0);
				EnableAssigningSingleOnSlSrPress(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x131: { // DisableAssigningSingleOnSlSrPress
				om.Initialize(0, 0, 0);
				DisableAssigningSingleOnSlSrPress(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x132: { // GetLastActiveNpad
				om.Initialize(0, 0, 4);
				var _return = GetLastActiveNpad();
				om.SetData(8, _return);
				break;
			}
			case 0x133: { // GetNpadSystemExtStyle
				om.Initialize(0, 0, 16);
				GetNpadSystemExtStyle(im.GetData<uint>(8), out var _0, out var _1);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x134: { // ApplyNpadSystemCommonPolicyFull
				om.Initialize(0, 0, 0);
				ApplyNpadSystemCommonPolicyFull();
				break;
			}
			case 0x135: { // GetNpadFullKeyGripColor
				om.Initialize(0, 0, 0);
				GetNpadFullKeyGripColor();
				break;
			}
			case 0x137: { // SetNpadPlayerLedBlinkingDevice
				om.Initialize(0, 0, 0);
				SetNpadPlayerLedBlinkingDevice(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x141: { // GetUniquePadsFromNpad
				om.Initialize(0, 0, 8);
				GetUniquePadsFromNpad(im.GetData<uint>(8), out var _0, im.GetSpan<ulong>(0xA, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x142: { // GetIrSensorState
				om.Initialize(0, 0, 8);
				var _return = GetIrSensorState(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				om.SetData(8, _return);
				break;
			}
			case 0x143: { // GetXcdHandleForNpadWithIrSensor
				om.Initialize(0, 0, 8);
				var _return = GetXcdHandleForNpadWithIrSensor(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				om.SetData(8, _return);
				break;
			}
			case 0x1F4: { // SetAppletResourceUserId
				om.Initialize(0, 0, 0);
				SetAppletResourceUserId(im.GetData<ulong>(8));
				break;
			}
			case 0x1F5: { // RegisterAppletResourceUserId
				om.Initialize(0, 0, 0);
				RegisterAppletResourceUserId(im.GetData<byte>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x1F6: { // UnregisterAppletResourceUserId
				om.Initialize(0, 0, 0);
				UnregisterAppletResourceUserId(im.GetData<ulong>(8));
				break;
			}
			case 0x1F7: { // EnableAppletToGetInput
				om.Initialize(0, 0, 0);
				EnableAppletToGetInput(im.GetData<byte>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x1F8: { // SetAruidValidForVibration
				om.Initialize(0, 0, 0);
				SetAruidValidForVibration(im.GetData<byte>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x1F9: { // EnableAppletToGetSixAxisSensor
				om.Initialize(0, 0, 0);
				EnableAppletToGetSixAxisSensor(im.GetData<byte>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x1FE: { // SetVibrationMasterVolume
				om.Initialize(0, 0, 0);
				SetVibrationMasterVolume(im.GetData<float>(8));
				break;
			}
			case 0x1FF: { // GetVibrationMasterVolume
				om.Initialize(0, 0, 4);
				var _return = GetVibrationMasterVolume();
				om.SetData(8, _return);
				break;
			}
			case 0x200: { // BeginPermitVibrationSession
				om.Initialize(0, 0, 0);
				BeginPermitVibrationSession(im.GetData<ulong>(8));
				break;
			}
			case 0x201: { // EndPermitVibrationSession
				om.Initialize(0, 0, 0);
				EndPermitVibrationSession();
				break;
			}
			case 0x208: { // EnableHandheldHids
				om.Initialize(0, 0, 0);
				EnableHandheldHids();
				break;
			}
			case 0x209: { // DisableHandheldHids
				om.Initialize(0, 0, 0);
				DisableHandheldHids();
				break;
			}
			case 0x21C: { // AcquirePlayReportControllerUsageUpdateEvent
				om.Initialize(0, 1, 0);
				var _return = AcquirePlayReportControllerUsageUpdateEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x21D: { // GetPlayReportControllerUsages
				om.Initialize(0, 0, 8);
				GetPlayReportControllerUsages(out var _0, im.GetSpan<byte>(0xA, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x21E: { // AcquirePlayReportRegisteredDeviceUpdateEvent
				om.Initialize(0, 1, 0);
				var _return = AcquirePlayReportRegisteredDeviceUpdateEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x21F: { // GetRegisteredDevicesOld
				om.Initialize(0, 0, 8);
				GetRegisteredDevicesOld(out var _0, im.GetSpan<byte>(0xA, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x220: { // AcquireConnectionTriggerTimeoutEvent
				om.Initialize(0, 1, 0);
				var _return = AcquireConnectionTriggerTimeoutEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x221: { // SendConnectionTrigger
				om.Initialize(0, 0, 0);
				SendConnectionTrigger(im.GetBytes(8, 0x6));
				break;
			}
			case 0x222: { // AcquireDeviceRegisteredEventForControllerSupport
				om.Initialize(0, 1, 0);
				var _return = AcquireDeviceRegisteredEventForControllerSupport();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x223: { // GetAllowedBluetoothLinksCount
				om.Initialize(0, 0, 8);
				var _return = GetAllowedBluetoothLinksCount();
				om.SetData(8, _return);
				break;
			}
			case 0x224: { // GetRegisteredDevices
				om.Initialize(0, 0, 0);
				GetRegisteredDevices();
				break;
			}
			case 0x2BC: { // ActivateUniquePad
				om.Initialize(0, 0, 0);
				ActivateUniquePad(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid);
				break;
			}
			case 0x2BE: { // AcquireUniquePadConnectionEventHandle
				om.Initialize(0, 1, 0);
				var _return = AcquireUniquePadConnectionEventHandle();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x2BF: { // GetUniquePadIds
				om.Initialize(0, 0, 8);
				GetUniquePadIds(out var _0, im.GetSpan<ulong>(0xA, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x2EF: { // AcquireJoyDetachOnBluetoothOffEventHandle
				om.Initialize(0, 1, 0);
				var _return = AcquireJoyDetachOnBluetoothOffEventHandle(im.GetData<ulong>(8), im.Pid);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x320: { // ListSixAxisSensorHandles
				om.Initialize(0, 0, 8);
				ListSixAxisSensorHandles(im.GetData<ulong>(8), out var _0, im.GetSpan<byte>(0xA, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x321: { // IsSixAxisSensorUserCalibrationSupported
				om.Initialize(0, 0, 1);
				var _return = IsSixAxisSensorUserCalibrationSupported();
				om.SetData(8, _return);
				break;
			}
			case 0x322: { // ResetSixAxisSensorCalibrationValues
				om.Initialize(0, 0, 0);
				ResetSixAxisSensorCalibrationValues();
				break;
			}
			case 0x323: { // StartSixAxisSensorUserCalibration
				om.Initialize(0, 0, 0);
				StartSixAxisSensorUserCalibration();
				break;
			}
			case 0x324: { // CancelSixAxisSensorUserCalibration
				om.Initialize(0, 0, 0);
				CancelSixAxisSensorUserCalibration();
				break;
			}
			case 0x325: { // GetUniquePadBluetoothAddress
				om.Initialize(0, 0, 6);
				GetUniquePadBluetoothAddress(im.GetData<ulong>(8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x326: { // DisconnectUniquePad
				om.Initialize(0, 0, 0);
				DisconnectUniquePad(im.GetData<ulong>(8));
				break;
			}
			case 0x327: { // GetUniquePadType
				om.Initialize(0, 0, 0);
				GetUniquePadType();
				break;
			}
			case 0x328: { // GetUniquePadInterface
				om.Initialize(0, 0, 0);
				GetUniquePadInterface();
				break;
			}
			case 0x329: { // GetUniquePadSerialNumber
				om.Initialize(0, 0, 0);
				GetUniquePadSerialNumber();
				break;
			}
			case 0x32A: { // GetUniquePadControllerNumber
				om.Initialize(0, 0, 0);
				GetUniquePadControllerNumber();
				break;
			}
			case 0x32B: { // GetSixAxisSensorUserCalibrationStage
				om.Initialize(0, 0, 0);
				GetSixAxisSensorUserCalibrationStage();
				break;
			}
			case 0x335: { // StartAnalogStickManualCalibration
				om.Initialize(0, 0, 0);
				StartAnalogStickManualCalibration(im.GetData<ulong>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x336: { // RetryCurrentAnalogStickManualCalibrationStage
				om.Initialize(0, 0, 0);
				RetryCurrentAnalogStickManualCalibrationStage(im.GetData<ulong>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x337: { // CancelAnalogStickManualCalibration
				om.Initialize(0, 0, 0);
				CancelAnalogStickManualCalibration(im.GetData<ulong>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x338: { // ResetAnalogStickManualCalibration
				om.Initialize(0, 0, 0);
				ResetAnalogStickManualCalibration(im.GetData<ulong>(8), im.GetData<ulong>(16));
				break;
			}
			case 0x339: { // GetAnalogStickState
				om.Initialize(0, 0, 0);
				GetAnalogStickState();
				break;
			}
			case 0x33A: { // GetAnalogStickManualCalibrationStage
				om.Initialize(0, 0, 0);
				GetAnalogStickManualCalibrationStage();
				break;
			}
			case 0x33B: { // IsAnalogStickButtonPressed
				om.Initialize(0, 0, 0);
				IsAnalogStickButtonPressed();
				break;
			}
			case 0x33C: { // IsAnalogStickInReleasePosition
				om.Initialize(0, 0, 0);
				IsAnalogStickInReleasePosition();
				break;
			}
			case 0x33D: { // IsAnalogStickInCircumference
				om.Initialize(0, 0, 0);
				IsAnalogStickInCircumference();
				break;
			}
			case 0x352: { // IsUsbFullKeyControllerEnabled
				om.Initialize(0, 0, 1);
				var _return = IsUsbFullKeyControllerEnabled();
				om.SetData(8, _return);
				break;
			}
			case 0x353: { // EnableUsbFullKeyController
				om.Initialize(0, 0, 0);
				EnableUsbFullKeyController(im.GetData<byte>(8));
				break;
			}
			case 0x354: { // IsUsbConnected
				om.Initialize(0, 0, 1);
				var _return = IsUsbConnected(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x384: { // ActivateInputDetector
				om.Initialize(0, 0, 0);
				ActivateInputDetector(im.GetData<ulong>(8), im.Pid);
				break;
			}
			case 0x385: { // NotifyInputDetector
				om.Initialize(0, 0, 0);
				NotifyInputDetector(im.GetData<uint>(8));
				break;
			}
			case 0x3E8: { // InitializeFirmwareUpdate
				om.Initialize(0, 0, 0);
				InitializeFirmwareUpdate();
				break;
			}
			case 0x3E9: { // GetFirmwareVersion
				om.Initialize(0, 0, 16);
				GetFirmwareVersion(im.GetData<ulong>(8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3EA: { // GetAvailableFirmwareVersion
				om.Initialize(0, 0, 16);
				GetAvailableFirmwareVersion(im.GetData<ulong>(8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3EB: { // IsFirmwareUpdateAvailable
				om.Initialize(0, 0, 1);
				var _return = IsFirmwareUpdateAvailable(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x3EC: { // CheckFirmwareUpdateRequired
				om.Initialize(0, 0, 8);
				var _return = CheckFirmwareUpdateRequired(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x3ED: { // StartFirmwareUpdate
				om.Initialize(0, 0, 8);
				var _return = StartFirmwareUpdate(im.GetData<ulong>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x3EE: { // AbortFirmwareUpdate
				om.Initialize(0, 0, 0);
				AbortFirmwareUpdate();
				break;
			}
			case 0x3EF: { // GetFirmwareUpdateState
				om.Initialize(0, 0, 4);
				GetFirmwareUpdateState(im.GetData<ulong>(8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3F0: { // ActivateAudioControl
				om.Initialize(0, 0, 0);
				ActivateAudioControl();
				break;
			}
			case 0x3F1: { // AcquireAudioControlEventHandle
				om.Initialize(0, 1, 0);
				var _return = AcquireAudioControlEventHandle();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x3F2: { // GetAudioControlStates
				om.Initialize(0, 0, 8);
				GetAudioControlStates(out var _0, im.GetSpan<byte>(0xA, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x3F3: { // DeactivateAudioControl
				om.Initialize(0, 0, 0);
				DeactivateAudioControl();
				break;
			}
			case 0x41A: { // IsSixAxisSensorAccurateUserCalibrationSupported
				om.Initialize(0, 0, 0);
				IsSixAxisSensorAccurateUserCalibrationSupported();
				break;
			}
			case 0x41B: { // StartSixAxisSensorAccurateUserCalibration
				om.Initialize(0, 0, 0);
				StartSixAxisSensorAccurateUserCalibration();
				break;
			}
			case 0x41C: { // CancelSixAxisSensorAccurateUserCalibration
				om.Initialize(0, 0, 0);
				CancelSixAxisSensorAccurateUserCalibration();
				break;
			}
			case 0x41D: { // GetSixAxisSensorAccurateUserCalibrationState
				om.Initialize(0, 0, 0);
				GetSixAxisSensorAccurateUserCalibrationState();
				break;
			}
			case 0x44C: { // GetHidbusSystemServiceObject
				om.Initialize(0, 0, 0);
				GetHidbusSystemServiceObject();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Hid.IHidSystemServer");
		}
	}
}

public partial class IHidTemporaryServer : _IHidTemporaryServer_Base;
public abstract class _IHidTemporaryServer_Base : IpcInterface {
	protected virtual void GetConsoleSixAxisSensorCalibrationValues(uint _0, ulong _1, ulong _2, out byte[] _3) =>
		throw new NotImplementedException("Nn.Hid.IHidTemporaryServer.GetConsoleSixAxisSensorCalibrationValues not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetConsoleSixAxisSensorCalibrationValues
				om.Initialize(0, 0, 24);
				GetConsoleSixAxisSensorCalibrationValues(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid, out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Hid.IHidTemporaryServer");
		}
	}
}

