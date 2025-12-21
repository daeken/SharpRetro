using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Lbl.Detail;
public partial class ILblController : _ILblController_Base;
public abstract class _ILblController_Base : IpcInterface {
	protected virtual void Unknown0() =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown0");
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown1");
	protected virtual void Unknown2(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown2");
	protected virtual void Unknown3(out byte[] _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown3 not implemented");
	protected virtual void Unknown4() =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown4");
	protected virtual void Unknown5(out byte[] _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown5 not implemented");
	protected virtual void TurnOnBacklight(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.TurnOnBacklight");
	protected virtual void TurnOffBacklight(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.TurnOffBacklight");
	protected virtual void GetBacklightStatus(out byte[] _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.GetBacklightStatus not implemented");
	protected virtual void Unknown9() =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown9");
	protected virtual void Unknown10() =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown10");
	protected virtual void Unknown11(out byte[] _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown11 not implemented");
	protected virtual void Unknown12() =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown12");
	protected virtual void Unknown13() =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown13");
	protected virtual void Unknown14(out byte[] _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown14 not implemented");
	protected virtual void Unknown15(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown15");
	protected virtual void ReadRawLightSensor(out byte[] _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.ReadRawLightSensor not implemented");
	protected virtual void Unknown17(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown17");
	protected virtual void Unknown18(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown18 not implemented");
	protected virtual void Unknown19(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown19");
	protected virtual void Unknown20(out byte[] _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown20 not implemented");
	protected virtual void Unknown21(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown21");
	protected virtual void Unknown22(out byte[] _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown22 not implemented");
	protected virtual void Unknown23(out byte[] _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown23 not implemented");
	protected virtual void Unknown24(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown24");
	protected virtual void Unknown25(out byte[] _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown25 not implemented");
	protected virtual void EnableVrMode() =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.EnableVrMode");
	protected virtual void DisableVrMode() =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.DisableVrMode");
	protected virtual void GetVrMode(out byte[] _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.GetVrMode not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				om.Initialize(0, 0, 0);
				Unknown0();
				break;
			}
			case 0x1: { // Unknown1
				om.Initialize(0, 0, 0);
				Unknown1();
				break;
			}
			case 0x2: { // Unknown2
				om.Initialize(0, 0, 0);
				Unknown2(im.GetBytes(8, 0x4));
				break;
			}
			case 0x3: { // Unknown3
				om.Initialize(0, 0, 4);
				Unknown3(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // Unknown4
				om.Initialize(0, 0, 0);
				Unknown4();
				break;
			}
			case 0x5: { // Unknown5
				om.Initialize(0, 0, 4);
				Unknown5(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6: { // TurnOnBacklight
				om.Initialize(0, 0, 0);
				TurnOnBacklight(im.GetBytes(8, 0x8));
				break;
			}
			case 0x7: { // TurnOffBacklight
				om.Initialize(0, 0, 0);
				TurnOffBacklight(im.GetBytes(8, 0x8));
				break;
			}
			case 0x8: { // GetBacklightStatus
				om.Initialize(0, 0, 4);
				GetBacklightStatus(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x9: { // Unknown9
				om.Initialize(0, 0, 0);
				Unknown9();
				break;
			}
			case 0xA: { // Unknown10
				om.Initialize(0, 0, 0);
				Unknown10();
				break;
			}
			case 0xB: { // Unknown11
				om.Initialize(0, 0, 1);
				Unknown11(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC: { // Unknown12
				om.Initialize(0, 0, 0);
				Unknown12();
				break;
			}
			case 0xD: { // Unknown13
				om.Initialize(0, 0, 0);
				Unknown13();
				break;
			}
			case 0xE: { // Unknown14
				om.Initialize(0, 0, 1);
				Unknown14(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xF: { // Unknown15
				om.Initialize(0, 0, 0);
				Unknown15(im.GetBytes(8, 0x4));
				break;
			}
			case 0x10: { // ReadRawLightSensor
				om.Initialize(0, 0, 4);
				ReadRawLightSensor(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x11: { // Unknown17
				om.Initialize(0, 0, 0);
				Unknown17(im.GetBytes(8, 0x8));
				break;
			}
			case 0x12: { // Unknown18
				om.Initialize(0, 0, 4);
				Unknown18(im.GetBytes(8, 0x4), out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x13: { // Unknown19
				om.Initialize(0, 0, 0);
				Unknown19(im.GetBytes(8, 0xC));
				break;
			}
			case 0x14: { // Unknown20
				om.Initialize(0, 0, 12);
				Unknown20(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x15: { // Unknown21
				om.Initialize(0, 0, 0);
				Unknown21(im.GetBytes(8, 0xC));
				break;
			}
			case 0x16: { // Unknown22
				om.Initialize(0, 0, 12);
				Unknown22(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x17: { // Unknown23
				om.Initialize(0, 0, 1);
				Unknown23(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x18: { // Unknown24
				om.Initialize(0, 0, 0);
				Unknown24(im.GetBytes(8, 0x4));
				break;
			}
			case 0x19: { // Unknown25
				om.Initialize(0, 0, 4);
				Unknown25(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1A: { // EnableVrMode
				om.Initialize(0, 0, 0);
				EnableVrMode();
				break;
			}
			case 0x1B: { // DisableVrMode
				om.Initialize(0, 0, 0);
				DisableVrMode();
				break;
			}
			case 0x1C: { // GetVrMode
				om.Initialize(0, 0, 1);
				GetVrMode(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Lbl.Detail.ILblController");
		}
	}
}

