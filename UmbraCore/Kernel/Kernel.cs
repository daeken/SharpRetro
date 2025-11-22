namespace UmbraCore.Kernel;

public static class Kernel {
    public static IpcManager IpcManager = new();
    public static MemoryManager MemoryManager = new();
    public static ThreadManager ThreadManager = new();
}