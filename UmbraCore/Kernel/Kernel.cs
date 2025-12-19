namespace UmbraCore.Kernel;

public static class Globals {
    public static bool IsNative;
    public static readonly HookManager HookManager = new();
    public static readonly IpcManager IpcManager = new();
    public static readonly MemoryManager MemoryManager = new();
    public static readonly ThreadManager ThreadManager = new();
    public static readonly MiscManager MiscManager = new();

    public static void Setup(GameWrapper game) {
        MemoryManager.Setup(game);
        ThreadManager.Setup(game);
        MiscManager.Setup(game);
    }
}