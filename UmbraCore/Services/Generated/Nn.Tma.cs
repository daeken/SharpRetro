using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Tma;
public partial class IHtcManager : _IHtcManager_Base;
public abstract class _IHtcManager_Base : IpcInterface {
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

