using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Grcsrv;
public partial class IContinuousRecorder : _IContinuousRecorder_Base;
public abstract class _IContinuousRecorder_Base : IpcInterface {
	protected virtual void Unknown1() =>
		Console.WriteLine("Stub hit for Nn.Grcsrv.IContinuousRecorder.Unknown1");
	protected virtual void Unknown2() =>
		Console.WriteLine("Stub hit for Nn.Grcsrv.IContinuousRecorder.Unknown2");
	protected virtual KObject Unknown10() =>
		throw new NotImplementedException("Nn.Grcsrv.IContinuousRecorder.Unknown10 not implemented");
	protected virtual void Unknown11() =>
		Console.WriteLine("Stub hit for Nn.Grcsrv.IContinuousRecorder.Unknown11");
	protected virtual void Unknown12() =>
		Console.WriteLine("Stub hit for Nn.Grcsrv.IContinuousRecorder.Unknown12");
	protected virtual void Unknown13(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Grcsrv.IContinuousRecorder.Unknown13");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: { // Unknown1
				break;
			}
			case 0x2: { // Unknown2
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
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Grcsrv.IContinuousRecorder");
		}
	}
}

public partial class IGameMovieTrimmer : _IGameMovieTrimmer_Base;
public abstract class _IGameMovieTrimmer_Base : IpcInterface {
	protected virtual void BeginTrim(uint _0, uint _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Grcsrv.IGameMovieTrimmer.BeginTrim");
	protected virtual void EndTrim(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Grcsrv.IGameMovieTrimmer.EndTrim not implemented");
	protected virtual KObject GetNotTrimmingEvent() =>
		throw new NotImplementedException("Nn.Grcsrv.IGameMovieTrimmer.GetNotTrimmingEvent not implemented");
	protected virtual void SetThumbnailRgba(uint _0, uint _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Grcsrv.IGameMovieTrimmer.SetThumbnailRgba");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: { // BeginTrim
				break;
			}
			case 0x2: { // EndTrim
				break;
			}
			case 0xA: { // GetNotTrimmingEvent
				break;
			}
			case 0x14: { // SetThumbnailRgba
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Grcsrv.IGameMovieTrimmer");
		}
	}
}

public partial class IGrcService : _IGrcService_Base;
public abstract class _IGrcService_Base : IpcInterface {
	protected virtual Nn.Grcsrv.IContinuousRecorder OpenContinuousRecorder(Span<byte> _0, KObject _1) =>
		throw new NotImplementedException("Nn.Grcsrv.IGrcService.OpenContinuousRecorder not implemented");
	protected virtual Nn.Grcsrv.IGameMovieTrimmer OpenGameMovieTrimmer(Span<byte> _0, KObject _1) =>
		throw new NotImplementedException("Nn.Grcsrv.IGrcService.OpenGameMovieTrimmer not implemented");
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: { // OpenContinuousRecorder
				break;
			}
			case 0x2: { // OpenGameMovieTrimmer
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Grcsrv.IGrcService");
		}
	}
}

