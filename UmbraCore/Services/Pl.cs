using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pl.Detail;

public partial class ISharedFontManager {
    static readonly byte[] FontData = new byte[0x1100000];
    static readonly Dictionary<uint, (uint Offset, uint Size)> FontInfo = [];

    protected override void RequestLoad(uint _0) {
        if(FontInfo.ContainsKey(_0)) return;
        var noff = FontInfo.Count == 0 ? 0U : FontInfo.Select(x => x.Value.Offset + x.Value.Size).Max();
        var fn = "openfonts/" + _0 switch {
            0 => "FontStandard", // Open Sans Regular
            1 => "FontChineseSimplified", // Open Sans Regular
            2 => "FontExtendedChineseSimplified", // Source Sans Pro Regular
            3 => "FontChineseTraditional", // Open Sans Light
            4 => "FontKorean", // Open Sans Regular
            5 => "FontNintendoExtended", // Roboto Medium
            _ => throw new NotImplementedException($"Unknown font {_0}"),
        };
        var data = File.ReadAllBytes(fn);
        data.CopyTo(FontData, noff);
        FontInfo[_0] = (noff, (uint) data.Length);
    }
    protected override uint GetLoadState(uint _0) {
        if(!FontInfo.ContainsKey(_0))
            RequestLoad(_0);
        return 1;
    }

    protected override KObject GetSharedMemoryNativeHandle() => new KSharedMemory(FontData);
    protected override uint GetSharedMemoryAddressOffset(uint _0) => FontInfo[_0].Offset;
    protected override uint GetSize(uint _0) => FontInfo[_0].Size;
}