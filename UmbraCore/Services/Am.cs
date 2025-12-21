using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Am.Service;

public partial class IWindowController {
    protected override ulong GetAppletResourceUserId() => 0xDEADBEEF_CAFEBABE;
}

public partial class ICommonStateGetter {
    protected override KObject GetEventHandle() => new Event(true);
    protected override uint ReceiveMessage() => 0xF; // Focused
    protected override byte GetCurrentFocusState() => 1;
}

public partial class IApplicationProxy {
    protected override IApplicationFunctions GetApplicationFunctions() => new();
    protected override ILibraryAppletCreator GetLibraryAppletCreator() => new();
    protected override ICommonStateGetter GetCommonStateGetter() => new();
    protected override ISelfController GetSelfController() => new();
    protected override IWindowController GetWindowController() => new();
    protected override IAudioController GetAudioController() => new();
    protected override IDisplayController GetDisplayController() => new();
    protected override IDebugFunctions GetDebugFunctions() => new();
}

public partial class IApplicationProxyService {
    protected override IApplicationProxy OpenApplicationProxy(ulong _0, ulong _1, KObject _2) => new();
}