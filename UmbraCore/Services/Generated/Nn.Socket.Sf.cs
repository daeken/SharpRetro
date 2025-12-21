using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Socket.Sf;
public partial class IClient : _IClient_Base;
public abstract class _IClient_Base : IpcInterface {
	protected virtual uint RegisterClient(Nn.Socket.BsdBufferConfig config, ulong pid, ulong transferMemorySize, KObject _3, ulong _4) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.RegisterClient not implemented");
	protected virtual void StartMonitoring(ulong pid, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Socket.Sf.IClient.StartMonitoring");
	protected virtual void Socket(uint domain, uint type, uint protocol, out int ret, out uint bsd_errno) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.Socket not implemented");
	protected virtual void SocketExempt(uint _0, uint _1, uint _2, out int ret, out uint bsd_errno) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.SocketExempt not implemented");
	protected virtual void Open(uint _0, Span<byte> _1, out int ret, out uint bsd_errno) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.Open not implemented");
	protected virtual void Select(uint nfds, Nn.Socket.Timeout timeout, Span<byte> readfds_in, Span<byte> writefds_in, Span<byte> errorfds_in, out int ret, out uint bsd_errno, Span<byte> readfds_out, Span<byte> writefds_out, Span<byte> errorfds_out) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.Select not implemented");
	protected virtual void Poll(uint _0, uint _1, Span<byte> _2, out int ret, out uint bsd_errno, Span<byte> _5) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.Poll not implemented");
	protected virtual void Sysctl(Span<byte> _0, Span<byte> _1, out int ret, out uint bsd_errno, out uint _4, Span<byte> _5) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.Sysctl not implemented");
	protected virtual void Recv(uint socket, uint flags, out int ret, out uint bsd_errno, Span<byte> message) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.Recv not implemented");
	protected virtual void RecvFrom(uint sock, uint flags, out int ret, out uint bsd_errno, out uint addrlen, Span<byte> message, Span<Nn.Socket.Sockaddr> _6) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.RecvFrom not implemented");
	protected virtual void Send(uint socket, uint flags, Span<byte> _2, out int ret, out uint bsd_errno) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.Send not implemented");
	protected virtual void SendTo(uint socket, uint flags, Span<byte> _2, Span<Nn.Socket.Sockaddr> _3, out int ret, out uint bsd_errno) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.SendTo not implemented");
	protected virtual void Accept(uint socket, out int ret, out uint bsd_errno, out uint addrlen, Span<Nn.Socket.Sockaddr> addr) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.Accept not implemented");
	protected virtual void Bind(uint socket, Span<Nn.Socket.Sockaddr> _1, out int ret, out uint bsd_errno) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.Bind not implemented");
	protected virtual void Connect(uint socket, Span<Nn.Socket.Sockaddr> _1, out int ret, out uint bsd_errno) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.Connect not implemented");
	protected virtual void GetPeerName(uint socket, out int ret, out uint bsd_errno, out uint addrlen, Span<Nn.Socket.Sockaddr> addr) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.GetPeerName not implemented");
	protected virtual void GetSockName(uint socket, out int ret, out uint bsd_errno, out uint addrlen, Span<Nn.Socket.Sockaddr> addr) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.GetSockName not implemented");
	protected virtual void GetSockOpt(uint _0, uint _1, uint _2, out int ret, out uint bsd_errno, out uint _5, Span<byte> _6) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.GetSockOpt not implemented");
	protected virtual void Listen(uint socket, uint backlog, out int ret, out uint bsd_errno) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.Listen not implemented");
	protected virtual void Ioctl(uint _0, uint _1, uint _2, Span<byte> _3, Span<byte> _4, Span<byte> _5, Span<byte> _6, out int ret, out uint bsd_errno, Span<byte> _9, Span<byte> _10, Span<byte> _11, Span<byte> _12) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.Ioctl not implemented");
	protected virtual void Fcntl(uint _0, uint _1, uint _2, out int ret, out uint bsd_errno) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.Fcntl not implemented");
	protected virtual void SetSockOpt(uint socket, uint level, uint option_name, Span<byte> _3, out int ret, out uint bsd_errno) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.SetSockOpt not implemented");
	protected virtual void Shutdown(uint socket, uint how, out int ret, out uint bsd_errno) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.Shutdown not implemented");
	protected virtual void ShutdownAllSockets(uint how, out int ret, out uint bsd_errno) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.ShutdownAllSockets not implemented");
	protected virtual void Write(uint socket, Span<byte> message, out int ret, out uint bsd_errno) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.Write not implemented");
	protected virtual void Read(uint socket, out int ret, out uint bsd_errno, Span<sbyte> message) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.Read not implemented");
	protected virtual void Close(uint socket, out int ret, out uint bsd_errno) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.Close not implemented");
	protected virtual void DuplicateSocket(uint _0, ulong _1, out int ret, out uint bsd_errno) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.DuplicateSocket not implemented");
	protected virtual void GetResourceStatistics(uint _0, uint _1, ulong _2, ulong _3, out int ret, out uint bsd_errno, Span<byte> _6) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.GetResourceStatistics not implemented");
	protected virtual void RecvMMsg(uint _0, uint _1, uint _2, UInt128 _3, out int ret, out uint bsd_errno, Span<byte> _6) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.RecvMMsg not implemented");
	protected virtual void SendMMsg(uint _0, uint _1, uint _2, Span<byte> _3, Span<byte> _4, out int ret, out uint bsd_errno) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.SendMMsg not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RegisterClient
				break;
			}
			case 0x1: { // StartMonitoring
				break;
			}
			case 0x2: { // Socket
				break;
			}
			case 0x3: { // SocketExempt
				break;
			}
			case 0x4: { // Open
				break;
			}
			case 0x5: { // Select
				break;
			}
			case 0x6: { // Poll
				break;
			}
			case 0x7: { // Sysctl
				break;
			}
			case 0x8: { // Recv
				break;
			}
			case 0x9: { // RecvFrom
				break;
			}
			case 0xA: { // Send
				break;
			}
			case 0xB: { // SendTo
				break;
			}
			case 0xC: { // Accept
				break;
			}
			case 0xD: { // Bind
				break;
			}
			case 0xE: { // Connect
				break;
			}
			case 0xF: { // GetPeerName
				break;
			}
			case 0x10: { // GetSockName
				break;
			}
			case 0x11: { // GetSockOpt
				break;
			}
			case 0x12: { // Listen
				break;
			}
			case 0x13: { // Ioctl
				break;
			}
			case 0x14: { // Fcntl
				break;
			}
			case 0x15: { // SetSockOpt
				break;
			}
			case 0x16: { // Shutdown
				break;
			}
			case 0x17: { // ShutdownAllSockets
				break;
			}
			case 0x18: { // Write
				break;
			}
			case 0x19: { // Read
				break;
			}
			case 0x1A: { // Close
				break;
			}
			case 0x1B: { // DuplicateSocket
				break;
			}
			case 0x1C: { // GetResourceStatistics
				break;
			}
			case 0x1D: { // RecvMMsg
				break;
			}
			case 0x1E: { // SendMMsg
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Socket.Sf.IClient");
		}
	}
}

