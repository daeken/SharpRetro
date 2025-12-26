using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pinmux;
public partial class IManager : _IManager_Base {
	public readonly string ServiceName;
	public IManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IManager_Base : IpcInterface {
	protected virtual Nn.Pinmux.ISession OpenSession(byte[] _0) =>
		throw new NotImplementedException("Nn.Pinmux.IManager.OpenSession not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // OpenSession
				var _return = OpenSession(im.GetBytes(8, 0x4));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pinmux.IManager");
		}
	}
}

public partial class ISession : _ISession_Base;
public abstract class _ISession_Base : IpcInterface {
	protected virtual void SetPinAssignment(byte[] _0) =>
		"Stub hit for Nn.Pinmux.ISession.SetPinAssignment".Log();
	protected virtual void GetPinAssignment(out byte[] _0) =>
		throw new NotImplementedException("Nn.Pinmux.ISession.GetPinAssignment not implemented");
	protected virtual void SetPinAssignmentForHardwareTest(byte[] _0) =>
		"Stub hit for Nn.Pinmux.ISession.SetPinAssignmentForHardwareTest".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // SetPinAssignment
				SetPinAssignment(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // GetPinAssignment
				GetPinAssignment(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // SetPinAssignmentForHardwareTest
				SetPinAssignmentForHardwareTest(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pinmux.ISession");
		}
	}
}

