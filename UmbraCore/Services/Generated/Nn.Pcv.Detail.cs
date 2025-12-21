using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pcv.Detail;
public partial class IPcvService : _IPcvService_Base {
	public readonly string ServiceName;
	public IPcvService(string serviceName) => ServiceName = serviceName;
}
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
				SetPowerEnabled(im.GetData<byte>(8), im.GetData<uint>(12));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // SetClockEnabled
				SetClockEnabled(im.GetData<byte>(8), im.GetData<uint>(12));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // SetClockRate
				SetClockRate(im.GetData<uint>(8), im.GetData<uint>(12));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // GetClockRate
				var _return = GetClockRate(im.GetData<uint>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x4: { // GetState
				GetState(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // GetPossibleClockRates
				GetPossibleClockRates(im.GetData<uint>(8), im.GetData<uint>(12), out var _0, out var _1, im.GetSpan<uint>(0xA, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x6: { // SetMinVClockRate
				SetMinVClockRate(im.GetData<uint>(8), im.GetData<uint>(12));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // SetReset
				SetReset(im.GetData<byte>(8), im.GetData<uint>(12));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // SetVoltageEnabled
				SetVoltageEnabled(im.GetData<byte>(8), im.GetData<uint>(12));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // GetVoltageEnabled
				var _return = GetVoltageEnabled(im.GetData<uint>(8));
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // GetVoltageRange
				GetVoltageRange(im.GetData<uint>(8), out var _0, out var _1, out var _2);
				om.Initialize(0, 0, 12);
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				break;
			}
			case 0xB: { // SetVoltageValue
				SetVoltageValue(im.GetData<uint>(8), im.GetData<uint>(12));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // GetVoltageValue
				var _return = GetVoltageValue(im.GetData<uint>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0xD: { // GetTemperatureThresholds
				GetTemperatureThresholds(im.GetData<uint>(8), out var _0, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0xE: { // SetTemperature
				SetTemperature(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xF: { // Initialize
				Initialize();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // IsInitialized
				var _return = IsInitialized();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x11: { // _Finalize
				_Finalize();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12: { // PowerOn
				PowerOn(im.GetData<uint>(8), im.GetData<uint>(12));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x13: { // PowerOff
				PowerOff(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x14: { // ChangeVoltage
				ChangeVoltage(im.GetData<uint>(8), im.GetData<uint>(12));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x15: { // GetPowerClockInfoEvent
				var _return = GetPowerClockInfoEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x16: { // GetOscillatorClock
				var _return = GetOscillatorClock();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x17: { // GetDvfsTable
				GetDvfsTable(im.GetData<uint>(8), im.GetData<uint>(12), out var _0, im.GetSpan<uint>(0xA, 0), im.GetSpan<uint>(0xA, 1));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x18: { // GetModuleStateTable
				GetModuleStateTable(im.GetData<uint>(8), out var _0, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x19: { // GetPowerDomainStateTable
				GetPowerDomainStateTable(im.GetData<uint>(8), out var _0, im.GetSpan<byte>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x1A: { // GetFuseInfo
				GetFuseInfo(im.GetData<uint>(8), out var _0, im.GetSpan<uint>(0xA, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pcv.Detail.IPcvService");
		}
	}
}

