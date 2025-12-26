using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Tma;
public partial class IHtcManager : _IHtcManager_Base {
	public readonly string ServiceName;
	public IHtcManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IHtcManager_Base : IpcInterface {
	protected virtual void GetEnvironmentVariable(Span<byte> _0, out uint _1, Span<byte> _2) =>
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
	protected virtual void GetBridgeIpAddress(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Tma.IHtcManager.GetBridgeIpAddress not implemented");
	protected virtual void GetBridgePort(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Tma.IHtcManager.GetBridgePort not implemented");
	protected virtual void SetUsbDetachedForDebug(byte _0) =>
		"Stub hit for Nn.Tma.IHtcManager.SetUsbDetachedForDebug".Log();
	protected virtual void GetBridgeSubnetMask(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Tma.IHtcManager.GetBridgeSubnetMask not implemented");
	protected virtual void GetBridgeMacAddress(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Tma.IHtcManager.GetBridgeMacAddress not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetEnvironmentVariable
				GetEnvironmentVariable(im.GetSpan<byte>(0x5, 0), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x1: { // GetEnvironmentVariableLength
				var _return = GetEnvironmentVariableLength(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // BindHostConnectionEvent
				var _return = BindHostConnectionEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x3: { // BindHostDisconnectionEvent
				var _return = BindHostDisconnectionEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x4: { // BindHostConnectionEventForSystem
				var _return = BindHostConnectionEventForSystem();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x5: { // BindHostDisconnectionEventForSystem
				var _return = BindHostDisconnectionEventForSystem();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x6: { // GetBridgeIpAddress
				GetBridgeIpAddress(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // GetBridgePort
				GetBridgePort(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // SetUsbDetachedForDebug
				SetUsbDetachedForDebug(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // GetBridgeSubnetMask
				GetBridgeSubnetMask(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // GetBridgeMacAddress
				GetBridgeMacAddress(im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Tma.IHtcManager");
		}
	}
}

public partial class IHtcsManager : _IHtcsManager_Base {
	public readonly string ServiceName;
	public IHtcsManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IHtcsManager_Base : IpcInterface {
	protected virtual void Unknown0(out uint _0, out uint _1) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.Unknown0 not implemented");
	protected virtual void Unknown1(uint _0, out uint _1, out uint _2) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.Unknown1 not implemented");
	protected virtual void Unknown2(byte[] _0, uint _1, out uint _2, out uint _3) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.Unknown2 not implemented");
	protected virtual void Unknown3(byte[] _0, uint _1, out uint _2, out uint _3) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.Unknown3 not implemented");
	protected virtual void Unknown4(uint _0, uint _1, out uint _2, out uint _3) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.Unknown4 not implemented");
	protected virtual void Unknown5(uint _0, out byte[] _1, out uint _2, out uint _3) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.Unknown5 not implemented");
	protected virtual void Unknown6(uint _0, uint _1, out uint _2, out ulong _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.Unknown6 not implemented");
	protected virtual void Unknown7(uint _0, uint _1, Span<byte> _2, out uint _3, out ulong _4) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.Unknown7 not implemented");
	protected virtual void Unknown8(uint _0, uint _1, out uint _2, out uint _3) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.Unknown8 not implemented");
	protected virtual void Unknown9(uint _0, uint _1, uint _2, out uint _3, out uint _4) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.Unknown9 not implemented");
	protected virtual void GetPeerNameAny(out byte[] _0) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.GetPeerNameAny not implemented");
	protected virtual void GetDefaultHostName(out byte[] _0) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.GetDefaultHostName not implemented");
	protected virtual void CreateSocketOld(out uint _0, out IpcInterface _1) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.CreateSocketOld not implemented");
	protected virtual void CreateSocket(byte _0, out uint _1, out IpcInterface _2) =>
		throw new NotImplementedException("Nn.Tma.IHtcsManager.CreateSocket not implemented");
	protected virtual void RegisterProcessId(ulong _0, ulong _1) =>
		"Stub hit for Nn.Tma.IHtcsManager.RegisterProcessId".Log();
	protected virtual void MonitorManager(ulong _0, ulong _1) =>
		"Stub hit for Nn.Tma.IHtcsManager.MonitorManager".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1(im.GetData<uint>(8), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetBytes(8, 0x42), im.GetData<uint>(76), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(im.GetBytes(8, 0x42), im.GetData<uint>(76), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4(im.GetData<uint>(8), im.GetData<uint>(12), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x5: { // Unknown5
				Unknown5(im.GetData<uint>(8), out var _0, out var _1, out var _2);
				om.Initialize(0, 0, 76);
				om.SetBytes(8, _0);
				om.SetData(76, _1);
				om.SetData(80, _2);
				break;
			}
			case 0x6: { // Unknown6
				Unknown6(im.GetData<uint>(8), im.GetData<uint>(12), out var _0, out var _1, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 16);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x7: { // Unknown7
				Unknown7(im.GetData<uint>(8), im.GetData<uint>(12), im.GetSpan<byte>(0x5, 0), out var _0, out var _1);
				om.Initialize(0, 0, 16);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x8: { // Unknown8
				Unknown8(im.GetData<uint>(8), im.GetData<uint>(12), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x9: { // Unknown9
				Unknown9(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0xA: { // GetPeerNameAny
				GetPeerNameAny(out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0xB: { // GetDefaultHostName
				GetDefaultHostName(out var _0);
				om.Initialize(0, 0, 32);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC: { // CreateSocketOld
				CreateSocketOld(out var _0, out var _1);
				om.Initialize(1, 0, 4);
				om.SetData(8, _0);
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0xD: { // CreateSocket
				CreateSocket(im.GetData<byte>(8), out var _0, out var _1);
				om.Initialize(1, 0, 4);
				om.SetData(8, _0);
				om.Move(0, CreateHandle(_1));
				break;
			}
			case 0x64: { // RegisterProcessId
				RegisterProcessId(im.GetData<ulong>(8), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x65: { // MonitorManager
				MonitorManager(im.GetData<ulong>(8), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Tma.IHtcsManager");
		}
	}
}

public partial class ISocket : _ISocket_Base;
public abstract class _ISocket_Base : IpcInterface {
	protected virtual void _Close(out uint _0, out uint _1) =>
		throw new NotImplementedException("Nn.Tma.ISocket._Close not implemented");
	protected virtual void Connect(byte[] _0, out uint _1, out uint _2) =>
		throw new NotImplementedException("Nn.Tma.ISocket.Connect not implemented");
	protected virtual void Bind(byte[] _0, out uint _1, out uint _2) =>
		throw new NotImplementedException("Nn.Tma.ISocket.Bind not implemented");
	protected virtual void Listen(uint _0, out uint _1, out uint _2) =>
		throw new NotImplementedException("Nn.Tma.ISocket.Listen not implemented");
	protected virtual void Accept(out byte[] _0, out uint _1, out IpcInterface _2) =>
		throw new NotImplementedException("Nn.Tma.ISocket.Accept not implemented");
	protected virtual void Recv(uint _0, out uint _1, out ulong _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Tma.ISocket.Recv not implemented");
	protected virtual void Send(uint _0, Span<byte> _1, out uint _2, out ulong _3) =>
		throw new NotImplementedException("Nn.Tma.ISocket.Send not implemented");
	protected virtual void Shutdown(uint _0, out uint _1, out uint _2) =>
		throw new NotImplementedException("Nn.Tma.ISocket.Shutdown not implemented");
	protected virtual void Fcntl(uint _0, uint _1, out uint _2, out uint _3) =>
		throw new NotImplementedException("Nn.Tma.ISocket.Fcntl not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // _Close
				_Close(out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x1: { // Connect
				Connect(im.GetBytes(8, 0x42), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x2: { // Bind
				Bind(im.GetBytes(8, 0x42), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x3: { // Listen
				Listen(im.GetData<uint>(8), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x4: { // Accept
				Accept(out var _0, out var _1, out var _2);
				om.Initialize(1, 0, 72);
				om.SetBytes(8, _0);
				om.SetData(76, _1);
				om.Move(0, CreateHandle(_2));
				break;
			}
			case 0x5: { // Recv
				Recv(im.GetData<uint>(8), out var _0, out var _1, im.GetSpan<byte>(0x22, 0));
				om.Initialize(0, 0, 16);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x6: { // Send
				Send(im.GetData<uint>(8), im.GetSpan<byte>(0x21, 0), out var _0, out var _1);
				om.Initialize(0, 0, 16);
				om.SetData(8, _0);
				om.SetData(16, _1);
				break;
			}
			case 0x7: { // Shutdown
				Shutdown(im.GetData<uint>(8), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x8: { // Fcntl
				Fcntl(im.GetData<uint>(8), im.GetData<uint>(12), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Tma.ISocket");
		}
	}
}

