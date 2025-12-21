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
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // RegisterProcess
				break;
			case 0x1: // UnregisterProcess
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Sm.Detail.IManagerInterface");
		}
	}
}

public partial class IUserInterface : _IUserInterface_Base;
public abstract class _IUserInterface_Base : IpcInterface {
	protected virtual void Initialize(ulong _0, ulong reserved) =>
		Console.WriteLine("Stub hit for Nn.Sm.Detail.IUserInterface.Initialize");
	protected virtual IpcInterface GetService(Span<byte> name) =>
		throw new NotImplementedException("Nn.Sm.Detail.IUserInterface.GetService not implemented");
	protected virtual IpcInterface RegisterService(Span<byte> name, byte _1, uint maxHandles) =>
		throw new NotImplementedException("Nn.Sm.Detail.IUserInterface.RegisterService not implemented");
	protected virtual void UnregisterService(Span<byte> name) =>
		Console.WriteLine("Stub hit for Nn.Sm.Detail.IUserInterface.UnregisterService");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Initialize
				break;
			case 0x1: // GetService
				break;
			case 0x2: // RegisterService
				break;
			case 0x3: // UnregisterService
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Sm.Detail.IUserInterface");
		}
	}
}

