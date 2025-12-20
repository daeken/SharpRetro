using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pl.Detail;
public partial class ISharedFontManager : _ISharedFontManager_Base;
public abstract class _ISharedFontManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // RequestLoad
				break;
			case 0x1: // GetLoadState
				break;
			case 0x2: // GetSize
				break;
			case 0x3: // GetSharedMemoryAddressOffset
				break;
			case 0x4: // GetSharedMemoryNativeHandle
				break;
			case 0x5: // GetSharedFontInOrderOfPriority
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pl.Detail.ISharedFontManager");
		}
	}
}

