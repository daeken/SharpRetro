using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pdm.Detail;
public partial class INotifyService : _INotifyService_Base;
public abstract class _INotifyService_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pdm.Detail.INotifyService.Unknown0");
	protected virtual void Unknown2(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pdm.Detail.INotifyService.Unknown2");
	protected virtual void Unknown3(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pdm.Detail.INotifyService.Unknown3");
	protected virtual void Unknown4() =>
		Console.WriteLine("Stub hit for Nn.Pdm.Detail.INotifyService.Unknown4");
	protected virtual void Unknown5(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pdm.Detail.INotifyService.Unknown5");
	protected virtual void Unknown6(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pdm.Detail.INotifyService.Unknown6");
	protected virtual void Unknown7(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Pdm.Detail.INotifyService.Unknown7");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Unknown0
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			case 0x5: // Unknown5
				break;
			case 0x6: // Unknown6
				break;
			case 0x7: // Unknown7
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pdm.Detail.INotifyService");
		}
	}
}

public partial class IQueryService : _IQueryService_Base;
public abstract class _IQueryService_Base : IpcInterface {
	protected virtual void Unknown0(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown0 not implemented");
	protected virtual void Unknown1() =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown1 not implemented");
	protected virtual void Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown3 not implemented");
	protected virtual void Unknown4(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown4 not implemented");
	protected virtual void Unknown5(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown5 not implemented");
	protected virtual void Unknown6(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown6 not implemented");
	protected virtual void Unknown7(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown7 not implemented");
	protected virtual void Unknown8(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown8 not implemented");
	protected virtual void Unknown9() =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown9 not implemented");
	protected virtual void Unknown10(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown10 not implemented");
	protected virtual void Unknown11(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown11 not implemented");
	protected virtual void Unknown12(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Pdm.Detail.IQueryService.Unknown12 not implemented");
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
			case 0x5: // Unknown5
				break;
			case 0x6: // Unknown6
				break;
			case 0x7: // Unknown7
				break;
			case 0x8: // Unknown8
				break;
			case 0x9: // Unknown9
				break;
			case 0xA: // Unknown10
				break;
			case 0xB: // Unknown11
				break;
			case 0xC: // Unknown12
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Pdm.Detail.IQueryService");
		}
	}
}

