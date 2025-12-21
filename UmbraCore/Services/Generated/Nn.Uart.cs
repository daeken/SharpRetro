using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Uart;
public partial class IManager : _IManager_Base;
public abstract class _IManager_Base : IpcInterface {
	protected virtual void DoesUartExist(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Uart.IManager.DoesUartExist not implemented");
	protected virtual void DoesUartExistForTest(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Uart.IManager.DoesUartExistForTest not implemented");
	protected virtual void SetUartBaudrate(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Uart.IManager.SetUartBaudrate not implemented");
	protected virtual void SetUartBaudrateForTest(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Uart.IManager.SetUartBaudrateForTest not implemented");
	protected virtual void IsSomethingUartValid(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Uart.IManager.IsSomethingUartValid not implemented");
	protected virtual void IsSomethingUartValidForTest(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Uart.IManager.IsSomethingUartValidForTest not implemented");
	protected virtual Nn.Uart.IPortSession GetSession() =>
		throw new NotImplementedException("Nn.Uart.IManager.GetSession not implemented");
	protected virtual void IsSomethingUartValid2(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Uart.IManager.IsSomethingUartValid2 not implemented");
	protected virtual void IsSomethingUartValid2ForTest(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Uart.IManager.IsSomethingUartValid2ForTest not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // DoesUartExist
				om.Initialize(0, 0, 1);
				DoesUartExist(im.GetBytes(8, 0x4), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // DoesUartExistForTest
				om.Initialize(0, 0, 1);
				DoesUartExistForTest(im.GetBytes(8, 0x4), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // SetUartBaudrate
				om.Initialize(0, 0, 1);
				SetUartBaudrate(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // SetUartBaudrateForTest
				om.Initialize(0, 0, 1);
				SetUartBaudrateForTest(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // IsSomethingUartValid
				om.Initialize(0, 0, 1);
				IsSomethingUartValid(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // IsSomethingUartValidForTest
				om.Initialize(0, 0, 1);
				IsSomethingUartValidForTest(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6: { // GetSession
				om.Initialize(1, 0, 0);
				var _return = GetSession();
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x7: { // IsSomethingUartValid2
				om.Initialize(0, 0, 1);
				IsSomethingUartValid2(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x8: { // IsSomethingUartValid2ForTest
				om.Initialize(0, 0, 1);
				IsSomethingUartValid2ForTest(im.GetBytes(8, 0x8), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Uart.IManager");
		}
	}
}

public partial class IPortSession : _IPortSession_Base;
public abstract class _IPortSession_Base : IpcInterface {
	protected virtual void OpenSession(byte[] _0, KObject _1, KObject _2, out byte[] _3) =>
		throw new NotImplementedException("Nn.Uart.IPortSession.OpenSession not implemented");
	protected virtual void OpenSessionForTest(byte[] _0, KObject _1, KObject _2, out byte[] _3) =>
		throw new NotImplementedException("Nn.Uart.IPortSession.OpenSessionForTest not implemented");
	protected virtual void Unknown2(out byte[] _0) =>
		throw new NotImplementedException("Nn.Uart.IPortSession.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Uart.IPortSession.Unknown3 not implemented");
	protected virtual void Unknown4(out byte[] _0) =>
		throw new NotImplementedException("Nn.Uart.IPortSession.Unknown4 not implemented");
	protected virtual void Unknown5(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Uart.IPortSession.Unknown5 not implemented");
	protected virtual void Unknown6(byte[] _0, out byte[] _1, out KObject _2) =>
		throw new NotImplementedException("Nn.Uart.IPortSession.Unknown6 not implemented");
	protected virtual void Unknown7(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Uart.IPortSession.Unknown7 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenSession
				om.Initialize(0, 0, 1);
				OpenSession(im.GetBytes(8, 0x20), Kernel.Get<KObject>(im.GetCopy(0)), Kernel.Get<KObject>(im.GetCopy(1)), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // OpenSessionForTest
				om.Initialize(0, 0, 1);
				OpenSessionForTest(im.GetBytes(8, 0x20), Kernel.Get<KObject>(im.GetCopy(0)), Kernel.Get<KObject>(im.GetCopy(1)), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 8);
				Unknown2(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 8);
				Unknown3(im.GetSpan<byte>(0x21, 0), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // Unknown4
				om.Initialize(0, 0, 8);
				Unknown4(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // Unknown5
				om.Initialize(0, 0, 8);
				Unknown5(out var _0, im.GetSpan<byte>(0x22, 0));
				om.SetBytes(8, _0);
				break;
			}
			case 0x6: { // Unknown6
				om.Initialize(0, 1, 1);
				Unknown6(im.GetBytes(8, 0x10), out var _0, out var _1);
				om.SetBytes(8, _0);
				om.Copy(0, CreateHandle(_1, copy: true));
				break;
			}
			case 0x7: { // Unknown7
				om.Initialize(0, 0, 1);
				Unknown7(im.GetBytes(8, 0x4), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Uart.IPortSession");
		}
	}
}

