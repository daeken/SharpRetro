using UmbraCore.Core;

namespace UmbraCore.Services.Nn.Codec.Detail;

// hwopus: hardware Opus decoder. Game requests this for title-
// screen / level music. Stub: return a workbuffer size, hand
// back a no-op decoder that produces silence. The game's audio
// path doesn't gate progression (it submits decoded PCM to the
// audren mixer; if PCM is zeros, silence).
public partial class IHardwareOpusDecoderManager {
    // _0 = OpusParametersSimple {int sampleRate; int channelCount}.
    // Real impl computes from libopus state size; the game passes
    // this size to CreateTransferMemory then back to Initialize.
    // 0x4000 is plenty for a stub (game allocates it, we don't
    // touch it).
    protected override uint GetWorkBufferSize(byte[] _0) {
        var rate = BitConverter.ToInt32(_0, 0);
        var ch = BitConverter.ToInt32(_0, 4);
        $"[hwopus] GetWorkBufferSize rate={rate} ch={ch} → 0x4000".Log();
        return 0x4000;
    }
    protected override IHardwareOpusDecoder Initialize(
            byte[] _0, uint sz, KObject tmem) {
        $"[hwopus] Initialize sz=0x{sz:X} tmem={tmem}".Log();
        return new();
    }
}

public partial class IHardwareOpusDecoder {
    // DecodeInterleaved: in=Opus packet, out=PCM s16 interleaved.
    // Stub: zero the output (= silence), report decoded sample
    // count from the Opus packet header (TOC byte → frame size).
    // ‡ The game may compute its own bytes-from-samples and read
    // past zero-fill if we underreport — see if it asserts.
    protected override void DecodeInterleaved(
            Span<byte> opus, out uint nSamples,
            out uint nConsumed, Span<byte> pcm) {
        pcm.Clear();
        // Opus TOC byte → audiosamples-per-frame at 48kHz.
        // Use 960 (= 20ms@48k, the common case) as a fixed lie;
        // the game just feeds it to audren.
        nSamples = 960;
        nConsumed = (uint)opus.Length;
    }
}
