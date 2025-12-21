using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Settings;
public partial class IFactorySettingsServer : _IFactorySettingsServer_Base;
public abstract class _IFactorySettingsServer_Base : IpcInterface {
	protected virtual void GetBluetoothBdAddress() =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetBluetoothBdAddress not implemented");
	protected virtual void GetConfigurationId1() =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetConfigurationId1 not implemented");
	protected virtual void GetAccelerometerOffset() =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetAccelerometerOffset not implemented");
	protected virtual void GetAccelerometerScale() =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetAccelerometerScale not implemented");
	protected virtual void GetGyroscopeOffset() =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetGyroscopeOffset not implemented");
	protected virtual void GetGyroscopeScale() =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetGyroscopeScale not implemented");
	protected virtual void GetWirelessLanMacAddress() =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetWirelessLanMacAddress not implemented");
	protected virtual uint GetWirelessLanCountryCodeCount() =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetWirelessLanCountryCodeCount not implemented");
	protected virtual void GetWirelessLanCountryCodes() =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetWirelessLanCountryCodes not implemented");
	protected virtual void GetSerialNumber() =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetSerialNumber not implemented");
	protected virtual void SetInitialSystemAppletProgramId(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.IFactorySettingsServer.SetInitialSystemAppletProgramId");
	protected virtual void SetOverlayDispProgramId(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.IFactorySettingsServer.SetOverlayDispProgramId");
	protected virtual void GetBatteryLot() =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetBatteryLot not implemented");
	protected virtual void GetEciDeviceCertificate() =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetEciDeviceCertificate not implemented");
	protected virtual void GetEticketDeviceCertificate() =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetEticketDeviceCertificate not implemented");
	protected virtual void GetSslKey() =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetSslKey not implemented");
	protected virtual void GetSslCertificate() =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetSslCertificate not implemented");
	protected virtual void GetGameCardKey() =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetGameCardKey not implemented");
	protected virtual void GetGameCardCertificate() =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetGameCardCertificate not implemented");
	protected virtual void GetEciDeviceKey() =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetEciDeviceKey not implemented");
	protected virtual void GetEticketDeviceKey() =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetEticketDeviceKey not implemented");
	protected virtual void GetSpeakerParameter() =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetSpeakerParameter not implemented");
	protected virtual uint GetLcdVendorId() =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetLcdVendorId not implemented");
	protected virtual void GetEciDeviceCertificate2() =>
		Console.WriteLine("Stub hit for Nn.Settings.IFactorySettingsServer.GetEciDeviceCertificate2");
	protected virtual void GetEciDeviceKey2() =>
		Console.WriteLine("Stub hit for Nn.Settings.IFactorySettingsServer.GetEciDeviceKey2");
	protected virtual void GetAmiiboKey() =>
		Console.WriteLine("Stub hit for Nn.Settings.IFactorySettingsServer.GetAmiiboKey");
	protected virtual void GetAmiiboEcqvCertificate() =>
		Console.WriteLine("Stub hit for Nn.Settings.IFactorySettingsServer.GetAmiiboEcqvCertificate");
	protected virtual void GetAmiiboEcdsaCertificate() =>
		Console.WriteLine("Stub hit for Nn.Settings.IFactorySettingsServer.GetAmiiboEcdsaCertificate");
	protected virtual void GetAmiiboEcqvBlsKey() =>
		Console.WriteLine("Stub hit for Nn.Settings.IFactorySettingsServer.GetAmiiboEcqvBlsKey");
	protected virtual void GetAmiiboEcqvBlsCertificate() =>
		Console.WriteLine("Stub hit for Nn.Settings.IFactorySettingsServer.GetAmiiboEcqvBlsCertificate");
	protected virtual void GetAmiiboEcqvBlsRootCertificate() =>
		Console.WriteLine("Stub hit for Nn.Settings.IFactorySettingsServer.GetAmiiboEcqvBlsRootCertificate");
	protected virtual void GetUnknownId() =>
		Console.WriteLine("Stub hit for Nn.Settings.IFactorySettingsServer.GetUnknownId");
	protected virtual void GetUnknownId2() =>
		Console.WriteLine("Stub hit for Nn.Settings.IFactorySettingsServer.GetUnknownId2");
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
	protected virtual void SetSettingsItemValue(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Settings.IFirmwareDebugSettingsServer.SetSettingsItemValue");
	protected virtual void ResetSettingsItemValue(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Settings.IFirmwareDebugSettingsServer.ResetSettingsItemValue");
	protected virtual Nn.Settings.ISettingsItemKeyIterator CreateSettingsItemKeyIterator(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Settings.IFirmwareDebugSettingsServer.CreateSettingsItemKeyIterator not implemented");
	protected virtual void ReadSettings(uint _0) =>
		throw new NotImplementedException("Nn.Settings.IFirmwareDebugSettingsServer.ReadSettings not implemented");
	protected virtual void ResetSettings(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.IFirmwareDebugSettingsServer.ResetSettings");
	protected virtual void SetWebInspectorFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.IFirmwareDebugSettingsServer.SetWebInspectorFlag");
	protected virtual void SetAllowedSslHosts(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.IFirmwareDebugSettingsServer.SetAllowedSslHosts");
	protected virtual void SetHostFsMountPoint(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.IFirmwareDebugSettingsServer.SetHostFsMountPoint");
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
	protected virtual void GoNext() =>
		Console.WriteLine("Stub hit for Nn.Settings.ISettingsItemKeyIterator.GoNext");
	protected virtual ulong GetKeySize() =>
		throw new NotImplementedException("Nn.Settings.ISettingsItemKeyIterator.GetKeySize not implemented");
	protected virtual void GetKey() =>
		throw new NotImplementedException("Nn.Settings.ISettingsItemKeyIterator.GetKey not implemented");
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
	protected virtual void GetLanguageCode() =>
		throw new NotImplementedException("Nn.Settings.ISettingsServer.GetLanguageCode not implemented");
	protected virtual void GetAvailableLanguageCodes() =>
		throw new NotImplementedException("Nn.Settings.ISettingsServer.GetAvailableLanguageCodes not implemented");
	protected virtual void MakeLanguageCode(uint _0) =>
		throw new NotImplementedException("Nn.Settings.ISettingsServer.MakeLanguageCode not implemented");
	protected virtual uint GetAvailableLanguageCodeCount() =>
		throw new NotImplementedException("Nn.Settings.ISettingsServer.GetAvailableLanguageCodeCount not implemented");
	protected virtual uint GetRegionCode() =>
		throw new NotImplementedException("Nn.Settings.ISettingsServer.GetRegionCode not implemented");
	protected virtual void GetAvailableLanguageCodes2() =>
		throw new NotImplementedException("Nn.Settings.ISettingsServer.GetAvailableLanguageCodes2 not implemented");
	protected virtual uint GetAvailableLanguageCodeCount2() =>
		throw new NotImplementedException("Nn.Settings.ISettingsServer.GetAvailableLanguageCodeCount2 not implemented");
	protected virtual void GetKeyCodeMap() =>
		throw new NotImplementedException("Nn.Settings.ISettingsServer.GetKeyCodeMap not implemented");
	protected virtual void GetQuestFlag() =>
		Console.WriteLine("Stub hit for Nn.Settings.ISettingsServer.GetQuestFlag");
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
	protected virtual void SetLanguageCode(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetLanguageCode");
	protected virtual void SetNetworkSettings(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetNetworkSettings");
	protected virtual void GetNetworkSettings() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetNetworkSettings not implemented");
	protected virtual void GetFirmwareVersion() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetFirmwareVersion not implemented");
	protected virtual void GetFirmwareVersion2() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetFirmwareVersion2 not implemented");
	protected virtual void GetFirmwareVersionDigest() =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.GetFirmwareVersionDigest");
	protected virtual byte GetLockScreenFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetLockScreenFlag not implemented");
	protected virtual void SetLockScreenFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetLockScreenFlag");
	protected virtual void GetBacklightSettings() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetBacklightSettings not implemented");
	protected virtual void SetBacklightSettings(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetBacklightSettings");
	protected virtual void SetBluetoothDevicesSettings(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetBluetoothDevicesSettings");
	protected virtual void GetBluetoothDevicesSettings() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetBluetoothDevicesSettings not implemented");
	protected virtual void GetExternalSteadyClockSourceId() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetExternalSteadyClockSourceId not implemented");
	protected virtual void SetExternalSteadyClockSourceId(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetExternalSteadyClockSourceId");
	protected virtual void GetUserSystemClockContext() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetUserSystemClockContext not implemented");
	protected virtual void SetUserSystemClockContext(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetUserSystemClockContext");
	protected virtual uint GetAccountSettings() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetAccountSettings not implemented");
	protected virtual void SetAccountSettings(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetAccountSettings");
	protected virtual void GetAudioVolume(uint _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetAudioVolume not implemented");
	protected virtual void SetAudioVolume(Span<byte> _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetAudioVolume");
	protected virtual void GetEulaVersions() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetEulaVersions not implemented");
	protected virtual void SetEulaVersions(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetEulaVersions");
	protected virtual uint GetColorSetId() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetColorSetId not implemented");
	protected virtual void SetColorSetId(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetColorSetId");
	protected virtual byte GetConsoleInformationUploadFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetConsoleInformationUploadFlag not implemented");
	protected virtual void SetConsoleInformationUploadFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetConsoleInformationUploadFlag");
	protected virtual byte GetAutomaticApplicationDownloadFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetAutomaticApplicationDownloadFlag not implemented");
	protected virtual void SetAutomaticApplicationDownloadFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetAutomaticApplicationDownloadFlag");
	protected virtual void GetNotificationSettings() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetNotificationSettings not implemented");
	protected virtual void SetNotificationSettings(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetNotificationSettings");
	protected virtual void GetAccountNotificationSettings() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetAccountNotificationSettings not implemented");
	protected virtual void SetAccountNotificationSettings(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetAccountNotificationSettings");
	protected virtual float GetVibrationMasterVolume() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetVibrationMasterVolume not implemented");
	protected virtual void SetVibrationMasterVolume(float _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetVibrationMasterVolume");
	protected virtual ulong GetSettingsItemValueSize(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetSettingsItemValueSize not implemented");
	protected virtual void GetSettingsItemValue(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetSettingsItemValue not implemented");
	protected virtual void GetTvSettings() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetTvSettings not implemented");
	protected virtual void SetTvSettings(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetTvSettings");
	protected virtual void GetEdid() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetEdid not implemented");
	protected virtual void SetEdid(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetEdid");
	protected virtual uint GetAudioOutputMode(uint _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetAudioOutputMode not implemented");
	protected virtual void SetAudioOutputMode(uint _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetAudioOutputMode");
	protected virtual byte IsForceMuteOnHeadphoneRemoved() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.IsForceMuteOnHeadphoneRemoved not implemented");
	protected virtual void SetForceMuteOnHeadphoneRemoved(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetForceMuteOnHeadphoneRemoved");
	protected virtual byte GetQuestFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetQuestFlag not implemented");
	protected virtual void SetQuestFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetQuestFlag");
	protected virtual void GetDataDeletionSettings() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetDataDeletionSettings not implemented");
	protected virtual void SetDataDeletionSettings(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetDataDeletionSettings");
	protected virtual ulong GetInitialSystemAppletProgramId() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetInitialSystemAppletProgramId not implemented");
	protected virtual ulong GetOverlayDispProgramId() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetOverlayDispProgramId not implemented");
	protected virtual void GetDeviceTimeZoneLocationName() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetDeviceTimeZoneLocationName not implemented");
	protected virtual void SetDeviceTimeZoneLocationName(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetDeviceTimeZoneLocationName");
	protected virtual ulong GetWirelessCertificationFileSize() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetWirelessCertificationFileSize not implemented");
	protected virtual void GetWirelessCertificationFile() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetWirelessCertificationFile not implemented");
	protected virtual void SetRegionCode(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetRegionCode");
	protected virtual void GetNetworkSystemClockContext() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetNetworkSystemClockContext not implemented");
	protected virtual void SetNetworkSystemClockContext(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetNetworkSystemClockContext");
	protected virtual byte IsUserSystemClockAutomaticCorrectionEnabled() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.IsUserSystemClockAutomaticCorrectionEnabled not implemented");
	protected virtual void SetUserSystemClockAutomaticCorrectionEnabled(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetUserSystemClockAutomaticCorrectionEnabled");
	protected virtual byte GetDebugModeFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetDebugModeFlag not implemented");
	protected virtual uint GetPrimaryAlbumStorage() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetPrimaryAlbumStorage not implemented");
	protected virtual void SetPrimaryAlbumStorage(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetPrimaryAlbumStorage");
	protected virtual byte GetUsb30EnableFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetUsb30EnableFlag not implemented");
	protected virtual void SetUsb30EnableFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetUsb30EnableFlag");
	protected virtual void GetBatteryLot() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetBatteryLot not implemented");
	protected virtual void GetSerialNumber() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetSerialNumber not implemented");
	protected virtual byte GetNfcEnableFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetNfcEnableFlag not implemented");
	protected virtual void SetNfcEnableFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetNfcEnableFlag");
	protected virtual void GetSleepSettings() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetSleepSettings not implemented");
	protected virtual void SetSleepSettings(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetSleepSettings");
	protected virtual byte GetWirelessLanEnableFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetWirelessLanEnableFlag not implemented");
	protected virtual void SetWirelessLanEnableFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetWirelessLanEnableFlag");
	protected virtual void GetInitialLaunchSettings() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetInitialLaunchSettings not implemented");
	protected virtual void SetInitialLaunchSettings(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetInitialLaunchSettings");
	protected virtual void GetDeviceNickName() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetDeviceNickName not implemented");
	protected virtual void SetDeviceNickName(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetDeviceNickName");
	protected virtual uint GetProductModel() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetProductModel not implemented");
	protected virtual uint GetLdnChannel() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetLdnChannel not implemented");
	protected virtual void SetLdnChannel(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetLdnChannel");
	protected virtual KObject AcquireTelemetryDirtyFlagEventHandle() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.AcquireTelemetryDirtyFlagEventHandle not implemented");
	protected virtual void GetTelemetryDirtyFlags() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetTelemetryDirtyFlags not implemented");
	protected virtual void GetPtmBatteryLot() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetPtmBatteryLot not implemented");
	protected virtual void SetPtmBatteryLot(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetPtmBatteryLot");
	protected virtual void GetPtmFuelGaugeParameter() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetPtmFuelGaugeParameter not implemented");
	protected virtual void SetPtmFuelGaugeParameter(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetPtmFuelGaugeParameter");
	protected virtual byte GetBluetoothEnableFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetBluetoothEnableFlag not implemented");
	protected virtual void SetBluetoothEnableFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetBluetoothEnableFlag");
	protected virtual void GetMiiAuthorId() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetMiiAuthorId not implemented");
	protected virtual void SetShutdownRtcValue(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetShutdownRtcValue");
	protected virtual ulong GetShutdownRtcValue() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetShutdownRtcValue not implemented");
	protected virtual KObject AcquireFatalDirtyFlagEventHandle() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.AcquireFatalDirtyFlagEventHandle not implemented");
	protected virtual void GetFatalDirtyFlags() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetFatalDirtyFlags not implemented");
	protected virtual byte GetAutoUpdateEnableFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetAutoUpdateEnableFlag not implemented");
	protected virtual void SetAutoUpdateEnableFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetAutoUpdateEnableFlag");
	protected virtual void GetNxControllerSettings() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetNxControllerSettings not implemented");
	protected virtual void SetNxControllerSettings(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetNxControllerSettings");
	protected virtual byte GetBatteryPercentageFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetBatteryPercentageFlag not implemented");
	protected virtual void SetBatteryPercentageFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetBatteryPercentageFlag");
	protected virtual byte GetExternalRtcResetFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetExternalRtcResetFlag not implemented");
	protected virtual void SetExternalRtcResetFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetExternalRtcResetFlag");
	protected virtual byte GetUsbFullKeyEnableFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetUsbFullKeyEnableFlag not implemented");
	protected virtual void SetUsbFullKeyEnableFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetUsbFullKeyEnableFlag");
	protected virtual void SetExternalSteadyClockInternalOffset(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetExternalSteadyClockInternalOffset");
	protected virtual ulong GetExternalSteadyClockInternalOffset() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetExternalSteadyClockInternalOffset not implemented");
	protected virtual void GetBacklightSettingsEx() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetBacklightSettingsEx not implemented");
	protected virtual void SetBacklightSettingsEx(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetBacklightSettingsEx");
	protected virtual uint GetHeadphoneVolumeWarningCount() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetHeadphoneVolumeWarningCount not implemented");
	protected virtual void SetHeadphoneVolumeWarningCount(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetHeadphoneVolumeWarningCount");
	protected virtual byte GetBluetoothAfhEnableFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetBluetoothAfhEnableFlag not implemented");
	protected virtual void SetBluetoothAfhEnableFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetBluetoothAfhEnableFlag");
	protected virtual byte GetBluetoothBoostEnableFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetBluetoothBoostEnableFlag not implemented");
	protected virtual void SetBluetoothBoostEnableFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetBluetoothBoostEnableFlag");
	protected virtual byte GetInRepairProcessEnableFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetInRepairProcessEnableFlag not implemented");
	protected virtual void SetInRepairProcessEnableFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetInRepairProcessEnableFlag");
	protected virtual byte GetHeadphoneVolumeUpdateFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetHeadphoneVolumeUpdateFlag not implemented");
	protected virtual void SetHeadphoneVolumeUpdateFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetHeadphoneVolumeUpdateFlag");
	protected virtual void NeedsToUpdateHeadphoneVolume(byte _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.NeedsToUpdateHeadphoneVolume not implemented");
	protected virtual uint GetPushNotificationActivityModeOnSleep() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetPushNotificationActivityModeOnSleep not implemented");
	protected virtual void SetPushNotificationActivityModeOnSleep(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetPushNotificationActivityModeOnSleep");
	protected virtual uint GetServiceDiscoveryControlSettings() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetServiceDiscoveryControlSettings not implemented");
	protected virtual void SetServiceDiscoveryControlSettings(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetServiceDiscoveryControlSettings");
	protected virtual uint GetErrorReportSharePermission() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetErrorReportSharePermission not implemented");
	protected virtual void SetErrorReportSharePermission(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetErrorReportSharePermission");
	protected virtual uint GetAppletLaunchFlags() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetAppletLaunchFlags not implemented");
	protected virtual void SetAppletLaunchFlags(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetAppletLaunchFlags");
	protected virtual void GetConsoleSixAxisSensorAccelerationBias() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetConsoleSixAxisSensorAccelerationBias not implemented");
	protected virtual void SetConsoleSixAxisSensorAccelerationBias(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetConsoleSixAxisSensorAccelerationBias");
	protected virtual void GetConsoleSixAxisSensorAngularVelocityBias() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetConsoleSixAxisSensorAngularVelocityBias not implemented");
	protected virtual void SetConsoleSixAxisSensorAngularVelocityBias(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetConsoleSixAxisSensorAngularVelocityBias");
	protected virtual void GetConsoleSixAxisSensorAccelerationGain() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetConsoleSixAxisSensorAccelerationGain not implemented");
	protected virtual void SetConsoleSixAxisSensorAccelerationGain(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetConsoleSixAxisSensorAccelerationGain");
	protected virtual void GetConsoleSixAxisSensorAngularVelocityGain() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetConsoleSixAxisSensorAngularVelocityGain not implemented");
	protected virtual void SetConsoleSixAxisSensorAngularVelocityGain(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetConsoleSixAxisSensorAngularVelocityGain");
	protected virtual uint GetKeyboardLayout() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetKeyboardLayout not implemented");
	protected virtual void SetKeyboardLayout(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetKeyboardLayout");
	protected virtual byte GetWebInspectorFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetWebInspectorFlag not implemented");
	protected virtual void GetAllowedSslHosts() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetAllowedSslHosts not implemented");
	protected virtual void GetHostFsMountPoint() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetHostFsMountPoint not implemented");
	protected virtual void GetRequiresRunRepairTimeReviser() =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.GetRequiresRunRepairTimeReviser");
	protected virtual void SetRequiresRunRepairTimeReviser() =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetRequiresRunRepairTimeReviser");
	protected virtual void SetBlePairingSettings() =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetBlePairingSettings");
	protected virtual void GetBlePairingSettings() =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.GetBlePairingSettings");
	protected virtual void GetConsoleSixAxisSensorAngularVelocityTimeBias() =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.GetConsoleSixAxisSensorAngularVelocityTimeBias");
	protected virtual void SetConsoleSixAxisSensorAngularVelocityTimeBias() =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetConsoleSixAxisSensorAngularVelocityTimeBias");
	protected virtual void GetConsoleSixAxisSensorAngularAcceleration() =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.GetConsoleSixAxisSensorAngularAcceleration");
	protected virtual void SetConsoleSixAxisSensorAngularAcceleration() =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetConsoleSixAxisSensorAngularAcceleration");
	protected virtual void GetRebootlessSystemUpdateVersion() =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.GetRebootlessSystemUpdateVersion");
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

