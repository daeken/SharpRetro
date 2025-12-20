using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Gpio;
public partial class IManager : _IManager_Base;
public abstract class _IManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // GetPadSession
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
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Gpio.IManager");
		}
	}
}

public partial class IPadSession : _IPadSession_Base;
public abstract class _IPadSession_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // SetDirection
				break;
			case 0x1: // GetDirection
				break;
			case 0x2: // SetInterruptMode
				break;
			case 0x3: // GetInterruptMode
				break;
			case 0x4: // SetInterruptEnable
				break;
			case 0x5: // GetInterruptEnable
				break;
			case 0x6: // GetInterruptStatus
				break;
			case 0x7: // ClearInterruptStatus
				break;
			case 0x8: // SetValue
				break;
			case 0x9: // GetValue
				break;
			case 0xA: // BindInterrupt
				break;
			case 0xB: // UnbindInterrupt
				break;
			case 0xC: // SetDebounceEnabled
				break;
			case 0xD: // GetDebounceEnabled
				break;
			case 0xE: // SetDebounceTime
				break;
			case 0xF: // GetDebounceTime
				break;
			case 0x10: // SetValueForSleepState
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Gpio.IPadSession");
		}
	}
}

