using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pdm.Detail;
public partial class INotifyService : _INotifyService_Base;
public abstract class _INotifyService_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0) =>
		"Stub hit for Nn.Pdm.Detail.INotifyService.Unknown0".Log();
	protected virtual void Unknown2(byte[] _0) =>
		"Stub hit for Nn.Pdm.Detail.INotifyService.Unknown2".Log();
	protected virtual void Unknown3(byte[] _0) =>
		"Stub hit for Nn.Pdm.Detail.INotifyService.Unknown3".Log();
	protected virtual void Unknown4() =>
		"Stub hit for Nn.Pdm.Detail.INotifyService.Unknown4".Log();
	protected virtual void Unknown5(Span<byte> _0) =>
		"Stub hit for Nn.Pdm.Detail.INotifyService.Unknown5".Log();
	protected virtual void Unknown6(byte[] _0) =>
		"Stub hit for Nn.Pdm.Detail.INotifyService.Unknown6".Log();
	protected virtual void Unknown7(byte[] _0) =>
		"Stub hit for Nn.Pdm.Detail.INotifyService.Unknown7".Log();
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(im.GetBytes(8, 0x1));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // Unknown5
				Unknown5(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // Unknown6
				Unknown6(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // Unknown7
				Unknown7(im.GetBytes(8, 0x10));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pdm.Detail.INotifyService");
		}
	}
}

public partial class IQueryService : _IQueryService_Base;
public abstract class _IQueryService_Base : IpcInterface {
	protected virtual void Unknown0(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown0 not implemented");
	protected virtual void Unknown1(out byte[] _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown1 not implemented");
	protected virtual void Unknown2(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown2 not implemented");
	protected virtual void Unknown3(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown3 not implemented");
	protected virtual void Unknown4(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown4 not implemented");
	protected virtual void Unknown5(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown5 not implemented");
	protected virtual void Unknown6(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown6 not implemented");
	protected virtual void Unknown7(Span<byte> _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown7 not implemented");
	protected virtual void Unknown8(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown8 not implemented");
	protected virtual void Unknown9(out byte[] _0) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown9 not implemented");
	protected virtual void Unknown10(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown10 not implemented");
	protected virtual void Unknown11(byte[] _0, out byte[] _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown11 not implemented");
	protected virtual void Unknown12(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown12 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Unknown0
				Unknown0(im.GetBytes(8, 0x4), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x1: { // Unknown1
				Unknown1(out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetBytes(8, 0x10), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(im.GetBytes(8, 0x8), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4(im.GetBytes(8, 0x8), out var _0);
				om.Initialize(0, 0, 40);
				om.SetBytes(8, _0);
				break;
			}
			case 0x5: { // Unknown5
				Unknown5(im.GetBytes(8, 0x18), out var _0);
				om.Initialize(0, 0, 40);
				om.SetBytes(8, _0);
				break;
			}
			case 0x6: { // Unknown6
				Unknown6(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 40);
				om.SetBytes(8, _0);
				break;
			}
			case 0x7: { // Unknown7
				Unknown7(im.GetSpan<byte>(0x5, 0), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x8: { // Unknown8
				Unknown8(im.GetBytes(8, 0x4), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0x9: { // Unknown9
				Unknown9(out var _0);
				om.Initialize(0, 0, 12);
				om.SetBytes(8, _0);
				break;
			}
			case 0xA: { // Unknown10
				Unknown10(im.GetBytes(8, 0x4), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0xB: { // Unknown11
				Unknown11(im.GetBytes(8, 0x18), out var _0, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetBytes(8, _0);
				break;
			}
			case 0xC: { // Unknown12
				Unknown12(im.GetBytes(8, 0x10), out var _0);
				om.Initialize(0, 0, 12);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pdm.Detail.IQueryService");
		}
	}
}

