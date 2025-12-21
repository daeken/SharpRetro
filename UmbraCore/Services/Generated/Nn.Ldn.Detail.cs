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
	protected virtual void GetSecurityParameterForMonitor(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ldn.Detail.IMonitorService.GetSecurityParameterForMonitor not implemented");
	protected virtual void GetNetworkConfigForMonitor(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ldn.Detail.IMonitorService.GetNetworkConfigForMonitor not implemented");
	protected virtual void InitializeMonitor() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IMonitorService.InitializeMonitor");
	protected virtual void FinalizeMonitor() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IMonitorService.FinalizeMonitor");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetStateForMonitor
				var _return = GetStateForMonitor();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // GetNetworkInfoForMonitor
				GetNetworkInfoForMonitor(im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GetIpv4AddressForMonitor
				GetIpv4AddressForMonitor(out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x3: { // GetDisconnectReasonForMonitor
				var _return = GetDisconnectReasonForMonitor();
				om.Initialize(0, 0, 2);
				om.SetData(8, _return);
				break;
			}
			case 0x4: { // GetSecurityParameterForMonitor
				GetSecurityParameterForMonitor(out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // GetNetworkConfigForMonitor
				GetNetworkConfigForMonitor(out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0x64: { // InitializeMonitor
				InitializeMonitor();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x65: { // FinalizeMonitor
				FinalizeMonitor();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldn.Detail.IMonitorService");
		}
	}
}

public partial class IMonitorServiceCreator : _IMonitorServiceCreator_Base {
	public readonly string ServiceName;
	public IMonitorServiceCreator(string serviceName) => ServiceName = serviceName;
}
public abstract class _IMonitorServiceCreator_Base : IpcInterface {
	protected virtual Nn.Ldn.Detail.IMonitorService CreateMonitorService() =>
		throw new NotImplementedException("Nn.Ldn.Detail.IMonitorServiceCreator.CreateMonitorService not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateMonitorService
				var _return = CreateMonitorService();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
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
	protected virtual void GetSecurityParameter(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ldn.Detail.ISystemLocalCommunicationService.GetSecurityParameter not implemented");
	protected virtual void GetNetworkConfig(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ldn.Detail.ISystemLocalCommunicationService.GetNetworkConfig not implemented");
	protected virtual KObject AttachStateChangeEvent() =>
		throw new NotImplementedException("Nn.Ldn.Detail.ISystemLocalCommunicationService.AttachStateChangeEvent not implemented");
	protected virtual void GetNetworkInfoLatestUpdate(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ldn.Detail.ISystemLocalCommunicationService.GetNetworkInfoLatestUpdate not implemented");
	protected virtual void Scan(ushort _0, byte[] _1, out ushort _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Ldn.Detail.ISystemLocalCommunicationService.Scan not implemented");
	protected virtual void ScanPrivate(ushort _0, byte[] _1, out ushort _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Ldn.Detail.ISystemLocalCommunicationService.ScanPrivate not implemented");
	protected virtual void OpenAccessPoint() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.OpenAccessPoint");
	protected virtual void CloseAccessPoint() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.CloseAccessPoint");
	protected virtual void CreateNetwork(byte[] _0, byte[] _1, byte[] _2) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.CreateNetwork");
	protected virtual void CreateNetworkPrivate(byte[] _0, byte[] _1, byte[] _2, byte[] _3, Span<byte> _4) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.CreateNetworkPrivate");
	protected virtual void DestroyNetwork() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.DestroyNetwork");
	protected virtual void Reject(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.Reject");
	protected virtual void SetAdvertiseData(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.SetAdvertiseData");
	protected virtual void SetStationAcceptPolicy(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.SetStationAcceptPolicy");
	protected virtual void AddAcceptFilterEntry(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.AddAcceptFilterEntry");
	protected virtual void ClearAcceptFilter() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.ClearAcceptFilter");
	protected virtual void OpenStation() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.OpenStation");
	protected virtual void CloseStation() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.CloseStation");
	protected virtual void Connect(byte[] _0, byte[] _1, uint _2, uint _3, Span<byte> _4) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.Connect");
	protected virtual void ConnectPrivate(byte[] _0, byte[] _1, byte[] _2, uint _3, uint _4, byte[] _5) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.ConnectPrivate");
	protected virtual void Disconnect() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.Disconnect");
	protected virtual void InitializeSystem(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.InitializeSystem");
	protected virtual void FinalizeSystem() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.ISystemLocalCommunicationService.FinalizeSystem");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetState
				var _return = GetState();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // GetNetworkInfo
				GetNetworkInfo(im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GetIpv4Address
				GetIpv4Address(out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x3: { // GetDisconnectReason
				var _return = GetDisconnectReason();
				om.Initialize(0, 0, 2);
				om.SetData(8, _return);
				break;
			}
			case 0x4: { // GetSecurityParameter
				GetSecurityParameter(out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // GetNetworkConfig
				GetNetworkConfig(out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0x64: { // AttachStateChangeEvent
				var _return = AttachStateChangeEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x65: { // GetNetworkInfoLatestUpdate
				GetNetworkInfoLatestUpdate(im.GetSpan<byte>(0x1A, 0), im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x66: { // Scan
				Scan(im.GetData<ushort>(8), im.GetBytes(16, 0x60), out var _0, im.GetSpan<byte>(0x22, 0));
				om.Initialize(0, 0, 2);
				om.SetData(8, _0);
				break;
			}
			case 0x67: { // ScanPrivate
				ScanPrivate(im.GetData<ushort>(8), im.GetBytes(16, 0x60), out var _0, im.GetSpan<byte>(0x22, 0));
				om.Initialize(0, 0, 2);
				om.SetData(8, _0);
				break;
			}
			case 0xC8: { // OpenAccessPoint
				OpenAccessPoint();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC9: { // CloseAccessPoint
				CloseAccessPoint();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCA: { // CreateNetwork
				CreateNetwork(im.GetBytes(8, 0x44), im.GetBytes(76, 0x30), im.GetBytes(128, 0x20));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCB: { // CreateNetworkPrivate
				CreateNetworkPrivate(im.GetBytes(8, 0x44), im.GetBytes(76, 0x20), im.GetBytes(108, 0x30), im.GetBytes(160, 0x20), im.GetSpan<byte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCC: { // DestroyNetwork
				DestroyNetwork();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCD: { // Reject
				Reject(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCE: { // SetAdvertiseData
				SetAdvertiseData(im.GetSpan<byte>(0x21, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCF: { // SetStationAcceptPolicy
				SetStationAcceptPolicy(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD0: { // AddAcceptFilterEntry
				AddAcceptFilterEntry(im.GetBytes(8, 0x6));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD1: { // ClearAcceptFilter
				ClearAcceptFilter();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12C: { // OpenStation
				OpenStation();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12D: { // CloseStation
				CloseStation();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12E: { // Connect
				Connect(im.GetBytes(8, 0x44), im.GetBytes(76, 0x30), im.GetData<uint>(124), im.GetData<uint>(128), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12F: { // ConnectPrivate
				ConnectPrivate(im.GetBytes(8, 0x44), im.GetBytes(76, 0x20), im.GetBytes(108, 0x30), im.GetData<uint>(156), im.GetData<uint>(160), im.GetBytes(168, 0x20));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x130: { // Disconnect
				Disconnect();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x190: { // InitializeSystem
				InitializeSystem(im.GetData<ulong>(8), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x191: { // FinalizeSystem
				FinalizeSystem();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldn.Detail.ISystemLocalCommunicationService");
		}
	}
}

public partial class ISystemServiceCreator : _ISystemServiceCreator_Base {
	public readonly string ServiceName;
	public ISystemServiceCreator(string serviceName) => ServiceName = serviceName;
}
public abstract class _ISystemServiceCreator_Base : IpcInterface {
	protected virtual Nn.Ldn.Detail.ISystemLocalCommunicationService CreateSystemLocalCommunicationService() =>
		throw new NotImplementedException("Nn.Ldn.Detail.ISystemServiceCreator.CreateSystemLocalCommunicationService not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateSystemLocalCommunicationService
				var _return = CreateSystemLocalCommunicationService();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
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
	protected virtual void GetSecurityParameter(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ldn.Detail.IUserLocalCommunicationService.GetSecurityParameter not implemented");
	protected virtual void GetNetworkConfig(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ldn.Detail.IUserLocalCommunicationService.GetNetworkConfig not implemented");
	protected virtual KObject AttachStateChangeEvent() =>
		throw new NotImplementedException("Nn.Ldn.Detail.IUserLocalCommunicationService.AttachStateChangeEvent not implemented");
	protected virtual void GetNetworkInfoLatestUpdate(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Ldn.Detail.IUserLocalCommunicationService.GetNetworkInfoLatestUpdate not implemented");
	protected virtual void Scan(ushort _0, byte[] _1, out ushort _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Ldn.Detail.IUserLocalCommunicationService.Scan not implemented");
	protected virtual void ScanPrivate(ushort _0, byte[] _1, out ushort _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Ldn.Detail.IUserLocalCommunicationService.ScanPrivate not implemented");
	protected virtual void OpenAccessPoint() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.OpenAccessPoint");
	protected virtual void CloseAccessPoint() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.CloseAccessPoint");
	protected virtual void CreateNetwork(byte[] _0, byte[] _1, byte[] _2) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.CreateNetwork");
	protected virtual void CreateNetworkPrivate(byte[] _0, byte[] _1, byte[] _2, byte[] _3, Span<byte> _4) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.CreateNetworkPrivate");
	protected virtual void DestroyNetwork() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.DestroyNetwork");
	protected virtual void Reject(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.Reject");
	protected virtual void SetAdvertiseData(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.SetAdvertiseData");
	protected virtual void SetStationAcceptPolicy(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.SetStationAcceptPolicy");
	protected virtual void AddAcceptFilterEntry(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.AddAcceptFilterEntry");
	protected virtual void ClearAcceptFilter() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.ClearAcceptFilter");
	protected virtual void OpenStation() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.OpenStation");
	protected virtual void CloseStation() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.CloseStation");
	protected virtual void Connect(byte[] _0, byte[] _1, uint _2, uint _3, Span<byte> _4) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.Connect");
	protected virtual void ConnectPrivate(byte[] _0, byte[] _1, byte[] _2, uint _3, uint _4, byte[] _5) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.ConnectPrivate");
	protected virtual void Disconnect() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.Disconnect");
	protected virtual void Initialize(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService.Initialize");
	protected virtual void _Finalize() =>
		Console.WriteLine("Stub hit for Nn.Ldn.Detail.IUserLocalCommunicationService._Finalize");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetState
				var _return = GetState();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // GetNetworkInfo
				GetNetworkInfo(im.GetSpan<byte>(0x1A, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GetIpv4Address
				GetIpv4Address(out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x3: { // GetDisconnectReason
				var _return = GetDisconnectReason();
				om.Initialize(0, 0, 2);
				om.SetData(8, _return);
				break;
			}
			case 0x4: { // GetSecurityParameter
				GetSecurityParameter(out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // GetNetworkConfig
				GetNetworkConfig(out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0x64: { // AttachStateChangeEvent
				var _return = AttachStateChangeEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x65: { // GetNetworkInfoLatestUpdate
				GetNetworkInfoLatestUpdate(im.GetSpan<byte>(0x1A, 0), im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x66: { // Scan
				Scan(im.GetData<ushort>(8), im.GetBytes(16, 0x60), out var _0, im.GetSpan<byte>(0x22, 0));
				om.Initialize(0, 0, 2);
				om.SetData(8, _0);
				break;
			}
			case 0x67: { // ScanPrivate
				ScanPrivate(im.GetData<ushort>(8), im.GetBytes(16, 0x60), out var _0, im.GetSpan<byte>(0x22, 0));
				om.Initialize(0, 0, 2);
				om.SetData(8, _0);
				break;
			}
			case 0xC8: { // OpenAccessPoint
				OpenAccessPoint();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC9: { // CloseAccessPoint
				CloseAccessPoint();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCA: { // CreateNetwork
				CreateNetwork(im.GetBytes(8, 0x44), im.GetBytes(76, 0x30), im.GetBytes(128, 0x20));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCB: { // CreateNetworkPrivate
				CreateNetworkPrivate(im.GetBytes(8, 0x44), im.GetBytes(76, 0x20), im.GetBytes(108, 0x30), im.GetBytes(160, 0x20), im.GetSpan<byte>(0x9, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCC: { // DestroyNetwork
				DestroyNetwork();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCD: { // Reject
				Reject(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCE: { // SetAdvertiseData
				SetAdvertiseData(im.GetSpan<byte>(0x21, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xCF: { // SetStationAcceptPolicy
				SetStationAcceptPolicy(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD0: { // AddAcceptFilterEntry
				AddAcceptFilterEntry(im.GetBytes(8, 0x6));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD1: { // ClearAcceptFilter
				ClearAcceptFilter();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12C: { // OpenStation
				OpenStation();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12D: { // CloseStation
				CloseStation();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12E: { // Connect
				Connect(im.GetBytes(8, 0x44), im.GetBytes(76, 0x30), im.GetData<uint>(124), im.GetData<uint>(128), im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12F: { // ConnectPrivate
				ConnectPrivate(im.GetBytes(8, 0x44), im.GetBytes(76, 0x20), im.GetBytes(108, 0x30), im.GetData<uint>(156), im.GetData<uint>(160), im.GetBytes(168, 0x20));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x130: { // Disconnect
				Disconnect();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x190: { // Initialize
				Initialize(im.GetData<ulong>(8), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x191: { // _Finalize
				_Finalize();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldn.Detail.IUserLocalCommunicationService");
		}
	}
}

public partial class IUserServiceCreator : _IUserServiceCreator_Base {
	public readonly string ServiceName;
	public IUserServiceCreator(string serviceName) => ServiceName = serviceName;
}
public abstract class _IUserServiceCreator_Base : IpcInterface {
	protected virtual Nn.Ldn.Detail.IUserLocalCommunicationService CreateUserLocalCommunicationService() =>
		throw new NotImplementedException("Nn.Ldn.Detail.IUserServiceCreator.CreateUserLocalCommunicationService not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CreateUserLocalCommunicationService
				var _return = CreateUserLocalCommunicationService();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ldn.Detail.IUserServiceCreator");
		}
	}
}

