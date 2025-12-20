using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Mmnv;
public partial class IRequest : _IRequest_Base;
public abstract class _IRequest_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // InitializeOld
				break;
			case 0x1: // FinalizeOld
				break;
			case 0x2: // SetAndWaitOld
				break;
			case 0x3: // GetOld
				break;
			case 0x4: // Initialize
				break;
			case 0x5: // Finalize
				break;
			case 0x6: // SetAndWait
				break;
			case 0x7: // Get
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Mmnv.IRequest");
		}
	}
}

