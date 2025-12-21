using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.I2c;
public partial class IManager : _IManager_Base;
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
				om.Initialize(1, 0, 0);
				var _return = OpenSessionForDev(im.GetData<ushort>(8), im.GetData<uint>(12), im.GetData<uint>(16), im.GetData<uint>(20));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // OpenSession
				om.Initialize(1, 0, 0);
				var _return = OpenSession(im.GetData<uint>(8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // HasDevice
				om.Initialize(0, 0, 1);
				var _return = HasDevice(im.GetData<uint>(8));
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // HasDeviceForDev
				om.Initialize(0, 0, 1);
				var _return = HasDeviceForDev(im.GetData<ushort>(8), im.GetData<uint>(12), im.GetData<uint>(16), im.GetData<uint>(20));
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
		Console.WriteLine("Stub hit for Nn.I2c.ISession.Send");
	protected virtual void Receive(uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.I2c.ISession.Receive not implemented");
	protected virtual void ExecuteCommandList(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.I2c.ISession.ExecuteCommandList not implemented");
	protected virtual void SendAuto(uint _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.I2c.ISession.SendAuto");
	protected virtual void ReceiveAuto(uint _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.I2c.ISession.ReceiveAuto not implemented");
	protected virtual void ExecuteCommandListAuto(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.I2c.ISession.ExecuteCommandListAuto not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Send
				om.Initialize(0, 0, 0);
				Send(im.GetData<uint>(8), im.GetSpan<byte>(0x5, 0));
				break;
			}
			case 0x1: { // Receive
				om.Initialize(0, 0, 0);
				Receive(im.GetData<uint>(8), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0x2: { // ExecuteCommandList
				om.Initialize(0, 0, 0);
				ExecuteCommandList(im.GetSpan<byte>(0x9, 0), im.GetSpan<byte>(0x6, 0));
				break;
			}
			case 0xA: { // SendAuto
				om.Initialize(0, 0, 0);
				SendAuto(im.GetData<uint>(8), im.GetSpan<byte>(0x21, 0));
				break;
			}
			case 0xB: { // ReceiveAuto
				om.Initialize(0, 0, 0);
				ReceiveAuto(im.GetData<uint>(8), im.GetSpan<byte>(0x22, 0));
				break;
			}
			case 0xC: { // ExecuteCommandListAuto
				om.Initialize(0, 0, 0);
				ExecuteCommandListAuto(im.GetSpan<byte>(0x9, 0), im.GetSpan<byte>(0x22, 0));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.I2c.ISession");
		}
	}
}

