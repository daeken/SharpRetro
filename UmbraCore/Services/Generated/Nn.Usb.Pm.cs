using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Usb.Pm;
public partial class IPmService : _IPmService_Base;
public abstract class _IPmService_Base : IpcInterface {
	protected virtual KObject Unknown0() =>
		throw new NotImplementedException("Nn.Usb.Pm.IPmService.Unknown0 not implemented");
	protected virtual void Unknown1(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Usb.Pm.IPmService.Unknown1 not implemented");
	protected virtual KObject Unknown2() =>
		throw new NotImplementedException("Nn.Usb.Pm.IPmService.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Usb.Pm.IPmService.Unknown3 not implemented");
	protected virtual void Unknown4(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Usb.Pm.IPmService.Unknown4");
	protected virtual void Unknown5(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Usb.Pm.IPmService.Unknown5 not implemented");
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
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Usb.Pm.IPmService");
		}
	}
}

