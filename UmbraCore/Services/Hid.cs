// ReSharper disable once CheckNamespace

using UmbraCore.Core;

namespace UmbraCore.Services.Nn.Hid;

public partial class IHidServer {
    public static readonly KSharedMemory Memory = new(0x40000);
    
    protected override IAppletResource CreateAppletResource(ulong _0, ulong _1) => new();
}

public partial class IAppletResource {
    protected override KObject GetSharedMemoryHandle() => IHidServer.Memory;
}