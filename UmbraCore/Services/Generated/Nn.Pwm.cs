using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pwm;
public partial class IChannelSession : _IChannelSession_Base;
public abstract class _IChannelSession_Base : IpcInterface {
	protected virtual void SetPeriod(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Pwm.IChannelSession.SetPeriod");
	protected virtual ulong GetPeriod() =>
		throw new NotImplementedException("Nn.Pwm.IChannelSession.GetPeriod not implemented");
	protected virtual void SetDuty(uint _0) =>
		Console.WriteLine("Stub hit for Nn.Pwm.IChannelSession.SetDuty");
	protected virtual uint GetDuty() =>
		throw new NotImplementedException("Nn.Pwm.IChannelSession.GetDuty not implemented");
	protected virtual void SetEnabled(byte _0) =>
		Console.WriteLine("Stub hit for Nn.Pwm.IChannelSession.SetEnabled");
	protected virtual byte GetEnabled() =>
		throw new NotImplementedException("Nn.Pwm.IChannelSession.GetEnabled not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // SetPeriod
				om.Initialize(0, 0, 0);
				SetPeriod(im.GetData<ulong>(8));
				break;
			}
			case 0x1: { // GetPeriod
				om.Initialize(0, 0, 8);
				var _return = GetPeriod();
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // SetDuty
				om.Initialize(0, 0, 0);
				SetDuty(im.GetData<uint>(8));
				break;
			}
			case 0x3: { // GetDuty
				om.Initialize(0, 0, 4);
				var _return = GetDuty();
				om.SetData(8, _return);
				break;
			}
			case 0x4: { // SetEnabled
				om.Initialize(0, 0, 0);
				SetEnabled(im.GetData<byte>(8));
				break;
			}
			case 0x5: { // GetEnabled
				om.Initialize(0, 0, 1);
				var _return = GetEnabled();
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pwm.IChannelSession");
		}
	}
}

public partial class IManager : _IManager_Base;
public abstract class _IManager_Base : IpcInterface {
	protected virtual Nn.Pwm.IChannelSession OpenSessionForDev(uint _0) =>
		throw new NotImplementedException("Nn.Pwm.IManager.OpenSessionForDev not implemented");
	protected virtual Nn.Pwm.IChannelSession OpenSession(uint _0) =>
		throw new NotImplementedException("Nn.Pwm.IManager.OpenSession not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenSessionForDev
				om.Initialize(1, 0, 0);
				var _return = OpenSessionForDev(im.GetData<uint>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // OpenSession
				om.Initialize(1, 0, 0);
				var _return = OpenSession(im.GetData<uint>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pwm.IManager");
		}
	}
}

