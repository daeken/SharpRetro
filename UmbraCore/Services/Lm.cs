// ReSharper disable once CheckNamespace

using LibSharpRetro;

namespace UmbraCore.Services.Nn.Lm;

public partial class ILogService {
    protected override ILogger Initialize(ulong _0, ulong _1) => new();
}

public partial class ILogger {
    protected override void Log(Span<byte> _0) {
        _0.Hexdump();
    }
}