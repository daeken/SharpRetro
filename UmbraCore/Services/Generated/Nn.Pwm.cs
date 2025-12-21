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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // SetPeriod
				break;
			}
			case 0x1: { // GetPeriod
				break;
			}
			case 0x2: { // SetDuty
				break;
			}
			case 0x3: { // GetDuty
				break;
			}
			case 0x4: { // SetEnabled
				break;
			}
			case 0x5: { // GetEnabled
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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenSessionForDev
				break;
			}
			case 0x1: { // OpenSession
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pwm.IManager");
		}
	}
}

