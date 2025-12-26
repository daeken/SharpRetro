using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.I2c;
public partial class IManager : _IManager_Base {
	public readonly string ServiceName;
	public IManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IManager_Base : IpcInterface {
	protected virtual Nn.I2c.ISession OpenSessionForDev(ushort _0, uint _1, uint _2, uint _3) =>
		throw new NotImplementedException("Nn.I2c.IManager.OpenSessionForDev not implemented");
	protected virtual Nn.I2c.ISession OpenSession(uint _0) =>
		throw new NotImplementedException("Nn.I2c.IManager.OpenSession not implemented");
	protected virtual byte HasDevice(uint _0) =>
		throw new NotImplementedException("Nn.I2c.IManager.HasDevice not implemented");
	protected virtual byte HasDeviceForDev(ushort _0, uint _1, uint _2, uint _3) =>
		throw new NotImplementedException("Nn.I2c.IManager.HasDeviceForDev not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenSessionForDev
				var _return = OpenSessionForDev(im.GetData<ushort>(8), im.GetData<uint>(12), im.GetData<uint>(16), im.GetData<uint>(20));
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
			case 0x2: { // HasDevice
				var _return = HasDevice(im.GetData<uint>(8));
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // HasDeviceForDev
				var _return = HasDeviceForDev(im.GetData<ushort>(8), im.GetData<uint>(12), im.GetData<uint>(16), im.GetData<uint>(20));
				om.Initialize(0, 0, 1);
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.I2c.IManager");
		}
	}
}

public partial class ISession : _ISession_Base;
public abstract class _ISession_Base : IpcInterface {
	protected virtual void Send(uint _0, Span<byte> _1) =>
		"Stub hit for Nn.I2c.ISession.Send".Log();
	protected virtual void Receive(uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.I2c.ISession.Receive not implemented");
	protected virtual void ExecuteCommandList(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.I2c.ISession.ExecuteCommandList not implemented");
	protected virtual void SendAuto(uint _0, Span<byte> _1) =>
		"Stub hit for Nn.I2c.ISession.SendAuto".Log();
	protected virtual void ReceiveAuto(uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.I2c.ISession.ReceiveAuto not implemented");
	protected virtual void ExecuteCommandListAuto(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.I2c.ISession.ExecuteCommandListAuto not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Send
				Send(im.GetData<uint>(8), im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Receive
				Receive(im.GetData<uint>(8), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // ExecuteCommandList
				ExecuteCommandList(im.GetSpan<byte>(0x9, 0), im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // SendAuto
				SendAuto(im.GetData<uint>(8), im.GetSpan<byte>(0x21, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // ReceiveAuto
				ReceiveAuto(im.GetData<uint>(8), im.GetSpan<byte>(0x22, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xC: { // ExecuteCommandListAuto
				ExecuteCommandListAuto(im.GetSpan<byte>(0x9, 0), im.GetSpan<byte>(0x22, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.I2c.ISession");
		}
	}
}

