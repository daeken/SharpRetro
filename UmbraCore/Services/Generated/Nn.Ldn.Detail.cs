using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ldn.Detail;
public partial class IMonitorService : _IMonitorService_Base;
public abstract class _IMonitorService_Base : IpcInterface {
	protected virtual uint GetStateForMonitor() =>
		throw new NotImplementedException("Nn.Ldn.Detail.IMonitorService.GetStateForMonitor not implemented");
	protected virtual void GetNetworkInfoForMonitor(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ldn.Detail.IMonitorService.GetNetworkInfoForMonitor not implemented");
	protected virtual void GetIpv4AddressForMonitor(out uint _0, out uint _1) =>
		throw new NotImplementedException("Nn.Ldn.Detail.IMonitorService.GetIpv4AddressForMonitor not implemented");
	protected virtual ushort GetDisconnectReasonForMonitor() =>
		throw new NotImplementedException("Nn.Ldn.Detail.IMonitorService.GetDisconnectReasonForMonitor not implemented");
	protected virtual void GetSecurityParameterForMonitor(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ldn.Detail.IMonitorService.GetSecurityParameterForMonitor not implemented");
	protected virtual void GetNetworkConfigForMonitor(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ldn.Detail.IMonitorService.GetNetworkConfigForMonitor not implemented");
	protected virtual void InitializeMonitor() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IMonitorService.InitializeMonitor");
	protected virtual void FinalizeMonitor() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IMonitorService.FinalizeMonitor");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetStateForMonitor
				break;
			}
			case 0x1: { // GetNetworkInfoForMonitor
				break;
			}
			case 0x2: { // GetIpv4AddressForMonitor
				break;
			}
			case 0x3: { // GetDisconnectReasonForMonitor
				break;
			}
			case 0x4: { // GetSecurityParameterForMonitor
				break;
			}
			case 0x5: { // GetNetworkConfigForMonitor
				break;
			}
			case 0x64: { // InitializeMonitor
				break;
			}
			case 0x65: { // FinalizeMonitor
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldn.Detail.IMonitorService");
		}
	}
}

public partial class IMonitorServiceCreator : _IMonitorServiceCreator_Base;
public abstract class _IMonitorServiceCreator_Base : IpcInterface {
	protected virtual Nn.Ldn.Detail.IMonitorService CreateMonitorService() =>
		throw new NotImplementedException("Nn.Ldn.Detail.IMonitorServiceCreator.CreateMonitorService not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateMonitorService
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldn.Detail.IMonitorServiceCreator");
		}
	}
}

public partial class ISystemLocalCommunicationService : _ISystemLocalCommunicationService_Base;
public abstract class _ISystemLocalCommunicationService_Base : IpcInterface {
	protected virtual uint GetState() =>
		throw new NotImplementedException("Nn.Ldn.Detail.ISystemLocalCommunicationService.GetState not implemented");
	protected virtual void GetNetworkInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ldn.Detail.ISystemLocalCommunicationService.GetNetworkInfo not implemented");
	protected virtual void GetIpv4Address(out uint _0, out uint _1) =>
		throw new NotImplementedException("Nn.Ldn.Detail.ISystemLocalCommunicationService.GetIpv4Address not implemented");
	protected virtual ushort GetDisconnectReason() =>
		throw new NotImplementedException("Nn.Ldn.Detail.ISystemLocalCommunicationService.GetDisconnectReason not implemented");
	protected virtual void GetSecurityParameter(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ldn.Detail.ISystemLocalCommunicationService.GetSecurityParameter not implemented");
	protected virtual void GetNetworkConfig(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ldn.Detail.ISystemLocalCommunicationService.GetNetworkConfig not implemented");
	protected virtual KObject AttachStateChangeEvent() =>
		throw new NotImplementedException("Nn.Ldn.Detail.ISystemLocalCommunicationService.AttachStateChangeEvent not implemented");
	protected virtual void GetNetworkInfoLatestUpdate(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ldn.Detail.ISystemLocalCommunicationService.GetNetworkInfoLatestUpdate not implemented");
	protected virtual void Scan(ushort _0, Span<byte> _1, out ushort _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Ldn.Detail.ISystemLocalCommunicationService.Scan not implemented");
	protected virtual void ScanPrivate(ushort _0, Span<byte> _1, out ushort _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Ldn.Detail.ISystemLocalCommunicationService.ScanPrivate not implemented");
	protected virtual void OpenAccessPoint() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.OpenAccessPoint");
	protected virtual void CloseAccessPoint() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.CloseAccessPoint");
	protected virtual void CreateNetwork(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.CreateNetwork");
	protected virtual void CreateNetworkPrivate(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3, Span<byte> _4) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.CreateNetworkPrivate");
	protected virtual void DestroyNetwork() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.DestroyNetwork");
	protected virtual void Reject(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.Reject");
	protected virtual void SetAdvertiseData(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.SetAdvertiseData");
	protected virtual void SetStationAcceptPolicy(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.SetStationAcceptPolicy");
	protected virtual void AddAcceptFilterEntry(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.AddAcceptFilterEntry");
	protected virtual void ClearAcceptFilter() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.ClearAcceptFilter");
	protected virtual void OpenStation() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.OpenStation");
	protected virtual void CloseStation() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.CloseStation");
	protected virtual void Connect(Span<byte> _0, Span<byte> _1, uint _2, uint _3, Span<byte> _4) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.Connect");
	protected virtual void ConnectPrivate(Span<byte> _0, Span<byte> _1, Span<byte> _2, uint _3, uint _4, Span<byte> _5) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.ConnectPrivate");
	protected virtual void Disconnect() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.Disconnect");
	protected virtual void InitializeSystem(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.InitializeSystem");
	protected virtual void FinalizeSystem() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.FinalizeSystem");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetState
				break;
			}
			case 0x1: { // GetNetworkInfo
				break;
			}
			case 0x2: { // GetIpv4Address
				break;
			}
			case 0x3: { // GetDisconnectReason
				break;
			}
			case 0x4: { // GetSecurityParameter
				break;
			}
			case 0x5: { // GetNetworkConfig
				break;
			}
			case 0x64: { // AttachStateChangeEvent
				break;
			}
			case 0x65: { // GetNetworkInfoLatestUpdate
				break;
			}
			case 0x66: { // Scan
				break;
			}
			case 0x67: { // ScanPrivate
				break;
			}
			case 0xC8: { // OpenAccessPoint
				break;
			}
			case 0xC9: { // CloseAccessPoint
				break;
			}
			case 0xCA: { // CreateNetwork
				break;
			}
			case 0xCB: { // CreateNetworkPrivate
				break;
			}
			case 0xCC: { // DestroyNetwork
				break;
			}
			case 0xCD: { // Reject
				break;
			}
			case 0xCE: { // SetAdvertiseData
				break;
			}
			case 0xCF: { // SetStationAcceptPolicy
				break;
			}
			case 0xD0: { // AddAcceptFilterEntry
				break;
			}
			case 0xD1: { // ClearAcceptFilter
				break;
			}
			case 0x12C: { // OpenStation
				break;
			}
			case 0x12D: { // CloseStation
				break;
			}
			case 0x12E: { // Connect
				break;
			}
			case 0x12F: { // ConnectPrivate
				break;
			}
			case 0x130: { // Disconnect
				break;
			}
			case 0x190: { // InitializeSystem
				break;
			}
			case 0x191: { // FinalizeSystem
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldn.Detail.ISystemLocalCommunicationService");
		}
	}
}

public partial class ISystemServiceCreator : _ISystemServiceCreator_Base;
public abstract class _ISystemServiceCreator_Base : IpcInterface {
	protected virtual Nn.Ldn.Detail.ISystemLocalCommunicationService CreateSystemLocalCommunicationService() =>
		throw new NotImplementedException("Nn.Ldn.Detail.ISystemServiceCreator.CreateSystemLocalCommunicationService not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateSystemLocalCommunicationService
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldn.Detail.ISystemServiceCreator");
		}
	}
}

public partial class IUserLocalCommunicationService : _IUserLocalCommunicationService_Base;
public abstract class _IUserLocalCommunicationService_Base : IpcInterface {
	protected virtual uint GetState() =>
		throw new NotImplementedException("Nn.Ldn.Detail.IUserLocalCommunicationService.GetState not implemented");
	protected virtual void GetNetworkInfo(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ldn.Detail.IUserLocalCommunicationService.GetNetworkInfo not implemented");
	protected virtual void GetIpv4Address(out uint _0, out uint _1) =>
		throw new NotImplementedException("Nn.Ldn.Detail.IUserLocalCommunicationService.GetIpv4Address not implemented");
	protected virtual ushort GetDisconnectReason() =>
		throw new NotImplementedException("Nn.Ldn.Detail.IUserLocalCommunicationService.GetDisconnectReason not implemented");
	protected virtual void GetSecurityParameter(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ldn.Detail.IUserLocalCommunicationService.GetSecurityParameter not implemented");
	protected virtual void GetNetworkConfig(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ldn.Detail.IUserLocalCommunicationService.GetNetworkConfig not implemented");
	protected virtual KObject AttachStateChangeEvent() =>
		throw new NotImplementedException("Nn.Ldn.Detail.IUserLocalCommunicationService.AttachStateChangeEvent not implemented");
	protected virtual void GetNetworkInfoLatestUpdate(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ldn.Detail.IUserLocalCommunicationService.GetNetworkInfoLatestUpdate not implemented");
	protected virtual void Scan(ushort _0, Span<byte> _1, out ushort _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Ldn.Detail.IUserLocalCommunicationService.Scan not implemented");
	protected virtual void ScanPrivate(ushort _0, Span<byte> _1, out ushort _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Ldn.Detail.IUserLocalCommunicationService.ScanPrivate not implemented");
	protected virtual void OpenAccessPoint() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.OpenAccessPoint");
	protected virtual void CloseAccessPoint() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.CloseAccessPoint");
	protected virtual void CreateNetwork(Span<byte> _0, Span<byte> _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.CreateNetwork");
	protected virtual void CreateNetworkPrivate(Span<byte> _0, Span<byte> _1, Span<byte> _2, Span<byte> _3, Span<byte> _4) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.CreateNetworkPrivate");
	protected virtual void DestroyNetwork() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.DestroyNetwork");
	protected virtual void Reject(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.Reject");
	protected virtual void SetAdvertiseData(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.SetAdvertiseData");
	protected virtual void SetStationAcceptPolicy(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.SetStationAcceptPolicy");
	protected virtual void AddAcceptFilterEntry(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.AddAcceptFilterEntry");
	protected virtual void ClearAcceptFilter() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.ClearAcceptFilter");
	protected virtual void OpenStation() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.OpenStation");
	protected virtual void CloseStation() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.CloseStation");
	protected virtual void Connect(Span<byte> _0, Span<byte> _1, uint _2, uint _3, Span<byte> _4) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.Connect");
	protected virtual void ConnectPrivate(Span<byte> _0, Span<byte> _1, Span<byte> _2, uint _3, uint _4, Span<byte> _5) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.ConnectPrivate");
	protected virtual void Disconnect() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.Disconnect");
	protected virtual void Initialize(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.Initialize");
	protected virtual void Finalize() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.Finalize");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetState
				break;
			}
			case 0x1: { // GetNetworkInfo
				break;
			}
			case 0x2: { // GetIpv4Address
				break;
			}
			case 0x3: { // GetDisconnectReason
				break;
			}
			case 0x4: { // GetSecurityParameter
				break;
			}
			case 0x5: { // GetNetworkConfig
				break;
			}
			case 0x64: { // AttachStateChangeEvent
				break;
			}
			case 0x65: { // GetNetworkInfoLatestUpdate
				break;
			}
			case 0x66: { // Scan
				break;
			}
			case 0x67: { // ScanPrivate
				break;
			}
			case 0xC8: { // OpenAccessPoint
				break;
			}
			case 0xC9: { // CloseAccessPoint
				break;
			}
			case 0xCA: { // CreateNetwork
				break;
			}
			case 0xCB: { // CreateNetworkPrivate
				break;
			}
			case 0xCC: { // DestroyNetwork
				break;
			}
			case 0xCD: { // Reject
				break;
			}
			case 0xCE: { // SetAdvertiseData
				break;
			}
			case 0xCF: { // SetStationAcceptPolicy
				break;
			}
			case 0xD0: { // AddAcceptFilterEntry
				break;
			}
			case 0xD1: { // ClearAcceptFilter
				break;
			}
			case 0x12C: { // OpenStation
				break;
			}
			case 0x12D: { // CloseStation
				break;
			}
			case 0x12E: { // Connect
				break;
			}
			case 0x12F: { // ConnectPrivate
				break;
			}
			case 0x130: { // Disconnect
				break;
			}
			case 0x190: { // Initialize
				break;
			}
			case 0x191: { // Finalize
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldn.Detail.IUserLocalCommunicationService");
		}
	}
}

public partial class IUserServiceCreator : _IUserServiceCreator_Base;
public abstract class _IUserServiceCreator_Base : IpcInterface {
	protected virtual Nn.Ldn.Detail.IUserLocalCommunicationService CreateUserLocalCommunicationService() =>
		throw new NotImplementedException("Nn.Ldn.Detail.IUserServiceCreator.CreateUserLocalCommunicationService not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateUserLocalCommunicationService
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldn.Detail.IUserServiceCreator");
		}
	}
}

