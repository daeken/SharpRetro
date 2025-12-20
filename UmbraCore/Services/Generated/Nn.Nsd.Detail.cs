using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nsd.Detail;
public partial class IManager : _IManager_Base;
public abstract class _IManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0xA: // GetSettingName
				break;
			case 0xB: // GetEnvironmentIdentifier
				break;
			case 0xC: // GetDeviceId
				break;
			case 0xD: // DeleteSettings
				break;
			case 0xE: // ImportSettings
				break;
			case 0x14: // Resolve
				break;
			case 0x15: // ResolveEx
				break;
			case 0x1E: // GetNasServiceSetting
				break;
			case 0x1F: // GetNasServiceSettingEx
				break;
			case 0x28: // GetNasRequestFqdn
				break;
			case 0x29: // GetNasRequestFqdnEx
				break;
			case 0x2A: // GetNasApiFqdn
				break;
			case 0x2B: // GetNasApiFqdnEx
				break;
			case 0x32: // GetCurrentSetting
				break;
			case 0x3C: // ReadSaveDataFromFsForTest
				break;
			case 0x3D: // WriteSaveDataToFsForTest
				break;
			case 0x3E: // DeleteSaveDataOfFsForTest
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nsd.Detail.IManager");
		}
	}
}

