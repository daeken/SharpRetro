using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Sm.Detail;
public partial class IManagerInterface : _IManagerInterface_Base;
public abstract class _IManagerInterface_Base : IpcInterface {
	protected virtual void RegisterProcess(ulong _0, Span<byte> _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Sm.Detail.IManagerInterface.RegisterProcess");
	protected virtual void UnregisterProcess(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Sm.Detail.IManagerInterface.UnregisterProcess");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // RegisterProcess
				om.Initialize(0, 0, 0);
				RegisterProcess(im.GetData<ulong>(8), im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x5, 1));
				break;
			}
			case 0x1: { // UnregisterProcess
				om.Initialize(0, 0, 0);
				UnregisterProcess(im.GetData<ulong>(8));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Sm.Detail.IManagerInterface");
		}
	}
}

public partial class IUserInterface : _IUserInterface_Base;
public abstract class _IUserInterface_Base : IpcInterface {
	protected virtual void Initialize(ulong _0, ulong reserved) =>
		Console.WriteLine("Stub hit for Nn.Sm.Detail.IUserInterface.Initialize");
	protected virtual IpcInterface GetService(byte[] name) =>
		throw new NotImplementedException("Nn.Sm.Detail.IUserInterface.GetService not implemented");
	protected virtual IpcInterface RegisterService(byte[] name, byte _1, uint maxHandles) =>
		throw new NotImplementedException("Nn.Sm.Detail.IUserInterface.RegisterService not implemented");
	protected virtual void UnregisterService(byte[] name) =>
		Console.WriteLine("Stub hit for Nn.Sm.Detail.IUserInterface.UnregisterService");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				om.Initialize(0, 0, 0);
				Initialize(im.Pid, im.GetData<ulong>(8));
				break;
			}
			case 0x1: { // GetService
				om.Initialize(1, 0, 0);
				var _return = GetService(im.GetBytes(8, 0x8));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // RegisterService
				om.Initialize(1, 0, 0);
				var _return = RegisterService(im.GetBytes(8, 0x8), im.GetData<byte>(16), im.GetData<uint>(20));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // UnregisterService
				om.Initialize(0, 0, 0);
				UnregisterService(im.GetBytes(8, 0x8));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Sm.Detail.IUserInterface");
		}
	}
}

