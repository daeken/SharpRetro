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
	protected virtual void Unknown13(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Grcsrv.IContinuousRecorder.Unknown13");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
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
			case 0xA: { // Unknown10
				om.Initialize(0, 1, 0);
				var _return = Unknown10();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0xB: { // Unknown11
				om.Initialize(0, 0, 0);
				Unknown11();
				break;
			}
			case 0xC: { // Unknown12
				om.Initialize(0, 0, 0);
				Unknown12();
				break;
			}
			case 0xD: { // Unknown13
				om.Initialize(0, 0, 0);
				Unknown13(im.GetBytes(8, 0x8));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Grcsrv.IContinuousRecorder");
		}
	}
}

public partial class IGameMovieTrimmer : _IGameMovieTrimmer_Base;
public abstract class _IGameMovieTrimmer_Base : IpcInterface {
	protected virtual void BeginTrim(uint _0, uint _1, byte[] _2) =>
		Console.WriteLine("Stub hit for Nn.Grcsrv.IGameMovieTrimmer.BeginTrim");
	protected virtual void EndTrim(out byte[] _0) =>
		throw new NotImplementedException("Nn.Grcsrv.IGameMovieTrimmer.EndTrim not implemented");
	protected virtual KObject GetNotTrimmingEvent() =>
		throw new NotImplementedException("Nn.Grcsrv.IGameMovieTrimmer.GetNotTrimmingEvent not implemented");
	protected virtual void SetThumbnailRgba(uint _0, uint _1, Span<byte> _2) =>
		Console.WriteLine("Stub hit for Nn.Grcsrv.IGameMovieTrimmer.SetThumbnailRgba");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: { // BeginTrim
				om.Initialize(0, 0, 0);
				BeginTrim(im.GetData<uint>(8), im.GetData<uint>(12), im.GetBytes(16, 0x40));
				break;
			}
			case 0x2: { // EndTrim
				om.Initialize(0, 0, 64);
				EndTrim(out var _0);
				om.SetBytes(8, _0);
				break;
			}
			case 0xA: { // GetNotTrimmingEvent
				om.Initialize(0, 1, 0);
				var _return = GetNotTrimmingEvent();
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x14: { // SetThumbnailRgba
				om.Initialize(0, 0, 0);
				SetThumbnailRgba(im.GetData<uint>(8), im.GetData<uint>(12), im.GetSpan<byte>(0x45, 0));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Grcsrv.IGameMovieTrimmer");
		}
	}
}

public partial class IGrcService : _IGrcService_Base;
public abstract class _IGrcService_Base : IpcInterface {
	protected virtual Nn.Grcsrv.IContinuousRecorder OpenContinuousRecorder(byte[] _0, KObject _1) =>
		throw new NotImplementedException("Nn.Grcsrv.IGrcService.OpenContinuousRecorder not implemented");
	protected virtual Nn.Grcsrv.IGameMovieTrimmer OpenGameMovieTrimmer(byte[] _0, KObject _1) =>
		throw new NotImplementedException("Nn.Grcsrv.IGrcService.OpenGameMovieTrimmer not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: { // OpenContinuousRecorder
				om.Initialize(1, 0, 0);
				var _return = OpenContinuousRecorder(im.GetBytes(8, 0x28), Kernel.Get<KObject>(im.GetCopy(0)));
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x2: { // OpenGameMovieTrimmer
				om.Initialize(1, 0, 0);
				var _return = OpenGameMovieTrimmer(im.GetBytes(8, 0x8), Kernel.Get<KObject>(im.GetCopy(0)));
				om.Move(0, CreateHandle(_return));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Grcsrv.IGrcService");
		}
	}
}

