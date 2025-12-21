using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Hid;
public partial class IActiveVibrationDeviceList : _IActiveVibrationDeviceList_Base;
public abstract class _IActiveVibrationDeviceList_Base : IpcInterface {
	protected virtual void ActivateVibrationDevice(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IActiveVibrationDeviceList.ActivateVibrationDevice");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // ActivateVibrationDevice
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Hid.IActiveVibrationDeviceList");
		}
	}
}

public partial class IAppletResource : _IAppletResource_Base;
public abstract class _IAppletResource_Base : IpcInterface {
	protected virtual KObject GetSharedMemoryHandle() =>
		throw new NotImplementedException("Nn.Hid.IAppletResource.GetSharedMemoryHandle not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetSharedMemoryHandle
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Hid.IAppletResource");
		}
	}
}

public partial class IHidDebugServer : _IHidDebugServer_Base;
public abstract class _IHidDebugServer_Base : IpcInterface {
	protected virtual void DeactivateDebugPad() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.DeactivateDebugPad");
	protected virtual void SetDebugPadAutoPilotState(Span<byte> _0) =>
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
	protected virtual void SetMouseAutoPilotState(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.SetMouseAutoPilotState");
	protected virtual void UnsetMouseAutoPilotState() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.UnsetMouseAutoPilotState");
	protected virtual void DeactivateKeyboard() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.DeactivateKeyboard");
	protected virtual void SetKeyboardAutoPilotState(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.SetKeyboardAutoPilotState");
	protected virtual void UnsetKeyboardAutoPilotState() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.UnsetKeyboardAutoPilotState");
	protected virtual void DeactivateXpad(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.DeactivateXpad");
	protected virtual void SetXpadAutoPilotState(uint _0, Span<byte> _1) =>
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
	protected virtual void GetShiftAccelerometerCalibrationValue(uint _0, ulong _1, ulong _2) =>
		throw new NotImplementedException("Nn.Hid.IHidDebugServer.GetShiftAccelerometerCalibrationValue not implemented");
	protected virtual void SetShiftGyroscopeCalibrationValue(uint _0, float _1, float _2, ulong _3, ulong _4) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.SetShiftGyroscopeCalibrationValue");
	protected virtual void GetShiftGyroscopeCalibrationValue(uint _0, ulong _1, ulong _2) =>
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
	protected virtual void GetFirmwareUpdateStage() =>
		throw new NotImplementedException("Nn.Hid.IHidDebugServer.GetFirmwareUpdateStage not implemented");
	protected virtual void GetFirmwareVersion(uint _0, uint _1) =>
		throw new NotImplementedException("Nn.Hid.IHidDebugServer.GetFirmwareVersion not implemented");
	protected virtual void GetDestinationFirmwareVersion(uint _0, uint _1) =>
		throw new NotImplementedException("Nn.Hid.IHidDebugServer.GetDestinationFirmwareVersion not implemented");
	protected virtual void DiscardFirmwareInfoCacheForRevert() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.DiscardFirmwareInfoCacheForRevert");
	protected virtual void StartFirmwareUpdateForRevert(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidDebugServer.StartFirmwareUpdateForRevert");
	protected virtual void GetAvailableFirmwareVersionForRevert(ulong _0) =>
		throw new NotImplementedException("Nn.Hid.IHidDebugServer.GetAvailableFirmwareVersionForRevert not implemented");
	protected virtual byte IsFirmwareUpdatingDevice(ulong _0) =>
		throw new NotImplementedException("Nn.Hid.IHidDebugServer.IsFirmwareUpdatingDevice not implemented");
	protected virtual void UpdateControllerColor(Span<byte> _0, Span<byte> _1, ulong _2) =>
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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // DeactivateDebugPad
				break;
			case 0x1: // SetDebugPadAutoPilotState
				break;
			case 0x2: // UnsetDebugPadAutoPilotState
				break;
			case 0xA: // DeactivateTouchScreen
				break;
			case 0xB: // SetTouchScreenAutoPilotState
				break;
			case 0xC: // UnsetTouchScreenAutoPilotState
				break;
			case 0x14: // DeactivateMouse
				break;
			case 0x15: // SetMouseAutoPilotState
				break;
			case 0x16: // UnsetMouseAutoPilotState
				break;
			case 0x1E: // DeactivateKeyboard
				break;
			case 0x1F: // SetKeyboardAutoPilotState
				break;
			case 0x20: // UnsetKeyboardAutoPilotState
				break;
			case 0x32: // DeactivateXpad
				break;
			case 0x33: // SetXpadAutoPilotState
				break;
			case 0x34: // UnsetXpadAutoPilotState
				break;
			case 0x3C: // DeactivateJoyXpad
				break;
			case 0x5B: // DeactivateGesture
				break;
			case 0x6E: // DeactivateHomeButton
				break;
			case 0x6F: // SetHomeButtonAutoPilotState
				break;
			case 0x70: // UnsetHomeButtonAutoPilotState
				break;
			case 0x78: // DeactivateSleepButton
				break;
			case 0x79: // SetSleepButtonAutoPilotState
				break;
			case 0x7A: // UnsetSleepButtonAutoPilotState
				break;
			case 0x7B: // DeactivateInputDetector
				break;
			case 0x82: // DeactivateCaptureButton
				break;
			case 0x83: // SetCaptureButtonAutoPilotState
				break;
			case 0x84: // UnsetCaptureButtonAutoPilotState
				break;
			case 0x85: // SetShiftAccelerometerCalibrationValue
				break;
			case 0x86: // GetShiftAccelerometerCalibrationValue
				break;
			case 0x87: // SetShiftGyroscopeCalibrationValue
				break;
			case 0x88: // GetShiftGyroscopeCalibrationValue
				break;
			case 0x8C: // DeactivateConsoleSixAxisSensor
				break;
			case 0x8D: // GetConsoleSixAxisSensorSamplingFrequency
				break;
			case 0x8E: // DeactivateSevenSixAxisSensor
				break;
			case 0xC9: // ActivateFirmwareUpdate
				break;
			case 0xCA: // DeactivateFirmwareUpdate
				break;
			case 0xCB: // StartFirmwareUpdate
				break;
			case 0xCC: // GetFirmwareUpdateStage
				break;
			case 0xCD: // GetFirmwareVersion
				break;
			case 0xCE: // GetDestinationFirmwareVersion
				break;
			case 0xCF: // DiscardFirmwareInfoCacheForRevert
				break;
			case 0xD0: // StartFirmwareUpdateForRevert
				break;
			case 0xD1: // GetAvailableFirmwareVersionForRevert
				break;
			case 0xD2: // IsFirmwareUpdatingDevice
				break;
			case 0xDD: // UpdateControllerColor
				break;
			case 0xDE: // ConnectUsbPadsAsync
				break;
			case 0xDF: // DisconnectUsbPadsAsync
				break;
			case 0xE0: // UpdateDesignInfo
				break;
			case 0xE1: // GetUniquePadDriverState
				break;
			case 0xE2: // GetSixAxisSensorDriverStates
				break;
			case 0x12D: // GetAbstractedPadHandles
				break;
			case 0x12E: // GetAbstractedPadState
				break;
			case 0x12F: // GetAbstractedPadsState
				break;
			case 0x141: // SetAutoPilotVirtualPadState
				break;
			case 0x142: // UnsetAutoPilotVirtualPadState
				break;
			case 0x143: // UnsetAllAutoPilotVirtualPadState
				break;
			case 0x15E: // AddRegisteredDevice
				break;
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
	protected virtual void GetXpadIds() =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetXpadIds not implemented");
	protected virtual void ActivateJoyXpad(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ActivateJoyXpad");
	protected virtual KObject GetJoyXpadLifoHandle(uint _0) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetJoyXpadLifoHandle not implemented");
	protected virtual void GetJoyXpadIds() =>
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
	protected virtual void GetSixAxisSensorFusionParameters(uint _0, ulong _1, ulong _2) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetSixAxisSensorFusionParameters not implemented");
	protected virtual void ResetSixAxisSensorFusionParameters(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.ResetSixAxisSensorFusionParameters");
	protected virtual void SetAccelerometerParameters(uint _0, float _1, float _2, ulong _3, ulong _4) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.SetAccelerometerParameters");
	protected virtual void GetAccelerometerParameters(uint _0, ulong _1, ulong _2) =>
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
	protected virtual void SetNpadJoyAssignmentModeSingleWithDestination(uint _0, ulong _1, ulong _2, ulong _3) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.SetNpadJoyAssignmentModeSingleWithDestination not implemented");
	protected virtual void GetVibrationDeviceInfo(uint _0) =>
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
	protected virtual void HasLeftRightBattery(uint _0) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.HasLeftRightBattery not implemented");
	protected virtual byte GetNpadInterfaceType(uint _0) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetNpadInterfaceType not implemented");
	protected virtual void GetNpadLeftRightInterfaceType(uint _0) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetNpadLeftRightInterfaceType not implemented");
	protected virtual ulong GetPalmaConnectionHandle(uint _0, ulong _1, ulong _2) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.GetPalmaConnectionHandle not implemented");
	protected virtual void InitializePalma(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidServer.InitializePalma");
	protected virtual KObject AcquirePalmaOperationCompleteEvent(ulong _0) =>
		throw new NotImplementedException("Nn.Hid.IHidServer.AcquirePalmaOperationCompleteEvent not implemented");
	protected virtual void GetPalmaOperationInfo(ulong _0) =>
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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CreateAppletResource
				break;
			case 0x1: // ActivateDebugPad
				break;
			case 0xB: // ActivateTouchScreen
				break;
			case 0x15: // ActivateMouse
				break;
			case 0x1F: // ActivateKeyboard
				break;
			case 0x20: // Unknown32
				break;
			case 0x28: // AcquireXpadIdEventHandle
				break;
			case 0x29: // ReleaseXpadIdEventHandle
				break;
			case 0x33: // ActivateXpad
				break;
			case 0x37: // GetXpadIds
				break;
			case 0x38: // ActivateJoyXpad
				break;
			case 0x3A: // GetJoyXpadLifoHandle
				break;
			case 0x3B: // GetJoyXpadIds
				break;
			case 0x3C: // ActivateSixAxisSensor
				break;
			case 0x3D: // DeactivateSixAxisSensor
				break;
			case 0x3E: // GetSixAxisSensorLifoHandle
				break;
			case 0x3F: // ActivateJoySixAxisSensor
				break;
			case 0x40: // DeactivateJoySixAxisSensor
				break;
			case 0x41: // GetJoySixAxisSensorLifoHandle
				break;
			case 0x42: // StartSixAxisSensor
				break;
			case 0x43: // StopSixAxisSensor
				break;
			case 0x44: // IsSixAxisSensorFusionEnabled
				break;
			case 0x45: // EnableSixAxisSensorFusion
				break;
			case 0x46: // SetSixAxisSensorFusionParameters
				break;
			case 0x47: // GetSixAxisSensorFusionParameters
				break;
			case 0x48: // ResetSixAxisSensorFusionParameters
				break;
			case 0x49: // SetAccelerometerParameters
				break;
			case 0x4A: // GetAccelerometerParameters
				break;
			case 0x4B: // ResetAccelerometerParameters
				break;
			case 0x4C: // SetAccelerometerPlayMode
				break;
			case 0x4D: // GetAccelerometerPlayMode
				break;
			case 0x4E: // ResetAccelerometerPlayMode
				break;
			case 0x4F: // SetGyroscopeZeroDriftMode
				break;
			case 0x50: // GetGyroscopeZeroDriftMode
				break;
			case 0x51: // ResetGyroscopeZeroDriftMode
				break;
			case 0x52: // IsSixAxisSensorAtRest
				break;
			case 0x53: // Unknown83
				break;
			case 0x5B: // ActivateGesture
				break;
			case 0x64: // SetSupportedNpadStyleSet
				break;
			case 0x65: // GetSupportedNpadStyleSet
				break;
			case 0x66: // SetSupportedNpadIdType
				break;
			case 0x67: // ActivateNpad
				break;
			case 0x68: // DeactivateNpad
				break;
			case 0x6A: // AcquireNpadStyleSetUpdateEventHandle
				break;
			case 0x6B: // DisconnectNpad
				break;
			case 0x6C: // GetPlayerLedPattern
				break;
			case 0x78: // SetNpadJoyHoldType
				break;
			case 0x79: // GetNpadJoyHoldType
				break;
			case 0x7A: // SetNpadJoyAssignmentModeSingleByDefault
				break;
			case 0x7B: // SetNpadJoyAssignmentModeSingle
				break;
			case 0x7C: // SetNpadJoyAssignmentModeDual
				break;
			case 0x7D: // MergeSingleJoyAsDualJoy
				break;
			case 0x7E: // StartLrAssignmentMode
				break;
			case 0x7F: // StopLrAssignmentMode
				break;
			case 0x80: // SetNpadHandheldActivationMode
				break;
			case 0x81: // GetNpadHandheldActivationMode
				break;
			case 0x82: // SwapNpadAssignment
				break;
			case 0x83: // IsUnintendedHomeButtonInputProtectionEnabled
				break;
			case 0x84: // EnableUnintendedHomeButtonInputProtection
				break;
			case 0x85: // SetNpadJoyAssignmentModeSingleWithDestination
				break;
			case 0xC8: // GetVibrationDeviceInfo
				break;
			case 0xC9: // SendVibrationValue
				break;
			case 0xCA: // GetActualVibrationValue
				break;
			case 0xCB: // CreateActiveVibrationDeviceList
				break;
			case 0xCC: // PermitVibration
				break;
			case 0xCD: // IsVibrationPermitted
				break;
			case 0xCE: // SendVibrationValues
				break;
			case 0xCF: // SendVibrationGcErmCommand
				break;
			case 0xD0: // GetActualVibrationGcErmCommand
				break;
			case 0xD1: // BeginPermitVibrationSession
				break;
			case 0xD2: // EndPermitVibrationSession
				break;
			case 0x12C: // ActivateConsoleSixAxisSensor
				break;
			case 0x12D: // StartConsoleSixAxisSensor
				break;
			case 0x12E: // StopConsoleSixAxisSensor
				break;
			case 0x12F: // ActivateSevenSixAxisSensor
				break;
			case 0x130: // StartSevenSixAxisSensor
				break;
			case 0x131: // StopSevenSixAxisSensor
				break;
			case 0x132: // InitializeSevenSixAxisSensor
				break;
			case 0x133: // FinalizeSevenSixAxisSensor
				break;
			case 0x134: // SetSevenSixAxisSensorFusionStrength
				break;
			case 0x135: // GetSevenSixAxisSensorFusionStrength
				break;
			case 0x136: // Unknown310
				break;
			case 0x190: // IsUsbFullKeyControllerEnabled
				break;
			case 0x191: // EnableUsbFullKeyController
				break;
			case 0x192: // IsUsbFullKeyControllerConnected
				break;
			case 0x193: // HasBattery
				break;
			case 0x194: // HasLeftRightBattery
				break;
			case 0x195: // GetNpadInterfaceType
				break;
			case 0x196: // GetNpadLeftRightInterfaceType
				break;
			case 0x1F4: // GetPalmaConnectionHandle
				break;
			case 0x1F5: // InitializePalma
				break;
			case 0x1F6: // AcquirePalmaOperationCompleteEvent
				break;
			case 0x1F7: // GetPalmaOperationInfo
				break;
			case 0x1F8: // PlayPalmaActivity
				break;
			case 0x1F9: // SetPalmaFrModeType
				break;
			case 0x1FA: // ReadPalmaStep
				break;
			case 0x1FB: // EnablePalmaStep
				break;
			case 0x1FC: // ResetPalmaStep
				break;
			case 0x1FD: // ReadPalmaApplicationSection
				break;
			case 0x1FE: // WritePalmaApplicationSection
				break;
			case 0x1FF: // ReadPalmaUniqueCode
				break;
			case 0x200: // SetPalmaUniqueCodeInvalid
				break;
			case 0x201: // WritePalmaActivityEntry
				break;
			case 0x202: // WritePalmaRgbLedPatternEntry
				break;
			case 0x203: // WritePalmaWaveEntry
				break;
			case 0x204: // SetPalmaDataBaseIdentificationVersion
				break;
			case 0x205: // GetPalmaDataBaseIdentificationVersion
				break;
			case 0x206: // SuspendPalmaFeature
				break;
			case 0x207: // GetPalmaOperationResult
				break;
			case 0x208: // ReadPalmaPlayLog
				break;
			case 0x209: // ResetPalmaPlayLog
				break;
			case 0x20A: // SetIsPalmaAllConnectable
				break;
			case 0x20B: // SetIsPalmaPairedConnectable
				break;
			case 0x20C: // PairPalma
				break;
			case 0x20D: // SetPalmaBoostMode
				break;
			case 0x3E8: // SetNpadCommunicationMode
				break;
			case 0x3E9: // GetNpadCommunicationMode
				break;
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
	protected virtual void GetNpadsWithNfc() =>
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
	protected virtual void GetNpadSystemExtStyle(uint _0) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetNpadSystemExtStyle not implemented");
	protected virtual void ApplyNpadSystemCommonPolicyFull() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.ApplyNpadSystemCommonPolicyFull");
	protected virtual void GetNpadFullKeyGripColor() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.GetNpadFullKeyGripColor");
	protected virtual void SetNpadPlayerLedBlinkingDevice(uint _0, uint _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.SetNpadPlayerLedBlinkingDevice");
	protected virtual void GetUniquePadsFromNpad(uint _0) =>
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
	protected virtual void GetPlayReportControllerUsages() =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetPlayReportControllerUsages not implemented");
	protected virtual KObject AcquirePlayReportRegisteredDeviceUpdateEvent() =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.AcquirePlayReportRegisteredDeviceUpdateEvent not implemented");
	protected virtual void GetRegisteredDevicesOld() =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetRegisteredDevicesOld not implemented");
	protected virtual KObject AcquireConnectionTriggerTimeoutEvent() =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.AcquireConnectionTriggerTimeoutEvent not implemented");
	protected virtual void SendConnectionTrigger(Span<byte> _0) =>
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
	protected virtual void GetUniquePadIds() =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetUniquePadIds not implemented");
	protected virtual KObject AcquireJoyDetachOnBluetoothOffEventHandle(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.AcquireJoyDetachOnBluetoothOffEventHandle not implemented");
	protected virtual void ListSixAxisSensorHandles(ulong _0) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.ListSixAxisSensorHandles not implemented");
	protected virtual byte IsSixAxisSensorUserCalibrationSupported() =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.IsSixAxisSensorUserCalibrationSupported not implemented");
	protected virtual void ResetSixAxisSensorCalibrationValues() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.ResetSixAxisSensorCalibrationValues");
	protected virtual void StartSixAxisSensorUserCalibration() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.StartSixAxisSensorUserCalibration");
	protected virtual void CancelSixAxisSensorUserCalibration() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.CancelSixAxisSensorUserCalibration");
	protected virtual void GetUniquePadBluetoothAddress(ulong _0) =>
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
	protected virtual void GetFirmwareVersion(ulong _0) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetFirmwareVersion not implemented");
	protected virtual void GetAvailableFirmwareVersion(ulong _0) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetAvailableFirmwareVersion not implemented");
	protected virtual byte IsFirmwareUpdateAvailable(ulong _0) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.IsFirmwareUpdateAvailable not implemented");
	protected virtual ulong CheckFirmwareUpdateRequired(ulong _0) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.CheckFirmwareUpdateRequired not implemented");
	protected virtual ulong StartFirmwareUpdate(ulong _0) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.StartFirmwareUpdate not implemented");
	protected virtual void AbortFirmwareUpdate() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.AbortFirmwareUpdate");
	protected virtual void GetFirmwareUpdateState(ulong _0) =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.GetFirmwareUpdateState not implemented");
	protected virtual void ActivateAudioControl() =>
		Console.WriteLine("Stub hit for Nn.Hid.IHidSystemServer.ActivateAudioControl");
	protected virtual KObject AcquireAudioControlEventHandle() =>
		throw new NotImplementedException("Nn.Hid.IHidSystemServer.AcquireAudioControlEventHandle not implemented");
	protected virtual void GetAudioControlStates() =>
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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1F: // SendKeyboardLockKeyEvent
				break;
			case 0x65: // AcquireHomeButtonEventHandle
				break;
			case 0x6F: // ActivateHomeButton
				break;
			case 0x79: // AcquireSleepButtonEventHandle
				break;
			case 0x83: // ActivateSleepButton
				break;
			case 0x8D: // AcquireCaptureButtonEventHandle
				break;
			case 0x97: // ActivateCaptureButton
				break;
			case 0xD2: // AcquireNfcDeviceUpdateEventHandle
				break;
			case 0xD3: // GetNpadsWithNfc
				break;
			case 0xD4: // AcquireNfcActivateEventHandle
				break;
			case 0xD5: // ActivateNfc
				break;
			case 0xD6: // GetXcdHandleForNpadWithNfc
				break;
			case 0xD7: // IsNfcActivated
				break;
			case 0xE6: // AcquireIrSensorEventHandle
				break;
			case 0xE7: // ActivateIrSensor
				break;
			case 0x12D: // ActivateNpadSystem
				break;
			case 0x12F: // ApplyNpadSystemCommonPolicy
				break;
			case 0x130: // EnableAssigningSingleOnSlSrPress
				break;
			case 0x131: // DisableAssigningSingleOnSlSrPress
				break;
			case 0x132: // GetLastActiveNpad
				break;
			case 0x133: // GetNpadSystemExtStyle
				break;
			case 0x134: // ApplyNpadSystemCommonPolicyFull
				break;
			case 0x135: // GetNpadFullKeyGripColor
				break;
			case 0x137: // SetNpadPlayerLedBlinkingDevice
				break;
			case 0x141: // GetUniquePadsFromNpad
				break;
			case 0x142: // GetIrSensorState
				break;
			case 0x143: // GetXcdHandleForNpadWithIrSensor
				break;
			case 0x1F4: // SetAppletResourceUserId
				break;
			case 0x1F5: // RegisterAppletResourceUserId
				break;
			case 0x1F6: // UnregisterAppletResourceUserId
				break;
			case 0x1F7: // EnableAppletToGetInput
				break;
			case 0x1F8: // SetAruidValidForVibration
				break;
			case 0x1F9: // EnableAppletToGetSixAxisSensor
				break;
			case 0x1FE: // SetVibrationMasterVolume
				break;
			case 0x1FF: // GetVibrationMasterVolume
				break;
			case 0x200: // BeginPermitVibrationSession
				break;
			case 0x201: // EndPermitVibrationSession
				break;
			case 0x208: // EnableHandheldHids
				break;
			case 0x209: // DisableHandheldHids
				break;
			case 0x21C: // AcquirePlayReportControllerUsageUpdateEvent
				break;
			case 0x21D: // GetPlayReportControllerUsages
				break;
			case 0x21E: // AcquirePlayReportRegisteredDeviceUpdateEvent
				break;
			case 0x21F: // GetRegisteredDevicesOld
				break;
			case 0x220: // AcquireConnectionTriggerTimeoutEvent
				break;
			case 0x221: // SendConnectionTrigger
				break;
			case 0x222: // AcquireDeviceRegisteredEventForControllerSupport
				break;
			case 0x223: // GetAllowedBluetoothLinksCount
				break;
			case 0x224: // GetRegisteredDevices
				break;
			case 0x2BC: // ActivateUniquePad
				break;
			case 0x2BE: // AcquireUniquePadConnectionEventHandle
				break;
			case 0x2BF: // GetUniquePadIds
				break;
			case 0x2EF: // AcquireJoyDetachOnBluetoothOffEventHandle
				break;
			case 0x320: // ListSixAxisSensorHandles
				break;
			case 0x321: // IsSixAxisSensorUserCalibrationSupported
				break;
			case 0x322: // ResetSixAxisSensorCalibrationValues
				break;
			case 0x323: // StartSixAxisSensorUserCalibration
				break;
			case 0x324: // CancelSixAxisSensorUserCalibration
				break;
			case 0x325: // GetUniquePadBluetoothAddress
				break;
			case 0x326: // DisconnectUniquePad
				break;
			case 0x327: // GetUniquePadType
				break;
			case 0x328: // GetUniquePadInterface
				break;
			case 0x329: // GetUniquePadSerialNumber
				break;
			case 0x32A: // GetUniquePadControllerNumber
				break;
			case 0x32B: // GetSixAxisSensorUserCalibrationStage
				break;
			case 0x335: // StartAnalogStickManualCalibration
				break;
			case 0x336: // RetryCurrentAnalogStickManualCalibrationStage
				break;
			case 0x337: // CancelAnalogStickManualCalibration
				break;
			case 0x338: // ResetAnalogStickManualCalibration
				break;
			case 0x339: // GetAnalogStickState
				break;
			case 0x33A: // GetAnalogStickManualCalibrationStage
				break;
			case 0x33B: // IsAnalogStickButtonPressed
				break;
			case 0x33C: // IsAnalogStickInReleasePosition
				break;
			case 0x33D: // IsAnalogStickInCircumference
				break;
			case 0x352: // IsUsbFullKeyControllerEnabled
				break;
			case 0x353: // EnableUsbFullKeyController
				break;
			case 0x354: // IsUsbConnected
				break;
			case 0x384: // ActivateInputDetector
				break;
			case 0x385: // NotifyInputDetector
				break;
			case 0x3E8: // InitializeFirmwareUpdate
				break;
			case 0x3E9: // GetFirmwareVersion
				break;
			case 0x3EA: // GetAvailableFirmwareVersion
				break;
			case 0x3EB: // IsFirmwareUpdateAvailable
				break;
			case 0x3EC: // CheckFirmwareUpdateRequired
				break;
			case 0x3ED: // StartFirmwareUpdate
				break;
			case 0x3EE: // AbortFirmwareUpdate
				break;
			case 0x3EF: // GetFirmwareUpdateState
				break;
			case 0x3F0: // ActivateAudioControl
				break;
			case 0x3F1: // AcquireAudioControlEventHandle
				break;
			case 0x3F2: // GetAudioControlStates
				break;
			case 0x3F3: // DeactivateAudioControl
				break;
			case 0x41A: // IsSixAxisSensorAccurateUserCalibrationSupported
				break;
			case 0x41B: // StartSixAxisSensorAccurateUserCalibration
				break;
			case 0x41C: // CancelSixAxisSensorAccurateUserCalibration
				break;
			case 0x41D: // GetSixAxisSensorAccurateUserCalibrationState
				break;
			case 0x44C: // GetHidbusSystemServiceObject
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Hid.IHidSystemServer");
		}
	}
}

public partial class IHidTemporaryServer : _IHidTemporaryServer_Base;
public abstract class _IHidTemporaryServer_Base : IpcInterface {
	protected virtual void GetConsoleSixAxisSensorCalibrationValues(uint _0, ulong _1, ulong _2) =>
		throw new NotImplementedException("Nn.Hid.IHidTemporaryServer.GetConsoleSixAxisSensorCalibrationValues not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetConsoleSixAxisSensorCalibrationValues
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Hid.IHidTemporaryServer");
		}
	}
}

