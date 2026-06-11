// ReSharper disable once CheckNamespace

using LibSharpRetro;

namespace UmbraCore.Services.Nn.Lm;

public partial class ILogService {
    protected override ILogger Initialize(ulong _0, ulong _1) => new();
}

public partial class ILogger {
    protected override void Log(Span<byte> _0) {
        // ‡ AV on Linux v0: type=0x21, ACount=0 → falls to X-desc (case 3)
        // which reads garbage at DescOffset when XCount=0 too. Span over
        // arbitrary memory → Memmove crashes (uncatchable AccessViolation).
        // Pre-existing IPC-decode edge, not the current blocker. No-op for now;
        // proper fix is GetSpan returning Span<T>.Empty when the descriptor
        // count for the resolved type is 0. ‡
        // _0.Hexdump;
    }
}