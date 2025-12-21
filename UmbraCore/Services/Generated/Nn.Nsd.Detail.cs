using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nsd.Detail;
public partial class IManager : _IManager_Base;
public abstract class _IManager_Base : IpcInterface {
	protected virtual void GetSettingName(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nsd.Detail.IManager.GetSettingName not implemented");
	protected virtual void GetEnvironmentIdentifier(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nsd.Detail.IManager.GetEnvironmentIdentifier not implemented");
	protected virtual void GetDeviceId(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nsd.Detail.IManager.GetDeviceId not implemented");
	protected virtual void DeleteSettings(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Nsd.Detail.IManager.DeleteSettings");
	protected virtual void ImportSettings(uint _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Nsd.Detail.IManager.ImportSettings not implemented");
	protected virtual void Resolve(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nsd.Detail.IManager.Resolve not implemented");
	protected virtual void ResolveEx(Span<byte> _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Nsd.Detail.IManager.ResolveEx not implemented");
	protected virtual void GetNasServiceSetting(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nsd.Detail.IManager.GetNasServiceSetting not implemented");
	protected virtual void GetNasServiceSettingEx(Span<byte> _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Nsd.Detail.IManager.GetNasServiceSettingEx not implemented");
	protected virtual void GetNasRequestFqdn(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nsd.Detail.IManager.GetNasRequestFqdn not implemented");
	protected virtual void GetNasRequestFqdnEx(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nsd.Detail.IManager.GetNasRequestFqdnEx not implemented");
	protected virtual void GetNasApiFqdn(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nsd.Detail.IManager.GetNasApiFqdn not implemented");
	protected virtual void GetNasApiFqdnEx(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nsd.Detail.IManager.GetNasApiFqdnEx not implemented");
	protected virtual void GetCurrentSetting(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nsd.Detail.IManager.GetCurrentSetting not implemented");
	protected virtual void ReadSaveDataFromFsForTest(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nsd.Detail.IManager.ReadSaveDataFromFsForTest not implemented");
	protected virtual void WriteSaveDataToFsForTest(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nsd.Detail.IManager.WriteSaveDataToFsForTest");
	protected virtual void DeleteSaveDataOfFsForTest() =>
		Console.WriteLine("Stub hit for Nn.Nsd.Detail.IManager.DeleteSaveDataOfFsForTest");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0xA: { // GetSettingName
				break;
			}
			case 0xB: { // GetEnvironmentIdentifier
				break;
			}
			case 0xC: { // GetDeviceId
				break;
			}
			case 0xD: { // DeleteSettings
				break;
			}
			case 0xE: { // ImportSettings
				break;
			}
			case 0x14: { // Resolve
				break;
			}
			case 0x15: { // ResolveEx
				break;
			}
			case 0x1E: { // GetNasServiceSetting
				break;
			}
			case 0x1F: { // GetNasServiceSettingEx
				break;
			}
			case 0x28: { // GetNasRequestFqdn
				break;
			}
			case 0x29: { // GetNasRequestFqdnEx
				break;
			}
			case 0x2A: { // GetNasApiFqdn
				break;
			}
			case 0x2B: { // GetNasApiFqdnEx
				break;
			}
			case 0x32: { // GetCurrentSetting
				break;
			}
			case 0x3C: { // ReadSaveDataFromFsForTest
				break;
			}
			case 0x3D: { // WriteSaveDataToFsForTest
				break;
			}
			case 0x3E: { // DeleteSaveDataOfFsForTest
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nsd.Detail.IManager");
		}
	}
}

