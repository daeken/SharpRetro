using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Htc.Tenv;
public partial class IService : _IService_Base;
public abstract class _IService_Base : IpcInterface {
	protected virtual void GetVariable(Span<byte> _0, out ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Htc.Tenv.IService.GetVariable not implemented");
	protected virtual ulong GetVariableLength(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Htc.Tenv.IService.GetVariableLength not implemented");
	protected virtual void WaitUntilVariableAvailable(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Htc.Tenv.IService.WaitUntilVariableAvailable");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetVariable
				break;
			}
			case 0x1: { // GetVariableLength
				break;
			}
			case 0x2: { // WaitUntilVariableAvailable
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Htc.Tenv.IService");
		}
	}
}

public partial class IServiceManager : _IServiceManager_Base;
public abstract class _IServiceManager_Base : IpcInterface {
	protected virtual IpcInterface GetServiceInterface(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Htc.Tenv.IServiceManager.GetServiceInterface not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetServiceInterface
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Htc.Tenv.IServiceManager");
		}
	}
}

