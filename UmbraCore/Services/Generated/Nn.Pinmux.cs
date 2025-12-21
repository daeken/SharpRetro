using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pinmux;
public partial class IManager : _IManager_Base;
public abstract class _IManager_Base : IpcInterface {
	protected virtual Nn.Pinmux.ISession OpenSession(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pinmux.IManager.OpenSession not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenSession
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pinmux.IManager");
		}
	}
}

public partial class ISession : _ISession_Base;
public abstract class _ISession_Base : IpcInterface {
	protected virtual void SetPinAssignment(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pinmux.ISession.SetPinAssignment");
	protected virtual void GetPinAssignment(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pinmux.ISession.GetPinAssignment not implemented");
	protected virtual void SetPinAssignmentForHardwareTest(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pinmux.ISession.SetPinAssignmentForHardwareTest");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // SetPinAssignment
				break;
			}
			case 0x1: { // GetPinAssignment
				break;
			}
			case 0x2: { // SetPinAssignmentForHardwareTest
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pinmux.ISession");
		}
	}
}

