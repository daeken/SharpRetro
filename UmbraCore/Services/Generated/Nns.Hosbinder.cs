using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nns.Hosbinder;
public partial class IHOSBinderDriver : _IHOSBinderDriver_Base {
	public readonly string ServiceName;
	public IHOSBinderDriver(string serviceName) => ServiceName = serviceName;
}
public abstract class _IHOSBinderDriver_Base : IpcInterface {
	protected virtual void TransactParcel() =>
		"Stub hit for Nns.Hosbinder.IHOSBinderDriver.TransactParcel".Log();
	protected virtual void AdjustRefcount() =>
		"Stub hit for Nns.Hosbinder.IHOSBinderDriver.AdjustRefcount".Log();
	protected virtual void GetNativeHandle() =>
		"Stub hit for Nns.Hosbinder.IHOSBinderDriver.GetNativeHandle".Log();
	protected virtual void TransactParcelAuto() =>
		"Stub hit for Nns.Hosbinder.IHOSBinderDriver.TransactParcelAuto".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // TransactParcel
				TransactParcel();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // AdjustRefcount
				AdjustRefcount();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // GetNativeHandle
				GetNativeHandle();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // TransactParcelAuto
				TransactParcelAuto();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nns.Hosbinder.IHOSBinderDriver");
		}
	}
}

