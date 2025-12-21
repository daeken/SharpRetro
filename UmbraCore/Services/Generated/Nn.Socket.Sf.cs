using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Socket.Sf;
public partial class IClient : _IClient_Base {
	public readonly string ServiceName;
	public IClient(string serviceName) => ServiceName = serviceName;
}
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
	protected virtual void _Close(uint socket, out int ret, out uint bsd_errno) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient._Close not implemented");
	protected virtual void DuplicateSocket(uint _0, ulong _1, out int ret, out uint bsd_errno) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.DuplicateSocket not implemented");
	protected virtual void GetResourceStatistics(uint _0, uint _1, ulong _2, ulong _3, out int ret, out uint bsd_errno, Span<byte> _6) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.GetResourceStatistics not implemented");
	protected virtual void RecvMMsg(uint _0, uint _1, uint _2, UInt128 _3, out int ret, out uint bsd_errno, Span<byte> _6) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.RecvMMsg not implemented");
	protected virtual void SendMMsg(uint _0, uint _1, uint _2, Span<byte> _3, Span<byte> _4, out int ret, out uint bsd_errno) =>
		throw new NotImplementedException("Nn.Socket.Sf.IClient.SendMMsg not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RegisterClient
				var _return = RegisterClient(*(Nn.Socket.BsdBufferConfig*) im.GetDataPointer(8), im.GetData<ulong>(8), im.GetData<ulong>(16), Kernel.Get<KObject>(im.GetCopy(0)), im.Pid);
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // StartMonitoring
				StartMonitoring(im.GetData<ulong>(8), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Socket
				Socket(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x3: { // SocketExempt
				SocketExempt(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x4: { // Open
				Open(im.GetData<uint>(8), im.GetSpan<byte>(0x21, 0), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x5: { // Select
				Select(im.GetData<uint>(8), *(Nn.Socket.Timeout*) im.GetDataPointer(16), im.GetSpan<byte>(0x21, 0), im.GetSpan<byte>(0x21, 1), im.GetSpan<byte>(0x21, 2), out var _0, out var _1, im.GetSpan<byte>(0x22, 0), im.GetSpan<byte>(0x22, 1), im.GetSpan<byte>(0x22, 2));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x6: { // Poll
				Poll(im.GetData<uint>(8), im.GetData<uint>(12), im.GetSpan<byte>(0x21, 0), out var _0, out var _1, im.GetSpan<byte>(0x22, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x7: { // Sysctl
				Sysctl(im.GetSpan<byte>(0x21, 0), im.GetSpan<byte>(0x21, 1), out var _0, out var _1, out var _2, im.GetSpan<byte>(0x22, 0));
				om.Initialize(0, 0, 12);
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				break;
			}
			case 0x8: { // Recv
				Recv(im.GetData<uint>(8), im.GetData<uint>(12), out var _0, out var _1, im.GetSpan<byte>(0x22, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x9: { // RecvFrom
				RecvFrom(im.GetData<uint>(8), im.GetData<uint>(12), out var _0, out var _1, out var _2, im.GetSpan<byte>(0x22, 0), im.GetSpan<Nn.Socket.Sockaddr>(0x22, 1));
				om.Initialize(0, 0, 12);
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				break;
			}
			case 0xA: { // Send
				Send(im.GetData<uint>(8), im.GetData<uint>(12), im.GetSpan<byte>(0x21, 0), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0xB: { // SendTo
				SendTo(im.GetData<uint>(8), im.GetData<uint>(12), im.GetSpan<byte>(0x21, 0), im.GetSpan<Nn.Socket.Sockaddr>(0x21, 1), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0xC: { // Accept
				Accept(im.GetData<uint>(8), out var _0, out var _1, out var _2, im.GetSpan<Nn.Socket.Sockaddr>(0x22, 0));
				om.Initialize(0, 0, 12);
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				break;
			}
			case 0xD: { // Bind
				Bind(im.GetData<uint>(8), im.GetSpan<Nn.Socket.Sockaddr>(0x21, 0), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0xE: { // Connect
				Connect(im.GetData<uint>(8), im.GetSpan<Nn.Socket.Sockaddr>(0x21, 0), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0xF: { // GetPeerName
				GetPeerName(im.GetData<uint>(8), out var _0, out var _1, out var _2, im.GetSpan<Nn.Socket.Sockaddr>(0x22, 0));
				om.Initialize(0, 0, 12);
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				break;
			}
			case 0x10: { // GetSockName
				GetSockName(im.GetData<uint>(8), out var _0, out var _1, out var _2, im.GetSpan<Nn.Socket.Sockaddr>(0x22, 0));
				om.Initialize(0, 0, 12);
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				break;
			}
			case 0x11: { // GetSockOpt
				GetSockOpt(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16), out var _0, out var _1, out var _2, im.GetSpan<byte>(0x22, 0));
				om.Initialize(0, 0, 12);
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				break;
			}
			case 0x12: { // Listen
				Listen(im.GetData<uint>(8), im.GetData<uint>(12), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x13: { // Ioctl
				Ioctl(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16), im.GetSpan<byte>(0x21, 0), im.GetSpan<byte>(0x21, 1), im.GetSpan<byte>(0x21, 2), im.GetSpan<byte>(0x21, 3), out var _0, out var _1, im.GetSpan<byte>(0x22, 0), im.GetSpan<byte>(0x22, 1), im.GetSpan<byte>(0x22, 2), im.GetSpan<byte>(0x22, 3));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x14: { // Fcntl
				Fcntl(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x15: { // SetSockOpt
				SetSockOpt(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16), im.GetSpan<byte>(0x21, 0), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x16: { // Shutdown
				Shutdown(im.GetData<uint>(8), im.GetData<uint>(12), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x17: { // ShutdownAllSockets
				ShutdownAllSockets(im.GetData<uint>(8), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x18: { // Write
				Write(im.GetData<uint>(8), im.GetSpan<byte>(0x21, 0), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x19: { // Read
				Read(im.GetData<uint>(8), out var _0, out var _1, im.GetSpan<sbyte>(0x22, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x1A: { // _Close
				_Close(im.GetData<uint>(8), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x1B: { // DuplicateSocket
				DuplicateSocket(im.GetData<uint>(8), im.GetData<ulong>(16), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x1C: { // GetResourceStatistics
				GetResourceStatistics(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<ulong>(16), im.Pid, out var _0, out var _1, im.GetSpan<byte>(0x22, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x1D: { // RecvMMsg
				RecvMMsg(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16), im.GetData<UInt128>(32), out var _0, out var _1, im.GetSpan<byte>(0x22, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x1E: { // SendMMsg
				SendMMsg(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16), im.GetSpan<byte>(0x21, 0), im.GetSpan<byte>(0x21, 1), out var _0, out var _1);
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Socket.Sf.IClient");
		}
	}
}

