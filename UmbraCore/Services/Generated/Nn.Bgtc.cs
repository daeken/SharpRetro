using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Bgtc;
public partial class IStateControlService : _IStateControlService_Base;
public abstract class _IStateControlService_Base : IpcInterface {
	protected virtual void Unknown1(out byte[] _0) =>
		throw new NotImplementedException("Nn.Bgtc.IStateControlService.Unknown1 not implemented");
	protected virtual KObject Unknown2() =>
		throw new NotImplementedException("Nn.Bgtc.IStateControlService.Unknown2 not implemented");
	protected virtual void Unknown3() =>
		Console.WriteLine("Stub hit for Nn.Bgtc.IStateControlService.Unknown3");
	protected virtual void Unknown4() =>
		Console.WriteLine("Stub hit for Nn.Bgtc.IStateControlService.Unknown4");
	protected virtual void Unknown5(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bgtc.IStateControlService.Unknown5");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 4);
				Unknown1(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 1, 0);
				var _return = Unknown2();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 0);
				Unknown3();
				break;
			}
			case 0x4: { // Unknown4
				om.Initialize(0, 0, 0);
				Unknown4();
				break;
			}
			case 0x5: { // Unknown5
				om.Initialize(0, 0, 0);
				Unknown5(im.GetBytes(8, 0x1));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bgtc.IStateControlService");
		}
	}
}

public partial class ITaskService : _ITaskService_Base;
public abstract class _ITaskService_Base : IpcInterface {
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Bgtc.ITaskService.Unknown1");
	protected virtual void Unknown2() =>
		Console.WriteLine("Stub hit for Nn.Bgtc.ITaskService.Unknown2");
	protected virtual KObject Unknown3() =>
		throw new NotImplementedException("Nn.Bgtc.ITaskService.Unknown3 not implemented");
	protected virtual void Unknown4(out byte[] _0) =>
		throw new NotImplementedException("Nn.Bgtc.ITaskService.Unknown4 not implemented");
	protected virtual void Unknown5(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Bgtc.ITaskService.Unknown5");
	protected virtual void Unknown6(out byte[] _0) =>
		throw new NotImplementedException("Nn.Bgtc.ITaskService.Unknown6 not implemented");
	protected virtual void Unknown11(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bgtc.ITaskService.Unknown11");
	protected virtual void Unknown12(out byte[] _0) =>
		throw new NotImplementedException("Nn.Bgtc.ITaskService.Unknown12 not implemented");
	protected virtual void Unknown13() =>
		Console.WriteLine("Stub hit for Nn.Bgtc.ITaskService.Unknown13");
	protected virtual KObject Unknown14() =>
		throw new NotImplementedException("Nn.Bgtc.ITaskService.Unknown14 not implemented");
	protected virtual void Unknown15(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Bgtc.ITaskService.Unknown15");
	protected virtual void Unknown101(out byte[] _0) =>
		throw new NotImplementedException("Nn.Bgtc.ITaskService.Unknown101 not implemented");
	protected virtual void Unknown102(out byte[] _0) =>
		throw new NotImplementedException("Nn.Bgtc.ITaskService.Unknown102 not implemented");
	protected virtual void Unknown103(out byte[] _0) =>
		throw new NotImplementedException("Nn.Bgtc.ITaskService.Unknown103 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 0);
				Unknown1();
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 0);
				Unknown2();
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 1, 0);
				var _return = Unknown3();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x4: { // Unknown4
				om.Initialize(0, 0, 1);
				Unknown4(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // Unknown5
				om.Initialize(0, 0, 0);
				Unknown5(im.GetSpan<byte>(0x9, 0));
				break;
			}
			case 0x6: { // Unknown6
				om.Initialize(0, 0, 1);
				Unknown6(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xB: { // Unknown11
				om.Initialize(0, 0, 0);
				Unknown11(im.GetBytes(8, 0x4));
				break;
			}
			case 0xC: { // Unknown12
				om.Initialize(0, 0, 4);
				Unknown12(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xD: { // Unknown13
				om.Initialize(0, 0, 0);
				Unknown13();
				break;
			}
			case 0xE: { // Unknown14
				om.Initialize(0, 1, 0);
				var _return = Unknown14();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xF: { // Unknown15
				om.Initialize(0, 0, 0);
				Unknown15(im.GetBytes(8, 0x8));
				break;
			}
			case 0x65: { // Unknown101
				om.Initialize(0, 0, 4);
				Unknown101(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x66: { // Unknown102
				om.Initialize(0, 0, 1);
				Unknown102(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x67: { // Unknown103
				om.Initialize(0, 0, 1);
				Unknown103(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Bgtc.ITaskService");
		}
	}
}

