using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Socket.Resolver;
public partial class IResolver : _IResolver_Base {
	public readonly string ServiceName;
	public IResolver(string serviceName) => ServiceName = serviceName;
}
public abstract class _IResolver_Base : IpcInterface {
	protected virtual void SetDnsAddressesPrivate(uint _0, Span<byte> _1) =>
		"Stub hit for Nn.Socket.Resolver.IResolver.SetDnsAddressesPrivate".Log();
	protected virtual void GetDnsAddressPrivate(uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Socket.Resolver.IResolver.GetDnsAddressPrivate not implemented");
	protected virtual void GetHostByName(byte _0, uint _1, ulong _2, ulong _3, Span<byte> _4, out uint _5, out uint _6, out uint _7, Span<byte> _8) =>
		throw new NotImplementedException("Nn.Socket.Resolver.IResolver.GetHostByName not implemented");
	protected virtual void GetHostByAddr(uint _0, uint _1, uint _2, ulong _3, ulong _4, Span<byte> _5, out uint _6, out uint _7, out uint _8, Span<byte> _9) =>
		throw new NotImplementedException("Nn.Socket.Resolver.IResolver.GetHostByAddr not implemented");
	protected virtual void GetHostStringError(uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Socket.Resolver.IResolver.GetHostStringError not implemented");
	protected virtual void GetGaiStringError(uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Socket.Resolver.IResolver.GetGaiStringError not implemented");
	protected virtual void GetAddrInfo(bool enable_nsd_resolve, uint _1, ulong pid_placeholder, ulong _3, Span<sbyte> host, Span<sbyte> service, Span<byte> hints, out int ret, out uint bsd_errno, out uint packed_addrinfo_size, Span<byte> response) =>
		throw new NotImplementedException("Nn.Socket.Resolver.IResolver.GetAddrInfo not implemented");
	protected virtual void GetNameInfo(uint _0, uint _1, ulong _2, ulong _3, Span<byte> _4, out uint _5, out uint _6, Span<byte> _7, Span<byte> _8) =>
		throw new NotImplementedException("Nn.Socket.Resolver.IResolver.GetNameInfo not implemented");
	protected virtual uint RequestCancelHandle(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Socket.Resolver.IResolver.RequestCancelHandle not implemented");
	protected virtual void CancelSocketCall(uint _0, ulong _1, ulong _2) =>
		"Stub hit for Nn.Socket.Resolver.IResolver.CancelSocketCall".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // SetDnsAddressesPrivate
				SetDnsAddressesPrivate(im.GetData<uint>(8), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // GetDnsAddressPrivate
				GetDnsAddressPrivate(im.GetData<uint>(8), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GetHostByName
				GetHostByName(im.GetData<byte>(8), im.GetData<uint>(12), im.GetData<ulong>(16), im.Pid, im.GetSpan<byte>(0x5, 0), out var _0, out var _1, out var _2, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 12);
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				break;
			}
			case 0x3: { // GetHostByAddr
				GetHostByAddr(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<uint>(16), im.GetData<ulong>(24), im.Pid, im.GetSpan<byte>(0x5, 0), out var _0, out var _1, out var _2, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 12);
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				break;
			}
			case 0x4: { // GetHostStringError
				GetHostStringError(im.GetData<uint>(8), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // GetGaiStringError
				GetGaiStringError(im.GetData<uint>(8), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // GetAddrInfo
				GetAddrInfo(im.GetData<bool>(8), im.GetData<uint>(12), im.GetData<ulong>(16), im.Pid, im.GetSpan<sbyte>(0x5, 0), im.GetSpan<sbyte>(0x5, 1), im.GetSpan<byte>(0x5, 2), out var _0, out var _1, out var _2, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 12);
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				break;
			}
			case 0x7: { // GetNameInfo
				GetNameInfo(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<ulong>(16), im.Pid, im.GetSpan<byte>(0x5, 0), out var _0, out var _1, im.GetSpan<byte>(0x6, 0), im.GetSpan<byte>(0x6, 1));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x8: { // RequestCancelHandle
				var _return = RequestCancelHandle(im.GetData<ulong>(8), im.Pid);
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x9: { // CancelSocketCall
				CancelSocketCall(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Socket.Resolver.IResolver");
		}
	}
}

