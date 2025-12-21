using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ro.Detail;
public partial class IDebugMonitorInterface : _IDebugMonitorInterface_Base;
public abstract class _IDebugMonitorInterface_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Ro.Detail.IDebugMonitorInterface.Unknown0 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
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
		Console.WriteLine("Stub hit for Nn.Ro.Detail.IRoInterface.Unknown1");
	protected virtual void Unknown2(ulong _0, ulong _1, ulong _2, ulong _3) =>
		Console.WriteLine("Stub hit for Nn.Ro.Detail.IRoInterface.Unknown2");
	protected virtual void Unknown3(ulong _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Ro.Detail.IRoInterface.Unknown3");
	protected virtual void Unknown4(ulong _0, ulong _1, KObject _2) =>
		Console.WriteLine("Stub hit for Nn.Ro.Detail.IRoInterface.Unknown4");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ro.Detail.IRoInterface");
		}
	}
}

