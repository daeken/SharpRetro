using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Hid;
public partial class IActiveVibrationDeviceList : _IActiveVibrationDeviceList_Base;
public abstract class _IActiveVibrationDeviceList_Base : IpcInterface {
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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetConsoleSixAxisSensorCalibrationValues
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Hid.IHidTemporaryServer");
		}
	}
}

