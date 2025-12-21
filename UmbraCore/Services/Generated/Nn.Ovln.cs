using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ovln;
public partial class IReceiver : _IReceiver_Base;
public abstract class _IReceiver_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ovln.IReceiver.Unknown0");
	protected virtual void Unknown1(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ovln.IReceiver.Unknown1");
	protected virtual KObject Unknown2() =>
		throw new NotImplementedException("Nn.Ovln.IReceiver.Unknown2 not implemented");
	protected virtual void Unknown3(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ovln.IReceiver.Unknown3 not implemented");
	protected virtual void Unknown4(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ovln.IReceiver.Unknown4 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 0);
				Unknown0(im.GetBytes(8, 0x10));
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 0);
				Unknown1(im.GetBytes(8, 0x10));
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 1, 0);
				var _return = Unknown2();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 128);
				Unknown3(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // Unknown4
				om.Initialize(0, 0, 136);
				Unknown4(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ovln.IReceiver");
		}
	}
}

public partial class IReceiverService : _IReceiverService_Base;
public abstract class _IReceiverService_Base : IpcInterface {
	protected virtual Nn.Ovln.IReceiver Unknown0() =>
		throw new NotImplementedException("Nn.Ovln.IReceiverService.Unknown0 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(1, 0, 0);
				var _return = Unknown0();
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ovln.IReceiverService");
		}
	}
}

public partial class ISender : _ISender_Base;
public abstract class _ISender_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Ovln.ISender.Unknown0");
	protected virtual void Unknown1(out byte[] _0) =>
		throw new NotImplementedException("Nn.Ovln.ISender.Unknown1 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 0);
				Unknown0(im.GetBytes(8, 0x88));
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 4);
				Unknown1(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ovln.ISender");
		}
	}
}

public partial class ISenderService : _ISenderService_Base;
public abstract class _ISenderService_Base : IpcInterface {
	protected virtual Nn.Ovln.ISender Unknown0(byte[] _0) =>
		throw new NotImplementedException("Nn.Ovln.ISenderService.Unknown0 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(1, 0, 0);
				var _return = Unknown0(im.GetBytes(8, 0x18));
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Ovln.ISenderService");
		}
	}
}

