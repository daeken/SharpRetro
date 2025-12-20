using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Grcsrv;
public partial class IContinuousRecorder : _IContinuousRecorder_Base;
public abstract class _IContinuousRecorder_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: // Unknown1
				break;
			case 0x2: // Unknown2
				break;
			case 0xA: // Unknown10
				break;
			case 0xB: // Unknown11
				break;
			case 0xC: // Unknown12
				break;
			case 0xD: // Unknown13
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Grcsrv.IContinuousRecorder");
		}
	}
}

public partial class IGameMovieTrimmer : _IGameMovieTrimmer_Base;
public abstract class _IGameMovieTrimmer_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: // BeginTrim
				break;
			case 0x2: // EndTrim
				break;
			case 0xA: // GetNotTrimmingEvent
				break;
			case 0x14: // SetThumbnailRgba
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Grcsrv.IGameMovieTrimmer");
		}
	}
}

public partial class IGrcService : _IGrcService_Base;
public abstract class _IGrcService_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x1: // OpenContinuousRecorder
				break;
			case 0x2: // OpenGameMovieTrimmer
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Grcsrv.IGrcService");
		}
	}
}

