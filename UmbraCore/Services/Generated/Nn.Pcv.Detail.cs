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
	protected virtual void _Finalize() =>
		Console.WriteLine("Stub hit for Nn.Pcv.Detail.IPcvService._Finalize");
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // SetPowerEnabled
				om.Initialize(0, 0, 0);
				SetPowerEnabled(im.GetData<byte>(8), im.GetData<uint>(12));
				break;
			}
			case 0x1: { // SetClockEnabled
				om.Initialize(0, 0, 0);
				SetClockEnabled(im.GetData<byte>(8), im.GetData<uint>(12));
				break;
			}
			case 0x2: { // SetClockRate
				om.Initialize(0, 0, 0);
				SetClockRate(im.GetData<uint>(8), im.GetData<uint>(12));
				break;
			}
			case 0x3: { // GetClockRate
				om.Initialize(0, 0, 4);
				var _return = GetClockRate(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x4: { // GetState
				om.Initialize(0, 0, 0);
				GetState(im.GetData<uint>(8));
				break;
			}
			case 0x5: { // GetPossibleClockRates
				om.Initialize(0, 0, 8);
				GetPossibleClockRates(im.GetData<uint>(8), im.GetData<uint>(12), out var _0, out var _1, im.GetSpan<uint>(0xA, 0));
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x6: { // SetMinVClockRate
				om.Initialize(0, 0, 0);
				SetMinVClockRate(im.GetData<uint>(8), im.GetData<uint>(12));
				break;
			}
			case 0x7: { // SetReset
				om.Initialize(0, 0, 0);
				SetReset(im.GetData<byte>(8), im.GetData<uint>(12));
				break;
			}
			case 0x8: { // SetVoltageEnabled
				om.Initialize(0, 0, 0);
				SetVoltageEnabled(im.GetData<byte>(8), im.GetData<uint>(12));
				break;
			}
			case 0x9: { // GetVoltageEnabled
				om.Initialize(0, 0, 1);
				var _return = GetVoltageEnabled(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // GetVoltageRange
				om.Initialize(0, 0, 12);
				GetVoltageRange(im.GetData<uint>(8), out var _0, out var _1, out var _2);
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				break;
			}
			case 0xB: { // SetVoltageValue
				om.Initialize(0, 0, 0);
				SetVoltageValue(im.GetData<uint>(8), im.GetData<uint>(12));
				break;
			}
			case 0xC: { // GetVoltageValue
				om.Initialize(0, 0, 4);
				var _return = GetVoltageValue(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			case 0xD: { // GetTemperatureThresholds
				om.Initialize(0, 0, 4);
				GetTemperatureThresholds(im.GetData<uint>(8), out var _0, im.GetSpan<byte>(0xA, 0));
				om.SetData(8, _0);
				break;
			}
			case 0xE: { // SetTemperature
				om.Initialize(0, 0, 0);
				SetTemperature(im.GetData<uint>(8));
				break;
			}
			case 0xF: { // Initialize
				om.Initialize(0, 0, 0);
				Initialize();
				break;
			}
			case 0x10: { // IsInitialized
				om.Initialize(0, 0, 1);
				var _return = IsInitialized();
				om.SetData(8, _return);
				break;
			}
			case 0x11: { // _Finalize
				om.Initialize(0, 0, 0);
				_Finalize();
				break;
			}
			case 0x12: { // PowerOn
				om.Initialize(0, 0, 0);
				PowerOn(im.GetData<uint>(8), im.GetData<uint>(12));
				break;
			}
			case 0x13: { // PowerOff
				om.Initialize(0, 0, 0);
				PowerOff(im.GetData<uint>(8));
				break;
			}
			case 0x14: { // ChangeVoltage
				om.Initialize(0, 0, 0);
				ChangeVoltage(im.GetData<uint>(8), im.GetData<uint>(12));
				break;
			}
			case 0x15: { // GetPowerClockInfoEvent
				om.Initialize(0, 1, 0);
				var _return = GetPowerClockInfoEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x16: { // GetOscillatorClock
				om.Initialize(0, 0, 4);
				var _return = GetOscillatorClock();
				om.SetData(8, _return);
				break;
			}
			case 0x17: { // GetDvfsTable
				om.Initialize(0, 0, 4);
				GetDvfsTable(im.GetData<uint>(8), im.GetData<uint>(12), out var _0, im.GetSpan<uint>(0xA, 0), im.GetSpan<uint>(0xA, 1));
				om.SetData(8, _0);
				break;
			}
			case 0x18: { // GetModuleStateTable
				om.Initialize(0, 0, 4);
				GetModuleStateTable(im.GetData<uint>(8), out var _0, im.GetSpan<byte>(0xA, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x19: { // GetPowerDomainStateTable
				om.Initialize(0, 0, 4);
				GetPowerDomainStateTable(im.GetData<uint>(8), out var _0, im.GetSpan<byte>(0xA, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x1A: { // GetFuseInfo
				om.Initialize(0, 0, 4);
				GetFuseInfo(im.GetData<uint>(8), out var _0, im.GetSpan<uint>(0xA, 0));
				om.SetData(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pcv.Detail.IPcvService");
		}
	}
}

