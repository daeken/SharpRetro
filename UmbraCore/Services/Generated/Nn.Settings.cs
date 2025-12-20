using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Settings;
public partial class IFactorySettingsServer : _IFactorySettingsServer_Base;
public abstract class _IFactorySettingsServer_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetBluetoothBdAddress
				break;
			case 0x1: // GetConfigurationId1
				break;
			case 0x2: // GetAccelerometerOffset
				break;
			case 0x3: // GetAccelerometerScale
				break;
			case 0x4: // GetGyroscopeOffset
				break;
			case 0x5: // GetGyroscopeScale
				break;
			case 0x6: // GetWirelessLanMacAddress
				break;
			case 0x7: // GetWirelessLanCountryCodeCount
				break;
			case 0x8: // GetWirelessLanCountryCodes
				break;
			case 0x9: // GetSerialNumber
				break;
			case 0xA: // SetInitialSystemAppletProgramId
				break;
			case 0xB: // SetOverlayDispProgramId
				break;
			case 0xC: // GetBatteryLot
				break;
			case 0xE: // GetEciDeviceCertificate
				break;
			case 0xF: // GetEticketDeviceCertificate
				break;
			case 0x10: // GetSslKey
				break;
			case 0x11: // GetSslCertificate
				break;
			case 0x12: // GetGameCardKey
				break;
			case 0x13: // GetGameCardCertificate
				break;
			case 0x14: // GetEciDeviceKey
				break;
			case 0x15: // GetEticketDeviceKey
				break;
			case 0x16: // GetSpeakerParameter
				break;
			case 0x17: // GetLcdVendorId
				break;
			case 0x18: // GetEciDeviceCertificate2
				break;
			case 0x19: // GetEciDeviceKey2
				break;
			case 0x1A: // GetAmiiboKey
				break;
			case 0x1B: // GetAmiiboEcqvCertificate
				break;
			case 0x1C: // GetAmiiboEcdsaCertificate
				break;
			case 0x1D: // GetAmiiboEcqvBlsKey
				break;
			case 0x1E: // GetAmiiboEcqvBlsCertificate
				break;
			case 0x1F: // GetAmiiboEcqvBlsRootCertificate
				break;
			case 0x20: // GetUnknownId
				break;
			case 0x21: // GetUnknownId2
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Settings.IFactorySettingsServer");
		}
	}
}

public partial class IFirmwareDebugSettingsServer : _IFirmwareDebugSettingsServer_Base;
public abstract class _IFirmwareDebugSettingsServer_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x2: // SetSettingsItemValue
				break;
			case 0x3: // ResetSettingsItemValue
				break;
			case 0x4: // CreateSettingsItemKeyIterator
				break;
			case 0xA: // ReadSettings
				break;
			case 0xB: // ResetSettings
				break;
			case 0x14: // SetWebInspectorFlag
				break;
			case 0x15: // SetAllowedSslHosts
				break;
			case 0x16: // SetHostFsMountPoint
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Settings.IFirmwareDebugSettingsServer");
		}
	}
}

public partial class ISettingsItemKeyIterator : _ISettingsItemKeyIterator_Base;
public abstract class _ISettingsItemKeyIterator_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GoNext
				break;
			case 0x1: // GetKeySize
				break;
			case 0x2: // GetKey
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Settings.ISettingsItemKeyIterator");
		}
	}
}

public partial class ISettingsServer : _ISettingsServer_Base;
public abstract class _ISettingsServer_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetLanguageCode
				break;
			case 0x1: // GetAvailableLanguageCodes
				break;
			case 0x2: // MakeLanguageCode
				break;
			case 0x3: // GetAvailableLanguageCodeCount
				break;
			case 0x4: // GetRegionCode
				break;
			case 0x5: // GetAvailableLanguageCodes2
				break;
			case 0x6: // GetAvailableLanguageCodeCount2
				break;
			case 0x7: // GetKeyCodeMap
				break;
			case 0x8: // GetQuestFlag
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Settings.ISettingsServer");
		}
	}
}

public partial class ISystemSettingsServer : _ISystemSettingsServer_Base;
public abstract class _ISystemSettingsServer_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // SetLanguageCode
				break;
			case 0x1: // SetNetworkSettings
				break;
			case 0x2: // GetNetworkSettings
				break;
			case 0x3: // GetFirmwareVersion
				break;
			case 0x4: // GetFirmwareVersion2
				break;
			case 0x5: // GetFirmwareVersionDigest
				break;
			case 0x7: // GetLockScreenFlag
				break;
			case 0x8: // SetLockScreenFlag
				break;
			case 0x9: // GetBacklightSettings
				break;
			case 0xA: // SetBacklightSettings
				break;
			case 0xB: // SetBluetoothDevicesSettings
				break;
			case 0xC: // GetBluetoothDevicesSettings
				break;
			case 0xD: // GetExternalSteadyClockSourceId
				break;
			case 0xE: // SetExternalSteadyClockSourceId
				break;
			case 0xF: // GetUserSystemClockContext
				break;
			case 0x10: // SetUserSystemClockContext
				break;
			case 0x11: // GetAccountSettings
				break;
			case 0x12: // SetAccountSettings
				break;
			case 0x13: // GetAudioVolume
				break;
			case 0x14: // SetAudioVolume
				break;
			case 0x15: // GetEulaVersions
				break;
			case 0x16: // SetEulaVersions
				break;
			case 0x17: // GetColorSetId
				break;
			case 0x18: // SetColorSetId
				break;
			case 0x19: // GetConsoleInformationUploadFlag
				break;
			case 0x1A: // SetConsoleInformationUploadFlag
				break;
			case 0x1B: // GetAutomaticApplicationDownloadFlag
				break;
			case 0x1C: // SetAutomaticApplicationDownloadFlag
				break;
			case 0x1D: // GetNotificationSettings
				break;
			case 0x1E: // SetNotificationSettings
				break;
			case 0x1F: // GetAccountNotificationSettings
				break;
			case 0x20: // SetAccountNotificationSettings
				break;
			case 0x23: // GetVibrationMasterVolume
				break;
			case 0x24: // SetVibrationMasterVolume
				break;
			case 0x25: // GetSettingsItemValueSize
				break;
			case 0x26: // GetSettingsItemValue
				break;
			case 0x27: // GetTvSettings
				break;
			case 0x28: // SetTvSettings
				break;
			case 0x29: // GetEdid
				break;
			case 0x2A: // SetEdid
				break;
			case 0x2B: // GetAudioOutputMode
				break;
			case 0x2C: // SetAudioOutputMode
				break;
			case 0x2D: // IsForceMuteOnHeadphoneRemoved
				break;
			case 0x2E: // SetForceMuteOnHeadphoneRemoved
				break;
			case 0x2F: // GetQuestFlag
				break;
			case 0x30: // SetQuestFlag
				break;
			case 0x31: // GetDataDeletionSettings
				break;
			case 0x32: // SetDataDeletionSettings
				break;
			case 0x33: // GetInitialSystemAppletProgramId
				break;
			case 0x34: // GetOverlayDispProgramId
				break;
			case 0x35: // GetDeviceTimeZoneLocationName
				break;
			case 0x36: // SetDeviceTimeZoneLocationName
				break;
			case 0x37: // GetWirelessCertificationFileSize
				break;
			case 0x38: // GetWirelessCertificationFile
				break;
			case 0x39: // SetRegionCode
				break;
			case 0x3A: // GetNetworkSystemClockContext
				break;
			case 0x3B: // SetNetworkSystemClockContext
				break;
			case 0x3C: // IsUserSystemClockAutomaticCorrectionEnabled
				break;
			case 0x3D: // SetUserSystemClockAutomaticCorrectionEnabled
				break;
			case 0x3E: // GetDebugModeFlag
				break;
			case 0x3F: // GetPrimaryAlbumStorage
				break;
			case 0x40: // SetPrimaryAlbumStorage
				break;
			case 0x41: // GetUsb30EnableFlag
				break;
			case 0x42: // SetUsb30EnableFlag
				break;
			case 0x43: // GetBatteryLot
				break;
			case 0x44: // GetSerialNumber
				break;
			case 0x45: // GetNfcEnableFlag
				break;
			case 0x46: // SetNfcEnableFlag
				break;
			case 0x47: // GetSleepSettings
				break;
			case 0x48: // SetSleepSettings
				break;
			case 0x49: // GetWirelessLanEnableFlag
				break;
			case 0x4A: // SetWirelessLanEnableFlag
				break;
			case 0x4B: // GetInitialLaunchSettings
				break;
			case 0x4C: // SetInitialLaunchSettings
				break;
			case 0x4D: // GetDeviceNickName
				break;
			case 0x4E: // SetDeviceNickName
				break;
			case 0x4F: // GetProductModel
				break;
			case 0x50: // GetLdnChannel
				break;
			case 0x51: // SetLdnChannel
				break;
			case 0x52: // AcquireTelemetryDirtyFlagEventHandle
				break;
			case 0x53: // GetTelemetryDirtyFlags
				break;
			case 0x54: // GetPtmBatteryLot
				break;
			case 0x55: // SetPtmBatteryLot
				break;
			case 0x56: // GetPtmFuelGaugeParameter
				break;
			case 0x57: // SetPtmFuelGaugeParameter
				break;
			case 0x58: // GetBluetoothEnableFlag
				break;
			case 0x59: // SetBluetoothEnableFlag
				break;
			case 0x5A: // GetMiiAuthorId
				break;
			case 0x5B: // SetShutdownRtcValue
				break;
			case 0x5C: // GetShutdownRtcValue
				break;
			case 0x5D: // AcquireFatalDirtyFlagEventHandle
				break;
			case 0x5E: // GetFatalDirtyFlags
				break;
			case 0x5F: // GetAutoUpdateEnableFlag
				break;
			case 0x60: // SetAutoUpdateEnableFlag
				break;
			case 0x61: // GetNxControllerSettings
				break;
			case 0x62: // SetNxControllerSettings
				break;
			case 0x63: // GetBatteryPercentageFlag
				break;
			case 0x64: // SetBatteryPercentageFlag
				break;
			case 0x65: // GetExternalRtcResetFlag
				break;
			case 0x66: // SetExternalRtcResetFlag
				break;
			case 0x67: // GetUsbFullKeyEnableFlag
				break;
			case 0x68: // SetUsbFullKeyEnableFlag
				break;
			case 0x69: // SetExternalSteadyClockInternalOffset
				break;
			case 0x6A: // GetExternalSteadyClockInternalOffset
				break;
			case 0x6B: // GetBacklightSettingsEx
				break;
			case 0x6C: // SetBacklightSettingsEx
				break;
			case 0x6D: // GetHeadphoneVolumeWarningCount
				break;
			case 0x6E: // SetHeadphoneVolumeWarningCount
				break;
			case 0x6F: // GetBluetoothAfhEnableFlag
				break;
			case 0x70: // SetBluetoothAfhEnableFlag
				break;
			case 0x71: // GetBluetoothBoostEnableFlag
				break;
			case 0x72: // SetBluetoothBoostEnableFlag
				break;
			case 0x73: // GetInRepairProcessEnableFlag
				break;
			case 0x74: // SetInRepairProcessEnableFlag
				break;
			case 0x75: // GetHeadphoneVolumeUpdateFlag
				break;
			case 0x76: // SetHeadphoneVolumeUpdateFlag
				break;
			case 0x77: // NeedsToUpdateHeadphoneVolume
				break;
			case 0x78: // GetPushNotificationActivityModeOnSleep
				break;
			case 0x79: // SetPushNotificationActivityModeOnSleep
				break;
			case 0x7A: // GetServiceDiscoveryControlSettings
				break;
			case 0x7B: // SetServiceDiscoveryControlSettings
				break;
			case 0x7C: // GetErrorReportSharePermission
				break;
			case 0x7D: // SetErrorReportSharePermission
				break;
			case 0x7E: // GetAppletLaunchFlags
				break;
			case 0x7F: // SetAppletLaunchFlags
				break;
			case 0x80: // GetConsoleSixAxisSensorAccelerationBias
				break;
			case 0x81: // SetConsoleSixAxisSensorAccelerationBias
				break;
			case 0x82: // GetConsoleSixAxisSensorAngularVelocityBias
				break;
			case 0x83: // SetConsoleSixAxisSensorAngularVelocityBias
				break;
			case 0x84: // GetConsoleSixAxisSensorAccelerationGain
				break;
			case 0x85: // SetConsoleSixAxisSensorAccelerationGain
				break;
			case 0x86: // GetConsoleSixAxisSensorAngularVelocityGain
				break;
			case 0x87: // SetConsoleSixAxisSensorAngularVelocityGain
				break;
			case 0x88: // GetKeyboardLayout
				break;
			case 0x89: // SetKeyboardLayout
				break;
			case 0x8A: // GetWebInspectorFlag
				break;
			case 0x8B: // GetAllowedSslHosts
				break;
			case 0x8C: // GetHostFsMountPoint
				break;
			case 0x8D: // GetRequiresRunRepairTimeReviser
				break;
			case 0x8E: // SetRequiresRunRepairTimeReviser
				break;
			case 0x8F: // SetBlePairingSettings
				break;
			case 0x90: // GetBlePairingSettings
				break;
			case 0x91: // GetConsoleSixAxisSensorAngularVelocityTimeBias
				break;
			case 0x92: // SetConsoleSixAxisSensorAngularVelocityTimeBias
				break;
			case 0x93: // GetConsoleSixAxisSensorAngularAcceleration
				break;
			case 0x94: // SetConsoleSixAxisSensorAngularAcceleration
				break;
			case 0x95: // GetRebootlessSystemUpdateVersion
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Settings.ISystemSettingsServer");
		}
	}
}

