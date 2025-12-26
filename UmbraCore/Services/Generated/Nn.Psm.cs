using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Psm;
public partial class IPsmServer : _IPsmServer_Base {
	public readonly string ServiceName;
	public IPsmServer(string serviceName) => ServiceName = serviceName;
}
public abstract class _IPsmServer_Base : IpcInterface {
	protected virtual void GetBatteryChargePercentage(out byte[] _0) =>
		throw new NotImplementedException("Nn.Psm.IPsmServer.GetBatteryChargePercentage not implemented");
	protected virtual void GetChargerType(out byte[] _0) =>
		throw new NotImplementedException("Nn.Psm.IPsmServer.GetChargerType not implemented");
	protected virtual void EnableBatteryCharging() =>
		"Stub hit for Nn.Psm.IPsmServer.EnableBatteryCharging".Log();
	protected virtual void DisableBatteryCharging() =>
		"Stub hit for Nn.Psm.IPsmServer.DisableBatteryCharging".Log();
	protected virtual void IsBatteryChargingEnabled(out byte[] _0) =>
		throw new NotImplementedException("Nn.Psm.IPsmServer.IsBatteryChargingEnabled not implemented");
	protected virtual void AcquireControllerPowerSupply() =>
		"Stub hit for Nn.Psm.IPsmServer.AcquireControllerPowerSupply".Log();
	protected virtual void ReleaseControllerPowerSupply() =>
		"Stub hit for Nn.Psm.IPsmServer.ReleaseControllerPowerSupply".Log();
	protected virtual Nn.Psm.IPsmSession OpenSession() =>
		throw new NotImplementedException("Nn.Psm.IPsmServer.OpenSession not implemented");
	protected virtual void EnableEnoughPowerChargeEmulation() =>
		"Stub hit for Nn.Psm.IPsmServer.EnableEnoughPowerChargeEmulation".Log();
	protected virtual void DisableEnoughPowerChargeEmulation() =>
		"Stub hit for Nn.Psm.IPsmServer.DisableEnoughPowerChargeEmulation".Log();
	protected virtual void EnableFastBatteryCharging() =>
		"Stub hit for Nn.Psm.IPsmServer.EnableFastBatteryCharging".Log();
	protected virtual void DisableFastBatteryCharging() =>
		"Stub hit for Nn.Psm.IPsmServer.DisableFastBatteryCharging".Log();
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
				GetBatteryChargePercentage(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // GetChargerType
				GetChargerType(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // EnableBatteryCharging
				EnableBatteryCharging();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // DisableBatteryCharging
				DisableBatteryCharging();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // IsBatteryChargingEnabled
				IsBatteryChargingEnabled(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // AcquireControllerPowerSupply
				AcquireControllerPowerSupply();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // ReleaseControllerPowerSupply
				ReleaseControllerPowerSupply();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // OpenSession
				var _return = OpenSession();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x8: { // EnableEnoughPowerChargeEmulation
				EnableEnoughPowerChargeEmulation();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x9: { // DisableEnoughPowerChargeEmulation
				DisableEnoughPowerChargeEmulation();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // EnableFastBatteryCharging
				EnableFastBatteryCharging();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // DisableFastBatteryCharging
				DisableFastBatteryCharging();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // GetBatteryVoltageState
				GetBatteryVoltageState(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD: { // GetRawBatteryChargePercentage
				GetRawBatteryChargePercentage(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0xE: { // IsEnoughPowerSupplied
				IsEnoughPowerSupplied(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0xF: { // GetBatteryAgePercentage
				GetBatteryAgePercentage(out var _0);
				om.Initialize(0, 0, 8);
				om.SetBytes(8, _0);
				break;
			}
			case 0x10: { // GetBatteryChargeInfoEvent
				var _return = GetBatteryChargeInfoEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x11: { // GetBatteryChargeInfoFields
				GetBatteryChargeInfoFields(out var _0);
				om.Initialize(0, 0, 64);
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
		"Stub hit for Nn.Psm.IPsmSession.UnbindStateChangeEvent".Log();
	protected virtual void SetChargerTypeChangeEventEnabled(byte[] _0) =>
		"Stub hit for Nn.Psm.IPsmSession.SetChargerTypeChangeEventEnabled".Log();
	protected virtual void SetPowerSupplyChangeEventEnabled(byte[] _0) =>
		"Stub hit for Nn.Psm.IPsmSession.SetPowerSupplyChangeEventEnabled".Log();
	protected virtual void SetBatteryVoltageStateChangeEventEnabled(byte[] _0) =>
		"Stub hit for Nn.Psm.IPsmSession.SetBatteryVoltageStateChangeEventEnabled".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // BindStateChangeEvent
				var _return = BindStateChangeEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // UnbindStateChangeEvent
				UnbindStateChangeEvent();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // SetChargerTypeChangeEventEnabled
				SetChargerTypeChangeEventEnabled(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // SetPowerSupplyChangeEventEnabled
				SetPowerSupplyChangeEventEnabled(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // SetBatteryVoltageStateChangeEventEnabled
				SetBatteryVoltageStateChangeEventEnabled(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Psm.IPsmSession");
		}
	}
}

