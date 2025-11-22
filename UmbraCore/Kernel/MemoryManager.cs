namespace UmbraCore.Kernel;

public class MemoryManager {
    public ulong Heap;
    public readonly Dictionary<ulong, (ulong Size, ulong Flags)> Regions = [];

    void MapAt(ulong addr, ulong size) => throw new NotImplementedException();
    ulong Alloc(ulong size) => throw new NotImplementedException();
    void Free(ulong addr) => throw new NotImplementedException();
}