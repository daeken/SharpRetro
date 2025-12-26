using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ro.Detail;
public partial class IDebugMonitorInterface : _IDebugMonitorInterface_Base;
public abstract class _IDebugMonitorInterface_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Ro.Detail.IDebugMonitorInterface.Unknown0 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(im.GetBytes(8, 0x8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ro.Detail.IDebugMonitorInterface");
		}
	}
}

public partial class IRoInterface : _IRoInterface_Base;
public abstract class _IRoInterface_Base : IpcInterface {
	protected virtual ulong Unknown0(ulong _0, ulong _1, ulong _2, ulong _3, ulong _4, ulong _5) =>
		throw new NotImplementedException("Nn.Ro.Detail.IRoInterface.Unknown0 not implemented");
	protected virtual void Unknown1(ulong _0, ulong _1, ulong _2) =>
		"Stub hit for Nn.Ro.Detail.IRoInterface.Unknown1".Log();
	protected virtual void Unknown2(ulong _0, ulong _1, ulong _2, ulong _3) =>
		"Stub hit for Nn.Ro.Detail.IRoInterface.Unknown2".Log();
	protected virtual void Unknown3(ulong _0, ulong _1, ulong _2) =>
		"Stub hit for Nn.Ro.Detail.IRoInterface.Unknown3".Log();
	protected virtual void Unknown4(ulong _0, ulong _1, KObject _2) =>
		"Stub hit for Nn.Ro.Detail.IRoInterface.Unknown4".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				var _return = Unknown0(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), im.GetData<ulong>(32), im.GetData<ulong>(40), im.Pid);
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetData<ulong>(8), im.GetData<ulong>(16), im.GetData<ulong>(24), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(im.GetData<ulong>(8), im.GetData<ulong>(16), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4(im.GetData<ulong>(8), im.Pid, Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ro.Detail.IRoInterface");
		}
	}
}

