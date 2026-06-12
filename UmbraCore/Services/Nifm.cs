using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nifm.Detail;

// (b-nifm) ·7827'd chain (NOTES.md @8ff2d66): IsNetworkAvailable()
// → GetNetworkConnectionPointer → NetworkConnection (= a Request,
// params byte=0x02) → IsAvailable() → Request::GetRequestState
// → RequestClient::GetRequestState (locks g_state+0x30, then IPC
// vtbl[+0x20] = our GetRequestState; vtbl[+0x28] = our GetResult,
// stored at [this+0x8c] but does NOT gate state; returns
// [this+0x88]) → (state == 3). IsRequestOnHold = (state == 2).
//
// ⟹ Our state=1 (= Error) already → IsNetworkAvailable()=false.
//
// Reference (kt[14] read-only, ryujinx-ref IRequest.cs):
//   RequestState{Invalid=0, Error=1, OnHold=2, Available=3}
//   Submit() → Success, signals event[0]
//   GetResult() → Success
//   GetSystemEventReadableHandles → event[0]=request-done,
//     event[1]=‡internet-status; event[0] signaled on Submit.
//
// Model: "no network adapter" — request completes instantly with
// state=Error. SDK polls state; gets 1; IsAvailable=false. The
// game's TTNetwork::Update per-slot enable-flag is set regardless
// at TTNetwork::Initialise (per (q-fix-d)), so this DOESN'T fix
// LEGO's (q) — that's h1f-side structural. This is for-correctness
// per sera's "~everything" (·590).

public partial class IRequest {
    // Event[0] = request-state-changed (signaled on Submit
    // completion). Event[1] = ‡(unused by this game; ref leaves
    // unsignaled). Both pre-signaled here = "already-done by
    // the time you ask" (we resolve synchronously).
    readonly Event _evtDone = new(triggered: true);
    readonly Event _evtStat = new(triggered: true);

    protected override void GetSystemEventReadableHandles(
            out KObject _0, out KObject _1) {
        _0 = _evtDone;
        _1 = _evtStat;
    }

    // 1 = Error (per ref RequestState). The original `// Free`
    // comment was wrong (1≠Free; there is no Free).
    protected override uint GetRequestState() => 1;

    // Was: throw IpcException((1111<<9)|110). RequestClient::
    // GetRequestState stores GetResult's W0 at [this+0x8c]; it's
    // read by SDK error-handlers but doesn't gate state. Throwing
    // returns the code via IpcManager's catch — the SDK then sees
    // a nonzero nn::Result FROM THE IPC CALL (not the same as
    // GetResult returning that code as data). Ref returns Success.
    protected override void GetResult() { /* Success */ }

    // Submit: ref signals event[0] + sets state=3 if a real
    // adapter exists, else state=1 + signals. We're already
    // pre-signaled + state=1; just log.
    protected override void Submit() {
        "[nifm] IRequest.Submit → done (state=Error)".Log();
    }

    protected override void Cancel() {
        "[nifm] IRequest.Cancel".Log();
    }
}

public partial class IGeneralService {
    protected override IRequest CreateRequest(uint _0) => new();

    // ‡ Other IGeneralService methods (GetClientId, IsAnyInternet
    // RequestAccepted, GetCurrentIpAddress, etc.) — the generated
    // void-no-out → "Stub hit".Log() pattern handles them; with-
    // out methods throw NotImplementedException = visible if a
    // game touches them. Override on demand.
}

public partial class IStaticService {
    protected override IGeneralService CreateGeneralService(ulong _0, ulong _1) => new();
    protected override IGeneralService CreateGeneralServiceOld() => new();
}
