using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nifm.Detail;

public partial class IRequest {
    protected override void GetSystemEventReadableHandles(out KObject _0, out KObject _1) {
        _0 = new Event(triggered: false);
        _1 = new Event(triggered: false);
    }

    protected override uint GetRequestState() => 1; // Free
    protected override void GetResult() {
        // TODO: Actually unfuck nifm, but for now this works
        Thread.Sleep(1000000);
        throw new IpcException((1111 << 9) | 110); // 110=nifm, 1111 == ResultNetworkCommunicationDisabled
    }
}

public partial class IGeneralService {
    protected override IRequest CreateRequest(uint _0) => new();
}

public partial class IStaticService {
    protected override IGeneralService CreateGeneralService(ulong _0, ulong _1) => new();
    protected override IGeneralService CreateGeneralServiceOld() => new();
}