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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // OpenSessionForDev
				break;
			case 0x1: // OpenSession
				break;
			case 0x2: // HasDevice
				break;
			case 0x3: // HasDeviceForDev
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.I2c.IManager");
		}
	}
}

public partial class ISession : _ISession_Base;
public abstract class _ISession_Base : IpcInterface {
	protected virtual void Send(uint _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.I2c.ISession.Send");
	protected virtual void Receive(uint _0) =>
		throw new NotImplementedException("Nn.I2c.ISession.Receive not implemented");
	protected virtual void ExecuteCommandList(Span<byte> _0) =>
		throw new NotImplementedException("Nn.I2c.ISession.ExecuteCommandList not implemented");
	protected virtual void SendAuto(uint _0, Span<byte> _1) =>
		Console.WriteLine("Stub hit for Nn.I2c.ISession.SendAuto");
	protected virtual void ReceiveAuto(uint _0) =>
		throw new NotImplementedException("Nn.I2c.ISession.ReceiveAuto not implemented");
	protected virtual void ExecuteCommandListAuto(Span<byte> _0) =>
		throw new NotImplementedException("Nn.I2c.ISession.ExecuteCommandListAuto not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Send
				break;
			case 0x1: // Receive
				break;
			case 0x2: // ExecuteCommandList
				break;
			case 0xA: // SendAuto
				break;
			case 0xB: // ReceiveAuto
				break;
			case 0xC: // ExecuteCommandListAuto
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.I2c.ISession");
		}
	}
}

