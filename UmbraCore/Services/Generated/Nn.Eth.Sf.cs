using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Eth.Sf;
public partial class IEthInterface : _IEthInterface_Base;
public abstract class _IEthInterface_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Initialize
				break;
			case 0x1: // Cancel
				break;
			case 0x2: // GetResult
				break;
			case 0x3: // GetMediaList
				break;
			case 0x4: // SetMediaType
				break;
			case 0x5: // GetMediaType
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Eth.Sf.IEthInterface");
		}
	}
}

public partial class IEthInterfaceGroup : _IEthInterfaceGroup_Base;
public abstract class _IEthInterfaceGroup_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetReadableHandle
				break;
			case 0x1: // Cancel
				break;
			case 0x2: // GetResult
				break;
			case 0x3: // GetInterfaceList
				break;
			case 0x4: // GetInterfaceCount
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Eth.Sf.IEthInterfaceGroup");
		}
	}
}

