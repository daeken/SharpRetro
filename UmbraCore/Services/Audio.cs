using UmbraCore.Core;

// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Audio.Detail;

public partial class IAudioRenderer {
    protected override KObject QuerySystemEvent() => new Event();
    protected override void RequestUpdateAudioRenderer(Span<AudioRendererUpdateDataHeader> _0, Span<AudioRendererUpdateDataHeader> _1, Span<AudioRendererUpdateDataHeader> _2) {
        "IAudioRenderer.RequestUpdateAudioRenderer hit".Log();
    }
}

public partial class IAudioRendererManager {
    protected override ulong GetWorkBufferSize(AudioRendererParameterInternal _0) => 0x100; // Sure!

    protected override IAudioRenderer OpenAudioRenderer(AudioRendererParameterInternal _0, ulong _1, ulong _2, ulong _3, KObject _4, KObject _5) => new();
}
