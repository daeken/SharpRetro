using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Settings;

public partial class ISettingsServer {
    protected override void GetAvailableLanguageCodes(out uint _0, Span<byte> _1) {
        _0 = 1;
        "en-US\0"u8.CopyTo(_1);
    }
}
