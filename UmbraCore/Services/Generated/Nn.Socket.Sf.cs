using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Socket.Sf;
public partial class IClient : _IClient_Base;
public abstract class _IClient_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // RegisterClient
				break;
			case 0x1: // StartMonitoring
				break;
			case 0x2: // Socket
				break;
			case 0x3: // SocketExempt
				break;
			case 0x4: // Open
				break;
			case 0x5: // Select
				break;
			case 0x6: // Poll
				break;
			case 0x7: // Sysctl
				break;
			case 0x8: // Recv
				break;
			case 0x9: // RecvFrom
				break;
			case 0xA: // Send
				break;
			case 0xB: // SendTo
				break;
			case 0xC: // Accept
				break;
			case 0xD: // Bind
				break;
			case 0xE: // Connect
				break;
			case 0xF: // GetPeerName
				break;
			case 0x10: // GetSockName
				break;
			case 0x11: // GetSockOpt
				break;
			case 0x12: // Listen
				break;
			case 0x13: // Ioctl
				break;
			case 0x14: // Fcntl
				break;
			case 0x15: // SetSockOpt
				break;
			case 0x16: // Shutdown
				break;
			case 0x17: // ShutdownAllSockets
				break;
			case 0x18: // Write
				break;
			case 0x19: // Read
				break;
			case 0x1A: // Close
				break;
			case 0x1B: // DuplicateSocket
				break;
			case 0x1C: // GetResourceStatistics
				break;
			case 0x1D: // RecvMMsg
				break;
			case 0x1E: // SendMMsg
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Socket.Sf.IClient");
		}
	}
}

