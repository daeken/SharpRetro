using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Ssl.Sf;

public partial class ISslService {
    protected override ISslContext CreateContext(uint _0, ulong _1, ulong _2) => new();
}
