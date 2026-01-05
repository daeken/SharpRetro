using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Am.Service;

public partial class ISelfController {
    protected override ulong CreateManagedDisplayLayer() {
        "Attempting to create managed display layer".Log();
        return 1;
    }
}

public partial class IWindowController {
    protected override ulong GetAppletResourceUserId() => 0xDEADBEEF_CAFEBABE;
}

public partial class ICommonStateGetter {
    protected override KObject GetEventHandle() => new Event(true);
    protected override uint ReceiveMessage() => 0xF; // Focused
    protected override byte GetCurrentFocusState() => 1;
    protected override byte GetOperationMode() => 1;
    protected override uint GetPerformanceMode() => 1;
}

public partial class IStorageAccessor(byte[] Data) {
    protected override ulong GetSize() => (ulong) Data.Length;
    protected override void Write(ulong _0, Span<byte> _1) => throw new NotImplementedException();
    protected override void Read(ulong offset, Span<byte> span) => Data.CopyTo(span[..Math.Min(span.Length, Data.Length)]);
}

public partial class IStorage(byte[] Data) {
    protected override IStorageAccessor Unknown0() => new(Data);
}

public partial class IApplicationFunctions {
    protected override IStorage PopLaunchParameter(uint _0) {
        var data = new byte[0x88];
        data[0] = 0xCA;
        data[1] = 0x97;
        data[2] = 0x94;
        data[3] = 0xC7;
        data[4] = 1;
        data[8] = 1;
        return new IStorage(data);
    }

    protected override ulong EnsureSaveData(byte[] _0) => 0;

    protected override void GetDesiredLanguage(out byte[] _0) {
        _0 = "en-US\0\0\0"u8.ToArray();
    }
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