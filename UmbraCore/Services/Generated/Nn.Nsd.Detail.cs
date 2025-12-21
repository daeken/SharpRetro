using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nsd.Detail;
public partial class IManager : _IManager_Base {
	public readonly string ServiceName;
	public IManager(string serviceName) => ServiceName = serviceName;
}
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
				GetSettingName(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // GetEnvironmentIdentifier
				GetEnvironmentIdentifier(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // GetDeviceId
				GetDeviceId(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD: { // DeleteSettings
				DeleteSettings(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // ImportSettings
				ImportSettings(im.GetData<uint>(8), im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x14: { // Resolve
				Resolve(im.GetSpan<byte>(0x15, 0), im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x15: { // ResolveEx
				ResolveEx(im.GetSpan<byte>(0x15, 0), out var _0, im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x1E: { // GetNasServiceSetting
				GetNasServiceSetting(im.GetSpan<byte>(0x15, 0), im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1F: { // GetNasServiceSettingEx
				GetNasServiceSettingEx(im.GetSpan<byte>(0x15, 0), out var _0, im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x28: { // GetNasRequestFqdn
				GetNasRequestFqdn(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x29: { // GetNasRequestFqdnEx
				GetNasRequestFqdnEx(out var _0, im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x2A: { // GetNasApiFqdn
				GetNasApiFqdn(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2B: { // GetNasApiFqdnEx
				GetNasApiFqdnEx(out var _0, im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x32: { // GetCurrentSetting
				GetCurrentSetting(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3C: { // ReadSaveDataFromFsForTest
				ReadSaveDataFromFsForTest(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3D: { // WriteSaveDataToFsForTest
				WriteSaveDataToFsForTest(im.GetSpan<byte>(0x15, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3E: { // DeleteSaveDataOfFsForTest
				DeleteSaveDataOfFsForTest();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nsd.Detail.IManager");
		}
	}
}

