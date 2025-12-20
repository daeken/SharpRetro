using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Psm;
public partial class IPsmServer : _IPsmServer_Base;
public abstract class _IPsmServer_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // GetBatteryChargePercentage
				break;
			case 0x1: // GetChargerType
				break;
			case 0x2: // EnableBatteryCharging
				break;
			case 0x3: // DisableBatteryCharging
				break;
			case 0x4: // IsBatteryChargingEnabled
				break;
			case 0x5: // AcquireControllerPowerSupply
				break;
			case 0x6: // ReleaseControllerPowerSupply
				break;
			case 0x7: // OpenSession
				break;
			case 0x8: // EnableEnoughPowerChargeEmulation
				break;
			case 0x9: // DisableEnoughPowerChargeEmulation
				break;
			case 0xA: // EnableFastBatteryCharging
				break;
			case 0xB: // DisableFastBatteryCharging
				break;
			case 0xC: // GetBatteryVoltageState
				break;
			case 0xD: // GetRawBatteryChargePercentage
				break;
			case 0xE: // IsEnoughPowerSupplied
				break;
			case 0xF: // GetBatteryAgePercentage
				break;
			case 0x10: // GetBatteryChargeInfoEvent
				break;
			case 0x11: // GetBatteryChargeInfoFields
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Psm.IPsmServer");
		}
	}
}

public partial class IPsmSession : _IPsmSession_Base;
public abstract class _IPsmSession_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // BindStateChangeEvent
				break;
			case 0x1: // UnbindStateChangeEvent
				break;
			case 0x2: // SetChargerTypeChangeEventEnabled
				break;
			case 0x3: // SetPowerSupplyChangeEventEnabled
				break;
			case 0x4: // SetBatteryVoltageStateChangeEventEnabled
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Psm.IPsmSession");
		}
	}
}

