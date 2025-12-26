using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Lbl.Detail;
public partial class ILblController : _ILblController_Base {
	public readonly string ServiceName;
	public ILblController(string serviceName) => ServiceName = serviceName;
}
public abstract class _ILblController_Base : IpcInterface {
	protected virtual void Unknown0() =>
		"Stub hit for Nn.Lbl.Detail.ILblController.Unknown0".Log();
	protected virtual void Unknown1() =>
		"Stub hit for Nn.Lbl.Detail.ILblController.Unknown1".Log();
	protected virtual void Unknown2(byte[] _0) =>
		"Stub hit for Nn.Lbl.Detail.ILblController.Unknown2".Log();
	protected virtual void Unknown3(out byte[] _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown3 not implemented");
	protected virtual void Unknown4() =>
		"Stub hit for Nn.Lbl.Detail.ILblController.Unknown4".Log();
	protected virtual void Unknown5(out byte[] _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown5 not implemented");
	protected virtual void TurnOnBacklight(byte[] _0) =>
		"Stub hit for Nn.Lbl.Detail.ILblController.TurnOnBacklight".Log();
	protected virtual void TurnOffBacklight(byte[] _0) =>
		"Stub hit for Nn.Lbl.Detail.ILblController.TurnOffBacklight".Log();
	protected virtual void GetBacklightStatus(out byte[] _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.GetBacklightStatus not implemented");
	protected virtual void Unknown9() =>
		"Stub hit for Nn.Lbl.Detail.ILblController.Unknown9".Log();
	protected virtual void Unknown10() =>
		"Stub hit for Nn.Lbl.Detail.ILblController.Unknown10".Log();
	protected virtual void Unknown11(out byte[] _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown11 not implemented");
	protected virtual void Unknown12() =>
		"Stub hit for Nn.Lbl.Detail.ILblController.Unknown12".Log();
	protected virtual void Unknown13() =>
		"Stub hit for Nn.Lbl.Detail.ILblController.Unknown13".Log();
	protected virtual void Unknown14(out byte[] _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown14 not implemented");
	protected virtual void Unknown15(byte[] _0) =>
		"Stub hit for Nn.Lbl.Detail.ILblController.Unknown15".Log();
	protected virtual void ReadRawLightSensor(out byte[] _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.ReadRawLightSensor not implemented");
	protected virtual void Unknown17(byte[] _0) =>
		"Stub hit for Nn.Lbl.Detail.ILblController.Unknown17".Log();
	protected virtual void Unknown18(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown18 not implemented");
	protected virtual void Unknown19(byte[] _0) =>
		"Stub hit for Nn.Lbl.Detail.ILblController.Unknown19".Log();
	protected virtual void Unknown20(out byte[] _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown20 not implemented");
	protected virtual void Unknown21(byte[] _0) =>
		"Stub hit for Nn.Lbl.Detail.ILblController.Unknown21".Log();
	protected virtual void Unknown22(out byte[] _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown22 not implemented");
	protected virtual void Unknown23(out byte[] _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown23 not implemented");
	protected virtual void Unknown24(byte[] _0) =>
		"Stub hit for Nn.Lbl.Detail.ILblController.Unknown24".Log();
	protected virtual void Unknown25(out byte[] _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown25 not implemented");
	protected virtual void EnableVrMode() =>
		"Stub hit for Nn.Lbl.Detail.ILblController.EnableVrMode".Log();
	protected virtual void DisableVrMode() =>
		"Stub hit for Nn.Lbl.Detail.ILblController.DisableVrMode".Log();
	protected virtual void GetVrMode(out byte[] _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.GetVrMode not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // Unknown5
				Unknown5(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6: { // TurnOnBacklight
				TurnOnBacklight(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // TurnOffBacklight
				TurnOffBacklight(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // GetBacklightStatus
				GetBacklightStatus(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x9: { // Unknown9
				Unknown9();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xA: { // Unknown10
				Unknown10();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xB: { // Unknown11
				Unknown11(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC: { // Unknown12
				Unknown12();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xD: { // Unknown13
				Unknown13();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0xE: { // Unknown14
				Unknown14(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0xF: { // Unknown15
				Unknown15(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x10: { // ReadRawLightSensor
				ReadRawLightSensor(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x11: { // Unknown17
				Unknown17(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x12: { // Unknown18
				Unknown18(im.GetBytes(8, 0x4), out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x13: { // Unknown19
				Unknown19(im.GetBytes(8, 0xC));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x14: { // Unknown20
				Unknown20(out var _0);
				om.Initialize(0, 0, 12);
				om.SetBytes(8, _0);
				break;
			}
			case 0x15: { // Unknown21
				Unknown21(im.GetBytes(8, 0xC));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x16: { // Unknown22
				Unknown22(out var _0);
				om.Initialize(0, 0, 12);
				om.SetBytes(8, _0);
				break;
			}
			case 0x17: { // Unknown23
				Unknown23(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x18: { // Unknown24
				Unknown24(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x19: { // Unknown25
				Unknown25(out var _0);
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1A: { // EnableVrMode
				EnableVrMode();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1B: { // DisableVrMode
				DisableVrMode();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1C: { // GetVrMode
				GetVrMode(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Lbl.Detail.ILblController");
		}
	}
}

