using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Uart;
public partial class IManager : _IManager_Base;
public abstract class _IManager_Base : IpcInterface {
	protected virtual void DoesUartExist(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Uart.IManager.DoesUartExist not implemented");
	protected virtual void DoesUartExistForTest(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Uart.IManager.DoesUartExistForTest not implemented");
	protected virtual void SetUartBaudrate(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Uart.IManager.SetUartBaudrate not implemented");
	protected virtual void SetUartBaudrateForTest(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Uart.IManager.SetUartBaudrateForTest not implemented");
	protected virtual void IsSomethingUartValid(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Uart.IManager.IsSomethingUartValid not implemented");
	protected virtual void IsSomethingUartValidForTest(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Uart.IManager.IsSomethingUartValidForTest not implemented");
	protected virtual Nn.Uart.IPortSession GetSession() =>
		throw new NotImplementedException("Nn.Uart.IManager.GetSession not implemented");
	protected virtual void IsSomethingUartValid2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Uart.IManager.IsSomethingUartValid2 not implemented");
	protected virtual void IsSomethingUartValid2ForTest(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Uart.IManager.IsSomethingUartValid2ForTest not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // DoesUartExist
				break;
			case 0x1: // DoesUartExistForTest
				break;
			case 0x2: // SetUartBaudrate
				break;
			case 0x3: // SetUartBaudrateForTest
				break;
			case 0x4: // IsSomethingUartValid
				break;
			case 0x5: // IsSomethingUartValidForTest
				break;
			case 0x6: // GetSession
				break;
			case 0x7: // IsSomethingUartValid2
				break;
			case 0x8: // IsSomethingUartValid2ForTest
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Uart.IManager");
		}
	}
}

public partial class IPortSession : _IPortSession_Base;
public abstract class _IPortSession_Base : IpcInterface {
	protected virtual void OpenSession(Span<byte> _0, KObject _1, KObject _2) =>
		throw new NotImplementedException("Nn.Uart.IPortSession.OpenSession not implemented");
	protected virtual void OpenSessionForTest(Span<byte> _0, KObject _1, KObject _2) =>
		throw new NotImplementedException("Nn.Uart.IPortSession.OpenSessionForTest not implemented");
	protected virtual void Unknown2() =>
		throw new NotImplementedException("Nn.Uart.IPortSession.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Uart.IPortSession.Unknown3 not implemented");
	protected virtual void Unknown4() =>
		throw new NotImplementedException("Nn.Uart.IPortSession.Unknown4 not implemented");
	protected virtual void Unknown5() =>
		throw new NotImplementedException("Nn.Uart.IPortSession.Unknown5 not implemented");
	protected virtual void Unknown6(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Uart.IPortSession.Unknown6 not implemented");
	protected virtual void Unknown7(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Uart.IPortSession.Unknown7 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // OpenSession
				break;
			case 0x1: // OpenSessionForTest
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			case 0x5: // Unknown5
				break;
			case 0x6: // Unknown6
				break;
			case 0x7: // Unknown7
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Uart.IPortSession");
		}
	}
}

