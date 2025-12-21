using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pcv.Detail;
public partial class IPcvService : _IPcvService_Base;
public abstract class _IPcvService_Base : IpcInterface {
	protected virtual void SetPowerEnabled(byte _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Pcv.Detail.IPcvService.SetPowerEnabled");
	protected virtual void SetClockEnabled(byte _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Pcv.Detail.IPcvService.SetClockEnabled");
	protected virtual void SetClockRate(uint _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Pcv.Detail.IPcvService.SetClockRate");
	protected virtual uint GetClockRate(uint _0) =>
		throw new NotImplementedException("Nn.Pcv.Detail.IPcvService.GetClockRate not implemented");
	protected virtual void GetState(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Pcv.Detail.IPcvService.GetState");
	protected virtual void GetPossibleClockRates(uint _0, uint _1, out uint _2, out uint _3, Span<uint> _4) =>
		throw new NotImplementedException("Nn.Pcv.Detail.IPcvService.GetPossibleClockRates not implemented");
	protected virtual void SetMinVClockRate(uint _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Pcv.Detail.IPcvService.SetMinVClockRate");
	protected virtual void SetReset(byte _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Pcv.Detail.IPcvService.SetReset");
	protected virtual void SetVoltageEnabled(byte _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Pcv.Detail.IPcvService.SetVoltageEnabled");
	protected virtual byte GetVoltageEnabled(uint _0) =>
		throw new NotImplementedException("Nn.Pcv.Detail.IPcvService.GetVoltageEnabled not implemented");
	protected virtual void GetVoltageRange(uint _0, out uint _1, out uint _2, out uint _3) =>
		throw new NotImplementedException("Nn.Pcv.Detail.IPcvService.GetVoltageRange not implemented");
	protected virtual void SetVoltageValue(uint _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Pcv.Detail.IPcvService.SetVoltageValue");
	protected virtual uint GetVoltageValue(uint _0) =>
		throw new NotImplementedException("Nn.Pcv.Detail.IPcvService.GetVoltageValue not implemented");
	protected virtual void GetTemperatureThresholds(uint _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Pcv.Detail.IPcvService.GetTemperatureThresholds not implemented");
	protected virtual void SetTemperature(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Pcv.Detail.IPcvService.SetTemperature");
	protected virtual void Initialize() =>
		Console.WriteLine("Stub hit for Nn.Pcv.Detail.IPcvService.Initialize");
	protected virtual byte IsInitialized() =>
		throw new NotImplementedException("Nn.Pcv.Detail.IPcvService.IsInitialized not implemented");
	protected virtual void Finalize() =>
		Console.WriteLine("Stub hit for Nn.Pcv.Detail.IPcvService.Finalize");
	protected virtual void PowerOn(uint _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Pcv.Detail.IPcvService.PowerOn");
	protected virtual void PowerOff(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Pcv.Detail.IPcvService.PowerOff");
	protected virtual void ChangeVoltage(uint _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Pcv.Detail.IPcvService.ChangeVoltage");
	protected virtual KObject GetPowerClockInfoEvent() =>
		throw new NotImplementedException("Nn.Pcv.Detail.IPcvService.GetPowerClockInfoEvent not implemented");
	protected virtual uint GetOscillatorClock() =>
		throw new NotImplementedException("Nn.Pcv.Detail.IPcvService.GetOscillatorClock not implemented");
	protected virtual void GetDvfsTable(uint _0, uint _1, out uint _2, Span<uint> _3, Span<uint> _4) =>
		throw new NotImplementedException("Nn.Pcv.Detail.IPcvService.GetDvfsTable not implemented");
	protected virtual void GetModuleStateTable(uint _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Pcv.Detail.IPcvService.GetModuleStateTable not implemented");
	protected virtual void GetPowerDomainStateTable(uint _0, out uint _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Pcv.Detail.IPcvService.GetPowerDomainStateTable not implemented");
	protected virtual void GetFuseInfo(uint _0, out uint _1, Span<uint> _2) =>
		throw new NotImplementedException("Nn.Pcv.Detail.IPcvService.GetFuseInfo not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // SetPowerEnabled
				break;
			}
			case 0x1: { // SetClockEnabled
				break;
			}
			case 0x2: { // SetClockRate
				break;
			}
			case 0x3: { // GetClockRate
				break;
			}
			case 0x4: { // GetState
				break;
			}
			case 0x5: { // GetPossibleClockRates
				break;
			}
			case 0x6: { // SetMinVClockRate
				break;
			}
			case 0x7: { // SetReset
				break;
			}
			case 0x8: { // SetVoltageEnabled
				break;
			}
			case 0x9: { // GetVoltageEnabled
				break;
			}
			case 0xA: { // GetVoltageRange
				break;
			}
			case 0xB: { // SetVoltageValue
				break;
			}
			case 0xC: { // GetVoltageValue
				break;
			}
			case 0xD: { // GetTemperatureThresholds
				break;
			}
			case 0xE: { // SetTemperature
				break;
			}
			case 0xF: { // Initialize
				break;
			}
			case 0x10: { // IsInitialized
				break;
			}
			case 0x11: { // Finalize
				break;
			}
			case 0x12: { // PowerOn
				break;
			}
			case 0x13: { // PowerOff
				break;
			}
			case 0x14: { // ChangeVoltage
				break;
			}
			case 0x15: { // GetPowerClockInfoEvent
				break;
			}
			case 0x16: { // GetOscillatorClock
				break;
			}
			case 0x17: { // GetDvfsTable
				break;
			}
			case 0x18: { // GetModuleStateTable
				break;
			}
			case 0x19: { // GetPowerDomainStateTable
				break;
			}
			case 0x1A: { // GetFuseInfo
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pcv.Detail.IPcvService");
		}
	}
}

