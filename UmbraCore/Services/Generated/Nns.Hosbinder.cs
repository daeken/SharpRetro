using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nns.Hosbinder;
public partial class IHOSBinderDriver : _IHOSBinderDriver_Base;
public abstract class _IHOSBinderDriver_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // TransactParcel
				break;
			case 0x1: // AdjustRefcount
				break;
			case 0x2: // GetNativeHandle
				break;
			case 0x3: // TransactParcelAuto
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nns.Hosbinder.IHOSBinderDriver");
		}
	}
}

