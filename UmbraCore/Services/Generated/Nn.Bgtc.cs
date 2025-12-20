using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Bgtc;
public partial class IStateControlService : _IStateControlService_Base;
public abstract class _IStateControlService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			case 0x5: // Unknown5
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bgtc.IStateControlService");
		}
	}
}

public partial class ITaskService : _ITaskService_Base;
public abstract class _ITaskService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			case 0x5: // Unknown5
				break;
			case 0x6: // Unknown6
				break;
			case 0xB: // Unknown11
				break;
			case 0xC: // Unknown12
				break;
			case 0xD: // Unknown13
				break;
			case 0xE: // Unknown14
				break;
			case 0xF: // Unknown15
				break;
			case 0x65: // Unknown101
				break;
			case 0x66: // Unknown102
				break;
			case 0x67: // Unknown103
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bgtc.ITaskService");
		}
	}
}

