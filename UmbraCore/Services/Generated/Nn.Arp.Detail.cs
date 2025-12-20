using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Arp.Detail;
public partial class IReader : _IReader_Base;
public abstract class _IReader_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetApplicationLaunchProperty
				break;
			case 0x1: // GetApplicationLaunchPropertyWithApplicationId
				break;
			case 0x2: // GetApplicationControlProperty
				break;
			case 0x3: // GetApplicationControlPropertyWithApplicationId
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Arp.Detail.IReader");
		}
	}
}

public partial class IRegistrar : _IRegistrar_Base;
public abstract class _IRegistrar_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Issue
				break;
			case 0x1: // SetApplicationLaunchProperty
				break;
			case 0x2: // SetApplicationControlProperty
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Arp.Detail.IRegistrar");
		}
	}
}

public partial class IWriter : _IWriter_Base;
public abstract class _IWriter_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // AcquireRegistrar
				break;
			case 0x1: // DeleteProperties
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Arp.Detail.IWriter");
		}
	}
}

