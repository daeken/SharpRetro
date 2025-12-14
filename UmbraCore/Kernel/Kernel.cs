namespace UmbraCore.Kernel;

public static class Kernel {
    public static readonly HookManager HookManager = new();
    public static readonly IpcManager IpcManager = new();
    public static readonly MemoryManager MemoryManager = new();
    public static readonly ThreadManager ThreadManager = new();
}