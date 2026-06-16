using System.Diagnostics;

namespace UmbraCore;

// (T6)×69 ×3: single time source for ALL emulator-side
// timestamps. Per sera ·11010 "singleton time object …
// timing is in like 15 different places". Built after
// sera kt[12]×26 (·11292 "timing changes you put in
// place") landed on (W'):
//
// ROOT: my 316ef3d (Jun-12, "MRS fast-path + watchpoint
// v2", the (q11)/kt[28] chapter) changed CNTPCT_EL0
// (op=3 a=0x5f01) from the ReadSr slow-path's process-
// relative `stopwatch.ElapsedMilliseconds × 19200` to
// `Stopwatch.GetTimestamp() × 19_200_000L / Frequency`.
// On Linux, GetTimestamp() = CLOCK_MONOTONIC ns (= host-
// uptime), Frequency = 1e9. At host-uptime > ~480s,
// `uptime_ns × 19.2M` overflows i64; the overflowed
// product cycles positive↔negative every 960.77s of
// host-uptime. In the negative half, `(ulong)(neg/1e9)`
// ≈ 2^64 − a few billion = the game sees CNTPCT ≈
// 1.84e19 ticks (≈ 30,000 years @ 19.2MHz).
//
// Verified at-data (T6)×69×2-cont: @u779 (host-uptime
// 193487s, ~372s into a 960.77s wrap cycle) prod=
// +7.15e18 → cntpct=7.15e9 (sane, ≈372s @ 19.2MHz);
// @u786 (uptime≈263000s) prod=−4.81e18 → cntpct=
// 0xfffffffee16da19c (garbage). u779 worked; u780-786
// all stuck at GameFramework::SystemInit poll-loop
// (t1 spin-polls Svc 0xB SleepThread, ~172K iterations,
// never satisfies). own ‡v0 ×10th-PROPER (own commit).
//
// ⚠ kt[39]: mechanism overflow→SystemInit-stuck is
// ‡INFERRED (n=6 all-stuck under a 50%-positive-window
// theory = 1.6% unlikely ⟹ likely a SECOND factor:
// the epoch-mismatch with NvnLinux.NowNs() [process-
// relative] meant game code mixing CPU-tick-time with
// GPU-timestamp-time [= ReportCounter, writes NowNs()]
// saw nonsense deltas even in the positive-product
// window). Both factors fixed here. u787 = the test.
// The overflow is a REAL bug regardless of (W')-
// causation (verified arithmetically + at-data).
//
// This class is THE epoch. Everything that returns time
// to the game — CNTPCT/CNTVCT via MRS op=3, Svc 0x1E
// GetSystemTick, nvn ReportCounter/DeviceGetTimestampNs/
// DeviceGetCurrentTimestampNs — reads from here.
// Process-relative (= repeatable per sera ·11004 "key
// off time rather than frames … repeatability in what
// you're capturing"); Int128 intermediate (= no overflow
// until process-uptime > ~30Ky).
//
// ‡ NOT yet wired: ISystemClock/ISteadyClock IPC (Nn.
// Timesrv stubs throw NotImplemented; game doesn't call
// them per u779-786 logs). ‡ Time-warp/scale knob (=
// sera ·11010 "do whatever fuckery we want") = ×70+.
public static class UmbraTime {
    static readonly long _t0 = Stopwatch.GetTimestamp();
    static readonly long _freq = Stopwatch.Frequency;
    public const ulong SwitchFreq = 19_200_000;

    // Raw host-resolution ticks since process start.
    // (= the same value NvnLinux's old _clock.Elapsed
    // Ticks gave; both are GetTimestamp()−t0.)
    public static long ElapsedHostTicks()
        => Stopwatch.GetTimestamp() - _t0;

    // 19.2MHz Switch system-counter ticks since process
    // start. = what CNTPCT_EL0/CNTVCT_EL0 (MRS op=3
    // a=0x5f01/0x5f02) + Svc 0x1E (GetSystemTick) return
    // on real hardware. Int128 intermediate ⟹ no
    // overflow (the 316ef3d bug) until process-uptime ×
    // 19.2M > 2^127 (≈ 30Ky at Frequency=1e9).
    public static ulong Ticks()
        => (ulong)((Int128)ElapsedHostTicks() * SwitchFreq / _freq);

    // Nanoseconds since process start. = what nvnDevice
    // GetCurrentTimestampInNanoseconds + ReportCounter's
    // timestamp field hold. Same epoch as Ticks() (=
    // Ticks() × 1e9 / 19.2M, modulo rounding) ⟹ game
    // code that mixes CPU-tick-derived and GPU-timestamp
    // -derived time sees consistent values.
    public static ulong Ns()
        => (ulong)((Int128)ElapsedHostTicks() * 1_000_000_000 / _freq);
}
