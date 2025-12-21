using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Tma;
public partial class IHtcManager : _IHtcManager_Base;
public abstract class _IHtcManager_Base : IpcInterface {
	protected virtual void GetEnvironmentVariable(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Tma.IHtcManager.GetEnvironmentVariable not implemented");
	protected virtual uint GetEnvironmentVariableLength(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Tma.IHtcManager.GetEnvironmentVariableLength not implemented");
	protected virtual KObject BindHostConnectionEvent() =>
		throw new NotImplementedException("Nn.Tma.IHtcManager.BindHostConnectionEvent not implemented");
	protected virtual KObject BindHostDisconnectionEvent() =>
		throw new NotImplementedException("Nn.Tma.IHtcManager.BindHostDisconnectionEvent not implemented");
	protected virtual KObject BindHostConnectionEventForSystem() =>
		throw new NotImplementedException("Nn.Tma.IHtcManager.BindHostConnectionEventForSystem not implemented");
	protected virtual KObject BindHostDisconnectionEventForSystem() =>
		throw new NotImplementedException("Nn.Tma.IHtcManager.BindHostDisconnectionEventForSystem not implemented");
	protected virtual void GetBridgeIpAddress() =>
		throw new NotImplementedException("Nn.Tma.IHtcManager.GetBridgeIpAddress not implemented");
	protected virtual void GetBridgePort() =>
		throw new NotImplementedException("Nn.Tma.IHtcManager.GetBridgePort not implemented");
	protected virtual void SetUsbDetachedForDebug(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Tma.IHtcManager.SetUsbDetachedForDebug");
	protected virtual void GetBridgeSubnetMask() =>
		throw new NotImplementedException("Nn.Tma.IHtcManager.GetBridgeSubnetMask not implemented");
	protected virtual void GetBridgeMacAddress() =>
		throw new NotImplementedException("Nn.Tma.IHtcManager.GetBridgeMacAddress not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetEnvironmentVariable
				break;
			case 0x1: // GetEnvironmentVariableLength
				break;
			case 0x2: // BindHostConnectionEvent
				break;
			case 0x3: // BindHostDisconnectionEvent
				break;
			case 0x4: // BindHostConnectionEventForSystem
				break;
			case 0x5: // BindHostDisconnectionEventForSystem
				break;
			case 0x6: // GetBridgeIpAddress
				break;
			case 0x7: // GetBridgePort
				break;
			case 0x8: // SetUsbDetachedForDebug
				break;
			case 0x9: // GetBridgeSubnetMask
				break;
			case 0xA: // GetBridgeMacAddress
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Tma.IHtcManager");
		}
	}
}

public partial class IHtcsManager : _IHtcsManager_Base;
public abstract class _IHtcsManager_Base : IpcInterface {
	protected virtual void Unknown0() =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.Unknown0 not implemented");
	protected virtual void Unknown1(uint _0) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.Unknown1 not implemented");
	protected virtual void Unknown2(Span<byte> _0, uint _1) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0, uint _1) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.Unknown3 not implemented");
	protected virtual void Unknown4(uint _0, uint _1) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.Unknown4 not implemented");
	protected virtual void Unknown5(uint _0) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.Unknown5 not implemented");
	protected virtual void Unknown6(uint _0, uint _1) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.Unknown6 not implemented");
	protected virtual void Unknown7(uint _0, uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.Unknown7 not implemented");
	protected virtual void Unknown8(uint _0, uint _1) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.Unknown8 not implemented");
	protected virtual void Unknown9(uint _0, uint _1, uint _2) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.Unknown9 not implemented");
	protected virtual void GetPeerNameAny() =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.GetPeerNameAny not implemented");
	protected virtual void GetDefaultHostName() =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.GetDefaultHostName not implemented");
	protected virtual void CreateSocketOld() =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.CreateSocketOld not implemented");
	protected virtual void CreateSocket(byte _0) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.CreateSocket not implemented");
	protected virtual void RegisterProcessId(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Tma.IHtcsManager.RegisterProcessId");
	protected virtual void MonitorManager(ulong _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Tma.IHtcsManager.MonitorManager");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			case 0x5: // Unknown5
				break;
			case 0x6: // Unknown6
				break;
			case 0x7: // Unknown7
				break;
			case 0x8: // Unknown8
				break;
			case 0x9: // Unknown9
				break;
			case 0xA: // GetPeerNameAny
				break;
			case 0xB: // GetDefaultHostName
				break;
			case 0xC: // CreateSocketOld
				break;
			case 0xD: // CreateSocket
				break;
			case 0x64: // RegisterProcessId
				break;
			case 0x65: // MonitorManager
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Tma.IHtcsManager");
		}
	}
}

public partial class ISocket : _ISocket_Base;
public abstract class _ISocket_Base : IpcInterface {
	protected virtual void Close() =>
		throw new NotImplementedException("Nn.Tma.ISocket.Close not implemented");
	protected virtual void Connect(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Tma.ISocket.Connect not implemented");
	protected virtual void Bind(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Tma.ISocket.Bind not implemented");
	protected virtual void Listen(uint _0) =>
		throw new NotImplementedException("Nn.Tma.ISocket.Listen not implemented");
	protected virtual void Accept() =>
		throw new NotImplementedException("Nn.Tma.ISocket.Accept not implemented");
	protected virtual void Recv(uint _0) =>
		throw new NotImplementedException("Nn.Tma.ISocket.Recv not implemented");
	protected virtual void Send(uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Tma.ISocket.Send not implemented");
	protected virtual void Shutdown(uint _0) =>
		throw new NotImplementedException("Nn.Tma.ISocket.Shutdown not implemented");
	protected virtual void Fcntl(uint _0, uint _1) =>
		throw new NotImplementedException("Nn.Tma.ISocket.Fcntl not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Close
				break;
			case 0x1: // Connect
				break;
			case 0x2: // Bind
				break;
			case 0x3: // Listen
				break;
			case 0x4: // Accept
				break;
			case 0x5: // Recv
				break;
			case 0x6: // Send
				break;
			case 0x7: // Shutdown
				break;
			case 0x8: // Fcntl
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Tma.ISocket");
		}
	}
}

