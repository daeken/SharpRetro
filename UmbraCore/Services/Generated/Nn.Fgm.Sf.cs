using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Fgm.Sf;
public partial class IDebugger : _IDebugger_Base {
	public readonly string ServiceName;
	public IDebugger(string serviceName) => ServiceName = serviceName;
}
public abstract class _IDebugger_Base : IpcInterface {
	protected virtual KObject Initialize(ulong _0, KObject _1) =>
		throw new NotImplementedException("Nn.Fgm.Sf.IDebugger.Initialize not implemented");
	protected virtual void Read(out uint _0, out uint _1, out uint _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Fgm.Sf.IDebugger.Read not implemented");
	protected virtual void Cancel() =>
		Console.WriteLine("Stub hit for Nn.Fgm.Sf.IDebugger.Cancel");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				var _return = Initialize(im.GetData<ulong>(8), Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // Read
				Read(out var _0, out var _1, out var _2, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 12);
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				break;
			}
			case 0x2: { // Cancel
				Cancel();
				om.Initialize(0, 0, 0);
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				var _return = Initialize(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x1: { // Set
				Set(im.GetData<uint>(8), im.GetData<uint>(12));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Get
				var _return = Get();
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // Cancel
				Cancel();
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fgm.Sf.IRequest");
		}
	}
}

public partial class ISession : _ISession_Base {
	public readonly string ServiceName;
	public ISession(string serviceName) => ServiceName = serviceName;
}
public abstract class _ISession_Base : IpcInterface {
	protected virtual Nn.Fgm.Sf.IRequest Initialize() =>
		throw new NotImplementedException("Nn.Fgm.Sf.ISession.Initialize not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				var _return = Initialize();
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fgm.Sf.ISession");
		}
	}
}

