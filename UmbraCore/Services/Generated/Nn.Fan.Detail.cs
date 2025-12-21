using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Fan.Detail;
public partial class IController : _IController_Base;
public abstract class _IController_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Fan.Detail.IController.Unknown0");
	protected virtual void Unknown1(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Fan.Detail.IController.Unknown1 not implemented");
	protected virtual void Unknown2(out byte[] _0) =>
		throw new NotImplementedException("Nn.Fan.Detail.IController.Unknown2 not implemented");
	protected virtual void Unknown3(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Fan.Detail.IController.Unknown3");
	protected virtual void Unknown4(out byte[] _0) =>
		throw new NotImplementedException("Nn.Fan.Detail.IController.Unknown4 not implemented");
	protected virtual void Unknown5() =>
		Console.WriteLine("Stub hit for Nn.Fan.Detail.IController.Unknown5");
	protected virtual void Unknown6() =>
		Console.WriteLine("Stub hit for Nn.Fan.Detail.IController.Unknown6");
	protected virtual void Unknown7(out byte[] _0) =>
		throw new NotImplementedException("Nn.Fan.Detail.IController.Unknown7 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 0);
				Unknown0(im.GetBytes(8, 0x4));
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 4);
				Unknown1(im.GetBytes(8, 0x4), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 4);
				Unknown2(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 0);
				Unknown3(im.GetBytes(8, 0x4));
				break;
			}
			case 0x4: { // Unknown4
				om.Initialize(0, 0, 4);
				Unknown4(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // Unknown5
				om.Initialize(0, 0, 0);
				Unknown5();
				break;
			}
			case 0x6: { // Unknown6
				om.Initialize(0, 0, 0);
				Unknown6();
				break;
			}
			case 0x7: { // Unknown7
				om.Initialize(0, 0, 4);
				Unknown7(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fan.Detail.IController");
		}
	}
}

public partial class IManager : _IManager_Base;
public abstract class _IManager_Base : IpcInterface {
	protected virtual Nn.Fan.Detail.IController Unknown0(byte[] _0) =>
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
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(1, 0, 0);
				var _return = Unknown0(im.GetBytes(8, 0x4));
				om.Move(0, CreateHandle(_return));
				break;
			}
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
				Unknown5();
				break;
			}
			case 0x6: { // Unknown6
				om.Initialize(0, 0, 0);
				Unknown6();
				break;
			}
			case 0x7: { // Unknown7
				om.Initialize(0, 0, 0);
				Unknown7();
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Fan.Detail.IManager");
		}
	}
}

