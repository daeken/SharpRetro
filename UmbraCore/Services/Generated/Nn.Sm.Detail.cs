using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Sm.Detail;
public partial class IManagerInterface : _IManagerInterface_Base {
	public readonly string ServiceName;
	public IManagerInterface(string serviceName) => ServiceName = serviceName;
}
public abstract class _IManagerInterface_Base : IpcInterface {
	protected virtual void RegisterProcess(ulong _0, Span<byte> _1, Span<byte> _2) =>
		"Stub hit for Nn.Sm.Detail.IManagerInterface.RegisterProcess".Log();
	protected virtual void UnregisterProcess(ulong _0) =>
		"Stub hit for Nn.Sm.Detail.IManagerInterface.UnregisterProcess".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RegisterProcess
				RegisterProcess(im.GetData<ulong>(8), im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x5, 1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // UnregisterProcess
				UnregisterProcess(im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Sm.Detail.IManagerInterface");
		}
	}
}

public partial class IUserInterface : _IUserInterface_Base {
	public readonly string ServiceName;
	public IUserInterface(string serviceName) => ServiceName = serviceName;
}
public abstract class _IUserInterface_Base : IpcInterface {
	protected virtual void Initialize(ulong _0, ulong reserved) =>
		"Stub hit for Nn.Sm.Detail.IUserInterface.Initialize".Log();
	protected virtual IpcInterface GetService(byte[] name) =>
		throw new NotImplementedException("Nn.Sm.Detail.IUserInterface.GetService not implemented");
	protected virtual IpcInterface RegisterService(byte[] name, byte _1, uint maxHandles) =>
		throw new NotImplementedException("Nn.Sm.Detail.IUserInterface.RegisterService not implemented");
	protected virtual void UnregisterService(byte[] name) =>
		"Stub hit for Nn.Sm.Detail.IUserInterface.UnregisterService".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				Initialize(im.Pid, im.GetData<ulong>(8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // GetService
				var _return = GetService(im.GetBytes(8, 0x8));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // RegisterService
				var _return = RegisterService(im.GetBytes(8, 0x8), im.GetData<byte>(16), im.GetData<uint>(20));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // UnregisterService
				UnregisterService(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Sm.Detail.IUserInterface");
		}
	}
}

