using UmbraCore.Core;

namespace UmbraCore.Services.Nn.Aocsrv.Detail;

// : NuDLCManager asks how many DLC.
// v0 = 0 (game has DLC1-5.DAT in romfs but returning 0 means it won't
// try to enumerate/mount them; safest path past the wall). + the rest
// of the family pre-stubbed since game will likely walk Count→List→
// BaseId→ChangedEvent in sequence.
public partial class IAddOnContentManager {
    protected override uint CountAddOnContent(ulong _0, ulong _1) {
        "[aoc] CountAddOnContent → 0".Log();
        return 0;
    }
    protected override uint CountAddOnContentByApplicationId(ulong _0) => 0;
    protected override void ListAddOnContent(uint offset, uint count,
            ulong _2, ulong _3, out uint outCount, Span<uint> buf) {
        $"[aoc] ListAddOnContent(off={offset}, n={count}) → 0".Log();
        outCount = 0;
    }
    protected override void ListAddOnContentByApplicationId(uint offset,
            uint count, ulong _2, out uint outCount, Span<uint> buf) {
        outCount = 0;
    }
    protected override ulong GetAddOnContentBaseId(ulong _0, ulong _1) {
        "[aoc] GetAddOnContentBaseId → 0x1000".Log();
        return 0x1000;  // ‡ titleId-base for DLC indices; game adds idx
    }
    protected override ulong GetAddOnContentBaseIdByApplicationId(ulong _0) => 0x1000;
    protected override KObject GetAddOnContentListChangedEvent() {
        "[aoc] GetAddOnContentListChangedEvent → never-fires KEvent".Log();
        return new Event();  // never signaled = DLC list never changes
    }
}
