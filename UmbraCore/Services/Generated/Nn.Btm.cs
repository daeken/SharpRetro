using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Btm;
public partial class IBtm : _IBtm_Base;
public abstract class _IBtm_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // RegisterSystemEventForConnectedDeviceConditionImpl
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			case 0x5: // Unknown5
				break;
			case 0x6: // Unknown6
				break;
			case 0x7: // Unknown7
				break;
			case 0x8: // RegisterSystemEventForRegisteredDeviceInfoImpl
				break;
			case 0x9: // Unknown9
				break;
			case 0xA: // Unknown10
				break;
			case 0xB: // Unknown11
				break;
			case 0xC: // Unknown12
				break;
			case 0xD: // Unknown13
				break;
			case 0xE: // EnableRadioImpl
				break;
			case 0xF: // DisableRadioImpl
				break;
			case 0x10: // Unknown16
				break;
			case 0x11: // Unknown17
				break;
			case 0x12: // Unknown18
				break;
			case 0x13: // Unknown19
				break;
			case 0x14: // Unknown20
				break;
			case 0x15: // Unknown21
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Btm.IBtm");
		}
	}
}

public partial class IBtmDebug : _IBtmDebug_Base;
public abstract class _IBtmDebug_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // RegisterSystemEventForDiscoveryImpl
				break;
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
			case 0x7: // Unknown7
				break;
			case 0x8: // Unknown8
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Btm.IBtmDebug");
		}
	}
}

public partial class IBtmSystem : _IBtmSystem_Base;
public abstract class _IBtmSystem_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetCoreImpl
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Btm.IBtmSystem");
		}
	}
}

public partial class IBtmSystemCore : _IBtmSystemCore_Base;
public abstract class _IBtmSystemCore_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // StartGamepadPairingImpl
				break;
			case 0x1: // CancelGamepadPairingImpl
				break;
			case 0x2: // ClearGamepadPairingDatabaseImpl
				break;
			case 0x3: // GetPairedGamepadCountImpl
				break;
			case 0x4: // EnableRadioImpl
				break;
			case 0x5: // DisableRadioImpl
				break;
			case 0x6: // GetRadioOnOffImpl
				break;
			case 0x7: // AcquireRadioEventImpl
				break;
			case 0x8: // AcquireGamepadPairingEventImpl
				break;
			case 0x9: // IsGamepadPairingStartedImpl
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Btm.IBtmSystemCore");
		}
	}
}

