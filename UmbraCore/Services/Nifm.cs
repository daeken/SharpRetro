using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nifm.Detail;

public partial class IRequest {
    protected override void GetSystemEventReadableHandles(out KObject _0, out KObject _1) {
        _0 = new Event(triggered: true);
        _1 = new Event(triggered: true);
    }

    protected override uint GetRequestState() => 4; // Blocking
    protected override void GetResult() {
        //Thread.Sleep(10000); // TODO: Actually unfuck nifm, but for now this works
    }
}

public partial class IGeneralService {
    protected override IRequest CreateRequest(uint _0) => new();
}

public partial class IStaticService {
    protected override IGeneralService CreateGeneralServiceOld() => new();
}