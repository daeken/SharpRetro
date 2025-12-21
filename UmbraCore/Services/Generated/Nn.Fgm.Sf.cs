using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Fgm.Sf;
public partial class IDebugger : _IDebugger_Base;
public abstract class _IDebugger_Base : IpcInterface {
	protected virtual KObject Initialize(ulong _0, KObject _1) =>
		throw new NotImplementedException("Nn.Fgm.Sf.IDebugger.Initialize not implemented");
	protected virtual void Read(out uint _0, out uint _1, out uint _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Fgm.Sf.IDebugger.Read not implemented");
	protected virtual void Cancel() =>
		Console.WriteLine("Stub hit for Nn.Fgm.Sf.IDebugger.Cancel");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				break;
			}
			case 0x1: { // Read
				break;
			}
			case 0x2: { // Cancel
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fgm.Sf.IDebugger");
		}
	}
}

public partial class IRequest : _IRequest_Base;
public abstract class _IRequest_Base : IpcInterface {
	protected virtual KObject Initialize(uint _0, ulong _1, ulong _2) =>
		throw new NotImplementedException("Nn.Fgm.Sf.IRequest.Initialize not implemented");
	protected virtual void Set(uint _0, uint _1) =>
		Console.WriteLine("Stub hit for Nn.Fgm.Sf.IRequest.Set");
	protected virtual uint Get() =>
		throw new NotImplementedException("Nn.Fgm.Sf.IRequest.Get not implemented");
	protected virtual void Cancel() =>
		Console.WriteLine("Stub hit for Nn.Fgm.Sf.IRequest.Cancel");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				break;
			}
			case 0x1: { // Set
				break;
			}
			case 0x2: { // Get
				break;
			}
			case 0x3: { // Cancel
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fgm.Sf.IRequest");
		}
	}
}

public partial class ISession : _ISession_Base;
public abstract class _ISession_Base : IpcInterface {
	protected virtual Nn.Fgm.Sf.IRequest Initialize() =>
		throw new NotImplementedException("Nn.Fgm.Sf.ISession.Initialize not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fgm.Sf.ISession");
		}
	}
}

