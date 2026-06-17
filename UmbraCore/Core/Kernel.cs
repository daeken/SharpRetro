namespace UmbraCore.Core;

public static class Kernel {
    public static uint HandleIter;
    public static readonly Dictionary<uint, KObject> Handles = [];
    public static readonly Dictionary<(ulong Start, ulong End), string> Symbols = [];

    public static string RomFsPath;
    public static bool IsNative;
    // (T6)×72: module[0] (= main game module) LoadBase, set
    // by MainLoop after module-load. For instruments that
    // need to read game .data globals at fixed module-
    // relative offsets (= the C33/C36 cutscene instruments,
    // which had hardcoded 0xfff5b… addrs from u779's specific
    // ASLR layout — own ‡v0×10th). With this, those addrs are
    // layout-independent (no setarch -R needed). + log it at
    // [boot] so future env-reconstruction (= the kt[40] class)
    // has it.
    public static ulong MainBase;
    public static readonly HookManager HookManager = new();
    public static readonly IpcManager IpcManager = new();
    public static readonly MemoryManager MemoryManager = new();
    public static readonly ThreadManager ThreadManager = new();
    public static readonly MiscManager MiscManager = new();
    public static readonly SyncManager SyncManager = new();
    public static readonly Renderer Renderer = new();
    
    public static void Setup(GameWrapper game, string romFsPath) {
        RomFsPath = romFsPath;
        IpcManager.Setup(game);
        MemoryManager.Setup(game);
        ThreadManager.Setup(game);
        MiscManager.Setup(game);
        SyncManager.Setup(game);

        game.Callbacks.CloseHandle = handle => {
            if(Handles.TryGetValue((uint) handle, out var obj))
                Close(obj);
            return 0;
        };
    }
    
    public static uint Add(KObject obj) {
        lock(Handles) {
            Handles[++HandleIter] = obj;
            return HandleIter;
        }
    }

    public static void Close(KObject obj) {
        obj.Close();
        lock(Handles) {
            Handles.Remove(obj.Handle);
        }
    }

    public static T Get<T>(ulong handle) where T : KObject => Get<T>((uint) handle);
    public static T Get<T>(uint handle) where T : KObject => Handles.TryGetValue(handle, out var obj) ? obj as T : null;

    public static unsafe void StackTrace(ulong* fp) {
        "Stack trace:".Log();
        while(fp != null) {
            var lr = fp[1];
            if(MemoryManager.IsKnownPointer(lr)) {
                var rebased = MemoryManager.Regions.First(x => x.Key <= lr && lr < x.Key + x.Value.Size).Key;
                rebased = lr - rebased + 0x71_00000000;
                
                var symbol = Symbols.FirstOrDefault(x => x.Key.Start <= lr && lr < x.Key.End);
                if(symbol.Value != null) {
                    var name = symbol.Value;
                    if(name.StartsWith("_Z"))
                        name = CxxDemangler.CxxDemangler.Demangle(name);
                    $"- 0x{rebased:X} - {name}".Log();
                } else
                    $"- 0x{rebased:X}".Log();
            }
            var nfp = *fp;
            if(nfp < (ulong) fp || nfp == (ulong) fp) break;
            var stride = nfp - (ulong) fp;
            if(stride > 0x10000) break;
            fp = (ulong*) nfp;
        }
    }
}