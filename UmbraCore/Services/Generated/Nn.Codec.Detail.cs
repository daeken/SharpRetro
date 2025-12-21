using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Codec.Detail;
public partial class JpegDecoder : _JpegDecoder_Base;
public abstract class _JpegDecoder_Base : IpcInterface {
	protected virtual void Unknown3001(Span<byte> _0, Span<byte> _1) =>
		throw new NotImplementedException("Nn.Codec.Detail.JpegDecoder.Unknown3001 not implemented");
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
	protected virtual void DecodeInterleaved(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Codec.Detail.IHardwareOpusDecoder.DecodeInterleaved not implemented");
	protected virtual void SetContext(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Codec.Detail.IHardwareOpusDecoder.SetContext");
	protected virtual void Unknown2(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Codec.Detail.IHardwareOpusDecoder.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0) =>
		Console.WriteLine("Stub hit for Nn.Codec.Detail.IHardwareOpusDecoder.Unknown3");
	protected virtual void Unknown4(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Codec.Detail.IHardwareOpusDecoder.Unknown4 not implemented");
	protected virtual void Unknown5(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Codec.Detail.IHardwareOpusDecoder.Unknown5 not implemented");
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
	protected virtual Nn.Codec.Detail.IHardwareOpusDecoder Initialize(Span<byte> _0, uint _1, KObject _2) =>
		throw new NotImplementedException("Nn.Codec.Detail.IHardwareOpusDecoderManager.Initialize not implemented");
	protected virtual uint GetWorkBufferSize(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Codec.Detail.IHardwareOpusDecoderManager.GetWorkBufferSize not implemented");
	protected virtual Nn.Codec.Detail.IHardwareOpusDecoder InitializeMultiStream(uint _0, KObject _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Codec.Detail.IHardwareOpusDecoderManager.InitializeMultiStream not implemented");
	protected virtual uint GetWorkBufferSizeMultiStream(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Codec.Detail.IHardwareOpusDecoderManager.GetWorkBufferSizeMultiStream not implemented");
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

