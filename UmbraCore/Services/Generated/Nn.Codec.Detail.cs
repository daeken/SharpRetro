using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Codec.Detail;
public partial class JpegDecoder : _JpegDecoder_Base;
public abstract class _JpegDecoder_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0xBB9: // Unknown3001
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Codec.Detail.JpegDecoder");
		}
	}
}

public partial class IHardwareOpusDecoder : _IHardwareOpusDecoder_Base;
public abstract class _IHardwareOpusDecoder_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // DecodeInterleaved
				break;
			case 0x1: // SetContext
				break;
			case 0x2: // Unknown2
				break;
			case 0x3: // Unknown3
				break;
			case 0x4: // Unknown4
				break;
			case 0x5: // Unknown5
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Codec.Detail.IHardwareOpusDecoder");
		}
	}
}

public partial class IHardwareOpusDecoderManager : _IHardwareOpusDecoderManager_Base;
public abstract class _IHardwareOpusDecoderManager_Base : IpcInterface {
	protected override void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: // Initialize
				break;
			case 0x1: // GetWorkBufferSize
				break;
			case 0x2: // InitializeMultiStream
				break;
			case 0x3: // GetWorkBufferSizeMultiStream
				break;
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Codec.Detail.IHardwareOpusDecoderManager");
		}
	}
}

