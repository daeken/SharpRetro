using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Gpio;
public partial class IManager : _IManager_Base;
public abstract class _IManager_Base : IpcInterface {
	protected virtual Nn.Gpio.IPadSession Unknown0(uint _0) =>
		throw new NotImplementedException("Nn.Gpio.IManager.Unknown0 not implemented");
	protected virtual Nn.Gpio.IPadSession GetPadSession(uint _0) =>
		throw new NotImplementedException("Nn.Gpio.IManager.GetPadSession not implemented");
	protected virtual Nn.Gpio.IPadSession Unknown2(uint _0) =>
		throw new NotImplementedException("Nn.Gpio.IManager.Unknown2 not implemented");
	protected virtual byte Unknown3(uint _0) =>
		throw new NotImplementedException("Nn.Gpio.IManager.Unknown3 not implemented");
	protected virtual void Unknown4(out byte[] _0) =>
		throw new NotImplementedException("Nn.Gpio.IManager.Unknown4 not implemented");
	protected virtual void Unknown5(byte _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Gpio.IManager.Unknown5");
	protected virtual void Unknown6(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Gpio.IManager.Unknown6");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(1, 0, 0);
				var _return = Unknown0(im.GetData<uint>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetPadSession
				om.Initialize(1, 0, 0);
				var _return = GetPadSession(im.GetData<uint>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(1, 0, 0);
				var _return = Unknown2(im.GetData<uint>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 1);
				var _return = Unknown3(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x4: { // Unknown4
				om.Initialize(0, 0, 16);
				Unknown4(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // Unknown5
				om.Initialize(0, 0, 0);
				Unknown5(im.GetData<byte>(8), im.GetData<uint>(12));
				break;
			}
			case 0x6: { // Unknown6
				om.Initialize(0, 0, 0);
				Unknown6(im.GetData<uint>(8));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Gpio.IManager");
		}
	}
}

public partial class IPadSession : _IPadSession_Base;
public abstract class _IPadSession_Base : IpcInterface {
	protected virtual void SetDirection(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Gpio.IPadSession.SetDirection");
	protected virtual uint GetDirection() =>
		throw new NotImplementedException("Nn.Gpio.IPadSession.GetDirection not implemented");
	protected virtual void SetInterruptMode(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Gpio.IPadSession.SetInterruptMode");
	protected virtual uint GetInterruptMode() =>
		throw new NotImplementedException("Nn.Gpio.IPadSession.GetInterruptMode not implemented");
	protected virtual void SetInterruptEnable(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Gpio.IPadSession.SetInterruptEnable");
	protected virtual byte GetInterruptEnable() =>
		throw new NotImplementedException("Nn.Gpio.IPadSession.GetInterruptEnable not implemented");
	protected virtual uint GetInterruptStatus() =>
		throw new NotImplementedException("Nn.Gpio.IPadSession.GetInterruptStatus not implemented");
	protected virtual void ClearInterruptStatus() =>
		Console.WriteLine("Stub hit for Nn.Gpio.IPadSession.ClearInterruptStatus");
	protected virtual void SetValue(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Gpio.IPadSession.SetValue");
	protected virtual uint GetValue() =>
		throw new NotImplementedException("Nn.Gpio.IPadSession.GetValue not implemented");
	protected virtual KObject BindInterrupt() =>
		throw new NotImplementedException("Nn.Gpio.IPadSession.BindInterrupt not implemented");
	protected virtual void UnbindInterrupt() =>
		Console.WriteLine("Stub hit for Nn.Gpio.IPadSession.UnbindInterrupt");
	protected virtual void SetDebounceEnabled(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Gpio.IPadSession.SetDebounceEnabled");
	protected virtual byte GetDebounceEnabled() =>
		throw new NotImplementedException("Nn.Gpio.IPadSession.GetDebounceEnabled not implemented");
	protected virtual void SetDebounceTime(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Gpio.IPadSession.SetDebounceTime");
	protected virtual uint GetDebounceTime() =>
		throw new NotImplementedException("Nn.Gpio.IPadSession.GetDebounceTime not implemented");
	protected virtual void SetValueForSleepState(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Gpio.IPadSession.SetValueForSleepState");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // SetDirection
				om.Initialize(0, 0, 0);
				SetDirection(im.GetData<uint>(8));
				break;
			}
			case 0x1: { // GetDirection
				om.Initialize(0, 0, 4);
				var _return = GetDirection();
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // SetInterruptMode
				om.Initialize(0, 0, 0);
				SetInterruptMode(im.GetData<uint>(8));
				break;
			}
			case 0x3: { // GetInterruptMode
				om.Initialize(0, 0, 4);
				var _return = GetInterruptMode();
				om.SetData(8, _return);
				break;
			}
			case 0x4: { // SetInterruptEnable
				om.Initialize(0, 0, 0);
				SetInterruptEnable(im.GetData<byte>(8));
				break;
			}
			case 0x5: { // GetInterruptEnable
				om.Initialize(0, 0, 1);
				var _return = GetInterruptEnable();
				om.SetData(8, _return);
				break;
			}
			case 0x6: { // GetInterruptStatus
				om.Initialize(0, 0, 4);
				var _return = GetInterruptStatus();
				om.SetData(8, _return);
				break;
			}
			case 0x7: { // ClearInterruptStatus
				om.Initialize(0, 0, 0);
				ClearInterruptStatus();
				break;
			}
			case 0x8: { // SetValue
				om.Initialize(0, 0, 0);
				SetValue(im.GetData<uint>(8));
				break;
			}
			case 0x9: { // GetValue
				om.Initialize(0, 0, 4);
				var _return = GetValue();
				om.SetData(8, _return);
				break;
			}
			case 0xA: { // BindInterrupt
				om.Initialize(0, 1, 0);
				var _return = BindInterrupt();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xB: { // UnbindInterrupt
				om.Initialize(0, 0, 0);
				UnbindInterrupt();
				break;
			}
			case 0xC: { // SetDebounceEnabled
				om.Initialize(0, 0, 0);
				SetDebounceEnabled(im.GetData<byte>(8));
				break;
			}
			case 0xD: { // GetDebounceEnabled
				om.Initialize(0, 0, 1);
				var _return = GetDebounceEnabled();
				om.SetData(8, _return);
				break;
			}
			case 0xE: { // SetDebounceTime
				om.Initialize(0, 0, 0);
				SetDebounceTime(im.GetData<uint>(8));
				break;
			}
			case 0xF: { // GetDebounceTime
				om.Initialize(0, 0, 4);
				var _return = GetDebounceTime();
				om.SetData(8, _return);
				break;
			}
			case 0x10: { // SetValueForSleepState
				om.Initialize(0, 0, 0);
				SetValueForSleepState(im.GetData<uint>(8));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Gpio.IPadSession");
		}
	}
}

