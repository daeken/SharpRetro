using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nifm.Detail;

public partial class IRequest {
    protected override void GetSystemEventReadableHandles(out KObject _0, out KObject _1) {
        _0 = new Event();
        _1 = new Event();
    }

    protected override uint GetRequestState() => 0;
}

public partial class IGeneralService {
    protected override IRequest CreateRequest(uint _0) => new();
}

public partial class IStaticService {
    protected override IGeneralService CreateGeneralServiceOld() => new();
}