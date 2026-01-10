using UmbraCore.Core;

// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Hid;

public partial class IHidServer {
    public static readonly KSharedMemory Memory = new(0x40000);
    
    protected override IAppletResource CreateAppletResource(ulong _0, ulong _1) => new();
    protected override long GetNpadJoyHoldType(ulong _0, ulong _1) => 0;
    protected override uint GetSupportedNpadStyleSet(ulong _0, ulong _1) => 0b11111;
    protected override IActiveVibrationDeviceList CreateActiveVibrationDeviceList() => new();
}

public partial class IAppletResource {
    protected override KObject GetSharedMemoryHandle() => IHidServer.Memory;
}