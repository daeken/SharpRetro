using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pcv.Detail;
public partial class IPcvService : _IPcvService_Base;
public abstract class _IPcvService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // SetPowerEnabled
				break;
			case 0x1: // SetClockEnabled
				break;
			case 0x2: // SetClockRate
				break;
			case 0x3: // GetClockRate
				break;
			case 0x4: // GetState
				break;
			case 0x5: // GetPossibleClockRates
				break;
			case 0x6: // SetMinVClockRate
				break;
			case 0x7: // SetReset
				break;
			case 0x8: // SetVoltageEnabled
				break;
			case 0x9: // GetVoltageEnabled
				break;
			case 0xA: // GetVoltageRange
				break;
			case 0xB: // SetVoltageValue
				break;
			case 0xC: // GetVoltageValue
				break;
			case 0xD: // GetTemperatureThresholds
				break;
			case 0xE: // SetTemperature
				break;
			case 0xF: // Initialize
				break;
			case 0x10: // IsInitialized
				break;
			case 0x11: // Finalize
				break;
			case 0x12: // PowerOn
				break;
			case 0x13: // PowerOff
				break;
			case 0x14: // ChangeVoltage
				break;
			case 0x15: // GetPowerClockInfoEvent
				break;
			case 0x16: // GetOscillatorClock
				break;
			case 0x17: // GetDvfsTable
				break;
			case 0x18: // GetModuleStateTable
				break;
			case 0x19: // GetPowerDomainStateTable
				break;
			case 0x1A: // GetFuseInfo
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pcv.Detail.IPcvService");
		}
	}
}

