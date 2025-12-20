using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Socket.Resolver;
public partial class IResolver : _IResolver_Base;
public abstract class _IResolver_Base : IpcInterface {
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

