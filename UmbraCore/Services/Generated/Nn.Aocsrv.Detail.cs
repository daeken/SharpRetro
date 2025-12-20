using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Aocsrv.Detail;
public partial class IAddOnContentManager : _IAddOnContentManager_Base;
public abstract class _IAddOnContentManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // CountAddOnContentByApplicationId
				break;
			case 0x1: // ListAddOnContentByApplicationId
				break;
			case 0x2: // CountAddOnContent
				break;
			case 0x3: // ListAddOnContent
				break;
			case 0x4: // GetAddOnContentBaseIdByApplicationId
				break;
			case 0x5: // GetAddOnContentBaseId
				break;
			case 0x6: // PrepareAddOnContentByApplicationId
				break;
			case 0x7: // PrepareAddOnContent
				break;
			case 0x8: // GetAddOnContentListChangedEvent
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Aocsrv.Detail.IAddOnContentManager");
		}
	}
}

