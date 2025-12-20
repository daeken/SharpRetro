using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Account.Profile;
public partial class IProfile : _IProfile_Base;
public abstract class _IProfile_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Get
				break;
			case 0x1: // GetBase
				break;
			case 0xA: // GetImageSize
				break;
			case 0xB: // LoadImage
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Profile.IProfile");
		}
	}
}

public partial class IProfileEditor : _IProfileEditor_Base;
public abstract class _IProfileEditor_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Get
				break;
			case 0x1: // GetBase
				break;
			case 0xA: // GetImageSize
				break;
			case 0xB: // LoadImage
				break;
			case 0x64: // Store
				break;
			case 0x65: // StoreWithImage
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Account.Profile.IProfileEditor");
		}
	}
}

