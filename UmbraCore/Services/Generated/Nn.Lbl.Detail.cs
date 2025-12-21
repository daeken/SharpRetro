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
	protected virtual void Unknown2(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown2");
	protected virtual void Unknown3(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown3 not implemented");
	protected virtual void Unknown4() =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown4");
	protected virtual void Unknown5(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown5 not implemented");
	protected virtual void TurnOnBacklight(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.TurnOnBacklight");
	protected virtual void TurnOffBacklight(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.TurnOffBacklight");
	protected virtual void GetBacklightStatus(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.GetBacklightStatus not implemented");
	protected virtual void Unknown9() =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown9");
	protected virtual void Unknown10() =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown10");
	protected virtual void Unknown11(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown11 not implemented");
	protected virtual void Unknown12() =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown12");
	protected virtual void Unknown13() =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown13");
	protected virtual void Unknown14(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown14 not implemented");
	protected virtual void Unknown15(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown15");
	protected virtual void ReadRawLightSensor(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.ReadRawLightSensor not implemented");
	protected virtual void Unknown17(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown17");
	protected virtual void Unknown18(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown18 not implemented");
	protected virtual void Unknown19(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown19");
	protected virtual void Unknown20(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown20 not implemented");
	protected virtual void Unknown21(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown21");
	protected virtual void Unknown22(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown22 not implemented");
	protected virtual void Unknown23(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown23 not implemented");
	protected virtual void Unknown24(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.Unknown24");
	protected virtual void Unknown25(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.Unknown25 not implemented");
	protected virtual void EnableVrMode() =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.EnableVrMode");
	protected virtual void DisableVrMode() =>
		Console.WriteLine("Stub hit for Nn.Lbl.Detail.ILblController.DisableVrMode");
	protected virtual void GetVrMode(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Lbl.Detail.ILblController.GetVrMode not implemented");
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
			case 0x6: { // TurnOnBacklight
				break;
			}
			case 0x7: { // TurnOffBacklight
				break;
			}
			case 0x8: { // GetBacklightStatus
				break;
			}
			case 0x9: { // Unknown9
				break;
			}
			case 0xA: { // Unknown10
				break;
			}
			case 0xB: { // Unknown11
				break;
			}
			case 0xC: { // Unknown12
				break;
			}
			case 0xD: { // Unknown13
				break;
			}
			case 0xE: { // Unknown14
				break;
			}
			case 0xF: { // Unknown15
				break;
			}
			case 0x10: { // ReadRawLightSensor
				break;
			}
			case 0x11: { // Unknown17
				break;
			}
			case 0x12: { // Unknown18
				break;
			}
			case 0x13: { // Unknown19
				break;
			}
			case 0x14: { // Unknown20
				break;
			}
			case 0x15: { // Unknown21
				break;
			}
			case 0x16: { // Unknown22
				break;
			}
			case 0x17: { // Unknown23
				break;
			}
			case 0x18: { // Unknown24
				break;
			}
			case 0x19: { // Unknown25
				break;
			}
			case 0x1A: { // EnableVrMode
				break;
			}
			case 0x1B: { // DisableVrMode
				break;
			}
			case 0x1C: { // GetVrMode
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Lbl.Detail.ILblController");
		}
	}
}

