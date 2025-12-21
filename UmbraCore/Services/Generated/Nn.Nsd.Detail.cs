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
	protected virtual void GetDeviceId(out byte[] _0) =>
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0xA: { // GetSettingName
				om.Initialize(0, 0, 0);
				GetSettingName(im.GetSpan<byte>(0x16, 0));
				break;
			}
			case 0xB: { // GetEnvironmentIdentifier
				om.Initialize(0, 0, 0);
				GetEnvironmentIdentifier(im.GetSpan<byte>(0x16, 0));
				break;
			}
			case 0xC: { // GetDeviceId
				om.Initialize(0, 0, 16);
				GetDeviceId(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD: { // DeleteSettings
				om.Initialize(0, 0, 0);
				DeleteSettings(im.GetData<uint>(8));
				break;
			}
			case 0xE: { // ImportSettings
				om.Initialize(0, 0, 0);
				ImportSettings(im.GetData<uint>(8), im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x14: { // Resolve
				om.Initialize(0, 0, 0);
				Resolve(im.GetSpan<byte>(0x15, 0), im.GetSpan<byte>(0x16, 0));
				break;
			}
			case 0x15: { // ResolveEx
				om.Initialize(0, 0, 4);
				ResolveEx(im.GetSpan<byte>(0x15, 0), out var _0, im.GetSpan<byte>(0x16, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x1E: { // GetNasServiceSetting
				om.Initialize(0, 0, 0);
				GetNasServiceSetting(im.GetSpan<byte>(0x15, 0), im.GetSpan<byte>(0x16, 0));
				break;
			}
			case 0x1F: { // GetNasServiceSettingEx
				om.Initialize(0, 0, 4);
				GetNasServiceSettingEx(im.GetSpan<byte>(0x15, 0), out var _0, im.GetSpan<byte>(0x16, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x28: { // GetNasRequestFqdn
				om.Initialize(0, 0, 0);
				GetNasRequestFqdn(im.GetSpan<byte>(0x16, 0));
				break;
			}
			case 0x29: { // GetNasRequestFqdnEx
				om.Initialize(0, 0, 4);
				GetNasRequestFqdnEx(out var _0, im.GetSpan<byte>(0x16, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x2A: { // GetNasApiFqdn
				om.Initialize(0, 0, 0);
				GetNasApiFqdn(im.GetSpan<byte>(0x16, 0));
				break;
			}
			case 0x2B: { // GetNasApiFqdnEx
				om.Initialize(0, 0, 4);
				GetNasApiFqdnEx(out var _0, im.GetSpan<byte>(0x16, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x32: { // GetCurrentSetting
				om.Initialize(0, 0, 0);
				GetCurrentSetting(im.GetSpan<byte>(0x16, 0));
				break;
			}
			case 0x3C: { // ReadSaveDataFromFsForTest
				om.Initialize(0, 0, 0);
				ReadSaveDataFromFsForTest(im.GetSpan<byte>(0x16, 0));
				break;
			}
			case 0x3D: { // WriteSaveDataToFsForTest
				om.Initialize(0, 0, 0);
				WriteSaveDataToFsForTest(im.GetSpan<byte>(0x15, 0));
				break;
			}
			case 0x3E: { // DeleteSaveDataOfFsForTest
				om.Initialize(0, 0, 0);
				DeleteSaveDataOfFsForTest();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nsd.Detail.IManager");
		}
	}
}

