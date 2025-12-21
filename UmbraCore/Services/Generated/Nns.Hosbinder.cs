using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nns.Hosbinder;
public partial class IHOSBinderDriver : _IHOSBinderDriver_Base;
public abstract class _IHOSBinderDriver_Base : IpcInterface {
	protected virtual void TransactParcel() =>
		Console.WriteLine("Stub hit for Nns.Hosbinder.IHOSBinderDriver.TransactParcel");
	protected virtual void AdjustRefcount() =>
		Console.WriteLine("Stub hit for Nns.Hosbinder.IHOSBinderDriver.AdjustRefcount");
	protected virtual void GetNativeHandle() =>
		Console.WriteLine("Stub hit for Nns.Hosbinder.IHOSBinderDriver.GetNativeHandle");
	protected virtual void TransactParcelAuto() =>
		Console.WriteLine("Stub hit for Nns.Hosbinder.IHOSBinderDriver.TransactParcelAuto");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // TransactParcel
				om.Initialize(0, 0, 0);
				TransactParcel();
				break;
			}
			case 0x1: { // AdjustRefcount
				om.Initialize(0, 0, 0);
				AdjustRefcount();
				break;
			}
			case 0x2: { // GetNativeHandle
				om.Initialize(0, 0, 0);
				GetNativeHandle();
				break;
			}
			case 0x3: { // TransactParcelAuto
				om.Initialize(0, 0, 0);
				TransactParcelAuto();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nns.Hosbinder.IHOSBinderDriver");
		}
	}
}

