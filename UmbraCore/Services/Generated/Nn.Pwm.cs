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
				SetPeriod(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // GetPeriod
				var _return = GetPeriod();
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // SetDuty
				SetDuty(im.GetData<uint>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // GetDuty
				var _return = GetDuty();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x4: { // SetEnabled
				SetEnabled(im.GetData<byte>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // GetEnabled
				var _return = GetEnabled();
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pwm.IChannelSession");
		}
	}
}

public partial class IManager : _IManager_Base {
	public readonly string ServiceName;
	public IManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IManager_Base : IpcInterface {
	protected virtual Nn.Pwm.IChannelSession OpenSessionForDev(uint _0) =>
		throw new NotImplementedException("Nn.Pwm.IManager.OpenSessionForDev not implemented");
	protected virtual Nn.Pwm.IChannelSession OpenSession(uint _0) =>
		throw new NotImplementedException("Nn.Pwm.IManager.OpenSession not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenSessionForDev
				var _return = OpenSessionForDev(im.GetData<uint>(8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // OpenSession
				var _return = OpenSession(im.GetData<uint>(8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pwm.IManager");
		}
	}
}

