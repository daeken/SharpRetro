using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Codec.Detail;
public partial class JpegDecoder : _JpegDecoder_Base;
public abstract class _JpegDecoder_Base : IpcInterface {
	protected virtual void Unknown3001(byte[] _0, Span<byte> _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Codec.Detail.JpegDecoder.Unknown3001 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0xBB9: { // Unknown3001
				Unknown3001(im.GetBytes(8, 0x28), im.GetSpan<byte>(0x5, 0), im.GetSpan<byte>(0x46, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Codec.Detail.JpegDecoder");
		}
	}
}

public partial class IHardwareOpusDecoder : _IHardwareOpusDecoder_Base;
public abstract class _IHardwareOpusDecoder_Base : IpcInterface {
	protected virtual void DecodeInterleaved(Span<byte> _0, out uint _1, out uint _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Codec.Detail.IHardwareOpusDecoder.DecodeInterleaved not implemented");
	protected virtual void SetContext(Span<byte> _0) =>
		"Stub hit for Nn.Codec.Detail.IHardwareOpusDecoder.SetContext".Log();
	protected virtual void Unknown2(Span<byte> _0, out uint _1, out uint _2, Span<byte> _3) =>
		throw new NotImplementedException("Nn.Codec.Detail.IHardwareOpusDecoder.Unknown2 not implemented");
	protected virtual void Unknown3(Span<byte> _0) =>
		"Stub hit for Nn.Codec.Detail.IHardwareOpusDecoder.Unknown3".Log();
	protected virtual void Unknown4(Span<byte> _0, out uint _1, out uint _2, out ulong _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Codec.Detail.IHardwareOpusDecoder.Unknown4 not implemented");
	protected virtual void Unknown5(Span<byte> _0, out uint _1, out uint _2, out ulong _3, Span<byte> _4) =>
		throw new NotImplementedException("Nn.Codec.Detail.IHardwareOpusDecoder.Unknown5 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // DecodeInterleaved
				DecodeInterleaved(im.GetSpan<byte>(0x5, 0), out var _0, out var _1, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x1: { // SetContext
				SetContext(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetSpan<byte>(0x5, 0), out var _0, out var _1, im.GetSpan<byte>(0x6, 0));
				om.Initialize(0, 0, 8);
				om.SetData(8, _0);
				om.SetData(12, _1);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(im.GetSpan<byte>(0x5, 0));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4(im.GetSpan<byte>(0x5, 0), out var _0, out var _1, out var _2, im.GetSpan<byte>(0x46, 0));
				om.Initialize(0, 0, 16);
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				break;
			}
			case 0x5: { // Unknown5
				Unknown5(im.GetSpan<byte>(0x5, 0), out var _0, out var _1, out var _2, im.GetSpan<byte>(0x46, 0));
				om.Initialize(0, 0, 16);
				om.SetData(8, _0);
				om.SetData(12, _1);
				om.SetData(16, _2);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Codec.Detail.IHardwareOpusDecoder");
		}
	}
}

public partial class IHardwareOpusDecoderManager : _IHardwareOpusDecoderManager_Base {
	public readonly string ServiceName;
	public IHardwareOpusDecoderManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IHardwareOpusDecoderManager_Base : IpcInterface {
	protected virtual Nn.Codec.Detail.IHardwareOpusDecoder Initialize(byte[] _0, uint _1, KObject _2) =>
		throw new NotImplementedException("Nn.Codec.Detail.IHardwareOpusDecoderManager.Initialize not implemented");
	protected virtual uint GetWorkBufferSize(byte[] _0) =>
		throw new NotImplementedException("Nn.Codec.Detail.IHardwareOpusDecoderManager.GetWorkBufferSize not implemented");
	protected virtual Nn.Codec.Detail.IHardwareOpusDecoder InitializeMultiStream(uint _0, KObject _1, Span<byte> _2) =>
		throw new NotImplementedException("Nn.Codec.Detail.IHardwareOpusDecoderManager.InitializeMultiStream not implemented");
	protected virtual uint GetWorkBufferSizeMultiStream(Span<byte> _0) =>
		throw new NotImplementedException("Nn.Codec.Detail.IHardwareOpusDecoderManager.GetWorkBufferSizeMultiStream not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // Initialize
				var _return = Initialize(im.GetBytes(8, 0x8), im.GetData<uint>(16), Kernel.Get<KObject>(im.GetCopy(0)));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x1: { // GetWorkBufferSize
				var _return = GetWorkBufferSize(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x2: { // InitializeMultiStream
				var _return = InitializeMultiStream(im.GetData<uint>(8), Kernel.Get<KObject>(im.GetCopy(0)), im.GetSpan<byte>(0x19, 0));
				om.Initialize(1, 0, 0);
				om.Move(0, CreateHandle(_return));
				break;
			}
			case 0x3: { // GetWorkBufferSizeMultiStream
				var _return = GetWorkBufferSizeMultiStream(im.GetSpan<byte>(0x19, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Codec.Detail.IHardwareOpusDecoderManager");
		}
	}
}

