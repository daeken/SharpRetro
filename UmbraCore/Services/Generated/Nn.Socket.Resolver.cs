using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Socket.Resolver;
public partial class IResolver : _IResolver_Base;
public abstract class _IResolver_Base : IpcInterface {
	protected virtual void SetDnsAddressesPrivate(uint _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.Socket.Resolver.IResolver.SetDnsAddressesPrivate");
	protected virtual void GetDnsAddressPrivate(uint _0) =>
		throw new NotImplementedException("Nn.Socket.Resolver.IResolver.GetDnsAddressPrivate not implemented");
	protected virtual void GetHostByName(byte _0, uint _1, ulong _2, ulong _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Socket.Resolver.IResolver.GetHostByName not implemented");
	protected virtual void GetHostByAddr(uint _0, uint _1, uint _2, ulong _3, ulong _4, Span<byte> _5) =>
		throw new NotImplementedException("Nn.Socket.Resolver.IResolver.GetHostByAddr not implemented");
	protected virtual void GetHostStringError(uint _0) =>
		throw new NotImplementedException("Nn.Socket.Resolver.IResolver.GetHostStringError not implemented");
	protected virtual void GetGaiStringError(uint _0) =>
		throw new NotImplementedException("Nn.Socket.Resolver.IResolver.GetGaiStringError not implemented");
	protected virtual void GetAddrInfo(bool enable_nsd_resolve, uint _1, ulong pid_placeholder, ulong _3, Span<sbyte> host, Span<sbyte> service, Span<byte> hints) =>
		throw new NotImplementedException("Nn.Socket.Resolver.IResolver.GetAddrInfo not implemented");
	protected virtual void GetNameInfo(uint _0, uint _1, ulong _2, ulong _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Socket.Resolver.IResolver.GetNameInfo not implemented");
	protected virtual uint RequestCancelHandle(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Socket.Resolver.IResolver.RequestCancelHandle not implemented");
	protected virtual void CancelSocketCall(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Socket.Resolver.IResolver.CancelSocketCall");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // SetDnsAddressesPrivate
				break;
			case 0x1: // GetDnsAddressPrivate
				break;
			case 0x2: // GetHostByName
				break;
			case 0x3: // GetHostByAddr
				break;
			case 0x4: // GetHostStringError
				break;
			case 0x5: // GetGaiStringError
				break;
			case 0x6: // GetAddrInfo
				break;
			case 0x7: // GetNameInfo
				break;
			case 0x8: // RequestCancelHandle
				break;
			case 0x9: // CancelSocketCall
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Socket.Resolver.IResolver");
		}
	}
}

