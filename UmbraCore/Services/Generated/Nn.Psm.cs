using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Psm;
public partial class IPsmServer : _IPsmServer_Base;
public abstract class _IPsmServer_Base : IpcInterface {
	protected virtual void GetBatteryChargePercentage(out byte[] _0) =>
		throw new NotImplementedException("Nn.Psm.IPsmServer.GetBatteryChargePercentage not implemented");
	protected virtual void GetChargerType(out byte[] _0) =>
		throw new NotImplementedException("Nn.Psm.IPsmServer.GetChargerType not implemented");
	protected virtual void EnableBatteryCharging() =>
		Console.WriteLine("Stub hit for Nn.Psm.IPsmServer.EnableBatteryCharging");
	protected virtual void DisableBatteryCharging() =>
		Console.WriteLine("Stub hit for Nn.Psm.IPsmServer.DisableBatteryCharging");
	protected virtual void IsBatteryChargingEnabled(out byte[] _0) =>
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
	protected virtual void GetBatteryVoltageState(out byte[] _0) =>
		throw new NotImplementedException("Nn.Psm.IPsmServer.GetBatteryVoltageState not implemented");
	protected virtual void GetRawBatteryChargePercentage(out byte[] _0) =>
		throw new NotImplementedException("Nn.Psm.IPsmServer.GetRawBatteryChargePercentage not implemented");
	protected virtual void IsEnoughPowerSupplied(out byte[] _0) =>
		throw new NotImplementedException("Nn.Psm.IPsmServer.IsEnoughPowerSupplied not implemented");
	protected virtual void GetBatteryAgePercentage(out byte[] _0) =>
		throw new NotImplementedException("Nn.Psm.IPsmServer.GetBatteryAgePercentage not implemented");
	protected virtual KObject GetBatteryChargeInfoEvent() =>
		throw new NotImplementedException("Nn.Psm.IPsmServer.GetBatteryChargeInfoEvent not implemented");
	protected virtual void GetBatteryChargeInfoFields(out byte[] _0) =>
		throw new NotImplementedException("Nn.Psm.IPsmServer.GetBatteryChargeInfoFields not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetBatteryChargePercentage
				om.Initialize(0, 0, 4);
				GetBatteryChargePercentage(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // GetChargerType
				om.Initialize(0, 0, 4);
				GetChargerType(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // EnableBatteryCharging
				om.Initialize(0, 0, 0);
				EnableBatteryCharging();
				break;
			}
			case 0x3: { // DisableBatteryCharging
				om.Initialize(0, 0, 0);
				DisableBatteryCharging();
				break;
			}
			case 0x4: { // IsBatteryChargingEnabled
				om.Initialize(0, 0, 1);
				IsBatteryChargingEnabled(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // AcquireControllerPowerSupply
				om.Initialize(0, 0, 0);
				AcquireControllerPowerSupply();
				break;
			}
			case 0x6: { // ReleaseControllerPowerSupply
				om.Initialize(0, 0, 0);
				ReleaseControllerPowerSupply();
				break;
			}
			case 0x7: { // OpenSession
				om.Initialize(1, 0, 0);
				var _return = OpenSession();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x8: { // EnableEnoughPowerChargeEmulation
				om.Initialize(0, 0, 0);
				EnableEnoughPowerChargeEmulation();
				break;
			}
			case 0x9: { // DisableEnoughPowerChargeEmulation
				om.Initialize(0, 0, 0);
				DisableEnoughPowerChargeEmulation();
				break;
			}
			case 0xA: { // EnableFastBatteryCharging
				om.Initialize(0, 0, 0);
				EnableFastBatteryCharging();
				break;
			}
			case 0xB: { // DisableFastBatteryCharging
				om.Initialize(0, 0, 0);
				DisableFastBatteryCharging();
				break;
			}
			case 0xC: { // GetBatteryVoltageState
				om.Initialize(0, 0, 4);
				GetBatteryVoltageState(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD: { // GetRawBatteryChargePercentage
				om.Initialize(0, 0, 8);
				GetRawBatteryChargePercentage(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xE: { // IsEnoughPowerSupplied
				om.Initialize(0, 0, 1);
				IsEnoughPowerSupplied(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xF: { // GetBatteryAgePercentage
				om.Initialize(0, 0, 8);
				GetBatteryAgePercentage(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x10: { // GetBatteryChargeInfoEvent
				om.Initialize(0, 1, 0);
				var _return = GetBatteryChargeInfoEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x11: { // GetBatteryChargeInfoFields
				om.Initialize(0, 0, 64);
				GetBatteryChargeInfoFields(out var _0);
				om.SetBytes(8, _0);
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
	protected virtual void SetChargerTypeChangeEventEnabled(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Psm.IPsmSession.SetChargerTypeChangeEventEnabled");
	protected virtual void SetPowerSupplyChangeEventEnabled(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Psm.IPsmSession.SetPowerSupplyChangeEventEnabled");
	protected virtual void SetBatteryVoltageStateChangeEventEnabled(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Psm.IPsmSession.SetBatteryVoltageStateChangeEventEnabled");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // BindStateChangeEvent
				om.Initialize(0, 1, 0);
				var _return = BindStateChangeEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // UnbindStateChangeEvent
				om.Initialize(0, 0, 0);
				UnbindStateChangeEvent();
				break;
			}
			case 0x2: { // SetChargerTypeChangeEventEnabled
				om.Initialize(0, 0, 0);
				SetChargerTypeChangeEventEnabled(im.GetBytes(8, 0x1));
				break;
			}
			case 0x3: { // SetPowerSupplyChangeEventEnabled
				om.Initialize(0, 0, 0);
				SetPowerSupplyChangeEventEnabled(im.GetBytes(8, 0x1));
				break;
			}
			case 0x4: { // SetBatteryVoltageStateChangeEventEnabled
				om.Initialize(0, 0, 0);
				SetBatteryVoltageStateChangeEventEnabled(im.GetBytes(8, 0x1));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Psm.IPsmSession");
		}
	}
}

