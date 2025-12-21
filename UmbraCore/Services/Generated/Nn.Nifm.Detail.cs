using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nifm.Detail;
public partial class IGeneralService : _IGeneralService_Base;
public abstract class _IGeneralService_Base : IpcInterface {
	protected virtual void GetClientId() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetClientId not implemented");
	protected virtual Nn.Nifm.Detail.IScanRequest CreateScanRequest() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.CreateScanRequest not implemented");
	protected virtual Nn.Nifm.Detail.IRequest CreateRequest(uint _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.CreateRequest not implemented");
	protected virtual void GetCurrentNetworkProfile() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetCurrentNetworkProfile not implemented");
	protected virtual void EnumerateNetworkInterfaces(uint _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.EnumerateNetworkInterfaces not implemented");
	protected virtual void EnumerateNetworkProfiles(byte _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.EnumerateNetworkProfiles not implemented");
	protected virtual void GetNetworkProfile(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetNetworkProfile not implemented");
	protected virtual void SetNetworkProfile(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.SetNetworkProfile not implemented");
	protected virtual void RemoveNetworkProfile(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IGeneralService.RemoveNetworkProfile");
	protected virtual void GetScanDataOld() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetScanDataOld not implemented");
	protected virtual void GetCurrentIpAddress() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetCurrentIpAddress not implemented");
	protected virtual void GetCurrentAccessPointOld() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetCurrentAccessPointOld not implemented");
	protected virtual void CreateTemporaryNetworkProfile(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.CreateTemporaryNetworkProfile not implemented");
	protected virtual void GetCurrentIpConfigInfo() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetCurrentIpConfigInfo not implemented");
	protected virtual void SetWirelessCommunicationEnabled(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IGeneralService.SetWirelessCommunicationEnabled");
	protected virtual byte IsWirelessCommunicationEnabled() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.IsWirelessCommunicationEnabled not implemented");
	protected virtual void GetInternetConnectionStatus() =>
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
	protected virtual void GetSsidListVersion() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetSsidListVersion not implemented");
	protected virtual void SetExclusiveClient(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IGeneralService.SetExclusiveClient");
	protected virtual void GetDefaultIpSetting() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetDefaultIpSetting not implemented");
	protected virtual void SetDefaultIpSetting(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IGeneralService.SetDefaultIpSetting");
	protected virtual void SetWirelessCommunicationEnabledForTest(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IGeneralService.SetWirelessCommunicationEnabledForTest");
	protected virtual void SetEthernetCommunicationEnabledForTest(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IGeneralService.SetEthernetCommunicationEnabledForTest");
	protected virtual KObject GetTelemetorySystemEventReadableHandle() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetTelemetorySystemEventReadableHandle not implemented");
	protected virtual void GetTelemetryInfo() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetTelemetryInfo not implemented");
	protected virtual void ConfirmSystemAvailability() =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IGeneralService.ConfirmSystemAvailability");
	protected virtual void SetBackgroundRequestEnabled(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IGeneralService.SetBackgroundRequestEnabled");
	protected virtual void GetScanData() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetScanData not implemented");
	protected virtual void GetCurrentAccessPoint() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IGeneralService.GetCurrentAccessPoint not implemented");
	protected virtual void Shutdown() =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IGeneralService.Shutdown");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: // GetClientId
				break;
			case 0x2: // CreateScanRequest
				break;
			case 0x4: // CreateRequest
				break;
			case 0x5: // GetCurrentNetworkProfile
				break;
			case 0x6: // EnumerateNetworkInterfaces
				break;
			case 0x7: // EnumerateNetworkProfiles
				break;
			case 0x8: // GetNetworkProfile
				break;
			case 0x9: // SetNetworkProfile
				break;
			case 0xA: // RemoveNetworkProfile
				break;
			case 0xB: // GetScanDataOld
				break;
			case 0xC: // GetCurrentIpAddress
				break;
			case 0xD: // GetCurrentAccessPointOld
				break;
			case 0xE: // CreateTemporaryNetworkProfile
				break;
			case 0xF: // GetCurrentIpConfigInfo
				break;
			case 0x10: // SetWirelessCommunicationEnabled
				break;
			case 0x11: // IsWirelessCommunicationEnabled
				break;
			case 0x12: // GetInternetConnectionStatus
				break;
			case 0x13: // SetEthernetCommunicationEnabled
				break;
			case 0x14: // IsEthernetCommunicationEnabled
				break;
			case 0x15: // IsAnyInternetRequestAccepted
				break;
			case 0x16: // IsAnyForegroundRequestAccepted
				break;
			case 0x17: // PutToSleep
				break;
			case 0x18: // WakeUp
				break;
			case 0x19: // GetSsidListVersion
				break;
			case 0x1A: // SetExclusiveClient
				break;
			case 0x1B: // GetDefaultIpSetting
				break;
			case 0x1C: // SetDefaultIpSetting
				break;
			case 0x1D: // SetWirelessCommunicationEnabledForTest
				break;
			case 0x1E: // SetEthernetCommunicationEnabledForTest
				break;
			case 0x1F: // GetTelemetorySystemEventReadableHandle
				break;
			case 0x20: // GetTelemetryInfo
				break;
			case 0x21: // ConfirmSystemAvailability
				break;
			case 0x22: // SetBackgroundRequestEnabled
				break;
			case 0x23: // GetScanData
				break;
			case 0x24: // GetCurrentAccessPoint
				break;
			case 0x25: // Shutdown
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nifm.Detail.IGeneralService");
		}
	}
}

public partial class INetworkProfile : _INetworkProfile_Base;
public abstract class _INetworkProfile_Base : IpcInterface {
	protected virtual void Update(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.INetworkProfile.Update not implemented");
	protected virtual void PersistOld(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.INetworkProfile.PersistOld not implemented");
	protected virtual void Persist() =>
		throw new NotImplementedException("Nn.Nifm.Detail.INetworkProfile.Persist not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Update
				break;
			case 0x1: // PersistOld
				break;
			case 0x2: // Persist
				break;
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
	protected virtual void GetSystemEventReadableHandles() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IRequest.GetSystemEventReadableHandles not implemented");
	protected virtual void Cancel() =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.Cancel");
	protected virtual void Submit() =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.Submit");
	protected virtual void SetRequirement(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.SetRequirement");
	protected virtual void SetRequirementPreset(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.SetRequirementPreset");
	protected virtual void SetPriority(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.SetPriority");
	protected virtual void SetNetworkProfileId(Span<byte> _0) =>
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
	protected virtual void GetRequirement() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IRequest.GetRequirement not implemented");
	protected virtual uint GetRevision() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IRequest.GetRevision not implemented");
	protected virtual void GetAppletInfo(uint _0) =>
		throw new NotImplementedException("Nn.Nifm.Detail.IRequest.GetAppletInfo not implemented");
	protected virtual void GetAdditionalInfo() =>
		throw new NotImplementedException("Nn.Nifm.Detail.IRequest.GetAdditionalInfo not implemented");
	protected virtual void SetKeptInSleep(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.SetKeptInSleep");
	protected virtual void RegisterSocketDescriptor(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.RegisterSocketDescriptor");
	protected virtual void UnregisterSocketDescriptor(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Nifm.Detail.IRequest.UnregisterSocketDescriptor");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetRequestState
				break;
			case 0x1: // GetResult
				break;
			case 0x2: // GetSystemEventReadableHandles
				break;
			case 0x3: // Cancel
				break;
			case 0x4: // Submit
				break;
			case 0x5: // SetRequirement
				break;
			case 0x6: // SetRequirementPreset
				break;
			case 0x8: // SetPriority
				break;
			case 0x9: // SetNetworkProfileId
				break;
			case 0xA: // SetRejectable
				break;
			case 0xB: // SetConnectionConfirmationOption
				break;
			case 0xC: // SetPersistent
				break;
			case 0xD: // SetInstant
				break;
			case 0xE: // SetSustainable
				break;
			case 0xF: // SetRawPriority
				break;
			case 0x10: // SetGreedy
				break;
			case 0x11: // SetSharable
				break;
			case 0x12: // SetRequirementByRevision
				break;
			case 0x13: // GetRequirement
				break;
			case 0x14: // GetRevision
				break;
			case 0x15: // GetAppletInfo
				break;
			case 0x16: // GetAdditionalInfo
				break;
			case 0x17: // SetKeptInSleep
				break;
			case 0x18: // RegisterSocketDescriptor
				break;
			case 0x19: // UnregisterSocketDescriptor
				break;
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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Submit
				break;
			case 0x1: // IsProcessing
				break;
			case 0x2: // GetResult
				break;
			case 0x3: // GetSystemEventReadableHandle
				break;
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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x4: // CreateGeneralServiceOld
				break;
			case 0x5: // CreateGeneralService
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Nifm.Detail.IStaticService");
		}
	}
}

