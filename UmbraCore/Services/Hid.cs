using UmbraCore.Core;

// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Hid;

public partial class IHidServer {
    public static readonly KSharedMemory Memory = new(0x40000);
    
    public static uint SupportedStyleSet;
    public static uint[] SupportedNpadIds = [];

    protected override IAppletResource CreateAppletResource(ulong _0, ulong _1) => new();
    protected override void SetSupportedNpadStyleSet(uint v, ulong _0, ulong _1) {
        SupportedStyleSet = v;
        $"[hid] SetSupportedNpadStyleSet 0x{v:X}".Log();
    }
    protected override void SetSupportedNpadIdType(ulong _0, ulong _1, Span<uint> ids) {
        SupportedNpadIds = ids.ToArray();
        $"[hid] SetSupportedNpadIdType [{string.Join(",", ids.ToArray().Select(i => $"0x{i:X}"))}]".Log();
    }
    protected override long GetNpadJoyHoldType(ulong _0, ulong _1) => 0;
    protected override uint GetSupportedNpadStyleSet(ulong _0, ulong _1) => 0b11111;
    protected override IActiveVibrationDeviceList CreateActiveVibrationDeviceList() => new();
    // SendVibrationValue: game sends rumble to assigned pads
    // every frame once the player↔port map is populated. No-op.
    protected override void SendVibrationValue(uint h, ulong _0, ulong _1) { }
    protected override void SendVibrationValues(ulong _0, Span<uint> _1, Span<byte> _2) { }
}

public partial class IAppletResource {
    protected override KObject GetSharedMemoryHandle() => IHidServer.Memory;
}