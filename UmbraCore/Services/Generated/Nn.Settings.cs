using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Settings;
public partial class IFactorySettingsServer : _IFactorySettingsServer_Base {
	public readonly string ServiceName;
	public IFactorySettingsServer(string serviceName) => ServiceName = serviceName;
}
public abstract class _IFactorySettingsServer_Base : IpcInterface {
	protected virtual void GetBluetoothBdAddress(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetBluetoothBdAddress not implemented");
	protected virtual void GetConfigurationId1(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetConfigurationId1 not implemented");
	protected virtual void GetAccelerometerOffset(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetAccelerometerOffset not implemented");
	protected virtual void GetAccelerometerScale(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetAccelerometerScale not implemented");
	protected virtual void GetGyroscopeOffset(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetGyroscopeOffset not implemented");
	protected virtual void GetGyroscopeScale(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetGyroscopeScale not implemented");
	protected virtual void GetWirelessLanMacAddress(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetWirelessLanMacAddress not implemented");
	protected virtual uint GetWirelessLanCountryCodeCount() =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetWirelessLanCountryCodeCount not implemented");
	protected virtual void GetWirelessLanCountryCodes(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetWirelessLanCountryCodes not implemented");
	protected virtual void GetSerialNumber(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetSerialNumber not implemented");
	protected virtual void SetInitialSystemAppletProgramId(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.IFactorySettingsServer.SetInitialSystemAppletProgramId");
	protected virtual void SetOverlayDispProgramId(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.IFactorySettingsServer.SetOverlayDispProgramId");
	protected virtual void GetBatteryLot(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetBatteryLot not implemented");
	protected virtual void GetEciDeviceCertificate(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetEciDeviceCertificate not implemented");
	protected virtual void GetEticketDeviceCertificate(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetEticketDeviceCertificate not implemented");
	protected virtual void GetSslKey(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetSslKey not implemented");
	protected virtual void GetSslCertificate(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetSslCertificate not implemented");
	protected virtual void GetGameCardKey(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetGameCardKey not implemented");
	protected virtual void GetGameCardCertificate(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetGameCardCertificate not implemented");
	protected virtual void GetEciDeviceKey(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetEciDeviceKey not implemented");
	protected virtual void GetEticketDeviceKey(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Settings.IFactorySettingsServer.GetEticketDeviceKey not implemented");
	protected virtual void GetSpeakerParameter(out byte[] _0) =>
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetBluetoothBdAddress
				GetBluetoothBdAddress(out var _0);
				om.Initialize(0, 0, 6);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // GetConfigurationId1
				GetConfigurationId1(out var _0);
				om.Initialize(0, 0, 30);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // GetAccelerometerOffset
				GetAccelerometerOffset(out var _0);
				om.Initialize(0, 0, 6);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // GetAccelerometerScale
				GetAccelerometerScale(out var _0);
				om.Initialize(0, 0, 6);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // GetGyroscopeOffset
				GetGyroscopeOffset(out var _0);
				om.Initialize(0, 0, 6);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // GetGyroscopeScale
				GetGyroscopeScale(out var _0);
				om.Initialize(0, 0, 6);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6: { // GetWirelessLanMacAddress
				GetWirelessLanMacAddress(out var _0);
				om.Initialize(0, 0, 6);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7: { // GetWirelessLanCountryCodeCount
				var _return = GetWirelessLanCountryCodeCount();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x8: { // GetWirelessLanCountryCodes
				GetWirelessLanCountryCodes(out var _0, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x9: { // GetSerialNumber
				GetSerialNumber(out var _0);
				om.Initialize(0, 0, 24);
				om.SetBytes(8, _0);
				break;
			}
			case 0xA: { // SetInitialSystemAppletProgramId
				SetInitialSystemAppletProgramId(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // SetOverlayDispProgramId
				SetOverlayDispProgramId(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // GetBatteryLot
				GetBatteryLot(out var _0);
				om.Initialize(0, 0, 24);
				om.SetBytes(8, _0);
				break;
			}
			case 0xE: { // GetEciDeviceCertificate
				GetEciDeviceCertificate(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xF: { // GetEticketDeviceCertificate
				GetEticketDeviceCertificate(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // GetSslKey
				GetSslKey(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x11: { // GetSslCertificate
				GetSslCertificate(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12: { // GetGameCardKey
				GetGameCardKey(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x13: { // GetGameCardCertificate
				GetGameCardCertificate(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x14: { // GetEciDeviceKey
				GetEciDeviceKey(out var _0);
				om.Initialize(0, 0, 84);
				om.SetBytes(8, _0);
				break;
			}
			case 0x15: { // GetEticketDeviceKey
				GetEticketDeviceKey(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x16: { // GetSpeakerParameter
				GetSpeakerParameter(out var _0);
				om.Initialize(0, 0, 90);
				om.SetBytes(8, _0);
				break;
			}
			case 0x17: { // GetLcdVendorId
				var _return = GetLcdVendorId();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x18: { // GetEciDeviceCertificate2
				GetEciDeviceCertificate2();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19: { // GetEciDeviceKey2
				GetEciDeviceKey2();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1A: { // GetAmiiboKey
				GetAmiiboKey();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1B: { // GetAmiiboEcqvCertificate
				GetAmiiboEcqvCertificate();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1C: { // GetAmiiboEcdsaCertificate
				GetAmiiboEcdsaCertificate();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1D: { // GetAmiiboEcqvBlsKey
				GetAmiiboEcqvBlsKey();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1E: { // GetAmiiboEcqvBlsCertificate
				GetAmiiboEcqvBlsCertificate();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1F: { // GetAmiiboEcqvBlsRootCertificate
				GetAmiiboEcqvBlsRootCertificate();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x20: { // GetUnknownId
				GetUnknownId();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x21: { // GetUnknownId2
				GetUnknownId2();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Settings.IFactorySettingsServer");
		}
	}
}

public partial class IFirmwareDebugSettingsServer : _IFirmwareDebugSettingsServer_Base {
	public readonly string ServiceName;
	public IFirmwareDebugSettingsServer(string serviceName) => ServiceName = serviceName;
}
public abstract class _IFirmwareDebugSettingsServer_Base : IpcInterface {
	protected virtual void SetSettingsItemValue(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Settings.IFirmwareDebugSettingsServer.SetSettingsItemValue");
	protected virtual void ResetSettingsItemValue(Span<byte> _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Settings.IFirmwareDebugSettingsServer.ResetSettingsItemValue");
	protected virtual Nn.Settings.ISettingsItemKeyIterator CreateSettingsItemKeyIterator(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Settings.IFirmwareDebugSettingsServer.CreateSettingsItemKeyIterator not implemented");
	protected virtual void ReadSettings(uint _0, out ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Settings.IFirmwareDebugSettingsServer.ReadSettings not implemented");
	protected virtual void ResetSettings(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.IFirmwareDebugSettingsServer.ResetSettings");
	protected virtual void SetWebInspectorFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.IFirmwareDebugSettingsServer.SetWebInspectorFlag");
	protected virtual void SetAllowedSslHosts(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.IFirmwareDebugSettingsServer.SetAllowedSslHosts");
	protected virtual void SetHostFsMountPoint(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.IFirmwareDebugSettingsServer.SetHostFsMountPoint");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x2: { // SetSettingsItemValue
				SetSettingsItemValue(im.GetSpan<byte>(0x19, 0), im.GetSpan<byte>(0x19, 1), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // ResetSettingsItemValue
				ResetSettingsItemValue(im.GetSpan<byte>(0x19, 0), im.GetSpan<byte>(0x19, 1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // CreateSettingsItemKeyIterator
				var _return = CreateSettingsItemKeyIterator(im.GetSpan<byte>(0x19, 0));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0xA: { // ReadSettings
				ReadSettings(im.GetData<uint>(8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				break;
			}
			case 0xB: { // ResetSettings
				ResetSettings(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x14: { // SetWebInspectorFlag
				SetWebInspectorFlag(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x15: { // SetAllowedSslHosts
				SetAllowedSslHosts(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x16: { // SetHostFsMountPoint
				SetHostFsMountPoint(im.GetSpan<byte>(0x15, 0));
				om.Initialize(0, 0, 0);
				break;
			}
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
	protected virtual void GetKey(out ulong _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Settings.ISettingsItemKeyIterator.GetKey not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GoNext
				GoNext();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // GetKeySize
				var _return = GetKeySize();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // GetKey
				GetKey(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Settings.ISettingsItemKeyIterator");
		}
	}
}

public partial class ISettingsServer : _ISettingsServer_Base {
	public readonly string ServiceName;
	public ISettingsServer(string serviceName) => ServiceName = serviceName;
}
public abstract class _ISettingsServer_Base : IpcInterface {
	protected virtual void GetLanguageCode(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISettingsServer.GetLanguageCode not implemented");
	protected virtual void GetAvailableLanguageCodes(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Settings.ISettingsServer.GetAvailableLanguageCodes not implemented");
	protected virtual void MakeLanguageCode(uint _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Settings.ISettingsServer.MakeLanguageCode not implemented");
	protected virtual uint GetAvailableLanguageCodeCount() =>
		throw new NotImplementedException("Nn.Settings.ISettingsServer.GetAvailableLanguageCodeCount not implemented");
	protected virtual uint GetRegionCode() =>
		throw new NotImplementedException("Nn.Settings.ISettingsServer.GetRegionCode not implemented");
	protected virtual void GetAvailableLanguageCodes2(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Settings.ISettingsServer.GetAvailableLanguageCodes2 not implemented");
	protected virtual uint GetAvailableLanguageCodeCount2() =>
		throw new NotImplementedException("Nn.Settings.ISettingsServer.GetAvailableLanguageCodeCount2 not implemented");
	protected virtual void GetKeyCodeMap(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Settings.ISettingsServer.GetKeyCodeMap not implemented");
	protected virtual void GetQuestFlag() =>
		Console.WriteLine("Stub hit for Nn.Settings.ISettingsServer.GetQuestFlag");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetLanguageCode
				GetLanguageCode(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // GetAvailableLanguageCodes
				GetAvailableLanguageCodes(out var _0, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x2: { // MakeLanguageCode
				MakeLanguageCode(im.GetData<uint>(8), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // GetAvailableLanguageCodeCount
				var _return = GetAvailableLanguageCodeCount();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x4: { // GetRegionCode
				var _return = GetRegionCode();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x5: { // GetAvailableLanguageCodes2
				GetAvailableLanguageCodes2(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x6: { // GetAvailableLanguageCodeCount2
				var _return = GetAvailableLanguageCodeCount2();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x7: { // GetKeyCodeMap
				GetKeyCodeMap(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // GetQuestFlag
				GetQuestFlag();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Settings.ISettingsServer");
		}
	}
}

public partial class ISystemSettingsServer : _ISystemSettingsServer_Base {
	public readonly string ServiceName;
	public ISystemSettingsServer(string serviceName) => ServiceName = serviceName;
}
public abstract class _ISystemSettingsServer_Base : IpcInterface {
	protected virtual void SetLanguageCode(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetLanguageCode");
	protected virtual void SetNetworkSettings(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetNetworkSettings");
	protected virtual void GetNetworkSettings(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetNetworkSettings not implemented");
	protected virtual void GetFirmwareVersion(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetFirmwareVersion not implemented");
	protected virtual void GetFirmwareVersion2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetFirmwareVersion2 not implemented");
	protected virtual void GetFirmwareVersionDigest() =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.GetFirmwareVersionDigest");
	protected virtual byte GetLockScreenFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetLockScreenFlag not implemented");
	protected virtual void SetLockScreenFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetLockScreenFlag");
	protected virtual void GetBacklightSettings(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetBacklightSettings not implemented");
	protected virtual void SetBacklightSettings(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetBacklightSettings");
	protected virtual void SetBluetoothDevicesSettings(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetBluetoothDevicesSettings");
	protected virtual void GetBluetoothDevicesSettings(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetBluetoothDevicesSettings not implemented");
	protected virtual void GetExternalSteadyClockSourceId(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetExternalSteadyClockSourceId not implemented");
	protected virtual void SetExternalSteadyClockSourceId(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetExternalSteadyClockSourceId");
	protected virtual void GetUserSystemClockContext(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetUserSystemClockContext not implemented");
	protected virtual void SetUserSystemClockContext(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetUserSystemClockContext");
	protected virtual uint GetAccountSettings() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetAccountSettings not implemented");
	protected virtual void SetAccountSettings(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetAccountSettings");
	protected virtual void GetAudioVolume(uint _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetAudioVolume not implemented");
	protected virtual void SetAudioVolume(byte[] _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetAudioVolume");
	protected virtual void GetEulaVersions(out uint _0, Span<byte> _1) =>
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
	protected virtual void GetNotificationSettings(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetNotificationSettings not implemented");
	protected virtual void SetNotificationSettings(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetNotificationSettings");
	protected virtual void GetAccountNotificationSettings(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetAccountNotificationSettings not implemented");
	protected virtual void SetAccountNotificationSettings(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetAccountNotificationSettings");
	protected virtual float GetVibrationMasterVolume() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetVibrationMasterVolume not implemented");
	protected virtual void SetVibrationMasterVolume(float _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetVibrationMasterVolume");
	protected virtual ulong GetSettingsItemValueSize(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetSettingsItemValueSize not implemented");
	protected virtual void GetSettingsItemValue(Span<byte> _0, Span<byte> _1, out ulong _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetSettingsItemValue not implemented");
	protected virtual void GetTvSettings(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetTvSettings not implemented");
	protected virtual void SetTvSettings(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetTvSettings");
	protected virtual void GetEdid(Span<byte> _0) =>
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
	protected virtual void GetDataDeletionSettings(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetDataDeletionSettings not implemented");
	protected virtual void SetDataDeletionSettings(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetDataDeletionSettings");
	protected virtual ulong GetInitialSystemAppletProgramId() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetInitialSystemAppletProgramId not implemented");
	protected virtual ulong GetOverlayDispProgramId() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetOverlayDispProgramId not implemented");
	protected virtual void GetDeviceTimeZoneLocationName(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetDeviceTimeZoneLocationName not implemented");
	protected virtual void SetDeviceTimeZoneLocationName(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetDeviceTimeZoneLocationName");
	protected virtual ulong GetWirelessCertificationFileSize() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetWirelessCertificationFileSize not implemented");
	protected virtual void GetWirelessCertificationFile(out ulong _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetWirelessCertificationFile not implemented");
	protected virtual void SetRegionCode(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetRegionCode");
	protected virtual void GetNetworkSystemClockContext(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetNetworkSystemClockContext not implemented");
	protected virtual void SetNetworkSystemClockContext(byte[] _0) =>
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
	protected virtual void GetBatteryLot(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetBatteryLot not implemented");
	protected virtual void GetSerialNumber(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetSerialNumber not implemented");
	protected virtual byte GetNfcEnableFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetNfcEnableFlag not implemented");
	protected virtual void SetNfcEnableFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetNfcEnableFlag");
	protected virtual void GetSleepSettings(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetSleepSettings not implemented");
	protected virtual void SetSleepSettings(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetSleepSettings");
	protected virtual byte GetWirelessLanEnableFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetWirelessLanEnableFlag not implemented");
	protected virtual void SetWirelessLanEnableFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetWirelessLanEnableFlag");
	protected virtual void GetInitialLaunchSettings(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetInitialLaunchSettings not implemented");
	protected virtual void SetInitialLaunchSettings(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetInitialLaunchSettings");
	protected virtual void GetDeviceNickName(Span<byte> _0) =>
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
	protected virtual void GetTelemetryDirtyFlags(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetTelemetryDirtyFlags not implemented");
	protected virtual void GetPtmBatteryLot(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetPtmBatteryLot not implemented");
	protected virtual void SetPtmBatteryLot(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetPtmBatteryLot");
	protected virtual void GetPtmFuelGaugeParameter(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetPtmFuelGaugeParameter not implemented");
	protected virtual void SetPtmFuelGaugeParameter(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetPtmFuelGaugeParameter");
	protected virtual byte GetBluetoothEnableFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetBluetoothEnableFlag not implemented");
	protected virtual void SetBluetoothEnableFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetBluetoothEnableFlag");
	protected virtual void GetMiiAuthorId(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetMiiAuthorId not implemented");
	protected virtual void SetShutdownRtcValue(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetShutdownRtcValue");
	protected virtual ulong GetShutdownRtcValue() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetShutdownRtcValue not implemented");
	protected virtual KObject AcquireFatalDirtyFlagEventHandle() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.AcquireFatalDirtyFlagEventHandle not implemented");
	protected virtual void GetFatalDirtyFlags(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetFatalDirtyFlags not implemented");
	protected virtual byte GetAutoUpdateEnableFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetAutoUpdateEnableFlag not implemented");
	protected virtual void SetAutoUpdateEnableFlag(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetAutoUpdateEnableFlag");
	protected virtual void GetNxControllerSettings(out uint _0, Span<byte> _1) =>
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
	protected virtual void GetBacklightSettingsEx(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetBacklightSettingsEx not implemented");
	protected virtual void SetBacklightSettingsEx(byte[] _0) =>
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
	protected virtual void NeedsToUpdateHeadphoneVolume(byte _0, out byte _1, out byte _2, out sbyte _3) =>
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
	protected virtual void GetConsoleSixAxisSensorAccelerationBias(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetConsoleSixAxisSensorAccelerationBias not implemented");
	protected virtual void SetConsoleSixAxisSensorAccelerationBias(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetConsoleSixAxisSensorAccelerationBias");
	protected virtual void GetConsoleSixAxisSensorAngularVelocityBias(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetConsoleSixAxisSensorAngularVelocityBias not implemented");
	protected virtual void SetConsoleSixAxisSensorAngularVelocityBias(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetConsoleSixAxisSensorAngularVelocityBias");
	protected virtual void GetConsoleSixAxisSensorAccelerationGain(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetConsoleSixAxisSensorAccelerationGain not implemented");
	protected virtual void SetConsoleSixAxisSensorAccelerationGain(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetConsoleSixAxisSensorAccelerationGain");
	protected virtual void GetConsoleSixAxisSensorAngularVelocityGain(out byte[] _0) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetConsoleSixAxisSensorAngularVelocityGain not implemented");
	protected virtual void SetConsoleSixAxisSensorAngularVelocityGain(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetConsoleSixAxisSensorAngularVelocityGain");
	protected virtual uint GetKeyboardLayout() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetKeyboardLayout not implemented");
	protected virtual void SetKeyboardLayout(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Settings.ISystemSettingsServer.SetKeyboardLayout");
	protected virtual byte GetWebInspectorFlag() =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetWebInspectorFlag not implemented");
	protected virtual void GetAllowedSslHosts(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Settings.ISystemSettingsServer.GetAllowedSslHosts not implemented");
	protected virtual void GetHostFsMountPoint(Span<byte> _0) =>
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // SetLanguageCode
				SetLanguageCode(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // SetNetworkSettings
				SetNetworkSettings(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GetNetworkSettings
				GetNetworkSettings(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x3: { // GetFirmwareVersion
				GetFirmwareVersion(im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // GetFirmwareVersion2
				GetFirmwareVersion2(im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // GetFirmwareVersionDigest
				GetFirmwareVersionDigest();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // GetLockScreenFlag
				var _return = GetLockScreenFlag();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x8: { // SetLockScreenFlag
				SetLockScreenFlag(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // GetBacklightSettings
				GetBacklightSettings(out var _0);
				om.Initialize(0, 0, 40);
				om.SetBytes(8, _0);
				break;
			}
			case 0xA: { // SetBacklightSettings
				SetBacklightSettings(im.GetBytes(8, 0x28));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // SetBluetoothDevicesSettings
				SetBluetoothDevicesSettings(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // GetBluetoothDevicesSettings
				GetBluetoothDevicesSettings(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0xD: { // GetExternalSteadyClockSourceId
				GetExternalSteadyClockSourceId(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0xE: { // SetExternalSteadyClockSourceId
				SetExternalSteadyClockSourceId(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xF: { // GetUserSystemClockContext
				GetUserSystemClockContext(out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0x10: { // SetUserSystemClockContext
				SetUserSystemClockContext(im.GetBytes(8, 0x20));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x11: { // GetAccountSettings
				var _return = GetAccountSettings();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x12: { // SetAccountSettings
				SetAccountSettings(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x13: { // GetAudioVolume
				GetAudioVolume(im.GetData<uint>(8), out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x14: { // SetAudioVolume
				SetAudioVolume(im.GetBytes(8, 0x8), im.GetData<uint>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x15: { // GetEulaVersions
				GetEulaVersions(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x16: { // SetEulaVersions
				SetEulaVersions(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x17: { // GetColorSetId
				var _return = GetColorSetId();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x18: { // SetColorSetId
				SetColorSetId(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19: { // GetConsoleInformationUploadFlag
				var _return = GetConsoleInformationUploadFlag();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x1A: { // SetConsoleInformationUploadFlag
				SetConsoleInformationUploadFlag(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1B: { // GetAutomaticApplicationDownloadFlag
				var _return = GetAutomaticApplicationDownloadFlag();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x1C: { // SetAutomaticApplicationDownloadFlag
				SetAutomaticApplicationDownloadFlag(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1D: { // GetNotificationSettings
				GetNotificationSettings(out var _0);
				om.Initialize(0, 0, 24);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1E: { // SetNotificationSettings
				SetNotificationSettings(im.GetBytes(8, 0x18));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1F: { // GetAccountNotificationSettings
				GetAccountNotificationSettings(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x20: { // SetAccountNotificationSettings
				SetAccountNotificationSettings(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x23: { // GetVibrationMasterVolume
				var _return = GetVibrationMasterVolume();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x24: { // SetVibrationMasterVolume
				SetVibrationMasterVolume(im.GetData<float>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x25: { // GetSettingsItemValueSize
				var _return = GetSettingsItemValueSize(im.GetSpan<byte>(0x19, 0), im.GetSpan<byte>(0x19, 1));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x26: { // GetSettingsItemValue
				GetSettingsItemValue(im.GetSpan<byte>(0x19, 0), im.GetSpan<byte>(0x19, 1), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				break;
			}
			case 0x27: { // GetTvSettings
				GetTvSettings(out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0x28: { // SetTvSettings
				SetTvSettings(im.GetBytes(8, 0x20));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x29: { // GetEdid
				GetEdid(im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2A: { // SetEdid
				SetEdid(im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2B: { // GetAudioOutputMode
				var _return = GetAudioOutputMode(im.GetData<uint>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x2C: { // SetAudioOutputMode
				SetAudioOutputMode(im.GetData<uint>(8), im.GetData<uint>(12));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2D: { // IsForceMuteOnHeadphoneRemoved
				var _return = IsForceMuteOnHeadphoneRemoved();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x2E: { // SetForceMuteOnHeadphoneRemoved
				SetForceMuteOnHeadphoneRemoved(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2F: { // GetQuestFlag
				var _return = GetQuestFlag();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x30: { // SetQuestFlag
				SetQuestFlag(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x31: { // GetDataDeletionSettings
				GetDataDeletionSettings(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x32: { // SetDataDeletionSettings
				SetDataDeletionSettings(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x33: { // GetInitialSystemAppletProgramId
				var _return = GetInitialSystemAppletProgramId();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x34: { // GetOverlayDispProgramId
				var _return = GetOverlayDispProgramId();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x35: { // GetDeviceTimeZoneLocationName
				GetDeviceTimeZoneLocationName(out var _0);
				om.Initialize(0, 0, 36);
				om.SetBytes(8, _0);
				break;
			}
			case 0x36: { // SetDeviceTimeZoneLocationName
				SetDeviceTimeZoneLocationName(im.GetBytes(8, 0x24));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x37: { // GetWirelessCertificationFileSize
				var _return = GetWirelessCertificationFileSize();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x38: { // GetWirelessCertificationFile
				GetWirelessCertificationFile(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				break;
			}
			case 0x39: { // SetRegionCode
				SetRegionCode(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3A: { // GetNetworkSystemClockContext
				GetNetworkSystemClockContext(out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3B: { // SetNetworkSystemClockContext
				SetNetworkSystemClockContext(im.GetBytes(8, 0x20));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3C: { // IsUserSystemClockAutomaticCorrectionEnabled
				var _return = IsUserSystemClockAutomaticCorrectionEnabled();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x3D: { // SetUserSystemClockAutomaticCorrectionEnabled
				SetUserSystemClockAutomaticCorrectionEnabled(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3E: { // GetDebugModeFlag
				var _return = GetDebugModeFlag();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x3F: { // GetPrimaryAlbumStorage
				var _return = GetPrimaryAlbumStorage();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x40: { // SetPrimaryAlbumStorage
				SetPrimaryAlbumStorage(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x41: { // GetUsb30EnableFlag
				var _return = GetUsb30EnableFlag();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x42: { // SetUsb30EnableFlag
				SetUsb30EnableFlag(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x43: { // GetBatteryLot
				GetBatteryLot(out var _0);
				om.Initialize(0, 0, 24);
				om.SetBytes(8, _0);
				break;
			}
			case 0x44: { // GetSerialNumber
				GetSerialNumber(out var _0);
				om.Initialize(0, 0, 24);
				om.SetBytes(8, _0);
				break;
			}
			case 0x45: { // GetNfcEnableFlag
				var _return = GetNfcEnableFlag();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x46: { // SetNfcEnableFlag
				SetNfcEnableFlag(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x47: { // GetSleepSettings
				GetSleepSettings(out var _0);
				om.Initialize(0, 0, 12);
				om.SetBytes(8, _0);
				break;
			}
			case 0x48: { // SetSleepSettings
				SetSleepSettings(im.GetBytes(8, 0xC));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x49: { // GetWirelessLanEnableFlag
				var _return = GetWirelessLanEnableFlag();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x4A: { // SetWirelessLanEnableFlag
				SetWirelessLanEnableFlag(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4B: { // GetInitialLaunchSettings
				GetInitialLaunchSettings(out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4C: { // SetInitialLaunchSettings
				SetInitialLaunchSettings(im.GetBytes(8, 0x20));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4D: { // GetDeviceNickName
				GetDeviceNickName(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4E: { // SetDeviceNickName
				SetDeviceNickName(im.GetSpan<byte>(0x15, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4F: { // GetProductModel
				var _return = GetProductModel();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x50: { // GetLdnChannel
				var _return = GetLdnChannel();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x51: { // SetLdnChannel
				SetLdnChannel(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x52: { // AcquireTelemetryDirtyFlagEventHandle
				var _return = AcquireTelemetryDirtyFlagEventHandle();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x53: { // GetTelemetryDirtyFlags
				GetTelemetryDirtyFlags(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x54: { // GetPtmBatteryLot
				GetPtmBatteryLot(out var _0);
				om.Initialize(0, 0, 24);
				om.SetBytes(8, _0);
				break;
			}
			case 0x55: { // SetPtmBatteryLot
				SetPtmBatteryLot(im.GetBytes(8, 0x18));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x56: { // GetPtmFuelGaugeParameter
				GetPtmFuelGaugeParameter(out var _0);
				om.Initialize(0, 0, 24);
				om.SetBytes(8, _0);
				break;
			}
			case 0x57: { // SetPtmFuelGaugeParameter
				SetPtmFuelGaugeParameter(im.GetBytes(8, 0x18));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x58: { // GetBluetoothEnableFlag
				var _return = GetBluetoothEnableFlag();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x59: { // SetBluetoothEnableFlag
				SetBluetoothEnableFlag(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5A: { // GetMiiAuthorId
				GetMiiAuthorId(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5B: { // SetShutdownRtcValue
				SetShutdownRtcValue(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5C: { // GetShutdownRtcValue
				var _return = GetShutdownRtcValue();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x5D: { // AcquireFatalDirtyFlagEventHandle
				var _return = AcquireFatalDirtyFlagEventHandle();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x5E: { // GetFatalDirtyFlags
				GetFatalDirtyFlags(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5F: { // GetAutoUpdateEnableFlag
				var _return = GetAutoUpdateEnableFlag();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x60: { // SetAutoUpdateEnableFlag
				SetAutoUpdateEnableFlag(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x61: { // GetNxControllerSettings
				GetNxControllerSettings(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x62: { // SetNxControllerSettings
				SetNxControllerSettings(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x63: { // GetBatteryPercentageFlag
				var _return = GetBatteryPercentageFlag();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x64: { // SetBatteryPercentageFlag
				SetBatteryPercentageFlag(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x65: { // GetExternalRtcResetFlag
				var _return = GetExternalRtcResetFlag();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x66: { // SetExternalRtcResetFlag
				SetExternalRtcResetFlag(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x67: { // GetUsbFullKeyEnableFlag
				var _return = GetUsbFullKeyEnableFlag();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x68: { // SetUsbFullKeyEnableFlag
				SetUsbFullKeyEnableFlag(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x69: { // SetExternalSteadyClockInternalOffset
				SetExternalSteadyClockInternalOffset(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6A: { // GetExternalSteadyClockInternalOffset
				var _return = GetExternalSteadyClockInternalOffset();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x6B: { // GetBacklightSettingsEx
				GetBacklightSettingsEx(out var _0);
				om.Initialize(0, 0, 44);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6C: { // SetBacklightSettingsEx
				SetBacklightSettingsEx(im.GetBytes(8, 0x2C));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6D: { // GetHeadphoneVolumeWarningCount
				var _return = GetHeadphoneVolumeWarningCount();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x6E: { // SetHeadphoneVolumeWarningCount
				SetHeadphoneVolumeWarningCount(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6F: { // GetBluetoothAfhEnableFlag
				var _return = GetBluetoothAfhEnableFlag();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x70: { // SetBluetoothAfhEnableFlag
				SetBluetoothAfhEnableFlag(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x71: { // GetBluetoothBoostEnableFlag
				var _return = GetBluetoothBoostEnableFlag();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x72: { // SetBluetoothBoostEnableFlag
				SetBluetoothBoostEnableFlag(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x73: { // GetInRepairProcessEnableFlag
				var _return = GetInRepairProcessEnableFlag();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x74: { // SetInRepairProcessEnableFlag
				SetInRepairProcessEnableFlag(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x75: { // GetHeadphoneVolumeUpdateFlag
				var _return = GetHeadphoneVolumeUpdateFlag();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x76: { // SetHeadphoneVolumeUpdateFlag
				SetHeadphoneVolumeUpdateFlag(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x77: { // NeedsToUpdateHeadphoneVolume
				NeedsToUpdateHeadphoneVolume(im.GetData<byte>(8), out var _0, out var _1, out var _2);
				om.Initialize(0, 0, 3);
				om.SetData(8, _0);
				om.SetData(9, _1);
				om.SetData(10, _2);
				break;
			}
			case 0x78: { // GetPushNotificationActivityModeOnSleep
				var _return = GetPushNotificationActivityModeOnSleep();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x79: { // SetPushNotificationActivityModeOnSleep
				SetPushNotificationActivityModeOnSleep(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7A: { // GetServiceDiscoveryControlSettings
				var _return = GetServiceDiscoveryControlSettings();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x7B: { // SetServiceDiscoveryControlSettings
				SetServiceDiscoveryControlSettings(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7C: { // GetErrorReportSharePermission
				var _return = GetErrorReportSharePermission();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x7D: { // SetErrorReportSharePermission
				SetErrorReportSharePermission(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7E: { // GetAppletLaunchFlags
				var _return = GetAppletLaunchFlags();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x7F: { // SetAppletLaunchFlags
				SetAppletLaunchFlags(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x80: { // GetConsoleSixAxisSensorAccelerationBias
				GetConsoleSixAxisSensorAccelerationBias(out var _0);
				om.Initialize(0, 0, 12);
				om.SetBytes(8, _0);
				break;
			}
			case 0x81: { // SetConsoleSixAxisSensorAccelerationBias
				SetConsoleSixAxisSensorAccelerationBias(im.GetBytes(8, 0xC));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x82: { // GetConsoleSixAxisSensorAngularVelocityBias
				GetConsoleSixAxisSensorAngularVelocityBias(out var _0);
				om.Initialize(0, 0, 12);
				om.SetBytes(8, _0);
				break;
			}
			case 0x83: { // SetConsoleSixAxisSensorAngularVelocityBias
				SetConsoleSixAxisSensorAngularVelocityBias(im.GetBytes(8, 0xC));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x84: { // GetConsoleSixAxisSensorAccelerationGain
				GetConsoleSixAxisSensorAccelerationGain(out var _0);
				om.Initialize(0, 0, 36);
				om.SetBytes(8, _0);
				break;
			}
			case 0x85: { // SetConsoleSixAxisSensorAccelerationGain
				SetConsoleSixAxisSensorAccelerationGain(im.GetBytes(8, 0x24));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x86: { // GetConsoleSixAxisSensorAngularVelocityGain
				GetConsoleSixAxisSensorAngularVelocityGain(out var _0);
				om.Initialize(0, 0, 36);
				om.SetBytes(8, _0);
				break;
			}
			case 0x87: { // SetConsoleSixAxisSensorAngularVelocityGain
				SetConsoleSixAxisSensorAngularVelocityGain(im.GetBytes(8, 0x24));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x88: { // GetKeyboardLayout
				var _return = GetKeyboardLayout();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x89: { // SetKeyboardLayout
				SetKeyboardLayout(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8A: { // GetWebInspectorFlag
				var _return = GetWebInspectorFlag();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x8B: { // GetAllowedSslHosts
				GetAllowedSslHosts(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x8C: { // GetHostFsMountPoint
				GetHostFsMountPoint(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8D: { // GetRequiresRunRepairTimeReviser
				GetRequiresRunRepairTimeReviser();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8E: { // SetRequiresRunRepairTimeReviser
				SetRequiresRunRepairTimeReviser();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8F: { // SetBlePairingSettings
				SetBlePairingSettings();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x90: { // GetBlePairingSettings
				GetBlePairingSettings();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x91: { // GetConsoleSixAxisSensorAngularVelocityTimeBias
				GetConsoleSixAxisSensorAngularVelocityTimeBias();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x92: { // SetConsoleSixAxisSensorAngularVelocityTimeBias
				SetConsoleSixAxisSensorAngularVelocityTimeBias();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x93: { // GetConsoleSixAxisSensorAngularAcceleration
				GetConsoleSixAxisSensorAngularAcceleration();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x94: { // SetConsoleSixAxisSensorAngularAcceleration
				SetConsoleSixAxisSensorAngularAcceleration();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x95: { // GetRebootlessSystemUpdateVersion
				GetRebootlessSystemUpdateVersion();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Settings.ISystemSettingsServer");
		}
	}
}

