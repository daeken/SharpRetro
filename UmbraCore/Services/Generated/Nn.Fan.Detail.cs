using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Fan.Detail;
public partial class IController : _IController_Base;
public abstract class _IController_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Fan.Detail.IController.Unknown0");
	protected virtual void Unknown1(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Fan.Detail.IController.Unknown1 not implemented");
	protected virtual void Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Fan.Detail.IController.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Fan.Detail.IController.Unknown3");
	protected virtual void Unknown4(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Fan.Detail.IController.Unknown4 not implemented");
	protected virtual void Unknown5() =>
		Console.WriteLine("Stub hit for Nn.Fan.Detail.IController.Unknown5");
	protected virtual void Unknown6() =>
		Console.WriteLine("Stub hit for Nn.Fan.Detail.IController.Unknown6");
	protected virtual void Unknown7(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Fan.Detail.IController.Unknown7 not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				break;
			}
			case 0x1: { // Unknown1
				break;
			}
			case 0x2: { // Unknown2
				break;
			}
			case 0x3: { // Unknown3
				break;
			}
			case 0x4: { // Unknown4
				break;
			}
			case 0x5: { // Unknown5
				break;
			}
			case 0x6: { // Unknown6
				break;
			}
			case 0x7: { // Unknown7
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fan.Detail.IController");
		}
	}
}

public partial class IManager : _IManager_Base;
public abstract class _IManager_Base : IpcInterface {
	protected virtual Nn.Fan.Detail.IController Unknown0(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Fan.Detail.IManager.Unknown0 not implemented");
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Fan.Detail.IManager.Unknown1");
	protected virtual void Unknown2() =>
		Console.WriteLine("Stub hit for Nn.Fan.Detail.IManager.Unknown2");
	protected virtual void Unknown3() =>
		Console.WriteLine("Stub hit for Nn.Fan.Detail.IManager.Unknown3");
	protected virtual void Unknown4() =>
		Console.WriteLine("Stub hit for Nn.Fan.Detail.IManager.Unknown4");
	protected virtual void Unknown5() =>
		Console.WriteLine("Stub hit for Nn.Fan.Detail.IManager.Unknown5");
	protected virtual void Unknown6() =>
		Console.WriteLine("Stub hit for Nn.Fan.Detail.IManager.Unknown6");
	protected virtual void Unknown7() =>
		Console.WriteLine("Stub hit for Nn.Fan.Detail.IManager.Unknown7");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				break;
			}
			case 0x1: { // Unknown1
				break;
			}
			case 0x2: { // Unknown2
				break;
			}
			case 0x3: { // Unknown3
				break;
			}
			case 0x4: { // Unknown4
				break;
			}
			case 0x5: { // Unknown5
				break;
			}
			case 0x6: { // Unknown6
				break;
			}
			case 0x7: { // Unknown7
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fan.Detail.IManager");
		}
	}
}

