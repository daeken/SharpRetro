using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pcv;
public partial class IArbitrationManager : _IArbitrationManager_Base;
public abstract class _IArbitrationManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // ReleaseControl
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pcv.IArbitrationManager");
		}
	}
}

public partial class IImmediateManager : _IImmediateManager_Base;
public abstract class _IImmediateManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // SetClockRate
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pcv.IImmediateManager");
		}
	}
}

