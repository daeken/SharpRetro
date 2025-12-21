using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nifm.Detail;
public partial class IGeneralService : _IGeneralService_Base;
public abstract class _IGeneralService_Base : IpcInterface {
	protected virtual void GetClientId(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetClientId not implemented");
	protected virtual Nn.Nifm.Detail.IScanRequest CreateScanRequest() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.CreateScanRequest not implemented");
	protected virtual Nn.Nifm.Detail.IRequest CreateRequest(uint _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.CreateRequest not implemented");
	protected virtual void GetCurrentNetworkProfile(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetCurrentNetworkProfile not implemented");
	protected virtual void EnumerateNetworkInterfaces(uint _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.EnumerateNetworkInterfaces not implemented");
	protected virtual void EnumerateNetworkProfiles(byte _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.EnumerateNetworkProfiles not implemented");
	protected virtual void GetNetworkProfile(byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetNetworkProfile not implemented");
	protected virtual void SetNetworkProfile(Span<byte> _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.SetNetworkProfile not implemented");
	protected virtual void RemoveNetworkProfile(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IGeneralService.RemoveNetworkProfile");
	protected virtual void GetScanDataOld(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetScanDataOld not implemented");
	protected virtual void GetCurrentIpAddress(out byte[] _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetCurrentIpAddress not implemented");
	protected virtual void GetCurrentAccessPointOld(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetCurrentAccessPointOld not implemented");
	protected virtual void CreateTemporaryNetworkProfile(Span<byte> _0, out byte[] _1, out Nn.Nifm.Detail.INetworkProfile _2) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.CreateTemporaryNetworkProfile not implemented");
	protected virtual void GetCurrentIpConfigInfo(out byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetCurrentIpConfigInfo not implemented");
	protected virtual void SetWirelessCommunicationEnabled(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IGeneralService.SetWirelessCommunicationEnabled");
	protected virtual byte IsWirelessCommunicationEnabled() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.IsWirelessCommunicationEnabled not implemented");
	protected virtual void GetInternetConnectionStatus(out byte[] _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetInternetConnectionStatus not implemented");
	protected virtual void SetEthernetCommunicationEnabled(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IGeneralService.SetEthernetCommunicationEnabled");
	protected virtual byte IsEthernetCommunicationEnabled() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.IsEthernetCommunicationEnabled not implemented");
	protected virtual byte IsAnyInternetRequestAccepted(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.IsAnyInternetRequestAccepted not implemented");
	protected virtual byte IsAnyForegroundRequestAccepted() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.IsAnyForegroundRequestAccepted not implemented");
	protected virtual void PutToSleep() =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IGeneralService.PutToSleep");
	protected virtual void WakeUp() =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IGeneralService.WakeUp");
	protected virtual void GetSsidListVersion(out byte[] _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetSsidListVersion not implemented");
	protected virtual void SetExclusiveClient(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IGeneralService.SetExclusiveClient");
	protected virtual void GetDefaultIpSetting(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetDefaultIpSetting not implemented");
	protected virtual void SetDefaultIpSetting(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IGeneralService.SetDefaultIpSetting");
	protected virtual void SetWirelessCommunicationEnabledForTest(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IGeneralService.SetWirelessCommunicationEnabledForTest");
	protected virtual void SetEthernetCommunicationEnabledForTest(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IGeneralService.SetEthernetCommunicationEnabledForTest");
	protected virtual KObject GetTelemetorySystemEventReadableHandle() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetTelemetorySystemEventReadableHandle not implemented");
	protected virtual void GetTelemetryInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetTelemetryInfo not implemented");
	protected virtual void ConfirmSystemAvailability() =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IGeneralService.ConfirmSystemAvailability");
	protected virtual void SetBackgroundRequestEnabled(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IGeneralService.SetBackgroundRequestEnabled");
	protected virtual void GetScanData(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetScanData not implemented");
	protected virtual void GetCurrentAccessPoint(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetCurrentAccessPoint not implemented");
	protected virtual void Shutdown() =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IGeneralService.Shutdown");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: { // GetClientId
				om.Initialize(0, 0, 0);
				GetClientId(im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x2: { // CreateScanRequest
				om.Initialize(1, 0, 0);
				var _return = CreateScanRequest();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x4: { // CreateRequest
				om.Initialize(1, 0, 0);
				var _return = CreateRequest(im.GetData<uint>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x5: { // GetCurrentNetworkProfile
				om.Initialize(0, 0, 0);
				GetCurrentNetworkProfile(im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x6: { // EnumerateNetworkInterfaces
				om.Initialize(0, 0, 4);
				EnumerateNetworkInterfaces(im.GetData<uint>(8), out var _0, im.GetSpan<byte>(0xA, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x7: { // EnumerateNetworkProfiles
				om.Initialize(0, 0, 4);
				EnumerateNetworkProfiles(im.GetData<byte>(8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x8: { // GetNetworkProfile
				om.Initialize(0, 0, 0);
				GetNetworkProfile(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x9: { // SetNetworkProfile
				om.Initialize(0, 0, 16);
				SetNetworkProfile(im.GetSpan<byte>(0x19, 0), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xA: { // RemoveNetworkProfile
				om.Initialize(0, 0, 0);
				RemoveNetworkProfile(im.GetBytes(8, 0x10));
				break;
			}
			case 0xB: { // GetScanDataOld
				om.Initialize(0, 0, 4);
				GetScanDataOld(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0xC: { // GetCurrentIpAddress
				om.Initialize(0, 0, 4);
				GetCurrentIpAddress(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD: { // GetCurrentAccessPointOld
				om.Initialize(0, 0, 0);
				GetCurrentAccessPointOld(im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0xE: { // CreateTemporaryNetworkProfile
				om.Initialize(1, 0, 16);
				CreateTemporaryNetworkProfile(im.GetSpan<byte>(0x19, 0), out var _0, out var _1);
				om.SetBytes(8, _0);
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0xF: { // GetCurrentIpConfigInfo
				om.Initialize(0, 0, 22);
				GetCurrentIpConfigInfo(out var _0, out var _1);
				om.SetBytes(8, _0);
				om.SetBytes(21, _1);
				break;
			}
			case 0x10: { // SetWirelessCommunicationEnabled
				om.Initialize(0, 0, 0);
				SetWirelessCommunicationEnabled(im.GetData<byte>(8));
				break;
			}
			case 0x11: { // IsWirelessCommunicationEnabled
				om.Initialize(0, 0, 1);
				var _return = IsWirelessCommunicationEnabled();
				om.SetData(8, _return);
				break;
			}
			case 0x12: { // GetInternetConnectionStatus
				om.Initialize(0, 0, 3);
				GetInternetConnectionStatus(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x13: { // SetEthernetCommunicationEnabled
				om.Initialize(0, 0, 0);
				SetEthernetCommunicationEnabled(im.GetData<byte>(8));
				break;
			}
			case 0x14: { // IsEthernetCommunicationEnabled
				om.Initialize(0, 0, 1);
				var _return = IsEthernetCommunicationEnabled();
				om.SetData(8, _return);
				break;
			}
			case 0x15: { // IsAnyInternetRequestAccepted
				om.Initialize(0, 0, 1);
				var _return = IsAnyInternetRequestAccepted(im.GetSpan<byte>(0x19, 0));
				om.SetData(8, _return);
				break;
			}
			case 0x16: { // IsAnyForegroundRequestAccepted
				om.Initialize(0, 0, 1);
				var _return = IsAnyForegroundRequestAccepted();
				om.SetData(8, _return);
				break;
			}
			case 0x17: { // PutToSleep
				om.Initialize(0, 0, 0);
				PutToSleep();
				break;
			}
			case 0x18: { // WakeUp
				om.Initialize(0, 0, 0);
				WakeUp();
				break;
			}
			case 0x19: { // GetSsidListVersion
				om.Initialize(0, 0, 16);
				GetSsidListVersion(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1A: { // SetExclusiveClient
				om.Initialize(0, 0, 0);
				SetExclusiveClient(im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x1B: { // GetDefaultIpSetting
				om.Initialize(0, 0, 0);
				GetDefaultIpSetting(im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x1C: { // SetDefaultIpSetting
				om.Initialize(0, 0, 0);
				SetDefaultIpSetting(im.GetSpan<byte>(0x19, 0));
				break;
			}
			case 0x1D: { // SetWirelessCommunicationEnabledForTest
				om.Initialize(0, 0, 0);
				SetWirelessCommunicationEnabledForTest(im.GetData<byte>(8));
				break;
			}
			case 0x1E: { // SetEthernetCommunicationEnabledForTest
				om.Initialize(0, 0, 0);
				SetEthernetCommunicationEnabledForTest(im.GetData<byte>(8));
				break;
			}
			case 0x1F: { // GetTelemetorySystemEventReadableHandle
				om.Initialize(0, 1, 0);
				var _return = GetTelemetorySystemEventReadableHandle();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x20: { // GetTelemetryInfo
				om.Initialize(0, 0, 0);
				GetTelemetryInfo(im.GetSpan<byte>(0x16, 0));
				break;
			}
			case 0x21: { // ConfirmSystemAvailability
				om.Initialize(0, 0, 0);
				ConfirmSystemAvailability();
				break;
			}
			case 0x22: { // SetBackgroundRequestEnabled
				om.Initialize(0, 0, 0);
				SetBackgroundRequestEnabled(im.GetData<byte>(8));
				break;
			}
			case 0x23: { // GetScanData
				om.Initialize(0, 0, 4);
				GetScanData(out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x24: { // GetCurrentAccessPoint
				om.Initialize(0, 0, 0);
				GetCurrentAccessPoint(im.GetSpan<byte>(0x1A, 0));
				break;
			}
			case 0x25: { // Shutdown
				om.Initialize(0, 0, 0);
				Shutdown();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nifm.Detail.IGeneralService");
		}
	}
}

public partial class INetworkProfile : _INetworkProfile_Base;
public abstract class _INetworkProfile_Base : IpcInterface {
	protected virtual void Update(Span<byte> _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Nifm.Detail.INetworkProfile.Update not implemented");
	protected virtual void PersistOld(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Nifm.Detail.INetworkProfile.PersistOld not implemented");
	protected virtual void Persist(out byte[] _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.INetworkProfile.Persist not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Update
				om.Initialize(0, 0, 16);
				Update(im.GetSpan<byte>(0x19, 0), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // PersistOld
				om.Initialize(0, 0, 16);
				PersistOld(im.GetBytes(8, 0x10), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // Persist
				om.Initialize(0, 0, 16);
				Persist(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nifm.Detail.INetworkProfile");
		}
	}
}

public partial class IRequest : _IRequest_Base;
public abstract class _IRequest_Base : IpcInterface {
	protected virtual uint GetRequestState() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IRequest.GetRequestState not implemented");
	protected virtual void GetResult() =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.GetResult");
	protected virtual void GetSystemEventReadableHandles(out KObject _0, out KObject _1) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IRequest.GetSystemEventReadableHandles not implemented");
	protected virtual void Cancel() =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.Cancel");
	protected virtual void Submit() =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.Submit");
	protected virtual void SetRequirement(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.SetRequirement");
	protected virtual void SetRequirementPreset(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.SetRequirementPreset");
	protected virtual void SetPriority(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.SetPriority");
	protected virtual void SetNetworkProfileId(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.SetNetworkProfileId");
	protected virtual void SetRejectable(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.SetRejectable");
	protected virtual void SetConnectionConfirmationOption(sbyte _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.SetConnectionConfirmationOption");
	protected virtual void SetPersistent(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.SetPersistent");
	protected virtual void SetInstant(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.SetInstant");
	protected virtual void SetSustainable(byte _0, byte _1) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.SetSustainable");
	protected virtual void SetRawPriority(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.SetRawPriority");
	protected virtual void SetGreedy(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.SetGreedy");
	protected virtual void SetSharable(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.SetSharable");
	protected virtual void SetRequirementByRevision(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.SetRequirementByRevision");
	protected virtual void GetRequirement(out byte[] _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IRequest.GetRequirement not implemented");
	protected virtual uint GetRevision() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IRequest.GetRevision not implemented");
	protected virtual void GetAppletInfo(uint _0, out uint _1, out uint _2, out uint _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IRequest.GetAppletInfo not implemented");
	protected virtual void GetAdditionalInfo(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IRequest.GetAdditionalInfo not implemented");
	protected virtual void SetKeptInSleep(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.SetKeptInSleep");
	protected virtual void RegisterSocketDescriptor(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.RegisterSocketDescriptor");
	protected virtual void UnregisterSocketDescriptor(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.UnregisterSocketDescriptor");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetRequestState
				om.Initialize(0, 0, 4);
				var _return = GetRequestState();
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // GetResult
				om.Initialize(0, 0, 0);
				GetResult();
				break;
			}
			case 0x2: { // GetSystemEventReadableHandles
				om.Initialize(0, 2, 0);
				GetSystemEventReadableHandles(out var _0, out var _1);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Copy(1, CreateHandle(_1, copy: true));
				break;
			}
			case 0x3: { // Cancel
				om.Initialize(0, 0, 0);
				Cancel();
				break;
			}
			case 0x4: { // Submit
				om.Initialize(0, 0, 0);
				Submit();
				break;
			}
			case 0x5: { // SetRequirement
				om.Initialize(0, 0, 0);
				SetRequirement(im.GetBytes(8, 0x24));
				break;
			}
			case 0x6: { // SetRequirementPreset
				om.Initialize(0, 0, 0);
				SetRequirementPreset(im.GetData<uint>(8));
				break;
			}
			case 0x8: { // SetPriority
				om.Initialize(0, 0, 0);
				SetPriority(im.GetData<byte>(8));
				break;
			}
			case 0x9: { // SetNetworkProfileId
				om.Initialize(0, 0, 0);
				SetNetworkProfileId(im.GetBytes(8, 0x10));
				break;
			}
			case 0xA: { // SetRejectable
				om.Initialize(0, 0, 0);
				SetRejectable(im.GetData<byte>(8));
				break;
			}
			case 0xB: { // SetConnectionConfirmationOption
				om.Initialize(0, 0, 0);
				SetConnectionConfirmationOption(im.GetData<sbyte>(8));
				break;
			}
			case 0xC: { // SetPersistent
				om.Initialize(0, 0, 0);
				SetPersistent(im.GetData<byte>(8));
				break;
			}
			case 0xD: { // SetInstant
				om.Initialize(0, 0, 0);
				SetInstant(im.GetData<byte>(8));
				break;
			}
			case 0xE: { // SetSustainable
				om.Initialize(0, 0, 0);
				SetSustainable(im.GetData<byte>(8), im.GetData<byte>(9));
				break;
			}
			case 0xF: { // SetRawPriority
				om.Initialize(0, 0, 0);
				SetRawPriority(im.GetData<byte>(8));
				break;
			}
			case 0x10: { // SetGreedy
				om.Initialize(0, 0, 0);
				SetGreedy(im.GetData<byte>(8));
				break;
			}
			case 0x11: { // SetSharable
				om.Initialize(0, 0, 0);
				SetSharable(im.GetData<byte>(8));
				break;
			}
			case 0x12: { // SetRequirementByRevision
				om.Initialize(0, 0, 0);
				SetRequirementByRevision(im.GetData<uint>(8));
				break;
			}
			case 0x13: { // GetRequirement
				om.Initialize(0, 0, 36);
				GetRequirement(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x14: { // GetRevision
				om.Initialize(0, 0, 4);
				var _return = GetRevision();
				om.SetData(8, _return);
				break;
			}
			case 0x15: { // GetAppletInfo
				om.Initialize(0, 0, 12);
				GetAppletInfo(im.GetData<uint>(8), out var _0, out var _1, out var _2, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				break;
			}
			case 0x16: { // GetAdditionalInfo
				om.Initialize(0, 0, 4);
				GetAdditionalInfo(out var _0, im.GetSpan<byte>(0x16, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x17: { // SetKeptInSleep
				om.Initialize(0, 0, 0);
				SetKeptInSleep(im.GetData<byte>(8));
				break;
			}
			case 0x18: { // RegisterSocketDescriptor
				om.Initialize(0, 0, 0);
				RegisterSocketDescriptor(im.GetData<uint>(8));
				break;
			}
			case 0x19: { // UnregisterSocketDescriptor
				om.Initialize(0, 0, 0);
				UnregisterSocketDescriptor(im.GetData<uint>(8));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nifm.Detail.IRequest");
		}
	}
}

public partial class IScanRequest : _IScanRequest_Base;
public abstract class _IScanRequest_Base : IpcInterface {
	protected virtual void Submit() =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IScanRequest.Submit");
	protected virtual byte IsProcessing() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IScanRequest.IsProcessing not implemented");
	protected virtual void GetResult() =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IScanRequest.GetResult");
	protected virtual KObject GetSystemEventReadableHandle() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IScanRequest.GetSystemEventReadableHandle not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Submit
				om.Initialize(0, 0, 0);
				Submit();
				break;
			}
			case 0x1: { // IsProcessing
				om.Initialize(0, 0, 1);
				var _return = IsProcessing();
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // GetResult
				om.Initialize(0, 0, 0);
				GetResult();
				break;
			}
			case 0x3: { // GetSystemEventReadableHandle
				om.Initialize(0, 1, 0);
				var _return = GetSystemEventReadableHandle();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nifm.Detail.IScanRequest");
		}
	}
}

public partial class IStaticService : _IStaticService_Base;
public abstract class _IStaticService_Base : IpcInterface {
	protected virtual Nn.Nifm.Detail.IGeneralService CreateGeneralServiceOld() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IStaticService.CreateGeneralServiceOld not implemented");
	protected virtual Nn.Nifm.Detail.IGeneralService CreateGeneralService(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IStaticService.CreateGeneralService not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x4: { // CreateGeneralServiceOld
				om.Initialize(1, 0, 0);
				var _return = CreateGeneralServiceOld();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x5: { // CreateGeneralService
				om.Initialize(1, 0, 0);
				var _return = CreateGeneralService(im.GetData<ulong>(8), im.Pid);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nifm.Detail.IStaticService");
		}
	}
}

