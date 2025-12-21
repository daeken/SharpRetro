using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Htc.Tenv;
public partial class IService : _IService_Base;
public abstract class _IService_Base : IpcInterface {
	protected virtual void GetVariable(byte[] _0, out ulong _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Htc.Tenv.IService.GetVariable not implemented");
	protected virtual ulong GetVariableLength(byte[] _0) =>
		throw new NotImplementedException("Nn.Htc.Tenv.IService.GetVariableLength not implemented");
	protected virtual void WaitUntilVariableAvailable(ulong _0) =>
		Console.WriteLine("Stub hit for Nn.Htc.Tenv.IService.WaitUntilVariableAvailable");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetVariable
				om.Initialize(0, 0, 8);
				GetVariable(im.GetBytes(8, 0x40), out var _0, im.GetSpan<byte>(0x6, 0));
				om.SetData(8, _0);
				break;
			}
			case 0x1: { // GetVariableLength
				om.Initialize(0, 0, 8);
				var _return = GetVariableLength(im.GetBytes(8, 0x40));
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // WaitUntilVariableAvailable
				om.Initialize(0, 0, 0);
				WaitUntilVariableAvailable(im.GetData<ulong>(8));
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // GetServiceInterface
				om.Initialize(1, 0, 0);
				var _return = GetServiceInterface(im.GetData<ulong>(8), im.Pid);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Htc.Tenv.IServiceManager");
		}
	}
}

