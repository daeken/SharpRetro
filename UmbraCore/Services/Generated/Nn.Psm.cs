using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Psm;
public partial class IPsmServer : _IPsmServer_Base;
public abstract class _IPsmServer_Base : IpcInterface {
	protected virtual void GetBatteryChargePercentage(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Psm.IPsmServer.GetBatteryChargePercentage not implemented");
	protected virtual void GetChargerType(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Psm.IPsmServer.GetChargerType not implemented");
	protected virtual void EnableBatteryCharging() =>
		Console.WriteLine("Stub hit for Nn.Psm.IPsmServer.EnableBatteryCharging");
	protected virtual void DisableBatteryCharging() =>
		Console.WriteLine("Stub hit for Nn.Psm.IPsmServer.DisableBatteryCharging");
	protected virtual void IsBatteryChargingEnabled(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Psm.IPsmServer.IsBatteryChargingEnabled not implemented");
	protected virtual void AcquireControllerPowerSupply() =>
		Console.WriteLine("Stub hit for Nn.Psm.IPsmServer.AcquireControllerPowerSupply");
	protected virtual void ReleaseControllerPowerSupply() =>
		Console.WriteLine("Stub hit for Nn.Psm.IPsmServer.ReleaseControllerPowerSupply");
	protected virtual Nn.Psm.IPsmSession OpenSession() =>
		throw new NotImplementedException("Nn.Psm.IPsmServer.OpenSession not implemented");
	protected virtual void EnableEnoughPowerChargeEmulation() =>
		Console.WriteLine("Stub hit for Nn.Psm.IPsmServer.EnableEnoughPowerChargeEmulation");
	protected virtual void DisableEnoughPowerChargeEmulation() =>
		Console.WriteLine("Stub hit for Nn.Psm.IPsmServer.DisableEnoughPowerChargeEmulation");
	protected virtual void EnableFastBatteryCharging() =>
		Console.WriteLine("Stub hit for Nn.Psm.IPsmServer.EnableFastBatteryCharging");
	protected virtual void DisableFastBatteryCharging() =>
		Console.WriteLine("Stub hit for Nn.Psm.IPsmServer.DisableFastBatteryCharging");
	protected virtual void GetBatteryVoltageState(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Psm.IPsmServer.GetBatteryVoltageState not implemented");
	protected virtual void GetRawBatteryChargePercentage(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Psm.IPsmServer.GetRawBatteryChargePercentage not implemented");
	protected virtual void IsEnoughPowerSupplied(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Psm.IPsmServer.IsEnoughPowerSupplied not implemented");
	protected virtual void GetBatteryAgePercentage(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Psm.IPsmServer.GetBatteryAgePercentage not implemented");
	protected virtual KObject GetBatteryChargeInfoEvent() =>
		throw new NotImplementedException("Nn.Psm.IPsmServer.GetBatteryChargeInfoEvent not implemented");
	protected virtual void GetBatteryChargeInfoFields(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Psm.IPsmServer.GetBatteryChargeInfoFields not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetBatteryChargePercentage
				break;
			}
			case 0x1: { // GetChargerType
				break;
			}
			case 0x2: { // EnableBatteryCharging
				break;
			}
			case 0x3: { // DisableBatteryCharging
				break;
			}
			case 0x4: { // IsBatteryChargingEnabled
				break;
			}
			case 0x5: { // AcquireControllerPowerSupply
				break;
			}
			case 0x6: { // ReleaseControllerPowerSupply
				break;
			}
			case 0x7: { // OpenSession
				break;
			}
			case 0x8: { // EnableEnoughPowerChargeEmulation
				break;
			}
			case 0x9: { // DisableEnoughPowerChargeEmulation
				break;
			}
			case 0xA: { // EnableFastBatteryCharging
				break;
			}
			case 0xB: { // DisableFastBatteryCharging
				break;
			}
			case 0xC: { // GetBatteryVoltageState
				break;
			}
			case 0xD: { // GetRawBatteryChargePercentage
				break;
			}
			case 0xE: { // IsEnoughPowerSupplied
				break;
			}
			case 0xF: { // GetBatteryAgePercentage
				break;
			}
			case 0x10: { // GetBatteryChargeInfoEvent
				break;
			}
			case 0x11: { // GetBatteryChargeInfoFields
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Psm.IPsmServer");
		}
	}
}

public partial class IPsmSession : _IPsmSession_Base;
public abstract class _IPsmSession_Base : IpcInterface {
	protected virtual KObject BindStateChangeEvent() =>
		throw new NotImplementedException("Nn.Psm.IPsmSession.BindStateChangeEvent not implemented");
	protected virtual void UnbindStateChangeEvent() =>
		Console.WriteLine("Stub hit for Nn.Psm.IPsmSession.UnbindStateChangeEvent");
	protected virtual void SetChargerTypeChangeEventEnabled(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Psm.IPsmSession.SetChargerTypeChangeEventEnabled");
	protected virtual void SetPowerSupplyChangeEventEnabled(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Psm.IPsmSession.SetPowerSupplyChangeEventEnabled");
	protected virtual void SetBatteryVoltageStateChangeEventEnabled(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Psm.IPsmSession.SetBatteryVoltageStateChangeEventEnabled");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // BindStateChangeEvent
				break;
			}
			case 0x1: { // UnbindStateChangeEvent
				break;
			}
			case 0x2: { // SetChargerTypeChangeEventEnabled
				break;
			}
			case 0x3: { // SetPowerSupplyChangeEventEnabled
				break;
			}
			case 0x4: { // SetBatteryVoltageStateChangeEventEnabled
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Psm.IPsmSession");
		}
	}
}

