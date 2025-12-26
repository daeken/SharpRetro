using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nifm.Detail;

public partial class IGeneralService {
    protected override IRequest CreateRequest(uint _0) => new();
}

public partial class IStaticService {
    protected override IGeneralService CreateGeneralServiceOld() => new();
}