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
		"Stub hit for Nn.Nifm.Detail.IGeneralService.RemoveNetworkProfile".Log();
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
		"Stub hit for Nn.Nifm.Detail.IGeneralService.SetWirelessCommunicationEnabled".Log();
	protected virtual byte IsWirelessCommunicationEnabled() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.IsWirelessCommunicationEnabled not implemented");
	protected virtual void GetInternetConnectionStatus(out byte[] _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetInternetConnectionStatus not implemented");
	protected virtual void SetEthernetCommunicationEnabled(byte _0) =>
		"Stub hit for Nn.Nifm.Detail.IGeneralService.SetEthernetCommunicationEnabled".Log();
	protected virtual byte IsEthernetCommunicationEnabled() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.IsEthernetCommunicationEnabled not implemented");
	protected virtual byte IsAnyInternetRequestAccepted(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.IsAnyInternetRequestAccepted not implemented");
	protected virtual byte IsAnyForegroundRequestAccepted() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.IsAnyForegroundRequestAccepted not implemented");
	protected virtual void PutToSleep() =>
		"Stub hit for Nn.Nifm.Detail.IGeneralService.PutToSleep".Log();
	protected virtual void WakeUp() =>
		"Stub hit for Nn.Nifm.Detail.IGeneralService.WakeUp".Log();
	protected virtual void GetSsidListVersion(out byte[] _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetSsidListVersion not implemented");
	protected virtual void SetExclusiveClient(Span<byte> _0) =>
		"Stub hit for Nn.Nifm.Detail.IGeneralService.SetExclusiveClient".Log();
	protected virtual void GetDefaultIpSetting(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetDefaultIpSetting not implemented");
	protected virtual void SetDefaultIpSetting(Span<byte> _0) =>
		"Stub hit for Nn.Nifm.Detail.IGeneralService.SetDefaultIpSetting".Log();
	protected virtual void SetWirelessCommunicationEnabledForTest(byte _0) =>
		"Stub hit for Nn.Nifm.Detail.IGeneralService.SetWirelessCommunicationEnabledForTest".Log();
	protected virtual void SetEthernetCommunicationEnabledForTest(byte _0) =>
		"Stub hit for Nn.Nifm.Detail.IGeneralService.SetEthernetCommunicationEnabledForTest".Log();
	protected virtual KObject GetTelemetorySystemEventReadableHandle() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetTelemetorySystemEventReadableHandle not implemented");
	protected virtual void GetTelemetryInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetTelemetryInfo not implemented");
	protected virtual void ConfirmSystemAvailability() =>
		"Stub hit for Nn.Nifm.Detail.IGeneralService.ConfirmSystemAvailability".Log();
	protected virtual void SetBackgroundRequestEnabled(byte _0) =>
		"Stub hit for Nn.Nifm.Detail.IGeneralService.SetBackgroundRequestEnabled".Log();
	protected virtual void GetScanData(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetScanData not implemented");
	protected virtual void GetCurrentAccessPoint(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetCurrentAccessPoint not implemented");
	protected virtual void Shutdown() =>
		"Stub hit for Nn.Nifm.Detail.IGeneralService.Shutdown".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: { // GetClientId
				GetClientId(im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // CreateScanRequest
				var _return = CreateScanRequest();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x4: { // CreateRequest
				var _return = CreateRequest(im.GetData<uint>(8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x5: { // GetCurrentNetworkProfile
				GetCurrentNetworkProfile(im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // EnumerateNetworkInterfaces
				EnumerateNetworkInterfaces(im.GetData<uint>(8), out var _0, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x7: { // EnumerateNetworkProfiles
				EnumerateNetworkProfiles(im.GetData<byte>(8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x8: { // GetNetworkProfile
				GetNetworkProfile(im.GetBytes(8, 0x10), im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // SetNetworkProfile
				SetNetworkProfile(im.GetSpan<byte>(0x19, 0), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0xA: { // RemoveNetworkProfile
				RemoveNetworkProfile(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // GetScanDataOld
				GetScanDataOld(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0xC: { // GetCurrentIpAddress
				GetCurrentIpAddress(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD: { // GetCurrentAccessPointOld
				GetCurrentAccessPointOld(im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // CreateTemporaryNetworkProfile
				CreateTemporaryNetworkProfile(im.GetSpan<byte>(0x19, 0), out var _0, out var _1);
				om.Initialize(1, 0, 16);
				om.SetBytes(8, _0);
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0xF: { // GetCurrentIpConfigInfo
				GetCurrentIpConfigInfo(out var _0, out var _1);
				om.Initialize(0, 0, 22);
				om.SetBytes(8, _0);
				om.SetBytes(21, _1);
				break;
			}
			case 0x10: { // SetWirelessCommunicationEnabled
				SetWirelessCommunicationEnabled(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x11: { // IsWirelessCommunicationEnabled
				var _return = IsWirelessCommunicationEnabled();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x12: { // GetInternetConnectionStatus
				GetInternetConnectionStatus(out var _0);
				om.Initialize(0, 0, 3);
				om.SetBytes(8, _0);
				break;
			}
			case 0x13: { // SetEthernetCommunicationEnabled
				SetEthernetCommunicationEnabled(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x14: { // IsEthernetCommunicationEnabled
				var _return = IsEthernetCommunicationEnabled();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x15: { // IsAnyInternetRequestAccepted
				var _return = IsAnyInternetRequestAccepted(im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x16: { // IsAnyForegroundRequestAccepted
				var _return = IsAnyForegroundRequestAccepted();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x17: { // PutToSleep
				PutToSleep();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x18: { // WakeUp
				WakeUp();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19: { // GetSsidListVersion
				GetSsidListVersion(out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1A: { // SetExclusiveClient
				SetExclusiveClient(im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1B: { // GetDefaultIpSetting
				GetDefaultIpSetting(im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1C: { // SetDefaultIpSetting
				SetDefaultIpSetting(im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1D: { // SetWirelessCommunicationEnabledForTest
				SetWirelessCommunicationEnabledForTest(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1E: { // SetEthernetCommunicationEnabledForTest
				SetEthernetCommunicationEnabledForTest(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1F: { // GetTelemetorySystemEventReadableHandle
				var _return = GetTelemetorySystemEventReadableHandle();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x20: { // GetTelemetryInfo
				GetTelemetryInfo(im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x21: { // ConfirmSystemAvailability
				ConfirmSystemAvailability();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x22: { // SetBackgroundRequestEnabled
				SetBackgroundRequestEnabled(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x23: { // GetScanData
				GetScanData(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x24: { // GetCurrentAccessPoint
				GetCurrentAccessPoint(im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x25: { // Shutdown
				Shutdown();
				om.Initialize(0, 0, 0);
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
				Update(im.GetSpan<byte>(0x19, 0), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // PersistOld
				PersistOld(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 16);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // Persist
				Persist(out var _0);
				om.Initialize(0, 0, 16);
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
		"Stub hit for Nn.Nifm.Detail.IRequest.GetResult".Log();
	protected virtual void GetSystemEventReadableHandles(out KObject _0, out KObject _1) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IRequest.GetSystemEventReadableHandles not implemented");
	protected virtual void Cancel() =>
		"Stub hit for Nn.Nifm.Detail.IRequest.Cancel".Log();
	protected virtual void Submit() =>
		"Stub hit for Nn.Nifm.Detail.IRequest.Submit".Log();
	protected virtual void SetRequirement(byte[] _0) =>
		"Stub hit for Nn.Nifm.Detail.IRequest.SetRequirement".Log();
	protected virtual void SetRequirementPreset(uint _0) =>
		"Stub hit for Nn.Nifm.Detail.IRequest.SetRequirementPreset".Log();
	protected virtual void SetPriority(byte _0) =>
		"Stub hit for Nn.Nifm.Detail.IRequest.SetPriority".Log();
	protected virtual void SetNetworkProfileId(byte[] _0) =>
		"Stub hit for Nn.Nifm.Detail.IRequest.SetNetworkProfileId".Log();
	protected virtual void SetRejectable(byte _0) =>
		"Stub hit for Nn.Nifm.Detail.IRequest.SetRejectable".Log();
	protected virtual void SetConnectionConfirmationOption(sbyte _0) =>
		"Stub hit for Nn.Nifm.Detail.IRequest.SetConnectionConfirmationOption".Log();
	protected virtual void SetPersistent(byte _0) =>
		"Stub hit for Nn.Nifm.Detail.IRequest.SetPersistent".Log();
	protected virtual void SetInstant(byte _0) =>
		"Stub hit for Nn.Nifm.Detail.IRequest.SetInstant".Log();
	protected virtual void SetSustainable(byte _0, byte _1) =>
		"Stub hit for Nn.Nifm.Detail.IRequest.SetSustainable".Log();
	protected virtual void SetRawPriority(byte _0) =>
		"Stub hit for Nn.Nifm.Detail.IRequest.SetRawPriority".Log();
	protected virtual void SetGreedy(byte _0) =>
		"Stub hit for Nn.Nifm.Detail.IRequest.SetGreedy".Log();
	protected virtual void SetSharable(byte _0) =>
		"Stub hit for Nn.Nifm.Detail.IRequest.SetSharable".Log();
	protected virtual void SetRequirementByRevision(uint _0) =>
		"Stub hit for Nn.Nifm.Detail.IRequest.SetRequirementByRevision".Log();
	protected virtual void GetRequirement(out byte[] _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IRequest.GetRequirement not implemented");
	protected virtual uint GetRevision() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IRequest.GetRevision not implemented");
	protected virtual void GetAppletInfo(uint _0, out uint _1, out uint _2, out uint _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IRequest.GetAppletInfo not implemented");
	protected virtual void GetAdditionalInfo(out uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IRequest.GetAdditionalInfo not implemented");
	protected virtual void SetKeptInSleep(byte _0) =>
		"Stub hit for Nn.Nifm.Detail.IRequest.SetKeptInSleep".Log();
	protected virtual void RegisterSocketDescriptor(uint _0) =>
		"Stub hit for Nn.Nifm.Detail.IRequest.RegisterSocketDescriptor".Log();
	protected virtual void UnregisterSocketDescriptor(uint _0) =>
		"Stub hit for Nn.Nifm.Detail.IRequest.UnregisterSocketDescriptor".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetRequestState
				var _return = GetRequestState();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // GetResult
				GetResult();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GetSystemEventReadableHandles
				GetSystemEventReadableHandles(out var _0, out var _1);
				om.Initialize(0, 2, 0);
				om.Copy(0, CreateHandle(_0, copy: true));
				om.Copy(1, CreateHandle(_1, copy: true));
				break;
			}
			case 0x3: { // Cancel
				Cancel();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // Submit
				Submit();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // SetRequirement
				SetRequirement(im.GetBytes(8, 0x24));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // SetRequirementPreset
				SetRequirementPreset(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // SetPriority
				SetPriority(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // SetNetworkProfileId
				SetNetworkProfileId(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // SetRejectable
				SetRejectable(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // SetConnectionConfirmationOption
				SetConnectionConfirmationOption(im.GetData<sbyte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // SetPersistent
				SetPersistent(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD: { // SetInstant
				SetInstant(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // SetSustainable
				SetSustainable(im.GetData<byte>(8), im.GetData<byte>(9));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xF: { // SetRawPriority
				SetRawPriority(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // SetGreedy
				SetGreedy(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x11: { // SetSharable
				SetSharable(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12: { // SetRequirementByRevision
				SetRequirementByRevision(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x13: { // GetRequirement
				GetRequirement(out var _0);
				om.Initialize(0, 0, 36);
				om.SetBytes(8, _0);
				break;
			}
			case 0x14: { // GetRevision
				var _return = GetRevision();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x15: { // GetAppletInfo
				GetAppletInfo(im.GetData<uint>(8), out var _0, out var _1, out var _2, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 12);
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				break;
			}
			case 0x16: { // GetAdditionalInfo
				GetAdditionalInfo(out var _0, im.GetSpan<byte>(0x16, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x17: { // SetKeptInSleep
				SetKeptInSleep(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x18: { // RegisterSocketDescriptor
				RegisterSocketDescriptor(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19: { // UnregisterSocketDescriptor
				UnregisterSocketDescriptor(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
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
		"Stub hit for Nn.Nifm.Detail.IScanRequest.Submit".Log();
	protected virtual byte IsProcessing() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IScanRequest.IsProcessing not implemented");
	protected virtual void GetResult() =>
		"Stub hit for Nn.Nifm.Detail.IScanRequest.GetResult".Log();
	protected virtual KObject GetSystemEventReadableHandle() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IScanRequest.GetSystemEventReadableHandle not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Submit
				Submit();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // IsProcessing
				var _return = IsProcessing();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // GetResult
				GetResult();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // GetSystemEventReadableHandle
				var _return = GetSystemEventReadableHandle();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nifm.Detail.IScanRequest");
		}
	}
}

public partial class IStaticService : _IStaticService_Base {
	public readonly string ServiceName;
	public IStaticService(string serviceName) => ServiceName = serviceName;
}
public abstract class _IStaticService_Base : IpcInterface {
	protected virtual Nn.Nifm.Detail.IGeneralService CreateGeneralServiceOld() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IStaticService.CreateGeneralServiceOld not implemented");
	protected virtual Nn.Nifm.Detail.IGeneralService CreateGeneralService(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IStaticService.CreateGeneralService not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x4: { // CreateGeneralServiceOld
				var _return = CreateGeneralServiceOld();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x5: { // CreateGeneralService
				var _return = CreateGeneralService(im.GetData<ulong>(8), im.Pid);
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nifm.Detail.IStaticService");
		}
	}
}

